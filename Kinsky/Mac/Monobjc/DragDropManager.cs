
using System;
using System.Collections.Generic;

using Linn;
using Linn.Kinsky;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    // static class for storing the internal data that is dragged from a playlist
    // or the browser
    public class DragDropManager
    {
        public static ViewDragData Current
        {
            get { return iCurrent; }
            set { iCurrent = value; }
        }

        public static NSDragOperation Convert(EDragOperation aDragOperation)
        {
            switch (aDragOperation)
            {
            case EDragOperation.eCopy:
                return NSDragOperation.NSDragOperationCopy;
            case EDragOperation.eDelete:
                return NSDragOperation.NSDragOperationDelete;
            case EDragOperation.eMove:
                return NSDragOperation.NSDragOperationMove;
            case EDragOperation.eNone:
                return NSDragOperation.NSDragOperationNone;
            default:
                Assert.Check(false);
                return NSDragOperation.NSDragOperationNone;
            }
        }

        public static EDragOperation Convert(NSDragOperation aDragOperation)
        {
            switch (aDragOperation)
            {
            case NSDragOperation.NSDragOperationCopy:
                return EDragOperation.eCopy;
            case NSDragOperation.NSDragOperationDelete:
                return EDragOperation.eDelete;
            case NSDragOperation.NSDragOperationMove:
                return EDragOperation.eMove;
            case NSDragOperation.NSDragOperationNone:
            default:
                return EDragOperation.eNone;
            }
        }

        private static ViewDragData iCurrent;
    }


    // Empty class to act as a token to signify that a pasteboard holds data that
    // is currently owned by the DragDropManager
    [ObjectiveCClass]
    public class PasteboardViewDragData : NSObject, INSCoding
    {
        public PasteboardViewDragData() : base() {}
        public PasteboardViewDragData(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("initWithCoder:")]
        public Id InitWithCoder(NSCoder aDecoder)
        {
            return null;
        }

        [ObjectiveCMessage("encodeWithCoder:")]
        public void EncodeWithCoder(NSCoder aEncoder)
        {
        }

        public static readonly NSString PboardType = new NSString("PasteboardViewDragData");
    }
	
	// Empty class to act as a token to signify that a pasteboard holds bookmark data
    [ObjectiveCClass]
    public class PasteboardViewDragDataBookmarks : NSObject, INSCoding
    {
        public PasteboardViewDragDataBookmarks() : base() {}
        public PasteboardViewDragDataBookmarks(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("initWithCoder:")]
        public Id InitWithCoder(NSCoder aDecoder)
        {
            return null;
        }

        [ObjectiveCMessage("encodeWithCoder:")]
        public void EncodeWithCoder(NSCoder aEncoder)
        {
        }

        public static readonly NSString PboardType = new NSString("PasteboardViewDragDataBookmarks");
    }


    // Class to convert NSPasteboard data to useful drag data information
    public class DraggableData : IDraggableData
    {
        public DraggableData(NSPasteboard aPasteboard)
        {
            iPasteboard = aPasteboard;
        }

        public ViewDragData GetViewDragData()
        {
            NSData data = iPasteboard.DataForType(PasteboardViewDragData.PboardType);
            return (data != null) ? DragDropManager.Current : null;
        }

        public MediaProviderDraggable GetInternal()
        {
            ViewDragData dragData = GetViewDragData();
            if (dragData != null)
            {
                return new MediaProviderDraggable(dragData.Retriever);
            }

            return null;
        }

        public string GetText()
        {
            return null;
        }

        public string GetUri()
        {
            if (iPasteboard.Types.ContainsObject(NSPasteboard.NSURLPboardType))
            {
                NSURL url = NSURL.URLFromPasteboard(iPasteboard);
                if (!url.IsFileURL)
                {
                    return NSURL.URLFromPasteboard(iPasteboard).AbsoluteString.ToString();
                }
            }

            return null;
        }

        public string[] GetFileList()
        {
            if (iPasteboard.Types.ContainsObject(NSPasteboard.NSFilenamesPboardType))
            {
                NSArray files = iPasteboard.PropertyListForType(NSPasteboard.NSFilenamesPboardType).CastTo<NSArray>();
                List<string> list = new List<string>();
                for (int i=0 ; i<files.Count ; i++)
                {
                    list.Add(files[i].CastTo<NSString>().ToString());
                }
                return list.ToArray();
            }

            return null;
        }

        private NSPasteboard iPasteboard;
    }


    // class for adding dragging source behaviour to a view - the view in this case is a
    // list based view i.e. the browser or playlist
    public class DragSource
    {
        public DragSource(IControllerDragSource aController)
        {
            iController = aController;
        }

        public int Begin(NSIndexSet aIndices, object aSource, NSPasteboard aPasteboard)
        {
            // build list of indices to be dragged
            List<int> indices = new List<int>();

            uint index = aIndices.FirstIndex;
            while (index != FoundationFramework.NSNotFound)
            {
                indices.Add((int)index);
                index = aIndices.IndexGreaterThanIndex(index);
            }

            // set the current data to be dragged
            DragDropManager.Current = iController.DragBegin(indices, aSource);

            // add a token to the pasteboard
            PasteboardViewDragData token = new PasteboardViewDragData();
            NSData tokenData = NSKeyedArchiver.ArchivedDataWithRootObject(token);
            token.Release();

            aPasteboard.DeclareTypesOwner(NSArray.ArrayWithObject(PasteboardViewDragData.PboardType), null);
            aPasteboard.SetDataForType(tokenData, PasteboardViewDragData.PboardType);

            return indices.Count;
        }

        public void End(NSPasteboard aPasteboard, NSDragOperation aOperation)
        {
            DraggableData dragData = new DraggableData(aPasteboard);

            iController.DragEnd(dragData, DragDropManager.Convert(aOperation));

            DragDropManager.Current = null;
        }

        private IControllerDragSource iController;
    }


    // class for adding dragging destination behaviour to a view - the view in this case is a
    // list based view i.e. the browser or playlist
    public class DragDestination
    {
        public DragDestination(IControllerDragDestination aController)
        {
            iController = aController;
        }

        public static void RegisterDragTypes(NSView aView)
        {
            NSArray dragTypes = NSArray.ArrayWithObjects(NSPasteboard.NSFilenamesPboardType,
                                                         NSPasteboard.NSURLPboardType,
                                                         PasteboardViewDragData.PboardType,
                                                         null);
            aView.RegisterForDraggedTypes(dragTypes);
        }

        public NSDragOperation ValidateDrop(INSDraggingInfo aInfo, object aDestination)
        {
            DraggableData dragData = new DraggableData(aInfo.DraggingPasteboard);

            EDragOperation op = iController.ValidateDrag(dragData, aDestination);

            return DragDropManager.Convert(op);
        }

        public bool AcceptDrop(INSDraggingInfo aInfo, int aIndex, object aDestination)
        {
            DraggableData dragData = new DraggableData(aInfo.DraggingPasteboard);

            return iController.AcceptDrop(dragData, aIndex, aDestination);
        }

        private IControllerDragDestination iController;
    }
}



