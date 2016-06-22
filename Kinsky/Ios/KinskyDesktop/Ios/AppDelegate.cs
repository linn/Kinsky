using System;
using System.Net;

using Foundation;
using UIKit;

using Linn;
using Linn.Kinsky;

namespace KinskyTouch
{
    public abstract class AppDelegate : UIApplicationDelegate, IStack
    { 
        private static readonly string kApiKey = "129c76d1b4043e568d19a9fea8a1f5534cdae703";

		internal class QueryOptions
        {
            public QueryOptions()
            {
            }

            public QueryOptions(string aQuery)
            {
                if(!string.IsNullOrEmpty(aQuery))
                {
                    string[] split1 = aQuery.Split('&');
                    foreach(string s in split1)
                    {
                        string[] split2 = s.Split(new char[] { '=' }, 2);
                        if(split2.Length == 2)
                        {
                            string key = Uri.UnescapeDataString(split2[0]);
                            if(key == "room")
                            {
                                Room = Uri.UnescapeDataString(split2[1]);
                            }
                            else if(key == "caller")
                            {
                                Caller = Uri.UnescapeDataString(split2[1]);
                            }
                            else if(key == "callerUri")
                            {
                                CallerUri = Uri.UnescapeDataString(split2[1]);
                            }
                        }
                    }
                }
            }

            public string Room
            {
                get;
                private set;
            }

            public string Caller
            {
                get;
                private set;
            }

            public string CallerUri
            {
                get;
                private set;
            }
        }

		public AppDelegate() : base()
		{
#if DEBUG
            Xamarin.Insights.Initialize(Xamarin.Insights.DebugModeKey);
#else
        Xamarin.Insights.Initialize(kApiKey);
#endif
			iScheduler = new Scheduler("StackScheduler", 1);
            iQueryOptions = new QueryOptions();
		}

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            UserLog.WriteLine("Kinsky called with " + url);

            iQueryOptions = new QueryOptions(url.Query);

            return true;
        }
		
        public override void WillTerminate(UIApplication application)
        {
            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.WillTerminate");
			iScheduler.Schedule(()=>
			{
                if(iStackStarted)
                {
                    Helper.Helper.Stack.Stop();
                    Helper.Helper.Dispose();
                    iStackStarted = false;
                }
                Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.Terminated");
            });
        }

        public override void OnActivated(UIApplication application)
        {
            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.OnActivated");
			iNotificationView = new NotificationView(Helper.Helper, Helper.Helper.Product, Helper.Helper.Invoker, ViewController);

            iScheduler.Schedule(()=>
			{
                if(!iStackStarted && iLoaded)
                {
                    StartStack();
                }
            });
        }

        public override void OnResignActivation(UIApplication application)
        {
            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.OnResignActivation");

            iScheduler.Schedule(()=>
			{
				if (iStackStarted)
				{
                    Helper.Helper.Stack.Stop();
                    iStackStarted = false;
				}
            });
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.DidEnterBackground");

            nint task = 0;
            task = application.BeginBackgroundTask(delegate {
                if(task != 0)
                {
                    application.EndBackgroundTask(task);
                    task = 0;
                }
            });

				
            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.DidEnterBackground 1");
				
			iScheduler.Schedule(()=>
			    {
                    Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.DidEnterBackground 2");
				    if (iStackStarted)
				    {
                        Helper.Helper.Stack.Stop();
                        iStackStarted = false;
				    }
					Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.DidEnterBackground 3");

                    application.BeginInvokeOnMainThread(delegate {
                        Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.DidEnterBackground 4");
                        if(task != 0)
                        {
                            application.EndBackgroundTask(task);
                            task = 0;
                        }
                    });
                });                

            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.DidEnterBackground 5");
        }

        public override void ReceiveMemoryWarning(UIApplication aApplication)
        {
            ArtworkCacheInstance.Instance.Flush();
        }

        public void Start(IPAddress aIpAddress)
        {
            iMediator.SelectRoom(iQueryOptions.Room);
            // clear previous query options so the next time we are launched if we weren't launched by a query we don't use old query options
            iQueryOptions = new QueryOptions();

            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.Start");
            iMediator.Open();
            iLibrary.Start(aIpAddress);
            iSharedPlaylists.Start(aIpAddress);
            iHttpClient.Start();
            iHttpServer.Start(aIpAddress);
            iLocator.Start();
        }

        public void Stop()
        {
            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            Trace.WriteLine(Trace.kKinskyTouch, t.ToString());
            Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.Stop");
            iLocator.Stop();
            iHttpServer.Stop();
            iHttpClient.Stop();
            iSharedPlaylists.Stop();
            iLibrary.Stop();
            iMediator.Close();
        }

        protected abstract HelperKinskyTouch Helper
        {
            get;
        }

        protected abstract UIViewController ViewController
        {
            get;
        }

        protected void OnFinishedLaunching()
        {
			iScheduler.Schedule(()=>
			{
                if(!iStackStarted)
                {
                    StartStack();
                }
				iLoaded = true;
            });
        }

        protected void SavePlaylistHandler(ISaveSupport aSaveSupport)
        {
            iSaver = new SaveViewController.Saver(aSaveSupport);
            UINavigationController controller = new UINavigationController(new SaveViewController(iSaver, aSaveSupport, "SaveDialog", NSBundle.MainBundle));
            controller.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;

            ViewController.PresentViewController(controller, true, null);
        }

        protected void StatusChanged(object sender, EventArgsStackStatus e)
        {
            switch(e.Status.State)
            {
            case EStackState.eBadInterface:
                Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.StatusChanged: eBadInterface");
                Helper.Helper.Interface.NetworkChanged();
                break;

            case EStackState.eNoInterface:
                Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.StatusChanged: eNoInterface");
                break;

            case EStackState.eNonexistentInterface:
                Trace.WriteLine(Trace.kKinskyTouch, "AppDelegate.StatusChanged: eNonexistentInterface");
                if(Helper.Helper.Interface.Allowed.Count == 2)
                {
                    Helper.Helper.Interface.Set(Helper.Helper.Interface.Allowed[1]);
                }
                break;

            default:
                break;
            }
        }

        protected void OnReachabilityChanged()
        {
            iScheduler.Schedule(()=>
            {
                Helper.Helper.Interface.NetworkChanged();
            });
        }

        private void StartStack()
        {
            Helper.Helper.Stack.Start();
            iStackStarted = true;
    
            // setup timer for rescan 2s later
            new System.Threading.Timer(Rescan, null, 2000, System.Threading.Timeout.Infinite);
        }

        private void Rescan(object aObject)
        {
            iScheduler.Schedule(()=>
			{
                if(iStackStarted)
                {
                    Helper.Helper.Rescan();
					iSharedPlaylists.Refresh();
					iLibrary.Refresh();
                }
            });
        }

        protected HttpClient iHttpClient;
        protected HttpServer iHttpServer;

        protected ContentDirectoryLocator iLocator;
        protected MediaProviderLibrary iLibrary;
        protected SharedPlaylists iSharedPlaylists;
     
        protected Mediator iMediator;

        private QueryOptions iQueryOptions;

        private bool iStackStarted;
        private bool iLoaded;

        private SaveViewController.Saver iSaver;
		private Scheduler iScheduler;
		protected NotificationView iNotificationView;
    }
}
