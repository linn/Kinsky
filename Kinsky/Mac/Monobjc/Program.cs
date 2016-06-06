using System;
using System.IO;
using System.Reflection;

using Linn;

using Monobjc;
using Monobjc.Cocoa;

namespace KinskyDesktop
{
	public static class Program
	{
        static void Main(string[] aArgs)
        {
            // when built with scons, the executing assembly appears in the KinskyDesktop.app/Contents/MacOs folder
            // whereas under MonoDevelop it is in the KinskyDesktop.app/Contents/Resources folder - this code will handle both
            string exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            string contentsFolder = Path.GetDirectoryName(exeFolder);
            string resFolder = Path.Combine(contentsFolder, "Resources");
            KinskyDesktop.Properties.Resources.SetBasePath(resFolder);

			ObjectiveCRuntime.LoadFramework("Cocoa");
            ObjectiveCRuntime.LoadFramework("Quartz");
			ObjectiveCRuntime.Initialize();

			NSApplication.Bootstrap();
            NSApplication.LoadNib("MainMenu.nib");
            NSApplication.RunApplication();
        }
	}
}

