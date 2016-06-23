
// NOTE that the code in this file should not depend on any specific toolkit i.e. so
// we can easily move from Monobjc to Monomac when it is ready
using System;
using System.Collections.Generic;

using Linn;
using Linn.Kinsky;
using Linn.Topology;

using Upnp;


namespace KinskyDesktop
{
    // Simple geometry structs - defined as partial so that toolkit dependent
    // parts of the code can provide additional methods for conversion between
    // these types and the corresponding toolkit types
    public partial struct Point
    {
        public Point(float aX, float aY)
        {
            X = aX;
            Y = aY;
        }

        public float X;
        public float Y;
    }

    public partial struct Rect
    {
        public Rect(float aX, float aY, float aWidth, float aHeight)
        {
            Origin = new Point(aX, aY);
            Width = aWidth;
            Height = aHeight;
        }

        public Point Origin;
        public float Width;
        public float Height;

        public bool ContainsPoint(Point aPoint)
        {
            return (aPoint.X >= Origin.X && aPoint.X < Origin.X + Width
                 && aPoint.Y >= Origin.Y && aPoint.Y < Origin.Y + Height);
        }

        public bool Intersects(Rect aRect)
        {
            return !((aRect.Origin.X > Origin.X + Width)  || (aRect.Origin.X + aRect.Width < Origin.X) ||
                     (aRect.Origin.Y > Origin.Y + Height) || (aRect.Origin.Y + aRect.Height < Origin.Y));
        }
    }


    // view interface and controller for top-level application tasks
    public interface IViewApp
    {
        ICrashLogDumper CreateCrashLogDumper(IHelper aHelper);
        void ShowAlertPanel(string aTitle, string aMessage);
    }

    public class ControllerApp
    {
        private static readonly string kApiKey = "129c76d1b4043e568d19a9fea8a1f5534cdae703";

        public ControllerApp(IViewApp aViewApp,
                             IInvoker aInvoker,
                             IAppRestartHandler aRestartHandler,
                             Rect aScreenRect)
        {
            // create the helper for the application
            HelperKinskyDesktop helper;
            try
            {
                helper = new HelperKinskyDesktop(new string[] {}, aScreenRect, aInvoker);
                #if DEBUG
                Xamarin.Insights.Initialize(Xamarin.Insights.DebugModeKey, helper.Version, helper.Product);
                #else
                Xamarin.Insights.Initialize(kApiKey, helper.Version, helper.Product);
                #endif

                ICrashLogDumper dumper = aViewApp.CreateCrashLogDumper(helper);
                helper.AddCrashLogDumper(dumper);
                helper.ProcessOptionsFileAndCommandLine();
                Xamarin.Insights.Identify(helper.OptionInstallId.Value, null);
            }
            catch (ArgumentException)
            {
                aViewApp.ShowAlertPanel("Network adapter error", "An error has occured whilst trying to detect available network adapters");
                // replace "return" with throw for the nightly build 2012-03-22
                // return;
                throw;
            }

            Trace.Level = Trace.kKinskyDesktop;

            // create the application model
            ModelMain.Create(helper, aInvoker, aRestartHandler);
            iModel = ModelMain.Instance;
        }

        public void InitialiseMainWindow(IViewMainWindow aViewMainWindow)
        {
            // create a main window helper for initialisation
            MainWindowHelper mainWindowHelper = new MainWindowHelper(iModel, aViewMainWindow);

            // set the splitter fraction - call this before the window is set to correct size
            aViewMainWindow.SplitterFraction = iModel.Helper.SplitterFraction.Native;

            // to make sure internal views are correctly configured, first set the rect of
            // the window to what it would be in normal mode
            aViewMainWindow.Rect = mainWindowHelper.NormalModeRect;

            if (iModel.Helper.KompactMode.Native)
            {
                // set the window to be in kompact mode
                aViewMainWindow.KompactModeEnter(mainWindowHelper.KompactModeRect);
            }
            else
            {
                // set to fullscreen if required
                aViewMainWindow.IsFullscreen = iModel.Helper.Fullscreen.Native;
            }

            // show the main window
            aViewMainWindow.Show(true);

            iNotificationView = new NotificationView (iModel.Helper);
        }

        public void Start(IViewMainWindow aView)
        {
            // start the app model
            iModel.Start();

            // check for no interface
            if (iModel.Helper.Interface.Interface.Status == NetworkInterface.EStatus.eUnconfigured)
            {
                aView.ShowAlertPanel("No network interface has been configured",
                                     "Kinsky's network interface can be configured in the 'Network' section of the Preferences dialog");
            }
        }

        public void Stop()
        {
            iModel.Stop();
        }
        
        public void Pause()
        {
            iModel.Pause();
        }
        
        public void Resume()
        {
            iModel.Resume();
        }

        public void RescanNetwork()
        {
            iModel.Rescan();
        }

        private ModelMain iModel;
        private NotificationView iNotificationView;
    }


    // Class for helping with some calculations regarding window rects when entering and
    // exiting kompact mode
    public class MainWindowHelper
    {
        public MainWindowHelper(ModelMain aModel, IViewMainWindow aView)
        {
            iModel = aModel;
            iView = aView;
        }

        public Rect KompactModeRect
        {
            get
            {
                if (iModel.Helper.KompactMode.Native)
                {
                    // in kompact mode, so return the current rect for the window
                    return new Rect(iModel.Helper.WindowX.Native, iModel.Helper.WindowY.Native,
                                    iModel.Helper.WindowWidth.Native, iView.KompactModeHeight);
                }
                else
                {
                    // not in kompact mode, so return the rect that the window **would** use if kompact
                    // mode was entered now
                    return new Rect(iModel.Helper.WindowX.Native,
                                    iModel.Helper.WindowY.Native + iModel.Helper.WindowHeight.Native - iView.KompactModeHeight,
                                    iModel.Helper.WindowWidth.Native,
                                    iView.KompactModeHeight);
                }
            }
        }

        public Rect NormalModeRect
        {
            get
            {
                if (iModel.Helper.KompactMode.Native)
                {
                    // in kompact mode, so return the rect that the window **would** use if kompact
                    // mode was exited now
                    return new Rect(iModel.Helper.WindowX.Native,
                                    iModel.Helper.WindowY.Native + iView.KompactModeHeight - iModel.Helper.WindowHeight.Native,
                                    iModel.Helper.WindowWidth.Native,
                                    iModel.Helper.WindowHeight.Native);
                }
                else
                {
                    // not in kompact mode, so return the current rect for the window
                    return new Rect(iModel.Helper.WindowX.Native, iModel.Helper.WindowY.Native,
                                    iModel.Helper.WindowWidth.Native, iModel.Helper.WindowHeight.Native);
                }
            }
        }

        private ModelMain iModel;
        private IViewMainWindow iView;
    }


    // view interface and controller for the main application window
    public interface IViewMainWindow
    {
        void Show(bool aShow);
        void ShowAlertPanel(string aTitle, string aMessage);
        bool PointInDragRect(Point aPt);
        bool PointInResizeRect(Point aPt);
        Rect Rect { get; set; }
        bool IsFullscreen { get; set; }
        float KompactModeHeight { get; }
        void KompactModeEnter(Rect aRect);
        void KompactModeExit(Rect aRect);
        float SplitterFraction { set; }
        void NowPlayingModeEnter();
        void NowPlayingModeExit();
    }

    public class ControllerMainWindow
    {
        public ControllerMainWindow(IViewMainWindow aView)
        {
            iView = aView;
            iModel = ModelMain.Instance;
            iHelper = new MainWindowHelper(iModel, iView);
            iDragging = false;
            iResizing = false;
            iDragOffset = new Point(0, 0);
        }

        public void MouseDown(Point aMousePt)
        {
            if (iView.PointInResizeRect(aMousePt))
            {
                // drag offset is (Window Bottom-Right Corner) - (Mouse Point)
                iDragOffset.X = iView.Rect.Origin.X + iView.Rect.Width - aMousePt.X;
                iDragOffset.Y = iView.Rect.Origin.Y - aMousePt.Y;
                iResizing = true;
            }
            else if (iView.PointInDragRect(aMousePt))
            {
                // drag offset is (Window Origin) - (Mouse Point)
                iDragOffset.X = iView.Rect.Origin.X - aMousePt.X;
                iDragOffset.Y = iView.Rect.Origin.Y - aMousePt.Y;
                iDragging = true;
            }
        }

        public void MouseDragged(Point aMousePt)
        {
            if (iResizing)
            {
                ResizeView(aMousePt);
            }
            else if (iDragging)
            {
                DragView(aMousePt);
            }
        }

        public void MouseUp(Point aMousePt)
        {
            if (iResizing)
            {
                iResizing = false;
                ResizeView(aMousePt);
            }
            else if (iDragging)
            {
                iDragging = false;
                DragView(aMousePt);
            }

            // resizing the window can implicitly cause it to be in fullscreen mode
            if (!iView.IsFullscreen)
            {
                // the window is no longer in fullscreen mode - regardless of whether the window started in
                // fullscreen or not, update the model state with the current window rect
                iModel.Helper.WindowX.Native = iView.Rect.Origin.X;
                iModel.Helper.WindowY.Native = iView.Rect.Origin.Y;
                iModel.Helper.WindowWidth.Native = iView.Rect.Width;

                // only update the window height if not in kompact mode - in kompact mode, this
                // is preserved so the window can exit kompact mode correctly
                if (!iModel.Helper.KompactMode.Native)
                {
                    iModel.Helper.WindowHeight.Native = iView.Rect.Height;
                }
            }
            else
            {
                // the window is now in fullscreen mode - do not update the model state of the
                // window since this contains the window rect information before fullscreen was entered
                // and will be used when restoring the window from fullscreen
            }

            // update fullscreen flag
            iModel.Helper.Fullscreen.Native = iView.IsFullscreen;
        }

        public void ButtonMaximiseClicked()
        {
            // exit kompact mode before maximising
            if (iModel.Helper.KompactMode.Native)
            {
                SetKompactMode(false);
            }

            // maximise
            SetViewToFullscreen(true);
        }

        public void ButtonRestoreClicked()
        {
            // go back to normal mode
            SetViewToFullscreen(false);
        }

        public void ButtonKompactClicked()
        {
            if (iModel.Helper.KompactMode.Native)
            {
                // exit kompact mode and return to normal mode
                SetKompactMode(false);
            }
            else
            {
                // entering kompact mode - first, exit fullscreen mode and return to normal mode
                if (iModel.Helper.Fullscreen.Native)
                {
                    SetViewToFullscreen(false);
                }

                // enter kompact mode
                SetKompactMode(true);
            }
        }

        public void ButtonNowPlayingEnterClicked()
        {
            // if required, exit kompact mode and return to normal mode
            if (iModel.Helper.KompactMode.Native)
            {
                SetKompactMode(false);
            }

            // enter now playing mode
            iView.NowPlayingModeEnter();
        }

        public void ButtonNowPlayingExitClicked()
        {
            // exit now playing mode, returning to normal mode
            iView.NowPlayingModeExit();
        }

        public void SetSplitterFraction(float aSplitterFraction)
        {
            iModel.Helper.SplitterFraction.Native = aSplitterFraction;
        }

        private void DragView(Point aMousePt)
        {
            Rect rect = iView.Rect;
            rect.Origin.X = iDragOffset.X + aMousePt.X;
            rect.Origin.Y = iDragOffset.Y + aMousePt.Y;
            iView.Rect = rect;
        }

        private void ResizeView(Point aMousePt)
        {
            // calculate topleft and bottomright corners of view from the mouse position
            Rect rect = iView.Rect;
            Point topLeft = new Point(rect.Origin.X, rect.Origin.Y + rect.Height);
            Point bottomRight = new Point(iDragOffset.X + aMousePt.X, iDragOffset.Y + aMousePt.Y);

            if (iModel.Helper.KompactMode.Native)
            {
                // in kompact mode, do not allow resizing the height
                bottomRight.Y = rect.Origin.Y;
            }
            else
            {
                // in normal mode, clamp height to be greater than minimum
                if (topLeft.Y - bottomRight.Y < iMinHeight)
                {
                    bottomRight.Y = topLeft.Y - iMinHeight;
                }
            }

            // always clamp the width to be greater than the minimum
            if (bottomRight.X - topLeft.X < iMinWidth)
            {
                bottomRight.X = topLeft.X + iMinWidth;
            }

            // calculate the new view rect
            rect.Origin.X = topLeft.X;
            rect.Origin.Y = bottomRight.Y;
            rect.Width = bottomRight.X - topLeft.X;
            rect.Height = topLeft.Y - bottomRight.Y;
            iView.Rect = rect;
        }

        private void SetViewToFullscreen(bool aFullscreen)
        {
            if (aFullscreen)
            {
                iView.IsFullscreen = true;
            }
            else
            {
                // when exiting fullscreen - always go to the normal mode rect
                iView.Rect = iHelper.NormalModeRect;
                iView.IsFullscreen = false;
            }

            // update the model fullscreen flag
            iModel.Helper.Fullscreen.Native = aFullscreen;
        }

        private void SetKompactMode(bool aKompactModeOn)
        {
            if (aKompactModeOn)
            {
                // get the kompact mode rect that the window will occupy when normal mode is exited
                Rect kompactRect = iHelper.KompactModeRect;

                // enter kompact mode
                iView.KompactModeEnter(kompactRect);

                // store the current model state for kompact mode
                iModel.Helper.KompactMode.Native = true;
                iModel.Helper.WindowX.Native = kompactRect.Origin.X;
                iModel.Helper.WindowY.Native = kompactRect.Origin.Y;
                iModel.Helper.WindowWidth.Native = kompactRect.Width;
            }
            else
            {
                // get the normal mode rect that the window will occupy when kompact mode is exited
                Rect normalRect = iHelper.NormalModeRect;

                // exit kompact mode
                iView.KompactModeExit(normalRect);

                // store the new model state for the normal mode
                iModel.Helper.KompactMode.Native = false;
                iModel.Helper.WindowX.Native = normalRect.Origin.X;
                iModel.Helper.WindowY.Native = normalRect.Origin.Y;
                iModel.Helper.WindowWidth.Native = normalRect.Width;
            }
        }

        private const float iMinWidth = 640;
        private const float iMinHeight = 480;

        private IViewMainWindow iView;
        private ModelMain iModel;
        private MainWindowHelper iHelper;
        private bool iDragging;
        private bool iResizing;
        private Point iDragOffset;
    }


    // view interface to all views that provide list like functionality with selections
    // and a class to provide some helper functionality
    public interface IViewSelectable
    {
        int ClickedItem { get; }
        int ClickedGroup { get; }

        bool SelectionContainsItem(int aIndex);
        bool SelectionContainsGroup(int aIndex);

        int SelectedItemCount { get; }
        IList<int> SelectedItems { get; }
    }

    public class ViewSelectableHelper
    {
        public ViewSelectableHelper(IViewSelectable aView, IList<ModelListGroup> aGroups)
        {
            iView = aView;
            iGroups = aGroups;
        }

        public int ClickedItemCount
        {
            get
            {
                if (iView.ClickedItem >= 0)
                {
                    // an item, as opposed to a group, has been clicked
                    if (iView.SelectionContainsItem(iView.ClickedItem))
                    {
                        // clicked item is part of selection - all selected items
                        // are clicked
                        return iView.SelectedItemCount;
                    }
                    else
                    {
                        // clicked item is not part of selection - only that item
                        // is clicked
                        return 1;
                    }
                }
                else if (iView.ClickedGroup >= 0)
                {
                    // a group, as opposed to an item, has been clicked
                    if (iView.SelectionContainsGroup(iView.ClickedGroup))
                    {
                        // clicked group is part of selection - all selected
                        // items are clicked
                        return iView.SelectedItemCount;
                    }
                    else
                    {
                        // clicked group is not part of selection - only items
                        // in this group are clicked
                        return iGroups[iView.ClickedGroup].Count;
                    }
                }
                else
                {
                    // no item or group clicked
                    return 0;
                }
            }
        }

        public IList<int> ClickedItems
        {
            get
            {
                if (iView.ClickedItem >= 0)
                {
                    // an item, as opposed to a group, has been clicked
                    if (iView.SelectionContainsItem(iView.ClickedItem))
                    {
                        // clicked item is part of selection - all selected items
                        // are clicked
                        return iView.SelectedItems;
                    }
                    else
                    {
                        // clicked item is not part of selection - only that item
                        // is clicked
                        List<int> items = new List<int>();
                        items.Add(iView.ClickedItem);
                        return items.AsReadOnly();
                    }
                }
                else if (iView.ClickedGroup >= 0)
                {
                    // a group, as opposed to an item, has been clicked
                    if (iView.SelectionContainsGroup(iView.ClickedGroup))
                    {
                        // clicked group is part of selection - all selected
                        // items are clicked
                        return iView.SelectedItems;
                    }
                    else
                    {
                        // clicked group is not part of selection - only items
                        // in this group are clicked
                        ModelListGroup g = iGroups[iView.ClickedGroup];

                        List<int> items = new List<int>();
                        for (int i=0 ; i<g.Count ; i++)
                        {
                            items.Add(g.FirstIndex + i);
                        }
                        return items.AsReadOnly();
                    }
                }
                else
                {
                    // no item or group clicked
                    List<int> items = new List<int>();
                    return items.AsReadOnly();
                }
            }
        }

        private IViewSelectable iView;
        private IList<ModelListGroup> iGroups;
    }


    // Controller interface for a dragging source and destination
    public interface IControllerDragSource
    {
        ViewDragData DragBegin(IList<int> aIndices, object aSource);
        void DragEnd(IDraggableData aDragData, EDragOperation aOperation);
    }

    public interface IControllerDragDestination
    {
        EDragOperation ValidateDrag(IDraggableData aDragData, object aDestination);
        bool AcceptDrop(IDraggableData aDragData, int aRow, object aDestination);
    }

    // view interface and controller for the ds playlist view
    public interface IViewPlaylistDs : IViewSelectable
    {
    }

    public class ControllerPlaylistDs : IControllerDragSource, IControllerDragDestination
    {
        public ControllerPlaylistDs(IViewPlaylistDs aView, IModelPlaylistDs aModel)
        {
            iView = aView;
            iModel = aModel;
        }

        public bool ValidateMenuItemPlay()
        {
            // show "Play" if an item has been clicked
            return (iView.ClickedItem >= 0);
        }

        public bool ValidateMenuItemMoveUp()
        {
            ViewSelectableHelper helper = new ViewSelectableHelper(iView, iModel.Groups);

            // show the "Move Up" item if
            // - any item other than the first is clicked
            // - the clicked item is the only selected item or is not part of the current item selection
            return (iView.ClickedItem > 0) && (helper.ClickedItemCount == 1);
        }

        public bool ValidateMenuItemMoveDown()
        {
            ViewSelectableHelper helper = new ViewSelectableHelper(iView, iModel.Groups);

            // show the "Move Down" item if
            // - any item other than the last is clicked
            // - the clicked item is the only selected item or is not part of the current item selection
            return (iView.ClickedItem >= 0) && (iView.ClickedItem < iModel.Playlist.Count - 1) && (helper.ClickedItemCount == 1);
        }

        public bool ValidateMenuItemSave()
        {
            ViewSelectableHelper helper = new ViewSelectableHelper(iView, iModel.Groups);

            // show "Save" if 1+ items have been clicked
            return (helper.ClickedItemCount > 0);
        }

        public bool ValidateMenuItemDelete()
        {
            ViewSelectableHelper helper = new ViewSelectableHelper(iView, iModel.Groups);

            // show "Delete" if 1+ items have been clicked
            return (helper.ClickedItemCount > 0);
        }

        public bool ValidateMenuItemDetails()
        {
            // show "Details" is 1 item has been clicked
            return (iView.ClickedItem >= 0);
        }


        public void MenuItemPlay()
        {
            if(ValidateMenuItemPlay())
            {
                iModel.TrackIndex = iView.ClickedItem;
            }
        }

        public void MenuItemMoveUp()
        {
            // assert what we expect from the Validate... method
            if(ValidateMenuItemMoveUp())
            {

                List<MrItem> items = new List<MrItem>();
                items.Add(iModel.Playlist[iView.ClickedItem]);

                // when moving an item up 1, the item is re-inserted **before**
                // the previous item and **after** the item 2 up in the list. If the
                // clicked item is second in the list, the item 2 up from it does not
                // exist, so an afterId of 0 denotes move to the start of the list
                uint afterId = 0;
                if (iView.ClickedItem > 1)
                {
                    afterId = iModel.Playlist[iView.ClickedItem - 2].Id;
                }

                iModel.MoveTracks(items, afterId);
            }
        }

        public void MenuItemMoveDown()
        {
            // assert what we expect from the Validate... method
            if(ValidateMenuItemMoveDown())
            {
                List<MrItem> items = new List<MrItem>();
                items.Add(iModel.Playlist[iView.ClickedItem]);

                // when moving down, the clicked item always is re-inserted after the
                // next item in the list - the Validate... method ensures there is
                // always a next item
                uint afterId = iModel.Playlist[iView.ClickedItem + 1].Id;

                iModel.MoveTracks(items, afterId);
            }
        }

        public void MenuItemSave()
        {
            // assert what we expect from the Validate... method
            if(ValidateMenuItemSave())
            {

                IList<MrItem> clickedItems = ClickedItems();

                iModel.SaveTracks(clickedItems);
            }
        }

        public void MenuItemDelete()
        {
            // assert what we expect from the Validate... method
            if(ValidateMenuItemDelete())
            {

                IList<MrItem> clickedItems = ClickedItems();

                iModel.DeleteTracks(clickedItems);
            }
        }

        public void MenuItemDetails()
        {
            // assert what we expect from the Validate... method
            //Assert.Check(ValidateMenuItemDetails());
        }


        #region IControllerDragSource implementation
        public ViewDragData DragBegin(IList<int> aIndices, object aSource)
        {
            // create the list of upnpObjects for this list of indices
            List<upnpObject> dragObjects = new List<upnpObject>();

            foreach (int index in aIndices)
            {
                if (index < iModel.Playlist.Count && iModel.Playlist[index].DidlLite.Count > 0)
                {
                    dragObjects.Add(iModel.Playlist[index].DidlLite[0]);
                }
            }

            IMediaRetriever retriever = new MediaRetrieverNoRetrieve(dragObjects);

            ModelMain.Instance.PlaySupport.SetDragging(true);

            return new ViewDragData(aIndices, retriever, aSource);
        }

        public void DragEnd(IDraggableData aDragData, EDragOperation aOperation)
        {
            ModelMain.Instance.PlaySupport.SetDragging(false);

            // when dragging items from the playlist, the only things that can change the
            // playlist are moving and deleting items - moving is handled in the AcceptDrop
            // method below
            if (aOperation == EDragOperation.eDelete)
            {
                ViewDragData dragData = aDragData.GetViewDragData();
                Assert.Check(dragData != null);

                List<MrItem> list = new List<MrItem>();

                foreach (int index in dragData.Indices)
                {
                    if (index < iModel.Playlist.Count)
                    {
                        list.Add(iModel.Playlist[index]);
                    }
                }

                iModel.DeleteTracks(list);
            }
        }
        #endregion IControllerDragSource implementation


        #region IControllerDragDestination implementation
        public EDragOperation ValidateDrag(IDraggableData aDragData, object aDestination)
        {
            // convert the dragged data into didl lite
            MediaProviderDraggable draggable = ModelMain.Instance.DropConverterExpand.Convert(aDragData);

            if (draggable != null)
            {
                foreach (upnpObject o in draggable.DragMedia)
                {
                    if (o.Res.Count > 0 &&
                        System.IO.Path.GetExtension(o.Res[0].Uri) == PluginManager.kPluginExtension)
                    {
                        return EDragOperation.eNone;
                    }
                }
            }

            ViewDragData dragData = aDragData.GetViewDragData();
            if (dragData != null && dragData.Source.Equals(aDestination))
            {
                // dragging from within the playlist
                return EDragOperation.eMove;
            }
            else
            {
                // dragged from outside the playlist
                return EDragOperation.eCopy;
            }
        }

        public bool AcceptDrop(IDraggableData aDragData, int aRow, object aDestination)
        {
            // get ID of item after which to insert
            uint afterId = 0;
            if (aRow > 0 && aRow - 1 < iModel.Playlist.Count)
            {
                afterId = iModel.Playlist[aRow - 1].Id;
            }


            ViewDragData dragData = aDragData.GetViewDragData();

            if (dragData != null && dragData.Source.Equals(aDestination))
            {
                // dragged data within the playlist
                List<MrItem> list = new List<MrItem>();

                foreach (int index in dragData.Indices)
                {
                    if (index < iModel.Playlist.Count)
                    {
                        list.Add(iModel.Playlist[index]);
                    }
                }

                // move items
                iModel.MoveTracks(list, afterId);
            }
            else
            {
                // dragged from outside playlist
                MediaProviderDraggable draggable = ModelMain.Instance.DropConverterExpand.Convert(aDragData);

                if (draggable != null)
                {
                    // insert into playlist
                    iModel.InsertTracks(draggable, afterId);
                }
            }

            return true;
        }
        #endregion IControllerDragDestination implementation


        private IList<MrItem> ClickedItems()
        {
            ViewSelectableHelper helper = new ViewSelectableHelper(iView, iModel.Groups);

            List<MrItem> items = new List<MrItem>();

            foreach (int i in helper.ClickedItems)
            {
                items.Add(iModel.Playlist[i]);
            }

            return items.AsReadOnly();
        }


        private IViewPlaylistDs iView;
        private IModelPlaylistDs iModel;
    }


    // view interface and controller for the main browser view
    public interface IViewBrowserPane
    {
        void SetBreadcrumb(IList<string> aPath);
        void EnableButtonUp(bool aEnable);
        void EnableButtonSize(bool aEnable);
        void EnableButtonList(bool aEnable);
        void SetSliderSizeValue(float aValue);
    }

    public class ControllerBrowser
    {
        public ControllerBrowser(IViewBrowserPane aView, OptionUint aContainerView, OptionFloat aContainerSizeThumbs, OptionFloat aContainerSizeList)
        {
            iView = aView;
            iContainerView = aContainerView;
            iContainerSizeThumbs = aContainerSizeThumbs;
            iContainerSizeList = aContainerSizeList;
        }

        public void SetModel(ModelBrowser aModel)
        {
            iModel = aModel;

            BrowserLocationChanged(iModel);
        }

        public void BrowserLocationChanged(object aBrowser)
        {
            // if this update comes from the this browser, update the breadcrumb
            if (aBrowser == iModel)
            {
                List<string> trail = new List<string>();
                foreach (Breadcrumb b in iModel.BreadcrumbTrail)
                {
                    trail.Add(b.Title);
                }
                iView.SetBreadcrumb(trail);

                iView.EnableButtonUp(iModel.BreadcrumbTrail.Count > 1);

                if (iModel.Location != null)
                {
                    iView.EnableButtonSize(!(iModel.Location.Current.Metadata is musicAlbum));
                    iView.EnableButtonList(!(iModel.Location.Current.Metadata is musicAlbum));
                }
                else
                {
                    iView.EnableButtonSize(false);
                    iView.EnableButtonList(false);
                }
            }
        }

        public void ButtonUpClicked()
        {
            iModel.Up(1);
        }

        public void ButtonListClicked(bool aListOn)
        {
            iContainerView.Native = (uint)(aListOn ? 1 : 0);
            iView.SetSliderSizeValue(aListOn ? iContainerSizeList.Native : iContainerSizeThumbs.Native);
        }

        public void BreadcrumbClicked(string aPathComponent)
        {
            BreadcrumbTrail trail = iModel.BreadcrumbTrail;

            for (int i=0 ; i<trail.Count ; i++)
            {
                if (trail[i].Title == aPathComponent)
                {
                    int up = trail.Count - i - 1;
                    if (up > 0)
                    {
                        iModel.Up((uint)up);
                        break;
                    }
                }
            }
        }

        public void SliderSizeChanged(float aValue)
        {
            if (iContainerView.Native == 1)
            {
                iContainerSizeList.Native = aValue;
            }
            else
            {
                iContainerSizeThumbs.Native = aValue;
            }
        }

        private IViewBrowserPane iView;
        private ModelBrowser iModel;
        private OptionUint iContainerView;
        private OptionFloat iContainerSizeThumbs;
        private OptionFloat iContainerSizeList;
    }


    // view interface and controller for the content view of the browser
    public interface IViewBrowserContent : IViewSelectable
    {
        void StartRename();
        void ShowAddBookmark(Location aLocation);
    }

    public class ControllerBrowserContent : IControllerDragSource, IControllerDragDestination
    {
        public ControllerBrowserContent(ModelBrowser aBrowser, ContainerContent aContent, IPlaylistSupport aPlaySupport, IViewBrowserContent aView)
        {
            iBrowser = aBrowser;
            iContent = aContent;
            iPlaySupport = aPlaySupport;
            iView = aView;
            iViewHelper = new ViewSelectableHelper(iView, null);
        }

        public ContainerContent Content
        {
            get { return iContent; }
        }

        public void ItemActivated(int aIndex)
        {
            upnpObject obj = iContent.Object(aIndex);

            if (obj is container)
            {
                iBrowser.Down(obj as container);
            }
            else if (obj is item)
            {
                List<upnpObject> list = new List<upnpObject>();
                list.Add(obj);

                // use a MediaRetrieverNoRetrieve here since we know we have only added
                // a single item to the playlist and will never have to recurse into containers
                iPlaySupport.PlayNow(new MediaRetrieverNoRetrieve(list));
            }
        }

        public bool CanRenameItem(int aIndex)
        {
            upnpObject obj = iContent.Object(aIndex);

            return (obj != null && iContent.Location.Current.HandleRename(obj));
        }

        public bool ValidateMenuItemOpen()
        {
            return (iViewHelper.ClickedItemCount == 1) && (iContent.Object(iView.ClickedItem) is container);
        }

        public bool ValidateMenuItemPlayNow()
        {
            return (iViewHelper.ClickedItemCount > 0) && iPlaySupport.IsOpen() && !iPlaySupport.IsInserting();
        }

        public bool ValidateMenuItemPlayNext()
        {
            return ValidateMenuItemPlayNow();
        }

        public bool ValidateMenuItemPlayLater()
        {
            return ValidateMenuItemPlayNow();
        }

        public bool ValidateMenuItemDelete()
        {
            DidlLite didl = new DidlLite();
            didl.AddRange(ClickedItems());

            return (didl.Count > 0) && (iContent.Location.Current.HandleDelete(didl));
        }

        public bool ValidateMenuItemRename()
        {
            IList<upnpObject> clickedItems = ClickedItems();

            return (clickedItems.Count == 1) && (iContent.Location.Current.HandleRename(clickedItems[0]));
        }

        public bool ValidateMenuItemBookmark()
        {
            IList<upnpObject> clickedItems = ClickedItems();

            return (clickedItems.Count == 1 && clickedItems[0] is container);
        }

        public bool ValidateMenuItemDetails()
        {
            return (iViewHelper.ClickedItemCount == 1);
        }

        public void MenuItemOpen()
        {
            if(ValidateMenuItemOpen())
            {
                iBrowser.Down(iContent.Object(iView.ClickedItem) as container);
            }
        }

        public void MenuItemPlayNow()
        {
            if(ValidateMenuItemPlayNow())
            {
                iPlaySupport.PlayNow(new MediaRetriever(iContent.Location.Current, ClickedItems()));
            }
        }

        public void MenuItemPlayNext()
        {
            if(ValidateMenuItemPlayNext())
            {
                iPlaySupport.PlayNext(new MediaRetriever(iContent.Location.Current, ClickedItems()));
            }
        }

        public void MenuItemPlayLater()
        {
            if(ValidateMenuItemPlayLater())
            {
                iPlaySupport.PlayLater(new MediaRetriever(iContent.Location.Current, ClickedItems()));
            }
        }

        public void MenuItemDelete()
        {
            if(ValidateMenuItemDelete())
            {
                IContainer ctr = iContent.Location.Current;

                foreach (upnpObject o in ClickedItems())
                {
                    ctr.Delete(o.Id);
                }
            }
        }

        public void MenuItemRename()
        {
            if(ValidateMenuItemRename())
            {
                iView.StartRename();
            }
        }

        public void MenuItemBookmark()
        {
            if(ValidateMenuItemBookmark())
            {

                IList<upnpObject> clickedItems = ClickedItems();
                IContainer c = iContent.Location.Current.ChildContainer(clickedItems[0] as container);
                if (c != null)
                {
                    Location l = new Location(iContent.Location, c);
                    iView.ShowAddBookmark(l);
                }
            }
        }

        public void MenuItemDetails()
        {
            //Assert.Check(ValidateMenuItemDetails());
        }


        #region IControllerDragSource implementation
        public ViewDragData DragBegin(IList<int> aIndices, object aSource)
        {
            // build the list of objects to drag
            List<upnpObject> dragObjects = new List<upnpObject>();

            foreach (int index in aIndices)
            {
                if (index < iContent.Count)
                {
                    upnpObject obj = iContent.Object(index);
                    if (obj != null)
                    {
                        dragObjects.Add(obj);
                    }
                }
            }

            // create a retriever that will recursively expand the list of objects to
            // contain all child items
            IMediaRetriever retriever = new MediaRetriever(iContent.Location.Current, dragObjects);

            ModelMain.Instance.PlaySupport.SetDragging(true);

            return new ViewDragData(aIndices, retriever, aSource);
        }

        public void DragEnd(IDraggableData aDragData, EDragOperation aOperation)
        {
            ModelMain.Instance.PlaySupport.SetDragging(false);

            if (aOperation == EDragOperation.eDelete || aOperation == EDragOperation.eMove)
            {
                ViewDragData dragData = aDragData.GetViewDragData();
                Assert.Check(dragData != null);

                MediaProviderDraggable draggable = new MediaProviderDraggable(dragData.Retriever);

                IContainer container = iContent.Location.Current;

                if (container.HandleDelete(draggable.DragMedia))
                {
                    foreach (upnpObject o in draggable.DragMedia)
                    {
                        container.Delete(o.Id);
                    }
                }
            }
        }
        #endregion IControllerDragSource implementation


        #region IControllerDragDestination implementation
        public EDragOperation ValidateDrag(IDraggableData aDragData, object aDestination)
        {
            // convert the drag data into didl lite
            MediaProviderDraggable draggable = ModelMain.Instance.DropConverterNoExpand.Convert(aDragData);

            if (draggable != null)
            {
                IContainer container = iContent.Location.Current;

                if (container.HandleInsert(draggable.DragMedia))
                {
                    ViewDragData dragData = aDragData.GetViewDragData();
                    if (dragData != null && dragData.Source.Equals(aDestination))
                    {
                        if (container.HandleMove(draggable.DragMedia))
                        {
                            return EDragOperation.eMove;
                        }
                    }
                    else
                    {
                        return EDragOperation.eCopy;
                    }
                }
            }

            return EDragOperation.eNone;
        }

        public bool AcceptDrop(IDraggableData aDragData, int aRow, object aDestination)
        {
            MediaProviderDraggable draggable = ModelMain.Instance.DropConverterNoExpand.Convert(aDragData);

            IContainer container = iContent.Location.Current;

            string id = string.Empty;
            if (iContent.Count > 0 && aRow > 0)
            {
                upnpObject obj = iContent.Object(aRow - 1);
                if (obj != null)
                {
                    id = obj.Id;
                }
            }

            container.Insert(id, draggable.Media);

            return true;
        }
        #endregion IControllerDragDestination implementation


        private IList<upnpObject> ClickedItems()
        {
            List<upnpObject> items = new List<upnpObject>();

            foreach (int i in iViewHelper.ClickedItems)
            {
                items.Add(iContent.Object(i));
            }

            return items;
        }

        private ModelBrowser iBrowser;
        private IPlaylistSupport iPlaySupport;
        private ContainerContent iContent;
        private IViewBrowserContent iView;
        private ViewSelectableHelper iViewHelper;
    }


    // Controller class for specific functionality of the browser album view
    public class ControllerBrowserAlbum
    {
        public ControllerBrowserAlbum(ContainerContent aContent, IPlaylistSupport aPlaySupport)
        {
            iContent = aContent;
            iPlaySupport = aPlaySupport;
        }

        public void MenuItemPlayNow()
        {
            iPlaySupport.PlayNow(new MediaRetriever(iContent.Location.Current, AllItems()));
        }

        public void MenuItemPlayNext()
        {
            iPlaySupport.PlayNext(new MediaRetriever(iContent.Location.Current, AllItems()));
        }

        public void MenuItemPlayLater()
        {
            iPlaySupport.PlayLater(new MediaRetriever(iContent.Location.Current, AllItems()));
        }

        public void MenuItemDetails()
        {
        }

        private IList<upnpObject> AllItems()
        {
            List<upnpObject> items = new List<upnpObject>();

            for (int i=0 ; i<iContent.Count ; i++)
            {
                upnpObject o = iContent.Object(i);
                if (o != null)
                {
                    items.Add(o);
                }
            }

            return items;
        }

        private ContainerContent iContent;
        private IPlaylistSupport iPlaySupport;
    }


    // Controller class for the transport buttons
    public class ControllerTransport
    {
        public ControllerTransport(IModelTransport aModel)
        {
            iModel = aModel;
        }

        public void ButtonPreviousClicked()
        {
            iModel.Previous();
        }

        public void ButtonPlayPauseClicked()
        {
            if (iModel.TransportState == ETransportState.ePlaying)
            {
                iModel.Pause();
            }
            else
            {
                iModel.Play();
            }
        }

        public void ButtonPlayStopClicked()
        {
            if (iModel.TransportState == ETransportState.ePlaying)
            {
                iModel.Stop();
            }
            else
            {
                iModel.Play();
            }
        }

        public void ButtonNextClicked()
        {
            iModel.Next();
        }

        private IModelTransport iModel;
    }


    // Controller class and view interface for the media time
    public interface IViewMediaTime
    {
        bool EnableSeekControl { set; }
        uint Value { set; }
        uint MaxValue { set; }
        string TextValue { set; }
        bool PreviewEnabled { set; }
        uint PreviewValue { set; }
        bool EnableHourglass { set; }
    }

    public class ControllerMediaTime
    {
        public ControllerMediaTime(IViewMediaTime aView, IModelMediaTime aModel)
        {
            iView = aView;
            iModel = aModel;
        }

        public void SeekTargetStarted()
        {
            iSeekTargetOn = true;
            iSeekTargetSeconds = iModel.Seconds;
            iSeekTargetDuration = iModel.Duration;

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void SeekTargetStopped()
        {
            iSeekTargetOn = false;

            iModel.Seek(iSeekTargetSeconds);

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void SeekTargetCancelled()
        {
            iSeekTargetOn = false;

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void SeekTargetIncrement()
        {
            iSeekTargetSeconds += iSeekTargetStepSeconds;

            if (iSeekTargetSeconds > iModel.Duration)
            {
                iSeekTargetSeconds = iModel.Duration;
            }

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void SeekTargetDecrement()
        {
            if (iSeekTargetSeconds > iSeekTargetStepSeconds)
            {
                iSeekTargetSeconds -= iSeekTargetStepSeconds;
            }
            else
            {
                iSeekTargetSeconds = 0;
            }

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void TimeClicked()
        {
            iShowTimeRemaining = !iShowTimeRemaining;

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void ModelEventInitialised()
        {
            iSeekTargetOn = false;

            // udpate all views to reflect current model state
            UpdateViews();
        }

        public void ModelEventClose()
        {
            iSeekTargetOn = false;

            // force the views to show nothing and be disabled
            iView.Value = 0;
            iView.MaxValue = 0;
            iView.TextValue = string.Empty;
            iView.PreviewEnabled = false;
            iView.EnableSeekControl = false;
            iView.EnableHourglass = false;
        }

        public void ModelEventChanged()
        {
            // state of the track has changed - potential reset the seek target state
            if (iSeekTargetOn)
            {
                if (!iModel.AllowSeeking || iModel.Duration == 0)
                {
                    // can no longer seek
                    iSeekTargetOn = false;
                }
                else if (iModel.Duration != iSeekTargetDuration)
                {
                    // duration has changed - change of track - reset seeking
                    iSeekTargetDuration = iModel.Duration;
                    iSeekTargetSeconds = 0;
                }
            }

            // update all views to reflect current model state
            UpdateViews();
        }

        private void UpdateViews()
        {
            if (!iModel.IsInitialised)
            {
                iView.Value = 0;
                iView.MaxValue = 0;
                iView.TextValue = string.Empty;
                iView.PreviewEnabled = false;
                iView.EnableSeekControl = false;
                iView.EnableHourglass = false;
            }
            else if (iModel.TransportState != ETransportState.eBuffering)
            {
                // set the current time value
                iView.Value = iModel.Seconds;
                iView.MaxValue = iModel.Duration;

                // set the seek target value
                iView.PreviewEnabled = iSeekTargetOn;
                iView.PreviewValue = iSeekTargetSeconds;

                // set the seek control to be enabled/disabled
                iView.EnableSeekControl = (iModel.Duration != 0) && iModel.AllowSeeking;

                // set the text time
                if (iModel.TransportState != ETransportState.eStopped)
                {
                    int seconds;
                    if (iModel.Duration != 0)
                    {
                        seconds = (int)(iSeekTargetOn ? iSeekTargetSeconds : iModel.Seconds);

                        if (iShowTimeRemaining)
                        {
                            seconds -= (int)iModel.Duration;
                        }
                    }
                    else
                    {
                        // a track with no duration is playing - no sense in displaying time remaining or seeking
                        seconds = (int)iModel.Seconds;
                    }

                    // show format as (total minutes):(seconds)
                    Time t = new Time(seconds);

                    iView.TextValue = string.Format("{0}{1}:{2:00}", (seconds < 0) ? "-" : string.Empty, (t.Hours * 60) + t.Minutes, t.Seconds);
                }
                else
                {
                    iView.TextValue = string.Empty;
                }

                iView.EnableHourglass = false;
            }
            else
            {
                // buffering - nothing to show
                iView.Value = 0;
                iView.MaxValue = 0;
                iView.PreviewEnabled = false;
                iView.TextValue = string.Empty;

                iView.EnableHourglass = true;
            }


            // update the seek step
            iSeekTargetStepSeconds = (uint)Math.Round((iModel.Duration * 0.01f) + 0.5f, MidpointRounding.AwayFromZero);
        }

        private IViewMediaTime iView;
        private IModelMediaTime iModel;

        private bool iShowTimeRemaining;
        private bool iSeekTargetOn;
        private uint iSeekTargetSeconds;
        private uint iSeekTargetStepSeconds;
        private uint iSeekTargetDuration;
    }
}


