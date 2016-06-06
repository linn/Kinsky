using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace Linn
{
    public class NetworkInfo
    {
        public static bool GetIsNetworkAvailable()
        {
	        return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        public static List<NetworkInfoModel> GetAllNetworkInterfaces()
        {
            List<NetworkInfoModel> adaptors = new List<NetworkInfoModel>();
            System.Net.NetworkInformation.NetworkInterface[] nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            foreach (System.Net.NetworkInformation.NetworkInterface adapter in nics)
            {
                string adapterName = adapter.Name;
                string adapterDescription = adapter.Description;
                string macAddress = adapter.GetPhysicalAddress().ToString();

                ENetworkInterfaceComponent networkInterfaceComponent = (adapter.Supports(NetworkInterfaceComponent.IPv4))
                    ? ENetworkInterfaceComponent.eIPv4
                    : ENetworkInterfaceComponent.eUnknown;

                ENetworkInterfaceType networkInterfaceType = (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    ? ENetworkInterfaceType.eLoopBack
                    : ENetworkInterfaceType.eUnknown;

                // ignore loopback adapters, non IPv4 adapters
                if (networkInterfaceType == ENetworkInterfaceType.eLoopBack)
                {
                    UserLog.WriteLine(adapter.Description + " is loopback - ignored");
                    continue;
                }

                if (networkInterfaceComponent != ENetworkInterfaceComponent.eIPv4)
                {
                    UserLog.WriteLine(adapter.Description + " does not support IPv4 - ignored");
                    continue;
                }

                bool? supportsMulticast = null;
                bool? isRecieveOnly = null;
                try
                {
                    supportsMulticast = adapter.SupportsMulticast;
                    isRecieveOnly = adapter.IsReceiveOnly;
                }
                catch (PlatformNotSupportedException)
                {
                    // Multicast, IsReceiveOnly support only defined in Windows XP and greater
                }

                EOperationalStatus operationalStatus = (EOperationalStatus)((int)adapter.OperationalStatus);
                IPInterfaceProperties properties = adapter.GetIPProperties();

                bool? isDHCPEnabled = null;
                bool? isAutoPrivateAddressEnabled = null;
                int? mtu = null;
                
                try
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    if (ipv4 != null)
                    {
                        isDHCPEnabled = ipv4.IsDhcpEnabled;
                        isAutoPrivateAddressEnabled = ipv4.IsAutomaticPrivateAddressingEnabled;
                        mtu = ipv4.Mtu;
                    }
                }
                catch { } // throws an error in mono 2.6
                
                List<IPAddress> gatewayAddress = new List<IPAddress>();
                foreach (GatewayIPAddressInformation gateway in properties.GatewayAddresses)
                {
                    gatewayAddress.Add(gateway.Address);
                }

                int i = 0;
                UnicastIPAddressInformationCollection uniCast = properties.UnicastAddresses;
                foreach (UnicastIPAddressInformation ip in uniCast)
                {
                    // only take ipv4 address of adapter (may have ipv4 and ipv6)
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        NetworkInfoModel adaptor = new NetworkInfoModel(adapterDescription
                                                              , adapterName + ((i == 0) ? "" : " (" + i.ToString() +")")
                                                              , networkInterfaceType
                                                              , ip.Address
                                                              , supportsMulticast
                                                              , networkInterfaceComponent
                                                              , macAddress
                                                              , operationalStatus
                                                              , isRecieveOnly
                                                              , isDHCPEnabled
                                                              , isAutoPrivateAddressEnabled
                                                              , mtu
                                                              , properties.IsDnsEnabled
                                                              , properties.IsDynamicDnsEnabled
                                                              , properties.DnsSuffix
                                                              , gatewayAddress);
                        
                        adaptors.Add(adaptor);
                        ++i;
                        UserLog.WriteLine(adapterName + " (" + ip.Address.ToString() + ") is supported");
                        Trace.WriteLine(Trace.kCore, adapterName + " [" + adapterDescription + "] (" + ip.Address.ToString() + ") is supported");
                    }
                }
            }

            return adaptors;
        }
    }
}
