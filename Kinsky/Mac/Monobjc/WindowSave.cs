
using System;
using System.Threading;

using Monobjc;
using Monobjc.Cocoa;

using Linn;
using Linn.Kinsky;
using System.Collections.Generic;


namespace KinskyDesktop
{
    // File's owner class for the WindowUpdate.xib file
    [ObjectiveCClass]
    public class WindowSave : NSWindowController
    {
        public WindowSave() : base() {}
        public WindowSave(IntPtr aInstance) : base(aInstance) {}

        public WindowSave(ISaveSupport aSaveSupport)
            : base()
        {
            iSaveSupport = aSaveSupport;
            iImageList = new Dictionary<uint, System.Uri>();
            //iImages = new List<NSImage>();
            iImageLocations = new List<string>();
            NSBundle.LoadNibNamedOwner("WindowSave.nib", this);
        }

        private void EventSaveLocationChangedHandler (object sender, EventArgs e)
        {
            UpdateSaveLocation();
        }

        private void EventSaveLocationsChangedHandler(object sender, EventArgs e)
        {
            UpdateSaveLocations();
        }

        private void EventImageListChangedHandler (object sender, EventArgs e)
        {
            UpdateImages();
        }

        public void Show()
        {
            // show the window modally
            Window.Center();
            Window.MakeKeyAndOrderFront(this);
            NSApplication.NSApp.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, null, null, IntPtr.Zero);
            NSApplication.NSApp.RunModalForWindow(Window);
            NSApplication.NSApp.EndSheet(Window);
            Window.OrderOut(this);
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            iSaveSupport.EventSaveLocationChanged += EventSaveLocationChangedHandler;
            iSaveSupport.EventSaveLocationsChanged += EventSaveLocationsChangedHandler;
            iSaveSupport.EventImageListChanged += EventImageListChangedHandler;

            TextName.StringValue = iSaveSupport.DefaultName;
            TextName.ActionEvent += TextNameChanged;

            ButtonLocation.ActionEvent += LocationChanged;

            UpdateSaveLocations();
            UpdateSaveLocation();
            UpdateImages();

            ButtonClose.ActionEvent += ButtonCloseClicked;
            ButtonSave.ActionEvent += ButtonSaveClicked;

            ArtworkCacheInstance.Instance.EventImageAdded += EventImageAddedHandler;

            Window.SetDelegate(d =>
            {
                d.WindowWillClose += WindowWillClose;
            });
            SetButtonState();
        }

        private void EventImageAddedHandler (object sender, ArtworkCache.EventArgsArtwork e)
        {
            ButtonImage.BeginInvoke((Action)(()=>{
                if (iImageLocations.Contains(e.Uri))
                {
                    NSImage img = GetImage(new Uri(e.Uri));
                    for(int i=0;i<iImageLocations.Count;i++)
                    {
                        if (iImageLocations[i] == e.Uri)
                        {
                            ButtonImage.ItemAtIndex(i).Image = img;
                        }
                    }
                }
            }));
        }

        private void WindowWillClose(NSNotification aNotification)
        {
            iSaveSupport.EventSaveLocationChanged -= EventSaveLocationChangedHandler;
            iSaveSupport.EventSaveLocationsChanged -= EventSaveLocationsChangedHandler;
            iSaveSupport.EventImageListChanged -= EventImageListChangedHandler;


            TextName.ActionEvent -= TextNameChanged;
            ButtonClose.ActionEvent -= ButtonCloseClicked;
            ButtonSave.ActionEvent -= ButtonSaveClicked;

            ButtonLocation.ActionEvent -= LocationChanged;

            ArtworkCacheInstance.Instance.EventImageAdded -= EventImageAddedHandler;

            NSApplication.NSApp.StopModal();
        }

        private void ButtonCloseClicked(Id aSender)
        {
            this.Close();
        }

        private void ButtonSaveClicked(Id aSender)
        {
            try
            {
                uint imageId = 0;
                if (iShowingImage)
                {
                    imageId = (uint)ButtonImage.SelectedTag;
                }
                iSaveSupport.Save(TextName.StringValue, TextDescription.StringValue, imageId);
                this.Close();
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Error saving playlist: " + ex.ToString());
            }
        }

        private void TextNameChanged(Id aSender)
        {
            SetButtonState();
        }

        private void LocationChanged(Id aSender)
        {
            iSaveSupport.SaveLocation = ButtonLocation.TitleOfSelectedItem;
        }

        private void SetButtonState()
        {
            ButtonSave.IsEnabled = !(TextName.StringValue.IsEqualToString(string.Empty));
        }

        private void UpdateSaveLocations()
        {
            ButtonLocation.RemoveAllItems();
            foreach (String location in iSaveSupport.SaveLocations)
            {
                ButtonLocation.AddItemWithTitle(location);
            }
        }

        private void UpdateSaveLocation()
        {
            string selected = ButtonLocation.TitleOfSelectedItem;
            if (selected != iSaveSupport.SaveLocation)
            {
                ButtonLocation.SelectItemWithTitle(iSaveSupport.SaveLocation);
            }
            if (ButtonLocation.SelectedItem == null && ButtonLocation.NumberOfItems > 0)
            {
                ButtonLocation.SelectItemAtIndex(0);
            }
        }

        private void UpdateImages()
        {
            iImageLocations.Clear();
            ButtonImage.RemoveAllItems();
            iImageList = iSaveSupport.ImageList;
            foreach(KeyValuePair<uint, System.Uri> kvp in iImageList)
            {
                NSImage img = GetImage(kvp.Value);
                iImageLocations.Add(kvp.Value.OriginalString);
                ButtonImage.AddItemWithTitle(@"");
                ButtonImage.LastItem.Tag = (int)kvp.Key;
                if (img != null)
                {
                    ButtonImage.LastItem.Image = img;
                }
            }
            if (iImageList.Count > 0)
            {
                ButtonImage.SelectItemAtIndex(0);
            }
            ToggleImageListVisibility();
        }

        private NSImage GetImage(Uri aUri)
        {
            ArtworkCache.Item item = ArtworkCacheInstance.Instance.Artwork(aUri);
            NSImage img = null;
            if (item != null)
            {
                if (!item.Failed)
                {
                    img = item.Image;
                }
                else
                {
                    img = Properties.Resources.IconLoading;
                }
            }
            if (img != null)
            {
                NSSize size = img.Size;
                size = new NSSize(kMaxImageSize, (size.height / size.width) * kMaxImageSize);
                img.Size = size;
            }
            return img;
        }

        private void ToggleImageListVisibility()
        {
            if (iShowingImage && iImageList.Count == 0)
            {
                ButtonImage.IsHidden = true;
                LabelImage.IsHidden = true;
                Window.SetFrameDisplay(new NSRect(Window.Frame.origin.x, Window.Frame.origin.y + kImageControlsHeight, Window.Frame.Width, Window.Frame.Height - kImageControlsHeight), true);
                BoxContainer.SetFrameOrigin(new NSPoint(BoxContainer.Frame.origin.x, BoxContainer.Frame.origin.y + kImageControlsHeight));
                iShowingImage = false;
            }
            else if (!iShowingImage && iImageList.Count != 0)
            {
                ButtonImage.IsHidden = false;
                LabelImage.IsHidden = false;
                Window.SetFrameDisplay(new NSRect(Window.Frame.origin.x, Window.Frame.origin.y - kImageControlsHeight, Window.Frame.Width, Window.Frame.Height + kImageControlsHeight), true);
                BoxContainer.SetFrameOrigin(new NSPoint(BoxContainer.Frame.origin.x, BoxContainer.Frame.origin.y - kImageControlsHeight));
                iShowingImage = true;
            }
        }

        [ObjectiveCField]
        public NSButton ButtonClose;

        [ObjectiveCField]
        public NSButton ButtonSave;

        [ObjectiveCField]
        public NSPopUpButton ButtonLocation;

        [ObjectiveCField]
        public NSTextField TextName;

        [ObjectiveCField]
        public NSTextField TextDescription;

        [ObjectiveCField]
        public NSPopUpButton ButtonImage;

        [ObjectiveCField]
        public NSTextField LabelImage;

        [ObjectiveCField]
        public NSBox BoxContainer;

        private IDictionary<uint, System.Uri> iImageList;
        //private List<NSImage> iImages;
        private List<string> iImageLocations;
        private ISaveSupport iSaveSupport;
        private const int kMaxImageSize = 50;
        private const int kImageControlsHeight = 60;
        private bool iShowingImage = true;
    }
}

