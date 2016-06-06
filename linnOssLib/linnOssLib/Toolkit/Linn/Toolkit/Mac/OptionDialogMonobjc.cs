
using System;
using System.Collections.Generic;

using Monobjc;
using Monobjc.Cocoa;


namespace Linn.Toolkit.Cocoa
{
    public class OptionDialogMonobjc
    {
        public OptionDialogMonobjc(IList<IOptionPage> aOptionPages)
        {
            // create a list of option pages
            iPages =  new List<OptionPageMonobjc>();

            NSRect windowRect = new NSRect(0, 0, kWidth, kHeight);
            float division = kWidth / 4;
            NSRect scrollViewRect = new NSRect(kPadding, kPadding, division - (kPadding * 2), kHeight - (kPadding * 2));
            NSRect pageRect = new NSRect(division + kPadding, kPadding, kWidth - division - (kPadding * 2), kHeight - (kPadding * 2));

            foreach(IOptionPage page in aOptionPages)
            {
                iPages.Add(new OptionPageMonobjc(page, pageRect));
            }

            // create main window for the dialog
            iWindow = new NSWindow(windowRect,
                                   NSWindowStyleMasks.NSClosableWindowMask |
                                   NSWindowStyleMasks.NSTitledWindowMask,
                                   NSBackingStoreType.NSBackingStoreBuffered,
                                   false);
            iWindow.Title = NSString.StringWithUTF8String("User Options");
            iWindow.SetDelegate(d =>
            {
                d.WindowShouldClose += delegate(Id aSender) { return true; };
                d.WindowWillClose += delegate(NSNotification aNotification) { NSApplication.NSApp.AbortModal(); }; });
            iWindow.IsReleasedWhenClosed = false;

            // create a view for the window content
            NSView view = new NSView();
            iWindow.ContentView = view;

            NSScrollView scrollView = new NSScrollView();
         
            scrollView.HasVerticalScroller = true;
            scrollView.HasHorizontalScroller = true;
            scrollView.AutohidesScrollers = true;
         
            NSTableColumn tableColumn = new NSTableColumn();
            tableColumn.ResizingMask = NSTableColumnResizingMasks.NSTableColumnAutoresizingMask | NSTableColumnResizingMasks.NSTableColumnUserResizingMask;
            tableColumn.IsEditable = false;

            iTableDelegate = new OptionDialogMonobjcDelegate(iPages);
            iTableDataSource = new OptionDialogMonobjcDataSource(iPages);

            iTableDelegate.EventSelectedPage += EventSelectedPageHandler;

            iTableView = new NSTableView();
            iTableView.DataSource = iTableDataSource;
            iTableView.HeaderView = null;
            iTableView.UsesAlternatingRowBackgroundColors = true;
            iTableView.AddTableColumn(tableColumn);
            iTableView.Delegate = iTableDelegate;
            iTableView.AllowsEmptySelection = false;
            iTableView.AllowsMultipleSelection = false;

            scrollView.Frame = scrollViewRect;
            iTableView.Frame = scrollView.ContentView.Bounds;
            tableColumn.Width = iTableView.Bounds.Width - 3;
         
            scrollView.DocumentView = iTableView;
         

            view.AddSubview(scrollView);

            iTableView.ReloadData();

            // view have been added to the window so they can be released
            view.Release();
            tableColumn.Release();
            scrollView.Release();
        }

        void EventSelectedPageHandler (object sender, EventArgsOptionPage e)
        {
            if (iCurrentView != null && iCurrentView != e.Page.View)
            {
                iCurrentView.RemoveFromSuperview();
            }
            iCurrentView = e.Page.View;
            iWindow.ContentView.AddSubview(iCurrentView);
        }


		
		public void Open()
		{
            // assert this is run once
            Assert.Check(iWindow != null);

            // run the window modally
            NSApplication.NSApp.RunModalForWindow(iWindow);

            // clean up
            iTableDelegate.EventSelectedPage -= EventSelectedPageHandler;
            iTableView.Delegate = null;
            iTableView.DataSource = null;
            iTableView.RemoveFromSuperview();
            iTableView.Release();
            iTableDelegate.Release();
            iTableDataSource.Release();
            foreach (OptionPageMonobjc page in iPages)
            {
                page.Dispose();
            }
            iWindow.Release();
            iWindow = null;
		}
		
		private NSWindow iWindow;
        private List<OptionPageMonobjc> iPages;
        private NSView iCurrentView;
        private const float kWidth= 600;
        private const float kHeight = 320;
        private const float kPadding = 10;
        private OptionDialogMonobjcDelegate iTableDelegate;
        private OptionDialogMonobjcDataSource iTableDataSource;
        private NSTableView iTableView;
    }

    [ObjectiveCClass]
    public class OptionDialogMonobjcDelegate : NSObject
    {
        public event EventHandler<EventArgsOptionPage> EventSelectedPage;

        public OptionDialogMonobjcDelegate() : base() {}
        public OptionDialogMonobjcDelegate(IntPtr aInstance) : base(aInstance) {}
        public OptionDialogMonobjcDelegate(List<OptionPageMonobjc> aPages)
        {
            iPages = aPages;
        }

        [ObjectiveCMessage("tableViewSelectionDidChange:")]
        public void TableViewSelectionDidChange(NSNotification aNotification)
        {
            NSTableView tableView = aNotification.Object.CastAs<NSTableView>();
            NSIndexSet selectedIndexSet = tableView.SelectedRowIndexes;

            int index = (int)selectedIndexSet.FirstIndex;
            OptionPageMonobjc page = iPages[index];
            OnEventSelectedPage(page);
        }

        private void OnEventSelectedPage(OptionPageMonobjc aPage)
        {
            EventHandler<EventArgsOptionPage> del = EventSelectedPage;
            if (del != null)
            {
                del(this, new EventArgsOptionPage(aPage));
            }
        }

        private List<OptionPageMonobjc> iPages;
    }

    public class EventArgsOptionPage : EventArgs
    {
        public EventArgsOptionPage(OptionPageMonobjc aPage) : base()
        {
            Page = aPage;
        }
        public OptionPageMonobjc Page {get;set;}

    }

    [ObjectiveCClass]
     public class OptionDialogMonobjcDataSource : NSObject
     {
         public OptionDialogMonobjcDataSource() {}
         public OptionDialogMonobjcDataSource(IntPtr aInstance) : base(aInstance) {}
    
         public OptionDialogMonobjcDataSource(IList<OptionPageMonobjc> aPages)
         {
             iPages = aPages;
         }
    
         [ObjectiveCMessage("numberOfRowsInTableView:")]
         public int numberOfRowsInTableView(NSTableView aTableView)
         {
             return iPages.Count;
         }
    
         [ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
         public NSObject tableView_objectValueForTableColumn_row(NSTableView aTableView, NSTableColumn aTableColumn, int aRowIndex)
         {
             return NSString.StringWithUTF8String(iPages[aRowIndex].Name);
         }

         private IList<OptionPageMonobjc> iPages;
     }
}

