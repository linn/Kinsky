using System;
using System.Net;

using Foundation;
using UIKit;

using Linn;
using Linn.Kinsky;
using Linn.Toolkit.Ios;
using CoreGraphics;

namespace KinskyTouch
{
    // The name AppDelegateIPad is referenced in the MainWindowIPad.xib file.
    public partial class AppDelegateIpad : AppDelegate
    {
        // This method is invoked when the application has loaded its UI and its ready to run
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			CreateNotificationView();
            Xamarin.Insights.Identify(Helper.OptionInstallId.Value, null);
            ObjCRuntime.Class.ThrowOnInitFailure = false;

            Ticker tick = new Ticker();

			helper.Helper.SetStackExtender(this);
            helper.Helper.Stack.EventStatusChanged += StatusChanged;

            ArtworkCacheInstance.Instance = new ArtworkCache();

            controlRotaryVolume.ViewBar.FontSize = 15.0f;
            controlRotaryVolume.ViewBar.InnerCircleRadius = 30.0f;
            controlRotaryVolume.ViewBar.OuterCircleRadius = 35.0f;
            iViewWidgetVolumeControl = new ViewWidgetVolumeControl(controlRotaryVolume);

            iViewWidgetVolumeButtons = new ViewWidgetVolumeButtons("VolumeButtons", null);
            iViewWidgetVolumeButtons.RepeatInterval = 0.1f;

            iViewWidgetVolumeRotary = new ViewWidgetVolumeRotary("VolumeRotary", null);

            iVolumeController = new VolumeControllerIpad(iViewWidgetVolumeButtons, iViewWidgetVolumeRotary, controlVolume);
            iVolumeController.SetRockers(helper.OptionEnableRocker.Native);
            viewController.SetVolumeController(iVolumeController);

            controlRotaryTime.ViewBar.FontSize = 15.0f;
            controlRotaryTime.ViewBar.InnerCircleRadius = 30.0f;
            controlRotaryTime.ViewBar.OuterCircleRadius = 35.0f;
            iViewWidgetTime = new ViewWidgetTime(controlRotaryTime, viewHourGlass);

            iViewWidgetTimeButtons = new ViewWidgetTimeButtons("TimeButtons", null, iViewWidgetTime);
            iViewWidgetTimeButtons.RepeatInterval = 0.1f;

            iViewWidgetTimeRotary = new ViewWidgetTimeRotary("TimeRotary", null, iViewWidgetTime);

            iTimeController = new TimeControllerIpad(iViewWidgetTimeButtons, iViewWidgetTimeRotary, controlTime);
            iTimeController.SetRockers(helper.OptionEnableRocker.Native);
            viewController.SetTimeController(iTimeController);

            controlVolume.Hidden = false;
            controlTime.Hidden = false;

            helper.OptionEnableRocker.EventValueChanged += delegate(object sender, EventArgs e) {
                iVolumeController.SetRockers(helper.OptionEnableRocker.Native);
                iTimeController.SetRockers(helper.OptionEnableRocker.Native);
            };

            Reachability.LocalWifiConnectionStatus();
            Reachability.ReachabilityChanged += delegate(object sender, EventArgs e) {
                OnReachabilityChanged();
            };

            new Action(delegate {
                Ticker ticker = new Ticker();

                iViewMaster = new ViewMaster();
    
                iHttpServer = new HttpServer(HttpServer.kPortKinskyTouch);
                iHttpClient = new HttpClient();
    
                iLibrary = new MediaProviderLibrary(helper.Helper);
                iSharedPlaylists = new SharedPlaylists(helper.Helper);
                iLocalPlaylists = new LocalPlaylists(helper.Helper, false);
                iLocalPlaylists.SaveDirectory.ResetToDefault();

                iConfigController = new ConfigControllerIpad(helper.Helper);
                iPlaySupport = new PlaySupport();
    
                MediaProviderSupport support = new MediaProviderSupport(iHttpServer);
                PluginManager pluginManager = new PluginManager(helper.Helper, iHttpClient, support);
    
                iLocator = new ContentDirectoryLocator(pluginManager, new AppRestartHandler());
                iLocator.Add(MediaProviderLibrary.kLibraryId, iLibrary);
                OptionBool optionSharedPlaylists = iLocator.Add(SharedPlaylists.kRootId, iSharedPlaylists);
                OptionBool optionLocalPlaylists = iLocator.Add(LocalPlaylists.kRootId, iLocalPlaylists);

                iSaveSupport = new SaveSupport(helper.Helper, iSharedPlaylists, optionSharedPlaylists, iLocalPlaylists, optionLocalPlaylists);
                iViewSaveSupport = new ViewSaveSupport(SavePlaylistHandler, iSaveSupport);
    
                helper.Helper.AddOptionPage(iLocator.OptionPage);
    
                InvokeOnMainThread(delegate {
                    AddViews();
                });

                iModel = new Model(iViewMaster, iPlaySupport);
                iMediator = new Mediator(helper.Helper, iModel);
                
                OnFinishedLaunching();

                UserLog.WriteLine(string.Format("FinishedLaunching background tasks in {0} ms", ticker.MilliSeconds));
            }).BeginInvoke(null, null);
            
            //Trace.Level = Trace.kKinskyTouch;
            
            // If you have defined a view, add it here:
            navigationController.View.Frame = new CGRect(CGPoint.Empty, viewBrowser.Frame.Size);
            viewBrowser.InsertSubview(navigationController.View, 0);

            /*ArtworkTileViewFactory f = new ArtworkTileViewFactory(iLibrary);
            NSBundle.MainBundle.LoadNib("ArtworkTileView", f, null);
            viewController.View.AddSubview(f.View);
            f.Initialise();*/

            window.RootViewController = viewController;
            
            window.MakeKeyAndVisible();

            UserLog.WriteLine(string.Format("FinishedLaunching in {0} ms", tick.MilliSeconds));

            return true;
        }

        protected override HelperKinskyTouch Helper
        {
            get
            {
                return helper;
            }
        }

        protected override UIViewController ViewController
        {
            get
            {
                return viewController;
            }
        }
		
		private void AddViews()
		{
            ViewWidgetSelectorRoom viewWidgetSelectorRoom = new ViewWidgetSelectorRoom();
			viewWidgetSelectorRoom.View.BackgroundColor = UIColor.Clear;
            ViewWidgetSelectorPopover<Linn.Kinsky.Room> popOverRoom = new ViewWidgetSelectorPopover<Room>(helper.Helper, viewWidgetSelectorRoom, viewWidgetSelectorRoom, navigationItemSource.LeftBarButtonItem, navigationItemSource.RightBarButtonItem);
			iViewMaster.ViewWidgetSelectorRoom.Add(viewWidgetSelectorRoom);
            iViewMaster.ViewWidgetSelectorRoom.Add(popOverRoom);

            ViewWidgetSelectorSource viewWidgetSelectorSource = new ViewWidgetSelectorSource();
			viewWidgetSelectorSource.View.BackgroundColor = UIColor.Clear;
            ViewWidgetSelectorPopover<Linn.Kinsky.Source> popOverSource = new ViewWidgetSelectorPopover<Source>(helper.Helper, viewWidgetSelectorSource, viewWidgetSelectorSource, navigationItemSource.RightBarButtonItem, navigationItemSource.LeftBarButtonItem);
			iViewMaster.ViewWidgetSelectorSource.Add(viewWidgetSelectorSource);
            iViewMaster.ViewWidgetSelectorSource.Add(popOverSource);

            viewInfo.Alignment = UITextAlignment.Left;
            viewInfo.TopAlign = true;

            viewOverlayInfo.Alignment = UITextAlignment.Center;
            viewOverlayInfo.TopAlign = false;

            ViewWidgetTrackArtworkRetriever artworkRetriever = new ViewWidgetTrackArtworkRetriever();
            ViewWidgetTrackArtwork artwork = new ViewWidgetTrackArtwork(imageViewArtwork);
            artworkRetriever.AddReceiver(artwork);

            iViewMaster.ViewWidgetTrack.Add(artworkRetriever);
            iViewMaster.ViewWidgetTrack.Add(artwork);
			iViewMaster.ViewWidgetTrack.Add(new ViewWidgetTrackMetadata(viewInfo, helper.OptionExtendedTrackInfo));
            iViewMaster.ViewWidgetTrack.Add(new ViewWidgetTrackMetadata(viewOverlayInfo, helper.OptionExtendedTrackInfo));
			
			ViewWidgetTransportControl transportControl = new ViewWidgetTransportControl(buttonLeft, buttonCentre, buttonRight);
			iViewMaster.ViewWidgetTransportControlDiscPlayer.Add(transportControl);
			iViewMaster.ViewWidgetTransportControlMediaRenderer.Add(transportControl);
			iViewMaster.ViewWidgetTransportControlRadio.Add(transportControl);

            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeControl);
            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeButtons);
            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeRotary);

            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetTime);
            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetTimeButtons);
            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetTimeRotary);

            iViewMaster.ViewWidgetPlayMode.Add(new ViewWidgetPlayMode(sourceToolbar, buttonShuffle, buttonRepeat));
            sourceToolbar.Initialise(buttonShuffle, buttonRepeat);

            iViewMaster.ViewWidgetPlaylist.Add(new ViewWidgetPlaylistMediaRenderer(tableViewSource, sourceToolbar, buttonViewInfo, iViewSaveSupport, helper.OptionGroupTracks));
            iViewMaster.ViewWidgetPlaylistRadio.Add(new ViewWidgetPlaylistRadio(tableViewSource, buttonViewInfo, iViewSaveSupport));

            ViewWidgetPlaylistReceiver playlistReceiver = new ViewWidgetPlaylistReceiver(tableViewSource, buttonViewInfo, imageViewPlaylistAux, iViewSaveSupport);
            iViewMaster.ViewWidgetPlaylistReceiver.Add(playlistReceiver);
            iViewMaster.ViewWidgetSelectorRoom.Add(playlistReceiver);

            iViewMaster.ViewWidgetPlaylistDiscPlayer.Add(new ViewWidgetPlaylistDiscPlayer(imageViewPlaylistAux));
            iViewMaster.ViewWidgetPlaylistAux.Add(new ViewWidgetPlaylistAux(imageViewPlaylistAux));

            ViewWidgetBrowserRoot viewBrowser = navigationController.TopViewController as ViewWidgetBrowserRoot;
            viewBrowser.Initialise(new Location(iLocator.Root), iPlaySupport, iConfigController, helper.OptionInsertMode, helper.Helper.LastLocation);

            iViewMaster.ViewWidgetButtonWasteBin.Add(new ViewWidgetButtonWasteBin(sourceToolbar.BarButtonItemDelete));
            iViewMaster.ViewWidgetButtonSave.Add(new ViewWidgetButtonSave(sourceToolbar.BarButtonItemSave));
		}

        /*private void DO_NOT_CALL()
        {
            new OssKinskyMppBbc.ContentDirectoryFactoryBbc();
            new OssKinskyMppMovieTrailers.MediaProviderMovieTrailersFactory();
            new OssKinskyMppWfmu.ContentDirectoryFactoryWfmu();
            throw new NotSupportedException();
        }*/
		
        private LocalPlaylists iLocalPlaylists;
		private Model iModel;

        private IViewSaveSupport iViewSaveSupport;
		private PlaySupport iPlaySupport;
        private SaveSupport iSaveSupport;

        private VolumeControllerIpad iVolumeController;
        private TimeControllerIpad iTimeController;
        private ConfigControllerIpad iConfigController;

		private ViewMaster iViewMaster;

        private ViewWidgetVolumeControl iViewWidgetVolumeControl;
        private ViewWidgetVolumeButtons iViewWidgetVolumeButtons;
        private ViewWidgetVolumeRotary iViewWidgetVolumeRotary;

        private ViewWidgetTime iViewWidgetTime;
        private ViewWidgetTimeButtons iViewWidgetTimeButtons;
        private ViewWidgetTimeRotary iViewWidgetTimeRotary;
    }
}
