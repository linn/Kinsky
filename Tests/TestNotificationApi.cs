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
            iServerResponseV1 = new NotificationServerResponse() { Uri = "http://notifications/1", Version = 1 };
            iServerResponseV2 = new NotificationServerResponse() { Uri = "http://notifications/2", Version = 2 };
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
        [TestCase(true, (uint)0)] // show again later should not persist notification id
        [TestCase(false, (uint)1)] // dismiss should persist notification id
        public void TestNotificationIdPersistence(bool aShowAgainLater, uint aExpectedPersistedId)
        {
            // arrange
            iServer.SetDesiredResponse(iServerResponseV1);
            var waitHandle = new AutoResetEvent(false);

            iView.ShowCallback = (notification, shownow) =>
            {
                if (!aShowAgainLater)
                {
                    notification.DontShowAgain();
                }
                waitHandle.Set();
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            Assert.IsNotNull(iView.LastShown);
            Assert.AreEqual(iView.Current, iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, aExpectedPersistedId); 
            Assert.AreEqual(iServerResponseV1.Uri, iView.Current.Uri);
            Assert.AreEqual(iServerResponseV1.Version, iView.Current.Version);
        }

        [Test]
        public void TestNotificationIsNotShownIfAlreadySeen()
        {
            // arrange
            iPersistence.LastNotificationVersion = 1;
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
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            Assert.IsNull(iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, iServerResponseV1.Version);
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
                notification.DontShowAgain();
                waitHandle.Set();
            };

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsNotNull(iView.Current);
            Assert.IsNotNull(iView.LastShown);
            Assert.AreEqual(iView.Current, iView.LastShown);
            Assert.AreEqual(iPersistence.LastNotificationVersion, iServerResponseV2.Version);
            Assert.AreEqual(iServerResponseV2.Uri, iView.Current.Uri);
            Assert.AreEqual(iServerResponseV2.Version, iView.Current.Version);
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
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
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
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
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
