using System;

using MonoTouch;
using UIKit;
using Foundation;

using Linn;
using Linn.Kinsky;
using Linn.Toolkit.Ios;

namespace KinskyTouch
{
    [Foundation.Register("HelperKinskyTouch")]
    public partial class HelperKinskyTouch : NSObject
    {
        public HelperKinskyTouch(IntPtr aInstance)
            : base(aInstance)
        {
            iHelper = new HelperKinsky(new string[] {}, new Invoker());

            UserLog.WriteLine("iOS version: " + UIDevice.CurrentDevice.SystemVersion);
			UserLog.WriteLine("MonoTouch version: " + ObjCRuntime.Constants.Version);

            UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;
            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.BatteryStateDidChangeNotification, delegate {
                EventOptionAutoLockValueChanged(this, EventArgs.Empty);
            });

            iCrashLogDumper = new CrashLogDumper(iHelper.Title, iHelper.Product, iHelper.Version);
            iHelper.AddCrashLogDumper(iCrashLogDumper);

            iOptionPageGeneral = new OptionPageGeneral("General");
            iHelper.AddOptionPage(iOptionPageGeneral);

            iOptionInsertMode = new OptionInsertMode();
            iHelper.AddOption(iOptionInsertMode);

            iOptionPageGeneral.OptionAutoLock.EventValueChanged += EventOptionAutoLockValueChanged;
            iOptionPageGeneral.OptionAutoSendCrashLog.EventValueChanged += EventOptionAutoSendCrashLogValueChanged;

            iHelper.ProcessOptionsFileAndCommandLine();

            EventOptionAutoLockValueChanged(this, EventArgs.Empty);
            EventOptionAutoSendCrashLogValueChanged(this, EventArgs.Empty);
        }

        public HelperKinsky Helper
        {
            get
            {
                return iHelper;
            }
        }


        public OptionBool OptionInstallId
        {
            get
            {
                return iHelper.OptionInstallId;
            }
        }

        public OptionBool OptionExtendedTrackInfo
        {
            get
            {
                return iOptionPageGeneral.OptionExtendedTrackInfo;
            }
        }

        public OptionBool OptionEnableLargeControls
        {
            get
            {
                return iOptionPageGeneral.OptionEnableLargeControls;
            }
        }

        public OptionBool OptionEnableRocker
        {
            get
            {
                return iOptionPageGeneral.OptionEnableRocker;
            }
        }

        public OptionBool OptionGroupTracks
        {
            get
            {
                return iOptionPageGeneral.OptionGroupTracks;
            }
        }

        public OptionEnum OptionInsertMode
        {
            get
            {
                return iOptionInsertMode;
            }
        }

        private void EventOptionAutoLockValueChanged(object sender, EventArgs e)
        {
            if(iOptionPageGeneral.OptionAutoLock.Value == "Always")
            {
                UIApplication.SharedApplication.IdleTimerDisabled = true;
            }
            else if(iOptionPageGeneral.OptionAutoLock.Value == "When charging")
            {
                if(UIDevice.CurrentDevice.BatteryState == UIDeviceBatteryState.Unplugged)
                {
                    UIApplication.SharedApplication.IdleTimerDisabled = false;
                }
                else
                {
                    UIApplication.SharedApplication.IdleTimerDisabled = true;
                }
            }
            else
            {
                UIApplication.SharedApplication.IdleTimerDisabled = false;
            }
        }

        private void EventOptionAutoSendCrashLogValueChanged(object sender, EventArgs e)
        {
            iCrashLogDumper.SetAutoSend(iOptionPageGeneral.OptionAutoSendCrashLog.Native);
        }

        private HelperKinsky iHelper;
        private CrashLogDumper iCrashLogDumper;
        private OptionPageGeneral iOptionPageGeneral;
        private OptionEnum iOptionInsertMode;
    }
}

