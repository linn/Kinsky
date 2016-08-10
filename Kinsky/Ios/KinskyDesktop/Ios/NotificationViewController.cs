using System;
using Foundation;
using Linn.Kinsky;
using UIKit;

namespace KinskyTouch
{
	public partial class NotificationViewController : UIViewController
	{
		private readonly INotification iNotification;

		private UIBarButtonItem iGetKazooButton;
		private UIBarButtonItem iCloseButton;

		public NotificationViewController(INotification aNotification) 
			: base("NotificationViewController", null)
		{
			iNotification = aNotification;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// give the dialog a border on iPad
			//if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			//{
			//	this.View.Layer.BorderColor = UIColor.LightGray.CGColor;
			//	this.View.Layer.BorderWidth = 3;
			//	this.View.Layer.CornerRadius = 18;
			//}
			iGetKazooButton = new UIBarButtonItem();
			iGetKazooButton.Title = "Try it now";

			iCloseButton = new UIBarButtonItem();
			iCloseButton.Title = "Not Now";

			iGetKazooButton.Clicked += GetKazoo;
			iCloseButton.Clicked += Close;

			Title = "";

			NavigationItem.LeftBarButtonItem = iCloseButton;
			NavigationItem.RightBarButtonItem = iGetKazooButton;
			webView.LoadRequest(new Foundation.NSUrlRequest(new NSUrl(iNotification.Uri(true))));
			iNotification.Shown();
		}


		private void GetKazoo(object sender, EventArgs args)
		{
            iNotification.TrackUsageEventDismissed(true);
			Dismiss(true, true, () =>
			{
				NotificationView.Instance.GetKazoo();
			});
		}

		private void Close(object sender, EventArgs args)
		{
            iNotification.TrackUsageEventDismissed(false);
			Dismiss(true, false, null);
		}

		private void Dismiss(bool aAnimated, bool aAcknowledged, Action aCallback)
		{
			iNotification.Closed(aAcknowledged);
			DismissViewController(aAnimated, aCallback);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


