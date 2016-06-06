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
    public partial class BookmarkNotFoundDialog : Window
    {
        private BookmarkManager iBookmarkManager;
        private Bookmark iBookmark;

        public BookmarkNotFoundDialog(BookmarkManager aBookmarkManager, Bookmark aBookmark)
        {
            InitializeComponent();
            iBookmarkManager = aBookmarkManager;
            iBookmark = aBookmark;
            txtNotFound.Text = string.Format("Bookmark '{0}' was not found.  Do you wish to delete it?", aBookmark.Title);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            iBookmarkManager.Remove(iBookmark);         
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
