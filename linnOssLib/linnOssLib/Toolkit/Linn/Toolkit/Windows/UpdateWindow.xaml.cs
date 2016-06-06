using System;
using System.Drawing;
using System.Windows;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Media.Animation;


namespace Linn.Toolkit.Wpf
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow(Icon aIcon, Bitmap aImage)
        {
            InitializeComponent();

            MemoryStream iconStream = new MemoryStream();
            aIcon.Save(iconStream);
            iconStream.Seek(0, SeekOrigin.Begin);
            Icon = BitmapFrame.Create(iconStream);

            image.Source = Imaging.CreateBitmapSourceFromHBitmap(aImage.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            iWindowHeightWidthDetails = this.Height;
            this.Height -= iDetailsHeight;
        }

        public UpdateWindow(Window aParent, Icon aIcon, Bitmap aImage)
            : this(aIcon, aImage)
        {
            Owner = aParent;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void buttonDetails_Click(object sender, RoutedEventArgs e)
        {
            if (this.Height == iWindowHeightWidthDetails)
            {
                this.Height = iWindowHeightWidthDetails - iDetailsHeight;
            }
            else
            {
                this.Height = iWindowHeightWidthDetails;
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            // For now, close the window. The handler below will prevent this from actually closing and will just
            // hide it but, also, the windows Closing event will be received by the ViewAutoUpdateStandard below
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private double iWindowHeightWidthDetails;
        private const double iDetailsHeight = 320;
    }


    // Implementation of the abstract ViewAutoUpdateStandard class in the Linn.Toolkit namespace
    public class ViewAutoUpdateStandard : Linn.Toolkit.ViewAutoUpdateStandard
    {
        public ViewAutoUpdateStandard(Icon aIcon, Bitmap aImage)
        {
            iView = new UpdateWindow(aIcon, aImage);

            iView.Closing += WindowClosing;
            iView.buttonUpdate.Click += ButtonUpdateClick;
            iView.checkBox.Click += ButtonAutoCheckClick;
        }

        #region Implementation of abstract interface

        protected override string Text1
        {
            set { iView.textBlock1.Text = value; }
        }

        protected override string Text2
        {
            set { iView.textBlock2.Text = value; }
        }

        protected override string ButtonCloseText
        {
            set { iView.buttonClose.Content = value; }
        }

        protected override void StartProgress(bool aIsIndeterminate)
        {
            iView.progressBar.IsIndeterminate = aIsIndeterminate;
            iView.progressBar.Value = 0;
        }

        protected override void StopProgress()
        {
            iView.progressBar.IsIndeterminate = false;
            iView.progressBar.Value = 0;
        }

        protected override bool ProgressHidden
        {
            set { iView.progressBar.Visibility = (value ? Visibility.Hidden : Visibility.Visible); }
        }

        protected override int ProgressValue
        {
            set { iView.progressBar.Value = value; }
        }

        protected override bool ButtonDetailsEnabled
        {
            set { iView.buttonDetails.IsEnabled = value; }
        }

        protected override bool ButtonUpdateEnabled
        {
            set { iView.buttonUpdate.IsEnabled = value; }
        }

        protected override bool ButtonCloseEnabled
        {
            set { iView.buttonClose.IsEnabled = value; }
        }

        protected override void SetButtonUpdateAsDefault()
        {
            iView.buttonClose.IsDefault = false;
            iView.buttonDetails.IsDefault = false;
            iView.buttonUpdate.IsDefault = true;
        }

        protected override void SetButtonCloseAsDefault()
        {
            iView.buttonDetails.IsDefault = false;
            iView.buttonUpdate.IsDefault = false;
            iView.buttonClose.IsDefault = true;
        }

        protected override bool ButtonAutoCheckHidden
        {
            set { iView.checkBox.Visibility = (value ? Visibility.Hidden : Visibility.Visible); }
        }

        protected override bool WindowHidden
        {
            set
            {
                if (!value)
                {
                    iView.Show();
                    iView.Focus();
                }
                else
                {
                    iView.Hide();
                }
            }
        }

        protected override string WebViewUri
        {
            set { iView.webBrowser.Navigate(value); }
        }

        protected override bool ShowCompatibilityBreak(string aButtonUpdate, string aButtonCancel, string aMessage, string aInformation)
        {
            MessageBoxResult r = MessageBox.Show(aMessage, aInformation, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return (r == MessageBoxResult.Yes);
        }

        public override bool ButtonAutoCheckOn
        {
            get { return iView.checkBox.IsChecked == true; }
            set { iView.checkBox.IsChecked = value; }
        }

        public override event EventHandler EventClosed;
        public override event EventHandler EventButtonUpdateClicked;
        public override event EventHandler EventButtonAutoCheckClicked;

        #endregion

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (EventClosed != null)
            {
                EventClosed(sender, EventArgs.Empty);
            }
        }

        private void ButtonUpdateClick(object sender, RoutedEventArgs e)
        {
            if (EventButtonUpdateClicked != null)
            {
                EventButtonUpdateClicked(sender, EventArgs.Empty);
            }
        }

        private void ButtonAutoCheckClick(object sender, RoutedEventArgs e)
        {
            if (EventButtonAutoCheckClicked != null)
            {
                EventButtonAutoCheckClicked(sender, EventArgs.Empty);
            }
        }

        private UpdateWindow iView;
    }
}
