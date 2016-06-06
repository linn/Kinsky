using System;
using System.Threading;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Linn.Kinsky;

using Upnp;

namespace KinskyTouch
{
    public partial class ArtworkTileViewFactory : UIViewController
    {
        internal class Connection : NSUrlConnectionDataDelegate
        {
            public Connection(ArtworkTileView aView)
            {
                iView = aView;
                iData = new NSMutableData();
            }
    
            public override void ReceivedData(NSUrlConnection connection, NSData data)
            {
                iData.AppendData(data);
            }
    
            public override void FailedWithError(NSUrlConnection connection, NSError error)
            {
            }
    
            public override void FinishedLoading(NSUrlConnection connection)
            {
                iView.Swap(new UIImage(iData));
                iData = null;
            }

            private ArtworkTileView iView;
            private NSMutableData iData;
        }

        public ArtworkTileViewFactory(MediaProviderLibrary aLibrary)
        {
            iLibrary = aLibrary;
            iTimer = new Timer(Elapsed);
            //iTimer.Change(0, 5000);
            iRandom = new Random((int)DateTime.Now.Ticks);
        }

        public ArtworkTileViewFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public void Initialise()
        {
            new Timer(Run, null, 5000, Timeout.Infinite);
        }

        private void Run(object state)
        {
            int count = View.Subviews.Length;

            Console.WriteLine("Initialise: " + count);

            for(int i = 0; i < count; ++i)
            {
                Uri uri = null;
                int loops = 0;
                while(uri == null)
                {
                    uri = GetArtwork(iLibrary, 0);
                    ++loops;
                }
                Console.WriteLine("loops=" + loops);

                BeginInvokeOnMainThread(delegate {
                    NSUrl url = new NSUrl(uri.AbsoluteUri);
                    NSUrlRequest request = new NSUrlRequest(url);
                    new NSUrlConnection(request, new Connection(View as ArtworkTileView), true);
                });
            }

            iTimer.Change(0, 5000);
        }

        private Uri GetArtwork(IContainer aContainer, uint aLevel)
        {
            try
            {
                uint count = aContainer.Open();
                if(count > 0)
                {
                    int index = iRandom.Next((int)count);
                    DidlLite didl = aContainer.Items((uint)index, 1);
    
                    foreach(upnpObject o in didl)
                    {
                        if(o is item || o is musicAlbum)
                        {
                            Uri uri = DidlLiteAdapter.ArtworkUri(o);
                            if(uri != null)
                            {
                                Console.WriteLine("aLevel=" + aLevel + " uri=" + uri.AbsoluteUri);
                                return uri;
                            }
                        }
                        if(o is container)
                        {
                            IContainer c = aContainer.ChildContainer(o as container);
                            return GetArtwork(c, ++aLevel);
                        }

                        return null;
                    }
                }
            }
            catch(Exception)
            {
            }

            return null;
        }

        private void Elapsed(object state)
        {
            Console.WriteLine(">Elapsed");
            Uri uri = null;
            int loops = 0;
            while(uri == null)
            {
                uri = GetArtwork(iLibrary, 0);
                ++loops;
            }

            Console.WriteLine("loops=" + loops);
            Console.WriteLine("uri=" + uri.AbsoluteUri);

            BeginInvokeOnMainThread(delegate {
                NSUrl url = new NSUrl(uri.AbsoluteUri);
                NSUrlRequest request = new NSUrlRequest(url);
                new NSUrlConnection(request, new Connection(View as ArtworkTileView), true);
            });
        }

        private Timer iTimer;
        private Random iRandom;
        private MediaProviderLibrary iLibrary;
    }

    public partial class ArtworkTileView : UIView
    {
        public ArtworkTileView(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iRandom = new Random((int)DateTime.Now.Ticks);
            iSelected = new List<int>();
            iToBeSelected = new List<int>();

            for(int i = 0; i < Subviews.Length; ++i)
            {
                iToBeSelected.Add(i);
            }
        }

        public void Swap(UIImage aImage)
        {
            int index = iRandom.Next(iToBeSelected.Count);
            Tile tile = Subviews[iToBeSelected[index]] as Tile;
            if(tile != null)
            {
                tile.Swap(aImage);
            }

            iSelected.Add(iToBeSelected[index]);
            iToBeSelected.RemoveAt(index);

            Console.WriteLine("iToBeSelected.Count=" + iToBeSelected.Count);
            if(iToBeSelected.Count == 0)
            {
                List<int> temp = iSelected;
                iSelected = iToBeSelected;
                iToBeSelected = temp;
            }
        }

        private Random iRandom;
        private List<int> iSelected;
        private List<int> iToBeSelected;
    }

    public partial class Tile : UIView
    {
        public Tile(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            iBackView = viewImageArtworkBack;
            iFrontView = viewImageArtworkFront;
        }

        public void Swap(UIImage aImage)
        {
            //BeginInvokeOnMainThread(delegate {
                iBackView.Image = aImage;
    
                UIView.Transition(iFrontView, iBackView, 1.0f, UIViewAnimationOptions.TransitionFlipFromLeft, () => {});
    
                UIImageView view = iBackView;
                iBackView = iFrontView;
                iFrontView = view;
            //});
        }

        private UIImageView iFrontView;
        private UIImageView iBackView;
    }
}

