using System;
using System.Threading;
using System.Collections.Generic;

using Linn;

using Monobjc.Cocoa;
using System.Text.RegularExpressions;

namespace KinskyDesktop
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
        public class Item
        {
            public Item(string aUri, NSImage aImage)
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
                        // use the first image representation to get byte count
                        if (Image.Representations.Count > 0)
                        {
                            NSImageRep rep = Image.Representations[0].CastAs<NSImageRep>();

                            return (uint)(rep.PixelsWide * rep.PixelsHigh * rep.BitsPerSample) / 8;
                        }
                    }

                    return 0;
                }
            }

            public readonly bool Failed;
            public readonly NSImage Image;
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
            iCacheData = new CacheData(kMaxCacheSizeLarge);

			iThread = new Thread(ProcessRequests);
			iThread.IsBackground = true;
			iThread.Name = "ArtworkCache";
			iThread.Start();
		}

        public int DownscaleSize
        {
            set
            {
                lock(iLock)
                {
                    // prevent artwork resizing below minimum size of playlist items
                    int adjustedValue = Math.Max(value, kMinDownscaleSize);
                    if (iDownscaleSize != adjustedValue)
                    {
                        iDownscaleSize = adjustedValue;
                        iUriConverter = new ScalingUriConverter(adjustedValue, false, false);
                        iCacheData.Clear();
                    }
                }
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
                    iPendingRequests.Add(uri);
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

                Trace.WriteLine(Trace.kKinskyDesktop, "ArtworkCache requesting  " + uri);

                // download the image
                NSAutoreleasePool pool = new NSAutoreleasePool();
                pool.Init();

                NSImage image = null;
                try
                {
                    string requestUri = uri;
                    if (uriConverter != null)
                    {
                        requestUri = uriConverter.Convert(requestUri);
                    }
                    NSString s = NSString.StringWithUTF8String(Uri.UnescapeDataString(requestUri));
                    NSURL url = NSURL.URLWithString(s.StringByAddingPercentEscapesUsingEncoding(NSStringEncoding.NSUTF8StringEncoding));

                    image = new NSImage(url);
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("ArtworkCache.ProcessRequests: " + uri + " (" + e.Message + ")");
                }

                pool.Release();

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
                        item.Image.Release();
                    }
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

                Trace.WriteLine(Trace.kKinskyDesktop, "ArtworkCache added (bytes= " + aItem.ByteCount + " count=" + iCache.Count + " total bytes=" + iByteCount + "): " + aItem.Uri);

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

            public void Clear()
            {
                foreach (Item item in iCache.Values)
                {
                    if (item.Image != null)
                    {
                        item.Image.Release();
                    }
                }
                iCache.Clear();
                iUsage.Clear();
                iByteCount = 0;
            }

            private Dictionary<string, Item> iCache = new Dictionary<string, Item>();
            private List<string> iUsage = new List<string>();
            private uint iMaxByteCount;
            private uint iByteCount = 0;
        }



        private const uint kMaxCacheSizeSmall = 10 * 1024 * 1024;
        private const uint kMaxCacheSizeMedium = 100 * 1024 * 1024;
        private const uint kMaxCacheSizeLarge = 300 * 1024 * 1024;
		
        private object iLock;
        private RequestList iPendingRequests;
        private CacheData iCacheData;
		private Thread iThread;
        private ScalingUriConverter iUriConverter;
        private int iDownscaleSize;
        private const int kMinDownscaleSize = 90;
	}
}