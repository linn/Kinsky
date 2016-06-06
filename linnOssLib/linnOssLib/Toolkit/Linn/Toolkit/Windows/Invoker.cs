
using System;
using System.Windows.Threading;


namespace Linn.Toolkit.Wpf
{
    public class Invoker : IInvoker
    {
        public Invoker(Dispatcher aDispatcher)
        {
            iDispatcher = aDispatcher;
        }

        public bool InvokeRequired
        {
            get { return !iDispatcher.CheckAccess(); }
        }
        
        public void BeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
                try
                {
#if DEBUG || TRACE
                    Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKING {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                    iDispatcher.BeginInvoke(aDelegate, aArgs);
#if DEBUG || TRACE
                    Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKED {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                }
                catch (System.Exception ex)
                {
                    UserLog.WriteLine("Exception: " + ex);
                    UserLog.WriteLine("Invocation details: " + this.GetCallInfo(aDelegate, aArgs));
                    throw ex;
                }
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

        private Dispatcher iDispatcher;
    }
}

