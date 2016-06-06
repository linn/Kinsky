using System;
using System.Collections.Generic;

using UIKit;

namespace KinskyTouch
{
    partial class SourceToolbar : UIToolbar
    {
        public SourceToolbar(IntPtr aInstance)
            : base(aInstance)
        {
            // spacer defined in subclass

            iButtonRepeat = new UIBarButtonItem();
            iButtonShuffle = new UIBarButtonItem();

            // spacer defined in subclass

            iButtonEdit = new UIBarButtonItem(UIBarButtonSystemItem.Edit);
            iButtonEdit.TintColor = UIColor.White;
            //iButtonEdit.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            //iButtonEdit.Width = 30.0f;
            iButtonDone = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, null);
            iButtonDone.TintColor = UIColor.White;
            //iButtonDone.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            iButtonDelete = new UIBarButtonItem(UIBarButtonSystemItem.Trash);
            iButtonDelete.TintColor = UIColor.White;
            //iButtonDelete.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);
            iButtonDelete.Style = UIBarButtonItemStyle.Bordered;
            //iButtonDelete.Width = 30.0f;
            iButtonSave = new UIBarButtonItem("Save", UIBarButtonItemStyle.Bordered, null);
            iButtonSave.TintColor = UIColor.White;
            //iButtonSave.SetBackgroundImage(new UIImage("Button.png"), UIControlState.Normal, UIBarMetrics.Default);

            iButtonEdit.Clicked += EditClicked;
            iButtonDone.Clicked += DoneClicked;
        }

        public void Initialise(UIButton aButtonShuffle, UIButton aButtonRepeat)
        {
            iButtonRepeat.CustomView = aButtonRepeat;
            iButtonShuffle.CustomView = aButtonShuffle;
        }

        public void SetAllowEditing(bool aAllowEditing)
        {
            if(iAllowEditing != aAllowEditing)
            {
                iAllowEditing = aAllowEditing;
                SetItems();
            }
        }

        public void SetAllowSaving(bool aAllowSaving)
        {
            if(iAllowSaving != aAllowSaving)
            {
                iAllowSaving = aAllowSaving;
                SetItems();
            }
        }

        public void SetAllowPlayMode(bool aAllowPlayMode)
        {
            if(iAllowPlayMode != aAllowPlayMode)
            {
                iAllowPlayMode = aAllowPlayMode;
                SetItems();
            }
        }

        public UIBarButtonItem BarButtonItemRoom
        {
            get
            {
                return iButtonRepeat;
            }
        }

        public UIBarButtonItem BarButtonItemSource
        {
            get
            {
                return iButtonShuffle;
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

        public UIBarButtonItem BarButtonItemDelete
        {
            get
            {
                return iButtonDelete;
            }
        }

        public UIBarButtonItem BarButtonItemSave
        {
            get
            {
                return iButtonSave;
            }
        }

        private void SetItems()
        {
            List<UIBarButtonItem> list = new List<UIBarButtonItem>();

            list.Add(iButtonSpacer1);

            if(iAllowPlayMode)
            {
                list.Add(iButtonRepeat);
                list.Add(iButtonShuffle);
            }

            if(iAllowEditing || iAllowSaving)
            {
                list.Add(iButtonSpacer2);
                iButtonSpacer2.Width = 20.0f;
            }

            if(iAllowEditing)
            {
                if(iEditing)
                {
                    list.Add(iButtonDone);
                    list.Add(iButtonDelete);

                    iButtonSpacer2.Width += 6.0f;
                }
                else
                {
                    list.Add(iButtonEdit);
                }
            }

            if(iAllowSaving)
            {
                if(!iEditing)
                {
                    list.Add(iButtonSave);
                }
            }

            SetItems(list.ToArray(), true);
        }

        public void SetEditing(bool aEditing)
        {
            if(iAllowEditing)
            {
                if(iEditing != aEditing)
                {
                    iEditing = aEditing;
                    SetItems();
                }
            }
        }

        private void EditClicked(object sender, EventArgs e)
        {
            SetEditing(true);
        }

        private void DoneClicked(object sender, EventArgs e)
        {
            SetEditing(false);
        }

        private bool iAllowEditing;
        private bool iAllowSaving;
        private bool iAllowPlayMode;
        private bool iEditing;

        protected UIBarButtonItem iButtonSpacer1;
        private UIBarButtonItem iButtonRepeat;
        private UIBarButtonItem iButtonShuffle;
        protected UIBarButtonItem iButtonSpacer2;
        private UIBarButtonItem iButtonEdit;
        private UIBarButtonItem iButtonDone;
        private UIBarButtonItem iButtonDelete;
        private UIBarButtonItem iButtonSave;
    }

    partial class SourceToolbarIpad : SourceToolbar
    {
        public SourceToolbarIpad(IntPtr aInstance)
            : base(aInstance)
        {
            iButtonSpacer1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            iButtonSpacer2 = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            iButtonSpacer2.Width = 20.0f;
        }
    }

    partial class SourceToolbarIphone : SourceToolbar
    {
        public SourceToolbarIphone(IntPtr aInstance)
            : base(aInstance)
        {
            iButtonSpacer1 = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
            //iButtonSpacer1.Width = 30.0f;
            iButtonSpacer2 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
        }
    }
}

