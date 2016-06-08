
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Upnp;


namespace Linn.Kinsky
{
    public interface IDraggableData
    {
        Linn.Kinsky.MediaProviderDraggable GetInternal();
        string GetText();
        string GetUri();
        string[] GetFileList();
    }

    public interface IDropConverter
    {
        MediaProviderDraggable Convert(IDraggableData aDraggableData);
    }

    public class DropConverterRoot : IDropConverter
    {
        public DropConverterRoot()
        {
            iDropConverters = new List<IDropConverter>();
        }

        public void Add(IDropConverter aDropConverter)
        {
            iDropConverters.Add(aDropConverter);
        }

        public MediaProviderDraggable Convert(IDraggableData aDraggableData)
        {
            foreach (IDropConverter c in iDropConverters)
            {
                MediaProviderDraggable draggable = c.Convert(aDraggableData);
                if (draggable != null)
                {
                    return draggable;
                }
            }

            return null;
        }

        private List<IDropConverter> iDropConverters;
    }

    public class DropConverterInternal : IDropConverter
    {
        public MediaProviderDraggable Convert(IDraggableData aDraggableData)
        {
            return aDraggableData.GetInternal();
        }
    }

    public class DropConverterText : IDropConverter
    {
        public MediaProviderDraggable Convert(IDraggableData aDraggableData)
        {
            string text = aDraggableData.GetText();
            if (text != null)
            {
                List<upnpObject> list = new List<upnpObject>();
                list.Add(UpnpObjectFactory.Create(text, text));
                return new MediaProviderDraggable(new MediaRetrieverNoRetrieve(list));
            }

            return null;
        }
    }

    public class DropConverterUri : IDropConverter
    {
        public MediaProviderDraggable Convert(IDraggableData aDraggableData)
        {
            string uri = aDraggableData.GetUri();
            if (uri != null)
            {
                List<upnpObject> list = new List<upnpObject>();
                list.Add(UpnpObjectFactory.Create(uri, uri));
                return new MediaProviderDraggable(new MediaRetrieverNoRetrieve(list));
            }

            return null;
        }
    }

    public class DropConverterFileDrop : IDropConverter
    {
        public DropConverterFileDrop(IVirtualFileSystem aVirtualFileSystem, bool aExpandFolders)
        {
            iVirtualFileSystem = aVirtualFileSystem;
            iExpandFolders = aExpandFolders;
        }

        public MediaProviderDraggable Convert(IDraggableData aDraggableData)
        {
            string[] files = aDraggableData.GetFileList();
            if (files == null)
            {
                return null;
            }

            List<upnpObject> list = new List<upnpObject>();

            try
            {
                foreach (string f in files)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(f);
                        bool isFolder = (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        
                        if (isFolder && iExpandFolders)
                        {
                            // this container is a folder that must be expanded
                            DirectoryInfo dirInfo = new DirectoryInfo(f);
                            list.AddRange(CreateUpnpList(dirInfo));
                        }
                        else if (isFolder)
                        {
                            // this container is a folder that must not be expanded
                            DirectoryInfo dirInfo = new DirectoryInfo(f);
                            string artworkUri = UpnpObjectFactory.FindArtworkUri(dirInfo, iVirtualFileSystem);
                            list.Add(UpnpObjectFactory.Create(dirInfo, artworkUri));
                        }
                        else
                        {
                            if (fileInfo.Extension == PluginManager.kPluginExtension)
                            {
                                list.Add(UpnpObjectFactory.Create(fileInfo.Name, fileInfo.FullName, "application/zip"));
                            }
                            else if (fileInfo.Extension == Playlist.kPlaylistExtension)
                            {
                                list.Add(UpnpObjectFactory.Create(fileInfo, string.Empty, fileInfo.FullName));
                            }
                            else
                            {
                                string artworkUri = UpnpObjectFactory.FindArtworkUri(fileInfo.Directory, iVirtualFileSystem);
                                upnpObject o = UpnpObjectFactory.Create(fileInfo, artworkUri, iVirtualFileSystem.Uri(fileInfo.FullName));
                                if (o != null)
                                {
                                    list.Add(o);
                                }
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        UserLog.WriteLine("Error retrieving content - file not found: " + f);
                    }
                    catch (IOException ex)
                    {
                        UserLog.WriteLine("Error retrieving content - IOException: " + ex + f);
                    }
                }
            }
            catch(HttpServerException ex)
            {
                UserLog.WriteLine("Error retrieving content: " + ex.Message);
            }

            return new MediaProviderDraggable(new MediaRetrieverNoRetrieve(list));
        }

        private IList<upnpObject> CreateUpnpList(DirectoryInfo aDirectory)
        {
            List<upnpObject> list = new List<upnpObject>();

            foreach (DirectoryInfo di in aDirectory.GetDirectories())
            {
                list.AddRange(CreateUpnpList(di));
            }

            FileInfo[] files = aDirectory.GetFiles();
            string artworkUri = UpnpObjectFactory.FindArtworkUri(aDirectory, iVirtualFileSystem);

            foreach (FileInfo fi in files)
            {
                upnpObject o = UpnpObjectFactory.Create(fi, artworkUri, iVirtualFileSystem.Uri(fi.FullName));
                if (o != null)
                {
                    list.Add(o);
                }
            }

            return list;
        }

        private readonly string[] kImageSearchExt = { ".png", ".jpg" };
        private IVirtualFileSystem iVirtualFileSystem;
        private bool iExpandFolders;
    }
}



