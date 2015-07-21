using System.Net;
using System.Collections.Generic;

namespace Linn
{
    public class NetworkInterface
    {
        public enum EStatus
        {
            eUnconfigured,
            eUnavailable,
            eAvailable
        }

        public NetworkInterface()
        {
            Name = "None";
            Status = EStatus.eUnconfigured;
            Info = null;
        }

        public NetworkInterface(string aName)
        {
            Name = aName;
            Status = EStatus.eUnavailable;
            Info = null;
        }

        public NetworkInterface(NetworkInfoModel aInfo)
        {
            Name = aInfo.Name;
            Info = aInfo;

            if (aInfo.IPAddress.Equals(IPAddress.Any) || aInfo.IPAddress.Equals(IPAddress.IPv6Any) ||
                aInfo.IPAddress.Equals(IPAddress.None) || aInfo.IPAddress.Equals(IPAddress.IPv6None))
            {
                Status = EStatus.eUnavailable;
            }
            else
            {
                Status = EStatus.eAvailable;
            }
        }

        public override string ToString()
        {
            switch (Status)
            {
                case EStatus.eAvailable:
                    return "(" + Name + ", " + Info.IPAddress.ToString() + ")";

                case EStatus.eUnavailable:
                    return "(" + Name + ", unavailable)";

                case EStatus.eUnconfigured:
                    return "(Interface unconfigured)";

                default:
                    Assert.Check(false);
                    return string.Empty;
            }
        }

        // all members are public-readonly for this immutable class
        public readonly string Name;
        public readonly EStatus Status;
        public readonly NetworkInfoModel Info;
    }

	public enum ENetworkInterfaceType
    {
        	eLoopBack
        	,eTokenRing
        	,eSlip
        	,ePpp
        	,eEthernet
        	,eFddi
        	,eUnknown
    }
	
	public enum ENetworkInterfaceComponent {
    	eIPv4
    	,eIPv6
    	,eUnknown	
    }
    
    public enum EOperationalStatus {
    	eUp = 1,
    	eDown = 2,
    	eTesting = 3,
    	eUnknown = 4,
    	eDormant = 5,
    	eNotPresent = 6,
    	eLowerLayerDown = 7
    }
	    

    // This class describing a network interface is immutable
    public class NetworkInfoModel : System.IEquatable<NetworkInfoModel> {

        public bool Equals(NetworkInfoModel aOther)
        {
            return Name == aOther.Name;
        }

		public bool? IsDhcpEnabled {
			get {
				return iISDHCPEnabled;
			}
		}

		public string DnsSuffix {
			get {
				return iDnsSuffix;
			}
		}

        public IList<IPAddress> GatewayIPAddressInformation
        {
			get {
				return iGatewayIPAddressInformation.AsReadOnly();
			}
		}

		public bool? IsAutomaticPrivateAddressingEnabled {
			get {
				return iIsAutomaticPrivateAddressingEnabled;
			}
		}

		public bool? IsDnsEnabled {
			get {
				return iIsDnsEnabled;
			}
		}

		public bool? IsDynamicDnsEnabled {
			get {
				return iIsDynamicDnsEnabled;
			}
		}

		public int? Mtu {
			get {
				return iMtu;
			}
		}

		public EOperationalStatus OperationalStatus 
		{
			get 
			{
				return iOperationalStatus;
			}
		}

		public bool? IsReceiveOnly {
			get {
				return iIsReceiveOnly;
			}
		}

        public ENetworkInterfaceComponent NetworkInterfaceComponent
        {
            get
            {
                return iNetworkInterfaceComponent;
            }
        }

        public string Description
        {
            get
            {
                return iDescription;
            }
        }

        public bool? SupportsMulticast
        {
            get
            {
                return iSupportsMulticast;
            }
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        public ENetworkInterfaceType NetworkInterfaceType
        {
            get
            {
                return iNetworkInterfaceType;
            }
        }

        public IPAddress IPAddress
        {
            get
            {
                return iIPAddress;
            }
        }

        public string MacAddress
        {
            get
            {
                return iMacAddress;
            }
        }

        public NetworkInfoModel(string aAdapterDescription,
            string aAdapterName,
            ENetworkInterfaceType aNetworkInterfaceType,
            IPAddress aIPAddress,
            bool? aSupportsMulticast,
   			ENetworkInterfaceComponent aNetworkInterfaceComponent,
            string aMacAddress,
           	EOperationalStatus aOperationalStatus,
            bool? aIsReceiveOnly,
          	bool? aISDHCPEnabled,
          	bool? aIsAutomaticPrivateAddressingEnabled,
          	int? aMtu,
          	bool? aIsDnsEnabled,
          	bool? aIsDynamicDnsEnabled,
          	string aDnsSuffix,
          	List<IPAddress> aGatewayIPAddressInformation
           )
        {
        	iNetworkInterfaceComponent = aNetworkInterfaceComponent;
        	iSupportsMulticast = aSupportsMulticast;
            iDescription = aAdapterDescription;
            iName = aAdapterName;
            iNetworkInterfaceType = aNetworkInterfaceType;
            iIPAddress = aIPAddress;
        	iMacAddress = aMacAddress;
        	iOperationalStatus = aOperationalStatus;
        	iIsReceiveOnly = aIsReceiveOnly;
        	iISDHCPEnabled = aISDHCPEnabled;
        	iIsAutomaticPrivateAddressingEnabled = aIsAutomaticPrivateAddressingEnabled;
        	iMtu = aMtu;
        	iIsDnsEnabled = aIsDnsEnabled;
        	iIsDynamicDnsEnabled = aIsDynamicDnsEnabled;
        	iDnsSuffix = aDnsSuffix;
        	iGatewayIPAddressInformation = aGatewayIPAddressInformation;
        }

        // all members of this class are declared as readonly in order to assert that
        // this class is immutable
        private readonly bool? iISDHCPEnabled;
        private readonly string iDnsSuffix;
        private readonly List<IPAddress> iGatewayIPAddressInformation;
        private readonly bool? iIsAutomaticPrivateAddressingEnabled;
        private readonly bool? iIsDnsEnabled;
        private readonly bool? iIsDynamicDnsEnabled;
        private readonly int? iMtu;
        private readonly EOperationalStatus iOperationalStatus;
        private readonly bool? iIsReceiveOnly;
        private readonly ENetworkInterfaceComponent iNetworkInterfaceComponent;
        private readonly string iDescription;
        private readonly bool? iSupportsMulticast;
        private readonly string iName;
        private readonly ENetworkInterfaceType iNetworkInterfaceType;
        private readonly IPAddress iIPAddress;
        private readonly string iMacAddress;
    }

}
