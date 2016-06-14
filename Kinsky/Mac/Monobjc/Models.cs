
using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;

using Linn;
using Linn.Kinsky;
using Linn.Topology;

using Upnp;


namespace KinskyDesktop
{

    public class EventArgsSavePlaylist : EventArgs
    {
        public EventArgsSavePlaylist(ISaveSupport aSaveSupport)
        {
            SaveSupport = aSaveSupport;
        }

        public readonly ISaveSupport SaveSupport;
    }


    public class ModelMain : IStack
    {
        private static ModelMain iInstance = null;

        public static void Create(HelperKinskyDesktop aHelper, IInvoker aInvoker,
                                  IAppRestartHandler aRestartHandler)
        {
            Assert.Check(iInstance == null);
            iInstance = new ModelMain(aHelper, aInvoker, aRestartHandler);
        }

        public static ModelMain Instance
        {
            get
            {
                Assert.Check(iInstance != null);
                return iInstance;
            }
        }

        public void Start()
        {
            iHelper.Stack.Start();

            iModelBrowser.Start();
        }

        public void Stop()
        {
            iHelper.Stack.Stop();
            iHelper.Dispose();
        }

        public void Pause()
        {
            iHelper.Stack.Stop();
        }

        public void Resume()
        {
            iHelper.Stack.Start();
        }

        public void Rescan()
        {
            iHelper.Rescan();
            iLocator.Refresh();
        }

        public ModelBrowser ModelBrowser
        {
            get { return iModelBrowser; }
        }

        public HelperKinskyDesktop Helper
        {
            get { return iHelper; }
        }

        public ViewMaster ViewMaster
        {
            get { return iMediatorViews; }
        }

        public IPlaylistSupport PlaySupport
        {
            get { return iPlaySupport; }
        }

        public ISaveSupport SaveSupport
        {
            get { return iSaveSupport; }
        }

        public IViewSaveSupport ViewSaveSupport
        {
            get { return iViewSaveSupport; }
        }

        public LocalPlaylists LocalPlaylists
        {
            get { return iLocalPlaylists; }
        }

        public SharedPlaylists SharedPlaylists
        {
            get { return iSharedPlaylists; }
        }

        public IDropConverter DropConverterExpand
        {
            get { return iDropConverterExpand; }
        }

        public IDropConverter DropConverterNoExpand
        {
            get { return iDropConverterNoExpand; }
        }

        public AutoUpdate AutoUpdate
        {
            get { return iAutoUpdate; }
        }

        public ModelTooltips ModelTooltips
        {
            get { return iModelTooltips; }
        }

        public IModelSelectionList<Linn.Kinsky.Room> ModelRoomList
        {
            get { return iModelRoomList; }
        }

        public IModelSelectionList<Linn.Kinsky.Source> ModelSourceList
        {
            get { return iModelSourceList; }
        }

        public IModelPlaylistAux ModelPlaylistAux
        {
            get { return iModelPlaylistAux; }
        }

        public IModelPlaylistReceiver ModelPlaylistReceiver
        {
            get { return iModelPlaylistReceiver; }
        }

        public IModelPlaylistRadio ModelPlaylistRadio
        {
            get { return iModelPlaylistRadio; }
        }

        public IModelPlaylistDs ModelPlaylistDs
        {
            get { return iModelPlaylistDs; }
        }

        public IModelTrack ModelTrack
        {
            get { return iModelTrack; }
        }

        public IModelTransport ModelTransport
        {
            get { return iModelTransport; }
        }

        public IModelVolumeControl ModelVolumeControl
        {
            get { return iModelVolume; }
        }

        public IModelMediaTime ModelMediaTime
        {
            get { return iModelMediaTime; }
        }

        public event EventHandler<EventArgsSavePlaylist> EventSavePlaylist;
        public event EventHandler<AutoUpdate.EventArgsUpdateFound> EventUpdateFound;


        private ModelMain(HelperKinskyDesktop aHelper, IInvoker aInvoker,
                          IAppRestartHandler aRestartHandler)
        {
            iInvoker = aInvoker;
            iHelper = aHelper;
            iHelper.SetStackExtender(this);

            iHttpServer = new HttpServer(HttpServer.kPortKinskyDesktop);
            iHttpClient = new HttpClient();

            iLocalPlaylists = new LocalPlaylists(iHelper, true);
            iSharedPlaylists = new SharedPlaylists(iHelper);
            iLibrary = new MediaProviderLibrary(iHelper);
            iSupport = new MediaProviderSupport(iHttpServer);

            iDropConverterExpand = new DropConverterRoot();
            iDropConverterExpand.Add(new DropConverterInternal());
            iDropConverterExpand.Add(new DropConverterUri());
            iDropConverterExpand.Add(new DropConverterFileDrop(iHttpServer, true));
            iDropConverterExpand.Add(new DropConverterText());

            iDropConverterNoExpand = new DropConverterRoot();
            iDropConverterNoExpand.Add(new DropConverterInternal());
            iDropConverterNoExpand.Add(new DropConverterUri());
            iDropConverterNoExpand.Add(new DropConverterFileDrop(iHttpServer, false));
            iDropConverterNoExpand.Add(new DropConverterText());

            PluginManager pluginManager = new PluginManager(iHelper, iHttpClient, iSupport);
            iLocator = new ContentDirectoryLocator(pluginManager, aRestartHandler);
            OptionBool optionLocalPlaylists = iLocator.Add(LocalPlaylists.kRootId, iLocalPlaylists);
            OptionBool optionSharedPlaylists = iLocator.Add(SharedPlaylists.kRootId, iSharedPlaylists);
            iLocator.Add(MediaProviderLibrary.kLibraryId, iLibrary);
            iHelper.AddOptionPage(iLocator.OptionPage);

            iPlaySupport = new PlaySupport();
            iSaveSupport = new SaveSupport(iHelper, iSharedPlaylists, optionSharedPlaylists, iLocalPlaylists, optionLocalPlaylists);
            iViewSaveSupport = new ViewSaveSupport(SavePlaylistHandler, iSaveSupport);

            iMediatorViews = new ViewMaster();
            Model model = new Model(iMediatorViews, iPlaySupport);
            iMediator = new Mediator(iHelper, model);


            // configure auto-updates
            iHelper.OptionPageUpdates.EventBetaVersionsChanged += OptionUpdatesChanged;

            EReleaseQuality currentBuildType = EReleaseQuality.Stable;
            if (iHelper.Product.Contains("(NightlyBuild)"))
            {
                currentBuildType = EReleaseQuality.Nightly;
            }
            else if (iHelper.Product.Contains("(Beta)"))
            {
                currentBuildType = EReleaseQuality.Beta;
            }
            else if (iHelper.Product.Contains("(Development)"))
            {
                currentBuildType = EReleaseQuality.Development;
            }
            
            var appName = iHelper.Title;
            iAutoUpdate = new AutoUpdate(iHelper,
                                         AutoUpdate.DefaultFeedLocation(appName, "MacOsX"),
                                         0,                 // update interval of 0 means infinite i.e. only check on application startup
                                         OptionUpdateType,
                                         appName,
                                         "macosx",
                                         1,
                                         currentBuildType);
            iAutoUpdate.EventUpdateFound += AutoUpdateFound;

            iModelBrowser = new ModelBrowser(iLocator.Root, iHelper.LastLocation.BreadcrumbTrail, aInvoker);
            iModelBrowser.EventLocationChanged += ModelBrowserLocationChanged;

            iModelRoomList = new ModelSelectionList<Linn.Kinsky.Room>(aInvoker, new ComparerRoom());
            iModelSourceList = new ModelSelectionList<Linn.Kinsky.Source>(aInvoker, new ComparerSource());
            iModelPlaylistAux = new ModelPlaylistAux(aInvoker);
            iModelPlaylistReceiver = new ModelPlaylistReceiver(aInvoker);
            iModelPlaylistRadio = new ModelPlaylistRadio(aInvoker, iViewSaveSupport);
            iModelPlaylistDs = new ModelPlaylistDs(aInvoker, iViewSaveSupport);
            iModelTrack = new ModelTrack(aInvoker);
            iModelTransport = new ModelTransport(aInvoker);
            iModelVolume = new ModelVolumeControl(aInvoker);
            iModelMediaTime = new ModelMediaTime(aInvoker);

            iMediatorViews.ViewWidgetSelectorRoom.Add(iModelRoomList);
            iMediatorViews.ViewWidgetSelectorSource.Add(iModelSourceList);
            iMediatorViews.ViewWidgetPlaylistAux.Add(iModelPlaylistAux);
            iMediatorViews.ViewWidgetPlaylistReceiver.Add(iModelPlaylistReceiver);
            iMediatorViews.ViewWidgetPlaylistRadio.Add(iModelPlaylistRadio);
            iMediatorViews.ViewWidgetPlaylist.Add(iModelPlaylistDs);
            iMediatorViews.ViewWidgetTrack.Add(iModelTrack);
            iMediatorViews.ViewWidgetTransportControlMediaRenderer.Add(iModelTransport);
            iMediatorViews.ViewWidgetTransportControlRadio.Add(iModelTransport);
            iMediatorViews.ViewWidgetVolumeControl.Add(iModelVolume);
            iMediatorViews.ViewWidgetMediaTime.Add(iModelMediaTime);

            iModelTooltips = new ModelTooltips(iHelper.ShowToolTips);
        }

        void IStack.Start(IPAddress aIpAddress)
        {
            iMediator.Open();
            iLibrary.Start(aIpAddress);
            iSharedPlaylists.Start(aIpAddress);
            iHttpClient.Start();
            iHttpServer.Start(aIpAddress);
            iLocator.Start();
            iAutoUpdate.Start();
        }

        void IStack.Stop()
        {
            iAutoUpdate.Stop();
            iLocator.Stop();
            iHttpServer.Stop();
            iHttpClient.Stop();
            iSharedPlaylists.Stop();
            iLibrary.Stop();
            iMediator.Close();
        }

        private void SavePlaylistHandler(ISaveSupport aSaveSupport)
        {
            if (EventSavePlaylist != null)
            {
                EventSavePlaylist(this, new EventArgsSavePlaylist(aSaveSupport));
            }
        }

        private EReleaseQuality OptionUpdateType
        {
            get
            {
                EReleaseQuality updateType = EReleaseQuality.Stable;
                if (iHelper.OptionPageUpdates.BetaVersions)
                {
                    updateType |= EReleaseQuality.Beta;
                }
                if (iHelper.OptionPageUpdates.DevelopmentVersions)
                {
                    updateType |= EReleaseQuality.Development;
                }
                if (iHelper.OptionPageUpdates.NightlyBuilds)
                {
                    updateType |= EReleaseQuality.Nightly;
                }
                return updateType;
            }
        }

        private void OptionUpdatesChanged(object sender, EventArgs e)
        {
            iAutoUpdate.DesiredQuality = OptionUpdateType;
        }

        private void AutoUpdateFound(object sender, Linn.AutoUpdate.EventArgsUpdateFound e)
        {
            if (iInvoker.TryBeginInvoke((EventHandler<AutoUpdate.EventArgsUpdateFound>)AutoUpdateFound, sender, e))
                return;

            if (EventUpdateFound != null && iHelper.OptionPageUpdates.AutoUpdate)
            {
                EventUpdateFound(sender, e);
            }
        }

        private void ModelBrowserLocationChanged(object sender, EventArgs e)
        {
            if (sender == iModelBrowser)
            {
                iHelper.LastLocation.BreadcrumbTrail = iModelBrowser.BreadcrumbTrail;
            }
        }

        private IInvoker iInvoker;
        private HelperKinskyDesktop iHelper;
        private HttpServer iHttpServer;
        private HttpClient iHttpClient;
        private PlaySupport iPlaySupport;
        private SaveSupport iSaveSupport;
        private ViewSaveSupport iViewSaveSupport;
        private LocalPlaylists iLocalPlaylists;
        private SharedPlaylists iSharedPlaylists;
        private MediaProviderLibrary iLibrary;
        private MediaProviderSupport iSupport;
        private ContentDirectoryLocator iLocator;
        private ViewMaster iMediatorViews;
        private Mediator iMediator;
        private DropConverterRoot iDropConverterExpand;
        private DropConverterRoot iDropConverterNoExpand;
        private AutoUpdate iAutoUpdate;

        private ModelBrowser iModelBrowser;
        private ModelSelectionList<Linn.Kinsky.Room> iModelRoomList;
        private ModelSelectionList<Linn.Kinsky.Source> iModelSourceList;
        private ModelPlaylistAux iModelPlaylistAux;
        private ModelPlaylistReceiver iModelPlaylistReceiver;
        private ModelPlaylistRadio iModelPlaylistRadio;
        private ModelPlaylistDs iModelPlaylistDs;
        private ModelTrack iModelTrack;
        private ModelTransport iModelTransport;
        private ModelVolumeControl iModelVolume;
        private ModelMediaTime iModelMediaTime;

        private ModelTooltips iModelTooltips;
    }


    // Simple helper class to allow subscribing to the tooltip option a little easier
    public class EventArgsTooltips : EventArgs
    {
        public EventArgsTooltips(bool aShow)
        {
            Show = aShow;
        }

        public readonly bool Show;
    }

    public class ModelTooltips
    {
        public ModelTooltips(OptionBool aShowTooltips)
        {
            iShowTooltips = aShowTooltips;
            iShowTooltips.EventValueChanged += ShowTooltipsChanged;
        }

        public event EventHandler<EventArgsTooltips> EventChanged
        {
            add
            {
                iEventChanged += value;
                value(iShowTooltips, new EventArgsTooltips(iShowTooltips.Native));
            }
            remove
            {
                iEventChanged -= value;
            }
        }

        private void ShowTooltipsChanged(object sender, EventArgs e)
        {
            if (iEventChanged != null)
            {
                iEventChanged(sender, new EventArgsTooltips(iShowTooltips.Native));
            }
        }

        private OptionBool iShowTooltips;
        private event EventHandler<EventArgsTooltips> iEventChanged;
    }


    public class ModelBrowser
    {
        public ModelBrowser(IContainer aRoot, BreadcrumbTrail aBreadcrumbTrail, IInvoker aInvoker)
        {
            iInvoker = aInvoker;
            iRoot = aRoot;
            iLocator = null;

            iBrowser = null;
            iLocation = null;
            iSelectedId = string.Empty;
            iBreadcrumbTrail = aBreadcrumbTrail;
        }

        public void Start()
        {
            Browse(iBreadcrumbTrail);
        }

        public BreadcrumbTrail BreadcrumbTrail
        {
            get { return iBreadcrumbTrail; }
        }

        public Location Location
        {
            get { return iLocation; }
        }

        public string SelectedId
        {
            get { return iSelectedId; }
        }

        public void Up(uint aLevels)
        {
            if (iLocation != null)
            {
                // already have existing location so can just go straight to the browser
                Assert.Check(iBrowser != null);
                iBrowser.Up(aLevels);
            }
            else
            {
                // do not have existing location - build up the new breadcrumb trail from the existing trail
                BreadcrumbTrail newTrail = iBreadcrumbTrail.TruncateEnd((int)aLevels);
                Browse(newTrail);
            }
        }

        public void Down(container aContainer)
        {
            Assert.Check(iBrowser != null);
            Assert.Check(iLocation != null);

            iBrowser.Down(aContainer);
        }

        public void Browse(BreadcrumbTrail aBreadcrumbTrail)
        {
            // browsing to a new place that we have no Location for
            iLocation = null;
            iSelectedId = string.Empty;
            iBreadcrumbTrail = aBreadcrumbTrail;

            // create and start a new locator to find the location
            iLocator = new LocatorAsync(iRoot, iBreadcrumbTrail, 3, 3000);
            iLocator.Locate(LocatorFinished);

            // notify that the breadcrumb has changed
            if (EventBreadcrumbChanged != null)
            {
                EventBreadcrumbChanged(this, EventArgs.Empty);
            }
        }

        public ContainerContent CreateContainerContent(IContentHandler aHandler)
        {
            Assert.Check(iLocation != null);

            return new ContainerContent(iLocation, aHandler, iInvoker);
        }

        public event EventHandler<EventArgs> EventBreadcrumbChanged;
        public event EventHandler<EventArgs> EventLocationChanged;
        public event EventHandler<EventArgs> EventLocationFailed;

        private void LocatorFinished(LocatorAsync aLocator, Location aLocation)
        {
            // run in main thread
            if (iInvoker.TryBeginInvoke((Action<LocatorAsync, Location>)LocatorFinished, aLocator, aLocation))
                return;

            // ignore locators that we are no longer interested in
            if (!iLocator.Equals(aLocator))
                return;

            if (aLocation != null)
            {
                if (iBrowser == null)
                {
                    // first time a valid location has been found - create the browser
                    iBrowser = new Browser(aLocation);
                    iBrowser.EventLocationChanged += LocationChanged;
                    // refresh the browser to fire the location changed event
                    iBrowser.Refresh();
                }
                else
                {
                    // browser to the found location
                    iBrowser.Browse(aLocation);
                }
            }
            else
            {
                // location could not be found - retry
                if (EventLocationFailed != null)
                {
                    EventLocationFailed(this, EventArgs.Empty);
                }
            }
        }

        private void LocationChanged(object sender, EventArgs e)
        {
            // run in main thread
            if (iInvoker.TryBeginInvoke((EventHandler<EventArgs>)LocationChanged, sender, e))
                return;

            // get the current location from the browser - no need to lock the data stored here since it
            // is only accessed from the main thread
            iBrowser.Lock();
            iLocation = iBrowser.Location;
            iSelectedId = iBrowser.SelectedId;
            iBrowser.Unlock();

            iBreadcrumbTrail = iLocation.BreadcrumbTrail;

            // notify the upper layers that a new location has been browsed
            if (EventLocationChanged != null)
            {
                EventLocationChanged(this, EventArgs.Empty);
            }
        }

        private class LocatorAsync
        {
            public LocatorAsync(IContainer aRoot, BreadcrumbTrail aBreadcrumbTrail, int aAttempts, int aIntervalMs)
            {
                iRoot = aRoot;
                iBreadcrumbTrail = aBreadcrumbTrail;
                iAttempts = aAttempts;
                iIntervalMs = aIntervalMs;
            }

            public delegate void DFinished(LocatorAsync aLocator, Location aLocation);

            public void Locate(DFinished aCallback)
            {
                Thread t = new Thread(new ThreadStart(() =>
                {
                    int attempts = iAttempts;
                    while (attempts > 0)
                    {
                        // try to locate
                        Location location = Locator.Locate(iRoot, iBreadcrumbTrail);

                        // notify and return if successful
                        if (location != null)
                        {
                            aCallback(this, location);
                            return;
                        }

                        // wait for next attempt
                        attempts--;
                        if (attempts > 0)
                        {
                            Thread.Sleep(iIntervalMs);
                        }
                    }

                    // unsuccessful
                    aCallback(this, null);
                }));

                t.IsBackground = true;
                t.Name = "Locator";
                t.Start();
            }

            private IContainer iRoot;
            private BreadcrumbTrail iBreadcrumbTrail;
            private int iAttempts;
            private int iIntervalMs;
        }

        private IInvoker iInvoker;
        private LocatorAsync iLocator;
        private IContainer iRoot;
        private IBrowser iBrowser;
        private Location iLocation;
        private BreadcrumbTrail iBreadcrumbTrail;
        private string iSelectedId;
    }


    // Model class for the contents of a container - uses the IInvoker to ensure that
    // changes to the data only happen in the main thread and any events for changes to
    // this data are also, consequently in the main thread
    public class ContainerContent : IContentHandler
    {
        public ContainerContent(Location aLocation, IContentHandler aHandler, IInvoker aInvoker)
        {
            iInvoker = aInvoker;
            iHandler = aHandler;
            iLocation = aLocation;
            iObjects = new upnpObject[] { };

            iCollectorStartIndex = -1;
            iCollectorCount = 0;

            // create the content collector - this will create the thread and run
            // until the Open callback is called
            iCollector = ContentCollectorMaster.Create(iLocation.Current, this);
        }

        public void Dispose()
        {
            if (iCollector != null)
            {
                iCollector.Dispose();
                iCollector = null;
                iObjects = new upnpObject[] { };
            }
        }

        public Location Location
        {
            get { return iLocation; }
        }

        public int Count
        {
            get { return iObjects.Length; }
        }

        public upnpObject Object(int aIndex)
        {
            if (iCollector == null)
                return null;

            if (iObjects[aIndex] == null)
            {
                if (aIndex < iCollectorStartIndex || aIndex >= iCollectorStartIndex + iCollectorCount)
                {
                    // this item is not currently cached and it is outside the range of the existing
                    // request - start a new request
                    int rem = aIndex % 100;
                    if (rem < 25)
                    {
                        iCollectorStartIndex = (100 * (aIndex / 100)) - 50;
                        if (iCollectorStartIndex < 0)
                        {
                            iCollectorStartIndex = 0;
                        }
                    }
                    else if (rem < 75)
                    {
                        iCollectorStartIndex = (100 * (aIndex / 100));
                    }
                    else
                    {
                        iCollectorStartIndex = (100 * (aIndex / 100)) + 50;
                    }

                    Assert.Check(iCollectorStartIndex >= 0 && iCollectorStartIndex < iObjects.Length);

                    // skip any items that are already loaded
                    while (iObjects[iCollectorStartIndex] != null && iCollectorStartIndex < aIndex)
                        iCollectorStartIndex++;

                    if (iCollectorStartIndex + 100 > iObjects.Length)
                    {
                        iCollectorCount = iObjects.Length - iCollectorStartIndex;
                    }
                    else
                    {
                        iCollectorCount = 100;
                    }

                    iCollector.Range((uint)iCollectorStartIndex, (uint)iCollectorCount);
                }
            }

            return iObjects[aIndex];
        }

        public bool UsesCollector(IContentCollector aCollector)
        {
            return aCollector == iCollector;
        }

        #region IContentHandler implementation
        private delegate void DOpen(IContentCollector aCollector, uint aCount);
        public void Open(IContentCollector aCollector, uint aCount)
        {
            // run in the main thread
            if (iInvoker.TryBeginInvoke((DOpen)Open, aCollector, aCount))
                return;

            if (iCollector == null)
                return;

            // it should not be possible for events to come from a different collector
            Assert.Check(aCollector == iCollector);

            // create an array containing null values for each item
            iObjects = new upnpObject[aCount];

            // notify upper layers
            iHandler.Open(aCollector, aCount);
        }

        public void Item(IContentCollector aCollector, uint aIndex, upnpObject aObject)
        {
        }

        private delegate void DItems(IContentCollector aCollector, uint aStartIndex, IList<upnpObject> aObjects);
        public void Items(IContentCollector aCollector, uint aStartIndex, IList<upnpObject> aObjects)
        {
            // run in the main thread
            if (iInvoker.TryBeginInvoke((DItems)Items, aCollector, aStartIndex, aObjects))
                return;

            if (iCollector == null)
                return;

            // it should not be possible for events to come from a different collector
            Assert.Check(aCollector == iCollector);

            // update all the objects in the array
            for (int i=0 ; i<aObjects.Count ; i++)
            {
                if (aStartIndex + i < iObjects.Length)
                {
                    iObjects[aStartIndex + i] = aObjects[i];
                }
            }

            // notify upper layers
            iHandler.Items(aCollector, aStartIndex, aObjects);
        }

        private delegate void DContentError(IContentCollector aCollector, string aMessage);
        public void ContentError(IContentCollector aCollector, string aMessage)
        {
            // run in the main thread
            if (iInvoker.TryBeginInvoke((DContentError)ContentError, aCollector, aMessage))
                return;

            if (iCollector == null)
                return;

            // it should not be possible for events to come from a different collector
            Assert.Check(aCollector == iCollector);

            // notify upper layers
            iHandler.ContentError(aCollector, aMessage);
        }
        #endregion IContentHandler implementation

        private IInvoker iInvoker;
        private IContentHandler iHandler;
        private Location iLocation;
        private IContentCollector iCollector;
        private upnpObject[] iObjects;

        private int iCollectorStartIndex;
        private int iCollectorCount;
    }


    // Comparer implementations for rooms and sources
    public class ComparerRoom : IComparer<Linn.Kinsky.Room>
    {
        public int Compare(Linn.Kinsky.Room x, Linn.Kinsky.Room y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    public class ComparerSource : IComparer<Linn.Kinsky.Source>
    {
        public int Compare(Linn.Kinsky.Source x, Linn.Kinsky.Source y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }


    // interface for a list of Kinsky items that can have single selection - this
    // interface is created because the upper layers of the application will only see
    // this and not the IViewWidgetSelector interface
    public interface IModelSelectionList<T>
    {
        IList<T> Items { get; }
        T SelectedItem { get; set; }

        // events will be triggered in the main thread
        event EventHandler<EventArgs> EventChanged;
    }

    // Class for a selection of kinsky objects that can have single selection.
    // The use of IInvoker means that this model only ever changes in the main
    // application thread, so the contents of this model and the views should
    // always be perfectly synchronised
    public class ModelSelectionList<T> : IViewWidgetSelector<T>, IModelSelectionList<T> where T : class
    {
        public ModelSelectionList(IInvoker aInvoker, IComparer<T> aComparer)
        {
            iInvoker = aInvoker;
            iItems = new List<T>();
            iSelectedItem = null;
        }


        #region IModelSelectionList implementation
        public IList<T> Items
        {
            get { return iItems; }
        }

        public T SelectedItem
        {
            get
            {
                return iSelectedItem;
            }
            set
            {
                iSelectedItem = value;

                // send notification to the Kinsky layer that selection has changed
                if (EventSelectionChanged != null)
                {
                    EventSelectionChanged(this, new EventArgsSelection<T>(iSelectedItem));
                }

                // notify upper layers - for the room selector, the Mediator does not
                // send out the SetSelected event when the room is set - need to do
                // it manually here
                if (EventChanged != null)
                {
                    EventChanged(this, EventArgs.Empty);
                }
            }
        }

        // these events from this class occur in the main thread
        public event EventHandler<EventArgs> EventChanged;
        #endregion IModelSelectionList implementation


        #region IViewWidgetSelector implementation
        public void Open()
        {
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            iItems.Clear();
            iSelectedItem = null;

            // this method called from Kinsky layer, notify upper layers of change
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DInsertItem(int aIndex, T aItem);
        public void InsertItem(int aIndex, T aItem)
        {
            iInvoker.BeginInvoke((DInsertItem)DoInsertItem, aIndex, aItem);
        }
        private void DoInsertItem(int aIndex, T aItem)
        {
            iItems.Insert(aIndex, aItem);

            // this method called from Kinsky layer, notify upper layers of change
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DRemoveItem(T aItem);
        public void RemoveItem(T aItem)
        {
            iInvoker.BeginInvoke((DRemoveItem)DoRemoveItem, aItem);
        }
        private void DoRemoveItem(T aItem)
        {
            iItems.Remove(aItem);

            if (aItem == iSelectedItem)
            {
                iSelectedItem = null;
            }

            // this method called from Kinsky layer, notify upper layers of change
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DItemChanged(T aItem);
        public void ItemChanged(T aItem)
        {
            iInvoker.BeginInvoke((DItemChanged)DoItemChanged, aItem);
        }
        private void DoItemChanged(T aItem)
        {
            // this method called from Kinsky layer, notify upper layers of change
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSetSelected(T aItem);
        public void SetSelected(T aItem)
        {
            iInvoker.BeginInvoke((DSetSelected)DoSetSelected, aItem);
        }
        private void DoSetSelected(T aItem)
        {
            iSelectedItem = aItem;

            // this method called from Kinsky layer, notify upper layers of change
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgsSelection<T>> EventSelectionChanged;
        #endregion IViewWidgetSelector implementation

        private IInvoker iInvoker;
        private List<T> iItems;
        private T iSelectedItem;
    }


    // interface for the ModelPlaylistAux that contains the relevant parts for
    // upper layers and allows the IViewWidgetPlaylistAux interface to be hidden
    public interface IModelPlaylistAux
    {
        string Type { get; }

        // events will be triggered in the main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventClose;
    }

    // Model for receiving kinsky notifications for the aux playlist - this is a bit of a
    // pointless class but is here for consistency
    public class ModelPlaylistAux : IModelPlaylistAux, IViewWidgetPlaylistAux
    {
        public ModelPlaylistAux(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
            iType = "Aux";
        }


        #region IModelPlaylistAux implementation
        public string Type
        {
            get { return iType; }
        }

        // these events from this class occur in the main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventClose;
        #endregion IModelPlaylistAux implementation


        #region IViewWidgetPlaylistAux implementation
        private delegate void DOpen(string aType);
        public void Open(string aType)
        {
            iInvoker.BeginInvoke((DOpen)DoOpen, aType);
        }
        private void DoOpen(string aType)
        {
            iType = aType;

            // called from the kinsky layer - notify the upper layers
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }
        #endregion IViewWidgetPlaylistAux implementation

        private IInvoker iInvoker;
        private string iType;
    }


    // interface for the ModelPlaylistReceiver that contains the relevant parts for
    // upper layers and allows the IViewWidgetPlaylistReceiver interface to be hidden
    public interface IModelPlaylistReceiver
    {
        IList<ModelSender> Senders { get; }
        int CurrentSenderIndex { get; set; }
        Channel Channel {get;}

        // events will be triggered in the main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventClose;
        event EventHandler<EventArgs> EventSendersChanged;
        event EventHandler<EventArgs> EventCurrentChanged;
    }

    // Model class to receive kinsky notifications about the receiver playlist
    public class ModelPlaylistReceiver : IModelPlaylistReceiver, IViewWidgetPlaylistReceiver
    {
        public ModelPlaylistReceiver(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
            iSenders = new List<ModelSender>();
            iChannel = null;
        }


        #region IModelPlaylistReceiver implementation
        public IList<ModelSender> Senders
        {
            get { return iSenders.AsReadOnly(); }
        }

        public Channel Channel
        {
            get
            {
                return iChannel;
            }
        }

        public int CurrentSenderIndex
        {
            get { return iCurrent; }
            set
            {
                if (value >= 0 && value < iSenders.Count)
                {
                    // send event to kinsky to change the channel - let the changed
                    // event come back up from kinsky before changing the index here
                    EventHandler<EventArgsSetChannel> ev = EventSetChannel;
                    if (ev != null)
                    {
                        List<upnpObject> items = new List<upnpObject>();
                        items.Add(iSenders[value].Metadata[0]);

                        ev(this, new EventArgsSetChannel(new MediaRetrieverNoRetrieve(items)));
                    }
                }
            }
        }

        // these events from this class occur in the main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventInitialised;
        public event EventHandler<EventArgs> EventClose;
        public event EventHandler<EventArgs> EventSendersChanged;
        public event EventHandler<EventArgs> EventCurrentChanged;
        #endregion IModelPlaylistReceiver implementation


        #region IViewWidgetPlaylistReceiver implementation
        private delegate void DOpen();
        public void Open()
        {
            iInvoker.BeginInvoke((DOpen)DoOpen);
        }
        private void DoOpen()
        {
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            iSenders.Clear();
            iChannel = null;
            iCurrent = -1;

            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        private delegate void DInitialised();
        public void Initialised()
        {
            iInvoker.BeginInvoke((DInitialised)DoInitialised);
        }
        private void DoInitialised()
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private delegate void DSetSenders(IList<ModelSender> aSenders);
        public void SetSenders(IList<ModelSender> aSenders)
        {
            iInvoker.BeginInvoke((DSetSenders)DoSetSenders, aSenders);
        }
        private void DoSetSenders(IList<ModelSender> aSenders)
        {
            iSenders.Clear();
            iSenders.AddRange(aSenders);
            UpdateCurrent();

            if (EventSendersChanged != null)
            {
                EventSendersChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSetChannel(Channel aChannel);
        public void SetChannel(Channel aChannel)
        {
            iInvoker.BeginInvoke((DSetChannel)DoSetChannel, aChannel);
        }
        private void DoSetChannel(Channel aChannel)
        {
            iChannel = aChannel;

            if (UpdateCurrent() && EventCurrentChanged != null)
            {
                EventCurrentChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSave();
        public void Save()
        {
        }

        public event EventHandler<EventArgsSetChannel> EventSetChannel;
        #endregion IViewWidgetPlaylistReceiver implementation

        private bool UpdateCurrent()
        {
            int index = -1;

            if (iChannel != null)
            {
                for (int i=0 ; i<iSenders.Count ; i++)
                {
                    if (iSenders[i].Metadata[0].Title == iChannel.DidlLite[0].Title)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index != iCurrent)
            {
                iCurrent = index;
                return true;
            }

            return false;
        }

        private IInvoker iInvoker;
        private List<ModelSender> iSenders;
        private int iCurrent;
        private Channel iChannel;
    }


    // Model interface for the kinsky radio playlist - hides the IViewWidgetPlaylistRadio
    // from the upper layers
    public interface IModelPlaylistRadio
    {
        IList<MrItem> Presets { get; }
        int PresetIndex { get; set; }

        // events are triggered in the main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventClose;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventPresetsChanged;
        event EventHandler<EventArgs> EventPresetIndexChanged;
    }

    // Model class for interfacing with the kinsky radio playlist
    public class ModelPlaylistRadio : IModelPlaylistRadio, IViewWidgetPlaylistRadio
    {
        public ModelPlaylistRadio(IInvoker aInvoker, IViewSaveSupport aSaveSupport)
        {
            iInvoker = aInvoker;
            iPresets = new List<MrItem>();

            iSaveSupport = aSaveSupport;
        }


        #region IModelPlaylistRadio implementation
        public IList<MrItem> Presets
        {
            get { return iPresets; }
        }

        public int PresetIndex
        {
            get
            {
                return iPresetIndex;
            }
            set
            {
                if (value >= 0 && value < iPresets.Count)
                {
                    // don't explicitly set iPresetIndex here - send notification to the kinsky layer
                    // and let kinsky eventing do it
                    EventHandler<EventArgsSetPreset> ev = EventSetPreset;
                    if (ev != null)
                    {
                        ev(this, new EventArgsSetPreset(iPresets[value]));
                    }
                }
            }
        }

        // these public events are triggered in the main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventClose;
        public event EventHandler<EventArgs> EventInitialised;
        public event EventHandler<EventArgs> EventPresetsChanged;
        public event EventHandler<EventArgs> EventPresetIndexChanged;
        #endregion IModelPlaylistRadio implementation


        #region IViewWidgetPlaylistRadio implementation
        private delegate void DOpen();
        public void Open()
        {
            iInvoker.BeginInvoke((DOpen)DoOpen);
        }
        private void DoOpen()
        {
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            iPresets.Clear();
            iPresetIndex = -1;
            iChannel = null;

            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        private delegate void DInitialised();
        public void Initialised()
        {
            iInvoker.BeginInvoke((DInitialised)DoInitialised);
        }
        private void DoInitialised()
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private delegate void DSetPresets(IList<MrItem> aPresets);
        public void SetPresets(IList<MrItem> aPresets)
        {
            iInvoker.BeginInvoke((DSetPresets)DoSetPresets, aPresets);
        }
        private void DoSetPresets(IList<MrItem> aPresets)
        {
            iPresets.Clear();
            iPresets.AddRange(aPresets);

            if (EventPresetsChanged != null)
            {
                EventPresetsChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSetChannel(Channel aChannel);
        public void SetChannel(Channel aChannel)
        {
            iInvoker.BeginInvoke((DSetChannel)DoSetChannel, aChannel);
        }
        private void DoSetChannel(Channel aChannel)
        {
            iChannel = aChannel;
        }

        private delegate void DSetPreset(int aPresetIndex);
        public void SetPreset(int aPresetIndex)
        {
            iInvoker.BeginInvoke((DSetPreset)DoSetPreset, aPresetIndex);
        }
        private void DoSetPreset(int aPresetIndex)
        {
            iPresetIndex = aPresetIndex;

            if (EventPresetIndexChanged != null)
            {
                EventPresetIndexChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSave();
        public void Save()
        {
            iInvoker.BeginInvoke((DSave)DoSave);
        }
        private void DoSave()
        {
            List<upnpObject> list = new List<upnpObject>();

            if (iChannel != null)
            {
                list.AddRange(iChannel.DidlLite);
            }

            iSaveSupport.Save(list);
        }

        public event EventHandler<EventArgsSetPreset> EventSetPreset;
        public event EventHandler<EventArgsSetChannel> EventSetChannel;
        #endregion IViewWidgetPlaylistRadio implementation


        private IInvoker iInvoker;

        private int iPresetIndex;
        private Channel iChannel;
        private List<MrItem> iPresets;

        private IViewSaveSupport iSaveSupport;
    }


    // Model interface for the kinsky ds playlist - hides the IViewWidgetPlaylist
    // from the upper layers
    public class ModelListGroup
    {
        public ModelListGroup(int aFirstIndex, int aLastIndex)
        {
            FirstIndex = aFirstIndex;
            LastIndex = aLastIndex;
        }

        public readonly int FirstIndex;
        public readonly int LastIndex;

        public int Count
        {
            get { return LastIndex - FirstIndex + 1; }
        }
    }

    public interface IModelPlaylistDs
    {
        IList<MrItem> Playlist { get; }

        MrItem Track { get; }
        int TrackIndex { get; set; }

        IList<ModelListGroup> Groups { get; }

        void DeleteTracks(IList<MrItem> aTracks);
        void MoveTracks(IList<MrItem> aTracks, uint aAfterId);
        void InsertTracks(IMediaRetriever aRetriever, uint aAfterId);
        void SaveTracks(IList<MrItem> aTracks);

        // events are triggered in the main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventClose;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventPlaylistChanged;
        event EventHandler<EventArgs> EventTrackChanged;
    }

    // Model class for interfacing with the kinsky ds playlist
    public class ModelPlaylistDs : IModelPlaylistDs, IViewWidgetPlaylist
    {
        public ModelPlaylistDs(IInvoker aInvoker, IViewSaveSupport aSaveSupport)
        {
            iInvoker = aInvoker;

            iSaveSupport = aSaveSupport;
        }


        #region IModelPlaylistDs implementation
        public IList<MrItem> Playlist
        {
            get { return iPlaylist.AsReadOnly(); }
        }

        public MrItem Track
        {
            get { return iTrack; }
        }

        public int TrackIndex
        {
            get
            {
                return iTrackIndex;
            }
            set
            {
                if (value >= 0 && value < iPlaylist.Count)
                {
                    // don't explicitly set iTrackIndex here - send notification to the kinsky layer
                    // and let kinsky eventing do it
                    EventHandler<EventArgsSeekTrack> ev = EventSeekTrack;
                    if (ev != null)
                    {
                        ev(this, new EventArgsSeekTrack((uint)value));
                    }
                }
            }
        }

        public IList<ModelListGroup> Groups
        {
            get { return iGroups.AsReadOnly(); }
        }

        public void DeleteTracks(IList<MrItem> aTracks)
        {
            EventHandler<EventArgsPlaylistDelete> ev = EventPlaylistDelete;
            if (ev != null)
            {
                ev(this, new EventArgsPlaylistDelete(aTracks));
            }
        }

        public void MoveTracks(IList<MrItem> aTracks, uint aAfterId)
        {
            EventHandler<EventArgsPlaylistMove> ev = EventPlaylistMove;
            if(ev != null)
            {
                ev(this, new EventArgsPlaylistMove(aAfterId, aTracks));
            }
        }

        public void InsertTracks(IMediaRetriever aRetriever, uint aAfterId)
        {
            EventHandler<EventArgsPlaylistInsert> ev = EventPlaylistInsert;
            if (ev != null)
            {
                ev(this, new EventArgsPlaylistInsert(aAfterId, aRetriever));
            }
        }

        public void SaveTracks(IList<MrItem> aTracks)
        {
            Save(aTracks);
        }

        // events are triggered in the main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventClose;
        public event EventHandler<EventArgs> EventInitialised;
        public event EventHandler<EventArgs> EventPlaylistChanged;
        public event EventHandler<EventArgs> EventTrackChanged;
        #endregion IModelPlaylistDs implementation


        #region IViewWidgetPlaylist implementation
        private delegate void DOpen();
        public void Open()
        {
            iInvoker.BeginInvoke((DOpen)DoOpen);
        }
        private void DoOpen()
        {
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            iPlaylist.Clear();
            iTrack = null;
            iTrackIndex = -1;
            iGroups.Clear();

            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        private delegate void DInitialised();
        public void Initialised()
        {
            iInvoker.BeginInvoke((DInitialised)DoInitialised);
        }
        private void DoInitialised()
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private delegate void DSetPlaylist(IList<MrItem> aPlaylist);
        public void SetPlaylist(IList<MrItem> aPlaylist)
        {
            iInvoker.BeginInvoke((DSetPlaylist)DoSetPlaylist, aPlaylist);
        }
        private void DoSetPlaylist(IList<MrItem> aPlaylist)
        {
            iPlaylist.Clear();
            iPlaylist.AddRange(aPlaylist);
            UpdateTrackIndex();
            BuildGroupList();

            if (EventPlaylistChanged != null)
            {
                EventPlaylistChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSetTrack(MrItem aTrack);
        public void SetTrack(MrItem aTrack)
        {
            iInvoker.BeginInvoke((DSetTrack)DoSetTrack, aTrack);
        }
        private void DoSetTrack(MrItem aTrack)
        {
            iTrack = aTrack;
            UpdateTrackIndex();

            if (EventTrackChanged != null)
            {
                EventTrackChanged(this, EventArgs.Empty);
            }
        }

        private delegate void DSave();
        public void Save()
        {
            iInvoker.BeginInvoke((DSave)DoSave);
        }
        private void DoSave()
        {
            Save(iPlaylist);
        }

        private delegate void DDelete();
        public void Delete()
        {
            // this has been called from the kinsky layer - send it right back!
            if (EventPlaylistDeleteAll != null)
            {
                EventPlaylistDeleteAll(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgsSeekTrack> EventSeekTrack;
        public event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;
        public event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;
        public event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;
        public event EventHandler<EventArgs> EventPlaylistDeleteAll;
        #endregion IViewWidgetPlaylist implementation


        private void UpdateTrackIndex()
        {
            iTrackIndex = -1;

            for (int i=0 ; i<iPlaylist.Count ; i++)
            {
                if (iPlaylist[i] == iTrack)
                {
                    iTrackIndex = i;
                    break;
                }
            }
        }

        private void Save(IList<MrItem> aTracks)
        {
            List<upnpObject> list = new List<upnpObject>();
            foreach (MrItem track in aTracks)
            {
                list.Add(track.DidlLite[0]);
            }

            iSaveSupport.Save(list);
        }

        private void BuildGroupList()
        {
            iGroups.Clear();

            int firstIndex = 0;
            while (firstIndex < iPlaylist.Count)
            {
                // group is currently defined by album title and album artist
                string groupAlbum = DidlLiteAdapter.Album(iPlaylist[firstIndex].DidlLite[0]);
                string groupArtist = DidlLiteAdapter.AlbumArtist(iPlaylist[firstIndex].DidlLite[0]);

                int lastIndex = firstIndex + 1;

                // if the album field is empty, then this playlist item is in its own group of 1 item,
                // if not, determine the subsequent tracks that are part of the group
                if (!string.IsNullOrEmpty(groupAlbum))
                {
                    while (lastIndex < iPlaylist.Count)
                    {
                        string album = DidlLiteAdapter.Album(iPlaylist[lastIndex].DidlLite[0]);
                        string artist = DidlLiteAdapter.AlbumArtist(iPlaylist[lastIndex].DidlLite[0]);

                        if (album == groupAlbum && artist == groupArtist)
                        {
                            // this track is part of the current group - move to next track
                            lastIndex++;
                        }
                        else
                        {
                            // this track is not part of the current group
                            break;
                        }
                    }
                }

                // lastIndex is currently the first item of the next group
                lastIndex--;

                // add the group
                iGroups.Add(new ModelListGroup(firstIndex, lastIndex));

                // move to start of next group
                firstIndex = lastIndex + 1;
            }
        }

        private IInvoker iInvoker;

        private List<MrItem> iPlaylist = new List<MrItem>();
        private int iTrackIndex;
        private MrItem iTrack;
        private List<ModelListGroup> iGroups = new List<ModelListGroup>();

        private IViewSaveSupport iSaveSupport;
    }


    // Model interface for the current track - hides the IViewWidgetTrack interface
    // from the upper layers
    public interface IModelTrack
    {
        Track Current { get; }

        // this event will be fired in the main thread
        event EventHandler<EventArgs> EventTrackChanged;
    }

    // Class containing all track data to be displayed
    public class Track
    {
        public Track()
        {
        }

        public Track(Track aCopy)
        {
            Title = aCopy.Title;
            Artist = aCopy.Artist;
            Album = aCopy.Album;
            ArtworkUri = aCopy.ArtworkUri;

            Bitrate = aCopy.Bitrate;
            SampleRate = aCopy.SampleRate;
            BitDepth = aCopy.BitDepth;
            Codec = aCopy.Codec;
            Lossless = aCopy.Lossless;
        }

        public bool IsEmpty
        {
            get
            {
                return (Title == string.Empty) &&
                       (Artist == string.Empty) &&
                       (Album == string.Empty) &&
                       (ArtworkUri == string.Empty) &&
                       (Bitrate == 0) &&
                       (SampleRate == 0) &&
                       (BitDepth == 0) &&
                       (Codec == string.Empty) &&
                       (Lossless == false);
            }
        }

        public string Title = string.Empty;
        public string Artist = string.Empty;
        public string Album = string.Empty;
        public string ArtworkUri = string.Empty;
        public uint Bitrate = 0;
        public float SampleRate = 0;
        public uint BitDepth = 0;
        public string Codec = string.Empty;
        public bool Lossless = false;
    }

    // Model for the current track - this implementation of IViewWidget... is slightly
    // different from others. Most others always immediately dispatch the calls from the
    // kinsky layer to the main thread so that these models essentially live in the same
    // thread as the views, giving great reduction in complexity.
    // The calls to this class from the kinsky layer do not do this - the model data is stored
    // and is primarily accessed in the kinsky threads. It is only when the Update() occurs
    // that the current track data is copied and pushed over to the main thread for updating views
    public class ModelTrack : IModelTrack, IViewWidgetTrack
    {
        public ModelTrack(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
            iPending = new Track();
            iCurrent = new Track();
        }


        #region IModelTrack implementation
        public Track Current
        {
            get { return iCurrent; }
        }

        // this event will be fired in the main thread
        public event EventHandler<EventArgs> EventTrackChanged;
        #endregion IModelTrack implementation


        #region IViewWidgetTrack implementation
        public void Open()
        {
            // clear the pending track info - do not update upper layers yet
            iPending = new Track();
        }

        public void Close()
        {
            // reset the pending track info and push it to the upper layers
            iPending = new Track();
            Update();
        }

        public void Initialised()
        {
            // notify the upper layers of changes to the track
            Update();
        }

        public void SetItem(upnpObject aObject)
        {
            // set pending data and leave updating to upper layers to Update() call
            if (aObject != null)
            {
                iPending.Title = DidlLiteAdapter.Title(aObject);
                iPending.Artist = DidlLiteAdapter.Artist(aObject);
                iPending.Album = DidlLiteAdapter.Album(aObject);

                Uri uri = DidlLiteAdapter.ArtworkUri(aObject);
                iPending.ArtworkUri = (uri != null) ? uri.AbsoluteUri : string.Empty;
            }
            else
            {
                iPending = new Track();
            }
        }

        public void SetMetatext(upnpObject aObject)
        {
            // set pending data and leave updating to upper layers to Update() call
            if (aObject != null)
            {
                iPending.Album = DidlLiteAdapter.Title(aObject);
                iPending.Artist = string.Empty;
            }
        }

        public void SetBitrate(uint aBitrate)
        {
            // set pending data and leave updating to upper layers to Update() call
            iPending.Bitrate = aBitrate;
        }

        public void SetSampleRate(float aSampleRate)
        {
            // set pending data and leave updating to upper layers to Update() call
            iPending.SampleRate = aSampleRate;
        }

        public void SetBitDepth(uint aBitDepth)
        {
            // set pending data and leave updating to upper layers to Update() call
            iPending.BitDepth = aBitDepth;
        }

        public void SetCodec(string aCodec)
        {
            // set pending data and leave updating to upper layers to Update() call
            iPending.Codec = aCodec;
        }

        public void SetLossless(bool aLossless)
        {
            // set pending data and leave updating to upper layers to Update() call
            iPending.Lossless = aLossless;
        }

        public void Update()
        {
            // copy the pending track data and send notification to the main thread
            Track track = new Track(iPending);

            iInvoker.BeginInvoke(new DNotify(Notify), track);
        }
        #endregion IViewWidgetTrack implementation

        private delegate void DNotify(Track aTrack);
        private void Notify(Track aTrack)
        {
            // this function should only be called from the main thread
            Assert.Check(!iInvoker.InvokeRequired);

            // update the current track info and notify
            iCurrent = aTrack;

            if (EventTrackChanged != null)
            {
                EventTrackChanged(this, EventArgs.Empty);
            }
        }

        private IInvoker iInvoker;
        private Track iPending;
        private Track iCurrent;
    }


    // Interface for the transport model - so the upper layers are hidden from the
    // IViewWidgetTransportControl interface
    public interface IModelTransport
    {
        bool IsInitialised { get; }
        bool PlayNowEnabled { get; }
        bool PlayNextEnabled { get; }
        bool PlayLaterEnabled { get; }
        bool AllowSkipping { get; }
        bool PauseNotStop { get; }
        bool Dragging { get; }
        ETransportState TransportState { get; }
        uint Duration { get; }

        void Play();
        void Pause();
        void Stop();
        void Previous();
        void Next();

        // these events are triggered in the main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventClose;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventChanged;
    }

    // Class that implements the IViewWidgetTransportControl kinsky interface
    public class ModelTransport : IModelTransport, IViewWidgetTransportControl
    {
        public ModelTransport(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
        }

        #region IModelTransport implementation
        public bool IsInitialised
        {
            get { return iInitialised; }
        }

        public bool PlayNowEnabled
        {
            get { return iPlayNowEnabled; }
        }

        public bool PlayNextEnabled
        {
            get { return iPlayNextEnabled; }
        }

        public bool PlayLaterEnabled
        {
            get { return iPlayLaterEnabled; }
        }

        public bool AllowSkipping
        {
            get { return iAllowSkipping; }
        }

        public bool PauseNotStop
        {
            get { return (iAllowPausing && iDuration != 0); }
        }

        public bool Dragging
        {
            get { return iDragging; }
        }

        public ETransportState TransportState
        {
            get { return iTransportState; }
        }

        public uint Duration
        {
            get { return iDuration; }
        }

        public void Play()
        {
            NotifyKinsky(EventPlay);
        }

        public void Pause()
        {
            NotifyKinsky(EventPause);
        }

        public void Stop()
        {
            NotifyKinsky(EventStop);
        }

        public void Previous()
        {
            NotifyKinsky(EventPrevious);
        }

        public void Next()
        {
            NotifyKinsky(EventNext);
        }

        // these events are triggered in the main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventClose;
        public event EventHandler<EventArgs> EventInitialised;
        public event EventHandler<EventArgs> EventChanged;
        #endregion IModelTransport implementation


        #region IViewWidgetTranasportControl implementation
        private delegate void DOpen();
        public void Open()
        {
            iInvoker.BeginInvoke((DOpen)DoOpen);
        }
        private void DoOpen()
        {
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            iInitialised = false;

            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        private delegate void DInitialised();
        public void Initialised()
        {
            iInvoker.BeginInvoke((DInitialised)DoInitialised);
        }
        private void DoInitialised()
        {
            iInitialised = true;

            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private delegate void DSetPlayNowEnabled(bool aEnabled);
        public void SetPlayNowEnabled(bool aEnabled)
        {
            iInvoker.BeginInvoke((DSetPlayNowEnabled)DoSetPlayNowEnabled, aEnabled);
        }
        private void DoSetPlayNowEnabled(bool aEnabled)
        {
            iPlayNowEnabled = aEnabled;

            NotifyChanged();
        }

        private delegate void DSetPlayNextEnabled(bool aEnabled);
        public void SetPlayNextEnabled(bool aEnabled)
        {
            iInvoker.BeginInvoke((DSetPlayNextEnabled)DoSetPlayNextEnabled, aEnabled);
        }
        private void DoSetPlayNextEnabled(bool aEnabled)
        {
            iPlayNextEnabled = aEnabled;

            NotifyChanged();
        }

        private delegate void DSetPlayLaterEnabled(bool aEnabled);
        public void SetPlayLaterEnabled(bool aEnabled)
        {
            iInvoker.BeginInvoke((DSetPlayLaterEnabled)DoSetPlayLaterEnabled, aEnabled);
        }
        private void DoSetPlayLaterEnabled(bool aEnabled)
        {
            iPlayLaterEnabled = aEnabled;

            NotifyChanged();
        }

        private delegate void DSetDragging(bool aDragging);
        public void SetDragging(bool aDragging)
        {
            iInvoker.BeginInvoke((DSetDragging)DoSetDragging, aDragging);
        }
        private void DoSetDragging(bool aDragging)
        {
            iDragging = aDragging;

            NotifyChanged();
        }

        private delegate void DSetTransportState(ETransportState aTransportState);
        public void SetTransportState(ETransportState aTransportState)
        {
            iInvoker.BeginInvoke((DSetTransportState)DoSetTransportState, aTransportState);
        }
        private void DoSetTransportState(ETransportState aTransportState)
        {
            iTransportState = aTransportState;

            NotifyChanged();
        }

        private delegate void DSetDuration(uint aDuration);
        public void SetDuration(uint aDuration)
        {
            iInvoker.BeginInvoke((DSetDuration)DoSetDuration, aDuration);
        }
        private void DoSetDuration(uint aDuration)
        {
            iDuration = aDuration;

            NotifyChanged();
        }

        private delegate void DSetAllowSkipping(bool aAllowSkipping);
        public void SetAllowSkipping(bool aAllowSkipping)
        {
            iInvoker.BeginInvoke((DSetAllowSkipping)DoSetAllowSkipping, aAllowSkipping);
        }
        private void DoSetAllowSkipping(bool aAllowSkipping)
        {
            iAllowSkipping = aAllowSkipping;

            NotifyChanged();
        }

        private delegate void DSetAllowPausing(bool aAllowPausing);
        public void SetAllowPausing(bool aAllowPausing)
        {
            iInvoker.BeginInvoke((DSetAllowPausing)DoSetAllowPausing, aAllowPausing);
        }
        private void DoSetAllowPausing(bool aAllowPausing)
        {
            iAllowPausing = aAllowPausing;

            NotifyChanged();
        }

        public event EventHandler<EventArgs> EventPause;
        public event EventHandler<EventArgs> EventPlay;
        public event EventHandler<EventArgs> EventStop;
        public event EventHandler<EventArgs> EventPrevious;
        public event EventHandler<EventArgs> EventNext;

        public event EventHandler<EventArgsPlay> EventPlayNow;
        public event EventHandler<EventArgsPlay> EventPlayNext;
        public event EventHandler<EventArgsPlay> EventPlayLater;
        #endregion IViewWidgetTranasportControl implementation


        private void NotifyChanged()
        {
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private void NotifyKinsky(EventHandler<EventArgs> aEvent)
        {
            if (aEvent != null)
            {
                aEvent(this, EventArgs.Empty);
            }
        }

        private IInvoker iInvoker;
        private bool iInitialised;
        private bool iPlayNowEnabled;
        private bool iPlayNextEnabled;
        private bool iPlayLaterEnabled;
        private bool iAllowSkipping;
        private bool iAllowPausing;
        private bool iDragging;
        private ETransportState iTransportState;
        private uint iDuration;
    }


    // interface for the volume control model
    public interface IModelVolumeControl
    {
        uint Volume { get; }
        uint VolumeLimit { get; }
        bool Mute { get; }

        void IncrementVolume();
        void DecrementVolume();
        void ToggleMute();

        // these events are fired in the main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventClose;
        event EventHandler<EventArgs> EventChanged;
    }

    // Implementation of the volume control model
    public class ModelVolumeControl : IModelVolumeControl, IViewWidgetVolumeControl
    {
        public ModelVolumeControl(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
        }


        #region IModelVolumeControl implementation
        public uint Volume
        {
            get { return iVolume; }
        }

        public uint VolumeLimit
        {
            get { return iVolumeLimit; }
        }

        public bool Mute
        {
            get { return iMute; }
        }

        public void IncrementVolume()
        {
            EventHandler<EventArgs> ev = EventVolumeIncrement;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        public void DecrementVolume()
        {
            EventHandler<EventArgs> ev = EventVolumeDecrement;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        public void ToggleMute()
        {
            EventHandler<EventArgsMute> ev = EventMuteChanged;
            if (ev != null)
            {
                ev(this, new EventArgsMute(!iMute));
            }
        }

        // these events are fired in the main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventInitialised;
        public event EventHandler<EventArgs> EventClose;
        public event EventHandler<EventArgs> EventChanged;
        #endregion IModelVolumeControl implementation


        #region IViewWidgetVolumeControl implementation
        private delegate void DOpen();
        public void Open()
        {
            iInvoker.BeginInvoke((DOpen)DoOpen);
        }
        private void DoOpen()
        {
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        private delegate void DInitialised();
        public void Initialised()
        {
            iInvoker.BeginInvoke((DInitialised)DoInitialised);
        }
        private void DoInitialised()
        {
            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private delegate void DSetVolume(uint aVolume);
        public void SetVolume(uint aVolume)
        {
            iInvoker.BeginInvoke((DSetVolume)DoSetVolume, aVolume);
        }
        private void DoSetVolume(uint aVolume)
        {
            iVolume = aVolume;
            NotifyChange();
        }

        private delegate void DSetMute(bool aMute);
        public void SetMute(bool aMute)
        {
            iInvoker.BeginInvoke((DSetMute)DoSetMute, aMute);
        }
        private void DoSetMute(bool aMute)
        {
            iMute = aMute;
            NotifyChange();
        }

        private delegate void DSetVolumeLimit(uint aVolumeLimit);
        public void SetVolumeLimit(uint aVolumeLimit)
        {
            iInvoker.BeginInvoke((DSetVolumeLimit)DoSetVolumeLimit, aVolumeLimit);
        }
        private void DoSetVolumeLimit(uint aVolumeLimit)
        {
            iVolumeLimit = aVolumeLimit;
            NotifyChange();
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;
        #endregion IViewWidgetVolumeControl implementation


        private void NotifyChange()
        {
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private IInvoker iInvoker;
        private uint iVolume;
        private uint iVolumeLimit;
        private bool iMute;
    }


    // interface for the media time model
    public interface IModelMediaTime
    {
        bool IsInitialised { get; }
        bool AllowSeeking { get; }
        ETransportState TransportState { get; }
        uint Duration { get; }
        uint Seconds { get; }
        void Seek(uint aSeconds);

        // events triggered from main thread
        event EventHandler<EventArgs> EventOpen;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventClose;
        event EventHandler<EventArgs> EventChanged;
    }

    // Implementation of the media time model
    public class ModelMediaTime : IModelMediaTime, IViewWidgetMediaTime
    {
        public ModelMediaTime(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
        }


        #region IModelMediaTime implementation
        public bool IsInitialised
        {
            get { return iInitialised; }
        }

        public bool AllowSeeking
        {
            get { return iAllowSeeking; }
        }

        public ETransportState TransportState
        {
            get { return iTransportState; }
        }

        public uint Duration
        {
            get { return iDuration; }
        }

        public uint Seconds
        {
            get { return iSeconds; }
        }

        public void Seek(uint aSeconds)
        {
            // send seek request to kinsky layer - let the iSeconds get
            // updated via SetSeconds() kinsky event
            EventHandler<EventArgsSeekSeconds> ev = EventSeekSeconds;
            if (ev != null)
            {
                ev(this, new EventArgsSeekSeconds(aSeconds));
            }
        }

        // events triggered from main thread
        public event EventHandler<EventArgs> EventOpen;
        public event EventHandler<EventArgs> EventInitialised;
        public event EventHandler<EventArgs> EventClose;
        public event EventHandler<EventArgs> EventChanged;
        #endregion IModelMediaTime implementation


        #region IViewWidgetMediaTime implementation
        private delegate void DOpen();
        public void Open()
        {
            iInvoker.BeginInvoke((DOpen)DoOpen);
        }
        private void DoOpen()
        {
            if (EventOpen != null)
            {
                EventOpen(this, EventArgs.Empty);
            }
        }

        private delegate void DClose();
        public void Close()
        {
            iInvoker.BeginInvoke((DClose)DoClose);
        }
        private void DoClose()
        {
            iInitialised = false;
            iAllowSeeking = false;
            iTransportState = ETransportState.eStopped;
            iDuration = 0;
            iSeconds = 0;

            if (EventClose != null)
            {
                EventClose(this, EventArgs.Empty);
            }
        }

        private delegate void DInitialised();
        public void Initialised()
        {
            iInvoker.BeginInvoke((DInitialised)DoInitialised);
        }
        private void DoInitialised()
        {
            iInitialised = true;

            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private delegate void DSetAllowSeeking(bool aAllowSeeking);
        public void SetAllowSeeking(bool aAllowSeeking)
        {
            iInvoker.BeginInvoke((DSetAllowSeeking)DoSetAllowSeeking, aAllowSeeking);
        }
        private void DoSetAllowSeeking(bool aAllowSeeking)
        {
            iAllowSeeking = aAllowSeeking;
            NotifyChange();
        }

        private delegate void DSetTransportState(ETransportState aTransportState);
        public void SetTransportState(ETransportState aTransportState)
        {
            iInvoker.BeginInvoke((DSetTransportState)DoSetTransportState, aTransportState);
        }
        private void DoSetTransportState(ETransportState aTransportState)
        {
            iTransportState = aTransportState;
            NotifyChange();
        }

        private delegate void DSetDuration(uint aDuration);
        public void SetDuration(uint aDuration)
        {
            iInvoker.BeginInvoke((DSetDuration)DoSetDuration, aDuration);
        }
        private void DoSetDuration(uint aDuration)
        {
            iDuration = aDuration;

            NotifyChange();
        }

        private delegate void DSetSeconds(uint aSeconds);
        public void SetSeconds(uint aSeconds)
        {
            iInvoker.BeginInvoke((DSetSeconds)DoSetSeconds, aSeconds);
        }
        private void DoSetSeconds(uint aSeconds)
        {
            iSeconds = aSeconds;
            NotifyChange();
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;
        #endregion IViewWidgetMediaTime implementation


        private void NotifyChange()
        {
            if (EventChanged != null)
            {
                EventChanged(this, EventArgs.Empty);
            }
        }

        private IInvoker iInvoker;

        private bool iInitialised;
        private bool iAllowSeeking;
        private ETransportState iTransportState;
        private uint iDuration;
        private uint iSeconds;
    }
}




