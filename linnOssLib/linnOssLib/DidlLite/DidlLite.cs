using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using Linn;

namespace Upnp
{
    public class Time
    {
        public class TimeInvalid : System.Exception
        {
            public TimeInvalid(string aTime) : base(aTime) { }
        }

        public Time(int aSecondsTotal)
        {
            iSecondsTotal = aSecondsTotal;
            CalculateHoursMinutesSeconds();
        }
        
        public Time(string aTime)
		{
			string time = aTime;
			bool negative = false;
			if(aTime.Length > 0)
			{
				if (aTime[0] == '-')
				{
					negative = true;
					time = aTime.Substring(1);
				}
			}
			
			string[] result = time.Split(':');
			int length = result.GetLength(0);
			if (length > 0)
			{
				try
				{
					// parse hours
					if(length > 2)
					{
						if (!string.IsNullOrEmpty(result[0]))
						{
							iSecondsTotal += int.Parse(result[0]) * 3600;
						}
						if (iSecondsTotal < 0)
						{
							iSecondsTotal = -iSecondsTotal;
						}
					}
					
					// parse minutes
					if(length > 1)
					{
						int mins = 0;
						if (!string.IsNullOrEmpty(result[length - 2]))
						{
							mins = int.Parse(result[length - 2]);
						}
						iSecondsTotal += mins * 60;
					}
					
					// parse seconds
					if(length > 0)
					{
						int index = result[length - 1].IndexOf('.');
						int secs = 0;
						if (index == -1)
						{
							if (!string.IsNullOrEmpty(result[length - 1]))
							{
								secs = int.Parse(result[length - 1]);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(result[length - 1]))
							{
								secs = int.Parse(result[length - 1].Substring(0, index));
							}
						}
						iSecondsTotal += secs;
					}
					
					// check if we are a negative time
					if (negative)
					{
						iSecondsTotal = -iSecondsTotal;
					}
					
					CalculateHoursMinutesSeconds();
				}
				catch(FormatException)
				{
					throw new TimeInvalid(aTime);
				}
			}
			else
			{
				throw new TimeInvalid(aTime);
			}
		}
        
        public int SecondsTotal
        {
            get
            {
                return iSecondsTotal;
            }
            set
            {
                iSecondsTotal = value;
                CalculateHoursMinutesSeconds();
            }
        }

        public int Hours
        {
            get
            {
                return iHours;
            }
        }

        public int Minutes
        {
            get
            {
                return iMinutes;
            }
        }

        public int Seconds
        {
            get
            {
                return iSeconds;
            }
        }

        public string ToPrettyString()
        {
            string time = string.Empty;
            if (iSecondsTotal < 0)
            {
                time += "-";
            }
            if (iHours > 0)
            {
                time += iHours;
                time += ":";
                time += string.Format("{0:00}", iMinutes);
            }
            else
            {
                time += iMinutes;
            }
            time += ":";
            time += string.Format("{0:00}", iSeconds);

            return time;
        }

        public override string ToString()
        {
            return (iSecondsTotal < 0 ? "- " : (iHours < 10 ? "0" : "")) + iHours.ToString() + ":" + String.Format("{0:00}", iMinutes) + ":" + String.Format("{0:00}", iSeconds);
        }

        private void CalculateHoursMinutesSeconds()
        {
            iHours = iSecondsTotal / 3600;
            iMinutes = (iSecondsTotal % 3600) / 60;
            iSeconds = (iSecondsTotal % 3600) % 60;

            if (iSecondsTotal < 0)
            {
                iHours = -iHours;
                iMinutes = -iMinutes;
                iSeconds = -iSeconds;
            }

            Assert.Check(iMinutes < 60);
            Assert.Check(iSeconds < 60);
        }
        
        private int iSecondsTotal;
        private int iHours;
        private int iMinutes;
        private int iSeconds;
    }
    
    public class DidlLite : List<upnpObject>
    {
        public DidlLite() { }
        
        public DidlLite(string aDidlLite)
        {
            XmlNameTable xmlNsTable = new NameTable();
            XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(xmlNsTable);
            xmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            xmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            xmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            xmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");

            XmlReader reader = XmlTextReader.Create(new StringReader(aDidlLite));

            while(!reader.EOF)
            {
                if(reader.NodeType == XmlNodeType.Element)
                {
                    XmlNode n = null;
                    if(reader.Name == "item")
                    {
                        XmlDocument xmlDocument = new XmlDocument(xmlNsMan.NameTable);
                        XmlNode node = xmlDocument.ReadNode(reader);
                        xmlDocument.AppendChild(node);

                        n = xmlDocument.SelectSingleNode("didl:item/upnp:class", xmlNsMan);
                    }
                    else if(reader.Name == "container")
                    {
                        XmlDocument xmlDocument = new XmlDocument(xmlNsMan.NameTable);
                        XmlNode node = xmlDocument.ReadNode(reader);
                        xmlDocument.AppendChild(node);

                        n = xmlDocument.SelectSingleNode("didl:container/upnp:class", xmlNsMan);
                    }
                    else
                    {
                        reader.Read();
                    }

                    if(n != null)
                    {
                        string didlType = n.InnerText;
                        //didlType = didlType.Replace("object", "upnpObject");
        
                        string[] names = didlType.Split('.');
                        int index = names.Length - 1;
        
                        while(index != -1)
                        {
                            if(names[index] == "item")
                            {
                                Add(new item(n.ParentNode));
                                break;
                            }
                            if(names[index] == "container")
                            {
                                Add(new container(n.ParentNode));
                                break;
                            }
                            if(names[index] == "audioItem")
                            {
                                Add(new audioItem(n.ParentNode));
                                break;
                            }
                            if(names[index] == "musicTrack")
                            {
                                Add(new musicTrack(n.ParentNode));
                                break;
                            }
                            if(names[index] == "audioBroadcast")
                            {
                                Add(new audioBroadcast(n.ParentNode));
                                break;
                            }
                            if(names[index] == "audioBook")
                            {
                                Add(new audioBook(n.ParentNode));
                                break;
                            }
                            if(names[index] == "videoItem")
                            {
                                Add(new videoItem(n.ParentNode));
                                break;
                            }
                            if(names[index] == "movie")
                            {
                                Add(new movie(n.ParentNode));
                                break;
                            }
                            if(names[index] == "videoBroadcast")
                            {
                                Add(new videoBroadcast(n.ParentNode));
                                break;
                            }
                            if(names[index] == "musicVideoClip")
                            {
                                Add(new musicVideoClip(n.ParentNode));
                                break;
                            }
                            if(names[index] == "imageItem")
                            {
                                Add(new imageItem(n.ParentNode));
                                break;
                            }
                            if(names[index] == "photo")
                            {
                                Add(new photo(n.ParentNode));
                                break;
                            }
                            if(names[index] == "playlistItem")
                            {
                                Add(new playlistItem(n.ParentNode));
                                break;
                            }
                            if(names[index] == "textItem")
                            {
                                Add(new textItem(n.ParentNode));
                                break;
                            }
                            if(names[index] == "album")
                            {
                                Add(new album(n.ParentNode));
                                break;
                            }
                            if(names[index] == "musicAlbum")
                            {
                                Add(new musicAlbum(n.ParentNode));
                                break;
                            }
                            if(names[index] == "photoAlbum")
                            {
                                Add(new photoAlbum(n.ParentNode));
                                break;
                            }
                            if(names[index] == "genre")
                            {
                                Add(new genre(n.ParentNode));
                                break;
                            }
                            if(names[index] == "musicGenre")
                            {
                                Add(new musicGenre(n.ParentNode));
                                break;
                            }
                            if(names[index] == "movieGenre")
                            {
                                Add(new movieGenre(n.ParentNode));
                                break;
                            }
                            if(names[index] == "playlistContainer")
                            {
                                Add(new playlistContainer(n.ParentNode));
                                break;
                            }
                            if(names[index] == "person")
                            {
                                Add(new person(n.ParentNode));
                                break;
                            }
                            if(names[index] == "musicArtist")
                            {
                                Add(new musicArtist(n.ParentNode));
                                break;
                            }
                            if(names[index] == "storageSystem")
                            {
                                Add(new storageSystem(n.ParentNode));
                                break;
                            }
                            if(names[index] == "storageVolume")
                            {
                                Add(new storageVolume(n.ParentNode));
                                break;
                            }
                            if(names[index] == "storageFolder")
                            {
                                Add(new storageFolder(n.ParentNode));
                                break;
                            }
                            else
                            {
                                index--;
                            }
                        }
        
                        if (index == -1)
                        {
                            UserLog.WriteLine(DateTime.Now + ": DidlLite construction error - no type (" + didlType + ")");
                            UserLog.WriteLine(n.ParentNode.InnerText);
                            Assert.Check(false);
                        }
                    }
                }
                else
                {
                    reader.Read();
                }
            }
        }

        public string Xml
        {
            get
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlNamespaceManager xmlNsMan = new XmlNamespaceManager(xmlDocument.NameTable);
                xmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
                xmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
                xmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                xmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
                
                XmlElement element = xmlDocument.CreateElement("DIDL-Lite", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
                xmlDocument.AppendChild(element);
                
                foreach (upnpObject o in this)
                {
                    xmlDocument.DocumentElement.AppendChild(xmlDocument.ImportNode(o.XmlNode, true));
                }

                return xmlDocument.OuterXml;
            }
        }
    }
    
    public class resource
    {
        public resource() {
        }
        public resource(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual long Size {
            get {
                lock (iLock)
                {
                    PreCacheSize();
                }
                return iSize;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iSize = value;
                    iSizeFound = true;
                }
            }
        }
        private void PreCacheSize() {
            lock (iLock)
            {
            if(!iSizeSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@size", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iSize = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iSizeFound = true;
                    }
                }
                iSizeSearched = true;
            }
            }
        }
        
        public virtual string Duration {
            get {
                lock (iLock)
                {
                    PreCacheDuration();
                }
                return iDuration;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDuration = value;
                    iDurationFound = true;
                }
            }
        }
        private void PreCacheDuration() {
            lock (iLock)
            {
            if(!iDurationSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@duration", iXmlNsMan);
                    if (n != null) {
                        iDuration = n.InnerText;
                        iDurationFound = true;
                    }
                }
                iDurationSearched = true;
            }
            }
        }
        
        public virtual int Bitrate {
            get {
                lock (iLock)
                {
                    PreCacheBitrate();
                }
                return iBitrate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iBitrate = value;
                    iBitrateFound = true;
                }
            }
        }
        private void PreCacheBitrate() {
            lock (iLock)
            {
            if(!iBitrateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@bitrate", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iBitrate = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iBitrateFound = true;
                    }
                }
                iBitrateSearched = true;
            }
            }
        }
        
        public virtual int SampleFrequency {
            get {
                lock (iLock)
                {
                    PreCacheSampleFrequency();
                }
                return iSampleFrequency;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iSampleFrequency = value;
                    iSampleFrequencyFound = true;
                }
            }
        }
        private void PreCacheSampleFrequency() {
            lock (iLock)
            {
            if(!iSampleFrequencySearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@sampleFrequency", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iSampleFrequency = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iSampleFrequencyFound = true;
                    }
                }
                iSampleFrequencySearched = true;
            }
            }
        }
        
        public virtual int BitsPerSample {
            get {
                lock (iLock)
                {
                    PreCacheBitsPerSample();
                }
                return iBitsPerSample;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iBitsPerSample = value;
                    iBitsPerSampleFound = true;
                }
            }
        }
        private void PreCacheBitsPerSample() {
            lock (iLock)
            {
            if(!iBitsPerSampleSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@bitsPerSample", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iBitsPerSample = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iBitsPerSampleFound = true;
                    }
                }
                iBitsPerSampleSearched = true;
            }
            }
        }
        
        public virtual int NrAudioChannels {
            get {
                lock (iLock)
                {
                    PreCacheNrAudioChannels();
                }
                return iNrAudioChannels;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iNrAudioChannels = value;
                    iNrAudioChannelsFound = true;
                }
            }
        }
        private void PreCacheNrAudioChannels() {
            lock (iLock)
            {
            if(!iNrAudioChannelsSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@nrAudioChannels", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iNrAudioChannels = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iNrAudioChannelsFound = true;
                    }
                }
                iNrAudioChannelsSearched = true;
            }
            }
        }
        
        public virtual string Resolution {
            get {
                lock (iLock)
                {
                    PreCacheResolution();
                }
                return iResolution;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iResolution = value;
                    iResolutionFound = true;
                }
            }
        }
        private void PreCacheResolution() {
            lock (iLock)
            {
            if(!iResolutionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@resolution", iXmlNsMan);
                    if (n != null) {
                        iResolution = n.InnerText;
                        iResolutionFound = true;
                    }
                }
                iResolutionSearched = true;
            }
            }
        }
        
        public virtual int ColourDepth {
            get {
                lock (iLock)
                {
                    PreCacheColourDepth();
                }
                return iColourDepth;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iColourDepth = value;
                    iColourDepthFound = true;
                }
            }
        }
        private void PreCacheColourDepth() {
            lock (iLock)
            {
            if(!iColourDepthSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@colourDepth", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iColourDepth = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iColourDepthFound = true;
                    }
                }
                iColourDepthSearched = true;
            }
            }
        }
        
        public virtual string ProtocolInfo {
            get {
                lock (iLock)
                {
                    PreCacheProtocolInfo();
                }
                return iProtocolInfo;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProtocolInfo = value;
                    iProtocolInfoFound = true;
                }
            }
        }
        private void PreCacheProtocolInfo() {
            lock (iLock)
            {
            if(!iProtocolInfoSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@protocolInfo", iXmlNsMan);
                    if (n != null) {
                        iProtocolInfo = n.InnerText;
                        iProtocolInfoFound = true;
                    }
                }
                iProtocolInfoSearched = true;
            }
            }
        }
        
        public virtual string Protection {
            get {
                lock (iLock)
                {
                    PreCacheProtection();
                }
                return iProtection;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProtection = value;
                    iProtectionFound = true;
                }
            }
        }
        private void PreCacheProtection() {
            lock (iLock)
            {
            if(!iProtectionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@protection", iXmlNsMan);
                    if (n != null) {
                        iProtection = n.InnerText;
                        iProtectionFound = true;
                    }
                }
                iProtectionSearched = true;
            }
            }
        }
        
        public virtual string ImportUri {
            get {
                lock (iLock)
                {
                    PreCacheImportUri();
                }
                return iImportUri;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iImportUri = value;
                    iImportUriFound = true;
                }
            }
        }
        private void PreCacheImportUri() {
            lock (iLock)
            {
            if(!iImportUriSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@importURI", iXmlNsMan);
                    if (n != null) {
                        iImportUri = n.InnerText;
                        iImportUriFound = true;
                    }
                }
                iImportUriSearched = true;
            }
            }
        }
        
        public virtual string Uri {
            get {
                lock (iLock)
                {
                    PreCacheUri();
                }
                return iUri;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iUri = value;
                    iUriFound = true;
                }
            }
        }
        private void PreCacheUri() {
            lock (iLock)
            {
            if(!iUriSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode(".", iXmlNsMan);
                    if (n != null) {
                        iUri = n.InnerText;
                        iUriFound = true;
                    }
                }
                iUriSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheSize();
			PreCacheDuration();
			PreCacheBitrate();
			PreCacheSampleFrequency();
			PreCacheBitsPerSample();
			PreCacheNrAudioChannels();
			PreCacheResolution();
			PreCacheColourDepth();
			PreCacheProtocolInfo();
			PreCacheProtection();
			PreCacheImportUri();
			PreCacheUri();
			iXmlDocument = null;
        }

        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iSizeFound) {
                string xpath = "@size";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Size.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iDurationFound) {
                string xpath = "@duration";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Duration.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iBitrateFound) {
                string xpath = "@bitrate";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Bitrate.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iSampleFrequencyFound) {
                string xpath = "@sampleFrequency";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(SampleFrequency.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iBitsPerSampleFound) {
                string xpath = "@bitsPerSample";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(BitsPerSample.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iNrAudioChannelsFound) {
                string xpath = "@nrAudioChannels";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(NrAudioChannels.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iResolutionFound) {
                string xpath = "@resolution";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Resolution.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iColourDepthFound) {
                string xpath = "@colourDepth";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(ColourDepth.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iProtocolInfoFound) {
                string xpath = "@protocolInfo";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(ProtocolInfo.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iProtectionFound) {
                string xpath = "@protection";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Protection.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iImportUriFound) {
                string xpath = "@importURI";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(ImportUri.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iUriFound) {
                XmlText text = aXmlDocument.CreateTextNode(Uri.ToString());
                aXmlElement.AppendChild(text);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iSizeFound = false;
        private bool iSizeSearched = false;
        private long iSize;
        private bool iDurationFound = false;
        private bool iDurationSearched = false;
        private string iDuration;
        private bool iBitrateFound = false;
        private bool iBitrateSearched = false;
        private int iBitrate;
        private bool iSampleFrequencyFound = false;
        private bool iSampleFrequencySearched = false;
        private int iSampleFrequency;
        private bool iBitsPerSampleFound = false;
        private bool iBitsPerSampleSearched = false;
        private int iBitsPerSample;
        private bool iNrAudioChannelsFound = false;
        private bool iNrAudioChannelsSearched = false;
        private int iNrAudioChannels;
        private bool iResolutionFound = false;
        private bool iResolutionSearched = false;
        private string iResolution;
        private bool iColourDepthFound = false;
        private bool iColourDepthSearched = false;
        private int iColourDepth;
        private bool iProtocolInfoFound = false;
        private bool iProtocolInfoSearched = false;
        private string iProtocolInfo;
        private bool iProtectionFound = false;
        private bool iProtectionSearched = false;
        private string iProtection;
        private bool iImportUriFound = false;
        private bool iImportUriSearched = false;
        private string iImportUri;
        private bool iUriFound = false;
        private bool iUriSearched = false;
        private string iUri;
    }
    public class searchClass
    {
        public searchClass() {
        }
        public searchClass(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual bool IncludeDerived {
            get {
                lock (iLock)
                {
                    PreCacheIncludeDerived();
                }
                return iIncludeDerived;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iIncludeDerived = value;
                    iIncludeDerivedFound = true;
                }
            }
        }
        private void PreCacheIncludeDerived() {
            lock (iLock)
            {
            if(!iIncludeDerivedSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@includeDerived", iXmlNsMan);
                    if (n != null) {
                        try
                        {
                            iIncludeDerived = bool.Parse(n.InnerText);
                        }
                        catch(FormatException e)
                        {
				            if(n.InnerText == "0")
					        {
					            iIncludeDerived = false;
					        }
					        else if(n.InnerText == "1")
					        {
					    	    iIncludeDerived = true;
					        }
					        else
					        {
						        throw e;
					        }
				        }
                        iIncludeDerivedFound = true;
                    }
                }
                iIncludeDerivedSearched = true;
            }
            }
        }
        
        public virtual string Name {
            get {
                lock (iLock)
                {
                    PreCacheName();
                }
                return iName;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iName = value;
                    iNameFound = true;
                }
            }
        }
        private void PreCacheName() {
            lock (iLock)
            {
            if(!iNameSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@name", iXmlNsMan);
                    if (n != null) {
                        iName = n.InnerText;
                        iNameFound = true;
                    }
                }
                iNameSearched = true;
            }
            }
        }
        
        public virtual string SearchClass {
            get {
                lock (iLock)
                {
                    PreCacheSearchClass();
                }
                return iSearchClass;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iSearchClass = value;
                    iSearchClassFound = true;
                }
            }
        }
        private void PreCacheSearchClass() {
            lock (iLock)
            {
            if(!iSearchClassSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode(".", iXmlNsMan);
                    if (n != null) {
                        iSearchClass = n.InnerText;
                        iSearchClassFound = true;
                    }
                }
                iSearchClassSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheIncludeDerived();
			PreCacheName();
			PreCacheSearchClass();
			iXmlDocument = null;
        }

        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iIncludeDerivedFound) {
                string xpath = "@includeDerived";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(IncludeDerived.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iNameFound) {
                string xpath = "@name";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Name.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iSearchClassFound) {
                XmlText text = aXmlDocument.CreateTextNode(SearchClass.ToString());
                aXmlElement.AppendChild(text);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iIncludeDerivedFound = false;
        private bool iIncludeDerivedSearched = false;
        private bool iIncludeDerived;
        private bool iNameFound = false;
        private bool iNameSearched = false;
        private string iName;
        private bool iSearchClassFound = false;
        private bool iSearchClassSearched = false;
        private string iSearchClass;
    }
    public class createClass
    {
        public createClass() {
        }
        public createClass(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual bool IncludeDerived {
            get {
                lock (iLock)
                {
                    PreCacheIncludeDerived();
                }
                return iIncludeDerived;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iIncludeDerived = value;
                    iIncludeDerivedFound = true;
                }
            }
        }
        private void PreCacheIncludeDerived() {
            lock (iLock)
            {
            if(!iIncludeDerivedSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@includeDerived", iXmlNsMan);
                    if (n != null) {
                        try
                        {
                            iIncludeDerived = bool.Parse(n.InnerText);
                        }
                        catch(FormatException e)
                        {
				            if(n.InnerText == "0")
					        {
					            iIncludeDerived = false;
					        }
					        else if(n.InnerText == "1")
					        {
					    	    iIncludeDerived = true;
					        }
					        else
					        {
						        throw e;
					        }
				        }
                        iIncludeDerivedFound = true;
                    }
                }
                iIncludeDerivedSearched = true;
            }
            }
        }
        
        public virtual string Name {
            get {
                lock (iLock)
                {
                    PreCacheName();
                }
                return iName;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iName = value;
                    iNameFound = true;
                }
            }
        }
        private void PreCacheName() {
            lock (iLock)
            {
            if(!iNameSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@name", iXmlNsMan);
                    if (n != null) {
                        iName = n.InnerText;
                        iNameFound = true;
                    }
                }
                iNameSearched = true;
            }
            }
        }
        
        public virtual string CreateClass {
            get {
                lock (iLock)
                {
                    PreCacheCreateClass();
                }
                return iCreateClass;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iCreateClass = value;
                    iCreateClassFound = true;
                }
            }
        }
        private void PreCacheCreateClass() {
            lock (iLock)
            {
            if(!iCreateClassSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode(".", iXmlNsMan);
                    if (n != null) {
                        iCreateClass = n.InnerText;
                        iCreateClassFound = true;
                    }
                }
                iCreateClassSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheIncludeDerived();
			PreCacheName();
			PreCacheCreateClass();
			iXmlDocument = null;
        }

        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iIncludeDerivedFound) {
                string xpath = "@includeDerived";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(IncludeDerived.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iNameFound) {
                string xpath = "@name";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Name.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iCreateClassFound) {
                XmlText text = aXmlDocument.CreateTextNode(CreateClass.ToString());
                aXmlElement.AppendChild(text);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iIncludeDerivedFound = false;
        private bool iIncludeDerivedSearched = false;
        private bool iIncludeDerived;
        private bool iNameFound = false;
        private bool iNameSearched = false;
        private string iName;
        private bool iCreateClassFound = false;
        private bool iCreateClassSearched = false;
        private string iCreateClass;
    }
    public class artist
    {
        public artist() {
        }
        public artist(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual string Role {
            get {
                lock (iLock)
                {
                    PreCacheRole();
                }
                return iRole;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRole = value;
                    iRoleFound = true;
                }
            }
        }
        private void PreCacheRole() {
            lock (iLock)
            {
            if(!iRoleSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@role", iXmlNsMan);
                    if (n != null) {
                        iRole = n.InnerText;
                        iRoleFound = true;
                    }
                }
                iRoleSearched = true;
            }
            }
        }
        
        public virtual string Artist {
            get {
                lock (iLock)
                {
                    PreCacheArtist();
                }
                return iArtist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtist = value;
                    iArtistFound = true;
                }
            }
        }
        private void PreCacheArtist() {
            lock (iLock)
            {
            if(!iArtistSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode(".", iXmlNsMan);
                    if (n != null) {
                        iArtist = n.InnerText;
                        iArtistFound = true;
                    }
                }
                iArtistSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheRole();
			PreCacheArtist();
			iXmlDocument = null;
        }

        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iRoleFound) {
                string xpath = "@role";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Role.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iArtistFound) {
                XmlText text = aXmlDocument.CreateTextNode(Artist.ToString());
                aXmlElement.AppendChild(text);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iRoleFound = false;
        private bool iRoleSearched = false;
        private string iRole;
        private bool iArtistFound = false;
        private bool iArtistSearched = false;
        private string iArtist;
    }
    public class actor
    {
        public actor() {
        }
        public actor(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual string Role {
            get {
                lock (iLock)
                {
                    PreCacheRole();
                }
                return iRole;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRole = value;
                    iRoleFound = true;
                }
            }
        }
        private void PreCacheRole() {
            lock (iLock)
            {
            if(!iRoleSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@role", iXmlNsMan);
                    if (n != null) {
                        iRole = n.InnerText;
                        iRoleFound = true;
                    }
                }
                iRoleSearched = true;
            }
            }
        }
        
        public virtual string Actor {
            get {
                lock (iLock)
                {
                    PreCacheActor();
                }
                return iActor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iActor = value;
                    iActorFound = true;
                }
            }
        }
        private void PreCacheActor() {
            lock (iLock)
            {
            if(!iActorSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode(".", iXmlNsMan);
                    if (n != null) {
                        iActor = n.InnerText;
                        iActorFound = true;
                    }
                }
                iActorSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheRole();
			PreCacheActor();
			iXmlDocument = null;
        }

        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iRoleFound) {
                string xpath = "@role";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Role.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iActorFound) {
                XmlText text = aXmlDocument.CreateTextNode(Actor.ToString());
                aXmlElement.AppendChild(text);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iRoleFound = false;
        private bool iRoleSearched = false;
        private string iRole;
        private bool iActorFound = false;
        private bool iActorSearched = false;
        private string iActor;
    }
    public class author
    {
        public author() {
        }
        public author(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual string Role {
            get {
                lock (iLock)
                {
                    PreCacheRole();
                }
                return iRole;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRole = value;
                    iRoleFound = true;
                }
            }
        }
        private void PreCacheRole() {
            lock (iLock)
            {
            if(!iRoleSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@role", iXmlNsMan);
                    if (n != null) {
                        iRole = n.InnerText;
                        iRoleFound = true;
                    }
                }
                iRoleSearched = true;
            }
            }
        }
        
        public virtual string Author {
            get {
                lock (iLock)
                {
                    PreCacheAuthor();
                }
                return iAuthor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAuthor = value;
                    iAuthorFound = true;
                }
            }
        }
        private void PreCacheAuthor() {
            lock (iLock)
            {
            if(!iAuthorSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode(".", iXmlNsMan);
                    if (n != null) {
                        iAuthor = n.InnerText;
                        iAuthorFound = true;
                    }
                }
                iAuthorSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheRole();
			PreCacheAuthor();
			iXmlDocument = null;
        }

        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iRoleFound) {
                string xpath = "@role";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Role.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iAuthorFound) {
                XmlText text = aXmlDocument.CreateTextNode(Author.ToString());
                aXmlElement.AppendChild(text);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iRoleFound = false;
        private bool iRoleSearched = false;
        private string iRole;
        private bool iAuthorFound = false;
        private bool iAuthorSearched = false;
        private string iAuthor;
    }
    public abstract class upnpObject
    {
        public upnpObject() {
        }
        public upnpObject(XmlNode aXmlNode)
        {
            iXmlDocument = new XmlDocument();
            iXmlNsMan = new XmlNamespaceManager(iXmlDocument.NameTable);
            iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
            iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
            iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
            iXmlDocument.AppendChild(iXmlDocument.ImportNode(aXmlNode, true));
        }
        
        public virtual string Id {
            get {
                lock (iLock)
                {
                    PreCacheId();
                }
                return iId;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iId = value;
                    iIdFound = true;
                }
            }
        }
        private void PreCacheId() {
            lock (iLock)
            {
            if(!iIdSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@id", iXmlNsMan);
                    if (n != null) {
                        iId = n.InnerText;
                        iIdFound = true;
                    }
                }
                iIdSearched = true;
            }
            }
        }
        
        public virtual string ParentId {
            get {
                lock (iLock)
                {
                    PreCacheParentId();
                }
                return iParentId;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iParentId = value;
                    iParentIdFound = true;
                }
            }
        }
        private void PreCacheParentId() {
            lock (iLock)
            {
            if(!iParentIdSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@parentID", iXmlNsMan);
                    if (n != null) {
                        iParentId = n.InnerText;
                        iParentIdFound = true;
                    }
                }
                iParentIdSearched = true;
            }
            }
        }
        
        public virtual string Title {
            get {
                lock (iLock)
                {
                    PreCacheTitle();
                }
                return iTitle;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iTitle = value;
                    iTitleFound = true;
                }
            }
        }
        private void PreCacheTitle() {
            lock (iLock)
            {
            if(!iTitleSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:title", iXmlNsMan);
                    if (n != null) {
                        iTitle = n.InnerText;
                        iTitleFound = true;
                    }
                }
                iTitleSearched = true;
            }
            }
        }
        
        public virtual string Creator {
            get {
                lock (iLock)
                {
                    PreCacheCreator();
                }
                return iCreator;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iCreator = value;
                    iCreatorFound = true;
                }
            }
        }
        private void PreCacheCreator() {
            lock (iLock)
            {
            if(!iCreatorSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:creator", iXmlNsMan);
                    if (n != null) {
                        iCreator = n.InnerText;
                        iCreatorFound = true;
                    }
                }
                iCreatorSearched = true;
            }
            }
        }
        
        public virtual string Class {
            get {
                lock (iLock)
                {
                    PreCacheClass();
                }
                return iClass;
            }
        }
        private void PreCacheClass() {
            lock (iLock)
            {
            if(!iClassSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:class", iXmlNsMan);
                    if (n != null) {
                        iClass = n.InnerText;
                        iClassFound = true;
                    }
                }
                iClassSearched = true;
            }
            }
        }
        
        public virtual List<resource> Res {
            get {
                lock (iLock)
                {
                    PreCacheRes();
                }
                return iRes;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRes = value;
                    iResFound = true;
                }
            }
        }
        private void PreCacheRes() {
            lock (iLock)
            {
            if(!iResSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("didl:res", iXmlNsMan)) {
                        iRes.Add(new resource(n));
                    }
                }
                iResSearched = true;
                iResFound = true;
            }
            }
        }
        
        public virtual bool Restricted {
            get {
                lock (iLock)
                {
                    PreCacheRestricted();
                }
                return iRestricted;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRestricted = value;
                    iRestrictedFound = true;
                }
            }
        }
        private void PreCacheRestricted() {
            lock (iLock)
            {
            if(!iRestrictedSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@restricted", iXmlNsMan);
                    if (n != null) {
                        try
                        {
                            iRestricted = bool.Parse(n.InnerText);
                        }
                        catch(FormatException e)
                        {
				            if(n.InnerText == "0")
					        {
					            iRestricted = false;
					        }
					        else if(n.InnerText == "1")
					        {
					    	    iRestricted = true;
					        }
					        else
					        {
						        throw e;
					        }
				        }
                        iRestrictedFound = true;
                    }
                }
                iRestrictedSearched = true;
            }
            }
        }
        
        public virtual string WriteStatus {
            get {
                lock (iLock)
                {
                    PreCacheWriteStatus();
                }
                return iWriteStatus;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iWriteStatus = value;
                    iWriteStatusFound = true;
                }
            }
        }
        private void PreCacheWriteStatus() {
            lock (iLock)
            {
            if(!iWriteStatusSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:writeStatus", iXmlNsMan);
                    if (n != null) {
                        iWriteStatus = n.InnerText;
                        iWriteStatusFound = true;
                    }
                }
                iWriteStatusSearched = true;
            }
            }
        }
        
        public virtual string Icon {
            get {
                lock (iLock)
                {
                    PreCacheIcon();
                }
                return iIcon;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iIcon = value;
                    iIconFound = true;
                }
            }
        }
        private void PreCacheIcon() {
            lock (iLock)
            {
            if(!iIconSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:icon", iXmlNsMan);
                    if (n != null) {
                        iIcon = n.InnerText;
                        iIconFound = true;
                    }
                }
                iIconSearched = true;
            }
            }
        }
        
        public virtual List<string> AlbumArtUri {
            get {
                lock (iLock)
                {
                    PreCacheAlbumArtUri();
                }
                return iAlbumArtUri;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAlbumArtUri = value;
                    iAlbumArtUriFound = true;
                }
            }
        }
        private void PreCacheAlbumArtUri() {
            lock (iLock)
            {
            if(!iAlbumArtUriSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:albumArtURI", iXmlNsMan)) {
                        iAlbumArtUri.Add(n.InnerText);
                    }
                }
                iAlbumArtUriSearched = true;
                iAlbumArtUriFound = true;
            }
            }
        }
        
        public virtual List<string> ArtworkUri {
            get {
                lock (iLock)
                {
                    PreCacheArtworkUri();
                }
                return iArtworkUri;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtworkUri = value;
                    iArtworkUriFound = true;
                }
            }
        }
        private void PreCacheArtworkUri() {
            lock (iLock)
            {
            if(!iArtworkUriSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:artworkURI", iXmlNsMan)) {
                        iArtworkUri.Add(n.InnerText);
                    }
                }
                iArtworkUriSearched = true;
                iArtworkUriFound = true;
            }
            }
        }
        
        public virtual string Udn {
            get {
                lock (iLock)
                {
                    PreCacheUdn();
                }
                return iUdn;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iUdn = value;
                    iUdnFound = true;
                }
            }
        }
        private void PreCacheUdn() {
            lock (iLock)
            {
            if(!iUdnSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:UDN", iXmlNsMan);
                    if (n != null) {
                        iUdn = n.InnerText;
                        iUdnFound = true;
                    }
                }
                iUdnSearched = true;
            }
            }
        }
        

        internal virtual void PreCache() {
			PreCacheId();
			PreCacheParentId();
			PreCacheTitle();
			PreCacheCreator();
			PreCacheClass();
			PreCacheRes();
			foreach(resource i in iRes) {
			    i.PreCache();
			}
			PreCacheRestricted();
			PreCacheWriteStatus();
			PreCacheIcon();
			PreCacheAlbumArtUri();
			PreCacheArtworkUri();
			PreCacheUdn();
			iXmlDocument = null;
        }

        internal XmlNode XmlNode {
            get {
                if(iXmlDocument == null) {
                    XmlDocument xmlDocument = new XmlDocument();
                    iXmlNsMan = new XmlNamespaceManager(xmlDocument.NameTable);
                    iXmlNsMan.AddNamespace("didl", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
                    iXmlNsMan.AddNamespace("upnp", "urn:schemas-upnp-org:metadata-1-0/upnp/");
                    iXmlNsMan.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                    iXmlNsMan.AddNamespace("ldl", "urn:linn-co-uk/DIDL-Lite");
                    XmlElement obj = null;
                    if(this is container) {
                        obj = xmlDocument.CreateElement("container", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
                    } else if(this is item) {
                        obj = xmlDocument.CreateElement("item", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
                    } else {
                        Assert.Check(false);
                    }
                    CreateDidlLite(xmlDocument, obj);
                    xmlDocument.AppendChild(obj);
                    return xmlDocument.FirstChild;
                }
                return iXmlDocument.FirstChild;
            }
        }
        
        internal virtual void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            if(iIdFound) {
                string xpath = "@id";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Id.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iParentIdFound) {
                string xpath = "@parentID";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(ParentId.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iTitleFound) {
                string xpath = "dc:title";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:title", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Title.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iCreatorFound) {
                string xpath = "dc:creator";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:creator", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Creator.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iClassFound) {
                string xpath = "upnp:class";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:class", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Class.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iResFound) {
                foreach(resource item in iRes) {
                    string xpath = "didl:res";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("res", "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/");
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iRestrictedFound) {
                string xpath = "@restricted";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Restricted.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iWriteStatusFound) {
                string xpath = "upnp:writeStatus";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:writeStatus", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(WriteStatus.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iIconFound) {
                string xpath = "upnp:icon";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:icon", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Icon.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iAlbumArtUriFound) {
                foreach(string item in iAlbumArtUri) {
                    string xpath = "upnp:albumArtURI";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:albumArtURI", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iArtworkUriFound) {
                foreach(string item in iArtworkUri) {
                    string xpath = "upnp:artworkURI";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:artworkURI", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iUdnFound) {
                string xpath = "upnp:UDN";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:UDN", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Udn.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        protected XmlDocument iXmlDocument;
        protected XmlNamespaceManager iXmlNsMan;
        protected object iLock = new object();
        private bool iIdFound = false;
        private bool iIdSearched = false;
        private string iId;
        private bool iParentIdFound = false;
        private bool iParentIdSearched = false;
        private string iParentId;
        private bool iTitleFound = false;
        private bool iTitleSearched = false;
        private string iTitle;
        private bool iCreatorFound = false;
        private bool iCreatorSearched = false;
        private string iCreator;
        protected bool iClassFound = false;
        private bool iClassSearched = false;
        protected string iClass;
        private bool iResFound = false;
        private bool iResSearched = false;
        private List<resource> iRes = new List<resource>();
        private bool iRestrictedFound = false;
        private bool iRestrictedSearched = false;
        private bool iRestricted;
        private bool iWriteStatusFound = false;
        private bool iWriteStatusSearched = false;
        private string iWriteStatus;
        private bool iIconFound = false;
        private bool iIconSearched = false;
        private string iIcon;
        private bool iAlbumArtUriFound = false;
        private bool iAlbumArtUriSearched = false;
        private List<string> iAlbumArtUri = new List<string>();
        private bool iArtworkUriFound = false;
        private bool iArtworkUriSearched = false;
        private List<string> iArtworkUri = new List<string>();
        private bool iUdnFound = false;
        private bool iUdnSearched = false;
        private string iUdn;
    }
    public class item : upnpObject
    {
        public item() {
            iClassFound = true;
            iClass = "object.item";
        }
        public item(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string RefId {
            get {
                lock (iLock)
                {
                    PreCacheRefId();
                }
                return iRefId;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRefId = value;
                    iRefIdFound = true;
                }
            }
        }
        private void PreCacheRefId() {
            lock (iLock)
            {
            if(!iRefIdSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@refID", iXmlNsMan);
                    if (n != null) {
                        iRefId = n.InnerText;
                        iRefIdFound = true;
                    }
                }
                iRefIdSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheRefId();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iRefIdFound) {
                string xpath = "@refID";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(RefId.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
        }
        
        private bool iRefIdFound = false;
        private bool iRefIdSearched = false;
        private string iRefId;
    }
    public class container : upnpObject
    {
        public container() {
            iClassFound = true;
            iClass = "object.container";
        }
        public container(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual int ChildCount {
            get {
                lock (iLock)
                {
                    PreCacheChildCount();
                }
                return iChildCount;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iChildCount = value;
                    iChildCountFound = true;
                }
            }
        }
        private void PreCacheChildCount() {
            lock (iLock)
            {
            if(!iChildCountSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@childCount", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iChildCount = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iChildCountFound = true;
                    }
                }
                iChildCountSearched = true;
            }
            }
        }
        
        public virtual List<createClass> CreateClass {
            get {
                lock (iLock)
                {
                    PreCacheCreateClass();
                }
                return iCreateClass;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iCreateClass = value;
                    iCreateClassFound = true;
                }
            }
        }
        private void PreCacheCreateClass() {
            lock (iLock)
            {
            if(!iCreateClassSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:createClass", iXmlNsMan)) {
                        iCreateClass.Add(new createClass(n));
                    }
                }
                iCreateClassSearched = true;
                iCreateClassFound = true;
            }
            }
        }
        
        public virtual List<searchClass> SearchClass {
            get {
                lock (iLock)
                {
                    PreCacheSearchClass();
                }
                return iSearchClass;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iSearchClass = value;
                    iSearchClassFound = true;
                }
            }
        }
        private void PreCacheSearchClass() {
            lock (iLock)
            {
            if(!iSearchClassSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:searchClass", iXmlNsMan)) {
                        iSearchClass.Add(new searchClass(n));
                    }
                }
                iSearchClassSearched = true;
                iSearchClassFound = true;
            }
            }
        }
        
        public virtual bool Searchable {
            get {
                lock (iLock)
                {
                    PreCacheSearchable();
                }
                return iSearchable;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iSearchable = value;
                    iSearchableFound = true;
                }
            }
        }
        private void PreCacheSearchable() {
            lock (iLock)
            {
            if(!iSearchableSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("@searchable", iXmlNsMan);
                    if (n != null) {
                        try
                        {
                            iSearchable = bool.Parse(n.InnerText);
                        }
                        catch(FormatException e)
                        {
				            if(n.InnerText == "0")
					        {
					            iSearchable = false;
					        }
					        else if(n.InnerText == "1")
					        {
					    	    iSearchable = true;
					        }
					        else
					        {
						        throw e;
					        }
				        }
                        iSearchableFound = true;
                    }
                }
                iSearchableSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheChildCount();
			PreCacheCreateClass();
			foreach(createClass i in iCreateClass) {
			    i.PreCache();
			}
			PreCacheSearchClass();
			foreach(searchClass i in iSearchClass) {
			    i.PreCache();
			}
			PreCacheSearchable();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iChildCountFound) {
                string xpath = "@childCount";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(ChildCount.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
            if(iCreateClassFound) {
                foreach(createClass item in iCreateClass) {
                    string xpath = "upnp:createClass";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:createClass", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iSearchClassFound) {
                foreach(searchClass item in iSearchClass) {
                    string xpath = "upnp:searchClass";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:searchClass", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iSearchableFound) {
                string xpath = "@searchable";
                XmlAttribute attribute = aXmlDocument.CreateAttribute(xpath.Substring(1));
                XmlText text = aXmlDocument.CreateTextNode(Searchable.ToString());
                attribute.AppendChild(text);
                aXmlElement.SetAttributeNode(attribute);
            }
        }
        
        private bool iChildCountFound = false;
        private bool iChildCountSearched = false;
        private int iChildCount;
        private bool iCreateClassFound = false;
        private bool iCreateClassSearched = false;
        private List<createClass> iCreateClass = new List<createClass>();
        private bool iSearchClassFound = false;
        private bool iSearchClassSearched = false;
        private List<searchClass> iSearchClass = new List<searchClass>();
        private bool iSearchableFound = false;
        private bool iSearchableSearched = false;
        private bool iSearchable;
    }
    public class audioItem : item
    {
        public audioItem() {
            iClassFound = true;
            iClass = "object.item.audioItem";
        }
        public audioItem(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<string> Genre {
            get {
                lock (iLock)
                {
                    PreCacheGenre();
                }
                return iGenre;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iGenre = value;
                    iGenreFound = true;
                }
            }
        }
        private void PreCacheGenre() {
            lock (iLock)
            {
            if(!iGenreSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:genre", iXmlNsMan)) {
                        iGenre.Add(n.InnerText);
                    }
                }
                iGenreSearched = true;
                iGenreFound = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Publisher {
            get {
                lock (iLock)
                {
                    PreCachePublisher();
                }
                return iPublisher;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iPublisher = value;
                    iPublisherFound = true;
                }
            }
        }
        private void PreCachePublisher() {
            lock (iLock)
            {
            if(!iPublisherSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:publisher", iXmlNsMan)) {
                        iPublisher.Add(n.InnerText);
                    }
                }
                iPublisherSearched = true;
                iPublisherFound = true;
            }
            }
        }
        
        public virtual string Language {
            get {
                lock (iLock)
                {
                    PreCacheLanguage();
                }
                return iLanguage;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLanguage = value;
                    iLanguageFound = true;
                }
            }
        }
        private void PreCacheLanguage() {
            lock (iLock)
            {
            if(!iLanguageSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:language", iXmlNsMan);
                    if (n != null) {
                        iLanguage = n.InnerText;
                        iLanguageFound = true;
                    }
                }
                iLanguageSearched = true;
            }
            }
        }
        
        public virtual List<string> Relation {
            get {
                lock (iLock)
                {
                    PreCacheRelation();
                }
                return iRelation;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRelation = value;
                    iRelationFound = true;
                }
            }
        }
        private void PreCacheRelation() {
            lock (iLock)
            {
            if(!iRelationSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:relation", iXmlNsMan)) {
                        iRelation.Add(n.InnerText);
                    }
                }
                iRelationSearched = true;
                iRelationFound = true;
            }
            }
        }
        
        public virtual List<string> Rights {
            get {
                lock (iLock)
                {
                    PreCacheRights();
                }
                return iRights;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRights = value;
                    iRightsFound = true;
                }
            }
        }
        private void PreCacheRights() {
            lock (iLock)
            {
            if(!iRightsSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:rights", iXmlNsMan)) {
                        iRights.Add(n.InnerText);
                    }
                }
                iRightsSearched = true;
                iRightsFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheGenre();
			PreCacheDescription();
			PreCacheLongDescription();
			PreCachePublisher();
			PreCacheLanguage();
			PreCacheRelation();
			PreCacheRights();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iGenreFound) {
                foreach(string item in iGenre) {
                    string xpath = "upnp:genre";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:genre", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iPublisherFound) {
                foreach(string item in iPublisher) {
                    string xpath = "dc:publisher";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:publisher", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iLanguageFound) {
                string xpath = "dc:language";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:language", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Language.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRelationFound) {
                foreach(string item in iRelation) {
                    string xpath = "dc:relation";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:relation", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iRightsFound) {
                foreach(string item in iRights) {
                    string xpath = "dc:rights";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:rights", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iGenreFound = false;
        private bool iGenreSearched = false;
        private List<string> iGenre = new List<string>();
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iPublisherFound = false;
        private bool iPublisherSearched = false;
        private List<string> iPublisher = new List<string>();
        private bool iLanguageFound = false;
        private bool iLanguageSearched = false;
        private string iLanguage;
        private bool iRelationFound = false;
        private bool iRelationSearched = false;
        private List<string> iRelation = new List<string>();
        private bool iRightsFound = false;
        private bool iRightsSearched = false;
        private List<string> iRights = new List<string>();
    }
    public class musicTrack : audioItem
    {
        public musicTrack() {
            iClassFound = true;
            iClass = "object.item.audioItem.musicTrack";
        }
        public musicTrack(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<artist> Artist {
            get {
                lock (iLock)
                {
                    PreCacheArtist();
                }
                return iArtist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtist = value;
                    iArtistFound = true;
                }
            }
        }
        private void PreCacheArtist() {
            lock (iLock)
            {
            if(!iArtistSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:artist", iXmlNsMan)) {
                        iArtist.Add(new artist(n));
                    }
                }
                iArtistSearched = true;
                iArtistFound = true;
            }
            }
        }
        
        public virtual List<string> Album {
            get {
                lock (iLock)
                {
                    PreCacheAlbum();
                }
                return iAlbum;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAlbum = value;
                    iAlbumFound = true;
                }
            }
        }
        private void PreCacheAlbum() {
            lock (iLock)
            {
            if(!iAlbumSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:album", iXmlNsMan)) {
                        iAlbum.Add(n.InnerText);
                    }
                }
                iAlbumSearched = true;
                iAlbumFound = true;
            }
            }
        }
        
        public virtual List<author> Author {
            get {
                lock (iLock)
                {
                    PreCacheAuthor();
                }
                return iAuthor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAuthor = value;
                    iAuthorFound = true;
                }
            }
        }
        private void PreCacheAuthor() {
            lock (iLock)
            {
            if(!iAuthorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:author", iXmlNsMan)) {
                        iAuthor.Add(new author(n));
                    }
                }
                iAuthorSearched = true;
                iAuthorFound = true;
            }
            }
        }
        
        public virtual int OriginalTrackNumber {
            get {
                lock (iLock)
                {
                    PreCacheOriginalTrackNumber();
                }
                return iOriginalTrackNumber;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iOriginalTrackNumber = value;
                    iOriginalTrackNumberFound = true;
                }
            }
        }
        private void PreCacheOriginalTrackNumber() {
            lock (iLock)
            {
            if(!iOriginalTrackNumberSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:originalTrackNumber", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iOriginalTrackNumber = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iOriginalTrackNumberFound = true;
                    }
                }
                iOriginalTrackNumberSearched = true;
            }
            }
        }
        
        public virtual List<string> Playlist {
            get {
                lock (iLock)
                {
                    PreCachePlaylist();
                }
                return iPlaylist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iPlaylist = value;
                    iPlaylistFound = true;
                }
            }
        }
        private void PreCachePlaylist() {
            lock (iLock)
            {
            if(!iPlaylistSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:playlist", iXmlNsMan)) {
                        iPlaylist.Add(n.InnerText);
                    }
                }
                iPlaylistSearched = true;
                iPlaylistFound = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual List<string> Contributor {
            get {
                lock (iLock)
                {
                    PreCacheContributor();
                }
                return iContributor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iContributor = value;
                    iContributorFound = true;
                }
            }
        }
        private void PreCacheContributor() {
            lock (iLock)
            {
            if(!iContributorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:contributor", iXmlNsMan)) {
                        iContributor.Add(n.InnerText);
                    }
                }
                iContributorSearched = true;
                iContributorFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        
        public virtual int OriginalDiscNumber {
            get {
                lock (iLock)
                {
                    PreCacheOriginalDiscNumber();
                }
                return iOriginalDiscNumber;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iOriginalDiscNumber = value;
                    iOriginalDiscNumberFound = true;
                }
            }
        }
        private void PreCacheOriginalDiscNumber() {
            lock (iLock)
            {
            if(!iOriginalDiscNumberSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:originalDiscNumber", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iOriginalDiscNumber = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iOriginalDiscNumberFound = true;
                    }
                }
                iOriginalDiscNumberSearched = true;
            }
            }
        }
        
        public virtual int OriginalDiscCount {
            get {
                lock (iLock)
                {
                    PreCacheOriginalDiscCount();
                }
                return iOriginalDiscCount;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iOriginalDiscCount = value;
                    iOriginalDiscCountFound = true;
                }
            }
        }
        private void PreCacheOriginalDiscCount() {
            lock (iLock)
            {
            if(!iOriginalDiscCountSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:originalDiscCount", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iOriginalDiscCount = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iOriginalDiscCountFound = true;
                    }
                }
                iOriginalDiscCountSearched = true;
            }
            }
        }
        
        public virtual string ReplayGainAlbum {
            get {
                lock (iLock)
                {
                    PreCacheReplayGainAlbum();
                }
                return iReplayGainAlbum;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iReplayGainAlbum = value;
                    iReplayGainAlbumFound = true;
                }
            }
        }
        private void PreCacheReplayGainAlbum() {
            lock (iLock)
            {
            if(!iReplayGainAlbumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:replayGainAlbum", iXmlNsMan);
                    if (n != null) {
                        iReplayGainAlbum = n.InnerText;
                        iReplayGainAlbumFound = true;
                    }
                }
                iReplayGainAlbumSearched = true;
            }
            }
        }
        
        public virtual string ReplayGainAlbumPeak {
            get {
                lock (iLock)
                {
                    PreCacheReplayGainAlbumPeak();
                }
                return iReplayGainAlbumPeak;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iReplayGainAlbumPeak = value;
                    iReplayGainAlbumPeakFound = true;
                }
            }
        }
        private void PreCacheReplayGainAlbumPeak() {
            lock (iLock)
            {
            if(!iReplayGainAlbumPeakSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:replayGainAlbumPeak", iXmlNsMan);
                    if (n != null) {
                        iReplayGainAlbumPeak = n.InnerText;
                        iReplayGainAlbumPeakFound = true;
                    }
                }
                iReplayGainAlbumPeakSearched = true;
            }
            }
        }
        
        public virtual string ReplayGainTrack {
            get {
                lock (iLock)
                {
                    PreCacheReplayGainTrack();
                }
                return iReplayGainTrack;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iReplayGainTrack = value;
                    iReplayGainTrackFound = true;
                }
            }
        }
        private void PreCacheReplayGainTrack() {
            lock (iLock)
            {
            if(!iReplayGainTrackSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:replayGainTrack", iXmlNsMan);
                    if (n != null) {
                        iReplayGainTrack = n.InnerText;
                        iReplayGainTrackFound = true;
                    }
                }
                iReplayGainTrackSearched = true;
            }
            }
        }
        
        public virtual string ReplayGainTrackPeak {
            get {
                lock (iLock)
                {
                    PreCacheReplayGainTrackPeak();
                }
                return iReplayGainTrackPeak;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iReplayGainTrackPeak = value;
                    iReplayGainTrackPeakFound = true;
                }
            }
        }
        private void PreCacheReplayGainTrackPeak() {
            lock (iLock)
            {
            if(!iReplayGainTrackPeakSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:replayGainTrackPeak", iXmlNsMan);
                    if (n != null) {
                        iReplayGainTrackPeak = n.InnerText;
                        iReplayGainTrackPeakFound = true;
                    }
                }
                iReplayGainTrackPeakSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheArtist();
			foreach(artist i in iArtist) {
			    i.PreCache();
			}
			PreCacheAlbum();
			PreCacheAuthor();
			foreach(author i in iAuthor) {
			    i.PreCache();
			}
			PreCacheOriginalTrackNumber();
			PreCachePlaylist();
			PreCacheStorageMedium();
			PreCacheContributor();
			PreCacheDate();
			PreCacheOriginalDiscNumber();
			PreCacheOriginalDiscCount();
			PreCacheReplayGainAlbum();
			PreCacheReplayGainAlbumPeak();
			PreCacheReplayGainTrack();
			PreCacheReplayGainTrackPeak();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iArtistFound) {
                foreach(artist item in iArtist) {
                    string xpath = "upnp:artist";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:artist", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iAlbumFound) {
                foreach(string item in iAlbum) {
                    string xpath = "upnp:album";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:album", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iAuthorFound) {
                foreach(author item in iAuthor) {
                    string xpath = "upnp:author";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:author", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iOriginalTrackNumberFound) {
                string xpath = "upnp:originalTrackNumber";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:originalTrackNumber", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(OriginalTrackNumber.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iPlaylistFound) {
                foreach(string item in iPlaylist) {
                    string xpath = "upnp:playlist";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:playlist", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iContributorFound) {
                foreach(string item in iContributor) {
                    string xpath = "dc:contributor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:contributor", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iOriginalDiscNumberFound) {
                string xpath = "upnp:originalDiscNumber";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:originalDiscNumber", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(OriginalDiscNumber.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iOriginalDiscCountFound) {
                string xpath = "upnp:originalDiscCount";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:originalDiscCount", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(OriginalDiscCount.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iReplayGainAlbumFound) {
                string xpath = "upnp:replayGainAlbum";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:replayGainAlbum", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ReplayGainAlbum.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iReplayGainAlbumPeakFound) {
                string xpath = "upnp:replayGainAlbumPeak";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:replayGainAlbumPeak", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ReplayGainAlbumPeak.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iReplayGainTrackFound) {
                string xpath = "upnp:replayGainTrack";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:replayGainTrack", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ReplayGainTrack.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iReplayGainTrackPeakFound) {
                string xpath = "upnp:replayGainTrackPeak";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:replayGainTrackPeak", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ReplayGainTrackPeak.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iArtistFound = false;
        private bool iArtistSearched = false;
        private List<artist> iArtist = new List<artist>();
        private bool iAlbumFound = false;
        private bool iAlbumSearched = false;
        private List<string> iAlbum = new List<string>();
        private bool iAuthorFound = false;
        private bool iAuthorSearched = false;
        private List<author> iAuthor = new List<author>();
        private bool iOriginalTrackNumberFound = false;
        private bool iOriginalTrackNumberSearched = false;
        private int iOriginalTrackNumber;
        private bool iPlaylistFound = false;
        private bool iPlaylistSearched = false;
        private List<string> iPlaylist = new List<string>();
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iContributorFound = false;
        private bool iContributorSearched = false;
        private List<string> iContributor = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
        private bool iOriginalDiscNumberFound = false;
        private bool iOriginalDiscNumberSearched = false;
        private int iOriginalDiscNumber;
        private bool iOriginalDiscCountFound = false;
        private bool iOriginalDiscCountSearched = false;
        private int iOriginalDiscCount;
        private bool iReplayGainAlbumFound = false;
        private bool iReplayGainAlbumSearched = false;
        private string iReplayGainAlbum;
        private bool iReplayGainAlbumPeakFound = false;
        private bool iReplayGainAlbumPeakSearched = false;
        private string iReplayGainAlbumPeak;
        private bool iReplayGainTrackFound = false;
        private bool iReplayGainTrackSearched = false;
        private string iReplayGainTrack;
        private bool iReplayGainTrackPeakFound = false;
        private bool iReplayGainTrackPeakSearched = false;
        private string iReplayGainTrackPeak;
    }
    public class audioBroadcast : audioItem
    {
        public audioBroadcast() {
            iClassFound = true;
            iClass = "object.item.audioItem.audioBroadcast";
        }
        public audioBroadcast(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string Region {
            get {
                lock (iLock)
                {
                    PreCacheRegion();
                }
                return iRegion;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRegion = value;
                    iRegionFound = true;
                }
            }
        }
        private void PreCacheRegion() {
            lock (iLock)
            {
            if(!iRegionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:region", iXmlNsMan);
                    if (n != null) {
                        iRegion = n.InnerText;
                        iRegionFound = true;
                    }
                }
                iRegionSearched = true;
            }
            }
        }
        
        public virtual string RadioCallSign {
            get {
                lock (iLock)
                {
                    PreCacheRadioCallSign();
                }
                return iRadioCallSign;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRadioCallSign = value;
                    iRadioCallSignFound = true;
                }
            }
        }
        private void PreCacheRadioCallSign() {
            lock (iLock)
            {
            if(!iRadioCallSignSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:radioCallSign", iXmlNsMan);
                    if (n != null) {
                        iRadioCallSign = n.InnerText;
                        iRadioCallSignFound = true;
                    }
                }
                iRadioCallSignSearched = true;
            }
            }
        }
        
        public virtual string RadioStatioID {
            get {
                lock (iLock)
                {
                    PreCacheRadioStatioID();
                }
                return iRadioStatioID;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRadioStatioID = value;
                    iRadioStatioIDFound = true;
                }
            }
        }
        private void PreCacheRadioStatioID() {
            lock (iLock)
            {
            if(!iRadioStatioIDSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:radioStationID", iXmlNsMan);
                    if (n != null) {
                        iRadioStatioID = n.InnerText;
                        iRadioStatioIDFound = true;
                    }
                }
                iRadioStatioIDSearched = true;
            }
            }
        }
        
        public virtual string RadioBand {
            get {
                lock (iLock)
                {
                    PreCacheRadioBand();
                }
                return iRadioBand;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRadioBand = value;
                    iRadioBandFound = true;
                }
            }
        }
        private void PreCacheRadioBand() {
            lock (iLock)
            {
            if(!iRadioBandSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:radioBand", iXmlNsMan);
                    if (n != null) {
                        iRadioBand = n.InnerText;
                        iRadioBandFound = true;
                    }
                }
                iRadioBandSearched = true;
            }
            }
        }
        
        public virtual int ChannelNr {
            get {
                lock (iLock)
                {
                    PreCacheChannelNr();
                }
                return iChannelNr;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iChannelNr = value;
                    iChannelNrFound = true;
                }
            }
        }
        private void PreCacheChannelNr() {
            lock (iLock)
            {
            if(!iChannelNrSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:channelNr", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iChannelNr = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iChannelNrFound = true;
                    }
                }
                iChannelNrSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheRegion();
			PreCacheRadioCallSign();
			PreCacheRadioStatioID();
			PreCacheRadioBand();
			PreCacheChannelNr();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iRegionFound) {
                string xpath = "upnp:region";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:region", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Region.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRadioCallSignFound) {
                string xpath = "upnp:radioCallSign";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:radioCallSign", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(RadioCallSign.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRadioStatioIDFound) {
                string xpath = "upnp:radioStationID";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:radioStationID", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(RadioStatioID.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRadioBandFound) {
                string xpath = "upnp:radioBand";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:radioBand", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(RadioBand.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iChannelNrFound) {
                string xpath = "upnp:channelNr";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:channelNr", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ChannelNr.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iRegionFound = false;
        private bool iRegionSearched = false;
        private string iRegion;
        private bool iRadioCallSignFound = false;
        private bool iRadioCallSignSearched = false;
        private string iRadioCallSign;
        private bool iRadioStatioIDFound = false;
        private bool iRadioStatioIDSearched = false;
        private string iRadioStatioID;
        private bool iRadioBandFound = false;
        private bool iRadioBandSearched = false;
        private string iRadioBand;
        private bool iChannelNrFound = false;
        private bool iChannelNrSearched = false;
        private int iChannelNr;
    }
    public class audioBook : audioItem
    {
        public audioBook() {
            iClassFound = true;
            iClass = "object.item.audioItem.audioBook";
        }
        public audioBook(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual List<string> Producer {
            get {
                lock (iLock)
                {
                    PreCacheProducer();
                }
                return iProducer;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProducer = value;
                    iProducerFound = true;
                }
            }
        }
        private void PreCacheProducer() {
            lock (iLock)
            {
            if(!iProducerSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:producer", iXmlNsMan)) {
                        iProducer.Add(n.InnerText);
                    }
                }
                iProducerSearched = true;
                iProducerFound = true;
            }
            }
        }
        
        public virtual List<string> Contributor {
            get {
                lock (iLock)
                {
                    PreCacheContributor();
                }
                return iContributor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iContributor = value;
                    iContributorFound = true;
                }
            }
        }
        private void PreCacheContributor() {
            lock (iLock)
            {
            if(!iContributorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:contributor", iXmlNsMan)) {
                        iContributor.Add(n.InnerText);
                    }
                }
                iContributorSearched = true;
                iContributorFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheStorageMedium();
			PreCacheProducer();
			PreCacheContributor();
			PreCacheDate();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iProducerFound) {
                foreach(string item in iProducer) {
                    string xpath = "upnp:producer";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:producer", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iContributorFound) {
                foreach(string item in iContributor) {
                    string xpath = "dc:contributor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:contributor", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iProducerFound = false;
        private bool iProducerSearched = false;
        private List<string> iProducer = new List<string>();
        private bool iContributorFound = false;
        private bool iContributorSearched = false;
        private List<string> iContributor = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
    }
    public class videoItem : item
    {
        public videoItem() {
            iClassFound = true;
            iClass = "object.item.videoItem";
        }
        public videoItem(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<string> Genre {
            get {
                lock (iLock)
                {
                    PreCacheGenre();
                }
                return iGenre;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iGenre = value;
                    iGenreFound = true;
                }
            }
        }
        private void PreCacheGenre() {
            lock (iLock)
            {
            if(!iGenreSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:genre", iXmlNsMan)) {
                        iGenre.Add(n.InnerText);
                    }
                }
                iGenreSearched = true;
                iGenreFound = true;
            }
            }
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Producer {
            get {
                lock (iLock)
                {
                    PreCacheProducer();
                }
                return iProducer;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProducer = value;
                    iProducerFound = true;
                }
            }
        }
        private void PreCacheProducer() {
            lock (iLock)
            {
            if(!iProducerSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:producer", iXmlNsMan)) {
                        iProducer.Add(n.InnerText);
                    }
                }
                iProducerSearched = true;
                iProducerFound = true;
            }
            }
        }
        
        public virtual string Rating {
            get {
                lock (iLock)
                {
                    PreCacheRating();
                }
                return iRating;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRating = value;
                    iRatingFound = true;
                }
            }
        }
        private void PreCacheRating() {
            lock (iLock)
            {
            if(!iRatingSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:rating", iXmlNsMan);
                    if (n != null) {
                        iRating = n.InnerText;
                        iRatingFound = true;
                    }
                }
                iRatingSearched = true;
            }
            }
        }
        
        public virtual List<actor> Actor {
            get {
                lock (iLock)
                {
                    PreCacheActor();
                }
                return iActor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iActor = value;
                    iActorFound = true;
                }
            }
        }
        private void PreCacheActor() {
            lock (iLock)
            {
            if(!iActorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:actor", iXmlNsMan)) {
                        iActor.Add(new actor(n));
                    }
                }
                iActorSearched = true;
                iActorFound = true;
            }
            }
        }
        
        public virtual List<string> Director {
            get {
                lock (iLock)
                {
                    PreCacheDirector();
                }
                return iDirector;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDirector = value;
                    iDirectorFound = true;
                }
            }
        }
        private void PreCacheDirector() {
            lock (iLock)
            {
            if(!iDirectorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:director", iXmlNsMan)) {
                        iDirector.Add(n.InnerText);
                    }
                }
                iDirectorSearched = true;
                iDirectorFound = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual string Publisher {
            get {
                lock (iLock)
                {
                    PreCachePublisher();
                }
                return iPublisher;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iPublisher = value;
                    iPublisherFound = true;
                }
            }
        }
        private void PreCachePublisher() {
            lock (iLock)
            {
            if(!iPublisherSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:publisher", iXmlNsMan);
                    if (n != null) {
                        iPublisher = n.InnerText;
                        iPublisherFound = true;
                    }
                }
                iPublisherSearched = true;
            }
            }
        }
        
        public virtual string Language {
            get {
                lock (iLock)
                {
                    PreCacheLanguage();
                }
                return iLanguage;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLanguage = value;
                    iLanguageFound = true;
                }
            }
        }
        private void PreCacheLanguage() {
            lock (iLock)
            {
            if(!iLanguageSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:language", iXmlNsMan);
                    if (n != null) {
                        iLanguage = n.InnerText;
                        iLanguageFound = true;
                    }
                }
                iLanguageSearched = true;
            }
            }
        }
        
        public virtual List<string> Relation {
            get {
                lock (iLock)
                {
                    PreCacheRelation();
                }
                return iRelation;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRelation = value;
                    iRelationFound = true;
                }
            }
        }
        private void PreCacheRelation() {
            lock (iLock)
            {
            if(!iRelationSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:relation", iXmlNsMan)) {
                        iRelation.Add(n.InnerText);
                    }
                }
                iRelationSearched = true;
                iRelationFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheGenre();
			PreCacheLongDescription();
			PreCacheProducer();
			PreCacheRating();
			PreCacheActor();
			foreach(actor i in iActor) {
			    i.PreCache();
			}
			PreCacheDirector();
			PreCacheDescription();
			PreCachePublisher();
			PreCacheLanguage();
			PreCacheRelation();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iGenreFound) {
                foreach(string item in iGenre) {
                    string xpath = "upnp:genre";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:genre", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iProducerFound) {
                foreach(string item in iProducer) {
                    string xpath = "upnp:producer";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:producer", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iRatingFound) {
                string xpath = "upnp:rating";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:rating", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Rating.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iActorFound) {
                foreach(actor item in iActor) {
                    string xpath = "upnp:actor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:actor", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDirectorFound) {
                foreach(string item in iDirector) {
                    string xpath = "upnp:director";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:director", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iPublisherFound) {
                string xpath = "dc:publisher";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:publisher", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Publisher.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iLanguageFound) {
                string xpath = "dc:language";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:language", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Language.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRelationFound) {
                foreach(string item in iRelation) {
                    string xpath = "dc:relation";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:relation", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iGenreFound = false;
        private bool iGenreSearched = false;
        private List<string> iGenre = new List<string>();
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iProducerFound = false;
        private bool iProducerSearched = false;
        private List<string> iProducer = new List<string>();
        private bool iRatingFound = false;
        private bool iRatingSearched = false;
        private string iRating;
        private bool iActorFound = false;
        private bool iActorSearched = false;
        private List<actor> iActor = new List<actor>();
        private bool iDirectorFound = false;
        private bool iDirectorSearched = false;
        private List<string> iDirector = new List<string>();
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iPublisherFound = false;
        private bool iPublisherSearched = false;
        private string iPublisher;
        private bool iLanguageFound = false;
        private bool iLanguageSearched = false;
        private string iLanguage;
        private bool iRelationFound = false;
        private bool iRelationSearched = false;
        private List<string> iRelation = new List<string>();
    }
    public class movie : videoItem
    {
        public movie() {
            iClassFound = true;
            iClass = "object.item.videoItem.movie";
        }
        public movie(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual int DvdRegionCode {
            get {
                lock (iLock)
                {
                    PreCacheDvdRegionCode();
                }
                return iDvdRegionCode;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDvdRegionCode = value;
                    iDvdRegionCodeFound = true;
                }
            }
        }
        private void PreCacheDvdRegionCode() {
            lock (iLock)
            {
            if(!iDvdRegionCodeSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:DVDRegionCode", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iDvdRegionCode = int.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iDvdRegionCodeFound = true;
                    }
                }
                iDvdRegionCodeSearched = true;
            }
            }
        }
        
        public virtual string ChannelName {
            get {
                lock (iLock)
                {
                    PreCacheChannelName();
                }
                return iChannelName;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iChannelName = value;
                    iChannelNameFound = true;
                }
            }
        }
        private void PreCacheChannelName() {
            lock (iLock)
            {
            if(!iChannelNameSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:channelName", iXmlNsMan);
                    if (n != null) {
                        iChannelName = n.InnerText;
                        iChannelNameFound = true;
                    }
                }
                iChannelNameSearched = true;
            }
            }
        }
        
        public virtual List<string> ScheduledStartTime {
            get {
                lock (iLock)
                {
                    PreCacheScheduledStartTime();
                }
                return iScheduledStartTime;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iScheduledStartTime = value;
                    iScheduledStartTimeFound = true;
                }
            }
        }
        private void PreCacheScheduledStartTime() {
            lock (iLock)
            {
            if(!iScheduledStartTimeSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:scheduledStartTime", iXmlNsMan)) {
                        iScheduledStartTime.Add(n.InnerText);
                    }
                }
                iScheduledStartTimeSearched = true;
                iScheduledStartTimeFound = true;
            }
            }
        }
        
        public virtual List<string> ScheduledEndTime {
            get {
                lock (iLock)
                {
                    PreCacheScheduledEndTime();
                }
                return iScheduledEndTime;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iScheduledEndTime = value;
                    iScheduledEndTimeFound = true;
                }
            }
        }
        private void PreCacheScheduledEndTime() {
            lock (iLock)
            {
            if(!iScheduledEndTimeSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:scheduledEndTime", iXmlNsMan)) {
                        iScheduledEndTime.Add(n.InnerText);
                    }
                }
                iScheduledEndTimeSearched = true;
                iScheduledEndTimeFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheStorageMedium();
			PreCacheDvdRegionCode();
			PreCacheChannelName();
			PreCacheScheduledStartTime();
			PreCacheScheduledEndTime();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDvdRegionCodeFound) {
                string xpath = "upnp:DVDRegionCode";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:DVDRegionCode", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(DvdRegionCode.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iChannelNameFound) {
                string xpath = "upnp:channelName";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:channelName", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ChannelName.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iScheduledStartTimeFound) {
                foreach(string item in iScheduledStartTime) {
                    string xpath = "upnp:scheduledStartTime";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:scheduledStartTime", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iScheduledEndTimeFound) {
                foreach(string item in iScheduledEndTime) {
                    string xpath = "upnp:scheduledEndTime";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:scheduledEndTime", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iDvdRegionCodeFound = false;
        private bool iDvdRegionCodeSearched = false;
        private int iDvdRegionCode;
        private bool iChannelNameFound = false;
        private bool iChannelNameSearched = false;
        private string iChannelName;
        private bool iScheduledStartTimeFound = false;
        private bool iScheduledStartTimeSearched = false;
        private List<string> iScheduledStartTime = new List<string>();
        private bool iScheduledEndTimeFound = false;
        private bool iScheduledEndTimeSearched = false;
        private List<string> iScheduledEndTime = new List<string>();
    }
    public class videoBroadcast : videoItem
    {
        public videoBroadcast() {
            iClassFound = true;
            iClass = "object.item.videoItem.videoBroadcast";
        }
        public videoBroadcast(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string Region {
            get {
                lock (iLock)
                {
                    PreCacheRegion();
                }
                return iRegion;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRegion = value;
                    iRegionFound = true;
                }
            }
        }
        private void PreCacheRegion() {
            lock (iLock)
            {
            if(!iRegionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:region", iXmlNsMan);
                    if (n != null) {
                        iRegion = n.InnerText;
                        iRegionFound = true;
                    }
                }
                iRegionSearched = true;
            }
            }
        }
        
        public virtual string ChannelNr {
            get {
                lock (iLock)
                {
                    PreCacheChannelNr();
                }
                return iChannelNr;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iChannelNr = value;
                    iChannelNrFound = true;
                }
            }
        }
        private void PreCacheChannelNr() {
            lock (iLock)
            {
            if(!iChannelNrSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:channelNr", iXmlNsMan);
                    if (n != null) {
                        iChannelNr = n.InnerText;
                        iChannelNrFound = true;
                    }
                }
                iChannelNrSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheRegion();
			PreCacheChannelNr();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iRegionFound) {
                string xpath = "upnp:region";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:region", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Region.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iChannelNrFound) {
                string xpath = "upnp:channelNr";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:channelNr", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ChannelNr.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iRegionFound = false;
        private bool iRegionSearched = false;
        private string iRegion;
        private bool iChannelNrFound = false;
        private bool iChannelNrSearched = false;
        private string iChannelNr;
    }
    public class musicVideoClip : videoItem
    {
        public musicVideoClip() {
            iClassFound = true;
            iClass = "object.item.videoItem.musicVideoClip";
        }
        public musicVideoClip(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<artist> Artist {
            get {
                lock (iLock)
                {
                    PreCacheArtist();
                }
                return iArtist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtist = value;
                    iArtistFound = true;
                }
            }
        }
        private void PreCacheArtist() {
            lock (iLock)
            {
            if(!iArtistSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:artist", iXmlNsMan)) {
                        iArtist.Add(new artist(n));
                    }
                }
                iArtistSearched = true;
                iArtistFound = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual List<string> Album {
            get {
                lock (iLock)
                {
                    PreCacheAlbum();
                }
                return iAlbum;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAlbum = value;
                    iAlbumFound = true;
                }
            }
        }
        private void PreCacheAlbum() {
            lock (iLock)
            {
            if(!iAlbumSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:album", iXmlNsMan)) {
                        iAlbum.Add(n.InnerText);
                    }
                }
                iAlbumSearched = true;
                iAlbumFound = true;
            }
            }
        }
        
        public virtual List<string> ScheduledStartTime {
            get {
                lock (iLock)
                {
                    PreCacheScheduledStartTime();
                }
                return iScheduledStartTime;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iScheduledStartTime = value;
                    iScheduledStartTimeFound = true;
                }
            }
        }
        private void PreCacheScheduledStartTime() {
            lock (iLock)
            {
            if(!iScheduledStartTimeSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:scheduledStartTime", iXmlNsMan)) {
                        iScheduledStartTime.Add(n.InnerText);
                    }
                }
                iScheduledStartTimeSearched = true;
                iScheduledStartTimeFound = true;
            }
            }
        }
        
        public virtual List<string> ScheduledEndTime {
            get {
                lock (iLock)
                {
                    PreCacheScheduledEndTime();
                }
                return iScheduledEndTime;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iScheduledEndTime = value;
                    iScheduledEndTimeFound = true;
                }
            }
        }
        private void PreCacheScheduledEndTime() {
            lock (iLock)
            {
            if(!iScheduledEndTimeSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:scheduledEndTime", iXmlNsMan)) {
                        iScheduledEndTime.Add(n.InnerText);
                    }
                }
                iScheduledEndTimeSearched = true;
                iScheduledEndTimeFound = true;
            }
            }
        }
        
        public virtual List<string> Contributor {
            get {
                lock (iLock)
                {
                    PreCacheContributor();
                }
                return iContributor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iContributor = value;
                    iContributorFound = true;
                }
            }
        }
        private void PreCacheContributor() {
            lock (iLock)
            {
            if(!iContributorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:contributor", iXmlNsMan)) {
                        iContributor.Add(n.InnerText);
                    }
                }
                iContributorSearched = true;
                iContributorFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheArtist();
			foreach(artist i in iArtist) {
			    i.PreCache();
			}
			PreCacheStorageMedium();
			PreCacheAlbum();
			PreCacheScheduledStartTime();
			PreCacheScheduledEndTime();
			PreCacheContributor();
			PreCacheDate();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iArtistFound) {
                foreach(artist item in iArtist) {
                    string xpath = "upnp:artist";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:artist", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iAlbumFound) {
                foreach(string item in iAlbum) {
                    string xpath = "upnp:album";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:album", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iScheduledStartTimeFound) {
                foreach(string item in iScheduledStartTime) {
                    string xpath = "upnp:scheduledStartTime";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:scheduledStartTime", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iScheduledEndTimeFound) {
                foreach(string item in iScheduledEndTime) {
                    string xpath = "upnp:scheduledEndTime";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:scheduledEndTime", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iContributorFound) {
                foreach(string item in iContributor) {
                    string xpath = "dc:contributor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:contributor", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iArtistFound = false;
        private bool iArtistSearched = false;
        private List<artist> iArtist = new List<artist>();
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iAlbumFound = false;
        private bool iAlbumSearched = false;
        private List<string> iAlbum = new List<string>();
        private bool iScheduledStartTimeFound = false;
        private bool iScheduledStartTimeSearched = false;
        private List<string> iScheduledStartTime = new List<string>();
        private bool iScheduledEndTimeFound = false;
        private bool iScheduledEndTimeSearched = false;
        private List<string> iScheduledEndTime = new List<string>();
        private bool iContributorFound = false;
        private bool iContributorSearched = false;
        private List<string> iContributor = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
    }
    public class imageItem : item
    {
        public imageItem() {
            iClassFound = true;
            iClass = "object.item.imageItem";
        }
        public imageItem(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual string Rating {
            get {
                lock (iLock)
                {
                    PreCacheRating();
                }
                return iRating;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRating = value;
                    iRatingFound = true;
                }
            }
        }
        private void PreCacheRating() {
            lock (iLock)
            {
            if(!iRatingSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:rating", iXmlNsMan);
                    if (n != null) {
                        iRating = n.InnerText;
                        iRatingFound = true;
                    }
                }
                iRatingSearched = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Publisher {
            get {
                lock (iLock)
                {
                    PreCachePublisher();
                }
                return iPublisher;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iPublisher = value;
                    iPublisherFound = true;
                }
            }
        }
        private void PreCachePublisher() {
            lock (iLock)
            {
            if(!iPublisherSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:publisher", iXmlNsMan)) {
                        iPublisher.Add(n.InnerText);
                    }
                }
                iPublisherSearched = true;
                iPublisherFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        
        public virtual List<string> Rights {
            get {
                lock (iLock)
                {
                    PreCacheRights();
                }
                return iRights;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRights = value;
                    iRightsFound = true;
                }
            }
        }
        private void PreCacheRights() {
            lock (iLock)
            {
            if(!iRightsSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:rights", iXmlNsMan)) {
                        iRights.Add(n.InnerText);
                    }
                }
                iRightsSearched = true;
                iRightsFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheLongDescription();
			PreCacheStorageMedium();
			PreCacheRating();
			PreCacheDescription();
			PreCachePublisher();
			PreCacheDate();
			PreCacheRights();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRatingFound) {
                string xpath = "upnp:rating";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:rating", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Rating.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iPublisherFound) {
                foreach(string item in iPublisher) {
                    string xpath = "dc:publisher";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:publisher", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRightsFound) {
                foreach(string item in iRights) {
                    string xpath = "dc:rights";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:rights", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iRatingFound = false;
        private bool iRatingSearched = false;
        private string iRating;
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iPublisherFound = false;
        private bool iPublisherSearched = false;
        private List<string> iPublisher = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
        private bool iRightsFound = false;
        private bool iRightsSearched = false;
        private List<string> iRights = new List<string>();
    }
    public class photo : imageItem
    {
        public photo() {
            iClassFound = true;
            iClass = "object.item.imageItem.photo";
        }
        public photo(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<string> Album {
            get {
                lock (iLock)
                {
                    PreCacheAlbum();
                }
                return iAlbum;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAlbum = value;
                    iAlbumFound = true;
                }
            }
        }
        private void PreCacheAlbum() {
            lock (iLock)
            {
            if(!iAlbumSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:album", iXmlNsMan)) {
                        iAlbum.Add(n.InnerText);
                    }
                }
                iAlbumSearched = true;
                iAlbumFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheAlbum();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iAlbumFound) {
                foreach(string item in iAlbum) {
                    string xpath = "upnp:album";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:album", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iAlbumFound = false;
        private bool iAlbumSearched = false;
        private List<string> iAlbum = new List<string>();
    }
    public class playlistItem : item
    {
        public playlistItem() {
            iClassFound = true;
            iClass = "object.item.playlistItem";
        }
        public playlistItem(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<artist> Artist {
            get {
                lock (iLock)
                {
                    PreCacheArtist();
                }
                return iArtist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtist = value;
                    iArtistFound = true;
                }
            }
        }
        private void PreCacheArtist() {
            lock (iLock)
            {
            if(!iArtistSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:artist", iXmlNsMan)) {
                        iArtist.Add(new artist(n));
                    }
                }
                iArtistSearched = true;
                iArtistFound = true;
            }
            }
        }
        
        public virtual List<string> Genre {
            get {
                lock (iLock)
                {
                    PreCacheGenre();
                }
                return iGenre;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iGenre = value;
                    iGenreFound = true;
                }
            }
        }
        private void PreCacheGenre() {
            lock (iLock)
            {
            if(!iGenreSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:genre", iXmlNsMan)) {
                        iGenre.Add(n.InnerText);
                    }
                }
                iGenreSearched = true;
                iGenreFound = true;
            }
            }
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        
        public virtual string Language {
            get {
                lock (iLock)
                {
                    PreCacheLanguage();
                }
                return iLanguage;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLanguage = value;
                    iLanguageFound = true;
                }
            }
        }
        private void PreCacheLanguage() {
            lock (iLock)
            {
            if(!iLanguageSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:language", iXmlNsMan);
                    if (n != null) {
                        iLanguage = n.InnerText;
                        iLanguageFound = true;
                    }
                }
                iLanguageSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheArtist();
			foreach(artist i in iArtist) {
			    i.PreCache();
			}
			PreCacheGenre();
			PreCacheLongDescription();
			PreCacheStorageMedium();
			PreCacheDescription();
			PreCacheDate();
			PreCacheLanguage();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iArtistFound) {
                foreach(artist item in iArtist) {
                    string xpath = "upnp:artist";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:artist", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iGenreFound) {
                foreach(string item in iGenre) {
                    string xpath = "upnp:genre";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:genre", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iLanguageFound) {
                string xpath = "dc:language";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:language", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Language.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iArtistFound = false;
        private bool iArtistSearched = false;
        private List<artist> iArtist = new List<artist>();
        private bool iGenreFound = false;
        private bool iGenreSearched = false;
        private List<string> iGenre = new List<string>();
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
        private bool iLanguageFound = false;
        private bool iLanguageSearched = false;
        private string iLanguage;
    }
    public class textItem : item
    {
        public textItem() {
            iClassFound = true;
            iClass = "object.item.textItem";
        }
        public textItem(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<author> Author {
            get {
                lock (iLock)
                {
                    PreCacheAuthor();
                }
                return iAuthor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iAuthor = value;
                    iAuthorFound = true;
                }
            }
        }
        private void PreCacheAuthor() {
            lock (iLock)
            {
            if(!iAuthorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:author", iXmlNsMan)) {
                        iAuthor.Add(new author(n));
                    }
                }
                iAuthorSearched = true;
                iAuthorFound = true;
            }
            }
        }
        
        public virtual string Protection {
            get {
                lock (iLock)
                {
                    PreCacheProtection();
                }
                return iProtection;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProtection = value;
                    iProtectionFound = true;
                }
            }
        }
        private void PreCacheProtection() {
            lock (iLock)
            {
            if(!iProtectionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:protection", iXmlNsMan);
                    if (n != null) {
                        iProtection = n.InnerText;
                        iProtectionFound = true;
                    }
                }
                iProtectionSearched = true;
            }
            }
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual string Rating {
            get {
                lock (iLock)
                {
                    PreCacheRating();
                }
                return iRating;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRating = value;
                    iRatingFound = true;
                }
            }
        }
        private void PreCacheRating() {
            lock (iLock)
            {
            if(!iRatingSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:rating", iXmlNsMan);
                    if (n != null) {
                        iRating = n.InnerText;
                        iRatingFound = true;
                    }
                }
                iRatingSearched = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Publisher {
            get {
                lock (iLock)
                {
                    PreCachePublisher();
                }
                return iPublisher;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iPublisher = value;
                    iPublisherFound = true;
                }
            }
        }
        private void PreCachePublisher() {
            lock (iLock)
            {
            if(!iPublisherSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:publisher", iXmlNsMan)) {
                        iPublisher.Add(n.InnerText);
                    }
                }
                iPublisherSearched = true;
                iPublisherFound = true;
            }
            }
        }
        
        public virtual List<string> Contributor {
            get {
                lock (iLock)
                {
                    PreCacheContributor();
                }
                return iContributor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iContributor = value;
                    iContributorFound = true;
                }
            }
        }
        private void PreCacheContributor() {
            lock (iLock)
            {
            if(!iContributorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:contributor", iXmlNsMan)) {
                        iContributor.Add(n.InnerText);
                    }
                }
                iContributorSearched = true;
                iContributorFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        
        public virtual List<string> Relation {
            get {
                lock (iLock)
                {
                    PreCacheRelation();
                }
                return iRelation;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRelation = value;
                    iRelationFound = true;
                }
            }
        }
        private void PreCacheRelation() {
            lock (iLock)
            {
            if(!iRelationSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:relation", iXmlNsMan)) {
                        iRelation.Add(n.InnerText);
                    }
                }
                iRelationSearched = true;
                iRelationFound = true;
            }
            }
        }
        
        public virtual string Language {
            get {
                lock (iLock)
                {
                    PreCacheLanguage();
                }
                return iLanguage;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLanguage = value;
                    iLanguageFound = true;
                }
            }
        }
        private void PreCacheLanguage() {
            lock (iLock)
            {
            if(!iLanguageSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:language", iXmlNsMan);
                    if (n != null) {
                        iLanguage = n.InnerText;
                        iLanguageFound = true;
                    }
                }
                iLanguageSearched = true;
            }
            }
        }
        
        public virtual List<string> Rights {
            get {
                lock (iLock)
                {
                    PreCacheRights();
                }
                return iRights;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRights = value;
                    iRightsFound = true;
                }
            }
        }
        private void PreCacheRights() {
            lock (iLock)
            {
            if(!iRightsSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:rights", iXmlNsMan)) {
                        iRights.Add(n.InnerText);
                    }
                }
                iRightsSearched = true;
                iRightsFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheAuthor();
			foreach(author i in iAuthor) {
			    i.PreCache();
			}
			PreCacheProtection();
			PreCacheLongDescription();
			PreCacheStorageMedium();
			PreCacheRating();
			PreCacheDescription();
			PreCachePublisher();
			PreCacheContributor();
			PreCacheDate();
			PreCacheRelation();
			PreCacheLanguage();
			PreCacheRights();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iAuthorFound) {
                foreach(author item in iAuthor) {
                    string xpath = "upnp:author";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:author", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iProtectionFound) {
                string xpath = "upnp:protection";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:protection", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Protection.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRatingFound) {
                string xpath = "upnp:rating";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:rating", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Rating.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iPublisherFound) {
                foreach(string item in iPublisher) {
                    string xpath = "dc:publisher";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:publisher", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iContributorFound) {
                foreach(string item in iContributor) {
                    string xpath = "dc:contributor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:contributor", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRelationFound) {
                foreach(string item in iRelation) {
                    string xpath = "dc:relation";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:relation", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iLanguageFound) {
                string xpath = "dc:language";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:language", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Language.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRightsFound) {
                foreach(string item in iRights) {
                    string xpath = "dc:rights";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:rights", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iAuthorFound = false;
        private bool iAuthorSearched = false;
        private List<author> iAuthor = new List<author>();
        private bool iProtectionFound = false;
        private bool iProtectionSearched = false;
        private string iProtection;
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iRatingFound = false;
        private bool iRatingSearched = false;
        private string iRating;
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iPublisherFound = false;
        private bool iPublisherSearched = false;
        private List<string> iPublisher = new List<string>();
        private bool iContributorFound = false;
        private bool iContributorSearched = false;
        private List<string> iContributor = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
        private bool iRelationFound = false;
        private bool iRelationSearched = false;
        private List<string> iRelation = new List<string>();
        private bool iLanguageFound = false;
        private bool iLanguageSearched = false;
        private string iLanguage;
        private bool iRightsFound = false;
        private bool iRightsSearched = false;
        private List<string> iRights = new List<string>();
    }
    public class album : container
    {
        public album() {
            iClassFound = true;
            iClass = "object.container.album";
        }
        public album(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Publisher {
            get {
                lock (iLock)
                {
                    PreCachePublisher();
                }
                return iPublisher;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iPublisher = value;
                    iPublisherFound = true;
                }
            }
        }
        private void PreCachePublisher() {
            lock (iLock)
            {
            if(!iPublisherSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:publisher", iXmlNsMan)) {
                        iPublisher.Add(n.InnerText);
                    }
                }
                iPublisherSearched = true;
                iPublisherFound = true;
            }
            }
        }
        
        public virtual List<string> Contributor {
            get {
                lock (iLock)
                {
                    PreCacheContributor();
                }
                return iContributor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iContributor = value;
                    iContributorFound = true;
                }
            }
        }
        private void PreCacheContributor() {
            lock (iLock)
            {
            if(!iContributorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:contributor", iXmlNsMan)) {
                        iContributor.Add(n.InnerText);
                    }
                }
                iContributorSearched = true;
                iContributorFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        
        public virtual List<string> Relation {
            get {
                lock (iLock)
                {
                    PreCacheRelation();
                }
                return iRelation;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRelation = value;
                    iRelationFound = true;
                }
            }
        }
        private void PreCacheRelation() {
            lock (iLock)
            {
            if(!iRelationSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:relation", iXmlNsMan)) {
                        iRelation.Add(n.InnerText);
                    }
                }
                iRelationSearched = true;
                iRelationFound = true;
            }
            }
        }
        
        public virtual List<string> Rights {
            get {
                lock (iLock)
                {
                    PreCacheRights();
                }
                return iRights;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRights = value;
                    iRightsFound = true;
                }
            }
        }
        private void PreCacheRights() {
            lock (iLock)
            {
            if(!iRightsSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:rights", iXmlNsMan)) {
                        iRights.Add(n.InnerText);
                    }
                }
                iRightsSearched = true;
                iRightsFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheStorageMedium();
			PreCacheLongDescription();
			PreCacheDescription();
			PreCachePublisher();
			PreCacheContributor();
			PreCacheDate();
			PreCacheRelation();
			PreCacheRights();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iPublisherFound) {
                foreach(string item in iPublisher) {
                    string xpath = "dc:publisher";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:publisher", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iContributorFound) {
                foreach(string item in iContributor) {
                    string xpath = "dc:contributor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:contributor", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRelationFound) {
                foreach(string item in iRelation) {
                    string xpath = "dc:relation";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:relation", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iRightsFound) {
                foreach(string item in iRights) {
                    string xpath = "dc:rights";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:rights", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iPublisherFound = false;
        private bool iPublisherSearched = false;
        private List<string> iPublisher = new List<string>();
        private bool iContributorFound = false;
        private bool iContributorSearched = false;
        private List<string> iContributor = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
        private bool iRelationFound = false;
        private bool iRelationSearched = false;
        private List<string> iRelation = new List<string>();
        private bool iRightsFound = false;
        private bool iRightsSearched = false;
        private List<string> iRights = new List<string>();
    }
    public class musicAlbum : album
    {
        public musicAlbum() {
            iClassFound = true;
            iClass = "object.container.album.musicAlbum";
        }
        public musicAlbum(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<artist> Artist {
            get {
                lock (iLock)
                {
                    PreCacheArtist();
                }
                return iArtist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtist = value;
                    iArtistFound = true;
                }
            }
        }
        private void PreCacheArtist() {
            lock (iLock)
            {
            if(!iArtistSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:artist", iXmlNsMan)) {
                        iArtist.Add(new artist(n));
                    }
                }
                iArtistSearched = true;
                iArtistFound = true;
            }
            }
        }
        
        public virtual List<string> Genre {
            get {
                lock (iLock)
                {
                    PreCacheGenre();
                }
                return iGenre;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iGenre = value;
                    iGenreFound = true;
                }
            }
        }
        private void PreCacheGenre() {
            lock (iLock)
            {
            if(!iGenreSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:genre", iXmlNsMan)) {
                        iGenre.Add(n.InnerText);
                    }
                }
                iGenreSearched = true;
                iGenreFound = true;
            }
            }
        }
        
        public virtual List<string> Producer {
            get {
                lock (iLock)
                {
                    PreCacheProducer();
                }
                return iProducer;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProducer = value;
                    iProducerFound = true;
                }
            }
        }
        private void PreCacheProducer() {
            lock (iLock)
            {
            if(!iProducerSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:producer", iXmlNsMan)) {
                        iProducer.Add(n.InnerText);
                    }
                }
                iProducerSearched = true;
                iProducerFound = true;
            }
            }
        }
        
        public virtual string Toc {
            get {
                lock (iLock)
                {
                    PreCacheToc();
                }
                return iToc;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iToc = value;
                    iTocFound = true;
                }
            }
        }
        private void PreCacheToc() {
            lock (iLock)
            {
            if(!iTocSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:toc", iXmlNsMan);
                    if (n != null) {
                        iToc = n.InnerText;
                        iTocFound = true;
                    }
                }
                iTocSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheArtist();
			foreach(artist i in iArtist) {
			    i.PreCache();
			}
			PreCacheGenre();
			PreCacheProducer();
			PreCacheToc();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iArtistFound) {
                foreach(artist item in iArtist) {
                    string xpath = "upnp:artist";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:artist", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iGenreFound) {
                foreach(string item in iGenre) {
                    string xpath = "upnp:genre";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:genre", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iProducerFound) {
                foreach(string item in iProducer) {
                    string xpath = "upnp:producer";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:producer", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iTocFound) {
                string xpath = "upnp:toc";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:toc", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Toc.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iArtistFound = false;
        private bool iArtistSearched = false;
        private List<artist> iArtist = new List<artist>();
        private bool iGenreFound = false;
        private bool iGenreSearched = false;
        private List<string> iGenre = new List<string>();
        private bool iProducerFound = false;
        private bool iProducerSearched = false;
        private List<string> iProducer = new List<string>();
        private bool iTocFound = false;
        private bool iTocSearched = false;
        private string iToc;
    }
    public class photoAlbum : album
    {
        public photoAlbum() {
            iClassFound = true;
            iClass = "object.container.album.photoAlbum";
        }
        public photoAlbum(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        

        internal override void PreCache() {
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
        }
        
    }
    public class genre : container
    {
        public genre() {
            iClassFound = true;
            iClass = "object.container.genre";
        }
        public genre(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheLongDescription();
			PreCacheDescription();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
    }
    public class musicGenre : genre
    {
        public musicGenre() {
            iClassFound = true;
            iClass = "object.container.genre.musicGenre";
        }
        public musicGenre(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        

        internal override void PreCache() {
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
        }
        
    }
    public class movieGenre : genre
    {
        public movieGenre() {
            iClassFound = true;
            iClass = "object.container.genre.movieGenre";
        }
        public movieGenre(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        

        internal override void PreCache() {
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
        }
        
    }
    public class playlistContainer : container
    {
        public playlistContainer() {
            iClassFound = true;
            iClass = "object.container.playlistContainer";
        }
        public playlistContainer(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<artist> Artist {
            get {
                lock (iLock)
                {
                    PreCacheArtist();
                }
                return iArtist;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtist = value;
                    iArtistFound = true;
                }
            }
        }
        private void PreCacheArtist() {
            lock (iLock)
            {
            if(!iArtistSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:artist", iXmlNsMan)) {
                        iArtist.Add(new artist(n));
                    }
                }
                iArtistSearched = true;
                iArtistFound = true;
            }
            }
        }
        
        public virtual List<string> Genre {
            get {
                lock (iLock)
                {
                    PreCacheGenre();
                }
                return iGenre;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iGenre = value;
                    iGenreFound = true;
                }
            }
        }
        private void PreCacheGenre() {
            lock (iLock)
            {
            if(!iGenreSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:genre", iXmlNsMan)) {
                        iGenre.Add(n.InnerText);
                    }
                }
                iGenreSearched = true;
                iGenreFound = true;
            }
            }
        }
        
        public virtual string LongDescription {
            get {
                lock (iLock)
                {
                    PreCacheLongDescription();
                }
                return iLongDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLongDescription = value;
                    iLongDescriptionFound = true;
                }
            }
        }
        private void PreCacheLongDescription() {
            lock (iLock)
            {
            if(!iLongDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:longDescription", iXmlNsMan);
                    if (n != null) {
                        iLongDescription = n.InnerText;
                        iLongDescriptionFound = true;
                    }
                }
                iLongDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Producer {
            get {
                lock (iLock)
                {
                    PreCacheProducer();
                }
                return iProducer;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iProducer = value;
                    iProducerFound = true;
                }
            }
        }
        private void PreCacheProducer() {
            lock (iLock)
            {
            if(!iProducerSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:producer", iXmlNsMan)) {
                        iProducer.Add(n.InnerText);
                    }
                }
                iProducerSearched = true;
                iProducerFound = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        
        public virtual string Description {
            get {
                lock (iLock)
                {
                    PreCacheDescription();
                }
                return iDescription;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDescription = value;
                    iDescriptionFound = true;
                }
            }
        }
        private void PreCacheDescription() {
            lock (iLock)
            {
            if(!iDescriptionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:description", iXmlNsMan);
                    if (n != null) {
                        iDescription = n.InnerText;
                        iDescriptionFound = true;
                    }
                }
                iDescriptionSearched = true;
            }
            }
        }
        
        public virtual List<string> Contributor {
            get {
                lock (iLock)
                {
                    PreCacheContributor();
                }
                return iContributor;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iContributor = value;
                    iContributorFound = true;
                }
            }
        }
        private void PreCacheContributor() {
            lock (iLock)
            {
            if(!iContributorSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:contributor", iXmlNsMan)) {
                        iContributor.Add(n.InnerText);
                    }
                }
                iContributorSearched = true;
                iContributorFound = true;
            }
            }
        }
        
        public virtual string Date {
            get {
                lock (iLock)
                {
                    PreCacheDate();
                }
                return iDate;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iDate = value;
                    iDateFound = true;
                }
            }
        }
        private void PreCacheDate() {
            lock (iLock)
            {
            if(!iDateSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:date", iXmlNsMan);
                    if (n != null) {
                        iDate = n.InnerText;
                        iDateFound = true;
                    }
                }
                iDateSearched = true;
            }
            }
        }
        
        public virtual string Language {
            get {
                lock (iLock)
                {
                    PreCacheLanguage();
                }
                return iLanguage;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLanguage = value;
                    iLanguageFound = true;
                }
            }
        }
        private void PreCacheLanguage() {
            lock (iLock)
            {
            if(!iLanguageSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:language", iXmlNsMan);
                    if (n != null) {
                        iLanguage = n.InnerText;
                        iLanguageFound = true;
                    }
                }
                iLanguageSearched = true;
            }
            }
        }
        
        public virtual List<string> Rights {
            get {
                lock (iLock)
                {
                    PreCacheRights();
                }
                return iRights;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iRights = value;
                    iRightsFound = true;
                }
            }
        }
        private void PreCacheRights() {
            lock (iLock)
            {
            if(!iRightsSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("dc:rights", iXmlNsMan)) {
                        iRights.Add(n.InnerText);
                    }
                }
                iRightsSearched = true;
                iRightsFound = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheArtist();
			foreach(artist i in iArtist) {
			    i.PreCache();
			}
			PreCacheGenre();
			PreCacheLongDescription();
			PreCacheProducer();
			PreCacheStorageMedium();
			PreCacheDescription();
			PreCacheContributor();
			PreCacheDate();
			PreCacheLanguage();
			PreCacheRights();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iArtistFound) {
                foreach(artist item in iArtist) {
                    string xpath = "upnp:artist";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:artist", iXmlNsMan.LookupNamespace(prefix));
                    item.CreateDidlLite(aXmlDocument, obj);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iGenreFound) {
                foreach(string item in iGenre) {
                    string xpath = "upnp:genre";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:genre", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iLongDescriptionFound) {
                string xpath = "upnp:longDescription";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:longDescription", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(LongDescription.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iProducerFound) {
                foreach(string item in iProducer) {
                    string xpath = "upnp:producer";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:producer", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iDescriptionFound) {
                string xpath = "dc:description";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:description", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Description.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iContributorFound) {
                foreach(string item in iContributor) {
                    string xpath = "dc:contributor";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:contributor", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iDateFound) {
                string xpath = "dc:date";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:date", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Date.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iLanguageFound) {
                string xpath = "dc:language";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:language", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Language.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iRightsFound) {
                foreach(string item in iRights) {
                    string xpath = "dc:rights";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("dc:rights", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
        }
        
        private bool iArtistFound = false;
        private bool iArtistSearched = false;
        private List<artist> iArtist = new List<artist>();
        private bool iGenreFound = false;
        private bool iGenreSearched = false;
        private List<string> iGenre = new List<string>();
        private bool iLongDescriptionFound = false;
        private bool iLongDescriptionSearched = false;
        private string iLongDescription;
        private bool iProducerFound = false;
        private bool iProducerSearched = false;
        private List<string> iProducer = new List<string>();
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
        private bool iDescriptionFound = false;
        private bool iDescriptionSearched = false;
        private string iDescription;
        private bool iContributorFound = false;
        private bool iContributorSearched = false;
        private List<string> iContributor = new List<string>();
        private bool iDateFound = false;
        private bool iDateSearched = false;
        private string iDate;
        private bool iLanguageFound = false;
        private bool iLanguageSearched = false;
        private string iLanguage;
        private bool iRightsFound = false;
        private bool iRightsSearched = false;
        private List<string> iRights = new List<string>();
    }
    public class person : container
    {
        public person() {
            iClassFound = true;
            iClass = "object.container.person";
        }
        public person(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string Language {
            get {
                lock (iLock)
                {
                    PreCacheLanguage();
                }
                return iLanguage;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iLanguage = value;
                    iLanguageFound = true;
                }
            }
        }
        private void PreCacheLanguage() {
            lock (iLock)
            {
            if(!iLanguageSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("dc:language", iXmlNsMan);
                    if (n != null) {
                        iLanguage = n.InnerText;
                        iLanguageFound = true;
                    }
                }
                iLanguageSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheLanguage();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iLanguageFound) {
                string xpath = "dc:language";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("dc:language", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(Language.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iLanguageFound = false;
        private bool iLanguageSearched = false;
        private string iLanguage;
    }
    public class musicArtist : person
    {
        public musicArtist() {
            iClassFound = true;
            iClass = "object.container.person.musicArtist";
        }
        public musicArtist(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual List<string> Genre {
            get {
                lock (iLock)
                {
                    PreCacheGenre();
                }
                return iGenre;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iGenre = value;
                    iGenreFound = true;
                }
            }
        }
        private void PreCacheGenre() {
            lock (iLock)
            {
            if(!iGenreSearched) {
                if(iXmlDocument != null) {
                    foreach (XmlNode n in iXmlDocument.FirstChild.SelectNodes("upnp:genre", iXmlNsMan)) {
                        iGenre.Add(n.InnerText);
                    }
                }
                iGenreSearched = true;
                iGenreFound = true;
            }
            }
        }
        
        public virtual string ArtistDiscographyUri {
            get {
                lock (iLock)
                {
                    PreCacheArtistDiscographyUri();
                }
                return iArtistDiscographyUri;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iArtistDiscographyUri = value;
                    iArtistDiscographyUriFound = true;
                }
            }
        }
        private void PreCacheArtistDiscographyUri() {
            lock (iLock)
            {
            if(!iArtistDiscographyUriSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:artistDiscographyURI", iXmlNsMan);
                    if (n != null) {
                        iArtistDiscographyUri = n.InnerText;
                        iArtistDiscographyUriFound = true;
                    }
                }
                iArtistDiscographyUriSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheGenre();
			PreCacheArtistDiscographyUri();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iGenreFound) {
                foreach(string item in iGenre) {
                    string xpath = "upnp:genre";
                    string prefix = "didl";
                    string[] split = xpath.Split(':');
                    if(split.Length > 1) {
                        prefix = split[0];
                    }
                    XmlElement obj = aXmlDocument.CreateElement("upnp:genre", iXmlNsMan.LookupNamespace(prefix));
                    XmlText text = aXmlDocument.CreateTextNode(item.ToString());
                    obj.AppendChild(text);
                    aXmlElement.AppendChild(obj);
                }
            }
            if(iArtistDiscographyUriFound) {
                string xpath = "upnp:artistDiscographyURI";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:artistDiscographyURI", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(ArtistDiscographyUri.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iGenreFound = false;
        private bool iGenreSearched = false;
        private List<string> iGenre = new List<string>();
        private bool iArtistDiscographyUriFound = false;
        private bool iArtistDiscographyUriSearched = false;
        private string iArtistDiscographyUri;
    }
    public class storageSystem : container
    {
        public storageSystem() {
            iClassFound = true;
            iClass = "object.container.storageSystem";
        }
        public storageSystem(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual long StorageTotal {
            get {
                lock (iLock)
                {
                    PreCacheStorageTotal();
                }
                return iStorageTotal;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageTotal = value;
                    iStorageTotalFound = true;
                }
            }
        }
        private void PreCacheStorageTotal() {
            lock (iLock)
            {
            if(!iStorageTotalSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageTotal", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iStorageTotal = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iStorageTotalFound = true;
                    }
                }
                iStorageTotalSearched = true;
            }
            }
        }
        
        public virtual long StorageUsed {
            get {
                lock (iLock)
                {
                    PreCacheStorageUsed();
                }
                return iStorageUsed;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageUsed = value;
                    iStorageUsedFound = true;
                }
            }
        }
        private void PreCacheStorageUsed() {
            lock (iLock)
            {
            if(!iStorageUsedSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageUsed", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iStorageUsed = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iStorageUsedFound = true;
                    }
                }
                iStorageUsedSearched = true;
            }
            }
        }
        
        public virtual long StorageFree {
            get {
                lock (iLock)
                {
                    PreCacheStorageFree();
                }
                return iStorageFree;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageFree = value;
                    iStorageFreeFound = true;
                }
            }
        }
        private void PreCacheStorageFree() {
            lock (iLock)
            {
            if(!iStorageFreeSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageFree", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iStorageFree = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iStorageFreeFound = true;
                    }
                }
                iStorageFreeSearched = true;
            }
            }
        }
        
        public virtual long StorageMaxParition {
            get {
                lock (iLock)
                {
                    PreCacheStorageMaxParition();
                }
                return iStorageMaxParition;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMaxParition = value;
                    iStorageMaxParitionFound = true;
                }
            }
        }
        private void PreCacheStorageMaxParition() {
            lock (iLock)
            {
            if(!iStorageMaxParitionSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMaxPartition", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iStorageMaxParition = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iStorageMaxParitionFound = true;
                    }
                }
                iStorageMaxParitionSearched = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheStorageTotal();
			PreCacheStorageUsed();
			PreCacheStorageFree();
			PreCacheStorageMaxParition();
			PreCacheStorageMedium();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iStorageTotalFound) {
                string xpath = "upnp:storageTotal";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageTotal", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageTotal.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageUsedFound) {
                string xpath = "upnp:storageUsed";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageUsed", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageUsed.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageFreeFound) {
                string xpath = "upnp:storageFree";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageFree", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageFree.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageMaxParitionFound) {
                string xpath = "upnp:storageMaxPartition";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMaxPartition", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMaxParition.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iStorageTotalFound = false;
        private bool iStorageTotalSearched = false;
        private long iStorageTotal;
        private bool iStorageUsedFound = false;
        private bool iStorageUsedSearched = false;
        private long iStorageUsed;
        private bool iStorageFreeFound = false;
        private bool iStorageFreeSearched = false;
        private long iStorageFree;
        private bool iStorageMaxParitionFound = false;
        private bool iStorageMaxParitionSearched = false;
        private long iStorageMaxParition;
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
    }
    public class storageVolume : container
    {
        public storageVolume() {
            iClassFound = true;
            iClass = "object.container.storageVolume";
        }
        public storageVolume(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual string StorageTotal {
            get {
                lock (iLock)
                {
                    PreCacheStorageTotal();
                }
                return iStorageTotal;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageTotal = value;
                    iStorageTotalFound = true;
                }
            }
        }
        private void PreCacheStorageTotal() {
            lock (iLock)
            {
            if(!iStorageTotalSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageTotal", iXmlNsMan);
                    if (n != null) {
                        iStorageTotal = n.InnerText;
                        iStorageTotalFound = true;
                    }
                }
                iStorageTotalSearched = true;
            }
            }
        }
        
        public virtual string StorageUsed {
            get {
                lock (iLock)
                {
                    PreCacheStorageUsed();
                }
                return iStorageUsed;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageUsed = value;
                    iStorageUsedFound = true;
                }
            }
        }
        private void PreCacheStorageUsed() {
            lock (iLock)
            {
            if(!iStorageUsedSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageUsed", iXmlNsMan);
                    if (n != null) {
                        iStorageUsed = n.InnerText;
                        iStorageUsedFound = true;
                    }
                }
                iStorageUsedSearched = true;
            }
            }
        }
        
        public virtual long StorageFree {
            get {
                lock (iLock)
                {
                    PreCacheStorageFree();
                }
                return iStorageFree;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageFree = value;
                    iStorageFreeFound = true;
                }
            }
        }
        private void PreCacheStorageFree() {
            lock (iLock)
            {
            if(!iStorageFreeSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageFree", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iStorageFree = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iStorageFreeFound = true;
                    }
                }
                iStorageFreeSearched = true;
            }
            }
        }
        
        public virtual string StorageMedium {
            get {
                lock (iLock)
                {
                    PreCacheStorageMedium();
                }
                return iStorageMedium;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageMedium = value;
                    iStorageMediumFound = true;
                }
            }
        }
        private void PreCacheStorageMedium() {
            lock (iLock)
            {
            if(!iStorageMediumSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageMedium", iXmlNsMan);
                    if (n != null) {
                        iStorageMedium = n.InnerText;
                        iStorageMediumFound = true;
                    }
                }
                iStorageMediumSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheStorageTotal();
			PreCacheStorageUsed();
			PreCacheStorageFree();
			PreCacheStorageMedium();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iStorageTotalFound) {
                string xpath = "upnp:storageTotal";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageTotal", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageTotal.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageUsedFound) {
                string xpath = "upnp:storageUsed";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageUsed", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageUsed.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageFreeFound) {
                string xpath = "upnp:storageFree";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageFree", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageFree.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
            if(iStorageMediumFound) {
                string xpath = "upnp:storageMedium";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageMedium", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageMedium.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iStorageTotalFound = false;
        private bool iStorageTotalSearched = false;
        private string iStorageTotal;
        private bool iStorageUsedFound = false;
        private bool iStorageUsedSearched = false;
        private string iStorageUsed;
        private bool iStorageFreeFound = false;
        private bool iStorageFreeSearched = false;
        private long iStorageFree;
        private bool iStorageMediumFound = false;
        private bool iStorageMediumSearched = false;
        private string iStorageMedium;
    }
    public class storageFolder : container
    {
        public storageFolder() {
            iClassFound = true;
            iClass = "object.container.storageFolder";
        }
        public storageFolder(XmlNode aXmlNode)
            : base(aXmlNode)
        {
        }
        
        public virtual long StorageUsed {
            get {
                lock (iLock)
                {
                    PreCacheStorageUsed();
                }
                return iStorageUsed;
            }
            set {
                lock (iLock)
                {
                    PreCache();
                    iStorageUsed = value;
                    iStorageUsedFound = true;
                }
            }
        }
        private void PreCacheStorageUsed() {
            lock (iLock)
            {
            if(!iStorageUsedSearched) {
                if(iXmlDocument != null) {
                    XmlNode n = iXmlDocument.FirstChild.SelectSingleNode("upnp:storageUsed", iXmlNsMan);
                    if (n != null) {
                        try
                        {
					        iStorageUsed = long.Parse(n.InnerText);
					    }
					    catch(FormatException) { }
                        catch (OverflowException) { }
                        iStorageUsedFound = true;
                    }
                }
                iStorageUsedSearched = true;
            }
            }
        }
        

        internal override void PreCache() {
			PreCacheStorageUsed();
			base.PreCache();
        }

        
        internal override void CreateDidlLite(XmlDocument aXmlDocument, XmlElement aXmlElement) {
            base.CreateDidlLite(aXmlDocument, aXmlElement);
            if(iStorageUsedFound) {
                string xpath = "upnp:storageUsed";
                string prefix = "ns";
                string[] split = xpath.Split(':');
                if(split.Length > 1) {
                    prefix = split[0];
                }
                XmlElement element = aXmlDocument.CreateElement("upnp:storageUsed", iXmlNsMan.LookupNamespace(prefix));
                XmlText text = aXmlDocument.CreateTextNode(StorageUsed.ToString());
                element.AppendChild(text);
                aXmlElement.AppendChild(element);
            }
        }
        
        private bool iStorageUsedFound = false;
        private bool iStorageUsedSearched = false;
        private long iStorageUsed;
    }
} // DidlLite

