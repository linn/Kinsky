using System;

using UIKit;
using Foundation;

using Linn.Kinsky;

namespace KinskyTouch
{
    public partial class CellSenderController : NSObject
    {
        public CellSenderController(IControllerRoomSelector aController)
        {
            iController = aController;
        }

        public void Initialise()
        {
            cellSender.ButtonRoom.TouchUpInside += TouchUpInside;
        }

        public CellSender Cell
        {
            get
            {
                return cellSender;
            }
        }

        public void SetRoom(Room aRoom)
        {
            iRoom = aRoom;
            SetButtonState();
        }

        public void SetPlaying(bool aPlaying)
        {
            iPlaying = aPlaying;
            SetButtonState();
        }

        private void TouchUpInside(object sender, EventArgs e)
        {
            iController.Select(iRoom);
        }

        private void SetButtonState()
        {
            cellSender.ButtonRoom.Hidden = !iPlaying || (iRoom == null);
        }

        private IControllerRoomSelector iController;
        private bool iPlaying;
        private Room iRoom;
    }

    partial class CellSender : CellLazyLoadImage
    {
        public CellSender(IntPtr aInstance)
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

        public void SetPlaying(bool aPlaying, bool aAnimated)
        {
            if(aAnimated)
            {
                if(aPlaying)
                {
                    UIView.Transition(imageViewArtwork, imageViewPlaying, 0.5f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionFlipFromRight, () => {});
                }
                else
                {
                    UIView.Transition(imageViewPlaying, imageViewArtwork, 0.5f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionFlipFromLeft, () => {});
                }
            }
            else
            {
                if(aPlaying)
                {
                    UIView.Transition(imageViewArtwork, imageViewPlaying, 0.0f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionNone, () => {});
                }
                else
                {
                    UIView.Transition(imageViewPlaying, imageViewArtwork, 0.0f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionNone, () => {});
                }
            }

            controller.SetPlaying(aPlaying);
        }

        public string Title
        {
            set
            {
                labelTitle.Text = value;
            }
        }

        public UIButton ButtonRoom
        {
            get
            {
                return buttonRoom;
            }
        }

        public void SetRoom(Room aRoom)
        {
            controller.SetRoom(aRoom);
        }
    }
}