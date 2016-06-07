using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Input;
using System.Drawing;
using System.Net;

namespace KinskyDesktopWpf
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [STAThread()]
        static void Main()
        {
            App app = new App();
            app.Run();
        }

        public App()
        {
            InitializeComponent();
        }

    }
}
