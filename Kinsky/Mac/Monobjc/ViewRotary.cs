
using System;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    public interface IViewRotary
    {
        event EventHandler<EventArgs> EventClockwiseStep;
        event EventHandler<EventArgs> EventAnticlockwiseStep;
        event EventHandler<EventArgs> EventStart;
        event EventHandler<EventArgs> EventStop;
        event EventHandler<EventArgs> EventCancelled;
        event EventHandler<EventArgs> EventClicked;

        bool Enabled { set; }
        bool Dimmed { set; }
        ViewRotaryBar ViewBar { get; }
    }


    [ObjectiveCClass]
    public class ViewRocker : NSView, IViewRotary
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewRocker));

        public ViewRocker() : base() {}
        public ViewRocker(IntPtr aInstance) : base(aInstance) {}

        public double RepeatInterval
        {
            set { iRepeatInterval = value; }
        }

        #region IViewRotary implementation
        public event EventHandler<EventArgs> EventClockwiseStep;
        public event EventHandler<EventArgs> EventAnticlockwiseStep;
        public event EventHandler<EventArgs> EventStart;
        public event EventHandler<EventArgs> EventStop;
        public event EventHandler<EventArgs> EventCancelled;
        public event EventHandler<EventArgs> EventClicked;

        public bool Enabled
        {
            set
            {
                iEnabled = value;
                UpdateState();
            }
        }

        public bool Dimmed
        {
            set
            {
                iDimmed = value;
                UpdateState();
            }
        }

        public ViewRotaryBar ViewBar
        {
            get { return iViewBar; }
        }
        #endregion IViewRotary implementation


        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect aFrameRect)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ThisClass, "initWithFrame:", aFrameRect);

            NSRect wheelRect = new NSRect(0, 0, aFrameRect.Width, aFrameRect.Height);

            if (iViewBar == null)
            {
                // create subviews
                iViewBar = new ViewRotaryBar();
                iViewBkgd = new NSImageView();
                iViewLeft = new NSImageView();
                iViewRight = new NSImageView();
                iViewMute = new NSImageView();

                // create subview hierarchy
                this.AddSubview(iViewBkgd);
                iViewBkgd.AddSubview(iViewBar);
                iViewBar.AddSubview(iViewLeft);
                iViewBar.AddSubview(iViewRight);
                iViewBar.AddSubview(iViewMute);
            }

            // initialise the views
            iViewBar.InitWithFrame(wheelRect);

            iViewBkgd.InitWithFrame(wheelRect);
            iViewBkgd.Image = Properties.Resources.ImageRocker;
            iViewBkgd.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iViewBkgd.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iViewBkgd.ImageScaling = NSImageScaling.NSImageScaleNone;

            iViewLeft.InitWithFrame(wheelRect);
            iViewLeft.Image = Properties.Resources.ImageRockerLeftOver;
            iViewLeft.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iViewLeft.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iViewLeft.ImageScaling = NSImageScaling.NSImageScaleNone;

            iViewRight.InitWithFrame(wheelRect);
            iViewRight.Image = Properties.Resources.ImageRockerRightOver;
            iViewRight.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iViewRight.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iViewRight.ImageScaling = NSImageScaling.NSImageScaleNone;

            iViewMute.InitWithFrame(wheelRect);
            iViewMute.Image = Properties.Resources.ImageWheelMute;
            iViewMute.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iViewMute.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iViewMute.ImageScaling = NSImageScaling.NSImageScaleNone;


            // initialise state of the widget
            iEnabled = false;
            iDimmed = false;
            iMouseDown = EMouse.eNot;
            iMouseOver = EMouse.eNot;
            UpdateState();

            // create tracking area
            iTrackingArea = TrackerHelper.Create(this, wheelRect, true);

            return this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iViewBar.Release();
            iViewBkgd.Release();
            iViewLeft.Release();
            iViewRight.Release();
            iViewMute.Release();
            iTrackingArea.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("updateTrackingAreas")]
        public override void UpdateTrackingAreas()
        {
            this.SendMessageSuper(ThisClass, "updateTrackingAreas");

            TrackerHelper.Destroy(this, iTrackingArea);
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, true);
        }

        [ObjectiveCMessage("mouseEntered:")]
        public override void MouseEntered(NSEvent aEvent)
        {
            // set the mouse over flag
            iMouseOver = MouseLocation(aEvent.LocationInWindow);

            UpdateState();
        }

        [ObjectiveCMessage("mouseMoved:")]
        public override void MouseMoved(NSEvent aEvent)
        {
            // update mouse over
            iMouseOver = MouseLocation(aEvent.LocationInWindow);

            UpdateState();
        }

        [ObjectiveCMessage("mouseExited:")]
        public override void MouseExited(NSEvent aEvent)
        {
            // clear the mouse over flag
            iMouseOver = EMouse.eNot;

            UpdateState();
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            if (iEnabled)
            {
                // get current mouse position
                iMouseDown = MouseLocation(aEvent.LocationInWindow);

                UpdateState();

                if (iMouseDown == EMouse.eLeft || iMouseDown == EMouse.eRight)
                {
                    // send notification that "rotation" has started
                    if (EventStart != null)
                    {
                        EventStart(this, EventArgs.Empty);
                    }

                    // do a step now and then set a timer for repeat
                    DoStep();

                    // use a fixed delay of 0.25 for the first delay
                    this.PerformSelectorWithObjectAfterDelay(ObjectiveCRuntime.Selector("timerUpdate"), null, 0.25);
                }
            }
        }

        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent aEvent)
        {
            // update the mouse over state
            iMouseOver = MouseLocation(aEvent.LocationInWindow);

            UpdateState();
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            try
            {
                if (iEnabled)
                {
                    EMouse currMousePos = MouseLocation(aEvent.LocationInWindow);

                    switch (iMouseDown)
                    {
                    case EMouse.eCentre:
                        // click if the mouse is still in the centre
                        if (currMousePos == ViewRocker.EMouse.eCentre)
                        {
                            if (EventClicked != null)
                            {
                                EventClicked(this, EventArgs.Empty);
                            }
                        }
                        break;

                    case EMouse.eLeft:
                    case EMouse.eRight:
                        // signal that "rotating" has stopped or been cancelled
                        if (currMousePos != EMouse.eNot)
                        {
                            if (EventStop != null)
                            {
                                EventStop(this, EventArgs.Empty);
                            }
                        }
                        else
                        {
                            if (EventCancelled != null)
                            {
                                EventCancelled(this, EventArgs.Empty);
                            }
                        }
                        break;
                    }
                }
            }
            finally
            {
                // always update the mouse down state - this is **very** important that this is not inside
                // some sort of if statement - if there is a chance that this iMouseDown state does not get
                // set to eNot, then it might get stuck in, for example, eRight which, for volume control
                // might mean that the volume keeps increasing until maximum!!!!
                iMouseDown = EMouse.eNot;

                UpdateState();
            }
        }

        [ObjectiveCMessage("timerUpdate")]
        public void TimerUpdate()
        {
            if (iEnabled && (iMouseDown == EMouse.eLeft || iMouseDown == EMouse.eRight))
            {
                DoStep();

                this.PerformSelectorWithObjectAfterDelay(ObjectiveCRuntime.Selector("timerUpdate"), null, iRepeatInterval);
            }
        }

        private void DoStep()
        {
            if (iMouseDown == EMouse.eLeft)
            {
                if (EventAnticlockwiseStep != null)
                {
                    EventAnticlockwiseStep(this, EventArgs.Empty);
                }
            }
            else if (iMouseDown == EMouse.eRight)
            {
                if (EventClockwiseStep != null)
                {
                    EventClockwiseStep(this, EventArgs.Empty);
                }
            }
        }

        private EMouse MouseLocation(NSPoint aPtInWindow)
        {
            NSPoint ptInView = ConvertPointFromView(aPtInWindow, null);

            if (Bounds.PointInRect(ptInView))
            {
                NSPoint centre = new NSPoint((Bounds.MinX + Bounds.MaxX)*0.5f, (Bounds.MinY + Bounds.MaxY)*0.5f);

                float dx = ptInView.x - centre.x;
                float dy = ptInView.y - centre.y;
                float r2 = dx*dx + dy*dy;

                if (r2 < iInnerRadius*iInnerRadius)
                {
                    return EMouse.eCentre;
                }
                else if (dx < 0.0f)
                {
                    return EMouse.eLeft;
                }
                else
                {
                    return EMouse.eRight;
                }
            }
            else
            {
                return EMouse.eNot;
            }
        }

        private void UpdateState()
        {
            if (iEnabled)
            {
                switch (iMouseDown)
                {
                case EMouse.eNot:
                    iViewLeft.Animator.AlphaValue = (iMouseOver == EMouse.eLeft) ? 1.0f : 0.0f;
                    iViewRight.Animator.AlphaValue = (iMouseOver == EMouse.eRight) ? 1.0f : 0.0f;
                    break;
                case EMouse.eLeft:
                    iViewLeft.Animator.AlphaValue = 1.0f;
                    iViewRight.Animator.AlphaValue = 0.0f;
                    break;
                case EMouse.eRight:
                    iViewLeft.Animator.AlphaValue = 0.0f;
                    iViewRight.Animator.AlphaValue = 1.0f;
                    break;
                case EMouse.eCentre:
                    iViewLeft.Animator.AlphaValue = 0.0f;
                    iViewRight.Animator.AlphaValue = 0.0f;
                    break;
                }

                // set the state of the mute
                iViewMute.Animator.AlphaValue = iDimmed ? 1.0f : 0.0f;

                // set the pressed states for the left and right buttons
                iViewLeft.Image = (iMouseDown == EMouse.eLeft) ? Properties.Resources.ImageRockerLeftDown : Properties.Resources.ImageRockerLeftOver;
                iViewRight.Image = (iMouseDown == EMouse.eRight) ? Properties.Resources.ImageRockerRightDown : Properties.Resources.ImageRockerRightOver;
            }
            else
            {
                iViewLeft.Animator.AlphaValue = 0.0f;
                iViewRight.Animator.AlphaValue = 0.0f;
                iViewMute.Animator.AlphaValue = 0.0f;
            }

            this.NeedsDisplay = true;
        }

        private const float iInnerRadius = 20.0f;

        private ViewRotaryBar iViewBar;
        private NSImageView iViewBkgd;
        private NSImageView iViewLeft;
        private NSImageView iViewRight;
        private NSImageView iViewMute;

        private enum EMouse
        {
            eNot,
            eLeft,
            eRight,
            eCentre
        }

        private bool iEnabled;
        private bool iDimmed;
        private EMouse iMouseDown;
        private EMouse iMouseOver;
        private NSTrackingArea iTrackingArea;
        private double iRepeatInterval = 0.1;
    }


    [ObjectiveCClass]
    public class ViewRotary : NSView, IViewRotary
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewRotary));

        public ViewRotary() : base() {}
        public ViewRotary(IntPtr aInstance) : base(aInstance) {}


        #region IViewRotary implementation
        public event EventHandler<EventArgs> EventClockwiseStep;
        public event EventHandler<EventArgs> EventAnticlockwiseStep;
        public event EventHandler<EventArgs> EventStart;
        public event EventHandler<EventArgs> EventStop;
        public event EventHandler<EventArgs> EventCancelled;
        public event EventHandler<EventArgs> EventClicked;

        public bool Enabled
        {
            set
            {
                iEnabled = value;
                UpdateState();
            }
        }

        public bool Dimmed
        {
            set
            {
                iDimmed = value;
                UpdateState();
            }
        }

        public ViewRotaryBar ViewBar
        {
            get { return iViewBar; }
        }
        #endregion IViewRotary implementation


        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect aFrameRect)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ThisClass, "initWithFrame:", aFrameRect);

            NSRect wheelRect = new NSRect(0, 0, aFrameRect.Width, aFrameRect.Height);

            if (iViewWheel == null)
            {
                // create subviews
                iViewWheel = new ViewRotaryImage();
                iViewMute = new NSImageView();
                iViewBar = new ViewRotaryBar();
                iViewGrip = new ViewRotaryGrip();

                // add views to the parent in back to front order
                this.AddSubview(iViewWheel);
                iViewWheel.AddSubview(iViewGrip);
                iViewGrip.AddSubview(iViewBar);
                iViewBar.AddSubview(iViewMute);
            }


            // initialise the views
            iViewWheel.InitWithFrame(wheelRect);
            iViewWheel.SetImages(Properties.Resources.ImageWheel, NSImageAlignment.NSImageAlignCenter,
                                 Properties.Resources.ImageWheelOver, NSImageAlignment.NSImageAlignCenter);

            iViewGrip.InitWithFrame(wheelRect);

            iViewBar.InitWithFrame(wheelRect);

            iViewMute.InitWithFrame(wheelRect);
            iViewMute.Image = Properties.Resources.ImageWheelMute;
            iViewMute.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iViewMute.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iViewMute.ImageScaling = NSImageScaling.NSImageScaleNone;


            // initialise angle for the control knob
            iKnobAngle = 270.0f;
            iViewGrip.Angle = iKnobAngle;

            // initialise state of the widget
            iEnabled = false;
            iMouseState = EMouse.eUp;
            iMouseOver = false;
            iDimmed = false;
            UpdateState();


            // create tracking area
            iTrackingArea = TrackerHelper.Create(this, wheelRect, false);

            return this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iViewWheel.Release();
            iViewGrip.Release();
            iViewBar.Release();
            iViewMute.Release();
            iTrackingArea.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("updateTrackingAreas")]
        public override void UpdateTrackingAreas()
        {
            this.SendMessageSuper(ThisClass, "updateTrackingAreas");

            TrackerHelper.Destroy(this, iTrackingArea);
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);
        }

        [ObjectiveCMessage("mouseEntered:")]
        public override void MouseEntered(NSEvent aEvent)
        {
            // state flag for mouse over always gets set
            iMouseOver = true;

            UpdateState();
        }

        [ObjectiveCMessage("mouseExited:")]
        public override void MouseExited(NSEvent aEvent)
        {
            // state flag for mouse over always gets cleared
            iMouseOver = false;

            UpdateState();
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            if (iEnabled)
            {
                // calculate mouse distance from centre
                NSPoint ptInView = ConvertPointFromView(aEvent.LocationInWindow, null);

                float dx = ptInView.x - Centre.x;
                float dy = ptInView.y - Centre.y;
                float r2 = dx*dx + dy*dy;

                // do a hit test for clicking or rotating
                if (r2 < iInnerRadius*iInnerRadius)
                {
                    // mouse down in centre
                    iMouseState = EMouse.eDownCentre;
                    UpdateState();
                }
                else
                {
                    // mouse down for rotation of the control
                    iMouseState = EMouse.eDownRing;
                    UpdateState();

                    iMouseDownAngle = CalculateMouseAngle(ptInView);
                    iLastMouseAngle = iMouseDownAngle;

                    // send notification that rotation has started
                    if (EventStart != null)
                    {
                        EventStart(this, EventArgs.Empty);
                    }
                }
            }
        }

        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent aEvent)
        {
            // only need to act on this event if the widget is enabled and
            // the mouse is down on the outer ring
            if (iEnabled && iMouseState == EMouse.eDownRing)
            {
                // get the current mouse angle
                float mouseAngle = CalculateMouseAngle(ConvertPointFromView(aEvent.LocationInWindow, null));

                // set the current angle for the knob
                iViewGrip.Angle = iKnobAngle + mouseAngle - iMouseDownAngle;

                this.NeedsDisplay = true;

                // calculate the change in angle and make sure it is in the range [-180, 180]
                float angleDiff = mouseAngle - iLastMouseAngle;
                while (angleDiff > 180.0f)
                    angleDiff -= 360.0f;
                while (angleDiff < -180.0f)
                    angleDiff += 360.0f;

                // the rotation is split into segments - calculate whether or not the rotation has moved
                // from 1 segment to another
                // note the use of (iLastMouseAngle + angleDiff) instead of mouseAngle - this avoids the situation
                // where lastSegment=0 and thisSegment=11, which is now interpreted as lastSegment=0 and thisSegment=-1,
                // which gives us the correct behaviour
                int lastSegment = (int)Math.Floor((iLastMouseAngle - iMouseDownAngle) / 30.0f);
                int thisSegment = (int)Math.Floor((iLastMouseAngle + angleDiff - iMouseDownAngle) / 30.0f);

                if (thisSegment > lastSegment)
                {
                    if (EventAnticlockwiseStep != null)
                    {
                        EventAnticlockwiseStep(this, EventArgs.Empty);
                    }
                }
                else if (thisSegment < lastSegment)
                {
                    if (EventClockwiseStep != null)
                    {
                        EventClockwiseStep(this, EventArgs.Empty);
                    }
                }

                iLastMouseAngle = mouseAngle;
            }
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            if (iEnabled)
            {
                // calculate mouse distance from centre
                NSPoint ptInView = ConvertPointFromView(aEvent.LocationInWindow, null);

                float dx = ptInView.x - Centre.x;
                float dy = ptInView.y - Centre.y;
                float r2 = dx*dx + dy*dy;

                if (iMouseState == EMouse.eDownRing)
                {
                    // get the current mouse angle
                    float mouseAngle = CalculateMouseAngle(ptInView);

                    // set the knob angle
                    iKnobAngle += mouseAngle - iMouseDownAngle;
                    iViewGrip.Angle = iKnobAngle;

                    this.NeedsDisplay = true;

                    // send notification that rotation has stopped
                    if (r2 < iOuterRadius*iOuterRadius)
                    {
                        if (EventStop != null)
                        {
                            EventStop(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if (EventCancelled != null)
                        {
                            EventCancelled(this, EventArgs.Empty);
                        }
                    }
                }
                else if (iMouseState == EMouse.eDownCentre)
                {
                    // check if the mouse is still inside the centre circle
                    if (r2 < iOuterRadius*iOuterRadius)
                    {
                        if (EventClicked != null)
                        {
                            EventClicked(this, EventArgs.Empty);
                        }
                    }
                }
            }

            // flag for mouse down always gets cleared
            iMouseState = EMouse.eUp;

            UpdateState();
        }

        private float CalculateMouseAngle(NSPoint aPtInView)
        {
            double radians = Math.Atan2(aPtInView.y - Centre.y, aPtInView.x - Centre.x);

            return (float)(radians * 180.0 / Math.PI);
        }

        private NSPoint Centre
        {
            get { return new NSPoint((Bounds.MinX + Bounds.MaxX)*0.5f, (Bounds.MinY + Bounds.MaxY)*0.5f); }
        }

        private void UpdateState()
        {
            if (iEnabled)
            {
                if (iMouseOver || iMouseState != EMouse.eUp)
                {
                    iViewWheel.ShowOver(true);
                }
                else
                {
                    iViewWheel.ShowOver(false);
                }

                iViewMute.Animator.AlphaValue = iDimmed ? 1.0f : 0.0f;
            }
            else
            {
                // disabled
                iViewWheel.ShowOver(false);
                iViewMute.Animator.AlphaValue = 0.0f;
            }

            this.NeedsDisplay = true;
        }

        private const float iInnerRadius = 28.0f;
        private const float iOuterRadius = 47.5f;

        private ViewRotaryImage iViewWheel;
        private NSImageView iViewMute;
        private ViewRotaryBar iViewBar;
        private ViewRotaryGrip iViewGrip;

        private float iKnobAngle;
        private float iMouseDownAngle;
        private float iLastMouseAngle;

        // enum to define 3 mutually exclusive aspects of the state
        private enum EMouse
        {
            eUp,
            eDownRing,
            eDownCentre
        }

        private bool iEnabled;
        private bool iMouseOver;
        private EMouse iMouseState;
        private bool iDimmed;
        private NSTrackingArea iTrackingArea;
    }


    // Class representing a component of the rotary control i.e. the wheel or the knob
    [ObjectiveCClass]
    public class ViewRotaryImage : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewRotaryImage));

        public ViewRotaryImage() : base() {}
        public ViewRotaryImage(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect aFrameRect)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ThisClass, "initWithFrame:", aFrameRect);

            // create the sub views and attach
            if (iImage == null)
            {
                iImage = new NSImageView();
                iImageOver = new NSImageView();

                this.AddSubview(iImage);
                iImage.AddSubview(iImageOver);
            }

            // initialise the subviews
            NSRect frame = new NSRect(0, 0, aFrameRect.Width, aFrameRect.Height);

            iImage.InitWithFrame(frame);
            iImage.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iImage.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iImage.ImageScaling = NSImageScaling.NSImageScaleNone;

            iImageOver.InitWithFrame(frame);
            iImageOver.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iImageOver.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iImageOver.ImageScaling = NSImageScaling.NSImageScaleNone;

            iImageOver.AlphaValue = 0.0f;

            return this;
        }

        public void SetImages(NSImage aImage, NSImageAlignment aAlignment, NSImage aImageOver, NSImageAlignment aAlignmentOver)
        {
            iImage.Image = aImage;
            iImage.ImageAlignment = aAlignment;

            iImageOver.Image = aImageOver;
            iImageOver.ImageAlignment = aAlignmentOver;
        }

        public void ShowOver(bool aShow)
        {
            iImageOver.Animator.AlphaValue = aShow ? 1.0f : 0.0f;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iImage.Release();
            iImageOver.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        private NSImageView iImage;
        private NSImageView iImageOver;
    }


    // Class representing the grip of the rotary control
    [ObjectiveCClass]
    public class ViewRotaryGrip : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewRotaryGrip));

        public ViewRotaryGrip() : base() {}
        public ViewRotaryGrip(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect aFrameRect)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ThisClass, "initWithFrame:", aFrameRect);

            if (iImage == null)
            {
                iImage = new NSImageView();
                this.AddSubview(iImage);
            }

            // initialise the subviews
            NSRect frame = new NSRect(0, 0, aFrameRect.Width, aFrameRect.Height);

            iImage.InitWithFrame(frame);
            iImage.Image = Properties.Resources.ImageScrews;
            iImage.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iImage.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iImage.ImageScaling = NSImageScaling.NSImageScaleNone;

            return this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iImage.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        public float Angle
        {
            set { iImage.FrameCenterRotation = value; }
        }

        private NSImageView iImage;
    }


    // Class to handle rendering of the rotary control bar
    [ObjectiveCClass]
    public class ViewRotaryBar : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewRotaryBar));

        public ViewRotaryBar() : base() {}
        public ViewRotaryBar(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect aFrameRect)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ThisClass, "initWithFrame:", aFrameRect);

            return this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iText.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        public float Value
        {
            set
            {
                iValue = value;
                NeedsDisplay = true;
            }
        }

        public float MaxValue
        {
            set
            {
                iMaxValue = value;
                NeedsDisplay = true;
            }
        }

        public float PreviewValue
        {
            set
            {
                iPreviewValue = value;
                NeedsDisplay = true;
            }
        }

        public bool PreviewEnabled
        {
            set
            {
                iPreviewEnabled = value;
                NeedsDisplay = true;
            }
        }

        public string Text
        {
            set
            {
                iText.Release();
                iText = new NSString(value);
                NeedsDisplay = true;
            }
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            // draw bars
            if (iMaxValue > 0)
            {
                // draw the current value bar
                NSColor colour = NSColor.ColorWithCalibratedRedGreenBlueAlpha(71.0f/255.0f, 172.0f/255.0f, 220.0f/255.0f, 1.0f);
                colour.SetStroke();

                DrawArc(0, iValue, true);

                // draw the preview bar
                if (iPreviewEnabled)
                {
                    colour = NSColor.ColorWithCalibratedRedGreenBlueAlpha(187.0f/255.0f, 187.0f/255.0f, 0.0f/255.0f, 1.0f);
                    colour.SetStroke();

                    DrawArc(iValue, iPreviewValue, (iPreviewValue > iValue));
                }
            }

            // draw text
            if (iText != null)
            {
                NSMutableParagraphStyle style = new NSMutableParagraphStyle();
                style.SetParagraphStyle(NSParagraphStyle.DefaultParagraphStyle);
                style.SetAlignment(NSTextAlignment.NSCenterTextAlignment);

                NSDictionary dict = NSDictionary.DictionaryWithObjectsAndKeys(FontManager.FontMedium, NSAttributedString.NSFontAttributeName,
                                                                              style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                              NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                              null);
                style.Release();

                NSSize size = iText.SizeWithAttributes(dict);
                NSRect rect = new NSRect((Bounds.Width - size.width)*0.5f, (Bounds.Height - size.height)*0.5f, size.width, size.height);

                iText.DrawInRectWithAttributes(rect, dict);
            }
        }

        private void DrawArc(float aStartValue, float aEndValue, bool aClockwise)
        {
            // The angles for this path:
            //  0 => along +ve x-axis
            // 90 => along +ve y-axis
            // i.e. +ve angles go anti-clockwise
            NSBezierPath path = new NSBezierPath();
            path.LineWidth = iLineWidth;

            float startAngle = iZeroAngle - (360.0f * aStartValue / iMaxValue);
            float endAngle = iZeroAngle - (360.0f * aEndValue / iMaxValue);

            path.AppendBezierPathWithArcWithCenterRadiusStartAngleEndAngleClockwise(
                                        new NSPoint(Bounds.Width * 0.5f, Bounds.Height * 0.5f),
                                        iRadius, startAngle, endAngle, aClockwise);
            path.Stroke();
            path.Release();
        }

        private readonly float iRadius = 29.5f;
        private readonly float iZeroAngle = -90.0f;
        private readonly float iLineWidth = 3.0f;

        private float iValue = 0.0f;
        private float iMaxValue = 100.0f;
        private float iPreviewValue = 0.0f;
        private bool iPreviewEnabled = false;
        private NSString iText = new NSString(string.Empty);
    }
}




