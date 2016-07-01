
using System;
using System.Threading;

using Monobjc;
using Monobjc.Cocoa;

using Linn;
using Linn.Kinsky;
using System.Diagnostics;

namespace KinskyDesktop
{
    // File's owner class for the WindowNotification.xib file
    [ObjectiveCClass]
    public class WindowNotification : NSWindowController
    {
        private INotification iNotification;
        public WindowNotification () : base() {}
        public WindowNotification (IntPtr aInstance) : base(aInstance) {}

        public WindowNotification (INotification aNotification)
            : base()
        {
            iNotification = aNotification;
            NSBundle.LoadNibNamedOwner("WindowNotification.nib", this);
        }

        public void Show(Action aClosed)
        {
            Window.SetDelegate(d =>
            {
                d.WindowWillClose += (n) => aClosed();
            });
            Window.Center();
            Window.MakeKeyAndOrderFront(this);
            // cannot make this window modal as it breaks webview
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            LoadUri (iNotification.Uri);

            ButtonDontShowAgain.State = (iNotification.DontShowAgain) ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
            ButtonClose.ActionEvent += ButtonCloseClicked;
            ButtonGetKazoo.ActionEvent += GetKazooClicked;

            Window = WindowOutlet;
        }

        private void LoadUri (string aUri)
        {
            WebView.SetValueForKey (aUri, "mainFrameURL");
        }

        private void ButtonCloseClicked (Id aSender)
        {
            iNotification.TrackUsageEventDismissed(false, ButtonDontShowAgain.State == NSCellStateValue.NSOnState);
            this.Dismiss ();
        }

        public void Dismiss ()
        {
            iNotification.Closed (ButtonDontShowAgain.State == NSCellStateValue.NSOnState);
            this.Close ();
        }

        private void GetKazooClicked (Id aSender)
        {
            iNotification.TrackUsageEventDismissed(true, ButtonDontShowAgain.State == NSCellStateValue.NSOnState);
            this.Dismiss ();
            GetKazoo();
        }

        private void GetKazoo ()
        {
            var appId = "848937349";
            var appName = "linn-kazoo";

            var appStoreLink = string.Format("macappstores://itunes.apple.com/app/{0}/id{1}?mt=12", appName, appId);
            var httpsLink = string.Format ("https://itunes.apple.com/app/{0}/id{1}?mt=12", appName, appId);
            var legacyLink = "https://www.linn.co.uk/software#kazoo";

            var link = IsAppStoreCapable ? appStoreLink : legacyLink;

            try { Process.Start (new ProcessStartInfo (link)); } catch {
                // fallback to https store version if appstore call fails
                try { Process.Start (new ProcessStartInfo (httpsLink)); } catch {
                    UserLog.WriteLine ("failed to launch Kazoo store link");
                } 
            }
        }

        private bool IsAppStoreCapable
        {
                get{
                    try {
                        var version = Environment.OSVersion.Version;
                        return !(version.Major == 10 && version.Minor < 12);
                    } catch {
                        return false;
                    }
            }
        }

        [ObjectiveCField]
        public NSButton ButtonClose;

        [ObjectiveCField]
        public NSButton ButtonGetKazoo;

        [ObjectiveCField]
        public NSButton ButtonDontShowAgain;

        [ObjectiveCField]
        public NSView WebView;

        [ObjectiveCField]
        public NSWindow WindowOutlet;

    }
}

