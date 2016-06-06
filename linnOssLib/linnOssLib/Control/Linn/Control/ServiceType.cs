using System;
using System.Collections.Generic;
using System.Text;

namespace Linn.Control
{
    public class ServiceType
    {
        public ServiceType(string aDomain, string aType, uint aVersion)
        {
            Assert.CheckDebug(aVersion > 0);
            Domain = aDomain;
            Type = aType;
            Version = aVersion;
        }

        public ServiceType(string aServiceString)
        {
            string[] tokens = aServiceString.Split(':');

            Assert.Check(tokens.Length == 5);

            Domain  = tokens[1];
            Type    = tokens[3];
            Version = UInt32.Parse(tokens[4]);
        }

        public bool IsSupportedBy(ServiceType aServiceType)
        {
            if (aServiceType.Domain == this.Domain)
            {
                if (aServiceType.Type == this.Type)
                {
                    if (aServiceType.Version >= this.Version)
                    {
                        return (true);
                    }
                }
            }
            return (false);
        }

        public override string ToString()
        {
            return ("ServiceType{" + Domain + ":" + Type + ":" + Version + "}");
        }

        public string Domain;
        public string Type;
        public uint Version;
    }
}
