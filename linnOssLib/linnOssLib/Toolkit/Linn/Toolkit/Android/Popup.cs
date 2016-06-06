using Android.Content;
using Android.Views;
using Android.Graphics;
using Android.Runtime;
using Android.Widget;
using Android.Graphics.Drawables;
using Linn;
using System;

namespace OssToolkitDroid
{

    public interface IPopup
    {
        void Show();
        void Dismiss();
        event EventHandler<EventArgs> EventDismissed;
    }

    public static class PopupManager
    {
        public static bool IsShowingPopup
        {
            get
            {
                lock (iLock)
                {
                    return iCount != 0;
                }
            }
        }
        public static void Add(IPopup aPopup)
        {
            lock (iLock)
            {
                iCount++;
            }
        }
        public static void Remove(IPopup aPopup)
        {
            lock (iLock)
            {
                iCount--;
            }
        }
        private static int iCount;
        private static object iLock = new object();
    }

    public abstract class PopupBase : IPopup
    {
        public PopupBase(Context aContext, View aViewRoot, View aAnchor)
        {
            iContext = aContext;
            iViewRoot = aViewRoot;
            iAnchor = aAnchor;
        }

        public event EventHandler<EventArgs> EventDismissed;

        public abstract PopupContent GetContent();

        public virtual void OnDismissed() { }

        public void Show()
        {
            PopupManager.Add(this);
            PopupContent contents = GetContent();

            iPopup = new PopupWindow(contents.Content);
            iPopup.SetBackgroundDrawable(new BitmapDrawable());
            iPopup.Width = contents.Width;
            iPopup.Height = contents.Height;
            iPopup.Touchable = true;
            iPopup.Focusable = true;
            iPopup.OutsideTouchable = true;
            iPopup.DismissEvent += DismissEventHandler;
            iPopup.ShowAtLocation(iAnchor, (int)Android.Views.GravityFlags.NoGravity, contents.X, contents.Y);
        }

        private void DismissEventHandler(object sender, EventArgs e)
        {
            if (iPopup != null)
            {
                iPopup.DismissEvent -= DismissEventHandler;
                OnEventDismissed(this, EventArgs.Empty);
                PopupManager.Remove(this);
                (iPopup.ContentView as ViewGroup).RemoveAllViews();
                iPopup.ContentView.Dispose();
                iPopup.Dispose();
                iPopup = null;
                iContext = null;
                iViewRoot = null;
                iAnchor = null;
            }
        }

        private void OnEventDismissed(object sender, EventArgs e)
        {
            EventHandler<EventArgs> dismissed = this.EventDismissed;
            if (dismissed != null)
            {
                dismissed(sender, e);
            }
        }

        public void Dismiss()
        {
            Assert.Check(iPopup != null);
            iPopup.Dismiss();
        }

        private PopupWindow iPopup;
        protected Context iContext;
        protected View iViewRoot;
        protected View iAnchor;
    }

    public class PopupContent
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public View Content { get; set; }
    }

    public class OverlayPopup : PopupBase
    {
        public OverlayPopup(Context aContext, View aViewRoot, View aAnchor, Color aBackground)
            : base(aContext, aViewRoot, aAnchor)
        {
            iBackground = aBackground;
        }

        public override PopupContent GetContent()
        {
            RelativeLayout result = new RelativeLayout(iContext);
            //result.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
            result.SetBackgroundColor(iBackground);
            iViewRoot.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            (iViewRoot.LayoutParameters as RelativeLayout.LayoutParams).AddRule(LayoutRules.CenterInParent);
            result.AddView(iViewRoot);
            int[] location = new int[2];
            iAnchor.GetLocationOnScreen(location);
            return new PopupContent() { Content = result, Height = iAnchor.Height, Width = iAnchor.Width, X = location[0], Y = location[1] };
        }
        private Color iBackground;
    }

    public class SpeechBubblePopup : PopupBase
    {

        public SpeechBubblePopup(Context aContext, View aViewRoot, View aAnchor, Paint aStroke, Paint aFill)
            : base(aContext, aViewRoot, aAnchor)
        {
            iStroke = aStroke;
            iFill = aFill;
        }

        public int Width { get; set; }
        public bool StretchVertical { get; set; }

        public override void OnDismissed()
        {
            iStroke.Dispose();
            iStroke = null;
            iFill.Dispose();
            iFill = null;

        }

        public override PopupContent GetContent()
        {
            IWindowManager windowManager = iContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            int screenWidth = windowManager.DefaultDisplay.Width - (kPadding * 2);
            int screenHeight = windowManager.DefaultDisplay.Height - (kPadding * 2);

            int[] location = new int[2];
            iAnchor.GetLocationOnScreen(location);

            PopupFrame frame = new PopupFrame(iContext, kPadding, kAnchorHeight, kAnchorWidth, kCornerRadius, iStroke, iFill);
            frame.AddView(iViewRoot);

            int left = location[0], top = location[1], right = location[0] + iAnchor.Width, bottom = location[1] + iAnchor.Height;

            int anchorCenterY = top + ((bottom - top) / 2);

            bool anchorTop = (anchorCenterY <= screenHeight / 2);

            frame.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);

            int height = 0, width = 0;
            int constrainedHeight = anchorTop ? screenHeight + kPadding - bottom : top - kPadding;
            if (StretchVertical)
            {
                height = constrainedHeight;
            }
            else
            {
                height = Math.Min(frame.MeasuredHeight, constrainedHeight);
            }
            int constrainedWidth = screenWidth - (kPadding * 2);
            width = Width != 0 ? Math.Min(Width, constrainedWidth) : Math.Min(frame.MeasuredWidth, constrainedWidth);


            int xPos = (left + ((right - left) / 2)) - (width / 2);

            int anchorOffset = xPos < kPadding ? kPadding - xPos : xPos + width > screenWidth - kPadding ? screenWidth - (xPos + width) + kPadding : 0;
            xPos += anchorOffset;

            int yPos = anchorTop ? bottom : top - height;

            frame.AnchorOffset = anchorOffset;
            frame.AnchorOnTop = anchorTop;

            return new PopupContent() { Content = frame, Height = height, Width = width, X = xPos, Y = yPos };
        }

        private const int kAnchorHeight = 20;
        private const int kAnchorWidth = 20;
        private const int kPadding = 10;
        private const int kCornerRadius = 25;
        private Paint iStroke, iFill;
    }

    public class PopupFrame : LinearLayout
    {
        public PopupFrame(Context aContext, int aPadding, int aAnchorHeight, int aAnchorWidth, int aCornerRadius, Paint aStroke, Paint aFill)
            : base(aContext)
        {
            iPadding = aPadding;
            iAnchorHeight = aAnchorHeight;
            iAnchorWidth = aAnchorWidth;
            iCornerRadius = aCornerRadius;
            iStroke = aStroke;
            iFill = aFill;
            SetWillNotDraw(false);
            this.SetPadding(iPadding, iPadding + iAnchorHeight, iPadding, iPadding);
        }

        public bool AnchorOnTop
        {
            set
            {
                iAnchorOnTop = value;
                if (value)
                {
                    this.SetPadding(iPadding, iPadding + iAnchorHeight, iPadding, iPadding);
                }
                else
                {
                    this.SetPadding(iPadding, iPadding, iPadding, iPadding + iAnchorHeight);
                }
                Invalidate();
            }
        }

        public int AnchorOffset
        {
            set
            {
                iAnchorOffset = value;
            }
        }

        protected void Initialise(Canvas aCanvas)
        {
            int left = (int)iStroke.StrokeWidth;
            int right = aCanvas.Width - (int)iStroke.StrokeWidth;
            int top = iAnchorOnTop ? iAnchorHeight : (int)iStroke.StrokeWidth;
            int bottom = iAnchorOnTop ? aCanvas.Height - (int)iStroke.StrokeWidth : aCanvas.Height - iAnchorHeight - (int)iStroke.StrokeWidth;
            int anchorApex = (int)((right - left) * 0.5) - iAnchorOffset;
            if (anchorApex < left + iPadding + iCornerRadius)
            {
                anchorApex = left + iPadding + iCornerRadius;
            }
            else if (anchorApex + iPadding + iCornerRadius > right)
            {
                anchorApex = right - iPadding - iCornerRadius;
            }

            iPath = new Path();
            iPath.MoveTo(left, bottom - iCornerRadius);
            iPath.LineTo(left, top + iCornerRadius);
            RectF rectf = new RectF(left, top, left + iCornerRadius, top + iCornerRadius);
            iPath.ArcTo(rectf, 180, 90);
            rectf.Dispose();

            if (iAnchorOnTop)
            {
                iPath.LineTo((float)(anchorApex - iAnchorWidth * 0.5), top);
                iPath.LineTo(anchorApex, top - iAnchorHeight);
                iPath.LineTo((float)(anchorApex + iAnchorWidth * 0.5), top);
            }

            iPath.LineTo(right - iCornerRadius, top);
            rectf = new RectF(right - iCornerRadius, top, right, top + iCornerRadius);
            iPath.ArcTo(rectf, 270, 90);
            rectf.Dispose();

            iPath.LineTo(right, bottom - iCornerRadius);
            rectf = new RectF(right - iCornerRadius, bottom - iCornerRadius, right, bottom);
            iPath.ArcTo(rectf, 0, 90);
            rectf.Dispose();
            if (!iAnchorOnTop)
            {
                iPath.LineTo((float)(anchorApex + iAnchorWidth * 0.5), bottom);
                iPath.LineTo(anchorApex, bottom + iAnchorHeight);
                iPath.LineTo((float)(anchorApex - iAnchorWidth * 0.5), bottom);
            }
            iPath.LineTo(left + iCornerRadius, bottom);
            rectf = new RectF(left, bottom - iCornerRadius, left + iCornerRadius, bottom);
            iPath.ArcTo(rectf, 90, 90);
            rectf.Dispose();
            rectf = null;
            iPath.Close();
            iInitialised = true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (iWidth != canvas.Width || iHeight != canvas.Height || !iInitialised)
            {
                Initialise(canvas);
                iWidth = canvas.Width;
                iHeight = canvas.Height;
            }

            canvas.DrawPath(iPath, iFill);
            canvas.DrawPath(iPath, iStroke);
        }

        private int iAnchorOffset;
        private bool iAnchorOnTop;
        private int iPadding;
        private int iAnchorHeight;
        private int iAnchorWidth;
        private int iCornerRadius;
        private Paint iStroke;
        private Paint iFill;
        private Path iPath;
        private int iWidth, iHeight;
        private bool iInitialised;
    }
}