using Linn;
using Android.Content;
using Android.OS;
using System;
namespace OssToolkitDroid
{

    [BroadcastReceiver]
    public class ScreenStateListener : BroadcastReceiver
    {
        private bool iIsScreenOn;
        private Scheduler iScheduler;
        public event EventHandler<EventArgsScreenState> EventScreenStateChanged;
        private Context iContext;

        // default constructor to satisfy framework requirements, should not be used
        public ScreenStateListener()
            : base()
        {
            Assert.Check(false);
        }

        public ScreenStateListener(Context aContext)
            : base()
        {
            iScheduler = new Scheduler("ScreenStateScheduler", 1);
            iContext = aContext;
            iIsScreenOn = IsScreenOn(aContext);
            aContext.RegisterReceiver(this, new IntentFilter(Intent.ActionScreenOn));
            aContext.RegisterReceiver(this, new IntentFilter(Intent.ActionScreenOff));
        }


        public override void OnReceive(Context aContext, Intent aIntent)
        {
            iScheduler.Schedule(() =>
            {
                bool isScreenOn = IsScreenOn(aContext);
                UserLog.WriteLine("ScreenState.OnReceive: " + isScreenOn);
                OnEventScreenStateChanged(isScreenOn);
                aIntent.Dispose();
            });
        }

        public static bool IsScreenOn(Context aContext)
        {
            return ((PowerManager)aContext.GetSystemService(Context.PowerService)).IsScreenOn;
        }

        private void OnEventScreenStateChanged(bool aIsScreenOn)
        {
            EventHandler<EventArgsScreenState> del = EventScreenStateChanged;
            if (del != null)
            {
                del(this, new EventArgsScreenState(aIsScreenOn));
            }
        }

        protected override void Dispose(bool disposing)
        {
            UserLog.WriteLine("ScreenStateListener::Dispose");
            iScheduler.Stop();
            iScheduler = null;
            iContext.UnregisterReceiver(this);
            iContext = null;
            base.Dispose(disposing);
        }

    }

    public class EventArgsScreenState : EventArgs
    {

        public EventArgsScreenState(bool aIsScreenOn)
        {
            iIsScreenOn = aIsScreenOn;
        }

        public bool IsScreenOn { get { return iIsScreenOn; } }

        private bool iIsScreenOn;
    }




}