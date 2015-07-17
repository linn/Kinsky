using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Upnp;

namespace Linn.Kinsky
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ContentDirectoryFactoryTypeAttribute : Attribute
    {
        public readonly string TypeName;

        public ContentDirectoryFactoryTypeAttribute(string aTypeName)
        {
            TypeName = aTypeName;
        }
    }

    public interface IContentDirectoryFactory
    {
        IContentDirectory Create(string aDataPath, IContentDirectorySupportV2 aSupport);
    }

    public interface IContentDirectorySupportFactory
    {
        object Create(uint aVersion);
    }

    public interface IContentDirectory
    {
        void Start();
        void Stop();

        string Name { get; }

        string Company { get; } // to be removed
        string Version { get; } // to be removed

        IContainer Root { get; }

        // should change this to Add(IOptionPage aPage) in the AppSupport
        // so that plug-ins are not restricted to only one.

        IOptionPage OptionPage { get; } 
    }

    public class EventArgsContainerUpdate : EventArgs
    {
        public EventArgsContainerUpdate(string aContainerId, string aUpdateId)
        {
            ContainerId = aContainerId;
            UpdateId = aUpdateId;
        }

        public string ContainerId;
        public string UpdateId;
    }

    public class MediaProviderDraggable : IMediaRetriever
    {
        public MediaProviderDraggable(IMediaRetriever aMediaRetriever)
        {
            iDragSource = null;
            iMediaRetriever = aMediaRetriever;
        }

        public MediaProviderDraggable(IMediaRetriever aMediaRetriever, object aDragSource)
        {
            iDragSource = aDragSource;
            iMediaRetriever = aMediaRetriever;
        }

        public DidlLite DragMedia
        {
            get
            {
                return iMediaRetriever.DragMedia;
            }
        }

        public DidlLite Media
        {
            get
            {
                return iMediaRetriever.Media;
            }
        }

        public object DragSource
        {
            get
            {
                return iDragSource;
            }
            set
            {
                iDragSource = value;
            }
        }

        private object iDragSource;
        private IMediaRetriever iMediaRetriever;
    }
} // Linn.Kinsky
