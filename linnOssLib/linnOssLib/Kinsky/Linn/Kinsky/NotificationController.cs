using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin;

namespace Linn.Kinsky
{

    public interface INotificationView
    {
        void Update(INotification aNotification, bool aShowNow);
    }

    public interface INotification
    {
        uint Version { get; }
        string Uri { get; }
        void Closed(bool aDontShowAgain);

        bool DontShowAgain { get; }

        void TrackUsageEventDismissed(bool aVisitedStorePage, bool aDontShowAgain)
    }

    public interface INotificationServer
    {
        Task<NotificationServerResponse> Check(uint aCurrentVersion, CancellationToken aCancelToken);
    }

    public interface INotificationPersistence
    {
        uint LastNotificationVersion { get; set; }
    }

    internal class Notification : INotification
    {
        private Action<bool> iClosed;
        private Func<bool> iDontShowAgain;
        public Notification(uint aVersion, string aUri, Func<bool> aDontShowAgain, Action<bool> aClosed)
        {
            iDontShowAgain = aDontShowAgain;
            Version = aVersion;
            Uri = aUri;
            iClosed = aClosed;
        }

        public string Uri { get; private set; }
        public uint Version { get; private set; }

        public void Closed(bool aDontShowAgain)
        {
            iClosed(aDontShowAgain);
        }

        public void TrackUsageEventDismissed(bool aVisitedStorePage, bool aDontShowAgain)
        {
            Insights.Track("NotificationDismissed", new Dictionary<string, string>() { { "VisitedStore", aVisitedStorePage.ToString() }, { "Version", Version.ToString() }, { "DontShowAgain", aDontShowAgain.ToString() } });
        }

        public bool DontShowAgain {
            get
            {
                return iDontShowAgain();
            }
        }
    }

    public sealed class NotificationServerResponse
    {
        public NotificationVersion[] Notifications;
    }

    public sealed class NotificationVersion
    {
        public uint Version { get; set; }
        public string Uri { get; set; }
    }

    public class NotificationServerHttp : INotificationServer
    {

        public static string DefaultUri(string aProduct)
        {
            return string.Format("http://eng.linn.co.uk/~iainm/Notifications/feed.json", aProduct); // TODO: live link!
        }

        private readonly string iUri;

        public NotificationServerHttp(string aBaseUri)
        {
            iUri = aBaseUri;
        }

        public Task<NotificationServerResponse> Check(uint aCurrentVersion, CancellationToken aCancelToken)
        {
            Assert.Check(!aCancelToken.IsCancellationRequested);

            TaskCompletionSource<NotificationServerResponse> tcs = new TaskCompletionSource<NotificationServerResponse>();
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            client.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }
                    else if (e.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else
                    {
                        try
                        {
                            tcs.SetResult(JsonConvert.DeserializeObject<NotificationServerResponse>(e.Result));
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                }
                finally
                {
                    client.Dispose();
                }
            };
            client.DownloadStringAsync(new Uri(string.Format("{0}?version={1}", iUri, aCurrentVersion)));

            aCancelToken.Register(() =>
            {
                client.CancelAsync();
            });

            return tcs.Task;
        }
    }

    public class NotificationController : IDisposable
    {
        private const double kTimerInterval = 24 * 60 * 60 * 1000; // daily

        private readonly IInvoker iInvoker;
        private readonly INotificationView iView;
        private readonly object iLock = new object();
        private readonly INotificationPersistence iNotificationPersistence;
        private readonly INotificationServer iNotificationServer;

        private Timer iTimer;
        private CancellationTokenSource iCancelTokenSource;
        private bool iRunning;
        private bool iDisposed;
        private uint iCurrentVersion;

        public NotificationController(IInvoker aInvoker, INotificationPersistence aNotificationPersistence, INotificationServer aNotificationServer, INotificationView aView)
        {
            iNotificationPersistence = aNotificationPersistence;
            iNotificationServer = aNotificationServer;
            iInvoker = aInvoker;
            iView = aView;

            iCurrentVersion = iNotificationPersistence.LastNotificationVersion;

            Start();
        }

        private void Start()
        {
            lock(iLock)
            {
                if (!iRunning)
                {
                    iRunning = true;
                    Assert.Check(iTimer == null);
                    iTimer = new Timer(kTimerInterval);
                    iTimer.AutoReset = true;
                    iTimer.Elapsed += CheckForUpdates;
                    iTimer.Start();
                    // call check for updates immediately
                    CheckForUpdates(null, EventArgs.Empty);
                }
            }
        }

        private void Stop()
        {
            lock(iLock)
            {
                if (iRunning)
                {
                    iRunning = false;
                    Assert.Check(iTimer != null);

                    iTimer.Elapsed -= CheckForUpdates;
                    iTimer.Stop();
                    iTimer.Dispose();
                    iTimer = null;
                    if (iCancelTokenSource != null)
                    {
                        iCancelTokenSource.Cancel();
                        iCancelTokenSource = null;
                    }
                }
            }
        }

        private void CheckForUpdates(object sender, EventArgs e)
        {
            lock (iLock)
            {
                Assert.Check(!iDisposed);
                if (iCancelTokenSource != null)
                {
                    iCancelTokenSource.Cancel();
                }
                iCancelTokenSource = new CancellationTokenSource();

                var cancelToken = iCancelTokenSource.Token;
                var previousVersion = iCurrentVersion;
                iNotificationServer.Check(previousVersion, cancelToken).ContinueWith(t =>
                {
                    iInvoker.BeginInvoke(new Action(() =>
                    {
                        if (!cancelToken.IsCancellationRequested)
                        {
                            if (t.IsFaulted)
                            {
                            t.Exception.Handle((ex =>
                            {
                                return true;
                            }));
                            UserLog.WriteLine("Error checking Upgrade Feed: " + t.Exception.GetBaseException());
                            }
                            else
                            {
                                var current = GetNextNotification(previousVersion, t.Result);
                                if (current != null)
                                {
                                    var notification = new Notification(current.Version, current.Uri, ()=>current.Version <= iNotificationPersistence.LastNotificationVersion, (dontshowagain) =>
                                    {
                                        if (dontshowagain && iNotificationPersistence.LastNotificationVersion < current.Version)
                                        {
                                            iNotificationPersistence.LastNotificationVersion = current.Version;
                                        }
                                        else if(!dontshowagain)
                                        {
                                            iNotificationPersistence.LastNotificationVersion = current.Version - 1;
                                        }
                                    });
                                    iView.Update(notification, current.Version > previousVersion);
                                    lock (iLock)
                                    {
                                        iCurrentVersion = current.Version;
                                    }
                               }                                
                            }
                        }
                     }));
                });
            }
        }

        private NotificationVersion GetNextNotification(uint aCurrentVersion, NotificationServerResponse aResponse)
        {
            if (aResponse.Notifications != null && aResponse.Notifications.Any())
            {
                // if there are unseen notifications, return the first of these
                var result = aResponse.Notifications.Where(n=>n.Version > aCurrentVersion).OrderBy(n => n.Version).FirstOrDefault();
                if (result == null)
                {
                    // else return the most up to date notification
                    result = aResponse.Notifications.OrderBy(n => n.Version).Last();
                }
                return result;
            }
            return null;
        }

        public void Dispose()
        {
            lock (iLock)
            {
                Assert.Check(!iDisposed);
                iDisposed = true;
            }
            Stop();
        }
    }
}
