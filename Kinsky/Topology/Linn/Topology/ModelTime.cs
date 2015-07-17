using System;

using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public interface IModelTime : IModel
    {
        void Open();
        void Close();

        Device Device { get; }
        
        uint Seconds { get; }
        uint Duration { get; }

        event EventHandler<EventArgs> EventInitialised;

        event EventHandler<EventArgs> EventSecondsChanged;
        event EventHandler<EventArgs> EventDurationChanged;
        event EventHandler<EventArgs> EventSubscriptionError;
    }

    public sealed class ModelTime : IModelTime
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public ModelTime(Source aSource)
            : this(aSource.Device, aSource.House.EventServer)
        {
        }

        public ModelTime(Device aDevice, IEventUpnpProvider aEventServer)
        {
            iDevice = aDevice;

            try
            {
                iServiceTime = new ServiceTime(aDevice, aEventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }
        }

        public void Open()
        {
            iServiceTime.EventStateSeconds += EventStateSecondsResponse;
            iServiceTime.EventStateDuration += EventStateDurationResponse;
            iServiceTime.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceTime.EventInitial += EventInitialResponse;
        }

        public void Close()
        {
            iServiceTime.EventStateSeconds -= EventStateSecondsResponse;
            iServiceTime.EventStateDuration -= EventStateDurationResponse;
            iServiceTime.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceTime.EventInitial -= EventInitialResponse;
        }

        public Device Device
        {
            get
            {
                return iDevice;
            }
        }

        public string DeviceXml
        {
            get
            {
                return iDevice.DeviceXml;
            }
        }

        public event EventHandler<EventArgs> EventInitialised;

        public event EventHandler<EventArgs> EventSecondsChanged;
        public event EventHandler<EventArgs> EventDurationChanged;

        private void EventInitialResponse(object sender, EventArgs e)
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        public uint Seconds
        {
            get
            {
                return iServiceTime.Seconds;
            }
        }

        public uint Duration
        {
            get
            {
                return iServiceTime.Duration;
            }
        }

        private void EventStateSecondsResponse(object sender, EventArgs e)
        {
            if (EventSecondsChanged != null)
            {
                EventSecondsChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateDurationResponse(object sender, EventArgs e)
        {
            if (EventDurationChanged != null)
            {
                EventDurationChanged(this, EventArgs.Empty);
            }
        }

        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            OnEventSubscriptionError();
        }

        private void OnEventSubscriptionError()
        {
            EventHandler<EventArgs> eventSubscriptionError = EventSubscriptionError;
            if (eventSubscriptionError != null)
            {
                eventSubscriptionError(this, EventArgs.Empty);
            }
        }
        private Device iDevice;
        private ServiceTime iServiceTime;
    }
}
