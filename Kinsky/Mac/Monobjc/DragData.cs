
using System;
using System.Collections.Generic;

using Linn;
using Linn.Kinsky;

using Upnp;


namespace KinskyDesktop
{
    // class to encapsulate data that is dragged from within one of the
    // application views i.e. the browser or the playlist
    public class ViewDragData
    {
        public ViewDragData(IList<int> aIndices, IMediaRetriever aRetriever, object aSource)
        {
            Indices = aIndices;
            Retriever = aRetriever;
            Source = aSource;
        }

        public readonly IList<int> Indices;
        public readonly IMediaRetriever Retriever;
        public readonly object Source;
    }


    // interface extending the Kinsky.IDraggableData interface to all retrieval of
    // ViewDragData
    public interface IDraggableData : Linn.Kinsky.IDraggableData
    {
        ViewDragData GetViewDragData();
    }


    // drag operations
    public enum EDragOperation
    {
        eNone,
        eCopy,
        eMove,
        eDelete
    }
}


