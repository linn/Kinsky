
using System;

using Linn;
using Linn.Toolkit.Cocoa;
using Linn.Kinsky;

using Monobjc;
using Monobjc.Cocoa;


// View classes that correspond to the MainMenu.xib file
using System.Runtime.InteropServices;

namespace KinskyDesktop
{
    // Main NSApplication delegate - a bit like a view for the top-level
    // application - even though this class acts like a controller for the NSApplication, we
    // treat it like a view so that our controllers can be toolkit independent
    [ObjectiveCClass]
    public class AppDelegate : NSObject, IViewApp, IInvoker, IAppRestartHandler
    {

        private System.Threading.Thread iThread;

        public AppDelegate() : base() { Init(); }
        public AppDelegate(IntPtr aInstance) : base(aInstance) { Init(); }

        private void Init()
        {
            iThread = System.Threading.Thread.CurrentThread;
        }

        #region IViewApp implementation
        public ICrashLogDumper CreateCrashLogDumper(IHelper aHelper)
        {
            return new CrashLogDumperMonobjc(aHelper.Title, aHelper.Product, aHelper.Version);
        }

        public void ShowAlertPanel(string aTitle, string aMessage)
        {
            AppKitFramework.NSRunAlertPanel(aTitle, aMessage, "OK", null, null);
        }
        #endregion IViewApp implementation


        #region IInvoker implementation
        bool IInvoker.InvokeRequired
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId != iThread.ManagedThreadId; }
        }

        void IInvoker.BeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
                try
                {
#if DEBUG || TRACE
                    Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKING {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                    NSApplication.NSApp.BeginInvoke(aDelegate, aArgs);
#if DEBUG || TRACE
                    Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKED {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                }
                catch (System.Exception ex)
                {
                    UserLog.WriteLine("Exception: " + ex);
                    UserLog.WriteLine("Invocation details: " + this.GetCallInfo(aDelegate, aArgs));
                    throw ex;
                }
        }

        bool IInvoker.TryBeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            if ((this as IInvoker).InvokeRequired)
            {
                NSApplication.NSApp.BeginInvoke(aDelegate, aArgs);
                return true;
            }
            return false;
        }
        #endregion IInvoker implementation


        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // create the controller for the application - this also creates the model
            iController = new ControllerApp(this, this, this, new Rect(NSScreen.MainScreen.VisibleFrame));

            // create the artwork cache for the application
            ArtworkCacheInstance.Instance = new ArtworkCache();

            // the model has now been created - listen to kompact mode changes
            ModelMain.Instance.Helper.KompactMode.EventValueChanged += KompactModeChanged;
            KompactModeChanged(this, EventArgs.Empty);

            // set up model eventing for the dock menu
            DockMenu.Initialise(ModelMain.Instance.ModelTransport, ModelMain.Instance.ModelVolumeControl, this);
            ModelMain.Instance.ViewMaster.ViewWidgetPlayMode.Add(DockMenu);
        }

        [ObjectiveCMessage("applicationWillFinishLaunching:")]
        public void ApplicationWillFinishLaunching(NSObject aNoticiation)
        {
            // create the main window
            iMainWindow = new WindowMainController();
            iMainWindow.EventWindowMainClosed += MainWindowClosed;
            NSBundle.LoadNibNamedOwner("MainWindow.nib", iMainWindow);

            // create the about box
            iWindowAbout = new WindowAbout();
            NSBundle.LoadNibNamedOwner("WindowAbout.nib", iWindowAbout);

            // initialise the main window from model data
            iController.InitialiseMainWindow(iMainWindow);

            // start the model - network stack etc...
            iController.Start(iMainWindow);

            //register for fast user switching notifications
            IntPtr handlerPtr = Monobjc.ObjectiveCRuntime.Selector("sessionChangedHandler:");
            NSWorkspace.SharedWorkspace.NotificationCenter.AddObserverSelectorNameObject(this, handlerPtr, NSWorkspace.NSWorkspaceSessionDidBecomeActiveNotification, null);
            NSWorkspace.SharedWorkspace.NotificationCenter.AddObserverSelectorNameObject(this, handlerPtr, NSWorkspace.NSWorkspaceSessionDidResignActiveNotification, null);
        }

        [ObjectiveCMessage("sessionChangedHandler:")]
        public void SessionChangeHandler(NSNotification aNotification)
        {
            UserLog.WriteLine("SessionChangeHandler: " + aNotification.Name.ToString());
            if (aNotification.Name.ToString().Equals(NSWorkspace.NSWorkspaceSessionDidBecomeActiveNotification))
            {
                iController.Resume();
            }
            else
            {
                iController.Pause();
            }
        }

        [ObjectiveCMessage("applicationWillTerminate:")]
        public void ApplicationWillTerminate(NSNotification aNotification)
        {
            // stop the model
            iController.Stop();

            // delete the main window
            iMainWindow.EventWindowMainClosed -= MainWindowClosed;
            iMainWindow.Release();
            iMainWindow = null;

            // delete the about box
            iWindowAbout.Release();
            iWindowAbout = null;
        }

        [ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
        public bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication aApplication)
        {
            return true;
        }


        [ObjectiveCMessage("about:")]
        public void About(Id aSender)
        {
            iWindowAbout.Show();
        }

        [ObjectiveCMessage("debugConsole:")]
        public void DebugConsole(Id aSender)
        {
            // close existing debug window
            if (iUserLog != null)
            {
                iUserLog.Window.Close();
                iUserLog.Release();
                iUserLog = null;
            }

            // load a fresh debug window
            iUserLog = new UserLogDialogMonobjc();
            NSBundle.LoadNibNamedOwner("UserLog.nib", iUserLog);
            iUserLog.Window.MakeKeyAndOrderFront(this);
        }

        [ObjectiveCMessage("onlineHelp:")]
        public void OnlineHelp(Id aSender)
        {
            const string kOnlineManualUri = "http://oss.linn.co.uk/trac/wiki/KinskyMacDavaarManual";

            try
            {
                System.Diagnostics.Process.Start(kOnlineManualUri);
            }
            catch (Exception)
            {
                IViewMainWindow v = iMainWindow;
                v.ShowAlertPanel("Online Help Failed", "Failed to contact " + kOnlineManualUri);
            }
        }

        [ObjectiveCMessage("preferences:")]
        public void Preferences(Id aSender)
        {
            OptionDialogMonobjc window = new OptionDialogMonobjc(ModelMain.Instance.Helper.OptionPages);
            window.Open();
        }

        [ObjectiveCMessage("rescanNetwork:")]
        public void RescanNetwork(Id aSender)
        {
            iController.RescanNetwork();
        }

        [ObjectiveCMessage("checkForUpdates:")]
        public void CheckForUpdates(Id aSender)
        {
            WindowUpdate windowUpdate = new WindowUpdate(ModelMain.Instance.AutoUpdate);
            bool close = windowUpdate.Show();
            windowUpdate.Release();

            if (close)
            {
                iMainWindow.Window.Close();
            }
        }

        private void KompactModeChanged(object sender, EventArgs e)
        {
            OptionBool kompactMode = ModelMain.Instance.Helper.KompactMode;

            MenuItemKompactMode.State = kompactMode.Native ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
        }


        #region IAppRestartHandler implementation
        public void Restart()
        {
            IInvoker invoker = this;
            if (invoker.TryBeginInvoke((MethodInvoker)Restart))
                return;

            IViewMainWindow v = iMainWindow;
            v.ShowAlertPanel("Restart", "Please restart Kinsky to complete plugin installation.");
        }
        #endregion IAppRestartHandler implementation


        private void MainWindowClosed(object sender, EventArgs e)
        {
            // close the debug window
            if (iUserLog != null)
            {
                iUserLog.Window.Close();
                iUserLog.Release();
                iUserLog = null;
            }

            iWindowAbout.Close();
        }

        [ObjectiveCField]
        public NSMenuItem MenuItemKompactMode;

        [ObjectiveCField]
        public ViewDockMenu DockMenu;

        private ControllerApp iController;
        private WindowMainController iMainWindow;
        private UserLogDialogMonobjc iUserLog;
        private WindowAbout iWindowAbout;
    }


    // Class for the file's owner for the about box
    [ObjectiveCClass]
    public class WindowAbout : NSWindowController
    {
        public WindowAbout() : base() {}
        public WindowAbout(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            TextFieldName.Font = FontManager.FontSemiLarge;
            TextFieldVersion.Font = FontManager.FontMedium;
            TextFieldCopyright.Font = FontManager.FontMedium;

            IHelper helper = ModelMain.Instance.Helper;

            string version = "Version " + helper.Version;
            if (helper.Product.Contains("NightlyBuild"))
            {
                NSString buildVersion = NSBundle.MainBundle.InfoDictionary[NSString.StringWithUTF8String("CFBundleVersion")].CastAs<NSString>();

                version += "." + buildVersion.ToString();

            }
            version += " (" + helper.Family + ")";

            ImageAppIcon.Image = Properties.Resources.ImageAbout;
            TextFieldName.StringValue = NSString.StringWithUTF8String(helper.Product);
            TextFieldVersion.StringValue = version;
            TextFieldCopyright.StringValue = NSString.StringWithUTF8String(helper.Copyright + " " + helper.Company);
        }

        public void Show()
        {
            if (!Window.IsVisible)
            {
                Window.Center();
            }
            Window.MakeKeyAndOrderFront(this);
        }

        [ObjectiveCField]
        public NSImageView ImageAppIcon;

        [ObjectiveCField]
        public NSTextField TextFieldName;

        [ObjectiveCField]
        public NSTextField TextFieldVersion;

        [ObjectiveCField]
        public NSTextField TextFieldCopyright;
    }


    // View class for the dock menu
    [ObjectiveCClass]
    public class ViewDockMenu : NSObject, IViewWidgetPlayMode
    {
        public ViewDockMenu() : base() {}
        public ViewDockMenu(IntPtr aInstance) : base(aInstance) {}


        public void Initialise(IModelTransport aModelTransport, IModelVolumeControl aModelVolume, IInvoker aInvoker)
        {
            iModelTransport = aModelTransport;
            iModelTransport.EventInitialised += ModelTransportInitialised;
            iModelTransport.EventClose += ModelTransportClose;
            iModelTransport.EventChanged += ModelTransportChanged;

            iControllerTransport = new ControllerTransport(iModelTransport);

            iModelVolume = aModelVolume;
            iModelVolume.EventOpen += ModelVolumeOpen;
            iModelVolume.EventClose += ModelVolumeClose;
            iModelVolume.EventChanged += ModelVolumeChanged;

            iInvoker = aInvoker;
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            MenuItemRepeat.ActionEvent += MenuItemRepeatClicked;
            MenuItemShuffle.ActionEvent += MenuItemShuffleClicked;
            MenuItemPrevious.ActionEvent += MenuItemPreviousClicked;
            MenuItemPlayPauseStop.ActionEvent += MenuItemPlayPauseStopClicked;
            MenuItemNext.ActionEvent += MenuItemNextClicked;
            MenuItemMute.ActionEvent += MenuItemMuteClicked;
        }


        #region events from IModelTransport
        private void ModelTransportInitialised(object sender, EventArgs e)
        {
            MenuItemPrevious.IsEnabled = true;
            MenuItemPlayPauseStop.IsEnabled = true;
            MenuItemNext.IsEnabled = true;
        }

        private void ModelTransportClose(object sender, EventArgs e)
        {
            MenuItemPrevious.IsEnabled = false;
            MenuItemPlayPauseStop.IsEnabled = false;
            MenuItemNext.IsEnabled = false;
        }

        private void ModelTransportChanged(object sender, EventArgs e)
        {
            switch (iModelTransport.TransportState)
            {
            case ETransportState.ePlaying:
            case ETransportState.eBuffering:
                MenuItemPlayPauseStop.Title = NSString.StringWithUTF8String(iModelTransport.PauseNotStop ? "Pause" : "Stop");
                break;

            default:
                MenuItemPlayPauseStop.Title = NSString.StringWithUTF8String("Play");
                break;
            }
        }
        #endregion events from IModelTransport


        #region events from IModelVolumeControl
        private void ModelVolumeOpen(object sender, EventArgs e)
        {
            MenuItemMute.IsEnabled = true;
        }

        private void ModelVolumeClose(object sender, EventArgs e)
        {
            MenuItemMute.IsEnabled = false;
        }

        private void ModelVolumeChanged(object sender, EventArgs e)
        {
            MenuItemMute.State = iModelVolume.Mute ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
        }
        #endregion events from IModelVolumeControl


        #region IViewWidgetPlayMode implementation
        void IViewWidgetPlayMode.Open()
        {
        }

        void IViewWidgetPlayMode.Close()
        {
            iInvoker.BeginInvoke((MethodInvoker)delegate()
            {
                MenuItemRepeat.IsEnabled = false;
                MenuItemShuffle.IsEnabled = false;
            });
        }

        void IViewWidgetPlayMode.Initialised()
        {
            iInvoker.BeginInvoke((MethodInvoker)delegate()
            {
                MenuItemRepeat.IsEnabled = true;
                MenuItemShuffle.IsEnabled = true;
            });
        }

        void IViewWidgetPlayMode.SetShuffle(bool aShuffle)
        {
            iInvoker.BeginInvoke((MethodInvoker)delegate()
            {
                MenuItemShuffle.State = aShuffle ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
            });
        }

        void IViewWidgetPlayMode.SetRepeat(bool aRepeat)
        {
            iInvoker.BeginInvoke((MethodInvoker)delegate()
            {
                MenuItemRepeat.State = aRepeat ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
            });
        }

        public event EventHandler<EventArgs> EventToggleShuffle;
        public event EventHandler<EventArgs> EventToggleRepeat;
        #endregion IViewWidgetPlayMode implementation


        private void MenuItemRepeatClicked(Id aSender)
        {
            EventHandler<EventArgs> ev = EventToggleRepeat;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        private void MenuItemShuffleClicked(Id aSender)
        {
            EventHandler<EventArgs> ev = EventToggleShuffle;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        private void MenuItemPreviousClicked(Id aSender)
        {
            iControllerTransport.ButtonPreviousClicked();
        }

        private void MenuItemPlayPauseStopClicked(Id aSender)
        {
            if (iModelTransport.PauseNotStop)
            {
                iControllerTransport.ButtonPlayPauseClicked();
            }
            else
            {
                iControllerTransport.ButtonPlayStopClicked();
            }
        }

        private void MenuItemNextClicked(Id aSender)
        {
            iControllerTransport.ButtonNextClicked();
        }

        private void MenuItemMuteClicked(Id aSender)
        {
            iModelVolume.ToggleMute();
        }

        [ObjectiveCField]
        public NSMenuItem MenuItemRepeat;

        [ObjectiveCField]
        public NSMenuItem MenuItemShuffle;

        [ObjectiveCField]
        public NSMenuItem MenuItemPrevious;

        [ObjectiveCField]
        public NSMenuItem MenuItemPlayPauseStop;

        [ObjectiveCField]
        public NSMenuItem MenuItemNext;

        [ObjectiveCField]
        public NSMenuItem MenuItemMute;

        private IModelTransport iModelTransport;
        private IModelVolumeControl iModelVolume;
        private ControllerTransport iControllerTransport;
        private IInvoker iInvoker;
    }
}



