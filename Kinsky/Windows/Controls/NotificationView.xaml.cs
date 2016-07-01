using Linn;
using Linn.Kinsky;
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

namespace KinskyDesktopWpf.Controls
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class NotificationView : Window
    {
        private INotification iNotification;
        
        public NotificationView()
        {
            InitializeComponent();            
        }

        public void Launch(INotification aNotification, Window aOwner, bool aChecked)
        {
            iNotification = aNotification;
            UserLog.WriteLine("Launch: " + iNotification.Uri);
            this.Owner = aOwner;
            chkDontShowAgain.IsChecked = aChecked;
            this.ContentRendered += (s, e) =>
            {
                UserLog.WriteLine("Loading: " + iNotification.Uri);
                Browser.Address = iNotification.Uri;
            };
            this.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            iNotification.Closed(chkDontShowAgain.IsChecked.Value);
            base.OnClosed(e);
        }

        private void Now_Click(object sender, RoutedEventArgs e)
        {
            iNotification.TrackUsageEventDismissed(true, chkDontShowAgain.IsChecked.Value);
            Close();
            KinskyDesktop.GetKazoo();
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            iNotification.TrackUsageEventDismissed(false, chkDontShowAgain.IsChecked.Value);
            Close();
        }
        
    }
}
