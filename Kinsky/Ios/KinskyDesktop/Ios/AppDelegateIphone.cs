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

    // The name AppDelegateIPhone is referenced in the MainWindowIPhone.xib file.
    public partial class AppDelegateIphone : AppDelegate
    {
        // This method is invoked when the application has loaded its UI and its ready to run
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
			CreateNotificationView();
            Xamarin.Insights.Identify(Helper.OptionInstallId.Value, null);
            ObjCRuntime.Class.ThrowOnInitFailure = false;

			Ticker tick = new Ticker();

            helper.Helper.SetStackExtender(this);
            helper.Helper.Stack.EventStatusChanged += StatusChanged;
			
			viewControllerRooms.StandbyButtonOffsetX = 5;

            ArtworkCacheInstance.Instance = new ArtworkCache();

            controlRotaryVolume.ViewBar.FontSize = 12.0f;
            controlRotaryVolume.ViewBar.InnerCircleRadius = 25.0f;
            controlRotaryVolume.ViewBar.OuterCircleRadius = 30.0f;
            iViewWidgetVolumeControl = new ViewWidgetVolumeControl(controlRotaryVolume);

            iViewWidgetVolumeRotary = new ViewWidgetVolumeRotary("VolumeRotary", null);

            iViewWidgetVolumeButtons = new ViewWidgetVolumeButtons("VolumeButtons", null);
            iViewWidgetVolumeButtons.RepeatInterval = 0.1f;

            iVolumeController = new VolumeControllerIphone(viewControllerNowPlaying, iViewWidgetVolumeRotary, iViewWidgetVolumeButtons, controlVolume, controlTime, scrollView);
            iVolumeController.SetRockers(helper.OptionEnableRocker.Native);

            controlRotaryTime.ViewBar.FontSize = 12.0f;
            controlRotaryTime.ViewBar.InnerCircleRadius = 25.0f;
            controlRotaryTime.ViewBar.OuterCircleRadius = 30.0f;
            iViewWidgetTime = new ViewWidgetTime(controlRotaryTime, viewHourGlass);

            iViewWidgetTimeRotary = new ViewWidgetTimeRotary("TimeRotary", null, iViewWidgetTime);

            iViewWidgetTimeButtons = new ViewWidgetTimeButtons("TimeButtons", null, iViewWidgetTime);
            iViewWidgetTimeButtons.RepeatInterval = 0.1f;

            iTimeController = new TimeControllerIphone(viewControllerNowPlaying, iViewWidgetTimeRotary, iViewWidgetTimeButtons, controlTime, controlVolume, scrollView);
            iTimeController.SetRockers(helper.OptionEnableRocker.Native);

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
			UIButton buttonStandby = CreateStandbyButton();
			UIButton buttonStandbyAll = CreateStandbyButton();
            iViewMaster.ViewWidgetSelectorRoom.Add(viewControllerRooms);
            iViewMaster.ViewWidgetSelectorRoom.Add(new ViewWidgetSelectorRoomNavigation(helper.Helper, navigationControllerRoomSource, scrollView, viewControllerSources, buttonRefresh, buttonStandby, buttonStandbyAll));

            iViewMaster.ViewWidgetSelectorSource.Add(viewControllerSources);

            viewInfo.TopAlign = true;
            viewInfo.Alignment = UITextAlignment.Left;

            ViewWidgetTrackArtworkRetriever artworkRetriever = new ViewWidgetTrackArtworkRetriever();
            ViewWidgetTrackArtwork artwork = new ViewWidgetTrackArtwork(imageViewArtwork);
            artworkRetriever.AddReceiver(artwork);
            artworkRetriever.AddReceiver(new ImageReceiverButton(buttonArtwork));

            iViewMaster.ViewWidgetTrack.Add(artworkRetriever);
            iViewMaster.ViewWidgetTrack.Add(artwork);
            iViewMaster.ViewWidgetTrack.Add(new ViewWidgetTrackMetadata(viewInfo, helper.OptionExtendedTrackInfo));

            ViewWidgetTransportControl transportControl = new ViewWidgetTransportControl(buttonLeft, buttonCentre, buttonRight);
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Add(transportControl);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Add(transportControl);
            iViewMaster.ViewWidgetTransportControlRadio.Add(transportControl);

            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeControl);
            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeRotary);
            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeButtons);

            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetTime);
            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetTimeRotary);
            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetTimeButtons);

            iViewMaster.ViewWidgetPlayMode.Add(new ViewWidgetPlayMode(sourceToolbar, buttonShuffle, buttonRepeat));
            sourceToolbar.Initialise(buttonShuffle, buttonRepeat);

            iViewMaster.ViewWidgetPlaylist.Add(new ViewWidgetPlaylistMediaRenderer(tableViewSource, sourceToolbar, new UIButton(), iViewSaveSupport, helper.OptionGroupTracks));
            iViewMaster.ViewWidgetPlaylistRadio.Add(new ViewWidgetPlaylistRadio(tableViewSource, new UIButton(), iViewSaveSupport));

            ViewWidgetPlaylistReceiver playlistReceiver = new ViewWidgetPlaylistReceiver(tableViewSource, new UIButton(), imageViewPlaylistAux, iViewSaveSupport);
            iViewMaster.ViewWidgetPlaylistReceiver.Add(playlistReceiver);
            iViewMaster.ViewWidgetSelectorRoom.Add(playlistReceiver);

            iViewMaster.ViewWidgetPlaylistDiscPlayer.Add(new ViewWidgetPlaylistDiscPlayer(imageViewPlaylistAux));
            iViewMaster.ViewWidgetPlaylistAux.Add(new ViewWidgetPlaylistAux(imageViewPlaylistAux));

            ViewWidgetBrowserRoot viewBrowser = navigationController.TopViewController as ViewWidgetBrowserRoot;
            viewBrowser.Initialise(new Location(iLocator.Root), iPlaySupport, new ConfigControllerIphone(viewController, helper.Helper), helper.OptionInsertMode, helper.Helper.LastLocation);
			
            iViewMaster.ViewWidgetButtonStandby.Add(new ViewWidgetButtonStandby(buttonStandby));
            iViewMaster.ViewWidgetButtonWasteBin.Add(new ViewWidgetButtonWasteBin(sourceToolbar.BarButtonItemDelete));
            iViewMaster.ViewWidgetButtonSave.Add(new ViewWidgetButtonSave(sourceToolbar.BarButtonItemSave));
        }
		
		private UIButton CreateStandbyButton()
		{
			UIButton result = new UIButton(new CGRect(0, 0, 35, 30));
            result.SetImage(new UIImage("Standby.png"), UIControlState.Normal);
            result.SetImage(new UIImage("StandbyDown.png"), UIControlState.Highlighted);
            result.SetImage(new UIImage("StandbyOn.png"), UIControlState.Selected);
            result.ShowsTouchWhenHighlighted = true;
			return result;
		}
		
        
		private LocalPlaylists iLocalPlaylists;
		private Model iModel;

        private IViewSaveSupport iViewSaveSupport;
        private PlaySupport iPlaySupport;
        private SaveSupport iSaveSupport;

        private VolumeControllerIphone iVolumeController;
        private TimeControllerIphone iTimeController;

        private ViewMaster iViewMaster;

        private ViewWidgetVolumeControl iViewWidgetVolumeControl;
        private ViewWidgetVolumeRotary iViewWidgetVolumeRotary;
        private ViewWidgetVolumeButtons iViewWidgetVolumeButtons;

        private ViewWidgetTime iViewWidgetTime;
        private ViewWidgetTimeRotary iViewWidgetTimeRotary;
        private ViewWidgetTimeButtons iViewWidgetTimeButtons;
    }
}
