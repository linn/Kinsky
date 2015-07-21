using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
//using System.Web.Services;
using System.Web.Services.Protocols;
using System.Threading;
using System.Net;
using System.IO;

using Linn.Control;

namespace Linn.ControlPoint
{
    public interface IInvokeHandler
    {
        void WriteBegin();
        void WriteArgumentString(string aName, string aValue);
        void WriteArgumentUint(string aName, uint aValue);
        void WriteArgumentBool(string aName, bool aValue);
        void WriteArgumentInt(string aName, int aValue);
        void WriteArgumentBinary(string aName, byte[] aValue);
        object WriteEnd(GetResponseStreamCallback aCallback);

        void ReadBegin(object aResult);
        string ReadArgumentString(string aName);
        uint ReadArgumentUint(string aName);
        bool ReadArgumentBool(string aName);
        int ReadArgumentInt(string aName);
        byte[] ReadArgumentBinary(string aName);
        void ReadEnd();
    }

    public interface IProtocol
    {
        IInvokeHandler CreateInvokeHandler(Service aService, Action aAction);
    }

    public class ServiceException : Exception
    {
        public ServiceException(int aCode, string aDescription)
            : base("Code=" + aCode.ToString() + " Description=\"" + aDescription + "\"")
        {
            Code = aCode;
            Description = aDescription;
        }

        public int Code;
        public string Description;
    }

    public class EventArgsError : EventArgs
    {
        public EventArgsError(int aCode, string aDescription)
        {
            Code = aCode;
            Description = aDescription;
        }

        public int Code;
        public string Description;
    }

    public class ServiceLocation
    {
        protected ServiceLocation()
        {
            iDictionary = new Dictionary<string,string>();
        }

        public string Find(string aKey)
        {
            string value;

            iDictionary.TryGetValue(aKey, out value);

            if (value == null)
            {
                throw (new ServiceException(901, "Incompatible Service Location"));
            }

            return (value);
        }

        public override string ToString()
        {
            string result = "ServiceLocation{";

            foreach (string key in iDictionary.Keys)
            {
                result += key;
                result += "{";
                result += iDictionary[key];
                result += "}";
            }

            result += "}";

            return (result);
        }

        protected Dictionary<string, string> iDictionary;
    }

    public abstract class Service
    {

        protected const int kCommsError = 300;

        protected Service(Device aDevice, ServiceType aType, IProtocol aProtocol)
        {
            iDevice = aDevice;
            iType = aType;
            iProtocol = aProtocol;

            iActions = new List<Action>();

            try
            {
                iLocation = Device.FindServiceLocation(aType);
            }
            catch (DeviceException)
            {
                throw (new ServiceException(902, "Device failure"));
            }
        }

        public abstract void Close();
        public abstract void Kill();

        public void AddAction(Action aAction)
        {
            iActions.Add(aAction);
        }

        public int GetActionCount()
        {
            return iActions.Count;
        }

        public Action GetActionAt(int aIndex)
        {
            return iActions[aIndex];
        }

        public override string ToString()
        {
            return ("Service{" + Device + Type + Location + "}");
        }

        public Device Device
        {
            get
            {
                return (iDevice);
            }
        }

        public ServiceLocation Location
        {
            get
            {
                return (iLocation);
            }
        }

        public ServiceType Type
        {
            get
            {
                return (iType);
            }
        }

        public IProtocol Protocol
        {
            get
            {
                return iProtocol;
            }
        }

        protected List<Action> iActions;

        private Device iDevice;
        private ServiceType iType;
        private ServiceLocation iLocation;

        private IProtocol iProtocol;
    }

    public class Argument
    {
        public enum EType
        {
            eString,
            eUint,
            eBool,
            eInt,
            eBinary
        }

        public Argument(string aName, EType aType)
        {
            iName = aName;
            iType = aType;
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        public EType Type
        {
            get
            {
                return iType;
            }
        }

        private string iName;
        private EType iType;
    }

    public class Action
    {
        public Action(string aName)
        {
            iName = aName;

            iInArguments = new List<Argument>();
            iOutArguments = new List<Argument>();
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        public void AddInArgument(Argument aArgument)
        {
            iInArguments.Add(aArgument);
        }

        public int GetInArgumentCount()
        {
            return iInArguments.Count;
        }

        public Argument GetInArgumentAt(int aIndex)
        {
            return iInArguments[aIndex];
        }

        public void AddOutArgument(Argument aArgument)
        {
            iOutArguments.Add(aArgument);
        }

        public int GetOutArgumentCount()
        {
            return iOutArguments.Count;
        }

        public Argument GetOutArgumentAt(int aIndex)
        {
            return iOutArguments[aIndex];
        }

        private string iName;
        private List<Argument> iInArguments;
        private List<Argument> iOutArguments;
    }

    public class EventServerException : Exception
    {
    }

    public abstract class EventServer
    {
        public abstract void Start(IPAddress aInterface);
        public abstract void Stop();
        public abstract IPAddress Interface { get; }
    }
}
