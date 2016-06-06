using Linn;
using Android.Content;
using Android.OS;
using System;
namespace OssToolkitDroid
{

    [BroadcastReceiver]
    public class PowerStateListener : BroadcastReceiver
    {
        private bool iIsConnected;
        private Scheduler iScheduler;
        public event EventHandler<EventArgsPowerState> EventPowerStateChanged;
        private Context iContext;
    
        // default constructor to satisfy framework requirements, should not be used
        public PowerStateListener()
            : base()
        {
            Assert.Check(false);
        }

        public PowerStateListener(Context aContext)
            : base()
        {
            iScheduler = new Scheduler("PowerStateChangeScheduler", 1);
            iContext = aContext;
            iIsConnected = IsConnected(aContext);
            iContext.RegisterReceiver(this, new IntentFilter(Intent.ActionBatteryChanged));
        }

        public override void OnReceive(Context aContext, Intent aIntent)
        {
            iScheduler.Schedule(() =>
            {
                bool isConnected = IsConnected(aIntent);
                if (isConnected != iIsConnected)
                {
                    iIsConnected = isConnected;
                    OnEventPowerStateChanged(isConnected);
                }
                aIntent.Dispose();
            });
        }

        public static bool IsConnected(Context aContext)
        {
            Intent intent = aContext.RegisterReceiver(null, new IntentFilter(Intent.ActionBatteryChanged));
            bool result = IsConnected(intent);
            intent.Dispose();
            return result;
        }

        private static bool IsConnected(Intent aIntent)
        {
            //hack: monodroid doesn't seem to expose BatteryManager.ExtraPlugged
            int plugged = aIntent.GetIntExtra("plugged", -1);
            return plugged == (int)Android.OS.BatteryPlugged.Ac || plugged == (int)Android.OS.BatteryPlugged.Usb;
        }

        private void OnEventPowerStateChanged(bool aIsConnected)
        {
            EventHandler<EventArgsPowerState> del = EventPowerStateChanged;
            if (del != null)
            {
                del(this, new EventArgsPowerState(aIsConnected));
            }
        }

        protected override void Dispose(bool disposing)
        {
            UserLog.WriteLine("PowerStateListener::Dispose");
            iScheduler.Stop();
            iScheduler = null;
            iContext.UnregisterReceiver(this);
            iContext = null;
            base.Dispose(disposing);
        }
    }

    public class EventArgsPowerState : EventArgs
    {

        public EventArgsPowerState(bool aIsConnected)
        {
            iIsConnected = aIsConnected;
        }

        public bool IsConnected { get { return iIsConnected; } }

        private bool iIsConnected;
    }
}