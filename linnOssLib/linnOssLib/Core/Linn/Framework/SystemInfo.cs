using System;
using System.IO;
using System.Reflection;


namespace Linn
{
    /// <summary>
    /// Description of Environment.
    /// </summary>
    public static class SystemInfo
    {
        public static string ServicePack
        {
            get
            {
                return Environment.OSVersion.ServicePack;
            }
        }

        public static PlatformId Platform
        {
            get
            {
                return PlatformIdConverters.From(Environment.OSVersion.Platform);
            }
        }

        public static string VersionString
        {
            get
            {
                return String.Format("{0} ({1})", Environment.OSVersion.Platform.ToString(), Environment.OSVersion.VersionString);
            }
        }

        public static string ComputerName
        {
            get
            {
                return System.Environment.MachineName;
            }
        }

        public static DirectoryInfo DataPathForApp(string aAppTitle)
        {
            return new DirectoryInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                aAppTitle));
        }
				        
		public static DirectoryInfo ExePathForApp(string aAppTitle)
        {
            return new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName));
        }
    }
}
