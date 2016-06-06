using System.Windows;
using Linn;
using System.IO;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Threading;
namespace KinskyDesktopWpf
{
    public class UiOptions
    {
        public UiOptions(IHelper aHelper)
        {
            iWindowWidth = new OptionDouble("windowwidth", "WindowWidth", "", 1024);
            iWindowHeight = new OptionDouble("windowheight", "WindowHeight", "", 600);
            iWindowLocationX = new OptionDouble("windowx", "WindowLocationX", "", -1);
            iWindowLocationY = new OptionDouble("windowy", "WindowLocationY", "", -1);

            iBrowserSplitterLocationLeft = new OptionInt("browsersplitterleft", "BrowserSplitterLocationLeft", "", 652);
            iBrowserSplitterLocationRight = new OptionInt("browsersplitterright", "BrowserSplitterLocationRight", "", 404);
            iFullscreen = new OptionBool("fullscreen", "Fullscreen", "", false);
            iMiniMode = new OptionBool("minimode", "MiniMode", "", false);
            iContainerView = new OptionUint("containerview", "ContainerView", "", 0);
            iContainerViewSizeThumbsView = new OptionDouble("containerviewsizethumbs", "ContainerViewSizeThumbs", "", 150);
            iContainerViewSizeListView = new OptionDouble("containerviewsizelist", "ContainerViewSizeList", "", 100);

            iOptionDialogSettings = new OptionDialogSettings();

            aHelper.AddOption(iWindowWidth);
            aHelper.AddOption(iWindowHeight);
            aHelper.AddOption(iWindowLocationX);
            aHelper.AddOption(iWindowLocationY);
            aHelper.AddOption(iBrowserSplitterLocationLeft);
            aHelper.AddOption(iBrowserSplitterLocationRight);
            aHelper.AddOption(iFullscreen);
            aHelper.AddOption(iMiniMode);
            aHelper.AddOption(iContainerView);
            aHelper.AddOption(iContainerViewSizeThumbsView);
            aHelper.AddOption(iContainerViewSizeListView);
            aHelper.AddOption(iOptionDialogSettings);
        }

        public OptionDialogSettings DialogSettings
        {
            get
            {
                return iOptionDialogSettings;
            }
        }

        public Size WindowSize
        {
            get
            {
                return new Size(iWindowWidth.Native, iWindowHeight.Native);
            }
            set
            {
                iWindowWidth.Native = value.Width;
                iWindowHeight.Native = value.Height;
            }
        }

        public Point WindowLocation
        {
            get
            {
                return new Point(iWindowLocationX.Native, iWindowLocationY.Native);
            }
            set
            {
                iWindowLocationX.Native = value.X;
                iWindowLocationY.Native = value.Y;
            }
        }

        public int BrowserSplitterLocationLeft
        {
            get
            {
                return iBrowserSplitterLocationLeft.Native;
            }
            set
            {
                iBrowserSplitterLocationLeft.Native = value;
            }
        }

        public int BrowserSplitterLocationRight
        {
            get
            {
                return iBrowserSplitterLocationRight.Native;
            }
            set
            {
                iBrowserSplitterLocationRight.Native = value;
            }
        }

        public bool Fullscreen
        {
            get
            {
                return iFullscreen.Native;
            }
            set
            {
                iFullscreen.Native = value;
            }
        }

        public bool MiniMode
        {
            get
            {
                return iMiniMode.Native;
            }
            set
            {
                iMiniMode.Native = value;
            }
        }

        public uint ContainerView
        {
            get
            {
                return iContainerView.Native;
            }
            set
            {
                iContainerView.Native = value;
            }
        }

        public double ContainerViewSizeListView
        {
            get
            {
                return iContainerViewSizeListView.Native;
            }
            set
            {
                iContainerViewSizeListView.Native = value;
            }
        }

        public double ContainerViewSizeThumbsView
        {
            get
            {
                return iContainerViewSizeThumbsView.Native;
            }
            set
            {
                iContainerViewSizeThumbsView.Native = value;
            }
        }

        private OptionDouble iWindowWidth;
        private OptionDouble iWindowHeight;
        private OptionDouble iWindowLocationX;
        private OptionDouble iWindowLocationY;
        private OptionInt iBrowserSplitterLocationLeft;
        private OptionInt iBrowserSplitterLocationRight;
        private OptionBool iFullscreen;
        private OptionBool iMiniMode;
        private OptionUint iContainerView;
        private OptionDouble iContainerViewSizeThumbsView;
        private OptionDouble iContainerViewSizeListView;
        private OptionDialogSettings iOptionDialogSettings;
    }

    public class OptionDialogSettings : OptionString
    {
        private Dictionary<Window, DialogSettings> iSettingsLookup;
        private XmlDocument iSettingsXml;
        private System.Threading.Timer iSaveTimer;
        private static int kSaveTimeoutMilliseconds = 100;
        private object iLock = new object();

        public OptionDialogSettings()
            : base("DialogSettings", "DialogSettings", "Position and size settings for dialogs.", "<dialogSettings/>")
        {
            iSettingsLookup = new Dictionary<Window, DialogSettings>();
            iSettingsXml = new XmlDocument();
            iSettingsXml.LoadXml(this.Value);
            iSaveTimer = new System.Threading.Timer(SaveTimerElapsed);
            iSaveTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public override bool Set(string aValue)
        {
            lock (iLock)
            {
                iSettingsXml.LoadXml(aValue);
                return base.Set(aValue);
            }
        }

        public void Register(Window aDialog, string aName)
        {
            lock (iLock)
            {
                iSettingsLookup.Add(aDialog, LoadSettings(aName, aDialog));
                aDialog.Closed += ClosedHandler;
                aDialog.SizeChanged += SizeChangedHandler;
                aDialog.LocationChanged += LocationChangedHandler;
            }
        }

        private void ClosedHandler(object sender, EventArgs e)
        {
            lock (iLock)
            {
                Window dialog = sender as Window;
                DialogUpdated(dialog);
                dialog.Closed -= ClosedHandler;
                dialog.SizeChanged -= SizeChangedHandler;
                dialog.LocationChanged -= LocationChangedHandler;
                iSettingsLookup.Remove(dialog);
            }
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            DialogUpdated(sender as Window);
        }

        private void LocationChangedHandler(object sender, EventArgs e)
        {
            DialogUpdated(sender as Window);
        }

        private DialogSettings LoadSettings(string aName, Window aDialog)
        {
            lock (iLock)
            {
                string xPath = string.Format("//dialog[@name='{0}']", aName);
                XmlElement settingsElem = iSettingsXml.SelectSingleNode(xPath) as XmlElement;
                DialogSettings settings;
                if (settingsElem != null)
                {
                    settings = new DialogSettings(settingsElem);
                    settings.Apply(aDialog);
                }
                else
                {
                    settings = new DialogSettings(aDialog, aName);
                    SaveSettings(settings);
                }
                return settings;
            }
        }

        private void SaveSettings(DialogSettings aSettings)
        {
            lock (iLock)
            {
                string xPath = string.Format("//dialog[@name='{0}']", aSettings.Name);
                XmlElement existing = iSettingsXml.SelectSingleNode(xPath) as XmlElement;
                if (existing == null)
                {
                    existing = aSettings.Create(iSettingsXml);
                    iSettingsXml.DocumentElement.AppendChild(existing);
                }
                aSettings.Save(existing);
                iSaveTimer.Change(kSaveTimeoutMilliseconds, Timeout.Infinite);
            }
        }

        private void SaveTimerElapsed(object aState)
        {
            lock (iLock)
            {
                base.Update(iSettingsXml.OuterXml);
                iSaveTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void DialogUpdated(Window aDialog)
        {
            lock (iLock)
            {
                DialogSettings settings = iSettingsLookup[aDialog];
                Assert.Check(settings != null);
                settings.Update(aDialog);
                SaveSettings(settings);
            }
        }

        private class DialogSettings
        {
            public DialogSettings(Window aDialog, string aName)
            {
                Name = aName;
                Update(aDialog);
            }

            public DialogSettings(XmlElement aSettings)
            {
                Name = aSettings.Attributes["name"].Value;
                Size = new Size(Double.Parse(aSettings.Attributes["width"].Value),
                                Double.Parse(aSettings.Attributes["height"].Value));
                Top = Double.Parse(aSettings.Attributes["top"].Value);
                Left = Double.Parse(aSettings.Attributes["left"].Value);
            }

            public XmlElement Create(XmlDocument aXmlDocument)
            {
                XmlElement elem = aXmlDocument.CreateElement("dialog");
                return elem;
            }

            public void Save(XmlElement aElement)
            {
                aElement.Attributes.RemoveAll();
                AppendAttribute(aElement, "name", Name);
                AppendAttribute(aElement, "width", Size.Width.ToString());
                AppendAttribute(aElement, "height", Size.Height.ToString());
                AppendAttribute(aElement, "top", Top.ToString());
                AppendAttribute(aElement, "left", Left.ToString());
            }

            public void Update(Window aDialog)
            {
                Size = new Size(aDialog.Width, aDialog.Height);
                Top = aDialog.Top;
                Left = aDialog.Left;
            }

            public void Apply(Window aDialog)
            {
                aDialog.Left = Left;
                aDialog.Top = Top;
                aDialog.Width = Size.Width;
                aDialog.Height = Size.Height;
            }

            private void AppendAttribute(XmlElement aElement, string aName, string aValue)
            {
                XmlAttribute attr = aElement.OwnerDocument.CreateAttribute(aName);
                attr.Value = aValue;
                aElement.Attributes.Append(attr);
            }

            public Size Size { get; private set; }
            public string Name { get; private set; }
            public Double Top { get; private set; }
            public Double Left { get; private set; }
        }
    }
}