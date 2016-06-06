using Linn;
using Android.OS;
using Java.Lang;
using Android.App;
using Android.Content;
using System;

namespace OssToolkitDroid
{

    public class Invoker : IInvoker
    {
        public Invoker(Context aContext)
        {
            iContext = aContext;
            iHandler = new Handler(iContext.MainLooper);
            iMainThreadId = iContext.MainLooper.Thread.Id;
        }

        #region IInvoker Members

        public bool InvokeRequired
        {
            get
            {
                return iMainThreadId != Thread.CurrentThread().Id;
            }
        }

        public void BeginInvoke(System.Delegate aDelegate, params object[] aArgs)
        {
            iHandler.Post((Action)(() =>
            {

                try
                {
#if DEBUG || TRACE
                    Linn.Trace.WriteLine(Linn.Trace.kGui, string.Format("{0} INVOKING {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                    aDelegate.DynamicInvoke(aArgs);
#if DEBUG || TRACE
                    Linn.Trace.WriteLine(Linn.Trace.kGui, string.Format("{0} INVOKED {1}", DateTime.Now.ToString(), this.GetCallInfo(aDelegate, aArgs)));
#endif
                }
                catch (System.Exception ex)
                {
                    UserLog.WriteLine("Exception: " + ex);
                    UserLog.WriteLine("Invocation details: " + this.GetCallInfo(aDelegate, aArgs));
                    ThrowException(ex);
                }
            }));
        }

        public bool TryBeginInvoke(System.Delegate aDelegate, params object[] aArgs)
        {
            if (InvokeRequired)
            {
                BeginInvoke(aDelegate, aArgs);
                return true;
            }
            return false;
        }

        public void ThrowException(System.Exception ex)
        {
            // force exception throw off UI thread
            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                throw new System.Exception("Unhandled exception in main thread", ex);
            })).Start();
        }

        #endregion
        private Context iContext;
        private Handler iHandler;
        private long iMainThreadId;
    }

}