using System;

namespace Linn
{
    public static class SystemEvents
    {
        public static event EventHandler<PowerModeChangedEventArgs> PowerModeChanged;

        static SystemEvents()
        {
#warning TODO: PowerModeChanged not implemented
        }
    }
}
