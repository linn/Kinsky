
using System;

using Monobjc;
using Monobjc.Cocoa;

using Linn;
using Linn.Kinsky;


namespace KinskyDesktop
{
    // File's owner of the ViewSelectionRoom.nib file
    [ObjectiveCClass]
    public class ViewSelectionRoom : NSViewController, IViewPopover
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewSelectionRoom));

        public ViewSelectionRoom() : base() {}
        public ViewSelectionRoom(IntPtr aInstance) : base(aInstance) {}

        public ViewSelectionRoom(IModelSelectionList<Linn.Kinsky.Room> aModel)
            : base()
        {
            iModel = aModel;
            NSBundle.LoadNibNamedOwner("ViewSelectionRoom.nib", this);
        }


        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // set appearance of view
            TextFieldTitle.TextColor = NSColor.WhiteColor;
            TextFieldTitle.Font = FontManager.FontLarge;

            ViewTable.RowHeight = 60.0f;
            ViewTable.BackgroundColor = NSColor.ClearColor;

            ButtonStandbyAll.ActionEvent += StandbyAllClick;
            ButtonStandbyAll.Frame = new NSRect(ButtonStandbyAll.Frame.origin.x, ButtonStandbyAll.Frame.origin.y, 23, 23);

            ButtonRefresh.Image = Properties.Resources.IconRefreshButton;
            ButtonRefresh.Frame = new NSRect(ButtonRefresh.Frame.origin.x, ButtonRefresh.Frame.origin.y, 21, 21);
            ButtonRefresh.ActionEvent += RefreshClick;

            Hourglass.Show(false);
            Hourglass.Frame = new NSRect(ButtonRefresh.Frame.origin.x - 4, ButtonRefresh.Frame.origin.y - 4, 30, 30);

            NSTableColumn nameColumn = ViewTable.TableColumns[1].CastAs<NSTableColumn>();
            TextFieldCellCentred nameCell = nameColumn.DataCell.CastAs<TextFieldCellCentred>();
            nameCell.TextColor = NSColor.WhiteColor;
            nameCell.Font = FontManager.FontSemiLarge;

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

            ButtonStandbyAll.ActionEvent -= StandbyAllClick;
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

        private void StandbyAllClick(Id aSender)
        {
            foreach(Room r in iModel.Items)
            {
                if (!r.Standby)
                {
                    r.Standby = true;
                }
            }
            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }


        #region IViewPopover implementation
        // no need to implement View and Release since the base class already implements them
        public event EventHandler<EventArgs> EventClose;
        #endregion IViewPopover implementation


        #region NSTableView delegate functions
        [ObjectiveCMessage("tableView:willDisplayCell:forTableColumn:row:")]
        public void TableViewWillDisplayCell(NSTableView aView, Id aCell, NSTableColumn aTableColumn, int aRow)
        {
            NSString identifier = aTableColumn.Identifier.CastTo<NSString>();

            if (identifier.Compare(NSString.StringWithUTF8String("standby")) == NSComparisonResult.NSOrderedSame)
            {
                NSButtonCell cell = aCell.CastTo<NSButtonCell>();

                Room r = iModel.Items[aRow];
                cell.Image = r.Standby ?  Properties.Resources.IconStandby : Properties.Resources.IconStandbyOn;
                cell.AlternateImage = r.Standby ? Properties.Resources.IconStandby : Properties.Resources.IconStandbyOn;
                cell.IsEnabled = !r.Standby;

            }
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
        }

        [ObjectiveCMessage("tableView:didClickCellAtColumn:row:")]
        public void TableViewDidClickRow(NSTableView aView, int aCol, int aRow)
        {
            // extended delegate function that is called on a mouse up - see
            // the TableViewClickable implementation below
            bool close = true;
            if (aRow == -1)
            {
                // ignore clicks on the table background
                return;
            }
            else if (aCol == 0)
            {
                Room r = iModel.Items[aRow];
                if (!r.Standby)
                {
                    close = false;
                    r.Standby = true;
                }
            }
            if (close && SelectedIndex != aRow)
            {
                // unselected room clicked
                iModel.SelectedItem = iModel.Items[aRow];
            }

            // close the popover if not a standby button click
            if (close && EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }
        #endregion NSTableView delegate functions


        #region NSArrayController bindings
        public NSMutableArray Rooms
        {
            [ObjectiveCMessage("rooms")]
            get { return iRooms; }
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
            // clear existing room list
            iRooms.Release();
            iRooms = new NSMutableArray();

            bool roomsOutOfStandby = false;

            // add current list of rooms
            foreach (Linn.Kinsky.Room room in iModel.Items)
            {
                if (!room.Standby)
                {
                    roomsOutOfStandby = true;
                }
                RoomData data = new RoomData();
                data.SetName(room.Name);

                iRooms.AddObject(data);
                data.Release();
            }

            ButtonStandbyAll.IsEnabled = roomsOutOfStandby;
            ButtonStandbyAll.Image = roomsOutOfStandby ? Properties.Resources.IconStandbyOn : Properties.Resources.IconStandby;

            // notify that there has been a change in the room list **before** the change in selection
            WillChangeValueForKey(NSString.StringWithUTF8String("rooms"));
            DidChangeValueForKey(NSString.StringWithUTF8String("rooms"));

            WillChangeValueForKey(NSString.StringWithUTF8String("selectionIndices"));
            DidChangeValueForKey(NSString.StringWithUTF8String("selectionIndices"));
        }

        private int SelectedIndex
        {
            get
            {
                return (iModel.SelectedItem != null) ? iModel.Items.IndexOf(iModel.SelectedItem) : -1;
            }
        }


        [ObjectiveCField]
        public NSTableView ViewTable;

        [ObjectiveCField]
        public NSTextField TextFieldTitle;

        [ObjectiveCField]
        public NSArrayController ArrayController;

        [ObjectiveCField]
        public NSButton ButtonStandbyAll;

        [ObjectiveCField]
        public NSButton ButtonRefresh;

        [ObjectiveCField]
        public ViewHourglass Hourglass;

        private IModelSelectionList<Linn.Kinsky.Room> iModel;
        private NSMutableArray iRooms = new NSMutableArray();
        private System.Threading.Timer iRefreshTimer;
        private const int kRefreshTimeout = 5000;
    }


    // Class for the data bindings
    [ObjectiveCClass]
    public class RoomData : NSObject
    {
        public RoomData() : base() {}
        public RoomData(IntPtr aInstance) : base(aInstance) {}

        public NSString Name
        {
            [ObjectiveCMessage("name")]
            get { return NSString.StringWithUTF8String(iName); }
        }

        public void SetName(string aName)
        {
            iName = aName;
        }

        private string iName = "asdassda";
    }
}



