using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
namespace KinskyDesktopWpf
{
    public partial class WindowChrome : UserControl
    {

        public event EventHandler MiniModeActiveChanged;

        private const int kDragHitBoxSize = 10;
        private EResizeMode iCurrentResizeMode;
        private Point iMouseDownLocation;
        private Rect iStartRect;
        private Rect iLastResizeRect;

        public WindowChrome()
            : base()
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(WindowChrome_MouseLeftButtonDown);
            this.MouseLeave += new MouseEventHandler(WindowChrome_MouseLeave);
            this.PreviewMouseMove += new MouseEventHandler(WindowChrome_PreviewMouseMove);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(WindowChrome_PreviewMouseLeftButtonUp);
        }

        void WindowChrome_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (iCurrentResizeMode != EResizeMode.NONE)
            {
                //WindowChrome chrome = (sender as FrameworkElement).FindVisualAncestor<WindowChrome>(); 
                //chrome.OnWindowResized(); 
            }
            iCurrentResizeMode = EResizeMode.NONE;
            (sender as FrameworkElement).ReleaseMouseCapture();
        }

        void WindowChrome_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point current = e.GetPosition(sender as FrameworkElement);
            Window w = Window.GetWindow(sender as DependencyObject);
            EResizeMode mode = GetResizeMode(current, w);
            if (mode != EResizeMode.NONE || iCurrentResizeMode != EResizeMode.NONE)
            {
                if (iCurrentResizeMode != EResizeMode.NONE)
                {
                    mode = iCurrentResizeMode;
                }
                switch (mode)
                {
                    case EResizeMode.BOTTOM:
                    case EResizeMode.TOP:
                        {
                            (sender as FrameworkElement).Cursor = Cursors.SizeNS;
                            break;
                        }
                    case EResizeMode.LEFT:
                    case EResizeMode.RIGHT:
                        {
                            (sender as FrameworkElement).Cursor = Cursors.SizeWE;
                            break;
                        }
                    case EResizeMode.TOPLEFT:
                    case EResizeMode.BOTTOMRIGHT:
                        {
                            (sender as FrameworkElement).Cursor = Cursors.SizeNWSE;
                            break;
                        }
                    case EResizeMode.TOPRIGHT:
                    case EResizeMode.BOTTOMLEFT:
                        {
                            (sender as FrameworkElement).Cursor = Cursors.SizeNESW;
                            break;
                        }
                    default:
                        break;
                }

            }
            else
            {
                (sender as FrameworkElement).ClearValue(FrameworkElement.CursorProperty);
            }

            if (iCurrentResizeMode != EResizeMode.NONE)
            {
                Point delta = new Point(current.X - iMouseDownLocation.X, current.Y - iMouseDownLocation.Y);
                double newWidth = Math.Max(iStartRect.Width + delta.X, w.MinWidth);
                if (newWidth < 0) newWidth = 0;
                if (iCurrentResizeMode != EResizeMode.TOP
                    && iCurrentResizeMode != EResizeMode.BOTTOM)
                {
                    if (iCurrentResizeMode == EResizeMode.LEFT || iCurrentResizeMode == EResizeMode.TOPLEFT || iCurrentResizeMode == EResizeMode.BOTTOMLEFT)
                    {
                        double adjustment = iStartRect.Left - w.Left;
                        newWidth = Math.Max(w.MinWidth, newWidth + adjustment);
                        w.Left = w.PointToScreen(current).X;
                        w.Width = newWidth;
                    }
                    else
                    {
                        w.Width = newWidth;
                    }
                }
                double newHeight = Math.Max(iStartRect.Height + delta.Y, w.MinHeight);
                if (newHeight < 0) newHeight = 0;
                if (iCurrentResizeMode != EResizeMode.LEFT
                && iCurrentResizeMode != EResizeMode.RIGHT)
                {
                    if (iCurrentResizeMode == EResizeMode.TOP || iCurrentResizeMode == EResizeMode.TOPLEFT || iCurrentResizeMode == EResizeMode.TOPRIGHT)
                    {
                        double adjustment = iStartRect.Top - w.Top;
                        newHeight = Math.Max(w.MinHeight, newHeight + adjustment);
                        w.Top = w.PointToScreen(current).Y;
                        w.Height = newHeight;
                    }
                    else
                    {
                        w.Height = newHeight;
                    }
                }
                iLastResizeRect = new Rect(w.Left, w.Top, w.ActualWidth, w.ActualHeight);
            }
        }

        void WindowChrome_MouseLeave(object sender, MouseEventArgs e)
        {
            iCurrentResizeMode = EResizeMode.NONE;
            (sender as FrameworkElement).ReleaseMouseCapture();
        }

        void WindowChrome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point current = e.GetPosition(sender as FrameworkElement);
            Window w = Window.GetWindow(sender as DependencyObject);
            EResizeMode mode = GetResizeMode(current, w);
            if (mode != EResizeMode.NONE)
            {
                iCurrentResizeMode = mode;
                iMouseDownLocation = current;

                iStartRect = new Rect(w.Left, w.Top, w.ActualWidth, w.ActualHeight);
                iLastResizeRect = iStartRect;
                e.Handled = true;
                (sender as FrameworkElement).CaptureMouse();
            }
            else
            {
                try
                {
                    Window.GetWindow(sender as DependencyObject).DragMove();
                }
                catch (InvalidOperationException ex)
                {
                    Linn.UserLog.WriteLine("InvalidOperationException caught in DragMove()" + ex);
                }
            }
        }

        private EResizeMode GetResizeMode(Point aPoint, Window aWindow)
        {
            if (aWindow.ResizeMode == ResizeMode.NoResize || aWindow.WindowState == WindowState.Maximized)
            {
                return EResizeMode.NONE;
            }
            EResizeMode resizeType = EResizeMode.NONE;
            if (aPoint.X < kDragHitBoxSize)
            {
                if (aPoint.Y < kDragHitBoxSize)
                {
                    resizeType = EResizeMode.TOPLEFT;
                }
                else if (aPoint.Y >= aWindow.ActualHeight - kDragHitBoxSize)
                {
                    resizeType = EResizeMode.BOTTOMLEFT;
                }
                else
                {
                    resizeType = EResizeMode.LEFT;
                }
            }
            else if (aPoint.X >= aWindow.ActualWidth - kDragHitBoxSize)
            {
                if (aPoint.Y < kDragHitBoxSize)
                {
                    resizeType = EResizeMode.TOPRIGHT;
                }
                else if (aPoint.Y >= aWindow.ActualHeight - kDragHitBoxSize)
                {
                    resizeType = EResizeMode.BOTTOMRIGHT;
                }
                else
                {
                    resizeType = EResizeMode.RIGHT;
                }
            }
            else if (aPoint.Y < kDragHitBoxSize)
            {
                resizeType = EResizeMode.TOP;
            }
            else if (aPoint.Y >= aWindow.ActualHeight - kDragHitBoxSize)
            {
                resizeType = EResizeMode.BOTTOM;
            }
            if (resizeType != EResizeMode.NONE && GetIsMiniModeActive(this))
            {
                if (resizeType != EResizeMode.RIGHT && resizeType != EResizeMode.LEFT)
                {
                    resizeType = EResizeMode.NONE;
                }
            }
            return resizeType;
        }

        private enum EResizeMode
        {
            NONE,
            TOPLEFT,
            BOTTOMLEFT,
            LEFT,
            TOPRIGHT,
            BOTTOMRIGHT,
            RIGHT,
            TOP,
            BOTTOM
        }


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(WindowChrome), new UIPropertyMetadata());


        public static bool GetShowMaximiseAndRestoreButtons(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowMaximiseAndRestoreButtonsProperty);
        }

        public static void SetShowMaximiseAndRestoreButtons(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowMaximiseAndRestoreButtonsProperty, value);
        }

        public static readonly DependencyProperty ShowMaximiseAndRestoreButtonsProperty =
            DependencyProperty.RegisterAttached("ShowMaximiseAndRestoreButtons", typeof(bool), typeof(WindowChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));



        public bool HideButtons
        {
            get { return (bool)GetValue(HideButtonsProperty); }
            set { SetValue(HideButtonsProperty, value); }
        }

        public static readonly DependencyProperty HideButtonsProperty =
            DependencyProperty.Register("HideButtons", typeof(bool), typeof(WindowChrome), new UIPropertyMetadata(false));



        public static bool GetIsMiniModeEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMiniModeEnabledProperty);
        }

        public static void SetIsMiniModeEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMiniModeEnabledProperty, value);
        }

        public static readonly DependencyProperty IsMiniModeEnabledProperty =
            DependencyProperty.RegisterAttached("IsMiniModeEnabled", typeof(bool), typeof(WindowChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));



        public static bool GetIsMiniModeActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMiniModeActiveProperty);
        }

        public static void SetIsMiniModeActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMiniModeActiveProperty, value);
        }

        public static readonly DependencyProperty IsMiniModeActiveProperty =
            DependencyProperty.RegisterAttached("IsMiniModeActive", typeof(bool), typeof(WindowChrome), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, (d, e) =>
            {
                WindowChrome c = (d as WindowChrome);
                if (c != null)
                {
                    c.OnMiniModeActiveChanged();
                }
            }));



        public void OnMiniModeActiveChanged()
        {
            if (MiniModeActiveChanged != null)
            {
                MiniModeActiveChanged(this, EventArgs.Empty);
            }
        }

        public bool IsAnimating { get; set; }

    }

    public partial class WindowChromeButtons : UserControl
    {
        public WindowChromeButtons() : base() { }
    }

    public class WindowChromeBackgroundPanel : Panel
    {
        private const double kMiniModeHeight = 120;
        private const double kMiniModeLeft = 12;
        private const double kMiniModeRight = 12;
        private const double kHeaderHeight = 120;
        private const double kTopLeftWidth = 12;
        private const double kTopRightWidth = 12;
        private const double kBottomLeftWidth = 8;
        private const double kBottomRightWidth = 8;
        private const double kFooterHeight = 43;
        private const double kLeftEdgeWidth = 7;
        private const double kRightEdgeWidth = 7;

        public WindowChromeBackgroundPanel() : base() { }

        protected override void OnRender(DrawingContext aDrawingContext)
        {
            base.OnRender(aDrawingContext);

            if (this.ActualHeight > kMiniModeHeight + kHeaderHeight)
            {
                Rect leftTopRect = new Rect(0, 0, kTopLeftWidth, kHeaderHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceTopLeftEdge, leftTopRect);
                Rect rightTopRect = new Rect(this.ActualWidth - kTopRightWidth, 0, kTopRightWidth, kHeaderHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceTopRightEdge, rightTopRect);
                Rect leftBottomRect = new Rect(0, this.ActualHeight - kFooterHeight, kBottomLeftWidth, kFooterHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceBottomLeftEdge, leftBottomRect);
                Rect rightBottomRect = new Rect(this.ActualWidth - kBottomRightWidth, this.ActualHeight - kFooterHeight, kBottomRightWidth, kFooterHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceBottomRightEdge, rightBottomRect);
                
                if (this.ActualWidth > kTopLeftWidth + kTopRightWidth)
                {
                    Rect topFillRect = new Rect(kTopLeftWidth, 0, this.ActualWidth - kTopLeftWidth - kTopRightWidth, kHeaderHeight);
                    aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceTopFiller, topFillRect);
                }
                if (this.ActualWidth > kBottomLeftWidth + kBottomRightWidth)
                {
                    Rect bottomFillRect = new Rect(kBottomLeftWidth, this.ActualHeight - kFooterHeight, this.ActualWidth - kBottomLeftWidth - kBottomRightWidth, kFooterHeight);
                    aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceBottomFiller, bottomFillRect);
                }
                if (this.ActualHeight > kHeaderHeight + kFooterHeight)
                {
                    Rect leftFillRect = new Rect(0, kHeaderHeight, kLeftEdgeWidth, this.ActualHeight - kHeaderHeight - kFooterHeight + 1);
                    aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceLeftFiller, leftFillRect);
                    Rect rightFillRect = new Rect(this.ActualWidth - kRightEdgeWidth, kHeaderHeight, kRightEdgeWidth, this.ActualHeight - kHeaderHeight - kFooterHeight + 1);
                    aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceRightFiller, rightFillRect);
                }
            }
            else
            {

                Rect leftTopRect = new Rect(0, 0, kMiniModeLeft, this.ActualHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceKModeLeft, leftTopRect);
                Rect rightTopRect = new Rect(this.ActualWidth - kMiniModeRight, 0, kMiniModeRight, this.ActualHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceKModeRight, rightTopRect);
                Rect centreTopRect = new Rect(leftTopRect.Width, 0, this.ActualWidth - leftTopRect.Width - rightTopRect.Width, this.ActualHeight);
                aDrawingContext.DrawImage(KinskyDesktopWpf.StaticImages.ImageSourceKModeFiller, centreTopRect);
            }
        }
    }
}