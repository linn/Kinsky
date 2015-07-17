using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Linn.Toolkit.Ios
{
    public class OptionDialogIpad
    {
        public OptionDialogIpad(IHelper aHelper, string aManualUri, UIImage aImageIcon)
        {
            iNavigationController = new UINavigationController(new OptionDialogRoot(aHelper, aManualUri, aImageIcon, new OptionPageAboutIpad(aHelper, aImageIcon)));
            iPopOverController = new UIPopoverController(iNavigationController);
        }

        public void Open(UIButton aButton)
        {
            if(!iPopOverController.PopoverVisible)
            {
                iNavigationController.PopToRootViewController(false);
                iPopOverController.SetPopoverContentSize(new SizeF(320, 670), true);
                iPopOverController.PresentFromRect(aButton.Bounds, aButton, UIPopoverArrowDirection.Any, true);
            }
            else
            {
                iPopOverController.Dismiss(true);
            }
        }

        public void Open(UIBarButtonItem aButton)
        {
            if(!iPopOverController.PopoverVisible)
            {
                iNavigationController.PopToRootViewController(false);
                iPopOverController.SetPopoverContentSize(new SizeF(320, 670), true);
                iPopOverController.PresentFromBarButtonItem(aButton, UIPopoverArrowDirection.Any, true);
            }
            else
            {
                iPopOverController.Dismiss(true);
            }
        }

        public void DidRotate(UIButton aButton)
        {
            if(iPopOverController.PopoverVisible)
            {
                iPopOverController.Dismiss(true);
                Open(aButton);
            }
        }

        public void DidRotate(UIBarButtonItem aButton)
        {
        }

        private UIPopoverController iPopOverController;
        private UINavigationController iNavigationController;
    }

    public class OptionDialogIphone
    {
        public OptionDialogIphone(UIViewController aViewController, IHelper aHelper, string aManualUri, UIImage aImageIcon)
        {
            iViewController = aViewController;
            
            iNavigationController = new UINavigationController(new OptionDialogRoot(aHelper, aManualUri, aImageIcon, new OptionPageAboutIphone(aHelper, aImageIcon)));
            iNavigationController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            iNavigationController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
			iNavigationController.TopViewController.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { iViewController.DismissViewController(true, () => { }); });
			// iOS5 and above only
            //iNavigationController.TopViewController.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { iViewController.DismissViewController(true, () => {}); });
        }

        public void Open()
        {
			iViewController.PresentModalViewController(iNavigationController, true);
            // iOS5 and above only
			//iViewController.PresentViewController(iNavigationController, true, () => {});
        }

        internal void Close()
        {
			iViewController.DismissViewController(true, () => { });
			// iOS5 and above only
            //iViewController.DismissViewController(true, () => {});
        }

        private UIViewController iViewController;
        private UINavigationController iNavigationController;
    }

    internal class OptionDialogRoot : UITableViewController
    {
        private class DataSource : UITableViewDataSource
        {
            public DataSource(IHelper aHelper, IList<IOptionPage> aOptionPages)
            {
                iHelper = aHelper;
                iOptionPages = aOptionPages;
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                if(aSection == 0)
                {
                    return 1;
                }
                else if(aSection < iOptionPages.Count + 1)
                {
					return iOptionPages[(int)(aSection - 1)].Options.Count;
                }

                return 1;
            }

            public override nint NumberOfSections(UITableView aTableView)
            {
                return iOptionPages.Count + 2;
            }

            public override string TitleForHeader(UITableView aTableView, nint aSection)
            {
                if(aSection == 0)
                {
                    return null;
                }
                else if(aSection < iOptionPages.Count + 1)
                {
					return iOptionPages[(int)(aSection - 1)].Name;
                }

                return "Links";
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                UITableViewCell cell = null;

                if(aIndexPath.Section > 0 && aIndexPath.Section < iOptionPages.Count + 1)
                {
                    Option option = iOptionPages[aIndexPath.Section - 1].Options[aIndexPath.Row];

                    if (option is OptionEnum || option is OptionNetworkInterface)
                    {
                        cell = aTableView.DequeueReusableCell(kOptionEnumCellIdentifier);

                        if(cell == null)
                        {
                            cell = new UITableViewCell(UITableViewCellStyle.Value1, kOptionEnumCellIdentifier);
                        }

                        cell.TextLabel.Text = option.Name;
                        cell.DetailTextLabel.Text = option.Value;
                        cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }
                    else if(option is OptionListUri)
                    {
                        cell = aTableView.DequeueReusableCell(kOptionEnumCellIdentifier);

                        if(cell == null)
                        {
                            cell = new UITableViewCell(UITableViewCellStyle.Value1, kOptionEnumCellIdentifier);
                        }

                        cell.TextLabel.Text = option.Name;
                        cell.DetailTextLabel.Text = (option as OptionListUri).Native.Count.ToString();
                        cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }
                    else if (option is OptionFilePath)
                    {
                        throw new NotImplementedException();
                        /*cell = aTableView.DequeueReusableCell(kCellIdentifier);
                        if(cell == null)
                        {
                            cell = new UITableViewCell(UITableViewCellStyle.Default, kCellIdentifier);
                        }
                        cell.TextLabel.Text = option.Name;*/
                    }
                    else if (option is OptionFolderPath)
                    {
                        throw new NotImplementedException();
                        /*cell = aTableView.DequeueReusableCell(kCellIdentifier);
                        if(cell == null)
                        {
                            cell = new UITableViewCell(UITableViewCellStyle.Default, kCellIdentifier);
                        }
                        cell.TextLabel.Text = option.Name;*/
                    }
                    else if (option is OptionBool)
                    {
                        OptionBoolCell boolCell = aTableView.DequeueReusableCell(kOptionBoolCellIdentifier) as OptionBoolCell;
    
                        if(boolCell == null)
                        {
                            boolCell = new OptionBoolCell(UITableViewCellStyle.Default, kOptionBoolCellIdentifier);
                        }
    
                        boolCell.TextLabel.Text = option.Name;
                        boolCell.SetOption(option as OptionBool);

                        cell = boolCell;
                    }
                    else if (option is OptionListFolderPath)
                    {
                        throw new NotImplementedException();
                        /*cell = aTableView.DequeueReusableCell(kCellIdentifier);
                        if(cell == null)
                        {
                            cell = new UITableViewCell(UITableViewCellStyle.Default, kCellIdentifier);
                        }
                        cell.TextLabel.Text = option.Name;*/
                    }
                }
                else
                {
                    cell = aTableView.DequeueReusableCell(kAboutCellIdentifier);

                    if(cell == null)
                    {
                        cell = new UITableViewCell(UITableViewCellStyle.Value1, kAboutCellIdentifier);
                    }

                    if(aIndexPath.Section == 0)
                    {
                        /*if(aIndexPath.Row == 0)
                        {
                            cell.TextLabel.Text = "Version";
                            cell.DetailTextLabel.Text = iHelper.Version;
                        }
                        else
                        {
                            cell.TextLabel.Text = "Family";
                            cell.DetailTextLabel.Text = iHelper.Family;
                        }*/
                        cell.TextLabel.Text = "About";
                        cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }
                    else
                    {
                        cell.TextLabel.Text = "View manual";
                    }
                }

                return cell;
            }

            private IHelper iHelper;
            private IList<IOptionPage> iOptionPages;
        }

        private class Delegate : UITableViewDelegate
        {
            private class AlertViewDelegate : UIAlertViewDelegate
            {
                public AlertViewDelegate(string aManualUri)
                {
                    iManualUri = aManualUri;
                }

                public override void Canceled(UIAlertView aAlertView)
                {
                }

                public override void Clicked(UIAlertView aAlertview, nint aButtonIndex)
                {
                    if(aButtonIndex != aAlertview.CancelButtonIndex)
                    {
                        NSUrl url = new NSUrl(iManualUri);
                        UIApplication.SharedApplication.OpenUrl(url);
                    }
                }

                private string iManualUri;
            }

            public Delegate(UITableViewController aController, IHelper aHelper, IList<IOptionPage> aOptionPages, string aManualUri, UIImage aImageIcon, OptionPageAbout aOptionPageAbout)
            {
                iController = aController;
                iHelper = aHelper;
                iOptionPages = aOptionPages;
                iImageIcon = aImageIcon;
                iOptionPageAbout = aOptionPageAbout;

                iAlertViewDelegate = new AlertViewDelegate(aManualUri);
            }

            public override NSIndexPath WillSelectRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                if(aIndexPath.Section > 0 && aIndexPath.Section < iOptionPages.Count + 1)
                {
                    Option option = iOptionPages[aIndexPath.Section - 1].Options[aIndexPath.Row];
    
                    if (option is OptionBool)
                    {
                        return null;
                    }
    
                    return aIndexPath;
                }

                //if(aIndexPath.Section > 0)
                //{
                    return aIndexPath;
                //}

                //return null;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                if(aIndexPath.Section > 0 && aIndexPath.Section < iOptionPages.Count + 1)
                {
                    Option option = iOptionPages[aIndexPath.Section - 1].Options[aIndexPath.Row];
    
                    if (option is OptionEnum || option is OptionNetworkInterface)
                    {
                        //iController.Title = "Back";
                        iController.NavigationController.PushViewController(new OptionPageEnum(option), true);
                    }
                    else if (option is OptionListUri)
                    {
                        iController.NavigationController.PushViewController(new OptionPageListUri(option), true);
                    }
                    else if (option is OptionFilePath)
                    {
                        throw new NotImplementedException();
                    }
                    else if (option is OptionFolderPath)
                    {
                        throw new NotImplementedException();
                    }
                    else if (option is OptionListFolderPath)
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    if(aIndexPath.Section == 0)
                    {
                        //iController.Title = "Back";
                        iController.NavigationController.PushViewController(iOptionPageAbout, true);
                    }
                    else
                    {
                        UIAlertView alert = new UIAlertView("This will open Safari", "Do you want to continue?", iAlertViewDelegate, "No", new string[] { "Yes" });
                        alert.Show();
                    }
                }
            }

            private UITableViewController iController;
            private IHelper iHelper;
            private IList<IOptionPage> iOptionPages;
            private AlertViewDelegate iAlertViewDelegate;
            private UIImage iImageIcon;
            private OptionPageAbout iOptionPageAbout;
        }

        public OptionDialogRoot(IHelper aHelper, string aManualUri, UIImage aImageIcon, OptionPageAbout aOptionPageAbout)
            : base(UITableViewStyle.Grouped)
        {
            iHelper = aHelper;
            iManualUri = aManualUri;
            iImageIcon = aImageIcon;
            iOptionPageAbout = aOptionPageAbout;

            foreach(IOptionPage p in iHelper.OptionPages)
            {
                foreach(Option o in p.Options)
                {
                    o.EventAllowedChanged += AllowedChanged;
                    o.EventValueChanged += ValueChanged;
                }

                p.EventChanged += PageChanged;
            }
        }

        public override void ViewWillAppear(bool aAnimated)
        {
            base.ViewWillAppear(aAnimated);

            Title = "Settings";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UpdateTableView();

            ContentSizeForViewInPopover = new SizeF(320, 320);
        }

        private void AllowedChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                UpdateTableView();
                //TableView.ReloadData();
            });
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                UpdateTableView();
                //TableView.ReloadData();
            });
        }

        private void PageChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                UpdateTableView();
                //TableView.ReloadData();
            });
        }

        private void UpdateTableView()
        {
            IList<IOptionPage> optionPages = CreateOptionPageList(iHelper.OptionPages);
            TableView.DataSource = new DataSource(iHelper, optionPages);
            TableView.Delegate = new Delegate(this, iHelper, optionPages, iManualUri, iImageIcon, iOptionPageAbout);
        }

        private IList<IOptionPage> CreateOptionPageList(IList<IOptionPage> aOptionPages)
        {
            IList<IOptionPage> optionPages = new List<IOptionPage>();

            foreach(IOptionPage p in aOptionPages)
            {
                bool ignorePage = false;

                foreach(Option o in p.Options)
                {
                    if(o.Name == OptionNetworkInterface.kName && o.Allowed.Count < 3)
                    {
                        ignorePage = true;
                    }
                }

                if(!ignorePage)
                {
                    optionPages.Add(p);
                }
            }

            return optionPages;
        }

        private static string kAboutCellIdentifier = "AboutCell";
        private static string kOptionEnumCellIdentifier = "OptionEnumCell";
        private static string kOptionBoolCellIdentifier = "OptionBoolCell";

        private string iManualUri;
        private IHelper iHelper;
        private UIImage iImageIcon;
        private OptionPageAbout iOptionPageAbout;
    }
}

