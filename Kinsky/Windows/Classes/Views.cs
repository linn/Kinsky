using System.Windows.Controls;
using System.Windows;
using Linn.Kinsky;
using System.Collections.Generic;
using System.Threading;
using System;
using Linn;
using System.Collections.ObjectModel;
using Linn.Topology;
using Upnp;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Text;
using System.Xml;

namespace KinskyDesktopWpf
{


    class MediatorHouse
    {
        private enum EHouseState
        {
            SelectingRoom,
            SelectingSource,
            ViewingPlaylist
        }

        private ViewWidgetSelectorRoom iViewWidgetSelectorRoom;
        private ViewWidgetSelectorSource iViewWidgetSelectorSource;
        private FrameworkElement iPlaylistWidget;
        private ListView iRoomSelectionList;
        private ListView iSourceSelectionList;
        private ToggleButton iButtonSelectRoom;
        private ToggleButton iButtonSelectSource;

        private bool iForceSelect = false;
        private Linn.Kinsky.Room iLastUserRoomSelect = null;
        private Linn.Kinsky.Source iLastUserSourceSelect = null;

        private object iLockObject;
        private EHouseState iCurrentHouseState;
        private Popup iPopupRoomSelection;
        private Popup iPopupSourceSelection;

        public MediatorHouse(ViewWidgetSelectorRoom aViewWidgetSelectorRoom,
                            ViewWidgetSelectorSource aViewWidgetSelectorSource,
                            FrameworkElement aPlaylistWidget,
                            ListView aRoomSelectionList,
                            ListView aSourceSelectionList,
                            ToggleButton aButtonSelectRoom,
                            ToggleButton aButtonSelectSource,
                            Popup aPopupRoomSelection,
                            Popup aPopupSourceSelection,
                            Button aStandbyAllButton)
        {
            iLockObject = new object();
            aStandbyAllButton.Click += StandbyAllButton_Click;
            iPopupRoomSelection = aPopupRoomSelection;
            iPopupSourceSelection = aPopupSourceSelection;
            iViewWidgetSelectorRoom = aViewWidgetSelectorRoom;
            iViewWidgetSelectorSource = aViewWidgetSelectorSource;
            iPlaylistWidget = aPlaylistWidget;
            iRoomSelectionList = aRoomSelectionList;
            iSourceSelectionList = aSourceSelectionList;
            iButtonSelectRoom = aButtonSelectRoom;
            iButtonSelectSource = aButtonSelectSource;

            iButtonSelectRoom.Click += new RoutedEventHandler(iButtonSelectRoom_Click);
            iButtonSelectSource.Click += new RoutedEventHandler(iButtonSelectSource_Click);

            iPopupRoomSelection.Closed += new EventHandler(iPopupRoomSelection_Closed);
            iPopupSourceSelection.Closed += new EventHandler(iPopupSourceSelection_Closed);

            iViewWidgetSelectorRoom.EventNotifyConsumersSelectionChanged += iViewWidgetSelectorRoom_EventNotifyConsumersSelectionChanged;
            iViewWidgetSelectorRoom.EventUserSelectedItem += iViewWidgetSelectorRoom_EventUserSelectedItem;

            iViewWidgetSelectorSource.EventNotifyConsumersSelectionChanged += iViewWidgetSelectorSource_EventNotifyConsumersSelectionChanged;
            iViewWidgetSelectorSource.EventUserSelectedItem += iViewWidgetSelectorSource_EventUserSelectedItem;
            iCurrentHouseState = EHouseState.ViewingPlaylist;
            iPopupRoomSelection.KeyDown += new KeyEventHandler(iPopupRoomSelection_KeyDown);
            iPopupSourceSelection.KeyDown += new KeyEventHandler(iPopupSourceSelection_KeyDown);
        }

        private void StandbyAllButton_Click(object sender, RoutedEventArgs e)
        {
            SetState(EHouseState.ViewingPlaylist);
        }

        void iPopupRoomSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Escape)
            {
                iPopupRoomSelection.IsOpen = false;
            }
        }

        void iPopupSourceSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Escape)
            {
                iPopupSourceSelection.IsOpen = false;
            }
        }

        void iPopupSourceSelection_Closed(object sender, EventArgs e)
        {
            if (!iButtonSelectSource.IsPressed)
            {
                SetState(EHouseState.ViewingPlaylist);
            }
        }

        void iPopupRoomSelection_Closed(object sender, EventArgs e)
        {
            if (!iButtonSelectRoom.IsPressed)
            {
                SetState(EHouseState.ViewingPlaylist);
            }
        }

        void iButtonSelectSource_Click(object sender, RoutedEventArgs e)
        {
            lock (iLockObject)
            {
                ToggleButton btn = sender as ToggleButton;
                if (btn.IsChecked.HasValue && btn.IsChecked.Value)
                {
                    if (iCurrentHouseState != EHouseState.SelectingSource)
                    {
                        SetState(EHouseState.SelectingSource);
                    }
                }
                else
                {
                    if (iCurrentHouseState == EHouseState.SelectingSource)
                    {
                        SetState(EHouseState.ViewingPlaylist);
                    }
                }
            }
        }

        void iButtonSelectRoom_Click(object sender, RoutedEventArgs e)
        {
            lock (iLockObject)
            {
                ToggleButton btn = sender as ToggleButton;
                if (btn.IsChecked.HasValue && btn.IsChecked.Value)
                {
                    if (iCurrentHouseState != EHouseState.SelectingRoom)
                    {
                        SetState(EHouseState.SelectingRoom);
                    }
                }
                else
                {
                    if (iCurrentHouseState == EHouseState.SelectingRoom)
                    {
                        SetState(EHouseState.ViewingPlaylist);
                    }
                }
            }
        }

        void iViewWidgetSelectorSource_EventUserSelectedItem(object sender, EventArgsSelection<Linn.Kinsky.Source> e)
        {
            iLastUserSourceSelect = e.Tag;
            if (e.Tag != null)
            {
                iForceSelect = true;
                SetBusyCursor();
            }
        }

        void iViewWidgetSelectorRoom_EventUserSelectedItem(object sender, EventArgsSelection<Linn.Kinsky.Room> e)
        {
            iLastUserRoomSelect = e.Tag;
            if (e.Tag != null)
            {
                iForceSelect = true;
                SetBusyCursor();
            }
        }

        void iViewWidgetSelectorSource_EventNotifyConsumersSelectionChanged(object sender, EventArgsSelection<Linn.Kinsky.Source> e)
        {
            lock (iLockObject)
            {
                if (iForceSelect && iCurrentHouseState != EHouseState.ViewingPlaylist)
                {
                    SetState(EHouseState.ViewingPlaylist);
                }
                iForceSelect = false;
                ClearBusyCursor();
            }
        }

        void iViewWidgetSelectorRoom_EventNotifyConsumersSelectionChanged(object sender, EventArgsSelection<Linn.Kinsky.Room> e)
        {
            lock (iLockObject)
            {

                if (iForceSelect && iCurrentHouseState != EHouseState.ViewingPlaylist)
                {
                    SetState(EHouseState.ViewingPlaylist);
                }
                iForceSelect = false;
                ClearBusyCursor();
            }
        }

        private void SetBusyCursor()
        {
            iRoomSelectionList.Cursor = Cursors.Wait;
            iSourceSelectionList.Cursor = Cursors.Wait;
            // ensure we clear busy cursor after a timeout to prevent hung DS source switching from making UI appear unresponsive
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromMilliseconds(1000);
            t.Tick += ((d, a) =>
            {
                ClearBusyCursor();
                t.Stop();
            });
            t.Start();
        }

        private void ClearBusyCursor()
        {
            iRoomSelectionList.ClearValue(FrameworkElement.CursorProperty);
            iSourceSelectionList.ClearValue(FrameworkElement.CursorProperty);
        }

        void SetState(EHouseState aState)
        {
            lock (iLockObject)
            {
                if (iCurrentHouseState != aState)
                {
                    EHouseState previousState = iCurrentHouseState;
                    iCurrentHouseState = aState;
                    if (aState == EHouseState.SelectingRoom)
                    {
                        iPopupRoomSelection.SetPopupOffset(iButtonSelectRoom);
                        iPopupRoomSelection.IsOpen = true;
                        iRoomSelectionList.Focus();
                        iViewWidgetSelectorRoom.ScrollToSelectedItem();
                    }
                    else
                    {
                        iPopupRoomSelection.IsOpen = false;
                    }
                    if (aState == EHouseState.SelectingSource)
                    {
                        iPopupSourceSelection.SetPopupOffset(iButtonSelectSource);
                        iPopupSourceSelection.IsOpen = true;
                        iSourceSelectionList.Focus();
                        iViewWidgetSelectorSource.ScrollToSelectedItem();
                    }
                    else
                    {
                        iPopupSourceSelection.IsOpen = false;
                    }

                    iButtonSelectRoom.IsChecked = aState == EHouseState.SelectingRoom;
                    iButtonSelectSource.IsChecked = aState == EHouseState.SelectingSource;
                }
            }
        }

    }

    public class ViewWidgetBookmarks
    {
        public ViewWidgetBookmarks(HelperKinsky aHelperKinsky,
                                   BookmarkManager aBookmarkManager,
                                   ListView aBookmarkList,
                                   IBrowser aBrowser,
                                   Popup aBookmarksListPopup,
                                   Popup aAddBookmarkPopup,
                                   ToggleButton aShowBookmarksListButton,
                                   ToggleButton aShowAddBookmarkButton,
                                   Button aAddBookmarkButton,
                                   Button aCancelAddBookmarkButton,
                                   TextBox aTitleEditor,
                                   TextBlock aBreadcrumbDisplay,
                                   Panel aAddBookmarkPanel,
                                   NavigationController aNavigationController)
        {
            iLock = new object();
            iNavigationController = aNavigationController;
            iOpen = false;
            iHelperKinsky = aHelperKinsky;
            iTitleEditor = aTitleEditor;
            iBreadcrumbDisplay = aBreadcrumbDisplay;
            iAddBookmarkPanel = aAddBookmarkPanel;
            iShowBookmarksListButton = aShowBookmarksListButton;
            iShowAddBookmarkButton = aShowAddBookmarkButton;
            iAddBookmarkButton = aAddBookmarkButton;
            iCancelAddBookmarkButton = aCancelAddBookmarkButton;
            iBookmarksListPopup = aBookmarksListPopup;
            iAddBookmarkPopup = aAddBookmarkPopup;
            iBookmarkList = aBookmarkList;
            iBookmarkCollection = new ObservableCollection<BookmarkViewModel>();
            iBookmarkList.ItemsSource = iBookmarkCollection;

            iBookmarkList.SelectionMode = SelectionMode.Single;
            iBookmarkList.PreviewMouseLeftButtonDown += iBookmarkList_PreviewMouseLeftButtonDown;
            iBookmarkList.PreviewMouseLeftButtonUp += iBookmarkList_PreviewMouseLeftButtonUp;

            iDragHelper = new DragHelper(iBookmarkList);
            iDragHelper.EventDragInitiated += new EventHandler<MouseEventArgs>(iDragHelper_EventDragInitiated);

            DragScroller scroller = new DragScroller(iBookmarkList, delegate(DragEventArgs e)
            {
                if (e.Data.GetDataPresent("bookmarks"))
                {
                    return DragDropEffects.Move;
                }
                return DragDropEffects.None;
            });
            scroller.ItemsDropped += new EventHandler<DragScroller.EventArgsItemsDropped>(Scroller_ItemsDropped);

            iBookmarkManager = aBookmarkManager;

            lock (iBookmarkCollection)
            {
                iBookmarkManager.EventBookmarkAdded += iBookmarkManager_EventBookmarkAdded;
                iBookmarkManager.EventBookmarkRemoved += iBookmarkManager_EventBookmarkRemoved;
                iBookmarkManager.EventBookmarksChanged += iBookmarkManager_EventBookmarksChanged;
                foreach (Bookmark b in iBookmarkManager.Bookmarks)
                {
                    iBookmarkCollection.Add(new BookmarkViewModel(b));
                }
            }

            iShowBookmarksListButton.Click += new RoutedEventHandler(iShowBookmarksListButton_Click);
            iShowAddBookmarkButton.Click += new RoutedEventHandler(iShowAddBookmarkButton_Click);
            iAddBookmarkButton.Click += new RoutedEventHandler(iAddBookmarkButton_Click);
            iCancelAddBookmarkButton.Click += new RoutedEventHandler(iCancelAddBookmarkButton_Click);
            iBookmarkList.CommandBindings.Add(new CommandBinding(Commands.DeleteCommand, new ExecutedRoutedEventHandler((d, e) =>
            {
                ListViewItem container = e.OriginalSource as ListViewItem;
                if (container != null)
                {
                    BookmarkViewModel model = iBookmarkList.ItemContainerGenerator.ItemFromContainer(container) as BookmarkViewModel;
                    iBookmarkManager.Remove(model.WrappedItem);
                }
            })));
            iBookmarkList.CommandBindings.Add(new CommandBinding(Commands.RenameCommand, new ExecutedRoutedEventHandler((d, e) =>
            {
                ListViewItem container = e.OriginalSource as ListViewItem;
                if (container != null)
                {
                    BookmarkViewModel model = iBookmarkList.ItemContainerGenerator.ItemFromContainer(container) as BookmarkViewModel;
                    iCurrentEditingModel = model;
                    iPreviousBookmarkTitle = model.WrappedItem.Title;
                    model.IsEditing = true;
                }
            })));

            iAddBookmarkPopup.Closed += iAddBookmarkPopup_Closed;
            iBookmarksListPopup.Closed += iBookmarksListPopup_Closed;
            iBookmarksListPopup.KeyDown += new KeyEventHandler(iBookmarksListPopup_KeyDown);
            iAddBookmarkPopup.KeyDown += new KeyEventHandler(iAddBookmarkPopup_KeyDown);


            iNavigationController.EventLocationChanged += EventLocationChangedHandler;
            iCurrentBookmark = new Bookmark(iHelperKinsky.LastLocation.BreadcrumbTrail);
        }

        void iBookmarkList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            iCurrentSelectedModel = null;
            BookmarkViewModel item = iBookmarkList.GetEventSourceItem<BookmarkViewModel, ListBoxItem>(e);
            if (iCurrentEditingModel == null)
            {
                // only want to stop propagation of routed event if the original source was not in a button
                // otherwise button click event will not happen on a button that is contained within a selected list item
                Button b = (e.OriginalSource as DependencyObject).FindVisualAncestor<Button>();
                if (b == null)
                {
                    iCurrentSelectedModel = item;
                }
            }

        }

        void iBookmarkList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (iCurrentSelectedModel != null)
            {
                iBookmarksListPopup.IsOpen = false;
                iNavigationController.Navigate(iCurrentSelectedModel.WrappedItem.BreadcrumbTrail);
                e.Handled = true;
            }

        }

        public void CloseEditor()
        {

            iBookmarkList.Dispatcher.BeginInvoke((Action)(() =>
             {
                 if (iCurrentEditingModel != null)
                 {
                     iCurrentEditingModel.IsEditing = false;
                     iCurrentEditingModel = null;
                 }
             }));
        }

        void Scroller_ItemsDropped(object sender, DragScroller.EventArgsItemsDropped e)
        {
            Assert.Check(e.DragEventArgs.Data.GetDataPresent("bookmarks"));
            List<BookmarkViewModel> models = e.DragEventArgs.Data.GetData("bookmarks") as List<BookmarkViewModel>;
            Assert.Check(models != null);
            ListBoxItem item = iBookmarkList.GetEventSourceElement<ListBoxItem>(e.DragEventArgs);
            int dropIndex = 0;
            if (item != null)
            {
                Point p = e.DragEventArgs.GetPosition(item);
                bool top = p.Y < item.ActualHeight / 2;
                BookmarkViewModel containedItem = GetEventSourceItem(e.DragEventArgs);
                int idx = iBookmarkList.Items.IndexOf(containedItem);
                dropIndex = top ? idx : idx + 1;
            }
            else if (iBookmarkList.Items.Count > 0)
            {
                Point p = e.DragEventArgs.GetPosition(iBookmarkList);
                bool top = p.Y < 10;
                dropIndex = top ? 0 : iBookmarkList.Items.Count;
            }
            List<Bookmark> bookmarksList = (from m in models select m.WrappedItem).ToList();
            iBookmarkManager.Move(dropIndex, bookmarksList);
            // prevent deletion
            iDropTargetSelf = true;
        }

        private BookmarkViewModel GetEventSourceItem(RoutedEventArgs args)
        {
            return iBookmarkList.GetEventSourceItem<BookmarkViewModel, ListViewItem>(args);
        }

        void iDragHelper_EventDragInitiated(object sender, MouseEventArgs e)
        {
            BookmarkViewModel sourceItem = GetEventSourceItem(e);
            if (sourceItem != null && !sourceItem.IsEditing && !(e.OriginalSource is Thumb))
            {
                List<BookmarkViewModel> selected = new List<BookmarkViewModel>();
                selected.Add(sourceItem);
                if (selected.Count > 0)
                {
                    DataObject data = new DataObject();
                    data.SetData("bookmarks", selected);
                    iDropTargetSelf = false;

                    DragDropEffects result = iDragHelper.DoDragDrop(iBookmarkList, data, DragDropEffects.Copy | DragDropEffects.Move, GetDragVisual(selected[0]));

                    if (result == DragDropEffects.Move && !iDropTargetSelf)
                    {
                        foreach (BookmarkViewModel item in selected)
                        {
                            iBookmarkManager.Remove(item.WrappedItem);
                        }
                    }
                    iBookmarksListPopup.PopupAnimation = PopupAnimation.None;
                    iBookmarksListPopup.IsOpen = false;
                    iBookmarksListPopup.IsOpen = true;
                    iBookmarksListPopup.PopupAnimation = PopupAnimation.Slide;
                }
            }
        }

        private FrameworkElement GetDragVisual(BookmarkViewModel aBookmark)
        {
            if (aBookmark != null)
            {
                Image img = new Image();
                img.Height = DragHelper.kDefaultVisualHeight;
                img.SetValue(Image.SourceProperty, StaticImages.ImageSourceIconLoading);
                WpfImageCache loader = KinskyDesktop.Instance.ImageCache;
                IconResolver resolver = KinskyDesktop.Instance.IconResolver;
                loader.Load(resolver.Resolve(aBookmark.WrappedItem), (s) =>
                {
                    iBookmarkList.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        img.SetValue(Image.SourceProperty, s);
                    }));
                });
                return img;
            }
            return null;
        }

        public void Open()
        {
            lock (iLock)
            {
                iOpen = true;
            }
        }

        public void Close()
        {
            lock (iLock)
            {
                iOpen = false;
            }
        }

        void EventLocationChangedHandler(object sender, EventArgs e)
        {
            lock (iLock)
            {
                if (iOpen)
                {
                    iCurrentBookmark = new Bookmark(iNavigationController.Location);
                }
            }
        }

        void iAddBookmarkPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                iAddBookmarkPopup.IsOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                AddBookmark();
            }
        }

        void iBookmarksListPopup_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.OriginalSource is TextBox)
            {
                if (e.Key == Key.Tab || e.Key == Key.Enter)
                {
                    iCurrentEditingModel.WrappedItem.Title = (e.OriginalSource as TextBox).Text;
                    iCurrentEditingModel.OnPropertyChanged("WrappedItem");
                    CloseEditor();
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    (e.OriginalSource as TextBox).Text = iPreviousBookmarkTitle;
                    CloseEditor();
                    e.Handled = true;
                }
            }
            else if (e.OriginalSource is ListViewItem)
            {
                if (e.Key == Key.Tab || e.Key == Key.Escape)
                {
                    iBookmarksListPopup.IsOpen = false;
                }
                else if (e.Key == Key.Space || e.Key == Key.Enter)
                {

                    BookmarkViewModel item = iBookmarkList.GetEventSourceItem<BookmarkViewModel, ListBoxItem>(e);
                    if (item != null)
                    {
                        iBookmarksListPopup.IsOpen = false;
                        iNavigationController.Navigate(item.WrappedItem.BreadcrumbTrail);
                        e.Handled = true;
                    }
                }
            }
        }

        void iAddBookmarkPopup_Closed(object sender, EventArgs e)
        {
            if (!iShowAddBookmarkButton.IsPressed)
            {
                iShowAddBookmarkButton.IsChecked = false;
            }
        }

        void iBookmarksListPopup_Closed(object sender, EventArgs e)
        {
            if (!iShowBookmarksListButton.IsPressed)
            {
                iShowBookmarksListButton.IsChecked = false;
            }
            CloseEditor();
        }

        void iShowAddBookmarkButton_Click(object sender, RoutedEventArgs e)
        {

            if (iShowAddBookmarkButton.IsChecked.HasValue && iShowAddBookmarkButton.IsChecked.Value)
            {
                if (iCurrentBookmark != null)
                {
                    ShowAddBookmark(new Bookmark(iCurrentBookmark.BreadcrumbTrail));
                }
            }
            else
            {
                iAddBookmarkPopup.IsOpen = false;
                iShowAddBookmarkButton.IsChecked = false;
            }

        }

        public void ShowAddBookmark(Bookmark aBookmark)
        {
            iNewBookmark = aBookmark;
            iTitleEditor.Text = iNewBookmark.Title;
            iBreadcrumbDisplay.Text = BookmarkBreadcrumbTextValueConverter.BreadcrumbTrail(iNewBookmark);
            iAddBookmarkPanel.DataContext = new BookmarkViewModel(iNewBookmark);
            iAddBookmarkPopup.SetPopupOffset(iShowAddBookmarkButton);
            iAddBookmarkPopup.IsOpen = true;
            iShowAddBookmarkButton.IsChecked = true;
            iAddBookmarkButton.Focus();
        }

        void iShowBookmarksListButton_Click(object sender, RoutedEventArgs e)
        {
            if (iShowBookmarksListButton.IsChecked.HasValue && iShowBookmarksListButton.IsChecked.Value)
            {
                iBookmarksListPopup.SetPopupOffset(iShowBookmarksListButton);
                iBookmarksListPopup.IsOpen = true;
                iShowBookmarksListButton.IsChecked = true;
                iBookmarkList.Focus();
            }
            else
            {
                iBookmarksListPopup.IsOpen = false;
                iShowBookmarksListButton.IsChecked = false;
            }
        }

        void iCancelAddBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            iAddBookmarkPopup.IsOpen = false;
        }
        void iAddBookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookmark();
        }

        private void AddBookmark()
        {
            iNewBookmark.Title = iTitleEditor.Text;
            iBookmarkManager.Insert(0, iNewBookmark);
            iAddBookmarkPopup.IsOpen = false;
        }

        void iBookmarkManager_EventBookmarkAdded(object sender, EventArgsBookmark e)
        {
            lock (iBookmarkCollection)
            {
                BookmarkViewModel item = (from s in iBookmarkCollection where s.WrappedItem == e.Bookmark select s).SingleOrDefault();
                if (item == null)
                {
                    int index = iBookmarkManager.IndexOf(e.Bookmark);
                    iBookmarkCollection.Insert(index, new BookmarkViewModel(e.Bookmark));
                }
            }
        }

        void iBookmarkManager_EventBookmarksChanged(object sender, EventArgs e)
        {
            lock (iBookmarkCollection)
            {
                iBookmarkCollection.Clear();
                foreach (Bookmark b in iBookmarkManager.Bookmarks)
                {
                    iBookmarkCollection.Add(new BookmarkViewModel(b));
                }
            }
        }

        void iBookmarkManager_EventBookmarkRemoved(object sender, EventArgsBookmark e)
        {
            lock (iBookmarkCollection)
            {
                BookmarkViewModel item = (from s in iBookmarkCollection where s.WrappedItem == e.Bookmark select s).SingleOrDefault();
                if (item != null)
                {
                    iBookmarkCollection.Remove(item);
                }
            }
        }

        //public BreadcrumbTrail CurrentLocation
        //{
        //    get
        //    {
        //        if (iCurrentBookmark != null)
        //        {
        //            return iCurrentBookmark.BreadcrumbTrail;
        //        }
        //        else
        //        {
        //            return BreadcrumbTrail.Default;
        //        }
        //    }
        //}

        private NavigationController iNavigationController;
        private ObservableCollection<BookmarkViewModel> iBookmarkCollection;
        private ListView iBookmarkList;
        private BookmarkManager iBookmarkManager;
        private object iLock;
        private Popup iBookmarksListPopup;
        private Popup iAddBookmarkPopup;
        private ToggleButton iShowBookmarksListButton;
        private ToggleButton iShowAddBookmarkButton;
        private Button iAddBookmarkButton;
        private Button iCancelAddBookmarkButton;
        private Bookmark iNewBookmark;
        private TextBox iTitleEditor;
        private TextBlock iBreadcrumbDisplay;
        private Panel iAddBookmarkPanel;
        private HelperKinsky iHelperKinsky;
        private bool iOpen;
        private Bookmark iCurrentBookmark;
        private DragHelper iDragHelper;
        private bool iDropTargetSelf;
        private string iPreviousBookmarkTitle;
        private BookmarkViewModel iCurrentEditingModel;
        private BookmarkViewModel iCurrentSelectedModel;
    }

    public enum ENavigationState
    {
        Navigating,
        Navigated,
        Failed
    }

    public class NavigationController
    {
        public event EventHandler<EventArgs> EventNavigationStateChanged;
        public event EventHandler<EventArgs> EventLocationChanged;

        public NavigationController(IBrowser aBrowser, Linn.Kinsky.IContainer aRootContainer, ViewWidgetBreadcrumb aViewWidgetBreadcrumb, HelperKinsky aHelperKinsky)
        {
            iLock = new object();
            iRootContainer = aRootContainer;
            iBrowser = aBrowser;
            iViewWidgetBreadcrumb = aViewWidgetBreadcrumb;
            iScheduler = new Scheduler("NavigationScheduler", 1);
            iNavigationState = ENavigationState.Navigating;
            iViewWidgetBreadcrumb.EventBreadcrumbNavigate += EventBreadcrumbNavigateHandler;
            iBrowser.EventLocationChanged += EventLocationChangedHandler;
            iHelperKinsky = aHelperKinsky;
        }

        public void Open()
        {
            Navigate(iHelperKinsky.LastLocation.BreadcrumbTrail, 5);
        }

        public void Close() { }

        public BreadcrumbTrail Home
        {
            get
            {
                return BreadcrumbTrail.Default;
            }
        }

        public void Retry()
        {
            Navigate(iLastNavigation);
        }

        private void EventLocationChangedHandler(object sender, EventArgs e)
        {
            lock (iLock)
            {
                if (iNavigationState == ENavigationState.Navigated && iCurrentLocation != iBrowser.Location)
                {
                    iCurrentLocation = iBrowser.Location;
                    iViewWidgetBreadcrumb.SetLocation(iCurrentLocation.BreadcrumbTrail);
                    iHelperKinsky.LastLocation.BreadcrumbTrail = iCurrentLocation.BreadcrumbTrail;
                }
            }
            if (iCurrentLocation != null && iCurrentLocation == iBrowser.Location && EventLocationChanged != null)
            {
                EventLocationChanged(this, EventArgs.Empty);
            }
        }

        public Location Location
        {
            get
            {
                lock (iLock)
                {
                    return iCurrentLocation;
                }
            }
        }

        void EventBreadcrumbNavigateHandler(object sender, EventArgsBreadcrumbNavigation e)
        {
            Navigate(e.BreadcrumbTrail);
        }

        public ENavigationState NavigationState
        {
            get
            {
                lock (iLock)
                {
                    return iNavigationState;
                }
            }
        }

        public void Up(int aLevels)
        {
            lock (iLock)
            {
                if (iCurrentLocation != null)
                {
                    int levels = aLevels;
                    if (levels >= iCurrentLocation.BreadcrumbTrail.Count)
                    {
                        levels = iCurrentLocation.BreadcrumbTrail.Count - 1;
                    }
                    Navigate(iCurrentLocation.BreadcrumbTrail.TruncateEnd(levels));
                }
            }
        }

        public void Down(container aContainer)
        {
            lock (iLock)
            {
                KillNavigator();
                iBrowser.Down(aContainer);
            }
        }

        public void Navigate(BreadcrumbTrail aBreadcrumbTrail)
        {
            Navigate(aBreadcrumbTrail, 3);
        }

        public void Navigate(BreadcrumbTrail aBreadcrumbTrail, int aRetryCount)
        {
            iNavigationState = ENavigationState.Navigating;
            OnEventNavigationStateChanged();
            iViewWidgetBreadcrumb.SetLocation(aBreadcrumbTrail);
            lock (iLock)
            {
                iLastNavigation = aBreadcrumbTrail;
                iViewWidgetBreadcrumb.SetLocation(aBreadcrumbTrail);
                KillNavigator();
                bool useNavigator = true;
                if (iCurrentLocation != null)
                {
                    BreadcrumbTrail currentLocationTrail = iCurrentLocation.BreadcrumbTrail;
                    if (currentLocationTrail.Count > aBreadcrumbTrail.Count)
                    {
                        useNavigator = false;
                        for (int i = 0; i < aBreadcrumbTrail.Count && !useNavigator; i++)
                        {
                            useNavigator = aBreadcrumbTrail[i].Id != currentLocationTrail[i].Id && aBreadcrumbTrail[i].Title != currentLocationTrail[i].Title;
                        }
                    }
                }
                if (useNavigator)
                {
                    iNavigationState = ENavigationState.Navigating;
                    iNavigator = new Navigator(iScheduler, iRootContainer, aBreadcrumbTrail);
                    iNavigator.EventFailed += EventFailedHandler;
                    iNavigator.EventSucceeded += EventSucceededHandler;
                    iNavigator.Navigate(aRetryCount);

                }
                else
                {
                    iNavigationState = ENavigationState.Navigated;
                    iBrowser.Up((uint)(iCurrentLocation.BreadcrumbTrail.Count - aBreadcrumbTrail.Count));
                }
            }
        }


        private void KillNavigator()
        {
            lock (iLock)
            {
                if (iNavigator != null)
                {
                    iNavigator.Cancel();
                    iNavigator.EventFailed -= EventFailedHandler;
                    iNavigator.EventSucceeded -= EventSucceededHandler;
                    iNavigator = null;
                }
            }
        }

        private void EventFailedHandler(object aSender, EventArgs aArgs)
        {
            KillNavigator();
            lock (iLock)
            {
                iNavigationState = ENavigationState.Failed;
            }
            OnEventNavigationStateChanged();
        }

        private void EventSucceededHandler(object aSender, EventArgsLocation aArgs)
        {
            KillNavigator();
            lock (iLock)
            {
                iNavigationState = ENavigationState.Navigated;
            }
            OnEventNavigationStateChanged();
            iBrowser.Browse(aArgs.Location);
        }

        private void OnEventNavigationStateChanged()
        {
            if (EventNavigationStateChanged != null)
            {
                EventNavigationStateChanged(this, EventArgs.Empty);
            }
        }

        private Navigator iNavigator;
        private Scheduler iScheduler;
        private object iLock;
        private Linn.Kinsky.IContainer iRootContainer;
        private IBrowser iBrowser;
        private ENavigationState iNavigationState;
        private Location iCurrentLocation;
        private ViewWidgetBreadcrumb iViewWidgetBreadcrumb;
        private BreadcrumbTrail iLastNavigation;
        private HelperKinsky iHelperKinsky;
    }

    class Navigator
    {
        public event EventHandler<EventArgs> EventFailed;
        public event EventHandler<EventArgsLocation> EventSucceeded;
        public Navigator(Scheduler aScheduler, Linn.Kinsky.IContainer aRootContainer, BreadcrumbTrail aBreadcrumbTrail)
        {
            iLock = new object();
            iScheduler = aScheduler;
            iBreadcrumbTrail = new BreadcrumbTrail(aBreadcrumbTrail);
            iRootContainer = aRootContainer;
        }

        public void Cancel()
        {
            lock (iLock)
            {
                iCancelled = true;
            }
        }

        public void Navigate(int aRetryCount)
        {
            if (iBreadcrumbTrail.Count > 0)
            {
                iScheduler.Schedule(() =>
                {
                    lock (iLock)
                    {
                        if (iCancelled)
                        {
                            return;
                        }
                    }

                    LocatorAsync locatorAsync = new LocatorAsync(iRootContainer, iBreadcrumbTrail);
                    locatorAsync.Locate((sender, location) =>
                    {

                        lock (iLock)
                        {
                            if (iCancelled)
                            {
                                return;
                            }
                        }
                        if (location != null)
                        {
                            lock (iLock)
                            {
                                if (!iCancelled)
                                {
                                    OnEventSucceeded(location);
                                }
                            }
                        }
                        else if (aRetryCount > 0)
                        {
                            Thread.Sleep(3000);
                            lock (iLock)
                            {
                                if (!iCancelled)
                                {
                                    Navigate(aRetryCount - 1);
                                }
                            }
                        }
                        else
                        {
                            lock (iLock)
                            {
                                if (!iCancelled)
                                {
                                    OnEventFailed();
                                }
                            }
                        }

                    });

                });
            }
        }

        private void OnEventFailed()
        {
            EventHandler<EventArgs> del = EventFailed;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }
        private void OnEventSucceeded(Location aLocation)
        {
            EventHandler<EventArgsLocation> del = EventSucceeded;
            if (del != null)
            {
                del(this, new EventArgsLocation(aLocation));
            }
        }

        private object iLock;
        private bool iCancelled;
        private Scheduler iScheduler;
        private BreadcrumbTrail iBreadcrumbTrail;
        private Linn.Kinsky.IContainer iRootContainer;
    }

    class EventArgsLocation : EventArgs
    {
        public EventArgsLocation(Location aLocation)
            : base()
        {
            Location = aLocation;
        }

        public Location Location { get; set; }
    }

    class ViewWidgetButton : IViewWidgetButton
    {
        private Object iLockObject;
        private bool iOpen;
        protected Button iButton;
        private RoutedEventHandler iEventHandler;

        public ViewWidgetButton(Button aButton)
        {
            iLockObject = new Object();
            iOpen = false;
            iButton = aButton;
            SetEnabled(false);
            iEventHandler = new RoutedEventHandler(ViewWidgetButton_Click);
            iButton.AddHandler(Button.ClickEvent, iEventHandler);
        }

        #region IViewWidgetButton Members

        public void Open()
        {
            lock (iLockObject)
            {
                Assert.Check(!iOpen);
                OnOpen();
                iOpen = true;
            }
        }

        public void Close()
        {
            lock (iLockObject)
            {
                OnClose();
                iOpen = false;
            }
        }

        public event EventHandler<EventArgs> EventClick;

        #endregion

        void ViewWidgetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen && EventClick != null)
                {
                    EventClick(this, EventArgs.Empty);
                }
            }
        }

        private void SetEnabled(bool aEnabled)
        {
            iButton.Dispatcher.BeginInvoke(new Action(delegate()
            {
                if (aEnabled)
                {
                    iButton.ClearValue(Control.IsEnabledProperty);
                }
                else
                {
                    iButton.IsEnabled = false;
                }
            }));
        }

        protected virtual void OnOpen()
        {
            SetEnabled(true);
        }

        protected virtual void OnClose()
        {
            SetEnabled(false);
        }
    }

    class ViewWidgetButtonSave : ViewWidgetButton
    {
        private DropConverter iDropConverter;
        private IViewSaveSupport iSaveSupport;
        public ViewWidgetButtonSave(Button aButton, DropConverter aDropConverter, IViewSaveSupport aSaveSupport)
            : base(aButton)
        {
            iDropConverter = aDropConverter;
            iSaveSupport = aSaveSupport;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            if (!iButton.Dispatcher.CheckAccess())
            {
                iButton.Dispatcher.BeginInvoke((Action)delegate()
                {
                    DoOpenButton();
                });
            }
            else
            {
                DoOpenButton();
            }
        }

        private void DoOpenButton()
        {
            iButton.AllowDrop = true;
            iButton.DragEnter += EventDragEnter;
            iButton.Drop += EventDragDrop;
            iButton.DragOver += EventDragOver;
            iButton.DragLeave += EventDragLeave;
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (!iButton.Dispatcher.CheckAccess())
            {
                iButton.Dispatcher.BeginInvoke((Action)delegate()
                {
                    DoCloseButton();
                });
            }
            else
            {
                DoCloseButton();
            }
        }

        private void DoCloseButton()
        {

            iButton.AllowDrop = false;
            iButton.DragEnter -= EventDragEnter;
            iButton.Drop -= EventDragDrop;
            iButton.DragOver -= EventDragOver;
            iButton.DragLeave -= EventDragLeave;
        }


        private void EventDragLeave(object sender, DragEventArgs e)
        {
            try
            {
                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, false);
                e.Handled = true;

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in SaveButton.DragLeave: " + ex);
            }
        }

        private void EventDragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Handled = true;

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in SaveButton.DragOver: " + ex);
            }
        }

        private void EventDragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;

                MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                if (draggable != null)
                {
                    if ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                }
                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, true);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in SaveButton.DragEnter: " + ex);
            }
        }

        private void EventDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(typeof(MediaProviderDraggable)))
                {
                    if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {
                        MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                        if (draggable != null)
                        {
                            iSaveSupport.Save(draggable.Media);
                        }
                    }
                }
                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, false);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in SaveButton.DragDrop: " + ex);
            }
        }
    }

    class ViewWidgetButtonWasteBin : ViewWidgetButton
    {
        private DropConverter iDropConverter;
        public ViewWidgetButtonWasteBin(Button aButton, DropConverter aDropConverter)
            : base(aButton)
        {
            iDropConverter = aDropConverter;
        }


        protected override void OnOpen()
        {
            base.OnOpen();
            if (!iButton.Dispatcher.CheckAccess())
            {
                iButton.Dispatcher.BeginInvoke((Action)delegate()
                {
                    DoOpenButton();
                });
            }
            else
            {
                DoOpenButton();
            }
        }

        private void DoOpenButton()
        {
            iButton.AllowDrop = true;
            iButton.DragEnter += EventDragEnter;
            iButton.Drop += EventDragDrop;
            iButton.DragOver += EventDragOver;
            iButton.DragLeave += EventDragLeave;
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (!iButton.Dispatcher.CheckAccess())
            {
                iButton.Dispatcher.BeginInvoke((Action)delegate()
                {
                    DoCloseButton();
                });
            }
            else
            {
                DoCloseButton();
            }
        }

        private void DoCloseButton()
        {

            iButton.AllowDrop = false;
            iButton.DragEnter -= EventDragEnter;
            iButton.Drop -= EventDragDrop;
            iButton.DragOver -= EventDragOver;
            iButton.DragLeave -= EventDragLeave;
        }

        private void EventDragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in WasteBin.DragOver: " + ex);
            }
        }

        private void EventDragLeave(object sender, DragEventArgs e)
        {
            try
            {
                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, false);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in WasteBin.DragLeave: " + ex);
            }
        }

        private void EventDragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;

                MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                if (draggable != null)
                {
                    if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
                else if (e.Data.GetDataPresent("bookmarks"))
                {
                    e.Effects = DragDropEffects.Move;
                }

                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, true);
                e.Handled = true;

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in WasteBin.DragEnter: " + ex);
            }
        }

        private void EventDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;

                MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                if (draggable != null)
                {
                    if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
                else if (e.Data.GetDataPresent("bookmarks"))
                {
                    e.Effects = DragDropEffects.Move;
                }
                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, false);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in WasteBin.DragDrop: " + ex);
            }
        }
    }

    class ViewWidgetPlaylistMediaRenderer : IViewWidgetPlaylist
    {

        private Object iLockObject;
        private bool iOpen;
        private List<MrItem> iMrItems;
        private List<IPlaylistItem> iPlaylistItems;
        private IViewSaveSupport iSaveSupport;
        private Panel iContainer;
        private IPlaylistWidget iPlaylistWidget;
        private IPlaylistSupport iPlaylistSupport;
        private MrItem iSelectedMrItem;
        private bool iGroupByAlbum;

        public ViewWidgetPlaylistMediaRenderer(Panel aContainer, IPlaylistWidget aPlaylistWidget, IViewSaveSupport aSaveSupport, IPlaylistSupport aPlaylistSupport, bool aGroupByAlbum)
        {
            iPlaylistWidget = aPlaylistWidget;
            iLockObject = new Object();
            iOpen = false;
            iSaveSupport = aSaveSupport;
            iContainer = aContainer;
            iMrItems = new List<MrItem>();
            iPlaylistWidget.AllowDrop = true;
            iGroupByAlbum = aGroupByAlbum;

            iPlaylistWidget.PlaylistSelectionChanged += new EventHandler<PlaylistSelectionEventArgs>(iPlaylistWidget_SelectionChanged);
            iPlaylistWidget.PlaylistItemsMoved += new EventHandler<PlaylistDropEventArgs>(iPlaylistWidget_PlaylistItemsMoved);
            iPlaylistWidget.PlaylistItemsAdded += new EventHandler<PlaylistDropEventArgs>(iPlaylistWidget_PlaylistItemsAdded);
            iPlaylistWidget.PlaylistItemsDeleted += new EventHandler<PlaylistDeleteEventArgs>(iPlaylistWidget_PlaylistItemsDeleted);
            iPlaylistWidget.PlaylistMoveUp += new EventHandler<PlaylistMoveEventArgs>(iPlaylistWidget_PlaylistMoveUp);
            iPlaylistWidget.PlaylistMoveDown += new EventHandler<PlaylistMoveEventArgs>(iPlaylistWidget_PlaylistMoveDown);
            iPlaylistWidget.PlaylistSave += new EventHandler<EventArgs>(iPlaylistWidget_PlaylistSave);
            iPlaylistSupport = aPlaylistSupport;
            aPlaylistSupport.EventIsInsertingChanged += new EventHandler<EventArgs>(aPlaylistSupport_EventIsInsertingChanged);
        }

        public void SetGroupByAlbum(bool aGroupByAlbum)
        {
            lock (iLockObject)
            {
                iGroupByAlbum = aGroupByAlbum;
            }
            RefreshPlaylist();
        }

        void aPlaylistSupport_EventIsInsertingChanged(object sender, EventArgs e)
        {
            iPlaylistWidget.SetLoading(iPlaylistSupport.IsInserting(), true);
        }

        void iPlaylistWidget_PlaylistSave(object sender, EventArgs e)
        {
            if (iOpen && iPlaylistWidget.IsSaveEnabled)
            {
                Save();
            }
        }

        void iPlaylistWidget_PlaylistMoveDown(object sender, PlaylistMoveEventArgs e)
        {
            lock (iLockObject)
            {
                int index = e.SelectedItem.Position;
                MrItem p = e.SelectedItem.WrappedItem;

                List<MrItem> items = new List<MrItem>();
                items.Add(p);

                uint afterId = 0;
                if ((index + 1) < iMrItems.Count)
                {
                    p = iMrItems[index + 1];
                    afterId = p.Id;
                }

                if (iOpen && EventPlaylistMove != null)
                {
                    EventPlaylistMove(this, new EventArgsPlaylistMove(afterId, items));
                }
            }
        }

        void iPlaylistWidget_PlaylistMoveUp(object sender, PlaylistMoveEventArgs e)
        {
            lock (iLockObject)
            {
                int index = e.SelectedItem.Position;
                MrItem p = e.SelectedItem.WrappedItem;

                List<MrItem> items = new List<MrItem>();
                items.Add(p);

                uint afterId = 0;
                if (index > 1)
                {
                    p = iMrItems[index - 2];
                    afterId = p.Id;
                }

                if (iOpen && EventPlaylistMove != null)
                {
                    EventPlaylistMove(this, new EventArgsPlaylistMove(afterId, items));
                }
            }
        }

        void iPlaylistWidget_PlaylistItemsDeleted(object sender, PlaylistDeleteEventArgs e)
        {
            if (iOpen && EventPlaylistDelete != null)
            {
                List<MrItem> items = (from d in e.DeletedItems
                                      select d.WrappedItem).ToList();
                EventPlaylistDelete(this, new EventArgsPlaylistDelete(items));
            }
        }

        void iPlaylistWidget_PlaylistItemsMoved(object sender, PlaylistDropEventArgs e)
        {
            lock (iLockObject)
            {
                if (e.Data != null)
                {
                    uint afterID = 0;
                    if (e.DropIndex > 0)
                    {
                        afterID = iMrItems[e.DropIndex - 1].Id;
                    }
                    List<MrItem> items = new List<MrItem>();
                    foreach (MrItem mrItem in this.iMrItems)
                    {
                        foreach (upnpObject item in e.Data.Media)
                        {
                            if (mrItem.DidlLite[0] == item)
                            {
                                items.Add(mrItem);
                                break;
                            }
                        }
                    }

                    if (iOpen && EventPlaylistMove != null)
                    {
                        EventPlaylistMove(this, new EventArgsPlaylistMove(afterID, items));
                    }
                }
            }
        }

        void iPlaylistWidget_PlaylistItemsAdded(object sender, PlaylistDropEventArgs e)
        {
            lock (iLockObject)
            {
                if (e.Data != null)
                {
                    uint afterID = 0;
                    if (e.DropIndex > 0)
                    {
                        afterID = iMrItems[e.DropIndex - 1].Id;
                    }
                    if (iOpen && EventPlaylistInsert != null)
                    {
                        EventPlaylistInsert(this, new EventArgsPlaylistInsert(afterID, e.Data));
                    }
                }
            }
        }

        void iPlaylistWidget_SelectionChanged(object sender, PlaylistSelectionEventArgs e)
        {
            if (EventSeekTrack != null)
            {
                EventSeekTrack(this, new EventArgsSeekTrack((uint)iMrItems.IndexOf(e.SelectedItem.WrappedItem)));
            }
        }

        #region IViewWidgetPlaylist Members

        public void Open()
        {
            lock (iLockObject)
            {
                Assert.Check(!iOpen);
                iOpen = true;
                iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iContainer.Children.Add(iPlaylistWidget as UIElement);
                    iPlaylistWidget.SetLoading(true, false);
                }));
            }
        }

        public void Close()
        {
            lock (iLockObject)
            {
                iMrItems.Clear();

                if (iOpen)
                {
                    iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        iContainer.Children.Remove(iPlaylistWidget as UIElement);
                    }));
                }

                iOpen = false;

            }
        }

        public void Initialised()
        {
            iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                   {
                       iPlaylistWidget.SetLoading(false, false);
                   }));
        }

        public void SetPlaylist(IList<MrItem> aPlaylist)
        {
            lock (iLockObject)
            {
                if (aPlaylist != null)
                {
                    iMrItems = aPlaylist.ToList();
                }
                else
                {
                    iMrItems = new List<MrItem>();
                }
                RefreshPlaylist();
            }
        }


        private void RefreshPlaylist()
        {
            iContainer.Dispatcher.BeginInvoke(new Action(delegate()
            {
                MrItem selectedMrItem = null;
                bool grouping;
                List<MrItem> items;
                lock (iLockObject)
                {
                    items = iMrItems.ToList();
                    selectedMrItem = iSelectedMrItem;
                    grouping = iGroupByAlbum;
                }
                List<IPlaylistItem> playlistItems = new List<IPlaylistItem>();
                string previousAlbum = null;
                string previousAlbumArtist = null;
                for (int i = 0; i < items.Count; ++i)
                {
                    MrItem current = items[i];
                    // extra logging for ticket #814
                    Assert.Check(items[i].DidlLite.Count > 0);
                    Assert.Check(i == items.Count - 1 || items[i + 1].DidlLite.Count > 0);
                    string nextAlbum = i == items.Count - 1 ? null : DidlLiteAdapter.Album(items[i + 1].DidlLite[0]);
                    string nextAlbumArtist = i == items.Count - 1 ? null : DidlLiteAdapter.AlbumArtist(items[i + 1].DidlLite[0]);
                    string currentAlbum = DidlLiteAdapter.Album(items[i].DidlLite[0]);
                    string currentAlbumArtist = DidlLiteAdapter.AlbumArtist(items[i].DidlLite[0]);
                    if (grouping && (currentAlbum != previousAlbum || currentAlbumArtist != previousAlbumArtist) && currentAlbum != string.Empty && currentAlbum == nextAlbum && currentAlbumArtist == nextAlbumArtist)
                    {
                        PlaylistGroupHeaderItem header = new PlaylistGroupHeaderItem();
                        header.WrappedItem = items[i];
                        header.Position = i - 1;
                        playlistItems.Add(header);
                    }

                    IPlaylistItem newItem = null;
                    if (grouping && (currentAlbum != previousAlbum || currentAlbumArtist != previousAlbumArtist || currentAlbum == string.Empty) && (currentAlbum != nextAlbum || currentAlbumArtist != nextAlbumArtist || currentAlbum == string.Empty))
                    {
                        newItem = new CollapsedPlaylistListItem() { WrappedItem = items[i] };
                    }
                    else
                    {
                        newItem = new PlaylistListItem() { WrappedItem = items[i] };
                    }

                    newItem.Position = i;
                    if (selectedMrItem != null && selectedMrItem == items[i])
                    {
                        newItem.IsPlaying = true;
                    }
                    playlistItems.Add(newItem);
                    previousAlbum = currentAlbum;
                    previousAlbumArtist = currentAlbumArtist;
                }
                iPlaylistWidget.Items = playlistItems;
                iPlaylistWidget.GroupByAlbum = grouping;
                lock (iLockObject)
                {
                    iPlaylistItems = playlistItems;
                }
            }));
        }

        public void SetTrack(MrItem aTrack)
        {
            lock (iLockObject)
            {
                iSelectedMrItem = aTrack;
            }
            iContainer.Dispatcher.BeginInvoke(new Action(delegate()
            {
                PlaylistListItem dummy = new PlaylistListItem() { WrappedItem = aTrack };
                iPlaylistWidget.SetNowPlayingItem(dummy, false);
            }));

        }

        public void Save()
        {
            if (iOpen && iPlaylistWidget.IsSaveEnabled)
            {
                List<upnpObject> list = new List<upnpObject>();

                lock (iLockObject)
                {
                    foreach (MrItem i in iMrItems)
                    {
                        upnpObject o = i.DidlLite[0];
                        list.Add(o);
                    }
                }

                iSaveSupport.Save(list);
            }
        }

        public void Delete()
        {
            if (iOpen && EventPlaylistDeleteAll != null)
            {
                EventPlaylistDeleteAll(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgsSeekTrack> EventSeekTrack;

        public event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;

        public event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;

        public event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;

        public event EventHandler<EventArgs> EventPlaylistDeleteAll;

        #endregion
    }

    class ViewWidgetPlaylistReceiver : IViewWidgetPlaylistReceiver, IViewWidgetSelector<Linn.Kinsky.Room>
    {
        private bool iOpen;
        private List<MrItem> iPlaylistItems;
        private IViewSaveSupport iSaveSupport;
        private Panel iContainer;
        private IPlaylistWidget iPlaylistWidget;
        private Channel iChannel;
        private List<Linn.Kinsky.Room> iRooms;
        private ModelSenders iSenders;
        private bool iScrollToSelected;

        public ViewWidgetPlaylistReceiver(Panel aContainer, IPlaylistWidget aPlaylistWidget, IViewSaveSupport aSaveSupport, ModelSenders aSenders)
        {
            iSenders = aSenders;
            iPlaylistWidget = aPlaylistWidget;
            iPlaylistWidget.AllowDrop = false;
            iOpen = false;
            iContainer = aContainer;
            iSaveSupport = aSaveSupport;
            iPlaylistItems = new List<MrItem>();
            iPlaylistWidget.PlaylistSelectionChanged += new EventHandler<PlaylistSelectionEventArgs>(iPlaylistWidget_SelectionChanged);
            iPlaylistWidget.EventPlaylistItemNavigationClick += new EventHandler<PlaylistSelectionEventArgs>(iPlaylistWidget_EventPlaylistItemNavigationClick);
            iRooms = new List<Linn.Kinsky.Room>();
        }

        void iPlaylistWidget_EventPlaylistItemNavigationClick(object sender, PlaylistSelectionEventArgs e)
        {
            if (e.SelectedItem != null && EventSelectionChanged != null)
            {
                Linn.Kinsky.Room foundRoom = FindRoom(iChannel);

                if (foundRoom != null)
                {
                    EventSelectionChanged(this, new EventArgsSelection<Linn.Kinsky.Room>(foundRoom));
                }
            }
        }

        private Linn.Kinsky.Room FindRoom(Channel aChannel)
        {
            Linn.Kinsky.Room foundRoom = null;

            if (aChannel != null)
            {
                foreach (ModelSender modelSender in iSenders.SendersList)
                {
                    foreach (resource r in modelSender.Metadata[0].Res)
                    {
                        if (r.Uri == aChannel.Uri)
                        {
                            foundRoom = (from room in iRooms where room.Name == modelSender.Room select room).SingleOrDefault();
                            if (foundRoom != null)
                            {
                                break;
                            }
                        }
                    }
                    if (foundRoom != null)
                    {
                        break;
                    }
                }
            }
            return foundRoom;
        }

        void iPlaylistWidget_PlaylistItemDropped(object sender, PlaylistDropEventArgs e)
        {
            if (e.Data != null)
            {
                if (EventSetChannel != null)
                {
                    EventSetChannel(this, new EventArgsSetChannel(new MediaRetrieverNoRetrieve(e.Data.Media)));
                }
            }
        }

        void iPlaylistWidget_SelectionChanged(object sender, PlaylistSelectionEventArgs e)
        {
            if (EventSetChannel != null)
            {
                List<upnpObject> items = new List<upnpObject>();
                items.Add(e.SelectedItem.WrappedItem.DidlLite[0]);
                EventSetChannel(this, new EventArgsSetChannel(new MediaRetrieverNoRetrieve(items)));
            }
        }

        #region ViewWidgetPlaylistReceiver Members

        public void Open()
        {
            Assert.Check(!iOpen);
            iOpen = true;
            iContainer.Children.Add(iPlaylistWidget as UIElement);
            iPlaylistWidget.SetLoading(true, false);
            iScrollToSelected = true;
        }

        public void Close()
        {
            if (iOpen)
            {
                iPlaylistItems.Clear();
                iContainer.Children.Remove(iPlaylistWidget as UIElement);
            }
            iOpen = false;
        }

        public void Initialised()
        {

            iPlaylistWidget.SetLoading(false, false);

        }

        public void SetSenders(IList<ModelSender> aSenders)
        {
            iPlaylistItems.Clear();
            List<IPlaylistItem> items = new List<IPlaylistItem>();
            for (int i = 0; i < aSenders.Count; ++i)
            {
                if (aSenders[i].Metadata != null)
                {
                    MrItem p = new MrItem(0, "", aSenders[i].Metadata);
                    iPlaylistItems.Add(p);
                    SenderListItem item = new SenderListItem() { WrappedItem = p, Sender = aSenders[i], Name = aSenders[i].FullName };
                    items.Add(item);
                }
            }
            iPlaylistWidget.Items = items;
            UpdateChannel(iChannel);
        }

        public void SetChannel(Channel aChannel)
        {
            UpdateChannel(aChannel);
        }

        private void UpdateChannel(Channel aChannel)
        {
            SenderListItem newItem = null;
            if (aChannel != null)
            {
                MrItem found = (from m in iPlaylistItems
                                where m.DidlLite[0].Title == aChannel.DidlLite[0].Title
                                select m).SingleOrDefault();
                Linn.Kinsky.Room foundRoom = FindRoom(aChannel);
                if (found != null)
                {
                    newItem = new SenderListItem() { WrappedItem = found, HasRoom = foundRoom != null };
                }
            }

            iPlaylistWidget.SetNowPlayingItem(newItem, iScrollToSelected);
            if (newItem != null)
            {
                iScrollToSelected = false;
            }
            iChannel = aChannel;
        }

        public void Save()
        {
        }

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        #endregion

        #region IViewWidgetSelector<Room> Members

        void IViewWidgetSelector<Linn.Kinsky.Room>.Open() { }
        void IViewWidgetSelector<Linn.Kinsky.Room>.Close()
        {
            iRooms.Clear();
        }


        public void InsertItem(int aIndex, Linn.Kinsky.Room aItem)
        {
            iRooms.Add(aItem);
            UpdateChannel(iChannel);
        }

        public void RemoveItem(Linn.Kinsky.Room aItem)
        {
            iRooms.Remove(aItem);
            UpdateChannel(iChannel);
        }

        public void ItemChanged(Linn.Kinsky.Room aItem)
        {
        }

        public void SetSelected(Linn.Kinsky.Room aItem)
        {
        }

        public event EventHandler<EventArgsSelection<Linn.Kinsky.Room>> EventSelectionChanged;

        #endregion
    }


    class ViewWidgetPlaylistRadio : IViewWidgetPlaylistRadio
    {
        private Object iLockObject;
        private bool iOpen;
        private List<MrItem> iPlaylistItems;
        private IViewSaveSupport iSaveSupport;
        private Panel iContainer;
        private IPlaylistWidget iPlaylistWidget;
        private Channel iChannel;
        private int iPresetIndex;
        private bool iScrollToSelected;

        public ViewWidgetPlaylistRadio(Panel aContainer, IPlaylistWidget aPlaylistWidget, IViewSaveSupport aSaveSupport)
        {
            iPlaylistWidget = aPlaylistWidget;
            iPlaylistWidget.AllowDrop = false;

            iLockObject = new Object();
            iOpen = false;
            iContainer = aContainer;
            iSaveSupport = aSaveSupport;
            iPlaylistItems = new List<MrItem>();
            iPlaylistWidget.PlaylistSelectionChanged += new EventHandler<PlaylistSelectionEventArgs>(iPlaylistWidget_SelectionChanged);
            iPlaylistWidget.PlaylistItemsAdded += new EventHandler<PlaylistDropEventArgs>(iPlaylistWidget_PlaylistItemDropped);
            iPresetIndex = -1;
        }

        void iPlaylistWidget_SelectionChanged(object sender, PlaylistSelectionEventArgs e)
        {
            if (iOpen && EventSetPreset != null)
            {
                EventSetPreset(this, new EventArgsSetPreset(e.SelectedItem.WrappedItem));
            }
        }
        void iPlaylistWidget_PlaylistItemDropped(object sender, PlaylistDropEventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen && e.Data != null)
                {
                    if (EventSetChannel != null)
                    {
                        EventSetChannel(this, new EventArgsSetChannel(new MediaRetrieverNoRetrieve(e.Data.Media)));
                    }
                }
            }
        }

        #region IViewWidgetPlaylistRadio Members

        public void Open()
        {
            lock (iLockObject)
            {
                Assert.Check(!iOpen);
                iOpen = true;
                iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iContainer.Children.Add(iPlaylistWidget as UIElement);
                    iPlaylistWidget.SetLoading(true, false);
                    iScrollToSelected = true;
                }));
            }
        }

        public void Close()
        {
            lock (iLockObject)
            {

                if (iOpen)
                {
                    iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        iPlaylistItems.Clear();
                        iContainer.Children.Remove(iPlaylistWidget as UIElement);
                    }));
                }

                iOpen = false;

            }
        }

        public void Initialised()
        {
            iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iPlaylistWidget.SetLoading(false, false);
                }));
        }

        public void SetPresets(IList<MrItem> aPresets)
        {
            lock (iLockObject)
            {

                iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iPlaylistItems.Clear();
                    List<IPlaylistItem> items = new List<IPlaylistItem>();
                    for (int i = 0; i < aPresets.Count; ++i)
                    {
                        MrItem p = aPresets[i];
                        iPlaylistItems.Add(p);
                        items.Add(new RadioListItem() { WrappedItem = p, Position = i });
                    }
                    iPlaylistWidget.Items = items;

                    UpdatePresetIndex();
                }));
            }
        }

        public void SetChannel(Channel aChannel)
        {
            lock (iLockObject)
            {
                iChannel = aChannel;
            }
        }

        public void SetPreset(int aPresetIndex)
        {
            lock (iLockObject)
            {
                iPresetIndex = aPresetIndex;
                iContainer.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    UpdatePresetIndex();
                }));
            }
        }

        private void UpdatePresetIndex()
        {
            if (iPresetIndex >= 0 && iPresetIndex < iPlaylistItems.Count)
            {
                PlaylistListItem item = new PlaylistListItem();
                item.WrappedItem = iPlaylistItems[iPresetIndex];
                iPlaylistWidget.SetNowPlayingItem(item, iScrollToSelected);
                iScrollToSelected = false;
            }
            else
            {
                iPlaylistWidget.SetNowPlayingItem(null, false);
            }
        }

        public void Save()
        {
            if (iOpen && iPlaylistWidget.IsSaveEnabled)
            {
                List<upnpObject> list = new List<upnpObject>();

                lock (iLockObject)
                {
                    if (iChannel != null)
                    {
                        list.AddRange(iChannel.DidlLite);
                    }
                }

                iSaveSupport.Save(list);
            }
        }

        public event EventHandler<EventArgsSetPreset> EventSetPreset;

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        #endregion
    }

    class ViewWidgetPlayMode : IViewWidgetPlayMode
    {
        public ViewWidgetPlayMode(ToggleButton aButtonRepeat, ToggleButton aButtonShuffle)
        {
            iLockObject = new Object();
            iOpen = false;
            iButtonRepeat = aButtonRepeat;
            iButtonRepeat.IsEnabled = false;
            iButtonShuffle = aButtonShuffle;
            iButtonShuffle.IsEnabled = false;
        }

        public void Open()
        {
            lock (iLockObject)
            {

                Assert.Check(!iOpen);

                iButtonRepeat.Click += EventButtonRepeat_Click;
                iButtonShuffle.Click += EventButtonShuffle_Click;

                iOpen = true;

            }
        }

        public void Close()
        {
            lock (iLockObject)
            {

                if (iOpen)
                {
                    iButtonRepeat.Click -= EventButtonRepeat_Click;
                    iButtonShuffle.Click -= EventButtonShuffle_Click;
                }

                iButtonRepeat.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iButtonRepeat.IsEnabled = false;
                }));

                iButtonShuffle.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iButtonShuffle.IsEnabled = false;
                }));

                iOpen = false;

            }
        }

        public void Initialised()
        {
            iButtonRepeat.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iButtonRepeat.ClearValue(Control.IsEnabledProperty);
            }));

            iButtonShuffle.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iButtonShuffle.ClearValue(Control.IsEnabledProperty);
            }));
        }

        public void SetShuffle(bool aShuffle)
        {
            iButtonShuffle.Dispatcher.BeginInvoke(new Action(delegate()
            {
                lock (iLockObject)
                {
                    if (iOpen)
                    {
                        iButtonShuffle.IsChecked = aShuffle;
                        ToolTipService.SetToolTip(iButtonShuffle, "Turn shuffle " + (aShuffle ? "off" : "on"));
                    }
                }
            }));
        }

        public void SetRepeat(bool aRepeat)
        {
            iButtonRepeat.Dispatcher.BeginInvoke(new Action(delegate()
            {
                lock (iLockObject)
                {
                    if (iOpen)
                    {
                        iButtonRepeat.IsChecked = aRepeat;
                        ToolTipService.SetToolTip(iButtonRepeat, "Turn repeat " + (aRepeat ? "off" : "on"));
                    }
                }
            }));
        }

        public event EventHandler<EventArgs> EventToggleShuffle;
        public event EventHandler<EventArgs> EventToggleRepeat;

        private void EventButtonRepeat_Click(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    if (EventToggleRepeat != null)
                    {
                        EventToggleRepeat(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void EventButtonShuffle_Click(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    if (EventToggleShuffle != null)
                    {
                        EventToggleShuffle(this, EventArgs.Empty);
                    }
                }
            }
        }

        private Object iLockObject;
        private bool iOpen;

        private ToggleButton iButtonRepeat;
        private ToggleButton iButtonShuffle;
    }


    class ViewWidgetPlayNowNextLater : IViewWidgetTransportControl
    {

        public ViewWidgetPlayNowNextLater(DropConverter aDropConverter, IPlaylistSupport aPlaylistSupport, Button aButtonPlayNow, Button aButtonPlayNext, Button aButtonPlayLater)
        {

            iLockObject = new Object();
            iOpen = false;
            iPlaylistSupport = aPlaylistSupport;
            iDropConverter = aDropConverter;
            iOpen = false;
            iButtonPlayNow = aButtonPlayNow;
            iButtonPlayNext = aButtonPlayNext;
            iButtonPlayLater = aButtonPlayLater;
        }

        private void PlaylistButton_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                Button source = sender as Button;
                e.Effects = DragDropEffects.None;

                iPlaylistSupport.SetDragging(true);
                MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                if (draggable != null)
                {
                    if (!iPlaylistSupport.IsInserting())
                    {
                        if ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
                        {
                            if ((source == iButtonPlayNow && iPlayNowEnabled) ||
                                (source == iButtonPlayNext && iPlayNextEnabled) ||
                                (source == iButtonPlayLater && iPlayLaterEnabled))
                            {
                                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, true);
                            }
                        }
                    }
                }
                e.Handled = true;

            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in PlayNowNextLater.DragEnter: " + ex);
            }
        }

        private void PlaylistButton_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, false);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in PlayNowNextLater.DragLeave: " + ex);
            }
        }

        private void PlaylistButton_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Button source = sender as Button;
                e.Effects = DragDropEffects.None;

                MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                if (draggable != null)
                {
                    if (!iPlaylistSupport.IsInserting())
                    {
                        if ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
                        {
                            if ((source == iButtonPlayNow && iPlayNowEnabled) ||
                                (source == iButtonPlayNext && iPlayNextEnabled) ||
                                (source == iButtonPlayLater && iPlayLaterEnabled))
                            {
                                (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, true);
                                e.Effects = DragDropEffects.Copy;
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in PlayNowNextLater.DragOver: " + ex);
            }
        }

        private void PlaylistButton_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Button source = sender as Button;
                if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                    if (draggable != null)
                    {
                        if (source == iButtonPlayNow && iPlayNowEnabled)
                        {
                            iPlaylistSupport.PlayNow(draggable);
                        }
                        else if (source == iButtonPlayNext && iPlayNextEnabled)
                        {
                            iPlaylistSupport.PlayNext(draggable);
                        }
                        else if (source == iButtonPlayLater && iPlayLaterEnabled)
                        {
                            iPlaylistSupport.PlayLater(draggable);
                        }
                    }

                    (sender as Button).SetValue(DragHelper.IsDraggedOverProperty, false);
                    iPlaylistSupport.SetDragging(false);
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in PlayNowNextLater.DragDrop: " + ex);
            }
        }
        #region IViewWidgetTransportControl Members

        public void Open()
        {
            lock (iLockObject)
            {
                Assert.Check(!iOpen);
                iButtonPlayNow.DragEnter += PlaylistButton_DragEnter;
                iButtonPlayNext.DragEnter += PlaylistButton_DragEnter;
                iButtonPlayLater.DragEnter += PlaylistButton_DragEnter;
                iButtonPlayNow.DragLeave += PlaylistButton_DragLeave;
                iButtonPlayNext.DragLeave += PlaylistButton_DragLeave;
                iButtonPlayLater.DragLeave += PlaylistButton_DragLeave;
                iButtonPlayNow.DragOver += PlaylistButton_DragOver;
                iButtonPlayNext.DragOver += PlaylistButton_DragOver;
                iButtonPlayLater.DragOver += PlaylistButton_DragOver;
                iButtonPlayNow.Drop += PlaylistButton_Drop;
                iButtonPlayNext.Drop += PlaylistButton_Drop;
                iButtonPlayLater.Drop += PlaylistButton_Drop;
                iOpen = true;
            }
        }

        public void Close()
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    iButtonPlayNow.DragEnter -= PlaylistButton_DragEnter;
                    iButtonPlayNext.DragEnter -= PlaylistButton_DragEnter;
                    iButtonPlayLater.DragEnter -= PlaylistButton_DragEnter;
                    iButtonPlayNow.DragLeave -= PlaylistButton_DragLeave;
                    iButtonPlayNext.DragLeave -= PlaylistButton_DragLeave;
                    iButtonPlayLater.DragLeave -= PlaylistButton_DragLeave;
                    iButtonPlayNow.DragOver -= PlaylistButton_DragOver;
                    iButtonPlayNext.DragOver -= PlaylistButton_DragOver;
                    iButtonPlayLater.DragOver -= PlaylistButton_DragOver;
                    iButtonPlayNow.Drop -= PlaylistButton_Drop;
                    iButtonPlayNext.Drop -= PlaylistButton_Drop;
                    iButtonPlayLater.Drop -= PlaylistButton_Drop;
                    iOpen = false;
                }
            }
        }

        public void Initialised()
        {
        }

        public void SetPlayNowEnabled(bool aEnabled)
        {
            lock (iLockObject)
            {
                iPlayNowEnabled = aEnabled;
            }
        }

        public void SetPlayNextEnabled(bool aEnabled)
        {
            lock (iLockObject)
            {
                iPlayNextEnabled = aEnabled;
            }
        }

        public void SetPlayLaterEnabled(bool aEnabled)
        {
            lock (iLockObject)
            {
                iPlayLaterEnabled = aEnabled;
            }
        }

        public void SetDragging(bool aDragging)
        {
            lock (iLockObject)
            {
                iDragging = aDragging;
            }
        }

        public void SetTransportState(ETransportState aTransportState) { }
        public void SetDuration(uint aDuration) { }
        public void SetAllowSkipping(bool aAllowSkipping) { }
        public void SetAllowPausing(bool aAllowPausing) { }
        public event EventHandler<EventArgs> EventPause;
        public event EventHandler<EventArgs> EventPlay;
        public event EventHandler<EventArgs> EventStop;
        public event EventHandler<EventArgs> EventPrevious;
        public event EventHandler<EventArgs> EventNext;
        public event EventHandler<EventArgsPlay> EventPlayNow;
        public event EventHandler<EventArgsPlay> EventPlayNext;
        public event EventHandler<EventArgsPlay> EventPlayLater;

        #endregion

        private object iLockObject;
        private bool iOpen;
        private bool iDragging;
        private Button iButtonPlayNow;
        private Button iButtonPlayNext;
        private Button iButtonPlayLater;
        private bool iPlayNowEnabled;
        private bool iPlayNextEnabled;
        private bool iPlayLaterEnabled;
        private IPlaylistSupport iPlaylistSupport;
        private DropConverter iDropConverter;
    }

    class ViewWidgetTransportControl : IViewWidgetTransportControl
    {
        public ViewWidgetTransportControl(KinskyDesktop aParentWindow, ThreekArray aThreekArrayControl, DropConverter aDropConverter, IPlaylistSupport aPlaylistSupport)
        {
            iParentWindow = aParentWindow;
            iLockObject = new Object();
            iOpen = false;

            iPlaylistSupport = aPlaylistSupport;
            iDropConverter = aDropConverter;
            iThreekArrayControl = aThreekArrayControl;
            iAllowSkipping = false;
            aParentWindow.PreviewKeyUp += new KeyEventHandler(iThreekArrayControl_KeyUp);
        }

        void iThreekArrayControl_KeyUp(object sender, KeyEventArgs e)
        {
            bool allowPause = iThreekArrayControl.IsUsingPauseButton;
            if (e.Key == Key.MediaPlayPause)
            {
                if (iTransportState == ETransportState.ePlaying || iTransportState == ETransportState.eBuffering)
                {
                    if (allowPause)
                    {
                        EventPause(this, EventArgs.Empty);
                    }
                    else
                    {
                        EventStop(this, EventArgs.Empty);
                    }
                }
                else if (iTransportState == ETransportState.ePaused || iTransportState == ETransportState.eStopped)
                {
                    EventPlay(this, EventArgs.Empty);
                }
            }
            else if (e.Key == Key.MediaStop && iTransportState != ETransportState.eStopped)
            {
                if (allowPause)
                {
                    EventPause(this, EventArgs.Empty);
                }
                else
                {
                    EventStop(this, EventArgs.Empty);
                }
            }
            else if (e.Key == Key.MediaNextTrack && iAllowSkipping && EventNext != null)
            {
                EventNext(this, EventArgs.Empty);
            }
            else if (e.Key == Key.MediaPreviousTrack && iAllowSkipping && EventPrevious != null)
            {
                EventPrevious(this, EventArgs.Empty);
            }
        }

        public void Open()
        {
            lock (iLockObject)
            {

                Assert.Check(!iOpen);


                iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iThreekArrayControl.ControlLeftEnabled = iAllowSkipping;
                    iThreekArrayControl.ControlMiddleEnabled = true;
                    iThreekArrayControl.ControlRightEnabled = iAllowSkipping;
                    iThreekArrayControl.IsEnabled = true;
                }));

                iThreekArrayControl.EventClickLeft += EventThreekArrayControl_EventLeftClick;
                iThreekArrayControl.EventClickMiddle += EventThreekArrayControl_EventMiddleClick;
                iThreekArrayControl.EventClickRight += EventThreekArrayControl_EventRightClick;

                iThreekArrayControl.EventDragDropLeft += EventDragDropLeft;
                iThreekArrayControl.EventDragDropMiddle += EventDragDropMiddle;
                iThreekArrayControl.EventDragDropRight += EventDragDropRight;
                iThreekArrayControl.EventDragOverLeft += EventDragOver;
                iThreekArrayControl.EventDragOverMiddle += EventDragOver;
                iThreekArrayControl.EventDragOverRight += EventDragOver;


                SetDragging(iPlaylistSupport.IsDragging());

                iOpen = true;

            }
        }

        public void Close()
        {
            lock (iLockObject)
            {

                if (iOpen)
                {

                    iThreekArrayControl.EventClickLeft -= EventThreekArrayControl_EventLeftClick;
                    iThreekArrayControl.EventClickMiddle -= EventThreekArrayControl_EventMiddleClick;
                    iThreekArrayControl.EventClickRight -= EventThreekArrayControl_EventRightClick;

                    iThreekArrayControl.EventDragDropLeft -= EventDragDropLeft;
                    iThreekArrayControl.EventDragDropMiddle -= EventDragDropMiddle;
                    iThreekArrayControl.EventDragDropRight -= EventDragDropRight;
                    iThreekArrayControl.EventDragOverLeft -= EventDragOver;
                    iThreekArrayControl.EventDragOverMiddle -= EventDragOver;
                    iThreekArrayControl.EventDragOverRight -= EventDragOver;
                }

                iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    iThreekArrayControl.ControlLeftEnabled = false;
                    iThreekArrayControl.ControlMiddleEnabled = false;
                    iThreekArrayControl.ControlRightEnabled = false;
                    iThreekArrayControl.IsEnabled = false;
                }));

                iOpen = false;

            }
        }

        public void Initialised()
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iThreekArrayControl.IsEnabled = true;
            }));
        }

        public void SetPlayNowEnabled(bool aEnabled)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iThreekArrayControl.PlaylistMiddleEnabled = aEnabled;
            }));
        }

        public void SetPlayNextEnabled(bool aEnabled)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iThreekArrayControl.PlaylistRightEnabled = aEnabled;
            }));
        }

        public void SetPlayLaterEnabled(bool aEnabled)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iThreekArrayControl.PlaylistLeftEnabled = aEnabled;
            }));
        }

        public void SetDragging(bool aDragging)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iThreekArrayControl.IsDragging = aDragging;
            }));
        }

        public void SetTransportState(ETransportState aTransportState)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iTransportState = aTransportState;
                switch (aTransportState)
                {
                    case ETransportState.eBuffering:
                    case ETransportState.ePlaying:
                        iThreekArrayControl.IsPlaying = true;
                        break;

                    default:
                        iThreekArrayControl.IsPlaying = false;
                        break;
                }
            }));
        }

        public void SetDuration(uint aDuration)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iDuration = aDuration;
                iThreekArrayControl.IsUsingPauseButton = aDuration != 0 && iAllowPausing;
            }));
        }

        public void SetAllowSkipping(bool aAllowSkipping)
        {
            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iAllowSkipping = aAllowSkipping;
                iThreekArrayControl.ControlLeftEnabled = iAllowSkipping;
                iThreekArrayControl.ControlRightEnabled = iAllowSkipping;
            }));
        }

        public void SetAllowPausing(bool aAllowPausing)
        {

            iThreekArrayControl.Dispatcher.BeginInvoke(new Action(delegate()
            {
                iAllowPausing = aAllowPausing;
                iThreekArrayControl.IsUsingPauseButton = iDuration != 0 && iAllowPausing;
            }));
        }


        public event EventHandler<EventArgs> EventPause;
        public event EventHandler<EventArgs> EventPlay;
        public event EventHandler<EventArgs> EventStop;

        public event EventHandler<EventArgs> EventPrevious;
        public event EventHandler<EventArgs> EventNext;

        public event EventHandler<EventArgsPlay> EventPlayNow;
        public event EventHandler<EventArgsPlay> EventPlayNext;
        public event EventHandler<EventArgsPlay> EventPlayLater;

        private void OnLeftClick()
        {
            if (EventPrevious != null)
            {
                EventPrevious(this, EventArgs.Empty);
            }
        }

        private void OnMiddleClick()
        {
            bool usePause = iDuration != 0 && iAllowPausing;
            if (iTransportState == ETransportState.ePlaying)
            {
                if (usePause && EventPause != null)
                {
                    EventPause(this, EventArgs.Empty);
                }
                else if (EventStop != null)
                {
                    EventStop(this, EventArgs.Empty);
                }
            }
            else if (iTransportState == ETransportState.eBuffering && EventStop != null)
            {
                EventStop(this, EventArgs.Empty);
            }
            else if (EventPlay != null)
            {
                EventPlay(this, EventArgs.Empty);
            }
        }

        private void OnRightClick()
        {
            if (EventNext != null)
            {
                EventNext(this, EventArgs.Empty);
            }
        }

        private void EventThreekArrayControl_EventLeftClick(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    OnLeftClick();
                }
            }
        }

        private void EventThreekArrayControl_EventMiddleClick(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    OnMiddleClick();
                }
            }
        }

        private void EventThreekArrayControl_EventRightClick(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    OnRightClick();
                }
            }
        }

        private void EventDragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;

                MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                if (draggable != null)
                {
                    if (!iPlaylistSupport.IsInserting())
                    {
                        if ((e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
                        {
                            e.Effects = DragDropEffects.Copy;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in TransportControl.DragOver: " + ex);
            }
        }

        private void EventDragDropLeft(object sender, DragEventArgs e)
        {
            try
            {
                if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                    if (draggable != null)
                    {
                        if (EventPlayLater != null)
                        {
                            EventPlayLater(this, new EventArgsPlay(draggable));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in TransportControl.DragDropLeft: " + ex);
            }
        }

        private void EventDragDropMiddle(object sender, DragEventArgs e)
        {
            try
            {
                if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                    if (draggable != null)
                    {
                        if (EventPlayNow != null)
                        {
                            EventPlayNow(this, new EventArgsPlay(draggable));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in TransportControl.DragDropMiddle: " + ex);
            }
        }

        private void EventDragDropRight(object sender, DragEventArgs e)
        {
            try
            {
                if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    MediaProviderDraggable draggable = iDropConverter.Convert(e.Data);
                    if (draggable != null)
                    {
                        if (EventPlayNext != null)
                        {
                            EventPlayNext(this, new EventArgsPlay(draggable));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Exception caught in TransportControl.DragDropRight: " + ex);
            }
        }

        private Object iLockObject;
        private bool iOpen;

        private IPlaylistSupport iPlaylistSupport;
        private KinskyDesktop iParentWindow;
        private DropConverter iDropConverter;

        private ThreekArray iThreekArrayControl;
        private ETransportState iTransportState;
        private uint iDuration;
        private bool iAllowSkipping;
        private bool iAllowPausing;
    }

    class ViewWidgetVolumeControl : IViewWidgetVolumeControl
    {
        public ViewWidgetVolumeControl(KinskyDesktop aParentWindow, Kontrol aRotaryControlVolume, Kontrol aRockerControlVolume)
        {
            iLockObject = new Object();
            iParentWindow = aParentWindow;
            iOpen = false;

            iRotaryControlVolume = aRotaryControlVolume;
            iRotaryControlVolume.IsEnabled = false;
            iRockerControlVolume = aRockerControlVolume;
            iRockerControlVolume.IsEnabled = false;
            iParentWindow.PreviewKeyDown += new KeyEventHandler(Volume_KeyDown);
        }

        void Volume_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.VolumeUp && iOpen && EventVolumeIncrement != null)
            {
                EventVolumeIncrement(this, EventArgs.Empty);
            }
            else if (e.Key == Key.VolumeDown && iOpen && EventVolumeDecrement != null)
            {
                EventVolumeDecrement(this, EventArgs.Empty);
            }
            else if (e.Key == Key.VolumeMute && iOpen && EventMuteChanged != null)
            {
                EventMuteChanged(this, new EventArgsMute(!iMute));
            }
        }

        public void Open()
        {
            lock (iLockObject)
            {

                Assert.Check(!iOpen);

                iRotaryControlVolume.Click += EventClick;
                iRotaryControlVolume.Increment += EventVolumeUp;
                iRotaryControlVolume.Decrement += EventVolumeDown;

                iRockerControlVolume.Click += EventClick;
                iRockerControlVolume.Increment += EventVolumeUp;
                iRockerControlVolume.Decrement += EventVolumeDown;

                iOpen = true;

            }
        }

        public void Close()
        {
            lock (iLockObject)
            {

                if (iOpen)
                {
                    iRotaryControlVolume.Click -= EventClick;
                    iRotaryControlVolume.Increment -= EventVolumeUp;
                    iRotaryControlVolume.Decrement -= EventVolumeDown;

                    iRockerControlVolume.Click -= EventClick;
                    iRockerControlVolume.Increment -= EventVolumeUp;
                    iRockerControlVolume.Decrement -= EventVolumeDown;
                }

                iOpen = false;

                iRotaryControlVolume.Dispatcher.BeginInvoke(new Action(delegate
                {
                    iRotaryControlVolume.IsDimmed = false;
                    iRotaryControlVolume.IsEnabled = false;
                    iRotaryControlVolume.Text = "";
                    Kontrol.SetValue(iRotaryControlVolume, 0d);

                    iRockerControlVolume.IsDimmed = false;
                    iRockerControlVolume.IsEnabled = false;
                    iRockerControlVolume.Text = "";
                    Kontrol.SetValue(iRockerControlVolume, 0d);
                }));

            }
        }

        public void Initialised()
        {
            iRotaryControlVolume.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRotaryControlVolume.IsEnabled = true;
                iRockerControlVolume.IsEnabled = true;
            }));
        }

        public void SetVolume(uint aVolume)
        {
            iRotaryControlVolume.Dispatcher.BeginInvoke(new Action(delegate
            {
                lock (iLockObject)
                {
                    if (iOpen)
                    {


                        iRotaryControlVolume.Text = aVolume.ToString();
                        Kontrol.SetValue(iRotaryControlVolume, aVolume);

                        iRockerControlVolume.Text = aVolume.ToString();
                        Kontrol.SetValue(iRockerControlVolume, aVolume);
                    }
                }
            }));
        }

        public void SetMute(bool aMute)
        {
            lock (iLockObject)
            {
                iMute = aMute;
            }
            iRotaryControlVolume.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRotaryControlVolume.IsDimmed = aMute;
            }));
            iRockerControlVolume.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRockerControlVolume.IsDimmed = aMute;
            }));
        }

        public void SetVolumeLimit(uint aVolumeLimit)
        {
            iRotaryControlVolume.Dispatcher.BeginInvoke(new Action(delegate
            {
                lock (iLockObject)
                {
                    if (iOpen)
                    {

                        Kontrol.SetMaxValue(iRotaryControlVolume, aVolumeLimit);
                        Kontrol.SetMaxValue(iRockerControlVolume, aVolumeLimit);
                    }
                }
            }));
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;

        private void EventClick(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {

                    if (EventMuteChanged != null)
                    {
                        EventMuteChanged(this, new EventArgsMute(!iMute));
                    }
                }
            }
        }

        private void EventVolumeUp(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {

                    if (EventVolumeIncrement != null)
                    {
                        EventVolumeIncrement(this, EventArgs.Empty);
                    }
                }

            }

        }

        private void EventVolumeDown(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {

                    if (EventVolumeDecrement != null)
                    {
                        EventVolumeDecrement(this, EventArgs.Empty);
                    }
                }

            }
        }

        private bool iOpen;
        private Object iLockObject;
        private bool iMute;

        private KinskyDesktop iParentWindow;
        private Kontrol iRotaryControlVolume;
        private Kontrol iRockerControlVolume;
    }

    class ViewWidgetMediaTime : IViewWidgetMediaTime
    {
        public ViewWidgetMediaTime(Kontrol aRotaryControlTracker, Kontrol aRockerControlTracker)
        {
            iLockObject = new Object();
            iDuration = new Time(0);

            iRotaryControlTracker = aRotaryControlTracker;
            iRockerControlTracker = aRockerControlTracker;
            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRotaryControlTracker.IsEnabled = false;
                Kontrol.SetValue(iRotaryControlTracker, 0);
                iRotaryControlTracker.Text = "";

                iRockerControlTracker.IsEnabled = false;
                Kontrol.SetValue(iRockerControlTracker, 0);
                iRockerControlTracker.Text = "";
            }));
        }

        public void Open()
        {
            lock (iLockObject)
            {

                Assert.Check(!iOpen);
                iRotaryControlTracker.Click += EventClick;
                iRotaryControlTracker.UpdateStarted += EventStartSeeking;
                iRotaryControlTracker.UpdateFinished += EventEndSeeking;
                iRotaryControlTracker.UpdateCancelled += EventCancelSeeking;
                iRotaryControlTracker.Increment += EventSeekForwards;
                iRotaryControlTracker.Decrement += EventSeekBackwards;

                iRockerControlTracker.Click += EventClick;
                iRockerControlTracker.UpdateStarted += EventStartSeeking;
                iRockerControlTracker.UpdateFinished += EventEndSeeking;
                iRockerControlTracker.UpdateCancelled += EventCancelSeeking;
                iRockerControlTracker.Increment += EventSeekForwards;
                iRockerControlTracker.Decrement += EventSeekBackwards;


                iOpen = true;

            }
        }

        public void Close()
        {
            lock (iLockObject)
            {

                if (iOpen)
                {
                    iRotaryControlTracker.Click -= EventClick;
                    iRotaryControlTracker.UpdateStarted -= EventStartSeeking;
                    iRotaryControlTracker.UpdateFinished -= EventEndSeeking;
                    iRotaryControlTracker.UpdateCancelled -= EventCancelSeeking;
                    iRotaryControlTracker.Increment -= EventSeekForwards;
                    iRotaryControlTracker.Decrement -= EventSeekBackwards;

                    iRockerControlTracker.Click -= EventClick;
                    iRockerControlTracker.UpdateStarted -= EventStartSeeking;
                    iRockerControlTracker.UpdateFinished -= EventEndSeeking;
                    iRockerControlTracker.UpdateCancelled -= EventCancelSeeking;
                    iRockerControlTracker.Increment -= EventSeekForwards;
                    iRockerControlTracker.Decrement -= EventSeekBackwards;
                }

                iOpen = false;

            }

            SetDuration(0);
            iSeconds = 0;

            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRotaryControlTracker.IsDimmed = false;
                iRotaryControlTracker.IsIndeterminate = false;
                iRotaryControlTracker.IsEnabled = false;
                Kontrol.SetValue(iRotaryControlTracker, 0);
                iRotaryControlTracker.Text = "";

                iRockerControlTracker.IsDimmed = false;
                iRockerControlTracker.IsIndeterminate = false;
                iRockerControlTracker.IsEnabled = false;
                Kontrol.SetValue(iRockerControlTracker, 0);
                iRockerControlTracker.Text = "";
            }));
        }

        public void Initialised()
        {
            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRotaryControlTracker.IsEnabled = true;
                iRockerControlTracker.IsEnabled = true;
            }));
            UpdateSeconds(iSeconds);
        }

        public void SetAllowSeeking(bool aAllowSeeking)
        {
            lock (iLockObject)
            {
                iAllowSeeking = aAllowSeeking;
            }
            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRotaryControlTracker.OuterRingEnabled = iAllowSeeking && iDuration.SecondsTotal != 0;
            }));
            iRockerControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                iRockerControlTracker.OuterRingEnabled = iAllowSeeking && iDuration.SecondsTotal != 0;
            }));
        }

        public void SetTransportState(ETransportState aTransportState)
        {
            SetBuffering(aTransportState == ETransportState.eBuffering);
            SetStopped(aTransportState == ETransportState.eStopped);

            if (aTransportState == ETransportState.ePlaying || aTransportState == ETransportState.ePaused)
            {
                lock (iLockObject)
                {
                    UpdateSeconds(iSeconds);
                    iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        iRotaryControlTracker.IsDimmed = aTransportState == ETransportState.ePaused;
                        iRockerControlTracker.IsDimmed = aTransportState == ETransportState.ePaused;
                    }));
                }
            }
        }

        public void SetDuration(uint aDuration)
        {
            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                EventCancelSeeking(null, EventArgs.Empty);
            }));

            lock (iLockObject)
            {

                iDuration = new Time((int)aDuration);
                float seek = iDuration.SecondsTotal / 100.0f;
                iSeekAmountPerStep = (uint)Math.Round(seek + 0.5f, MidpointRounding.AwayFromZero);
                bool buffering = iBuffering;
                bool stopped = iStopped;

            }

            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                Kontrol.SetMaxValue(iRotaryControlTracker, aDuration);
                Kontrol.SetMaxValue(iRockerControlTracker, aDuration);

                iRotaryControlTracker.OuterRingEnabled = iAllowSeeking && iDuration.SecondsTotal != 0;
                iRockerControlTracker.OuterRingEnabled = iAllowSeeking && iDuration.SecondsTotal != 0;
            }));
        }

        public void SetSeconds(uint aSeconds)
        {
            iSeconds = aSeconds;
            UpdateSeconds(aSeconds);
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;

        private void SetBuffering(bool aBuffering)
        {
            lock (iLockObject)
            {
                iBuffering = aBuffering;
            }

            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (aBuffering)
                {
                    Kontrol.SetValue(iRotaryControlTracker, 0);
                    iRotaryControlTracker.Text = string.Empty;

                    Kontrol.SetValue(iRockerControlTracker, 0);
                    iRockerControlTracker.Text = string.Empty;
                }
                iRotaryControlTracker.IsIndeterminate = aBuffering;
                iRockerControlTracker.IsIndeterminate = aBuffering;
            }));
        }

        private void SetStopped(bool aStopped)
        {
            lock (iLockObject)
            {
                iStopped = aStopped;
            }

            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (aStopped)
                {
                    Kontrol.SetValue(iRotaryControlTracker, 0);
                    iRotaryControlTracker.Text = string.Empty;

                    Kontrol.SetValue(iRockerControlTracker, 0);
                    iRockerControlTracker.Text = string.Empty;
                }
            }));
        }

        private void UpdateSeconds(uint aSeconds)
        {
            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                lock (iLockObject)
                {
                    if (iOpen)
                    {
                        if (!iBuffering && !iStopped)
                        {
                            Time duration = new Time(iDuration.SecondsTotal);


                            Kontrol.SetValue(iRotaryControlTracker, aSeconds);
                            Kontrol.SetValue(iRockerControlTracker, aSeconds);

                            if (!iApplyTargetSeconds)
                            {
                                string t = string.Empty;
                                if (iShowTimeRemaining && duration.SecondsTotal > 0)
                                {
                                    Time time = new Time((int)aSeconds - duration.SecondsTotal);
                                    t = FormatTime(time, duration);
                                }
                                else
                                {
                                    Time time = new Time((int)aSeconds);
                                    t = FormatTime(time, duration);
                                }
                                iRotaryControlTracker.Text = t;
                                iRockerControlTracker.Text = t;
                            }
                        }
                        else
                        {


                            Kontrol.SetValue(iRotaryControlTracker, 0);
                            iRotaryControlTracker.Text = string.Empty;

                            Kontrol.SetValue(iRockerControlTracker, 0);
                            iRockerControlTracker.Text = string.Empty;
                        }
                    }
                    else
                    {
                        Kontrol.SetValue(iRotaryControlTracker, 0);
                        iRotaryControlTracker.Text = string.Empty;

                        Kontrol.SetValue(iRockerControlTracker, 0);
                        iRockerControlTracker.Text = string.Empty;


                    }
                }
            }));
        }

        private void UpdateTargetSeconds()
        {
            iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
            {
                lock (iLockObject)
                {
                    if (iOpen)
                    {
                        Time duration = new Time(iDuration.SecondsTotal);
                        bool buffering = iBuffering;


                        if (!buffering)
                        {
                            if (iApplyTargetSeconds)
                            {
                                string t = string.Empty;
                                if (iShowTimeRemaining)
                                {
                                    Time time = new Time((int)iTargetSeconds - duration.SecondsTotal);
                                    t = FormatTime(time, duration);
                                }
                                else
                                {
                                    Time time = new Time((int)iTargetSeconds);
                                    t = FormatTime(time, duration);
                                }
                                iRotaryControlTracker.Text = t;
                                iRockerControlTracker.Text = t;

                                Kontrol.SetUpdatingValue(iRotaryControlTracker, iTargetSeconds);
                                Kontrol.SetUpdatingValue(iRockerControlTracker, iTargetSeconds);
                            }
                        }
                    }
                }
            }));
        }

        private void EventClick(object sender, EventArgs e)
        {
            iShowTimeRemaining = !iShowTimeRemaining;
            SetSeconds((uint)Kontrol.GetValue(iRotaryControlTracker));
        }

        private void EventStartSeeking(object sender, EventArgs e)
        {
            iApplyTargetSeconds = false;
        }

        private void EventEndSeeking(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    if (iStopped)
                    {
                        iRotaryControlTracker.Text = string.Empty;
                        iRockerControlTracker.Text = string.Empty;
                    }



                    if (iApplyTargetSeconds)
                    {
                        iSeconds = iTargetSeconds;

                        if (EventSeekSeconds != null)
                        {
                            EventSeekSeconds(this, new EventArgsSeekSeconds(iTargetSeconds));
                        }

                        iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
               {
                   Kontrol.SetUpdatingValue(iRotaryControlTracker, 0d);
                   Kontrol.SetUpdatingValue(iRockerControlTracker, 0d);
               }));

                        iApplyTargetSeconds = false;
                    }
                }
            }
        }

        private void EventCancelSeeking(object sender, EventArgs e)
        {
            lock (iLockObject)
            {
                if (iOpen)
                {
                    if (iStopped)
                    {
                        iRotaryControlTracker.Text = string.Empty;
                        iRockerControlTracker.Text = string.Empty;
                    }
                }
                iRotaryControlTracker.Dispatcher.BeginInvoke(new Action(delegate
               {
                   Kontrol.SetUpdatingValue(iRotaryControlTracker, 0d);
                   Kontrol.SetUpdatingValue(iRockerControlTracker, 0d);
               }));

            }

            iApplyTargetSeconds = false;
            iTargetSeconds = 0;
        }

        private void EventSeekForwards(object sender, EventArgs e)
        {
            Kontrol kontrol = sender as Kontrol;
            Assert.Check(kontrol != null);

            lock (iLockObject)
            {
                if (iOpen)
                {
                    if (!iApplyTargetSeconds)
                    {
                        iTargetSeconds = iSeconds;
                        iApplyTargetSeconds = true;
                    }

                    iTargetSeconds += iSeekAmountPerStep;

                    if (iTargetSeconds > iDuration.SecondsTotal)
                    {
                        iTargetSeconds = (uint)iDuration.SecondsTotal;
                    }

                    UpdateTargetSeconds();
                }
            }
        }

        private void EventSeekBackwards(object sender, EventArgs e)
        {
            Kontrol kontrol = sender as Kontrol;
            Assert.Check(kontrol != null);

            lock (iLockObject)
            {
                if (iOpen)
                {
                    if (!iApplyTargetSeconds)
                    {
                        iTargetSeconds = iSeconds;
                        iApplyTargetSeconds = true;
                    }

                    if (iTargetSeconds > iSeekAmountPerStep)
                    {
                        iTargetSeconds -= iSeekAmountPerStep;
                    }
                    else
                    {
                        iTargetSeconds = 0;
                    }



                    UpdateTargetSeconds();
                }
            }
        }

        private string FormatTime(Time aTime, Time aDuration)
        {
            string result = string.Empty;
            int minutes = ((aTime.Hours % 24) * 60) + aTime.Minutes;
            result = minutes + ":" + string.Format("{0:00}", aTime.Seconds);
            if ((iShowTimeRemaining && aDuration.SecondsTotal > 0) || aTime.SecondsTotal < 0)
            {
                result = "-" + result;
            }
            return result;
        }

        private const string kEmptyTime = "\u2013:\u2013\u2013";

        private Object iLockObject;
        private bool iOpen;


        private bool iShowTimeRemaining;

        private bool iApplyTargetSeconds;
        private uint iTargetSeconds;
        private uint iSeekAmountPerStep;

        private bool iBuffering;
        private bool iStopped;
        private Time iDuration;
        private uint iSeconds;

        private Kontrol iRotaryControlTracker;
        private Kontrol iRockerControlTracker;

        private bool iAllowSeeking;
    }

    class ViewWidgetPlaylistAux : IViewWidgetPlaylistAux
    {
        public ViewWidgetPlaylistAux(Panel aContainer)
        {
            iLockObject = new Object();
            iPlaylistWidget = new PlaylistWidgetAux();
            iContainer = aContainer;
        }

        public void Open(string aType)
        {
            lock (iLockObject)
            {

                BitmapSource image = KinskyDesktopWpf.StaticImages.ImageSourceIconAuxSource;
                if (aType == "Spdif")
                {
                    image = KinskyDesktopWpf.StaticImages.ImageSourceIconSpdifSource;
                }
                else if (aType == "Toslink")
                {
                    image = KinskyDesktopWpf.StaticImages.ImageSourceIconTosLinkSource;
                }
                else if (aType == "Disc")
                {
                    image = KinskyDesktopWpf.StaticImages.ImageSourceIconDiscSource;
                }

                iContainer.Dispatcher.BeginInvoke(new Action(() =>
                {
                    iPlaylistWidget.AuxImageSource = image;
                    iContainer.Children.Add(iPlaylistWidget);
                    Trace.WriteLine(Trace.kKinsky, "ViewWidgetPlaylistAux Opened");
                }));

            }
        }

        public void Close()
        {
            lock (iLockObject)
            {

                iContainer.Dispatcher.BeginInvoke(new Action(() =>
                {
                    iContainer.Children.Remove(iPlaylistWidget);
                    Trace.WriteLine(Trace.kKinsky, "ViewWidgetPlaylistAux Closed");
                }));

            }
        }

        private Object iLockObject;
        private Panel iContainer;
        private PlaylistWidgetAux iPlaylistWidget;
    }

    class ViewWidgetPlaylistDiscPlayer : ViewWidgetPlaylistAux, IViewWidgetPlaylistDiscPlayer
    {
        public ViewWidgetPlaylistDiscPlayer(Panel aContainer)
            : base(aContainer)
        { }

        public void Open()
        {
            Open("Disc");
        }

        public void Initialised()
        {
        }

        public void Eject()
        {
        }
    }

    public class ViewWidgetTrack : IViewWidgetTrack, IViewWidgetPlaylist, IViewWidgetPlaylistRadio, IViewWidgetPlaylistReceiver
    {

        private ViewWidgetTrackDisplay iViewWidgetTrackDisplay;
        private IPlaylistWidget[] iPlaylistWidgets;
        private object iLock;
        private bool iOpen;
        private IImageCache<BitmapImage> iFullResCache;
        private const int kCacheSize = 1 * 1024 * 1024;
        private const int kThreadCount = 1;
        private string iPendingImageUri;
        private WpfImageWrapper iErrorImage;

        public ViewWidgetTrack(ViewWidgetTrackDisplay aViewWidgetTrackDisplay, IPlaylistWidget[] aPlaylistWidgets)
        {
            iViewWidgetTrackDisplay = aViewWidgetTrackDisplay;
            iViewWidgetTrackDisplay.TitleButton.Click += new RoutedEventHandler(TitleButton_Click);
            iPlaylistWidgets = aPlaylistWidgets;
            iLock = new object();
            iOpen = false;
            iErrorImage = new WpfImageWrapper(StaticImages.ImageSourceIconLoading);
            iFullResCache = new ThreadedImageCache<BitmapImage>(kCacheSize, 0, kThreadCount, new WpfImageLoader(new ScalingUriConverter(kUpscaleImageSize, true, true)));
            iFullResCache.EventImageAdded += EventImageAddedHandler;
            iFullResCache.EventRequestFailed += EventRequestFailedHandler;
        }

        private void EventRequestFailedHandler(object sender, EventArgsImageFailure e)
        {
            EventImageAddedHandler(sender, new EventArgsImage<BitmapImage>(e.Uri, iErrorImage));
        }

        private void EventImageAddedHandler(object sender, EventArgsImage<BitmapImage> e)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (iPendingImageUri != null && iPendingImageUri == e.Uri)
                {
                    iViewWidgetTrackDisplay.Artwork = e.Image.Image;
                }
            }));
        }

        void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (IPlaylistWidget playlist in iPlaylistWidgets)
            {
                playlist.ScrollToNowPlaying();
            }
        }

        #region IViewWidgetTrack Members

        public void Open()
        {
            lock (iLock)
            {
                Assert.Check(!iOpen);
                iOpen = true;
            }
        }

        public void Close()
        {
            lock (iLock)
            {
                if (iOpen)
                {
                    iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        iViewWidgetTrackDisplay.Artwork = null;
                        iViewWidgetTrackDisplay.Display1 = string.Empty;
                        iViewWidgetTrackDisplay.Display2 = string.Empty;
                        iViewWidgetTrackDisplay.Display3 = string.Empty;
                        iViewWidgetTrackDisplay.Bitrate = 0;
                        iViewWidgetTrackDisplay.SampleRate = 0f;
                        iViewWidgetTrackDisplay.BitDepth = 0;
                        iViewWidgetTrackDisplay.Codec = string.Empty;
                        iViewWidgetTrackDisplay.Lossless = false;
                    }));
                }

                iOpen = false;
            }
        }

        public void Initialised()
        {
        }

        public void SetItem(upnpObject aObject)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (aObject != null)
                {
                    ItemInfo info = new ItemInfo(aObject);
                    iViewWidgetTrackDisplay.Display1 = info.DisplayItem(0).HasValue ? info.DisplayItem(0).Value.Value : string.Empty;
                    iViewWidgetTrackDisplay.Display2 = info.DisplayItem(1).HasValue ? info.DisplayItem(1).Value.Value : string.Empty;
                    iViewWidgetTrackDisplay.Display3 = info.DisplayItem(2).HasValue ? info.DisplayItem(2).Value.Value : string.Empty;
                    iPendingImageUri = null;

                    Uri uri = DidlLiteAdapter.ArtworkUri(aObject);

                    if (uri == null)
                    {
                        iViewWidgetTrackDisplay.Artwork = StaticImages.ImageSourceIconLoading;
                    }
                    else
                    {
                        iViewWidgetTrackDisplay.Artwork = StaticImages.ImageSourceIconLoading;
                        Icon<BitmapImage> icon = KinskyDesktop.Instance.IconResolver.Resolve(aObject);
                        if (icon.IsUri)
                        {
                            iPendingImageUri = icon.ImageUri.OriginalString;
                            IImage<BitmapImage> image = iFullResCache.Image(iPendingImageUri);
                            if (image != null)
                            {
                                iViewWidgetTrackDisplay.Artwork = image.Image;
                                iPendingImageUri = null;
                            }
                        }
                        else
                        {
                            iViewWidgetTrackDisplay.Artwork = icon.Image;
                        }
                    }
                }
                else
                {
                    iViewWidgetTrackDisplay.Display1 = string.Empty;
                    iViewWidgetTrackDisplay.Display2 = string.Empty;
                    iViewWidgetTrackDisplay.Display3 = string.Empty;

                    iViewWidgetTrackDisplay.Codec = string.Empty;
                    iViewWidgetTrackDisplay.BitDepth = 0;
                    iViewWidgetTrackDisplay.SampleRate = 0;
                    iViewWidgetTrackDisplay.Bitrate = 0;
                    iViewWidgetTrackDisplay.Lossless = false;

                    iViewWidgetTrackDisplay.Artwork = null;
                }
            }));
        }

        public void SetMetatext(upnpObject aObject)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
               {
                   if (aObject != null)
                   {
                       ItemInfo info = new ItemInfo(aObject);
                       iViewWidgetTrackDisplay.Display2 = info.DisplayItem(0).HasValue ? info.DisplayItem(0).Value.Value : string.Empty;
                       iViewWidgetTrackDisplay.Display3 = info.DisplayItem(1).HasValue ? info.DisplayItem(1).Value.Value : string.Empty;
                   }
               }));
        }

        public void SetBitrate(uint aBitrate)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.Bitrate = aBitrate;
            }));
        }

        public void SetSampleRate(float aSampleRate)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.SampleRate = aSampleRate;
            }));
        }

        public void SetBitDepth(uint aBitDepth)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.BitDepth = aBitDepth;
            }));
        }

        public void SetCodec(string aCodec)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.Codec = aCodec;
            }));
        }

        public void SetLossless(bool aLossless)
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.Lossless = aLossless;
            }));
        }

        public void Update()
        {
        }

        #endregion





        #region IViewWidgetPlaylist Members
        void IViewWidgetPlaylist.Open()
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.TitleButton.IsEnabled = true;
            }));
        }

        void IViewWidgetPlaylist.Close()
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.TitleButton.IsEnabled = false;
            }));
        }

        public void SetPlaylist(IList<MrItem> aPlaylist)
        {
        }

        public void SetTrack(MrItem aTrack)
        {
        }

        public void Save()
        {
        }

        public void Delete()
        {
        }

        public event EventHandler<EventArgsSeekTrack> EventSeekTrack;

        public event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;

        public event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;

        public event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;

        public event EventHandler<EventArgs> EventPlaylistDeleteAll;

        #endregion

        #region IViewWidgetPlaylistRadio Members

        void IViewWidgetPlaylistRadio.Open()
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.TitleButton.IsEnabled = true;
            }));
        }

        void IViewWidgetPlaylistRadio.Close()
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.TitleButton.IsEnabled = false;
            }));
        }
        public void SetPresets(IList<MrItem> aPresets)
        {
        }

        public void SetChannel(Channel aChannel)
        {
        }

        public void SetPreset(int aPresetIndex)
        {
        }

        public event EventHandler<EventArgsSetPreset> EventSetPreset;

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        #endregion

        #region IViewWidgetPlaylistReceiver Members

        void IViewWidgetPlaylistReceiver.Open()
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.TitleButton.IsEnabled = true;
            }));
        }

        void IViewWidgetPlaylistReceiver.Close()
        {
            iViewWidgetTrackDisplay.Dispatcher.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrackDisplay.TitleButton.IsEnabled = false;
            }));
        }
        public void SetSenders(IList<ModelSender> aSenders)
        {
        }

        #endregion

        private const int kUpscaleImageSize = 2048;
    }

    class ViewSysTrayContextMenu : IViewWidgetTransportControl, IViewWidgetVolumeControl, IViewWidgetPlayMode
    {

        public ViewSysTrayContextMenu(SystrayForm aForm)
        {
            iLockObject = new object();
            iOpen = false;

            iForm = aForm;
            iMenuItemShowHide = aForm.ShowToolStripMenuItem;
            iMenuItemPlayPauseStop = aForm.PlayToolStripMenuItem;
            iMenuItemNext = aForm.NextToolStripMenuItem;
            iMenuItemPrevious = aForm.PreviousToolStripMenuItem;
            iMenuItemShuffle = aForm.ShuffleToolStripMenuItem;
            iMenuItemRepeat = aForm.RepeatToolStripMenuItem;
            iMenuItemMute = aForm.MuteToolStripMenuItem;

            iAllowSkipping = false;
        }

        public void Open() { }

        void IViewWidgetTransportControl.Open()
        {
            if (!iForm.IsDisposed)
            {
                iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    iMenuItemPlayPauseStop.Enabled = false;
                    iMenuItemNext.Enabled = iAllowSkipping;
                    iMenuItemPrevious.Enabled = iAllowSkipping;
                });
            }
        }

        void IViewWidgetTransportControl.Close()
        {
            lock (iLockObject)
            {
                iMenuItemPlayPauseStop.Click -= PlayClick;
                iMenuItemNext.Click -= NextClick;
                iMenuItemPrevious.Click -= PreviousClick;

                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemPlayPauseStop.Enabled = false;
                        iMenuItemNext.Enabled = false;
                        iMenuItemPrevious.Enabled = false;
                    });
                }
            }
        }

        void IViewWidgetVolumeControl.Close()
        {
            lock (iLockObject)
            {

                iMenuItemMute.Click -= MuteClick;
                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemMute.Enabled = false;
                    });
                }
            }
        }

        void IViewWidgetPlayMode.Close()
        {
            lock (iLockObject)
            {

                iMenuItemRepeat.Click -= RepeatClick;
                iMenuItemShuffle.Click -= ShuffleClick;
                if (!iForm.IsDisposed)
                {

                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemRepeat.Enabled = false;
                        iMenuItemShuffle.Enabled = false;
                    });
                }

            }
        }

        void IViewWidgetTransportControl.Initialised()
        {
            lock (iLockObject)
            {

                iMenuItemPlayPauseStop.Click += PlayClick;
                iMenuItemNext.Click += NextClick;
                iMenuItemPrevious.Click += PreviousClick;

                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemPlayPauseStop.Enabled = true;
                        iMenuItemNext.Enabled = iAllowSkipping;
                        iMenuItemPrevious.Enabled = iAllowSkipping;
                    });
                }

            }
        }

        void IViewWidgetVolumeControl.Initialised()
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemMute.Enabled = true;
                    });
                }
                iMenuItemMute.Click += MuteClick;

            }
        }

        void IViewWidgetPlayMode.Initialised()
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemRepeat.Enabled = true;
                        iMenuItemShuffle.Enabled = true;
                    });
                }
                iMenuItemRepeat.Click += RepeatClick;
                iMenuItemShuffle.Click += ShuffleClick;

            }
        }

        public void SetPlayNowEnabled(bool aEnabled) { }
        public void SetPlayNextEnabled(bool aEnabled) { }
        public void SetPlayLaterEnabled(bool aEnabled) { }

        public void SetDragging(bool aDragging) { }

        public void SetTransportState(ETransportState aTransportState)
        {
            lock (iLockObject)
            {

                iTransportState = aTransportState;

                UpdateTransportButton();

            }
        }

        private void UpdateTransportButton()
        {
            if (!iForm.IsDisposed)
            {

                iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    switch (iTransportState)
                    {
                        case ETransportState.ePlaying:
                        case ETransportState.eBuffering:
                            iMenuItemPlayPauseStop.Text = iDuration != 0 && iAllowPausing ? "Pause" : "Stop";
                            iMenuItemPlayPauseStop.Image = iDuration != 0 && iAllowPausing ? kImageSysTrayPause : kImageSysTrayStop;
                            break;

                        default:
                            iMenuItemPlayPauseStop.Text = "Play";
                            iMenuItemPlayPauseStop.Image = kImageSysTrayPlay;
                            break;
                    }
                });
            }
        }


        public void SetDuration(uint aDuration)
        {
            lock (iLockObject)
            {
                iDuration = aDuration;
                UpdateTransportButton();
            }
        }

        public void SetVolume(uint aVolume) { }

        public void SetMute(bool aMute)
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {
                    iMute = aMute;

                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemMute.CheckState = aMute ? System.Windows.Forms.CheckState.Checked : System.Windows.Forms.CheckState.Unchecked;
                    });
                }
            }
        }

        public void SetVolumeLimit(uint aVolumeLimit) { }

        public void SetShuffle(bool aShuffle)
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemShuffle.CheckState = aShuffle ? System.Windows.Forms.CheckState.Checked : System.Windows.Forms.CheckState.Unchecked;
                    });
                }
            }
        }

        public void SetRepeat(bool aRepeat)
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {

                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iMenuItemRepeat.CheckState = aRepeat ? System.Windows.Forms.CheckState.Checked : System.Windows.Forms.CheckState.Unchecked;
                    });
                }

            }
        }

        public void SetAllowSkipping(bool aAllowSkipping)
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iAllowSkipping = aAllowSkipping;
                        iMenuItemPrevious.Enabled = iAllowSkipping;
                        iMenuItemNext.Enabled = iAllowSkipping;
                    });
                }
            }
        }

        public void SetAllowPausing(bool aAllowPausing)
        {
            lock (iLockObject)
            {
                if (!iForm.IsDisposed)
                {
                    iForm.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                    {
                        iAllowPausing = aAllowPausing;
                        UpdateTransportButton();
                    });
                }
            }
        }

        public event EventHandler<EventArgs> EventPause;
        public event EventHandler<EventArgs> EventPlay;
        public event EventHandler<EventArgs> EventStop;

        public event EventHandler<EventArgs> EventPrevious;
        public event EventHandler<EventArgs> EventNext;

        public event EventHandler<EventArgsPlay> EventPlayNow;
        public event EventHandler<EventArgsPlay> EventPlayNext;
        public event EventHandler<EventArgsPlay> EventPlayLater;

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;

        public event EventHandler<EventArgs> EventToggleShuffle;
        public event EventHandler<EventArgs> EventToggleRepeat;

        protected void PlayClick(object sender, EventArgs e)
        {
            bool usePause = iDuration != 0 && iAllowPausing;
            if (iTransportState == ETransportState.ePlaying)
            {
                if (usePause && EventPause != null)
                {
                    EventPause(this, EventArgs.Empty);
                }
                else if (EventStop != null)
                {
                    EventStop(this, EventArgs.Empty);
                }
            }
            else if (iTransportState == ETransportState.eBuffering && EventStop != null)
            {
                EventStop(this, EventArgs.Empty);
            }
            else if (EventPlay != null)
            {
                EventPlay(this, EventArgs.Empty);
            }
        }

        protected void NextClick(object sender, EventArgs e)
        {
            if (EventNext != null)
            {
                EventNext(this, EventArgs.Empty);
            }
        }

        protected void PreviousClick(object sender, EventArgs e)
        {
            if (EventPrevious != null)
            {
                EventPrevious(this, EventArgs.Empty);
            }
        }

        private void MuteClick(object sender, EventArgs e)
        {
            if (EventMuteChanged != null)
            {
                EventMuteChanged(this, new EventArgsMute(!iMute));
            }
        }

        private void ShuffleClick(object sender, EventArgs e)
        {
            if (EventToggleShuffle != null)
            {
                EventToggleShuffle(this, EventArgs.Empty);
            }
        }

        private void RepeatClick(object sender, EventArgs e)
        {
            if (EventToggleRepeat != null)
            {
                EventToggleRepeat(this, EventArgs.Empty);
            }
        }

        private static readonly System.Drawing.Bitmap kImageSysTrayPlay = StaticImages.SysTrayPlay;
        private static readonly System.Drawing.Bitmap kImageSysTrayPause = StaticImages.SysTrayPause;
        private static readonly System.Drawing.Bitmap kImageSysTrayStop = StaticImages.SysTrayStop;

        private object iLockObject;
        private bool iOpen;

        private bool iMute;
        private ETransportState iTransportState;
        private uint iDuration;

        private System.Windows.Forms.ToolStripMenuItem iMenuItemPlayPauseStop;
        private System.Windows.Forms.ToolStripMenuItem iMenuItemPrevious;
        private System.Windows.Forms.ToolStripMenuItem iMenuItemNext;

        private System.Windows.Forms.ToolStripMenuItem iMenuItemRepeat;
        private System.Windows.Forms.ToolStripMenuItem iMenuItemShuffle;

        private System.Windows.Forms.ToolStripMenuItem iMenuItemMute;

        private System.Windows.Forms.ToolStripMenuItem iMenuItemShowHide;
        private SystrayForm iForm;
        private bool iAllowSkipping;
        private bool iAllowPausing;
    }

    abstract class ViewWidgetSelector<T> : IViewWidgetSelector<T>, IComparer where T : class
    {
        protected ObservableCollection<ListViewModelBase> iItems;
        protected ListBox iSelectorList;
        protected Control iSelectorDisplay;
        protected object iSelectedItem;

        public ViewWidgetSelector(ListBox aSelectorList, Control aSelectorDisplay)
        {
            iItems = new ObservableCollection<ListViewModelBase>();
            iSelectorList = aSelectorList;
            iSelectorDisplay = aSelectorDisplay;
            CollectionViewSource viewSource = new CollectionViewSource();
            viewSource.Source = iItems;
            ListCollectionView view = viewSource.View as ListCollectionView;
            view.CustomSort = this;
            iSelectorList.ItemsSource = viewSource.View;
            iSelectorDisplay.DataContext = null;
            iSelectorList.SelectionMode = SelectionMode.Single;
            iSelectorList.PreviewMouseLeftButtonDown += iSelectorList_PreviewMouseLeftButtonDown;
            iSelectorList.PreviewMouseLeftButtonUp += iSelectorList_PreviewMouseLeftButtonUp;
            iSelectorList.KeyDown += new KeyEventHandler(iSelectorList_KeyDown);
        }

        void iSelectorList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListViewItem listItem = e.OriginalSource as ListViewItem;
                if (listItem != null)
                {
                    ListViewModel<T> item = iSelectorList.ItemContainerGenerator.ItemFromContainer(listItem) as ListViewModel<T>;
                    T selected = null;
                    if (item != null)
                    {
                        selected = item.WrappedItem;
                        OnEventSelectionChanged(selected);
                        e.Handled = true;
                    }
                    OnEventUserSelectedItem(selected);
                    if (selected == iSelectedItem)
                    {
                        // force notify consumers if item is reselected as eventing will not pick up a change
                        OnEventNotifyConsumersSelectionChanged(selected);
                    }
                }
            }
        }

        #region IViewWidgetSelector Members

        public void Open()
        {

            Trace.WriteLine(Trace.kKinskyDesktop, "ViewWidgetSelector.Open");
            SetSelectionState(null);
        }

        public void Close()
        {
            Trace.WriteLine(Trace.kKinskyDesktop, "ViewWidgetSelector.Close");
            this.iItems.Clear();
            SetSelectionState(null);
        }

        public virtual void InsertItem(int aIndex, T aItem)
        {
            Trace.WriteLine(Trace.kKinskyDesktop, "ViewWidgetSelector.InsertItem: " + aIndex + ", " + aItem);
            ListViewModelBase model = CreateViewModel(aItem);
            bool withinRange = aIndex <= iItems.Count;
            if (!withinRange)
            {
                UserLog.WriteLine("Out of range insert!" + aItem);
            }
            Assert.Check(withinRange);
            iItems.Insert(aIndex, model);
        }

        public virtual void RemoveItem(T aItem)
        {

            ListViewModelBase foundItem = null;

            Trace.WriteLine(Trace.kKinskyDesktop, "ViewWidgetSelector.RemoveItem: " + aItem);

            foreach (ListViewModelBase item in iItems)
            {
                if (CompareWrappedItem(item, aItem))
                {
                    foundItem = item;
                    break;
                }
            }
            if (foundItem != null)
            {
                this.iItems.Remove(foundItem);
                if (foundItem.IsSelected)
                {
                    SetSelectionState(null);
                }
            }
        }

        public abstract void ItemChanged(T aItem);

        public void SetSelected(T aItem)
        {

            SetSelectionState(aItem);
        }

        protected void OnEventSelectionChanged(T aItem)
        {
            if (EventSelectionChanged != null)
            {
                EventSelectionChanged(this, new EventArgsSelection<T>(aItem));
            }
        }

        protected virtual void SetSelectionState(T aItem)
        {
            iSelectedItem = aItem;
            ListViewModelBase selected = null;
            foreach (ListViewModelBase item in iItems)
            {
                item.IsSelected = CompareWrappedItem(item, aItem);
                if (item.IsSelected)
                {
                    selected = item;
                    iSelectorList.SelectedItem = item;
                }
            }
            if (selected == null && aItem != null)
            {
                iSelectorList.SelectedItem = null;
                selected = CreateViewModel(aItem);
            }
            iSelectorDisplay.DataContext = selected;
            OnEventNotifyConsumersSelectionChanged(aItem);
        }

        public void ScrollToSelectedItem()
        {

            iSelectorList.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (iSelectorList.SelectedItem != null)
                {
                    iSelectorList.UpdateLayout();
                    try
                    {
                        iSelectorList.ScrollIntoView(iSelectorList.SelectedItem);
                    }
                    catch (NullReferenceException) { } //bug in virtualizing stack panel, ignore
                }
            }), DispatcherPriority.Background);
        }

        public event EventHandler<EventArgsSelection<T>> EventSelectionChanged;

        #endregion

        #region IComparer Members

        public int Compare(object x, object y)
        {
            return iItems.IndexOf(x as ListViewModelBase).CompareTo(iItems.IndexOf(y as ListViewModelBase));
        }

        #endregion

        public abstract ListViewModelBase CreateViewModel(T aItem);

        protected abstract bool CompareWrappedItem(ListViewModelBase aViewModel, object aComparisonItem);

        public T SelectedItem
        {
            get
            {
                return iSelectedItem as T;
            }
        }

        public event EventHandler<EventArgsSelection<T>> EventNotifyConsumersSelectionChanged;
        protected void OnEventNotifyConsumersSelectionChanged(T aSelectedItem)
        {
            if (EventNotifyConsumersSelectionChanged != null)
            {
                EventNotifyConsumersSelectionChanged(this, new EventArgsSelection<T>(aSelectedItem));
            }
        }

        public event EventHandler<EventArgsSelection<T>> EventUserSelectedItem;
        protected void OnEventUserSelectedItem(T aSelectedItem)
        {
            if (EventUserSelectedItem != null)
            {
                EventUserSelectedItem(this, new EventArgsSelection<T>(aSelectedItem));
            }
        }

        void iSelectorList_PreviewMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (iMouseSelectedItem != null)
            {
                OnEventSelectionChanged(iMouseSelectedItem);
                OnEventUserSelectedItem(iMouseSelectedItem);
                if (iMouseSelectedItem == iSelectedItem)
                {
                    // force notify consumers if item is reselected as eventing will not pick up a change
                    OnEventNotifyConsumersSelectionChanged(iMouseSelectedItem);
                }
            }
        }

        void iSelectorList_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            ListViewModel<T> item = iSelectorList.GetEventSourceItem<ListViewModel<T>, ListBoxItem>(e);
            iMouseSelectedItem = null;
            // only want to stop propagation of routed event if the original source was not in a button
            // otherwise button click event will not happen on a button that is contained within a selected list item
            Button b = (e.OriginalSource as DependencyObject).FindVisualAncestor<Button>();
            if (b == null)
            {

                if (item != null)
                {
                    iMouseSelectedItem = item.WrappedItem;
                    e.Handled = true;
                }
            }
        }
        private T iMouseSelectedItem;
    }

    class ViewWidgetSelectorRoom : ViewWidgetSelector<Linn.Kinsky.Room>
    {

        public ViewWidgetSelectorRoom(ListBox aSelectorList, Control aSelectorDisplay, Button aStandbyAllButton)
            : base(aSelectorList, aSelectorDisplay)
        {
            iStandbyAllButton = aStandbyAllButton;
            iStandbyAllButton.IsEnabled = false;
            aStandbyAllButton.Click += StandbyAllButtonClickHandler;
        }

        public override void InsertItem(int aIndex, Linn.Kinsky.Room aItem)
        {
            base.InsertItem(aIndex, aItem);
            EvaluateStandbyAllButtonState();
        }

        public override void RemoveItem(Linn.Kinsky.Room aItem)
        {
            base.RemoveItem(aItem);
            EvaluateStandbyAllButtonState();
        }

        private void StandbyAllButtonClickHandler(object sender, EventArgs e)
        {
            foreach (RoomViewModel vm in iItems)
            {
                if (!vm.WrappedItem.Standby)
                {
                    vm.WrappedItem.Standby = true;
                }
            }
        }

        public override ListViewModelBase CreateViewModel(Linn.Kinsky.Room aItem)
        {
            return new RoomViewModel(aItem.Name, aItem == iSelectedItem, aItem);
        }

        protected override bool CompareWrappedItem(ListViewModelBase aViewModel, object aComparisonItem)
        {
            return (aViewModel as RoomViewModel).WrappedItem == aComparisonItem;
        }

        public override void ItemChanged(Linn.Kinsky.Room aTag)
        {
            RoomViewModel foundItem = null;
            foreach (RoomViewModel item in iItems)
            {
                if (CompareWrappedItem(item, aTag))
                {
                    foundItem = item;
                    break;
                }
            }
            if (foundItem != null)
            {
                foundItem.WrappedItem = aTag;
                int idx = iItems.IndexOf(foundItem);
                if (idx >= 0)
                {
                    iItems[idx] = foundItem;
                }
            }
            EvaluateStandbyAllButtonState();
        }

        private void EvaluateStandbyAllButtonState()
        {
            iStandbyAllButton.IsEnabled = (from RoomViewModel vm in iItems where vm.WrappedItem.Standby == false select vm).Count() > 0;
        }
        private Button iStandbyAllButton;
    }

    class ViewWidgetSelectorSource : ViewWidgetSelector<Linn.Kinsky.Source>
    {
        public ViewWidgetSelectorSource(ListBox aSelectorList, Control aSelectorDisplay)
            : base(aSelectorList, aSelectorDisplay)
        {
        }

        public override ListViewModelBase CreateViewModel(Linn.Kinsky.Source aItem)
        {
            return new SourceViewModel(aItem.Name, aItem == iSelectedItem, aItem);
        }

        protected override bool CompareWrappedItem(ListViewModelBase aViewModel, object aComparisonItem)
        {
            return (aViewModel as SourceViewModel).WrappedItem.Equals(aComparisonItem);
        }

        public override void ItemChanged(Linn.Kinsky.Source aItem)
        {

            SourceViewModel foundItem = null;
            foreach (SourceViewModel item in iItems)
            {
                if (CompareWrappedItem(item, aItem))
                {
                    foundItem = item;
                    break;
                }
            }
            if (foundItem != null)
            {
                foundItem.Name = aItem.Name;
            }
        }
    }

    class ViewWidgetButtonStandby : IViewWidgetButton
    {
        private object iLockObject;
        private bool iOpen;
        public ViewWidgetButtonStandby(ViewKinsky aViewKinsky)
        {
            iLockObject = new object();
        }

        #region IViewWidgetButton Members

        void IViewWidgetButton.Open()
        {
            lock (iLockObject)
            {
                iOpen = true;
            }
        }

        void IViewWidgetButton.Close()
        {
            lock (iLockObject)
            {
                iOpen = false;
            }
        }


        public void OnEventClick()
        {
            bool open;
            lock (iLockObject)
            {
                open = iOpen;
            }
            if (EventClick != null && open)
            {
                EventClick(this, new EventArgs());
            }
        }
        public event EventHandler<EventArgs> EventClick;

        #endregion
    }

}