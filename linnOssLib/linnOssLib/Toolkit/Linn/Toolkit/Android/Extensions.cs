using Android.Content;
using Android.Views;
using Android.Util;
using Android.Runtime;

namespace OssToolkitDroid
{
    public static class ContextExtensions
    {
        public static float ThemedResourceAttribute(this Context aContext, int aResourceId)
        {
            TypedValue value = new TypedValue();
            aContext.Theme.ResolveAttribute(aResourceId, value, true);
            IWindowManager windowManager = aContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            DisplayMetrics outMetrics = new DisplayMetrics();
            windowManager.DefaultDisplay.GetMetrics(outMetrics);
            return value.GetDimension(outMetrics);
        }
    }
}