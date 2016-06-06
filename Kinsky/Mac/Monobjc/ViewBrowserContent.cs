
using System;
using System.Collections.Generic;

using Linn;
using Linn.Kinsky;

using Upnp;

using Monobjc;
using Monobjc.Cocoa;
using Monobjc.ImageKit;


// View classes that correspond to the ViewBrowserContent.xib file

namespace KinskyDesktop
{
    public interface IViewAddBookmark
    {
        void ShowAddBookmark(Location aLocation);
    }


    // Top level delegate class for the nib
    [ObjectiveCClass]
    public class ViewBrowserContent : NSObject, IContentHandler
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewBrowserContent));

        public ViewBrowserContent() : base() {}
        public ViewBrowserContent(IntPtr aInstance) : base(aInstance) {}

        public void Initialise(ModelBrowser aModel, IViewAddBookmark aViewAddBookmark)
        {
            iModel = aModel;

            // load the nib after members have been set
            NSBundle.LoadNibNamedOwner("ViewBrowserContent.nib", this);

            ViewNormal.Initialise(aViewAddBookmark);
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise appearance of the root view
            ViewRoot.BackgroundColour = NSColor.BlackColor;
            ViewRoot.SetOpaque(true);

            ViewError.SetBrowser(iModel);

            // this makes the hourglass always be drawn on top of other layers - not entirely
            // sure why
            Hourglass.WantsLayer = true;

            // set the current view
            iCurrentView = ViewNormal;

            // setup model eventing
            iModel.EventBreadcrumbChanged += ModelLocationChanged;
            iModel.EventLocationChanged += ModelLocationChanged;
            iModel.EventLocationFailed += ModelLocationFailed;
            ArtworkCacheInstance.Instance.EventImageAdded += ArtworkCacheUpdated;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventBreadcrumbChanged -= ModelLocationChanged;
            iModel.EventLocationChanged -= ModelLocationChanged;
            iModel.EventLocationFailed -= ModelLocationFailed;
            ArtworkCacheInstance.Instance.EventImageAdded -= ArtworkCacheUpdated;

            if (iContent != null)
            {
                iContent.Dispose();
            }

            // clean up top level nib objects
            ViewAlbum.Release();
            ViewError.Release();
            ViewNormal.Release();
            ViewRoot.Release();
            ArrayController.Release();
            MenuAlbum.Release();
            MenuList.Release();
            DataSource.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region IContentHandler interface methods
        void IContentHandler.Open(IContentCollector aCollector, uint aCount)
        {
            // this is called in the main thread
            if (iContent != null && iContent.UsesCollector(aCollector))
            {
                // content container has been successfully opened - update data source
                DataSource.SetContent(iContent);

                // hide the hourglass
                Hourglass.Show(false);
                ViewError.Show(false);

                // setup the view to show and its controller
                if (iContent.Location.Current.Metadata is musicAlbum)
                {
                    ViewNormal.Show(false);
                    ViewAlbum.SetContent(iModel, iContent);
                    ViewAlbum.Show(true);
                    iCurrentView = ViewAlbum;
                }
                else
                {
                    ViewAlbum.Show(false);
                    ViewNormal.SetContent(iModel, iContent);
                    ViewNormal.Show(true);
                    iCurrentView = ViewNormal;
                }

                // refresh the view
                iCurrentView.SetNeedsDisplay();
            }
        }

        void IContentHandler.Item(IContentCollector aCollector, uint aIndex, upnpObject aObject)
        {
            // this should not be called
            Assert.Check(false);
        }

        void IContentHandler.Items(IContentCollector aCollector, uint aStartIndex, IList<upnpObject> aObjects)
        {
            // this is called in the main thread
            if (iContent != null && iContent.UsesCollector(aCollector))
            {
                int selectedItem = -1;
                if (iCurrentView.IsImageBrowserView)
                {
                    // image browser view requires notification to be sent out
                    selectedItem = DataSource.RefreshItemsNotify((int)aStartIndex, aObjects.Count);
                }
                else
                {
                    // table views do not require notification to be sent out - more efficient
                    selectedItem = DataSource.RefreshItems((int)aStartIndex, aObjects.Count);
                }

                if (selectedItem != -1)
                {
                    iCurrentView.ScrollToIndex(selectedItem);
                }

                // refresh the view
                iCurrentView.SetNeedsDisplay();
            }
        }

        void IContentHandler.ContentError(IContentCollector aCollector, string aMessage)
        {
            // this is called in the main thread
            if (iContent != null && iContent.UsesCollector(aCollector))
            {
                ViewAlbum.SetContent(null, null);
                ViewNormal.SetContent(null, null);

                ViewAlbum.Show(false);
                ViewNormal.Show(false);
                Hourglass.Show(false);
                ViewError.Show(true);
            }
        }
        #endregion IContentHandler interface methods


        #region Context menu handlers
        [ObjectiveCMessage("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem aItem)
        {
            // the current view may currently have no controller e.g. if it is in between
            // locations
            if (iCurrentView.Controller == null)
            {
                return false;
            }

            if (aItem.Action == ObjectiveCRuntime.Selector("open:"))
            {
                return iCurrentView.Controller.ValidateMenuItemOpen();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("playNow:"))
            {
                return iCurrentView.Controller.ValidateMenuItemPlayNow();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("playNext:"))
            {
                return iCurrentView.Controller.ValidateMenuItemPlayNext();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("playLater:"))
            {
                return iCurrentView.Controller.ValidateMenuItemPlayLater();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("delete:"))
            {
                return iCurrentView.Controller.ValidateMenuItemDelete();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("rename:"))
            {
                return iCurrentView.Controller.ValidateMenuItemRename();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("bookmark:"))
            {
                return iCurrentView.Controller.ValidateMenuItemBookmark();
            }
            else if (aItem.Action == ObjectiveCRuntime.Selector("details:"))
            {
                return iCurrentView.Controller.ValidateMenuItemDetails();
            }
            return false;
        }

        [ObjectiveCMessage("open:")]
        public void ContextMenuOpen(Id aSender)
        {
            iCurrentView.Controller.MenuItemOpen();
        }

        [ObjectiveCMessage("playNow:")]
        public void ContextMenuPlayNow(Id aSender)
        {
            iCurrentView.Controller.MenuItemPlayNow();
        }

        [ObjectiveCMessage("playNext:")]
        public void ContextMenuPlayNext(Id aSender)
        {
            iCurrentView.Controller.MenuItemPlayNext();
        }

        [ObjectiveCMessage("playLater:")]
        public void ContextMenuPlayLater(Id aSender)
        {
            iCurrentView.Controller.MenuItemPlayLater();
        }

        [ObjectiveCMessage("delete:")]
        public void ContextMenuDelete(Id aSender)
        {
            iCurrentView.Controller.MenuItemDelete();
        }

        [ObjectiveCMessage("rename:")]
        public void ContextMenuRename(Id aSender)
        {
            iCurrentView.Controller.MenuItemRename();
        }

        [ObjectiveCMessage("bookmark:")]
        public void ContextMenuBookmark(Id aSender)
        {
            iCurrentView.Controller.MenuItemBookmark();
        }

        [ObjectiveCMessage("details:")]
        public void ContextMenuDetails(Id aSender)
        {
            iCurrentView.Controller.MenuItemDetails();
        }
        #endregion Context menu handlers


        private void ModelLocationChanged(object sender, EventArgs e)
        {
            // dispose of previous model content data
            if (iContent != null)
            {
                iContent.Dispose();
                iContent = null;
            }

            // clear the data source
            DataSource.Clear(iModel.SelectedId);

            // clear the previous content from the views
            ViewAlbum.SetContent(null, null);
            ViewNormal.SetContent(null, null);

            // create the model content for the new location if it is valid, this will then attempt to open the
            // new location and feed back via the IContentHandler interface
            if (iModel.Location != null)
            {
                iContent = iModel.CreateContainerContent(this);
            }

            // this method is called in response to the LocationChanged and BreadcrumbChanged events from the ModelBrowser
            // - make sure the error view is hidden but leave the other views visible - this is primarily for the IKImageBrowserView
            // that needs to be visible to refresh correctly. The problems is as follows:
            // 1. Browser is displaying container A.
            // 2. User clicks breadcrumb to go to container B.
            // 3. This method gets called - data source is cleared, hourglass is shown and (previously) the ViewNormal (which
            //    is showing the ImageBrowserView) is hidden
            // 4. ContentCollector is started (outside of this function)
            // 5. IContentHandler.Open is called - data source is initialised with new item count, hourglass is hidden and
            //    ViewNormal is shown
            // 6. At this point, the view momentarily displays the contents of container A before showing the contents of B - this
            //    can be very clearly seen by adding Sleeps at the beginning and end of the IContentHandler.Open implementation
            ViewError.Show(false);
            ViewNormal.SetNeedsDisplay();
            ViewAlbum.SetNeedsDisplay();

            // show the hourglass
            Hourglass.Show(true);
        }

        private void ModelLocationFailed(object sender, EventArgs e)
        {
            ViewAlbum.Show(false);
            ViewNormal.Show(false);
            Hourglass.Show(false);
            ViewError.Show(true);
        }

        private void ArtworkCacheUpdated(object sender, ArtworkCache.EventArgsArtwork e)
        {
            if (NSApplication.NSApp.InvokeRequired)
            {
                NSApplication.NSApp.BeginInvoke((EventHandler<ArtworkCache.EventArgsArtwork>)ArtworkCacheUpdated, sender, e);
            }
            else
            {
                iCurrentView.SetNeedsDisplay();
            }
        }

        [ObjectiveCField]
        public NSArrayController ArrayController;

        [ObjectiveCField]
        public BrowserDataSource DataSource;

        [ObjectiveCField]
        public NSMenu MenuAlbum;

        [ObjectiveCField]
        public NSMenu MenuList;

        [ObjectiveCField]
        public ViewBrowserAlbum ViewAlbum;

        [ObjectiveCField]
        public ViewBrowserError ViewError;

        [ObjectiveCField]
        public ViewHourglass Hourglass;

        [ObjectiveCField]
        public ViewBrowserNormal ViewNormal;

        [ObjectiveCField]
        public ViewEmpty ViewRoot;

        private ModelBrowser iModel;
        private ContainerContent iContent;
        private IViewBrowser iCurrentView;
    }


    // Window controller for the rename sheet
    [ObjectiveCClass]
    public class SheetRenameController : NSWindowController
    {
        public SheetRenameController() : base() {}
        public SheetRenameController(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
        }

        public void Show(NSWindow aParent, IContainer aContainer, upnpObject aObject)
        {
            // initialise the sheet
            iContainer = aContainer;
            iObject = aObject;
            TextField.StringValue = NSString.StringWithUTF8String(DidlLiteAdapter.Title(iObject));
            TextField.SelectText(this);

            // show the sheet
            NSApplication.NSApp.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(Window, aParent, RenameDidEnd, IntPtr.Zero);
        }

        [ObjectiveCMessage("buttonCancel:")]
        public void ButtonCancel(Id aSender)
        {
            // close the sheet
            NSApplication.NSApp.EndSheet(Window);
        }

        [ObjectiveCMessage("buttonRename:")]
        public void ButtonRename(Id aSender)
        {
            // close the sheet and rename the item
            NSApplication.NSApp.EndSheet(Window);

            iContainer.Rename(iObject.Id, TextField.StringValue.ToString());
        }

        private void RenameDidEnd(NSWindow aWindow, int aReturnCode, IntPtr aContextInfo)
        {
            // must hide the window after sheet has been closed
            Window.OrderOut(this);
        }

        [ObjectiveCField]
        public NSTextField TextField;

        private IContainer iContainer;
        private upnpObject iObject;
    }


    public interface IViewBrowser
    {
        ControllerBrowserContent Controller { get; }
        void ScrollToIndex(int aIndex);
        void SetNeedsDisplay();
        void Show(bool aShow);
        bool IsImageBrowserView { get; }
    }


    // View class for the normal browser view
    [ObjectiveCClass]
    public class ViewBrowserNormal : NSObject, IViewBrowser, IViewBrowserContent
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewBrowserNormal));

        public ViewBrowserNormal() : base() {}
        public ViewBrowserNormal(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise appearance of the NSTableView
            ViewTable.RowHeight = 90;
            ViewTable.BackgroundColor = NSColor.BlackColor;

            NSTableColumn titleCol = ViewTable.TableColumns[kTableColumnTitle].CastAs<NSTableColumn>();
            NSTextFieldCell titleCell = titleCol.DataCell.CastAs<NSTextFieldCell>();
            titleCell.TextColor = NSColor.WhiteColor;
            titleCell.Font = FontManager.FontMedium;


            // initialise appearance of the IKImageBrowserView
            ViewImageBrowser.SetValueForKey(NSColor.BlackColor, "IKImageBrowserBackgroundColorKey");

            Id attr = ViewImageBrowser.ValueForKey("IKImageBrowserCellsTitleAttributesKey");
            NSMutableDictionary dict = new NSMutableDictionary(attr.CastTo<NSDictionary>());
            dict.SetObjectForKey(NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName);
            dict.SetObjectForKey(FontManager.FontMedium, NSAttributedString.NSFontAttributeName);
            ViewImageBrowser.SetValueForKey(dict, "IKImageBrowserCellsTitleAttributesKey");
            dict.Release();

            attr = ViewImageBrowser.ValueForKey("IKImageBrowserCellsHighlightedTitleAttributesKey");
            dict = new NSMutableDictionary(attr.CastTo<NSDictionary>());
            dict.SetObjectForKey(NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName);
            dict.SetObjectForKey(FontManager.FontMedium, NSAttributedString.NSFontAttributeName);
            ViewImageBrowser.SetValueForKey(dict, "IKImageBrowserCellsHighlightedTitleAttributesKey");
            dict.Release();

            ViewImageBrowserParent.BackgroundColor = NSColor.BlackColor;


            // initialise model data
            iOptionContainerView = ModelMain.Instance.Helper.ContainerView;
            iOptionContainerSizeThumbs = ModelMain.Instance.Helper.ContainerSizeThumbs;
            iOptionContainerSizeList = ModelMain.Instance.Helper.ContainerSizeList;

            // initialise which view to show
            iListMode = (iOptionContainerView.Native == 1);
            Show(true);

            // initialise the size of the views
            OptionContainerSizeThumbsChanged(this, EventArgs.Empty);
            OptionContainerSizeListChanged(this, EventArgs.Empty);


            // setup data source for drag and drop
            ViewTable.DataSource = this;
            ViewImageBrowser.DataSource = this;
            ViewImageBrowser.DraggingDestinationDelegate = this;
            ViewImageBrowser.SendMessage("setAllowsDroppingOnItems:", true);

            // setup accepted drag types for the views
            DragDestination.RegisterDragTypes(ViewTable);
            DragDestination.RegisterDragTypes(ViewImageBrowser.CastTo<NSView>());

            // within the application, browser items can be copied, moved and deleted
            // they cannot be dragged outside the application - this is handled for the
            // image browser in the ImageBrowserDragDrop class
            ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationMove |
                                                             NSDragOperation.NSDragOperationCopy |
                                                             NSDragOperation.NSDragOperationDelete,
                                                             true);
            ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationNone, false);


            // setup some event delegates
            ViewTable.DoubleActionEvent += ItemDoubleClicked;

            // setup eventing from the model
            iOptionContainerView.EventValueChanged += OptionContainerViewChanged;
            iOptionContainerSizeThumbs.EventValueChanged += OptionContainerSizeThumbsChanged;
            iOptionContainerSizeList.EventValueChanged += OptionContainerSizeListChanged;

            // setup delegates for the views - if this is done in IB, the methods can be called
            // before awakeFromNib which causes complications
            ViewTable.Delegate = this;
            ViewImageBrowser.Delegate = this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            // for some reason, after this is dealloced, the ImageBrowserView sends a message
            // to its delegate (this object) which causes a crash. Clearing the delegate here
            // resolves it. The message is "imageBrowser:willDisplayCellsAtIndexes:" which
            // appears to be an undocumented delegate methods
            ViewTable.Delegate = null;
            ViewImageBrowser.Delegate = null;

            iOptionContainerView.EventValueChanged -= OptionContainerViewChanged;
            iOptionContainerSizeThumbs.EventValueChanged -= OptionContainerSizeThumbsChanged;
            iOptionContainerSizeList.EventValueChanged -= OptionContainerSizeListChanged;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        public void Initialise(IViewAddBookmark aViewAddBookmark)
        {
            iViewAddBookmark = aViewAddBookmark;
        }

        public void SetContent(ModelBrowser aBrowser, ContainerContent aContent)
        {
            if (aContent != null)
            {
                iController = new ControllerBrowserContent(aBrowser, aContent, ModelMain.Instance.PlaySupport, this);
                iDragSource = new DragSource(iController);
                iDragDestination = new DragDestination(iController);
            }
            else
            {
                iController  = null;
                iDragSource = null;
                iDragDestination = null;
            }
        }


        #region NSTableView delegate methods
        private void ItemDoubleClicked(Id sender)
        {
            // this is actually an event that the table view has been double clicked, which
            // can happen outside of any items
            // the iController member can also be null - for example, when the container has been opened
            // and the hourglass is showing in the view
            if (ViewTable.ClickedRow > -1 && iController != null)
            {
                iController.ItemActivated(ViewTable.ClickedRow);
            }
        }

        [ObjectiveCMessage("tableView:shouldEditTableColumn:row:")]
        public bool TableViewShouldEdit(NSTableView aView, NSTableColumn aTableColumn, int aRow)
        {
            return iController.CanRenameItem(aRow);
        }
        #endregion NSTableView delegate methods


        #region IKImageBrowserView delegate methods
        [ObjectiveCMessage("imageBrowser:backgroundWasRightClickedWithEvent:")]
        public void BackgroundRightClicked(IKImageBrowserView aView, NSEvent aEvent)
        {
            // even though, in IB, the IKImageBrowserView class has an outlet for a menu,
            // this does not work - so we have to manually show them here - use the same one as
            // the table view
            NSMenu.PopUpContextMenuWithEventForView(ViewTable.Menu, aEvent, aView.CastAs<NSView>());
        }

        [ObjectiveCMessage("imageBrowser:cellWasRightClickedAtIndex:withEvent:")]
        public void CellRightClicked(IKImageBrowserView aView, uint aIndex, NSEvent aEvent)
        {
            // even though, in IB, the IKImageBrowserView class has an outlet for a menu,
            // this does not work - so we have to manually show them here - use the same one as
            // the table view
            NSMenu.PopUpContextMenuWithEventForView(ViewTable.Menu, aEvent, aView.CastAs<NSView>());
        }

        [ObjectiveCMessage("imageBrowser:cellWasDoubleClickedAtIndex:")]
        public void CellDoubleClicked(IKImageBrowserView aView, uint aIndex)
        {
            iController.ItemActivated((int)aIndex);
        }
        #endregion IKImageBrowserView delegate methods


        #region NSTableView data source methods
        [ObjectiveCMessage("tableView:writeRowsWithIndexes:toPasteboard:")]
        public bool TableViewWriteToPasteboard(NSTableView aTableView, NSIndexSet aRows, NSPasteboard aPasteboard)
        {
            return (iDragSource.Begin(aRows, ViewTable, aPasteboard) > 0);
        }

        [ObjectiveCMessage("tableView:draggedImage:endedAt:operation:")]
        public void TableViewDraggedImageEnded(NSTabView aTableView, NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {
            iDragSource.End(NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard), aOperation);
        }

        [ObjectiveCMessage("tableView:validateDrop:proposedRow:proposedDropOperation:")]
        public NSDragOperation TableViewValidateDrop(NSTableView aTableView, INSDraggingInfo aInfo, int aRow, NSTableViewDropOperation aOperation)
        {
            return iDragDestination.ValidateDrop(aInfo, ViewTable);
        }

        [ObjectiveCMessage("tableView:acceptDrop:row:dropOperation:")]
        public bool TableViewAcceptDrop(NSTableView aTableView, INSDraggingInfo aInfo, int aRow, NSTableViewDropOperation aOperation)
        {
            return iDragDestination.AcceptDrop(aInfo, aRow, ViewTable);
        }
        #endregion NSTableView data source methods


        #region IKImageBrowserView data source methods
        [ObjectiveCMessage("imageBrowser:writeItemsAtIndexes:toPasteboard:")]
        public uint ImageBrowserWriteToPasteboard(IKImageBrowserView aView, NSIndexSet aIndexes, NSPasteboard aPasteboard)
        {
            return (uint)iDragSource.Begin(aIndexes, ViewImageBrowser, aPasteboard);
        }

        [ObjectiveCMessage("imageBrowser:draggedImage:endedAt:operation:")]
        public void ImageBrowserDraggedImageEnded(IKImageBrowserView aView, NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {
            iDragSource.End(NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard), aOperation);
        }
        #endregion IKImageBrowserView data source methods


        #region IKImageBrowserView dragging delegate methods
        [ObjectiveCMessage("draggingEntered:")]
        public NSDragOperation DraggingEntered(INSDraggingInfo aInfo)
        {
            return iDragDestination.ValidateDrop(aInfo, ViewImageBrowser);
        }

        [ObjectiveCMessage("draggingUpdated:")]
        public NSDragOperation DraggingUpdated(INSDraggingInfo aInfo)
        {
            return iDragDestination.ValidateDrop(aInfo, ViewImageBrowser);
        }

        [ObjectiveCMessage("performDragOperation:")]
        public bool PerformDragOperation(INSDraggingInfo aInfo)
        {
            return iDragDestination.AcceptDrop(aInfo, (int)ViewImageBrowser.IndexAtLocationOfDroppedItem, ViewImageBrowser);
        }
        #endregion IKImageBrowserView dragging delegate methods


        #region IViewBrowser implementation
        public ControllerBrowserContent Controller
        {
            get { return iController; }
        }

        public void ScrollToIndex(int aIndex)
        {
            // scroll the NSTableView to this item
            ViewTable.ScrollRowToVisible(aIndex);

            // scroll the IKImageBrowserView to this item
            NSRect itemFrame = ViewImageBrowser.ItemFrameAtIndex((uint)aIndex);
            ViewImageBrowser.SendMessage("scrollRectToVisible:", itemFrame);
        }

        public void SetNeedsDisplay()
        {
            if (iListMode)
            {
                ViewTable.SetNeedsDisplayInRect(ViewTableParent.DocumentVisibleRect);
            }
            else
            {
                ViewImageBrowser.SendMessage("setNeedsDisplayInRect:", ViewImageBrowserParent.DocumentVisibleRect);
            }
        }

        public void Show(bool aShow)
        {
            if (aShow)
            {
                iIsVisible = true;
                ViewTableParent.IsHidden = !iListMode;
                ViewImageBrowserParent.IsHidden = iListMode;

                // ensure the data is reloaded when the view is shown - the bindings will handle further updates
                // this was to fix the bizarre problem where going from list view to icon view did not refresh
                // the first item in the icon view (its title and image were blank). This fixes it without impacting
                // performance
                if (iListMode)
                {
                    ViewTable.ReloadData();
                }
                else
                {
                    ViewImageBrowser.ReloadData();
                }
            }
            else
            {
                iIsVisible = false;
                ViewTableParent.IsHidden = true;
                ViewImageBrowserParent.IsHidden = true;
            }
        }

        public bool IsImageBrowserView
        {
            get { return !iListMode; }
        }
        #endregion IViewBrowser implementation


        #region IViewBrowserContent interface methods
        public int ClickedItem
        {
            get
            {
                if (iListMode)
                {
                    return ViewTable.ClickedRow;
                }
                else
                {
                    if (ViewImageBrowser.SelectionIndexes.Count > 0)
                    {
                        return (int)ViewImageBrowser.SelectionIndexes.FirstIndex;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        public int ClickedGroup
        {
            // no groups in this view
            get { return -1; }
        }

        public bool SelectionContainsItem(int aIndex)
        {
            return iListMode ? ViewTable.IsRowSelected(aIndex) : ViewImageBrowser.SelectionIndexes.ContainsIndex((uint)aIndex);
        }

        public bool SelectionContainsGroup(int aIndex)
        {
            // no groups in this view
            return false;
        }

        public int SelectedItemCount
        {
            get { return iListMode ? ViewTable.NumberOfSelectedRows : (int)ViewImageBrowser.SelectionIndexes.Count; }
        }

        public IList<int> SelectedItems
        {
            get
            {
                List<int> list = new List<int>();

                NSIndexSet selectedIndexSet = iListMode ? ViewTable.SelectedRowIndexes : ViewImageBrowser.SelectionIndexes;

                uint index = selectedIndexSet.FirstIndex;
                while (index != FoundationFramework.NSNotFound)
                {
                    list.Add((int)index);
                    index = selectedIndexSet.IndexGreaterThanIndex(index);
                }

                return list.AsReadOnly();
            }
        }

        public void StartRename()
        {
            SheetRename.Show(ViewTable.Window, iController.Content.Location.Current, iController.Content.Object(ClickedItem));
        }

        public void ShowAddBookmark(Location aLocation)
        {
            iViewAddBookmark.ShowAddBookmark(aLocation);
        }
        #endregion IViewBrowserContent interface methods


        private void OptionContainerViewChanged(object sender, EventArgs e)
        {
            ShowList(iOptionContainerView.Native == 1);
        }

        private void OptionContainerSizeThumbsChanged(object sender, EventArgs e)
        {
            // set size for the image browser view
            float cellSize = 70 + (iOptionContainerSizeThumbs.Native * 150);
            ViewImageBrowser.CellSize = new NSSize(cellSize, cellSize);
            ArtworkCacheInstance.Instance.DownscaleSize = (int)cellSize;
        }

        private void OptionContainerSizeListChanged(object sender, EventArgs e)
        {
            // set size for the table view
            ViewTable.RowHeight = 30 + (iOptionContainerSizeList.Native * 170);

            // set the width of the image column
            NSTableColumn col = ViewTable.TableColumns[kTableColumnImage].CastAs<NSTableColumn>();
            col.Width = ViewTable.RowHeight;
            ViewTable.SizeLastColumnToFit();
            ArtworkCacheInstance.Instance.DownscaleSize = (int)ViewTable.RowHeight;
        }

        private void ShowList(bool aShow)
        {
            // apply scrolling to the view to become visible so that the
            // same items are visible
            if (aShow)
            {
                // get the visible rect of the IKImageBrowserView
                NSRect visibleRect = ViewImageBrowserParent.DocumentVisibleRect;

                // get the indices of visible items
                NSIndexSet rowIndices = ViewImageBrowser.SendMessage<NSIndexSet>("rowIndexesInRect:", visibleRect);
                uint numCols = ViewImageBrowser.SendMessage<uint>("numberOfColumns");

                // pick the last item in the second-to-last visible row - visible rows include those that
                // are partially visible, so the last visible row may be barely visible
                int itemIndex = (int)(rowIndices.LastIndex * numCols) - 1;

                // scroll the NSTableView to this index - this puts the item
                // at the bottom of the visible part of the view
                ViewTable.ScrollRowToVisible(itemIndex);
            }
            else
            {
                // get the visible rect of the NSTableView
                NSRect visibleRect = ViewTableParent.DocumentVisibleRect;

                // get the range of rows in this visible rect
                NSRange rows = ViewTable.RowsInRect(visibleRect);

                // get the frame of the item corresponding to the last visible item
                NSRect itemFrame = ViewImageBrowser.ItemFrameAtIndex(rows.location + rows.length);

                // note the IKImageBrowserView method ScrollIndexToVisible does not
                // appear to work, so we need to manually scroll to a given rect
                // - ScrollIndexToVisible(0) scrolled so that the 2nd row of the
                // data was at the top of the view
                ViewImageBrowser.SendMessage("scrollRectToVisible:", itemFrame);
            }

            // show and hide the views
            iListMode = aShow;
            Show(iIsVisible);
        }


        [ObjectiveCField]
        public NSScrollView ViewTableParent;

        [ObjectiveCField]
        public NSTableView ViewTable;

        [ObjectiveCField]
        public NSScrollView ViewImageBrowserParent;

        [ObjectiveCField]
        public IKImageBrowserView ViewImageBrowser;

        [ObjectiveCField]
        public SheetRenameController SheetRename;

        private const int kTableColumnImage = 0;
        private const int kTableColumnTitle = 1;

        private OptionUint iOptionContainerView;
        private OptionFloat iOptionContainerSizeThumbs;
        private OptionFloat iOptionContainerSizeList;
        private bool iListMode;
        private bool iIsVisible;
        private ControllerBrowserContent iController;
        private DragSource iDragSource;
        private DragDestination iDragDestination;
        private IViewAddBookmark iViewAddBookmark;
    }


    // View class for the album browser view
    [ObjectiveCClass]
    public class ViewBrowserAlbum : NSObject, IViewBrowser, IViewBrowserContent
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewBrowserAlbum));

        public ViewBrowserAlbum() : base() {}
        public ViewBrowserAlbum(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // initialise appearance of the NSTableView
            ViewTable.BackgroundColor = NSColor.BlackColor;

            // set font for duration column
            NSTableColumn col = ViewTable.TableColumns[2].CastAs<NSTableColumn>();
            NSTextFieldCell cell = col.DataCell.CastAs<NSTextFieldCell>();
            cell.TextColor = NSColor.WhiteColor;
            cell.Font = FontManager.FontSmall;

            TextFieldAlbum.Font = FontManager.FontLarge;
            TextFieldArtist.Font = FontManager.FontMedium;

            ViewRoot.BackgroundColour = NSColor.BlackColor;
            ViewRoot.SetOpaque(true);

            // initialise the view as hidden
            ViewRoot.IsHidden = true;

            // setup data source for drag and drop
            ViewTable.DataSource = this;

            // within the application, album items can only be copied they cannot be dragged outside the application
            ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationCopy, true);
            ViewTable.SetDraggingSourceOperationMaskForLocal(NSDragOperation.NSDragOperationNone, false);

            // setup some event delegates
            ViewTable.DoubleActionEvent += ItemDoubleClicked;
            ImageArtwork.EventMouseDown += ImageArtworkMouseDown;
            ImageArtwork.EventRightMouseDown += ImageArtworkRightMouseDown;
            ViewRoot.EventMouseDown += ViewRootMouseDown;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            this.SendMessageSuper(ThisClass, "dealloc");
        }


        public void SetContent(ModelBrowser aBrowser, ContainerContent aContent)
        {
            if (aContent != null)
            {
                iController = new ControllerBrowserContent(aBrowser, aContent, ModelMain.Instance.PlaySupport, this);
                iControllerAlbum = new ControllerBrowserAlbum(aContent, ModelMain.Instance.PlaySupport);
                iDragSource = new DragSource(iController);
            }
            else
            {
                iController = null;
                iControllerAlbum = null;
                iDragSource = null;
            }
        }


        #region NSTableView delegate methods
        private void ItemDoubleClicked(Id sender)
        {
            // this is actually an event that the table view has been double clicked, which
            // can happen outside of any items
            if (ViewTable.ClickedRow > -1 && iController != null)
            {
                iController.ItemActivated(ViewTable.ClickedRow);
            }
        }
        #endregion NSTableView delegate methods


        #region NSTableView data source methods
        // note that only the methods for the dragging source are implemented i.e. the album view cannot act
        // as a dragging destination
        [ObjectiveCMessage("tableView:writeRowsWithIndexes:toPasteboard:")]
        public bool TableViewWriteToPasteboard(NSTableView aTableView, NSIndexSet aRows, NSPasteboard aPasteboard)
        {
            return (iDragSource.Begin(aRows, ViewTable, aPasteboard) > 0);
        }

        [ObjectiveCMessage("tableView:draggedImage:endedAt:operation:")]
        public void TableViewDraggedImageEnded(NSTabView aTableView, NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {
            iDragSource.End(NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard), aOperation);
        }
        #endregion NSTableView data source methods


        #region NSDraggingSource methods for dragging the NSImageView
        [ObjectiveCMessage("draggingSourceOperationMaskForLocal:")]
        public NSDragOperation DraggingSourceOperationMask(bool aLocal)
        {
            return aLocal ? NSDragOperation.NSDragOperationCopy : NSDragOperation.NSDragOperationNone;
        }

        [ObjectiveCMessage("draggedImage:endedAt:operation:")]
        public void DraggedImageEnded(NSImage aImage, NSPoint aPt, NSDragOperation aOperation)
        {
            iDragSource.End(NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard), aOperation);
        }
        #endregion NSDraggingSource methods for dragging the NSImageView


        #region context menu handlers for the album art menu
        [ObjectiveCMessage("albumPlayNow:")]
        public void AlbumPlayNow(Id aSender)
        {
            iControllerAlbum.MenuItemPlayNow();
        }

        [ObjectiveCMessage("albumPlayNext:")]
        public void AlbumPlayNext(Id aSender)
        {
            iControllerAlbum.MenuItemPlayNext();
        }

        [ObjectiveCMessage("albumPlayLater:")]
        public void AlbumPlayLater(Id aSender)
        {
            iControllerAlbum.MenuItemPlayLater();
        }

        [ObjectiveCMessage("albumDetails:")]
        public void AlbumDetails(Id aSender)
        {
            iControllerAlbum.MenuItemDetails();
        }
        #endregion context menu handlers for the album art menu


        #region IViewBrowser implementation
        public ControllerBrowserContent Controller
        {
            get { return iController; }
        }

        public void ScrollToIndex(int aIndex)
        {
            ViewTable.ScrollRowToVisible(aIndex);
        }

        public void SetNeedsDisplay()
        {
            // album view will never really have that many items so just refresh the whole view
            ViewTable.SetNeedsDisplay();
            ImageArtwork.SetNeedsDisplay();
            TextFieldAlbum.SetNeedsDisplay();
            TextFieldArtist.SetNeedsDisplay();
        }

        public void Show(bool aShow)
        {
            ViewRoot.IsHidden = !aShow;
        }

        public bool IsImageBrowserView
        {
            get { return false; }
        }
        #endregion IViewBrowser implementation

        #region IViewBrowserContent interface methods
        public int ClickedItem
        {
            get { return ViewTable.ClickedRow; }
        }

        public int ClickedGroup
        {
            // no groups in this view
            get { return -1; }
        }

        public bool SelectionContainsItem(int aIndex)
        {
            return ViewTable.IsRowSelected(aIndex);
        }

        public bool SelectionContainsGroup(int aIndex)
        {
            // no groups in this view
            return false;
        }

        public int SelectedItemCount
        {
            get { return ViewTable.NumberOfSelectedRows; }
        }

        public IList<int> SelectedItems
        {
            get
            {
                List<int> list = new List<int>();

                NSIndexSet selectedIndexSet = ViewTable.SelectedRowIndexes;

                uint index = selectedIndexSet.FirstIndex;
                while (index != FoundationFramework.NSNotFound)
                {
                    list.Add((int)index);
                    index = selectedIndexSet.IndexGreaterThanIndex(index);
                }

                return list.AsReadOnly();
            }
        }

        void IViewBrowserContent.StartRename()
        {
            // can never rename in album view
        }

        void IViewBrowserContent.ShowAddBookmark(Location aLocation)
        {
            // items in the album view are tracks - these cannot be bookmarked
            Assert.Check(false);
        }
        #endregion IViewBrowserContent interface methods


        private void ImageArtworkMouseDown(NSEvent aEvent)
        {
            Assert.Check(iDragSource != null);

            // select all items in the list
            ViewTable.SelectAll(this);
            ViewTable.Window.MakeFirstResponder(ViewTable);

            // create the dragging image - first draw the cell contents to an image
            NSRect imageRect = new NSRect(NSPoint.NSZeroPoint, ImageArtwork.Bounds.size);

            NSImage cellImage = new NSImage(imageRect.size);
            cellImage.LockFocus();
            ImageArtwork.Cell.DrawWithFrameInView(imageRect, ImageArtwork);
            cellImage.UnlockFocus();

            // now draw the cell image to the drag image with transparency added
            NSImage dragImage = new NSImage(imageRect.size);
            dragImage.LockFocus();
            cellImage.DrawInRectFromRectOperationFraction(imageRect, NSRect.NSZeroRect,
                                                          NSCompositingOperation.NSCompositeSourceOver, 0.75f);
            dragImage.UnlockFocus();

            // start a drag operation
            NSPasteboard pboard = NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard);

            iDragSource.Begin(ViewTable.SelectedRowIndexes, ImageArtwork, pboard);

            ImageArtwork.DragImageAtOffsetEventPasteboardSourceSlideBack(dragImage, NSPoint.NSZeroPoint, NSSize.NSZeroSize, aEvent, pboard, this, true);
        }

        private void ImageArtworkRightMouseDown(NSEvent aEvent)
        {
            // select all items in the list
            ViewTable.SelectAll(this);
            ViewTable.Window.MakeFirstResponder(ViewTable);
        }

        private void ViewRootMouseDown(NSEvent aEvent)
        {
            ViewTable.DeselectAll(this);
            ViewTable.Window.MakeFirstResponder(ViewTable);
        }

        [ObjectiveCField]
        public ViewEmpty ViewRoot;

        [ObjectiveCField]
        public NSTableView ViewTable;

        [ObjectiveCField]
        public ImageViewClickable ImageArtwork;

        [ObjectiveCField]
        public NSTextField TextFieldAlbum;

        [ObjectiveCField]
        public NSTextField TextFieldArtist;

        private ControllerBrowserContent iController;
        private ControllerBrowserAlbum iControllerAlbum;
        private DragSource iDragSource;
    }


    // View class for the error browser view
    [ObjectiveCClass]
    public class ViewBrowserError : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewBrowserError));

        public ViewBrowserError() : base() {}
        public ViewBrowserError(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            ViewRoot.BackgroundColour = NSColor.BlackColor;
            ViewRoot.SetOpaque(true);
            ViewRoot.IsHidden = true;

            Image.Image = Properties.Resources.IconError;

            Text.Font = FontManager.FontLarge;

            IButtonHoverType2 button = ButtonRetry.Initialise();
            button.Text = NSString.StringWithUTF8String("Retry");
            button.Font = FontManager.FontMedium;
            ButtonRetry.EventClicked += ButtonRetryClicked;

            button = ButtonHome.Initialise();
            button.Text = NSString.StringWithUTF8String("Home");
            button.Font = FontManager.FontMedium;
            ButtonHome.EventClicked += ButtonHomeClicked;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            this.SendMessageSuper(ThisClass, "dealloc");
        }

        public void SetBrowser(ModelBrowser aBrowser)
        {
            iBrowser = aBrowser;
        }

        public void Show(bool aShow)
        {
            ViewRoot.IsHidden = !aShow;
        }

        private void ButtonRetryClicked(Id aSender)
        {
            iBrowser.Browse(iBrowser.BreadcrumbTrail);
        }

        private void ButtonHomeClicked(Id aSender)
        {
            iBrowser.Browse(iBrowser.BreadcrumbTrail.TruncateEnd(iBrowser.BreadcrumbTrail.Count - 1));
        }

        [ObjectiveCField]
        public ViewEmpty ViewRoot;

        [ObjectiveCField]
        public NSImageView Image;

        [ObjectiveCField]
        public NSTextField Text;

        [ObjectiveCField]
        public ButtonHoverPush ButtonRetry;

        [ObjectiveCField]
        public ButtonHoverPush ButtonHome;

        private ModelBrowser iBrowser;
    }


    // Custom cell class for the table view in the normal browser view
    [ObjectiveCClass]
    public class CellBrowserNormal : NSTextFieldCell
    {
        public CellBrowserNormal() : base() {}
        public CellBrowserNormal(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawWithFrame:inView:")]
        public void Draw(NSRect aCellFrame, NSView aControlView)
        {
            BrowserDataItem item = ObjectValue.CastAs<BrowserDataItem>();

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
            NSString title = NSString.StringWithUTF8String(item.Title);
            if (title != null)
            {
                title.DrawAtPointWithAttributes(pt, titleAttr);

                // draw subtitle if container is a music album
                if (item.UpnpObject is musicAlbum)
                {
                    NSSize size = title.SizeWithAttributes(titleAttr);
                    pt.y += size.height;

                    string subtitle = DidlLiteAdapter.AlbumArtist(item.UpnpObject);
                    NSString subtitle2 = NSString.StringWithUTF8String(subtitle);
                    subtitle2.DrawAtPointWithAttributes(pt, subtitleAttr);
                }
            }
        }
    }


    // IKImageBrowserView derived class to simply reroute the draggedImage:endedAt:operation:
    // through the data source - this basically extends the IKImageBrowserView data source protocol
    [ObjectiveCClass]
    public class ImageBrowserDragDrop : IKImageBrowserView
    {
        public ImageBrowserDragDrop() : base() {}
        public ImageBrowserDragDrop(IntPtr aInstance) : base(aInstance) {}

        public bool IsOpaque
        {
            [ObjectiveCMessage("isOpaque")]
            get { return true; }
        }

        [ObjectiveCMessage("draggedImage:endedAt:operation:")]
        public void DraggedImageEnded(NSImage aImage, NSPoint aPoint, NSDragOperation aOperation)
        {
            if (this.DataSource != null)
            {
                this.DataSource.SendMessage("imageBrowser:draggedImage:endedAt:operation:", this, aImage, aPoint, aOperation);
            }
        }

        [ObjectiveCMessage("draggingSourceOperationMaskForLocal:")]
        public NSDragOperation DraggingSourceOperationMask(bool aIsLocal)
        {
            if (aIsLocal)
            {
                return NSDragOperation.NSDragOperationCopy | NSDragOperation.NSDragOperationDelete | NSDragOperation.NSDragOperationMove;
            }
            else
            {
                return NSDragOperation.NSDragOperationNone;
            }
        }
    }


    public class BrowserImages
    {
        public static NSImage GetImage(upnpObject aObject)
        {
            if (aObject != null)
            {
                // get the artwork URI
                Uri artworkUri = DidlLiteAdapter.ArtworkUri(aObject);

                if (artworkUri != null)
                {
                    // get artwork from the cache
                    ArtworkCache.Item item = ArtworkCacheInstance.Instance.Artwork(artworkUri);

                    if (item == null)
                    {
                        // download of artwork pending - return null here rather than the
                        // "loading" image - this will cause the image browser view to update
                        return null;
                    }
                    else if (item.Failed)
                    {
                        // download failed
                        return Properties.Resources.IconAlbumArtError;
                    }
                    else
                    {
                        // download ok
                        return item.Image;
                    }
                }
                else
                {
                    // object has no artwork URI - use default image
                    return GetDefaultImage(aObject);
                }
            }
            else
            {
                // no object - no image
                return null;
            }
        }

        public static NSImage GetDefaultImage(upnpObject aObject)
        {
            if (aObject is container)
            {
                if (aObject is musicAlbum)
                {
                    return Properties.Resources.IconAlbum;
                }
                else if (aObject is person)
                {
                    return Properties.Resources.IconArtist;
                }
                else if (aObject is playlistContainer)
                {
                    return Properties.Resources.IconPlaylist;
                }
                else if (aObject.ParentId == MediaProviderLibrary.kLibraryId ||
                         aObject.Id == MediaProviderLibrary.kLibraryId)
                {
                    return Properties.Resources.IconLibrary;
                }
                else
                {
                    return Properties.Resources.IconFolder;
                }
            }
            else if (aObject is item)
            {
                if (aObject is audioBroadcast)
                {
                    return Properties.Resources.IconRadio;
                }
                else if (aObject is videoItem)
                {
                    return Properties.Resources.IconVideo;
                }
                else if (aObject is playlistItem)
                {
                    return Properties.Resources.IconPlaylistItem;
                }
                else if (aObject.Title == "Access denied")
                {
                    return Properties.Resources.IconError;
                }
                else
                {
                    return Properties.Resources.IconTrack;
                }
            }
            else
            {
                return Properties.Resources.IconAlbumArtNone;
            }
        }
    }


    // Data source class for use with the browser content views
    [ObjectiveCClass]
    public class BrowserDataSource : NSObject
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(BrowserDataSource));

        public static NSString KeyItems = new NSString("items");
        public static NSString KeySelectionIndices = new NSString("selectionIndices");
        public static NSString KeyAlbumArtwork = new NSString("albumArtwork");
        public static NSString KeyAlbumTitle = new NSString("albumTitle");
        public static NSString KeyAlbumArtist = new NSString("albumArtist");

        public BrowserDataSource() : base() {}
        public BrowserDataSource(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            ArtworkCacheInstance.Instance.EventImageAdded += ArtworkCacheUpdated;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iContent = null;
            iItems.Release();
            ArtworkCacheInstance.Instance.EventImageAdded -= ArtworkCacheUpdated;

            this.SendMessageSuper(ThisClass, "dealloc");
        }


        #region NSArrayController bindings
        public NSMutableArray Items
        {
            [ObjectiveCMessage("items")]    // must be same as KeyItems member
            get
            {
                return iItems;
            }
        }

        public NSIndexSet SelectionIndices
        {
            [ObjectiveCMessage("selectionIndices")] // must be same as KeySelectionIndices member
            get
            {
                return iSelectionIndices;
            }
            [ObjectiveCMessage("setSelectionIndices:")]
            set
            {
                iSelectionIndices.Release();
                iSelectionIndices = value;
                iSelectionIndices.Retain();

                // when going up a folder, the container just exited will be automatically
                // selected. If this takes a while to load, the user can manually select
                // something else - this code will be called. So, to prevent the user selection
                // from being automatically wiped out, cancel the auto-selection of the previous
                // container. Only do this is the selection is actually set to something - sometimes
                // this appears to be set with an empty selection - ignore this case
                if (iSelectionIndices.Count > 0)
                {
                    iItemIdToSelect = string.Empty;
                }
            }
        }
        #endregion NSArrayController bindings

        #region bindings for the album info in the album view
        public NSImage AlbumArtwork
        {
            [ObjectiveCMessage("albumArtwork")] // must be same as KeyAlbumArtwork
            get
            {
                if (iContent != null)
                {
                    upnpObject o = iContent.Location.Current.Metadata;
                    return BrowserImages.GetImage(o);
                }
                else
                {
                    return null;
                }
            }
        }

        public NSString AlbumTitle
        {
            [ObjectiveCMessage("albumTitle")] // must be same as KeyAlbumTitle
            get
            {
                if (iContent != null)
                {
                    return DidlLiteAdapter.Title(iContent.Location.Current.Metadata);
                }
                else
                {
                    return null;
                }
            }
        }

        public NSString AlbumArtist
        {
            [ObjectiveCMessage("albumArtist")] // must be same as KeyAlbumArtist
            get
            {
                if (iContent != null)
                {
                    return DidlLiteAdapter.AlbumArtist(iContent.Location.Current.Metadata);
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion bindings for the album info in the album view

        public void Clear(string aItemIdToSelect)
        {
            WillChangeValueForKey(KeyItems);
            WillChangeValueForKey(KeySelectionIndices);
            WillChangeValueForKey(KeyAlbumArtwork);
            WillChangeValueForKey(KeyAlbumTitle);
            WillChangeValueForKey(KeyAlbumArtist);

            // Clear the content reference from the items so that the garbage collector
            // can free it - there appears to be a leak with the ImageBrowserView where
            // these items never reach a ref count of 0
            for (int i=0 ; i<iItems.Count ; i++)
            {
                iItems[i].CastTo<BrowserDataItem>().ClearContent();
            }

            iContent = null;
            iItems.Release();
            iItems = new NSMutableArray();

            iSelectionIndices.Release();
            iSelectionIndices = new NSIndexSet();

            iItemIdToSelect = aItemIdToSelect;

            DidChangeValueForKey(KeyItems);
            DidChangeValueForKey(KeySelectionIndices);
            DidChangeValueForKey(KeyAlbumArtwork);
            DidChangeValueForKey(KeyAlbumTitle);
            DidChangeValueForKey(KeyAlbumArtist);
        }

        public void SetContent(ContainerContent aContent)
        {
            WillChangeValueForKey(KeyItems);
            WillChangeValueForKey(KeySelectionIndices);
            WillChangeValueForKey(KeyAlbumArtwork);
            WillChangeValueForKey(KeyAlbumTitle);
            WillChangeValueForKey(KeyAlbumArtist);

            // Clear the content reference from the items so that the garbage collector
            // can free it - there appears to be a leak with the ImageBrowserView where
            // these items never reach a ref count of 0
            for (int i=0 ; i<iItems.Count ; i++)
            {
                iItems[i].CastTo<BrowserDataItem>().ClearContent();
            }

            iContent = aContent;
            iItems.Release();
            iItems = new NSMutableArray();

            iSelectionIndices.Release();
            iSelectionIndices = new NSIndexSet();

            if (aContent != null)
            {
                for (int i=0 ; i<iContent.Count ; i++)
                {
                    BrowserDataItem item = new BrowserDataItem(i, iContent);
                    iItems.AddObject(item);
                    item.Release();
                }
            }

            DidChangeValueForKey(KeyItems);
            DidChangeValueForKey(KeySelectionIndices);
            DidChangeValueForKey(KeyAlbumArtwork);
            DidChangeValueForKey(KeyAlbumTitle);
            DidChangeValueForKey(KeyAlbumArtist);
        }

        public int RefreshItemsNotify(int aStartIndex, int aCount)
        {
            // signal that a range of items are about to be updated
            NSRange range = new NSRange((uint)aStartIndex, (uint)aCount);
            NSIndexSet indexSet = new NSIndexSet(range);
            WillChangeValuesAtIndexesForKey(NSKeyValueChange.NSKeyValueChangeReplacement, indexSet, KeyItems);

            int selectedItem = RefreshItems(aStartIndex, aCount);

            // signal that items have changed
            DidChangeValuesAtIndexesForKey(NSKeyValueChange.NSKeyValueChangeReplacement, indexSet, KeyItems);
            indexSet.Release();

            if (selectedItem != -1)
            {
                WillChangeValueForKey(KeySelectionIndices);
                DidChangeValueForKey(KeySelectionIndices);
            }

            return selectedItem;
        }

        public int RefreshItems(int aStartIndex, int aCount)
        {
            // update the items
            int selectedItem = -1;

            for (int i=0 ; i<aCount ; i++)
            {
                BrowserDataItem item = iItems[aStartIndex + i].CastTo<BrowserDataItem>();
                item.Refresh();

                if (!string.IsNullOrEmpty(iItemIdToSelect) && iItemIdToSelect == iContent.Object(aStartIndex + i).Id)
                {
                    selectedItem = aStartIndex + i;
                }
            }

            // update the selection
            if (selectedItem != -1)
            {
                iSelectionIndices.Release();
                iSelectionIndices = new NSIndexSet((uint)selectedItem);
            }

            return selectedItem;
        }

        private void ArtworkCacheUpdated(object sender, ArtworkCache.EventArgsArtwork e)
        {
            // only need to handle changes to the artwork in album mode (not artwork of the data in the lists)
            WillChangeValueForKey(KeyAlbumArtwork);
            DidChangeValueForKey(KeyAlbumArtwork);
        }

        private ContainerContent iContent;
        private NSMutableArray iItems = new NSMutableArray();
        private NSIndexSet iSelectionIndices = new NSIndexSet();
        private string iItemIdToSelect;
    }


    // Data source item for a single item in the browser content views
    [ObjectiveCClass]
    public class BrowserDataItem : NSObject, IDataTrackItem, INSCopying
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(BrowserDataItem));

        public BrowserDataItem() : base() {}
        public BrowserDataItem(IntPtr aInstance) : base(aInstance) {}

        public BrowserDataItem(int aIndex, ContainerContent aContent)
            : base()
        {
            iIndex = aIndex;
            iContent = aContent;
            iImageUid = iContent.GetHashCode().ToString() + iIndex.ToString();

            iVersion = 0;
        }

        public void ClearContent()
        {
            iContent = null;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            this.SendMessageSuper(ThisClass, "dealloc");
        }

        public void Refresh()
        {
            iVersion++;
        }

        public upnpObject UpnpObject
        {
            get
            {
                return (iContent != null) ? iContent.Object(iIndex) : null;
            }
        }

        #region NSTableView bindings
        public NSImage Image
        {
            [ObjectiveCMessage("image")]
            get
            {
                upnpObject o = (iContent != null) ? iContent.Object(iIndex) : null;
                return BrowserImages.GetImage(o);
            }
        }

        public Id TrackItem
        {
            [ObjectiveCMessage("trackItem")]
            get
            {
                return new WrappedDataTrackItem(this);
            }
        }

        public NSString Duration
        {
            [ObjectiveCMessage("duration")]
            get
            {
                upnpObject o = (iContent != null) ? iContent.Object(iIndex) : null;
                if (o != null)
                {
                    return NSString.StringWithUTF8String(DidlLiteAdapter.Duration(o));
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion NSTableView bindings


        #region IDataTrackItem implementation
        public bool IsGroup
        {
            get { return false; }
        }
        public int Index
        {
            get { return iIndex; }
        }
        public string Title
        {
            get
            {
                upnpObject o = (iContent != null) ? iContent.Object(iIndex) : null;
                if (o != null)
                {
                    return DidlLiteAdapter.Title(o);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Subtitle1
        {
            get
            {
                upnpObject o = (iContent != null) ? iContent.Object(iIndex) : null;
                string albumArtist = DidlLiteAdapter.AlbumArtist(o);
                string artist = DidlLiteAdapter.Artist(o);
                if (!string.IsNullOrEmpty(albumArtist) && albumArtist != artist)
                {
                    return artist;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Subtitle2
        {
            get { return string.Empty; }
        }
        public string TrackNumber
        {
            get
            {
                upnpObject o = (iContent != null) ? iContent.Object(iIndex) : null;
                return DidlLiteAdapter.OriginalTrackNumber(o);
            }
        }
        #endregion IDataTrackItem implementation


        #region IKImageBrowserItem protocol implementation
        [ObjectiveCMessage("imageRepresentationType")]
        public NSString ImageRepresentationType()
        {
            return RepresentationType;
        }

        [ObjectiveCMessage("imageRepresentation")]
        public Id ImageRepresentation()
        {
            return Image;
        }

        [ObjectiveCMessage("imageUID")]
        public NSString ImageUID()
        {
            //return NSString.StringWithUTF8String(this.GetHashCode().ToString() + iVersion.ToString());
            return NSString.StringWithUTF8String(iImageUid + iVersion.ToString());
        }

        [ObjectiveCMessage("imageTitle")]
        public NSString ImageTitle()
        {
            return NSString.StringWithUTF8String(Title);
        }

        [ObjectiveCMessage("imageVersion")]
        public uint ImageVersion()
        {
            return iVersion;
        }
        #endregion IKImageBrowserItem protocol implementation

        #region INSCopying implementation
        [ObjectiveCMessage("copyWithZone:")]
        public Id CopyWithZone(IntPtr aZone)
        {
            BrowserDataItem copy = new BrowserDataItem();
            copy.iVersion = iVersion;
            copy.iIndex = iIndex;
            copy.iContent = iContent;
            return copy;
        }
        #endregion INSCopying implementation

        private static NSString RepresentationType = new NSString("IKImageBrowserNSImageRepresentationType");

        private uint iVersion;
        private int iIndex;
        private ContainerContent iContent;
        private string iImageUid;
    }
}



