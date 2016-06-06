using System;

using Linn.Kinsky;

namespace KinskyDesktopWpf
{
	internal class AppRestartHandler : IAppRestartHandler
	{
		public void Restart()
		{
			Console.WriteLine("Restart required to complete installation of plugin.");
		}
	}
}