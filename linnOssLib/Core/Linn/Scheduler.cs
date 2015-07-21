
using System;
using System.Threading;
using System.Collections.Generic;


namespace Linn
{
    public class Fifo<T>
    {
        public Fifo()
        {
            iLock = new object();
            iHasItems = new ManualResetEvent(false);
            iQueue = new Queue<T>();
            iRunning = true;
        }

        public void Push(T aItem)
        {
            lock (iLock)
            {
                iQueue.Enqueue(aItem);
                iHasItems.Set();
            }
        }

        public T Pop()
        {
            while (iRunning)
            {
                iHasItems.WaitOne();

                lock (iLock)
                {
                    if (iQueue.Count > 0 && iRunning)
                    {
                        T item = iQueue.Dequeue();

                        if (iQueue.Count == 0)
                        {
                            iHasItems.Reset();
                        }

                        return item;
                    }
                }
            }
            return default(T);
        }

        public void Start()
        {
            lock (iLock)
            {
                iRunning = true;
            }
        }

        public void Stop()
        {
            lock (iLock)
            {
                iRunning = false;
                iQueue.Clear();
                iHasItems.Set();
            }
        }

        private object iLock;
        private ManualResetEvent iHasItems;
        private Queue<T> iQueue;
        private bool iRunning;
    }

    public class Scheduler
    {
        public delegate void DCallback();
        public delegate void DCallbackWithParams(params object[] aParams);
        public event EventHandler<EventArgsSchedulerError> SchedulerError;

        public Scheduler(string aName, int aThreadCount)
        {
            iThreadCount = aThreadCount;
            iName = aName;
            iLock = new object();
            Start();
        }

        public void Schedule(DCallback aCallback)
        {
            iFifo.Push(new Callback(aCallback));
        }

        public void Schedule(DCallbackWithParams aCallback, params object[] aParams)
        {
            iFifo.Push(new Callback(aCallback, aParams));
        }

        public void Start()
        {
            lock (iLock)
            {
                if (!iRunning)
                {
                    iRunning = true;
                    iFifo = new Fifo<Callback>();
                    iFifo.Start();
                    iThreads = new Thread[iThreadCount];
                    for (int i = 0; i < iThreadCount; i++)
                    {
                        iThreads[i] = new Thread(Run);
                        iThreads[i].IsBackground = true;
                        iThreads[i].Name = iName + i;
                        iThreads[i].Start();
                    }
                }
            }
        }

        public void Stop()
        {
            lock (iLock)
            {
                if (iRunning)
                {
                    iRunning = false;
                    iFifo.Stop();
                    for (int i = 0; i < iThreadCount; i++)
                    {
                        iThreads[i].Join();
                        iThreads[i] = null;
                    }
                }
            }
        }

        private void Run()
        {
            while (iRunning)
            {
                Callback callback = iFifo.Pop();

                try
                {
                    if (iRunning && callback != null)
                    {
                        if (callback.DCallback != null)
                        {
                            callback.DCallback();
                        }
                        else
                        {
                            callback.DCallbackWithParams(callback.Params);
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(Trace.kCore, "Scheduler.Run exception on thread " + Thread.CurrentThread.Name + ":");
                    Trace.WriteLine(Trace.kCore, e);
                    UserLog.WriteLine("SchedulerError: " + e.ToString());
                    if (SchedulerError != null)
                    {
                        SchedulerError(this, new EventArgsSchedulerError(e));
                    }
                }
            }
        }

        private class Callback
        {
            public Callback(DCallback aCallback)
            {
                DCallback = aCallback;
            }
            public Callback(DCallbackWithParams aCallback, params object[] aParams)
            {
                DCallbackWithParams = aCallback;
                Params = aParams;
            }
            public DCallback DCallback { get; set; }
            public DCallbackWithParams DCallbackWithParams { get; set; }
            public object[] Params { get; set; }
        }

        private Thread[] iThreads;
        private Fifo<Callback> iFifo;
        private bool iRunning;
        private int iThreadCount;
        private string iName;
        private object iLock;
    }


    public class EventArgsSchedulerError : EventArgs
    {
        public EventArgsSchedulerError(Exception aError)
            : base()
        {
            iError = aError;
        }

        public Exception Error { get { return iError; } }
        private Exception iError;
    }
}


