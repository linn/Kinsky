using System;
namespace Linn.Kinsky
{

    public class EventArgsAngleUpdated : EventArgs
    {
        public EventArgsAngleUpdated(double aAngle)
        {
            Angle = aAngle;
        }

        public double Angle { get; set; }
    }

    public class RotaryControlTracker
    {

        public RotaryControlTracker()
        {
            StepSize = kDefaultStepSize;
            iFirstUpdate = true;
        }

        public RotaryControlTracker(double aHeight, double aWidth)
            : this()
        {
            Height = aHeight;
            Width = aWidth;
        }

        public event EventHandler<EventArgs> EventIncremented;
        public event EventHandler<EventArgs> EventDecremented;
        public event EventHandler<EventArgsAngleUpdated> EventAngleUpdated;

        public double Height { get; set; }
        public double Width { get; set; }
        public double StepSize { get; set; }

        public void TrackStart()
        {
            iFirstUpdate = true;
        }

        public void TrackMoveEvent(double x, double y)
        {
            Assert.Check(Height != 0 && Width != 0);
            double angle = ((Math.Atan2(y - (Height * 0.5f), x - (Width * 0.5f)) * 180) / Math.PI) + 270;

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

            if (iLastAngle > StepSize)
            {
                int steps = (int)(iLastAngle / StepSize);

                Trace.WriteLine(Trace.kKinsky, "RotaryControl.UpdateAngle: delta=" + delta + ", steps=" + steps);

                for (int i = 0; i < steps; ++i)
                {
                    OnEventIncrement();
                }

                iLastAngle = iLastAngle - (steps * StepSize);
            }

            if (iLastAngle < -StepSize)
            {
                int steps = (int)(-iLastAngle / StepSize);

                Trace.WriteLine(Trace.kKinsky, "RotaryControl.UpdateAngle: delta=" + delta + ", steps=" + steps);

                for (int i = 0; i < steps; ++i)
                {
                    OnEventDecremented();
                }

                iLastAngle = (-iLastAngle) - (steps * StepSize);
            }
            iCurrentAngle += delta;
            OnEventAngleUpdated(iCurrentAngle);

            iOldAngle = iNewAngle;

        }

        private void OnEventIncrement()
        {
            EventHandler<EventArgs> del = EventIncremented;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventDecremented()
        {
            EventHandler<EventArgs> del = EventDecremented;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventAngleUpdated(double aAngle)
        {
            EventHandler<EventArgsAngleUpdated> del = EventAngleUpdated;
            if (del != null)
            {
                del(this, new EventArgsAngleUpdated(aAngle));
            }
        }

        private double iLastAngle;
        private double iOldAngle;
        private double iNewAngle;
        private const int kDefaultStepSize = 20;
        private bool iFirstUpdate;
        private double iCurrentAngle;

    }

}