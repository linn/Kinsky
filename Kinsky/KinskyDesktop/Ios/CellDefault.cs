using System;

using Foundation;
using UIKit;

namespace KinskyTouch
{
    public partial class CellDefaultFactory : NSObject
    {
        public CellDefaultFactory()
        {
        }

        public CellDefaultFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellDefault Cell
        {
            get
            {
                return cellDefault;
            }
        }
    }

    public partial class CellDefault : UITableViewCell
    {
        public CellDefault(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public UIImage Image
        {
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