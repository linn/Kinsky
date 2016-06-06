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
using System.Drawing;

namespace KinskyDesktopWpf
{

    public partial class ViewWidgetTrackDisplay : UserControl
    {
        public ViewWidgetTrackDisplay()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ImageSource Artwork
        {
            get { return (ImageSource)GetValue(ArtworkProperty); }
            set { SetValue(ArtworkProperty, value); }
        }
        public static readonly DependencyProperty ArtworkProperty =
            DependencyProperty.Register("Artwork", typeof(ImageSource), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(null));


        public string Display1
        {
            get { return (string)GetValue(Display1Property); }
            set { SetValue(Display1Property, value); }
        }
        public static readonly DependencyProperty Display1Property =
            DependencyProperty.Register("Display1", typeof(string), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(string.Empty));


        public string Display2
        {
            get { return (string)GetValue(Display2Property); }
            set { SetValue(Display2Property, value); }
        }
        public static readonly DependencyProperty Display2Property =
            DependencyProperty.Register("Display2", typeof(string), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(string.Empty));



        public string Display3
        {
            get { return (string)GetValue(Display3Property); }
            set { SetValue(Display3Property, value); }
        }

        public static readonly DependencyProperty Display3Property =
            DependencyProperty.Register("Display3", typeof(string), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(string.Empty));



        public uint Bitrate
        {
            get { return (uint)GetValue(BitrateProperty); }
            set { SetValue(BitrateProperty, value); }
        }

        public static readonly DependencyProperty BitrateProperty =
            DependencyProperty.Register("Bitrate", typeof(uint), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata());



        public float SampleRate
        {
            get { return (float)GetValue(SampleRateProperty); }
            set { SetValue(SampleRateProperty, value); }
        }

        public static readonly DependencyProperty SampleRateProperty =
            DependencyProperty.Register("SampleRate", typeof(float), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(0f));




        public uint BitDepth
        {
            get { return (uint)GetValue(BitDepthProperty); }
            set { SetValue(BitDepthProperty, value); }
        }

        public static readonly DependencyProperty BitDepthProperty =
            DependencyProperty.Register("BitDepth", typeof(uint), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata());




        public string Codec
        {
            get { return (string)GetValue(CodecProperty); }
            set { SetValue(CodecProperty, value); }
        }

        public static readonly DependencyProperty CodecProperty =
            DependencyProperty.Register("Codec", typeof(string), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(string.Empty));



        public bool Lossless
        {
            get { return (bool)GetValue(LosslessProperty); }
            set { SetValue(LosslessProperty, value); }
        }

        public static readonly DependencyProperty LosslessProperty =
            DependencyProperty.Register("Lossless", typeof(bool), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(false));


        public bool ShowExtendedInformation
        {
            get { return (bool)GetValue(ShowExtendedInformationProperty); }
            set { SetValue(ShowExtendedInformationProperty, value); }
        }

        public static readonly DependencyProperty ShowExtendedInformationProperty =
            DependencyProperty.Register("ShowExtendedInformation", typeof(bool), typeof(ViewWidgetTrackDisplay), new UIPropertyMetadata(true));

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ScreenCoordinatesRoutedEventArgs args = new ScreenCoordinatesRoutedEventArgs(ViewWidgetTrackDisplay.OpenTrackDialogClickEvent, this, new System.Windows.Point(
                this.PointToScreen(e.GetPosition(this)).X,
                this.PointToScreen(e.GetPosition(this)).Y));
            RaiseEvent(args);
            e.Handled = true;
        }


        public event ScreenCoordinatesRoutedEventHandler OpenTrackDialogClick
        {
            add { AddHandler(OpenTrackDialogClickEvent, value); }
            remove { RemoveHandler(OpenTrackDialogClickEvent, value); }
        }

        public static readonly RoutedEvent OpenTrackDialogClickEvent = EventManager.RegisterRoutedEvent(
            "OpenTrackDialogClick", RoutingStrategy.Bubble, typeof(ScreenCoordinatesRoutedEventHandler), typeof(ViewWidgetTrackDisplay));

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
            this.Artwork = KinskyDesktopWpf.StaticImages.ImageSourceIconAlbumError;
            img.SetBinding(System.Windows.Controls.Image.SourceProperty, "Artwork");
            e.Handled = true;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {

            System.Windows.Media.Color from = (System.Windows.Media.Color)Application.Current.FindResource("TextColour");
            System.Windows.Media.Color to = (System.Windows.Media.Color)Application.Current.FindResource("TextBrightColour");
            (sender as DependencyObject).AnimateTextColourIn(from, to);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {

            System.Windows.Media.Color from = (System.Windows.Media.Color)Application.Current.FindResource("TextBrightColour");
            System.Windows.Media.Color to = (System.Windows.Media.Color)Application.Current.FindResource("TextColour");
            (sender as DependencyObject).AnimateTextColourOut(from, to);
        }

        public Button TitleButton { get { return titleButton; } }

        private void Image_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                ScreenCoordinatesRoutedEventArgs args = new ScreenCoordinatesRoutedEventArgs(ViewWidgetTrackDisplay.OpenTrackDialogClickEvent, this, new System.Windows.Point(0,0));
                RaiseEvent(args);
            }
        }

    }
}
