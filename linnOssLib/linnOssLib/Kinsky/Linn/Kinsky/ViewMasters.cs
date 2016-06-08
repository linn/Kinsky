using System;
using System.Threading;
using System.Collections.Generic;

using Linn.Topology;

using Upnp;

namespace Linn.Kinsky
{

    public class ViewMaster : IView
    {
        private ViewWidgetSelectorMaster<Room> iViewWidgetSelectorRoom;
        private ViewWidgetButtonMaster iViewWidgetButtonStandby;
        private ViewWidgetSelectorMaster<Source> iViewWidgetSelectorSource;
        private ViewWidgetVolumeControlMaster iViewWidgetVolumeControl;
        private ViewWidgetMediaTimeMaster iViewWidgetMediaTime;
        private ViewWidgetTransportControlMaster iViewWidgetTransportControlMediaRenderer;
        private ViewWidgetTransportControlMaster iViewWidgetTransportControlDiscPlayer;
        private ViewWidgetTransportControlMaster iViewWidgetTransportControlRadio;
        private ViewWidgetTrackMaster iViewWidgetTrack;
        private ViewWidgetPlayModeMaster iViewWidgetPlayMode;
        private ViewWidgetPlaylistMaster iViewWidgetPlaylist;
        private ViewWidgetPlaylistRadioMaster iViewWidgetPlaylistRadio;
        private ViewWidgetPlaylistReceiverMaster iViewWidgetPlaylistReceiver;
        private ViewWidgetPlaylistAuxMaster iViewWidgetPlaylistAux;
        private ViewWidgetPlaylistDiscPlayerMaster iViewWidgetPlaylistDiscPlayer;
        private ViewWidgetButtonMaster iViewWidgetButtonSave;
        private ViewWidgetButtonMaster iViewWidgetButtonWasteBin;
        private ViewWidgetReceiversMaster iViewWidgetReceivers;
        private ViewWidgetButtonMaster iViewWidgetButtonReceivers;

        public ViewMaster()
        {
            iViewWidgetSelectorRoom = new ViewWidgetSelectorMaster<Room>();
            iViewWidgetButtonStandby = new ViewWidgetButtonMaster();
            iViewWidgetSelectorSource = new ViewWidgetSelectorMaster<Source>();
            iViewWidgetVolumeControl = new ViewWidgetVolumeControlMaster();
            iViewWidgetMediaTime = new ViewWidgetMediaTimeMaster();
            iViewWidgetTransportControlMediaRenderer = new ViewWidgetTransportControlMaster();
            iViewWidgetTransportControlDiscPlayer = new ViewWidgetTransportControlMaster();
            iViewWidgetTransportControlRadio = new ViewWidgetTransportControlMaster();
            iViewWidgetTrack = new ViewWidgetTrackMaster();
            iViewWidgetPlayMode = new ViewWidgetPlayModeMaster();
            iViewWidgetPlaylist = new ViewWidgetPlaylistMaster();
            iViewWidgetPlaylistRadio = new ViewWidgetPlaylistRadioMaster();
            iViewWidgetPlaylistReceiver = new ViewWidgetPlaylistReceiverMaster();
            iViewWidgetPlaylistAux = new ViewWidgetPlaylistAuxMaster();
            iViewWidgetPlaylistDiscPlayer = new ViewWidgetPlaylistDiscPlayerMaster();
            iViewWidgetButtonSave = new ViewWidgetButtonMaster();
            iViewWidgetButtonWasteBin = new ViewWidgetButtonMaster();
            iViewWidgetReceivers = new ViewWidgetReceiversMaster();
            iViewWidgetButtonReceivers = new ViewWidgetButtonMaster();
        }

        #region IView Members

        IViewWidgetSelector<Room> IView.ViewWidgetSelectorRoom
        {
            get { return iViewWidgetSelectorRoom; }
        }

        public ViewWidgetSelectorMaster<Room> ViewWidgetSelectorRoom
        {
            get { return iViewWidgetSelectorRoom; }
        }

        IViewWidgetButton IView.ViewWidgetButtonStandby
        {
            get { return iViewWidgetButtonStandby; }
        }

        public ViewWidgetButtonMaster ViewWidgetButtonStandby
        {
            get { return iViewWidgetButtonStandby; }
        }

        IViewWidgetSelector<Source> IView.ViewWidgetSelectorSource
        {
            get { return iViewWidgetSelectorSource; }
        }

        public ViewWidgetSelectorMaster<Source> ViewWidgetSelectorSource
        {
            get { return iViewWidgetSelectorSource; }
        }

        IViewWidgetVolumeControl IView.ViewWidgetVolumeControl
        {
            get { return iViewWidgetVolumeControl; }
        }

        public ViewWidgetVolumeControlMaster ViewWidgetVolumeControl
        {
            get { return iViewWidgetVolumeControl; }
        }

        IViewWidgetMediaTime IView.ViewWidgetMediaTime
        {
            get { return iViewWidgetMediaTime; }
        }

        public ViewWidgetMediaTimeMaster ViewWidgetMediaTime
        {
            get { return iViewWidgetMediaTime; }
        }

        IViewWidgetTransportControl IView.ViewWidgetTransportControlMediaRenderer
        {
            get { return iViewWidgetTransportControlMediaRenderer; }
        }

        public ViewWidgetTransportControlMaster ViewWidgetTransportControlMediaRenderer
        {
            get { return iViewWidgetTransportControlMediaRenderer; }
        }

        IViewWidgetTransportControl IView.ViewWidgetTransportControlDiscPlayer
        {
            get { return iViewWidgetTransportControlDiscPlayer; }
        }

        public ViewWidgetTransportControlMaster ViewWidgetTransportControlDiscPlayer
        {
            get { return iViewWidgetTransportControlDiscPlayer; }
        }

        IViewWidgetTransportControl IView.ViewWidgetTransportControlRadio
        {
            get { return iViewWidgetTransportControlRadio; }
        }

        public ViewWidgetTransportControlMaster ViewWidgetTransportControlRadio
        {
            get { return iViewWidgetTransportControlRadio; }
        }

        IViewWidgetTrack IView.ViewWidgetTrack
        {
            get { return iViewWidgetTrack; }
        }

        public ViewWidgetTrackMaster ViewWidgetTrack
        {
            get { return iViewWidgetTrack; }
        }

        IViewWidgetPlayMode IView.ViewWidgetPlayMode
        {
            get { return iViewWidgetPlayMode; }
        }

        public ViewWidgetPlayModeMaster ViewWidgetPlayMode
        {
            get { return iViewWidgetPlayMode; }
        }

        IViewWidgetPlaylist IView.ViewWidgetPlaylist
        {
            get { return iViewWidgetPlaylist; }
        }

        public ViewWidgetPlaylistMaster ViewWidgetPlaylist
        {
            get { return iViewWidgetPlaylist; }
        }

        IViewWidgetPlaylistRadio IView.ViewWidgetPlaylistRadio
        {
            get { return iViewWidgetPlaylistRadio; }
        }

        public ViewWidgetPlaylistReceiverMaster ViewWidgetPlaylistReceiver
        {
            get { return iViewWidgetPlaylistReceiver; }
        }

        IViewWidgetPlaylistReceiver IView.ViewWidgetPlaylistReceiver
        {
            get { return iViewWidgetPlaylistReceiver; }
        }

        public ViewWidgetPlaylistRadioMaster ViewWidgetPlaylistRadio
        {
            get { return iViewWidgetPlaylistRadio; }
        }

        IViewWidgetPlaylistAux IView.ViewWidgetPlaylistAux
        {
            get { return iViewWidgetPlaylistAux; }
        }

        public ViewWidgetPlaylistAuxMaster ViewWidgetPlaylistAux
        {
            get { return iViewWidgetPlaylistAux; }
        }

        IViewWidgetPlaylistDiscPlayer IView.ViewWidgetPlaylistDiscPlayer
        {
            get { return iViewWidgetPlaylistDiscPlayer; }
        }

        public ViewWidgetPlaylistDiscPlayerMaster ViewWidgetPlaylistDiscPlayer
        {
            get { return iViewWidgetPlaylistDiscPlayer; }
        }

        IViewWidgetButton IView.ViewWidgetButtonSave
        {
            get { return iViewWidgetButtonSave; }
        }

        public ViewWidgetButtonMaster ViewWidgetButtonSave
        {
            get { return iViewWidgetButtonSave; }
        }

        IViewWidgetButton IView.ViewWidgetButtonWasteBin
        {
            get { return iViewWidgetButtonWasteBin; }
        }

        public ViewWidgetButtonMaster ViewWidgetButtonWasteBin
        {
            get { return iViewWidgetButtonWasteBin; }
        }

        IViewWidgetReceivers IView.ViewWidgetReceivers
        {
            get { return iViewWidgetReceivers; }
        }

        public ViewWidgetReceiversMaster ViewWidgetReceivers
        {
            get { return iViewWidgetReceivers; }
        }

        IViewWidgetButton IView.ViewWidgetButtonReceivers
        {
            get { return iViewWidgetButtonReceivers; }
        }

        public ViewWidgetButtonMaster ViewWidgetButtonReceivers
        {
            get { return iViewWidgetButtonReceivers; }
        }

        #endregion

    }

    public class ViewWidgetSelectorMaster<T> : IViewWidgetSelector<T>
    {
        private Mutex iMutex;
        private List<IViewWidgetSelector<T>> iViewWidgetSelectorList;

        public ViewWidgetSelectorMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetSelectorList = new List<IViewWidgetSelector<T>>();
        }

        public void Add(IViewWidgetSelector<T> aViewWidgetSelector)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetSelectorList.Add(aViewWidgetSelector);
                aViewWidgetSelector.EventSelectionChanged += OnEventSelectionChanged;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetSelector<T> aViewWidgetSelector)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetSelectorList.Remove(aViewWidgetSelector);
                aViewWidgetSelector.EventSelectionChanged -= OnEventSelectionChanged;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        #region IViewWidgetSelector Members

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetSelector<T> i in iViewWidgetSelectorList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetSelector<T> i in iViewWidgetSelectorList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void InsertItem(int aIndex, T aItem)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetSelector<T> i in iViewWidgetSelectorList)
                {
                    i.InsertItem(aIndex, aItem);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void RemoveItem(T aItem)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetSelector<T> i in iViewWidgetSelectorList)
                {
                    i.RemoveItem(aItem);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void ItemChanged(T aItem)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetSelector<T> i in iViewWidgetSelectorList)
                {
                    i.ItemChanged(aItem);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetSelected(T aItem)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetSelector<T> i in iViewWidgetSelectorList)
                {
                    i.SetSelected(aItem);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgsSelection<T>> EventSelectionChanged;

        #endregion

        private void OnEventSelectionChanged(object sender, EventArgsSelection<T> e)
        {
            if (EventSelectionChanged != null)
            {
                EventSelectionChanged(this, e);
            }
        }
    }

    public class ViewWidgetButtonMaster : IViewWidgetButton
    {
        private Mutex iMutex;
        private List<IViewWidgetButton> iViewWidgetButtonList;

        public ViewWidgetButtonMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetButtonList = new List<IViewWidgetButton>();
        }

        public void Add(IViewWidgetButton aViewWidgetButton)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetButtonList.Add(aViewWidgetButton);
                aViewWidgetButton.EventClick += OnEventClick;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetButton aViewWidgetButton)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetButtonList.Remove(aViewWidgetButton);
                aViewWidgetButton.EventClick -= OnEventClick;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }


        #region IViewWidgetButton Members


        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetButton i in iViewWidgetButtonList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetButton i in iViewWidgetButtonList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgs> EventClick;

        #endregion
        private void OnEventClick(object sender, EventArgs e)
        {
            if (EventClick != null)
            {
                EventClick(this, e);
            }
        }
    }

    public class ViewWidgetTrackMaster : IViewWidgetTrack
    {
        public ViewWidgetTrackMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetTrackList = new List<IViewWidgetTrack>();
        }

        public void Add(IViewWidgetTrack aViewWidgetTrack)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetTrackList.Add(aViewWidgetTrack);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetTrack aViewWidgetTrack)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetTrackList.Remove(aViewWidgetTrack);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetItem(upnpObject aObject)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetItem(aObject);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetMetatext(upnpObject aObject)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetMetatext(aObject);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetBitrate(uint aBitrate)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetBitrate(aBitrate);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetSampleRate(float aSampleRate)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetSampleRate(aSampleRate);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetBitDepth(uint aBitDepth)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetBitDepth(aBitDepth);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetCodec(string aCodec)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetCodec(aCodec);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetLossless(bool aLossless)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.SetLossless(aLossless);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Update()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTrack i in iViewWidgetTrackList)
                {
                    i.Update();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetTrack> iViewWidgetTrackList;
    }

    public class ViewWidgetMediaTimeMaster : IViewWidgetMediaTime
    {
        public ViewWidgetMediaTimeMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetMediaTimeList = new List<IViewWidgetMediaTime>();
        }

        public void Add(IViewWidgetMediaTime aViewWidgetMediaTime)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetMediaTimeList.Add(aViewWidgetMediaTime);

                aViewWidgetMediaTime.EventSeekSeconds += SeekSeconds;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetMediaTime aViewWidgetMediaTime)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetMediaTime.EventSeekSeconds -= SeekSeconds;

                iViewWidgetMediaTimeList.Remove(aViewWidgetMediaTime);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetAllowSeeking(bool aAllowSeeking)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.SetAllowSeeking(aAllowSeeking);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetTransportState(ETransportState aTransportState)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.SetTransportState(aTransportState);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetDuration(uint aDuration)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.SetDuration(aDuration);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetSeconds(uint aSeconds)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetMediaTime i in iViewWidgetMediaTimeList)
                {
                    i.SetSeconds(aSeconds);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgsSeekSeconds> EventSeekSeconds;

        private void SeekSeconds(object sender, EventArgsSeekSeconds e)
        {
            if (EventSeekSeconds != null)
            {
                EventSeekSeconds(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetMediaTime> iViewWidgetMediaTimeList;
    }

    public class ViewWidgetPlayModeMaster : IViewWidgetPlayMode
    {
        public ViewWidgetPlayModeMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetPlayMode = new List<IViewWidgetPlayMode>();
        }

        public void Add(IViewWidgetPlayMode aViewWidgetPlayMode)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlayMode.Add(aViewWidgetPlayMode);

                aViewWidgetPlayMode.EventToggleRepeat += ToggleRepeat;
                aViewWidgetPlayMode.EventToggleShuffle += ToggleShuffle;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetPlayMode aViewWidgetPlayMode)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetPlayMode.EventToggleRepeat -= ToggleRepeat;
                aViewWidgetPlayMode.EventToggleShuffle -= ToggleShuffle;

                iViewWidgetPlayMode.Remove(aViewWidgetPlayMode);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlayMode i in iViewWidgetPlayMode)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlayMode i in iViewWidgetPlayMode)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlayMode i in iViewWidgetPlayMode)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetShuffle(bool aShuffle)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlayMode i in iViewWidgetPlayMode)
                {
                    i.SetShuffle(aShuffle);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetRepeat(bool aRepeat)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlayMode i in iViewWidgetPlayMode)
                {
                    i.SetRepeat(aRepeat);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgs> EventToggleShuffle;
        public event EventHandler<EventArgs> EventToggleRepeat;

        private void ToggleRepeat(object sender, EventArgs e)
        {
            if (EventToggleRepeat != null)
            {
                EventToggleRepeat(this, e);
            }
        }

        private void ToggleShuffle(object sender, EventArgs e)
        {
            if (EventToggleShuffle != null)
            {
                EventToggleShuffle(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetPlayMode> iViewWidgetPlayMode;
    }

    public class ViewWidgetTransportControlMaster : IViewWidgetTransportControl
    {
        public ViewWidgetTransportControlMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetTransportControlList = new List<IViewWidgetTransportControl>();
        }

        public void Add(IViewWidgetTransportControl aViewWidgetTransportControl)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetTransportControlList.Add(aViewWidgetTransportControl);

                aViewWidgetTransportControl.EventNext += Next;
                aViewWidgetTransportControl.EventPause += Pause;
                aViewWidgetTransportControl.EventPlay += Play;
                aViewWidgetTransportControl.EventStop += Stop;
                aViewWidgetTransportControl.EventPlayLater += PlayLater;
                aViewWidgetTransportControl.EventPlayNext += PlayNext;
                aViewWidgetTransportControl.EventPlayNow += PlayNow;
                aViewWidgetTransportControl.EventPrevious += Previous;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetTransportControl aViewWidgetTransportControl)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetTransportControl.EventNext -= Next;
                aViewWidgetTransportControl.EventPause -= Pause;
                aViewWidgetTransportControl.EventPlay -= Play;
                aViewWidgetTransportControl.EventStop -= Stop;
                aViewWidgetTransportControl.EventPlayLater -= PlayLater;
                aViewWidgetTransportControl.EventPlayNext -= PlayNext;
                aViewWidgetTransportControl.EventPlayNow -= PlayNow;
                aViewWidgetTransportControl.EventPrevious -= Previous;

                iViewWidgetTransportControlList.Remove(aViewWidgetTransportControl);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetPlayNowEnabled(bool aEnabled)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetPlayNowEnabled(aEnabled);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetPlayNextEnabled(bool aEnabled)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetPlayNextEnabled(aEnabled);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetPlayLaterEnabled(bool aEnabled)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetPlayLaterEnabled(aEnabled);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetDragging(bool aDragging)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetDragging(aDragging);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetTransportState(ETransportState aTransportState)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetTransportState(aTransportState);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetDuration(uint aDuration)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetDuration(aDuration);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetAllowSkipping(bool aAllowSkipping)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetAllowSkipping(aAllowSkipping);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetAllowPausing(bool aAllowPausing)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetTransportControl i in iViewWidgetTransportControlList)
                {
                    i.SetAllowPausing(aAllowPausing);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
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

        private void Next(object sender, EventArgs e)
        {
            if (EventNext != null)
            {
                EventNext(this, e);
            }
        }

        private void Pause(object sender, EventArgs e)
        {
            if (EventPause != null)
            {
                EventPause(this, e);
            }
        }

        private void Play(object sender, EventArgs e)
        {
            if (EventPlay != null)
            {
                EventPlay(this, e);
            }
        }

        private void Stop(object sender, EventArgs e)
        {
            if (EventStop != null)
            {
                EventStop(this, e);
            }
        }

        private void PlayLater(object sender, EventArgsPlay e)
        {
            if (EventPlayLater != null)
            {
                EventPlayLater(this, e);
            }
        }

        private void PlayNext(object sender, EventArgsPlay e)
        {
            if (EventPlayNext != null)
            {
                EventPlayNext(this, e);
            }
        }

        private void PlayNow(object sender, EventArgsPlay e)
        {
            if (EventPlayNow != null)
            {
                EventPlayNow(this, e);
            }
        }

        private void Previous(object sender, EventArgs e)
        {
            if (EventPrevious != null)
            {
                EventPrevious(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetTransportControl> iViewWidgetTransportControlList;
    }

    public class ViewWidgetVolumeControlMaster : IViewWidgetVolumeControl
    {
        public ViewWidgetVolumeControlMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetVolumeControl = new List<IViewWidgetVolumeControl>();
        }

        public void Add(IViewWidgetVolumeControl aViewWidgetVolumeControl)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetVolumeControl.Add(aViewWidgetVolumeControl);

                aViewWidgetVolumeControl.EventMuteChanged += Mute;
                aViewWidgetVolumeControl.EventVolumeChanged += Volume;
                aViewWidgetVolumeControl.EventVolumeDecrement += VolumeDecrement;
                aViewWidgetVolumeControl.EventVolumeIncrement += VolumeIncrement;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetVolumeControl aViewWidgetVolumeControl)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetVolumeControl.EventMuteChanged -= Mute;
                aViewWidgetVolumeControl.EventVolumeChanged -= Volume;
                aViewWidgetVolumeControl.EventVolumeDecrement -= VolumeDecrement;
                aViewWidgetVolumeControl.EventVolumeIncrement -= VolumeIncrement;

                iViewWidgetVolumeControl.Remove(aViewWidgetVolumeControl);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetVolumeControl i in iViewWidgetVolumeControl)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetVolumeControl i in iViewWidgetVolumeControl)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetVolumeControl i in iViewWidgetVolumeControl)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetVolume(uint aVolume)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetVolumeControl i in iViewWidgetVolumeControl)
                {
                    i.SetVolume(aVolume);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetMute(bool aMute)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetVolumeControl i in iViewWidgetVolumeControl)
                {
                    i.SetMute(aMute);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetVolumeLimit(uint aVolumeLimit)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetVolumeControl i in iViewWidgetVolumeControl)
                {
                    i.SetVolumeLimit(aVolumeLimit);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgs> EventVolumeIncrement;
        public event EventHandler<EventArgs> EventVolumeDecrement;
        public event EventHandler<EventArgsVolume> EventVolumeChanged;
        public event EventHandler<EventArgsMute> EventMuteChanged;

        private void Mute(object sender, EventArgsMute e)
        {
            if (EventMuteChanged != null)
            {
                EventMuteChanged(this, e);
            }
        }

        private void Volume(object sender, EventArgsVolume e)
        {
            if (EventVolumeChanged != null)
            {
                EventVolumeChanged(this, e);
            }
        }

        private void VolumeDecrement(object sender, EventArgs e)
        {
            if (EventVolumeDecrement != null)
            {
                EventVolumeDecrement(this, e);
            }
        }

        private void VolumeIncrement(object sender, EventArgs e)
        {
            if (EventVolumeIncrement != null)
            {
                EventVolumeIncrement(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetVolumeControl> iViewWidgetVolumeControl;
    }

    public class ViewWidgetPlaylistMaster : IViewWidgetPlaylist
    {
        public ViewWidgetPlaylistMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetPlaylistList = new List<IViewWidgetPlaylist>();
        }

        public void Add(IViewWidgetPlaylist aViewWidgetPlaylist)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistList.Add(aViewWidgetPlaylist);

                aViewWidgetPlaylist.EventPlaylistDelete += PlaylistDelete;
                aViewWidgetPlaylist.EventPlaylistDeleteAll += PlaylistDeleteAll;
                aViewWidgetPlaylist.EventPlaylistInsert += PlaylistInsert;
                aViewWidgetPlaylist.EventPlaylistMove += PlaylistMove;
                aViewWidgetPlaylist.EventSeekTrack += SeekTrack;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetPlaylist aViewWidgetPlaylist)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetPlaylist.EventPlaylistDelete -= PlaylistDelete;
                aViewWidgetPlaylist.EventPlaylistDeleteAll -= PlaylistDeleteAll;
                aViewWidgetPlaylist.EventPlaylistInsert -= PlaylistInsert;
                aViewWidgetPlaylist.EventPlaylistMove -= PlaylistMove;
                aViewWidgetPlaylist.EventSeekTrack -= SeekTrack;

                iViewWidgetPlaylistList.Remove(aViewWidgetPlaylist);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetPlaylist(IList<MrItem> aPlaylist)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.SetPlaylist(aPlaylist);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetTrack(MrItem aTrack)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.SetTrack(aTrack);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Save()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.Save();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Delete()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylist i in iViewWidgetPlaylistList)
                {
                    i.Delete();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgsSeekTrack> EventSeekTrack;

        public event EventHandler<EventArgsPlaylistInsert> EventPlaylistInsert;
        public event EventHandler<EventArgsPlaylistMove> EventPlaylistMove;
        public event EventHandler<EventArgsPlaylistDelete> EventPlaylistDelete;

        public event EventHandler<EventArgs> EventPlaylistDeleteAll;

        private void SeekTrack(object sender, EventArgsSeekTrack e)
        {
            if (EventSeekTrack != null)
            {
                EventSeekTrack(this, e);
            }
        }

        private void PlaylistInsert(object sender, EventArgsPlaylistInsert e)
        {
            if (EventPlaylistInsert != null)
            {
                EventPlaylistInsert(this, e);
            }
        }

        private void PlaylistMove(object sender, EventArgsPlaylistMove e)
        {
            if (EventPlaylistMove != null)
            {
                EventPlaylistMove(this, e);
            }
        }

        private void PlaylistDelete(object sender, EventArgsPlaylistDelete e)
        {
            if (EventPlaylistDelete != null)
            {
                EventPlaylistDelete(this, e);
            }
        }

        private void PlaylistDeleteAll(object sender, EventArgs e)
        {
            if (EventPlaylistDeleteAll != null)
            {
                EventPlaylistDeleteAll(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetPlaylist> iViewWidgetPlaylistList;
    }

    public class ViewWidgetPlaylistRadioMaster : IViewWidgetPlaylistRadio
    {
        public ViewWidgetPlaylistRadioMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetPlaylistRadioList = new List<IViewWidgetPlaylistRadio>();
        }

        public void Add(IViewWidgetPlaylistRadio aViewWidgetPlaylistRadio)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistRadioList.Add(aViewWidgetPlaylistRadio);

                aViewWidgetPlaylistRadio.EventSetChannel += SetChannel;
                aViewWidgetPlaylistRadio.EventSetPreset += SetPreset;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetPlaylistRadio aViewWidgetPlaylistRadio)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetPlaylistRadio.EventSetChannel -= SetChannel;
                aViewWidgetPlaylistRadio.EventSetPreset -= SetPreset;

                iViewWidgetPlaylistRadioList.Remove(aViewWidgetPlaylistRadio);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetPresets(IList<MrItem> aPresets)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.SetPresets(aPresets);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetChannel(Channel aChannel)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.SetChannel(aChannel);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetPreset(int aPresetIndex)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.SetPreset(aPresetIndex);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Save()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistRadio i in iViewWidgetPlaylistRadioList)
                {
                    i.Save();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgsSetPreset> EventSetPreset;
        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        private void SetChannel(object sender, EventArgsSetChannel e)
        {
            if (EventSetChannel != null)
            {
                EventSetChannel(this, e);
            }
        }

        private void SetPreset(object sender, EventArgsSetPreset e)
        {
            if (EventSetPreset != null)
            {
                EventSetPreset(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetPlaylistRadio> iViewWidgetPlaylistRadioList;
    }

    public class ViewWidgetPlaylistReceiverMaster : IViewWidgetPlaylistReceiver
    {
        public ViewWidgetPlaylistReceiverMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetPlaylistReceiverList = new List<IViewWidgetPlaylistReceiver>();
        }

        public void Add(IViewWidgetPlaylistReceiver aViewWidgetPlaylistReceiver)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistReceiverList.Add(aViewWidgetPlaylistReceiver);

                aViewWidgetPlaylistReceiver.EventSetChannel += SetChannel;
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetPlaylistReceiver aViewWidgetPlaylistReceiver)
        {
            try
            {
                iMutex.WaitOne();

                aViewWidgetPlaylistReceiver.EventSetChannel -= SetChannel;

                iViewWidgetPlaylistReceiverList.Remove(aViewWidgetPlaylistReceiver);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistReceiver i in iViewWidgetPlaylistReceiverList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistReceiver i in iViewWidgetPlaylistReceiverList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistReceiver i in iViewWidgetPlaylistReceiverList)
                {
                    i.Initialised();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetSenders(IList<ModelSender> aSenders)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistReceiver i in iViewWidgetPlaylistReceiverList)
                {
                    i.SetSenders(aSenders);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetChannel(Channel aChannel)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistReceiver i in iViewWidgetPlaylistReceiverList)
                {
                    i.SetChannel(aChannel);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Save()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistReceiver i in iViewWidgetPlaylistReceiverList)
                {
                    i.Save();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public event EventHandler<EventArgsSetChannel> EventSetChannel;

        private void SetChannel(object sender, EventArgsSetChannel e)
        {
            if (EventSetChannel != null)
            {
                EventSetChannel(this, e);
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetPlaylistReceiver> iViewWidgetPlaylistReceiverList;
    }

    public class ViewWidgetPlaylistAuxMaster : IViewWidgetPlaylistAux
    {
        public ViewWidgetPlaylistAuxMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetPlaylistAuxList = new List<IViewWidgetPlaylistAux>();
        }

        public void Add(IViewWidgetPlaylistAux aViewWidgetPlaylistAux)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistAuxList.Add(aViewWidgetPlaylistAux);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetPlaylistAux aViewWidgetPlaylistAux)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistAuxList.Remove(aViewWidgetPlaylistAux);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Open(string aType)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistAux i in iViewWidgetPlaylistAuxList)
                {
                    i.Open(aType);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistAux i in iViewWidgetPlaylistAuxList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        private Mutex iMutex;
        private List<IViewWidgetPlaylistAux> iViewWidgetPlaylistAuxList;
    }

    public class ViewWidgetPlaylistDiscPlayerMaster : IViewWidgetPlaylistDiscPlayer
    {

        private Mutex iMutex;
        private List<IViewWidgetPlaylistDiscPlayer> iViewWidgetPlaylistDiscPlayerList;

        public ViewWidgetPlaylistDiscPlayerMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetPlaylistDiscPlayerList = new List<IViewWidgetPlaylistDiscPlayer>();
        }

        public void Add(IViewWidgetPlaylistDiscPlayer aViewWidgetPlaylistDiscPlayer)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistDiscPlayerList.Add(aViewWidgetPlaylistDiscPlayer);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetPlaylistDiscPlayer aViewWidgetPlaylistDiscPlayer)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetPlaylistDiscPlayerList.Remove(aViewWidgetPlaylistDiscPlayer);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        #region IViewWidgetPlaylistDiscPlayer Members

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistDiscPlayer i in iViewWidgetPlaylistDiscPlayerList)
                {
                    i.Open();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR Open: " + e.Message);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistDiscPlayer i in iViewWidgetPlaylistDiscPlayerList)
                {
                    i.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR Close: " + e.Message);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Initialised()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistDiscPlayer i in iViewWidgetPlaylistDiscPlayerList)
                {
                    i.Initialised();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR Close: " + e.Message);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Eject()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetPlaylistDiscPlayer i in iViewWidgetPlaylistDiscPlayerList)
                {
                    i.Eject();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR Close: " + e.Message);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        #endregion
    }

    public class ViewWidgetReceiversMaster : IViewWidgetReceivers
    {

        private Mutex iMutex;
        private List<IViewWidgetReceivers> iViewWidgetReceiversList;

        public ViewWidgetReceiversMaster()
        {
            iMutex = new Mutex(false);
            iViewWidgetReceiversList = new List<IViewWidgetReceivers>();
        }

        public void Add(IViewWidgetReceivers aViewWidgetReceivers)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetReceiversList.Add(aViewWidgetReceivers);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Remove(IViewWidgetReceivers aViewWidgetReceivers)
        {
            try
            {
                iMutex.WaitOne();

                iViewWidgetReceiversList.Remove(aViewWidgetReceivers);
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }


        #region IViewWidgetReceivers Members

        public void Open()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetReceivers i in iViewWidgetReceiversList)
                {
                    i.Open();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void Close()
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetReceivers i in iViewWidgetReceiversList)
                {
                    i.Close();
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        public void SetSender(ModelSender aSender)
        {
            try
            {
                iMutex.WaitOne();
                foreach (IViewWidgetReceivers i in iViewWidgetReceiversList)
                {
                    i.SetSender(aSender);
                }
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        #endregion
    }
}