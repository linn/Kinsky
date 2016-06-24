using System;

using UIKit;

using Linn;
using Linn.Toolkit.Ios;

namespace KinskyTouch
{
    public class ConfigControllerIpad : ConfigController
    {
        public ConfigControllerIpad(Helper aHelper)
            : base(aHelper)
        {
        }

        public override void OpenDialog(UIBarButtonItem aBarButtonItem)
        {
            if(iOptionDialog == null)
            {
                iOptionDialog = new OptionDialogIpad(iHelper, kManualUri, KinskyTouch.Properties.ResourceManager.Icon);
            }

			var getKazooButton = new UIBarButtonItem("Try Linn's latest control app", UIBarButtonItemStyle.Bordered, new EventHandler((s, e) =>
			{
				iOptionDialog.NavigationController.DismissViewController(true, null);
				NotificationView.Instance.ShowCurrent();
			}));

            iOptionDialog.Open(aBarButtonItem);
			iOptionDialog.NavigationController.ToolbarHidden = false;
			iOptionDialog.NavigationController.TopViewController.SetToolbarItems(new UIBarButtonItem[] { getKazooButton }, false);
        }

        public static string kManualUri = "http://oss.linn.co.uk/trac/wiki/KinskyIpadDavaarManual";

        private OptionDialogIpad iOptionDialog;
    }

    public class ConfigControllerIphone : ConfigController
    {
        public ConfigControllerIphone(UIViewController aViewController, Helper aHelper)
            : base(aHelper)
        {
            iViewController = aViewController;
        }

        public override void OpenDialog(UIBarButtonItem aBarButtonItem)
        {
            if(iOptionDialog == null)
            {
                iOptionDialog = new OptionDialogIphone(iViewController, iHelper, kManualUri, KinskyTouch.Properties.ResourceManager.Icon);
            }

			var getKazooButton = new UIBarButtonItem("Try Linn's latest control app", UIBarButtonItemStyle.Bordered, new EventHandler((s, e) =>
			{
				iOptionDialog.NavigationController.DismissViewController(true, null);
				NotificationView.Instance.ShowCurrent();
			}));
            iOptionDialog.Open();
			iOptionDialog.NavigationController.ToolbarHidden = false;
			iOptionDialog.NavigationController.TopViewController.SetToolbarItems(new UIBarButtonItem[] { getKazooButton }, false);
		}

        public static string kManualUri = "http://oss.linn.co.uk/trac/wiki/KinskyIphoneDavaarManual";

        private UIViewController iViewController;
        private OptionDialogIphone iOptionDialog;
    }

    public abstract class ConfigController
    {
        public ConfigController(Helper aHelper)
        {
            iHelper = aHelper;
        }

        public abstract void OpenDialog(UIBarButtonItem aBarButtonItem);

        protected Helper iHelper;
    }
}

