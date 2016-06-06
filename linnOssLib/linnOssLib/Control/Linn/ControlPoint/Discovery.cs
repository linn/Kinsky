using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;

using Linn.Control;
using Linn.Control.Ssdp;

namespace Linn.ControlPoint
{

    public class DeviceException : Exception
    {
        public DeviceException(int aCode, string aDescription)
            : base("Code=" + aCode.ToString() + " Description=\"" + aDescription + "\"")
        {
            Console.WriteLine(aDescription + " (Code: " + aCode + ")");

            Code = aCode;
            Description = aDescription;
        }

        public int Code;
        public string Description;
    }

    public abstract class Device
    {
        private static readonly string kUdnLinnPrefix = "4c494e4e-";
        public DateTime LastSeen { get; set; }

        protected static readonly string kKeyUdn = "Udn";

        protected Device()
        {
            iMutex = new Mutex();
            iDictionary = new Dictionary<string, string>();
            LastSeen = DateTime.Now;
        }

        protected Device(string aUdn)
        {
            iMutex = new Mutex();
            iDictionary = new Dictionary<string, string>();

            Add(kKeyUdn, aUdn);
            LastSeen = DateTime.Now;
        }

        public abstract uint HasService(ServiceType aType);
        public abstract bool HasAction(ServiceType aType, string aAction);
        public abstract bool HasState(ServiceType aType, string aState);

        public abstract ServiceLocation FindServiceLocation(ServiceType aType);
        public abstract Device RelatedDevice(string aUdn);

        public bool IsLinn
        {
            get
            {
                string udn = Find(kKeyUdn);
                return (udn.StartsWith(kUdnLinnPrefix));
            }
        }

        public string Find(string aKey)
        {
            string value;

            Lock();

            iDictionary.TryGetValue(aKey, out value);

            Unlock();

            return (value);
        }

        public void Add(string aKey, string aValue)
        {
            Lock();

            iDictionary.Add(aKey, aValue);

            Unlock();
        }

        public void Remove(string aKey)
        {
            Lock();

            iDictionary.Remove(aKey);

            Unlock();
        }

        public string Udn
        {
            get
            {
                return Find(kKeyUdn);
            }
        }

        public abstract void Open();

        public event EventHandler<EventArgs> EventOpened;
        public event EventHandler<EventArgs> EventOpenFailed;

        protected void Opened()
        {
            if (EventOpened != null)
            {
                EventOpened(this, EventArgs.Empty);
            }
        }
        protected void OpenFailed()
        {
            if (EventOpenFailed != null)
            {
                EventOpenFailed(this, EventArgs.Empty);
            }
        }

        public abstract string Name
        {
            get;
        }

        public abstract string Model
        {
            get;
        }

        public abstract string Location
        {
            get;
        }

        public abstract string IpAddress
        {
            get;
        }

        public abstract string PresentationUri
        {
            get;
        }

        public abstract string DeviceXml
        {
            get;
        }

        public override string ToString()
        {
            string result = "Device{Udn{" + Find(kKeyUdn) + "}";

            Lock();

            foreach (string key in iDictionary.Keys)
            {
                result += key;
                result += "{";
                result += iDictionary[key];
                result += "}";
            }

            Unlock();

            result += "}";

            return (result);
        }

        protected void Lock()
        {
            iMutex.WaitOne();
        }

        protected void Unlock()
        {
            iMutex.ReleaseMutex();
        }

        private Mutex iMutex;
        private Dictionary<string, string> iDictionary;
    }

    public abstract class DeviceList
    {
        public class EventArgsDevice : EventArgs
        {
            public EventArgsDevice(Device aDevice)
            {
                Device = aDevice;
            }

            public Device Device;
        }

        public EventHandler<EventArgsDevice> EventDeviceAdded;
        public EventHandler<EventArgsDevice> EventDeviceRemoved;

        protected DeviceList()
        {
            iDictionary = new Dictionary<string, Device>();
        }

        public abstract void Start(IPAddress aInterface);
        public abstract void Stop();
        public abstract void Rescan();
        
        public void Remove(Device aDevice)
        {
            Remove(aDevice.Udn);
        }

        protected void Add(Device aDevice)
        {
            bool added = false;
            lock (iDictionary)
            {
                Device device;

                iDictionary.TryGetValue(aDevice.Udn, out device);

                if (device == null)
                {
                    if (aDevice.IpAddress != IPAddress.Loopback.ToString())
                    {
                        aDevice.LastSeen = DateTime.Now;
                        iDictionary.Add(aDevice.Udn, aDevice);

                        added = true;

                        UserLog.WriteLine(DateTime.Now + ": Device+                   " + aDevice);
                        Trace.WriteLine(Trace.kTopology, "Device List+            " + aDevice);
                    }
                    else
                    {
                        UserLog.WriteLine(DateTime.Now + ": Device+                   " + aDevice + " - ignored");
                        Trace.WriteLine(Trace.kTopology, "Device List+            " + aDevice + " - ignored");
                    }
                }
                else
                {
                    device.LastSeen = DateTime.Now;

                    Trace.WriteLine(Trace.kTopology, "Device List?            " + aDevice);
                }
            }
            if (added && EventDeviceAdded != null)
            {
                EventDeviceAdded(this, new EventArgsDevice(aDevice));
            }
        }

        protected void Remove(string aUdn)
        {
            bool removed = false;
            Device device;
            lock (iDictionary)
            {

                iDictionary.TryGetValue(aUdn, out device);

                if (device != null)
                {

                    iDictionary.Remove(aUdn);

                    removed = true;

                    UserLog.WriteLine(DateTime.Now + ": Device-                   " + aUdn);
                    Trace.WriteLine(Trace.kTopology, "Device List-            " + aUdn);

                }
            }
            if (removed && EventDeviceRemoved != null)
            {
                EventDeviceRemoved(this, new EventArgsDevice(device));
            }
        }

        protected void Clear()
        {
            Trace.WriteLine(Trace.kTopology, "Device List             Clear");
            lock (iDictionary)
            {
                iDictionary.Clear();
            }
        }

        protected void RemoveExpiredDevices(DateTime aNotSeenSince)
        {
            Trace.WriteLine(Trace.kTopology, "RemoveExpiredDevices");
            List<Device> expired = new List<Device>();
            DateTime now = DateTime.Now;
            lock (iDictionary)
            {
                foreach (Device d in iDictionary.Values)
                {
                    if (d.LastSeen < aNotSeenSince)
                    {
                        expired.Add(d);
                    }
                }
            }
            foreach (Device d in expired)
            {
                UserLog.WriteLine("Device List             Expired:" + d);
                Trace.WriteLine(Trace.kTopology, "Device List             Expired:" + d);
                Remove(d.Udn);
            }
        }

        private Dictionary<string, Device> iDictionary;
    }
}
