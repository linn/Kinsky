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
        private static readonly string kApiKey = "129c76d1b4043e568d19a9fea8a1f5534cdae703";

        [STAThread()]
        static void Main()
        {
            App app = new App();
            app.Run();
        }

        public App()
        {
#if DEBUG
            Xamarin.Insights.Initialize(Xamarin.Insights.DebugModeKey);
#else
            Xamarin.Insights.Initialize(kApiKey);
#endif
            InitializeComponent();
        }

    }
}
