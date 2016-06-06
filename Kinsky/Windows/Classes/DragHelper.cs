using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Documents;
using Linn;

namespace KinskyDesktopWpf
{

    public class DragScroller
    {

        public class EventArgsItemsDropped : EventArgs
        {
            public EventArgsItemsDropped(DragEventArgs aArgs)
            {
                DragEventArgs = aArgs;
            }
            public DragEventArgs DragEventArgs { get; set; }
        }

        public event EventHandler<EventArgsItemsDropped> ItemsDropped;

        public DragScroller(ListView aListView, Func<DragEventArgs, DragDropEffects> aDragEffectsFunction)
        {
            iDragEffectsFunction = aDragEffectsFunction;
            iListView = aListView;
            iListView.AllowDrop = true;
            iListView.DragEnter += ListView_DragEnter;
            iListView.DragOver += ListView_DragOver;
            iListView.DragLeave += ListView_DragLeave;
            iListView.Drop += ListView_Drop;
            iScrollTimer = new DispatcherTimer();
            iScrollTimer.Interval = TimeSpan.FromMilliseconds(kScrollTimer);
            iScrollTimer.Tick += new EventHandler(ScrollTimer_Elapsed);
        }

        private void ScrollTimer_Elapsed(object sender, EventArgs args)
        {
            if (iDragging)
            {
                ScrollViewer viewer = iListView.GetScrollViewer();
                if (viewer != null && (int)iScrollTimer.Tag != 0)
                {
                    viewer.ScrollToVerticalOffset(viewer.VerticalOffset + (int)iScrollTimer.Tag);
                }
            }
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                iDragging = true;
                DragDropEffects effects = iDragEffectsFunction(e);
                e.Effects = effects;
                if (effects != DragDropEffects.None)
                {
                    ListBoxItem item = iListView.GetEventSourceElement<ListBoxItem>(e);
                    if (item != null)
                    {
                        Point p = e.GetPosition(item);
                        bool top = p.Y < item.ActualHeight / 2;
                        SetAdorner(item, top);
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in DragScroller.DragOver: " + ex);
            }
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                DragDropEffects effects = iDragEffectsFunction(e);
                e.Effects = effects;
                if (effects != DragDropEffects.None)
                {
                    Point p = e.GetPosition(iListView);
                    int scrollAmount = 0;
                    if (p.Y > iListView.ActualHeight - kScrollThreshold)
                    {
                        scrollAmount += kScrollAmount;
                    }
                    else if (p.Y < kScrollThreshold)
                    {
                        scrollAmount -= kScrollAmount;
                    }

                    if (scrollAmount != 0 && !iScrollTimer.IsEnabled)
                    {
                        iScrollTimer.Tag = scrollAmount;
                        iScrollTimer.Start();
                    }

                    ListBoxItem item = iListView.GetEventSourceElement<ListBoxItem>(e);
                    if (item != null)
                    {
                        p = e.GetPosition(item);
                        bool top = p.Y < item.ActualHeight / 2;
                        SetAdorner(item, top);
                    }

                } 
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in DragScroller.DragOver: " + ex);
            }
        }

        private void ListView_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                iDragging = false;
                DragDropEffects effects = iDragEffectsFunction(e);
                e.Effects = effects;
                RemoveAdorner();
                iScrollTimer.Stop();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in DragScroller.DragLeave: " + ex);
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                iDragging = false;
                DragDropEffects effects = iDragEffectsFunction(e);
                e.Effects = effects;
                if (effects != DragDropEffects.None)
                {
                    OnItemsDropped(e);
                }
                RemoveAdorner();
                iScrollTimer.Stop();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in DragScroller.Drop: " + ex);
            }
        }

        private void OnItemsDropped(DragEventArgs aArgs)
        {
            if (ItemsDropped != null)
            {
                ItemsDropped(this, new EventArgsItemsDropped(aArgs));
            }
        }

        private void RemoveAdorner()
        {
            if (iAdorner != null)
            {
                try
                {
                    AdornerLayer.GetAdornerLayer(iAdorner.AdornedElement).Remove(iAdorner);
                }
                catch { } //element may no longer exist
            }
        }

        private void SetAdorner(UIElement aElement, bool aTop)
        {
            RemoveAdorner();
            iAdorner = new DragAdorner(aElement, aTop);
            AdornerLayer.GetAdornerLayer(aElement).Add(iAdorner);
        }

        private Func<DragEventArgs, DragDropEffects> iDragEffectsFunction;
        private bool iDragging;
        private DispatcherTimer iScrollTimer;
        private ListView iListView;
        private static int kScrollThreshold = 20;
        private static int kScrollAmount = 25;
        private static double kScrollTimer = 100;
        private Adorner iAdorner;
    }

    public class DragHelper
    {

        public static readonly DependencyProperty IsDraggedOverProperty = DependencyProperty.RegisterAttached(
          "IsDraggedOver",
          typeof(bool),
          typeof(DragHelper),
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
        );
        public static void SetIsDraggedOver(UIElement element, bool value)
        {
            element.SetValue(IsDraggedOverProperty, value);
        }
        public static bool GetIsDraggedOver(UIElement element)
        {
            return (bool)element.GetValue(IsDraggedOverProperty);
        }

        public event EventHandler<MouseEventArgs> EventDragInitiated;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public static Int32 GWL_EXSTYLE = -20;
        public static Int32 WS_EX_LAYERED = 0x00080000;
        public static Int32 WS_EX_TRANSPARENT = 0x00000020;


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetWindowLong(IntPtr hWnd, Int32 nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 newVal);




        private UIElement iDragSource;
        private Point iStartPoint;
        private Window iDragdropWindow;
        private bool iIsDragging;
        protected DragDropEffects iAllowedEffects = DragDropEffects.Copy | DragDropEffects.Move;
        public const int kDefaultVisualHeight = 50;
        private object iSender;


        protected bool IsDragging
        {
            get
            {
                return iIsDragging;
            }
            set
            {
                iIsDragging = value;
            }
        }

        public DragDropEffects AllowedEffects
        {
            get
            {
                return iAllowedEffects;
            }
            set
            {
                iAllowedEffects = value;
            }
        }

        public DragHelper(UIElement aDragSource)
        {
            iDragSource = aDragSource;
            iDragSource.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown), true);
            iDragSource.AddHandler(UIElement.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(DragSource_PreviewMouseRightButtonDown), true);
            iDragSource.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(DragSource_PreviewMouseMove);
            iDragSource.MouseLeave += new MouseEventHandler(iDragSource_MouseLeave);
        }

        void iDragSource_MouseLeave(object sender, MouseEventArgs e)
        {
            iSender = null;
        }

        void DragSource_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            iSender = null;
        }

        void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            iStartPoint = e.GetPosition(iDragSource);
            iSender = sender;
        }

        void DragSource_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed
                && !IsDragging
                && sender == iSender)
            {
                Point position = e.GetPosition(iDragSource);
                if (Math.Abs(position.X - iStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - iStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (EventDragInitiated != null)
                    {
                        EventDragInitiated(sender, e);
                    }
                    iSender = null;
                }
            }
        }

        public DragDropEffects DoDragDrop(UIElement aSourceElement, IDataObject aDataObject, DragDropEffects aEffects, FrameworkElement aSourceVisual)
        {
            QueryContinueDragEventHandler queryContinue = null;
            DragDropEffects resultEffects = DragDropEffects.None;

            if (aDataObject != null && aSourceElement != null)
            {
                DragDrop.AddPreviewQueryContinueDragHandler(aSourceElement, queryContinue = new QueryContinueDragEventHandler(DragHelper_OnQueryContinueDrag));

                IsDragging = true;
                CreateDragDropWindow(aSourceVisual);
                this.iDragdropWindow.Show();
                try
                {
                    resultEffects = DragDrop.DoDragDrop(aSourceElement, aDataObject, aEffects);
                }
                catch (Exception ex)
                {
                    Linn.UserLog.WriteLine("Exception caught performing DragDrop : " + ex.ToString());
                }

                DestroyDragDropWindow();
                DragDrop.RemovePreviewQueryContinueDragHandler(aSourceElement, DragHelper_OnQueryContinueDrag);
                IsDragging = false;
                System.Windows.Input.Mouse.Capture(null);
                DestroyDragDropWindow();
            }
            return resultEffects;
        }

        protected bool AllowsLink
        {
            get
            {
                return ((this.AllowedEffects & DragDropEffects.Link) != 0);
            }
        }

        protected bool AllowsMove
        {
            get
            {
                return ((this.AllowedEffects & DragDropEffects.Move) != 0);
            }
        }

        protected bool AllowsCopy
        {
            get
            {
                return ((this.AllowedEffects & DragDropEffects.Copy) != 0);
            }
        }


        DragDropEffects GetDragDropEffects()
        {
            DragDropEffects effects = DragDropEffects.None;
            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (ctrl && shift && this.AllowsLink)
                effects |= DragDropEffects.Link;
            else if (ctrl && this.AllowsCopy)
                effects |= DragDropEffects.Copy;
            else if (this.AllowsMove)
                effects |= DragDropEffects.Move;

            return effects;
        }


        void DragHelper_OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            UpdateWindowLocation();
        }



        protected void DestroyDragDropWindow()
        {
            if (this.iDragdropWindow != null)
            {
                this.iDragdropWindow.Close();
                this.iDragdropWindow = null;
            }
        }

        protected void CreateDragDropWindow(FrameworkElement aDragImage)
        {
            this.iDragdropWindow = new Window();
            iDragdropWindow.WindowStyle = WindowStyle.None;
            iDragdropWindow.AllowsTransparency = true;
            iDragdropWindow.AllowDrop = false;
            iDragdropWindow.Background = null;
            iDragdropWindow.IsHitTestVisible = false;
            iDragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            iDragdropWindow.Topmost = true;
            iDragdropWindow.ShowInTaskbar = false;

            iDragdropWindow.SourceInitialized += new EventHandler(
            delegate(object sender, EventArgs args)
            {
                PresentationSource windowSource = PresentationSource.FromVisual(this.iDragdropWindow);
                IntPtr handle = ((System.Windows.Interop.HwndSource)windowSource).Handle;

                Int32 styles = GetWindowLong(handle, GWL_EXSTYLE);
                SetWindowLong(handle, GWL_EXSTYLE, styles | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            });

            this.iDragdropWindow.Content = aDragImage;

            UpdateWindowLocation();
        }


        protected void UpdateWindowLocation()
        {
            if (this.iDragdropWindow != null)
            {
                POINT p;
                if (!GetCursorPos(out p))
                {
                    return;
                }
                this.iDragdropWindow.Left = (double)p.X;
                this.iDragdropWindow.Top = (double)p.Y;
            }
        }

    }

    public class DragAdorner : Adorner
    {
        private bool iRenderTop;
        public DragAdorner(UIElement adornedElement, bool aRenderTop)
            : base(adornedElement)
        {
            iRenderTop = aRenderTop;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            Pen drawingPen = new Pen(Application.Current.FindResource("SolidBorderBrush") as Brush, 3);

            Point left = iRenderTop ? adornedElementRect.TopLeft : adornedElementRect.BottomLeft;
            Point right = iRenderTop ? adornedElementRect.TopRight : adornedElementRect.BottomRight;

            drawingContext.DrawLine(drawingPen, left, right);
        }
    }
}