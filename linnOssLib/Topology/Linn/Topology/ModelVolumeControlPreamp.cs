using System;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class ModelVolumeControlPreamp : ModelVolumeControl, IVolumeLimiterControl
    {
        public ModelVolumeControlPreamp(Preamp aPreamp)
        {
            iPreamp = aPreamp;

            iVolumeLimiter = new VolumeLimiter(this);

            try
            {
                iServiceVolume= new ServiceVolume(iPreamp.Device, iPreamp.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionSetMute = iServiceVolume.CreateAsyncActionSetMute();
            iActionSetVolume = iServiceVolume.CreateAsyncActionSetVolume();
            iActionVolumeInc = iServiceVolume.CreateAsyncActionVolumeInc();
            iActionVolumeDec = iServiceVolume.CreateAsyncActionVolumeDec();
        }

        public override void Open()
        {
            iVolumeLimiter.Start();

            iServiceVolume.EventStateMute += EventStateMuteResponse;
            iServiceVolume.EventStateVolume += EventStateVolumeResponse;
            iServiceVolume.EventStateVolumeLimit += EventStateVolumeLimitResponse;
            iServiceVolume.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceVolume.EventInitial += EventInitialResponse;
        }

        public override void Close()
        {
            iVolumeLimiter.Stop();

            iServiceVolume.EventStateMute -= EventStateMuteResponse;
            iServiceVolume.EventStateVolume -= EventStateVolumeResponse;
            iServiceVolume.EventStateVolumeLimit -= EventStateVolumeLimitResponse;
            iServiceVolume.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceVolume.EventInitial -= EventInitialResponse;
        }

        public override Device Device
        {
            get { return iPreamp.Device; }
        }

        public override string Name
        {
            get
            {
                return iPreamp.Type;
            }
        }

        private void EventInitialResponse(object sender, EventArgs e)
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        public override event EventHandler<EventArgs> EventInitialised;
        public override event EventHandler<EventArgs> EventMuteStateChanged;
        public override event EventHandler<EventArgs> EventVolumeChanged;
        public override event EventHandler<EventArgs> EventVolumeLimitChanged;

        public override void IncrementVolume()
        {
            iVolumeLimiter.IncrementVolume();
            //iActionVolumeInc.VolumeIncBegin();
        }

        public override void DecrementVolume()
        {
            iVolumeLimiter.DecrementVolume();
            //iActionVolumeDec.VolumeDecBegin();
        }

        public override void SetVolume(uint aValue)
        {
            iActionSetVolume.SetVolumeBegin(aValue);
        }

        public override void ToggleMute()
        {
            iActionSetMute.SetMuteBegin(!iServiceVolume.Mute);
        }

        public override void SetMute(bool aValue)
        {
            iActionSetMute.SetMuteBegin(aValue);
        }

        public override uint Volume
        {
            get
            {
                return iServiceVolume.Volume;
            }
        }

        public override bool Mute
        {
            get
            {
                return iServiceVolume.Mute;
            }
        }

        public override uint VolumeLimit
        {
            get
            {
                return iServiceVolume.VolumeLimit;
            }
        }

        void IVolumeLimiterControl.IncrementVolume()
        {
            iServiceVolume.VolumeIncSync();
        }

        void IVolumeLimiterControl.DecrementVolume()
        {
            iServiceVolume.VolumeDecSync();
        }

        private void EventStateMuteResponse(object sender, EventArgs e)
        {
            if (EventMuteStateChanged != null)
            {
                EventMuteStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateVolumeResponse(object sender, EventArgs e)
        {
            if (EventVolumeChanged != null)
            {
                EventVolumeChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateVolumeLimitResponse(object sender, EventArgs e)
        {
            if (EventVolumeLimitChanged != null)
            {
                EventVolumeLimitChanged(this, EventArgs.Empty);
            }
        }

        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            OnEventSubscriptionError();
        }

        private Preamp iPreamp;

        private VolumeLimiter iVolumeLimiter;

        private ServiceVolume iServiceVolume;

        private ServiceVolume.AsyncActionSetMute iActionSetMute;
        private ServiceVolume.AsyncActionSetVolume iActionSetVolume;
        private ServiceVolume.AsyncActionVolumeInc iActionVolumeInc;
        private ServiceVolume.AsyncActionVolumeDec iActionVolumeDec;

    }
} // Linn.Topology
