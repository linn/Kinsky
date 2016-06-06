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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;
using System.Collections;
using System.Windows.Controls.Primitives;

namespace KinskyDesktopWpf
{
    /// <summary>
    /// Interaction logic for KinskyDesktop.xaml
    /// </summary>
    public partial class ViewKinsky : UserControl
    {

        public ViewKinsky()
        {
            InitializeComponent();
            this.DataContext = this;
            iPopupOpen = false;
            sliderSize.KeyDown += new KeyEventHandler(sliderSize_KeyDown);
            this.SizeChanged += new SizeChangedEventHandler(ViewKinsky_SizeChanged);
        }

        void ViewKinsky_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.NewSize.IsEmpty)
            {
                FixSplitBarSizes(e.NewSize.Width - 20); // 20px adjustment for margins etc.
            }
        }

        private void FixSplitBarSizes(double aViewWidth)
        {
            double splitterMinWidth = (double)Application.Current.Resources["SplitterMinWidth"];
            double sizeRatio = SplitBarPositionLeft.Value / (SplitBarPositionLeft.Value + SplitBarPositionRight.Value);
            double leftPosition = Math.Min(Math.Max(aViewWidth * sizeRatio, splitterMinWidth + 1), aViewWidth - splitterMinWidth);
            SplitBarPositionLeft = new GridLength(leftPosition, GridUnitType.Star);
            SplitBarPositionRight = new GridLength(aViewWidth - leftPosition, GridUnitType.Star);
        }

        void sliderSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Escape)
            {
                popupSlider.IsOpen = false;
            }
        }

        internal void Initialise(KinskyDesktop aKinskyDesktop,
                                 HelperKinsky aHelperKinsky,
                                ContentDirectoryLocator aLocator,
                               IViewSaveSupport aSaveSupport,
                               IPlaylistSupport aPlaylistSupport,
                               DropConverter aBrowseDropConverter,
                               DropConverter aViewDropConverter,
                               ViewMaster aViewMaster,
                               UiOptions aUIOptions,
                               OptionBool aPlaylistGroupingOption,
                               ModelSenders aSenders
                               )
        {
            iHelperKinsky = aHelperKinsky;
            iLocator = aLocator;
            iBrowseDropConverter = aBrowseDropConverter;
            iViewMaster = aViewMaster;
            iUIOptions = aUIOptions;
            iSenders = aSenders;
            iPlaylistSupport = aPlaylistSupport;
            aPlaylistSupport.EventIsDraggingChanged += new EventHandler<EventArgs>(aPlaylistSupport_EventIsDraggingChanged);
            iViewDropConverter = aViewDropConverter;
            iBrowser = new Browser(new Location(iLocator.Root));

            ViewWidgetButton upButtonWidgetBrowser = new ViewWidgetButton(buttonUpBrowser);

            breadcrumbBrowser.SetButtonUpDirectory(upButtonWidgetBrowser);

            iNavigationController = new NavigationController(iBrowser, iLocator, breadcrumbBrowser, iHelperKinsky);

            iBookmarks = new ViewWidgetBookmarks(iHelperKinsky,
                                                                    iHelperKinsky.BookmarkManager,
                                                                    lstBookmarks,
                                                                    iBrowser,
                                                                    popupBookmarksList,
                                                                    popupAddBookmark,
                                                                    buttonShowBookmarksList,
                                                                    buttonShowAddBookmark,
                                                                    buttonAddBookmark,
                                                                    buttonCancelAddBookmark,
                                                                    txtTitle,
                                                                    txtBreadcrumb,
                                                                    pnlAddBookmark,
                                                                    iNavigationController);

            iBrowserWidget = new ViewWidgetBrowser(iPlaylistSupport,
                                                iBrowseDropConverter,
                                                buttonChangeSize,
                                                buttonToggleListView,
                                                iUIOptions,
                                                sliderSize,
                                                iNavigationController,
                                                iBookmarks);
            pnlBrowser.Children.Add(iBrowserWidget);

            iRoomSelector = new ViewWidgetSelectorRoom(lstRooms, ctlSelectRoom, btnStandbyAll);
            IPlaylistWidget playlistMediaRenderer = new PlaylistWidget(aViewDropConverter, aPlaylistSupport, iUIOptions);
            IPlaylistWidget playlistRadio = new PlaylistWidget(aViewDropConverter, aPlaylistSupport, iUIOptions);
            IPlaylistWidget playlistReceiver = new PlaylistWidget(aViewDropConverter, aPlaylistSupport, iUIOptions);
            iViewWidgetPlaylistMediaRenderer = new ViewWidgetPlaylistMediaRenderer(pnlPlaylist, playlistMediaRenderer, aSaveSupport, aPlaylistSupport, aPlaylistGroupingOption.Native);
            iViewWidgetPlaylistRadio = new ViewWidgetPlaylistRadio(pnlPlaylist, playlistRadio, aSaveSupport);
            iViewWidgetPlaylistAux = new ViewWidgetPlaylistAux(pnlPlaylist);
            iViewWidgetPlaylistDiscPlayer = new ViewWidgetPlaylistDiscPlayer(pnlPlaylist);
            iViewWidgetPlaylistReceiver = new ViewWidgetPlaylistReceiver(pnlPlaylist, playlistReceiver, aSaveSupport, iSenders);

            iViewWidgetButtonStandby = new ViewWidgetButtonStandby(this);
            iViewWidgetButtonSave = new ViewWidgetButtonSave(buttonSave, aViewDropConverter, aSaveSupport);
            iViewWidgetButtonWasteBin = new ViewWidgetButtonWasteBin(buttonDelete, aViewDropConverter);

            iViewWidgetPlayMode = new ViewWidgetPlayMode(buttonRepeat, buttonShuffle);

            iViewWidgetTransportControl = new ViewWidgetTransportControl(aKinskyDesktop, threeKArray, aViewDropConverter, aPlaylistSupport);

            iViewWidgetPlayNowNextLater = new ViewWidgetPlayNowNextLater(aViewDropConverter, aPlaylistSupport, buttonPlayNow, buttonPlayNext, buttonPlayLater);

            iViewWidgetVolumeControl = new ViewWidgetVolumeControl(aKinskyDesktop, rotaryControlVolume, rockerControlVolume);
            iViewWidgetMediaTime = new ViewWidgetMediaTime(rotaryControlMediaTime, rockerControlMediaTime);
            iViewWidgetTrack = new ViewWidgetTrack(viewWidgetTrackDisplay, new IPlaylistWidget[] { playlistMediaRenderer, playlistRadio, playlistReceiver });
            SplitBarPositionLeft = new GridLength(iUIOptions.BrowserSplitterLocationLeft, GridUnitType.Star);
            SplitBarPositionRight = new GridLength(iUIOptions.BrowserSplitterLocationRight, GridUnitType.Star);
            iSourceSelector = new ViewWidgetSelectorSource(lstSources, ctlSelectSource);

            iMediatorHouse = new MediatorHouse(iRoomSelector,
                                               iSourceSelector,
                                               pnlPlaylist,
                                               lstRooms,
                                               lstSources,
                                               buttonSelectRoom,
                                               buttonSelectSource,
                                               popupRoomSelection,
                                               popupSourceSelection, 
                                               btnStandbyAll);;

            aPlaylistGroupingOption.EventValueChanged += (d, e) =>
            {
                iViewWidgetPlaylistMediaRenderer.SetGroupByAlbum(aPlaylistGroupingOption.Native);
            };

            iInitialised = true;
        }

        void aPlaylistSupport_EventIsDraggingChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                IsDragging = iPlaylistSupport.IsDragging();
            }));
        }

        private void popupSlider_Closed(object sender, EventArgs e)
        {
            if (!buttonChangeSize.IsPressed)
            {
                iPopupOpen = false;
            }
        }

        private void buttonChangeSize_Click(object sender, RoutedEventArgs e)
        {
            if (iInitialised)
            {
                if (iPopupOpen)
                {
                    popupSlider.IsOpen = false;
                    iPopupOpen = false;
                }
                else
                {
                    iPopupOpen = true;
                    popupSlider.IsOpen = true;
                    sliderSize.Focus();
                }
            }
        }

        internal void Start()
        {
            iBookmarks.Open();
            iBrowserWidget.Open();
            iNavigationController.Open();
            iViewMaster.ViewWidgetSelectorRoom.Add(iViewWidgetPlaylistReceiver);
            iViewMaster.ViewWidgetSelectorRoom.Add(iRoomSelector);
            iViewMaster.ViewWidgetButtonStandby.Add(iViewWidgetButtonStandby);
            iViewMaster.ViewWidgetSelectorSource.Add(iSourceSelector);
            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeControl);
            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetMediaTime);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Add(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Add(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlRadio.Add(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Add(iViewWidgetPlayNowNextLater);
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Add(iViewWidgetPlayNowNextLater);
            iViewMaster.ViewWidgetTransportControlRadio.Add(iViewWidgetPlayNowNextLater);
            iViewMaster.ViewWidgetTrack.Add(iViewWidgetTrack);
            iViewMaster.ViewWidgetPlayMode.Add(iViewWidgetPlayMode);
            iViewMaster.ViewWidgetPlaylist.Add(iViewWidgetPlaylistMediaRenderer);
            iViewMaster.ViewWidgetPlaylist.Add(iViewWidgetTrack);
            iViewMaster.ViewWidgetPlaylistRadio.Add(iViewWidgetPlaylistRadio);
            iViewMaster.ViewWidgetPlaylistRadio.Add(iViewWidgetTrack);
            iViewMaster.ViewWidgetPlaylistAux.Add(iViewWidgetPlaylistAux);
            iViewMaster.ViewWidgetPlaylistDiscPlayer.Add(iViewWidgetPlaylistDiscPlayer);
            iViewMaster.ViewWidgetPlaylistReceiver.Add(iViewWidgetPlaylistReceiver);
            iViewMaster.ViewWidgetPlaylistReceiver.Add(iViewWidgetTrack);
            iViewMaster.ViewWidgetButtonSave.Add(iViewWidgetButtonSave);
            iViewMaster.ViewWidgetButtonWasteBin.Add(iViewWidgetButtonWasteBin);
        }

        internal void Stop()
        {
            iBookmarks.Close();
            iBrowserWidget.Close();
            iNavigationController.Close();
            iViewMaster.ViewWidgetSelectorRoom.Remove(iRoomSelector);
            iViewMaster.ViewWidgetSelectorRoom.Remove(iViewWidgetPlaylistReceiver);
            iViewMaster.ViewWidgetButtonStandby.Remove(iViewWidgetButtonStandby);
            iViewMaster.ViewWidgetSelectorSource.Remove(iSourceSelector);
            iViewMaster.ViewWidgetVolumeControl.Remove(iViewWidgetVolumeControl);
            iViewMaster.ViewWidgetMediaTime.Remove(iViewWidgetMediaTime);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Remove(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Remove(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlRadio.Remove(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Remove(iViewWidgetPlayNowNextLater);
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Remove(iViewWidgetPlayNowNextLater);
            iViewMaster.ViewWidgetTransportControlRadio.Remove(iViewWidgetPlayNowNextLater);
            iViewMaster.ViewWidgetTrack.Remove(iViewWidgetTrack);
            iViewMaster.ViewWidgetPlayMode.Remove(iViewWidgetPlayMode);
            iViewMaster.ViewWidgetPlaylist.Remove(iViewWidgetPlaylistMediaRenderer);
            iViewMaster.ViewWidgetPlaylist.Remove(iViewWidgetTrack);
            iViewMaster.ViewWidgetPlaylistRadio.Remove(iViewWidgetPlaylistRadio);
            iViewMaster.ViewWidgetPlaylistRadio.Remove(iViewWidgetTrack);
            iViewMaster.ViewWidgetPlaylistAux.Remove(iViewWidgetPlaylistAux);
            iViewMaster.ViewWidgetPlaylistDiscPlayer.Remove(iViewWidgetPlaylistDiscPlayer);
            iViewMaster.ViewWidgetPlaylistReceiver.Remove(iViewWidgetPlaylistReceiver);
            iViewMaster.ViewWidgetPlaylistReceiver.Remove(iViewWidgetTrack);
            iViewMaster.ViewWidgetButtonSave.Remove(iViewWidgetButtonSave);
            iViewMaster.ViewWidgetButtonWasteBin.Remove(iViewWidgetButtonWasteBin);
        }

        public void Rescan()
        {
            if (iHelperKinsky != null)
            {
                iLocator.Refresh();
                iHelperKinsky.Rescan();
            }
        }


        public void SetShowExtendedTrackInfo(bool aShow)
        {
            viewWidgetTrackDisplay.ShowExtendedInformation = aShow;
        }


        public GridLength SplitBarPositionLeft
        {
            get { return (GridLength)GetValue(SplitBarPositionLeftProperty); }
            set { SetValue(SplitBarPositionLeftProperty, value); }
        }

        public static readonly DependencyProperty SplitBarPositionLeftProperty =
            DependencyProperty.Register("SplitBarPositionLeft", typeof(GridLength), typeof(ViewKinsky), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) =>
            {
                ViewKinsky view = d as ViewKinsky;
                if (view.iInitialised && view.centrePanel.ColumnDefinitions[0].ActualWidth > 0)
                {
                    view.iUIOptions.BrowserSplitterLocationLeft = (int)view.centrePanel.ColumnDefinitions[0].ActualWidth;
                }
            }));

        public GridLength SplitBarPositionRight
        {
            get { return (GridLength)GetValue(SplitBarPositionRightProperty); }
            set { SetValue(SplitBarPositionRightProperty, value); }
        }

        public static readonly DependencyProperty SplitBarPositionRightProperty =
            DependencyProperty.Register("SplitBarPositionRight", typeof(GridLength), typeof(ViewKinsky), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) =>
            {
                ViewKinsky view = d as ViewKinsky;
                if (view.iInitialised && view.centrePanel.ColumnDefinitions[1].ActualWidth > 0)
                {
                    view.iUIOptions.BrowserSplitterLocationRight = (int)view.centrePanel.ColumnDefinitions[1].ActualWidth;
                }
            }));


        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging", typeof(bool), typeof(ViewKinsky), new UIPropertyMetadata(false));

        public bool ShowFullScreenArtwork
        {
            get { return (bool)GetValue(ShowFullScreenArtworkProperty); }
            set { SetValue(ShowFullScreenArtworkProperty, value); }
        }

        public static readonly DependencyProperty ShowFullScreenArtworkProperty =
            DependencyProperty.Register("ShowFullScreenArtwork", typeof(bool), typeof(ViewKinsky), new UIPropertyMetadata(false, (d, e) =>
            {
                if ((bool)e.NewValue)
                {
                    ViewKinsky sender = d as ViewKinsky;
                    sender.fullScreenArtwork.Visibility = Visibility.Visible;
                    sender.fullScreenArtwork.RenderTransform = new TranslateTransform();

                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("Height"), 80, sender.mainView.ActualHeight, AnimationExtensions.kUIElementAnimationDuration, () =>
                    {
                    });
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("Width"), 80, sender.mainView.ActualWidth, AnimationExtensions.kUIElementAnimationDuration, () =>
                    {
                    });
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"), 10, 0, AnimationExtensions.kUIElementAnimationDuration, () => { });
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"), 10, 0, AnimationExtensions.kUIElementAnimationDuration, () => { });
                    sender.fullScreenArtwork.AnimateVisibility(true, () => { });
                    sender.fullScreenArtworkImage.Focus();
                }
                else
                {
                    ViewKinsky sender = d as ViewKinsky;
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("Height"), sender.mainView.ActualHeight, 80, AnimationExtensions.kUIElementAnimationDuration, () =>
                    {
                        sender.fullScreenArtwork.Visibility = Visibility.Collapsed;
                        sender.fullScreenArtwork.RenderTransform = null;
                    });
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("Width"), sender.mainView.ActualWidth, 80, AnimationExtensions.kUIElementAnimationDuration, () =>
                    {
                    });
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"), 0, 10, AnimationExtensions.kUIElementAnimationDuration, () => { });
                    sender.fullScreenArtwork.DoubleAnimate(new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"), 0, 10, AnimationExtensions.kUIElementAnimationDuration, () => { });
                    sender.fullScreenArtwork.AnimateVisibility(false, () => { });
                }
                WindowChrome chrome = (d as ViewKinsky).FindVisualAncestor<WindowChrome>();
                chrome.HideButtons = (bool)e.NewValue;
            }));


        public void OpenTrackDialog_Click(object sender, ScreenCoordinatesRoutedEventArgs args)
        {
            WindowChrome window = this.FindVisualAncestor<WindowChrome>();
            if ((bool)window.GetValue(WindowChrome.IsMiniModeActiveProperty) == false)
            {
                ShowFullScreenArtwork = true;
            }
        }

        private void btnStandby_Click(object sender, RoutedEventArgs e)
        {
            Linn.Kinsky.Room room = ((e.OriginalSource as FrameworkElement).FindVisualAncestor<ListViewItem>().Content as RoomViewModel).WrappedItem;
            room.Standby = true;
            e.Handled = true;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            iRefreshTimer = new System.Threading.Timer((a) =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (iRefreshTimer != null)
                    {
                        iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        iRefreshTimer.Dispose();
                        iRefreshTimer = null;
                    }
                    ShowThrobbers(false);
                }));
            });
            iRefreshTimer.Change(kRefreshTimeout, Timeout.Infinite);
            ShowThrobbers(true);
            Rescan();
        }

        private void ShowThrobbers(bool aShow)
        {
            btnRefreshRooms.Visibility = aShow ? Visibility.Collapsed : Visibility.Visible;
            btnRefreshSources.Visibility = aShow ? Visibility.Collapsed : Visibility.Visible;
            progressRefreshRooms.Visibility = aShow ? Visibility.Visible : Visibility.Collapsed;
            progressRefreshRooms.IsEnabled = aShow;
            progressRefreshRooms.IsAnimating = aShow;
            progressRefreshSources.Visibility = aShow ? Visibility.Visible : Visibility.Collapsed;
            progressRefreshSources.IsEnabled = aShow;
            progressRefreshSources.IsAnimating = aShow;
        }

        private void topPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DateTime clickTime = DateTime.Now;
            if (clickTime.CompareTo(iLastTopPanelMouseDown.AddMilliseconds(iMouseDoubleClickTime)) < 0)
            {
                WindowChrome mainWindowChrome = this.FindVisualAncestor<WindowChrome>();
                if (!mainWindowChrome.IsAnimating)
                {
                    WindowChrome.SetIsMiniModeActive(mainWindowChrome, !WindowChrome.GetIsMiniModeActive(mainWindowChrome));
                }
                clickTime = DateTime.MinValue;
            }
            else
            {
                iLastTopPanelMouseDown = clickTime;
            }
        }

        private void fullScreenArtworkButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ShowFullScreenArtwork)
            {
                Point screenTopLeft = Window.GetWindow(this).PointToScreen(new Point(0, 0));
                try
                {
                    Window.GetWindow(this).DragMove();
                }
                catch (InvalidOperationException ex)
                {
                    UserLog.WriteLine("InvalidOperationException caught in DragMove()" + ex);
                }
                Point newScreenTopLeft = Window.GetWindow(this).PointToScreen(new Point(0, 0));
                if (Math.Abs(screenTopLeft.X - newScreenTopLeft.X) <= SystemParameters.MinimumHorizontalDragDistance &&
                        Math.Abs(screenTopLeft.Y - newScreenTopLeft.Y) <= SystemParameters.MinimumVerticalDragDistance)
                {
                    ShowFullScreenArtwork = false;
                }
                e.Handled = true;
            }
        }

        private void fullScreenArtworkImage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Space)
            {
                ShowFullScreenArtwork = false;
            }
            e.Handled = true;
        }

        private void Bookmark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool isTextBox = e.OriginalSource is TextBox || e.OriginalSource.GetType().FullName == "System.Windows.Controls.TextBoxView";
            if (!isTextBox)
            {
                iBookmarks.CloseEditor();
            }
        }

        private ViewWidgetPlaylistMediaRenderer iViewWidgetPlaylistMediaRenderer;
        private ViewWidgetPlaylistRadio iViewWidgetPlaylistRadio;
        private IViewWidgetButton iViewWidgetButtonStandby;
        private IViewWidgetButton iViewWidgetButtonSave;
        private IViewWidgetButton iViewWidgetButtonWasteBin;
        private IViewWidgetPlayMode iViewWidgetPlayMode;
        private IViewWidgetTransportControl iViewWidgetTransportControl;
        private IViewWidgetVolumeControl iViewWidgetVolumeControl;
        private IViewWidgetMediaTime iViewWidgetMediaTime;
        private IViewWidgetPlaylistAux iViewWidgetPlaylistAux;
        private IViewWidgetPlaylistDiscPlayer iViewWidgetPlaylistDiscPlayer;
        private ViewWidgetPlaylistReceiver iViewWidgetPlaylistReceiver;
        private DropConverter iViewDropConverter;
        private bool iPopupOpen;

        private ViewWidgetSelectorRoom iRoomSelector;
        private ViewWidgetSelectorSource iSourceSelector;
        private ViewWidgetTrack iViewWidgetTrack;
        private ViewMaster iViewMaster;
        internal UiOptions iUIOptions;
        private IPlaylistSupport iPlaylistSupport;
        private ViewWidgetBrowser iBrowserWidget;
        private IBrowser iBrowser;
        private HelperKinsky iHelperKinsky;

        private const double kRoomSelectorHeight = 40;
        private const double kRoomSelectorWidth = 500;
        private ModelSenders iSenders;
        private MediatorHouse iMediatorHouse;
        private bool iInitialised = false;

        private ContentDirectoryLocator iLocator;
        private DropConverter iBrowseDropConverter;
        private ViewWidgetPlayNowNextLater iViewWidgetPlayNowNextLater;
        private DateTime iLastTopPanelMouseDown = DateTime.MinValue;
        private static uint iMouseDoubleClickTime = NativeWindowMethods.GetDoubleClickTime();
        private ViewWidgetBookmarks iBookmarks;
        private NavigationController iNavigationController;
        private System.Threading.Timer iRefreshTimer;
        private const int kRefreshTimeout = 5000;
    }

}
