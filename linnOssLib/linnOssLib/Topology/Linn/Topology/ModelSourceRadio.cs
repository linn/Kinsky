using System;
using System.Xml;
using System.Collections.Generic;

using Linn.ControlPoint.Upnp;
using Linn.ControlPoint;

using Upnp;

namespace Linn.Topology
{
    public class Channel
    {
        public Channel(string aUri, DidlLite aDidlLite)
        {
            iUri = aUri;
            iDidlLite = aDidlLite;
        }

        public string Uri
        {
            get
            {
                return iUri;
            }
        }

        public DidlLite DidlLite
        {
            get
            {
                return iDidlLite;
            }
        }

        private string iUri;
        private DidlLite iDidlLite;
    }

    public class RadioIdArray : IIdArray
    {
        public RadioIdArray(ServiceRadio aServiceRadio)
        {
            iServiceRadio = aServiceRadio;
            iServiceRadio.EventStateIdArray += EventStateIdArrayResponse;
        }

        public string Read(uint aId)
        {
            return iServiceRadio.ReadSync(aId);
        }

        public string ReadList(string aIdList)
        {
            return iServiceRadio.ReadListSync(aIdList);
        }

        public MrItem Default
        {
            get
            {
                return kEmptyPreset;
            }
        }

        public IList<MrItem> ParseMetadataXml(string aXml)
        {
            List<MrItem> list = new List<MrItem>();

            if (aXml != null)
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(aXml);

                    foreach (XmlNode n in document.SelectNodes("/ChannelList/Entry"))
                    {
                        uint id = uint.Parse(n["Id"].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        string metadata = n["Metadata"].InnerText;
                        DidlLite didl = null;
                        if (id > 0)
                        {
                            try
                            {
                                didl = new DidlLite(metadata);
                                if (didl[0].Res.Count > 0)
                                {
                                    string uri = didl[0].Res[0].Uri;
                                    list.Add(new MrItem(id, uri, didl));
                                }
                            }
                            catch(XmlException e)
                            {
                                UserLog.WriteLine("RadioIdArray.ParseMetadataXml: " + e.Message + " whilst parsing " + metadata);
                                didl = new DidlLite();
                                item item = new item();
                                item.Id = id.ToString();
                                item.Title = "Metadata error";
                                didl.Add(item);

                                list.Add(new MrItem(id, null, didl));
                            }
                        }
                    }
                }
                catch (XmlException e) { Console.WriteLine(e); }
                catch (FormatException e) { Console.WriteLine(e); }
            }

            return list;
        }

        public event EventHandler<EventArgs> EventIdArray;

        private void EventStateIdArrayResponse(object sender, EventArgs e)
        {
            if (EventIdArray != null)
            {
                EventIdArray(this, EventArgs.Empty);
            }
        }

        public static readonly MrItem kEmptyPreset = new MrItem(0, null, new DidlLite("<DidlLite xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\"><item><dc:title>Empty</dc:title><upnp:class>object.item.audioItem</upnp:class></item></DidlLite>"));

        private ServiceRadio iServiceRadio;
    }

    public interface IModelSourceRadio : IModelSource
    {
        event EventHandler<EventArgs> EventControlInitialised;
        void EventInitialResponseRadio(object sender, EventArgs e);
        event EventHandler<EventArgs> EventPresetChanged;
        event EventHandler<EventArgs> EventPresetsChanged;
        void EventSetChannelResponse(object sender, EventArgs e);
        event EventHandler<EventArgs> EventSetId;
        event EventHandler<EventArgs> EventTransportStateChanged;
        event EventHandler<EventArgs> EventChannelChanged;
        void Lock();
        bool Next();
        void Pause();
        void Play();
        void PlayNow(DidlLite aDidlLite);
        MrItem Preset(uint aIndex);
        int PresetIndex { get; }
        bool Previous();
        string ProtocolInfo { get; }
        void SeekSeconds(uint aSeconds);
        void SetChannel(DidlLite aDidlLite);
        bool SetPreset(MrItem aPreset);
        void Stop();
        ModelSourceRadio.ETransportState TransportState { get; }
        void Unlock();
        Channel Channel { get; }
        event EventHandler<EventArgs> EventSubscriptionError;
    }

    public class ModelSourceRadio : ModelSource, IMediaSupported, IModelSourceRadio
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public enum ETransportState
        {
            eUnknown,
            ePlaying,
            eStopped,
            eBuffering
        }

        public ModelSourceRadio(Source aSource)
        {
            iSource = aSource;

            try
            {
                iServiceRadio = new ServiceRadio(iSource.Device, iSource.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionPlay = iServiceRadio.CreateAsyncActionPlay();
            iActionPause = iServiceRadio.CreateAsyncActionPause();
            iActionStop = iServiceRadio.CreateAsyncActionStop();
            iActionSetId = iServiceRadio.CreateAsyncActionSetId();
            iActionSetChannel = iServiceRadio.CreateAsyncActionSetChannel();
            iActionSeekSecondAbsolute = iServiceRadio.CreateAsyncActionSeekSecondAbsolute();

            iActionPlayNowSetChannel = iServiceRadio.CreateAsyncActionSetChannel();
            iActionPlayNowSetChannel.EventResponse += EventSetChannelResponse;

            iIdArray = new ModelIdArray(new RadioIdArray(iServiceRadio));
        }

        public override void Open()
        {
            iIdArray.EventIdArrayChanged += EventIdArrayChanged;
            iIdArray.Open();

            iActionSetId.EventResponse += EventActionSetIdResponse;

            iServiceRadio.EventStateIdArray += EventIdArrayResponse;
            iServiceRadio.EventStateTransportState += EventTransportStateResponse;
            iServiceRadio.EventStateId += EventCurrentChannelIdResponse;
            iServiceRadio.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceRadio.EventInitial += EventInitialResponseRadio;
            iServiceRadio.EventStateUri += EventStateUriResponse;
            iServiceRadio.EventStateMetadata += EventStateMetadataResponse;
        }

        public override void Close()
        {
            iIdArray.EventIdArrayChanged -= EventIdArrayChanged;
            iIdArray.Close();

            iActionSetId.EventResponse -= EventActionSetIdResponse;

            iServiceRadio.EventStateIdArray -= EventIdArrayResponse;
            iServiceRadio.EventStateTransportState -= EventTransportStateResponse;
            iServiceRadio.EventStateId -= EventCurrentChannelIdResponse;
            iServiceRadio.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceRadio.EventInitial -= EventInitialResponseRadio;
            iServiceRadio.EventStateUri -= EventStateUriResponse;
            iServiceRadio.EventStateMetadata -= EventStateMetadataResponse;
        }

        public override string Name
        {
            get
            {
                return iSource.FullName;
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
            return BestSupportedResource(iServiceRadio.ProtocolInfo, aObject);
        }

        public void Lock()
        {
            iIdArray.Lock();
        }

        public void Unlock()
        {
            iIdArray.Unlock();
        }

        public void EventInitialResponseRadio(object sender, EventArgs e)
        {
            if (EventControlInitialised != null)
            {
                EventControlInitialised(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventControlInitialised;

        public event EventHandler<EventArgs> EventTransportStateChanged;

        public event EventHandler<EventArgs> EventPresetChanged;
        public event EventHandler<EventArgs> EventPresetsChanged;
        public event EventHandler<EventArgs> EventChannelChanged;

        public event EventHandler<EventArgs> EventSetId;

        public void PlayNow(DidlLite aDidlLite)
        {
            SetChannel(aDidlLite, iActionPlayNowSetChannel);
        }

        public MrItem Preset(uint aIndex)
        {
            Lock();
            MrItem item = iIdArray.AtIndex(aIndex);
            Unlock();

            return item;
        }

        public bool SetPreset(MrItem aPreset)
        {
            /*Lock();
            int index = iIdArray.Index(aPreset.Id);
            Unlock();*/

            if (aPreset.DidlLite[0].Res.Count > 0)
            {
                string uri = aPreset.DidlLite[0].Res[0].Uri;
                iActionSetId.SetIdBegin(aPreset.Id, uri);
                return true;
            }

            return false;
        }

        public void SetChannel(DidlLite aDidlLite)
        {
            SetChannel(aDidlLite, iActionSetChannel);
        }

        private void SetChannel(DidlLite aDidlLite, ServiceRadio.AsyncActionSetChannel aAction)
        {
            if (aDidlLite.Count > 0)
            {
                upnpObject o = aDidlLite[0];
                if (o.Res.Count > 0)
                {
                    string uri = o.Res[0].Uri;
                    DidlLite didl = new DidlLite();
                    didl.Add(o);
                    aAction.SetChannelBegin(uri, didl.Xml);
                }
            }
        }

        public void Play()
        {
            iActionPlay.PlayBegin();
        }

        public void Pause()
        {
            iActionPause.PauseBegin();
        }

        public void Stop()
        {
            iActionStop.StopBegin();
        }

        public bool Previous()
        {
            bool result = false;

            Lock();

            if (iPresetIndex != -1)
            {
                int index = iPresetIndex - 1;
                while (index > -1)
                {
                    MrItem preset = Preset((uint)index);
                    if (preset.Id != 0)
                    {
                        SetPreset(preset);
                        result = true;
                        break;
                    }
                    else
                    {
                        index--;
                    }
                }
            }

            Unlock();

            return result;
        }

        public bool Next()
        {
            bool result = false;

            Lock();

            if (iPresetIndex != -1)
            {
                int index = iPresetIndex + 1;
                while (index < kMaxPresets)
                {
                    MrItem preset = Preset((uint)index);
                    if (preset.Id != 0)
                    {
                        SetPreset(preset);
                        result = true;
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            Unlock();

            return result;
        }

        public void SeekSeconds(uint aSeconds)
        {
            iActionSeekSecondAbsolute.SeekSecondAbsoluteBegin(aSeconds);
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
                return iServiceRadio.ProtocolInfo;
            }
        }

        public int PresetIndex
        {
            get
            {
                return iPresetIndex;
            }
        }

        public Channel Channel
        {
            get
            {
                return iChannel;
            }
        }

        private void EventActionSetIdResponse(object sender, ServiceRadio.AsyncActionSetId.EventArgsResponse e)
        {
            if (EventSetId != null)
            {
                EventSetId(this, EventArgs.Empty);
            }
        }

        private void EventIdArrayResponse(object sender, EventArgs e)
        {
            iIdArray.SetIdArray(ByteArray.Unpack(iServiceRadio.IdArray));
        }

        private void EventTransportStateResponse(object sender, EventArgs e)
        {
            if (iServiceRadio.TransportState == "Playing")
            {
                iTransportState = ETransportState.ePlaying;
            }
            else if (iServiceRadio.TransportState == "Stopped")
            {
                iTransportState = ETransportState.eStopped;
            }
            else if (iServiceRadio.TransportState == "Buffering")
            {
                iTransportState = ETransportState.eBuffering;
            }
            else
            {
                iTransportState = ETransportState.eUnknown;
            }

            if (EventTransportStateChanged != null)
            {
                EventTransportStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateUriResponse(object sender, EventArgs e)
        {
            iChannel = new Channel(iServiceRadio.Uri, string.IsNullOrEmpty(iServiceRadio.Metadata) ? new DidlLite() : new DidlLite(iServiceRadio.Metadata));

            if (EventChannelChanged != null)
            {
                EventChannelChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateMetadataResponse(object sender, EventArgs e)
        {
            iChannel = new Channel(iServiceRadio.Uri, string.IsNullOrEmpty(iServiceRadio.Metadata) ? new DidlLite() : new DidlLite(iServiceRadio.Metadata));

            if (EventChannelChanged != null)
            {
                EventChannelChanged(this, EventArgs.Empty);
            }
        }

        private void EventCurrentChannelIdResponse(object sender, EventArgs e)
        {
            SetPresetIndex();

            if (EventPresetChanged != null)
            {
                EventPresetChanged(this, EventArgs.Empty);
            }
        }

        public void EventSetChannelResponse(object sender, EventArgs e)
        {
            Play();
        }

        private void EventIdArrayChanged(object sender, EventArgs e)
        {
            Trace.WriteLine(Trace.kTopology, "ModelSourceRadio.EventIdArrayChanged");

            SetPresetIndex();

            if (EventPresetChanged != null)
            {
                EventPresetChanged(this, EventArgs.Empty);
            }

            if (EventPresetsChanged != null)
            {
                EventPresetsChanged(this, EventArgs.Empty);
            }
        }

        private void SetPresetIndex()
        {
            Lock();

            uint id = iServiceRadio.Id;
            if (id == 0)
            {
                iPresetIndex = -1;
            }
            else
            {
                iPresetIndex = iIdArray.Index(id);
            }

            Unlock();
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

        public const uint kMaxPresets = 100;

        private Source iSource;

        private ServiceRadio iServiceRadio;
        private ServiceRadio.AsyncActionPlay iActionPlay;
        private ServiceRadio.AsyncActionPause iActionPause;
        private ServiceRadio.AsyncActionStop iActionStop;
        private ServiceRadio.AsyncActionSetId iActionSetId;
        private ServiceRadio.AsyncActionSetChannel iActionSetChannel;
        private ServiceRadio.AsyncActionSetChannel iActionPlayNowSetChannel;
        private ServiceRadio.AsyncActionSeekSecondAbsolute iActionSeekSecondAbsolute;

        private ModelIdArray iIdArray;

        private ETransportState iTransportState;
        private int iPresetIndex;
        private Channel iChannel;
    }
} // Linn.Topology
