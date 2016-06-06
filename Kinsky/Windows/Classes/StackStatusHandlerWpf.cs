using System;
using System.Windows;

namespace Linn
{
    public class StackStatusHandlerWpf : StackStatusHandler
    {
        public StackStatusHandlerWpf(string aTitle) {
            iTitle = aTitle;
            iNotifyIcon = null;
        }

        public StackStatusHandlerWpf(string aTitle, System.Windows.Forms.NotifyIcon aNotifyIcon)
        {
            iTitle = aTitle;
            iNotifyIcon = aNotifyIcon;
        }

        public override void StackStatusStartupChanged(object sender, EventArgsStackStatus e)
        {
            string msg;
            MessageBoxResult res;

            switch (e.Status.State)
            {
                case EStackState.eStopped:
                case EStackState.eOk:
                    break;

                case EStackState.eNoInterface:
                    msg = iTitle + " requires a network adapter to be configured.";
                    msg += "\n\nWould you like to select a network adapter now?";
                    res = MessageBox.Show(msg, "No network configuration",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);
                    ShowOptions = (res == MessageBoxResult.Yes);
                    break;

                case EStackState.eBadInterface:
                case EStackState.eNonexistentInterface:
                    msg = "A problem occurred with the configured network adapter (" + e.Status.Interface.Name + ")";
                    msg += "\n\nWould you like to select a different network adapter now?";
                    res = MessageBox.Show(msg, "Network configuration error",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Error);
                    ShowOptions = (res == MessageBoxResult.Yes);
                    break;
            }
        }

        public override void StackStatusChanged(object sender, EventArgsStackStatus e) {
            switch (e.Status.State)
            {
                case EStackState.eStopped:
                    break;
                case EStackState.eOk:
                    if (iNotifyIcon != null && iNotifyIcon.Visible) {
                        iNotifyIcon.ShowBalloonTip(5000, iTitle, iTitle + " is now connected using \"" + e.Status.Interface.Name + "\"", System.Windows.Forms.ToolTipIcon.Info);
                    }
                    break;
                case EStackState.eBadInterface:
                case EStackState.eNoInterface:
                case EStackState.eNonexistentInterface:
                    if (iNotifyIcon != null && iNotifyIcon.Visible) {
                        iNotifyIcon.ShowBalloonTip(10000, iTitle, iTitle + " has lost connectivity.", System.Windows.Forms.ToolTipIcon.Warning);
                    }
                    break;
            }
        }

        public override void StackStatusOptionsChanged(object sender, EventArgsStackStatus e) {
            string msg;

            switch (e.Status.State)
            {
                case EStackState.eStopped:
                case EStackState.eOk:
                case EStackState.eNoInterface:
                case EStackState.eNonexistentInterface:
                    break;

                case EStackState.eBadInterface:
                    msg = "A problem occurred with the selected network adapter (" + e.Status.Interface.Name + ")";
                    MessageBox.Show(msg, "Network configuration error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    break;
            }
        }

        private string iTitle;
        private System.Windows.Forms.NotifyIcon iNotifyIcon;
    }

}



