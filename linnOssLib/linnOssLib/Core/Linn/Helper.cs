using System;
using System.Net;
using System.Net.Configuration;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Xamarin;

namespace Linn
{
    public interface IHelper : IDisposable
    {
        void ProcessCommandLine();
        void ProcessOptionsFileAndCommandLine();

        string Title { get; }
        string Version { get; }
        string Family { get; }
        string Description { get; }
        string Product { get; }
        string Copyright { get; }
        string Company { get; }

        DirectoryInfo DataPath { get; }
        DirectoryInfo ExePath { get; }

#warning TODO: OptionParser needs to be converted over to use the generic options system
        OptionParser OptionParser { get; }

        IList<IOptionPage> OptionPages { get; }

        OptionNetworkInterface Interface { get; }

        void AddOption(OptionParser.Option aOption);

        void AddOptionPage(IOptionPage aPage);
        void RemoveOptionPage(IOptionPage aPage);

        void AddOption(Option aOption);
        void RemoveOption(Option aOption);

        event EventHandler<EventArgs> EventOptionChanged;
        event EventHandler<EventArgs> EventOptionPagesChanged;

        void AddCrashLogDumper(ICrashLogDumper aDumper);
        void RemoveCrashLogDumper(ICrashLogDumper aDumper);

        void UnhandledException(Exception aException);

        event EventHandler<EventArgs> EventErrorOccurred;
    }


    public class Helper : IHelper
    {
        public Helper(string[] aArgs)
        {
            iAssemblyAttributes = Linn.AssemblyInfo.GetAssemblyInfo();
            iLock = new object();
            iErrorThrown = false;

            // create the data and exe paths
            iDataPath = SystemInfo.DataPathForApp(Product);

            if (!iDataPath.Exists)
            {
                try
                {
                    iDataPath.Create();
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message + "\n");
                }
            }

            iExePath = SystemInfo.ExePathForApp(Product);
            


            // add the unhandled exception handler - this should catch all unhandled
            // exceptions in all threads
#if !DEBUG && !TRACE
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
#endif

            // initialise crash log dumpers
            iCrashLogDumpers = new List<ICrashLogDumper>();
            iCrashLogDumpers.Add(new CrashLogDumperStdout());
            // only add 1 crash file for several instances of the app
            string[] filenames = System.IO.Directory.GetFiles(".", "*.crash");
            if (filenames.Length == 0)
            {
                string exeNoExt = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
                string crashFile = Path.Combine(iDataPath.FullName, exeNoExt) + ".crash";
                iCrashLogDumpers.Add(new CrashLogDumperFile(crashFile));
            }


            // add trace listeners
            Trace.AddListener(new TraceListenerConsole());
            
            // remove unused log files
            filenames = System.IO.Directory.GetFiles(iDataPath.FullName, "*.log");
            foreach (string file in filenames)
            {
                try
                {
                    // delete the file and ignore exceptions - file may be in use
                    File.Delete(file);
                }
                catch (IOException) { }
            }
            iTraceFile = new TraceListenerFile(iDataPath.FullName);
            Trace.AddListener(iTraceFile);


            // add user log listener
            try
            {
                iUserLogFile = new UserLogFile(iDataPath.FullName);
                UserLog.AddListener(iUserLogFile);
            }
            catch (IOException) { }

            UserLog.WriteLine(Product + " v" + Version + " (" + Family + ")");

            // log the mono version if being used
            Type t = Type.GetType("Mono.Runtime");
            if (t != null)
            {
                MethodInfo displayName = t.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                if (displayName != null) {
                    UserLog.WriteLine("Mono Version: " + displayName.Invoke(null, null));
                }
            }

            // create option parser
            iOptionParser = new OptionParser(aArgs);
            iOptionParser.Usage = "usage: " + Product;
            // add the trace level option
            iOptionTraceLevel = new OptionParser.OptionString("-t", "--tracelevel", "kNone",
                "Set the trace level for the application. Multiple levels specified in quotes.", "LEVEL1 [LEVEL2 LEVEL3 ...]");
            iOptionParser.AddOption(iOptionTraceLevel);

            iOptionNic = new OptionParser.OptionString("-i", "--interface", "0.0.0.0",
                "Sets the interface to bind to", "INTERFACE");
            iOptionParser.AddOption(iOptionNic);

            ServicePointManager.DefaultConnectionLimit = 50;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;

            iOptionPages = new List<IOptionPage>();
            iOptionManager = new OptionManager(Path.Combine(iDataPath.FullName, "Options.xml"));

            iOptionPage = new OptionPage("Network");
            iOptionNetworkInterface = new OptionNetworkInterface("interface");
            iOptionPage.Add(iOptionNetworkInterface);

            // create the network stack
            iStack = new Stack(iOptionNetworkInterface);
        }

        public void ProcessCommandLine()
        {
            // parse command line args
            try
            {
                iOptionParser.Parse();
            }
            catch (OptionParser.OptionParserError e)
            {
                Console.WriteLine(e.Message + "\n");
                iOptionParser.DisplayHelp();
            }

            // setup trace levels
            try
            {
                string[] traceLevels = iOptionTraceLevel.Value.Split(new char[] { ' ' });

                Trace.Level = Trace.kNone;
                foreach (string level in traceLevels)
                {
                    Trace.Level |= Trace.ConvertLevel(level);
                }

                if (Trace.Level != Trace.kNone)
                {
                    Console.WriteLine("Setting trace level to " + iOptionTraceLevel.Value);
                }
            }
            catch (TraceLevelError e)
            {
                Console.WriteLine(e.Message + "\n");
                iOptionParser.DisplayHelp();
            }

            iCommandLineInterface = GetInterface(iOptionNic.Value);
            if (iCommandLineInterface != null)
            {
                iOptionNetworkInterface.Set(iCommandLineInterface.Name);
            }

            Trace.WriteLine(Trace.kCore, "Helper.Start: ExePath=" + ExePath.FullName);
            Trace.WriteLine(Trace.kCore, "Helper.Start: DataPath=" + DataPath.FullName);
            Trace.WriteLine(Trace.kCore, "Helper.Start: Product=" + Product);
        }        

        public void ProcessOptionsFileAndCommandLine()
        {
#if !DEBUG && !TRACE
            Insights.DisableExceptionCatching = true;
#endif
            
            AddOptionPage(iOptionPage);
            ProcessCommandLine();
        }

        public void Dispose()
        {
            // remove trace listeners
            if (iTraceFile != null)
            {
                Trace.RemoveListener(iTraceFile);
                iTraceFile.Dispose();
                iTraceFile = null;
            }

            // remove user log listener
            if (iUserLogFile != null)
            {
                UserLog.RemoveListener(iUserLogFile);
                iUserLogFile.Dispose();
                iUserLogFile = null;
            }

            // dispose of network interface option
            if (iOptionNetworkInterface != null)
            {
                iOptionNetworkInterface.Dispose();
            }
        }

        public OptionParser OptionParser
        {
            get
            {
                return iOptionParser;
            }
        }

        public void AddOption(OptionParser.Option aOption)
        {
            iOptionParser.AddOption(aOption);
        }

        public IList<IOptionPage> OptionPages
        {
            get { lock (iOptionPages) { return new List<IOptionPage>(iOptionPages).AsReadOnly(); } }
        }

        public void AddOptionPage(IOptionPage aPage)
        {
            lock (iOptionPages)
            {
                if (aPage.Name == "General" || aPage.Name == "Network")
                {
                    if (iOptionPages.Count > 0 && iOptionPages[0].Name == "General")
                    {
                        iOptionPages.Insert(1, aPage);
                    }
                    else
                    {
                        iOptionPages.Insert(0, aPage);
                    }
                }
                else
                {
                    iOptionPages.Add(aPage);
                }
            }

            foreach (Option o in aPage.Options)
            {
                iOptionManager.Add(o);
            }

            aPage.EventOptionAdded += OptionPageOptionAdded;
            aPage.EventOptionRemoved += OptionPageOptionRemoved;
            aPage.EventChanged += OptionPageChanged;

            OnEventOptionPagesChanged();
        }

        public void RemoveOptionPage(IOptionPage aPage)
        {
            aPage.EventOptionAdded -= OptionPageOptionAdded;
            aPage.EventOptionRemoved -= OptionPageOptionRemoved;
            aPage.EventChanged -= OptionPageChanged;

            foreach (Option o in aPage.Options)
            {
                iOptionManager.Remove(o);
            }
            lock (iOptionPages)
            {
                iOptionPages.Remove(aPage);
            }
            OnEventOptionPagesChanged();
        }

        public void AddOption(Option aOption)
        {
            iOptionManager.Add(aOption);
        }

        public void RemoveOption(Option aOption)
        {
            iOptionManager.Remove(aOption);
        }

        public event EventHandler<EventArgs> EventOptionChanged;

        private void OptionPageOptionAdded(object sender, EventArgsOption e)
        {
            iOptionManager.Add(e.Option);
        }

        private void OptionPageOptionRemoved(object sender, EventArgsOption e)
        {
            iOptionManager.Remove(e.Option);
        }

        private void OptionPageChanged(object sender, EventArgs e)
        {
            if (EventOptionChanged != null)
            {
                EventOptionChanged(sender, e);
            }
        }

        public void AddCrashLogDumper(ICrashLogDumper aDumper)
        {
            iCrashLogDumpers.Add(aDumper);
        }

        public void RemoveCrashLogDumper(ICrashLogDumper aDumper)
        {
            iCrashLogDumpers.Remove(aDumper);
        }

        public void UnhandledException(Exception aException)
        {
            bool firstException = !iErrorThrown;

            lock (iLock)
            {
                if (!iErrorThrown)
                {
                    iErrorThrown = true;

                    // create and dump the crash log
                    CrashLog cl = new CrashLog(aException, iOptionManager);
                    foreach (ICrashLogDumper d in iCrashLogDumpers)
                    {
                        d.Dump(cl);
                    }

                    if (Insights.IsInitialized)
                    {
                        var data = new Dictionary<string, string> { { "Logs", UserLog.Text } };
                        string options = string.Empty;
                        foreach (var o in iOptionManager.OptionValues)
                        {
                            options += string.Format("{0} : {1}\n", o.Key, o.Value);
                        }
                        data.Add("Options", options);
                        Insights.Report(aException, data, Xamarin.Insights.Severity.Error);
                        Insights.PurgePendingCrashReports().Wait();
                    }
                }
            }

            // notify
            if (firstException && EventErrorOccurred != null)
            {
                EventErrorOccurred(this, EventArgs.Empty);
            }
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(e.ExceptionObject as Exception);
            // an unknown error has occurred that could be anywhere - no guarantee
            // that a clean exit can be performed - kill it
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch(Exception) { }
        }

        public event EventHandler<EventArgs> EventErrorOccurred;

        public string Title
        {
            get { return iAssemblyAttributes.Title; }
        }

        public string Version
        {
            get { return iAssemblyAttributes.Version; }
        }

        public string Family
        {
            get { return VersionSupport.Family(Version); }
        }

        public string Description
        {
            get { return iAssemblyAttributes.Description; }
        }

        public string Product
        {
            get { return iAssemblyAttributes.Product; }
        }

        public string Copyright
        {
            get { return iAssemblyAttributes.Copyright; }
        }

        public string Company
        {
            get { return iAssemblyAttributes.Company; }
        }

        public string InformationalVersion
        {
            get { return iAssemblyAttributes.InformationalVersion; }
        }

        public EBuildType BuildType
        {
            get
            {
                return iAssemblyAttributes.BuildType;
            }
        }

        public DirectoryInfo DataPath
        {
            get { return iDataPath; }
        }

        public DirectoryInfo ExePath
        {
            get { return iExePath; }
        }

        public Stack Stack
        {
            get { return iStack; }
        }

        public OptionNetworkInterface Interface
        {
            get
            {
                return iOptionNetworkInterface;
            }
        }

        protected NetworkInfoModel CommandLineInterface {
            get
            {
                return iCommandLineInterface;
            }
        }

        private NetworkInfoModel GetInterface(string aNetworkInterface)
        {
            IPAddress addr = null;
            try
            {
                addr = IPAddress.Parse(aNetworkInterface);
            }
            catch (FormatException)
            {
            }

            IList<NetworkInfoModel> ifaces = NetworkInfo.GetAllNetworkInterfaces();
            foreach (NetworkInfoModel iface in ifaces)
            {
                if (addr != null && addr != IPAddress.None)
                {
                    if (iface.IPAddress.Equals(addr))
                    {
                        return iface;
                    }
                }
                else
                {
                    if (iface.Name == aNetworkInterface)
                    {
                        return iface;
                    }
                }
            }

            return null;
        }

        private object iLock;
        private bool iErrorThrown;

        private DirectoryInfo iExePath;
        private DirectoryInfo iDataPath;
        private AssemblyInfoModel iAssemblyAttributes;

        private List<IOptionPage> iOptionPages;
        private OptionManager iOptionManager;
        private OptionPage iOptionPage;
        private OptionNetworkInterface iOptionNetworkInterface;
        
        private List<ICrashLogDumper> iCrashLogDumpers;
        private TraceListenerFile iTraceFile;
        
        private UserLogFile iUserLogFile;
        
        private OptionParser iOptionParser;
        private OptionParser.OptionString iOptionTraceLevel;
        private OptionParser.OptionString iOptionNic;
        private NetworkInfoModel iCommandLineInterface;

        private Stack iStack;

        public event EventHandler<EventArgs> EventOptionPagesChanged;
        private void OnEventOptionPagesChanged()
        {
            if (EventOptionPagesChanged != null)
            {
                EventOptionPagesChanged(this, EventArgs.Empty);
            }
        }

    }
}


