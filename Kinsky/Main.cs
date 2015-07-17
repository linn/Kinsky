
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace KinskyTouch
{
    public class Application
    {
        private readonly string kApiKey = "129c76d1b4043e568d19a9fea8a1f5534cdae703";

        static void Main (string[] args)
        {
#if DEBUG
            Xamarin.Insights.Initialize(Xamarin.Insights.DebugModeKey);
#else
            Xamarin.Insights.Initialize(kApiKey);
#endif
            UIApplication.Main(args);
        }
    }
}
