using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;

namespace Linn.Kinsky
{
    public interface IHttpClient
    {
        Stream Request(Uri aUri);
        Stream RequestLow(Uri aUri);
        Stream RequestHigh(Uri aUri);
    }

    public class HttpClient : IHttpClient
    {
        public delegate void HttpClientDelegate(Stream aStream);

        internal class HttpClientSession
        {
            public HttpClientSession(Uri aUri)
            {
                iUri = aUri;
                iComplete = new ManualResetEvent(false);
            }

            internal void Run()
            {
                try
                {
                    WebRequest request = WebRequest.Create(iUri);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    iStream = request.GetResponse().GetResponseStream();
                }
                catch (WebException)
                {
                }

                iComplete.Set();
            }

            internal Stream Wait()
            {
                iComplete.WaitOne();

                return (iStream);
            }

            private Uri iUri;
            private ManualResetEvent iComplete;
            private Stream iStream;
        }

        private const ThreadPriority kPriority = ThreadPriority.AboveNormal;

        public HttpClient()
        {
            iStartStopLock = new object();
            iSemaphore = new ManualResetEvent(false);
            iSessions = new List<List<HttpClientSession>>();
            iSessions.Add(new List<HttpClientSession>());
            iSessions.Add(new List<HttpClientSession>());
            iSessions.Add(new List<HttpClientSession>());
        }

        private Stream Request(Uri aUri, int aPriority)
        {
            HttpClientSession session = new HttpClientSession(aUri);
            Add(session, aPriority);
            return (session.Wait());
        }

        public Stream Request(Uri aUri)
        {
            return (Request(aUri, 1));
        }

        public Stream RequestLow(Uri aUri)
        {
            return (Request(aUri, 2));
        }

        public Stream RequestHigh(Uri aUri)
        {
            return (Request(aUri, 0));
        }

        private void Add(HttpClientSession aSession, int aPriority)
        {
            lock (iSessions)
            {
                iSessions[aPriority].Add(aSession);
                iSemaphore.Set();
            }
        }

        public void Start()
        {
            lock (iStartStopLock)
            {
                Assert.Check(!iRunning);

                iThread = new Thread(new ThreadStart(Run));
                iThread.Priority = kPriority;
                iThread.Name = "Http Client";
                iThread.Start();

                Trace.WriteLine(Trace.kKinsky, "HttpClient.Start() successful");
            }
        }

        public void Stop()
        {
            lock (iStartStopLock)
            {
                iRunning = false;
                iSemaphore.Set();
                if (iThread != null)
                {
                    iThread.Join();
                    iThread = null;
                }
                Trace.WriteLine(Trace.kKinsky, "HttpClient.Stop() successful");
            }
        }

        private HttpClientSession GetNextSession()
        {
            foreach (List<HttpClientSession> list in iSessions)
            {
                if (list.Count > 0)
                {
                    HttpClientSession session = list[0];
                    list.RemoveAt(0);
                    return (session);
                }
            }

            return (null);
        }

        private void Run()
        {
            while (iRunning)
            {
                HttpClientSession session;

                try
                {
                    Monitor.Enter(iSessions);

                    session = GetNextSession();

                    if (session == null)
                    {
                        iSemaphore.Reset();

                        try
                        {
                            Monitor.Exit(iSessions);

                            iSemaphore.WaitOne();
                        }
                        finally
                        {
                            Monitor.Enter(iSessions);
                        }

                        session = GetNextSession();
                    }
                }
                finally
                {
                    Monitor.Exit(iSessions);
                }

                if (iRunning)
                {
                    session.Run();
                }
            }
            lock (iSessions)
            {
                foreach (List<HttpClientSession> list in iSessions)
                {
                    list.Clear();
                }
            }
        }

        private object iStartStopLock;
        private ManualResetEvent iSemaphore;
        private List<List<HttpClientSession>> iSessions;
        private Thread iThread;
        private bool iRunning;
    }
} // Linn.Kinsky
