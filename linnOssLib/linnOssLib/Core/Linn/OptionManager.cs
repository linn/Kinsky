using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Linn
{
    public class OptionManager
    {
        public OptionManager(string aFilename)
        {
            // lock for the dictionary - note that it is only the iDictionary
            // and the writing to the options file that require locking since
            // the iLoadDictionary does not change once the constructor has completed
            iLock = new object();

            iFilename = aFilename;
            iDictionary = new Dictionary<string, string>();
            iLoadDictionary = new Dictionary<string, string>();

            Load();
        }

        private void OptionChanged(object sender, EventArgs e)
        {
            Option option = sender as Option;

            lock (iLock)
            {
                iDictionary[option.Id] = option.Value;
                Save();
            }
        }

        public void Add(Option aOption)
        {
            lock (iLock)
            {
                Assert.Check(!iDictionary.ContainsKey(aOption.Id));

                string value;
                if (iLoadDictionary.TryGetValue(aOption.Id, out value))
                {
                    aOption.Set(value);
                    iLoadDictionary.Remove(aOption.Id);
                }

                iDictionary[aOption.Id] = aOption.Value;

                aOption.EventValueChanged += OptionChanged;
            }
        }

        public void Remove(Option aOption)
        {
            lock (iLock)
            {
                aOption.EventValueChanged -= OptionChanged;

                if (iLoadDictionary.ContainsKey(aOption.Id))
                {
                    iLoadDictionary.Remove(aOption.Id);
                }

                if (iDictionary.ContainsKey(aOption.Id))
                {
                    iDictionary.Remove(aOption.Id);
                }
            }
        }

        public Dictionary<string, string> OptionValues
        {
            get
            {
                // takes a snapshot of the current option values
                Dictionary<string, string> values = new Dictionary<string, string>();
                lock (iLock)
                {
                    foreach (string aKey in iDictionary.Keys)
                    {
                        values.Add(aKey, iDictionary[aKey]);
                    }
                }
                return values;
            }
        }

        // XML schema is as follows:
        // <Dictionary>
        //   <Entry>
        //     <Key>key</Key>
        //     <Value>value</Value>
        //   </Entry>
        //   ...
        // </Dictionary>
        private void Save()
        {
            lock (iLock)
            {
                XmlDocument document = new XmlDocument();
                XmlNode dictionaryNode = document.CreateElement("Dictionary");

                foreach (KeyValuePair<string, string> p in iDictionary)
                {
                    XmlNode keyNode = document.CreateElement("Key");
                    keyNode.InnerText = p.Key;

                    XmlNode valueNode = document.CreateElement("Value");
                    valueNode.InnerText = p.Value;

                    XmlNode entryNode = document.CreateElement("Entry");
                    entryNode.AppendChild(keyNode);
                    entryNode.AppendChild(valueNode);

                    dictionaryNode.AppendChild(entryNode);
                }

                foreach (KeyValuePair<string, string> p in iLoadDictionary)
                {
                    XmlNode keyNode = document.CreateElement("Key");
                    keyNode.InnerText = p.Key;

                    XmlNode valueNode = document.CreateElement("Value");
                    valueNode.InnerText = p.Value;

                    XmlNode entryNode = document.CreateElement("Entry");
                    entryNode.AppendChild(keyNode);
                    entryNode.AppendChild(valueNode);

                    dictionaryNode.AppendChild(entryNode);
                }

                document.AppendChild(dictionaryNode);
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
                Trace.WriteLine(Trace.kCore, "Loading settings from " + iFilename);
                if (File.Exists(iFilename))
                {
                    LoadOptionsFile(iFilename);
                }
                else
                {
                    UserLog.WriteLine("Could not find options file " + iFilename);
                    Trace.WriteLine(Trace.kCore, "Could not find options file " + iFilename);
                    MigrateOldOptions();
                }
            }
        }

        private void LoadOptionsFile(string aFilename)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(aFilename);

                XmlNodeList entryNodeList = document.SelectNodes("Dictionary/Entry");
                foreach (XmlNode entryNode in entryNodeList)
                {
                    // get the xml nodes
                    XmlNode keyNode = entryNode.SelectSingleNode("Key");
                    XmlNode valueNode = entryNode.SelectSingleNode("Value");

                    // add the dictionary entry
                    if (keyNode != null && valueNode != null)
                    {
                        iLoadDictionary.Add(keyNode.InnerText, valueNode.InnerText);

                        try
                        {
                            if (iLoadDictionary["room"] == "First Discovered")
                            {
                                iLoadDictionary["room"] = "Last Selected";
                            }
                        }
                        catch (Exception)
                        {
                            Trace.WriteLine(Trace.kCore, "Failed to migrate startup room setting");
                        }

                        Trace.WriteLine(Trace.kCore, "Loaded: " + keyNode.InnerText + " [" + valueNode.InnerText + "]");
                    }
                }
            }
            catch (Exception e)
            {
                UserLog.WriteLine("Options file corrupt: " + e.ToString());
                Trace.WriteLine(Trace.kCore, "Options file corrupt: " + e.ToString());
            }
        }

        private void MigrateOldOptions()
        {


            //if (Directory.Exists(previousLocation))
            //{
            //    Directory.Move(previousLocation, dataPath.FullName);
            //    string optionsFile = Path.Combine(previousLocation, "Options.xml");
            //    if (File.Exists(optionsFile))
            //    {
            //        string optionsText = File.ReadAllText(optionsFile).Replace(aPreviousPath, dataPath.FullName);
            //        File.WriteAllText(optionsFile, optionsText);
            //    }
            //}

            Trace.WriteLine(Trace.kCore, "Migrating old options...");

            string dataPath = Path.GetDirectoryName(iFilename);
            bool migrated = false;


            // rename KinskyDesktop to Kinsky
            if (dataPath.Contains("Kinsky"))
            {
                string previousDataPath;
                previousDataPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "KinskyDesktop");
                if (Directory.Exists(previousDataPath))
                {
                    string optionsFilename = Path.Combine(previousDataPath, Path.GetFileName(iFilename));
                    if (File.Exists(optionsFilename))
                    {
                        StreamReader reader = null;
                        string optionsText = string.Empty;
                        try
                        {
                            reader = new StreamReader(optionsFilename);
                            optionsText = reader.ReadToEnd().Replace(previousDataPath, dataPath);
                        }
                        finally
                        {
                            if (reader != null)
                            {
                                reader.Close();
                                reader.Dispose();
                            }
                        }
                        StreamWriter writer = null;
                        try
                        {
                            writer = new StreamWriter(iFilename);
                            writer.Write(optionsText);
                            writer.Flush();
                        }
                        finally
                        {
                            if (writer != null)
                            {
                                writer.Close();
                                writer.Dispose();
                            }
                        }
                        LoadOptionsFile(iFilename);
                        if (Directory.Exists(Path.Combine(previousDataPath, "Playlists")))
                        {
                            Directory.Move(Path.Combine(previousDataPath, "Playlists"), Path.Combine(dataPath, "Playlists"));
                        }
                        migrated = true;
                    }
                    dataPath = previousDataPath;
                }
            }
            if (!migrated)
            {
                // Network.xml
                if (File.Exists(Path.Combine(dataPath, "Network.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Network.xml")));

                        iLoadDictionary["interface"] = doc.SelectSingleNode("/Network/Adapter").InnerText;
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate network settings");
                    }
                }

                // AutoSelect.xml
                if (File.Exists(Path.Combine(dataPath, "AutoSelect.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "AutoSelect.xml")));

                        iLoadDictionary["room"] = doc.SelectSingleNode("/AutoSelect/StartupRoom").InnerText;
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate auto select settings");
                    }
                }

                // MediaProviders.xml
                if (File.Exists(Path.Combine(dataPath, "MediaProviders.xml")))
                {
                    // nothing to do
                }

                // Library.xml
                if (File.Exists(Path.Combine(dataPath, "Library.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Library.xml")));

                        SetValue(iLoadDictionary, "albumview", doc, "/Library/AlbumView");
                        SetValue(iLoadDictionary, "containerview", doc, "/Library/ContainerView");
                        SetValue(iLoadDictionary, "containerviewsize", doc, "/Library/ContainerViewSize");
                        SetValue(iLoadDictionary, "displayartwork", doc, "/Library/ShowArtwork");
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate library settings");
                    }
                }

                // LocalPlaylist.xml
                if (File.Exists(Path.Combine(dataPath, "LocalPlaylist.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "LocalPlaylist.xml")));

                        iLoadDictionary["playlistpath"] = doc.SelectSingleNode("/LocalPlaylist/SaveDirectory").InnerText;
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate local playlist settings");
                    }
                }

                // Folder.xml
                if (File.Exists(Path.Combine(dataPath, "Folder.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Folder.xml")));

                        XmlNodeList folders = doc.SelectNodes("/Folder/Folders");
                        List<string> list = new List<string>();
                        foreach (XmlNode f in folders)
                        {
                            list.Add(f.InnerText);
                        }
                        iLoadDictionary["folders"] = StringListConverter.ListToString(list);
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate local playlist settings");
                    }
                }

                // General.xml
                if (File.Exists(Path.Combine(dataPath, "General.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "General.xml")));

                        SetValue(iLoadDictionary, "volumestep", doc, "/UserOptions/VolumeStepSize");
                        SetValue(iLoadDictionary, "playlistinfo", doc, "/UserOptions/Details");
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate general settings");
                    }
                }

                // KinskyDesktop.xml
                if (File.Exists(Path.Combine(dataPath, "KinskyDesktop.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "KinskyDesktop.xml")));

                        iLoadDictionary["windowwidth"] = doc.SelectSingleNode("/KinskyDesktop/WindowSize/Width").InnerText;
                        iLoadDictionary["windowheight"] = doc.SelectSingleNode("/KinskyDesktop/WindowSize/Height").InnerText;
                        iLoadDictionary["windowx"] = doc.SelectSingleNode("/KinskyDesktop/WindowLocation/X").InnerText;
                        iLoadDictionary["windowy"] = doc.SelectSingleNode("/KinskyDesktop/WindowLocation/Y").InnerText;
                        iLoadDictionary["splitterx"] = doc.SelectSingleNode("/KinskyDesktop/SplitterLocation").InnerText;
                        iLoadDictionary["minimode"] = doc.SelectSingleNode("/KinskyDesktop/MiniMode").InnerText;
                        iLoadDictionary["minimodewidth"] = doc.SelectSingleNode("/KinskyDesktop/MiniModeWidth").InnerText;
                        iLoadDictionary["minimodex"] = doc.SelectSingleNode("/KinskyDesktop/MiniModeLocation/X").InnerText;
                        iLoadDictionary["minimodey"] = doc.SelectSingleNode("/KinskyDesktop/MiniModeLocation/Y").InnerText;
                        iLoadDictionary["fullscreen"] = doc.SelectSingleNode("/KinskyDesktop/Fullscreen").InnerText;
                        iLoadDictionary["windowtransparency"] = doc.SelectSingleNode("/KinskyDesktop/Transparency").InnerText;
                        iLoadDictionary["windowborder"] = doc.SelectSingleNode("/KinskyDesktop/WindowBorder").InnerText;
                        iLoadDictionary["hidemouse"] = doc.SelectSingleNode("/KinskyDesktop/HideMouseCursor").InnerText;
                        iLoadDictionary["trackinfo"] = doc.SelectSingleNode("/KinskyDesktop/ShowTrackInfo").InnerText;
                        bool b = bool.Parse(doc.SelectSingleNode("/KinskyDesktop/UseRockerControls").InnerText);
                        iLoadDictionary["rotarycontrols"] = (!b).ToString();
                        iLoadDictionary["tooltips"] = doc.SelectSingleNode("/KinskyDesktop/ShowToolTips").InnerText;
                        iLoadDictionary["background"] = ((int.Parse(doc.SelectSingleNode("/KinskyDesktop/BackgroundColour/A").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 24) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/BackgroundColour/R").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 16) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/BackgroundColour/G").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 8) | int.Parse(doc.SelectSingleNode("/KinskyDesktop/BackgroundColour/B").InnerText, System.Globalization.CultureInfo.InvariantCulture)).ToString();
                        iLoadDictionary["highlight"] = ((int.Parse(doc.SelectSingleNode("/KinskyDesktop/HighlightColour/A").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 24) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/HighlightColour/R").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 16) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/HighlightColour/G").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 8) | int.Parse(doc.SelectSingleNode("/KinskyDesktop/HighlightColour/B").InnerText, System.Globalization.CultureInfo.InvariantCulture)).ToString();
                        iLoadDictionary["text"] = ((int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextColour/A").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 24) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextColour/R").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 16) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextColour/G").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 8) | int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextColour/B").InnerText, System.Globalization.CultureInfo.InvariantCulture)).ToString();
                        iLoadDictionary["textmuted"] = ((int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextMutedColour/A").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 24) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextMutedColour/R").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 16) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextMutedColour/G").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 8) | int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextMutedColour/B").InnerText, System.Globalization.CultureInfo.InvariantCulture)).ToString();
                        iLoadDictionary["textbright"] = ((int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextBrightColour/A").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 24) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextBrightColour/R").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 16) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextBrightColour/G").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 8) | int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextBrightColour/B").InnerText, System.Globalization.CultureInfo.InvariantCulture)).ToString();
                        iLoadDictionary["texthighlighted"] = ((int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextHighlightColour/A").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 24) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextHighlightColour/R").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 16) | (int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextHighlightColour/G").InnerText, System.Globalization.CultureInfo.InvariantCulture) << 8) | int.Parse(doc.SelectSingleNode("/KinskyDesktop/TextHighlightColour/B").InnerText, System.Globalization.CultureInfo.InvariantCulture)).ToString();
                        int i = int.Parse(doc.SelectSingleNode("/KinskyDesktop/FontPreset").InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        switch (i)
                        {
                            case 0:
                                iLoadDictionary["fontsize"] = "small";
                                break;
                            case 1:
                                iLoadDictionary["fontsize"] = "medium";
                                break;
                            case 2:
                                iLoadDictionary["fontsize"] = "large";
                                break;
                            default:
                                //Assert.Check(false);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate Kinsky settings");
                    }
                }

                // Updates.xml
                if (File.Exists(Path.Combine(dataPath, "Updates.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Updates.xml")));

                        iLoadDictionary["beta"] = doc.SelectSingleNode("/Updates/BetaUpdates").InnerText;
                        bool bypass = bool.Parse(doc.SelectSingleNode("/Updates/BypassUpdates").InnerText);
                        iLoadDictionary["autoupdate"] = (!bypass).ToString();
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate settings from Updates.xml");
                    }
                }

                // View.xml
                if (File.Exists(Path.Combine(dataPath, "View.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "View.xml")));

                        string iconView = doc.SelectSingleNode("/View/IconView").InnerText;
                        if (iconView == "LargeIcon")
                        {
                            iLoadDictionary["iconview"] = "Large Icon";
                        }
                        else if (iconView == "Tile")
                        {
                            iLoadDictionary["iconview"] = "Tile";
                        }
                        else if (iconView == "List")
                        {
                            iLoadDictionary["iconview"] = "List";
                        }
                        else
                        {
                            iLoadDictionary["iconview"] = "Details";
                        }
                        iLoadDictionary["radiotimeusername"] = doc.SelectSingleNode("/View/RadioTimeUsername").InnerText;
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate settings from View.xml");
                    }
                }

                // Application.xml (Kinsky Jukebox)
                if (File.Exists(Path.Combine(dataPath, "Application.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Application.xml")));

                        iLoadDictionary["leftsplitter"] = doc.SelectSingleNode("/Application/LeftSplitterLocation").InnerText;
                        iLoadDictionary["rightsplitter"] = doc.SelectSingleNode("/Application/RightSplitterLocation").InnerText;
                        iLoadDictionary["width"] = doc.SelectSingleNode("/Application/WindowSize/Width").InnerText;
                        iLoadDictionary["height"] = doc.SelectSingleNode("/Application/WindowSize/Height").InnerText;
                        iLoadDictionary["maximised"] = doc.SelectSingleNode("/Application/WindowFullScreen").InnerText;
                        iLoadDictionary["minimised"] = doc.SelectSingleNode("/Application/WindowMinimized").InnerText;
                        iLoadDictionary["printpagespersheet"] = doc.SelectSingleNode("/Application/PrintPagesPerSheetIndex").InnerText;
                        string layout = doc.SelectSingleNode("/Application/PrintPageLayout").InnerText;
                        if (layout == "ePortraitWithoutTrackDetails")
                        {
                            iLoadDictionary["printpagelayout"] = "Portait";
                        }
                        else if (layout == "eLandscapeWithoutTrackDetails")
                        {
                            iLoadDictionary["printpagelayout"] = "Landscape";
                        }
                        else if (layout == "eLandscapeWithTrackDetails")
                        {
                            iLoadDictionary["printpagelayout"] = "Landscape Track Details";
                        }
                        else
                        {
                            iLoadDictionary["printpagelayout"] = "Portait Track Details";
                        }
                        iLoadDictionary["printorderbooklet"] = doc.SelectSingleNode("/Application/PrintOrderBooklet").InnerText;
                        string docType = doc.SelectSingleNode("/Application/PrintDocumentType").InnerText;
                        if (docType == "eRtf")
                        {
                            iLoadDictionary["printdoctype"] = "Rtf";
                        }
                        else
                        {
                            iLoadDictionary["printdoctype"] = "Pdf";
                        }
                        XmlNodeList sections = doc.SelectNodes("/Application/PrintSectionStatus/First");
                        XmlNodeList sectionsVisible = doc.SelectNodes("/Application/PrintSectionStatus/Second");
                        if (sections.Count == sectionsVisible.Count)
                        {
                            List<string> list = new List<string>();
                            for (int i = 0; i < sections.Count; i++)
                            {
                                if (sectionsVisible[i].InnerText == "true")
                                {
                                    list.Add(sections[i].InnerText);
                                }
                            }
                            iLoadDictionary["printsections"] = StringListConverter.ListToString(list);
                        }

                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate settings from Application.xml");
                    }
                }

                // Collection.xml (Kinsky Jukebox)
                if (File.Exists(Path.Combine(dataPath, "Collection.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Collection.xml")));

                        // setup options
                        iLoadDictionary["scandirectory"] = doc.SelectSingleNode("/Collection/CollectionLocation").InnerText;
                        iLoadDictionary["scanurl"] = doc.SelectSingleNode("/Collection/CollectionHttpLocation").InnerText;
                        iLoadDictionary["usehttpserver"] = doc.SelectSingleNode("/Collection/UseHttpServer").InnerText;
                        iLoadDictionary["compliationsfoldername"] = doc.SelectSingleNode("/Collection/CompilationsFolder").InnerText;
                        iLoadDictionary["randomiselargeplaylists"] = doc.SelectSingleNode("/Collection/Randomize").InnerText;

                        // organisation options
                        iLoadDictionary["newcutoff"] = doc.SelectSingleNode("/Collection/NewMusicCutoffDays").InnerText;
                        string arrange = doc.SelectSingleNode("/Collection/SortByYear").InnerText;
                        if (arrange == "true")
                        {
                            iLoadDictionary["albumarrangement"] = "Arrange Albums by Year";
                        }
                        else
                        {
                            iLoadDictionary["albumarrangement"] = "Arrange Albums Alphabetically";
                        }
                        string group = doc.SelectSingleNode("/Collection/IgnoreCompilations").InnerText;
                        if (group == "true")
                        {
                            iLoadDictionary["groupcompilations"] = "Group as 'Various'";
                        }
                        else
                        {
                            iLoadDictionary["groupcompilations"] = "Retain Artist Names";
                        }
                        string radio = doc.SelectSingleNode("/Collection/IncludeLocalRadio").InnerText;
                        if (radio == "true")
                        {
                            iLoadDictionary["localradio"] = "Create Local Radio Playlists";
                        }
                        else
                        {
                            iLoadDictionary["localradio"] = "Ignore";
                        }
                        XmlNodeList sortType = doc.SelectNodes("/Collection/SortTypeStatus/First");
                        XmlNodeList sortTypeSelected = doc.SelectNodes("/Collection/SortTypeStatus/Second");
                        if (sortType.Count == sortTypeSelected.Count)
                        {
                            for (int i = 0; i < sortType.Count; i++)
                            {
                                string type = sortType[i].InnerText;
                                if (type == "New")
                                {
                                    iLoadDictionary["sortnew"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Artist / Album" || type == "Artist/Album" ||
                                         type == "Artist \\ Album" || type == "Artist\\Album")
                                {
                                    iLoadDictionary["sortartistalbum"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Artist")
                                {
                                    iLoadDictionary["sortartist"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Album")
                                {
                                    iLoadDictionary["sortalbum"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Genre")
                                {
                                    iLoadDictionary["sortgenre"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Composer")
                                {
                                    iLoadDictionary["sortcomposer"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Conductor")
                                {
                                    iLoadDictionary["sortconductor"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Arist A to Z" || type == "Arist AtoZ" || type == "AristAtoZ")
                                {
                                    iLoadDictionary["sortartistaz"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Title A to Z" || type == "Title AtoZ" || type == "TitleAtoZ")
                                {
                                    iLoadDictionary["sorttitleaz"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Album Artist / Album" || type == "Album Artist/Album" || type == "AlbumArtist/Album" ||
                                         type == "Album Artist \\ Album" || type == "Album Artist\\Album" || type == "AlbumArtist\\Album")
                                {
                                    iLoadDictionary["sortalbumartistalbum"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "Album Artist" || type == "AlbumArtist")
                                {
                                    iLoadDictionary["sortalbumartist"] = sortTypeSelected[i].InnerText;
                                }
                                else if (type == "All")
                                {
                                    iLoadDictionary["sortall"] = sortTypeSelected[i].InnerText;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate settings from Collection.xml");
                    }
                }

                // Wizard.xml (Kinsky Jukebox)
                if (File.Exists(Path.Combine(dataPath, "Wizard.xml")))
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(File.OpenRead(Path.Combine(dataPath, "Wizard.xml")));

                        string config = doc.SelectSingleNode("/Wizard/Config").InnerText;
                        if (config == "eAlways")
                        {
                            iLoadDictionary["wizardchangeconfig"] = "Always";
                        }
                        else if (config == "eNever")
                        {
                            iLoadDictionary["wizardchangeconfig"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardchangeconfig"] = "Prompt";
                        }

                        string import = doc.SelectSingleNode("/Wizard/ImportSavedPresets").InnerText;
                        if (import == "eAlways")
                        {
                            iLoadDictionary["wizardimport"] = "Always";
                        }
                        else if (import == "eNever")
                        {
                            iLoadDictionary["wizardimport"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardimport"] = "Prompt";
                        }

                        string clear = doc.SelectSingleNode("/Wizard/ClearImport").InnerText;
                        if (clear == "eAlways")
                        {
                            iLoadDictionary["wizardclear"] = "Always";
                        }
                        else if (clear == "eNever")
                        {
                            iLoadDictionary["wizardclear"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardclear"] = "Prompt";
                        }

                        string correctIp = doc.SelectSingleNode("/Wizard/CorrectIp").InnerText;
                        if (correctIp == "eAlways")
                        {
                            iLoadDictionary["wizardipcorrect"] = "Always";
                        }
                        else if (correctIp == "eNever")
                        {
                            iLoadDictionary["wizardipcorrect"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardipcorrect"] = "Prompt";
                        }

                        string useScan = doc.SelectSingleNode("/Wizard/UseCurrentScan").InnerText;
                        if (useScan == "eAlways")
                        {
                            iLoadDictionary["wizardusescan"] = "Always";
                        }
                        else if (useScan == "eNever")
                        {
                            iLoadDictionary["wizardusescan"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardusescan"] = "Prompt";
                        }

                        string scanType = doc.SelectSingleNode("/Wizard/ScanType").InnerText;
                        if (scanType == "eQuickScan")
                        {
                            iLoadDictionary["wizardscantype"] = "Quick";
                        }
                        else if (scanType == "eFullScan")
                        {
                            iLoadDictionary["wizardscantype"] = "Full";
                        }
                        else
                        {
                            iLoadDictionary["wizardscantype"] = "Prompt";
                        }

                        string sync = doc.SelectSingleNode("/Wizard/SyncDs").InnerText;
                        if (sync == "eAlways")
                        {
                            iLoadDictionary["wizardsync"] = "Always";
                        }
                        else if (sync == "eNever")
                        {
                            iLoadDictionary["wizardsync"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardsync"] = "Prompt";
                        }

                        string print = doc.SelectSingleNode("/Wizard/Print").InnerText;
                        if (print == "eAlways")
                        {
                            iLoadDictionary["wizardprint"] = "Always";
                        }
                        else if (print == "eNever")
                        {
                            iLoadDictionary["wizardprint"] = "Never";
                        }
                        else
                        {
                            iLoadDictionary["wizardprint"] = "Prompt";
                        }
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine(Trace.kCore, "Failed to migrate settings from Wizard.xml");
                    }
                }
            }

            Save();
        }

        private void SetValue(Dictionary<string, string> aDict, string aKey, XmlDocument aDoc, string aXpath)
        {
            XmlNode node = aDoc.SelectSingleNode(aXpath);
            if (node != null)
            {
                aDict[aKey] = node.InnerText;
            }
        }

        private object iLock;
        private string iFilename;
        private Dictionary<string, string> iLoadDictionary;
        private Dictionary<string, string> iDictionary;
    }
}