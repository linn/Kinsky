using Upnp;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Linn.Topology;

namespace Linn.Kinsky
{

    public class PlaylistDisplayItem<ImageType>
    {
        // internal constructor as this item should only be created by PlaylistDisplayHelper
        internal PlaylistDisplayItem(int aStartIndex,
                                     int aCount,
                                     upnpObject aItem,
                                     AbstractIconResolver<ImageType> aIconResolver,
                                     bool aIsGrouped,
                                     bool aIsHeaderItem,
                                     bool aCanMoveDown,
                                     bool aCanMoveUp)
        {
            iLock = new object();
            iStartIndex = aStartIndex;
            iCount = aCount;
            iItem = aItem;
            iIsGrouped = aIsGrouped;
            iIsHeaderItem = aIsHeaderItem;
            iCanMoveDown = aCanMoveDown;
            iCanMoveUp = aCanMoveUp;
            iTechnicalInfo = string.Empty;
            iDisplayField1 = string.Empty;
            iDisplayField2 = string.Empty;
            iDisplayField3 = string.Empty;
            iIcon = aIconResolver.GetIcon(iItem);
        }

        public int StartIndex { get { return iStartIndex; } }

        public int Count { get { return iCount; } internal set { iCount = value; } }

        public bool IsGrouped { get { return iIsGrouped; } }

        public bool CanMoveDown { get { return iCanMoveDown; } internal set { iCanMoveDown = value; } }

        public bool CanMoveUp { get { return iCanMoveUp; } internal set { iCanMoveUp = value; } }

        public uint PreviousId { get { return iPreviousId; } internal set { iPreviousId = value; } }

        public uint NextId { get { return iNextId; } internal set { iNextId = value; } }

        public string DisplayField1
        {
            get
            {
                EnsureLoaded();
                return iDisplayField1;
            }
        }

        public string DisplayField2
        {
            get
            {
                EnsureLoaded();
                return iDisplayField2;
            }
        }

        public string DisplayField3
        {
            get
            {
                EnsureLoaded();
                return iDisplayField3;
            }
        }

        public string TechnicalInfo
        {
            get
            {
                EnsureLoaded();
                return iTechnicalInfo;
            }
        }

        public Icon<ImageType> Icon
        {
            get
            {
                EnsureLoaded();
                return iIcon;
            }
        }

        private void EnsureLoaded()
        {
            lock (iLock)
            {
                if (!iIsLoaded)
                {
                    if (iIsHeaderItem)
                    {
                        iDisplayField1 = DidlLiteAdapter.Album(iItem);
                        iDisplayField2 = DidlLiteAdapter.AlbumArtist(iItem);
                        if (iDisplayField2 == string.Empty)
                        {
                            iDisplayField2 = DidlLiteAdapter.Artist(iItem);
                        }
                    }
                    else
                    {
                        ItemInfo info = new ItemInfo(iItem);
                        if (iIsGrouped)
                        {
                            ReadOnlyCollection<KeyValuePair<string, string>> displayItems = info.DisplayItems;
                            int counter = 0;
                            foreach (KeyValuePair<string, string> item in displayItems)
                            {
                                if (item.Key == "Album Artist" || item.Key == "Album" || (item.Key == "Artist" && DidlLiteAdapter.AlbumArtist(iItem) == item.Value))
                                {
                                    // don't show this info for a grouped item
                                    continue;
                                }
                                if (counter == 0)
                                {
                                    iDisplayField1 = item.Value;
                                }
                                else if (counter == 1)
                                {
                                    iDisplayField2 = item.Value;
                                }
                                else if (counter == 2)
                                {
                                    iDisplayField3 = item.Value;
                                }
                                else
                                {
                                    continue;
                                }
                                counter++;
                            }
                        }
                        else
                        {
                            iDisplayField1 = info.DisplayItem(0).HasValue ? info.DisplayItem(0).Value.Value : null;
                            iDisplayField2 = info.DisplayItem(1).HasValue ? info.DisplayItem(1).Value.Value : null;
                            iDisplayField3 = info.DisplayItem(2).HasValue ? info.DisplayItem(2).Value.Value : null;
                        }
                        iTechnicalInfo = DidlLiteAdapter.Duration(iItem);
                        if (iTechnicalInfo == string.Empty)
                        {
                            iTechnicalInfo = DidlLiteAdapter.Bitrate(iItem);
                        }
                    }
                    iItem = null;
                    iIsLoaded = true;
                }
            }
        }

        private string iDisplayField1;
        private string iDisplayField2;
        private string iDisplayField3;
        private string iTechnicalInfo;
        private Icon<ImageType> iIcon;

        private int iStartIndex;
        private int iCount;
        private bool iIsGrouped;
        private object iLock;
        private upnpObject iItem;
        private bool iIsHeaderItem;
        private bool iIsLoaded;
        private bool iCanMoveDown;
        private bool iCanMoveUp;
        private uint iPreviousId = 0;
        private uint iNextId = 0;
    }

    public class PlaylistDisplayHelper<ImageType>
    {

        public PlaylistDisplayHelper(IList<MrItem> aPlaylist, bool aGroupByAlbum, AbstractIconResolver<ImageType> aIconResolver)
        {
            iPlaylist = aPlaylist;
            iGroupByAlbum = aGroupByAlbum;
            iIconResolver = aIconResolver;
        }


        public ReadOnlyCollection<PlaylistDisplayItem<ImageType>> DisplayItems
        {
            get
            {
                List<PlaylistDisplayItem<ImageType>> result = new List<PlaylistDisplayItem<ImageType>>();
                string previousAlbum = null;
                string previousAlbumArtist = null;
                PlaylistDisplayItem<ImageType> currentGroupDescriptor = null;
                PlaylistDisplayItem<ImageType> previousGroupDescriptor = null;
                for (int i = 0; i < iPlaylist.Count; ++i)
                {
                    //MrItem current = aItems[i];
                    // extra logging for ticket #814
                    Assert.Check(iPlaylist[i].DidlLite.Count > 0);
                    Assert.Check(i == iPlaylist.Count - 1 || iPlaylist[i + 1].DidlLite.Count > 0);
                    if (iGroupByAlbum)
                    {
                        string nextAlbum = i == iPlaylist.Count - 1 ? null : DidlLiteAdapter.Album(iPlaylist[i + 1].DidlLite[0]);
                        string nextAlbumArtist = i == iPlaylist.Count - 1 ? null : DidlLiteAdapter.AlbumArtist(iPlaylist[i + 1].DidlLite[0]);
                        string currentAlbum = DidlLiteAdapter.Album(iPlaylist[i].DidlLite[0]);
                        string currentAlbumArtist = DidlLiteAdapter.AlbumArtist(iPlaylist[i].DidlLite[0]);

                        if (currentAlbum != string.Empty && ((currentAlbum == nextAlbum && currentAlbumArtist == nextAlbumArtist) || (currentAlbum == previousAlbum && currentAlbumArtist == previousAlbumArtist)))
                        {
                            if (currentAlbum != previousAlbum || currentAlbumArtist != previousAlbumArtist)
                            {
                                uint previousId = 0;
                                if (currentGroupDescriptor != null)
                                {
                                    currentGroupDescriptor.CanMoveDown = true;
                                    if (currentGroupDescriptor.StartIndex > 0)
                                    {
                                        previousId = iPlaylist[currentGroupDescriptor.StartIndex - 1].Id;
                                    }
                                    previousGroupDescriptor = currentGroupDescriptor;
                                    previousGroupDescriptor.NextId = iPlaylist[i].Id;
                                }
                                currentGroupDescriptor = new PlaylistDisplayItem<ImageType>(i, 1, iPlaylist[i].DidlLite[0], iIconResolver, true, true, false, currentGroupDescriptor != null);
                                currentGroupDescriptor.PreviousId = previousId;
                                result.Add(currentGroupDescriptor);
                            }
                            else
                            {
                                Assert.Check(currentGroupDescriptor != null);
                                currentGroupDescriptor.Count = currentGroupDescriptor.Count + 1;
                                if (previousGroupDescriptor != null)
                                {
                                    previousGroupDescriptor.NextId = iPlaylist[i].Id;
                                }
                            }
                        }
                        else
                        {
                            currentGroupDescriptor = null;
                        }

                        previousAlbum = currentAlbum;
                        previousAlbumArtist = currentAlbumArtist;
                    }
                    PlaylistDisplayItem<ImageType> item = new PlaylistDisplayItem<ImageType>(i,
                                                                                  1,
                                                                                  iPlaylist[i].DidlLite[0],
                                                                                  iIconResolver,
                                                                                  currentGroupDescriptor != null,
                                                                                  false,
                                                                                  i != iPlaylist.Count - 1,
                                                                                  i != 0);
                    if (i > 1)
                    {
                        item.PreviousId = iPlaylist[i - 2].Id;
                    }
                    if (i != iPlaylist.Count - 1)
                    {
                        item.NextId = iPlaylist[i + 1].Id;
                    }
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        private IList<MrItem> iPlaylist;
        private bool iGroupByAlbum;
        private AbstractIconResolver<ImageType> iIconResolver;
    }

}