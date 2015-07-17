using System;

using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

using Upnp;

namespace Linn.Topology
{
    public interface IModelInfo : IModel
    {
        void Open();
        void Close();

        Device Device { get; }

        uint Bitrate { get; }
        bool Lossless { get; }
        uint BitDepth { get; }
        uint SampleRate { get; }
        string CodecName { get; }
        DidlLite Metatext { get; }
        Channel Track { get; }

        event EventHandler<EventArgs> EventInitialised;

        event EventHandler<EventArgs> EventTrackChanged;
        event EventHandler<EventArgs> EventMetaTextChanged;
        event EventHandler<EventArgs> EventDetailsChanged;
        event EventHandler<EventArgs> EventSubscriptionError;
    }

    public sealed class ModelInfo : IModelInfo
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public ModelInfo(Source aSource)
            : this(aSource.Device, aSource.House.EventServer)
        {
        }

        public ModelInfo(Device aDevice, IEventUpnpProvider aEventServer)
        {
            iDevice = aDevice;

            try
            {
				iServiceInfo = new Linn.ControlPoint.Upnp.ServiceInfo(aDevice, aEventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }
        }

        public void Open()
        {
            iServiceInfo.EventStateMetatext += EventStateMetatextChanged;
            iServiceInfo.EventStateTrackCount += EventStateTrackCountChanged;
            iServiceInfo.EventStateDetailsCount += EventStateDetailsCountChanged;
            iServiceInfo.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceInfo.EventInitial += EventInitialResponse;
        }

        public void Close()
        {
            iServiceInfo.EventStateMetatext -= EventStateMetatextChanged;
            iServiceInfo.EventStateTrackCount -= EventStateTrackCountChanged;
            iServiceInfo.EventStateDetailsCount -= EventStateDetailsCountChanged;
            iServiceInfo.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceInfo.EventInitial -= EventInitialResponse;
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

        public event EventHandler<EventArgs> EventTrackChanged;
        public event EventHandler<EventArgs> EventMetaTextChanged;
        public event EventHandler<EventArgs> EventDetailsChanged;

        private void EventInitialResponse(object sender, EventArgs e)
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        public uint Bitrate
        {
            get
            {
                return iServiceInfo.BitRate;
            }
        }

        public bool Lossless
        {
            get
            {
                return iServiceInfo.Lossless;
            }
        }

        public uint BitDepth
        {
            get
            {
                return iServiceInfo.BitDepth;
            }
        }

        public uint SampleRate
        {
            get
            {
                return iServiceInfo.SampleRate;
            }
        }

        public string CodecName
        {
            get
            {
                return iServiceInfo.CodecName;
            }
        }

        public DidlLite Metatext
        {
            get
            {
                lock (this)
                {
                    return iMetatext;
                }
            }
        }

        public Channel Track
        {
            get
            {
                lock (this)
                {
                    return iTrack;
                }
            }
        }

        private void EventStateMetatextChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                iMetatext = null;

                string metatext = iServiceInfo.Metatext;

                if (!string.IsNullOrEmpty(metatext))
                {
                    try
                    {
                        iMetatext = new DidlLite(metatext);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (EventMetaTextChanged != null)
            {
                EventMetaTextChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateTrackCountChanged(object sender, EventArgs e)
        {
            DidlLite didl = null;
            string uri = iServiceInfo.Uri;
            string metadata = iServiceInfo.Metadata;

            if (!string.IsNullOrEmpty(metadata))
            {
                try
                {
                    didl = new DidlLite(metadata);
                }
                catch (Exception)
                {
                }
            }

            if (didl == null)
            {
                if(!string.IsNullOrEmpty(uri))
                {
                    didl = new DidlLite();
    
                    item item = new item();
                    item.Title = uri;
    
                    didl.Add(item);
                }
            }

            lock (this)
            {
                iTrack = new Channel(uri, didl);
            }

            if (EventTrackChanged != null)
            {
                EventTrackChanged(this, EventArgs.Empty);
            }

            if (EventDetailsChanged != null)
            {
                EventDetailsChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateDetailsCountChanged(object sender, EventArgs e)
        {
            if (EventDetailsChanged != null)
            {
                EventDetailsChanged(this, EventArgs.Empty);
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

		private Linn.ControlPoint.Upnp.ServiceInfo iServiceInfo;

        private Channel iTrack;
        private DidlLite iMetatext;
    }
}
