
using System;
using System.Collections.Generic;

using Monobjc;
using Monobjc.Cocoa;

using Linn;
using Linn.Kinsky;


namespace KinskyDesktop
{
    // File's owner of the ViewSelectionBookmark.nib file
    [ObjectiveCClass]
    public class ViewSelectionBookmark : NSViewController, IViewPopover
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewSelectionBookmark));

        public ViewSelectionBookmark() : base() {}
        public ViewSelectionBookmark(IntPtr aInstance) : base(aInstance) {}

        public ViewSelectionBookmark(BookmarkManager aBookmarkManager, ModelBrowser aBrowser)
            : base()
        {
            iBookmarkManager = aBookmarkManager;
            iBrowser = aBrowser;
            NSBundle.LoadNibNamedOwner("ViewSelectionBookmark.nib", this);
        }


        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // set appearance of view
            TextFieldTitle.TextColor = NSColor.WhiteColor;
            TextFieldTitle.Font = FontManager.FontLarge;

            ViewTable.RowHeight = 60.0f;
            ViewTable.BackgroundColor = NSColor.ClearColor;

            NSTableColumn textColumn = ViewTable.TableColumns[1].CastAs<NSTableColumn>();
            NSTextFieldCell textCell = textColumn.DataCell.CastAs<NSTextFieldCell>();
            textCell.TextColor = NSColor.WhiteColor;
            textCell.Font = FontManager.FontSemiLarge;

            // setup model eventing
            iBookmarkManager.EventBookmarkAdded += ModelChanged;
            iBookmarkManager.EventBookmarkRemoved += ModelChanged;
            iBookmarkManager.EventBookmarksChanged += ModelChanged;
            ModelChanged(this, EventArgs.Empty);
			
			// setup drag/drop
			ViewTable.DataSource = this;
			ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationMove |
                                                             NSDragOperation.NSDragOperationDelete,
                                                             true);
			NSArray dragTypes = NSArray.ArrayWithObject(PasteboardViewDragDataBookmarks.PboardType);
            ViewTable.RegisterForDraggedTypes(dragTypes);

            // setup delegate - if this is done in IB, the methods can be called
            // before awakeFromNib which causes complications
            ViewTable.Delegate = this;
            ViewTable.DeselectAll(this);
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iBookmarkManager.EventBookmarkAdded -= ModelChanged;
            iBookmarkManager.EventBookmarkRemoved -= ModelChanged;
            iBookmarkManager.EventBookmarksChanged -= ModelChanged;

            View.Release();
            ArrayController.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region IViewPopover implementation
        // no need to implement View and Release since the base class already implements them
        public event EventHandler<EventArgs> EventClose;
        #endregion IViewPopover implementation
		
		#region NSTableView data source methods
        [ObjectiveCMessage("tableView:writeRowsWithIndexes:toPasteboard:")]
        public bool TableViewWriteToPasteboard(NSTableView aTableView, NSIndexSet aRows, NSPasteboard aPasteboard)
        {
            Console.WriteLine("TableViewWriteToPasteboard::" + aRows.Count);
			
			iDraggedBookmarks = new List<Bookmark>();

            uint index = aRows.FirstIndex;
            while (index != FoundationFramework.NSNotFound)
            {
                iDraggedBookmarks.Add(iBookmarks[(int)index].CastAs<BookmarkData>().Bookmark);
                index = aRows.IndexGreaterThanIndex(index);
            }
			
			
            // add a token to the pasteboard
            PasteboardViewDragDataBookmarks token = new PasteboardViewDragDataBookmarks();
            NSData tokenData = NSKeyedArchiver.ArchivedDataWithRootObject(token);
            token.Release();

            aPasteboard.DeclareTypesOwner(NSArray.ArrayWithObject(PasteboardViewDragDataBookmarks.PboardType), null);
            aPasteboard.SetDataForType(tokenData, PasteboardViewDragDataBookmarks.PboardType);
			iDragging = true;
			return true;
        }

        [ObjectiveCMessage("tableView:draggedImage:endedAt:operation:")]
        public void TableViewDraggedImageEnded(NSTabView aTableView, NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {            
			if (aOperation == NSDragOperation.NSDragOperationDelete)
			{
				foreach(Bookmark b in iDraggedBookmarks)
				{
					UserLog.WriteLine("Deleting bookmark: " + b.Title);
					iBookmarkManager.Remove(b);
				}
			}
        }

        [ObjectiveCMessage("tableView:validateDrop:proposedRow:proposedDropOperation:")]
        public NSDragOperation TableViewValidateDrop(NSTableView aTableView, INSDraggingInfo aInfo, int aRow, NSTableViewDropOperation aOperation)
        {
			if (aTableView == ViewTable)
			{
				return NSDragOperation.NSDragOperationMove;
			}
			return NSDragOperation.NSDragOperationNone;
        }

        [ObjectiveCMessage("tableView:acceptDrop:row:dropOperation:")]
        public bool TableViewAcceptDrop(NSTableView aTableView, INSDraggingInfo aInfo, int aRow, NSTableViewDropOperation aOperation)
        {
			if (aTableView == ViewTable)
			{
				UserLog.WriteLine("Moving " + iDraggedBookmarks.Count + " bookmark(s) to index " + aRow);
				iBookmarkManager.Move(aRow, iDraggedBookmarks);
				return true;
			}
			return false;
        }
        #endregion NSTableView data source methods

        #region NSTableView delegate functions
        [ObjectiveCMessage("tableView:didClickCellAtColumn:row:")]
        public void TableViewDidClickRow(NSTableView aView, int aCol, int aRow)
        {
			if (!iDragging){
	            // extended delegate function that is called on a mouse up - see
	            // the TableViewClickable implementation below
	            if (aRow == -1)
	            {
	                // ignore clicks on the table background
	                return;
	            }
	            else
	            {
	                // browse to the selected bookmark
	                BookmarkData data = iBookmarks.ObjectAtIndex((uint)aRow).CastAs<BookmarkData>();
	                iBrowser.Browse(data.Bookmark.BreadcrumbTrail);
	            }
	
	            // always close the popover
	            if (EventClose != null)
	            {
	                EventClose(this, EventArgs.Empty);
	            }
			}
			iDragging = false;
        }
        #endregion NSTableView delegate functions


        #region NSArrayController bindings
        public NSMutableArray Bookmarks
        {
            [ObjectiveCMessage("bookmarks")]
            get { return iBookmarks; }
        }
        #endregion NSArrayController bindings


        [ObjectiveCMessage("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem aItem)
        {
            return (aItem.Action == ObjectiveCRuntime.Selector("deleteBookmark:") && ViewTable.ClickedRow >= 0);
        }

        [ObjectiveCMessage("deleteBookmark:")]
        public void DeleteBookmark(Id aSender)
        {
            Assert.Check(ViewTable.ClickedRow >= 0);

            BookmarkData data = iBookmarks.ObjectAtIndex((uint)ViewTable.ClickedRow).CastAs<BookmarkData>();
            iBookmarkManager.Remove(data.Bookmark);
        }

        private void ModelChanged(object sender, EventArgs e)
        {
            iBookmarks.Release();
            iBookmarks = new NSMutableArray();

            foreach (Bookmark b in iBookmarkManager.Bookmarks)
            {
                BookmarkData data = new BookmarkData(b);
                iBookmarks.AddObject(data);
                data.Release();
            }

            WillChangeValueForKey(NSString.StringWithUTF8String("bookmarks"));
            DidChangeValueForKey(NSString.StringWithUTF8String("bookmarks"));
        }

        [ObjectiveCField]
        public NSTableView ViewTable;

        [ObjectiveCField]
        public NSTextField TextFieldTitle;

        [ObjectiveCField]
        public NSArrayController ArrayController;

        private BookmarkManager iBookmarkManager;
        private ModelBrowser iBrowser;
        private NSMutableArray iBookmarks = new NSMutableArray();
		private bool iDragging;
		private List<Bookmark> iDraggedBookmarks;
    }


    // Class for the data bindings
    [ObjectiveCClass]
    public class BookmarkData : NSObject, INSCopying
    {
        public BookmarkData() : base() {}
        public BookmarkData(IntPtr aInstance) : base(aInstance) {}

        public BookmarkData(Bookmark aBookmark)
        {
            iBookmark = aBookmark;
        }

        public NSImage Image
        {
            [ObjectiveCMessage("image")]
            get { return Properties.Resources.IconBookmark; }
        }

        public Bookmark Bookmark
        {
            get { return iBookmark; }
        }

        #region INSCopying implementation
        [ObjectiveCMessage("copyWithZone:")]
        public Id CopyWithZone(IntPtr aZone)
        {
            BookmarkData copy = new BookmarkData();
            copy.iBookmark = iBookmark;
            return copy;
        }
        #endregion INSCopying implementation

        private Bookmark iBookmark;
    }


    // Custom cell class for the bookmarks
    [ObjectiveCClass]
    public class CellBookmark : NSTextFieldCell
    {
        public CellBookmark() : base() {}
        public CellBookmark(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawWithFrame:inView:")]
        public void Draw(NSRect aCellFrame, NSView aControlView)
        {
            BookmarkData item = ObjectValue.CastAs<BookmarkData>();

            // create attributes for the title and subtitle text
            NSMutableParagraphStyle style = new NSMutableParagraphStyle();
            style.SetParagraphStyle(NSParagraphStyle.DefaultParagraphStyle);
            style.SetLineBreakMode(NSLineBreakMode.NSLineBreakByTruncatingTail);

            NSDictionary titleAttr = NSDictionary.DictionaryWithObjectsAndKeys(FontManager.FontMedium, NSAttributedString.NSFontAttributeName,
                                                                               NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                               style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                               null);
            NSDictionary subtitleAttr = NSDictionary.DictionaryWithObjectsAndKeys(FontManager.FontSmall, NSAttributedString.NSFontAttributeName,
                                                                                  NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                                  style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                                  null);
            style.Release();

            NSPoint pt = aCellFrame.origin;

            // draw title
            NSString title = NSString.StringWithUTF8String(item.Bookmark.Title);
            title.DrawAtPointWithAttributes(pt, titleAttr);

            NSSize size = title.SizeWithAttributes(titleAttr);
            pt.y += size.height;

            // draw breadcrumb
            List<string> titleTrail = new List<string>();
            foreach (Breadcrumb bc in item.Bookmark.BreadcrumbTrail)
            {
                titleTrail.Add(bc.Title);
            }
            string breadcrumb = string.Join("/", titleTrail.ToArray());
            NSString breadcrumb2 = NSString.StringWithUTF8String(breadcrumb);
            breadcrumb2.DrawAtPointWithAttributes(pt, subtitleAttr);
        }
    }
}



