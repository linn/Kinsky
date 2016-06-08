
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Threading;

using Linn;


namespace OssKinskyMppItunes
{
    public class LibraryHeader
    {
        public long MajorVersion = -1;
        public long MinorVersion = -1;
        public string AppVersion = null;
        public long Features = -1;
        public bool ShowContentRatings = false;
        public string MusicFolder = null;
        public string LibraryId = null;
    }

    public class LibraryItem
    {
        public long TrackId = -1;
        public string Name = null;
        public string Artist = null;
        public string AlbumArtist = null;
        public string Composer = null;
        public string Album = null;
        public string Genre = null;
        public string Kind = null;
        public long Size = -1;
        public long TotalTime = -1;
        public long DiscNumber = -1;
        public long DiscCount = -1;
        public long TrackNumber = -1;
        public long TrackCount = -1;
        public long Year = -1;
        public DateTime DateModified;
        public DateTime DateAdded;
        public long BitRate = -1;
        public long SampleRate = -1;
        public string PersistentId = null;
        public string TrackType = null;
        public string Location = null;
        public string AlbumArtId = null;
        public string AlbumArtExt = null;
    }

    public class LibraryPlaylist
    {
        public string Name = null;
        public long Id = -1;
        public string PersistentId = null;
        public bool IsVisible = true;
        public bool IsSmart = false;
        public long DistinguishedKind = -1;
        public List<long> Items = new List<long>();
    }

    public class Library
    {
        public Library(string aXmlFile)
        {
            iXmlFile = aXmlFile;
            iItems = new Dictionary<long, LibraryItem>();
            iMutex = new Mutex();

            LibraryXmlReaderHeader reader = new LibraryXmlReaderHeader();
            iHeader = reader.Read(iXmlFile);

            iAlbumArtPath = Path.Combine(Path.GetDirectoryName(iXmlFile), "Album Artwork");
            iAlbumArtPath = Path.Combine(iAlbumArtPath, "Cache");
            iAlbumArtPath = Path.Combine(iAlbumArtPath, iHeader.LibraryId);
        }

        public string XmlFile
        {
            get { return iXmlFile; }
        }

        public LibraryHeader Header
        {
            get { return iHeader; }
        }

        public LibraryItem GetItem(long aId)
        {
            try
            {
                iMutex.WaitOne();
                return iItems[aId];
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void AddItem(LibraryItem aItem)
        {
            try
            {
                iMutex.WaitOne();
                iItems.Add(aItem.TrackId, aItem);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public string AlbumArtRoot
        {
            get { return iAlbumArtPath; }
        }

        public string GetAlbumArtFilenameNoExt(string aId)
        {
            string filename = string.Empty;

            uint[] d = GetAlbumArtFilenameDigits(aId);

            if (d != null)
            {

                // leave off the file extension - this can be "itc" or "itc2"
                filename = iAlbumArtPath + Path.DirectorySeparatorChar
                         + d[0].ToString("00") + Path.DirectorySeparatorChar
                         + d[1].ToString("00") + Path.DirectorySeparatorChar
                         + d[2].ToString("00") + Path.DirectorySeparatorChar
                         + iHeader.LibraryId + "-" + aId;
            }

            return filename;
        }

        public uint[] GetAlbumArtFilenameDigits(string aId)
        {
            if (aId.Length > 3)
            {
                string last3Chars = aId.Substring(aId.Length - 3);
                uint digit1 = Convert.ToUInt32(last3Chars.Substring(0, 1), 16);
                uint digit2 = Convert.ToUInt32(last3Chars.Substring(1, 1), 16);
                uint digit3 = Convert.ToUInt32(last3Chars.Substring(2, 1), 16);

                // digits are returned in reverse order
                return new uint[] { digit3, digit2, digit1 };
            }

            return null;
        }

        private string iXmlFile;
        private LibraryHeader iHeader;
        private Dictionary<long, LibraryItem> iItems;
        private Mutex iMutex;
        private string iAlbumArtPath;
    }


    public class LibraryXmlReader
    {
        public class Error : Exception
        {
        }

        protected XmlReader OpenFile(string aFilename)
        {
            FileStream stream = null;
            XmlReader reader = null;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CloseInput = true;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;
                // the iTunes XML file contains a DTD declaration - the first of the next 2 lines means that the presence
                // of the DTD will not cause the parser to throw. The second line is to prevent the DTD being downloaded
                // from the apple website - the iTunes plugin should not require an internet connection to work
                settings.ProhibitDtd = false;
                settings.XmlResolver = null;

                stream = new FileStream(aFilename, FileMode.Open, FileAccess.Read);
                reader = XmlReader.Create(stream, settings);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "dict")
                        break;
                }

                if (reader.EOF)
                {
                    UserLog.WriteLine(DateTime.Now + " : No top level dict in Itunes XML file: " + aFilename);
                    reader.Close();
                    throw new Error();
                }
            }
            catch (Exception)
            {
                if (stream != null)
                    stream.Close();
                if (reader != null)
                    reader.Close();
                throw;
            }

            return reader;
        }


        protected void ReadToKey(XmlDictReader aReader, string aKey)
        {
            while (!aReader.EndOfDict)
            {
                string key = aReader.ReadKey();
                if (key == aKey)
                {
                    return;
                }
                else
                {
                    aReader.SkipValue();
                }
            }
        }
    }


    public class LibraryXmlReaderHeader : LibraryXmlReader
    {
        public LibraryHeader Read(string aFilename)
        {
            LibraryHeader header = new LibraryHeader();
            XmlReader reader = OpenFile(aFilename);

            try
            {
                XmlDictReader dictReader = new XmlDictReader(reader);
                while (!dictReader.EndOfDict)
                {
                    string key = dictReader.ReadKey();

                    if (key == "Major Version")
                    {
                        header.MajorVersion = dictReader.ReadValueLong();
                    }
                    else if (key == "Minor Version")
                    {
                        header.MinorVersion = dictReader.ReadValueLong();
                    }
                    else if (key == "Application Version")
                    {
                        header.AppVersion = dictReader.ReadValueString();
                    }
                    else if (key == "Features")
                    {
                        header.Features = dictReader.ReadValueLong();
                    }
                    else if (key == "Show Content Ratings")
                    {
                        header.ShowContentRatings = dictReader.ReadValueBool();
                    }
                    else if (key == "Music Folder")
                    {
                        header.MusicFolder = dictReader.ReadValueString();
                    }
                    else if (key == "Library Persistent ID")
                    {
                        header.LibraryId = dictReader.ReadValueString();
                    }
                    else
                    {
                        dictReader.SkipValue();
                    }
                }
            }
            finally 
            {
                reader.Close();
            }

            return header;
        }
    }


    public class LibraryXmlReaderItems : LibraryXmlReader
    {
        public LibraryXmlReaderItems(string aFilename)
        {
            iReader = OpenFile(aFilename);
            try
            {
                XmlDictReader rootReader = new XmlDictReader(iReader);
                ReadToKey(rootReader, "Tracks");
                if (!rootReader.EndOfDict)
                {
                    iTrackListReader = rootReader.ReadValueDictBegin();
                }
            }
            catch (Exception)
            {
                iReader.Close();
                throw;
            }
        }

        public bool End
        {
            get { return (iTrackListReader == null || iTrackListReader.EndOfDict); }
        }

        public LibraryItem Read()
        {
            Assert.Check(!End);

            LibraryItem item;
            try
            {
                iTrackListReader.ReadKey();

                XmlDictReader trackReader = iTrackListReader.ReadValueDictBegin();
                item = ReadItem(trackReader);
                iTrackListReader.ReadValueDictEnd(trackReader);

                if (End)
                {
                    iReader.Close();
                }
            }
            catch (Exception)
            {
                iReader.Close();
                throw;
            }

            return item;
        }

        private LibraryItem ReadItem(XmlDictReader aReader)
        {
            LibraryItem track = new LibraryItem();

            while (!aReader.EndOfDict)
            {
                string propKey = aReader.ReadKey();

                if (propKey == "Track ID")
                {
                    track.TrackId = aReader.ReadValueLong();
                }
                else if (propKey == "Name")
                {
                    track.Name = aReader.ReadValueString();
                }
                else if (propKey == "Artist")
                {
                    track.Artist = aReader.ReadValueString();
                }
                else if (propKey == "Album Artist")
                {
                    track.AlbumArtist = aReader.ReadValueString();
                }
                else if (propKey == "Composer")
                {
                    track.Composer = aReader.ReadValueString();
                }
                else if (propKey == "Album")
                {
                    track.Album = aReader.ReadValueString();
                }
                else if (propKey == "Genre")
                {
                    track.Genre = aReader.ReadValueString();
                }
                else if (propKey == "Kind")
                {
                    track.Kind = aReader.ReadValueString();
                }
                else if (propKey == "Size")
                {
                    track.Size = aReader.ReadValueLong();
                }
                else if (propKey == "Total Time")
                {
                    track.TotalTime = aReader.ReadValueLong();
                }
                else if (propKey == "Disc Number")
                {
                    track.DiscNumber = aReader.ReadValueLong();
                }
                else if (propKey == "Disc Count")
                {
                    track.DiscCount = aReader.ReadValueLong();
                }
                else if (propKey == "Track Number")
                {
                    track.TrackNumber = aReader.ReadValueLong();
                }
                else if (propKey == "Track Count")
                {
                    track.TrackCount = aReader.ReadValueLong();
                }
                else if (propKey == "Year")
                {
                    track.Year = aReader.ReadValueLong();
                }
                else if (propKey == "Date Modified")
                {
                    track.DateModified = aReader.ReadValueDate();
                }
                else if (propKey == "Date Added")
                {
                    track.DateAdded = aReader.ReadValueDate();
                }
                else if (propKey == "Bit Rate")
                {
                    track.BitRate = aReader.ReadValueLong();
                }
                else if (propKey == "Sample Rate")
                {
                    track.SampleRate = aReader.ReadValueLong();
                }
                else if (propKey == "Persistent ID")
                {
                    track.PersistentId = aReader.ReadValueString();
                }
                else if (propKey == "Track Type")
                {
                    track.TrackType = aReader.ReadValueString();
                }
                else if (propKey == "Location")
                {
                    track.Location = aReader.ReadValueString();
                }
                else
                {
                    aReader.SkipValue();
                }
            }

            return track;
        }

        private XmlReader iReader;
        private XmlDictReader iTrackListReader;
    }


    public class LibraryXmlReaderPlaylists : LibraryXmlReader
    {
        public LibraryXmlReaderPlaylists(string aFilename)
        {
            iReader = OpenFile(aFilename);
            try
            {
                XmlDictReader rootReader = new XmlDictReader(iReader);
                ReadToKey(rootReader, "Playlists");
                if (!rootReader.EndOfDict)
                {
                    iPlaylistListReader = rootReader.ReadValueArrayBegin();
                }
            }
            catch (Exception)
            {
                iReader.Close();
                throw;
            }
        }

        public bool End
        {
            get { return (iPlaylistListReader == null || iPlaylistListReader.EndOfArray); }
        }

        public LibraryPlaylist Read()
        {
            Assert.Check(!End);

            LibraryPlaylist playlist;
            try
            {
                XmlDictReader playlistReader = iPlaylistListReader.ReadValueDictBegin();
                playlist = ReadPlaylist(playlistReader);
                iPlaylistListReader.ReadValueDictEnd(playlistReader);

                if (End)
                {
                    iReader.Close();
                }
            }
            catch (Exception)
            {
                iReader.Close();
                throw;
            }

            return playlist;
        }

        private LibraryPlaylist ReadPlaylist(XmlDictReader aReader)
        {
            LibraryPlaylist playlist = new LibraryPlaylist();

            while (!aReader.EndOfDict)
            {
                string propKey = aReader.ReadKey();

                if (propKey == "Name")
                {
                    playlist.Name = aReader.ReadValueString();
                }
                else if (propKey == "Playlist ID")
                {
                    playlist.Id = aReader.ReadValueLong();
                }
                else if (propKey == "Playlist Persistent ID")
                {
                    playlist.PersistentId = aReader.ReadValueString();
                }
                else if (propKey == "Visible")
                {
                    playlist.IsVisible = aReader.ReadValueBool();
                }
                else if (propKey == "Distinguished Kind")
                {
                    playlist.DistinguishedKind = aReader.ReadValueLong();
                }
                else if (propKey == "Smart Info" ||
                         propKey == "Smart Criteria")
                {
                    playlist.IsSmart = true;
                    aReader.SkipValue();
                }
                else if (propKey == "Playlist Items")
                {
                    XmlArrayReader itemListReader = aReader.ReadValueArrayBegin();
                    while (!itemListReader.EndOfArray)
                    {
                        XmlDictReader itemReader = itemListReader.ReadValueDictBegin();
                        itemReader.ReadKey();
                        playlist.Items.Add(itemReader.ReadValueLong());
                        itemListReader.ReadValueDictEnd(itemReader);
                    }
                    aReader.ReadValueArrayEnd(itemListReader);
                }
                else
                {
                    aReader.SkipValue();
                }
            }

            return playlist;
        }

        private XmlReader iReader;
        private XmlArrayReader iPlaylistListReader;
    }


    public class XmlArrayReader
    {
        public class Error : Exception
        {
        }

        public XmlArrayReader(XmlReader aReader)
        {
            Assert.Check(aReader.Name == "array");
            iReader = aReader;
            iEndOfArray = false;
            iCurrentDictReader = null;
            iReader.Read();
            SkipToNextElement();
        }

        public bool EndOfArray
        {
            get { return iEndOfArray; }
        }

        public XmlDictReader ReadValueDictBegin()
        {
            Assert.Check(!EndOfArray && iCurrentDictReader == null);
            if (iReader.Name != "dict")
                throw new Error();

            iCurrentDictReader = new XmlDictReader(iReader);
            return iCurrentDictReader;
        }

        public void ReadValueDictEnd(XmlDictReader aReader)
        {
            Assert.Check(!EndOfArray && iCurrentDictReader != null);
            Assert.Check(aReader.EndOfDict);
            Assert.Check(aReader == iCurrentDictReader);
            iCurrentDictReader = null;
            SkipToNextElement();
        }

        void SkipToNextElement()
        {
            do
            {
                if (iReader.NodeType == XmlNodeType.EndElement && iReader.Name == "array")
                {
                    iEndOfArray = true;
                    iReader.Read();
                    return;
                }

                if (iReader.NodeType == XmlNodeType.Element)
                    return;
            }
            while (iReader.Read());

            throw new Error();
        }

        private XmlReader iReader;
        private bool iEndOfArray;
        private XmlDictReader iCurrentDictReader;
    }


    public class XmlDictReader
    {
        public class Error : Exception
        {
        }

        public XmlDictReader(XmlReader aReader)
        {
            Assert.Check(aReader.Name == "dict");
            iReader = aReader;
            iEndOfDict = false;
            iCurrentDictReader = null;
            iCurrentArrayReader = null;
            SkipToNextKey();
        }

        public bool EndOfDict
        {
            get { return iEndOfDict; }
        }

        public string ReadKey()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            Assert.Check(iReader.Name == "key");
            return iReader.ReadElementContentAsString();
        }

        public void SkipValue()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            iReader.Skip();
            SkipToNextKey();
        }

        public XmlDictReader ReadValueDictBegin()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "dict")
                throw new Error();

            iCurrentDictReader = new XmlDictReader(iReader);
            return iCurrentDictReader;
        }

        public void ReadValueDictEnd(XmlDictReader aReader)
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader != null && iCurrentArrayReader == null);
            Assert.Check(aReader.EndOfDict);
            Assert.Check(aReader == iCurrentDictReader);
            iCurrentDictReader = null;
            SkipToNextKey();
        }

        public XmlArrayReader ReadValueArrayBegin()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "array")
                throw new Error();

            iCurrentArrayReader = new XmlArrayReader(iReader);
            return iCurrentArrayReader;
        }

        public void ReadValueArrayEnd(XmlArrayReader aReader)
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader != null);
            Assert.Check(aReader.EndOfArray);
            Assert.Check(aReader == iCurrentArrayReader);
            iCurrentArrayReader = null;
            SkipToNextKey();
        }

        public DateTime ReadValueDate()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "date")
                throw new Error();

            DateTime value = iReader.ReadElementContentAsDateTime();
            SkipToNextKey();
            return value;
        }

        public double ReadValueDouble()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "real")
                throw new Error();

            double value = iReader.ReadElementContentAsDouble();
            SkipToNextKey();
            return value;
        }

        public long ReadValueLong()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "integer")
                throw new Error();

            long value = iReader.ReadElementContentAsLong();
            SkipToNextKey();
            return value;
        }

        public string ReadValueString()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "string")
                throw new Error();

            string value = iReader.ReadElementContentAsString();
            SkipToNextKey();
            return value;
        }

        public bool ReadValueBool()
        {
            Assert.Check(!iEndOfDict && iCurrentDictReader == null && iCurrentArrayReader == null);
            if (iReader.Name != "true" && iReader.Name != "false")
                throw new Error();

            bool value = (iReader.Name == "true");
            SkipToNextKey();
            return value;
        }

        private void SkipToNextKey()
        {
            do
            {
                if (iReader.NodeType == XmlNodeType.Element && iReader.Name == "key")
                    return;

                if (iReader.NodeType == XmlNodeType.EndElement && iReader.Name == "dict")
                {
                    iEndOfDict = true;
                    iReader.Read();
                    return;
                }
            }
            while (iReader.Read());

            throw new Error();
        }

        private XmlReader iReader;
        private bool iEndOfDict;
        private XmlDictReader iCurrentDictReader;
        private XmlArrayReader iCurrentArrayReader;
    }


    public class LibraryLoader
    {
        public LibraryLoader(Library aLibrary)
        {
            iLibrary = aLibrary;
        }

        public void Start()
        {
            if (iThread != null)
                Stop();

            iThread = new Thread(this.Run);
            iThread.Name = "iTunes library loader";
            iThread.Start();
        }

        public void Stop()
        {
            if (iThread != null)
            {
                iThread.Abort();
                iThread.Join();
                iThread = null;
            }
        }

        public delegate void ItemAdded(LibraryItem aItem);
        public event ItemAdded EventItemAdded;
        public delegate void PlaylistAdded(LibraryPlaylist aPlaylist);
        public event PlaylistAdded EventPlaylistAdded;
        public delegate void Finished();
        public event Finished EventFinished;
        public delegate void Error();
        public event Error EventError;

        private void Run()
        {
            try
            {
                // read all items - must be done before playlists
                LibraryXmlReaderItems reader = new LibraryXmlReaderItems(iLibrary.XmlFile);
                while (!reader.End)
                {
                    LibraryItem item = reader.Read();
                    iLibrary.AddItem(item);
                    if (EventItemAdded != null)
                        EventItemAdded(item);
                }

                // read playlists - must be done after all items
                LibraryXmlReaderPlaylists playlistReader = new LibraryXmlReaderPlaylists(iLibrary.XmlFile);
                while (!playlistReader.End)
                {
                    LibraryPlaylist playlist = playlistReader.Read();
                    if (playlist.DistinguishedKind == -1 && !playlist.IsSmart && playlist.IsVisible && EventPlaylistAdded != null)
                    {
                        EventPlaylistAdded(playlist);
                    }
                }

                // send finished event
                if (EventFinished != null)
                    EventFinished();
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception aExc)
            {
                UserLog.WriteLine(DateTime.Now + " : reading tracks from Itunes XML failed: " + aExc);
                if (EventError != null)
                    EventError();
            }
        }

        private Library iLibrary;
        private Thread iThread;
    }


}



