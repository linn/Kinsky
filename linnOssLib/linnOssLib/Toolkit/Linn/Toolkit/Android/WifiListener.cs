using Linn;
using Android.Content;
using Android.Runtime;
namespace OssToolkitDroid
{

    [BroadcastReceiver]
    public class WifiListener : BroadcastReceiver
    {

        private IHelper iHelper;
        private Scheduler iScheduler;
        private Context iContext;

        // default constructor to satisfy framework requirements, should not be used
        public WifiListener()
            : base()
        {
            Assert.Check(false);
        }

        public WifiListener(Context aContext, IHelper aHelper)
            : base()
        {
            iHelper = aHelper;
            iScheduler = new Scheduler("WifiStateChangeScheduler", 1);
            iContext = aContext;
            iLockObject = new object();
            iContext.RegisterReceiver(this, new IntentFilter(Android.Net.Wifi.WifiManager.NetworkStateChangedAction));
        }

        protected override void Dispose(bool disposing)
        {
            UserLog.WriteLine("WifiListener::Dispose");
            iScheduler.Stop();
            iContext.UnregisterReceiver(this);
            iContext = null;
            base.Dispose(disposing);
        }

        public override void OnReceive(Context aContext, Intent aIntent)
        {
            Refresh(aContext);
            aIntent.Dispose();
        }

        public void Refresh(Context aContext)
        {
            lock (iLockObject)
            {
                if (iScheduler != null)
                {
                    iScheduler.Schedule(() =>
                    {
                        NetworkInfo.RefreshWifiInfo(aContext);
                        if (iHelper != null)
                        {
                            iHelper.Interface.NetworkChanged();
                        }
                    });
                }
            }
        }

        private object iLockObject;
    }
}