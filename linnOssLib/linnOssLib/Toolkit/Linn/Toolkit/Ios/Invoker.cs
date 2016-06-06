using System;

using UIKit;
using Foundation;
using Linn;

namespace Linn.Toolkit.Ios
{
    public class Invoker : IInvoker
    {
        public void BeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            UIApplication.SharedApplication.BeginInvokeOnMainThread(delegate {
                try
                {
#if DEBUG || TRACE
                    Trace.WriteLine(Trace.kGui, string.Format("{0} INVOKING {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                    aDelegate.DynamicInvoke(aArgs);
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
            });
        }
    
        public bool TryBeginInvoke(Delegate aDelegate, params object[] aArgs)
        {
            if(InvokeRequired)
            {
                BeginInvoke(aDelegate, aArgs);
                return true;
            }
    
            return false;
        }
    
        public bool InvokeRequired
        {
            get
            {
                return !NSThread.Current.IsMainThread;
            }
        }
    }
}