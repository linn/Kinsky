using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Windows.Input;

namespace KinskyDesktopWpf
{
    public abstract class Kontrol : Control
    {


        protected bool iClickEventStart;
        protected bool iMouseDown;
        protected bool iFirstUpdate;
        protected bool ShouldCaptureMouse { get; set; }

        #region Events
        public event EventHandler UpdateStarted;
        protected void OnUpdateStarted()
        {
            if (UpdateStarted != null)
            {
                UpdateStarted(this, EventArgs.Empty);
            }
        }
        public event EventHandler UpdateFinished;
        protected void OnUpdateFinished()
        {
            if (UpdateFinished != null)
            {
                UpdateFinished(this, EventArgs.Empty);
            }
        }
        public event EventHandler UpdateCancelled;
        protected void OnUpdateCancelled()
        {
            if (UpdateCancelled != null)
            {
                UpdateCancelled(this, EventArgs.Empty);
            }
        }
        public event EventHandler Click;
        protected void OnClick()
        {
            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }
        public event EventHandler Increment;
        protected void OnIncrement()
        {
            if (Increment != null)
            {
                Increment(this, EventArgs.Empty);
            }
        }
        public event EventHandler Decrement;
        protected void OnDecrement()
        {
            if (Decrement != null)
            {
                Decrement(this, EventArgs.Empty);
            }
        }
        #endregion

        static Kontrol()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Kontrol), new FrameworkPropertyMetadata(typeof(Kontrol)));
        }

        public Kontrol()
            : base()
        {
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseLeftButtonDownHandler);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MouseLeftButtonUpHandler);
            this.PreviewMouseMove += new MouseEventHandler(MouseMoveHandler);
            this.MouseLeave += new MouseEventHandler(MouseLeaveHandler);
            this.ShouldCaptureMouse = true;
        }


        protected virtual void MouseLeftButtonDownHandler(object sender, MouseEventArgs args)
        {
            if (HitTestCentre(args))
            {
                iClickEventStart = true;
            }
            else if (HitTestRing(args))
            {
                iMouseDown = true;
                iFirstUpdate = true;
                OnUpdateStarted();
            }
            if (this.ShouldCaptureMouse)
            {
                this.CaptureMouse();
            }
            args.Handled = true;
        }

        protected virtual void MouseLeftButtonUpHandler(object sender, MouseEventArgs args)
        {
            iMouseDown = false;
            if (HitTestCentre(args))
            {
                if (iClickEventStart)   // mouse down event was inside inner circle
                {
                    OnClick();
                }
                OnUpdateFinished();
            }
            else if (HitTestRing(args))
            {
                OnUpdateFinished();
            }
            else
            {
                OnUpdateCancelled();
            }
            if (this.ShouldCaptureMouse)
            {
                this.ReleaseMouseCapture();
            }
            iClickEventStart = false;
            args.Handled = true;
        }

        protected virtual void MouseMoveHandler(object sender, MouseEventArgs args)
        {
            if (HitTestCentre(args))
            {
                if (MouseOverRing)
                {
                    MouseOverRing = false;
                }
                if (!MouseOverCentre)
                {
                    MouseOverCentre = this.IsEnabled;
                }
            }
            else if (HitTestRing(args))
            {
                if (!MouseOverRing)
                {
                    MouseOverRing = this.IsEnabled;
                }
                if (MouseOverCentre)
                {
                    MouseOverCentre = false;
                }
            }
            else
            {
                if (MouseOverRing)
                {
                    MouseOverRing = false;
                }
                if (MouseOverCentre)
                {
                    MouseOverCentre = false;
                }
            }
            args.Handled = true;
        }

        void MouseLeaveHandler(object sender, MouseEventArgs e)
        {
            if (MouseOverRing)
            {
                MouseOverRing = false;
            }
            if (MouseOverCentre)
            {
                MouseOverCentre = false;
            }
        }

        protected bool HitTestRing(MouseEventArgs args)
        {
            System.Windows.Point point = args.GetPosition(this);

            double x = point.X - (ActualWidth * 0.5d);
            double y = point.Y - (ActualHeight * 0.5d);
            double distSquared = (x * x) + (y * y);
            double outerCircleRadius = OuterCircleDiameter / 2;
            return (distSquared < outerCircleRadius * outerCircleRadius) && !HitTestCentre(args) && OuterRingEnabled;
        }

        protected bool HitTestCentre(MouseEventArgs args)
        {
            System.Windows.Point point = args.GetPosition(this);

            double x = point.X - (ActualWidth * 0.5d);
            double y = point.Y - (ActualHeight * 0.5d);
            double distSquared = (x * x) + (y * y);
            double innerCircleRadius = InnerCircleDiameter / 2;
            return distSquared < innerCircleRadius * innerCircleRadius;
        }

        #region DependencyProperties
        public static double GetValue(DependencyObject obj)
        {
            return (double)obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, double value)
        {
            obj.SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(double), typeof(Kontrol), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));


        public static double GetUpdatingValue(DependencyObject obj)
        {
            return (double)obj.GetValue(UpdatingValueProperty);
        }

        public static void SetUpdatingValue(DependencyObject obj, double value)
        {
            obj.SetValue(UpdatingValueProperty, value);
        }

        public static readonly DependencyProperty UpdatingValueProperty =
            DependencyProperty.RegisterAttached("UpdatingValue", typeof(double), typeof(Kontrol), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));


        public static double GetMaxValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxValueProperty);
        }

        public static void SetMaxValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.RegisterAttached("MaxValue", typeof(double), typeof(Kontrol), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));



        public bool IsDimmed
        {
            get { return (bool)GetValue(IsDimmedProperty); }
            set { SetValue(IsDimmedProperty, value); }
        }

        public static readonly DependencyProperty IsDimmedProperty =
            DependencyProperty.Register("IsDimmed", typeof(bool), typeof(Kontrol), new UIPropertyMetadata(false));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Kontrol), new UIPropertyMetadata(string.Empty));




        public double InnerCircleDiameter
        {
            get { return (double)GetValue(InnerCircleDiameterProperty); }
            set { SetValue(InnerCircleDiameterProperty, value); }
        }

        public static readonly DependencyProperty InnerCircleDiameterProperty =
            DependencyProperty.Register("InnerCircleDiameter", typeof(double), typeof(Kontrol), new UIPropertyMetadata(47d));

        public double OuterCircleDiameter
        {
            get { return (double)GetValue(OuterCircleDiameterProperty); }
            set { SetValue(OuterCircleDiameterProperty, value); }
        }

        public static readonly DependencyProperty OuterCircleDiameterProperty =
            DependencyProperty.Register("OuterCircleDiameter", typeof(double), typeof(Kontrol), new UIPropertyMetadata(85d));
        
        public double MiddleCircleDiameter
        {
            get { return (double)GetValue(MiddleCircleDiameterProperty); }
            set { SetValue(MiddleCircleDiameterProperty, value); }
        }

        public static readonly DependencyProperty MiddleCircleDiameterProperty =
            DependencyProperty.Register("MiddleCircleDiameter", typeof(double), typeof(Kontrol), new UIPropertyMetadata(85d));


        public bool MouseOverRing
        {
            get { return (bool)GetValue(MouseOverRingProperty); }
            set { SetValue(MouseOverRingProperty, value); }
        }

        public static readonly DependencyProperty MouseOverRingProperty =
            DependencyProperty.Register("MouseOverRing", typeof(bool), typeof(Kontrol), new UIPropertyMetadata(false));



        public bool MouseOverCentre
        {
            get { return (bool)GetValue(MouseOverCentreProperty); }
            set { SetValue(MouseOverCentreProperty, value); }
        }

        public static readonly DependencyProperty MouseOverCentreProperty =
            DependencyProperty.Register("MouseOverCentre", typeof(bool), typeof(Kontrol), new UIPropertyMetadata(false));



        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(Kontrol), new UIPropertyMetadata(false));



        public bool OuterRingEnabled
        {
            get { return (bool)GetValue(OuterRingEnabledProperty); }
            set { SetValue(OuterRingEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OuterRingEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OuterRingEnabledProperty =
            DependencyProperty.Register("OuterRingEnabled", typeof(bool), typeof(Kontrol), new UIPropertyMetadata(true));

        

        #endregion

    }
}