using System;

using UIKit;
using Foundation;

using Linn;
using Linn.Toolkit.Ios;
using CoreGraphics;
using ObjCRuntime;

namespace KinskyTouch
{
    internal class UIBarButtonItemPlay : UIBarButtonItem
    {
        internal enum EInsertMode
        {
            ePlayNow,
            ePlayNext,
            ePlayLater
        }

        internal UIBarButtonItemPlay(OptionEnum aOptionInsertMode, UIBarButtonItemStyle aStyle, EventHandler aHandler)
            : base(string.Empty, aStyle, aHandler)
        {
            iOptionInsertMode = aOptionInsertMode;

            if(aOptionInsertMode.Value == OptionInsertMode.kPlayNow)
            {
                State = EInsertMode.ePlayNow;
            }
            else if(aOptionInsertMode.Value == OptionInsertMode.kPlayNext)
            {
                State = EInsertMode.ePlayNext;
            }
            else if(aOptionInsertMode.Value == OptionInsertMode.kPlayLater)
            {
                State = EInsertMode.ePlayLater;
            }

            Clicked += EventClicked;
        }

        protected override void Dispose(bool aDisposing)
        {
            base.Dispose(aDisposing);

            if(aDisposing)
            {
                Clicked -= EventClicked;
            }
        }

        public EInsertMode State
        {
            get
            {
                return iState;
            }
            set
            {
                switch(value)
                {
                case EInsertMode.ePlayNow:
                    Title = OptionInsertMode.kPlayNow;
                    iOptionInsertMode.Set(OptionInsertMode.kPlayNow);
                    break;
                case EInsertMode.ePlayNext:
                    Title = OptionInsertMode.kPlayNext;
                    iOptionInsertMode.Set(OptionInsertMode.kPlayNext);
                    break;
                case EInsertMode.ePlayLater:
                    Title = OptionInsertMode.kPlayLater;
                    iOptionInsertMode.Set(OptionInsertMode.kPlayLater);
                    break;
                }

                iState = value;
            }
        }

        private void EventClicked(object sender, EventArgs e)
        {
            switch(iState)
            {
            case EInsertMode.ePlayNow:
                State = EInsertMode.ePlayNext;
                break;
            case EInsertMode.ePlayNext:
                State = EInsertMode.ePlayLater;
                break;
            case EInsertMode.ePlayLater:
                State = EInsertMode.ePlayNow;
                break;
            }
        }

        private EInsertMode iState;
        private OptionEnum iOptionInsertMode;
    }

    public partial class BrowserToolbar
    {
        public BrowserToolbar(UITableViewController aTableViewController, ConfigController aConfigController, OptionEnum aOptionInsertMode)
        {
            iTableViewController = aTableViewController;
            iConfigController = aConfigController;

            iButtonConfig = new UIBarButtonItem(new UIImage("Settings.png"), UIBarButtonItemStyle.Bordered, null);
            iButtonConfig.TintColor = UIColor.White;
			//iButtonConfig.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            iButtonConfig.Width = 40.0f;

			iBadgeView = new BadgeView(new CGRect(40, 0, 14, 14));
			iBadgeView.Value = 1;
			iBadgeView.Font = iBadgeView.Font.WithSize(10);
			iBadgeView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0); // transparent
			iBadgeView.FillColor = UIColor.FromRGB(15, 151, 179); // #0F9783
			iBadgeView.StrokeColor = UIColor.White;
			iBadgeView.TextColor = UIColor.White;
			iBadgeView.Hidden = NotificationView.Instance.Current == null || NotificationView.Instance.Current.HasBeenAcknowledged;
			NotificationView.Instance.EventCurrentChanged += (s, e) => 
			{
				iBadgeView.Hidden = NotificationView.Instance.Current == null || NotificationView.Instance.Current.HasBeenAcknowledged;
			};

			iButtonPlay = new UIBarButtonItemPlay(aOptionInsertMode, UIBarButtonItemStyle.Bordered, null);
            iButtonPlay.TintColor = UIColor.White;
            //iButtonPlay.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            iButtonSpacer1 = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            iButtonSpacer1.Width = 20.0f;
            iButtonEdit = new UIBarButtonItem(UIBarButtonSystemItem.Edit);
            iButtonEdit.TintColor = UIColor.White;
            //iButtonEdit.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            iButtonDone = new UIBarButtonItem(UIBarButtonSystemItem.Done);
            iButtonDone.TintColor = UIColor.White;
            //iButtonDone.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            iButtonSpacer2 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            //iButtonSpacer2.Width = 30.0f;

            iButtonConfig.Clicked += ConfigClicked;
            iButtonEdit.Clicked += EditClicked;
            iButtonDone.Clicked += DoneClicked;
			iBadgeView.TouchUpInside += ConfigClicked;

            iTableViewController.SetToolbarItems(new UIBarButtonItem[] { iButtonConfig, iButtonPlay, iButtonSpacer1, iButtonSpacer2 }, false);
			iTableViewController.NavigationController.Toolbar.AddSubview(iBadgeView);
        }

        public bool AllowEditing
        {
            get
            {
                return iAllowEditing;
            }
            set
            {
                if(iAllowEditing != value)
                {
                    if(value)
                    {
                        iTableViewController.SetToolbarItems(new UIBarButtonItem[] { iButtonConfig, iButtonPlay, iButtonSpacer1, iButtonEdit, iButtonSpacer2 }, true);
                    }
                    else
                    {
                        iTableViewController.SetToolbarItems(new UIBarButtonItem[] { iButtonConfig, iButtonPlay, iButtonSpacer1, iButtonSpacer2 }, true);
                    }
                }
        
                iAllowEditing = value;
            }
        }

        public UIBarButtonItem BarButtonItemConfig
        {
            get
            {
                return iButtonConfig;
            }
        }

        public UIBarButtonItem BarButtonItemEdit
        {
            get
            {
                return iButtonEdit;
            }
        }

        public UIBarButtonItem BarButtonItemDone
        {
            get
            {
                return iButtonDone;
            }
        }

        private void SetEditing(bool aEditing)
        {
            if(iAllowEditing)
            {
                if(aEditing)
                {
                    iTableViewController.SetToolbarItems(new UIBarButtonItem[] { iButtonConfig, iButtonPlay, iButtonSpacer1, iButtonDone, iButtonSpacer2 }, true);
                }
                else
                {
                    iTableViewController.SetToolbarItems(new UIBarButtonItem[] { iButtonConfig, iButtonPlay, iButtonSpacer1, iButtonEdit, iButtonSpacer2 }, true);
                }
            }
        }

        private void ConfigClicked(object sender, EventArgs e)
        {
            iConfigController.OpenDialog(iButtonConfig);
        }

        private void EditClicked(object sender, EventArgs e)
        {
            SetEditing(true);
            iTableViewController.SetEditing(true, true);
        }

        private void DoneClicked(object sender, EventArgs e)
        {
            SetEditing(false);
            iTableViewController.SetEditing(false, true);
        }

        private bool iAllowEditing;

        private UITableViewController iTableViewController;
        private ConfigController iConfigController;

        private UIBarButtonItem iButtonConfig;
        private UIBarButtonItem iButtonPlay;
        private UIBarButtonItem iButtonSpacer1;
        private UIBarButtonItem iButtonEdit;
        private UIBarButtonItem iButtonDone;
        private UIBarButtonItem iButtonSpacer2;
		private BadgeView iBadgeView;
    }
}

