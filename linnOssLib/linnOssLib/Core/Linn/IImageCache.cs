using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Linn
{
    public interface IImage<ImageType>
    {
        ImageType Image { get; }
        int SizeBytes { get; }
        int ReferenceCount { get; }
        void IncrementReferenceCount();
        void DecrementReferenceCount();
    }

    public interface IImageLoader<ImageType>
    {
        IImage<ImageType> LoadImage(Uri aUri, int aDownscaleImageSize);
    }

    public class EventArgsImage<ImageType> : EventArgs
    {
        public EventArgsImage(string aUri, IImage<ImageType> aImage)
        {
            Uri = aUri;
            Image = aImage;
        }

        public string Uri;
        public IImage<ImageType> Image;
    }

    public class EventArgsImageFailure : EventArgs
    {
        public EventArgsImageFailure(string aUri)
        {
            Uri = aUri;
        }

        public string Uri;
    }

    public interface IImageCache<ImageType>
    {
        void Clear();
        event EventHandler<EventArgsImage<ImageType>> EventImageAdded;
        event EventHandler<EventArgsImageFailure> EventRequestFailed;
        event EventHandler<EventArgsImageFailure> EventRequestCancelled;
        IImage<ImageType> Image(string aUri);
        uint MaxCacheSize { get; }
        void Add(string aUri, IImage<ImageType> aImage);
        void Remove(string aUri);
        bool Contains(string aUri);
        bool IsRunning { get; set; }
        void CancelRequest(string aUri);
    }

    public interface IImageUriConverter
    {
        string Convert(string aUri);
    }

    public class ThreadedImageCache<ImageType> : IImageCache<ImageType>
    {

        public ThreadedImageCache(int aMaxSize, int aDownscaleImageSize, int aThreadCount, IImageLoader<ImageType> aImageLoader)
            : this(aMaxSize, aDownscaleImageSize, aThreadCount, aImageLoader, kNoPendingRequestLimit) { }

        public ThreadedImageCache(int aMaxSize, int aDownscaleImageSize, int aThreadCount, IImageLoader<ImageType> aImageLoader, int aPendingRequestLimit)
        {
            iPendingRequestLimit = aPendingRequestLimit;
            iLockObject = new object();
            iDownscaleImageSize = aDownscaleImageSize;
            iMaxCacheSize = aMaxSize;
            iImageCache = new Dictionary<string, IImage<ImageType>>();
            iImageCacheFailures = new List<string>();
            iImageCacheUsage = new List<string>();
            iImageLoader = aImageLoader;

            iEvent = new ManualResetEvent(false);
            iPendingRequests = new List<ImageRequest>();
            iExecutingRequests = new List<ImageRequest>();
            iIsRunning = true;

            iThreads = new List<Thread>();

            for (int i = 0; i < aThreadCount; i++)
            {
                Thread t = new Thread(ProcessRequests);
                t.IsBackground = true;
                t.Name = "ImageCache" + i;
                t.Start();
                iThreads.Add(t);
            }
        }

        public int DownscaleImageSize
        {
            set
            {
                lock (iLockObject)
                {
                    iDownscaleImageSize = value;
                    Clear(false);
                }
            }
        }

        public IImageLoader<ImageType> ImageLoader
        {
            set
            {
                Assert.Check(value != null);
                lock (iLockObject)
                {
                    iImageLoader = value;
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                lock (iLockObject)
                {
                    return iIsRunning;
                }
            }
            set
            {
                lock (iLockObject)
                {
                    iIsRunning = value;
                    if (value && iPendingRequests.Count > 0)
                    {
                        iEvent.Set();
                    }
                }
            }
        }

        public IImage<ImageType> Image(string aUri)
        {
            List<ImageRequest> cancelledRequests = new List<ImageRequest>();
            bool inFailedList = false;
            IImage<ImageType> image = null;
            lock (iLockObject)
            {
                if (!iImageCache.TryGetValue(aUri, out image))
                {
                    if (iExecutingRequests.Find((i) => { return (i.Uri == aUri); }) == null)
                    {
                        ImageRequest request = iPendingRequests.Find((i) => { return (i.Uri == aUri); });
                        if (request != null)
                        {
                            iPendingRequests.Remove(request);
                            request.Count += 1;
                        }

                        iPendingRequests.Insert(0, request != null ? request : new ImageRequest() { Uri = aUri } );
                    }

                    while (iPendingRequestLimit != kNoPendingRequestLimit && iPendingRequests.Count > iPendingRequestLimit)
                    {
                        ImageRequest cancelled = iPendingRequests[iPendingRequests.Count - 1];
                        cancelledRequests.Add(cancelled);
                        iPendingRequests.RemoveAt(iPendingRequests.Count - 1);
                    }

                    if (IsRunning)
                    {
                        iEvent.Set();
                    }
                }
                else if (iImageCacheFailures.Contains(aUri))
                {
                    inFailedList = true;
                }
                else
                {
                    iImageCacheUsage.Remove(aUri);
                    iImageCacheUsage.Add(aUri);
                }

            }
            foreach (ImageRequest cancelled in cancelledRequests)
            {
                OnEventRequestCancelled(cancelled.Uri);
            }
            if (inFailedList)
            {
                OnEventRequestFailed(aUri);
            }
            return image;
        }

        public void CancelRequest(string aUri)
        {
            ImageRequest cancelled = null;
            lock (iLockObject)
            {
                ImageRequest request = iPendingRequests.Find((i) => { return (i.Uri == aUri); });
                if (request != null)
                {
                    request.Count -= 1;
                    if (request.Count == 0)
                    {
                        iPendingRequests.Remove(request);
                        cancelled = request;
                    }
                }
            }
            if (cancelled != null)
            {
                OnEventRequestCancelled(cancelled.Uri);
            }
        }

        public bool Contains(string aUri)
        {
            lock (iLockObject)
            {
                return iImageCache.ContainsKey(aUri);
            }
        }

        public void Clear()
        {
            Clear(true);
        }

        public void Clear(bool aClearPending)
        {
            lock (iLockObject)
            {
                foreach (IImage<ImageType> image in iImageCache.Values)
                {
                    image.DecrementReferenceCount();
                }
                if (aClearPending)
                {
                    iPendingRequests.Clear();
                }
                iImageCache.Clear();
                iImageCacheUsage.Clear();
                iImageCacheFailures.Clear();
                iCurrentCacheSize = 0;
            }
        }

        public void Add(string aUri, IImage<ImageType> aImage)
        {
            lock (iLockObject)
            {
                if (!iImageCache.ContainsKey(aUri))
                {
                    int imageSize = aImage.SizeBytes;
                    iCurrentCacheSize += imageSize;
                    iImageCache.Add(aUri, aImage);
                    aImage.IncrementReferenceCount();
                    iImageCacheUsage.Add(aUri);
                    RemoveStaleCacheItems();
                }
            }
        }

        public void Remove(string aUri)
        {
            lock (iLockObject)
            {
                if (iImageCache.ContainsKey(aUri))
                {
                    IImage<ImageType> image = iImageCache[aUri] as IImage<ImageType>;
                    iImageCache.Remove(aUri);
                    iImageCacheUsage.Remove(aUri);
                    iCurrentCacheSize -= image.SizeBytes;
                    image.DecrementReferenceCount();
                }
                else if (iImageCacheFailures.Contains(aUri))
                {
                    iImageCacheFailures.Remove(aUri);
                }
            }
        }

        public uint MaxCacheSize
        {
            get
            {
                return (uint)iMaxCacheSize;
            }
        }

        public event EventHandler<EventArgsImage<ImageType>> EventImageAdded;
        public event EventHandler<EventArgsImageFailure> EventRequestFailed;
        public event EventHandler<EventArgsImageFailure> EventRequestCancelled;

        private void ProcessRequests()
        {
            while (true)
            {
                iEvent.WaitOne();

                Monitor.Enter(iLockObject);

                if (iPendingRequests.Count > 0 && IsRunning)
                {
                    ImageRequest request = iPendingRequests[0];
                    IImageLoader<ImageType> loader = iImageLoader;
                    iPendingRequests.Remove(request);
                    iExecutingRequests.Add(request);
                    Monitor.Exit(iLockObject);
                    try
                    {
                        IImage<ImageType> img = loader.LoadImage(new Uri(request.Uri), iDownscaleImageSize);
                        img.IncrementReferenceCount();
                        lock (iLockObject)
                        {
                            Add(request.Uri, img);
                            iExecutingRequests.Remove(request);
                        }
                        OnEventImageAdded(request, img);
                        img.DecrementReferenceCount();
                    }
                    catch (Exception ex)
                    {
                        UserLog.WriteLine("Error downloading image: " + request.Uri + ", " + ex.ToString());
                        lock (iLockObject)
                        {
                            iImageCacheFailures.Add(request.Uri);
                            iExecutingRequests.Remove(request);
                        }
                        OnEventRequestFailed(request.Uri);
                    }
                }
                else
                {
                    iEvent.Reset();
                    Monitor.Exit(iLockObject);
                }
            }
        }

        private void OnEventImageAdded(ImageRequest aRequest, IImage<ImageType> aImage)
        {
            EventHandler<EventArgsImage<ImageType>> del = EventImageAdded;
            if (del != null)
            {
                del(this, new EventArgsImage<ImageType>(aRequest.Uri, aImage));
            }
        }

        private void OnEventRequestFailed(string aUri)
        {
            EventHandler<EventArgsImageFailure> del = EventRequestFailed;
            if (del != null)
            {
                del(this, new EventArgsImageFailure(aUri));
            }
        }

        private void OnEventRequestCancelled(string aUri)
        {
            EventHandler<EventArgsImageFailure> del = EventRequestCancelled;
            if (del != null)
            {
                del(this, new EventArgsImageFailure(aUri));
            }
        }


        private void RemoveStaleCacheItems()
        {
            while (iCurrentCacheSize > iMaxCacheSize)
            {
                string uriToRemove = iImageCacheUsage[0];
                Remove(uriToRemove);
                Trace.WriteLine(Trace.kKinsky, "ImageCache.RemoveStaleCacheItems: Removed " + uriToRemove);
            }

            Assert.Check(iImageCache.Count == iImageCacheUsage.Count);
            Assert.Check(iCurrentCacheSize <= iMaxCacheSize);
        }

        private object iLockObject;
        private int iMaxCacheSize;
        private Dictionary<string, IImage<ImageType>> iImageCache;
        private List<string> iImageCacheFailures;
        private List<string> iImageCacheUsage;
        private ManualResetEvent iEvent;
        private List<Thread> iThreads;
        private List<ImageRequest> iPendingRequests;
        private int iCurrentCacheSize;
        private int iDownscaleImageSize;
        private IImageLoader<ImageType> iImageLoader;
        private bool iIsRunning;
        private const int kNoPendingRequestLimit = ~0;
        private int iPendingRequestLimit;
        private List<ImageRequest> iExecutingRequests;

        private class ImageRequest
        {
            public ImageRequest()
            {
                Count = 1;
            }
            public string Uri { get; set; }
            public int Count { get; set; }
        }
    }

    public abstract class AbstractStreamImageLoader<ImageType> : IImageLoader<ImageType>
    {

        public AbstractStreamImageLoader(IImageUriConverter aUriConverter)
        {
            iUriConverter = aUriConverter;
        }

        public IImage<ImageType> LoadImage(Uri aUri, int aDownscaleImageSize)
        {
            using (WebClient web = new WebClient())
            {
                if (iUriConverter != null)
                {
                    aUri = new Uri(iUriConverter.Convert(aUri.OriginalString));
                }
                using (Stream imgData = web.OpenRead(aUri))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        byte[] buffer = new byte[8192];
                        int count = imgData.Read(buffer, 0, buffer.Length);
                        while (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                            count = imgData.Read(buffer, 0, buffer.Length);
                        }
                        memory.Seek(0, SeekOrigin.Begin);
                        return CreateImageFromStream(memory, aDownscaleImageSize != 0, aDownscaleImageSize);
                    }
                }
            }
        }

        protected abstract IImage<ImageType> CreateImageFromStream(MemoryStream aStream, bool aDownscaleImage, int aDownscaleImageSize);
        private IImageUriConverter iUriConverter;
    }

    public class DefaultUriConverter : IImageUriConverter
    {
        #region IImageUriConverter Members

        public string Convert(string aUri)
        {
            return aUri;
        }

        #endregion
    }

    public class ScalingUriConverter : IImageUriConverter
    {
        // aScaleByWidthAndHeight - flag to turn off scaling by width/height as twonky is SLOW in this mode and degrades browser artwork performance
        // iUpscaleOnly - flag to prevent scaling if scaling will result in a lower resolution image being returned
        public ScalingUriConverter(int aDesiredSize, bool aScaleByWidthAndHeight, bool aUpscaleOnly)
        {
            iDesiredSize = aDesiredSize;
            iUpscaleOnly = aUpscaleOnly;
            iScaleByWidthAndHeight = aScaleByWidthAndHeight;
            iSizeRegex = new Regex("(?<prefix>(.*?[?&]size=))(?<size>([0-9]+))(?<suffix>(.*?))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            iWidthHeightRegex = new Regex("(?<prefix>(.*?/W))(?<width>([0-9]+))(?<middle>(/H))(?<height>([0-9]+))(?<suffix>(/.*?))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public string Convert(string aUri)
        {
            try
            {
                if (iSizeRegex.IsMatch(aUri))
                {
                    Match m = iSizeRegex.Match(aUri);
                    double size = double.Parse(m.Groups["size"].Value, System.Globalization.CultureInfo.InvariantCulture);
                    if (!iUpscaleOnly || (size != 0 && size < iDesiredSize))
                    {
                        aUri = iSizeRegex.Replace(aUri, string.Format("${{prefix}}{0}${{suffix}}", iDesiredSize));
                    }
                }
                else if (iScaleByWidthAndHeight && iWidthHeightRegex.IsMatch(aUri))
                {
                    Match m = iWidthHeightRegex.Match(aUri);
                    double width = double.Parse(m.Groups["width"].Value, System.Globalization.CultureInfo.InvariantCulture);
                    double height = double.Parse(m.Groups["height"].Value, System.Globalization.CultureInfo.InvariantCulture);
                    if (!iUpscaleOnly || (width < iDesiredSize && height < iDesiredSize))
                    {
                        if (width == height)
                        {
                            width = iDesiredSize;
                            height = iDesiredSize;
                        }
                        else
                        {
                            if (width > height)
                            {
                                width = iDesiredSize;
                                height = (int)(iDesiredSize * (height / width));
                            }
                            else
                            {
                                width = (int)(iDesiredSize * (width / height));
                                height = iDesiredSize;
                            }
                        }
                        aUri = iWidthHeightRegex.Replace(aUri, string.Format("${{prefix}}{0}${{middle}}{1}${{suffix}}", width, height));
                    }
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Error caught parsing uri: " + aUri + ", " + ex);
            }
            return aUri;
        }


        private int iDesiredSize;
        private Regex iWidthHeightRegex;
        private Regex iSizeRegex;
        private bool iUpscaleOnly;
        private bool iScaleByWidthAndHeight;
    }



}