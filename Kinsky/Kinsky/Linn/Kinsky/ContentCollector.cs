using System;
using System.Threading;
using System.Collections.Generic;

using Upnp;

namespace Linn.Kinsky
{
    public class ContentCollectorMaster
    {
        // old style content collector - backed by fixed size array
        public static IContentCollector Create(IContainer aContainer, IContentHandler aContentHandler)
        {
            return new ContentCollector(aContainer, aContentHandler);
        }

        // new style content collector - choose your cache, range size, thread count and number of chunks to read ahead at a time
        public static IContentCollector<upnpObject> Create(IContainer aContainer, IContentCache<upnpObject> aCache, int aRangeSize, int aThreadCount, int aReadAheadRanges)
        {
            return new CachingContentCollector<upnpObject>(new ContainerWrapper(aContainer), aCache, aRangeSize, aThreadCount, aReadAheadRanges);
        }
    }

    public class EventArgsItemsFailed : EventArgs
    {
        public EventArgsItemsFailed(int aStartIndex, int aCount, Exception aException)
        {
            iStartIndex = aStartIndex;
            iCount = aCount;
            iException = aException;
        }

        public Exception Exception { get { return iException; } }
        public int StartIndex { get { return iStartIndex; } }
        public int Count { get { return iCount; } }
        private int iStartIndex;
        private int iCount;
        private Exception iException;
    }

    public class EventArgsItemsLoaded<T> : EventArgs
    {
        public EventArgsItemsLoaded(int aStartIndex, IList<T> aItems, ERequestPriority aRequestPriority)
            : base()
        {
            iStartIndex = aStartIndex;
            iItems = aItems;
            iRequestPriority = aRequestPriority;
        }

        public int StartIndex { get { return iStartIndex; } }
        public IList<T> Items { get { return iItems; } }
        public ERequestPriority RequestPriority { get { return iRequestPriority; } }
        private int iStartIndex;
        private IList<T> iItems;
        private ERequestPriority iRequestPriority;
    }

    public interface IContentCollector<T> : IDisposable
    {
        event EventHandler<EventArgs> EventOpened;
        event EventHandler<EventArgsItemsLoaded<T>> EventItemsLoaded;
        event EventHandler<EventArgsItemsFailed> EventItemsFailed;
        T Item(int aIndex, ERequestPriority aPriority);
        int Count { get; }
        bool IsRunning { get; set; }
    }

    public interface IFixedSizeContentCache<T> : IContentCache<T>
    {
        void Initialise(int aMaxItemsCount);
    }

    public interface IContentCache<T>
    {
        void Clear();
        void Add(int aIndex, T aObject);
        void AddRange(int aStartIndex, IList<T> aObjects);
        void Remove(int aIndex);
        bool TryGet(int aIndex, out T aObject);
    }

    public interface IContainer<T>
    {
        int Open();
        IList<T> Items(int aStartIndex, int aCount);
        void Close();
    }

    internal class ContainerWrapper : IContainer<upnpObject>
    {
        public ContainerWrapper(IContainer aContainer)
        {
            iContainer = aContainer;
        }
        private IContainer iContainer;

        #region IContainer<upnpObject> Members

        public int Open()
        {
            return (int)iContainer.Open();
        }

        public IList<upnpObject> Items(int aStartIndex, int aCount)
        {
            return iContainer.Items((uint)aStartIndex, (uint)aCount);
        }

        public void Close()
        {
            iContainer.Close();
        }
        #endregion
    }

    public enum ERequestPriority
    {
        Background,
        Foreground
    }

    public class CachingContentCollector<T> : IContentCollector<T>
    {
        public CachingContentCollector(IContainer<T> aContainer, IContentCache<T> aCache, int aRangeSize, int aThreadCount, int aReadAheadRanges)
        {
            Assert.Check(aContainer != null);
            Assert.Check(aCache != null);
            Assert.Check(aRangeSize > 0);
            iRangeSize = aRangeSize;
            iContainer = aContainer;
            iDequeueScheduler = new Scheduler("ContentCollectorDequeue", aThreadCount);
            iDequeueScheduler.SchedulerError += iScheduler_SchedulerError;
            iReadAheadRanges = aReadAheadRanges;
            iCache = aCache;
            iOpened = false;
            iDisposed = false;
            iQueuedRequests = new List<RangeRequest>();
            iExecutingRequests = new Dictionary<int, int>();
            iQueueLock = new object();
            iIsRunning = true;
            iRunningEvent = new ManualResetEvent(true);

            iDequeueScheduler.Schedule(new Scheduler.DCallback(Open));
        }

        public bool IsRunning
        {
            get
            {
                lock (iQueueLock)
                {
                    return iIsRunning;
                }
            }
            set
            {
                lock (iQueueLock)
                {
                    if (!value)
                    {
                        iRunningEvent.Reset();
                    }
                    iIsRunning = value;
                    if (value)
                    {
                        iRunningEvent.Set();
                    }
                }
            }
        }

        void iScheduler_SchedulerError(object sender, EventArgsSchedulerError e)
        {
            UserLog.WriteLine("Unhandled exception in content collector: " + e.Error.ToString());
            throw e.Error;
        }

        #region IContentCollector<T> Members

        public T Item(int aIndex, ERequestPriority aPriority)
        {
            Assert.Check(iOpened);
            Assert.Check(!iDisposed);
            Assert.Check(aIndex >= 0 && aIndex < iCount);
            T result = default(T);
            if (!iCache.TryGet(aIndex, out result))
            {
                EnqueueRequest(aIndex, aPriority);
            }
            // readahead optimisation for foreground requests
            if (aPriority == ERequestPriority.Foreground && iReadAheadRanges > 0)
            {
                bool ascending = aIndex >= iLastRequest;
                iLastRequest = aIndex;
                ReadAhead(ascending, aIndex, iReadAheadRanges);
            }
            return result;
        }

        private void ReadAhead(bool aAscending, int aIndex, int aRangeCount)
        {
            int offset = aAscending ? (aRangeCount * iRangeSize) : -(aRangeCount * iRangeSize);
            int current = aIndex + offset;
            T outParam = default(T);
            for (int i = 0; i < aRangeCount; i++)
            {
                current = aAscending ? current - iRangeSize : current + iRangeSize;
                if (current >= 0 && current < iCount && !iCache.TryGet(current, out outParam))
                {
                    EnqueueRequest(current, ERequestPriority.Background);
                }
                else
                {
                    break;
                }
            }
        }

        public int Count
        {
            get
            {
                Assert.Check(iOpened && !iDisposed);
                return iCount;
            }
        }

        private EventHandler<EventArgs> iEventOpened;
        public event EventHandler<EventArgs> EventOpened
        {
            add
            {
                if (iOpened && value != null)
                {
                    value(this, EventArgs.Empty);
                }
                iEventOpened += value;
            }

            remove
            {
                iEventOpened -= value;
            }
        }
        public event EventHandler<EventArgsItemsLoaded<T>> EventItemsLoaded;

        private EventHandler<EventArgsItemsFailed> iEventItemsFailed;
        public event EventHandler<EventArgsItemsFailed> EventItemsFailed
        {
            add
            {
                if (iOpenException != null && value != null)
                {
                    value(this, new EventArgsItemsFailed(0, 0, iOpenException));
                }
                iEventItemsFailed += value;
            }

            remove
            {
                iEventItemsFailed -= value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Assert.Check(!iDisposed);
            new Thread(new ThreadStart(() =>
            {
                iDequeueScheduler.Stop();
                iDequeueScheduler.SchedulerError -= iScheduler_SchedulerError;
                if (iOpened)
                {
                    iCache.Clear();
                    iContainer.Close();
                }
            })).Start();
            iDisposed = true;
        }

        #endregion

        private void Open()
        {

            Assert.Check(!iOpened);
            if (!iDisposed)
            {
                try
                {
                    iCount = iContainer.Open();
                    if (iCache is IFixedSizeContentCache<T>)
                    {
                        (iCache as IFixedSizeContentCache<T>).Initialise(iCount);
                    }
                    iOpened = true;
                    EventHandler<EventArgs> eventInstance = iEventOpened;
                    if (eventInstance != null)
                    {
                        eventInstance(this, EventArgs.Empty);
                    }
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("Exception caught opening container: " + e.Message + (e.InnerException != null ? (", " + e.InnerException.Message) : ""));
                    iOpenException = e;
                    EventHandler<EventArgsItemsFailed> eventItemsFailed = iEventItemsFailed;
                    if (eventItemsFailed != null)
                    {
                        eventItemsFailed(this, new EventArgsItemsFailed(0, 0, iOpenException));
                    }
                }
            }
        }

        private void EnqueueRequest(int aIndex, ERequestPriority aPriority)
        {
            if (!iDisposed)
            {
                Assert.Check(iOpened);
                Assert.Check(aIndex >= 0 && aIndex < iCount);
                T item = default(T);
                if (!iCache.TryGet(aIndex, out item))
                {
                    lock (iQueueLock)
                    {
                        // get the correct start index for this request's chunk
                        int startIndex = iRangeSize * (aIndex / iRangeSize);

                        // first ensure that the request is not currently being executed
                        if (iExecutingRequests.ContainsKey(startIndex))
                        {
                            return;
                        }

                        // enqueue the request according to its priority (readahead requests are treated as slightly lower priority).
                        int insertIndex = 0;
                        int existingIndex = -1;
                        RangeRequest existingRequest = null;

                        for (int i = 0; i < iQueuedRequests.Count; i++)
                        {
                            RangeRequest current = iQueuedRequests[i];
                            if (current.Priority <= aPriority)
                            {
                                insertIndex++;
                            }
                            if (iQueuedRequests[i].StartIndex == startIndex)
                            {
                                existingIndex = i;
                                existingRequest = current;
                            }
                            if (existingRequest != null && current.Priority > aPriority)
                            {
                                break;
                            }
                        }
                        if (existingRequest != null)
                        {
                            // if it is already enqueued, but has been re-enqueued at a higher priority, move it up the queue
                            if (insertIndex - 1 > existingIndex)
                            {
                                existingRequest.Priority = aPriority;
                                iQueuedRequests.RemoveAt(existingIndex);
                                iQueuedRequests.Insert(insertIndex - 1, existingRequest);
                            }
                        }
                        else
                        {
                            Assert.Check(insertIndex <= iQueuedRequests.Count);
                            iQueuedRequests.Insert(insertIndex, new RangeRequest() { Priority = aPriority, StartIndex = startIndex });
                            iDequeueScheduler.Schedule(new Scheduler.DCallback(DoDequeueRequest));
                        }
                    }
                }
            }
        }

        private void DoDequeueRequest()
        {
            if (!iDisposed)
            {
                Assert.Check(iOpened);
                int startIndex;
                RangeRequest request;
                bool running = false;
                lock (iQueueLock)
                {
                    running = iIsRunning;
                }
                if (!running)
                {
                    iRunningEvent.WaitOne();
                }
                lock (iQueueLock)
                {
                    if (iQueuedRequests.Count == 0)
                    {
                        return;
                    }
                    request = iQueuedRequests[iQueuedRequests.Count - 1];
                    startIndex = request.StartIndex;
                    iQueuedRequests.RemoveAt(iQueuedRequests.Count - 1);
                    Assert.Check(!iExecutingRequests.ContainsKey(startIndex));
                    iExecutingRequests.Add(startIndex, startIndex);
                }
                try
                {
                    int count = Math.Min(iCount - startIndex, iRangeSize);
                    IList<T> aItems = iContainer.Items(startIndex, count);
                    Assert.Check(aItems.Count == count);
                    iCache.AddRange(startIndex, aItems);
                    if (EventItemsLoaded != null)
                    {
                        EventItemsLoaded(this, new EventArgsItemsLoaded<T>(startIndex, aItems, request.Priority));
                    }
                }
                catch (Exception ex)
                {
                    EventHandler<EventArgsItemsFailed> eventItemsFailed = iEventItemsFailed;
                    if (eventItemsFailed != null)
                    {
                        eventItemsFailed(this, new EventArgsItemsFailed(startIndex, iRangeSize, ex));
                    }
                }

                lock (iQueueLock)
                {
                    iExecutingRequests.Remove(startIndex);
                }
            }
        }

        private volatile bool iOpened;
        private volatile bool iDisposed;
        private IContainer<T> iContainer;
        private int iCount;
        private Scheduler iDequeueScheduler;
        private List<RangeRequest> iQueuedRequests;
        private Dictionary<int, int> iExecutingRequests;
        private int iRangeSize;
        private int iLastRequest;
        private object iQueueLock;
        private IContentCache<T> iCache;
        private int iReadAheadRanges;
        private Exception iOpenException;
        private bool iIsRunning;
        private ManualResetEvent iRunningEvent;

        private class RangeRequest
        {
            public ERequestPriority Priority { get; set; }
            public int StartIndex { get; set; }
        }
    }


    public class DictionaryBackedContentCache<T> : IContentCache<T>
    {
        public DictionaryBackedContentCache(int aSize, Dictionary<int, T> aDictionary)
        {
            iSize = aSize;
            iCache = aDictionary;
            iCacheUseage = new List<int>();
            iLock = new object();
        }

        public DictionaryBackedContentCache(int aSize)
            : this(aSize, new Dictionary<int, T>())
        {
        }

        #region IContentCollectorCache Members

        public void Clear()
        {
            lock (iLock)
            {
                iCache.Clear();
                iCacheUseage.Clear();
                Assert.Check(iCache.Count == iCacheUseage.Count);
            }
        }


        public void Add(int aIndex, T aItem)
        {
            lock (iLock)
            {
                if (iCache.ContainsKey(aIndex))
                {
                    iCacheUseage.Remove(aIndex);
                    iCacheUseage.Add(aIndex);
                }
                else
                {
                    iCache[aIndex] = aItem;
                    iCacheUseage.Add(aIndex);
                    if (iCacheUseage.Count > iSize)
                    {
                        iCache.Remove(iCacheUseage[0]);
                        iCacheUseage.RemoveAt(0);
                    }
                }

                Assert.Check(iCache.Count == iCacheUseage.Count);
            }
        }

        public void AddRange(int aStartIndex, IList<T> aItems)
        {
            lock (iLock)
            {
                for (int i = 0; i < aItems.Count; i++)
                {
                    Add(aStartIndex + i, aItems[i]);
                }
            }
        }

        public void Remove(int aIndex)
        {
            lock (iLock)
            {
                if (iCache.ContainsKey(aIndex))
                {
                    iCache.Remove(aIndex);
                    iCacheUseage.Remove(aIndex);
                }
                Assert.Check(iCache.Count == iCacheUseage.Count);
            }
        }


        public bool TryGet(int aIndex, out T aObject)
        {
            lock (iLock)
            {
                if (iCache.ContainsKey(aIndex))
                {
                    iCacheUseage.Remove(aIndex);
                    iCacheUseage.Add(aIndex);
                    aObject = iCache[aIndex];
                    return true;
                }
                aObject = default(T);
                return false;
            }
        }

        #endregion

        private int iSize;
        private Dictionary<int, T> iCache;
        private List<int> iCacheUseage;
        private object iLock;
    }

    public class ArrayBackedContentCache<T> : IFixedSizeContentCache<T>
    {
        public ArrayBackedContentCache()
        {
            iLock = new object();
            iInitialised = false;
        }

        #region IContentCollectorCache Members

        public void Clear()
        {
            Assert.Check(iInitialised);
            lock (iLock)
            {
                Assert.Check(iCache != null);
                for (int i = 0; i < iCache.Length; i++)
                {
                    iCache[i] = default(T);
                }
            }
        }


        public void Add(int aIndex, T aItem)
        {
            Assert.Check(iInitialised);
            lock (iLock)
            {
                Assert.Check(iCache != null && iCache.Length > aIndex);
                iCache[aIndex] = aItem;
            }
        }

        public void AddRange(int aStartIndex, IList<T> aItems)
        {
            Assert.Check(iInitialised);
            lock (iLock)
            {
                for (int i = 0; i < aItems.Count; i++)
                {
                    Add(aStartIndex + i, aItems[i]);
                }
            }
        }

        public void Remove(int aIndex)
        {
            Assert.Check(iInitialised);
            lock (iLock)
            {
                Assert.Check(iCache != null && iCache.Length > aIndex);
                iCache[aIndex] = default(T);
            }
        }


        public bool TryGet(int aIndex, out T aObject)
        {
            Assert.Check(iInitialised);
            lock (iLock)
            {
                Assert.Check(iCache != null && iCache.Length > aIndex);
                if (iCache[aIndex] != null)
                {
                    aObject = iCache[aIndex];
                    return true;
                }
                aObject = default(T);
                return false;
            }
        }

        public void Initialise(int aMaxItemsCount)
        {
            lock (iLock)
            {
                iCache = new T[aMaxItemsCount];
                iInitialised = true;
            }
        }

        #endregion

        private T[] iCache;
        private object iLock;
        private bool iInitialised;
    }


    public interface IContentHandler
    {
        void Open(IContentCollector aCollector, uint aCount);
        void Item(IContentCollector aCollector, uint aIndex, upnpObject aObject);
        void Items(IContentCollector aCollector, uint aStartIndex, IList<upnpObject> aObjects);
        void ContentError(IContentCollector aCollector, string aMessage);
    }

    public interface IContentCollector : IDisposable
    {
        upnpObject Item(uint aIndex);
        void Range(uint aStartIndex, uint aCount);
    }

    internal class ContentCollector : IContentCollector
    {
        private object iContentHandlerLock;
        public ContentCollector(IContainer aContainer, IContentHandler aContentHandler)
        {
            iContentHandlerLock = new object();
            iContainer = aContainer;
            iContentHandler = aContentHandler;

            iStartIndex = -1;
            iCount = -1;

            iExit = false;
            iMutex = new Mutex(false);
            iEvent = new AutoResetEvent(false);

            iThread = new Thread(Run);
            iThread.Name = "ContentCollector";
            iThread.IsBackground = true;
            iThread.Start();
        }

        public void Dispose()
        {
            iMutex.WaitOne();

            lock (iContentHandlerLock)
            {
                iContentHandler = null;
            }
            iExit = true;
            iEvent.Set();

            iMutex.ReleaseMutex();
        }

        public upnpObject Item(uint aIndex)
        {
            upnpObject item = null;

            iMutex.WaitOne();

            if (iContent != null)
            {
                item = iContent[aIndex];
                if (item == null)
                {
                    int startIndex = (int)aIndex - 50;
                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                    int count = 100;
                    if (startIndex + count > iContent.Length)
                    {
                        count = iContent.Length - startIndex;
                    }

                    if ((iStartIndex == -1 && iCount == -1) || (startIndex > iStartIndex + iCount && startIndex + count < iStartIndex))
                    {
                        Trace.WriteLine(Trace.kKinsky, "item at " + aIndex + " not found downloading range(" + startIndex + "," + count + ")");
                        Range((uint)startIndex, (uint)count);
                    }
                }
            }

            iMutex.ReleaseMutex();

            return item;
        }

        public void Range(uint aStartIndex, uint aCount)
        {
            iMutex.WaitOne();

            Assert.Check(iContent != null);
            Assert.Check(aStartIndex + aCount <= iContent.Length);

            iRangeUpdated = true;
            iStartIndex = (int)aStartIndex;
            Assert.Check(iStartIndex >= 0);
            iCount = (int)aCount;
            Assert.Check(iCount >= 0);
            iEvent.Set();

            iMutex.ReleaseMutex();
        }

        private void Run()
        {
            bool errorRaised = false;
            uint total = 0;
            try
            {
                total = iContainer.Open();
            }
            catch (Exception e)
            {
                // container has died
                UserLog.WriteLine(DateTime.Now + ": Exception caught opening container (" + e.Message + (e.InnerException != null ? (", " + e.InnerException.Message) : "") + ")");
                IContentHandler handler;
                lock (iContentHandlerLock)
                {
                    handler = iContentHandler;
                }
                if (handler != null)
                {
                    handler.ContentError(this, e.Message);
                }
                errorRaised = true;
            }

            iMutex.WaitOne();

            iContent = new upnpObject[(int)total];

            iMutex.ReleaseMutex();
            if (!errorRaised)
            {
                IContentHandler handler;
                lock (iContentHandlerLock)
                {
                    handler = iContentHandler;
                }
                if (handler != null)
                {
                    handler.Open(this, total);
                }
            }

            iMutex.WaitOne();

            while (!iExit)
            {
                iMutex.ReleaseMutex();

                iEvent.WaitOne();

                iMutex.WaitOne();

                IContentHandler handler;
                lock (iContentHandlerLock)
                {
                    handler = iContentHandler;
                }

                if (iRangeUpdated)
                {
                    Assert.Check(iExit || iStartIndex >= 0);
                    Assert.Check(iExit || iCount >= 0);

                    uint startIndex = (uint)iStartIndex;
                    uint index = (uint)iStartIndex;
                    uint totalCount = (uint)iCount;
                    uint count = 0;
                    iRangeUpdated = false;
                    while (!iExit && !iRangeUpdated && count < totalCount)
                    {
                        iMutex.ReleaseMutex();
                        DidlLite didl = new DidlLite();
                        try
                        {
                            uint callCount = (startIndex + totalCount) - index;
                            if (callCount > kCountPerCall)
                            {
                                callCount = kCountPerCall;
                            }

                            for (int i = (int)index; i < (int)(index + callCount) && i < iContent.Length; ++i)
                            {
                                if (iContent[i] != null)
                                {
                                    didl.Add(iContent[i]);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (didl.Count == 0)
                            {
                                didl = iContainer.Items(index, callCount);
                            }
                        }
                        catch (Exception e)
                        {
                            iMutex.WaitOne();

                            // container has died
                            UserLog.WriteLine(DateTime.Now + ": Exception caught retrieving container content (" + e.Message + (e.InnerException != null ? (", " + e.InnerException.Message) : "") + ")");
                            UserLog.WriteLine(DateTime.Now + e.StackTrace);
                            if (!errorRaised)
                            {
                                lock (iContentHandlerLock)
                                {
                                    handler = iContentHandler;
                                }
                                if (handler != null)
                                {
                                    handler.ContentError(this, e.Message);
                                }
                                errorRaised = true;
                            }
                            break;
                        }

                        iMutex.WaitOne();

                        if (iExit || total < index + didl.Count)
                        {
                            if (total < index + didl.Count)
                            {
                                UserLog.WriteLine(DateTime.Now + ": Error: " + iContainer.Metadata.Title + " returned more items than expected");

                                lock (iContentHandlerLock)
                                {
                                    handler = iContentHandler;
                                }
                                if (handler != null)
                                {
                                    handler.ContentError(this, iContainer.Metadata.Title + " returned more items than expected");
                                }
                            }
                            break;
                        }

                        iMutex.ReleaseMutex();

                        for (uint i = 0; i < didl.Count; ++i)
                        {
                            upnpObject o = didl[(int)i];
                            iContent[index + i] = o;

                            lock (iContentHandlerLock)
                            {
                                handler = iContentHandler;
                            }
                            if (handler != null)
                            {
                                handler.Item(this, index + i, o);
                            }
                        }

                        lock (iContentHandlerLock)
                        {
                            handler = iContentHandler;
                        }
                        if (handler != null)
                        {
                            handler.Items(this, index, didl);
                        }

                        index += (uint)didl.Count;
                        count += (uint)didl.Count;

                        iMutex.WaitOne();

                        if (!iRangeUpdated)
                        {
                            iStartIndex = -1;
                            iCount = -1;
                        }
                    }
                }

            }

            iMutex.ReleaseMutex();

            iContainer.Close();

            Trace.WriteLine(Trace.kKinsky, "ContentCollector for " + DidlLiteAdapter.Title(iContainer.Metadata) + " disposed");
        }

        private const uint kCountPerCall = 100;
        private bool iExit;

        private Mutex iMutex;
        private Thread iThread;
        private AutoResetEvent iEvent;

        private bool iRangeUpdated;
        private int iStartIndex;
        private int iCount;

        private upnpObject[] iContent;
        private IContainer iContainer;
        private IContentHandler iContentHandler;
    }
}
