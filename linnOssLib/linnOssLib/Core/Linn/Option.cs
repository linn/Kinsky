using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Linn
{
    public abstract class Option
    {
        protected Option(string aId, string aName, string aDescription)
        {
            iId = aId;
            iName = aName;
            iDescription = aDescription;
            iAttributes = new Dictionary<string, string>();
            iEnabled = true;
        }

        public string Id
        {
            get
            {
                return iId;
            }
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        public string Description
        {
            get
            { 
                return iDescription; 
            }
        }

        public IDictionary<string, string> Attributes
        {
            get
            {
                return (iAttributes);
            }
        }

        public void AddAttribute(string aKey, string aValue)
        {
            iAttributes.Add(aKey, aValue);
        }

        public virtual IList<string> Allowed
        {
            get
            {
                return (null);
            }
        }

        public bool Enabled {
            get
            {
                return iEnabled;
            }
            set
            {
                iEnabled = value;
                if (EventEnabledChanged != null)
                {
                    EventEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> EventEnabledChanged;

        public abstract void ResetToDefault();
        public abstract string Value { get; }
        public abstract bool Set(string aValue);

        public abstract event EventHandler<EventArgs> EventValueChanged;
        public abstract event EventHandler<EventArgs> EventAllowedChanged;

        private string iId;
        private string iName;
        private string iDescription;
        private Dictionary<string, string> iAttributes;
        private bool iEnabled;
    }

    public abstract class OptionSimple<T> : Option
    {
        public OptionSimple(string aId, string aName, string aDescription, T aDefault)
            : base(aId, aName, aDescription)
        {
            iDefault = aDefault;
            iValue = iDefault;
            AddAttribute("Default", iDefault.ToString());
        }

        public override void ResetToDefault()
        {
            Set(iDefault.ToString());
        }

        public override string Value
        {
            get
            {
                return (iValue.ToString());
            }
        }

        EventHandler<EventArgs> iEventValueChanged;
        public override event EventHandler<EventArgs> EventValueChanged { add { iEventValueChanged += value; } remove { iEventValueChanged -= value; } }

        public override IList<string> Allowed
        {
            get
            {
                return (null);
            }
        }

        public override event EventHandler<EventArgs> EventAllowedChanged { add { } remove { } }

        protected void Update(T aValue)
        {
            if (iValue.ToString() != aValue.ToString())
            {
                iValue = aValue;

                if (iEventValueChanged != null)
                {
                    iEventValueChanged(this, EventArgs.Empty);
                }
            }
        }

        public T Native
        {
            get
            {
                return (iValue);
            }
            set
            {
                Update(value);
            }
        }

        private T iDefault;
        private T iValue;
    }


    public class OptionString : OptionSimple<string>
    {
        public OptionString(string aId, string aName, string aDescription, string aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public override bool Set(string aValue)
        {
            Update(aValue);
            return (true);
        }
    }

    public class OptionFilePath : OptionString
    {
        public OptionFilePath(string aId, string aName, string aDescription, string aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }
    }

    public class OptionFolderPath : OptionString
    {
        public OptionFolderPath(string aId, string aName, string aDescription, string aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }
    }

    public class OptionFolderName : OptionString
    {
        public OptionFolderName(string aId, string aName, string aDescription, string aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }
    }

    public class OptionUri : OptionString
    {
        public OptionUri(string aId, string aName, string aDescription, string aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public void IncludeTestButton() {
            AddAttribute("TestButton", "True");
        }
    }

    public class OptionBool : OptionSimple<bool>
    {
        public OptionBool(string aId, string aName, string aDescription, bool aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public override bool Set(string aValue)
        {
            // note the TryParse method is absent in the compact framework
            try
            {
	            Update(bool.Parse(aValue));
	            return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public class OptionBoolEnum : Option
    {
        public OptionBoolEnum(string aId, string aName, string aDescription, bool aDefault, string aTrueString, string aFalseString)
            : base(aId, aName, aDescription)
        {
            iAllowed = new List<string>() { aTrueString, aFalseString };
            if (aDefault) {
                iDefaultIndex = 0;
            }
            else {
                iDefaultIndex = 1;
            }
            iValueIndex = iDefaultIndex;
        }

        public OptionBoolEnum(string aId, string aName, string aDescription, bool aDefault)
            : this(aId, aName, aDescription, aDefault, "True", "False")
        {
        }

        public bool Native {
            get {
                if (iValueIndex == 0) {
                    return true;
                }
                return false;
            }
            set {
                if (value) {
                    Set(iAllowed[0]);
                }
                else {
                    Set(iAllowed[1]);
                }
            }
        }

        public override void ResetToDefault() {
            Assert.Check(iAllowed.Count > iDefaultIndex);
            Set(iAllowed[iDefaultIndex]);
        }

        public override string Value {
            get {
                return (iAllowed[iValueIndex]);
            }
        }

        public override bool Set(string aValue) {
            int index = iAllowed.IndexOf(aValue);

            if (index < 0) {
                return (false);
            }

            if (iValueIndex != index) {
                iValueIndex = index;

                if (EventValueChanged != null) {
                    EventValueChanged(this, EventArgs.Empty);
                }
            }

            return (true);
        }

        public override event EventHandler<EventArgs> EventValueChanged;

        public override IList<string> Allowed {
            get {
                return (iAllowed.AsReadOnly());
            }
        }

        public override event EventHandler<EventArgs> EventAllowedChanged { add { } remove { } }

        private List<string> iAllowed;
        private int iDefaultIndex;
        private int iValueIndex;
    }

    public class OptionInt : OptionSimple<int>
    {
        public OptionInt(string aId, string aName, string aDescription, int aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public override bool Set(string aValue)
        {
            // note the TryParse method is absent in the compact framework
            try
            {
                Update(int.Parse(aValue, System.Globalization.CultureInfo.InvariantCulture));
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public class OptionColor : OptionInt
    {
        public OptionColor(string aId, string aName, string aDescription, int aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }
    }

    public class OptionUint : OptionSimple<uint>
    {
        public OptionUint(string aId, string aName, string aDescription, uint aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public override bool Set(string aValue)
        {
            // note the TryParse method is absent in the compact framework
            try
            {
                Update(uint.Parse(aValue, System.Globalization.CultureInfo.InvariantCulture));
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public class OptionFloat : OptionSimple<float>
    {
        public OptionFloat(string aId, string aName, string aDescription, float aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public override bool Set(string aValue)
        {
            // note the TryParse method is absent in the compact framework
            try
            {
                Update(float.Parse(aValue, System.Globalization.CultureInfo.InvariantCulture));
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
    public class OptionDouble : OptionSimple<double>
    {
        public OptionDouble(string aId, string aName, string aDescription, double aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        public override bool Set(string aValue)
        {
            // note the TryParse method is absent in the compact framework
            try
            {
                Update(double.Parse(aValue, System.Globalization.CultureInfo.InvariantCulture));
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
    public class OptionNumber : OptionSimple<int>
    {
        public OptionNumber(string aId, string aName, string aDescription, int aDefault, int aMin, int aMax)
            : base(aId, aName, aDescription, aDefault)
        {
            iMin = aMin;
            iMax = aMax;
            AddAttribute("Min", iMin.ToString());
            AddAttribute("Max", iMax.ToString());
        }

        public override bool Set(string aValue)
        {
            // note the TryParse method is absent in the compact framework
            try
            {
                int value = int.Parse(aValue, System.Globalization.CultureInfo.InvariantCulture);
                if (value >= iMin && value <= iMax)
                {
                    Update(value);
                    return (true);
                }
            }
            catch (FormatException)
            {
            }
            return (false);
        }

        private int iMin;
        private int iMax;
    }

    public abstract class OptionListSimple<T> : Option
    {
        public OptionListSimple(string aId, string aName, string aDescription)
            : this(aId, aName, aDescription, new List<T>())
        {
        }

        public OptionListSimple(string aId, string aName, string aDescription, IList<T> aDefault)
            : base(aId, aName, aDescription)
        {
            iDefault = aDefault;
            iValue = iDefault;
        }

        public override void ResetToDefault()
        {
            Set(StringListConverter.ListToString(NativeToList(iDefault)));
        }

        public override bool Set(string aValue)
        {
            try
            {
                Update(ListToNative(StringListConverter.StringToList(aValue)));
                return (true);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public override string Value
        {
            get
            {
                return StringListConverter.ListToString(NativeToList(iValue));
            }
        }

        EventHandler<EventArgs> iEventValueChanged;
        public override event EventHandler<EventArgs> EventValueChanged { add { iEventValueChanged += value; } remove { iEventValueChanged -= value; } }

        public override IList<string> Allowed
        {
            get
            {
                return (null);
            }
        }

        public override event EventHandler<EventArgs> EventAllowedChanged { add { } remove { } }

        protected abstract T ToNative(string aValue);
        protected abstract string ToString(T aValue);

        private IList<T> ListToNative(IList<string> aList)
        {
            IList<T> list = new List<T>();
            foreach(string s in aList)
            {
                list.Add(ToNative(s));
            }
            return list;
        }

        private IList<string> NativeToList(IList<T> aList)
        {
            IList<string> list = new List<string>();
            foreach(T v in aList)
            {
                list.Add(ToString(v));
            }
            return list;
        }

        protected void Update(IList<T> aValue)
        {
            if (StringListConverter.ListToString(NativeToList(iValue)) != StringListConverter.ListToString(NativeToList(aValue)))
            {
                iValue = aValue;

                if (iEventValueChanged != null)
                {
                    iEventValueChanged(this, EventArgs.Empty);
                }
            }
        }

        public IList<T> Native
        {
            get
            {
                return (iValue);
            }
            set
            {
                Update(value);
            }
        }

        private IList<T> iDefault;
        private IList<T> iValue;
    }

    public class OptionListString : OptionListSimple<string>
    {
        public OptionListString(string aId, string aName, string aDescription)
            : this(aId, aName, aDescription, new List<string>())
        {
        }

        public OptionListString(string aId, string aName, string aDescription, IList<string> aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        protected override string ToNative(string aValue)
        {
            return aValue;
        }

        protected override string ToString(string aValue)
        {
            return aValue;
        }

        public void Add(string aValue)
        {
            List<string> list = StringListConverter.StringToList(Value);
            list.Add(aValue);
            Update(list);
        }

        public void Remove(string aValue)
        {
            List<string> list = StringListConverter.StringToList(Value);
            list.Remove(aValue);
            Update(list);
        }

        public void Clear()
        {
            Update(new List<string>());
        }
    }

    public class OptionListFolderPath : OptionListString
    {
        public OptionListFolderPath(string aId, string aName, string aDescription)
            : base(aId, aName, aDescription)
        {
        }

        public OptionListFolderPath(string aId, string aName, string aDescription, IList<string> aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }
    }

    public class OptionListUri : OptionListSimple<Uri>
    {
        public OptionListUri(string aId, string aName, string aDescription)
            : base(aId, aName, aDescription)
        {
        }

        public OptionListUri(string aId, string aName, string aDescription, IList<Uri> aDefault)
            : base(aId, aName, aDescription, aDefault)
        {
        }

        protected override Uri ToNative(string aValue)
        {
            return new Uri(aValue);
        }

        protected override string ToString(Uri aValue)
        {
            return aValue.OriginalString;
        }
    }

    public class OptionEnum : Option
    {
        public OptionEnum(string aId, string aName, string aDescription)
            : base(aId, aName, aDescription)
        {
            iAllowed = new List<string>();
            iDefaultIndex = 0;
            iValueIndex = 0;
        }

        public void Add(string aAllowed)
        {
            iAllowed.Add(aAllowed);
            if (EventAllowedChanged != null)
            {
                EventAllowedChanged(this, EventArgs.Empty);
            }
        }

        public void AddDefault(string aAllowed)
        {
            iDefaultIndex = iAllowed.Count;
            iValueIndex = iDefaultIndex;
            iAllowed.Add(aAllowed);
            if (EventAllowedChanged != null)
            {
                EventAllowedChanged(this, EventArgs.Empty);
            }
        }

        public override void ResetToDefault()
        {
            Assert.Check(iAllowed.Count > iDefaultIndex);
            Set(iAllowed[iDefaultIndex]);
        }

        public override string Value
        {
            get
            {
                return (iAllowed[iValueIndex]);
            }
        }

        public override bool Set(string aValue)
        {
            int index = iAllowed.IndexOf(aValue);

            if (index < 0)
            {
                return (false);
            }

            if (iValueIndex != index)
            {
                iValueIndex = index;

                if (EventValueChanged != null)
                {
                    EventValueChanged(this, EventArgs.Empty);
                }
            }

            return (true);
        }

        public override event EventHandler<EventArgs> EventValueChanged;

        public override IList<string> Allowed
        {
            get
            {
                return (iAllowed.AsReadOnly());
            }
        }

        public override event EventHandler<EventArgs> EventAllowedChanged;

        private List<string> iAllowed;
        private int iDefaultIndex;
        private int iValueIndex;
    }

    public class OptionNetworkInterface : Option, IDisposable
    {
        public OptionNetworkInterface(string aId)
            : base(aId, kName, "Please select the network interface (adapter) you wish to use from the list of available network interfaces")
        {
            iInterface = new NetworkInterface();

            NetworkChanged();

            iNetworkChangeWatcher = new NetworkChangeWatcher();
            iNetworkChangeWatcher.EventNetworkChanged += NetworkChangedHandler;
        }

        private void NetworkChangedHandler(object sender, EventArgs e)
        {
            NetworkChanged();
        }

        public void NetworkChanged()
        {
            NetworkInterface newInterface = null;

            lock (this)
            {
                // rebuild list of available interfaces
                iAllowedInterfaces = new List<NetworkInterface>();
                iAllowedInterfaces.Add(new NetworkInterface());

                foreach (NetworkInfoModel i in NetworkInfo.GetAllNetworkInterfaces())
                {
                    if (i.OperationalStatus == EOperationalStatus.eUp ||
                        i.OperationalStatus == EOperationalStatus.eUnknown)
                    {
                        // don't add 3G network card on iPads/iPhones
                        if(i.Name != "pdp_ip0")
                        {
                            iAllowedInterfaces.Add(new NetworkInterface(i));
                        }
                    }
                }

                // send to user log
                string userLogMsg = DateTime.Now + ": Network interface event received. Available interfaces:";
                for (int i = 1; i < iAllowedInterfaces.Count; i++)
                {
                    userLogMsg += Environment.NewLine + DateTime.Now + ":     " + iAllowedInterfaces[i].ToString();
                }
                UserLog.WriteLine(userLogMsg);

                // set the default interface - if there is only 1 real interface - set this as default
                if (iAllowedInterfaces.Count == 2)
                {
                    iDefault = iAllowedInterfaces[1];
                }
                else
                {
                    iDefault = iAllowedInterfaces[0];
                }

                // update the existing interface
                switch (iInterface.Status)
                {
                    case NetworkInterface.EStatus.eUnconfigured:
                        if (iDefault.Status != NetworkInterface.EStatus.eUnconfigured)
                        {
                            // interface unconfigured -> available/unavailable default
                            newInterface = iDefault;
                        }
                        break;

                    case NetworkInterface.EStatus.eUnavailable:
                        {
                            NetworkInterface found = FindInterface(iInterface.Name);
                            if (found != null && found.Status == NetworkInterface.EStatus.eAvailable)
                            {
                                // interface unavailable -> available
                                newInterface = found;
                            }
                        }
                        break;

                    case NetworkInterface.EStatus.eAvailable:
                        {
                            NetworkInterface found = FindInterface(iInterface.Name);
                            if (found != null)
                            {
                                if (found.Status == NetworkInterface.EStatus.eAvailable)
                                {
                                    if (!iInterface.Info.IPAddress.Equals(found.Info.IPAddress))
                                    {
                                        // interface available -> available (ip address change)
                                        newInterface = found;
                                    }
                                }
                                else
                                {
                                    // interface available -> unavailable
                                    newInterface = found;
                                }
                            }
                            else
                            {
                                // interface available -> unavailable
                                newInterface = new NetworkInterface(iInterface.Name);
                            }
                        }
                        break;
                }

                if (newInterface != null)
                {
                    iInterface = newInterface;
                }

                Trace.WriteLine(Trace.kCore, "OptionNetworkInterface.NetworkChange(): Network interface event - " + iInterface + " -> " + newInterface);
                UserLog.WriteLine(DateTime.Now + ": Network interface event - " + iInterface + " -> " + newInterface);
            }

            EventHandler<EventArgs> eventAllowedChanged = EventAllowedChanged;
            if (eventAllowedChanged != null)
            {
                eventAllowedChanged(this, EventArgs.Empty);
            }

            EventHandler<EventArgs> eventValueChanged = EventValueChanged;
            if (newInterface != null && eventValueChanged != null)
            {
                eventValueChanged(this, EventArgs.Empty);
            }
        }

        public override void ResetToDefault()
        {
            EventHandler<EventArgs> handler = null;

            lock (this)
            {
                if (iInterface.Name != iDefault.Name)
                {
                    iInterface = iDefault;
                    handler = EventValueChanged;
                }
            }

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public override string Value
        {
            get
            {
                lock (this)
                {
                    return (iInterface.Name);
                }
            }
        }

        public override bool Set(string aValue)
        {
            EventHandler<EventArgs> handler = null;

            lock (this)
            {
                if (aValue != iInterface.Name)
                {
                    NetworkInterface found = FindInterface(aValue);
                    if (found != null)
                    {
                        iInterface = found;
                    }
                    else
                    {
                        iInterface = new NetworkInterface(aValue);
                    }
                    handler = EventValueChanged;
                }
            }

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            return (true);
        }

        public override event EventHandler<EventArgs> EventValueChanged;

        public override IList<string> Allowed
        {
            get
            {
                lock (this)
                {
                    List<string> allowed = new List<string>();
                    foreach (NetworkInterface i in iAllowedInterfaces)
                    {
                        allowed.Add(i.Name);
                    }
                    return (allowed.AsReadOnly());
                }
            }
        }

        public override event EventHandler<EventArgs> EventAllowedChanged;

        public NetworkInterface Interface
        {
            get
            {
                lock (this)
                {
                    return (iInterface);
                }
            }
        }

        #region IDisposable implementation
        public void Dispose ()
        {
            iNetworkChangeWatcher.EventNetworkChanged -= NetworkChangedHandler;
            iNetworkChangeWatcher.Dispose();
        }
        #endregion

        public NetworkInterface FindInterface(string aName)
        {
            foreach (NetworkInterface i in iAllowedInterfaces)
            {
                if (i.Name == aName)
                {
                    return i;
                }
            }
            return null;
        }

        public const string kName = "Network Interface";

        private List<NetworkInterface> iAllowedInterfaces;
        private NetworkInterface iInterface;
        private NetworkInterface iDefault;
        private NetworkChangeWatcher iNetworkChangeWatcher;
    }

    public class StringListConverter
    {
        public static List<string> StringToList(string aValue)
        {
            List<string> list = new List<string>();
            StringReader sr = new StringReader(aValue);
            XmlReader reader = XmlReader.Create(sr);

            try
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        list.Add(reader.ReadContentAsString());
                    }
                }
            }
            catch (XmlException)
            {
            }

            reader.Close();
            return (list);
        }

        public static string ListToString(IList<string> aValue)
        {
            StringBuilder value = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(value);

            writer.WriteStartElement("List");
            foreach (string s in aValue)
            {
                writer.WriteElementString("Entry", s);
            }
            writer.WriteEndElement();
            writer.Close();

            return (value.ToString());
        }
    }
}
