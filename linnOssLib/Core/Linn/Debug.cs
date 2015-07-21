using System.IO;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace Linn
{

    //
    // Assert
    //
    public class AssertionError : Exception
    {
        public AssertionError() : base("ASSERT") { }
        public AssertionError(string aMessage) : base("ASSERT: " + aMessage) { }
    }

    public class Assert
    {
        public static void Check(bool aVal)
        {
            if (aVal == false)
            {
                throw new AssertionError();
            }
        }

        [Conditional("DEBUG")]
        public static void CheckDebug(bool aVal)
        {
            if (aVal == false)
            {
                throw new AssertionError();
            }
        }

        public static void Check(bool aVal, string aMessage)
        {
            if (aVal == false)
            {
                throw new AssertionError(aMessage);
            }
        }

        [Conditional("DEBUG")]
        public static void CheckDebug(bool aVal, string aMessage)
        {
            if (aVal == false)
            {
                throw new AssertionError(aMessage);
            }
        }
    }

    //
    // CrashLog classes
    //
    public class CrashLog
    {
        public CrashLog(Exception aException, OptionManager aOptionManager)
        {
            iException = aException;
            iUserLog = Linn.UserLog.Text;
            iSystemDetails = DebugInformation.SystemDetails();
            iOptions = aOptionManager.OptionValues;
            AssemblyInfoModel model = AssemblyInfo.GetAssemblyInfo();
            iProductDetails = string.Format("{0} - {1}", model.Product, model.InformationalVersion != string.Empty ? model.InformationalVersion : model.Version);
        }

        public override string ToString()
        {
            // Used to create a text version of the standard Linn crash log - other methods to get
            // access the individual components are below in case a crash log dumper wants to do something else
            string logText = iException.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            logText += "User Log:" + Environment.NewLine + Environment.NewLine;
            logText += iUserLog + Environment.NewLine + Environment.NewLine;
            logText += "Options:" + Environment.NewLine + Environment.NewLine;
            foreach (string aOption in iOptions.Keys)
            {
                logText += aOption + " : " + iOptions[aOption] + Environment.NewLine;
            }
            logText += Environment.NewLine + Environment.NewLine;
            logText += iSystemDetails;
            logText += "Product Details:" + Environment.NewLine + Environment.NewLine;
            logText += iProductDetails;
            logText += Environment.NewLine + Environment.NewLine;
            logText += "Crash date and time:" + Environment.NewLine + Environment.NewLine;
            logText += DateTime.Now.ToString();
            return logText;
        }

        public Exception Exception
        {
            get { return iException; }
        }

        public string UserLog
        {
            get { return iUserLog; }
        }

        public string SystemDetails
        {
            get { return iSystemDetails; }
        }

        public Dictionary<string, string> Options
        {
            get { return iOptions; }
        }

        private Exception iException;
        private string iUserLog;
        private string iSystemDetails;
        private string iProductDetails;
        private Dictionary<string, string> iOptions;
    }

    public interface ICrashLogDumper
    {
        void Dump(CrashLog aCrashLog);
    }

    public class CrashLogDumperStdout : ICrashLogDumper
    {
        public void Dump(CrashLog aCrashLog)
        {
            // For dumping to stdout, just write the exception
            Console.WriteLine(aCrashLog.Exception.ToString());
        }
    }

    public class CrashLogDumperFile : ICrashLogDumper
    {
        public CrashLogDumperFile(string aFilename)
        {
            iFilename = aFilename;
        }

        public void Dump(CrashLog aCrashLog)
        {
            try
            {
                StreamWriter f = new StreamWriter(iFilename);
                f.Write(aCrashLog.ToString());
                f.Close();
            }
            catch (System.UnauthorizedAccessException)
            {
                //if no permissions to write to log file 
            }
            catch (Exception)
            {
                Console.WriteLine("Unhandled Exception - cannot write to log file:");
                Console.WriteLine(aCrashLog.ToString());
            }
        }

        private string iFilename;
    }

    //
    // Tracing
    //
    public class TraceLevelError : System.Exception
    {
        public TraceLevelError(string aLevel)
            : base("Trace level " + aLevel + " not defined") { }
    }

    public abstract class TraceListener
    {
        public virtual void Write(object aObject)
        {
            Write(aObject.ToString());
        }

        public virtual void Write(string aString)
        {
        }

        public virtual void WriteLine(object aObject)
        {
            WriteLine(aObject.ToString());
        }

        public virtual void WriteLine(string aString)
        {
        }
    }

    public class Trace
    {
        public static readonly uint kNone = 0x00000000;
        public static readonly uint kTest = 0x00000001;
        public static readonly uint kUpnp = 0x00000002;
        public static readonly uint kGui = 0x00000004;
        public static readonly uint kConfigApp = 0x00000008;
        public static readonly uint kKinsky = 0x00000010;
        public static readonly uint kReflash = 0x00000020;
        public static readonly uint kCore = 0x00000040;
        public static readonly uint kKinskyEd = 0x00000080;
        public static readonly uint kDiscovery = 0x00000100;
        public static readonly uint kPreamp = 0x00000200;
        public static readonly uint kMediaRenderer = 0x00000400;
        public static readonly uint kMediaServer = 0x00000800;
        public static readonly uint kSdp = 0x00001000;
        public static readonly uint kTopology = 0x00002000;
        public static readonly uint kRendering = 0x00004000;
        public static readonly uint kPerformance = 0x00008000;
        public static readonly uint kControl = 0x00010000;
        public static readonly uint kInformation = 0x00020000;
        public static readonly uint kKinskyPda = 0x00040000;
        public static readonly uint kKinskyClassic = 0x00080000;
        public static readonly uint kKinskyDesktop = 0x00100000;
        public static readonly uint kKinskyWeb = 0x00200000;
        public static readonly uint kKinskyTouch = 0x00400000;
        public static readonly uint kAll = 0xffffffff;

        public static uint ConvertLevel(string aLevel)
        {
            if (aLevel == "kNone") { return kNone; }
            if (aLevel == "kTest") { return kTest; }
            if (aLevel == "kUpnp") { return kUpnp; }
            if (aLevel == "kGui") { return kGui; }
            if (aLevel == "kConfigApp") { return kConfigApp; }
            if (aLevel == "kKinsky") { return kKinsky; }
            if (aLevel == "kReflash") { return kReflash; }
            if (aLevel == "kCore") { return kCore; }
            if (aLevel == "kKinskyEd") { return kKinskyEd; }
            if (aLevel == "kDiscovery") { return kDiscovery; }
            if (aLevel == "kPreamp") { return kPreamp; }
            if (aLevel == "kMediaRenderer") { return kMediaRenderer; }
            if (aLevel == "kMediaServer") { return kMediaServer; }
            if (aLevel == "kSdp") { return kSdp; }
            if (aLevel == "kTopology") { return kTopology; }
            if (aLevel == "kRendering") { return kRendering; }
            if (aLevel == "kPerformance") { return kPerformance; }
            if (aLevel == "kControl") { return kControl; }
            if (aLevel == "kInformation") { return kInformation; }
            if (aLevel == "kKinskyPda") { return kKinskyPda; }
            if (aLevel == "kKinskyClassic") { return kKinskyClassic; }
            if (aLevel == "kKinskyDesktop") { return kKinskyDesktop; }
            if (aLevel == "kKinskyWeb") { return kKinskyWeb; }
            if (aLevel == "kAll") { return kAll; }
            throw new TraceLevelError(aLevel);
        }

        public static uint Level
        {
            get { return iLevel; }
            set { iLevel = value; }
        }

        [Conditional("TRACE")]
        public static void Write(uint aLevel, object aValue)
        {
            if ((iLevel & aLevel) == aLevel)
            {
                lock (iLock)
                {
                    foreach (TraceListener l in iListenerList)
                    {
                        try
                        {
                            l.Write(aValue);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        [Conditional("TRACE")]
        public static void Write(uint aLevel, string aMsg)
        {
            if ((iLevel & aLevel) == aLevel)
            {
                lock (iLock)
                {
                    foreach (TraceListener l in iListenerList)
                    {
                        try
                        {
                            l.Write(aMsg);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        [Conditional("TRACE")]
        public static void WriteLine(uint aLevel, object aValue)
        {
            if ((iLevel & aLevel) == aLevel)
            {
                lock (iLock)
                {
                    foreach (TraceListener l in iListenerList)
                    {
                        try
                        {
                            l.WriteLine(aValue);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        [Conditional("TRACE")]
        public static void WriteLine(uint aLevel, string aMsg)
        {
            if ((iLevel & aLevel) == aLevel)
            {
                lock (iLock)
                {
                    foreach (TraceListener l in iListenerList)
                    {
                        try
                        {
                            l.WriteLine(aMsg);
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        public static void Clear()
        {
            lock(iLock)
            {
                iListenerList.Clear();
            }
        }

        public static void AddListener(TraceListener aListener)
        {
            lock (iLock)
            {
                iListenerList.Add(aListener);
            }
        }

        public static void RemoveListener(TraceListener aListener)
        {
            lock (iLock)
            {
                iListenerList.Remove(aListener);
            }
        }

        private static object iLock = new object();
        private static uint iLevel = kAll;
        private static List<TraceListener> iListenerList = new List<TraceListener>();
    };


    public class TraceListenerConsole : TraceListener
    {
        public override void Write(string aMsg)
        {
            Console.Write(aMsg);
        }
        public override void WriteLine(string aMsg)
        {
            Console.WriteLine(aMsg);
        }
    }

    public class TraceListenerFile : TraceListener, IDisposable
    {
        public TraceListenerFile(string aBasePath)
        {
#if TRACE
            string exeNoExt = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
            string identifier = "0";//Process.GetCurrentProcess().Id.ToString();

            string filename = Path.Combine(aBasePath, exeNoExt) + "_" + identifier + ".log";

            try
            {
                iFile = new StreamWriter(filename);
            }
            catch (Exception) { }
#endif
        }

        public void Dispose()
        {
#if TRACE
            if (iFile != null)
            {
                iFile.Close();
            }
#endif
        }

        public override void Write(string aMsg)
        {
            if (iFile != null)
            {
                iFile.Write(aMsg);
            }
        }

        public override void WriteLine(string aMsg)
        {
            if (iFile != null)
            {
                iFile.WriteLine(aMsg);
            }
        }

        private StreamWriter iFile = null;
    }

}  // namespace Linn
