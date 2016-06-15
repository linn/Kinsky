using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Linn.Kinsky
{

    public interface INotificationView
    {
        void Show(INotification aNotification);
        void ShowBadge(); // badge will always be shown while user is running kinsky
    }

    public interface INotification
    {
        uint Version { get; }
        string Uri { get; }
        void Closed(bool aShowAgainLater);
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
        private Action<bool> iClosedCallback;
        private bool iClosed;
        public Notification(uint aVersion, string aUri, Action<bool> aClosedCallback)
        {
            Version = aVersion;
            Uri = aUri;
            iClosedCallback = aClosedCallback;
        }

        public string Uri { get; private set; }
        public uint Version { get; private set; }

        public void Closed(bool aShowAgainLater)
        {
            Assert.Check(!iClosed);
            iClosed = true;
            iClosedCallback(aShowAgainLater);
        }
    }

    public sealed class NotificationServerResponse
    {
        public uint Version { get; set; }
        public string Uri { get; set; }
    }

    public class NotificationServerHttp : INotificationServer
    {

        public static string DefaultUri(string aProduct)
        {
            return string.Format("http://pc820/applications/{0}/Notifications/feed.json", aProduct);
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
            client.DownloadStringAsync(new Uri(string.Format("{0}?version={1}&cachebuster={2}", iUri, aCurrentVersion, Guid.NewGuid())));

            aCancelToken.Register(() =>
            {
                client.CancelAsync();
            });

            return tcs.Task;
        }
    }

    public class NotificationController : IDisposable
    {
        private const double kTimerInterval = 24 * 60 * 60; // daily

        private readonly IInvoker iInvoker;
        private readonly INotificationView iView;
        private readonly object iLock = new object();
        private readonly INotificationPersistence iNotificationPersistence;
        private readonly INotificationServer iNotificationServer;

        private Timer iTimer;
        private CancellationTokenSource iCancelTokenSource;
        private Notification iCurrent;
        private bool iRunning;

        public NotificationController(IInvoker aInvoker, INotificationPersistence aNotificationPersistence, INotificationServer aNotificationServer, INotificationView aView)
        {
            iNotificationPersistence = aNotificationPersistence;
            iNotificationServer = aNotificationServer;
            iInvoker = aInvoker;
            iView = aView;

            iCurrent = null;
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
                if (iCancelTokenSource != null)
                {
                    iCancelTokenSource.Cancel();
                }
                iCancelTokenSource = new CancellationTokenSource();

                var cancelToken = iCancelTokenSource.Token;
                var currentVersion = iCurrent != null ? iCurrent.Version : 0;
                iNotificationServer.Check(currentVersion, cancelToken).ContinueWith(t =>
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
                        }else
                        {
                            var response = t.Result;
                            lock (iLock)
                            {
                                iCurrent = new Notification(response.Version, response.Uri, (showagainlater) =>
                                {
                                    if (!showagainlater)
                                    {
                                        iNotificationPersistence.LastNotificationVersion = response.Version;
                                    }
                                }); ;
                            }

                            iInvoker.BeginInvoke(new Action(() =>
                            {
                                iView.ShowBadge(); // we always show badge after we get a response, badge is a bit odd in that it never goes away - we wish to encourage uptake of Kazoo
                            }));

                            if (response.Version > currentVersion)
                            {
                                Show(); // new version available - show notification to user
                            }
                        }
                    }
                });
            }
        }

        public void Show()
        {
            iInvoker.BeginInvoke(new Action(() =>
            {
                lock(iLock)
                {
                    if (iCurrent != null)
                    {
                        iView.Show(iCurrent);
                    }
                }
            }));
        }
        

        public void Dispose()
        {
            Stop();
        }
    }
}
