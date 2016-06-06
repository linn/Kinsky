using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace KinskyTouch
{
    public abstract class CellLazyLoadImage : UITableViewCell
    {
        internal class Connection : NSUrlConnectionDataDelegate
        {
            public Connection(Uri aUri, CellLazyLoadImage aCell)
            {
                iUri = aUri;
                iCell = aCell;
                iData = new NSMutableData();
            }
    
            public override void ReceivedData(NSUrlConnection connection, NSData data)
            {
                iData.AppendData(data);
            }
    
            public override void FailedWithError(NSUrlConnection connection, NSError error)
            {
                if(iCell != null)
                {
                    iData = null;

                    ArtworkCacheInstance.Instance.AddImage(iUri, null);

                    if(iCell.iUri == iUri)
                    {
                        iCell.Image = KinskyTouch.Properties.ResourceManager.AlbumError;
                    }
                }
            }
    
            public override void FinishedLoading(NSUrlConnection connection)
            {
                if(iCell != null)
                {
                    try
                    {
                        UIImage temp = new UIImage(iData);
                        if(temp.CGImage.Width > 0 && temp.CGImage.Height > 0)
                        {
                            UIImage image = ImageToFitSize(temp, new CGSize(107.0f, 107.0f));
                            iData = null;
        
                            ArtworkCacheInstance.Instance.AddImage(iUri, image);
        
                            if(iCell.iUri == iUri)
                            {
                                iCell.Image = image;
                            }
                        }
                        else
                        {
                            iData = null;

                            ArtworkCacheInstance.Instance.AddImage(iUri, null);
        
                            if(iCell.iUri == iUri)
                            {
                                iCell.Image = KinskyTouch.Properties.ResourceManager.AlbumError;
                            }
                        }
                    }
                    catch
                    {
                        iCell.Image = KinskyTouch.Properties.ResourceManager.AlbumError;
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
    
            private Uri iUri;
            private CellLazyLoadImage iCell;
            private NSMutableData iData;
        }

        public CellLazyLoadImage(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public void SetArtworkUri(Uri aUri)
        {
            /*ArtworkCacheInstance.Instance.Cancel(aUri);
            ArtworkCache.Item item = ArtworkCacheInstance.Instance.Artwork(aUri);
            if(item != null)
            {
                if(item.Image != null)
                {
                    Image = item.Image;
                }
            }*/

            iUri = aUri;

            if(aUri != null)
            {
                ArtworkCache.Item item = ArtworkCacheInstance.Instance.Artwork(aUri);
                if(item != null)
                {
                    if(item.Image != null)
                    {
                        Image = item.Image;
                    }
                    else
                    {
                        Image = KinskyTouch.Properties.ResourceManager.AlbumError;
                    }
                }
                else
                {
                    if(iConnection != null)
                    {
                        iConnection.Cancel();
                    }
                    NSUrl url = new NSUrl(ArtworkCacheInstance.Instance.ScaledUri(aUri.AbsoluteUri));
                    NSUrlRequest request = new NSUrlRequest(url);
                    iConnection = new NSUrlConnection(request, new Connection(aUri, this), true);

                    Image = KinskyTouch.Properties.ResourceManager.Loading;
                }
            }
        }

        public abstract UIImage Image { set; get; }

        private Uri iUri;
        private NSUrlConnection iConnection;
    }
}

