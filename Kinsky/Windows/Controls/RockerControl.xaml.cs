using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Linn;
using Linn.Kinsky;
using Linn.ControlPoint.Upnp;
using Linn.Control.Ssdp;
using Linn.Topology;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;
using Upnp;
using System.Drawing;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace KinskyDesktopWpf
{

    public partial class RockerControl : Kontrol
    {

        private const int kTimerStartDelay = 250;
        private DispatcherTimer iTimer;
        public RockerControl()
        {
            InitializeComponent();
            this.ShouldCaptureMouse = false;
            iTimer = new DispatcherTimer();
            iTimer.Tick += new EventHandler(iTimer_Tick);
        }

        void iTimer_Tick(object sender, EventArgs e)
        {
            iTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            OnTimerTick();
        }

        void OnTimerTick()
        {
            if (LeftButtonDown)
            {
                OnDecrement();
            }
            else if (RightButtonDown)
            {
                OnIncrement();
            }
        }

        protected override void MouseLeftButtonDownHandler(object sender, MouseEventArgs args)
        {
            base.MouseLeftButtonDownHandler(sender, args);
            args.Handled = !(args.OriginalSource is Polygon);
        }
        protected override void MouseLeftButtonUpHandler(object sender, MouseEventArgs args)
        {
            try
            {
                base.MouseLeftButtonUpHandler(sender, args);
                args.Handled = !(args.OriginalSource is Polygon);
            }
            finally
            {
                if (LeftButtonDown || RightButtonDown)
                {
                    LeftButtonDown = false;
                    RightButtonDown = false;
                    iTimer.Stop();
                }
            }
        }


        private void hitBoxLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OuterRingEnabled)
            {
                iTimer.Interval = TimeSpan.FromMilliseconds(kTimerStartDelay);
                iTimer.Start();
                LeftButtonDown = true;
                OnTimerTick();
                e.Handled = true;
            }
        }

        private void hitBoxRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OuterRingEnabled)
            {
                iTimer.Interval = TimeSpan.FromMilliseconds(kTimerStartDelay);
                iTimer.Start();
                RightButtonDown = true;
                OnTimerTick();
                e.Handled = true;
            }
        }

        private void hitBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (LeftButtonDown || RightButtonDown)
            {
                LeftButtonDown = false;
                RightButtonDown = false;
                iTimer.Stop();
            }
        }


        public bool LeftButtonDown
        {
            get { return (bool)GetValue(LeftButtonDownProperty); }
            set { SetValue(LeftButtonDownProperty, value); }
        }

        public static readonly DependencyProperty LeftButtonDownProperty =
            DependencyProperty.Register("LeftButtonDown", typeof(bool), typeof(RockerControl), new UIPropertyMetadata(false));

        public bool RightButtonDown
        {
            get { return (bool)GetValue(RightButtonDownProperty); }
            set { SetValue(RightButtonDownProperty, value); }
        }

        public static readonly DependencyProperty RightButtonDownProperty =
            DependencyProperty.Register("RightButtonDown", typeof(bool), typeof(RockerControl), new UIPropertyMetadata(false));


        public int TimerInterval
        {
            get { return (int)GetValue(TimerIntervalProperty); }
            set { SetValue(TimerIntervalProperty, value); }
        }

        public static readonly DependencyProperty TimerIntervalProperty =
            DependencyProperty.Register("TimerInterval", typeof(int), typeof(RockerControl), new FrameworkPropertyMetadata(100, (d, e) =>
            {
                RockerControl thisObj = d as RockerControl;
                if (thisObj.iTimer != null)
                {
                    thisObj.iTimer.Interval = TimeSpan.FromMilliseconds((int)e.NewValue);
                }
            }));

        

    }
}
