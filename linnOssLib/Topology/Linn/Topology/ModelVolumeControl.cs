using System;
using System.Threading;
using Linn.ControlPoint;

namespace Linn.Topology
{
    internal interface IVolumeLimiterControl
    {
        void IncrementVolume();
        void DecrementVolume();
    }

    internal class VolumeLimiter
    {
        public VolumeLimiter(IVolumeLimiterControl aVolumeLimiterControl)
        {
            iCount = 0;
            iEvent = new ManualResetEvent(false);
            iVolumeLimiterControl = aVolumeLimiterControl;
        }

        public void Start()
        {
            lock(this)
            {
                Assert.Check(iThread == null);

                iExiting = false;
                iCount = 0;
                iEvent.Reset();

                iThread = new Thread(Run);
                iThread.Name = "VolumeLimiter";
                iThread.IsBackground = true;
    
                iThread.Start();
            }
        }

        public void Stop()
        {
            lock(this)
            {
                iExiting = true;
                iEvent.Set();

                iThread.Join();
                iThread = null;
            }
        }

        public void IncrementVolume()
        {
            lock(this)
            {
                if(iCount < 0)
                {
                    iCount = 0;
                }

                if(iCount < kMaxOutstandingIncrements)
                {
                    ++iCount;
                }

                Assert.Check(iCount <= kMaxOutstandingIncrements);

                iEvent.Set();
            }
        }

        public void DecrementVolume()
        {
            lock(this)
            {
                if(iCount > 0)
                {
                    iCount = 0;
                }

                --iCount;

                iEvent.Set();
            }
        }

        private void Run()
        {
            while(!iExiting)
            {
                iEvent.WaitOne();

                if(!iExiting)
                {
                    bool increment = false;
                    bool decrement = false;
    
                    lock(this)
                    {
                        Assert.Check(iCount <= kMaxOutstandingIncrements);
                        
                        if(iCount > 0)
                        {
                            increment = true;
                            --iCount;
                        }
                        else if(iCount < 0)
                        {
                            decrement = true;
                            ++iCount;
                        }
                        else
                        {
                            iEvent.Reset();
                        }
                    }

                    try
                    {
                        if(increment)
                        {
                            iVolumeLimiterControl.IncrementVolume();
                        }
                        if(decrement)
                        {
                            iVolumeLimiterControl.DecrementVolume();
                        }
                    }
                    catch(System.Net.WebException) { }
                    catch(Linn.ControlPoint.ServiceException) { }
                }
            }
        }

        private const int kMaxOutstandingIncrements = 10;

        private bool iExiting;
        private Thread iThread;
        private ManualResetEvent iEvent;

        private int iCount;
        private IVolumeLimiterControl iVolumeLimiterControl;
    }
    
    public interface IModelVolumeControl
    {
        void Close();
        void DecrementVolume();
        event EventHandler<EventArgs> EventInitialised;
        event EventHandler<EventArgs> EventMuteStateChanged;
        event EventHandler<EventArgs> EventVolumeChanged;
        event EventHandler<EventArgs> EventVolumeLimitChanged;
        void IncrementVolume();
        bool Mute { get; }
        string Name { get; }
        void Open();
        void SetMute(bool aValue);
        void SetVolume(uint aValue);
        void ToggleMute();
        uint Volume { get; }
        uint VolumeLimit { get; }
        event EventHandler<EventArgs> EventSubscriptionError;
        Device Device { get; }
    }

    public abstract class ModelVolumeControl : IModelVolumeControl
    {

        public event EventHandler<EventArgs> EventSubscriptionError;
        public abstract void Open();
        public abstract void Close();

        public abstract string Name { get; }

        public abstract event EventHandler<EventArgs> EventInitialised;

        public abstract event EventHandler<EventArgs> EventMuteStateChanged;
        public abstract event EventHandler<EventArgs> EventVolumeChanged;
        public abstract event EventHandler<EventArgs> EventVolumeLimitChanged;

        public abstract void IncrementVolume();
        public abstract void DecrementVolume();
        public abstract void SetVolume(uint aValue);
        public abstract void ToggleMute();
        public abstract void SetMute(bool aValue);

        public abstract uint Volume { get; }
        public abstract bool Mute { get; }
        public abstract uint VolumeLimit { get; }

        public abstract Device Device {get;}

        protected void OnEventSubscriptionError()
        {
            EventHandler<EventArgs> eventSubscriptionError = EventSubscriptionError;
            if (eventSubscriptionError != null)
            {
                eventSubscriptionError(this, EventArgs.Empty);
            }
        }
    }
} // Linn.Topology