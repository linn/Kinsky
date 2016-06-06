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
using System.Globalization;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;

namespace KinskyDesktopWpf
{

    public partial class ViewWidgetBrowser : UserControl
    {

        public ViewWidgetBrowser(IPlaylistSupport aSupport,
                                 DropConverter aDropConverter,
                                 ToggleButton aSizeButton,
                                 ToggleButton aViewButton,
                                 UiOptions aUIOptions,
                                 Slider aSlider,
                                 NavigationController aNavigationController,
                                 ViewWidgetBookmarks aViewWidgetBookmarks)
            : base()
        {
            InitializeComponent();
            iOpen = false;
            iNavigationController = aNavigationController;
            iViewWidgetBookmarks = aViewWidgetBookmarks;
            iNavigationController.EventNavigationStateChanged += EventNavigationStateChangedHandler;
            iNavigationController.EventLocationChanged += EventLocationChangedHandler;
            iSupport = aSupport;
            iDropConverter = aDropConverter;
            iUIOptions = aUIOptions;

            iViewIndex = iUIOptions.ContainerView;
            aViewButton.Click += aViewButton_EventClick;
            this.iSizeButton = aSizeButton;
            this.iViewButton = aViewButton;
            iSlider = aSlider;
            iSlider.PreviewMouseDown += new MouseButtonEventHandler(iSlider_PreviewMouseDown);
            iSlider.ValueChanged += aSlider_ValueChanged;

            iErrorMessage = string.Empty;
            SetSize();
            SetView();

            iDragHelper = new DragHelper(lstBrowser);
            iDragHelper.EventDragInitiated += new EventHandler<MouseEventArgs>(iDragHelper_EventDragInitiated);
            iDragHelperContainer = new DragHelper(pnlContainerInfo);
            iDragHelperContainer.EventDragInitiated += new EventHandler<MouseEventArgs>(iDragHelper_EventDragInitiated);

            lstBrowser.SelectionMode = SelectionMode.Extended;
            iScrollIndexCache = new Dictionary<string, int>();
            lstBrowser.AllowDrop = true;
            lstBrowser.MouseLeftButtonUp += new MouseButtonEventHandler(lstBrowser_MouseLeftButtonUp);
            UpdateCacheSize();
        }

        void lstBrowser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lstBrowser.SelectedItems.Count == 1 && iViewIndex == kThumbsView && !iIsAlbum)
            {
                lstBrowser.FindVisualChild<VirtualizingTilePanel>().EnsureVisible(lstBrowser.SelectedIndex);
            }
        }

        void iSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (iViewIndex == kThumbsView)
            {
                VirtualizingTilePanel panel = lstBrowser.FindVisualChild<VirtualizingTilePanel>();
                if (panel != null)
                {
                    iSliderStartIndex = panel.CurrentIndex;
                }
                else
                {
                    iSliderStartIndex = 0;
                }
            }
            else
            {
                iSliderStartIndex = 0;
            }
        }

        public bool ContainerInfoSelected
        {
            get { return (bool)GetValue(ContainerInfoSelectedProperty); }
            set { SetValue(ContainerInfoSelectedProperty, value); }
        }

        public static readonly DependencyProperty ContainerInfoSelectedProperty =
            DependencyProperty.Register("ContainerInfoSelected", typeof(bool), typeof(ViewWidgetBrowser), new FrameworkPropertyMetadata(false, (d, e) =>
            {
                ViewWidgetBrowser me = d as ViewWidgetBrowser;
                Color from = (Color)me.FindResource("TextColour");
                Color to = (Color)me.FindResource("TextBrightColour");
                me.pnlContainerInfo.AnimateTextColourOut(from, to);
            }));

        private void EventLocationChangedHandler(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                OnLocationChanged();
            }));
        }

        private void OnLocationChanged()
        {
            if (iErrorMessage != string.Empty)
            {
                iErrorMessage = string.Empty;
            }
            StopSearch();
            iSearchText = string.Empty;
            Location location = iNavigationController.Location;
            if (location != null)
            {
                iContainer = location.Current;

                if (iOpen)
                {
                    CancelContentCollector();
                    iNavigationState = ENavigationState.Navigating;
                    SetPanelState();
                    iContentCollector = ContentCollectorMaster.Create(iContainer, new ArrayBackedContentCache<upnpObject>(), kRangeSize, kThreadCount, 0);
                    iData = new BrowserList(iContentCollector, Dispatcher, iContainer);
                    lstBrowser.ItemsSource = iData;
                    iContentCollector.EventOpened += iContentCollector_EventOpened;
                    iContentCollector.EventItemsFailed += iContentCollector_EventItemsFailed;
                    iContentCollector.EventItemsLoaded += iContentCollector_EventItemsLoaded;
                }

                string currentLocation = location.ToString();
                List<string> keys = iScrollIndexCache.Keys.ToList<string>();
                foreach (string key in keys)
                {
                    if (!currentLocation.StartsWith(key))
                    {
                        iScrollIndexCache.Remove(key);
                    }
                }
            }
        }

        private void CancelContentCollector()
        {
            if (iData != null)
            {
                ScrollViewer sv = lstBrowser.GetScrollViewer();
                if (sv != null)
                {
                    sv.ScrollToTop();
                }
                iContentCollector.EventOpened -= iContentCollector_EventOpened;
                iContentCollector.EventItemsFailed -= iContentCollector_EventItemsFailed;
                iContentCollector.EventItemsLoaded -= iContentCollector_EventItemsLoaded;
                iContentCollector.Dispose();
                iData.Dispose();
                iData = null;
            }
        }

        void iContentCollector_EventItemsLoaded(object sender, EventArgsItemsLoaded<upnpObject> e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (sender == iContentCollector)
                {
                    for (int i = 0; i < e.Items.Count; i++)
                    {
                        if (e.StartIndex + i == iLastSelectedIndex)
                        {
                            lstBrowser.EnsureSelected(e.StartIndex + i);
                        }
                    }
                }
            }));
        }

        void iContentCollector_EventItemsFailed(object sender, EventArgsItemsFailed e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (sender == iContentCollector)
                {
                    iNavigationState = ENavigationState.Failed;
                    UserLog.WriteLine("Content collector failed: " + e.Exception);
                    iIsAlbum = false;
                    iErrorMessage = e.Exception.Message;
                    CancelContentCollector();
                    SetView();
                }
            }));
        }

        void iContentCollector_EventOpened(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (sender == iContentCollector)
                {
                    iNavigationState = ENavigationState.Navigated;
                    bool isAlbum = iIsAlbum;
                    iIsAlbum = iContainer.Metadata is musicAlbum;
                    pnlContainerInfo.Content = new BrowserItem(iContainer.Metadata, null);
                    ContainerInfoSelected = false;
                    if (iCurrentRenamingItem != null)
                    {
                        iCurrentRenamingItem = null;
                    }
                    if (isAlbum != iIsAlbum)
                    {
                        SetView();
                    }
                    else
                    {
                        SetPanelState();
                    }
                    string key = iNavigationController.Location.ToString();
                    if (iScrollIndexCache.ContainsKey(key))
                    {
                        iLastSelectedIndex = iScrollIndexCache[key];
                        lstBrowser.EnsureSelected(iLastSelectedIndex);
                    }
                    else
                    {
                        iLastSelectedIndex = 0;
                    }
                    for (int i = iContentCollector.Count - 1; i > 0; i -= kRangeSize)
                    {
                        // pre-cache items
                        iContentCollector.Item(i, ERequestPriority.Background);
                    }
                }
            }));
        }



        private void EventNavigationStateChangedHandler(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                iNavigationState = iNavigationController.NavigationState;
                if (iNavigationState == ENavigationState.Navigated && iNavigationController.Location != null)
                {
                    OnLocationChanged();
                }
                SetPanelState();
            }));
        }

        void aSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (iOpen)
            {
                if (iViewIndex == kThumbsView)
                {
                    iUIOptions.ContainerViewSizeThumbsView = e.NewValue;
                }
                else
                {
                    iUIOptions.ContainerViewSizeListView = e.NewValue;
                }
            }
            SetView();
            UpdateCacheSize();
        }

        void ContainerInfo_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && ContainerInfoSelected)
            {
                ContainerInfoSelected = false;
                foreach (BrowserItem item in lstBrowser.Items)
                {
                    if (lstBrowser.SelectedItems.Contains(item))
                    {
                        lstBrowser.SelectedItems.Remove(item);
                    }
                }
            }
            else
            {
                ContainerInfoSelected = true;
                foreach (BrowserItem item in lstBrowser.Items)
                {
                    if (!lstBrowser.SelectedItems.Contains(item))
                    {
                        lstBrowser.SelectedItems.Add(item);
                    }
                }
            }
            DateTime now = DateTime.Now;
            // double click
            if (iLastContainerPress.AddMilliseconds(500) > now)
            {
                iSupport.PlayNow(new MediaRetrieverNoRetrieve(SelectedUpnpObjects()));
            }
            iLastContainerPress = now;
            iRightMouseSelectedItem = null;
            iRightMouseSelectedIndex = 0;
            if (!pnlContainerInfo.IsFocused)
            {
                pnlContainerInfo.Focus();
            }
            e.Handled = true;
        }

        void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && iCurrentRenamingItem == null)
            {
                BrowserItem item = lstBrowser.GetEventSourceItem<BrowserItem, ListViewItem>(e);
                if (item != null)
                {
                    List<upnpObject> items = new List<upnpObject>();
                    items.Add(item.WrappedItem);
                    {
                        Activate(items, lstBrowser.Items.IndexOf(item));
                    }
                }
            }
        }

        private void Activate(List<upnpObject> aItems, int aIndex)
        {
            foreach (upnpObject obj in aItems)
            {
                if (obj is container)
                {
                    string key = iNavigationController.Location.ToString();
                    if (iScrollIndexCache.ContainsKey(key))
                    {
                        iScrollIndexCache[key] = aIndex;
                    }
                    else
                    {
                        iScrollIndexCache.Add(key, aIndex);
                    }
                    ScrollViewer sv = lstBrowser.FindVisualChild<ScrollViewer>();
                    if (sv != null)
                    {
                        sv.ScrollToTop();
                    }
                    lstBrowser.ItemsSource = null;
                    iNavigationController.Down(obj as container);
                    return;
                }
            }
            iSupport.PlayNow(new MediaRetrieverNoRetrieve(aItems));
        }

        void ListView_PreviewRightMouseButtonDown(object sender, MouseButtonEventArgs args)
        {
            BrowserItem viewModel = lstBrowser.GetEventSourceItem<BrowserItem, ListViewItem>(args);
            if (viewModel != null)
            {
                iRightMouseSelectedItem = viewModel.WrappedItem;
                iRightMouseSelectedIndex = lstBrowser.Items.IndexOf(viewModel);
            }
            else
            {
                iRightMouseSelectedItem = null;
                iRightMouseSelectedIndex = 0;
            }
        }

        void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem[] items = lstBrowser.FindVisualChildren<ListViewItem>();
            foreach (ListViewItem item in items)
            {
                TextBlock[] textblocks = item.FindVisualChildren<TextBlock>();
                foreach (TextBlock t in textblocks)
                {
                    t.ClearValue(ForegroundProperty);
                }
            }
            ContainerInfoSelected = lstBrowser.SelectedItems.Count == lstBrowser.Items.Count;
            if (iCurrentRenamingItem != null)
            {
                if (e.AddedItems.Count > 0)
                {
                    iCurrentRenamingItem.IsEditing = false;
                    iCurrentRenamingItem = null;
                }
            }
            e.Handled = true;
        }

        void iDragHelper_EventDragInitiated(object sender, MouseEventArgs args)
        {
            if (iCurrentRenamingItem == null && !(args.OriginalSource is Thumb))
            {
                DidlLite didl = SelectedUpnpObjects();

                if (didl.Count > 0)
                {
                    DragDropEffects dragDropEffects = DragDropEffects.Copy;
                    if (iContainer.HandleDelete(didl))
                    {
                        dragDropEffects |= DragDropEffects.Move;
                    }
                    MediaProviderDraggable draggable = new MediaProviderDraggable(new MediaRetriever(iContainer, didl), this);

                    DataObject data = new DataObject();
                    data.SetData(draggable);
                    iSupport.SetDragging(true);

                    UIElement source;
                    upnpObject dragItem;
                    if (sender == pnlContainerInfo)
                    {
                        source = pnlContainerInfo;
                        dragItem = iContainer.Metadata;
                    }
                    else
                    {
                        source = lstBrowser.GetEventSourceElement<ListViewItem>(args);
                        BrowserItem viewModel = lstBrowser.GetEventSourceItem<BrowserItem, ListViewItem>(args);
                        if (viewModel != null)
                        {
                            dragItem = viewModel.WrappedItem;
                        }
                        else
                        {
                            dragItem = null;
                        }
                    }

                    DragDropEffects result = iDragHelper.DoDragDrop(source, data, dragDropEffects, GetDragVisual(dragItem));

                    if (result == DragDropEffects.Move)
                    {
                        foreach (upnpObject o in didl)
                        {
                            iContainer.Delete(o.Id);
                        }
                    }
                    iSupport.SetDragging(false);

                }
            }

        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                SetEffects(e);

                ListViewItem item = lstBrowser.GetEventSourceElement<ListViewItem>(e);

                if (item != null && e.Effects != DragDropEffects.None)
                {
                    iDragFeedbackAdorner = new DragAdorner(item, false);
                    AdornerLayer.GetAdornerLayer(item).Add(iDragFeedbackAdorner);
                }
                iSupport.SetDragging(true);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in Browser.DragEnter: " + ex);
            }
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                SetEffects(e);
                iSupport.SetDragging(true);
                e.Handled = true;

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in Browser.DragOver: " + ex);
            }
        }
        private void ListView_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                SetEffects(e);
                ListViewItem item = lstBrowser.GetEventSourceElement<ListViewItem>(e);
                if (item != null && iDragFeedbackAdorner != null)
                {
                    AdornerLayer.GetAdornerLayer(item).Remove(iDragFeedbackAdorner);
                }
                iSupport.SetDragging(false);
                e.Handled = true;

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in Browser.DragLeave: " + ex);
            }
        }

        private void SetEffects(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy ||
                (e.AllowedEffects & DragDropEffects.Link) == DragDropEffects.Link ||
                (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                MediaProviderDraggable r = iDropConverter.Convert(e.Data);
                if (r != null)
                {
                    if (iContainer.HandleInsert(r.DragMedia))
                    {
                        if (((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy) && (r.DragSource != this))
                        {
                            e.Effects = DragDropEffects.Copy;
                        }
                        else if ((e.AllowedEffects & DragDropEffects.Link) == DragDropEffects.Link)
                        {
                            e.Effects = DragDropEffects.Link;
                        }
                        else if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
                        {
                            e.Effects = DragDropEffects.Move;
                        }
                    }
                }
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                SetEffects(e);
                MediaProviderDraggable r = iDropConverter.Convert(e.Data);
                if (r != null)
                {
                    BrowserItem viewModel = lstBrowser.GetEventSourceItem<BrowserItem, ListViewItem>(e);
                    string id = string.Empty;
                    if (viewModel != null)
                    {
                        upnpObject item = viewModel.WrappedItem;
                        if (item != null)
                        {
                            id = item.Id;
                        }
                    }
                    if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy ||
                        (e.Effects & DragDropEffects.Link) == DragDropEffects.Link ||
                        (e.Effects & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        iContainer.Insert(id, r.DragMedia);
                    }
                }
                iSupport.SetDragging(false);

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in Browser.DragDrop: " + ex);
            }
        }


        void aViewButton_EventClick(object sender, RoutedEventArgs e)
        {
            OnViewClick();
        }

        public void ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = e.OriginalSource as Image;
            ListViewItem item = img.FindVisualAncestor<ListViewItem>();
            BrowserItem browserItem = (lstBrowser.ItemContainerGenerator.ItemFromContainer(item) as BrowserItem);
            if (browserItem != null)
            {
                UserLog.WriteLine(string.Format("{0}:{1}", e.ErrorException.Message, browserItem.ImageSource));
                browserItem.ImageSource = KinskyDesktopWpf.StaticImages.ImageSourceIconAlbumError;
                img.SetBinding(Image.SourceProperty, "ImageSource");
            }
            e.Handled = true;
        }

        public void Open()
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                Assert.Check(!iOpen);
                iOpen = true;
                //OnLocationChanged();
                iSizeButton.ClearValue(Control.IsEnabledProperty);
                iViewButton.ClearValue(Control.IsEnabledProperty);
            });
        }

        public void Close()
        {
            Dispatcher.BeginInvoke((Action)delegate()
            {
                if (iOpen)
                {
                    iSizeButton.IsEnabled = false;
                    iViewButton.IsEnabled = false;
                }
                iOpen = false;
            });
        }
        public void SetSize()
        {
            double size = iViewIndex == kThumbsView ? iUIOptions.ContainerViewSizeThumbsView : iUIOptions.ContainerViewSizeListView;

            ItemSize = size;
            Resources["BrowserImageHeight"] = size * (2d / 3d);
            Resources["BrowserTileSize"] = size;

            if (iViewIndex == kThumbsView)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    VirtualizingTilePanel panel = lstBrowser.FindVisualChild<VirtualizingTilePanel>();
                    if (panel != null && iSliderStartIndex != -1)
                    {
                        panel.CurrentIndex = iSliderStartIndex;
                    }
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        private void UpdateCacheSize()
        {
            // prevent image size being smaller than the browser image size
            KinskyDesktop.Instance.ImageCache.DownscaleImageSize = Math.Max((int)ItemSize, kMinimumImageSize);
        }

        public void OnViewClick()
        {
            iViewIndex = (uint)((iViewIndex + 1) % iViews.Length);
            iUIOptions.ContainerView = iViewIndex;
            iSliderStartIndex = -1;
            SetView();
            UpdateCacheSize();
        }

        public void SetView()
        {
            double sliderValue = iViewIndex == kThumbsView ? iUIOptions.ContainerViewSizeThumbsView : iUIOptions.ContainerViewSizeListView;
            iSlider.ValueChanged -= aSlider_ValueChanged;
            iSlider.Minimum = iViewIndex == kThumbsView ? kMinItemWidthThumbsView : kMinItemWidthListView;
            iSlider.Maximum = iViewIndex == kThumbsView ? kMaxItemWidthThumbsView : kMaxItemWidthListView;
            iSlider.Value = sliderValue;
            iSlider.ValueChanged += aSlider_ValueChanged;
            SetSize();
            iViewIndex = iUIOptions.ContainerView;
            iViewButton.IsChecked = iViewIndex != kThumbsView;

            if (iIsAlbum)
            {
                lstBrowser.Style = null;
                lstBrowser.ItemContainerStyle = FindResource("BrowserListItemContainerStyle") as Style;
                lstBrowser.View = null;
                lstBrowser.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
                lstBrowser.ItemTemplate = FindResource("AlbumViewItem") as DataTemplate;
            }
            else if (iViewIndex % iViews.Length == kListView)
            {
                lstBrowser.Style = null;
                lstBrowser.ItemContainerStyle = FindResource("BrowserListItemContainerStyle") as Style;
                lstBrowser.View = null;
                lstBrowser.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
                lstBrowser.ClearValue(ListView.ItemTemplateProperty);
            }
            else
            {
                lstBrowser.Style = Application.Current.FindResource(typeof(TileView)) as Style;
                lstBrowser.ItemContainerStyle = null;
                lstBrowser.View = FindResource("BrowserTileView") as ViewBase;

                // msbuild throws a wobbly if we try to instantiate this via xaml style...
                FrameworkElementFactory childFactory = new FrameworkElementFactory(typeof(VirtualizingTilePanel));
                Binding b = new Binding();
                b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ViewWidgetBrowser), 1);
                b.Path = new PropertyPath("ItemSize");
                childFactory.SetBinding(VirtualizingTilePanel.ItemSizeProperty, b);

                lstBrowser.ItemsPanel = new ItemsPanelTemplate(childFactory);
                lstBrowser.ClearValue(ListView.ItemTemplateProperty);

            }
            SetPanelState();
        }

        private void SetPanelState()
        {
            switch (iNavigationState)
            {
                case ENavigationState.Failed:
                    {
                        pnlBrowser.Visibility = Visibility.Visible;
                        pnlError.Visibility = Visibility.Visible;
                        lstBrowser.Visibility = Visibility.Collapsed;
                        pnlContainerInfo.Visibility = Visibility.Collapsed;
                        pnlLoading.Visibility = Visibility.Collapsed;
                        pnlProgress.IsAnimating = false;
                        break;
                    }
                case ENavigationState.Navigating:
                    {
                        pnlBrowser.Visibility = Visibility.Collapsed;
                        pnlLoading.Visibility = Visibility.Visible;
                        pnlContainerInfo.Visibility = Visibility.Collapsed;
                        pnlProgress.IsAnimating = true;
                        break;
                    }
                case ENavigationState.Navigated:
                    {
                        pnlBrowser.Visibility = Visibility.Visible;
                        lstBrowser.Visibility = Visibility.Visible;
                        pnlError.Visibility = Visibility.Collapsed;
                        pnlLoading.Visibility = Visibility.Collapsed;
                        pnlProgress.IsAnimating = false;


                        if (iIsAlbum)
                        {
                            pnlContainerInfo.Visibility = Visibility.Visible;
                            iSizeButton.IsEnabled = false;
                            iViewButton.IsEnabled = false;
                        }
                        else
                        {
                            pnlContainerInfo.Visibility = Visibility.Collapsed;
                            iSizeButton.IsEnabled = true;
                            iViewButton.IsEnabled = true;
                        }


                        break;
                    }
                default:
                    {
                        Assert.Check(false);
                        break;
                    }
            }
        }


        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(ViewWidgetBrowser), new UIPropertyMetadata(0d));



        private FrameworkElement GetDragVisual(upnpObject dragItem)
        {
            if (dragItem != null)
            {
                Image img = new Image();
                double containerSize = iViewIndex == kThumbsView ? iUIOptions.ContainerViewSizeThumbsView : iUIOptions.ContainerViewSizeListView;

                img.Height = containerSize * (2 / 3);
                img.SetValue(Image.SourceProperty, StaticImages.ImageSourceIconLoading);
                WpfImageCache loader = KinskyDesktop.Instance.ImageCache;
                IconResolver resolver = KinskyDesktop.Instance.IconResolver;
                loader.Load(resolver.Resolve(dragItem), (s) =>
                {
                    this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        img.SetValue(Image.SourceProperty, s);
                    }));
                });
                return img;
            }
            return null;
        }

        private DidlLite SelectedUpnpObjects()
        {
            DidlLite items = new DidlLite();
            Dictionary<BrowserItem, BrowserItem> itemDict = new Dictionary<BrowserItem, BrowserItem>();
            foreach (BrowserItem i in lstBrowser.SelectedItems)
            {
                itemDict.Add(i, i);
            }
            foreach (BrowserItem i in lstBrowser.Items)
            {
                if (itemDict.ContainsKey(i))
                {
                    items.Add(i.WrappedItem);
                }
            }
            return items;
        }

        #region Command Bindings

        private void OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DidlLite didl = new DidlLite();

            container selected = iRightMouseSelectedItem as container;
            e.CanExecute = iOpen && selected != null;
            e.Handled = true;
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            container selected = iRightMouseSelectedItem as container;
            if (selected != null)
            {
                List<upnpObject> list = new List<upnpObject>();
                list.Add(selected);
                Activate(list, iRightMouseSelectedIndex);
            }
        }

        private void PlayNowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DidlLite didl = SelectedUpnpObjects();

            e.CanExecute = iOpen && didl.Count > 0;
            e.Handled = true;
        }

        private void PlayNowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            iSupport.PlayNow(new MediaRetriever(iContainer, SelectedUpnpObjects()));
        }

        private void PlayNextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DidlLite didl = SelectedUpnpObjects();

            e.CanExecute = iOpen && didl.Count > 0;
            e.Handled = true;
        }

        private void PlayNextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            iSupport.PlayNext(new MediaRetriever(iContainer, SelectedUpnpObjects()));
        }

        private void PlayLaterCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DidlLite didl = SelectedUpnpObjects();

            e.CanExecute = iOpen && didl.Count > 0;
            e.Handled = true;
        }

        private void PlayLaterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            iSupport.PlayLater(new MediaRetriever(iContainer, SelectedUpnpObjects()));
        }

        private void DetailsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            upnpObject selected = iRightMouseSelectedItem as upnpObject;
            e.CanExecute = iOpen && selected != null;
            e.Handled = true;
        }

        private void DetailsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            upnpObject selected = iRightMouseSelectedItem as upnpObject;
            DetailsDialog details = new DetailsDialog(selected, iContainer.Metadata, iUIOptions);
            details.Owner = Window.GetWindow(this);
            details.ShowDialog();
        }

        private void BookmarkCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = iRightMouseSelectedItem != null && iRightMouseSelectedItem is container;
            e.Handled = true;
        }

        private void BookmarkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (iRightMouseSelectedItem != null && iRightMouseSelectedItem is container)
            {
                Linn.Kinsky.IContainer container = iContainer.ChildContainer(iRightMouseSelectedItem as container);
                if (container != null)
                {
                    Location newLocation = new Location(iNavigationController.Location, container);
                    iViewWidgetBookmarks.ShowAddBookmark(new Bookmark(newLocation));
                }
            }
        }

        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanDelete();
            e.Handled = true;
        }

        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Delete();
        }

        private void RenameCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lstBrowser.SelectedItems.Count == 1)
            {
                e.CanExecute = iContainer.HandleRename((lstBrowser.SelectedItem as BrowserItem).WrappedItem);
            }
            else
            {
                e.CanExecute = false;
            }
            e.Handled = true;
        }

        private void RenameExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (lstBrowser.SelectedItems.Count == 1)
            {
                if (iCurrentRenamingItem != null)
                {
                    iCurrentRenamingItem.IsEditing = false;
                }
                iCurrentRenamingItem = (lstBrowser.SelectedItem as BrowserItem);
                iCurrentRenamingItem.IsEditing = true;
                // need to invoke this with background priority to ensure it gets called 
                // after the data trigger which adds the textbox to the visual tree has been invoked                
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    DependencyObject listItem = lstBrowser.ItemContainerGenerator.ContainerFromItem(iCurrentRenamingItem);
                    if (listItem != null)
                    {
                        TextBox txtTitleEditor = listItem.FindVisualChild<TextBox>();
                        // paranoia: ensure the textbox is definitely created to prevent an app crash
                        if (txtTitleEditor != null)
                        {
                            txtTitleEditor.Focus();
                            txtTitleEditor.SelectAll();
                        }
                    }
                }), DispatcherPriority.Background);
                lstBrowser.SelectedItems.Clear();
            }
        }
        #endregion


        private void lstBrowser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (iCurrentRenamingItem == null)
            {
                DateTime now = DateTime.Now;
                TimeSpan ts = now - iLastKeyDown;
                if (ts > TimeSpan.FromSeconds(kSearchTimeoutSeconds))
                {
                    iSearchText = string.Empty;
                }
                if (iData != null && iContainer != null)
                {
                    iSearchText += e.Text;
                    Search(iSearchText);
                    e.Handled = true;
                }
                iLastKeyDown = now;
            }
        }

        private void Search(string aSearchText)
        {
            StopSearch();
            if (aSearchText.Length == 0)
            {
                if (iData.Count > 0)
                {
                    lstBrowser.EnsureSelected(0);
                }
            }
            else
            {

                int index = iData.Find((b) =>
                {
                    return b.WrappedItem != null && b.WrappedItem.Title != null && b.WrappedItem.Title.ToUpperInvariant().StartsWith(aSearchText.ToUpperInvariant());
                });
                if (index > 0)
                {
                    lstBrowser.EnsureSelected(index);
                }
                else
                {
                    Assert.Check(iContainer != null);
                    iSearcher = new Searcher(aSearchText, iContentCollector, lstBrowser, iData);
                }
            }

        }

        private void StopSearch()
        {
            if (iSearcher != null)
            {
                iSearcher.Dispose();
                iSearcher = null;
            }
        }

        private class Searcher : IDisposable
        {

            public Searcher(string aSearchText, IContentCollector<upnpObject> aContentCollector, ListView aListView, BrowserList aData)
            {
                Assert.Check(aContentCollector != null);
                Assert.Check(aListView != null);
                Assert.Check(aData != null);
                Assert.Check(aSearchText != null);
                iSearchText = aSearchText.ToUpperInvariant();
                iLockObject = new object();
                iRetry = false;
                iDisposed = false;
                iListView = aListView;
                iData = aData;
                iContentCollector = aContentCollector;
                iContentCollector.EventOpened += iContentCollector_EventOpened;
                iContentCollector.EventItemsFailed += iContentCollector_EventItemsFailed;
                iContentCollector.EventItemsLoaded += iContentCollector_EventItemsLoaded;
            }

            void iContentCollector_EventItemsLoaded(object sender, EventArgsItemsLoaded<upnpObject> e)
            {
                for (int i = 0; i < e.Items.Count; i++)
                {
                    int index = e.StartIndex + i;
                    if (index == iCurrentPosition)
                    {
                        Item(index, e.Items[i]);
                        break;
                    }
                }
            }

            void iContentCollector_EventItemsFailed(object sender, EventArgsItemsFailed e)
            {
                UserLog.WriteLine("SearchContentHandler:ItemsFailed: " + e.Exception.Message);
                Dispose();
            }

            void iContentCollector_EventOpened(object sender, EventArgs e)
            {
                lock (iLockObject)
                {
                    if (!iDisposed)
                    {
                        iCount = iContentCollector.Count;
                        iLowerBound = 0;
                        iUpperBound = iCount;
                        if (iCount > 0)
                        {
                            if (!iSearching && !iDisposed)
                            {
                                StartSearch();
                            }
                        }
                        else
                        {
                            Dispose();
                        }
                    }
                }
            }

            private void StartSearch()
            {
                iSearching = true;
                iCurrentPosition = iUpperBound / 2;
                iContentCollector.Item(iCurrentPosition, ERequestPriority.Foreground);
            }

            private void Item(int aIndex, upnpObject aObject)
            {
                lock (iLockObject)
                {
                    if (iSearching && !iDisposed)
                    {
                        string title = aObject.Title.ToUpperInvariant();
                        if (iRetry)
                        {
                            title = iRetryRegex.Replace(title, "");
                        }
                        if (title.StartsWith(iSearchText))
                        {

                            iListView.Dispatcher.BeginInvoke((Action)delegate()
                            {
                                lock (iLockObject)
                                {
                                    if (iSearching && !iDisposed)
                                    {
                                        BrowserItem item = iData[(int)aIndex];
                                        if (item is PlaceholderBrowserItem)
                                        {
                                            iData[(int)aIndex] = iData.CreateViewModel(aObject);
                                        }
                                        iListView.EnsureSelected((int)aIndex);
                                    }
                                    Dispose();
                                }
                            });
                        }
                        else
                        {
                            if (aIndex == iCurrentPosition)
                            {
                                Trace.WriteLine(Trace.kKinskyDesktop, "Search... aIndex:" + aIndex + ", iCurrentPosition: " + iCurrentPosition + ", iLowerBound: " + iLowerBound + ", iUpperBound: " + iUpperBound + ", title: " + title + ", iSearchText: " + iSearchText);
                                if (title.CompareTo(iSearchText) < 0)
                                {
                                    iLowerBound = iCurrentPosition + 1;
                                }
                                else
                                {
                                    iUpperBound = iCurrentPosition - 1;
                                }
                                iCurrentPosition = iLowerBound + ((iUpperBound - iLowerBound) / 2);
                                if (iLowerBound <= iUpperBound && iCurrentPosition >= 0 && iCurrentPosition < iData.Count)
                                {
                                    iContentCollector.Item(iCurrentPosition, ERequestPriority.Foreground);
                                }
                                else
                                {
                                    if (!iRetry)
                                    {
                                        iRetry = true;
                                        iRetryRegex = new Regex("^THE ");
                                        iLowerBound = 0;
                                        iUpperBound = iCount;
                                        StartSearch();
                                    }
                                    else
                                    {
                                        Dispose();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #region IDisposable Members

            public void Dispose()
            {
                if (!iDisposed)
                {
                    iSearching = false;
                    iContentCollector.EventOpened -= iContentCollector_EventOpened;
                    iContentCollector.EventItemsFailed -= iContentCollector_EventItemsFailed;
                    iContentCollector.EventItemsLoaded -= iContentCollector_EventItemsLoaded;
                    iDisposed = true;
                }
            }

            #endregion

            private IContentCollector<upnpObject> iContentCollector;
            private ListView iListView;
            private BrowserList iData;
            private bool iDisposed;
            private bool iSearching;
            private int iLowerBound;
            private int iUpperBound;
            private int iCurrentPosition;
            private int iCount;
            private object iLockObject;
            private bool iRetry;
            private Regex iRetryRegex;
            private string iSearchText;
        }

        private void txtTitleEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            HideTitleEditor(sender as TextBox);
        }

        private void txtTitleEditor_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void lstBrowser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.XButton1 == MouseButtonState.Pressed)
            {
                iNavigationController.Up(1);
                e.Handled = true;
                return;
            }
            if (iCurrentRenamingItem != null && iCurrentRenamingItem.IsEditing)
            {
                TextBox titleEditor = lstBrowser.ItemContainerGenerator.ContainerFromItem(iCurrentRenamingItem).FindVisualChild<TextBox>();
                HideTitleEditor(titleEditor);
            }
        }

        private void txtTitleEditor_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TextBox titleEditor = (sender as TextBox);
            if (e.Key == Key.Enter)
            {
                HideTitleEditor(titleEditor);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                //reset text
                titleEditor.Text = DidlLiteAdapter.Title(iCurrentRenamingItem.WrappedItem);
                HideTitleEditor(titleEditor);
                e.Handled = true;
            }
        }

        private void HideTitleEditor(TextBox aTitleEditor)
        {
            if (iCurrentRenamingItem != null)
            {
                upnpObject o = iCurrentRenamingItem.WrappedItem;
                string newTitle = aTitleEditor.Text.Trim('\n');
                if (DidlLiteAdapter.Title(o) != newTitle)
                {
                    Linn.Kinsky.IContainer container = iContainer;
                    container.Rename(o.Id, newTitle);
                }
                iCurrentRenamingItem.IsEditing = false;
                iCurrentRenamingItem = null;
            }
        }

        private bool CanDelete()
        {
            DidlLite didl = new DidlLite();
            didl.AddRange(SelectedUpnpObjects());
            if (iContainer != null)
            {
                return iContainer.HandleDelete(didl);
            }
            else
            {
                return false;
            }
        }

        private void Delete()
        {
            IList<upnpObject> list = SelectedUpnpObjects();
            Linn.Kinsky.IContainer container = iContainer;
            if (iContainer != null)
            {
                foreach (upnpObject o in list)
                {
                    container.Delete(o.Id);
                }
            }
        }

        private void lstBrowser_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && CanDelete())
            {
                Delete();
            }
            if (e.Key == Key.Back && iCurrentRenamingItem == null)
            {
                iNavigationController.Up(1);
            }
        }

        private void lstBrowser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back && iSearchText.Length > 0)
            {
                iSearchText = iSearchText.Substring(0, iSearchText.Length - 1);
                iLastKeyDown = DateTime.Now;
                Search(iSearchText);
                e.Handled = true;
            }
            if (e.Key == Key.Enter && iCurrentRenamingItem == null)
            {
                List<upnpObject> selected = SelectedUpnpObjects();
                Activate(selected, lstBrowser.SelectedIndex);
                e.Handled = true;
            }
        }

        private void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            if (iNavigationController.NavigationState == ENavigationState.Failed || iNavigationState == ENavigationState.Failed || iNavigationController.Location == null)
            {
                iNavigationController.Retry();
            }
            else
            {
                OnLocationChanged();
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            iNavigationController.Navigate(iNavigationController.Home);
        }

        private ENavigationState iNavigationState;
        private BrowserList iData;
        private Linn.Kinsky.IContainer iContainer;
        private bool iOpen;
        private IContentCollector<upnpObject> iContentCollector;
        private const int kRangeSize = 50;
        private const int kThreadCount = 4;

        private IPlaylistSupport iSupport;
        private DropConverter iDropConverter;
        private ToggleButton iSizeButton;
        private ToggleButton iViewButton;
        private uint iViewIndex = 0;
        private bool iIsAlbum = false;
        private string iErrorMessage = string.Empty;
        private Dictionary<string, int> iScrollIndexCache;

        private const double kMinItemWidthListView = 50;
        private const double kMaxItemWidthListView = 200;
        private const double kDefaultItemWidthListView = 100;
        private const double kMinItemWidthThumbsView = 100;
        private const double kMaxItemWidthThumbsView = 250;
        private const double kDefaultItemWidthThumbsView = 150;
        private const int kSearchTimeoutSeconds = 2;

        private const int kThumbsView = 0;
        private const int kListView = 1;
        private int[] iViews = { kThumbsView, kListView };

        private DragHelper iDragHelper;
        private DragHelper iDragHelperContainer;

        private upnpObject iRightMouseSelectedItem;
        private int iRightMouseSelectedIndex;
        private DragAdorner iDragFeedbackAdorner;
        private UiOptions iUIOptions;
        private DateTime iLastContainerPress;
        private Searcher iSearcher;
        private BrowserItem iCurrentRenamingItem;
        private string iSearchText = string.Empty;
        private DateTime iLastKeyDown = DateTime.MinValue;
        private int iLastSelectedIndex;
        private Slider iSlider;
        private NavigationController iNavigationController;
        private int iSliderStartIndex = -1;
        private ViewWidgetBookmarks iViewWidgetBookmarks;
        private const int kMinimumImageSize = 60;
    }


    public class BrowserList : LazyLoadingList<BrowserItem, upnpObject>
    {
        public BrowserList(IContentCollector<upnpObject> aContentCollector, Dispatcher aDispatcher, Linn.Kinsky.IContainer aContainer)
            : base(aContentCollector, aDispatcher)
        {
            iContainer = aContainer;
        }

        public override BrowserItem CreateViewModel(upnpObject aItem)
        {
            if (aItem == null)
            {
                return new PlaceholderBrowserItem();
            }
            return new BrowserItem(aItem, iContainer.Metadata);
        }
        private Linn.Kinsky.IContainer iContainer;
    }
}
