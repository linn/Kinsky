using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Linn;
using Linn.Kinsky;
using Linn.Toolkit.Ios;

using Upnp;
using CoreGraphics;

namespace KinskyTouch
{
    [Foundation.Register("ViewWidgetBrowser")]
    partial class ViewWidgetBrowser : UITableViewController, IViewWidgetContent
    {
        internal class SectionIndexCollector : IContentHandler
        {
            public class SectionIndex
            {
                public SectionIndex(uint aIndex, string aTitle)
                {
                    iIndex = aIndex;
                    iTitle = aTitle;
                }

                public uint Index
                {
                    get
                    {
                        return iIndex;
                    }
                }

                public string Title
                {
                    get
                    {
                        return iTitle;
                    }
                }

                private uint iIndex;
                private string iTitle;
            }

            public SectionIndexCollector(ViewWidgetBrowser aViewWidgetBrowser)
            {
                iViewWidgetBrowser = aViewWidgetBrowser;

                iSectionIndexList = new List<SectionIndex>();
                iTitles = new List<string>();
                iPreviousTitleStartsWithThe = false;
                iFirstTheIndex = 0;
            }

            public void Open(IContentCollector aCollector, uint aCount)
            {
                iCount = aCount;
                if(iCount >= kCountToAddSections)
                {
                    aCollector.Range(0, aCount);
                }
            }

            public void Item(IContentCollector aCollector, uint aIndex, upnpObject aObject)
            {
            }

            public void Items(IContentCollector aCollector, uint aStartIndex, IList<upnpObject> aObjects)
            {
                Trace.WriteLine(Trace.kKinskyTouch, "SectionIndexCollector.Items: aStartIndex=" + aStartIndex + ", count=" + aObjects.Count);

                uint index = aStartIndex;
                foreach(upnpObject o in aObjects)
                {
                    string title = DidlLiteAdapter.Title(o);
                    string letter = string.Empty;
                    bool startsWithThe = title.ToLowerInvariant().StartsWith("the ");

                    if((title.Length > 0 && !startsWithThe) || (title.Length > 4 && startsWithThe))
                    {
                        if(startsWithThe)
                        {
                            letter = title[4].ToString().ToUpperInvariant();
                        }
                        else
                        {
                            letter = title[0].ToString().ToUpperInvariant();
                        }

                        if(letter.CompareTo("A") < 0 || letter.CompareTo("Z") > 0)
                        {
                            letter = "#";
                        }

                        if(iPreviousTitleStartsWithThe && (letter != "#") && (letter.CompareTo("T") >= 0) && !startsWithThe)
                        {
                            if(!iTitles.Contains("T"))
                            {
                                iCurrentSectionIndex = new SectionIndex(iFirstTheIndex, "T");
                                iSectionIndexList.Add(iCurrentSectionIndex);
                                iTitles.Add("T");
                            }
                        }
                    }

                    if(!string.IsNullOrEmpty(letter) && (iCurrentSectionIndex == null || iCurrentSectionIndex.Title.ToUpperInvariant() != letter && !startsWithThe))
                    {
                        if(!iTitles.Contains(letter) || (letter == "#" && iPreviousLetter != "#"))
                        {
                            if(iPreviousTitleStartsWithThe && iPreviousLetter == letter)
                            {
                                iCurrentSectionIndex = new SectionIndex(iFirstTheIndex, letter);
                            }
                            else
                            {
                                iCurrentSectionIndex = new SectionIndex(index, letter);
                            }
                            iSectionIndexList.Add(iCurrentSectionIndex);
                            iTitles.Add(letter);
                        }
                    }

                    if(startsWithThe && !iPreviousTitleStartsWithThe)
                    {
                        iFirstTheIndex = index;
                    }
                    iPreviousTitleStartsWithThe = startsWithThe;
                    iPreviousLetter = letter;
                    ++index;
                }

                if(index == iCount)
                {
                    Trace.WriteLine(Trace.kKinskyTouch, "SectionIndexCollector.Items: Finished creating section title index list");

                    string previousTitle = string.Empty;
                    foreach (SectionIndex i in iSectionIndexList)
                    {
                        if(i.Title.Length > 0)
                        {
                            bool isInRange = false;
                            char c = i.Title.ToUpperInvariant()[0];
                            if(c.CompareTo('A') >= 0 && c.CompareTo('Z') <= 0)
                            {
                                isInRange = true;
                            }

                            if (isInRange && i.Title.CompareTo(previousTitle) < 0)
                            {
                                string message = string.Empty;
                                for(int j = 0; j < iSectionIndexList.Count; ++j)
                                {
                                    if(j > 0)
                                    {
                                        message += ", ";
                                    }
                                    message += string.Format("{0} ({1})", iSectionIndexList[j].Title, iSectionIndexList[j].Index);
                                }
                                Trace.WriteLine(Trace.kKinskyTouch, "SectionIndexCollector: " + message);
                                UserLog.WriteLine("SectionIndexCollector: " + message);

                                return; // don't show an alphabet index list if titles are not sorted alphabetically
                            }

                            if(isInRange)
                            {
                                previousTitle = i.Title;
                            }
                        }
                    }

                    iViewWidgetBrowser.SetSectionIndexTitles(iSectionIndexList);
                }
            }

            public void ContentError(IContentCollector aCollector, string aMessage)
            {
            }

            private const int kCountToAddSections = 100;

            private uint iCount;

            private ViewWidgetBrowser iViewWidgetBrowser;

            private SectionIndex iCurrentSectionIndex;
            private List<string> iTitles;
            private List<SectionIndex> iSectionIndexList;
            private bool iPreviousTitleStartsWithThe;
            private string iPreviousLetter;
            private uint iFirstTheIndex;
        }

        private interface IDataSource
        {
            upnpObject ObjectAt(NSIndexPath aIndexPath);
        }

        private class DataSource : UITableViewDataSource, IContentHandler, IDataSource
        {
            public DataSource(IntPtr aInstance)
                : base(aInstance)
            {
                iLockObject = new object();

                iSectionTitles = new List<string>();
                iSectionRowCount = new List<int>();
                iSectionIndexTitles = new List<SectionIndexCollector.SectionIndex>();
            }

            public DataSource(UITableView aTableView, IContainer aContainer, BrowserToolbar aToolbar)
            {
                iTableView = aTableView;
                iContainer = aContainer;
                iToolbar = aToolbar;

                iLockObject = new object();

                iSectionTitles = new List<string>();
                iSectionRowCount = new List<int>();
                iSectionIndexTitles = new List<SectionIndexCollector.SectionIndex>();
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                lock(iLockObject)
                {
					return iSectionRowCount[(int)aSection];
                }
            }

            public override nint NumberOfSections(UITableView aTableView)
            {
                lock(iLockObject)
                {
                    return iSectionTitles.Count;
                }
            }

            [Export("sectionIndexTitlesForTableView:")]
            public NSArray SectionTitles(UITableView aTableview)
            {
                lock(iLockObject)
                {
                    if(iSectionTitles.Count > 2)
                    {
                        return NSArray.FromStrings(iSectionTitles.ToArray());
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /*public override string[] SectionIndexTitles(UITableView aTableView)
            {
                lock(iLockObject)
                {
                    if(iSectionTitles.Count > 2)
                    {
                        return iSectionTitles.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
            }*/

            public override string TitleForHeader(UITableView aTableView, nint aSection)
            {
                lock(iLockObject)
                {
                    if(iSectionTitles.Count < 2 || iSectionTitles[0] == string.Empty || iCount < iTableView.SectionIndexMinimumDisplayRowCount)
                    {
                        return null;
                    }
					return iSectionTitles[(int)aSection];
                }
            }

            public override nint SectionFor(UITableView aTableView, string aTitle, nint aAtIndex)
            {
                return aAtIndex;
            }

            public override bool CanEditRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                lock(iLockObject)
                {
                    DidlLite didl = new DidlLite();
                    upnpObject o = ObjectAt(aIndexPath);
                    if(o == null)
                    {
                        return false;
                    }
                    didl.Add(o);
                    return iContainer.HandleDelete(didl);
                }
            }

            public override void CommitEditingStyle(UITableView aTableView, UITableViewCellEditingStyle aEditingStyle, NSIndexPath aIndexPath)
            {
                lock(iLockObject)
                {
                    if(aEditingStyle == UITableViewCellEditingStyle.Delete)
                    {
                        upnpObject o = ObjectAt(aIndexPath);
                        iContainer.Delete(o.Id);
                        //aTableView.DeleteRows(new NSIndexPath[] { aIndexPath }, UITableViewRowAnimation.Fade);
                    }
                }
            }

            public override bool CanMoveRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                lock(iLockObject)
                {
                    DidlLite didl = new DidlLite();
                    upnpObject o = ObjectAt(aIndexPath);
                    if(o == null)
                    {
                        return false;
                    }
                    didl.Add(o);
                    return iContainer.HandleMove(didl);
                }
            }

            public override void MoveRow(UITableView aTableView, NSIndexPath aSourceIndexPath, NSIndexPath aDestinationIndexPath)
            {
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellBrowser cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellBrowser;

                if(cell == null)
                {
                    CellBrowserFactory factory = new CellBrowserFactory();
                    NSBundle.MainBundle.LoadNib("CellBrowser", factory, null);
                    cell = factory.Cell;
                }

                lock(iLockObject)
                {
                    upnpObject o = ObjectAt(aIndexPath);

                    cell.Image = null;
                    string t = DidlLiteAdapter.OriginalTrackNumber(o);
                    if(string.IsNullOrEmpty(t))
                    {
                        cell.Title = string.Format("{0}", DidlLiteAdapter.Title(o));
                    }
                    else
                    {
                        cell.Title = string.Format("{0} {1}", t, DidlLiteAdapter.Title(o));
                    }
                    cell.ArtistAlbum = GetArtistAlbum(o);
                    cell.DurationBitrate = DidlLiteAdapter.Duration(o);
                    cell.AccessoryView = (o is container) ? new UIImageView(KinskyTouch.Properties.ResourceManager.Disclosure) : null;

                    cell.Image = GetDefaultArtwork(o);

                    Uri uri = DidlLiteAdapter.ArtworkUri(o);
                    cell.SetArtworkUri(uri);
                }

                cell.BackgroundView = new UIView();
                cell.BackgroundView.BackgroundColor = UIColor.Black;
                return cell;
            }

            public virtual void Open(IContentCollector aContentCollector, uint aCount)
            {
                OnOpen(aContentCollector, aCount);
            }

            public virtual void Item(IContentCollector aContentCollector, uint aIndex, upnpObject aObject)
            {
                OnItem(aContentCollector, aIndex, aObject);
            }

            public virtual void Items(IContentCollector aContentCollector, uint aStartIndex, IList<upnpObject> aObjects)
            {
                OnItems(aContentCollector, aStartIndex, aObjects);
            }

            public void ContentError(IContentCollector aContentCollector, string aMessage)
            {
                lock(iLockObject)
                {
                    BeginInvokeOnMainThread(delegate {
                        iCount = 0;
                        iTableView.ReloadData();
                    });
                }
            }

            public virtual upnpObject ObjectAt(NSIndexPath aIndexPath)
            {
                lock(iLockObject)
                {
                    //Console.WriteLine(aIndexPath.Section + " " + iSectionTitles.Count);
                    int offset = (int)iSectionIndexTitles[aIndexPath.Section].Index;
                    int index = offset + aIndexPath.Row;

                    return iContentCollector.Item((uint)index);
                }
            }

            public void SetSectionIndexTitles(List<SectionIndexCollector.SectionIndex> aSectionIndexTitles)
            {
                BeginInvokeOnMainThread(delegate {
                    lock(iLockObject)
                    {
                        if(iCount >= iTableView.SectionIndexMinimumDisplayRowCount)
                        {
                            iSectionTitles.Clear();
                            iSectionRowCount.Clear();

                            for(int i = 0; i < aSectionIndexTitles.Count; ++i)
                            {
                                iSectionTitles.Add(aSectionIndexTitles[i].Title);

                                int rows = 0;
                                if(i + 1 < aSectionIndexTitles.Count)
                                {
                                    rows = (int)(aSectionIndexTitles[i + 1].Index - aSectionIndexTitles[i].Index);
                                }
                                else
                                {
                                    rows = (int)(iCount - aSectionIndexTitles[i].Index);
                                }
                                iSectionRowCount.Add(rows);
                            }
                            iSectionIndexTitles = aSectionIndexTitles;

                            iTableView.ReloadData();
                        }
                    }
                });
            }

            protected void OnOpen(IContentCollector aContentCollector, uint aCount)
            {
                BeginInvokeOnMainThread(delegate {
                    lock(iLockObject)
                    {
                        //Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.DataSource.Open: location count is " + aCount);

                        iToolbar.AllowEditing = false;

                        iContentCollector = aContentCollector;
                        iCount = (int)aCount;

                        iSectionTitles.Clear();
                        iSectionRowCount.Clear();
                        iSectionIndexTitles.Clear();

                        if(aCount < kNumSections - 1)
                        {
                            iRowsPerSection = 0;
                            iSectionTitles.Add(string.Empty);
                            iSectionRowCount.Add((int)aCount);
                            iSectionIndexTitles.Add(new SectionIndexCollector.SectionIndex(0, string.Empty));
                        }
                        else
                        {
                            iRowsPerSection = (int)Math.Floor(((float)aCount / (float)(kNumSections - 1)) + 0.5f);

                            int index = 0;
                            for(uint i = 0; i < kNumSections; ++i)
                            {
                                iSectionTitles.Add(string.Empty);
                                iSectionIndexTitles.Add(new SectionIndexCollector.SectionIndex((uint)index, string.Empty));

                                int rows = iRowsPerSection;
                                if(i == kNumSections - 2)
                                {
                                    rows = iCount - index;
                                }
                                if(index + iRowsPerSection > iCount)
                                {
                                    rows = iCount - index;
                                }

                                iSectionRowCount.Add(rows);
                                index += rows;
                            }
                        }

                        //Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.DataSource.Open: iRowsPerSection=" + iRowsPerSection);
                    }

                    iTableView.ReloadData();
                });
            }

            protected void OnItem(IContentCollector aContentCollector, uint aIndex, upnpObject aObject)
            {
                /*DidlLiteAdapter.Title(aObject);
                DidlLiteAdapter.Artist(aObject);
                DidlLiteAdapter.Duration(aObject);
                DidlLiteAdapter.ArtworkUri(aObject);
                DidlLiteAdapter.OriginalTrackNumber(aObject);*/

                /*//BeginInvokeOnMainThread(delegate {
                    int section = 0;
                    int index = 0;
                    lock(iLockObject)
                    {
                        if(aContentCollector == iContentCollector)
                        {
                            Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.DataSource.Item: aIndex=" + aIndex);

                            section = 0;
                            for(int i = 0; i < iSectionIndexTitles.Count; ++i)
                            {
                                if(aIndex < iSectionIndexTitles[i].Index)
                                {
                                    section = i - 1;
                                    break;
                                }
                            }

                            index = (int)aIndex - (int)iSectionIndexTitles[section].Index;
                            if(section + 1 < iSectionRowCount.Count)
                            {
                                if(index > iSectionRowCount[section] - 1)
                                {
                                    ++section;
                                    index = (int)aIndex - (int)iSectionIndexTitles[section].Index;
                                }
                            }


                            DidlLite didl = new DidlLite();
                            didl.Add(aObject);
                            if(!iToolbar.AllowEditing && (iContainer.HandleDelete(didl) || iContainer.HandleMove(didl)))
                            {
                                iToolbar.AllowEditing = true;
                            }

                            //iTableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, section) }, UITableViewRowAnimation.Fade);
                        }
                        else
                        {
                            Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.DataSource.Item: aIndex=" + aIndex + " thrown away");
                        }
                    }

                if(!iTableView.Tracking && !iTableView.Decelerating && !iTableView.Dragging)
                {
                    BeginInvokeOnMainThread(delegate {
                        iTableView.ReloadData();
                    });
                }*/
            }

            protected void OnItems(IContentCollector aContentCollector, uint aStartIndex, IList<upnpObject> aObjects)
            {
                lock(iLockObject)
                {
                    if(aContentCollector == iContentCollector)
                    {
                        Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.DataSource.Items: aStartIndex=" + aStartIndex + ", count=" + aObjects.Count);

                        DidlLite didl = new DidlLite();
                        didl.AddRange(aObjects);
                        if(!iToolbar.AllowEditing && (iContainer.HandleDelete(didl) || iContainer.HandleMove(didl)))
                        {
                            BeginInvokeOnMainThread(delegate {
                                iToolbar.AllowEditing = true;
                            });
                        }
                    }
                    else
                    {
                        Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.DataSource.Items: aStartIndex=" + aStartIndex + ", count=" + aObjects.Count + " thrown away");
                    }
                }

                BeginInvokeOnMainThread(delegate {
                    if(!iTableView.Tracking && !iTableView.Decelerating && !iTableView.Dragging)
                    {
                        iTableView.ReloadData();
                    }
                });
            }

            protected string GetArtistAlbum(upnpObject aObject)
            {
                if(aObject is album || aObject is audioItem)
                {
                    return DidlLiteAdapter.Artist(aObject);
                }

                return string.Empty;
            }

            protected UIImage GetDefaultArtwork(upnpObject aObject)
            {
                if(aObject == null)
                {
                    return KinskyTouch.Properties.ResourceManager.Loading;
                }
                else if(aObject.Id == MediaProviderLibrary.kLibraryId)
                {
                    return KinskyTouch.Properties.ResourceManager.Library;
                }
                else if(aObject is item)
                {
                    if(aObject is audioBroadcast)
                    {
                        return KinskyTouch.Properties.ResourceManager.Radio;
                    }
                    else if(aObject is playlistItem)
                    {
                        return KinskyTouch.Properties.ResourceManager.PlaylistItem;
                    }
                    else if(aObject is audioItem)
                    {
                        return KinskyTouch.Properties.ResourceManager.Track;
                    }
                    else if (aObject is videoItem)
                    {
                        return KinskyTouch.Properties.ResourceManager.Video;
                    }

                    return KinskyTouch.Properties.ResourceManager.PlaylistItem;
                }
                else if(aObject is container)
                {
                    if(aObject is album)
                    {
                        return KinskyTouch.Properties.ResourceManager.Album;
                    }
                    else if(aObject is person)
                    {
                        return KinskyTouch.Properties.ResourceManager.Artist;
                    }
                    else if(aObject is playlistContainer)
                    {
                        return KinskyTouch.Properties.ResourceManager.Playlist;
                    }

                    return KinskyTouch.Properties.ResourceManager.Directory;
                }

                return null;
            }

            private static NSString kCellIdentifier = new NSString("CellBrowser");

            private const int kNumSections = 21;

            private object iLockObject;

            private int iCount;
            private int iRowsPerSection;

            private UITableView iTableView;
            private BrowserToolbar iToolbar;

            protected IContentCollector iContentCollector;
            protected IContainer iContainer;
            protected List<SectionIndexCollector.SectionIndex> iSectionIndexTitles;

            private List<string> iSectionTitles;
            private List<int> iSectionRowCount;
        }

        private class DataSourceAlbum : DataSource
        {
            public DataSourceAlbum(UITableView aTableView, IContainer aContainer, BrowserToolbar aBrowserToolbar)
                : base(aTableView, aContainer, aBrowserToolbar)
            {
                iContainer = aContainer;
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                if(aIndexPath.Section == 0 && aIndexPath.Row == 0)
                {
                    CellBrowserHeader cell = aTableView.DequeueReusableCell(kCellHeaderIdentifier) as CellBrowserHeader;

                    if(cell == null)
                    {
                        CellBrowserHeaderFactory factory = new CellBrowserHeaderFactory();
                        NSBundle.MainBundle.LoadNib("CellBrowserHeader", factory, null);
                        cell = factory.Cell;
                    }

                    lock(this)
                    {
                        upnpObject o = ObjectAt(aIndexPath);

                        cell.Image = GetDefaultArtwork(o);
                        cell.Title = DidlLiteAdapter.Title(o);
                        cell.ArtistAlbum = GetArtistAlbum(o);
                        cell.Composer = DidlLiteAdapter.Composer(o);

                        Uri uri = DidlLiteAdapter.ArtworkUri(o);
                        cell.SetArtworkUri(uri);
                    }

                    cell.BackgroundView = new UIView();
                    cell.BackgroundView.BackgroundColor = UIColor.Black;
                    return cell;
                }
                else
                {
                    CellBrowserItem cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellBrowserItem;

                    if(cell == null)
                    {
                        CellBrowserItemFactory factory = new CellBrowserItemFactory();
                        NSBundle.MainBundle.LoadNib("CellBrowserItem", factory, null);
                        cell = factory.Cell;
                    }

                    lock(this)
                    {
                        upnpObject o = ObjectAt(aIndexPath);

                        cell.Title = DidlLiteAdapter.Title(o);
                        if(iContainer.Metadata is playlistContainer)
                        {
                            int offset = (int)iSectionIndexTitles[aIndexPath.Section].Index;
                            int index = offset + aIndexPath.Row;
                            cell.TrackNumber = index.ToString();
                        }
                        else
                        {
                            cell.TrackNumber = DidlLiteAdapter.OriginalTrackNumber(o);
                        }
                        string a = GetArtistAlbum(o);
                        if(a == GetArtistAlbum(iContainer.Metadata))
                        {
                            a = string.Empty;
                        }
                        cell.ArtistAlbum = a;
                        cell.DurationBitrate = DidlLiteAdapter.Duration(o);
                    }

                    cell.BackgroundView = new UIView();
                    cell.BackgroundView.BackgroundColor = UIColor.Black;
                    return cell;
                }
            }

            public override void Open(IContentCollector aContentCollector, uint aCount)
            {
                OnOpen(aContentCollector, aCount + 1);
            }

            public override void Item(IContentCollector aContentCollector, uint aIndex, upnpObject aObject)
            {
                base.Item(aContentCollector, aIndex + 1, aObject);
            }

            public override void Items(IContentCollector aContentCollector, uint aStartIndex, IList<upnpObject> aObjects)
            {
                base.Items(aContentCollector, aStartIndex + 1, aObjects);
            }

            public override upnpObject ObjectAt(NSIndexPath aIndexPath)
            {
                lock(this)
                {
                    int offset = (int)iSectionIndexTitles[aIndexPath.Section].Index;
                    int index = offset + aIndexPath.Row - 1;

                    if(index < 0)
                    {
                        return iContainer.Metadata;
                    }

                    return iContentCollector.Item((uint)index);
                }
            }

            private static NSString kCellHeaderIdentifier = new NSString("CellBrowserHeader");
            private static NSString kCellIdentifier = new NSString("CellBrowserItem");
        }

        private class Delegate : UITableViewDelegate, IDisposable
        {
            public Delegate(IntPtr aInstance)
                : base(aInstance)
            {
            }

            public Delegate(ViewWidgetBrowser aController, Location aLocation, IPlaylistSupport aPlaySupport, IDataSource aDataSource,
                            ConfigController aConfigController, OptionEnum aOptionInsertMode, OptionBreadcrumbTrail aOptionBreadcrumbTrail)
            {
                iController = aController;
                iLocation = aLocation;
                iPlaySupport = aPlaySupport;
                iDataSource = aDataSource;
                iConfigController = aConfigController;
                iOptionInsertMode = aOptionInsertMode;
                iOptionBreadcrumbTrail = aOptionBreadcrumbTrail;
            }

            public override nfloat GetHeightForRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                if(iDataSource is DataSourceAlbum)
                {
                    if(aIndexPath.Section == 0 && aIndexPath.Row == 0)
                    {
                        return 120.0f;
                    }
                    else
                    {
                        return 55.0f;
                    }
                }

                return 73.0f;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);

                upnpObject o = iDataSource.ObjectAt(aIndexPath);
                if(o != null)
                {
                    IMediaRetriever retriever = null;
                    if(o is container)
                    {
                        if(o != iLocation.Current.Metadata)
                        {
                            container c = o as container;

                            IContainer container = iLocation.Current.ChildContainer(c);

                            Location location = new Location(iLocation, container);

                            ViewWidgetBrowser controller = new ViewWidgetBrowser(location, iPlaySupport, iConfigController, iOptionInsertMode, iOptionBreadcrumbTrail);

                            if(iController.NavigationController != null)
                            {
                                iController.NavigationController.PushViewController(controller, true);
                            }

                            return;
                        }
                        else
                        {
                            retriever = new MediaRetriever(iLocation.PreviousLocation().Current, new List<upnpObject>() { o });
                        }
                    }
                    else
                    {
                        retriever = new MediaRetrieverNoRetrieve(new List<upnpObject> { o });
                    }

                    if(iOptionInsertMode.Value == OptionInsertMode.kPlayNow)
                    {
                        iPlaySupport.PlayNow(retriever);
                    }
                    else if(iOptionInsertMode.Value == OptionInsertMode.kPlayNext)
                    {
                        iPlaySupport.PlayNext(retriever);
                    }
                    else if(iOptionInsertMode.Value == OptionInsertMode.kPlayLater)
                    {
                        iPlaySupport.PlayLater(retriever);
                    }
                }
            }

            public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
            {
                if(!willDecelerate)
                {
                    iController.TableView.ReloadData();
                }
            }

            public override void DecelerationEnded(UIScrollView scrollView)
            {
                iController.TableView.ReloadData();
            }

            private ViewWidgetBrowser iController;
            private Location iLocation;
            private IPlaylistSupport iPlaySupport;
            private ViewWidgetBrowser.IDataSource iDataSource;
            private ConfigController iConfigController;
            private OptionEnum iOptionInsertMode;
            private OptionBreadcrumbTrail iOptionBreadcrumbTrail;
        }

        private class SearchDisplayDelegate : UISearchDisplayDelegate
        {
            public SearchDisplayDelegate(SearchResultsDataSource aSearchResultsDataSource, IContainer aContainer, UILongPressGestureRecognizer aGesture)
            {
                iSearchResultsDataSource = aSearchResultsDataSource;
                iContainer = aContainer;
                iGesture = aGesture;
            }

            public override void DidLoadSearchResults(UISearchDisplayController aController, UITableView aTableView)
            {
                aTableView.RowHeight = 73.0f;
                aTableView.BackgroundColor = UIColor.Black;
                aTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                aTableView.ShowsHorizontalScrollIndicator = false;
                aTableView.IndicatorStyle = UIScrollViewIndicatorStyle.White;

                aTableView.AddGestureRecognizer(iGesture);

                iTableView = aTableView;
            }

            public override void DidBeginSearch(UISearchDisplayController aController)
            {
                //Console.WriteLine("DidBeginSearch");
                iContentCollector = ContentCollectorMaster.Create(iContainer, iSearchResultsDataSource);

                ArtworkCacheInstance.Instance.EventImageAdded += EventImageAdded;
            }

            public override void DidEndSearch(UISearchDisplayController aController)
            {
                //Console.WriteLine("DidEndSearch");
                ArtworkCacheInstance.Instance.EventImageAdded -= EventImageAdded;

                if(iContentCollector != null)
                {
                    iContentCollector.Dispose();
                    iContentCollector = null;
                }

                iTableView = null;
            }

            public override bool ShouldReloadForSearchString(UISearchDisplayController aController, string aForSearchString)
            {
                iSearchResultsDataSource.SetSearchString(iContentCollector, aForSearchString, aController.SearchResultsTableView);

                return false;
            }

            private void EventImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
            {
                BeginInvokeOnMainThread(delegate {
                    if(iTableView != null && !iTableView.Dragging && !iTableView.Decelerating && !iTableView.Tracking)
                    {
                        iTableView.ReloadData();
                    }
                });
            }

            private UILongPressGestureRecognizer iGesture;
            private IContentCollector iContentCollector;
            private IContainer iContainer;
            private SearchResultsDataSource iSearchResultsDataSource;
            private UITableView iTableView;
        }

        private class SearchResultsDataSource : UITableViewDataSource, IContentHandler, IDataSource
        {
            public SearchResultsDataSource()
            {
                iList = new List<upnpObject>();
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                lock(this)
                {
                    return iList.Count;
                }
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellBrowser cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellBrowser;

                if(cell == null)
                {
                    CellBrowserFactory factory = new CellBrowserFactory();
                    NSBundle.MainBundle.LoadNib("CellBrowser", factory, null);
                    cell = factory.Cell;
                }

                lock(this)
                {
                    upnpObject o = ObjectAt(aIndexPath);

                    cell.Image = null;
                    string t = DidlLiteAdapter.OriginalTrackNumber(o);
                    if(string.IsNullOrEmpty(t))
                    {
                        cell.Title = string.Format("{0}", DidlLiteAdapter.Title(o));
                    }
                    else
                    {
                        cell.Title = string.Format("{0} {1}", t, DidlLiteAdapter.Title(o));
                    }
                    cell.ArtistAlbum = GetArtistAlbum(o);
                    cell.DurationBitrate = DidlLiteAdapter.Duration(o);
                    cell.AccessoryView = (o is container) ? new UIImageView(KinskyTouch.Properties.ResourceManager.Disclosure) : null;

                    cell.Image = GetDefaultArtwork(o);

                    Uri uri = DidlLiteAdapter.ArtworkUri(o);
                    cell.SetArtworkUri(uri);
                }

                cell.BackgroundView = new UIView();
                cell.BackgroundView.BackgroundColor = UIColor.Black;
                return cell;
            }

            public void SetSearchString(IContentCollector aCollector, string aSearchString, UITableView aTableView)
            {
                lock(this)
                {
                    if(aCollector != null && aTableView != null && aSearchString != null && aCollector == iContentCollector)
                    {
                        iTableView = aTableView;
                        iSearchString = aSearchString.ToLower();

                        BeginInvokeOnMainThread(delegate {
                            lock(this)
                            {
                                iList.Clear();
                                iTableView.ReloadData();
                            }
                            if(string.IsNullOrEmpty(iSearchString))
                            {
                                iContentCollector.Range(0, 0);
                            }
                            else
                            {
                                iContentCollector.Range(0, iCount);
                            }
                        });
                    }
                }
            }

            public upnpObject ObjectAt(NSIndexPath aIndexPath)
            {
                lock(this)
                {
                    return iList[aIndexPath.Row];
                }
            }

            public void Open(IContentCollector aCollector, uint aCount)
            {
                lock(this)
                {
                    iContentCollector = aCollector;
                    iCount = aCount;
                    aCollector.Range(0, aCount);
                }
            }

            public void Item(IContentCollector aCollector, uint aIndex, upnpObject aObject)
            {
                /*BeginInvokeOnMainThread(delegate {
                    if(aCollector == iContentCollector)
                    {
                        if(!string.IsNullOrEmpty(iSearchString))
                        {
                            if(DidlLiteAdapter.Title(aObject).ToLower().Contains(iSearchString) || DidlLiteAdapter.Artist(aObject).ToLower().Contains(iSearchString))
                            {
                                lock(this)
                                {
                                    iList.Add(aObject);

                                    if(!iTableView.Tracking && !iTableView.Decelerating && !iTableView.Dragging)
                                    {
                                        iTableView.ReloadData();
                                    }
                                }
                            }
                        }
                    }
                });*/
            }

            public void Items(IContentCollector aCollector, uint aStartIndex, IList<upnpObject> aObjects)
            {
                BeginInvokeOnMainThread(delegate {
                    if(aCollector == iContentCollector)
                    {
                        if(!string.IsNullOrEmpty(iSearchString))
                        {
                            foreach(upnpObject o in aObjects)
                            {
                                if(DidlLiteAdapter.Title(o).ToLower().Contains(iSearchString) || DidlLiteAdapter.Artist(o).ToLower().Contains(iSearchString))
                                {
                                    lock(this)
                                    {
                                        iList.Add(o);
                                    }
                                }
                            }

                            if(!iTableView.Tracking && !iTableView.Decelerating && !iTableView.Dragging)
                            {
                                iTableView.ReloadData();
                            }
                        }
                    }
                });
            }

            public void ContentError(IContentCollector aCollector, string aMessage)
            {
            }

            private string GetArtistAlbum(upnpObject aObject)
            {
                if(aObject is album || aObject is audioItem)
                {
                    return DidlLiteAdapter.Artist(aObject);
                }

                return string.Empty;
            }

            private UIImage GetDefaultArtwork(upnpObject aObject)
            {
                if(aObject == null)
                {
                    return KinskyTouch.Properties.ResourceManager.Loading;
                }
                else if(aObject.Id == MediaProviderLibrary.kLibraryId)
                {
                    return KinskyTouch.Properties.ResourceManager.Library;
                }
                else if(aObject is item)
                {
                    if(aObject is audioBroadcast)
                    {
                        return KinskyTouch.Properties.ResourceManager.Radio;
                    }
                    else if(aObject is playlistItem)
                    {
                        return KinskyTouch.Properties.ResourceManager.PlaylistItem;
                    }
                    else if(aObject is audioItem)
                    {
                        return KinskyTouch.Properties.ResourceManager.Track;
                    }
                    else if (aObject is videoItem)
                    {
                        return KinskyTouch.Properties.ResourceManager.Video;
                    }

                    return KinskyTouch.Properties.ResourceManager.PlaylistItem;
                }
                else if(aObject is container)
                {
                    if(aObject is album)
                    {
                        return KinskyTouch.Properties.ResourceManager.Album;
                    }
                    else if(aObject is person)
                    {
                        return KinskyTouch.Properties.ResourceManager.Artist;
                    }
                    else if(aObject is playlistContainer)
                    {
                        return KinskyTouch.Properties.ResourceManager.Playlist;
                    }

                    return KinskyTouch.Properties.ResourceManager.Directory;
                }

                return null;
            }

            private static NSString kCellIdentifier = new NSString("CellBrowser");

            private List<upnpObject> iList;
            private string iSearchString;
            private IContentCollector iContentCollector;
            private UITableView iTableView;
            private uint iCount;
        }

        private class LongPressGestureDelegate : UIGestureRecognizerDelegate
        {
            public override bool ShouldReceiveTouch(UIGestureRecognizer aRecogniser, UITouch aTouch)
            {
                return true;
            }
        }

        public ViewWidgetBrowser(Location aLocation, IPlaylistSupport aPlaySupport, ConfigController aConfigController, OptionEnum aOptionInsertMode, OptionBreadcrumbTrail aOptionBreadcrumbTrail)
        {
            iLocation = aLocation;
            iPlaySupport = aPlaySupport;
            iConfigController = aConfigController;
            iOptionInsertMode = aOptionInsertMode;
            iOptionBreadcrumbTrail = aOptionBreadcrumbTrail;

            if(iLocation != null)
            {
                IContainer container = iLocation.Current;
                Title = DidlLiteAdapter.Title(container.Metadata);
				if(string.IsNullOrEmpty(Title))
				{
					Title = " ";
				}
            }
        }

        public ViewWidgetBrowser(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = 73.0f;
            TableView.BackgroundColor = UIColor.Black;
			TableView.BackgroundView = new UIView();
			TableView.BackgroundView.BackgroundColor = UIColor.Black;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.ShowsHorizontalScrollIndicator = false;
            TableView.SectionIndexMinimumDisplayRowCount = 100;
            TableView.IndicatorStyle = UIScrollViewIndicatorStyle.White;

			UISearchBar searchBar = new UISearchBar(new CGRect(0, 0, TableView.Frame.Width, 0));
            searchBar.BarStyle = UIBarStyle.Black;
            searchBar.AutocorrectionType = UITextAutocorrectionType.No;
            searchBar.Placeholder = "Filter";
            searchBar.SizeToFit();

            TableView.TableHeaderView = searchBar;

            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, Back);
            NavigationController.NavigationBar.TintColor = UIColor.White;

            iSearchController = new UISearchDisplayController(searchBar, this);

            if(TableView.ContentOffset.Y < searchBar.Frame.Height)
            {
                TableView.SetContentOffset(new CGPoint(0, searchBar.Frame.Height), false);
            }

            Open();
        }

        [Obsolete]
        public override void ViewDidUnload ()
        {
            base.ViewDidUnload ();

            // This methods is deprecated for iOS 6 and above. In earlier versions, iOS will purge views under low memory conditions.
            // This means that browser views that are not currently visible will be purged and when the user clicks the back button
            // to show them, the purged views will be reloaded and ViewDidLoad() will be called for a second time. This means Open()
            // is also called for a second time and the Assert.Check(iContentCollector == null) is fired. Therefore, this method
            // is implemented and will Close() any views that are purged. On iOS 6+, views are never purged and this function is
            // never called.
            Close();
        }

        private void Back(object sender, EventArgs e)
        {
            //Console.WriteLine("Back pressed");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if(iOptionBreadcrumbTrail != null && iLocation != null)
            {
                iOptionBreadcrumbTrail.BreadcrumbTrail = iLocation.BreadcrumbTrail;
            }
            ArtworkCacheInstance.Instance.EventImageAdded += EventImageAdded;
        }

        private void EventImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
        {
            if(!TableView.Dragging && !TableView.Decelerating && !TableView.Tracking)
            {
                InvokeOnMainThread(delegate {
                    TableView.ReloadData();
                });
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            ArtworkCacheInstance.Instance.EventImageAdded -= EventImageAdded;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            // NavigationController == null means that this view has been popped off the stack
            // and can now be cleaned up
            if (NavigationController == null)
            {
                if(iToolbar != null)
                {
                    iToolbar = null;
                }

                if(iSearchController != null)
                {
                    iSearchWasActive = iSearchController.Active;
                    iSearchString = iSearchController.SearchBar.Text;

                    iSearchController.Dispose();
                    iSearchController = null;
                }

                Close ();
            }
        }

        public void Open()
        {
            lock(this)
            {
                if(iLocation != null)
                {
                    Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.Open: " + iLocation.Current.Metadata.Title);

                    Assert.Check(iContentCollector == null);

                    iToolbar = new BrowserToolbar(this, iConfigController, iOptionInsertMode);

					foreach (IContainer c in iLocation.Containers)
					{
	                    c.EventContentAdded += ContentAdded;
    	                c.EventContentRemoved += ContentRemoved;
        	            c.EventContentUpdated += ContentUpdated;
						c.EventTreeChanged += TreeChanged;
					}

					IContainer container = iLocation.Current;

                    if(container.Metadata is musicAlbum || container.Metadata is playlistContainer)
                    {
                        iDataSource = new ViewWidgetBrowser.DataSourceAlbum(TableView, container, iToolbar);
                    }
                    else
                    {
                        iDataSource = new ViewWidgetBrowser.DataSource(TableView, container, iToolbar);
                    }

                    Title = DidlLiteAdapter.Title(container.Metadata);
					if(string.IsNullOrEmpty(Title))
					{
						Title = " ";
					}
                    TableView.DataSource = iDataSource;
                    TableView.Delegate = new ViewWidgetBrowser.Delegate(this, iLocation, iPlaySupport, iDataSource, iConfigController, iOptionInsertMode, iOptionBreadcrumbTrail);
                    TableView.SectionIndexBackgroundColor = UIColor.Clear;

                    UILongPressGestureRecognizer gesture = new UILongPressGestureRecognizer(LongPressGesture);
                    gesture.MinimumPressDuration = 1.0f;
                    gesture.Delegate = new LongPressGestureDelegate();
                    TableView.AddGestureRecognizer(gesture);

                    iContentCollector = ContentCollectorMaster.Create(container, iDataSource);

                    if(!(container.Metadata is musicAlbum || container.Metadata is playlistContainer))
                    {
                        iSectionIndexCollector = new SectionIndexCollector(this);
                        iContentCollector2 = ContentCollectorMaster.Create(container, iSectionIndexCollector);
                    }

                    gesture = new UILongPressGestureRecognizer(LongPressGesture);
                    gesture.MinimumPressDuration = 1.0f;
                    gesture.Delegate = new LongPressGestureDelegate();

                    SearchResultsDataSource dataSource = new SearchResultsDataSource();
                    iSearchController.Delegate = new SearchDisplayDelegate(dataSource, container, gesture);
                    iSearchController.SearchResultsDataSource = dataSource;
                    iSearchController.SearchResultsDelegate = new Delegate(this, iLocation, iPlaySupport, dataSource, iConfigController, iOptionInsertMode, iOptionBreadcrumbTrail);
                    iSearchController.SearchResultsTableView.SectionIndexBackgroundColor = UIColor.Clear;

                    /*iSearchController.Active = iSearchWasActive;
                    if(!string.IsNullOrEmpty(iSearchString))
                    {
                        iSearchController.SearchBar.Text = iSearchString;
                    }*/
                }
            }
        }

        public void Close()
        {
            lock(this)
            {
                if(iLocation != null)
                {
                    Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.Close: " + iLocation.Current.Metadata.Title);

                    foreach (IContainer c in iLocation.Containers)
					{
	                    c.EventContentAdded -= ContentAdded;
    	                c.EventContentRemoved -= ContentRemoved;
        	            c.EventContentUpdated -= ContentUpdated;
						c.EventTreeChanged -= TreeChanged;
					}

                    if(iContentCollector != null)
                    {
                        iContentCollector.Dispose();
                        iContentCollector = null;
                    }

                    TableView.DataSource = null;
                    TableView.Delegate = null;

                    if(iDataSource != null)
                    {
                        iDataSource.Dispose();
                        iDataSource = null;
                    }

                    if(iContentCollector2 != null)
                    {
                        iContentCollector2.Dispose();
                        iContentCollector2 = null;
                    }

                    iSectionIndexCollector = null;
                }
            }
        }

        public void OnSizeClick()
        {
        }

        public void OnViewClick()
        {
        }

        public void Focus()
        {
        }

        protected void OnLongPressGesture(UILongPressGestureRecognizer aRecogniser)
        {
            UITableView tableView = TableView;
            IDataSource dataSouce = iDataSource;

            if(aRecogniser.View != TableView)
            {
                tableView = iSearchController.SearchResultsTableView;
                dataSouce = iSearchController.SearchResultsDataSource as IDataSource;
            }

            if(aRecogniser.State == UIGestureRecognizerState.Began && iPlaySupport.IsInsertAllowed() && !iPlaySupport.IsInserting())
            {
                CGPoint point = aRecogniser.LocationInView(tableView);
                NSIndexPath path = tableView.IndexPathForRowAtPoint(point);
                if(path != null)
                {
                    upnpObject o = dataSouce.ObjectAt(path);
                    string title = DidlLiteAdapter.Title(o);

                    UIActionSheet sheet = new UIActionSheet(title);
                    sheet.AddButton("Play Now");
                    sheet.AddButton("Play Next");
                    sheet.AddButton("Play Later");
                    sheet.AddButton("Cancel");
                    sheet.CancelButtonIndex = 3;

                    Assert.Check(NavigationController.ViewControllers.Length > 0);

                    ViewWidgetBrowser viewWidgetBrowser = NavigationController.ViewControllers[0] as ViewWidgetBrowser;
					CGRect rect = tableView.RectForRowAtIndexPath (path);
					viewWidgetBrowser.OpenActionSheet(sheet, new CGRect((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height), tableView);

                    sheet.Clicked += delegate (object sender, UIButtonEventArgs e)
                    {
                        MediaRetriever r = null;
                        if(o == iLocation.Current.Metadata)
                        {
                            r = new MediaRetriever(iLocation.PreviousLocation().Current, new List<upnpObject>() { o });
                        }
                        else
                        {
                            r = new MediaRetriever(iLocation.Current, new List<upnpObject>() { o });
                        }

                        switch(e.ButtonIndex)
                        {
                        case 0:
                            iPlaySupport.PlayNow(r);
                            break;
                        case 1:
                            iPlaySupport.PlayNext(r);
                            break;
                        case 2:
                            iPlaySupport.PlayLater(r);
                            break;
                        default:
                            break;
                        }
                    };
                }
            }
        }

        protected virtual void OpenActionSheet(UIActionSheet aActionSheet, CGRect aRect, UITableView aTableView)
        {
            throw new NotSupportedException();
        }

        internal void SetSectionIndexTitles(List<SectionIndexCollector.SectionIndex> aSectionIndexTitles)
        {
            Trace.WriteLine(Trace.kKinskyTouch, ">ViewWidgetBrowser.SetSectionIndexTitles");

            lock(this)
            {
                if(iDataSource != null)
                {
                    iDataSource.SetSectionIndexTitles(aSectionIndexTitles);
                }

                if(iContentCollector2 != null)
                {
                    iContentCollector2.Dispose();
                    iContentCollector2 = null;
                }
            }
        }

		private void ContentAdded(object sender, EventArgs e)
        {
			if (sender == iLocation.Current)
			{
				Refresh();
			}
		}

		private void ContentRemoved(object sender, EventArgsContentRemoved e)
        {
			int index = iLocation.Containers.IndexOf(sender as IContainer);
			if (index != -1 && (index == iLocation.Containers.Count - 1 || iLocation.Containers[index + 1].Id == e.Id))
			{
				Refresh();
			}
		}

		private void ContentUpdated(object sender, EventArgs e)
        {
			if (sender == iLocation.Current)
			{
				Refresh ();
			}
		}

        private void TreeChanged(object sender, EventArgs e)
        {
            if (iLocation.Current.HasTreeChangeAffectedLeaf)
			{
				Refresh();
			}
        }

		private void Refresh()
		{
			lock(this)
            {
                Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetBrowser.ContentChanged: " + iLocation.Current.Metadata.Title);

                if(iContentCollector != null)
                {
                    iContentCollector.Dispose();
                    iContentCollector = null;
                }

                if(iContentCollector2 != null)
                {
                    iContentCollector2.Dispose();
                    iContentCollector2 = null;
                }

                IContainer container = iLocation.Current;
                iContentCollector = ContentCollectorMaster.Create(container, iDataSource);

                if(!(container.Metadata is musicAlbum || container.Metadata is playlistContainer))
                {
                    iSectionIndexCollector = new SectionIndexCollector(this);
                    iContentCollector2 = ContentCollectorMaster.Create(container, iSectionIndexCollector);
                }
            }
		}

        private void LongPressGesture(UILongPressGestureRecognizer aRecogniser)
        {
            OnLongPressGesture(aRecogniser);
        }

        protected Location iLocation;
        protected IPlaylistSupport iPlaySupport;
        protected ConfigController iConfigController;
        protected OptionEnum iOptionInsertMode;
        protected OptionBreadcrumbTrail iOptionBreadcrumbTrail;
        protected BrowserToolbar iToolbar;

        private UISearchDisplayController iSearchController;
        private bool iSearchWasActive;
        private string iSearchString;

        private IContentCollector iContentCollector;
        private DataSource iDataSource;

        private IContentCollector iContentCollector2;
        private SectionIndexCollector iSectionIndexCollector;
    }

    partial class ViewWidgetBrowserRoot : ViewWidgetBrowser
    {
        public ViewWidgetBrowserRoot(IntPtr aInstance)
            : base(aInstance)
        {
            iLock = new object();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iViewHourGlass = new ViewHourGlass();
            iViewHourGlass.BackgroundColor = UIColor.Black;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            iViewHourGlass.Frame = new CGRect(0.0f, 44.0f, NavigationController.View.Frame.Width, NavigationController.View.Frame.Height - 88.0f);
            iViewHourGlass.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
        }

        public void Initialise(Location aRootLocation, PlaySupport aPlaySupport, ConfigController aConfigController, OptionEnum aOptionInsertMode, OptionBreadcrumbTrail aOptionBreadcrumbTrail)
        {
            iRootLocation = aRootLocation;
            iPlaySupport = aPlaySupport;
            iConfigController = aConfigController;
            iOptionInsertMode = aOptionInsertMode;
            iOptionBreadcrumbTrail = aOptionBreadcrumbTrail;

            // try to locate the last location
            iRetrys = 5;

            BeginInvokeOnMainThread(delegate {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Home", UIBarButtonItemStyle.Plain, CancelLocate);
                iToolbar = new BrowserToolbar(this, iConfigController, iOptionInsertMode);

                NavigationController.View.Superview.AddSubview(iViewHourGlass);
                iViewHourGlass.Start();

                Locate();
            });
        }

        protected virtual void OnLocateError()
        {
            NavigationItem.LeftBarButtonItem = null;
            iViewHourGlass.Stop();
            iViewHourGlass.RemoveFromSuperview();
        }

        protected void Retry()
        {
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Home", UIBarButtonItemStyle.Plain, CancelLocate);

            NavigationController.View.Superview.AddSubview(iViewHourGlass);
            iViewHourGlass.Start();

            new System.Threading.Timer(Locate, null, 500, System.Threading.Timeout.Infinite);
        }

        protected void Home()
        {
            iOptionBreadcrumbTrail.BreadcrumbTrail = iRootLocation.BreadcrumbTrail;
            Located(null, iRootLocation);
        }

        private void Locate(object aObject)
        {
            Locate();
        }

        private void Locate()
        {
            lock(iLock)
            {
                iLocatorAsync = new LocatorAsync(iRootLocation.Current, iOptionBreadcrumbTrail.BreadcrumbTrail);
                iLocatorAsync.Locate(Located);
            }
        }

        private void Located(LocatorAsync aLocator, Location aLocation)
        {
            BeginInvokeOnMainThread(delegate {
                lock(iLock)
                {
                    if(iLocatorAsync != aLocator)
                    {
                        return;
                    }

                    iLocatorAsync = null;
                }

                if(aLocation != null)
                {
                    UserLog.WriteLine("Found last browse location (" + aLocation.BreadcrumbTrail.ToString() + ")");
                }
                else
                {
                    UserLog.WriteLine("Failed to find last browse location");
                }

                if(aLocation == null)
                {
                    if(iRetrys > 0)
                    {
                        --iRetrys;
                        new System.Threading.Timer(Locate, null, 3000, System.Threading.Timeout.Infinite);
                    }
                    else
                    {
                        OnLocateError();
                    }
                }
                else
                {
                    NavigationItem.LeftBarButtonItem = null;

                    iViewHourGlass.Stop();
                    iViewHourGlass.RemoveFromSuperview();

                    NavigationController.PopToRootViewController(false);

                    Location location = null;
                    foreach(IContainer c in aLocation.Containers)
                    {
                        if(location == null)
                        {
                            location = new Location(c);
                            iLocation = location;

                            Close();
                            Open();
                        }
                        else
                        {
                            location = new Location(location, c);

                            ViewWidgetBrowser controller = new ViewWidgetBrowser(location, iPlaySupport, iConfigController, iOptionInsertMode, iOptionBreadcrumbTrail);

                            NavigationController.PushViewController(controller, false);
                        }
                    }
                }
            });
        }

        private void CancelLocate(object sender, EventArgs e)
        {
            lock(iLock)
            {
                iLocatorAsync = null;
                Home();
            }
        }

        private object iLock;
        private uint iRetrys;

        private Location iRootLocation;
        private LocatorAsync iLocatorAsync;

        private ViewHourGlass iViewHourGlass;
    }

    partial class ViewWidgetBrowserRootIpad : ViewWidgetBrowserRoot
    {
        public ViewWidgetBrowserRootIpad(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            buttonHome.TouchUpInside += TouchUpInsideHome;
            buttonRetry.TouchUpInside += TouchUpInsideRetry;
        }

        protected override void OnLocateError()
        {
            base.OnLocateError();
            viewError.Hidden = false;
        }

        protected override void OpenActionSheet(UIActionSheet aActionSheet, CGRect aRect, UITableView aTableView)
        {
            aActionSheet.ShowFrom(aRect, aTableView, true);
        }

        private void TouchUpInsideRetry(object sender, EventArgs e)
        {
            viewError.Hidden = true;
            Retry();
        }

        private void TouchUpInsideHome(object sender, EventArgs e)
        {
            viewError.Hidden = true;
            Home();
        }

        private void LongPressGesture(UILongPressGestureRecognizer aRecogniser)
        {
            OnLongPressGesture(aRecogniser);
        }
    }

    partial class ViewWidgetBrowserRootIphone : ViewWidgetBrowserRoot
    {
        public ViewWidgetBrowserRootIphone(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            buttonHome.TouchUpInside += TouchUpInsideHome;
            buttonRetry.TouchUpInside += TouchUpInsideRetry;
        }

        protected override void OnLocateError()
        {
            base.OnLocateError();
            viewError.Hidden = false;
        }

        protected override void OpenActionSheet(UIActionSheet aActionSheet, CGRect aRect, UITableView aTableView)
        {
            //aActionSheet.ShowFromToolbar(NavigationController.Toolbar);
            aActionSheet.ShowFrom(new CGRect(0, aTableView.Frame.Bottom - 10, aTableView.Superview.Frame.Width, 10), aTableView.Superview, true);
        }

        private void TouchUpInsideRetry(object sender, EventArgs e)
        {
            viewError.Hidden = true;
            Retry();
        }

        private void TouchUpInsideHome(object sender, EventArgs e)
        {
            viewError.Hidden = true;
            Home();
        }

        private void LongPressGesture(UILongPressGestureRecognizer aRecogniser)
        {
            OnLongPressGesture(aRecogniser);
        }
    }
}
