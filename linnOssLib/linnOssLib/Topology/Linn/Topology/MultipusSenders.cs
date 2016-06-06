using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class Senders
    {
        public class EventArgsSender : EventArgs
        {
            internal EventArgsSender(Sender aSender)
            {
                Sender = aSender;
            }

            public Sender Sender;
        }

        public event EventHandler<EventArgsSender> EventSenderAdded;
        public event EventHandler<EventArgsSender> EventSenderRemoved;

        internal interface IJob
        {
            void Execute(Senders aSenders);
        }

        internal abstract class Job : IJob
        {
            public Job(Device aDevice)
            {
                iDevice = aDevice;
            }

            public abstract void Execute(Senders aSenders);

            protected Device iDevice;
        }

        internal class JobSenderAdded : Job
        {
            public JobSenderAdded(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(Senders aSenders)
            {
                aSenders.DoSenderAdded(iDevice);
            }
        }

        internal class JobSenderRemoved : Job
        {
            public JobSenderRemoved(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(Senders aSenders)
            {
                aSenders.DoSenderRemoved(iDevice);
            }
        }

        internal class JobAbort : IJob
        {
            public JobAbort()
            {
            }

            public void Execute(Senders aSenders)
            {
                Thread.CurrentThread.Abort();
            }
        }

        private const ThreadPriority kPriority = ThreadPriority.Normal;

        public Senders(ISsdpNotifyProvider aListenerNotify)
        {
            // create discovery system

            iDeviceListSender = new DeviceListUpnp(ServiceSender.ServiceType(), aListenerNotify);
            iDeviceListSender.EventDeviceAdded += SenderAdded;
            iDeviceListSender.EventDeviceRemoved += SenderRemoved;

            iMutex = new Mutex();
            iJobList = new List<IJob>();
            iJobReady = new ManualResetEvent(false);
            iDevices = new Dictionary<string, Device>();
            iLock = new object();
            iOpen = false;
        }

        public void Start(IPAddress aInterface)
        {
            Assert.Check(iThread == null);

            // start the discovery system

            iThread = new Thread(new ThreadStart(Run));
            iThread.Priority = kPriority;
            iThread.Name = "Senders";

            iThread.Start();
            iDeviceListSender.Start(aInterface);

            Trace.WriteLine(Trace.kTopology, "Senders.Start() successful");
            iOpen = true;
        }

        public void Stop()
        {
            if (iOpen)
            {
                // stop the discovery system

                ScheduleJob(new JobAbort());
                iDeviceListSender.Stop();
                iThread.Join();
                iThread = null;

                iJobList.Clear();
                lock (iLock)
                {
                    foreach (string key in iDevices.Keys)
                    {
                        iDevices[key].EventOpened -= Opened;
                        iDevices[key].EventOpenFailed -= OpenFailed;
                    }
                    iDevices.Clear();
                }

                Trace.WriteLine(Trace.kTopology, "Senders.Stop() successful");
                iOpen = false;
            }
        }

        public void Rescan()
        {
            // rescan

            iDeviceListSender.Rescan();
        }

        public void RemoveDevice(Device aDevice)
        {
            iDeviceListSender.Remove(aDevice);
        }

        internal void ScheduleJob(IJob aJob)
        {
            iMutex.WaitOne();
            iJobList.Add(aJob);
            iJobReady.Set();
            iMutex.ReleaseMutex();
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    iMutex.WaitOne();

                    if (iJobList.Count == 0)
                    {
                        iJobReady.Reset();

                        iMutex.ReleaseMutex();

                        iJobReady.WaitOne();

                        iMutex.WaitOne();
                    }

                    IJob job = iJobList[0];

                    iJobList.RemoveAt(0);

                    iMutex.ReleaseMutex();

                    job.Execute(this);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void SenderAdded(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Senders+                " + e.Device);

            lock (iLock)
            {
                if (iDevices.ContainsKey(e.Device.Udn))
                {
                    iDevices[e.Device.Udn].EventOpened -= Opened;
                    iDevices[e.Device.Udn].EventOpenFailed -= OpenFailed;
                    iDevices.Remove(e.Device.Udn);
                }
                iDevices.Add(e.Device.Udn, e.Device);
            }

            e.Device.EventOpened += Opened;
            e.Device.EventOpenFailed += OpenFailed;

            e.Device.Open();
        }

        private void OpenFailed(object obj, EventArgs e)
        {
            iDeviceListSender.Remove(obj as Device);
        }

        private void Opened(object obj, EventArgs e)
        {
            lock (iLock)
            {
                Device d = obj as Device;
                if (iOpen && iDevices.ContainsKey(d.Udn) && iDevices[d.Udn] == d)
                {
                    ScheduleJob(new JobSenderAdded(obj as Device));
                }
            }
        }

        internal void DoSenderAdded(Device aDevice)
        {
            if (EventSenderAdded != null)
            {
                EventSenderAdded(this, new EventArgsSender(new Sender(this, aDevice)));
            }
        }

        private void SenderRemoved(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Senders-                " + e.Device);

            e.Device.EventOpened -= Opened;
            e.Device.EventOpenFailed -= OpenFailed;

            ScheduleJob(new JobSenderRemoved(e.Device));
        }

        internal void DoSenderRemoved(Device aDevice)
        {
            lock (iLock)
            {
                iDevices.Remove(aDevice.Udn);
            }
            if (EventSenderRemoved != null)
            {
                EventSenderRemoved(this, new EventArgsSender(new Sender(this, aDevice)));
            }
        }

        private Mutex iMutex;
        private List<IJob> iJobList;
        private ManualResetEvent iJobReady;
        private Thread iThread;

        private DeviceListUpnp iDeviceListSender;
        private Dictionary<string, Device> iDevices;
        private object iLock;
        private bool iOpen;
    }

    public class Sender
    {
        public Sender(Senders aSenders, Device aDevice)
        {
            iSenders = aSenders;
            iDevice = aDevice;
        }

        public Senders Senders
        {
            get
            {
                return (iSenders);
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
            return (String.Format("Sender({0})", iDevice));
        }

        private Senders iSenders;
        private Device iDevice;
    }
}
