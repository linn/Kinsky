using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Linn;
using Linn.Kinsky;

using Upnp;

namespace OssKinskyMppMovieTrailers
{
    internal interface IVideo
    {
        string Uri { get; }
        uint Size { get; }
    }

    internal interface IPoster
    {
        string UriSmall { get; }
        string UriLarge { get; }
    }

    internal interface ITrailer
    {
        string Id { get; }
        videoItem Metadata { get; }
    }

    internal class Video : IVideo
    {
        public Video(string aUri, uint aSize)
        {
            iUri = aUri;
            iSize = aSize;
        }

        public string Uri
        {
            get
            {
                return iUri;
            }
        }

        public uint Size
        {
            get
            {
                return iSize;
            }
        }

        private string iUri;
        private uint iSize;
    }

    internal class Poster : IPoster
    {
        public Poster(string aUriSmall, string aUriLarge)
        {
            iUriSmall = aUriSmall;
            iUriLarge = aUriLarge;
        }

        public string UriSmall
        {
            get
            {
                return iUriSmall;
            }
        }

        public string UriLarge
        {
            get
            {
                return iUriLarge;
            }
        }

        private string iUriSmall;
        private string iUriLarge;
    }

    internal class Trailer : ITrailer
    {
        public Trailer(string aId, string aTitle, string aRunTime, string aRating, string aStudio, string aDirector, string aDescription,
            IList<string> aCast, IList<string> aGenre, IPoster aPoster, IVideo aVideo)
        {
            iId = aId;

            iMetadata = new videoItem();
            iMetadata.Id = aId;
            iMetadata.Title = aTitle;
            iMetadata.Genre.AddRange(aGenre);

            string[] directors = aDirector.Split(',');
            foreach (string s in directors)
            {
                iMetadata.Director.Add(s.Trim());
            }

            foreach (string s in aCast)
            {
                actor actor = new actor();
                actor.Actor = s;
                iMetadata.Actor.Add(actor);
            }

            iMetadata.LongDescription = aDescription;
            iMetadata.Publisher = aStudio;
            iMetadata.Rating = aRating;
            
            if (!string.IsNullOrEmpty(aPoster.UriSmall))
            {
                iMetadata.AlbumArtUri.Add(aPoster.UriSmall);
            }

            resource res = new resource();
            try
            {
                res.Duration = (new Time(string.Format("0:{0}", aRunTime))).ToString();
            }
            catch (Time.TimeInvalid)
            {
                res.Duration = (new Time(0)).ToString();
            }
            res.Size = aVideo.Size;
            res.ProtocolInfo = "http-get:*:video/x-m4v:*";
            res.Uri = aVideo.Uri;

            iMetadata.Res.Add(res);
        }

        public string Id
        {
            get
            {
                return iId;
            }
        }

        public videoItem Metadata
        {
            get
            {
                return iMetadata;
            }
        }

        private string iId;
        private videoItem iMetadata;
    }

    internal class Podcast : IContainer, IDisposable
    {
        public Podcast(WebFetcher aWebFetcher, string aInstallPath, IContentDirectorySupportV2 aSupport)
        {
            iTrailers = new List<Trailer>();

            iWebFetcher = aWebFetcher;

            iWebFile = iWebFetcher.Create(new Uri(kAppleTrailersHiResUri), "podcasts.xml", 60);
            iWebFile.EventContentsChanged += WebFileContentsChanged;
            iWebFile.Open();

            string movieTrailerLogo = aSupport.VirtualFileSystem.Uri(Path.Combine(aInstallPath, "MovieTrailers.png"));

            iMetadata = new container();
            iMetadata.Id = "movietrailers";
            iMetadata.Title = "Movie Trailers";
            iMetadata.AlbumArtUri.Add(movieTrailerLogo);
            iMetadata.ChildCount = 0;
        }

        public void Dispose()
        {
            iWebFile.Dispose();
        }

        public uint Open()
        {
            lock (this)
            {
                return (uint)iTrailers.Count;
            }
        }

        public void Close() { }

        public void Refresh()
        {
            iWebFile.Refresh();
        }

        public container Metadata
        {
            get
            {
                return iMetadata;
            }
        }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            int index = (int)aStartIndex;
            int count = (int)aCount;
            int required = index + count;
            int items = iTrailers.Count;

            if (required > items)
            {
                count -= required - items;
            }

            DidlLite didl = new DidlLite();

            lock (this)
            {
                foreach (Trailer trailer in iTrailers.GetRange(index, count))
                {
                    didl.Add(trailer.Metadata);
                }
            }

            return (didl);
        }

        public IContainer ChildContainer(container aContainer)
        {
            return (null);
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

        private void WebFileContentsChanged(object obj, EventArgs e)
        {
            bool added = false;

            try
            {
                XmlDocument document = new XmlDocument();
                StringReader reader = new StringReader(iWebFile.Contents);
                document.Load(reader);

                List<Trailer> trailers = new List<Trailer>();

                XmlNodeList list = document.SelectNodes("/records/movieinfo");
                foreach (XmlNode n in list)
                {
                    try
                    {
                        string id = n.SelectSingleNode("@id").InnerText;
                        string title = n.SelectSingleNode("info/title").InnerText;
                        string runtime = n.SelectSingleNode("info/runtime").InnerText;
                        string rating = n.SelectSingleNode("info/rating").InnerText;
                        string studio = n.SelectSingleNode("info/studio").InnerText;
                        //string postdate;
                        //string releaseddate;
                        //string copyright;
                        string director = n.SelectSingleNode("info/director").InnerText;
                        string description = n.SelectSingleNode("info/description").InnerText;

                        List<string> cast = new List<string>();
                        XmlNodeList c = n.SelectNodes("cast/name");
                        foreach (XmlNode a in c)
                        {
                            string name = a.InnerText;

                            if (name != null)
                            {
                                cast.Add(name);
                            }
                        }

                        List<string> genre = new List<string>();
                        XmlNodeList g = n.SelectNodes("genre/name");
                        foreach (XmlNode a in g)
                        {
                            string name = a.InnerText;

                            if (name != null)
                            {
                                genre.Add(name);
                            }
                        }

                        string uriSmall = n.SelectSingleNode("poster/location").InnerText;
                        string uriLarge = n.SelectSingleNode("poster/xlarge").InnerText;
                        Poster poster = new Poster(uriSmall, uriLarge);

                        uint size = 0;
                        try
                        {
                            size = uint.Parse(n.SelectSingleNode("preview/large/@filesize").InnerText);
                        }
                        catch (FormatException) { }
                        string uri = n.SelectSingleNode("preview/large").InnerText;

                        Video video = new Video(uri, size);

                        trailers.Add(new Trailer(id, title, runtime, rating, studio, director, description, cast, genre, poster, video));

                        added = true;
                    }
                    catch (NullReferenceException) { }
                }

                lock (this)
                {
                    iTrailers = trailers;
                }
            }
            catch (XmlException)
            {
            }

            if (added)
            {
                iMetadata.ChildCount = iTrailers.Count;

                if (EventContentAdded != null)
                {
                    EventContentAdded(this, EventArgs.Empty);
                }
            }
        }

        private const string kAppleTrailersLowResUri = "http://www.apple.com/trailers/home/xml/current.xml";
        private const string kAppleTrailersHiResUri = "http://www.apple.com/trailers/home/xml/current_720p.xml";

        private container iMetadata;

        private WebFetcher iWebFetcher;
        private WebFile iWebFile;

        private List<Trailer> iTrailers;
    }
}
