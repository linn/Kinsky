using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using Upnp;
using System.Text;
using System.Collections.ObjectModel;

namespace Linn.Kinsky
{

    public class BookmarkManager
    {

        public BookmarkManager(string aFilename)
        {
            iLock = new object();

            iFilename = aFilename;
            iBookmarks = new List<Bookmark>();

            Load();
        }

        public Bookmark Find(string aTitle)
        {
            lock (iLock)
            {
                foreach (Bookmark bmk in iBookmarks)
                {
                    if (bmk.Title.Equals(aTitle))
                    {
                        return bmk;
                    }
                }
                return null;
            }
        }

        public IEnumerable<Bookmark> Bookmarks
        {
            get
            {
                lock (iLock)
                {
                    return iBookmarks.ToArray();
                }
            }
        }

        public int IndexOf(Bookmark aBookmark)
        {
            lock (iLock)
            {
                return iBookmarks.IndexOf(aBookmark);
            }
        }

        public void Insert(int aIndex, Bookmark aBookmark)
        {
            lock (iLock)
            {
                iBookmarks.Insert(aIndex, aBookmark);
                Save();
            }
            OnBookmarkAdded(aBookmark);
        }

        public void Add(Bookmark aBookmark)
        {
            lock (iLock)
            {
                Assert.Check(!iBookmarks.Contains(aBookmark));
                iBookmarks.Add(aBookmark);
                Save();
            }
            OnBookmarkAdded(aBookmark);
        }

        public void Remove(Bookmark aBookmark)
        {
            lock (iLock)
            {
                if (iBookmarks.Contains(aBookmark))
                {
                    iBookmarks.Remove(aBookmark);
                    Save();
                }
            }
            OnBookmarkRemoved(aBookmark);
        }


        public void Move(int aIndex, List<Bookmark> aBookmarks)
        {
            lock (iLock)
            {
                int insertIndex = aIndex;
                foreach (Bookmark b in aBookmarks)
                {
                    if (iBookmarks.IndexOf(b) < insertIndex)
                    {
                        insertIndex--;
                    }
                    iBookmarks.Remove(b);
                }
                foreach (Bookmark b in aBookmarks)
                {
                    iBookmarks.Insert(insertIndex++, b);
                }
                Save();
            }
            OnBookmarksChanged();

        }

        // XML schema is as follows:
        // <Bookmarks version="...">
        //   <Bookmark title="..." image="...">
        //      <BreadcrumbTrail>
        //          <Breadcrumb id="..." title="...">
        //          ...
        //      </BreadcrumbTrail>
        //   </Bookmark>
        // </Bookmarks>
        public void Save()
        {
            lock (iLock)
            {
                XmlDocument document = new XmlDocument();
                XmlElement rootNode = document.CreateElement("Bookmarks");
                XmlAttribute versionAttribute = document.CreateAttribute("version");
                versionAttribute.Value = kFileVersion;
                rootNode.Attributes.Append(versionAttribute);
                document.AppendChild(rootNode);

                foreach (Bookmark bookmark in iBookmarks)
                {
                    // <Bookmark>
                    XmlElement bookmarkElement = bookmark.Save(document);
                    rootNode.AppendChild(bookmarkElement);
                }
                try
                {
                    document.Save(iFilename);
                }
                catch (Exception e)
                {
                    UserLog.WriteLine(String.Format("Failed to save options file {0}: {1}", iFilename, e.ToString()));
                }
            }
        }

        private void Load()
        {
            lock (iLock)
            {
                Trace.WriteLine(Trace.kKinsky, "Loading settings from " + iFilename);
                if (File.Exists(iFilename))
                {
                    LoadBookmarks(iFilename);
                }
                else
                {
                    UserLog.WriteLine("Could not find bookmarks file " + iFilename);
                    Trace.WriteLine(Trace.kKinsky, "Could not find bookmarks file " + iFilename);
                    MigrateOldBookmarks();
                }
            }
        }

        private void LoadBookmarks(string aFilename)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(aFilename);
                List<Bookmark> newBookmarks = new List<Bookmark>();
                XmlNodeList bookmarkNodeList = document.SelectNodes("Bookmarks/Bookmark");
                foreach (XmlNode bookmarkNode in bookmarkNodeList)
                {
                    Bookmark bookmark = new Bookmark();
                    bookmark.Load(bookmarkNode);
                    newBookmarks.Add(bookmark);
                }
                iBookmarks = newBookmarks;
            }
            catch (Exception e)
            {
                UserLog.WriteLine("Bookmarks file corrupt: " + e.ToString());
                Trace.WriteLine(Trace.kKinsky, "Bookmarks file corrupt: " + e.ToString());
            }
        }

        private void MigrateOldBookmarks()
        {
            string dataPath = Path.GetDirectoryName(iFilename);
            // rename KinskyDesktop to Kinsky
            if (dataPath.Contains("Kinsky"))
            {
                string previousDataPath;
                previousDataPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "KinskyDesktop");
                if (Directory.Exists(previousDataPath))
                {
                    string oldFilename = Path.Combine(previousDataPath, Path.GetFileName(iFilename));
                    if (File.Exists(oldFilename))
                    {
                        UserLog.WriteLine("Migrating bookmarks from " + oldFilename);
                        LoadBookmarks(oldFilename);
                        Save();
                    }
                }
            }
        }


        private void OnBookmarkAdded(Bookmark aBookmark)
        {
            if (EventBookmarkAdded != null)
            {
                EventBookmarkAdded(this, new EventArgsBookmark(aBookmark));
            }
        }

        private void OnBookmarkRemoved(Bookmark aBookmark)
        {
            if (EventBookmarkRemoved != null)
            {
                EventBookmarkRemoved(this, new EventArgsBookmark(aBookmark));
            }
        }

        private void OnBookmarksChanged()
        {
            if (EventBookmarksChanged != null)
            {
                EventBookmarksChanged(this, EventArgs.Empty);
            }
        }

        private object iLock;
        private string iFilename;
        private List<Bookmark> iBookmarks;
        private static string kFileVersion = "1.0";
        public event EventHandler<EventArgsBookmark> EventBookmarkAdded;
        public event EventHandler<EventArgsBookmark> EventBookmarkRemoved;
        public event EventHandler<EventArgs> EventBookmarksChanged;
    }

    public class EventArgsBookmark : EventArgs
    {
        public EventArgsBookmark(Bookmark aBookmark)
            : base()
        {
            Bookmark = aBookmark;
        }

        public Bookmark Bookmark { get; set; }
    }

    public class Bookmark
    {
        internal Bookmark()
        {
            BreadcrumbTrail = BreadcrumbTrail.Default;
        }

        public Bookmark(BreadcrumbTrail aBreadcrumbTrail)
        {
            BreadcrumbTrail = aBreadcrumbTrail;
            if (BreadcrumbTrail.Count > 0)
            {
                Title = BreadcrumbTrail[BreadcrumbTrail.Count - 1].Title;
                Class = string.Empty;
                Image = string.Empty;
            }
        }

        public Bookmark(Location aLocation)
            : this()
        {
            BreadcrumbTrail = aLocation.BreadcrumbTrail;

            Title = DidlLiteAdapter.Title(aLocation.Current.Metadata);
            Class = aLocation.Current.Metadata.Class;
            Uri uri = DidlLiteAdapter.ArtworkUri(aLocation.Current.Metadata);
            if (uri != null)
            {
                Image = uri.OriginalString;
            }
            else
            {
                Image = string.Empty;
            }
        }

        public void Load(XmlNode aBookmarkNode)
        {
            if (aBookmarkNode.Attributes.GetNamedItem("title") != null)
            {
                Title = aBookmarkNode.Attributes.GetNamedItem("title").Value;
            }
            if (aBookmarkNode.Attributes.GetNamedItem("image") != null)
            {
                Image = aBookmarkNode.Attributes.GetNamedItem("image").Value;
            }
            if (aBookmarkNode.Attributes.GetNamedItem("class") != null)
            {
                Class = aBookmarkNode.Attributes.GetNamedItem("class").Value;
            }
            if (BreadcrumbTrail == null)
            {
                BreadcrumbTrail = BreadcrumbTrail.Default;
            }
            BreadcrumbTrail.Load(aBookmarkNode);
        }

        public XmlElement Save(XmlDocument aDocument)
        {
            XmlElement bookmarkElement = aDocument.CreateElement("Bookmark");

            //@title
            XmlAttribute titleAttribute = aDocument.CreateAttribute("title");
            titleAttribute.Value = Title;
            bookmarkElement.Attributes.Append(titleAttribute);

            //@image
            XmlAttribute imageAttribute = aDocument.CreateAttribute("image");
            imageAttribute.Value = Image;
            bookmarkElement.Attributes.Append(imageAttribute);

            //@class
            XmlAttribute classAttribute = aDocument.CreateAttribute("class");
            classAttribute.Value = Class;
            bookmarkElement.Attributes.Append(classAttribute);

            if (BreadcrumbTrail == null)
            {
                BreadcrumbTrail = BreadcrumbTrail.Default;
            }

            bookmarkElement.AppendChild(BreadcrumbTrail.Save(aDocument));
            return bookmarkElement;
        }

        public string Title { get; set; }
        public string Image { get; set; }
        public string Class { get; set; }
        public BreadcrumbTrail BreadcrumbTrail { get; internal set; }

        public override string ToString()
        {
            return String.Format("Bookmark - Title={0}, Breadcrumbs={1}", Title, BreadcrumbTrail);
        }
    }

    public class BreadcrumbTrail : ReadOnlyCollection<Breadcrumb>
    {
        private static string kHomeId = "Home";
        private static string kHomeTitle = "Home";

        public static BreadcrumbTrail Default
        {
            get
            {
                List<Breadcrumb> list = new List<Breadcrumb>();
                list.Add(new Breadcrumb(kHomeId, kHomeTitle));
                return new BreadcrumbTrail(list);
            }
        }

        public BreadcrumbTrail(IList<Breadcrumb> aBreadcrumbList) : base(aBreadcrumbList) { }

        public BreadcrumbTrail TruncateStart(int aLevels)
        {
            // create a new BreadcrumbTrail by truncating from the start of this trail
            Assert.Check(aLevels < this.Count);
            List<Breadcrumb> newTrail = new List<Breadcrumb>();
            for (int i = aLevels; i < this.Count; i++)
            {
                newTrail.Add(this[i]);
            }
            return new BreadcrumbTrail(newTrail);
        }

        public BreadcrumbTrail TruncateEnd(int aLevels)
        {
            // create a new BreadcrumbTrail by truncating from the end of this trail
            Assert.Check(aLevels < this.Count);
            List<Breadcrumb> newTrail = new List<Breadcrumb>();
            for (int i = 0; i < (this.Count - aLevels); i++)
            {
                newTrail.Add(this[i]);
            }
            return new BreadcrumbTrail(newTrail);
        }

        public XmlElement Save(XmlDocument aDocument)
        {
            //<Breadcrumb>
            XmlElement trailElement = aDocument.CreateElement("BreadcrumbTrail");
            foreach (Breadcrumb breadcrumb in this)
            {
                XmlElement breadcrumbElement = aDocument.CreateElement("Breadcrumb");
                //@id
                XmlAttribute idAttribute = aDocument.CreateAttribute("id");
                idAttribute.Value = breadcrumb.Id;
                breadcrumbElement.Attributes.Append(idAttribute);
                //@title
                XmlAttribute breadcrumbTitleAttribute = aDocument.CreateAttribute("title");
                breadcrumbTitleAttribute.Value = breadcrumb.Title;
                breadcrumbElement.Attributes.Append(breadcrumbTitleAttribute);
                trailElement.AppendChild(breadcrumbElement);
            }
            return trailElement;
        }

        public void Load(XmlNode aParentNode)
        {
            this.Items.Clear();
            foreach (XmlNode breadcrumbNode in aParentNode.SelectNodes("BreadcrumbTrail/Breadcrumb"))
            {
                this.Items.Add(new Breadcrumb(breadcrumbNode.Attributes.GetNamedItem("id").Value,
                                                   breadcrumbNode.Attributes.GetNamedItem("title").Value));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Breadcrumb b in this)
            {
                sb.AppendFormat("{{{0}:{1}}}/", b.Id, b.Title);
            }
            return sb.ToString();
        }
    }

    public class Breadcrumb
    {
        public Breadcrumb(string aId, string aTitle)
        {
            Id = aId;
            Title = aTitle;
        }
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class OptionBreadcrumbTrail : OptionString
    {
        public OptionBreadcrumbTrail(string aId, string aName, string aDescription, BreadcrumbTrail aDefault)
            : base(aId, aName, aDescription, BreadcrumbTrailConverter.Convert(aDefault))
        {
        }

        public BreadcrumbTrail BreadcrumbTrail
        {
            get
            {
                return BreadcrumbTrailConverter.Convert(Value);
            }
            set
            {
                Set(BreadcrumbTrailConverter.Convert(value));
            }
        }

        static class BreadcrumbTrailConverter
        {
            public static BreadcrumbTrail Convert(string aBreadcrumbTrail)
            {
                BreadcrumbTrail result = BreadcrumbTrail.Default;
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(aBreadcrumbTrail);
                    result.Load(doc);
                }
                catch (XmlException) { }
                return result;
            }

            public static string Convert(BreadcrumbTrail aBreadcrumbTrail)
            {
                XmlDocument doc = new XmlDocument();
                if (aBreadcrumbTrail != null)
                {
                    doc.AppendChild(aBreadcrumbTrail.Save(doc));
                }
                return doc.OuterXml;
            }
        }
    }

}