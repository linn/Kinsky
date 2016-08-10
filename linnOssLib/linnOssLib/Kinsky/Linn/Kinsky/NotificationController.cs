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
        string Uri(bool aAppendCacheBuster);
        bool HasBeenAcknowledged { get; }
        void Shown();
        void Closed(bool aAcknowledged);
        void TrackUsageEventDismissed(bool aVisitedStorePage);
    }

    public interface INotificationServer
    {
        Task<NotificationServerResponse> Check(uint aCurrentVersion, CancellationToken aCancelToken);
    }

    public interface INotificationPersistence
    {
        uint LastNotificationVersion { get; set; }
        uint LastAcknowledgedNotificationVersion { get; set; }
        DateTime LastShownNotification { get; set; }
    }

    internal class Notification : INotification
    {
        private Action<bool> iClosed;
        private Action iShown;
        private readonly string iUri;

        public Notification(uint aVersion, string aUri, bool aHasBeenAcknowledged, Action aShown, Action<bool> aClosed)
        {
            Version = aVersion;
            iUri = aUri;
            iClosed = aClosed;
            iShown = aShown;
            HasBeenAcknowledged = aHasBeenAcknowledged;
        }

        public string Uri(bool aAppendCacheBuster)
        {
             return aAppendCacheBuster ? CacheBuster(iUri) : iUri;
        }
        public uint Version { get; private set; }

        public void Shown()
        {
            iShown();
        }

        public void Closed(bool aAcknowledged)
        {
            iClosed(aAcknowledged);
        }

        public void TrackUsageEventDismissed(bool aVisitedStorePage)
        {
            if (Insights.IsInitialized)
            {
                Insights.Track(string.Format("NotificationDismissedV{0}", Version), new Dictionary<string, string>() { { "VisitedStore", aVisitedStorePage.ToString() } });
            }
        }
        private string CacheBuster(string aUri)
        {
            var querySeparator = aUri.IndexOf("?") == -1 ? "?" : ":";
            return string.Format("{0}{1}dontcache={2}", aUri, querySeparator, Guid.NewGuid());
        }

        public bool HasBeenAcknowledged { get; internal set; }
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
            return string.Format("https://cloud.linn.co.uk/applications/{0}/notifications/feed.json", aProduct.ToLowerInvariant());
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

        private readonly TimeSpan iShowAgainTimespan;
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

        public static TimeSpan DefaultTimespan
        {
            get
            {
                return TimeSpan.FromDays(28); // default timespan shows ad again in a month's time
            }
        }

        public NotificationController(IInvoker aInvoker, INotificationPersistence aNotificationPersistence, INotificationServer aNotificationServer, INotificationView aView, TimeSpan aShowAgainTimespan)
        {
            iShowAgainTimespan = aShowAgainTimespan;
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
                                    Notification notification = null;
                                    notification = new Notification(current.Version, current.Uri, current.Version <= iNotificationPersistence.LastAcknowledgedNotificationVersion, ()=> 
                                    {
                                        lock (iLock)
                                        {
                                            iNotificationPersistence.LastShownNotification = DateTime.Now;
                                        }
                                    },
                                    (acknowledged) =>
                                    {
                                        if (iNotificationPersistence.LastNotificationVersion < current.Version)
                                        {
                                            iNotificationPersistence.LastNotificationVersion = current.Version;
                                        }
                                        if (acknowledged)
                                        {
                                            if (iNotificationPersistence.LastAcknowledgedNotificationVersion < current.Version)
                                            {
                                                iNotificationPersistence.LastAcknowledgedNotificationVersion = current.Version;
                                            }
                                            // event out the changed acknowledgment status
                                            notification.HasBeenAcknowledged = true;
                                            iView.Update(notification, false); 
                                        }
                                    });
                                    iView.Update(notification, current.Version > previousVersion || (TimespanElapsed() && !notification.HasBeenAcknowledged));
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

        private bool TimespanElapsed()
        {
            lock (iLock)
            {
                return iNotificationPersistence.LastShownNotification.Add(iShowAgainTimespan).CompareTo(DateTime.Now) < 0;
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
