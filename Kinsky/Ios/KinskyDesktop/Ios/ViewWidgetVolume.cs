using System;
using System.Threading;

using Linn.Kinsky;

using UIKit;
using Foundation;

namespace KinskyTouch
{
    internal class ViewWidgetVolumeControl : IViewWidgetVolumeControl
    {
        public ViewWidgetVolumeControl(UIControlWheel aControl)
        {
            iControl = aControl;
        }

        public void Open()
        {
            iOpen = true;
        }

        public void Close()
        {
            iControl.ViewBar.Value = 0;
            iControl.ViewBar.Text = string.Empty;

            UIControl control = iControl as UIControl;
            control.Enabled = false;

            iOpen = false;
        }

        public void Initialised()
        {
            if(iOpen)
            {
                UIControl control = iControl as UIControl;
                control.Enabled = true;
            }
        }

        public void SetVolume(uint aVolume)
        {
            if(iOpen)
            {
                iControl.ViewBar.Value = (float)aVolume;
                iControl.ViewBar.Text = aVolume.ToString();
            }
        }

        public void SetMute(bool aMute)
        {
            iMute = aMute;
            iControl.Dimmed = aMute;
        }

        public void SetVolumeLimit(uint aVolumeLimit)
        {
            if(iOpen)
            {
                iControl.ViewBar.MaxValue = (float)aVolumeLimit;
            }
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;

        private void Clicked(object sender, EventArgs e)
        {
            if(iOpen && EventMuteChanged != null)
            {
                EventMuteChanged(this, new EventArgsMute(!iMute));
            }
        }

        private void RotateAntiClockwise(object sender, EventArgs e)
        {
            if(iOpen && EventVolumeDecrement != null)
            {
                EventVolumeDecrement(this, EventArgs.Empty);
            }
        }

        private void RotateClockwise(object sender, EventArgs e)
        {
            if(iOpen && EventVolumeIncrement != null)
            {
                EventVolumeIncrement(this, EventArgs.Empty);
            }
        }

        private bool iOpen;

        private bool iMute;

        private UIControlWheel iControl;
    }

    internal interface IViewWidgetVolumePopover
    {
        void Dismiss();
    }

    public partial class ViewWidgetVolumeButtons : UIViewController, IViewWidgetVolumeControl, IViewWidgetVolumePopover
    {
        public ViewWidgetVolumeButtons(string aNibName, NSBundle aBundle)
            : base(aNibName, aBundle)
        {
            iTimer = new System.Threading.Timer(TimerUpdate);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            buttonUp.TouchDown += UpTouchDown;
            buttonUp.TouchUpInside += TouchCancel;
            buttonUp.TouchUpOutside += TouchCancel;
            buttonUp.TouchDragOutside += TouchCancel;
            
            buttonDown.TouchDown += DownTouchDown;
            buttonDown.TouchUpInside += TouchCancel;
            buttonDown.TouchUpOutside += TouchCancel;
            buttonDown.TouchDragOutside += TouchCancel;
            
            buttonMute.TouchDown += MuteTouchDown;
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
            CancelAutoRepeat();
        }
        
        public void Open()
        {
            iOpen = true;
        }

        public void Close()
        {
            if(buttonUp != null)
            {
                buttonUp.Enabled = false;
                buttonDown.Enabled = false;
                buttonMute.Enabled = false;
            }

            iOpen = false;
        }

        public void Initialised()
        {
            if(buttonUp != null)
            {
                buttonUp.Enabled = true;
                buttonDown.Enabled = true;
                buttonMute.Enabled = true;
            }
        }

        public void SetVolume(uint aVolume)
        {
        }

        public void SetMute(bool aMute)
        {
            iMute = aMute;
        }

        public void SetVolumeLimit(uint aVolumeLimit)
        {
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;

        public void Dismiss()
        {
            CancelAutoRepeat();
        }

        public float RepeatInterval
        {
            set
            {
                iRepeatInterval = value;
            }
        }

        private void UpTouchDown(object sender, EventArgs e)
        {
            iHeldButton = sender as UIButton;

            if(iOpen && EventVolumeIncrement != null)
            {
                EventVolumeIncrement(this, EventArgs.Empty);
            }

            iTimer.Change((int)(kAutoRepeatDelay * 1000.0f), Timeout.Infinite);
        }

        private void DownTouchDown(object sender, EventArgs e)
        {
            iHeldButton = sender as UIButton;

            if(iOpen && EventVolumeDecrement != null)
            {
                EventVolumeDecrement(this, EventArgs.Empty);
            }

            iTimer.Change((int)(kAutoRepeatDelay * 1000.0f), Timeout.Infinite);
        }

        private void MuteTouchDown(object sender, EventArgs e)
        {
            if(iOpen && EventMuteChanged != null)
            {
                EventMuteChanged(this, new EventArgsMute(!iMute));
            }
        }

        private void TouchCancel(object sender, EventArgs e)
        {
            CancelAutoRepeat();
        }

        private void CancelAutoRepeat()
        {
            iHeldButton = null;
            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerUpdate(object aObject)
        {
            BeginInvokeOnMainThread(delegate {
                if(iHeldButton == buttonUp)
                {
                    if(iOpen && EventVolumeIncrement != null)
                    {
                        EventVolumeIncrement(this, EventArgs.Empty);
                    }
                }
                else if(iHeldButton == buttonDown)
                {
                    if(iOpen && EventVolumeDecrement != null)
                    {
                        EventVolumeDecrement(this, EventArgs.Empty);
                    }
                }
    
                iTimer.Change((int)(iRepeatInterval * 1000.0f), Timeout.Infinite);
            });
        }

        private const float kAutoRepeatDelay = 0.25f;

        private bool iOpen;
        private float iRepeatInterval;
        private UIButton iHeldButton;
        private System.Threading.Timer iTimer;

        private bool iMute;
    }

    public partial class ViewWidgetVolumeRotary : UIViewController, IViewWidgetVolumeControl, IViewWidgetVolumePopover
    {
        public ViewWidgetVolumeRotary(string aNibName, NSBundle aBundle)
            : base(aNibName, aBundle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            controlRotary.ImageWheel = KinskyTouch.Properties.ResourceManager.WheelLarge;
            controlRotary.ImageGrip = KinskyTouch.Properties.ResourceManager.WheelGripLarge;
            controlRotary.ImageWheelOver = KinskyTouch.Properties.ResourceManager.WheelLargeOver;

            SetCentreImage(iMute);
            
            controlRotary.EventClicked += Clicked;
            controlRotary.EventRotateClockwise += RotateClockwise;
            controlRotary.EventRotateAntiClockwise += RotateAntiClockwise;
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

        public void Open()
        {
            iOpen = true;
        }

        public void Close()
        {
            if(controlRotary != null)
            {
                controlRotary.Enabled = false;
            }
            if(imageViewMuteOff != null && imageViewMuteOn != null)
            {
                imageViewMuteOn.Alpha = 0.0f;
                imageViewMuteOff.Alpha = 0.0f;
            }

            iOpen = false;
        }

        public void Initialised()
        {
            if(controlRotary != null)
            {
                controlRotary.Enabled = true;
            }
        }

        public void SetVolume(uint aVolume)
        {
        }

        public void SetMute(bool aMute)
        {
            iMute = aMute;

            if(imageViewMuteOff != null && imageViewMuteOn != null)
            {
                SetCentreImage(aMute);
            }
        }

        public void SetVolumeLimit(uint aVolumeLimit)
        {
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;

        public void Dismiss()
        {
        }

        private void SetCentreImage(bool aMute)
        {
            UIView.BeginAnimations("togglemute");
            UIView.SetAnimationDuration(0.15f);
            if(aMute)
            {
                imageViewMuteOn.Alpha = 1.0f;
                imageViewMuteOff.Alpha = 0.0f;
            }
            else
            {
                imageViewMuteOn.Alpha = 0.0f;
                imageViewMuteOff.Alpha = 1.0f;
            }
            UIView.CommitAnimations();
        }

        private void Clicked(object sender, EventArgs e)
        {
            if(iOpen && EventMuteChanged != null)
            {
                EventMuteChanged(this, new EventArgsMute(!iMute));
            }
        }

        private void RotateClockwise(object sender, EventArgs e)
        {
            if(iOpen && EventVolumeIncrement != null)
            {
                EventVolumeIncrement(this, EventArgs.Empty);
            }
        }

        private void RotateAntiClockwise(object sender, EventArgs e)
        {
            if(iOpen && EventVolumeDecrement != null)
            {
                EventVolumeDecrement(this, EventArgs.Empty);
            }
        }

        private bool iOpen;

        private bool iMute;
    }
}

