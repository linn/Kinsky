using Android.Widget;
using Linn.Kinsky;
using System;
using Upnp;
using Linn;
using Android.Content;
using OssToolkitDroid;
using Android.Views;
using Android.Graphics;
using Android.Util;
using System.Collections.Generic;
using Android.Runtime;
using Android.Views.Animations;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Graphics.Drawables;
using Android.Content.PM;
using Android.Support.V4.View;
using Android.Webkit;

namespace KinskyDroid
{

    public class PhoneViewPagerAdapter : PagerAdapter
    {
        public PhoneViewPagerAdapter(List<View> aViews, ViewGroup aHiddenContainer)
            : base()
        {
            iViews = aViews;
            iHiddenContainer = aHiddenContainer;
        }

        public void Close()
        {
            foreach (View v in iViews)
            {
                v.Dispose();
            }
            iViews.Clear();
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup p0, int p1)
        {
            View v = iViews[p1];
            iHiddenContainer.RemoveView(v);
            p0.AddView(v);
            return v;
        }

        public override void DestroyItem(ViewGroup p0, int p1, Java.Lang.Object p2)
        {
            p0.RemoveView((View)p2);
            iHiddenContainer.AddView((View)p2);
        }

        public override void DestroyItem(View p0, int p1, Java.Lang.Object p2)
        {
            (p0 as ViewGroup).RemoveView((View)p2);
            iHiddenContainer.AddView((View)p2);
        }

        public override Java.Lang.Object InstantiateItem(View p0, int p1)
        {
            View v = iViews[p1];
            iHiddenContainer.RemoveView(v);
            (p0 as ViewGroup).AddView(v);
            return v;
        }

        public override int Count
        {
            get
            {
                return iViews.Count;
            }
        }

        public override bool IsViewFromObject(View p0, Java.Lang.Object p1)
        {
            return p0 == (View)p1;
        }

        public override void RestoreState(Android.OS.IParcelable p0, Java.Lang.ClassLoader p1)
        {
        }

        public override void FinishUpdate(ViewGroup p0)
        {
        }

        public override void FinishUpdate(View p0)
        {
        }

        public override void StartUpdate(View p0)
        {
        }

        public override void StartUpdate(ViewGroup p0)
        {
        }

        public override Android.OS.IParcelable SaveState()
        {
            return null;
        }

        private List<View> iViews;
        private ViewGroup iHiddenContainer;
    }

    public class PhonePagerListener : ViewPager.SimpleOnPageChangeListener
    {
        public event EventHandler<EventArgsPageSelection> EventPageSelected;
        public event EventHandler<EventArgsScrollState> EventScrollStateChanged;
        public PhonePagerListener() : base() { }

        public override void OnPageSelected(int p0)
        {
            OnEventPageSelected((EPageIndex)p0);
        }

        public override void OnPageScrollStateChanged(int p0)
        {
            OnEventScrollStateChanged((EScrollState)p0);
        }

        private void OnEventScrollStateChanged(EScrollState aScrollState)
        {
            EventHandler<EventArgsScrollState> del = EventScrollStateChanged;
            if (del != null)
            {
                del(this, new EventArgsScrollState(aScrollState));
            }
        }
        private void OnEventPageSelected(EPageIndex aPage)
        {
            EventHandler<EventArgsPageSelection> del = EventPageSelected;
            if (del != null)
            {
                del(this, new EventArgsPageSelection(aPage));
            }
        }
    }

    public enum EScrollState
    {
        Idle = 0,
        Dragging = 1,
        Settling = 2
    }

    public class EventArgsPageSelection : EventArgs
    {
        public EventArgsPageSelection(EPageIndex aPage)
        {
            Page = aPage;
        }

        public EPageIndex Page { get; set; }
    }

    public class EventArgsScrollState : EventArgs
    {
        public EventArgsScrollState(EScrollState aScrollState)
        {
            ScrollState = aScrollState;
        }

        public EScrollState ScrollState { get; set; }
    }

    public class ViewKinskyPhone : ViewKinsky
    {
        public ViewKinskyPhone(Stack aStack, Activity aActivity, AndroidViewMaster aViewMaster, AndroidResourceManager aResourceManager, IconResolver aIconResolver)
            : base(aStack, aActivity, aViewMaster, aResourceManager, aIconResolver)
        {
            iBrowserPopupFactory = new SpeechBubblePopupFactory(aStack, Color.Black);
            iPopupFactory = new OverlayPopupFactory(iStack, new Color(0, 0, 0, 127));
            iCurrentPage = aViewMaster.CurrentPageIndex;
            Init(aActivity);
        }


        protected override void SavePlaylist(ISaveSupport aSaveSupport)
        {
            if (!PopupManager.IsShowingPopup && iOpen)
            {
                View saveButton = iRootView.FindViewById(Resource.Id.playlistsavebutton);
                View popupAnchor = iRootView.FindViewById(Resource.Id.playlisttopsectionphone);
                iSavePlaylistDialog = new SavePlaylistDialog(iStack, iPopupFactory, saveButton, popupAnchor, Color.Black, iStack.SaveSupport, iViewMaster.ImageCache, iIconResolver, true);
                iSavePlaylistDialog.EventDismissed += SavePlaylistDialog_EventDismissedHandler;
            }
        }

        protected override void Init(Activity aActivity)
        {
            Assert.Check(!iStack.Invoker.InvokeRequired);

            aActivity.RequestedOrientation = ScreenOrientation.Portrait;
            aActivity.SetContentView(Resource.Layout.MainPhone);
            iRootView = aActivity.FindViewById(Resource.Id.rootview);
            iHiddenContainer = aActivity.FindViewById<ViewGroup>(Resource.Id.hiddencontainerphone);
            List<View> views = new List<View>();
            views.Add(iStack.LayoutInflater.Inflate(Resource.Layout.RoomAndSourceControlsPhone, null));
            views.Add(iStack.LayoutInflater.Inflate(Resource.Layout.PlaylistScreenPhone, null));
            views.Add(iStack.LayoutInflater.Inflate(Resource.Layout.BrowserScreenPhone, null));
            foreach (View v in views)
            {
                iHiddenContainer.AddView(v);
            }
            iViewPager = aActivity.FindViewById<ViewPager>(Resource.Id.viewpagerphone);
            iViewPager.Adapter = new PhoneViewPagerAdapter(views, iHiddenContainer);

            iViewPager.CurrentItem = (int)iCurrentPage;
            iPhonePagerListener = new PhonePagerListener();
            iPhonePagerListener.EventPageSelected += EventPageSelectedHandler;
            iViewPager.SetOnPageChangeListener(iPhonePagerListener);
            iPlaylistContainer = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iPageIndicator = iRootView.FindViewById<MultiViewPageIndicator>(Resource.Id.viewpageindicatorphone);
            iPageIndicator.EventPagePrevious += EventPagePreviousHandler;
            iPageIndicator.EventPageNext += EventPageNextHandler;
            iPageIndicator.Count = views.Count;
            iPageIndicator.SelectedIndex = (int)iCurrentPage;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (iRootView != null)
            {

                iRootView.FindViewById(Resource.Id.settingsbutton).Dispose();
                iRootView.FindViewById(Resource.Id.browsereditmodebutton).Dispose();
                iRootView.FindViewById(Resource.Id.browserthrobber).Dispose();
                iRootView.FindViewById(Resource.Id.backbutton).Dispose();
                iRootView.FindViewById(Resource.Id.locationdisplay).Dispose();
                iRootView.FindViewById(Resource.Id.playnownextlaterbutton).Dispose();
                iRootView.FindViewById(Resource.Id.browser).Dispose();
                iRootView.FindViewById(Resource.Id.roomlist).Dispose();
                iRootView.FindViewById(Resource.Id.sourcelist).Dispose();
                iRootView.FindViewById(Resource.Id.roomsrefreshbutton).Dispose();
                iRootView.FindViewById(Resource.Id.roomsrefreshthrobber).Dispose();
                iRootView.FindViewById(Resource.Id.roomandsourceviewswitcher).Dispose();
                iRootView.FindViewById(Resource.Id.sourcesstandbybutton).Dispose();
                iRootView.FindViewById(Resource.Id.sourcelistbackbutton).Dispose();
                iRootView.FindViewById(Resource.Id.sourcelisttitle).Dispose();
                iRootView.FindViewById(Resource.Id.standbybuttonall).Dispose();
                iRootView.FindViewById(Resource.Id.transportcontrols).Dispose();
                iRootView.FindViewById(Resource.Id.volumedisplay).Dispose();
                iRootView.FindViewById(Resource.Id.playlisttopsectionphone).Dispose();
                iRootView.FindViewById(Resource.Id.timedisplay).Dispose();
                iRootView.FindViewById(Resource.Id.playlist).Dispose();
                iRootView.FindViewById(Resource.Id.playlisteditmodebutton).Dispose();
                iRootView.FindViewById(Resource.Id.playlistsavebutton).Dispose();
                iRootView.FindViewById(Resource.Id.playlistdeletebutton).Dispose();
                iRootView.FindViewById(Resource.Id.playlistbuttons).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay1).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay2).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay3).Dispose();
                iRootView.FindViewById(Resource.Id.trackartwork).Dispose();
                iRootView.FindViewById(Resource.Id.trackartworkfullscreen).Dispose();
                iRootView.FindViewById(Resource.Id.shufflebutton).Dispose();
                iRootView.FindViewById(Resource.Id.repeatbutton).Dispose();
                iRootView.FindViewById(Resource.Id.fullscreenartworkcontainerphone).Dispose();
                iRootView.FindViewById(Resource.Id.playlistcontainerphone).Dispose();
                iRootView.FindViewById(Resource.Id.trackshowplaylistbutton).Dispose();

                iPhonePagerListener.EventPageSelected -= EventPageSelectedHandler;
                iPhonePagerListener.Dispose();
                iPhonePagerListener = null;
                PhoneViewPagerAdapter adapter = iViewPager.Adapter as PhoneViewPagerAdapter;
                iViewPager.Adapter = null;
                iViewPager.RemoveAllViews();
                iViewPager.SetOnPageChangeListener(null);
                iViewPager.Dispose();
                iViewPager = null;
                adapter.Close();
                adapter.Dispose();
                adapter = null;
                iHiddenContainer.RemoveAllViews();
                iHiddenContainer.Dispose();
                iHiddenContainer = null;
                iPageIndicator.EventPagePrevious -= EventPagePreviousHandler;
                iPageIndicator.EventPageNext -= EventPageNextHandler;
                iPageIndicator.Dispose();
                iPageIndicator = null;

                iRootView.Dispose();
                iRootView = null;
            }
        }

        private void EventPagePreviousHandler(object sender, EventArgs e)
        {
            if (iCurrentPage > 0)
            {
                iViewPager.CurrentItem = (int)iCurrentPage - 1;
            }
        }

        private void EventPageNextHandler(object sender, EventArgs e)
        {
            if ((int)iCurrentPage < iViewPager.Adapter.Count - 1)
            {
                iViewPager.CurrentItem = (int)iCurrentPage + 1;
            }
        }

        private void EventPageSelectedHandler(object sender, EventArgsPageSelection e)
        {
            iCurrentPage = (EPageIndex)e.Page;
            iPageIndicator.SelectedIndex = (int)iCurrentPage;
            iViewMaster.CurrentPageIndex = iCurrentPage;
        }

        protected override void OnOpened()
        {
            iOptionsMediator = new OptionsMediator(iPopupFactory, iStack, iViewMaster.ImageCache, iIconResolver, true);
            iOptionsMediator.PopupAnchor = iRootView.FindViewById(Resource.Id.viewpagerphone);
            iRootView.KeepScreenOn = iStack.AutoLock;

            iStack.EventAutoLockChanged += EventAutoLockChangedHandler;

            iOptionsMediator.SettingsButton = iRootView.FindViewById(Resource.Id.settingsbutton);
            iOptionsMediator.SettingsButtonBadge = iRootView.FindViewById(Resource.Id.settingsbuttonbadge);

            iStack.OptionEnableRocker.EventValueChanged += OptionEnableRockerEventValueChangedHandler;

            // browser
            ToggleButton editButton = iRootView.FindViewById<ToggleButton>(Resource.Id.browsereditmodebutton);
            Throbber throbber = iRootView.FindViewById<Throbber>(Resource.Id.browserthrobber);
            iBrowser = new ViewWidgetBrowser(iStack,
                                             iStack.RootLocation,
                                             iStack.Invoker,
                                             iViewMaster.ImageCache,
                                             iIconResolver,
                                             iRootView.FindViewById<Button>(Resource.Id.backbutton),
                                             iRootView.FindViewById<TextView>(Resource.Id.locationdisplay),
                                             editButton,
                                             iViewMaster.FlingStateManager,
                                             throbber,
                                             iRootView.FindViewById<Button>(Resource.Id.playnownextlaterbutton),
                                             iStack.PlaySupport,
                                             iStack.OptionInsertMode,
                                             iBrowserPopupFactory);
            iBrowser.EventLocationChanged += EventLocationChangedHandler;
            iBrowser.Navigate(iStack.CurrentLocation, 5);
            (iRootView.FindViewById(Resource.Id.browser) as RelativeLayout).AddView(iBrowser);


            // room/source selection
            ListView roomList = iRootView.FindViewById<ListView>(Resource.Id.roomlist);
            ListView sourceList = iRootView.FindViewById<ListView>(Resource.Id.sourcelist);
            roomList.Visibility = ViewStates.Visible;
            roomList.DividerHeight = 0;
            sourceList.Visibility = ViewStates.Visible;
            sourceList.DividerHeight = 0;
            ImageButton refreshButton = iRootView.FindViewById<ImageButton>(Resource.Id.roomsrefreshbutton);
            Throbber refreshThrobber = iRootView.FindViewById<Throbber>(Resource.Id.roomsrefreshthrobber);
            ViewSwitcher roomSourceSwitcher = iRootView.FindViewById<ViewSwitcher>(Resource.Id.roomandsourceviewswitcher);
            ToggleButton standbyButton = iRootView.FindViewById<ToggleButton>(Resource.Id.sourcesstandbybutton);
            Button backButton = iRootView.FindViewById<Button>(Resource.Id.sourcelistbackbutton);
            iRoomSourceMediator = new RoomSourceListsMediator(iStack,
                                                              iStack,
                                                              roomList,
                                                              sourceList,
                                                              refreshButton,
                                                              refreshThrobber,
                                                              roomSourceSwitcher,
                                                              standbyButton,
                                                              backButton,
                                                              iRootView.FindViewById<TextView>(Resource.Id.sourcelisttitle),
                                                              iIconResolver,
                                                              iViewMaster.RoomSelector,
                                                              iViewMaster.SourceSelector,
                                                              iViewPager,
                                                              iRootView.FindViewById<ToggleButton>(Resource.Id.standbybuttonall));
            iViewMaster.ViewWidgetButtonStandby.Button = standbyButton;

            // transport controls
            iViewMaster.ViewWidgetTransportControl.TransportControls = iRootView.FindViewById<TransportControls>(Resource.Id.transportcontrols);

            // volume control
            DisplayControl volumeDisplay = iRootView.FindViewById<DisplayControl>(Resource.Id.volumedisplay);
            iViewMaster.ViewWidgetVolumeControl.DisplayControl = volumeDisplay;
            iViewMaster.ViewWidgetVolumeControl.PopupAnchor = iRootView.FindViewById(Resource.Id.playlisttopsectionphone);

            // media time control
            DisplayControl mediaTimeDisplay = iRootView.FindViewById<DisplayControl>(Resource.Id.timedisplay);
            iViewMaster.ViewWidgetMediaTime.DisplayControl = mediaTimeDisplay;
            iViewMaster.ViewWidgetMediaTime.PopupAnchor = iRootView.FindViewById(Resource.Id.playlisttopsectionphone);

            // popup controls
            SetPopupControlFactories();

            // playlist control
            iViewMaster.ViewWidgetPlaylist.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iPlaylistMediaRenderer = new ListView(iStack);
            iPlaylistMediaRenderer.DividerHeight = 0;

            iViewMaster.ViewWidgetPlaylist.PlaylistView = iPlaylistMediaRenderer;
            iViewMaster.ViewWidgetPlaylist.EditButton = iRootView.FindViewById<ToggleButton>(Resource.Id.playlisteditmodebutton);
            iViewMaster.ViewWidgetPlaylist.SaveButton = iRootView.FindViewById<Button>(Resource.Id.playlistsavebutton);
            iViewMaster.ViewWidgetPlaylist.DeleteButton = iRootView.FindViewById<ImageButton>(Resource.Id.playlistdeletebutton);
            iViewMaster.ViewWidgetPlaylist.ButtonContainer = iRootView.FindViewById<ViewGroup>(Resource.Id.playlistbuttons);
            iViewMaster.ViewWidgetPlaylist.ScrollToButton = iRootView.FindViewById<View>(Resource.Id.trackdisplaycontainer);

            iViewMaster.ViewWidgetPlaylistReceiver.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iViewMaster.ViewWidgetPlaylistReceiver.ScrollToButton = iRootView.FindViewById<View>(Resource.Id.trackdisplaycontainer);
            iPlaylistReceiver = new ListView(iStack);
            iPlaylistReceiver.DividerHeight = 0;

            iViewMaster.ViewWidgetPlaylistReceiver.PlaylistView = iPlaylistReceiver;

            iViewMaster.ViewWidgetPlaylistRadio.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iViewMaster.ViewWidgetPlaylistRadio.ScrollToButton = iRootView.FindViewById<View>(Resource.Id.trackdisplaycontainer);
            iPlaylistRadio = new ListView(iStack);
            iPlaylistRadio.DividerHeight = 0;

            iViewMaster.ViewWidgetPlaylistRadio.PlaylistView = iPlaylistRadio;

            iViewMaster.ViewWidgetPlaylistDiscPlayer.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);

            iViewMaster.ViewWidgetPlaylistAux.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);


            // track control
            iViewMaster.ViewWidgetTrack.Display1 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay1);
            iViewMaster.ViewWidgetTrack.Display2 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay2);
            iViewMaster.ViewWidgetTrack.Display3 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay3);

            iViewMaster.ViewWidgetTrack.ImageView = iRootView.FindViewById<LazyLoadingImageView>(Resource.Id.trackartwork);

            iViewMaster.ViewWidgetTrackFullscreen.ImageView = iRootView.FindViewById<LazyLoadingImageView>(Resource.Id.trackartworkfullscreen);

            // save/delete buttons
            iViewMaster.ViewWidgetButtonSave.Button = iRootView.FindViewById<View>(Resource.Id.playlistsavebutton);
            iViewMaster.ViewWidgetButtonWasteBin.Button = iRootView.FindViewById<View>(Resource.Id.playlistdeletebutton);

            // repeat/shuffle buttons
            iViewMaster.ViewWidgetPlayMode.RepeatButton = iRootView.FindViewById<ToggleButton>(Resource.Id.repeatbutton);
            iViewMaster.ViewWidgetPlayMode.ShuffleButton = iRootView.FindViewById<ToggleButton>(Resource.Id.shufflebutton);

            iLargeArtworkMediator = new PhoneArtworkMediator(
                    iRootView.FindViewById(Resource.Id.fullscreenartworkcontainerphone),
                    iRootView.FindViewById(Resource.Id.playlistcontainerphone),
                    iRootView.FindViewById(Resource.Id.trackartwork),
                    iRootView.FindViewById(Resource.Id.trackshowplaylistbutton),
                    iViewMaster.IsShowingLargeArtwork);

            iLargeArtworkMediator.EventViewStateChanged += EventFullscreenChangedHandler;

            // go to room selection page if no room selected
            iTimer = new System.Threading.Timer(TimerCallback);
            iTimer.Change(5000, Timeout.Infinite);

            double containerWidth = iStack.Resources.DisplayMetrics.WidthPixels;
            double width = Math.Min(containerWidth, 600);

            new ControlsLayout(iRootView.FindViewById<RelativeLayout>(Resource.Id.volumeandtransportcontrolscontainer),
                               iRootView.FindViewById<RelativeLayout>(Resource.Id.volumeandtransportcontrols),
                               iRootView.FindViewById<TransportControls>(Resource.Id.transportcontrols),
                               iRootView.FindViewById<DisplayControl>(Resource.Id.timedisplay),
                               iRootView.FindViewById<DisplayControl>(Resource.Id.volumedisplay))
            .Layout(containerWidth, width);

            new ToolbarLayoutPhone(iRootView.FindViewById<ViewGroup>(Resource.Id.trackartworkcontainer),
                                   iRootView.FindViewById<ViewGroup>(Resource.Id.trackcontrols),
                                   iRootView.FindViewById<ViewGroup>(Resource.Id.roomlisttitlebar),
                                   iRootView.FindViewById<ViewGroup>(Resource.Id.sourcelisttitlebar),
                                   iRootView.FindViewById<ViewGroup>(Resource.Id.browsercontrolscontainer))
            .Layout(iStack.Resources.DisplayMetrics.HeightPixels);
        }

        private void TimerCallback(object aState)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                if (iOpen && iViewMaster.RoomSelector.SelectedItem == null && iCurrentPage != 0)
                {
                    iViewPager.CurrentItem = 0;
                }
            }));
        }

        protected override void OnClosed()
        {
            iStack.EventAutoLockChanged -= EventAutoLockChangedHandler;
            iStack.OptionEnableRocker.EventValueChanged -= OptionEnableRockerEventValueChangedHandler;

            iBrowser.EventLocationChanged -= EventLocationChangedHandler;
            iBrowser.Close();
            (iRootView.FindViewById(Resource.Id.browser) as RelativeLayout).RemoveView(iBrowser);
            iBrowser.Dispose();
            iBrowser = null;


            iRoomSourceMediator.Close();
            iViewMaster.ViewWidgetButtonStandby.Button = null;

            if (iSavePlaylistDialog != null)
            {
                iSavePlaylistDialog.Dismiss();
            }
            iViewMaster.ViewWidgetTransportControl.TransportControls = null;
            iViewMaster.ViewWidgetVolumeControl.DisplayControl = null;
            iViewMaster.ViewWidgetVolumeControl.PopupAnchor = null;
            iViewMaster.ViewWidgetMediaTime.DisplayControl = null;
            iViewMaster.ViewWidgetMediaTime.PopupAnchor = null;


            iViewMaster.ViewWidgetPlaylist.ContainerView = null;
            iViewMaster.ViewWidgetPlaylist.PlaylistView = null;
            iViewMaster.ViewWidgetPlaylist.EditButton = null;
            iViewMaster.ViewWidgetPlaylist.SaveButton = null;
            iViewMaster.ViewWidgetPlaylist.DeleteButton = null;
            iViewMaster.ViewWidgetPlaylist.ButtonContainer = null;
            iViewMaster.ViewWidgetPlaylistRadio.ContainerView = null;
            iViewMaster.ViewWidgetPlaylistRadio.EditButton = null;
            iViewMaster.ViewWidgetPlaylistRadio.SaveButton = null;
            iViewMaster.ViewWidgetPlaylistRadio.PlaylistView = null;
            iViewMaster.ViewWidgetPlaylistRadio.ButtonContainer = null;
            iViewMaster.ViewWidgetPlaylistDiscPlayer.ContainerView = null;
            iViewMaster.ViewWidgetPlaylistReceiver.ContainerView = null;
            iViewMaster.ViewWidgetPlaylistReceiver.EditButton = null;
            iViewMaster.ViewWidgetPlaylistReceiver.SaveButton = null;
            iViewMaster.ViewWidgetPlaylistReceiver.PlaylistView = null;
            iViewMaster.ViewWidgetPlaylistReceiver.ButtonContainer = null;
            iViewMaster.ViewWidgetPlaylistAux.ContainerView = null;

            iPlaylistMediaRenderer.Dispose();
            iPlaylistMediaRenderer = null;
            iPlaylistRadio.Dispose();
            iPlaylistRadio = null;
            iPlaylistReceiver.Dispose();
            iPlaylistReceiver = null;


            iViewMaster.ViewWidgetTrack.Display1 = null;
            iViewMaster.ViewWidgetTrack.Display2 = null;
            iViewMaster.ViewWidgetTrack.Display3 = null;
            iViewMaster.ViewWidgetTrack.TechnicalInfo = null;
            iViewMaster.ViewWidgetTrack.ImageView = null;
            iViewMaster.ViewWidgetTrackFullscreen.Display1 = null;
            iViewMaster.ViewWidgetTrackFullscreen.Display2 = null;
            iViewMaster.ViewWidgetTrackFullscreen.Display3 = null;
            iViewMaster.ViewWidgetTrackFullscreen.TechnicalInfo = null;
            iViewMaster.ViewWidgetTrackFullscreen.ImageView = null;
            iViewMaster.ViewWidgetButtonSave.Button = null;
            iViewMaster.ViewWidgetButtonWasteBin.Button = null;
            iViewMaster.ViewWidgetPlayMode.RepeatButton = null;
            iViewMaster.ViewWidgetPlayMode.ShuffleButton = null;
            iLargeArtworkMediator.EventViewStateChanged -= EventFullscreenChangedHandler;
            iLargeArtworkMediator.Dispose();

            ListView roomList = iRootView.FindViewById<ListView>(Resource.Id.roomlist);
            ListView sourceList = iRootView.FindViewById<ListView>(Resource.Id.sourcelist);
            roomList.Visibility = ViewStates.Gone;
            sourceList.Visibility = ViewStates.Gone;
            iOptionsMediator.SettingsButton = null;
            iOptionsMediator.PopupAnchor = null;
            iOptionsMediator.Dispose();
            iOptionsMediator = null;
            iTimer.Change(Timeout.Infinite, Timeout.Infinite);
            iTimer.Dispose();
        }

        private void EventLocationChangedHandler(object sender, EventArgsLocation e)
        {
            iStack.CurrentLocation = e.Location.BreadcrumbTrail;
        }

        private void EventAutoLockChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                if (iOpen && iRootView != null)
                {
                    iRootView.KeepScreenOn = iStack.AutoLock;
                }
            }));
        }

        private void SavePlaylistDialog_EventDismissedHandler(object sender, EventArgs e)
        {
            if (iSavePlaylistDialog != null)
            {
                iSavePlaylistDialog.EventDismissed -= SavePlaylistDialog_EventDismissedHandler;
                iSavePlaylistDialog = null;
            }
        }

        private void EventFullscreenChangedHandler(object sender, EventArgs e)
        {
            iViewMaster.IsShowingLargeArtwork = iLargeArtworkMediator.IsShowingLargeArtwork;
        }

        private void OptionEnableRockerEventValueChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                SetPopupControlFactories();
            }));
        }

        private void SetPopupControlFactories()
        {
            bool showRotary = !iStack.OptionEnableRocker.Native;

            // volume control
            iViewMaster.ViewWidgetVolumeControl.PopupControlFactory = showRotary ?
                (IPopupControlFactory)new RotaryControlFactory(iPopupFactory,
                    iResourceManager.GetBitmap(Resource.Drawable.MuteActive),
                    iResourceManager.GetBitmap(Resource.Drawable.Mute)) :
                    (IPopupControlFactory)new ButtonControlFactory(iPopupFactory,
                        iResourceManager.GetBitmap(Resource.Drawable.VolumeDown),
                        iResourceManager.GetBitmap(Resource.Drawable.VolumeUp),
                        iResourceManager.GetBitmap(Resource.Drawable.MuteIcon),
                        kTimerInitialDelay,
                        kVolumeTimerInterval,
                        iStack.Invoker);

            // media time control
            iViewMaster.ViewWidgetMediaTime.PopupControlFactory = showRotary ?
                (IPopupControlFactory)new RotaryControlFactory(iPopupFactory,
                    iResourceManager.GetBitmap(Resource.Drawable.ClockIconElapsed),
                    iResourceManager.GetBitmap(Resource.Drawable.ClockIconRemaining)) :
                    (IPopupControlFactory)new ButtonControlFactory(iPopupFactory,
                        iResourceManager.GetBitmap(Resource.Drawable.FrwdButton),
                        iResourceManager.GetBitmap(Resource.Drawable.FfwdButton),
                        iResourceManager.GetBitmap(Resource.Drawable.ClockIcon),
                        kTimerInitialDelay,
                        kSeekTimerInterval,
                        iStack.Invoker);
        }

        public override bool OnKeyUp(Keycode aKeyCode, KeyEvent e)
        {
            if (aKeyCode == Keycode.Menu && iOpen)
            {
                iOptionsMediator.ToggleOptions();
            }
            else if (aKeyCode == Keycode.Back && iOpen)
            {
                if (iCurrentPage == EPageIndex.Browser && iBrowser.CanGoUp())
                {
                    iBrowser.Up(1);
                    return true;
                }
                else if (iCurrentPage == EPageIndex.RoomSource && !iRoomSourceMediator.IsShowingRooms)
                {
                    iRoomSourceMediator.IsShowingRooms = true;
                    return true;
                }
            }
            return base.OnKeyUp(aKeyCode, e);
        }

        private ViewWidgetBrowser iBrowser;
        private OptionsMediator iOptionsMediator;
        private IPopupFactory iPopupFactory;
        private IPopupFactory iBrowserPopupFactory;
        private SavePlaylistDialog iSavePlaylistDialog;
        private RoomSourceListsMediator iRoomSourceMediator;
        private ILargeArtworkMediator iLargeArtworkMediator;
        private ListView iPlaylistMediaRenderer;
        private ListView iPlaylistRadio;
        private ListView iPlaylistReceiver;
        private View iRootView;
        private ViewPager iViewPager;
        private ViewGroup iHiddenContainer;
        private EPageIndex iCurrentPage;
        private PhonePagerListener iPhonePagerListener;
        private ViewGroup iPlaylistContainer;
        private MultiViewPageIndicator iPageIndicator;

        private const int kMaxImageCacheSize = 1 * 1024 * 1024;
        private System.Threading.Timer iTimer;
    }

    public enum EPageIndex
    {
        RoomSource = 0,
        NowPlaying = 1,
        Browser = 2
    }

    public class ViewKinskyTablet : ViewKinsky
    {
        public ViewKinskyTablet(Stack aStack, Activity aActivity, AndroidViewMaster aViewMaster, AndroidResourceManager aResourceManager, IconResolver aIconResolver)
            : base(aStack, aActivity, aViewMaster, aResourceManager, aIconResolver)
        {
            iBrowserPopupFactory = new SpeechBubblePopupFactory(aStack, Color.Black);
            iPopupFactory = new SpeechBubblePopupFactory(aStack, Color.Black);
            Init(aActivity);
        }

        protected override void SavePlaylist(ISaveSupport aSaveSupport)
        {
            if (!PopupManager.IsShowingPopup && iOpen)
            {
                View saveButton = iRootView.FindViewById(Resource.Id.playlistsavebutton);
                View popupAnchor = saveButton;
                iSavePlaylistDialog = new SavePlaylistDialog(iStack, iPopupFactory, saveButton, popupAnchor, Color.Black, iStack.SaveSupport, iViewMaster.ImageCache, iIconResolver, false);
                iSavePlaylistDialog.EventDismissed += SavePlaylistDialog_EventDismissedHandler;
            }
        }

        protected override void Init(Activity aActivity)
        {
            Assert.Check(!iStack.Invoker.InvokeRequired);
            aActivity.RequestedOrientation = ScreenOrientation.Unspecified;
            aActivity.SetContentView(Resource.Layout.MainTablet);
            iRootView = aActivity.FindViewById(Resource.Id.rootview);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (iRootView != null)
            {
                iRootView.FindViewById(Resource.Id.settingsbutton).Dispose();
                iRootView.FindViewById(Resource.Id.browsereditmodebutton).Dispose();
                iRootView.FindViewById(Resource.Id.browserthrobber).Dispose();
                iRootView.FindViewById(Resource.Id.backbutton).Dispose();
                iRootView.FindViewById(Resource.Id.locationdisplay).Dispose();
                iRootView.FindViewById(Resource.Id.playnownextlaterbutton).Dispose();
                iRootView.FindViewById(Resource.Id.browser).Dispose();
                iRootView.FindViewById(Resource.Id.selectroombutton).Dispose();
                iRootView.FindViewById(Resource.Id.selectsourcebutton).Dispose();
                iRootView.FindViewById(Resource.Id.transportcontrols).Dispose();
                iRootView.FindViewById(Resource.Id.volumedisplay).Dispose();
                iRootView.FindViewById(Resource.Id.timedisplay).Dispose();
                iRootView.FindViewById(Resource.Id.playlist).Dispose();
                iRootView.FindViewById(Resource.Id.playlisteditmodebutton).Dispose();
                iRootView.FindViewById(Resource.Id.playlistsavebutton).Dispose();
                iRootView.FindViewById(Resource.Id.playlistdeletebutton).Dispose();
                iRootView.FindViewById(Resource.Id.playlistbuttons).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay1).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay2).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay3).Dispose();
                iRootView.FindViewById(Resource.Id.tracktechnicalinfo).Dispose();
                iRootView.FindViewById(Resource.Id.trackartwork).Dispose();
                iRootView.FindViewById(Resource.Id.trackartworkfullscreen).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay1fullscreen).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay2fullscreen).Dispose();
                iRootView.FindViewById(Resource.Id.trackdisplay3fullscreen).Dispose();
                iRootView.FindViewById(Resource.Id.tracktechnicalinfofullscreen).Dispose();
                iRootView.FindViewById(Resource.Id.shufflebutton).Dispose();
                iRootView.FindViewById(Resource.Id.repeatbutton).Dispose();
                iRootView.Dispose();
                iRootView = null;
            }
        }

        protected override void OnOpened()
        {
            iOptionsMediator = new OptionsMediator(iPopupFactory, iStack, iViewMaster.ImageCache, iIconResolver, false);
            iOptionsMediator.PopupAnchor = iRootView.FindViewById(Resource.Id.settingsbutton);

            iRootView.KeepScreenOn = iStack.AutoLock;

            iStack.EventAutoLockChanged += EventAutoLockChangedHandler;

            iOptionsMediator.SettingsButton = iRootView.FindViewById(Resource.Id.settingsbutton);
            iOptionsMediator.SettingsButtonBadge = iRootView.FindViewById(Resource.Id.settingsbuttonbadge);

            iStack.OptionEnableRocker.EventValueChanged += OptionEnableRockerEventValueChangedHandler;

            // browser
            ToggleButton editButton = iRootView.FindViewById<ToggleButton>(Resource.Id.browsereditmodebutton);
            Throbber throbber = iRootView.FindViewById<Throbber>(Resource.Id.browserthrobber);
            iBrowser = new ViewWidgetBrowser(iStack,
                                             iStack.RootLocation,
                                             iStack.Invoker,
                                             iViewMaster.ImageCache,
                                             iIconResolver,
                                             iRootView.FindViewById<Button>(Resource.Id.backbutton),
                                             iRootView.FindViewById<TextView>(Resource.Id.locationdisplay),
                                             editButton,
                                             iViewMaster.FlingStateManager,
                                             throbber,
                                             iRootView.FindViewById<Button>(Resource.Id.playnownextlaterbutton),
                                             iStack.PlaySupport,
                                             iStack.OptionInsertMode,
                                             iBrowserPopupFactory);
            iBrowser.EventLocationChanged += EventLocationChangedHandler;
            iBrowser.Navigate(iStack.CurrentLocation, 5);
            (iRootView.FindViewById(Resource.Id.browser) as RelativeLayout).AddView(iBrowser);


            // room/source selection
            Button selectRoom = iRootView.FindViewById<Button>(Resource.Id.selectroombutton);
            Button selectSource = iRootView.FindViewById<Button>(Resource.Id.selectsourcebutton);
            iRoomSourceMediator = new RoomSourcePopupsMediator(iStack, iStack, selectRoom, selectSource, iPopupFactory, iIconResolver, iViewMaster.RoomSelector, iViewMaster.SourceSelector);
            // transport controls
            iViewMaster.ViewWidgetTransportControl.TransportControls = iRootView.FindViewById<TransportControls>(Resource.Id.transportcontrols);

            // volume control
            DisplayControl volumeDisplay = iRootView.FindViewById<DisplayControl>(Resource.Id.volumedisplay);
            iViewMaster.ViewWidgetVolumeControl.DisplayControl = volumeDisplay;

            iViewMaster.ViewWidgetVolumeControl.PopupAnchor = volumeDisplay;

            // media time control
            DisplayControl mediaTimeDisplay = iRootView.FindViewById<DisplayControl>(Resource.Id.timedisplay);
            iViewMaster.ViewWidgetMediaTime.DisplayControl = mediaTimeDisplay;
            iViewMaster.ViewWidgetMediaTime.PopupAnchor = mediaTimeDisplay;

            // popup controls
            SetPopupControlFactories();

            // playlist control
            iViewMaster.ViewWidgetPlaylist.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iPlaylistMediaRenderer = new ListView(iStack);
            iPlaylistMediaRenderer.DividerHeight = 0;
            iViewMaster.ViewWidgetPlaylist.PlaylistView = iPlaylistMediaRenderer;
            iViewMaster.ViewWidgetPlaylist.EditButton = iRootView.FindViewById<ToggleButton>(Resource.Id.playlisteditmodebutton);
            iViewMaster.ViewWidgetPlaylist.SaveButton = iRootView.FindViewById<Button>(Resource.Id.playlistsavebutton);
            iViewMaster.ViewWidgetPlaylist.DeleteButton = iRootView.FindViewById<ImageButton>(Resource.Id.playlistdeletebutton);
            iViewMaster.ViewWidgetPlaylist.ButtonContainer = iRootView.FindViewById<ViewGroup>(Resource.Id.playlistbuttons);
            iViewMaster.ViewWidgetPlaylist.ScrollToButton = iRootView.FindViewById<View>(Resource.Id.trackdisplaycontainer);

            iViewMaster.ViewWidgetPlaylistReceiver.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iViewMaster.ViewWidgetPlaylistReceiver.ScrollToButton = iRootView.FindViewById<View>(Resource.Id.trackdisplaycontainer);
            iPlaylistReceiver = new ListView(iStack);
            iPlaylistReceiver.DividerHeight = 0;
            iViewMaster.ViewWidgetPlaylistReceiver.PlaylistView = iPlaylistReceiver;

            iViewMaster.ViewWidgetPlaylistRadio.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);
            iViewMaster.ViewWidgetPlaylistRadio.ScrollToButton = iRootView.FindViewById<View>(Resource.Id.trackdisplaycontainer);
            iPlaylistRadio = new ListView(iStack);
            iPlaylistRadio.DividerHeight = 0;
            iViewMaster.ViewWidgetPlaylistRadio.PlaylistView = iPlaylistRadio;

            iViewMaster.ViewWidgetPlaylistDiscPlayer.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);

            iViewMaster.ViewWidgetPlaylistAux.ContainerView = iRootView.FindViewById<RelativeLayout>(Resource.Id.playlist);


            // track control
            iViewMaster.ViewWidgetTrack.Display1 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay1);
            iViewMaster.ViewWidgetTrack.Display2 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay2);
            iViewMaster.ViewWidgetTrack.Display3 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay3);
            iViewMaster.ViewWidgetTrack.TechnicalInfo = iRootView.FindViewById<TextView>(Resource.Id.tracktechnicalinfo);
            iViewMaster.ViewWidgetTrack.ImageView = iRootView.FindViewById<LazyLoadingImageView>(Resource.Id.trackartwork);

            iViewMaster.ViewWidgetTrackFullscreen.Display1 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay1fullscreen);
            iViewMaster.ViewWidgetTrackFullscreen.Display2 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay2fullscreen);
            iViewMaster.ViewWidgetTrackFullscreen.Display3 = iRootView.FindViewById<TextView>(Resource.Id.trackdisplay3fullscreen);
            iViewMaster.ViewWidgetTrackFullscreen.TechnicalInfo = iRootView.FindViewById<TextView>(Resource.Id.tracktechnicalinfofullscreen);
            iViewMaster.ViewWidgetTrackFullscreen.ImageView = iRootView.FindViewById<LazyLoadingImageView>(Resource.Id.trackartworkfullscreen);

            // save/delete buttons
            iViewMaster.ViewWidgetButtonSave.Button = iRootView.FindViewById<View>(Resource.Id.playlistsavebutton);
            iViewMaster.ViewWidgetButtonWasteBin.Button = iRootView.FindViewById<View>(Resource.Id.playlistdeletebutton);

            // repeat/shuffle buttons
            iViewMaster.ViewWidgetPlayMode.RepeatButton = iRootView.FindViewById<ToggleButton>(Resource.Id.repeatbutton);
            iViewMaster.ViewWidgetPlayMode.ShuffleButton = iRootView.FindViewById<ToggleButton>(Resource.Id.shufflebutton);

            // setup full screen animations mediator
            iLargeArtworkMediator = new FullscreenArtworkMediator(
                iRootView.FindViewById(Resource.Id.trackcontrolsfullscreen),
                iRootView.FindViewById(Resource.Id.mainlayout),
                iRootView.FindViewById(Resource.Id.trackartwork),
                iRootView.FindViewById(Resource.Id.trackcontrolsfullscreen),
                iViewMaster.IsShowingLargeArtwork);
            iLargeArtworkMediator.EventViewStateChanged += EventFullscreenChangedHandler;

            double containerWidth = Math.Min(iStack.Resources.DisplayMetrics.WidthPixels, iStack.Resources.DisplayMetrics.HeightPixels) / 2;
            double width = containerWidth;
            new ControlsLayout(iRootView.FindViewById<RelativeLayout>(Resource.Id.volumeandtransportcontrolscontainer),
                               iRootView.FindViewById<RelativeLayout>(Resource.Id.volumeandtransportcontrols),
                               iRootView.FindViewById<TransportControls>(Resource.Id.transportcontrols),
                               iRootView.FindViewById<DisplayControl>(Resource.Id.timedisplay),
                               iRootView.FindViewById<DisplayControl>(Resource.Id.volumedisplay))
            .Layout(containerWidth, width);
        }

        protected override void OnClosed()
        {
            iStack.EventAutoLockChanged -= EventAutoLockChangedHandler;
            iStack.OptionEnableRocker.EventValueChanged -= OptionEnableRockerEventValueChangedHandler;
            iBrowser.EventLocationChanged -= EventLocationChangedHandler;
            iBrowser.Close();
            (iRootView.FindViewById(Resource.Id.browser) as RelativeLayout).RemoveView(iBrowser);
            iBrowser.Dispose();
            iBrowser = null;

            iRoomSourceMediator.Close();
            iViewMaster.ViewWidgetButtonStandby.Button = null;

            if (iSavePlaylistDialog != null)
            {
                iSavePlaylistDialog.Dismiss();
            }
            iViewMaster.ViewWidgetTransportControl.TransportControls = null;
            iViewMaster.ViewWidgetVolumeControl.DisplayControl = null;
            iViewMaster.ViewWidgetVolumeControl.PopupAnchor = null;
            iViewMaster.ViewWidgetMediaTime.DisplayControl = null;
            iViewMaster.ViewWidgetMediaTime.PopupAnchor = null;


            iViewMaster.ViewWidgetPlaylist.ContainerView = null;
            iViewMaster.ViewWidgetPlaylist.PlaylistView = null;
            iViewMaster.ViewWidgetPlaylist.EditButton = null;
            iViewMaster.ViewWidgetPlaylist.SaveButton = null;
            iViewMaster.ViewWidgetPlaylist.DeleteButton = null;
            iViewMaster.ViewWidgetPlaylist.ButtonContainer = null;
            iViewMaster.ViewWidgetPlaylistRadio.ContainerView = null;
            iViewMaster.ViewWidgetPlaylistRadio.EditButton = null;
            iViewMaster.ViewWidgetPlaylistRadio.SaveButton = null;
            iViewMaster.ViewWidgetPlaylistRadio.PlaylistView = null;
            iViewMaster.ViewWidgetPlaylistRadio.ButtonContainer = null;
            iViewMaster.ViewWidgetPlaylistDiscPlayer.ContainerView = null;
            iViewMaster.ViewWidgetPlaylistReceiver.ContainerView = null;
            iViewMaster.ViewWidgetPlaylistReceiver.EditButton = null;
            iViewMaster.ViewWidgetPlaylistReceiver.SaveButton = null;
            iViewMaster.ViewWidgetPlaylistReceiver.PlaylistView = null;
            iViewMaster.ViewWidgetPlaylistReceiver.ButtonContainer = null;
            iViewMaster.ViewWidgetPlaylistAux.ContainerView = null;

            iPlaylistMediaRenderer.Dispose();
            iPlaylistMediaRenderer = null;
            iPlaylistRadio.Dispose();
            iPlaylistRadio = null;
            iPlaylistReceiver.Dispose();
            iPlaylistReceiver = null;


            iViewMaster.ViewWidgetTrack.Display1 = null;
            iViewMaster.ViewWidgetTrack.Display2 = null;
            iViewMaster.ViewWidgetTrack.Display3 = null;
            iViewMaster.ViewWidgetTrack.TechnicalInfo = null;
            iViewMaster.ViewWidgetTrack.ImageView = null;
            iViewMaster.ViewWidgetTrackFullscreen.Display1 = null;
            iViewMaster.ViewWidgetTrackFullscreen.Display2 = null;
            iViewMaster.ViewWidgetTrackFullscreen.Display3 = null;
            iViewMaster.ViewWidgetTrackFullscreen.TechnicalInfo = null;
            iViewMaster.ViewWidgetTrackFullscreen.ImageView = null;
            iViewMaster.ViewWidgetButtonSave.Button = null;
            iViewMaster.ViewWidgetButtonWasteBin.Button = null;
            iViewMaster.ViewWidgetPlayMode.RepeatButton = null;
            iViewMaster.ViewWidgetPlayMode.ShuffleButton = null;
            iLargeArtworkMediator.EventViewStateChanged -= EventFullscreenChangedHandler;
            iLargeArtworkMediator.Dispose();

            iOptionsMediator.SettingsButton = null;
            iOptionsMediator.PopupAnchor = null;
            iOptionsMediator.Dispose();
            iOptionsMediator = null;
        }

        private void EventLocationChangedHandler(object sender, EventArgsLocation e)
        {
            iStack.CurrentLocation = e.Location.BreadcrumbTrail;
        }

        private void EventAutoLockChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                if (iOpen && iRootView != null)
                {
                    iRootView.KeepScreenOn = iStack.AutoLock;
                }
            }));
        }

        private void SavePlaylistDialog_EventDismissedHandler(object sender, EventArgs e)
        {
            if (iSavePlaylistDialog != null)
            {
                iSavePlaylistDialog.EventDismissed -= SavePlaylistDialog_EventDismissedHandler;
                iSavePlaylistDialog = null;
            }
        }

        private void EventFullscreenChangedHandler(object sender, EventArgs e)
        {
            iViewMaster.IsShowingLargeArtwork = iLargeArtworkMediator.IsShowingLargeArtwork;
        }

        private void OptionEnableRockerEventValueChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                SetPopupControlFactories();
            }));
        }

        private void SetPopupControlFactories()
        {
            bool showRotary = !iStack.OptionEnableRocker.Native;

            // volume control
            iViewMaster.ViewWidgetVolumeControl.PopupControlFactory = showRotary ?
                (IPopupControlFactory)new RotaryControlFactory(iPopupFactory,
                    iResourceManager.GetBitmap(Resource.Drawable.MuteActive),
                    iResourceManager.GetBitmap(Resource.Drawable.Mute)) :
                    (IPopupControlFactory)new ButtonControlFactory(iPopupFactory,
                        iResourceManager.GetBitmap(Resource.Drawable.VolumeDown),
                        iResourceManager.GetBitmap(Resource.Drawable.VolumeUp),
                        iResourceManager.GetBitmap(Resource.Drawable.MuteIcon),
                        kTimerInitialDelay,
                        kVolumeTimerInterval,
                        iStack.Invoker);

            // media time control
            iViewMaster.ViewWidgetMediaTime.PopupControlFactory = showRotary ?
                (IPopupControlFactory)new RotaryControlFactory(iPopupFactory,
                    iResourceManager.GetBitmap(Resource.Drawable.ClockIconElapsed),
                    iResourceManager.GetBitmap(Resource.Drawable.ClockIconRemaining)) :
                    (IPopupControlFactory)new ButtonControlFactory(iPopupFactory,
                        iResourceManager.GetBitmap(Resource.Drawable.FrwdButton),
                        iResourceManager.GetBitmap(Resource.Drawable.FfwdButton),
                        iResourceManager.GetBitmap(Resource.Drawable.ClockIcon),
                        kTimerInitialDelay,
                        kSeekTimerInterval,
                        iStack.Invoker);
        }

        public override bool OnKeyUp(Keycode aKeyCode, KeyEvent e)
        {
            if (aKeyCode == Keycode.Menu && iOpen)
            {
                iOptionsMediator.ToggleOptions();
            }
            else if (aKeyCode == Keycode.Back && iOpen)
            {
                if (iBrowser.CanGoUp())
                {
                    iBrowser.Up(1);
                    return true;
                }
            }
            return base.OnKeyUp(aKeyCode, e);
        }

        private ViewWidgetBrowser iBrowser;
        private OptionsMediator iOptionsMediator;
        private IPopupFactory iPopupFactory;
        private IPopupFactory iBrowserPopupFactory;
        private SavePlaylistDialog iSavePlaylistDialog;
        private IRoomSourceMediator iRoomSourceMediator;
        private ILargeArtworkMediator iLargeArtworkMediator;
        private ListView iPlaylistMediaRenderer;
        private ListView iPlaylistRadio;
        private ListView iPlaylistReceiver;
        private View iRootView;
    }

    public abstract class ViewKinsky
    {
        public ViewKinsky(Stack aStack, Activity aActivity, AndroidViewMaster aViewMaster, AndroidResourceManager aResourceManager, IconResolver aIconResolver)
        {
            iStack = aStack;
            iViewMaster = aViewMaster;
            iResourceManager = aResourceManager;
            iIconResolver = aIconResolver;
        }

        public virtual void Dispose()
        {
            Assert.Check(!iStack.Invoker.InvokeRequired);
            Close();
        }

        protected abstract void Init(Activity aActivity);

        protected abstract void SavePlaylist(ISaveSupport aSaveSupport);

        protected abstract void OnOpened();

        protected abstract void OnClosed();

        public virtual bool OnKeyDown(Keycode aKeyCode, KeyEvent e)
        {

            if (aKeyCode == Keycode.VolumeDown && iOpen)
            {
                iViewMaster.ViewWidgetVolumeControl.DecrementVolume();
                return true;
            }
            else if (aKeyCode == Keycode.VolumeUp && iOpen)
            {
                iViewMaster.ViewWidgetVolumeControl.IncrementVolume();
                return true;
            }
            return false;
        }

        public virtual bool OnKeyUp(Keycode aKeyCode, KeyEvent e)
        {
            return false;
        }

        public void Open()
        {
            Assert.Check(!iStack.Invoker.InvokeRequired);
            if (!iOpen)
            {
                iViewMaster.EventPlaylistSave += EventPlaylistSaveHandler;
                OnOpened();
                iOpen = true;
            }
        }

        private void EventPlaylistSaveHandler(object sender, AndroidViewMaster.EventArgsPlaylistSave e)
        {
            SavePlaylist(e.SaveSupport);
        }

        public void Close()
        {
            Assert.Check(!iStack.Invoker.InvokeRequired);
            if (iOpen)
            {
                iViewMaster.EventPlaylistSave -= EventPlaylistSaveHandler;
                OnClosed();
                iOpen = false;
            }
        }

        protected bool iOpen;
        protected Stack iStack;
        protected AndroidViewMaster iViewMaster;
        protected AndroidResourceManager iResourceManager;
        protected IconResolver iIconResolver;

        protected const int kTimerInitialDelay = 250;
        protected const int kVolumeTimerInterval = 100;
        protected const int kSeekTimerInterval = 50;
    }

    public class AndroidViewMaster
    {

        public event EventHandler<EventArgsPlaylistSave> EventPlaylistSave;

        public AndroidViewMaster(Context aContext,
                                 ViewMaster aViewMaster,
                                 IInvoker aInvoker,
                                 AndroidResourceManager aResourceManager,
                                 SaveSupport aSaveSupport,
                                 IconResolver aIconResolver,
                                 OptionBool aOptionGroupTracks,
                                 OptionBool aOptionExtendedTrackInfo,
                                 int aMaxCacheSize)
        {
            iContext = aContext;
            iViewMaster = aViewMaster;
            iInvoker = aInvoker;
            iResourceManager = aResourceManager;
            iIconResolver = aIconResolver;
            iOptionGroupTracks = aOptionGroupTracks;
            iOptionExtendedTrackInfo = aOptionExtendedTrackInfo;
            iViewSaveSupport = new ViewSaveSupport(SavePlaylist, aSaveSupport);
            IImage<Bitmap> errorImage = new AndroidImageWrapper(iResourceManager.GetBitmap(Resource.Drawable.Loading));
            int imageSize = (int)iContext.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight) * 2;
            AndroidImageLoader loaderLowRes = new AndroidImageLoader(new ScalingUriConverter(imageSize, false, false));
            AndroidImageLoader loaderHighRes = new AndroidImageLoader(new ScalingUriConverter(kImageSizeHiRes, true, false));
            iFlingStateManager = new FlingStateManager();
            iImageCache = new AndroidImageCache(aMaxCacheSize, imageSize, 2, loaderLowRes, iInvoker, iFlingStateManager, kPendingImageCacheRequestLimit);
            iHighResImageCache = new AndroidImageCache(aMaxCacheSize, kImageSizeHiRes, 1, loaderHighRes, iInvoker, iFlingStateManager, kPendingImageCacheRequestLimit);
            iViewWidgetSelectorRoom = new ViewWidgetSelectorRoom();
            iViewMaster.ViewWidgetSelectorRoom.Add(iViewWidgetSelectorRoom);
            iViewWidgetSelectorSource = new ViewWidgetSelector<Linn.Kinsky.Source>();
            iViewMaster.ViewWidgetSelectorSource.Add(iViewWidgetSelectorSource);
            iViewWidgetTransportControl = new ViewWidgetTransportControl();
            iViewMaster.ViewWidgetTransportControlDiscPlayer.Add(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlMediaRenderer.Add(iViewWidgetTransportControl);
            iViewMaster.ViewWidgetTransportControlRadio.Add(iViewWidgetTransportControl);
            iViewWidgetVolumeControl = new ViewWidgetVolumeControl();
            iViewMaster.ViewWidgetVolumeControl.Add(iViewWidgetVolumeControl);
            iViewWidgetMediaTime = new ViewWidgetMediaTime();
            iViewMaster.ViewWidgetMediaTime.Add(iViewWidgetMediaTime);
            iViewWidgetPlaylist = new ViewWidgetPlaylist(iContext, iInvoker, iIconResolver, iImageCache, iFlingStateManager, iViewSaveSupport);
            iViewMaster.ViewWidgetPlaylist.Add(iViewWidgetPlaylist);
            iViewWidgetPlaylistRadio = new ViewWidgetPlaylistRadio(iContext, iInvoker, iIconResolver, iImageCache, iFlingStateManager, iViewSaveSupport);
            iViewMaster.ViewWidgetPlaylistRadio.Add(iViewWidgetPlaylistRadio);
            iViewWidgetPlaylistDiscPlayer = new ViewWidgetPlaylistDiscPlayer(iContext, kAuxSourceImageWidth);
            iViewMaster.ViewWidgetPlaylistDiscPlayer.Add(iViewWidgetPlaylistDiscPlayer);
            iViewWidgetPlaylistAux = new ViewWidgetPlaylistAux(iContext, Resource.Drawable.Source, kAuxSourceImageWidth);
            iViewMaster.ViewWidgetPlaylistAux.Add(iViewWidgetPlaylistAux);
            iViewWidgetPlaylistReceiver = new ViewWidgetPlaylistReceiver(iContext, iInvoker, iIconResolver, iImageCache, iFlingStateManager, iViewSaveSupport);
            iViewMaster.ViewWidgetPlaylistReceiver.Add(iViewWidgetPlaylistReceiver);
            iViewMaster.ViewWidgetSelectorRoom.Add(iViewWidgetPlaylistReceiver);
            iViewWidgetTrack = new ViewWidgetTrack(iInvoker, iResourceManager, iHighResImageCache);
            iViewWidgetTrackFullscreen = new ViewWidgetTrack(iInvoker, iResourceManager, iHighResImageCache);
            iViewMaster.ViewWidgetTrack.Add(iViewWidgetTrack);
            iViewMaster.ViewWidgetTrack.Add(iViewWidgetTrackFullscreen);
            iViewWidgetButtonSave = new ViewWidgetButton();
            iViewMaster.ViewWidgetButtonSave.Add(iViewWidgetButtonSave);
            iViewWidgetButtonWasteBin = new ViewWidgetButton();
            iViewMaster.ViewWidgetButtonWasteBin.Add(iViewWidgetButtonWasteBin);
            iViewWidgetPlayMode = new ViewWidgetPlayMode();
            iViewMaster.ViewWidgetPlayMode.Add(iViewWidgetPlayMode);
            iViewWidgetButtonStandby = new ViewWidgetButtonStandby();
            iViewMaster.ViewWidgetButtonStandby.Add(iViewWidgetButtonStandby);
            iOptionGroupTracks.EventValueChanged += OptionGroupTracks_EventValueChangedHandler;
            iOptionExtendedTrackInfo.EventValueChanged += OptionExtendedTrackInfo_EventValueChangedHandler;

            SetPlaylistGrouping(iOptionGroupTracks.Native);
            SetDisplayExtendedTrackInfo(iOptionExtendedTrackInfo.Native);
            CurrentPageIndex = EPageIndex.NowPlaying;
        }

        public bool IsShowingLargeArtwork { get; set; }
        public EPageIndex CurrentPageIndex { get; set; }

        public AndroidImageCache ImageCache
        {
            get
            {
                return iImageCache;
            }
        }

        public FlingStateManager FlingStateManager
        {
            get
            {
                return iFlingStateManager;
            }
        }

        private void SavePlaylist(ISaveSupport aSaveSupport)
        {
            OnEventPlaylistSave(aSaveSupport);
        }

        public void ClearCache()
        {
            iImageCache.Clear();
            iHighResImageCache.Clear();
        }

        private void SetDisplayExtendedTrackInfo(bool aDisplayExtendedTrackInfo)
        {
            iInvoker.BeginInvoke((Action)(() =>
            {
                iViewWidgetTrack.DisplayTechnicalInfo = aDisplayExtendedTrackInfo;
                iViewWidgetTrackFullscreen.DisplayTechnicalInfo = aDisplayExtendedTrackInfo;
            }));
        }

        private void SetPlaylistGrouping(bool aPlaylistGrouping)
        {
            iInvoker.BeginInvoke((Action)(() =>
            {
                iViewWidgetPlaylist.IsGrouping = aPlaylistGrouping;
            }));
        }

        private void OptionExtendedTrackInfo_EventValueChangedHandler(object sender, EventArgs e)
        {
            SetDisplayExtendedTrackInfo(iOptionExtendedTrackInfo.Native);
        }

        private void OptionGroupTracks_EventValueChangedHandler(object sender, EventArgs e)
        {
            SetPlaylistGrouping(iOptionGroupTracks.Native);
        }

        public ViewWidgetPlayMode ViewWidgetPlayMode
        {
            get
            {
                return iViewWidgetPlayMode;
            }
        }

        public ViewWidgetButton ViewWidgetButtonStandby
        {
            get
            {
                return iViewWidgetButtonStandby;
            }
        }

        public ViewWidgetButton ViewWidgetButtonWasteBin
        {
            get
            {
                return iViewWidgetButtonWasteBin;
            }
        }

        public ViewWidgetButton ViewWidgetButtonSave
        {
            get
            {
                return iViewWidgetButtonSave;
            }
        }

        public ViewWidgetTrack ViewWidgetTrackFullscreen
        {
            get
            {
                return iViewWidgetTrackFullscreen;
            }
        }

        public ViewWidgetTrack ViewWidgetTrack
        {
            get
            {
                return iViewWidgetTrack;
            }
        }

        public ViewWidgetPlaylist ViewWidgetPlaylist
        {
            get
            {
                return iViewWidgetPlaylist;
            }
        }

        public ViewWidgetPlaylistRadio ViewWidgetPlaylistRadio
        {
            get
            {
                return iViewWidgetPlaylistRadio;
            }
        }

        public ViewWidgetPlaylistAux ViewWidgetPlaylistAux
        {
            get
            {
                return iViewWidgetPlaylistAux;
            }
        }

        public ViewWidgetPlaylistDiscPlayer ViewWidgetPlaylistDiscPlayer
        {
            get
            {
                return iViewWidgetPlaylistDiscPlayer;
            }
        }

        public ViewWidgetPlaylistReceiver ViewWidgetPlaylistReceiver
        {
            get
            {
                return iViewWidgetPlaylistReceiver;
            }
        }

        public ViewWidgetVolumeControl ViewWidgetVolumeControl
        {
            get
            {
                return iViewWidgetVolumeControl;
            }
        }

        public ViewWidgetMediaTime ViewWidgetMediaTime
        {
            get
            {
                return iViewWidgetMediaTime;
            }
        }

        public ViewWidgetTransportControl ViewWidgetTransportControl
        {
            get
            {
                return iViewWidgetTransportControl;
            }
        }

        public ViewWidgetSelectorRoom RoomSelector
        {
            get
            {
                return iViewWidgetSelectorRoom;
            }
        }

        public ViewWidgetSelector<Linn.Kinsky.Source> SourceSelector
        {
            get
            {
                return iViewWidgetSelectorSource;
            }
        }

        private void OnEventPlaylistSave(ISaveSupport aSaveSupport)
        {
            EventHandler<EventArgsPlaylistSave> del = EventPlaylistSave;
            if (del != null)
            {
                del(this, new EventArgsPlaylistSave(aSaveSupport));
            }
        }

        private Context iContext;
        private IInvoker iInvoker;
        private ViewMaster iViewMaster;
        private OptionBool iOptionGroupTracks;
        private OptionBool iOptionExtendedTrackInfo;

        private const int kAuxSourceImageWidth = 96;
        private const int kImageSizeHiRes = 1024;
        private const int kImageCacheThreadCount = 2;
        private const int kPendingImageCacheRequestLimit = 50;
        protected AndroidImageCache iImageCache;
        private AndroidImageCache iHighResImageCache;
        protected AndroidResourceManager iResourceManager;
        private ViewSaveSupport iViewSaveSupport;
        protected FlingStateManager iFlingStateManager;
        protected IconResolver iIconResolver;

        private ViewWidgetSelectorRoom iViewWidgetSelectorRoom;
        private ViewWidgetSelector<Linn.Kinsky.Source> iViewWidgetSelectorSource;
        private ViewWidgetTransportControl iViewWidgetTransportControl;
        private ViewWidgetVolumeControl iViewWidgetVolumeControl;
        private ViewWidgetMediaTime iViewWidgetMediaTime;
        private ViewWidgetPlaylist iViewWidgetPlaylist;
        private ViewWidgetPlaylistRadio iViewWidgetPlaylistRadio;
        private ViewWidgetPlaylistDiscPlayer iViewWidgetPlaylistDiscPlayer;
        private ViewWidgetPlaylistReceiver iViewWidgetPlaylistReceiver;
        private ViewWidgetPlaylistAux iViewWidgetPlaylistAux;
        private ViewWidgetTrack iViewWidgetTrack;
        private ViewWidgetTrack iViewWidgetTrackFullscreen;
        private ViewWidgetButton iViewWidgetButtonWasteBin;
        private ViewWidgetButton iViewWidgetButtonSave;
        private ViewWidgetPlayMode iViewWidgetPlayMode;
        private ViewWidgetButtonStandby iViewWidgetButtonStandby;

        public class EventArgsPlaylistSave : EventArgs
        {
            public EventArgsPlaylistSave(ISaveSupport aSaveSupport)
                : base()
            {
                iSaveSupport = aSaveSupport;
            }

            public ISaveSupport SaveSupport
            {
                get
                {
                    return iSaveSupport;
                }
            }

            private ISaveSupport iSaveSupport;
        }
    }

    #region FullscreenArtworkMediator

    public interface ILargeArtworkMediator
    {
        void Dispose();
        event EventHandler<EventArgs> EventViewStateChanged;
        bool IsShowingLargeArtwork { get; set; }
    }

    public class PhoneArtworkMediator : ILargeArtworkMediator
    {
        public PhoneArtworkMediator(View aFullscreenView, View aMainView, View aShowFullscreenButton, View aHideFullscreenButton, bool aInitialState)
        {
            iFullscreenView = aFullscreenView;
            iMainView = aMainView;
            iShowFullscreenButton = aShowFullscreenButton;
            iHideFullscreenButton = aHideFullscreenButton;
            iHideFullscreenButton.Click += HideFullscreenButtonClick;
            iShowFullscreenButton.Click += ShowFullscreenButtonClick;
            IsShowingLargeArtwork = aInitialState;
        }

        public event EventHandler<EventArgs> EventViewStateChanged;

        public void Dispose()
        {
            iDisposed = true;
            iHideFullscreenButton.Click -= HideFullscreenButtonClick;
            iShowFullscreenButton.Click -= ShowFullscreenButtonClick;
            iShowFullscreenButton.Dispose();
            iShowFullscreenButton = null;
            iHideFullscreenButton.Dispose();
            iHideFullscreenButton = null;
            iMainView.Dispose();
            iMainView = null;
            iFullscreenView.Dispose();
            iFullscreenView = null;
        }

        public bool IsShowingLargeArtwork
        {
            get
            {
                return iIsShowingLargeArtwork;
            }
            set
            {
                if (!iDisposed)
                {
                    iFullscreenView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                    iHideFullscreenButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;

                    iMainView.Visibility = value ? ViewStates.Gone : ViewStates.Visible;
                    iShowFullscreenButton.Visibility = value ? ViewStates.Gone : ViewStates.Visible;

                    iIsShowingLargeArtwork = value;
                    OnEventViewStateChanged();
                }
            }
        }

        private void OnEventViewStateChanged()
        {
            EventHandler<EventArgs> del = EventViewStateChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }


        private void HideFullscreenButtonClick(object sender, EventArgs e)
        {
            IsShowingLargeArtwork = false;
        }

        private void ShowFullscreenButtonClick(object sender, EventArgs e)
        {
            IsShowingLargeArtwork = true;
        }

        private bool iDisposed;
        private View iFullscreenView;
        private View iMainView;
        private View iHideFullscreenButton;
        private View iShowFullscreenButton;
        private bool iIsShowingLargeArtwork;
    }

    public class FullscreenArtworkMediator : Java.Lang.Object, Animation.IAnimationListener, ILargeArtworkMediator
    {
        public FullscreenArtworkMediator(View aFullscreenView, View aMainView, View aShowFullscreenButton, View aHideFullscreenButton, bool aInitialState)
        {
            iFullscreenView = aFullscreenView;
            iMainView = aMainView;
            iShowFullscreenButton = aShowFullscreenButton;
            iHideFullscreenButton = aHideFullscreenButton;
            iHideFullscreenButton.Click += HideFullscreenButtonClick;
            iShowFullscreenButton.Click += ShowFullscreenButtonClick;
            iShowAnimation = new ScaleAnimation(0, 1, 0, 1);
            iShowAnimation.Duration = kDuration;
            iShowAnimation.Interpolator = new Android.Views.Animations.LinearInterpolator();
            iShowAnimation.SetAnimationListener(this);
            iHideAnimation = new ScaleAnimation(1, 0, 1, 0);
            iHideAnimation.Duration = kDuration;
            iHideAnimation.Interpolator = new Android.Views.Animations.LinearInterpolator();
            iHideAnimation.SetAnimationListener(this);
            iIsShowingLargeArtwork = aInitialState;
            iFullscreenView.Visibility = iIsShowingLargeArtwork ? ViewStates.Visible : ViewStates.Gone;
        }

        public event EventHandler<EventArgs> EventViewStateChanged;

        public void Dispose()
        {
            iDisposed = true;
            iShowAnimation.SetAnimationListener(null);
            iHideAnimation.SetAnimationListener(null);
            iShowAnimation.Dispose();
            iShowAnimation = null;
            iHideAnimation.Dispose();
            iHideAnimation = null;
            iHideFullscreenButton.Click -= HideFullscreenButtonClick;
            iHideFullscreenButton = null;
            iShowFullscreenButton.Click -= ShowFullscreenButtonClick;
            iShowFullscreenButton = null;
            iFullscreenView.Dispose();
            iFullscreenView = null;
            iMainView.Dispose();
            iMainView = null;
            base.Dispose();
        }

        public bool IsShowingLargeArtwork
        {
            get
            {
                return iIsShowingLargeArtwork;
            }
            set
            {
                if (!iAnimating && !iDisposed)
                {
                    iAnimating = true;
                    iFullscreenView.Visibility = ViewStates.Visible;
                    if (!iIsShowingLargeArtwork)
                    {
                        iFullscreenView.Animation = iShowAnimation;
                    }
                    else
                    {
                        iFullscreenView.Animation = iHideAnimation;
                    }
                    iIsShowingLargeArtwork = value;
                    iFullscreenView.StartAnimation(iFullscreenView.Animation);
                    OnEventViewStateChanged();
                }
            }
        }

        private void OnEventViewStateChanged()
        {
            EventHandler<EventArgs> del = EventViewStateChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }


        private void HideFullscreenButtonClick(object sender, EventArgs e)
        {
            IsShowingLargeArtwork = false;
        }

        private void ShowFullscreenButtonClick(object sender, EventArgs e)
        {
            IsShowingLargeArtwork = true;
        }

        #region IAnimationListener Members

        public void OnAnimationEnd(Animation animation)
        {
            if (!iIsShowingLargeArtwork && !iDisposed)
            {
                iFullscreenView.Visibility = ViewStates.Gone;
            }
            iAnimating = false;
        }

        public void OnAnimationRepeat(Animation animation)
        {
        }

        public void OnAnimationStart(Animation animation)
        {
        }

        #endregion

        private View iFullscreenView;
        private View iMainView;
        private View iHideFullscreenButton;
        private View iShowFullscreenButton;
        private bool iIsShowingLargeArtwork;
        private bool iAnimating;
        private Animation iShowAnimation;
        private Animation iHideAnimation;
        private const int kDuration = 300;
        private bool iDisposed;
    }

    #endregion

    #region OptionsMediator

    public class OptionsMediator
    {
        public OptionsMediator(IPopupFactory aPopupFactory, Stack aStack, AndroidImageCache aImageCache, IconResolver aIconResolver, bool aShowCancelButton)
        {
            iImageCache = aImageCache;
            iIconResolver = aIconResolver;
            iPopupFactory = aPopupFactory;
            iStack = aStack;
            iOptionsView = new OptionsView(iStack, iStack.Invoker, Resource.Drawable.BrowserDown, Android.Graphics.Color.Black, iImageCache, iIconResolver.IconLoading.Image);
            iOptionsView.ItemBackgroundColor = new Android.Graphics.Color(10, 10, 10);
            iOptionsView.BackButtonLayoutId = Resource.Layout.BackButton;
            iOptionsView.EditButtonLayoutId = Resource.Layout.EditButton;
            iOptionsView.RequestDeleteButtonResourceId = Resource.Layout.RequestDeleteButton;
            iOptionsView.ConfirmDeleteButtonResourceId = Resource.Layout.ConfirmDeleteButton;
            iOptionsView.CancelButtonLayoutId = Resource.Layout.BackButton;
            iOptionsView.CancelButtonText = "Back";
            iOptionsView.ShowCancelButton = aShowCancelButton;
            iOptionsView.EventCancelButtonClicked += EventCancelButtonClickedHandler;
            iGetKazooButtonContainer = iStack.LayoutInflater.Inflate(Resource.Layout.GetKazoo, null) as ViewGroup;
            var layoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            layoutParameters.AddRule(LayoutRules.Below, iOptionsView.MasterTitleView.Id);
            layoutParameters.AddRule(LayoutRules.CenterHorizontal);
            iOptionsView.MasterContainer.AddView(iGetKazooButtonContainer, layoutParameters);
            iGetKazooButton = iGetKazooButtonContainer.FindViewById<Button>(Resource.Id.settingsgetkazoobutton);
            iGetKazooButton.Click += GetKazooButtonClickHandler;
            iStack.NotificationView.EventNotificationUpdated += NotificationUpdated;
            NotificationUpdated(this, EventArgs.Empty);

            iStack.HelperKinsky.EventOptionPagesChanged += EventOptionPagesChangedHandler;
            aStack.Invoker.BeginInvoke((Action)(() =>
            {
                UpdateOptionPages();
            }));
        }

        private void NotificationUpdated(object sender, EventArgs args)
        {
            iGetKazooButtonContainer.Visibility = iStack.NotificationView.HasNotification ? ViewStates.Visible : ViewStates.Gone;
            if (iSettingsButtonBadge != null)
            {
                iSettingsButtonBadge.Visibility = iStack.NotificationView.HasNotification ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        private void GetKazooButtonClickHandler(object sender, EventArgs args)
        {
            if (iStack.NotificationView.HasNotification)
            {
                iStack.NotificationView.ShowNow();
                iPopup.Dismiss();
            }
        }

        private void EventCancelButtonClickedHandler(object sender, EventArgs e)
        {
            iPopup.Dismiss();
        }

        public View SettingsButton
        {
            set
            {
                if (iSettingsButton != null)
                {
                    iSettingsButton.Click -= SettingsButtonClickHandler;
                    iSettingsButton.Visibility = ViewStates.Gone;
                }
                iSettingsButton = value;
                if (iSettingsButton != null)
                {
                    iSettingsButton.Click += SettingsButtonClickHandler;
                    iSettingsButton.Visibility = ViewStates.Visible;
                }
                if (iPopup != null)
                {
                    iPopup.Dismiss();
                }
            }
        }

        public View SettingsButtonBadge
        {
            set
            {
                iSettingsButtonBadge = value;
                NotificationUpdated(this, EventArgs.Empty); // update badge visibility
            }
        }

        public View PopupAnchor
        {
            set
            {
                iPopupAnchor = value;
            }
        }

        private void SettingsButtonClickHandler(object sender, EventArgs e)
        {
            ShowOptions();
        }

        private void ShowOptions()
        {
            Assert.Check(iPopupAnchor != null, "iPopupAnchor != null");
            Assert.Check(iOptionsView != null, "iOptionsView != null");
            if (!PopupManager.IsShowingPopup)
            {
                iPopup = iPopupFactory.CreatePopup(iOptionsView, iPopupAnchor);
                if (iPopup is SpeechBubblePopup)
                {
                    IWindowManager windowManager = iStack.ApplicationContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
                    int screenWidth = windowManager.DefaultDisplay.Width;
                    (iPopup as SpeechBubblePopup).Width = screenWidth / 2;
                    (iPopup as SpeechBubblePopup).StretchVertical = true;
                }
                iPopup.Show();
                iPopup.EventDismissed += EventDismissedHandler;
            }
        }

        private void EventOptionPagesChangedHandler(object sender, EventArgs e)
        {
            if (iStack.Invoker.InvokeRequired)
            {
                iStack.Invoker.BeginInvoke((Action)(() =>
                {
                    UpdateOptionPages();
                }));
            }
            else
            {
                UpdateOptionPages();
            }
        }

        private void EventDismissedHandler(object sender, EventArgs e)
        {
            if (iOptionsView != null)
            {
                if (!iOptionsView.IsShowingMasterView)
                {
                    iOptionsView.ToggleView();
                }
            }
            if (iPopup != null)
            {
                iPopup.EventDismissed -= EventDismissedHandler;
                iPopup = null;
            }
        }

        private void UpdateOptionPages()
        {
            if (iOptionsView != null)
            {
                List<IOptionPage> optionPages = new List<IOptionPage>();
                IList<IOptionPage> helperKinskyPages = iStack.HelperKinsky.OptionPages;
                foreach (IOptionPage page in helperKinskyPages)
                {
                    bool ignorePage = false;

                    foreach (Option o in page.Options)
                    {
                        if (o.Name == OptionNetworkInterface.kName && o.Allowed.Count < 3)
                        {
                            ignorePage = true;
                        }
                    }

                    if (!ignorePage)
                    {
                        optionPages.Add(page);
                    }
                }

                if (iAboutView != null)
                {
                    iAboutView.Dispose();
                }

                iAboutView = iStack.LayoutInflater.Inflate(Resource.Layout.HelpAbout, null);
                iAboutView.FindViewById<TextView>(Resource.Id.helpaboutproduct).Text = iStack.HelperKinsky.Product;
                iAboutView.FindViewById<TextView>(Resource.Id.helpaboutversion).Text = iStack.HelperKinsky.Version;
                iAboutView.FindViewById<TextView>(Resource.Id.helpaboutcopyright).Text = iStack.HelperKinsky.Copyright;
                iAboutView.FindViewById<TextView>(Resource.Id.helpaboutcompany).Text = iStack.HelperKinsky.Company;
                iAboutView.FindViewById<TextView>(Resource.Id.helpaboutdescription).Text = iStack.HelperKinsky.Description;

                optionPages.Insert(0, new HelpAboutOptionPage(iAboutView, iStack.TabletView ? kManualLinkTablet : kManualLinkPhone));
                iOptionsView.OptionPages = optionPages;
            }
        }

        public void ToggleOptions()
        {
            if (iPopup != null)
            {
                iPopup.Dismiss();
            }
            else
            {
                ShowOptions();
            }
        }

        public void Dispose()
        {
            iStack.NotificationView.EventNotificationUpdated -= NotificationUpdated;
            iGetKazooButton.Click -= GetKazooButtonClickHandler;
            iStack.HelperKinsky.EventOptionPagesChanged -= EventOptionPagesChangedHandler;
            if (iPopup != null)
            {
                iPopup.Dismiss();
            }
            if (iAboutView != null)
            {
                iAboutView.Dispose();
            }
            iOptionsView.EventCancelButtonClicked -= EventCancelButtonClickedHandler;
            iOptionsView.Dispose();
            iOptionsView = null;
        }

        private Button iGetKazooButton;
        private ViewGroup iGetKazooButtonContainer;
        private IPopupFactory iPopupFactory;
        private View iSettingsButton;
        private Stack iStack;
        private OptionsView iOptionsView;
        private IPopup iPopup;
        private View iPopupAnchor;
        private AndroidImageCache iImageCache;
        private IconResolver iIconResolver;
        private static string kManualLinkTablet = "http://oss.linn.co.uk/trac/wiki/KinskyAndroidTabletDavaarManual";
        private static string kManualLinkPhone = "http://oss.linn.co.uk/trac/wiki/KinskyAndroidPhoneDavaarManual";
        private View iAboutView;
        private View iSettingsButtonBadge;
    }

    #endregion

    #region ViewWidgetPlayMode

    public class ViewWidgetPlayMode : IViewWidgetPlayMode
    {

        public ViewWidgetPlayMode() { }

        public ToggleButton ShuffleButton
        {
            set
            {
                if (iShuffleButton != null)
                {
                    iShuffleButton.Click -= ShuffleButtonClickHandler;
                }
                iShuffleButton = value;
                if (iShuffleButton != null)
                {
                    iShuffleButton.Click += ShuffleButtonClickHandler;
                }
                UpdateButtons();
            }
        }

        public ToggleButton RepeatButton
        {
            set
            {
                if (iRepeatButton != null)
                {
                    iRepeatButton.Click -= RepeatButtonClickHandler;
                }
                iRepeatButton = value;
                if (iRepeatButton != null)
                {
                    iRepeatButton.Click += RepeatButtonClickHandler;
                }
                UpdateButtons();
            }
        }

        #region IViewWidgetPlayMode Members

        public void Open()
        {
        }

        public void Close()
        {
            iInitialised = false;
            UpdateButtons();
        }

        public void Initialised()
        {
            iInitialised = true;
            UpdateButtons();
        }

        public void SetShuffle(bool aShuffle)
        {
            iShuffle = aShuffle;
            UpdateButtons();
        }

        public void SetRepeat(bool aRepeat)
        {
            iRepeat = aRepeat;
            UpdateButtons();
        }

        public event EventHandler<EventArgs> EventToggleShuffle;

        public event EventHandler<EventArgs> EventToggleRepeat;

        #endregion

        private void ShuffleButtonClickHandler(object sender, EventArgs e)
        {
            OnEventToggleShuffle();
        }

        private void RepeatButtonClickHandler(object sender, EventArgs e)
        {
            OnEventToggleRepeat();
        }

        private void UpdateButtons()
        {
            if (iShuffleButton != null)
            {
                iShuffleButton.Visibility = iInitialised ? ViewStates.Visible : ViewStates.Gone;
                iShuffleButton.Checked = iShuffle;
            }
            if (iRepeatButton != null)
            {
                iRepeatButton.Visibility = iInitialised ? ViewStates.Visible : ViewStates.Gone;
                iRepeatButton.Checked = iRepeat;
            }
        }

        private void OnEventToggleShuffle()
        {
            EventHandler<EventArgs> del = EventToggleShuffle;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventToggleRepeat()
        {
            EventHandler<EventArgs> del = EventToggleRepeat;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private ToggleButton iRepeatButton;
        private ToggleButton iShuffleButton;
        private bool iInitialised;
        private bool iShuffle;
        private bool iRepeat;
    }

    #endregion

    #region ViewWidgetButton

    public class ViewWidgetButtonStandby : ViewWidgetButton
    {
        public ViewWidgetButtonStandby() : base() { }

        protected override void SetButtonState()
        {
            if (iViewButton is ToggleButton)
            {
                (iViewButton as ToggleButton).Checked = !iOpen;
            }
        }
    }

    public class ViewWidgetButton : IViewWidgetButton
    {

        public ViewWidgetButton()
        {
            iOpen = false;
        }

        public View Button
        {
            set
            {
                if (iViewButton != null)
                {
                    iViewButton.Click -= ClickHandler;
                }
                iViewButton = value;
                if (iViewButton != null)
                {
                    iViewButton.Click += ClickHandler;
                }
                SetButtonState();
            }
        }

        #region IViewWidgetButton Members

        public void Open()
        {
            iOpen = true;
            SetButtonState();
        }

        public void Close()
        {
            iOpen = false;
            SetButtonState();
        }

        public event EventHandler<EventArgs> EventClick;

        #endregion

        protected virtual void SetButtonState()
        {
            if (iViewButton != null)
            {
                iViewButton.Enabled = iOpen;
            }
        }

        private void ClickHandler(object sender, EventArgs e)
        {
            OnEventClick();
        }

        private void OnEventClick()
        {
            EventHandler<EventArgs> del = EventClick;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        protected View iViewButton;
        protected bool iOpen;
    }

    #endregion

    #region Track

    public class ViewWidgetTrack : IViewWidgetTrack
    {
        public ViewWidgetTrack(IInvoker aInvoker, AndroidResourceManager aResourceManager, AndroidImageCache aHighResCache)
        {
            iResourceManager = aResourceManager;
            iInvoker = aInvoker;
            iHighResCache = aHighResCache;
            iDisplayTechnicalInfo = true;
        }

        public bool DisplayTechnicalInfo
        {
            set
            {
                iDisplayTechnicalInfo = value;
                Refresh();
            }
        }

        public TextView Display1
        {
            set
            {
                iDisplay1 = value;
                Refresh();
            }
        }

        public TextView Display2
        {
            set
            {
                iDisplay2 = value;
                Refresh();
            }
        }

        public TextView Display3
        {
            set
            {
                iDisplay3 = value;
                Refresh();
            }
        }

        public TextView TechnicalInfo
        {
            set
            {
                iTechnicalInfo = value;
                Refresh();
            }
        }

        public LazyLoadingImageView ImageView
        {
            set
            {
                iImageView = value;
                UpdateImage();
            }
        }

        #region IViewWidgetTrack Members

        public void Open()
        {
            Assert.Check(!iOpen);
            iOpen = true;
            UpdateImage();
            Refresh();
        }

        public void Close()
        {
            iOpen = false;
            iDisplayText1 = string.Empty;
            iDisplayText2 = string.Empty;
            iDisplayText3 = string.Empty;
            iTechnicalInfoText = string.Empty;
            iIcon = null;
            UpdateImage();

            iBitrate = 0;
            iSampleRate = 0;
            iBitDepth = 0;
            iCodec = string.Empty;
            iLossless = false;

            Refresh();
        }

        public void Initialised()
        {
        }

        public void SetItem(upnpObject aObject)
        {
            if (aObject != null)
            {
                ItemInfo info = new ItemInfo(aObject);
                ReadOnlyCollection<KeyValuePair<string, string>> displayItems = info.DisplayItems;
                iDisplayText1 = displayItems.Count > 0 ? displayItems[0].Value : string.Empty;
                iDisplayText2 = displayItems.Count > 1 ? displayItems[1].Value : string.Empty;
                iDisplayText3 = displayItems.Count > 2 ? displayItems[2].Value : string.Empty;
                iIcon = new IconResolver(iResourceManager).GetIcon(aObject);
            }
            else
            {
                iDisplayText1 = string.Empty;
                iDisplayText2 = string.Empty;
                iDisplayText3 = string.Empty;
                iIcon = null;
            }
            UpdateImage();
            Refresh();
        }

        public void SetMetatext(upnpObject aObject)
        {
            if (aObject != null)
            {
                ItemInfo info = new ItemInfo(aObject);
                ReadOnlyCollection<KeyValuePair<string, string>> displayItems = info.DisplayItems;
                iDisplayText2 = displayItems.Count > 0 ? displayItems[0].Value : string.Empty;
                iDisplayText3 = displayItems.Count > 1 ? displayItems[1].Value : string.Empty;
                Refresh();
            }
        }

        public void SetBitrate(uint aBitrate)
        {
            iBitrate = aBitrate;
        }

        public void SetSampleRate(float aSampleRate)
        {
            iSampleRate = aSampleRate;
        }

        public void SetBitDepth(uint aBitDepth)
        {
            iBitDepth = aBitDepth;
        }

        public void SetCodec(string aCodec)
        {
            iCodec = aCodec;
        }

        public void SetLossless(bool aLossless)
        {
            iLossless = aLossless;
        }

        public void Update()
        {
            Refresh();
        }

        #endregion

        private void Refresh()
        {
            if (iOpen)
            {

                iTechnicalInfoText = TechnicalInfoHelper.FormatTechnicalInfo(iCodec, iBitrate, iBitDepth, iSampleRate, iLossless);
                if (iDisplay1 != null)
                {
                    iDisplay1.Text = iDisplayText1;
                }
                if (iDisplay2 != null)
                {
                    iDisplay2.Text = iDisplayText2;
                }
                if (iDisplay3 != null)
                {
                    iDisplay3.Text = iDisplayText3;
                }
                if (iTechnicalInfo != null)
                {
                    iTechnicalInfo.Text = iTechnicalInfoText;
                    iTechnicalInfo.Visibility = iDisplayTechnicalInfo ? ViewStates.Visible : ViewStates.Gone;
                }
            }
            else
            {
                if (iDisplay1 != null)
                {
                    iDisplay1.Text = string.Empty;
                }
                if (iDisplay2 != null)
                {
                    iDisplay2.Text = string.Empty;
                }
                if (iDisplay3 != null)
                {
                    iDisplay3.Text = string.Empty;
                }
                if (iTechnicalInfo != null)
                {
                    iTechnicalInfo.Text = string.Empty;
                }
            }
        }

        private void UpdateImage()
        {
            if (iOpen && iImageView != null)
            {
                if (iIcon != null)
                {
                    iImageView.SetImageBitmap(iResourceManager.GetBitmap(Resource.Drawable.Loading));
                    if (iIcon.IsUri)
                    {
                        iImageView.LoadImage(iHighResCache, iIcon.ImageUri);
                    }
                    else
                    {
                        iImageView.SetImageBitmap(iIcon.Image);
                    }
                }
                else
                {
                    iImageView.SetImageBitmap(iResourceManager.GetBitmap(Resource.Drawable.Loading));
                }
            }
            else
            {
                if (iImageView != null)
                {
                    iImageView.SetImageBitmap(null);
                }
            }
        }



        private IInvoker iInvoker;
        private TextView iDisplay1;
        private TextView iDisplay2;
        private TextView iDisplay3;
        private TextView iTechnicalInfo;
        private LazyLoadingImageView iImageView;

        private string iDisplayText1;
        private string iDisplayText2;
        private string iDisplayText3;
        private string iTechnicalInfoText;
        private Icon<Bitmap> iIcon;

        private uint iBitrate;
        private float iSampleRate;
        private uint iBitDepth;
        private string iCodec;
        private bool iLossless;

        private bool iOpen;
        private bool iDisplayTechnicalInfo;
        private AndroidResourceManager iResourceManager;
        private AndroidImageCache iHighResCache;
    }

    #endregion

    #region Playlist

    public class ViewWidgetPlaylistReceiver : ViewWidgetPlaylistBase, IViewWidgetPlaylistReceiver, IViewWidgetSelector<Linn.Kinsky.Room>
    {
        public ViewWidgetPlaylistReceiver(Context aContext,
            IInvoker aInvoker,
            IconResolver aIconResolver,
            AndroidImageCache aImageCache,
            FlingStateManager aFlingStateManager,
            IViewSaveSupport aViewSaveSupport) :
            base(aContext, aInvoker, aIconResolver, aImageCache, aFlingStateManager, aViewSaveSupport)
        {
            iRooms = new List<Room>();
        }

        protected override bool CanEdit
        {
            get { return false; }
        }
        protected override bool CanSave
        {
            get { return false; }
        }
        protected override bool ShowAdditionalInfo
        {
            get { return false; }
        }

        #region IViewWidgetPlaylistReceiver Members


        public void SetSenders(IList<Linn.Topology.ModelSender> aSenders)
        {
            iSenders = aSenders;
            UpdateSenders();
        }

        public void SetChannel(Linn.Topology.Channel aChannel)
        {
            iChannel = aChannel;
            UpdateSenders();
        }

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        #endregion

        #region IViewWidgetSelector<Room> Members

        void IViewWidgetSelector<Room>.Open() { }
        void IViewWidgetSelector<Room>.Close() { iRooms.Clear(); }

        public void InsertItem(int aIndex, Room aItem)
        {
            iRooms.Insert(aIndex, aItem);
            UpdateSenders();
        }

        public void RemoveItem(Room aItem)
        {
            iRooms.Remove(aItem);
            UpdateSenders();
        }

        public void ItemChanged(Room aItem) { }

        public void SetSelected(Room aItem) { }

        public event EventHandler<EventArgsSelection<Room>> EventSelectionChanged;

        #endregion

        protected override void CreateAdapter()
        {
            base.CreateAdapter();
            iPlaylistAdapter.EventJumpToRoomClick += EventJumpToRoomClickHandler;
            OnPlaylistUpdated();
        }

        protected override void CloseAdapter()
        {
            iPlaylistAdapter.EventJumpToRoomClick -= EventJumpToRoomClickHandler;
            base.CloseAdapter();
        }

        private void EventJumpToRoomClickHandler(object sender, EventArgs args)
        {
            if (iPlaylistAdapter != null && iPlaylistAdapter.Room != null)
            {
                OnEventSelectRoom(iPlaylistAdapter.Room);
            }
        }

        private void OnEventSelectRoom(Room aRoom)
        {
            EventHandler<EventArgsSelection<Room>> del = EventSelectionChanged;
            if (del != null)
            {
                del(this, new EventArgsSelection<Room>(aRoom));
            }
        }

        private void UpdateSenders()
        {
            List<Linn.Topology.MrItem> items = new List<Linn.Topology.MrItem>();
            if (iSenders != null)
            {
                for (int i = 0; i < iSenders.Count; ++i)
                {
                    if (iSenders[i].Metadata != null)
                    {
                        DidlLite didl = new DidlLite();
                        didl.Add(iSenders[i].Metadata[0]);
                        didl[0].Title = iSenders[i].FullName;
                        items.Add(new Linn.Topology.MrItem(0, "", didl));
                    }
                }
            }
            SetPlaylist(items);
        }

        protected override void UpdateTrack()
        {
            base.UpdateTrack();
            Room room = null;
            if (iPlaylistAdapter != null)
            {
                if (iChannel != null)
                {
                    room = FindRoom(iChannel);
                }
                iPlaylistAdapter.Room = room;
                OnEventDataChanged();
            }
        }

        protected override void OnPlaylistUpdated()
        {
            Linn.Topology.MrItem newItem = null;
            if (iChannel != null && iPlaylistItems != null)
            {
                newItem = (from m in iPlaylistItems
                           where m.DidlLite[0].Title == iChannel.DidlLite[0].Title
                           select m).SingleOrDefault();

            }
            SetTrack(newItem);
            base.OnPlaylistUpdated();
        }

        protected override void OnEventSeekTrack(uint aIndex)
        {
            List<upnpObject> items = new List<upnpObject>();
            items.Add(iPlaylistItems[(int)aIndex].DidlLite[0]);
            OnEventSetChannel(new MediaRetrieverNoRetrieve(items));
            base.OnEventSeekTrack(aIndex);
        }

        private void OnEventSetChannel(IMediaRetriever aRetriever)
        {
            EventHandler<EventArgsSetChannel> del = EventSetChannel;
            if (del != null)
            {
                del(this, new EventArgsSetChannel(aRetriever));
            }
        }

        private Linn.Kinsky.Room FindRoom(Linn.Topology.Channel aChannel)
        {
            Linn.Kinsky.Room foundRoom = null;

            if (aChannel != null && iSenders != null)
            {
                foreach (Linn.Topology.ModelSender modelSender in iSenders)
                {
                    foreach (resource r in modelSender.Metadata[0].Res)
                    {
                        if (r.Uri == aChannel.Uri)
                        {
                            foundRoom = (from room in iRooms where room.Name == modelSender.Room select room).SingleOrDefault();
                            if (foundRoom != null)
                            {
                                break;
                            }
                        }
                    }
                    if (foundRoom != null)
                    {
                        break;
                    }
                }
            }
            return foundRoom;
        }

        private List<Room> iRooms;
        private Linn.Topology.Channel iChannel;
        IList<Linn.Topology.ModelSender> iSenders;
    }

    public class ViewWidgetPlaylistRadio : ViewWidgetPlaylistBase, IViewWidgetPlaylistRadio
    {
        public ViewWidgetPlaylistRadio(Context aContext, IInvoker aInvoker, IconResolver aIconResolver, AndroidImageCache aImageCache, FlingStateManager aFlingStateManager, IViewSaveSupport aViewSaveSupport)
            : base(aContext, aInvoker, aIconResolver, aImageCache, aFlingStateManager, aViewSaveSupport)
        {
            iPresetIndex = -1;
        }

        protected override bool CanEdit
        {
            get { return false; }
        }
        protected override bool CanSave
        {
            get { return false; }
        }
        protected override bool ShowAdditionalInfo
        {
            get { return false; }
        }

        #region IViewWidgetPlaylistRadio Members


        public void SetPresets(IList<Linn.Topology.MrItem> aPresets)
        {
            base.SetPlaylist(aPresets);
        }

        public void SetChannel(Linn.Topology.Channel aChannel)
        {
            iChannel = aChannel;
        }

        public void SetPreset(int aPresetIndex)
        {
            iPresetIndex = aPresetIndex;
            if (iPlaylistItems != null && aPresetIndex >= 0 && aPresetIndex < iPlaylistItems.Count)
            {
                SetTrack(iPlaylistItems[aPresetIndex]);
            }
            else
            {
                SetTrack(null);
            }
        }

        public event EventHandler<EventArgsSetPreset> EventSetPreset;

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        #endregion

        protected override void OnPlaylistUpdated()
        {
            if (iPresetIndex != -1)
            {
                SetPreset(iPresetIndex);
            }
            base.OnPlaylistUpdated();
        }

        protected override void OnEventSeekTrack(uint aIndex)
        {
            Linn.Topology.MrItem preset = null;
            if (aIndex >= 0 && aIndex < iPlaylistItems.Count)
            {
                preset = iPlaylistItems[(int)aIndex];
            }
            EventHandler<EventArgsSetPreset> del = EventSetPreset;
            if (del != null)
            {
                del(this, new EventArgsSetPreset(preset));
            }
        }

        protected virtual void OnEventSetChannel(IMediaRetriever aRetriever)
        {
            EventHandler<EventArgsSetChannel> del = EventSetChannel;
            if (del != null)
            {
                del(this, new EventArgsSetChannel(aRetriever));
            }

        }

        private Linn.Topology.Channel iChannel;
        private int iPresetIndex;
    }

    public class ViewWidgetPlaylist : ViewWidgetPlaylistBase, IViewWidgetPlaylist
    {
        public ViewWidgetPlaylist(Context aContext, IInvoker aInvoker, IconResolver aIconResolver, AndroidImageCache aImageCache, FlingStateManager aFlingStateManager, IViewSaveSupport aViewSaveSupport)
            : base(aContext, aInvoker, aIconResolver, aImageCache, aFlingStateManager, aViewSaveSupport) { }


        protected override bool CanEdit
        {
            get { return true; }
        }
        protected override bool CanSave
        {
            get { return true; }
        }
        protected override bool ShowAdditionalInfo
        {
            get { return true; }
        }
    }

    public abstract class ViewWidgetPlaylistBase : IAsyncLoader<PlaylistDisplayItem<Bitmap>>
    {
        public ViewWidgetPlaylistBase(Context aContext, IInvoker aInvoker, IconResolver aIconResolver, AndroidImageCache aImageCache, FlingStateManager aFlingStateManager, IViewSaveSupport aViewSaveSupport)
        {
            iViewSaveSupport = aViewSaveSupport;
            iContext = aContext;
            iFlingStateManager = aFlingStateManager;
            iInvoker = aInvoker;
            iIconResolver = aIconResolver;
            iImageCache = aImageCache;
            iIsGrouping = false;
        }

        protected abstract bool CanEdit { get; }
        protected abstract bool CanSave { get; }
        protected abstract bool ShowAdditionalInfo { get; }

        public View DeleteButton
        {
            set
            {
                iDeleteButton = value;
                SetButtonState();
            }
        }

        public View SaveButton
        {
            set
            {
                iSaveButton = value;
                SetButtonState();
            }
        }

        public ViewGroup ButtonContainer
        {
            set
            {
                iButtonContainer = value;
                SetButtonState();
            }
        }

        public ToggleButton EditButton
        {
            set
            {
                if (iEditModeButton != null)
                {
                    iEditModeButton.Click -= EditButtonClickHandler;
                }
                iEditModeButton = value;
                if (iEditModeButton != null)
                {
                    iEditModeButton.Click += EditButtonClickHandler;
                }
                SetButtonState();
            }
        }

        public ListView PlaylistView
        {
            set
            {
                if (iPlaylistView != null)
                {
                    iPlaylistView.ItemClick -= ItemClickHandler;
                    iPlaylistView.Adapter = null;
                    if (iContainerView != null && iInitialised)
                    {
                        iContainerView.RemoveView(iPlaylistView);
                    }
                    iPlaylistView.SetOnScrollListener(null);
                    iFlingStateManager.SetFlinging(iPlaylistView, false);
                    iFlingStateManager.EventFlingStateChanged -= EventFlingStateChangedHandler;
                    if (iPlaylistAdapter != null)
                    {
                        CloseAdapter();
                    }
                }
                iPlaylistView = value;
                if (iPlaylistView != null)
                {
                    using (ViewGroup.LayoutParams parms = new ViewGroup.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent))
                    {
                        iPlaylistView.LayoutParameters = parms;
                    }
                    iPlaylistView.ItemClick += ItemClickHandler;
                    if (iInitialised)
                    {
                        CreateAdapter();
                        iPlaylistView.Adapter = iPlaylistAdapter;
                    }
                    if (iContainerView != null && iInitialised)
                    {
                        iContainerView.RemoveAllViews();
                        iContainerView.AddView(iPlaylistView);
                    }
                    iPlaylistView.SetOnScrollListener(new FlingScrollListener(iFlingStateManager));
                    iFlingStateManager.EventFlingStateChanged += EventFlingStateChangedHandler;
                    iPlaylistView.FastScrollEnabled = true;
                }
                SetButtonState();
                UpdateTrack();
            }
        }

        public ViewGroup ContainerView
        {
            set
            {
                if (iContainerView != null && iContainerView.ChildCount > 0)
                {
                    iContainerView.RemoveAllViews();
                }
                iContainerView = value;
                if (value != null && iInitialised && iPlaylistView != null)
                {
                    iContainerView.AddView(iPlaylistView);
                }
                SetButtonState();
            }
        }

        public View ScrollToButton
        {
            set
            {
                if (iScrollToButton != null)
                {
                    iScrollToButton.Click -= ScrollToButtonClick;
                }
                iScrollToButton = value;
                if (iScrollToButton != null)
                {
                    iScrollToButton.Click += ScrollToButtonClick;
                }
            }
        }

        private void ScrollToButtonClick(object sender, EventArgs args)
        {
            if (iInitialised && iPlaylistView != null && iPlaylistDisplayItems != null)
            {
                int scrollIndex = iPlaylistDisplayItems.IndexOf(CurrentDisplayTrack);
                if (scrollIndex != -1)
                {
                    iPlaylistView.SmoothScrollToPosition(scrollIndex);
                }
            }
        }

        public bool IsGrouping
        {
            set
            {
                Assert.Check(!iInvoker.InvokeRequired, "!iInvoker.InvokeRequired");
                iIsGrouping = value;
                if (iInitialised && iPlaylistItems != null)
                {
                    SetPlaylist(iPlaylistItems);
                }
            }
        }

        #region IViewWidgetPlaylist Members

        public virtual void Open()
        {
            iEditing = false;
            SetButtonState();
        }

        public virtual void Close()
        {
            iInitialised = false;
            if (iPlaylistItems != null)
            {
                iPlaylistItems.Clear();
                iPlaylistDisplayItems = null;
            }
            if (iPlaylistView != null)
            {
                iPlaylistView.Adapter = null;
                iFlingStateManager.SetFlinging(iPlaylistView, false);
            }
            if (iPlaylistAdapter != null)
            {
                CloseAdapter();
            }
            if (iContainerView != null && iContainerView.ChildCount > 0)
            {
                iContainerView.RemoveAllViews();
            }
            iEditing = false;
            SetButtonState();
        }

        protected virtual void CreateAdapter()
        {
            iPlaylistAdapter = new PlaylistAdapter(iContext, this, iIconResolver, iImageCache);
            iPlaylistAdapter.EventItemDeleted += EventItemDeletedHandler;
            iPlaylistAdapter.EventItemMovedDown += EventItemMovedDownHandler;
            iPlaylistAdapter.EventItemMovedUp += EventItemMovedUpHandler;
            iPlaylistAdapter.ShowAdditionalInfo = ShowAdditionalInfo;
        }

        protected virtual void CloseAdapter()
        {
            iPlaylistAdapter.EventItemDeleted -= EventItemDeletedHandler;
            iPlaylistAdapter.EventItemMovedDown -= EventItemMovedDownHandler;
            iPlaylistAdapter.EventItemMovedUp -= EventItemMovedUpHandler;
            iPlaylistAdapter.Close();
            iPlaylistAdapter.Dispose();
            iPlaylistAdapter = null;
        }

        public virtual void Initialised()
        {
            iInitialised = true;
            if (iContainerView != null)
            {
                iContainerView.RemoveAllViews();
                if (iPlaylistView != null)
                {
                    iContainerView.AddView(iPlaylistView);
                }
            }
            CreateAdapter();
            if (iPlaylistView != null)
            {
                iPlaylistView.Adapter = iPlaylistAdapter;
            }
            iEditing = false;
            SetButtonState();
        }

        public void SetPlaylist(IList<Linn.Topology.MrItem> aPlaylist)
        {
            Assert.Check(aPlaylist != null, "aPlaylist != null");
            new Thread(new ThreadStart(() =>
            {
                ReadOnlyCollection<PlaylistDisplayItem<Bitmap>> displayItems = new PlaylistDisplayHelper<Bitmap>(aPlaylist, iIsGrouping, iIconResolver).DisplayItems;
                iInvoker.BeginInvoke((Action)(() =>
                {
                    if (iInitialised)
                    {
                        iPlaylistItems = aPlaylist;
                        iPlaylistDisplayItems = displayItems;
                        OnPlaylistUpdated();
                    }
                }));
            })).Start();
        }

        protected virtual void OnPlaylistUpdated()
        {
            iPendingMove = false;
            UpdateTrack();
            SetButtonState();
        }

        public void SetTrack(Linn.Topology.MrItem aTrack)
        {
            iTrack = aTrack;
            UpdateTrack();
        }

        public void Save()
        {
            List<upnpObject> saveItems = new List<upnpObject>();
            foreach (Linn.Topology.MrItem item in iPlaylistItems)
            {
                saveItems.Add(item.DidlLite[0]);
            }
            iViewSaveSupport.Save(saveItems);
        }

        public void Delete()
        {
            OnEventPlaylistDeleteAll();
        }

        public event EventHandler<EventArgsSeekTrack> EventSeekTrack;

        public event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;

        public event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;

        public event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;

        public event EventHandler<EventArgs> EventPlaylistDeleteAll;

        #endregion

        #region IAsyncLoader<PlaylistDisplayItem<Bitmap>> Members

        public event EventHandler<EventArgs> EventDataChanged;

        public PlaylistDisplayItem<Bitmap> Item(int aIndex)
        {
            Assert.Check(iPlaylistDisplayItems != null, "iPlaylistDisplayItems != null");
            Assert.Check(aIndex < iPlaylistDisplayItems.Count, "aIndex < iPlaylistDisplayItems.Count");
            return iPlaylistDisplayItems[aIndex];
        }

        public int Count
        {
            get
            {
                if (iPlaylistDisplayItems != null)
                {
                    return iPlaylistDisplayItems.Count;
                }
                return 0;
            }
        }

        #endregion

        #region Private Methods

        private void SetButtonState()
        {
            bool hasItems = iInitialised && iPlaylistItems != null && iPlaylistItems.Count > 0;
            iEditing = iEditing && hasItems;
            if (iEditModeButton != null)
            {
                iEditModeButton.Visibility = hasItems && CanEdit ? ViewStates.Visible : ViewStates.Gone;
                iEditModeButton.Checked = iEditing;
            }
            if (iSaveButton != null)
            {
                iSaveButton.Visibility = hasItems && CanSave && !iEditing ? ViewStates.Visible : ViewStates.Gone;
            }
            if (iDeleteButton != null)
            {
                iDeleteButton.Visibility = iEditing ? ViewStates.Visible : ViewStates.Gone;
            }
            if (iPlaylistAdapter != null)
            {
                iPlaylistAdapter.EditMode = iEditing;
                iPlaylistAdapter.IsGrouping = iIsGrouping;
            }
            if (iButtonContainer != null)
            {
                iButtonContainer.Visibility = iInitialised && CanEdit && CanSave ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        private void EventItemDeletedHandler(object sender, EventArgsListEdit<PlaylistDisplayItem<Bitmap>> e)
        {
            Assert.Check(CanEdit && iInitialised);
            List<Linn.Topology.MrItem> items = new List<Linn.Topology.MrItem>();
            for (int i = e.Item.StartIndex; i < e.Item.StartIndex + e.Item.Count; i++)
            {
                items.Add(iPlaylistItems[i]);
            }
            OnEventPlaylistDelete(items);
        }

        private void EventItemMovedDownHandler(object sender, EventArgsListEdit<PlaylistDisplayItem<Bitmap>> e)
        {
            Assert.Check(CanEdit && iInitialised);
            if (!iPendingMove)
            {
                List<Linn.Topology.MrItem> items = new List<Linn.Topology.MrItem>();
                for (int i = e.Item.StartIndex; i < e.Item.StartIndex + e.Item.Count; i++)
                {
                    items.Add(iPlaylistItems[i]);
                }
                OnEventPlaylistMove(e.Item.NextId, items);
            }
        }

        private void EventItemMovedUpHandler(object sender, EventArgsListEdit<PlaylistDisplayItem<Bitmap>> e)
        {
            Assert.Check(CanEdit && iInitialised);
            if (!iPendingMove)
            {
                iPendingMove = true;
                List<Linn.Topology.MrItem> items = new List<Linn.Topology.MrItem>();
                for (int i = e.Item.StartIndex; i < e.Item.StartIndex + e.Item.Count; i++)
                {
                    items.Add(iPlaylistItems[i]);
                }
                OnEventPlaylistMove(e.Item.PreviousId, items);
            }
        }

        private void EditButtonClickHandler(object sender, EventArgs e)
        {
            iEditing = iEditModeButton.Checked;
            SetButtonState();
        }

        protected virtual void UpdateTrack()
        {
            if (iPlaylistAdapter != null)
            {
                iPlaylistAdapter.CurrentTrack = CurrentDisplayTrack;
            }
            OnEventDataChanged();
        }

        private PlaylistDisplayItem<Bitmap> CurrentDisplayTrack
        {
            get
            {
                if (iTrack != null && iPlaylistItems != null && iPlaylistItems.IndexOf(iTrack) != -1 && iPlaylistDisplayItems != null)
                {
                    return (from i in iPlaylistDisplayItems where i.StartIndex == iPlaylistItems.IndexOf(iTrack) && i.Count == 1 select i).SingleOrDefault();
                }
                return null;
            }
        }

        private void ItemClickHandler(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            Assert.Check(iPlaylistAdapter != null, "iPlaylistAdapter != null");
            Assert.Check(iPlaylistDisplayItems != null, "iPlaylistDisplayItems != null");
            Assert.Check(e.Position < iPlaylistDisplayItems.Count, "e.Position < iPlaylistDisplayItems.Count");
            int startIndex = iPlaylistDisplayItems[e.Position].StartIndex;
            Assert.Check(iPlaylistItems != null, "iPlaylistItems != null");
            Assert.Check(startIndex < iPlaylistItems.Count, "startIndex < iPlaylistItems.Count");
            OnEventSeekTrack((uint)startIndex);
        }

        protected virtual void OnEventDataChanged()
        {
            EventHandler<EventArgs> del = EventDataChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        protected virtual void OnEventSeekTrack(uint aIndex)
        {
            EventHandler<EventArgsSeekTrack> del = EventSeekTrack;
            if (del != null)
            {
                del(this, new EventArgsSeekTrack(aIndex));
            }
        }

        protected virtual void OnEventPlaylistInsert(uint aInsertAfterId, IMediaRetriever aMediaRetriever)
        {
            EventHandler<EventArgsPlaylistInsert> del = EventPlaylistInsert;
            if (del != null)
            {
                del(this, new EventArgsPlaylistInsert(aInsertAfterId, aMediaRetriever));
            }
        }

        protected virtual void OnEventPlaylistMove(uint aInsertAfterId, IList<Linn.Topology.MrItem> aPlaylistItems)
        {
            EventHandler<EventArgsPlaylistMove> del = EventPlaylistMove;
            if (del != null)
            {
                del(this, new EventArgsPlaylistMove(aInsertAfterId, aPlaylistItems));
            }
        }

        protected virtual void OnEventPlaylistDelete(IList<Linn.Topology.MrItem> aPlaylistItems)
        {
            EventHandler<EventArgsPlaylistDelete> del = EventPlaylistDelete;
            if (del != null)
            {
                del(this, new EventArgsPlaylistDelete(aPlaylistItems));
            }
        }

        protected virtual void OnEventPlaylistDeleteAll()
        {
            EventHandler<EventArgs> del = EventPlaylistDeleteAll;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void EventFlingStateChangedHandler(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired)
            {
                iInvoker.BeginInvoke((Action)(() =>
                {
                    SetFlingState();
                }));
            }
            else
            {
                SetFlingState();
            }
        }

        private void SetFlingState()
        {
            if (!iFlingStateManager.IsFlinging())
            {
                OnEventDataChanged();
            }
        }

        #endregion

        private bool iInitialised;
        private ListView iPlaylistView;
        protected PlaylistAdapter iPlaylistAdapter;
        private ViewGroup iContainerView;
        private IInvoker iInvoker;
        protected IList<Linn.Topology.MrItem> iPlaylistItems;
        private ReadOnlyCollection<PlaylistDisplayItem<Bitmap>> iPlaylistDisplayItems;
        private IconResolver iIconResolver;
        private AndroidImageCache iImageCache;
        protected Linn.Topology.MrItem iTrack;
        private bool iIsGrouping;
        private FlingStateManager iFlingStateManager;
        private ToggleButton iEditModeButton;
        private View iDeleteButton;
        private View iSaveButton;
        private bool iEditing;
        private Context iContext;
        private IViewSaveSupport iViewSaveSupport;
        private ViewGroup iButtonContainer;
        private View iScrollToButton;
        private bool iPendingMove;
    }

    public class ViewWidgetPlaylistDiscPlayer : ViewWidgetPlaylistAux, IViewWidgetPlaylistDiscPlayer
    {

        public ViewWidgetPlaylistDiscPlayer(Context aContext, int aImageWidth) : base(aContext, Resource.Drawable.CD, aImageWidth) { }

        #region IViewWidgetPlaylistDiscPlayer Members

        public void Eject()
        {
        }

        public void Open()
        {
            Open(string.Empty);
        }

        public void Initialised()
        {
        }

        #endregion
    }

    public class ViewWidgetPlaylistAux : IViewWidgetPlaylistAux
    {
        public ViewWidgetPlaylistAux(Context aContext, int aImageResourceId, int aImageWidth)
        {
            iImageResourceId = aImageResourceId;
            iContext = aContext;
            iImageWidth = aImageWidth;
        }

        public ViewGroup ContainerView
        {
            set
            {
                if (iContainerView != null && iContainerView.ChildCount > 0)
                {
                    iContainerView.RemoveAllViews();
                }
                iContainerView = value;
                if (iContainerView != null && iOpen)
                {
                    iContainerView.RemoveAllViews();
                    iContainerView.AddView(iImageView);
                }
            }
        }

        #region IViewWidgetPlaylistAux Members

        public void Open(string aType)
        {
            Assert.Check(!iOpen);
            iOpen = true;
            if (iImageView == null)
            {
                iImageView = new ImageView(iContext);
                using (RelativeLayout.LayoutParams parms = new RelativeLayout.LayoutParams(iImageWidth, RelativeLayout.LayoutParams.WrapContent))
                {
                    parms.AddRule(LayoutRules.CenterInParent);
                    iImageView.LayoutParameters = parms;
                }
                iImageView.SetImageResource(iImageResourceId);
            }
            if (iContainerView != null)
            {
                iContainerView.RemoveAllViews();
                iContainerView.AddView(iImageView);
            }
        }

        public void Close()
        {
            iOpen = false;
            if (iContainerView != null)
            {
                iContainerView.RemoveAllViews();
            }
            if (iImageView != null)
            {
                iImageView.Dispose();
                iImageView = null;
            }
        }

        #endregion

        private ViewGroup iContainerView;
        private bool iOpen;
        private ImageView iImageView;
        private int iImageResourceId;
        private Context iContext;
        private int iImageWidth;
    }

    public class PlaylistAdapter : AsyncArrayAdapter<PlaylistDisplayItem<Bitmap>, string>
    {
        public event EventHandler<EventArgs> EventJumpToRoomClick;

        public PlaylistAdapter(Context aContext, IAsyncLoader<PlaylistDisplayItem<Bitmap>> aLoader, IconResolver aIconResolver, AndroidImageCache aImageCache)
            : base(aContext, aLoader, "PlaylistAdapter")
        {
            iIconResolver = aIconResolver;
            iImageCache = aImageCache;
            iPlaceholder = new BitmapDrawable(iIconResolver.IconLoading.Image);
        }

        public Room Room { get; set; }

        public override void Close()
        {
            if (iCurrentJumpToRoomButton != null)
            {
                iCurrentJumpToRoomButton.Click -= JumpToRoomClickHandler;
                iCurrentJumpToRoomButton = null;
            }
            iPlaceholder.Dispose();
            iPlaceholder = null;
            base.Close();
        }

        public bool ShowAdditionalInfo { get; set; }

        public PlaylistDisplayItem<Bitmap> CurrentTrack
        {
            set
            {
                iCurrentTrack = value;
            }
        }

        public bool IsGrouping
        {
            set
            {
                iIsGrouping = value;
                NotifyDataSetChanged();
            }
        }

        protected override View CreateItemView(Context aContext, PlaylistDisplayItem<Bitmap> aItem, ViewGroup aRoot)
        {
            return LayoutInflater.Inflate(Resource.Layout.PlaylistItem, aRoot, false);
        }

        protected override void RecycleItemView(Context aContext, PlaylistDisplayItem<Bitmap> aItem, ViewCache aViewCache)
        {
            PopulateView(aItem, aViewCache);
        }

        protected override void DestroyItemView(Context aContext, ViewCache aViewCache)
        {
            LazyLoadingImageView imageView = aViewCache.FindViewById<LazyLoadingImageView>(Resource.Id.playlistitemicon);
            imageView.Dispose();
            base.DestroyItemView(aContext, aViewCache);
        }


        private void PopulateView(PlaylistDisplayItem<Bitmap> aItem, ViewCache aViewCache)
        {
            // should never get empty items here
            Assert.Check(aItem != null);
            LazyLoadingImageView imageView = aViewCache.FindViewById<LazyLoadingImageView>(Resource.Id.playlistitemicon);
            Icon<Bitmap> icon = aItem.Icon;
            if (icon.IsUri)
            {
                imageView.SetImageDrawable(iPlaceholder);
                imageView.LoadImage(iImageCache, icon.ImageUri);
            }
            else
            {
                imageView.SetImageDrawable(iPlaceholder);
            }
            imageView.Visibility = aItem == iCurrentTrack ? ViewStates.Invisible : ViewStates.Visible;

            RelativeLayout imageViewContainer = aViewCache.FindViewById<RelativeLayout>(Resource.Id.playlistitemiconcontainer);
            imageViewContainer.Visibility = aItem.IsGrouped && aItem.Count == 1 ? ViewStates.Gone : ViewStates.Visible;

            ImageView playingItem = aViewCache.FindViewById<ImageView>(Resource.Id.playlistitemplaying);
            playingItem.Visibility = aItem == iCurrentTrack ? ViewStates.Visible : ViewStates.Gone;

            ImageButton jumpToRoom = aViewCache.FindViewById<ImageButton>(Resource.Id.playlistitemjumptoroom);
            if (aItem == iCurrentTrack && Room != null)
            {
                jumpToRoom.Visibility = ViewStates.Visible;
                if (iCurrentJumpToRoomButton != null && iCurrentJumpToRoomButton != jumpToRoom)
                {
                    iCurrentJumpToRoomButton.Click -= JumpToRoomClickHandler;
                }
                iCurrentJumpToRoomButton = jumpToRoom;
                iCurrentJumpToRoomButton.Click += JumpToRoomClickHandler;
            }
            else
            {
                jumpToRoom.Visibility = ViewStates.Gone;
            }

            TextView firstLine = aViewCache.FindViewById<TextView>(Resource.Id.playlistitemfirstline);
            firstLine.Text = string.Format("{0}{1}", aItem.Count == 1 ? (aItem.StartIndex + 1).ToString() + ". " : string.Empty, aItem.DisplayField1);
            firstLine.Visibility = ViewStates.Visible;
            TextView secondLine = aViewCache.FindViewById<TextView>(Resource.Id.playlistitemsecondline);
            secondLine.Text = aItem.DisplayField2;
            secondLine.Visibility = !ShowAdditionalInfo || aItem.DisplayField2 == string.Empty ? ViewStates.Gone : ViewStates.Visible;
            TextView thirdLine = aViewCache.FindViewById<TextView>(Resource.Id.playlistitemthirdline);
            thirdLine.Text = aItem.DisplayField3;
            thirdLine.Visibility = !ShowAdditionalInfo || (aItem.IsGrouped && aItem.Count == 1) || aItem.DisplayField3 == string.Empty ? ViewStates.Gone : ViewStates.Visible;
            TextView technicalInfo = aViewCache.FindViewById<TextView>(Resource.Id.playlistitemtechnicalinfo);
            technicalInfo.Text = aItem.TechnicalInfo;
            technicalInfo.Visibility = technicalInfo.Text.Length > 0 ? ViewStates.Visible : ViewStates.Gone;
        }

        private void JumpToRoomClickHandler(object sender, EventArgs args)
        {
            EventHandler<EventArgs> del = EventJumpToRoomClick;
            if (Room != null && del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        protected override bool CanDeleteItem(PlaylistDisplayItem<Bitmap> aItem, int aPosition)
        {
            return EditMode;
        }

        protected override bool CanMoveItemUp(PlaylistDisplayItem<Bitmap> aItem, int aPosition)
        {
            return EditMode && aItem.CanMoveUp;
        }

        protected override bool CanMoveItemDown(PlaylistDisplayItem<Bitmap> aItem, int aPosition)
        {
            return EditMode && aItem.CanMoveDown;
        }

        protected override int RequestDeleteButtonResourceId
        {
            get
            {
                return Resource.Layout.RequestDeleteButton;
            }
        }

        protected override int ConfirmDeleteButtonResourceId
        {
            get
            {
                return Resource.Layout.ConfirmDeleteButton;
            }
        }

        protected override int MoveUpButtonResourceId
        {
            get
            {
                return Resource.Layout.MoveUpButton;
            }
        }

        protected override int MoveDownButtonResourceId
        {
            get
            {
                return Resource.Layout.MoveDownButton;
            }
        }

        private IconResolver iIconResolver;
        private AndroidImageCache iImageCache;
        private PlaylistDisplayItem<Bitmap> iCurrentTrack;
        private bool iIsGrouping;
        private BitmapDrawable iPlaceholder;
        private ImageButton iCurrentJumpToRoomButton;
    }

    #endregion

    #region Save Playlist Dialog

    public class SavePlaylistDialog
    {
        public SavePlaylistDialog(Stack aStack, IPopupFactory aPopupFactory, View aSaveButton, View aPopupAnchor, Color aPopupBackground, ISaveSupport aSaveSupport, AndroidImageCache aImageCache, IconResolver aIconResolver, bool aShowCancelButton)
        {
            iStack = aStack;
            iContext = aStack.ApplicationContext;
            iSaveSupport = aSaveSupport;
            iImageCache = aImageCache;
            iIconResolver = aIconResolver;
            iSaveSupport.EventImageListChanged += EventImageListChangedHandler;
            iSaveSupport.EventSaveLocationChanged += EventSaveLocationChangedHandler;
            iSaveSupport.EventSaveLocationsChanged += EventSaveLocationsChangedHandler;
            iOptionsView = new OptionsView(iContext, iStack.Invoker, Resource.Drawable.BrowserDown, Android.Graphics.Color.Black, iImageCache, iIconResolver.IconLoading.Image);
            iOptionsView.BackButtonLayoutId = Resource.Layout.BackButton;
            iOptionsView.SaveButtonLayoutId = Resource.Layout.SaveButton;
            iOptionsView.CancelButtonLayoutId = Resource.Layout.CancelButton;
            iOptionsView.ShowSaveButton = true;
            iOptionsView.ShowCancelButton = aShowCancelButton;
            iOptionsView.EventSaveButtonClicked += EventSaveButtonClickedHandler;
            iOptionsView.EventCancelButtonClicked += EventCancelButtonClickedHandler;
            iOptionsView.MasterTitle = "Save Playlist";
            PopulateOptions();

            iPopup = aPopupFactory.CreatePopup(iOptionsView, aPopupAnchor, aPopupBackground);

            if (iPopup is SpeechBubblePopup)
            {
                IWindowManager windowManager = iContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
                int screenWidth = windowManager.DefaultDisplay.Width;
                (iPopup as SpeechBubblePopup).Width = screenWidth / 2;
                (iPopup as SpeechBubblePopup).StretchVertical = true;
            }
            iPopup.Show();
            iPopup.EventDismissed += EventDismissedHandler;
        }

        public void Dismiss()
        {
            if (iPopup != null)
            {
                iPopup.Dismiss();
            }
        }

        public event EventHandler<EventArgs> EventDismissed;

        private void EventSaveLocationChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                PopulateOptions();
            }));
        }

        private void EventSaveLocationsChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                PopulateOptions();
            }));
        }

        private void EventImageListChangedHandler(object sender, EventArgs e)
        {
            iStack.Invoker.BeginInvoke((Action)(() =>
            {
                PopulateOptions();
            }));
        }

        private void EventDismissedHandler(object sender, EventArgs e)
        {
            if (iPopup != null)
            {
                iSaveSupport.EventImageListChanged -= EventImageListChangedHandler;
                iSaveSupport.EventSaveLocationChanged -= EventSaveLocationChangedHandler;
                iSaveSupport.EventSaveLocationsChanged -= EventSaveLocationsChangedHandler;

                iOptionsView.EventSaveButtonClicked -= EventSaveButtonClickedHandler;
                iOptionsView.EventCancelButtonClicked -= EventCancelButtonClickedHandler;
                iPopup.EventDismissed -= EventDismissedHandler;
                iPopup = null;
                OnEventDismissed();
            }
        }

        private void EventCancelButtonClickedHandler(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void EventSaveButtonClickedHandler(object sender, EventArgs e)
        {
            string title = iOptionTitle.Value;
            string description = iOptionDescription.Value;
            string location = iOptionSaveLocation.Value;
            string uri = string.Empty;
            if (iSaveSupport.ImageList.Count > 0)
            {
                uri = iOptionImageList.Value;
            }
            if (title != string.Empty && location != string.Empty)
            {
                uint imageId = 0;
                IDictionary<uint, Uri> imageList = iSaveSupport.ImageList;
                if (uri != string.Empty && imageList != null)
                {
                    foreach (uint key in imageList.Keys)
                    {
                        if (imageList[key].OriginalString == uri)
                        {
                            imageId = key;
                            break;
                        }
                    }
                }
                try
                {
                    iSaveSupport.Save(title, description, imageId);
                    iPopup.Dismiss();
                }
                catch (PlaylistManagerNotFoundException)
                {
                    Toast.MakeText(iContext, "Save failed: could not find PlaylistManager.", ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    UserLog.WriteLine(DateTime.Now + ": Could not save playlist file: " + ex);
                    Toast.MakeText(iContext, "Save failed: " + ex.Message, ToastLength.Long).Show();
                }
            }
        }

        private void PopulateOptions()
        {
            iOptionPage = new OptionPage(string.Empty);

            if (iOptionSaveLocation != null)
            {
                iOptionSaveLocation.EventValueChanged -= EventSaveLocationChangedHandler;
            }
            iOptionSaveLocation = new OptionEnum(string.Empty, "Location", string.Empty);
            if (iSaveSupport.SaveLocations.Count > 0)
            {
                bool found = false;
                foreach (string location in iSaveSupport.SaveLocations)
                {
                    iOptionSaveLocation.Add(location);
                    if (location == iSaveSupport.SaveLocation)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    iSaveSupport.SaveLocation = iOptionSaveLocation.Allowed[0];
                }
                iOptionSaveLocation.Set(iSaveSupport.SaveLocation);
                iOptionPage.Add(iOptionSaveLocation);
            }
            iOptionSaveLocation.EventValueChanged += EventOptionSaveLocationChangedHandler;

            iOptionTitle = new OptionString(string.Empty, "Name", string.Empty, iSaveSupport.DefaultName);
            iOptionPage.Add(iOptionTitle);

            iOptionDescription = new OptionString(string.Empty, "Description", string.Empty, string.Empty);
            iOptionPage.Add(iOptionDescription);

            iOptionImageList = new OptionImage(string.Empty, "Image", string.Empty);
            if (iSaveSupport.ImageList.Count > 0)
            {
                foreach (Uri image in iSaveSupport.ImageList.Values)
                {
                    iOptionImageList.Add(image.OriginalString);
                }
                iOptionPage.Add(iOptionImageList);
            }

            List<IOptionPage> optionPages = new List<IOptionPage>();
            optionPages.Add(iOptionPage);
            iOptionsView.OptionPages = optionPages;
        }

        private void EventOptionSaveLocationChangedHandler(object sender, EventArgs e)
        {
            iSaveSupport.SaveLocation = iOptionSaveLocation.Value;
        }

        private void OnEventDismissed()
        {
            EventHandler<EventArgs> del = EventDismissed;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private ISaveSupport iSaveSupport;
        private Context iContext;
        private OptionPage iOptionPage;
        private OptionEnum iOptionSaveLocation;
        private OptionString iOptionTitle;
        private OptionString iOptionDescription;
        private OptionImage iOptionImageList;
        private OptionsView iOptionsView;
        private IPopup iPopup;
        private Stack iStack;
        private AndroidImageCache iImageCache;
        private IconResolver iIconResolver;
    }

    #endregion

    #region Media Time

    public class ViewWidgetMediaTime : IViewWidgetMediaTime
    {
        public ViewWidgetMediaTime()
        {
        }

        public IPopupControlFactory PopupControlFactory
        {
            set
            {
                Assert.Check(value != null);
                ClosePopup();
                iPopupControlFactory = value;
            }
        }

        public DisplayControl DisplayControl
        {
            set
            {
                if (iDisplayControl != null)
                {
                    iDisplayControl.Click -= DisplayControlClickHandler;
                }
                iDisplayControl = value;
                if (iDisplayControl != null)
                {
                    iDisplayControl.Click += DisplayControlClickHandler;
                }
                UpdateControls();
            }
        }

        public View PopupAnchor
        {
            set
            {
                iPopupAnchor = value;
            }
        }


        #region IViewWidgetMediaTime Members

        public void Open()
        {
        }

        public void Close()
        {
            iInitialised = false;
            iIsUpdating = false;
            UpdateControls();
        }

        public void Initialised()
        {
            iInitialised = true;
            UpdateControls();
        }

        public void SetAllowSeeking(bool aAllowSeeking)
        {
            iAllowSeeking = aAllowSeeking;
            UpdateControls();
        }

        public void SetTransportState(ETransportState aTransportState)
        {
            iTransportState = aTransportState;
            UpdateControls();
        }

        public void SetDuration(uint aDuration)
        {
            iDuration = aDuration;
            Time duration = new Time((int)aDuration);
            float seek = duration.SecondsTotal / 100.0f;
            iSeekAmountPerStep = (uint)Math.Round(seek + 0.5f, MidpointRounding.AwayFromZero);
            UpdateControls();
        }

        public void SetSeconds(uint aSeconds)
        {
            iSeconds = aSeconds;
            UpdateControls();
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;

        #endregion

        #region "private methods"


        private void UpdateControls()
        {
            if (iDisplayControl != null)
            {
                if (iInitialised)
                {
                    iDisplayControl.IsUpdating = iIsUpdating;
                    iDisplayControl.MaxValue = iDuration;
                    iDisplayControl.Value = iSeconds;
                    iDisplayControl.UpdatingValue = iIsUpdating ? iSeekSeconds : 0;
                    iDisplayControl.IsDimmed = iTransportState == ETransportState.ePaused;
                    iDisplayControl.IsIndeterminate = iTransportState == ETransportState.eBuffering;
                    string text = string.Empty;
                    Time duration = new Time((int)iDuration);
                    if (iIsUpdating)
                    {
                        if (iShowTimeRemaining)
                        {
                            text = FormatTime(new Time((int)iSeekSeconds - duration.SecondsTotal), duration);
                        }
                        else
                        {
                            text = FormatTime(new Time((int)iSeekSeconds), duration);
                        }
                    }
                    else if (iTransportState != ETransportState.eStopped && iTransportState != ETransportState.eBuffering)
                    {
                        if (iShowTimeRemaining)
                        {
                            text = FormatTime(new Time((int)iSeconds - duration.SecondsTotal), duration);
                        }
                        else
                        {
                            text = FormatTime(new Time((int)iSeconds), duration);
                        }
                    }
                    iDisplayControl.Text = text;
                }
                else
                {
                    iDisplayControl.IsUpdating = false;
                    iDisplayControl.MaxValue = 0;
                    iDisplayControl.Value = 0;
                    iDisplayControl.UpdatingValue = 0;
                    iDisplayControl.IsDimmed = false;
                    iDisplayControl.IsIndeterminate = false;
                    iDisplayControl.Text = string.Empty;
                }
                iDisplayControl.Enabled = iInitialised;
            }
            if (iPopupControl != null)
            {
                iPopupControl.ToggleButtonState = !iShowTimeRemaining;
                iPopupControl.AllowSeeking = iInitialised && iAllowSeeking;
            }
        }

        private string FormatTime(Time aTime, Time aDuration)
        {
            string result = string.Empty;
            int minutes = ((aTime.Hours % 24) * 60) + aTime.Minutes;
            result = minutes + ":" + string.Format("{0:00}", aTime.Seconds);
            if ((iShowTimeRemaining && aDuration.SecondsTotal > 0) || aTime.SecondsTotal < 0)
            {
                result = "-" + result;
            }
            return result;
        }

        private void DisplayControlClickHandler(object sender, EventArgs e)
        {
            if (!PopupManager.IsShowingPopup)
            {
                OpenPopup();
            }
        }

        private void OpenPopup()
        {
            Assert.Check(iPopupControlFactory != null, "iPopupControlFactory != null");
            Assert.Check(iPopupControl == null, "iPopupControl == null");
            Assert.Check(iPopupAnchor != null, "iPopupAnchor != null");
            iPopupControl = iPopupControlFactory.Create(iPopupAnchor.Context, iPopupAnchor);
            iPopupControl.EventDismissed += EventDismissedHandler;
            iPopupControl.EventUpdateStarted += EventUpdateStartedHandler;
            iPopupControl.EventUpdateFinished += EventUpdateFinishedHandler;
            iPopupControl.EventUpdateCancelled += EventUpdateCancelledHandler;
            iPopupControl.EventIncrement += EventIncrementHandler;
            iPopupControl.EventDecrement += EventDecrementHandler;
            iPopupControl.EventToggleButtonClick += EventToggleButtonClickHandler;
            UpdateControls();
            iPopupControl.Show();
        }

        private void EventIncrementHandler(object sender, EventArgs e)
        {
            iSeekSeconds += iSeekAmountPerStep;
            if (iSeekSeconds > iDuration)
            {
                iSeekSeconds = iDuration;
            }
            UpdateControls();
        }

        private void EventDecrementHandler(object sender, EventArgs e)
        {
            if (iSeekSeconds > iSeekAmountPerStep)
            {
                iSeekSeconds -= iSeekAmountPerStep;
            }
            else
            {
                iSeekSeconds = 0;
            }
            UpdateControls();
        }

        private void EventToggleButtonClickHandler(object sender, EventArgs e)
        {
            iShowTimeRemaining = !iShowTimeRemaining;
            UpdateControls();
        }

        private void EventUpdateStartedHandler(object sender, EventArgs e)
        {
            iIsUpdating = true;
            iSeekSeconds = iSeconds;
        }

        private void EventUpdateFinishedHandler(object sender, EventArgs e)
        {
            iIsUpdating = false;
            OnEventSeekSeconds(iSeekSeconds);
        }

        private void EventUpdateCancelledHandler(object sender, EventArgs e)
        {
            iIsUpdating = false;
        }

        private void EventDismissedHandler(object sender, EventArgs e)
        {
            iPopupControl.EventDismissed -= EventDismissedHandler;
            iPopupControl.EventUpdateStarted -= EventUpdateStartedHandler;
            iPopupControl.EventUpdateFinished -= EventUpdateFinishedHandler;
            iPopupControl.EventUpdateCancelled -= EventUpdateCancelledHandler;
            iPopupControl.EventIncrement -= EventIncrementHandler;
            iPopupControl.EventDecrement -= EventDecrementHandler;
            iPopupControl.EventToggleButtonClick -= EventToggleButtonClickHandler;
            iPopupControl = null;
        }

        private void ClosePopup()
        {
            if (iPopupControl != null)
            {
                iPopupControl.Dismiss();
            }
        }

        private void OnEventSeekSeconds(uint aSeekSeconds)
        {
            EventHandler<EventArgsSeekSeconds> del = EventSeekSeconds;
            if (del != null)
            {
                del(this, new EventArgsSeekSeconds(aSeekSeconds));
            }
        }


        #endregion

        private DisplayControl iDisplayControl;
        private bool iInitialised;
        private IPopupControl iPopupControl;
        private IPopupControlFactory iPopupControlFactory;
        private bool iAllowSeeking;
        private ETransportState iTransportState;
        private uint iDuration;
        private uint iSeconds;
        private bool iShowTimeRemaining;
        private bool iIsUpdating;
        private uint iSeekSeconds;
        private uint iSeekAmountPerStep;
        private View iPopupAnchor;
    }

    #endregion

    #region Volume Control

    public class ViewWidgetVolumeControl : IViewWidgetVolumeControl
    {
        public ViewWidgetVolumeControl()
        {
        }

        public IPopupControlFactory PopupControlFactory
        {
            set
            {
                Assert.Check(value != null);
                ClosePopup();
                iPopupControlFactory = value;
            }
        }

        public DisplayControl DisplayControl
        {
            set
            {
                if (iDisplayControl != null)
                {
                    iDisplayControl.Click -= DisplayControlClickHandler;
                }
                iDisplayControl = value;
                if (iDisplayControl != null)
                {
                    iDisplayControl.Click += DisplayControlClickHandler;
                }
                UpdateControls();
            }
        }

        public View PopupAnchor
        {
            set
            {
                iPopupAnchor = value;
            }
        }

        #region IViewWidgetVolumeControl Members

        public void Open()
        {
        }

        public void Close()
        {
            iInitialised = false;
            iVolumeLimit = 0;
            iVolume = 0;
            iMute = false;
            UpdateControls();
        }

        public void Initialised()
        {
            iInitialised = true;
            UpdateControls();
        }

        public void SetVolume(uint aVolume)
        {
            iVolume = aVolume;
            UpdateControls();
        }

        public void SetMute(bool aMute)
        {
            iMute = aMute;
            UpdateControls();
        }

        public void SetVolumeLimit(uint aVolumeLimit)
        {
            iVolumeLimit = aVolumeLimit;
            UpdateControls();
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;

        public event EventHandler<EventArgs> EventVolumeDecrement;

        public event EventHandler<EventArgsVolume> EventVolumeChanged;

        public event EventHandler<EventArgsMute> EventMuteChanged;

        #endregion

        #region "private methods"

        private void OnEventVolumeIncrement()
        {
            EventHandler<EventArgs> handler = EventVolumeIncrement;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        private void OnEventVolumeDecrement()
        {
            EventHandler<EventArgs> handler = EventVolumeDecrement;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        private void OnEventVolumeChanged(uint aVolume)
        {
            EventHandler<EventArgsVolume> handler = EventVolumeChanged;
            if (handler != null)
            {
                handler(this, new EventArgsVolume(aVolume));
            }
        }
        private void OnEventMuteChanged(bool aMute)
        {
            EventHandler<EventArgsMute> handler = EventMuteChanged;
            if (handler != null)
            {
                handler(this, new EventArgsMute(aMute));
            }
        }

        private void UpdateControls()
        {
            if (iDisplayControl != null)
            {
                iDisplayControl.MaxValue = iVolumeLimit;
                iDisplayControl.Value = iVolume;
                iDisplayControl.IsDimmed = iInitialised && iMute;
                iDisplayControl.Text = iVolume.ToString();
                iDisplayControl.Enabled = iInitialised;
            }
            if (iPopupControl != null)
            {
                iPopupControl.ToggleButtonState = iMute;
                iPopupControl.AllowSeeking = iInitialised;
            }
        }

        private void DisplayControlClickHandler(object sender, EventArgs e)
        {
            if (!PopupManager.IsShowingPopup)
            {
                OpenPopup();
            }
        }

        private void OpenPopup()
        {
            Assert.Check(iPopupControlFactory != null, "iPopupControlFactory != null");
            Assert.Check(iPopupControl == null, "iPopupControl == null");
            Assert.Check(iPopupAnchor != null, "iPopupAnchor != null");
            iPopupControl = iPopupControlFactory.Create(iPopupAnchor.Context, iPopupAnchor);
            iPopupControl.EventDismissed += EventDismissedHandler;
            iPopupControl.EventIncrement += EventIncrementHandler;
            iPopupControl.EventDecrement += EventDecrementHandler;
            iPopupControl.EventToggleButtonClick += EventToggleButtonClickHandler;
            UpdateControls();
            iPopupControl.Show();
        }

        private void EventIncrementHandler(object sender, EventArgs e)
        {
            OnEventVolumeIncrement();
        }

        private void EventDecrementHandler(object sender, EventArgs e)
        {
            OnEventVolumeDecrement();
        }

        private void EventToggleButtonClickHandler(object sender, EventArgs e)
        {
            OnEventMuteChanged(!iMute);
        }

        private void EventDismissedHandler(object sender, EventArgs e)
        {
            iPopupControl.EventDismissed -= EventDismissedHandler;
            iPopupControl.EventIncrement -= EventIncrementHandler;
            iPopupControl.EventDecrement -= EventDecrementHandler;
            iPopupControl.EventToggleButtonClick -= EventToggleButtonClickHandler;
            iPopupControl = null;
        }

        private void ClosePopup()
        {
            if (iPopupControl != null)
            {
                iPopupControl.Dismiss();
            }
        }

        public void IncrementVolume()
        {
            OnEventVolumeIncrement();
        }

        public void DecrementVolume()
        {
            OnEventVolumeDecrement();
        }


        #endregion

        private DisplayControl iDisplayControl;
        private uint iVolume;
        private bool iMute;
        private uint iVolumeLimit;
        private bool iInitialised;
        private IPopupControl iPopupControl;
        private IPopupControlFactory iPopupControlFactory;
        private View iPopupAnchor;
    }

    #endregion

    #region Popup Controls

    public interface IPopupFactory
    {
        IPopup CreatePopup(View aViewRoot, View aAnchor);
        IPopup CreatePopup(View aViewRoot, View aAnchor, Color aBackground);
    }

    public class OverlayPopupFactory : IPopupFactory
    {
        public OverlayPopupFactory(Context aContext, Color aDefaultBackground)
        {
            iContext = aContext;
            iDefaultBackground = aDefaultBackground;
        }

        public IPopup CreatePopup(View aViewRoot, View aAnchor)
        {
            return CreatePopup(aViewRoot, aAnchor, iDefaultBackground);
        }

        public IPopup CreatePopup(View aViewRoot, View aAnchor, Color aBackground)
        {
            return new OverlayPopup(iContext, aViewRoot, aAnchor, aBackground);
        }

        private Context iContext;
        private Color iDefaultBackground;
    }

    public class SpeechBubblePopupFactory : IPopupFactory
    {
        public SpeechBubblePopupFactory(Context aContext, Color aDefaultBackground)
        {
            iContext = aContext;
            iDefaultBackground = aDefaultBackground;
        }

        public IPopup CreatePopup(View aViewRoot, View aAnchor)
        {
            return CreatePopup(aViewRoot, aAnchor, iDefaultBackground);
        }

        public IPopup CreatePopup(View aViewRoot, View aAnchor, Color aBackground)
        {
            Paint stroke = new Paint() { Color = Android.Graphics.Color.White, StrokeWidth = 3 };
            stroke.SetStyle(Paint.Style.Stroke);
            stroke.AntiAlias = true;
            Paint fill = new Paint();
            fill.Color = aBackground != null ? aBackground : iDefaultBackground;
            fill.SetStyle(Paint.Style.Fill);
            return new SpeechBubblePopup(iContext, aViewRoot, aAnchor, stroke, fill);
        }

        private Context iContext;
        private Color iDefaultBackground;
    }

    public interface IPopupControlFactory
    {
        IPopupControl Create(Context aContext, View aAnchor);
    }

    public interface IPopupControl : IPopup
    {
        event EventHandler<EventArgs> EventIncrement;
        event EventHandler<EventArgs> EventDecrement;
        event EventHandler<EventArgs> EventToggleButtonClick;
        event EventHandler<EventArgs> EventUpdateStarted;
        event EventHandler<EventArgs> EventUpdateFinished;
        event EventHandler<EventArgs> EventUpdateCancelled;
        bool ToggleButtonState { set; }
        bool AllowSeeking { set; }
    }

    public class RotaryControlFactory : IPopupControlFactory
    {

        public RotaryControlFactory(IPopupFactory aPopupFactory, Bitmap aCentreButtonImageOn, Bitmap aCentreButtonImageOff)
        {
            iPopupFactory = aPopupFactory;
            iCentreButtonImageOn = aCentreButtonImageOn;
            iCentreButtonImageOff = aCentreButtonImageOff;
        }

        #region IPopupControlFactory Members

        public IPopupControl Create(Context aContext, View aAnchor)
        {
            return new RotaryControl(aContext, aAnchor, iPopupFactory, iCentreButtonImageOn, iCentreButtonImageOff);
        }

        #endregion
        private IPopupFactory iPopupFactory;
        private Bitmap iCentreButtonImageOn;
        private Bitmap iCentreButtonImageOff;
    }

    public class RotaryControl : LinearLayout, IPopupControl
    {
        public RotaryControl(Context aContext, View aAnchor, IPopupFactory aPopupFactory, Bitmap aCentreButtonImageOn, Bitmap aCentreButtonImageOff)
            : base(aContext)
        {
            iAnchor = aAnchor;
            iPopupFactory = aPopupFactory;
            iCentreButtonImageOn = aCentreButtonImageOn;
            iCentreButtonImageOff = aCentreButtonImageOff;
            View content = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.RotaryControl, null);
            AddView(content);
            iGrip = content.FindViewById<ImageView>(Resource.Id.rotarygrip);
            iGrip.SetScaleType(ImageView.ScaleType.Matrix);
            iCentreButton = content.FindViewById<ImageView>(Resource.Id.rotarycentrebutton);
            iWheel = content.FindViewById<ImageView>(Resource.Id.rotarybackground);
        }

        private void CentreButtonClickHandler(object sender, EventArgs e)
        {
            EventToggleButtonClick(this, EventArgs.Empty);
        }

        void TouchHandler(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
            {
                iWheel.Pressed = true;
                if (iTracker == null)
                {
                    iTracker = new RotaryControlTracker(Width, Height);
                }
                iTracker.EventIncremented += EventIncrementedHandler;
                iTracker.EventDecremented += EventDecrementedHandler;
                iTracker.EventAngleUpdated += EventAngleUpdatedHandler;
                iTracker.TrackStart();
                OnEventUpdateStarted();
            }
            else if (e.Event.Action == MotionEventActions.Move)
            {
                Assert.Check(iTracker != null);
                if (iAllowSeeking)
                {
                    iTracker.TrackMoveEvent(e.Event.GetX(), e.Event.GetY());
                }
            }
            else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
            {
                iWheel.Pressed = false;
                Assert.Check(iTracker != null);
                iTracker.EventIncremented -= EventIncrementedHandler;
                iTracker.EventDecremented -= EventDecrementedHandler;
                iTracker.EventAngleUpdated -= EventAngleUpdatedHandler;
                float x = e.Event.GetX();
                float y = e.Event.GetY();
                if (x < 0 || x > Width || y < 0 || y > Height)
                {
                    OnEventUpdateCancelled();
                }
                else
                {
                    OnEventUpdateFinished();
                }
            }
        }

        private void EventIncrementedHandler(object sender, EventArgs e)
        {
            OnEventIncrement();
        }

        private void EventDecrementedHandler(object sender, EventArgs e)
        {
            OnEventDecrement();
        }

        private void EventAngleUpdatedHandler(object sender, EventArgsAngleUpdated e)
        {
            Matrix m = new Matrix();
            m.SetRotate((float)e.Angle, iGrip.Width / 2, iGrip.Height / 2);
            iGrip.ImageMatrix = m;
        }


        #region IPopupControl Members

        public event EventHandler<EventArgs> EventIncrement;

        public event EventHandler<EventArgs> EventDecrement;

        public event EventHandler<EventArgs> EventToggleButtonClick;

        public event EventHandler<EventArgs> EventUpdateStarted;

        public event EventHandler<EventArgs> EventUpdateFinished;

        public event EventHandler<EventArgs> EventUpdateCancelled;

        public bool ToggleButtonState
        {
            set
            {
                iCentreButton.SetImageBitmap(value ? iCentreButtonImageOn : iCentreButtonImageOff);
            }
        }

        public bool AllowSeeking
        {
            set
            {
                iAllowSeeking = value;
            }
        }

        #endregion

        #region IPopup Members

        public void Show()
        {
            Assert.Check(iPopup == null);

            iPopup = iPopupFactory.CreatePopup(this, iAnchor);
            iPopup.EventDismissed += EventDismissedHandler;
            iCentreButton.Click += CentreButtonClickHandler;
            this.Touch += TouchHandler;
            iPopup.Show();
        }

        void EventDismissedHandler(object sender, EventArgs e)
        {
            Assert.Check(iPopup != null);
            iCentreButton.Click -= CentreButtonClickHandler;
            iPopup.EventDismissed -= EventDismissedHandler;
            this.Touch -= TouchHandler;
            OnEventDismissed();
            iPopup = null;
        }

        public void Dismiss()
        {
            if (iPopup != null)
            {
                iPopup.Dismiss();
            }
        }

        public event EventHandler<EventArgs> EventDismissed;

        #endregion

        private void OnEventDismissed()
        {
            EventHandler<EventArgs> del = EventDismissed;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventIncrement()
        {
            EventHandler<EventArgs> del = EventIncrement;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventDecrement()
        {
            EventHandler<EventArgs> del = EventDecrement;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventUpdateStarted()
        {
            EventHandler<EventArgs> del = EventUpdateStarted;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventUpdateFinished()
        {
            EventHandler<EventArgs> del = EventUpdateFinished;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventUpdateCancelled()
        {
            EventHandler<EventArgs> del = EventUpdateCancelled;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private ImageView iGrip;
        private ImageView iCentreButton;
        private ImageView iWheel;
        private Bitmap iCentreButtonImageOn;
        private Bitmap iCentreButtonImageOff;
        private IPopupFactory iPopupFactory;
        private IPopup iPopup;
        private View iAnchor;
        private RotaryControlTracker iTracker;
        private bool iAllowSeeking;
    }

    public class ButtonControlFactory : IPopupControlFactory
    {
        public ButtonControlFactory(IPopupFactory aPopupFactory,
            Bitmap aButton1Image,
            Bitmap aButton2Image,
            Bitmap aButton3Image,
            int aTimerInitialDelay,
            int aTimerInterval,
            IInvoker aInvoker)
        {
            iPopupFactory = aPopupFactory;
            iButton1Image = aButton1Image;
            iButton2Image = aButton2Image;
            iButton3Image = aButton3Image;
            iTimerInitialDelay = aTimerInitialDelay;
            iTimerInterval = aTimerInterval;
            iInvoker = aInvoker;
        }

        public IPopupControl Create(Context aContext, View aAnchor)
        {
            return new ButtonControl(aContext, aAnchor, iPopupFactory, iButton1Image, iButton2Image, iButton3Image, iTimerInitialDelay, iTimerInterval, iInvoker);
        }

        private IPopupFactory iPopupFactory;
        private Bitmap iButton1Image;
        private Bitmap iButton2Image;
        private Bitmap iButton3Image;
        private int iTimerInitialDelay;
        private int iTimerInterval;
        private IInvoker iInvoker;
    }

    public class ButtonControl : LinearLayout, IPopupControl
    {
        public ButtonControl(Context aContext,
            View aAnchor,
            IPopupFactory aPopupFactory,
            Bitmap aButton1Image,
            Bitmap aButton2Image,
            Bitmap aButton3Image,
            int aTimerInitialDelay,
            int aTimerInterval,
            IInvoker aInvoker)
            : base(aContext)
        {
            iTimerInitialDelay = aTimerInitialDelay;
            iTimerInterval = aTimerInterval;
            iInvoker = aInvoker;
            iAnchor = aAnchor;
            iPopupFactory = aPopupFactory;
            iButton1Image = aButton1Image;
            iButton2Image = aButton2Image;
            iButton3Image = aButton3Image;
            View content = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.ButtonControl, null);
            AddView(content);
            iButton1 = content.FindViewById<ImageButton>(Resource.Id.buttoncontrolbutton1);
            iButton1.SetImageBitmap(iButton1Image);
            iButton2 = content.FindViewById<ImageButton>(Resource.Id.buttoncontrolbutton2);
            iButton2.SetImageBitmap(iButton2Image);
            iButton3 = content.FindViewById<ImageButton>(Resource.Id.buttoncontrolbutton3);
            iButton3.SetImageBitmap(iButton3Image);
        }

        #region IPopupControl Members

        public event EventHandler<EventArgs> EventIncrement;

        public event EventHandler<EventArgs> EventDecrement;

        public event EventHandler<EventArgs> EventToggleButtonClick;

        public event EventHandler<EventArgs> EventUpdateStarted;

        public event EventHandler<EventArgs> EventUpdateFinished;

        public event EventHandler<EventArgs> EventUpdateCancelled;

        public bool ToggleButtonState
        {
            set
            {
            }
        }

        public bool AllowSeeking
        {
            set
            {
                iAllowSeeking = value;
            }
        }

        #endregion

        #region IPopup Members

        public void Show()
        {
            Assert.Check(iPopup == null);

            iPopup = iPopupFactory.CreatePopup(this, iAnchor);
            iPopup.EventDismissed += EventDismissedHandler;
            iButton1.Touch += TouchHandler;
            iButton2.Touch += TouchHandler;
            iButton3.Click += ClickHandler;
            iPopup.Show();
        }

        void EventDismissedHandler(object sender, EventArgs e)
        {
            Assert.Check(iPopup != null);
            iPopup.EventDismissed -= EventDismissedHandler;
            iButton1.Touch -= TouchHandler;
            iButton2.Touch -= TouchHandler;
            iButton3.Click -= ClickHandler;
            OnEventDismissed();
            iPopup = null;
        }

        public void Dismiss()
        {
            if (iPopup != null)
            {
                iPopup.Dismiss();
            }
        }

        public event EventHandler<EventArgs> EventDismissed;

        #endregion

        private void OnEventDismissed()
        {
            EventHandler<EventArgs> del = EventDismissed;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventIncrement()
        {
            EventHandler<EventArgs> del = EventIncrement;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventDecrement()
        {
            EventHandler<EventArgs> del = EventDecrement;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventUpdateStarted()
        {
            EventHandler<EventArgs> del = EventUpdateStarted;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventUpdateFinished()
        {
            EventHandler<EventArgs> del = EventUpdateFinished;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventUpdateCancelled()
        {
            EventHandler<EventArgs> del = EventUpdateCancelled;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventToggleButtonClick()
        {
            EventHandler<EventArgs> del = EventToggleButtonClick;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void ClickHandler(object sender, EventArgs e)
        {
            OnEventToggleButtonClick();
        }

        private void TouchHandler(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
            {
                // button down, start timer
                StartTimer(sender as ImageButton);
            }
            else if (iTimer != null)
            {
                // gesture cancelled, cancel timer
                if (e.Event.Action == MotionEventActions.Cancel || e.Event.Action == MotionEventActions.Outside)
                {
                    StopTimer(false);
                }
                else if (e.Event.Action == MotionEventActions.Move)
                {
                    // moved outwith bounds of button, cancel timer
                    if (!HitTest(e, iCurrentButton))
                    {
                        StopTimer(false);
                    }
                }
                else if (e.Event.Action == MotionEventActions.Up)
                {
                    // up over button, indicate success
                    if (HitTest(e, iCurrentButton))
                    {
                        StopTimer(true);
                    }
                    else
                    {
                        // up outwith bounds of button, cancel timer
                        StopTimer(false);
                    }
                }
            }
        }

        private bool HitTest(View.TouchEventArgs e, View aView)
        {
            if (aView == null) return false;
            float x = e.Event.GetX();
            float y = e.Event.GetY();
            Rect outRect = new Rect();
            aView.GetHitRect(outRect);
            return x > (0 - kErrorMargin) && x <= outRect.Width() + kErrorMargin && y > 0 - kErrorMargin && y <= outRect.Height() + kErrorMargin;
        }

        private void StartTimer(ImageButton aButton)
        {
            if (iTimer != null)
            {
                StopTimer(false);
            }
            if (iAllowSeeking)
            {
                iCurrentButton = aButton;
                iCurrentButton.Pressed = true;
                OnEventUpdateStarted();
                iTimer = new System.Threading.Timer(TimerCallback);
                iTimer.Change(iTimerInitialDelay, iTimerInterval);
            }
        }

        private void StopTimer(bool aFinished)
        {
            if (iTimer != null)
            {
                iTimer.Change(Timeout.Infinite, Timeout.Infinite);
                iTimer.Dispose();
                iTimer = null;
                iCurrentButton.Pressed = false;
                iCurrentButton = null;
                if (aFinished)
                {
                    OnEventUpdateFinished();
                }
                else
                {
                    OnEventUpdateCancelled();
                }
            }
        }

        private void TimerCallback(object e)
        {
            iInvoker.BeginInvoke((Action)(() =>
            {
                if (iCurrentButton == iButton1)
                {
                    OnEventDecrement();
                }
                else if (iCurrentButton == iButton2)
                {
                    OnEventIncrement();
                }
            }));
        }


        private ImageButton iCurrentButton;
        private System.Threading.Timer iTimer;
        private View iAnchor;
        private IPopupFactory iPopupFactory;
        private Bitmap iButton1Image;
        private Bitmap iButton2Image;
        private Bitmap iButton3Image;
        private ImageButton iButton1;
        private ImageButton iButton2;
        private ImageButton iButton3;
        private IPopup iPopup;
        private int iTimerInitialDelay;
        private int iTimerInterval;
        private IInvoker iInvoker;
        private bool iAllowSeeking;
        private const int kErrorMargin = 20;
    }

    #endregion

    #region Transport Controls

    public class ViewWidgetTransportControl : IViewWidgetTransportControl
    {

        public ViewWidgetTransportControl()
        {
        }

        public TransportControls TransportControls
        {
            set
            {
                if (iTransportControls != null)
                {
                    iTransportControls.EventPlayPauseStopClick -= EventPlayPauseStopClickHandler;
                    iTransportControls.EventSkipBackClick -= EventSkipBackClickHandler;
                    iTransportControls.EventSkipForwardClick -= EventSkipForwardClickHandler;
                    if (iInitialised)
                    {
                        iTransportControls.Close();
                    }
                }
                iTransportControls = value;
                if (iTransportControls != null)
                {
                    iTransportControls.EventPlayPauseStopClick += EventPlayPauseStopClickHandler;
                    iTransportControls.EventSkipBackClick += EventSkipBackClickHandler;
                    iTransportControls.EventSkipForwardClick += EventSkipForwardClickHandler;
                    if (iInitialised)
                    {
                        iTransportControls.Open();
                    }
                }
                UpdateTransportControls();
            }
        }

        #region IViewWidgetTransportControl Members

        public void Open()
        {
        }

        public void Close()
        {
            if (iInitialised)
            {
                if (iTransportControls != null)
                {
                    iTransportControls.Close();
                }
                iInitialised = false;
                UpdateTransportControls();
            }
        }

        public void Initialised()
        {
            iInitialised = true;
            if (iTransportControls != null)
            {
                iTransportControls.Open();
            }
            UpdateTransportControls();
        }

        public void SetPlayNowEnabled(bool aEnabled) { }

        public void SetPlayNextEnabled(bool aEnabled) { }

        public void SetPlayLaterEnabled(bool aEnabled) { }

        public void SetDragging(bool aDragging) { }

        public void SetTransportState(ETransportState aTransportState)
        {
            iTransportState = aTransportState;
            UpdateTransportControls();
        }

        public void SetDuration(uint aDuration)
        {
            iDuration = aDuration;
            UpdateTransportControls();
        }

        public void SetAllowSkipping(bool aAllowSkipping)
        {
            iAllowSkipping = aAllowSkipping;
            UpdateTransportControls();
        }

        public void SetAllowPausing(bool aAllowPausing)
        {
            iAllowPausing = aAllowPausing;
            UpdateTransportControls();
        }

        public event EventHandler<EventArgs> EventPause;

        public event EventHandler<EventArgs> EventPlay;

        public event EventHandler<EventArgs> EventStop;

        public event EventHandler<EventArgs> EventPrevious;

        public event EventHandler<EventArgs> EventNext;

        public event EventHandler<EventArgsPlay> EventPlayNow;

        public event EventHandler<EventArgsPlay> EventPlayNext;

        public event EventHandler<EventArgsPlay> EventPlayLater;

        #endregion

        private void EventPlayPauseStopClickHandler(object sender, EventArgs e)
        {
            Assert.Check(iInitialised);
            bool usePause = iDuration != 0 && iAllowPausing;
            if (iTransportState == ETransportState.ePlaying)
            {
                if (usePause && EventPause != null)
                {
                    EventPause(this, EventArgs.Empty);
                }
                else if (EventStop != null)
                {
                    EventStop(this, EventArgs.Empty);
                }
            }
            else if (iTransportState == ETransportState.eBuffering && EventStop != null)
            {
                EventStop(this, EventArgs.Empty);
            }
            else if (EventPlay != null)
            {
                EventPlay(this, EventArgs.Empty);
            }
        }

        private void EventSkipBackClickHandler(object sender, EventArgs e)
        {
            Assert.Check(iInitialised);
            if (EventPrevious != null)
            {
                EventPrevious(this, EventArgs.Empty);
            }
        }

        private void EventSkipForwardClickHandler(object sender, EventArgs e)
        {
            Assert.Check(iInitialised);
            if (EventNext != null)
            {
                EventNext(this, EventArgs.Empty);
            }
        }

        private void UpdateTransportControls()
        {
            if (iTransportControls != null)
            {
                TransportControls.EPlayPauseStopButtonState playPauseStopState = TransportControls.EPlayPauseStopButtonState.ShowPlayButton;
                switch (iTransportState)
                {
                    case ETransportState.eBuffering:
                    case ETransportState.ePlaying:
                        playPauseStopState = iAllowPausing && iDuration != 0 ? TransportControls.EPlayPauseStopButtonState.ShowPauseButton : TransportControls.EPlayPauseStopButtonState.ShowStopButton;
                        break;
                    default:
                        break;
                }
                iTransportControls.PlayPauseStopButtonState = playPauseStopState;
                iTransportControls.IsPlayPauseStopEnabled = iInitialised;
                iTransportControls.IsSkipBackEnabled = iInitialised && iAllowSkipping;
                iTransportControls.IsSkipForwardEnabled = iInitialised && iAllowSkipping;
                iTransportControls.IsDimmed = !iInitialised;
            }
        }

        private TransportControls iTransportControls;
        private bool iAllowSkipping;
        private bool iAllowPausing;
        private ETransportState iTransportState;
        private uint iDuration;
        private bool iInitialised;
    }

    public class ToolbarLayoutPhone
    {
        public ToolbarLayoutPhone(ViewGroup aArtworkContainer,
                                  ViewGroup aTrackControls,
                                  ViewGroup aRoomListTitleBar,
                                  ViewGroup aSourceListTitleBar,
                                  ViewGroup aBrowserTitleBar)
        {
            iArtworkContainer = aArtworkContainer;
            iTrackControls = aTrackControls;
            iRoomListTitleBar = aRoomListTitleBar;
            iSourceListTitleBar = aSourceListTitleBar;
            iBrowserTitleBar = aBrowserTitleBar;
        }

        public void Layout(double aScreenHeight)
        {
            double minToolbarHeight = TypedValue.ApplyDimension(ComplexUnitType.Dip,
                              (float)kMinToolbarHeight, iArtworkContainer.Context.Resources.DisplayMetrics);

            double toolbarHeight = Math.Max(minToolbarHeight, aScreenHeight / 15);
            iArtworkContainer.LayoutParameters.Height = (int)toolbarHeight;
            iArtworkContainer.LayoutParameters.Width = (int)toolbarHeight;
            iTrackControls.LayoutParameters.Height = (int)toolbarHeight;
            if (iRoomListTitleBar != null)
            {
                iRoomListTitleBar.LayoutParameters.Height = (int)toolbarHeight;
            }
            if (iSourceListTitleBar != null)
            {
                iSourceListTitleBar.LayoutParameters.Height = (int)toolbarHeight;
            }
            if (iBrowserTitleBar != null)
            {
                iBrowserTitleBar.LayoutParameters.Height = (int)toolbarHeight;
            }
        }

        private ViewGroup iArtworkContainer;
        private ViewGroup iTrackControls;
        private ViewGroup iRoomListTitleBar;
        private ViewGroup iSourceListTitleBar;
        private ViewGroup iBrowserTitleBar;
        private const double kMinToolbarHeight = 45;
    }

    public class ControlsLayout
    {
        public ControlsLayout(RelativeLayout aContainer, RelativeLayout aControls, TransportControls aTransportControls, DisplayControl aVolumeControl, DisplayControl aMediaTimeControl)
        {
            iContainer = aContainer;
            iControls = aControls;
            iTransportControls = aTransportControls;
            iVolumeControl = aVolumeControl;
            iMediaTimeControl = aMediaTimeControl;
        }
        public void Layout(double aContainerWidth, double aWidth)
        {
            double height = aWidth * kTransportControlsRatio;
            iContainer.LayoutParameters.Width = (int)aContainerWidth;
            iContainer.LayoutParameters.Height = (int)height;
            iControls.LayoutParameters.Width = (int)aWidth;
            iControls.LayoutParameters.Height = (int)height;
            iTransportControls.LayoutParameters.Width = (int)(aWidth * kArrayWidthRatio);
            height = (height * kArrayHeightRatio);
            iTransportControls.LayoutParameters.Height = (int)height;
            iVolumeControl.LayoutParameters.Height = (int)height;
            iVolumeControl.LayoutParameters.Width = (int)height;
            iMediaTimeControl.LayoutParameters.Height = (int)height;
            iMediaTimeControl.LayoutParameters.Width = (int)height;
        }
        private RelativeLayout iContainer;
        private RelativeLayout iControls;
        private TransportControls iTransportControls;
        private DisplayControl iVolumeControl;
        private DisplayControl iMediaTimeControl;
        private const double kTransportControlsRatio = (double)100 / (double)400;
        private const double kArrayHeightRatio = (double)84 / (double)100;
        private const double kArrayWidthRatio = (double)243 / (double)400;
    }

    public class TransportControls : RelativeLayout
    {

        public event EventHandler<EventArgs> EventSkipBackClick;
        public event EventHandler<EventArgs> EventSkipForwardClick;
        public event EventHandler<EventArgs> EventPlayPauseStopClick;

        public TransportControls(IntPtr aHandle, JniHandleOwnership aHandleOwnership)
            : base(aHandle, aHandleOwnership)
        {
            Init();
        }

        public TransportControls(Context aContext, IAttributeSet aAttributeSet)
            : base(aContext, aAttributeSet)
        {
            Init();
        }

        private void Init()
        {
            iBackground = new ImageView(Context);
            Stack stack = (this.Context.ApplicationContext as Stack);
            iBackground.SetImageBitmap(stack.ResourceManager.GetBitmap(Resource.Drawable.Array));
            RelativeLayout.LayoutParams backgroundParams = new RelativeLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            backgroundParams.AddRule(LayoutRules.CenterHorizontal);
            backgroundParams.AddRule(LayoutRules.CenterVertical);
            iBackground.LayoutParameters = backgroundParams;
            AddView(iBackground);

            iSkipBackButton = new ImageButton(Context);
            iSkipBackButton.Id = kSkipBackButtonId;

            iSkipForwardButton = new ImageButton(Context);
            iSkipForwardButton.Id = kSkipForwardButtonId;


            iPlayPauseStopButton = new ImageButton(Context);
            iPlayPauseStopButton.Id = kPlayPauseStopButtonId;

            RelativeLayout.LayoutParams playPauseStopButtonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //playPauseStopButtonLayoutParams.TopMargin = 20;
            playPauseStopButtonLayoutParams.AddRule(LayoutRules.CenterHorizontal);
            playPauseStopButtonLayoutParams.AddRule(LayoutRules.CenterVertical);
            iPlayPauseStopButton.LayoutParameters = playPauseStopButtonLayoutParams;

            RelativeLayout.LayoutParams skipBackButtonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //skipBackButtonLayoutParams.TopMargin = 40;
            skipBackButtonLayoutParams.AddRule(LayoutRules.LeftOf, kPlayPauseStopButtonId);
            skipBackButtonLayoutParams.AddRule(LayoutRules.CenterVertical);
            iSkipBackButton.LayoutParameters = skipBackButtonLayoutParams;

            RelativeLayout.LayoutParams skipForwardButtonLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //skipForwardButtonLayoutParams.TopMargin = 40;
            skipForwardButtonLayoutParams.AddRule(LayoutRules.RightOf, kPlayPauseStopButtonId);
            skipForwardButtonLayoutParams.AddRule(LayoutRules.CenterVertical);
            iSkipForwardButton.LayoutParameters = skipForwardButtonLayoutParams;


            iSkipBackButton.SetBackgroundResource(Resource.Drawable.SkipBackButton);
            iSkipForwardButton.SetBackgroundResource(Resource.Drawable.SkipForwardButton);
            iPlayPauseStopButton.SetBackgroundResource(Resource.Drawable.PlayButton);

            AddView(iSkipBackButton);
            AddView(iPlayPauseStopButton);
            AddView(iSkipForwardButton);

            IsSkipBackEnabled = false;
            IsSkipForwardEnabled = false;
            IsPlayPauseStopEnabled = false;
            IsDimmed = true;
        }

        public void Open()
        {
            iSkipBackButton.Click += SkipBackClick;
            iSkipForwardButton.Click += SkipForwardClick;
            iPlayPauseStopButton.Click += PlayPauseStopClick;
        }

        public void Close()
        {
            iSkipBackButton.Click -= SkipBackClick;
            iSkipForwardButton.Click -= SkipForwardClick;
            iPlayPauseStopButton.Click -= PlayPauseStopClick;
        }

        protected override void Dispose(bool disposing)
        {
            iSkipBackButton.Dispose();
            iSkipBackButton = null;
            iSkipForwardButton.Dispose();
            iSkipForwardButton = null;
            iPlayPauseStopButton.Dispose();
            iPlayPauseStopButton = null;
            iBackground.Dispose();
            iBackground = null;
            //RemoveAllViews();
            base.Dispose(disposing);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int measuredHeight = this.MeasuredHeight;
            iPlayPauseStopButton.Measure(MeasureSpec.MakeMeasureSpec(measuredHeight, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec(measuredHeight, MeasureSpecMode.Exactly));

            double skipButtonHeight = this.MeasuredHeight * kSkipButtonHeightRatio;
            double actualWidth = this.MeasuredHeight * kWidthHeightRatio;
            double skipButtonWidth = actualWidth * kSkipButtonWidthRatio;

            iSkipBackButton.Measure(MeasureSpec.MakeMeasureSpec((int)skipButtonHeight, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec((int)skipButtonWidth, MeasureSpecMode.Exactly));
            iSkipForwardButton.Measure(MeasureSpec.MakeMeasureSpec((int)skipButtonHeight, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec((int)skipButtonWidth, MeasureSpecMode.Exactly));

        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            //base.OnLayout(changed, l, t, r, b);

            int centreX = ((r - l) / 2);
            int centreY = ((b - t) / 2);

            int playPauseStopHeight = this.MeasuredHeight;
            int playPauseStopWidth = this.MeasuredHeight;

            int playPauseStopLeft = centreX - (playPauseStopWidth / 2);
            int playPauseStopTop = centreY - (playPauseStopHeight / 2);

            iPlayPauseStopButton.Layout(playPauseStopLeft, playPauseStopTop, playPauseStopLeft + playPauseStopWidth, playPauseStopTop + playPauseStopHeight);

            int backgroundHeight = iBackground.MeasuredHeight;
            int backgroundWidth = iBackground.MeasuredWidth;

            int backgroundLeft = centreX - (backgroundWidth / 2);
            int backgroundTop = centreY - (backgroundHeight / 2);
            iBackground.Layout(backgroundLeft, backgroundTop, backgroundLeft + backgroundWidth, backgroundTop + backgroundHeight);


            double skipButtonHeight = this.MeasuredHeight * kSkipButtonHeightRatio;
            double actualWidth = this.MeasuredHeight * kWidthHeightRatio;
            double skipButtonWidth = actualWidth * kSkipButtonWidthRatio;


            int skipBackLeft = playPauseStopLeft - (int)skipButtonWidth;
            int skipBackTop = centreY - (int)(skipButtonHeight / 2);
            iSkipBackButton.Layout(skipBackLeft, skipBackTop, skipBackLeft + (int)skipButtonWidth, skipBackTop + (int)skipButtonHeight);

            int skipForwardLeft = playPauseStopLeft + playPauseStopWidth;
            int skipForwardTop = centreY - (int)(skipButtonHeight / 2);
            iSkipForwardButton.Layout(skipForwardLeft, skipForwardTop, skipForwardLeft + (int)skipButtonWidth, skipForwardTop + (int)skipButtonHeight);
        }

        public bool IsDimmed
        {
            set
            {
                iIsDimmed = value;
                ClearAnimation();
                AlphaAnimation anim = new AlphaAnimation(value ? 1 : 0.5f, value ? 0.5f : 1);
                anim.Duration = 0;
                anim.FillAfter = true;
                StartAnimation(anim);
                anim.Dispose();
            }
        }

        private void SkipBackClick(object sender, EventArgs e)
        {
            OnEventSkipBackClick();
        }

        private void SkipForwardClick(object sender, EventArgs e)
        {
            OnEventSkipForwardClick();
        }

        private void PlayPauseStopClick(object sender, EventArgs e)
        {
            OnEventPlayPauseStopClick();
        }

        public bool IsSkipBackEnabled
        {
            get
            {
                return iSkipBackButton.Enabled;
            }
            set
            {
                iSkipBackButton.Enabled = value;
                iSkipBackButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        public bool IsSkipForwardEnabled
        {
            get
            {
                return iSkipForwardButton.Enabled;
            }
            set
            {
                iSkipForwardButton.Enabled = value;
                iSkipForwardButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        public bool IsPlayPauseStopEnabled
        {
            get
            {
                return iPlayPauseStopButton.Enabled;
            }
            set
            {
                iPlayPauseStopButton.Enabled = value;
                iPlayPauseStopButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        public EPlayPauseStopButtonState PlayPauseStopButtonState
        {
            set
            {
                int resource = -1;
                if (value == EPlayPauseStopButtonState.ShowPlayButton)
                {
                    resource = Resource.Drawable.PlayButton;
                }
                else if (value == EPlayPauseStopButtonState.ShowPauseButton)
                {
                    resource = Resource.Drawable.PauseButton;
                }
                else if (value == EPlayPauseStopButtonState.ShowStopButton)
                {
                    resource = Resource.Drawable.StopButton;
                }
                else
                {
                    Assert.Check(false);
                }
                iPlayPauseStopButton.SetBackgroundResource(resource);
            }
        }

        protected void OnEventSkipBackClick()
        {
            EventHandler<EventArgs> eventClick = EventSkipBackClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }

        protected void OnEventSkipForwardClick()
        {
            EventHandler<EventArgs> eventClick = EventSkipForwardClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }

        protected void OnEventPlayPauseStopClick()
        {
            EventHandler<EventArgs> eventClick = EventPlayPauseStopClick;
            if (eventClick != null)
            {
                eventClick(this, EventArgs.Empty);
            }
        }

        private ImageView iBackground;
        private ImageButton iSkipBackButton;
        private ImageButton iSkipForwardButton;
        private ImageButton iPlayPauseStopButton;
        private const int kSkipBackButtonId = 1000;
        private const int kSkipForwardButtonId = 1001;
        private const int kPlayPauseStopButtonId = 1002;
        private bool iIsDimmed;
        private const double kSkipButtonHeightRatio = 39d / 84d;
        private const double kSkipButtonWidthRatio = 50d / 243d;
        private const double kWidthHeightRatio = 234d / 84d;


        public enum EPlayPauseStopButtonState
        {
            ShowPlayButton,
            ShowPauseButton,
            ShowStopButton
        }
    }

    #endregion

    #region Display Controls

    public class DisplayControl : RelativeLayout
    {
        public DisplayControl(IntPtr aHandle, JniHandleOwnership aHandleOwnership)
            : base(aHandle, aHandleOwnership)
        {
            Init();
        }

        public DisplayControl(Context aContext, IAttributeSet aAttributeSet)
            : base(aContext, aAttributeSet)
        {
            Init();
        }

        private void Init()
        {
            SetWillNotDraw(false);

            iPaint = new Paint();
            iPaint.StrokeWidth = kStrokeWidth;
            iPaint.AntiAlias = true;
            iPaint.SetStyle(Paint.Style.Stroke);
            iPaint.Color = kStrokeColour;

            iUpdatingPaint = new Paint();
            iUpdatingPaint.StrokeWidth = kStrokeWidth;
            iUpdatingPaint.Color = kUpdatingStrokeColour;
            iUpdatingPaint.AntiAlias = true;
            iUpdatingPaint.SetStyle(Paint.Style.Stroke);

            Bitmap background = (this.Context.ApplicationContext as Stack).ResourceManager.GetBitmap(Resource.Drawable.Wheel);
            ImageView backgroundImage = new ImageView(Context);
            RelativeLayout.LayoutParams backgroundLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            backgroundImage.LayoutParameters = backgroundLayoutParams;
            backgroundImage.SetImageBitmap(background);
            AddView(backgroundImage);

            iThrobber = new Throbber(Context);
            RelativeLayout.LayoutParams throbberLayoutParams = new RelativeLayout.LayoutParams(kThrobberDiameter, kThrobberDiameter);
            throbberLayoutParams.AddRule(LayoutRules.CenterHorizontal);
            throbberLayoutParams.AddRule(LayoutRules.CenterVertical);

            iThrobber.LayoutParameters = throbberLayoutParams;
            AddView(iThrobber);



            iDimmer = new ImageView(Context);
            iDimmer.SetImageBitmap((this.Context.ApplicationContext as Stack).ResourceManager.GetBitmap(Resource.Drawable.WheelMute));
            RelativeLayout.LayoutParams dimmerLayoutParams = new RelativeLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            dimmerLayoutParams.AddRule(LayoutRules.CenterHorizontal);
            dimmerLayoutParams.AddRule(LayoutRules.CenterVertical);
            iDimmer.LayoutParameters = dimmerLayoutParams;
            AddView(iDimmer);

            iText = string.Empty;
            iTextPaint = new Paint();
            iTextPaint.AntiAlias = true;
            iTextPaint.StrokeWidth = 5;
            iTextPaint.StrokeCap = Paint.Cap.Round;
            iTextPaint.TextSize = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip,
                              kFontSize, Context.Resources.DisplayMetrics);
            iTextPaint.SetTypeface(new TextView(Context).Typeface);

            IsIndeterminate = false;
            IsDimmed = true;
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                bool changed = base.Enabled != value;
                if (changed)
                {
                    base.Enabled = value;
                    iInvalid = true;
                    Invalidate();
                }
            }
        }

        private void RecalculateDrawables(Canvas aCanvas)
        {
            float value = iValue;
            float maxValue = iMaxValue;
            float updatingValue = iUpdatingValue;
            float innerRingDiameter = this.Height * kInnerRingRatio;

            if (maxValue > 0)
            {
                if (value >= maxValue)
                {
                    value = maxValue - 0.1f;
                }
                iSweepAngleValue = 360f * (value / maxValue);
                iStartAngleValue = 90f;

                float startX = ((float)this.Width / 2f) - (innerRingDiameter / 2f);
                float startY = ((float)this.Height / 2f) - (innerRingDiameter / 2f);
                if (iArcRect != null)
                {
                    iArcRect.Dispose();
                }
                iArcRect = new RectF(startX, startY, startX + innerRingDiameter, startY + innerRingDiameter);

                iDrawUpdatingArc = updatingValue != value && iIsUpdating;
                if (iDrawUpdatingArc)
                {
                    iStartAngleUpdating = iStartAngleValue + iSweepAngleValue;
                    iSweepAngleUpdating = 360f * ((updatingValue - value) / maxValue);
                }
            }

            if (Enabled)
            {
                iTextPaint.Color = iDimmer.Visibility == ViewStates.Visible ? Color.Gray : Color.White;
                if (iTextBounds != null)
                {
                    iTextBounds.Dispose();
                    iTextBounds = null;
                }
                iTextBounds = new Rect();
                iTextPaint.GetTextBounds(iText, 0, iText.Length, iTextBounds);
                while (iTextBounds.Width() > innerRingDiameter && iText.Length > 0)
                {
                    iText = iText.Substring(1);
                    iTextPaint.GetTextBounds(iText, 0, iText.Length, iTextBounds);
                }

            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (iInvalid || iWidth != canvas.Width || iHeight != canvas.Height)
            {
                RecalculateDrawables(canvas);
                iInvalid = false;
                iWidth = canvas.Width;
                iHeight = canvas.Height;
            }

            if (iMaxValue != 0)
            {
                canvas.DrawArc(iArcRect, iStartAngleValue, iSweepAngleValue, false, iPaint);
                if (iDrawUpdatingArc)
                {
                    canvas.DrawArc(iArcRect, iStartAngleUpdating, iSweepAngleUpdating, false, iUpdatingPaint);
                }
            }

            if (Enabled)
            {
                canvas.DrawText(iText, (this.Width / 2) - (iTextBounds.Width() / 2), (this.Height / 2) + (iTextBounds.Height() / 2), iTextPaint);
            }
        }

        public string Text
        {
            set
            {
                bool changed = iText != value;
                if (changed)
                {
                    iText = value;
                    iInvalid = true;
                    this.Invalidate();
                }
            }
        }

        public float Value
        {
            set
            {
                bool changed = iValue != value;
                if (changed)
                {
                    iValue = value;
                    iInvalid = true;
                    this.Invalidate();
                }
            }
        }

        public float MaxValue
        {
            set
            {
                bool changed = iMaxValue != value;
                if (changed)
                {
                    iMaxValue = value;
                    iInvalid = true;
                    this.Invalidate();
                }
            }
        }

        public float UpdatingValue
        {
            set
            {
                bool changed = iUpdatingValue != value;
                if (changed)
                {
                    iUpdatingValue = value;
                    iInvalid = true;
                    this.Invalidate();
                }
            }
        }

        public bool IsUpdating
        {
            set
            {
                bool changed = iIsUpdating != value;
                if (changed)
                {
                    iIsUpdating = value;
                    iInvalid = true;
                    this.Invalidate();
                }
            }
        }

        public bool IsIndeterminate
        {
            set
            {
                bool changed = iThrobber.IsShowing != value;
                if (changed)
                {
                    iThrobber.IsShowing = value;
                    iInvalid = true;
                    Invalidate();
                }
            }
        }

        public bool IsDimmed
        {
            set
            {
                iDimmer.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
                iInvalid = true;
                Invalidate();
            }
        }

        private bool iInvalid = true;
        private float iValue;
        private float iMaxValue;
        private float iUpdatingValue;
        private Paint iPaint;
        private Paint iUpdatingPaint;
        private const int kStrokeWidth = 3;
        private static Color kStrokeColour = new Color(71, 172, 220);
        private static Color kUpdatingStrokeColour = new Color(187, 187, 0);
        private const int kThrobberDiameter = 80;
        private const int kFontSize = 14;
        private Throbber iThrobber;
        private string iText;
        private ImageView iDimmer;
        private bool iIsUpdating;
        private Paint iTextPaint;
        private int iWidth, iHeight;
        private Rect iTextBounds;
        private RectF iArcRect;
        private float iStartAngleValue, iSweepAngleValue, iStartAngleUpdating, iSweepAngleUpdating;
        private bool iDrawUpdatingArc;
        private const float kInnerRingRatio = 0.7f;
    }

    #endregion

    #region Throbber

    public class Throbber : RelativeLayout
    {

        public Throbber(Context aContext)
            : base(aContext)
        {
            Init();
        }

        public Throbber(IntPtr aHandle, JniHandleOwnership aHandleOwnership)
            : base(aHandle, aHandleOwnership)
        {
            Init();
        }

        public Throbber(Context aContext, IAttributeSet aAttributeSet)
            : base(aContext, aAttributeSet)
        {
            Init();
        }

        public bool IsShowing
        {
            get
            {
                return iIsShowing;
            }
            set
            {
                bool changed = iIsShowing != value;
                iIsShowing = value;
                this.Visibility = value ? ViewStates.Visible : ViewStates.Invisible;
                if (changed && value)
                {
                    (iForegroundImageView.Drawable as AnimationDrawable).Start();
                }
                else if (changed)
                {
                    (iForegroundImageView.Drawable as AnimationDrawable).Stop();
                }
            }
        }

        private void Init()
        {
            Bitmap backgroundImage = (this.Context.ApplicationContext as Stack).ResourceManager.GetBitmap(Resource.Drawable.HourGlass);
            Bitmap foregroundImage = (this.Context.ApplicationContext as Stack).ResourceManager.GetBitmap(Resource.Drawable.HourGlass2);

            iBackgroundView = new ImageView(Context);
            iBackgroundView.LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            iBackgroundView.SetImageBitmap(backgroundImage);
            AddView(iBackgroundView);

            AnimationDrawable anim = new AnimationDrawable();
            anim.OneShot = false;
            for (int i = 0; i < kFrameCount; i++)
            {
                Bitmap tempImage = Bitmap.CreateBitmap(foregroundImage.Width, foregroundImage.Height, Bitmap.Config.Argb8888);
                Canvas tempCanvas = new Canvas(tempImage);
                tempCanvas.Rotate(i * 45, foregroundImage.Width / 2, foregroundImage.Height / 2);
                tempCanvas.DrawBitmap(foregroundImage, 0, 0, null);
                Drawable frame = new BitmapDrawable(tempImage);
                anim.AddFrame(frame, kDuration / kFrameCount);
                tempCanvas.Dispose();
                tempImage.Dispose();
                frame.Dispose();
            }

            iForegroundImageView = new ImageView(Context);
            iForegroundImageView.LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            iForegroundImageView.SetImageDrawable(anim);
            anim.Dispose();
            AddView(iForegroundImageView);

            IsShowing = false;
        }

        protected override void Dispose(bool disposing)
        {
            //ViewGroup.LayoutParams parms = iForegroundImageView.LayoutParameters;
            iForegroundImageView.Dispose();
            //parms.Dispose();
            //parms = iBackgroundView.LayoutParameters;
            iBackgroundView.Dispose();
            //parms.Dispose();
            base.Dispose(disposing);
        }

        private const int kFrameCount = 8;
        private const int kDuration = 500;
        private ImageView iForegroundImageView;
        private ImageView iBackgroundView;
        private bool iIsShowing;
    }

    #endregion

    #region Room And Source Selectors

    public interface IRoomSourceMediator
    {
        void Close();
    }

    public class RoomSourceListsMediator : IRoomSourceMediator
    {
        public RoomSourceListsMediator(Context aContext, Stack aStack, ListView aRoomList, ListView aSourceList, View aRefreshButton, Throbber aRefreshThrobber, ViewSwitcher aViewSwitcher, View aStandbyButton, View aBackButton, TextView aSourceListTitle, IconResolver aIconResolver, ViewWidgetSelectorRoom aRoomSelector, ViewWidgetSelector<Source> aSourceSelector, ViewPager aViewPager, ToggleButton aStandbyAllButton)
        {
            iContext = aContext;
            iStack = aStack;
            iRoomList = aRoomList;
            iSourceList = aSourceList;
            iRefreshButton = aRefreshButton;
            iRefreshThrobber = aRefreshThrobber;
            iViewSwitcher = aViewSwitcher;
            iStandbyButton = aStandbyButton;
            iBackButton = aBackButton;
            iSourceListTitle = aSourceListTitle;
            iIconResolver = aIconResolver;
            iRoomSelector = aRoomSelector;
            iSourceSelector = aSourceSelector;
            iStandbyAllButton = aStandbyAllButton;

            iRoomAdapter = new ViewWidgetRoomAdapter(iContext, iRoomSelector, iStack.Invoker, iIconResolver);
            iRoomAdapter.EventUserSelectedItem += RoomAdapterEventUserSelectedItem;
            iSourceAdapter = new ViewWidgetSourceAdapter(iContext, iSourceSelector, iStack.Invoker, iIconResolver);
            iSourceAdapter.EventUserSelectedItem += SourceAdapterEventUserSelectedItem;
            iRoomAdapter.ListView = iRoomList;
            iSourceAdapter.ListView = iSourceList;
            iRefreshButton.Click += RefreshClickHandler;
            iStandbyButton.Click += StandbyClickHandler;
            iBackButton.Click += BackClickHandler;
            iRoomSelector.EventDataChanged += RoomSelector_EventDataChanged;
            iIsShowingRooms = true;
            iRoomSelector.StandbyAllButton = iStandbyAllButton;
            iViewPager = aViewPager;
            RoomChanged();
        }

        public bool IsShowingRooms
        {
            get { return iIsShowingRooms; }
            set
            {
                if (iIsShowingRooms != value)
                {
                    if (iIsShowingRooms && iRoomSelector.SelectedItem != null)
                    {
                        ShowSources();
                    }
                    else if (!iIsShowingRooms)
                    {
                        ShowRooms();
                        iRoomSelector.SelectedItem = null;
                    }
                }
            }
        }


        private void RoomSelector_EventDataChanged(object sender, EventArgs e)
        {
            RoomChanged();
        }

        private void RoomChanged()
        {
            Room room = iRoomSelector.SelectedItem;
            if (room != null)
            {
                ShowSources();
            }
            else
            {
                ShowRooms();
            }
            iSourceListTitle.Text = room == null ? "Sources" : room.Name;
        }

        private void StandbyClickHandler(object sender, EventArgs e)
        {
            Room room = iRoomSelector.SelectedItem;
            if (room != null)
            {
                room.Standby = true;
                ShowRooms();
                iRoomSelector.SelectedItem = null;
            }
        }

        private void BackClickHandler(object sender, EventArgs e)
        {
            ShowRooms();
            iRoomSelector.SelectedItem = null;
        }

        private void RefreshClickHandler(object sender, EventArgs e)
        {
            iRefreshTimer = new System.Threading.Timer((a) =>
            {
                iStack.Invoker.BeginInvoke((Action)(() =>
                {
                    if (iRefreshTimer != null)
                    {
                        iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        iRefreshTimer.Dispose();
                        iRefreshTimer = null;
                    }
                    if (iRefreshButton != null)
                    {
                        iRefreshButton.Visibility = ViewStates.Visible;
                    }
                    if (iRefreshThrobber != null)
                    {
                        iRefreshThrobber.Visibility = ViewStates.Gone;
                        iRefreshThrobber.Enabled = false;
                    }
                }));
            });
            iRefreshTimer.Change(kRefreshTimeout, Timeout.Infinite);
            iRefreshButton.Visibility = ViewStates.Gone;
            iRefreshThrobber.IsShowing = true;
            iStack.Rescan();
        }

        public void Close()
        {
            iRoomSelector.StandbyAllButton = null;
            iRoomAdapter.EventUserSelectedItem -= RoomAdapterEventUserSelectedItem;
            iSourceAdapter.EventUserSelectedItem -= SourceAdapterEventUserSelectedItem;
            iRefreshButton.Click -= RefreshClickHandler;
            iStandbyButton.Click -= StandbyClickHandler;
            iBackButton.Click -= BackClickHandler;
            if (iRefreshTimer != null)
            {
                iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                iRefreshTimer.Dispose();
                iRefreshTimer = null;
            }
            iRefreshButton.Visibility = ViewStates.Visible;
            iRefreshThrobber.IsShowing = false;
            iRoomSelector.EventDataChanged -= RoomSelector_EventDataChanged;
            iRoomAdapter.ListView = null;
            iSourceAdapter.ListView = null;
        }

        private void RoomAdapterEventUserSelectedItem(object sender, EventArgs e)
        {
            ShowSources();
        }

        private void SourceAdapterEventUserSelectedItem(object sender, EventArgs e)
        {
            iViewPager.CurrentItem = (int)EPageIndex.NowPlaying;
        }

        private void ShowRooms()
        {
            if (!iIsShowingRooms)
            {
                iIsShowingRooms = true;
                using (Animation inAnim = CreateTranslateAnimation(-1, 0))
                {
                    iViewSwitcher.InAnimation = inAnim;
                }
                using (Animation outAnim = CreateTranslateAnimation(0, 1))
                {
                    iViewSwitcher.OutAnimation = outAnim;
                }
                iViewSwitcher.DisplayedChild = 0;
            }
        }

        private void ShowSources()
        {
            if (iIsShowingRooms)
            {
                iIsShowingRooms = false;
                using (Animation inAnim = CreateTranslateAnimation(1, 0))
                {
                    iViewSwitcher.InAnimation = inAnim;
                }
                using (Animation outAnim = CreateTranslateAnimation(0, -1))
                {
                    iViewSwitcher.OutAnimation = outAnim;
                }
                iViewSwitcher.DisplayedChild = 1;
            }
        }

        private Animation CreateTranslateAnimation(float fromX, float toX)
        {
            TranslateAnimation anim = new TranslateAnimation(Dimension.RelativeToParent, fromX, Dimension.RelativeToParent, toX, Dimension.RelativeToParent, 0f, Dimension.RelativeToParent, 0f);
            anim.Duration = 500;
            using (DecelerateInterpolator interpolator = new Android.Views.Animations.DecelerateInterpolator())
            {
                anim.Interpolator = interpolator;
            }
            return anim;
        }

        private ListView iRoomList;
        private ListView iSourceList;
        private Context iContext;
        private ViewWidgetRoomAdapter iRoomAdapter;
        private ViewWidgetSourceAdapter iSourceAdapter;
        private View iRefreshButton;
        private Throbber iRefreshThrobber;
        private Stack iStack;
        private System.Threading.Timer iRefreshTimer;
        private const int kRefreshTimeout = 5000;
        private ViewSwitcher iViewSwitcher;
        private bool iIsShowingRooms;
        private View iStandbyButton;
        private View iBackButton;
        private TextView iSourceListTitle;
        private ViewWidgetSelectorRoom iRoomSelector;
        private ViewWidgetSelector<Source> iSourceSelector;
        private IconResolver iIconResolver;
        private ViewPager iViewPager;
        private ToggleButton iStandbyAllButton;
    }

    public class RoomSourcePopupsMediator : IRoomSourceMediator
    {
        public RoomSourcePopupsMediator(Context aContext, Stack aStack, Button aRoomButton, Button aSourceButton, IPopupFactory aPopupFactory, IconResolver aIconResolver, ViewWidgetSelectorRoom aRoomSelector, ViewWidgetSelector<Source> aSourceSelector)
        {
            iStack = aStack;
            iContext = aContext;
            iPopupFactory = aPopupFactory;
            iIconResolver = aIconResolver;
            iRoomSelector = aRoomSelector;
            iSourceSelector = aSourceSelector;
            iRoomAdapter = new ViewWidgetRoomAdapter(aContext, iRoomSelector, aStack.Invoker, iIconResolver);
            iSourceAdapter = new ViewWidgetSourceAdapter(aContext, iSourceSelector, aStack.Invoker, iIconResolver);
            iRoomButton = aRoomButton;
            iSourceButton = aSourceButton;
            iRoomButton.Enabled = true;
            iSourceButton.Enabled = true;
            iRoomAdapter.SelectorButton = iRoomButton;
            iSourceAdapter.SelectorButton = iSourceButton;
            iRoomButton.Click += selectRoom_Click;
            iSourceButton.Click += selectSource_Click;
        }

        public void Close()
        {
            if (iRefreshTimer != null)
            {
                iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                iRefreshTimer.Dispose();
                iRefreshTimer = null;
            }
            if (iPopup != null)
            {
                iPopup.Dismiss();
            }
            iRoomButton.Click -= selectRoom_Click;
            iSourceButton.Click -= selectSource_Click;
            iRoomAdapter.Close();
            iSourceAdapter.Close();
            iRoomButton.Enabled = false;
            iSourceButton.Enabled = false;
            iRoomButton.Text = string.Empty;
            iSourceButton.Text = string.Empty;
        }

        private void selectRoom_Click(object sender, EventArgs e)
        {
            if (!PopupManager.IsShowingPopup)
            {
                LayoutInflater inflater = (LayoutInflater)iContext.GetSystemService(Context.LayoutInflaterService);
                View popupView = inflater.Inflate(Resource.Layout.RoomList, null, false);
                ListView listView = popupView.FindViewById<ListView>(Resource.Id.roomlist);
                listView.DividerHeight = 0;
                iRoomAdapter.ListView = listView;
                iRoomAdapter.EventUserSelectedItem += iRoomAdapter_EventUserSelectedItem;
                iRefreshButton = popupView.FindViewById<ImageButton>(Resource.Id.roomsrefreshbutton);
                iRefreshThrobber = popupView.FindViewById<Throbber>(Resource.Id.roomsrefreshthrobber);
                iRefreshButton.Click += RefreshClickHandler;
                iRoomSelector.StandbyAllButton = popupView.FindViewById<ToggleButton>(Resource.Id.standbybuttonall);
                iPopup = ShowPopup(popupView, sender as View);
                iPopup.EventDismissed += RoomPopup_EventDismissed;
            }
        }

        private void iRoomAdapter_EventUserSelectedItem(object sender, EventArgs e)
        {
            Assert.Check(iPopup != null);
            iPopup.Dismiss();
        }

        private void RoomPopup_EventDismissed(object sender, EventArgs e)
        {
            if (iRefreshTimer != null)
            {
                iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                iRefreshTimer.Dispose();
                iRefreshTimer = null;
            }
            if (iRefreshButton != null)
            {
                iRefreshButton.Click -= RefreshClickHandler;
                iRefreshButton.Dispose();
                iRefreshButton = null;
            }
            if (iRefreshThrobber != null)
            {
                iRefreshThrobber.Dispose();
                iRefreshThrobber = null;
            }
            iRoomSelector.StandbyAllButton = null;
            iRoomAdapter.EventUserSelectedItem -= iRoomAdapter_EventUserSelectedItem;
            iRoomAdapter.ListView = null;
            if (iPopup != null)
            {
                iPopup.EventDismissed -= RoomPopup_EventDismissed;
                iPopup = null;
            }
        }

        private void selectSource_Click(object sender, EventArgs e)
        {
            if (!PopupManager.IsShowingPopup)
            {
                LayoutInflater inflater = (LayoutInflater)iContext.GetSystemService(Context.LayoutInflaterService);
                View popupView = inflater.Inflate(Resource.Layout.SourceList, null, false);
                ListView listView = popupView.FindViewById<ListView>(Resource.Id.sourcelist);
                listView.DividerHeight = 0;
                iSourceAdapter.ListView = listView;
                iSourceAdapter.EventUserSelectedItem += iSourceAdapter_EventUserSelectedItem;
                iRefreshButton = popupView.FindViewById<ImageButton>(Resource.Id.sourcesrefreshbutton);
                iRefreshThrobber = popupView.FindViewById<Throbber>(Resource.Id.sourcesrefreshthrobber);
                iRefreshButton.Click += RefreshClickHandler;
                iPopup = ShowPopup(popupView, sender as View);
                iPopup.EventDismissed += SourcePopup_EventDismissed;
            }
        }

        private void iSourceAdapter_EventUserSelectedItem(object sender, EventArgs e)
        {
            Assert.Check(iPopup != null);
            iPopup.Dismiss();
        }

        private void SourcePopup_EventDismissed(object sender, EventArgs e)
        {
            if (iRefreshTimer != null)
            {
                iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                iRefreshTimer.Dispose();
                iRefreshTimer = null;
            }
            if (iRefreshButton != null)
            {
                iRefreshButton.Click -= RefreshClickHandler;
                iRefreshButton.Dispose();
                iRefreshButton = null;
            }
            if (iRefreshThrobber != null)
            {
                iRefreshThrobber.Dispose();
                iRefreshThrobber = null;
            }
            iSourceAdapter.EventUserSelectedItem -= iSourceAdapter_EventUserSelectedItem;
            iSourceAdapter.ListView = null;
            if (iPopup != null)
            {
                iPopup.EventDismissed -= SourcePopup_EventDismissed;
                iPopup = null;
            }
        }

        private IPopup ShowPopup(View aViewRoot, View aAnchor)
        {
            IWindowManager windowManager = iContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            int screenWidth = windowManager.DefaultDisplay.Width;
            IPopup popup = iPopupFactory.CreatePopup(aViewRoot, aAnchor);
            if (popup is SpeechBubblePopup)
            {
                (popup as SpeechBubblePopup).Width = screenWidth / 3;
                (popup as SpeechBubblePopup).StretchVertical = true;
            }
            popup.Show();
            return popup;
        }

        private void RefreshClickHandler(object sender, EventArgs e)
        {
            iRefreshTimer = new System.Threading.Timer((a) =>
            {
                iStack.Invoker.BeginInvoke((Action)(() =>
                {
                    if (iRefreshTimer != null)
                    {
                        iRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        iRefreshTimer.Dispose();
                        iRefreshTimer = null;
                    }
                    if (iRefreshButton != null)
                    {
                        iRefreshButton.Visibility = ViewStates.Visible;
                    }
                    if (iRefreshThrobber != null)
                    {
                        iRefreshThrobber.IsShowing = false;
                    }
                }));
            });
            iRefreshTimer.Change(kRefreshTimeout, Timeout.Infinite);
            iRefreshButton.Visibility = ViewStates.Gone;
            iRefreshThrobber.IsShowing = true;
            iStack.Rescan();
        }

        private Button iRoomButton;
        private Button iSourceButton;
        private Context iContext;
        private IPopup iPopup;
        private ViewWidgetRoomAdapter iRoomAdapter;
        private ViewWidgetSourceAdapter iSourceAdapter;
        private ImageButton iRefreshButton;
        private Throbber iRefreshThrobber;
        private Stack iStack;
        private System.Threading.Timer iRefreshTimer;
        private const int kRefreshTimeout = 5000;
        private IPopupFactory iPopupFactory;
        private ViewWidgetSelectorRoom iRoomSelector;
        private ViewWidgetSelector<Source> iSourceSelector;
        private IconResolver iIconResolver;
    }

    public class ViewWidgetSourceAdapter : ViewWidgetSelectorAdapter<Source>
    {
        public ViewWidgetSourceAdapter(Context aContext, ViewWidgetSelector<Source> aViewWidgetSelector, IInvoker aInvoker, IconResolver aIconResolver)
            : base(aContext, aViewWidgetSelector, aInvoker, aIconResolver)
        {
        }

        protected override View CreateItemView(Context aContext, Source aItem, ViewGroup aRoot)
        {
            return LayoutInflater.Inflate(Resource.Layout.SourceListItem, aRoot, false);
        }

        protected override void RecycleItemView(Context aContext, Source aItem, ViewCache aViewCache)
        {
            UpdateView(aItem, aViewCache);
        }

        private void UpdateView(Source aSource, ViewCache aViewCache)
        {
            Assert.Check(aSource != null);
            aViewCache.FindViewById<TextView>(Resource.Id.sourcename).Text = aSource.Name;
            aViewCache.FindViewById<LazyLoadingImageView>(Resource.Id.sourceicon).SetImageBitmap(IconResolver.GetIcon(aSource).Image);
            int index = iViewWidgetSelector.IndexOf(aSource);
            aViewCache.FindViewById<ImageView>(Resource.Id.currentsource).Visibility = index == iViewWidgetSelector.IndexOf(iViewWidgetSelector.SelectedItem) ? ViewStates.Visible : ViewStates.Gone;
        }

        protected override string GetSelectorText(Source aItem)
        {
            return aItem != null ? aItem.Name : "Select Source";
        }
    }

    public class ViewWidgetRoomAdapter : ViewWidgetSelectorAdapter<Room>
    {
        public ViewWidgetRoomAdapter(Context aContext, ViewWidgetSelector<Room> aViewWidgetSelector, IInvoker aInvoker, IconResolver aIconResolver)
            : base(aContext, aViewWidgetSelector, aInvoker, aIconResolver)
        {
        }

        protected override View CreateItemView(Context aContext, Room aItem, ViewGroup aRoot)
        {
            View result = LayoutInflater.Inflate(Resource.Layout.RoomListItem, aRoot, false);
            ToggleButton standbyButton = result.FindViewById<ToggleButton>(Resource.Id.standbybutton);
            standbyButton.Click += standbyButton_Click;
            return result;
        }

        protected override void RecycleItemView(Context aContext, Room aItem, ViewCache aViewCache)
        {
            UpdateView(aItem, aViewCache);
        }

        protected override void DestroyItemView(Context aContext, ViewCache aViewCache)
        {
            ToggleButton standbyButton = aViewCache.FindViewById<ToggleButton>(Resource.Id.standbybutton);
            standbyButton.Click -= standbyButton_Click;
            base.DestroyItemView(aContext, aViewCache);
        }

        private void UpdateView(Room aRoom, ViewCache aViewCache)
        {
            Assert.Check(aRoom != null);
            aViewCache.FindViewById<TextView>(Resource.Id.roomname).Text = aRoom.Name;
            ToggleButton standbyButton = aViewCache.FindViewById<ToggleButton>(Resource.Id.standbybutton);
            int index = iViewWidgetSelector.IndexOf(aRoom);
            standbyButton.Checked = aRoom.Standby;
            standbyButton.Tag = new Java.Lang.Integer(index);
            aViewCache.FindViewById<ImageView>(Resource.Id.currentroom).Visibility = index == iViewWidgetSelector.IndexOf(iViewWidgetSelector.SelectedItem) ? ViewStates.Visible : ViewStates.Gone;
        }

        private void standbyButton_Click(object sender, EventArgs e)
        {
            int index = (sender as ToggleButton).Tag.JavaCast<Java.Lang.Integer>().IntValue();
            Room room = iViewWidgetSelector.Item(index);
            (sender as ToggleButton).Checked = room.Standby;
            if (!room.Standby)
            {
                room.Standby = true;
            }
            else
            {
                Select(index);
            }
        }

        protected override string GetSelectorText(Room aItem)
        {
            return aItem != null ? aItem.Name : "Select Room";
        }
    }

    public abstract class ViewWidgetSelectorAdapter<T> : AsyncArrayAdapter<T, string>
    {
        public ViewWidgetSelectorAdapter(Context aContext, ViewWidgetSelector<T> aViewWidgetSelector, IInvoker aInvoker, IconResolver aIconResolver)
            : base(aContext, aViewWidgetSelector, "ViewWidgetSelectorAdapter")
        {
            iInvoker = aInvoker;
            iViewWidgetSelector = aViewWidgetSelector;
            iViewWidgetSelector.EventDataChanged += iViewWidgetSelector_EventDataChanged;
            iViewWidgetSelector.EventSelectionChanged += iViewWidgetSelector_EventSelectionChanged;
            iIconResolver = aIconResolver;
        }

        void iViewWidgetSelector_EventSelectionChanged(object sender, EventArgsSelection<T> e)
        {
            Refresh();
        }

        public event EventHandler<EventArgs> EventUserSelectedItem;

        public ListView ListView
        {
            set
            {
                if (iListView != null)
                {
                    iListView.ItemClick -= iListView_ItemClick;
                    iListView.Adapter = null;
                    iListView = null;
                    Clear();
                }
                if (value != null)
                {
                    iListView = value;
                    iListView.ItemClick += iListView_ItemClick;
                    iListView.Adapter = this;
                }
                Refresh();
            }
        }

        public Button SelectorButton
        {
            set
            {
                iSelectorButton = value;
                if (iSelectorButton != null)
                {
                    T selectedItem = iViewWidgetSelector.SelectedItem;
                    iSelectorButton.Text = GetSelectorText(selectedItem);
                }
            }
        }

        protected abstract string GetSelectorText(T aItem);

        protected IconResolver IconResolver
        {
            get
            {
                return iIconResolver;
            }
        }

        void iViewWidgetSelector_EventDataChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        void iListView_ItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            Select(e.Position);
        }

        private void Refresh()
        {
            Assert.Check(!iInvoker.InvokeRequired);
            if (iListView != null)
            {
                NotifyDataSetChanged();
            }
            if (iSelectorButton != null)
            {
                iSelectorButton.Text = GetSelectorText(iViewWidgetSelector.SelectedItem);
            }
        }


        public override void Close()
        {
            if (iListView != null)
            {
                iListView.ItemClick -= iListView_ItemClick;
                iListView.Adapter = null;
            }
            iViewWidgetSelector.EventDataChanged -= iViewWidgetSelector_EventDataChanged;
            iViewWidgetSelector.EventSelectionChanged -= iViewWidgetSelector_EventSelectionChanged;
            iViewWidgetSelector = null;
            base.Close();
        }

        protected void OnEventUserSelectedItem()
        {
            EventHandler<EventArgs> eventUserSelectedItem = EventUserSelectedItem;
            if (eventUserSelectedItem != null)
            {
                eventUserSelectedItem(this, EventArgs.Empty);
            }
        }

        protected void Select(int aIndex)
        {
            T item = iViewWidgetSelector.Item(aIndex);
            OnEventUserSelectedItem();
            iViewWidgetSelector.SelectedItem = item;
        }

        private ListView iListView;
        protected ViewWidgetSelector<T> iViewWidgetSelector;
        private IconResolver iIconResolver;
        private Button iSelectorButton;
        private IInvoker iInvoker;
    }

    public class ViewWidgetSelectorRoom : ViewWidgetSelector<Linn.Kinsky.Room>
    {
        public ViewWidgetSelectorRoom() : base() { }

        public ToggleButton StandbyAllButton
        {
            set
            {
                if (iStandbyAllButton != null)
                {
                    iStandbyAllButton.Click -= StandbyAllButtonClickHandler;
                }
                iStandbyAllButton = value;
                if (iStandbyAllButton != null)
                {
                    iStandbyAllButton.Click += StandbyAllButtonClickHandler;
                }
                EvaluateStandbyAllButtonState();
            }
        }

        private void StandbyAllButtonClickHandler(object sender, EventArgs e)
        {
            foreach (Linn.Kinsky.Room room in iItems)
            {
                if (!room.Standby)
                {
                    room.Standby = true;
                }
            }
        }

        private void EvaluateStandbyAllButtonState()
        {
            if (iStandbyAllButton != null)
            {
                bool outOfStandby = (from Room r in iItems where r.Standby == false select r).Count() > 0;
                iStandbyAllButton.Enabled = outOfStandby;
                iStandbyAllButton.Checked = !outOfStandby;
            }
        }

        protected override void OnEventDataChanged()
        {
            base.OnEventDataChanged();
            EvaluateStandbyAllButtonState();
        }
        private ToggleButton iStandbyAllButton;
    }

    public class ViewWidgetSelector<T> : IViewWidgetSelector<T>, IAsyncLoader<T>
    {
        public ViewWidgetSelector()
        {
            iItems = new List<T>();
        }

        public int IndexOf(T aItem)
        {
            return iItems.IndexOf(aItem);
        }

        public T SelectedItem
        {
            get
            {
                return iSelectedItem;
            }
            set
            {
                iSelectedItem = value;
                OnEventSelectionChanged();
            }
        }

        #region IAsyncLoader<T> Members

        public event EventHandler<EventArgs> EventDataChanged;

        public T Item(int aIndex)
        {
            Assert.Check(aIndex < iItems.Count);
            return iItems[aIndex];
        }

        public int Count
        {
            get { return iItems.Count; }
        }

        #endregion

        #region IViewWidgetSelector<T> Members

        public void Open()
        {
            iSelectedItem = default(T);
        }

        public void Close()
        {
            iItems.Clear();
        }

        public void InsertItem(int aIndex, T aItem)
        {
            iItems.Insert(aIndex, aItem);
            OnEventDataChanged();
        }

        public void RemoveItem(T aItem)
        {
            iItems.Remove(aItem);
            OnEventDataChanged();
        }

        public void ItemChanged(T aItem)
        {
            OnEventDataChanged();
        }

        public void SetSelected(T aItem)
        {
            iSelectedItem = aItem;
            OnEventDataChanged();
        }

        public event EventHandler<EventArgsSelection<T>> EventSelectionChanged;

        #endregion



        protected virtual void OnEventDataChanged()
        {
            EventHandler<EventArgs> eventDataChanged = EventDataChanged;
            if (eventDataChanged != null)
            {
                eventDataChanged(this, EventArgs.Empty);
            }
        }

        private void OnEventSelectionChanged()
        {
            EventHandler<EventArgsSelection<T>> eventSelectionChanged = EventSelectionChanged;
            if (eventSelectionChanged != null)
            {
                eventSelectionChanged(this, new EventArgsSelection<T>(iSelectedItem));
            }
        }

        protected List<T> iItems;
        private T iSelectedItem;
    }
    #endregion

    #region TabletBackground

    public class TabletBackground : RelativeLayout
    {
        public TabletBackground(IntPtr aHandle, JniHandleOwnership aHandleOwnership)
            : base(aHandle, aHandleOwnership)
        {
            SetWillNotDraw(false);
        }

        public TabletBackground(Context aContext, IAttributeSet aAttributeSet)
            : base(aContext, aAttributeSet)
        {
            SetWillNotDraw(false);
        }

        private void InitDrawables()
        {
            Stack stack = this.Context.ApplicationContext as Stack;
            int width = this.Width;
            int height = this.Height;
            iTopLeftImage = stack.ResourceManager.GetBitmap(Resource.Drawable.TopLeftEdge);
            iTopLeftRect = new Rect(0, 0, iTopLeftImage.Width, iTopLeftImage.Height);

            iTopRightImage = stack.ResourceManager.GetBitmap(Resource.Drawable.TopRightEdge);
            iTopRightSourceRect = new Rect(0, 0, iTopRightImage.Width, iTopRightImage.Height);
            iTopRightDestRect = new Rect(width - iTopRightImage.Width, 0, width, iTopRightImage.Height);

            iTopFillerImage = stack.ResourceManager.GetBitmap(Resource.Drawable.TopFiller);
            iTopFillerSourceRect = new Rect(0, 0, iTopFillerImage.Width, iTopFillerImage.Height);
            iTopFillerDestRect = new Rect(iTopLeftImage.Width, 0, width - iTopRightImage.Width, iTopFillerImage.Height);

            iBottomLeftImage = stack.ResourceManager.GetBitmap(Resource.Drawable.BottomLeftEdge);
            iBottomLeftSourceRect = new Rect(0, 0, iBottomLeftImage.Width, iBottomLeftImage.Height);
            iBottomLeftDestRect = new Rect(0, height - iBottomLeftImage.Height, iBottomLeftImage.Width, height);

            iBottomRightImage = stack.ResourceManager.GetBitmap(Resource.Drawable.BottomRightEdge);
            iBottomRightSourceRect = new Rect(0, 0, iBottomRightImage.Width, iBottomRightImage.Height);
            iBottomRightDestRect = new Rect(width - iBottomRightImage.Width, height - iBottomRightImage.Height, width, height);

            iBottomFillerImage = stack.ResourceManager.GetBitmap(Resource.Drawable.BottomFiller);
            iBottomFillerSourceRect = new Rect(0, 0, iBottomFillerImage.Width, iBottomFillerImage.Height);
            iBottomFillerDestRect = new Rect(iBottomLeftImage.Width, height - iBottomLeftImage.Height, width - iBottomRightImage.Width, height);

            iLeftFillerImage = stack.ResourceManager.GetBitmap(Resource.Drawable.LeftFiller);
            iLeftFillerSourceRect = new Rect(0, 0, iLeftFillerImage.Width, iLeftFillerImage.Height);
            iLeftFillerDestRect = new Rect(0, iTopLeftImage.Height, iLeftFillerImage.Width, height - iBottomLeftImage.Height);

            iRightFillerImage = stack.ResourceManager.GetBitmap(Resource.Drawable.RightFiller);
            iRightFillerSourceRect = new Rect(0, 0, iRightFillerImage.Width, iRightFillerImage.Height);
            iRightFillerDestRect = new Rect(width - iRightFillerImage.Width, iTopRightImage.Height, width, height - iBottomRightImage.Height);

            iLogoImage = stack.ResourceManager.GetBitmap(Resource.Drawable.LinnLogo);
            iLogoSourceRect = new Rect(0, 0, iLogoImage.Width, iLogoImage.Height);
            iLogoDestRect = new Rect((width / 2) - (iLogoImage.Width / 2), height - iLogoImage.Height - kLogoMargin, (width / 2) + (iLogoImage.Width / 2), height - kLogoMargin);

            iBackground = new Paint() { Color = Color.Black };

            iWidth = this.Width;
            iHeight = this.Height;
        }


        protected override void OnDraw(Canvas canvas)
        {

            if (this.Width != iWidth || this.Height != iHeight)
            {
                InitDrawables();
            }
            canvas.DrawRect(0, 0, iWidth, iHeight, iBackground);

            canvas.DrawBitmap(iTopLeftImage, iTopLeftRect, iTopLeftRect, null);
            canvas.DrawBitmap(iTopRightImage, iTopRightSourceRect, iTopRightDestRect, null);

            canvas.DrawBitmap(iBottomLeftImage, iBottomLeftSourceRect, iBottomLeftDestRect, null);

            canvas.DrawBitmap(iBottomRightImage, iBottomRightSourceRect, iBottomRightDestRect, null);

            if (canvas.Width > iTopLeftImage.Width + iTopRightImage.Width)
            {
                canvas.DrawBitmap(iTopFillerImage, iTopFillerSourceRect, iTopFillerDestRect, null);
            }
            if (canvas.Width > iBottomLeftImage.Width + iBottomRightImage.Width)
            {
                canvas.DrawBitmap(iBottomFillerImage, iBottomFillerSourceRect, iBottomFillerDestRect, null);
            }
            if (canvas.Height > iTopLeftImage.Height + iBottomLeftImage.Height)
            {
                canvas.DrawBitmap(iLeftFillerImage, iLeftFillerSourceRect, iLeftFillerDestRect, null);
            }
            if (canvas.Height > iTopRightImage.Height + iBottomRightImage.Height)
            {
                canvas.DrawBitmap(iRightFillerImage, iRightFillerSourceRect, iRightFillerDestRect, null);
            }

            canvas.DrawBitmap(iLogoImage, iLogoSourceRect, iLogoDestRect, null);

            base.OnDraw(canvas);
        }

        private Bitmap iTopLeftImage;
        private Rect iTopLeftRect;

        private Bitmap iTopRightImage;
        private Rect iTopRightSourceRect;
        private Rect iTopRightDestRect;

        private Bitmap iTopFillerImage;
        private Rect iTopFillerSourceRect;
        private Rect iTopFillerDestRect;

        private Bitmap iBottomLeftImage;
        private Rect iBottomLeftSourceRect;
        private Rect iBottomLeftDestRect;

        private Bitmap iBottomRightImage;
        private Rect iBottomRightSourceRect;
        private Rect iBottomRightDestRect;

        private Bitmap iBottomFillerImage;
        private Rect iBottomFillerSourceRect;
        private Rect iBottomFillerDestRect;

        private Bitmap iLeftFillerImage;
        private Rect iLeftFillerSourceRect;
        private Rect iLeftFillerDestRect;

        private Bitmap iRightFillerImage;
        private Rect iRightFillerSourceRect;
        private Rect iRightFillerDestRect;

        private Bitmap iLogoImage;
        private Rect iLogoSourceRect;
        private Rect iLogoDestRect;

        private const int kLogoMargin = 7;

        private Paint iBackground;
        private int iWidth;
        private int iHeight;
    }

    #endregion

    #region Browser


    public class EventArgsLocation : EventArgs
    {
        public EventArgsLocation(Location aLocation)
        {
            iLocation = aLocation;
        }

        public Location Location
        {
            get
            {
                return iLocation;
            }
        }

        private Location iLocation;
    }

    public class ViewWidgetBrowser : LinearLayout
    {

        public event EventHandler<EventArgsLocation> EventLocationChanged;

        public ViewWidgetBrowser(Context aContext,
            IContainer aRootContainer,
            IInvoker aInvoker,
            AndroidImageCache aImageCache,
            IconResolver aIconResolver,
            Button aBackButton,
            TextView aLocationDisplay,
            ToggleButton aEditButton,
            FlingStateManager aFlingStateManager,
            Throbber aThrobber,
            Button aPlayNowNextLaterButton,
            IPlaylistSupport aPlaylistSupport,
            OptionInsertMode aOptionInsertMode,
            IPopupFactory aPopupFactory)
            : base(aContext)
        {
            Assert.Check(!aInvoker.InvokeRequired);
            iPopupFactory = aPopupFactory;
            iOptionInsertMode = aOptionInsertMode;
            iErrorPanel = new BrowserErrorPanel(aContext);
            iErrorPanel.Open();
            iErrorPanel.Panel.Visibility = ViewStates.Gone;
            iErrorPanel.EventRetryClick += EventRetryClickHandler;
            iErrorPanel.EventHomeClick += EventHomeClickHandler;
            iThrobber = aThrobber;
            iThrobber.IsShowing = false;
            iListViewStack = new Stack<BrowserListView>();
            //iBrowserPanel = new ViewFlipper(aContext);
            iBrowserPanel = new RelativeLayout(aContext);
            AddView(iErrorPanel.Panel);
            AddView(iBrowserPanel);
            //AddView(iBrowserPanel);
            iFlingStateManager = aFlingStateManager;
            iEditButton = aEditButton;
            iLocationDisplay = aLocationDisplay;
            iBackButton = aBackButton;
            iImageCache = aImageCache;
            iIconResolver = aIconResolver;
            iPlayNowNextLaterButton = aPlayNowNextLaterButton;
            iPlayNowNextLaterButton.Visibility = ViewStates.Visible;
            iPlayNowNextLaterButton.Enabled = true;
            iPlayNowNextLaterButton.Text = iOptionInsertMode.Value;
            iPlayNowNextLaterButton.Click += PlayNowNextLaterButtonClickHandler;
            this.LayoutParameters = new LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            iInvoker = aInvoker;
            iPlaylistSupport = aPlaylistSupport;

            iRootContainer = aRootContainer;
            iLocation = new Location(aRootContainer);

            for (int i = 0; i < iLocation.Containers.Count; i++)
            {
                IContainer container = iLocation.Containers[i];
                container.EventContentAdded += ContentAdded;
                container.EventContentRemoved += ContentRemoved;
                container.EventContentUpdated += ContentUpdated;
                container.EventTreeChanged += TreeChanged;
                if (iCurrent != null)
                {
                    iListViewStack.Push(iCurrent);
                }
                iCurrent = new BrowserListView(Context, container, iInvoker, this, iImageCache, iIconResolver, iEditButton, iFlingStateManager, iPlaylistSupport, iPopupFactory);
                iBrowserPanel.RemoveAllViews();
                iBrowserPanel.AddView(iCurrent);
                //iBrowserPanel.AddView(iCurrent);
                //iBrowserPanel.ShowNext();
            }
            iCurrent.Active = true;
            SetDisplayState(EDisplayState.Loading);
            this.FocusableInTouchMode = true;
            this.RequestFocus();
            iBackButton.Click += aBackButton_Click;
            UpdateControls();
            iOpen = true;
        }

        public void Close()
        {
            iOpen = false;
            iCurrent.EventDisplayStateChanged -= EventDisplayStateChangedHandler;
            while (iLocation != null)
            {
                IContainer container = iLocation.Current;
                container.EventContentAdded -= ContentAdded;
                container.EventContentRemoved -= ContentRemoved;
                container.EventContentUpdated -= ContentUpdated;
                container.EventTreeChanged -= TreeChanged;

                iBrowserPanel.RemoveAllViews();
                //iBrowserPanel.ShowPrevious();
                //iBrowserPanel.RemoveView(iCurrent);
                Assert.Check(iCurrent != null, "iCurrent != null");
                iCurrent.Close();
                iCurrent.Dispose();
                if (iListViewStack.Count > 0)
                {
                    iCurrent = iListViewStack.Pop();
                }
                iLocation = iLocation.PreviousLocation();
            }
            iBackButton.Click -= aBackButton_Click;
            iPlayNowNextLaterButton.Click -= PlayNowNextLaterButtonClickHandler;
            iErrorPanel.EventRetryClick -= EventRetryClickHandler;
            iErrorPanel.EventHomeClick -= EventHomeClickHandler;
            iErrorPanel.Close();
            iPlayNowNextLaterButton.Visibility = ViewStates.Invisible;
            if (iScheduler != null)
            {
                iScheduler.Stop();
                iScheduler = null;
            }
            RemoveAllViews();
            iBrowserPanel.Dispose();
        }

        public Location CurrentLocation
        {
            get
            {
                return iLocation;
            }
        }

        private void PlayNowNextLaterButtonClickHandler(object sender, EventArgs e)
        {
            string buttonText = iPlayNowNextLaterButton.Text;
            IList<string> insertModes = iOptionInsertMode.Allowed;
            for (int i = 0; i < insertModes.Count; i++)
            {
                if (buttonText == insertModes[i])
                {
                    iPlayNowNextLaterButton.Text = i == insertModes.Count - 1 ? insertModes[0] : insertModes[i + 1];
                    iOptionInsertMode.Set(iPlayNowNextLaterButton.Text);
                    break;
                }
            }
        }

        public string PlayMode
        {
            get
            {
                return iOptionInsertMode.Value;
            }
        }

        private void EventRetryClickHandler(object sender, EventArgs e)
        {
            if (iNavigateTrail != null)
            {
                Navigate(iNavigateTrail, 3);
            }
            else
            {
                Navigate(iLocation.BreadcrumbTrail, 3);
            }
        }

        private void EventHomeClickHandler(object sender, EventArgs e)
        {
            iNavigateTrail = null;
            Browse(new Location(iRootContainer));
        }

        private void EventDisplayStateChangedHandler(object sender, EventArgs e)
        {
            if (iNavigateTrail == null && iOpen)
            {
                SetDisplayState(iCurrent.DisplayState);
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            if (iLocation.Containers.Count > 0)
            {
                int index = iLocation.Containers.Count - 1;
                //iBackButton.Text = index == 0 ? "" : iLocation.Containers[index - 1].Metadata.Title;
                iBackButton.Enabled = index > 0;
                iLocationDisplay.Text = iLocation.Containers[index].Metadata.Title;
                iBackButton.Visibility = iLocation.Containers.Count > 1 ? ViewStates.Visible : ViewStates.Gone;
            }
            else
            {
                //iBackButton.Text = string.Empty;
                iBackButton.Enabled = false;
                iLocationDisplay.Text = string.Empty;
                iBackButton.Visibility = ViewStates.Gone;
            }
        }

        private void aBackButton_Click(object sender, EventArgs e)
        {
            Assert.Check(!iInvoker.InvokeRequired);
            if (CanGoUp())
            {
                Up(1);
            }
        }

        public bool CanGoUp()
        {
            return iLocation.Containers.Count > 1;
        }

        public void Up(uint aLevels)
        {
            if (iInvoker.TryBeginInvoke((Action<uint>)(Up), aLevels))
                return;
            Up(aLevels, true);
        }

        public void Up(uint aLevels, bool aAnimate)
        {
            if (iInvoker.TryBeginInvoke((Action<uint, bool>)(Up), aLevels, aAnimate))
                return;
            if (iOpen)
            {
                iCurrent.Active = false;
                iCurrent.EventDisplayStateChanged -= EventDisplayStateChangedHandler;
                for (uint i = 0; i < aLevels && iLocation.Containers.Count > 1; ++i)
                {
                    IContainer container = iLocation.Current;
                    container.EventContentAdded -= ContentAdded;
                    container.EventContentRemoved -= ContentRemoved;
                    container.EventContentUpdated -= ContentUpdated;
                    container.EventTreeChanged -= TreeChanged;

                    iLocation = iLocation.PreviousLocation();
                    iBrowserPanel.RemoveAllViews();
                    //iBrowserPanel.ShowPrevious();
                    //iBrowserPanel.RemoveView(iCurrent);
                    iCurrent.Close();
                    iCurrent = iListViewStack.Pop();
                    iBrowserPanel.AddView(iCurrent);
                    //iCurrent = iBrowserPanel.CurrentView as BrowserListView;
                }
                iCurrent.Active = true;
                iCurrent.EventDisplayStateChanged += EventDisplayStateChangedHandler;
                SetDisplayState(iCurrent.DisplayState);
                UpdateControls();
                OnEventLocationChanged(iLocation);
            }
        }

        public void Down(container aContainer)
        {
            if (iInvoker.TryBeginInvoke((Action<container>)(Down), aContainer))
                return;
            Down(aContainer, true);
        }

        public void Down(container aContainer, bool aAnimate)
        {
            if (iInvoker.TryBeginInvoke((Action<container, bool>)(Down), aContainer, aAnimate))
                return;

            if (iOpen)
            {
                iCurrent.Active = false;
                iCurrent.EventDisplayStateChanged -= EventDisplayStateChangedHandler;
                IContainer container = iLocation.Current.ChildContainer(aContainer);
                if (container != null)
                {
                    iLocation = new Location(iLocation, container);

                    container.EventContentAdded += ContentAdded;
                    container.EventContentRemoved += ContentRemoved;
                    container.EventContentUpdated += ContentUpdated;
                    container.EventTreeChanged += TreeChanged;
                    iListViewStack.Push(iCurrent);
                    iCurrent = new BrowserListView(Context, iLocation.Current, iInvoker, this, iImageCache, iIconResolver, iEditButton, iFlingStateManager, iPlaylistSupport, iPopupFactory);
                    iBrowserPanel.RemoveAllViews();
                    iBrowserPanel.AddView(iCurrent);
                    //iBrowserPanel.AddView(iCurrent);
                    //iBrowserPanel.ShowNext();
                }
                iCurrent.Active = true;
                iCurrent.EventDisplayStateChanged += EventDisplayStateChangedHandler;
                SetDisplayState(iCurrent.DisplayState);
                UpdateControls();
                OnEventLocationChanged(iLocation);
            }
        }

        public void Browse(Location aLocation)
        {
            if (iInvoker.TryBeginInvoke((Action<Location>)(Browse), aLocation))
                return;
            if (iOpen)
            {
                Assert.Check(iLocation != null, "iLocation != null");
                Assert.Check(aLocation != null, "aLocation != null");
                Up((uint)(iLocation.Containers.Count - 1), false);

                for (int i = 1; i < aLocation.Containers.Count; i++)
                {
                    container next = aLocation.Containers[i].Metadata;
                    Down(next, false);
                }

                iCurrent.Active = true;
                iCurrent.Refresh();
            }
        }

        public void Navigate(BreadcrumbTrail aBreadcrumbTrail, int aRetryCount)
        {
            if (!iOpen)
            {
                return;
            }
            Assert.Check(aBreadcrumbTrail.Count > 0);
            iNavigateTrail = aBreadcrumbTrail;
            iInvoker.BeginInvoke((Action)(() =>
            {
                if (iOpen)
                {
                    SetDisplayState(EDisplayState.Loading);
                }
            }));
            if (iScheduler == null)
            {
                iScheduler = new Scheduler("BrowserScheduler", 1);
            }
            iScheduler.Schedule(() =>
            {
                if (!iOpen)
                {
                    return;
                }
                iLocatorAsync = new LocatorAsync(iRootContainer, aBreadcrumbTrail);
                iLocatorAsync.Locate((sender, location) =>
                {
                    if (!iOpen)
                    {
                        return;
                    }
                    if (sender == iLocatorAsync)
                    {
                        if (location != null)
                        {
                            iNavigateTrail = null;
                            iInvoker.BeginInvoke((Action)(() =>
                            {
                                if (iOpen)
                                {
                                    Browse(location);
                                }
                            }));
                        }
                        else if (aRetryCount > 0)
                        {
                            Thread.Sleep(3000);
                            Navigate(aBreadcrumbTrail, aRetryCount - 1);
                        }
                        else
                        {
                            iInvoker.BeginInvoke((Action)(() =>
                            {
                                if (iOpen)
                                {
                                    SetDisplayState(EDisplayState.NotFound);
                                }
                            }));
                        }
                    }
                });
            });
        }

        private void SetDisplayState(EDisplayState aDisplayState)
        {
            iDisplayState = aDisplayState;
            iErrorPanel.Panel.Visibility = iDisplayState == EDisplayState.Error || iDisplayState == EDisplayState.NotFound ? ViewStates.Visible : ViewStates.Gone;
            iThrobber.IsShowing = iDisplayState == EDisplayState.Loading;
            iBrowserPanel.Visibility = iDisplayState == EDisplayState.Loaded ? ViewStates.Visible : ViewStates.Gone;
            if (iDisplayState == EDisplayState.Error)
            {
                iLocationDisplay.Text = "Error...";
            }
            else if (iDisplayState == EDisplayState.NotFound)
            {
                iLocationDisplay.Text = "Not Found...";
            }
            else if (iDisplayState == EDisplayState.Loading)
            {
                iLocationDisplay.Text = "Searching...";
            }
        }

        private void ContentAdded(object sender, EventArgs e)
        {
            if (!iOpen)
            {
                return;
            }
            if (iInvoker.TryBeginInvoke((Action<object, EventArgs>)(ContentAdded), sender, e))
                return;
            if (iLocation != null && iCurrent != null)
            {
                if (sender == iLocation.Current)
                {
                    iCurrent.Refresh();
                }
            }
        }

        private void ContentRemoved(object sender, EventArgsContentRemoved e)
        {
            if (!iOpen)
            {
                return;
            }
            if (iInvoker.TryBeginInvoke((Action<object, EventArgsContentRemoved>)(ContentRemoved), sender, e))
                return;
            if (iCurrent != null && iLocation != null)
            {
                int index = iLocation.Containers.IndexOf(sender as IContainer);
                if (index != -1 && (index == iLocation.Containers.Count - 1 || iLocation.Containers[index + 1].Id == e.Id))
                {
                    iCurrent.Refresh();
                }
            }
        }

        private void ContentUpdated(object sender, EventArgs e)
        {
            if (!iOpen)
            {
                return;
            }
            if (iInvoker.TryBeginInvoke((Action<object, EventArgs>)(ContentUpdated), sender, e))
                return;
            if (iLocation != null && iCurrent != null)
            {
                if (sender == iLocation.Current)
                {
                    iCurrent.Refresh();
                }
            }
        }

        private void TreeChanged(object sender, EventArgs e)
        {
            if (!iOpen)
            {
                return;
            }
            if (iInvoker.TryBeginInvoke((Action<object, EventArgs>)(TreeChanged), sender, e))
                return;
            if (iCurrent != null && iLocation != null)
            {
                if (iLocation.Current.HasTreeChangeAffectedLeaf)
                {
                    iCurrent.Refresh();
                }
            }
        }

        private void OnEventLocationChanged(Location aLocation)
        {
            EventHandler<EventArgsLocation> del = EventLocationChanged;
            if (del != null)
            {
                del(this, new EventArgsLocation(aLocation));
            }
        }

        private BrowserListView iCurrent;
        private Location iLocation;
        private IInvoker iInvoker;
        private AndroidImageCache iImageCache;
        private IconResolver iIconResolver;
        private Button iBackButton;
        private TextView iLocationDisplay;
        private ToggleButton iEditButton;
        private FlingStateManager iFlingStateManager;
        private Scheduler iScheduler;
        private LocatorAsync iLocatorAsync;
        private BrowserErrorPanel iErrorPanel;
        private Throbber iThrobber;
        private IContainer iRootContainer;
        private RelativeLayout iBrowserPanel;
        private Stack<BrowserListView> iListViewStack;
        //private ViewFlipper iBrowserPanel;
        private BreadcrumbTrail iNavigateTrail;
        private EDisplayState iDisplayState;
        private Button iPlayNowNextLaterButton;
        private IPlaylistSupport iPlaylistSupport;
        private OptionInsertMode iOptionInsertMode;
        private IPopupFactory iPopupFactory;
        private volatile bool iOpen;
    }

    public enum EDisplayState
    {
        Loading,
        Loaded,
        Error,
        NotFound
    }

    public class BrowserListView : ListView, IAsyncLoader<BrowserItem>
    {
        public BrowserListView(Context aContext, IContainer aContainer, IInvoker aInvoker, ViewWidgetBrowser aParent, AndroidImageCache aImageCache, IconResolver aIconResolver, ToggleButton aEditModeButton, FlingStateManager aFlingStateManager, IPlaylistSupport aPlaylistSupport, IPopupFactory aPopupFactory)
            : base(aContext)
        {
            iPopupFactory = aPopupFactory;
            iFlingStateManager = aFlingStateManager;
            iEditModeButton = aEditModeButton;
            iImageCache = aImageCache;
            iIconResolver = aIconResolver;
            iContainer = aContainer;
            iParent = aParent;
            iInvoker = aInvoker;
            CreateContentCollector();
            this.ItemClick += BrowserListView_ItemClick;
            iEditModeButton.Click += EditButtonClicked;
            this.LayoutParameters = new LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            iCanEdit = false;
            iLevel2Cache = new DictionaryBackedContentCache<BrowserItem>(kLevel2CacheSize);
            iPendingActivatedIndex = -1;
            iFlingScrollListener = new FlingScrollListener(iFlingStateManager);
            this.SetOnScrollListener(iFlingScrollListener);
            iFlingStateManager.EventFlingStateChanged += EventFlingStateChangedHandler;
            iDisplayState = EDisplayState.Loading;
            iPlaylistSupport = aPlaylistSupport;
            this.ItemLongClick += BrowserListView_ItemLongClick;
            DividerHeight = 0;
            FastScrollEnabled = true;
        }

        public EDisplayState DisplayState
        {
            get
            {
                return iDisplayState;
            }
        }

        public event EventHandler<EventArgs> EventDisplayStateChanged;


        public void EventFlingStateChangedHandler(object sender, EventArgs e)
        {
            if (iActive)
            {
                SetFlingState();
            }
        }

        private void SetFlingState()
        {
            if (iContentCollector != null)
            {
                iContentCollector.IsRunning = !iFlingStateManager.IsFlinging();
            }
            if (!iFlingStateManager.IsFlinging())
            {
                OnEventDataChanged();
            }
        }

        public void Close()
        {
            iFlingStateManager.SetFlinging(this, false);
            iFlingStateManager.EventFlingStateChanged -= EventFlingStateChangedHandler;
            this.SetOnScrollListener(null);
            this.ItemClick -= BrowserListView_ItemClick;
            this.ItemLongClick -= BrowserListView_ItemLongClick;
            iEditModeButton.Click -= EditButtonClicked;
            DestroyContentCollector();
            ClosePopup();
        }

        public bool Active
        {
            set
            {
                if (value)
                {
                    iEditModeButton.Visibility = iCanEdit ? ViewStates.Visible : ViewStates.Gone;
                    EditMode = false;
                    iEditModeButton.Checked = false;
                }
                iActive = value;
            }
        }

        private void EditButtonClicked(object sender, EventArgs args)
        {
            if (iActive)
            {
                EditMode = iEditModeButton.Checked;
            }
        }

        public bool EditMode
        {
            get
            {
                return iEditMode;
            }
            set
            {
                iEditMode = value;
                if (Adapter != null)
                {
                    (Adapter as BrowserListAdaptor).EditMode = value;
                }
            }
        }

        void iContentCollector_EventItemsFailed(object sender, EventArgsItemsFailed e)
        {
            iInvoker.BeginInvoke((Action)(() =>
            {
                iDisplayState = EDisplayState.Error;
                OnEventDisplayStateChanged();
            }));
        }

        private void EnsureCacheItems(EventArgsItemsLoaded<upnpObject> e)
        {
            lock (iLevel2Cache)
            {
                BrowserItem current = null;
                for (int i = 0; i < e.Items.Count; i++)
                {
                    int cacheIndex = e.StartIndex + i;
                    if (!iLevel2Cache.TryGet(cacheIndex, out current))
                    {
                        BrowserItem item = new BrowserItem(e.Items[i], iContainer.Metadata is musicAlbum ? iContainer.Metadata : null, iIconResolver, iContainer.Metadata is musicAlbum, false);
                        if (!iFlingStateManager.IsFlinging())
                        {
                            item.EnsureLoaded();
                            if (item.Icon.IsUri && !iImageCache.Contains(item.Icon.ImageUri.OriginalString))
                            {
                                iImageCache.Image(item.Icon.ImageUri.OriginalString);
                            }
                        }
                        iLevel2Cache.Add(cacheIndex, item);
                    }
                }
            }
        }

        void iContentCollector_EventItemsLoaded(object sender, EventArgsItemsLoaded<upnpObject> e)
        {
            EnsureCacheItems(e);
            if (!iFlingStateManager.IsFlinging())
            {
                iInvoker.BeginInvoke((Action)(() =>
                {
                    if (sender == iContentCollector && !iFlingStateManager.IsFlinging())
                    {
                        if (!iTestedCanEdit)
                        {
                            DidlLite didl = new DidlLite();
                            didl.AddRange(e.Items);
                            iCanEdit = iContainer.HandleDelete(didl) || iContainer.HandleMove(didl);
                            (Adapter as BrowserListAdaptor).CanDelete = iCanEdit;
                            if (iActive)
                            {
                                iEditModeButton.Visibility = iCanEdit ? ViewStates.Visible : ViewStates.Gone;
                            }
                            iTestedCanEdit = true;
                        }
                        OnEventDataChanged();
                        if (iPendingActivatedIndex != -1 && iPendingActivatedIndex >= e.StartIndex && iPendingActivatedIndex < e.StartIndex + e.Items.Count)
                        {
                            Activate(e.Items[iPendingActivatedIndex - e.StartIndex], iPendingActivationType, iAllowActivateBrowse);
                            iPendingActivatedIndex = -1;
                        }
                    }
                }));
            }
        }

        void iContentCollector_EventOpened(object sender, EventArgs e)
        {
            iInvoker.BeginInvoke((Action)(() =>
            {
                if (sender == iContentCollector)
                {
                    iDisplayState = EDisplayState.Loaded;
                    OnEventDisplayStateChanged();
                    iTestedCanEdit = false;
                    iAdaptorCount = iContentCollector.Count;
                    Adapter = new BrowserListAdaptor(Context, this, iInvoker, iImageCache, iIconResolver, iFlingStateManager);
                    (Adapter as BrowserListAdaptor).EventItemDeleted += EventItemDeletedHandler;
                    if (iContainer.Metadata is musicAlbum)
                    {
                        List<SectionHeader<BrowserItem>> sectionHeaders = new List<SectionHeader<BrowserItem>>();
                        sectionHeaders.Add(new SectionHeader<BrowserItem>(0, new BrowserItem(iContainer.Metadata, null, iIconResolver, true, true)));
                        (Adapter as BrowserListAdaptor).SetSectionHeaders(sectionHeaders);
                    }
                    (Adapter as BrowserListAdaptor).EditMode = iEditMode;
                }
            }));
        }

        private void CreateContentCollector()
        {
            Assert.Check(!iInvoker.InvokeRequired);
            DestroyContentCollector();
            if (iDisplayState != EDisplayState.Loading)
            {
                iDisplayState = EDisplayState.Loading;
                OnEventDisplayStateChanged();
            }
            iContentCollector = ContentCollectorMaster.Create(iContainer, new DictionaryBackedContentCache<upnpObject>(kCacheSize), kRangeSize, kThreadCount, kReadAheadRanges);
            iContentCollector.EventItemsLoaded += iContentCollector_EventItemsLoaded;
            iContentCollector.EventItemsFailed += iContentCollector_EventItemsFailed;
            iContentCollector.EventOpened += iContentCollector_EventOpened;
            // hide any pending popup menus
            ClosePopup();
        }

        private void DestroyContentCollector()
        {
            Assert.Check(!iInvoker.InvokeRequired);
            if (Adapter != null)
            {
                (Adapter as BrowserListAdaptor).EventItemDeleted -= EventItemDeletedHandler;
                (Adapter as BrowserListAdaptor).Close();
                Adapter.Dispose();
                Adapter = null;
            }
            if (iContentCollector != null)
            {
                iContentCollector.EventOpened -= iContentCollector_EventOpened;
                iContentCollector.EventItemsLoaded -= iContentCollector_EventItemsLoaded;
                iContentCollector.EventItemsFailed -= iContentCollector_EventItemsFailed;
                iContentCollector.Dispose();
                iContentCollector = null;
                iLevel2Cache.Clear();
            }
        }

        private void EventItemDeletedHandler(object sender, EventArgsListEdit<BrowserItem> e)
        {
            iContainer.Delete(e.Item.Id);
        }

        private void BrowserListView_ItemLongClick(object sender, ItemLongClickEventArgs e)
        {
            iLastLongPressPosition = e.Position;
            iLastLongPressItem = e.View;
            DateTime now = DateTime.Now;
            if ((now - iLastSwipe) > TimeSpan.FromMilliseconds(kLastSwipeTimeout))
            {
                ShowPopupMenu();
            }
        }

        public void ShowPopupMenu()
        {
            if (!PopupManager.IsShowingPopup)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                iPopupMenu = inflater.Inflate(Resource.Layout.BrowserMenu, null);
                iPopupMenu.Tag = iLastLongPressPosition;
                Button playNow = iPopupMenu.FindViewById<Button>(Resource.Id.browsermenuplaynow);
                Button playNext = iPopupMenu.FindViewById<Button>(Resource.Id.browsermenuplaynext);
                Button playLater = iPopupMenu.FindViewById<Button>(Resource.Id.browsermenuplaylater);
                TextView title = iPopupMenu.FindViewById<TextView>(Resource.Id.browsermenutitle);
                playNow.Click += MenuPlayNowClickHandler;
                playNext.Click += MenuPlayNextClickHandler;
                playLater.Click += MenuPlayLaterClickHandler;
                BrowserItem item = (Adapter as BrowserListAdaptor)[iLastLongPressPosition] as BrowserItem;
                if (item != null)
                {
                    title.Text = item.DisplayField1;
                }
                else
                {
                    title.Text = "Choose...";
                }
                iPopup = iPopupFactory.CreatePopup(iPopupMenu, iLastLongPressItem);
                iPopup.EventDismissed += EventDismissedHandler;
                iPopup.Show();
            }
        }

        private void MenuPlayNowClickHandler(object sender, EventArgs e)
        {
            if (iPopupMenu != null)
            {
                Activate((int)iPopupMenu.Tag, OptionInsertMode.kPlayNow, false);
                iPopup.Dismiss();
            }
        }

        private void MenuPlayNextClickHandler(object sender, EventArgs e)
        {
            if (iPopupMenu != null)
            {
                Activate((int)iPopupMenu.Tag, OptionInsertMode.kPlayNext, false);
                iPopup.Dismiss();
            }
        }

        private void MenuPlayLaterClickHandler(object sender, EventArgs e)
        {
            if (iPopupMenu != null)
            {
                Activate((int)iPopupMenu.Tag, OptionInsertMode.kPlayLater, false);
                iPopup.Dismiss();
            }
        }

        private void EventDismissedHandler(object sender, EventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            if (iPopup != null)
            {
                Button playNow = iPopupMenu.FindViewById<Button>(Resource.Id.browsermenuplaynow);
                Button playNext = iPopupMenu.FindViewById<Button>(Resource.Id.browsermenuplaynext);
                Button playLater = iPopupMenu.FindViewById<Button>(Resource.Id.browsermenuplaylater);
                playNow.Click -= MenuPlayNowClickHandler;
                playNext.Click -= MenuPlayNextClickHandler;
                playLater.Click -= MenuPlayLaterClickHandler;
                playNow.Dispose();
                playNext.Dispose();
                playLater.Dispose();
                iPopupMenu.Tag = null;
                iPopupMenu.Dispose();
                iPopupMenu = null;
                iPopup.EventDismissed -= EventDismissedHandler;
                iPopup = null;
            }
        }

        private void BrowserListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Activate(e.Position, iParent.PlayMode, true);
        }

        private void Activate(int aPosition, string aPlayMode, bool aAllowBrowse)
        {
            if (iContainer.Metadata is musicAlbum && aPosition == 0)
            {
                Activate(iContainer.Metadata, aPlayMode, aAllowBrowse);
            }
            else
            {
                int index = iContainer.Metadata is musicAlbum ? aPosition - 1 : aPosition;
                upnpObject request = iContentCollector.Item(index, ERequestPriority.Foreground);
                iAllowActivateBrowse = aAllowBrowse;
                iPendingActivatedIndex = index;
                iPendingActivationType = aPlayMode;
                if (request != null)
                {
                    iPendingActivatedIndex = -1;
                    Activate(request, aPlayMode, aAllowBrowse);
                }
            }
        }

        private void Activate(upnpObject aItem, string aPlayMode, bool aAllowBrowse)
        {
            Assert.Check(aItem != null);
            if (aItem is container && aItem != iContainer.Metadata && aAllowBrowse)
            {
                iParent.Down(aItem as container);
            }
            else
            {
                MediaRetriever retriever = null;
                if (aItem == iContainer.Metadata)
                {
                    retriever = new MediaRetriever(iParent.CurrentLocation.PreviousLocation().Current, new List<upnpObject>() { aItem });
                }
                else
                {
                    retriever = new MediaRetriever(iParent.CurrentLocation.Current, new List<upnpObject>() { aItem });
                }

                switch (aPlayMode)
                {
                    case OptionInsertMode.kPlayNow:
                        {
                            iPlaylistSupport.PlayNow(retriever);
                            break;
                        }
                    case OptionInsertMode.kPlayNext:
                        {
                            iPlaylistSupport.PlayNext(retriever);
                            break;
                        }
                    case OptionInsertMode.kPlayLater:
                        {
                            iPlaylistSupport.PlayLater(retriever);
                            break;
                        }
                    default:
                        {
                            Assert.Check(false);
                            break;
                        }
                }
            }
        }

        internal void Refresh()
        {
            DestroyContentCollector();
            CreateContentCollector();
        }

        private IContainer iContainer;
        private IContentCollector<upnpObject> iContentCollector;

        #region IAsyncLoader<upnpObject> Members

        public event EventHandler<EventArgs> EventDataChanged;

        public BrowserItem Item(int aIndex)
        {
            BrowserItem result = null;
            lock (iLevel2Cache)
            {
                if (!iLevel2Cache.TryGet(aIndex, out result) && !iFlingStateManager.IsFlinging())
                {
                    upnpObject item = iContentCollector.Item(aIndex, ERequestPriority.Foreground);
                    if (item != null)
                    {
                        result = new BrowserItem(item, iContainer.Metadata is musicAlbum ? iContainer.Metadata : null, iIconResolver, iContainer.Metadata is musicAlbum, false);
                        iLevel2Cache.Add(aIndex, result);
                    }
                }
            }
            return result;
        }

        int IAsyncLoader<BrowserItem>.Count
        {
            get
            {
                return iAdaptorCount;
            }
        }

        public DateTime LastSwipe
        {
            set
            {
                iLastSwipe = value;
            }
        }


        #endregion

        private void OnEventDataChanged()
        {
            EventHandler<EventArgs> evtDataChanged = EventDataChanged;
            if (evtDataChanged != null)
            {
                evtDataChanged(this, EventArgs.Empty);
            }
        }

        private void OnEventDisplayStateChanged()
        {
            EventHandler<EventArgs> del = EventDisplayStateChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private IInvoker iInvoker;
        private int iAdaptorCount;
        private ViewWidgetBrowser iParent;
        private AndroidImageCache iImageCache;
        private IconResolver iIconResolver;
        private const int kCacheSize = 50;
        private const int kLevel2CacheSize = 500;
        private const int kRangeSize = 10;
        private const int kThreadCount = 2;
        private const int kReadAheadRanges = 0;
        private bool iEditMode;
        private ToggleButton iEditModeButton;
        private bool iActive;
        private bool iCanEdit;
        private bool iTestedCanEdit;
        private int iPendingActivatedIndex;
        private bool iAllowActivateBrowse;
        private string iPendingActivationType;
        private FlingStateManager iFlingStateManager;
        private FlingScrollListener iFlingScrollListener;
        private EDisplayState iDisplayState;
        private IPlaylistSupport iPlaylistSupport;

        private DictionaryBackedContentCache<BrowserItem> iLevel2Cache;
        private IPopupFactory iPopupFactory;
        private IPopup iPopup;
        private View iPopupMenu;
        private int iLastLongPressPosition;
        private View iLastLongPressItem;
        private DateTime iLastSwipe = DateTime.MinValue;
        private const int kLastSwipeTimeout = 2000;
    }

    public class BrowserListAdaptor : AsyncArrayAdapter<BrowserItem, BrowserItem>
    {
        public BrowserListAdaptor(Context aContext, IAsyncLoader<BrowserItem> aLoader, IInvoker aInvoker, AndroidImageCache aImageCache, IconResolver aIconResolver, FlingStateManager aFlingStateManager)
            : base(aContext, aLoader, "BrowserListAdaptor")
        {
            iFlingStateManager = aFlingStateManager;
            iImageCache = aImageCache;
            iIconResolver = aIconResolver;
            iInvoker = aInvoker;
            CanDelete = false;
            iPlaceholder = new BitmapDrawable(iIconResolver.IconLoading.Image);
            iPreferredHeight = (int)aContext.ThemedResourceAttribute(Android.Resource.Attribute.ListPreferredItemHeight);
        }

        public bool CanDelete { get; set; }

        protected override View CreateSectionHeaderView(Context aContext, BrowserItem aItem, ViewGroup aRoot)
        {
            return LayoutInflater.Inflate(Resource.Layout.BrowserItem, aRoot, false);
        }

        protected override View CreateItemView(Context aContext, BrowserItem aItem, ViewGroup aRoot)
        {
            return LayoutInflater.Inflate(Resource.Layout.BrowserItem, aRoot, false);
        }

        protected override void RecycleSectionHeaderView(Context aContext, BrowserItem aItem, ViewCache aViewCache)
        {
            PopulateView(aItem, aViewCache);
        }

        protected override void RecycleItemView(Context aContext, BrowserItem aItem, ViewCache aViewCache)
        {
            PopulateView(aItem, aViewCache);
        }

        protected override void DestroyItemView(Context aContext, ViewCache aViewCache)
        {
            LazyLoadingImageView imageView = aViewCache.FindViewById<LazyLoadingImageView>(Resource.Id.browseritemicon);
            imageView.Dispose();
            base.DestroyItemView(aContext, aViewCache);
        }

        protected override bool CanDeleteItem(BrowserItem aItem, int aPosition)
        {
            return CanDelete;
        }

        protected override int RequestDeleteButtonResourceId
        {
            get
            {
                return Resource.Layout.RequestDeleteButton;
            }
        }

        protected override int ConfirmDeleteButtonResourceId
        {
            get
            {
                return Resource.Layout.ConfirmDeleteButton;
            }
        }

        protected override int MoveUpButtonResourceId
        {
            get
            {
                return Resource.Layout.MoveUpButton;
            }
        }

        protected override int MoveDownButtonResourceId
        {
            get
            {
                return Resource.Layout.MoveDownButton;
            }
        }

        private void PopulateView(BrowserItem aItem, ViewCache aViewCache)
        {
            if (aItem != null)
            {
                bool isGroupedItem = aItem.IsGrouped && !aItem.IsGroupHeader;

                if (aItem.IsGroupHeader)
                {
                    iHeaderArtist = aItem.DisplayField2;
                }
                LazyLoadingImageView imageView = aViewCache.FindViewById<LazyLoadingImageView>(Resource.Id.browseritemicon);
                Icon<Bitmap> icon = aItem.Icon;
                if (icon.IsUri)
                {
                    imageView.SetImageDrawable(iPlaceholder);
                    imageView.LoadImage(iImageCache, icon.ImageUri);
                }
                else
                {
                    imageView.SetImageBitmap(icon.Image);
                }

                RelativeLayout imageViewContainer = aViewCache.FindViewById<RelativeLayout>(Resource.Id.browseritemiconcontainer);
                imageViewContainer.Visibility = isGroupedItem ? ViewStates.Gone : ViewStates.Visible;
                int preferredSize = aItem.IsGroupHeader ? iPreferredHeight * 2 : iPreferredHeight;
                if (imageViewContainer.LayoutParameters.Width != preferredSize)
                {
                    imageViewContainer.LayoutParameters.Width = preferredSize;
                    imageViewContainer.LayoutParameters.Height = preferredSize;
                }

                TextView firstLine = aViewCache.FindViewById<TextView>(Resource.Id.browseritemfirstline);
                string originalTrackNumber = aItem.OriginalTrackNumber;
                if (originalTrackNumber != string.Empty)
                {
                    originalTrackNumber = string.Format("{0}. ", originalTrackNumber);
                }
                string firstLineText = isGroupedItem ? string.Format("{0}{1}", originalTrackNumber, aItem.DisplayField1) : aItem.DisplayField1;
                firstLine.Text = firstLineText;
                firstLine.Visibility = ViewStates.Visible;
                TextView secondLine = aViewCache.FindViewById<TextView>(Resource.Id.browseritemsecondline);
                secondLine.Text = aItem.DisplayField2;
                secondLine.Visibility = aItem.DisplayField2 == string.Empty ? ViewStates.Gone : ViewStates.Visible;
                TextView thirdLine = aViewCache.FindViewById<TextView>(Resource.Id.browseritemthirdline);
                thirdLine.Text = aItem.DisplayField3;
                thirdLine.Visibility = isGroupedItem || aItem.DisplayField3 == string.Empty ? ViewStates.Gone : ViewStates.Visible;
                TextView technicalInfo = aViewCache.FindViewById<TextView>(Resource.Id.browseritemtechnicalinfo);
                technicalInfo.Text = aItem.TechnicalInfo;
                technicalInfo.Visibility = technicalInfo.Text.Length > 0 ? ViewStates.Visible : ViewStates.Gone;
                ImageView navIcon = aViewCache.FindViewById<ImageView>(Resource.Id.browseritemnavigationicon);
                navIcon.Visibility = aItem.CanBrowseDown ? ViewStates.Visible : ViewStates.Gone;
            }
            else
            {
                aViewCache.FindViewById<LazyLoadingImageView>(Resource.Id.browseritemicon).SetImageDrawable(iPlaceholder);
                TextView firstLine = aViewCache.FindViewById<TextView>(Resource.Id.browseritemfirstline);
                firstLine.Visibility = ViewStates.Visible;
                firstLine.Text = "...";
                aViewCache.FindViewById<TextView>(Resource.Id.browseritemsecondline).Visibility = ViewStates.Gone;
                aViewCache.FindViewById<TextView>(Resource.Id.browseritemthirdline).Visibility = ViewStates.Gone;
                aViewCache.FindViewById<TextView>(Resource.Id.browseritemtechnicalinfo).Visibility = ViewStates.Gone;
                aViewCache.FindViewById<ImageView>(Resource.Id.browseritemnavigationicon).Visibility = ViewStates.Gone;
            }
        }


        private AndroidImageCache iImageCache;
        private IconResolver iIconResolver;
        private IInvoker iInvoker;
        private BitmapDrawable iPlaceholder;
        private string iHeaderArtist;
        private FlingStateManager iFlingStateManager;
        private int iPreferredHeight;

    }

    public class BrowserErrorPanel
    {
        public event EventHandler<EventArgs> EventHomeClick;
        public event EventHandler<EventArgs> EventRetryClick;

        public BrowserErrorPanel(Context aContext)
        {
            LayoutInflater inflater = (LayoutInflater)aContext.GetSystemService(Context.LayoutInflaterService);
            iPanel = inflater.Inflate(Resource.Layout.BrowserErrorPanel, null);
        }

        public void Open()
        {
            Button homeButton = iPanel.FindViewById<Button>(Resource.Id.browsererrorhomebutton);
            homeButton.Click += HomeButtonClickHandler;
            Button retryButton = iPanel.FindViewById<Button>(Resource.Id.browsererrorretrybutton);
            retryButton.Click += RetryButtonClickHandler;
        }

        public void Close()
        {
            Button homeButton = iPanel.FindViewById<Button>(Resource.Id.browsererrorhomebutton);
            homeButton.Click -= HomeButtonClickHandler;
            homeButton.Dispose();
            Button retryButton = iPanel.FindViewById<Button>(Resource.Id.browsererrorretrybutton);
            retryButton.Click -= RetryButtonClickHandler;
            retryButton.Dispose();
            iPanel.Dispose();
        }

        public View Panel
        {
            get
            {
                return iPanel;
            }
        }


        private void HomeButtonClickHandler(object sender, EventArgs e)
        {
            OnEventHomeClick();
        }

        private void RetryButtonClickHandler(object sender, EventArgs e)
        {
            OnEventRetryClick();
        }

        private void OnEventHomeClick()
        {
            EventHandler<EventArgs> del = EventHomeClick;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private void OnEventRetryClick()
        {
            EventHandler<EventArgs> del = EventRetryClick;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        private View iPanel;
    }

    public class BrowserItem
    {
        public BrowserItem(upnpObject aItem, upnpObject aParent, IconResolver aIconResolver, bool aIsGrouped, bool aIsGroupHeader)
        {
            iLock = new object();
            iItem = aItem;
            iParent = aParent;
            iIsGroupHeader = aIsGroupHeader;
            iIsGrouped = aIsGrouped;
            iTechnicalInfo = string.Empty;
            iDisplayField1 = string.Empty;
            iDisplayField2 = string.Empty;
            iDisplayField3 = string.Empty;
            iIcon = aIconResolver.GetIcon(aItem);
            iCanBrowseDown = iItem is container && !iIsGroupHeader;
        }

        public string OriginalTrackNumber
        {
            get
            {
                return iOriginalTrackNumber;
            }
        }

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

        public Icon<Bitmap> Icon
        {
            get
            {
                EnsureLoaded();
                return iIcon;
            }
        }

        public bool CanBrowseDown
        {
            get
            {
                return iCanBrowseDown;
            }
        }

        public string Id
        {
            get
            {
                return iId;
            }
        }

        public bool IsGrouped
        {
            get
            {
                return iIsGrouped;
            }
        }

        public bool IsGroupHeader
        {
            get
            {
                return iIsGroupHeader;
            }
        }

        public void EnsureLoaded()
        {
            lock (iLock)
            {
                if (!iIsLoaded)
                {
                    ItemInfo info = new ItemInfo(iItem, iParent);
                    ReadOnlyCollection<KeyValuePair<string, string>> displayInfo = info.DisplayItems;
                    if (displayInfo.Count > 0)
                    {
                        iDisplayField1 = displayInfo[0].Value;
                    }
                    if (displayInfo.Count > 1)
                    {
                        iDisplayField2 = displayInfo[1].Value;
                    }
                    if (displayInfo.Count > 2)
                    {
                        iDisplayField3 = displayInfo[2].Value;
                    }
                    iTechnicalInfo = DidlLiteAdapter.Duration(iItem);
                    if (iTechnicalInfo == string.Empty)
                    {
                        iTechnicalInfo = DidlLiteAdapter.Bitrate(iItem);
                    }
                    iOriginalTrackNumber = DidlLiteAdapter.OriginalTrackNumber(iItem);
                    iId = iItem.Id;
                    iIsLoaded = true;

                    // free up reference to item once loaded
                    iItem = null;
                }
            }
        }

        private string iDisplayField1;
        private string iDisplayField2;
        private string iDisplayField3;
        private string iTechnicalInfo;
        private string iOriginalTrackNumber;
        private Icon<Bitmap> iIcon;
        private bool iCanBrowseDown;
        private object iLock;
        private upnpObject iItem;
        private upnpObject iParent;
        private bool iIsLoaded;
        private bool iIsGroupHeader;
        private bool iIsGrouped;
        private string iId;
    }

    #endregion

    #region Notification

    public class NotificationView : INotificationView
    {
        private readonly Stack iStack;
        private Activity iActivity;
        private INotification iNotification;
        private bool iShowOnActivityStart;
        private IPopup iPopup;

        public event EventHandler<EventArgs> EventNotificationUpdated;

        public NotificationView(Stack aStack)
        {
            iStack = aStack;
        }

        public Activity Activity
        {
            set
            {
                if (iActivity != null)
                {
                    if (iPopup != null)
                    {
                        iPopup.EventDismissed -= PopupDismissed;
                        iPopup.Dismiss();
                        iPopup = null;
                    }
                }
                iActivity = value;
                if (value != null)
                {
                    if (iShowOnActivityStart)
                    {
                        ShowPopup(value, iNotification);
                    }
                }
                OnNotificationUpdated();
            }
        }

        public void Update(INotification aNotification, bool aShowNow)
        {
            iNotification = aNotification;
            OnNotificationUpdated();
            if (aShowNow)
            {
                if (iActivity != null)
                {
                    ShowPopup(iActivity, aNotification);
                }
                else
                {
                    iShowOnActivityStart = true;
                }
            }
        }

        private void OnNotificationUpdated()
        {
            var del = EventNotificationUpdated;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        public bool HasNotification
        {
            get
            {
                return iNotification != null && iActivity != null;
            }
        }

        public void ShowNow()
        {
            Assert.Check(HasNotification);
            ShowPopup(iActivity, iNotification);
        }

        private void ShowPopup(Activity aActivity, INotification aNotification)
        {
            LayoutInflater inflater = (LayoutInflater)aActivity.GetSystemService(Context.LayoutInflaterService);
            var popupFactory = new OverlayPopupFactory(aActivity, new Color(0, 0, 0, 200));

            // create the view
            View popupView = inflater.Inflate(Resource.Layout.Notification, null, false);
            var dimensions = GetDimensions(aActivity);

            // create the popup
            iPopup = popupFactory.CreatePopup(popupView, aActivity.FindViewById(Resource.Id.rootview));
            iPopup.EventDismissed += PopupDismissed;
            iPopup.Show();

            // set the width and height
            //var layoutParams = new RelativeLayout.LayoutParams(dimensions.X, dimensions.Y);
            popupView.LayoutParameters.Width = dimensions.X;
            popupView.LayoutParameters.Height = dimensions.Y;
            //= layoutParams;

            // load the webview
            var browser = popupView.FindViewById<WebView>(Resource.Id.notificationwebview);
            browser.LoadUrl(aNotification.Uri);

            var closeButton = popupView.FindViewById<Button>(Resource.Id.notificationclose);
            var dontShowCheckbox = popupView.FindViewById<CheckBox>(Resource.Id.notificationcheckbox);
            var getKazooButton = popupView.FindViewById<Button>(Resource.Id.notificationgetkazoo);

            dontShowCheckbox.Checked = iStack.HelperKinsky.LastNotificationVersion == aNotification.Version;

            closeButton.Click += (s, e) => ClosePopup(dontShowCheckbox.Checked);
            getKazooButton.Click += (s, e) => GetKazoo();
        }

        private void GetKazoo()
        {
            var packageId = "uk.co.linn.kazoo";
            var intent = new Intent(Intent.ActionView.ToString(), Android.Net.Uri.Parse(GetMarketplaceUri(iActivity, packageId)));
            iActivity.StartActivity(intent);
        }

        private string GetMarketplaceUri(Activity aActivity, string aPackageId)
        {
            string storeWebUrl = string.Format("https://play.google.com/store/apps/details?id={0}", aPackageId);
            string marketUrl = string.Format("market://details?id={0}", aPackageId);
            string amazonUrl = string.Format("https://www.amazon.com/gp/mas/dl/android?p={0}", aPackageId);

            var pm = aActivity.PackageManager;

            var url = storeWebUrl;
            var model = Android.OS.Build.Model;
            if (!string.IsNullOrEmpty(model))
            {
                model = model.ToLowerInvariant();
            }
            try
            {
                var installer = pm.GetInstallerPackageName(aActivity.PackageName);
                if (!string.IsNullOrEmpty(installer) && installer == "com.android.vending")
                {
                    url = marketUrl;
                }
                else if (!string.IsNullOrEmpty(installer) && installer == "com.amazon.venezia")
                {
                    url = amazonUrl;
                }
                else if (!string.IsNullOrEmpty(model)
                         && (model.Contains("amazon")
                         || model.Contains("kindle")))
                {
                    url = amazonUrl;
                }
                else
                {
                    try
                    {
                        if (pm.GetPackageInfo("com.android.vending", 0) != null)
                        {
                            url = marketUrl;
                        }
                    }
                    catch (PackageManager.NameNotFoundException)
                    {
                    }
                    try
                    {
                        if (pm.GetPackageInfo("com.amazon.venezia", 0) != null)
                        {
                            url = amazonUrl;
                        }
                    }
                    catch (PackageManager.NameNotFoundException)
                    {
                    }
                }
            }
            catch
            {
            }
            return url;
        }

        private void ClosePopup(bool aDontShowAgain)
        {
            if (aDontShowAgain)
            {
                iNotification.DontShowAgain();
            }
            iPopup.Dismiss();
        }

        private Point GetDimensions(Activity aActivity)
        {
            //DisplayMetrics dm = this.ApplicationContext.Resources.DisplayMetrics;
            //UserLog.WriteLine("Display Metrics: WidthPixels=" + dm.WidthPixels + ", Xdpi=" + dm.Xdpi + ", HeightPixels=" + dm.HeightPixels + ", Ydpi=" + dm.Ydpi);
            //float screenWidth = dm.WidthPixels / dm.Xdpi;
            //float screenHeight = dm.HeightPixels / dm.Ydpi;
            //IWindowManager windowManager = aActivity.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            //Point displaySize = new Point();
            //windowManager.DefaultDisplay.GetSize(displaySize);

            // use the dimensions of the root view
            var rootView = aActivity.FindViewById(Resource.Id.rootview);
            var width = rootView.Width;
            var height = rootView.Height;

            // don't fill the whole screen
            var margin = 50;
            width -= (margin * 2);
            height -= (margin * 2);

            //if (iStack.TabletView)
            //{
            //    // inject some margins into the popup if showing in tablet view
            //    width -= width / 5;
            //    height -= height / 5;
            //}

            return new Point(width, height);
        }

        private void PopupDismissed(object sender, EventArgs args)
        {
            iPopup.EventDismissed -= PopupDismissed;
            iPopup = null;
            iShowOnActivityStart = false;
        }

    }

    #endregion
}