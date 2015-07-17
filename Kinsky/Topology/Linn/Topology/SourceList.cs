using System;
using System.Collections.Generic;

namespace Linn.Topology
{
    public class SourceList
    {

        public class EventArgsSource : EventArgs
        {
            public EventArgsSource(Source aSource)
            {
                iSource = aSource;
            }

            public Source Source
            {
                get
                {
                    return (iSource);
                }
            }

            private Source iSource;
        }

        public SourceList(House aHouse, Predicate<Source> aPredicate)
        {
            iHouse = aHouse;
            iPredicate = aPredicate;

            iSourceList = new List<Source>();
        }

        public void Start()
        {
            lock(this)
            {
               iHouse.Lock();

                iHouse.EventRoomAdded += RoomAdded;
                iHouse.EventRoomRemoved += RoomRemoved;

                foreach (Room r in iHouse.RoomList)
                {
                    r.Lock();

                    r.EventSourceAdded += SourceAdded;
                    r.EventSourceRemoved += SourceRemoved;

                    foreach(Source s in r.SourceList)
                    {
                        if (iPredicate(s))
                        {
                            iSourceList.Add(s);
                        }
                    }

                    r.Unlock();
                }

                iHouse.Unlock();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                iHouse.Lock();

                iHouse.EventRoomAdded -= RoomAdded;
                iHouse.EventRoomRemoved -= RoomRemoved;

                foreach (Room r in iHouse.RoomList)
                {
                    r.EventSourceAdded -= SourceAdded;
                    r.EventSourceRemoved -= SourceRemoved;
                }

                iHouse.Unlock();

                iSourceList.Clear();
            }
        }

        public IList<Source> Sources
        {
            get
            {
                lock (this)
                {
                    return new List<Source>(iSourceList).AsReadOnly();
                }
            }
        }

        public event EventHandler<EventArgsSource> EventSourceAdded;
        public event EventHandler<EventArgsSource> EventSourceRemoved;

        private void RoomAdded(object sender, EventArgsRoom e)
        {
            lock (this)
            {
                e.Room.EventSourceAdded += SourceAdded;
                e.Room.EventSourceRemoved += SourceRemoved;
            }
        }

        private void RoomRemoved(object sender, EventArgsRoom e)
        {
            IList<Source> sourceList = new List<Source>();

            lock (this)
            {
                e.Room.EventSourceAdded -= SourceAdded;
                e.Room.EventSourceRemoved -= SourceRemoved;
                Room room = e.Room as Room;
                room.Lock();

                foreach (Source s in room.SourceList)
                {
                    if (iSourceList.Remove(s))
                    {
                        sourceList.Add(s);
                    }
                }

                room.Unlock();
            }

            foreach(Source s in sourceList)
            {
                if (EventSourceRemoved != null)
                {
                    EventSourceRemoved(this, new EventArgsSource(s));
                }
            }
        }

        private void SourceAdded(object sender, Topology.EventArgsSource e)
        {
            bool added = false;

            lock (this)
            {
                if (iPredicate(e.Source as Source))
                {
                    iSourceList.Add(e.Source as Source);

                    added = true;
                }
            }

            if (added && EventSourceAdded != null)
            {
                EventSourceAdded(this, new EventArgsSource(e.Source as Source));
            }
        }

        private void SourceRemoved(object sender, Topology.EventArgsSource e)
        {
            bool removed = false;
            lock (this)
            {
                if (iSourceList.Remove(e.Source as Source))
                {
                    removed = true;
                }
            }

            if (removed && EventSourceRemoved != null)
            {
                EventSourceRemoved(this, new EventArgsSource(e.Source as Source));
            }
        }

        private House iHouse;
        private Predicate<Source> iPredicate;
        private List<Source> iSourceList;
    }
}