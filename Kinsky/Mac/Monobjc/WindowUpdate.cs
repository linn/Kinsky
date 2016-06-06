
using System;
using System.Threading;

using Monobjc;
using Monobjc.Cocoa;

using Linn;


namespace KinskyDesktop
{
    // File's owner class for the WindowUpdate.xib file
    [ObjectiveCClass]
    public class WindowUpdate : NSWindowController
    {
        public WindowUpdate() : base() {}
        public WindowUpdate(IntPtr aInstance) : base(aInstance) {}

        public WindowUpdate(AutoUpdate aAutoUpdate)
            : base()
        {
            iAutoUpdate = aAutoUpdate;
            NSBundle.LoadNibNamedOwner("WindowUpdate.nib", this);
        }

        public bool Show()
        {
            // initialise the window
            iUpdateInfo = null;
            iState = WindowUpdate.EState.eChecking;
            iUpdateStarted = false;
            UpdateControls();

            // start the check thread
            iThread = new Thread(ThreadFuncCheck);
            iThread.IsBackground = true;
            iThread.Name = "UpdateCheck";
            iThread.Start();

            // show the window modally
            Window.Center();
            Window.MakeKeyAndOrderFront(this);
            NSApplication.NSApp.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, null, null, IntPtr.Zero);
            NSApplication.NSApp.RunModalForWindow(Window);
            NSApplication.NSApp.EndSheet(Window);
            Window.OrderOut(this);

            // returns true if the update has successfully started
            return iUpdateStarted;
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            ButtonDetails.ActionEvent += ButtonDetailsClicked;
            ButtonClose.ActionEvent += ButtonCloseClicked;
            ButtonUpdate.ActionEvent += ButtonUpdateClicked;

            Window.SetDelegate(d =>
            {
                d.WindowWillClose += WindowWillClose;
            });
        }

        private void WindowWillClose(NSNotification aNotification)
        {
            if (iThread != null)
            {
                iThread.Abort();
                iThread.Join();
                iThread = null;
            }
            NSApplication.NSApp.StopModal();
        }

        private void ButtonDetailsClicked(Id aSender)
        {
            try
            {
                System.Diagnostics.Process.Start(iUpdateInfo.History.AbsoluteUri);
            }
            catch (Exception)
            {
                NSAlert alert = new NSAlert();

                alert.AddButtonWithTitle(NSString.StringWithUTF8String("Close"));
                alert.MessageText = NSString.StringWithUTF8String("Failed to retrieve update details.");
                alert.InformativeText = NSString.StringWithUTF8String("Failed to contact " + iUpdateInfo.History);
                alert.AlertStyle = NSAlertStyle.NSWarningAlertStyle;

                alert.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, null, IntPtr.Zero);

                alert.Release();
            }
        }

        private void ButtonCloseClicked(Id aSender)
        {
            this.Close();
        }

        private void ButtonUpdateClicked(Id aSender)
        {
            if (iUpdateInfo.IsCompatibilityFamilyUpgrade)
            {
                NSAlert alert = new NSAlert();

                alert.AddButtonWithTitle(NSString.StringWithUTF8String("Update"));
                alert.AddButtonWithTitle(NSString.StringWithUTF8String("Cancel"));
                alert.MessageText = NSString.StringWithUTF8String("This is a compatibility family upgrade. Do you wish to continue with the upgrade?");
                alert.InformativeText = NSString.StringWithUTF8String("Updating " + iUpdateInfo.Name + " to a new compatibility family will also require updating Linn DS firmware.");
                alert.AlertStyle = NSAlertStyle.NSWarningAlertStyle;

                alert.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, CompatibilityAlertEnd, IntPtr.Zero);

                alert.Release();
            }
            else
            {
                CompatibilityAlertEnd(null, NSAlert.NSAlertFirstButtonReturn, IntPtr.Zero);
            }
        }

        private void CompatibilityAlertEnd(NSAlert aAlert, int aReturnCode, IntPtr aZero)
        {
            if (aReturnCode == NSAlert.NSAlertFirstButtonReturn)
            {
                iState = WindowUpdate.EState.eDownloading;
                UpdateControls();

                iAutoUpdate.EventUpdateProgress += AutoUpdateProgress;
                iAutoUpdate.EventUpdateFailed += AutoUpdateFailed;

                iThread = new Thread(ThreadFuncUpdate);
                iThread.IsBackground = true;
                iThread.Name = "Update";
                iThread.Start();
            }
        }


        private void ThreadFuncCheck()
        {
            iUpdateInfo = iAutoUpdate.CheckForUpdate();

            NSApplication.NSApp.BeginInvoke((MethodInvoker)delegate()
            {
                iState = (iUpdateInfo != null) ? WindowUpdate.EState.eAvailable : WindowUpdate.EState.eUnavailable;
                iThread = null;

                UpdateControls();
            });
        }


        private void ThreadFuncUpdate()
        {
            // download
            iAutoUpdate.DownloadUpdate(iUpdateInfo);

            // update UI
            NSApplication.NSApp.BeginInvoke((MethodInvoker)delegate()
            {
                iState = WindowUpdate.EState.eUpdating;
                UpdateControls();
            });

            // start the update
            if (iAutoUpdate.ApplyUpdate(iUpdateInfo))
            {
                // update started successfully - close the dialog
                NSApplication.NSApp.BeginInvoke((MethodInvoker)delegate()
                {
                    iUpdateStarted = true;
                    iThread = null;
                    this.Close();
                });
            }
            else
            {
                // failed to update
                NSApplication.NSApp.BeginInvoke((MethodInvoker)delegate()
                {
                    iThread = null;
                });
            }
        }

        private void AutoUpdateProgress(object sender, EventArgs e)
        {
            NSApplication.NSApp.BeginInvoke((MethodInvoker)delegate()
            {
                Progress.DoubleValue = iAutoUpdate.UpdateProgress;
            });
        }

        private void AutoUpdateFailed(object sender, EventArgs e)
        {
            NSApplication.NSApp.BeginInvoke((MethodInvoker)delegate()
            {
                iState = WindowUpdate.EState.eFailed;
                UpdateControls();
            });
        }

        private void UpdateControls()
        {
            switch (iState)
            {
            case EState.eChecking:
                Text.StringValue = NSString.StringWithUTF8String("Checking for updates...");
                Progress.IsIndeterminate = true;
                Progress.StartAnimation(this);
                DisableButton(ButtonDetails);
                DisableButton(ButtonUpdate);
                EnableButton(ButtonClose, true);
                break;

            case EState.eUnavailable:
                Progress.StopAnimation(this);
                Progress.IsIndeterminate = false;
                Text.StringValue = NSString.StringWithUTF8String("There are no updates available.");
                DisableButton(ButtonDetails);
                DisableButton(ButtonUpdate);
                EnableButton(ButtonClose, true);
                break;

            case EState.eAvailable:
                Progress.StopAnimation(this);
                Progress.IsIndeterminate = false;
                Text.StringValue = NSString.StringWithUTF8String(string.Format("There is a new version of {0} ({1}) available.", iUpdateInfo.Name, iUpdateInfo.Version));
                EnableButton(ButtonDetails, false);
                EnableButton(ButtonClose, false);
                EnableButton(ButtonUpdate, true);
                break;

            case EState.eDownloading:
                Text.StringValue = NSString.StringWithUTF8String(string.Format("Downloading {0} ({1}).", iUpdateInfo.Name, iUpdateInfo.Version));
                EnableButton(ButtonDetails, false);
                EnableButton(ButtonClose, false);
                DisableButton(ButtonUpdate);
                break;

            case EState.eUpdating:
                Progress.IsIndeterminate = true;
                Progress.StartAnimation(this);
                Text.StringValue = NSString.StringWithUTF8String(string.Format("Updating {0} to {1}.", iUpdateInfo.Name, iUpdateInfo.Version));
                DisableButton(ButtonDetails);
                DisableButton(ButtonClose);
                DisableButton(ButtonUpdate);
                break;

            case EState.eFailed:
                Progress.IsIndeterminate = false;
                Progress.StopAnimation(this);
                Progress.DoubleValue = 0;
                Text.StringValue = NSString.StringWithUTF8String(string.Format("The update failed."));
                DisableButton(ButtonDetails);
                EnableButton(ButtonClose, true);
                DisableButton(ButtonUpdate);
                break;
            }
        }

        private void DisableButton(NSButton aButton)
        {
            aButton.IsEnabled = false;
        }

        private void EnableButton(NSButton aButton, bool aDefault)
        {
            aButton.IsEnabled = true;
            aButton.KeyEquivalent = aDefault ? NSString.StringWithUTF8String("\r") : NSString.Empty;
        }

        [ObjectiveCField]
        public NSTextField Text;

        [ObjectiveCField]
        public NSProgressIndicator Progress;

        [ObjectiveCField]
        public NSButton ButtonDetails;

        [ObjectiveCField]
        public NSButton ButtonClose;

        [ObjectiveCField]
        public NSButton ButtonUpdate;

        private enum EState
        {
            eChecking,
            eAvailable,
            eUnavailable,
            eDownloading,
            eUpdating,
            eFailed
        }

        private AutoUpdate iAutoUpdate;
        private AutoUpdate.AutoUpdateInfo iUpdateInfo;
        private Thread iThread;
        private EState iState;
        private bool iUpdateStarted;
    }
}

