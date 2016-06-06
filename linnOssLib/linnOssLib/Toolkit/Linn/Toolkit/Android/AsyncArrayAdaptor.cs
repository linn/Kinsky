using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using Android.Views;
using System;
using System.Linq;
using Linn;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
namespace OssToolkitDroid
{

    public class EventArgsListEdit<T> : EventArgs
    {
        public EventArgsListEdit(T aItem, int aPosition)
            : base()
        {
            Item = aItem;
        }
        public T Item { get; set; }
        public int Position { get; set; }
    }


    public abstract class AsyncArrayAdapter<ItemType, SectionHeaderType> : BaseAdapter
    {

        public event EventHandler<EventArgsListEdit<ItemType>> EventItemDeleted;
        public event EventHandler<EventArgsListEdit<SectionHeaderType>> EventSectionDeleted;
        public event EventHandler<EventArgsListEdit<ItemType>> EventItemMovedUp;
        public event EventHandler<EventArgsListEdit<SectionHeaderType>> EventSectionMovedUp;
        public event EventHandler<EventArgsListEdit<ItemType>> EventItemMovedDown;
        public event EventHandler<EventArgsListEdit<SectionHeaderType>> EventSectionMovedDown;

        public AsyncArrayAdapter(Context aContext, IAsyncLoader<ItemType> aLoader, string aId)
            : base()
        {
            iContext = aContext;
            iSectionHeaders = new List<SectionHeader<SectionHeaderType>>();
            iLoader = aLoader;
            iLoader.EventDataChanged += iLoader_DataChanged;
            iDisconnectedSectionHeaderViews = new List<View>();
            iDisconnectedItemViews = new List<View>();
            iDeleteIndex = -1;
            EditMode = false;
            iViews = new List<RecyclingContainerView>();
            iId = aId;
        }

        public virtual void Clear()
        {
            foreach (RecyclingContainerView view in iViews)
            {
                DisconnectContainer(view);
                ViewCache cache = view.Tag as ViewCache;
                view.Tag = null;
                view.EventRequestDeleteClick -= EventRequestDeleteHandler;
                view.EventConfirmDeleteClick -= EventConfirmDeleteHandler;
                view.EventMoveUpClick -= EventMoveUpHandler;
                view.EventMoveDownClick -= EventMoveDownHandler;
                view.Dispose();
                cache.Close();
                cache.Dispose();
            }
            foreach (View v in iDisconnectedSectionHeaderViews)
            {
                v.Dispose();
            }
            iDisconnectedSectionHeaderViews.Clear();
            foreach (View v in iDisconnectedItemViews)
            {
                v.Dispose();
            }
            iDisconnectedItemViews.Clear();

            iViews.Clear();
        }

        public virtual void Close()
        {
            Clear();
            iLoader.EventDataChanged -= iLoader_DataChanged;
        }

        protected virtual View CreateSectionHeaderView(Context aContext, SectionHeaderType aSectionHeader, ViewGroup aRoot)
        {
            return new View(aContext);
        }
        protected virtual View CreateItemView(Context aContext, ItemType aItem, ViewGroup aRoot)
        {
            return new View(aContext);
        }
        protected virtual void RecycleSectionHeaderView(Context aContext, SectionHeaderType aSectionHeader, ViewCache aViewCache) { }
        protected virtual void RecycleItemView(Context aContext, ItemType aItem, ViewCache aViewCache) { }
        protected virtual void DestroySectionHeaderView(Context aContext, ViewCache aViewCache) { }
        protected virtual void DestroyItemView(Context aContext, ViewCache aViewCache) { }
        protected virtual bool CanMoveItemUp(ItemType aItem, int aPosition) { return false; }
        protected virtual bool CanMoveSectionUp(SectionHeaderType aSection, int aPosition) { return false; }
        protected virtual bool CanMoveItemDown(ItemType aItem, int aPosition) { return false; }
        protected virtual bool CanMoveSectionDown(SectionHeaderType aSection, int aPosition) { return false; }
        protected virtual bool CanDeleteItem(ItemType aItem, int aPosition) { return false; }
        protected virtual bool CanDeleteSection(SectionHeaderType aSection, int aPosition) { return false; }
        protected virtual int RequestDeleteButtonResourceId { get { return 0; } }
        protected virtual int ConfirmDeleteButtonResourceId { get { return 0; } }
        protected virtual int MoveDownButtonResourceId { get { return 0; } }
        protected virtual int MoveUpButtonResourceId { get { return 0; } }

        public object this[int aPosition]
        {
            get
            {
                SectionHeader<SectionHeaderType> header = GetSectionHeader(aPosition);
                if (header != null)
                {
                    // user has clicked on a header
                    return header.Header;
                }
                // user has clicked on an item, return it if loaded, or null if not
                int loaderPosition = GetLoaderPosition(aPosition);
                return iLoader.Item(loaderPosition);
            }
        }

        private void iLoader_DataChanged(object sender, EventArgs e)
        {
            iDeleteIndex = -1;
            NotifyDataSetChanged();
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public void SetSectionHeaders(List<SectionHeader<SectionHeaderType>> aSectionHeaders)
        {
            if (aSectionHeaders == null) throw new ArgumentNullException();
            iSectionHeaders = aSectionHeaders;
            this.NotifyDataSetChanged();
        }

        public override int Count
        {
            get
            {
                return iLoader.Count + iSectionHeaders.Count;
            }
        }

        public bool EditMode
        {
            get
            {
                return iEditMode;
            }
            set
            {
                iEditMode = value;
                if (!iEditMode)
                {
                    iDeleteIndex = -1;
                }
                NotifyDataSetChanged();
            }
        }

        public override View GetView(int aPosition, View aConvertView, ViewGroup aParent)
        {
            RecyclingContainerView root = aConvertView as RecyclingContainerView;
            if (root == null)
            {
                root = CreateContainerView(iContext);
            }
            else if (root.HasResurfacedWithoutJNIHandle)
            {
                UserLog.WriteLine("Ticket #1194: Item has resurfaced after dispose! - " + iId);
                Assert.Check(false);
            }
            ViewCache cache = root.Tag as ViewCache;
            Assert.Check(cache != null);
            cache.Position = aPosition;
            SectionHeader<SectionHeaderType> header = GetSectionHeader(aPosition);
            ItemType item = default(ItemType);
            bool isPlaceholder = false;
            if (header != null)
            {
                SetHeader(root, header);
            }
            else
            {
                int loaderPosition = GetLoaderPosition(aPosition);
                item = iLoader.Item(loaderPosition);

                if (item != null)
                {
                    SetItem(root, item);
                }
                else
                {
                    SetItem(root, default(ItemType));
                    isPlaceholder = true;
                }
            }
            if (iEditMode && !isPlaceholder)
            {
                root.EditMode = aPosition == iDeleteIndex ? EItemEditMode.ConfirmDelete : EItemEditMode.Editing;
                if (header != null)
                {
                    root.CanDelete = CanDeleteSection(header.Header, aPosition);
                    root.CanMoveUp = CanMoveSectionUp(header.Header, aPosition);
                    root.CanMoveDown = CanMoveSectionDown(header.Header, aPosition);
                }
                else
                {
                    root.CanDelete = CanDeleteItem(item, aPosition);
                    root.CanMoveUp = CanMoveItemUp(item, aPosition);
                    root.CanMoveDown = CanMoveItemDown(item, aPosition);
                }
            }
            else
            {
                root.EditMode = EItemEditMode.None;
            }
            root.IsFirst = aPosition == 0;
            root.IsLast = aPosition == Count - 1;
            return root;
        }

        private void SetHeader(RecyclingContainerView aRoot, SectionHeader<SectionHeaderType> aHeader)
        {
            ViewCache cache = aRoot.Tag as ViewCache;
            Assert.Check(cache != null);
            if (cache.ChildType == EChildViewType.Header)
            {
                RecycleSectionHeaderView(iContext, aHeader.Header, cache);
            }
            else
            {
                DisconnectContainer(aRoot);
                if (iDisconnectedSectionHeaderViews.Count > 0)
                {
                    View recycledView = iDisconnectedSectionHeaderViews[0];
                    iDisconnectedSectionHeaderViews.RemoveAt(0);
                    aRoot.Content = recycledView;
                    RecycleSectionHeaderView(iContext, aHeader.Header, cache);
                }
                else
                {
                    int childCount = aRoot.ChildCount;
                    View view = CreateSectionHeaderView(iContext, aHeader.Header, aRoot);
                    Assert.Check(aRoot.ChildCount == childCount);
                    Assert.Check(view != null);
                    aRoot.Content = view;
                    RecycleSectionHeaderView(iContext, aHeader.Header, cache);
                }
                cache.ChildType = EChildViewType.Header;
            }
        }

        private void SetItem(RecyclingContainerView aRoot, ItemType aItem)
        {
            ViewCache cache = aRoot.Tag as ViewCache;
            Assert.Check(cache != null);
            if (cache.ChildType == EChildViewType.Item)
            {
                RecycleItemView(iContext, aItem, cache);
            }
            else
            {
                DisconnectContainer(aRoot);
                if (iDisconnectedItemViews.Count > 0)
                {
                    View recycledView = iDisconnectedItemViews[0];
                    iDisconnectedItemViews.RemoveAt(0);
                    aRoot.Content = recycledView;
                    RecycleItemView(iContext, aItem, cache);
                }
                else
                {
                    int childCount = aRoot.ChildCount;
                    View view = CreateItemView(iContext, aItem, aRoot);
                    Assert.Check(aRoot.ChildCount == childCount);
                    Assert.Check(view != null);
                    aRoot.Content = view;
                    RecycleItemView(iContext, aItem, cache);
                }
                cache.ChildType = EChildViewType.Item;
            }
        }

        private void DisconnectContainer(RecyclingContainerView aRoot)
        {
            ViewCache cache = aRoot.Tag as ViewCache;
            Assert.Check(cache != null);
            View disconnectedChild = aRoot.Content;
            switch (cache.ChildType)
            {
                case EChildViewType.Header:
                    {
                        if (iDisconnectedSectionHeaderViews.Count < kMaxDisconnectedViews)
                        {
                            iDisconnectedSectionHeaderViews.Add(disconnectedChild);
                        }
                        else
                        {
                            DestroySectionHeaderView(iContext, cache);
                            disconnectedChild.Dispose();
                            disconnectedChild = null;
                        }
                        break;
                    }
                case EChildViewType.Item:
                    {
                        if (iDisconnectedItemViews.Count < kMaxDisconnectedViews)
                        {
                            iDisconnectedItemViews.Add(disconnectedChild);
                        }
                        else
                        {
                            DestroyItemView(iContext, cache);
                            disconnectedChild.Dispose();
                            disconnectedChild = null;
                        }
                        break;
                    }
                default:
                    {
                        Assert.Check(false);
                        break;
                    }
            }
            aRoot.Content = null;
            cache.Clear();
        }

        private RecyclingContainerView CreateContainerView(Context aContext)
        {
            RecyclingContainerView result = new RecyclingContainerView(aContext, RequestDeleteButtonResourceId, ConfirmDeleteButtonResourceId, MoveDownButtonResourceId, MoveUpButtonResourceId);
            using (ListView.LayoutParams layoutParams = new ListView.LayoutParams(ListView.LayoutParams.FillParent, ListView.LayoutParams.WrapContent))
            {
                result.LayoutParameters = layoutParams;
            }
            ViewCache cache = new ViewCache(result);
            result.Tag = cache;
            View view = CreateItemView(aContext, default(ItemType), result);
            result.EditMode = EItemEditMode.None;
            Assert.Check(view != null);
            result.Content = view;
            result.EventRequestDeleteClick += EventRequestDeleteHandler;
            result.EventConfirmDeleteClick += EventConfirmDeleteHandler;
            result.EventMoveUpClick += EventMoveUpHandler;
            result.EventMoveDownClick += EventMoveDownHandler;
            iViews.Add(result);
            return result;
        }

        private void EventRequestDeleteHandler(object sender, EventArgs args)
        {
            RecyclingContainerView senderView = sender as RecyclingContainerView;
            ViewCache cache = senderView.Tag as ViewCache;
            Assert.Check(cache != null);
            if (cache.Position == iDeleteIndex)
            {
                iDeleteIndex = -1;
            }
            else
            {
                iDeleteIndex = cache.Position;
            }
            NotifyDataSetChanged();
        }

        private void EventConfirmDeleteHandler(object sender, EventArgs args)
        {
            RecyclingContainerView senderView = sender as RecyclingContainerView;
            ViewCache cache = senderView.Tag as ViewCache;
            Assert.Check(cache != null);
            int position = cache.Position;
            SectionHeader<SectionHeaderType> header = GetSectionHeader(position);
            if (header != null)
            {
                OnEventSectionDeleted(header.Header, position);
            }
            else
            {
                int loaderPosition = GetLoaderPosition(position);
                ItemType item = iLoader.Item(loaderPosition);
                Assert.Check(item != null);
                OnEventItemDeleted(item, position);
            }
            iDeleteIndex = -1;
        }

        private void EventMoveUpHandler(object sender, EventArgs args)
        {
            RecyclingContainerView senderView = sender as RecyclingContainerView;
            ViewCache cache = senderView.Tag as ViewCache;
            Assert.Check(cache != null);
            int position = cache.Position;
            SectionHeader<SectionHeaderType> header = GetSectionHeader(position);
            if (header != null)
            {
                OnEventSectionMovedUp(header.Header, position);
            }
            else
            {
                int loaderPosition = GetLoaderPosition(position);
                ItemType item = iLoader.Item(loaderPosition);
                Assert.Check(item != null);
                OnEventItemMovedUp(item, position);
            }
        }

        private void EventMoveDownHandler(object sender, EventArgs args)
        {
            RecyclingContainerView senderView = sender as RecyclingContainerView;
            ViewCache cache = senderView.Tag as ViewCache;
            Assert.Check(cache != null);
            int position = cache.Position;
            SectionHeader<SectionHeaderType> header = GetSectionHeader(position);
            if (header != null)
            {
                OnEventSectionMovedDown(header.Header, position);
            }
            else
            {
                int loaderPosition = GetLoaderPosition(position);
                ItemType item = iLoader.Item(loaderPosition);
                Assert.Check(item != null);
                OnEventItemMovedDown(item, position);
            }
        }


        private void OnEventItemDeleted(ItemType aItem, int aPosition)
        {
            EventHandler<EventArgsListEdit<ItemType>> eventDeleted = EventItemDeleted;
            if (eventDeleted != null)
            {
                eventDeleted(this, new EventArgsListEdit<ItemType>(aItem, aPosition));
            }
        }

        private void OnEventSectionDeleted(SectionHeaderType aSection, int aPosition)
        {
            EventHandler<EventArgsListEdit<SectionHeaderType>> eventDeleted = EventSectionDeleted;
            if (eventDeleted != null)
            {
                eventDeleted(this, new EventArgsListEdit<SectionHeaderType>(aSection, aPosition));
            }
        }

        private void OnEventItemMovedUp(ItemType aItem, int aPosition)
        {
            EventHandler<EventArgsListEdit<ItemType>> eventMovedUp = EventItemMovedUp;
            if (eventMovedUp != null)
            {
                eventMovedUp(this, new EventArgsListEdit<ItemType>(aItem, aPosition));
            }
        }

        private void OnEventSectionMovedUp(SectionHeaderType aSection, int aPosition)
        {
            EventHandler<EventArgsListEdit<SectionHeaderType>> eventMovedUp = EventSectionMovedUp;
            if (eventMovedUp != null)
            {
                eventMovedUp(this, new EventArgsListEdit<SectionHeaderType>(aSection, aPosition));
            }
        }

        private void OnEventItemMovedDown(ItemType aItem, int aPosition)
        {
            EventHandler<EventArgsListEdit<ItemType>> eventMovedDown = EventItemMovedDown;
            if (eventMovedDown != null)
            {
                eventMovedDown(this, new EventArgsListEdit<ItemType>(aItem, aPosition));
            }
        }

        private void OnEventSectionMovedDown(SectionHeaderType aSection, int aPosition)
        {
            EventHandler<EventArgsListEdit<SectionHeaderType>> eventMovedDown = EventSectionMovedDown;
            if (eventMovedDown != null)
            {
                eventMovedDown(this, new EventArgsListEdit<SectionHeaderType>(aSection, aPosition));
            }
        }

        public SectionHeader<SectionHeaderType> GetSectionHeader(int aPosition)
        {
            Assert.Check(aPosition < Count);
            int counter = 0;
            foreach (SectionHeader<SectionHeaderType> idx in iSectionHeaders)
            {
                if (idx.Index + counter == aPosition)
                {
                    return idx;
                }
                else if (idx.Index + counter < aPosition)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }
            return null;
        }

        public int GetLoaderPosition(int aPosition)
        {
            Assert.Check(aPosition < Count);
            int counter = 0;
            foreach (SectionHeader<SectionHeaderType> idx in iSectionHeaders)
            {
                if (idx.Index + counter == aPosition)
                {
                    Assert.Check(false);
                }
                else if (idx.Index + counter < aPosition)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }
            return aPosition - counter;
        }

        public int GetActualPosition(int aLoaderPosition)
        {
            Assert.Check(aLoaderPosition < iLoader.Count);
            int position = aLoaderPosition;
            foreach (SectionHeader<SectionHeaderType> idx in iSectionHeaders)
            {
                if (idx.Index <= position)
                {
                    position++;
                }
                else
                {
                    break;
                }
            }
            return position;
        }

        protected LayoutInflater LayoutInflater
        {
            get
            {
                return (LayoutInflater)iContext.GetSystemService(Context.LayoutInflaterService);
            }
        }

        protected Context iContext;
        private IAsyncLoader<ItemType> iLoader;
        private List<SectionHeader<SectionHeaderType>> iSectionHeaders;
        private List<View> iDisconnectedSectionHeaderViews;
        private List<View> iDisconnectedItemViews;
        private const int kMaxDisconnectedViews = 20;
        private int iDeleteIndex;
        private bool iEditMode;
        private List<RecyclingContainerView> iViews;
        private string iId;
    }

    public enum EItemEditMode
    {
        None,
        Editing,
        ConfirmDelete
    }

    
    public class RecyclingContainerView : LinearLayout
    {

        public RecyclingContainerView(IntPtr aJavaRef, JniHandleOwnership aTranserOwnership) 
            : base(aJavaRef, aTranserOwnership)
        {
            iHasResurfacedWithoutJNIHandle = true;
        }

        public bool HasResurfacedWithoutJNIHandle
        {
            get
            {
                return iHasResurfacedWithoutJNIHandle;
            }
        }

        public event EventHandler<EventArgs> EventRequestDeleteClick;
        public event EventHandler<EventArgs> EventConfirmDeleteClick;
        public event EventHandler<EventArgs> EventMoveUpClick;
        public event EventHandler<EventArgs> EventMoveDownClick;

        public RecyclingContainerView(Context aContext,
            int aRequestDeleteButtonResourceId,
            int aConfirmDeleteButtonResourceId,
            int aMoveDownButtonResourceId,
            int aMoveUpButtonResourceId)
            : base(aContext)
        {
            iRequestDeleteButtonResourceId = aRequestDeleteButtonResourceId;
            iConfirmDeleteButtonResourceId = aConfirmDeleteButtonResourceId;
            iMoveDownButtonResourceId = aMoveDownButtonResourceId;
            iMoveUpButtonResourceId = aMoveUpButtonResourceId;
            this.Orientation = Android.Widget.Orientation.Horizontal;
            EditMode = EItemEditMode.None;
        }

        private void CreateControls()
        {
            RemoveAllViews();

            if (EditMode == EItemEditMode.None)
            {
                if (iRequestDeleteButton != null)
                {
                    iRequestDeleteButton.Click -= RequestDeleteClickHandler;
                    iRequestDeleteButton.Dispose();
                    iRequestDeleteButton = null;
                }
                if (iConfirmDeleteButton != null)
                {
                    iConfirmDeleteButton.Click -= ConfirmDeleteClickHandler;
                    iConfirmDeleteButton.Dispose();
                    iConfirmDeleteButton = null;
                }
                if (iMoveUpButton != null)
                {
                    iMoveUpButton.Click -= MoveUpButtonClickHandler;
                    iMoveUpButton.Dispose();
                    iMoveUpButton = null;
                }
                if (iMoveDownButton != null)
                {
                    iMoveDownButton.Click -= MoveDownButtonClickHandler;
                    iMoveDownButton.Dispose();
                    iMoveDownButton = null;
                }
                if (iContentPlaceholder != null)
                {
                    iContentPlaceholder.Dispose();
                    iContentPlaceholder = null;
                }
            }
            else{
                iRequestDeleteButton = CreateRequestDeleteButton();
                iRequestDeleteButton.Click += RequestDeleteClickHandler;
                using (LinearLayout.LayoutParams requestDeleteButtonLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent))
                {
                    requestDeleteButtonLayoutParams.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                    iRequestDeleteButton.LayoutParameters = requestDeleteButtonLayoutParams;
                    requestDeleteButtonLayoutParams.LeftMargin = 5;
                }
                AddView(iRequestDeleteButton);


                iContentPlaceholder = new LinearLayout(Context);
                using (LinearLayout.LayoutParams contentPlaceholderLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent))
                {
                    contentPlaceholderLayoutParams.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                    contentPlaceholderLayoutParams.Weight = 1;
                    iContentPlaceholder.LayoutParameters = contentPlaceholderLayoutParams;
                }
                AddView(iContentPlaceholder);

                iConfirmDeleteButton = CreateConfirmDeleteButton();
                iConfirmDeleteButton.Click += ConfirmDeleteClickHandler;
                using (LinearLayout.LayoutParams confirmDeleteButtonLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent))
                {
                    confirmDeleteButtonLayoutParams.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                    confirmDeleteButtonLayoutParams.RightMargin = 5;
                    iConfirmDeleteButton.LayoutParameters = confirmDeleteButtonLayoutParams;
                }
                AddView(iConfirmDeleteButton);

                iMoveUpButton = CreateMoveUpButton();
                iMoveUpButton.Click += MoveUpButtonClickHandler;
                using (LinearLayout.LayoutParams moveUpButtonLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent))
                {
                    moveUpButtonLayoutParams.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                    moveUpButtonLayoutParams.RightMargin = 5;
                    iMoveUpButton.LayoutParameters = moveUpButtonLayoutParams;
                }
                AddView(iMoveUpButton);

                iMoveDownButton = CreateMoveDownButton();
                iMoveDownButton.Click += MoveDownButtonClickHandler;
                using (LinearLayout.LayoutParams moveDownButtonLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent))
                {
                    moveDownButtonLayoutParams.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                    moveDownButtonLayoutParams.RightMargin = 5;
                    iMoveDownButton.LayoutParameters = moveDownButtonLayoutParams;
                }
                AddView(iMoveDownButton);
            }
        }

        private View CreateMoveUpButton()
        {
            View moveUpButton;
            if (iMoveUpButtonResourceId == 0)
            {
                moveUpButton = new Button(Context);
                (moveUpButton as Button).Text = "Up";
            }
            else
            {
                moveUpButton = LayoutInflater.FromContext(Context).Inflate(iMoveUpButtonResourceId, null);
            }
            moveUpButton.Id = kMoveUpButtonId;
            return moveUpButton;
        }

        private View CreateMoveDownButton()
        {
            View moveDownButton;
            if (iMoveDownButtonResourceId == 0)
            {
                moveDownButton = new Button(Context);
                (moveDownButton as Button).Text = "Down";
            }
            else
            {
                moveDownButton = LayoutInflater.FromContext(Context).Inflate(iMoveDownButtonResourceId, null);
            }
            moveDownButton.Id = kMoveDownButtonId;
            return moveDownButton;
        }


        private View CreateRequestDeleteButton()
        {
            View requestDeleteButton;
            if (iRequestDeleteButtonResourceId == 0)
            {
                requestDeleteButton = new Button(Context);
                (requestDeleteButton as Button).Text = "Delete";
            }
            else
            {
                requestDeleteButton = LayoutInflater.FromContext(Context).Inflate(iRequestDeleteButtonResourceId, null);
            }
            requestDeleteButton.Id = kRequestDeleteButtonId;
            return requestDeleteButton;
        }

        private View CreateConfirmDeleteButton()
        {
            View confirmDeleteButton;
            if (iConfirmDeleteButtonResourceId == 0)
            {
                confirmDeleteButton = new Button(Context);
                (confirmDeleteButton as Button).Text = "Confirm";
            }
            else
            {
                confirmDeleteButton = LayoutInflater.FromContext(Context).Inflate(iConfirmDeleteButtonResourceId, null);
            }
            confirmDeleteButton.Id = kConfirmDeleteButtonId;
            return confirmDeleteButton;
        }


        private void MoveUpButtonClickHandler(object sender, EventArgs e)
        {
            OnEventMoveUp();
        }

        private void MoveDownButtonClickHandler(object sender, EventArgs e)
        {
            OnEventMoveDown();
        }

        private void RequestDeleteClickHandler(object sender, EventArgs e)
        {
            OnEventRequestDelete();
        }

        private void ConfirmDeleteClickHandler(object sender, EventArgs e)
        {
            OnEventConfirmDelete();
        }

        public EItemEditMode EditMode
        {
            get
            {
                return iEditMode;
            }
            set
            {
                bool changed = iEditMode != value;
                EItemEditMode previous = iEditMode;
                iEditMode = value;
                if (changed)
                {
                    if (iContent != null)
                    {
                        RemoveContent(iContent, previous != EItemEditMode.None);
                    }
                    CreateControls(); 
                    if (iContent != null)
                    {
                        AddContent(iContent, iEditMode != EItemEditMode.None);
                    }
                    SetButtonState();
                }
            }
        }

        private void SetButtonState()
        {
            if (iEditMode != EItemEditMode.None)
            {
                iRequestDeleteButton.Visibility = !iCanDelete ? ViewStates.Gone : ViewStates.Visible;
                iConfirmDeleteButton.Visibility = iEditMode == EItemEditMode.ConfirmDelete && iCanDelete ? ViewStates.Visible : ViewStates.Gone;
                iMoveUpButton.Visibility = iEditMode == EItemEditMode.ConfirmDelete ? ViewStates.Gone : !iCanMoveUp ? ViewStates.Invisible : ViewStates.Visible;
                iMoveDownButton.Visibility = iEditMode == EItemEditMode.ConfirmDelete ? ViewStates.Gone : !iCanMoveDown ? ViewStates.Invisible : ViewStates.Visible;
                iRequestDeleteButton.Selected = iEditMode != EItemEditMode.Editing;
                iMoveUpButton.Enabled = !iIsFirst;
                iMoveDownButton.Enabled = !iIsLast;
            }
        }

        public bool CanDelete
        {
            set
            {
                iCanDelete = value;
                SetButtonState();
            }
        }

        public bool CanMoveUp
        {
            set
            {
                iCanMoveUp = value;
                SetButtonState();
            }
        }

        public bool CanMoveDown
        {
            set
            {
                iCanMoveDown = value;
                SetButtonState();
            }
        }

        public bool IsFirst
        {
            set
            {
                iIsFirst = value;
                SetButtonState();
            }
        }

        public bool IsLast
        {
            set
            {
                iIsLast = value;
                SetButtonState();
            }
        }


        private void OnEventMoveUp()
        {
            EventHandler<EventArgs> eventClick = EventMoveUpClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }

        private void OnEventMoveDown()
        {
            EventHandler<EventArgs> eventClick = EventMoveDownClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }

        private void OnEventRequestDelete()
        {
            EventHandler<EventArgs> eventClick = EventRequestDeleteClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }


        private void OnEventConfirmDelete()
        {
            EventHandler<EventArgs> eventClick = EventConfirmDeleteClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }

        internal View Content
        {
            get
            {
                return iContent;
            }
            set
            {
                if (iContent != null)
                {
                    RemoveContent(iContent, EditMode != EItemEditMode.None);
                }
                iContent = value;
                if (iContent != null)
                {
                    AddContent(iContent, EditMode != EItemEditMode.None);
                }
            }
        }

        private void AddContent(View aContent, bool aPlaceholder)
        {
            if (aPlaceholder)
            {
                iContentPlaceholder.AddView(aContent);
            }
            else
            {
                AddView(aContent);
            }
        }

        private void RemoveContent(View aContent, bool aPlaceholder)
        {
            if (aPlaceholder)
            {
                iContentPlaceholder.RemoveView(aContent);
            }
            else
            {
                RemoveView(aContent);
            }
        }

        private EItemEditMode iEditMode;
        private bool iCanMoveUp;
        private bool iCanMoveDown;
        private bool iIsFirst;
        private bool iIsLast;
        private bool iCanDelete;

        private View iRequestDeleteButton;
        private View iConfirmDeleteButton;
        private View iMoveUpButton;
        private View iMoveDownButton;
        private LinearLayout iContentPlaceholder;
        private const int kRequestDeleteButtonId = 1000;
        private const int kConfirmDeleteButtonId = 1001;
        private const int kMoveUpButtonId = 1002;
        private const int kMoveDownButtonId = 1003;
        private View iContent;

        private int iRequestDeleteButtonResourceId = 0;
        private int iConfirmDeleteButtonResourceId = 0;
        private int iMoveDownButtonResourceId = 0;
        private int iMoveUpButtonResourceId = 0;
        private bool iHasResurfacedWithoutJNIHandle = false;
    }
	
    public class ViewCache : Java.Lang.Object
    {

        public ViewCache(View aBaseView)
        {
            iBaseView = aBaseView;
            iChildViews = new Dictionary<int, View>();
            ChildType = EChildViewType.Item;
        }

        public int Position { get; set; }

        protected View BaseView
        {
            get
            {
                return iBaseView;
            }
        }

        public T FindViewById<T>(int aViewId) where T : View
        {
            if (!iChildViews.ContainsKey(aViewId))
            {
                iChildViews[aViewId] = iBaseView.FindViewById<T>(aViewId);
            }
            T result = iChildViews[aViewId] as T;
            return result;
        }

        internal void Close()
        {
            foreach (View v in iChildViews.Values)
            {
                v.Dispose();
            }
            Clear();
        }

        internal void Clear()
        {
            iChildViews.Clear();
        }

        internal EChildViewType ChildType { get; set; }

        private View iBaseView;
        private Dictionary<int, View> iChildViews;
    }

    public enum EChildViewType
    {
        Header,
        Item
    }

    public interface IAsyncLoader<T>
    {
        event EventHandler<EventArgs> EventDataChanged;
        T Item(int aIndex);
        int Count { get; }
    }

    public class SectionHeader<SectionHeaderType>
    {
        public SectionHeader(int aIndex, SectionHeaderType aHeader)
        {
            iIndex = aIndex;
            iHeader = aHeader;
        }

        public int Index
        {
            get
            {
                return iIndex;
            }
        }

        public SectionHeaderType Header
        {
            get
            {
                return iHeader;
            }
        }

        private int iIndex;
        private SectionHeaderType iHeader;
    }

    public class FlingStateManager
    {
        public FlingStateManager()
        {
            iFlingingLists = new List<AbsListView>();
        }

        public event EventHandler<EventArgs> EventFlingStateChanged;

        public bool IsFlinging()
        {
            lock (iFlingingLists)
            {
                return iFlingingLists.Count != 0;
            }
        }

        private void OnEventFlingStateChanged()
        {
            EventHandler<EventArgs> del = EventFlingStateChanged;
            if (del != null)
            {
                del(this, new EventArgs());
            }
        }

        public void SetFlinging(AbsListView aListView, bool aFlinging)
        {
            bool stateChanged = false;
            lock (iFlingingLists)
            {
                bool wasFlinging = iFlingingLists.Count != 0;
                if (aFlinging && !iFlingingLists.Contains(aListView))
                {
                    iFlingingLists.Add(aListView);
                }
                else if (!aFlinging && iFlingingLists.Contains(aListView))
                {
                    iFlingingLists.Remove(aListView);
                }
                bool isFlinging = iFlingingLists.Count != 0;
                if (wasFlinging && !isFlinging)
                {
                    stateChanged = true;
                }
                else if (!wasFlinging && isFlinging)
                {
                    stateChanged = true;
                }
            }
            if (stateChanged)
            {
                OnEventFlingStateChanged();
            }
        }

        private List<AbsListView> iFlingingLists;
    }

    public class FlingScrollListener : Java.Lang.Object, Android.Widget.AbsListView.IOnScrollListener
    {

        public FlingScrollListener(FlingStateManager aFlingStateManager)
            : base()
        {
            iLock = new object();
            iFlingStateManager = aFlingStateManager;
            iVisibleScrollRange = ScrollRange.Undefined;
            iScrollState = ScrollState.Idle;
        }

        public ScrollRange VisibleScrollRange
        {
            get
            {
                lock (iLock)
                {
                    return iVisibleScrollRange;
                }
            }
        }

        #region IOnScrollListener Members

        public void OnScroll(AbsListView aView, int aFirstVisibleItem, int aVisibleItemCount, int aTotalItemCount)
        {
            lock (iLock)
            {
                iVisibleScrollRange = new ScrollRange(aFirstVisibleItem, aFirstVisibleItem + aVisibleItemCount);
            }
        }

        public void OnScrollStateChanged(AbsListView aView, ScrollState aScrollState)
        {
            lock (iLock)
            {
                iScrollState = aScrollState;
                iFlingStateManager.SetFlinging(aView, aScrollState != ScrollState.Idle);
            }
        }

        #endregion

        private FlingStateManager iFlingStateManager;
        private ScrollRange iVisibleScrollRange;
        private object iLock;
        private ScrollState iScrollState;

        public sealed class ScrollRange
        {
            internal ScrollRange(int aStartIndex, int aEndIndex)
            {
                StartIndex = aStartIndex;
                EndIndex = aEndIndex;
            }

            public bool Contains(int aIndex)
            {
                return aIndex >= StartIndex && aIndex <= EndIndex;
            }

            public int StartIndex;
            public int EndIndex;
            internal const int kUndefined = ~0;
            public static ScrollRange Undefined = new ScrollRange(kUndefined, kUndefined);
        }
    }

}