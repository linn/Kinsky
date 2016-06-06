using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
//using System.Web.Services;
using System.Web.Services.Protocols;
using System.Threading;
using System.Net;
using System.IO;

using System.Diagnostics;

using Linn.Ascii;
using Linn.Network;
using Linn.Control.Http;

namespace Linn.ControlPoint.Upnp
{
    public class EventArgsEvent : EventArgs
    {
        public EventArgsEvent(string aSubscriptionId, uint aSequenceNo, XmlDocument aXml)
        {
            SubscriptionId = aSubscriptionId;
            SequenceNo = aSequenceNo;
            Xml = aXml;
        }

        public string SubscriptionId;
        public uint SequenceNo;
        public XmlDocument Xml;
    }

    public delegate void EventHandlerEvent(EventServerUpnp obj, EventArgsEvent e);

    public interface IEventUpnpProvider
    {
        string Uri { get; }
        void AddSession(string aSubscriptionId, EventHandlerEvent aDelegate);
        void RemoveSession(string aSubscriptionId);
    }

    public class EventServerUpnp : EventServer, IEventUpnpProvider
    {
        public EventServerUpnp()
        {
            iSessions = new Dictionary<string, EventHandlerEvent>();
            iActiveSessionStreams = new List<TcpSessionStream>();
            iEventFifo = new Fifo<Event>();
            iScheduler = new Scheduler("EventServerScheduler", kSchedulerThreads);
        }

        public override void Start(IPAddress aInterface)
        {
            Assert.Check(iThreadNetwork == null && iThreadHandle == null);

            iInterface = aInterface;

            iServer = new TcpServer(Interface);
            iUri = "http://" + iServer.Endpoint.ToString() + "/event";

            iScheduler.Start();

            iThreadHandleAbort = false;
            iThreadHandle = new Thread(RunHandle);
            iThreadHandle.Priority = kPriority;
            iThreadHandle.Name = "Upnp Event Server Handling";
            iThreadHandle.Start();

            iThreadNetworkAbort = false;
            iThreadNetwork = new Thread(RunNetwork);
            iThreadNetwork.Priority = kPriority;
            iThreadNetwork.Name = "Upnp Event Server Network";
            iThreadNetwork.Start();

            Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.Start() successful");
        }

        public override void Stop()
        {
            if (iThreadHandle != null && iThreadNetwork != null)
            {

                iThreadNetworkAbort = true;
                iServer.Shutdown();
                lock (iActiveSessionStreams)
                {
                    for (int i = 0; i < iActiveSessionStreams.Count; i++)
                    {
                        iActiveSessionStreams[i].Close();
                    }
                    iActiveSessionStreams.Clear();
                }
                iScheduler.Stop();
                iThreadNetwork.Join();
                iThreadNetwork = null;
                iServer.Close();

                iThreadHandleAbort = true;
                iEventFifo.Push(null);
                iThreadHandle.Join();
                iThreadHandle = null;

                lock (iSessions)
                {
                    iSessions.Clear();
                }

                iEventFifo = new Fifo<Event>();

                Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.Stop() successful");
            }
            else
            {
                Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.Stop() already stopped - silently do nothing");
            }
        }

        public override IPAddress Interface
        {
            get
            {
                Assert.Check(iInterface != null);
                return iInterface;
            }
        }

        public string Uri
        {
            get
            {
                Assert.Check(iUri != null);
                return iUri;
            }
        }

        private void RunNetwork()
        {
            Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunNetwork() started");

            try
            {
                while (!iThreadNetworkAbort)
                {
                    try
                    {
                        TcpSessionStream sessionStream = new TcpSessionStream();

                        // listen for new incoming event
                        iServer.Accept(sessionStream);
                        lock (iActiveSessionStreams)
                        {
                            iActiveSessionStreams.Add(sessionStream);
                        }

                        // set the timeout for the call to receive - this prevents event server
                        // threads from getting indefinitely blocked by remote clients
                        sessionStream.ReceiveTimeout = 30 * 1000;

                        iScheduler.Schedule((aObject) =>
                        {
                            TcpSessionStream aSessionStream = aObject[0] as TcpSessionStream;
                            if (!iThreadNetworkAbort)
                            {
                                Srb readBuffer = new Srb(kMaxReadBufferBytes, aSessionStream);
                                Swb writeBuffer = new Swb(kMaxWriteBufferBytes, aSessionStream);
                                try
                                {
                                    // read the event request
                                    EventRequest request = new EventRequest();
                                    request.Read(readBuffer);

                                    // get the session for this subscription ID
                                    EventHandlerEvent session = null;
                                    if (request.SubscriptionId != null)
                                    {
                                        session = GetSession(request.SubscriptionId);
                                        if (session == null)
                                        {
                                            // Wait 500mS and try again
                                            Trace.WriteLine(Trace.kUpnp, "EventServerUpnpScheduler: Early Event!     " + request.SubscriptionId);
                                            Thread.Sleep(500);

                                            session = GetSession(request.SubscriptionId);
                                        }
                                    }

                                    // send the event response
                                    request.WriteResponse(writeBuffer, (session != null));

                                    // add event to be handled
                                    if (session != null)
                                    {
                                        Event ev = new Event(request, session);
                                        iEventFifo.Push(ev);
                                    }
                                }
                                catch (HttpError e)
                                {
                                    Trace.WriteLine(Trace.kUpnp, "EventServerUpnpScheduler HttpError: " + e.ToString());
                                    UserLog.WriteLine("EventServerUpnpScheduler HttpError: " + e.ToString());
                                }
                                catch (NetworkError e)
                                {
                                    Trace.WriteLine(Trace.kUpnp, "EventServerUpnpScheduler NetworkError: " + e.ToString());
                                    UserLog.WriteLine("EventServerUpnpScheduler NetworkError: " + e.ToString());
                                }
                                catch (ReaderError e)
                                {
                                    Trace.WriteLine(Trace.kUpnp, "EventServerUpnpScheduler ReaderError: " + e.ToString());
                                    UserLog.WriteLine("EventServerUpnpScheduler ReaderError: " + e.ToString());
                                }
                                catch (WriterError e)
                                {
                                    Trace.WriteLine(Trace.kUpnp, "EventServerUpnpScheduler WriterError: " + e.ToString());
                                    UserLog.WriteLine("EventServerUpnpScheduler WriterError: " + e.ToString());
                                }
                                catch (Exception e)
                                {
                                    Trace.WriteLine(Trace.kUpnp, "EventServerUpnpScheduler Exception: " + e.ToString());
                                    UserLog.WriteLine("EventServerUpnpScheduler Exception: " + e.ToString());
                                    throw;
                                }
                                finally
                                {
                                    lock (iActiveSessionStreams)
                                    {
                                        if (iActiveSessionStreams.Contains(aSessionStream))
                                        {
                                            aSessionStream.Close();
                                            iActiveSessionStreams.Remove(aSessionStream);
                                        }
                                    }
                                }
                            }
                        }, sessionStream);
                    }
                    catch (NetworkError e)
                    {
                        Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunNetwork() NetworkError: " + e.ToString());
                        UserLog.WriteLine("EventServerUpnp.RunNetwork() NetworkError: " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunNetwork() Exception: " + e.ToString());
                UserLog.WriteLine("EventServerUpnp.RunNetwork() Exception: " + e.ToString());
                throw e;
            }

            Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunNetwork() finished");
        }

        private void RunHandle()
        {
            Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunHandle() started");

            while (!iThreadHandleAbort)
            {
                Event ev = iEventFifo.Pop();

                try
                {
                    if (ev != null)
                    {
                        // We are required to take a sub string since Philips streamium has a buffer write error
                        string str = ASCIIEncoding.UTF8.GetString(ev.Request.Body, 0, ev.Request.Body.Length);
                        StringReader reader = new StringReader(str.Substring(0, str.LastIndexOf('>') + 1));

                        XmlDocument document = new XmlDocument();
                        document.Load(reader);

                        Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunHandle() handling event: " + ev.Request.SubscriptionId);
                        ev.Handler(this, new EventArgsEvent(ev.Request.SubscriptionId, ev.Request.SequenceNumber, document));
                    }
                }
                catch (XmlException e)
                {
                    Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunHandle() XmlException: " + ev.Request.SubscriptionId + " " + e.ToString());
                }
                catch (Exception ex)
                {
                    // Logging for #1002, plus any future event server issues
                    string eventXml = string.Empty;
                    if (ev.Request.Body != null)
                    {
                        eventXml = ASCIIEncoding.UTF8.GetString(ev.Request.Body, 0, ev.Request.Body.Length);
                    }
                    UserLog.WriteLine("EventServerUpnp.RunHandle: Exception: " + ev.Request.SubscriptionId + " " + ex.ToString() + " " + eventXml);
                    throw ex;
                }
            }

            Trace.WriteLine(Trace.kUpnp, "EventServerUpnp.RunHandle() finished");
        }

        public void AddSession(string aSubscriptionId, EventHandlerEvent aDelegate)
        {
            lock (iSessions)
            {
                try
                {
                    Trace.WriteLine(Trace.kUpnp, "Event Session+   " + aSubscriptionId);
                    iSessions.Add(aSubscriptionId, aDelegate);
                }
                catch (ArgumentException)
                {
                    // Session already exists - replace the old one with the new one
                    iSessions.Remove(aSubscriptionId);
                    iSessions.Add(aSubscriptionId, aDelegate);
                }
            }
        }

        public void RemoveSession(string aSubscriptionId)
        {
            lock (iSessions)
            {
                try
                {
                    Trace.WriteLine(Trace.kUpnp, "Event Session-   " + aSubscriptionId);
                    iSessions.Remove(aSubscriptionId);
                }
                catch (ArgumentException)
                {
                    Trace.WriteLine(Trace.kUpnp, "Event Session!   " + aSubscriptionId);
                    throw (new EventServerException());
                }
            }
        }

        private EventHandlerEvent GetSession(string aSubscriptionId)
        {
            lock (iSessions)
            {
                EventHandlerEvent session = null;
                iSessions.TryGetValue(aSubscriptionId, out session);
                return session;
            }
        }

        private class Event
        {
            public Event(EventRequest aRequest, EventHandlerEvent aHandler)
            {
                Request = aRequest;
                Handler = aHandler;
            }

            public readonly EventRequest Request;
            public readonly EventHandlerEvent Handler;
        }

        private static readonly int kMaxReadBufferBytes = 50000;
        private static readonly int kMaxWriteBufferBytes = 1000;
        private static readonly ThreadPriority kPriority = ThreadPriority.Normal;

        private Dictionary<string, EventHandlerEvent> iSessions;

        private IPAddress iInterface;
        public string iUri;
        private TcpServer iServer;

        private Fifo<Event> iEventFifo;
        private Scheduler iScheduler;
        private const int kSchedulerThreads = 3;
        private List<TcpSessionStream> iActiveSessionStreams;

        private Thread iThreadNetwork;
        private bool iThreadNetworkAbort;
        private Thread iThreadHandle;
        private bool iThreadHandleAbort;
    }


    internal class EventRequest : IWriterRequest
    {
        public EventRequest()
        {
        }

        public string SubscriptionId
        {
            get { return iSubscriptionIdReceived; }
        }

        public uint SequenceNumber
        {
            get { return iSequenceNo; }
        }

        public byte[] Body
        {
            get { return iBody; }
        }

        public override string ToString()
        {
            string ret = "iMethodReceived: " + iMethodReceived + Environment.NewLine
                       + "iHostReceived: " + iHostReceived + Environment.NewLine
                       + "iContentTypeReceived: " + iContentTypeReceived + Environment.NewLine
                       + "iContentLengthReceived: " + iContentLengthReceived + Environment.NewLine
                       + "iNotificationTypeReceived: " + iNotificationTypeReceived + Environment.NewLine
                       + "iNotificationSubtypeReceived: " + iNotificationSubtypeReceived + Environment.NewLine
                       + "iSubscriptionIdReceived: " + iSubscriptionIdReceived + Environment.NewLine
                       + "iSequenceNo: " + iSequenceNo + Environment.NewLine
                       + "iSequenceNoReceived: " + iSequenceNoReceived + Environment.NewLine
                       + "iTransferEncodingChunked: " + iTransferEncodingChunked + Environment.NewLine
                       + "iBody: " + iBody;
            return ret;
        }

        public void Read(IReader aReader)
        {
            Trace.WriteLine(Trace.kUpnp, "EventRequest.Read() reading event request");

            // reset all event information before reading the event
            iMethodReceived = false;
            iHostReceived = false;
            iContentTypeReceived = false;
            iContentLengthReceived = 0;
            iNotificationTypeReceived = false;
            iNotificationSubtypeReceived = false;
            iSubscriptionIdReceived = null;
            iSequenceNo = 0;
            iSequenceNoReceived = false;
            iTransferEncodingChunked = false;
            iBody = null;

            // create an HTTP reader to read the event
            iReader = aReader;
            ReaderRequest readerHttp = new ReaderRequest(aReader, this);
            readerHttp.Read();
        }

        public void WriteResponse(IWriter aWriter, bool aSubscriptionIdFound)
        {
            WriterResponse response = new WriterResponse(aWriter);

            if (!iMethodReceived ||
                !iHostReceived ||
                !iContentTypeReceived ||
                iSubscriptionIdReceived == null ||
                !iSequenceNoReceived ||
                iBody == null)
            {
                Trace.WriteLine(Trace.kUpnp, "EventRequest.WriteResponse() invalid event: " + this.ToString());
                response.WriteStatus(new Status(412, "Precondition Failed"), EVersion.eHttp11);
            }
            else if (!iNotificationTypeReceived || !iNotificationSubtypeReceived)
            {
                Trace.WriteLine(Trace.kUpnp, "EventRequest.WriteResponse() invalid event: " + this.ToString());
                response.WriteStatus(new Status(400, "Bad Request"), EVersion.eHttp11);
            }
            else if (!aSubscriptionIdFound)
            {
                Trace.WriteLine(Trace.kUpnp, "EventRequest.WriteResponse(): Stray Event!     " + iSubscriptionIdReceived);
                response.WriteStatus(new Status(412, "Precondition Failed"), EVersion.eHttp11);
            }
            else
            {
                Trace.WriteLine(Trace.kUpnp, "EventRequest.WriteResponse(): Event OK!        " + iSubscriptionIdReceived);
                response.WriteStatus(new Status(200, "OK"), EVersion.eHttp11);
            }

            response.WriteFlush();
        }

        void IWriter.Write(byte aValue)
        {
            Assert.Check(false);
        }

        void IWriter.Write(byte[] aBuffer)
        {
            Assert.Check(false);
        }

        void IWriter.WriteFlush()
        {
            if (iTransferEncodingChunked)
            {
                int bytes = 0;
                List<byte[]> chunks = new List<byte[]>();

                while (true)
                {
                    byte[] length = iReader.ReadUntil(Ascii.Ascii.kAsciiLf);

                    try
                    {
                        string slen = ASCIIEncoding.UTF8.GetString(length, 0, length.Length).Trim();

                        if (slen.Length == 0)
                        {
                            break;
                        }

                        int len = Convert.ToInt32(slen, 16);

                        byte[] chunk = iReader.Read(len);

                        chunks.Add(chunk);

                        bytes += len;
                    }
                    catch (FormatException)
                    {
                    }
                    catch (OverflowException)
                    {
                    }
                }

                int index = 0;

                iBody = new byte[bytes];

                foreach (byte[] chunk in chunks)
                {
                    Array.Copy(chunk, 0, iBody, index, chunk.Length);
                    index += chunk.Length;
                }

                iReader.ReadFlush();
            }
            else if (iContentLengthReceived > 0)
            {
                // read the event body
                iBody = iReader.Read(iContentLengthReceived);

                iReader.ReadFlush();
            }
            else
            {
                Trace.WriteLine(Trace.kUpnp, "EventRequest.WriteFlush(): 0-byte Event!    " + iSubscriptionIdReceived);
            }
        }

        // NOTIFY delivery path HTTP/1.1

        void IWriterMethod.WriteMethod(byte[] aMethod, byte[] aUri, EVersion aVersion)
        {
            if (ASCIIEncoding.UTF8.GetString(aMethod, 0, aMethod.Length) == Upnp.kUpnpMethodNotify)
            {
                iMethodReceived = true;
            }
        }

        // HOST: delivery host:delivery port
        // CONTENT-TYPE: text/xml
        // CONTENT-LENGTH: Bytes in body
        // NT: upnp:event
        // NTS: upnp:propchange
        // SID: uuid:subscription-UUID
        // SEQ: event key

        void IWriterHeader.WriteHeader(byte[] aField, byte[] aValue)
        {
            string field = ASCIIEncoding.UTF8.GetString(aField, 0, aField.Length);

            if (String.Compare(field, Upnp.kUpnpHeaderHost, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadHost(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderContentType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadContentType(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderContentLength, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadContentLength(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderNt, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadNt(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderNts, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadNts(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderSid, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadSid(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderSeq, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadSeq(aValue);
                return;
            }

            if (String.Compare(field, Upnp.kUpnpHeaderTransferEncoding, StringComparison.OrdinalIgnoreCase) == 0)
            {
                ReadTransferEncoding(aValue);
                return;
            }
        }

        private void ReadHost(byte[] aValue)
        {
            iHostReceived = true;
        }

        private void ReadContentType(byte[] aValue)
        {
            iContentTypeReceived = true;
        }

        private void ReadContentLength(byte[] aValue)
        {
            try
            {
                iContentLengthReceived = Convert.ToInt32(ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length));
            }
            catch (FormatException)
            {
                iContentLengthReceived = 0;
            }
            catch (OverflowException)
            {
                iContentLengthReceived = 0;
            }
        }

        private void ReadNt(byte[] aValue)
        {
            iNotificationTypeReceived = true;
        }

        private void ReadNts(byte[] aValue)
        {
            iNotificationSubtypeReceived = true;
        }

        private void ReadSid(byte[] aValue)
        {
            string header = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (header.StartsWith(Upnp.kUpnpUuid))
            {
                string sid = header.Substring(Upnp.kUpnpUuid.Length);

                if (sid.Length > 0)
                {
                    iSubscriptionIdReceived = sid;
                }
            }
        }

        private void ReadSeq(byte[] aValue)
        {
            try
            {
                string header = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

                iSequenceNo = Convert.ToUInt32(header);
                iSequenceNoReceived = true;
            }
            catch (FormatException)
            {
                iSequenceNoReceived = false;
            }
            catch (OverflowException)
            {
                iSequenceNoReceived = false;
            }
        }

        private void ReadTransferEncoding(byte[] aValue)
        {
            if (ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length) == Upnp.kUpnpChunked)
            {
                iTransferEncodingChunked = true;
            }
        }

        private IReader iReader;
        private bool iMethodReceived;
        private bool iHostReceived;
        private bool iContentTypeReceived;
        private int iContentLengthReceived;
        private bool iNotificationTypeReceived;
        private bool iNotificationSubtypeReceived;
        private string iSubscriptionIdReceived;
        private uint iSequenceNo;
        private bool iSequenceNoReceived;
        private bool iTransferEncodingChunked;
        private byte[] iBody;
    }
}


