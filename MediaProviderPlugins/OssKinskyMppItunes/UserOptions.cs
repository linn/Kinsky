
using System;
using System.IO;

using Linn;


namespace OssKinskyMppItunes
{
    public class UserOptions : OptionPage
    {
        public UserOptions()
            : base("iTunes")
        {
            // create the default xml file
            string xmlFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "iTunes");
            xmlFile = Path.Combine(xmlFile, "iTunes Music Library.xml");

            iLibraryXmlFile = new OptionFilePath("itunesxml",
                                                 "iTunes music library file",
                                                 "The filename of the iTunes XML library file",
                                                 xmlFile);
            Add(iLibraryXmlFile);
        }

        public string LibraryXmlFile
        {
            get { return iLibraryXmlFile.Value; }
            set { iLibraryXmlFile.Set(value); }
        }

        public event EventHandler<EventArgs> LibraryXmlFileChanged
        {
            add { iLibraryXmlFile.EventValueChanged += value; }
            remove { iLibraryXmlFile.EventValueChanged -= value; }
        }

        private OptionFilePath iLibraryXmlFile;
    }

} // OssKinskyMppItunes




