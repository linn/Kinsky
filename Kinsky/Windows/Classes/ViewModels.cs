using System.Windows;
using System.ComponentModel;
using Linn.Topology;
using System.Collections.ObjectModel;
using Linn;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Linn.Kinsky;
using System.Windows.Media;
using Upnp;
using System.Runtime.InteropServices;

namespace KinskyDesktopWpf
{

    #region ListViewModelBase
    public class ListViewModelBase : INotifyPropertyChanged
    {

        private bool iIsSelected;
        private string iName;
        private bool iIsEditing;

        public ListViewModelBase(string aName, bool aIsSelected)
        {
            iName = aName;
            iIsSelected = aIsSelected;
            iIsEditing = false;
        }


        public virtual bool IsSelected
        {
            get { return iIsSelected; }
            set { iIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public virtual string Name
        {
            get { return iName; }
            set { iName = value; OnPropertyChanged("Name"); }
        }

        public virtual void OnPropertyChanged(string aPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(aPropertyName));
            }
        }

        public bool IsEditing { get { return iIsEditing; } set { iIsEditing = value; OnPropertyChanged("IsEditing"); } }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
    #endregion

    #region ListViewModel<T>
    public class ListViewModel<T> : ListViewModelBase, IImageFetch
    {
        private T iWrappedItem;
        private WeakReference iImageSource;

        public ListViewModel(T aWrappedItem)
            : base(string.Empty, false)
        {
            iWrappedItem = aWrappedItem;
        }

        public ListViewModel(string aName, T aWrappedItem, bool aIsSelected)
            : base(aName, aIsSelected)
        {
            iWrappedItem = aWrappedItem;
        }

        public bool RequiresImageFetch
        {
            get
            {
                return iImageSource == null || !iImageSource.IsAlive;
            }
        }

        public void FetchImage()
        {
            Loader.Load(Resolver.Resolve(iWrappedItem), (s) =>
            {
                ImageSource = s;
            });
        }

        protected IconResolver Resolver
        {
            get
            {
                return KinskyDesktop.Instance.IconResolver;
            }
        }

        protected WpfImageCache Loader
        {
            get
            {
                return KinskyDesktop.Instance.ImageCache;
            }
        }

        public virtual T WrappedItem
        {
            get { return iWrappedItem; }
            set { iWrappedItem = value; OnPropertyChanged("WrappedItem"); }
        }

        public virtual ImageSource ImageSource
        {
            get
            {
                if (RequiresImageFetch)
                {
                    iImageSource = new WeakReference(StaticImages.ImageSourceIconLoading);
                    FetchImage();
                }
                return iImageSource.Target as ImageSource;
            }
            set
            {
                if (value != null)
                {
                    iImageSource = new WeakReference(value);
                }
                else
                {
                    iImageSource = null;
                }
                OnPropertyChanged("ImageSource");
            }
        }
    }
    #endregion

    #region PlaylistItemBase
    public class PlaylistItemBase : ListViewModel<MrItem>, IPlaylistItem
    {
        private int iPosition;
        private bool iIsPlaying;
        public PlaylistItemBase()
            : base(null, null, false)
        {
        }
        public int Position
        {
            get { return iPosition; }
            set { iPosition = value; OnPropertyChanged("Position"); }
        }

        public bool IsPlaying
        {
            get { return iIsPlaying; }
            set { iIsPlaying = value; OnPropertyChanged("IsPlaying"); }
        }

    }
    #endregion

    #region PlaylistListItem
    public class PlaylistListItem : PlaylistItemBase { }
    #endregion

    #region CollapsedPlaylistListItem
    public class CollapsedPlaylistListItem : PlaylistListItem { }
    #endregion

    #region RadioListItem
    public class RadioListItem : PlaylistItemBase { }
    #endregion

    #region PlaylistGroupHeaderItem
    public class PlaylistGroupHeaderItem : PlaylistItemBase { }
    #endregion

    #region SenderListItem
    public class SenderListItem : PlaylistItemBase
    {
        private ModelSender iSender;
        private WeakReference iImageSource;
        private bool iHasRoom;
        public ModelSender Sender
        {
            get { return iSender; }
            set { iSender = value; OnPropertyChanged("ImageSource"); }
        }

        public bool HasRoom
        {
            get { return iHasRoom; }
            set { iHasRoom = value; OnPropertyChanged("HasRoom"); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                if (iImageSource == null || !iImageSource.IsAlive)
                {
                    iImageSource = new WeakReference(StaticImages.ImageSourceIconLoading);
                    Loader.Load(Resolver.Resolve(Sender), (s) =>
                    {
                        ImageSource = s;
                    });
                }
                return iImageSource.Target as ImageSource;
            }
            set
            {
                if (value != null)
                {
                    iImageSource = new WeakReference(value);
                }
                else
                {
                    iImageSource = null;
                }
                OnPropertyChanged("ImageSource");
            }
        }

    }
    #endregion

    #region BrowserItem
    public class BrowserItem : ListViewModel<upnpObject>
    {
        public BrowserItem(upnpObject aUpnpObject, upnpObject aParent)
            : base(null, aUpnpObject, false)
        {
            ItemInfo info = new ItemInfo(aUpnpObject, aParent);
            KeyValuePair<string, string>? item = info.DisplayItem(0);
            iDisplayField1 = item.HasValue ? item.Value.Value : string.Empty;
            item = info.DisplayItem(1);
            iDisplayField2 = item.HasValue ? item.Value.Value : string.Empty;
            item = info.DisplayItem(2);
            iDisplayField3 = item.HasValue ? item.Value.Value : string.Empty;
            item = info.DisplayItem(3);
            iDisplayField4 = item.HasValue ? item.Value.Value : string.Empty;
            item = info.DisplayItem(4);
            iDisplayField5 = item.HasValue ? item.Value.Value : string.Empty;

        }

        public string DisplayField1
        {
            get { return iDisplayField1; }
        }
        public string DisplayField2
        {
            get { return iDisplayField2; }
        }
        public string DisplayField3
        {
            get { return iDisplayField3; }
        }
        public string DisplayField4
        {
            get { return iDisplayField4; }
        }
        public string DisplayField5
        {
            get { return iDisplayField5; }
        }
        private string iDisplayField1;
        private string iDisplayField2;
        private string iDisplayField3;
        private string iDisplayField4;
        private string iDisplayField5;
    }
    #endregion

    #region PlaceholderBrowserItem
    public class PlaceholderBrowserItem : BrowserItem
    {
        public PlaceholderBrowserItem()
            : base(null, null)
        {
            WrappedItem = new Upnp.item();
            WrappedItem.Title = "";
            ImageSource = KinskyDesktopWpf.StaticImages.ImageSourceIconLoading;
        }


        public override ImageSource ImageSource
        {
            get
            {
                return KinskyDesktopWpf.StaticImages.ImageSourceIconLoading;
            }
            set
            {
            }
        }
    }
    #endregion

    #region RoomViewModel
    public class RoomViewModel : ListViewModel<Linn.Kinsky.Room>
    {
        public RoomViewModel(string aName, bool aIsSelected, Linn.Kinsky.Room aWrappedItem)
            : base(aName, aWrappedItem, aIsSelected)
        {
        }

    }
    #endregion

    #region SourceViewModel
    public class SourceViewModel : ListViewModel<Linn.Kinsky.Source>
    {
        public SourceViewModel(string aName, bool aIsSelected, Linn.Kinsky.Source aWrappedItem)
            : base(aName, aWrappedItem, aIsSelected)
        {
        }

    }
    #endregion

    #region BookmarkViewModel
    public class BookmarkViewModel : ListViewModel<Linn.Kinsky.Bookmark>
    {
        public BookmarkViewModel(Linn.Kinsky.Bookmark aWrappedItem)
            : base(aWrappedItem)
        {
        }

    }
    #endregion

    #region HierarchicalViewModel
    public class HierarchicalViewModel : INotifyPropertyChanged
    {

        static readonly HierarchicalViewModel iLazyLoadPlaceHolder = new HierarchicalViewModel();

        readonly ObservableCollection<HierarchicalViewModel> iChildren;
        readonly HierarchicalViewModel iParent;

        bool iIsExpanded;
        bool iIsSelected;

        protected HierarchicalViewModel(HierarchicalViewModel aParent)
        {
            iParent = aParent;

            iChildren = new ObservableCollection<HierarchicalViewModel>();

            iChildren.Add(iLazyLoadPlaceHolder);
        }
        private HierarchicalViewModel()
        {
        }
        public ObservableCollection<HierarchicalViewModel> Children
        {
            get
            {
                return iChildren;
            }
        }
        public bool HasLazyLoadedChildren
        {
            get
            {
                return Children.Count != 1 || Children[0] != iLazyLoadPlaceHolder;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return iIsExpanded;
            }
            set
            {
                if (value != iIsExpanded)
                {
                    iIsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                if (iIsExpanded && iParent != null)
                    iParent.IsExpanded = true;

                if (!HasLazyLoadedChildren)
                {
                    Children.Remove(iLazyLoadPlaceHolder);
                    LoadChildren();
                }
            }
        }
        public bool IsSelected
        {
            get
            {
                return iIsSelected;
            }
            set
            {
                if (value != iIsSelected)
                {
                    iIsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        protected virtual void LoadChildren()
        {
        }


        public HierarchicalViewModel Parent
        {
            get
            {
                return iParent;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
    #endregion

    #region OptionViewModelFactory
    public static class OptionViewModelFactory
    {
        public static OptionViewModel Create(Option aOption)
        {
            if (aOption is OptionEnum || aOption is OptionNetworkInterface)
            {
                return new EnumOptionViewModel(aOption);
            }
            else if (aOption is OptionBool)
            {
                return new BoolOptionViewModel(aOption);
            }
            else if (aOption is OptionListFolderPath)
            {
                return new FolderListOptionViewModel(aOption as OptionListString);
            }
            else if (aOption is OptionListUri)
            {
                return new UriListOptionViewModel(aOption as OptionListUri);
            }
            else if (aOption is OptionFolderPath)
            {
                return new FolderOptionViewModel(aOption);
            }
            else if (aOption is OptionFilePath)
            {
                return new FileOptionViewModel(aOption);
            }
            else if (aOption is OptionColor)
            {
                return new ColourOptionViewModel(aOption);
            }
            return new OptionViewModel(aOption);
        }
    }
    #endregion

    #region OptionPageViewModel
    public class OptionPageViewModel : HierarchicalViewModel
    {
        public OptionPageViewModel(HierarchicalViewModel aParent, IOptionPage aWrappedOptionPage, Dispatcher aDispatcher)
            : base(aParent)
        {
            Name = aWrappedOptionPage.Name;
            Options = new List<OptionViewModel>();
            foreach (Option o in aWrappedOptionPage.Options)
            {
                OptionViewModel model = OptionViewModelFactory.Create(o);
                model.Dispatcher = aDispatcher;
                Options.Add(model);
            }
        }

        public string Name { get; set; }
        public List<OptionViewModel> Options { get; set; }

    }
    #endregion

    #region OptionViewModel
    public class OptionViewModel : INotifyPropertyChanged, IDisposable
    {
        public OptionViewModel(Option aWrappedOption)
        {
            WrappedOption = aWrappedOption;
            WrappedOption.EventValueChanged += aWrappedOption_EventValueChanged;
            WrappedOption.EventAllowedChanged += aWrappedOption_EventAllowedChanged;
        }
        internal Dispatcher Dispatcher { get; set; }

        public Option WrappedOption { get; set; }

        public IList<string> Allowed
        {
            get
            {
                return WrappedOption.Allowed;
            }
        }

        public string Value
        {
            get
            {
                return WrappedOption.Value;
            }
            set
            {
                if (value != WrappedOption.Value)
                {
                    // error caught if set returns false
                    if (!WrappedOption.Set(value))
                    {
                        // notify property has not been changed
                        OnPropertyChanged("Value");
                    }
                }
            }
        }
        public string ToolTip
        {
            get
            {
                return WrappedOption.Description;
            }
        }

        void aWrappedOption_EventValueChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Value");
        }
        void aWrappedOption_EventAllowedChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Allowed");
        }

        public virtual void ResetToDefault()
        {
            WrappedOption.ResetToDefault();
            OnPropertyChanged("Value");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }));
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            WrappedOption.EventValueChanged -= aWrappedOption_EventValueChanged;
            WrappedOption.EventAllowedChanged -= aWrappedOption_EventAllowedChanged;
        }

        #endregion
    }
    #endregion

    #region EnumOptionViewModel
    public class EnumOptionViewModel : OptionViewModel
    {
        public EnumOptionViewModel(Option aWrappedOption) : base(aWrappedOption) { }
    }
    #endregion

    #region BoolOptionViewModel
    public class BoolOptionViewModel : OptionViewModel
    {
        public BoolOptionViewModel(Option aWrappedOption) : base(aWrappedOption) { }
    }
    #endregion

    public interface IListOptionViewModel
    {
        void AddItem();
        void RemoveItems(object[] aItems);
    }

    #region ListOptionViewModel
    public abstract class ListOptionViewModel<T> : OptionViewModel, IListOptionViewModel
    {
        public ListOptionViewModel(OptionListSimple<T> aWrappedOption)
            : base(aWrappedOption)
        {
            List = new ObservableCollection<T>();
            foreach (T item in aWrappedOption.Native)
            {
                List.Add(item);
            }
        }

        public ObservableCollection<T> List { get; set; }

        public virtual void AddItem() { }

        public virtual void RemoveItems(object[] aItems) { }

        public override void ResetToDefault()
        {
            List.Clear();
            base.ResetToDefault();
        }
    }
    #endregion

    #region UriListOptionViewModel
    public class UriListOptionViewModel : ListOptionViewModel<Uri>
    {
        public UriListOptionViewModel(OptionListUri aWrappedOption)
            : base(aWrappedOption)
        {
        }
        public override void AddItem()
        {
            StringInputDialog dialog = new StringInputDialog();
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string uriString = dialog.Result();
                try
                {
                    Uri uri = new Uri(uriString);
                    List.Add(uri);
                    (WrappedOption as OptionListUri).Native = new List<Uri>(List);
                }
                catch (UriFormatException e)
                {
                    UserLog.WriteLine("UriFormatException caught parsing URI: " + uriString + ", " + e);
                    MessageBox.Show("The text you have entered is not a valid link.", "Invalid URI", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.None);
                }
            }
        }
        public override void RemoveItems(object[] aItems)
        {
            foreach (Uri item in aItems)
            {
                List.Remove(item);
                (WrappedOption as OptionListUri).Native = new List<Uri>(List);
            }
        }
    }
    #endregion

    #region FolderListOptionViewModel
    public class FolderListOptionViewModel : ListOptionViewModel<string>
    {
        public FolderListOptionViewModel(OptionListString aWrappedOption)
            : base(aWrappedOption)
        {
        }
        public override void AddItem()
        {
            //need to use winforms dialogs here as unfortunately wpf does not ship with folder browser dialog
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = WrappedOption.Description;
            try
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dlg.SelectedPath;
                    if (List.Contains(path))
                    {
                        throw new ItemAlreadyExistsException(String.Format("Cannot add item as it already exists: {0}", path));
                    }
                    List.Add(path);
                    (WrappedOption as OptionListString).Native = new List<string>(List);
                }
            }
            catch (SEHException ex)
            {
                // COM Error - local machine problem
                UserLog.WriteLine("Exception caught browsing for folder: " + ex);
            }
        }
        public override void RemoveItems(object[] aItems)
        {
            foreach (string item in aItems)
            {
                List.Remove(item);
            }
            (WrappedOption as OptionListString).Native = new List<string>(List);
        }
    }
    #endregion

    #region FileOptionViewModel
    public class FileOptionViewModel : OptionViewModel
    {
        public FileOptionViewModel(Option aWrappedOption) : base(aWrappedOption) { }
    }
    #endregion

    #region FolderOptionViewModel
    public class FolderOptionViewModel : OptionViewModel
    {
        public FolderOptionViewModel(Option aWrappedOption) : base(aWrappedOption) { }
    }
    #endregion

    #region ColourOptionViewModel
    public class ColourOptionViewModel : OptionViewModel
    {
        public ColourOptionViewModel(Option aWrappedOption) : base(aWrappedOption) { }
    }
    #endregion

}
