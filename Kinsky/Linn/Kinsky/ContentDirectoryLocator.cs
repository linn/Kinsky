using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using Upnp;

namespace Linn.Kinsky
{
    public class Location
    {
        public Location(IContainer aContainer)
        {
            iContainers = new List<IContainer>();
            iContainers.Add(aContainer);
        }

        public Location(IList<IContainer> aContainers)
        {
            Assert.Check(aContainers.Count > 0);
            iContainers = new List<IContainer>(aContainers);
        }

        public Location(Location aLocation, IContainer aContainer)
        {
            iContainers = new List<IContainer>(aLocation.Containers);
            iContainers.Add(aContainer);
        }

        public IList<IContainer> Containers
        {
            get
            {
                return iContainers.AsReadOnly();
            }
        }

        public Location PreviousLocation()
        {
            if (iContainers.Count > 1)
            {
                Location location = new Location(iContainers.GetRange(0, iContainers.Count - 1));
                return location;
            }
            else
            {
                return null;
            }
        }

        public Location NextLocation()
        {
            if (iContainers.Count > 1)
            {
                Location location = new Location(iContainers.GetRange(1, iContainers.Count));
                return location;
            }
            else
            {
                return null;
            }
        }

        public IContainer Current
        {
            get
            {
                return iContainers[iContainers.Count - 1];
            }
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach (IContainer c in iContainers)
            {
                result += string.Format("{0}/", c.Metadata.Title);
            }

            return result;
        }

        public BreadcrumbTrail BreadcrumbTrail
        {
            get
            {
                List<Breadcrumb> trail = new List<Breadcrumb>();
                foreach (IContainer c in iContainers)
                {
                    trail.Add(new Breadcrumb(c.Metadata.Id, c.Metadata.Title));
                }
                return new BreadcrumbTrail(trail);
            }
        }

        private List<IContainer> iContainers;
    }


    public class LocatorAsync
    {
        public LocatorAsync(IContainer aRootContainer, BreadcrumbTrail aBreadcrumbTrail)
        {
            iRootContainer = aRootContainer;
            iBreadcrumbTrail = aBreadcrumbTrail;
        }

        public delegate void DFinished(LocatorAsync aLocator, Location aLocation);

        public void Locate(DFinished aCallback)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                if (aCallback != null)
                {
                    aCallback(this, Locator.Locate(iRootContainer, iBreadcrumbTrail));
                }
            }));
            t.IsBackground = true;
            t.Name = "Locator";
            t.Start();
        }

        private IContainer iRootContainer;
        private BreadcrumbTrail iBreadcrumbTrail;
    }


    public class Locator
    {
        public static Location Locate(Location aLocation)
        {
            Location location = new Location(aLocation.Containers[0]);
            List<string> relativeLocation = new List<string>();
            for (int i = 1; i < aLocation.Containers.Count; ++i)
            {
                relativeLocation.Add(aLocation.Containers[i].Metadata.Id);
            }
            return Locate(location, relativeLocation);
        }

        public static Location Locate(Location aLocation, List<string> aRelativeLocation)
        {
            if (aRelativeLocation.Count > 0)
            {
                IContainer container = aLocation.Current;
                try
                {
                    uint count = container.Open();

                    uint index = 0;
                    while (index < count)
                    {
                        DidlLite didl = container.Items(index, kCountPerCall);

                        foreach (upnpObject o in didl)
                        {
                            if (o is container)
                            {
                                if (o.Id == aRelativeLocation[0])
                                {
                                    Location location = new Location(aLocation, container.ChildContainer(o as container));
                                    if (aRelativeLocation.Count > 1)
                                    {
                                        List<string> relativeLocation = aRelativeLocation.GetRange(1, aRelativeLocation.Count - 1);
                                        return Locate(location, relativeLocation);
                                    }
                                    return location;
                                }
                            }
                        }

                        index += (uint)didl.Count;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(Trace.kKinsky, "Error retrieving container content (" + e.Message + ")");
                }
                finally
                {
                    container.Close();
                }
            }

            return aLocation;
        }

        public static Location Locate(IContainer aParentContainer, BreadcrumbTrail aBreadcrumbTrail)
        {
            Assert.Check(aBreadcrumbTrail.Count > 0);
            if (aBreadcrumbTrail.Count == 1)
            {
                container c = aParentContainer.Metadata;
                if (c.Id == aBreadcrumbTrail[0].Id || c.Title == aBreadcrumbTrail[0].Title)
                {
                    return new Location(aParentContainer);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<IContainer> candidates = FindCandidateChildren(aParentContainer, aBreadcrumbTrail[1]);
                foreach (IContainer candidate in candidates)
                {
                    BreadcrumbTrail tail = aBreadcrumbTrail.TruncateStart(1);
                    Location loc = Locate(candidate, tail);
                    if (loc != null)
                    {
                        List<IContainer> containers = new List<IContainer>();
                        containers.Add(aParentContainer);
                        for (int i = 0; i < loc.Containers.Count; i++)
                        {
                            containers.Add(loc.Containers[i]);
                        }
                        return new Location(containers);
                    }
                }
                return null;
            }
        }

        private static List<IContainer> FindCandidateChildren(IContainer aParentContainer, Breadcrumb aBreadcrumb)
        {
            List<IContainer> result = new List<IContainer>();
            try
            {
                uint count = aParentContainer.Open();

                uint index = 0;
                while (index < count)
                {
                    DidlLite didl = aParentContainer.Items(index, kCountPerCall);

                    foreach (upnpObject o in didl)
                    {
                        if (o is container)
                        {
                            if (o.Id == aBreadcrumb.Id)
                            {
                                // give priority to an Id match, insert at index 0
                                result.Insert(0, aParentContainer.ChildContainer(o as container));
                            }
                            else if (o.Title == aBreadcrumb.Title)
                            {
                                result.Add(aParentContainer.ChildContainer(o as container));
                            }
                        }
                    }

                    index += (uint)didl.Count;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Trace.kKinsky, "Error retrieving container content (" + e.Message + ")");
            }
            return result;
        }

        private static readonly uint kCountPerCall = 100;
    }

    public interface IAppRestartHandler
    {
        void Restart();
    }

    public class ContentDirectoryLocator : IContainer
    {
        public ContentDirectoryLocator(PluginManager aPluginManager, IAppRestartHandler aAppRestartHandler)
        {
            iOptionPage = new OptionPage("Plugins");

            iMutex = new Mutex(false);

            iMetadata = new container();
            iMetadata.Id = kId;
            iMetadata.Title = kTitle;
            iMetadata.Restricted = false;
            iMetadata.Searchable = true;

            iEnabledContentDirectories = new SortedList<Key, IContainer>();
            iDisabledContentDirectories = new SortedList<Key, IContainer>();

            iAppRestartHandler = aAppRestartHandler;

            iPluginManager = aPluginManager;
            iPluginManager.EventPluginInstalled += PluginInstalled;
            iPluginManager.EventPluginUnInstalled += PluginUnInstalled;
            iPluginManager.EventRestartRequired += RestartRequired;
        }

        public IOptionPage OptionPage
        {
            get
            {
                return iOptionPage;
            }
        }

        public void Start()
        {
            iPluginManager.Start();
        }

        public void Stop()
        {
            iPluginManager.Stop();
        }

        public IContainer Root
        {
            get
            {
                return this;
            }
        }

        public uint Open()
        {
            iMutex.WaitOne();
            uint count = (uint)iEnabledContentDirectories.Count;
            iMutex.ReleaseMutex();
            return count;
        }

        public void Close()
        {
        }

        public void Refresh()
        {
            iMutex.WaitOne();

            foreach (IContainer c in iEnabledContentDirectories.Values)
            {
                c.Refresh();
            }

            iMutex.ReleaseMutex();
        }

        public IContainer ChildContainer(container aContainer)
        {
            iMutex.WaitOne();

            foreach (IContainer c in iEnabledContentDirectories.Values)
            {
                if (c.Metadata.Id == aContainer.Id)
                {
                    iMutex.ReleaseMutex();

                    return c;
                }
            }

            iMutex.ReleaseMutex();

            return null;
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
            foreach (upnpObject o in aDidlLite)
            {
                if (o.Res.Count == 1)
                {
                    bool result = (Path.GetExtension(o.Res[0].Uri) == PluginManager.kPluginExtension);
                    if (!result)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            foreach (upnpObject o in aDidlLite)
            {
                if (o.Res.Count == 1)
                {
                    iPluginManager.InstallPlugin(o.Res[0].Uri);
                }
            }
        }

        public bool HandleDelete(DidlLite aDidlLite)
        {
            foreach (upnpObject o in aDidlLite)
            {
                foreach (KeyValuePair<Key, IContainer> k in iEnabledContentDirectories)
                {
                    if (k.Value.Metadata.Id == o.Id)
                    {
                        if (!k.Key.CanDelete)
                        {
                            return false;
                        }
                    }
                }

                foreach (KeyValuePair<Key, IContainer> k in iDisabledContentDirectories)
                {
                    if (k.Value.Metadata.Id == o.Id)
                    {
                        if (!k.Key.CanDelete)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void Delete(string aId)
        {
            iMutex.WaitOne();

            foreach (KeyValuePair<Key, IContainer> k in iEnabledContentDirectories)
            {
                if (k.Value.Metadata.Id == aId)
                {
                    iMutex.ReleaseMutex();
                    Assert.Check(k.Key.CanDelete);

                    iPluginManager.UninstallPlugin(k.Key.Plugin);

                    return;
                }
            }

            foreach (KeyValuePair<Key, IContainer> k in iDisabledContentDirectories)
            {
                if (k.Value.Metadata.Id == aId)
                {
                    iMutex.ReleaseMutex();
                    Assert.Check(k.Key.CanDelete);

                    iPluginManager.UninstallPlugin(k.Key.Plugin);

                    return;
                }
            }

            iMutex.ReleaseMutex();
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

            try
            {
                iMutex.WaitOne();

                uint endIndex = aStartIndex + aCount;
                for (uint i = aStartIndex; i < endIndex && i < iEnabledContentDirectories.Count; ++i)
                {
                    didl.Add(iEnabledContentDirectories.Values[(int)i].Metadata);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
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

        protected void OnEventTreeChanged()
        {
            EventHandler<EventArgs> del = EventTreeChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
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
                return false;
            }
        }

        public OptionBool Add(string aId, IContainer aRoot)
        {
            return Add(new Key(aId), aRoot);
        }

        private OptionBool Add(Key aKey, IContainer aRoot)
        {
            OptionBool option = null;

            try
            {
                iMutex.WaitOne();

                int index = 0;
                for (int i = 0; i < iOptionPage.Options.Count; ++i, ++index)
                {
                    if (iOptionPage.Options[i].Name.CompareTo(aKey.Id) > 0)
                    {
                        break;
                    }
                }

                // This code can be removed once there is a decent way to install and uninstall plugins
                if (aKey.Id != "Library")
                {
                    bool state = true;
                    if (aKey.Id == "Movie Trailers" || aKey.Id == "Shoutcast" || aKey.Id == "WFMU")
                    {
                        state = false;
                    }

                    option = new OptionBool(aKey.Id, aKey.Id, "Enable or disable " + aKey.Id + " plugin", state);
                    option.EventValueChanged += ValueChanged;
                    iOptionPage.Insert(index, option);

                    if (option.Native)
                    {
                        iEnabledContentDirectories.Add(aKey, aRoot);
                        aRoot.EventContentAdded += PluginContentChanged;
                        aRoot.EventContentRemoved += PluginContentChanged;
                        aRoot.EventContentUpdated += PluginContentChanged;
                    }
                    else
                    {
                        iDisabledContentDirectories.Add(aKey, aRoot);
                    }
                }
                else
                {
                    iEnabledContentDirectories.Add(aKey, aRoot);
                    aRoot.EventContentAdded += PluginContentChanged;
                    aRoot.EventContentRemoved += PluginContentChanged;
                    aRoot.EventContentUpdated += PluginContentChanged;
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }

            if (option != null && option.Native && EventContentAdded != null)
            {
                EventContentAdded(this, EventArgs.Empty);
            }

            return option;
        }

        public void Remove(string aId)
        {
            Remove(new Key(aId));
        }

        private void Remove(Key aKey)
        {
            IContainer root = null;

            try
            {
                iMutex.WaitOne();

                if (iEnabledContentDirectories.ContainsKey(aKey) || iDisabledContentDirectories.ContainsKey(aKey))
                {
                    root = iEnabledContentDirectories[aKey];
                    if (root == null)
                    {
                        root = iDisabledContentDirectories[aKey];
                    }
                    root.EventContentAdded -= PluginContentChanged;
                    root.EventContentRemoved -= PluginContentChanged;
                    root.EventContentUpdated -= PluginContentChanged;
                    // remove from the list
                    iEnabledContentDirectories.Remove(aKey);
                    iDisabledContentDirectories.Remove(aKey);

                    Option option = null;
                    foreach (Option o in iOptionPage.Options)
                    {
                        if (o.Id == aKey.Id)
                        {
                            option = o;
                            break;
                        }
                    }
                    if (option != null)
                    {
                        option.EventValueChanged -= ValueChanged;
                        iOptionPage.Remove(option);
                    }
                }

            }
            finally
            {
                iMutex.ReleaseMutex();
            }

            if (EventContentRemoved != null)
            {
                EventContentRemoved(this, new EventArgsContentRemoved(root.Metadata.Id));
            }
        }

        private void PluginInstalled(object sender, PluginManager.EventArgsPlugin e)
        {
            Add(new Key(e.Plugin), e.Plugin.MediaProvider.Root);
        }

        private void RestartRequired(object sender, EventArgs e)
        {
            iAppRestartHandler.Restart();
        }

        private void PluginUnInstalled(object sender, PluginManager.EventArgsPlugin e)
        {
            Remove(new Key(e.Plugin));
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            IContainer root = null;

            iMutex.WaitOne();

            OptionBool option = sender as OptionBool;
            Key key = new Key(option.Id);
            if (option.Native)
            {
                if (iDisabledContentDirectories.TryGetValue(key, out root))
                {
                    iDisabledContentDirectories.Remove(key);
                    iEnabledContentDirectories.Add(key, root);
                    root.EventContentAdded += PluginContentChanged;
                    root.EventContentRemoved += PluginContentChanged;
                    root.EventContentUpdated += PluginContentChanged;
                }
            }
            else
            {
                if (iEnabledContentDirectories.TryGetValue(key, out root))
                {
                    iEnabledContentDirectories.Remove(key);
                    iDisabledContentDirectories.Add(key, root);
                    root.EventContentAdded -= PluginContentChanged;
                    root.EventContentRemoved -= PluginContentChanged;
                    root.EventContentUpdated -= PluginContentChanged;
                }
            }

            iMutex.ReleaseMutex();

            if (option.Native && root != null && EventContentAdded != null)
            {
                EventContentAdded(this, EventArgs.Empty);
            }

            if (!option.Native && root != null && EventContentRemoved != null)
            {
                EventContentRemoved(this, new EventArgsContentRemoved(root.Metadata.Id));
            }
        }

        private void PluginContentChanged(object sender, EventArgs e)
        {
            EventHandler<EventArgs> del = EventContentUpdated;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private class Key : IComparable<Key>
        {
            public Key(string aId)
            {
                iPlugin = null;
                iId = aId;
            }

            public Key(Plugin aPlugin)
            {
                iPlugin = aPlugin;
                iId = aPlugin.MediaProvider.Name;
            }

            public int CompareTo(Key aKey)
            {
                //return iId.CompareTo(aKey.iId);
                int index1 = iPluginOrder.IndexOf(iId);
                int index2 = iPluginOrder.IndexOf(aKey.iId);

                if (index1 == -1)
                {
                    return 1;
                }

                if (index2 == -1)
                {
                    return -1;
                }

                if (index1 == index2)
                {
                    return 0;
                }
                else if (index1 < index2)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            public string Id
            {
                get { return iId; }
            }

            public Plugin Plugin
            {
                get { return iPlugin; }
            }

            public bool CanDelete
            {
                get { return (iPlugin != null && !iPlugin.IsStandardPlugin); }
            }

            private List<string> iPluginOrder = new List<string>(new string[] { "Library", "Folder", "Itunes", "Local Playlists", "Shared Playlists", "BBC", "Shoutcast", "WFMU", "Movie Trailers" });
            private Plugin iPlugin;
            private string iId;
        }


        public static readonly string kId = "Home";

        private const string kTitle = "Home";

        private Mutex iMutex;

        private IAppRestartHandler iAppRestartHandler;
        private PluginManager iPluginManager;

        private IOptionPage iOptionPage;

        private container iMetadata;
        private SortedList<Key, IContainer> iEnabledContentDirectories;
        private SortedList<Key, IContainer> iDisabledContentDirectories;
    }
}