
using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;

using Linn;
using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;
using Linn.Topology;

using Upnp;

namespace Linn.Kinsky
{
    public interface IViewSaveSupport
    {
        void Save(IList<upnpObject> aList);
    }

    public delegate void PlaylistSaver(SaveSupport aSaveSupport);
    public class ViewSaveSupport : IViewSaveSupport
    {
        public ViewSaveSupport(PlaylistSaver aPlaylistSaver, SaveSupport aSaveSupport)
        {
            iPlaylistSaver = aPlaylistSaver;
            iSaveSupport = aSaveSupport;
        }

        public void Save(IList<upnpObject> aList)
        {
            // this should perhaps be handled in the UI?
            if(aList.Count == 0)
            {
                return;
            }

            // generate a default file title for the new playlist
            DateTime now = DateTime.Now;
            string defaultName = String.Format("{0:00}", now.Year)
                               + String.Format("{0:00}", now.Month)
                               + String.Format("{0:00}", now.Day)
                               + "_"
                               + String.Format("{0:00}", now.Hour)
                               + String.Format("{0:00}", now.Minute)
                               + String.Format("{0:00}", now.Second);

            iSaveSupport.SetDefaultName(defaultName);
            iSaveSupport.SetList(aList);
            iPlaylistSaver(iSaveSupport);
        }

        private PlaylistSaver iPlaylistSaver;
        private SaveSupport iSaveSupport;
    }

    public interface ISaveSupport
    {
        void Save(string aName, string aDescription, uint aImageId);
        bool Exists(string aName);

        string DefaultName { get; }

        IDictionary<uint, Uri> ImageList { get; }

        string SaveLocation { get; set; }
        IList<string> SaveLocations { get; }

        event EventHandler<EventArgs> EventSaveLocationChanged;
        event EventHandler<EventArgs> EventSaveLocationsChanged;
        event EventHandler<EventArgs> EventImageListChanged;
    }

    public class SaveSupport : ISaveSupport
    {
        public SaveSupport(HelperKinsky aHelper, SharedPlaylists aSharedPlaylists, OptionBool aOptionSharedPlaylists, LocalPlaylists aLocalPlaylists, OptionBool aOptionLocalPlaylists)
        {
            iInvoker = aHelper.Invoker;

            iOptionSaveLocation = new OptionString("savelocation", "Save Location", "Location to save playlists to", "Local Playlists");
            aHelper.AddOption(iOptionSaveLocation);

            iOptionSaveLocation.EventValueChanged += SaveLocationChanged;

            iSharedPlaylists = aSharedPlaylists;
            iSharedPlaylists.EventContentAdded += PlaylistsChanged;
            iSharedPlaylists.EventContentRemoved += PlaylistsChanged;
            iSharedPlaylists.EventContentUpdated += PlaylistsChanged;

            iOptionSharedPlaylists = aOptionSharedPlaylists;
            iOptionSharedPlaylists.EventValueChanged += PlaylistsChanged;

            iLocalPlaylists = aLocalPlaylists;
            iOptionLocalPlaylists = aOptionLocalPlaylists;

            iOptionLocalPlaylists.EventValueChanged += PlaylistsChanged;

            PlaylistsChanged(this, EventArgs.Empty);
        }

        internal void SetDefaultName(string aDefaultName)
        {
            iDefaultName = aDefaultName;
        }

        internal void SetList(IList<upnpObject> aList)
        {
            iList = aList;
        }

        public void Save(string aName, string aDescription, uint aImageId)
        {
            if(iOptionSaveLocation.Native == iLocalPlaylists.Metadata.Title)
            {
                iLocalPlaylists.Save(aName, iList);
            }
            else
            {
                ModelPlaylistManager pm = iSharedPlaylists.Find(iOptionSaveLocation.Native);
                if(pm == null)
                {
                    throw new PlaylistManagerNotFoundException();
                }

                ModelPlaylist p = pm.Insert(0, aName, aDescription, aImageId);

                DidlLite didl = new DidlLite();
                didl.AddRange(iList);
                p.Insert(0, didl);
            }
        }

        public bool Exists(string aName)
        {
            return false;
        }

        public string DefaultName
        {
            get
            {
                return iDefaultName;
            }
        }

        public IDictionary<uint, Uri> ImageList
        {
            get
            {
                if(iModelPlaylistManager != null)
                {
                    return iModelPlaylistManager.ImageList;
                }

                return new Dictionary<uint, Uri>();
            }
        }

        public string SaveLocation
        {
            get
            {
                return iOptionSaveLocation.Native;
            }
            set
            {
                iOptionSaveLocation.Native = value;
                SetImageList(value);
            }
        }

        public IList<string> SaveLocations
        {
            get
            {
                lock(this)
                {
                    return iSaveLocationsList.AsReadOnly();
                }
            }
        }

        public event EventHandler<EventArgs> EventSaveLocationChanged;
        public event EventHandler<EventArgs> EventSaveLocationsChanged;
        public event EventHandler<EventArgs> EventImageListChanged;

        private void SetImageList(string aSaveLocation)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if(iModelPlaylistManager != null)
            {
                iModelPlaylistManager.EventImagesListChanged -= EventImageListChanged;
            }

            iModelPlaylistManager = iSharedPlaylists.Find(aSaveLocation);

            if(iModelPlaylistManager != null)
            {
                iModelPlaylistManager.EventImagesListChanged += EventImageListChanged;
            }

            if(EventImageListChanged != null)
            {
                EventImageListChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DPlaylistsChanged(object sender, EventArgs e);
        private void PlaylistsChanged(object sender, EventArgs e)
        {
            Delegate del = new DPlaylistsChanged(delegate(object s, EventArgs a)
            {
                List<string> list = new List<string>();
    
                if(iOptionLocalPlaylists.Native)
                {
                    list.Add(iLocalPlaylists.Metadata.Title);
                }
    
                if(iOptionSharedPlaylists.Native)
                {
                    uint count = iSharedPlaylists.Open();
                    DidlLite didl = iSharedPlaylists.Items(0, count);
                    foreach(upnpObject o in didl)
                    {
                        list.Add(o.Title);
                    }
    
                    SetImageList(iOptionSaveLocation.Native);
                }
    
                lock(this)
                {
                    iSaveLocationsList = list;
                }
    
                if(EventSaveLocationsChanged != null)
                {
                    EventSaveLocationsChanged(this, EventArgs.Empty);
                }
            });
            if (iInvoker.TryBeginInvoke(del, new object[] { sender, e }))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private void SaveLocationChanged(object sender, EventArgs e)
        {
            if(EventSaveLocationChanged != null)
            {
                EventSaveLocationChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DImageListChanged(object sender, EventArgs e);
        private void ImageListChanged(object sender, EventArgs e)
        {
            Delegate del = new DImageListChanged(delegate(object s, EventArgs a)
            {
                if(EventImageListChanged != null)
                {
                    EventImageListChanged(this, EventArgs.Empty);
                }
            });
            if (iInvoker.TryBeginInvoke(del, new object[] { sender, e }))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private IInvoker iInvoker;

        private OptionString iOptionSaveLocation;

        private SharedPlaylists iSharedPlaylists;
        private OptionBool iOptionSharedPlaylists;
        private LocalPlaylists iLocalPlaylists;
        private OptionBool iOptionLocalPlaylists;

        private List<string> iSaveLocationsList;
        private ModelPlaylistManager iModelPlaylistManager;

        private string iDefaultName;
        private IList<upnpObject> iList;
    }
} // Linn.Kinsky
