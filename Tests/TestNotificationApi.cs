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
        private static TimeSpan kTimeoutMilliseconds = TimeSpan.FromMilliseconds(50000);

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

            iView.ShowCallback = (notification) =>
            {
                if (!aShowAgainLater)
                {
                    notification.DontShowAgain();
                }
                waitHandle.Set();
            };
            var controllerCanShow = false;

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
                controllerCanShow = controller.CanShow;
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.That(controllerCanShow);
            Assert.That(iView.IsShowingBadge);
            Assert.AreEqual(iPersistence.LastNotificationVersion, aExpectedPersistedId); 
            Assert.IsNotNull(iView.Current);
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

            iServer.CheckCallback = (response) =>
            {
                iInvoker.BeginInvoke(new Action(() =>
                {
                    waitHandle.Set();
                }));
            };
            iView.ShowCallback = (notification) =>
            {
                Assert.Fail("View show should not be called");
            };
            var controllerCanShow = false;

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
                controllerCanShow = controller.CanShow;
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.That(controllerCanShow);
            Assert.AreEqual(true, iView.IsShowingBadge);
            Assert.AreEqual(iPersistence.LastNotificationVersion, iServerResponseV1.Version);
            Assert.IsNull(iView.Current);
        }

        [Test]
        public void TestNotificationIsShownIfNewNotificationAvailable()
        {
            // arrange
            iPersistence.LastNotificationVersion = 1;
            iServer.SetDesiredResponse(iServerResponseV2);
            var waitHandle = new AutoResetEvent(false);

            iView.ShowCallback = (notification) =>
            {
                notification.DontShowAgain();
                waitHandle.Set();
            };
            var controllerCanShow = false;

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
                controllerCanShow = controller.CanShow;
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.That(controllerCanShow);
            Assert.AreEqual(true, iView.IsShowingBadge);
            Assert.AreEqual(iPersistence.LastNotificationVersion, iServerResponseV2.Version);
            Assert.IsNotNull(iView.Current);
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
            iView.ShowCallback = (notification) =>
            {
                Assert.Fail("View show should not be called");
            };
            var controllerCanShow = false;

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
                controllerCanShow = controller.CanShow;
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsFalse(controllerCanShow);
            Assert.IsFalse(iView.IsShowingBadge);
            Assert.AreEqual(iPersistence.LastNotificationVersion, 0);
            Assert.IsNull(iView.Current);
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
            iView.ShowCallback = (notification) =>
            {
                Assert.Fail("View show should not be called");
            };
            var controllerCanShow = false;

            // act
            using (var controller = new NotificationController(iInvoker, iPersistence, iServer, iView))
            {
                Assert.That(waitHandle.WaitOne(kTimeoutMilliseconds));
                controllerCanShow = controller.CanShow;
            }

            // flush pending invocations
            AwaitInvoker();

            // assert
            Assert.IsFalse(controllerCanShow);
            Assert.IsFalse(iView.IsShowingBadge);
            Assert.AreEqual(iPersistence.LastNotificationVersion, 0);
            Assert.IsNull(iView.Current);
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

        public void Show(INotification aNotification)
        {
            Assert.IsFalse(iInvoker.InvokeRequired);
            Current = aNotification;
            var del = ShowCallback;
            if (del != null)
            {
                del.Invoke(aNotification);
            }
        }

        public void ShowBadge()
        {
            Assert.IsFalse(iInvoker.InvokeRequired);
            IsShowingBadge = true;
        }

        public INotification Current { get; set; }

        public bool IsShowingBadge { get; set; }

        public Action<INotification> ShowCallback { get; set; }
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
