using System;
using Upnp;
namespace Linn.Kinsky
{
    public class Icon<ImageType>
    {
        public Icon(ImageType aImage)
        {
            iIsUri = false;
            iImage = aImage;
        }

        public Icon(Uri aUri)
        {
            iIsUri = true;
            iImageUri = aUri;
        }

        public bool IsUri { get { return iIsUri; } }
        public ImageType Image { get { return iImage; } }
        public Uri ImageUri { get { return iImageUri; } }
        private bool iIsUri;
        private ImageType iImage;
        private Uri iImageUri;
    }

    public abstract class AbstractIconResolver<ImageType>
    {
        public Icon<ImageType> GetIcon(Linn.Topology.MrItem aMrItem)
        {
            if (aMrItem != null && aMrItem.DidlLite.Count > 0)
            {
                return GetIcon(aMrItem.DidlLite[0]);
            }
            return IconNoArtwork;
        }

        public Icon<ImageType> GetIcon(upnpObject aUpnpObject)
        {
            if (aUpnpObject != null)
            {
                Uri uri = DidlLiteAdapter.ArtworkUri(aUpnpObject);
                if (uri != null)
                {
                    return new Icon<ImageType>(uri);
                }
                if (aUpnpObject is container)
                {
                    if (aUpnpObject is musicAlbum)
                    {
                        return IconAlbum;
                    }
                    else if (aUpnpObject is person)
                    {
                        return IconArtist;
                    }
                    else if (aUpnpObject is playlistContainer)
                    {
                        return IconPlaylistContainer;
                    }
                    else if (aUpnpObject.ParentId == MediaProviderLibrary.kLibraryId || aUpnpObject.Id == MediaProviderLibrary.kLibraryId)
                    {
                        return IconLibrary;
                    }
                    else return IconDirectory;
                }
                if (aUpnpObject is item)
                {
                    if (aUpnpObject is audioBroadcast)
                    {
                        return IconRadio;
                    }
                    else if (aUpnpObject is videoItem)
                    {
                        return IconVideo;
                    }
                    else if (aUpnpObject is playlistItem)
                    {
                        return IconPlaylistItem;
                    }
                    else if (aUpnpObject.Title == "Access denied")
                    {
                        return IconError;
                    }
                    else return IconTrack;
                }
            }
            return IconNoArtwork;
        }

        public Icon<ImageType> GetIcon(IRoom aRoom)
        {
            return IconRoom;
        }

        public Icon<ImageType> GetIcon(Linn.Topology.ModelSender aSender)
        {
            if (aSender.Metadata != null && aSender.Metadata.Count > 0)
            {
                Icon<ImageType> image = GetIcon(aSender.Metadata[0]);
                if (image.IsUri)
                {
                    return image;
                }
            }
            return aSender.Status == Linn.Topology.ModelSender.EStatus.eBlocked ? IconSenderSourceNoSend : IconSenderSource;
        }

        public Icon<ImageType> GetIcon(ISource aSource)
        {
            Assert.Check(aSource != null);
            switch (aSource.Type)
            {
                case Linn.Kinsky.Source.kSourceAux:
                    return IconSource;
                case Linn.Kinsky.Source.kSourceAnalog:
                    return IconSource;
                case Linn.Kinsky.Source.kSourceDisc:
                    return IconDiscSource;
                case Linn.Kinsky.Source.kSourceDs:
                    return IconPlaylistSource;
                case Linn.Kinsky.Source.kSourceRadio:
                    return IconRadioSource;
                case Linn.Kinsky.Source.kSourceSpdif:
                    return IconSource;
                case Linn.Kinsky.Source.kSourceToslink:
                    return IconSource;
                case Linn.Kinsky.Source.kSourceTuner:
                    return IconRadioSource;
                case Linn.Kinsky.Source.kSourceUpnpAv:
                    return IconUpnpAvSource;
                case Linn.Kinsky.Source.kSourceReceiver:
                    return IconSenderSource;
                default:
                    return IconSource;
            }
        }

        public Icon<ImageType> GetIcon(Bookmark aBookmark)
        {
            if (aBookmark != null)
            {
                if (aBookmark.BreadcrumbTrail[aBookmark.BreadcrumbTrail.Count - 1].Id == kLibraryId)
                {
                    return IconLibrary;
                }
                else if (!string.IsNullOrEmpty(aBookmark.Image))
                {
                    return new Icon<ImageType>(new Uri(aBookmark.Image));
                }
                else if (string.Compare(aBookmark.Class, kContainerClass, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return IconDirectory;
                }
                else if (string.Compare(aBookmark.Class, kMusicAlbumClass, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return IconAlbum;
                }
                else if (string.Compare(aBookmark.Class, kMusicArtistClass, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return IconArtist;
                }
                else if (string.Compare(aBookmark.Class, kPersonClass, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return IconArtist;
                }
                else if (string.Compare(aBookmark.Class, kPlaylistContainerClass, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return IconPlaylistContainer;
                }
            }
            return IconBookmark;
        }

        public abstract Icon<ImageType> IconSource { get; }
        public abstract Icon<ImageType> IconDiscSource { get; }
        public abstract Icon<ImageType> IconPlaylistSource { get; }
        public abstract Icon<ImageType> IconRadioSource { get; }
        public abstract Icon<ImageType> IconUpnpAvSource { get; }
        public abstract Icon<ImageType> IconSenderSource { get; }
        public abstract Icon<ImageType> IconSenderSourceNoSend { get; }
        public abstract Icon<ImageType> IconAlbum { get; }
        public abstract Icon<ImageType> IconArtist { get; }
        public abstract Icon<ImageType> IconPlaylistContainer { get; }
        public abstract Icon<ImageType> IconLibrary { get; }
        public abstract Icon<ImageType> IconDirectory { get; }
        public abstract Icon<ImageType> IconRadio { get; }
        public abstract Icon<ImageType> IconVideo { get; }
        public abstract Icon<ImageType> IconPlaylistItem { get; }
        public abstract Icon<ImageType> IconError { get; }
        public abstract Icon<ImageType> IconTrack { get; }
        public abstract Icon<ImageType> IconRoom { get; }
        public abstract Icon<ImageType> IconNoArtwork { get; }
        public abstract Icon<ImageType> IconBookmark { get; }
        public abstract Icon<ImageType> IconLoading { get; }

        private const string kLibraryId = "Library";
        private const string kContainerClass = "object.container";
        private const string kMusicAlbumClass = "object.container.album.musicAlbum";
        private const string kPersonClass = "object.container.person";
        private const string kMusicArtistClass = "object.container.person.musicArtist";
        private const string kPlaylistContainerClass = "object.container.playlistContainer";
    }
}