using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Net;
using System.Xml;

using Upnp;

using Linn.Kinsky;
using Linn;

[assembly: ContentDirectoryFactoryType("OssKinskyMppWfmu.ContentDirectoryFactoryWfmu")]

namespace OssKinskyMppWfmu
{
    public class ContentDirectoryFactoryWfmu : IContentDirectoryFactory
    {
        public IContentDirectory Create(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            return (new ContentDirectoryWfmu(aDataPath, aSupport));
        }
    }

    public class ContentDirectoryWfmu : IContentDirectory, IContainer
    {
        IContentDirectorySupportV2 iSupport;

        private OptionPage iOptionPage;
        private OptionBool iOptionUk;

        private container iMetadata;

        private WebFetcher iWebFetcher;

        private Live iLive;
        private Podcasts iPodcasts;

        public ContentDirectoryWfmu(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            iSupport = aSupport;
            iOptionPage = new OptionPage("Wfmu");

            string installPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            iWebFetcher = new WebFetcher(aDataPath);

            string wfmuLogo = aSupport.VirtualFileSystem.Uri(Path.Combine(installPath, "Wfmu.png"));

            iLive = new Live(wfmuLogo);
            iPodcasts = new Podcasts(iWebFetcher, wfmuLogo);

            iMetadata = new container();
            iMetadata.Id = "wfmu";
            iMetadata.Title = "WFMU";
            iMetadata.AlbumArtUri.Add(wfmuLogo);
            iMetadata.ChildCount = 2;
        }

        public void Start()
        {
            iLive.Start();
            iPodcasts.Start();
        }

        public void Stop()
        {
            iLive.Stop();
            iPodcasts.Stop();
        }

        public uint Open()
        {
            lock (this)
            {
                return (2);
            }
        }

        public void Close() { }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            uint index = aStartIndex;
            uint count = aCount;

            while (count-- > 0)
            {
                if (index == 0)
                {
                    didl.Add(iLive.Metadata);
                }
                else if (index == 1)
                {
                    didl.Add(iPodcasts.Metadata);
                }
                else
                {
                    break;
                }

                index++;
            }

            return (didl);
        }

        public void Refresh()
        {
            iLive.Refresh();
            iPodcasts.Refresh();
        }

        public IContainer ChildContainer(container aContainer)
        {
            if (iLive.Metadata.Id == aContainer.Id)
            {
                return (iLive);
            }

            if (iPodcasts.Metadata.Id == aContainer.Id)
            {
                return (iPodcasts);
            }

            return (null);
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
            return (false);
        }

        public bool HandleInsert(DidlLite aDidlLite)
        {
            return (false);
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }

        public bool HandleDelete(DidlLite aDidlLite)
        {
            return (false);
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

        public string Name
        {
            get
            {
                return "WFMU";
            }
        }

        public string Company
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length != 0)
                {
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public IContainer Root
        {
            get
            {
                return this;
            }
        }

        public IOptionPage OptionPage
        {
            get
            {
                return iOptionPage;
            }
        }
    }


    internal class Live : IContainer, IDisposable
    {
        private container iMetadata;
        private List<LiveStation> iStations;

        public Live(string aAlbumArtUri)
        {
            iStations = new List<LiveStation>();

            iStations.Add(new LiveStation("wfmu", "WFMU", "Independent free form radio", aAlbumArtUri, "http://mp3stream.wfmu.org", 128));
            iStations.Add(new LiveStation("rns", "WFMU's Rock 'n' Soul Ichiban", "The best in obscuro 50's and 60's Rock 'n' Soul hits", aAlbumArtUri, "http://mp3stream.wfmu.org:443", 128));
            iStations.Add(new LiveStation("dodiy", "WFMU's Do or DIY", "Home of all things avant-retard", aAlbumArtUri, "http://do-or-diy.wfmu.org", 128));
            iStations.Add(new LiveStation("drummer", "WFMU's Give the Drummer Radio", "The finest in Micronesian doo-wop, Appalachian mambo, Turkish mariachi ...", aAlbumArtUri, "http://motherlode.wfmu.org:443", 128));
            iStations.Add(new LiveStation("ubu", "Ubu Sound / WFMU", "All types of sound art, historical and contemporary", aAlbumArtUri, "http://ubustream.wfmu.org", 128));

            iMetadata = new container();
            iMetadata.Id = "live";
            iMetadata.Title = "Live Radio";
            iMetadata.AlbumArtUri.Add(aAlbumArtUri);
            iMetadata.ChildCount = iStations.Count;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public uint Open()
        {
            return ((uint)iStations.Count);
        }

        public void Close() { }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            int index = (int)aStartIndex;
            int count = (int)aCount;
            int required = index + count;
            int items = iStations.Count;

            if (required > items)
            {
                count -= required - items;
            }

            DidlLite didl = new DidlLite();

            foreach (LiveStation station in iStations.GetRange(index, count))
            {
                didl.Add(station.Metadata);
            }

            return (didl);
        }

        public void Refresh()
        {
        }

        public IContainer ChildContainer(container aContainer)
        {
            return (null);
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
            return (false);
        }

        public bool HandleInsert(DidlLite aDidlLite)
        {
            return (false);
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }

        public bool HandleDelete(DidlLite aDidlLite)
        {
            return (false);
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

        public void Dispose()
        {
        }
    }

    internal class LiveStation
    {
        internal LiveStation(string aId, string aName, string aDescription, string aAlbumArtUri, string aAudioUri, int aKbps)
        {
            iId = aId;
            iName = aName;
            iDescription = aDescription;
            iAlbumArtUri = aAlbumArtUri;
            iAudioUri = aAudioUri;
            iKbps = aKbps * 125;

            UpdateMetadata();
        }

        private void UpdateMetadata()
        {
            iMetadata = new audioItem();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.Description = iDescription;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            resource res = new resource();
            res.Uri = iAudioUri;
            res.NrAudioChannels = 2;
            res.Bitrate = iKbps;
            res.ProtocolInfo = "http-get:*:audio/x-mpeg:*";

            iMetadata.Res.Add(res);
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        public audioItem Metadata
        {
            get
            {
                return iMetadata;
            }
        }

        private string iId;
        private string iName;
        private string iDescription;
        private string iAlbumArtUri;
        private string iAudioUri;
        private int iKbps;
        private audioItem iMetadata;
    }

    internal class Podcasts : IContainer, IDisposable
    {
        private WebFetcher iWebFetcher;
        private string iAlbumArtUri;

        private WebFile iWebFile;

        private List<PodcastShow> iShows;
        private string iInstallPath;
        private container iMetadata;

        private void WebFileContentsChanged(object obj, EventArgs e)
        {
            bool added = false;

            string contents = iWebFile.Contents;

            while (true)
            {

                int titleStart = contents.IndexOf("<b>") + 3;

                if (titleStart < 0)
                {
                    break;
                }

                int titleEnd = contents.IndexOf("</b>", titleStart);

                if (titleEnd < 0)
                {
                    break;
                }

                string title = contents.Substring(titleStart, titleEnd - titleStart);

                if (title.StartsWith("Subscribe"))
                {
                    contents = contents.Substring(titleEnd);
                    continue;
                }

                int idStart = contents.IndexOf("/playlists/", titleEnd) + 11;

                contents = contents.Substring(idStart);

                if (contents.Length >= 2)
                {
                    string id = contents.Substring(0, 2);

                    PodcastShow s = FindShow(id);

                    if (s == null)
                    {
                        s = new PodcastShow(id, title, iAlbumArtUri, iWebFetcher);

                        added = true;

                        lock (this)
                        {
                            iShows.Add(s);
                            iMetadata.ChildCount = iShows.Count;
                        }
                    }
                }

                if (added)
                {
                    if (EventContentAdded != null)
                    {
                        EventContentAdded(this, EventArgs.Empty);
                    }
                }
            }
        }


        PodcastShow FindShow(string aId)
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    if (show.Id == aId)
                    {
                        return (show);
                    }
                }

                return (null);
            }
        }

        string GetContents(XmlNode aNavigator, string aXpath)
        {
            XmlNode node = aNavigator.SelectSingleNode(aXpath);

            if (node == null)
            {
                return (null);
            }

            return (node.Value);
        }

        public Podcasts(WebFetcher aWebFetcher, string aAlbumArtUri)
        {
            iWebFetcher = aWebFetcher;
            iAlbumArtUri = aAlbumArtUri;

            iMetadata = new container();
            iMetadata.Id = "podcasts";
            iMetadata.Title = "Podcasts";
            iMetadata.AlbumArtUri.Add(aAlbumArtUri);

            iShows = new List<PodcastShow>();

            iWebFile = iWebFetcher.Create(new Uri("http://wfmu.org/podcast"), "podcasts.xml", 60);
            iWebFile.EventContentsChanged += WebFileContentsChanged;
            iWebFile.Open();
        }

        public void Start()
        {
        }

        public void Stop()
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    show.Dispose();
                }
            }
        }

        public uint Open()
        {
            lock (this)
            {
                return ((uint)iShows.Count);
            }
        }

        public void Close() { }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            lock (this)
            {
                int index = (int)aStartIndex;
                int count = (int)aCount;
                int required = index + count;
                int items = iShows.Count;

                if (required > items)
                {
                    count -= required - items;
                }

                foreach (PodcastShow show in iShows.GetRange(index, count))
                {
                    didl.Add(show.Metadata);
                }
            }

            return (didl);
        }

        public void Refresh()
        {
        }

        public IContainer ChildContainer(container aContainer)
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    if (show.Id == aContainer.Id)
                    {
                        return (show);
                    }
                }

                return (null);
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
            return (false);
        }

        public bool HandleInsert(DidlLite aDidlLite)
        {
            return (false);
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }

        public bool HandleDelete(DidlLite aDidlLite)
        {
            return (false);
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

        public void Dispose()
        {
            iWebFile.Dispose();
        }
    }


    internal class PodcastEpisode
    {
        internal PodcastEpisode(string aId, string aName, string aDate, string aAlbumArtUri, string aAudioUri, string aDuration)
        {
            iId = aId;
            iName = aName;
            iAlbumArtUri = aAlbumArtUri;
            iAudioUri = aAudioUri;

            DateTime date;

            try
            {
                date = DateTime.Parse(aDate);
                iDate = date;
            }
            catch (FormatException) { }

            iMetadata = new audioItem();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            resource res = new resource();
            res.Uri = iAudioUri;
            res.ProtocolInfo = "http-get:*:audio/x-mpeg:*";
            if (aDuration != null)
            {
                res.Duration = new Time(aDuration).ToString();
            }

            iMetadata.Res.Add(res);
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        public audioItem Metadata
        {
            get
            {
                return (iMetadata);
            }
        }

        private string iId;
        private string iName;
        private Nullable<DateTime> iDate;
        private string iAlbumArtUri;
        private string iAudioUri;
        private audioItem iMetadata;
    }

    internal class PodcastShow : IContainer, IDisposable
    {
        internal PodcastShow(string aId, string aName, string aAlbumArtUri, WebFetcher aWebFetcher)
        {
            iId = aId;
            iName = aName;
            iAlbumArtUri = aAlbumArtUri;
            // iAlbumArtUri = "http://wfmu.org/podcast_images/" + iId.ToLower() + "_rss.png";
            iWebFetcher = aWebFetcher;

            iMetadata = new container();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            iEpisodes = new List<PodcastEpisode>();

            iWebFile = iWebFetcher.Create(new Uri("http://wfmu.org/podcast/" + iId + ".xml"), iId + ".xml", 60);
            iWebFile.EventContentsChanged += WebFileContentsChanged;

            iWebFile.Open();
        }

        private string GetContents(XmlNode aNavigator, string aXpath, XmlNamespaceManager aNsManager)
        {
            XmlNode node = aNavigator.SelectSingleNode(aXpath, aNsManager);

            if (node == null)
            {
                return (null);
            }

            return (node.InnerXml);
        }

        PodcastEpisode FindEpisode(string aId)
        {
            lock (this)
            {
                foreach (PodcastEpisode episode in iEpisodes)
                {
                    if (episode.Id == aId)
                    {
                        return (episode);
                    }
                }

                return (null);
            }
        }

        private void WebFileContentsChanged(object obj, EventArgs e)
        {
            bool added = false;

            try
            {
                XmlDocument document = new XmlDocument();
                StringReader reader = new StringReader(iWebFile.Contents);
                document.Load(reader);

                XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(document.NameTable);
                xmlNsMan.AddNamespace("blogChannel", "http://backend.userland.com/blogChannelModule");
                xmlNsMan.AddNamespace("itunes", "http://www.itunes.com/dtds/podcast-1.0.dtd");

                /*
                    <title>Moyles: 06 Aug 10. A Message</title> 
                    <description>Unfortunately the team are away on their hols, so there are no highlights to bring you this week. However here's a quick message to explain (well, sort of)</description> 
                    <itunes:subtitle>Unfortunately the team are away on their hols, so there are no highlights to bring you this week. However here's a quick message to explain (well, sort of)...</itunes:subtitle> 
                    <itunes:summary>Unfortunately the team are away on their hols, so there are no highlights to bring you this week. However here's a quick message to explain (well, sort of)</itunes:summary> 
                    <pubDate>Fri, 06 Aug 2010 01:30:00 +0100</pubDate> 
                    <itunes:duration>4:07</itunes:duration> 
                    <enclosure url="http://downloads.bbc.co.uk/podcasts/radio1/moyles/moyles_20100806-0130a.mp3" length="2038299" type="audio/mpeg" /> 
                    <guid isPermaLink="false">http://downloads.bbc.co.uk/podcasts/radio1/moyles/moyles_20100806-0130.mp3</guid> 
                    <link>http://downloads.bbc.co.uk/podcasts/radio1/moyles/moyles_20100806-0130a.mp3</link> 
                    <media:content url="http://downloads.bbc.co.uk/podcasts/radio1/moyles/moyles_20100806-0130a.mp3" fileSize="2038299" type="audio/mpeg" medium="audio" expression="full" duration="247" /> 
                    <itunes:author>BBC Radio 1</itunes:author> 
                 */

                foreach (XmlNode episode in document.SelectNodes("rss/channel/item"))
                {
                    string title = GetContents(episode, "title", xmlNsMan);
                    string description = GetContents(episode, "description", xmlNsMan);
                    string guid = GetContents(episode, "guid", xmlNsMan);
                    string link = GetContents(episode, "enclosure/@url", xmlNsMan);
                    string date = GetContents(episode, "pubDate", xmlNsMan);
                    string duration = GetContents(episode, "itunes:duration", xmlNsMan);

                    if (title != null && description != null && guid != null && link != null)
                    {
                        PodcastEpisode x = FindEpisode(guid);

                        if (x == null)
                        {
                            x = new PodcastEpisode(guid, title, date, iAlbumArtUri, link, duration);

                            added = true;

                            lock (this)
                            {
                                iEpisodes.Add(x);
                                iMetadata.ChildCount = iEpisodes.Count;
                            }
                        }
                    }
                }
            }
            catch (XmlException)
            {
            }

            if (added)
            {
                if (EventContentAdded != null)
                {
                    EventContentAdded(this, EventArgs.Empty);
                }
            }
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        // IContainer

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public uint Open()
        {
            lock (this)
            {
                return ((uint)iEpisodes.Count);
            }
        }

        public void Close() { }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            lock (this)
            {
                int index = (int)aStartIndex;
                int count = (int)aCount;
                int required = index + count;
                int items = iEpisodes.Count;

                if (required > items)
                {
                    count -= required - items;
                }

                foreach (PodcastEpisode episode in iEpisodes.GetRange(index, count))
                {
                    didl.Add(episode.Metadata);
                }
            }

            return (didl);
        }

        public void Refresh()
        {
            iWebFile.Refresh();
        }

        public IContainer ChildContainer(container aContainer)
        {
            return (null);
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
            return (false);
        }

        public bool HandleInsert(DidlLite aDidlLite)
        {
            return (false);
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }

        public bool HandleDelete(DidlLite aDidlLite)
        {
            return (false);
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

        public void Dispose()
        {
            iWebFile.Dispose();
        }

        private string iId;
        private string iName;
        private string iAlbumArtUri;
        private container iMetadata;
        private WebFetcher iWebFetcher;
        private WebFile iWebFile;
        private List<PodcastEpisode> iEpisodes;
    }

    internal class PodcastStation : IContainer, IDisposable
    {
        internal PodcastStation(string aId, string aName, string aAlbumArtUri)
        {
            iId = aId;
            iName = aName;
            iAlbumArtUri = aAlbumArtUri;

            iMetadata = new container();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            iShows = new List<PodcastShow>();
        }

        public void Add(PodcastShow aShow)
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    if (show.Id == aShow.Id)
                    {
                        return;
                    }
                }

                iShows.Add(aShow);
            }

            if (EventContentAdded != null)
            {
                EventContentAdded(this, EventArgs.Empty);
            }
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        // IContainer

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public uint Open()
        {
            lock (this)
            {
                return ((uint)iShows.Count);
            }
        }

        public void Close() { }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            lock (this)
            {
                int index = (int)aStartIndex;
                int count = (int)aCount;
                int required = index + count;
                int items = iShows.Count;

                if (required > items)
                {
                    count -= required - items;
                }

                foreach (PodcastShow show in iShows.GetRange(index, count))
                {
                    didl.Add(show.Metadata);
                }
            }

            return didl;
        }

        public void Refresh()
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    show.Refresh();
                }
            }
        }

        public IContainer ChildContainer(container aContainer)
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    if (show.Id == aContainer.Id)
                    {
                        return (show);
                    }
                }

                return (null);
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
            return (false);
        }

        public bool HandleInsert(DidlLite aDidlLite)
        {
            return (false);
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
        }

        public bool HandleDelete(DidlLite aDidlLite)
        {
            return (false);
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

        public void Dispose()
        {
            lock (this)
            {
                foreach (PodcastShow show in iShows)
                {
                    show.Dispose();
                }
            }
        }

        private string iId;
        private string iName;
        private string iAlbumArtUri;
        private container iMetadata;
        private List<PodcastShow> iShows;
    }
}
