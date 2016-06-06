
using System;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    // File's owner of the ViewSelectionSource.nib file
    [ObjectiveCClass]
    public class ViewSelectionSource : NSViewController, IViewPopover
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewSelectionSource));

        public ViewSelectionSource() : base() {}
        public ViewSelectionSource(IntPtr aInstance) : base(aInstance) {}

        public ViewSelectionSource(IModelSelectionList<Linn.Kinsky.Source> aModel)
            : base()
        {
            iModel = aModel;
            NSBundle.LoadNibNamedOwner("ViewSelectionSource.nib", this);
        }


        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // set appearance of view
            TextFieldTitle.TextColor = NSColor.WhiteColor;
            TextFieldTitle.Font = FontManager.FontLarge;

            ViewTable.RowHeight = 60.0f;
            ViewTable.BackgroundColor = NSColor.ClearColor;

            ButtonRefresh.Image = Properties.Resources.IconRefreshButton;
            ButtonRefresh.Frame = new NSRect(ButtonRefresh.Frame.origin.x, ButtonRefresh.Frame.origin.y, 21, 21);
            ButtonRefresh.ActionEvent += RefreshClick;

            Hourglass.Show(false);
            Hourglass.Frame = new NSRect(ButtonRefresh.Frame.origin.x - 4, ButtonRefresh.Frame.origin.y - 4, 30, 30);

            // setup model eventing
            iModel.EventChanged += ModelChanged;

            ModelChanged(this, EventArgs.Empty);

            if (SelectedIndex != -1)
            {
                ViewTable.ScrollRowToVisible(SelectedIndex);
            }

            // setup delegate - if this is done in IB, the methods can be called
            // before awakeFromNib which causes complications
            ViewTable.Delegate = this;
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iModel.EventChanged -= ModelChanged;

            StopRefresh();
            ButtonRefresh.ActionEvent -= RefreshClick;

            View.Release();
            ArrayController.Release();

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        private void RefreshClick(Id aSender)
        {
            StartRefresh();
        }

        private void StartRefresh()
        {
            StopRefresh();
            iRefreshTimer = new System.Threading.Timer(TimerCallback);
            ButtonRefresh.IsHidden = true;
            Hourglass.Show(true);
            iRefreshTimer.Change(kRefreshTimeout, System.Threading.Timeout.Infinite);
            ModelMain.Instance.Rescan();
        }

        private void TimerCallback(object aSender)
        {
            ButtonRefresh.BeginInvoke((Action)(()=>{
                StopRefresh();
            }));
        }

        private void StopRefresh()
        {
            if (iRefreshTimer != null)
            {
                iRefreshTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                iRefreshTimer.Dispose();
                iRefreshTimer = null;
                ButtonRefresh.IsHidden = false;
                Hourglass.Show(false);
            }
        }


        #region IViewPopover implementation
        // no need to implement View and Release since the base class already implements them
        public event EventHandler<EventArgs> EventClose;
        #endregion IViewPopover implementation


        #region NSTableView delegate functions
        [ObjectiveCMessage("tableView:willDisplayCell:forTableColumn:row:")]
        public void TableViewWillDisplayCell(NSTableView aView, NSCell aCell, NSTableColumn aTableColumn, int aRow)
        {
            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(NSString.StringWithUTF8String("selected")) == NSComparisonResult.NSOrderedSame)
            {
                NSImageCell cell = aCell.CastTo<NSImageCell>();
                if (aRow == SelectedIndex)
                {
                    cell.Image = Properties.Resources.IconTick;
                    cell.IsEnabled = true;
                }
                else
                {
                    cell.Image = null;
                    cell.IsEnabled = false;
                }
            }
            else
            {
                aCell.RepresentedObject = iSources.ObjectAtIndex((uint)aRow);
            }
        }

        [ObjectiveCMessage("tableView:didClickCellAtColumn:row:")]
        public void TableViewDidClickRow(NSTableView aView, int aCol, int aRow)
        {
            // ignore clicks on the table background
            if (aRow == -1)
            {
                return;
            }

            // extended delegate function that is called on a mouse up - see
            // the TableViewClickable implementation
            SourceData data = iSources.ObjectAtIndex((uint)aRow).CastAs<SourceData>();

            if (SelectedIndex != aRow)
            {
                // unselected row clicked
                iModel.SelectedItem = data.Source;
            }

            // always close the popover
            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }
        #endregion NSTableView delegate functions


        #region NSArrayController bindings
        public NSMutableArray Sources
        {
            [ObjectiveCMessage("sources")]
            get { return iSources; }
        }

        public NSIndexSet SelectionIndices
        {
            [ObjectiveCMessage("selectionIndices")]
            get
            {
                NSIndexSet selection = new NSIndexSet();
                selection.Autorelease();
                return selection;
            }
            [ObjectiveCMessage("setSelectionIndices:")]
            set
            {
            }
        }
        #endregion NSArrayController bindings


        private void ModelChanged(object sender, EventArgs e)
        {
            // clear existing source list
            iSources.Release();
            iSources = new NSMutableArray();

            // add current list of sources
            foreach (Linn.Kinsky.Source source in iModel.Items)
            {
                SourceData data = new SourceData(source);
                iSources.AddObject(data);
                data.Release();
            }

            // notify that there has been a change in the source list **before** the change in selection
            WillChangeValueForKey(NSString.StringWithUTF8String("sources"));
            DidChangeValueForKey(NSString.StringWithUTF8String("sources"));

            WillChangeValueForKey(NSString.StringWithUTF8String("selectionIndices"));
            DidChangeValueForKey(NSString.StringWithUTF8String("selectionIndices"));
        }

        private int SelectedIndex
        {
            get
            {
                if (iModel.SelectedItem != null)
                {
                    for (uint i=0 ; i<iSources.Count ; i++)
                    {
                        SourceData data = iSources.ObjectAtIndex(i).CastAs<SourceData>();
                        if (data.Source == iModel.SelectedItem)
                        {
                            return (int)i;
                        }
                    }
                }

                return -1;
            }
        }

        [ObjectiveCField]
        public NSArrayController ArrayController;

        [ObjectiveCField]
        public NSTextField TextFieldTitle;

        [ObjectiveCField]
        public NSTableView ViewTable;

        [ObjectiveCField]
        public NSButton ButtonRefresh;

        [ObjectiveCField]
        public ViewHourglass Hourglass;

        private IModelSelectionList<Linn.Kinsky.Source> iModel;
        private NSMutableArray iSources = new NSMutableArray();
        private System.Threading.Timer iRefreshTimer;
        private const int kRefreshTimeout = 5000;
    }


    // Class for the data bindings
    [ObjectiveCClass]
    public class SourceData : NSObject
    {
        public SourceData() : base() {}
        public SourceData(IntPtr aInstance) : base(aInstance) {}

        public SourceData(Linn.Kinsky.Source aSource)
            : base()
        {
            iSource = aSource;
        }

        public NSImage Image
        {
            [ObjectiveCMessage("image")]
            get
            {
                return SourceImages.GetImage(iSource.Type);
            }
        }

        public NSString Name
        {
            [ObjectiveCMessage("name")]
            get
            {
                return NSString.StringWithUTF8String(iSource.Name);
            }
        }

        public Linn.Kinsky.Source Source
        {
            get { return iSource; }
        }

        private Linn.Kinsky.Source iSource;
    }


    // Cell class for items in the table view
    [ObjectiveCClass]
    public class SourceSelectionTableCell : TextFieldCellCentred
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(SourceSelectionTableCell));

        public SourceSelectionTableCell() : base() {}
        public SourceSelectionTableCell(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawWithFrame:inView:")]
        public void Draw(NSRect aCellFrame, NSView aControlView)
        {
            this.TextColor = NSColor.WhiteColor;
            this.Font = FontManager.FontSemiLarge;

            NSRect imageFrame = new NSRect(aCellFrame.MinX + 3, aCellFrame.MinY + 3, aCellFrame.Height - 6, aCellFrame.Height - 6);
            NSRect textFrame = new NSRect(imageFrame.MaxX + 6, imageFrame.MinY, aCellFrame.Width - aCellFrame.MinX - aCellFrame.Height - 6, aCellFrame.Height - 6);

            SourceData data = this.RepresentedObject.CastAs<SourceData>();

            if (data.Image != null)
            {
                imageFrame = ImageRect(data.Image, imageFrame);
                data.Image.SendMessage("drawInRect:fromRect:operation:fraction:respectFlipped:hints:", imageFrame, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f, true, null);
            }

            this.SendMessageSuper(ThisClass, "drawWithFrame:inView:", textFrame, aControlView);
        }

        private NSRect ImageRect(NSImage aImage, NSRect aRect)
        {
            if(aImage.Size.width > aImage.Size.height)
            {
                float height = aRect.Width * (aImage.Size.height / aImage.Size.width);
                float offset = (aRect.Height - height) * 0.5f;
                return new NSRect(aRect.MinX, aRect.MinY + offset, aRect.Width, height);
            }
            else if(aImage.Size.height > aImage.Size.width)
            {
                float width = aRect.Height * (aImage.Size.width / aImage.Size.height);
                float offset = (aRect.Width - width) * 0.5f;
                return new NSRect(aRect.MinX + offset, aRect.MinY, width, aRect.Height);
            }

            return aRect;
        }
    }
}



