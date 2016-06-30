using System;
using Foundation;
using Linn;
using Linn.Kinsky;
using UIKit;

namespace KinskyTouch
{
	public class NotificationView : INotificationView, IDisposable
	{
		public static NotificationView Instance { get; set; }

		private readonly NotificationController iNotificationController;
		private readonly UIViewController iParent;

		public event EventHandler<EventArgs> EventCurrentChanged;

		private INotification iNotification;

		public NotificationView(INotificationPersistence aPersistence, string aProduct, IInvoker aInvoker, UIViewController aParent)
		{
			iParent = aParent;
			iNotificationController = new NotificationController(aInvoker, aPersistence, new NotificationServerHttp(NotificationServerHttp.DefaultUri(aProduct)), this);
			NotificationView.Instance = this;
		}

		public void Update(INotification aNotification, bool aShowNow)
		{
			iNotification = aNotification;
			if (aShowNow)
			{
				ShowNotification(iNotification);
			}
			var del = EventCurrentChanged;
			if (del != null)
			{
				del(this, EventArgs.Empty);
			}
		}

		public INotification Current
		{
			get
			{
				return iNotification;
			}
		}

		public void ShowCurrent()
		{
			Assert.Check(Current != null);
			ShowNotification(Current);
		}

		public void GetKazoo()
		{
			var appid = 848379604;
			var pvc = new StoreKit.SKStoreProductViewController();
			pvc.Finished += (s, e) =>
			{
				iParent.DismissViewController(true,null);
			};

			pvc.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			iParent.PresentViewController(pvc, true, null);

			pvc.LoadProduct(new StoreKit.StoreProductParameters (appid), (ok, error) =>
			{
				if (!ok)
				{
					UserLog.WriteLine("failed to open kazoo store url: " + error);
				}
			});
		}
			

		private void ShowNotification(INotification aNotification)
		{
			UINavigationController controller = new UINavigationController(new NotificationViewController(aNotification));
			controller.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

			iParent.PresentViewController(controller, true, null);
		}

		public void Dispose()
		{
			iNotificationController.Dispose();
		}
	}
}

