using System;
using System.IO;

using UIKit;

namespace KinskyTouch.Properties
{
    internal class ResourceManager
    {
        internal ResourceManager() {
        }

        internal static UIImage GetObject(string aName) {
            string filename = aName + ".png";
            return UIImage.FromFile(filename);
        }

        private static UIImage iImageIcon;
        internal static UIImage Icon
        {
            get
            {
                if(iImageIcon == null)
                {
                    iImageIcon = GetObject("KinskyLogoAbout");
                }
                return iImageIcon;
            }
        }

        private static UIImage iImageButton;
        internal static UIImage Button
        {
            get
            {
                if(iImageButton == null)
                {
                    iImageButton = GetObject("Button");
                }
                return iImageButton;
            }
        }

        private static UIImage iImageAlbum;
        internal static UIImage Album
        {
            get
            {
                if(iImageAlbum == null)
                {
                    iImageAlbum = GetObject("Album");
                }
                return iImageAlbum;
            }
        }

        private static UIImage iImageAlbumError;
        internal static UIImage AlbumError
        {
            get
            {
                if(iImageAlbumError == null)
                {
                    iImageAlbumError = GetObject("AlbumArtError");
                }
                return iImageAlbumError;
            }
        }

        private static UIImage iImageArtist;
        internal static UIImage Artist
        {
            get
            {
                if(iImageArtist == null)
                {
                    iImageArtist = GetObject("Artist");
                }
                return iImageArtist;
            }
        }

        private static UIImage iImageDirectory;
        internal static UIImage Directory
        {
            get
            {
                if(iImageDirectory == null)
                {
                    iImageDirectory = GetObject("Directory");
                }
                return iImageDirectory;
            }
        }

        private static UIImage iImageLibrary;
        internal static UIImage Library
        {
            get
            {
                if(iImageLibrary == null)
                {
                    iImageLibrary = GetObject("Library");
                }
                return iImageLibrary;
            }
        }

        private static UIImage iImageList;
        internal static UIImage List
        {
            get
            {
                if(iImageList == null)
                {
                    iImageList = GetObject("List");
                }
                return iImageList;
            }
        }

        private static UIImage iImagePlaylist;
        internal static UIImage Playlist
        {
            get
            {
                if(iImagePlaylist == null)
                {
                    iImagePlaylist = GetObject("Playlist");
                }
                return iImagePlaylist;
            }
        }

        private static UIImage iImagePlaylistItem;
        internal static UIImage PlaylistItem
        {
            get
            {
                if(iImagePlaylistItem == null)
                {
                    iImagePlaylistItem = GetObject("PlaylistItem");
                }
                return iImagePlaylistItem;
            }
        }

        private static UIImage iImageTrack;
        internal static UIImage Track
        {
            get
            {
                if(iImageTrack == null)
                {
                    iImageTrack = GetObject("Track");
                }
                return iImageTrack;
            }
        }

        internal static UIImage Radio
        {
            get
            {
                return SourceRadio;
            }
        }

        private static UIImage iImageVideo;
        internal static UIImage Video
        {
            get
            {
                if(iImageVideo == null)
                {
                    iImageVideo = GetObject("Video");
                }
                return iImageVideo;
            }
        }

        private static UIImage iImageDisclosure;
        internal static UIImage Disclosure
        {
            get
            {
                if(iImageDisclosure == null)
                {
                    iImageDisclosure = GetObject("DisclosureIndicator");
                }
                return iImageDisclosure;
            }
        }

        private static UIImage iImageLoading;
        internal static UIImage Loading
        {
            get
            {
                if(iImageLoading == null)
                {
                    iImageLoading = GetObject("Loading");
                }
                return iImageLoading;
            }
        }

        private static UIImage iImageRoom;
        internal static UIImage Room
        {
            get
            {
                if(iImageRoom == null)
                {
                    iImageRoom = GetObject("Room");
                }
                return iImageRoom;
            }
        }

        private static UIImage iImageSourceExternal;
        internal static UIImage SourceExternal
        {
            get
            {
                if(iImageSourceExternal == null)
                {
                    iImageSourceExternal = GetObject("Source");
                }
                return iImageSourceExternal;
            }
        }

        private static UIImage iImageSourceDisc;
        internal static UIImage SourceDisc
        {
            get
            {
                if(iImageSourceDisc == null)
                {
                    iImageSourceDisc = GetObject("CD");
                }
                return iImageSourceDisc;
            }
        }

        private static UIImage iImageSourcePlaylist;
        internal static UIImage SourcePlaylist
        {
            get
            {
                if(iImageSourcePlaylist == null)
                {
                    iImageSourcePlaylist = GetObject("PlaylistSource");
                }
                return iImageSourcePlaylist;
            }
        }

        private static UIImage iImageSourceRadio;
        internal static UIImage SourceRadio
        {
            get
            {
                if(iImageSourceRadio == null)
                {
                    iImageSourceRadio = GetObject("Radio");
                }
                return iImageSourceRadio;
            }
        }

        private static UIImage iImageSourceSongcast;
        internal static UIImage SourceSongcast
        {
            get
            {
                if(iImageSourceSongcast == null)
                {
                    iImageSourceSongcast = GetObject("Sender");
                }
                return iImageSourceSongcast;
            }
        }

        private static UIImage iImageSourceSongcastNotSending;
        internal static UIImage SourceSongcastNotSending
        {
            get
            {
                if(iImageSourceSongcastNotSending == null)
                {
                    iImageSourceSongcastNotSending = GetObject("SenderNoReceive");
                }
                return iImageSourceSongcastNotSending;
            }
        }

        private static UIImage iImageSourceUpnpAv;
        internal static UIImage SourceUpnpAv
        {
            get
            {
                if(iImageSourceUpnpAv == null)
                {
                    iImageSourceUpnpAv = GetObject("UPNP");
                }
                return iImageSourceUpnpAv;
            }
        }

        private static UIImage iImageWheel;
        internal static UIImage Wheel
        {
            get
            {
                if(iImageWheel == null)
                {
                    iImageWheel = GetObject("Wheel");
                }
                return iImageWheel;
            }
        }

        private static UIImage iImageWheelLarge;
        internal static UIImage WheelLarge
        {
            get
            {
                if(iImageWheelLarge == null)
                {
                    iImageWheelLarge = GetObject("WheelLarge");
                }
                return iImageWheelLarge;
            }
        }

        private static UIImage iImageWheelLargeOver;
        internal static UIImage WheelLargeOver
        {
            get
            {
                if(iImageWheelLargeOver == null)
                {
                    iImageWheelLargeOver = GetObject("WheelLargeOver");
                }
                return iImageWheelLargeOver;
            }
        }

        private static UIImage iImageWheelGripLarge;
        internal static UIImage WheelGripLarge
        {
            get
            {
                if(iImageWheelGripLarge == null)
                {
                    iImageWheelGripLarge = GetObject("ScrewsLarge");
                }
                return iImageWheelGripLarge;
            }
        }

        private static UIImage iImageWheelMute;
        internal static UIImage WheelMute
        {
            get
            {
                if(iImageWheelMute == null)
                {
                    iImageWheelMute = GetObject("WheelMute");
                }
                return iImageWheelMute;
            }
        }

        private static UIImage iImageMute;
        internal static UIImage Mute
        {
            get
            {
                if(iImageMute == null)
                {
                    iImageMute = GetObject("Mute");
                }
                return iImageMute;
            }
        }

        private static UIImage iImageMuteActive;
        internal static UIImage MuteActive
        {
            get
            {
                if(iImageMuteActive == null)
                {
                    iImageMuteActive = GetObject("MuteActive");
                }
                return iImageMuteActive;
            }
        }

        private static UIImage iImageClockElapsed;
        internal static UIImage ClockElapsed
        {
            get
            {
                if(iImageClockElapsed == null)
                {
                    iImageClockElapsed = GetObject("ClockIconElapsed");
                }
                return iImageClockElapsed;
            }
        }

        private static UIImage iImageClockRemaining;
        internal static UIImage ClockRemaining
        {
            get
            {
                if(iImageClockRemaining == null)
                {
                    iImageClockRemaining = GetObject("ClockIconRemaining");
                }
                return iImageClockRemaining;
            }
        }

        private static UIImage iImageStandby;
        internal static UIImage Standby
        {
            get
            {
                if(iImageStandby == null)
                {
                    iImageStandby = GetObject("Standby");
                }
                return iImageStandby;
            }
        }

        private static UIImage iImageTableViewBackground;
        internal static UIImage TableViewBackground
		{
            get
            {
                if(iImageTableViewBackground == null)
                {
                    iImageTableViewBackground = GetObject("UITableViewBackground");
                }
                return iImageTableViewBackground;
             }
        }
    }
}

