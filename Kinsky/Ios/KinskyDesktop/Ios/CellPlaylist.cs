using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace KinskyTouch
{
    public partial class CellPlaylistFactory : NSObject
    {
        public CellPlaylistFactory()
        {
        }

        public CellPlaylistFactory(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public CellPlaylist Cell
        {
            get
            {
                return cellPlaylist;
            }
        }
    }

    public partial class CellPlaylist : CellLazyLoadImage
    {
        public CellPlaylist(IntPtr aInstance)
            : base(aInstance)
        {
        }

        public override UIImage Image
        {
            get
            {
                return imageViewArtwork.Image;
            }
            
            set
            {
                imageViewArtwork.Image = value;
            }
        }

        public void SetPlaying(bool aPlaying, bool aAnimated)
        {
            if(aAnimated)
            {
                if(aPlaying)
                {
                    UIView.Transition(imageViewArtwork, imageViewPlaying, 0.5f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionFlipFromRight, () => {});
                }
                else
                {
                    UIView.Transition(imageViewPlaying, imageViewArtwork, 0.5f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionFlipFromLeft, () => {});
                }
            }
            else
            {
                if(aPlaying)
                {
                    UIView.Transition(imageViewArtwork, imageViewPlaying, 0.0f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionNone, () => {});
                }
                else
                {
                    UIView.Transition(imageViewPlaying, imageViewArtwork, 0.0f, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.TransitionNone, () => {});
                }
            }
        }

        public string Title
        {
            set
            {
                labelTitle.Text = value;
            }
        }

        public string Artist
        {
            set
            {
                labelArtist.Text = value;
            }
        }

        public string Album
        {
            set
            {
                labelAlbum.Text = value;
            }
        }

        public string DurationBitrate
        {
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    nfloat width = ContentView.Frame.Right - labelTitle.Frame.X;
                    labelTitle.Frame = new CGRect(labelTitle.Frame.X, labelTitle.Frame.Y, width, labelTitle.Frame.Height);
                }
                else
                {
                    nfloat width = labelDurationBitrate.Frame.X - labelTitle.Frame.X;
                    labelTitle.Frame = new CGRect(labelTitle.Frame.X, labelTitle.Frame.Y, width, labelTitle.Frame.Height);
                }
                labelDurationBitrate.Text = value;
            }
        }
    }
}