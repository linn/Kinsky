using Linn;
using Linn.Kinsky;
using Linn.Toolkit.Wpf;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Assert = NUnit.Framework.Assert;

namespace Tests
{
    [TestFixture]
    public class TestNotificationApi
    {
        private static TimeSpan kTimeoutMilliseconds = TimeSpan.FromMilliseconds(5000);

        private IInvoker iInvoker;
        private MockPersistence iPersistence;
        private MockNotificationServer iServer;
        private NotificationVersion iNotificationVersion1;
        private NotificationVersion iNotificationVersion2;
        private NotificationServerResponse iServerResponseV1;
        private NotificationServerResponse iServerResponseV2;
        private AutoResetEvent iWaitHandleInvoker;
        private MockNotificationView iView;

        [SetUp]
        public void Setup()
        {
            iInvoker = new MockInvoker();
            iWaitHandleInvoker = new AutoResetEvent(false);
            iPersistence = new MockPersistence { LastNotificationVersion = 0 };
            iServer = new MockNotificationServer();
            iNotificationVersion1 = new NotificationVersion() { Uri = "http://notifications/1", Version = 1 };
            iNotificationVersion2 = new NotificationVersion() { Uri = "http://notifications/2", Version = 2 };
            iServerResponseV1 = new NotificationServerResponse() { Notifications = new NotificationVersion[] { iNotificationVersion1 } };
            iServerResponseV2 = new NotificationServerResponse() { Notifications = new NotificationVersion[] {  iNotificationVersion1, iNotificationVersion2}  };
            iView = new MockNotificationView(iInvoker);
        }

        [TearDown]
        public void TearDown()
        {
        }

        // flush out any pending invoker actions
        private void AwaitInvoker()
        {            
            iInvoker.BeginInvoke(new Action(() =>
            {
                iWaitHandleInvoker.Set();
            }));
            Assert.That(iWaitHandleInvoker.WaitOne(kTimeoutMilliseconds));
        }

        [Test]
        public void TestNotificationIdPersistence()
        {
            // arrange
            iServer.SetDesiredResponse(iServerResponseV1);
            var waitHandle = new AutoResetEvent(false);

            iView.ShowCallback = (notification, shownow) =>
            {
                notification.Closed(false);
                waitHandle.Set();
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView, NotificationController.DefaultTimespan))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            Assert.IsNotNull(iView.LastShown);
            Assert.AreEqual(iView.Current, iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, 1); 
            Assert.AreEqual(iNotificationVersion1.Uri, iView.Current.Uri(false));
            Assert.AreEqual(iNotificationVersion1.Version, iView.Current.Version);
        }

        [Test]
        public void TestNotificationIsNotShownIfAlreadySeen()
        {
            // arrange
            iPersistence.LastNotificationVersion = 1;
            iPersistence.LastShownNotification = DateTime.Now;
            iServer.SetDesiredResponse(iServerResponseV1);
            var waitHandle = new AutoResetEvent(false);
           
            iView.ShowCallback = (notification, shownow) =>
            {
                if (shownow)
                {
                    Assert.Fail("View should not be shown.");
                }
                waitHandle.Set();
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView, NotificationController.DefaultTimespan))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            Assert.IsNull(iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, iNotificationVersion1.Version);
        }


        [Test]
        [TestCase(-2, true)]
        [TestCase(-1, true)]
        [TestCase(0, false)]
        [TestCase(1, false)]
        public void TestNotificationReshowTimespan(int aDaysElapsed, bool aShouldShow)
        {
            // arrange
            iPersistence.LastNotificationVersion = 1;
            iPersistence.LastShownNotification = DateTime.Now.Add(TimeSpan.FromDays(aDaysElapsed)).AddMinutes(-5); // take 5 mins off to ensure days comparison works properly
            iServer.SetDesiredResponse(iServerResponseV1);
            var waitHandle = new AutoResetEvent(false);

            iView.ShowCallback = (notification, shownow) =>
            {
                if (shownow && !aShouldShow)
                {
                    Assert.Fail("View should not be shown.");
                }
                waitHandle.Set();
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView, TimeSpan.FromDays(1)))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            if (aShouldShow)
            {
                Assert.IsNotNull(iView.LastShown);
            }else
            {
                Assert.IsNull(iView.LastShown);
            }
            Assert.AreEqual(iPersistence.LastNotificationVersion, iNotificationVersion1.Version);
        }

        [Test]
        public void TestNotificationIsShownIfNewNotificationAvailable()
        {
            // arrange
            iPersistence.LastNotificationVersion = 1;
            iServer.SetDesiredResponse(iServerResponseV2);
            var waitHandle = new AutoResetEvent(false);

            iView.ShowCallback = (notification, shownow) =>
            {
                notification.Closed(false);
                waitHandle.Set();
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView, NotificationController.DefaultTimespan))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            Assert.IsNotNull(iView.LastShown);
            Assert.AreEqual(iView.Current, iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, iNotificationVersion2.Version);
            Assert.AreEqual(iNotificationVersion2.Uri, iView.Current.Uri(false));
            Assert.AreEqual(iNotificationVersion2.Version, iView.Current.Version);
        }


        [Test]
        public void TestServerReadExceptionResultsInNoViewBeingShown()
        {
            // arrange
            iServer.SetDesiredResponse(null); // simulates a faulted task in server.check
            var waitHandle = new AutoResetEvent(false);

            iServer.CheckCallback = (response) =>
            {
                iInvoker.BeginInvoke(new Action(() =>
                {
                    waitHandle.Set();
                }));
            };
            iView.ShowCallback = (notification, shownow) =>
            {
                Assert.Fail("View show should not be called");
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView, NotificationController.DefaultTimespan))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNull(iView.Current);
            Assert.IsNull(iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, 0);
        }


        [Test]
        public void TestServerTaskCancelledResultsInNoViewBeingShown()
        {
            // arrange
            iServer.ForceCancelTask = true; // simulate a cancellation
            var waitHandle = new AutoResetEvent(false);

            iServer.CheckCallback = (response) =>
            {
                iInvoker.BeginInvoke(new Action(() =>
                {
                    waitHandle.Set();
                }));
            };
            iView.ShowCallback = (notification, shownow) =>
            {
                Assert.Fail("View show should not be called");
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView, NotificationController.DefaultTimespan))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNull(iView.Current);
            Assert.IsNull(iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, 0);
        }
    }

    class MockInvoker : IInvoker
    {
        public MockInvoker()
        {
            iScheduler = new Scheduler("Invoker", 1);
            iScheduler.SchedulerError += (s, e) =>
            {
                throw new Exception("Invoker Error", e.Error);
            };
        }

        public bool InvokeRequired
        {
            get { return !System.Threading.Thread.CurrentThread.Name.StartsWith("Invoker"); }
        }

        public void BeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKING {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
            iScheduler.Schedule(new Scheduler.DCallback(() => { aDelegate.DynamicInvoke(aArgs); }));
            Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKED {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
        }

        public bool TryBeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(aDelegate, aArgs);
                return true;
            }
            return false;
        }

        private Scheduler iScheduler;
    }

    class MockNotificationView : INotificationView
    {
        private readonly IInvoker iInvoker;
        public MockNotificationView(IInvoker aInvoker)
        {
            iInvoker = aInvoker;
        }

        public void Update(INotification aNotification, bool aShowNow)
        {
            Assert.IsFalse(iInvoker.InvokeRequired);
            Current = aNotification;
            if (aShowNow)
            {
                LastShown = aNotification;
            }
            var del = ShowCallback;
            if (del != null)
            {
                del.Invoke(aNotification, aShowNow);
            }
        }

        public INotification LastShown { get; set; }

        public INotification Current { get; set; }

        public Action<INotification, bool> ShowCallback { get; set; }
    }

    class MockPersistence : INotificationPersistence
    {
        public uint LastNotificationVersion { get; set; }
        public uint LastAcknowledgedNotificationVersion { get; set; }
        public DateTime LastShownNotification { get; set; }
    }


    class MockNotificationServer : INotificationServer
    {
        private NotificationServerResponse iResponse;
        public MockNotificationServer()
        {
        }

        public void SetDesiredResponse(NotificationServerResponse aResponse)
        {
            iResponse = aResponse;
        }

        public Task<NotificationServerResponse> Check(uint aCurrentVersion, CancellationToken aCancelToken)
        {
            return Task.Factory.StartNew<NotificationServerResponse>(() =>
            {
                try
                {
                    if (ForceCancelTask)
                    {
                        throw new TaskCanceledException();
                    }
                    aCancelToken.ThrowIfCancellationRequested();
                    if (iResponse != null)
                    {
                        return iResponse;
                    }
                    else
                    {
                        throw new HttpServerException("Exception");  // simulate an error in http comms causing faulted task if no response is provided
                    }
                }
                finally
                {
                    var del = CheckCallback;
                    if (del != null)
                    {
                        del.Invoke(iResponse);
                    }
                }
            });
        }

        public bool ForceCancelTask { get;  set; }


        public Action<NotificationServerResponse> CheckCallback { get; set; }
    }
}
