using System;

using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

using Upnp;

namespace Linn.Topology
{

    public interface IModelSourceReceiver : IModelSource
    {
        Channel Channel { get; }
        event EventHandler<EventArgs> EventChannelChanged;
        event EventHandler<EventArgs> EventControlInitialised;
        void EventInitialResponseReceiver(object sender, EventArgs e);
        void EventSetChannelResponse(object sender, EventArgs e);
        event EventHandler<EventArgs> EventTransportStateChanged;
        bool IsPlayingSender(ModelSender aSender);
        bool Next();
        void Play();
        void PlayNow(DidlLite aDidlLite);
        bool Previous();
        string ProtocolInfo { get; }
        void SetChannel(DidlLite aDidlLite);
        void Stop();
        ModelSourceReceiver.ETransportState TransportState { get; }
        event EventHandler<EventArgs> EventSubscriptionError;
    }

    public sealed class ModelSourceReceiver : ModelSource, IModelSourceReceiver, IMediaSupported
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public enum ETransportState
        {
            eUnknown,
            ePlaying,
            eWaiting,
            eStopped,
            eBuffering
        }

        public ModelSourceReceiver(Source aSource)
        {
            iSource = aSource;

            try
            {
                iServiceReceiver = new ServiceReceiver(iSource.Device, iSource.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionPlay = iServiceReceiver.CreateAsyncActionPlay();
            iActionStop = iServiceReceiver.CreateAsyncActionStop();
            iActionSetChannel = iServiceReceiver.CreateAsyncActionSetSender();

            iActionPlayNowSetChannel = iServiceReceiver.CreateAsyncActionSetSender();
            iActionPlayNowSetChannel.EventResponse += EventSetChannelResponse;
        }

        public override void Open()
        {
            iServiceReceiver.EventStateTransportState += EventStateTransportStateChanged;
            iServiceReceiver.EventStateUri += EventStateChannelChanged;
            iServiceReceiver.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceReceiver.EventInitial += EventInitialResponseReceiver;
        }

        public override void Close()
        {
            iServiceReceiver.EventStateTransportState -= EventStateTransportStateChanged;
            iServiceReceiver.EventStateUri -= EventStateChannelChanged;
            iServiceReceiver.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceReceiver.EventInitial -= EventInitialResponseReceiver;
        }

        public override string Name
        {
            get
            {
                return (iSource.FullName);
            }
        }

        public override Source Source
        {
            get
            {
                return iSource;
            }
        }

        public resource BestSupportedResource(upnpObject aObject)
        {
            return BestSupportedResource(iServiceReceiver.ProtocolInfo, aObject);
        }

        public event EventHandler<EventArgs> EventControlInitialised;

        public event EventHandler<EventArgs> EventTransportStateChanged;
        public event EventHandler<EventArgs> EventChannelChanged;

        public void EventInitialResponseReceiver(object sender, EventArgs e)
        {
            if (EventControlInitialised != null)
            {
                EventControlInitialised(this, EventArgs.Empty);
            }
        }

        public void PlayNow(DidlLite aDidlLite)
        {
            SetChannel(aDidlLite, iActionPlayNowSetChannel);
        }

        public void SetChannel(DidlLite aDidlLite)
        {
            SetChannel(aDidlLite, iActionSetChannel);
        }

        private void SetChannel(DidlLite aDidlLite, ServiceReceiver.AsyncActionSetSender aAction)
        {
            if (aDidlLite.Count > 0)
            {
                upnpObject o = aDidlLite[0];
                if (o.Res.Count > 0)
                {
                    string uri = o.Res[0].Uri;
                    DidlLite didl = new DidlLite();
                    didl.Add(o);
                    aAction.SetSenderBegin(uri, didl.Xml);
                }
            }
        }

        public void Play()
        {
            iActionPlay.PlayBegin();
        }

        public void Stop()
        {
            iActionStop.StopBegin();
        }

        public bool Previous()
        {
            throw new NotImplementedException();
        }

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public ETransportState TransportState
        {
            get
            {
                return iTransportState;
            }
        }

        public string ProtocolInfo
        {
            get
            {
                return iServiceReceiver.ProtocolInfo;
            }
        }

        public Channel Channel
        {
            get
            {
                lock (this)
                {
                    return iChannel;
                }
            }
        }

        public void EventSetChannelResponse(object sender, EventArgs e)
        {
            Play();
        }

        private void EventStateTransportStateChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                if (iServiceReceiver.TransportState == "Playing")
                {
                    iTransportState = ETransportState.ePlaying;
                }
                else if (iServiceReceiver.TransportState == "Stopped")
                {
                    iTransportState = ETransportState.eStopped;
                }
                else if (iServiceReceiver.TransportState == "Buffering")
                {
                    iTransportState = ETransportState.eBuffering;
                }
                else if (iServiceReceiver.TransportState == "Waiting")
                {
                    iTransportState = ETransportState.eWaiting;
                }
                else
                {
                    iTransportState = ETransportState.eUnknown;
                }
            }

            if (EventTransportStateChanged != null)
            {
                EventTransportStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateChannelChanged(object sender, EventArgs e)
        {
            DidlLite didl = null;
            string uri = iServiceReceiver.Uri;
            string metadata = iServiceReceiver.Metadata;

            if (metadata.Length > 0)
            {
                try
                {
                    didl = new DidlLite(iServiceReceiver.Metadata);
                }
                catch (Exception)
                {
                }
            }

            if (didl == null)
            {
                didl = new DidlLite();

                item item = new item();
                item.Title = uri;

                didl.Add(item);
            }

            lock(this)
            {
                if (!string.IsNullOrEmpty(uri))
                {
                    iChannel = new Channel(uri, didl);
                }
                else
                {
                    iChannel = null;
                }
            }

            if (EventChannelChanged != null)
            {
                EventChannelChanged(this, EventArgs.Empty);
            }
        }

        public bool IsPlayingSender(ModelSender aSender)
        {
            bool playingSender = false;

            if (this.Channel != null && aSender != null)
            {

                if (this.TransportState == ModelSourceReceiver.ETransportState.ePlaying
                    || this.TransportState == ModelSourceReceiver.ETransportState.eBuffering
                    || this.TransportState == ModelSourceReceiver.ETransportState.eWaiting)
                {
                    if (aSender.Metadata.Count > 0 && this.Channel.DidlLite.Count > 0)
                    {
                        upnpObject sender = aSender.Metadata[0];
                        upnpObject receiver = this.Channel.DidlLite[0];

                        if (sender.Res.ToString() == receiver.Res.ToString() && DidlLiteAdapter.Title(sender) == DidlLiteAdapter.Title(receiver))
                        {
                            playingSender = true;
                        }
                    }
                }
            }

            return playingSender;
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

        private Source iSource;

        private ServiceReceiver iServiceReceiver;
        private ServiceReceiver.AsyncActionPlay iActionPlay;
        private ServiceReceiver.AsyncActionStop iActionStop;
        private ServiceReceiver.AsyncActionSetSender iActionSetChannel;
        private ServiceReceiver.AsyncActionSetSender iActionPlayNowSetChannel;

        private ETransportState iTransportState;
        private Channel iChannel;
    }
}