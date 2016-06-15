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
        [Test]
        public void ServerResponseResultsInNotificationShown()
        {
            // arrange
            var waitHandle = new AutoResetEvent(false);
            var invoker = CreateInvoker();
            var persistence = new MockPersistence() { LastNotificationVersion = 0 };
            var server = new MockNotificationServer();
            var desiredResponse = new NotificationServerResponse() { Uri = "http://notifications/1", Version = 1 };
            server.SetDesiredResponse(desiredResponse);
            var view = new MockNotificationView(invoker);

            // act
            Task.Factory.StartNew(() =>
            {
                using (var controller = new NotificationController(invoker, persistence, server, view))
                {
                    
                }

                // flush out any pending invoker actions
                invoker.BeginInvoke(new Action(() =>
                {
                    waitHandle.Set();
                }));
            });
            waitHandle.WaitOne();

            // assert
            Assert.AreEqual(true, view.IsShowingBadge);
            Assert.AreEqual(persistence.LastNotificationVersion, desiredResponse.Version);
            Assert.IsNotNull(view.Current);
            Assert.AreEqual(desiredResponse.Uri, view.Current.Uri);
            Assert.AreEqual(desiredResponse.Version, view.Current.Version);
        }

        private IInvoker CreateInvoker()
        {
            return new Invoker(Dispatcher.FromThread(Thread.CurrentThread)); // fixme: tests require wpf
        }
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
        }

        public void ShowBadge()
        {
            Assert.IsFalse(iInvoker.InvokeRequired);
            IsShowingBadge = true;
        }

        public INotification Current { get; set; }

        public bool IsShowingBadge { get; set; }
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
                aCancelToken.ThrowIfCancellationRequested();
                if (iResponse != null)
                {
                    return iResponse;
                }
                else
                {
                    throw new HttpServerException("Exception");
                }
            });
        }
    }
}
