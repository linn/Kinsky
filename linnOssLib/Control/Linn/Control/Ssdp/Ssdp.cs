using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

using Linn;
using Linn.Ascii;
using Linn.Network;
using Linn.Control.Http;

namespace Linn.Control.Ssdp
{

    // IWriterSsdp - base interface shared by IWriterSsdpNotify and IWriterSsdpMsearchResponse

    public interface IWriterSsdp : IWriter
    {
        void WriteMaxAge(uint aMaxAge);
        void WriteLocation(byte[] aAddress, uint aPort, byte[] aUrl);
        void WriteServer(byte[] aValue);
        void WriteTypeRoot();
        void WriteTypeUuid(byte[] aUuid);
        void WriteTypeDeviceType(byte[] aDomain, byte[] aType, uint aVersion);
        void WriteTypeServiceType(byte[] aDomain, byte[] aType, uint aVersion);
        void WriteUsnRoot(byte[] aUuid);
        void WriteUsnUuid(byte[] aUuid);
        void WriteUsnDeviceType(byte[] aDomain, byte[] aType, uint aVersion, byte[] aUuid);
        void WriteUsnServiceType(byte[] aDomain, byte[] aType, uint aVersion, byte[] aUuid);
    }

    // IWriterSsdpNotify - Ssdp Http header writer interface for sending alive/byebye messages

    public interface IWriterSsdpNotify : IWriterSsdp
    {
        void WriteMethod();
        void WriteHost();
        void WriteSubTypeAlive();
        void WriteSubTypeByeBye();
    };

    // IWriterSsdpMsearchResponse - Ssdp Http header writer interface for sending msearch responses

    public interface IWriterSsdpMsearchResponse : IWriterSsdp
    {
        void WriteStatus();
        void WriteHost(EndPoint aEndpoint);
        void WriteExt();
    };

    // ISwriterSsdpMsearchRequest - Ssdp Http header writer interface for sending msearch requests
    // and streaming the responses

    public interface IWriterSsdpMsearchRequest : IWriter, IReaderSource
    {
        void WriteMethod();
        void WriteHost();
        void WriteMan();
        void WriteMx(uint aSeconds);
        void WriteTypeRoot();
        void WriteTypeUuid(byte[] aUuid);
        void WriteTypeDeviceType(byte[] aDomain, byte[] aType, uint aVersion);
        void WriteTypeServiceType(byte[] aDomain, byte[] aType, uint aVersion);
        void WriteTypeAll();
    };

    public class Ssdp
    {
        public static readonly string kSsdpMethodMsearch = "M-SEARCH";
        public static readonly string kSsdpMethodNotify = "NOTIFY";
        public static readonly string kSsdpMethodUri = "*";

        public static readonly string kSsdpStatusOk = "OK";

        public static readonly string kSsdpHeaderHost = "HOST";
        public static readonly string kSsdpHeaderServer = "SERVER";
        public static readonly string kSsdpHeaderCacheControl = "CACHE-CONTROL";
        public static readonly string kSsdpHeaderLocation = "LOCATION";
        public static readonly string kSsdpHeaderNt = "NT";
        public static readonly string kSsdpHeaderSt = "ST";
        public static readonly string kSsdpHeaderUsn = "USN";
        public static readonly string kSsdpHeaderNts = "NTS";
        public static readonly string kSsdpHeaderExt = "EXT";
        public static readonly string kSsdpHeaderMan = "MAN";
        public static readonly string kSsdpHeaderMx = "MX";

        public static readonly string kSsdpAlive = "ssdp:alive";
        public static readonly string kSsdpByeBye = "ssdp:byebye";

        public static readonly uint kSsdpMinMaxAge = 1800;
        public static readonly byte kSsdpMaxAgeSeparator = Ascii.Ascii.kAsciiEquals;
        public static readonly string kSsdpMaxAge = "max-age";

        public static readonly byte kSsdpUuidSeparator = Ascii.Ascii.kAsciiColon;
        public static readonly string kSsdpUuid = "uuid";

        public static readonly string kSsdpUpnpRoot = "upnp:rootdevice";
        public static readonly string kSsdpUrn = "urn:";
        public static readonly string kSsdpUsnSeparator = "::";
        public static readonly byte kSsdpVersionSeparator = Ascii.Ascii.kAsciiColon;
        public static readonly string kSsdpDeviceSeparator = ":device:";
        public static readonly string kSsdpServiceSeparator = ":service:";
        public static readonly string kSsdpPortSeparator = ":";
        public static readonly string kSsdpUrlSeparator = "/";
        public static readonly string kSsdpSchemas = "schemas-";
        public static readonly string kSsdpUpnpOrg = "upnp-org";
        public static readonly string kSsdpHttp = "http://";

        public static readonly string kSsdpMsearchDiscover = "\"ssdp:discover\"";
        public static readonly string kSsdpMsearchAll = "ssdp:all";
        public static readonly string kSsdpMsearchUpnpRoot = "upnp:rootdevice";
        public static readonly string kSsdpMsearchUuid = "uuid";
        public static readonly string kSsdpMsearchUrn = "urn";
        public static readonly string kSsdpMsearchDevice = "device";
        public static readonly string kSsdpMsearchService = "service";

        public static readonly string kSsdpMulticastAddress = "239.255.255.250";
        public static readonly string kSsdpMulticastAddressAndPort = "239.255.255.250:1900";

        public static readonly byte[] kSsdpMulticastIpAddress = { 239, 255, 255, 250 };
        public static readonly int kSsdpMulticastIpPort = 1900;

        public static bool BeginsWith(byte[] aValue1, string aValue2)
        {
            if (aValue1.Length >= aValue2.Length)
            {
                byte[] prefix = new byte[aValue2.Length];
                Array.Copy(aValue1, 0, prefix, 0, aValue2.Length);
                string sprefix = ASCIIEncoding.UTF8.GetString(prefix, 0, prefix.Length);
                if (sprefix == aValue2) {
                    return (true);
                }
            }
            return (false);
        }

        // For implementing IWriterSsdp

        public static void WriteMaxAge(IWriterHeaderExtended aWriter, uint aMaxAge)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderCacheControl));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpMaxAge));
            stream.WriteSpace();
            stream.Write(kSsdpMaxAgeSeparator);
            stream.WriteSpace();
            stream.WriteUint(aMaxAge);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteLocation(IWriterHeaderExtended aWriter, byte[] aAddress, uint aPort, byte[] aUrl)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderLocation));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpHttp));
            stream.Write(aAddress);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpPortSeparator));
            stream.WriteUint(aPort);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUrlSeparator));
            stream.Write(aUrl);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUrlSeparator));
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteServer(IWriterHeaderExtended aWriter, byte[] aValue)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderServer), aValue);
        }

        private static void WriteTypeRoot(IWriterAscii aWriter)
        {
            aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUpnpRoot));
        }

        private static void WriteTypeUuid(IWriterAscii aWriter, byte[] aUuid)
        {
            aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUuid));
            aWriter.Write(kSsdpUuidSeparator);
            aWriter.Write(aUuid);
        }

        private static void WriteTypeDeviceType(IWriterAscii aWriter, byte[] aDomain, byte[] aType, uint aVersion)
        {
            aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUrn));
            if (aDomain == ASCIIEncoding.UTF8.GetBytes(kSsdpUpnpOrg))
            {
                aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpSchemas));
            }
            aWriter.Write(aDomain);
            aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpDeviceSeparator));
            aWriter.Write(aType);
            aWriter.Write(kSsdpVersionSeparator);
            aWriter.WriteUint(aVersion);
        }

        private static void WriteTypeServiceType(IWriterAscii aWriter, byte[] aDomain, byte[] aType, uint aVersion)
        {
            aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUrn));
            if (aDomain == ASCIIEncoding.UTF8.GetBytes(kSsdpUpnpOrg))
            {
                aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpSchemas));
            }
            aWriter.Write(aDomain);
            aWriter.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpServiceSeparator));
            aWriter.Write(aType);
            aWriter.Write(kSsdpVersionSeparator);
            aWriter.WriteUint(aVersion);
        }

        public static void WriteNotificationTypeRoot(IWriterHeaderExtended aWriter)
        {
            WriteTypeRoot(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderNt)));
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteNotificationTypeUuid(IWriterHeaderExtended aWriter, byte[] aUuid)
        {
            WriteTypeUuid(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderNt)), aUuid);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteNotificationTypeDeviceType(IWriterHeaderExtended aWriter, byte[] aDomain, byte[] aType, uint aVersion)
        {
            WriteTypeDeviceType(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderNt)), aDomain, aType, aVersion);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteNotificationTypeServiceType(IWriterHeaderExtended aWriter, byte[] aDomain, byte[] aType, uint aVersion)
        {
            WriteTypeServiceType(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderNt)), aDomain, aType, aVersion);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteSearchTypeRoot(IWriterHeaderExtended aWriter)
        {
            WriteTypeRoot(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderSt)));
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteSearchTypeUuid(IWriterHeaderExtended aWriter, byte[] aUuid)
        {
            WriteTypeUuid(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderSt)), aUuid);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteSearchTypeDeviceType(IWriterHeaderExtended aWriter, byte[] aDomain, byte[] aType, uint aVersion)
        {
            WriteTypeDeviceType(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderSt)), aDomain, aType, aVersion);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteSearchTypeServiceType(IWriterHeaderExtended aWriter, byte[] aDomain, byte[] aType, uint aVersion)
        {
            WriteTypeServiceType(aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderSt)), aDomain, aType, aVersion);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteUsnRoot(IWriterHeaderExtended aWriter, byte[] aUuid)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderUsn));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUuid));
            stream.Write(kSsdpUuidSeparator);
            stream.Write(aUuid);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUsnSeparator));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUpnpRoot));
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteUsnUuid(IWriterHeaderExtended aWriter, byte[] aUuid)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderUsn));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUuid));
            stream.Write(kSsdpUuidSeparator);
            stream.Write(aUuid);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteUsnDeviceType(IWriterHeaderExtended aWriter, byte[] aDomain, byte[] aType, uint aVersion, byte[] aUuid)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderUsn));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUuid));
            stream.Write(kSsdpUuidSeparator);
            stream.Write(aUuid);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUsnSeparator));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUrn));
            if (aDomain == ASCIIEncoding.UTF8.GetBytes(kSsdpUpnpOrg))
            {
                stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpSchemas));
            }
            stream.Write(aDomain);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpDeviceSeparator));
            stream.Write(aType);
            stream.Write(kSsdpVersionSeparator);
            stream.WriteUint(aVersion);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteUsnServiceType(IWriterHeaderExtended aWriter, byte[] aDomain, byte[] aType, uint aVersion, byte[] aUuid)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderUsn));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUuid));
            stream.Write(kSsdpUuidSeparator);
            stream.Write(aUuid);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUsnSeparator));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpUrn));
            if (aDomain == ASCIIEncoding.UTF8.GetBytes(kSsdpUpnpOrg))
            {
                stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpSchemas));
            }
            stream.Write(aDomain);
            stream.Write(ASCIIEncoding.UTF8.GetBytes(kSsdpServiceSeparator));
            stream.Write(aType);
            stream.Write(kSsdpVersionSeparator);
            stream.WriteUint(aVersion);
            aWriter.WriteHeaderTerminator();
        }

        // For implementing IWriterSsdpNotify

        public static void WriteMethodNotify(IWriterMethod aWriter)
        {
            aWriter.WriteMethod(ASCIIEncoding.UTF8.GetBytes(kSsdpMethodNotify), ASCIIEncoding.UTF8.GetBytes(kSsdpMethodUri), EVersion.eHttp11);
        }

        public static void WriteSubTypeAlive(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderNts), ASCIIEncoding.UTF8.GetBytes(kSsdpAlive));
        }

        public static void WriteSubTypeByeBye(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderNts), ASCIIEncoding.UTF8.GetBytes(kSsdpByeBye));
        }

        // For implementing IWriterSsdpMsearchRequest

        public static void WriteMethodMsearch(IWriterMethod aWriter)
        {
            aWriter.WriteMethod(ASCIIEncoding.UTF8.GetBytes(kSsdpMethodMsearch), ASCIIEncoding.UTF8.GetBytes(kSsdpMethodUri), EVersion.eHttp11);
        }

        public static void WriteMan(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderMan), ASCIIEncoding.UTF8.GetBytes(kSsdpMsearchDiscover));
        }

        public static void WriteMx(IWriterHeaderExtended aWriter, uint aSeconds)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderMx));
            stream.WriteUint(aSeconds);
            aWriter.WriteHeaderTerminator();
        }

        public static void WriteSearchTypeAll(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderSt), ASCIIEncoding.UTF8.GetBytes(kSsdpMsearchAll));
        }

        // For implementing IWriterSsdpNotify & IWriterSsdpMsearchRequest

        public static void WriteHost(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeader(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderHost), ASCIIEncoding.UTF8.GetBytes(kSsdpMulticastAddressAndPort));
        }

        // For implementing IWriterSsdpMsearchResponse
        public static void WriteStatus(IWriterStatus aWriter)
        {
            aWriter.WriteStatus(Http.Http.kOk, EVersion.eHttp11);
        }

        public static void WriteHost(IWriterHeaderExtended aWriter, EndPoint aEndpoint)
        {
            IWriterAscii stream = aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderHost));
            stream.Write(ASCIIEncoding.UTF8.GetBytes(aEndpoint.ToString()));
            aWriter.WriteHeaderTerminator();

        }

        public static void WriteExt(IWriterHeaderExtended aWriter)
        {
            aWriter.WriteHeaderField(ASCIIEncoding.UTF8.GetBytes(kSsdpHeaderExt));
            aWriter.WriteHeaderTerminator();
        }
    }

    // WriterSsdpMsearchRequest - concrete writer of msearch request Http headers to the multicast udp endpoint

    class WriterSsdpMsearchRequest : IWriterSsdpMsearchRequest
    {
        private static readonly int kMaxBufferBytes = 1024;
        private static readonly int kTimeToLive = 4;

        public WriterSsdpMsearchRequest(IPAddress aInterface)
        {
            IPAddress multicast = new IPAddress(Ssdp.kSsdpMulticastIpAddress);
            iSocket = new UdpMulticastStream(aInterface, multicast, Ssdp.kSsdpMulticastIpPort, kTimeToLive);
            iBuffer = new Swb(kMaxBufferBytes, iSocket);
            iWriter = new WriterRequest(iBuffer);
        }

        // IReaderSource

        public int Read(byte[] aBuffer, int aOffset, int aMaxBytes)
        {
            return (iSocket.Read(aBuffer, aOffset, aMaxBytes));
        }

        public void ReadFlush()
        {
            iSocket.ReadFlush();
        }

        /*
        public void ReadShutdown()
        {
            iSocket.ReadShutdown();
        }
        */

        // IWriter

        public void Write(byte aValue)
        {
        }

        public void Write(byte[] aBuffer)
        {
        }

        public void WriteFlush()
        {
            // LOG(kSsdp, iBuffer.Buffer());
            iWriter.WriteFlush();
        }

        // IWriterSsdpMsearchRequest

        public void WriteMethod()
        {
            Ssdp.WriteMethodMsearch(iWriter);
        }

        public void WriteHost()
        {
            Ssdp.WriteHost(iWriter);
        }

        public void WriteMan()
        {
            Ssdp.WriteMan(iWriter);
        }

        public void WriteMx(uint aSeconds)
        {
            Ssdp.WriteMx(iWriter, aSeconds);
        }

        public void WriteTypeRoot()
        {
            Ssdp.WriteSearchTypeRoot(iWriter);
        }

        public void WriteTypeUuid(byte[] aUuid)
        {
            Ssdp.WriteSearchTypeUuid(iWriter, aUuid);
        }

        public void WriteTypeDeviceType(byte[] aDomain, byte[] aType, uint aVersion)
        {
            Ssdp.WriteSearchTypeDeviceType(iWriter, aDomain, aType, aVersion);
        }

        public void WriteTypeServiceType(byte[] aDomain, byte[] aType, uint aVersion)
        {
            Ssdp.WriteSearchTypeServiceType(iWriter, aDomain, aType, aVersion);
        }

        public void WriteTypeAll()
        {
            Ssdp.WriteSearchTypeAll(iWriter);
        }

        public void Shutdown()
        {
            iSocket.Shutdown();
        }

        public void Close()
        {
            iSocket.Close();
        }

        private UdpMulticastStream iSocket;
        private Swb iBuffer;
        private WriterRequest iWriter;
    }

    // ISsdpMsearchHandler - called by SsdpMulticastListener on receiving an m-search request

    public interface ISsdpMsearchHandler
    {
        void SearchAll(EndPoint aEndpoint, uint aMx);
        void SearchRoot(EndPoint aEndpoint, uint aMx);
        void SearchUuid(EndPoint aEndpoint, uint aMx, byte[] aUuid);
        void SearchDeviceType(EndPoint aEndpoint, uint aMx, byte[] aDomain, byte[] aType, uint aVersion);
        void SearchServiceType(EndPoint aEndpoint, uint aMx, byte[] aDomain, byte[] aType, uint aVersion);
    };

    // SsdpMsearchHandler - timer for Msearch responses thawt spreads individual responses over the requested time period
    //                    - adds and removes itself from the provided fifo
    //
    // NULL IMPLEMENTATION FOR CONTROL POINT ONLY

    public class SsdpMsearchHandler : ISsdpMsearchHandler
    {
        public SsdpMsearchHandler()
        {
        }

        // ISsdpMsearchHandler

        public void SearchAll(EndPoint aEndpoint, uint aMx)
        {
        }

        public void SearchRoot(EndPoint aEndpoint, uint aMx)
        {
        }

        public void SearchUuid(EndPoint aEndpoint, uint aMx, byte[] aUuid)
        {
        }

        public void SearchDeviceType(EndPoint aEndpoint, uint aMx, byte[] aDomain, byte[] aType, uint aVersion)
        {
        }

        public void SearchServiceType(EndPoint aEndpoint, uint aMx, byte[] aDomain, byte[] aType, uint aVersion)
        {
        }
    };

    // ISsdpMsearch - generic interface for SsdpMsearcher

    public interface ISsdpMsearch
    {
        void SsdpMsearchRoot(uint aSeconds);
        void SsdpMsearchUuid(byte[] aUuid, uint aSeconds);
        void SsdpMsearchDeviceType(byte[] aDomain, byte[] aType, uint aVersion, uint aSeconds);
        void SsdpMsearchServiceType(byte[] aDomain, byte[] aType, uint aVersion, uint aSeconds);
        void SsdpMsearchAll(uint aSeconds);
    };

    // SsdpMsearcher - concrete class for issuing msearch requests

    public class SsdpMsearcher : ISsdpMsearch, IReaderSource
    {
        private static readonly uint kMinMsearchSeconds = 1;
        private static readonly uint kMaxMsearchSeconds = 120;

        public SsdpMsearcher(IPAddress aInterface)
        {
            iWriter = new WriterSsdpMsearchRequest(aInterface);
        }

        // IReaderSource

        public int Read(byte[] aBuffer, int aOffset, int aMaxBytes)
        {
            return (iWriter.Read(aBuffer, aOffset, aMaxBytes));
        }

        public void ReadFlush()
        {
            iWriter.ReadFlush();
        }

        /*
        public void ReadShutdown()
        {
            iWriter.ReadShutdown();
        }
         */

        // ISsdpMsearch

        public void SsdpMsearchRoot(uint aSeconds)
        {
            SsdpMsearch(aSeconds);
            iWriter.WriteTypeRoot();
            iWriter.WriteFlush();
        }

        public void SsdpMsearchUuid(byte[] aUuid, uint aSeconds)
        {
            SsdpMsearch(aSeconds);
            iWriter.WriteTypeUuid(aUuid);
            iWriter.WriteFlush();
        }

        public void SsdpMsearchDeviceType(byte[] aDomain, byte[] aType, uint aVersion, uint aSeconds)
        {
            SsdpMsearch(aSeconds);
            iWriter.WriteTypeDeviceType(aDomain, aType, aVersion);
            iWriter.WriteFlush();
        }

        public void SsdpMsearchServiceType(byte[] aDomain, byte[] aType, uint aVersion, uint aSeconds)
        {
            SsdpMsearch(aSeconds);
            iWriter.WriteTypeServiceType(aDomain, aType, aVersion);
            iWriter.WriteFlush();
        }

        public void SsdpMsearchAll(uint aSeconds)
        {
            SsdpMsearch(aSeconds);
            iWriter.WriteTypeAll();
            iWriter.WriteFlush();
        }

        private void SsdpMsearch(uint aSeconds)
        {
            Assert.Check(aSeconds >= kMinMsearchSeconds);
            Assert.Check(aSeconds <= kMaxMsearchSeconds);

            iWriter.WriteMethod();
            iWriter.WriteHost();
            iWriter.WriteMan();
            iWriter.WriteMx(aSeconds);
        }

        public void Shutdown()
        {
            iWriter.Shutdown();
        }

        public void Close()
        {
            iWriter.Close();
        }

        private WriterSsdpMsearchRequest iWriter;
    }

    // ISsdpNotifyHandler - called by SsdpMulticastListener on receiving an alive or byebye notification

    public interface ISsdpNotifyHandler
    {
        void NotifyRootAlive(byte[] aUuid, byte[] aLocation, uint aMaxAge);
        void NotifyUuidAlive(byte[] aUuid, byte[] aLocation, uint aMaxAge);
        void NotifyDeviceTypeAlive(byte[] aUuid, byte[] aDomain, byte[] aType, uint aVersion, byte[] aLocation, uint aMaxAge);
        void NotifyServiceTypeAlive(byte[] aUuid, byte[] aDomain, byte[] aType, uint aVersion, byte[] aLocation, uint aMaxAge);
        void NotifyRootByeBye(byte[] aUuid);
        void NotifyUuidByeBye(byte[] aUuid);
        void NotifyDeviceTypeByeBye(byte[] aUuid, byte[] aDomain, byte[] aType, uint aVersion);
        void NotifyServiceTypeByeBye(byte[] aUuid, byte[] aDomain, byte[] aType, uint aVersion);
    }

    public enum ESsdpTarget
    {
        eUnknown,
        eRoot,
        eUuid,
        eDeviceType,
        eServiceType,
        eAll
    }

    // SsdpListener - base class for SsdpListenerMulticast and SsdpListenerUnicast

    public abstract class SsdpListener
    {
        private static readonly ThreadPriority kPriority = ThreadPriority.BelowNormal;

        protected SsdpListener()
        {
            iStarted = new ManualResetEvent(false);
        }

        public void Start(IPAddress aInterface)
        {
            Trace.WriteLine(Trace.kUpnp, "SsdpListener.Start() starting...");

            Assert.Check(iThread == null);

            iTerminate = false;
            iInterface = aInterface;

            OnStart();

            iThread = new Thread(new ThreadStart(Run));
            iThread.Priority = kPriority;
            iThread.Name = "Ssdp Listener";
            iThread.Start();

            iStarted.WaitOne();

            Trace.WriteLine(Trace.kUpnp, "SsdpListener.Start() successful");
        }

        public void Stop()
        {
            Trace.WriteLine(Trace.kUpnp, "SsdpListener.Stop() stopping...");

            if (iThread != null)
            {
                OnStop();

                iTerminate = true;

                iThread.Join();

                iStarted.Reset();
                iThread = null;

                Trace.WriteLine(Trace.kUpnp, "SsdpListener.Stop() successful");
            }
            else
            {
                Trace.WriteLine(Trace.kUpnp, "SsdpListener.Stop() already stopped - silently do nothing");
            }
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void Run();

        protected void ReadCacheControl(byte[] aValue)
        {
            Parser parser = new Parser(aValue);

            byte[] temp = Ascii.Ascii.Trim(parser.Next(Ssdp.kSsdpMaxAgeSeparator));
            string maxage = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
            
            if (maxage == Ssdp.kSsdpMaxAge) {
                try {
                    iMaxAge = Ascii.Ascii.Uint(parser.Remaining());
                }
                catch (AsciiError) {
                    throw (new HttpError());
                }

                /*
                Can't really apply this test because there is one important transgressor
                who uses a Max Age of 180 seconds : MediaTomb
                If they fix this problem, then we should reinstate this following check

                if (iMaxAge < Ssdp.kSsdpMinMaxAge) {
                    throw (new HttpError());
                }
                */
            }
        }

        protected void ReadLocation(byte[] aValue)
        {
            iLocation = aValue;
            iLocationReceived = true;
        }

        protected void ReadServer(byte[] aValue)
        {
            iServerReceived = true;
        }

        protected void ReadType(byte[] aValue)
        {
            string value = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (value == Ssdp.kSsdpMsearchAll) {
                iTarget = ESsdpTarget.eAll;
                return;
            }
            
            if (value == Ssdp.kSsdpMsearchUpnpRoot) {
                iTarget = ESsdpTarget.eRoot;
                return;
            }
            
            Parser parser = new Parser(aValue);

            byte[] temp = parser.Next(Ascii.Ascii.kAsciiColon);
            string type = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
            
            if (type == Ssdp.kSsdpMsearchUuid) {
                iUuid = parser.Remaining();
                iTarget = ESsdpTarget.eUuid;
                return;
            }
            
            if (type == Ssdp.kSsdpMsearchUrn) {
                iDomain = parser.Next(Ascii.Ascii.kAsciiColon);
                temp = parser.Next(Ascii.Ascii.kAsciiColon);
                string kind = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
                iType = parser.Next(Ascii.Ascii.kAsciiColon);
                
                try {
                    iVersion = Ascii.Ascii.Uint(parser.Remaining());
                }
                catch (AsciiError) {
                    throw (new HttpError());
                }

                /*
                if (Ssdp.BeginsWith(iDomain, Ssdp.kSsdpSchemas))
                {
                    int prefixbytes = Ssdp.kSsdpSchemas.Length;
                    byte[] domain = new byte[iDomain.Length - prefixbytes];
                    Array.Copy(iDomain, prefixbytes, domain, 0, iDomain.Length - prefixbytes);
                    iDomain = domain;
                }

                for (uint i = 0; i < iDomain.Length; i++)
                {
                    if (iDomain[i] == Ascii.Ascii.kAsciiMinus)
                    {
                        iDomain[i] = Ascii.Ascii.kAsciiDot;
                    }
                }
                */

                if (kind == Ssdp.kSsdpMsearchDevice) {
                    iTarget = ESsdpTarget.eDeviceType;
                    return;
                }
                
                if (kind == Ssdp.kSsdpMsearchService) {
                    iTarget = ESsdpTarget.eServiceType;
                    return;
                }
            }
            throw (new HttpError());
        }

        protected void ReadUsn(byte[] aValue)
        {
            Parser parser = new Parser(aValue);

            byte[] temp = parser.Next(Ascii.Ascii.kAsciiColon);
            string prefix = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
            
            if (prefix != Ssdp.kSsdpUuid) {
                throw (new HttpError());
            }
            
            iUsnUuid = parser.Next(Ascii.Ascii.kAsciiColon);
            
            parser.Next(Ascii.Ascii.kAsciiColon); // double colon separator

            temp = parser.Remaining();
            string value = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);

            if (value.Length == 0) {
                iUsnTarget = ESsdpTarget.eUuid;
                return;
            }
                
            if (value == Ssdp.kSsdpUpnpRoot) {
                iUsnTarget = ESsdpTarget.eRoot;
                return;
            }

            temp = parser.Next(Ascii.Ascii.kAsciiColon);
            string type = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
            
            if (type == Ssdp.kSsdpMsearchUrn) {
                iUsnDomain = parser.Next(Ascii.Ascii.kAsciiColon);
                temp = parser.Next(Ascii.Ascii.kAsciiColon);
                string kind = ASCIIEncoding.UTF8.GetString(temp, 0, temp.Length);
                iUsnType = parser.Next(Ascii.Ascii.kAsciiColon);
                
                try {
                    iUsnVersion = Ascii.Ascii.Uint(parser.Remaining());
                }
                catch (AsciiError) {
                    throw (new HttpError());
                }
                
                /*
                if (Ssdp.BeginsWith(iUsnDomain, Ssdp.kSsdpSchemas))
                {
                    int prefixbytes = Ssdp.kSsdpSchemas.Length;
                    byte[] domain = new byte[iUsnDomain.Length - prefixbytes];
                    Array.Copy(iUsnDomain, prefixbytes, domain, 0, iUsnDomain.Length - prefixbytes);
                    iUsnDomain = domain;
                }

                for (uint i = 0; i < iUsnDomain.Length; i++)
                {
                    if (iUsnDomain[i] == Ascii.Ascii.kAsciiMinus)
                    {
                        iUsnDomain[i] = Ascii.Ascii.kAsciiDot;
                    }
                }
                */

                if (kind == Ssdp.kSsdpMsearchDevice)
                {
                    iUsnTarget = ESsdpTarget.eDeviceType;
                    return;
                }
                
                if (kind == Ssdp.kSsdpMsearchService) {
                    iUsnTarget = ESsdpTarget.eServiceType;
                    return;
                }

            }
            throw (new HttpError());
        }

        protected ManualResetEvent iStarted;
        protected IPAddress iInterface;
        protected Thread iThread;
        protected uint iMaxAge;
        protected bool iLocationReceived;
        protected byte[] iLocation;
        protected bool iServerReceived;
        protected ESsdpTarget iTarget;
        protected byte[] iUuid;
        protected byte[] iDomain;
        protected byte[] iType;
        protected uint iVersion;
        protected ESsdpTarget iUsnTarget;
        protected byte[] iUsnUuid;
        protected byte[] iUsnDomain;
        protected byte[] iUsnType;
        protected uint iUsnVersion;
        protected volatile bool iTerminate;
    }

    public interface ISsdpNotifyProvider
    {
        void Add(ISsdpNotifyHandler aHandler);
    }

    // SsdpListenerMulticast - listens to the multicast udp endpoint
    //                       - processes received messages and passes them on to either an ISsdpMsearchHandler or an ISsdpNotifyHandler

    public class SsdpListenerMulticast : SsdpListener, IWriterRequest, ISsdpNotifyProvider
    {
        private static readonly int kMaxBufferBytes = 1024;

        public SsdpListenerMulticast()
        {
            iLockObject = new object();

            iMsearchList = new List<ISsdpMsearchHandler>();
            iNotifyList = new List<ISsdpNotifyHandler>();
        }

        public void Add(ISsdpNotifyHandler aHandler)
        {
            lock (iLockObject)
            {
                iNotifyList.Add(aHandler);
            }
        }

        public void Add(ISsdpMsearchHandler aHandler)
        {
            lock (iLockObject)
            {
                iMsearchList.Add(aHandler);
            }
        }

        protected override void OnStart()
        {
            IPAddress multicast = new IPAddress(Ssdp.kSsdpMulticastIpAddress);
            iSocket = new UdpMulticastReader(iInterface, multicast, Ssdp.kSsdpMulticastIpPort);
            iBuffer = new Srb(kMaxBufferBytes, iSocket);
            iReader = new ReaderRequest(iBuffer, this);
        }

        protected override void OnStop()
        {
            if (iSocket != null)
            {
                iSocket.Shutdown();
                iSocket.Close();
            }
        }

        protected override void Run()
        {
            iStarted.Set();

            while (!iTerminate)
            {
                iMethodReceived = false;
                iHostReceived = false;
                iManReceived = false;
                iMx = 0;
                iTarget = ESsdpTarget.eUnknown;
                iUsnTarget = ESsdpTarget.eUnknown;
                iMaxAge = 0;
                iLocationReceived = false;
                iSubTypeReceived = false;
                iServerReceived = false;

                try
                {
                    iReader.Read();
                }
                catch (HttpError)
                {
                    //LOG(kSsdp, "SSDP: HttpError\n");
                }
                catch (ReaderError)
                {
                    //LOG(kSsdp, "SSDP: ReaderError\n");
                }
                catch (ThreadAbortException)
                {
                    if(iSocket != null)
                    {
                        iSocket.Shutdown();
                        iSocket.Close();
                    }

                    iSocket = null;
                    iBuffer = null;
                    iReader = null;
                }
            }

            if(iSocket != null)
            {
                iSocket.Shutdown();
                iSocket.Close();
            }

            iSocket = null;
            iBuffer = null;
            iReader = null;
        }

        // IWriterRequest

        public void Write(byte aValue)
        {
            Assert.Check(false);
        }

        public void Write(byte[] aBuffer)
        {
            Assert.Check(false);
        }

        public void WriteFlush()
        {
            // LOG(kSsdp, "\n");

            if (iMethodReceived)
            {
                if (iMethodMsearch) // M-SEARCH method
                {
                    if (iHostReceived && iManReceived && (iMx > 0))
                    {
                        switch (iTarget)
                        {
                            case ESsdpTarget.eRoot:
                                lock (iLockObject)
                                {
                                    foreach (ISsdpMsearchHandler handler in iMsearchList)
                                    {
                                        handler.SearchRoot(iSocket.Sender(), iMx);
                                    }
                                }
                                return;
                            case ESsdpTarget.eUuid:
                                lock (iLockObject)
                                {
                                    foreach (ISsdpMsearchHandler handler in iMsearchList)
                                    {
                                        handler.SearchUuid(iSocket.Sender(), iMx, iUuid);
                                    }
                                }
                                return;
                            case ESsdpTarget.eDeviceType:
                                lock (iLockObject)
                                {
                                    foreach (ISsdpMsearchHandler handler in iMsearchList)
                                    {
                                        handler.SearchDeviceType(iSocket.Sender(), iMx, iDomain, iType, iVersion);
                                    }
                                }
                                return;
                            case ESsdpTarget.eServiceType:
                                lock (iLockObject)
                                {
                                    foreach (ISsdpMsearchHandler handler in iMsearchList)
                                    {
                                        handler.SearchServiceType(iSocket.Sender(), iMx, iDomain, iType, iVersion);
                                    }
                                }
                                return;
                            case ESsdpTarget.eAll:
                                lock (iLockObject)
                                {
                                    foreach (ISsdpMsearchHandler handler in iMsearchList)
                                    {
                                        handler.SearchAll(iSocket.Sender(), iMx);
                                    }
                                }
                                return;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    if (iSubTypeReceived)
                    {
                        if (iSubTypeAlive)
                        {
                            if (iHostReceived && (iMaxAge > 0) && iLocationReceived && iServerReceived)
                            {
                                switch (iTarget)
                                {
                                    case ESsdpTarget.eRoot:
                                        if (iUsnTarget == ESsdpTarget.eRoot)
                                        {
                                            lock (iLockObject)
                                            {
                                                foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                {
                                                    handler.NotifyRootAlive(iUsnUuid, iLocation, iMaxAge);
                                                }
                                            }
                                            return;
                                        }
                                        break;
                                    case ESsdpTarget.eUuid:
                                        if (iUsnTarget == ESsdpTarget.eUuid)
                                        {
                                            if (ASCIIEncoding.UTF8.GetString(iUuid, 0, iUuid.Length) == ASCIIEncoding.UTF8.GetString(iUsnUuid, 0, iUsnUuid.Length))
                                            {
                                                lock (iLockObject)
                                                {
                                                    foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                    {
                                                        handler.NotifyUuidAlive(iUsnUuid, iLocation, iMaxAge);
                                                    }
                                                }
                                                return;
                                            }
                                        }
                                        break;
                                    case ESsdpTarget.eDeviceType:
                                        if (iUsnTarget == ESsdpTarget.eDeviceType)
                                        {
                                            if (ASCIIEncoding.UTF8.GetString(iDomain, 0, iDomain.Length) == ASCIIEncoding.UTF8.GetString(iUsnDomain, 0, iUsnDomain.Length))
                                            {
                                                if (ASCIIEncoding.UTF8.GetString(iType, 0, iType.Length) == ASCIIEncoding.UTF8.GetString(iUsnType, 0, iUsnType.Length))
                                                {
                                                    if (iVersion <= iUsnVersion)
                                                    {
                                                        lock (iLockObject)
                                                        {
                                                            foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                            {
                                                                handler.NotifyDeviceTypeAlive(iUsnUuid, iDomain, iType, iVersion, iLocation, iMaxAge);
                                                            }
                                                        }
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case ESsdpTarget.eServiceType:
                                        if (iUsnTarget == ESsdpTarget.eServiceType)
                                        {
                                            if (ASCIIEncoding.UTF8.GetString(iDomain, 0, iDomain.Length) == ASCIIEncoding.UTF8.GetString(iUsnDomain, 0, iUsnDomain.Length))
                                            {
                                                if (ASCIIEncoding.UTF8.GetString(iType, 0, iType.Length) == ASCIIEncoding.UTF8.GetString(iUsnType, 0, iUsnType.Length))
                                                {
                                                    if (iVersion <= iUsnVersion)
                                                    {
                                                        lock (iLockObject)
                                                        {
                                                            foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                            {
                                                                handler.NotifyServiceTypeAlive(iUsnUuid, iDomain, iType, iVersion, iLocation, iMaxAge);
                                                            }
                                                        }
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (iHostReceived)
                            {
                                switch (iTarget)
                                {
                                    case ESsdpTarget.eRoot:
                                        if (iUsnTarget == ESsdpTarget.eRoot)
                                        {
                                            lock (iLockObject)
                                            {
                                                foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                {
                                                    handler.NotifyRootByeBye(iUsnUuid);
                                                }
                                            }
                                            return;
                                        }
                                        break;
                                    case ESsdpTarget.eUuid:
                                        if (iUsnTarget == ESsdpTarget.eUuid)
                                        {
                                            if (ASCIIEncoding.UTF8.GetString(iUuid, 0, iUuid.Length) == ASCIIEncoding.UTF8.GetString(iUsnUuid, 0, iUsnUuid.Length))
                                            {
                                                lock (iLockObject)
                                                {
                                                    foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                    {
                                                        handler.NotifyUuidByeBye(iUsnUuid);
                                                    }
                                                }
                                                return;
                                            }
                                        }
                                        break;
                                    case ESsdpTarget.eDeviceType:
                                        if (iUsnTarget == ESsdpTarget.eDeviceType)
                                        {
                                            if (ASCIIEncoding.UTF8.GetString(iDomain, 0, iDomain.Length) == ASCIIEncoding.UTF8.GetString(iUsnDomain, 0, iUsnDomain.Length))
                                            {
                                                if (ASCIIEncoding.UTF8.GetString(iType, 0, iType.Length) == ASCIIEncoding.UTF8.GetString(iUsnType, 0, iUsnType.Length))
                                                {
                                                    if (iVersion <= iUsnVersion)
                                                    {
                                                        lock (iLockObject)
                                                        {
                                                            foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                            {
                                                                handler.NotifyDeviceTypeByeBye(iUsnUuid, iDomain, iType, iVersion);
                                                            }
                                                        }
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    case ESsdpTarget.eServiceType:
                                        if (iUsnTarget == ESsdpTarget.eServiceType)
                                        {
                                            if (ASCIIEncoding.UTF8.GetString(iDomain, 0, iDomain.Length) == ASCIIEncoding.UTF8.GetString(iUsnDomain, 0, iUsnDomain.Length))
                                            {
                                                if (ASCIIEncoding.UTF8.GetString(iType, 0, iType.Length) == ASCIIEncoding.UTF8.GetString(iUsnType, 0, iUsnType.Length))
                                                {
                                                    if (iVersion <= iUsnVersion)
                                                    {
                                                        lock (iLockObject)
                                                        {
                                                            foreach (ISsdpNotifyHandler handler in iNotifyList)
                                                            {
                                                                handler.NotifyServiceTypeByeBye(iUsnUuid, iDomain, iType, iVersion);
                                                            }
                                                        }
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            throw (new HttpError());
        }

        public void WriteMethod(byte[] aMethod, byte[] aUri, EVersion aVersion)
        {
            //LOG(kSsdp, aMethod);
            //LOG(kSsdp, " ");
            //LOG(kSsdp, aUri);
            //LOG(kSsdp, " ");
            //LOG(kSsdp, Version(aVersion));
            //LOG(kSsdp, "\n");

            if (aVersion != EVersion.eHttp11)
            {
                throw (new HttpError());
            }

            string uri = ASCIIEncoding.UTF8.GetString(aUri, 0, aUri.Length);

            if (uri != Ssdp.kSsdpMethodUri)
            {
                throw (new HttpError());
            }

            string method = ASCIIEncoding.UTF8.GetString(aMethod, 0, aMethod.Length);

            if (String.Compare(method, Ssdp.kSsdpMethodMsearch, true) == 0)
            {
                iMethodReceived = true;
                iMethodMsearch = true;
                return;
            }

            if (String.Compare(method, Ssdp.kSsdpMethodNotify, true) == 0)
            {
                iMethodReceived = true;
                iMethodMsearch = false;
                return;
            }

            throw (new HttpError());
        }

        public void WriteHeader(byte[] aField, byte[] aValue)
        {
            // LOG(kSsdp, aField);
            // LOG(kSsdp, ": ");
            // LOG(kSsdp, aValue);
            // LOG(kSsdp, "\n");

            if (iMethodReceived)
            {
                string field = ASCIIEncoding.UTF8.GetString(aField, 0, aField.Length);

                if (iMethodMsearch)
                { // M-SEARCH method
                    if (String.Compare(field, Ssdp.kSsdpHeaderHost, true) == 0)
                    {
                        ReadHost(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderMan, true) == 0)
                    {
                        ReadMan(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderMx, true) == 0)
                    {
                        ReadMx(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderSt, true) == 0)
                    {
                        ReadType(aValue);
                        return;
                    }
                }
                else
                { // NOTIFY method
                    if (String.Compare(field, Ssdp.kSsdpHeaderHost, true) == 0)
                    {
                        ReadHost(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderCacheControl, true) == 0)
                    {
                        ReadCacheControl(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderLocation, true) == 0)
                    {
                        ReadLocation(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderNt, true) == 0)
                    {
                        ReadType(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderNts, true) == 0)
                    {
                        ReadSubType(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderServer, true) == 0)
                    {
                        ReadServer(aValue);
                        return;
                    }

                    if (String.Compare(field, Ssdp.kSsdpHeaderUsn, true) == 0)
                    {
                        ReadUsn(aValue);
                        return;
                    }

                    return;
                }
            }
        }

        // Read methods

        private void ReadHost(byte[] aValue)
        {
            string value = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (value != Ssdp.kSsdpMulticastAddressAndPort)
            {
                if (value != Ssdp.kSsdpMulticastAddress)
                {
                    throw (new HttpError());
                }
            }
            iHostReceived = true;
        }

        private void ReadMan(byte[] aValue)
        {
            string value = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (value != Ssdp.kSsdpMsearchDiscover)
            {
                throw (new HttpError());
            }
            iManReceived = true;
        }

        private void ReadMx(byte[] aValue)
        {
            try
            {
                iMx = Ascii.Ascii.Uint(aValue);
            }
            catch (AsciiError)
            {
                throw (new HttpError());
            }
        }

        private void ReadSubType(byte[] aValue)
        {
            string value = ASCIIEncoding.UTF8.GetString(aValue, 0, aValue.Length);

            if (value == Ssdp.kSsdpAlive)
            {
                iSubTypeReceived = true;
                iSubTypeAlive = true;
                return;
            }

            if (value == Ssdp.kSsdpByeBye)
            {
                iSubTypeReceived = true;
                iSubTypeAlive = false;
                return;
            }

            throw (new HttpError());
        }

        private object iLockObject;
        private UdpMulticastReader iSocket;
        private Srb iBuffer;
        private ReaderRequest iReader;
        private bool iMethodReceived;
        private bool iMethodMsearch;
        private bool iHostReceived;
        private bool iManReceived;
        private uint iMx;
        private bool iSubTypeReceived;
        private bool iSubTypeAlive;

        private List<ISsdpNotifyHandler> iNotifyList;
        private List<ISsdpMsearchHandler> iMsearchList;
    };

    // SsdpListenerUnicast - sends out an msearch request and listens to the unicast responses
    //                     - processes received messages and passes them on an ISsdpNotifyHandler

    public class SsdpListenerUnicast : SsdpListener, IWriterResponse, ISsdpMsearch
    {
        private static readonly int kMaxBufferBytes = 1024;

        public SsdpListenerUnicast(ISsdpNotifyHandler aNotifyHandler)
        {
            iNotifyHandler = aNotifyHandler;
        }

        protected override void OnStart()
        {
            iSearcher = new SsdpMsearcher(iInterface);
            iBuffer = new Srb(kMaxBufferBytes, iSearcher);
            iReader = new ReaderResponse(iBuffer, this);
        }

        protected override void OnStop()
        {
            if(iSearcher != null)
            {
                iSearcher.Shutdown();
                iSearcher.Close();
            }
        }

        protected override void Run()
        {
            iStarted.Set();

            while (!iTerminate)
            {
                iStatusReceived = false;
                iExtReceived = false;
                iTarget = ESsdpTarget.eUnknown;
                iUsnTarget = ESsdpTarget.eUnknown;
                iMaxAge = 0;
                iLocationReceived = false;
                iServerReceived = false;

                try
                {
                    iReader.Read();
                    //LOG(kSsdp, iBuffer.Buffer());
                }
                catch (HttpError)
                {
                    //LOG(kSsdp, "SSDP: HttpError\n");
                }
                catch (ReaderError)
                {
                    //LOG(kSsdp, "SSDP: ReaderError\n");
                }
                catch (ThreadAbortException)
                {
                    //LOG(kSsdp, "SSDP: ReaderError\n");
                    if(iSearcher != null)
                    {
                        iSearcher.Shutdown();
                        iSearcher.Close();
                    }

                    iReader = null;
                    iBuffer = null;
                    iSearcher = null;
                }
            }

            if(iSearcher != null)
            {
                iSearcher.Shutdown();
                iSearcher.Close();
            }

            iReader = null;
            iBuffer = null;
            iSearcher = null;
        }

        // ISsdpMsearch

        public void SsdpMsearchRoot(uint aSeconds)
        {
            if (iSearcher != null)
            {
                iSearcher.SsdpMsearchRoot(aSeconds);
            }
        }

        public void SsdpMsearchUuid(byte[] aUuid, uint aSeconds)
        {
            if (iSearcher != null)
            {
                iSearcher.SsdpMsearchUuid(aUuid, aSeconds);
            }
        }

        public void SsdpMsearchDeviceType(byte[] aDomain, byte[] aType, uint aVersion, uint aSeconds)
        {
            if (iSearcher != null)
            {
                iSearcher.SsdpMsearchDeviceType(aDomain, aType, aVersion, aSeconds);
            }
        }

        public void SsdpMsearchServiceType(byte[] aDomain, byte[] aType, uint aVersion, uint aSeconds)
        {
            if (iSearcher != null)
            {
                iSearcher.SsdpMsearchServiceType(aDomain, aType, aVersion, aSeconds);
            }
        }

        public void SsdpMsearchAll(uint aSeconds)
        {
            if (iSearcher != null)
            {
                iSearcher.SsdpMsearchAll(aSeconds);
            }
        }

        // IWriterResponse

        public void Write(byte aValue)
        {
            Assert.Check(false);
        }

        public void Write(byte[] aBuffer)
        {
            Assert.Check(false);
        }

        public void WriteFlush()
        {
            // LOG(kSsdp, "\n");

            if (iStatusReceived && (iMaxAge > 0) && iExtReceived && iLocationReceived && iServerReceived)
            {
                switch (iTarget)
                {
                    case ESsdpTarget.eRoot:
                        if (iUsnTarget == ESsdpTarget.eRoot)
                        {
                            iNotifyHandler.NotifyRootAlive(iUsnUuid, iLocation, iMaxAge);
                            return;
                        }
                        break;
                    case ESsdpTarget.eUuid:
                        if (iUsnTarget == ESsdpTarget.eUuid)
                        {
                            if (ASCIIEncoding.UTF8.GetString(iUuid, 0, iUuid.Length) == ASCIIEncoding.UTF8.GetString(iUsnUuid, 0, iUsnUuid.Length))
                            {
                                iNotifyHandler.NotifyUuidAlive(iUsnUuid, iLocation, iMaxAge);
                                return;
                            }
                        }
                        break;
                    case ESsdpTarget.eDeviceType:
                        if (iUsnTarget == ESsdpTarget.eDeviceType)
                        {
                            if (ASCIIEncoding.UTF8.GetString(iDomain, 0, iDomain.Length) == ASCIIEncoding.UTF8.GetString(iUsnDomain, 0, iUsnDomain.Length))
                            {
                                if (ASCIIEncoding.UTF8.GetString(iType, 0, iType.Length) == ASCIIEncoding.UTF8.GetString(iUsnType, 0, iUsnType.Length))
                                {
                                    if (iVersion <= iUsnVersion)
                                    {
                                        iNotifyHandler.NotifyDeviceTypeAlive(iUsnUuid, iDomain, iType, iVersion, iLocation, iMaxAge);
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case ESsdpTarget.eServiceType:
                        if (iUsnTarget == ESsdpTarget.eServiceType)
                        {
                            if (ASCIIEncoding.UTF8.GetString(iDomain, 0, iDomain.Length) == ASCIIEncoding.UTF8.GetString(iUsnDomain, 0, iUsnDomain.Length))
                            {
                                if (ASCIIEncoding.UTF8.GetString(iType, 0, iType.Length) == ASCIIEncoding.UTF8.GetString(iUsnType, 0, iUsnType.Length))
                                {
                                    if (iVersion <= iUsnVersion)
                                    {
                                        iNotifyHandler.NotifyServiceTypeAlive(iUsnUuid, iDomain, iType, iVersion, iLocation, iMaxAge);
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            throw (new HttpError());
        }

        public void WriteStatus(Status aStatus, EVersion aVersion)
        {
            // LOG(kSsdp, Version(aVersion));
            // LOG(kSsdp, " %d ", aStatus.Code());
            // LOG(kSsdp, aStatus.Reason());
            // LOG(kSsdp, "\n");

            if (aVersion != EVersion.eHttp11)
            {
                throw (new HttpError());
            }

            if (aStatus.Code() != 200)
            {
                throw (new HttpError());
            }

            if (String.Compare(aStatus.Reason(), Ssdp.kSsdpStatusOk, true) != 0)
            {
                throw (new HttpError());
            }

            iStatusReceived = true;
        }

        public void WriteHeader(byte[] aField, byte[] aValue)
        {
            // LOG(kSsdp, aField);
            // LOG(kSsdp, ": ");
            // LOG(kSsdp, aValue);
            // LOG(kSsdp, "\n");

            if (iStatusReceived)
            {
                string field = ASCIIEncoding.UTF8.GetString(aField, 0, aField.Length);

                if (String.Compare(field, Ssdp.kSsdpHeaderCacheControl, true) == 0)
                {
                    ReadCacheControl(aValue);
                    return;
                }

                if (String.Compare(field, Ssdp.kSsdpHeaderExt, true) == 0)
                {
                    ReadExt(aValue);
                    return;
                }

                if (String.Compare(field, Ssdp.kSsdpHeaderLocation, true) == 0)
                {
                    ReadLocation(aValue);
                    return;
                }

                if (String.Compare(field, Ssdp.kSsdpHeaderServer, true) == 0)
                {
                    ReadServer(aValue);
                    return;
                }

                if (String.Compare(field, Ssdp.kSsdpHeaderSt, true) == 0)
                {
                    ReadType(aValue);
                    return;
                }

                if (String.Compare(field, Ssdp.kSsdpHeaderUsn, true) == 0)
                {
                    ReadUsn(aValue);
                    return;
                }

                return;
            }

            throw (new HttpError());
        }

        private void ReadExt(byte[] aValue)
        {
            if (aValue.Length != 0) {
                throw (new HttpError());
            }
            iExtReceived = true;
        }

        private ISsdpNotifyHandler iNotifyHandler;
        private SsdpMsearcher iSearcher;
        private Srb iBuffer;
        private ReaderResponse iReader;

        private bool iStatusReceived;
        private bool iExtReceived;
    }
}
