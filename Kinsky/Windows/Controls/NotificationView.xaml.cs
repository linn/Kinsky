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
        private bool iAcknowledged;
        
        public NotificationView()
        {
            InitializeComponent();            
        }

        public void Launch(INotification aNotification, Window aOwner)
        {
            iNotification = aNotification;
            iNotification.Shown();
            iAcknowledged = aNotification.HasBeenAcknowledged;
            var uri = iNotification.Uri(true);
            UserLog.WriteLine("Launch: " + uri);
            this.Owner = aOwner;
            this.ContentRendered += (s, e) =>
            {
                UserLog.WriteLine("Loading: " + uri);
                Browser.Address = uri;
            };
            this.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            iNotification.Closed(iAcknowledged);
            base.OnClosed(e);
        }

        private void Now_Click(object sender, RoutedEventArgs e)
        {
            iAcknowledged = true;
            iNotification.TrackUsageEventDismissed(true);
            Close();
            KinskyDesktop.GetKazoo();
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            iNotification.TrackUsageEventDismissed(false);
            Close();
        }
        
    }
}
