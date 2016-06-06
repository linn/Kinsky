using System.Windows;
using System;
using Linn.Topology;
using Linn.Kinsky;
using System.Collections.Generic;
namespace KinskyDesktopWpf
{

    public interface IPlaylistItem
    {
        MrItem WrappedItem { get; set; }
        bool IsPlaying { get; set; }
        int Position { get; set; }
    }

    public class PlaylistSelectionEventArgs : EventArgs
    {
        public PlaylistSelectionEventArgs(IPlaylistItem aSelectedItem)
            : base()
        {
            SelectedItem = aSelectedItem;
        }

        public IPlaylistItem SelectedItem { get; set; }
    }
    public class PlaylistMoveEventArgs : EventArgs
    {
        public PlaylistMoveEventArgs(IPlaylistItem aSelectedItem)
            : base()
        {
            SelectedItem = aSelectedItem;
        }

        public IPlaylistItem SelectedItem { get; set; }
    }
    public class PlaylistDropEventArgs : EventArgs
    {
        public MediaProviderDraggable Data { get; set; }
        public int DropIndex { get; set; }
    }
    public class PlaylistDeleteEventArgs : EventArgs
    {
        public PlaylistDeleteEventArgs(List<IPlaylistItem> aDeletedItems)
            : base()
        {
            DeletedItems = aDeletedItems;
        }
        public List<IPlaylistItem> DeletedItems { get; set; }
    }

    public interface IPlaylistWidget
    {
        event EventHandler<PlaylistSelectionEventArgs> PlaylistSelectionChanged;
        event EventHandler<PlaylistSelectionEventArgs> EventPlaylistItemNavigationClick;
        event EventHandler<PlaylistDropEventArgs> PlaylistItemsAdded;
        event EventHandler<PlaylistDropEventArgs> PlaylistItemsMoved;
        event EventHandler<PlaylistDeleteEventArgs> PlaylistItemsDeleted;
        event EventHandler<PlaylistMoveEventArgs> PlaylistMoveUp;
        event EventHandler<PlaylistMoveEventArgs> PlaylistMoveDown;
        event EventHandler<EventArgs> PlaylistSave;

        void SetNowPlayingItem(IPlaylistItem aSelectedItem, bool aScrollToSelected);
        void SetLoading(bool aLoading, bool aIsInserting);
        bool AllowDrop { get; set; }
        bool GroupByAlbum { get; set; }
        List<IPlaylistItem> Items { set; }
        List<IPlaylistItem> SelectedItems();
        bool IsSaveEnabled { get; }
        void ScrollToNowPlaying();
    }

}