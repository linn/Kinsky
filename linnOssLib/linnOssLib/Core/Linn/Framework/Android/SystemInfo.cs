using System;
using System.IO;


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
                return PlatformId.Android;
            }
        }

        public static string VersionString
        {
            get
            {
                return String.Format("{0} ({1})", "Android", Environment.OSVersion.VersionString);
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
            return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        }

        public static DirectoryInfo ExePathForApp(string aAppTitle)
        {
            return DataPathForApp(aAppTitle);
        }
    }
}
