using System;
using Linn;
using Linn.Kinsky;

namespace KinskyDesktop
{
    public class NotificationView : INotificationView
    {
        private readonly NotificationController iNotificationController;
        private INotification iNotification;
        private WindowNotification iCurrentWindow;

        public event EventHandler<EventArgs> EventNotificationUpdated;

        public NotificationView (HelperKinsky aHelper)
        {
            iNotificationController = new NotificationController (aHelper.Invoker, aHelper, new NotificationServerHttp (NotificationServerHttp.DefaultUri (aHelper.Product)), this, NotificationController.DefaultTimespan);
        }

        public void Update (INotification aNotification, bool aShowNow)
        {
            iNotification = aNotification;
            if (aShowNow) {
                Show();
            }
            var del = EventNotificationUpdated;
            if (del != null) {
                del (this, EventArgs.Empty);
            }
        }

        public bool CanShow {
            get
            {
                return iNotification != null;
            }
        }

        public bool HasUnacknowledgedNotification
        {
            get
            {
                return iNotification != null && !iNotification.HasBeenAcknowledged;
            }
        }

        public void Show ()
        {
            Assert.Check (iNotification != null);
            if (iCurrentWindow != null) {
                iCurrentWindow.Close ();
            }
            iCurrentWindow = new WindowNotification (iNotification);
            iCurrentWindow.Show (() => {
                iCurrentWindow.Release ();
                iCurrentWindow = null;
            });
        }
    }
}

