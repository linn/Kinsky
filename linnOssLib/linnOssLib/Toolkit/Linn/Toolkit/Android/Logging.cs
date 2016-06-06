using Linn;
using System;
using Android.Widget;
namespace OssToolkitDroid
{
    public class AndroidTraceListener : TraceListener
    {
        public override void Write(string aMsg)
        {
            Android.Util.Log.Info("TRACE", aMsg);
        }
        public override void WriteLine(string aMsg)
        {
            Android.Util.Log.Info("TRACE", aMsg);
        }
    }

    public class AndroidUserLogListener : IUserLogListener
    {

        #region IUserLogListener Members

        public void Write(string aMessage)
        {
            Android.Util.Log.Info("USERLOG", string.Format("{0} - {1}", DateTime.Now, aMessage));
        }

        public void WriteLine(string aMessage)
        {
            Android.Util.Log.Info("USERLOG", string.Format("{0} - {1}", DateTime.Now, aMessage));
        }

        #endregion
    }

}