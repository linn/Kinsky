using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace KinskyTouch
{
    public class VolumeControllerIpad
    {
        private class Delegate : UIPopoverControllerDelegate
        {
            public Delegate(IViewWidgetVolumePopover aViewWidgetVolumePopover)
            {
                iViewWidgetVolumePopover = aViewWidgetVolumePopover;
            }

            public override void DidDismiss(UIPopoverController aPopoverController)
            {
                iViewWidgetVolumePopover.Dismiss();
            }

            private IViewWidgetVolumePopover iViewWidgetVolumePopover;
        }

        public VolumeControllerIpad(ViewWidgetVolumeButtons aViewWidgetVolumeButtons, ViewWidgetVolumeRotary aViewWidgetVolumeRotary, UIControl aControl)
        {
            iControl = aControl;
            iControl.TouchDown += TouchDown;

            iViewWidgetVolumeButtons = aViewWidgetVolumeButtons;
            iViewWidgetVolumeRotary = aViewWidgetVolumeRotary;
        }

        public void DidRotate(UIInterfaceOrientation aFromInterfaceOrientation)
        {
            iViewWidgetVolumeButtons.DidRotate(aFromInterfaceOrientation);
            iViewWidgetVolumeRotary.DidRotate(aFromInterfaceOrientation);

            if(iPopoverController.PopoverVisible)
            {
                iPopoverController.Dismiss(true);
                PresentPopover();
            }
        }

        public void SetRockers(bool aRockers)
        {
            if(iPopoverController != null)
            {
                iPopoverController.Dispose();
                iPopoverController = null;
            }

            if(aRockers)
            {
                iPopoverController = new UIPopoverController(iViewWidgetVolumeButtons);
				iPopoverController.BackgroundColor = iViewWidgetVolumeButtons.View.BackgroundColor;
                iPopoverController.Delegate = new Delegate(iViewWidgetVolumeButtons);
            }
            else
            {
                iPopoverController = new UIPopoverController(iViewWidgetVolumeRotary);
				iPopoverController.BackgroundColor = iViewWidgetVolumeRotary.View.BackgroundColor;
                iPopoverController.Delegate = new Delegate(iViewWidgetVolumeRotary);
            }
        }

        private void PresentPopover()
        {
            if(iPopoverController.ContentViewController == iViewWidgetVolumeRotary)
            {
                iPopoverController.SetPopoverContentSize(new System.Drawing.SizeF(196, 196), false);
            }
            else
            {
                iPopoverController.SetPopoverContentSize(new System.Drawing.SizeF(296, 77), false);
            }

            iPopoverController.PresentFromRect(iControl.Frame, iControl.Superview, UIPopoverArrowDirection.Up, true);
        }

        private void TouchDown(object sender, EventArgs e)
        {
            PresentPopover();
        }

        private ViewWidgetVolumeButtons iViewWidgetVolumeButtons;
        private ViewWidgetVolumeRotary iViewWidgetVolumeRotary;

        private UIControl iControl;
        private UIPopoverController iPopoverController;
    }

    public class VolumeControllerIphone
    {
        public VolumeControllerIphone(UIViewController aViewController, ViewWidgetVolumeRotary aViewWidgetVolumeRotary, ViewWidgetVolumeButtons aViewWidgetVolumeButtons, UIControl aVolumeControl, UIControl aTimeControl, UIScrollView aScrollView)
        {
            iViewController = aViewController;

            iVolumeControl = aVolumeControl;
            iVolumeControl.TouchDown += VolumeTouchDown;

            iTimeControl = aTimeControl;
            iTimeControl.TouchDown += TimeTouchDown;

            iViewWidgetVolumeRotary = aViewWidgetVolumeRotary;
            iViewWidgetVolumeButtons = aViewWidgetVolumeButtons;
            iScrollView = aScrollView;
        }

        private void VolumeTouchDown(object sender, EventArgs e)
        {
            if(iRockers)
            {
                if(iViewWidgetVolumeRotary.View.Superview == null)
                {
                    UIView view = new UIView(new CGRect(0.0f, 0.0f, 320.0f, iViewController.View.Bounds.Bottom - 76.0f));
                    view.BackgroundColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.6f);

					iViewWidgetVolumeRotary.View.Frame = new CGRect((nfloat)((view.Bounds.Width - 196.0f) * 0.5f), (nfloat)((view.Bounds.Height - 196.0f) * 0.5f), 196.0f, 196.0f);
                    iViewWidgetVolumeRotary.View.BackgroundColor = UIColor.Clear;
                    view.AddSubview(iViewWidgetVolumeRotary.View);

                    iViewController.View.AddSubview(view);

                    iScrollView.ScrollEnabled = false;
                }
                else
                {
                    iViewWidgetVolumeRotary.View.Superview.RemoveFromSuperview();
                    iViewWidgetVolumeRotary.View.RemoveFromSuperview();

                    iScrollView.ScrollEnabled = true;
                }
            }
            else
            {
                if(iViewWidgetVolumeButtons.View.Superview == null)
                {
                    UIView view1 = new UIView(new CGRect(0.0f, 0.0f, 320.0f, iViewController.View.Bounds.Bottom - 76.0f));
                    view1.BackgroundColor = UIColor.Clear;

					UIView view2 = new UIView(new CGRect(0.0f, (nfloat)(view1.Bounds.Bottom - 77.0f), 320.0f, 77.0f));
                    view2.BackgroundColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.6f);
                    view1.AddSubview(view2);

					iViewWidgetVolumeButtons.View.Frame = new CGRect(12.0f, 0.0f, 320.0f, 77.0f);
                    iViewWidgetVolumeButtons.View.BackgroundColor = UIColor.Clear;
                    view2.AddSubview(iViewWidgetVolumeButtons.View);

                    iViewController.View.AddSubview(view1);

                    iScrollView.ScrollEnabled = false;
                }
                else
                {
                    iViewWidgetVolumeButtons.View.Superview.Superview.RemoveFromSuperview();
                    iViewWidgetVolumeButtons.View.RemoveFromSuperview();

                    iScrollView.ScrollEnabled = true;
                }
            }
        }

        private void TimeTouchDown(object sender, EventArgs e)
        {
            if(iViewWidgetVolumeRotary.View.Superview != null)
            {
                iViewWidgetVolumeRotary.View.Superview.RemoveFromSuperview();
                iViewWidgetVolumeRotary.View.RemoveFromSuperview();
            }

            if(iViewWidgetVolumeButtons.View.Superview != null)
            {
                iViewWidgetVolumeButtons.View.Superview.Superview.RemoveFromSuperview();
                iViewWidgetVolumeButtons.View.RemoveFromSuperview();
            }
        }

        public void SetRockers(bool aRockers)
        {
            iRockers = !aRockers;
        }

        private bool iRockers;

        private UIViewController iViewController;
        private ViewWidgetVolumeRotary iViewWidgetVolumeRotary;
        private ViewWidgetVolumeButtons iViewWidgetVolumeButtons;

        private UIControl iVolumeControl;
        private UIControl iTimeControl;
        private UIScrollView iScrollView;
    }
}

