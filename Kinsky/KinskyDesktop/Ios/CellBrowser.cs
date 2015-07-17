using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace KinskyTouch
{
    public partial class CellBrowserFactory : NSObject
    {
        public CellBrowserFactory()
        {
        }

        public CellBrowserFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellBrowser Cell
        {
            get
            {
                return cellBrowser;
            }
        }
    }

    public partial class CellBrowser : CellLazyLoadImage
    {
        public CellBrowser(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override UIImage Image
        {
            get
            {
                return imageViewArtwork.Image;
            }
            
            set
            {
                imageViewArtwork.Image = value;
            }
        }

        public string Title
        {
            set
            {
                labelTitle.Text = value;
            }
        }

        public string ArtistAlbum
        {
            set
            {
                labelArtistAlbum.Text = value;
            }
        }

        public string DurationBitrate
        {
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    nfloat width = ContentView.Frame.Right - labelTitle.Frame.X;
                    labelTitle.Frame = new CGRect(labelTitle.Frame.X, labelTitle.Frame.Y, width, labelTitle.Frame.Height);
                }
                else
                {
                    nfloat width = labelDurationBitrate.Frame.X - labelTitle.Frame.X;
                    labelTitle.Frame = new CGRect(labelTitle.Frame.X, labelTitle.Frame.Y, width, labelTitle.Frame.Height);
                }
                labelDurationBitrate.Text = value;
            }
        }
    }
}