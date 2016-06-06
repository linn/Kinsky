using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media.Animation;
using System.Collections;
using System.Reflection;
using System.Windows.Automation.Provider;

namespace KinskyDesktopWpf
{

    public class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo
    {
        public VirtualizingTilePanel()
        {
            // For use in the IScrollInfo implementation
            this.RenderTransform = _trans;
            iPreviousFirstVisibleItemIndex = -1;
            iPreviousLastVisibleItemIndex = -1;
            iPreviousAvailableSize = Size.Empty;
        }

        private int iPreviousFirstVisibleItemIndex;
        private int iPreviousLastVisibleItemIndex;
        private int iFirstVisibleChildIndex;
        private List<UIElement> iRealizedChildren;
        private Size iPreviousAvailableSize;
        private int iChildrenPerRow;
        private object iCurrentFocusedItem;



        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set
            { SetValue(ItemSizeProperty, value); }
        }

        public static readonly DependencyProperty ItemSizeProperty =
            DependencyProperty.Register("ItemSize", typeof(double), typeof(VirtualizingTilePanel), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));


        public int CurrentIndex
        {
            get
            {
                return iPreviousFirstVisibleItemIndex;
            }
            set
            {
                BringIndexIntoView(value);
            }
        }


        private static MethodInfo kInternalMove = typeof(UIElementCollection).GetMethod("MoveVisualChild", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo kInternalInsert = typeof(VirtualizingPanel).GetMethod("InsertInternalChild", BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo kInternalAdd = typeof(VirtualizingPanel).GetMethod("AddInternalChild", BindingFlags.NonPublic | BindingFlags.Static);

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            UpdateLayout();
            base.OnGotKeyboardFocus(e);
            UIElement container = e.NewFocus as UIElement;
            var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
            object item = gen.ItemFromContainer(container);
            iCurrentFocusedItem = item;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            iCurrentFocusedItem = null;
        }

        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateScrollInfo(availableSize);
            EnsureRealizedChildren();
            Size desiredSize = new Size(availableSize.Width, availableSize.Height);
            iChildrenPerRow = (int)Math.Floor(availableSize.Width / ItemSize);

            // Figure out range that's visible based on layout algorithm
            int firstVisibleItemIndex, lastVisibleItemIndex;
            GetVisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);

            if (iPreviousFirstVisibleItemIndex == -1 || iPreviousFirstVisibleItemIndex != firstVisibleItemIndex || iPreviousLastVisibleItemIndex != lastVisibleItemIndex || availableSize.Width != iPreviousAvailableSize.Width || availableSize.Height != iPreviousAvailableSize.Height)
            {
                iPreviousAvailableSize = availableSize;
                iPreviousFirstVisibleItemIndex = firstVisibleItemIndex;
                iPreviousLastVisibleItemIndex = lastVisibleItemIndex;
                IList children = this.RealizedChildren;
                ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
                int itemCount = (itemsOwner != null) ? itemsOwner.Items.Count : 0;


                IItemContainerGenerator generator = this.ItemContainerGenerator;
                CleanupContainers(firstVisibleItemIndex, itemsOwner);

                GeneratorPosition position = this.IndexToGeneratorPositionForStart(firstVisibleItemIndex, out this.iFirstVisibleChildIndex);
                int childIndex = this.iFirstVisibleChildIndex;
                if (itemCount > 0)
                {
                    double actualSize = availableSize.Width / iChildrenPerRow;
                    Size size = new Size(actualSize, actualSize);
                    using (generator.StartAt(position, GeneratorDirection.Forward, true))
                    {
                        for (int i = firstVisibleItemIndex; i <= lastVisibleItemIndex; i++)
                        {
                            bool newlyRealized;
                            UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                            if (child == null)
                            {
                                break;
                            }
                            this.AddContainerFromGenerator(childIndex, child, newlyRealized);
                            childIndex++;
                            child.Measure(size);
                        }
                    }
                }
            }
            this.DisconnectRecycledContainers();
            return desiredSize;
        }

        private void DisconnectRecycledContainers()
        {
            int index = 0;
            UIElement seek = (this.iRealizedChildren.Count > 0) ? this.iRealizedChildren[0] : null;
            UIElementCollection internalChildren = base.InternalChildren;
            for (int i = 0; i < internalChildren.Count; i++)
            {
                UIElement current = internalChildren[i];
                if (current == seek)
                {
                    index++;
                    if (index < this.iRealizedChildren.Count)
                    {
                        seek = this.iRealizedChildren[index];
                    }
                    else
                    {
                        seek = null;
                    }
                }
                else
                {
                    MethodInfo internalMove = typeof(UIElementCollection).GetMethod("RemoveNoVerify", BindingFlags.NonPublic | BindingFlags.Instance);
                    internalMove.Invoke(internalChildren, new object[] { current });
                    i--;
                }
            }
        }


        private int ChildIndexFromRealizedIndex(int realizedChildIndex)
        {
            if (realizedChildIndex < this.iRealizedChildren.Count)
            {
                UIElement element = this.iRealizedChildren[realizedChildIndex];
                UIElementCollection internalChildren = base.InternalChildren;
                for (int i = realizedChildIndex; i < internalChildren.Count; i++)
                {
                    if (internalChildren[i] == element)
                    {
                        return i;
                    }
                }
            }
            return realizedChildIndex;
        }


        private bool InsertContainer(int childIndex, UIElement container, bool isRecycled)
        {
            bool success = false;
            UIElementCollection internalChildren = base.InternalChildren;
            int num = 0;
            if (childIndex > 0)
            {
                num = this.ChildIndexFromRealizedIndex(childIndex - 1) + 1;
            }
            if ((!isRecycled || (num >= internalChildren.Count)) || (internalChildren[num] != container))
            {
                if (num < internalChildren.Count)
                {
                    int index = num;


                    if (isRecycled && (VisualTreeHelper.GetParent(container) != null))
                    {
                        kInternalMove.Invoke(internalChildren, new object[] { container, internalChildren[num] });
                        success = true;
                    }
                    else
                    {
                        kInternalInsert.Invoke(null, new object[] { internalChildren, index, container });
                    }
                }
                else if (isRecycled && (VisualTreeHelper.GetParent(container) != null))
                {
                    kInternalMove.Invoke(internalChildren, new object[] { container, null });
                    success = true;
                }
                else
                {
                    kInternalAdd.Invoke(null, new object[] { internalChildren, container });
                }
            }
            this.iRealizedChildren.Insert(childIndex, container);
            this.ItemContainerGenerator.PrepareItemContainer(container);
            return success;
        }

        private bool AddContainerFromGenerator(int childIndex, UIElement child, bool newlyRealized)
        {
            bool success = false;
            if (!newlyRealized)
            {
                IList realizedChildren = this.RealizedChildren;
                if ((childIndex < realizedChildren.Count) && (realizedChildren[childIndex] == child))
                {
                    return success;
                }
                return this.InsertContainer(childIndex, child, true);
            }
            this.InsertContainer(childIndex, child, false);
            return success;
        }


        private void EnsureRealizedChildren()
        {
            if (this.iRealizedChildren == null)
            {
                UIElementCollection internalChildren = base.InternalChildren;
                this.iRealizedChildren = new List<UIElement>(internalChildren.Count);
                for (int i = 0; i < internalChildren.Count; i++)
                {
                    this.iRealizedChildren.Add(internalChildren[i]);
                }
            }
        }

        private IList RealizedChildren
        {
            get
            {
                this.EnsureRealizedChildren();
                return this.iRealizedChildren;
            }
        }

        private void CleanupContainers(int firstViewport, ItemsControl itemsControl)
        {
            int startIndex = -1;
            int count = 0;
            int itemIndex = -1;
            IList realizedChildren = this.RealizedChildren;
            bool found = false;
            if (realizedChildren.Count != 0)
            {
                for (int i = 0; i < realizedChildren.Count; i++)
                {
                    UIElement container = (UIElement)realizedChildren[i];
                    int previousIndex = itemIndex;
                    itemIndex = this.GetGeneratedIndex(i);
                    if ((itemIndex - previousIndex) != 1)
                    {
                        found = true;
                    }
                    if (found)
                    {
                        if ((startIndex >= 0) && (count > 0))
                        {
                            this.CleanupRange(realizedChildren, this.ItemContainerGenerator, startIndex, count);
                            i -= count;
                            count = 0;
                            startIndex = -1;
                        }
                        found = false;
                    }
                    if (startIndex == -1)
                    {
                        startIndex = i;
                    }
                    count++;
                }
                if ((startIndex >= 0) && (count > 0))
                {
                    this.CleanupRange(realizedChildren, this.ItemContainerGenerator, startIndex, count);
                }
            }
        }

        private int GetGeneratedIndex(int childIndex)
        {
            return this.ItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0));
        }

        private void CleanupRange(IList children, IItemContainerGenerator generator, int startIndex, int count)
        {
            ((IRecyclingItemContainerGenerator)generator).Recycle(new GeneratorPosition(startIndex, 0), count);
            this.iRealizedChildren.RemoveRange(startIndex, count);
            this.AdjustFirstVisibleChildIndex(startIndex, count);
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.iRealizedChildren.Clear();
                    iPreviousFirstVisibleItemIndex = -1;
                    iPreviousLastVisibleItemIndex = -1;
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    this.iRealizedChildren.RemoveRange(args.Position.Index, args.ItemUICount);
                    this.AdjustFirstVisibleChildIndex(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.iRealizedChildren.RemoveRange(args.OldPosition.Index, args.ItemUICount);
                    this.AdjustFirstVisibleChildIndex(args.OldPosition.Index, args.ItemUICount);
                    break;
            }
        }

        private void AdjustFirstVisibleChildIndex(int startIndex, int count)
        {
            if (startIndex < this.iFirstVisibleChildIndex)
            {
                int num = (startIndex + count) - 1;
                if (num < this.iFirstVisibleChildIndex)
                {
                    this.iFirstVisibleChildIndex -= count;
                }
                else
                {
                    this.iFirstVisibleChildIndex = startIndex;
                }
            }
        }


        private GeneratorPosition IndexToGeneratorPositionForStart(int index, out int childIndex)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;
            GeneratorPosition position = (generator != null) ? generator.GeneratorPositionFromIndex(index) : new GeneratorPosition(-1, index + 1);
            childIndex = (position.Offset == 0) ? position.Index : (position.Index + 1);
            return position;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    NavigateDown();
                    e.Handled = true;
                    break;
                case Key.Left:
                    NavigateLeft();
                    e.Handled = true;
                    break;
                case Key.Right:
                    NavigateRight();
                    e.Handled = true;
                    break;
                case Key.Up:
                    NavigateUp();
                    e.Handled = true;
                    break;
                case Key.Home:
                    NavigateHome();
                    e.Handled = true;
                    break;
                case Key.End:
                    NavigateEnd();
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    NavigatePageDown();
                    e.Handled = true;
                    break;
                case Key.PageUp:
                    NavigatePageUp();
                    e.Handled = true;
                    break;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        private void NavigateLeft()
        {
            try
            {
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                DependencyObject container = gen.ContainerFromItem(iCurrentFocusedItem);
                if (container == null) { container = Keyboard.FocusedElement as DependencyObject; }
                if (container != null)
                {
                    int itemIndex = gen.IndexFromContainer(container);
                    DependencyObject previous = null;
                    if (itemIndex > 0)
                    {
                        previous = gen.ContainerFromIndex(itemIndex - 1);
                        while (previous == null && _offset.Y > 0)
                        {
                            itemIndex = gen.IndexFromContainer(container);
                            LineUp();
                            UpdateLayout();
                            previous = gen.ContainerFromIndex(itemIndex - 1);
                        }
                        if (previous != null)
                        {
                            (previous as UIElement).Focus();
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigateRight()
        {
            try
            {
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                DependencyObject container = gen.ContainerFromItem(iCurrentFocusedItem);
                if (container == null) { container = Keyboard.FocusedElement as DependencyObject; }
                if (container != null)
                {
                    int itemIndex = gen.IndexFromContainer(container);
                    DependencyObject next = null;
                    next = gen.ContainerFromIndex(itemIndex + 1);
                    while (next == null && _offset.Y < _extent.Height - _viewport.Height - ItemSize)
                    {
                        itemIndex = gen.IndexFromContainer(container);
                        LineDown();
                        UpdateLayout();
                        next = gen.ContainerFromIndex(itemIndex + 1);
                    }
                    if (next != null)
                    {
                        (next as UIElement).Focus();
                    }
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigateUp()
        {
            try
            {
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                DependencyObject container = gen.ContainerFromItem(iCurrentFocusedItem);
                if (container == null) { container = Keyboard.FocusedElement as DependencyObject; }
                if (container != null)
                {
                    int itemIndex = gen.IndexFromContainer(container);
                    DependencyObject previous = null;
                    if (itemIndex >= iChildrenPerRow)
                    {
                        previous = gen.ContainerFromIndex(itemIndex - iChildrenPerRow);
                        while (previous == null && _offset.Y > 0)
                        {
                            itemIndex = gen.IndexFromContainer(container);
                            LineUp();
                            UpdateLayout();
                            previous = gen.ContainerFromIndex(itemIndex - iChildrenPerRow);
                        }
                        if (previous != null)
                        {
                            //hack: for some reason, calling previous.Focus() doesn't always work - need to go back one element at a time???
                            int idx = itemIndex;
                            while (idx >= itemIndex - iChildrenPerRow)
                            {
                                var hack = gen.ContainerFromIndex(idx--);
                                if (hack != null)
                                {
                                    (hack as UIElement).Focus();
                                }
                            }
                            //(previous as UIElement).Focus();
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigateDown()
        {
            try
            {
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                DependencyObject container = gen.ContainerFromItem(iCurrentFocusedItem);
                if (container == null) { container = Keyboard.FocusedElement as DependencyObject; }
                if (container != null)
                {
                    int itemIndex = gen.IndexFromContainer(container);
                    DependencyObject next = null;
                    next = gen.ContainerFromIndex(itemIndex + iChildrenPerRow);
                    while (next == null && _offset.Y < _extent.Height - _viewport.Height - ItemSize)
                    {
                        itemIndex = gen.IndexFromContainer(container);
                        LineDown();
                        UpdateLayout();
                        next = gen.ContainerFromIndex(itemIndex + iChildrenPerRow);
                    }
                    if (next != null)
                    {
                        //hack: for some reason, calling next.Focus() doesn't always work - need to go forward one element at a time???
                        int idx = itemIndex;
                        while (idx <= itemIndex + iChildrenPerRow)
                        {
                            var hack = gen.ContainerFromIndex(idx++);
                            if (hack != null)
                            {
                                (hack as UIElement).Focus();
                            }
                        }
                        //(next as UIElement).Focus();
                    }
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigateHome()
        {
            try
            {
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                DependencyObject next = null;
                while (_offset.Y > 0)
                {
                    LineUp();
                }
                UpdateLayout();
                next = gen.ContainerFromIndex(0);
                if (next != null)
                {
                    (next as UIElement).Focus();
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigateEnd()
        {
            try
            {
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                while (_offset.Y < _extent.Height - _viewport.Height - ItemSize)
                {
                    LineDown();
                }
                UpdateLayout();
                if (this.InternalChildren.Count > 0)
                {
                    (this.InternalChildren[this.InternalChildren.Count - 1] as UIElement).Focus();
                }

            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigatePageDown()
        {
            try
            {
                int itemCount = (int)(Math.Round(ViewportHeight / ItemSize, MidpointRounding.AwayFromZero) * iChildrenPerRow);
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                UIElement selected = (UIElement)Keyboard.FocusedElement;
                int itemIndex = gen.IndexFromContainer(selected);
                DependencyObject next = null;
                next = gen.ContainerFromIndex(itemIndex + itemCount);
                while (next == null && _offset.Y < _extent.Height - _viewport.Height - ItemSize)
                {
                    itemIndex = gen.IndexFromContainer(selected);
                    PageDown();
                    UpdateLayout();
                    next = gen.ContainerFromIndex(itemIndex + itemCount);
                }
                if (next != null)
                {
                    //hack: for some reason, calling next.Focus() doesn't always work - need to go forward one element at a time???
                    int idx = itemIndex;
                    while (idx <= itemIndex + itemCount)
                    {
                        var hack = gen.ContainerFromIndex(idx++);
                        if (hack != null)
                        {
                            (hack as UIElement).Focus();
                        }
                    }
                    //(next as UIElement).Focus();
                }
            }
            catch (IndexOutOfRangeException) { }
        }

        private void NavigatePageUp()
        {
            try
            {
                int itemCount = (int)(Math.Round(ViewportHeight / ItemSize, MidpointRounding.AwayFromZero) * iChildrenPerRow);
                var gen = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);
                UIElement selected = (UIElement)Keyboard.FocusedElement;
                int itemIndex = gen.IndexFromContainer(selected);
                DependencyObject previous = null;
                if (itemIndex < itemCount)
                {
                    itemCount = itemIndex;
                }
                previous = gen.ContainerFromIndex(itemIndex - itemCount);
                while (previous == null && _offset.Y > 0)
                {
                    itemIndex = gen.IndexFromContainer(selected);
                    if (itemIndex < itemCount)
                    {
                        itemCount = itemIndex;
                    }
                    PageUp();
                    UpdateLayout();
                    previous = gen.ContainerFromIndex(itemIndex - itemCount);
                }
                if (previous != null)
                {
                    //hack: for some reason, calling previous.Focus() doesn't always work - need to go back one element at a time???
                    int idx = itemIndex;
                    while (idx >= itemIndex - itemCount)
                    {
                        var hack = gen.ContainerFromIndex(idx--);
                        if (hack != null)
                        {
                            (hack as UIElement).Focus();
                        }
                    }
                    //(previous as UIElement).Focus();
                }
            }
            catch (IndexOutOfRangeException) { }
        }


        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            UpdateScrollInfo(finalSize);
            for (int i = 0; i < iRealizedChildren.Count; i++)
            {
                UIElement child = iRealizedChildren[i];

                // Map the child offset to an item offset
                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));
                ArrangeChild(itemIndex, child, finalSize);
            }
            return finalSize;
        }

        #region Layout specific code
        // I've isolated the layout specific code to this region. If you want to do something other than tiling, this is
        // where you'll make your changes

        /// <summary>
        /// Calculate the extent of the view based on the available size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <param name="itemCount">number of data items</param>
        /// <returns></returns>
        private Size CalculateExtent(Size availableSize, int itemCount)
        {
            // See how big we are
            return new Size(iChildrenPerRow * this.ItemSize,
                (this.ItemSize * Math.Ceiling((double)itemCount / iChildrenPerRow)) + (this.ItemSize / 2));
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            if (this.ItemSize == 0 || double.IsInfinity(_offset.Y))
            {
                firstVisibleItemIndex = 0;
                lastVisibleItemIndex = 0;
            }
            else
            {
                firstVisibleItemIndex = (int)Math.Floor(_offset.Y / this.ItemSize) * iChildrenPerRow;
                lastVisibleItemIndex = (int)Math.Ceiling((_offset.Y + _viewport.Height) / this.ItemSize) * iChildrenPerRow - 1;
            }

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
            if (lastVisibleItemIndex >= itemCount)
                lastVisibleItemIndex = itemCount - 1;

        }

        /// <summary>
        /// Get the size of the children. We assume they are all the same
        /// </summary>
        /// <returns>The size</returns>
        private Size GetChildSize()
        {
            return new Size(this.ItemSize, this.ItemSize);
        }

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        {
            if (iChildrenPerRow > 0)
            {
                double actualSize = finalSize.Width / iChildrenPerRow;
                int row = itemIndex / iChildrenPerRow;
                int column = itemIndex % iChildrenPerRow;

                double x = column * actualSize;

                child.Arrange(new Rect(x, row * this.ItemSize, this.ItemSize, this.ItemSize));
            }
        }

        #endregion

        #region IScrollInfo implementation
        // See Ben Constable's series of posts at http://blogs.msdn.com/bencon/


        private void UpdateScrollInfo(Size availableSize)
        {
            // See how many items there are
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            Size extent = CalculateExtent(availableSize, itemCount);
            // Update extent
            if (extent != _extent)
            {
                _extent = extent;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }

            // Update viewport
            if (availableSize != _viewport)
            {
                _viewport = availableSize;
                if (_owner != null)
                    _owner.InvalidateScrollInfo();
            }
        }

        public ScrollViewer ScrollOwner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool CanHorizontallyScroll
        {
            get { return _canHScroll; }
            set { _canHScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return _canVScroll; }
            set { _canVScroll = value; }
        }

        public double HorizontalOffset
        {
            get { return _offset.X; }
        }

        public double VerticalOffset
        {
            get { return _offset.Y; }
        }

        public double ExtentHeight
        {
            get { return _extent.Height; }
        }

        public double ExtentWidth
        {
            get { return _extent.Width; }
        }

        public double ViewportHeight
        {
            get { return _viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return _viewport.Width; }
        }

        public void LineUp()
        {
            SetVerticalOffset(this.VerticalOffset - ItemSize);
        }

        public void LineDown()
        {
            SetVerticalOffset(this.VerticalOffset + ItemSize);
        }

        public void PageUp()
        {
            double offset = Math.Ceiling(_viewport.Height / ItemSize) * ItemSize;
            SetVerticalOffset(this.VerticalOffset - offset);
        }

        public void PageDown()
        {
            double offset = Math.Ceiling(_viewport.Height / ItemSize) * ItemSize;
            SetVerticalOffset(this.VerticalOffset + offset);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(this.VerticalOffset - ItemSize);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(this.VerticalOffset + ItemSize + 1);
        }

        public void LineLeft()
        {
            throw new InvalidOperationException();
        }

        public void LineRight()
        {
            throw new InvalidOperationException();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public void MouseWheelLeft()
        {
            throw new InvalidOperationException();
        }

        public void MouseWheelRight()
        {
            throw new InvalidOperationException();
        }

        public void PageLeft()
        {
            throw new InvalidOperationException();
        }

        public void PageRight()
        {
            throw new InvalidOperationException();
        }

        public void SetHorizontalOffset(double offset)
        {

        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Height >= _extent.Height)
                {
                    offset = _extent.Height - _viewport.Height;
                }
            }

            offset = Math.Round(offset / this.ItemSize) * this.ItemSize;

            _offset.Y = offset;

            if (_owner != null)
                _owner.InvalidateScrollInfo();

            _trans.Y = -offset;

            // Force us to realize the correct children
            InvalidateMeasure();
        }
        protected override void BringIndexIntoView(int index)
        {
            var offset = GetOffsetForFirstVisibleIndex(index);
            SetVerticalOffset(offset.Height);
        }

        private Size GetOffsetForFirstVisibleIndex(int index)
        {
            Size offset = new Size(Math.Max(0, _offset.X), Math.Max(0, (index / iChildrenPerRow) * ItemSize));

            return offset;
        }

        public void EnsureVisible(int index)
        {
            double offsetY = Math.Max(0, (index / iChildrenPerRow) * ItemSize);
            if (offsetY + ItemSize - _offset.Y > _viewport.Height)
            {
                LineDown();
                UpdateLayout();
                UIElement container = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this).ContainerFromIndex(index) as UIElement;
                if (container != null)
                {
                    container.Focus();
                }
            }
        }

        private TranslateTransform _trans = new TranslateTransform();
        private ScrollViewer _owner;
        private bool _canHScroll = false;
        private bool _canVScroll = false;
        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset;

        #endregion

    }

}