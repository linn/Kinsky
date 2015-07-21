using System;
using System.Collections.Generic;
using System.Xml;
using System.Threading;

using Upnp;

using Linn;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class ModelSourceMediaRendererUpnpAv : ModelSourceMediaRenderer, IModelInfo, IModelTime
    {
        public ModelSourceMediaRendererUpnpAv(Source aSource)
        {
            DidlLite didl = new DidlLite();
            audioItem itemAudio = new audioItem();
            itemAudio.Title = "Unknown";
            didl.Add(itemAudio);
            kUnknownPlaylistItem = new MrItem(0, null, didl);

            iSource = aSource;
            iInstanceId = 0;
            iTrackId = 0;

            try
            {
                iServiceConnectionManager = new ServiceConnectionManager(iSource.Device, iSource.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionGetProtocolInfo = iServiceConnectionManager.CreateAsyncActionGetProtocolInfo();

            iActionGetProtocolInfo.EventResponse += EventResponseGetProtocolInfo;

            try
            {
                iServiceAVTransport = new ServiceAVTransport(iSource.Device, iSource.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionPlay = iServiceAVTransport.CreateAsyncActionPlay();
            iActionPause = iServiceAVTransport.CreateAsyncActionPause();
            iActionStop = iServiceAVTransport.CreateAsyncActionStop();
            iActionSeek = iServiceAVTransport.CreateAsyncActionSeek();
            iActionNext = iServiceAVTransport.CreateAsyncActionNext();
            iActionPrevious = iServiceAVTransport.CreateAsyncActionPrevious();
            iActionSetAVTransportURI = iServiceAVTransport.CreateAsyncActionSetAVTransportURI();
            iActionGetPositionInfo = iServiceAVTransport.CreateAsyncActionGetPositionInfo();
            iActionGetTransportSettings = iServiceAVTransport.CreateAsyncActionGetTransportSettings();
            iActionSetPlayMode = iServiceAVTransport.CreateAsyncActionSetPlayMode();

            iActionGetPositionInfo.EventResponse += EventResponseGetPositionInfo;
            iActionGetTransportSettings.EventResponse += EventResponseGetTransportSettings;
            iActionSetAVTransportURI.EventResponse += EventResponseSetAVTransportURI;

            iTimer = new Linn.Timer();

            // Sets the timer interval to 1 second.
            iTimer.Interval = 1000;
            iTimer.Elapsed += TimerElapsed;
            iTimer.AutoReset = false;

            iMutex = new Mutex(false);
            iPlaylist = new List<MrItem>();
        }

        public override resource BestSupportedResource(upnpObject aObject)
        {
            return BestSupportedResource(iProtocolInfo, aObject);
        }

        public override void Open()
        {
            try
            {
                Lock();
                if (!iOpen)
                {
                    iServiceAVTransport.EventStateLastChange += EventStateLastChangeResponse;
                    iServiceAVTransport.EventSubscriptionError += EventSubscriptionErrorHandler;
                    iServiceAVTransport.EventInitial += EventInitialResponse;

                    iInitialised = false;
                    iMaster = false;
                    iExpectEventStop = false;
                    iExpectedTrack = null;

                    iTrackBitrate = 0;
                    iTrackLossless = false;
                    iTrackBitDepth = 0;
                    iTrackSampleRate = 0;
                    iTrackCodecName = string.Empty;
                    iOpen = true;
                }
                else if(iInitialised)
                {
                    OnInitialised();
                }
            }
            finally
            {
                Unlock();
            }
        }

        public override void Close()
        {
            try
            {
                Lock();
                if (iOpen)
                {
                    iTimer.Stop();

                    iServiceAVTransport.EventStateLastChange -= EventStateLastChangeResponse;
                    iServiceAVTransport.EventSubscriptionError -= EventSubscriptionErrorHandler;
                    iServiceAVTransport.EventInitial -= EventInitialResponse;
                    iOpen = false;
                    iInitialised = false;
                }
            }
            finally
            {
                Unlock();
            }
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

        public Device Device
        {
            get
            {
                return iSource.Device;
            }
        }

        private void EventInitialResponse(object sender, EventArgs e)
        {
            iActionGetProtocolInfo.GetProtocolInfoBegin();
        }

        private event EventHandler<EventArgs> iEventInitialisedInfo;
        event EventHandler<EventArgs> IModelInfo.EventInitialised
        {
            add
            {
                iEventInitialisedInfo += value;
            }
            remove
            {
                iEventInitialisedInfo -= value;
            }
        }

        private event EventHandler<EventArgs> iEventTrackChanged;
        event EventHandler<EventArgs> IModelInfo.EventTrackChanged
        {
            add
            {
                iEventTrackChanged += value;
            }
            remove
            {
                iEventTrackChanged -= value;
            }
        }

        public event EventHandler<EventArgs> EventMetaTextChanged;
        public event EventHandler<EventArgs> EventDetailsChanged;

        private event EventHandler<EventArgs> iEventInitialisedTime;
        event EventHandler<EventArgs> IModelTime.EventInitialised
        {
            add
            {
                iEventInitialisedTime += value;
            }
            remove
            {
                iEventInitialisedTime -= value;
            }
        }

        public event EventHandler<EventArgs> EventSecondsChanged;
        public event EventHandler<EventArgs> EventDurationChanged;

        public override event EventHandler<EventArgs> EventControlInitialised;
        public override event EventHandler<EventArgs> EventPlaylistInitialised;

        public override event EventHandler<EventArgs> EventTransportStateChanged;
        public override event EventHandler<EventArgs> EventTrackChanged;
        public override event EventHandler<EventArgs> EventRepeatChanged;
        public override event EventHandler<EventArgs> EventShuffleChanged;

        public override event EventHandler<EventArgs> EventPlaylistChanged;

        public void TimerElapsed(object aSender, EventArgs aArgs)
        {
            iActionGetPositionInfo.GetPositionInfoBegin(iInstanceId);
        }

        private void EventResponseGetProtocolInfo(object obj, ServiceConnectionManager.AsyncActionGetProtocolInfo.EventArgsResponse e)
        {
            UserLog.WriteLine(e.Sink);
            iProtocolInfo = e.Sink;
            if (Device.Model == "WD TV Live Hub" || Device.Model == "WD TV Live")
            {
                iProtocolInfo += ",http-get:*:audio/x-flac:*,http-get:*:audio/mp4:*,http-get:*:audio/wav:*,http-get:*:video/quicktime:*,http-get:*:video/x-matroska:*";
            }
            if (Device.Model == "AVR-3808")
            {
                iProtocolInfo += ",http-get:*:audio/x-flac:*";
            }

            try
            {
                Lock();

                iInitialised = true;
                OnInitialised();
            }
            finally
            {
                Unlock();
            }

            iActionGetPositionInfo.GetPositionInfoBegin(iInstanceId);
        }

        private void OnInitialised()
        {
            if (iEventInitialisedTime != null)
            {
                iEventInitialisedTime(this, EventArgs.Empty);
            }

            if (iEventInitialisedInfo != null)
            {
                iEventInitialisedInfo(this, EventArgs.Empty);
            }

            if (EventTransportStateChanged != null)
            {
                EventTransportStateChanged(this, EventArgs.Empty);
            }

            if (EventControlInitialised != null)
            {
                EventControlInitialised(this, EventArgs.Empty);
            }

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }

            if (EventTrackChanged != null)
            {
                EventTrackChanged(this, EventArgs.Empty);
            }

            if (iEventTrackChanged != null)
            {
                iEventTrackChanged(this, EventArgs.Empty);
            }

            if (EventMetaTextChanged != null)
            {
                EventMetaTextChanged(this, EventArgs.Empty);
            }

            if (EventDetailsChanged != null)
            {
                EventDetailsChanged(this, EventArgs.Empty);
            }

            if (EventPlaylistInitialised != null)
            {
                EventPlaylistInitialised(this, EventArgs.Empty);
            }

            if (EventRepeatChanged != null)
            {
                EventRepeatChanged(this, EventArgs.Empty);
            }

            if (EventShuffleChanged != null)
            {
                EventShuffleChanged(this, EventArgs.Empty);
            }
        }

        private void EventResponseGetPositionInfo(object obj, ServiceAVTransport.AsyncActionGetPositionInfo.EventArgsResponse e)
        {
            iTimer.Start();

            try
            {
                Time elapsed = new Time(e.RelTime);
                Assert.CheckDebug(elapsed.SecondsTotal >= 0);
                iTrackElapsed = (elapsed.SecondsTotal >= 0) ? (uint)elapsed.SecondsTotal : 0;

                if (EventSecondsChanged != null)
                {
                    EventSecondsChanged(this, EventArgs.Empty);
                }
            }
            catch (Time.TimeInvalid)
            {
                iTrackElapsed = 0;
            }
        }

        private void EventResponseGetTransportSettings(object obj, ServiceAVTransport.AsyncActionGetTransportSettings.EventArgsResponse e)
        {
            if (e.PlayMode == "")
            {
            }
        }

        public uint Seconds
        {
            get
            {
                return iTrackElapsed;
            }
        }

        public override void Play()
        {
            if (iTrackPlaylistItem == null && iPlaylist.Count > 0)
            {
                SeekTrack(0);
            }
            else
            {
                try
                {
                    Lock();

                    iTrackIndex = iPlaylist.IndexOf(iTrackPlaylistItem);
                    if (iTrackIndex != -1)
                    {
                        // set this ahead of time to make for better interaction (will be corrected later if needed!)
                        iMaster = false;
                        iExpectedTrack = iTrackPlaylistItem;
                        iExpectEventStop = false;
                    }
                }
                finally
                {
                    Unlock();
                }

                iActionPlay.PlayBegin(iInstanceId, "1");
            }
        }

        public override void Pause()
        {
            iActionPause.PauseBegin(iInstanceId);
        }

        public override void Stop()
        {
            iExpectEventStop = true;
            iActionStop.StopBegin(iInstanceId);
        }

        public override void Previous()
        {
            if (iMaster)
            {
                try
                {
                    Lock();

                    bool looped = false;
                    int index = iTrackIndex - 1;
                    if (index < 0)
                    {
                        looped = true;
                        index = iPlaylist.Count - 1;
                    }

                    if ((looped && iRepeat) || !looped)
                    {
                        if (index < iPlaylist.Count)
                        {
                            SeekTrack((uint)index);
                            // set this ahead of time to make for better interaction (will be corrected later if needed!)
                            //iTrackIndex = index;
                        }
                    }
                }
                finally
                {
                    Unlock();
                }
            }
            else
            {
                iActionPrevious.PreviousBegin(iInstanceId);
            }
        }

        public override void Next()
        {
            if (iMaster)
            {
                try
                {
                    Lock();

                    bool looped = false;
                    int index = iTrackIndex + 1;

                    if (index >= iPlaylist.Count)
                    {
                        looped = true;
                        index = 0;
                    }

                    if ((looped && iRepeat) || !looped)
                    {
                        if (index < iPlaylist.Count)
                        {
                            SeekTrack((uint)index);
                            // set this ahead of time to make for better interaction (will be corrected later if needed!)
                            //iTrackIndex = index;
                        }
                    }
                }
                finally
                {
                    Unlock();
                }
            }
            else
            {
                iActionNext.NextBegin(iInstanceId);
            }
        }

        public override void SeekSeconds(uint aSeconds)
        {
            Time time = new Time((int)aSeconds);
            iActionSeek.SeekBegin(iInstanceId, ServiceAVTransport.kSeekModeRelTime, time.ToString());
        }

        public override void SeekTrack(uint aTrack)
        {
            MrItem track;
            try
            {
                Lock();

                track = iPlaylist[(int)aTrack];

                // set this ahead of time to make for better interaction (will be corrected later if needed!)
                iMaster = false;
                iExpectedTrack = track;
                iExpectEventStop = false;
                //SetCurrent(track);
                //iTrackIndex = (int)aTrack;
            }
            finally
            {
                Unlock();
            }

            iActionSetAVTransportURI.SetAVTransportURIBegin(iInstanceId, track.Uri, track.DidlLite.Xml);
            //iActionSeek.SeekBegin(iInstanceId, ServiceAVTransport.kSeekModeTrackNr, aTrack.ToString());
            //Play();
        }

        private void EventResponseSetAVTransportURI(object sender, ServiceAVTransport.AsyncActionSetAVTransportURI.EventArgsResponse e)
        {
            iActionPlay.PlayBegin(iInstanceId, "1");
        }

        public override void ToggleRepeat()
        {
            iRepeat = !iRepeat;
            if (EventRepeatChanged != null)
            {
                EventRepeatChanged(this, EventArgs.Empty);
            }

            /*if (iRepeat)
            {
                iActionSetPlayMode.SetPlayModeBegin(iInstanceId, ServiceAVTransport.kCurrentPlayModeNormal);
            }
            else
            {
                iActionSetPlayMode.SetPlayModeBegin(iInstanceId, ServiceAVTransport.kCurrentPlayModeRepeatAll);
            }*/
        }

        public override void ToggleShuffle()
        {
            iShuffle = !iShuffle;
            if (EventShuffleChanged != null)
            {
                EventShuffleChanged(this, EventArgs.Empty);
            }

            /*if (!iShuffle)
            {
                iActionSetPlayMode.SetPlayModeBegin(iInstanceId, ServiceAVTransport.kCurrentPlayModeNormal);
            }
            else
            {
                iActionSetPlayMode.SetPlayModeBegin(iInstanceId, ServiceAVTransport.kCurrentPlayModeShuffle);
            }*/
        }

        public override ETransportState TransportState
        {
            get
            {
                return (iTransportState);
            }
        }

        public override int TrackIndex
        {
            get
            {
                return iTrackIndex;
            }
        }

        public uint Duration
        {
            get
            {
                return (iTrackDuration);
            }
        }

        public uint Bitrate
        {
            get
            {
                return iTrackBitrate;
            }
        }

        public bool Lossless
        {
            get
            {
                return iTrackLossless;
            }
        }

        public uint BitDepth
        {
            get
            {
                return iTrackBitDepth;
            }
        }

        public uint SampleRate
        {
            get
            {
                return iTrackSampleRate;
            }
        }

        public string CodecName
        {
            get
            {
                return iTrackCodecName;
            }
        }

        public DidlLite Metatext
        {
            get
            {
                return null;
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

        public override MrItem TrackPlaylistItem
        {
            get
            {
                lock (this)
                {
                    return iTrackPlaylistItem;
                }
            }
        }

        public override bool Repeat
        {
            get
            {
                return iRepeat;
            }
        }

        public override bool Shuffle
        {
            get
            {
                return iShuffle;
            }
        }

        public override string ProtocolInfo
        {
            get
            {
                return iProtocolInfo;
            }
        }

        public override void Lock()
        {
            iMutex.WaitOne();
        }

        public override void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        public override uint PlayNow(DidlLite aDidlLite)
        {
            uint count;
            try
            {
                Lock();

                int index = iPlaylist.Count;
                count = PlaylistInsert(index, aDidlLite);
                SeekTrack((uint)index);
            }
            finally
            {
                Unlock();
            }
            return count;
        }

        public override uint PlayNext(DidlLite aDidlLite)
        {
            uint count;
            try
            {
                Lock();

                count = PlaylistInsert(iTrackIndex + 1, aDidlLite);
            }
            finally
            {
                Unlock();
            }
            return count;
        }

        public override uint PlayLater(DidlLite aDidlLite)
        {
            uint count;
            try
            {
                Lock();
                count = PlaylistInsert(iPlaylist.Count, aDidlLite);
            }
            finally
            {
                Unlock();
            }

            return count;
        }

        public override MrItem PlaylistItem(uint aIndex)
        {
            MrItem result = kUnknownPlaylistItem;
            try
            {
                Lock();
                if (aIndex < iPlaylist.Count)
                {
                    result = iPlaylist[(int)aIndex];
                }
            }
            finally
            {
                Unlock();
            }

            return result;
        }

        public override void PlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems)
        {
            try
            {
                Lock();


                int index = 0;
                if (aInsertAfterId > 0)
                {
                    for (int i = 0; i < iPlaylist.Count; ++i)
                    {
                        if (iPlaylist[i].Id == aInsertAfterId)
                        {
                            index = i + 1;
                            break;
                        }
                    }
                }

                for (int i = 0; i < aPlaylistItems.Count; ++i)
                {
                    MrItem item = aPlaylistItems[i];
                    iPlaylist.Insert(index + i, new MrItem(++iTrackId, item.Uri, item.DidlLite));
                }

                foreach (MrItem i in aPlaylistItems)
                {
                    iPlaylist.Remove(i);
                }
            }
            finally
            {
                Unlock();
            }

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }
        }

        public override uint PlaylistInsert(uint aInsertAfterId, DidlLite aDidlLite)
        {
            uint count;
            try
            {
                Lock();

                int index = 0;
                if (aInsertAfterId > 0)
                {
                    for (int i = 0; i < iPlaylist.Count; ++i)
                    {
                        if (iPlaylist[i].Id == aInsertAfterId)
                        {
                            index = i + 1;
                            break;
                        }
                    }
                }

                count = PlaylistInsert(index, aDidlLite);
            }
            finally
            {
                Unlock();
            }

            return count;
        }

        private uint PlaylistInsert(int aIndex, DidlLite aDidlLite)
        {
            uint count = 0;
            try
            {
                Lock();

                int index = 0;
                foreach (upnpObject item in aDidlLite)
                {
                    resource resource = BestSupportedResource(item);
                    if (resource != null)
                    {
                        string uri = resource.Uri;
                        DidlLite didl = new DidlLite();
                        didl.Add(item);
                        iPlaylist.Insert(aIndex + index, new MrItem(++iTrackId, uri, didl));
                        ++count;
                        ++index;
                    }
                }
            }
            finally
            {
                Unlock();
            }

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }

            return count;
        }

        public override void PlaylistDelete(IList<MrItem> aPlaylistItems)
        {
            try
            {
                Lock();

                bool deletedPlaying = false;
                foreach (MrItem i in aPlaylistItems)
                {
                    iPlaylist.Remove(i);
                    if (iMaster)
                    {
                        if (i == iTrackPlaylistItem)
                        {
                            deletedPlaying = true;
                        }
                    }
                }

                if (deletedPlaying)
                {
                    if (iTrackIndex < iPlaylist.Count)
                    {
                        if (iTransportState == ETransportState.eStopped)
                        {
                            MrItem track = iPlaylist[iTrackIndex];
                            iExpectedTrack = track;
                            //SetCurrent(track);

                            iActionSetAVTransportURI.SetAVTransportURIBegin(iInstanceId, iTrackPlaylistItem.Uri, iTrackPlaylistItem.DidlLite.Xml);
                        }
                        else
                        {
                            SeekTrack((uint)iTrackIndex);
                        }
                    }
                    else
                    {
                        iExpectedTrack = null;
                        SetCurrent(null);
                        iTrackIndex = -1;

                        iActionSetAVTransportURI.SetAVTransportURIBegin(iInstanceId, string.Empty, string.Empty);
                    }
                }
            }
            finally
            {
                Unlock();
            }

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }
        }

        public override void PlaylistDeleteAll()
        {
            try
            {
                Lock();

                iPlaylist.Clear();

                if (iMaster)
                {
                    iExpectedTrack = null;
                    SetCurrent(null);
                    iTrackIndex = -1;

                    iActionSetAVTransportURI.SetAVTransportURIBegin(iInstanceId, string.Empty, string.Empty);
                }
            }
            finally
            {
                Unlock();
            }

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }
        }

        public override bool IsInserting()
        {
            throw new NotImplementedException();
        }

        public override uint PlaylistTrackCount
        {
            get
            {
                return (uint)iPlaylist.Count;
            }
        }

        public override uint PlaylistTracksMax
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        private void EventStateLastChangeResponse(object sender, EventArgs e)
        {
            try
            {
                string lastChange = iServiceAVTransport.LastChange;
                if (lastChange != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(lastChange);
                    XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(xmlDoc.NameTable);
                    xmlNsMan.AddNamespace("ns", "urn:schemas-upnp-org:metadata-1-0/AVT/");

                    XmlNodeList eventList = xmlDoc.DocumentElement.SelectNodes("/ns:Event/ns:InstanceID", xmlNsMan);
                    foreach (XmlNode n in eventList)
                    {
                        XmlNodeList valList = n.SelectNodes("@val", xmlNsMan);
                        foreach (XmlAttribute v in valList)
                        {
                            if (uint.Parse(v.Value, System.Globalization.CultureInfo.InvariantCulture) == iInstanceId)
                            {
                                XmlNode state = n.SelectSingleNode("ns:CurrentTrackURI", xmlNsMan);
                                string trackUri = string.Empty;
                                if (state != null)
                                {
                                    XmlNode val = state.SelectSingleNode("@val", xmlNsMan);
                                    if (val != null)
                                    {
                                        trackUri = val.Value;
                                    }
                                }
                                if (iExpectedTrack != null && trackUri == iExpectedTrack.Uri)
                                {
                                    iMaster = true;
                                }
                                else if (iTrackPlaylistItem == null || trackUri != iTrackPlaylistItem.Uri)
                                {
                                    iMaster = false;
                                }
                                state = n.SelectSingleNode("ns:CurrentTrackMetaData", xmlNsMan);
                                if (state != null)
                                {
                                    XmlNode val = state.SelectSingleNode("@val", xmlNsMan);
                                    if (val != null)
                                    {
                                        MrItem trackPlaylistItem = null;
                                        try
                                        {
                                            if (!iMaster)
                                            {
                                                DidlLite didl = new DidlLite(val.Value);
                                                if (didl.Count == 0)
                                                {
                                                    item item = new item();
                                                    item.Title = trackUri;

                                                    didl.Add(item);
                                                }
												// hack for sonos devices artwork (which specifies a relative path)
                                                if (didl[0].AlbumArtUri.Count > 0 && didl[0].AlbumArtUri[0] != string.Empty)
                                                {
                                                    string albumArt = didl[0].AlbumArtUri[0];
                                                    if (albumArt.StartsWith("/getaa?", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        try
                                                        {
                                                            Uri uri = new Uri(Device.Location);
                                                            string newUri = string.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, albumArt);
                                                            didl[0].AlbumArtUri[0] = newUri;
                                                        }
                                                        catch { }
                                                    }
                                                }
                                                trackPlaylistItem = new MrItem(++iTrackId, trackUri, didl);
                                                Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: uri=" + trackPlaylistItem.Uri + ", metadata=" + trackPlaylistItem.DidlLite.Xml);
                                            }
                                        }
                                        catch (XmlException)
                                        {
                                            DidlLite didl = new DidlLite();

                                            item item = new item();
                                            item.Title = trackUri;

                                            didl.Add(item);

                                            trackPlaylistItem = new MrItem(++iTrackId, trackUri, didl);

                                            Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: uri=" + trackUri + ", metadata=");
                                        }

                                        Trace.WriteLine(Trace.kTopology, "iMaster=" + iMaster + ", trackUri=" + trackUri + ", iTrackPlaylistItem.Uri=" + ((iTrackPlaylistItem == null) ? "" : iTrackPlaylistItem.Uri) + ", iExpectedUri=" + ((iExpectedTrack == null) ? "" : iExpectedTrack.Uri));

                                        bool changed = false;
                                        if (iExpectedTrack != null && trackUri == iExpectedTrack.Uri)
                                        {
                                            SetCurrent(iExpectedTrack);
                                            iTrackIndex = iPlaylist.IndexOf(iExpectedTrack);
                                            iExpectedTrack = null;
                                            changed = true;
                                        }
                                        else if (iTrackPlaylistItem == null || trackUri != iTrackPlaylistItem.Uri)
                                        {
                                            SetCurrent(trackPlaylistItem);
                                            iTrackIndex = -1;
                                            iExpectedTrack = null;
                                            changed = true;
                                        }

                                        if (changed)
                                        {
                                            if (EventTrackChanged != null)
                                            {
                                                EventTrackChanged(this, EventArgs.Empty);
                                            }

                                            if (iEventTrackChanged != null)
                                            {
                                                iEventTrackChanged(this, EventArgs.Empty);
                                            }

                                            if (EventMetaTextChanged != null)
                                            {
                                                EventMetaTextChanged(this, EventArgs.Empty);
                                            }

                                            if (EventDetailsChanged != null)
                                            {
                                                EventDetailsChanged(this, EventArgs.Empty);
                                            }
                                        }
                                    }
                                }

                                state = n.SelectSingleNode("ns:TransportState", xmlNsMan);
                                if (state != null)
                                {
                                    XmlNode val = state.SelectSingleNode("@val", xmlNsMan);
                                    if (val != null)
                                    {
                                        ETransportState transportState = ETransportState.eUnknown;
                                        if (val.Value == "PLAYING")
                                        {
                                            transportState = ETransportState.ePlaying;
                                        }
                                        else if (val.Value == "PAUSED_PLAYBACK")
                                        {
                                            transportState = ETransportState.ePaused;
                                        }
                                        else if (val.Value == "STOPPED")
                                        {
                                            transportState = ETransportState.eStopped;
                                        }
                                        else if (val.Value == "TRANSITIONING")
                                        {
                                            transportState = ETransportState.eBuffering;
                                        }
                                        else
                                        {
                                            transportState = ETransportState.eUnknown;
                                        }

                                        Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: transportState=" + transportState);

                                        if (transportState != iTransportState)
                                        {
                                            iTransportState = transportState;

                                            if (iTransportState == ETransportState.eStopped)
                                            {
                                                if (iMaster)
                                                {
                                                    if (!iExpectEventStop)
                                                    {
                                                        Next();
                                                    }
                                                }
                                                iExpectEventStop = false;
                                            }

                                            if (EventTransportStateChanged != null)
                                            {
                                                EventTransportStateChanged(this, EventArgs.Empty);
                                            }
                                        }
                                    }
                                }

                                /*state = n.SelectSingleNode("ns:CurrentTrack", xmlNsMan);
                                if (state != null)
                                {
                                    XmlNode val = state.SelectSingleNode("@val", xmlNsMan);
                                    if (val != null)
                                    {
                                        int trackIndex = int.Parse(val.Value, System.Globalization.CultureInfo.InvariantCulture) - 1;

                                        Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: trackIndex=" + trackIndex);

                                        if (trackIndex != iTrackIndex)
                                        {
                                            iTrackIndex = trackIndex;

                                            if (EventTrackChanged != null)
                                            {
                                                EventTrackChanged(this, EventArgs.Empty);
                                            }
                                        }
                                    }
                                }*/

                                state = n.SelectSingleNode("ns:CurrentTrackDuration", xmlNsMan);
                                if (state != null)
                                {
                                    XmlNode val = state.SelectSingleNode("@val", xmlNsMan);
                                    if (val != null)
                                    {
                                        try
                                        {
                                            Time duration = new Time(val.Value);
                                            Assert.CheckDebug(duration.SecondsTotal >= 0);
                                            uint trackDuration = (duration.SecondsTotal >= 0) ? (uint)duration.SecondsTotal : 0;

                                            Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: trackDuration=" + duration.ToPrettyString());

                                            iTrackDuration = trackDuration;

                                            if (EventDurationChanged != null)
                                            {
                                                EventDurationChanged(this, EventArgs.Empty);
                                            }
                                        }
                                        catch (Time.TimeInvalid)
                                        {
                                            if (val.Value != kNotImplemented)
                                            {
                                                UserLog.WriteLine("ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: Invalid element CurrentTrackDuration: " + val.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string lastChange = iServiceAVTransport.LastChange != null ? iServiceAVTransport.LastChange : "";
                Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: " + ex.Message + "\nLastChange: " + lastChange);
                UserLog.WriteLine("ModelSourceMediaRendererUpnpAv.EventStateLastChangeResponse: " + ex.Message + "\nLastChange: " + lastChange);
                return;
            }
        }

        private void SetCurrent(MrItem aItem)
        {
            lock (this)
            {
                iTrackPlaylistItem = aItem;

                iTrack = null;
                if (aItem != null)
                {
                    iTrack = new Channel(aItem.Uri, aItem.DidlLite);
                }
            }
        }

        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            OnEventSubscriptionError();
        }

        private readonly MrItem kUnknownPlaylistItem;
        private const string kNotImplemented = "NOT_IMPLEMENTED";

        private Source iSource;
        private Linn.Timer iTimer;

        private ServiceConnectionManager iServiceConnectionManager;
        private ServiceConnectionManager.AsyncActionGetProtocolInfo iActionGetProtocolInfo;

        private ServiceAVTransport iServiceAVTransport;
        private ServiceAVTransport.AsyncActionPlay iActionPlay;
        private ServiceAVTransport.AsyncActionPause iActionPause;
        private ServiceAVTransport.AsyncActionStop iActionStop;
        private ServiceAVTransport.AsyncActionSeek iActionSeek;
        private ServiceAVTransport.AsyncActionNext iActionNext;
        private ServiceAVTransport.AsyncActionPrevious iActionPrevious;
        private ServiceAVTransport.AsyncActionSetAVTransportURI iActionSetAVTransportURI;
        private ServiceAVTransport.AsyncActionGetPositionInfo iActionGetPositionInfo;
        private ServiceAVTransport.AsyncActionGetTransportSettings iActionGetTransportSettings;
        private ServiceAVTransport.AsyncActionSetPlayMode iActionSetPlayMode;

        private bool iExpectEventStop;
        private MrItem iExpectedTrack;

        private uint iInstanceId;
        private string iProtocolInfo;

        private bool iMaster;

        private uint iTrackElapsed;
        private uint iTrackDuration;

        private ETransportState iTransportState;

        private int iTrackIndex;
        private uint iTrackBitrate;
        private bool iTrackLossless;
        private uint iTrackBitDepth;
        private uint iTrackSampleRate;
        private string iTrackCodecName;

        private bool iRepeat;
        private bool iShuffle;

        private Mutex iMutex;
        private uint iTrackId;
        private List<MrItem> iPlaylist;

        private MrItem iTrackPlaylistItem;
        private Channel iTrack;
        private bool iOpen;
        private bool iInitialised;
    }
} // Linn.Topology
