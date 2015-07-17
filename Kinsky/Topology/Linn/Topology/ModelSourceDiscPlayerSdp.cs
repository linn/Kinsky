using Linn.ControlPoint.Upnp;
using Linn.ControlPoint;
using System;
using Upnp;

namespace Linn.Topology
{
    public class ModelSourceDiscPlayerSdp : ModelSourceDiscPlayer
    {
        public ModelSourceDiscPlayerSdp(Source aSource)
        {
            iSource = aSource;
            iInitialised = false;

            try
            {
                iServiceSdp = new ServiceSdp(iSource.Device, iSource.House.EventServer);
            }
            catch (ServiceException)
            {
                throw new ModelSourceException(301, "Service failure");
            }

            iActionSetNext = iServiceSdp.CreateAsyncActionSetNext();
            iActionSetPrev = iServiceSdp.CreateAsyncActionSetPrev();
            iActionOpen = iServiceSdp.CreateAsyncActionOpen();
            iActionClose = iServiceSdp.CreateAsyncActionClose();
            iActionPlay = iServiceSdp.CreateAsyncActionPlay();
            iActionPause = iServiceSdp.CreateAsyncActionPause();
            iActionStop = iServiceSdp.CreateAsyncActionStop();
            iActionSetTime = iServiceSdp.CreateAsyncActionSetTime();
            iActionSetTrack = iServiceSdp.CreateAsyncActionSetTrack();
            iActionSetProgramShuffle = iServiceSdp.CreateAsyncActionSetProgramShuffle();
            iActionSetProgramOff = iServiceSdp.CreateAsyncActionSetProgramOff();
            iActionSetRepeatAll = iServiceSdp.CreateAsyncActionSetRepeatAll();
            iActionSetRepeatOff = iServiceSdp.CreateAsyncActionSetRepeatOff();
        }

        public override void Open()
        {
            iServiceSdp.EventStateDiscLength += EventStateDiscLengthResponse;
            iServiceSdp.EventStateDiscType += EventStateDiscTypeResponse;
            iServiceSdp.EventStateDiscState += EventStateDiscStateResponse;
            iServiceSdp.EventStateDomain += EventStateDomainResponse;
            iServiceSdp.EventStatePlayState += EventStatePlayStateResponse;
            iServiceSdp.EventStateProgramMode += EventStateProgramModeResponse;
            iServiceSdp.EventStateRepeatMode += EventStateRepeatModeResponse;
            iServiceSdp.EventStateTrack += EventStateTrackResponse;
            iServiceSdp.EventStateTrackLength += EventStateTrackLengthResponse;
            iServiceSdp.EventStateTrayState += EventStateTrayStateResponse;
            iServiceSdp.EventStateTotalTracks += EventStateTotalTracksResponse;
            iServiceSdp.EventSubscriptionError += EventSubscriptionErrorHandler;
            iServiceSdp.EventInitial += EventInitialResponse;
        }

        public override void Close()
        {
            iServiceSdp.EventStateDiscLength -= EventStateDiscLengthResponse;
            iServiceSdp.EventStateDiscType -= EventStateDiscTypeResponse;
            iServiceSdp.EventStateDiscState -= EventStateDiscStateResponse;
            iServiceSdp.EventStateDomain -= EventStateDomainResponse;
            iServiceSdp.EventStatePlayState -= EventStatePlayStateResponse;
            iServiceSdp.EventStateProgramMode -= EventStateProgramModeResponse;
            iServiceSdp.EventStateRepeatMode -= EventStateRepeatModeResponse;
            iServiceSdp.EventStateTrack -= EventStateTrackResponse;
            iServiceSdp.EventStateTrackLength -= EventStateTrackLengthResponse;
            iServiceSdp.EventStateTrayState -= EventStateTrayStateResponse;
            iServiceSdp.EventStateTotalTracks -= EventStateTotalTracksResponse;
            iServiceSdp.EventSubscriptionError -= EventSubscriptionErrorHandler;
            iServiceSdp.EventInitial -= EventInitialResponse;
        }

        public override string Name
        {
            get
            {
                return (iSource.FullName);
            }
        }

        public override Source Source
        {
            get
            {
                return iSource;
            }
        }

        public override event EventHandler<EventArgs> EventInitialised;

        public override event EventHandler<EventArgs> EventTrayStateChanged;
        public override event EventHandler<EventArgs> EventDiscStateChanged;
        public override event EventHandler<EventArgs> EventPlayStateChanged;
        public override event EventHandler<EventArgs> EventRepeatModeChanged;
        public override event EventHandler<EventArgs> EventProgramModeChanged;
        public override event EventHandler<EventArgs> EventDiscTypeChanged;
        public override event EventHandler<EventArgs> EventDomainChanged;

        public override event EventHandler<EventArgs> EventTrackIndexChanged;
        public override event EventHandler<EventArgs> EventTrackCountChanged;
        public override event EventHandler<EventArgs> EventTrackDurationChanged;

        public override void Eject()
        {
            if (iInitialised)
            {
                if (iTrayState == ETrayState.eClosed || iTrayState == ETrayState.eClosing)
                {
                    iActionOpen.OpenBegin();
                }
                else if (iTrayState == ETrayState.eOpened || iTrayState == ETrayState.eOpening)
                {
                    iActionClose.CloseBegin();
                }
            }
        }

        public override void Play()
        {
            iActionPlay.PlayBegin();
        }

        public override void Pause()
        {
            iActionPause.PauseBegin();
        }

        public override void Stop()
        {
            iActionStop.StopBegin();
        }

        public override void Previous()
        {
            iActionSetPrev.SetPrevBegin("SkipTrack");
        }

        public override void Next()
        {
            iActionSetNext.SetNextBegin("SkipTrack");
        }

        public override void SeekPosition(float aPosition)
        {
            //iActionSetTime.SetTimeBegin();
        }

        public override void SeekSeconds(uint aSeconds)
        {
            Time time = new Time((int)aSeconds);
            iActionSetTime.SetTimeBegin(time.ToString() + ".00", 0);
        }

        public override void SeekTrack(uint aTrack)
        {
            iActionSetTrack.SetTrackBegin((int)(aTrack + 1), 0);
        }

        public override void ToggleShuffle()
        {
            if (iInitialised)
            {
                if (iProgramMode == EProgramMode.eOff)
                {
                    iActionSetProgramShuffle.SetProgramShuffleBegin();
                }
                else
                {
                    iActionSetProgramOff.SetProgramOffBegin();
                }
            }
        }

        public override void ToggleRepeat()
        {
            if (iInitialised)
            {
                if (iRepeatMode == ERepeatMode.eOff)
                {
                    iActionSetRepeatAll.SetRepeatAllBegin();
                }
                else
                {
                    iActionSetRepeatOff.SetRepeatOffBegin();
                }
            }
        }

        public override ETrayState TrayState
        {
            get
            {
                return iTrayState;
            }
        }

        public override EDiscState DiscState
        {
            get
            {
                return iDiscState;
            }
        }

        public override EPlayState PlayState
        {
            get
            {
                return iPlayState;
            }
        }

        public override ERepeatMode RepeatMode
        {
            get
            {
                return iRepeatMode;
            }
        }

        public override EProgramMode ProgramMode
        {
            get
            {
                return iProgramMode;
            }
        }

        public override EDiscType DiscType
        {
            get
            {
                return iDiscType;
            }
        }

        public override EDomain Domain
        {
            get
            {
                return iDomain;
            }
        }

        public override uint TrackCount
        {
            get
            {
                return (uint)iServiceSdp.TotalTracks;
            }
        }

        public override int TrackIndex
        {
            get
            {
                return iServiceSdp.Track;
            }
        }

        public override uint TrackDuration
        {
            get
            {
                Time time = new Time(iServiceSdp.TrackLength);
                return (uint)time.SecondsTotal;
            }
        }

        private void EventInitialResponse(object obj, EventArgs e)
        {
            iInitialised = true;

            if (EventInitialised != null)
            {
                EventInitialised(this, EventArgs.Empty);
            }
        }

        private void EventStateDiscElapsedTimeResponse(object obj, EventArgs e)
        {
        }

        private void EventStateDiscLengthResponse(object obj, EventArgs e)
        {
        }

        private void EventStateDiscRemainingTimeResponse(object obj, EventArgs e)
        {
        }

        private void EventStateDiscTypeResponse(object obj, EventArgs e)
        {
            string type = iServiceSdp.DiscType;
            if (type == "CD")
            {
                iDiscType = EDiscType.eCd;
            }
            else if (type == "Data Disc")
            {
                iDiscType = EDiscType.eData;
            }
            else if (type == "DVD")
            {
                iDiscType = EDiscType.eDvd;
            }
            else if (type == "DVD-Audio")
            {
                iDiscType = EDiscType.eDvdAudio;
            }
            else if (type == "No Disc")
            {
                iDiscType = EDiscType.eNoDisc;
            }
            else if (type == "SACD")
            {
                iDiscType = EDiscType.eSacd;
            }
            else if (type == "SVCD")
            {
                iDiscType = EDiscType.eSvcd;
            }
            else if (type == "Unknown")
            {
                iDiscType = EDiscType.eUnknown;
            }
            else if (type == "Unsupported Disc Type")
            {
                iDiscType = EDiscType.eUnsupported;
            }
            else if (type == "VCD")
            {
                iDiscType = EDiscType.eVcd;
            }
            else
            {
                Trace.WriteLine(Trace.kSdp, type);
                Assert.Check(false);
            }

            if (EventDiscTypeChanged != null)
            {
                EventDiscTypeChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateDiscStateResponse(object obj, EventArgs e)
        {
            string state = iServiceSdp.DiscState;
            if (state == "Disc Loading")
            {
                iDiscState = EDiscState.eLoading;
            }
            else if (state == "Disc Loaded")
            {
                iDiscState = EDiscState.eLoaded;
            }
            else
            {
                iDiscState = EDiscState.eUnknown;
            }

            if (EventDiscStateChanged != null)
            {
                EventDiscStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateDomainResponse(object obj, EventArgs e)
        {
            string domain = iServiceSdp.Domain;
            if (domain == "Copyright")
            {
                iDomain = EDomain.eCopyright;
            }
            else if (domain == "Password")
            {
                iDomain = EDomain.ePassword;
            }
            else if (domain == "Title Menu")
            {
                iDomain = EDomain.eTitleMenu;
            }
            else if (domain == "Root Menu")
            {
                iDomain = EDomain.eRootMenu;
            }
            else if (domain == "None")
            {
                iDomain = EDomain.eNone;
            }
            else
            {
                Trace.WriteLine(Trace.kSdp, domain);
                Assert.Check(false);
            }

            if (EventDomainChanged != null)
            {
                EventDomainChanged(this, EventArgs.Empty);
            }
        }

        private void EventStatePlayStateResponse(object obj, EventArgs e)
        {
            string playState = iServiceSdp.PlayState;
            if (playState == "Playing")
            {
                iPlayState = EPlayState.ePlaying;
            }
            else if (playState == "Stopped")
            {
                iPlayState = EPlayState.eStopped;
            }
            else if (playState == "Paused")
            {
                iPlayState = EPlayState.ePaused;
            }
            else if (playState == "Suspended")
            {
                iPlayState = EPlayState.eSuspended;
            }

            else if (playState == "Unknown")
            {
                iPlayState = EPlayState.eUnknown;
            }
            else
            {
                Trace.WriteLine(Trace.kSdp, playState);
                Assert.Check(false);
            }

            if (EventPlayStateChanged != null)
            {
                EventPlayStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateProgramModeResponse(object obj, EventArgs e)
        {
            string programMode = iServiceSdp.ProgramMode;
            if (programMode == "Off")
            {
                iProgramMode = EProgramMode.eOff;
            }
            else if (programMode == "Random")
            {
                iProgramMode = EProgramMode.eRandom;
            }
            else if (programMode == "Shuffle")
            {
                iProgramMode = EProgramMode.eShuffle;
            }
            else if (programMode == "Stored")
            {
                iProgramMode = EProgramMode.eStored;
            }
            else
            {
                Trace.WriteLine(Trace.kSdp, programMode);
                Assert.Check(false);
            }

            if (EventProgramModeChanged != null)
            {
                EventProgramModeChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateRepeatModeResponse(object obj, EventArgs e)
        {
            string repeatMode = iServiceSdp.RepeatMode;
            if (repeatMode == "Off")
            {
                iRepeatMode = ERepeatMode.eOff;
            }
            else if (repeatMode == "All")
            {
                iRepeatMode = ERepeatMode.eAll;
            }
            else if (repeatMode == "Track")
            {
                iRepeatMode = ERepeatMode.eTrack;
            }
            else if (repeatMode == "A-B")
            {
                iRepeatMode = ERepeatMode.eAB;
            }
            else
            {
                Trace.WriteLine(Trace.kSdp, repeatMode);
                Assert.Check(false);
            }

            if (EventRepeatModeChanged != null)
            {
                EventRepeatModeChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateTrackResponse(object obj, EventArgs e)
        {
            if (EventTrackIndexChanged != null)
            {
                EventTrackIndexChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateTrackLengthResponse(object obj, EventArgs e)
        {
            if (EventTrackDurationChanged != null)
            {
                EventTrackDurationChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateTrackRemainingTimeResponse(object obj, EventArgs e)
        {
        }

        private void EventStateTrayStateResponse(object obj, EventArgs e)
        {
            string trayState = iServiceSdp.TrayState;
            if (trayState == "Tray Open")
            {
                iTrayState = ETrayState.eOpened;
            }
            else if (trayState == "Tray Closed")
            {
                iTrayState = ETrayState.eClosed;
            }
            else if (trayState == "Tray Opening")
            {
                iTrayState = ETrayState.eOpening;
            }
            else if (trayState == "Tray Closing")
            {
                iTrayState = ETrayState.eClosing;
            }
            else if (trayState == "Unknown")
            {
                iTrayState = ETrayState.eUnknown;
            }
            else
            {
                Trace.WriteLine(Trace.kSdp, trayState);
                Assert.Check(false);
            }

            if (EventTrayStateChanged != null)
            {
                EventTrayStateChanged(this, EventArgs.Empty);
            }
        }

        private void EventStateTotalTracksResponse(object obj, EventArgs e)
        {
            if (EventTrackCountChanged != null)
            {
                EventTrackCountChanged(this, EventArgs.Empty);
            }
        }

        private void EventSubscriptionErrorHandler(object sender, EventArgs e)
        {
            OnEventSubscriptionError();
        }

        private Source iSource;
        private bool iInitialised;
        private ServiceSdp iServiceSdp;

        private ETrayState iTrayState;
        private EDiscState iDiscState;
        private EPlayState iPlayState;
        private ERepeatMode iRepeatMode;
        private EProgramMode iProgramMode;
        private EDiscType iDiscType;
        private EDomain iDomain;

        private ServiceSdp.AsyncActionSetNext iActionSetNext;
        private ServiceSdp.AsyncActionSetPrev iActionSetPrev;
        private ServiceSdp.AsyncActionOpen iActionOpen;
        private ServiceSdp.AsyncActionClose iActionClose;
        private ServiceSdp.AsyncActionPlay iActionPlay;
        private ServiceSdp.AsyncActionPause iActionPause;
        private ServiceSdp.AsyncActionStop iActionStop;
        private ServiceSdp.AsyncActionSetTime iActionSetTime;
        private ServiceSdp.AsyncActionSetTrack iActionSetTrack;
        private ServiceSdp.AsyncActionSetProgramShuffle iActionSetProgramShuffle;
        private ServiceSdp.AsyncActionSetProgramOff iActionSetProgramOff;
        private ServiceSdp.AsyncActionSetRepeatAll iActionSetRepeatAll;
        private ServiceSdp.AsyncActionSetRepeatOff iActionSetRepeatOff;
    }
} // Linn.Topology
