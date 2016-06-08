using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

using Linn.Network;
using Linn.Ascii;

namespace Linn.Kinsky
{
    public interface IHttpSessionHandler
    {
        Stream GetStream();
        long StreamBytes { get; }
        void Close();
    }

    public class HttpServerException : Exception
    {
        public HttpServerException(string aMessage) : base(aMessage) { }
    }

    public class HttpServer : IVirtualFileSystem
    {
        private class HttpServerSession
        {
            internal class TcpSessionStream : TcpStream
            {
                internal TcpSessionStream()
                {
                }

                internal void SetSocket(Socket aSocket)
                {
                    iSocket = aSocket;
                }
            }

            public HttpServerSession(HttpServer aServer, Socket aSocket)
            {
                iServer = aServer;
                iSession = new TcpSessionStream();
                iReadBuffer = new Srb(kMaxReadBufferBytes, iSession);

                iSession.SetSocket(aSocket);

                iSocket = aSocket;

                iThread = new Thread(new ThreadStart(Run));
                iThread.IsBackground = true;
                iThread.Name = "HttpServerSession";
            }

            public void Start()
            {
                iExiting = false;
                iThread.Start();
            }

            public void Stop()
            {
                Assert.Check(iThread != null);

                iExiting = true;

                iSocket.Close();

                //iThread.Abort();

                iThread.Join();
            }
            private void Run()
            {
                string filename = string.Empty;
                try
                {
                    // GET /Kinsky?file=test.flac HTTP/1.1
                    // Host: x.x.x.x
                    // Connection: close
                    // Range: bytes=0-

                    // get the request header
                    string headers = string.Empty;

                    while (true)
                    {
                        byte[] header = iReadBuffer.ReadUntil(Ascii.Ascii.kAsciiLf);
                        headers += Encoding.ASCII.GetString(header, 0, header.Length).Replace("\r", "") + "\n";
                        if (header.Length == 0 || (header[0] == Ascii.Ascii.kAsciiCr))
                        {
                            break;
                        }
                    }

                    UserLog.WriteLine("HttpServerSession: " + headers);

                    string[] splitRequest = headers.Split('\n');

                    Filename(splitRequest, ref filename);

                    if (filename != string.Empty)
                    {
                        IHttpSessionHandler handler = GetHandler(filename);
                        if (handler == null)
                        {
                            throw new NotSupportedException();
                        }
                        Stream stream = handler.GetStream();
                        try
                        {
                            long startBytes = 0;
                            long endBytes = handler.StreamBytes - 1;
                            bool partialGet = Range(splitRequest, ref startBytes, ref endBytes);

                            stream.Position += startBytes;

                            FileInfo fileInfo = new FileInfo(filename);
                            SendResponseGet(partialGet, fileInfo.Name, startBytes, endBytes, stream.Length);
                            SendResponseFile(stream);
                        }
                        finally
                        {
                            handler.Close();
                            iSocket.Close();
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    try
                    {
                        iSocket.Close();
                    }
                    catch (SocketException) { }
                }
                catch (SocketException e)
                {
                    Trace.WriteLine(Trace.kKinsky, "HttpServerSession: " + e.Message);
                    UserLog.WriteLine("HttpServerSession: " + e.Message);
                }
                catch (Exception e)
                {
                    try
                    {
                        Trace.WriteLine(Trace.kKinsky, "HttpServerSession: " + e.Message);
                        UserLog.WriteLine("HttpServerSession: " + e.Message);
                        SendResponseNotFound(filename);
                        iSocket.Close();
                    }
                    catch (Exception) { }
                }

                if (EventSessionFinished != null)
                {
                    EventSessionFinished(this, EventArgs.Empty);
                }
            }

            private void Filename(string[] aRequest, ref string aFilename)
            {
                foreach (string h in aRequest)
                {
                    if (h.Length > kMethodGet.Length && h.Substring(0, kMethodGet.Length) == kMethodGet)
                    {
                        Trace.WriteLine(Trace.kKinsky, h);

                        string[] request = h.Split(' ');
                        if (request.Length > 2)
                        {
                            string filename = System.Uri.UnescapeDataString(request[1].Replace(HttpServer.kUriPrefix + HttpServer.kUriQuery, ""));
                            string[] split = filename.Split('?');
                            foreach (string s in split)
                            {
                                if (!s.Contains("size="))
                                {
                                    aFilename = s;
                                }
                            }
                        }
                    }
                }
                try
                {
                    FileInfo f = new FileInfo(aFilename);
                    aFilename = f.FullName;
                    Trace.WriteLine(Trace.kKinsky, "HttpServerSession: " + aFilename + " requested");
                    UserLog.WriteLine("HttpServerSession: " + aFilename + " requested");
                }
                catch (Exception e)
                {
                    Trace.WriteLine(Trace.kKinsky, "HttpServerSession: " + aFilename + " requested: " + e.Message);
                    aFilename = string.Empty;
                    UserLog.WriteLine("HttpServerSession: " + aFilename + " requested: " + e.Message);
                }
            }

            private bool Range(string[] aRequest, ref long aStartByte, ref long aEndByte)
            {
                foreach (string h in aRequest)
                {
                    string[] request = h.Split(':');
                    if (request[0] == kHeaderRange)
                    {
                        Trace.WriteLine(Trace.kTest, h);

                        if (request.Length > 1)
                        {
                            string bytes = request[1].Replace("bytes=", "");
                            string[] splitBytes = bytes.Split('-');
                            try
                            {
                                if (splitBytes.Length > 0)
                                {
                                    aStartByte = long.Parse(splitBytes[0]);
                                }
                                if (splitBytes.Length > 1 && splitBytes[1] != string.Empty)
                                {
                                    aEndByte = long.Parse(splitBytes[1]);
                                }

                                Trace.WriteLine(Trace.kKinsky, "HttpServerSession: Range " + aStartByte + " - " + aEndByte);
                                UserLog.WriteLine("HttpServerSession: Range " + aStartByte + " - " + aEndByte);

                                return true;
                            }
                            catch (FormatException)
                            {
                            }
                        }
                    }
                }

                return false;
            }

            private void SendResponseNotFound(string filename)
            {
                string html = "<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML 2.0//EN\">\n";
                html += "<html>\n";
                html += "<head>\n<title>404 Not Found</title>\n</head>\n";
                html += "<body>\n";
                html += "<h1>Not Found</h1>\n<p>The requested URL " + HttpServer.kUriPrefix + HttpServer.kUriQuery + filename + " was not found on this server.</p>\n";
                html += "<hr>";
                html += "Kinsky Server at " + iSocket.LocalEndPoint;
                html += "</body>\n";
                html += "</html>";
                string response = string.Empty;
                response += "HTTP/1.1 404 Not Found\r\n";
                response += "Server: Kinsky\r\n";
                response += "Content-Type: text/html\r\n";
                response += "Accept-Ranges: bytes\r\n";
                response += "Content-Length: " + html.Length + "\r\n";
                response += "Connection: close\r\n";
                response += "\r\n";

                response += html;

                byte[] buffer = Encoding.ASCII.GetBytes(response);
                iSocket.Send(buffer);
            }

            private void SendResponseGet(bool aPartialGet, string aFilename, long aStartBytes, long aEndBytes, long aTotalBytes)
            {
                string response = string.Empty;

                if (aPartialGet)
                {
                    response += "HTTP/1.1 206 Partial Content\r\n";
                    response += "Server: Kinsky\r\n";
                    response += "Content-Disposition: attachment; filename=\"" + aFilename + "\"\r\n";
                    response += "Accept-Ranges: bytes\r\n";
                    response += "Content-Range: bytes " + aStartBytes.ToString() + "-" + aEndBytes.ToString() + "/" + aTotalBytes.ToString() + "\r\n";
                    response += "Content-Length: " + ((aEndBytes - aStartBytes) + 1).ToString() + "\r\n";
                    response += "Connection: close\r\n";
                    response += "\r\n";
                }
                else
                {
                    response += "HTTP/1.1 200 OK\r\n";
                    response += "Server: Kinsky\r\n";
                    response += "Content-Disposition: attachment; filename=\"" + aFilename + "\"\r\n";
                    response += "Accept-Ranges: bytes\r\n";
                    response += "Content-Length: " + aTotalBytes.ToString() + "\r\n";
                    response += "Connection: close\r\n";
                    response += "\r\n";
                }

                byte[] buffer = Encoding.ASCII.GetBytes(response);
                iSocket.Send(buffer);
            }

            private void SendResponseFile(Stream aStream)
            {
                byte[] data = new byte[40960];  // 40Kb
                int bytesread;
                while ((bytesread = aStream.Read(data, 0, 40960)) > 0 && !iExiting)
                {
                    iSocket.Send(data, bytesread, SocketFlags.None);
                }
            }

            private IHttpSessionHandler GetHandler(string aFilename)
            {
                string ext = Path.GetExtension(aFilename).ToLower();
                switch (ext)
                {
                    // audio types
                    case ".aac":
                    case ".aif":
                    case ".aifc":
                    case ".aiff":
                    case ".dpl":
                    case ".flac":
                    case ".m3u":
                    case ".m2a":
                    case ".m4a":
                    case ".mp1":
                    case ".mp2":
                    case ".mp3":
                    case ".mpa":
                    case ".oga":
                    case ".ogg":
                    case ".pls":
                    case ".wav":
                    case ".wave":
                    case ".wma":

                    // image types
                    case ".bmp":
                    case ".gif":
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".tif":
                    case ".tiff":

                    // video types
                    case ".asf":
                    case ".avi":
                    case ".mp4":
                    case ".mpg":
                    case ".mpeg":
                    case ".mov":
                    case ".wmv":
                        return new HttpSessionHandlerFile(aFilename);

                    case ".itc":
                    case ".itc2":
                        return new HttpSessionHandlerItc2(aFilename);
                }

                if (aFilename.ToLower().EndsWith("manifest.xml"))
                    return new HttpSessionHandlerFile(aFilename);

                if (iServer.IsPathPublic(aFilename))
                    return new HttpSessionHandlerFile(aFilename);

                return null;
            }

            public event EventHandler<EventArgs> EventSessionFinished;

            private const int kMaxReadBufferBytes = 8000;
            private const string kMethodGet = "GET";
            private const string kHeaderRange = "Range";

            private HttpServerSession.TcpSessionStream iSession;
            private Srb iReadBuffer;

            private bool iExiting;
            private Thread iThread;
            private Socket iSocket;
            private HttpServer iServer;
        }

        public HttpServer(int aPort)
        {
            iMutex = new Mutex(false);

            iStarted = false;
            iPort = aPort;
            iBaseUri = string.Empty;

            // named handles not supported in later versions of mono for mobile devices - throw a NotSupportedException
            try
            {
                iEvent = new EventWaitHandle(true, EventResetMode.AutoReset, string.Format("KinskyHttpServer{0}", iPort));
            }
            catch(NotSupportedException)
            {
                iEvent = new EventWaitHandle(true, EventResetMode.AutoReset);
            }

            iSessions = new List<HttpServerSession>();
            iPublicPaths = new List<string>();
        }

        public void Start(IPAddress aInterface)
        {
            iThread = new Thread(new ThreadStart(Run));
            iThread.IsBackground = true;
            iThread.Name = "HttpServer";

            iInterface = aInterface;

            IPEndPoint endpoint = new IPEndPoint(iInterface, iPort);
            iBaseUri = "http://" + endpoint.ToString() + kUriPrefix;
            iCancelStart = false;
            iThread.Start();
        }

        public void Stop()
        {
            iCancelStart = true;
            if (iStarted)
            {
                Assert.Check(iThread != null);
                iExit = true;
                try
                {
                    iListener.Stop();
                    UserLog.WriteLine("Stopping HTTP Server at " + iBaseUri + "...Success");
                }
                catch (SocketException e)
                {
                    UserLog.WriteLine("Stopping HTTP Server at " + iBaseUri + "...Failed (" + e.Message + ")");
                    throw (new Linn.Network.NetworkError());
                }

                iThread.Join();

                HttpServer.HttpServerSession[] sessions = iSessions.ToArray();
                foreach (HttpServerSession s in sessions)
                {
                    s.Stop();
                }

                Assert.Check(iSessions.Count == 0);

                iBaseUri = string.Empty;
                iStarted = false;

                iEvent.Set();
            }
        }

        public bool IsPathPublic(string aPath)
        {
            FileInfo info = new FileInfo(aPath);

            iMutex.WaitOne();

            for (int i = 0; i < iPublicPaths.Count; ++i)
            {
                if (iPublicPaths[i] == info.FullName || info.DirectoryName.StartsWith(iPublicPaths[i]))
                {
                    iMutex.ReleaseMutex();

                    return true;
                }
            }

            iMutex.ReleaseMutex();

            return false;
        }

        public void MakePathPublic(string aPath)
        {
            FileInfo info = new FileInfo(aPath);

            iMutex.WaitOne();

            if (!iPublicPaths.Contains(info.FullName))
            {
                iPublicPaths.Add(info.FullName);
            }

            iMutex.ReleaseMutex();
        }

        public string Uri(string aUnescapedFilename)
        {
            if (iBaseUri == string.Empty)
            {
                throw new HttpServerException("Web server is not running");
            }

            string uri = new Uri(iBaseUri + kUriQuery + System.Uri.EscapeDataString(aUnescapedFilename)).AbsoluteUri;

            return uri;
        }

        private void Run()
        {
            while (!iStarted && !iCancelStart)
            {
                iEvent.WaitOne();

                iListener = new TcpListener(iInterface, iPort);
                iListener.Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                try
                {
                    iListener.Start(kListenSlots);
                    UserLog.WriteLine("Starting HTTP Server at " + iBaseUri + "...Success");
                    iStarted = true;
                }
                catch (SocketException)
                {
                    UserLog.WriteLine(String.Format("Unable to start HTTP Server at " + iBaseUri + " on port {0}.", iPort));
                    Timer t = new Timer(5000);
                    EventHandler handler = null;
                    handler = (d, e) =>
                    {
                        UserLog.WriteLine(String.Format("Retry HTTP Server at " + iBaseUri + " on port {0}.", iPort));
                        iEvent.Set();
                        t.Elapsed -= handler;
                        t.Stop();
                        t.Dispose();
                    };
                    t.Elapsed += handler;
                    t.Start();
                }
            }
            if (iStarted)
            {
                try
                {
                    while (!iExit)
                    {
                        Socket socket = iListener.AcceptSocket();

                        iMutex.WaitOne();

                        Trace.WriteLine(Trace.kKinsky, "HttpServer.Run: Socket accepted from " + socket.RemoteEndPoint);
                        UserLog.WriteLine("HttpServer.Run: Socket accepted from " + socket.RemoteEndPoint);

                        HttpServerSession session = new HttpServerSession(this, socket);
                        iSessions.Add(session);

                        Trace.WriteLine(Trace.kKinsky, "HttpServer.Run: " + iSessions.Count + " active sessions");

                        iMutex.ReleaseMutex();

                        session.EventSessionFinished += EventSessionFinished;

                        session.Start();
                    }
                }
                catch (SocketException)
                {
                }

                iExit = false;

                UserLog.WriteLine("HTTP server stopped");
            }
        }

        private void EventSessionFinished(object sender, EventArgs e)
        {
            HttpServerSession session = sender as HttpServerSession;

            session.EventSessionFinished -= EventSessionFinished;

            iMutex.WaitOne();
            iSessions.Remove(session);
            Trace.WriteLine(Trace.kKinsky, "HttpServer.EventSessionFinished: " + iSessions.Count + " active sessions");
            iMutex.ReleaseMutex();
        }

        public const int kPortKinskyDesktop = 50008;
        public const int kPortKinskyJukebox = 50009;
        public const int kPortKinskyClassic = 50010;
        public const int kPortKinskyPda = 50011;
        public const int kPortKinskyWeb = 50012;
        public const int kPortKinskyTouch = 50013;
        public const int kPortKinskyDroid = 50014;
        public const int kPortInstallWizard = 50015;
        public const int kPortKonfig = 50016;

        public const string kUriPrefix = "/Kinsky";
        public const string kUriQuery = "?file=";

        private const int kListenSlots = 100;

        private IPAddress iInterface;
        private int iPort;
        private string iBaseUri;

        private bool iStarted;
        private bool iExit;
        private bool iCancelStart;

        private Mutex iMutex;
        private Thread iThread;
        private TcpListener iListener;

        private EventWaitHandle iEvent;

        private List<HttpServerSession> iSessions;
        private List<string> iPublicPaths;
    }


    public class HttpSessionHandlerFile : IHttpSessionHandler
    {
        public HttpSessionHandlerFile(string aFilename)
        {
            iStream = File.OpenRead(aFilename);
        }

        public Stream GetStream()
        {
            return iStream;
        }

        public long StreamBytes
        {
            get { return iStream.Length; }
        }

        public void Close()
        {
            iStream.Close();
            iStream.Dispose();
            iStream = null;
        }

        private FileStream iStream;
    }


    public class HttpSessionHandlerItc2 : IHttpSessionHandler
    {
        public HttpSessionHandlerItc2(string aFilename)
        {
            iStream = File.OpenRead(aFilename);
            iItc2 = new FileItc2(iStream);
            iItem = null;

            // select largest image
            foreach (FileItc2.Item item in iItc2.Items)
            {
                if (iItem == null || item.ImageWidth > iItem.ImageWidth)
                {
                    iItem = item;
                }
            }

            if (iItem == null)
            {
                throw new FileNotFoundException(aFilename);
            }
        }

        public Stream GetStream()
        {
            return iItem.GetStream();
        }

        public long StreamBytes
        {
            get { return iItem.StreamBytes; }
        }

        public void Close()
        {
            iStream.Close();
            iStream.Dispose();
            iStream = null;
        }

        private FileStream iStream;
        private FileItc2 iItc2;
        private FileItc2.Item iItem;
    }


} // Linn.Kinsky
