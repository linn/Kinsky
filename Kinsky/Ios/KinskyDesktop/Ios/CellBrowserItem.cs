using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace KinskyTouch
{
    public partial class CellBrowserItemFactory : NSObject
    {
        public CellBrowserItemFactory()
        {
        }

        public CellBrowserItemFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellBrowserItem Cell
        {
            get
            {
                return cellBrowserItem;
            }
        }
    }

    public partial class CellBrowserItem : UITableViewCell
    {
        public CellBrowserItem(IntPtr aInstance)
            : base(aInstance)
        {
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

        public string TrackNumber
        {
            set
            {
                labelTrackNumber.Text = value;
            }
        }
    }
}