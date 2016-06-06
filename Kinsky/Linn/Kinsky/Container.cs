using System;
using System.Collections.Generic;
using System.Threading;

using Upnp;

namespace Linn.Kinsky
{
    public class EventArgsContentRemoved : EventArgs
    {
        public EventArgsContentRemoved(string aId)
        {
            Id = aId;
        }

        public string Id;
    }

    public interface IContainer
    {
        uint Open();
        void Close();

        string Id { get; }

        void Refresh();

        IContainer ChildContainer(container aContainer);

        container Metadata { get; }
		
		bool HandleMove(DidlLite aDidlLite);

        bool HandleInsert(DidlLite aDidlLite);
        void Insert(string aAfterId, DidlLite aDidlLite);

		bool HandleDelete(DidlLite aDidlLite);
        void Delete(string aId);

        bool HandleRename(upnpObject aUpnpObject);
        void Rename(string aId, string aTitle);

        DidlLite Items(uint aStartIndex, uint aCount);
        DidlLite Search(string aSearchCriterea, uint aStartIndex, uint aCount);

        bool HasTreeChangeAffectedLeaf { get; }

        event EventHandler<EventArgs> EventContentUpdated;
        event EventHandler<EventArgs> EventContentAdded;
        event EventHandler<EventArgsContentRemoved> EventContentRemoved;
        event EventHandler<EventArgs> EventTreeChanged;
    }



    public class EventArgsContainerUpdated : EventArgs
    {
        public enum EType
        {
            eUpdated,
            eAdded,
            eRemoved,
        }

        private EventArgsContainerUpdated(string aId)
        {
            iType = EType.eRemoved;
            iId = aId;
        }

        private EventArgsContainerUpdated(EType aType)
        {
            iType = aType;
        }

        public EType Type
        {
            get
            {
                return (iType);
            }
        }

        public string Id
        {
            get
            {
                return (iId);
            }
        }

        static public EventArgsContainerUpdated Added = new EventArgsContainerUpdated(EType.eAdded);
        static public EventArgsContainerUpdated Updated = new EventArgsContainerUpdated(EType.eUpdated);

        private EType iType;
        private string iId;
    }

    public interface IContainer2
    {
        uint Count { get; }

        void Refresh();

        IContainer Open(container aContainer);

        DidlLite Items(uint aStartIndex, uint aCount);

        event EventHandler<EventArgsContainerUpdated> EventContentUpdated;
    }

}