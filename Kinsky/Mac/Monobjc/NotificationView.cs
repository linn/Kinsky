using System;
using Linn;
using Linn.Kinsky;

namespace KinskyDesktop
{
    public class NotificationView : INotificationView
    {
        private readonly NotificationController iNotificationController;
        private readonly INotificationPersistence iNotificationPersistence;
        private INotification iNotification;
        private WindowNotification iCurrentWindow;

        public event EventHandler<EventArgs> EventNotificationUpdated;

        public NotificationView (HelperKinsky aHelper)
        {
            iNotificationPersistence = aHelper;
            iNotificationController = new NotificationController (aHelper.Invoker, iNotificationPersistence, new NotificationServerHttp (NotificationServerHttp.DefaultUri (aHelper.Product)), this);
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

        public void Show ()
        {
            Assert.Check (iNotification != null);
            if (iCurrentWindow != null) {
                iCurrentWindow.Dismiss (false);
            }
            iCurrentWindow = new WindowNotification (iNotificationPersistence, iNotification);
            iCurrentWindow.Show (() => {
                iCurrentWindow.Release ();
                iCurrentWindow = null;
            });
        }
    }
}

