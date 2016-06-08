using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

using Linn;

using Upnp;
using Linn.Kinsky;

[assembly: ContentDirectoryFactoryType("OssKinskyMppFolder.MediaProviderFolderFactory")]

namespace OssKinskyMppFolder
{
    public class MediaProviderFolderFactory : IContentDirectoryFactory
    {
        public IContentDirectory Create(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            return new MediaProviderFolder(aDataPath, aSupport);
        }
    }

    public class MediaProviderFolder : IContentDirectory, IContainer
    {
        public MediaProviderFolder(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            iSupport = aSupport;

            iMutex = new Mutex(false);
            iFolders = new List<string>();

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            iMetadata = new container();
            iMetadata.Id = kRootId;
            iMetadata.Title = Name;
            iMetadata.WriteStatus = "PROTECTED";
            iMetadata.Restricted = false;
            iMetadata.Searchable = true;
            iMetadata.AlbumArtUri.Add(iSupport.VirtualFileSystem.Uri(Path.Combine(path, "Folder.png")));

            iPage = new OptionPage("Folders");
            iOption = new OptionListFolderPath("folders", "Folders", "Folders to be included");
            iPage.Add(iOption);
            iOption.EventValueChanged += FoldersChanged;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public string Name
        {
            get
            {
                return "Folder";
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
                return iPage;
            }
        }

        public uint Open()
        {
            iMutex.WaitOne();

            uint count = (uint)iFolders.Count;

            iMutex.ReleaseMutex();

            return count;
        }

        public void Close() { }

        public void Refresh() { }

        public IContainer ChildContainer(container aContainer)
        {
            return new ContainerFolder(aContainer, iSupport.VirtualFileSystem);
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
                if (o is item)
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
                if (o.Res.Count > 0)
                {
                    iOption.Add(o.Res[0].Uri);
                }
            }
        }
		
	public bool HandleDelete(DidlLite aDidlLite)
	{
		return true;
	}

        public void Delete(string aId)
        {
            iMutex.WaitOne();
            IList<string> folders = new List<string>(iFolders);
            iMutex.ReleaseMutex();

            foreach (string s in folders)
            {
                if (aId == s)
                {
                    iOption.Remove(s);
                    break;
                }
            }
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
            iMutex.WaitOne();

            DidlLite didl = new DidlLite();

            foreach (string s in iFolders)
            {
                storageFolder folder = new storageFolder();
                resource resource = new resource();
                folder.Res.Add(resource);

                folder.Id = s;
                folder.ParentId = kRootId;
                folder.Title = s;
                folder.WriteStatus = "PROTECTED";
                folder.Restricted = true;
                folder.Searchable = true;

                resource.Uri = s;

                didl.Add(folder);
            }

            iMutex.ReleaseMutex();

            return didl;
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
            get { return iMetadata.Id; }
        }


        public bool HasTreeChangeAffectedLeaf
        {
            get
            {
                return false;
            }
        }

        private void FoldersChanged(object sender, EventArgs e)
        {
            iMutex.WaitOne();
            
            iFolders = new List<string>(iOption.Native);

            iMutex.ReleaseMutex();

            if (EventContentUpdated != null)
            {
                EventContentUpdated(this, EventArgs.Empty);
            }
        }

        private const string kRootId = "Folder";

        private Mutex iMutex;

        private container iMetadata;

        private OptionPage iPage;
        private OptionListFolderPath iOption;
        private IList<string> iFolders;

        private IContentDirectorySupportV2 iSupport;
    }

    internal class ContainerFolder : IContainer
    {
        public ContainerFolder(container aContainer, IVirtualFileSystem aVirtualFileSystem)
        {
            iVirtualFileSystem = aVirtualFileSystem;
            iMetadata = aContainer;

            iFileSystemWatcher = new FileSystemWatcher();
            iFileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            iFileSystemWatcher.Filter = "*.*";

            iFileSystemWatcher.Changed += Changed;
            iFileSystemWatcher.Created += Changed;
            iFileSystemWatcher.Deleted += Changed;
            iFileSystemWatcher.Renamed += Renamed;
            iFileSystemWatcher.Error += Error;

            iPath = new DirectoryInfo(iMetadata.Id);
            try
            {
                iFileSystemWatcher.Path = iPath.FullName;
                iFileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception) { }
        }

        public uint Open()
        {
            try
            {
                iDirectories = iPath.GetDirectories();
                Array.Sort<DirectoryInfo>(iDirectories, new Comparison<DirectoryInfo>(CompareDirectories));
                iFiles = iPath.GetFiles();
                Array.Sort<FileInfo>(iFiles, new Comparison<FileInfo>(CompareFiles));
                return (uint)(iDirectories.Length + iFiles.Length);
            }
            catch (Exception e)
            {
                UserLog.WriteLine("Failed to open " + iPath.FullName + ": " + e.Message);

                return 0;
            }
        }

        public void Close() { }

        public void Refresh() { }

        public IContainer ChildContainer(container aContainer)
        {
            return new ContainerFolder(aContainer, iVirtualFileSystem);
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
            return false;
        }

        public void Insert(string aAfterId, DidlLite aDidlLite)
        {
            throw new NotSupportedException();
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

        private static int CompareDirectories(DirectoryInfo x, DirectoryInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }
        private static int CompareFiles(FileInfo x, FileInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }

        public DidlLite Items(uint aStartIndex, uint aCount)
        {
            DidlLite didl = new DidlLite();

            uint endIndex = aStartIndex + aCount;
            for (uint i = aStartIndex; i < endIndex && i < (iDirectories.Length + iFiles.Length); ++i)
            {
                if (i < iDirectories.Length)
                {
                    DirectoryInfo d = iDirectories[i];
                    string artworkUri = string.Empty;
                    try
                    {
                        artworkUri = UpnpObjectFactory.FindArtworkUri(d, iVirtualFileSystem);
                    }
                    catch (UnauthorizedAccessException) { } // failed on directory permissions, just create an empty uri
                    storageFolder folder = UpnpObjectFactory.Create(d, artworkUri);
                    folder.ParentId = (d.Parent == null) ? "" : d.Parent.FullName;

                    didl.Add(folder);
                }
                else
                {
                    FileInfo f = iFiles[i - iDirectories.Length];

                    string resUri = iVirtualFileSystem.Uri(f.FullName);
                    string artworkUri = UpnpObjectFactory.FindArtworkUri(iPath, iVirtualFileSystem);

                    upnpObject obj = UpnpObjectFactory.Create(f, artworkUri, resUri);
                    obj.ParentId = f.Directory.FullName;

                    didl.Add(obj);
                }
            }

            return didl;
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
            get { return iMetadata.Id; }
        }


        public bool HasTreeChangeAffectedLeaf
        {
            get
            {
                return false;
            }
        }

        private void Changed(object sender, FileSystemEventArgs e)
        {
            if (EventContentUpdated != null)
            {
                EventContentUpdated(this, EventArgs.Empty);
            }
        }

        private void Renamed(object sender, RenamedEventArgs e)
        {
            if (EventContentUpdated != null)
            {
                EventContentUpdated(this, EventArgs.Empty);
            }
        }

        private void Error(object sender, ErrorEventArgs e)
        {
            Exception ex = e.GetException();
            UserLog.WriteLine(DateTime.Now + ": FileSystem watcher error (" + ex + ") - file system changes will not be detected");
        }

        private DirectoryInfo iPath;
        private FileSystemWatcher iFileSystemWatcher;

        private IVirtualFileSystem iVirtualFileSystem;
        private container iMetadata;
        private DirectoryInfo[] iDirectories;
        private FileInfo[] iFiles;
        private storageFolder iTestDidlLiteStorageFolder;
    }
}
