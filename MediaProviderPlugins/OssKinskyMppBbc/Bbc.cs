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

[assembly: ContentDirectoryFactoryType("OssKinskyMppBbc.ContentDirectoryFactoryBbc")]

namespace OssKinskyMppBbc
{
    public class ContentDirectoryFactoryBbc : IContentDirectoryFactory
    {
        public IContentDirectory Create(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            return (new ContentDirectoryBbc(aDataPath, aSupport));
        }
    }

    public class ContentDirectoryBbc : IContentDirectory, IContainer
    {
        IContentDirectorySupportV2 iSupport;

        private OptionPage iOptionPage;
        private OptionBool iOptionUk;

        private container iMetadata;

        private WebFetcher iWebFetcher;

        private Live iLive;
        private Listen iListen;
        private Podcasts iPodcasts;

        public ContentDirectoryBbc(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            iSupport = aSupport;
            iOptionPage = new OptionPage("BBC");

            iOptionUk = new OptionBool("ukoperation", "Uk Operation", "Set if operating within the UK", true);
            iOptionUk.EventValueChanged += UkValueChanged;

            iOptionPage.Add(iOptionUk);
            
            string installPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            iWebFetcher = new WebFetcher(aDataPath);

            string bbcLogo = aSupport.VirtualFileSystem.Uri(Path.Combine(installPath, "Bbc.png"));

            iMetadata = new container();
            iMetadata.ChildCount = 3;
            iMetadata.Id = "bbc";
            iMetadata.Title = "BBC";
            iMetadata.AlbumArtUri.Add(bbcLogo);

            iLive = new Live(bbcLogo);
            iListen = new Listen(iWebFetcher, bbcLogo);
            iPodcasts = new Podcasts(iWebFetcher, bbcLogo);
        }

        public void UkValueChanged(object obj, EventArgs e)
        {
            iLive.SetUk(iOptionUk.Native);
            iListen.SetUk(iOptionUk.Native);
        }

        public void Start()
        {
            iLive.Start();
            iListen.Start();
            iPodcasts.Start();
        }

        public void Stop()
        {
            iLive.Stop();
            iListen.Stop();
            iPodcasts.Stop();
        }

        public uint Open()
        {
            lock (this)
            {
                return (3);
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
                else if (index == 2)
                {
                    didl.Add(iListen.Metadata);
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
            iListen.Refresh();
            iPodcasts.Refresh();
        }

        public IContainer ChildContainer(container aContainer)
        {
            if (iLive.Metadata.Id == aContainer.Id)
            {
                return (iLive);
            }

            if (iListen.Metadata.Id == aContainer.Id)
            {
                return (iListen);
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
                return "BBC";
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
        private album iMetadata;
        private List<LiveStation> iStations;

        public Live(string aAlbumArtUri)
        {
            iStations = new List<LiveStation>();

            iStations.Add(new LiveStation("radio1", "BBC Radio 1", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_one.gif", "http://www.bbc.co.uk/radio/listen/live/r1.asx", 48, 128));
            iStations.Add(new LiveStation("1xtra", "BBC 1xtra", "http://bbc.co.uk/iplayer/img/radio/bbc_1xtra.gif", "http://www.bbc.co.uk/radio/listen/live/r1x.asx", 48, 128));
            iStations.Add(new LiveStation("radio2", "BBC Radio 2", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_two.gif", "http://www.bbc.co.uk/radio/listen/live/r2.asx", 48, 128));
            iStations.Add(new LiveStation("radio3", "BBC Radio 3", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_three.gif", "http://www.bbc.co.uk/radio/listen/live/r3.asx", 48, 192));
            iStations.Add(new LiveStation("radio4", "BBC Radio 4", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_four.gif", "http://www.bbc.co.uk/radio/listen/live/r4.asx", 48, 128));
            iStations.Add(new LiveStation("radio4extra", "BBC Radio 4 Extra", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_four.gif", "http://www.bbc.co.uk/radio/listen/live/r4x.asx", 48, 128));
            iStations.Add(new LiveStation("fivelive", "BBC Radio 5live", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_five_live.gif", "http://www.bbc.co.uk/radio/listen/live/r5l.asx", 48, 48));
            iStations.Add(new LiveStation("sportsextra", "BBC Radio 5live Sports Extra", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_five_live_sports_extra.gif", "http://www.bbc.co.uk/radio/listen/live/r5lsp.asx", 48, 48));
            iStations.Add(new LiveStation("6music", "BBC 6Music", "http://bbc.co.uk/iplayer/img/radio/bbc_6music.gif", "http://www.bbc.co.uk/radio/listen/live/r6.asx", 48, 128));
            iStations.Add(new LiveStation("asiannetwork", "BBC AsianNetwork", "http://bbc.co.uk/iplayer/img/radio/bbc_asian_network.gif", "http://www.bbc.co.uk/radio/listen/live/ran.asx", 48, 128));

            iStations.Add(new LiveStation("alba", "BBC Radio nan Gaidheal", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_nan_gaidheal.gif", "http://www.bbc.co.uk/radio/listen/live/rng.asx", 48, 80));
            iStations.Add(new LiveStation("radioscotland", "BBC Radio Scotland", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_scotland_1.gif", "http://www.bbc.co.uk/radio/listen/live/rs.asx", 48, 80));
            iStations.Add(new LiveStation("radioulster", "BBC Radio Ulster", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_ulster.gif", "http://www.bbc.co.uk/radio/listen/live/ru.asx", 48, 80));
            iStations.Add(new LiveStation("radiofoyle", "BBC Radio Foyle", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_ulster.gif", "http://www.bbc.co.uk/radio/listen/live/rf.asx", 48, 80));
            iStations.Add(new LiveStation("radiowales", "BBC Radio Wales", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_wales.gif", "http://www.bbc.co.uk/radio/listen/live/rw.asx", 48, 80));
            iStations.Add(new LiveStation("radiocymru", "BBC Radio Cymru", "http://bbc.co.uk/iplayer/img/radio/bbc_radio_cymru.gif", "http://www.bbc.co.uk/radio/listen/live/rc.asx", 48, 80));

            iStations.Add(new LiveStation("worldservice", "BBC World Service", "http://bbc.co.uk/iplayer/img/radio/bbc_world_service.gif", "http://www.bbc.co.uk/worldservice/meta/tx/nb/live_infent_au_nb.asx", 32, 32));
            iStations.Add(new LiveStation("worldservice-news", "BBC World Service - English News", "http://bbc.co.uk/iplayer/img/radio/bbc_world_service.gif", "http://www.bbc.co.uk/worldservice/meta/tx/nb/live_news_au_nb.asx", 32, 32));
            iStations.Add(new LiveStation("worldservice-arabic", "BBC Arabic", "http://bbc.co.uk/iplayer/img/radio/bbc_world_service.gif", "http://www.bbc.co.uk/arabic/meta/tx/nb/arabic_live_au_nb.asx", 32, 32));
            iStations.Add(new LiveStation("worldservice-russian", "BBC Russian", "http://bbc.co.uk/iplayer/img/radio/bbc_world_service.gif", "http://www.bbc.co.uk/russian/meta/tx/nb/russian_live_au_nb.asx", 32, 32));

            iStations.Add(new LiveStation("worldservice", "BBC Berkshire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcberkshire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Bristol", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcbristol.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Cambridgeshire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbccambridgeshire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Cornwall", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbccornwall.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Coventry & Warwickshire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbccoventryandwarwickshire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Cumbria", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbccumbria.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Derby", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcderby.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Devon", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcdevon.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Essex", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcessex.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Gloucestershire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcgloucestershire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Guernsey", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcguernsey.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Hereford & Worcester", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcherefordandworcester.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Humberside", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbchumberside.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Jersey", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcjersey.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Kent", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbckent.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Lancashire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbclancashire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Leeds", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcleeds.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Leicester", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcleicester.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Lincolnshire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbclincolnshire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC London", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbclondon.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Manchester", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcmanchester.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Merseyside", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcmerseyside.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Newcastle", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcnewcastle.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Norfolk", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcnorfolk.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Northampton", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcnorthampton.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Nottingham", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcnottingham.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Oxford", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcoxford.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Sheffield", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcsheffield.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Shropshire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcshropshire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Solent", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcsolent.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Somerset", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcsomerset.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Stoke", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcstoke.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Suffolk", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcsuffolk.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Surrey", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcsurrey.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Sussex", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcsussex.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Tees", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbctees.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Three Counties", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcthreecounties.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC Wiltshire", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcwiltshire.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC WM", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcwm.asx", 48, 48));
            iStations.Add(new LiveStation("worldservice", "BBC York", aAlbumArtUri, "http://bbc.co.uk/radio/listen/live/bbcyork.asx", 48, 48));

            iMetadata = new album();
            iMetadata.ChildCount = iStations.Count;
            iMetadata.Id = "live";
            iMetadata.Title = "Live Radio";
            iMetadata.AlbumArtUri.Add(aAlbumArtUri);
        }

        public void SetUk(bool aValue)
        {
            foreach (LiveStation station in iStations)
            {
                station.SetUk(aValue);
            }

            if (EventContentUpdated != null)
            {
                EventContentUpdated(this, EventArgs.Empty);
            }
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
        internal LiveStation(string aId, string aName, string aAlbumArtUri, string aAudioUri, int aLoKbps, int aHiKbps)
        {
            iId = aId;
            iName = aName;
            iAlbumArtUri = aAlbumArtUri;
            iAudioUri = aAudioUri;
            iLoKbps = aLoKbps * 125;
            iHiKbps = aHiKbps * 125;

            iUk = true;

            UpdateMetadata();
        }

        private void UpdateMetadata()
        {
            iMetadata = new audioItem();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            resource res = new resource();
            res.Uri = iAudioUri;
            res.NrAudioChannels = 2;
            res.Bitrate = iUk ? iHiKbps : iLoKbps;
            res.ProtocolInfo = "http-get:*:audio/x-ms-wma:*";

            iMetadata.Res.Add(res);
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        public void SetUk(bool aValue)
        {
            iUk = aValue;
            UpdateMetadata();
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
        private string iAlbumArtUri;
        private string iAudioUri;
        private int iLoKbps;
        private int iHiKbps;
        private bool iUk;
        private audioItem iMetadata;
    }

    internal class Podcasts : IContainer, IDisposable
    {
        private WebFetcher iWebFetcher;
        private string iAlbumArtUri;

        private WebFile iWebFile;

        private List<PodcastStation> iStations;
        private string iInstallPath;
        private container iMetadata;

        private void WebFileContentsChanged(object obj, EventArgs e)
        {
            bool added = false;

            try
            {
                XmlDocument document = new XmlDocument();
                StringReader reader = new StringReader(iWebFile.Contents);
                document.Load(reader);

                foreach (XmlNode station in document.SelectNodes("opml/body/outline/outline"))
                {
                    string fullname = GetContents(station, "@fullname");
                    string networkid = GetContents(station, "@text");

                    if (fullname != null && networkid != null)
                    {
                        PodcastStation s = FindStation(networkid);

                        if (s == null)
                        {
                            s = new PodcastStation(networkid, fullname, FindStationAlbumArtUri(networkid));

                            added = true;

                            lock (this)
                            {
                                iStations.Add(s);
                                iMetadata.ChildCount = iStations.Count;
                            }
                        }

                        foreach (XmlNode show in station.SelectNodes("outline"))
                        {
                            // <outline
                            //    type="rss"
                            //    imageHref="http://www.bbc.co.uk/podcasts/assets/artwork/moyles.jpg"
                            //    xmlUrl="http://downloads.bbc.co.uk/podcasts/radio1/moyles/rss.xml"
                            //    imageHrefTVSafe=""
                            //    text="Best of Chris Moyles"
                            //    keyname="moyles"
                            //    active="true"
                            //    allow="all"
                            //    networkName=""
                            //    networkId=""
                            //    typicalDurationMins="36"
                            //    page="http://www.bbc.co.uk/radio1/chrismoyles/"
                            //    flavour="Programme Highlights"
                            //    rsstype=""
                            //    rssenc=""
                            //    language="en-gb"
                            //    description="Weekly highlights from the award-winning Chris Moyles breakfast show, as broadcast by Chris and team every morning from 6.30am to 10am."
                            //    bbcgenres="Entertainment"
                            //  />

                            string imageHref = GetContents(show, "@imageHref");
                            string xmlUrl = GetContents(show, "@xmlUrl");
                            string text = GetContents(show, "@text");
                            string keyname = GetContents(show, "@keyname");
                            string description = GetContents(show, "@description");

                            if (imageHref != null && xmlUrl != null && text != null && keyname != null && description != null)
                            {
                                if (s.Find(keyname) == null)
                                {
                                    WebFile webFile = iWebFetcher.Create(new Uri(xmlUrl), keyname + ".xml", 60);
                                    s.Add(new PodcastShow(keyname, text, description, imageHref, webFile));
                                }
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

        string FindStationAlbumArtUri(string aId)
        {
            switch (aId)
            {
                case "radio1":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_one.gif");
                case "1xtra":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_1xtra.gif");
                case "radio2":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_two.gif");
                case "radio3":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_three.gif");
                case "radio4":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_four.gif");
                case "radio4extra":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_four.gif");
                case "5live":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_five_live.gif");
                case "5livesportsextra":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_five_live_sports_extra.gif");
                case "6music":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_6music.gif");
                case "asiannetwork":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_asian_network.gif");
                case "worldservice":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_world_service.gif");
                case "alba":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_nan_gaidheal.gif");
                case "scotland":
                case "radioscotland":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_scotland_1.gif");
                case "northernireland":
                case "radioulster":
                case "radiofoyle":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_ulster.gif");
                case "wales":
                case "radiowales":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_wales.gif");
                case "cymru":
                case "radiocymru":
                    return ("http://bbc.co.uk/iplayer/img/radio/bbc_radio_cymru.gif");
                default:
                    return (iAlbumArtUri);
            }
        }

        PodcastStation FindStation(string aId)
        {
            lock (this)
            {
                foreach (PodcastStation station in iStations)
                {
                    if (station.Id == aId)
                    {
                        return (station);
                    }
                }

                return (null);
            }
        }

        string GetContents(XmlNode aNode, string aXpath)
        {
            XmlNode node = aNode.SelectSingleNode(aXpath);

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

            iStations = new List<PodcastStation>();

            iWebFile = iWebFetcher.Create(new Uri("http://www.bbc.co.uk/podcasts.opml"), "opml.xml", 60);
            iWebFile.EventContentsChanged += WebFileContentsChanged;
        }

        public void Start()
        {
        }

        public void Stop()
        {
            lock (this)
            {
                foreach (PodcastStation station in iStations)
                {
                    station.Dispose();
                }
            }
        }

        public uint Open()
        {
            lock (this)
            {
                iWebFile.Open();

                return ((uint)iStations.Count);
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
                int items = iStations.Count;

                if (required > items)
                {
                    count -= required - items;
                }

                foreach (PodcastStation station in iStations.GetRange(index, count))
                {
                    didl.Add(station.Metadata);
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
                foreach (PodcastStation station in iStations)
                {
                    if (station.Id == aContainer.Id)
                    {
                        return (station);
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
        internal PodcastEpisode(string aId, string aName, string aDescription, string aAlbumArtUri, string aAudioUri, string aDuration)
        {
            iId = aId;
            iName = aName;
            iDescription = aDescription;
            iAlbumArtUri = aAlbumArtUri;
            iAudioUri = aAudioUri;

            iMetadata = new audioItem();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            resource res = new resource();
            res.Uri = iAudioUri;
            res.ProtocolInfo = "http-get:*:audio/x-mpeg:*";
            res.Duration = new Time(aDuration).ToString();

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
        private string iDescription;
        private string iAlbumArtUri;
        private string iAudioUri;
        private audioItem iMetadata;
    }

    internal class PodcastShow : IContainer, IDisposable
    {
        internal PodcastShow(string aId, string aName, string aDescription, string aAlbumArtUri, WebFile aWebFile)
        {
            iId = aId;
            iName = aName;
            iDescription = aDescription;
            iAlbumArtUri = aAlbumArtUri;
            iWebFile = aWebFile;

            iMetadata = new container();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            iEpisodes = new List<PodcastEpisode>();

            iWebFile.EventContentsChanged += WebFileContentsChanged;
        }

        private string GetContents(XmlNode aNavigator, string aXpath, XmlNamespaceManager aNsManager)
        {
            XmlNode node = aNavigator.SelectSingleNode(aXpath, aNsManager);

            if (node == null)
            {
                return (null);
            }

            return (node.FirstChild.Value);
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
                xmlNsMan.AddNamespace("media", "http://search.yahoo.com/mrss/");
                xmlNsMan.AddNamespace("itunes", "http://www.itunes.com/dtds/podcast-1.0.dtd");
                xmlNsMan.AddNamespace("ppg", "http://bbc.co.uk/2009/01/ppgRss");
                xmlNsMan.AddNamespace("atom", "http://www.w3.org/2005/Atom");

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
                    string link = GetContents(episode, "link", xmlNsMan);
                    string duration = GetContents(episode, "itunes:duration", xmlNsMan);

                    if (title != null && description != null && guid != null && link != null)
                    {
                        PodcastEpisode x = FindEpisode(guid);

                        if (x == null)
                        {
                            x = new PodcastEpisode(guid, title, description, iAlbumArtUri, link, duration);

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
                iWebFile.Open();

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
        private string iDescription;
        private string iAlbumArtUri;
        private container iMetadata;
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

        public PodcastShow Find(string aId)
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

        public void Add(PodcastShow aShow)
        {
            lock (this)
            {
                iShows.Add(aShow);

                iMetadata.ChildCount = iShows.Count;
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
                int index = (int) aStartIndex;
                int count = (int) aCount;
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

    public class Listen : IContainer, IDisposable
    {
        private WebFetcher iWebFetcher;
        private string iAlbumArtUri;

        private List<ListenStation> iStations;
        private container iMetadata;

        public Listen(WebFetcher aWebFetcher, string aAlbumArtUri)
        {
            iWebFetcher = aWebFetcher;
            iAlbumArtUri = aAlbumArtUri;

            iStations = new List<ListenStation>();

            AddStation("radio1", "BBC Radio 1", "bbc_radio_one.gif");
            AddStation("1xtra", "BBC 1xtra", "bbc_1xtra.gif");
            AddStation("radio2", "BBC Radio 2", "bbc_radio_two.gif");
            AddStation("radio3", "BBC Radio 3", "bbc_radio_three.gif");
            AddStation("radio4", "BBC Radio 4", "bbc_radio_four.gif");
            AddStation("radio4extra", "BBC Radio 4 Extra", "bbc_radio_four.gif");
            AddStation("fivelive", "BBC Radio 5live", "bbc_radio_five_live.gif");
            AddStation("sportsextra", "BBC Radio 5live Sports Extra", "bbc_radio_five_live_sports_extra.gif");
            AddStation("6music", "BBC 6Music", "bbc_6music.gif");
            AddStation("asiannetwork", "BBC AsianNetwork", "bbc_asian_network.gif");

            /* Unable to infer audio uri from xml file for these stations
             * 
            AddStation("worldservice", "BBC World Service", "bbc_world_service.gif");
            AddStation("alba", "BBC Radio nan Gaidheal", "bbc_radio_nan_gaidheal.gif");
            AddStation("radioscotland", "BBC Radio Scotland", "bbc_radio_scotland_1.gif");
            AddStation("radioulster", "BBC Radio Ulster", "bbc_radio_ulster.gif");
            AddStation("radiofoyle", "BBC Radio Foyle", "bbc_radio_ulster.gif");
            AddStation("radiowales", "BBC Radio Wales", "bbc_radio_wales.gif");
            AddStation("radiocymru", "BBC Radio Cymru", "bbc_radio_cymru.gif");
             * 
            */

            iMetadata = new container();
            iMetadata.ChildCount = iStations.Count;
            iMetadata.Id = "listen";
            iMetadata.Title = "Listen Again (UK)";
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);
        }

        public void SetUk(bool aValue)
        {
            lock (this)
            {
                foreach (ListenStation station in iStations)
                {
                    station.SetUk(aValue);
                }
            }
        }

        private void AddStation(string aId, string aName, string aArtworkFile)
        {
            string artworkUri = "http://bbc.co.uk/iplayer/img/radio/" + aArtworkFile;

            string xmlPath = aId + ".xml";

            Uri xmlUri = new Uri("http://bbc.co.uk/radio/aod/availability/" + xmlPath);

            WebFile webFile = iWebFetcher.Create(xmlUri, xmlPath, 30);

            ListenStation station = new ListenStation(aId, aName, webFile, artworkUri);

            iStations.Add(station);
        }

        public void Start()
        {
            lock (this)
            {
                foreach (ListenStation station in iStations)
                {
                    station.Refresh();
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                foreach (ListenStation station in iStations)
                {
                    station.Dispose();
                }
            }
        }

        public uint Open()
        {
            lock (this)
            {
                return ((uint)iStations.Count);
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
                int items = iStations.Count;

                if (required > items)
                {
                    count -= required - items;
                }

                foreach (ListenStation station in iStations.GetRange(index, count))
                {
                    didl.Add(station.Metadata);
                }
            }
            return (didl);
        }

        public void Refresh()
        {
            lock (this)
            {
                foreach (ListenStation station in iStations)
                {
                    station.Refresh();
                }
            }
        }

        public IContainer ChildContainer(container aContainer)
        {
            lock (this)
            {
                foreach (ListenStation station in iStations)
                {
                    if (station.Id == aContainer.Id)
                    {
                        return (station);
                    }
                }
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

        public void Dispose()
        {
            lock (this)
            {
                foreach (ListenStation station in iStations)
                {
                    station.Dispose();
                }
            }
        }
    }
    
    internal class ListenShow : IComparable
    {
        private string iId;
        private string iName;
        private string iDescription;
        private DateTime iStartTime;
        private uint iDuration;
        private string iProgramId;
        private string iAlbumArtUri;
        private bool iUk;
        private OptionBool iOption;
        private audioItem iMetadata;
        
        internal ListenShow(string aId, string aName, string aDescription, DateTime aStartTime, uint aDuration, string aProgramId)
        {
            iId = aId;
            iName = aName;
            iDescription = aDescription;
            iStartTime = aStartTime;
            iDuration = aDuration;
            iProgramId = aProgramId;
            iAlbumArtUri = "http://node1.bbcimg.co.uk/iplayer/images/episode/" + iId + "_640_360.jpg";

            UpdateMetadata(true);
        }

        private void UpdateMetadata(bool aUk)
        {
            string audioUri;

            if (aUk)
            {
                audioUri = "http://www.bbc.co.uk/mediaselector/4/asx/" + iProgramId;
            }
            else
            {
                audioUri = "http://www.bbc.co.uk/mediaselector/4/asx/" + iProgramId;
            }

            iMetadata = new audioItem();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.Description = iDescription;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            resource res = new resource();
            res.Uri = audioUri;
            res.Duration = new Time((int)iDuration).ToString();
            res.ProtocolInfo = "http-get:*:audio/x-ms-wma:*";

            iMetadata.Res.Add(res);
        }

        public void SetUk(bool aValue)
        {
            UpdateMetadata(aValue);
        }

        public Int32 CompareTo(object aObject)
        {
            ListenShow show = aObject as ListenShow;
            Int32 res = iName.CompareTo(show.iName);
            if (res == 0)
            {
                res = iStartTime.CompareTo(show.iStartTime);
            }
            return (res);
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        public upnpObject UpnpObject
        {
            get
            {
                return iMetadata;
            }
        }
    }
    
    internal class ListenStation : IContainer, IDisposable
    {
        private string iId;
        private string iName;
        private WebFile iWebFile;
        private string iAlbumArtUri;
        private container iMetadata;
        private List<ListenShow> iShows;
        
        internal ListenStation(string aId, string aName, WebFile aWebFile, string aAlbumArtUri)
        {
            iId = aId;
            iName = aName;
            iWebFile = aWebFile;
            iAlbumArtUri = aAlbumArtUri;

            iMetadata = new container();
            iMetadata.Id = iId;
            iMetadata.Title = iName;
            iMetadata.AlbumArtUri.Add(iAlbumArtUri);

            iShows = new List<ListenShow>();

            iWebFile.EventContentsChanged += WebFileContentsChanged;
        }

        public void SetUk(bool aValue)
        {
            lock (this)
            {
                foreach (ListenShow show in iShows)
                {
                    show.SetUk(aValue);
                }
            }

            if (EventContentUpdated != null)
            {
                EventContentUpdated(this, EventArgs.Empty);
            }
        }

        private void WebFileContentsChanged(object obj, EventArgs e)
        {
            ProcessXml(iWebFile.Contents);
        }

        private void ProcessXml(string aXml)
        {
            Nullable<DateTime> timestamp = CollectTimestamp(aXml);

            if (timestamp != null)
            {
                IList<ListenShow> list = CollectShows(aXml);

                bool added = false;

                lock (this)
                {
                    foreach (ListenShow show in list)
                    {
                        bool found = false;

                        foreach (ListenShow current in iShows)
                        {
                            if (show.Id == current.Id)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            iShows.Add(show);
                            iMetadata.ChildCount = iShows.Count;
                            added = true;
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

        private IList<ListenShow> CollectShows(string aXml)
        {
            List<ListenShow> list = new List<ListenShow>();

            try
            {
                StringReader sr = new StringReader(aXml);
                XmlReader xr = XmlReader.Create(sr);

                xr.MoveToContent();

                if (xr.Name != "schedule")
                {
                    throw (new XmlException());
                }

                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.Name == "entry")
                    {
                        string pid = null;
                        string title = null;
                        string synopsis = null;
                        string version = null;
                        DateTime start = DateTime.Now;
                        bool availability = false;
                        DateTime availabilityStart = DateTime.Now;
                        DateTime availabilityEnd = DateTime.Now;

                        uint duration = 0;

                        while (xr.Read())
                        {
                            if (xr.NodeType == XmlNodeType.Element)
                            {
                                if (xr.Name == "pid")
                                {
                                    if (pid != null)
                                    {
                                        throw (new XmlException());
                                    }

                                    pid = xr.ReadElementContentAsString();
                                }
                                else if (xr.Name == "title")
                                {
                                    if (title != null)
                                    {
                                        throw (new XmlException());
                                    }

                                    title = xr.ReadElementContentAsString();
                                }
                                else if (xr.Name == "synopsis")
                                {
                                    if (synopsis != null)
                                    {
                                        throw (new XmlException());
                                    }

                                    synopsis = xr.ReadElementContentAsString();
                                }
                                else if (xr.Name == "availability")
                                {
                                    if (availability)
                                    {
                                        throw (new XmlException());
                                    }

                                    if (!xr.MoveToAttribute("start"))
                                    {
                                        throw (new XmlException());
                                    }

                                    availabilityStart = xr.ReadContentAsDateTime();

                                    if (!xr.MoveToAttribute("end"))
                                    {
                                        throw (new XmlException());
                                    }

                                    availabilityEnd = xr.ReadContentAsDateTime();

                                    availability = true;
                                }
                                else if (xr.Name == "broadcast")
                                {
                                    if (version != null)
                                    {
                                        throw (new XmlException());
                                    }

                                    if (!xr.MoveToAttribute("version_pid"))
                                    {
                                        throw (new XmlException());
                                    }

                                    version = xr.ReadContentAsString();

                                    if (!xr.MoveToAttribute("start"))
                                    {
                                        throw (new XmlException());
                                    }

                                    start = xr.ReadContentAsDateTime();

                                    if (!xr.MoveToAttribute("duration"))
                                    {
                                        throw (new XmlException());
                                    }

                                    duration = (uint) xr.ReadContentAsInt();
                                }
                            }
                            else if (xr.NodeType == XmlNodeType.EndElement)
                            {
                                if (xr.Name == "entry")
                                {
                                    if (pid != null && title != null && synopsis != null && version != null && availability)
                                    {
                                        DateTime now = DateTime.Now;

                                        if (availabilityStart < now && now < availabilityEnd)
                                        {
                                            // Check accessible

                                            try
                                            {
                                                
                                                // WebRequest request = HttpWebRequest.Create(audioUri);
                                                // request.Method = "HEAD";
                                                // Stream stream = request.GetResponse().GetResponseStream();
                                                // StreamReader reader = new StreamReader(stream);
                                                // string xml = reader.ReadToEnd();
                                                // reader.Close();
                                                

                                                bool found = false;

                                                foreach (ListenShow show in list)
                                                {
                                                    if (show.Id == pid)
                                                    {
                                                        found = true;
                                                        break;
                                                    }
                                                }

                                                if (!found)
                                                {
                                                    list.Add(new ListenShow(pid, title, synopsis, start, duration, version));
                                                }
                                            }
                                            catch (WebException)
                                            {
                                            }
                                        }

                                        break;
                                    }
                                    throw (new XmlException());
                                }
                            }
                        }

                    }
                }
            }
            catch (XmlException)
            {
            }

            list.Sort();

            return (list);
        }

        private Nullable<DateTime> CollectTimestamp(string aXml)
        {
            try
            {
                StringReader sr = new StringReader(aXml);
                XmlReader xr = XmlReader.Create(sr);

                xr.MoveToContent();

                if (xr.Name != "schedule")
                {
                    throw (new XmlException());
                }

                if (!xr.MoveToAttribute("updated"))
                {
                    throw (new XmlException());
                }

                return (xr.ReadContentAsDateTime());
            }
            catch (XmlException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch (FormatException)
            {
            }

            return (null);
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
                iWebFile.Open();
                
                return ((uint)iShows.Count);
            }
        }

        public void Close() { }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            lock (this)
            {
                for(int i=(int)aStartIndex;i<aStartIndex + aCount;i++)
                {
                    didl.Add(iShows[i].UpnpObject);
                }
            }

            return didl;
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

        public void Dispose()
        {
            iWebFile.Dispose();
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
    }
}
