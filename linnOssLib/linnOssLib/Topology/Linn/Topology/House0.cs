using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;
using Xamarin;

namespace Linn.Topology.Layer0
{
    public interface IStack
    {
        void Start(IPAddress aInterface);
        void Rescan();
        void Stop();
        event EventHandler<EventArgsDevice> EventProductAdded;
        event EventHandler<EventArgsDevice> EventProductRemoved;
        event EventHandler<EventArgsDevice> EventUpnpAdded;
        event EventHandler<EventArgsDevice> EventUpnpRemoved;
        void RemoveProduct(Device aDevice);
        void RemoveUpnp(Device aDevice);
    }

    public class EventArgsDevice : EventArgs
    {
        public EventArgsDevice(Device aDevice)
        {
            iDevice = aDevice;
        }

        public Device Device
        {
            get
            {
                return (iDevice);
            }
        }

        private Device iDevice;
    }

    public class Stack : IStack
    {
        public event EventHandler<EventArgsDevice> EventProductAdded;
        public event EventHandler<EventArgsDevice> EventProductRemoved;
        public event EventHandler<EventArgsDevice> EventUpnpAdded;
        public event EventHandler<EventArgsDevice> EventUpnpRemoved;

        internal interface IJob
        {
            void Execute(Stack aStack);
        }

        internal abstract class Job : IJob
        {
            public Job(Device aDevice)
            {
                iDevice = aDevice;
            }

            public abstract void Execute(Stack aStack);

            protected Device iDevice;
        }

        internal class JobProductAdded : Job
        {
            public JobProductAdded(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(Stack aStack)
            {
                aStack.DoProductAdded(iDevice);
            }
        }

        internal class JobProductRemoved : Job
        {
            public JobProductRemoved(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(Stack aStack)
            {
                aStack.DoProductRemoved(iDevice);
            }
        }

        internal class JobUpnpAdded : Job
        {
            public JobUpnpAdded(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(Stack aStack)
            {
                aStack.DoUpnpAdded(iDevice);
            }
        }

        internal class JobUpnpRemoved : Job
        {
            public JobUpnpRemoved(Device aDevice)
                : base(aDevice)
            {
            }

            public override void Execute(Stack aStack)
            {
                aStack.DoUpnpRemoved(iDevice);
            }
        }

        internal class JobAbort : IJob
        {
            public JobAbort()
            {
            }

            public void Execute(Stack aStack)
            {
                Thread.CurrentThread.Abort();
            }
        }

        private const ThreadPriority kPriority = ThreadPriority.Normal;

        public Stack(ISsdpNotifyProvider aListenerNotify)
        {
            // create discovery system

            iDeviceListProduct = new DeviceListUpnp(ServiceProduct.ServiceType(), aListenerNotify);
            iDeviceListProduct.EventDeviceAdded += ProductAdded;
            iDeviceListProduct.EventDeviceRemoved += ProductRemoved;

            iDeviceListUpnp = new DeviceListUpnp(ServiceAVTransport.ServiceType(), aListenerNotify);
            iDeviceListUpnp.EventDeviceAdded += UpnpAdded;
            iDeviceListUpnp.EventDeviceRemoved += UpnpRemoved;

            iDeviceAnalytics = new DeviceAnalytics(this);

            iMutex = new Mutex();
            iJobList = new List<IJob>();
            iJobReady = new ManualResetEvent(false);
        }

        public void Start(IPAddress aInterface)
        {
            Assert.Check(iThread == null);

            // start the discovery system

            iThread = new Thread(new ThreadStart(Run));
            iThread.Priority = kPriority;
            iThread.Name = "Topology Layer 0";

            iThread.Start();
            iDeviceListProduct.Start(aInterface);
            iDeviceListUpnp.Start(aInterface);

            Trace.WriteLine(Trace.kTopology, "Layer0.Stack.Start() successful");
            iDeviceAnalytics.Start();
        }

        public void Stop()
        {
            if (iThread != null)
            {
                // stop the discovery system

                ScheduleJob(new JobAbort());
                iDeviceListProduct.Stop();
                iDeviceListUpnp.Stop();
                iThread.Join();
                iThread = null;

                iDeviceAnalytics.Stop();
                Trace.WriteLine(Trace.kTopology, "Layer0.Stack.Stop() successful");
            }
            else
            {
                Trace.WriteLine(Trace.kTopology, "Layer0.Stack.Stop() already stopped - silently do nothing");
            }
        }

        public void Rescan()
        {
            // rescan

            iDeviceListProduct.Rescan();
            iDeviceListUpnp.Rescan();
        }

        internal void ScheduleJob(IJob aJob)
        {
            try
            {
                iMutex.WaitOne();
                iJobList.Add(aJob);
                iJobReady.Set();
            }
            finally
            {
                iMutex.ReleaseMutex();
            }
        }

        private void Run()
        {
            try
            {
                while (true)
                {
                    IJob job = null;
                    try
                    {
                        iMutex.WaitOne();

                        if (iJobList.Count == 0)
                        {
                            iJobReady.Reset();
                            try
                            {
                                iMutex.ReleaseMutex();

                                iJobReady.WaitOne();
                            }
                            finally
                            {
                                iMutex.WaitOne();
                            }
                        }

                        job = iJobList[0];

                        iJobList.RemoveAt(0);
                    }
                    finally
                    {
                        iMutex.ReleaseMutex();
                    }

                    job.Execute(this);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void ProductAdded(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer0 Product+         " + e.Device);

            ScheduleJob(new JobProductAdded(e.Device));
        }

        internal void DoProductAdded(Device aDevice)
        {
            if (EventProductAdded != null)
            {
                EventProductAdded(this, new EventArgsDevice(aDevice));
            }
        }

        private void ProductRemoved(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer0 Product-         " + e.Device);

            ScheduleJob(new JobProductRemoved(e.Device));
        }

        internal void DoProductRemoved(Device aDevice)
        {
            if (EventProductRemoved != null)
            {
                EventProductRemoved(this, new EventArgsDevice(aDevice));
            }
        }

        private void UpnpAdded(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer0 Upnp+            " + e.Device);

            if (!e.Device.IsLinn) // filter out Linn Upnp devices
            {
                ScheduleJob(new JobUpnpAdded(e.Device));
            }
        }

        internal void DoUpnpAdded(Device aDevice)
        {
            if (EventUpnpAdded != null)
            {
                EventUpnpAdded(this, new EventArgsDevice(aDevice));
            }
        }

        private void UpnpRemoved(object obj, DeviceList.EventArgsDevice e)
        {
            Trace.WriteLine(Trace.kTopology, "Layer0 Upnp-            " + e.Device);

            if (!e.Device.IsLinn) // filter out Linn Upnp devices
            {
                ScheduleJob(new JobUpnpRemoved(e.Device));
            }
        }

        internal void DoUpnpRemoved(Device aDevice)
        {
            if (EventUpnpRemoved != null)
            {
                EventUpnpRemoved(this, new EventArgsDevice(aDevice));
            }
        }

        public void RemoveProduct(Device aDevice)
        {
            iDeviceListProduct.Remove(aDevice);
        }

        public void RemoveUpnp(Device aDevice)
        {
            iDeviceListUpnp.Remove(aDevice);
        }

        private Mutex iMutex;
        private List<IJob> iJobList;
        private ManualResetEvent iJobReady;
        private Thread iThread;

        private DeviceListUpnp iDeviceListProduct;
        private DeviceListUpnp iDeviceListUpnp;
        private DeviceAnalytics iDeviceAnalytics;
    }

    class DeviceAnalytics
    {
        private readonly IStack iStack;
        private readonly Dictionary<string, DeviceWatcher> iDeviceWatchers;
        private readonly Dictionary<string, Device> iOpenDevices;
        private SafeTimer iTimer;
        private const uint kTimeout = 1 * 60 * 1000; // 1 minute
        private bool iRunning;
        private readonly object iLock = new object();

        public DeviceAnalytics(IStack aStack)
        {
            iStack = aStack;
            iDeviceWatchers = new Dictionary<string, DeviceWatcher>();
            iOpenDevices = new Dictionary<string, Device>();
        }

        public void Start()
        {
            iStack.EventProductAdded += ProductAdded;
            iStack.EventUpnpAdded += UpnpAdded;

            lock(iLock)
            {
                iRunning = true;
                iTimer = new SafeTimer(SendAnalyticsEvents);
                iTimer.FireIn(kTimeout);
            }
        }

        public void Stop()
        {
            iStack.EventProductAdded -= ProductAdded;
            iStack.EventUpnpAdded -= UpnpAdded;

            SafeTimer timer = null;
            lock(iLock)
            {
                iRunning = false;
                foreach (var device in iDeviceWatchers.Values)
                {
                    device.Dispose();
                }
                iDeviceWatchers.Clear();
                iOpenDevices.Clear();
                timer = iTimer;
                iTimer = null;
            }
            if (timer != null)
            {
                timer.Dispose();
            }
        }

        private void ProductAdded(object sender, EventArgsDevice e)
        {
            Watch(e.Device, true);
        }

        private void UpnpAdded(object sender, EventArgsDevice e)
        {
            Watch(e.Device, false);
        }

        private void Watch(Device aDevice, bool aRequiresOpened)
        {
            lock (iLock)
            {
                if (!iRunning)
                {
                    return;
                }
                if (!iDeviceWatchers.ContainsKey(aDevice.Udn))
                {
                    iDeviceWatchers.Add(aDevice.Udn, new DeviceWatcher(aDevice, aRequiresOpened, () =>
                    {
                        lock (iLock)
                        {
                            if (!iRunning)
                            {
                                return;
                            }
                            if (!iOpenDevices.ContainsKey(aDevice.Udn))
                            {
                                iOpenDevices.Add(aDevice.Udn, aDevice);
                            }
                        }
                    }));
                }
            }
        }

        private void SendAnalyticsEvents()
        {
            var deviceEvents = new List<Dictionary<string, string>>();
            var allLinn = true;
            var devicesCount = 0;

            lock (iLock)
            {
                if (!iRunning)
                {
                    return;
                }
                foreach (var device in iOpenDevices.Values)
                {
                    var eventData = new Dictionary<string, string>();
                    allLinn &= device.IsLinn;
                    eventData.Add("Manufacturer", device.Manufacturer);
                    eventData.Add("Model", device.Model);
                    deviceEvents.Add(eventData);
                    eventData.Add("Model." + (device.IsLinn ? "Linn" : "Non_Linn"), device.Model);
                    eventData.Add("Model." + device.Manufacturer.Replace(" ","_"), device.Model);
                }
                devicesCount = iOpenDevices.Count;
            }

            if (Insights.IsInitialized)
            {
                foreach (var eventData in deviceEvents)
                {
                    Insights.Track("Device", eventData);
                }
                if (devicesCount > 0)
                {
                    Insights.Track("House", new Dictionary<string, string>() { { "IsAllLinn", allLinn.ToString() } });
                }
            }
        }

        class DeviceWatcher : IDisposable
        {
            public Device Device { get; private set; }
            private System.Action iOpened;

            public DeviceWatcher(Device aDevice, bool aRequiresOpen, System.Action aOpened)
            {
                Device = aDevice;
                iOpened = aOpened;

                Device.EventOpened += DeviceOpened;
                if (aRequiresOpen)
                {
                    Device.Open();
                }
            }

            private void DeviceOpened(object sender, EventArgs args)
            {
                iOpened();
            }

            public void Dispose()
            {
                Device.EventOpened -= DeviceOpened;
            }
        }
    }
}
