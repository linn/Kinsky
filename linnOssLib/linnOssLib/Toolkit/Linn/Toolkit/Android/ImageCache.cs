using Linn;
using System.IO;
using Android.Graphics;
using Android.Widget;
using System;
using Android.Runtime;
using Android.Util;
using Android.Content;

namespace OssToolkitDroid
{

    public class AndroidImageCache : ThreadedImageCache<Bitmap>
    {
        public AndroidImageCache(int aMaxSize, int aDownscaleImageSize, int aThreadCount, IImageLoader<Bitmap> aImageLoader, IInvoker aInvoker, FlingStateManager aFlingStateManager, int aPendingRequestLimit)
            : base(aMaxSize, aDownscaleImageSize, aThreadCount, aImageLoader, aPendingRequestLimit)
        {
            Assert.Check(aInvoker != null);
            iInvoker = aInvoker;
            iFlingStateManager = aFlingStateManager;
            iFlingStateManager.EventFlingStateChanged += EventFlingStateChangedHandler;
        }

        private void EventFlingStateChangedHandler(object sender, EventArgs e)
        {
            IsRunning = !iFlingStateManager.IsFlinging();
        }
        public IInvoker Invoker { get { return iInvoker; } }
        private IInvoker iInvoker;
        private FlingStateManager iFlingStateManager;
    }

    public class AndroidImageLoader : AbstractStreamImageLoader<Bitmap>
    {
        public AndroidImageLoader(IImageUriConverter aUriConverter)
            : base(aUriConverter)
        {
        }

        protected override IImage<Bitmap> CreateImageFromStream(MemoryStream aStream, bool aDownscaleImage, int aDownscaleImageSize)
        {
            int scale = 1;
            if (aDownscaleImage)
            {
                BitmapFactory.Options decodeBoundsOptions = new BitmapFactory.Options();
                decodeBoundsOptions.InJustDecodeBounds = true;
                aStream.Position = 0;

                using (MemoryStream tempStream = new MemoryStream())
                {
                    byte[] buffer = new byte[8192];
                    int count = aStream.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        tempStream.Write(buffer, 0, count);
                        count = aStream.Read(buffer, 0, buffer.Length);
                    }
                    tempStream.Seek(0, SeekOrigin.Begin);
                    BitmapFactory.DecodeStream(tempStream, null, decodeBoundsOptions);
                }
                while (decodeBoundsOptions.OutWidth / scale >= aDownscaleImageSize && decodeBoundsOptions.OutHeight / scale >= aDownscaleImageSize)
                {
                    scale *= 2;
                }
                decodeBoundsOptions.Dispose();
                decodeBoundsOptions = null;
            }

            BitmapFactory.Options decodeOptions = new BitmapFactory.Options();
            decodeOptions.InSampleSize = scale;
            aStream.Position = 0;
            Bitmap result;
            using (MemoryStream tempStream = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                int count = aStream.Read(buffer, 0, buffer.Length);
                while (count > 0)
                {
                    tempStream.Write(buffer, 0, count);
                    count = aStream.Read(buffer, 0, buffer.Length);
                }
                tempStream.Seek(0, SeekOrigin.Begin);
                result = BitmapFactory.DecodeStream(tempStream, null, decodeOptions);
            }
            if (result == null)
            {
                throw new Exception("ImageLoader: DecodeByteArray returned null...");
            }
            decodeOptions.Dispose();

            return new AndroidImageWrapper(result);
        }
    
    }

    public class AndroidImageWrapper : IImage<Bitmap>
    {
        public AndroidImageWrapper(Bitmap aImage)
        {
            Assert.Check(aImage != null);
            iImage = aImage;
            iSizeBytes = aImage.RowBytes * aImage.Height;
        }

        #region IImage Members

        public IntPtr Handle
        {
            get
            {
                Assert.Check(iImage != null);
                return iImage.Handle;
            }
        }

        public Bitmap Image
        {
            get
            {
                Assert.Check(iImage != null);
                return iImage;
            }
        }

        public int SizeBytes
        {
            get
            {
                Assert.Check(iImage != null);
                return iSizeBytes;
            }
        }

        //public int ReferenceCount
        //{
        //    get
        //    {
        //        Assert.Check(iImage != null);
        //        lock (iImage)
        //        {
        //            return iReferenceCount;
        //        }
        //    }
        //}

        //public void IncrementReferenceCount()
        //{
        //    Assert.Check(iImage != null, "iImage != null");
        //    Assert.Check(!iImage.IsRecycled, "!iImage.IsRecycled");
        //    lock (iImage)
        //    {
        //        iReferenceCount++;
        //    }
        //}

        //public void DecrementReferenceCount()
        //{
        //        Assert.Check(iImage != null);
        //    lock (iImage)
        //    {
        //        iReferenceCount--;
        //        if (iReferenceCount == 0)
        //        {
        //            iImage.Recycle();
        //            iImage.Dispose();
        //            iImage = null;
        //        }
        //    }
        //}

        #endregion

        private int iSizeBytes;
        private Bitmap iImage;
        //private int iReferenceCount;
    }

    public class LazyLoadingImageView : ImageView
    {
        public LazyLoadingImageView(IntPtr aHandle, JniHandleOwnership aHandleOwnership)
            : base(aHandle, aHandleOwnership)
        {
            Init();
        }

        public LazyLoadingImageView(Context aContext, IAttributeSet aAttributeSet)
            : base(aContext, aAttributeSet)
        {
            Init();
        }

        public LazyLoadingImageView(Context aContext)
            : base(aContext)
        {
            Init();
        }

        private void Init()
        {
            iLockObject = new object();
            iDisposed = false;
        }

        public Bitmap ErrorImage
        {
            set
            {
                lock (iLockObject)
                {
                    iErrorImage = value;
                }
            }
        }

        public void LoadImage(AndroidImageCache aImageCache, Uri aUri)
        {
            lock (iLockObject)
            {
                RemoveCacheHandler();
                Assert.Check(aUri != null);
                Assert.Check(aImageCache != null);
                Assert.Check(!aImageCache.Invoker.InvokeRequired);
                AddCacheHandler(aImageCache);
                iUri = aUri.OriginalString;
                IImage<Bitmap> image = aImageCache.Image(iUri);
                if (image != null && !iDisposed)
                {
                    SetImage(image);
                }
            }
        }

        private void SetImage(IImage<Bitmap> aImage)
        {
            //aImage.IncrementReferenceCount();
            RemoveCacheHandler();
            if (aImage != iImageBitmap)
            {
                DereferenceOldImage();
            }
            // set current image bitmap to be null to prevent internal SetImageDrawable call from decrementing ref count
            iImageBitmap = null;
            try
            {
                Assert.Check(!aImage.Image.IsRecycled);
                base.SetImageBitmap(aImage.Image);
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception in SetImageBitmap: " + ex);
            }
            iImageBitmap = aImage;
        }

        public override void SetImageURI(Android.Net.Uri uri)
        {
            throw new NotImplementedException("Please use LoadImage() method");
        }

        public override void SetImageResource(int resId)
        {
            RemoveCacheHandler();
            DereferenceOldImage();
            base.SetImageResource(resId);
        }

        public override void SetImageDrawable(Android.Graphics.Drawables.Drawable drawable)
        {
            RemoveCacheHandler();
            DereferenceOldImage();
            base.SetImageDrawable(drawable);
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            RemoveCacheHandler();
            DereferenceOldImage();
            try
            {
                base.SetImageBitmap(bm);
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception in SetImageBitmap: " + ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (iLockObject)
            {
                iDisposed = true;
            }
            RemoveCacheHandler();
            DereferenceOldImage();
            base.Dispose(disposing);
        }

        private void AddCacheHandler(AndroidImageCache aImageCache)
        {
            lock (iLockObject)
            {
                Assert.Check(iImageCache == null);
                iImageCache = aImageCache;
                iImageCache.EventImageAdded += ImageAddedHandler;
                iImageCache.EventRequestCancelled += RequestCancelledHandler;
                iImageCache.EventRequestFailed += RequestFailedHandler;
            }
        }

        private void RemoveCacheHandler()
        {
            lock (iLockObject)
            {
                if (iImageCache != null)
                {
                    iImageCache.EventImageAdded -= ImageAddedHandler;
                    iImageCache.EventRequestCancelled -= RequestCancelledHandler;
                    iImageCache.EventRequestFailed -= RequestFailedHandler;
                    iImageCache = null;
                }
            }
        }

        private void DereferenceOldImage()
        {
            lock (iLockObject)
            {
                if (iImageBitmap != null)
                {
                    //iImageBitmap.DecrementReferenceCount();
                    iImageBitmap = null;
                }
            }
        }


        private void ImageAddedHandler(object sender, EventArgsImage<Bitmap> e)
        {
            lock (iLockObject)
            {
                if (iImageCache != null && e.Uri == iUri)
                {
                    //e.Image.IncrementReferenceCount();
                    iImageCache.Invoker.BeginInvoke((Action)(() =>
                    {
                        lock (iLockObject)
                        {
                            if (iImageCache != null && e.Uri == iUri && !iDisposed)
                            {
                                SetImage(e.Image);
                            }
                            //e.Image.DecrementReferenceCount();
                        }
                    }));
                }
            }
        }

        private void RequestCancelledHandler(object sender, EventArgsImageFailure e)
        {
            lock (iLockObject)
            {
                if (iImageCache != null && e.Uri == iUri)
                {
                    // request was cancelled, try again
                    IImage<Bitmap> image = iImageCache.Image(iUri);
                    if (image != null && !iDisposed)
                    {
                        SetImage(image);
                    }
                }
            }
        }

        private void RequestFailedHandler(object sender, EventArgsImageFailure e)
        {
            lock (iLockObject)
            {
                if (iImageCache != null && e.Uri == iUri && iErrorImage != null)
                {
                    iImageCache.Invoker.BeginInvoke((Action)(() =>
                    {
                        lock (iLockObject)
                        {
                            if (iImageCache != null && e.Uri == iUri && iErrorImage != null)
                            {
                                SetImageBitmap(iErrorImage);
                            }
                        }
                    }));
                }
            }
        }

        private string iUri;
        private AndroidImageCache iImageCache;
        private object iLockObject;
        private Bitmap iErrorImage;
        private IImage<Bitmap> iImageBitmap;
        private bool iDisposed;
    }
}