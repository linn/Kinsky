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
using Upnp;

namespace KinskyDesktopWpf
{
    /// <summary>
    /// Interaction logic for KinskyDesktop.xaml
    /// </summary>
    public partial class DetailsDialog : Window
    {

        public DetailsDialog(upnpObject aUpnpObject, upnpObject aParent, UiOptions aUiOptions)
        {
            InitializeComponent();
            iUpnpObject = aUpnpObject;
            iParent = aParent;
            details.Content = new BrowserItem(aUpnpObject, aParent);
            details.ItemInfo = new ItemInfo(aUpnpObject, aParent);
            iUiOptions = aUiOptions;
            this.Loaded += LoadedHandler;
        }

        void LoadedHandler(object sender, RoutedEventArgs e)
        {
            iUiOptions.DialogSettings.Register(this, "Details");
            this.Loaded -= LoadedHandler;
        }

        private void ClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (details.Content is BrowserItem)
            {
                DidlLite didl = new DidlLite();
                if (iUpnpObject != null)
                {
                    didl.Add(iUpnpObject);
                }
                if (iParent != null)
                {
                    didl.Add(iParent);
                }
                try
                {
                    Clipboard.SetText(didl.Xml);
                }
                catch (Exception ex)
                {
                    UserLog.WriteLine("Clipboard failed: " + ex);
                }
            }
            else
            {
                UserLog.WriteLine("Clipboard not copied: " + details.Content);
            }
        }

        #region Command Bindings
        private void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion

        private UiOptions iUiOptions;
        private upnpObject iUpnpObject;
        private upnpObject iParent;
    }
}
