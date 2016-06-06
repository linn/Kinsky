using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Linn.Kinsky;
using Linn.ControlPoint.Upnp;
using Linn.Control.Ssdp;
using System.Net;
using Linn;
using System.Collections.ObjectModel;
using System.IO;

namespace KinskyDesktopWpf
{
    /// <summary>
    /// Interaction logic for KinskyDesktop.xaml
    /// </summary>
    public partial class SavePlaylistDialog : Window
    {
        //private string iDirectory;
        //private string iFilename;
        private ISaveSupport iSaveSupport;
        private UiOptions iUiOptions;
        public SavePlaylistDialog(ISaveSupport aSaveSupport, UiOptions aUiOptions)
        {
            InitializeComponent();
            iSaveSupport = aSaveSupport;
            iSaveSupport.EventImageListChanged += EventImageListChangedHandler;
            iSaveSupport.EventSaveLocationsChanged += EventSaveLocationsChangedHandler;
            iSaveSupport.EventSaveLocationChanged += EventSaveLocationChangedHandler;
            UpdateLocations();
            UpdateLocation();
            UpdateImages();
            txtFilename.Text = iSaveSupport.DefaultName;
            this.KeyUp += SavePlaylistDialog_KeyUp;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                new ExecutedRoutedEventHandler(delegate(object sender, ExecutedRoutedEventArgs args) { this.CloseWindow(); })));
            iUiOptions = aUiOptions;
            this.Loaded += LoadedHandler;
        }

        void LoadedHandler(object sender, RoutedEventArgs e)
        {
            iUiOptions.DialogSettings.Register(this, "SavePlaylist");
            this.Loaded -= LoadedHandler;
        }

        void SavePlaylistDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Save())
                {
                    CloseWindow();
                }
            }
        }

        void EventSaveLocationsChangedHandler(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                UpdateLocations();
            }));
        }

        void EventSaveLocationChangedHandler(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                UpdateLocation();
            }));
        }

        void EventImageListChangedHandler(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                UpdateImages();
            }));
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (Save())
            {
                CloseWindow();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }


        private void UpdateLocations()
        {
            cmbLocation.ItemsSource = iSaveSupport.SaveLocations;
        }

        private void UpdateLocation()
        {
            string selected = cmbLocation.SelectedItem as string;
            if (selected != iSaveSupport.SaveLocation)
            {
                cmbLocation.SelectedItem = iSaveSupport.SaveLocation;
            }
            // if we failed to find a selected item, set it to first item in the list
            if (cmbLocation.SelectedItem == null && cmbLocation.Items.Count > 0)
            {
                cmbLocation.SelectedIndex = 0;
            }
        }

        private void UpdateImages()
        {
            List<ImageItem> images = new List<ImageItem>();
            foreach (KeyValuePair<uint, Uri> kvp in iSaveSupport.ImageList)
            {
                images.Add(new ImageItem(kvp.Key, kvp.Value));
            }     

            cmbImages.ItemsSource = images;
            if (images.Count > 0)
            {
                cmbImages.SelectedIndex = 0;
            }
            Visibility visibility = images.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            lblImages.Visibility = visibility;
            cmbImages.Visibility = visibility;
        }

        private bool Save()
        {
            try
            {
                uint imageId = 0;
                if (cmbImages.SelectedItem != null)
                {
                    imageId = (cmbImages.SelectedItem as ImageItem).Id;
                }
                iSaveSupport.Save(txtFilename.Text, txtDescription.Text, imageId);
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception in iSaveSupport.Save(): " + ex);
                MessageBox.Show(string.Format("An error occurred saving playlist: {0}", ex.Message), "Error saving playlist...", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void CloseWindow()
        {
            iSaveSupport.EventImageListChanged += EventImageListChangedHandler;
            iSaveSupport.EventSaveLocationsChanged += EventSaveLocationsChangedHandler;
            iSaveSupport.EventSaveLocationChanged += EventSaveLocationChangedHandler;
            this.KeyUp -= SavePlaylistDialog_KeyUp;
            Close();
        }

        private void cmbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = cmbLocation.SelectedItem as string;            
            if (selected != null && selected != iSaveSupport.SaveLocation)
            {
                iSaveSupport.SaveLocation = cmbLocation.SelectedItem as string;
            }
        }

        private class ImageItem
        {
            public ImageItem(uint aId, Uri aUri)
            {
                Id = aId;
                Uri = aUri;
            }
            public uint Id { get; set; }
            public Uri Uri { get; set; }
        }

    }
}