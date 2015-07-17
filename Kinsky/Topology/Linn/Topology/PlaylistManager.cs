using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;

using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class PlaylistManagers
    {
        public class EventArgsPlaylistManager : EventArgs
        {
            internal EventArgsPlaylistManager(PlaylistManager aPlaylistManager)
            {
                PlaylistManager = aPlaylistManager;
            }

            public PlaylistManager PlaylistManager;
        }

        public event EventHandler<EventArgsPlaylistManager> EventPlaylistManagerAdded;
        public event EventHandler<EventArgsPlaylistManager> EventPlaylistManagerRemoved;

        internal interface IJob
        {
            void Execute(PlaylistManagers aManagers);
        }

        internal abstract class Job : IJob
        {
            public Job(Device aDevice)
            {
                iDevice = aDevice;
            }

            public abstract void Execute(PlaylistManagers aManagers);

            protected Device iDevice;
        }

        internal class JobPlaylistManagerAdded : Job
        {
            public JobPlaylistManagerAdded(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(PlaylistManagers aManagers)
            {
                aManagers.DoPlaylistManagerAdded(iDevice);
            }
        }

        internal class JobPlaylistManagerRemoved : Job
        {
            public JobPlaylistManagerRemoved(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(PlaylistManagers aManagers)
            {
                aManagers.DoPlaylistManagerRemoved(iDevice);
            }
        }

        internal class JobAbort : IJob
        {
            public JobAbort()
            {
            }

            public void Execute(PlaylistManagers aManagers)
            {
                Thread.CurrentThread.Abort();
            }
        }

        private const ThreadPriority kPriority = ThreadPriority.Normal;

        public PlaylistManagers(ISsdpNotifyProvider aListenerNotify)
        {
            // create discovery system

            iDeviceListPlaylistManager = new DeviceListUpnp(ServicePlaylistManager.ServiceType(), aListenerNotify);
            iDeviceListPlaylistManager.EventDeviceAdded += PlaylistManagerAdded;
            iDeviceListPlaylistManager.EventDeviceRemoved += PlaylistManagerRemoved;

            iLock = new object();
            iJobList = new List<IJob>();
            iJobReady = new ManualResetEvent(false);
            iPlaylistManagers = new Dictionary<Device, PlaylistManager>();
        }

        public void Start(IPAddress aInterface)
        {
            Assert.Check(iThread == null);

            // start the discovery system

            iThread = new Thread(new ThreadStart(Run));
            iThread.Priority = kPriority;
            iThread.Name = "PlaylistManager";

            iThread.Start();
            iDeviceListPlaylistManager.Start(aInterface);
        }

        public void Stop()
        {
            // stop the discovery system

            ScheduleJob(new JobAbort());
            iDeviceListPlaylistManager.Stop();
            // iThread can be null if Stop() is called after a stack start previously failed
            if (iThread != null)
            {
                iThread.Join();
                iThread = null;
            }

            iJobList.Clear();
        }

        public void Rescan()
        {
            // rescan

            iDeviceListPlaylistManager.Rescan();
        }

        public void RemoveDevice(Device aDevice)
        {
            iDeviceListPlaylistManager.Remove(aDevice);
        }

        internal void ScheduleJob(IJob aJob)
        {
            lock(iLock)
            {
                iJobList.Add(aJob);
                iJobReady.Set();
            }
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    IJob job = null;

                    lock(iLock)
                    {
                        if(iJobList.Count == 0)
                        {
                            iJobReady.Reset();
                        }
                    }

                    iJobReady.WaitOne();

                    lock(iLock)
                    {
                        job = iJobList[0];
                        iJobList.RemoveAt(0);
                    }

                    job.Execute(this);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void PlaylistManagerAdded(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "PlaylistManager+                " + e.Device);

            e.Device.EventOpened += Opened;
            e.Device.EventOpenFailed += OpenFailed;

            e.Device.Open();
        }


        private void OpenFailed(object obj, EventArgs e)
        {
            iDeviceListPlaylistManager.Remove(obj as Device);
        }

        private void Opened(object obj, EventArgs e)
        {
            ScheduleJob(new JobPlaylistManagerAdded(obj as Device));
        }

        internal void DoPlaylistManagerAdded(Device aDevice)
        {
            PlaylistManager manager = new PlaylistManager(this, aDevice);
            iPlaylistManagers.Add(aDevice, manager);
            if (EventPlaylistManagerAdded != null)
            {
                EventPlaylistManagerAdded(this, new EventArgsPlaylistManager(manager));
            }
        }

        private void PlaylistManagerRemoved(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "PlaylistManagers-                " + e.Device);

            e.Device.EventOpened -= Opened;
            e.Device.EventOpenFailed -= OpenFailed;

            ScheduleJob(new JobPlaylistManagerRemoved(e.Device));
        }

        internal void DoPlaylistManagerRemoved(Device aDevice)
        {
            if(iPlaylistManagers.ContainsKey(aDevice))
            {
                PlaylistManager manager = iPlaylistManagers[aDevice];
                iPlaylistManagers.Remove(aDevice);
                if (EventPlaylistManagerRemoved != null)
                {
                    EventPlaylistManagerRemoved(this, new EventArgsPlaylistManager(manager));
                }
            }
        }

        private object iLock;
        
        private List<IJob> iJobList;
        private ManualResetEvent iJobReady;
        private Thread iThread;

        private DeviceListUpnp iDeviceListPlaylistManager;
        private Dictionary<Device, PlaylistManager> iPlaylistManagers;
    }

    public class PlaylistManager
    {
        public PlaylistManager(PlaylistManagers aManagers, Device aDevice)
        {
            iManagers = aManagers;
            iDevice = aDevice;
        }

        public PlaylistManagers Managers
        {
            get
            {
                return (iManagers);
            }
        }

        public string Name
        {
            get
            {
                return (iDevice.Name);
            }
        }

        public Device Device
        {
            get
            {
                return (iDevice);
            }
        }

        public override string ToString()
        {
            return (String.Format("PlaylistManager({0})", iDevice));
        }

        private PlaylistManagers iManagers;
        private Device iDevice;
    }
}

