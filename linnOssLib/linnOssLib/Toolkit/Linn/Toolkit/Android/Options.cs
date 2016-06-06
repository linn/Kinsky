using Android.Widget;
using Android.Content;
using Linn;
using System.Collections.Generic;
using System;
using Android.Views;
using Android.Runtime;
using Android.Views.Animations;
using Android.Util;
using Android.Views.InputMethods;
using System.Collections;
using System.Collections.ObjectModel;
using Android.Graphics;

namespace OssToolkitDroid
{

    #region OptionsView

    public class OptionsView : MasterDetailView
    {
        public OptionsView(Context aContext, IInvoker aInvoker, int aExpanderImageResourceId, Android.Graphics.Color aBackgroundColor, AndroidImageCache aImageCache, Bitmap aPlaceholderImage)
            : base(aContext, aBackgroundColor)
        {
            iImageCache = aImageCache;
            iPlaceholderImage = aPlaceholderImage;
            iInvoker = aInvoker;
            iExpanderImageResourceId = aExpanderImageResourceId;
            iOptionsList = new ListView(aContext);
            iOptionsList.DividerHeight = 0;
            iOptionsList.SetBackgroundColor(aBackgroundColor);
            iAdapter = new OptionListAdapter(aContext, this, iInvoker);
            iOptionsList.Adapter = iAdapter;
            MasterView = iOptionsList;
            HeaderTextStyleId = Android.Resource.Style.TextAppearanceLarge;
            MasterTitle = "Settings";
            HeaderBackgroundColor = aBackgroundColor;
            ItemBackgroundColor = aBackgroundColor;
            RequestDeleteButtonResourceId = 0;
            ConfirmDeleteButtonResourceId = 0;
        }

        public IInvoker Invoker
        {
            get
            {
                return iInvoker;
            }
        }

        public Android.Graphics.Color HeaderBackgroundColor { get; set; }

        public Android.Graphics.Color ItemBackgroundColor { get; set; }

        public int RequestDeleteButtonResourceId { get; set; }

        public int ConfirmDeleteButtonResourceId { get; set; }

        public int ExpanderImageResourceId
        {
            get
            {
                return iExpanderImageResourceId;
            }
        }
        public override void OnAnimationEnd(Animation animation)
        {
            base.OnAnimationEnd(animation);
            iAdapter.NotifyDataSetChanged();
        }

        internal void NotifyDataSetChanged()
        {
            iAdapter.NotifyDataSetChanged();
        }

        public int HeaderTextStyleId { get; set; }

        public IList<IOptionPage> OptionPages
        {
            set
            {
                Assert.Check(value != null);
                iAdapter.OptionList = value;
            }
        }

        public virtual OptionViewModel CreateViewModel(Option aOption)
        {
            if (aOption is OptionImage)
            {
                return new ImageOptionViewModel(Context, aOption, this, iImageCache, iPlaceholderImage);
            }
            else if (aOption is OptionShowView)
            {
                return new ShowViewOptionViewModel(Context, aOption, this);
            }
            else if (aOption is OptionHyperlink)
            {
                return new HyperlinkOptionViewModel(Context, aOption, this);
            }
            else if (aOption is OptionEnum || aOption is OptionNetworkInterface)
            {
                return new EnumOptionViewModel(Context, aOption, this);
            }
            else if (aOption is OptionBool)
            {
                //todo
                return new BoolOptionViewModel(Context, aOption, this);
            }
            else if (aOption is OptionListUri)
            {
                return new UriListOptionViewModel(Context, aOption, this);
            }
            else if (aOption is OptionString)
            {
                return new StringOptionViewModel(Context, aOption, this);
            }
            else
            {
                //todo: other option types
                throw new NotImplementedException();
            }
        }

        public new void Dispose()
        {
            MasterView = null;
            iOptionsList.Adapter = null;
            iOptionsList.Dispose();
            iOptionsList = null;
            iAdapter.Dispose();
            iAdapter = null;
            base.Dispose();
        }

        private ListView iOptionsList;
        private int iExpanderImageResourceId;
        private IInvoker iInvoker;
        private OptionListAdapter iAdapter;
        private Bitmap iPlaceholderImage;
        private AndroidImageCache iImageCache;
    }

    internal class OptionListAdapter : BaseAdapter
    {
        internal OptionListAdapter(Context aContext, OptionsView aParent, IInvoker aInvoker)
            : base()
        {
            iParent = aParent;
            iViewList = new List<OptionViewModel>();
            iContext = aContext;
            iInvoker = aInvoker;
        }

        public IList<IOptionPage> OptionList
        {
            set
            {
                if (iOptionPages != null)
                {
                    foreach (IOptionPage page in iOptionPages)
                    {
                        page.EventChanged -= EventChangedHander;
                        page.EventOptionAdded -= EventOptionAddedHandler;
                        page.EventOptionRemoved -= EventOptionRemovedHandler;
                    }
                }
                foreach (OptionViewModel option in iViewList)
                {
                    option.Dispose();
                }
                iViewList.Clear();

                iOptionPages = value;

                if (iOptionPages != null)
                {
                    foreach (IOptionPage page in iOptionPages)
                    {
                        iViewList.Add(new HeaderOptionViewModel(iContext, page.Name, iParent));
                        page.EventChanged += EventChangedHander;
                        page.EventOptionAdded += EventOptionAddedHandler;
                        page.EventOptionRemoved += EventOptionRemovedHandler;
                        foreach (Option option in page.Options)
                        {
                            iViewList.Add(iParent.CreateViewModel(option));
                        }
                    }
                }
                NotifyDataSetChanged();
            }
        }

        private void EventChangedHander(object sender, EventArgs e)
        {
            NotifyDataSetChanged();
        }

        private void EventOptionAddedHandler(object sender, EventArgsOption e)
        {
            NotifyDataSetChanged();
        }

        private void EventOptionRemovedHandler(object sender, EventArgsOption e)
        {
            NotifyDataSetChanged();
        }

        public override void NotifyDataSetChanged()
        {
            if (iInvoker.InvokeRequired)
            {
                iInvoker.BeginInvoke((Action)(() =>
                {
                    base.NotifyDataSetChanged();
                }));
            }
            else
            {
                base.NotifyDataSetChanged();
            }
        }


        public override int Count
        {
            get { return iViewList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int aPosition, View aConvertView, ViewGroup aParent)
        {
            View result = aConvertView;
            OptionViewModel viewModel = iViewList[aPosition];
            result = viewModel.CreateView();
            (result as IOptionView).Recycle(viewModel, iParent);
            return result;
        }

        public new void Dispose()
        {
            OptionList = null;
            base.Dispose();
        }

        private List<OptionViewModel> iViewList;
        private Context iContext;
        private OptionsView iParent;
        private IList<IOptionPage> iOptionPages;
        private IInvoker iInvoker;
    }

    public abstract class OptionViewModel
    {
        public OptionViewModel(Context aContext, Option aOption, OptionsView aParent)
        {
            iContext = aContext;
            iOption = aOption;
            if (iOption != null)
            {
                iOption.EventAllowedChanged += EventChangedHander;
                iOption.EventEnabledChanged += EventChangedHander;
                iOption.EventValueChanged += EventChangedHander;
            }
            iParent = aParent;
        }

        protected Context Context { get { return iContext; } }

        public void Dispose()
        {
            if (iOption != null)
            {
                iOption.EventAllowedChanged -= EventChangedHander;
                iOption.EventEnabledChanged -= EventChangedHander;
                iOption.EventValueChanged -= EventChangedHander;
            }
        }

        private void EventChangedHander(object sender, EventArgs e)
        {
            iParent.NotifyDataSetChanged();
        }

        public Option Option
        {
            get
            {
                return iOption;
            }
        }

        public abstract View CreateView();

        protected Context iContext;
        private Option iOption;
        protected OptionsView iParent;
    }

    public interface IOptionView
    {
        bool CanRecycle(OptionViewModel aOptionViewModel);
        void Recycle(OptionViewModel aOptionViewModel, OptionsView aParent);
        void Destroy();
    }

    public interface IListEditor<T>
    {
        event EventHandler<EventArgs> EventItemsChanged;
        IList<T> Items { get; set; }
        ListView ListView { get; }
        bool EditMode { set; }
        void Close();
    }

    public abstract class ListOptionView<T> : ExpandableOptionView
    {
        public ListOptionView(Context aContext, OptionViewModel aOptionViewModel)
            : base(aContext, aOptionViewModel)
        {
        }

        protected override void PopulateView(ViewCache aViewCache)
        {
            aViewCache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            aViewCache.FindViewById<TextView>(kValueViewId).Text = (iOptionViewModel.Option as OptionListSimple<T>).Native.Count.ToString();
        }

        protected override void OnClick()
        {
            ShowEditor();
        }

        private void ShowEditor()
        {
            iEditor = CreateEditor();
            iEditor.Items = (iOptionViewModel.Option as OptionListSimple<T>).Native;
            iParent.DetailView = iEditor.ListView;
            iParent.DetailTitle = iOptionViewModel.Option.Name;
            iParent.EditMode = true;
            iParent.ToggleView();
            iParent.EventDetailClosed += EventDetailsClosedHandler;
            iEditor.EventItemsChanged += EventItemsChangedHandler;
            iParent.EventEditButtonClicked += EventEditButtonChangedHandler;
            iOptionViewModel.Option.EventValueChanged += EventValueChangedHandler;
        }

        void EventValueChangedHandler(object sender, EventArgs e)
        {
            if (iParent.Invoker.InvokeRequired)
            {
                iParent.Invoker.BeginInvoke((Action)(() =>
                {
                    RefreshValue();
                }));
            }
            else
            {
                RefreshValue();
            }
        }

        private void RefreshValue()
        {
            if (iEditor != null)
            {
                iEditor.Items = (iOptionViewModel.Option as OptionListSimple<T>).Native;
            }
        }

        private void HideEditor()
        {
            if (iEditor != null)
            {
                iEditor.Close();
                iOptionViewModel.Option.EventValueChanged -= EventValueChangedHandler;
                iEditor.EventItemsChanged -= EventItemsChangedHandler;
                iParent.EventDetailClosed -= EventDetailsClosedHandler;
                iParent.EventEditButtonClicked -= EventEditButtonChangedHandler;
                if (!iParent.IsShowingMasterView)
                {
                    iParent.ToggleView();
                }
                iEditor = null;
            }
        }

        private void EventItemsChangedHandler(object sender, EventArgs e)
        {
            (iOptionViewModel.Option as OptionListSimple<T>).Native = iEditor.Items;
        }

        private void EventEditButtonChangedHandler(object sender, EventArgs e)
        {
            iEditor.EditMode = iParent.IsEditing;
        }

        protected abstract IListEditor<T> CreateEditor();

        private void EventDetailsClosedHandler(object sender, EventArgs e)
        {
            HideEditor();
        }

        private IListEditor<T> iEditor;
    }

    public abstract class ExpandableOptionView : RelativeLayout, IOptionView
    {
        public ExpandableOptionView(Context aContext, OptionViewModel aOptionViewModel)
            : base(aContext)
        {
            this.Click += ClickHandler;

            this.LayoutParameters = new ListView.LayoutParams(LayoutParams.FillParent, (int)aContext.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight));
        }

        private void ClickHandler(object sender, EventArgs e)
        {
            if (iOptionViewModel.Option.Enabled)
            {
                OnClick();
            }
        }

        public abstract bool CanRecycle(OptionViewModel aOptionViewModel);

        void IOptionView.Recycle(OptionViewModel aOptionViewModel, OptionsView aParent)
        {
            iParent = aParent;
            if (this.Tag == null)
            {
                this.Tag = new ViewCache(this);

                // sometimes tag doesn't come through on container recycling for some reason??
                // clear views to be on the safe side
                this.RemoveAllViews();

                CreateView(aParent);
            }

            iOptionViewModel = aOptionViewModel;
            FindViewById<ImageView>(kExpanderId).Visibility = iOptionViewModel.Option.Enabled ? ViewStates.Visible : ViewStates.Invisible;
            PopulateView(this.Tag as ViewCache);
        }

        protected virtual void CreateView(OptionsView aParent)
        {
            this.SetBackgroundColor(aParent.ItemBackgroundColor);

            LinearLayout container = new LinearLayout(Context);
            container.Id = kContainerId;

            TextView textView = new TextView(Context);
            LinearLayout.LayoutParams textViewLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            textViewLayoutParams.Gravity = GravityFlags.Left;
            textView.LayoutParameters = textViewLayoutParams;
            textView.Id = kTextViewId;
            textView.SetLines(1);
            textView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
            textView.Gravity = GravityFlags.Left;

            ImageView expander = new ImageView(Context);
            expander.Id = kExpanderId;
            expander.SetImageResource(iParent.ExpanderImageResourceId);
            RelativeLayout.LayoutParams imageViewLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            imageViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            imageViewLayoutParams.AddRule(LayoutRules.AlignParentRight);
            imageViewLayoutParams.RightMargin = 10;
            expander.LayoutParameters = imageViewLayoutParams;

            TextView valueView = new TextView(Context);
            LinearLayout.LayoutParams valueViewLayoutParams = new LinearLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.WrapContent);
            valueViewLayoutParams.Gravity = GravityFlags.Right;
            valueView.LayoutParameters = valueViewLayoutParams;
            valueView.SetLines(1);
            valueView.Id = kValueViewId;
            valueView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
            valueView.Gravity = GravityFlags.Right;


            RelativeLayout.LayoutParams containerLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.WrapContent);
            containerLayoutParams.AddRule(LayoutRules.CenterVertical);
            containerLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            containerLayoutParams.AddRule(LayoutRules.LeftOf, kExpanderId);
            containerLayoutParams.RightMargin = 10;
            containerLayoutParams.LeftMargin = 10;
            container.LayoutParameters = containerLayoutParams;

            AddView(expander);
            container.AddView(textView);
            container.AddView(valueView);
            AddView(container);
        }

        void IOptionView.Destroy()
        {
            ViewCache cache = this.Tag as ViewCache;
            if (cache != null)
            {
                DestroyView(cache as ViewCache);
                cache.Clear();
                cache.Dispose();
                this.Tag = null;
            }
            this.Click -= ClickHandler;
            iParent = null;
            this.Dispose();
        }

        protected virtual void DestroyView(ViewCache aViewCache)
        {
            aViewCache.FindViewById<TextView>(kTextViewId).Dispose();
            aViewCache.FindViewById<TextView>(kValueViewId).Dispose();
            aViewCache.FindViewById<ImageView>(kExpanderId).Dispose();
            aViewCache.FindViewById<LinearLayout>(kContainerId).Dispose();
        }

        protected abstract void PopulateView(ViewCache aViewCache);
        protected abstract void OnClick();

        protected const int kTextViewId = 1001;
        protected const int kValueViewId = 1002;
        protected const int kExpanderId = 1003;
        protected const int kContainerId = 1004;
        protected OptionViewModel iOptionViewModel;
        protected OptionsView iParent;
    }

    public abstract class StringListEditor<T> : IListEditor<T>, IAsyncLoader<string>
    {
        public StringListEditor(Context aContext, ListView aListView, int aRequestDeleteButtonResourceId, int aConfirmDeleteButtonResourceId)
        {
            iListView = aListView;
            iEditingItems = new List<string>();
            iAdapter = new StringListEditorAdapter<T>(aContext, this, aRequestDeleteButtonResourceId, aConfirmDeleteButtonResourceId);
            iListView.Adapter = iAdapter;
            iAdapter.EventItemDeleted += EventItemDeletedHandler;
        }

        public void Close()
        {
            iListView.Adapter = null;
            iAdapter.EventItemDeleted -= EventItemDeletedHandler;
            iAdapter.Close();
            iAdapter.Dispose();
            iAdapter = null;
        }

        private void EventItemDeletedHandler(object sender, EventArgsListEdit<string> e)
        {
            Assert.Check(e.Position < iEditingItems.Count - 1);
            iEditingItems.RemoveAt(e.Position);
            OnEventItemsChanged();
        }

        public bool EditMode
        {
            set
            {
                iAdapter.EditMode = value;
            }
        }

        public abstract bool IsValid(string aString);
        public abstract T ConvertFromString(string aString);
        public abstract string ConvertToString(T aValue);

        #region IListEditor<T> Members

        public event EventHandler<EventArgs> EventItemsChanged;


        public IList<T> Items
        {
            get
            {
                List<T> result = new List<T>();
                foreach (string s in iEditingItems)
                {
                    if (IsValid(s) && s != string.Empty)
                    {
                        T item = ConvertFromString(s);
                        Assert.Check(item != null);
                        result.Add(item);
                    }
                }
                return result;
            }
            set
            {
                Assert.Check(value != null);
                iOriginalItems = value;
                iEditingItems = new List<string>();
                foreach (T item in iOriginalItems)
                {
                    iEditingItems.Add(ConvertToString(item));
                }
                iEditingItems.Add(string.Empty);
                iAdapter.NotifyDataSetChanged();
            }
        }

        public ListView ListView
        {
            get { return iListView; }
        }

        #endregion


        #region IAsyncLoader<string> Members

        public event EventHandler<EventArgs> EventDataChanged;

        public string Item(int aIndex)
        {
            return iEditingItems[aIndex];
        }

        public int Count
        {
            get { return iEditingItems.Count; }
        }

        #endregion

        internal void UpdateItem(int aPosition, string aText)
        {
            Assert.Check(aPosition < iEditingItems.Count);
            Assert.Check(IsValid(aText));
            iEditingItems[aPosition] = aText;
            if (aPosition == iEditingItems.Count - 1)
            {
                iEditingItems.Add(string.Empty);
            }
            OnEventItemsChanged();
        }

        protected void OnEventItemsChanged()
        {
            EventHandler<EventArgs> del = EventItemsChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private ListView iListView;
        private StringListEditorAdapter<T> iAdapter;
        private IList<T> iOriginalItems;
        private List<string> iEditingItems;
    }

    public class StringListEditorAdapter<T> : AsyncArrayAdapter<string, string>
    {
        public StringListEditorAdapter(Context aContext, StringListEditor<T> aParent, int aRequestDeleteButtonResourceId, int aConfirmDeleteButtonResourceId)
            : base(aContext, aParent, "StringListEditorAdapter")
        {
            iParent = aParent;
            iRequestDeleteButtonResourceId = aRequestDeleteButtonResourceId;
            iConfirmDeleteButtonResourceId = aConfirmDeleteButtonResourceId;
            iLastEditPosition = -1;
        }

        public override void Close()
        {
            if (iLastEditPosition != -1 && iParent.IsValid(iLastEditText) && iParent.Item(iLastEditPosition) != iLastEditText)
            {
                iParent.UpdateItem(iLastEditPosition, iLastEditText);
            }
            base.Close();
        }

        protected override int RequestDeleteButtonResourceId
        {
            get
            {
                return iRequestDeleteButtonResourceId;
            }
        }

        protected override int ConfirmDeleteButtonResourceId
        {
            get
            {
                return iConfirmDeleteButtonResourceId;
            }
        }

        protected override bool CanDeleteItem(string aItem, int aPosition)
        {
            return aPosition != iParent.Count - 1;
        }

        protected override View CreateItemView(Context aContext, string aItem, ViewGroup aRoot)
        {
            RelativeLayout result = new RelativeLayout(aContext);

            result.LayoutParameters = new ListView.LayoutParams(ListView.LayoutParams.FillParent, (int)aContext.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight));

            EditText text = new EditText(aContext);
            text.ImeOptions = ImeAction.Done;
            text.SetImeActionLabel("Done", ImeAction.Done);
            text.SetSingleLine(true);
            text.Id = kEditViewId;
            text.SetSelectAllOnFocus(true);
            text.FocusChange += FocusChangeHandler;
            text.EditorAction += EditorActionHandler;
            text.TextChanged += TextChangedHandler;
            RelativeLayout.LayoutParams textViewLayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
            textViewLayoutParameters.AddRule(LayoutRules.CenterVertical);
            text.LayoutParameters = textViewLayoutParameters;
            result.AddView(text);
            return result;
        }

        protected override void DestroyItemView(Context aContext, ViewCache aViewCache)
        {
            EditText text = aViewCache.FindViewById<EditText>(kEditViewId);
            text.EditorAction -= EditorActionHandler;
            text.TextChanged -= TextChangedHandler;
            text.FocusChange -= FocusChangeHandler;
        }

        protected override void RecycleItemView(Context aContext, string aItem, ViewCache aViewCache)
        {
            EditText text = aViewCache.FindViewById<EditText>(kEditViewId);
            text.TextChanged -= TextChangedHandler;
            text.Text = iParent.Item(aViewCache.Position);
            text.TextChanged += TextChangedHandler;
            text.Tag = aViewCache.Position;
        }

        private void FocusChangeHandler(object sender, View.FocusChangeEventArgs e)
        {
            EditText text = sender as EditText;
            if (!e.HasFocus)
            {
                int position = (int)text.Tag;
                if (!iParent.IsValid(text.Text))
                {
                    // reset text on lost focus
                    text.Text = iParent.Item(position);
                }
                else
                {
                    iParent.UpdateItem(position, text.Text);
                }
            }
        }

        private void TextChangedHandler(object sender, Android.Text.TextChangedEventArgs e)
        {
            EditText text = sender as EditText;
            int position = (int)text.Tag;
            iLastEditPosition = position;
            iLastEditText = text.Text;
        }

        private void EditorActionHandler(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                EditText text = sender as EditText;
                int position = (int)text.Tag;
                if (iParent.IsValid(text.Text))
                {
                    iParent.UpdateItem(position, text.Text);
                }
            }
        }

        private const int kEditViewId = 1001;
        private StringListEditor<T> iParent;
        private int iRequestDeleteButtonResourceId;
        private int iConfirmDeleteButtonResourceId;
        private string iLastEditText;
        private int iLastEditPosition;
    }

    public class OptionImage : OptionEnum
    {
        public OptionImage(string aId, string aName, string aDescription) : base(aId, aName, aDescription) { }
    }
    #endregion

    #region HeaderOptionView

    public class HeaderOptionViewModel : OptionViewModel
    {
        public HeaderOptionViewModel(Context aContext, string aName, OptionsView aParent)
            : base(aContext, null, aParent)
        {
            iName = aName;
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        private string iName;

        public override View CreateView()
        {
            return new HeaderOptionView(iContext);
        }
    }

    public class HeaderOptionView : RelativeLayout, IOptionView
    {
        public HeaderOptionView(Context aContext)
            : base(aContext)
        {
            this.LayoutParameters = new ListView.LayoutParams(LayoutParams.FillParent, (int)aContext.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight));
        }

        bool IOptionView.CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is HeaderOptionViewModel;
        }

        void IOptionView.Recycle(OptionViewModel aOptionViewModel, OptionsView aParent)
        {
            Assert.Check(aOptionViewModel is HeaderOptionViewModel);
            if (this.Tag == null)
            {
                this.Tag = new ViewCache(this);
                // sometimes tag doesn't come through on container recycling for some reason??
                // clear views to be on the safe side
                this.RemoveAllViews();

                this.SetBackgroundColor(aParent.HeaderBackgroundColor);

                TextView textView = new TextView(Context, null, aParent.HeaderTextStyleId);
                textView.Id = kTextViewId;
                textView.Text = (aOptionViewModel as HeaderOptionViewModel).Name;
                RelativeLayout.LayoutParams textViewLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                textViewLayoutParams.AddRule(LayoutRules.AlignParentBottom);
                textViewLayoutParams.BottomMargin = 10;
                textView.LayoutParameters = textViewLayoutParams;
                AddView(textView);
            }
            ViewCache cache = this.Tag as ViewCache;
            cache.FindViewById<TextView>(kTextViewId).Text = (aOptionViewModel as HeaderOptionViewModel).Name;
        }

        void IOptionView.Destroy()
        {
            ViewCache cache = this.Tag as ViewCache;
            if (cache != null)
            {
                cache.FindViewById<TextView>(kTextViewId).Dispose();
                cache.Clear();
                cache.Dispose();
                this.Tag = null;
            }
            this.Dispose();
        }
        private const int kTextViewId = 1001;
    }

    #endregion

    #region BoolOptionView

    public class BoolOptionViewModel : OptionViewModel
    {
        public BoolOptionViewModel(Context aContext, Option aOption, OptionsView aParent) : base(aContext, aOption, aParent) { }
        public override View CreateView()
        {
            return new BoolOptionView(iContext, this);
        }
    }
    public class BoolOptionView : RelativeLayout, IOptionView
    {
        public BoolOptionView(Context aContext, OptionViewModel aOptionViewModel)
            : base(aContext)
        {
        }

        void IOptionView.Recycle(OptionViewModel aOptionViewModel, OptionsView aParent)
        {
            iParent = aParent;
            if (this.Tag == null)
            {
                this.Tag = new ViewCache(this);

                // sometimes tag doesn't come through on container recycling for some reason??
                // clear views to be on the safe side
                this.RemoveAllViews();

                this.SetBackgroundColor(aParent.ItemBackgroundColor);

                TextView textView = new TextView(Context);
                textView.Id = kTextViewId;
                textView.Text = aOptionViewModel.Option.Name;
                textView.SetLines(1);
                textView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;

                RelativeLayout.LayoutParams textViewLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                textViewLayoutParams.AddRule(LayoutRules.CenterVertical);
                textViewLayoutParams.AddRule(LayoutRules.AlignParentLeft);
                textViewLayoutParams.AddRule(LayoutRules.LeftOf, kCheckBoxId);
                textViewLayoutParams.LeftMargin = 10;
                textView.LayoutParameters = textViewLayoutParams;

                AddView(textView);

                CheckBox checkBox = new CheckBox(Context);
                checkBox.Id = kCheckBoxId;
                RelativeLayout.LayoutParams checkBoxLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                checkBoxLayoutParams.AddRule(LayoutRules.CenterVertical);
                checkBoxLayoutParams.AddRule(LayoutRules.AlignParentRight);
                checkBoxLayoutParams.RightMargin = 10;
                checkBox.LayoutParameters = checkBoxLayoutParams;
                checkBox.CheckedChange += CheckedChangeHandler;
                AddView(checkBox);

            }

            iOptionViewModel = aOptionViewModel;

            ViewCache cache = this.Tag as ViewCache;
            cache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            CheckBox chk = cache.FindViewById<CheckBox>(kCheckBoxId);
            chk.Enabled = iOptionViewModel.Option.Enabled;
            chk.Checked = (iOptionViewModel.Option as OptionBool).Native;
        }

        void IOptionView.Destroy()
        {
            ViewCache cache = this.Tag as ViewCache;
            if (cache != null)
            {
                cache.FindViewById<TextView>(kTextViewId).Dispose();
                cache.FindViewById<CheckBox>(kCheckBoxId).Dispose();
                cache.FindViewById<CheckBox>(kCheckBoxId).CheckedChange -= CheckedChangeHandler;
                cache.Clear();
                cache.Dispose();
                this.Tag = null;
            }
            iParent = null;
            this.Dispose();
        }

        bool IOptionView.CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is BoolOptionViewModel;
        }

        private void CheckedChangeHandler(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            (iOptionViewModel.Option as OptionBool).Native = e.IsChecked;
        }

        private OptionViewModel iOptionViewModel;
        private const int kTextViewId = 1001;
        private const int kCheckBoxId = 1002;
        private OptionsView iParent;
    }

    #endregion

    #region EnumOptionView

    public class EnumOptionViewModel : OptionViewModel
    {
        public EnumOptionViewModel(Context aContext, Option aOption, OptionsView aParent) : base(aContext, aOption, aParent) { }
        public override View CreateView()
        {
            return new EnumOptionView(iContext, this);
        }
    }
    public class EnumOptionView : ExpandableOptionView
    {
        public EnumOptionView(Context aContext, OptionViewModel aOptionViewModel)
            : base(aContext, aOptionViewModel)
        {
        }

        protected override void PopulateView(ViewCache aViewCache)
        {
            Assert.Check(iOptionViewModel is EnumOptionViewModel);
            aViewCache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            string value = iOptionViewModel.Option.Value;
            if (iOptionViewModel.Option is OptionNetworkInterface)
            {
                List<NetworkInfoModel> nics = NetworkInfo.GetAllNetworkInterfaces();
                foreach (NetworkInfoModel nic in nics)
                {
                    if (nic.Name == iOptionViewModel.Option.Value)
                    {
                        value = nic.Description;
                    }
                }
            }
            aViewCache.FindViewById<TextView>(kValueViewId).Text = value;
        }

        protected override void OnClick()
        {
            ShowEditor();
        }

        private void ShowEditor()
        {
            iOptionViewModel.Option.EventAllowedChanged += EventAllowedChangedHandler;
            iListView = new ListView(Context);
            SetAdapter();
            iListView.ItemClick += ItemClickHandler;
            iParent.EventDetailClosed += EventDetailClosedHandler;
            iParent.DetailView = iListView;
            iParent.DetailTitle = iOptionViewModel.Option.Name;
            iParent.EditMode = false;
            iParent.ToggleView();
        }

        private void ItemClickHandler(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            string value = iListView.Adapter.GetItem(e.Position).ToString();
            if (iOptionViewModel.Option is OptionNetworkInterface)
            {
                List<NetworkInfoModel> nics = NetworkInfo.GetAllNetworkInterfaces();
                foreach (NetworkInfoModel nic in nics)
                {
                    if (nic.Description == value)
                    {
                        value = nic.Name;
                    }
                }
            }
            iOptionViewModel.Option.Set(value);
            HideEditor();
        }

        private void EventDetailClosedHandler(object sender, EventArgs e)
        {
            HideEditor();
        }

        private void HideEditor()
        {
            iParent.EventDetailClosed -= EventDetailClosedHandler;
            iOptionViewModel.Option.EventAllowedChanged -= EventAllowedChangedHandler;
            iListView.ItemClick -= ItemClickHandler;
            iParent.DetailView = null;
            iListView.Adapter.Dispose();
            iListView.Adapter = null;
        }

        private void EventAllowedChangedHandler(object sender, EventArgs e)
        {
            if (iParent.Invoker.InvokeRequired)
            {
                iParent.Invoker.BeginInvoke((Action)(() =>
                {
                    SetAdapter();
                }));
            }
            else
            {
                SetAdapter();
            }
        }

        private void SetAdapter()
        {
            if (iOptionViewModel.Option is OptionNetworkInterface)
            {
                List<string> allowed = new List<string>(iOptionViewModel.Option.Allowed);
                List<NetworkInfoModel> nics = NetworkInfo.GetAllNetworkInterfaces();
                for (int i = 0; i < allowed.Count; i++)
                {
                    foreach (NetworkInfoModel nic in nics)
                    {
                        if (nic.Name == allowed[i])
                        {
                            allowed[i] = nic.Description;
                        }
                    }
                }
                iListView.Adapter = new EnumOptionListAdapter(Context, allowed);
            }
            else
            {
                iListView.Adapter = new EnumOptionListAdapter(Context, new List<string>(iOptionViewModel.Option.Allowed));
            }
        }

        public override bool CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is EnumOptionViewModel;
        }

        private ListView iListView;
    }

    public class EnumOptionListAdapter : ArrayAdapter
    {
        public EnumOptionListAdapter(Context aContext, IList aOptions) : base(aContext, 0, aOptions) { }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            RelativeLayout result = convertView as RelativeLayout;
            if (result != null)
            {
                result.FindViewById<TextView>(kTextViewId).Text = this.GetItem(position).ToString();
            }
            else
            {

                result = new RelativeLayout(Context);

                result.LayoutParameters = new ListView.LayoutParams(ListView.LayoutParams.FillParent, (int)Context.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight));

                TextView text = new TextView(Context);
                text.Id = kTextViewId;
                text.Text = this.GetItem(position).ToString();
                RelativeLayout.LayoutParams textViewLayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                textViewLayoutParameters.AddRule(LayoutRules.CenterVertical);
                text.LayoutParameters = textViewLayoutParameters;
                result.AddView(text);
            }
            return result;
        }

        private const int kTextViewId = 1001;
    }

    #endregion

    #region ImageOptionView

    public class ImageOptionViewModel : OptionViewModel
    {
        public ImageOptionViewModel(Context aContext, Option aOption, OptionsView aParent, AndroidImageCache aImageCache, Bitmap aPlaceholderImage)
            : base(aContext, aOption, aParent)
        {
            iImageCache = aImageCache;
            iPlaceholderImage = aPlaceholderImage;
        }
        public override View CreateView()
        {
            return new ImageOptionView(iContext, this, iImageCache, iPlaceholderImage);
        }
        private AndroidImageCache iImageCache;
        private Bitmap iPlaceholderImage;
    }
    public class ImageOptionView : ExpandableOptionView
    {
        public ImageOptionView(Context aContext, OptionViewModel aOptionViewModel, AndroidImageCache aImageCache, Bitmap aPlaceholderImage)
            : base(aContext, aOptionViewModel)
        {
            iImageCache = aImageCache;
            iPlaceholderImage = aPlaceholderImage;
        }

        protected override void CreateView(OptionsView aParent)
        {
            this.SetBackgroundColor(aParent.ItemBackgroundColor);

            TextView textView = new TextView(Context);
            textView.Id = kTextViewId;
            textView.SetLines(1);
            textView.Ellipsize = Android.Text.TextUtils.TruncateAt.End;

            RelativeLayout.LayoutParams textViewLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            textViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            textViewLayoutParams.AddRule(LayoutRules.AlignParentLeft);
            textViewLayoutParams.AddRule(LayoutRules.LeftOf, kValueViewId);
            textViewLayoutParams.LeftMargin = 10;
            textView.LayoutParameters = textViewLayoutParams;

            AddView(textView);

            ImageView expander = new ImageView(Context);
            expander.Id = kExpanderId;
            expander.SetImageResource(iParent.ExpanderImageResourceId);
            RelativeLayout.LayoutParams imageViewLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            imageViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            imageViewLayoutParams.AddRule(LayoutRules.AlignParentRight);
            imageViewLayoutParams.RightMargin = 10;
            expander.LayoutParameters = imageViewLayoutParams;
            AddView(expander);

            LazyLoadingImageView valueView = new LazyLoadingImageView(Context);
            valueView.Id = kValueViewId;

            RelativeLayout.LayoutParams valueViewLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            valueViewLayoutParams.AddRule(LayoutRules.CenterVertical);
            valueViewLayoutParams.AddRule(LayoutRules.LeftOf, kExpanderId);
            valueViewLayoutParams.SetMargins(0, 10, 0, 10);
            valueView.LayoutParameters = valueViewLayoutParams;

            AddView(valueView);
        }

        protected override void DestroyView(ViewCache aViewCache)
        {
            aViewCache.FindViewById<TextView>(kTextViewId).Dispose();
            aViewCache.FindViewById<LazyLoadingImageView>(kValueViewId).Dispose();
            aViewCache.FindViewById<ImageView>(kExpanderId).Dispose();
        }

        protected override void PopulateView(ViewCache aViewCache)
        {
            Assert.Check(iOptionViewModel is ImageOptionViewModel);
            aViewCache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            string value = iOptionViewModel.Option.Value;
            LazyLoadingImageView imageView = aViewCache.FindViewById<LazyLoadingImageView>(kValueViewId);
            imageView.SetImageBitmap(iPlaceholderImage);
            Uri uriOut = null;
            if (value != string.Empty && Uri.TryCreate(value, UriKind.Absolute, out uriOut))
            {
                imageView.LoadImage(iImageCache, uriOut);
            }
        }

        protected override void OnClick()
        {
            ShowEditor();
        }

        private void ShowEditor()
        {
            iOptionViewModel.Option.EventAllowedChanged += EventAllowedChangedHandler;
            iListView = new ListView(Context);
            SetAdapter();
            iListView.ItemClick += ItemClickHandler;
            iParent.EventDetailClosed += EventDetailClosedHandler;
            iParent.DetailView = iListView;
            iParent.DetailTitle = iOptionViewModel.Option.Name;
            iParent.EditMode = false;
            iParent.ToggleView();
        }

        private void ItemClickHandler(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            string value = iListView.Adapter.GetItem(e.Position).ToString();
            iOptionViewModel.Option.Set(value);
            HideEditor();
        }

        private void EventDetailClosedHandler(object sender, EventArgs e)
        {
            HideEditor();
        }

        private void HideEditor()
        {
            iParent.EventDetailClosed -= EventDetailClosedHandler;
            iOptionViewModel.Option.EventAllowedChanged -= EventAllowedChangedHandler;
            iListView.ItemClick -= ItemClickHandler;
            iListView.Adapter.Dispose();
            iListView.Adapter = null;
            iParent.DetailView = null;
        }

        private void EventAllowedChangedHandler(object sender, EventArgs e)
        {
            if (iParent.Invoker.InvokeRequired)
            {
                iParent.Invoker.BeginInvoke((Action)(() =>
                {
                    SetAdapter();
                }));
            }
            else
            {
                SetAdapter();
            }
        }

        private void SetAdapter()
        {
            iListView.Adapter = new ImageOptionListAdapter(Context, new List<string>(iOptionViewModel.Option.Allowed), iImageCache, iPlaceholderImage);
        }

        public override bool CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is ImageOptionViewModel;
        }

        private ListView iListView;
        private AndroidImageCache iImageCache;
        private Bitmap iPlaceholderImage;
    }

    public class ImageOptionListAdapter : ArrayAdapter
    {
        public ImageOptionListAdapter(Context aContext, IList aOptions, AndroidImageCache aImageCache, Bitmap aPlaceholderImage)
            : base(aContext, 0, aOptions)
        {
            iImageCache = aImageCache;
            iPlaceholderImage = aPlaceholderImage;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            RelativeLayout result = convertView as RelativeLayout;
            LazyLoadingImageView imageView;
            if (result != null)
            {
                imageView = result.FindViewById<LazyLoadingImageView>(kImageViewId);
            }
            else
            {

                result = new RelativeLayout(Context);

                result.LayoutParameters = new ListView.LayoutParams(ListView.LayoutParams.FillParent, (int)Context.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight));

                imageView = new LazyLoadingImageView(Context);
                imageView.Id = kImageViewId;

                RelativeLayout.LayoutParams imageViewLayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                imageViewLayoutParameters.AddRule(LayoutRules.CenterVertical);
                imageViewLayoutParameters.SetMargins(0, 10, 0, 10);
                imageView.LayoutParameters = imageViewLayoutParameters;
                result.AddView(imageView);
            }
            imageView.SetImageBitmap(iPlaceholderImage);
            Uri outUri = null;
            if (Uri.TryCreate(this.GetItem(position).ToString(), UriKind.Absolute, out outUri))
            {
                imageView.LoadImage(iImageCache, outUri);
            }
            return result;
        }

        private const int kImageViewId = 1001;
        private AndroidImageCache iImageCache;
        private Bitmap iPlaceholderImage;
    }

    #endregion

    #region UriListOptionViewModel

    public class UriListOptionViewModel : OptionViewModel
    {
        public UriListOptionViewModel(Context aContext, Option aOption, OptionsView aParent) : base(aContext, aOption, aParent) { }
        public override View CreateView()
        {
            return new UriListOptionView(iContext, this, iParent.RequestDeleteButtonResourceId, iParent.ConfirmDeleteButtonResourceId);
        }
    }

    public class UriListOptionView : ListOptionView<Uri>
    {
        public UriListOptionView(Context aContext, OptionViewModel aOptionViewModel, int aRequestDeleteButtonResourceId, int aConfirmDeleteButtonResourceId)
            : base(aContext, aOptionViewModel)
        {
            iRequestDeleteButtonResourceId = aRequestDeleteButtonResourceId;
            iConfirmDeleteButtonResourceId = aConfirmDeleteButtonResourceId;
        }

        protected override IListEditor<Uri> CreateEditor()
        {
            return new UriListEditor(Context, kUriKind, new ListView(Context), iRequestDeleteButtonResourceId, iConfirmDeleteButtonResourceId);
        }

        public override bool CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is UriListOptionViewModel;
        }

        private static UriKind kUriKind = UriKind.Absolute;
        private int iRequestDeleteButtonResourceId, iConfirmDeleteButtonResourceId;
    }

    public class UriListEditor : StringListEditor<Uri>
    {

        public UriListEditor(Context aContext, UriKind aUriKind, ListView aListView, int aRequestDeleteButtonResourceId, int aConfirmDeleteButtonResourceId)
            : base(aContext, aListView, aRequestDeleteButtonResourceId, aConfirmDeleteButtonResourceId)
        {
            iUriKind = aUriKind;
        }

        public override bool IsValid(string aString)
        {
            Uri outUri;
            return Uri.TryCreate(aString, iUriKind, out outUri);
        }

        public override Uri ConvertFromString(string aString)
        {
            Uri outUri;
            if (!Uri.TryCreate(aString, iUriKind, out outUri))
            {
                Assert.Check(false);
            }
            return outUri;
        }

        public override string ConvertToString(Uri aUri)
        {
            Assert.Check(aUri != null);
            return aUri.OriginalString;
        }

        private UriKind iUriKind;
    }

    #endregion

    #region StringOptionView

    public class StringOptionViewModel : OptionViewModel
    {
        public StringOptionViewModel(Context aContext, Option aOption, OptionsView aParent) : base(aContext, aOption, aParent) { }
        public override View CreateView()
        {
            return new StringOptionView(iContext, this);
        }
    }

    public class StringOptionView : ExpandableOptionView
    {
        public StringOptionView(Context aContext, StringOptionViewModel aOptionViewModel)
            : base(aContext, aOptionViewModel)
        {
        }

        protected override void PopulateView(ViewCache aViewCache)
        {
            Assert.Check(iOptionViewModel is StringOptionViewModel);
            aViewCache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            aViewCache.FindViewById<TextView>(kValueViewId).Text = iOptionViewModel.Option.Value;
        }

        protected override void OnClick()
        {
            ShowEditor();
        }

        void EditorActionHandler(object sender, TextView.EditorActionEventArgs e)
        {
            // user clicked done
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
            {
                HideEditor();
            }
        }

        public override bool CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is StringOptionViewModel;
        }

        private void ShowEditor()
        {
            iEditorContainer = new RelativeLayout(Context);
            iEditor = new EditText(Context) { Text = iOptionViewModel.Option.Value };
            iEditor.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.WrapContent);
            iEditor.SetSelectAllOnFocus(true);
            iEditor.ImeOptions = ImeAction.Done;
            iEditor.SetImeActionLabel("Done", ImeAction.Done);
            iEditor.SetSingleLine(true);
            iEditorContainer.AddView(iEditor);
            iParent.DetailView = iEditorContainer;
            iParent.DetailTitle = iOptionViewModel.Option.Name;
            iParent.EditMode = false;
            iParent.ToggleView();
            iEditor.RequestFocus();
            iEditor.EditorAction += EditorActionHandler;
            iParent.EventDetailClosed += EventDetailsClosedHandler;
        }

        private void HideEditor()
        {
            if (iEditor != null)
            {
                iEditor.EditorAction -= EditorActionHandler;
                string newText = iEditor.Text;
                if (newText != iOptionViewModel.Option.Value)
                {
                    iOptionViewModel.Option.Set(newText);
                }
                InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(iParent.WindowToken, HideSoftInputFlags.None);
                iParent.EventDetailClosed -= EventDetailsClosedHandler;
                if (!iParent.IsShowingMasterView)
                {
                    iParent.ToggleView();
                }
                iEditorContainer.RemoveAllViews();
                iEditor.Dispose();
                iEditor = null;
                iEditorContainer.Dispose();
                iEditorContainer = null;
                iParent.DetailView = null;
            }
        }

        private void EventDetailsClosedHandler(object sender, EventArgs e)
        {
            HideEditor();
        }

        private EditText iEditor;
        private RelativeLayout iEditorContainer;
    }

    #endregion


    #region ShowViewOptionView

    public class OptionShowView : Option
    {

        public OptionShowView(View aView, string aId, string aName, string aDescription)
            : base(aId, aName, aDescription)
        {
            iView = aView;
        }

        public View View
        {
            get
            {
                return iView;
            }
        }

        public override void ResetToDefault()
        {
            throw new NotImplementedException();
        }

        public override string Value
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Set(string aValue)
        {
            throw new NotImplementedException();
        }

        public override event EventHandler<EventArgs> EventValueChanged;

        public override event EventHandler<EventArgs> EventAllowedChanged;

        private View iView;
    }

    public class ShowViewOptionViewModel : OptionViewModel
    {
        public ShowViewOptionViewModel(Context aContext, Option aOption, OptionsView aParent) : base(aContext, aOption, aParent) { }
        public override View CreateView()
        {
            return new ShowViewOptionView(iContext, this);
        }
    }

    public class ShowViewOptionView : ExpandableOptionView
    {
        public ShowViewOptionView(Context aContext, OptionViewModel aOptionViewModel)
            : base(aContext, aOptionViewModel)
        {
        }

        public override bool CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is ShowViewOptionViewModel;
        }

        protected override void PopulateView(ViewCache aViewCache)
        {
            aViewCache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            aViewCache.FindViewById<TextView>(kValueViewId).Text = iOptionViewModel.Option.Description;
        }

        protected override void OnClick()
        {
            iParent.DetailTitle = iOptionViewModel.Option.Name;
            iParent.DetailView = (iOptionViewModel.Option as OptionShowView).View;
            iParent.EditMode = false;
            if (iParent.IsShowingMasterView)
            {
                iParent.ToggleView();
            }
        }
    }

    public class HyperlinkOptionViewModel : OptionViewModel
    {
        public HyperlinkOptionViewModel(Context aContext, Option aOption, OptionsView aParent) : base(aContext, aOption, aParent) { }
        public override View CreateView()
        {
            return new HyperlinkOptionView(iContext, this);
        }
    }

    #endregion

    #region HyperlinkOptionViewModel

    public class OptionHyperlink : Option
    {
        public OptionHyperlink(string aHyperLink, string aId, string aName, string aDescription)
            : base(aId, aName, aDescription)
        {
            iHyperlink = aHyperLink;
        }

        public override void ResetToDefault()
        {
            throw new NotImplementedException();
        }

        public override string Value
        {
            get { return iHyperlink; }
        }

        public override bool Set(string aValue)
        {
            throw new NotImplementedException();
        }

        public override event EventHandler<EventArgs> EventValueChanged;

        public override event EventHandler<EventArgs> EventAllowedChanged;

        private string iHyperlink;
    }

    public class HyperlinkOptionView : ExpandableOptionView
    {
        public HyperlinkOptionView(Context aContext, OptionViewModel aOptionViewModel)
            : base(aContext, aOptionViewModel)
        {
        }

        public override bool CanRecycle(OptionViewModel aOptionViewModel)
        {
            return aOptionViewModel is HyperlinkOptionViewModel;
        }

        protected override void PopulateView(ViewCache aViewCache)
        {
            aViewCache.FindViewById<TextView>(kTextViewId).Text = iOptionViewModel.Option.Name;
            aViewCache.FindViewById<TextView>(kValueViewId).Text = iOptionViewModel.Option.Description;
        }

        protected override void OnClick()
        {
            try
            {
                Intent intent = new Intent(Intent.ActionView);
                intent.SetData(Android.Net.Uri.Parse(iOptionViewModel.Option.Value));
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                UserLog.WriteLine("Failed to launch browser for link: " + iOptionViewModel.Option.Value);
                UserLog.WriteLine("Error: " + ex);
                Toast.MakeText(Context, "Could not launch web browser.", ToastLength.Long).Show();
            }
        }
    }

    #endregion

    #region HelpAboutOptionPage

    public class HelpAboutOptionPage : IOptionPage
    {
        public HelpAboutOptionPage(View aAboutView, string aManualLink)
        {
            iOptions = new List<Option>();
            if (aAboutView != null)
            {
                iOptions.Add(new OptionShowView(aAboutView, string.Empty, "About", string.Empty));
            }
            if (aManualLink != string.Empty)
            {
                iOptions.Add(new OptionHyperlink(aManualLink, string.Empty, "View Manual", string.Empty));
            }
        }

        #region IOptionPage Members

        public string Name
        {
            get { return "Help"; }
        }

        public ReadOnlyCollection<Option> Options
        {
            get { return iOptions.AsReadOnly(); }
        }

        public void Insert(int aIndex, Option aOption)
        {
            throw new NotImplementedException();
        }

        public void Remove(Option aOption)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<EventArgsOption> EventOptionAdded;

        public event EventHandler<EventArgsOption> EventOptionRemoved;

        public event EventHandler<EventArgs> EventChanged;

        #endregion
        private List<Option> iOptions;
    }


    #endregion
}