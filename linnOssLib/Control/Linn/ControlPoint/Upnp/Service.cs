using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
//using System.Web.Services;
using System.Web.Services.Protocols;
using System.Threading;
using System.Net;

using System.Diagnostics;

using Linn.Ascii;
using Linn.Network;
using Linn.Control;
using Linn.Control.Http;

namespace Linn.ControlPoint.Upnp
{
    /*
    public class SoapHttpClientProtocol : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        protected SoapHttpClientProtocol(ServiceUpnp aService)
        {
            Url = aService.ControlUri.AbsoluteUri;
            Proxy = aService.WebProxy;
            PreAuthenticate = false;
        }

        protected override WebRequest GetWebRequest(Uri aUri)
        {
            HttpWebRequest request = base.GetWebRequest(aUri) as HttpWebRequest;
            request.KeepAlive = false;
            request.Pipelined = false;
            return (request);
        }
    }

    */

    public class ProtocolUpnp : IProtocol
    {
        public IInvokeHandler CreateInvokeHandler(Service aService, Action aAction)
        {
            return new InvokeHandlerUpnp(aService as ServiceUpnp, aAction);
        }
    }

    public class InvokeHandlerUpnp : IInvokeHandler
    {
        public InvokeHandlerUpnp(ServiceUpnp aService, Action aAction)
        {
            iService = aService;
            iAction = aAction;

            iIndex = -1;
        }

        public void WriteBegin()
        {
            iWriteDocument = new XmlDocument();

            XmlElement envelope = iWriteDocument.CreateElement("s:Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            envelope.SetAttribute("encodingStyle", "http://schemas.xmlsoap.org/soap/envelope/", "http://schemas.xmlsoap.org/soap/encoding/");
            XmlElement body = iWriteDocument.CreateElement("s:Body", "http://schemas.xmlsoap.org/soap/envelope/");

            iActionNode = iWriteDocument.CreateElement(string.Format("u:{0}", iAction.Name), Upnp.ServiceTypeToString(iService.Type));

            body.AppendChild(iActionNode);
            envelope.AppendChild(body);
            iWriteDocument.AppendChild(envelope);
        }

        public void WriteArgumentString(string aName, string aValue)
        {
            XmlElement argument = iWriteDocument.CreateElement(aName);
            argument.InnerText = aValue;
            iActionNode.AppendChild(argument);
        }

        public void WriteArgumentUint(string aName, uint aValue)
        {
            XmlElement argument = iWriteDocument.CreateElement(aName);
            argument.InnerText = aValue.ToString();
            iActionNode.AppendChild(argument);
        }

        public void WriteArgumentBool(string aName, bool aValue)
        {
            XmlElement argument = iWriteDocument.CreateElement(aName);
            argument.InnerText = aValue ? "1" : "0";
            iActionNode.AppendChild(argument);
        }

        public void WriteArgumentInt(string aName, int aValue)
        {
            XmlElement argument = iWriteDocument.CreateElement(aName);
            argument.InnerText = aValue.ToString();
            iActionNode.AppendChild(argument);
        }

        public void WriteArgumentBinary(string aName, byte[] aValue)
        {
            XmlElement argument = iWriteDocument.CreateElement(aName);
            argument.InnerText = Convert.ToBase64String(aValue);
            iActionNode.AppendChild(argument);
        }

        public object WriteEnd(GetResponseStreamCallback aCallback)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] message = encoding.GetBytes(iWriteDocument.OuterXml);

            HttpWebRequest request = null;
            Stream reqStream = null;
            try
            {
                request = WebRequest.Create(iService.ControlUri.AbsoluteUri) as HttpWebRequest;
                request.KeepAlive = false;
                request.Pipelined = false;
                request.Proxy = iService.WebProxy;

                request.Method = "POST";
                request.ContentType = "text/xml; charset=\"utf-8\"";
                request.Headers.Add("SOAPACTION", string.Format("\"{0}#{1}\"", Upnp.ServiceTypeToString(iService.Type), iAction.Name));
                request.ContentLength = message.Length;
                request.Timeout = kControlInvokeTimeout;
                request.ReadWriteTimeout = kControlInvokeTimeout;

                if (aCallback == null)
                {
                    reqStream = request.GetRequestStream();
                    reqStream.Write(message, 0, message.Length);
                }
                else
                {
                    WebRequestPool.QueueJob(new JobSendRequest(aCallback, request, message));
                }
            }
            finally
            {
                if (aCallback == null && reqStream != null)
                {
                    reqStream.Close();
                    reqStream.Dispose();
                }

                iWriteDocument = null;
                iActionNode = null;
            }

            return request;
        }

        public void ReadBegin(object aResult)
        {
            HttpWebRequest request = aResult as HttpWebRequest;

            HttpWebResponse response = null;
            Stream resStream = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                resStream = response.GetResponseStream();

                iReadDocument = new XmlDocument();
                iReadDocument.Load(resStream);

                XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(iReadDocument.NameTable);
                xmlNsMan.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    xmlNsMan.AddNamespace("u", Upnp.ServiceTypeToString(iService.Type));
                    XmlNodeList nodeList = iReadDocument.SelectNodes(string.Format("/s:Envelope/s:Body/u:{0}Response", iAction.Name), xmlNsMan);
                    if (nodeList.Count == 1)
                    {
                        iResponse = nodeList[0].ChildNodes;
                        iIndex = 0;
                    }
                    else
                    {
                        throw new ServiceException(600, "Invalid response: " + iReadDocument.OuterXml);
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    response = e.Response as HttpWebResponse;
                    resStream = response.GetResponseStream();

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(resStream);

                        XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(document.NameTable);
                        xmlNsMan.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
                        xmlNsMan.AddNamespace("c", "urn:schemas-upnp-org:control-1-0");

                        string faultCode = string.Empty; 
                        XmlNode faultCodeNode = document.SelectSingleNode("/s:Envelope/s:Body/s:Fault/faultcode", xmlNsMan); 
                        if (faultCodeNode != null) 
                        { 
                            faultCode = faultCodeNode.InnerText; 
                        } 
                        string faultString = string.Empty; 
                        XmlNode faultStringNode = document.SelectSingleNode("/s:Envelope/s:Body/s:Fault/faultstring", xmlNsMan); 
                        if (faultStringNode != null) 
                        { 
                            faultString = faultStringNode.InnerText; 
                        } 
                        XmlNode detail = document.SelectSingleNode("/s:Envelope/s:Body/s:Fault/detail", xmlNsMan);
                        throw new SoapException(faultString, new XmlQualifiedName(faultCode), string.Empty, detail);
                    }

                    throw new ServiceException(600, "Invalid response: " + response.StatusDescription);
                }

                throw new Exception(e.Status.ToString());
            }
            finally
            {
                if (resStream != null)
                {
                    resStream.Close();
                    resStream.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public string ReadArgumentString(string aName)
        {
            Assert.Check(iResponse[iIndex].Name == aName);
            return iResponse[iIndex++].InnerText;
        }

        public uint ReadArgumentUint(string aName)
        {
            Assert.Check(iResponse[iIndex].Name == aName);
            return Convert.ToUInt32(iResponse[iIndex++].InnerText);
        }

        public bool ReadArgumentBool(string aName)
        {
            Assert.Check(iResponse[iIndex].Name == aName);
            string value = iResponse[iIndex++].InnerText;
            if (value == "0") {
            	return (false);
            }
            if (value == "1") {
            	return (true);
            }
            return Convert.ToBoolean(value);
        }

        public int ReadArgumentInt(string aName)
        {
            Assert.Check(iResponse[iIndex].Name == aName);
            return Convert.ToInt32(iResponse[iIndex++].InnerText);
        }

        public byte[] ReadArgumentBinary(string aName)
        {
            Assert.Check(iResponse[iIndex].Name == aName);
            return Convert.FromBase64String(iResponse[iIndex++].InnerText);
        }

        public void ReadEnd()
        {
            iIndex = -1;
        }

        private static readonly int kControlInvokeTimeout = 100000; // milliseconds

        private ServiceUpnp iService;
        private Action iAction;

        private XmlDocument iWriteDocument;
        private XmlElement iActionNode;

        private XmlDocument iReadDocument;
        private XmlNodeList iResponse;
        private int iIndex;
    }

    class HeaderUpnpServer : IHeader
    {
        public HeaderUpnpServer()
        {
        }

	    public void Reset()
        {
            iReceived = false;
        }

	    public bool Recognise(byte[] aHeader)
        {
            return (String.Compare(ASCIIEncoding.UTF8.GetString(aHeader, 0, aHeader.Length), Upnp.kUpnpHeaderServer, StringComparison.OrdinalIgnoreCase) == 0);
        }

	    public void Process(byte[] aValue)
        {
            iReceived = true;
        }

        public bool Received
        {
            get
            {
                return (iReceived);
            }
        }

        private bool iReceived;
    }

    class HeaderUpnpSid : IHeader
    {
        public HeaderUpnpSid()
        {
        }

	    public void Reset()
        {
            iSid = null;
            iReceived = false;
        }

	    public bool Recognise(byte[] aHeader)
        {
            return (String.Compare(ASCIIEncoding.UTF8.GetString(aHeader, 0, aHeader.Length), Upnp.kUpnpHeaderSid, StringComparison.OrdinalIgnoreCase) == 0);
        }

	    public void Process(byte[] aValue)
        {
            string value = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (value.StartsWith(Upnp.kUpnpUuid))
            {
                iSid = value.Substring(Upnp.kUpnpUuid.Length);

                if (iSid.Length > 0)
                {
                    iReceived = true;
                }
            }
        }

        public bool Received
        {
            get
            {
                return (iReceived);
            }
        }

        public string Sid
        {
            get
            {
                return (iSid);
            }
        }

        private bool iReceived;
        private string iSid;
    }

    class HeaderUpnpTimeout : IHeader
    {
        public HeaderUpnpTimeout()
        {
        }

	    public void Reset()
        {
            iTimeout = 0;
            iReceived = false;
        }

	    public bool Recognise(byte[] aHeader)
        {
            return (String.Compare(ASCIIEncoding.UTF8.GetString(aHeader, 0, aHeader.Length), Upnp.kUpnpHeaderTimeout, StringComparison.OrdinalIgnoreCase) == 0);
        }

	    public void Process(byte[] aValue)
        {
            string value = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (value.StartsWith(Upnp.kUpnpSecond))
            {
                try
                {
                    iTimeout = Convert.ToUInt32(value.Substring(Upnp.kUpnpSecond.Length));
                    iReceived = true;
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }
        }

        public bool Received
        {
            get
            {
                return (iReceived);
            }
        }

        public uint Timeout
        {
            get
            {
                return (iTimeout);
            }
        }

        private bool iReceived;
        private uint iTimeout;
    }

    public class Upnp
    {
        public const string kUpnpMethodSubscribe = "SUBSCRIBE";
        public const string kUpnpMethodUnsubscribe = "UNSUBSCRIBE";
        public const string kUpnpMethodNotify = "NOTIFY";

        public const string kUpnpHeaderHost = "HOST";
        public const string kUpnpHeaderCallback = "CALLBACK";
        public const string kUpnpHeaderNt = "NT";
        public const string kUpnpHeaderTimeout = "TIMEOUT";
        public const string kUpnpHeaderServer = "SERVER";
        public const string kUpnpHeaderSid = "SID";
        public const string kUpnpHeaderContentType = "CONTENT-TYPE";
        public const string kUpnpHeaderContentLength = "CONTENT-LENGTH";
        public const string kUpnpHeaderNts = "NTS";
        public const string kUpnpHeaderSeq = "SEQ";
        public const string kUpnpHeaderTransferEncoding = "TRANSFER-ENCODING";

        public const string kUpnpEvent = "upnp:event";
        public const string kUpnpSecond = "Second-";
        public const string kUpnpUuid = "uuid:";
        public const string kUpnpChunked = "chunked";

        public const byte kUpnpTimeoutSeparator = Ascii.Ascii.kAsciiHyphen;
        public const byte kUpnpCallbackUriBegin = Ascii.Ascii.kAsciiAngleOpen;
        public const byte kUpnpCallbackUriEnd = Ascii.Ascii.kAsciiAngleClose;

        private const string kUpnpOrg = "upnp.org";
        private const string kSchemasUpnpOrg = "schemas.upnp.org";

        public static void WriteMethodSubscribe(IWriterMethod aWriter, string aUri)
        {
            aWriter.WriteMethod(ASCIIEncoding.UTF8.GetBytes(kUpnpMethodSubscribe), ASCIIEncoding.UTF8.GetBytes(aUri), EVersion.eHttp11);
        }

        public static void WriteMethodUnsubscribe(IWriterMethod aWriter, string aUri)
        {
            aWriter.WriteMethod(ASCIIEncoding.UTF8.GetBytes(kUpnpMethodUnsubscribe), ASCIIEncoding.UTF8.GetBytes(aUri), EVersion.eHttp11);
        }

        public static void WriteHost(IWriterHeaderExtended aWriter, string aEndpoint)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kUpnpHeaderHost), ASCIIEncoding.UTF8.GetBytes(aEndpoint));
        }

        public static void WriteNotificationType(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kUpnpHeaderNt), ASCIIEncoding.UTF8.GetBytes(kUpnpEvent));
        }

        public static void WriteCallback(IWriterHeaderExtended aWriter, string aUri)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kUpnpHeaderCallback));
            stream.Write(kUpnpCallbackUriBegin);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(aUri));
            stream.Write(kUpnpCallbackUriEnd);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteTimeout(IWriterHeaderExtended aWriter, uint aTimeout)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kUpnpHeaderTimeout));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kUpnpSecond));
            stream.WriteUint(aTimeout);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteSid(IWriterHeaderExtended aWriter, string aSubscriptionId)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kUpnpHeaderSid));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kUpnpUuid));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(aSubscriptionId));
            aWriter.WriteHeaderTerminator();
        }

        public static string ServiceTypeToString(ServiceType aType)
        {
            string domain = aType.Domain;

            if (domain == kUpnpOrg)
            {
                domain = kSchemasUpnpOrg;
            }

            domain = domain.Replace('.', '-');

            // eg urn:linn-co-uk:service:Volkano:2

            return ("urn:" + domain + ":service:" + aType.Type + ":" + aType.Version.ToString());
        }

        public static ServiceType StringToServiceType(string aValue)
        {
            string[] split = aValue.Split(new char[] { ':' });

            string domain = split[1].Replace('-', '.');

            if (domain == kSchemasUpnpOrg)
            {
                domain = kUpnpOrg;
            }

            string type = split[3];

            uint version = 0;

            try
            {
                version = Convert.ToUInt32(split[4]);
            }
            catch (FormatException)
            {
            }

            return (new ServiceType(domain, type, version));
        }

        public static XmlNode DeviceXmlRoot(string aDeviceXml)
        {
            StringReader reader = new StringReader(aDeviceXml);
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(reader);
            }
            catch (XmlException)
            {
                return (null);
            }

            XmlNamespaceManager nsmanager = new XmlNamespaceManager(document.NameTable);
            nsmanager.AddNamespace("u", "urn:schemas-upnp-org:device-1-0");

            return (document.SelectSingleNode("/u:root/u:device", nsmanager));
        }

        public static XmlNode DeviceXmlExplicit(string aDeviceXml, string aUdn)
        {
            StringReader reader = new StringReader(aDeviceXml);
            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(reader);
            }
            catch (XmlException)
            {
                return (null);
            }

            XmlNamespaceManager nsmanager = new XmlNamespaceManager(document.NameTable);
            nsmanager.AddNamespace("u", "urn:schemas-upnp-org:device-1-0");

            // are we the root device?

            XmlNode udn = document.SelectSingleNode("/u:root/u:device/u:UDN", nsmanager);

            if (udn != null && udn.FirstChild != null)
            {
                if (udn.FirstChild.Value.Substring(5) == aUdn)
                {
                    return (document.SelectSingleNode("/u:root/u:device", nsmanager));
                }
            }

            // no, so hunt down the embedded devices

            foreach (XmlNode e in document.SelectNodes("/u:root/u:device/u:deviceList/u:device", nsmanager))
            {
                udn = e.SelectSingleNode("u:UDN", nsmanager);

                if (udn != null && udn.FirstChild != null)
                {
                    if (udn.FirstChild.Value.Substring(5) == aUdn)
                    {
                        return (e);
                    }
                }
            }

            return (null);
        }

        public static uint HasService(XmlNode aDevice, ServiceType aType)
        {
            if (aDevice != null)
            {
                XmlNamespaceManager nsmanager = new XmlNamespaceManager(aDevice.OwnerDocument.NameTable);
                nsmanager.AddNamespace("u", "urn:schemas-upnp-org:device-1-0");

                foreach (XmlNode s in aDevice.SelectNodes("u:serviceList/u:service/u:serviceType", nsmanager))
                {
                    ServiceType type = Upnp.StringToServiceType(s.FirstChild.Value);

                    if (aType.IsSupportedBy(type))
                    {
                        return (type.Version);
                    }
                }
            }

            return (0);
        }

        public static string PresentationUri(XmlNode aDevice)
        {
            return (Child(aDevice, "presentationURL"));
        }

        public static string FriendlyName(XmlNode aDevice)
        {
            return (Child(aDevice, "friendlyName"));
        }

        public static string Udn(XmlNode aDevice)
        {
            return (Child(aDevice, "UDN").Substring(5)); // remove uuid:
        }

        public static string ModelName(XmlNode aDevice)
        {
            return (Child(aDevice, "modelName"));
        }

        public static string ModelNumber(XmlNode aDevice)
        {
            return (Child(aDevice, "modelNumber"));
        }

        private static string Child(XmlNode aDevice, string aPath)
        {
            if (aDevice != null)
            {
                XmlNamespaceManager nsmanager = new XmlNamespaceManager(aDevice.OwnerDocument.NameTable);
                nsmanager.AddNamespace("u", "urn:schemas-upnp-org:device-1-0");

                XmlNode node = aDevice.SelectSingleNode("u:" + aPath, nsmanager);

                if (node != null)
                {
                    if (node.FirstChild != null)
                    {
                        return (node.FirstChild.Value);
                    }
                }
            }

            return (null);
        }
    }

    public abstract class ServiceUpnp : Linn.ControlPoint.Service
    {
        private const uint kSubscriptionTimeout = 1800;

        private const int kMaxWriteBufferBytes = 1000;
        private const int kMaxReadBufferBytes = 1000;

        protected const string kBasePropertyPath = "e:propertyset/e:property/";
        protected const string kNamespaceUpnpService = "urn:schemas-upnp-org:event-1-0";

        public event EventHandler<EventArgs> EventSubscriptionError;

        protected ServiceUpnp(Device aDevice, ServiceType aType, IProtocol aProtocol)
            : this(aDevice, aType, aProtocol, null)
        {
        }

        protected ServiceUpnp(Device aDevice, ServiceType aType, IProtocol aProtocol, IEventUpnpProvider aEventServer)
            : base(aDevice, aType, aProtocol)
        {
            Server = aEventServer;
            iMutex = new Mutex();
            iUnsubscribeCompleted = new ManualResetEvent(false);
            iControlUri = new Uri(Location.Find(ServiceLocationUpnp.kKeyUpnpControlUri));
            iSubscribing = false;
            iUnsubscribing = false;
            iPendingSubscribe = false;
            iPendingUnsubscribe = false;
            iClosing = false;

            if (Server != null)
            {
                iEventUri = new Uri(Location.Find(ServiceLocationUpnp.kKeyUpnpSubscriptionUri));
                IPAddress address; 
                if (IPAddress.TryParse(iEventUri.Host, out address)) 
                { 
                    iEventEndpoint = new IPEndPoint(address, iEventUri.Port); 
                } 
                else 
                { 
                    try 
                    { 
                        IPAddress[] addresses = Dns.GetHostEntry(iEventUri.Host).AddressList; 
                        for (int i = 0; i < addresses.Length && address == null; i++) 
                        { 
                            if (addresses[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) 
                            { 
                                address = addresses[i]; 
                                break; 
                            } 
                        } 
                        if (address != null) 
                        { 
                            iEventEndpoint = new IPEndPoint(address, iEventUri.Port); 
                        } 
                        else 
                        { 
                            UserLog.WriteLine("Endpoint not found: " + iEventUri.Host + ":" + iEventUri.Port); 
                            throw new NetworkError(); 
                        } 
                    } 
                    catch (Exception ex) 
                    { 
                        throw (new ServiceException(903, "Endpoint lookup failure: " + iEventUri.Host + ":" + iEventUri.Port + ", " + ex)); 
                    } 
                }

                iSubscriptionTimer = new System.Threading.Timer(SubscriptionTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);

                iRequest = new TcpClientStream();
                iWriteBuffer = new Swb(kMaxWriteBufferBytes, iRequest);
                iReadBuffer = new Srb(kMaxReadBufferBytes, iRequest);
                iWriter = new WriterRequest(iWriteBuffer);
                iReader = new ReaderResponse2(iReadBuffer);
                iHeaderUpnpSid = new HeaderUpnpSid();
                iHeaderUpnpServer = new HeaderUpnpServer();
                iHeaderUpnpTimeout = new HeaderUpnpTimeout();
                iReader.AddHeader(iHeaderUpnpSid);
                iReader.AddHeader(iHeaderUpnpServer);
                iReader.AddHeader(iHeaderUpnpTimeout);
            }
        }

        // Must close service before discarding it
        // After closing, must not use further

        public override void Close()
        {
            Trace.WriteLine(Trace.kUpnp, "Close            " + this);

            iMutex.WaitOne();

            iClosing = true;

			if (iSubscriptionTimer != null)
			{
				iSubscriptionTimer.Dispose();
			}

            if (iSubscriptionId != null)  // are we subscribed?
            {
                if ((iUnsubscribing || iPendingUnsubscribe) && !iPendingSubscribe)
                {
                    iUnsubscribeCompleted.Reset();

                    iMutex.ReleaseMutex();

                    iUnsubscribeCompleted.WaitOne();
                }
                else
                {
                    iMutex.ReleaseMutex();

                    Assert.Check(false); // Must have issued unsubscribe before closing
                }
            }
            else
            {
                iMutex.ReleaseMutex();
            }
        }

        public override void Kill()
        {
            Trace.WriteLine(Trace.kUpnp, "Kill             " + this);

			if (iSubscriptionTimer != null)
			{
				iSubscriptionTimer.Dispose();
			}

            if (Server != null)
            {

                iMutex.WaitOne();

                iClosing = true;

                if (iSubscriptionId != null)
                {
                    Server.RemoveSession(iSubscriptionId);

                    iSubscriptionId = null;

                    iUnsubscribeCompleted.Set();
                }

                iMutex.ReleaseMutex();
            }
        }

        private void SubscriptionTimerElapsed(Object state)
        {
            Renew();
        }

        protected void Subscribe()
        {
            if (Server == null)
            {
                throw (new ServiceException(302, "Subscribe without Upnp Event Server disallowed"));
            }

            Trace.WriteLine(Trace.kUpnp, "Subscribe        " + this);

            iMutex.WaitOne();

            if (iClosing)
            {
                iMutex.ReleaseMutex();
                Assert.Check(false);
            }

            if (iSubscribing)
            {
                iPendingUnsubscribe = false;

                iMutex.ReleaseMutex();

                Trace.WriteLine(Trace.kUpnp, "Subscribe        Already Subscribing");

                return;
            }

            if (iUnsubscribing)
            {
                iPendingSubscribe = true;

                iMutex.ReleaseMutex();

                Trace.WriteLine(Trace.kUpnp, "Subscribe        Unsubscribing(Queue Pending Subscribe)");

                return;
            }

            Assert.Check(iSubscriptionId == null);

            iSubscribing = true;

            iMutex.ReleaseMutex();

            PendingSubscribe();
        }

        private void HandleSubscriptionException(string aMethodName, Exception aException)
        {
            string message;
            try
            {
                message = String.Format("{0}: ServiceUpnp.HandleSubscriptionException - Device:{1}[{2}({3})], SubscriptionId:{4}, ServiceType:{5}, Method:{6}, Exception:{7}", DateTime.Now, Device.Name, Device.Udn, Device.IpAddress, iSubscriptionId, Type, aMethodName, aException);
            }
            catch (Exception)
            {
                message = String.Format("{0}: ServiceUpnp.HandleSubscriptionException - Device Unknown, SubscriptionId:{1}, ServiceType:{2}, Method:{3}, Exception:{4}", DateTime.Now, iSubscriptionId, Type, aMethodName, aException);
            }
            UserLog.WriteLine(message);
            Trace.WriteLine(Trace.kUpnp, message);
            iRequest.Close();
            iSubscribing = false;
            iPendingSubscribe = false;
            iUnsubscribing = false;
            iPendingUnsubscribe = false;
            iSubscriptionId = null;
        }

        private void PendingSubscribe()
        {
            Trace.WriteLine(Trace.kUpnp, "Subscribe        Begin Connect");

            try
            {
                iRequest.BeginConnect(iEventEndpoint, SubscribeConnect);
            }
            catch (Exception ex) 
            {
                HandleSubscriptionException("PendingSubscribe", ex);
            }
        }

        private void SubscribeConnect(IAsyncResult aAsync)
        {
            Trace.WriteLine(Trace.kUpnp, "Subscribe        End Connect");

            try
            {
                iRequest.EndConnect(aAsync);

                // The event URI might also be required to include the URI fragment
                Upnp.WriteMethodSubscribe(iWriter, iEventUri.PathAndQuery);
                Upnp.WriteHost(iWriter, iEventUri.Host + ":" + iEventUri.Port.ToString());
                Upnp.WriteCallback(iWriter, Server.Uri);
                Upnp.WriteNotificationType(iWriter);
                Upnp.WriteTimeout(iWriter, kSubscriptionTimeout);

                iWriter.WriteFlush();

                Trace.WriteLine(Trace.kUpnp, "Subscribe        Begin Wait For Data");

                iRequest.BeginWaitForData(SubscribeWaitForData);
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("SubscribeConnect", ex);
            }
        }

        private void SubscribeWaitForData(IAsyncResult aAsync)
        {
            Trace.WriteLine(Trace.kUpnp, "Subscribe        End Wait For Data");

            try
            {
                iRequest.EndWaitForData(aAsync);

                iReader.Read();

                iRequest.Close(); // closes the socket

                if (iHeaderUpnpServer.Received && iHeaderUpnpTimeout.Received && iHeaderUpnpSid.Received)
                {
                    iSubscriptionTimer.Change(iHeaderUpnpTimeout.Timeout * 500, Timeout.Infinite);

                    iMutex.WaitOne();

                    iSubscribing = false;

                    iSubscriptionId = iHeaderUpnpSid.Sid;

                    Trace.WriteLine(Trace.kUpnp, "Subscribe        Complete");

                    Server.AddSession(iSubscriptionId, EventServerEvent);

                    // Check for pending unsubscribe

                    if (iPendingUnsubscribe)
                    {
                        iPendingUnsubscribe = false;
                        iUnsubscribing = true;

                        iMutex.ReleaseMutex();

                        Trace.WriteLine(Trace.kUpnp, "Subscribe        Pending Unsubscribe");

                        PendingUnsubscribe();
                    }
                    else
                    {
                        iMutex.ReleaseMutex();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("SubscribeWaitForData", ex);
            }
        }

        protected void Unsubscribe()
        {
            if (Server == null)
            {
                throw (new ServiceException(303, "Unsubscribe without Upnp Event Server disallowed"));
            }

            Trace.WriteLine(Trace.kUpnp, "Unsubscribe      " + this);

            iMutex.WaitOne();

            if (iClosing)
            {
                iMutex.ReleaseMutex();
                Assert.Check(false);
            }

            if (iUnsubscribing)
            {
                iPendingSubscribe = false;

                iMutex.ReleaseMutex();

                Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Already Unsubscribing");

                return;
            }

            if (iSubscribing)
            {
                iPendingUnsubscribe = true;

                iMutex.ReleaseMutex();

                Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Subscribing(Queue Pending Unsubscribe)");

                return;
            }

            if (iSubscriptionId == null)
            {
                iMutex.ReleaseMutex();

                Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Killed");

                return;
            }

            iUnsubscribing = true;

            iMutex.ReleaseMutex();

            PendingUnsubscribe();
        }

        private void PendingUnsubscribe()
        {
            Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Begin Connect");

            Server.RemoveSession(iSubscriptionId);

            iSubscriptionTimer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                iRequest.BeginConnect(iEventEndpoint, UnsubscribeConnect);
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("PendingUnsubscribe", ex);
            }
        }

        private void UnsubscribeConnect(IAsyncResult aAsync)
        {
            Trace.WriteLine(Trace.kUpnp, "Unsubscribe      End Connect");

            try
            {
                iRequest.EndConnect(aAsync);

                iMutex.WaitOne();

                string sid = iSubscriptionId;

                iMutex.ReleaseMutex();

                if (sid != null)
                {
                    Upnp.WriteMethodUnsubscribe(iWriter, iEventUri.AbsolutePath);
                    Upnp.WriteHost(iWriter, iEventUri.Host + ":" + iEventUri.Port.ToString());
                    Upnp.WriteSid(iWriter, sid);
                    iWriter.WriteFlush();

                    Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Begin Wait For Data");

                    iRequest.BeginWaitForData(UnsubscribeWaitForData);
                }
                else
                {
                    Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Killed");

                    iRequest.Close();
                }
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("UnsubscribeConnect", ex);
            }
        }

        private void UnsubscribeWaitForData(IAsyncResult aAsync)
        {
            Trace.WriteLine(Trace.kUpnp, "Unsubscribe      End Wait For Data");

            try
            {
                iRequest.EndWaitForData(aAsync);

                iReader.Read();

                iRequest.Close(); // closes the socket

                Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Complete");

                iMutex.WaitOne();

                iUnsubscribing = false;

                iSubscriptionId = null;

                iUnsubscribeCompleted.Set();

                // Check for pending subscribe

                if (iPendingSubscribe)
                {
                    iPendingSubscribe = false;
                    iSubscribing = true;

                    iMutex.ReleaseMutex();

                    Trace.WriteLine(Trace.kUpnp, "Unsubscribe      Pending Subscribe");

                    PendingSubscribe();
                }
                else
                {
                    iMutex.ReleaseMutex();
                }
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("UnsubscribeWaitForData", ex);
            }
        }

        private void Renew()
        {
            Trace.WriteLine(Trace.kUpnp, "Renew            " + this);

            iMutex.WaitOne();

            if (iUnsubscribing)
            {
                iMutex.ReleaseMutex();
                return;
            }

            iSubscribing = true;

            iMutex.ReleaseMutex();

            try
            {
                iRequest.BeginConnect(iEventEndpoint, RenewConnect);
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("Renew", ex);
            }
        }

        private void RenewConnect(IAsyncResult aAsync)
        {
            try
            {
                iRequest.EndConnect(aAsync);

                iMutex.WaitOne();

                string sid = iSubscriptionId;

                iMutex.ReleaseMutex();

                if (sid != null)
                {

                    Upnp.WriteMethodSubscribe(iWriter, iEventUri.AbsolutePath);
                    Upnp.WriteHost(iWriter, iEventUri.Host + ":" + iEventUri.Port.ToString());
                    Upnp.WriteSid(iWriter, iSubscriptionId);
                    Upnp.WriteTimeout(iWriter, kSubscriptionTimeout);

                    iWriter.WriteFlush();

                    iRequest.BeginWaitForData(RenewWaitForData);
                }
                else
                {
                    iRequest.Close();
                    iSubscribing = false;
                    iPendingSubscribe = false;
                    iUnsubscribing = false;
                    iPendingUnsubscribe = false;
                    iSubscriptionId = null;
                }
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("RenewConnect", ex);
            }
        }

        private void RenewWaitForData(IAsyncResult aAsync)
        {
            try
            {
                iRequest.EndWaitForData(aAsync);

                iReader.Read();

                iRequest.Close(); // closes the socket

                if (iHeaderUpnpServer.Received && iHeaderUpnpTimeout.Received && iHeaderUpnpSid.Received)
                {
                    iSubscriptionTimer.Change(iHeaderUpnpTimeout.Timeout * 500, Timeout.Infinite);

                    iMutex.WaitOne();

                    iSubscribing = false;

                    // Check for pending unsubscribe

                    if (iPendingUnsubscribe)
                    {
                        iPendingUnsubscribe = false;

                        iMutex.ReleaseMutex();

                        Unsubscribe();
                    }
                    else
                    {
                        iMutex.ReleaseMutex();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleSubscriptionException("RenewWaitForData", ex);
            }
        }

        private static void ProcessSoapException(ref SoapException aException, out int aCode, out string aDescription)
        {
            if (aException.Message == "UPnPError")
            {
                if (!aException.Code.IsEmpty && (aException.Code.Name == "Client" || aException.Code.Name.EndsWith(":Client")))
                {
                    try
                    {
                        XmlNode node = aException.Detail;
                        XmlNamespaceManager nsmanager = new XmlNamespaceManager(node.OwnerDocument.NameTable);
                        nsmanager.AddNamespace("u", "urn:schemas-upnp-org:control-1-0");

                        XmlNode code = node.SelectSingleNode("u:UPnPError/u:errorCode", nsmanager);
                        XmlNode description = node.SelectSingleNode("u:UPnPError/u:errorDescription", nsmanager);

                        if (code != null && description != null)
                        {
                            aCode = int.Parse(code.FirstChild.Value, System.Globalization.CultureInfo.InvariantCulture);
                            aDescription = description.FirstChild.Value;
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            aCode = kCommsError;
            aDescription = aException.Message;
        }

        protected ServiceException CreateServiceException(ref SoapException aException)
        {
            int code;
            string description;
            ProcessSoapException(ref aException, out code, out description);
            return (new ServiceException(code, description));
        }

        protected ServiceException CreateServiceException(ref Exception aException)
        {
            return (new ServiceException(900, aException.Message + " " + Device));
        }

        protected EventArgsError CreateEventArgsError(ref SoapException aException)
        {
            int code;
            string description;
            ProcessSoapException(ref aException, out code, out description);
            return (new EventArgsError(code, description));
        }

        protected EventArgsError CreateEventArgsError(ref WebException aException)
        {
            return (new EventArgsError(900, aException.Message + " " + Device.Name));
        }

        public Uri ControlUri
        {
            get
            {
                return (iControlUri);
            }
        }

        public string SubscriptionId
        {
            get
            {
                return (iSubscriptionId);
            }
        }

        public WebProxy WebProxy
        {
            get
            {
                return (iWebProxy);
            }
        }

        protected abstract void EventServerEvent(EventServerUpnp obj, EventArgsEvent e);

        internal IEventUpnpProvider Server;

        private string iSubscriptionId;
        private bool iSubscribing;
        private bool iUnsubscribing;
        private bool iPendingSubscribe;
        private bool iPendingUnsubscribe;
        private bool iClosing;

        private Uri iControlUri;
        private Uri iEventUri;
        private EndPoint iEventEndpoint;
        private System.Threading.Timer iSubscriptionTimer;
        private Mutex iMutex;
        private ManualResetEvent iUnsubscribeCompleted;

        private TcpClientStream iRequest;
        private Swb iWriteBuffer;
        private Srb iReadBuffer;
        private WriterRequest iWriter;
        private ReaderResponse2 iReader;
        private HeaderUpnpSid iHeaderUpnpSid;
        private HeaderUpnpServer iHeaderUpnpServer;
        private HeaderUpnpTimeout iHeaderUpnpTimeout;
        private static WebProxy iWebProxy = new WebProxy();
    }

    public class ServiceLocationUpnp : ServiceLocation
    {
        public const string kKeyUpnpControlUri = "UpnpControlUri";
        public const string kKeyUpnpSubscriptionUri = "UpnpSubscriptionUri";

        public ServiceLocationUpnp(Uri aControlUri, Uri aSubscriptionUri)
        {
            iDictionary.Add(kKeyUpnpControlUri, aControlUri.ToString());
            if (aSubscriptionUri != null)
            {
                iDictionary.Add(kKeyUpnpSubscriptionUri, aSubscriptionUri.ToString());
            }
        }
    }
}
