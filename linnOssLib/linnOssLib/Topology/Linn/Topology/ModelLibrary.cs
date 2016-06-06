using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections.Generic;

using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;

using Upnp;

namespace Linn.Topology
{
    public class LibraryNotFoundException : Exception
    {
        public LibraryNotFoundException(string aUdn)
            : base("Media server udn " + aUdn + " not found")
        {
        }
    }

    public class ModelLibrary
    {
        public ModelLibrary(ISsdpNotifyProvider aListenerNotify, IEventUpnpProvider aEventServer)
        {
            iUpdateId = 0;
            iMutex = new Mutex(false);

            iLibrary = new Linn.Topology.Library(aListenerNotify);

            iEventServer = aEventServer;

            iMediaServerByUdn = new Dictionary<string, ModelMediaServer>();
            iMediaServerList = new SortedList<string, MediaServer>();

            iHomeContainer = new container();
            iHomeContainer.Title = "Library";
            iHomeContainer.Id = kLibraryId;
            iHomeContainer.ChildCount = 0;
            iHomeContainer.Restricted = true;
            iHomeContainer.Searchable = true;

            iLibrary.EventMediaServerAdded += MediaServerAdded;
            iLibrary.EventMediaServerRemoved += MediaServerRemoved;
        }

        public void Start(IPAddress aInterface)
        {
            iMutex.WaitOne();

            iLibrary.Start(aInterface);
            
            iMutex.ReleaseMutex();
        }

        public void Stop()
        {
            iMutex.WaitOne();

            iLibrary.Stop();

            foreach (ModelMediaServer m in iMediaServerByUdn.Values)
            {
                m.EventContentDirectoryInitialised -= EventContentDirectoryInitialisedResponse;
                m.EventContainerUpdated -= EventContainerUpdatedResponse;
                m.Close();
            }

            iMediaServerByUdn.Clear();
            iMediaServerList.Clear();

            iHomeContainer.ChildCount = 0;

            iMutex.ReleaseMutex();
        }

        public void Rescan()
        {
            iLibrary.Rescan();
        }

        public container HomeContainer
        {
            get
            {
                return iHomeContainer;
            }
        }

        public event EventHandler<ModelMediaServer.EventArgsContainerUpdate> EventContainerUpdated;

        public uint Count(string aMediaServerId, string aId, out uint aUpdateId)
        {
            // we are in the library container
            if (aMediaServerId == string.Empty)
            {
                iMutex.WaitOne();
                aUpdateId = iUpdateId;
                uint count = (uint)iHomeContainer.ChildCount;
                iMutex.ReleaseMutex();

                return count;
            }
            else
            {
                ModelMediaServer server = ModelMediaServer(aMediaServerId);
                if (server != null)
                {
                    DidlLite result;
                    uint numberReturned, totalMatches;
                    server.Browse(aId, 0, 1, out result, out numberReturned, out totalMatches, out aUpdateId);

                    return totalMatches;
                }
            }

            // we get here if the media server we are querying is not in our media server list
            throw new LibraryNotFoundException(aMediaServerId);
        }

        public DidlLite Items(string aMediaServerId, string aId, uint aStartIndex, uint aCount, out uint aUpdateId)
        {
            if (aMediaServerId == string.Empty)
            {
                DidlLite result = new DidlLite();

                iMutex.WaitOne();

                aUpdateId = iUpdateId;
                uint endIndex = aStartIndex + aCount;
                for (uint i = 0; i < endIndex && i < iMediaServerList.Values.Count; ++i)
                {
                    MediaServer server = iMediaServerList.Values[(int)i];
                    ModelMediaServer s;
                    if (iMediaServerByUdn.TryGetValue(server.Device.Udn, out s))
                    {
                        result.Add(s.Metadata);
                    }
                }

                iMutex.ReleaseMutex();

                return result;
            }
            else
            {
                ModelMediaServer server = ModelMediaServer(aMediaServerId);
                if (server != null)
                {
                    DidlLite result;
                    uint numberReturned, totalMatches;
                    server.Browse(aId, aStartIndex, aCount, out result, out numberReturned, out totalMatches, out aUpdateId);
                    return result;
                }
            }

            // we get here if the media server we are querying is not in our media server list
            throw new LibraryNotFoundException(aMediaServerId);
        }

        public DidlLite Search(string aSearchCriteria, string aMediaServerId, string aId, uint aStartIndex, uint aCount, out uint aUpdateId)
        {
            if (aMediaServerId == string.Empty)
            {
                DidlLite result = new DidlLite();

                iMutex.WaitOne();
                // we need to search across all discovered media servers
                aUpdateId = iUpdateId;
                iMutex.ReleaseMutex();

                return result;
            }
            else
            {
                ModelMediaServer server = ModelMediaServer(aMediaServerId);
                if (server != null)
                {
                    DidlLite result;
                    uint numberReturned, totalMatches;
                    server.Search(aSearchCriteria, aId, aStartIndex, aCount, out result, out numberReturned, out totalMatches, out aUpdateId);
                    return result;
                }
            }
            // we get here if the media server we are querying is not in our media server list
            throw new LibraryNotFoundException(aMediaServerId);
        }

        private ModelMediaServer ModelMediaServer(string aUdn)
        {
            iMutex.WaitOne();

            ModelMediaServer server;
            if (iMediaServerByUdn.TryGetValue(aUdn, out server))
            {
                iMutex.ReleaseMutex();

                return server;
            }
            else
            {
                iMutex.ReleaseMutex();
            }

            return null;
        }

        private void MediaServerAdded(object sender, Library.EventArgsMediaServer e)
        {
            ModelMediaServer server = null;

            Trace.WriteLine(Trace.kMediaServer, ">ModelLibrary.MediaServerAdded: " + e.MediaServer);
            try
            {
                server = new ModelMediaServer(e.MediaServer, iEventServer);
            }
            catch (Linn.ControlPoint.ServiceException ex)
            {
                // Need to show media server icon with error symbol
                UserLog.WriteLine(e.MediaServer + ": (" + ex.Message + ")");

                return;
            }

            try
            {
                iMutex.WaitOne();

                UserLog.WriteLine(DateTime.Now + ": Library+                " + server);

                iMediaServerByUdn.Add(server.Udn, server);
                iMediaServerList.Add(server.Name + server.Udn, e.MediaServer);

                iHomeContainer.ChildCount = iMediaServerList.Count;

                iUpdateId++;

                iMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(Trace.kMediaServer, "ModelLibrary.MediaServerAdded: " + ex.Message);

                iMutex.ReleaseMutex();

                throw ex;
            }

            server.EventContentDirectoryInitialised += EventContentDirectoryInitialisedResponse;
            server.EventContainerUpdated += EventContainerUpdatedResponse;

            server.Open();

            iMutex.WaitOne();
            uint updateId = iUpdateId;
            iMutex.ReleaseMutex();

            EventContainerUpdatedResponse(null, new ModelMediaServer.EventArgsContainerUpdate(iHomeContainer.Id, updateId.ToString()));

            Trace.WriteLine(Trace.kMediaServer, "<ModelLibrary.MediaServerAdded");
        }

        private void MediaServerRemoved(object sender, Library.EventArgsMediaServer e)
        {
            Trace.WriteLine(Trace.kMediaServer, ">ModelLibrary.MediaServerRemoved: " + e.MediaServer.Name);

            string udn = e.MediaServer.Device.Udn;
            ModelMediaServer server;

            iMutex.WaitOne();

            if (iMediaServerByUdn.TryGetValue(udn, out server))
            {
                try
                {
                    iMediaServerByUdn.Remove(server.Udn);
                    iMediaServerList.Remove(server.Name + server.Udn);

                    iHomeContainer.ChildCount = iMediaServerList.Count;

                    iUpdateId++;

                    iMutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(Trace.kMediaServer, "ModelLibrary.MediaServerRemoved: " + ex.Message);

                    iMutex.ReleaseMutex();

                    throw ex;
                }

                server.Close();

                server.EventContentDirectoryInitialised -= EventContentDirectoryInitialisedResponse;
                server.EventContainerUpdated -= EventContainerUpdatedResponse;

                UserLog.WriteLine(DateTime.Now + ": Library-                " + server);

                iMutex.WaitOne();
                uint updateId = iUpdateId;
                iMutex.ReleaseMutex();

                EventContainerUpdatedResponse(null, new ModelMediaServer.EventArgsContainerUpdate(iHomeContainer.Id, updateId.ToString()));
            }
            else
            {
                iMutex.ReleaseMutex();
            }

            Trace.WriteLine(Trace.kMediaServer, "<ModelLibrary.MediaServerRemoved");
        }

        private void EventContentDirectoryInitialisedResponse(object sender, EventArgs e)
        {
        }

        private void EventContainerUpdatedResponse(object sender, ModelMediaServer.EventArgsContainerUpdate e)
        {
            if (EventContainerUpdated != null)
            {
                EventContainerUpdated(this, e);
            }
        }

        public const string kLibraryId = "Library";

        private uint iUpdateId;
        private Mutex iMutex;

        private Library iLibrary;
        private IEventUpnpProvider iEventServer;
        private Dictionary<string, ModelMediaServer> iMediaServerByUdn;
        private SortedList<string, MediaServer> iMediaServerList;
        private container iHomeContainer;
    }
} // Linn.Topology
