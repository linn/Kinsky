using System;
using System.Threading;

using Linn.Kinsky;

using Upnp;

using UIKit;
using Foundation;

namespace KinskyTouch
{
    public interface IViewWidgetTimeDisplay
    {
        bool ShowingElapsed { get; }
        void ToggleTimeDisplay();

        void SetTargetSeconds(uint aSeconds);

        void StartSeek();
        void EndSeek();
    }

    internal class ViewWidgetTime : IViewWidgetMediaTime, IViewWidgetTimeDisplay
    {
        public ViewWidgetTime(UIControlWheel aControl, ViewHourGlass aViewHourGlass)
        {
            iControl = aControl;

            iViewHourGlass = aViewHourGlass;
        }

        public void Open()
        {
            lock(this)
            {
                iOpen = true;
            }
        }

        public void Close()
        {
            iControl.ViewBar.Value = 0;
            iControl.ViewBar.Text = string.Empty;

            iViewHourGlass.Stop();

            UIControl control = iControl as UIControl;
            control.BeginInvokeOnMainThread(delegate {
                control.Enabled = false;
            });

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

        public void SetAllowSeeking(bool aAllowSeeking)
        {
            iAllowSeeking = aAllowSeeking;
        }

        public void SetTransportState(ETransportState aTransportState)
        {
            if(iOpen)
            {
                iTransportState = aTransportState;

                if(aTransportState == ETransportState.eStopped)
                {
                    SetSeconds(0);
                }
                else
                {
                    SetSeconds(iSeconds);
                }

                iControl.Dimmed = (aTransportState == ETransportState.ePaused);
                if(aTransportState == ETransportState.eBuffering)
                {
                    iViewHourGlass.Start();
                }
                else
                {
                    iViewHourGlass.Stop();
                }
            }
        }

        public void SetDuration(uint aDuration)
        {
            if(iOpen)
            {
                iControl.ViewBar.MaxValue = (float)aDuration;
                iDuration = aDuration;

                float seek = iDuration / 100.0f;
                iSeekAmountPerStep = (uint)Math.Round(seek + 0.5f, MidpointRounding.AwayFromZero);
            }
        }

        public void SetSeconds(uint aSeconds)
        {
            if(iOpen)
            {
                iSeconds = aSeconds;

                if(iTransportState != ETransportState.eBuffering && iTransportState != ETransportState.eStopped)
                {
                    if(!iApplyTargetSeconds)
                    {
                        if(iShowRemaining && iDuration > 0)
                        {
                            iTime = new Time((int)-(iDuration - aSeconds));
                        }
                        else
                        {
                            iTime = new Time((int)aSeconds);
                        }

                        iControl.ViewBar.Text = iTime.ToPrettyString();
                    }
                    
                    iControl.ViewBar.Value = (float)iSeconds;
                }
                else
                {
                    iControl.ViewBar.Value = 0;
                    iControl.ViewBar.Text = "";
                }
            }
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;

        public bool ShowingElapsed
        {
            get
            {
                return !iShowRemaining;
            }
        }

        public void ToggleTimeDisplay()
        {
            iShowRemaining = !iShowRemaining;
            SetSeconds(iSeconds);
        }

        public void SetTargetSeconds(uint aSeconds)
        {
            if(iOpen)
            {
                if(!iApplyTargetSeconds)
                {
                    iApplyTargetSeconds = true;

                    iControl.ViewBar.FontColour = UIColor.Yellow;
                    iControl.ViewBar.PreviewEnabled = true;
                }

                iTargetSeconds = aSeconds;

                if(iTransportState != ETransportState.eBuffering)
                {
                    if (iApplyTargetSeconds)
                    {
                        string t = string.Empty;
                        if (iShowRemaining)
                        {
                            Time time = new Time((int)(iTargetSeconds - iDuration));
                            t = time.ToPrettyString();
                        }
                        else
                        {
                            Time time = new Time((int)iTargetSeconds);
                            t = time.ToPrettyString();
                        }

                        iControl.ViewBar.PreviewValue = iTargetSeconds;
                        iControl.ViewBar.Text = t;
                    }
                }
            }
        }

        public void StartSeek()
        {
            if(iOpen && iAllowSeeking && iDuration > 0)
            {
                iSeeking = true;
                iApplyTargetSeconds = false;
            }
        }

        public void EndSeek()
        {
            iSeeking = false;
    
            if (iTransportState == ETransportState.eStopped)
            {
                iControl.ViewBar.Text = string.Empty;
            }

            iControl.ViewBar.FontColour = UIColor.White;
            iControl.ViewBar.PreviewValue = 0;
            iControl.ViewBar.PreviewEnabled = false;

            iApplyTargetSeconds = false;
            iTargetSeconds = 0;

            SetSeconds(iSeconds);
        }

        private void Clicked(object sender, EventArgs e)
        {
            ToggleTimeDisplay();
        }

        private void EventStartSeeking(object sender, EventArgs e)
        {
            StartSeek();
        }

        private void EventEndSeeking(object sender, EventArgs e)
        {
            if(iOpen)
            {
                if(iSeeking)
                {
    
                    if(iApplyTargetSeconds)
                    {
                        iSeconds = iTargetSeconds;
    
                        if(EventSeekSeconds != null)
                        {
                            EventSeekSeconds(this, new EventArgsSeekSeconds(iTargetSeconds));
                        }
                    }

                    EndSeek();
                }
            }
        }

        private void EventCancelSeeking(object sender, EventArgs e)
        {
            if (iOpen)
            {
                if(iSeeking)
                {
                    EndSeek();
                }
            }
        }

        private void EventSeekForwards(object sender, EventArgs e)
        {
            if (iOpen && iSeeking)
            {
                if (!iApplyTargetSeconds)
                {
                    iTargetSeconds = iSeconds;
                }

                iTargetSeconds += iSeekAmountPerStep;

                if (iTargetSeconds > iDuration)
                {
                    iTargetSeconds = iDuration;
                }

                SetTargetSeconds(iTargetSeconds);
            }
        }

        private void EventSeekBackwards(object sender, EventArgs e)
        {
            if (iOpen && iSeeking)
            {
                if (!iApplyTargetSeconds)
                {
                    iTargetSeconds = iSeconds;
                }

                if (iTargetSeconds > iSeekAmountPerStep)
                {
                    iTargetSeconds -= iSeekAmountPerStep;
                }
                else
                {
                    iTargetSeconds = 0;
                }

                SetTargetSeconds(iTargetSeconds);
            }
        }

        private bool iOpen;

        private Time iTime;
        private uint iSeconds;
        private uint iDuration;
        private ETransportState iTransportState;

        private bool iAllowSeeking;
        private bool iShowRemaining;

        private uint iSeekAmountPerStep;
        private bool iSeeking;
        private bool iApplyTargetSeconds;
        private uint iTargetSeconds;

        private UIControlWheel iControl;
        private ViewHourGlass iViewHourGlass;
    }

    internal interface IViewWidgetTimePopover
    {
        void Dismiss();
    }

    public partial class ViewWidgetTimeButtons : UIViewController, IViewWidgetMediaTime, IViewWidgetTimePopover
    {
        public ViewWidgetTimeButtons(string aNibName, NSBundle aBundle, IViewWidgetTimeDisplay aViewWidgetTimeDisplay)
            : base(aNibName, aBundle)
        {
            iViewWidgetTimeDisplay = aViewWidgetTimeDisplay;
            iTimer = new System.Threading.Timer(TimerUpdate);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            buttonFfwd.TouchDown += FfwdTouchDown;
            buttonFfwd.TouchUpInside += TouchUpInside;
            buttonFfwd.TouchUpOutside += TouchCancel;
            buttonFfwd.TouchDragOutside += TouchCancel;
            
            buttonFrwd.TouchDown += FrwdTouchDown;
            buttonFrwd.TouchUpInside += TouchUpInside;
            buttonFrwd.TouchUpOutside += TouchCancel;
            buttonFrwd.TouchDragOutside += TouchCancel;
            
            buttonTime.TouchDown += TimeTouchDown;
            
            buttonFfwd.Enabled = iAllowSeeking && (iDuration > 0);
            buttonFrwd.Enabled = iAllowSeeking && (iDuration > 0);
            buttonTime.Enabled = iAllowSeeking && (iDuration > 0);
        }

        [Obsolete]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
        {
            return UIInterfaceOrientationMask.All;
        }

        public void Open()
        {
            iOpen = true;
        }

        public void Close()
        {
            iOpen = false;
        }

        public void Initialised()
        {
            if(buttonTime != null)
            {
                buttonFfwd.Enabled = iAllowSeeking && (iDuration > 0);
                buttonFrwd.Enabled = iAllowSeeking && (iDuration > 0);
                buttonTime.Enabled = iAllowSeeking && (iDuration > 0);
            }
        }

        public void SetAllowSeeking(bool aAllowSeeking)
        {
            iAllowSeeking = aAllowSeeking;

            if(iOpen)
            {
                if(buttonTime != null)
                {
                    lock(this)
                    {
                        buttonFfwd.Enabled = aAllowSeeking && (iDuration > 0);
                        buttonFrwd.Enabled = aAllowSeeking && (iDuration > 0);
                        buttonTime.Enabled = aAllowSeeking && (iDuration > 0);
                    }
                }
            }
        }

        public void SetTransportState(ETransportState aTransportState)
        {
        }

        public void SetDuration(uint aDuration)
        {
            iDuration = aDuration;

            float seek = aDuration / 100.0f;
            iSeekAmountPerStep = (uint)Math.Round(seek + 0.5f, MidpointRounding.AwayFromZero);

            if(iOpen)
            {
                if(buttonTime != null)
                {
                    buttonFfwd.Enabled = iAllowSeeking && (aDuration > 0);
                    buttonFrwd.Enabled = iAllowSeeking && (aDuration > 0);
                    buttonTime.Enabled = iAllowSeeking && (aDuration > 0);
                }
            }
        }

        public void SetSeconds(uint aSeconds)
        {
            iSeconds = aSeconds;
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;

        public void Dismiss()
        {
            CancelAutoRepeat();
            iViewWidgetTimeDisplay.EndSeek();
        }

        public float RepeatInterval
        {
            set
            {
                iRepeatInterval = value;
            }
        }

        private void TimeTouchDown(object sender, EventArgs e)
        {
            iViewWidgetTimeDisplay.ToggleTimeDisplay();
        }

        private void FfwdTouchDown(object sender, EventArgs e)
        {
            iTargetSeconds = iSeconds;

            iHeldButton = buttonFfwd as UIButton;

            iViewWidgetTimeDisplay.StartSeek();

            iTimer.Change((int)(kAutoRepeatDelay * 1000.0f), Timeout.Infinite);
        }

        private void FrwdTouchDown(object sender, EventArgs e)
        {
            iTargetSeconds = iSeconds;

            iHeldButton = buttonFrwd as UIButton;

            iViewWidgetTimeDisplay.StartSeek();

            iTimer.Change((int)(kAutoRepeatDelay * 1000.0f), Timeout.Infinite);
        }

        private void TouchUpInside(object sender, EventArgs e)
        {
            if(iOpen && EventSeekSeconds != null)
            {
                EventSeekSeconds(this, new EventArgsSeekSeconds(iTargetSeconds));
            }

            CancelAutoRepeat();
            iViewWidgetTimeDisplay.EndSeek();
        }

        private void TouchCancel(object sender, EventArgs e)
        {
            CancelAutoRepeat();
            iViewWidgetTimeDisplay.EndSeek();
        }

        private void CancelAutoRepeat()
        {
            iHeldButton = null;
            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerUpdate(object aObject)
        {
            if(iOpen)
            {
                if(iHeldButton == buttonFfwd)
                {
                    iTargetSeconds += iSeekAmountPerStep;

                    if(iTargetSeconds > iDuration)
                    {
                        iTargetSeconds = iDuration;
                    }
                }
                else if(iHeldButton == buttonFrwd)
                {
                    if(iTargetSeconds > iSeekAmountPerStep)
                    {
                        iTargetSeconds -= iSeekAmountPerStep;
                    }
                    else
                    {
                        iTargetSeconds = 0;
                    }
                }

                BeginInvokeOnMainThread(delegate {
                    iViewWidgetTimeDisplay.SetTargetSeconds(iTargetSeconds);
                });

                iTimer.Change((int)(iRepeatInterval * 1000.0f), Timeout.Infinite);
            }
        }

        private const float kAutoRepeatDelay = 0.25f;

        private bool iOpen;
        private float iRepeatInterval;
        private UIButton iHeldButton;
        private System.Threading.Timer iTimer;

        private uint iSeconds;
        private uint iDuration;
        private bool iAllowSeeking;
        private uint iTargetSeconds;
        private uint iSeekAmountPerStep;

        private IViewWidgetTimeDisplay iViewWidgetTimeDisplay;
    }

    public partial class ViewWidgetTimeRotary : UIViewController, IViewWidgetMediaTime, IViewWidgetTimePopover
    {
        public ViewWidgetTimeRotary(string aNibName, NSBundle aBundle, IViewWidgetTimeDisplay aViewWidgetTimeDisplay)
            : base(aNibName, aBundle)
        {
            iViewWidgetTimeDisplay = aViewWidgetTimeDisplay;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            controlRotary.ImageWheel = KinskyTouch.Properties.ResourceManager.WheelLarge;
            controlRotary.ImageGrip = KinskyTouch.Properties.ResourceManager.WheelGripLarge;
            controlRotary.ImageWheelOver = KinskyTouch.Properties.ResourceManager.WheelLargeOver;

            SetCentreImage();
            
            controlRotary.EventClicked += Clicked;
            
            controlRotary.EventStartRotation += StartRotation;
            controlRotary.EventEndRotation += EndRotation;
            controlRotary.EventCancelRotation += CancelRotation;
            
            controlRotary.EventRotateClockwise += RotateClockwise;
            controlRotary.EventRotateAntiClockwise += RotateAntiClockwise;
            
            controlRotary.Enabled = iAllowSeeking && (iDuration > 0);
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
            if(imageViewElapsed != null && imageViewRemaining != null)
            {
                imageViewElapsed.Alpha = 0.0f;
                imageViewRemaining.Alpha = 0.0f;
            }

            iOpen = false;
        }

        public void Initialised()
        {
            if(controlRotary != null)
            {
                controlRotary.Enabled = iAllowSeeking && (iDuration > 0);
            }
            if(imageViewElapsed != null && imageViewRemaining != null)
            {
                SetCentreImage();
            }
        }

        public void SetAllowSeeking(bool aAllowSeeking)
        {
            iAllowSeeking = aAllowSeeking;

            if(iOpen)
            {
                if(controlRotary != null)
                {
                    controlRotary.Enabled = aAllowSeeking && (iDuration > 0);
                }
            }
        }

        public void SetTransportState(ETransportState aTransportState)
        {
        }

        public void SetDuration(uint aDuration)
        {
            iDuration = aDuration;

            float seek = aDuration / 100.0f;
            iSeekAmountPerStep = (uint)Math.Round(seek + 0.5f, MidpointRounding.AwayFromZero);

            if(iOpen)
            {
                if(controlRotary != null)
                {
                    lock(this)
                    {
                        controlRotary.Enabled = iAllowSeeking && (aDuration > 0);
                    }
                }
            }
        }

        public void SetSeconds(uint aSeconds)
        {
            iSeconds = aSeconds;
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;

        public void Dismiss()
        {
            iViewWidgetTimeDisplay.EndSeek();
        }

        private void SetCentreImage()
        {
            UIView.BeginAnimations("toggletime");
            UIView.SetAnimationDuration(0.15f);
            if(iViewWidgetTimeDisplay.ShowingElapsed)
            {
                imageViewElapsed.Alpha = 1.0f;
                imageViewRemaining.Alpha = 0.0f;
            }
            else
            {
                imageViewElapsed.Alpha = 0.0f;
                imageViewRemaining.Alpha = 1.0f;
            }
            UIView.CommitAnimations();
        }

        private void Clicked(object sender, EventArgs e)
        {
            iViewWidgetTimeDisplay.ToggleTimeDisplay();
            SetCentreImage();
        }

        private void StartRotation(object sender, EventArgs e)
        {
            iTargetSeconds = iSeconds;

            iViewWidgetTimeDisplay.StartSeek();
        }

        private void EndRotation(object sender, EventArgs e)
        {
            if(iOpen && EventSeekSeconds != null)
            {
                EventSeekSeconds(this, new EventArgsSeekSeconds(iTargetSeconds));
            }

            iViewWidgetTimeDisplay.EndSeek();
        }

        private void CancelRotation(object sender, EventArgs e)
        {
            iViewWidgetTimeDisplay.EndSeek();
        }

        private void RotateClockwise(object sender, EventArgs e)
        {
            iTargetSeconds += iSeekAmountPerStep;

            if(iTargetSeconds > iDuration)
            {
                iTargetSeconds = iDuration;
            }

            iViewWidgetTimeDisplay.SetTargetSeconds(iTargetSeconds);
        }

        private void RotateAntiClockwise(object sender, EventArgs e)
        {
            if(iTargetSeconds > iSeekAmountPerStep)
            {
                iTargetSeconds -= iSeekAmountPerStep;
            }
            else
            {
                iTargetSeconds = 0;
            }

            iViewWidgetTimeDisplay.SetTargetSeconds(iTargetSeconds);
        }

        private bool iOpen;

        private uint iSeconds;
        private uint iDuration;
        private bool iAllowSeeking;
        private uint iTargetSeconds;
        private uint iSeekAmountPerStep;

        private IViewWidgetTimeDisplay iViewWidgetTimeDisplay;
    }
}

