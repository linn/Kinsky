using System;

using Foundation;
using UIKit;

namespace KinskyTouch
{
    public partial class CellLazyLoadDefaultFactory : NSObject
    {
        public CellLazyLoadDefaultFactory()
        {
        }

        public CellLazyLoadDefaultFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellLazyLoadDefault Cell
        {
            get
            {
                return cellLazyLoadDefault;
            }
        }
    }

    public partial class CellLazyLoadDefault : CellLazyLoadImage
    {
        public CellLazyLoadDefault(IntPtr aInstance)
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
    }
}
