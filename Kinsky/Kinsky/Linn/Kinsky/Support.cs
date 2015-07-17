using System.Net;
using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;

using Linn.Control.Ssdp;
using Linn.ControlPoint.Upnp;
using Linn.Topology;
using Upnp;

namespace Linn.Kinsky
{
    public interface IContentDirectorySupportV2
    {
        IVirtualFileSystem VirtualFileSystem { get; }
    }

    public interface IVirtualFileSystem
    {
        void MakePathPublic(string aPath);
        string Uri(string aUnescapedFilename);
    }

} // Linn.Kinsky
