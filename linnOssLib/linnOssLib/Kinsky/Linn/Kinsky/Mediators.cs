using System;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections.Generic;

using Linn;
using Linn.Control;
using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;
using Linn.Topology;
using Linn.Kinsky;
using Upnp;
using System.Collections.ObjectModel;

namespace Linn.Kinsky
{

    public interface IMediator
    {
        void Open();
        void Close();
        void SelectRoom(string aRoom);
    }

    public class Mediator : IMediator
    {
        public Mediator(HelperKinsky aHelper, IModel aModel)
        {
            iOpen = false;
            iInvoker = aHelper.Invoker;
            iMediatorRoom = new MediatorRoom(aHelper, aModel);
            iViewWidgetButtonSave = aModel.ModelWidgetButtonSave;
            iViewWidgetButtonWasteBin = aModel.ModelWidgetButtonWasteBin;
        }

        delegate void DOpen();
        public void Open()
        {
            Delegate del = new DOpen(delegate()
            {
                Assert.Check(!iOpen);
                iMediatorRoom.Open();
                iViewWidgetButtonSave.Open();
                iViewWidgetButtonWasteBin.Open();
                iOpen = true;
                Trace.WriteLine(Trace.kKinsky, "Mediator.Open() successful");
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        delegate void DClose();
        public void Close()
        {
            Delegate del = new DClose(delegate()
            {
                if (iOpen)
                {
                    iMediatorRoom.Close();
                    iViewWidgetButtonSave.Close();
                    iViewWidgetButtonWasteBin.Close();
                    iOpen = false;
                    Trace.WriteLine(Trace.kKinsky, "Mediator.Close() successful");
                }
                else
                {
                    Trace.WriteLine(Trace.kKinsky, "Mediator.Close() already closed - silently do nothing");
                }
            });
            if (iInvoker.TryBeginInvoke(del))
                return;
            del.Method.Invoke(del.Target, new object[] { });
        }

        delegate void DSelectRoom(string aRoom);
        public void SelectRoom(string aRoom)
        {
            Delegate del = new DSelectRoom(delegate(string r)
            {
                iMediatorRoom.SetOverrideStartupRoom(r);
                Trace.WriteLine(Trace.kKinsky, "Mediator.SelectRoom() successful");
            });
            if (iInvoker.TryBeginInvoke(del, aRoom))
                return;
            del.Method.Invoke(del.Target, new object[] { aRoom });
        }

        private bool iOpen;
        private MediatorRoom iMediatorRoom;

        private IViewWidgetButton iViewWidgetButtonSave;
        private IViewWidgetButton iViewWidgetButtonWasteBin;
        private IInvoker iInvoker;
    }

    internal class MediatorRoom
    {
        internal MediatorRoom(HelperKinsky aHelper, IModel aModel)
        {
            iInvoker = aHelper.Invoker;

            iStartupRoom = aHelper.StartupRoom;
            iLastSelectedRoom = aHelper.LastSelectedRoom;
            iHouse = aHelper.House;

            iMediatorSource = new MediatorSource(aHelper, aModel);

            iModelSelectorRoom = aModel.ModelWidgetSelectorRoom;
            iModelStandby = aModel.ModelWidgetButtonStandby;
            iViewWidgetTrack = aModel.ModelWidgetTrack;
            iViewWidgetTime = aModel.ModelWidgetMediaTime;
            iViewWidgetVolume = aModel.ModelWidgetVolumeControl;

            iOpen = false;
            iRooms = new List<IRoom>();

        }

        delegate void DAddExistingRooms();
        internal void AddExistingRooms()
        {
            int index = 0;
            foreach (Room room in iHouse.Rooms)
            {
                iRooms.Add(room);
                room.EventStandbyChanged += EventStandbyChanged;
                if (iOpen)
                {
                    iModelSelectorRoom.InsertItem(index++, room);
                }
            }
        }

        internal void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Assert.Check(!iOpen);
            iHouse.EventRoomInserted += EventRoomInserted;
            iHouse.EventRoomRemoved += EventRoomRemoved;

            iModelStandby.EventClick += EventStandbyClick;

            iModelSelectorRoom.EventSelectionChanged += EventSelectionChanged;
            iModelSelectorRoom.Open();

            iMediatorSource.Open(null);

            iAutoSelect = true;
            iOpen = true;

            AddExistingRooms();
        }

        internal void Close()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                iHouse.EventRoomInserted -= EventRoomInserted;
                iHouse.EventRoomRemoved -= EventRoomRemoved;
                iMediatorSource.Close();
                foreach (Room r in iRooms)
                {
                    r.EventStandbyChanged -= EventStandbyChanged;
                }
                iRooms.Clear();
                SelectRoom(null);
                iModelSelectorRoom.Close();
                iModelSelectorRoom.EventSelectionChanged -= EventSelectionChanged;

                iModelStandby.Close();
                iModelStandby.EventClick -= EventStandbyClick;
            }

            iOpen = false;
        }

        internal void SetOverrideStartupRoom(string aRoom)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }

            iOverrideAutoSelect = (aRoom != null);
            iOverrideStartupRoom = aRoom;
        }

        private void SelectRoom(IRoom aRoom)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                Trace.WriteLine(Trace.kKinsky, "MediatorRoom.SelectRoom: " + aRoom);
                if (iRoom != null)
                {
                    UserLog.WriteLine(DateTime.Now + ": Left " + iRoom);
                    iInfo.EventOpened -= EventInfoOpened;
                    iTime.EventOpened -= EventTimeOpened;
                    iVolume.EventOpened -= EventVolumeOpened;
                    iInfo.EventClosed -= EventInfoClosed;
                    iTime.EventClosed -= EventTimeClosed;
                    iVolume.EventClosed -= EventVolumeClosed;
                    iInfo.EventTrackChanged -= EventTrackChanged;
                    iInfo.EventMetatextChanged -= EventMetatextChanged;
                    iInfo.EventDetailsChanged -= EventDetailsChanged;
                    iTime.EventDurationChanged -= EventDurationChanged;
                    iTime.EventSecondsChanged -= EventSecondsChanged;
                    iVolume.EventVolumeLimitChanged -= EventVolumeLimitChanged;
                    iVolume.EventVolumeChanged -= EventVolumeChanged;
                    iVolume.EventMuteChanged -= EventMuteChanged;
                    iRoom.Close();
                    iVolume = null;
                    iInfo = null;
                    iTime = null;
                    iVolumeOpen = false;
                    iInfoOpen = false;
                    iTimeOpen = false;

                    iViewWidgetVolume.EventMuteChanged -= iViewWidgetVolume_EventMuteChanged;
                    iViewWidgetVolume.EventVolumeChanged -= iViewWidgetVolume_EventVolumeChanged;
                    iViewWidgetVolume.EventVolumeDecrement -= iViewWidgetVolume_EventVolumeDecrement;
                    iViewWidgetVolume.EventVolumeIncrement -= iViewWidgetVolume_EventVolumeIncrement;

                    iViewWidgetTrack.Close();
                    iViewWidgetTime.Close();
                    iViewWidgetVolume.Close();
                }
                iRoom = aRoom;
                if (iRoom != null)
                {
                    UserLog.WriteLine(DateTime.Now + ": Entered " + iRoom);
                    iVolume = iRoom.Volume;
                    iInfo = iRoom.Info;
                    iTime = iRoom.Time;

                    iInfo.EventOpened += EventInfoOpened;
                    iTime.EventOpened += EventTimeOpened;
                    iVolume.EventOpened += EventVolumeOpened;
                    iInfo.EventClosed += EventInfoClosed;
                    iTime.EventClosed += EventTimeClosed;
                    iVolume.EventClosed += EventVolumeClosed;
                    iInfo.EventTrackChanged += EventTrackChanged;
                    iInfo.EventMetatextChanged += EventMetatextChanged;
                    iInfo.EventDetailsChanged += EventDetailsChanged;
                    iTime.EventDurationChanged += EventDurationChanged;
                    iTime.EventSecondsChanged += EventSecondsChanged;
                    iVolume.EventVolumeLimitChanged += EventVolumeLimitChanged;
                    iVolume.EventVolumeChanged += EventVolumeChanged;
                    iVolume.EventMuteChanged += EventMuteChanged;

                    iTimeOpen = true;
                    iViewWidgetTime.Open();
                    iVolumeOpen = true;
                    iViewWidgetVolume.Open();
                    iInfoOpen = true;
                    iViewWidgetTrack.Open();

                    iViewWidgetVolume.EventMuteChanged += iViewWidgetVolume_EventMuteChanged;
                    iViewWidgetVolume.EventVolumeChanged += iViewWidgetVolume_EventVolumeChanged;
                    iViewWidgetVolume.EventVolumeDecrement += iViewWidgetVolume_EventVolumeDecrement;
                    iViewWidgetVolume.EventVolumeIncrement += iViewWidgetVolume_EventVolumeIncrement;

                    iRoom.Open();


                    iMediatorSource.Close();
                    iModelStandby.Close();
                    iRoom.Standby = false;
                    iModelStandby.Open();
                    iLastRoom = iRoom.Name;
                    iLastSelectedRoom.Set(iRoom.Name);
                    iMediatorSource.Open(iRoom);
                }
                else
                {
                    iMediatorSource.Close();
                    iModelStandby.Close();
                }
                iAutoSelect = false;
                iModelSelectorRoom.SetSelected(iRoom as Room);
            }
        }

        void iViewWidgetVolume_EventVolumeIncrement(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolumeOpen)
            {
                iVolume.IncrementVolume();
            }
        }

        void iViewWidgetVolume_EventVolumeDecrement(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolumeOpen)
            {
                iVolume.DecrementVolume();
            }
        }

        void iViewWidgetVolume_EventVolumeChanged(object sender, EventArgsVolume e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolumeOpen)
            {
                iVolume.Volume = e.Volume;
            }
        }

        void iViewWidgetVolume_EventMuteChanged(object sender, EventArgsMute e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolumeOpen)
            {
                iVolume.Mute = e.Mute;
            }
        }
        private void EventInfoOpened(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iInfo == sender)
            {
                if (!iInfoOpen)
                {
                    iInfoOpen = true;
                    iViewWidgetTrack.Open();
                    iViewWidgetTrack.SetBitrate(iInfo.Bitrate);
                    iViewWidgetTrack.SetSampleRate(iInfo.SampleRate);
                    iViewWidgetTrack.SetBitDepth(iInfo.BitDepth);
                    iViewWidgetTrack.SetCodec(iInfo.Codec);
                    iViewWidgetTrack.SetLossless(iInfo.Lossless);
                    iViewWidgetTrack.SetItem(iInfo.Track);
                    iViewWidgetTrack.SetMetatext(iInfo.Metatext);
                }
                iViewWidgetTrack.Initialised();
            }
        }
        private void EventInfoClosed(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iInfo == sender)
            {
                iInfoOpen = false;
                iViewWidgetTrack.Close();
            }
        }
        private void EventVolumeOpened(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolume == sender)
            {
                if (!iVolumeOpen)
                {
                    iVolumeOpen = true;
                    iViewWidgetVolume.Open();
                    iViewWidgetVolume.SetMute(iVolume.Mute);
                    iViewWidgetVolume.SetVolume(iVolume.Volume);
                    iViewWidgetVolume.SetVolumeLimit(iVolume.VolumeLimit);
                }
                iViewWidgetVolume.Initialised();
            }
        }
        private void EventVolumeClosed(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolume == sender)
            {
                iVolumeOpen = false;
                iViewWidgetVolume.Close();
            }
        }

        private void EventTimeOpened(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iTime == sender)
            {
                if (!iTimeOpen)
                {
                    iTimeOpen = true;
                    Assert.Check(iTime != null, "iTime != null");
                    Assert.Check(iViewWidgetTime != null, "iViewWidgetTime != null");
                    Assert.Check(iRoom != null, "iRoom != null");
                    iViewWidgetTime.Open();
                    iViewWidgetTime.SetDuration(iTime.Duration);
                    iViewWidgetTime.SetSeconds(iTime.Seconds);
                    ETransportState transportState = ETransportState.eStopped;
                    if (iRoom.Current != null)
                    {
                        transportState = iRoom.Current.TransportState;
                    }
                    iViewWidgetTime.SetTransportState(transportState);
                }
                iViewWidgetTime.Initialised();
            }
        }
        private void EventTimeClosed(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iTime == sender)
            {
                iTimeOpen = false;
                iViewWidgetTime.Close();
            }
        }
        private void EventTrackChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iInfo == sender)
            {
                iViewWidgetTrack.SetItem(iInfo.Track);
                iViewWidgetTrack.Update();
            }
        }
        private void EventMetatextChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iInfo == sender)
            {
                iViewWidgetTrack.SetMetatext(iInfo.Metatext);
                iViewWidgetTrack.Update();
            }
        }
        private void EventDetailsChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iInfo == sender)
            {
                iViewWidgetTrack.SetBitrate(iInfo.Bitrate);
                iViewWidgetTrack.SetSampleRate(iInfo.SampleRate);
                iViewWidgetTrack.SetBitDepth(iInfo.BitDepth);
                iViewWidgetTrack.SetCodec(iInfo.Codec);
                iViewWidgetTrack.SetLossless(iInfo.Lossless);
                iViewWidgetTrack.Update();
            }
        }
        private void EventDurationChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iTime == sender)
            {
                iViewWidgetTime.SetDuration(iTime.Duration);
            }
        }
        private void EventSecondsChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iTime == sender)
            {
                iViewWidgetTime.SetSeconds(iTime.Seconds);
            }
        }
        private void EventVolumeLimitChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolume == sender)
            {
                iViewWidgetVolume.SetVolumeLimit(iVolume.VolumeLimit);
            }
        }
        private void EventVolumeChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolume == sender)
            {
                iViewWidgetVolume.SetVolume(iVolume.Volume);
            }
        }

        private void EventMuteChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen && iVolume == sender)
            {
                iViewWidgetVolume.SetMute(iVolume.Mute);
            }
        }

        private void EventStandbyChanged(object sender, EventArgs args)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                if (iRoom == sender)
                {
                    Trace.WriteLine(Trace.kKinsky, "MediatorRoom.EventStandbyChanged");
                    if (iRoom.Standby)
                    {
                        iLastRoom = string.Empty;
                        SelectRoom(null);
                    }
                }
                iModelSelectorRoom.ItemChanged(sender as Room);
            }
        }

        private void EventRoomInserted(object sender, EventArgsItemInsert<IRoom> e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Trace.WriteLine(Trace.kKinsky, "MediatorRoom.EventRoomInserted: " + e.Item);
            if (!iRooms.Contains(e.Item))
            {
                iRooms.Insert(e.Index, e.Item);
                if (iOpen)
                {
                    InsertModelRoom(e.Index, e.Item as Room);
                }
                e.Item.EventStandbyChanged += EventStandbyChanged;
            }
        }

        private void InsertModelRoom(int aIndex, Room aRoom)
        {
            iModelSelectorRoom.InsertItem(aIndex, aRoom);

            if (iModelSelectorRoom.Selected == null)
            {
                if (iAutoSelect)
                {
                    if(iOverrideAutoSelect)
                    {
                        if(iOverrideStartupRoom == aRoom.Name)
                        {
                            iAutoSelect = false;
                            SelectRoom(aRoom);
                            iModelSelectorRoom.SetSelected(aRoom);
                        }
                    }
                    else if (iStartupRoom.Value == aRoom.Name || (iStartupRoom.Value == OptionStartupRoom.kLastSelected && (iLastSelectedRoom.Value == aRoom.Name || iLastSelectedRoom.Value == string.Empty)))
                    {
                        iAutoSelect = false;
                        SelectRoom(aRoom);
                        iModelSelectorRoom.SetSelected(aRoom);
                    }
                }
                else if (iLastRoom == aRoom.Name)
                {
                    SelectRoom(aRoom);
                    iModelSelectorRoom.SetSelected(aRoom);
                }
            }
        }

        private void EventRoomRemoved(object sender, EventArgsItem<IRoom> e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Trace.WriteLine(Trace.kKinsky, "MediatorRoom.EventRoomRemoved: " + e.Item);
            if (iRooms.Contains(e.Item))
            {
                iRooms.Remove(e.Item);
                if (iOpen)
                {
                    iModelSelectorRoom.RemoveItem(e.Item as Room);
                    if (e.Item == iRoom)
                    {
                        SelectRoom(null);
                    }
                }
                e.Item.EventStandbyChanged -= EventStandbyChanged;
            }
        }

        private void EventSelectionChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                IRoom room = iModelSelectorRoom.Selected;
                Trace.WriteLine(Trace.kKinsky, "MediatorRoom.EventSelectionChanged: " + room);
                SelectRoom(room);
            }
        }

        private void EventStandbyClick(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                Trace.WriteLine(Trace.kKinsky, "MediatorRoom.EventStandbyClick: ");
                if (iRoom != null)
                {
                    iRoom.Standby = true;
                }
            }
        }

        private bool iOpen;

        private bool iAutoSelect;
        private bool iOverrideAutoSelect;
        private string iOverrideStartupRoom;
        private OptionStartupRoom iStartupRoom;
        private OptionString iLastSelectedRoom;

        private IHouse iHouse;
        private IViewWidgetButton iModelStandby;
        private IModelWidgetSelector<Room> iModelSelectorRoom;
        private IViewWidgetTrack iViewWidgetTrack;
        private IViewWidgetMediaTime iViewWidgetTime;
        private IViewWidgetVolumeControl iViewWidgetVolume;

        private IRoom iRoom;
        private IRoomVolume iVolume;
        private IRoomTime iTime;
        private IRoomInfo iInfo;
        private string iLastRoom;

        private MediatorSource iMediatorSource;
        private List<IRoom> iRooms;
        private IInvoker iInvoker;
        private bool iVolumeOpen;
        private bool iTimeOpen;
        private bool iInfoOpen;
    }

    internal class MediatorSource
    {
        public MediatorSource(HelperKinsky aHelper, IModel aModel)
        {
            iInvoker = aHelper.Invoker;
            iModel = aModel;
            iSources = new List<ISource>();
            iModelSelectorSource = aModel.ModelWidgetSelectorSource;

            iModelSenders = aHelper.Senders;

            iViewButtonReceivers = aModel.ModelWidgetButtonReceivers;
            iViewWidgetReceivers = aModel.ModelWidgetReceivers;

            iModelSelectorSource.EventSelectionChanged += EventSelectionChanged;

            iOpen = false;
        }

        public void Open(IRoom aRoom)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Trace.WriteLine(Trace.kKinsky, "MediatorSource.Open: Room(" + aRoom + ")");

            Assert.Check(!iOpen);

            iModelSelectorSource.Open();

            iRoom = aRoom;

            iViewButtonReceivers.EventClick += EventReceiversClick;

            iOpen = true;

            if (iRoom != null)
            {
                iRoom.Standby = false;
                iRoom.EventCurrentChanged += EventCurrentChanged;
                iRoom.EventSourceInserted += EventSourceInserted;
                iRoom.EventSourceRemoved += EventSourceRemoved;

                ReadOnlyCollection<ISource> sources = iRoom.Sources;
                for (int i = 0; i < sources.Count; i++)
                {
                    Trace.WriteLine(Trace.kKinsky, "Manual source insert: " + i + sources[i].Name);
                    EventSourceInserted(this, new EventArgsItemInsert<ISource>(i, sources[i]));
                }
                SelectSource(iRoom.Current);
            }
        }

        void EventSourceInserted(object sender, EventArgsItemInsert<ISource> e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (!iSources.Contains(e.Item))
            {
                iSources.Insert(e.Index, e.Item);
            }
            if (iOpen)
            {
                Trace.WriteLine(Trace.kKinsky, "MediatorSource.EventSourceInserted: " + e.Index + e.Item);

                iModelSelectorSource.InsertItem(e.Index, e.Item as Source);
                if (iModelSelectorSource.Selected == null && e.Item == iRoom.Current)
                {
                    iModelSelectorSource.SetSelected(e.Item as Source);
                }
                if (e.Item is Source)
                {
                    (e.Item as Source).Standby = false;
                }
            }
            e.Item.EventNameChanged += SourceNameChanged;
        }

        void EventSourceRemoved(object sender, EventArgsItem<ISource> e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }

            iSources.Remove(e.Item);
            Trace.WriteLine(Trace.kKinsky, "MediatorSource.EventSourceRemoved: " + e.Item);
            iModelSelectorSource.RemoveItem(e.Item as Source);
            e.Item.EventNameChanged -= SourceNameChanged;
        }

        void SourceNameChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }

            Trace.WriteLine(Trace.kKinsky, "MediatorSource.SourceNameChanged: " + sender);
            iModelSelectorSource.ItemChanged(sender as Source);
        }

        public void Close()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Trace.WriteLine(Trace.kKinsky, "MediatorSource.Close()");

            if (iOpen)
            {
                iModelSelectorSource.Close();

                iViewButtonReceivers.Close();
                iViewButtonReceivers.EventClick -= EventReceiversClick;

                if (iRoom != null)
                {
                    iRoom.EventSourceInserted -= EventSourceInserted;
                    iRoom.EventSourceRemoved -= EventSourceRemoved;
                    iRoom.EventCurrentChanged -= EventCurrentChanged;

                    ReadOnlyCollection<ISource> sources = iRoom.Sources;
                    for (int i = 0; i < sources.Count; i++)
                    {
                        Trace.WriteLine(Trace.kKinsky, "Manual source remove: " + i + sources[i].Name);
                        EventSourceRemoved(this, new EventArgsItem<ISource>(sources[i]));
                    }
                }

                SelectSource(null);
            }


            iOpen = false;
        }

        public void SelectSource(ISource aSource)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iOpen)
            {
                // update the view for the receivers button
                iViewButtonReceivers.Close();
                iCurrentSender = null;

                if (iSource != null)
                {
                    iMediatorPlaylist.Close();
                    iMediatorPlayMode.Close();
                    iMediatorTransport.Close();

                    iSource.EventOpened -= EventOpened;
                    iSource.EventClosed -= EventClosed;
                }

                iSource = aSource;

                if (iSource != null)
                {
                    foreach (ModelSender s in iModelSenders.SendersList)
                    {
                        if (s.Udn == iSource.Udn)
                        {
                            iViewButtonReceivers.Open();
                            iCurrentSender = s;
                        }
                    }

                    iMediatorPlaylist = new MediatorPlaylist(iSource, iModel, iInvoker);
                    iMediatorPlaylist.Open();
                    iMediatorTransport = new MediatorTransport(iSource, iModel, iInvoker);
                    iMediatorTransport.Open();
                    iMediatorPlayMode = new MediatorPlayMode(iSource, iModel, iInvoker);
                    iMediatorPlayMode.Open();

                    iSource.EventOpened += EventOpened;
                    iSource.EventClosed += EventClosed;
                }

                // set the selected source in the view
                UserLog.WriteLine(DateTime.Now + ": Selected " + iSource);
                Trace.WriteLine(Trace.kKinsky, "MediatorSource.SelectSource: " + iSource);

                iModelSelectorSource.SetSelected(iSource as Source);
            }
        }

        void EventOpened(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iMediatorTransport.Initialised();
            iMediatorPlaylist.Initialised();
        }

        void EventClosed(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iMediatorPlayMode.Close();
            iMediatorTransport.Close();
            iMediatorPlaylist.Close();
        }

        private void EventSelectionChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Trace.WriteLine(Trace.kKinsky, "MediatorSource.EventSelectionChanged()");
            if (iModelSelectorSource.Selected != null)
            {
                iModelSelectorSource.Selected.Select();
            }
        }

        private void EventReceiversClick(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetReceivers.SetSender(iCurrentSender);
            iViewWidgetReceivers.Open();
        }

        private void EventCurrentChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            Trace.WriteLine(Trace.kKinsky, "MediatorSource.CurrentChanged()");
            SelectSource(iRoom.Current);
        }


        private List<ISource> iSources;
        private bool iOpen;
        private IRoom iRoom;

        private IModelWidgetSelector<Source> iModelSelectorSource;

        private IViewWidgetButton iViewButtonReceivers;
        private IViewWidgetReceivers iViewWidgetReceivers;

        private MediatorTransport iMediatorTransport;
        private MediatorPlayMode iMediatorPlayMode;
        private MediatorPlaylist iMediatorPlaylist;

        private ModelSenders iModelSenders;
        private ModelSender iCurrentSender;
        private IInvoker iInvoker;
        private ISource iSource;
        private IModel iModel;
    }

    internal class MediatorPlaylist
    {
        public MediatorPlaylist(ISource aSource, IModel aModel, IInvoker aInvoker)
        {
            iSource = aSource;
            iInvoker = aInvoker;
            iViewWidgetPlaylist = aModel.ModelWidgetPlaylist;
            iViewWidgetPlaylistAux = aModel.ModelWidgetPlaylistAux;
            iViewWidgetPlaylistDiscPlayer = aModel.ModelWidgetPlaylistDiscPlayer;
            iViewWidgetPlaylistRadio = aModel.ModelWidgetPlaylistRadio;
            iViewWidgetPlaylistReceiver = aModel.ModelWidgetPlaylistReceiver;
            iViewButtonSave = aModel.ModelWidgetButtonSave;
            iViewButtonWasteBin = aModel.ModelWidgetButtonWasteBin;
            iPlaylistSupport = aModel.ModelSupport;
            if (iSource is IPlaylistSource)
            {
                iViewWidgetTransportControl = aModel.ModelWidgetTransportControlMediaRenderer;
            }
            else if (iSource is IRadioSource || iSource is IReceiverSource)
            {
                iViewWidgetTransportControl = aModel.ModelWidgetTransportControlRadio;
            }
            else if (iSource is IDiscSource)
            {
                iViewWidgetTransportControl = aModel.ModelWidgetTransportControlDiscPlayer;
            }
        }

        public void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IPlaylistSource)
            {
                IPlaylistSource source = iSource as IPlaylistSource;
                source.EventItemsChanged += EventItemsChanged;
                source.EventCurrentChanged += EventCurrentChanged;

                iViewButtonSave.EventClick += ButtonSave_EventClick;
                iViewButtonWasteBin.EventClick += ButtonWasteBin_EventClick;
                iViewWidgetPlaylist.EventPlaylistDelete += EventPlaylistDelete;
                iViewWidgetPlaylist.EventPlaylistDeleteAll += EventPlaylistDeleteAll;
                iViewWidgetPlaylist.EventPlaylistInsert += EventPlaylistInsert;
                iViewWidgetPlaylist.EventPlaylistMove += EventPlaylistMove;
                iViewWidgetPlaylist.EventSeekTrack += EventSeekTrack;
                iViewWidgetPlaylist.SetPlaylist(new List<MrItem>(source.Items));
                iViewWidgetPlaylist.SetTrack(source.Current);
                iViewWidgetPlaylist.Open();

                iPlaylistSupport.EventIsDraggingChanged += EventIsDraggingChanged;
                iPlaylistSupport.EventPlayNow += EventPlayNow;
                iPlaylistSupport.EventPlayNext += EventPlayNext;
                iPlaylistSupport.EventPlayLater += EventPlayLater;
                iPlaylistSupport.EventPlayInsert += EventPlayInsert;
                iPlaylistSupport.EventMove += EventMove;
                iPlaylistSupport.SetInsertAllowed(true);
                iPlaylistSupport.Open();
            }
            else if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                source.EventItemsChanged += EventItemsChanged;
                source.EventCurrentChanged += EventCurrentChanged;
                source.EventChannelChanged += EventChannelChanged;
                iViewWidgetPlaylistRadio.EventSetChannel += EventSetChannel;
                iViewWidgetPlaylistRadio.EventSetPreset += EventSetPreset;
                iViewWidgetPlaylistRadio.Open();
                iViewWidgetPlaylistRadio.SetPresets(new List<MrItem>(source.Items));
                iViewWidgetPlaylistRadio.SetPreset(source.Items.IndexOf(source.Current));

                iPlaylistSupport.EventIsDraggingChanged += EventIsDraggingChanged;
                iPlaylistSupport.SetInsertAllowed(false);
                iPlaylistSupport.Open();
            }
            else if (iSource is IReceiverSource)
            {
                IReceiverSource source = iSource as IReceiverSource;
                source.EventItemsChanged += EventItemsChanged;
                source.EventChannelChanged += EventChannelChanged;
                iViewWidgetPlaylistReceiver.EventSetChannel += EventSetChannel;
                iViewWidgetPlaylistReceiver.Open();

                iPlaylistSupport.EventIsDraggingChanged += EventIsDraggingChanged;
                iPlaylistSupport.SetInsertAllowed(false);
                iPlaylistSupport.Open();
            }
            else if (iSource is IDiscSource)
            {
                iViewWidgetPlaylistDiscPlayer.Open();
            }
            else
            {
                iViewWidgetPlaylistAux.Open(iSource.Type);
            }
        }

        public void Initialised()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IPlaylistSource)
            {
                iViewWidgetPlaylist.Initialised();
            }
            else if (iSource is IRadioSource)
            {
                iViewWidgetPlaylistRadio.Initialised();
            }
            else if (iSource is IReceiverSource)
            {
                iViewWidgetPlaylistReceiver.Initialised();
            }
            else if (iSource is IDiscSource)
            {
                iViewWidgetPlaylistDiscPlayer.Initialised();
            }
        }

        public void Close()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }

            if (iSource is IPlaylistSource)
            {
                IPlaylistSource source = iSource as IPlaylistSource;
                source.EventItemsChanged -= EventItemsChanged;
                source.EventCurrentChanged -= EventCurrentChanged;
                iViewButtonSave.EventClick -= ButtonSave_EventClick;
                iViewButtonWasteBin.EventClick -= ButtonWasteBin_EventClick;
                iViewWidgetPlaylist.EventPlaylistDelete -= EventPlaylistDelete;
                iViewWidgetPlaylist.EventPlaylistDeleteAll -= EventPlaylistDeleteAll;
                iViewWidgetPlaylist.EventPlaylistInsert -= EventPlaylistInsert;
                iViewWidgetPlaylist.EventPlaylistMove -= EventPlaylistMove;
                iViewWidgetPlaylist.EventSeekTrack -= EventSeekTrack;
                iViewWidgetPlaylist.Close();

                iPlaylistSupport.EventIsDraggingChanged -= EventIsDraggingChanged;
                iPlaylistSupport.EventPlayNow -= EventPlayNow;
                iPlaylistSupport.EventPlayNext -= EventPlayNext;
                iPlaylistSupport.EventPlayLater -= EventPlayLater;
                iPlaylistSupport.EventPlayInsert -= EventPlayInsert;
                iPlaylistSupport.EventMove -= EventMove;
                if (iPlaylistSupport.IsOpen())
                {
                    iPlaylistSupport.Close();
                }
            }
            else if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                source.EventItemsChanged -= EventItemsChanged;
                source.EventCurrentChanged -= EventCurrentChanged;
                source.EventChannelChanged -= EventChannelChanged;
                iViewWidgetPlaylistRadio.EventSetChannel -= EventSetChannel;
                iViewWidgetPlaylistRadio.EventSetPreset -= EventSetPreset;
                iViewWidgetPlaylistRadio.Close();

                iPlaylistSupport.EventIsDraggingChanged -= EventIsDraggingChanged;
                if (iPlaylistSupport.IsOpen())
                {
                    iPlaylistSupport.Close();
                }
            }
            else if (iSource is IReceiverSource)
            {
                IReceiverSource source = iSource as IReceiverSource;
                source.EventItemsChanged -= EventItemsChanged;
                source.EventChannelChanged -= EventChannelChanged;
                iViewWidgetPlaylistReceiver.EventSetChannel -= EventSetChannel;
                iViewWidgetPlaylistReceiver.Close();

                iPlaylistSupport.EventIsDraggingChanged -= EventIsDraggingChanged;
                if (iPlaylistSupport.IsOpen())
                {
                    iPlaylistSupport.Close();
                }
            }
            else if (iSource is IDiscSource)
            {
                iViewWidgetPlaylistDiscPlayer.Close();
            }
            else
            {
                iViewWidgetPlaylistAux.Close();
            }
        }

        private delegate void DEventMove(object sender, EventArgsMove e);
        void EventMove(object sender, EventArgsMove e)
        {
            Delegate del = new DEventMove(delegate(object s, EventArgsMove args)
            {
                if (iSource is IPlaylistSource)
                {
                    (iSource as IPlaylistSource).Move(e.InsertAfterId, e.MoveItems, e.Callback);
                }
            });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private delegate void DEventPlayInsert(object sender, EventArgsInsert e);
        void EventPlayInsert(object sender, EventArgsInsert e)
        {
            Delegate del = new DEventPlayInsert(delegate(object s, EventArgsInsert args)
            {
                if (iSource is IPlaylistSource)
                {
                    (iSource as IPlaylistSource).Insert(e.InsertAfterId, e.Retriever, e.Callback);
                }
            });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private delegate void DEventPlayNow(object sender, EventArgsPlay e);
        void EventPlayNow(object sender, EventArgsPlay e)
        {
            Delegate del = new DEventPlayNow(delegate(object s, EventArgsPlay args)
            {
                iSource.PlayNow(e.Retriever, e.Callback);
            });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private delegate void DEventPlayNext(object sender, EventArgsPlay e);
        void EventPlayNext(object sender, EventArgsPlay e)
        {
            Delegate del = new DEventPlayNext(delegate(object s, EventArgsPlay args)
            {
                iSource.PlayNext(e.Retriever, e.Callback);
            });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        private delegate void DEventPlayLater(object sender, EventArgsPlay e);
        void EventPlayLater(object sender, EventArgsPlay e)
        {
            Delegate del = new DEventPlayLater(delegate(object s, EventArgsPlay args)
            {
                iSource.PlayLater(e.Retriever, e.Callback);
            });
            if (iInvoker.TryBeginInvoke(del, sender, e))
                return;
            del.Method.Invoke(del.Target, new object[] { sender, e });
        }

        void EventIsDraggingChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetTransportControl.SetDragging(iPlaylistSupport.IsDragging());
        }

        void EventCurrentChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IPlaylistSource)
            {
                IPlaylistSource source = iSource as IPlaylistSource;
                iViewWidgetPlaylist.SetTrack(source.Current);
            }
            else if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                iViewWidgetPlaylistRadio.SetPreset(source.Items.IndexOf(source.Current));
            }
        }

        void EventChannelChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                iViewWidgetPlaylistRadio.SetChannel(source.Channel);
            }
            else if (iSource is IReceiverSource)
            {
                IReceiverSource source = iSource as IReceiverSource;
                iViewWidgetPlaylistReceiver.SetChannel(source.Channel);
            }
        }

        void EventSetChannel(object sender, EventArgsSetChannel e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                source.SetChannel(e.Retriever.Media);
                source.Play();
            }
            else if (iSource is IReceiverSource)
            {
                IReceiverSource source = iSource as IReceiverSource;
                source.SetChannel(e.Retriever.Media);
                source.Play();
            }
        }

        void EventSetPreset(object sender, EventArgsSetPreset e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                //source.
                source.Current = e.Preset;
                source.Play();
            }
        }

        void EventItemsChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource is IPlaylistSource)
            {
                IPlaylistSource source = iSource as IPlaylistSource;
                iViewWidgetPlaylist.SetPlaylist(new List<MrItem>(source.Items));
            }
            else if (iSource is IRadioSource)
            {
                IRadioSource source = iSource as IRadioSource;
                iViewWidgetPlaylistRadio.SetPresets(new List<MrItem>(source.Items));
            }
            else if (iSource is IReceiverSource)
            {
                IReceiverSource source = iSource as IReceiverSource;
                List<ModelSender> senders = new List<ModelSender>();
                ReadOnlyCollection<IModelSender> items = source.Items;
                foreach (IModelSender s in items)
                {
                    senders.Add(s as ModelSender);
                }
                iViewWidgetPlaylistReceiver.SetSenders(senders);
            }

        }

        void EventPlaylistDelete(object sender, EventArgsPlaylistDelete e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            (iSource as IPlaylistSource).Delete(e.PlaylistItems);
        }

        void EventPlaylistDeleteAll(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            (iSource as IPlaylistSource).DeleteAll();
        }

        void EventPlaylistInsert(object sender, EventArgsPlaylistInsert e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iPlaylistSupport.PlayInsert(e.InsertAfterId, e.Retriever);
        }

        void EventPlaylistMove(object sender, EventArgsPlaylistMove e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iPlaylistSupport.Move(e.InsertAfterId, e.PlaylistItems);
        }

        void EventSeekTrack(object sender, EventArgsSeekTrack e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            IPlaylistSource source = iSource as IPlaylistSource;
            source.Current = source.Items[(int)e.Index];
        }

        void ButtonSave_EventClick(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetPlaylist.Save();
        }

        void ButtonWasteBin_EventClick(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetPlaylist.Delete();
        }

        private IViewWidgetPlaylist iViewWidgetPlaylist;
        private IViewWidgetPlaylistAux iViewWidgetPlaylistAux;
        private IViewWidgetPlaylistDiscPlayer iViewWidgetPlaylistDiscPlayer;
        private IViewWidgetPlaylistRadio iViewWidgetPlaylistRadio;
        private IViewWidgetPlaylistReceiver iViewWidgetPlaylistReceiver;
        private IViewWidgetButton iViewButtonSave;
        private IViewWidgetButton iViewButtonWasteBin;
        private ISource iSource;
        private IInvoker iInvoker;
        private IPlaylistSupport iPlaylistSupport;
        private IViewWidgetTransportControl iViewWidgetTransportControl;
    }

    internal class MediatorPlayMode
    {
        public MediatorPlayMode(ISource aSource, IModel aModel, IInvoker aInvoker)
        {
            if (aSource is IPlayModeProvider)
            {
                iSource = aSource as IPlayModeProvider;
            }
            iViewWidgetPlayMode = aModel.ModelWidgetPlayMode;
            iInvoker = aInvoker;
        }

        public void Open()
        {
            if (iSource != null)
            {
                iViewWidgetPlayMode.EventToggleRepeat += EventToggleRepeat;
                iViewWidgetPlayMode.EventToggleShuffle += EventToggleShuffle;
                iSource.EventShuffleChanged += EventShuffleChanged;
                iSource.EventRepeatChanged += EventRepeatChanged;
                iViewWidgetPlayMode.Open();
                iViewWidgetPlayMode.Initialised();
            }
        }

        public void Close()
        {
            if (iSource != null)
            {
                iViewWidgetPlayMode.EventToggleRepeat -= EventToggleRepeat;
                iViewWidgetPlayMode.EventToggleShuffle -= EventToggleShuffle;
                iSource.EventShuffleChanged -= EventShuffleChanged;
                iSource.EventRepeatChanged -= EventRepeatChanged;
                iViewWidgetPlayMode.Close();
            }
        }


        void EventShuffleChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetPlayMode.SetShuffle(iSource.Shuffle);
        }

        void EventRepeatChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetPlayMode.SetRepeat(iSource.Repeat);
        }

        void EventToggleRepeat(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Repeat = !iSource.Repeat;
        }

        void EventToggleShuffle(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Shuffle = !iSource.Shuffle;
        }

        private IPlayModeProvider iSource;
        private IViewWidgetPlayMode iViewWidgetPlayMode;
        private IInvoker iInvoker;
    }

    internal class MediatorTransport
    {
        public MediatorTransport(ISource aSource, IModel aModel, IInvoker aInvoker)
        {
            iSource = aSource;
            if (iSource is IPlaylistSource)
            {
                iViewWidgetTransportControl = aModel.ModelWidgetTransportControlMediaRenderer;
            }
            else if (iSource is IRadioSource || iSource is IReceiverSource)
            {
                iViewWidgetTransportControl = aModel.ModelWidgetTransportControlRadio;
            }
            else if (iSource is IDiscSource)
            {
                iViewWidgetTransportControl = aModel.ModelWidgetTransportControlDiscPlayer;
            }
            iViewWidgetTime = aModel.ModelWidgetMediaTime;
            iViewWidgetTime.SetAllowSeeking(aSource.AllowSeeking);
            iTime = iSource.Room.Time;
            iInvoker = aInvoker;
        }

        public void Open()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetTime.EventSeekSeconds += EventSeekSeconds;
            iTime.EventDurationChanged += EventDurationChanged;
            iSource.EventTransportStateChanged += EventTransportStateChanged;
            iViewWidgetTime.SetTransportState(iSource.TransportState);
            if (iViewWidgetTransportControl != null)
            {
                iViewWidgetTransportControl.SetAllowSkipping(iSource.AllowSkipping);
                iViewWidgetTransportControl.SetAllowPausing(iSource.Type == Source.kSourceDs ||
                                                            iSource.Type == Source.kSourceRadio ||
                                                            iSource.Type == Source.kSourceUpnpAv);
                iViewWidgetTransportControl.SetDuration(iTime.Duration);
                iViewWidgetTransportControl.SetPlayNowEnabled(iSource.AllowPlayNowNextLater);
                iViewWidgetTransportControl.SetPlayNextEnabled(iSource.AllowPlayNowNextLater);
                iViewWidgetTransportControl.SetPlayLaterEnabled(iSource.AllowPlayNowNextLater);
                iViewWidgetTransportControl.EventPlay += EventPlay;
                iViewWidgetTransportControl.EventPause += EventPause;
                iViewWidgetTransportControl.EventStop += EventStop;
                iViewWidgetTransportControl.EventNext += EventNext;
                iViewWidgetTransportControl.EventPrevious += EventPrevious;
                iViewWidgetTransportControl.EventPlayNow += EventPlayNow;
                iViewWidgetTransportControl.EventPlayNext += EventPlayNext;
                iViewWidgetTransportControl.EventPlayLater += EventPlayLater;
                iViewWidgetTransportControl.Open();
            }
        }

        public void Close()
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iViewWidgetTime.EventSeekSeconds -= EventSeekSeconds;
            iTime.EventDurationChanged -= EventDurationChanged;
            iSource.EventTransportStateChanged -= EventTransportStateChanged;
            if (iViewWidgetTransportControl != null)
            {
                iViewWidgetTransportControl.EventPlay -= EventPlay;
                iViewWidgetTransportControl.EventPause -= EventPause;
                iViewWidgetTransportControl.EventStop -= EventStop;
                iViewWidgetTransportControl.EventNext -= EventNext;
                iViewWidgetTransportControl.EventPrevious -= EventPrevious;
                iViewWidgetTransportControl.EventPlayNow -= EventPlayNow;
                iViewWidgetTransportControl.EventPlayNext -= EventPlayNext;
                iViewWidgetTransportControl.EventPlayLater -= EventPlayLater;
                iViewWidgetTransportControl.Close();
            }
        }

        void EventPlay(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Play();
        }

        void EventPause(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Pause();
        }

        void EventStop(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Stop();
        }

        void EventNext(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource.Next() && iSource is IRadioSource)
            {
                iSource.Play();
            }
        }

        void EventPrevious(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource.Previous() && iSource is IRadioSource)
            {
                iSource.Play();
            }
        }

        void EventPlayNow(object sender, EventArgsPlay e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.PlayNow(e.Retriever, e.Callback);
        }

        void EventPlayNext(object sender, EventArgsPlay e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.PlayNext(e.Retriever, e.Callback);
        }

        void EventPlayLater(object sender, EventArgsPlay e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.PlayLater(e.Retriever, e.Callback);
        }

        void EventSeekSeconds(object sender, EventArgsSeekSeconds e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            iSource.Seek(e.Seconds);
        }

        void EventTransportStateChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iSource.TransportState == ETransportState.eWaiting)
            {
                if (iViewWidgetTransportControl != null)
                {
                    iViewWidgetTransportControl.SetTransportState(ETransportState.ePlaying);
                }
                iViewWidgetTime.SetTransportState(ETransportState.ePaused);
            }
            else
            {
                if (iViewWidgetTransportControl != null)
                {
                    iViewWidgetTransportControl.SetTransportState(iSource.TransportState);
                }
                iViewWidgetTime.SetTransportState(iSource.TransportState);
            }
        }

        void EventDurationChanged(object sender, EventArgs e)
        {
            if (iInvoker.InvokeRequired) { throw new InvocationException(); }
            if (iViewWidgetTransportControl != null && iTime == sender)
            {
                iViewWidgetTransportControl.SetDuration(iTime.Duration);
            }
        }

        public void Initialised()
        {
            if (iViewWidgetTransportControl != null)
            {
                iViewWidgetTransportControl.Initialised();
            }
        }

        private IViewWidgetTransportControl iViewWidgetTransportControl;
        private IViewWidgetMediaTime iViewWidgetTime;
        private ISource iSource;
        private IInvoker iInvoker;
        private IRoomTime iTime;
    }
} // Linn.Kinsky
