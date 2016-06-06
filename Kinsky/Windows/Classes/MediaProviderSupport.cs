using System;
using System.Net;

using Linn;
using Linn.Kinsky;
using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;

namespace KinskyDesktopWpf
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



