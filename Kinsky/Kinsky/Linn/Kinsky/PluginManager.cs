using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

using ICSharpCode.SharpZipLib.Zip;

namespace Linn.Kinsky
{
    public class Plugin
    {
        public Plugin(string aPackage, string aFilename, string aPluginPath, IContentDirectorySupportV2 aSupport)
        {
            iPackage = aPackage;
            iFilename = aFilename;

            Load(aPluginPath, aSupport);
        }

        public bool IsStandardPlugin
        {
            get
            {
                return (iPackage == string.Empty);
            }
        }

        public string Package
        {
            get
            {
                return iPackage;
            }
        }

        public string Filename
        {
            get
            {
                return iFilename;
            }
        }

        public IContentDirectory MediaProvider
        {
            get
            {
                return iMediaProvider;
            }
        }

        private void Load(string aPluginPath, IContentDirectorySupportV2 aSupport)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(iFilename);
                if (assembly.Location != iFilename)
                {
                    throw new PluginManager.NotMediaProviderPluginException(iFilename);
                }
                object[] attributes = assembly.GetCustomAttributes(typeof(ContentDirectoryFactoryTypeAttribute), false);
                if(attributes.Length > 0)
                {
                    ContentDirectoryFactoryTypeAttribute type = attributes[0] as ContentDirectoryFactoryTypeAttribute;
                    if(type != null)
                    {
                        IContentDirectoryFactory factory = Activator.CreateInstance(assembly.GetType(type.TypeName)) as IContentDirectoryFactory;
    
                        Trace.WriteLine(Trace.kKinsky, "Plugin.Load: Found MediaProvider plugin (" + iFilename + ")");
    
                        string dataPath = Path.Combine(aPluginPath, Path.GetFileNameWithoutExtension(iFilename));
                        if (!Directory.Exists(dataPath))
                        {
                            try
                            {
                                Directory.CreateDirectory(dataPath);
                            }
                            catch (Exception e)
                            {
                                UserLog.WriteLine("Failed to create data directory for " + iFilename + "(" + e.Message + ")");
                            }
                        }
    
                        iMediaProvider = factory.Create(dataPath, aSupport);
                        UserLog.WriteLine("Loaded plugin " + iMediaProvider.Name);
                        Trace.WriteLine(Trace.kKinsky, "Loaded plugin " + iMediaProvider.Name);
                    }
                    else
                    {
                        UserLog.WriteLine(iFilename + " is not a plugin");
                        Trace.WriteLine(Trace.kKinsky, "Plugin.Load(" + iFilename + "): not a plugin");
                    }
                }
            }
            catch (Exception e)   // just ignore all errors
            {
                UserLog.WriteLine("Error loading " + iFilename + " (" + e.Message + ")");
                Trace.WriteLine(Trace.kKinsky, "Plugin.Load(" + iFilename + "): " + e.Message);
            }

            if (iMediaProvider == null)
            {
                throw new PluginManager.NotMediaProviderPluginException(iFilename);
            }
        }

        private Type GetInterface(Type aType, string aName)
        {
            Type[] interfaces = aType.GetInterfaces();
            foreach (Type type in interfaces)
            {
                if (type.Name == aName)
                {
                    return type;
                }
            }
            return null;
        }

        private const string kMediaProviderFactory = "IContentDirectoryFactory";

        private string iPackage;
        private string iFilename;

        private IContentDirectory iMediaProvider;
    }

    public class PluginManager
    {
        public class NotMediaProviderPluginException : Exception
        {
            public NotMediaProviderPluginException(string aFilename)
                : base(string.Format("{0} is not a media provider plugin", aFilename))
            { }
        }

        public class EventArgsPlugin : EventArgs
        {
            public EventArgsPlugin(Plugin aPlugin)
            {
                Plugin = aPlugin;
            }

            public Plugin Plugin;
        }

        private class FileHelper
        {
            public static string[] GetFilesRecursive(string aPath, string aExtension)
            {
                List<string> files = new List<string>();

                try
                {
                    string[] dirs = Directory.GetDirectories(aPath);
                    if (dirs != null) //android!
                    {
                        foreach (string d in dirs)
                        {
                            files.AddRange(GetFilesRecursive(d, aExtension));
                        }
                        files.AddRange(Directory.GetFiles(aPath, aExtension));
                    }
                }
                catch (DirectoryNotFoundException)
                {
                }

                return files.ToArray();
            }
        }

        public PluginManager(IHelper aHelper, IHttpClient aHttpClient, IContentDirectorySupportV2 aSupport)
        {
            iHttpClient = aHttpClient;
            iSupport = aSupport;
            iHelper = aHelper;
            iPluginPath = Path.Combine(aHelper.DataPath.FullName, "Plugins");
            iStdPluginPath = Path.Combine(aHelper.ExePath.FullName, "");
            iDownloadPath = Path.Combine(aHelper.DataPath.FullName, "DownloadCache");

            iPlugins = new List<Plugin>();

            if (!Directory.Exists(iDownloadPath))
            {
                try
                {
                    Directory.CreateDirectory(iDownloadPath);
                }
                catch (Exception)
                {
                    UserLog.WriteLine("Unable to create plugin download cache directory");
                }
            }

            if (!Directory.Exists(iPluginPath))
            {
                try
                {
                    Directory.CreateDirectory(iPluginPath);
                }
                catch (Exception)
                {
                    UserLog.WriteLine("Unable to create plugin cache directory");
                }
            }

            /*if (!Directory.Exists(iStdPluginPath))
            {
                try
                {
                    Directory.CreateDirectory(iStdPluginPath);
                }
                catch (Exception)
                {
                    UserLog.WriteLine("Unable to create standard plugin directory");
                }
            }*/

            CleanUpPlugins();
        }

        public void Start()
        {
            Assert.Check(iThread == null);

            iThread = new Thread(Run);
            iThread.Name = "PluginManager";
            iThread.IsBackground = true;
            iThread.Start();

            Trace.WriteLine(Trace.kKinsky, "PluginManager.Start() successful");
        }

        public void Stop()
        {
            if (iThread != null)
            {
                iThread.Join();
                iThread = null;

                foreach (Plugin p in iPlugins)
                {
                    p.MediaProvider.Stop();
                }

                Trace.WriteLine(Trace.kKinsky, "PluginManager.Stop() successful");
            }
            else
            {
                Trace.WriteLine(Trace.kKinsky, "PluginManager.Stop() already stopped - silently do nothing");
            }
        }

        public void InstallPlugin(string aFilename)
        {
            if (Path.GetExtension(aFilename) == kPluginExtension)
            {
                bool resetRequired = false;
                string filename = Path.Combine(iDownloadPath, Path.GetFileName(aFilename));
                string removedFilename = Path.Combine(iDownloadPath, Path.GetFileNameWithoutExtension(aFilename) + kPluginUninstalledExtension);

                if (File.Exists(filename) || File.Exists(removedFilename))
                {
                    resetRequired = true;
                }

                Uri uri;
                if (Uri.TryCreate(aFilename, UriKind.RelativeOrAbsolute, out uri))
                {
                    if (uri.IsFile)
                    {
                        File.Copy(aFilename, filename, true);
                    }
                    else
                    {
                        Stream s = iHttpClient.Request(uri);
                        FileStream fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write);

                        byte[] data = new byte[40960];  // 40Kb
                        int bytesread;
                        while ((bytesread = s.Read(data, 0, 40960)) > 0)
                        {
                            fs.Write(data, 0, bytesread);
                        }

                        fs.Close();
                        fs.Dispose();

                        s.Close();
                        s.Dispose();
                    }

                    if (resetRequired)
                    {
                        AskUserForRestart();
                    }
                    else
                    {
                        bool sameName;
                        LoadDownloadedPlugin(filename, out sameName);
                        if (sameName)
                        {
                            AskUserForRestart();
                        }
                    }
                }
            }
        }

        public void UninstallPlugin(Plugin aPlugin)
        {
            if (aPlugin.Package != string.Empty)
            {
                // file might not exisit if plugin uninstalled from another instance of kinsky desktop
                // or user has manually deleted the original package...
                if (File.Exists(aPlugin.Package))
                {
                    File.Move(aPlugin.Package, aPlugin.Package.Replace(kPluginExtension, kPluginUninstalledExtension));
                }

                // bring down the plugin
                aPlugin.MediaProvider.Stop();
                if (aPlugin.MediaProvider.OptionPage != null)
                {
                    iHelper.RemoveOptionPage(aPlugin.MediaProvider.OptionPage);
                }

                // remove from the list
                iPlugins.Remove(aPlugin);

                // notify
                if (EventPluginUnInstalled != null)
                {
                    EventPluginUnInstalled(this, new EventArgsPlugin(aPlugin));
                }
            }
            else
            {
                // if we don't find the original package, it must be a standard plugin
                Trace.WriteLine(Trace.kKinsky, string.Format("WARNING: {0} is not allowed to be uninstalled", aPlugin.MediaProvider.Name));
            }
        }

        public EventHandler<EventArgsPlugin> EventPluginInstalled;
        public EventHandler<EventArgsPlugin> EventPluginUnInstalled;

        public EventHandler<EventArgs> EventRestartRequired;

        private void Run()
        {
            // load standard plugins first, so that we ensure we are running the latest released plugins
            // usually we would load downloaded plugins first, so we can override the standard plugins
            // we will revert back to this when the upgrade/installing of plugins is refactored to make it easier for the end user
            LoadStandardPlugins();
            LoadDownloadedPlugins();
        }

        private void AskUserForRestart()
        {
            if (EventRestartRequired != null)
            {
                EventRestartRequired(this, EventArgs.Empty);
            }
        }

        private void ExtractPlugin(string aFilename)
        {
            try
            {
                FastZip zip = new FastZip();
                zip.ExtractZip(aFilename, Path.Combine(iPluginPath, Path.GetFileNameWithoutExtension(aFilename)), "");
            }
            catch (Exception) { }
        }

        private void LoadStandardPlugins()
        {
            string[] files = FileHelper.GetFilesRecursive(iStdPluginPath, "OssKinskyMpp*.dll");
            if (files.Length == 0)
            {
                files = FileHelper.GetFilesRecursive(iStdPluginPath, "*.dll");
            }
            foreach (string f in files)
            {
                bool sameName;
                LoadPlugin(string.Empty, f, out sameName);
            }
        }

        private void LoadDownloadedPlugins()
        {
            if (Directory.Exists(iDownloadPath))
            {
                string[] packages = Directory.GetFiles(iDownloadPath, "*" + kPluginExtension);
                foreach (string p in packages)
                {
                    bool sameName;
                    if (!LoadDownloadedPlugin(p, out sameName))
                    {
                        // if package does not contain a valid plugin mark it for deletion
                        try
                        {
                            File.Move(p, p.Replace(kPluginExtension, kPluginUninstalledExtension));
                        }
                        catch { } // if this fails, we don't really care - it's probably due to a second instance of KD already having removed the file.
                    }
                }
            }
        }

        private bool LoadDownloadedPlugin(string aPackage, out bool aSameName)
        {
            aSameName = false;
            string pluginPath = Path.Combine(iPluginPath, Path.GetFileNameWithoutExtension(aPackage));

            if (!Directory.Exists(pluginPath))
            {
                ExtractPlugin(aPackage);
            }

            string[] files = FileHelper.GetFilesRecursive(pluginPath, "OssKinskyMpp*.dll");
            if (files.Length == 0)
            {
                files = FileHelper.GetFilesRecursive(pluginPath, "*.dll");
            }

            bool found = false;
            foreach (string f in files)
            {
                bool sameName;
                found |= LoadPlugin(aPackage, f, out sameName);
                aSameName |= sameName;
            }

            return found;
        }

        private bool LoadPlugin(string aPackage, string aFilename, out bool aSameNameFound)
        {
            aSameNameFound = false;
            Plugin p = FindPlugin(aPackage, aFilename);
            if (p != null)
            {
                p.MediaProvider.Start();
            }
            else
            {
                try
                {
                    // create the new plugin and add to the list
                    p = new Plugin(aPackage, aFilename, iPluginPath, iSupport);
                    iPlugins.Add(p);

                    // initialise the plugin
                    if (p.MediaProvider.OptionPage != null)
                    {
                        if (p.MediaProvider.OptionPage.Options.Count > 0)
                        {
                            iHelper.AddOptionPage(p.MediaProvider.OptionPage);
                        }
                    }
                    p.MediaProvider.Start();

                    // notify
                    if (EventPluginInstalled != null)
                    {
                        EventPluginInstalled(this, new EventArgsPlugin(p));
                    }
                }
                catch (NotMediaProviderPluginException) { }
                catch (ArgumentException e)
                {
                    aSameNameFound = true;
                    if (aPackage == string.Empty)
                    {
                        UserLog.WriteLine("Plugin " + p.MediaProvider.Name + " overridden by user downloaded plugin");
                    }
                    else
                    {
                        UserLog.WriteLine("Failed to load plugin " + p.MediaProvider.Name + " (" + e.Message + ")");
                    }
                }
            }

            return (p != null);
        }

        private Plugin FindPlugin(string aPackage, string aFilename)
        {
            foreach (Plugin p in iPlugins)
            {
                if (p.Filename == aFilename)
                {
                    return p;
                }
            }

            return null;
        }

        private void CleanUpPlugins()
        {
            if (Directory.Exists(iDownloadPath))
            {
                // clean up uninstalled plugins and shadow cache
                string[] plugins = Directory.GetFiles(iDownloadPath, "*" + kPluginUninstalledExtension);
                foreach (string p in plugins)
                {
                    try
                    {
                        File.Delete(p);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(Trace.kKinsky, "Failed to delete " + p + " (" + e.Message + ")");
                    }
                    if (Directory.Exists(Path.Combine(iPluginPath, Path.GetFileNameWithoutExtension(p))))
                    {
                        try
                        {
                            string[] dirs = Directory.GetDirectories(Path.Combine(iPluginPath, Path.GetFileNameWithoutExtension(p)));
                            foreach (string d in dirs)
                            {
                                try
                                {
                                    string[] files = FileHelper.GetFilesRecursive(d, "*");
                                    foreach (string f in files)
                                    {
                                        try
                                        {
                                            File.Delete(f);
                                        }
                                        catch (Exception) { }
                                    }
                                    string[] directories = FileHelper.GetFilesRecursive(d, "*");
                                    foreach (string f in directories)
                                    {
                                        try
                                        {
                                            Directory.Delete(f);
                                        }
                                        catch (Exception) { }
                                    }
                                    Directory.Delete(d);
                                }
                                catch (Exception e)
                                {
                                    Trace.WriteLine(Trace.kKinsky, "Failed to delete " + d + " (" + e.Message + ")");
                                }
                            }
                        }
                        catch(DirectoryNotFoundException)
                        {
                            Trace.WriteLine(Trace.kKinsky, "Failed to find plugin directory");
                        }
                    }
                }
            }
        }

        public readonly static string kPluginExtension = ".kpz";

        private const string kPluginUninstalledExtension = ".kpz.remove";

        private Thread iThread;

        private string iDownloadPath;
        private string iPluginPath;
        private string iStdPluginPath;

        private IHttpClient iHttpClient;
        private IContentDirectorySupportV2 iSupport;
        private IHelper iHelper;

        private List<Plugin> iPlugins;
    }
} // Linn.Kinsky
