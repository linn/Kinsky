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
    public partial class AboutDialog : Window
    {
        private UiOptions iUiOptions;
        public AboutDialog(HelperKinsky aHelperKinsky, string aSize, UiOptions aUiOptions)
        {
            InitializeComponent();
            if (aSize == KinskyDesktop.kFontOptionLarge)
            {
                Height = 180;
                Width = 480;
            }
            txtProduct.Text = aHelperKinsky.Product;
            txtVersion.Text = String.Format("Version: {0}", aHelperKinsky.Version);
            txtCopyright.Text = aHelperKinsky.Copyright;
            txtCompany.Text = aHelperKinsky.Company;
            txtDescription.Text = aHelperKinsky.Description;
            iUiOptions = aUiOptions;
            this.Loaded += LoadedHandler;
        }

        void LoadedHandler(object sender, RoutedEventArgs e)
        {
            iUiOptions.DialogSettings.Register(this, "About");
            this.Loaded -= LoadedHandler;
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
    }
}
