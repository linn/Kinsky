using System;
using System.Collections.Generic;
using Linn.ControlPoint.Upnp;
using Upnp;

namespace Linn.Topology
{
    public interface IModelSourceMediaRenderer : IModelSource
    {
        event EventHandler<EventArgs> EventSubscriptionError;
        event EventHandler<EventArgs> EventControlInitialised;
        event EventHandler<EventArgs> EventPlaylistChanged;
        event EventHandler<EventArgs> EventPlaylistInitialised;
        event EventHandler<EventArgs> EventRepeatChanged;
        event EventHandler<EventArgs> EventShuffleChanged;
        event EventHandler<EventArgs> EventTrackChanged;
        event EventHandler<EventArgs> EventTransportStateChanged;
        bool IsInserting();
        void Lock();
        void Next();
        void Pause();
        void Play();
        uint PlayLater(DidlLite aDidlLite);
        void PlaylistDelete(IList<MrItem> aPlaylistItems);
        void PlaylistDeleteAll();
        uint PlaylistInsert(uint aInsertAfterId, Upnp.DidlLite aDidlLite);
        Linn.Topology.MrItem PlaylistItem(uint aIndex);
        void PlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems);
        uint PlaylistTrackCount { get; }
        uint PlaylistTracksMax { get; }
        uint PlayNext(DidlLite aDidlLite);
        uint PlayNow(DidlLite aDidlLite);
        void Previous();
        string ProtocolInfo { get; }
        bool Repeat { get; }
        void SeekSeconds(uint aSeconds);
        void SeekTrack(uint aTrack);
        bool Shuffle { get; }
        void Stop();
        void ToggleRepeat();
        void ToggleShuffle();
        int TrackIndex { get; }
        MrItem TrackPlaylistItem { get; }
        ModelSourceMediaRenderer.ETransportState TransportState { get; }
        void Unlock();
    }
    public abstract class ModelSourceMediaRenderer : ModelSource, IMediaSupported, IModelSourceMediaRenderer
    {
        public event EventHandler<EventArgs> EventSubscriptionError;

        public enum ETransportState
        {
            eUnknown,
            ePlaying,
            ePaused,
            eStopped,
            eBuffering
        }

        public static ModelSourceMediaRenderer Create(Source aSource)
        {
            if (aSource != null)
            {
                if (aSource.Type == "Playlist")
                {
                    return (new ModelSourceMediaRendererDs(aSource));
                }
                if (aSource.Type == "UpnpAv")
                {
                    return (new ModelSourceMediaRendererUpnpAv(aSource));
                }
            }

            return (null);
        }

        protected void OnEventSubscriptionError()
        {
            EventHandler<EventArgs> eventSubscriptionError = EventSubscriptionError;
            if (eventSubscriptionError != null)
            {
                eventSubscriptionError(this, EventArgs.Empty);
            }
        }

        public abstract resource BestSupportedResource(upnpObject aObject);

        public abstract event EventHandler<EventArgs> EventControlInitialised;
        public abstract event EventHandler<EventArgs> EventPlaylistInitialised;

        public abstract event EventHandler<EventArgs> EventTransportStateChanged;
        public abstract event EventHandler<EventArgs> EventTrackChanged;
        public abstract event EventHandler<EventArgs> EventRepeatChanged;
        public abstract event EventHandler<EventArgs> EventShuffleChanged;

        public abstract event EventHandler<EventArgs> EventPlaylistChanged;

        public abstract void Play();
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Previous();
        public abstract void Next();
        public abstract void SeekSeconds(uint aSeconds);
        public abstract void SeekTrack(uint aTrack);
        public abstract void ToggleRepeat();
        public abstract void ToggleShuffle();

        public abstract ETransportState TransportState { get; }
        public abstract int TrackIndex { get; }
        public abstract MrItem TrackPlaylistItem { get; }

        public abstract bool Repeat { get; }
        public abstract bool Shuffle { get; }
        public abstract string ProtocolInfo { get; }

        public abstract void Lock();
        public abstract void Unlock();

        public abstract uint PlayNow(DidlLite aDidlLite);
        public abstract uint PlayNext(DidlLite aDidlLite);
        public abstract uint PlayLater(DidlLite aDidlLite);

        public abstract MrItem PlaylistItem(uint aIndex);
        public abstract void PlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems);
        public abstract uint PlaylistInsert(uint aInsertAfterId, DidlLite aDidlLite);
        public abstract void PlaylistDelete(IList<MrItem> aPlaylistItems);
        public abstract void PlaylistDeleteAll();
        public abstract bool IsInserting();

        public abstract uint PlaylistTrackCount { get; }
        public abstract uint PlaylistTracksMax { get; }
    }

} // Linn.Topology
