
using System;
using System.Collections.Generic;

using Linn;


namespace OssKinskyMppItunes
{
    public class Database
    {
        public Database()
        {
            iNextId = 0;
            iTopLevelContainerList = new List<TopLevelContainer>();

            iRoot = new NodeRoot("iTunes", iNextId, null);
            iNextId++;

            iPlaylistBuilder = new NodePlaylistBuilder();

            AddTopLevelContainer("All Tracks", new NodeItemBuilder(new OriginalItemFactory()));
            AddTopLevelContainer("Playlists", iPlaylistBuilder);
            iPlaylistContainer = iTopLevelContainerList[iTopLevelContainerList.Count - 1].Container;
        }

        public NodeContainer Root
        {
            get { return iRoot; }
        }

        public void AddTopLevelContainer(string aName, INodeBuilder aBuilder)
        {
            NodeContainer container = new NodeContainer(aName, iNextId, null);
            iNextId++;

            TopLevelContainer c = new TopLevelContainer();
            c.Container = container;
            c.Builder = aBuilder;
            iTopLevelContainerList.Add(c);

            iRoot.Add(container.Name, container);
        }

        public void Add(LibraryItem aItem)
        {
            ItemDesc desc = new ItemDesc();
            desc.LibItem = aItem;
            desc.Id = iNextId;
            iNextId++;

            foreach (TopLevelContainer c in iTopLevelContainerList)
            {
                c.Builder.Add(c.Container, desc, ref iNextId);
            }
        }

        public void Add(LibraryPlaylist aPlaylist)
        {
            iPlaylistBuilder.Add(iPlaylistContainer, aPlaylist, ref iNextId);
        }

        private struct TopLevelContainer
        {
            public NodeContainer Container;
            public INodeBuilder Builder;
        }

        private NodeContainer iRoot;
        private List<TopLevelContainer> iTopLevelContainerList;
        private NodePlaylistBuilder iPlaylistBuilder;
        private NodeContainer iPlaylistContainer;
        private UInt32 iNextId;
    }



    public struct ItemDesc
    {
        public LibraryItem LibItem;
        public UInt32 Id;
    }



    public interface INode
    {
        UInt32 Id { get; }
        uint ChildCount { get; }
        INode Child(uint aIndex);
        void Process(INodeProcessor aProcessor);
    }


    public class NodeItem : INode
    {
        public NodeItem(UInt32 aId, UInt32 aRefId, LibraryItem aLibraryItem)
        {
            iId = aId;
            iRefId = aRefId;
            iLibraryItem = aLibraryItem;
        }

        public UInt32 Id
        {
            get { return iId; }
        }

        public uint ChildCount
        {
            get { return 0; }
        }

        public INode Child(uint aIndex)
        {
            Assert.Check(false);
            return null;
        }

        public void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }

        public UInt32 RefId
        {
            get { return iRefId; }
        }

        public LibraryItem LibraryItem
        {
            get { return iLibraryItem; }
        }

        private UInt32 iId;
        private UInt32 iRefId;
        private LibraryItem iLibraryItem;
    }


    public class NodeContainer : INode
    {
        public NodeContainer(string aName, UInt32 aId, IComparer<NodeItem> aComparer)
        {
            iName = aName;
            iId = aId;
            iContainerDict = new SortedList<string, NodeContainer>();
            iItemList = new List<NodeItem>();
            iComparer = aComparer;
        }

        public UInt32 Id
        {
            get { return iId; }
        }

        public uint ChildCount
        {
            get { return (uint)(iContainerDict.Count + iItemList.Count); }
        }

        public INode Child(uint aIndex)
        {
            Assert.Check(aIndex < ChildCount);

            if (aIndex < iContainerDict.Count)
                return iContainerDict.Values[(int)aIndex];
            else
                return iItemList[(int)aIndex - iContainerDict.Count];
        }

        public virtual void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }

        public string Name
        {
            get { return iName; }
        }

        public IList<NodeContainer> ContainerList
        {
            get { return iContainerDict.Values; }
        }

        public IList<NodeItem> ItemList
        {
            get { return iItemList.AsReadOnly(); }
        }

        public NodeContainer Container(string aKey)
        {
            if (iContainerDict.ContainsKey(aKey))
                return iContainerDict[aKey];
            else
                return null;
        }

        public void Add(string aKey, NodeContainer aContainer)
        {
            iContainerDict.Add(aKey, aContainer);
        }

        public void Add(NodeItem aItem)
        {
            iItemList.Add(aItem);
            if (iComparer != null)
            {
                iItemList.Sort(iComparer);
            }
        }

        private UInt32 iId;
        private string iName;
        private SortedList<string, NodeContainer> iContainerDict;
        private List<NodeItem> iItemList;
        private IComparer<NodeItem> iComparer;
    }


    public class NodeRoot : NodeContainer
    {
        public NodeRoot(string aName, UInt32 aId, IComparer<NodeItem> aComparer)
            : base(aName, aId, aComparer)
        {
        }

        public override void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }
    }


    public class NodeAlbum : NodeContainer
    {
        public NodeAlbum(string aName, UInt32 aId, IComparer<NodeItem> aComparer)
            : base(aName, aId, aComparer)
        {
        }

        public override void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }
    }


    public class NodeArtist : NodeContainer
    {
        public NodeArtist(string aName, UInt32 aId, IComparer<NodeItem> aComparer)
            : base(aName, aId, aComparer)
        {
        }

        public override void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }
    }


    public class NodeGenre : NodeContainer
    {
        public NodeGenre(string aName, UInt32 aId, IComparer<NodeItem> aComparer)
            : base(aName, aId, aComparer)
        {
        }

        public override void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }
    }


    public class NodePlaylist : NodeContainer
    {
        public NodePlaylist(string aName, UInt32 aId, IComparer<NodeItem> aComparer)
            : base(aName, aId, aComparer)
        {
        }

        public override void Process(INodeProcessor aProcessor)
        {
            aProcessor.Process(this);
        }
    }


    public class ComparerAlbumTrackNumber : IComparer<NodeItem>
    {
        public ComparerAlbumTrackNumber(Library aLibrary)
        {
            iLibrary = aLibrary;
        }

        public int Compare(NodeItem aItem1, NodeItem aItem2)
        {
            LibraryItem libItem1 = aItem1.LibraryItem;
            LibraryItem libItem2 = aItem2.LibraryItem;

            if (string.IsNullOrEmpty(libItem1.Album) && string.IsNullOrEmpty(libItem2.Album))
                return 0;
            else if (string.IsNullOrEmpty(libItem1.Album))
                return 1;
            else if (string.IsNullOrEmpty(libItem2.Album))
                return -1;

            int albumCmp = libItem1.Album.CompareTo(libItem2.Album);
            if (albumCmp != 0)
            {
                return albumCmp;
            }
            else
            {
                if (libItem1.TrackNumber > libItem2.TrackNumber)
                    return 1;
                else if (libItem1.TrackNumber < libItem2.TrackNumber)
                    return -1;
                else
                    return 0;
            }
        }

        private Library iLibrary;
    }

    public class ComparerTrackNumber : IComparer<NodeItem>
    {
        public ComparerTrackNumber(Library aLibrary)
        {
            iLibrary = aLibrary;
        }

        public int Compare(NodeItem aItem1, NodeItem aItem2)
        {
            LibraryItem libItem1 = aItem1.LibraryItem;
            LibraryItem libItem2 = aItem2.LibraryItem;
            if (libItem1.TrackNumber > libItem2.TrackNumber)
                return 1;
            else if (libItem1.TrackNumber < libItem2.TrackNumber)
                return -1;
            else
                return 0;
        }

        private Library iLibrary;
    }



    public interface INodeProcessor
    {
        void Process(NodeItem aNode);
        void Process(NodeContainer aNode);
        void Process(NodeRoot aNode);
        void Process(NodeAlbum aNode);
        void Process(NodeArtist aNode);
        void Process(NodeGenre aNode);
        void Process(NodePlaylist aNode);
    }



    public interface INodeBuilder
    {
        void Add(NodeContainer aParent, ItemDesc aDesc, ref UInt32 aNextId);
    }


    public class NodeItemBuilder : INodeBuilder
    {
        public interface IItemFactory
        {
            NodeItem Create(ItemDesc aDesc, ref UInt32 aNextId);
        }

        public NodeItemBuilder(IItemFactory aFactory)
        {
            iFactory = aFactory;
        }

        public void Add(NodeContainer aParent, ItemDesc aDesc, ref UInt32 aNextId)
        {
            NodeItem item = iFactory.Create(aDesc, ref aNextId);
            aParent.Add(item);
        }

        private IItemFactory iFactory;
    }


    public class NodeContainerBuilder : INodeBuilder
    {
        public interface IContainerFactory
        {
            string ContainerKey(LibraryItem aItem);
            string ContainerName(LibraryItem aItem);
            NodeContainer Create(string aName, LibraryItem aItem, UInt32 aId);
        }

        public NodeContainerBuilder(INodeBuilder aChild, IContainerFactory aFactory)
        {
            iChild = aChild;
            iFactory = aFactory;
        }

        public void Add(NodeContainer aParent, ItemDesc aDesc, ref UInt32 aNextId)
        {
            string containerKey = iFactory.ContainerKey(aDesc.LibItem);
            string containerName = iFactory.ContainerName(aDesc.LibItem);
            if (string.IsNullOrEmpty(containerKey))
            {
                containerKey = "Unknown";
                containerName = "Unknown";
            }

            NodeContainer container = aParent.Container(containerKey);
            if (container == null)
            {
                container = iFactory.Create(containerName, aDesc.LibItem, aNextId);
                aNextId++;
                aParent.Add(containerKey, container);
            }

            iChild.Add(container, aDesc, ref aNextId);
        }

        private INodeBuilder iChild;
        private IContainerFactory iFactory;
    }


    public class NodePlaylistBuilder : INodeBuilder
    {
        public NodePlaylistBuilder()
        {
            iItemDict = new Dictionary<long, ItemDesc>();
        }

        public void Add(NodeContainer aParent, ItemDesc aDesc, ref UInt32 aNextId)
        {
            iItemDict.Add(aDesc.LibItem.TrackId, aDesc);
        }

        public void Add(NodeContainer aParent, LibraryPlaylist aPlaylist, ref UInt32 aNextId)
        {
            // create the container for the playlist
            NodePlaylist playlist = new NodePlaylist(aPlaylist.Name, aNextId, null);
            aNextId++;

            // given that the ID (from the aNextId variable) is unique, it is easy to construct
            // a unique key for the playlist - having <playlistname><playlistid> ensures
            // uniqueness and alphabetical sorting
            aParent.Add(playlist.Name + playlist.Id.ToString(), playlist);

            // add all tracks
            foreach (long libItemId in aPlaylist.Items)
            {
                if (iItemDict.ContainsKey(libItemId))
                {
                    ItemDesc desc = iItemDict[libItemId];
                    NodeItem item = new NodeItem(aNextId, desc.Id, desc.LibItem);
                    aNextId++;
                    playlist.Add(item);
                }
            }
        }

        private Dictionary<long, ItemDesc> iItemDict;
    }


    public class ItemFactory : NodeItemBuilder.IItemFactory
    {
        public NodeItem Create(ItemDesc aDesc, ref UInt32 aNextId)
        {
            NodeItem item = new NodeItem(aNextId, aDesc.Id, aDesc.LibItem);
            aNextId++;
            return item;
        }
    }

    public class OriginalItemFactory : NodeItemBuilder.IItemFactory
    {
        public NodeItem Create(ItemDesc aDesc, ref UInt32 aNextId)
        {
            NodeItem item = new NodeItem(aDesc.Id, aDesc.Id, aDesc.LibItem);
            return item;
        }
    }


    
    public class ContainerArtistFactory : NodeContainerBuilder.IContainerFactory
    {
        public ContainerArtistFactory(IComparer<NodeItem> aItemComparer)
        {
            iItemComparer = aItemComparer;
        }

        public string ContainerKey(LibraryItem aItem)
        {
            return aItem.Artist;
        }

        public string ContainerName(LibraryItem aItem)
        {
            return aItem.Artist;
        }

        public NodeContainer Create(string aName, LibraryItem aItem, UInt32 aId)
        {
            return new NodeArtist(aName, aId, iItemComparer);
        }

        private IComparer<NodeItem> iItemComparer;
    }

    public class ContainerAlbumArtistFactory : NodeContainerBuilder.IContainerFactory
    {
        public ContainerAlbumArtistFactory(IComparer<NodeItem> aItemComparer)
        {
            iItemComparer = aItemComparer;
        }

        public string ContainerKey(LibraryItem aItem)
        {
            return string.IsNullOrEmpty(aItem.AlbumArtist) ? aItem.Artist : aItem.AlbumArtist;
        }

        public string ContainerName(LibraryItem aItem)
        {
            return string.IsNullOrEmpty(aItem.AlbumArtist) ? aItem.Artist : aItem.AlbumArtist;
        }

        public NodeContainer Create(string aName, LibraryItem aItem, UInt32 aId)
        {
            return new NodeArtist(aName, aId, iItemComparer);
        }

        private IComparer<NodeItem> iItemComparer;
    }

    public class ContainerAlbumFactory : NodeContainerBuilder.IContainerFactory
    {
        public ContainerAlbumFactory(IComparer<NodeItem> aItemComparer)
        {
            iItemComparer = aItemComparer;
        }

        public string ContainerKey(LibraryItem aItem)
        {
            if (string.IsNullOrEmpty(aItem.Album))
            {
                return null;
            }
            else
            {
                // make the key <album><album artist> so that albums with the same name by
                // different artists are not merged
                return aItem.Album + (string.IsNullOrEmpty(aItem.AlbumArtist) ? aItem.Artist : aItem.AlbumArtist);
            }
        }

        public string ContainerName(LibraryItem aItem)
        {
            return aItem.Album;
        }

        public NodeContainer Create(string aName, LibraryItem aItem, UInt32 aId)
        {
            return new NodeAlbum(aName, aId, iItemComparer);
        }

        private IComparer<NodeItem> iItemComparer;
    }

    public class ContainerGenreFactory : NodeContainerBuilder.IContainerFactory
    {
        public ContainerGenreFactory(IComparer<NodeItem> aItemComparer)
        {
            iItemComparer = aItemComparer;
        }

        public string ContainerKey(LibraryItem aItem)
        {
            return aItem.Genre;
        }

        public string ContainerName(LibraryItem aItem)
        {
            return aItem.Genre;
        }

        public NodeContainer Create(string aName, LibraryItem aItem, UInt32 aId)
        {
            return new NodeGenre(aName, aId, iItemComparer);
        }

        private IComparer<NodeItem> iItemComparer;
    }


} // OssKinskyMppItunes



