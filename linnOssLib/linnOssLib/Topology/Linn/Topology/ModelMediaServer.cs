using System;
using System.Xml;
using System.Threading;
using System.Collections.Generic;

using Upnp;

using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class ModelMediaServer
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public class EventArgsContainerUpdate : EventArgs
        {
            public EventArgsContainerUpdate(string aContainerId, string aUpdateId)
            {
                ContainerId = aContainerId;
                UpdateId = aUpdateId;
            }

            public string ContainerId;
            public string UpdateId;
        }

        public ModelMediaServer(MediaServer aMediaServer, IEventUpnpProvider aEventServer)
        {
            iMediaServer = aMediaServer;

            iServiceContentDirectory = new ServiceContentDirectory(iMediaServer.Device, aEventServer);

            iMetadata = new container();
            iMetadata.ParentId = "Library";
            string title = iMediaServer.Name;
            if (title != null)
            {
                iMetadata.Title = title;
            }
            iMetadata.Id = Udn;
            iMetadata.Restricted = true;

            if (title != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(aMediaServer.Device.DeviceXml);

                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("ns", "urn:schemas-upnp-org:device-1-0");

                XmlNodeList icons = doc.SelectNodes("/ns:root/ns:device/ns:iconList/ns:icon", nsm);

                string url = string.Empty;
                int width = 0;
                int height = 0;
                int depth = 0;
                foreach (XmlNode i in icons)
                {
                    // some servers do not implement this part of the XML correctly i.e. some of these
                    // elements are missing - given that this code is just to identify an icon to show
                    // for the server, it is a bit harsh to enforce the standard and forego the use of the
                    // server, so this incorrect XML is just accounted for here
                    XmlNode widthNode = i.SelectSingleNode("ns:width", nsm);
                    XmlNode heightNode = i.SelectSingleNode("ns:height", nsm);
                    XmlNode depthNode = i.SelectSingleNode("ns:depth", nsm);
                    XmlNode urlNode = i.SelectSingleNode("ns:url", nsm);

                    try
                    {
                        int w = (widthNode != null) ? int.Parse(widthNode.InnerText, System.Globalization.CultureInfo.InvariantCulture) : 0;
                        int h = (heightNode != null) ? int.Parse(heightNode.InnerText, System.Globalization.CultureInfo.InvariantCulture) : 0;
                        int d = (depthNode != null) ? int.Parse(depthNode.InnerText, System.Globalization.CultureInfo.InvariantCulture) : 0;

                        if (urlNode != null && (w > width || h > height || d > depth))
                        {
                            width = w;
                            height = h;
                            depth = d;
                            url = urlNode.InnerText;
                            if (url.StartsWith("/"))
                            {
                                url = url.Substring(1);
                            }
                        }
                    }
                    catch (FormatException) { }
                }

                if (url != string.Empty)
                {
                    Uri uri = new Uri(aMediaServer.Device.Location);
                    iMetadata.AlbumArtUri.Add(string.Format("http://{0}:{1}/{2}", aMediaServer.Device.IpAddress, uri.Port, url));
                }
            }
        }

        public void Open()
        {
            iServiceContentDirectory.EventStateContainerUpdateIDs += EventStateContainerUpdateIDsResponse;
            iServiceContentDirectory.EventStateSystemUpdateID += EventStateSystemUpdateID;
            iServiceContentDirectory.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceContentDirectory.EventInitial += EventInitialResponse;
        }

        public void Close()
        {
            iServiceContentDirectory.EventStateContainerUpdateIDs -= EventStateContainerUpdateIDsResponse;
            iServiceContentDirectory.EventStateSystemUpdateID -= EventStateSystemUpdateID;
            iServiceContentDirectory.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceContentDirectory.EventInitial -= EventInitialResponse;
        }

        public string Udn
        {
            get
            {
                return iMediaServer.Device.Udn;
            }
        }

        public string Name
        {
            get
            {
                string title = iMediaServer.Name;
                iMetadata.Title = title;
                return title;
            }
        }

        private void EventInitialResponse(object sender, EventArgs e)
        {
            InitialiseHomeContainer();

            if (EventContentDirectoryInitialised != null)
            {
                EventContentDirectoryInitialised(this, EventArgs.Empty);
            }
        }

        private void InitialiseHomeContainer()
        {
            /*try
            {
                string result;
                uint numberReturned, totalMatches, updateId;

                iServiceContentDirectory.BrowseSync("0", ServiceContentDirectory.kBrowseFlagBrowseDirectChildren, "*", 0, 1, "",
                    out result, out numberReturned, out totalMatches, out updateId);

                iHomeContainer.ChildCount = (int)totalMatches;
            }
            catch (ServiceException)
            {
                iHomeContainer.ChildCount = 0;
            }*/
        }

        public EventHandler<EventArgs> EventContentDirectoryInitialised;
        public EventHandler<EventArgsContainerUpdate> EventContainerUpdated;
        public EventHandler<EventArgs> EventSystemUpdateIDStateChanged;

        public void Browse(string aId, uint aStartIndex, uint aCount, out DidlLite aResult, out uint aNumberReturned, out uint aTotalMatches, out uint aUpdateId)
        {
            string result;
            iServiceContentDirectory.BrowseSync(aId, ServiceContentDirectory.kBrowseFlagBrowseDirectChildren, "*", aStartIndex, aCount, "",
                out result, out aNumberReturned, out aTotalMatches, out aUpdateId);

            try
            {
                aResult = new DidlLite(result);
            }
            catch (XmlException ex)
            {
                UserLog.WriteLine("Xml error loading range of items: " + ex);
                UserLog.WriteLine(result);
                aResult = new DidlLite();
                for (int i = 0; i < aNumberReturned; i++)
                {
                    item errorItem = new item();
                    errorItem.Title = "Invalid data returned by server - check user log for details.";
                    aResult.Add(errorItem);
                }
            }
            
            //Assert.CheckDebug(aResult.Count == aNumberReturned);
        }

        public void Search(string aSearchCriteria, string aId, uint aStartIndex, uint aCount, out DidlLite aResult, out uint aNumberReturned, out uint aTotalMatches, out uint aUpdateId)
        {
            string result;
            iServiceContentDirectory.SearchSync(aId, aSearchCriteria, "*", aStartIndex, aCount, "",
                out result, out aNumberReturned, out aTotalMatches, out aUpdateId);
            try
            {
                aResult = new DidlLite(result);
            }
            catch (XmlException ex)
            {
                UserLog.WriteLine("Xml error loading range of items: " + ex);
                UserLog.WriteLine(result);
                aResult = new DidlLite();
                for (int i = 0; i < aNumberReturned; i++)
                {
                    item errorItem = new item();
                    errorItem.Title = "Invalid data returned by server - check user log for details.";
                    aResult.Add(errorItem);
                }
            }
            //Assert.CheckDebug(aResult.Count == aNumberReturned);
        }

        public container Metadata
        {
            get
            {
                return iMetadata;
            }
        }

        public uint SystemUpdateID
        {
            get
            {
                return iServiceContentDirectory.SystemUpdateID;
            }
        }

        public override string ToString()
        {
            return (String.Format("MediaServer({0})", Name));
        }

        private void EventStateContainerUpdateIDsResponse(object sender, EventArgs e)
        {
            string updateIDs = iServiceContentDirectory.ContainerUpdateIDs;
            if (updateIDs.Length > 0)
            {
                string[] split = updateIDs.Split(',');
                if (split.Length < 2)
                {
                    return;
                }
                //Assert.Check(split.Length > 1);
                for (int i = 0; i < split.Length; i += 2)
                {
                    if (i + 1 < split.Length)
                    {
                        if (EventContainerUpdated != null)
                        {
                            EventContainerUpdated(this, new EventArgsContainerUpdate(split[i], split[i + 1]));
                        }
                    }
                }
            }
        }

        private void EventStateSystemUpdateID(object sender, EventArgs e)
        {
            if (EventSystemUpdateIDStateChanged != null)
            {
                EventSystemUpdateIDStateChanged(this, EventArgs.Empty);
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

        private container iMetadata;
        private MediaServer iMediaServer;

        private ServiceContentDirectory iServiceContentDirectory;
    }
} // Linn.Topology
