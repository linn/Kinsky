using System;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology.Layer1
{
    public interface IStack
    {
        void Start(IPAddress aInterface);
        void Rescan();
        void Stop();
        event EventHandler<EventArgsGroup> EventGroupAdded;
        event EventHandler<EventArgsGroup> EventGroupRemoved;
        void RemoveDevice(Device aDevice);
    }

    public class EventArgsGroup : EventArgs
    {
        public EventArgsGroup(IGroup aGroup)
        {
            iGroup = aGroup;
        }

        public IGroup Group
        {
            get
            {
                return (iGroup);
            }
        }

        private IGroup iGroup;
    }

    public interface IGroup
    {
        string Room { get; }
        string Name { get; }
        IPreamp Preamp { get; }
        uint SourceCount { get; }
        uint CurrentSource { get; }
        ISource Source(uint aIndex);
        void SetStandby(bool aValue);
        void Select(uint aSourceIndex);
        bool Standby { get; }
        event EventHandler<EventArgs> EventCurrentSourceChanged;
        event EventHandler<EventArgs> EventStandbyChanged;
        event EventHandler<EventArgsSource> EventSourceChanged;
        bool HasInfo { get; }
        bool HasTime { get; }
    }

    public class EventArgsSource : EventArgs
    {
        public EventArgsSource(uint aIndex)
        {
            iIndex = aIndex;
        }

        public uint Index
        {
            get
            {
                return (iIndex);
            }
        }

        private uint iIndex;
    }

    public interface ISource
    {
        string Name { get; }
        string Type { get; }
        bool Visible { get; }
        Device Device { get; }
    }

    public interface IPreamp
    {
        string Type { get; }
        Device Device { get; }
    }

    public class Stack : IStack
    {
        public event EventHandler<EventArgsGroup> EventGroupAdded;
        public event EventHandler<EventArgsGroup> EventGroupRemoved;

        public class Group
        {
            protected Group(string aRoom, string aName, bool aStandby, IPreamp aPreamp, uint aCurrentSource)
            {
                iRoom = aRoom;
                iName = aName;
                iStandby = aStandby;
                iPreamp = aPreamp;
                iCurrentSource = aCurrentSource;

                iMutex = new Mutex();
            }

            public bool Standby
            {
                get
                {
                    bool result;
                    try
                    {
                        Lock();
                        result = iStandby;
                    }
                    finally
                    {
                        Unlock();
                    }
                    return result;
                }
            }

            public string Room
            {
                get
                {
                    return (iRoom);
                }
            }

            public string Name
            {
                get
                {
                    return (iName);
                }
            }

            public IPreamp Preamp
            {
                get
                {
                    return (iPreamp);
                }
            }

            public uint CurrentSource
            {
                get
                {
                    uint result;
                    try
                    {
                        Lock();
                        result = iCurrentSource;
                    }
                    finally
                    {
                        Unlock();
                    }
                    return result;
                }
            }

            protected void Lock()
            {
                iMutex.WaitOne();
            }

            protected void Unlock()
            {
                iMutex.ReleaseMutex();
            }

            public void UpdateCurrentSource(uint aIndex)
            {
                Lock();

                if (iCurrentSource != aIndex)
                {
                    iCurrentSource = aIndex;

                    Unlock();

                    if (EventCurrentSourceChanged != null)
                    {
                        EventCurrentSourceChanged(this, EventArgs.Empty);
                    }
                }
                else
                {
                    Unlock();
                }
            }

            public void UpdateStandby(bool aStandby)
            {
                Lock();

                if (iStandby != aStandby)
                {
                    iStandby = aStandby;

                    Unlock();

                    if (EventStandbyChanged != null)
                    {
                        EventStandbyChanged(this, EventArgs.Empty);
                    }
                }
                else
                {
                    Unlock();
                }
            }

            public void UpdateSource(uint aIndex)
            {
                if (EventSourceChanged != null)
                {
                    EventSourceChanged(this, new EventArgsSource(aIndex));
                }
            }

            private string iRoom;
            private string iName;
            private bool iStandby;

            private uint iCurrentSource;

            private IPreamp iPreamp;

            private Mutex iMutex;

            public event EventHandler<EventArgs> EventCurrentSourceChanged;
            public event EventHandler<EventArgs> EventStandbyChanged;
            public event EventHandler<EventArgsSource> EventSourceChanged;
        }

        public interface IProduct
        {
            void SetStandby(bool aValue);
            void Select(uint aSourceIndex);
            uint SourceCount { get; }
            uint CurrentSource { get; }
            ISource Source(uint aIndex);
            bool HasInfo { get; }
            bool HasTime { get; }
        }

        public class GroupProduct : Group, IGroup
        {
            public GroupProduct(string aRoom, string aName, bool aStandby, IPreamp aPreamp, IProduct aProduct)
                : base(aRoom, aName, aStandby, aPreamp, aProduct.CurrentSource)
            {
                iProduct = aProduct;
            }

            public void SetStandby(bool aValue)
            {
                iProduct.SetStandby(aValue);
            }

            public void Select(uint aIndex)
            {
                iProduct.Select(aIndex);
            }

            public uint SourceCount
            {
                get
                {
                    return (iProduct.SourceCount);
                }
            }

            public bool HasInfo
            {
                get
                {
                    return iProduct.HasInfo;
                }
            }

            public bool HasTime
            {
                get
                {
                    return iProduct.HasTime;
                }
            }

            public ISource Source(uint aIndex)
            {
                return (iProduct.Source(aIndex));
            }

            IProduct iProduct;
        }

        public class GroupMediaRenderer : Group, IGroup
        {
            public GroupMediaRenderer(string aRoom, string aName, ISource aSource, IPreamp aPreamp)
                : base(aRoom, aName, false, aPreamp, 0)
            {
                iSource = aSource;
                iSourceCount = 0;

                if (iSource != null)
                {
                    iSourceCount = 1;
                }
            }

            public void SetStandby(bool aValue)
            {
            }

            public void Select(uint aIndex)
            {
            }

            public uint SourceCount
            {
                get
                {
                    return (iSourceCount);
                }
            }

            public ISource Source(uint aIndex)
            {
                Assert.Check(aIndex == 0);
                Assert.Check(iSource != null);
                return (iSource);
            }

            public bool HasInfo
            {
                get
                {
                    return true;
                }
            }

            public bool HasTime
            {
                get
                {
                    return true;
                }
            }

            ISource iSource;
            uint iSourceCount;
        }

        public class Preamp : IPreamp
        {
            public Preamp(string aType, Device aDevice)
            {
                iType = aType;
                iDevice = aDevice;
            }

            public string Type
            {
                get
                {
                    return (iType);
                }
            }

            public Device Device
            {
                get
                {
                    return (iDevice);
                }
            }

            public override string ToString()
            {
                return (String.Format("Preamp({0}, {1})", iType, iDevice));
            }

            private string iType;
            private Device iDevice;
        }

        public class Source : ISource
        {
            internal Source(string aName, string aType, bool aVisible, Device aDevice)
            {
                iName = aName;
                iType = aType;
                iVisible = aVisible;
                iDevice = aDevice;
            }

            public string Name
            {
                get
                {
                    return (iName);
                }
            }

            public string Type
            {
                get
                {
                    return (iType);
                }
            }

            public bool Visible
            {
                get
                {
                    return (iVisible);
                }
            }

            public Device Device
            {
                get
                {
                    return (iDevice);
                }
            }

            public override string ToString()
            {
                return (String.Format("Source({0}, {1}, {2}, {3})", iName, iType, iVisible, iDevice));
            }

            private string iName;
            private string iType;
            private bool iVisible;
            private Device iDevice;
        }

        internal class Product : IProduct
        {
            private const string kSourceTypeUpnp = "UpnpAv";
            
            public event EventHandler<Layer0.EventArgsDevice> EventSubscriptionError;

            public Product(Stack aStack, Device aDevice)
            {
                iStack = aStack;
                iDevice = aDevice;

                iMutex = new Mutex();

                iServiceProduct = new ServiceProduct(iDevice, iStack.EventServer);
                iServiceProduct.EventSubscriptionError += EventSubscriptionErrorHandler;

                iActionSetStandby = iServiceProduct.CreateAsyncActionSetStandby();
                iActionSetSourceIndex = iServiceProduct.CreateAsyncActionSetSourceIndex();

                iKilled = false;
            }

            void EventSubscriptionErrorHandler(object sender, EventArgs e)
            {
                OnEventSubscriptionError();
            }

            public void Select(uint aIndex)
            {
                iActionSetSourceIndex.SetSourceIndexBegin(aIndex);
            }

            public void SetStandby(bool aValue)
            {
                iActionSetStandby.SetStandbyBegin(aValue);
            }

            public uint SourceCount
            {
                get
                {
                    return ((uint)iSourceList.Count);
                }
            }

            public uint CurrentSource
            {
                get
                {
                    return (iCurrentSource);
                }
            }

            public ISource Source(uint aIndex)
            {
                return (iSourceList[(int)aIndex]);
            }

            public override string ToString()
            {
                return (iDevice.Udn);
            }

            public void Open()
            {
                Trace.WriteLine(Trace.kTopology, "Layer1 Product Open+    " + this);

                Lock();

                iServiceProduct.EventInitial += Initial;

                Unlock();
            }

            private void Initial(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    if (iServiceProduct.SourceXml != null)
                    {
                        iStack.ScheduleJob(new JobProductInitial(this));
                    }
                }

                Unlock();
            }

            public void DoInitial()
            {
                Lock();

                if (!iKilled)
                {
                    iRoom = iServiceProduct.ProductRoom;
                    iName = iServiceProduct.ProductName;
                    iStandby = iServiceProduct.Standby;
                    iCurrentSource = iServiceProduct.SourceIndex;
                    iSourceList = ParseSourceXml(iServiceProduct.SourceXml);

                    if (iSourceList.Count == 0)
                    {
                        iSourceList.Add(CreateSource(iName, "Aux", false));
                        iName = String.Empty;
                    }

                    iServiceProduct.EventStateProductRoom += RoomChanged;
                    iServiceProduct.EventStateProductName += NameChanged;
                    iServiceProduct.EventStateSourceIndex += SourceIndexChanged;
                    iServiceProduct.EventStateSourceXml += SourceXmlChanged;
                    iServiceProduct.EventStateStandby += StandbyChanged;

                    DoAddGroup();
                }

                Unlock();
            }

            protected void OnEventSubscriptionError()
            {
                if (EventSubscriptionError != null)
                {
                    EventSubscriptionError(this, new Layer0.EventArgsDevice(iDevice));
                }
            }

            private List<ISource> ParseSourceXml(string aXml)
            {
                List<ISource> list = new List<ISource>();

                if (aXml != null)
                {
                    try
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(aXml);

                        foreach (XmlNode n in document.SelectNodes("/SourceList/Source"))
                        {
                            string name = n["Name"].InnerText;
                            string type = n["Type"].InnerText;
                            bool visible;
                            try
                            {
                                visible = bool.Parse(n["Visible"].InnerText);
                            }
                            catch (FormatException)
                            {
                                visible = (n["Visible"].InnerText == "1");
                            }
                            list.Add(CreateSource(name, type, visible));
                        }
                    }
                    catch (XmlException ex)
                    {
                        // logging for ticket #1001
                        UserLog.WriteLine("Ticket #1001: XmlException caught in ParseSourceXml()" + ex + ", " + aXml);
                    }
                }

                return (list);
            }

            private void DoAddGroup()
            {
                Preamp preamp = null;

                if (iServiceProduct.Attributes.Contains("Volume"))
                {
                    preamp = new Preamp("Preamp", iDevice);
                }

                iGroup = new GroupProduct(iRoom, iName, iStandby, preamp, this);

                if (iStack.EventGroupAdded != null)
                {
                    iStack.EventGroupAdded(iStack, new EventArgsGroup(iGroup));
                }
            }

            private void DoRemoveGroup()
            {
                if (iGroup != null)
                {
                    if (iStack.EventGroupRemoved != null)
                    {
                        iStack.EventGroupRemoved(iStack, new EventArgsGroup(iGroup));
                    }
                }
            }

            public void Close()
            {
                Trace.WriteLine(Trace.kTopology, "Layer1 Product Close-   " + this);

                Lock();
                iServiceProduct.EventSubscriptionError -= EventSubscriptionErrorHandler;
                iServiceProduct.EventInitial -= Initial;

                Unlock();
            }

            public void Kill()
            {
                Trace.WriteLine(Trace.kTopology, "Layer1 Product Kill-   " + this);

                Lock();

                if (!iKilled)
                {
                    iKilled = true;

                    iStack.ScheduleJob(new JobProductKilled(this));

                    Unlock();
                }
                else
                {
                    Unlock();
                }
            }

            public void OnKilled()
            {
                if (iGroup != null && iStack.EventGroupRemoved != null)
                {
                    iStack.EventGroupRemoved(iStack, new EventArgsGroup(iGroup));
                }

                iServiceProduct.EventSubscriptionError -= EventSubscriptionErrorHandler;
                iServiceProduct.Kill();
            }

            // Lock functions

            public void Lock()
            {
                iMutex.WaitOne();
            }

            public void Unlock()
            {
                iMutex.ReleaseMutex();
            }

            internal ServiceProduct Service
            {
                get
                {
                    return (iServiceProduct);
                }
            }

            private ISource CreateSource(string aName, string aType, bool aVisible)
            {
                ISource source;

                if (aType == kSourceTypeUpnp)
                {
                    if (iLinnMediaRendererDevice == null)
                    {
                        iLinnMediaRendererDevice = DeviceUpnp.CreateLinnMediaRendererDevice(iDevice);
                    }

                    source = new Source(aName, aType, aVisible, iLinnMediaRendererDevice);
                }
                else
                {
                    source = new Source(aName, aType, aVisible, iDevice);
                }

                return (source);
            }

            private void NameChanged(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobProductNameChanged(this));
                }

                Unlock();
            }

            public void DoNameChanged()
            {
                Lock();

                if (!iKilled)
                {
                    if (iName != iServiceProduct.ProductName)
                    {
                        DoRemoveGroup();

                        iName = iServiceProduct.ProductName;

                        DoAddGroup();
                    }
                }

                Unlock();
            }

            private void RoomChanged(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobProductRoomChanged(this));
                }

                Unlock();
            }

            public void DoRoomChanged()
            {
                Lock();

                if (!iKilled)
                {
                    if (iRoom != iServiceProduct.ProductRoom)
                    {
                        DoRemoveGroup();

                        iRoom = iServiceProduct.ProductRoom;

                        DoAddGroup();
                    }
                }

                Unlock();
            }


            private void SourceIndexChanged(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobProductSourceIndexChanged(this));
                }

                Unlock();
            }

            public void DoSourceIndexChanged()
            {
                Lock();

                if (!iKilled)
                {
                    if (iCurrentSource != iServiceProduct.SourceIndex)
                    {
                        iCurrentSource = iServiceProduct.SourceIndex;

                        iGroup.UpdateCurrentSource(iCurrentSource);
                    }
                }

                Unlock();
            }

            private void StandbyChanged(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobProductStandbyChanged(this));
                }

                Unlock();
            }

            private void SourceXmlChanged(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobProductSourceXmlChanged(this));
                }

                Unlock();
            }

            public void DoSourceXmlChanged()
            {
                Lock();

                if (!iKilled)
                {
                    List<ISource> list = ParseSourceXml(iServiceProduct.SourceXml);

                    uint index = 0;

                    // logging for ticket #1001
                    if (list.Count > iSourceList.Count)
                    {
                        UserLog.WriteLine("Logging for ticket #1001." + iServiceProduct.SourceXml);
                        foreach (ISource source in iSourceList)
                        {
                            UserLog.WriteLine("Existing source: " + source);
                        }
                        foreach (ISource source in list)
                        {
                            UserLog.WriteLine("New source: " + source);
                        }
                        Assert.Check(false);
                    }

                    foreach (ISource source in list)
                    {
                        ISource old = iSourceList[(int)index];

                        if (source.Visible != old.Visible || source.Name != old.Name)
                        {
                            iSourceList[(int)index] = source;
                            iGroup.UpdateSource(index);
                        }

                        index++;
                    }
                }

                Unlock();
            }


            public void DoStandbyChanged()
            {
                Lock();

                if (!iKilled)
                {
                    iStandby = iServiceProduct.Standby;
                    iGroup.UpdateStandby(iStandby);
                }

                Unlock();
            }

            public bool HasInfo
            {
                get
                {
                    return iServiceProduct.Attributes.Contains("Info");
                }
            }

            public bool HasTime
            {
                get
                {
                    return iServiceProduct.Attributes.Contains("Time");
                }
            }

            private Stack iStack;
            private Device iDevice;
            private Mutex iMutex;

            private bool iKilled;

            private ServiceProduct iServiceProduct;

            private ServiceProduct.AsyncActionSetStandby iActionSetStandby;
            private ServiceProduct.AsyncActionSetSourceIndex iActionSetSourceIndex;

            private string iRoom;
            private string iName;
            private bool iStandby;

            private uint iCurrentSource;

            List<ISource> iSourceList;

            private Device iLinnMediaRendererDevice;

            private GroupProduct iGroup;
        }

        internal class Upnp
        {
            public event EventHandler<Layer0.EventArgsDevice> EventSubscriptionError;
            public Upnp(Stack aStack, Device aDevice)
            {
                iStack = aStack;
                iDevice = aDevice;

                iMutex = new Mutex();

                iKilled = false;
                iClosed = false;
            }

            public override string ToString()
            {
                return (iDevice.Udn);
            }

            public void Open()
            {
                Trace.WriteLine(Trace.kTopology, "Layer1 Upnp Open+       " + this);

                Lock();
                iClosed = false;
                iDevice.EventOpened += Opened;
                iDevice.EventOpenFailed += OpenFailed;

                Unlock();

                iDevice.Open();
            }

            private void OpenFailed(object obj, EventArgs e)
            {
                OnEventSubscriptionError();
            }

            private void Opened(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled && !iClosed)
                {
                    iStack.ScheduleJob(new JobUpnpOpened(this));
                }

                Unlock();
            }

            public void DoOpened()
            {
                Lock();

                if (iKilled)
                {
                    Unlock();
                    return;
                }

                // Is this a Sonos device

                XmlNode root = Linn.ControlPoint.Upnp.Upnp.DeviceXmlRoot(iDevice.DeviceXml);

                if (root == null)
                {
                    UserLog.WriteLine(DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find root node");
                    Trace.WriteLine(Trace.kTopology, DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find root node");
                    Unlock();
                    return;
                }

                if (Linn.ControlPoint.Upnp.Upnp.HasService(root, ServiceDeviceProperties.ServiceType()) > 0)
                {
                    string udn = Linn.ControlPoint.Upnp.Upnp.Udn(root);

                    if (udn == null)
                    {
                        UserLog.WriteLine(DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find UDN");
                        Trace.WriteLine(Trace.kTopology, DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find UDN");
                        Unlock();
                        return;
                    }

                    Device sonos = iDevice.RelatedDevice(udn);

                    iServiceDeviceProperties = new ServiceDeviceProperties(sonos, iStack.EventServer);
                    iServiceDeviceProperties.EventSubscriptionError += EventSubscriptionErrorHandler;
                    iServiceDeviceProperties.EventInitial += SonosInitial;

                    Unlock();

                    return;
                }


                XmlNode device = Linn.ControlPoint.Upnp.Upnp.DeviceXmlExplicit(iDevice.DeviceXml, iDevice.Udn);

                if (device == null)
                {
                    UserLog.WriteLine(DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find device node");
                    Trace.WriteLine(Trace.kTopology, DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find device node");
                    Unlock();
                    return;
                }

                string friendly = Linn.ControlPoint.Upnp.Upnp.FriendlyName(device);

                if (friendly == null)
                {
                    UserLog.WriteLine(DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find friendlyName node");
                    Trace.WriteLine(Trace.kTopology, DateTime.Now + ": Upnp.DoOpened: " + iDevice.Location + ": failed to find friendlyName node");
                    Unlock();
                    return;
                }

                ParseName(friendly, out iRoom, out iName);

                if (Linn.ControlPoint.Upnp.Upnp.HasService(device, ServiceRenderingControl.ServiceType()) > 0)
                {
                    iPreamp = new Preamp("UpnpAv", iDevice);
                }

                DoAddGroup();

                Unlock();
            }

            private void EventSubscriptionErrorHandler(object sender, EventArgs e)
            {
                OnEventSubscriptionError();
            }

            public void Close()
            {
                Trace.WriteLine(Trace.kTopology, "Layer1 Upnp Close-      " + this);

                Lock();

                iDevice.EventOpened -= Opened;
                iDevice.EventOpenFailed -= OpenFailed;

                if (iServiceDeviceProperties != null)
                {
                    iServiceDeviceProperties.EventSubscriptionError -= EventSubscriptionErrorHandler;
                    iServiceDeviceProperties.EventInitial -= SonosInitial;
                    iServiceDeviceProperties.EventStateZoneName -= SonosRoomChanged;
                }

                iClosed = true;

                Unlock();
            }

            private void SonosInitial(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobUpnpSonosInitial(this));
                }

                Unlock();
            }

            public void OnSonosInitial()
            {
                Lock();

                if (!iKilled)
                {

                    XmlNode device = Linn.ControlPoint.Upnp.Upnp.DeviceXmlExplicit(iDevice.DeviceXml, iDevice.Udn);

                    if (device == null)
                    {
                        Unlock();
                        return;
                    }

                    iRoom = iServiceDeviceProperties.ZoneName;
                    iName = Linn.ControlPoint.Upnp.Upnp.ModelNumber(device);

                    iPreamp = new Preamp("UpnpAv", iDevice);

                    iServiceDeviceProperties.EventStateZoneName += SonosRoomChanged;

                    DoAddGroup();
                }

                Unlock();
            }

            private void SonosRoomChanged(object obj, EventArgs e)
            {
                Lock();

                if (!iKilled)
                {
                    iStack.ScheduleJob(new JobUpnpSonosRoomChanged(this));
                }

                Unlock();
            }

            public void OnSonosRoomChanged()
            {
                Lock();

                if (!iKilled)
                {
                    if (iRoom != iServiceDeviceProperties.ZoneName)
                    {
                        DoRemoveGroup();

                        iRoom = iServiceDeviceProperties.ZoneName;

                        DoAddGroup();
                    }
                }

                Unlock();
            }

            private void DoAddGroup()
            {
                iSource = new Source(iName, "UpnpAv", true, iDevice);

                iGroup = new GroupMediaRenderer(iRoom, iName, iSource, iPreamp);

                if (iStack.EventGroupAdded != null)
                {
                    iStack.EventGroupAdded(iStack, new EventArgsGroup(iGroup));
                }
            }

            private void DoRemoveGroup()
            {
                if (iGroup != null)
                {
                    if (iStack.EventGroupRemoved != null)
                    {
                        iStack.EventGroupRemoved(iStack, new EventArgsGroup(iGroup));
                    }
                }
            }

            public void Kill()
            {
                Trace.WriteLine(Trace.kTopology, "Layer1 Upnp Kill-      " + this);

                Lock();

                if (!iKilled)
                {
                    iKilled = true;

                    iStack.ScheduleJob(new JobUpnpKilled(this));

                    Unlock();
                }
                else
                {
                    Unlock();
                }
            }

            public void OnKilled()
            {
                if (iGroup != null && iStack.EventGroupRemoved != null)
                {
                    iStack.EventGroupRemoved(iStack, new EventArgsGroup(iGroup));
                }

                iDevice.EventOpened -= Opened;

                iDevice.EventOpenFailed -= OpenFailed; 
                if (iServiceDeviceProperties != null)
                {
                    iServiceDeviceProperties.EventSubscriptionError -= EventSubscriptionErrorHandler;
                    iServiceDeviceProperties.EventInitial -= SonosInitial;
                    iServiceDeviceProperties.EventStateZoneName -= SonosRoomChanged;
                    iServiceDeviceProperties.Kill();
                }
            }

            // Lock functions

            public void Lock()
            {
                iMutex.WaitOne();
            }

            public void Unlock()
            {
                iMutex.ReleaseMutex();
            }

            private static bool ParseBrackets(string aFriendlyName, out string aRoom, out string aName, char aOpen, char aClose)
            {
                int open = aFriendlyName.IndexOf(aOpen);

                if (open >= 0)
                {
                    int close = aFriendlyName.IndexOf(aClose);

                    if (close > -0)
                    {
                        int bracketed = close - open - 1;

                        if (bracketed > 1)
                        {
                            aRoom = aFriendlyName.Substring(0, open).Trim();
                            aName = aFriendlyName.Substring(open + 1, bracketed).Trim();
                            return (true);
                        }
                    }
                }

                aRoom = aFriendlyName;
                aName = aFriendlyName;

                return (false);
            }

            private void ParseName(string aFriendlyName, out string aRoom, out string aName)
            {
                if (ParseBrackets(aFriendlyName, out aRoom, out aName, '(', ')'))
                {
                    return;
                }

                if (ParseBrackets(aFriendlyName, out aRoom, out aName, '[', ']'))
                {
                    return;
                }

                if (ParseBrackets(aFriendlyName, out aRoom, out aName, '<', '>'))
                {
                    return;
                }

                int index = aFriendlyName.IndexOf(':');

                if (index < 0)
                {
                    index = aFriendlyName.IndexOf('.');
                }

                if (index < 0)
                {
                    aRoom = aFriendlyName;
                    aName = aFriendlyName;
                    return;
                }

                aRoom = aFriendlyName.Substring(0, index).Trim();
                aName = aFriendlyName.Substring(index + 1).Trim();
            }

            protected void OnEventSubscriptionError()
            {
                if (EventSubscriptionError != null)
                {
                    EventSubscriptionError(this, new Layer0.EventArgsDevice(iDevice));
                }
            }
            
            private Stack iStack;
            private Device iDevice;
            private Mutex iMutex;

            private bool iKilled;

            private string iRoom;
            private string iName;

            private GroupMediaRenderer iGroup;

            private IPreamp iPreamp;
            private ISource iSource;

            private ServiceDeviceProperties iServiceDeviceProperties;
            private bool iClosed;
        }

        internal interface IJob
        {
            void Execute(Stack aStack);
        }

        internal class JobProductInitial : IJob
        {
            public JobProductInitial(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.DoInitial();
            }

            private Product iProduct;
        }

        internal class JobProductKilled : IJob
        {
            public JobProductKilled(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.OnKilled();
            }

            private Product iProduct;
        }

        internal class JobProductRoomChanged : IJob
        {
            public JobProductRoomChanged(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.DoRoomChanged();
            }

            private Product iProduct;
        }

        internal class JobProductNameChanged : IJob
        {
            public JobProductNameChanged(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.DoNameChanged();
            }

            private Product iProduct;
        }

        internal class JobProductSourceIndexChanged : IJob
        {
            public JobProductSourceIndexChanged(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.DoSourceIndexChanged();
            }

            private Product iProduct;
        }

        internal class JobProductSourceXmlChanged : IJob
        {
            public JobProductSourceXmlChanged(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.DoSourceXmlChanged();
            }

            private Product iProduct;
        }

        internal class JobProductStandbyChanged : IJob
        {
            public JobProductStandbyChanged(Product aProduct)
            {
                iProduct = aProduct;
            }

            public void Execute(Stack aStack)
            {
                iProduct.DoStandbyChanged();
            }

            private Product iProduct;
        }

        internal class JobUpnpOpened : IJob
        {
            public JobUpnpOpened(Upnp aUpnp)
            {
                iUpnp = aUpnp;
            }

            public void Execute(Stack aStack)
            {
                iUpnp.DoOpened();
            }

            private Upnp iUpnp;
        }

        internal class JobUpnpKilled : IJob
        {
            public JobUpnpKilled(Upnp aUpnp)
            {
                iUpnp = aUpnp;
            }

            public void Execute(Stack aStack)
            {
                iUpnp.OnKilled();
            }

            private Upnp iUpnp;
        }

        internal class JobAbort : IJob
        {
            public JobAbort()
            {
            }

            public void Execute(Stack aStack)
            {
                Thread.CurrentThread.Abort();
            }
        }

        internal class JobUpnpSonosInitial : IJob
        {
            public JobUpnpSonosInitial(Upnp aUpnp)
            {
                iUpnp = aUpnp;
            }

            public void Execute(Stack aStack)
            {
                iUpnp.OnSonosInitial();
            }

            private Upnp iUpnp;
        }

        internal class JobUpnpSonosRoomChanged : IJob
        {
            public JobUpnpSonosRoomChanged(Upnp aUpnp)
            {
                iUpnp = aUpnp;
            }

            public void Execute(Stack aStack)
            {
                iUpnp.OnSonosRoomChanged();
            }

            private Upnp iUpnp;
        }

        private static readonly ThreadPriority kPriority = ThreadPriority.Normal;

        public Stack(Linn.Topology.Layer0.IStack aLayer0, IEventUpnpProvider aEventServer)
        {
            iEventServer = aEventServer;

            iMutex = new Mutex();
            iJobList = new List<IJob>();
            iJobReady = new ManualResetEvent(false);

            iProductList = new SortedList<string, Product>();
            iUpnpList = new SortedList<string, Upnp>();

            iLayer0 = aLayer0;
            iLayer0.EventProductAdded += ProductAdded;
            iLayer0.EventProductRemoved += ProductRemoved;
            iLayer0.EventUpnpAdded += UpnpAdded;
            iLayer0.EventUpnpRemoved += UpnpRemoved;
        }

        internal void Select(string aPreampId, string aSourceId)
        {
        }

        internal void ScheduleJob(IJob aJob)
        {
            iMutex.WaitOne();
            iJobList.Add(aJob);
            iJobReady.Set();
            iMutex.ReleaseMutex();
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    iMutex.WaitOne();

                    if (iJobList.Count == 0)
                    {
                        iJobReady.Reset();

                        iMutex.ReleaseMutex();

                        iJobReady.WaitOne();

                        iMutex.WaitOne();
                    }

                    IJob job = iJobList[0];

                    iJobList.RemoveAt(0);

                    iMutex.ReleaseMutex();

                    job.Execute(this);
                }
            }
            catch (ThreadAbortException)
            {
                lock (iProductList)
                {
                    foreach (Product p in iProductList.Values)
                    {
                        p.Close();
                    }
                    iProductList.Clear();
                }
                lock (iUpnpList)
                {
                    foreach (Upnp u in iUpnpList.Values)
                    {
                        u.Close();
                    }
                    iUpnpList.Clear();
                }
            }
        }

        public void Start(IPAddress aInterface)
        {
            iRunning = true;
            Assert.Check(iThread == null);

            iThread = new Thread(new ThreadStart(Run));
            iThread.Priority = kPriority;
            iThread.Name = "Topology Layer 1";

            iThread.Start();
            iLayer0.Start(aInterface);

            Trace.WriteLine(Trace.kTopology, "Layer1.Stack.Start() successful");
        }

        public void Stop()
        {
            if (iThread != null)
            {
                iRunning = false;
                ScheduleJob(new JobAbort());
                iLayer0.Stop();
                iThread.Join();
                iThread = null;
                iProductList.Clear();
                iUpnpList.Clear();

                Trace.WriteLine(Trace.kTopology, "Layer1.Stack.Stop() successful");
            }
            else
            {
                Trace.WriteLine(Trace.kTopology, "Layer1.Stack.Stop() already stopped - silently do nothing");
            }
        }

        public void Rescan()
        {
            // rescan

            iLayer0.Rescan();
        }

        private void ProductAdded(object obj, Layer0.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer1 Product+         " + e.Device);

            Product product;
            lock (iProductList)
            {
                iProductList.TryGetValue(e.Device.Udn, out product);

                if (product == null && iRunning)
                {
                    try
                    {
                        product = new Product(this, e.Device);
                        product.EventSubscriptionError += EventSubscriptionErrorHandler;
                        iProductList.Add(e.Device.Udn, product);

                        product.Open();
                    }
                    catch (ServiceException ex)
                    {
                        Trace.WriteLine(Trace.kTopology, "Add device failed: " + e.Device + ", \n" + ex.Message + "\n" + ex.StackTrace);
                        UserLog.WriteLine("Add device failed: " + e.Device);
                    }
                }
            }
        }

        private void ProductRemoved(object obj, Layer0.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer1 Product-         " + e.Device);

            Product product;
            lock (iProductList)
            {
                iProductList.TryGetValue(e.Device.Udn, out product);

                if (product != null)
                {
                    iProductList.Remove(e.Device.Udn);

                    product.Kill();
                    product.EventSubscriptionError -= EventSubscriptionErrorHandler;
                }
            }
        }

        private void UpnpAdded(object obj, Layer0.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer1 Upnp+            " + e.Device);

            Upnp upnp;
            lock (iUpnpList)
            {
                iUpnpList.TryGetValue(e.Device.Udn, out upnp);

                if (upnp == null && iRunning)
                {
                    upnp = new Upnp(this, e.Device);
                    upnp.EventSubscriptionError += EventSubscriptionErrorHandler;
                    iUpnpList.Add(e.Device.Udn, upnp);

                    upnp.Open();
                }
            }
        }

        private void UpnpRemoved(object obj, Layer0.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer1 Upnp-            " + e.Device);

            Upnp upnp;
            lock (iUpnpList)
            {
                iUpnpList.TryGetValue(e.Device.Udn, out upnp);

                if (upnp != null)
                {
                    iUpnpList.Remove(e.Device.Udn);

                    upnp.EventSubscriptionError -= EventSubscriptionErrorHandler;
                    upnp.Kill();
                }
            }
        }

        public IEventUpnpProvider EventServer
        {
            get
            {
                return (iEventServer);
            }
        }

        private void EventSubscriptionErrorHandler(object sender, Layer0.EventArgsDevice e)
        {
            string message = String.Format("{0}: Removing disconnected device: {1}", DateTime.Now, e.Device.Udn);
            UserLog.WriteLine(message);
            Trace.WriteLine(Trace.kTopology, message);
            RemoveDevice(e.Device);
        }

        public void RemoveDevice(Device aDevice)
        {
            if (aDevice.IsLinn)
            {
                iLayer0.RemoveProduct(aDevice);
            }
            else
            {
                iLayer0.RemoveUpnp(aDevice);
            }
        }

        private Mutex iMutex;
        private List<IJob> iJobList;
        private ManualResetEvent iJobReady;
        private Thread iThread;

        private Linn.Topology.Layer0.IStack iLayer0;

        public IEventUpnpProvider iEventServer;

        private SortedList<string, Product> iProductList;
        private SortedList<string, Upnp> iUpnpList;
        private bool iRunning;
    }
}

