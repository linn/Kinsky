
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace KinskyTouch
{
    public class Application
    {
        private readonly string kApiKey = "7898d7061464381703ade7e3c9c305f64a9db1c4";

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
