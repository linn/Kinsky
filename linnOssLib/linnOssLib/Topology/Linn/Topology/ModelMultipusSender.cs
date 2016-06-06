using System;

using Linn.ControlPoint.Upnp;

using Upnp;

namespace Linn.Topology
{
    public interface IModelSender
    {
        bool Audio { get; }
        string FullName { get; }
        DidlLite Metadata { get; }
        string Name { get; }
        string Room { get; }
        ModelSender.EStatus Status { get; }
        string Udn { get; }
        event EventHandler<EventArgs> EventSubscriptionError;
    }

    public sealed class ModelSender : IModelSender
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public enum EStatus
        {
            eSending,
            eReady,
            eBlocked,
            eInactive,
            eDisabled,
            eUnknown
        }

        public ModelSender(Sender aSender, IEventUpnpProvider aEventServer)
        {
            iSender = aSender;

            iServiceSender = new ServiceSender(aSender.Device, aEventServer);
        }

        public void Open()
        {
            iServiceSender.EventStateMetadata += EventStateMetadataResponse;
            iServiceSender.EventStateStatus += EventStateStatusResponse;
            iServiceSender.EventStateAudio += EventStateAudioResponse;
            iServiceSender.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceSender.EventInitial += EventInitialResponse;
        }

        public void Close()
        {
            iServiceSender.EventStateMetadata -= EventStateMetadataResponse;
            iServiceSender.EventStateStatus -= EventStateStatusResponse;
            iServiceSender.EventStateAudio -= EventStateAudioResponse;
            iServiceSender.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceSender.EventInitial -= EventInitialResponse;
        }

        public EventHandler<EventArgs> EventSenderInitialised;

        public EventHandler<EventArgs> EventAudioChanged;
        public EventHandler<EventArgs> EventEnabledChanged;
        public EventHandler<EventArgs> EventMetadataChanged;
        public EventHandler<EventArgs> EventStatusChanged;

        public Sender Sender
        {
            get
            {
                return iSender;
            }
        }

        public string Room
        {
            get
            {
                lock (this)
                {
                    return iRoom;
                }
            }
        }

        public string Name
        {
            get
            {
                lock (this)
                {
                    return iName;
                }
            }
        }

        public string FullName
        {
            get
            {
                lock (this)
                {
                    return iFullName;
                }
            }
        }

        public string Udn
        {
            get
            {
                return iSender.Device.Udn;
            }
        }

        public bool Audio
        {
            get
            {
                return iServiceSender.Audio;
            }
        }

        public EStatus Status
        {
            get
            {
                lock (this)
                {
                    return iStatus;
                }
            }
        }

        public DidlLite Metadata
        {
            get
            {
                lock (this)
                {
                    return iMetadata;
                }
            }
        }

        private void EventInitialResponse(object sender, EventArgs e)
        {
            if (EventSenderInitialised != null)
            {
                EventSenderInitialised(this, EventArgs.Empty);
            }
        }

        private void EventStateStatusResponse(object sender, EventArgs e)
        {
            lock (this)
            {
                if (iServiceSender.Status == "Sending")
                {
                    iStatus = EStatus.eSending;
                }
                else if (iServiceSender.Status == "Ready")
                {
                    iStatus = EStatus.eReady;
                }
                else if (iServiceSender.Status == "Blocked")
                {
                    iStatus = EStatus.eBlocked;
                }
                else if (iServiceSender.Status == "Inactive")
                {
                    iStatus = EStatus.eInactive;
                }
                else if (iServiceSender.Status == "Disabled")
                {
                    iStatus = EStatus.eDisabled;
                }
                else
                {
                    iStatus = EStatus.eUnknown;
                }
            }

            if (EventStatusChanged != null)
            {
                EventStatusChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateMetadataResponse(object sender, EventArgs e)
        {
            lock (this)
            {
                try
                {
                    iMetadata = new DidlLite(iServiceSender.Metadata);
                    string metadata = (iMetadata.Count > 0) ? DidlLiteAdapter.Title(iMetadata[0]) : "Unknown";
                    ParseName(metadata, out iRoom, out iName);
                    iFullName = string.Format("{0} ({1})", iRoom, iName);
                }
                catch (Exception ex)
                {
                    UserLog.WriteLine("Logging for ticket #1125: " + ex);
                    UserLog.WriteLine("Logging for ticket #1125: " + iServiceSender.Metadata);
                    iMetadata = null;
                }

            }

            if (EventMetadataChanged != null)
            {
                EventMetadataChanged(this, EventArgs.Empty);
            }
        }

        private static bool ParseBrackets(string aMetadata, out string aRoom, out string aName, char aOpen, char aClose)
        {
            int open = aMetadata.IndexOf(aOpen);

            if (open >= 0)
            {
                int close = aMetadata.IndexOf(aClose);

                if (close > -0)
                {
                    int bracketed = close - open - 1;

                    if (bracketed > 1)
                    {
                        aRoom = aMetadata.Substring(0, open).Trim();
                        aName = aMetadata.Substring(open + 1, bracketed).Trim();
                        return (true);
                    }
                }
            }

            aRoom = aMetadata;
            aName = aMetadata;

            return (false);
        }

        private void ParseName(string aMetadata, out string aRoom, out string aName)
        {
            if (ParseBrackets(aMetadata, out aRoom, out aName, '(', ')'))
            {
                return;
            }

            if (ParseBrackets(aMetadata, out aRoom, out aName, '[', ']'))
            {
                return;
            }

            if (ParseBrackets(aMetadata, out aRoom, out aName, '<', '>'))
            {
                return;
            }

            int index = aMetadata.IndexOf(':');

            if (index < 0)
            {
                index = aMetadata.IndexOf('.');
            }

            if (index < 0)
            {
                aRoom = aMetadata;
                aName = aMetadata;
                return;
            }

            string temp = aMetadata.Substring(0, index).Trim();
            aName = aMetadata.Substring(index + 1).Trim();

            index = temp.IndexOf('.');
            if(index < 0)
            {
                aRoom = temp;
            }
            else
            {
                aRoom = temp.Substring(index + 1).Trim();
            }
        }
        
        private void EventStateAudioResponse(object sender, EventArgs e)
        {
            if(EventAudioChanged!=null)
            {
                EventAudioChanged(this, EventArgs.Empty);
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

        private Sender iSender;

        private EStatus iStatus;
        private DidlLite iMetadata;

        private string iRoom;
        private string iName;
        private string iFullName;

        private ServiceSender iServiceSender;
    }
}