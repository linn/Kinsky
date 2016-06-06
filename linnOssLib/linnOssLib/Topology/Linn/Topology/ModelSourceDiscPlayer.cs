using System;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public interface IModelSourceDiscPlayer : IModelSource
    {
        ModelSourceDiscPlayer.EDiscState DiscState { get; }
        ModelSourceDiscPlayer.EDiscType DiscType { get; }
        ModelSourceDiscPlayer.EDomain Domain { get; }
        void Eject();
        event EventHandler<EventArgs> EventDiscStateChanged;
        event EventHandler<EventArgs> EventDiscTypeChanged;
        event EventHandler<EventArgs> EventDomainChanged;
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventPlayStateChanged;
        event EventHandler<EventArgs> EventProgramModeChanged;
        event EventHandler<EventArgs> EventRepeatModeChanged;
        event EventHandler<EventArgs> EventTrackCountChanged;
        event EventHandler<EventArgs> EventTrackDurationChanged;
        event EventHandler<EventArgs> EventTrackIndexChanged;
        event EventHandler<EventArgs> EventTrayStateChanged;
        void Next();
        void Pause();
        void Play();
        ModelSourceDiscPlayer.EPlayState PlayState { get; }
        void Previous();
        ModelSourceDiscPlayer.EProgramMode ProgramMode { get; }
        ModelSourceDiscPlayer.ERepeatMode RepeatMode { get; }
        void SeekPosition(float aPosition);
        void SeekSeconds(uint aSeconds);
        void SeekTrack(uint aTrack);
        void Stop();
        void ToggleRepeat();
        void ToggleShuffle();
        uint TrackCount { get; }
        uint TrackDuration { get; }
        int TrackIndex { get; }
        ModelSourceDiscPlayer.ETrayState TrayState { get; }
        event EventHandler<EventArgs> EventSubscriptionError;
    }

    public abstract class ModelSourceDiscPlayer : ModelSource, IModelSourceDiscPlayer
    {
        public event EventHandler<EventArgs> EventSubscriptionError;
        public enum ETrayState
        {
            eOpened,
            eClosed,
            eOpening,
            eClosing,
            eUnknown
        }

        public enum EDiscState
        {
            eLoading,
            eLoaded,
            eUnknown
        }

        public enum EPlayState
        {
            ePlaying,
            eStopped,
            ePaused,
            eSuspended,
            eUnknown
        }

        public enum ERepeatMode
        {
            eOff,
            eAll,
            eTrack,
            eAB
        }

        public enum EProgramMode
        {
            eOff,
            eRandom,
            eShuffle,
            eStored
        }

        public enum EDiscType
        {
            eUnsupported,
            eNoDisc,
            eCd,
            eSacd,
            eVcd,
            eSvcd,
            eDvd,
            eDvdAudio,
            eData,
            eUnknown
        }

        public enum EDomain
        {
            eNone,
            eRootMenu,
            eTitleMenu,
            eCopyright,
            ePassword
        }

        protected void OnEventSubscriptionError()
        {
            EventHandler<EventArgs> eventSubscriptionError = EventSubscriptionError;
            if (eventSubscriptionError != null)
            {
                eventSubscriptionError(this, EventArgs.Empty);
            }
        }
        public abstract event EventHandler<EventArgs> EventInitialised;

        public abstract event EventHandler<EventArgs> EventTrayStateChanged;
        public abstract event EventHandler<EventArgs> EventDiscStateChanged;
        public abstract event EventHandler<EventArgs> EventPlayStateChanged;
        public abstract event EventHandler<EventArgs> EventRepeatModeChanged;
        public abstract event EventHandler<EventArgs> EventProgramModeChanged;
        public abstract event EventHandler<EventArgs> EventDiscTypeChanged;
        public abstract event EventHandler<EventArgs> EventDomainChanged;

        public abstract event EventHandler<EventArgs> EventTrackIndexChanged;
        public abstract event EventHandler<EventArgs> EventTrackCountChanged;
        public abstract event EventHandler<EventArgs> EventTrackDurationChanged;

        public abstract void Eject();
        public abstract void Play();
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Previous();
        public abstract void Next();
        public abstract void SeekPosition(float aPosition);
        public abstract void SeekSeconds(uint aSeconds);
        public abstract void SeekTrack(uint aTrack);
        public abstract void ToggleShuffle();
        public abstract void ToggleRepeat();

        public abstract ETrayState TrayState { get; }
        public abstract EDiscState DiscState { get; }
        public abstract EPlayState PlayState { get; }
        public abstract ERepeatMode RepeatMode { get; }
        public abstract EProgramMode ProgramMode { get; }
        public abstract EDiscType DiscType { get; }
        public abstract EDomain Domain { get; }

        public abstract uint TrackCount { get; }
        public abstract int TrackIndex { get; }
        public abstract uint TrackDuration { get; }
    }

} // Linn.Topology
