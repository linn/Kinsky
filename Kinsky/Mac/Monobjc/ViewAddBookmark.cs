
using System;
using System.Collections.Generic;

using Monobjc;
using Monobjc.Cocoa;

using Linn;
using Linn.Kinsky;


namespace KinskyDesktop
{
    // File's owner of the ViewAddBookmark.nib file
    [ObjectiveCClass]
    public class ViewAddBookmark : NSViewController, IViewPopover
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewAddBookmark));

        public ViewAddBookmark() : base() {}
        public ViewAddBookmark(IntPtr aInstance) : base(aInstance) {}

        public ViewAddBookmark(BookmarkManager aBookmarkManager, Location aLocation)
            : base()
        {
            iBookmarkManager = aBookmarkManager;
            iBookmark = new Bookmark(aLocation);

            NSBundle.LoadNibNamedOwner("ViewAddBookmark.nib", this);
        }


        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // set appearance of view
            LabelHeader.TextColor = NSColor.WhiteColor;
            LabelHeader.Font = FontManager.FontLarge;

            LabelTitle.TextColor = NSColor.WhiteColor;
            LabelTitle.Font = FontManager.FontMedium;

            LabelLocation.TextColor = NSColor.WhiteColor;
            LabelLocation.Font = FontManager.FontMedium;

            TextFieldTitle.BackgroundColor = NSColor.BlackColor;
            TextFieldTitle.TextColor = NSColor.WhiteColor;
            TextFieldTitle.Font = FontManager.FontMedium;

            TextFieldLocation.TextColor = NSColor.WhiteColor;
            TextFieldLocation.Font = FontManager.FontMedium;

            IButtonHoverType2 button = ButtonAdd.Initialise();
            button.Text = NSString.StringWithUTF8String("Add");

            ButtonAdd.EventClicked += ButtonAddClicked;
            ButtonAdd.KeyEquivalent = NSString.StringWithUTF8String("\r");
            TextFieldTitle.Delegate = this;

            // set the data for the current location
            TextFieldTitle.StringValue = NSString.StringWithUTF8String(iBookmark.Title);

            List<string> titleTrail = new List<string>();
            foreach (Breadcrumb bc in iBookmark.BreadcrumbTrail)
            {
                titleTrail.Add(bc.Title);
            }
            TextFieldLocation.StringValue = NSString.StringWithUTF8String(string.Join("/", titleTrail.ToArray()));
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            View.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("controlTextDidChange:")]
        public void ControlTextDidChange(NSNotification aNotification)
        {
            ButtonAdd.Enabled = (TextFieldTitle.StringValue.Length > 0);
        }

        private void ButtonAddClicked(Id aSender)
        {
            iBookmark.Title = TextFieldTitle.StringValue.ToString();
            iBookmarkManager.Insert(0, iBookmark);

            // close the popover
            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        #region IViewPopover implementation
        // no need to implement View and Release since the base class already implements them
        public event EventHandler<EventArgs> EventClose;
        #endregion IViewPopover implementation


        [ObjectiveCField]
        public ButtonHoverPush ButtonAdd;

        [ObjectiveCField]
        public NSTextField LabelHeader;

        [ObjectiveCField]
        public NSTextField LabelLocation;

        [ObjectiveCField]
        public NSTextField LabelTitle;

        [ObjectiveCField]
        public NSTextField TextFieldLocation;

        [ObjectiveCField]
        public NSTextField TextFieldTitle;

        private Bookmark iBookmark;
        private BookmarkManager iBookmarkManager;
    }
}



