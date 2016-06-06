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
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace KinskyDesktopWpf
{

    public partial class PlaylistWidget : UserControl, IPlaylistWidget
    {

        private DropConverter iDropConverter;
        public event EventHandler<PlaylistSelectionEventArgs> PlaylistSelectionChanged;
        public event EventHandler<PlaylistSelectionEventArgs> EventPlaylistItemNavigationClick;
        public event EventHandler<PlaylistDropEventArgs> PlaylistItemsAdded;
        public event EventHandler<PlaylistDropEventArgs> PlaylistItemsMoved;
        public event EventHandler<PlaylistDeleteEventArgs> PlaylistItemsDeleted;
        public event EventHandler<PlaylistMoveEventArgs> PlaylistMoveUp;
        public event EventHandler<PlaylistMoveEventArgs> PlaylistMoveDown;
        public event EventHandler<EventArgs> PlaylistSave;

        private IPlaylistItem iNowPlayingItem;
        private DragHelper iDragHelper;
        private IPlaylistSupport iSupport;

        private IPlaylistItem iRightMouseSelectedItem;

        private const int kMinItemsForVirtualization = 50;
        private UiOptions iUiOptions;

        public PlaylistWidget(DropConverter aDropConverter, IPlaylistSupport aSupport, UiOptions aUiOptions)
        {
            InitializeComponent();
            iUiOptions = aUiOptions;
            iDropConverter = aDropConverter;
            iDragHelper = new DragHelper(lstPlaylist);
            iDragHelper.EventDragInitiated += new EventHandler<MouseEventArgs>(iDragHelper_EventDragInitiated);

            lstPlaylist.SelectionMode = SelectionMode.Extended;
            iSupport = aSupport;
            DragScroller scroller = new DragScroller(lstPlaylist, delegate(DragEventArgs e)
            {
                return GetEffects(e);
            });
            scroller.ItemsDropped += new EventHandler<DragScroller.EventArgsItemsDropped>(Scroller_ItemsDropped);
        }

        void Scroller_ItemsDropped(object sender, DragScroller.EventArgsItemsDropped e)
        {
            ListBoxItem item = lstPlaylist.GetEventSourceElement<ListBoxItem>(e.DragEventArgs);
            int dropIndex = 0;
            if (item != null)
            {
                Point p = e.DragEventArgs.GetPosition(item);
                bool top = p.Y < item.ActualHeight / 2;
                IPlaylistItem containedItem = GetEventSourceItem(e.DragEventArgs);
                dropIndex = top ? containedItem.Position : containedItem.Position + 1;
            }
            else if (lstPlaylist.Items.Count > 0)
            {
                Point p = e.DragEventArgs.GetPosition(lstPlaylist);
                bool top = p.Y < 10;
                dropIndex = top ? 0 : (lstPlaylist.Items[lstPlaylist.Items.Count - 1] as PlaylistListItem).Position + 1;
            }
            MediaProviderDraggable draggable = iDropConverter.Convert(e.DragEventArgs.Data);
            PlaylistDropEventArgs eventArgs = new PlaylistDropEventArgs();
            eventArgs.Data = draggable;
            eventArgs.DropIndex = dropIndex;

            if (draggable != null && draggable.DragSource == this && PlaylistItemsMoved != null)
            {
                PlaylistItemsMoved(this, eventArgs);
            }
            else
            {
                if (draggable != null && PlaylistItemsAdded != null)
                {
                    PlaylistItemsAdded(this, eventArgs);
                }
            }
        }

        void iDragHelper_EventDragInitiated(object sender, MouseEventArgs args)
        {
            List<upnpObject> tracks = new List<upnpObject>();
            List<MrItem> deletedTracks = new List<MrItem>();
            foreach (IPlaylistItem item in SelectedItems())
            {
                tracks.Add(item.WrappedItem.DidlLite[0]);
                deletedTracks.Add(item.WrappedItem);
            }

            if (tracks.Count > 0)
            {
                MediaProviderDraggable draggable = new MediaProviderDraggable(new MediaRetrieverNoRetrieve(tracks), this);

                DataObject data = new DataObject();
                data.SetData(draggable);
                ListBoxItem item = lstPlaylist.GetEventSourceElement<ListBoxItem>(args);

                DragDropEffects result = iDragHelper.DoDragDrop(item, data, DragDropEffects.Copy | DragDropEffects.Move, GetDragVisual(args));

                if (result == DragDropEffects.Move && PlaylistItemsDeleted != null)
                {
                    PlaylistItemsDeleted(this, new PlaylistDeleteEventArgs(SelectedItems()));
                }
            }
        }

        public void SetLoading(bool aLoading, bool aIsInserting)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (!aIsInserting)
                {
                    pnlProgress.Visibility = aLoading ? Visibility.Visible : Visibility.Collapsed;
                    lstPlaylist.Visibility = aLoading ? Visibility.Collapsed : Visibility.Visible;
                    progressBar.IsAnimating = aLoading;
                }
                if (aLoading)
                {
                    lstPlaylist.Cursor = Cursors.Wait;
                }
                else
                {
                    lstPlaylist.ClearValue(Control.CursorProperty);
                }
            }));
        }

        public bool GroupByAlbum
        {
            get { return (bool)GetValue(GroupByAlbumProperty); }
            set { SetValue(GroupByAlbumProperty, value); }
        }

        public static readonly DependencyProperty GroupByAlbumProperty =
            DependencyProperty.Register("GroupByAlbum", typeof(bool), typeof(PlaylistWidget), new UIPropertyMetadata(false));



        public bool IsSaveEnabled
        {
            get
            {
                return this.IsVisible;
            }
        }

        void ListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IPlaylistItem selected = GetEventSourceItem(e);
                if (PlaylistSelectionChanged != null && selected != null)
                {
                    if (selected is PlaylistGroupHeaderItem)
                    {
                        selected = (IPlaylistItem)lstPlaylist.Items[lstPlaylist.Items.IndexOf(selected) + 1];
                    }
                    PlaylistSelectionChanged(this, new PlaylistSelectionEventArgs(selected));
                }
            }
        }

        void ListBox_PreviewRightMouseButtonDown(object sender, MouseButtonEventArgs args)
        {
            iRightMouseSelectedItem = GetEventSourceItem(args);
        }

        void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem[] items = lstPlaylist.FindVisualChildren<ListBoxItem>();
            foreach (ListBoxItem item in items)
            {
                TextBlock[] textblocks = item.FindVisualChildren<TextBlock>();
                foreach (TextBlock t in textblocks)
                {
                    t.ClearValue(ForegroundProperty);
                }
            }
            List<IPlaylistItem> addList = new List<IPlaylistItem>();
            List<IPlaylistItem> removeList = new List<IPlaylistItem>();
            foreach (IPlaylistItem item in e.AddedItems)
            {
                if (item is PlaylistGroupHeaderItem)
                {
                    DependencyObject container = (DependencyObject)lstPlaylist.ItemContainerGenerator.ContainerFromItem(item);
                    if (container != null)
                    {
                        int index = lstPlaylist.ItemContainerGenerator.IndexFromContainer(container);
                        for (int i = index + 1; i < lstPlaylist.Items.Count
                            && !(lstPlaylist.Items[i] is PlaylistGroupHeaderItem) && !(lstPlaylist.Items[i] is CollapsedPlaylistListItem); i++)
                        {
                            if (!e.AddedItems.Contains(lstPlaylist.Items[i]))
                            {
                                addList.Add((IPlaylistItem)lstPlaylist.Items[i]);
                            }
                        }
                    }
                }
            }
            if (e.RemovedItems.Count == 1 && e.RemovedItems[0] is PlaylistGroupHeaderItem)
            {
                DependencyObject container = (DependencyObject)lstPlaylist.ItemContainerGenerator.ContainerFromItem(e.RemovedItems[0]);
                if (container != null)
                {
                    int index = lstPlaylist.ItemContainerGenerator.IndexFromContainer(container);
                    for (int i = index + 1; i < lstPlaylist.Items.Count
                        && !(lstPlaylist.Items[i] is PlaylistGroupHeaderItem) && !(lstPlaylist.Items[i] is CollapsedPlaylistListItem); i++)
                    {
                        if (lstPlaylist.SelectedItems.Contains(lstPlaylist.Items[i])
                            && !e.RemovedItems.Contains(lstPlaylist.Items[i]))
                        {
                            removeList.Add((IPlaylistItem)lstPlaylist.Items[i]);
                        }
                    }
                }
            }
            foreach (IPlaylistItem item in addList)
            {
                lstPlaylist.SelectedItems.Add(item);
            }
            foreach (IPlaylistItem item in removeList)
            {
                lstPlaylist.SelectedItems.Remove(item);
            }
            if (lstPlaylist.SelectedItems.Count == 1 && lstPlaylist.SelectedItem is PlaylistGroupHeaderItem)
            {
                lstPlaylist.SelectedItems.Clear();
            }
            e.Handled = true;
        }

        private IPlaylistItem GetEventSourceItem(RoutedEventArgs args)
        {
            return lstPlaylist.GetEventSourceItem<IPlaylistItem, ListBoxItem>(args);
        }

        public List<IPlaylistItem> Items
        {
            set
            {
                lstPlaylist.SetValue(ScrollViewer.CanContentScrollProperty, value.Count > kMinItemsForVirtualization);
                lstPlaylist.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, value.Count > kMinItemsForVirtualization);
                lstPlaylist.ItemsSource = value;
            }
        }

        public List<IPlaylistItem> SelectedItems()
        {
            List<IPlaylistItem> items = new List<IPlaylistItem>();
            foreach (IPlaylistItem item in lstPlaylist.SelectedItems)
            {
                if (!(item is PlaylistGroupHeaderItem))
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public void SetNowPlayingItem(IPlaylistItem aSelectedItem, bool aScrollToSelected)
        {
            if (lstPlaylist.ItemsSource != null)
            {
                List<IPlaylistItem> items = lstPlaylist.ItemsSource as List<IPlaylistItem>;
                if (items != null)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] != null)
                        {
                            if (aSelectedItem != null && items[i].WrappedItem == aSelectedItem.WrappedItem && !(items[i] is PlaylistGroupHeaderItem))
                            {
                                if (aScrollToSelected)
                                {
                                    lstPlaylist.UpdateLayout();
                                    try
                                    {
                                        lstPlaylist.ScrollIntoView(items[i]);
                                    }
                                    catch (NullReferenceException) { } //bug in virtualizing stack panel, ignore
                                }
                                iNowPlayingItem = items[i];
                                items[i].IsPlaying = true;
                                if (items[i] is SenderListItem && aSelectedItem is SenderListItem)
                                {
                                    (items[i] as SenderListItem).HasRoom = (aSelectedItem as SenderListItem).HasRoom;
                                }
                            }
                            else
                            {
                                items[i].IsPlaying = false;
                            }
                        }
                    }
                }
            }
        }

        public void ScrollToNowPlaying()
        {
            if (iNowPlayingItem != null)
            {
                lstPlaylist.UpdateLayout();
                try
                {
                    lstPlaylist.ScrollIntoView(iNowPlayingItem);
                }
                catch (NullReferenceException) { } //bug in virtualizing stack panel, ignore
            }
        }

        private DragDropEffects GetEffects(DragEventArgs args)
        {

            MediaProviderDraggable draggable = iDropConverter.Convert(args.Data);
            DragDropEffects result = DragDropEffects.None;
            if (draggable != null && AllowDrop)
            {

                // only perform a move operation if we are dragging onto the playlist from the playlist
                if ((args.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move && (draggable.DragSource == this))
                {
                    result = DragDropEffects.Move;
                }
                else
                {
                    if ((args.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {
                        result = DragDropEffects.Copy;
                    }
                    else if ((args.AllowedEffects & DragDropEffects.Link) == DragDropEffects.Link)
                    {
                        result = DragDropEffects.Link;
                    }
                }
                if (result != DragDropEffects.None)
                {
                    foreach (upnpObject o in draggable.DragMedia)
                    {
                        if (o.Res.Count > 0)
                        {
                            if (System.IO.Path.GetExtension(o.Res[0].Uri) == PluginManager.kPluginExtension)
                            {
                                result = DragDropEffects.None;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private FrameworkElement GetDragVisual(MouseEventArgs args)
        {
            IPlaylistItem dragItem = GetEventSourceItem(args);
            if (dragItem != null)
            {
                Image img = new Image();
                img.Height = DragHelper.kDefaultVisualHeight;
                img.SetValue(Image.SourceProperty, StaticImages.ImageSourceIconLoading);
                KinskyDesktop.Instance.ImageCache.Load(KinskyDesktop.Instance.IconResolver.Resolve(dragItem.WrappedItem), (s) =>
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

        #region Command Bindings

        private void PlayCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DidlLite didl = new DidlLite();

            e.CanExecute = iRightMouseSelectedItem != null && !(iRightMouseSelectedItem is PlaylistGroupHeaderItem) && PlaylistSelectionChanged != null;
            e.Handled = true;
        }

        private void PlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (iRightMouseSelectedItem != null && !(iRightMouseSelectedItem is PlaylistGroupHeaderItem) && PlaylistSelectionChanged != null)
            {
                PlaylistSelectionChanged(this, new PlaylistSelectionEventArgs(iRightMouseSelectedItem));
            }
        }

        private void MoveUpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = iRightMouseSelectedItem != null
                            && !(iRightMouseSelectedItem is PlaylistGroupHeaderItem)
                            && iRightMouseSelectedItem.Position > 0
                            && PlaylistMoveUp != null;
            e.Handled = true;
        }

        private void MoveUpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (PlaylistMoveUp != null)
            {
                PlaylistMoveUp(this, new PlaylistMoveEventArgs(iRightMouseSelectedItem));
            }
        }

        private void MoveDownCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = iRightMouseSelectedItem != null
                            && !(iRightMouseSelectedItem is PlaylistGroupHeaderItem)
                            && lstPlaylist.Items.Count > 0
                            && lstPlaylist.Items[lstPlaylist.Items.Count - 1] != iRightMouseSelectedItem
                            && PlaylistMoveDown != null;
            e.Handled = true;
        }

        private void MoveDownExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (PlaylistMoveDown != null)
            {
                PlaylistMoveDown(this, new PlaylistMoveEventArgs(iRightMouseSelectedItem));
            }
        }

        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lstPlaylist.Items.Count > 0;
            e.Handled = true;
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (PlaylistSave != null)
            {
                PlaylistSave(this, EventArgs.Empty);
            }
        }

        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItems().Count > 0;
            e.Handled = true;
        }

        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (PlaylistItemsDeleted != null)
            {
                PlaylistItemsDeleted(this, new PlaylistDeleteEventArgs(SelectedItems()));
            }
        }


        private void DetailsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = iRightMouseSelectedItem != null
                            && !(iRightMouseSelectedItem is PlaylistGroupHeaderItem);
            e.Handled = true;
        }

        private void DetailsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistItemBase selected = iRightMouseSelectedItem as PlaylistItemBase;
            if (selected != null && !(selected is PlaylistGroupHeaderItem))
            {
                DetailsDialog details = new DetailsDialog(selected.WrappedItem.DidlLite[0], null, iUiOptions);
                details.Owner = Window.GetWindow(this);
                details.ShowDialog();
            }
        }


        #endregion

        private void lstPlaylist_KeyUp(object sender, KeyEventArgs e)
        {
            List<IPlaylistItem> selected = SelectedItems();
            bool isCtrl = ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
            if (e.Key == Key.Delete && !isCtrl && PlaylistItemsDeleted != null && selected.Count > 0)
            {
                PlaylistItemsDeleted(this, new PlaylistDeleteEventArgs(selected));
            }
            if (e.Key == Key.Enter && selected.Count > 0)
            {
                PlaylistSelectionChanged(this, new PlaylistSelectionEventArgs(selected[0]));
            }
        }

        private void ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = e.OriginalSource as Image;
            ListViewItem item = img.FindVisualAncestor<ListViewItem>();
            PlaylistItemBase playlistItem = (lstPlaylist.ItemContainerGenerator.ItemFromContainer(item) as PlaylistItemBase);
            if (playlistItem != null)
            {
                UserLog.WriteLine(string.Format("{0}:{1}", e.ErrorException.Message, playlistItem.ImageSource));
                playlistItem.ImageSource = KinskyDesktopWpf.StaticImages.ImageSourceIconAlbumError;
                img.SetBinding(Image.SourceProperty, "ImageSource");
            }
            e.Handled = true;
        }

        private void SenderButton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject source = (sender as DependencyObject).FindVisualAncestor<ListViewItem>();
            PlaylistItemBase playlistItem = (lstPlaylist.ItemContainerGenerator.ItemFromContainer(source) as PlaylistItemBase);

            if (EventPlaylistItemNavigationClick != null && playlistItem != null)
            {
                EventPlaylistItemNavigationClick(this, new PlaylistSelectionEventArgs(playlistItem));
            }
        }

    }


}
