using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace KinskyTouch
{
    public class TimeControllerIpad
    {
        private class Delegate : UIPopoverControllerDelegate
        {
            public Delegate(IViewWidgetTimePopover aViewWidgetTimePopover)
            {
                iViewWidgetTimePopover = aViewWidgetTimePopover;
            }

            public override void DidDismiss(UIPopoverController aPopoverController)
            {
                iViewWidgetTimePopover.Dismiss();
            }

            private IViewWidgetTimePopover iViewWidgetTimePopover;
        }

        public TimeControllerIpad(ViewWidgetTimeButtons aViewWidgetTimeButtons, ViewWidgetTimeRotary aViewWidgetTimeRotary, UIControl aControl)
        {
            iControl = aControl;
            iControl.TouchDown += TouchDown;

            iViewWidgetTimeButtons = aViewWidgetTimeButtons;
            iViewWidgetTimeRotary = aViewWidgetTimeRotary;

            iPopoverController = new UIPopoverController(aViewWidgetTimeButtons);
        }

        public void DidRotate(UIInterfaceOrientation aFromInterfaceOrientation)
        {
            iViewWidgetTimeButtons.DidRotate(aFromInterfaceOrientation);
            iViewWidgetTimeRotary.DidRotate(aFromInterfaceOrientation);

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
                iPopoverController = new UIPopoverController(iViewWidgetTimeButtons);
				iPopoverController.BackgroundColor = iViewWidgetTimeButtons.View.BackgroundColor;
                iPopoverController.Delegate = new Delegate(iViewWidgetTimeButtons);
            }
            else
            {
                iPopoverController = new UIPopoverController(iViewWidgetTimeRotary);
				iPopoverController.BackgroundColor = iViewWidgetTimeRotary.View.BackgroundColor;
                iPopoverController.Delegate = new Delegate(iViewWidgetTimeRotary);
            }
        }

        private void TouchDown(object sender, EventArgs e)
        {
            PresentPopover();
        }

        private void PresentPopover()
        {
            if(iPopoverController.ContentViewController == iViewWidgetTimeRotary)
            {
                iPopoverController.SetPopoverContentSize(new System.Drawing.SizeF(196, 196), false);
            }
            else
            {
                iPopoverController.SetPopoverContentSize(new System.Drawing.SizeF(296, 77), false);
            }

            iPopoverController.PresentFromRect(iControl.Frame, iControl.Superview, UIPopoverArrowDirection.Up, true);
        }

        private ViewWidgetTimeButtons iViewWidgetTimeButtons;
        private ViewWidgetTimeRotary iViewWidgetTimeRotary;

        private UIControl iControl;
        private UIPopoverController iPopoverController;
    }

    public class TimeControllerIphone
    {
        public TimeControllerIphone(UIViewController aViewController, ViewWidgetTimeRotary aViewWidgetTimeRotary, ViewWidgetTimeButtons aViewWidgetTimeButtons, UIControl aTimeControl, UIControl aVolumeControl,UIScrollView aScrollView)
        {
            iViewController = aViewController;

            iTimeControl = aTimeControl;
            iTimeControl.TouchDown += TimeTouchDown;

            iVolumeControl = aVolumeControl;
            iVolumeControl.TouchDown += VolumeTouchDown;

            iViewWidgetTimeRotary = aViewWidgetTimeRotary;
            iViewWidgetTimeButtons = aViewWidgetTimeButtons;
            iScrollView = aScrollView;
        }

        private void TimeTouchDown(object sender, EventArgs e)
        {
            if(iRockers)
            {
                if(iViewWidgetTimeRotary.View.Superview == null)
                {
                    UIView view = new UIView(new CGRect(0.0f, 0.0f, 320.0f, iViewController.View.Bounds.Bottom - 76.0f));
                    view.BackgroundColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.6f);

					iViewWidgetTimeRotary.View.Frame = new CGRect((nfloat)((view.Bounds.Width - 196.0f) * 0.5f), (nfloat)((view.Bounds.Height - 196.0f) * 0.5), (nfloat)(196.0f), (nfloat)(196.0f));
					iViewWidgetTimeRotary.View.BackgroundColor = UIColor.Clear;
                    view.AddSubview(iViewWidgetTimeRotary.View);

                    iViewController.View.AddSubview(view);

                    iScrollView.ScrollEnabled = false;
                }
                else
                {
                    iViewWidgetTimeRotary.View.Superview.RemoveFromSuperview();
                    iViewWidgetTimeRotary.View.RemoveFromSuperview();

                    iScrollView.ScrollEnabled = true;
                }
            }
            else
            {
                if(iViewWidgetTimeButtons.View.Superview == null)
                {
                    UIView view1 = new UIView(new CGRect(0.0f, 0.0f, 320.0f, iViewController.View.Bounds.Bottom - 76.0f));
                    view1.BackgroundColor = UIColor.Clear;

                    UIView view2 = new UIView(new CGRect(0.0f, view1.Bounds.Bottom - 77.0f, 320.0f, 77.0f));
                    view2.BackgroundColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.6f);
                    view1.AddSubview(view2);

                    iViewWidgetTimeButtons.View.Frame = new System.Drawing.RectangleF(12.0f, 0.0f, 320.0f, 77.0f);
                    iViewWidgetTimeButtons.View.BackgroundColor = UIColor.Clear;
                    view2.AddSubview(iViewWidgetTimeButtons.View);

                    iViewController.View.AddSubview(view1);

                    iScrollView.ScrollEnabled = false;
                }
                else
                {
                    iViewWidgetTimeButtons.View.Superview.Superview.RemoveFromSuperview();
                    iViewWidgetTimeButtons.View.RemoveFromSuperview();

                    iScrollView.ScrollEnabled = true;
                }
            }
        }

        private void VolumeTouchDown(object sender, EventArgs e)
        {
            if(iViewWidgetTimeRotary.View.Superview != null)
            {
                iViewWidgetTimeRotary.View.Superview.RemoveFromSuperview();
                iViewWidgetTimeRotary.View.RemoveFromSuperview();
            }

            if(iViewWidgetTimeButtons.View.Superview != null)
            {
                iViewWidgetTimeButtons.View.Superview.Superview.RemoveFromSuperview();
                iViewWidgetTimeButtons.View.RemoveFromSuperview();
            }
        }

        public void SetRockers(bool aRockers)
        {
            iRockers = !aRockers;
        }

        private bool iRockers;

        private UIViewController iViewController;
        private ViewWidgetTimeRotary iViewWidgetTimeRotary;
        private ViewWidgetTimeButtons iViewWidgetTimeButtons;

        private UIControl iTimeControl;
        private UIControl iVolumeControl;
        private UIScrollView iScrollView;
    }
}

