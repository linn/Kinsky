using System;
using System.Collections;

namespace Upnp
{
    public class DidlLiteAdapter
    {
        private static string NonNullString(string aString)
        {
            return (aString != null ? aString : string.Empty);
        }

        public static string Title(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && !string.IsNullOrEmpty(aObject.Title))
            {
                result = aObject.Title.Trim();
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Album(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is musicAlbum)
            {
                musicAlbum album = aObject as musicAlbum;
                result = album.Title;
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                if (item.Album.Count > 0)
                {
                    result = item.Album[0];
                }
            }
            else if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                if (item.Album.Count > 0)
                {
                    result = item.Album[0];
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string AlbumArtist(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is person)
            {
                person person = aObject as person;
                result = person.Title;
            }
            else if (aObject is musicAlbum)
            {
                // Album Artist for a musicAlbum object is, in order of preference,
                // - the <artist role="albumArtist"> value
                // - the <artist> value
                // - the <creator> value
                musicAlbum album = aObject as musicAlbum;

                string roleAlbumArtist = null;
                string roleNull = null;
                foreach (artist a in album.Artist)
                {
                    if (string.IsNullOrEmpty(a.Role))
                    {
                        roleNull = a.Artist;
                    }
                    else if (a.Role.ToLower() == "albumartist")
                    {
                        roleAlbumArtist = a.Artist;
                    }
                }

                if (!string.IsNullOrEmpty(roleAlbumArtist))
                {
                    result = roleAlbumArtist;
                }
                else if (!string.IsNullOrEmpty(roleNull))
                {
                    result = roleNull;
                }
                else
                {
                    result = album.Creator;
                }
            }
            else if (aObject is musicTrack)
            {
                // Album Artist for a track is unambiguous - it is the <artist role="albumArtist"> value or nothing
                musicTrack item = aObject as musicTrack;
                foreach (artist a in item.Artist)
                {
                    if (a.Role != null && a.Role.ToLower() == "albumartist")
                    {
                        result = a.Artist;
                        break;
                    }
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                foreach (artist a in item.Artist)
                {
                    if (a.Role != null && a.Role.ToLower() == "albumartist")
                    {
                        result = a.Artist;
                        break;
                    }
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Artist(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is person)
            {
                person person = aObject as person;
                result = person.Title;
            }
            else if (aObject is musicAlbum)
            {
                // Artist and ALbum Artist are the same for musicAlbums
                return AlbumArtist(aObject);
            }
            else if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                foreach (artist a in item.Artist)
                {
                    if (a.Role == null || (a.Role != null && a.Role.ToLower() == "performer"))
                    {
                        result = a.Artist;
                        if (a.Role != null)
                        {
                            break;
                        }
                    }
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                foreach (artist a in item.Artist)
                {
                    if (a.Role == null || (a.Role != null && a.Role.ToLower() == "performer"))
                    {
                        result = a.Artist;
                        if (a.Role != null)
                        {
                            break;
                        }
                    }
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Actor(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is videoItem)
            {
                videoItem item = aObject as videoItem;
                for (int i = 0; i < item.Actor.Count; ++i)
                {
                    if (i > 0 && i < item.Actor.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Actor[i].Actor;
                }
            }
            else if (aObject is videoBroadcast)
            {
                videoBroadcast item = aObject as videoBroadcast;
                for (int i = 0; i < item.Actor.Count; ++i)
                {
                    if (i > 0 && i < item.Actor.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Actor[i].Actor;
                }
            }
            else if (aObject is movie)
            {
                movie item = aObject as movie;
                for (int i = 0; i < item.Actor.Count; ++i)
                {
                    if (i > 0 && i < item.Actor.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Actor[i].Actor;
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                for (int i = 0; i < item.Actor.Count; ++i)
                {
                    if (i > 0 && i < item.Actor.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Actor[i].Actor;
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Director(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is videoItem)
            {
                videoItem item = aObject as videoItem;
                for (int i = 0; i < item.Director.Count; ++i)
                {
                    if (i > 0 && i < item.Director.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Director[i];
                }
            }
            else if (aObject is videoBroadcast)
            {
                videoBroadcast item = aObject as videoBroadcast;
                for (int i = 0; i < item.Director.Count; ++i)
                {
                    if (i > 0 && i < item.Director.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Director[i];
                }
            }
            else if (aObject is movie)
            {
                movie item = aObject as movie;
                for (int i = 0; i < item.Director.Count; ++i)
                {
                    if (i > 0 && i < item.Director.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Director[i];
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                for (int i = 0; i < item.Director.Count; ++i)
                {
                    if (i > 0 && i < item.Director.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Director[i];
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Publisher(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is album)
            {
                album item = aObject as album;
                for (int i = 0; i < item.Publisher.Count; ++i)
                {
                    if (i > 0 && i < item.Publisher.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Publisher[i];
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                result = item.Publisher;
            }
            else if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                for (int i = 0; i < item.Publisher.Count; ++i)
                {
                    if (i > 0 && i < item.Publisher.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Publisher[i];
                }
            }
            else if (aObject is audioBook)
            {
                audioBook item = aObject as audioBook;
                for (int i = 0; i < item.Publisher.Count; ++i)
                {
                    if (i > 0 && i < item.Publisher.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Publisher[i];
                }
            }
            else if (aObject is videoItem)
            {
                videoItem item = aObject as videoItem;
                result = item.Publisher;
            }
            else if (aObject is videoBroadcast)
            {
                videoBroadcast item = aObject as videoBroadcast;
                result = item.Publisher;
            }
            else if (aObject is movie)
            {
                movie item = aObject as movie;
                result = item.Publisher;
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Contributor(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is album)
            {
                album album = aObject as album;
                if (album.Contributor.Count > 0)
                {
                    result = album.Contributor[0];
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                if (item.Contributor.Count > 0)
                {
                    result = item.Contributor[0];
                }
            }
            else if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                if (item.Contributor.Count > 0)
                {
                    result = item.Contributor[0];
                }
            }
            else if (aObject is audioBook)
            {
                audioBook item = aObject as audioBook;
                if (item.Contributor.Count > 0)
                {
                    result = item.Contributor[0];
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Genre(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is genre)
            {
                genre genre = aObject as genre;
                result = genre.Title;
            }
            else if (aObject is musicArtist)
            {
                musicArtist artist = aObject as musicArtist;
                for (int i = 0; i < artist.Genre.Count; ++i)
                {
                    if (i > 0 && i < artist.Genre.Count - 1)
                    {
                        result += ", ";
                    }
                    result += artist.Genre[i];
                }
            }
            else if (aObject is musicAlbum)
            {
                musicAlbum album = aObject as musicAlbum;
                for (int i = 0; i < album.Genre.Count; ++i)
                {
                    if (i > 0 && i < album.Genre.Count - 1)
                    {
                        result += ", ";
                    }
                    result += album.Genre[i];
                }
            }
            else if (aObject is audioItem)
            {
                audioItem item = aObject as audioItem;
                for (int i = 0; i < item.Genre.Count; ++i)
                {
                    if (i > 0 && i < item.Genre.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Genre[i];
                }
            }
            else if (aObject is videoItem)
            {
                videoItem item = aObject as videoItem;
                for (int i = 0; i < item.Genre.Count; ++i)
                {
                    if (i > 0 && i < item.Genre.Count - 1)
                    {
                        result += ", ";
                    }
                    result += item.Genre[i];
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string ReleaseYear(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is playlistContainer)
            {
                playlistContainer container = aObject as playlistContainer;
                result = container.Date;
            }
            else if (aObject is album)
            {
                album album = aObject as album;
                result = album.Date;
            }
            else if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                result = item.Date;
            }
            else if (aObject is audioBook)
            {
                audioBook item = aObject as audioBook;
                result = item.Date;
            }
            if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                result = item.Date;
            }

            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    DateTime t = DateTime.Parse(result);
                    result = t.Year.ToString().Trim();
                }
                catch (FormatException) { }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Composer(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                foreach (artist a in item.Artist)
                {
                    if (a.Role != null && a.Role.ToLower() == "composer")
                    {
                        result = a.Artist;
                    }
                }
                if (result == string.Empty)
                {
                    foreach (author a in item.Author)
                    {
                        if (a.Role != null && a.Role.ToLower() == "composer")
                        {
                            result = a.Author;
                        }
                    }
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                foreach (artist a in item.Artist)
                {
                    if (a.Role != null && a.Role.ToLower() == "composer")
                    {
                        result = a.Artist;
                    }
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string ProtocolInfo(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                if (aObject.Res[0].ProtocolInfo != null)
                {
                    result = aObject.Res[0].ProtocolInfo;
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string MimeType(upnpObject aObject)
        {
            string result = string.Empty;
            string protocolInfo = ProtocolInfo(aObject);

            if (protocolInfo != string.Empty)
            {
                string[] split = protocolInfo.Split(':');
                if (split.Length == 4)
                {
                    result = split[2];
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Bitrate(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                float bitrate = (aObject.Res[0].Bitrate * 8) / 1000.0f;
                if (bitrate > 0)
                {
                    string bitrateString = bitrate.ToString() + " kbps";
                    result = bitrateString;
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Duration(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                if (aObject.Res[0].Duration != null)
                {
                    try
                    {
                        Time time = new Time(aObject.Res[0].Duration);
                        result = time.ToPrettyString();
                    }
                    catch (Time.TimeInvalid) { }
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Size(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                long size = aObject.Res[0].Size / 1024 / 1024;
                if (size > 0)
                {
                    result = size.ToString() + " MB";
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Uri(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                result = aObject.Res[0].Uri;
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Conductor(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is musicTrack)
            {
                musicTrack item = aObject as musicTrack;
                foreach (artist a in item.Artist)
                {
                    if (a.Role != null && a.Role.ToLower() == "conductor")
                    {
                        result = a.Artist;
                    }
                }
            }
            else if (aObject is musicVideoClip)
            {
                musicVideoClip item = aObject as musicVideoClip;
                foreach (artist a in item.Artist)
                {
                    if (a.Role != null && a.Role.ToLower() == "conductor")
                    {
                        result = a.Artist;
                    }
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Count(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is container)
            {
                container container = aObject as container;
                result = container.ChildCount.ToString();
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static Uri ArtworkUri(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null)
            {
                if (aObject.ArtworkUri.Count > 0)
                {
                    result = aObject.ArtworkUri[0];
                }
                else if (aObject.Icon != null)
                {
                    result = aObject.Icon;
                }
                else if (aObject.AlbumArtUri.Count > 0)
                {
                    result = aObject.AlbumArtUri[0];
                }
                else if (aObject is photo)
                {
                    if (aObject.Res.Count > 0)
                    {
                        if (aObject.Res[0].Uri != null)
                        {
                            result = aObject.Res[0].Uri;
                        }
                    }
                }
            }

            if (result.Length > 0)
            {
                try
                {
                    Uri uri = new Uri(result);
                    return (uri);
                }
                catch (UriFormatException)
                {
                }
            }

            return null;
        }

        public static string OriginalTrackNumber(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is musicTrack)
            {
                musicTrack track = aObject as musicTrack;
                if(track.OriginalTrackNumber > 0)
                {
                    result = track.OriginalTrackNumber.ToString();
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string SampleRate(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                float sampleRate = aObject.Res[0].SampleFrequency / 1000.0f;
                if (sampleRate > 0)
                {
                    string sampleRateString = sampleRate.ToString() + " kHz";
                    result = sampleRateString;
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string BitDepth(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject != null && aObject.Res.Count > 0)
            {
                int bitDepth = aObject.Res[0].BitsPerSample;
                if (bitDepth > 0)
                {
                    result = bitDepth.ToString() + " bits";
                }
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Description(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is audioItem)
            {
                audioItem item = aObject as audioItem;
                result = item.Description;
            }
            else if (aObject is videoItem)
            {
                videoItem item = aObject as videoItem;
                result = item.Description;
            }
            else if (aObject is album)
            {
                album item = aObject as album;
                result = item.Description;
            }
            else if (aObject is playlistContainer)
            {
                playlistContainer item = aObject as playlistContainer;
                result = item.Description;
            }

            // always return a non-null string
            return NonNullString(result);
        }

        public static string Info(upnpObject aObject)
        {
            string result = string.Empty;

            if (aObject is audioItem)
            {
                audioItem item = aObject as audioItem;
                result = item.LongDescription;
            }
            else if (aObject is videoItem)
            {
                videoItem item = aObject as videoItem;
                result = item.LongDescription;
            }
            else if (aObject is album)
            {
                album item = aObject as album;
                result = item.LongDescription;
            }

            // always return a non-null string
            return NonNullString(result);
        }
    }
} // Upnp
