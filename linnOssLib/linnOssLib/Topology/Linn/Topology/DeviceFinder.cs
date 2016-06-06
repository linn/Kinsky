using System;
using System.Net;
using System.Threading;

using Linn.Control;
using Linn.Control.Ssdp;
using Linn.ControlPoint;
using Linn.ControlPoint.Upnp;

namespace Linn.Topology
{
    public class DeviceFinderException : Exception
    {
    }

    public class DeviceFinder
    {
        private SsdpListenerMulticast iListener;
        private DeviceListUpnp iDeviceList;
        private ManualResetEvent iSemaphore;
        private Device iDevice;
        private string iUglyName;
        private bool iFound;

        public DeviceFinder(string aUglyName)
        {
            iUglyName = aUglyName;

            ServiceType type = ServiceVolkano.ServiceType();

            type.Version = 1;

            iListener = new SsdpListenerMulticast();

            iDeviceList = new DeviceListUpnp(type, iListener);

            iDeviceList.EventDeviceAdded += DeviceAdded;

            iSemaphore = new ManualResetEvent(false);

            iFound = false;
        }

        public Device Find(IPAddress aInterface, int aTimeout) // in milliseconds
        {
            Trace.WriteLine(Trace.kTopology, "DeviceFinder Find: " + iUglyName);

            iListener.Start(aInterface);
            iDeviceList.Start(aInterface);

            iSemaphore.WaitOne(aTimeout, false);

            Trace.WriteLine(Trace.kTopology, "DeviceFinder Found: " + iUglyName);

            iDeviceList.Stop();
            iListener.Stop();

            if (iFound)
            {
                return (iDevice);
            }

            throw (new DeviceFinderException());
        }

        private void DeviceAdded(object obj, DeviceList.EventArgsDevice e)
        {
            if (!iFound)
            {
                Trace.WriteLine(Trace.kTopology, "Added: " + e.Device.Udn);

                ServiceVolkano volkano = new ServiceVolkano(e.Device);

                ServiceVolkano.AsyncActionUglyName async = volkano.CreateAsyncActionUglyName();

                async.EventResponse += UglyNameResponse;

                async.UglyNameBegin();
            }
        }

        private void UglyNameResponse(object obj, ServiceVolkano.AsyncActionUglyName.EventArgsResponse aResponse)
        {
            if (!iFound)
            {
                Trace.WriteLine(Trace.kTopology, "Found: " + aResponse.aUglyName);

                if (String.Compare(aResponse.aUglyName, iUglyName, true) == 0)
                {
                    iDevice = (obj as ServiceVolkano).Device;

                    iFound = true;

                    iSemaphore.Set();
                }
            }
        }
    }
}
