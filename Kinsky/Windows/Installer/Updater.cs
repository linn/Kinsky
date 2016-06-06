using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System;

namespace KinskyDesktopUpdate
{
    public class Updater
    {
        private static string kUpdateReadySignal = "KinskyDesktopUpdateReadySignal";
        private static string kUpdateStartedSignal = "KinskyDesktopUpdateStartedSignal";
		private static string kApplicationNameV1 = "KinskyDesktop";
		private static string kApplicationNameV2 = "Kinsky";
		private static string kUpdateFolder = "Updates";
		private static string kInstallerName = "InstallerKinsky.exe";
		
        public static void PostInstall()
        {
            Thread t = new Thread(delegate()
            {
                // await main app shutdown
                using (EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, kUpdateReadySignal))
                {
                    using (EventWaitHandle waitHandle2 = new EventWaitHandle(false, EventResetMode.AutoReset, kUpdateStartedSignal))
                    {
                        waitHandle.WaitOne();
						try{
							string fileNameV1 = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), kApplicationNameV1),kUpdateFolder),kInstallerName);
							string fileNameV2 = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), kApplicationNameV2),kUpdateFolder),kInstallerName);
						
							if (System.IO.File.Exists(fileNameV2))
							{
								Process.Start(fileNameV2);
							}else
							{						
								Process.Start(fileNameV1);
							}
						}catch{}
                        waitHandle2.Set();
                    }
                } 
            });
            t.Start();
        }
    }
}

