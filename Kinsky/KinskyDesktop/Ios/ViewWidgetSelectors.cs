using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Linn;
using Linn.Topology;
using Linn.Kinsky;
using CoreGraphics;

namespace KinskyTouch
{
    internal class ViewWidgetSelectorPopover<T> : UIPopoverControllerDelegate, IViewWidgetSelector<T> where T : class
    {
        public ViewWidgetSelectorPopover(HelperKinsky aHelper, UITableViewController aController, IViewWidgetSelector<T> aSelector, UIBarButtonItem aButtonItemOpen, UIBarButtonItem aButtonItemCancel)
        {
            iHelper = aHelper;
            iButton = aButtonItemOpen;
            iDefaultTitle = iButton.Title;

            iController = aController;

            aButtonItemOpen.Clicked += OpenClicked;
            aButtonItemCancel.Clicked += CancelClicked;

            aSelector.EventSelectionChanged += SelectionChanged;
			iItems = new List<T>();
        }

        public void Open()
        {
			UpdateStandbyAllButtonState();
        }

        public void Close()
        {
        }

        public void InsertItem(int aIndex, T aItem)
        {
			iItems.Add(aItem);
			UpdateStandbyAllButtonState();
        }

        public void RemoveItem(T aItem)
        {
			iItems.Remove(aItem);
			UpdateStandbyAllButtonState();
        }

        public void ItemChanged(T aItem)
        {
            if(aItem == iSelectedItem)
            {
                SetSelected(aItem);
            }
			UpdateStandbyAllButtonState();
        }

        public void SetSelected(T aItem)
        {
            if(aItem is Linn.Kinsky.Room)
            {
                Linn.Kinsky.Room room = aItem as Linn.Kinsky.Room;
                iButton.Title = room.Name;
            }
            else if(aItem is Linn.Kinsky.Source)
            {
                Linn.Kinsky.Source source = aItem as Linn.Kinsky.Source;
                iButton.Title = String.Format("{0}", source.Name);
            }
            else
            {
                iButton.Title = iDefaultTitle;
                Dismiss();
            }

            iSelectedItem = aItem;
        }

        public event EventHandler<EventArgsSelection<T>> EventSelectionChanged;

        public override void DidDismiss(UIPopoverController aPopoverController)
        {
            Dismiss();
        }

        private void Dismiss()
        {
            if(iPopover != null)
            {
                iRefreshButton.Clicked -= RefreshClicked;
                iRefreshButton.Dispose();
                iRefreshButton = null;
				
				if (iButtonStandbyAll != null)
				{
					iButtonStandbyAll.TouchUpInside -= StandbyAllClicked;
					iButtonStandbyAll.Dispose();
					iButtonStandbyAll = null;
				}
    
                iPopover.Dismiss(true);
                iPopover.Dispose();
                iPopover = null;
            }
        }

        private void OpenClicked(object sender, EventArgs e)
        {
            if(iPopover == null)
            {
				iController.PreferredContentSize = new CGSize(300, 500);
                UINavigationController navigationController = new UINavigationController(iController);
                iRefreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh);
                navigationController.NavigationBar.TopItem.RightBarButtonItem = iRefreshButton;
				if (typeof(T) == typeof(Linn.Kinsky.Room))
				{
					iButtonStandbyAll = new UIButton(new CGRect(0, 0, 35, 30));
            		iButtonStandbyAll.SetImage(new UIImage("Standby.png"), UIControlState.Normal);
            		iButtonStandbyAll.SetImage(new UIImage("StandbyDown.png"), UIControlState.Highlighted);
            		iButtonStandbyAll.SetImage(new UIImage("StandbyOn.png"), UIControlState.Selected);
					//iButtonStandbyAll.SetBackgroundImage(new UIImage("UIBarButton.png"), UIControlState.Normal);
					iButtonStandbyAll.TouchUpInside += StandbyAllClicked;
					
					UIBarButtonItem barButton = new UIBarButtonItem();
					barButton.CustomView = iButtonStandbyAll;
					navigationController.NavigationBar.TopItem.LeftBarButtonItem = barButton;
					UpdateStandbyAllButtonState();
				}
                iPopover = new UIPopoverController(navigationController);
                iPopover.Delegate = this;

				iController.View.BackgroundColor = UIColor.Clear;
				navigationController.View.BackgroundColor = UIColor.Clear;
				navigationController.NavigationBar.BarStyle = UIBarStyle.BlackTranslucent;

				iPopover.BackgroundColor = UIColor.Clear;//new UIColor(0.10f, 0.10f, 0.10f, 1.0f); //UIColor.Black;

                iRefreshButton.Clicked += RefreshClicked;

                navigationController.PopToRootViewController(false);
                //iPopover.SetPopoverContentSize(new SizeF(320, 600), true);
                iPopover.PresentFromBarButtonItem(iButton, UIPopoverArrowDirection.Any, true);
            }
            else
            {
                Dismiss();
            }
        }

        private void StandbyAllClicked (object sender, EventArgs e)
        {
			if (iButtonStandbyAll.Selected)
			{
				foreach (T room in iItems)
				{
					(room as Linn.Kinsky.Room).Standby = true;	
				}
				Dismiss();
			}
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void SelectionChanged(object sender, EventArgsSelection<T> e)
        {
            if(iPopover != null)
            {
                Dismiss();
            }
        }

        private void RefreshClicked (object sender, EventArgs e)
		{
			iHelper.Rescan ();

			UIActivityIndicatorView view = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.White);
            //view.BackgroundColor = UIColor.Clear;
			view.Frame = new CGRect(0.0f, 0.0f, 25.0f, 25.0f);

			UIBarButtonItem item = new UIBarButtonItem ();
			item.CustomView = view;

			UINavigationController navigationController = iPopover.ContentViewController as UINavigationController;
			if (navigationController.ViewControllers.Length > 0)
			{
				navigationController.ViewControllers[0].NavigationItem.RightBarButtonItem = item;

				view.StartAnimating();

				new System.Threading.Timer(delegate {
					BeginInvokeOnMainThread(delegate {
						view.StopAnimating();

						if (iRefreshButton != null && navigationController.ViewControllers.Length > 0)
						{
							navigationController.ViewControllers[0].NavigationItem.RightBarButtonItem = iRefreshButton;
						}
					});
				}, null, 3000, System.Threading.Timeout.Infinite);
			}
        }
		
		private void UpdateStandbyAllButtonState()
		{
			if (iButtonStandbyAll != null)
			{
				bool selected = false;
				foreach (T room in iItems)
				{
					if (!(room as Linn.Kinsky.Room).Standby)
					{
						selected = true;
						break;
					}
				}
				iButtonStandbyAll.Selected = selected;
			}
		}

        private string iDefaultTitle;
        private T iSelectedItem;
        private HelperKinsky iHelper;

        private UIBarButtonItem iButton;
        private UIBarButtonItem iRefreshButton;
        private UITableViewController iController;
        private UIPopoverController iPopover;
		private UIButton iButtonStandbyAll;
		private List<T> iItems;
    }

    internal class ViewWidgetSelectorRoomNavigation : IViewWidgetSelector<Linn.Kinsky.Room>, IControllerRoomSelector
    {
        private class Delegate : UINavigationControllerDelegate
        {
            public Delegate(IControllerRoomSelector aControllerRoomSelector, ViewWidgetSelectorSource aViewWidgetSelectorSource)
            {
                iControllerRoomSelector = aControllerRoomSelector;
                iViewWidgetSelectorSource = aViewWidgetSelectorSource;
            }

            public override void WillShowViewController(UINavigationController aNavigationController, UIViewController aViewController, bool aAnimated)
            {
                if(aNavigationController.ViewControllers[0] == aNavigationController.TopViewController)
                {
                    iViewWidgetSelectorSource.Title = string.Empty;
                    iControllerRoomSelector.Select(null);
                }
            }

            private IControllerRoomSelector iControllerRoomSelector;
            private ViewWidgetSelectorSource iViewWidgetSelectorSource;
        }

        public ViewWidgetSelectorRoomNavigation(HelperKinsky aHelper, UINavigationController aNavigationController, UIScrollView aScrollView, ViewWidgetSelectorSource aViewWidgetSelectorSource, UIBarButtonItem aButtonRefresh, UIButton aButtonStandby, UIButton aButtonStandbyAll)
        {
            iHelper = aHelper;
			
			iRooms = new List<Linn.Kinsky.Room>();

            iNavigationController = aNavigationController;
            iNavigationController.Delegate = new Delegate(this, aViewWidgetSelectorSource);

            iScrollView = aScrollView;
            iViewWidgetSelectorSource = aViewWidgetSelectorSource;

            iButtonRefresh = aButtonRefresh;
            iNavigationController.ViewControllers[0].NavigationItem.RightBarButtonItem = iButtonRefresh;
            iNavigationController.NavigationBar.BackgroundColor = UIColor.Black;
						
			iButtonStandbyAll = aButtonStandbyAll;
			UIBarButtonItem barButton = new UIBarButtonItem(iButtonStandbyAll);
            iNavigationController.ViewControllers[0].NavigationItem.LeftBarButtonItem = barButton;
			UpdateStandbyAllButtonState();

            iButtonStandby = aButtonStandby;
			UIBarButtonItem barButton2 = new UIBarButtonItem(iButtonStandby);
            iViewWidgetSelectorSource.NavigationItem.RightBarButtonItem = barButton2;

            iViewWidgetSelectorSource.EventSelectionChanged += SelectionChanged;
            iButtonRefresh.Clicked += RefreshClicked;
            iButtonStandby.TouchUpInside += StandbyClicked;
			iButtonStandbyAll.TouchUpInside += StandbyAllClicked;
        }

        public void Select(Linn.Kinsky.Room aRoom)
        {
            if(EventSelectionChanged != null)
            {
                EventSelectionChanged(this, new EventArgsSelection<Linn.Kinsky.Room>(aRoom));
            }
        }

        public void Open()
        {
			iRooms.Clear();
			UpdateStandbyAllButtonState();
        }

        public void Close()
        {
            iNavigationController.PopToRootViewController(true);
        }

        public void InsertItem(int aIndex, Linn.Kinsky.Room aItem)
        {
			iRooms.Add(aItem);
			UpdateStandbyAllButtonState();
        }

        public void RemoveItem(Linn.Kinsky.Room aItem)
        {
			iRooms.Remove(aItem);
			UpdateStandbyAllButtonState();
        }

        public void ItemChanged(Linn.Kinsky.Room aItem)
		{
			UpdateStandbyAllButtonState();
        }

        public void SetSelected(Linn.Kinsky.Room aItem)
        {
            if(aItem != null)
            {
                if(iNavigationController.ViewControllers.Length == 1)
                {
                    iNavigationController.PushViewController(iViewWidgetSelectorSource, true);
                }
                iViewWidgetSelectorSource.Title = aItem.Name;
            }
			else
			{
				if (iNavigationController.ViewControllers.Length > 1)
				{
					iNavigationController.PopToRootViewController(true);	
				}
			}
			iRoom = aItem;
        }

        public event EventHandler<EventArgsSelection<Linn.Kinsky.Room>> EventSelectionChanged;

        private void SelectionChanged(object sender, EventArgsSelection<Linn.Kinsky.Source> e)
        {
            CGRect rect = iScrollView.Frame;
            rect.Offset(rect.Width * 1, 0);
            iScrollView.ScrollRectToVisible(rect, true);
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            iHelper.Rescan();

            UIActivityIndicatorView view = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
            view.Frame = new CGRect(0.0f, 0.0f, 25.0f, 25.0f);

            UIBarButtonItem item = new UIBarButtonItem();
            item.CustomView = view;

            iNavigationController.ViewControllers[0].NavigationItem.RightBarButtonItem = item;

            view.StartAnimating();

            new System.Threading.Timer(delegate {
                iButtonRefresh.BeginInvokeOnMainThread(delegate {
                    view.StopAnimating();
                    iNavigationController.ViewControllers[0].NavigationItem.RightBarButtonItem = iButtonRefresh;
                });
            }, null, 3000, System.Threading.Timeout.Infinite);
        }

        private void StandbyClicked(object sender, EventArgs e)
        {
			if (iRoom != null)
			{
				iRoom.Standby = true;	
			}
            iNavigationController.PopToRootViewController(true);
        }
		
		private void StandbyAllClicked (object sender, EventArgs e)
        {
			if (iButtonStandbyAll.Selected)
			{
				foreach (Linn.Kinsky.Room room in iRooms)
				{
					room.Standby = true;	
				}
			}
        }
		
		private void UpdateStandbyAllButtonState()
		{
			if (iButtonStandbyAll != null)
			{
				bool selected = false;
				foreach (Linn.Kinsky.Room room in iRooms)
				{
					if (!room.Standby)
					{
						selected = true;
						break;
					}
				}
				iButtonStandbyAll.Selected = selected;
			}
		}

        private HelperKinsky iHelper;

        private UINavigationController iNavigationController;
        private UIScrollView iScrollView;
        private ViewWidgetSelectorSource iViewWidgetSelectorSource;

        private UIBarButtonItem iButtonRefresh;
        private UIButton iButtonStandby;
        private UIButton iButtonStandbyAll;
		private Linn.Kinsky.Room iRoom;
		private List<Linn.Kinsky.Room> iRooms;
    }

    [Foundation.Register("ViewWidgetSelectorRoom")]
    internal class ViewWidgetSelectorRoom : UITableViewController, IViewWidgetSelector<Linn.Kinsky.Room>, IControllerRoomSelector
    {
        private class DataSource : UITableViewDataSource
        {
            public DataSource(UITableView aTableView, int aStandbyButtonOffsetX)
            {
                iTableView = aTableView;
				iStandbyButtonOffsetX = aStandbyButtonOffsetX;
                iRooms = new List<Linn.Kinsky.Room>();
            }

            public void SetRoom(Linn.Kinsky.Room aRoom)
            {
                if(aRoom != iRoom)
                {
                    Linn.Kinsky.Room oldRoom = iRoom;
                    iRoom = aRoom;

                    iTableView.BeginUpdates();

                    int oldIndex = iRooms.IndexOf(oldRoom);
                    if(oldIndex > -1)
                    {
                        NSIndexPath path = NSIndexPath.FromRowSection(oldIndex, 0);
                        UITableViewCell cell = iTableView.CellAt(path);
                        if(cell != null)
                        {
                            cell.AccessoryView = null;
                        }
                        iTableView.ReloadRows(new NSIndexPath[] { path }, UITableViewRowAnimation.Fade);
                    }

                    int newIndex = iRooms.IndexOf(aRoom);
                    if(newIndex > -1)
                    {
                        NSIndexPath path = NSIndexPath.FromRowSection(newIndex, 0);
                        /*UITableViewCell cell = iTableView.CellAt(NSIndexPath.FromRowSection(newIndex, 0));
                        if(cell != null)
                        {
                            cell.AccessoryView = iButtonStandby;
                        }*/
                        iTableView.ReloadRows(new NSIndexPath[] { path }, UITableViewRowAnimation.Fade);
                    }

                    iTableView.EndUpdates();
                }
            }

            public void InsertItem(int aIndex, Linn.Kinsky.Room aRoom)
            {
                iTableView.BeginUpdates();
                iRooms.Insert(aIndex, aRoom);
                iTableView.InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(aIndex, 0) }, UITableViewRowAnimation.Fade);
                iTableView.EndUpdates();
            }

            public void RemoveItem(Linn.Kinsky.Room aRoom)
            {
                int index = iRooms.IndexOf(aRoom);
                iTableView.BeginUpdates();
                iRooms.Remove(aRoom);
                iTableView.DeleteRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
                iTableView.EndUpdates();
            }

            public void Clear()
            {
                iRooms.Clear();
                iTableView.ReloadData();
            }
			
			public void ReloadData()
			{
				iTableView.ReloadData();	
			}

            public NSIndexPath IndexPathFor(Linn.Kinsky.Room aRoom)
            {
                int index = iRooms.IndexOf(aRoom);
                if(index > -1)
                {
                    return NSIndexPath.FromRowSection(index, 0);
                }
                else
                {
                    return null;
                }
            }

            public Linn.Kinsky.Room RoomAt(int aIndex)
            {
                return iRooms[aIndex];
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iRooms.Count;
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellRoom cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellRoom;
                if(cell == null)
                {
                    CellRoomFactory factory = new CellRoomFactory();
                    NSBundle.MainBundle.LoadNib("CellRoom", factory, null);
                    cell = factory.Cell;
					if (iStandbyButtonOffsetX != 0)
					{
						cell.SetStandbyButtonOffsetX(iStandbyButtonOffsetX);	
					}
                }

                Linn.Kinsky.Room room = iRooms[aIndexPath.Row];

                cell.BackgroundView = new UIView();
                cell.BackgroundView.BackgroundColor = UIColor.Clear;
                cell.BackgroundColor = UIColor.Clear;
                cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
                cell.TextLabel.TextColor = UIColor.White;
				cell.Standby = room.Standby;

                cell.Position = room.GetHashCode();//aIndexPath.Row;

                cell.Title = room.Name;
                cell.Accessory = (iRoom == room) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				return cell;
            }

            private UITableView iTableView;
            private List<Linn.Kinsky.Room> iRooms;
            private Linn.Kinsky.Room iRoom;
			private int iStandbyButtonOffsetX;
        }

        private class Delegate : UITableViewDelegate
        {
            public Delegate(IControllerRoomSelector aController, DataSource aDataSource)
            {
                iController = aController;
                iDataSource = aDataSource;
            }
			
            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);
                iController.Select(iDataSource.RoomAt(aIndexPath.Row));
            }

            private IControllerRoomSelector iController;
            private DataSource iDataSource;
        }

        public ViewWidgetSelectorRoom(IntPtr aInstance)
            : base(aInstance)
        {
            iRooms = new List<Linn.Kinsky.Room>();
        }
		
		[Action("standbyTouchUpInside:")]
        public void StandbyTouchUpInside(UIButton aSender)
        {
            foreach (Linn.Kinsky.Room r in iRooms)
            {
                if(r.GetHashCode() == aSender.Tag)
                {
                    if (!r.Standby)
                    {
                        r.Standby = true;
                    }
                    else
                    {
                        Select(r);
                    }
                    break;
                }
            }	
		}

        public ViewWidgetSelectorRoom()
        {
            iRooms = new List<Linn.Kinsky.Room>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = 73.0f;
            TableView.BackgroundColor = UIColor.Clear;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.ShowsHorizontalScrollIndicator = false;
            TableView.IndicatorStyle = UIScrollViewIndicatorStyle.White;

            Title = "Rooms";

            // clean up any old data source and delegate - ViewDidLoad can be called multiple
            // times if iOS purges views under low memory conditions
            if(iDataSource != null)
            {
                iDataSource.Clear();
                iDataSource.Dispose();
                iDataSource = null;
            }
            
            if(iDelegate != null)
            {
                iDelegate.Dispose();
                iDelegate = null;
            }

            // create new data source and delegate
            iDataSource = new DataSource(TableView, StandbyButtonOffsetX);
            TableView.DataSource = iDataSource;
            iDelegate = new Delegate(this, iDataSource);
            TableView.Delegate = new Delegate(this, iDataSource);
            
            for(int i = 0; i < iRooms.Count; ++i)
            {
                iDataSource.InsertItem(i, iRooms[i]);
            }
            
            iDataSource.SetRoom(iRoom);
            
            NSIndexPath path = iDataSource.IndexPathFor(iRoom);
            if(path != null)
            {
                TableView.ScrollToRow(path, UITableViewScrollPosition.Middle, false);
            }
        }
		
		public int StandbyButtonOffsetX { get; set; }

        public void Open()
        {
            iOpen = true;
        }

        public void Close()
        {
            if(iDataSource != null)
            {
                iDataSource.Clear();
            }
            iRooms.Clear();

            iOpen = false;
        }

        public void InsertItem(int aIndex, Linn.Kinsky.Room aItem)
        {
            if(iOpen)
            {
                iRooms.Insert(aIndex, aItem);
                if(iDataSource != null)
                {
                    iDataSource.InsertItem(aIndex, aItem);
                }
            }
        }

        public void RemoveItem(Linn.Kinsky.Room aItem)
        {
            if(iOpen)
            {
                iRooms.Remove(aItem);
                if(iDataSource != null)
                {
                    iDataSource.RemoveItem(aItem);
                }
            }
        }

        public void ItemChanged(Linn.Kinsky.Room aItem)
        {
			if(iDataSource != null)
            {
				iDataSource.ReloadData();
			}
        }

        public void SetSelected(Linn.Kinsky.Room aItem)
        {
            SetLabel(aItem);
        }

        public void Select(Linn.Kinsky.Room aRoom)
        {
            SetLabel(aRoom);

            if(EventSelectionChanged != null)
            {
                EventSelectionChanged(this, new EventArgsSelection<Linn.Kinsky.Room>(aRoom));
            }
        }
        
        public event EventHandler<EventArgsSelection<Linn.Kinsky.Room>> EventSelectionChanged;

        private void SetLabel(Linn.Kinsky.Room aRoom)
        {
            iRoom = aRoom;
            if(iDataSource != null)
            {
                iDataSource.SetRoom(aRoom);
            }
        }

        private static NSString kCellIdentifier = new NSString("CellRoom");

        private bool iOpen;

        private DataSource iDataSource;
        private Delegate iDelegate;
        private List<Linn.Kinsky.Room> iRooms;
        private Linn.Kinsky.Room iRoom;

        //private UITableView iTableView;
        //private UILabel iLabel;
    }

    [Foundation.Register("ViewWidgetSelectorSource")]
    internal class ViewWidgetSelectorSource : UITableViewController, IViewWidgetSelector<Linn.Kinsky.Source>, IControllerSourceSelector
    {
        private class DataSource : UITableViewDataSource
        {
            public DataSource(UITableView aTableView)
            {
                iTableView = aTableView;
                iSources = new List<Linn.Kinsky.Source>();
            }

            public void SetSource(Linn.Kinsky.Source aSource)
            {
                Linn.Kinsky.Source oldSource = iSource;
                iSource = aSource;

                if(oldSource != null)
                {
                    int index = iSources.IndexOf(oldSource);
                    if(index > -1)
                    {
                        iTableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
                    }
                }

                if(aSource != null)
                {
                    int index = iSources.IndexOf(aSource);
                    if(index > -1)
                    {
                        iTableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
                    }
                }
            }

            public void InsertItem(int aIndex, Linn.Kinsky.Source aSource)
            {
                iTableView.BeginUpdates();
                iSources.Insert(aIndex, aSource);
                iTableView.InsertRows(new NSIndexPath[] { NSIndexPath.FromRowSection(aIndex, 0) }, UITableViewRowAnimation.Fade);
                iTableView.EndUpdates();
            }

            public void RemoveItem(Linn.Kinsky.Source aSource)
            {
                int index = iSources.IndexOf(aSource);
                if(index > -1)
                {
                    iTableView.BeginUpdates();
                    iSources.Remove(aSource);
                    iTableView.DeleteRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
                    iTableView.EndUpdates();
                }
            }

            public void ItemChanged(Linn.Kinsky.Source aSource)
            {
                int index = iSources.IndexOf(aSource);
                if(index > -1)
                {
                    iTableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(index, 0) }, UITableViewRowAnimation.Fade);
                }
            }

            public void Clear()
            {
                iSources.Clear();
                iTableView.ReloadData();
            }

            public NSIndexPath IndexPathFor(Linn.Kinsky.Source aSource)
            {
                if(aSource != null)
                {
                    int index = iSources.IndexOf(aSource);
                    if(index > -1)
                    {
                        return NSIndexPath.FromRowSection(index, 0);
                    }
                }

                return null;
            }

            public Linn.Kinsky.Source SourceAt(NSIndexPath aPathIndex)
            {
                return iSources[aPathIndex.Row];
            }

            public override nint RowsInSection(UITableView aTableView, nint aSection)
            {
                return iSources.Count;
            }

            public override UITableViewCell GetCell(UITableView aTableView, NSIndexPath aIndexPath)
            {
                CellDefault cell = aTableView.DequeueReusableCell(kCellIdentifier) as CellDefault;
                if(cell == null)
                {
                    CellDefaultFactory factory = new CellDefaultFactory();
                    NSBundle.MainBundle.LoadNib("CellDefault", factory, null);
                    cell = factory.Cell;
                }

                cell.BackgroundView = new UIView();
                cell.BackgroundView.BackgroundColor = UIColor.Clear;
                cell.BackgroundColor = UIColor.Clear;
                cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
                //cell.TextLabel.TextColor = UIColor.White;

                Linn.Kinsky.Source source = iSources[aIndexPath.Row];
                cell.Title = source.Name;
                cell.Image = GetSourceImage(source);
                cell.Accessory = (iSource == source) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

                return cell;
            }

            private UIImage GetSourceImage(Linn.Kinsky.Source aSource)
            {
                switch(aSource.Type)
                {
                    case Linn.Kinsky.Source.kSourceAux:
                    case Linn.Kinsky.Source.kSourceAnalog:
                    case Linn.Kinsky.Source.kSourceSpdif:
                    case Linn.Kinsky.Source.kSourceToslink:
                        return KinskyTouch.Properties.ResourceManager.SourceExternal;

                    case Linn.Kinsky.Source.kSourceDisc:
                        return KinskyTouch.Properties.ResourceManager.SourceDisc;

                    case Linn.Kinsky.Source.kSourceDs:
                        return KinskyTouch.Properties.ResourceManager.SourcePlaylist;

                    case Linn.Kinsky.Source.kSourceRadio:
                    case Linn.Kinsky.Source.kSourceTuner:
                        return KinskyTouch.Properties.ResourceManager.SourceRadio;

                    case Linn.Kinsky.Source.kSourceUpnpAv:
                        return KinskyTouch.Properties.ResourceManager.SourceUpnpAv;

                    case Linn.Kinsky.Source.kSourceReceiver:
                        return KinskyTouch.Properties.ResourceManager.SourceSongcast;

                    default:
                        return KinskyTouch.Properties.ResourceManager.SourceExternal;
                }
            }

            private UITableView iTableView;

            private List<Linn.Kinsky.Source> iSources;
            private Linn.Kinsky.Source iSource;
        }

        private class Delegate : UITableViewDelegate
        {
            public Delegate(IControllerSourceSelector aController, ViewWidgetSelectorSource.DataSource aDataSource)
            {
                iController = aController;
                iDataSource = aDataSource;
            }

            public override void RowSelected(UITableView aTableView, NSIndexPath aIndexPath)
            {
                aTableView.DeselectRow(aIndexPath, true);
                iController.Select(iDataSource.SourceAt(aIndexPath));
            }

            private IControllerSourceSelector iController;
            private ViewWidgetSelectorSource.DataSource iDataSource;
        }

        public ViewWidgetSelectorSource()
        {
            iSources = new List<Linn.Kinsky.Source>();
        }

        public ViewWidgetSelectorSource(IntPtr aInstance)
            : base(aInstance)
        {
            iSources = new List<Linn.Kinsky.Source>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.SectionHeaderHeight = 42.0f;
            TableView.RowHeight = 73.0f;
            TableView.BackgroundColor = UIColor.Clear;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.ShowsHorizontalScrollIndicator = false;
            TableView.IndicatorStyle = UIScrollViewIndicatorStyle.White;

            Title = "Sources";

            // clean up any old data source and delegate - ViewDidLoad can be called multiple
            // times if iOS purges views under low memory conditions
            if(iDataSource != null)
            {
                iDataSource.Clear();
                iDataSource.Dispose();
                iDataSource = null;
            }
            
            if(iDelegate != null)
            {
                iDelegate.Dispose();
                iDelegate = null;
            }
            
            // create new data source and delegate
            iDataSource = new DataSource(TableView);
            TableView.DataSource = iDataSource;
            iDelegate = new Delegate(this, iDataSource);
            TableView.Delegate = iDelegate;
            
            for(int i = 0; i < iSources.Count; ++i)
            {
                iDataSource.InsertItem(i, iSources[i]);
            }
            iDataSource.SetSource(iSource);
            
            NSIndexPath path = iDataSource.IndexPathFor(iSource);
            if(path != null)
            {
                TableView.ScrollToRow(path, UITableViewScrollPosition.Middle, false);
            }
        }

        public void Open()
        {
            iOpen = true;
        }

        public void Close()
        {
            iSources.Clear();
            if(iDataSource != null)
            {
                iDataSource.Clear();
            }

            iOpen = false;
        }

        public void InsertItem(int aIndex, Linn.Kinsky.Source aItem)
        {
            if(iOpen)
            {
                iSources.Insert(aIndex, aItem);
                if(iDataSource != null)
                {
                    iDataSource.InsertItem(aIndex, aItem);
                }
            }
        }

        public void RemoveItem(Linn.Kinsky.Source aItem)
        {
            if(iOpen)
            {
                iSources.Remove(aItem);
                if(iDataSource != null)
                {
                    iDataSource.RemoveItem(aItem);
                }
            }
        }

        public void ItemChanged(Linn.Kinsky.Source aItem)
        {
            if(iOpen)
            {
                if(iDataSource != null)
                {
                    iDataSource.ItemChanged(aItem);
                }
            }
        }

        public void SetSelected(Linn.Kinsky.Source aItem)
        {
            SetLabel(aItem);
        }

        public void Select(Linn.Kinsky.Source aSource)
        {
            SetLabel(aSource);

            if(EventSelectionChanged != null)
            {
                EventSelectionChanged(this, new EventArgsSelection<Linn.Kinsky.Source>(aSource));
            }
        }

        public event EventHandler<EventArgsSelection<Linn.Kinsky.Source>> EventSelectionChanged;

        private void SetLabel(Linn.Kinsky.Source aSource)
        {
            if(iDataSource != null)
            {
                iDataSource.SetSource(aSource);
            }

            iSource = aSource;
        }

        private static NSString kCellIdentifier = new NSString("CellDefault");

        private bool iOpen;

        private DataSource iDataSource;
        private Delegate iDelegate;

        private List<Linn.Kinsky.Source> iSources;
        private Linn.Kinsky.Source iSource;
    }
}