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
        private static string kAppstoreUri = "https://www.microsoft.com/en-us/store/apps/linn-kazoo-beta/9nblggh4np11";

        public NotificationView()
        {
            InitializeComponent();
        }

        public void Launch(INotification aNotification, Window aOwner)
        {
            iNotification = aNotification;
            this.Owner = aOwner;
            var rendered = false;
            this.ContentRendered += (s, e) =>
            {
                if (!rendered)
                {
                    rendered = true;
                    Browser.Load(iNotification.Uri);
                }
            };
            
            this.ShowDialog();
        }
        
        private void Now_Click(object sender, RoutedEventArgs e)
        {
            Close();
            OpenStorePage();
        }

        private void Later_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            iNotification.DontShowAgain();
            Close();
        }
        
        public static void OpenStorePage()
        {
            System.Diagnostics.Process.Start(kAppstoreUri);
        }
    }
}
