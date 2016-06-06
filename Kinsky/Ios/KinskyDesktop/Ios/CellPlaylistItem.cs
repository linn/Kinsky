using System;

using UIKit;
using Foundation;

namespace KinskyTouch
{
    public partial class CellPlaylistItemFactory : NSObject
    {
        public CellPlaylistItemFactory()
        {
        }

        public CellPlaylistItemFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellPlaylistItem Cell
        {
            get
            {
                return cellPlaylistItem;
            }
        }
    }

    public partial class CellPlaylistItem : CellPlaylist
    {
        public CellPlaylistItem(IntPtr aInstance)
            : base(aInstance)
        {
        }
    }
}

