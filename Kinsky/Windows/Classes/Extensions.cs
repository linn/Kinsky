
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using Linn;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Linn.Kinsky;
using System.Runtime.InteropServices;

namespace KinskyDesktopWpf
{
    public static class ImageExtensions
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapImage ToBitmapImage(this System.Drawing.Image aImage)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    aImage.Save(ms, aImage.RawFormat);
                    System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                    bImg.BeginInit();
                    bImg.StreamSource = new MemoryStream(ms.ToArray());
                    bImg.EndInit();
                    bImg.Freeze();
                    return bImg;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Trace.kKinskyDesktop, "Error loading image: " + e.Message);
                Trace.WriteLine(Trace.kKinskyDesktop, e.StackTrace);
                return null;
            }
        }
        public static BitmapFrame ToBitmapFrame(this System.Drawing.Image aImage)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                aImage.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                return BitmapFrame.Create(ms);
            }
            catch (Exception e)
            {
                Trace.WriteLine(Trace.kKinskyDesktop, "Error loading image: " + e.Message);
                Trace.WriteLine(Trace.kKinskyDesktop, e.StackTrace);
                return null;
            }
        }

        public static int SizeBytes(this System.Windows.Media.Imaging.BitmapImage aImage)
        {
            if (aImage.StreamSource != null)
            {
                return (int)aImage.StreamSource.Length;
            }
            return (aImage.PixelHeight * aImage.PixelWidth * aImage.Format.BitsPerPixel) / 8;
        }
    }

    public static class DependencyObjectExtensions
    {
        public static childItem FindVisualChild<childItem>(this DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static childItem[] FindVisualChildren<childItem>(this DependencyObject obj) where childItem : DependencyObject
        {
            return obj.FindVisualChildren<childItem>(false);
        }

        public static childItem[] FindVisualChildren<childItem>(this DependencyObject obj, bool aIncludeSelf) where childItem : DependencyObject
        {
            List<childItem> items = new List<childItem>();
            if (aIncludeSelf && obj is childItem)
            {
                items.Add(obj as childItem);
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    items.Add((childItem)child);
                else
                {
                    childItem[] childOfChild = FindVisualChildren<childItem>(child, aIncludeSelf);
                    foreach (childItem c in childOfChild)
                    {
                        items.Add(c);
                    }
                }
            }
            return items.ToArray();
        }

        public static ancestorItem FindVisualAncestor<ancestorItem>(this DependencyObject obj) where ancestorItem : DependencyObject
        {
            DependencyObject dep = obj;
            if (dep is ancestorItem) return dep as ancestorItem;
            do
            {
                dep = VisualTreeHelper.GetParent(dep);
            } while ((dep != null) && !(dep is ancestorItem));
            return dep as ancestorItem;
        }

        public static void DockChild(this Panel aPanel, UIElement aElement)
        {
            if (!aPanel.Children.Contains(aElement))
            {
                Panel parent = aElement.FindVisualAncestor<Panel>();
                if (parent != aPanel)
                {
                    parent.Children.Remove(aElement);
                }
                aPanel.Children.Insert(0, aElement);
            }
        }

    }

    public static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.RegisterAttached(
          "BackgroundOpacity",
          typeof(double),
          typeof(FrameworkElementExtensions),
          new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender)
        );
        public static void SetBackgroundOpacity(FrameworkElement element, double value)
        {
            element.SetValue(BackgroundOpacityProperty, value);
        }
        public static double GetBackgroundOpacity(FrameworkElement element)
        {
            return (double)element.GetValue(BackgroundOpacityProperty);
        }
    }

    public static class TabControlExtensions
    {

        public const int kMaxTabs = 4;

        public static readonly DependencyProperty CanAddTabProperty = DependencyProperty.RegisterAttached(
          "CanAddTab",
          typeof(bool),
          typeof(TabControlExtensions),
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
        );
        public static void SetCanAddTab(TabControl element, bool value)
        {
            element.SetValue(CanAddTabProperty, value);
        }
        public static bool GetCanAddTab(TabControl element)
        {
            return (bool)element.GetValue(CanAddTabProperty);
        }
        public static readonly DependencyProperty CanRemoveTabProperty = DependencyProperty.RegisterAttached(
          "CanRemoveTab",
          typeof(bool),
          typeof(TabControlExtensions),
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
        );
        public static void SetCanRemoveTab(TabControl element, bool value)
        {
            element.SetValue(CanRemoveTabProperty, value);
        }
        public static bool GetCanRemoveTab(TabControl element)
        {
            return (bool)element.GetValue(CanRemoveTabProperty);
        }

        public static void AddTabItemAddHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(TabControlExtensions.TabItemAddEvent, handler);
            }
        }
        public static void RemoveTabItemAddHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(TabControlExtensions.TabItemAddEvent, handler);
            }
        }

        public static readonly RoutedEvent TabItemAddEvent = EventManager.RegisterRoutedEvent(
            "TabItemAdd", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TabControlExtensions));


        public static void AddTabItemRemoveHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.AddHandler(TabControlExtensions.TabItemRemoveEvent, handler);
            }
        }
        public static void RemoveTabItemRemoveHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(TabControlExtensions.TabItemRemoveEvent, handler);
            }
        }

        public static readonly RoutedEvent TabItemRemoveEvent = EventManager.RegisterRoutedEvent(
            "TabItemRemove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TabControlExtensions));

    }

    public static class PopupExtensions
    {
        public static void SetPopupOffset(this Popup aPopup, FrameworkElement aRelativeElement)
        {
            Window containingWindow = aRelativeElement.FindVisualAncestor<Window>();

            System.Windows.Point windowPosition = containingWindow.PointToScreen(new System.Windows.Point(0, 0));
            Rect virtualScreenCoords = new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);

            double desiredPopupWidth = aPopup.Width;
            double desiredPopupHeight = aPopup.Height;
            System.Windows.Point relativeElementTopLeftCorner = aRelativeElement.TranslatePoint(new System.Windows.Point(0, 0), containingWindow);

            Dock desiredHorizontalPlacement = (Dock)aPopup.GetValue(PopupExtensions.DesiredHorizontalPlacementProperty);
            Dock desiredVerticalPlacement = (Dock)aPopup.GetValue(PopupExtensions.DesiredVerticalPlacementProperty);
            Dock actualHorizontalPlacement = desiredHorizontalPlacement;
            Dock actualVerticalPlacement = desiredVerticalPlacement;



            // determine best fit horizontally
            if (desiredHorizontalPlacement == Dock.Right)
            {
                System.Windows.Point popupBottomRight = new System.Windows.Point(relativeElementTopLeftCorner.X + desiredPopupWidth, relativeElementTopLeftCorner.Y + aRelativeElement.ActualHeight + desiredPopupHeight);
                if (popupBottomRight.X > containingWindow.Width || containingWindow.PointToScreen(popupBottomRight).X > virtualScreenCoords.Width - virtualScreenCoords.Left)
                {
                    actualHorizontalPlacement = Dock.Left;
                }
            }
            else
            {
                System.Windows.Point popupBottomLeft = new System.Windows.Point(relativeElementTopLeftCorner.X + aRelativeElement.ActualWidth - desiredPopupWidth, relativeElementTopLeftCorner.Y + aRelativeElement.ActualHeight + desiredPopupHeight);
                if (popupBottomLeft.X < 0 || containingWindow.PointToScreen(popupBottomLeft).X < virtualScreenCoords.Left)
                {
                    actualHorizontalPlacement = Dock.Right;
                }
            }

            // calculate horizontal offset based on actual horizontal placement
            double horizontalOffset = (actualHorizontalPlacement == Dock.Right ? relativeElementTopLeftCorner.X : relativeElementTopLeftCorner.X + aRelativeElement.ActualWidth - desiredPopupWidth);

            // determine best fit vertically
            System.Windows.Point bottomDockLeft = new System.Windows.Point(horizontalOffset, relativeElementTopLeftCorner.Y + aRelativeElement.ActualHeight + desiredPopupHeight);
            System.Windows.Point topDockLeft = new System.Windows.Point(horizontalOffset, relativeElementTopLeftCorner.Y - desiredPopupHeight);
            if (desiredVerticalPlacement == Dock.Bottom)
            {
                if (containingWindow.PointToScreen(bottomDockLeft).Y > virtualScreenCoords.Height - virtualScreenCoords.Left && containingWindow.PointToScreen(topDockLeft).Y > virtualScreenCoords.Top)
                {
                    actualVerticalPlacement = Dock.Top;
                }
            }
            else
            {
                if (containingWindow.PointToScreen(bottomDockLeft).Y < virtualScreenCoords.Height - virtualScreenCoords.Left && containingWindow.PointToScreen(topDockLeft).Y < virtualScreenCoords.Top)
                {
                    actualVerticalPlacement = Dock.Bottom;
                }
            }

            double verticalOffset = (actualVerticalPlacement == Dock.Bottom ? relativeElementTopLeftCorner.Y + aRelativeElement.ActualHeight : relativeElementTopLeftCorner.Y - desiredPopupHeight);

            // setup popup based on actual best placements and resulting co-ordinates relative to containing window
            aPopup.SetValue(PopupExtensions.VerticalPlacementProperty, actualVerticalPlacement);
            aPopup.SetValue(PopupExtensions.HorizontalPlacementProperty, actualHorizontalPlacement);
            aPopup.PlacementTarget = containingWindow;
            aPopup.Placement = PlacementMode.Relative;
            aPopup.HorizontalOffset = horizontalOffset;
            aPopup.VerticalOffset = verticalOffset;

            // adjust maxheight of popup to prevent it being shifted up by wpf
            if (actualVerticalPlacement == Dock.Bottom)
            {
                System.Windows.Point bottomLeftScreenCoords = containingWindow.PointToScreen(new System.Windows.Point(horizontalOffset, verticalOffset + desiredPopupHeight));
                if (bottomLeftScreenCoords.Y > virtualScreenCoords.Height)
                {
                    System.Windows.Point screenBottomScreenCoords = new System.Windows.Point(bottomLeftScreenCoords.X, virtualScreenCoords.Height);
                    System.Windows.Point screenBottomWindowCoords = containingWindow.PointFromScreen(screenBottomScreenCoords);
                    aPopup.MaxHeight = screenBottomWindowCoords.Y - verticalOffset;
                }
                else
                {
                    aPopup.ClearValue(FrameworkElement.MaxHeightProperty);
                }
            }
            else
            {
                System.Windows.Point topLeftScreenCoords = containingWindow.PointToScreen(new System.Windows.Point(horizontalOffset, verticalOffset));
                if (topLeftScreenCoords.Y < virtualScreenCoords.Top)
                {
                    System.Windows.Point screenTopScreenCoords = new System.Windows.Point(topLeftScreenCoords.X, virtualScreenCoords.Top);
                    System.Windows.Point screenTopWindowCoords = containingWindow.PointFromScreen(screenTopScreenCoords);
                    aPopup.MaxHeight = desiredPopupHeight - (screenTopWindowCoords.Y - verticalOffset);
                }
                else
                {
                    aPopup.ClearValue(FrameworkElement.MaxHeightProperty);
                }
            }
        }

        public static readonly DependencyProperty DesiredVerticalPlacementProperty = DependencyProperty.RegisterAttached(
          "DesiredVerticalPlacement",
          typeof(Dock),
          typeof(PopupExtensions),
          new FrameworkPropertyMetadata(Dock.Bottom, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
        );
        public static void SetDesiredVerticalPlacement(TabControl element, Dock value)
        {
            element.SetValue(DesiredVerticalPlacementProperty, value);
        }
        public static Dock GetDesiredVerticalPlacement(TabControl element)
        {
            return (Dock)element.GetValue(DesiredVerticalPlacementProperty);
        }

        public static readonly DependencyProperty DesiredHorizontalPlacementProperty = DependencyProperty.RegisterAttached(
          "DesiredHorizontalPlacement",
          typeof(Dock),
          typeof(PopupExtensions),
          new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
        );
        public static void SetDesiredHorizontalPlacement(TabControl element, Dock value)
        {
            element.SetValue(DesiredHorizontalPlacementProperty, value);
        }
        public static Dock GetDesiredHorizontalPlacement(TabControl element)
        {
            return (Dock)element.GetValue(DesiredHorizontalPlacementProperty);
        }

        public static readonly DependencyProperty HorizontalPlacementProperty = DependencyProperty.RegisterAttached(
         "HorizontalPlacement",
         typeof(Dock),
         typeof(PopupExtensions),
         new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
            );

        public static void SetHorizontalPlacement(TabControl element, Dock value)
        {
            element.SetValue(HorizontalPlacementProperty, value);
        }
        public static Dock GetHorizontalPlacement(TabControl element)
        {
            return (Dock)element.GetValue(HorizontalPlacementProperty);
        }

        public static readonly DependencyProperty VerticalPlacementProperty = DependencyProperty.RegisterAttached(
         "VerticalPlacement",
         typeof(Dock),
         typeof(PopupExtensions),
         new FrameworkPropertyMetadata(Dock.Bottom, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits)
            );

        public static void SetVerticalPlacement(TabControl element, Dock value)
        {
            element.SetValue(VerticalPlacementProperty, value);
        }
        public static Dock GetVerticalPlacement(TabControl element)
        {
            return (Dock)element.GetValue(VerticalPlacementProperty);
        }
    }

    public static class ItemsControlExtensions
    {
        public static ScrollViewer GetScrollViewer(this ItemsControl aItemsControl)
        {
            ScrollViewer result = null;
            DependencyObject current = aItemsControl;
            while (VisualTreeHelper.GetChildrenCount(current) > 0 && result == null)
            {
                current = VisualTreeHelper.GetChild(current, 0);
                result = current as ScrollViewer;
            }
            return result;
        }

        public static void ScrollToCenterOfView(this ItemsControl itemsControl, object item)
        {
            // Scroll immediately if possible
            if (!itemsControl.TryScrollToCenterOfView(item))
            {
                // Otherwise wait until everything is loaded, then scroll
                if (itemsControl is ListBox)
                {
                    ListBox lst = ((ListBox)itemsControl);
                    lst.UpdateLayout();
                    try
                    {
                        lst.ScrollIntoView(item);
                    }
                    catch (NullReferenceException) { } //bug in virtualizing stack panel, ignore
                }
                itemsControl.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    itemsControl.TryScrollToCenterOfView(item);
                }));
            }
        }

        private static bool TryScrollToCenterOfView(this ItemsControl itemsControl, object item)
        {
            // Find the container
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
            if (container == null) return false;

            // Find the ScrollContentPresenter
            ScrollContentPresenter presenter = null;
            for (Visual vis = container; vis != null && vis != itemsControl; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if ((presenter = vis as ScrollContentPresenter) != null)
                    break;
            if (presenter == null) return false;

            // Find the IScrollInfo
            var scrollInfo =
                !presenter.CanContentScroll ? presenter :
                presenter.Content as IScrollInfo ??
                FirstVisualChild(presenter.Content as ItemsPresenter) as IScrollInfo ??
                presenter;

            // Compute the center point of the container relative to the scrollInfo
            System.Windows.Size size = container.RenderSize;
            System.Windows.Point center = container.TransformToAncestor((Visual)scrollInfo).Transform(new System.Windows.Point(size.Width / 2, size.Height / 2));
            center.Y += scrollInfo.VerticalOffset;
            center.X += scrollInfo.HorizontalOffset;

            // Adjust for logical scrolling
            if (scrollInfo is StackPanel || scrollInfo is VirtualizingStackPanel)
            {
                double logicalCenter = itemsControl.ItemContainerGenerator.IndexFromContainer(container) + 0.5;
                Orientation orientation = scrollInfo is StackPanel ? ((StackPanel)scrollInfo).Orientation : ((VirtualizingStackPanel)scrollInfo).Orientation;
                if (orientation == Orientation.Horizontal)
                    center.X = logicalCenter;
                else
                    center.Y = logicalCenter;
            }

            // Scroll the center of the container to the center of the viewport
            if (scrollInfo.CanVerticallyScroll) scrollInfo.SetVerticalOffset(CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));
            if (scrollInfo.CanHorizontallyScroll) scrollInfo.SetHorizontalOffset(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth));
            return true;
        }

        private static double CenteringOffset(double center, double viewport, double extent)
        {
            return Math.Min(extent - viewport, Math.Max(0, center - viewport / 2));
        }
        private static DependencyObject FirstVisualChild(Visual visual)
        {
            if (visual == null) return null;
            if (VisualTreeHelper.GetChildrenCount(visual) == 0) return null;
            return VisualTreeHelper.GetChild(visual, 0);
        }



        public static S GetEventSourceElement<S>(this ItemsControl aItemsControl, RoutedEventArgs args) where S : class
        {
            DependencyObject dep = (DependencyObject)args.OriginalSource;
            while ((dep != null) && !(dep is S))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            return dep as S;
        }

        public static T GetEventSourceItem<T, S>(this ItemsControl aItemsControl, RoutedEventArgs args)
            where T : class
            where S : DependencyObject
        {
            S container = aItemsControl.GetEventSourceElement<S>(args);
            if (container != null)
            {
                return aItemsControl.ItemContainerGenerator.ItemFromContainer(container) as T;
            }
            return null;
        }

        public static void EnsureSelected(this ListView aListView, int aIndex)
        {
            aListView.Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    aListView.Focus();
                    int count = aListView.Items.Count;
                    if (aIndex < count)
                    {
                        object item = aListView.Items.GetItemAt(aIndex);
                        if (item != null && !(item is PlaceholderBrowserItem))
                        {
                            aListView.UpdateLayout();
                            try
                            {
                                aListView.ScrollIntoView(item);
                            }
                            catch (NullReferenceException) { }// ignore.  item is not realized and there is an internal bug in VirtualizingStackPanel that doesn't cope with this condition
                            aListView.SelectedItems.Clear();
                            aListView.SelectedItems.Add(item);

                            EventHandler statusChangedHandler = null;
                            statusChangedHandler = (object sender, EventArgs e) =>
                            {
                                if (aListView.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                                {
                                    int selectedIndex = aListView.SelectedIndex;
                                    if (selectedIndex < 0)
                                        aListView.ItemContainerGenerator.StatusChanged -= statusChangedHandler;
                                    else
                                    {
                                        ListBoxItem listBoxItem = aListView.ItemContainerGenerator.ContainerFromIndex(selectedIndex) as ListBoxItem;
                                        if (listBoxItem != null)
                                        {
                                            listBoxItem.Focus();
                                            aListView.ItemContainerGenerator.StatusChanged += null;
                                            aListView.ItemContainerGenerator.StatusChanged -= statusChangedHandler;
                                        }
                                    }
                                }
                            };
                            aListView.ItemContainerGenerator.StatusChanged += statusChangedHandler;
                            statusChangedHandler(aListView, EventArgs.Empty);
                        }
                    }
                }
                catch (Exception e)
                {
                    UserLog.WriteLine("Exception in EnsureSelected: " + e.ToString());
                    Assert.CheckDebug(false);
                }
            }), DispatcherPriority.SystemIdle);
        }
    }

    public static class AnimationExtensions
    {

        public static Duration kTextRolloverAnimationDuration = new Duration(TimeSpan.FromMilliseconds(50.0));
        public static Duration kImageRolloverAnimationDuration = new Duration(TimeSpan.FromMilliseconds(200.0));
        public static Duration kUIElementAnimationDuration = new Duration(TimeSpan.FromMilliseconds(200.0));


        public static void AnimateVisibility(this UIElement aElement, bool aVisible, Action aTimeoutAction)
        {
            Visibility visibility = aVisible ? Visibility.Visible : Visibility.Collapsed;
            if (aVisible)
            {
                aElement.Visibility = visibility;
            }
            aElement.DoubleAnimate(new PropertyPath("Opacity"), aVisible ? 0 : 1, aVisible ? 1 : 0, kUIElementAnimationDuration, () =>
            {
                if (aTimeoutAction != null)
                {
                    aTimeoutAction.Invoke();
                }
                aElement.Visibility = visibility;
            });
        }

        public static void DoubleAnimate(this DependencyObject aElement, PropertyPath aPropertyPath, double aFromValue, double aToValue, Duration aDuration, Action aTimeoutAction)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation(aFromValue, aToValue, aDuration, FillBehavior.Stop);

            Storyboard.SetTarget(animation, aElement);
            Storyboard.SetTargetProperty(animation, aPropertyPath);
            storyboard.Children.Add(animation);

            EventHandler handler = null;
            handler = (d, e) =>
            {
                if (aTimeoutAction != null)
                {
                    aTimeoutAction.Invoke();
                }
                storyboard.Completed -= handler;
                storyboard.Remove();
            };
            storyboard.Completed += handler;
            storyboard.Begin();
        }

        public static void GridLengthAnimate(this DependencyObject aElement, PropertyPath aPropertyPath, GridLength aFromValue, GridLength aToValue, Duration aDuration, Action aTimeoutAction)
        {
            Storyboard storyboard = new Storyboard();
            GridLengthAnimation animation = new GridLengthAnimation();
            animation.From = aFromValue;
            animation.To = aToValue;
            animation.Duration = aDuration;
            animation.FillBehavior = FillBehavior.Stop;


            Storyboard.SetTarget(animation, aElement);
            Storyboard.SetTargetProperty(animation, aPropertyPath);
            storyboard.Children.Add(animation);

            EventHandler handler = null;
            handler = (d, e) =>
            {
                if (aTimeoutAction != null)
                {
                    aTimeoutAction.Invoke();
                }
                storyboard.Completed -= handler;
                storyboard.Remove();
            };
            storyboard.Completed += handler;
            storyboard.Begin();
        }


        public static void AnimateTextColourIn(this DependencyObject source, System.Windows.Media.Color from, System.Windows.Media.Color to)
        {
            ColorAnimation anim = new ColorAnimation(from, to, kTextRolloverAnimationDuration);
            TextBlock[] txtBlocks = source.FindVisualChildren<TextBlock>(true);
            foreach (TextBlock txt in txtBlocks)
            {
                txt.Foreground = new SolidColorBrush(from);
                txt.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
        }
        public static void ClearAnimatedTextColour(this DependencyObject source)
        {
            TextBlock[] txtBlocks = source.FindVisualChildren<TextBlock>(true);
            foreach (TextBlock item in txtBlocks)
            {
                item.ClearValue(TextBlock.ForegroundProperty);
            }
        }

        public static void AnimateTextColourOut(this DependencyObject source, System.Windows.Media.Color from, System.Windows.Media.Color to)
        {
            ColorAnimation anim = new ColorAnimation(from, to, kTextRolloverAnimationDuration);
            TextBlock[] txtBlocks = source.FindVisualChildren<TextBlock>(true);

            foreach (TextBlock txt in txtBlocks)
            {
                txt.Foreground = new SolidColorBrush(from);
                txt.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = kTextRolloverAnimationDuration.TimeSpan;
            t.Tick += ((d, a) =>
            {
                source.ClearAnimatedTextColour();
                t.Stop();
            });
            t.Start();
        }
        #region GridLengthAnimation
        internal class GridLengthAnimation : AnimationTimeline
        {
            static GridLengthAnimation()
            {
                FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                    typeof(GridLengthAnimation));

                ToProperty = DependencyProperty.Register("To", typeof(GridLength),
                    typeof(GridLengthAnimation));
            }

            public override Type TargetPropertyType
            {
                get
                {
                    return typeof(GridLength);
                }
            }

            protected override System.Windows.Freezable CreateInstanceCore()
            {
                return new GridLengthAnimation();
            }

            public static readonly DependencyProperty FromProperty;
            public GridLength From
            {
                get
                {
                    return (GridLength)GetValue(GridLengthAnimation.FromProperty);
                }
                set
                {
                    SetValue(GridLengthAnimation.FromProperty, value);
                }
            }

            public static readonly DependencyProperty ToProperty;
            public GridLength To
            {
                get
                {
                    return (GridLength)GetValue(GridLengthAnimation.ToProperty);
                }
                set
                {
                    SetValue(GridLengthAnimation.ToProperty, value);
                }
            }

            public override object GetCurrentValue(object defaultOriginValue,
                object defaultDestinationValue, AnimationClock animationClock)
            {
                double fromVal = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
                double toVal = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;

                if (fromVal > toVal)
                {
                    return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal, GridUnitType.Star);
                }
                else
                    return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal, GridUnitType.Star);
            }
        }
        #endregion
    }


    public delegate void ScreenCoordinatesRoutedEventHandler(
        Object sender,
        ScreenCoordinatesRoutedEventArgs e
    );

    public class ScreenCoordinatesRoutedEventArgs : RoutedEventArgs
    {
        public ScreenCoordinatesRoutedEventArgs(RoutedEvent aRoutedEvent, object aSource, System.Windows.Point aScreenCoordinates)
            : base(aRoutedEvent, aSource)
        {
            ScreenCoordinates = aScreenCoordinates;
        }
        public System.Windows.Point ScreenCoordinates { get; set; }
    }

    public delegate void SelectionRoutedEventHandler(
        Object sender,
        SelectionRoutedEventArgs e
    );

    public class SelectionRoutedEventArgs : RoutedEventArgs
    {
        public SelectionRoutedEventArgs(RoutedEvent aRoutedEvent, object aSource, object aSelectedObject)
            : base(aRoutedEvent, aSource)
        {
            SelectedObject = aSelectedObject;
        }
        public object SelectedObject { get; set; }
    }

    public class ItemAlreadyExistsException : ApplicationException
    {
        public ItemAlreadyExistsException(string message) : base(message) { }
    }

    // fixes defect in design of ListView which prevents drag and drop in tandem with multiselect due to selection happening on mouse down instead of mouse up
    
    public class ListViewFix : ListView
    {
        private static uint MouseDoubleClickTime = NativeWindowMethods.GetDoubleClickTime();
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            ListViewItem listViewItem = (e.OriginalSource as DependencyObject).FindVisualAncestor<ListViewItem>();
            var clickedItem = listViewItem == null
                ? null
                : ItemContainerGenerator.ItemFromContainer(listViewItem);
            var previousClick = iLastClick;
            iLastClick = DateTime.Now;
            // left mouse double click
            if (clickedItem != null &&
                clickedItem == iLastClickedItem &&
                iLastClick.CompareTo(previousClick.AddMilliseconds(MouseDoubleClickTime)) < 0
                && e.LeftButton == MouseButtonState.Pressed)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseDoubleClickEvent;
                args.Source = e.OriginalSource;
                RaiseEvent(args);
                e.Handled = true;
                return;
            }
            if (clickedItem != null && SelectedItems.Contains(clickedItem))
            {
                // Since we are stopping the click action from
                // working, we have to also handle focus.
                if (!IsFocused)
                    Focus();
                iDelayClickAction = true;
                // only want to stop propagation of routed event if the original source was not in a button
                // otherwise button click event will not happen on a button that is contained within a selected list item
                Button b = (e.OriginalSource as DependencyObject).FindVisualAncestor<Button>();
                TextBox tb = (e.OriginalSource as DependencyObject).FindVisualAncestor<TextBox>();
                if (b == null && tb == null)
                {
                    e.Handled = true;
                }
                iLastClickedItem = clickedItem;
                Mouse.Capture(this, CaptureMode.SubTree);
                return;
            }
            iDelayClickAction = false;
            iLastClickedItem = clickedItem;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (iDelayClickAction)
            {
                var listViewItem = (e.OriginalSource as DependencyObject).FindVisualAncestor<ListViewItem>();
                var clickedItem = listViewItem == null
                    ? null
                    : ItemContainerGenerator.ItemFromContainer(listViewItem);
                if (clickedItem != null)
                {
                    if (Keyboard.Modifiers == ModifierKeys.None)
                    {
                        SelectedItems.Clear();
                        SelectedItem = clickedItem;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        SelectedItems.Remove(clickedItem);
                    }
                }
            }
            iDelayClickAction = false;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
            //base.OnMouseDoubleClick(e);
        }

        private DateTime iLastClick = DateTime.MinValue;
        private object iLastClickedItem;
        private bool iDelayClickAction;
    }

    /// <summary>
    /// Attached property provider which adds the read-only attached property
    /// <c>TextBlockService.IsTextTrimmed</c> to the framework's <see cref="TextBlock"/> control.
    /// </summary>
    public class TextBlockService
    {
        static TextBlockService()
        {
            // Register for the SizeChanged event on all TextBlocks, even if the event was handled.
            EventManager.RegisterClassHandler(
                typeof(TextBlock),
                FrameworkElement.SizeChangedEvent,
                new SizeChangedEventHandler(OnTextBlockSizeChanged),
                true);
        }

        #region Attached Property [TextBlockService.IsTextTrimmed]

        /// <summary>
        /// Key returned upon registering the read-only attached property <c>IsTextTrimmed</c>.
        /// </summary>
        public static readonly DependencyPropertyKey IsTextTrimmedKey = DependencyProperty.RegisterAttachedReadOnly(
            "IsTextTrimmed",
            typeof(bool),
            typeof(TextBlockService),
            new PropertyMetadata(false));    // defaults to false

        /// <summary>
        /// Identifier associated with the read-only attached property <c>IsTextTrimmed</c>.
        /// </summary>
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        /// <summary>
        /// Returns the current effective value of the IsTextTrimmed attached property.
        /// </summary>
        /// <remarks>Invoked automatically by the framework when databound.</remarks>
        /// <param name="target"><see cref="TextBlock"/> to evaluate</param>
        /// <returns>Effective value of the IsTextTrimmed attached property</returns>
        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static Boolean GetIsTextTrimmed(TextBlock target)
        {
            return (Boolean)target.GetValue(IsTextTrimmedProperty);
        }

        #endregion (Attached Property [TextBlockService.IsTextTrimmed])

        #region Attached Property [TextBlockService.AutomaticToolTipEnabled]

        /// <summary>
        /// Identifier associated with the attached property <c>AutomaticToolTipEnabled</c>.
        /// </summary>
        public static readonly DependencyProperty AutomaticToolTipEnabledProperty = DependencyProperty.RegisterAttached(
            "AutomaticToolTipEnabled",
            typeof(bool),
            typeof(TextBlockService),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));    // defaults to true

        /// <summary>
        /// Gets the current effective value of the AutomaticToolTipEnabled attached property.
        /// </summary>
        /// <param name="target"><see cref="TextBlock"/> to evaluate</param>
        /// <returns>Effective value of the AutomaticToolTipEnabled attached property</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Boolean GetAutomaticToolTipEnabled(DependencyObject element)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(AutomaticToolTipEnabledProperty);
        }

        /// <summary>
        /// Sets the current effective value of the AutomaticToolTipEnabled attached property.
        /// </summary>
        /// <param name="target"><see cref="TextBlock"/> to evaluate</param>
        /// <param name="value"><c>true</c> to enable the automatic ToolTip; otherwise <c>false</c></param>
        public static void SetAutomaticToolTipEnabled(DependencyObject element, bool value)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(AutomaticToolTipEnabledProperty, value);
        }

        #endregion (Attached Property [TextBlockService.AutomaticToolTipEnabled])

        /// <summary>
        /// Event handler for TextBlock's SizeChanged routed event. Triggers evaluation of the
        /// IsTextTrimmed attached property.
        /// </summary>
        /// <param name="sender">Object where the event handler is attached</param>
        /// <param name="e">Event data</param>
        public static void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (null == textBlock)
            {
                return;
            }

            if (TextTrimming.None == textBlock.TextTrimming)
            {
                SetIsTextTrimmed(textBlock, false);
            }
            else
            {
                SetIsTextTrimmed(textBlock, CalculateIsTextTrimmed(textBlock));
            }
        }

        /// <summary>
        /// Sets the instance value of read-only dependency property <see cref="IsTextTrimmed"/>.
        /// </summary>
        /// <param name="target">Associated <see cref="TextBlock"/> instance</param>
        /// <param name="value">New value for IsTextTrimmed</param>
        private static void SetIsTextTrimmed(TextBlock target, Boolean value)
        {
            target.SetValue(IsTextTrimmedKey, value);
        }

        /// <summary>
        /// Determines whether or not the text in <paramref name="textBlock"/> is currently being
        /// trimmed due to width or height constraints.
        /// </summary>
        /// <remarks>Does not work properly when TextWrapping is set to WrapWithOverflow.</remarks>
        /// <param name="textBlock"><see cref="TextBlock"/> to evaluate</param>
        /// <returns><c>true</c> if the text is currently being trimmed; otherwise <c>false</c></returns>
        private static bool CalculateIsTextTrimmed(TextBlock textBlock)
        {
            if (!textBlock.IsArrangeValid)
            {
                return GetIsTextTrimmed(textBlock);
            }

            Typeface typeface = new Typeface(
                textBlock.FontFamily,
                textBlock.FontStyle,
                textBlock.FontWeight,
                textBlock.FontStretch);

            // FormattedText is used to measure the whole width of the text held up by TextBlock container
            FormattedText formattedText = new FormattedText(
                textBlock.Text,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                textBlock.FlowDirection,
                typeface,
                textBlock.FontSize,
                textBlock.Foreground);

            formattedText.MaxTextWidth = textBlock.ActualWidth;

            // When the maximum text width of the FormattedText instance is set to the actual
            // width of the textBlock, if the textBlock is being trimmed to fit then the formatted
            // text will report a larger height than the textBlock. Should work whether the
            // textBlock is single or multi-line.
            return (formattedText.Height > textBlock.ActualHeight);
        }
    }

    internal static class NativeWindowMethods
    {
        [DllImport("user32.dll")]
        internal static extern uint GetDoubleClickTime();

    }

    public class Invoker : IInvoker
    {
        private Dispatcher iDispatcher;
        public Invoker(Dispatcher aDispatcher)
        {
            iDispatcher = aDispatcher;
        }

        #region IInvoker Members

        public bool InvokeRequired
        {
            get { return !iDispatcher.CheckAccess(); }
        }

        public void BeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            iDispatcher.BeginInvoke(aDelegate, aArgs);
        }

        public bool TryBeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            if (!iDispatcher.CheckAccess())
            {
                iDispatcher.BeginInvoke(aDelegate, aArgs);
                return true;
            }
            return false;
        }

        #endregion
    }

    public class ClippingPanel : Panel
    {
        public ClippingPanel() : base() { }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
            }
            return availableSize;
        }

        // Arrange the child elements to their final position
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            double height = 0d;
            foreach (UIElement child in InternalChildren)
            {
                height += child.DesiredSize.Height;
                if (height < finalSize.Height)
                {
                    child.Arrange(new Rect(0, height - child.DesiredSize.Height, finalSize.Width, child.DesiredSize.Height));
                }
                else
                {
                    child.Arrange(new Rect(0, 0, 0, 0));
                }
            }
            return finalSize;
        }
    }

}