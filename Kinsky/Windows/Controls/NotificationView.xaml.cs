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
        private INotificationPersistence iPersistence;
        
        public NotificationView()
        {
            InitializeComponent();            
        }

        public void Launch(INotificationPersistence aPersistence, INotification aNotification, Window aOwner)
        {
            iNotification = aNotification;
            UserLog.WriteLine("Launch: " + iNotification.Uri);
            iPersistence = aPersistence;
            this.Owner = aOwner;
            chkDontShowAgain.IsChecked = aPersistence.LastNotificationVersion == aNotification.Version;
            this.ContentRendered += (s, e) =>
            {
                UserLog.WriteLine("Loading: " + iNotification.Uri);
                Browser.Address = iNotification.Uri;
            };
            this.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (chkDontShowAgain.IsChecked.Value)
            {
                iNotification.DontShowAgain();
            }
            else if (iPersistence.LastNotificationVersion == iNotification.Version)
            {
                iPersistence.LastNotificationVersion = 0;
            }
            base.OnClosed(e);
        }

        private void Now_Click(object sender, RoutedEventArgs e)
        {
            KinskyDesktop.GetKazoo();
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
    }
}
