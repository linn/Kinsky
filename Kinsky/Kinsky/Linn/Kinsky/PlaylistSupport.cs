
using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;

using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;
using Linn.Topology;
using Upnp;
using System.Collections.ObjectModel;

namespace Linn.Kinsky
{
    public class PlaySupport : IPlaylistSupport
    {
        public PlaySupport()
        {
            iLock = new object();
        }

        public void Open()
        {
            Assert.Check(!iOpen);

            iOpen = true;

            if (EventIsOpenChanged != null)
            {
                EventIsOpenChanged(this, EventArgs.Empty);
            }
        }

        public void Close()
        {
            iOpen = false;

            if (EventIsOpenChanged != null)
            {
                EventIsOpenChanged(this, EventArgs.Empty);
            }
        }

        public bool IsOpen()
        {
            return iOpen;
        }

        public bool IsInserting()
        {
            lock (iLock)
            {
                return iInserting;
            }
        }

        public bool IsDragging()
        {
            return iDragging;
        }

        public bool IsInsertAllowed()
        {
            return iInsertAllowed;
        }

        public void SetDragging(bool aDragging)
        {
            if (aDragging != iDragging)
            {
                iDragging = aDragging;

                if (EventIsDraggingChanged != null)
                {
                    EventIsDraggingChanged(this, EventArgs.Empty);
                }
            }
        }

        public void SetInsertAllowed(bool aInsertAllowed)
        {
            if (aInsertAllowed != iInsertAllowed)
            {
                iInsertAllowed = aInsertAllowed;

                if (EventIsInsertAllowedChanged != null)
                {
                    EventIsInsertAllowedChanged(this, EventArgs.Empty);
                }
            }
        }

        private void InsertCompletedCallback(uint aCount)
        {
            lock (iLock)
            {
                iInserting = false;
                OnEventIsInsertingChanged();
            }
        }

        private void MoveCompletedCallback()
        {
            lock (iLock)
            {
                iInserting = false;
                OnEventIsInsertingChanged();
            }
        }

        public void PlayNow(IMediaRetriever aMediaRetriever)
        {
            lock (iLock)
            {
                EventHandler<EventArgsPlay> del = EventPlayNow;
                if (iInsertAllowed && !iInserting && del != null)
                {
                    iInserting = true;
                    del(this, new EventArgsPlay(aMediaRetriever, InsertCompletedCallback));
                    OnEventIsInsertingChanged();
                }
            }
        }

        public void PlayNext(IMediaRetriever aMediaRetriever)
        {
            lock (iLock)
            {
                EventHandler<EventArgsPlay> del = EventPlayNext;
                if (iInsertAllowed && !iInserting && del != null)
                {
                    iInserting = true;
                    del(this, new EventArgsPlay(aMediaRetriever, InsertCompletedCallback));
                    OnEventIsInsertingChanged();
                }
            }
        }

        public void PlayLater(IMediaRetriever aMediaRetriever)
        {
            lock (iLock)
            {
                EventHandler<EventArgsPlay> del = EventPlayLater;
                if (iInsertAllowed && !iInserting && del != null)
                {
                    iInserting = true;
                    del(this, new EventArgsPlay(aMediaRetriever, InsertCompletedCallback));
                    OnEventIsInsertingChanged();
                }
            }
        }

        public void PlayInsert(uint aInsertAfterId, IMediaRetriever aMediaRetriever)
        {
            lock (iLock)
            {
                EventHandler<EventArgsInsert> del = EventPlayInsert;
                if (iInsertAllowed && !iInserting && del != null)
                {
                    iInserting = true;
                    del(this, new EventArgsInsert(aInsertAfterId, aMediaRetriever, InsertCompletedCallback));
                    OnEventIsInsertingChanged();
                }
            }
        }

        public void Move(uint aInsertAfterId, IList<MrItem> aPlaylistItems)
        {
            lock (iLock)
            {
                EventHandler<EventArgsMove> del = EventMove;
                if (iInsertAllowed && !iInserting && del != null)
                {
                    iInserting = true;
                    del(this, new EventArgsMove(aInsertAfterId, aPlaylistItems, MoveCompletedCallback));
                    OnEventIsInsertingChanged();
                }
            }
        }

        protected void OnEventIsInsertingChanged()
        {
            EventHandler<EventArgs> del = EventIsInsertingChanged;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> EventIsOpenChanged;
        public event EventHandler<EventArgs> EventIsInsertingChanged;
        public event EventHandler<EventArgs> EventIsDraggingChanged;
        public event EventHandler<EventArgs> EventIsInsertAllowedChanged;

        public event EventHandler<EventArgsPlay> EventPlayNow;
        public event EventHandler<EventArgsPlay> EventPlayNext;
        public event EventHandler<EventArgsPlay> EventPlayLater;
        public event EventHandler<EventArgsInsert> EventPlayInsert;
        public event EventHandler<EventArgsMove> EventMove;

        private bool iOpen;
        private bool iDragging;
        private bool iInsertAllowed;
        private bool iInserting;

        private object iLock;
    }

} // Linn.Kinsky