using System;
using System.Xml;
using System.Collections.Generic;

using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

using Upnp;

namespace Linn.Topology
{
    public class ModelPlaylist
    {
        public class PlaylistIdArray : IIdArray
        {
            public PlaylistIdArray(uint aPlaylistId, ServicePlaylistManager aServicePlaylistManager)
            {
                iPlaylistId = aPlaylistId;
                iServicePlaylistManager = aServicePlaylistManager;
            }

            public string Read(uint aId)
            {
                return ReadList(aId.ToString());
            }

            public string ReadList(string aIdList)
            {
                return iServicePlaylistManager.ReadListSync(iPlaylistId, aIdList);
            }

            public MrItem Default
            {
                get
                {
                    return kDefault;
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
                            //string udn = n["Udn"].InnerText;
                            string metadata = n["Metadata"].InnerXml;
                            DidlLite didl = null;
                            if (id > 0)
                            {
                                try
                                {
                                    didl = new DidlLite(metadata);
                                    list.Add(new MrItem(id, null, didl));
                                }
                                catch(Exception e)
                                {
                                    UserLog.WriteLine("PlaylistIdArray.ParseMetadataXml: " + e.Message + " whilst parsing " + metadata);
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
                    catch (XmlException e) { }
                    catch (FormatException e) { }
                }
    
                return list;
            }

            public static readonly MrItem kDefault = new MrItem(0, null, new DidlLite("<DidlLite xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\"><item><dc:title>Unknown</dc:title><upnp:class>object.item</upnp:class></item></DidlLite>"));

            private uint iPlaylistId;

            private ServicePlaylistManager iServicePlaylistManager;
        }

        public ModelPlaylist(uint aId, ServicePlaylistManager aServicePlaylistManager)
        {
            iId = aId;
            iServicePlaylistManager = aServicePlaylistManager;

            iActionDeleteId = iServicePlaylistManager.CreateAsyncActionDeleteId();

            iIdArray = new ModelIdArray(new PlaylistIdArray(aId, aServicePlaylistManager));
        }

        internal void IncRef()
        {
            ++iRefCount;
            Trace.WriteLine(Trace.kTopology, "ModelPlaylist.IncRef: " + iRefCount);
        }

        internal void DecRef()
        {
            --iRefCount;
            Trace.WriteLine(Trace.kTopology, "ModelPlaylist.DecRef: " + iRefCount);
        }

        internal uint RefCount
        {
            get
            {
                Trace.WriteLine(Trace.kTopology, "ModelPlaylist.RefCount: " + iRefCount);
                return iRefCount;
            }
        }

        internal uint Token
        {
            set
            {
                if(iToken != value)
                {
                    iToken = value;
                    SetIdArray();
                }
            }
        }

        public uint Id
        {
            get
            {
                return iId;
            }
        }

        internal void Open(uint aToken)
        {
            iIdArray.EventIdArrayChanged += EventIdArrayChanged;
            iIdArray.Open();

            iToken = aToken;
            SetIdArray();
        }

        internal void Close()
        {
            iIdArray.Close();
            iIdArray.EventIdArrayChanged -= EventIdArrayChanged;
        }

        public void Lock()
        {
            iIdArray.Lock();
        }

        public void Unlock()
        {
            iIdArray.Unlock();
        }

        public uint MaxCount
        {
            get
            {
                return iServicePlaylistManager.TracksMax;
            }
        }

        public uint Count
        {
            get
            {
                iIdArray.Lock();
                uint count = (uint)iIdArray.IdArray.Count;
                iIdArray.Unlock();

                return count;
            }
        }

        public MrItem Track(uint aIndex)
        {
            Lock();
            MrItem item = iIdArray.AtIndex(aIndex);
            Unlock();

            return item;
        }

        public void Delete(string aTrackMetadataId)
        {
            Lock();

            uint id = 0;
            uint count = Count;
            for (uint i = 0; i < count; ++i)
            {
                MrItem item = Track(i);
                if (item.DidlLite[0].Id == aTrackMetadataId)
                {
                    id = iIdArray.IdArray[(int)i];
                    break;
                }
            }

            Unlock();

            Delete(id);
        }

        public void Delete(uint aTrackId)
        {
            iActionDeleteId.DeleteIdBegin(iId, aTrackId);
        }

        public void Insert(string aAfterTrackMetadataId, DidlLite aDidlLite)
        {
            Lock();

            uint id = 0;
            uint count = Count;
            for (uint i = 0; i < count; ++i)
            {
                MrItem item = Track(i);
                if (item.DidlLite[0].Id == aAfterTrackMetadataId)
                {
                    id = iIdArray.IdArray[(int)i];
                    break;
                }
            }

            Unlock();

            Insert(id, aDidlLite);
        }

        public void Insert(uint aAfterId, DidlLite aDidlLite)
        {
            uint id = aAfterId;
            foreach (upnpObject o in aDidlLite)
            {
                DidlLite didl = new DidlLite();
                didl.Add(o);
                uint newId = iServicePlaylistManager.InsertSync(iId, id, didl.Xml);
                id = newId;
            }
        }

        public EventHandler<EventArgs> EventPlaylistChanged;

        private void SetIdArray()
        {
            if(iToken > 0)
            {
                byte[] bytes = iServicePlaylistManager.PlaylistReadArraySync(iId);
                IList<uint> array = ByteArray.Unpack(bytes);
                iIdArray.SetIdArray(array);
            }
            else
            {
                iIdArray.SetIdArray(new List<uint>());
            }
        }

        private void EventIdArrayChanged(object sender, EventArgs e)
        {
            if(EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }
        }

        private uint iId;
        private uint iToken;

        private uint iRefCount;

        private ModelIdArray iIdArray;

        private ServicePlaylistManager iServicePlaylistManager;
        private ServicePlaylistManager.AsyncActionDeleteId iActionDeleteId;
    }

    public class ModelPlaylistManager
    {
        public class PlaylistsIdArray : IIdArray
        {
            public PlaylistsIdArray(ServicePlaylistManager aServicePlaylistManager, ModelPlaylistManager aModelPlaylistManger)
            {
                iServicePlaylistManager = aServicePlaylistManager;
                iModelPlaylistManager = aModelPlaylistManger;
            }
    
            public string Read(uint aId)
            {
                return ReadList(aId.ToString());
            }
    
            public string ReadList(string aIdList)
            {
                return iServicePlaylistManager.PlaylistReadListSync(aIdList);
            }

            public MrItem Default
            {
                get
                {
                    return kDefault;
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
    
                        foreach (XmlNode n in document.SelectNodes("/PlaylistList/Entry"))
                        {
                            uint id = uint.Parse(n["Id"].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            string name = n["Name"].InnerText;
                            string description = n["Description"].InnerText;
                            uint imageId = uint.Parse(n["ImageId"].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            DidlLite didl = null;
                            if (id > 0)
                            {
                                didl = new DidlLite();

                                playlistContainer p = new playlistContainer();

                                p.Id = id.ToString();
                                p.Title = name;
                                p.Description = description;

                                if(imageId != 0)
                                {
                                    Uri uri;
                                    if(iModelPlaylistManager.ImageList.TryGetValue(imageId, out uri))
                                    {
                                        p.AlbumArtUri.Add(uri.AbsoluteUri);
                                    }
                                }

                                didl.Add(p);

                                list.Add(new MrItem(id, null, didl));
                            }
                        }
                    }
                    catch (XmlException e) { }
                    catch (FormatException e) { }
                }
    
                return list;
            }

            public static readonly MrItem kDefault = new MrItem(0, null, new DidlLite("<DidlLite xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\"><container><dc:title>Unknown</dc:title><upnp:class>object.container.playlistContainer</upnp:class></container></DidlLite>"));
    
            private ServicePlaylistManager iServicePlaylistManager;
            private ModelPlaylistManager iModelPlaylistManager;
        }

        public event EventHandler<EventArgs> EventSubscriptionError;

        public ModelPlaylistManager(PlaylistManager aPlaylistManager, IEventUpnpProvider aEventServer)
        {
            iPlaylistManager = aPlaylistManager;

            try
            {
                iServicePlaylistManager = new ServicePlaylistManager(iPlaylistManager.Device, aEventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionPlaylistDeleteId = iServicePlaylistManager.CreateAsyncActionPlaylistDeleteId();

            iLock = new object();

            iModelPlaylistList = new Dictionary<uint, ModelPlaylist>();

            iIdArray = new ModelIdArray(new PlaylistsIdArray(iServicePlaylistManager, this));
            iIdArray.EventIdArrayChanged += EventIdArrayChanged;
        }

        public void Open()
        {
            iIdArray.Open();

            iServicePlaylistManager.EventStateImagesXml += EventImagesXmlResponse;
            iServicePlaylistManager.EventStateMetadata += EventMetadataResponse;
            iServicePlaylistManager.EventStateIdArray += EventIdArrayResponse;
            iServicePlaylistManager.EventStateTokenArray += EventTokenArrayResponse;
            iServicePlaylistManager.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServicePlaylistManager.EventInitial += EventInitialResponse;
        }

        public void Close()
        {
            iIdArray.Close();

            iServicePlaylistManager.EventStateImagesXml -= EventImagesXmlResponse;
            iServicePlaylistManager.EventStateMetadata -= EventMetadataResponse;
            iServicePlaylistManager.EventStateIdArray -= EventIdArrayResponse;
            iServicePlaylistManager.EventStateTokenArray -= EventTokenArrayResponse;
            iServicePlaylistManager.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServicePlaylistManager.EventInitial -= EventInitialResponse;
        }

        public string Udn
        {
            get
            {
                return iPlaylistManager.Device.Udn;
            }
        }

        public string Name
        {
            get
            {
                return iPlaylistManager.Name;
            }
        }

        public PlaylistManager PlaylistManager
        {
            get
            {
                return iPlaylistManager;
            }
        }

        public void Lock()
        {
            iIdArray.Lock();
        }

        public void Unlock()
        {
            iIdArray.Unlock();
        }

        private void EventInitialResponse(object sender, EventArgs e)
        {
            if (EventPlaylistManagerInitialised != null)
            {
                EventPlaylistManagerInitialised(this, EventArgs.Empty);
            }
        }

        public IDictionary<uint, Uri> ImageList
        {
            get
            {
                lock(iLock)
                {
                    return iImageList;
                }
            }
        }

        public DidlLite Metadata
        {
            get
            {
                lock(iLock)
                {
                    return iMetadata;
                }
            }
        }

        public uint MaxCount
        {
            get
            {
                return iServicePlaylistManager.PlaylistsMax;
            }
        }

        public uint Count
        {
            get
            {
                Lock();
                uint count = (uint)iIdArray.IdArray.Count;
                Unlock();

                return count;
            }
        }

        public MrItem Playlist(uint aIndex)
        {
            Lock();
            MrItem item = iIdArray.AtIndex(aIndex);
            Unlock();

            return item;
        }

        public void Delete(uint aId)
        {
            iActionPlaylistDeleteId.PlaylistDeleteIdBegin(aId);
        }

        public ModelPlaylist Insert(uint aAfterId, string aName, string aDescription, uint aImageId)
        {
            uint newId = iServicePlaylistManager.PlaylistInsertSync(aAfterId, aName, aDescription, aImageId);
            ModelPlaylist p = new ModelPlaylist(newId, iServicePlaylistManager);
            p.Open(0);
            return p;
        }

        public void Move(uint aId, uint aAfterId)
        {
            iServicePlaylistManager.PlaylistMoveSync(aId, aAfterId);
        }

        public void SetName(uint aId, string aName)
        {
            iServicePlaylistManager.PlaylistSetNameSync(aId, aName);
        }

        public void SetDescription(uint aId, string aDescription)
        {
            iServicePlaylistManager.PlaylistSetDescriptionSync(aId, aDescription);
        }

        public void SetImageId(uint aId, uint aImageId)
        {
            iServicePlaylistManager.PlaylistSetImageIdSync(aId, aImageId);
        }

        public ModelPlaylist CreatePlaylist(uint aId)
        {
            lock(iLock)
            {
                ModelPlaylist playlist;
                if(iModelPlaylistList.TryGetValue(aId, out playlist))
                {
                    playlist.IncRef();
                    RemoveStalePlaylists();
                    return playlist;
                }
    
                playlist = new ModelPlaylist(aId, iServicePlaylistManager);
                playlist.IncRef();
    
                iIdArray.Lock();
    
                int index = iIdArray.Index(aId);
                Assert.Check(index != -1);
                uint token = iTokenArray[index];
                playlist.Open(token);
    
                iIdArray.Unlock();

                iModelPlaylistList.Add(playlist.Id, playlist);
    
                RemoveStalePlaylists();
    
                return playlist;
            }
        }

        public void DestroyPlaylist(ModelPlaylist aModelPlaylist)
        {
            aModelPlaylist.DecRef();
        }

        public EventHandler<EventArgs> EventPlaylistManagerInitialised;

        public EventHandler<EventArgs> EventMetadataChanged;
        public EventHandler<EventArgs> EventPlaylistsChanged;
        public EventHandler<EventArgs> EventImagesListChanged;

        private void RemoveStalePlaylists()
        {
            IList<ModelPlaylist> playlists = new List<ModelPlaylist>(iModelPlaylistList.Values);
            foreach(ModelPlaylist p in playlists)
            {
                if(p.RefCount == 0)
                {
                    Trace.WriteLine(Trace.kTopology, "Removed playlist " + p.Id);
                    p.Close();
                    iModelPlaylistList.Remove(p.Id);
                }
            }
        }

        private void EventImagesXmlResponse(object sender, EventArgs e)
        {
            lock (iLock)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(iServicePlaylistManager.ImagesXml);

                    iImageList = new Dictionary<uint, Uri>();
                    foreach (XmlNode n in doc.SelectNodes("/ImageList/Entry"))
                    {
                        uint id = uint.Parse(n["Id"].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        string uri = n["Uri"].InnerText;
                        iImageList.Add(id, new Uri(uri));
                    }
                }
                catch (Exception)
                {
                    iImageList = new Dictionary<uint, Uri>();
                }
            }

            if(EventImagesListChanged != null)
            {
                EventImagesListChanged(this, EventArgs.Empty);
            }
        }

        private void EventMetadataResponse(object sender, EventArgs e)
        {
            lock (iLock)
            {
                try
                {
                    iMetadata = new DidlLite(iServicePlaylistManager.Metadata);
                }
                catch (Exception)
                {
                    iMetadata = null;
                }

            }

            if (EventMetadataChanged != null)
            {
                EventMetadataChanged(this, EventArgs.Empty);
            }
        }

        private void EventIdArrayResponse(object sender, EventArgs e)
        {
            // handled in token array response
        }

        private void EventTokenArrayResponse(object sender, EventArgs e)
        {
            iIdArray.Lock();

            iIdArray.ClearCache();
            iIdArray.SetIdArray(ByteArray.Unpack(iServicePlaylistManager.IdArray));
            iTokenArray = ByteArray.Unpack(iServicePlaylistManager.TokenArray);

            foreach(ModelPlaylist p in iModelPlaylistList.Values)
            {
                int index = iIdArray.Index(p.Id);
                uint token = 0;
                if(index > -1)
                {
                    token = iTokenArray[index];
                }
                p.Token = token;
            }

            iIdArray.Unlock();
        }

        private void EventIdArrayChanged(object sender, EventArgs e)
        {
            Trace.WriteLine(Trace.kTopology, "ModelPlaylistManager.EventIdArrayChanged");

            if(EventPlaylistsChanged != null)
            {
                EventPlaylistsChanged(this, EventArgs.Empty);
            }
        }

        public override string ToString()
        {
            return (String.Format("PlaylistManager({0})", Name));
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
        private object iLock;

        private Dictionary<uint, Uri> iImageList;
        private DidlLite iMetadata;
        private PlaylistManager iPlaylistManager;

        private ModelIdArray iIdArray;
        private IList<uint> iTokenArray;
        private Dictionary<uint, ModelPlaylist> iModelPlaylistList;

        private ServicePlaylistManager iServicePlaylistManager;
        private ServicePlaylistManager.AsyncActionPlaylistDeleteId iActionPlaylistDeleteId;
    }
}

