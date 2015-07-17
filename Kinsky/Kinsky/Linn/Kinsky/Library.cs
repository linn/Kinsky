using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Upnp;

using Linn;
using Linn.Topology;
using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Kinsky
{
    public class MediaProviderLibrary : IContainer
    {
        public MediaProviderLibrary(HelperKinsky aHelper)
        {
            iEventProvider = aHelper.EventServer;

            iLock = new object();
            iStarted = false;

            iMetadata = new container();
            iMetadata.Id = kLibraryId;
            iMetadata.Title = "Library";
            iMetadata.WriteStatus = "PROTECTED";
            iMetadata.Restricted = true;
            iMetadata.Searchable = true;

            iOptionCloudServers = aHelper.CloudServers;
            iOptionCloudServers.EventValueChanged += CloudServersChanged;

            iMediaServers = new SortedList<MediaServer, ModelMediaServer>(new MediaServerComparer());
            iMediaServerRootContainers = new Dictionary<string, ContainerMediaServer>();
            iLibrary = new Library(aHelper.SsdpNotifyProvider);

            CloudServersChanged(this, EventArgs.Empty);
        }

        public void Start(IPAddress aIpAddress)
        {
            Trace.WriteLine(Trace.kMediaServer, "MediaProviderLibrary.Start(): Using interface " + aIpAddress);

            Assert.Check(!iStarted);

            iLibrary.EventMediaServerAdded += MediaServerAdded;
            iLibrary.EventMediaServerRemoved += MediaServerRemoved;
            iLibrary.Start(aIpAddress);

            iStarted = true;

            Trace.WriteLine(Trace.kMediaServer, "MediaProviderLibrary.Start() successful");
        }

        public void Stop()
        {
            if (iStarted)
            {
                // stop the library - this call will block until all threads associated with
                // iLibrary have stopped and, therefore, no more calls to the event delegates
                // will occur
                iLibrary.Stop();
                iLibrary.EventMediaServerAdded -= MediaServerAdded;
                iLibrary.EventMediaServerRemoved -= MediaServerRemoved;

                List<ModelMediaServer> servers;
                lock (iLock)
                {
                    servers = new List<ModelMediaServer>(iMediaServers.Values);
                    iMediaServers.Clear();
                    iMediaServerRootContainers.Clear();
                    iMetadata.ChildCount = 0;
                }

                foreach (ModelMediaServer m in servers)
                {
                    m.Close();
                    if (EventContentRemoved != null)
                    {
                        EventContentRemoved(this, new EventArgsContentRemoved(m.Metadata.Id));
                    }
                }

                iStarted = false;

                Trace.WriteLine(Trace.kMediaServer, "MediaProviderLibrary.Stop() successful");
            }
            else
            {
                Trace.WriteLine(Trace.kMediaServer, "MediaProviderLibrary.Stop() already stopped - silently do nothing");
            }
        }

        public uint Open()
        {
            lock (iLock)
            {
                return (uint)iMediaServers.Count;
            }
        }

        public void Close()
        {
        }

        public void Refresh()
        {
            iLibrary.Rescan();
        }

        public IContainer ChildContainer(container aContainer)
        {
            lock (iLock)
            {
                if (iMediaServerRootContainers.ContainsKey(aContainer.Id))
                {
                    return iMediaServerRootContainers[aContainer.Id];
                }
                else
                {
                    return null;
                }
            }
        }

        public container Metadata
        {
            get
            {
                return iMetadata;
            }
        }
		
		public bool HandleMove(DidlLite aDidlLite)
		{
			return false;
		}

        public bool HandleInsert(DidlLite aDidlLite)
        {
            return false;
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }
		
		public bool HandleDelete(DidlLite aDidlLite)
		{
			return false;
		}

        public void Delete(string aId)
        {
            throw new NotSupportedException();
        }

        public bool HandleRename(upnpObject aObject)
        {
            return false;
        }

        public void Rename(string aId, string aTitle)
        {
            throw new NotSupportedException();
        }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            lock (iLock)
            {
                for (int i = (int)aStartIndex;
                    i < aStartIndex + aCount && i < iMediaServers.Count;
                    i++)
                {
                    didl.Add(iMediaServers.Values[i].Metadata);
                }
            }

            return didl;
        }

        public DidlLite Search(string aSearchCriterea, uint aStartIndex, uint aCount)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> EventContentUpdated;
        public event EventHandler<EventArgs> EventContentAdded;
        public event EventHandler<EventArgsContentRemoved> EventContentRemoved;
        public event EventHandler<EventArgs> EventTreeChanged;
        
        string IContainer.Id
        {
            get { return iMetadata.Id; }
        }

        public bool HasTreeChangeAffectedLeaf
        {
            get
            {
                return false;
            }
        }


        private void CloudServersChanged(object sender, EventArgs e)
        {
            iLibrary.SetCloudServers(iOptionCloudServers.Native);
        }

        private void MediaServerAdded(object sender, Library.EventArgsMediaServer e)
        {
            ModelMediaServer mediaServer = null;
            ContainerMediaServer rootContainer = null;
            try
            {
                mediaServer = new ModelMediaServer(e.MediaServer, iEventProvider);
                mediaServer.EventContainerUpdated += EventContainerUpdatedHandler;
                mediaServer.EventSystemUpdateIDStateChanged += EventContainerUpdatedHandler;
                mediaServer.EventContentDirectoryInitialised += EventModelMediaServerInitial;
                mediaServer.Open();

                rootContainer = new ContainerMediaServer(mediaServer, mediaServer.Metadata);
            }
            catch (Exception)
            {
                // failed to create the media server - a dodgy media server shouldn't crash the application
                UserLog.WriteLine(string.Format("{0}: Failed to create media server ({1}, {2})", DateTime.Now, e.MediaServer.Name, e.MediaServer.Device.Model));
            }

            if (mediaServer != null)
            {
                lock (iLock)
                {
                    iMediaServers.Remove(e.MediaServer);
                    iMediaServers.Add(e.MediaServer, mediaServer);
                    iMetadata.ChildCount = iMediaServers.Count;

                    iMediaServerRootContainers[mediaServer.Metadata.Id] = rootContainer;
                }

                if (EventContentAdded != null)
                {
                    EventContentAdded(this, EventArgs.Empty);
                }
            }
        }

        private void MediaServerRemoved(object sender, Library.EventArgsMediaServer e)
        {
            bool removed = false;
            ModelMediaServer ms;

            lock (iLock)
            {
                if (iMediaServers.TryGetValue(e.MediaServer, out ms))
                {
                    ms.EventContainerUpdated -= EventContainerUpdatedHandler;
                    ms.EventSystemUpdateIDStateChanged -= EventContainerUpdatedHandler;
                    ms.EventContentDirectoryInitialised -= EventModelMediaServerInitial;
                    ms.Close();

                    iMediaServers.Remove(e.MediaServer);
                    iMetadata.ChildCount = iMediaServers.Count;

                    iMediaServerRootContainers.Remove(ms.Metadata.Id);
                
                    removed = true;
                }
            }

            if (removed && EventContentRemoved != null)
            {
                EventContentRemoved(this, new EventArgsContentRemoved(ms.Metadata.Id));
            }
        }

        private void EventModelMediaServerInitial(object sender, EventArgs args)
        {
            ModelMediaServer ms = sender as ModelMediaServer;
            ContainerMediaServer rootContainer = null;
            
            lock (iLock)
            {
                iMediaServerRootContainers.TryGetValue(ms.Metadata.Id, out rootContainer);
            }
            
            if (rootContainer != null)
            {
                rootContainer.OnEventInitial();
            }
        }

        private void EventContainerUpdatedHandler(object sender, EventArgs args)
        {
            ModelMediaServer ms = sender as ModelMediaServer;
            ContainerMediaServer rootContainer = null;

            lock (iLock)
            {
                iMediaServerRootContainers.TryGetValue(ms.Metadata.Id, out rootContainer);
            }

            if (rootContainer != null)
            {
                ThreadPool.QueueUserWorkItem(delegate {
                    rootContainer.OnEventTreeChanged();
                });
            }
        }

        private class MediaServerComparer : IComparer<MediaServer>
        {
            public int Compare(MediaServer aMediaServer1, MediaServer aMediaServer2)
            {
                try
                {
                    // make sure servers with the same UDN are taken as identical
                    if (aMediaServer1.Device.Udn == aMediaServer2.Device.Udn)
                    {
                        return 0;
                    }

                    // use the name as primary comparer for sorting - fallback to UDN
                    // if 2 separate servers have the same name
                    int nameCmp = string.Compare(aMediaServer1.Name, aMediaServer2.Name);
                    if (nameCmp != 0)
                    {
                        return nameCmp;
                    }
                    else
                    {
                        return aMediaServer1.Device.Udn.CompareTo(aMediaServer2.Device.Udn);
                    }
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("Logging for #892: Exception caught in MediaServerComparer.Compare: " + e);
                    throw e;
                }
            }
        }

        public const string kLibraryId = "Library";

        private const uint kCountPerCall = 100;

        private object iLock;
        private bool iStarted;
        private IEventUpnpProvider iEventProvider;

        private container iMetadata;

        private Library iLibrary;
        private SortedList<MediaServer, ModelMediaServer> iMediaServers;
        private Dictionary<string, ContainerMediaServer> iMediaServerRootContainers;

        private OptionListUri iOptionCloudServers;
    }

    internal class ContainerMediaServer : IContainer
    {
        public ContainerMediaServer(ModelMediaServer aMediaServer, container aContainer)
        {
            iUpdateId = 0;
            iMetadata = aContainer;
            iMediaServer = aMediaServer;
            iLock = new object();
            iReceivedInitial = false;
        }

        public uint Open()
        {
            try
            {
                DidlLite didl;
                uint count;
                uint total;
                lock (iLock)
                {
                    iMediaServer.Browse(Id, 0, 1, out didl, out count, out total, out iUpdateId);
                }
                return total;
                
            }
            catch (ServiceException e)
            {
                Trace.WriteLine(Trace.kKinsky, "ContainerMediaServer.Open: " + e.Message);

                if (e.Code == 801) // Access denied
                {
                    return 1;
                }
                else
                {
                    throw e;
                }
            }
        }

        public void Close()
        {
        }

        public void Refresh() { }

        public IContainer ChildContainer(container aContainer)
        {
            return new ContainerMediaServer(iMediaServer, aContainer);
        }

        public container Metadata
        {
            get
            {
                return iMetadata;
            }
        }
		
		public bool HandleMove(DidlLite aDidlLite)
		{
			return false;
		}
		
		public bool HandleInsert(DidlLite aDidlLite)
        {
            return false;
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }
        
		public bool HandleDelete(DidlLite aDidlLite)
		{
			return false;
		}
		
		public void Delete(string aId)
        {
            throw new NotSupportedException();
        }

        public bool HandleRename(upnpObject aObject)
        {
            return false;
        }

        public void Rename(string aId, string aTitle)
        {
            throw new NotSupportedException();
        }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            try
            {
                DidlLite didl;
                bool changed = false;

                uint count;
                uint total;
                uint updateId;
                uint newUpdateId;
                string id;
                lock (iLock)
                {
                    id = Id;
                    updateId = iUpdateId;
                }
                iMediaServer.Browse(id, aStartIndex, aCount, out didl, out count, out total, out newUpdateId);

                if (updateId != newUpdateId)
                {
                    lock (iLock)
                    {
                        iUpdateId = newUpdateId;
                    }
                    changed = true;
                }
                

                if (changed && EventContentUpdated != null)
                {
                    EventContentUpdated(this, EventArgs.Empty);
                }

                return didl;
            }
            catch (ServiceException e)
            {
                Trace.WriteLine(Trace.kKinsky, "ContainerMediaServer.Items: " + e.Message);

                if (e.Code == 801) // Access denied
                {
                    DidlLite result = new DidlLite();
                    item item = new item();
                    item.Title = "Access denied";
                    result.Add(item);

                    return result;
                }
                else
                {
                    throw e;
                }
            }
        }

        public DidlLite Search(string aSearchCriterea, uint aStartIndex, uint aCount)
        {
            try
            {
                DidlLite didl;
                bool changed = false;

                uint count;
                uint total;
                uint updateId;
                uint newUpdateId;
                string id;
                lock (iLock)
                {
                    id = Id;
                    updateId = iUpdateId;
                }

                Trace.WriteLine(Trace.kKinsky, aSearchCriterea + ", Id=" + id + ", aStartIndex=" + aStartIndex + ", aCount=" + aCount);
                iMediaServer.Search(aSearchCriterea, id, aStartIndex, aCount, out didl, out count, out total, out newUpdateId);
                Trace.WriteLine(Trace.kKinsky, "count=" + count + ", total=" + total + ", updateId=" + iUpdateId);

                if (updateId != iUpdateId)
                {
                    lock (iLock)
                    {
                        iUpdateId = newUpdateId;
                    }
                    changed = true;
                }

                if (changed && EventContentUpdated != null)
                {
                    EventContentUpdated(this, EventArgs.Empty);
                }

                return didl;
            }
            catch (ServiceException e)
            {
                Trace.WriteLine(Trace.kKinsky, "ContainerMediaServer.Search: " + e.Message);
                throw e;
            }
        }

        public event EventHandler<EventArgs> EventContentUpdated;
        public event EventHandler<EventArgs> EventContentAdded { add { } remove { } }
        public event EventHandler<EventArgsContentRemoved> EventContentRemoved { add { } remove { } }
        public event EventHandler<EventArgs> EventTreeChanged;

        public void OnEventInitial()
        {
            iReceivedInitial = true;
        }

        public void OnEventTreeChanged()
        {
            EventHandler<EventArgs> del = EventTreeChanged;
            if (iReceivedInitial && del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private string Id
        {
            get
            {
                string id = kRootId;
                if (iMetadata.Id != iMediaServer.Udn)
                {
                    id = iMetadata.Id;
                }
                return id;
            }
        }

        string IContainer.Id
        {
            get { return iMetadata.Id; }
        }

        public bool HasTreeChangeAffectedLeaf
        {
            get
            {
                try
                {
                    bool changed = false;
                    DidlLite didl;
                    uint count;
                    uint total;
                    uint updateId;
                    uint newUpdateId;
                    string id;

                    lock (iLock) 
                    { 
                        id = Id; 
                        updateId = iUpdateId;
                    } 

                    iMediaServer.Browse(id, 0, 1, out didl, out count, out total, out newUpdateId); 
                    if (updateId != newUpdateId) 
                    { 
                        lock (iLock) 
                        { 
                            iUpdateId = newUpdateId; 
                        } 
                        changed = true; 
                    } 

                    return changed;
                } 
                catch 
                { 
                    return true; 
                }
            }
        }

        private const int kErrorAccessDenied = 801;
        private const string kRootId = "0";

        private uint iUpdateId;
        private container iMetadata;
        private ModelMediaServer iMediaServer;
        private object iLock;
        private bool iReceivedInitial;
        
    }
} 
