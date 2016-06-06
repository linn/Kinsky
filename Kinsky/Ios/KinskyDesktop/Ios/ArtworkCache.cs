using System;
using System.Threading;
using System.Collections.Generic;

using Linn;

using UIKit;
using Foundation;
using CoreGraphics;

namespace KinskyTouch
{
    // single type class to provide easy global access to the artwork cache
    // during setup of views
    public static class ArtworkCacheInstance
    {
        public static ArtworkCache Instance
        {
            get { return iInstance; }
            set { iInstance = value; }
        }

        private static ArtworkCache iInstance;
    }


    public class ArtworkCache
    {
        /*internal class Connection : NSUrlConnectionDelegate
        {
            public Connection(string aUri, ArtworkCache aCache)
            {
                iUri = aUri;
                iCache = aCache;
                iData = new NSMutableData();
            }
    
            public override void ReceivedData(NSUrlConnection connection, NSData data)
            {
                iData.AppendData(data);
            }
    
            public override void FailedWithError(NSUrlConnection connection, NSError error)
            {
            }
    
            public override void FinishedLoading(NSUrlConnection connection)
            {
                iCache.AddImage(iUri, ImageToFitSize(new UIImage(iData), new SizeF(60.0f, 60.0f)));
                iData = null;
            }
    
            private UIImage ImageToFitSize(UIImage aImage, SizeF aFitSize)
            {
                if(aImage.Size.Width > aFitSize.Width || aImage.Size.Height > aFitSize.Height)
                {
                    double imageScaleFactor = 1.0;
                    imageScaleFactor = aImage.CurrentScale;
            
                    double sourceWidth = aImage.Size.Width * imageScaleFactor;
                    double sourceHeight = aImage.Size.Height * imageScaleFactor;
                    double targetWidth = aFitSize.Width;
                    double targetHeight = aFitSize.Height;
        
                    double sourceRatio = sourceWidth / sourceHeight;
                    double targetRatio = targetWidth / targetHeight;
        
                    bool scaleWidth = (sourceRatio <= targetRatio);
                    scaleWidth = !scaleWidth;
    
                    double scalingFactor, scaledWidth, scaledHeight;
    
                    if (scaleWidth)
                    {
                        scalingFactor = 1.0 / sourceRatio;
                        scaledWidth = targetWidth;
                        scaledHeight = Math.Round(targetWidth * scalingFactor);
                    }
                    else
                    {
                        scalingFactor = sourceRatio;
                        scaledWidth = Math.Round(targetHeight * scalingFactor);
                        scaledHeight = targetHeight;
                    }
    
                    RectangleF destRect = new RectangleF(0, 0, (float)scaledWidth, (float)scaledHeight);
        
                    UIGraphics.BeginImageContextWithOptions(destRect.Size, false, 0.0f);
                    aImage.Draw(destRect);
                    UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
        
                    return newImage;
                }
    
                return aImage;
            }

            private string iUri;
            private ArtworkCache iCache;
            private NSMutableData iData;
        }*/

        public class Item
        {
            public Item(string aUri, UIImage aImage)
            {
                Uri = aUri;
                Failed = (aImage == null);
                Image = aImage;
            }

            public uint ByteCount
            {
                get
                {
                    if (Image != null)
                    {
                        return (uint)(Image.CGImage.Width * Image.CGImage.Height * Image.CGImage.BitsPerPixel) / 8;
                    }

                    return 0;
                }
            }

            public readonly bool Failed;
            public readonly UIImage Image;
            public readonly string Uri;
        }

        public class EventArgsArtwork : EventArgs
        {
            public EventArgsArtwork(string aUri)
            {
                Uri = aUri;
            }
            
            public string Uri;
        }

        public ArtworkCache()
        {
            iLock = new object();

            iPendingRequests = new RequestList();
            iCacheData = new CacheData(kMaxCacheSizeSmall);
			DownscaleSize = (int)kDownscaleSize;

            iThread = new Thread(ProcessRequests);
            iThread.IsBackground = true;
            iThread.Name = "ArtworkCache";
            iThread.Priority = ThreadPriority.Lowest;
            iThread.Start();
        }
		
		public int DownscaleSize
		{
			set
			{
				lock(iLock)
				{
					if (iDownscaleSize != value)
					{
						iDownscaleSize = value;
				 		iUriConverter = new ScalingUriConverter(value, false, false);
						iCacheData.Flush();
					}
				}
			}
		}
		
		public string ScaledUri(string aUri)
		{
			lock(iLock)
			{
				string result = aUri;
				if (iUriConverter != null)
				{
					result = iUriConverter.Convert(aUri);
				}
				return result;
			}
		}

        public Item Artwork(Uri aUri)
        {
            lock (iLock)
            {
                string uri = aUri.OriginalString;

                Item item = null;
                if (!iCacheData.TryGetItem(uri, out item) || item.Failed)
                {
                    // artwork not in cache or a failed download is in the cache - make a new request
                    //iPendingRequests.Add(uri);
                }

                return item;
            }
        }
        
        public event EventHandler<EventArgsArtwork> EventImageAdded;

        private void ProcessRequests()
        {
            while (true)
            {
                // wait for an artwork request
                iPendingRequests.Wait();

                // get the first request in the queue - leave the request in the queue for now so that if
                // the main thread requests it again, it will not get re-added and downloaded again
                string uri;
				ScalingUriConverter uriConverter;
                lock (iLock)
                {
                    uri = iPendingRequests.FirstRequest;
					uriConverter = iUriConverter;
                }

                // download the image
                NSAutoreleasePool pool = new NSAutoreleasePool();
                pool.Init();

                UIImage image = null;
                try
                {
					string requestUri = uri;
					if (uriConverter != null)
					{
						requestUri = uriConverter.Convert(uri);	
					}
                    NSData data = NSData.FromUrl(NSUrl.FromString(requestUri));
                    image = ImageToFitSize(new UIImage(data), new CGSize(kDownscaleSize, kDownscaleSize));
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("ArtworkCache.ProcessRequests: " + uri + " (" + e.Message + ")");
                }

                pool.Dispose();

                // insert the image into the cache
                List<Item> staleItems;
                lock (iLock)
                {
                    // add to the cache
                    Item item = new Item(uri, image);
                    staleItems = iCacheData.Add(item);

                    // remove the request
                    iPendingRequests.Remove(uri);
                }

                // send notification that the image was added to the cache
                if (EventImageAdded != null)
                {
                    EventImageAdded(this, new EventArgsArtwork(uri));
                }

                // clean up all stale items outside of the lock
                foreach (Item item in staleItems)
                {
                    if (item.Image != null)
                    {
                        item.Image.Dispose();
                    }
                }
            }
        }

        private UIImage ImageToFitSize(UIImage aImage, CGSize aFitSize)
        {
            if(aImage.Size.Width > aFitSize.Width || aImage.Size.Height > aFitSize.Height)
            {
                double imageScaleFactor = 1.0;
                imageScaleFactor = aImage.CurrentScale;

                double sourceWidth = aImage.Size.Width * imageScaleFactor;
                double sourceHeight = aImage.Size.Height * imageScaleFactor;
                double targetWidth = aFitSize.Width;
                double targetHeight = aFitSize.Height;

                double sourceRatio = sourceWidth / sourceHeight;
                double targetRatio = targetWidth / targetHeight;

                bool scaleWidth = (sourceRatio <= targetRatio);
                scaleWidth = !scaleWidth;

                double scalingFactor, scaledWidth, scaledHeight;

                if (scaleWidth)
                {
                    scalingFactor = 1.0 / sourceRatio;
                    scaledWidth = targetWidth;
                    scaledHeight = Math.Round(targetWidth * scalingFactor);
                }
                else
                {
                    scalingFactor = sourceRatio;
                    scaledWidth = Math.Round(targetHeight * scalingFactor);
                    scaledHeight = targetHeight;
                }

                CGRect destRect = new CGRect(0, 0, (float)scaledWidth, (float)scaledHeight);
    
                UIGraphics.BeginImageContextWithOptions(destRect.Size, false, 0.0f);
                aImage.Draw(destRect);
                UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                return newImage;
            }

            return aImage;
        }

        internal void Flush()
        {
            lock(iLock)
            {
                iCacheData.Flush();
            }
        }

        internal void AddImage(Uri aUri, UIImage aImage)
        {
            string uri = aUri.OriginalString;

            // insert the image into the cache
            List<Item> staleItems;
            lock (iLock)
            {
                // add to the cache
                Item item = new Item(uri, aImage);
                staleItems = iCacheData.Add(item);

                // remove the request
                //iPendingRequests.Remove(uri);
            }

            // send notification that the image was added to the cache
            /*if (EventImageAdded != null)
            {
                EventImageAdded(this, new EventArgsArtwork(uri));
            }*/

            // clean up all stale items outside of the lock
            foreach (Item item in staleItems)
            {
                if (item.Image != null)
                {
                    item.Image.Dispose();
                }
            }
        }


        // class to encapsulate the data for the request list
        private class RequestList
        {
            public void Wait()
            {
                iEvent.WaitOne();
            }

            public string FirstRequest
            {
                get
                {
                    // gets the first request but does not remove it from the queue
                    Assert.Check(iRequests.Count > 0);
                    return iRequests[0];
                }
            }

            public void Add(string aUri)
            {
                // make sure uri does not appear in the request list twice
                iRequests.Remove(aUri);
                iRequests.Insert(0, aUri);
                iEvent.Set();
            }

            public void Remove(string aUri)
            {
                iRequests.Remove(aUri);
                if (iRequests.Count == 0)
                {
                    iEvent.Reset();
                }
            }

            private List<string> iRequests = new List<string>();
            private ManualResetEvent iEvent = new ManualResetEvent(false);
        }

        // class to encapsulate the cache data
        private class CacheData
        {
            public CacheData(uint aMaxByteCount)
            {
                iMaxByteCount = aMaxByteCount;
            }

            public bool TryGetItem(string aUri, out Item aItem)
            {
                bool cached = iCache.TryGetValue(aUri, out aItem);
                if (cached)
                {
                    // update usage of this cache item
                    iUsage.Remove(aUri);
                    iUsage.Add(aUri);
                }
                return cached;
            }

            public void Flush()
            {
                foreach(string s in iUsage)
                {
                    Item itemToRemove = iCache[s];
                    iCache.Remove(s);
                    iByteCount -= itemToRemove.ByteCount;

                    if (itemToRemove.Image != null)
                    {
                        itemToRemove.Image.Dispose();
                    }
                }
                iUsage.Clear();

                Assert.Check(iCache.Count == 0);
                Assert.Check(iCache.Count == iUsage.Count);
                Assert.Check(iByteCount == 0);

                Trace.WriteLine(Trace.kKinskyDesktop, "ArtworkCache flushed");
                UserLog.WriteLine("ArtworkCache flushed");
            }

            public List<Item> Add(Item aItem)
            {
                // if item already exists in the cache, decrement the total byte count
                Item oldItem = null;
                if (iCache.TryGetValue(aItem.Uri, out oldItem))
                {
                    iByteCount -= oldItem.ByteCount;
                }

                // add the new cache item
                iByteCount += aItem.ByteCount;
                iCache[aItem.Uri] = aItem;

                Trace.WriteLine(Trace.kKinskyDesktop, "ArtworkCache added (count=" + iCache.Count + " bytes=" + iByteCount + "): " + aItem.Uri);

                // update usage of this cache item
                iUsage.Remove(aItem.Uri);
                iUsage.Add(aItem.Uri);

                // remove stale cache items
                List<Item> toRemove = new List<Item>();

                while (iByteCount > iMaxByteCount)
                {
                    string uriToRemove = iUsage[0];
                    iUsage.RemoveAt(0);

                    Item itemToRemove = iCache[uriToRemove];
                    iCache.Remove(uriToRemove);
                    iByteCount -= itemToRemove.ByteCount;

                    toRemove.Add(itemToRemove);

                    Trace.WriteLine(Trace.kKinskyDesktop, "ArtworkCache removed (count=" + iCache.Count + " bytes=" + iByteCount + "): " + uriToRemove);
                }

                Assert.Check(iCache.Count == iUsage.Count);
                Assert.Check(iByteCount <= iMaxByteCount);

                return toRemove;
            }

            private Dictionary<string, Item> iCache = new Dictionary<string, Item>();
            private List<string> iUsage = new List<string>();
            private uint iMaxByteCount;
            private uint iByteCount = 0;
        }

        private const uint kMaxCacheSizeSmall = 30 * 1024 * 1024;
        private const uint kMaxCacheSizeMedium = 100 * 1024 * 1024;
        private const uint kMaxCacheSizeLarge = 300 * 1024 * 1024;
        
        private object iLock;
        private RequestList iPendingRequests;
        private CacheData iCacheData;
        private Thread iThread;
		private int iDownscaleSize;
		private ScalingUriConverter iUriConverter;
		private const float kDownscaleSize = 107.0f;
    }
}