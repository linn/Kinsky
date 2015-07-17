using System;
using Linn.Topology;
using System.Collections.Generic;
using Upnp;
using System.Threading;

namespace Linn.Kinsky
{

    public interface IView
    {
        IViewWidgetSelector<Room> ViewWidgetSelectorRoom { get; }
        IViewWidgetButton ViewWidgetButtonStandby { get; }
        IViewWidgetSelector<Source> ViewWidgetSelectorSource { get; }
        IViewWidgetVolumeControl ViewWidgetVolumeControl { get; }
        IViewWidgetMediaTime ViewWidgetMediaTime { get; }
        IViewWidgetTransportControl ViewWidgetTransportControlMediaRenderer { get; }
        IViewWidgetTransportControl ViewWidgetTransportControlDiscPlayer { get; }
        IViewWidgetTransportControl ViewWidgetTransportControlRadio { get; }
        IViewWidgetTrack ViewWidgetTrack { get; }
        IViewWidgetPlayMode ViewWidgetPlayMode { get; }
        IViewWidgetPlaylist ViewWidgetPlaylist { get; }
        IViewWidgetPlaylistRadio ViewWidgetPlaylistRadio { get; }
        IViewWidgetPlaylistReceiver ViewWidgetPlaylistReceiver { get; }
        IViewWidgetPlaylistAux ViewWidgetPlaylistAux { get; }
        IViewWidgetPlaylistDiscPlayer ViewWidgetPlaylistDiscPlayer { get; }
        IViewWidgetButton ViewWidgetButtonSave { get; }
        IViewWidgetButton ViewWidgetButtonWasteBin { get; }
        IViewWidgetReceivers ViewWidgetReceivers { get; }
        IViewWidgetButton ViewWidgetButtonReceivers { get; }
    }

    public class EventArgsSelection<T> : EventArgs
    {
        public EventArgsSelection(T aTag)
        {
            Tag = aTag;
        }

        public T Tag;
    }

    public interface IViewWidgetSelector<T>
    {
        void Open();
        void Close();

        void InsertItem(int aIndex, T aItem);
        void RemoveItem(T aItem);
        void ItemChanged(T aItem);

        void SetSelected(T aItem);

        event EventHandler<EventArgsSelection<T>> EventSelectionChanged;
    }

    public interface IViewWidgetButton
    {
        void Open();
        void Close();

        //void SetEnabled(bool aEnabled);

        event EventHandler<EventArgs> EventClick;
    }
	
    public class EventArgsVolume : EventArgs
    {
        public EventArgsVolume(uint aVolume)
        {
            Volume = aVolume;
        }

        public uint Volume;
    }

    public class EventArgsMute : EventArgs
    {
        public EventArgsMute(bool aMute)
        {
            Mute = aMute;
        }

        public bool Mute;
    }

    public interface IViewWidgetVolumeControl
    {
        void Open();
        void Close();

        void Initialised();

        void SetVolume(uint aVolume);
        void SetMute(bool aMute);
        void SetVolumeLimit(uint aVolumeLimit);

        event EventHandler<EventArgs> EventVolumeIncrement;
        event EventHandler<EventArgs> EventVolumeDecrement;
        event EventHandler<EventArgsVolume> EventVolumeChanged;
        event EventHandler<EventArgsMute> EventMuteChanged;
    }

    public class EventArgsSeekSeconds : EventArgs
    {
        public EventArgsSeekSeconds(uint aSeconds)
        {
            Seconds = aSeconds;
        }

        public uint Seconds;
    }

    public enum ETransportState
    {
        eBuffering,
        ePlaying,
        ePaused,
        eStopped,
        eWaiting,
        eUnknown
    }

    public interface IViewWidgetMediaTime
    {
        void Open();
        void Close();

        void Initialised();

        void SetAllowSeeking(bool aAllowSeeking);
        void SetTransportState(ETransportState aTransportState);
        void SetDuration(uint aDuration);
        void SetSeconds(uint aSeconds);

        event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;
    }

    public interface IViewWidgetTransportControl
    {
        void Open();
        void Close();

        void Initialised();

        void SetPlayNowEnabled(bool aEnabled);
        void SetPlayNextEnabled(bool aEnabled);
        void SetPlayLaterEnabled(bool aEnabled);

        void SetDragging(bool aDragging);
        void SetTransportState(ETransportState aTransportState);
		void SetDuration(uint aDuration);
        void SetAllowSkipping(bool aAllowSkipping);
        void SetAllowPausing(bool aAllowPausing);

        event EventHandler<EventArgs> EventPause;
        event EventHandler<EventArgs> EventPlay;
        event EventHandler<EventArgs> EventStop;
        event EventHandler<EventArgs> EventPrevious;
        event EventHandler<EventArgs> EventNext;

        event EventHandler<EventArgsPlay> EventPlayNow;
        event EventHandler<EventArgsPlay> EventPlayNext;
        event EventHandler<EventArgsPlay> EventPlayLater;
    }

    public interface IViewWidgetTrack
    {
        void Open();
        void Close();

        void Initialised();
		
        void SetItem(upnpObject aObject);

        void SetMetatext(upnpObject aObject);

        void SetBitrate(uint aBitrate);
        void SetSampleRate(float aSampleRate);
        void SetBitDepth(uint aBitDepth);
        void SetCodec(string aCodec);
        void SetLossless(bool aLossless);

        void Update();
    }

    public interface IViewWidgetPlayMode
    {
        void Open();
        void Close();

        void Initialised();

        void SetShuffle(bool aShuffle);
        void SetRepeat(bool aRepeat);

        event EventHandler<EventArgs> EventToggleShuffle;
        event EventHandler<EventArgs> EventToggleRepeat;
    }

    public class EventArgsSeekTrack : EventArgs
    {
        public EventArgsSeekTrack(uint aIndex)
        {
            Index = aIndex;
        }

        public uint Index;
    }

    public class EventArgsPlaylistInsert : EventArgs
    {
        public EventArgsPlaylistInsert(uint aInsertAfterId, IMediaRetriever aRetriever)
        {
            InsertAfterId = aInsertAfterId;
            Retriever = aRetriever;
        }

        public uint InsertAfterId;
        public IMediaRetriever Retriever;
    }

    public class EventArgsPlaylistMove : EventArgs
    {
        public EventArgsPlaylistMove(uint aInsertAfterId, IList<MrItem> aPlaylistItems)
        {
            InsertAfterId = aInsertAfterId;
            PlaylistItems = aPlaylistItems;
        }

        public uint InsertAfterId;
        public IList<MrItem> PlaylistItems;
    }

    public class EventArgsPlaylistDelete : EventArgs
    {
        public EventArgsPlaylistDelete(IList<MrItem> aPlaylistItems)
        {
            PlaylistItems = aPlaylistItems;
        }

        public IList<MrItem> PlaylistItems;
    }

    public interface IViewWidgetPlaylist
    {
        void Open();
        void Close();

        void Initialised();

        void SetPlaylist(IList<MrItem> aPlaylist);
        void SetTrack(MrItem aTrack);
        
        void Save();
        void Delete();

        event EventHandler<EventArgsSeekTrack> EventSeekTrack;

        event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;
        event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;
        event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;

        event EventHandler<EventArgs> EventPlaylistDeleteAll;
    }

    public class EventArgsSetPreset : EventArgs
    {
        public EventArgsSetPreset(MrItem aPreset)
        {
            Preset = aPreset;
        }

        public MrItem Preset;
    }

    public class EventArgsSetChannel : EventArgs
    {
        public EventArgsSetChannel(IMediaRetriever aRetriever)
        {
            Retriever = aRetriever;
        }

        public IMediaRetriever Retriever;
    }

    public interface IViewWidgetPlaylistRadio
    {
        void Open();
        void Close();

        void Initialised();

        void SetPresets(IList<MrItem> aPresets);
        void SetChannel(Channel aChannel);
        void SetPreset(int aPresetIndex);

        void Save();

        event EventHandler<EventArgsSetPreset> EventSetPreset;
        event EventHandler<EventArgsSetChannel> EventSetChannel;
    }

    public interface IViewWidgetPlaylistReceiver
    {
        void Open();
        void Close();

        void Initialised();

        void SetSenders(IList<ModelSender> aSenders);
        void SetChannel(Channel aChannel);

        void Save();

        event EventHandler<EventArgsSetChannel> EventSetChannel;
    }
    
    public interface IViewWidgetPlaylistDiscPlayer
    {
        void Open();
        void Close();

        void Initialised();

        void Eject();
    }

    public interface IViewWidgetPlaylistAux
    {
        void Open(string aType);
        void Close();
    }

    public interface IViewWidgetLocation
    {
        void Open();
        void Close();

        void SetLocation(IList<string> aLocation);
    }

    public interface IViewWidgetContent
    {
        void Open();
        void Close();

        void OnSizeClick();
        void OnViewClick();

        void Focus();
    }

    public interface IViewWidgetReceivers
    {
        void Open();
        void Close();

        void SetSender(ModelSender aSender);
    }

} // Linn.Kinsky
