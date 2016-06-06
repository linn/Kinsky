using System;

using Linn.Kinsky;


namespace KinskyDesktop
{
	public class MediaProviderSupport : IContentDirectorySupportV2
	{
		public MediaProviderSupport(IVirtualFileSystem aVirtualFileSystem)
		{
			iVirtualFileSystem = aVirtualFileSystem;
		}
		
		public IVirtualFileSystem VirtualFileSystem
		{
			get
			{
				return iVirtualFileSystem;
			}
		}

		private IVirtualFileSystem iVirtualFileSystem;
	}
}



