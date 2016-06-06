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
using System.Windows.Controls.Primitives;

namespace KinskyDesktopWpf
{
    public partial class PlaylistWidgetTimeline : UserControl, IPlaylistWidget
    {
        public event EventHandler<PlaylistSelectionEventArgs> PlaylistSelectionChanged;
        public event EventHandler<PlaylistDropEventArgs> PlaylistItemDropped;
        public event EventHandler<PlaylistDeleteEventArgs> PlaylistItemsDeleted;
        public event EventHandler<PlaylistMoveEventArgs> PlaylistMoveUp;
        public event EventHandler<PlaylistMoveEventArgs> PlaylistMoveDown;
        public event EventHandler<EventArgs> PlaylistSave;

        private IPlaylistItem iNowPlayingItem;

        private IPlaylistItem iRightMouseSelectedItem;
        private ScrollViewer iScrollViewer;

        private bool iDoubleClick;
        private bool iLeftButtonDown;


        private Point iMouseDragStartPoint;
        private DateTime iMouseDownTime;

        private const double kDeceleration = 980;
        private const double kTimeFactor = 3000;
        private const double kDistanceFactor = 2000;
        private const double kMaxVelocity = 1;
        private const double kMaxScrollTime = 1;

        private DateTime iLastUserIntervention = DateTime.MinValue;
        private const int kLastUserInterventionTimeoutSeconds = 30;

        private static string kImageHeightResourceKey = "ImageHeight";
        private static string kTextHeightResourceKey = "TextHeight";

        private DateTime iLastMouseWheelEvent;

        public PlaylistWidgetTimeline()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(PlaylistWidgetTimeline_SizeChanged);
            lstPlaylist.Loaded += lstPlaylist_Loaded;
            SetSize();
        }

        public bool IsSaveEnabled
        {
            get
            {
                return this.IsVisible;
            }
        }

        void PlaylistWidgetTimeline_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            Resources[kImageHeightResourceKey] = this.ActualHeight / 36;
            Resources[kTextHeightResourceKey] = this.ActualHeight / 3;
        }

        void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            iDoubleClick = false;
            iLeftButtonDown = true;
            iMouseDragStartPoint = e.GetPosition(this);
            DateTime previousMouseDownTime = iMouseDownTime;
            iMouseDownTime = DateTime.Now;

            IPlaylistItem selected = GetEventSourceItem(e);
            if (selected != null)
            {
                if (iMouseDownTime.Subtract(previousMouseDownTime) < TimeSpan.FromMilliseconds(500))
                {
                    iDoubleClick = true;
                    if (PlaylistSelectionChanged != null)
                    {
                        iLastUserIntervention = DateTime.MinValue;
                        PlaylistSelectionChanged(this, new PlaylistSelectionEventArgs(selected));
                    }
                }
                else
                {
                    DispatcherTimer t = new DispatcherTimer();
                    t.Interval = TimeSpan.FromSeconds(0.5);
                    t.Tick += ((d, a) =>
                    {
                        if (!iDoubleClick && !iLeftButtonDown)
                        {
                            iLastUserIntervention = DateTime.Now;
                            AnimateToPosition(selected.Position, true, AnimationExtensions.kUIElementAnimationDuration.TimeSpan, 0.0);
                        }
                        t.Stop();
                    });
                    t.Start();
                }
            }
            e.Handled = true;
        }

        void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            iLeftButtonDown = false;
            KineticScroll(iMouseDragStartPoint.X, e.GetPosition(this).X, iMouseDownTime, DateTime.Now);
            e.Handled = true;
        }

        void ListView_PreviewRightMouseButtonDown(object sender, MouseButtonEventArgs args)
        {
            iRightMouseSelectedItem = GetEventSourceItem(args);
        }

        private int iMouseWheelDelta = 0;
        void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            iLastUserIntervention = DateTime.Now;
            iLastMouseWheelEvent = DateTime.Now;
            iMouseWheelDelta = args.Delta < 0 ? iMouseWheelDelta - 1 : iMouseWheelDelta + 1;
            DateTime tmp = iLastMouseWheelEvent;

            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(0.1);

            t.Tick += ((d, a) =>
            {
                if (tmp == iLastMouseWheelEvent)
                {
                    double offset = Math.Round(ScrollOffset) + iMouseWheelDelta;
                    if (offset > lstPlaylist.Items.Count - 1)
                    {
                        offset = lstPlaylist.Items.Count - 1;
                    }
                    else if (offset < 0)
                    {
                        offset = 0;
                    }
                    AnimateToPosition(offset, true, AnimationExtensions.kUIElementAnimationDuration.TimeSpan, 0.0);
                    iMouseWheelDelta = 0;
                }
                t.Stop();
            });
            t.Start();


        }

        private void KineticScroll(double startX, double endX, DateTime startTime, DateTime endTime)
        {
            double timeScrolled = endTime.Subtract(startTime).TotalSeconds;
            double distanceScrolled = Math.Max(Math.Abs(endX - startX), 0) / SystemParameters.WorkArea.Width;

            double velocity = distanceScrolled / timeScrolled;
            velocity = Math.Min(kMaxVelocity, velocity);
            double itemCount = lstPlaylist.Items.Count;
            double timeToScroll = (velocity / kDeceleration) * kTimeFactor;
            if (timeToScroll > kMaxScrollTime)
            {
                timeToScroll = kMaxScrollTime;
            }
            double distanceToScroll = ((velocity * velocity) / (2 * kDeceleration)) * kDistanceFactor * itemCount;
            //UserLog.WriteLine("Velocity: " + velocity);
            //UserLog.WriteLine("Time: " + timeToScroll);
            //UserLog.WriteLine("Distance: " + distanceToScroll);


            if (endX > startX)
            {
                distanceToScroll *= -1;
            }
            double toValue = ScrollOffset + distanceToScroll;
            if (toValue >= lstPlaylist.Items.Count)
            {
                toValue = lstPlaylist.Items.Count - 1;
            }
            else if (toValue < 0)
            {
                toValue = 0;
            }
            AnimateToPosition(Math.Round(toValue), true, TimeSpan.FromSeconds(timeToScroll), 1.0);            
        }


        public void ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Image img = e.OriginalSource as Image;
            ListViewItem item = img.FindVisualAncestor<ListViewItem>();
            PlaylistItemBase playlistItem = (lstPlaylist.ItemContainerGenerator.ItemFromContainer(item) as PlaylistItemBase);
            if (playlistItem != null)
            {
                UserLog.WriteLine(string.Format("{0}:{1}", e.ErrorException.Message, playlistItem.ImageSource));
                playlistItem.ImageSource = KinskyDesktopWpf.Resources.ImageSourceIconAlbumError;
                img.SetBinding(Image.SourceProperty, "ImageSource");
            }
            e.Handled = true;
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

        public bool IsRadioView { get; set; }

        public List<IPlaylistItem> Items
        {
            set
            {
                ObservableCollection<IPlaylistItem> items = new ObservableCollection<IPlaylistItem>();
                for (int i = 0; i < value.Count; i++)
                {
                    value[i].Position = i;
                    items.Add(value[i]);
                }
                lstPlaylist.ItemsSource = value;
                Scroller.Maximum = value.Count - 1;
                UpdateDetailsPanel();
            }
        }

        void lstPlaylist_Loaded(object sender, RoutedEventArgs e)
        {
            SetScrollViewer();
        }

        void SetScrollViewer()
        {
            if (iScrollViewer == null)
            {
                iScrollViewer = lstPlaylist.FindVisualChild<ScrollViewer>();
                if (iScrollViewer != null)
                {
                    iScrollViewer.ScrollChanged += iScrollViewer_ScrollChanged;
                }
            }
        }

        void iScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateDetailsPanel();
        }

        private void UpdateDetailsPanel()
        {
            SetScrollViewer();
            IPlaylistItem item = lstPlaylist.Items.CurrentItem as IPlaylistItem;

            UIElement listItem = lstPlaylist.ItemContainerGenerator.ContainerFromItem(item) as UIElement;

            if (listItem != null)
            {
                Size size = listItem.RenderSize;
                Point topCenter = listItem.TransformToAncestor((Visual)this).Transform(new System.Windows.Point(size.Width / 2, 0));

                double left = Math.Max(0, Math.Min(cnvDetails.ActualWidth - pnlDetails.ActualWidth, topCenter.X - pnlDetails.ActualWidth / 2));

                pnlDetails.SetValue(Canvas.LeftProperty, left);
                pnlDetails.SetValue(Canvas.TopProperty, 0.0);
                pnlDetails.DataContext = item;
            }
            else
            {
                pnlDetails.DataContext = null;
            }
        }

        private void Scroller_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            iLastUserIntervention = DateTime.Now;
            AnimateToPosition(Math.Round(e.NewValue), false, AnimationExtensions.kUIElementAnimationDuration.TimeSpan, 0.0);
        }

        public void AnimateToPosition(double position, bool updateScroller, TimeSpan aTimeSpan, double aDecelarationRatio)
        {
            Storyboard storyBoard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(position, new Duration(aTimeSpan));
            animation.DecelerationRatio = aDecelarationRatio;
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("ScrollOffset"));
            storyBoard.Children.Add(animation);

            EventHandler handler = null;
            handler = (d, e) =>
            {
                ScrollOffset = position;
                UpdateDetailsPanel();
                if (updateScroller)
                {
                    Scroller.Value = position;
                }
                storyBoard.Completed -= handler;
                storyBoard.Remove();
            };
            storyBoard.Completed += handler;

            storyBoard.Begin();
            if (updateScroller)
            {
                Storyboard storyBoard2 = new Storyboard();
                DoubleAnimation scrollerAnimation = new DoubleAnimation(position, new Duration(aTimeSpan), FillBehavior.Stop);
                scrollerAnimation.DecelerationRatio = aDecelarationRatio;
                Storyboard.SetTarget(scrollerAnimation, Scroller);
                Storyboard.SetTargetProperty(scrollerAnimation, new PropertyPath("Value"));
                storyBoard2.Children.Add(scrollerAnimation);
                storyBoard2.Begin();
            }
        }

        public static readonly DependencyProperty ScrollOffsetProperty =
   DependencyProperty.Register("ScrollOffset", typeof(double), typeof(PlaylistWidgetTimeline),
   new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnScrollOffsetChanged)));


        public double ScrollOffset
        {
            get { return (double)GetValue(ScrollOffsetProperty); }
            set { SetValue(ScrollOffsetProperty, value); }
        }

        private static void OnScrollOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PlaylistWidgetTimeline me = obj as PlaylistWidgetTimeline;

            if (me != null)
            {
                int itemPosition = Math.Max(0,Math.Min(me.lstPlaylist.Items.Count - 1,(int)Math.Round((double)args.NewValue)));
                if (itemPosition < me.lstPlaylist.Items.Count)
                {
                    me.lstPlaylist.Items.MoveCurrentToPosition(itemPosition);
                    PlaylistItemBase item = me.lstPlaylist.Items[itemPosition] as PlaylistItemBase;
                    if (item != null)
                    {
                        me.lstPlaylist.ScrollToCenterOfView(item);
                    }
                }
                me.UpdateDetailsPanel();
            }
        }

        public List<IPlaylistItem> SelectedItems()
        {
            List<IPlaylistItem> items = new List<IPlaylistItem>();
            foreach (IPlaylistItem item in lstPlaylist.SelectedItems)
            {
                items.Add(item);
            }
            return items;
        }

        public void SetNowPlayingItem(IPlaylistItem aSelectedItem)
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
                            if (aSelectedItem != null && items[i].WrappedItem == aSelectedItem.WrappedItem)
                            {
                                items[i].IsPlaying = true;
                                iNowPlayingItem = items[i];
                                if (iLastUserIntervention.AddSeconds(kLastUserInterventionTimeoutSeconds) < DateTime.Now)
                                {
                                    AnimateToPosition(items[i].Position, true, AnimationExtensions.kUIElementAnimationDuration.TimeSpan, 0.0);
                                }
                            }
                            else
                            {
                                if (items[i].IsPlaying){
                                    var item = items[i];
                                    DispatcherTimer t = new DispatcherTimer();
                                    t.Interval = TimeSpan.FromSeconds(0.5);
                                    t.Tick += ((d, a) =>
                                    {
                                        item.IsPlaying = false;
                                        t.Stop();                                        
                                    });
                                    t.Start();
                                }
                            }
                        }
                    }
                }
            }
        }


        private IPlaylistItem GetEventSourceItem(RoutedEventArgs args)
        {
            return lstPlaylist.GetEventSourceItem<IPlaylistItem, ListBoxItem>(args);
        }

        #region Command Bindings

        private void PlayCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DidlLite didl = new DidlLite();

            e.CanExecute = iRightMouseSelectedItem != null && PlaylistSelectionChanged != null;
            e.Handled = true;
        }

        private void PlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (iRightMouseSelectedItem != null && PlaylistSelectionChanged != null)
            {
                PlaylistSelectionChanged(this, new PlaylistSelectionEventArgs(iRightMouseSelectedItem));
            }
        }

        private void MoveUpCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = iRightMouseSelectedItem != null
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
        private void ScrollToCurrentCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            IPlaylistItem currentPlaying = lstPlaylist.Items[(int)Math.Round(ScrollOffset)] as IPlaylistItem;
            e.CanExecute = iNowPlayingItem != null
                && currentPlaying != null
                && !(currentPlaying).IsPlaying;
            e.Handled = true;
        }

        private void ScrollToCurrentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (iNowPlayingItem != null)
            {
                AnimateToPosition(iNowPlayingItem.Position, true, AnimationExtensions.kUIElementAnimationDuration.TimeSpan, 0.0);
            }
        }
        private void DetailsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = iRightMouseSelectedItem != null;
            e.Handled = true;
        }

        private void DetailsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistItemBase selected = iRightMouseSelectedItem as PlaylistItemBase;
            if (selected != null)
            {
                DetailsDialog details = new DetailsDialog(selected.WrappedItem.DidlLite[0]);
                details.Owner = Window.GetWindow(this);
                details.ShowDialog();
            }
        }


        #endregion
    }
}
