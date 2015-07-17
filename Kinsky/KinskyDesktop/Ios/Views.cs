using System;
using System.Threading;
using System.Collections.Generic;

using Foundation;
using UIKit;

using Upnp;

using Linn;
using Linn.Topology;
using Linn.Kinsky;

namespace KinskyTouch
{
	internal class ViewWidgetTransportControl : IViewWidgetTransportControl
	{
		public ViewWidgetTransportControl(UIButton aButtonLeft, UIButton aButtonCentre, UIButton aButtonRight)
		{
			iButtonLeft = aButtonLeft;
			iButtonCentre = aButtonCentre;
			iButtonRight = aButtonRight;
			
			iButtonLeft.TouchUpInside += ButtonLeftTouchUpInside;
			iButtonCentre.TouchUpInside += ButtonCentreTouchUpInside;
			iButtonRight.TouchUpInside += ButtonRightTouchUpInside;
		}
		
		public void Open()
		{
			lock(this)
			{
				iOpen = true;
			}
		}

		public void Close()
		{
			lock(this)
			{
                iButtonLeft.Hidden = true;
				iButtonCentre.Enabled = false;
				iButtonRight.Hidden = true;

                SetCentreButtonImage(ETransportState.eStopped, 0, false);
				
				iOpen = false;
			}
		}

		public void Initialised()
		{
			lock(this)
			{
				if(iOpen)
				{
                    iButtonLeft.Enabled = true;
					iButtonCentre.Enabled = true;
					iButtonRight.Enabled = true;

                    SetCentreButtonImage(iTransportState, iDuration, iAllowPausing);
                    SetAllowSkipping(iAllowSkipping);
				}
			}
		}

		public void SetPlayNowEnabled(bool aEnabled)
		{
		}

		public void SetPlayNextEnabled(bool aEnabled)
		{
		}

		public void SetPlayLaterEnabled(bool aEnabled)
		{
		}

		public void SetDragging(bool aDragging)
		{
		}

        public void SetAllowSkipping(bool aAllowSkipping)
        {
            lock(this)
            {
                iAllowSkipping = aAllowSkipping;
                if(iOpen)
                {
                    iButtonLeft.Hidden = !aAllowSkipping;
                    iButtonRight.Hidden = !aAllowSkipping;
                }
            }
        }

        public void SetAllowPausing(bool aAllowPausing)
        {
            lock(this)
            {
                iAllowPausing = aAllowPausing;
                if(iOpen)
                {
                    SetCentreButtonImage(iTransportState, iDuration, aAllowPausing);
                }
            }
        }

		public void SetTransportState(ETransportState aTransportState)
		{
			lock(this)
			{
				iTransportState = aTransportState;
                if(iOpen)
                {
                    SetCentreButtonImage(aTransportState, iDuration, iAllowPausing);
                }
			}
		}

		public void SetDuration(uint aDuration)
		{
			lock(this)
			{
				iDuration = aDuration;
                if(iOpen)
                {
                    SetCentreButtonImage(iTransportState, aDuration, iAllowPausing);
                }
			}
		}

		public event EventHandler<EventArgs> EventPause;
		public event EventHandler<EventArgs> EventPlay;
		public event EventHandler<EventArgs> EventStop;
		
		public event EventHandler<EventArgs> EventPrevious;
		public event EventHandler<EventArgs> EventNext;
		
		public event EventHandler<EventArgsPlay> EventPlayNow;
		public event EventHandler<EventArgsPlay> EventPlayNext;
		public event EventHandler<EventArgsPlay> EventPlayLater;

        private void SetCentreButtonImage(ETransportState aTransportState, uint aDuration, bool aAllowPausing)
        {
            switch(aTransportState)
            {
            case ETransportState.eBuffering:
                iButtonCentre.SetImage(kImageStop, UIControlState.Normal);
                iButtonCentre.SetImage(kImageStopOver, UIControlState.Highlighted);
                break;
            case ETransportState.ePlaying:
                if(aDuration == 0 || !aAllowPausing)
                {
                    iButtonCentre.SetImage(kImageStop, UIControlState.Normal);
                    iButtonCentre.SetImage(kImageStopOver, UIControlState.Highlighted);
                }
                else
                {
                    iButtonCentre.SetImage(kImagePause, UIControlState.Normal);
                    iButtonCentre.SetImage(kImagePauseOver, UIControlState.Highlighted);
                }
                break;
            case ETransportState.ePaused:
            case ETransportState.eStopped:
            case ETransportState.eUnknown:
                iButtonCentre.SetImage(kImagePlay, UIControlState.Normal);
                iButtonCentre.SetImage(kImagePlayOver, UIControlState.Highlighted);
                break;
            default:
                Assert.Check(false);
                break;
            }
        }

		private void ButtonLeftTouchUpInside(object sender, EventArgs e)
		{
			if(EventPrevious != null)
			{
				EventPrevious(this, EventArgs.Empty);
			}
		}
		
		private void ButtonCentreTouchUpInside(object sender, EventArgs e)
		{
			bool eventPause = false;
			bool eventStop = false;
			bool eventPlay = false;
			
			lock(this)
			{
				if(iTransportState == ETransportState.ePlaying)
				{
					if(iDuration > 0 && iAllowPausing)
					{
						eventPause = true;
					}
					else
					{
						eventStop = true;
					}
				}
                else if(iTransportState == ETransportState.eBuffering)
                {
                    eventStop = true;
                }
				else
				{
					eventPlay = true;
				}
			}
			
			if(eventPause && EventPause != null)
			{
				EventPause(this, EventArgs.Empty);
			}
			
			if(eventStop && EventStop != null)
			{
				EventStop(this, EventArgs.Empty);
			}
			
			if(eventPlay && EventPlay != null)
			{
				EventPlay(this, EventArgs.Empty);
			}
		}
		
		private void ButtonRightTouchUpInside(object sender, EventArgs e)
		{
			if(EventNext != null)
			{
				EventNext(this, EventArgs.Empty);
			}
		}

        private static UIImage kImageStop = new UIImage("Stop.png");
        private static UIImage kImageStopOver = new UIImage("StopOver.png");
        private static UIImage kImagePlay = new UIImage("Play.png");
        private static UIImage kImagePlayOver = new UIImage("PlayOver.png");
        private static UIImage kImagePause = new UIImage("Pause.png");
        private static UIImage kImagePauseOver = new UIImage("PauseOver.png");
		
		private bool iOpen;
		private ETransportState iTransportState;
        private bool iAllowSkipping;
        private bool iAllowPausing;
		private uint iDuration;
		
		private UIButton iButtonLeft;
		private UIButton iButtonCentre;
		private UIButton iButtonRight;
	}

    internal class ViewWidgetPlayMode : IViewWidgetPlayMode
    {
        public ViewWidgetPlayMode(SourceToolbar aSourceToolbar, UIButton aButtonShuffle, UIButton aButtonRepeat)
        {
            iSourceToolbar = aSourceToolbar;
            iButtonShuffle = aButtonShuffle;
            iButtonRepeat = aButtonRepeat;

            iButtonShuffle.TouchUpInside += ButtonShuffleTouchUpInside;
            iButtonRepeat.TouchUpInside += ButtonRepeatTouchUpInside;
        }

        public void Open()
        {
            lock(this)
            {
                iOpen = true;
            }
        }

        public void Close()
        {
            lock(this)
            {
                iButtonShuffle.Hidden = true;
                iButtonRepeat.Hidden = true;
                iSourceToolbar.SetAllowPlayMode(false);

                iOpen = false;
            }
        }

        public void Initialised()
        {
            lock(this)
            {
                if(iOpen)
                {
                    iButtonShuffle.Hidden = false;
                    iButtonRepeat.Hidden = false;
                    iSourceToolbar.SetAllowPlayMode(true);
                }
            }
        }

        public void SetShuffle(bool aShuffle)
        {
            lock(this)
            {
                iButtonShuffle.Selected = aShuffle;
            }
        }

        public void SetRepeat(bool aRepeat)
        {
            lock(this)
            {
                iButtonRepeat.Selected = aRepeat;
            }
        }

        public event EventHandler<EventArgs> EventToggleShuffle;
        public event EventHandler<EventArgs> EventToggleRepeat;

        private void ButtonShuffleTouchUpInside(object sender, EventArgs e)
        {
            if(EventToggleShuffle != null)
            {
                EventToggleShuffle(this, EventArgs.Empty);
            }
        }

        private void ButtonRepeatTouchUpInside(object sender, EventArgs e)
        {
            if(EventToggleRepeat != null)
            {
                EventToggleRepeat(this, EventArgs.Empty);
            }
        }

        private bool iOpen;

        private SourceToolbar iSourceToolbar;

        private UIButton iButtonShuffle;
        private UIButton iButtonRepeat;
    }
}