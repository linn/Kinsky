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
	[Register ("AppDelegateIphone")]
	partial class AppDelegateIphone
	{
		[Outlet]
		UIKit.UIButton buttonArtwork { get; set; }

		[Outlet]
		UIKit.UIButton buttonCentre { get; set; }

		[Outlet]
		UIKit.UIButton buttonLeft { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem buttonRefresh { get; set; }

		[Outlet]
		UIKit.UIButton buttonRepeat { get; set; }

		[Outlet]
		UIKit.UIButton buttonRight { get; set; }

		[Outlet]
		UIKit.UIButton buttonShuffle { get; set; }

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
		UIKit.UINavigationController navigationController { get; set; }

		[Outlet]
		UIKit.UINavigationController navigationControllerRoomSource { get; set; }

		[Outlet]
		UIKit.UIPageControl pageControl { get; set; }

		[Outlet]
		UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		KinskyTouch.SourceToolbarIphone sourceToolbar { get; set; }

		[Outlet]
		UIKit.UITableView tableViewSource { get; set; }

		[Outlet]
		UIKit.UIView viewBrowser { get; set; }

		[Outlet]
		KinskyTouch.UIViewControllerKinskyTouchIphone viewController { get; set; }

		[Outlet]
		UIKit.UIViewController viewControllerBrowser { get; set; }

		[Outlet]
		KinskyTouch.UIViewControllerNowPlaying viewControllerNowPlaying { get; set; }

		[Outlet]
		KinskyTouch.ViewWidgetSelectorRoom viewControllerRooms { get; set; }

		[Outlet]
		KinskyTouch.ViewWidgetSelectorSource viewControllerSources { get; set; }

		[Outlet]
		KinskyTouch.ViewHourGlassIphone viewHourGlass { get; set; }

		[Outlet]
		KinskyTouch.UIViewInfoIphone viewInfo { get; set; }

		[Outlet]
		UIKit.UIWindow window { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (tableViewSource != null) {
				tableViewSource.Dispose ();
				tableViewSource = null;
			}

			if (navigationController != null) {
				navigationController.Dispose ();
				navigationController = null;
			}

			if (sourceToolbar != null) {
				sourceToolbar.Dispose ();
				sourceToolbar = null;
			}

			if (viewControllerSources != null) {
				viewControllerSources.Dispose ();
				viewControllerSources = null;
			}

			if (helper != null) {
				helper.Dispose ();
				helper = null;
			}

			if (buttonRepeat != null) {
				buttonRepeat.Dispose ();
				buttonRepeat = null;
			}

			if (buttonShuffle != null) {
				buttonShuffle.Dispose ();
				buttonShuffle = null;
			}

			if (viewBrowser != null) {
				viewBrowser.Dispose ();
				viewBrowser = null;
			}

			if (imageViewArtwork != null) {
				imageViewArtwork.Dispose ();
				imageViewArtwork = null;
			}

			if (buttonCentre != null) {
				buttonCentre.Dispose ();
				buttonCentre = null;
			}

			if (buttonLeft != null) {
				buttonLeft.Dispose ();
				buttonLeft = null;
			}

			if (buttonRight != null) {
				buttonRight.Dispose ();
				buttonRight = null;
			}

			if (controlRotaryTime != null) {
				controlRotaryTime.Dispose ();
				controlRotaryTime = null;
			}

			if (controlRotaryVolume != null) {
				controlRotaryVolume.Dispose ();
				controlRotaryVolume = null;
			}

			if (viewHourGlass != null) {
				viewHourGlass.Dispose ();
				viewHourGlass = null;
			}

			if (viewInfo != null) {
				viewInfo.Dispose ();
				viewInfo = null;
			}

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}

			if (viewControllerRooms != null) {
				viewControllerRooms.Dispose ();
				viewControllerRooms = null;
			}

			if (viewControllerBrowser != null) {
				viewControllerBrowser.Dispose ();
				viewControllerBrowser = null;
			}

			if (navigationControllerRoomSource != null) {
				navigationControllerRoomSource.Dispose ();
				navigationControllerRoomSource = null;
			}

			if (controlTime != null) {
				controlTime.Dispose ();
				controlTime = null;
			}

			if (controlVolume != null) {
				controlVolume.Dispose ();
				controlVolume = null;
			}

			if (viewControllerNowPlaying != null) {
				viewControllerNowPlaying.Dispose ();
				viewControllerNowPlaying = null;
			}

			if (viewController != null) {
				viewController.Dispose ();
				viewController = null;
			}

			if (buttonArtwork != null) {
				buttonArtwork.Dispose ();
				buttonArtwork = null;
			}

			if (buttonRefresh != null) {
				buttonRefresh.Dispose ();
				buttonRefresh = null;
			}

			if (imageViewPlaylistAux != null) {
				imageViewPlaylistAux.Dispose ();
				imageViewPlaylistAux = null;
			}
		}
	}

	[Register ("ViewWidgetBrowserRootIphone")]
	partial class ViewWidgetBrowserRootIphone
	{
		[Outlet]
		UIKit.UIButton buttonHome { get; set; }

		[Outlet]
		UIKit.UIButton buttonRetry { get; set; }

		[Outlet]
		UIKit.UIView viewError { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonHome != null) {
				buttonHome.Dispose ();
				buttonHome = null;
			}

			if (buttonRetry != null) {
				buttonRetry.Dispose ();
				buttonRetry = null;
			}

			if (viewError != null) {
				viewError.Dispose ();
				viewError = null;
			}
		}
	}

	[Register ("UIViewControllerNowPlaying")]
	partial class UIViewControllerNowPlaying
	{
		[Outlet]
		UIKit.UIButton buttonArtwork { get; set; }

		[Outlet]
		UIKit.UIButton buttonList { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewArtwork { get; set; }

		[Outlet]
		UIKit.UINavigationBar navigationBar { get; set; }

		[Outlet]
		UIKit.UIView viewPlaylist { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imageViewArtwork != null) {
				imageViewArtwork.Dispose ();
				imageViewArtwork = null;
			}

			if (viewPlaylist != null) {
				viewPlaylist.Dispose ();
				viewPlaylist = null;
			}

			if (buttonArtwork != null) {
				buttonArtwork.Dispose ();
				buttonArtwork = null;
			}

			if (buttonList != null) {
				buttonList.Dispose ();
				buttonList = null;
			}

			if (navigationBar != null) {
				navigationBar.Dispose ();
				navigationBar = null;
			}
		}
	}

	[Register ("UIViewInfoIphone")]
	partial class UIViewInfoIphone
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("SourceToolbarIphone")]
	partial class SourceToolbarIphone
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("UIViewControllerKinskyTouchIphone")]
	partial class UIViewControllerKinskyTouchIphone
	{
		[Outlet]
		UIKit.UINavigationController navigationControllerBrowser { get; set; }

		[Outlet]
		UIKit.UINavigationController navigationControllerRoomSource { get; set; }

		[Outlet]
		UIKit.UIPageControl pageControl { get; set; }

		[Outlet]
		UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		UIKit.UIViewController viewControllerBrowser { get; set; }

		[Outlet]
		KinskyTouch.UIViewControllerNowPlaying viewControllerNowPlaying { get; set; }

		[Outlet]
		UIKit.UINavigationController viewControllerRoomSource { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewControllerBrowser != null) {
				viewControllerBrowser.Dispose ();
				viewControllerBrowser = null;
			}

			if (viewControllerNowPlaying != null) {
				viewControllerNowPlaying.Dispose ();
				viewControllerNowPlaying = null;
			}

			if (viewControllerRoomSource != null) {
				viewControllerRoomSource.Dispose ();
				viewControllerRoomSource = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (navigationControllerBrowser != null) {
				navigationControllerBrowser.Dispose ();
				navigationControllerBrowser = null;
			}

			if (navigationControllerRoomSource != null) {
				navigationControllerRoomSource.Dispose ();
				navigationControllerRoomSource = null;
			}
		}
	}

	[Register ("ViewHourGlassIphone")]
	partial class ViewHourGlassIphone
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
