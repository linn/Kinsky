// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace KinskyTouch
{
	[Register ("NotificationViewController")]
	partial class NotificationViewController
	{
		[Outlet]
		UIKit.UISwitch swDontShowAgain { get; set; }

		[Outlet]
		UIKit.UIWebView webView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}

			if (swDontShowAgain != null) {
				swDontShowAgain.Dispose ();
				swDontShowAgain = null;
			}
		}
	}
}
