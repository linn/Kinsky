using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Linn.Toolkit.Ios;
using CoreGraphics;

namespace KinskyTouch
{
    public partial class UIViewControllerKinskyTouchIpad : UIViewController
    {
        private class TapGestureDelegate : UIGestureRecognizerDelegate
        {
            public override bool ShouldReceiveTouch(UIGestureRecognizer aRecogniser, UITouch aTouch)
            {
                return true;
            }
        }

		public UIViewControllerKinskyTouchIpad(IntPtr aInstance)
			: base(aInstance)
		{
		}

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            UITapGestureRecognizer gesture = new UITapGestureRecognizer(TapGesture);
            gesture.NumberOfTapsRequired = 1;
            gesture.Delegate = new TapGestureDelegate();

            viewArtwork.AddGestureRecognizer(gesture);

            iArtworkRectangle = viewArtwork.Frame;

            iOsVersionLessThan5 = !UIDevice.CurrentDevice.CheckSystemVersion(5,0);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (iOsVersionLessThan5)
            {
                navigationController.ViewWillAppear(animated);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (iOsVersionLessThan5)
            {
                navigationController.ViewDidAppear(animated);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (iOsVersionLessThan5)
            {
                navigationController.ViewWillDisappear(animated);
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (iOsVersionLessThan5)
            {
                navigationController.ViewDidDisappear(animated);
            }
        }

        [Obsolete]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }

        public override void DidRotate(UIInterfaceOrientation aFromInterfaceOrientation)
        {
            base.DidRotate(aFromInterfaceOrientation);
            
            iVolumeController.DidRotate(aFromInterfaceOrientation);
            iTimeController.DidRotate(aFromInterfaceOrientation);
        }

        public void SetVolumeController(VolumeControllerIpad aVolumeController)
        {
            iVolumeController = aVolumeController;
        }

        public void SetTimeController(TimeControllerIpad aTimeController)
        {
            iTimeController = aTimeController;
        }

        private void TapGesture(UITapGestureRecognizer aRecogniser)
        {
            UIView.BeginAnimations("zoom");
            if(viewArtwork.Superview == View)
            {
                viewInfo.AddSubview(viewArtwork);
                viewArtwork.Frame = iArtworkRectangle;
                viewArtwork.BackgroundColor = UIColor.Clear;
                viewArtwork.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin;
                viewOverlay.Hidden = true;
            }
            else
            {
                View.AddSubview(viewArtwork);
                viewArtwork.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
                viewArtwork.BackgroundColor = UIColor.Black;
                viewArtwork.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
                viewOverlay.Hidden = false;
            }
            UIView.CommitAnimations();
        }

        private bool iOsVersionLessThan5;
        private CGRect iArtworkRectangle;

        private VolumeControllerIpad iVolumeController;
        private TimeControllerIpad iTimeController;
    }

    public partial class UIViewControllerKinskyTouchIphone : UIViewController
    {
        private class Delegate : UIScrollViewDelegate
        {
            public Delegate(UIScrollView aScrollView, UIPageControl aPageControl)
            {
                iScrollView = aScrollView;
                iPageControl = aPageControl;

                iPageControl.ValueChanged += PageChanged;
            }

            public override void Scrolled(UIScrollView aScrollView)
            {
                if(!iPageControlUsed)
                {
                    nfloat pageWidth = aScrollView.Frame.Width;
                    int page = (int)(Math.Floor((aScrollView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1);
                    iPageControl.CurrentPage = page;
                }
            }

            public override void DraggingStarted(UIScrollView aScrollView)
            {
                iPageControlUsed = false;
            }

            public override void DecelerationEnded(UIScrollView aScrollView)
            {
                iPageControlUsed = false;
            }

            private void PageChanged(object sender, EventArgs e)
            {
                nint page = iPageControl.CurrentPage;

                CGRect rect = iScrollView.Frame;
                rect.Offset(rect.Width * page, 0);
                iScrollView.ScrollRectToVisible(rect, true);

                iPageControlUsed = true;
            }

            private UIScrollView iScrollView;
            private UIPageControl iPageControl;
            private bool iPageControlUsed;
        }

        public UIViewControllerKinskyTouchIphone(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (iOsVersionLessThan5)
            {
                navigationControllerBrowser.ViewWillAppear(animated);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (iOsVersionLessThan5)
            {
                navigationControllerBrowser.ViewDidAppear(animated);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (iOsVersionLessThan5)
            {
                navigationControllerBrowser.ViewWillDisappear(animated);
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (iOsVersionLessThan5)
            {
                navigationControllerBrowser.ViewDidDisappear(animated);
            }
        }

        [Obsolete]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if(toInterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
        {
            return UIInterfaceOrientationMask.Portrait;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            scrollView.ContentSize = new CGSize(scrollView.Frame.Width * 3, scrollView.Frame.Height);
            scrollView.Delegate = new Delegate(scrollView, pageControl);

            viewControllerRoomSource.View.Frame = new CGRect(scrollView.Frame.Width * 0, 0, scrollView.Frame.Width, scrollView.Frame.Height);
            scrollView.AddSubview(viewControllerRoomSource.View);

            viewControllerNowPlaying.View.Frame = new CGRect(scrollView.Frame.Width * 1, 0, scrollView.Frame.Width, scrollView.Frame.Height);
            scrollView.AddSubview(viewControllerNowPlaying.View);

            viewControllerBrowser.View.Frame = new CGRect(scrollView.Frame.Width * 2, 0, scrollView.Frame.Width, scrollView.Frame.Height);
            scrollView.AddSubview(viewControllerBrowser.View);

            pageControl.CurrentPage = 1;
            CGRect rect = scrollView.Frame;
            rect.Offset(rect.Width * 1, 0);
            scrollView.ScrollRectToVisible(rect, false);

            iOsVersionLessThan5 = !UIDevice.CurrentDevice.CheckSystemVersion(5,0);
        }

        private bool iOsVersionLessThan5;
    }

    public partial class UIViewControllerNowPlaying : UIViewController
    {
        public UIViewControllerNowPlaying(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            viewPlaylist.Frame = imageViewArtwork.Frame;

            iNowPlaying = true;

            buttonList.TouchDown += ListClicked;
            buttonArtwork.TouchDown += ListClicked;

            buttonArtwork.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            iView = new UIView(new CGRect(0.0f, 0.0f, 45.0f, 29.0f));
            iView.AddSubview(buttonArtwork);
            iView.AddSubview(buttonList);

            UIBarButtonItem b = new UIBarButtonItem(iView);
            navigationBar.TopItem.RightBarButtonItem = b;
            iView.Frame = new CGRect(0.0f, 0.0f, 45.0f, 29.0f);
        }

        private void ListClicked(object sender, EventArgs e)
        {
            if(iNowPlaying)
            {
                UIView.Transition(imageViewArtwork, viewPlaylist, 1.0f, UIViewAnimationOptions.TransitionFlipFromLeft, () => {});
                UIView.Transition(buttonList, buttonArtwork, 1.0f, UIViewAnimationOptions.TransitionFlipFromLeft, () => {});
                iNowPlaying = false;
            }
            else
            {
                UIView.Transition(viewPlaylist, imageViewArtwork, 1.0f, UIViewAnimationOptions.TransitionFlipFromRight, () => {});
                UIView.Transition(buttonArtwork, buttonList, 1.0f, UIViewAnimationOptions.TransitionFlipFromRight, () => {});
                iNowPlaying = true;
            }
        }

        private bool iNowPlaying;

        private UIView iView;
    }
}
