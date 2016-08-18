using System;
using System.Collections.Generic;
using Upnp;
using Linn.ControlPoint.Upnp;
using System.Threading;
using Linn.ControlPoint;
using System.Xml;

namespace Linn.Topology
{
    public class ModelSourceMediaRendererDs : ModelSourceMediaRenderer
    {
        
        public ModelSourceMediaRendererDs(Source aSource)
            : base(aSource)
        {
            
            iSource = aSource;
            iInserting = false;

            try
            {
                iServicePlaylist = new ServicePlaylist(iSource.Device, iSource.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionPlay = iServicePlaylist.CreateAsyncActionPlay();
            iActionPause = iServicePlaylist.CreateAsyncActionPause();
            iActionStop = iServicePlaylist.CreateAsyncActionStop();
            iActionNext = iServicePlaylist.CreateAsyncActionNext();
            iActionPrevious = iServicePlaylist.CreateAsyncActionPrevious();
            iActionSeekSecondAbsolute = iServicePlaylist.CreateAsyncActionSeekSecondAbsolute();
            iActionSeekIndex = iServicePlaylist.CreateAsyncActionSeekIndex();
            iActionDelete = iServicePlaylist.CreateAsyncActionDeleteId();
            iActionDeleteAll = iServicePlaylist.CreateAsyncActionDeleteAll();
            iActionSetRepeat = iServicePlaylist.CreateAsyncActionSetRepeat();
            iActionSetShuffle = iServicePlaylist.CreateAsyncActionSetShuffle();

            iIdArray = new ModelIdArray(new PlaylistIdArray(iServicePlaylist));
        }

        public override resource BestSupportedResource(upnpObject aObject)
        {
            return BestSupportedResource(iServicePlaylist.ProtocolInfo, aObject);
        }

        public override void Open()
        {
            iIdArray.EventIdArrayChanged += EventIdArrayChanged;
            iIdArray.Open();
            iTrackIndex = -1;
            iTrackPlaylistItem = null;
            iInserting = false;

            iServicePlaylist.EventStateTransportState += EventStateTransportStateResponse;
            iServicePlaylist.EventStateId += EventStateTrackIdResponse;
            iServicePlaylist.EventStateRepeat += EventStateRepeatResponse;
            iServicePlaylist.EventStateShuffle += EventStateShuffleResponse;
            iServicePlaylist.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServicePlaylist.EventStateIdArray += EventStateIdArrayResponse;
            iServicePlaylist.EventInitial += EventInitialResponsePlaylist;
        }

        public override void Close()
        {
            iIdArray.EventIdArrayChanged -= EventIdArrayChanged;
            iIdArray.Close();
            iServicePlaylist.EventStateTransportState -= EventStateTransportStateResponse;
            iServicePlaylist.EventStateId -= EventStateTrackIdResponse;
            iServicePlaylist.EventStateRepeat -= EventStateRepeatResponse;
            iServicePlaylist.EventStateShuffle -= EventStateShuffleResponse;
            iServicePlaylist.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServicePlaylist.EventStateIdArray -= EventStateIdArrayResponse;
            iServicePlaylist.EventInitial -= EventInitialResponsePlaylist;
        }

        public override Source Source
        {
            get
            {
                return iSource;
            }
        }

        public override string Name
        {
            get
            {
                return (iSource.FullName);
            }
        }

        private void EventIdArrayChanged(object sender, EventArgs e)
        {
            Trace.WriteLine(Trace.kTopology, "ModelSourceMediaRendererDs.EventIdArrayChanged");

            UpdateTrack();

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }

            if (EventTrackChanged != null)
            {
                EventTrackChanged(this, EventArgs.Empty);
            }
        }

        public void EventInitialResponsePlaylist(object sender, EventArgs e)
        {
            if (EventPlaylistInitialised != null)
            {
                EventPlaylistInitialised(this, EventArgs.Empty);
            }

            if (EventControlInitialised != null)
            {
                EventControlInitialised(this, EventArgs.Empty);
            }
        }

        public override event EventHandler<EventArgs> EventControlInitialised;
        public override event EventHandler<EventArgs> EventPlaylistInitialised;

        public override event EventHandler<EventArgs> EventTransportStateChanged;
        public override event EventHandler<EventArgs> EventTrackChanged;
        public override event EventHandler<EventArgs> EventRepeatChanged;
        public override event EventHandler<EventArgs> EventShuffleChanged;

        public override event EventHandler<EventArgs> EventPlaylistChanged;

        public override void Play()
        {
            iActionPlay.PlayBegin();
        }

        public override void Pause()
        {
            iActionPause.PauseBegin();
        }

        public override void Stop()
        {
            iActionStop.StopBegin();
        }

        public override void Previous()
        {
            iActionPrevious.PreviousBegin();
        }

        public override void Next()
        {
            iActionNext.NextBegin();
        }

        public override void SeekSeconds(uint aSeconds)
        {
            iActionSeekSecondAbsolute.SeekSecondAbsoluteBegin(aSeconds);
        }

        public override void SeekTrack(uint aTrack)
        {
            iActionSeekIndex.SeekIndexBegin(aTrack);
        }

        public override void ToggleRepeat()
        {
            iActionSetRepeat.SetRepeatBegin(!Repeat);
        }

        public override void ToggleShuffle()
        {
            iActionSetShuffle.SetShuffleBegin(!Shuffle);
        }

        public override ETransportState TransportState
        {
            get
            {
                return iTransportState;
            }
        }

        public override int TrackIndex
        {
            get
            {
                return iTrackIndex;
            }
        }

        public override MrItem TrackPlaylistItem
        {
            get
            {
                return iTrackPlaylistItem;
            }
        }

        public override bool Repeat
        {
            get
            {
                return iServicePlaylist.Repeat;
            }
        }

        public override bool Shuffle
        {
            get
            {
                return iServicePlaylist.Shuffle;
            }
        }

        public override string ProtocolInfo
        {
            get
            {
                return iServicePlaylist.ProtocolInfo;
            }
        }

        public override void Lock()
        {
            iIdArray.Lock();
        }

        public override void Unlock()
        {
            iIdArray.Unlock();
        }

        public override uint PlayNow(DidlLite aDidlLite)
        {
            uint id = 0;
            try
            {
                Lock();

                if (iIdArray.IdArray.Count > 0)
                {
                    id = iIdArray.IdArray[iIdArray.IdArray.Count - 1];
                }
            }
            finally
            {
                Unlock();
            }

            return PlaylistInsert(id, aDidlLite, true);
        }

        public override uint PlayNext(DidlLite aDidlLite)
        {
            uint id = 0;
            try
            {
                Lock();
                if (iTrackIndex != -1)
                {
                    id = iIdArray.IdArray[iTrackIndex];
                }
            }
            finally
            {
                Unlock();
            }

            return PlaylistInsert(id, aDidlLite);
        }

        public override uint PlayLater(DidlLite aDidlLite)
        {
            uint id = 0;
            try
            {
                Lock();
                if (iIdArray.IdArray.Count > 0)
                {
                    id = iIdArray.IdArray[iIdArray.IdArray.Count - 1];
                }
            }
            finally
            {
                Unlock();
            }

            return PlaylistInsert(id, aDidlLite);
        }

        public override MrItem PlaylistItem(uint aIndex)
        {
            MrItem item;
            try
            {
                Lock();

                item = iIdArray.AtIndex(aIndex);
            }
            finally
            {
                Unlock();
            }

            return item;
        }

        public override void PlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems)
        {
            Assert.Check(aPlaylistItems.Count > 0);

            try
            {
                uint afterId = aInsertAfterId;
                if (afterId == aPlaylistItems[0].Id)
                {
                    Lock();

                    int index = iIdArray.IdArray.IndexOf(afterId);

                    if (index > 0)
                    {
                        afterId = iIdArray.IdArray[index - 1];
                    }
                    else
                    {
                        afterId = 0;
                    }

                    Unlock();
                }
                foreach (MrItem i in aPlaylistItems)
                {
                    try
                    {
                        afterId = iServicePlaylist.InsertSync(afterId, i.Uri, i.DidlLite.Xml);
                        iServicePlaylist.DeleteIdSync(i.Id);
                    }
                    catch (ServiceException e)
                    {
                        if (e.Code == 801)   // playlist full
                        {
                            iServicePlaylist.DeleteIdSync(i.Id);
                            afterId = iServicePlaylist.InsertSync(afterId, i.Uri, i.DidlLite.Xml);
                        }
                        else
                        {
                            throw e;
                        }
                    }
                }
            }
            catch (ServiceException)
            {
                // Handle service exceptions here - these are silently ignored. Basically,
                // if one of these is fired, the likely cause is that the playlist on the DS
                // is out of sync with that displayed in the UI. This can be because another
                // control point is manipulating the list and also because there is some problem
                // with the control point keeping in sync (not receiving events from the DS, for example).
                // There is no real best course of action here - eventually, the eventing should make
                // sure everything is back in sync but, in short of making all playlist manipulations
                // atomic, there is nothing that can be done and, given that these should be exceptional
                // circumstances, doing nothing is ok
            }
        }

        public override uint PlaylistInsert(uint aInsertAfterId, DidlLite aDidlLite)
        {
            return PlaylistInsert(aInsertAfterId, aDidlLite, false);
        }

        private uint PlaylistInsert(uint aInsertAfterId, DidlLite aDidlLite, bool aStartPlaying)
        {
            uint count = 0;
            bool locked = false;
            try
            {
                Lock();
                locked = true;
                if (!iInserting)
                {
                    iInserting = true;

                    Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererDs.PlaylistInsert: aInsertAfterId=" + aInsertAfterId + ", iPlaylistIds.Count=" + iIdArray.IdArray.Count);

                    uint id = aInsertAfterId;
                    uint index = 0;
                    if (aInsertAfterId > 0)
                    {
                        index = (uint)(iIdArray.IdArray.IndexOf(aInsertAfterId) + 1);
                    }

                    Unlock();
                    locked = false;

                    bool error = false;
                    for (int i = 0; i < aDidlLite.Count; ++i)
                    {
                        try
                        {
                            upnpObject item = aDidlLite[i];
                            resource resource = BestSupportedResource(item);
                            if (resource != null)
                            {
                                string uri = resource.Uri;
                                DidlLite didl = new DidlLite();
                                didl.Add(item);

                                uint newId = iServicePlaylist.InsertSync(id, uri, didl.Xml);
                                ++count;

                                if (i == 0)
                                {
                                    if (aStartPlaying)
                                    {
                                        Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererDs.PlaylistInsert: SeekTrack " + index);
                                        SeekTrack(index);
                                    }
                                }

                                // cache the item to save re-downloading it
                                iIdArray.AddToCache(newId, new MrItem(newId, uri, didl));

                                id = newId;
                                ++index;
                                error = false;
                            }
                        }
                        catch (System.IO.IOException e)
                        {
                            if (error)
                            {
                                UserLog.WriteLine("Insert failed (" + e.Message + ")");
                                break;
                            }
                            error = true;
                        }
                        catch (ServiceException e)
                        {
                            UserLog.WriteLine("Insert failed (" + e.Message + ")");
                            break;
                        }
                    }

                    Lock();
                    locked = true;
                    iInserting = false;
                    Unlock();
                    locked = false;

                    Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererDs.PlaylistInsert: insert finished");
                }
                else
                {
                    Unlock();
                    locked = false;
                }
            }
            finally
            {
                if (locked)
                {
                    Unlock();
                }
                iInserting = false;
            }

            return count;
        }

        public override void PlaylistDelete(IList<MrItem> aPlaylistItems)
        {
            foreach (MrItem i in aPlaylistItems)
            {
                if (i != PlaylistIdArray.kEmptyItem)
                {
                    iActionDelete.DeleteIdBegin(i.Id);
                }
            }
        }

        public override void PlaylistDeleteAll()
        {
            iActionDeleteAll.DeleteAllBegin();
        }

        public override bool IsInserting()
        {
            bool result;
            try
            {
                Lock();
                result = iInserting;
            }
            finally
            {
                Unlock();
            }
            return result;
        }

        public override uint PlaylistTrackCount
        {
            get
            {
                return (uint)iIdArray.IdArray.Count;
            }
        }

        public override uint PlaylistTracksMax
        {
            get
            {
                return iServicePlaylist.TracksMax;
            }
        }

        private void EventStateTransportStateResponse(object sender, EventArgs e)
        {
            string transportState = iServicePlaylist.TransportState;
            if (transportState == "Playing")
            {
                iTransportState = ETransportState.ePlaying;
            }
            else if (transportState == "Paused")
            {
                iTransportState = ETransportState.ePaused;
            }
            else if (transportState == "Stopped")
            {
                iTransportState = ETransportState.eStopped;
            }
            else if (transportState == "Buffering")
            {
                iTransportState = ETransportState.eBuffering;
            }
            else
            {
                Assert.CheckDebug(false);
                iTransportState = ETransportState.eUnknown;
            }

            if (EventTransportStateChanged != null)
            {
                EventTransportStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateTrackIdResponse(object sender, EventArgs e)
        {
            UpdateTrack();
        }

        private void EventStateIdArrayResponse(object sender, EventArgs e)
        {
            iIdArray.SetIdArray(ByteArray.Unpack(iServicePlaylist.IdArray));
        }

        private void UpdateTrack()
        {
            bool locked = false;
            try
            {
                Lock();
                locked = true;

                uint trackId = iServicePlaylist.Id;
                iTrackIndex = iIdArray.Index(trackId);

                MrItem item = iIdArray.AtIndex((uint)iTrackIndex);

                if (iTrackPlaylistItem == null || item != iTrackPlaylistItem)
                {
                    Trace.WriteLine(Trace.kMediaRenderer, "ModelSourceMediaRendererDs.UpdateTrack: iTrackIndex=" + iTrackIndex + ", iPlaylistIds.Count=" + iIdArray.IdArray.Count + ", trackId=" + trackId);
                    iTrackPlaylistItem = item;

                    Unlock();
                    locked = false;

                    if (EventTrackChanged != null)
                    {
                        EventTrackChanged(this, EventArgs.Empty);
                    }
                }
                else
                {
                    Unlock();
                    locked = false;
                }
            }
            finally
            {
                if (locked)
                {
                    Unlock();
                }
            }
        }

        private void EventStateRepeatResponse(object sender, EventArgs e)
        {
            if (EventRepeatChanged != null)
            {
                EventRepeatChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateShuffleResponse(object sender, EventArgs e)
        {
            if (EventShuffleChanged != null)
            {
                EventShuffleChanged(this, EventArgs.Empty);
            }
        }

        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            OnEventSubscriptionError();
        }

        private const uint kMaxCacheSize = 1000;

        private Source iSource;

        private ETransportState iTransportState;
        private MrItem iTrackPlaylistItem;
        private int iTrackIndex;

        private ServicePlaylist iServicePlaylist;
        private ServicePlaylist.AsyncActionDeleteId iActionDelete;
        private ServicePlaylist.AsyncActionDeleteAll iActionDeleteAll;
        private ServicePlaylist.AsyncActionSetRepeat iActionSetRepeat;
        private ServicePlaylist.AsyncActionSetShuffle iActionSetShuffle;
        private ServicePlaylist.AsyncActionPlay iActionPlay;
        private ServicePlaylist.AsyncActionPause iActionPause;
        private ServicePlaylist.AsyncActionStop iActionStop;
        private ServicePlaylist.AsyncActionNext iActionNext;
        private ServicePlaylist.AsyncActionPrevious iActionPrevious;
        private ServicePlaylist.AsyncActionSeekSecondAbsolute iActionSeekSecondAbsolute;
        private ServicePlaylist.AsyncActionSeekIndex iActionSeekIndex;

        private bool iInserting;
        private ModelIdArray iIdArray;
    }

    public class PlaylistIdArray : IIdArray
    {
        public PlaylistIdArray(ServicePlaylist aServicePlaylist)
        {
            iServicePlaylist = aServicePlaylist;
            iServicePlaylist.EventStateIdArray += EventStateIdArrayResponse;
        }

        public string Read(uint aId)
        {
            return ReadList(aId.ToString());
        }

        public string ReadList(string aIdList)
        {
            return iServicePlaylist.ReadListSync(aIdList);
        }

        public MrItem Default
        {
            get
            {
                return kEmptyItem;
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

                    foreach (XmlNode n in document.SelectNodes("/TrackList/Entry"))
                    {
                        uint id = uint.Parse(n["Id"].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        string uri = n["Uri"].InnerText;
                        string metadata = n["Metadata"].InnerText;
                        DidlLite didl = null;
                        try
                        {
                            didl = new DidlLite(metadata);
                            if (didl.Count == 0)
                            {
                                UserLog.WriteLine(string.Format("Empty DidlLite created from metadata '{0}'", metadata));
                                item item = new item();
                                item.Title = uri;

                                didl.Add(item);
                            }
                        }
                        catch (XmlException)
                        {
                            didl = new DidlLite();

                            item item = new item();
                            item.Title = uri;

                            didl.Add(item);
                        }
                        list.Add(new MrItem(id, uri, didl));
                    }
                }
                catch (XmlException e)
                {
                    Trace.WriteLine(Trace.kTopology, "IdArrayMetadataCollector.ParseMetadataXml: " + e.Message);
                }
                catch (FormatException) { }
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

        public static readonly MrItem kEmptyItem = new MrItem(0, null, new DidlLite("<DidlLite xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\"><item><dc:title>Empty</dc:title><upnp:class>object.item.audioItem</upnp:class></item></DidlLite>"));

        private ServicePlaylist iServicePlaylist;
    }

} // Linn.Topology
