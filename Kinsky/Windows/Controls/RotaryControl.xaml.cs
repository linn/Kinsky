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

namespace KinskyDesktopWpf
{

    public partial class RotaryControl : Kontrol
    {




        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(RotaryControl), new UIPropertyMetadata(0d));


        private double iLastAngle;
        private double iOldAngle;
        private double iNewAngle;
        private const int kStepSize = 20;

        public RotaryControl()
        {
            InitializeComponent();
        }

        protected override void MouseMoveHandler(object sender, MouseEventArgs args)
        {
            base.MouseMoveHandler(sender, args);
            if (iMouseDown && OuterRingEnabled)
            {
                UpdateAngle(args);
            }
        }

        private void UpdateAngle(MouseEventArgs args)
        {
            System.Windows.Point point = args.GetPosition(this);
            
            double angle = ((Math.Atan2(point.Y - (Height * 0.5f), point.X - (Width * 0.5f)) * 180) / Math.PI) + 270;

            if (angle > 360)
            {
                angle -= 360;
            }

            iNewAngle = angle;
            if (iFirstUpdate)
            {
                iLastAngle = 0;
                iOldAngle = iNewAngle;
                iFirstUpdate = false;
            }

            double delta = iNewAngle - iOldAngle;

            if (delta > 180)
            {
                delta -= 360;
            }

            if (delta < -180)
            {
                delta += 360;
            }

            iLastAngle += delta;
            
            // event according to angle delta

            if (iLastAngle > kStepSize)
            {
                int steps = (int)(iLastAngle / kStepSize);

                Trace.WriteLine(Trace.kKinsky, "RotaryControl.UpdateAngle: delta=" + delta + ", steps=" + steps);

                for (int i = 0; i < steps; ++i)
                {
                    OnIncrement();
                }

                iLastAngle = iLastAngle - (steps * kStepSize);
            }

            if (iLastAngle < - kStepSize)
            {
                int steps = (int)(-iLastAngle / kStepSize);

                Trace.WriteLine(Trace.kKinsky, "RotaryControl.UpdateAngle: delta=" + delta + ", steps=" + steps);

                for (int i = 0; i < steps; ++i)
                {
                    OnDecrement();
                }

                iLastAngle = (-iLastAngle) - (steps * kStepSize);
            }

            PositionRingPoints(delta);

            iOldAngle = iNewAngle;
            
        }




        private void PositionRingPoints(double delta)
        {
            Angle += delta;
        }
    }
}
