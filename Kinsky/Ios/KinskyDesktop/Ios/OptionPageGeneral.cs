using System;

using Linn;

namespace KinskyTouch
{
    public class OptionPageGeneral : OptionPage
    {
        public OptionPageGeneral(string aName)
            : base(aName)
        {
            iOptionExtendedTrackInfo = new OptionBool("trackinfo", "Extended track info", "Show extended track information for the current track", true);
            Add(iOptionExtendedTrackInfo);

            iOptionEnableRocker = new OptionBool("rocker", "Button controls", "Enable button controls for controlling volume and seeking", false);
            Add(iOptionEnableRocker);

            iOptionEnableLargeControls = new OptionBool("largecontrols", "Large controls", "Enable large controls for controlling volume and seeking", false);
            //Add(iOptionEnableLargeControls);

            iOptionGroupTracks = new OptionBool("groupplaylist", "Group playlist tracks", "Grouping tracks by album within the playlist window", true);
            Add(iOptionGroupTracks);

            iOptionAutoLock = new OptionEnum("autolock", "Prevent auto-lock", "When to prevent auto-lock");
            iOptionAutoLock.AddDefault("Never");
            iOptionAutoLock.Add("When charging");
            iOptionAutoLock.Add("Always");
            Add(iOptionAutoLock);

            iOptionAutoSendCrashLog = new OptionBool("autosendcrashlog", "Auto send crash log", "Automatically send crash logs to Linn", true);
            Add(iOptionAutoSendCrashLog);
        }

        public OptionBool OptionExtendedTrackInfo
        {
            get
            {
                return iOptionExtendedTrackInfo;
            }
        }

        public OptionBool OptionEnableLargeControls
        {
            get
            {
                return iOptionEnableLargeControls;
            }
        }

        public OptionBool OptionEnableRocker
        {
            get
            {
                return iOptionEnableRocker;
            }
        }

        public OptionBool OptionGroupTracks
        {
            get
            {
                return iOptionGroupTracks;
            }
        }

        public OptionEnum OptionAutoLock
        {
            get
            {
                return iOptionAutoLock;
            }
        }

        public OptionBool OptionAutoSendCrashLog
        {
            get
            {
                return iOptionAutoSendCrashLog;
            }
        }

        private OptionBool iOptionGroupTracks;
        private OptionBool iOptionExtendedTrackInfo;
        private OptionBool iOptionEnableLargeControls;
        private OptionBool iOptionEnableRocker;
        private OptionEnum iOptionAutoLock;
        private OptionBool iOptionAutoSendCrashLog;
    }
}

