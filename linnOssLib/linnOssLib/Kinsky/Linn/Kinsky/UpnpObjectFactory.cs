using System;
using System.IO;

using TagLib;

using Upnp;

namespace Linn.Kinsky
{
    public class UpnpObjectFactory
    {
        static UpnpObjectFactory()
        {
            // register aiff file extension for aiff files as taglib expects aif (ticket #1008)
            if (!TagLib.FileTypes.AvailableTypes.ContainsKey("taglib/aiff"))
            {
                TagLib.FileTypes.AvailableTypes.Add("taglib/aiff", typeof(TagLib.Aiff.File));
            }
        }

        public static storageFolder Create(DirectoryInfo aInfo, string aArtworkUri)
        {
            storageFolder folder = new storageFolderLocal(aInfo);
            resource resource = new resource();
            folder.Res.Add(resource);

            folder.Id = aInfo.FullName;
            folder.Title = aInfo.Name;
            folder.WriteStatus = "PROTECTED";
            folder.Restricted = true;
            folder.Searchable = true;

            folder.StorageUsed = -1;

            resource.ProtocolInfo = "internal:127.0.0.1:*:local-folder";
            resource.Uri = aInfo.FullName;

            if (!string.IsNullOrEmpty(aArtworkUri))
            {
                folder.AlbumArtUri.Add(aArtworkUri);
            }

            return folder;
        }

        public static upnpObject Create(string aTitle, string aResourceUri)
        {
            return Create(aTitle, aResourceUri, "*");
        }

        public static upnpObject Create(string aTitle, string aResourceUri, string aMimeType)
        {
            item item = new item();
            resource resource = new resource();
            item.Res.Add(resource);

            item.Id = aResourceUri;
            item.Title = aTitle;
            item.WriteStatus = "PROTECTED";
            item.Restricted = true;

            resource.ProtocolInfo = string.Format("http-get:*:{0}:*", aMimeType);
            resource.Uri = aResourceUri;

            return item;
        }

        public static upnpObject Create(FileInfo aInfo, string aArtworkUri, string aResourceUri)
        {
            // check for playlist file
            if (aInfo.Extension == Playlist.kPlaylistExtension)
            {
                playlistContainer playlist = new playlistContainer();
                resource resource = new resource();
                playlist.Res.Add(resource);

                playlist.Id = aInfo.FullName;
                playlist.Title = aInfo.Name;
                playlist.WriteStatus = "PROTECTED";
                playlist.Restricted = true;

                resource.Size = aInfo.Length;
                resource.Uri = aResourceUri;

                return playlist;
            }

            // check for audio/video file
            try
            {

                if (IsTagLibSupported(aInfo))
                {
                    TagLib.File f = TagLib.File.Create(aInfo.FullName);

                    if (f.Properties.MediaTypes == MediaTypes.Audio)
                    {
                        musicTrack track = new musicTrack();
                        resource resource = new resource();
                        track.Res.Add(resource);

                        track.Id = aInfo.FullName;
                        track.WriteStatus = "PROTECTED";
                        track.Restricted = true;

                        if (!f.Tag.IsEmpty)
                        {
                            if (f.Tag.Title != null)
                            {
                                track.Title = f.Tag.Title;
                            }
                            else
                            {
                                track.Title = aInfo.Name;
                            }
                            if (f.Tag.Album != null)
                            {
                                track.Album.Add(f.Tag.Album);
                            }
                            foreach (string g in f.Tag.Genres)
                            {
                                track.Genre.Add(g);
                            }
                            track.OriginalTrackNumber = (int)f.Tag.Track;
                            track.Date = f.Tag.Year.ToString();
                            foreach (string p in f.Tag.Performers)
                            {
                                artist performer = new artist();
                                performer.Artist = p;
                                performer.Role = "Performer";
                                track.Artist.Add(performer);
                            }
                            foreach (string a in f.Tag.AlbumArtists)
                            {
                                artist artist = new artist();
                                artist.Artist = a;
                                artist.Role = "AlbumArtist";
                                track.Artist.Add(artist);
                            }
                            foreach (string c in f.Tag.Composers)
                            {
                                artist composer = new artist();
                                composer.Artist = c;
                                composer.Role = "Composer";
                                track.Artist.Add(composer);
                            }
                            if (f.Tag.Conductor != null)
                            {
                                artist conductor = new artist();
                                conductor.Artist = f.Tag.Conductor;
                                conductor.Role = "Conductor";
                                track.Artist.Add(conductor);
                            }
                        }
                        else
                        {
                            track.Title = aInfo.Name;
                        }

                        resource.Bitrate = (int)((f.Properties.AudioBitrate * 1000.0f) / 8.0f);
                        resource.Duration = new Time((int)f.Properties.Duration.TotalSeconds).ToString();
                        resource.NrAudioChannels = f.Properties.AudioChannels;
                        resource.SampleFrequency = f.Properties.AudioSampleRate;
                        resource.Size = aInfo.Length;
                        resource.Uri = aResourceUri;

                        resource.ProtocolInfo = string.Format("http-get:*:{0}:*", f.MimeType.Replace("taglib", "audio"));
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("flac", "x-flac");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("aif:", "aiff:");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("wma", "x-ms-wma");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("asf", "x-ms-asf");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("mp3", "mpeg");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("mpeg3", "mpeg");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("m4a", "x-m4a");
                        resource.ProtocolInfo = resource.ProtocolInfo.Replace("dsf", "x-dsf");

                        if (!string.IsNullOrEmpty(aArtworkUri))
                        {
                            track.AlbumArtUri.Add(aArtworkUri);
                        }

                        return track;
                    }
                    else if (f.Properties.MediaTypes == TagLib.MediaTypes.Video)
                    {
                        videoItem video = new videoItem();
                        resource resource = new resource();
                        video.Res.Add(resource);

                        video.Id = aInfo.FullName;
                        video.WriteStatus = "PROTECTED";
                        video.Restricted = true;

                        if (!f.Tag.IsEmpty)
                        {
                            if (f.Tag.Title != null)
                            {
                                video.Title = f.Tag.Title;
                            }
                            else
                            {
                                video.Title = aInfo.Name;
                            }
                            foreach (string g in f.Tag.Genres)
                            {
                                video.Genre.Add(g);
                            }
                            foreach (string p in f.Tag.Performers)
                            {
                                actor performer = new actor();
                                performer.Actor = p;
                                performer.Role = "Actor";
                                video.Actor.Add(performer);
                            }
                        }
                        else
                        {
                            video.Title = aInfo.Name;
                        }

                        resource.Bitrate = (int)((f.Properties.AudioBitrate * 1000.0f) / 8.0f);
                        resource.Duration = new Time((int)f.Properties.Duration.TotalSeconds).ToString();
                        resource.NrAudioChannels = f.Properties.AudioChannels;
                        resource.SampleFrequency = f.Properties.AudioSampleRate;
                        resource.Size = aInfo.Length;
                        resource.Uri = aResourceUri;

                        resource.ProtocolInfo = string.Format("http-get:*:{0}:*", f.MimeType.Replace("taglib", "video"));

                        if (!string.IsNullOrEmpty(aArtworkUri))
                        {
                            video.AlbumArtUri.Add(aArtworkUri);
                        }

                        return video;
                    }
                }
            }
            catch (TagLib.UnsupportedFormatException)
            {
            }
            catch (Exception e)
            {
                UserLog.WriteLine(aInfo.FullName + ": " + e.Message);
            }

            // check for image file
            string mimeType;
            if (IsImageFile(aInfo, out mimeType))
            {
                photo photo = new photo();
                resource resource = new resource();
                photo.Res.Add(resource);

                photo.Id = aInfo.FullName;
                photo.Title = aInfo.Name;
                photo.WriteStatus = "PROTECTED";
                photo.Restricted = true;

                resource.Size = aInfo.Length;
                resource.Uri = aResourceUri;
                resource.ProtocolInfo = string.Format("http-get:*:{0}:*", mimeType);

                if (!string.IsNullOrEmpty(aArtworkUri))
                {
                    photo.AlbumArtUri.Add(aArtworkUri);
                }

                return photo;
            }

            // all other types
            {
                item item = new item();
                resource resource = new resource();
                item.Res.Add(resource);

                item.Id = aInfo.FullName;
                item.Title = aInfo.Name;
                item.WriteStatus = "PROTECTED";
                item.Restricted = true;

                resource.Size = aInfo.Length;
                resource.Uri = aResourceUri;
                // dff (dsdiff codec) files are not currently supported by Taglib, but we still want the correct mime type for the resource
                if (aInfo.Extension != null && aInfo.Extension.ToLowerInvariant() == ".dff") 
                {
                    resource.ProtocolInfo = string.Format("http-get:*:audio/x-dff:*");
                }
                else
                {
                    resource.ProtocolInfo = string.Format("http-get:*:application/octet-stream:*");
                }

                if (!string.IsNullOrEmpty(aArtworkUri))
                {
                    item.AlbumArtUri.Add(aArtworkUri);
                }

                return item;
            }
        }

        private static bool IsTagLibSupported(FileInfo aFileInfo)
        {
            string str = string.Empty;
            int startIndex = aFileInfo.Name.LastIndexOf(".") + 1;
            if ((startIndex >= 1) && (startIndex < aFileInfo.Name.Length))
            {
                str = aFileInfo.Name.Substring(startIndex, aFileInfo.Name.Length - startIndex);
            }
            string mimetype = "taglib/" + str.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            return FileTypes.AvailableTypes.ContainsKey(mimetype);
        }

        public static string FindArtworkUri(DirectoryInfo aInfo, IVirtualFileSystem aVirtualFileSystem)
        {
            string[] kImageSearchExt = { ".jpg", ".png" };
            string[] kImageSearchName = { "folder", "cover" };
            try
            {
                foreach (string s in kImageSearchExt)
                {
                    foreach (string n in kImageSearchName)
                    {

                        string filename = Path.Combine(aInfo.FullName, string.Format("{0}{1}", n, s));
                        if (System.IO.File.Exists(filename))
                        {
                            return aVirtualFileSystem.Uri(filename);
                        }
                    }
                }
            }
            catch { } //ignore path errors

            return null;
        }

        private static bool IsImageFile(FileInfo aInfo, out string aMimeType)
        {
            switch (aInfo.Extension)
            {
                case ".bmp":
                    aMimeType = "image/bmp";
                    return true;

                case ".gif":
                    aMimeType = "image/gif";
                    return true;

                case ".jpeg":
                case ".jpg":
                    aMimeType = "image/jpeg";
                    return true;

                case ".png":
                    aMimeType = "image/png";
                    return true;

                case ".tif":
                case ".tiff":
                    aMimeType = "image/tiff";
                    return true;

                default:
                    aMimeType = "application/octet-stream";
                    return false;
            }
        }
    }

    // lazy loading child count property
    internal class storageFolderLocal : storageFolder
    {
        private string iFullName;
        private int iCount = -1;
        public storageFolderLocal(DirectoryInfo aInfo)
            : base()
        {
            this.iFullName = aInfo.FullName;
        }

        public override int ChildCount
        {
            get
            {
                if (iCount == -1)
                {
                    try
                    {
                        iCount = new DirectoryInfo(iFullName).GetFileSystemInfos().Length;
                    }
                    catch
                    {
                        iCount = 0;
                    }
                }
                return iCount;
            }
            set
            {
                iCount = value;
            }
        }
    }
}


/* 
 TAGLIB - POTENTIAL MIMETYPES:
taglib/aac
audio/aac
taglib/aif
audio/x-aiff
audio/aiff
sound/aiff
application/x-aiff
taglib/ape
audio/x-ape
audio/ape
application/x-ape
taglib/wma
taglib/wmv
taglib/asf
audio/x-ms-wma
audio/x-ms-asf
video/x-ms-asf
taglib/flac
audio/x-flac
application/x-flac
audio/flac
taglib/mp3
audio/x-mp3
application/x-id3
audio/mpeg
audio/x-mpeg
audio/x-mpeg-3
audio/mpeg3
audio/mp3
taglib/m2a
taglib/mp2
taglib/mp1
audio/x-mp2
audio/x-mp1
taglib/mpg
taglib/mpeg
taglib/mpe
taglib/mpv2
taglib/m2v
video/x-mpg
video/mpeg
taglib/m4a
taglib/m4b
taglib/m4v
taglib/m4p
taglib/mp4
audio/mp4
audio/x-m4a
video/mp4
video/x-m4v
taglib/mpc
taglib/mp+
taglib/mpp
audio/x-musepack
taglib/ogg
taglib/oga
taglib/ogv
application/ogg
application/x-ogg
audio/vorbis
audio/x-vorbis
audio/x-vorbis+ogg
audio/ogg
audio/x-ogg
video/ogg
video/x-ogm+ogg
video/x-theora+ogg
video/x-theora
taglib/avi
taglib/wav
taglib/divx
video/avi
video/msvideo
video/x-msvideo
image/avi
application/x-troff-msvideo
audio/avi
audio/wav
audio/wave
audio/x-wav
taglib/wv
audio/x-wavpack

 */
