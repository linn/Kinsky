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
    public partial class UserLogDialog : Window, IUserLogListener
    {

        private UiOptions iUiOptions;
        public UserLogDialog(UiOptions aUiOptions)
        {
            InitializeComponent();
            UserLog.AddListener(this);
            txtUserLog.Text = TruncateText(UserLog.Text);
            iUiOptions = aUiOptions;
            this.Loaded += LoadedHandler;
        }

        void LoadedHandler(object sender, RoutedEventArgs e)
        {
            iUiOptions.DialogSettings.Register(this, "UserLog");
            this.Loaded -= LoadedHandler;
        }

        private string TruncateText(string aText)
        {
            if (aText.Length >= txtUserLog.MaxLength)
            {
                return aText.Remove(0, aText.Length - txtUserLog.MaxLength);
            }

            return aText;
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

        #region IUserLogListener Members

        public void Write(string aMessage)
        {
            txtUserLog.Dispatcher.BeginInvoke((Action)(()=>{
                txtUserLog.Text = TruncateText(UserLog.Text);
            }));
        }

        public void WriteLine(string aMessage)
        {
            txtUserLog.Dispatcher.BeginInvoke((Action)(() =>
            {
                txtUserLog.Text = TruncateText(UserLog.Text);
            }));
        }

        #endregion

        private void UserLogDialog_Closed(object sender, EventArgs args)
        {
            UserLog.RemoveListener(this);
        }

        private void Button_CopyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(UserLog.Text);
            }
            catch { }
        }
    }
}
