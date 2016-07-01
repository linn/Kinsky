using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Linn
{
    public class SafeTimer : IDisposable
    {
        private readonly object iLock;
        private readonly System.Threading.Timer iTimer;

        private bool iDisposed;

        public SafeTimer(Action<SafeTimer> aExpired)
        {
            iLock = new object();
            iTimer = new System.Threading.Timer((state) => {
                if (!iDisposed)
                {
                    aExpired(this);
                }
            });
            iDisposed = false;
        }

        public SafeTimer(Action aExpired)
            : this((t) => aExpired())
        {
        }

        public void FireIn(uint aMilliseconds)
        {
            lock (iLock)
            {
                if (!iDisposed)
                {
                    iTimer.Change((int)aMilliseconds, Timeout.Infinite);
                }
            }
        }

        public void Cancel()
        {
            lock (iLock)
            {
                if (!iDisposed)
                {
                    iTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        // IDisposable

        public void Dispose()
        {
            lock (iLock)
            {
                if (iDisposed)
                {
                    throw new ObjectDisposedException("SafeTimer");
                }
                iTimer.Change(Timeout.Infinite, Timeout.Infinite);
                iDisposed = true;
            }

            using (var wait = new ManualResetEvent(false))
            {
                iTimer.Dispose(wait);
                wait.WaitOne();
            }
        }
    }

}
