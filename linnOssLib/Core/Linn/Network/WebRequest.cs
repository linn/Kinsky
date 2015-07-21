using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Linn
{
    public interface IJob
    {
        void Execute();
        bool HasWork();
    }

    public class JobSendRequest : IJob
    {
        public JobSendRequest(GetResponseStreamCallback aCallback, HttpWebRequest aRequest, byte[] aMessage)
        {
            iRequest = aRequest;
            iMessage = aMessage;
            iCallback = aCallback;
        }

        public void Execute()
        {
            Stream reqStream = null;

            try
            {
                reqStream = iRequest.GetRequestStream();
                reqStream.Write(iMessage, 0, iMessage.Length);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(Trace.kCore, "Exception caught sending HttpWebRequest: " + ex);
            }
            finally
            {
                if (reqStream != null)
                {
                    reqStream.Close();
                    reqStream.Dispose();
                }
            }

            WebRequestPool.QueueJob(new JobGetResponse(iCallback, iRequest));
        }

        public bool HasWork()
        {
            return true;
        }

        private HttpWebRequest iRequest;
        private byte[] iMessage;
        private GetResponseStreamCallback iCallback;
    }


    public delegate void GetResponseStreamCallback(object aResult);
    public class JobGetResponse : IJob
    {
        public JobGetResponse(GetResponseStreamCallback aCallback, object aResult)
        {
            iResult = aResult;
            iCallback = aCallback;
        }

        public void Execute()
        {
            iCallback(iResult);
        }

        public bool HasWork()
        {
            return iCallback != null;
        }

        private object iResult;
        private GetResponseStreamCallback iCallback;
    }


    public class WebRequestPool
    {
        static WebRequestPool()
        {
            iLock = new object();
            iHasFreeThreads = new ManualResetEvent(true);

            iFreePool = new Stack<WebRequestThread>();
            iWorkPool = new List<WebRequestThread>();

            for (int i = 0; i < kMinNumThreads; ++i)
            {
                iFreePool.Push(new WebRequestThread());
            }
        }

        public static void QueueJob(IJob aJob)
        {
            if (!aJob.HasWork())
                return;

            WebRequestThread t = null;
            while (t == null)
            {
                List<WebRequestThread> threadsToDelete = new List<WebRequestThread>();

                // get a free thread from the pool
                lock (iLock)
                {
                    // remove any excess threads from the free pool
                    while (iFreePool.Count > kMinNumThreads)
                    {
                        threadsToDelete.Add(iFreePool.Pop());
                    }

                    // get a free thread, if available
                    if (iFreePool.Count > 0)
                    {
                        t = iFreePool.Pop();
                        Trace.WriteLine(Trace.kCore, "WebRequestPool.QueueJob: popped iFreePool new count = " + iFreePool.Count);

                        if (iFreePool.Count == 0)
                        {
                            iHasFreeThreads.Reset();
                        }
                    }
                }

                // cleanup excess threads
                foreach (WebRequestThread toDelete in threadsToDelete)
                {
                    toDelete.Dispose();
                    Trace.WriteLine(Trace.kCore, "WebRequestPool.QueueJob: deleted WebRequestThread");
                }

                // if no threads are free, create a new one if allowed
                if (t == null && kCanCreateThreads)
                {
                    try
                    {
                        t = new WebRequestThread();
                        Trace.WriteLine(Trace.kCore, "WebRequestPool.QueueJob: created WebRequestThread");
                    }
                    catch (OutOfMemoryException)
                    {
                        Trace.WriteLine(Trace.kCore, "WebRequestPool.QueueJob: failed to create WebRequestThread");
                    }
                }

                // wait for a free thread if there are none available and could not create one
                if (t == null)
                {
                    Trace.WriteLine(Trace.kCore, "WebRequestPool.QueueJob: no threads available, waiting...");
                    iHasFreeThreads.WaitOne();
                }
            }

            // add the thread to the work pool
            lock (iLock)
            {
                iWorkPool.Add(t);
                Trace.WriteLine(Trace.kCore, "WebRequestPool.QueueJob: iFreePool.Count=" + iFreePool.Count + ", iWorkPool.Count=" + iWorkPool.Count);
            }

            t.Start(aJob);
        }

        internal static void JobFinished(WebRequestThread aThread)
        {
            lock (iLock)
            {
                iWorkPool.Remove(aThread);

                iFreePool.Push(aThread);
                if (iFreePool.Count == 1)
                {
                    iHasFreeThreads.Set();
                }

                Trace.WriteLine(Trace.kCore, "WebRequestPool.JobFinished: iFreePool.Count=" + iFreePool.Count + ", iWorkPool.Count=" + iWorkPool.Count);
            }
        }

        private const bool kCanCreateThreads = true;
        private const int kMinNumThreads = 20;

        private static object iLock;
        private static ManualResetEvent iHasFreeThreads;

        private static Stack<WebRequestThread> iFreePool;
        private static List<WebRequestThread> iWorkPool;
    }


    internal class WebRequestThread : IDisposable
    {
        public WebRequestThread()
        {
            iJob = null;
            iEvent = new ManualResetEvent(false);

            iThread = new Thread(Run);
            iThread.Name = "WebRequestThread{" + iThread.ManagedThreadId + "}";
            iThread.IsBackground = true;

            iThread.Start();
        }

        public void Start(IJob aJob)
        {
            Assert.Check(iThread != null);
            Assert.Check(iJob == null);
            Assert.Check(aJob != null);
            Trace.WriteLine(Trace.kCore, "WebRequestThread.Start() on thread " + iThread.Name);

            iJob = aJob;
            iEvent.Set();
        }

        public void Dispose()
        {
            Assert.Check(iThread != null);
            Assert.Check(iJob == null);
            Trace.WriteLine(Trace.kCore, "WebRequestThread.Dispose() on thread " + iThread.Name);

            iEvent.Set();
            iThread.Join();
            iThread = null;
        }

        private void Run()
        {
            while (true)
            {
                // wait for the thread to be woken
                iEvent.WaitOne();
                Trace.WriteLine(Trace.kCore, "WebRequestThread.Run() wake on thread " + iThread.Name);

                // check for termination
                if (iJob == null)
                {
                    break;
                }

                // run the job
                try
                {
                    iJob.Execute();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(Trace.kCore, "WebRequestThread.Run() exception on thread " + iThread.Name);
                    Trace.WriteLine(Trace.kCore, e);
                }

                // clear the job and notify the WebRequestPool
                iJob = null;
                iEvent.Reset();

                Trace.WriteLine(Trace.kCore, "WebRequestThread.Run() finished on thread " + iThread.Name);

                WebRequestPool.JobFinished(this);
            }
        }

        private ManualResetEvent iEvent;
        private Thread iThread;
        private IJob iJob;
    }
}
