using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;


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
            Linn.CopiedFromMono.NetworkInterface[] nics = Linn.CopiedFromMono.NetworkInterface.ImplGetAllNetworkInterfaces();

            foreach (Linn.CopiedFromMono.NetworkInterface adapter in nics)
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
                Linn.CopiedFromMono.IPInterfaceProperties properties = adapter.GetIPProperties();

                bool? isDHCPEnabled = null;
                bool? isAutoPrivateAddressEnabled = null;
                int? mtu = null;
                
                try
                {
                    Linn.CopiedFromMono.IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    if (ipv4 != null)
                    {
                        isDHCPEnabled = ipv4.IsDhcpEnabled;
                        isAutoPrivateAddressEnabled = ipv4.IsAutomaticPrivateAddressingEnabled;
                        mtu = ipv4.Mtu;
                    }
                }
                catch { } // throws an error in mono 2.6
                
                List<IPAddress> gatewayAddress = new List<IPAddress>();
                foreach (IPAddress gateway in properties.GatewayAddresses)
                {
                    gatewayAddress.Add(gateway);
                }

                int i = 0;
                List<IPAddress> uniCast = properties.UnicastAddresses;
                foreach (IPAddress ip in uniCast)
                {
                    // only take ipv4 address of adapter (may have ipv4 and ipv6)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        NetworkInfoModel adaptor = new NetworkInfoModel(adapterDescription
                                                              , adapterName + ((i == 0) ? "" : " (" + i.ToString() +")")
                                                              , networkInterfaceType
                                                              , ip
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
                        UserLog.WriteLine(adapterName + " (" + ip.ToString() + ") is supported");
                        Trace.WriteLine(Trace.kCore, adapterName + " [" + adapterDescription + "] (" + ip.ToString() + ") is supported");
                    }
                }
            }

            return adaptors;
        }
    }
}


namespace Linn.CopiedFromMono
{
    class NetworkInterface
    {
        public static NetworkInterface [] ImplGetAllNetworkInterfaces ()
        {
            var interfaces = new Dictionary <string, NetworkInterface> ();
            IntPtr ifap;
            if (getifaddrs (out ifap) != 0)
                throw new SystemException ("getifaddrs() failed");

            try {
                IntPtr next = ifap;
                while (next != IntPtr.Zero) {
                    MacOsStructs.ifaddrs addr = (MacOsStructs.ifaddrs) Marshal.PtrToStructure (next, typeof (MacOsStructs.ifaddrs));
                    IPAddress address = IPAddress.None;
                    string    name = addr.ifa_name;
                    int       index = -1;
                    byte[]    macAddress = null;
                    NetworkInterfaceType type = NetworkInterfaceType.Unknown;

                    if (addr.ifa_addr != IntPtr.Zero) {
                        MacOsStructs.sockaddr sockaddr = (MacOsStructs.sockaddr) Marshal.PtrToStructure (addr.ifa_addr, typeof (MacOsStructs.sockaddr));

                        if (sockaddr.sa_family == AF_INET6) {
                            MacOsStructs.sockaddr_in6 sockaddr6 = (MacOsStructs.sockaddr_in6) Marshal.PtrToStructure (addr.ifa_addr, typeof (MacOsStructs.sockaddr_in6));
                            address = new IPAddress (sockaddr6.sin6_addr.u6_addr8, sockaddr6.sin6_scope_id);
                        } else if (sockaddr.sa_family == AF_INET) {
                            MacOsStructs.sockaddr_in sockaddrin = (MacOsStructs.sockaddr_in) Marshal.PtrToStructure (addr.ifa_addr, typeof (MacOsStructs.sockaddr_in));
                            address = new IPAddress (sockaddrin.sin_addr);
                        } else if (sockaddr.sa_family == AF_LINK) {
                            MacOsStructs.sockaddr_dl sockaddrdl = (MacOsStructs.sockaddr_dl) Marshal.PtrToStructure (addr.ifa_addr, typeof (MacOsStructs.sockaddr_dl));

                            macAddress = new byte [(int) sockaddrdl.sdl_alen];
                            Array.Copy (sockaddrdl.sdl_data, sockaddrdl.sdl_nlen, macAddress, 0, Math.Min (macAddress.Length, sockaddrdl.sdl_data.Length - sockaddrdl.sdl_nlen));
                            index = sockaddrdl.sdl_index;

                            int hwtype = (int) sockaddrdl.sdl_type;
                            if (Enum.IsDefined (typeof (MacOsArpHardware), hwtype)) {
                                switch ((MacOsArpHardware) hwtype) {
                                    case MacOsArpHardware.ETHER:
                                        type = NetworkInterfaceType.Ethernet;
                                        break;

                                    case MacOsArpHardware.ATM:
                                        type = NetworkInterfaceType.Atm;
                                        break;
                                    
                                    case MacOsArpHardware.SLIP:
                                        type = NetworkInterfaceType.Slip;
                                        break;
                                    
                                    case MacOsArpHardware.PPP:
                                        type = NetworkInterfaceType.Ppp;
                                        break;
                                    
                                    case MacOsArpHardware.LOOPBACK:
                                        type = NetworkInterfaceType.Loopback;
                                        macAddress = null;
                                        break;

                                    case MacOsArpHardware.FDDI:
                                        type = NetworkInterfaceType.Fddi;
                                        break;
                                }
                            }
                        }
                    }

                    NetworkInterface iface = null;

                    if (!interfaces.TryGetValue (name, out iface)) {
                        iface = new NetworkInterface (name);
                        interfaces.Add (name, iface);
                    }

                    if (!address.Equals (IPAddress.None))
                        iface.AddAddress (address);

                    if (macAddress != null || type == NetworkInterfaceType.Loopback)
                        iface.SetLinkLayerInfo (index, macAddress, type);

                    next = addr.ifa_next;
                }
            } finally {
                freeifaddrs (ifap);
            }

            NetworkInterface [] result = new NetworkInterface [interfaces.Count];
            int x = 0;
            foreach (NetworkInterface thisInterface in interfaces.Values) {
                result [x] = thisInterface;
                x++;
            }
            return result;
        }

        [DllImport ("libc")]
        static extern int getifaddrs (out IntPtr ifap);

        [DllImport ("libc")]
        static extern void freeifaddrs (IntPtr ifap);

        const int AF_INET  = 2;
        const int AF_INET6 = 30;
        const int AF_LINK  = 18;

        private NetworkInterface(string aName)
        {
            iName = aName;
            iAddresses = new List<IPAddress>();
        }

        private void AddAddress(IPAddress aAddress)
        {
            iAddresses.Add(aAddress);
        }

        private void SetLinkLayerInfo(int aIndex, byte[] aMacAddress, NetworkInterfaceType aType)
        {
            //this.index = aIndex;
            iMacAddress = aMacAddress;
            iType = aType;
        }

        public string Name
        {
            get { return iName; }
        }

        public string Description
        {
            get { return iName; }
        }

        public PhysicalAddress GetPhysicalAddress()
        {
            if (iMacAddress != null)
                return new PhysicalAddress(iMacAddress);
            else
                return PhysicalAddress.None;
        }

        public bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
        {
            bool wantIPv4 = networkInterfaceComponent == NetworkInterfaceComponent.IPv4;
            bool wantIPv6 = wantIPv4 ? false : networkInterfaceComponent == NetworkInterfaceComponent.IPv6;
                
            foreach (IPAddress address in iAddresses) {
                if (wantIPv4 && address.AddressFamily == AddressFamily.InterNetwork)
                    return true;
                else if (wantIPv6 && address.AddressFamily == AddressFamily.InterNetworkV6)
                    return true;
            }
            
            return false;
        }

        public NetworkInterfaceType NetworkInterfaceType
        {
            get { return iType; }
        }

        public bool SupportsMulticast
        {
            get { return false; }
        }

        public bool IsReceiveOnly
        {
            get { return false; }
        }

        public OperationalStatus OperationalStatus
        {
            get { return OperationalStatus.Unknown; }
        }

        public IPInterfaceProperties GetIPProperties()
        {
            if (iInterfaceProperties == null) {
                iInterfaceProperties = new IPInterfaceProperties(iName, iAddresses);
            }
            return iInterfaceProperties;
        }

        private string iName;
        private List<IPAddress> iAddresses;
        private byte[] iMacAddress;
        private NetworkInterfaceType iType;
        private IPInterfaceProperties iInterfaceProperties;
    }


    class IPInterfaceProperties
    {
        public IPInterfaceProperties(string aInterfaceName, List<IPAddress> aAddresses)
        {
            iInterfaceName = aInterfaceName;
            iAddresses = aAddresses;
        }

        public IPv4InterfaceProperties GetIPv4Properties()
        {
            if (iIPv4Properties == null) {
                iIPv4Properties = new IPv4InterfaceProperties();
            }
            return iIPv4Properties;
        }

        public bool IsDnsEnabled
        {
            get { return true; }
        }

        public bool IsDynamicDnsEnabled
        {
            get { return false; }
        }

        public List<IPAddress> UnicastAddresses
        {
            get
            {
                List<IPAddress> unicastAddresses = new List<IPAddress>();
                foreach (IPAddress address in iAddresses)
                {
                    switch (address.AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            byte top = address.GetAddressBytes()[0];
                            if (top >= 224 && top <= 239)
                                continue;
                            unicastAddresses.Add(address);
                            break;

                        case AddressFamily.InterNetworkV6:
                            if (address.IsIPv6Multicast)
                                continue;
                            unicastAddresses.Add (address);
                            break;
                    }
                }
                return unicastAddresses;
            }
        }

        public string DnsSuffix
        {
            get
            {
                ParseResolvConf();
                return dns_suffix;
            }
        }

        public List<IPAddress> GatewayAddresses
        {
            get
            {
                ParseRouteInfo(iInterfaceName);
                return gateways;
            }
        }

        static Regex ns = new Regex (@"\s*nameserver\s+(?<address>.*)");
        static Regex search = new Regex (@"\s*search\s+(?<domain>.*)");
        private void ParseResolvConf ()
        {
            try {
                DateTime wt = File.GetLastWriteTime ("/etc/resolv.conf");
                if (wt <= last_parse)
                    return;

                last_parse = wt;
                dns_suffix = "";
                using (StreamReader reader = new StreamReader ("/etc/resolv.conf")) {
                    string str;
                    string line;
                    while ((line = reader.ReadLine ()) != null) {
                        line = line.Trim ();
                        if (line.Length == 0 || line [0] == '#')
                            continue;
                        Match match = ns.Match (line);
                        if (match.Success) {
                            try {
                                str = match.Groups ["address"].Value;
                                str = str.Trim ();
                            } catch {
                            }
                        } else {
                            match = search.Match (line);
                            if (match.Success) {
                                str = match.Groups ["domain"].Value;
                                string [] parts = str.Split (',');
                                dns_suffix = parts [0].Trim ();
                            }
                        }
                    }
                }
            } catch {
            } finally {
            }
        }

        void ParseRouteInfo (string iface)
        {
            try {
                gateways = new List<IPAddress>();
                using (StreamReader reader = new StreamReader ("/proc/net/route")) {
                    string line;
                    reader.ReadLine (); // Ignore first line
                    while ((line = reader.ReadLine ()) != null) {
                        line = line.Trim ();
                        if (line.Length == 0)
                            continue;

                        string [] parts = line.Split ('\t');
                        if (parts.Length < 3)
                            continue;
                        string gw_address = parts [2].Trim ();
                        byte [] ipbytes = new byte [4];  
                        if (gw_address.Length == 8 && iface.Equals (parts [0], StringComparison.OrdinalIgnoreCase)) {
                            for (int i = 0; i < 4; i++) {
                                if (!Byte.TryParse (gw_address.Substring (i * 2, 2), NumberStyles.HexNumber, null, out ipbytes [3 - i]))
                                    continue;
                            }
                            IPAddress ip = new IPAddress (ipbytes);
                            if (!ip.Equals (IPAddress.Any))
                                gateways.Add (ip);
                        }
                    }
                }
            } catch {
            }
        }

        private string iInterfaceName;
        private List<IPAddress> iAddresses;
        private IPv4InterfaceProperties iIPv4Properties;
        private DateTime last_parse;
        private string dns_suffix;
        private List<IPAddress> gateways;
    }

    class IPv4InterfaceProperties
    {
        public bool IsDhcpEnabled
        {
            get { return false; }
        }

        public bool IsAutomaticPrivateAddressingEnabled
        {
            get { return false; }
        }

        public int Mtu
        {
            get { return 0; }
        }
    }

    namespace MacOsStructs
    {
        internal struct ifaddrs
        {
            public IntPtr  ifa_next;
            public string  ifa_name;
            public uint    ifa_flags;
            public IntPtr  ifa_addr;
            public IntPtr  ifa_netmask;
            public IntPtr  ifa_dstaddr;
            public IntPtr  ifa_data;
        }

        internal struct sockaddr
        {
            public byte  sa_len;
            public byte  sa_family;
        }

        internal struct sockaddr_in
        {
            public byte   sin_len;
            public byte   sin_family;
            public ushort sin_port;
            public uint   sin_addr;
        }

        internal struct in6_addr
        {
            [MarshalAs (UnmanagedType.ByValArray, SizeConst=16)]
            public byte[] u6_addr8;
        }

        internal struct sockaddr_in6
        {
            public byte     sin6_len;
            public byte     sin6_family;
            public ushort   sin6_port;
            public uint     sin6_flowinfo;
            public in6_addr sin6_addr;
            public uint     sin6_scope_id;
        }

        internal struct sockaddr_dl
        {
            public byte   sdl_len;
            public byte   sdl_family;
            public ushort sdl_index;
            public byte   sdl_type;
            public byte   sdl_nlen;
            public byte   sdl_alen;
            public byte   sdl_slen;

            [MarshalAs (UnmanagedType.ByValArray, SizeConst=12)]
            public byte[] sdl_data;
        }

    }

    internal enum MacOsArpHardware
    {
        ETHER = 0x6,
        ATM = 0x25,
        SLIP = 0x1c,
        PPP = 0x17,
        LOOPBACK = 0x18,
        FDDI = 0xf
    }


}
