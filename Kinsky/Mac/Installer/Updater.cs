
using System;
using System.Diagnostics;
using System.IO;


namespace KinskyDesktopUpdate
{
    public class Updater
    {
        public static void RunInstaller()
        {
            Process.Start(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library"), "Kinsky"), "Updates"), "InstallerKinsky.pkg"));
        }
    }
}

