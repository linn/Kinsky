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
    public partial class StringInputDialog : Window
    {
        public StringInputDialog()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                new ExecutedRoutedEventHandler(delegate(object sender, ExecutedRoutedEventArgs args) { this.Close(); })));
        }

        public string Result()
        {
            return txtUserInput.Text;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {            
			DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
