using System.Net;
using System.Collections.Generic;
using System;

namespace Linn
{

    public enum PlatformId
    {
        Win32S = 0,
        Win32Windows = 1,
        Win32NT = 2,
        WinCE = 3,
        Unix = 4,
        Xbox = 5,
        MacOSX = 6,
        IOS = 7,
        Unknown = 8,
    }

    public class PlatformIdConverters
    {
        static public PlatformId From(PlatformID aPlatformID)
        {
            switch (aPlatformID)
            {
                case PlatformID.MacOSX:
                    return PlatformId.MacOSX;
                case PlatformID.Unix:
                    return PlatformId.Unix;
                case PlatformID.Win32NT:
                    return PlatformId.Win32NT;
                case PlatformID.Win32S:
                    return PlatformId.Win32S;
                case PlatformID.Win32Windows:
                    return PlatformId.Win32Windows;
                case PlatformID.WinCE:
                    return PlatformId.WinCE;
                case PlatformID.Xbox:
                    return PlatformId.Xbox;
                default:
                    Assert.Check(false);
                    break;
            }

            return PlatformId.Unknown;
        }
    }

    public enum PowerModes
    {
        eResume,
        eStatusChange,
        eSuspend,
        eUnknown
    }

    public class PowerModeChangedEventArgs : EventArgs
    {
        private PowerModes iMode;
        public PowerModes Mode
        {
            get
            {
                return iMode;
            }
        }

        public PowerModeChangedEventArgs(PowerModes aModes)
        {
            iMode = aModes;
        }
    }
}
