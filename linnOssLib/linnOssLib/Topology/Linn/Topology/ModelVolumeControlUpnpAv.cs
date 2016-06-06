using System;
using Linn.ControlPoint.Upnp;
using Linn.ControlPoint;
using System.Xml;

namespace Linn.Topology
{
    public class ModelVolumeControlUpnpAv : ModelVolumeControl, IVolumeLimiterControl
    {
        public ModelVolumeControlUpnpAv(Preamp aPreamp)
        {
            iPreamp = aPreamp;
            iInstanceId = 0;

            iVolumeLimiter = new VolumeLimiter(this);

            try
            {
                iServiceRenderingControl = new ServiceRenderingControl(aPreamp.Device, aPreamp.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionSetMute = iServiceRenderingControl.CreateAsyncActionSetMute();
            iActionSetVolume = iServiceRenderingControl.CreateAsyncActionSetVolume();
        }

        public override void Open()
        {
            iVolumeLimiter.Start();

            iServiceRenderingControl.EventStateLastChange += EventStateLastChangeResponse;
            iServiceRenderingControl.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceRenderingControl.EventInitial += EventInitialResponse;
        }

        public override void Close()
        {
            iVolumeLimiter.Stop();

            iServiceRenderingControl.EventStateLastChange -= EventStateLastChangeResponse;
            iServiceRenderingControl.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceRenderingControl.EventInitial -= EventInitialResponse;
        }

        public override string Name
        {
            get
            {
                return iPreamp.Type;
            }
        }

        public override Device Device
        {
            get { return iPreamp.Device; }
        }

        private void EventInitialResponse(object sender, EventArgs e)
        {
            if (EventVolumeLimitChanged != null)
            {
                EventVolumeLimitChanged(this, EventArgs.Empty);
            }

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
            //iActionSetVolume.SetVolumeBegin(iInstanceId, ServiceRenderingControl.kChannelMaster, iVolume + 1);
        }

        public override void DecrementVolume()
        {
            iVolumeLimiter.DecrementVolume();
            //iActionSetVolume.SetVolumeBegin(iInstanceId, ServiceRenderingControl.kChannelMaster, iVolume - 1);
        }

        public override void SetVolume(uint aValue)
        {
            iActionSetVolume.SetVolumeBegin(iInstanceId, ServiceRenderingControl.kChannelMaster, aValue);
        }

        public override void ToggleMute()
        {
            iActionSetMute.SetMuteBegin(iInstanceId, ServiceRenderingControl.kChannelMaster, !iMute);
        }

        public override void SetMute(bool aValue)
        {
            iActionSetMute.SetMuteBegin(iInstanceId, ServiceRenderingControl.kChannelMaster, aValue);
        }

        public override uint Volume
        {
            get
            {
                return iVolume;
            }
        }

        public override bool Mute
        {
            get
            {
                return iMute;
            }
        }

        public override uint VolumeLimit
        {
            get
            {
                return 100;
            }
        }

        void IVolumeLimiterControl.IncrementVolume()
        {
            iServiceRenderingControl.SetVolumeSync(iInstanceId, ServiceRenderingControl.kChannelMaster, iVolume + 1);
        }

        void IVolumeLimiterControl.DecrementVolume()
        {
            iServiceRenderingControl.SetVolumeSync(iInstanceId, ServiceRenderingControl.kChannelMaster, iVolume - 1);
        }

        private void EventStateLastChangeResponse(object sender, EventArgs e)
        {
            try
            {
                string lastChange = iServiceRenderingControl.LastChange;
                if (lastChange != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(lastChange);
                    XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(xmlDoc.NameTable);
                    xmlNsMan.AddNamespace("ns", "urn:schemas-upnp-org:metadata-1-0/RCS/");

                    XmlNodeList eventList = xmlDoc.DocumentElement.SelectNodes("/ns:Event/ns:InstanceID", xmlNsMan);
                    foreach (XmlNode instance in eventList)
                    {
                        XmlNode val = instance.SelectSingleNode("@val", xmlNsMan);
                        if (val != null)
                        {
                            if (uint.Parse(val.Value, System.Globalization.CultureInfo.InvariantCulture) == iInstanceId)
                            {
                                XmlNodeList stateVariable = instance.SelectNodes("ns:Mute", xmlNsMan);
                                for (int i = 0; i < stateVariable.Count; ++i)
                                {
                                    val = stateVariable[i].SelectSingleNode("@channel", xmlNsMan);
                                    if (val != null)
                                    {
                                        string channel = val.Value;
                                        Trace.WriteLine(Trace.kPreamp, "LastEventParser: channel " + channel);
                                        val = stateVariable[i].SelectSingleNode("@val", xmlNsMan);
                                        if (val != null && channel == "Master")
                                        {
                                            bool mute;
                                            try
                                            {
                                                mute = bool.Parse(val.Value);
                                            }
                                            catch (FormatException)
                                            {
                                                mute = (val.Value == "1");
                                            }

                                            iMute = mute;
                                            Trace.WriteLine(Trace.kPreamp, "ModelVolumeControlUpnpAv.LastEventParser: mute changed to " + iMute);

                                            if (EventMuteStateChanged != null)
                                            {
                                                EventMuteStateChanged(this, EventArgs.Empty);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        UserLog.WriteLine("ModelVolumeControlUpnpAv.EventStateLastChangeResponse: Invalid element Mute: " + stateVariable[i].OuterXml);
                                    }
                                }

                                stateVariable = instance.SelectNodes("ns:Volume", xmlNsMan);
                                for (int i = 0; i < stateVariable.Count; ++i)
                                {
                                    val = stateVariable[i].SelectSingleNode("@channel", xmlNsMan);
                                    if (val != null)
                                    {
                                        string channel = val.Value;
                                        Trace.WriteLine(Trace.kPreamp, "LastEventParser: channel " + channel);
                                        val = stateVariable[i].SelectSingleNode("@val", xmlNsMan);
                                        if (val != null && channel == "Master")
                                        {
                                            try
                                            {
                                                iVolume = uint.Parse(val.Value, System.Globalization.CultureInfo.InvariantCulture);
                                                Trace.WriteLine(Trace.kPreamp, "LastEventParser: volume changed to " + iVolume);

                                                if (EventVolumeChanged != null)
                                                {
                                                    EventVolumeChanged(this, EventArgs.Empty);
                                                }
                                            }
                                            catch (FormatException) { }
                                        }
                                    }
                                    else
                                    {
                                        UserLog.WriteLine("ModelVolumeControlUpnpAv.EventStateLastChangeResponse: Invalid element Volume: " + stateVariable[i].OuterXml);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string lastChange = iServiceRenderingControl.LastChange != null ? iServiceRenderingControl.LastChange : "";
                Trace.WriteLine(Trace.kMediaRenderer, "ModelVolumeControlUpnpAv.EventStateLastChangeResponse: " + ex.Message + "\nLastChange: " + lastChange);
                UserLog.WriteLine("ModelVolumeControlUpnpAv.EventStateLastChangeResponse: " + ex.Message + "\nLastChange: " + lastChange);
                return;
            }
        }

        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            OnEventSubscriptionError();
        }

        private const string kNotImplemented = "NOT_IMPLEMENTED";

        private Preamp iPreamp;

        private VolumeLimiter iVolumeLimiter;

        private ServiceRenderingControl iServiceRenderingControl;

        private ServiceRenderingControl.AsyncActionSetMute iActionSetMute;
        private ServiceRenderingControl.AsyncActionSetVolume iActionSetVolume;

        private uint iInstanceId;
        private bool iMute;
        private uint iVolume;
    }
} // Linn.Topology