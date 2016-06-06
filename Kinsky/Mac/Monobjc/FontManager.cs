
using System;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    public static class FontManager
    {
        static private NSFont iSmall = NSFont.SystemFontOfSize(10.0f);
        static private NSFont iMedium = NSFont.SystemFontOfSize(12.0f);
        static private NSFont iSemiLarge = NSFont.SystemFontOfSize(13.0f);
        static private NSFont iLarge = NSFont.SystemFontOfSize(16.0f);

        public static NSFont FontSmall
        {
            get { return iSmall; }
        }

        public static NSFont FontMedium
        {
            get { return iMedium; }
        }

        public static NSFont FontSemiLarge
        {
            get { return iSemiLarge; }
        }

        public static NSFont FontLarge
        {
            get { return iLarge; }
        }
    }
}

