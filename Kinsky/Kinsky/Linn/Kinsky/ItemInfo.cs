using System.Collections.Generic;
using Upnp;
using System.Collections.ObjectModel;
using System;
using Linn.Topology;

namespace Linn.Kinsky
{

    public static class TechnicalInfoHelper
    {
        public static string FormatTechnicalInfo(string aCodec, uint aBitrate, uint aBitDepth, float aSampleRate, bool aLossless)
        {
            string bitrate = aBitrate != 0 ? string.Format("{0} kbps", aBitrate) : string.Empty;
            string sampleRateBitDepth = aSampleRate != 0 ? string.Format("{0} kHz", aSampleRate) : string.Empty;
            if (aLossless && aBitDepth != 0)
            {
                string bitdepth = string.Format("{0} bits", aBitDepth);
                sampleRateBitDepth += string.Format(sampleRateBitDepth == string.Empty ? "{0}" : " / {0}", bitdepth);
            }
            return string.Format("{0} {1} {2}", aCodec, sampleRateBitDepth, bitrate).Trim();
        }
    }

    public class ItemInfo
    {

        public ItemInfo(upnpObject aItem) : this(aItem, null) { }
        public ItemInfo(upnpObject aItem, upnpObject aParent)
        {
            iItem = aItem;
            iParent = aParent;
            iParsed = false;
            iLock = new object();
        }


        private string GetItemType(upnpObject aItem)
        {
            //todo:
            return aItem.Class;
        }

        public ReadOnlyCollection<KeyValuePair<string, string>> AllItems
        {
            get
            {
                Parse();
                return iFullInfoList.AsReadOnly();
            }
        }

        public ReadOnlyCollection<KeyValuePair<string, string>> DisplayItems
        {
            get
            {
                Parse();
                return iDisplayInfoList.AsReadOnly();
            }
        }

        public KeyValuePair<string, string>? DisplayItem(int aIndex)
        {
            Parse();
            if (aIndex < iDisplayInfoList.Count)
            {
                return iDisplayInfoList[aIndex];
            }
            return null;
        }

        private void Parse()
        {
            lock (iLock)
            {
                if (!iParsed)
                {
                    List<KeyValuePair<string, string>> displayList = new List<KeyValuePair<string, string>>();
                    List<KeyValuePair<string, string>> fullList = new List<KeyValuePair<string, string>>();

                    if (iItem != null)
                    {
                        string type = GetItemType(iItem);
                        fullList.Add(new KeyValuePair<string, string>("Type", type));

                        string title = DidlLiteAdapter.Title(iItem);
                        if (title != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Title", title));
                            fullList.Add(new KeyValuePair<string, string>("Title", title));
                        }

                        string album = DidlLiteAdapter.Album(iItem);
                        // if parent is a musicAlbum and album is blank, try to get album info from parent
                        if (album == string.Empty && iParent is musicAlbum)
                        {
                            album = DidlLiteAdapter.Album(iParent);
                        }
                        // don't display album field if we are a musicAlbum as it will be same as Title
                        if (album != string.Empty && !(iItem is musicAlbum))
                        {
                            // only insert album info for display if parent not a musicAlbum
                            if (!(iParent is musicAlbum))
                            {
                                displayList.Add(new KeyValuePair<string, string>("Album", album));
                            }
                            fullList.Add(new KeyValuePair<string, string>("Album", album));
                        }

                        string artist = DidlLiteAdapter.Artist(iItem);
                        string albumArtist = DidlLiteAdapter.AlbumArtist(iItem);
                        string parentArtist = string.Empty;
                        if (iParent != null)
                        {
                            parentArtist = DidlLiteAdapter.Artist(iParent);
                        }

                        // don't display artist field if we are a person, as it will be same as title
                        if (artist != string.Empty && !(iItem is person))
                        {
                            // only display artist field when:
                            // our parent is not a music album 
                            // or we are a music album and album artist field is blank
                            // our parent is a music album but artist field is different from album artist field and different from parent's artist field

                            if (!(iParent is musicAlbum) ||
                                (iItem is musicAlbum && albumArtist == string.Empty) ||
                                (iParent is musicAlbum && albumArtist != artist && parentArtist != artist))
                            {
                                displayList.Add(new KeyValuePair<string, string>("Artist", artist));
                            }
                            fullList.Add(new KeyValuePair<string, string>("Artist", artist));
                        }

                        if (albumArtist != string.Empty)
                        {
                            // only display albumartist field when:
                            // we are a musicAlbum and album artist is different from artist
                            // or artist field is blank and album artist is not the same as parent's artist field

                            if ((iItem is musicAlbum && albumArtist != artist) ||
                                (artist == string.Empty && parentArtist != albumArtist))
                            {
                                displayList.Add(new KeyValuePair<string, string>("Album Artist", albumArtist));
                            }
                            fullList.Add(new KeyValuePair<string, string>("Album Artist", albumArtist));
                        }

                        string count = DidlLiteAdapter.Count(iItem);
                        if (count != string.Empty && count != "0")
                        {
                            displayList.Add(new KeyValuePair<string, string>("Count", count));
                            fullList.Add(new KeyValuePair<string, string>("Count", count));
                        }

                        string composer = DidlLiteAdapter.Composer(iItem);
                        if (composer != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Composer", composer));
                            fullList.Add(new KeyValuePair<string, string>("Composer", composer));
                        }

                        string genre = DidlLiteAdapter.Genre(iItem);
                        if (genre != string.Empty)
                        {
                            string parentGenre = string.Empty;
                            if (iParent != null)
                            {
                                parentGenre = DidlLiteAdapter.Genre(iParent);
                            }
                            if (parentGenre != genre)
                            {
                                displayList.Add(new KeyValuePair<string, string>("Genre", genre));
                            }
                            fullList.Add(new KeyValuePair<string, string>("Genre", genre));
                        }

                        string releaseYear = DidlLiteAdapter.ReleaseYear(iItem);
                        if (releaseYear != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Release Year", releaseYear));
                            fullList.Add(new KeyValuePair<string, string>("Release Year", releaseYear));
                        }

                        string originalTrackNumber = DidlLiteAdapter.OriginalTrackNumber(iItem);
                        if (originalTrackNumber != string.Empty)
                        {
                            fullList.Add(new KeyValuePair<string, string>("Original Track No.", originalTrackNumber));
                        }

                        string conductor = DidlLiteAdapter.Conductor(iItem);
                        if (conductor != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Conductor", conductor));
                            fullList.Add(new KeyValuePair<string, string>("Conductor", conductor));
                        }

                        string actor = DidlLiteAdapter.Actor(iItem);
                        if (actor != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Actor", actor));
                            fullList.Add(new KeyValuePair<string, string>("Actor", actor));
                        }

                        string director = DidlLiteAdapter.Director(iItem);
                        if (director != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Director", director));
                            fullList.Add(new KeyValuePair<string, string>("Director", director));
                        }

                        string publisher = DidlLiteAdapter.Publisher(iItem);
                        if (publisher != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Publisher", publisher));
                            fullList.Add(new KeyValuePair<string, string>("Publisher", publisher));
                        }

                        string contributer = DidlLiteAdapter.Contributor(iItem);
                        if (contributer != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Contributor", contributer));
                            fullList.Add(new KeyValuePair<string, string>("Contributor", contributer));
                        }

                        string duration = DidlLiteAdapter.Duration(iItem);
                        if (duration != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Duration", duration));
                            fullList.Add(new KeyValuePair<string, string>("Duration", duration));
                        }

                        string size = DidlLiteAdapter.Size(iItem);
                        if (size != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Size", size));
                            fullList.Add(new KeyValuePair<string, string>("Size", size));
                        }

                        string bitrate = DidlLiteAdapter.Bitrate(iItem);
                        if (bitrate != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Bitrate", bitrate));
                            fullList.Add(new KeyValuePair<string, string>("Bitrate", bitrate));
                        }

                        string sampleRate = DidlLiteAdapter.SampleRate(iItem);
                        if (sampleRate != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("SampleRate", sampleRate));
                            fullList.Add(new KeyValuePair<string, string>("SampleRate", sampleRate));
                        }

                        string bitDepth = DidlLiteAdapter.BitDepth(iItem);
                        if (bitDepth != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Bit Depth", bitDepth));
                            fullList.Add(new KeyValuePair<string, string>("Bit Depth", bitDepth));
                        }

                        string mimeType = DidlLiteAdapter.MimeType(iItem);
                        if (mimeType != string.Empty)
                        {
                            fullList.Add(new KeyValuePair<string, string>("Mime Type", mimeType));
                        }

                        string protocolInfo = DidlLiteAdapter.ProtocolInfo(iItem);
                        if (protocolInfo != string.Empty)
                        {
                            fullList.Add(new KeyValuePair<string, string>("Protocol Info", protocolInfo));
                        }

                        string description = DidlLiteAdapter.Description(iItem);
                        if (description != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Description", description));
                            fullList.Add(new KeyValuePair<string, string>("Description", description));
                        }

                        string info = DidlLiteAdapter.Info(iItem);
                        if (info != string.Empty)
                        {
                            displayList.Add(new KeyValuePair<string, string>("Info", description));
                            fullList.Add(new KeyValuePair<string, string>("Info", description));
                        }

                        string uri = DidlLiteAdapter.Uri(iItem);
                        if (uri != string.Empty)
                        {
                            fullList.Add(new KeyValuePair<string, string>("Uri", uri));
                        }

                        Uri artworkUri = DidlLiteAdapter.ArtworkUri(iItem);
                        if (artworkUri != null)
                        {
                            fullList.Add(new KeyValuePair<string, string>("Artwork Uri", artworkUri.OriginalString));
                        }
                    }
                    iFullInfoList = fullList;
                    iDisplayInfoList = displayList;
                    iParsed = true;
                }
            }
        }

        private List<KeyValuePair<string, string>> iFullInfoList;
        private List<KeyValuePair<string, string>> iDisplayInfoList;
        private bool iParsed;
        private object iLock;
        private upnpObject iItem;
        private upnpObject iParent;

    }
}
