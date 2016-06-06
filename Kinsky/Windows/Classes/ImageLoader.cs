using Linn.Kinsky;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Upnp;
using Linn.Topology;
using System.Collections.Generic;
using System.Threading;
using Linn;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace KinskyDesktopWpf
{

    public class WpfImageLoader : AbstractStreamImageLoader<BitmapImage>
    {
        public WpfImageLoader(IImageUriConverter aConverter) : base(aConverter) { }

        protected override IImage<BitmapImage> CreateImageFromStream(MemoryStream aStream, bool aDownscaleImage, int aDownscaleImageSize)
        {
            BitmapImage bi = new BitmapImage();

            bi.BeginInit();
            if (aDownscaleImage)
            {
                bi.DecodePixelHeight = (int)aDownscaleImageSize;
            }
            bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            bi.CacheOption = BitmapCacheOption.Default;
            bi.StreamSource = aStream;
            bi.EndInit();

            byte[] encodedData = GetEncodedImageData(bi);
            MemoryStream streamSource = new MemoryStream();
            streamSource.Write(encodedData, 0, encodedData.Length);
            streamSource.Seek(0, SeekOrigin.Begin);
            bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            bi.CacheOption = BitmapCacheOption.Default;
            bi.StreamSource = streamSource;
            bi.EndInit();
            bi.Freeze();
            return new WpfImageWrapper(bi);
        }

        private byte[] GetEncodedImageData(BitmapImage image)
        {
            byte[] result = null;

            BitmapEncoder encoder = new PngBitmapEncoder();
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);

                br.Read(result, 0, (int)stream.Length);

                br.Close();

                stream.Close();
            }
            return result;
        }

    }


    public class WpfImageCache
    {

        public WpfImageCache(int aCacheSize, int aDownscaleImageSize, int aThreadCount)
        {
            iRegisteredCallbacks = new Dictionary<string, List<Action<BitmapImage>>>();
            iErrorImage = new WpfImageWrapper(StaticImages.ImageSourceIconLoading);
            iImageCache = new ThreadedImageCache<BitmapImage>(aCacheSize, aDownscaleImageSize, aThreadCount, new WpfImageLoader(new ScalingUriConverter(aDownscaleImageSize, false, false)));
            iImageCache.EventImageAdded += OnImageCacheUpdated;
            iImageCache.EventRequestFailed += OnImageRequestFailed;
        }

        public int DownscaleImageSize
        {
            set
            {
                iImageCache.ImageLoader = new WpfImageLoader(new ScalingUriConverter(value, false, false));
                iImageCache.DownscaleImageSize = value;
            }
        }

        private void OnImageRequestFailed(object sender, EventArgsImageFailure args)
        {
            OnImageCacheUpdated(sender, new EventArgsImage<BitmapImage>(args.Uri, iErrorImage));
        }

        private void OnImageCacheUpdated(object sender, EventArgsImage<BitmapImage> args)
        {
            lock (iRegisteredCallbacks)
            {
                Dictionary<string, List<Action<BitmapImage>>> callbacksDictionary = iRegisteredCallbacks;
                string key = args.Uri;
                if (callbacksDictionary.ContainsKey(key))
                {
                    List<Action<BitmapImage>> callbackList = callbacksDictionary[key];
                    callbacksDictionary.Remove(key);
                    foreach (Action<BitmapImage> callback in callbackList)
                    {
                        callback.Invoke(args.Image.Image);
                    }
                }
            }
        }

        public void Load(Icon<BitmapImage> aSource, Action<BitmapImage> aCallback)
        {
            if (aSource.IsUri)
            {
                DownloadImage(aSource.ImageUri, aCallback);
            }
            else
            {
                if (aCallback != null)
                {
                    aCallback(aSource.Image);
                }
            }
        }

        public void Clear()
        {
            iImageCache.Clear();
        }

        private void DownloadImage(Uri aUri, Action<BitmapImage> aCallback)
        {
            IImage<BitmapImage> artwork = null;
            string key = aUri.OriginalString;
            lock (iRegisteredCallbacks)
            {
                if (aCallback != null)
                {
                    Dictionary<string, List<Action<BitmapImage>>> callbacksDictionary = iRegisteredCallbacks;
                    List<Action<BitmapImage>> callbacksList;
                    if (callbacksDictionary.ContainsKey(key))
                    {
                        callbacksList = callbacksDictionary[key];
                    }
                    else
                    {
                        callbacksList = new List<Action<BitmapImage>>();
                        callbacksDictionary[key] = callbacksList;
                    }
                    callbacksList.Add(aCallback);
                }
            }
            artwork = iImageCache.Image(key);
            if (artwork != null)
            {
                OnImageCacheUpdated(this, new EventArgsImage<BitmapImage>(key, artwork));
            }
        }
        private ThreadedImageCache<BitmapImage> iImageCache;
        private Dictionary<string, List<Action<BitmapImage>>> iRegisteredCallbacks;
        private WpfImageWrapper iErrorImage;
    }

    public class WpfImageWrapper : IImage<BitmapImage>
    {
        public WpfImageWrapper(BitmapImage aImage)
        {
            Assert.Check(aImage != null);
            iImage = aImage;
        }

        #region IImage<BitmapImage> Members

        public BitmapImage Image
        {
            get { return iImage; }
        }

        public int SizeBytes
        {
            get
            {
                return iImage.SizeBytes();
            }
        }

        public int ReferenceCount
        {
            get
            {
                lock (iImage)
                {
                    return iReferenceCount;
                }
            }
        }

        public void IncrementReferenceCount()
        {
            lock (iImage)
            {
                iReferenceCount++;
            }
        }

        public void DecrementReferenceCount()
        {
            lock (iImage)
            {
                iReferenceCount--;
            }
        }


        #endregion
        private int iReferenceCount;
        private BitmapImage iImage;

    }

    public class IconResolver : AbstractIconResolver<BitmapImage>
    {
        public Icon<BitmapImage> Resolve(object aObject)
        {
            if (aObject is Linn.Topology.MrItem)
            {
                return GetIcon(aObject as Linn.Topology.MrItem);
            }
            else if (aObject is upnpObject)
            {
                return GetIcon(aObject as upnpObject);
            }
            else if (aObject is Linn.Kinsky.IRoom)
            {
                return GetIcon(aObject as Linn.Kinsky.IRoom);
            }
            else if (aObject is Linn.Kinsky.ISource)
            {
                return GetIcon(aObject as Linn.Kinsky.ISource);
            }
            else if (aObject is Linn.Topology.ModelSender)
            {
                return GetIcon(aObject as Linn.Topology.ModelSender);
            }
            else if (aObject is Bookmark)
            {
                return GetIcon(aObject as Bookmark);
            }
            Assert.Check(false);
            return null;
        }

        public override Icon<BitmapImage> IconSource
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconAuxSource); }
        }

        public override Icon<BitmapImage> IconDiscSource
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconDiscSource); }
        }

        public override Icon<BitmapImage> IconPlaylistSource
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconPlaylistSource); }
        }

        public override Icon<BitmapImage> IconRadioSource
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconRadioSource); }
        }

        public override Icon<BitmapImage> IconUpnpAvSource
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconUpnpAvSource); }
        }

        public override Icon<BitmapImage> IconSenderSource
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconSenderSource); }
        }

        public override Icon<BitmapImage> IconSenderSourceNoSend
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconSenderSourceNoSend); }
        }

        public override Icon<BitmapImage> IconAlbum
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconAlbum); }
        }

        public override Icon<BitmapImage> IconArtist
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconArtist); }
        }

        public override Icon<BitmapImage> IconPlaylistContainer
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconPlaylistContainer); }
        }

        public override Icon<BitmapImage> IconLibrary
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconLibrary); }
        }

        public override Icon<BitmapImage> IconDirectory
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconDirectory); }
        }

        public override Icon<BitmapImage> IconRadio
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconRadio); }
        }

        public override Icon<BitmapImage> IconVideo
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconVideo); }
        }

        public override Icon<BitmapImage> IconPlaylistItem
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconPlaylistItem); }
        }

        public override Icon<BitmapImage> IconError
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconError); }
        }

        public override Icon<BitmapImage> IconTrack
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconTrack); }
        }

        public override Icon<BitmapImage> IconRoom
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconRoom); }
        }

        public override Icon<BitmapImage> IconNoArtwork
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconLoading); }
        }

        public override Icon<BitmapImage> IconBookmark
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconBookmark); }
        }

        public override Icon<BitmapImage> IconLoading
        {
            get { return new Icon<BitmapImage>(StaticImages.ImageSourceIconLoading); }
        }
    }

    public class StaticImages : KinskyDesktopWpf.Properties.Resources
    {
        public StaticImages() { }

        public static readonly BitmapImage ImageSourceKinskyIcon = KinskyIcon.ToBitmapImage();

        public static readonly BitmapImage ImageSourceNoAlbumArt = NoAlbumArt.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconAlbum = IconAlbum.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconAlbumError = IconAlbumError.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconArtist = IconArtist.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconDirectory = IconDirectory.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconError = IconError.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconPlaylistContainer = IconPlaylistContainer.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconPlaylistItem = IconPlaylistItem.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconRadio = IconRadioSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconServer = IconLibrary.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconTrack = IconTrack.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconVideo = IconVideo.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconAuxSource = IconAuxSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconDiscSource = IconDiscSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconPlaylistSource = IconPlaylistSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconRadioSource = IconRadioSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconSpdifSource = IconSpdifSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconTosLinkSource = IconTosLinkSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconUpnpAvSource = IconUpnpAvSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconSenderSource = IconSenderSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconSenderSourceNoSend = IconSenderSourceNoSend.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconReceiverSource = IconReceiverSource.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconReceiverSourceNoReceive = IconReceiverSourceNoReceive.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconRoom = IconRoom.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconLibrary = IconLibrary.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconLoading = IconLoading.ToBitmapImage();
        public static readonly BitmapImage ImageSourceIconBookmark = IconBookmark.ToBitmapImage();

        public static readonly BitmapImage ImageSourceBottomFiller = BottomFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBottomLeftEdge = BottomLeftEdge.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBottomRightEdge = BottomRightEdge.ToBitmapImage();
        public static readonly BitmapImage ImageSourceLeftFiller = LeftFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRightFiller = RightFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceTopFiller = TopFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceTopLeftEdge = TopLeftEdge.ToBitmapImage();
        public static readonly BitmapImage ImageSourceTopRightEdge = TopRightEdge.ToBitmapImage();
        public static readonly BitmapImage ImageSourceTopBarFiller = TopBarFiller.ToBitmapImage();


        public static readonly BitmapImage ImageSourceKModeLeft = KModeLeft.ToBitmapImage();
        public static readonly BitmapImage ImageSourceKModeRight = KModeRight.ToBitmapImage();
        public static readonly BitmapImage ImageSourceKModeFiller = KModeFiller.ToBitmapImage();

        public static readonly BitmapImage ImageSourceAboutBox = AboutBox.ToBitmapImage();

        public static readonly BitmapImage ImageSourceBusyIcon = BusyIcon.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBusyIconElement = BusyIconElement.ToBitmapImage();

        public static readonly BitmapImage ImageSourceIconPlaying = IconPlaying.ToBitmapImage();

        public static readonly BitmapImage ImageSourceRepeatActiveRollover = RepeatActiveRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRepeatActiveDown = RepeatActiveDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceShuffle = Shuffle.ToBitmapImage();
        public static readonly BitmapImage ImageSourceShuffleRollover = ShuffleRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceShuffleDown = ShuffleDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceShuffleActive = ShuffleActive.ToBitmapImage();
        public static readonly BitmapImage ImageSourceShuffleActiveRollover = ShuffleActiveRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceShuffleActiveDown = ShuffleActiveDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSave = Save.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSaveRollover = SaveRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSaveDown = SaveDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceDelete = Delete.ToBitmapImage();
        public static readonly BitmapImage ImageSourceDeleteRollover = DeleteRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceDeleteDown = DeleteDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceHome = Home.ToBitmapImage();
        public static readonly BitmapImage ImageSourceHomeRollover = HomeRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceHomeDown = HomeDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceHomeOn = HomeOn.ToBitmapImage();
        public static readonly BitmapImage ImageSourceHomeOnRollover = HomeOnRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceHomeOnDown = HomeOnDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceReceivers = Receivers.ToBitmapImage();
        public static readonly BitmapImage ImageSourceReceiversRollover = ReceiversRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceReceiversDown = ReceiversDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceReceiversOn = ReceiversOn.ToBitmapImage();
        public static readonly BitmapImage ImageSourceReceiversOnRollover = ReceiversOnRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceReceiversOnDown = ReceiversOnDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStandby = Standby.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStandbyRollover = StandbyRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStandbyDown = StandbyDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStandbyActive = StandbyActive.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStandbyActiveRollover = StandbyActiveRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStandbyActiveDown = StandbyActiveDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSize = Size.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSizeDown = SizeDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSizeActive = SizeActive.ToBitmapImage();
        public static readonly BitmapImage ImageSourceView = View.ToBitmapImage();
        public static readonly BitmapImage ImageSourceViewRollover = ViewRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceViewDown = ViewDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBack = Back.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBackRollover = BackRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBackDown = BackDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRepeat = Repeat.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRepeatRollover = RepeatRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRepeatDown = RepeatDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRepeatActive = RepeatActive.ToBitmapImage();
        public static readonly BitmapImage ImageSourceList = List.ToBitmapImage();
        public static readonly BitmapImage ImageSourceListRollover = ListRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceListDown = ListDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceThumbs = Thumbs.ToBitmapImage();
        public static readonly BitmapImage ImageSourceThumbsRollover = ThumbsRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceThumbsDown = ThumbsDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRemove = Remove.ToBitmapImage();

        public static readonly BitmapImage ImageSourceScrollCircle = ScrollCircle.ToBitmapImage();
        public static readonly BitmapImage ImageSourceScrollCircleOpaque = ScrollCircleOpaque.ToBitmapImage();
        public static readonly BitmapImage ImageSourceScrollCircleMouse = ScrollCircleMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceScrollCircleTouch = ScrollCircleTouch.ToBitmapImage();

        public static readonly BitmapImage ImageSourceArray = Array.ToBitmapImage();
        public static readonly BitmapImage ImageSourceTramlines = Tramlines.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSkipBackOver = SkipBackOver.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSkipBackDown = SkipBackDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlay = Play.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayRollover = PlayRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayDown = PlayDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStop = Stop.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStopRollover = StopRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourceStopDown = StopDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePause = Pause.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePauseRollover = PauseRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePauseDown = PauseDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSkipForward = SkipForward.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSkipForwardOver = SkipForwardOver.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSkipForwardDown = SkipForwardDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceSkipBack = SkipBack.ToBitmapImage();


        public static readonly BitmapImage ImageSourceRocker = Rocker.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRockerLeftOver = RockerLeftOver.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRockerRightOver = RockerRightOver.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRockerLeftDown = RockerLeftDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRockerRightDown = RockerRightDown.ToBitmapImage();

        public static readonly BitmapImage ImageSourceWheel = Wheel.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWheelOver = WheelOver.ToBitmapImage();
        public static readonly BitmapImage ImageSourceScrews = Screws.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWheelMute = WheelMute.ToBitmapImage();

        public static readonly BitmapImage ImageSourceWindowsCloseMouse = WindowsCloseMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMiniTouch = WindowsMiniTouch.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMaximize = WindowsMaximize.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMiniMouse = WindowsMiniMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMinimize = WindowsMinimize.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMinimizeMouse = WindowsMinimizeMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsRestoreMouse = WindowsRestoreMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMaximizeMouse = WindowsMaximizeMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsClose = WindowsClose.ToBitmapImage();
        public static readonly BitmapImage ImageSourceMiniModeCloseTouch = MiniModeCloseTouch.ToBitmapImage();
        public static readonly BitmapImage ImageSourceMiniModeCloseMouse = MiniModeCloseMouse.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMaximizeTouch = WindowsMaximizeTouch.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsCloseTouch = WindowsCloseTouch.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsRestoreTouch = WindowsRestoreTouch.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMinimizeTouch = WindowsMinimizeTouch.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsMini = WindowsMini.ToBitmapImage();
        public static readonly BitmapImage ImageSourceWindowsRestore = WindowsRestore.ToBitmapImage();
        public static readonly BitmapImage ImageSourceMiniModeClose = MiniModeClose.ToBitmapImage();

        public static readonly BitmapImage ImageSourceLogo = Logo.ToBitmapImage();

        public static readonly BitmapImage ImageSourceDrawerTop = DrawerTop.ToBitmapImage();
        public static readonly BitmapImage ImageSourceDrawerTopFiller = DrawerTopFiller.ToBitmapImage();

        public static readonly BitmapImage ImageSourceTransparency = Transparency.ToBitmapImage();

        public static readonly BitmapImage ImageSourcePlayNext = PlayNext.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayNextRollover = PlayNextRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayNow = PlayNow.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayNowRollover = PlayNowRollover.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayLater = PlayLater.ToBitmapImage();
        public static readonly BitmapImage ImageSourcePlayLaterRollover = PlayLaterRollover.ToBitmapImage();

        public static readonly BitmapImage ImageSourceBoxLeft = BoxLeft.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxOverLeft = BoxOverLeft.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxDownLeft = BoxDownLeft.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxFiller = BoxFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxOverFiller = BoxOverFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxDownFiller = BoxDownFiller.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxRight = BoxRight.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxOverRight = BoxOverRight.ToBitmapImage();
        public static readonly BitmapImage ImageSourceBoxDownRight = BoxDownRight.ToBitmapImage();


        public static readonly BitmapImage ImageSourceAddTab = AddTab.ToBitmapImage();
        public static readonly BitmapImage ImageSourceAddTabDown = AddTabDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceAddTabOver = AddTabOver.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRemoveTab = RemoveTab.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRemoveTabDown = RemoveTabDown.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRemoveTabOver = RemoveTabOver.ToBitmapImage();

        public static readonly BitmapImage ImageSourceTick = Tick.ToBitmapImage();
        public static readonly BitmapImage ImageSourceRefreshButton = RefreshButton.ToBitmapImage();

    }



}