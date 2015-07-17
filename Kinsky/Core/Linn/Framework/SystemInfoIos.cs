using System;
using System.IO;
using System.Reflection;

using MonoTouch;

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
                return PlatformId.IOS;
            }
        }

        public static string VersionString
        {
            get
            {
				return String.Format("{0} ({1}) using MonoTouch {2}", "iOS", Environment.OSVersion.VersionString, ObjCRuntime.Constants.Version);
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
            return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }
		        
		public static DirectoryInfo ExePathForApp(string aAppTitle)
        {
            return new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName));
        }
		
    }
}
