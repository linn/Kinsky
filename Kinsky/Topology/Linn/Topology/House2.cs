using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Net;

using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class EventArgsRoom : EventArgs
    {
        public EventArgsRoom(IRoom aRoom)
        {
            iRoom = aRoom;
        }

        public IRoom Room
        {
            get
            {
                return (iRoom);
            }
        }

        private IRoom iRoom;
    }

    public class EventArgsSource : EventArgs
    {
        public EventArgsSource(ISource aSource)
        {
            iSource = aSource;
        }

        public ISource Source
        {
            get
            {
                return (iSource);
            }
        }

        private ISource iSource;
    }

    public interface ISource
    {
        IGroup Group { get; }
        string Udn { get; }
        string Name { get; }
        string Type { get; }
        void Select();
        bool Standby { get; set; }
        IRoom Room { get; }
    }

    public interface IRoom
    {
        ReadOnlyCollection<ISource> Sources { get; }
        IPreamp Preamp { get; }
        string Name { get; }
        ISource Current { get; }
        bool Standby { get; }
        void SetStandby(bool aStandby);
        event EventHandler<EventArgsSource> EventSourceAdded;
        event EventHandler<EventArgsSource> EventSourceRemoved;
        event EventHandler<EventArgs> EventCurrentChanged;
        event EventHandler<EventArgs> EventStandbyChanged;
        event EventHandler<EventArgs> EventPreampChanged;
    }

    public interface IGroup
    {
        string Name { get; }
        bool HasInfo { get; }
        bool HasTime { get; }
    }

    public interface IHouse
    {
        void Start(IPAddress aInterface);
        void Stop();
        event EventHandler<EventArgsRoom> EventRoomAdded;
        event EventHandler<EventArgsRoom> EventRoomRemoved;
        IModelFactory ModelFactory { get; }
        void RemoveDevice(Device aDevice);
    }

    public interface IPreamp
    {
        string Type { get; }
    }

    public class Preamp : IPreamp
    {
        internal Preamp(Group aGroup, Layer1.IPreamp aPreamp)
        {
            iGroup = aGroup;
            iPreamp = aPreamp;
        }

        public string Type
        {
            get
            {
                return (iPreamp.Type);
            }
        }

        public Room Room
        {
            get
            {
                return (iGroup.Room);
            }
        }

        public House House
        {
            get
            {
                return (iGroup.House);
            }
        }

        public Device Device
        {
            get
            {
                return (iPreamp.Device);
            }
        }

        public override string ToString()
        {
            return (String.Format("Preamp({0})", iGroup));
        }

        private Group iGroup;
        private Layer1.IPreamp iPreamp;
    }


    public class Source : ISource
    {
        internal Source(Group aGroup, uint aIndex, Layer1.ISource aSource)
        {
            iGroup = aGroup;
            iIndex = aIndex;
            iSource = aSource;
            iChildList = new List<Group>();
            UpdateDisplayName();
        }

        private void UpdateDisplayName()
        {
            iFullName = iGroup.Name + " (" + iSource.Name + ")";
        }

        public void Select()
        {
            iGroup.Select(iIndex);
        }

        public Room Room
        {
            get
            {
                return (iGroup.Room);
            }
        }

        public House House
        {
            get
            {
                return (iGroup.House);
            }
        }

        public string FullName
        {
            get
            {
                return (iFullName);
            }
        }

        public string Name
        {
            get
            {
                return (iSource.Name);
            }
        }

        public string Type
        {
            get
            {
                return (iSource.Type);
            }
        }

        public bool Visible
        {
            get
            {
                return (iSource.Visible);
            }
        }

        public Group Group
        {
            get
            {
                return (iGroup);
            }
        }

        public Device Device
        {
            get
            {
                return (iSource.Device);
            }
        }

        internal bool IsDescendedFrom(Group aGroup)
        {
            Group group = iGroup;

            while (true)
            {
                if (group == aGroup)
                {
                    return (true);
                }

                Source source = group.Parent;

                if (source == null)
                {
                    return (false);
                }

                group = source.Group;

                if (group == iGroup) // circularity check
                {
                    return (false);
                }
            }
        }

        internal Preamp MasterPreamp
        {
            get
            {
                return (iGroup.MasterPreamp);
            }
        }

        internal Group MasterGroup
        {
            get
            {
                return (iGroup.MasterGroup);
            }
        }

        public void RegisterChild(Group aGroup)
        {
            iChildList.Add(aGroup);
            aGroup.Parent = this;
        }

        public void DeregisterChild(Group aGroup)
        {
            iChildList.Remove(aGroup);
            aGroup.Parent = null;

            if (iChildList.Count == 0)
            {
                if (iSource.Visible)
                {
                    iGroup.Room.Add(this);
                }
            }
        }

        internal void Kill()
        {
            if (iChildList.Count > 0)
            {
                foreach (Group group in iChildList)
                {
                    group.Parent = null;
                }

                iChildList.Clear();
            }
            else if (iSource.Visible)
            {
                iGroup.Room.Remove(this);
            }
        }

        internal Group Child
        {
            get
            {
                if (iChildList.Count > 0)
                {
                    return (iChildList[0]);
                }

                return (null);
            }
        }

        internal Source MostDescended
        {
            get
            {
                Source source = this;
                Group child = source.Child;

                while (child != null)
                {
                    source = child.CurrentSource;
                    child = source.Child;
                    //detection code for ticket #679
                    if (child == this.Child)
                    {
                        IList<Source> sources = new List<Source>();
                        IList<Group> groups = new List<Group>();
                        Source s = this;
                        Group g = s.Child;
                        while (g != null)
                        {
                            sources.Add(s);
                            groups.Add(g);
                            s = g.CurrentSource;
                            g = s.Child;
                            if (g == this.Child)
                            {
                                break;
                            }
                        }

                        UserLog.WriteLine("!!!!!Infinite loop detected!!!!!");
                        foreach (Source sr in sources)
                        {
                            UserLog.WriteLine("Source: " + sr.FullName);
                        }
                        foreach (Group gr in groups)
                        {
                            UserLog.WriteLine("Group: " + gr.Name);
                        }
                        throw new Exception("!!!!!Infinite loop detected!!!!!");
                    }
                }
                return (source);
            }
        }

        internal bool Active
        {
            get
            {
                if (iChildList.Count > 0)
                {
                    return (false);
                }

                return (iSource.Visible);
            }
        }

        internal void Update(Layer1.ISource aSource)
        {
            if (Active)
            {
                iGroup.Room.Remove(this);
            }

            if (iSource.Name != aSource.Name)
            {
                if (iChildList.Count > 0)
                {
                    foreach (Group child in iChildList)
                    {
                        child.Parent = null;
                    }

                    iChildList.Clear();
                }

                iSource = aSource;
                UpdateDisplayName();

                iGroup.Room.Unorphan(this);
            }
            else
            {
                iSource = aSource;
            }

            if (Active)
            {
                iGroup.Room.Add(this);
            }
        }

        public override string ToString()
        {
            return (String.Format("Source({0}, {1}, {2})", iGroup, FullName, Type));
        }

        private Group iGroup;
        private uint iIndex;
        private Layer1.ISource iSource;

        private string iFullName;
        private List<Group> iChildList;

        #region ISource Members

        IGroup ISource.Group
        {
            get { return Group; }
        }

        string ISource.Udn
        {
            get { return Device.Udn; }
        }

        public bool Standby
        {
            get
            {
                return iGroup.Standby;
            }
            set
            {
                iGroup.SetStandby(value);
            }
        }

        #endregion

        #region ISource Members


        IRoom ISource.Room
        {
            get { return this.Room; }
        }

        #endregion
    }


    public class Group : IGroup
    {
        public Group(Room aRoom, Layer1.IGroup aGroup)
        {
            iRoom = aRoom;
            iGroup = aGroup;

            iSourceList = new List<Source>();

            iMasterGroup = this;

            if (iGroup.Preamp != null)
            {
                iPreamp = new Preamp(this, iGroup.Preamp);
                iMasterPreamp = iPreamp;
            }
            else
            {
                iMasterPreamp = null;
            }

            InitialiseSources();

            iGroup.EventSourceChanged += SourceChanged;
            iGroup.EventCurrentSourceChanged += CurrentSourceChanged;
            iGroup.EventStandbyChanged += StandbyChanged;
        }

        private void StandbyChanged(object sender, EventArgs args)
        {
            if (EventStandbyChanged != null)
            {
                EventStandbyChanged(this, EventArgs.Empty);
            }
        }

        public Room Room
        {
            get
            {
                return (iRoom);
            }
        }

        public House House
        {
            get
            {
                return (iRoom.House);
            }
        }

        public string Name
        {
            get
            {
                return (iGroup.Name);
            }
        }

        public Preamp Preamp
        {
            get
            {
                return (iPreamp);
            }
        }

        internal Preamp MasterPreamp
        {
            get
            {
                return (iMasterPreamp);
            }
        }

        internal Group MasterGroup
        {
            get
            {
                return (iMasterGroup);
            }
        }

        private void ForceSelect(uint aIndex)
        {
            // Select source for this room without receiving source change
            // event from the device. This is used when the room has no current
            // source or when moving between two island groups with no connecting
            // parent (i.e. their master groups are different);

            Source source = iSourceList[(int)aIndex];
            iRoom.SetCurrentSourceAndPreamp(source);
        }

        public void Select(uint aIndex)
        {
            if (iRoom.Current != null)
            {
                if (iMasterGroup != iRoom.Current.MasterGroup)
                {
                    ForceSelect(aIndex);
                }
            }
            else
            {
                ForceSelect(aIndex);
            }

            iGroup.Select(aIndex);

            if (iParent != null)
            {
                iParent.Select();
            }
        }

        public void SetStandby(bool aStandby)
        {
            iGroup.SetStandby(aStandby);
        }

        public Source Find(string aSurname)
        {
            foreach (Source source in iSourceList)
            {
                if (source.Name == aSurname)
                {
                    return (source);
                }
            }
            return (null);
        }

        public bool Represents(Layer1.IGroup aGroup)
        {
            return (iGroup == aGroup);
        }

        public Source Parent
        {
            get
            {
                return (iParent);
            }
            set
            {
                iParent = value;

                if (iParent != null)
                {
                    iMasterGroup = iParent.MasterGroup;
                    iMasterPreamp = iParent.MasterPreamp;
                }
                else
                {
                    iMasterGroup = this;
                    iMasterPreamp = iPreamp;
                }

                iRoom.ParentChanged(this);
            }
        }

        private void InitialiseSources()
        {
            uint count = iGroup.SourceCount;

            for (uint index = 0; index < count; index++)
            {
                Source source = new Source(this, index, iGroup.Source(index));

                iSourceList.Add(source);
            }

            foreach (Source source in iSourceList)
            {
                iRoom.Unorphan(source);

                if (source.Active)
                {
                    iRoom.Add(source);
                }
            }
        }

        internal void Kill()
        {
            foreach (Source source in iSourceList)
            {
                source.Kill();
            }
        }

        private void SourceChanged(object obj, Layer1.EventArgsSource e)
        {
            uint index = e.Index;

            Source source = iSourceList[(int)index];
            source.Update(iGroup.Source(index));
        }

        public Source CurrentSource
        {
            get
            {
                uint index = iGroup.CurrentSource;
                Assert.Check(index < iSourceList.Count);
                return (iSourceList[(int)index]);
            }
        }

        private void CurrentSourceChanged(object obj, EventArgs e)
        {
            iRoom.SetCurrentSourceAndPreamp(CurrentSource.MostDescended);
        }

        public override string ToString()
        {
            return (String.Format("Group({0}, {1})", Room, Name));
        }

        public bool Standby
        {
            get
            {
                return iGroup.Standby;
            }
        }

        public bool HasInfo
        {
            get
            {
                return iGroup.HasInfo;
            }
        }

        public bool HasTime
        {
            get
            {
                return iGroup.HasTime;
            }
        }

        Room iRoom;

        Layer1.IGroup iGroup;

        Source iParent;

        List<Source> iSourceList;

        Preamp iPreamp;
        Preamp iMasterPreamp;

        Group iMasterGroup;

        public event EventHandler<EventArgs> EventStandbyChanged;
    }


    public class Room : IRoom
    {

        internal Room(House aHouse, string aName)
        {
            iHouse = aHouse;
            iName = aName;

            iMutex = new Mutex();

            iGroupList = new List<Group>();
            iSourceList = new List<Source>();
        }

        public bool Standby
        {
            get
            {
                Lock();

                List<Group> groups = new List<Group>(iGroupList);

                Unlock();

                foreach (Group g in groups)
                {
                    if (!g.Standby)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void SetStandby(bool aValue)
        {
            Lock();

            List<Group> groups = new List<Group>(iGroupList);

            Unlock();

            foreach (Group group in groups)
            {
                group.SetStandby(aValue);
            }
        }

        public House House
        {
            get
            {
                return (iHouse);
            }
        }

        public string Name
        {
            get
            {
                return (iName);
            }
        }

        ISource IRoom.Current
        {
            get
            {
                return Current;
            }
        }

        public Source Current
        {
            get
            {
                Lock();

                Source source = iCurrent;

                Unlock();

                return (source);
            }
        }

        public Preamp Preamp
        {
            get
            {
                Lock();

                Preamp preamp = iPreamp;

                Unlock();

                return (preamp);
            }
        }

        public IList<Source> SourceList
        {
            get
            {
                Lock();

                List<Source> list = new List<Source>(iSourceList);

                Unlock();

                return (list.AsReadOnly());
            }
        }

        public override string ToString()
        {
            return (String.Format("Room({0})", iName));
        }

        internal void HookUpParent(Group aGroup)
        {
            foreach (Group g in iGroupList)
            {
                Source source = g.Find(aGroup.Name);

                if (source != null)
                {
                    if (!source.IsDescendedFrom(aGroup)) // circular dependency protection
                    {
                        if (source.Active)
                        {
                            Remove(source);
                        }
                        source.RegisterChild(aGroup);
                    }
                }
            }
        }

        internal void Add(Layer1.IGroup aGroup)
        {
            Group group = new Group(this, aGroup);

            if (iGroupList.Count > 0)
            {
                HookUpParent(group);
            }
            else
            {
                SetCurrentSourceAndPreamp(group.CurrentSource);
            }

            Lock();

            iGroupList.Add(group);

            Unlock();
            group.EventStandbyChanged += StandbyChanged;
        }

        void StandbyChanged(object sender, EventArgs e)
        {
            if (EventStandbyChanged != null)
            {
                EventStandbyChanged(this, EventArgs.Empty);
            }
        }

        internal bool Remove(Layer1.IGroup aGroup)
        {
            foreach (Group group in iGroupList)
            {
                if (group.Represents(aGroup))
                {
                    Source parent = group.Parent;

                    if (parent != null)
                    {
                        if (iCurrent != null)
                        {
                            if (iCurrent.Group == group)
                            {
                                SetCurrentSourceAndPreamp(parent);
                            }
                        }

                        parent.DeregisterChild(group);
                    }
                    else
                    {
                        if (iCurrent != null)
                        {
                            if (iCurrent.Group == group)
                            {
                                SetCurrentSourceAndPreamp(null);
                            }
                        }
                    }

                    Lock();

                    iGroupList.Remove(group);

                    Unlock();

                    group.Kill();
                    group.EventStandbyChanged -= StandbyChanged;

                    return (iGroupList.Count == 0);
                }
            }
            return (false);
        }

        internal void ParentChanged(Group aGroup)
        {
            if (iCurrent != null)
            {
                SetCurrentSourceAndPreamp(iCurrent.MasterGroup.CurrentSource.MostDescended);
            }
        }

        internal void Unorphan(Source aSource)
        {
            foreach (Group group in iGroupList)
            {
                if (group.Parent == null)
                {
                    if (group.Name == aSource.Name && !aSource.IsDescendedFrom(group)) // circular dependency protection
                    {
                        aSource.RegisterChild(group);
                    }
                }
            }
        }

        internal void Add(Source aSource)
        {
            Trace.WriteLine(Trace.kTopology, "House Source+          " + aSource);
            //UserLog.WriteLine(DateTime.Now + ": House Source+          " + aSource);

            Lock();

            iSourceList.Add(aSource);

            Unlock();

            if (EventSourceAdded != null)
            {
                EventSourceAdded(this, new EventArgsSource(aSource));
            }
        }

        internal void Remove(Source aSource)
        {
            Trace.WriteLine(Trace.kTopology, "House Source-          " + aSource);
            //UserLog.WriteLine(DateTime.Now + ": House Source-          " + aSource);

            Lock();

            iSourceList.Remove(aSource);

            Unlock();

            if (EventSourceRemoved != null)
            {
                EventSourceRemoved(this, new EventArgsSource(aSource));
            }
        }

        internal void SetCurrentSourceAndPreamp(Source aSource)
        {
            Preamp preamp = null;

            Lock();

            if (iCurrent != aSource)
            {
                iCurrent = aSource;

                if (iCurrent != null)
                {
                    preamp = iCurrent.MasterPreamp;
                }

                Unlock();

                SetPreamp(preamp);

                Trace.WriteLine(Trace.kTopology, "House Source*          " + this);

                if (EventCurrentChanged != null)
                {
                    EventCurrentChanged(this, EventArgs.Empty);
                }
            }
            else
            {
                if (iCurrent != null)
                {
                    preamp = iCurrent.MasterPreamp;
                }

                Unlock();

                SetPreamp(preamp);
            }
        }

        internal void SetPreamp(Preamp aPreamp)
        {
            Lock();

            if (iPreamp != aPreamp)
            {
                iPreamp = aPreamp;

                Unlock();

                Trace.WriteLine(Trace.kTopology, "House Preamp*          " + this);

                if (EventPreampChanged != null)
                {
                    EventPreampChanged(this, EventArgs.Empty);
                }
            }
            else
            {
                Unlock();
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

        public event EventHandler<EventArgs> EventStandbyChanged;
        public event EventHandler<EventArgs> EventPreampChanged;
        public event EventHandler<EventArgsSource> EventSourceAdded;
        public event EventHandler<EventArgsSource> EventSourceRemoved;
        public event EventHandler<EventArgs> EventCurrentChanged;

        private House iHouse;
        private string iName;

        private Mutex iMutex;

        private List<Group> iGroupList;
        private List<Source> iSourceList;

        private Preamp iPreamp;

        private Source iCurrent;

        #region IRoom Members

        IPreamp IRoom.Preamp
        {
            get { return Preamp; }
        }

        ReadOnlyCollection<ISource> IRoom.Sources
        {
            get
            {
                List<ISource> sources = new List<ISource>();
                IList<Source> snapshot = SourceList;
                foreach (Source s in snapshot)
                {
                    sources.Add(s);
                }
                return new ReadOnlyCollection<ISource>(sources);
            }
        }
        #endregion
    }


    public class House : IHouse
    {

        public House(ISsdpNotifyProvider aListenerNotify, IEventUpnpProvider aEventServer, IModelFactory aModelFactory)
        {
            iModelFactory = aModelFactory;
            iEventServer = aEventServer;

            iMutex = new Mutex();

            Linn.Topology.Layer0.IStack layer0 = new Linn.Topology.Layer0.Stack(aListenerNotify);
            iLayer1 = new Linn.Topology.Layer1.Stack(layer0, iEventServer);

            iLayer1.EventGroupAdded += GroupAdded;
            iLayer1.EventGroupRemoved += GroupRemoved;

            iRoomList = new SortedList<string, Room>();
        }

        public House(Linn.Topology.Layer1.IStack aLayer1)
        {
            iMutex = new Mutex();

            iLayer1 = aLayer1;
            iLayer1.EventGroupAdded += GroupAdded;
            iLayer1.EventGroupRemoved += GroupRemoved;

            iRoomList = new SortedList<string, Room>();
        }

        public void Start(IPAddress aInterface)
        {
            // start the discovery system
            iLayer1.Start(aInterface);
        }

        public void Stop()
        {
            // stop the discovery system
            iLayer1.Stop();

            iRoomList.Clear();

            iModelFactory.ClearCache();
        }

        public void Rescan()
        {
            // rescan
            iLayer1.Rescan();
        }

        public IList<Room> RoomList
        {
            get
            {
                Lock();

                List<Room> list = new List<Room>(iRoomList.Values);

                Unlock();

                return (list.AsReadOnly());
            }
        }

        private Room Find(string aRoom)
        {
            Room room;

            iRoomList.TryGetValue(aRoom, out room);

            if (room == null)
            {
                room = new Room(this, aRoom);

                Lock();

                iRoomList.Add(aRoom, room);

                Unlock();

                Trace.WriteLine(Trace.kTopology, "House Room+            " + room);
                //UserLog.WriteLine(DateTime.Now + ": House Room+            " + room);

                if (EventRoomAdded != null)
                {
                    EventRoomAdded(this, new EventArgsRoom(room));
                }
            }

            return (room);
        }

        private void Remove(Room aRoom)
        {
            Lock();

            iRoomList.Remove(aRoom.Name);

            Unlock();

            Trace.WriteLine(Trace.kTopology, "House Room-            " + aRoom);
            //UserLog.WriteLine(DateTime.Now + ": House Room-            " + aRoom);

            if (EventRoomRemoved != null)
            {
                EventRoomRemoved(this, new EventArgsRoom(aRoom));
            }
        }

        private void GroupAdded(object obj, Layer1.EventArgsGroup e)
        {
            Room room = Find(e.Group.Room);

            room.Add(e.Group);
        }


        private void GroupRemoved(object obj, Layer1.EventArgsGroup e)
        {
            Room room = Find(e.Group.Room);

            if (room.Remove(e.Group))
            {
                Remove(room);
            }
        }

        // Lock functions

        internal void Lock()
        {
            iMutex.WaitOne();
        }

        internal void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        public IEventUpnpProvider EventServer
        {
            get
            {
                return (iEventServer);
            }
        }
        #region IHouse Members


        public IModelFactory ModelFactory
        {
            get { return iModelFactory; }
        }

        public event EventHandler<EventArgsRoom> EventRoomAdded;
        public event EventHandler<EventArgsRoom> EventRoomRemoved;

        #endregion

        public void RemoveDevice(Device aDevice)
        {
            iLayer1.RemoveDevice(aDevice);
        }

        IEventUpnpProvider iEventServer;

        private Mutex iMutex;
        private Linn.Topology.Layer1.IStack iLayer1;

        private SortedList<string, Room> iRoomList;
        private IModelFactory iModelFactory;

    }
}
