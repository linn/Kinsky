using System;
using Foundation;
using Linn.Kinsky;
using UIKit;

namespace KinskyTouch
{
	public partial class NotificationViewController : UIViewController
	{
		private readonly INotificationPersistence iPersistence;
		private readonly INotification iNotification;
		private readonly bool iInitialChecked;

		private UIBarButtonItem iGetKazooButton;
		private UIBarButtonItem iCloseButton;

		public NotificationViewController(INotificationPersistence aPersistence, INotification aNotification, bool aInitialChecked) 
			: base("NotificationViewController", null)
		{
			iPersistence = aPersistence;
			iNotification = aNotification;
			iInitialChecked = aInitialChecked;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// give the dialog a border on iPad
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				this.View.Layer.BorderColor = UIColor.LightGray.CGColor;
				this.View.Layer.BorderWidth = 3;
				this.View.Layer.CornerRadius = 18;
			}
			iGetKazooButton = new UIBarButtonItem();
			iGetKazooButton.Title = "Get Kazoo";

			iCloseButton = new UIBarButtonItem();
			iCloseButton.Title = "Not Now";

			iGetKazooButton.Clicked += GetKazoo;
			iCloseButton.Clicked += Close;

			Title = "";

			NavigationItem.LeftBarButtonItem = iCloseButton;
			NavigationItem.RightBarButtonItem = iGetKazooButton;
			swDontShowAgain.SetState(iInitialChecked, false);
			webView.LoadRequest(new Foundation.NSUrlRequest(new NSUrl(iNotification.Uri)));
		}


		private void GetKazoo(object sender, EventArgs args)
		{
			Dismiss(true, () =>
			{
				NotificationView.Instance.GetKazoo();
			});
		}

		private void Close(object sender, EventArgs args)
		{
			Dismiss(true, null);
		}

		private void Dismiss(bool aAnimated, Action aCallback)
		{
			if (swDontShowAgain.On)
			{
				iNotification.DontShowAgain();
			}
			else if (iPersistence.LastNotificationVersion == iNotification.Version)
			{
				iPersistence.LastNotificationVersion = 0; // reset the saved version to allow showing the ad again if the user has unchecked this
			}
			DismissViewController(aAnimated, aCallback);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


