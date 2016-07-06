
using System;
using Monobjc.Cocoa;


namespace KinskyDesktop.Properties
{
    internal class Resources
    {
        internal Resources()
        {
        }

        private static string iBasePath;
        internal static void SetBasePath(string aBasePath)
        {
            iBasePath = aBasePath;
        }

        // This class loads the image on first access and retains a reference to it
        // The use of an implicit cast means they can be used in place of an NSImage
        public class ResourceImage
        {
            public ResourceImage(string aName)
            {
                iName = aName;
            }

            public static implicit operator NSImage(ResourceImage aResImage)
            {
                // load the image on first access
                if (aResImage.iImage == null)
                {
                    string fullpath = System.IO.Path.Combine(iBasePath, aResImage.iName + ".png");
                    try
                    {
                        aResImage.iImage = new NSImage(fullpath);
                    }
                    catch (Exception)
                    {
                        Linn.UserLog.WriteLine(DateTime.Now + ": Logging for #788: " + fullpath + " " + System.IO.File.Exists(fullpath));
                        throw;
                    }
                }
                return aResImage.iImage;
            }

            private readonly string iName;
            private NSImage iImage;
        }


        public static ResourceImage ImageAbout = new ResourceImage("Images/About");
        public static ResourceImage ImageArray = new ResourceImage("Images/Array");
        public static ResourceImage ImageBottomFiller = new ResourceImage("Images/BottomFiller");
        public static ResourceImage ImageBottomLeftEdge = new ResourceImage("Images/BottomLeftEdge");
        public static ResourceImage ImageBottomRightEdge = new ResourceImage("Images/BottomRightEdge");
        public static ResourceImage ImageBoxDownFiller = new ResourceImage("Images/BoxDownFiller");
        public static ResourceImage ImageBoxDownLeft = new ResourceImage("Images/BoxDownLeft");
        public static ResourceImage ImageBoxDownRight = new ResourceImage("Images/BoxDownRight");
        public static ResourceImage ImageBoxFiller = new ResourceImage("Images/BoxFiller");
        public static ResourceImage ImageBoxLeft = new ResourceImage ("Images/BoxLeft");
        public static ResourceImage ImageBoxNotificationLeft = new ResourceImage ("Images/BoxNotificationLeft");
        public static ResourceImage ImageBoxNotificationRight = new ResourceImage ("Images/BoxNotificationRight");
        public static ResourceImage ImageBoxNotificationFiller = new ResourceImage ("Images/BoxNotificationFiller");
        public static ResourceImage ImageBoxNotificationDownLeft = new ResourceImage ("Images/BoxNotificationDownLeft");
        public static ResourceImage ImageBoxNotificationDownRight = new ResourceImage ("Images/BoxNotificationDownRight");
        public static ResourceImage ImageBoxNotificationDownFiller = new ResourceImage ("Images/BoxNotificationDownFiller");
        public static ResourceImage ImageBoxNotificationOverLeft = new ResourceImage ("Images/BoxNotificationOverLeft");
        public static ResourceImage ImageBoxNotificationOverRight = new ResourceImage ("Images/BoxNotificationOverRight");
        public static ResourceImage ImageBoxNotificationOverFiller = new ResourceImage ("Images/BoxNotificationOverFiller");
        public static ResourceImage ImageBoxOverFiller = new ResourceImage("Images/BoxOverFiller");
        public static ResourceImage ImageBoxOverLeft = new ResourceImage("Images/BoxOverLeft");
        public static ResourceImage ImageBoxOverRight = new ResourceImage("Images/BoxOverRight");
        public static ResourceImage ImageBoxRight = new ResourceImage("Images/BoxRight");
        public static ResourceImage ImageKmodeFiller = new ResourceImage("Images/KmodeFiller");
        public static ResourceImage ImageKmodeLeft = new ResourceImage("Images/KmodeLeft");
        public static ResourceImage ImageKmodeRight = new ResourceImage("Images/KmodeRight");
        public static ResourceImage ImageLeftFiller = new ResourceImage("Images/LeftFiller");
        public static ResourceImage ImageLinnLogo = new ResourceImage("Images/LinnLogo");
        public static ResourceImage ImagePause = new ResourceImage("Images/Pause");
        public static ResourceImage ImagePauseDown = new ResourceImage("Images/PauseDown");
        public static ResourceImage ImagePauseOver = new ResourceImage("Images/PauseOver");
        public static ResourceImage ImagePlay = new ResourceImage("Images/Play");
        public static ResourceImage ImagePlayDown = new ResourceImage("Images/PlayDown");
        public static ResourceImage ImagePlayOver = new ResourceImage("Images/PlayOver");
        public static ResourceImage ImageRightFiller = new ResourceImage("Images/RightFiller");
        public static ResourceImage ImageRocker = new ResourceImage("Images/Rocker");
        public static ResourceImage ImageRockerLeftDown = new ResourceImage("Images/RockerLeftDown");
        public static ResourceImage ImageRockerLeftOver = new ResourceImage("Images/RockerLeftOver");
        public static ResourceImage ImageRockerRightDown = new ResourceImage("Images/RockerRightDown");
        public static ResourceImage ImageRockerRightOver = new ResourceImage("Images/RockerRightOver");
        public static ResourceImage ImageScrews = new ResourceImage("Images/Screws");
        public static ResourceImage ImageSkipBack = new ResourceImage("Images/SkipBack");
        public static ResourceImage ImageSkipBackDown = new ResourceImage("Images/SkipBackDown");
        public static ResourceImage ImageSkipBackOver = new ResourceImage("Images/SkipBackOver");
        public static ResourceImage ImageSkipForward = new ResourceImage("Images/SkipForward");
        public static ResourceImage ImageSkipForwardDown = new ResourceImage("Images/SkipForwardDown");
        public static ResourceImage ImageSkipForwardOver = new ResourceImage("Images/SkipForwardOver");
        public static ResourceImage ImageStop = new ResourceImage("Images/Stop");
        public static ResourceImage ImageStopDown = new ResourceImage("Images/StopDown");
        public static ResourceImage ImageStopOver = new ResourceImage("Images/StopOver");
        public static ResourceImage ImageTopBarFiller = new ResourceImage("Images/TopBarFiller");
        public static ResourceImage ImageTopFiller = new ResourceImage("Images/TopFiller");
        public static ResourceImage ImageTopLeftEdge = new ResourceImage("Images/TopLeftEdge");
        public static ResourceImage ImageTopRightEdge = new ResourceImage("Images/TopRightEdge");
        public static ResourceImage ImageTramLines = new ResourceImage("Images/TramLines");
        public static ResourceImage ImageWheel = new ResourceImage("Images/Wheel");
        public static ResourceImage ImageWheelMute = new ResourceImage("Images/WheelMute");
        public static ResourceImage ImageWheelOver = new ResourceImage("Images/WheelOver");

        public static ResourceImage IconAlbum = new ResourceImage("Images/Album");
        public static ResourceImage IconAlbumArtError = new ResourceImage("Images/AlbumArtError");
        public static ResourceImage IconAlbumArtNone = new ResourceImage("Images/AlbumArtNone");
        public static ResourceImage IconArtist = new ResourceImage("Images/Artist");
        public static ResourceImage IconBack = new ResourceImage("Images/Back");
        public static ResourceImage IconBackDown = new ResourceImage("Images/BackDown");
        public static ResourceImage IconCd = new ResourceImage("Images/CD");
        public static ResourceImage IconDelete = new ResourceImage("Images/Delete");
        public static ResourceImage IconError = new ResourceImage("Images/Error");
        public static ResourceImage IconFolder = new ResourceImage("Images/Folder");
        public static ResourceImage IconHourglass = new ResourceImage("Images/HourGlass");
        public static ResourceImage IconHourglass2 = new ResourceImage("Images/HourGlass2");
        public static ResourceImage IconBookmark = new ResourceImage("Images/IconBookmark");
        public static ResourceImage IconLibrary = new ResourceImage("Images/Library");
        public static ResourceImage IconList = new ResourceImage("Images/List");
        public static ResourceImage IconLoading = new ResourceImage("Images/Loading");
        public static ResourceImage IconOsXClose = new ResourceImage("Images/OsXClose");
        public static ResourceImage IconOsXCloseMouse = new ResourceImage("Images/OsXCloseMouse");
        public static ResourceImage IconOsXCloseTouch = new ResourceImage("Images/OsXCloseTouch");
        public static ResourceImage IconOsXMaximise = new ResourceImage("Images/OsXMaximize");
        public static ResourceImage IconOsXMaximiseMouse = new ResourceImage("Images/OsXMaximizeMouse");
        public static ResourceImage IconOsXMaximiseTouch = new ResourceImage("Images/OsXMaximizeTouch");
        public static ResourceImage IconOsXMini = new ResourceImage("Images/OsXMini");
        public static ResourceImage IconOsXMinimise = new ResourceImage("Images/OsXMinimize");
        public static ResourceImage IconOsXMinimiseMouse = new ResourceImage("Images/OsXMinimizeMouse");
        public static ResourceImage IconOsXMinimiseTouch = new ResourceImage("Images/OsXMinimizeTouch");
        public static ResourceImage IconOsXMiniMouse = new ResourceImage("Images/OsXMiniMouse");
        public static ResourceImage IconOsXMiniTouch = new ResourceImage("Images/OsXMiniTouch");
        public static ResourceImage IconPlaying = new ResourceImage("Images/Playing");
        public static ResourceImage IconPlaylist = new ResourceImage("Images/Playlist");
        public static ResourceImage IconPlaylistItem = new ResourceImage("Images/PlaylistItem");
        public static ResourceImage IconPlaylistSource = new ResourceImage("Images/PlaylistSource");
        public static ResourceImage IconRadio = new ResourceImage("Images/Radio");
        public static ResourceImage IconReceiver = new ResourceImage("Images/Receiver");
        public static ResourceImage IconRepeat = new ResourceImage("Images/Repeat");
        public static ResourceImage IconRepeatOn = new ResourceImage("Images/RepeatOn");
        public static ResourceImage IconRoom = new ResourceImage("Images/Room");
        public static ResourceImage IconSave = new ResourceImage("Images/Save");
        public static ResourceImage IconSender = new ResourceImage("Images/Sender");
        public static ResourceImage IconShuffle = new ResourceImage("Images/Shuffle");
        public static ResourceImage IconShuffleOn = new ResourceImage("Images/ShuffleOn");
        public static ResourceImage IconSize = new ResourceImage("Images/Size");
        public static ResourceImage IconSource = new ResourceImage("Images/Source");
        public static ResourceImage IconStandby = new ResourceImage("Images/Standby");
        public static ResourceImage IconStandbyOn = new ResourceImage("Images/StandbyOn");
        public static ResourceImage IconThumbs = new ResourceImage("Images/Thumbs");
        public static ResourceImage IconTrack = new ResourceImage("Images/Track");
        public static ResourceImage IconUpnp = new ResourceImage("Images/UPNP");
        public static ResourceImage IconVideo = new ResourceImage("Images/Video");

        public static ResourceImage IconSettings = new ResourceImage ("Images/SettingsWhite");

        public static ResourceImage IconTick = new ResourceImage("Images/Tick");
        public static ResourceImage IconRefreshButton = new ResourceImage("Images/RefreshButton");

        public static ResourceImage Badge = new ResourceImage ("Images/Badge");

        public static ResourceImage IconSourceRadio = new ResourceImage("Images/Radio");
    }
}





