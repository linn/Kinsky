using System.Collections.Generic;
using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Text;

using Linn;
using Linn.Topology;

using Upnp;

namespace Linn.Kinsky
{
    public interface IBrowser
    {
        void Lock();
        void Unlock();

        void Refresh();

        void Up(uint aLevels);
        void Down(container aContainer);
        void Browse(Location aLocation);

        string SelectedId { get; }
        Location Location { get; }

        event EventHandler<EventArgs> EventLocationChanged;
    }

    public class Browser : IBrowser
    {
        public Browser(Location aLocation)
        {
            iMutex = new Mutex(false);

            iSelectedId = string.Empty;
            iLocation = aLocation;

            for (int i = 0; i < iLocation.Containers.Count; i++)
            {
                IContainer container = iLocation.Containers[i];
                container.EventContentAdded += ContentAdded;
                container.EventContentRemoved += ContentRemoved;
                container.EventContentUpdated += ContentUpdated;
                container.EventTreeChanged += TreeChanged;
            }
        }

        public void Refresh()
        {
            if (EventLocationChanged != null)
            {
                EventLocationChanged(this, EventArgs.Empty);
            }
        }

        public void Up(uint aLevels)
        {
            Up(aLevels, true);
        }

        public void Up(uint aLevels, bool aRaiseLocationChangedEvent)
        {
            Lock();

            for (uint i = 0; i < aLevels && iLocation.Containers.Count > 1; ++i)
            {
                IContainer container = iLocation.Current;
                container.EventContentAdded -= ContentAdded;
                container.EventContentRemoved -= ContentRemoved;
                container.EventContentUpdated -= ContentUpdated;
                container.EventTreeChanged -= TreeChanged;

                iSelectedId = container.Metadata.Id;
                iLocation = iLocation.PreviousLocation();
            }

            Unlock();

            if (EventLocationChanged != null && aRaiseLocationChangedEvent)
            {
                EventLocationChanged(this, EventArgs.Empty);
            }
        }

        public void Down(container aContainer)
        {
            Down(aContainer, true);
        }

        public void Down(container aContainer, bool aRaiseLocationChangedEvent)
        {
            Lock();

            IContainer container = iLocation.Current.ChildContainer(aContainer);

            if (container != null)
            {
                iSelectedId = string.Empty;
                iLocation = new Location(iLocation, container);

                container.EventContentAdded += ContentAdded;
                container.EventContentRemoved += ContentRemoved;
                container.EventContentUpdated += ContentUpdated;
                container.EventTreeChanged += TreeChanged;
            }

            Unlock();

            if (container != null && EventLocationChanged != null && aRaiseLocationChangedEvent)
            {
                EventLocationChanged(this, EventArgs.Empty);
            }
        }

        public void Browse(Location aLocation)
        {
            Lock();

            // Browse up to the root
            Up((uint)(iLocation.Containers.Count - 1), false);

            // Browse down to the new location
            for (int i = 1 ; i < aLocation.Containers.Count; i++)
            {
                container next = aLocation.Containers[i].Metadata;
                Down(next, false);
            }

            Unlock();

            if (EventLocationChanged != null)
            {
                EventLocationChanged(this, EventArgs.Empty);
            }
        }

        public void Lock()
        {
            iMutex.WaitOne();
        }

        public void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        public string SelectedId
        {
            get
            {
                return iSelectedId;
            }
        }

        public Location Location
        {
            get
            {
                return iLocation;
            }
        }

        public event EventHandler<EventArgs> EventLocationChanged;

        private void ContentAdded(object sender, EventArgs e)
        {
            Lock();
            IContainer current = iLocation.Current;
            Unlock();
            if (sender == current)
            {
                if (EventLocationChanged != null)
                {
                    EventLocationChanged(this, EventArgs.Empty);
                }
            }
        }

        private void ContentRemoved(object sender, EventArgsContentRemoved e)
        {
            Lock();
            bool changed = false;
            int index = iLocation.Containers.IndexOf(sender as IContainer);
            if (index != -1 && (index == iLocation.Containers.Count - 1 || iLocation.Containers[index + 1].Id == e.Id))
            {
                changed = true;
            }
            Unlock();

            if (changed)
            {
                if (EventLocationChanged != null)
                {
                    EventLocationChanged(this, EventArgs.Empty);
                }
            }
        }

        private void ContentUpdated(object sender, EventArgs e)
        {
            Lock();
            IContainer current = iLocation.Current;
            Unlock();
            if (sender == current)
            {
                if (EventLocationChanged != null)
                {
                    EventLocationChanged(this, EventArgs.Empty);
                }
            }
        }

        private void TreeChanged(object sender, EventArgs e)
        {
            Lock();
            IContainer current = iLocation.Current;
            Unlock();
            if (current.HasTreeChangeAffectedLeaf)
            {
                if (EventLocationChanged != null)
                {
                    EventLocationChanged(this, EventArgs.Empty);
                }
            }
        }

        private Mutex iMutex;

        private Location iLocation;
        private string iSelectedId;
    }
} // Linn.Kinsky

