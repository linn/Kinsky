using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace KinskyTouch
{
    public partial class CellRoomFactory : NSObject
    {
        public CellRoomFactory()
        {
        }

        public CellRoomFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellRoom Cell
        {
            get
            {
                return cellRoom;
            }
        }
    }

    public partial class CellRoom : UITableViewCell
    {
        public CellRoom(IntPtr aInstance)
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
		
		public bool Standby
		{
			set
			{
				buttonStandby.Selected = !value;	
			}
		}
		
		public int Position
		{
			set
			{
				buttonStandby.Tag = value;
			}
		}
		
		public void SetStandbyButtonOffsetX(int aOffsetX)
		{
			CGRect currentBounds = buttonStandby.Frame;
			CGRect newBounds = new CGRect(currentBounds.X + aOffsetX, currentBounds.Y, currentBounds.Width, currentBounds.Height);			
			buttonStandby.Frame = newBounds;
		}
			
    }
}