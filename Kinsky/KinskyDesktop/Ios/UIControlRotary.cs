using System;
using System.Threading;

using Foundation;
using UIKit;
using CoreGraphics;

using Linn;
using Linn.Kinsky;

namespace KinskyTouch
{
    internal interface IControlRotary
    {
        bool Dimmed { get; set; }
        ViewRotaryBar ViewBar { get; }

        event EventHandler<EventArgs> EventClicked;

        event EventHandler<EventArgs> EventStartRotation;
        event EventHandler<EventArgs> EventEndRotation;
        event EventHandler<EventArgs> EventCancelRotation;

        event EventHandler<EventArgs> EventRotateClockwise;
        event EventHandler<EventArgs> EventRotateAntiClockwise;
    }

    internal class ViewRotaryBar : UIView
    {
        public ViewRotaryBar(CGRect aRect)
            : base(aRect)
        {
            iFontSize = 15.0f;
            iFontColour = UIColor.White;

            BackgroundColor = UIColor.Clear;

            iInnerCircleRadius = 30.0f;
            iOuterCircleRadius = 35.0f;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if(!string.IsNullOrEmpty(iText))
            {
                NSString text = new NSString(iText);
                UIFont font = UIFont.SystemFontOfSize(iFontSize);
                CGSize size = text.StringSize(font);
                iFontColour.SetColor();
                text.DrawString(new CGRect(((Frame.Width * 0.5f) - 25) + 3, (Frame.Height - size.Height) * 0.5f, (25 * 2) - 3, size.Height),
                                font, UILineBreakMode.Clip, UITextAlignment.Center);
            }

            // render the % arc
            UIColor colour = new UIColor(71.0f / 255.0f, 172.0f / 255.0f, 220.0f / 255.0f, 255.0f / 255.0f);
            colour.SetStroke();

            UIBezierPath path = new UIBezierPath();
            path.LineWidth = 3.0f;
            if(iMaxValue != 0)
            {
                float angle = (float)((Math.PI * 0.5f) + ((iValue / (float)iMaxValue) * 2.0f * Math.PI));
                path.AddArc(new CGPoint(Frame.Width * 0.5f, Frame.Height * 0.5f), iInnerCircleRadius, (float)(Math.PI * 0.5f), angle, true);
                path.Stroke();
            }

            if(iPreviewEnabled)
            {
                // render the % preview arc
                colour = new UIColor(187.0f / 255.0f, 187.0f / 255.0f, 0.0f, 255.0f / 255.0f);
                colour.SetStroke();
    
                path = new UIBezierPath();
                path.LineWidth = 3.0f;
                if(MaxValue != 0)
                {
                    float angle = (float)((Math.PI * 0.5f) + ((Value / (float)MaxValue) * 2.0f * Math.PI));
                    float angleTarget = (float)((Math.PI * 0.5f) + ((iPreviewValue / (float)MaxValue) * 2.0f * Math.PI));
                    path.AddArc(new CGPoint(Frame.Width * 0.5f, Frame.Height * 0.5f), iInnerCircleRadius, angle, angleTarget, angle < angleTarget);
                    path.Stroke();
                }
            }
        }

        public float FontSize
        {
            get
            {
                return iFontSize;
            }
            set
            {
                iFontSize = value;
                SetNeedsDisplay();
            }
        }

        public UIColor FontColour
        {
            get
            {
                return iFontColour;
            }
            set
            {
                iFontColour = value;
                SetNeedsDisplay();
            }
        }

        public float InnerCircleRadius
        {
            get
            {
                return iInnerCircleRadius;
            }
            set
            {
                iInnerCircleRadius = value;
                SetNeedsDisplay();
            }
        }

        public float OuterCircleRadius
        {
            get
            {
                return iOuterCircleRadius;
            }
            set
            {
                iOuterCircleRadius = value;
                SetNeedsDisplay();
            }
        }

        public string Text
        {
            get
            {
                return iText;
            }
            set
            {
                iText = value;
                SetNeedsDisplay();
            }
        }

        public float Value
        {
            get
            {
                return iValue;
            }
            set
            {
                iValue = value;
                SetNeedsDisplay();
            }
        }

        public float MaxValue
        {
            get
            {
                return iMaxValue;
            }
            set
            {
                iMaxValue = value;
                SetNeedsDisplay();
            }
        }

        public float PreviewValue
        {
            get
            {
                return iPreviewValue;
            }
            set
            {
                iPreviewValue = value;
                SetNeedsDisplay();
            }
        }

        public bool PreviewEnabled
        {
            get
            {
                return iPreviewEnabled;
            }
            set
            {
                iPreviewEnabled = value;
                SetNeedsDisplay();
            }
        }

        private float iInnerCircleRadius;
        private float iOuterCircleRadius;

        private string iText;
        private UIColor iFontColour;
        private float iFontSize;

        private float iMaxValue;
        private float iValue;

        private bool iPreviewEnabled;
        private float iPreviewValue;
    }

    [Foundation.Register("UIControlWheel")]
    partial class UIControlWheel : UIControl
    {
        public UIControlWheel(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iViewBar = new ViewRotaryBar(Bounds);
            AddSubview(iViewBar);

            iViewDimmed = new UIImageView(Bounds);
            iViewDimmed.Image = KinskyTouch.Properties.ResourceManager.WheelMute;
            iViewDimmed.Alpha = 0.0f;
            AddSubview(iViewDimmed);
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            iImageWheel.Draw(new CGRect(0.0f, 0.0f, Frame.Width, Frame.Height), CGBlendMode.Normal, 1.0f);
        }

        public bool Dimmed
        {
            get
            {
                return iDimmed;
            }
            set
            {
                BeginInvokeOnMainThread(delegate {
                    UIView.BeginAnimations("dimmed");
                    UIView.SetAnimationDuration(0.15f);
                    iViewDimmed.Alpha = value ? 1.0f : 0.0f;
                    UIView.CommitAnimations();
                    iDimmed = value;
                });
            }
        }

        public ViewRotaryBar ViewBar
        {
            get
            {
                return iViewBar;
            }
        }

        public UIImage ImageWheel
        {
            set
            {
                iImageWheel = value;
            }
        }

        private bool iDimmed;

        private UIImage iImageWheel = KinskyTouch.Properties.ResourceManager.Wheel;

        private ViewRotaryBar iViewBar;
        private UIImageView iViewDimmed;
    }

    [Foundation.Register("UIControlRocker")]
    partial class UIControlRocker : UIControlWheel, IControlRotary
    {
        private enum ETouch
        {
            eNot,
            eLeft,
            eRight,
            eCentre
        }

        public UIControlRocker(IntPtr aInstance)
            : base(aInstance)
        {
            iTimer = new System.Threading.Timer(TimerUpdate);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iInnerCircle = 15.0f;

            iTouchBeganLocation = UIControlRocker.ETouch.eNot;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if(iTouchBeganLocation == UIControlRocker.ETouch.eLeft)
            {
                iImageOverLeft.Draw(new CGRect(0.0f, 0.0f, Frame.Width, Frame.Height), CGBlendMode.Normal, 1.0f);
            }
            if(iTouchBeganLocation == UIControlRocker.ETouch.eRight)
            {
                iImageOverRight.Draw(new CGRect(0.0f, 0.0f, Frame.Width, Frame.Height), CGBlendMode.Normal, 1.0f);
            }

			iImageGrip.Draw(new CGRect(0.0f, 0.0f, Frame.Width, Frame.Height), CGBlendMode.Normal, 1.0f);

            /*UIColor colour = new UIColor(255.0f / 255.0f, 255.0f / 255.0f, 225.0f / 255.0f, 255.0f / 255.0f);
            colour.SetStroke();

            UIBezierPath path = new UIBezierPath();
            path.LineWidth = 1.0f;
            path.AddArc(new PointF(Frame.Width * 0.5f, Frame.Height * 0.5f), iInnerCircle, 0.0f, (float)(2.0f * Math.PI), true);
            path.Stroke();*/
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;
            iTouchBeganLocation = TouchLocation(touch);

            if(iTouchBeganLocation == UIControlRocker.ETouch.eLeft || iTouchBeganLocation == UIControlRocker.ETouch.eRight)
            {
                if(EventStartRotation != null)
                {
                    EventStartRotation(this, EventArgs.Empty);
                }

                DoStep();

                //PerformSelector(new MonoTouch.ObjCRuntime.Selector("TimerUpdate"), null, kAutoRepeatDelay);
                iTimer.Change((int)(kAutoRepeatDelay * 1000.0f), Timeout.Infinite);
                SetNeedsDisplay();
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            try
            {
                base.TouchesEnded(touches, evt);

                UITouch touch = touches.AnyObject as UITouch;
                UIControlRocker.ETouch touchLocation = TouchLocation(touch);
    
                switch(iTouchBeganLocation)
                {
                case ETouch.eCentre:
                    if(touchLocation == UIControlRocker.ETouch.eCentre)
                    {
                        if(EventClicked != null)
                        {
                            EventClicked(this, EventArgs.Empty);
                        }
                    }
                    break;
    
                case ETouch.eLeft:
                case ETouch.eRight:
                    if(touchLocation != UIControlRocker.ETouch.eNot)
                    {
                        if(EventEndRotation != null)
                        {
                            EventEndRotation(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if(EventCancelRotation != null)
                        {
                            EventCancelRotation(this, EventArgs.Empty);
                        }
                    }
                    break;
                }
            }
            finally
            {
                iTouchBeganLocation = UIControlRocker.ETouch.eNot;
            }

            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
            SetNeedsDisplay();
        }

        public float RepeatInterval
        {
            set
            {
                iRepeatInterval = value;
            }
        }

        public event EventHandler<EventArgs> EventClicked;

        public event EventHandler<EventArgs> EventStartRotation;
        public event EventHandler<EventArgs> EventEndRotation;
        public event EventHandler<EventArgs> EventCancelRotation;

        public event EventHandler<EventArgs> EventRotateClockwise;
        public event EventHandler<EventArgs> EventRotateAntiClockwise;

        private ETouch TouchLocation(UITouch aTouch)
        {
			CGPoint point = aTouch.LocationInView(this);

            if(Bounds.Contains(point))
            {
                nfloat x = point.X - (Frame.Width * 0.5f);
                nfloat y = point.Y - (Frame.Height * 0.5f);
                nfloat distSquare = (x * x) + (y * y);

                if(distSquare < iInnerCircle * iInnerCircle)
                {
                    return UIControlRocker.ETouch.eCentre;
                }
                else
                {
                    if(x < 0)
                    {
                        return UIControlRocker.ETouch.eLeft;
                    }
                    else
                    {
                        return UIControlRocker.ETouch.eRight;
                    }
                }
            }
            else
            {
                return UIControlRocker.ETouch.eNot;
            }
        }

        private void DoStep()
        {
            if(iTouchBeganLocation == UIControlRocker.ETouch.eLeft)
            {
                if(EventRotateAntiClockwise != null)
                {
                    EventRotateAntiClockwise(this, EventArgs.Empty);
                }
            }
            else if(iTouchBeganLocation == UIControlRocker.ETouch.eRight)
            {
                if(EventRotateClockwise != null)
                {
                    EventRotateClockwise(this, EventArgs.Empty);
                }
            }
        }

        //[Export("TimerUpdate")]
        private void TimerUpdate(object aObject)
        {
            BeginInvokeOnMainThread(delegate {
                if(Enabled && (iTouchBeganLocation == UIControlRocker.ETouch.eLeft || iTouchBeganLocation == UIControlRocker.ETouch.eRight))
                {
                    DoStep();
                    //PerformSelector(new MonoTouch.ObjCRuntime.Selector("TimerUpdate"), null, iRepeatInterval);
                    iTimer.Change((int)(iRepeatInterval * 1000.0f), Timeout.Infinite);
                }
            });
        }

        private const float kAutoRepeatDelay = 0.25f;

        private static UIImage iImageOverLeft = new UIImage("RockerLeftDown.png");
        private static UIImage iImageOverRight = new UIImage("RockerRightDown.png");
        private static UIImage iImageGrip = new UIImage("Rocker.png");

        private float iRepeatInterval;
        private float iInnerCircle;

        private ETouch iTouchBeganLocation;
        private System.Threading.Timer iTimer;
    }

    [Foundation.Register("UIControlRotary")]
    partial class UIControlRotary : UIControlWheel, IControlRotary
    {
        public UIControlRotary(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iAngle = 270.0f;
            iInnerRadius = Center.X * 0.3f;
            iOuterRadius = Centre.X;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if((iTouchedInner && !iOutsideInner) || (iTouchedOuter && !iOutsideOuter))
            {
                if(iImageOver != null)
                {
					iImageOver.Draw(new CGRect(0.0f, 0.0f, Frame.Width, Frame.Height), CGBlendMode.Normal, 1.0f);
                }
            }

            CGContext c = UIGraphics.GetCurrentContext();

            c.SaveState();

            c.TranslateCTM(Frame.Width * 0.5f, Frame.Height * 0.5f);
            c.RotateCTM((float)(iAngle) * 0.01745329f);
            c.TranslateCTM(-Frame.Width * 0.5f, -Frame.Height * 0.5f);

			iImageGrip.Draw(new CGRect(0.0f, 0.0f, Frame.Width, Frame.Height), CGBlendMode.Normal, 1.0f);

            c.RestoreState();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            iTouchedInner = false;
            iOutsideInner = false;

            iTouchedOuter = false;
            iOutsideOuter = false;
            iRotating = false;

            UITouch touch = touches.AnyObject as UITouch;
            CGPoint point = touch.LocationInView(this);

            nfloat dx = point.X - Centre.X;
            nfloat dy = point.Y - Centre.Y;
            nfloat r2 = dx*dx + dy*dy;

            if(r2 > iInnerRadius * iInnerRadius)
            {
                iTouchedOuter = true;
                iRotating = true;

                if(EventStartRotation != null)
                {
                    EventStartRotation(this, EventArgs.Empty);
                }
            }
            else
            {
                iTouchedInner = true;
            }


            iTouchDownAngle = CalculateMouseAngle(point);
            iLastTouchAngle = iTouchDownAngle;

            SetNeedsDisplay();
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;
            CGPoint point = touch.LocationInView(this);

            nfloat dx = point.X - Centre.X;
            nfloat dy = point.Y - Centre.Y;
            nfloat r2 = dx*dx + dy*dy;

            iOutsideOuter = (r2 > iOuterRadius * iOuterRadius);
            iOutsideInner = (r2 > iInnerRadius * iInnerRadius);

            if(iTouchedOuter /*&& iOutsideInner*/)
            {
                UpdateAngle(point);
            }

            SetNeedsDisplay();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if(iRotating)
            {
                if(iOutsideOuter)
                {
                    if(EventCancelRotation != null)
                    {
                        EventCancelRotation(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if(EventEndRotation != null)
                    {
                        EventEndRotation(this, EventArgs.Empty);
                    }
                }
            }
            else
            {
                if(!iOutsideInner)
                {
                    if(EventClicked != null)
                    {
                        EventClicked(this, EventArgs.Empty);
                    }
                }
            }

            UITouch touch = touches.AnyObject as UITouch;
            CGPoint point = touch.LocationInView(this);

            // get the current mouse angle
            float angle = CalculateMouseAngle(point);

            // set the knob angle
            iKnobAngle += angle - iTouchDownAngle;

            iTouchedInner = false;
            iOutsideInner = false;

            iTouchedOuter = false;
            iOutsideOuter = false;
            iRotating = false;

            SetNeedsDisplay();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;
            CGPoint point = touch.LocationInView(this);

            // get the current mouse angle
            float angle = CalculateMouseAngle(point);

            // set the knob angle
            iKnobAngle += angle - iTouchDownAngle;

            if(iRotating)
            {
                if(EventCancelRotation != null)
                {
                    EventCancelRotation(this, EventArgs.Empty);
                }
            }

            iRotating = false;
            iOutsideOuter = false;
            iTouchedOuter = false;

            iOutsideInner = false;
            iTouchedInner = false;

            SetNeedsDisplay();
        }

        public event EventHandler<EventArgs> EventClicked;

        public event EventHandler<EventArgs> EventStartRotation;
        public event EventHandler<EventArgs> EventEndRotation;
        public event EventHandler<EventArgs> EventCancelRotation;

        public event EventHandler<EventArgs> EventRotateClockwise;
        public event EventHandler<EventArgs> EventRotateAntiClockwise;

        public UIImage ImageWheelOver
        {
            set
            {
                iImageOver = value;
            }
        }

        public UIImage ImageGrip
        {
            set
            {
                iImageGrip = value;
            }
        }

        private void UpdateAngle(CGPoint aPoint)
        {
            // get the current mouse angle
            float angle = CalculateMouseAngle(aPoint);

            // set the current angle for the knob
            iAngle = iKnobAngle + angle - iTouchDownAngle;

            // calculate the change in angle and make sure it is in the range [-180, 180]
            float angleDiff = angle - iLastTouchAngle;
            while (angleDiff > 180.0f)
                angleDiff -= 360.0f;
            while (angleDiff < -180.0f)
                angleDiff += 360.0f;

            // the rotation is split into segments - calculate whether or not the rotation has moved
            // from 1 segment to another
            // note the use of (iLastMouseAngle + angleDiff) instead of mouseAngle - this avoids the situation
            // where lastSegment=0 and thisSegment=11, which is now interpreted as lastSegment=0 and thisSegment=-1,
            // which gives us the correct behaviour
            int lastSegment = (int)Math.Floor((iLastTouchAngle - iTouchDownAngle) / 30.0f);
            int thisSegment = (int)Math.Floor((iLastTouchAngle + angleDiff - iTouchDownAngle) / 30.0f);

            /*if(!iRotating && ((thisSegment > lastSegment) || (thisSegment < lastSegment)))
            {
                iRotating = true;

                if(EventStartRotation != null)
                {
                    EventStartRotation(this, EventArgs.Empty);
                }
            }*/

            if (thisSegment > lastSegment)
            {
                if (EventRotateClockwise != null)
                {
                    EventRotateClockwise(this, EventArgs.Empty);
                }
            }
            else if (thisSegment < lastSegment)
            {
                if (EventRotateAntiClockwise != null)
                {
                    EventRotateAntiClockwise(this, EventArgs.Empty);
                }
            }

            iLastTouchAngle = angle;
        }

        private float CalculateMouseAngle(CGPoint aPtInView)
        {
            double radians = Math.Atan2(aPtInView.Y - Centre.Y, aPtInView.X - Centre.X);

            return (float)(radians * 180.0 / Math.PI);
        }

        private CGPoint Centre
        {
            get
            {
                return new CGPoint(Frame.Width * 0.5f, Frame.Height * 0.5f);
            }
        }

        private UIImage iImageOver = new UIImage("WheelOver.png");
        private UIImage iImageGrip = new UIImage("Screws.png");

        private bool iTouchedInner;
        private bool iOutsideInner;

        private bool iTouchedOuter;
        private bool iOutsideOuter;
        private bool iRotating;

        private nfloat iInnerRadius;
        private nfloat iOuterRadius;

        private float iAngle;
        private float iKnobAngle;
        private float iTouchDownAngle;
        private float iLastTouchAngle;
    }

    partial class ViewHourGlass : UIView
    {
        public ViewHourGlass()
        {
            iTicker = new Ticker();
            iTimer = new System.Threading.Timer(TimerElapsed);
            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public ViewHourGlass(IntPtr aInstance)
            : base(aInstance)
        {
            iTicker = new Ticker();
            iTimer = new System.Threading.Timer(TimerElapsed);
            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            iAngleRemainder = 0.0f;
            iAngle = 0.0f;
            iTicker.Reset();
            iTimer.Change(kUpdateRate, kUpdateRate);
            iStarted = true;

            SetNeedsDisplay();
        }

        public void Stop()
        {
            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
            iStarted = false;

            SetNeedsDisplay();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iStarted = false;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if(iStarted)
            {
                iImageBuffering.Draw(new CGPoint((Frame.Width - iImageBuffering.Size.Width) * 0.5f, (Frame.Height - iImageBuffering.Size.Height) * 0.5f), CGBlendMode.Normal, 1.0f);
    
                CGContext c = UIGraphics.GetCurrentContext();
    
                c.SaveState();
    
                c.TranslateCTM(Frame.Width * 0.5f, Frame.Height * 0.5f);
                c.RotateCTM(iAngle * 0.01745329f);
                c.TranslateCTM(-Frame.Width * 0.5f, -Frame.Height * 0.5f);
    
                iImageBufferingElement.Draw(new CGPoint((Frame.Width - iImageBufferingElement.Size.Width) * 0.5f, (Frame.Height - iImageBufferingElement.Size.Height) * 0.5f), CGBlendMode.Normal, 1.0f);
    
                c.RestoreState();
            }
        }

        private void TimerElapsed(object sender)
        {
            iAngleRemainder += ((360 * kRevolutionsPerSecond) / 1000.0f) * iTicker.MilliSeconds;
            float angle = ((int)(iAngleRemainder / 45.0f)) * 45.0f;
            iAngleRemainder -= angle;
            iAngle += angle;
            if(iAngle > 360.0f)
            {
                iAngle -= 360.0f;
            }
            iTicker.Reset();

            BeginInvokeOnMainThread(delegate {
                SetNeedsDisplay();
            });
        }

        private static UIImage iImageBuffering = new UIImage("HourGlass.png");
        private static UIImage iImageBufferingElement = new UIImage("HourGlass2.png");

        private const uint kUpdateRate = 20;
        private const float kRevolutionsPerSecond = 1.5f;

        private bool iStarted;

        private System.Threading.Timer iTimer;
        private Ticker iTicker;
        private float iAngle;
        private float iAngleRemainder;
    }

    partial class ViewHourGlassIpad : ViewHourGlass
    {
        public ViewHourGlassIpad(IntPtr aInstance)
            : base(aInstance)
        {
        }
    }

    partial class ViewHourGlassIphone : ViewHourGlass
    {
        public ViewHourGlassIphone(IntPtr aInstance)
            : base(aInstance)
        {
        }
    }
}