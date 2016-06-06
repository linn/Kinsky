using System.Collections.Generic;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Linn;
namespace OssToolkitDroid
{

    public class AndroidResourceManager
    {
        public AndroidResourceManager(Resources aResources)
        {
            iBitmapCache = new Dictionary<int, Bitmap>();
            iResources = aResources;
        }

        public Bitmap GetBitmap(int aResourceId)
        {
            Bitmap source;
            if (iBitmapCache.ContainsKey(aResourceId))
            {
                source = iBitmapCache[aResourceId];
            }
            else
            {
                Drawable drawable = iResources.GetDrawable(aResourceId);
                Assert.Check(drawable != null);
                source = (drawable as BitmapDrawable).Bitmap;
                Assert.Check(source != null);
                iBitmapCache[aResourceId] = source;
            }
            return source;
        }

        private Dictionary<int, Bitmap> iBitmapCache;
        private Resources iResources;
    }

}