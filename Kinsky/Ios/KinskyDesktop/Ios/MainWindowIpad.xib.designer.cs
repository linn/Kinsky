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
	[Register ("AppDelegateIpad")]
	partial class AppDelegateIpad
	{
		[Outlet]
		UIKit.UIButton buttonCentre { get; set; }

		[Outlet]
		UIKit.UIButton buttonLeft { get; set; }

		[Outlet]
		UIKit.UIButton buttonRepeat { get; set; }

		[Outlet]
		UIKit.UIButton buttonRight { get; set; }

		[Outlet]
		UIKit.UIButton buttonShuffle { get; set; }

		[Outlet]
		UIKit.UIButton buttonViewInfo { get; set; }

		[Outlet]
		KinskyTouch.UIControlWheel controlRotaryTime { get; set; }

		[Outlet]
		KinskyTouch.UIControlWheel controlRotaryVolume { get; set; }

		[Outlet]
		UIKit.UIControl controlTime { get; set; }

		[Outlet]
		UIKit.UIControl controlVolume { get; set; }

		[Outlet]
		KinskyTouch.HelperKinskyTouch helper { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewArtwork { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewPlaylistAux { get; set; }

		[Outlet]
		UIKit.UILabel labelRoom { get; set; }

		[Outlet]
		UIKit.UILabel labelSource { get; set; }

		[Outlet]
		UIKit.UINavigationController navigationController { get; set; }

		[Outlet]
		UIKit.UINavigationItem navigationItemSource { get; set; }

		[Outlet]
		KinskyTouch.SourceToolbarIpad sourceToolbar { get; set; }

		[Outlet]
		UIKit.UITableView tableViewSource { get; set; }

		[Outlet]
		UIKit.UIView viewBrowser { get; set; }

		[Outlet]
		KinskyTouch.UIViewControllerKinskyTouchIpad viewController { get; set; }

		[Outlet]
		KinskyTouch.ViewHourGlassIpad viewHourGlass { get; set; }

		[Outlet]
		KinskyTouch.UIViewInfoIpad viewInfo { get; set; }

		[Outlet]
		KinskyTouch.UIViewInfoIpad viewOverlayInfo { get; set; }

		[Outlet]
		UIKit.UIWindow window { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (viewController != null) {
				viewController.Dispose ();
				viewController = null;
			}

			if (buttonLeft != null) {
				buttonLeft.Dispose ();
				buttonLeft = null;
			}

			if (buttonRight != null) {
				buttonRight.Dispose ();
				buttonRight = null;
			}

			if (buttonCentre != null) {
				buttonCentre.Dispose ();
				buttonCentre = null;
			}

			if (buttonShuffle != null) {
				buttonShuffle.Dispose ();
				buttonShuffle = null;
			}

			if (buttonRepeat != null) {
				buttonRepeat.Dispose ();
				buttonRepeat = null;
			}

			if (navigationController != null) {
				navigationController.Dispose ();
				navigationController = null;
			}

			if (controlRotaryVolume != null) {
				controlRotaryVolume.Dispose ();
				controlRotaryVolume = null;
			}

			if (controlRotaryTime != null) {
				controlRotaryTime.Dispose ();
				controlRotaryTime = null;
			}

			if (tableViewSource != null) {
				tableViewSource.Dispose ();
				tableViewSource = null;
			}

			if (navigationItemSource != null) {
				navigationItemSource.Dispose ();
				navigationItemSource = null;
			}

			if (labelRoom != null) {
				labelRoom.Dispose ();
				labelRoom = null;
			}

			if (labelSource != null) {
				labelSource.Dispose ();
				labelSource = null;
			}

			if (imageViewArtwork != null) {
				imageViewArtwork.Dispose ();
				imageViewArtwork = null;
			}

			if (imageViewPlaylistAux != null) {
				imageViewPlaylistAux.Dispose ();
				imageViewPlaylistAux = null;
			}

			if (helper != null) {
				helper.Dispose ();
				helper = null;
			}

			if (sourceToolbar != null) {
				sourceToolbar.Dispose ();
				sourceToolbar = null;
			}

			if (viewBrowser != null) {
				viewBrowser.Dispose ();
				viewBrowser = null;
			}

			if (viewHourGlass != null) {
				viewHourGlass.Dispose ();
				viewHourGlass = null;
			}

			if (viewInfo != null) {
				viewInfo.Dispose ();
				viewInfo = null;
			}

			if (viewOverlayInfo != null) {
				viewOverlayInfo.Dispose ();
				viewOverlayInfo = null;
			}

			if (controlTime != null) {
				controlTime.Dispose ();
				controlTime = null;
			}

			if (controlVolume != null) {
				controlVolume.Dispose ();
				controlVolume = null;
			}

			if (buttonViewInfo != null) {
				buttonViewInfo.Dispose ();
				buttonViewInfo = null;
			}
		}
	}

	[Register ("UIViewControllerKinskyTouchIpad")]
	partial class UIViewControllerKinskyTouchIpad
	{
		[Outlet]
		KinskyTouch.HelperKinskyTouch helper { get; set; }

		[Outlet]
		UIKit.UINavigationController navigationController { get; set; }

		[Outlet]
		UIKit.UIView viewArtwork { get; set; }

		[Outlet]
		UIKit.UIView viewBrowserPlaylist { get; set; }

		[Outlet]
		UIKit.UIView viewInfo { get; set; }

		[Outlet]
		KinskyTouch.UIViewInfoIpad viewOverlay { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewBrowserPlaylist != null) {
				viewBrowserPlaylist.Dispose ();
				viewBrowserPlaylist = null;
			}

			if (navigationController != null) {
				navigationController.Dispose ();
				navigationController = null;
			}

			if (viewArtwork != null) {
				viewArtwork.Dispose ();
				viewArtwork = null;
			}

			if (viewInfo != null) {
				viewInfo.Dispose ();
				viewInfo = null;
			}

			if (helper != null) {
				helper.Dispose ();
				helper = null;
			}

			if (viewOverlay != null) {
				viewOverlay.Dispose ();
				viewOverlay = null;
			}
		}
	}

	[Register ("ViewWidgetBrowserRootIpad")]
	partial class ViewWidgetBrowserRootIpad
	{
		[Outlet]
		UIKit.UIButton buttonHome { get; set; }

		[Outlet]
		UIKit.UIButton buttonRetry { get; set; }

		[Outlet]
		UIKit.UIView viewError { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewError != null) {
				viewError.Dispose ();
				viewError = null;
			}

			if (buttonHome != null) {
				buttonHome.Dispose ();
				buttonHome = null;
			}

			if (buttonRetry != null) {
				buttonRetry.Dispose ();
				buttonRetry = null;
			}
		}
	}

	[Register ("SourceToolbarIpad")]
	partial class SourceToolbarIpad
	{
		[Outlet]
		UIKit.UIButton buttonRepeat { get; set; }

		[Outlet]
		UIKit.UIButton buttonShuffle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonRepeat != null) {
				buttonRepeat.Dispose ();
				buttonRepeat = null;
			}

			if (buttonShuffle != null) {
				buttonShuffle.Dispose ();
				buttonShuffle = null;
			}
		}
	}

	[Register ("UIViewInfoIpad")]
	partial class UIViewInfoIpad
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("ViewHourGlassIpad")]
	partial class ViewHourGlassIpad
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
