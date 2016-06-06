
using System;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    public interface IViewPopover
    {
        NSView View { get; }
        void Release();
        event EventHandler<EventArgs> EventClose;
    }


    // File's owner class for the WindowPopover.xib file
    [ObjectiveCClass]
    public class WindowPopover : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(WindowPopover));

        public WindowPopover() : base() {}
        public WindowPopover(IntPtr aInstance) : base(aInstance) {}

        public WindowPopover(IViewPopover aView)
            : base()
        {
            iView = aView;
            bool ret = NSBundle.LoadNibNamedOwner("WindowPopover.nib", this);
            if (!ret)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 nib failed to load");
        }

        public void Show(NSPoint aAnchor, bool aAnchorOnLeft, NSSize aSize)
        {
            // calculate the rect for the window - the passed in arguments are "preferred" values
            // and will only be honoured if possible
            float x, y, w, h, anchorPos;
            bool anchorOnTop = false;

            if (NSScreen.MainScreen == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 MainScreen null");
            if (ViewBkgd == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 ViewBkgd null");
            if (Window == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 Window null");
            if (ViewContent == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 ViewContent null");
            if (iView == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 iView null");
            if (iView.View == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 iView.View null");
            if (iAnimation == null)
                Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #822 iAnimation null");

            NSRect screenFrame = NSScreen.MainScreen.VisibleFrame;

            // determine the y-pos, height and anchor position
            if (aAnchor.y - aSize.height > screenFrame.MinY)
            {
                // window can fit in available screen with given coords and anchor at the top
                h = aSize.height;
                y = aAnchor.y - aSize.height;
                anchorOnTop = true;
            }
            else if (aAnchor.y + aSize.height < screenFrame.MaxY)
            {
                // window can fit in available screen with given coords and anchor at the bottom
                h = aSize.height;
                y = aAnchor.y;
                anchorOnTop = false;
            }
            else if (aAnchor.y > screenFrame.MidY)
            {
                // window height is truncated with anchor at top
                h = aAnchor.y - screenFrame.MinY;
                y = screenFrame.MinY;
                anchorOnTop = true;
            }
            else
            {
                // window height is truncated with anchor at bottom
                h = screenFrame.MaxY - aAnchor.y;
                y = aAnchor.y;
                anchorOnTop = false;
            }

            // determine x-pos and width
            const float anchorIndent = 50.0f;

            if (aAnchorOnLeft)
            {
                float left = aAnchor.x - anchorIndent;
                float right = left + aSize.width;
                if (right < screenFrame.MaxX)
                {
                    // anchor ok on the left
                    x = left;
                    w = aSize.width;
                    anchorPos = anchorIndent;
                }
                else
                {
                    // move anchor to the right
                    x = aAnchor.x + anchorIndent - aSize.width;
                    w = aSize.width;
                    anchorPos = aSize.width - anchorIndent;
                }
            }
            else
            {
                float left = aAnchor.x + anchorIndent - aSize.width;
                if (left > screenFrame.MinX)
                {
                    // anchor ok on right
                    x = left;
                    w = aSize.width;
                    anchorPos = aSize.width - anchorIndent;
                }
                else
                {
                    // move anchor to the left
                    x = aAnchor.x - anchorIndent;
                    w = aSize.width;
                    anchorPos = anchorIndent;
                }
            }

            // set the target rect for the window
            ViewBkgd.AnchorOnTop = anchorOnTop;
            ViewBkgd.AnchorPos = anchorPos;

            NSRect targetFrame = new NSRect(x, y, w, h);
            NSRect initialFrame = GetMinimalRect(targetFrame);

            // initialise the content view to be the target size - this avoids adverse animation
            // effects where the view could potentially have a 0 or -ve height
            Window.SetFrameDisplay(targetFrame, false);
            iView.View.Frame = ViewContent.Bounds;

            // initialise and show the window
            Window.SetFrameDisplay(initialFrame, false);
            Window.MakeKeyAndOrderFront(this);

            // the popover closes when it resigns key window status. Normally, this would mean
            // it can no longer be seen (the main window is on top). Given that this window should
            // animate when closing, setting the window level will ensure that the popover is always
            // on top of the main window, so the closing animation can be seen
            Window.Level = NSWindowLevel.NSFloatingWindowLevel;

            // animate
            NSViewAnimationHelper.SetFrames(iAnimation, Window, initialFrame, targetFrame);
            iAnimation.Duration = 0.2;
            iAnimation.StartAnimation();
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            Window.Delegate = this;

            iAnimation = NSViewAnimationHelper.Create();

            ViewContent.AddSubview(iView.View);
            iView.EventClose += ViewClose;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iAnimation.Release();

            iView.View.RemoveFromSuperview();
            iView.EventClose -= ViewClose;
            iView.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("windowDidResignKey:")]
        public void WindowDidResignKey(NSNotification aNotification)
        {
            Close();
        }

        private void ViewClose(object sender, EventArgs e)
        {
            Close();
        }

        private void Close()
        {
            NSRect targetFrame = GetMinimalRect(Window.Frame);

            // animate
            NSViewAnimationHelper.SetFrames(iAnimation, Window, Window.Frame, targetFrame);
            iAnimation.StartAnimation();

            Window.Delegate = null;
            Window.Close();
            this.Autorelease();
        }

        private NSRect GetMinimalRect(NSRect aFullRect)
        {
            return new NSRect(aFullRect.origin.x,
                              ViewBkgd.AnchorOnTop ? aFullRect.origin.y + aFullRect.Height - ViewBkgd.MinHeight : aFullRect.origin.y,
                              aFullRect.Width,
                              ViewBkgd.MinHeight);
        }

        [ObjectiveCField]
        public WindowBorderless Window;

        [ObjectiveCField]
        public ViewPopoverBkgd ViewBkgd;

        [ObjectiveCField]
        public NSView ViewContent;

        private NSViewAnimation iAnimation;
        private IViewPopover iView;
    }


    // Background view class for the popover
    [ObjectiveCClass]
    public class ViewPopoverBkgd : NSView
    {
        public ViewPopoverBkgd() : base() {}
        public ViewPopoverBkgd(IntPtr aInstance) : base(aInstance) {}

        public float AnchorPos
        {
            set { iAnchorApex = value; }
        }

        public bool AnchorOnTop
        {
            get { return iAnchorOnTop; }
            set { iAnchorOnTop = value; }
        }

        public float MinHeight
        {
            get { return iPadding + iAnchorHeight + iCornerRadius + iCornerRadius + iAnchorHeight + iPadding; }
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            float left = Bounds.MinX + iPadding;
            float right = Bounds.MaxX - iPadding;
            float top = Bounds.MaxY - iPadding - iAnchorHeight;
            float bottom = Bounds.MinY + iPadding + iAnchorHeight;

            NSBezierPath path = new NSBezierPath();

            // start the path in the bottom left corner, just past the arc of the corner and go clockwise
            path.MoveToPoint(new NSPoint(left, bottom + iCornerRadius));
            path.LineToPoint(new NSPoint(left, top - iCornerRadius));
            path.AppendBezierPathWithArcFromPointToPointRadius(new NSPoint(left, top),
                                                               new NSPoint(left + iCornerRadius, top),
                                                               iCornerRadius);
            if (iAnchorOnTop)
            {
                path.LineToPoint(new NSPoint(iAnchorApex - iAnchorWidth*0.5f, top));
                path.LineToPoint(new NSPoint(iAnchorApex, top + iAnchorHeight));
                path.LineToPoint(new NSPoint(iAnchorApex + iAnchorWidth*0.5f, top));
            }
            path.LineToPoint(new NSPoint(right - iCornerRadius, top));
            path.AppendBezierPathWithArcFromPointToPointRadius(new NSPoint(right, top),
                                                               new NSPoint(right, top - iCornerRadius),
                                                               iCornerRadius);
            path.LineToPoint(new NSPoint(right, bottom + iCornerRadius));
            path.AppendBezierPathWithArcFromPointToPointRadius(new NSPoint(right, bottom),
                                                               new NSPoint(right - iCornerRadius, bottom),
                                                               iCornerRadius);
            if (!iAnchorOnTop)
            {
                path.LineToPoint(new NSPoint(iAnchorApex + iAnchorWidth*0.5f, bottom));
                path.LineToPoint(new NSPoint(iAnchorApex, bottom - iAnchorHeight));
                path.LineToPoint(new NSPoint(iAnchorApex - iAnchorWidth*0.5f, bottom));
            }
            path.LineToPoint(new NSPoint(left + iCornerRadius, bottom));
            path.AppendBezierPathWithArcFromPointToPointRadius(new NSPoint(left, bottom),
                                                               new NSPoint(left, bottom + iCornerRadius),
                                                               iCornerRadius);
            path.ClosePath();

            NSColor.WhiteColor.SetStroke();
            path.LineWidth = 4.0f;
            path.Stroke();

            NSColor.ColorWithCalibratedWhiteAlpha(0.0f, 0.9f).SetFill();
            path.Fill();

            path.Release();
        }

        private float iPadding = 5.0f;
        private float iCornerRadius = 10.0f;
        private float iAnchorWidth = 20.0f;
        private float iAnchorHeight = 20.0f * 0.866f;
        private float iAnchorApex = 50.0f;
        private bool iAnchorOnTop = true;
    }
}


