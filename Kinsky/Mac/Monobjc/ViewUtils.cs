
using System;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    // Class for a text cell in the table view where the text is vertically centred
    [ObjectiveCClass]
    public class TextFieldCellCentred : NSTextFieldCell
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(TextFieldCellCentred));

        public TextFieldCellCentred() : base() {}
        public TextFieldCellCentred(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawingRectForBounds:")]
        public override NSRect DrawingRectForBounds(NSRect aRect)
        {
            NSRect rect = this.SendMessageSuper<NSRect>(ThisClass, "drawingRectForBounds:", aRect);

            float textHeight = AttributedStringValue.Size.height;

            return new NSRect(rect.origin.x, rect.MidY - textHeight*0.5f, rect.Width, textHeight);
        }
    }


    // Custom scrollbar class - for **vertical** scrollers only!
    [ObjectiveCClass]
    public class ViewScroller : NSScroller
    {
        public ViewScroller() : base() {}
        public ViewScroller(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            // draw background
            NSRect slotRect = RectForPart(NSScrollerPart.NSScrollerKnobSlot);

            NSColor.BlackColor.Set();
            AppKitFramework.NSRectFill(slotRect);
            AppKitFramework.NSRectFill(aRect);

            NSColor.ColorWithCalibratedWhiteAlpha(0.1f, 1.0f).Set();

            NSBezierPath path = new NSBezierPath();
            path.AppendBezierPathWithRoundedRectXRadiusYRadius(slotRect, 2.0f, 2.0f);
            path.LineWidth = 3.0f;
            path.Stroke();
            path.Release();

            // draw know
            DrawKnob();

            // draw the arrow button backgrounds
            DrawArrow(NSScrollerPart.NSScrollerIncrementLine);
            DrawArrow(NSScrollerPart.NSScrollerDecrementLine);
        }

        private void DrawArrow(NSScrollerPart aScrollPart)
        {
            NSRect rect = RectForPart(aScrollPart);

            // draw background
            NSColor.BlackColor.Set();
            AppKitFramework.NSRectFill(rect);

            // draw button background
            if (aScrollPart == HitPart)
            {
                NSColor.WhiteColor.Set();
            }
            else
            {
                NSColor.ColorWithCalibratedWhiteAlpha(0.1f, 1.0f).Set();
            }

            NSBezierPath path = new NSBezierPath();
            path.AppendBezierPathWithRoundedRectXRadiusYRadius(rect.InsetRect(1.0f, 1.0f), 2.0f, 2.0f);
            path.LineWidth = 2.0f;
            path.Stroke();
            path.Release();

            // draw arrow
            NSColor.WhiteColor.Set();

            float arrowSize = 6.0f;

            NSRect arrowRect = new NSRect(rect.MidX - (arrowSize * 0.5f),
                                          rect.MidY - (arrowSize * 0.5f),
                                          arrowSize,
                                          arrowSize);

            path = new NSBezierPath();
            if (aScrollPart == NSScrollerPart.NSScrollerDecrementLine)
            {
                path.MoveToPoint(new NSPoint(arrowRect.MinX, arrowRect.MaxY));
                path.LineToPoint(new NSPoint(arrowRect.MaxX, arrowRect.MaxY));
                path.LineToPoint(new NSPoint(arrowRect.MidX, arrowRect.MinY));
                path.LineToPoint(new NSPoint(arrowRect.MinX, arrowRect.MaxY));
            }
            else
            {
                path.MoveToPoint(new NSPoint(arrowRect.MinX, arrowRect.MinY));
                path.LineToPoint(new NSPoint(arrowRect.MaxX, arrowRect.MinY));
                path.LineToPoint(new NSPoint(arrowRect.MidX, arrowRect.MaxY));
                path.LineToPoint(new NSPoint(arrowRect.MinX, arrowRect.MinY));
            }
            path.ClosePath();
            path.Fill();
            path.Release();
        }
    }


    // helper class for sliding view animations
    public static class NSViewAnimationHelper
    {
        public static NSViewAnimation Create()
        {
            return new NSViewAnimation(0.35f, NSAnimationCurve.NSAnimationEaseIn);
        }

        public static void SetFrames(NSViewAnimation aAnimation, Id aTarget, NSRect aStart, NSRect aEnd)
        {
            NSDictionary animDict = NSDictionary.DictionaryWithObjectsAndKeys(aTarget, NSViewAnimation.NSViewAnimationTargetKey,
                                                                              NSValue.ValueWithRect(aStart), NSViewAnimation.NSViewAnimationStartFrameKey,
                                                                              NSValue.ValueWithRect(aEnd), NSViewAnimation.NSViewAnimationEndFrameKey,
                                                                              null);
            aAnimation.ViewAnimations = NSArray.ArrayWithObject(animDict);
        }
    }


    // View that represents nothing but a background colour
    [ObjectiveCClass]
    public class ViewEmpty : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ViewEmpty));

        public ViewEmpty()
            : base()
        {
        }

        public ViewEmpty(IntPtr aInstance)
            : base(aInstance)
        {
        }

        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            if (iBackgroundColour != null)
            {
                iBackgroundColour.Release();
                iBackgroundColour = null;
            }

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        public NSColor BackgroundColour
        {
            set
            {
                if (iBackgroundColour != null)
                {
                    iBackgroundColour.Release();
                }
                iBackgroundColour = value;
                iBackgroundColour.Retain();
            }
        }

        public void SetOpaque(bool aIsOpaque)
        {
            iIsOpaque = aIsOpaque;
        }

        public override bool IsOpaque
        {
            [ObjectiveCMessage("isOpaque")]
            get
            {
                return iIsOpaque;
            }
        }

        public delegate void DEventHandler(NSEvent aEvent);
        public event DEventHandler EventClick;
        public event DEventHandler EventMouseDown;

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            // fill the view with the clear colour
            if (iBackgroundColour != null)
            {
                iBackgroundColour.Set();
            }
            else
            {
                NSColor.ClearColor.Set();
            }
            AppKitFramework.NSRectFill(aRect);
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            if (EventMouseDown != null)
            {
                EventMouseDown(aEvent);
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseDown:", aEvent);
            }
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            if (EventClick != null)
            {
                EventClick(aEvent);
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseUp:", aEvent);
            }
        }

        private NSColor iBackgroundColour = null;
        private bool iIsOpaque = false;
    }


    // Simple helper to help with creation of tracking areas - note that the view classes
    // that use this should handle various objective-C messages such as:
    //    updateTrackingAreas
    //    mouseEntered:
    //    mouseExited:
    //    mouseMoved:
    public static class TrackerHelper
    {
        public static NSTrackingArea Create(NSView aView, NSRect aTrackRect, bool aTrackMove)
        {
            NSTrackingArea trackingArea;
            if (aTrackMove)
            {
                trackingArea = new NSTrackingArea(aTrackRect,
                                                  NSTrackingAreaOptions.NSTrackingActiveInActiveApp |
                                                  NSTrackingAreaOptions.NSTrackingMouseEnteredAndExited |
                                                  NSTrackingAreaOptions.NSTrackingMouseMoved,
                                                  aView,
                                                  null);
            }
            else
            {
                trackingArea = new NSTrackingArea(aTrackRect,
                                                  NSTrackingAreaOptions.NSTrackingActiveInActiveApp |
                                                  NSTrackingAreaOptions.NSTrackingMouseEnteredAndExited,
                                                  aView,
                                                  null);
            }

            aView.AddTrackingArea(trackingArea);
            return trackingArea;
        }

        public static void Destroy(NSView aView, NSTrackingArea aTrackingArea)
        {
            aView.RemoveTrackingArea(aTrackingArea);
            aTrackingArea.Release();
        }
    }


    // Class to allow detection of single click events in a table and reroute drag/drop draggedImage:endedAt:operation
    [ObjectiveCClass]
    public class TableViewClickable : TableViewDragDrop
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(TableViewClickable));

        public TableViewClickable() : base() {}
        public TableViewClickable(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            // extend the NSTableView delegate protocol to allow single click
            // events to be passed on
            NSPoint pt = this.ConvertPointFromView(aEvent.LocationInWindow, null);
            int clickedRow = this.RowAtPoint(pt);
            int clickedCol = this.ColumnAtPoint(pt);

            // the base class mouseDown: message runs its own inner loop
            // that prevents the mouseUp: being overridden - this
            // function blocks until the mouseUp: has occurred
            this.SendMessageSuper(ThisClass, "mouseDown:", aEvent);

            // send message to the delegate - this is effectively like a mouseUp:
            // event
            this.Delegate.SendMessage("tableView:didClickCellAtColumn:row:", this, clickedCol, clickedRow);
        }
    }


    // Borderless window
    [ObjectiveCClass]
    public class WindowBorderless : NSWindow
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(WindowBorderless));

        public WindowBorderless() : base() {}
        public WindowBorderless(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("initWithContentRect:styleMask:backing:defer:")]
        public override Id InitWithContentRectStyleMaskBackingDefer(NSRect aContentRect, NSWindowStyleMasks aWindowStyle, NSBackingStoreType aBufferingType, bool aDeferCreation)
        {
            // overridden to set NSBorderlessWindowMask for no title bar
            this.SendMessageSuper(ThisClass,
                                  "initWithContentRect:styleMask:backing:defer:",
                                  aContentRect,
                                  NSWindowStyleMasks.NSBorderlessWindowMask,
                                  aBufferingType, aDeferCreation);
            return this;
        }

        [ObjectiveCMessage("initWithContentRect:styleMask:backing:defer:screen:")]
        public override Id InitWithContentRectStyleMaskBackingDeferScreen(NSRect aContentRect, NSWindowStyleMasks aWindowStyle, NSBackingStoreType aBufferingType, bool aDeferCreation, NSScreen aScreen)
        {
            // overridden to set NSBorderlessWindowMask for no title bar
            this.SendMessageSuper(ThisClass,
                                  "initWithContentRect:styleMask:backing:defer:screen:",
                                  aContentRect,
                                  NSWindowStyleMasks.NSBorderlessWindowMask,
                                  aBufferingType, aDeferCreation, aScreen);
            return this;
        }

        public override bool CanBecomeKeyWindow
        {
            [ObjectiveCMessage("canBecomeKeyWindow")]
            get { return true; }
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this.IsOpaque = false;
            this.BackgroundColor = NSColor.ClearColor;
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            if (EventMouseDown != null)
            {
                EventMouseDown(aEvent);
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseDown:", aEvent);
            }
        }
        
        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent aEvent)
        {
            if (EventMouseDragged != null)
            {
                EventMouseDragged(aEvent);
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseDragged:", aEvent);
            }
        }
        
        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            if (EventMouseUp != null)
            {
                EventMouseUp(aEvent);
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseUp:", aEvent);
            }
        }
        
        public delegate void DEventHandler(NSEvent aEvent);
        public event DEventHandler EventMouseDown;
        public event DEventHandler EventMouseDragged;
        public event DEventHandler EventMouseUp;
    }


    // Image utils
    public static class NSImageHelper
    {
        public static NSRect CentreImageInRect(NSImage aImage, NSRect aRect)
        {
            float imageAspect = aImage.Size.width / aImage.Size.height;
            float rectAspect = aRect.Width / aRect.Height;

            if (imageAspect > rectAspect)
            {
                float imageWidth = aRect.Width;
                float imageHeight = imageWidth / imageAspect;
                return new NSRect(aRect.MinX, aRect.MidY - imageHeight*0.5f, imageWidth, imageHeight);
            }
            else
            {
                float imageHeight = aRect.Height;
                float imageWidth = imageHeight * imageAspect;
                return new NSRect(aRect.MidX - imageWidth*0.5f, aRect.MinY, imageWidth, imageHeight);
            }
        }
    }


    // Clickable image view
    [ObjectiveCClass]
    public class ImageViewClickable : NSImageView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ImageViewClickable));

        public ImageViewClickable() : base() {}
        public ImageViewClickable(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent aEvent)
        {
            if (EventMouseDown != null)
            {
                EventMouseDown(aEvent);
            }
            else if (EventClick != null)
            {
                // click event is being listened to, so do not treat this view as transparent and
                // pass the mouse down event up to the super class
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseDown:", aEvent);
            }
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent aEvent)
        {
            if (EventClick != null)
            {
                EventClick(aEvent);
            }
            else
            {
                this.SendMessageSuper(ThisClass, "mouseUp:", aEvent);
            }
        }

        [ObjectiveCMessage("rightMouseDown:")]
        public override void RightMouseDown(NSEvent aEvent)
        {
            if (EventRightMouseDown != null)
            {
                EventRightMouseDown(aEvent);
            }

            // right clicks are for context menus, so this is a mechanism by which functionality can be
            // **added** to this event, rather than override. If the requirement to override arises in the
            // future, then the EventRightMouseDown delegate will probably need a return value so both cases
            // can be supported
            this.SendMessageSuper(ThisClass, "rightMouseDown:", aEvent);
        }

        public delegate void DEventHandler(NSEvent aEvent);
        public event DEventHandler EventClick;
        public event DEventHandler EventMouseDown;
        public event DEventHandler EventRightMouseDown;
    }


    // Classes for use with a custom cell for displaying track data in the browser and playlist
    public interface IDataTrackItem
    {
        bool IsGroup { get; }
        int Index { get; }
        string Title { get; }
        string TrackNumber { get; }
        string Subtitle1 { get; }
        string Subtitle2 { get; }
    }

    // Cocoa class to wrap a data item for passing to the custom cell
    [ObjectiveCClass]
    public class WrappedDataTrackItem : NSObject, INSCopying
    {
        public WrappedDataTrackItem() : base() {}
        public WrappedDataTrackItem(IntPtr aInstance) : base(aInstance) {}
        public WrappedDataTrackItem(IDataTrackItem aItem)
        {
            Item = aItem;
        }

        [ObjectiveCMessage("copyWithZone:")]
        public Id CopyWithZone(IntPtr aZone)
        {
            return new WrappedDataTrackItem(Item);
        }

        public readonly IDataTrackItem Item;
    }

    // Custom cell class for the track details
    [ObjectiveCClass]
    public class CellTrackItem : NSTextFieldCell
    {
        public CellTrackItem() : base() {}
        public CellTrackItem(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawWithFrame:inView:")]
        public void Draw(NSRect aCellFrame, NSView aControlView)
        {
            WrappedDataTrackItem wrapped = ObjectValue.CastAs<WrappedDataTrackItem>();
            IDataTrackItem item = wrapped.Item;

            // create attributes for the title and subtitle text
            NSMutableParagraphStyle style = new NSMutableParagraphStyle();
            style.SetParagraphStyle(NSParagraphStyle.DefaultParagraphStyle);
            style.SetLineBreakMode(NSLineBreakMode.NSLineBreakByTruncatingTail);

            // set font sizes depending on whether this item is a group or track
            NSFont titleFont = item.IsGroup ? FontManager.FontLarge : FontManager.FontSemiLarge;
            NSFont subtitleFont = item.IsGroup ? FontManager.FontMedium : FontManager.FontSmall;

            NSDictionary titleAttr = NSDictionary.DictionaryWithObjectsAndKeys(titleFont, NSAttributedString.NSFontAttributeName,
                                                                               NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                               style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                               null);
            NSDictionary subtitleAttr = NSDictionary.DictionaryWithObjectsAndKeys(subtitleFont, NSAttributedString.NSFontAttributeName,
                                                                                  NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                                  style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                                  null);
            style.Release();

            float vertPos = aCellFrame.origin.y;
            float titleIndent = 0.0f;
            NSSize size;
            NSRect rect;

            // draw index for non-group items
            if (!item.IsGroup && item.TrackNumber != string.Empty)
            {
                NSString index = NSString.StringWithUTF8String(string.Format("{0}. ", item.TrackNumber));

                size = index.SizeWithAttributes(titleAttr);
                rect = new NSRect(aCellFrame.MinX, vertPos, size.width, size.height);
                index.DrawInRectWithAttributes(rect, titleAttr);

                titleIndent = size.width;
            }

            // draw the title
            NSString title = NSString.StringWithUTF8String(item.Title);

            size = title.SizeWithAttributes(titleAttr);
            rect = new NSRect(aCellFrame.MinX + titleIndent, vertPos, aCellFrame.Width - titleIndent, size.height);
            title.DrawInRectWithAttributes(rect, titleAttr);

            vertPos += size.height;

            // draw subtitle 1 aligned with the left of the cell
            if (item.Subtitle1 != string.Empty)
            {
                NSString subtitle1 = NSString.StringWithUTF8String(item.Subtitle1);

                size = subtitle1.SizeWithAttributes(subtitleAttr);
                rect = new NSRect(aCellFrame.MinX, vertPos, aCellFrame.Width, size.height);
                subtitle1.DrawInRectWithAttributes(rect, subtitleAttr);

                vertPos += size.height;
            }

            // draw subtitle 2 aligned with the left of the cell
            if (item.Subtitle2 != string.Empty)
            {
                NSString subtitle2 = NSString.StringWithUTF8String(item.Subtitle2);

                size = subtitle2.SizeWithAttributes(subtitleAttr);
                rect = new NSRect(aCellFrame.MinX, vertPos, aCellFrame.Width, size.height);
                subtitle2.DrawInRectWithAttributes(rect, subtitleAttr);
            }
        }
    }
}


