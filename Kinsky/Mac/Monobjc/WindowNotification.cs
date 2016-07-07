
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
        private bool iDismissed = false;

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
                d.WindowWillClose += (n) => {
                    OnDismissed (false); // in case window is closed from title bar
                    aClosed ();
                };
            });
            Window.Center();
            Window.MakeKeyAndOrderFront(this);
            // cannot make this window modal as it breaks webview
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            WebView.SetValueForKey (false, "drawsBackground"); // prevent white background on webview
            LoadUri (iNotification.Uri(true));
            iNotification.Shown();

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
            iNotification.TrackUsageEventDismissed(false);
            this.OnDismissed (false);
            this.Close ();
        }

        private void OnDismissed (bool aGetKazooClicked)
        {
            if (!iDismissed) {
                iDismissed = true;
                iNotification.TrackUsageEventDismissed (aGetKazooClicked);
                iNotification.Closed ();
            }
        }

        private void GetKazooClicked (Id aSender)
        {
            this.OnDismissed (true);
            this.Close ();
            GetKazoo();
        }

        private void GetKazoo ()
        {
            /*var appId = "848937349";
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
            }*/
            var link = "https://www.linn.co.uk/software#kazoo";
            try { Process.Start (new ProcessStartInfo (link)); } catch { }
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
        public NSView WebView;

        [ObjectiveCField]
        public NSWindow WindowOutlet;

    }
}

