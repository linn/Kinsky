
using System;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.IO;

using Upnp;

using Linn;
using Linn.Kinsky;

[assembly: ContentDirectoryFactoryType("OssKinskyMppItunes.MediaProviderItunesFactory")]

namespace OssKinskyMppItunes
{

    public class MediaProviderItunesFactory : IContentDirectoryFactory
    {
        public IContentDirectory Create(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            return new MediaProviderItunes(aDataPath, aSupport);
        }
    }

    public class MediaProviderItunes : IContentDirectory
    {
        public MediaProviderItunes(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            iSupport = aSupport;

            // create the user options - don't start any scanning yet - the options will
            // be set at default until the application gets any stored values from
            // the options file, which occurs after this constructor and before the Start() method
            iUserOptions = new UserOptions();
            iInstallPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
        }

        public void Start()
        {
            // application has now initialised the options with any stored values - ok
            // to listen for changes
            iUserOptions.LibraryXmlFileChanged += LibraryXmlFileChanged;

            // assume that the initial option specifies a valid xml file, so the library is
            // being updated - the error handling will reset it appropriately if not the case
            iRoot = new ContainerItunes(new ContainerDataMessage("Updating...", iSupport, XmlInserted, iInstallPath));

            // mimic this event to initialise all objects for the plugin
            LibraryXmlFileChanged(this, EventArgs.Empty);
        }

        public void Stop()
        {
            // plugin has stopped - stop listening for changes to the options
            iUserOptions.LibraryXmlFileChanged -= LibraryXmlFileChanged;

            if (iScanner != null)
            {
                iScanner.Stop();
                iScanner = null;
            }

            if (iFileWatcher != null)
            {
                iFileWatcher.Dispose();
                iFileWatcher = null;
            }
        }

        public string Name
        {
            get { return "Itunes"; }
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
            get { return iRoot; }
        }

        public IOptionPage OptionPage
        {
            get { return iUserOptions; }
        }

        private void XmlInserted(string aAfterId, DidlLite aDidlLite)
        {
            if (aDidlLite.Count > 0 && aDidlLite[0].Res.Count > 0)
            {
                iUserOptions.LibraryXmlFile = aDidlLite[0].Res[0].Uri;
            }
        }

        private void LibraryXmlFileChanged(object sender, EventArgs e)
        {
            // stop any existing scanner
            if (iScanner != null)
                iScanner.Stop();

            // change the root data to show updating
            iRoot.Data = new ContainerDataMessage("Updating...", iSupport, XmlInserted, iInstallPath);

            // rescan the new xml file
            iScanner = new LibraryScanner();
            iScanner.Start(iUserOptions.LibraryXmlFile, this.LibraryScannerFinished, this.LibraryScannerError);

            // kill off the old file watcher
            if (iFileWatcher != null)
            {
                iFileWatcher.EnableRaisingEvents = false;
                iFileWatcher.Dispose();
                iFileWatcher = null;
            }

            // create a new file watcher
            try
            {
                string path = Path.GetDirectoryName(iUserOptions.LibraryXmlFile);
                string file = Path.GetFileName(iUserOptions.LibraryXmlFile);

                iFileWatcher = new FileSystemWatcher();
                iFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                iFileWatcher.Path = path;
                iFileWatcher.Filter = file;
                iFileWatcher.IncludeSubdirectories = false;
                iFileWatcher.Changed += LibraryXmlFileUpdated;
                iFileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception)
            {
                if (iFileWatcher != null)
                {
                    iFileWatcher.Dispose();
                    iFileWatcher = null;
                }
            }
        }

        private void LibraryXmlFileUpdated(object sender, FileSystemEventArgs e)
        {
            LibraryXmlFileChanged(sender, e);
        }

        private void LibraryScannerFinished(Database aDatabase, Library aLibrary)
        {
            try
            {
                // scanning of the XML file has finished - scan for artwork
                AlbumArtProcessor albumArtProcessor = new AlbumArtProcessor(aLibrary);
                albumArtProcessor.Update(aDatabase.Root);
            }
            catch (Exception e)
            {
                // problem with artwork - just log the error and carry on so at least the plugin is usable
                UserLog.WriteLine(DateTime.Now + " iTunes plugin: error scanning for artwork in library file [" + aLibrary.XmlFile + "]: " + e);
            }

            try
            {
                // swap in the data for the root container that contains the scanned database
                iRoot.Data = new ContainerDataNode(aDatabase.Root, null, aLibrary, iSupport, XmlInserted, iInstallPath);
            }
            catch (Exception e)
            {
                // swap in the error message
                UserLog.WriteLine(DateTime.Now + " iTunes plugin: error converting library file [" + aLibrary.XmlFile + "] to database: " + e);
                iRoot.Data = new ContainerDataMessage("Error occurred in loading the iTunes Music Library file. Please reconfigure.", iSupport, XmlInserted, iInstallPath);
            }
        }

        private void LibraryScannerError()
        {
            // swap in the error message
            iRoot.Data = new ContainerDataMessage("Error occurred in loading the iTunes Music Library file. Please reconfigure.", iSupport, XmlInserted, iInstallPath);
        }

        private class LibraryScanner
        {
            public delegate void Finished(Database aDatabase, Library aLibrary);
            public delegate void Error();

            public void Start(string aLibraryXmlFile, Finished aFinished, Error aError)
            {
                UserLog.WriteLine(DateTime.Now + " iTunes plugin: scanning library file [" + aLibraryXmlFile + "]");
                Stop();

                iFinished = aFinished;
                iError = aError;

                try
                {
                    iLibrary = new Library(aLibraryXmlFile);
                }
                catch (Exception e)
                {
                    UserLog.WriteLine(DateTime.Now + " iTunes plugin: error scanninng library file [" + aLibraryXmlFile + "] - bad header information: " + e);
                    iDatabase = null;
                    iLibrary = null;
                    iLoader = null;
                    if (iError != null)
                        iError();
                    return;
                }
                
                // create the new database
                iDatabase = new Database();

                // sort by album
                {
                    INodeBuilder itemBuilder = new NodeItemBuilder(new ItemFactory());
                    INodeBuilder albumBuilder = new NodeContainerBuilder(itemBuilder, new ContainerAlbumFactory(new ComparerTrackNumber(iLibrary)));
                    iDatabase.AddTopLevelContainer("Album", albumBuilder);
                }

                // sort by artist
                {
                    INodeBuilder itemBuilder = new NodeItemBuilder(new ItemFactory());
                    INodeBuilder artistBuilder = new NodeContainerBuilder(itemBuilder, new ContainerArtistFactory(new ComparerAlbumTrackNumber(iLibrary)));
                    iDatabase.AddTopLevelContainer("Artist", artistBuilder);
                }

                // sort by album artist/album
                {
                    INodeBuilder itemBuilder = new NodeItemBuilder(new ItemFactory());
                    INodeBuilder albumBuilder = new NodeContainerBuilder(itemBuilder, new ContainerAlbumFactory(new ComparerTrackNumber(iLibrary)));
                    INodeBuilder artistBuilder = new NodeContainerBuilder(albumBuilder, new ContainerAlbumArtistFactory(null));
                    iDatabase.AddTopLevelContainer("Artist/Album", artistBuilder);
                }

                // sort by genre/artist/album
                {
                    INodeBuilder itemBuilder = new NodeItemBuilder(new ItemFactory());
                    INodeBuilder albumBuilder = new NodeContainerBuilder(itemBuilder, new ContainerAlbumFactory(new ComparerTrackNumber(iLibrary)));
                    INodeBuilder artistBuilder = new NodeContainerBuilder(albumBuilder, new ContainerAlbumArtistFactory(null));
                    INodeBuilder genreBuilder = new NodeContainerBuilder(artistBuilder, new ContainerGenreFactory(null));
                    iDatabase.AddTopLevelContainer("Genre/Artist/Album", genreBuilder);
                }

                try
                {
                    iLoader = new LibraryLoader(iLibrary);
                    iLoader.EventItemAdded += this.ItemAdded;
                    iLoader.EventPlaylistAdded += this.PlaylistAdded;
                    iLoader.EventFinished += this.LoadingFinshed;
                    iLoader.EventError += this.LoadingError;
                    iLoader.Start();
                }
                catch (Exception e)
                {
	                UserLog.WriteLine(DateTime.Now + " iTunes plugin: error scanninng library file [" + aLibraryXmlFile + "] - bad header information: " + e);
                    iDatabase = null;
                    iLibrary = null;
                    iLoader = null;
                    if (iError != null)
                        iError();
                }
            }

            public void Stop()
            {
                if (iLoader != null)
                    iLoader.Stop();
            }

            private void ItemAdded(LibraryItem aItem)
            {
                iDatabase.Add(aItem);
            }

            private void PlaylistAdded(LibraryPlaylist aPlaylist)
            {
                iDatabase.Add(aPlaylist);
            }

            private void LoadingFinshed()
            {
                UserLog.WriteLine(DateTime.Now + " iTunes plugin: completed scanninng library file [" + iLibrary.XmlFile + "]");
                if (iFinished != null)
                    iFinished(iDatabase, iLibrary);
            }

            private void LoadingError()
            {
                UserLog.WriteLine(DateTime.Now + " iTunes plugin: error scanninng library file [" + iLibrary.XmlFile + "] - XML track errors");
                if (iError != null)
                    iError();
            }

            private Database iDatabase;
            private Library iLibrary;
            private LibraryLoader iLoader;
            private Finished iFinished;
            private Error iError;
        }

        private IContentDirectorySupportV2 iSupport;
        private UserOptions iUserOptions;
        private LibraryScanner iScanner;
        private ContainerItunes iRoot;
        private string iInstallPath;
        private FileSystemWatcher iFileWatcher;
    }



    public class ContainerItunes : IContainer
    {
        public delegate void InsertDelegate(string aAfterId, DidlLite aDidlLite);

        public interface IData
        {
            uint ChildCount { get; }
            IData ChildContainer(container aContainer);
            container Metadata { get; }
            DidlLite Items(uint aStartIndex, uint aCount);
            InsertDelegate Inserter { get; }
        }

        public ContainerItunes(IData aData)
        {
            iData = aData;
            iMutex = new Mutex();
        }

        public IData Data
        {
            get
            {
                iMutex.WaitOne();
                IData data = iData;
                iMutex.ReleaseMutex();
                return data;
            }
            set
            {
                iMutex.WaitOne();
                iData = value;
                iMutex.ReleaseMutex();
                if (EventContentUpdated != null)
                    EventContentUpdated(this, null);
            }
        }

        public uint Open()
        {
            IData data = Data;
            return data.ChildCount;
        }

        public void Close() { }

        public void Refresh()
        {
        }

        public IContainer ChildContainer(container aContainer)
        {
            IData data = Data;
            return new ContainerItunes(data.ChildContainer(aContainer));
        }

        public container Metadata
        {
            get
            {
                IData data = Data;
                return data.Metadata;
            }
        }
		
		public bool HandleMove(DidlLite aDidlLite)
		{
			return false;
		}

        public bool HandleInsert(DidlLite aDidlLite)
        {
            if (aDidlLite.Count != 1 || !(aDidlLite[0] is item) || aDidlLite[0].Res.Count == 0)
                return false;

            IData data = Data;
            return (data.Inserter != null);
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            IData data = Data;
            if (data.Inserter != null)
                data.Inserter(aAfterId, aDidlLite);
        }
		
		public bool HandleDelete(DidlLite aDidlLite)
		{
			return false;
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

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            IData data = Data;
            return data.Items(aStartIndex, aCount);
        }

        public DidlLite Search(string aSearchCriterea, uint aStartIndex, uint aCount)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgs> EventContentUpdated;
        public event EventHandler<EventArgs> EventContentAdded { add { } remove { } }
        public event EventHandler<EventArgsContentRemoved> EventContentRemoved { add { } remove { } }
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
            get { return Metadata.Id; }
        }


        public bool HasTreeChangeAffectedLeaf
        {
            get
            {
                return false;
            }
        }

        private IData iData;
        private Mutex iMutex;
    }



    public class ContainerDataNode : ContainerItunes.IData
    {
        public ContainerDataNode(NodeContainer aNode, NodeContainer aParent,
                                 Library aLibrary, IContentDirectorySupportV2 aSupport,
                                 ContainerItunes.InsertDelegate aInserter, string aInstallPath)
        {
            iLibrary = aLibrary;
            iSupport = aSupport;
            iNode = aNode;
            iMetadataFactory = new MetadataFactory(iLibrary, iSupport, aInstallPath);
            iMetadata = iMetadataFactory.Create(iNode, aParent);
            iInserter = aInserter;
            iInstallPath = aInstallPath;
        }

        public uint ChildCount
        {
            get { return iNode.ChildCount; }
        }

        public ContainerItunes.IData ChildContainer(container aContainer)
        {
            foreach (NodeContainer node in iNode.ContainerList)
            {
                if (node.Id == Convert.ToUInt32(aContainer.Id))
                {
                    return new ContainerDataNode(node, iNode, iLibrary, iSupport, null, iInstallPath);
                }
            }

            return null;
        }

        public container Metadata
        {
            get { return iMetadata; }
        }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            uint endIndex = (aStartIndex + aCount < iNode.ChildCount) ? aStartIndex + aCount : iNode.ChildCount;
            for (uint i = aStartIndex; i < endIndex; i++)
            {
                upnpObject metadata = iMetadataFactory.Create(iNode.Child(i), iNode);
                didl.Add(metadata);
            }

            return didl;
        }

        public ContainerItunes.InsertDelegate Inserter
        {
            get { return iInserter; }
        }

        private Library iLibrary;
        private IContentDirectorySupportV2 iSupport;
        private NodeContainer iNode;
        private MetadataFactory iMetadataFactory;
        private container iMetadata;
        private ContainerItunes.InsertDelegate iInserter;
        private string iInstallPath;
    }



    public class ContainerDataMessage : ContainerItunes.IData
    {
        public ContainerDataMessage(string aText, IContentDirectorySupportV2 aSupport, ContainerItunes.InsertDelegate aInserter, string aInstallPath)
        {
            // create a root node
            NodeRoot root = new NodeRoot("iTunes", 0, null);

            // create the metadata for it
            MetadataFactory factory = new MetadataFactory(null, aSupport, aInstallPath);
            iMetadata = factory.Create(root, null);

            // create the child metadata
            iChildMetadata = new item();
            iChildMetadata.Id = "1";
            iChildMetadata.ParentId = "0";
            iChildMetadata.Restricted = true;
            iChildMetadata.Title = aText;
            iChildMetadata.WriteStatus = "PROTECTED";
            try
            {
                iChildMetadata.ArtworkUri.Add(aSupport.VirtualFileSystem.Uri(Path.Combine(aInstallPath, "Itunes.png")));
            }
            catch(HttpServerException) { }
            iInserter = aInserter;
        }

        public uint ChildCount
        {
            get { return 1; }
        }

        public ContainerItunes.IData ChildContainer(container aContainer)
        {
            Assert.Check(false);
            return null;
        }

        public container Metadata
        {
            get { return iMetadata; }
        }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            Assert.Check(aStartIndex == 0);

            DidlLite didl = new DidlLite();
            didl.Add(iChildMetadata);
            return didl;
        }

        public ContainerItunes.InsertDelegate Inserter
        {
            get { return iInserter; }
        }

        private container iMetadata;
        private item iChildMetadata;
        private ContainerItunes.InsertDelegate iInserter;
    }



    public class MetadataFactory : INodeProcessor
    {
        public MetadataFactory(Library aLibrary, IContentDirectorySupportV2 aSupport, string aInstallPath)
        {
            iLibrary = aLibrary;
            iSupport = aSupport;
            iInstallPath = aInstallPath;
        }

        public container Create(NodeContainer aNode, NodeContainer aParent)
        {
            iParent = aParent;
            aNode.Process(this);
            container metadata = iMetadata as container;
            Assert.Check(metadata != null);
            return metadata;
        }

        public upnpObject Create(INode aNode, NodeContainer aParent)
        {
            iParent = aParent;
            aNode.Process(this);
            return iMetadata;
        }

        public void Process(NodeItem aNode)
        {
            LibraryItem libItem = aNode.LibraryItem;

            musicTrack track = new musicTrack();
            track.Id = aNode.Id.ToString();
            track.RefId = aNode.RefId.ToString();
            track.ParentId = iParent.Id.ToString();
            track.Restricted = true;
            track.WriteStatus = "PROTECTED";

            if (libItem.Name != null)
                track.Title = libItem.Name;

            if (libItem.Album != null)
                track.Album.Add(libItem.Album);

            if (libItem.Artist != null)
            {
                artist artist = new artist();
                artist.Artist = libItem.Artist;
                track.Artist.Add(artist);
            }

            if (libItem.Genre != null)
                track.Genre.Add(libItem.Genre);

            if (libItem.DiscCount != -1)
                track.OriginalDiscCount = (int)libItem.DiscCount;

            if (libItem.DiscNumber != -1)
                track.OriginalDiscNumber = (int)libItem.DiscNumber;

            if (libItem.TrackNumber != -1)
                track.OriginalTrackNumber = (int)libItem.TrackNumber;

            resource res = new resource();
            if (libItem.BitRate != -1)
                res.Bitrate = ((int)libItem.BitRate * 1000) / 8;

            if (libItem.SampleRate != -1)
                res.SampleFrequency = (int)libItem.SampleRate;

            if (libItem.Size != -1)
                res.Size = libItem.Size;

            if (libItem.TotalTime != -1)
            {
                Upnp.Time totalTime = new Time(Convert.ToInt32(libItem.TotalTime / 1000));
                res.Duration = totalTime.ToString();
            }

            if (libItem.Kind.Contains("MPEG audio file"))
                res.ProtocolInfo = "http-get:*:audio/mpeg:*";
            else if (libItem.Kind.Contains("WAV audio file"))
                res.ProtocolInfo = "http-get:*:audio/x-wav:*";
            else if (libItem.Kind.Contains("AIFF audio file"))
                res.ProtocolInfo = "http-get:*:audio/x-aiff:*";
            else if (libItem.Kind.Contains("AAC audio file"))
                res.ProtocolInfo = "http-get:*:audio/x-m4a:*";
            else if (libItem.Kind.Contains("Apple Lossless audio file"))
                res.ProtocolInfo = "http-get:*:audio/x-m4a:*";
            else
                // if the file kind is unrecognised, set the protocol info so the
                // DS will decide - this means that KDT will add the track to the
                // DS playlist and then the DS can decide whether it wants to play it
                res.ProtocolInfo = "http-get:*:*:*";

            if (libItem.Location != null)
            {
                System.Uri uri = new System.Uri(libItem.Location);
                if (uri.Scheme == "file" && uri.Host == "localhost")
                {
                    // handle paths that are "/Users/..." or "/C:\\Users\\..." i.e. mac or windows
                    // strings passed to the VirtualFileSystem.Uri method must be unescaped. The unescaping
                    // was initially implemented at the point where the Location element is read from
                    // the XML file (the Location URI in the XML file is escaped). However, when this
                    // unescaped string was passed to the Uri constructor, the uri.AbsolutePath returns
                    // an **escaped** string, thus another unescape had to be performed here anyway. So,
                    // no point in doing it twice - just do it here
                    string path = System.Uri.UnescapeDataString(uri.AbsolutePath);
                    if (path[2] == ':')
                        path = path.Substring(1);
                    try
                    {
                        res.Uri = iSupport.VirtualFileSystem.Uri(path);
                    }
                    catch(HttpServerException) { }
                }
            }

            if (res.ProtocolInfo != "" && res.Uri != "")
                track.Res.Add(res);

            if (libItem.AlbumArtId != null)
            {
                string filename = iLibrary.GetAlbumArtFilenameNoExt(libItem.AlbumArtId) + libItem.AlbumArtExt;
                try
                {
                    string albumArtUri = iSupport.VirtualFileSystem.Uri(filename);          
                    track.AlbumArtUri.Add(albumArtUri);
                }
                catch(HttpServerException) { }
            }

            iMetadata = track;
        }

        public void Process(NodeContainer aNode)
        {
            container metadata = new container();
            SetContainerMetadata(metadata, aNode);
            iMetadata = metadata;
        }

        public void Process(NodeRoot aNode)
        {
            container metadata = new container();
            try
            {
                metadata.AlbumArtUri.Add(iSupport.VirtualFileSystem.Uri(Path.Combine(iInstallPath, "Itunes.png")));
            }
            catch(HttpServerException) { }
            SetContainerMetadata(metadata, aNode);
            iMetadata = metadata;
        }

        public void Process(NodeAlbum aNode)
        {
            musicAlbum metadata = new musicAlbum();
            SetContainerMetadata(metadata, aNode);

            LibraryItem albumArtItem = null;
            foreach (NodeItem item in aNode.ItemList)
            {
                LibraryItem libItem = item.LibraryItem;
                if (libItem.AlbumArtId != null)
                {
                    albumArtItem = libItem;
                    break;
                }
            }

            if (albumArtItem != null)
            {
                string filename = iLibrary.GetAlbumArtFilenameNoExt(albumArtItem.AlbumArtId) + albumArtItem.AlbumArtExt;
                try
                {
                   string albumArtUri = iSupport.VirtualFileSystem.Uri(filename);
                    metadata.AlbumArtUri.Add(albumArtUri);
                }
                catch(HttpServerException) { }
            }

            iMetadata = metadata;
        }

        public void Process(NodeArtist aNode)
        {
            musicArtist metadata = new musicArtist();
            SetContainerMetadata(metadata, aNode);
            iMetadata = metadata;
        }

        public void Process(NodeGenre aNode)
        {
            musicGenre metadata = new musicGenre();
            SetContainerMetadata(metadata, aNode);
            iMetadata = metadata;
        }

        public void Process(NodePlaylist aNode)
        {
            playlistContainer metadata = new playlistContainer();
            SetContainerMetadata(metadata, aNode);
            iMetadata = metadata;
        }

        private void SetContainerMetadata(container aContainer, NodeContainer aNode)
        {
            aContainer.Id = aNode.Id.ToString();
            aContainer.ParentId = (iParent != null) ? iParent.Id.ToString() : "-1";
            aContainer.ChildCount = (int)aNode.ChildCount;
            aContainer.Restricted = true;
            aContainer.Searchable = false;
            aContainer.Title = aNode.Name;
            aContainer.WriteStatus = "PROTECTED";
        }

        IContentDirectorySupportV2 iSupport;
        private Library iLibrary;
        private NodeContainer iParent;
        private upnpObject iMetadata;
        private string iInstallPath;
    }


    public class AlbumArtProcessor : INodeProcessor
    {
        public AlbumArtProcessor(Library aLibrary)
        {
            iLibrary = aLibrary;

            // build all the dictionaries to hold the file info for the .itc files
            // The structure mirrors the file system. Files are in a 3 level deep
            // directory structure i.e.
            //
            // /album art root/<digit1>/<digit2>/<digit3>/file1.itc2
            // /album art root/<digit1>/<digit2>/<digit3>/file2.itc2
            // ...etc...
            // 
            // and the corresponding structure is
            //
            // List<string> l = iFileDict[<digit1>][<digit2>][<digit3>]
            //
            // where the list, l, will contain the list of files in the corresponding folder
            //
            iFileDict = new Dictionary<uint, Dictionary<uint, Dictionary<uint, List<string>>>>();
            for (uint d1 = 0; d1 < 16; d1++)
            {
                iFileDict.Add(d1, new Dictionary<uint, Dictionary<uint, List<string>>>());

                for (uint d2 = 0; d2 < 16; d2++)
                {
                    iFileDict[d1].Add(d2, new Dictionary<uint, List<string>>());

                    for (uint d3 = 0; d3 < 16; d3++)
                    {
                        iFileDict[d1][d2].Add(d3, new List<string>());

                        string path = Path.Combine(iLibrary.AlbumArtRoot, d1.ToString("00"));
                        path = Path.Combine(path, d2.ToString("00"));
                        path = Path.Combine(path, d3.ToString("00"));

                        if (Directory.Exists(path))
                        {
                            iFileDict[d1][d2][d3].AddRange(Directory.GetFiles(path, "*.itc*"));
                        }
                    }
                }
            }
        }

        public void Update(INode aNode)
        {
            // process this node
            aNode.Process(this);

            // update all children
            for (uint i = 0; i < aNode.ChildCount; i++)
            {
                Update(aNode.Child(i));
            }
        }

        // these types are ignored
        public void Process(NodeItem aNode) {}
        public void Process(NodeContainer aNode) {}
        public void Process(NodeRoot aNode) {}
        public void Process(NodeArtist aNode) {}
        public void Process(NodeGenre aNode) {}
        public void Process(NodePlaylist aNode) {}

        public void Process(NodeAlbum aNode)
        {
            try
            {
                // find the ID of the track containing the album art
                string albumArtId = null;
                string albumArtExt = null;

                foreach (NodeItem item in aNode.ItemList)
                {
                    LibraryItem libItem = item.LibraryItem;

                    // obtain the 3 digits for the folder hierarchy where the artwork file
                    // for this track will reside
                    uint[] d = iLibrary.GetAlbumArtFilenameDigits(libItem.PersistentId);
                    if (d == null)
                        continue;

                    List<string> l = iFileDict[d[0]][d[1]][d[2]];

                    // check if the artwork file exists for this track
                    string filename = iLibrary.GetAlbumArtFilenameNoExt(libItem.PersistentId);
                    if (l.IndexOf(filename + ".itc") != -1)
                    {
                        albumArtId = libItem.PersistentId;
                        albumArtExt = ".itc";
                        break;
                    }
                    else if (l.IndexOf(filename + ".itc2") != -1)
                    {
                        albumArtId = libItem.PersistentId;
                        albumArtExt = ".itc2";
                        break;
                    }
                }

                // now set the album art ID for each track
                if (albumArtId != null)
                {
                    foreach (NodeItem item in aNode.ItemList)
                    {
                        LibraryItem libItem = item.LibraryItem;
                        libItem.AlbumArtId = albumArtId;
                        libItem.AlbumArtExt = albumArtExt;
                    }
                }
            }
            catch (Exception e)
            {
                UserLog.WriteLine(DateTime.Now + ": iTunes plugin: error scanning album art - skipping album: " + aNode.Name + ", " + e.Message);
            }
        }

        private Library iLibrary;
        private Dictionary<uint, Dictionary<uint, Dictionary<uint, List<string>>>> iFileDict;
    }


} // OssKinskyMppItunes



