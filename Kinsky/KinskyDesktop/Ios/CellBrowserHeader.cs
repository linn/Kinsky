using System;

using Foundation;
using UIKit;

namespace KinskyTouch
{
    public partial class CellBrowserHeaderFactory : NSObject
    {
        public CellBrowserHeaderFactory()
        {
        }

        public CellBrowserHeaderFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellBrowserHeader Cell
        {
            get
            {
                return cellBrowserHeader;
            }
        }
    }

    public partial class CellBrowserHeader : CellLazyLoadImage
    {
        public CellBrowserHeader(IntPtr aInstance)
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

        public string Composer
        {
            set
            {
                labelComposer.Text = value;
            }
        }
    }
}