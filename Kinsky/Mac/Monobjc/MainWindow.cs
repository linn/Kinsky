
using System;
using System.Collections.Generic;

using Linn;
using Linn.Kinsky;
using Linn.Topology;

using Upnp;

using Monobjc;
using Monobjc.Cocoa;

using System.Linq;
using Linn.Toolkit.Cocoa;


// View classes that correspond to the MainWindow.xib file

namespace KinskyDesktop
{
    // implementation of Monobjc specific parts of the simply geometry structs
    public partial struct Point
    {
        public Point(NSPoint aPoint)
        {
            X = aPoint.x;
            Y = aPoint.y;
        }

        public NSPoint ToNSPoint()
        {
            return new NSPoint(X, Y);
        }
    }

    public partial struct Rect
    {
        public Rect(NSRect aRect)
        {
            Origin = new Point(aRect.origin);
            Width = aRect.Width;
            Height = aRect.Height;
        }

        public NSRect ToNSRect()
        {
            return new NSRect(Origin.X, Origin.Y, Width, Height);
        }
    }


    // Cocoa type controller for the main window class
    [ObjectiveCClass]
    public class WindowMainController : NSObject, IViewMainWindow
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(WindowMainController));

        public WindowMainController() : base() {}
        public WindowMainController(IntPtr aInstance) : base(aInstance) {}

        public event EventHandler<EventArgs> EventWindowMainClosed;

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // because the window is a borderless one, we need to add it manually to the windows menu
            NSApplication.NSApp.AddWindowsItemTitleFilename(Window,
                                                            NSString.StringWithUTF8String(ModelMain.Instance.Helper.Title),
                                                            false);
            Window.Delegate = this;

            TopViewTrack.ImageViewArtwork.EventClick += ButtonNowPlayingEnterClicked;

            WindowBorderless nowPlaying = WindowNowPlaying.Window as WindowBorderless;
            nowPlaying.EventMouseDown += WindowNowPlayingMouseDown;
            nowPlaying.EventMouseDragged += WindowNowPlayingMouseDragged;
            nowPlaying.EventMouseUp += WindowNowPlayingMouseUp;

            // create animations
            iAnimKompactMode = NSViewAnimationHelper.Create();

            // setup some model eventing
            iModel = ModelMain.Instance;
            iModel.EventSavePlaylist += SavePlaylistHandler;
            iModel.EventUpdateFound += AutoUpdateFound;

            // create the lower level controller
            iController = new ControllerMainWindow(this);
            Window.SetController(iController);
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            // remove window from the window menu
            NSApplication.NSApp.RemoveWindowsItem(Window);

            // disconnect window from model
            iModel.EventSavePlaylist -= SavePlaylistHandler;
            iModel.EventUpdateFound -= AutoUpdateFound;

            WindowBorderless nowPlaying = WindowNowPlaying.Window as WindowBorderless;
            nowPlaying.EventMouseDown -= WindowNowPlayingMouseDown;
            nowPlaying.EventMouseDragged -= WindowNowPlayingMouseDragged;
            nowPlaying.EventMouseUp -= WindowNowPlayingMouseUp;
            
            TopViewTrack.ImageViewArtwork.EventClick -= ButtonNowPlayingEnterClicked;

            iController = null;

            // release the room selection view
            iAnimKompactMode.Release();

            // release the window and other top level nib objects
            Window.Delegate = null;
            Window.Release();

            WindowNowPlaying.Release();
            TopMenuPlaylistDs.Release();
            TopMenuPlaylistRadio.Release();
            TopViewBrowserPane.Release();
            TopViewButtonSave.Release();
            TopViewButtonWasteBin.Release();
            TopViewMediaTime.Release();
            TopViewPlaylistDs.Release();
            TopViewPlaylistPane.Release();
            TopViewPlaylistRadio.Release();
            TopViewPlaylistReceiver.Release();
            TopViewPlayMode.Release();
            TopViewRoomSource.Release();
            TopViewTrack.Release();
            TopViewTransport.Release();
            TopViewVolumeControl.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region NSWindowDelegate implementation
        [ObjectiveCMessage("windowWillClose:")]
        public void MainWindowWillClose(NSNotification aNotification)
        {
            WindowNowPlaying.Window.Close();

            if (EventWindowMainClosed != null)
            {
                EventWindowMainClosed(this, EventArgs.Empty);
            }
        }
        #endregion NSWindowDelegate implementation


        #region NSSplitView delegate implementation
        [ObjectiveCMessage("splitViewDidResizeSubviews:")]
        public void SplitViewDidResizeSubviews(NSNotification aNotification)
        {
            NSView leftView = Window.ViewSplitter.Subviews[0].CastAs<NSView>();

            iController.SetSplitterFraction(leftView.Frame.Width / Window.ViewSplitter.Frame.Width);
        }

        [ObjectiveCMessage("splitView:constrainMaxCoordinate:ofSubviewAt:")]
        public float SplitViewConstrainMaxCoordinate(NSSplitView aSplitView, float aProposed, int aDividerIndex)
        {
            return aSplitView.Bounds.MaxX - 250;
        }

        [ObjectiveCMessage("splitView:constrainMinCoordinate:ofSubviewAt:")]
        public float SplitViewConstrainMinCoordinate(NSSplitView aSplitView, float aProposed, int aDividerIndex)
        {
            return aSplitView.Bounds.MinX + 250;
        }
        #endregion NSSplitView delegate implementation


        #region IViewMainWindow implementation
        void IViewMainWindow.Show(bool aShow)
        {
            if (aShow)
            {
                Window.MakeKeyAndOrderFront(this);
            }
            else
            {
                Window.OrderOut(this);
            }
        }

        public void SetNotificationView (NotificationView aView)
        {
            iNotificationView = aView;
            Window.SetNotificationView (aView);
        }

        void IViewMainWindow.ShowAlertPanel(string aTitle, string aMessage)
        {
            NSAlert alert = new NSAlert();

            alert.AddButtonWithTitle(NSString.StringWithUTF8String("OK"));
            alert.MessageText = NSString.StringWithUTF8String(aTitle);
            alert.InformativeText = NSString.StringWithUTF8String(aMessage);
            alert.AlertStyle = NSAlertStyle.NSWarningAlertStyle;

            alert.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, null, IntPtr.Zero);

            alert.Release();
        }

        bool IViewMainWindow.PointInDragRect(Point aPt)
        {
            NSPoint pt = Window.ConvertScreenToBase(aPt.ToNSPoint());
            return Window.BkgdHeaderCentre.Frame.PointInRect(pt) || Window.BkgdFooterCentre.Frame.PointInRect(pt);
        }

        bool IViewMainWindow.PointInResizeRect(Point aPt)
        {
            NSRect rect = new NSRect(Window.Frame.MaxX - 25, Window.Frame.MinY, 25, 25);
            return rect.PointInRect(aPt.ToNSPoint());
        }

        Rect IViewMainWindow.Rect
        {
            get
            {
                return new Rect(Window.Frame);
            }
            set
            {
                Window.SetFrameDisplay(value.ToNSRect(), true);
                WindowNowPlaying.UpdateFrame(value.ToNSRect());
            }
        }

        bool IViewMainWindow.IsFullscreen
        {
            get
            {
                return Window.IsZoomed;
            }
            set
            {
                if ((!Window.IsZoomed && value) || (Window.IsZoomed && !value))
                {
                    Window.Zoom(this);
                }
            }
        }

        float IViewMainWindow.KompactModeHeight
        {
            get { return ViewBkgdKompact.Frame.Height; }
        }

        void IViewMainWindow.KompactModeEnter(Rect aRect)
        {
            // change the view anchoring while in kompact mode
            ViewMain.AutoresizingMask = NSResizingFlags.NSViewMinYMargin | NSResizingFlags.NSViewWidthSizable;

            // run the animation
            NSViewAnimationHelper.SetFrames(iAnimKompactMode, Window, Window.Frame, aRect.ToNSRect());
            iAnimKompactMode.StartAnimation();

            ViewBkgdKompact.IsHidden = false;
            ViewBkgdNormal.IsHidden = true;
            ViewMain.IsHidden = true;
        }

        void IViewMainWindow.KompactModeExit(Rect aRect)
        {
            ViewBkgdKompact.IsHidden = true;
            ViewBkgdNormal.IsHidden = false;
            ViewMain.IsHidden = false;

            // run the animation
            NSViewAnimationHelper.SetFrames(iAnimKompactMode, Window, Window.Frame, aRect.ToNSRect());
            iAnimKompactMode.StartAnimation();

            // change the view anchoring back to what it was previosuly
            ViewMain.AutoresizingMask = NSResizingFlags.NSViewWidthSizable | NSResizingFlags.NSViewHeightSizable;
        }

        float IViewMainWindow.SplitterFraction
        {
            set
            {
                float splitterPos = Window.ViewSplitter.Frame.Width * value;
                Window.ViewSplitter.SetPositionOfDividerAtIndex(splitterPos, 0);
            }
        }

        void IViewMainWindow.NowPlayingModeEnter()
        {
            WindowNowPlaying.Show();
        }

        void IViewMainWindow.NowPlayingModeExit()
        {
            WindowNowPlaying.Hide();
        }
        #endregion IViewMainWindow implementation


        private void SavePlaylistHandler(object sender, EventArgsSavePlaylist e)
        {
            Assert.Check(!NSApplication.NSApp.InvokeRequired);

            WindowSave windowSave = new WindowSave(e.SaveSupport);
            windowSave.Show();
            windowSave.Release();
        }

        private void AutoUpdateFound(object sender, AutoUpdate.EventArgsUpdateFound e)
        {
            NSAlert alert = new NSAlert();

            alert.AddButtonWithTitle(NSString.StringWithUTF8String("Upgrade Now"));
            alert.AddButtonWithTitle(NSString.StringWithUTF8String("Ask Again Later"));
            alert.AddButtonWithTitle(NSString.StringWithUTF8String("Change Preferences..."));

            alert.MessageText = NSString.StringWithUTF8String("A new version of Kinsky is available." + Environment.NewLine + "Do you want to upgrade?");
            alert.InformativeText = NSString.StringWithUTF8String("Kinsky can automatically check for new and updated versions using its Software Update feature. Select Software Update in Preferences to change the way Kinsky checks for updates.");
            alert.AlertStyle = NSAlertStyle.NSWarningAlertStyle;

            alert.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, AutoUpdateFoundEnd, IntPtr.Zero);

            alert.Release();
        }

        private void AutoUpdateFoundEnd(NSAlert aAlert, int aReturnCode, IntPtr aContextInfo)
        {
            if (aReturnCode == NSAlert.NSAlertFirstButtonReturn)
            {
                aAlert.Window.SendMessage("orderOut:", this);
                NSApplication.NSApp.Delegate.SendMessage("checkForUpdates:", this);
            }
            else if (aReturnCode == NSAlert.NSAlertThirdButtonReturn)
            {
                aAlert.Window.SendMessage("orderOut:", this);
                NSApplication.NSApp.Delegate.SendMessage("preferences:", this);
            }
        }

        private void ButtonNowPlayingEnterClicked(NSEvent aEvent)
        {
            iController.ButtonNowPlayingEnterClicked();
        }

        private void WindowNowPlayingMouseDown(NSEvent aEvent)
        {
            iWindowNowPlayingDragged = false;

            iController.MouseDown(new Point(NSEvent.MouseLocation));
        }
        
        private void WindowNowPlayingMouseDragged(NSEvent aEvent)
        {
            iWindowNowPlayingDragged = true;

            iController.MouseDragged(new Point(NSEvent.MouseLocation));
        }
        
        private void WindowNowPlayingMouseUp(NSEvent aEvent)
        {
            if (!iWindowNowPlayingDragged)
            {
                iController.ButtonNowPlayingExitClicked();
            }

            iController.MouseUp(new Point(NSEvent.MouseLocation));
        }

        public void OpenSettings ()
        {
            OptionDialogMonobjc window = new OptionDialogMonobjc (ModelMain.Instance.Helper.OptionPages);
            AddGetKazooButtonToOptionsWindow (window);
            window.Open ();
        }

        private void AddGetKazooButtonToOptionsWindow (OptionDialogMonobjc aWindow)
        {
            if (iNotificationView != null) {
                NSPoint origin = new NSPoint (10, 10);
                NSSize marginsTopRight = new NSSize (10, 10);
                var buttonHeight = 20;
                var buttonWidth = 250;


                ActionEventHandler clickHandler = (n) => {
                    aWindow.RootView.Window.Close ();
                    iNotificationView.Show ();
                };

                var getKazooButton = new NSButton ();
                getKazooButton.InitWithFrame (new NSRect (origin.x, origin.y, buttonWidth, buttonHeight));

                // todo: should set this button be the default (blue) button, why is it not working?!?
                getKazooButton.SetButtonType (NSButtonType.NSMomentaryPushInButton);
                getKazooButton.KeyEquivalent = @"\r"; //set as default, tshould turn the button blue
                getKazooButton.IsBordered = true; // try this?
                getKazooButton.BezelStyle = NSBezelStyle.NSRoundedBezelStyle; // and this???
                getKazooButton.Cell.IsBezeled = true; // and this??????
                // I give up - thanks, Apple...!

                getKazooButton.Title = "Try Linn's latest control app";

                //var getKazooButton = new ButtonHoverPush ();
                //var buttonCell = getKazooButton.Initialise ();
                //buttonCell.Text = NSString.StringWithUTF8String ("Try Linn's latest control app");

                EventHandler<EventArgs> notificationUpdated = (s, e) => {
                    getKazooButton.IsHidden = !iNotificationView.CanShow;
                    getKazooButton.NeedsDisplay = true;
                };

                getKazooButton.ActionEvent += clickHandler;
                //getKazooButton.EventClicked += clickHandler;

                // set button frame and add to view
                aWindow.RootView.AddSubview (getKazooButton);
                getKazooButton.NeedsDisplay = true;

                // update button visibility
                iNotificationView.EventNotificationUpdated += notificationUpdated;
                notificationUpdated (this, EventArgs.Empty);

                // resize existing views to make room for button inserted at the bottom
                var dialogWindow = aWindow.RootView.Window;
                var buttonHeightOffset = getKazooButton.Frame.Height + origin.x + marginsTopRight.height;
                dialogWindow.SetFrameDisplay (new NSRect (dialogWindow.Frame.origin.x, dialogWindow.Frame.origin.y, dialogWindow.Frame.Width, dialogWindow.Frame.Height + buttonHeightOffset),true);
                aWindow.OptionsContainer.Frame = new NSRect (aWindow.OptionsContainer.Frame.origin.x, aWindow.OptionsContainer.Frame.origin.y + buttonHeightOffset, aWindow.OptionsContainer.Frame.Width, aWindow.OptionsContainer.Frame.Height);

                // add close handler to window
                aWindow.Closed = () => {
                    if (iNotificationView != null) {
                        iNotificationView.EventNotificationUpdated -= notificationUpdated;
                        getKazooButton.ActionEvent -= clickHandler;
                    }
                };
            }
        }

        [ObjectiveCField]
        public WindowMain Window;

        [ObjectiveCField]
        public WindowNowPlayingController WindowNowPlaying;

        [ObjectiveCField]
        public NSView ViewBkgdNormal;

        [ObjectiveCField]
        public NSView ViewBkgdKompact;

        [ObjectiveCField]
        public NSView ViewMain;

        [ObjectiveCField]
        public NSMenu TopMenuPlaylistDs;

        [ObjectiveCField]
        public NSMenu TopMenuPlaylistRadio;

        [ObjectiveCField]
        public ViewBrowserPane TopViewBrowserPane;

        [ObjectiveCField]
        public ViewButtonSave TopViewButtonSave;

        [ObjectiveCField]
        public ViewButtonWasteBin TopViewButtonWasteBin;

        [ObjectiveCField]
        public ViewMediaTime TopViewMediaTime;

        [ObjectiveCField]
        public ViewPlaylistDs TopViewPlaylistDs;

        [ObjectiveCField]
        public ViewPlaylistPane TopViewPlaylistPane;

        [ObjectiveCField]
        public ViewPlaylistRadio TopViewPlaylistRadio;

        [ObjectiveCField]
        public ViewPlaylistReceiver TopViewPlaylistReceiver;

        [ObjectiveCField]
        public ViewPlayMode TopViewPlayMode;

        [ObjectiveCField]
        public ViewRoomSource TopViewRoomSource;

        [ObjectiveCField]
        public ViewTrack TopViewTrack;

        [ObjectiveCField]
        public ViewTransport TopViewTransport;

        [ObjectiveCField]
        public ViewVolumeControl TopViewVolumeControl;

        private ModelMain iModel;
        private ControllerMainWindow iController;
        private NSViewAnimation iAnimKompactMode;
        private bool iWindowNowPlayingDragged;
        private NotificationView iNotificationView;
    }


    // Implementation of the main window view
    [ObjectiveCClass]
    public class WindowMain : NSWindow
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(WindowMain));

        public WindowMain() : base() {}
        public WindowMain(IntPtr aInstance) : base(aInstance) {}

        public void SetController(ControllerMainWindow aController)
        {
            iController = aController;
        }

        public void SetNotificationView (NotificationView aView)
        {
            aView.EventNotificationUpdated += (s, e) => {
                iBadge.IsHidden = !aView.CanShow;
            };
        }

        [ObjectiveCMessage("initWithContentRect:styleMask:backing:defer:")]
        public override Id InitWithContentRectStyleMaskBackingDefer(NSRect aContentRect, NSWindowStyleMasks aWindowStyle, NSBackingStoreType aBufferingType, bool aDeferCreation)
        {
            // overridden to set NSBorderlessWindowMask for no title bar
            this.SendMessageSuper(ThisClass,
                                  "initWithContentRect:styleMask:backing:defer:",
                                  aContentRect,
                                  NSWindowStyleMasks.NSBorderlessWindowMask,
                                  aBufferingType, aDeferCreation);
            return this;
        }

        [ObjectiveCMessage("initWithContentRect:styleMask:backing:defer:screen:")]
        public override Id InitWithContentRectStyleMaskBackingDeferScreen(NSRect aContentRect, NSWindowStyleMasks aWindowStyle, NSBackingStoreType aBufferingType, bool aDeferCreation, NSScreen aScreen)
        {
            // overridden to set NSBorderlessWindowMask for no title bar
            this.SendMessageSuper(ThisClass,
                                  "initWithContentRect:styleMask:backing:defer:screen:",
                                  aContentRect,
                                  NSWindowStyleMasks.NSBorderlessWindowMask,
                                  aBufferingType, aDeferCreation, aScreen);
            return this;
        }

        public override bool CanBecomeKeyWindow
        {
            [ObjectiveCMessage("canBecomeKeyWindow")]
            get { return true; }
        }

        public override bool CanBecomeMainWindow
        {
            [ObjectiveCMessage("canBecomeMainWindow")]
            get { return true; }
        }

        [ObjectiveCMessage("windowWillUseStandardFrame:defaultFrame:")]
        public NSRect WindowWillUseStandardFrame(NSWindow aWindow, NSRect aDefaultFrame)
        {
            return this.Screen.VisibleFrame;
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            this.SendMessageSuper(ThisClass, "mouseDown:", aEvent);

            iController.MouseDown(new Point(NSEvent.MouseLocation));
        }

        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent aEvent)
        {
            this.SendMessageSuper(ThisClass, "mouseDragged:", aEvent);

            iController.MouseDragged(new Point(NSEvent.MouseLocation));
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            this.SendMessageSuper(ThisClass, "mouseUp:", aEvent);

            iController.MouseUp(new Point(NSEvent.MouseLocation));
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            iBadge = CreateBadge ();
            this.ContentView.AddSubview (iBadge);
            iBadge.IsHidden = true;

            // make the window transparent
            this.IsOpaque = false;
            // this line needs to be added when the views are CoreAnimation layer-backed i.e. so stuff can animate
            this.BackgroundColor = NSColor.ClearColor;


            // set up background images for the application
            BkgdHeaderLeft.Image = Properties.Resources.ImageTopLeftEdge;
            BkgdHeaderCentre.Image = Properties.Resources.ImageTopFiller;
            BkgdHeaderRight.Image = Properties.Resources.ImageTopRightEdge;
            BkgdFooterLeft.Image = Properties.Resources.ImageBottomLeftEdge;
            BkgdFooterCentre.Image = Properties.Resources.ImageBottomFiller;
            BkgdFooterRight.Image = Properties.Resources.ImageBottomRightEdge;
            BkgdBorderLeft.Image = Properties.Resources.ImageLeftFiller;
            BkgdBorderRight.Image = Properties.Resources.ImageRightFiller;
            BkgdTopBar.Image = Properties.Resources.ImageTopBarFiller;

            BkgdCentre.BackgroundColour = NSColor.BlackColor;
            BkgdCentre.SetOpaque(true);

            KompactLeft.Image = Properties.Resources.ImageKmodeLeft;
            KompactCentre.Image = Properties.Resources.ImageKmodeFiller;
            KompactRight.Image = Properties.Resources.ImageKmodeRight;


            // setup the window buttons
            ButtonClose.Initialise(Properties.Resources.IconOsXClose,
                                   Properties.Resources.IconOsXCloseMouse,
                                   Properties.Resources.IconOsXCloseTouch,
                                   PanelWindowButtons);
            ButtonMinimise.Initialise(Properties.Resources.IconOsXMinimise,
                                      Properties.Resources.IconOsXMinimiseMouse,
                                      Properties.Resources.IconOsXMinimiseTouch,
                                      PanelWindowButtons);
            ButtonMaximise.Initialise(Properties.Resources.IconOsXMaximise,
                                      Properties.Resources.IconOsXMaximiseMouse,
                                      Properties.Resources.IconOsXMaximiseTouch,
                                      PanelWindowButtons);
            ButtonKompact.Initialise(Properties.Resources.IconOsXMini,
                                     Properties.Resources.IconOsXMiniMouse,
                                     Properties.Resources.IconOsXMiniTouch,
                                     PanelWindowButtons);
            ButtonSettings.Initialise (Properties.Resources.IconSettings);


            // setup handlers for widgets
            ButtonClose.EventClicked += ButtonCloseClicked;
            ButtonMaximise.EventClicked += ButtonMaximiseClicked;
            ButtonMinimise.EventClicked += ButtonMinimiseClicked;
            ButtonKompact.EventClicked += ButtonKompactClicked;
            ButtonSettings.EventClicked += ButtonSettingsClicked;
            iBadge.EventClick += ButtonSettingsClicked;
        }

        private ImageViewClickable CreateBadge ()
        {
            var badge = new ImageViewClickable ();
            badge.Frame = new NSRect (30, 20, 25, 25);
            badge.Image = Properties.Resources.Badge;
            return badge;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            ButtonClose.EventClicked -= ButtonCloseClicked;
            ButtonMaximise.EventClicked -= ButtonMaximiseClicked;
            ButtonMinimise.EventClicked -= ButtonMinimiseClicked;
            ButtonKompact.EventClicked -= ButtonKompactClicked;
            ButtonSettings.EventClicked -= ButtonSettingsClicked;
            iBadge.EventClick -= ButtonSettingsClicked;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        // attribute used to this method also gets called in response to main menu
        [ObjectiveCMessage("mainWindowZoom:")]
        public void ButtonMaximiseClicked(Id aSender)
        {
            if (this.IsZoomed)
            {
                iController.ButtonRestoreClicked();
            }
            else
            {
                iController.ButtonMaximiseClicked();
            }
        }

        // attribute used to this method also gets called in response to main menu
        [ObjectiveCMessage("mainWindowMinimise:")]
        public void ButtonMinimiseClicked(Id aSender)
        {
            this.Miniaturize(aSender);
        }

        // attribute used to this method also gets called in response to main menu
        [ObjectiveCMessage("kompactMode:")]
        public void ButtonKompactClicked(Id aSender)
        {
            iController.ButtonKompactClicked();
        }

        private void ButtonCloseClicked(Id aSender)
        {
            this.Close();
        }

        private void ButtonSettingsClicked (Id aSender)
        {
            iController.OpenSettings ();
        }

        [ObjectiveCField]
        public NSSplitView ViewSplitter;

        [ObjectiveCField]
        public NSImageView BkgdHeaderLeft;

        [ObjectiveCField]
        public NSImageView BkgdHeaderCentre;

        [ObjectiveCField]
        public NSImageView BkgdHeaderRight;

        [ObjectiveCField]
        public NSImageView BkgdFooterLeft;

        [ObjectiveCField]
        public NSImageView BkgdFooterCentre;

        [ObjectiveCField]
        public NSImageView BkgdFooterRight;

        [ObjectiveCField]
        public NSImageView BkgdBorderLeft;

        [ObjectiveCField]
        public NSImageView BkgdBorderRight;

        [ObjectiveCField]
        public NSImageView BkgdTopBar;

        [ObjectiveCField]
        public ViewEmpty BkgdCentre;

        [ObjectiveCField]
        public NSImageView KompactLeft;

        [ObjectiveCField]
        public NSImageView KompactCentre;

        [ObjectiveCField]
        public NSImageView KompactRight;

        [ObjectiveCField]
        public ButtonHoverPush ButtonClose;

        [ObjectiveCField]
        public ButtonHoverPush ButtonMinimise;

        [ObjectiveCField]
        public ButtonHoverPush ButtonMaximise;

        [ObjectiveCField]
        public ButtonHoverPush ButtonKompact;

        [ObjectiveCField]
        public ViewHoverTracker PanelWindowButtons;

        [ObjectiveCField]
        public ButtonHoverPush ButtonSettings;

        private ControllerMainWindow iController;
        private ImageViewClickable iBadge;
    }


    // interface for an object that tracks mouse events
    public delegate void DMouseEvent(NSEvent aEvent);
    public interface IMouseTracker
    {
        event DMouseEvent EventMouseEntered;
        event DMouseEvent EventMouseExited;
        event DMouseEvent EventMouseMoved;
        event DMouseEvent EventMouseUp;
        event DMouseEvent EventMouseDown;
    }


    // Transparent view that can be used for defining hover rects
    [ObjectiveCClass]
    public class ViewHoverTracker : NSView, IMouseTracker
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewHoverTracker));

        public ViewHoverTracker()
            : base()
        {
        }

        public ViewHoverTracker(IntPtr aInstance)
            : base(aInstance)
        {
        }

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect aFrame)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ThisClass, "initWithFrame:", aFrame);

            iTrackingArea = TrackerHelper.Create(this, this.Bounds, true);

            return this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iTrackingArea.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("updateTrackingAreas")]
        public override void UpdateTrackingAreas()
        {
            this.SendMessageSuper(ThisClass, "updateTrackingAreas");

            TrackerHelper.Destroy(this, iTrackingArea);
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, true);
        }

        [ObjectiveCMessage("mouseEntered:")]
        public override void MouseEntered (NSEvent aEvent)
        {
            if (!FireEvent(EventMouseEntered, aEvent))
            {
                this.SendMessageSuper(ThisClass, "mouseEntered:", aEvent);
            }
        }

        [ObjectiveCMessage("mouseExited:")]
        public override void MouseExited (NSEvent aEvent)
        {
            if (!FireEvent(EventMouseExited, aEvent))
            {
                this.SendMessageSuper(ThisClass, "mouseExited:", aEvent);
            }
        }

        [ObjectiveCMessage("mouseMoved:")]
        public override void MouseMoved (NSEvent aEvent)
        {
            if (!FireEvent(EventMouseMoved, aEvent))
            {
                this.SendMessageSuper(ThisClass, "mouseMoved:", aEvent);
            }
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            if (!FireEvent(EventMouseUp, aEvent))
            {
                this.SendMessageSuper(ThisClass, "mouseUp:", aEvent);
            }
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            if (!FireEvent(EventMouseDown, aEvent))
            {
                this.SendMessageSuper(ThisClass, "mouseDown:", aEvent);
            }
        }

        private bool FireEvent(DMouseEvent aHandler, NSEvent aEvent)
        {
            if (aHandler != null)
            {
                aHandler(aEvent);
                return true;
            }
            else
            {
                return false;
            }
        }

        #region IMouseTracker implementation
        public event DMouseEvent EventMouseEntered;
        public event DMouseEvent EventMouseExited;
        public event DMouseEvent EventMouseMoved;
        public event DMouseEvent EventMouseUp;
        public event DMouseEvent EventMouseDown;
        #endregion IMouseTracker implementation

        private NSTrackingArea iTrackingArea;
    }


    // View for custom drawing of the split view divider
    [ObjectiveCClass]
    public class ViewSplitter : NSSplitView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewSplitter));

        public ViewSplitter() : base() {}
        public ViewSplitter(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            DividerStyle = NSSplitViewDividerStyle.NSSplitViewDividerStyleThin;
        }

        public override NSColor DividerColor
        {
            [ObjectiveCMessage("dividerColor")]
            get
            {
                return NSColor.ColorWithCalibratedWhiteAlpha(0.4f, 1.0f);
            }
        }

        public override float DividerThickness
        {
            [ObjectiveCMessage("dividerThickness")]
            get
            {
                return 2.0f;
            }
        }

        [ObjectiveCMessage("drawDividerInRect:")]
        public override void DrawDividerInRect(NSRect aRect)
        {
            NSRect rect = new NSRect(aRect.MinX, aRect.MinY + 43.0f, aRect.Width, aRect.Height - 43.0f);
            this.SendMessageSuper(ThisClass, "drawDividerInRect:", rect);
        }
    }


    // View for the size slider
    [ObjectiveCClass]
    public class ViewSlider : NSSlider
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewSlider));

        public ViewSlider() : base() {}
        public ViewSlider(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("resignFirstResponder")]
        public override bool ResignFirstResponder()
        {
            this.Animator.IsHidden = true;
            return this.SendMessageSuper<bool>(ThisClass, "resignFirstResponder");
        }
    }


    // View encapsulating the main components of the browser view - the content pane, breadcrumb, back button etc...
    [ObjectiveCClass]
    public class ViewBrowserPane : NSObject, IViewBrowserPane, IViewAddBookmark
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewBrowserPane));

        public ViewBrowserPane() : base() {}
        public ViewBrowserPane(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise appearance of browser components
            ViewBrowserContent.BackgroundColour = NSColor.GreenColor;
            ViewBrowserContent.SetOpaque(true);

            ImageLogo.Image = Properties.Resources.ImageLinnLogo;

            ButtonBack.Initialise(Properties.Resources.IconBack,
                                  Properties.Resources.IconBackDown);

            Breadcrumb.BackgroundColor = NSColor.ClearColor;

            IButtonHoverType2 button;
            button = ButtonBookmarkAdd.Initialise();
            button.Text = NSString.StringWithUTF8String("+");

            button = ButtonBookmarkList.Initialise(Properties.Resources.IconBookmark);
            button.ImageWidth = 25.0f;

            ButtonList.Initialise(Properties.Resources.IconList, Properties.Resources.IconThumbs);
            ButtonSize.Initialise(Properties.Resources.IconSize);

            // initialise UI state from model
            if (ModelMain.Instance.Helper.ContainerView.Native == 1)
            {
                // list view is visible
                ButtonList.IsOn = true;
                SliderSize.FloatValue = ModelMain.Instance.Helper.ContainerSizeList.Native;
            }
            else
            {
                // thumbs view is visible
                ButtonList.IsOn = false;
                SliderSize.FloatValue = ModelMain.Instance.Helper.ContainerSizeThumbs.Native;
            }

            // setup some event handlers
            ButtonBack.EventClicked += ButtonBackClicked;
            ButtonList.EventClicked += ButtonListClicked;
            ButtonSize.EventClicked += ButtonSizeClicked;
            Breadcrumb.ActionEvent += BreadcrumbClicked;
            SliderSize.ActionEvent += SliderSizeChanged;
            ButtonBookmarkAdd.EventClicked += ButtonBookmarkAddClicked;
            ButtonBookmarkList.EventClicked += ButtonBookmarkListClicked;

            // create and initialise the view for the browser contents
            iViewBrowserContent = new ViewBrowserContent();
            iViewBrowserContent.Initialise(ModelMain.Instance.ModelBrowser, this);
            iViewBrowserContent.ViewRoot.Frame = ViewBrowserContent.Bounds;
            ViewBrowserContent.AddSubview(iViewBrowserContent.ViewRoot);

            // create the controller for the browser pane
            iController = new ControllerBrowser(this,
                                                ModelMain.Instance.Helper.ContainerView,
                                                ModelMain.Instance.Helper.ContainerSizeThumbs,
                                                ModelMain.Instance.Helper.ContainerSizeList);

            // link up model eventing
            ModelMain.Instance.ModelBrowser.EventBreadcrumbChanged += BrowserLocationChanged;
            ModelMain.Instance.ModelBrowser.EventLocationChanged += BrowserLocationChanged;
            ModelMain.Instance.ModelBrowser.EventLocationFailed += BrowserLocationChanged;
            ModelMain.Instance.ModelTooltips.EventChanged += ShowTooltipsChanged;

            // manually setup the controller here
            iController.SetModel(ModelMain.Instance.ModelBrowser);
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            // cleanup event handlers
            ButtonBack.EventClicked -= ButtonBackClicked;
            ButtonList.EventClicked -= ButtonListClicked;
            ButtonSize.EventClicked -= ButtonSizeClicked;
            Breadcrumb.ActionEvent -= BreadcrumbClicked;
            SliderSize.ActionEvent -= SliderSizeChanged;
            ButtonBookmarkAdd.EventClicked -= ButtonBookmarkAddClicked;
            ButtonBookmarkList.EventClicked -= ButtonBookmarkListClicked;

            ModelMain.Instance.ModelTooltips.EventChanged -= ShowTooltipsChanged;
            ModelMain.Instance.ModelBrowser.EventBreadcrumbChanged -= BrowserLocationChanged;
            ModelMain.Instance.ModelBrowser.EventLocationChanged -= BrowserLocationChanged;
            ModelMain.Instance.ModelBrowser.EventLocationFailed -= BrowserLocationChanged;

            iViewBrowserContent.ViewRoot.RemoveFromSuperview();
            iViewBrowserContent.Release();
            iViewBrowserContent = null;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region IViewBrowserPane methods
        public void SetBreadcrumb(IList<string> aPath)
        {
            NSMutableArray pathCompCells = new NSMutableArray();

            for (int i=0 ; i<aPath.Count ; i++)
            {
                BreadcrumbCell pathCompCell = new BreadcrumbCell();
                pathCompCell.Title = NSString.StringWithUTF8String(aPath[i]);
                pathCompCell.TextColor = NSColor.WhiteColor;
                pathCompCell.Font = FontManager.FontLarge;
                pathCompCell.IsLast = (i == aPath.Count - 1);

                pathCompCells.Add(pathCompCell);

                pathCompCell.Release();
            }

            Breadcrumb.PathComponentCells = pathCompCells;
            pathCompCells.Release();
        }

        public void EnableButtonUp(bool aEnable)
        {
            ButtonBack.Enabled = aEnable;
        }

        public void EnableButtonSize(bool aEnable)
        {
            ButtonSize.Enabled = aEnable;
        }

        public void EnableButtonList(bool aEnable)
        {
            ButtonList.Enabled = aEnable;
        }

        public void SetSliderSizeValue(float aValue)
        {
            SliderSize.FloatValue = aValue;
        }
        #endregion IViewBrowserPane methods


        private void BrowserLocationChanged(object sender, EventArgs e)
        {
            iController.BrowserLocationChanged(sender);
        }

        private void ButtonBackClicked(Id sender)
        {
            iController.ButtonUpClicked();
        }

        private void ButtonListClicked(Id sender)
        {
            // for the ButtonList toggle button:
            //  - off state show list icon, so thumbs view should be visible
            //  - on state shows thumbs icon, so list view should be visible
            iController.ButtonListClicked(ButtonList.IsOn);
            UpdateTooltips();
        }

        private void ButtonSizeClicked(Id sender)
        {
            if (SliderSize.IsHidden)
            {
                SliderSize.Animator.IsHidden = false;
                SliderSize.Window.MakeFirstResponder(SliderSize);
            }
            else
            {
                SliderSize.Animator.IsHidden = true;
            }
        }

        private void ButtonBookmarkListClicked(Id sender)
        {
            // calculate the mid point of the button in screen coordinates
            NSPoint anchor = new NSPoint(ButtonBookmarkList.Bounds.MidX, ButtonBookmarkList.Bounds.MidY);
            anchor = ButtonBookmarkList.ConvertPointToView(anchor, null);
            anchor = ButtonBookmarkList.Window.ConvertBaseToScreen(anchor);

            // calculate the height of the window
            float height = Math.Max(ViewBrowserContent.Bounds.Height, 400);

            // create the view and popover
            ViewSelectionBookmark view = new ViewSelectionBookmark(ModelMain.Instance.Helper.BookmarkManager, ModelMain.Instance.ModelBrowser);
            WindowPopover popover = new WindowPopover(view);
            popover.Show(anchor, false, new NSSize(500, height));
        }

        private void ButtonBookmarkAddClicked(Id sender)
        {
            ShowAddBookmark(ModelMain.Instance.ModelBrowser.Location);
        }

        #region IViewAddBookmark methods
        public void ShowAddBookmark(Location aLocation)
        {
            if (aLocation != null)
            {
                // calculate the mid point of the button in screen coordinates
                NSPoint anchor = new NSPoint(ButtonBookmarkAdd.Bounds.MidX, ButtonBookmarkAdd.Bounds.MidY);
                anchor = ButtonBookmarkAdd.ConvertPointToView(anchor, null);
                anchor = ButtonBookmarkAdd.Window.ConvertBaseToScreen(anchor);
    
                // create the view and popover
                ViewAddBookmark view = new ViewAddBookmark(ModelMain.Instance.Helper.BookmarkManager, aLocation);
                WindowPopover popover = new WindowPopover(view);
                popover.Show(anchor, false, new NSSize(500, 220));
            }
        }
        #endregion IViewAddBookmark methods

        private void SliderSizeChanged(Id sender)
        {
            iController.SliderSizeChanged(SliderSize.FloatValue);
        }

        private void BreadcrumbClicked(Id sender)
        {
            if (Breadcrumb.ClickedPathComponentCell != null)
            {
                string pathCompString = Breadcrumb.ClickedPathComponentCell.Title.ToString();

                iController.BreadcrumbClicked(pathCompString);
            }
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            iShowTooltips = e.Show;
            UpdateTooltips();
        }

        private void UpdateTooltips()
        {
            if (iShowTooltips)
            {
                ButtonBack.ToolTip = NSString.StringWithUTF8String("Move up a directory");
                ButtonList.ToolTip = NSString.StringWithUTF8String(ButtonList.IsOn ? "Switch browser to thumbnail view" : "Switch browser to list view");
                ButtonSize.ToolTip = NSString.StringWithUTF8String("Change size of browser images");
                ButtonBookmarkAdd.ToolTip = NSString.StringWithUTF8String("Bookmark this location");
                ButtonBookmarkList.ToolTip = NSString.StringWithUTF8String("Show bookmarks");
            }
            else
            {
                ButtonBack.ToolTip = null;
                ButtonList.ToolTip = null;
                ButtonSize.ToolTip = null;
                ButtonBookmarkAdd.ToolTip = null;
                ButtonBookmarkList.ToolTip = null;
            }
        }

        [ObjectiveCField]
        public NSPathControl Breadcrumb;

        [ObjectiveCField]
        public ButtonHoverPush ButtonBack;

        [ObjectiveCField]
        public ButtonHoverPush ButtonBookmarkAdd;

        [ObjectiveCField]
        public ButtonHoverPush ButtonBookmarkList;

        [ObjectiveCField]
        public ButtonHoverPush ButtonList;

        [ObjectiveCField]
        public ButtonHoverPush ButtonSize;

        [ObjectiveCField]
        public NSSlider SliderSize;

        [ObjectiveCField]
        public NSImageView ImageLogo;

        [ObjectiveCField]
        public NSView ViewBackground;

        [ObjectiveCField]
        public ViewEmpty ViewBrowserContent;

        private ViewBrowserContent iViewBrowserContent;
        private ControllerBrowser iController;
        private bool iShowTooltips;
    }


    // Custom view class for drawing the background of the breadcrumb
    [ObjectiveCClass]
    public class ViewBreadcrumbBkgd : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewBreadcrumbBkgd));

        public ViewBreadcrumbBkgd() : base() {}
        public ViewBreadcrumbBkgd(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            this.SendMessageSuper(ThisClass, "drawRect:", aRect);

            NSImage left = Properties.Resources.ImageBoxLeft;
            NSImage right = Properties.Resources.ImageBoxRight;
            NSImage filler = Properties.Resources.ImageBoxFiller;

            NSRect leftRect = new NSRect(Bounds.MinX, Bounds.MinY, left.Size.width, Bounds.Height);
            left.DrawInRectFromRectOperationFraction(leftRect, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);

            NSRect rightRect = new NSRect(Bounds.MaxX - right.Size.width, Bounds.MinY, right.Size.width, Bounds.Height);
            right.DrawInRectFromRectOperationFraction(rightRect, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);

            NSRect fillerRect = new NSRect(Bounds.MinX + left.Size.width, Bounds.MinY, Bounds.Width - left.Size.width - right.Size.width, Bounds.Height);
            filler.DrawInRectFromRectOperationFraction(fillerRect, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);
        }
    }


    // Cell class for the breadcrumb - for drawing the little triangles in the path
    [ObjectiveCClass]
    public class BreadcrumbCell : NSPathComponentCell
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(BreadcrumbCell));

        public BreadcrumbCell() : base() {}
        public BreadcrumbCell(IntPtr aInstance) : base(aInstance) {}

        public bool IsLast
        {
            set { iIsLast = value; }
        }

        [ObjectiveCMessage("drawWithFrame:inView:")]
        public void Draw(NSRect aCellFrame, NSView aView)
        {
            this.SendMessageSuper(ThisClass, "drawWithFrame:inView:", aCellFrame, aView);

            if (!iIsLast)
            {
                NSBezierPath path = new NSBezierPath();

                float arrowX = aCellFrame.origin.x + aCellFrame.Width - aCellFrame.Height + 15.0f;
                float arrowY = aCellFrame.origin.y + (aCellFrame.Height / 2) - 3.0f;
                NSRect arrowRect = new NSRect(arrowX, arrowY, 5.0f, 6.0f);

                NSColor.WhiteColor.SetFill();
                path.MoveToPoint(new NSPoint(arrowRect.origin.x, arrowRect.origin.y));
                path.LineToPoint(new NSPoint(arrowRect.origin.x, arrowRect.origin.y + arrowRect.size.height));
                path.LineToPoint(new NSPoint(arrowRect.origin.x + arrowRect.size.width, arrowRect.origin.y + (arrowRect.size.height / 2)));
                path.LineToPoint(new NSPoint(arrowRect.origin.x, arrowRect.origin.y));
                path.ClosePath();
                path.Fill();
                path.Release();
            }
        }

        private bool iIsLast;
    }


    public static class SourceImages
    {
        public static NSImage GetImage(string aType)
        {
            switch (aType)
            {
            case "Aux":
            case "Analog":
            case "Spdif":
            case "Toslink":
            default:
                return Properties.Resources.IconSource;

            case "Disc":
                return Properties.Resources.IconCd;

            case "Playlist":
                return Properties.Resources.IconPlaylistSource;

            case "Radio":
            case "Tuner":
                return Properties.Resources.IconSourceRadio;

            case "UpnpAv":
                return Properties.Resources.IconUpnp;

            case "Receiver":
                return Properties.Resources.IconSender;
            }
        }
    }


    // View encapsulating the various components of the playlist panel
    [ObjectiveCClass]
    public class ViewPlaylistPane : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewPlaylistPane));

        public ViewPlaylistPane() : base() {}
        public ViewPlaylistPane(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise appearance
            ViewPlaylistContent.BackgroundColour = NSColor.BlackColor;
            ViewPlaylistContent.SetOpaque(true);

            // set playlist content views to be hidden
            ViewPlaylistAux.IsHidden = true;

            // setup model eventing
            ModelMain model = ModelMain.Instance;
            model.ModelPlaylistAux.EventOpen += ModelPlaylistAuxOpen;
            model.ModelPlaylistAux.EventClose += ModelPlaylistAuxClose;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            // clear eventing
            ModelMain model = ModelMain.Instance;
            model.ModelPlaylistAux.EventOpen -= ModelPlaylistAuxOpen;
            model.ModelPlaylistAux.EventClose -= ModelPlaylistAuxClose;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        private void ModelPlaylistAuxOpen(object sender, EventArgs e)
        {
            ViewPlaylistAux.Image = SourceImages.GetImage(ModelMain.Instance.ModelPlaylistAux.Type);
            ViewPlaylistAux.IsHidden = false;
        }

        private void ModelPlaylistAuxClose(object sender, EventArgs e)
        {
            ViewPlaylistAux.Image = null;
            ViewPlaylistAux.IsHidden = true;
        }

        [ObjectiveCField]
        public NSView ViewRoot;

        [ObjectiveCField]
        public ViewEmpty ViewPlaylistContent;

        [ObjectiveCField]
        public NSImageView ViewPlaylistAux;
    }


    // View class to handle the receiver source
    [ObjectiveCClass]
    public class ViewPlaylistReceiver : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewPlaylistReceiver));

        public ViewPlaylistReceiver() : base() {}
        public ViewPlaylistReceiver(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise the view to be hidden
            ViewRoot.IsHidden = true;

            NSTableColumn gotoRoomColumn = ViewTable.TableColumnWithIdentifier(NSString.StringWithUTF8String("gotoRoom"));
            NSButtonCell gotoRoomCell = gotoRoomColumn.DataCell.CastAs<NSButtonCell>();
            gotoRoomCell.ActionEvent += ButtonGotoRoomClicked;

            ViewTable.DoubleActionEvent += TableViewItemDoubleClick;

            iArtworkCache = ArtworkCacheInstance.Instance;
            iArtworkCache.EventImageAdded += ArtworkCacheImageAdded;

            // setup model eventing
            iModel = ModelMain.Instance.ModelPlaylistReceiver;
            iModel.EventOpen += ModelOpen;
            iModel.EventInitialised += ModelInitialised;
            iModel.EventClose += ModelClose;
            iModel.EventSendersChanged += ModelSendersChanged;
            iModel.EventCurrentChanged += ModelCurrentChanged;

            iModelRooms = ModelMain.Instance.ModelRoomList;

            // setup delegate and datasource - if this is done in IB, the methods can be called
            // before awakeFromNib which causes complications
            ViewTable.Delegate = this;
            ViewTable.DataSource = this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            // clear model eventing
            iModel.EventOpen -= ModelOpen;
            iModel.EventInitialised -= ModelInitialised;
            iModel.EventClose -= ModelClose;
            iModel.EventSendersChanged -= ModelSendersChanged;
            iModel.EventCurrentChanged -= ModelCurrentChanged;

            iArtworkCache.EventImageAdded -= ArtworkCacheImageAdded;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        #region eventing from IModelPlaylistReceiver
        private void ModelOpen(object sender, EventArgs e)
        {
            iUpdatedSinceOpened = false;

            ViewTable.SizeToFit();
        }

        private void ModelInitialised(object sender, EventArgs e)
        {
            ViewRoot.IsHidden = false;
            ViewTable.ReloadData();
        }

        private void ModelClose(object sender, EventArgs e)
        {
            ViewRoot.IsHidden = true;
            ViewTable.ReloadData();
        }

        private void ModelSendersChanged(object sender, EventArgs e)
        {
            ViewTable.ReloadData();

            // scroll to current sender on first update
            if (!iUpdatedSinceOpened)
            {
                ViewTable.ScrollRowToVisible(iModel.CurrentSenderIndex);
                iUpdatedSinceOpened = true;
            }
        }

        private void ModelCurrentChanged(object sender, EventArgs e)
        {
            ViewTable.ReloadData();
        }
        #endregion eventing from IModelPlaylistReceiver


        #region NSTableView data source protocol methods
        [ObjectiveCMessage("numberOfRowsInTableView:")]
        public int TableViewNumberOfRows(NSTableView aTableView)
        {
            return iModel.Senders.Count;
        }

        [ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
        public Id TableViewObjectValue(NSTableView aTableView, NSTableColumn aTableColumn, int aRowIndex)
        {
            if (aRowIndex < 0 && aRowIndex >= iModel.Senders.Count)
                return null;

            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(kKeyImage) == NSComparisonResult.NSOrderedSame)
            {
                if (aRowIndex == iModel.CurrentSenderIndex)
                {
                    return Properties.Resources.IconPlaying;
                }
                else
                {
                    NSImage artwork = Properties.Resources.IconSender;

                    Uri artworkUri = DidlLiteAdapter.ArtworkUri(iModel.Senders[aRowIndex].Metadata[0]);
                    if (artworkUri != null)
                    {
                        try
                        {
                            ArtworkCache.Item item = iArtworkCache.Artwork(artworkUri);
                            if (item != null && !item.Failed)
                            {
                                artwork = item.Image;
                            }
                        }
                        catch (Exception) {}
                    }

                    return artwork;
                }
            }
            else if (identifier.Compare(kKeyTitle) == NSComparisonResult.NSOrderedSame)
            {
                return NSString.StringWithUTF8String(iModel.Senders[aRowIndex].FullName);
            }
            else
            {
                return null;
            }
        }
        #endregion NSTableView data source protocol methods


        #region NSTableView delegate functions
        [ObjectiveCMessage("tableView:willDisplayCell:forTableColumn:row:")]
        public void TableViewWillDisplayCell(NSTableView aView, Id aCell, NSTableColumn aTableColumn, int aRowIndex)
        {
            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(kKeyImage) == NSComparisonResult.NSOrderedSame)
            {
                NSImageCell cell = aCell.CastAs<NSImageCell>();

                if (aRowIndex == iModel.CurrentSenderIndex)
                {
                    cell.ImageScaling = NSImageScaling.NSImageScaleNone;
                    cell.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
                }
                else
                {
                    cell.ImageScaling = NSImageScaling.NSImageScaleProportionallyUpOrDown;
                    cell.ImageAlignment = NSImageAlignment.NSImageAlignTop;
                }
            }

            if (identifier.Compare(kKeyTitle) == NSComparisonResult.NSOrderedSame)
            {
                NSTextFieldCell textCell = aCell.CastAs<NSTextFieldCell>();

                textCell.TextColor = NSColor.WhiteColor;
                textCell.Font = FontManager.FontSemiLarge;
            }

            if (identifier.Compare(kKeyGotoRoom) == NSComparisonResult.NSOrderedSame)
            {
                NSButtonCell cell = aCell.CastTo<NSButtonCell>();

                if (aRowIndex == iModel.CurrentSenderIndex)
                {
                    Linn.Kinsky.IRoom room = FindRoom(iModel.Channel);
                    if (room != null)
                    {
                        cell.Image = Properties.Resources.IconRoom;
                        cell.AlternateImage = Properties.Resources.IconRoom;
                        cell.IsEnabled = true;
                    }
                    else
                    {
                        cell.Image = null;
                        cell.AlternateImage = null;
                        cell.IsEnabled = false;
                    }
                }
                else
                {
                    cell.Image = null;
                    cell.AlternateImage = null;
                    cell.IsEnabled = false;
                }
            }
        }
        #endregion NSTableView delegate functions


        private void TableViewItemDoubleClick(Id aSender)
        {
            iModel.CurrentSenderIndex = ViewTable.ClickedRow;
        }

        private void ArtworkCacheImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
        {
            // this is called from the artwork cache thread
            ViewTable.BeginInvoke((MethodInvoker)delegate()
            {
                ViewTable.ReloadData();
            });
        }

        private void ButtonGotoRoomClicked(Id aSender)
        {
            Linn.Kinsky.Room room = FindRoom (iModel.Channel);
            if (room != null)
            {
                Console.WriteLine("Goto room: " + room.Name);
                iModelRooms.SelectedItem = room;
            }
        }

        private Linn.Kinsky.Room FindRoom(Channel aChannel)
        {
            Linn.Kinsky.Room result = null;
            if (aChannel != null)
            {
                foreach(ModelSender modelSender in iModel.Senders)
                {
                    foreach(resource r in modelSender.Metadata[0].Res)
                    {
                        if (r.Uri == aChannel.Uri)
                        {
                            result = (from room in iModelRooms.Items where room.Name == modelSender.Room select room).SingleOrDefault();
                            if (result != null)
                            {
                                break;
                            }
                        }
                    }
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        [ObjectiveCField]
        public NSScrollView ViewRoot;

        [ObjectiveCField]
        public NSTableView ViewTable;

        private static NSString kKeyImage = new NSString("image");
        private static NSString kKeyTitle = new NSString("title");
        private static NSString kKeyGotoRoom = new NSString("gotoRoom");

        private IModelPlaylistReceiver iModel;
        private IModelSelectionList<Linn.Kinsky.Room> iModelRooms;
        private ArtworkCache iArtworkCache;
        private bool iUpdatedSinceOpened;
    }


    // View class to handle the radio playlist
    [ObjectiveCClass]
    public class ViewPlaylistRadio : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewPlaylistRadio));

        public ViewPlaylistRadio() : base() {}
        public ViewPlaylistRadio(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise the view to be hidden
            ViewRoot.IsHidden = true;

            // setup the artwork cache
            iArtworkCache = ArtworkCacheInstance.Instance;
            iArtworkCache.EventImageAdded += ArtworkCacheImageAdded;

            ViewTable.DoubleActionEvent += TableViewItemDoubleClicked;

            // set up model eventing
            iModel = ModelMain.Instance.ModelPlaylistRadio;
            iModel.EventOpen += ModelPlaylistRadioOpen;
            iModel.EventClose += ModelPlaylistRadioClose;
            iModel.EventInitialised += ModelPlaylistRadioInitialised;
            iModel.EventPresetsChanged += ModelPlaylistRadioPresetsChanged;
            iModel.EventPresetIndexChanged += ModelPlaylistRadioPresetIndexChanged;

            // setup delegate and datasource - if this is done in IB, the methods can be called
            // before awakeFromNib which causes complications
            ViewTable.Delegate = this;
            ViewTable.DataSource = this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventOpen -= ModelPlaylistRadioOpen;
            iModel.EventClose -= ModelPlaylistRadioClose;
            iModel.EventInitialised -= ModelPlaylistRadioInitialised;
            iModel.EventPresetsChanged -= ModelPlaylistRadioPresetsChanged;
            iModel.EventPresetIndexChanged -= ModelPlaylistRadioPresetIndexChanged;
            iArtworkCache.EventImageAdded -= ArtworkCacheImageAdded;
            ViewTable.DoubleActionEvent -= TableViewItemDoubleClicked;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region eventing from IModelPlaylistRadio
        private void ModelPlaylistRadioOpen(object sender, EventArgs e)
        {
            iUpdatedSinceOpened = false;

            ViewTable.SizeToFit();
        }

        private void ModelPlaylistRadioClose(object sender, EventArgs e)
        {
            // hide the view
            ViewRoot.IsHidden = true;
            ViewTable.ReloadData();
        }

        private void ModelPlaylistRadioInitialised(object sender, EventArgs e)
        {
            // show the view
            ViewRoot.IsHidden = false;
            ViewTable.ReloadData();
        }

        private void ModelPlaylistRadioPresetsChanged(object sender, EventArgs e)
        {
            // reload the table data
            ViewTable.ReloadData();

            // scroll to current preset on first update
            if (!iUpdatedSinceOpened)
            {
                ViewTable.ScrollRowToVisible(iModel.PresetIndex);
                iUpdatedSinceOpened = true;
            }
        }

        private void ModelPlaylistRadioPresetIndexChanged(object sender, EventArgs e)
        {
            ViewTable.ReloadData();
        }
        #endregion eventing from IModelPlaylistRadio


        #region NSTableView data source protocol methods
        [ObjectiveCMessage("numberOfRowsInTableView:")]
        public int TableViewNumberOfRows(NSTableView aTableView)
        {
            return iModel.Presets.Count;
        }

        [ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
        public Id TableViewObjectValue(NSTableView aTableView, NSTableColumn aTableColumn, int aRowIndex)
        {
            if (aRowIndex >= iModel.Presets.Count)
                return null;

            DataTrackItemRadio rowItem = new DataTrackItemRadio(aRowIndex, iModel.Presets[aRowIndex].DidlLite[0]);

            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(kKeyImage) == NSComparisonResult.NSOrderedSame)
            {
                if (aRowIndex == iModel.PresetIndex)
                {
                    return Properties.Resources.IconPlaying;
                }
                else
                {
                    NSImage artwork = Properties.Resources.IconRadio;

                    Uri artworkUri = DidlLiteAdapter.ArtworkUri(iModel.Presets[aRowIndex].DidlLite[0]);
                    if (artworkUri != null)
                    {
                        try
                        {
                            ArtworkCache.Item item = iArtworkCache.Artwork(artworkUri);
                            if (item != null && !item.Failed)
                            {
                                artwork = item.Image;
                            }
                        }
                        catch (Exception) {}
                    }

                    return artwork;
                }
            }
            else if (identifier.Compare(kKeyBitrate) == NSComparisonResult.NSOrderedSame)
            {
                return NSString.StringWithUTF8String(rowItem.Bitrate);
            }
            else if (identifier.Compare(kKeyText) == NSComparisonResult.NSOrderedSame)
            {
                return new WrappedDataTrackItem(rowItem);
            }

            return null;
        }
        #endregion NSTableView data source protocol methods


        #region NSTableView delegate protocol methods
        [ObjectiveCMessage("tableView:willDisplayCell:forTableColumn:row:")]
        public void TableViewWillDisplayCell(NSTableView aTableView, Id aCell, NSTableColumn aTableColumn, int aRowIndex)
        {
            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(kKeyImage) == NSComparisonResult.NSOrderedSame)
            {
                NSImageCell cell = aCell.CastAs<NSImageCell>();

                if (aRowIndex == iModel.PresetIndex)
                {
                    cell.ImageScaling = NSImageScaling.NSImageScaleNone;
                    cell.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
                }
                else
                {
                    cell.ImageScaling = NSImageScaling.NSImageScaleProportionallyUpOrDown;
                    cell.ImageAlignment = NSImageAlignment.NSImageAlignTop;
                }
            }

            if (identifier.Compare(kKeyBitrate) == NSComparisonResult.NSOrderedSame)
            {
                NSTextFieldCell textCell = aCell.CastAs<NSTextFieldCell>();

                textCell.TextColor = NSColor.WhiteColor;
                textCell.Font = FontManager.FontSmall;
            }
        }
        #endregion NSTableView delegate protocol methods


        public void TableViewItemDoubleClicked(Id aSender)
        {
            // double clicking is the same as the play context menu, although double clicking can be
            // performed on an area of the table view outside of the items, so check the clicked row index
            if (ViewTable.ClickedRow != -1)
            {
                Play(this);
            }
        }

        #region menu methods
        [ObjectiveCMessage("play:")]
        public void Play(Id aSender)
        {
            iModel.PresetIndex = ViewTable.ClickedRow;
        }

        [ObjectiveCMessage("save:")]
        public void Save(Id aSender)
        {
        }

        [ObjectiveCMessage("details:")]
        public void Details(Id aSender)
        {
        }
        #endregion menu methods


        private void ArtworkCacheImageAdded(object aSender, ArtworkCache.EventArgsArtwork e)
        {
            // this is called from the artwork cache thread
            ViewTable.BeginInvoke((MethodInvoker)delegate()
            {
                ViewTable.ReloadData();
            });
        }

        // Data item interface for use with the custom table cell
        private class DataTrackItemRadio : IDataTrackItem
        {
            public DataTrackItemRadio(int aIndex, upnpObject aUpnpObject)
            {
                iIndex = aIndex;
                iUpnpObject = aUpnpObject;
            }

            public bool IsGroup
            {
                get { return false; }
            }
            public int Index
            {
                get { return iIndex; }
            }
            public string Title
            {
                get { return DidlLiteAdapter.Title(iUpnpObject); }
            }
            public string Subtitle1
            {
                get { return string.Empty; }
            }
            public string Subtitle2
            {
                get { return string.Empty; }
            }
            public string Bitrate
            {
                get { return DidlLiteAdapter.Bitrate(iUpnpObject); }
            }
            public string TrackNumber
            {
                get 
                {
                    return (Index+1).ToString();
                }
            }
            private int iIndex;
            private upnpObject iUpnpObject;
        }

        [ObjectiveCField]
        public NSScrollView ViewRoot;

        [ObjectiveCField]
        public NSTableView ViewTable;

        private static NSString kKeyImage = new NSString("image");
        private static NSString kKeyText = new NSString("text");
        private static NSString kKeyBitrate = new NSString("bitrate");

        private IModelPlaylistRadio iModel;
        private ArtworkCache iArtworkCache;
        private bool iUpdatedSinceOpened;
    }


    // NSTableView derived class to simply reroute the draggedImage:endedAt:operation:
    // through the data source - this basically extends the NSTableView data source protocol
    [ObjectiveCClass]
    public class TableViewDragDrop : NSTableView
    {
        public TableViewDragDrop() : base() {}
        public TableViewDragDrop(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("draggedImage:endedAt:operation:")]
        public void DraggedImageEnded(NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {
            if (this.DataSource != null)
            {
                this.DataSource.SendMessage("tableView:draggedImage:endedAt:operation:", this, aImage, aPoint, aOperation);
            }
        }

        [ObjectiveCMessage("cancelOperation:")]
        public override void CancelOperation(Id aSender)
        {
            // this is captured when the ESC key is pressed - abort the editing
            if (CurrentEditor != null)
            {
                // putting this call in an "if" statement prevents compiler warnings
                if (AbortEditing)
                {
                }

                // make sure this view has the focus
                this.Window.MakeFirstResponder(this);
            }
        }
    }

    // View class to handle the ds playlist
    [ObjectiveCClass]
    public class ViewPlaylistDs : NSObject, IViewPlaylistDs
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewPlaylistDs));

        public ViewPlaylistDs() : base() {}
        public ViewPlaylistDs(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise the view to be hidden
            ViewRoot.IsHidden = true;

            iController = new ControllerPlaylistDs(this, ModelMain.Instance.ModelPlaylistDs);

            // setup drag and drop
            iDragSource = new DragSource(iController);

            iDragDestination = new DragDestination(iController);

            DragDestination.RegisterDragTypes(ViewTable);

            // within the application, playlist items can be copied, moved and deleted
            // they cannot be dragged outside the application
            ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationMove |
                                                             NSDragOperation.NSDragOperationCopy |
                                                             NSDragOperation.NSDragOperationDelete,
                                                             true);
            ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationNone, false);


            // setup the artwork cache
            iArtworkCache = ArtworkCacheInstance.Instance;
            iArtworkCache.EventImageAdded += ArtworkCacheImageAdded;

            ViewTable.DoubleActionEvent += TableViewItemDoubleClicked;

            // set up model eventing
            iModel = ModelMain.Instance.ModelPlaylistDs;
            iModel.EventOpen += ModelPlaylistDsOpen;
            iModel.EventClose += ModelPlaylistDsClose;
            iModel.EventInitialised += ModelPlaylistDsInitialised;
            iModel.EventPlaylistChanged += ModelPlaylistDsPlaylistChanged;
            iModel.EventTrackChanged += ModelPlaylistDsTrackChanged;

            iDataSource = new DataSourcePlaylistDs(iModel);

            ModelMain.Instance.Helper.PlaylistGrouping.EventValueChanged += PlaylistGroupingChanged;
            PlaylistGroupingChanged(this, EventArgs.Empty);

            // setup delegate and datasource - if this is done in IB, the methods can be called
            // before awakeFromNib which causes complications
            ViewTable.Delegate = this;
            ViewTable.DataSource = this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            ModelMain.Instance.Helper.PlaylistGrouping.EventValueChanged -= PlaylistGroupingChanged;
            iModel.EventOpen -= ModelPlaylistDsOpen;
            iModel.EventClose -= ModelPlaylistDsClose;
            iModel.EventInitialised -= ModelPlaylistDsInitialised;
            iModel.EventPlaylistChanged -= ModelPlaylistDsPlaylistChanged;
            iModel.EventTrackChanged -= ModelPlaylistDsTrackChanged;
            iArtworkCache.EventImageAdded -= ArtworkCacheImageAdded;
            ViewTable.DoubleActionEvent -= TableViewItemDoubleClicked;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region IModelPlaylisDs eventing
        private void ModelPlaylistDsOpen(object sender, EventArgs e)
        {
            iUpdatedSinceOpened = false;

            ViewTable.SizeToFit();
        }

        private void ModelPlaylistDsClose(object sender, EventArgs e)
        {
            // hide the view
            ViewRoot.IsHidden = true;

            // clear the data source - must reload the table data immediately after to ensure
            // the view and data source are in sync
            iDataSource.Clear();
            ViewTable.ReloadData();
        }

        private void ModelPlaylistDsInitialised(object sender, EventArgs e)
        {
            // refresh the data source - must reload the table data immediately after to ensure
            // the view and data source are in sync
            iDataSource.Refresh();
            ViewTable.ReloadData();

            // show the view
            ViewRoot.IsHidden = false;
        }

        private void ModelPlaylistDsPlaylistChanged(object sender, EventArgs e)
        {
            // refresh the data source - must reload the table data immediately after to ensure
            // the view and data source are in sync
            iDataSource.Refresh();
            ViewTable.ReloadData();

            // scroll to current track on first update
            if (!iUpdatedSinceOpened)
            {
                ViewTable.ScrollRowToVisible(iDataSource.PlayingIndex);
                iUpdatedSinceOpened = true;
            }
        }

        private void ModelPlaylistDsTrackChanged(object sender, EventArgs e)
        {
            ViewTable.ReloadData();
        }
        #endregion IModelPlaylisDs eventing


        #region NSTableView data source methods
        [ObjectiveCMessage("numberOfRowsInTableView:")]
        public int TableViewNumberOfRows(NSTableView aTableView)
        {
            return iDataSource.RowCount;
        }

        [ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
        public Id TableViewObjectValue(NSTableView aTableView, NSTableColumn aTableColumn, int aRowIndex)
        {
            DataSourcePlaylistDs.Item rowItem = iDataSource.RowItem(aRowIndex);

            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(kKeyImage) == NSComparisonResult.NSOrderedSame)
            {
                if (aRowIndex == iDataSource.PlayingIndex)
                {
                    return Properties.Resources.IconPlaying;
                }
                else
                {
                    return rowItem.Image;
                }
            }
            else if (identifier.Compare(kKeyDuration) == NSComparisonResult.NSOrderedSame)
            {
                return NSString.StringWithUTF8String(rowItem.Duration);
            }
            else if (identifier.Compare(kKeyText) == NSComparisonResult.NSOrderedSame)
            {
                return new WrappedDataTrackItem(rowItem);
            }

            return null;
        }

        [ObjectiveCMessage("tableView:writeRowsWithIndexes:toPasteboard:")]
        public bool TableViewWriteToPasteboard(NSTableView aTableView, NSIndexSet aRows, NSPasteboard aPasteboard)
        {
            // convert the passed in view indices in aRows into track indices in the model
            NSMutableIndexSet modelIndexSet = new NSMutableIndexSet();

            uint index = aRows.FirstIndex;
            while (index != FoundationFramework.NSNotFound)
            {
                DataSourcePlaylistDs.Item item = iDataSource.RowItem((int)index);

                if (item.IsGroup)
                {
                    // add all items in this group
                    DataSourcePlaylistDs.Group grp = item as DataSourcePlaylistDs.Group;

                    for (int i=grp.FirstItem ; i<=grp.LastItem ; i++)
                    {
                        if (!modelIndexSet.ContainsIndex((uint)i))
                        {
                            modelIndexSet.AddIndex((uint)i);
                        }
                    }
                }
                else
                {
                    if (!modelIndexSet.ContainsIndex((uint)item.Index))
                    {
                        modelIndexSet.AddIndex((uint)item.Index);
                    }
                }

                index = aRows.IndexGreaterThanIndex(index);
            }

            modelIndexSet.Autorelease();

            // now pass the model indices to the controller since these are the items that will be dragged
            return (iDragSource.Begin(modelIndexSet, ViewTable, aPasteboard) > 0);
        }

        [ObjectiveCMessage("tableView:draggedImage:endedAt:operation:")]
        public void TableViewDraggedImageEnded(NSTabView aTableView, NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {
            iDragSource.End(NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard), aOperation);
        }

        [ObjectiveCMessage("tableView:validateDrop:proposedRow:proposedDropOperation:")]
        public NSDragOperation TableViewValidateDrop(NSTableView aTableView, INSDraggingInfo aInfo, int aRow, NSTableViewDropOperation aOperation)
        {
            return iDragDestination.ValidateDrop(aInfo, ViewTable);
        }

        [ObjectiveCMessage("tableView:acceptDrop:row:dropOperation:")]
        public bool TableViewAcceptDrop(NSTableView aTableView, INSDraggingInfo aInfo, int aRow, NSTableViewDropOperation aOperation)
        {
            int dropIndex = -1;

            if (aRow < ViewTable.NumberOfRows)
            {
                // convert the view row index onto which the drop will occur into a model index
                DataSourcePlaylistDs.Item item = iDataSource.RowItem(aRow);

                // if the drop target row is a group item, use the first item in the group as drop target
                if (item.IsGroup)
                {
                    item = iDataSource.RowItem(aRow + 1);
                }

                dropIndex = item.Index;
            }
            else
            {
                // dropping at the end of the table
                dropIndex = iModel.Playlist.Count;
            }

            // pass the model drop index onto the controller
            return iDragDestination.AcceptDrop(aInfo, dropIndex, ViewTable);
        }
        #endregion NSTableView data source methods


        #region NSTableView delegate protocol methods
        [ObjectiveCMessage("tableView:willDisplayCell:forTableColumn:row:")]
        public void TableViewWillDisplayCell(NSTableView aTableView, Id aCell, NSTableColumn aTableColumn, int aRowIndex)
        {
            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(kKeyImage) == NSComparisonResult.NSOrderedSame)
            {
                NSImageCell cell = aCell.CastAs<NSImageCell>();

                if (aRowIndex == iDataSource.PlayingIndex)
                {
                    cell.ImageScaling = NSImageScaling.NSImageScaleNone;
                    cell.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
                }
                else
                {
                    cell.ImageScaling = NSImageScaling.NSImageScaleProportionallyUpOrDown;
                    cell.ImageAlignment = NSImageAlignment.NSImageAlignTop;
                }
            }

            if (identifier.Compare(kKeyDuration) == NSComparisonResult.NSOrderedSame)
            {
                NSTextFieldCell textCell = aCell.CastAs<NSTextFieldCell>();

                textCell.TextColor = NSColor.WhiteColor;
                textCell.Font = FontManager.FontSmall;
            }
        }

        [ObjectiveCMessage("tableView:heightOfRow:")]
        public float TableViewHeightOfRow(NSTableView aTableView, int aRowIndex)
        {
            return iDataSource.RowItem(aRowIndex).RowHeight;
        }
        #endregion NSTableView delegate protocol methods


        public void TableViewItemDoubleClicked(Id aSender)
        {
            // double clicking is the same as the play context menu, although double clicking can be
            // performed on an area of the table view outside of the items, so check the clicked row index
            if (ViewTable.ClickedRow != -1)
            {
                // ignore double clicks on group items
                DataSourcePlaylistDs.Item item = iDataSource.RowItem(ViewTable.ClickedRow);
                if (!item.IsGroup)
                {
                    iController.MenuItemPlay();
                }
            }
        }

        #region Context menu methods
        [ObjectiveCMessage("play:")]
        public void Play(Id aSender)
        {
            iController.MenuItemPlay();
        }

        [ObjectiveCMessage("moveUp:")]
        public void MoveUp(Id aSender)
        {
            iController.MenuItemMoveUp();
        }

        [ObjectiveCMessage("moveDown:")]
        public void MoveDown(Id aSender)
        {
            iController.MenuItemMoveDown();
        }

        [ObjectiveCMessage("save:")]
        public void Save(Id aSender)
        {
            iController.MenuItemSave();
        }

        [ObjectiveCMessage("delete:")]
        public void Delete(Id aSender)
        {
            iController.MenuItemDelete();
        }

        [ObjectiveCMessage("details:")]
        public void Details(Id aSender)
        {
            iController.MenuItemDetails();
        }

        [ObjectiveCMessage("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem aItem)
        {
            if (aItem.Action == ObjectiveCRuntime.Selector("play:"))
            {
                return iController.ValidateMenuItemPlay();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("moveUp:"))
            {
                return iController.ValidateMenuItemMoveUp();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("moveDown:"))
            {
                return iController.ValidateMenuItemMoveDown();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("save:"))
            {
                return iController.ValidateMenuItemSave();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("delete:"))
            {
                return iController.ValidateMenuItemDelete();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("details:"))
            {
                return iController.ValidateMenuItemDetails();
            }
            return false;
        }
        #endregion Context menu methods


        #region IViewPlaylistDs implementation
        public int ClickedItem
        {
            get
            {
                if (ViewTable.ClickedRow != -1)
                {
                    // a view row was clicked
                    DataSourcePlaylistDs.Item item = iDataSource.RowItem(ViewTable.ClickedRow);

                    // return the index of the clicked item, or -1 if a group was clicked
                    return item.IsGroup ? -1 : item.Index;
                }
                else
                {
                    // no view row is clicked
                    return -1;
                }
            }
        }

        public int ClickedGroup
        {
            get
            {
                if (ViewTable.ClickedRow != -1)
                {
                    // a view row was clicked
                    DataSourcePlaylistDs.Item item = iDataSource.RowItem(ViewTable.ClickedRow);

                    // return the index of the clicked group, or -1 if an item was clicked
                    return item.IsGroup ? item.Index : -1;
                }
                else
                {
                    // no view row is clicked
                    return -1;
                }
            }
        }

        public bool SelectionContainsItem(int aIndex)
        {
            // convert the passed in model track index into a view index
            int viewIndex = iDataSource.TrackToViewIndex(aIndex);
            return (viewIndex != -1) ? ViewTable.IsRowSelected(viewIndex) : false;
        }

        public bool SelectionContainsGroup(int aIndex)
        {
            // convert the passed in model group index into a view index
            int viewIndex = iDataSource.GroupToViewIndex(aIndex);
            return (viewIndex != -1) ? ViewTable.IsRowSelected(viewIndex) : false;
        }

        public int SelectedItemCount
        {
            get { return SelectedItems.Count; }
        }

        public IList<int> SelectedItems
        {
            get
            {
                List<int> selectedItems = new List<int>();

                NSIndexSet selectedIndexSet = ViewTable.SelectedRowIndexes;

                uint index = selectedIndexSet.FirstIndex;
                while (index != FoundationFramework.NSNotFound)
                {
                    DataSourcePlaylistDs.Item item = iDataSource.RowItem((int)index);

                    if (item.IsGroup)
                    {
                        // add all items in the selected group
                        DataSourcePlaylistDs.Group grp = item as DataSourcePlaylistDs.Group;

                        for (int i=grp.FirstItem ; i<=grp.LastItem ; i++)
                        {
                            if (!selectedItems.Contains(i))
                            {
                                selectedItems.Add(i);
                            }
                        }
                    }
                    else
                    {
                        // add this item
                        if (!selectedItems.Contains(item.Index))
                        {
                            selectedItems.Add(item.Index);
                        }
                    }

                    index = selectedIndexSet.IndexGreaterThanIndex(index);
                }

                return selectedItems.AsReadOnly();
            }
        }
        #endregion IViewPlaylistDs implementation


        private void ArtworkCacheImageAdded(object sender, ArtworkCache.EventArgsArtwork e)
        {
            // this is called from the artwork cache thread
            ViewTable.BeginInvoke((MethodInvoker)delegate()
            {
                ViewTable.ReloadData();
            });
        }

        private void PlaylistGroupingChanged(object sender, EventArgs e)
        {
            iDataSource.GroupItems = ModelMain.Instance.Helper.PlaylistGrouping.Native;
            if (!ViewRoot.IsHidden)
            {
                iDataSource.Refresh();
                ViewTable.ReloadData();
            }
        }

        [ObjectiveCField]
        public NSScrollView ViewRoot;

        [ObjectiveCField]
        public NSTableView ViewTable;

        private static NSString kKeyImage = new NSString("image");
        private static NSString kKeyText = new NSString("text");
        private static NSString kKeyDuration = new NSString("duration");

        private IModelPlaylistDs iModel;
        private ArtworkCache iArtworkCache;
        private DataSourcePlaylistDs iDataSource;
        private ControllerPlaylistDs iController;
        private DragSource iDragSource;
        private DragDestination iDragDestination;
        private bool iUpdatedSinceOpened;
    }


    // Data source class for the ds playlist
    public class DataSourcePlaylistDs
    {
        // abstract base class for all items in the ds playlist view
        public abstract class Item : IDataTrackItem
        {
            public abstract NSImage Image { get; }
            public abstract string Duration { get; }
            public abstract float RowHeight { get; }

            #region IDataTrackItem interface
            public abstract string Title { get; }
            public abstract string Subtitle1 { get; }
            public abstract string Subtitle2 { get; }
            public abstract bool IsGroup { get; }
            public int Index
            {
                get { return iIndex; }
            }
            public abstract string TrackNumber { get; }
            #endregion IDataTrackItem interface

            protected Item(upnpObject aUpnpObject, int aIndex)
            {
                iUpnpObject = aUpnpObject;
                iIndex = aIndex;
            }

            protected NSImage Artwork
            {
                get
                {
                    NSImage artwork = Properties.Resources.IconTrack;

                    Uri artworkUri = DidlLiteAdapter.ArtworkUri(iUpnpObject);
                    if (artworkUri != null)
                    {
                        try
                        {
                            ArtworkCache.Item item = ArtworkCacheInstance.Instance.Artwork(artworkUri);
                            if (item != null && !item.Failed)
                            {
                                artwork = item.Image;
                            }
                        }
                        catch (Exception) {}
                    }

                    return artwork;
                }
            }

            protected upnpObject iUpnpObject;
            private int iIndex;
        }

        // class for group items in the ds playlist view
        public class Group : Item
        {
            public Group(upnpObject aUpnpObject, int aFirstItem, int aLastItem, int aGroupIndex)
                : base(aUpnpObject, aGroupIndex)
            {
                FirstItem = aFirstItem;
                LastItem = aLastItem;
            }

            public override NSImage Image
            {
                get { return Artwork; }
            }
            public override string Duration
            {
                get { return string.Empty; }
            }
            public override string Title
            {
                get { return DidlLiteAdapter.Album(iUpnpObject); }
            }
            public override string TrackNumber 
            {
                get 
                {
                    return string.Empty;
                }
            }
            public override string Subtitle1
            {
                get
                {
                    string albumArtist = DidlLiteAdapter.AlbumArtist(iUpnpObject);
                    return (string.IsNullOrEmpty(albumArtist)) ? DidlLiteAdapter.Artist(iUpnpObject) : albumArtist;
                }
            }
            public override string Subtitle2
            {
                get { return string.Empty; }
            }
            public override bool IsGroup
            {
                get { return true; }
            }
            public override float RowHeight
            {
                get { return 70.0f; }
            }

            public readonly int FirstItem;
            public readonly int LastItem;
        }

        // class for a grouped track item in the ds playlist view
        public class TrackGrouped : Item
        {
            public TrackGrouped(upnpObject aUpnpObject, int aIndex)
                : base(aUpnpObject, aIndex)
            {
            }

            public override NSImage Image
            {
                get { return null; }
            }
            public override string Duration
            {
                get { return DidlLiteAdapter.Duration(iUpnpObject); }
            }
            public override string Title
            {
                get { return DidlLiteAdapter.Title(iUpnpObject); }
            }
            public override string TrackNumber 
            {
                get 
                {
                    return (Index+1).ToString();
                }
            }
            public override string Subtitle1
            {
                get
                {
                    string albumArtist = DidlLiteAdapter.AlbumArtist(iUpnpObject);
                    string artist = DidlLiteAdapter.Artist(iUpnpObject);
                    if (!string.IsNullOrEmpty(albumArtist) && albumArtist != artist)
                    {
                        return artist;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            public override string Subtitle2
            {
                get { return string.Empty; }
            }
            public override bool IsGroup
            {
                get { return false; }
            }
            public override float RowHeight
            {
                get { return 40.0f; }
            }
        }

        // class for an ungrouped track item in the ds playlist view
        public class TrackUngrouped : Item
        {
            public TrackUngrouped(upnpObject aUpnpObject, int aIndex)
                : base(aUpnpObject, aIndex)
            {
            }

            public override NSImage Image
            {
                get { return Artwork; }
            }
            public override string Duration
            {
                get { return DidlLiteAdapter.Duration(iUpnpObject); }
            }
            public override string Title
            {
                get { return DidlLiteAdapter.Title(iUpnpObject); }
            }
            public override string TrackNumber 
            {
                get 
                {
                    return (Index+1).ToString();
                }
            }
            public override string Subtitle1
            {
                get { return DidlLiteAdapter.Album(iUpnpObject); }
            }
            public override string Subtitle2
            {
                get { return DidlLiteAdapter.Artist(iUpnpObject); }
            }
            public override bool IsGroup
            {
                get { return false; }
            }
            public override float RowHeight
            {
                get { return 70.0f; }
            }
        }

        public DataSourcePlaylistDs(IModelPlaylistDs aModel)
        {
            iModel = aModel;
        }

        public bool GroupItems
        {
            set { iGroupedItems = value; }
        }

        public void Clear()
        {
            iItems.Clear();
            iTrackToViewIndex.Clear();
            iGroupToViewIndex.Clear();
        }

        public void Refresh()
        {
            iItems.Clear();
            iTrackToViewIndex.Clear();
            iGroupToViewIndex.Clear();

            if (iGroupedItems)
            {
                // build the list of group and track items
                for (int groupIndex=0 ; groupIndex<iModel.Groups.Count ; groupIndex++)
                {
                    ModelListGroup g = iModel.Groups[groupIndex];

                    if (g.Count > 1)
                    {
                        // a group containing > 1 item - add the group item
                        iItems.Add(new Group(iModel.Playlist[g.FirstIndex].DidlLite[0], g.FirstIndex, g.LastIndex, groupIndex));
                        iGroupToViewIndex[groupIndex] = iItems.Count - 1;

                        // then add all track items in the group
                        for (int i=g.FirstIndex ; i<=g.LastIndex ; i++)
                        {
                            iItems.Add(new TrackGrouped(iModel.Playlist[i].DidlLite[0], i));
                            iTrackToViewIndex[i] = iItems.Count - 1;
                        }

                    }
                    else
                    {
                        // a group containing 1 item
                        iItems.Add(new TrackUngrouped(iModel.Playlist[g.FirstIndex].DidlLite[0], g.FirstIndex));
                        iTrackToViewIndex[g.FirstIndex] = iItems.Count - 1;
                    }
                }
            }
            else
            {
                for (int i=0 ; i<iModel.Playlist.Count ; i++)
                {
                    iItems.Add(new TrackUngrouped(iModel.Playlist[i].DidlLite[0], i));
                    iTrackToViewIndex[i] = i;
                }
            }
        }

        public int RowCount
        {
            get { return iItems.Count; }
        }

        public Item RowItem(int aIndex)
        {
            return iItems[aIndex];
        }

        public int PlayingIndex
        {
            get { return TrackToViewIndex(iModel.TrackIndex); }
        }

        public int TrackToViewIndex(int aTrackIndex)
        {
            return (aTrackIndex != -1) ? iTrackToViewIndex[aTrackIndex] : -1;
        }

        public int GroupToViewIndex(int aGroupIndex)
        {
            return (aGroupIndex != -1) ? iGroupToViewIndex[aGroupIndex] : -1;
        }

        private IModelPlaylistDs iModel;
        private List<Item> iItems = new List<Item>();
        private Dictionary<int, int> iTrackToViewIndex = new Dictionary<int, int>();
        private Dictionary<int, int> iGroupToViewIndex = new Dictionary<int, int>();
        private bool iGroupedItems = true;
    }


    // View class to handle the current track
    [ObjectiveCClass]
    public class ViewTrack : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewTrack));

        public ViewTrack() : base() {}
        public ViewTrack(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            iUriConverter = new ScalingUriConverter(kDesiredImageSize, true, true);
            ViewTrackInfoText.FontTitle = FontManager.FontLarge;
            ViewTrackInfoText.FontSubtitle = FontManager.FontSemiLarge;
            ViewTrackInfoText.FontTechInfo = FontManager.FontSmall;

            // setup model eventing
            iModel = ModelMain.Instance.ModelTrack;
            iModel.EventTrackChanged += ModelTrackChanged;

            iShowExtendedInfo = ModelMain.Instance.Helper.ShowTechnicalInfo;
            iShowExtendedInfo.EventValueChanged += ModelTrackChanged;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventTrackChanged -= ModelTrackChanged;
            iShowExtendedInfo.EventValueChanged -= ModelTrackChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        private void ModelTrackChanged(object sender, EventArgs e)
        {
            NSAutoreleasePool pool = new NSAutoreleasePool();
            pool.Init();

            Track track = iModel.Current;

            // set title
            if (string.IsNullOrEmpty(track.Title))
            {
                ViewTrackInfoText.Title = string.Empty;
                WindowNowPlaying.ViewTrackInfoText.Title = "Now Playing";
            }
            else
            {
                ViewTrackInfoText.Title = track.Title;
                WindowNowPlaying.ViewTrackInfoText.Title = track.Title;
            }

            // set album-artist
            ViewTrackInfoText.Artist = track.Artist;
            ViewTrackInfoText.Album = track.Album;
            WindowNowPlaying.ViewTrackInfoText.Artist = track.Artist;
            WindowNowPlaying.ViewTrackInfoText.Album = track.Album;

            // audio format data
            if (track.Codec != string.Empty && iShowExtendedInfo.Native)
            {
                string techInfo = string.Empty;
                if (track.Lossless)
                {
                    techInfo = string.Format("{0}   {1} kHz / {2} bits   {3} kbps", track.Codec, track.SampleRate, track.BitDepth, track.Bitrate);
                }
                else
                {
                    techInfo = string.Format("{0}   {1} kHz   {2} kbps", track.Codec, track.SampleRate, track.Bitrate);
                }
                ViewTrackInfoText.TechInfo = techInfo;
                WindowNowPlaying.ViewTrackInfoText.TechInfo = techInfo;
            }
            else
            {
                ViewTrackInfoText.TechInfo = string.Empty;
                WindowNowPlaying.ViewTrackInfoText.TechInfo = string.Empty;
            }

            pool.Release();


            // update the artwork
            if (track.IsEmpty)
            {
                // the current track is empty - display nothing
                iArtworkUri = string.Empty;
                ImageViewArtwork.Image = null;
                WindowNowPlaying.ImageArtwork.Image = null;
            }
            else if (track.ArtworkUri == string.Empty)
            {
                // no album art for this track - set the image and uri for the display
                iArtworkUri = string.Empty;
                ImageViewArtwork.Image = Properties.Resources.IconLoading;
                WindowNowPlaying.ImageArtwork.Image = Properties.Resources.IconLoading;
            }
            else if (iUriConverter.Convert(track.ArtworkUri).CompareTo(iArtworkUri) != 0)
            {
                // artwork has changed - clear the displayed image and download the artwork
                iArtworkUri = iUriConverter.Convert(track.ArtworkUri);
                ImageViewArtwork.Image = Properties.Resources.IconLoading;
                WindowNowPlaying.ImageArtwork.Image = Properties.Resources.IconLoading;

                NSURL artworkUri = NSURL.URLWithString(NSString.StringWithUTF8String(iArtworkUri));

                this.PerformSelectorInBackgroundWithObject(ObjectiveCRuntime.Selector("downloadArtwork:"), artworkUri);
            }
        }

        [ObjectiveCMessage("downloadArtwork:")]
        public void DownloadArtwork(NSURL aArtworkUri)
        {
            NSAutoreleasePool pool = new NSAutoreleasePool();
            pool.Init();

            // download the artwork
            NSImage artwork = null;
            try
            {
                artwork = new NSImage(aArtworkUri);
            }
            catch (Exception)
            {
                artwork = Properties.Resources.IconLoading;
                artwork.Retain();
            }

            // build up the array for results of download
            NSArray results = NSArray.ArrayWithObjects(artwork, aArtworkUri, null);
            artwork.Release();

            // send results back to main thread
            this.PerformSelectorOnMainThreadWithObjectWaitUntilDone(ObjectiveCRuntime.Selector("downloadArtworkFinished:"), results, false);

            pool.Release();
        }

        [ObjectiveCMessage("downloadArtworkFinished:")]
        public void DownloadArtworkFinished(NSArray aResults)
        {
            NSImage artwork = aResults[0].CastAs<NSImage>();
            NSURL artworkUri = aResults[1].CastAs<NSURL>();

            // update the image view if this downloaded artwork if for the current track
            if (artworkUri.AbsoluteString.ToString() == iArtworkUri)
            {
                ImageViewArtwork.Image = artwork;
                WindowNowPlaying.ImageArtwork.Image = artwork;
            }
        }

        [ObjectiveCField]
        public ImageViewClickable ImageViewArtwork;

        [ObjectiveCField]
        public ViewTrackInfo ViewTrackInfoText;

        [ObjectiveCField]
        public WindowNowPlayingController WindowNowPlaying;

        private IModelTrack iModel;
        private string iArtworkUri;
        private OptionBool iShowExtendedInfo;
        private ScalingUriConverter iUriConverter;
        private const int kDesiredImageSize = 2048;
    }


    // View for rendering of the track info text
    [ObjectiveCClass]
    public class ViewTrackInfo : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewTrackInfo));

        public ViewTrackInfo() : base() {}
        public ViewTrackInfo(IntPtr aInstance) : base(aInstance) {}

        public string Title
        {
            set
            {
                iTitle = value;
                NeedsDisplay = true;
            }
        }

        public string Artist
        {
            set
            {
                iArtist = value;
                NeedsDisplay = true;
            }
        }

        public string Album
        {
            set
            {
                iAlbum = value;
                NeedsDisplay = true;
            }
        }

        public string TechInfo
        {
            set
            {
                iTechInfo = value;
                NeedsDisplay = true;
            }
        }

        public NSFont FontTitle
        {
            set
            {
                if (iFontTitle != null)
                {
                    iFontTitle.Release();
                }
                iFontTitle = value;
                iFontTitle.Retain();
            }
        }

        public NSFont FontSubtitle
        {
            set
            {
                if (iFontSubtitle != null)
                {
                    iFontSubtitle.Release();
                }
                iFontSubtitle = value;
                iFontSubtitle.Retain();
            }
        }

        public NSFont FontTechInfo
        {
            set
            {
                if (iFontTechInfo != null)
                {
                    iFontTechInfo.Release();
                }
                iFontTechInfo = value;
                iFontTechInfo.Retain();
            }
        }

        public NSTextAlignment Alignment
        {
            set { iAlignment = value; }
        }

        public bool TopAlign
        {
            set { iTopAlign = value; }
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            // create attributes for the title and subtitle text
            NSMutableParagraphStyle style = new NSMutableParagraphStyle();
            style.SetParagraphStyle(NSParagraphStyle.DefaultParagraphStyle);
            style.SetLineBreakMode(NSLineBreakMode.NSLineBreakByTruncatingTail);
            style.SetAlignment(iAlignment);

            NSDictionary titleAttr = NSDictionary.DictionaryWithObjectsAndKeys(iFontTitle, NSAttributedString.NSFontAttributeName,
                                                                               NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                               style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                               null);
            NSDictionary subtitleAttr = NSDictionary.DictionaryWithObjectsAndKeys(iFontSubtitle, NSAttributedString.NSFontAttributeName,
                                                                                  NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                                  style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                                  null);
            NSDictionary techInfoAttr = NSDictionary.DictionaryWithObjectsAndKeys(iFontTechInfo, NSAttributedString.NSFontAttributeName,
                                                                                  NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                                  style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                                  null);
            style.Release();

            // for this text drawing, origin is in bottom left corner and +ve y is up
            float left = Bounds.MinX;
            float vertPos = Bounds.MaxY;

            if (!iTopAlign)
            {
                vertPos = NextVerticalPos(iTitle, titleAttr, left, vertPos);
                vertPos = NextVerticalPos(iAlbum, subtitleAttr, left, vertPos);
                vertPos = NextVerticalPos(iArtist, subtitleAttr, left, vertPos);
                vertPos -= 5;
                vertPos = NextVerticalPos(iTechInfo, techInfoAttr, left, vertPos);

                float totalHeight = Bounds.MaxY - vertPos;
                vertPos = Bounds.MaxY - (Bounds.Height - totalHeight)*0.5f;
            }

            vertPos = DrawText(iTitle, titleAttr, left, vertPos);
            vertPos = DrawText(iAlbum, subtitleAttr, left, vertPos);
            vertPos = DrawText(iArtist, subtitleAttr, left, vertPos);
            vertPos -= 5;
            vertPos = DrawText(iTechInfo, techInfoAttr, left, vertPos);
        }

        private float DrawText(string aText, NSDictionary aAttr, float aLeft, float aTop)
        {
            if (!string.IsNullOrEmpty(aText))
            {
                NSString text = NSString.StringWithUTF8String(aText);

                NSSize size = text.SizeWithAttributes(aAttr);
                NSRect rect = new NSRect(aLeft, aTop - size.height, Bounds.Width, size.height);
                text.DrawInRectWithAttributes(rect, aAttr);

                return aTop - size.height;
            }

            return aTop;
        }

        private float NextVerticalPos(string aText, NSDictionary aAttr, float aLeft, float aTop)
        {
            if (!string.IsNullOrEmpty(aText))
            {
                NSString text = NSString.StringWithUTF8String(aText);
                NSSize size = text.SizeWithAttributes(aAttr);
                return aTop - size.height;
            }

            return aTop;
        }

        private string iTitle = string.Empty;
        private string iArtist = string.Empty;
        private string iAlbum = string.Empty;
        private string iTechInfo = string.Empty;
        private NSFont iFontTitle = null;
        private NSFont iFontSubtitle = null;
        private NSFont iFontTechInfo = null;
        private NSTextAlignment iAlignment = NSTextAlignment.NSLeftTextAlignment;
        private bool iTopAlign = true;
    }


    // View class to handle the transport buttons
    [ObjectiveCClass]
    public class ViewTransport : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewTransport));

        public ViewTransport() : base() {}
        public ViewTransport(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise appearance
            PanelBackground.Image = Properties.Resources.ImageArray;
            PanelBackground.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            PanelBackground.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            PanelBackground.ImageScaling = NSImageScaling.NSImageScaleNone;

            ImageTramlines.Image = Properties.Resources.ImageTramLines;
            ImageTramlines.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            ImageTramlines.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            ImageTramlines.ImageScaling = NSImageScaling.NSImageScaleNone;

            ButtonPlayPause.Initialise(Properties.Resources.ImagePlay,
                                       Properties.Resources.ImagePlayOver,
                                       Properties.Resources.ImagePlayDown,
                                       Properties.Resources.ImagePause,
                                       Properties.Resources.ImagePauseOver,
                                       Properties.Resources.ImagePauseDown);
            ButtonPlayStop.Initialise(Properties.Resources.ImagePlay,
                                      Properties.Resources.ImagePlayOver,
                                      Properties.Resources.ImagePlayDown,
                                      Properties.Resources.ImageStop,
                                      Properties.Resources.ImageStopOver,
                                      Properties.Resources.ImageStopDown);
            ButtonPrevious.Initialise(Properties.Resources.ImageSkipBack,
                                      Properties.Resources.ImageSkipBackOver,
                                      Properties.Resources.ImageSkipBackDown);
            ButtonNext.Initialise(Properties.Resources.ImageSkipForward,
                                  Properties.Resources.ImageSkipForwardOver,
                                  Properties.Resources.ImageSkipForwardDown);

            ButtonNext.IsHidden = true;
            ButtonPrevious.IsHidden = true;
            ButtonPlayPause.IsHidden = true;
            ButtonPlayStop.IsHidden = true;

            // setup model eventing
            iModel = ModelMain.Instance.ModelTransport;
            iModel.EventClose += ModelTransportClose;
            iModel.EventInitialised += ModelTransportInitialised;
            iModel.EventChanged += ModelTransportChanged;

            iController = new ControllerTransport(iModel);

            ModelMain.Instance.ModelTooltips.EventChanged += ShowTooltipsChanged;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventClose -= ModelTransportClose;
            iModel.EventInitialised -= ModelTransportInitialised;
            iModel.EventChanged -= ModelTransportChanged;
            ModelMain.Instance.ModelTooltips.EventChanged -= ShowTooltipsChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        private void ModelTransportInitialised(object sender, EventArgs e)
        {
            ButtonPrevious.EventClicked += ButtonPreviousClicked;
            ButtonPlayPause.EventClicked += ButtonPlayPauseClicked;
            ButtonPlayStop.EventClicked += ButtonPlayStopClicked;
            ButtonNext.EventClicked += ButtonNextClicked;

            ModelTransportChanged(sender, e);
        }

        private void ModelTransportClose(object sender, EventArgs e)
        {
            ButtonPrevious.EventClicked -= ButtonPreviousClicked;
            ButtonPlayPause.EventClicked -= ButtonPlayPauseClicked;
            ButtonPlayStop.EventClicked -= ButtonPlayStopClicked;
            ButtonNext.EventClicked -= ButtonNextClicked;

            ModelTransportChanged(sender, e);
        }

        private void ModelTransportChanged(object sender, EventArgs e)
        {
            // set visibility of various buttons
            if (iModel.IsInitialised)
            {
                ButtonPrevious.IsHidden = false;
                ButtonNext.IsHidden = false;
                ButtonPrevious.Enabled = iModel.AllowSkipping;
                ButtonNext.Enabled = iModel.AllowSkipping;

                ButtonPlayPause.IsHidden = !iModel.PauseNotStop;
                ButtonPlayStop.IsHidden = iModel.PauseNotStop;
            }
            else
            {
                ButtonPrevious.IsHidden = true;
                ButtonNext.IsHidden = true;
                ButtonPlayPause.IsHidden = true;
                ButtonPlayStop.IsHidden = true;
            }

            // update the button states
            switch (iModel.TransportState)
            {
            case ETransportState.ePlaying:
                ButtonPlayPause.IsOn = true;
                ButtonPlayStop.IsOn = true;
                break;

            case ETransportState.ePaused:
                ButtonPlayPause.IsOn = false;
                break;

            case ETransportState.eStopped:
                ButtonPlayStop.IsOn = false;
                break;
            }
        }

        private void ButtonPreviousClicked(Id aSender)
        {
            iController.ButtonPreviousClicked();
        }

        private void ButtonPlayPauseClicked(Id aSender)
        {
            iController.ButtonPlayPauseClicked();
        }

        private void ButtonPlayStopClicked(Id aSender)
        {
            iController.ButtonPlayStopClicked();
        }

        private void ButtonNextClicked(Id aSender)
        {
            iController.ButtonNextClicked();
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            if (e.Show)
            {
                ButtonPlayPause.ToolTip = NSString.StringWithUTF8String("Playback controls");
                ButtonPlayStop.ToolTip = NSString.StringWithUTF8String("Playback controls");
                ButtonNext.ToolTip = NSString.StringWithUTF8String("Playback controls");
                ButtonPrevious.ToolTip = NSString.StringWithUTF8String("Playback controls");
            }
            else
            {
                ButtonPlayPause.ToolTip = null;
                ButtonPlayStop.ToolTip = null;
                ButtonNext.ToolTip = null;
                ButtonPrevious.ToolTip = null;
            }
        }

        [ObjectiveCField]
        public NSImageView PanelBackground;

        [ObjectiveCField]
        public NSImageView ImageTramlines;

        [ObjectiveCField]
        public ButtonHoverPush ButtonPlayPause;

        [ObjectiveCField]
        public ButtonHoverPush ButtonPlayStop;

        [ObjectiveCField]
        public ButtonHoverPush ButtonNext;

        [ObjectiveCField]
        public ButtonHoverPush ButtonPrevious;


        private IModelTransport iModel;
        private ControllerTransport iController;
    }


    // View class to handle the volume control
    [ObjectiveCClass]
    public class ViewVolumeControl : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewVolumeControl));

        public ViewVolumeControl() : base() {}
        public ViewVolumeControl(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            iOpen = false;

            Rotary.ViewBar.PreviewEnabled = false;
            Rocker.ViewBar.PreviewEnabled = false;

            iModel = ModelMain.Instance.ModelVolumeControl;
            iModel.EventOpen += ModelVolumeControlOpen;
            iModel.EventInitialised += ModelVolumeControlInitialised;
            iModel.EventClose += ModelVolumeControlClose;
            iModel.EventChanged += ModelVolumeControlChanged;

            ModelMain.Instance.ModelTooltips.EventChanged += ShowTooltipsChanged;

            iEnableRocker = ModelMain.Instance.Helper.EnableRocker;
            iEnableRocker.EventValueChanged += EnableRockerChanged;
            EnableRockerChanged(this, EventArgs.Empty);
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventOpen -= ModelVolumeControlOpen;
            iModel.EventInitialised -= ModelVolumeControlInitialised;
            iModel.EventClose -= ModelVolumeControlClose;
            iModel.EventChanged -= ModelVolumeControlChanged;

            ModelMain.Instance.ModelTooltips.EventChanged -= ShowTooltipsChanged;

            iEnableRocker.EventValueChanged -= EnableRockerChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        private void ModelVolumeControlOpen(object sender, EventArgs e)
        {
            // open should not be called more than once
            iCurrent.EventAnticlockwiseStep += RotaryAnticlockwiseStep;
            iCurrent.EventClockwiseStep += RotaryClockwiseStep;
            iCurrent.EventClicked += RotaryClicked;

            Assert.Check(!iOpen);
            iOpen = true;
        }

        private void ModelVolumeControlInitialised(object sender, EventArgs e)
        {
            // initialised can be called many times
            iCurrent.ViewBar.Value = iModel.Volume;
            iCurrent.ViewBar.MaxValue = iModel.VolumeLimit;
            iCurrent.ViewBar.Text = iModel.Volume.ToString();
            iCurrent.Dimmed = iModel.Mute;
            iCurrent.Enabled = true;
        }

        private void ModelVolumeControlClose(object sender, EventArgs e)
        {
            // volume control close can be called without having been opened
            if (iOpen)
            {
                iCurrent.EventAnticlockwiseStep -= RotaryAnticlockwiseStep;
                iCurrent.EventClockwiseStep -= RotaryClockwiseStep;
                iCurrent.EventClicked -= RotaryClicked;

                iCurrent.ViewBar.Value = 0;
                iCurrent.ViewBar.MaxValue = 0;
                iCurrent.ViewBar.Text = string.Empty;
                iCurrent.Dimmed = false;
                iCurrent.Enabled = false;

                iOpen = false;
            }
        }

        private void ModelVolumeControlChanged(object sender, EventArgs e)
        {
            iCurrent.ViewBar.Value = iModel.Volume;
            iCurrent.ViewBar.MaxValue = iModel.VolumeLimit;
            iCurrent.ViewBar.Text = iModel.Volume.ToString();
            iCurrent.Dimmed = iModel.Mute;
        }

        private void RotaryClockwiseStep(object sender, EventArgs e)
        {
            iModel.IncrementVolume();
        }

        private void RotaryAnticlockwiseStep(object sender, EventArgs e)
        {
            iModel.DecrementVolume();
        }

        private void RotaryClicked(object sender, EventArgs e)
        {
            iModel.ToggleMute();
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            if (e.Show)
            {
                Rocker.ToolTip = NSString.StringWithUTF8String("Volume control (click center to toggle mute, click left/right to adjust volume)");
                Rotary.ToolTip = NSString.StringWithUTF8String("Volume control (click center to toggle mute, rotate ring to adjust volume)");
            }
            else
            {
                Rocker.ToolTip = null;
                Rotary.ToolTip = null;
            }
        }

        private void EnableRockerChanged(object sender, EventArgs e)
        {
            // close the control if it is open in order to detach the current controller
            bool open = iOpen;
            if (open)
            {
                ModelVolumeControlClose(this, EventArgs.Empty);
            }

            if (iEnableRocker.Native)
            {
                Rotary.IsHidden = true;
                Rocker.IsHidden = false;
                iCurrent = Rocker;
            }
            else
            {
                Rocker.IsHidden = true;
                Rotary.IsHidden = false;
                iCurrent = Rotary;
            }

            // reopen the control to attach the new controller
            if (open)
            {
                ModelVolumeControlOpen(this, EventArgs.Empty);
                ModelVolumeControlInitialised(this, EventArgs.Empty);
            }
        }

        [ObjectiveCField]
        public ViewRocker Rocker;

        [ObjectiveCField]
        public ViewRotary Rotary;

        private IModelVolumeControl iModel;
        private OptionBool iEnableRocker;
        private IViewRotary iCurrent;
        private bool iOpen;
    }


    // View class to handle the media time
    [ObjectiveCClass]
    public class ViewMediaTime : NSObject, IViewMediaTime
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewMediaTime));

        public ViewMediaTime() : base() {}
        public ViewMediaTime(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            iOpen = false;

            Rotary.Enabled = false;
            Rocker.Enabled = false;
            RotaryHourglass.IsHidden = true;
            RockerHourglass.IsHidden = true;

            // set the repeat interval for seeking to be a bit faster than for volume
            Rocker.RepeatInterval = 0.05;

            iModel = ModelMain.Instance.ModelMediaTime;
            iModel.EventOpen += ModelMediaTimeOpen;
            iModel.EventInitialised += ModelMediaTimeInitialised;
            iModel.EventClose += ModelMediaTimeClose;
            iModel.EventChanged += ModelMediaTimeChanged;

            iController = new ControllerMediaTime(this, iModel);

            ModelMain.Instance.ModelTooltips.EventChanged += ShowTooltipsChanged;

            iEnableRocker = ModelMain.Instance.Helper.EnableRocker;
            iEnableRocker.EventValueChanged += EnableRockerChanged;
            EnableRockerChanged(this, EventArgs.Empty);
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventOpen -= ModelMediaTimeOpen;
            iModel.EventInitialised -= ModelMediaTimeInitialised;
            iModel.EventClose -= ModelMediaTimeClose;
            iModel.EventChanged -= ModelMediaTimeChanged;

            ModelMain.Instance.ModelTooltips.EventChanged -= ShowTooltipsChanged;

            iEnableRocker.EventValueChanged -= EnableRockerChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region IViewMediaTime implementation
        public bool EnableSeekControl
        {
            set { iCurrent.Enabled = value; }
        }

        public uint Value
        {
            set { iCurrent.ViewBar.Value = value; }
        }

        public uint MaxValue
        {
            set { iCurrent.ViewBar.MaxValue = value; }
        }

        public string TextValue
        {
            set { iCurrent.ViewBar.Text = value; }
        }

        public bool PreviewEnabled
        {
            set { iCurrent.ViewBar.PreviewEnabled = value; }
        }

        public uint PreviewValue
        {
            set { iCurrent.ViewBar.PreviewValue = value; }
        }

        public bool EnableHourglass
        {
            set { iCurrentHourglass.Show(value); }
        }
        #endregion IViewMediaTime implementation


        private void ModelMediaTimeOpen(object sender, EventArgs e)
        {
            // should not be called more than once after a Close
            iCurrent.EventStart += RotaryStart;
            iCurrent.EventStop += RotaryStop;
            iCurrent.EventClicked += RotaryClicked;
            iCurrent.EventClockwiseStep += RotaryClockwiseStep;
            iCurrent.EventAnticlockwiseStep += RotaryAnticlockwiseStep;
            iCurrent.EventCancelled += RotaryCancelled;

            Assert.Check(!iOpen);
            iOpen = true;
        }

        private void ModelMediaTimeInitialised(object sender, EventArgs e)
        {
            // initialised can be called many times
            iController.ModelEventInitialised();
        }

        private void ModelMediaTimeClose(object sender, EventArgs e)
        {
            // close can be called without having been opened
            if (iOpen)
            {
                iCurrent.EventStart -= RotaryStart;
                iCurrent.EventStop -= RotaryStop;
                iCurrent.EventClicked -= RotaryClicked;
                iCurrent.EventClockwiseStep -= RotaryClockwiseStep;
                iCurrent.EventAnticlockwiseStep -= RotaryAnticlockwiseStep;
                iCurrent.EventCancelled -= RotaryCancelled;

                iController.ModelEventClose();

                iOpen = false;
            }
        }

        private void ModelMediaTimeChanged(object sender, EventArgs e)
        {
            iController.ModelEventChanged();
        }

        private void RotaryStart(object sender, EventArgs e)
        {
            iController.SeekTargetStarted();
        }

        private void RotaryStop(object sender, EventArgs e)
        {
            iController.SeekTargetStopped();
        }

        private void RotaryCancelled(object sender, EventArgs e)
        {
            iController.SeekTargetCancelled();
        }

        private void RotaryClicked(object sender, EventArgs e)
        {
            iController.TimeClicked();
        }

        private void RotaryClockwiseStep(object sender, EventArgs e)
        {
            iController.SeekTargetIncrement();
        }

        private void RotaryAnticlockwiseStep(object sender, EventArgs e)
        {
            iController.SeekTargetDecrement();
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            if (e.Show)
            {
                Rocker.ToolTip = NSString.StringWithUTF8String("Track control (click center to toggle time display, click left/right to seek within track)");
                Rotary.ToolTip = NSString.StringWithUTF8String("Track control (click center to toggle time display, rotate ring to seek within track)");
            }
            else
            {
                Rocker.ToolTip = null;
                Rotary.ToolTip = null;
            }
        }

        private void EnableRockerChanged(object sender, EventArgs e)
        {
            // close the control if it is open in order to detach the current controller
            bool open = iOpen;
            if (open)
            {
                ModelMediaTimeClose(this, EventArgs.Empty);
            }

            if (iEnableRocker.Native)
            {
                Rotary.IsHidden = true;
                Rocker.IsHidden = false;
                iCurrent = Rocker;
                iCurrentHourglass = RockerHourglass;
            }
            else
            {
                Rocker.IsHidden = true;
                Rotary.IsHidden = false;
                iCurrent = Rotary;
                iCurrentHourglass = RotaryHourglass;
            }

            // reopen the control to attach the new controller
            if (open)
            {
                ModelMediaTimeOpen(this, EventArgs.Empty);
                ModelMediaTimeInitialised(this, EventArgs.Empty);
            }
        }

        [ObjectiveCField]
        public ViewRocker Rocker;

        [ObjectiveCField]
        public ViewHourglass RockerHourglass;

        [ObjectiveCField]
        public ViewRotary Rotary;

        [ObjectiveCField]
        public ViewHourglass RotaryHourglass;

        private ControllerMediaTime iController;
        private IModelMediaTime iModel;
        private OptionBool iEnableRocker;
        private IViewRotary iCurrent;
        private ViewHourglass iCurrentHourglass;
        private bool iOpen;
    }


    // View class to handle the repeat and shuffle - no need to implement a model
    // and controller class for this - too simple
    [ObjectiveCClass]
    public class ViewPlayMode : NSObject, IViewWidgetPlayMode
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewPlayMode));

        public ViewPlayMode() : base() {}
        public ViewPlayMode(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            ButtonRepeat.Initialise(Properties.Resources.IconRepeat, Properties.Resources.IconRepeatOn);
            ButtonShuffle.Initialise(Properties.Resources.IconShuffle, Properties.Resources.IconShuffleOn);

            ButtonRepeat.Enabled = false;
            ButtonShuffle.Enabled = false;
            ButtonRepeat.IsHidden = true;
            ButtonShuffle.IsHidden = true;

            ButtonRepeat.EventClicked += ButtonRepeatClicked;
            ButtonShuffle.EventClicked += ButtonShuffleClicked;

            ModelMain.Instance.ViewMaster.ViewWidgetPlayMode.Add(this);
            ModelMain.Instance.ModelTooltips.EventChanged += ShowTooltipsChanged;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            ButtonRepeat.EventClicked -= ButtonRepeatClicked;
            ButtonShuffle.EventClicked -= ButtonShuffleClicked;

            ModelMain.Instance.ViewMaster.ViewWidgetPlayMode.Remove(this);
            ModelMain.Instance.ModelTooltips.EventChanged -= ShowTooltipsChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region IViewWidgetPlayMode implementation
        public void Open()
        {
        }

        public void Close()
        {
            // called from kinsky threads
            ButtonRepeat.BeginInvoke((MethodInvoker)delegate()
            {
                ButtonRepeat.Enabled = false;
                ButtonShuffle.Enabled = false;
                ButtonRepeat.IsHidden = true;
                ButtonShuffle.IsHidden = true;
            });
        }

        public void Initialised()
        {
            // called from kinsky threads
            ButtonRepeat.BeginInvoke((MethodInvoker)delegate()
            {
                ButtonRepeat.Enabled = true;
                ButtonShuffle.Enabled = true;
                ButtonRepeat.IsHidden = false;
                ButtonShuffle.IsHidden = false;
                Update();
            });
        }

        public void SetShuffle(bool aShuffle)
        {
            // called from kinsky threads
            ButtonRepeat.BeginInvoke((MethodInvoker)delegate()
            {
                ButtonShuffle.IsOn = aShuffle;
                Update();
            });
        }

        public void SetRepeat(bool aRepeat)
        {
            // called from kinsky threads
            ButtonRepeat.BeginInvoke((MethodInvoker)delegate()
            {
                ButtonRepeat.IsOn = aRepeat;
                Update();
            });
        }

        public event EventHandler<EventArgs> EventToggleShuffle;
        public event EventHandler<EventArgs> EventToggleRepeat;
        #endregion IViewWidgetPlayMode implementation


        private void ButtonRepeatClicked(Id aSender)
        {
            EventHandler<EventArgs> ev = EventToggleRepeat;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        private void ButtonShuffleClicked(Id aSender)
        {
            EventHandler<EventArgs> ev = EventToggleShuffle;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            iShowTooltips = e.Show;
            Update();
        }

        private void Update()
        {
            if (iShowTooltips)
            {
                ButtonRepeat.ToolTip = NSString.StringWithUTF8String(ButtonRepeat.IsOn ? "Turn off repeat" : "Turn on repeat");
                ButtonShuffle.ToolTip = NSString.StringWithUTF8String(ButtonShuffle.IsOn ? "Turn off shuffle" : "Turn on shuffle");
            }
            else
            {
                ButtonRepeat.ToolTip = null;
                ButtonShuffle.ToolTip = null;
            }
        }

        [ObjectiveCField]
        public ButtonHoverPush ButtonRepeat;

        [ObjectiveCField]
        public ButtonHoverPush ButtonShuffle;

        private bool iShowTooltips;
    }


    // Base view class to some of the buttons that interface with the kinsky layer
    [ObjectiveCClass]
    public class ViewButton : NSObject, IViewWidgetButton
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewButton));

        public ViewButton() : base() {}
        public ViewButton(IntPtr aInstance) : base(aInstance) {}

        public virtual void OnOpen() {}
        public virtual void OnClose() {}

        #region IViewWidgetButton implementation
        public void Open()
        {
            // called from kinsky threads
            Button.BeginInvoke((MethodInvoker)delegate()
            {
                Button.Enabled = true;
                OnOpen();
            });
        }

        public void Close()
        {
            // called from kinsky threads
            Button.BeginInvoke((MethodInvoker)delegate()
            {
                Button.Enabled = false;
                OnClose();
            });
        }

        public event EventHandler<EventArgs> EventClick;
        #endregion IViewWidgetButton implementation

        protected void OnAwakeFromNib()
        {
            Button.EventClicked += ButtonClicked;

            ModelMain.Instance.ModelTooltips.EventChanged += ShowTooltipsChanged;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public virtual void Dealloc()
        {
            Button.EventClicked -= ButtonClicked;
            ModelMain.Instance.ModelTooltips.EventChanged -= ShowTooltipsChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        protected virtual string TooltipText
        {
            get { return string.Empty; }
        }

        private void ButtonClicked(Id aSender)
        {
            EventHandler<EventArgs> ev = EventClick;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            if (e.Show)
            {
                Button.ToolTip = NSString.StringWithUTF8String(TooltipText);
            }
            else
            {
                Button.ToolTip = null;
            }
        }

        [ObjectiveCField]
        public ButtonHoverPush Button;
    }

    // View class for the save button
    [ObjectiveCClass]
    public class ViewButtonSave : ViewButton, IDragDestination
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewButtonSave));

        public ViewButtonSave() : base() {}
        public ViewButtonSave(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            Button.Initialise(Properties.Resources.IconSave);

            ModelMain.Instance.ViewMaster.ViewWidgetButtonSave.Add(this);

            // setup drag and drop
            Button.RegisterForDraggedTypes(NSArray.ArrayWithObject(PasteboardViewDragData.PboardType));
            Button.DragDelegate = this;

            OnAwakeFromNib();
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public override void Dealloc()
        {
            ModelMain.Instance.ViewMaster.ViewWidgetButtonSave.Remove(this);

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        #region IDragDestination implementation
        public NSDragOperation DraggingEntered(INSDraggingInfo aInfo)
        {
            if ((aInfo.DraggingSourceOperationMask & NSDragOperation.NSDragOperationCopy) == NSDragOperation.NSDragOperationCopy)
            {
                return NSDragOperation.NSDragOperationCopy;
            }

            return NSDragOperation.NSDragOperationNone;
        }

        public bool PerformDragOperation(INSDraggingInfo aInfo)
        {
            // create a draggable data object
            DraggableData dragData = new DraggableData(aInfo.DraggingPasteboard);

            if (dragData != null)
            {
                MediaProviderDraggable draggable = ModelMain.Instance.DropConverterExpand.Convert(dragData);
                if (draggable != null)
                {
                    ModelMain.Instance.ViewSaveSupport.Save(draggable.Media);
                }
            }

            return true;
        }
        #endregion IDragDestination implementation

        protected override string TooltipText
        {
            get { return "Save playlist"; }
        }
    }

    // View class for the delete button
    [ObjectiveCClass]
    public class ViewButtonWasteBin : ViewButton, IDragDestination
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewButtonWasteBin));

        public ViewButtonWasteBin() : base() {}
        public ViewButtonWasteBin(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            Button.Initialise(Properties.Resources.IconDelete);

            ModelMain.Instance.ViewMaster.ViewWidgetButtonWasteBin.Add(this);

            // setup drag and drop
            Button.RegisterForDraggedTypes(NSArray.ArrayWithObjects(PasteboardViewDragData.PboardType, PasteboardViewDragDataBookmarks.PboardType,null));
            Button.DragDelegate = this;

            OnAwakeFromNib();
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public override void Dealloc()
        {
            ModelMain.Instance.ViewMaster.ViewWidgetButtonWasteBin.Remove(this);

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        #region IDragDestination implementation
        public NSDragOperation DraggingEntered(INSDraggingInfo aInfo)
        {
            if ((aInfo.DraggingSourceOperationMask & NSDragOperation.NSDragOperationDelete) == NSDragOperation.NSDragOperationDelete)
            {
                return NSDragOperation.NSDragOperationDelete;
            }

            return NSDragOperation.NSDragOperationNone;
        }

        public bool PerformDragOperation(INSDraggingInfo aInfo)
        {
            return true;
        }
        #endregion IDragDestination implementation

        protected override string TooltipText
        {
            get { return "Delete dragged items (or click to clear playlist)"; }
        }
    }


    // View class to handle the room and source selection
    [ObjectiveCClass]
    public class ViewRoomSource : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewRoomSource));

        public ViewRoomSource() : base() {}
        public ViewRoomSource(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            iButtonRoom = ButtonRoom.Initialise(null);
            iButtonRoom.Text = NSString.StringWithUTF8String("Select room");
            iButtonRoom.TextOnLeft = false;
            iButtonRoom.ImageWidth = 25.0f;
            ButtonRoom.EventClicked += ButtonRoomClicked;

            iButtonSource = ButtonSource.Initialise(null);
            iButtonSource.Text = NSString.StringWithUTF8String("Select source");
            iButtonSource.TextOnLeft = true;
            iButtonSource.ImageWidth = 25.0f;
            ButtonSource.EventClicked += ButtonSourceClicked;

            UpdateButtonWidths();

            ViewPlaylist.PostsFrameChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this,
                                                                             ObjectiveCRuntime.Selector("parentFrameChanged:"),
                                                                             NSView.NSViewFrameDidChangeNotification,
                                                                             ViewPlaylist);

            iModel = ModelMain.Instance;
            iModel.ModelRoomList.EventChanged += RoomListChanged;
            iModel.ModelSourceList.EventChanged += SourceListChanged;
            iModel.ModelTooltips.EventChanged += ShowTooltipsChanged;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);

            iModel.ModelRoomList.EventChanged -= RoomListChanged;
            iModel.ModelSourceList.EventChanged -= SourceListChanged;
            iModel.ModelTooltips.EventChanged -= ShowTooltipsChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        private void RoomListChanged(object sender, EventArgs e)
        {
            Linn.Kinsky.Room room = iModel.ModelRoomList.SelectedItem;
            if (room != null)
            {
                iButtonRoom.Text = NSString.StringWithUTF8String(room.Name);
                iButtonRoom.ImageOff = Properties.Resources.IconRoom;
            }
            else
            {
                iButtonRoom.Text = NSString.StringWithUTF8String("Select room");
                iButtonRoom.ImageOff = null;
            }
            UpdateButtonWidths();
        }

        private void SourceListChanged(object sender, EventArgs e)
        {
            Linn.Kinsky.Source source = iModel.ModelSourceList.SelectedItem;
            if (source != null)
            {
                iButtonSource.Text = NSString.StringWithUTF8String(source.Name);
                iButtonSource.ImageOff = SourceImages.GetImage(source.Type);
            }
            else
            {
                iButtonSource.Text = NSString.StringWithUTF8String("Select source");
                iButtonSource.ImageOff = null;
            }
            UpdateButtonWidths();
        }

        private void ShowTooltipsChanged(object sender, EventArgsTooltips e)
        {
            if (e.Show)
            {
                ButtonRoom.ToolTip = NSString.StringWithUTF8String("Click to select room");
                ButtonSource.ToolTip = NSString.StringWithUTF8String("Click to select source");
            }
            else
            {
                ButtonRoom.ToolTip = null;
                ButtonSource.ToolTip = null;
            }
        }

        private void ButtonRoomClicked(Id aSender)
        {
            // calculate the mid point of the button in screen coordinates
            NSPoint anchor = new NSPoint(ButtonRoom.Bounds.MidX, ButtonRoom.Bounds.MidY);
            anchor = ButtonRoom.ConvertPointToView(anchor, null);
            anchor = ButtonRoom.Window.ConvertBaseToScreen(anchor);

            // calculate the height of the window
            float height = Math.Max(ViewPlaylist.Bounds.Height, 400);

            // create the view and popover
            ViewSelectionRoom view = new ViewSelectionRoom(iModel.ModelRoomList);
            WindowPopover popover = new WindowPopover(view);
            popover.Show(anchor, true, new NSSize(300, height));
        }

        private void ButtonSourceClicked(Id aSender)
        {
            // calculate the mid point of the button in screen coordinates
            NSPoint anchor = new NSPoint(ButtonSource.Bounds.MidX, ButtonSource.Bounds.MidY);
            anchor = ButtonSource.ConvertPointToView(anchor, null);
            anchor = ButtonSource.Window.ConvertBaseToScreen(anchor);

            // calculate the height of the window
            float height = Math.Max(ViewPlaylist.Bounds.Height, 400);

            // create the view and popover
            ViewSelectionSource view = new ViewSelectionSource(iModel.ModelSourceList);
            WindowPopover popover = new WindowPopover(view);
            popover.Show(anchor, false, new NSSize(300, height));
        }

        [ObjectiveCMessage("parentFrameChanged:")]
        public void ParentFrameChanged(NSNotification aNotification)
        {
            UpdateButtonWidths();
        }

        private void UpdateButtonWidths()
        {
            float widthAvailable = ButtonSource.Frame.MaxX - ButtonRoom.Frame.MinX;

            float preferredWidthRoom = ButtonRoom.PreferredWidth;
            float preferredWidthSource = ButtonSource.PreferredWidth;

            float widthRoom;
            float widthSource;

            if (widthAvailable >= preferredWidthRoom + preferredWidthSource)
            {
                // enough space for both buttons at their preferred widths
                widthRoom = preferredWidthRoom;
                widthSource = preferredWidthSource;
            }
            else
            {
                // not enough space
                float halfWidth = widthAvailable * 0.5f;
                if (preferredWidthRoom <= halfWidth)
                {
                    // room button is less than half the available width - allow it to be its preferred size
                    widthRoom = preferredWidthRoom;
                    widthSource = widthAvailable - widthRoom;
                }
                else if (preferredWidthSource <= halfWidth)
                {
                    // source button is less than half the available width - allow it to be its preferred size
                    widthSource = preferredWidthSource;
                    widthRoom = widthAvailable - widthSource;
                }
                else
                {
                    // both buttons preferred widths take up more than half the available width - scale them
                    // so that the ratio of their preferred widths is preserved
                    float ratio = preferredWidthRoom / preferredWidthSource;
                    widthSource = widthAvailable / (1.0f + ratio);
                    widthRoom = widthAvailable - widthSource;
                }
            }

            ButtonRoom.SetWidth(widthRoom, true);
            ButtonRoom.NeedsDisplay = true;

            ButtonSource.SetWidth(widthSource, false);
            ButtonSource.NeedsDisplay = true;
        }

        [ObjectiveCField]
        public ButtonHoverPush ButtonRoom;

        [ObjectiveCField]
        public ButtonHoverPush ButtonSource;

        [ObjectiveCField]
        public NSView ViewPlaylist;

        private ModelMain iModel;
        private IButtonHoverType2 iButtonRoom;
        private IButtonHoverType2 iButtonSource;
    }


    // Class for the now playing window
    [ObjectiveCClass]
    public class WindowNowPlayingController : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(WindowNowPlayingController));

        public WindowNowPlayingController() : base() {}
        public WindowNowPlayingController(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            ImageArtwork.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            ImageArtwork.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            ImageArtwork.ImageScaling = NSImageScaling.NSImageScaleProportionallyUpOrDown;

            ViewTextBkgd.BackgroundColour = NSColor.ColorWithCalibratedWhiteAlpha(0.0f, 0.5f);
            ViewTextBkgd.SetOpaque(false);

            ViewTrackInfoText.FontTitle = NSFont.SystemFontOfSize(26.0f);
            ViewTrackInfoText.FontSubtitle = FontManager.FontLarge;
            ViewTrackInfoText.FontTechInfo = FontManager.FontSmall;
            ViewTrackInfoText.Alignment = NSTextAlignment.NSCenterTextAlignment;
            ViewTrackInfoText.TopAlign = false;

            // setting window level ensures this window always appears above the main window
            Window.Level = NSWindowLevel.NSFloatingWindowLevel;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            this.SendMessageSuper(ThisClass, "dealloc");
        }

        public void Show()
        {
            // set this stuff here - these are set to different values in the awakeFromNib for the WindowBorderless class
            // which can get called after the awakeFromNib for this class
            Window.BackgroundColor = NSColor.BlackColor;
            Window.IsOpaque = true;

            // calculate initial rect from the image view in the main window
            NSRect windowRect = ImageArtworkMain.ConvertRectToView(ImageArtworkMain.Bounds, null);
            NSRect initialRect = new NSRect(WindowMain.ConvertBaseToScreen(windowRect.origin), windowRect.size);

            // set the initial rect for the window
            Window.SetFrameDisplay(initialRect, false);
            Window.AlphaValue = 0.0f;
            Window.MakeKeyAndOrderFront(this);

            // calculate the final frame
            NSRect finalRect = WindowMain.Frame.InsetRect(5, 5);

            // animate to cover the whole main window
            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = 0.15;

            Window.Animator.AlphaValue = 1.0f;
            Window.Animator.SetFrameDisplay(finalRect, true);

            NSAnimationContext.EndGrouping();
        }

        public void UpdateFrame(NSRect aRect)
        {
            Window.SetFrameDisplay(aRect.InsetRect(5, 5), true);
        }

        public void Hide()
        {
            // calculate the final rect from the image view in the main window
            NSRect windowRect = ImageArtworkMain.ConvertRectToView(ImageArtworkMain.Bounds, null);
            NSRect finalRect = new NSRect(WindowMain.ConvertBaseToScreen(windowRect.origin), windowRect.size);

            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = 0.15;

            Window.Animator.AlphaValue = 0.0f;
            Window.Animator.SetFrameDisplay(finalRect, true);

            NSAnimationContext.EndGrouping();

            WindowMain.MakeKeyAndOrderFront(this);
        }

        [ObjectiveCField]
        public NSWindow Window;

        [ObjectiveCField]
        public ImageViewClickable ImageArtwork;

        [ObjectiveCField]
        public NSWindow WindowMain;

        [ObjectiveCField]
        public NSImageView ImageArtworkMain;

        [ObjectiveCField]
        public ViewEmpty ViewTextBkgd;

        [ObjectiveCField]
        public ViewTrackInfo ViewTrackInfoText;
    }
}



