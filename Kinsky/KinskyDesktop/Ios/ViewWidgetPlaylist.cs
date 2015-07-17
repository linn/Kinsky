using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Linn;
using Linn.Topology;
using Linn.Kinsky;

using Upnp;

namespace KinskyTouch
{
    internal class ViewWidgetPlaylistMediaRenderer : IViewWidgetPlaylist, IControllerPlaylistMediaRenderer
    {
        private class Group
        {
            public Group(int aStartIndex, int aEndIndex)
            {
                iStartIndex = aStartIndex;
                iEndIndex = aEndIndex;
                iCount = aEndIndex - aStartIndex + 1;
            }

            public int StartIndex
            {
                get
                {
                    return iStartIndex;
                }
            }

            public int EndIndex
            {
                get
                {
                    return iEndIndex;
                }
            }

            public int Count
            {
                get
                {
                    return iCount;
                }
            }

            private int iStartIndex;
            private int iEndIndex;
            private int iCount;
        }

        private abstract class Item
        {
            public Item(int aIndex, upnpObject aObject)
            {
                iIndex = aIndex;
                iObject = aObject;
            }

            public int Index
            {
                get
                {
                    return iIndex;
                }
            }

            public abstract float RowHeight { get; }

            public abstract string Header { get; }
            public abstract string SubHeader1 { get; }
            public abstract string SubHeader2 { get; }
            public abstract string Duration { get; }

            public abstract UIImage ArtworkDefault { get; }
            public abstract Uri ArtworkUri { get; }

            protected  upnpObject iObject;
            private int iIndex;
        }

        private class ItemGroup : Item
        {
            public ItemGroup(int aIndex, int aCount, upnpObject aObject)
                :  base(aIndex, aObject)
            {
                iCount = aCount;
            }

            public int Count
            {
                get
                {
                    return iCount;
                }
            }

            public override float RowHeight
            {
                get
                {
                    return 73.0f;//120.0f;
                }
            }

            public override string Header
            {
                get
                {
                    return DidlLiteAdapter.Album(iObject);
                }
            }

            public override string SubHeader1
            {
                get
                {
                    string albumArtist = DidlLiteAdapter.AlbumArtist(iObject);
                    return (string.IsNullOrEmpty(albumArtist)) ? DidlLiteAdapter.Artist(iObject) : albumArtist;
                }
            }

            public override string SubHeader2
            {
                get
                {
                    return string.Empty;
                }
            }

            public override string Duration
            {
                get
                {
                    return string.Empty;
                }
            }

            public override UIImage ArtworkDefault
            {
                get
                {
                    return KinskyTouch.Properties.ResourceManager.Album;
                }
            }

            public override Uri ArtworkUri
            {
                get
                {
                    return DidlLiteAdapter.ArtworkUri(iObject);
                }
            }

            private int iCount;
        }

        private class ItemTrackGrouped : Item
        {
            public ItemTrackGrouped(int aIndex, upnpObject aObject)
                : base(aIndex, aObject)
            {
            }

            public override float RowHeight
            {
                get
                {
                    return 55.0f;
                }
            }

            public override string Header
            {
                get
                {
                    return DidlLiteAdapter.Title(iObject);
                }
            }

            public override string SubHeader1
            {
                get
                {
                    string albumArtist = DidlLiteAdapter.AlbumArtist(iObject);
                    string artist = DidlLiteAdapter.Artist(iObject);
                    if (!string.IsNullOrEmpty(albumArtist) && albumArtist != artist)
                    {
                        return artist;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override string SubHeader2
            {
                get
                {
                    return string.Empty;
                }
            }

            public override string Duration
            {
                get
                {
                    return DidlLiteAdapter.Duration(iObject);
                }
            }

            public override UIImage ArtworkDefault
            {
                get
                {
                    return null;
                }
            }

            public override Uri ArtworkUri
            {
                get
                {
                    return null;
                }
            }
        }

        private class ItemTrackUngrouped : Item
        {
            public ItemTrackUngrouped(int aIndex, upnpObject aObject)
                : base(aIndex, aObject)
            {
            }

            public override float RowHeight
            {
                get
                {
                    return 73.0f;
                }
            }

            public override string Header
            {
                get
                {
                    return DidlLiteAdapter.Title(iObject);
                }
            }

            public override string SubHeader1
            {
                get
                {
                    return DidlLiteAdapter.Album(iObject);
                }
            }

            public override string SubHeader2
            {
                get
                {
                    return DidlLiteAdapter.Artist(iObject);
                }
            }

            public override string Duration
            {
                get
                {
                    return DidlLiteAdapter.Duration(iObject);
                }
            }

            public override UIImage ArtworkDefault
            {
                get
                {
                    return KinskyTouch.Properties.ResourceManager.Track;
                }
            }

            public override Uri ArtworkUri
            {
                get
                {
                    return DidlLiteAdapter.ArtworkUri(iObject);
                }
            }
        }

        private class DataSource : UITableViewDataSource
        {
            public DataSource(UITableView aTableView, IControllerPlaylistMediaRenderer aControllerPlaylistMediaRenderer, List<Item> aItems, List<int> aTrackViewIndex, IList<MrItem> aPlaylist, MrItem aTrack)
            {
                iTableView = aTableView;
                iControllerPlaylistMediaRenderer = aControllerPlaylistMediaRenderer;
                iPlaylist = aPlaylist;
                iItems = aItems;
                iTrackViewIndex = aTrackViewIndex;
                iTrackIndexPath = NSIndexPath.FromRowSection(-1, -1);
                SetTrack(aTrack);
            }

            public void SetTrack(MrItem aItem)
            {
                int i = iPlaylist.IndexOf(aItem);
                int row = (i > -1) ? iTrackViewIndex[i] : -1;
                NSIndexPath index = NSIndexPath.FromRowSection(row, (row > -1) ? 0 : -1);

                if(iTrackIndexPath.Section > -1 && iTrackIndexPath.Row > -1)
                {
                    CellPlaylist cell = iTableView.CellAt(iTrackIndexPath) as CellPlaylist;
                    if(cell != null)
                    {
                        cell.SetPlaying(false, true);
                    }
                }

                if(index.Section > -1 && index.Row > -1)
                {
                    CellPlaylist cell = iTableView.CellAt(index) as CellPlaylist;
                    if(cell != null)
                    {
                        cell.SetPlaying(true, true);
                    }
                }

                iTrackIndexPath = index;
            }

            public NSIndexPath PlayingIndexPath
            {
                get
                {
                    return iTrackIndexPath;
                }
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iItems.Count;
            }

            public override nint NumberOfSections(UITableView aTableView)
            {
                return 1;
            }

            public override bool CanMoveRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                return true;
            }

            public override void MoveRow(UITableView aTableView, NSIndexPath aSourceIndexPath, NSIndexPath aDestinationIndexPath)
            {
                Trace.WriteLine(Trace.kKinskyTouch, "ViewWidgetPlaylistMediaRenderer.DataSource.MoveRow: section=" + aDestinationIndexPath.Section + ", row=" + aDestinationIndexPath.Row);

                try
                {
                    Item srcItem = iItems[aSourceIndexPath.Row];
                    Item destItem = iItems[aDestinationIndexPath.Row];

                    uint afterId = 0;
                    if(aDestinationIndexPath.Row > 0)
                    {
                        if(aSourceIndexPath.Row < aDestinationIndexPath.Row)
                        {
                            afterId = iPlaylist[destItem.Index].Id;
                        }
                        else
                        {
                            if(destItem.Index > 0 )
                            {
                                afterId = iPlaylist[destItem.Index - 1].Id;
                            }
                        }
                    }

                    if(srcItem is ItemGroup)
                    {
                        ItemGroup g = srcItem as ItemGroup;

                        List<MrItem> mrItems = new List<MrItem>();
                        int endIndex = g.Index + g.Count;
                        for(int i = g.Index; i < endIndex; ++i)
                        {
                            mrItems.Add(iPlaylist[i]);
                        }

                        iControllerPlaylistMediaRenderer.PlaylistMove(afterId, mrItems);
                    }
                    else
                    {
                        MrItem mrItem = iPlaylist[srcItem.Index];
    
                        iControllerPlaylistMediaRenderer.PlaylistMove(afterId, new List<MrItem> { mrItem });
                    }
                }
                catch(ArgumentOutOfRangeException)
                {
                    // handle case where user moves an item and the playlist changes before iOS calls the callback
                }
            }

            public override void CommitEditingStyle(UITableView aTableView, UITableViewCellEditingStyle aEditingStyle, NSIndexPath aIndexPath)
            {
                try
                {
                    if(aEditingStyle == UITableViewCellEditingStyle.Delete)
                    {
                        Item item = iItems[aIndexPath.Row];

                        if(item is ItemGroup)
                        {
                            ItemGroup g = item as ItemGroup;

                            List<NSIndexPath> paths = new List<NSIndexPath>();
                            int index = aIndexPath.Row + 1;     // start of group tracks

                            List<MrItem> mrItems = new List<MrItem>();

                            for(int i = 0; i < g.Count; ++i, ++index)
                            {
                                mrItems.Add(iPlaylist[g.Index]);

                                // update lists ahead of being evented
                                iPlaylist.RemoveAt(g.Index);
                                iTrackViewIndex.RemoveAt(g.Index);
                                iItems.RemoveAt(aIndexPath.Row + 1);
                                paths.Add(NSIndexPath.FromRowSection(index, 0));
                            }

                            // delete header too
                            iItems.RemoveAt(aIndexPath.Row);
                            paths.Add(NSIndexPath.FromRowSection(aIndexPath.Row, 0));

                            for(int i = g.Index; i < iTrackViewIndex.Count; ++i)
                            {
                                iTrackViewIndex[i] = --iTrackViewIndex[i];
                            }

                            aTableView.DeleteRows(paths.ToArray(), UITableViewRowAnimation.Right);

                            iControllerPlaylistMediaRenderer.PlaylistDelete(mrItems);
                        }
                        else
                        {
							MrItem itemToRemove = iPlaylist[item.Index];
                            iPlaylist.RemoveAt(item.Index);
                            iTrackViewIndex.RemoveAt(item.Index);
                            iItems.RemoveAt(aIndexPath.Row);

                            for(int i = item.Index; i < iTrackViewIndex.Count; ++i)
                            {
                                iTrackViewIndex[i] = --iTrackViewIndex[i];
                            }

                            aTableView.DeleteRows(new NSIndexPath[] { aIndexPath }, UITableViewRowAnimation.Right);

                            iControllerPlaylistMediaRenderer.PlaylistDelete(new List<MrItem> { itemToRemove });
                        }
                    }
                }
                catch(ArgumentOutOfRangeException)
                {
                    // playlist has been altered between user selecting an item to delete and iOS issuing the delete callback
                }
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                Item item = iItems[aIndexPath.Row];

                CellPlaylist cell = null;
                if(item is ItemTrackGrouped)
                {
                    cell = aTableView.DequeueReusableCell(kCellItemIdentifier) as CellPlaylistItem;

                    if(cell == null)
                    {
                        CellPlaylistItemFactory factory = new CellPlaylistItemFactory();
                        NSBundle.MainBundle.LoadNib("CellPlaylistItem", factory, null);
                        cell = factory.Cell;
                    }
                }
                else
                {
                    cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellPlaylist;

                    if(cell == null)
                    {
                        CellPlaylistFactory factory = new CellPlaylistFactory();
                        NSBundle.MainBundle.LoadNib("CellPlaylist", factory, null);
                        cell = factory.Cell;
                    }
                }

                cell.Image = item.ArtworkDefault;
                if(item is ItemGroup)
                {
                    cell.Title = item.Header;
                }
                else
                {
                    cell.Title = string.Format("{0}. {1}", item.Index + 1, item.Header);
                }
                cell.Album = item.SubHeader1;
                cell.Artist = item.SubHeader2;
                cell.DurationBitrate = item.Duration;
                cell.SetPlaying(((iTrackIndexPath.Section == aIndexPath.Section) && (iTrackIndexPath.Row == aIndexPath.Row)), false);
                cell.SetArtworkUri(item.ArtworkUri);
                cell.BackgroundView = new UIView();
                cell.BackgroundView.BackgroundColor = UIColor.Black;

                return cell;
            }

            private UITableView iTableView;
            private IControllerPlaylistMediaRenderer iControllerPlaylistMediaRenderer;
            private IList<MrItem> iPlaylist;
            private NSIndexPath iTrackIndexPath;
            private List<Item> iItems;
            private List<int> iTrackViewIndex;
        }

        private class Delegate : UITableViewDelegate
        {
			public Delegate(IControllerPlaylistMediaRenderer aControllerPlaylistMediaRenderer, List<Item> aItems, Action aAction)
            {
                iControllerPlaylistMediaRenderer = aControllerPlaylistMediaRenderer;
                iItems = aItems;
				iAction = aAction;
            }

            public override nfloat GetHeightForRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                return iItems[aIndexPath.Row].RowHeight;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);
                iControllerPlaylistMediaRenderer.SeekTrack((uint)iItems[aIndexPath.Row].Index);
            }

			public override void DraggingEnded(UIScrollView aScrollView, bool aWillDecelerate)
			{
				if(!aWillDecelerate)
				{
					iAction();
				}
			}

			public override void DecelerationEnded(UIScrollView aScrollView)
			{
				iAction();
			}

            private IControllerPlaylistMediaRenderer iControllerPlaylistMediaRenderer;
            private List<Item> iItems;
			private Action iAction;
        }

        public ViewWidgetPlaylistMediaRenderer(UITableView aTableView, SourceToolbar aToolbar, UIButton aButtonViewInfo, IViewSaveSupport aSaveSupport, OptionBool aOptionGroupTracks)
        {
            iTableView = aTableView;
            iToolbar = aToolbar;
            iSaveSupport = aSaveSupport;
            iOptionGroupTracks = aOptionGroupTracks;

            iGroups = new List<Group>();

            iButtonViewInfo = aButtonViewInfo;
        }

        public void Open()
        {
            iUpdatedSinceOpened = false;

            ArtworkCacheInstance.Instance.EventImageAdded += EventImageAdded;
            iOptionGroupTracks.EventValueChanged += OptionGroupTracksValueChanged;

            //iTableView.DraggingEnded += TableViewDraggingEnded;
            //iTableView.DecelerationEnded += TableViewDecelerationEnded;

            iButtonViewInfo.TouchUpInside += ViewInfoTouchUpInside;

            iOpen = true;
        }

        public void Close()
        {
            iDataSource = new ViewWidgetPlaylistMediaRenderer.DataSource(iTableView, this, new List<Item>(), new List<int>(), new List<MrItem>(), null);
            iTableView.DataSource = iDataSource;
			iTableView.Delegate = new ViewWidgetPlaylistMediaRenderer.Delegate(this, new List<Item>(), () => { iTableView.ReloadData(); });
            iTableView.ReloadData();

            iTableView.SetEditing(false, true);
            iTableView.Hidden = true;

            iToolbar.SetAllowEditing(false);
            iToolbar.SetAllowSaving(false);

            iToolbar.BarButtonItemEdit.Clicked -= EditClicked;
            iToolbar.BarButtonItemDone.Clicked -= DoneClicked;

            ArtworkCacheInstance.Instance.EventImageAdded -= EventImageAdded;
            iOptionGroupTracks.EventValueChanged -= OptionGroupTracksValueChanged;

            //iTableView.DraggingEnded -= TableViewDraggingEnded;
            //iTableView.DecelerationEnded -= TableViewDecelerationEnded;

            iButtonViewInfo.TouchUpInside -= ViewInfoTouchUpInside;

            iOpen = false;
        }

        public void Initialised()
        {
            if(iOpen)
            {
                iTableView.Hidden = false;
                iTableView.SetEditing(false, true);

                iToolbar.BarButtonItemEdit.Clicked += EditClicked;
                iToolbar.BarButtonItemDone.Clicked += DoneClicked;
            }
        }

        public void SetPlaylist(IList<MrItem> aPlaylist)
        {
            iPlaylist = aPlaylist;

            BuildGroupList();

            List<Item> items = new List<Item>();
            List<int> trackViewIndex = new List<int>();
            foreach(Group g in iGroups)
            {
                if(iOptionGroupTracks.Native)
                {
                    if(g.Count > 1)
                    {
                        items.Add(new ItemGroup(g.StartIndex, g.Count, iPlaylist[g.StartIndex].DidlLite[0]));
    
                        int endIndex = g.StartIndex + g.Count;
                        for(int i = g.StartIndex; i < endIndex; ++i)
                        {
                            trackViewIndex.Add(items.Count);
                            items.Add(new ItemTrackGrouped(i, iPlaylist[i].DidlLite[0]));
                        }
                    }
                    else
                    {
                        trackViewIndex.Add(items.Count);
                        items.Add(new ItemTrackUngrouped(g.StartIndex, iPlaylist[g.StartIndex].DidlLite[0]));
                    }
                }
                else
                {
                    int endIndex = g.StartIndex + g.Count;
                    for(int i = g.StartIndex; i < endIndex; ++i)
                    {
                        trackViewIndex.Add(items.Count);
                        items.Add(new ItemTrackUngrouped(i, iPlaylist[i].DidlLite[0]));
                    }
                }
            }

            if(iOpen)
            {
                if(aPlaylist.Count > 0)
                {
                    iToolbar.SetAllowEditing(true);
                    iToolbar.SetAllowSaving(true);
                }
                else
                {
                    iTableView.SetEditing(false, true);
                    iToolbar.SetEditing(false);
                    iToolbar.SetAllowEditing(false);
                    iToolbar.SetAllowSaving(false);
                }

                iDataSource = new ViewWidgetPlaylistMediaRenderer.DataSource(iTableView, this, items, trackViewIndex, aPlaylist, iTrack);
                iTableView.DataSource = iDataSource;
				iTableView.Delegate = new ViewWidgetPlaylistMediaRenderer.Delegate(this, items, () => { iTableView.ReloadData(); });
                iTableView.ReloadData();

                iDataSource.SetTrack(iTrack);

                if(!iUpdatedSinceOpened)
                {
                    ScrollToCurrent();
                    iUpdatedSinceOpened = true;
                }

                //SetTrack(iTrack);
            }
        }

        public void SetTrack(MrItem aTrack)
        {
            if(iDataSource != null)
            {
                iDataSource.SetTrack(aTrack);
            }
            iTrack = aTrack;
        }

        public void Save()
        {
            List<upnpObject> list = new List<upnpObject>();

            if(iPlaylist == null)
            {
                return;
            }

            foreach (MrItem track in iPlaylist)
            {
                list.Add(track.DidlLite[0]);
            }

            iSaveSupport.Save(list);
        }

        public void Delete()
        {
            if(EventPlaylistDeleteAll!=null)
            {
                EventPlaylistDeleteAll(this, EventArgs.Empty);
            }
        }

        public void SeekTrack(uint aIndex)
        {
            if(EventSeekTrack != null)
            {
                EventSeekTrack(this, new EventArgsSeekTrack(aIndex));
            }
        }

        public void PlaylistInsert(uint aInsertAfterId, IMediaRetriever aMediaRetriever)
        {
        }

        public void PlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems)
        {
            if(EventPlaylistMove != null)
            {
                EventPlaylistMove(this, new EventArgsPlaylistMove(aInsertAfterId, aPlaylistItems));
            }
        }

        public void PlaylistDelete(IList<MrItem> aPlaylistItems)
        {
            if(EventPlaylistDelete != null)
            {
                EventPlaylistDelete(this, new EventArgsPlaylistDelete(aPlaylistItems));
            }
        }

        public void PlaylistDeleteAll()
        {
        }

        public event EventHandler<EventArgsSeekTrack> EventSeekTrack;
        public event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;
        public event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;
        public event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;
        public event EventHandler<EventArgs> EventPlaylistDeleteAll;

        private void ScrollToCurrent()
        {
            // scroll to current track
            if(iDataSource.PlayingIndexPath.Row != -1 && iDataSource.PlayingIndexPath.Section != -1 && iPlaylist.Count > 0)
            {
                iTableView.ScrollToRow(iDataSource.PlayingIndexPath, UITableViewScrollPosition.Middle, false);
            }
        }

        private void ViewInfoTouchUpInside(object sender, EventArgs e)
        {
            if(iOpen)
            {
                ScrollToCurrent();
            }
        }

        private void EventImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
        {
            iTableView.ReloadData();
        }

        /*private void TableViewDecelerationEnded(object sender, EventArgs e)
        {
            iTableView.ReloadData();
        }

        private void TableViewDraggingEnded(object sender, DraggingEventArgs e)
        {
            if(!e.Decelerate)
            {
                iTableView.ReloadData();
            }
        }*/

        private void EditClicked(object sender, EventArgs e)
        {
            if(iOpen)
            {
                iTableView.SetEditing(true, true);
            }
        }

        private void DoneClicked(object sender, EventArgs e)
        {
            if(iOpen)
            {
                iTableView.SetEditing(false, true);
            }
        }

        private void BuildGroupList()
        {
            iGroups.Clear();

            int firstIndex = 0;
            while (firstIndex < iPlaylist.Count)
            {
                // group is currently defined by album title and album artist
                string groupAlbum = DidlLiteAdapter.Album(iPlaylist[firstIndex].DidlLite[0]);
                string groupArtist = DidlLiteAdapter.AlbumArtist(iPlaylist[firstIndex].DidlLite[0]);

                int lastIndex = firstIndex + 1;

                // if the album field is empty, then this playlist item is in its own group of 1 item,
                // if not, determine the subsequent tracks that are part of the group
                if (!string.IsNullOrEmpty(groupAlbum))
                {
                    while (lastIndex < iPlaylist.Count)
                    {
                        string album = DidlLiteAdapter.Album(iPlaylist[lastIndex].DidlLite[0]);
                        string artist = DidlLiteAdapter.AlbumArtist(iPlaylist[lastIndex].DidlLite[0]);

                        if (album == groupAlbum && artist == groupArtist)
                        {
                            // this track is part of the current group - move to next track
                            lastIndex++;
                        }
                        else
                        {
                            // this track is not part of the current group
                            break;
                        }
                    }
                }

                // lastIndex is currently the first item of the next group
                lastIndex--;

                // add the group
                iGroups.Add(new Group(firstIndex, lastIndex));

                // move to start of next group
                firstIndex = lastIndex + 1;
            }
        }

        private void OptionGroupTracksValueChanged(object sender, EventArgs e)
        {
            SetPlaylist(iPlaylist);
        }

        private static NSString kCellIdentifier = new NSString("CellPlaylist");
        private static NSString kCellItemIdentifier = new NSString("CellPlaylistItem");

        private bool iOpen;
        private bool iUpdatedSinceOpened;

        private IViewSaveSupport iSaveSupport;
        private OptionBool iOptionGroupTracks;

        private List<Group> iGroups;
        private IList<MrItem> iPlaylist;
        private DataSource iDataSource;
        private MrItem iTrack;

        private UITableView iTableView;
        private SourceToolbar iToolbar;
        private UIButton iButtonViewInfo;
    }

    internal class ViewWidgetPlaylistRadio : IViewWidgetPlaylistRadio, IControllerPlaylistRadio
    {
        private class DataSource : UITableViewDataSource
        {
            public DataSource(UITableView aTableView, IList<MrItem> aPresets)
            {
                iTableView = aTableView;
                iPresets = aPresets;
                iPresetIndexPath = NSIndexPath.FromRowSection(-1, -1);
            }

            public void SetPreset(int aPresetIndex)
            {
                if(iPresetIndexPath.Row > -1 && iPresetIndexPath.Row < iPresets.Count)
                {
                    CellPlaylist cell = iTableView.CellAt(iPresetIndexPath) as CellPlaylist;
                    if(cell != null)
                    {
                        cell.SetPlaying(false, true);
                    }
                }

                if(aPresetIndex > -1)
                {
                    CellPlaylist cell = iTableView.CellAt(NSIndexPath.FromRowSection(aPresetIndex, 0)) as CellPlaylist;
                    if(cell != null)
                    {
                        cell.SetPlaying(true, true);
                    }
                }

                iPresetIndexPath = NSIndexPath.FromRowSection(aPresetIndex, 0);
            }

            public NSIndexPath PlayingIndexPath
            {
                get
                {
                    return iPresetIndexPath;
                }
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iPresets.Count;
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellPlaylist cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellPlaylist;
                if(cell == null)
                {
                    CellPlaylistFactory factory = new CellPlaylistFactory();
                    NSBundle.MainBundle.LoadNib("CellPlaylist", factory, null);
                    cell = factory.Cell;
                }

                cell.Image = KinskyTouch.Properties.ResourceManager.SourceRadio;
                cell.Title = string.Format("{0}. {1}", aIndexPath.Row + 1, DidlLiteAdapter.Title(iPresets[aIndexPath.Row].DidlLite[0]));
                cell.Artist = string.Empty;
                cell.Album = string.Empty;
                cell.DurationBitrate = DidlLiteAdapter.Bitrate(iPresets[aIndexPath.Row].DidlLite[0]);
                cell.SetPlaying((iPresetIndexPath.Row == aIndexPath.Row), false);

                Uri uri = DidlLiteAdapter.ArtworkUri(iPresets[aIndexPath.Row].DidlLite[0]);
                cell.SetArtworkUri(uri);

                cell.BackgroundView = new UIView();
                cell.BackgroundView.BackgroundColor = UIColor.Black;
                return cell;
            }

            private UITableView iTableView;
            private IList<MrItem> iPresets;
            private NSIndexPath iPresetIndexPath;
        }

        private class Delegate : UITableViewDelegate
        {
			public Delegate(IControllerPlaylistRadio aControllerPlaylistRadio, Action aAction)
            {
                iControllerPlaylistRadio = aControllerPlaylistRadio;
				iAction = aAction;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);
                iControllerPlaylistRadio.SelectPreset(aIndexPath.Row);
            }

			public override void DraggingEnded(UIScrollView aScrollView, bool aWillDecelerate)
			{
				if(!aWillDecelerate)
				{
					iAction();
				}
			}

			public override void DecelerationEnded (UIScrollView scrollView)
			{
				iAction();
			}

            private IControllerPlaylistRadio iControllerPlaylistRadio;
			private Action iAction;
        }

        public ViewWidgetPlaylistRadio(UITableView aTableView, UIButton aButtonViewInfo, IViewSaveSupport aSaveSupport)
        {
            iTableView = aTableView;
            iSaveSupport = aSaveSupport;

            iButtonViewInfo = aButtonViewInfo;
        }

        public void Open()
        {
            iUpdatedSinceOpened = false;

            ArtworkCacheInstance.Instance.EventImageAdded += EventImageAdded;

            //iTableView.DraggingEnded += TableViewDraggingEnded;
            //iTableView.DecelerationEnded += TableViewDecelerationEnded;

            iButtonViewInfo.TouchUpInside += ViewInfoTouchUpInside;

            iOpen = true;
        }

        public void Close()
        {
            iPresets = null;

            iDataSource = new ViewWidgetPlaylistRadio.DataSource(iTableView, new List<MrItem>());
            iTableView.DataSource = iDataSource;
			iTableView.Delegate = new ViewWidgetPlaylistRadio.Delegate(this, () => { iTableView.ReloadData(); });
            iTableView.ReloadData();

            iTableView.Hidden = true;

            ArtworkCacheInstance.Instance.EventImageAdded -= EventImageAdded;

            //iTableView.DraggingEnded -= TableViewDraggingEnded;
            //iTableView.DecelerationEnded -= TableViewDecelerationEnded;

            iButtonViewInfo.TouchUpInside -= ViewInfoTouchUpInside;

            iOpen = false;
        }

        public void Initialised()
        {
            if(iOpen)
            {
                iTableView.Hidden = false;
            }
        }

        public void SetPresets(IList<MrItem> aPresets)
        {
            if(iOpen)
            {
                iPresets = aPresets;

                iDataSource = new ViewWidgetPlaylistRadio.DataSource(iTableView, aPresets);
                iTableView.DataSource = iDataSource;
				iTableView.Delegate = new ViewWidgetPlaylistRadio.Delegate(this, () => { iTableView.ReloadData(); });
                iTableView.ReloadData();

                iDataSource.SetPreset(iPresetIndex);

                if(!iUpdatedSinceOpened)
                {
                    ScrollToCurrent();
                    iUpdatedSinceOpened = true;
                }
            }
        }

        public void SetChannel(Channel aChannel)
        {
            iChannel = aChannel;
        }

        public void SetPreset(int aPresetIndex)
        {
            if(iDataSource != null)
            {
                iDataSource.SetPreset(aPresetIndex);
            }
            iPresetIndex = aPresetIndex;
        }

        public void Save()
        {
            List<upnpObject> list = new List<upnpObject>();

            if(iChannel == null)
            {
                return;
            }

            list.AddRange(iChannel.DidlLite);

            iSaveSupport.Save(list);
        }

        public void SelectPreset(int aPresetIndex)
        {
            MrItem preset = null;

            if(aPresetIndex > -1 && aPresetIndex < iPresets.Count)
            {
                preset = iPresets[aPresetIndex];
            }

            if(EventSetPreset != null)
            {
                EventSetPreset(this, new EventArgsSetPreset(preset));
            }
        }

        public void SelectChannel(Channel aChannel)
        {
        }

        public event EventHandler<EventArgsSetPreset> EventSetPreset;
        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        private void ScrollToCurrent()
        {
            if(iDataSource.PlayingIndexPath.Row != -1 && iDataSource.PlayingIndexPath.Section != -1 && iPresets.Count > 0)
            {
                iTableView.ScrollToRow(iDataSource.PlayingIndexPath, UITableViewScrollPosition.Middle, false);
            }
        }

        private void ViewInfoTouchUpInside(object sender, EventArgs e)
        {
            if(iOpen)
            {
                ScrollToCurrent();
            }
        }

        private void EventImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
        {
            iTableView.ReloadData();
        }

        /*private void TableViewDecelerationEnded(object sender, EventArgs e)
        {
            iTableView.ReloadData();
        }

        private void TableViewDraggingEnded(object sender, DraggingEventArgs e)
        {
            if(!e.Decelerate)
            {
                iTableView.ReloadData();
            }
        }*/

        private static NSString kCellIdentifier = new NSString("CellPlaylist");

        private bool iOpen;
        private bool iUpdatedSinceOpened;

        private IViewSaveSupport iSaveSupport;

        private DataSource iDataSource;
        private UITableView iTableView;
        private UIButton iButtonViewInfo;

        private int iPresetIndex;
        private Channel iChannel;
        private IList<MrItem> iPresets;
    }

    internal class ViewWidgetPlaylistDiscPlayer : IViewWidgetPlaylistDiscPlayer
    {
        public ViewWidgetPlaylistDiscPlayer(UIImageView aImageView)
        {
            iImageView = aImageView;
        }

        public void Open()
        {
            iImageView.Image = KinskyTouch.Properties.ResourceManager.SourceDisc;
            iImageView.Hidden = false;
        }

        public void Close()
        {
            iImageView.Image = null;
            iImageView.Hidden = true;
        }

        public void Initialised()
        {
        }

        public void Eject()
        {
        }

        private UIImageView iImageView;
    }

    internal class ViewWidgetPlaylistReceiver : IViewWidgetPlaylistReceiver, IViewWidgetSelector<Linn.Kinsky.Room>, IControllerPlaylistReceiver, IControllerRoomSelector
    {
        private class DataSource : UITableViewDataSource
        {
            public DataSource(UITableView aTableView, IList<ModelSender> aSenders, IList<Linn.Kinsky.Room> aRooms, IControllerRoomSelector aController)
            {
                iTableView = aTableView;
                iSenders = aSenders;
                iRooms = new List<Linn.Kinsky.Room>(aRooms);
                iController = aController;
                iPlayingIndexPath = NSIndexPath.FromRowSection(-1, -1);
            }

            public void SetSender(ModelSender aSender)
            {
                ModelSender oldSender = iSender;
                iSender = aSender;

                if(oldSender != null)
                {
                    for(int i = 0; i < iSenders.Count; ++i)
                    {
                        if(iSenders[i] == oldSender)
                        {
                            CellSender cell = iTableView.CellAt(NSIndexPath.FromRowSection(i, 0)) as CellSender;
                            if(cell != null)
                            {
                                cell.SetRoom(null);
                                cell.SetPlaying(false, true);
                            }
                            break;
                        }
                    }
                }

                NSIndexPath indexPath = NSIndexPath.FromRowSection(-1, -1);
                if(aSender != null)
                {
                    for(int i = 0; i < iSenders.Count; ++i)
                    {
                        if(iSenders[i] == aSender)
                        {
                            CellSender cell = iTableView.CellAt(NSIndexPath.FromRowSection(i, 0)) as CellSender;
                            if(cell != null)
                            {
                                cell.SetRoom(RoomFor(aSender));
                                cell.SetPlaying(true, true);
                            }
                            indexPath = NSIndexPath.FromRowSection(i, 0);
                            break;
                        }
                    }
                }

                iPlayingIndexPath = indexPath;
            }

            public NSIndexPath PlayingIndexPath
            {
                get
                {
                    return iPlayingIndexPath;
                }
            }

            public void AddRoom(Linn.Kinsky.Room aRoom)
            {
                iRooms.Add(aRoom);

                for(int i = 0; i < iSenders.Count; ++i)
                {
                    if(iSenders[i].Room == aRoom.Name)
                    {
                        BeginInvokeOnMainThread(delegate {
                            iTableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(i, 0) }, UITableViewRowAnimation.Fade);
                        });
                        break;
                    }
                }
            }

            public void RemoveRoom(Linn.Kinsky.Room aRoom)
            {
                iRooms.Remove(aRoom);

                for(int i = 0; i < iSenders.Count; ++i)
                {
                    if(iSenders[i].Room == aRoom.Name)
                    {
                        BeginInvokeOnMainThread(delegate {
                            iTableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(i, 0) }, UITableViewRowAnimation.Fade);
                        });
                        break;
                    }
                }
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iSenders.Count;
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellSender cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellSender;

                if(cell == null)
                {
                    CellSenderController controller = new CellSenderController(iController);
                    NSBundle.MainBundle.LoadNib("CellSender", controller, null);
                    controller.Initialise();
                    cell = controller.Cell;
                }

                ModelSender sender = iSenders[aIndexPath.Row];

                cell.Image = KinskyTouch.Properties.ResourceManager.SourceSongcast;
                cell.Title = sender.FullName;
                cell.SetRoom(RoomFor(sender));
                cell.SetPlaying(sender == iSender, false);

                Uri uri = DidlLiteAdapter.ArtworkUri(sender.Metadata[0]);
                cell.SetArtworkUri(uri);

                return cell;
            }

            public ModelSender SenderAt(NSIndexPath aIndexPath)
            {
                return iSenders[aIndexPath.Row];
            }

            private Linn.Kinsky.Room RoomFor(ModelSender aSender)
            {
                foreach(Linn.Kinsky.Room r in iRooms)
                {
                    if(aSender.Room == r.Name)
                    {
                        return r;
                    }
                }

                return null;
            }

            private ModelSender iSender;
            private NSIndexPath iPlayingIndexPath;

            private UITableView iTableView;
            private IList<ModelSender> iSenders;
            private List<Linn.Kinsky.Room> iRooms;
            private IControllerRoomSelector iController;
        }

        private class Delegate : UITableViewDelegate
        {
			public Delegate(DataSource aDataSource, IControllerPlaylistReceiver aController, Action aAction)
            {
                iDataSource = aDataSource;
                iController = aController;
				iAction = aAction;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);
                iController.SelectSender(iDataSource.SenderAt(aIndexPath));
            }

			public override void DraggingEnded(UIScrollView aScrollView, bool aWillDecelerate)
			{
				if(!aWillDecelerate) 
				{
					iAction();
				}
			}

			public override void DecelerationEnded(UIScrollView aScrollView)
			{
				iAction();
			}

            private DataSource iDataSource;
            private IControllerPlaylistReceiver iController;
			private Action iAction;
        }

        public ViewWidgetPlaylistReceiver(UITableView aTableView, UIButton aButtonViewInfo, UIImageView aImageView, IViewSaveSupport aSaveSupport)
        {
            iTableView = aTableView;
            iImageView = aImageView;
            iSaveSupport = aSaveSupport;

            iRooms = new SortedList<string, Linn.Kinsky.Room>();

            iButtonViewInfo = aButtonViewInfo;
        }

        void IViewWidgetPlaylistReceiver.Open()
        {
            iUpdatedSinceOpened = false;

            ArtworkCacheInstance.Instance.EventImageAdded += EventImageAdded;

            //iTableView.DraggingEnded += TableViewDraggingEnded;
            //iTableView.DecelerationEnded += TableViewDecelerationEnded;

            iButtonViewInfo.TouchUpInside += ViewInfoTouchUpInside;

            iPlaylistReceiverOpen = true;
        }

        void IViewWidgetPlaylistReceiver.Close()
        {
            iImageView.Image = new UIImage();
            iImageView.Hidden = true;

            iDataSource = new DataSource(iTableView, new List<ModelSender>(), new List<Linn.Kinsky.Room>(), this);
            iTableView.DataSource = iDataSource;
			iTableView.Delegate = new Delegate(iDataSource, this, () => { iTableView.ReloadData(); });
            iTableView.ReloadData();

            iTableView.Hidden = true;

            ArtworkCacheInstance.Instance.EventImageAdded -= EventImageAdded;

            //iTableView.DraggingEnded -= TableViewDraggingEnded;
            //iTableView.DecelerationEnded -= TableViewDecelerationEnded;

            iButtonViewInfo.TouchUpInside -= ViewInfoTouchUpInside;

            iPlaylistReceiverOpen = false;
        }

        public void Initialised()
        {
            if(iPlaylistReceiverOpen)
            {
                iImageView.Image = kIconReceiverSource;
                iImageView.Hidden = false;

                iTableView.Hidden = false;
            }
        }

        public void SetSenders(IList<ModelSender> aSenders)
        {
            iSenders = aSenders;

            iDataSource = new DataSource(iTableView, aSenders, iRooms.Values, this);
            iTableView.DataSource = iDataSource;
			iTableView.Delegate = new Delegate(iDataSource, this, () => { iTableView.ReloadData(); });
            iTableView.ReloadData();

            SetChannel(iChannel);

            if(!iUpdatedSinceOpened)
            {
                ScrollToCurrent();
                iUpdatedSinceOpened = true;
            }
        }

        public void SetChannel(Channel aChannel)
        {
            iSender = null;
            iChannel = aChannel;

            if(iSenders != null && aChannel != null)
            {
                foreach(ModelSender s in iSenders)
                {
                    foreach(resource r in s.Metadata[0].Res)
                    {
                        if(aChannel.Uri == r.Uri)
                        {
                            iSender = s;
                            break;
                        }
                    }

                    if(iSender != null)
                    {
                        break;
                    }
                }

                if(iDataSource != null)
                {
                    iDataSource.SetSender(iSender);
                }
            }
        }

        public void Save()
        {
            List<upnpObject> list = new List<upnpObject>();

            if(iChannel == null)
            {
                return;
            }

            list.AddRange(iChannel.DidlLite);

            iSaveSupport.Save(list);
        }

        public void SelectSender(ModelSender aSender)
        {
            if(iPlaylistReceiverOpen)
            {
                if(EventSetChannel != null)
                {
                    EventSetChannel(this, new EventArgsSetChannel(new MediaRetrieverNoRetrieve(aSender.Metadata)));
                }
            }
        }

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        void IViewWidgetSelector<Linn.Kinsky.Room>.Open()
        {
            Assert.Check(!iSelectorRoomOpen);
            iSelectorRoomOpen = true;
        }

        void IViewWidgetSelector<Linn.Kinsky.Room>.Close()
        {
            if(iSelectorRoomOpen)
            {
                iRooms.Clear();
            }

            iSelectorRoomOpen = false;
        }

        public void InsertItem(int aIndex, Linn.Kinsky.Room aItem)
        {
            if(iSelectorRoomOpen)
            {
                iRooms.Add(aItem.Name, aItem);
            }
        }

        public void RemoveItem(Linn.Kinsky.Room aItem)
        {
            if(iSelectorRoomOpen)
            {
                iRooms.Remove(aItem.Name);
            }
        }

        public void ItemChanged(Linn.Kinsky.Room aItem)
        {
        }

        public void SetSelected(Linn.Kinsky.Room aItem)
        {
        }

        public void Select(Linn.Kinsky.Room aRoom)
        {
            if(EventSelectionChanged != null)
            {
                EventSelectionChanged(this, new EventArgsSelection<Linn.Kinsky.Room>(aRoom));
            }
        }

        public event EventHandler<EventArgsSelection<Linn.Kinsky.Room>> EventSelectionChanged;

        private void ScrollToCurrent()
        {
            if(iDataSource.PlayingIndexPath.Row != -1 && iDataSource.PlayingIndexPath.Section != -1 && iSenders.Count > 0)
            {
                iTableView.ScrollToRow(iDataSource.PlayingIndexPath, UITableViewScrollPosition.Middle, false);
            }
        }

        private void ViewInfoTouchUpInside(object sender, EventArgs e)
        {
            if(iPlaylistReceiverOpen)
            {
                ScrollToCurrent();
            }
        }

        private void EventImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
        {
            iTableView.ReloadData();
        }

        /*private void TableViewDecelerationEnded(object sender, EventArgs e)
        {
            iTableView.ReloadData();
        }

        private void TableViewDraggingEnded(object sender, DraggingEventArgs e)
        {
            if(!e.Decelerate)
            {
                iTableView.ReloadData();
            }
        }*/

        private static NSString kCellIdentifier = new NSString("CellSender");
        private static UIImage kIconReceiverSource = new UIImage("Sender.png");

        private bool iPlaylistReceiverOpen;
        private bool iUpdatedSinceOpened;

        private IViewSaveSupport iSaveSupport;

        private DataSource iDataSource;

        private Channel iChannel;
        private ModelSender iSender;
        private IList<ModelSender> iSenders;

        private bool iSelectorRoomOpen;
        private SortedList<string, Linn.Kinsky.Room> iRooms;

        private UITableView iTableView;
        private UIImageView iImageView;
        private UIButton iButtonViewInfo;
    }

    internal class ViewWidgetPlaylistAux : IViewWidgetPlaylistAux
    {
        public ViewWidgetPlaylistAux(UIImageView aImageView)
        {
            iImageView = aImageView;
        }

        public void Open(string aType)
        {
            iImageView.Image = KinskyTouch.Properties.ResourceManager.SourceExternal;
            iImageView.Hidden = false;
        }

        public void Close()
        {
            iImageView.Image = null;
            iImageView.Hidden = true;
        }

        private UIImageView iImageView;
    }
}