using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;
using Linn.Topology;

namespace Linn.Kinsky
{
    public class HelperKinsky : Helper, IStack
    {
        public HelperKinsky(string[] aArgs, IInvoker aInvoker)
            : base(aArgs)
        {
            iInvoker = aInvoker;
            iEventServer = new EventServerUpnp();
            iListenerNotify = new SsdpListenerMulticast();

            IModelFactory factory = new ModelFactory();
            iTopologyHouse = new Linn.Topology.House(iListenerNotify, iEventServer, factory);
            iSenders = new ModelSenders(iListenerNotify, iEventServer);
            iHouse = new House(iTopologyHouse, iInvoker, iSenders);

            OptionPage optionPage = new OptionPage("Startup Room");
            iOptionStartupRoom = new OptionStartupRoom(iHouse);
            optionPage.Add(iOptionStartupRoom);
            AddOptionPage(optionPage);

            //optionPage = new OptionPage("Cloud Servers");
            iOptionCloudServers = new OptionListUri("cloudservers", "Server locations", "List of locations for cloud media servers", new List<Uri>());
            //optionPage.Add(iOptionCloudServers);
            //AddOptionPage(optionPage);

            iOptionLastSelectedRoom = new OptionString("lastroom", "Last Selected Room", "The last room selected", string.Empty);
            AddOption(iOptionLastSelectedRoom);

            iOptionLastLocation = new OptionBreadcrumbTrail("lastlocation", "Last Location", "The last location visited by the browser", BreadcrumbTrail.Default);
            AddOption(iOptionLastLocation);

            iBookmarkManager = new BookmarkManager(Path.Combine(DataPath.FullName, "Bookmarks.xml"));

            Stack.SetStack(this);
        }

        public BookmarkManager BookmarkManager
        {
            get
            {
                return iBookmarkManager;
            }
        }

        public ISsdpNotifyProvider SsdpNotifyProvider
        {
            get
            {
                return iListenerNotify;
            }
        }

        public IEventUpnpProvider EventServer
        {
            get
            {
                return iEventServer;
            }
        }

        internal IInvoker Invoker
        {
            get
            {
                return iInvoker;
            }
        }

        public House House
        {
            get
            {
                return iHouse;
            }
        }

        internal Topology.House TopologyHouse
        {
            get
            {
                return iTopologyHouse;
            }
        }

        public ModelSenders Senders
        {
            get
            {
                return iSenders;
            }
        }

        public OptionStartupRoom StartupRoom
        {
            get
            {
                return iOptionStartupRoom;
            }
        }

        public OptionBreadcrumbTrail LastLocation
        {
            get
            {
                return iOptionLastLocation;
            }
        }

        public OptionString LastSelectedRoom
        {
            get
            {
                return iOptionLastSelectedRoom;
            }
        }

        public OptionListUri CloudServers
        {
            get
            {
                return iOptionCloudServers;
            }
        }

        public void SetStackExtender(IStack aStackExtender)
        {
            StackStatus status = Stack.Status;

            Assert.Check(iStackExtender == null);
            Assert.Check(status.State == EStackState.eStopped);
            iStackExtender = aStackExtender;
        }

        public void Rescan()
        {
            lock (this)
            {
                StackStatus status = Stack.Status;
                if (status.State == EStackState.eOk)
                {
                    iTopologyHouse.Rescan();
                    iSenders.Rescan();
                }
            }
        }

        void IStack.Start(IPAddress aIpAddress)
        {
            iEventServer.Start(aIpAddress);
            iListenerNotify.Start(aIpAddress);
            iHouse.Start(aIpAddress);
            iSenders.Start(aIpAddress);

            if (iStackExtender != null)
            {
                iStackExtender.Start(aIpAddress);
            }
        }

        void IStack.Stop()
        {
            if (iStackExtender != null)
            {
                iStackExtender.Stop();
            }

            iSenders.Stop();
            iHouse.Stop();
            iListenerNotify.Stop();
            iEventServer.Stop();
        }

        private EventServerUpnp iEventServer;
        private SsdpListenerMulticast iListenerNotify;

        private Topology.House iTopologyHouse;
        private House iHouse;
        private ModelSenders iSenders;

        private IStack iStackExtender;

        private OptionStartupRoom iOptionStartupRoom;
        private OptionString iOptionLastSelectedRoom;
        private OptionBreadcrumbTrail iOptionLastLocation;
        private OptionListUri iOptionCloudServers;

        private BookmarkManager iBookmarkManager;
        private IInvoker iInvoker;
    }

    
}


