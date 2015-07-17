using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using System;

using Upnp;

using Linn;

namespace Linn.Kinsky
{
    public class Playlist
    {
        public class SaveException : Exception
        {
            public SaveException(string aFilename, string aMessage)
                : base(aMessage)
            {
                Filename = aFilename;
            }

            public string Filename;
        }

        public Playlist(string aFilename)
            : this()
        {
            iFilename = aFilename;

            iHomeContainer.Id = aFilename;
            iHomeContainer.Title = Name;
        }

        public Playlist()
        {
            iTracks = new List<upnpObject>();
            iHomeContainer = new playlistContainer();
            iHomeContainer.Id = "";
            iHomeContainer.ParentId = "Playlists";
            iHomeContainer.Restricted = false;
        }

        public container HomeContainer
        {
            get
            {
                return iHomeContainer;
            }
        }

        public string Filename
        {
            get
            {
                return iFilename;
            }
        }
		
		public string Directory
		{
			get
			{
				FileInfo info = new FileInfo(iFilename);
				return info.DirectoryName;
			}
		}

        public string Name
        {
            get
            {                
                try
                {
                    FileInfo info = new FileInfo(iFilename);
                    return info.Name.Replace(kPlaylistExtension, "");
                }
                // playlist is possibly a uri
                catch (ArgumentException ex)
                {
                    try
                    {
                        Uri uri = new Uri(iFilename);
                        return uri.OriginalString;
                    }
                    catch (UriFormatException)
                    {
                        throw ex;
                    }
                }
            }
        }
		
        public void Insert(int aIndex, upnpObject aObject)
        {
            DidlLite didl = new DidlLite();
            didl.Add(aObject);
			
            didl = new DidlLite(didl.Xml);
			
            upnpObject o = didl[0];
			
            o.ParentId = iFilename;
            o.Id = string.Format("{0}/{1}", iFilename, iNextTrackId++);
            o.Restricted = false;
			
            iTracks.Insert(aIndex, o);
        }
		
        public void Remove(upnpObject aObject)
        {
            iTracks.Remove(aObject);
        }

        public ReadOnlyCollection<upnpObject> Tracks
        {
            get
            {
                return iTracks.AsReadOnly();
            }
        }

        public void Load(string aFilename)
        {
            iFilename = aFilename;

            iHomeContainer.Id = aFilename;
            iHomeContainer.Title = Name;

            Load();
        }

        public void Load()
        {
            if (iTracks.Count > 0)
            {
                iTracks.Clear();
            }

            XmlDocument document = new XmlDocument();
            document.Load(iFilename);

            XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(document.NameTable);
            xmlNsMan.AddNamespace("linn", "urn:linn-co-uk/playlist");

            XmlNode version = document.SelectSingleNode("/linn:Playlist/@version", xmlNsMan);

            try
            {
                uint versionNumber = uint.Parse(version.FirstChild.Value, System.Globalization.CultureInfo.InvariantCulture);
                if (versionNumber == kVersion)
                {
                    XmlNodeList tracks = document.SelectNodes("/linn:Playlist/linn:Track", xmlNsMan);
                    foreach (XmlNode t in tracks)
                    {
                        DidlLite didl = null;
                        if (t.FirstChild != null)
                        {
                            didl = new DidlLite(t.FirstChild.OuterXml);
                            if (didl.Count > 0)
                            {
                                iTracks.Add(didl[0]);
                            }
                        }
                    }
                }
                else
                {
                    UserLog.WriteLine(DateTime.Now + ": Playlist " + iFilename + " failed version check; found " + versionNumber + ", expected " + kVersion.ToString());
                }
            }
            catch (FormatException)
            {
                UserLog.WriteLine(DateTime.Now + ": Playlist " + iFilename + " failed version check; found unknown, expected " + kVersion.ToString());
            }
        }

        public void Save()
        {
            Assert.Check(iFilename != null);

            Save(iFilename);
        }

        public void SaveAs(string aFilename)
        {
            Save(aFilename);

            // get rid of the old file
            //Delete();

            iFilename = aFilename;

            iHomeContainer.Title = Name;
        }

        private void Save(string aFilename)
        {
            XmlDocument document = new XmlDocument();

            XmlElement playlist = document.CreateElement("linn", "Playlist", "urn:linn-co-uk/playlist");
            playlist.SetAttribute("version", kVersion.ToString());

            foreach (upnpObject o in iTracks)
            {
                XmlElement track = document.CreateElement("linn", "Track", "urn:linn-co-uk/playlist");

                DidlLite didlLite = new DidlLite();
                if(o.ParentId != aFilename)
                {
                    o.ParentId = aFilename;
                    o.Id = string.Format("{0}/{1}", aFilename, iNextTrackId++);
                    o.Restricted = false;
                }
                didlLite.Add(o);

                XmlDocument didl = new XmlDocument();
                didl.LoadXml(didlLite.Xml);
                XmlNode n = document.ImportNode(didl.DocumentElement, true);

                track.AppendChild(n);

                playlist.AppendChild(track);
            }

            document.AppendChild(playlist);

            try
            {
                document.Save(aFilename);
            }
            catch (XmlException e)
            {
                throw new SaveException(aFilename, e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new SaveException(aFilename, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new SaveException(aFilename, e.Message);
            }
            catch (NotSupportedException e)
            {
                throw new SaveException(aFilename, e.Message);
            }

            iHomeContainer.Id = aFilename;
            iHomeContainer.ChildCount = iTracks.Count;

            Trace.WriteLine(Trace.kKinsky, "Saved " + iTracks.Count + " tracks as " + aFilename);
        }

        public const string kPlaylistExtension = ".dpl";
        private const uint kVersion = 3;
		
        private uint iNextTrackId = 0;

        private string iFilename;
        private container iHomeContainer;
        private List<upnpObject> iTracks;
    }
} // Linn.Topology
