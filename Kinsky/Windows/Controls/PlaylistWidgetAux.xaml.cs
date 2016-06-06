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
using Linn;
using Linn.Kinsky;
using Linn.ControlPoint.Upnp;
using Linn.Control.Ssdp;
using Linn.Topology;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;
using Upnp;
using System.Collections;
using System.Globalization;

namespace KinskyDesktopWpf
{
   
    public partial class PlaylistWidgetAux : UserControl
    {
        public PlaylistWidgetAux()
        {
            InitializeComponent();
        }

        public ImageSource AuxImageSource
        {
            get { return (ImageSource)GetValue(AuxImageSourceProperty); }
            set { SetValue(AuxImageSourceProperty, value); }
        }

        public static readonly DependencyProperty AuxImageSourceProperty =
            DependencyProperty.Register("AuxImageSource", typeof(ImageSource), typeof(PlaylistWidgetAux), new UIPropertyMetadata(null));

        public bool IsSaveEnabled
        {
            get
            {
                return this.IsVisible;
            }
        }
    }
}
