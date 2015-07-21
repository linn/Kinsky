using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Linn;

namespace Linn.Toolkit.Ios
{
    public class OptionPageAbout : UIViewController
    {
        public OptionPageAbout(IHelper aHelper, UIImage aImage, UIColor aFontColour)
        {
            iHelper = aHelper;
            iImage = aImage;
            iFontColour = aFontColour;
        }

        public OptionPageAbout(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "About";

            //View.BackgroundColor = UIColor.FromRGBA(215.0f/255.0f,217.0f/255.0f,223.0f/255.0f,1.0f);//UIColor.GroupTableViewBackgroundColor;
            UITableView tableView = new UITableView(View.Bounds, UITableViewStyle.Grouped);
            View.AddSubview(tableView);

            UIImageView imageView = new UIImageView(iImage);
            imageView.Frame = new RectangleF(85, 80, 150, 150);
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            View.AddSubview(imageView);

            UILabel label;

            label = new UILabel(new RectangleF(0, 240, 320, 20));
            label.TextAlignment = UITextAlignment.Center;
            label.Text = iHelper.Product;
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = iFontColour;

            View.AddSubview(label);

            label = new UILabel(new RectangleF(0, 260, 320, 20));
            label.TextAlignment = UITextAlignment.Center;
            label.Text = string.Format("Version {0} ({1})", iHelper.Version, iHelper.Family);
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = iFontColour;

            View.AddSubview(label);

            label = new UILabel(new RectangleF(0, 280, 320, 20));
            label.TextAlignment = UITextAlignment.Center;
            label.Text = string.Format("{0} {1}", iHelper.Copyright, iHelper.Company);
            label.BackgroundColor = UIColor.Clear;
            label.TextColor = iFontColour;

            View.AddSubview(label);

            ContentSizeForViewInPopover = new SizeF(320, 320);
        }

        private IHelper iHelper;
        private UIImage iImage;
        private UIColor iFontColour;
    }

    public class OptionPageAboutIpad : OptionPageAbout
    {
        public OptionPageAboutIpad(IHelper aHelper, UIImage aImage)
            : base(aHelper, aImage, UIColor.Black)
        {
        }

        public OptionPageAboutIpad(IntPtr aInstance)
            : base(aInstance)
        {
        }
    }

    public class OptionPageAboutIphone : OptionPageAbout
    {
        public OptionPageAboutIphone(IHelper aHelper, UIImage aImage)
            : base(aHelper, aImage, UIColor.Black)
        {
        }

        public OptionPageAboutIphone(IntPtr aInstance)
            : base(aInstance)
        {
        }
    }

    public class OptionPageEnum : UITableViewController
    {
        private class DataSource : UITableViewDataSource
        {
            public DataSource(Option aOption)
            {
                iOption = aOption;
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iOption.Allowed.Count;
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                UITableViewCell cell = aTableView.DequeueReusableCell(kCellIdentifier);

                if(cell == null)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Default, kCellIdentifier);
                }

                cell.TextLabel.Text = iOption.Allowed[aIndexPath.Row];
                cell.Accessory = (iOption.Value == iOption.Allowed[aIndexPath.Row]) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

                return cell;
            }

            private Option iOption;
        }

        private class Delegate : UITableViewDelegate
        {
            public Delegate(Option aOption)
            {
                iOption = aOption;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                iOption.Set(iOption.Allowed[aIndexPath.Row]);
            }

            private Option iOption;
        }

        public OptionPageEnum(Option aOption)
            : base(UITableViewStyle.Grouped)
        {
            iOption = aOption;

            iOption.EventAllowedChanged += AllowedChanged;
            iOption.EventValueChanged += ValueChanged;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.DataSource = new DataSource(iOption);
            TableView.Delegate = new Delegate(iOption);

            Title = iOption.Name;
            ContentSizeForViewInPopover = new SizeF(320, 320);
        }

        private void AllowedChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                TableView.ReloadData();
            });
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                TableView.ReloadData();
            });
        }

        private static string kCellIdentifier = "OptionEnumValues";

        private Option iOption;
    }

    public class OptionBoolCell : UITableViewCell
    {
        public OptionBoolCell(UITableViewCellStyle aStyle, string aReuseIdentifier)
            : base(aStyle, aReuseIdentifier)
        {
            iSwitch = new UISwitch();
            iSwitch.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
            iSwitch.ValueChanged += SwitchValueChanged;

            AccessoryView = iSwitch;
        }

        public void SetState(bool aState)
        {
            BeginInvokeOnMainThread(delegate {
                iSwitch.On = aState;
            });
        }

        public void SetOption(OptionBool aOption)
        {
            if(iOption != null)
            {
                iOption.EventValueChanged -= OptionValueChanged;
            }

            iOption = aOption;
            iOption.EventValueChanged += OptionValueChanged;

            SetState(iOption.Native);
        }

        private void SwitchValueChanged(object sender, EventArgs e)
        {
            iOption.Native = iSwitch.On;
        }

        private void OptionValueChanged(object sender, EventArgs e)
        {
            SetState(iOption.Native);
        }

        private OptionBool iOption;
        private UISwitch iSwitch;
    }

    public class OptionPageListUri : UITableViewController
    {
        private class DataSource : UITableViewDataSource
        {
            private class CellEditable : UITableViewCell
            {
                public CellEditable(UITableViewCellStyle aStyle, string aReUseIdentifier)
                    : base(aStyle, aReUseIdentifier)
                {
					RectangleF rect = new RectangleF(
						new PointF((float)ContentView.Bounds.Location.X, (float)ContentView.Bounds.Location.Y),
						new SizeF((float)ContentView.Bounds.Size.Width, (float)ContentView.Bounds.Size.Height));
                    rect.Inflate(-20.0f, -10.0f);
                    iTextField = new UITextField(rect);
                    iTextField.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    iTextField.ClearButtonMode = UITextFieldViewMode.WhileEditing;
                    iTextField.Placeholder = "Server URL";

                    ContentView.AddSubview(iTextField);
                }

                public override void SetSelected(bool aSelected, bool aAnimated)
                {
                }

                public UITextField TextField
                {
                    get
                    {
                        return iTextField;
                    }
                }

                private UITextField iTextField;
            }

            private class Delegate : UITextFieldDelegate
            {
                public Delegate(int aIndex, List<string> aList, Option aOption, UITableViewCell aCell)
                {
                    iIndex = aIndex;
                    iList = aList;
                    iOption = aOption;
                    iCell = aCell;
                }

                public override bool ShouldBeginEditing(UITextField aTextField)
                {
                    iStartText = aTextField.Text;
                    return true;
                }

                public override bool ShouldEndEditing(UITextField aTextField)
                {
                    if(aTextField.Text != iStartText)
                    {
                        if(Update(aTextField))
                        {
                            aTextField.ResignFirstResponder();
                        }
                        else
                        {
                            iCell.BackgroundColor = UIColor.Red;
                        }
                    }

                    return true;
                }

                public override bool ShouldClear(UITextField aTextField)
                {
                    iCell.BackgroundColor = UIColor.White;
                    return true;
                }

                public override bool ShouldReturn(UITextField aTextField)
                {
                    if(aTextField.Text != iStartText)
                    {
                        if(Update(aTextField))
                        {
                            iStartText = aTextField.Text;
                        }
                        else
                        {
                            iCell.BackgroundColor = UIColor.Red;
                        }
                    }

                    aTextField.ResignFirstResponder();

                    return true;
                }

                private bool Update(UITextField aTextField)
                {
                    if(iIndex < iList.Count)
                    {
                        if(string.IsNullOrEmpty(aTextField.Text))
                        {
                            iList.RemoveAt(iIndex);
                        }
                        else
                        {
                            if(Uri.IsWellFormedUriString(aTextField.Text, UriKind.Absolute))
                            {
                                iList[iIndex] = aTextField.Text;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(aTextField.Text))
                        {
                            if(Uri.IsWellFormedUriString(aTextField.Text, UriKind.Absolute))
                            {
                                iList.Add(aTextField.Text);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    iOption.Set(StringListConverter.ListToString(iList));

                    return true;
                }

                private int iIndex;
                private List<string> iList;
                private Option iOption;
                private UITableViewCell iCell;
                private string iStartText;
            }

            public DataSource(Option aOption)
            {
                iOption = aOption;

                iList = new List<string>(StringListConverter.StringToList(iOption.Value));
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iList.Count + 1;
            }

            public override bool CanEditRow(UITableView aTableView, NSIndexPath aIndexPath)
            {
                return (aIndexPath.Row < iList.Count);
            }

            public override void CommitEditingStyle(UITableView aTableView, UITableViewCellEditingStyle aEditingStyle, NSIndexPath aIndexPath)
            {
                if(aEditingStyle == UITableViewCellEditingStyle.Delete)
                {
                    iList.RemoveAt(aIndexPath.Row);

                    aTableView.DeleteRows(new NSIndexPath[] { aIndexPath }, UITableViewRowAnimation.Right);

                    iOption.Set(StringListConverter.ListToString(iList));
                }
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellEditable cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellEditable;

                if(cell == null)
                {
                    cell = new CellEditable(UITableViewCellStyle.Default, kCellIdentifier);
                }

                cell.TextField.Delegate = new Delegate(aIndexPath.Row, iList, iOption, cell);
                if(aIndexPath.Row < iList.Count)
                {
                    cell.TextField.Text = iList[aIndexPath.Row];
                }
                else
                {
                    cell.TextField.Text = string.Empty;
                }

                return cell;
            }

            private Option iOption;
            private List<string> iList;
        }

        public OptionPageListUri(Option aOption)
            : base(UITableViewStyle.Grouped)
        {
            iOption = aOption;

            iOption.EventAllowedChanged += AllowedChanged;
            iOption.EventValueChanged += ValueChanged;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.DataSource = new DataSource(iOption);

            Title = iOption.Name;
            NavigationItem.RightBarButtonItem = EditButtonItem;
            ContentSizeForViewInPopover = new SizeF(320, 320);
        }

        private void AllowedChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                TableView.ReloadData();
            });
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            BeginInvokeOnMainThread(delegate {
                TableView.ReloadData();
            });
        }

        private static string kCellIdentifier = "OptionListValues";

        private Option iOption;
    }
}

