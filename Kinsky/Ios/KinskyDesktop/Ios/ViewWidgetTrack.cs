using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Upnp;

using Linn;
using Linn.Kinsky;

namespace KinskyTouch
{
    /// <summary>
    /// This is test summary
    /// </summary>
    internal class ViewWidgetTrackMetadata : IViewWidgetTrack
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="aViewInfo">
        /// A <see cref="UIViewInfo"/>
        /// </param>
        /// <param name="aOptionExtendedTrackInfo">
        /// A <see cref="OptionBool"/>
        /// </param>
        public ViewWidgetTrackMetadata(UIViewInfo aViewInfo, OptionBool aOptionExtendedTrackInfo)
        {
            iViewInfo = aViewInfo;

            iOptionExtendedTrackInfo = aOptionExtendedTrackInfo;
            iOptionExtendedTrackInfo.EventValueChanged += OptionExtendedTrackInfoChanged;
        }

        /// <summary>
        ///
        /// </summary>
        public void Open()
        {
            iOpen = true;
        }

        /// <summary>
        ///
        /// </summary>
        public void Close()
        {
            iViewInfo.Header = string.Empty;
            iViewInfo.SubHeader1 = string.Empty;
            iViewInfo.SubHeader2 = string.Empty;
            iViewInfo.SubHeader3 = string.Empty;
            iViewInfo.SetNeedsDisplay();

            iOpen = false;
        }

        /// <summary>
        ///
        /// </summary>
        public void Initialised()
        {
            if(iOpen)
            {
                // do nothing
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aObject">
        /// A <see cref="Upnp.upnpObject"/>
        /// </param>
        public void SetItem(Upnp.upnpObject aObject)
        {
            if(aObject != null)
            {
                iTitle = DidlLiteAdapter.Title(aObject);
                iMetatext = DidlLiteAdapter.Album(aObject);
                iMetatext2 = DidlLiteAdapter.Artist(aObject);
            }
            else
            {
                iTitle = string.Empty;
                iMetatext = string.Empty;
                iMetatext2 = string.Empty;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aObject">
        /// A <see cref="Upnp.upnpObject"/>
        /// </param>
        public void SetMetatext(Upnp.upnpObject aObject)
        {
            if(aObject != null)
            {
                iMetatext = DidlLiteAdapter.Title(aObject);
                iMetatext2 = string.Empty;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aBitrate">
        /// A <see cref="System.UInt32"/>
        /// </param>
        public void SetBitrate(uint aBitrate)
        {
            iBitrate = aBitrate;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aSampleRate">
        /// A <see cref="System.Single"/>
        /// </param>
        public void SetSampleRate(float aSampleRate)
        {
            iSampleRate = aSampleRate;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aBitDepth">
        /// A <see cref="System.UInt32"/>
        /// </param>
        public void SetBitDepth(uint aBitDepth)
        {
            iBitDepth = aBitDepth;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aCodec">
        /// A <see cref="System.String"/>
        /// </param>
        public void SetCodec(string aCodec)
        {
            iCodec = aCodec;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aLossless">
        /// A <see cref="System.Boolean"/>
        /// </param>
        public void SetLossless(bool aLossless)
        {
            iLossless = aLossless;
        }

        /// <summary>
        ///
        /// </summary>
        public void Update()
        {
            string technicalInfo = string.Empty;

            if(!string.IsNullOrEmpty(iCodec) && iOptionExtendedTrackInfo.Native)
            {
                technicalInfo += iCodec;
                if(iLossless)
                {
                    technicalInfo += string.Format("   {0} kHz / {1} bits", iSampleRate.ToString(), iBitDepth.ToString());
                }
                else
                {
                    technicalInfo += string.Format("   {0} kHz", iSampleRate.ToString());
                }
                technicalInfo += string.Format("   {0} kbps", iBitrate.ToString());
            }

            iViewInfo.Header = iTitle;
            iViewInfo.SubHeader1 = iMetatext;
            iViewInfo.SubHeader2 = iMetatext2;
            iViewInfo.SubHeader3 = technicalInfo;

            iViewInfo.SetNeedsDisplay();
        }

        private void OptionExtendedTrackInfoChanged(object sender, EventArgs e)
        {
            Update();
        }

        private bool iOpen;

        private string iTitle;
        private string iMetatext;
        private string iMetatext2;

        private uint iBitrate;
        private float iSampleRate;
        private uint iBitDepth;
        private string iCodec;
        private bool iLossless;

        private UIViewInfo iViewInfo;

        private OptionBool iOptionExtendedTrackInfo;
    }

    /// <summary>
    ///
    /// </summary>
    internal interface IImageReceiver
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="aImage">
        /// A <see cref="UIImage"/>
        /// </param>
        void SetImage(UIImage aImage);
    }

    /// <summary>
    ///
    /// </summary>
    internal class ViewWidgetTrackArtworkRetriever : IViewWidgetTrack, IImageReceiver
    {
        /// <summary>
        ///
        /// </summary>
        private class Delegate : NSUrlConnectionDataDelegate
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="aImageReceiver">
            /// A <see cref="IImageReceiver"/>
            /// </param>
            public Delegate(IImageReceiver aImageReceiver)
            {
                iImageReceiver = aImageReceiver;
                iData = new NSMutableData();
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="connection">
            /// A <see cref="NSUrlConnection"/>
            /// </param>
            /// <param name="data">
            /// A <see cref="NSData"/>
            /// </param>
            public override void ReceivedData(NSUrlConnection connection, NSData data)
            {
                iData.AppendData(data);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="connection">
            /// A <see cref="NSUrlConnection"/>
            /// </param>
            /// <param name="error">
            /// A <see cref="NSError"/>
            /// </param>
            public override void FailedWithError(NSUrlConnection connection, NSError error)
            {
                iImageReceiver.SetImage(KinskyTouch.Properties.ResourceManager.AlbumError);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="connection">
            /// A <see cref="NSUrlConnection"/>
            /// </param>
            public override void FinishedLoading(NSUrlConnection connection)
            {
                try
                {
                    UIImage image = new UIImage(iData);
                    iImageReceiver.SetImage(image);
                }
                catch
                {
                    iImageReceiver.SetImage(KinskyTouch.Properties.ResourceManager.AlbumError);
                }
            }

            private IImageReceiver iImageReceiver;
            private NSMutableData iData;
        }

        /// <summary>
        ///
        /// </summary>
        public ViewWidgetTrackArtworkRetriever()
        {
            iImageReceivers = new List<IImageReceiver>();
			iUriConverter = new ScalingUriConverter(kUpscaledArtworkSize, true, true);
        }

        /// <summary>
        ///
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public void Close()
        {
			if(iConnection != null)
			{
				iConnection.Cancel();
				iConnection = null;
			}
			SetImage(KinskyTouch.Properties.ResourceManager.Loading);
        }

        /// <summary>
        ///
        /// </summary>
        public void Initialised()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aObject">
        /// A <see cref="Upnp.upnpObject"/>
        /// </param>
        public void SetItem(Upnp.upnpObject aObject)
        {
			if(iConnection != null)
			{
				iConnection.Cancel();
				iConnection = null;
			}
			if(aObject != null)
            {
                Uri uri = DidlLiteAdapter.ArtworkUri(aObject);
				if(uri != null)
                    {
                        NSUrl url = new NSUrl(iUriConverter.Convert(uri.AbsoluteUri));
                        NSUrlRequest request = new NSUrlRequest(url);
                        iConnection = new NSUrlConnection(request, new Delegate(this), true);
                    }
                    else
                    {
                        SetImage(KinskyTouch.Properties.ResourceManager.Loading);
                    }
            }
            else
            {
				SetImage(KinskyTouch.Properties.ResourceManager.Loading);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aObject">
        /// A <see cref="Upnp.upnpObject"/>
        /// </param>
        public void SetMetatext(Upnp.upnpObject aObject)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aBitrate">
        /// A <see cref="System.UInt32"/>
        /// </param>
        public void SetBitrate(uint aBitrate)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aSampleRate">
        /// A <see cref="System.Single"/>
        /// </param>
        public void SetSampleRate(float aSampleRate)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aBitDepth">
        /// A <see cref="System.UInt32"/>
        /// </param>
        public void SetBitDepth(uint aBitDepth)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aCodec">
        /// A <see cref="System.String"/>
        /// </param>
        public void SetCodec(string aCodec)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aLossless">
        /// A <see cref="System.Boolean"/>
        /// </param>
        public void SetLossless(bool aLossless)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public void Update()
        {
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="aImage">
        /// A <see cref="UIImage"/>
        /// </param>
        public void SetImage(UIImage aImage)
        {
            foreach(IImageReceiver r in iImageReceivers)
            {
                r.SetImage(aImage);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aImageReceiver">
        /// A <see cref="IImageReceiver"/>
        /// </param>
        public void AddReceiver(IImageReceiver aImageReceiver)
        {
            if(!iImageReceivers.Contains(aImageReceiver))
            {
                iImageReceivers.Add(aImageReceiver);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aImageReceiver">
        /// A <see cref="IImageReceiver"/>
        /// </param>
        public void RemoveReceiver(IImageReceiver aImageReceiver)
        {
            iImageReceivers.Remove(aImageReceiver);
        }

        private NSUrlConnection iConnection;
        private List<IImageReceiver> iImageReceivers;
		private ScalingUriConverter iUriConverter;
		private const int kUpscaledArtworkSize = 1024;
    }

    /// <summary>
    ///
    /// </summary>
    internal class ViewWidgetTrackArtwork : IViewWidgetTrack, IImageReceiver
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="aUiImageViewArtwork">
        /// A <see cref="UIImageView"/>
        /// </param>
        public ViewWidgetTrackArtwork(UIImageView aUiImageViewArtwork)
        {
            iUiImageViewArtwork = aUiImageViewArtwork;
        }

        /// <summary>
        ///
        /// </summary>
        public void Open()
        {
            iOpen = true;
        }

        /// <summary>
        ///
        /// </summary>
        public void Close()
        {
            iUiImageViewArtwork.Hidden = true;
            iUiImageViewArtwork.Image = null;

            iOpen = false;
        }

        /// <summary>
        ///
        /// </summary>
        public void Initialised()
        {
            if(iOpen)
            {
                iUiImageViewArtwork.Hidden = false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aObject">
        /// A <see cref="Upnp.upnpObject"/>
        /// </param>
        public void SetItem(Upnp.upnpObject aObject)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aObject">
        /// A <see cref="Upnp.upnpObject"/>
        /// </param>
        public void SetMetatext(Upnp.upnpObject aObject)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aBitrate">
        /// A <see cref="System.UInt32"/>
        /// </param>
        public void SetBitrate(uint aBitrate)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aSampleRate">
        /// A <see cref="System.Single"/>
        /// </param>
        public void SetSampleRate(float aSampleRate)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aBitDepth">
        /// A <see cref="System.UInt32"/>
        /// </param>
        public void SetBitDepth(uint aBitDepth)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aCodec">
        /// A <see cref="System.String"/>
        /// </param>
        public void SetCodec(string aCodec)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aLossless">
        /// A <see cref="System.Boolean"/>
        /// </param>
        public void SetLossless(bool aLossless)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="aImage">
        /// A <see cref="UIImage"/>
        /// </param>
        public void SetImage(UIImage aImage)
        {
            iUiImageViewArtwork.Image = aImage;
        }

        private bool iOpen;

        private UIImageView iUiImageViewArtwork;
    }

    /// <summary>
    ///
    /// </summary>
    internal class ImageReceiverButton : IImageReceiver
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="aButton">
        /// A <see cref="UIButton"/>
        /// </param>
        public ImageReceiverButton(UIButton aButton)
        {
            iButton = aButton;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aImage">
        /// A <see cref="UIImage"/>
        /// </param>
        public void SetImage(UIImage aImage)
        {
            if(aImage == KinskyTouch.Properties.ResourceManager.Loading)
            {
                iButton.SetImage(aImage, UIControlState.Normal);
            }
            else
            {
                iButton.SetImage(aImage, UIControlState.Normal);
            }
        }

        UIButton iButton;
    }
}

