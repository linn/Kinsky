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
using Upnp;

namespace KinskyDesktopWpf
{
    public partial class UpnpObjectDetailsDisplay : ContentControl
    {

        public UpnpObjectDetailsDisplay()
        {
            InitializeComponent();
        }


        public ItemInfo ItemInfo
        {
            get { return (ItemInfo)GetValue(ItemInfoProperty); }
            set { SetValue(ItemInfoProperty, value); }
        }

        public static readonly DependencyProperty ItemInfoProperty =
            DependencyProperty.Register("ItemInfo", typeof(ItemInfo), typeof(UpnpObjectDetailsDisplay), new UIPropertyMetadata(null));

        
    }

}
