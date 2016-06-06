
using System;

using Monobjc;
using Monobjc.Cocoa;

using Linn;


namespace KinskyDesktop
{
    // Button view for hover-style buttons
    [ObjectiveCClass]
    public class ButtonHoverPush : NSView
    {
        private static readonly Class ThisClass = Class.GetClassFromType(typeof(ButtonHoverPush));

        public ButtonHoverPush() : base() {}
        public ButtonHoverPush(IntPtr aInstance) : base(aInstance) {}

        public void Initialise(NSImage aImageNoHover, NSImage aImageHover, NSImage aImagePush)
        {
            // button initialised as a type 1 non-toggle button with internal mouse tracking
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);

            // create and initialise the cell
            Initialise(new ButtonHoverCellType1(aImageNoHover, aImageHover, aImagePush));
        }

        public void Initialise(NSImage aImageNoHover, NSImage aImageHover, NSImage aImagePush, IMouseTracker aTracker)
        {
            // button initialised as a type 1 non-toggle button with an external mouse tracker
            aTracker.EventMouseEntered += TrackerMouseEntered;
            aTracker.EventMouseExited += TrackerMouseExited;

            // create and initialise the cell
            Initialise(new ButtonHoverCellType1(aImageNoHover, aImageHover, aImagePush));
        }

        public void Initialise(NSImage aImageNoHoverOff, NSImage aImageHoverOff, NSImage aImagePushOff,
                               NSImage aImageNoHoverOn, NSImage aImageHoverOn, NSImage aImagePushOn)
        {
            // button initialised as a type 1 toggle button with internal mouse tracking
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);

            // create and initialise the cell
            Initialise(new ButtonHoverCellType1(aImageNoHoverOff, aImageHoverOff, aImagePushOff,
                                                aImageNoHoverOn, aImageHoverOn, aImagePushOn));
        }

        public IButtonHoverType2 Initialise()
        {
            // button initialised as a type 2 non-toggle button with internal mouse tracking
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);

            // create and initialise the cell
            ButtonHoverCellType2 cell = new ButtonHoverCellType2();
            Initialise(cell);

            return cell;
        }

        public IButtonHoverType2 Initialise(NSImage aImage)
        {
            // button initialised as a type 2 non-toggle button with internal mouse tracking
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);

            // create and initialise the cell
            ButtonHoverCellType2 cell = new ButtonHoverCellType2();
            cell.ImageOff = aImage;
            Initialise(cell);

            return cell;
        }

        public IButtonHoverType2 Initialise(NSImage aImageOff, NSImage aImageOn)
        {
            // button initialised as a type 2 toggle button with internal mouse tracking
            iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);

            // create and initialise the cell
            ButtonHoverCellType2 cell = new ButtonHoverCellType2();
            cell.ImageOff = aImageOff;
            cell.ImageOn = aImageOn;
            Initialise(cell);

            return cell;
        }

        public IDragDestination DragDelegate
        {
            set { iDragDelegate = value; }
        }

        public bool IsOn
        {
            get { return iOn; }
            set
            {
                iOn = value;
                iCell.SetOn(value);
            }
        }

        public bool Enabled
        {
            get { return iEnabled; }
            set
            {
                iEnabled = value;
                iCell.SetEnabled(value);

                if (iHovering)
                {
                    iCell.SetHovering(value);
                }
            }
        }

        public float PreferredWidth
        {
            get { return iCell.FittedWidth; }
        }

        public void SetWidth(float aWidth, bool aClampLeft)
        {
            Frame = new NSRect(aClampLeft ? Frame.MinX : Frame.MaxX - aWidth,
                               Frame.MinY,
                               aWidth, Frame.Height);
        }

        public NSString KeyEquivalent
        {
            set
            {
                iCell.KeyEquivalent = value;
            }
        }

        public event ActionEventHandler EventClicked;


        [ObjectiveCMessage("dealloc", SynchronizeFields = false)]
        public void Dealloc()
        {
            iCell.Dealloc();

            if (iTrackingArea != null)
            {
                iTrackingArea.Release();
            }

            this.SendMessageSuper(ThisClass, "dealloc");
        }

        [ObjectiveCMessage("updateTrackingAreas")]
        public override void UpdateTrackingAreas()
        {
            this.SendMessageSuper(ThisClass, "updateTrackingAreas");

            // only do stuff if this button has been configured to use internal mouse
            // tracking i.e. tracking using its own NSView
            if (iTrackingArea != null)
            {
                TrackerHelper.Destroy(this, iTrackingArea);
                iTrackingArea = TrackerHelper.Create(this, this.Bounds, false);
            }
        }

        [ObjectiveCMessage("mouseEntered:")]
        public override void MouseEntered(NSEvent aEvent)
        {
            // this method will only be called if internal tracking is being used
            TrackerMouseEntered(aEvent);
        }

        [ObjectiveCMessage("mouseExited:")]
        public override void MouseExited(NSEvent aEvent)
        {
            // this method will only be called if internal tracking is being used
            TrackerMouseExited(aEvent);
        }

        [ObjectiveCMessage("viewDidHide")]
        public override void ViewDidHide()
        {
            // if view is hidden (directly or via a hidden ancestor) then make sure the hovering state is off
            iHovering = false;
            if (iCell != null)
            {
                iCell.SetHovering(false);
            }
        }

        [ObjectiveCMessage("draggingEntered:")]
        public NSDragOperation DraggingEntered(INSDraggingInfo aInfo)
        {
            if (iDragDelegate != null)
            {
                return iDragDelegate.DraggingEntered(aInfo);
            }
            else
            {
                return this.SendMessageSuper<NSDragOperation>(ThisClass, "draggingEntered:", aInfo);
            }
        }

        [ObjectiveCMessage("performDragOperation:")]
        public bool PerformDragOperation(INSDraggingInfo aInfo)
        {
            if (iDragDelegate != null)
            {
                return iDragDelegate.PerformDragOperation(aInfo);
            }
            else
            {
                return this.SendMessageSuper<bool>(ThisClass, "performDragOperation:", aInfo);
            }
        }

        private void TrackerMouseEntered(NSEvent aEvent)
        {
            iHovering = true;

            if (iEnabled)
            {
                iCell.SetHovering(true);
            }
        }

        private void TrackerMouseExited(NSEvent aEvent)
        {
            iHovering = false;

            iCell.SetHovering(false);
        }

        private void ButtonClicked(Id aId)
        {
            IsOn = !IsOn;

            if (EventClicked != null)
            {
                EventClicked(this);
            }
        }

        private void Initialise(IButtonHoverCell aCell)
        {
            iCell = aCell;
            iCell.Initialise(this);
            iCell.EventClicked += ButtonClicked;

            iCell.SetEnabled(iEnabled);
            iCell.SetHovering(iHovering);
            iCell.SetOn(iOn);
        }

        private IButtonHoverCell iCell;
        private NSTrackingArea iTrackingArea;
        private bool iOn = false;
        private bool iHovering = false;
        private bool iEnabled = true;
        private IDragDestination iDragDelegate;
    }


    // interface for dragging destinations
    public interface IDragDestination
    {
        NSDragOperation DraggingEntered(INSDraggingInfo aInfo);
        bool PerformDragOperation(INSDraggingInfo aInfo);
    }


    // Cell interface to abstract the graphical appearance of the ButtonHoverPush class
    public interface IButtonHoverCell
    {
        void Initialise(NSView aParent);
        void Dealloc();

        void SetEnabled(bool aEnabled);
        void SetOn(bool aOn);
        void SetHovering(bool aHovering);

        float FittedWidth { get; }
        NSString KeyEquivalent { set; }

        event ActionEventHandler EventClicked;
    }


    // interface for buttons with changable image and text
    public interface IButtonHoverType2
    {
        NSImage ImageOff { set; }
        NSImage ImageOn { set; }
        float ImageWidth { set; }
        NSString Text { set; }
        bool TextOnLeft { set; }
        NSFont Font { set; }
    }


    // Button cell for Type 1 buttons:
    // - each state of the button is made of 1 image
    // - 6 states: no-hover, hover and push for on and off states
    public class ButtonHoverCellType1 : IButtonHoverCell
    {
        public ButtonHoverCellType1(NSImage aImageBkgd, NSImage aImageButtonUp, NSImage aImageButtonDown)
        {
            // a non-toggle button
            iImageBkgdOff = aImageBkgd;
            iImageButtonOffUp = aImageButtonUp;
            iImageButtonOffDown = aImageButtonDown;

            iImageBkgdOff.Retain();
            iImageButtonOffUp.Retain();
            iImageButtonOffDown.Retain();
        }

        public ButtonHoverCellType1(NSImage aImageBkgdOff, NSImage aImageButtonOffUp, NSImage aImageButtonOffDown,
                                    NSImage aImageBkgdOn, NSImage aImageButtonOnUp, NSImage aImageButtonOnDown)
        {
            // a toggle button
            iImageBkgdOff = aImageBkgdOff;
            iImageButtonOffUp = aImageButtonOffUp;
            iImageButtonOffDown = aImageButtonOffDown;
            iImageBkgdOn = aImageBkgdOn;
            iImageButtonOnUp = aImageButtonOnUp;
            iImageButtonOnDown = aImageButtonOnDown;

            iImageBkgdOff.Retain();
            iImageButtonOffUp.Retain();
            iImageButtonOffDown.Retain();
            iImageBkgdOn.Retain();
            iImageButtonOnUp.Retain();
            iImageButtonOnDown.Retain();
        }

        public void Initialise(NSView aParent)
        {
            // create background
            iBkgd = new NSImageView();
            iBkgd.InitWithFrame(aParent.Bounds);
            iBkgd.ImageAlignment = NSImageAlignment.NSImageAlignCenter;
            iBkgd.ImageFrameStyle = NSImageFrameStyle.NSImageFrameNone;
            iBkgd.ImageScaling = NSImageScaling.NSImageScaleNone;
            aParent.AddSubview(iBkgd);

            // create button
            iButton = new NSButton();
            iButton.InitWithFrame(aParent.Bounds);
            iButton.SetButtonType(NSButtonType.NSMomentaryChangeButton);
            iButton.IsBordered = false;
            iButton.Title = NSString.Empty;
            iButton.AlternateTitle = NSString.Empty;
            iButton.ImagePosition = NSCellImagePosition.NSImageOnly;
            iButton.AlphaValue = 0.0f;
            iBkgd.AddSubview(iButton);

            SetOn(false);
        }

        public void Dealloc()
        {
            iButton.RemoveFromSuperview();
            iButton.Release();

            iBkgd.RemoveFromSuperview();
            iBkgd.Release();

            iImageBkgdOff.Release();
            iImageButtonOffUp.Release();
            iImageButtonOffDown.Release();

            if (iImageBkgdOn != null)
            {
                iImageBkgdOn.Release();
                iImageButtonOnUp.Release();
                iImageButtonOnDown.Release();
            }
        }

        public void SetEnabled(bool aEnabled)
        {
            iButton.IsEnabled = aEnabled;
        }

        public void SetOn(bool aOn)
        {
            if (aOn && iImageBkgdOn != null)
            {
                iBkgd.Image = iImageBkgdOn;
                iButton.Image = iImageButtonOnUp;
                iButton.AlternateImage = iImageButtonOnDown;
            }
            else
            {
                iBkgd.Image = iImageBkgdOff;
                iButton.Image = iImageButtonOffUp;
                iButton.AlternateImage = iImageButtonOffDown;
            }
            iBkgd.NeedsDisplay = true;
        }

        public void SetHovering(bool aHovering)
        {
            iButton.Animator.AlphaValue = aHovering ? 1.0f : 0.0f;
        }

        public float FittedWidth
        {
            get { return iBkgd.Bounds.Width; }
        }

        public NSString KeyEquivalent
        {
            set { iButton.KeyEquivalent = value; }
        }

        public event ActionEventHandler EventClicked
        {
            add { iButton.ActionEvent += value; }
            remove { iButton.ActionEvent -= value; }
        }

        private NSButton iButton;
        private NSImageView iBkgd;
        private NSImage iImageBkgdOff;
        private NSImage iImageButtonOffUp;
        private NSImage iImageButtonOffDown;
        private NSImage iImageBkgdOn;
        private NSImage iImageButtonOnUp;
        private NSImage iImageButtonOnDown;
    }


    // Class to handle the drawing of type 2 hover buttons i.e. buttons that have:
    // - a background that consists of left, right and filler images
    // - a foreground image or text
    public class ButtonType2Drawer
    {
        public ButtonType2Drawer()
        {
        }

        public void Release()
        {
            Image = null;
            Text = null;
        }

        public NSImage Image
        {
            get
            {
                return iImage;
            }
            set
            {
                if (iImage != null)
                {
                    iImage.Release();
                }

                iImage = value;

                if (iImage != null)
                {
                    iImage.Retain();
                }
            }
        }

        public float ImageWidth
        {
            set
            {
                iImageWidth = value;
            }
        }

        public NSString Text
        {
            set
            {
                if (iText != null)
                {
                    iText.Release();
                }

                iText = value;

                if (iText != null)
                {
                    iText.Retain();
                }
            }
        }

        public bool TextLeft
        {
            set
            {
                iTextLeft = value;
            }
        }

        public NSFont Font
        {
            set
            {
                iFont = value;
            }
        }

        public void DrawNoHover(NSRect aRect)
        {
            Draw(aRect, Properties.Resources.ImageBoxLeft,
                 Properties.Resources.ImageBoxRight,
                 Properties.Resources.ImageBoxFiller);
        }

        public void DrawHover(NSRect aRect)
        {
            Draw(aRect, Properties.Resources.ImageBoxOverLeft,
                 Properties.Resources.ImageBoxOverRight,
                 Properties.Resources.ImageBoxOverFiller);
        }

        public void DrawDown(NSRect aRect)
        {
            Draw(aRect, Properties.Resources.ImageBoxDownLeft,
                 Properties.Resources.ImageBoxDownRight,
                 Properties.Resources.ImageBoxDownFiller);
        }

        public NSSize CalculateSize()
        {
            // assume images for the background are the same for the no-hover, hover and down states
            NSImage left = Properties.Resources.ImageBoxLeft;
            NSImage right = Properties.Resources.ImageBoxRight;

            return new NSSize(TextSize.width + ImageSize.width + left.Size.width + right.Size.width + 2.0f*iTextPadding, left.Size.height);
        }

        public void DrawText(NSRect aRect)
        {
            if (iText == null)
                return;

            // assume images for the background are the same for the no-hover, hover and down states
            NSImage left = Properties.Resources.ImageBoxLeft;
            NSImage right = Properties.Resources.ImageBoxRight;
            NSRect fillerRect = new NSRect(aRect.MinX + left.Size.width, aRect.MinY, aRect.Width - left.Size.width - right.Size.width, aRect.Height);

            // calculate the text rect
            NSSize textSz = TextSize;
            NSSize imgSz = ImageSize;
            NSRect textRect = new NSRect(iTextLeft ? fillerRect.MinX + iTextPadding: fillerRect.MinX + imgSz.width + iTextPadding,
                                         aRect.MidY - textSz.height*0.5f,
                                         fillerRect.Width - imgSz.width - 2.0f*iTextPadding,
                                         textSz.height);

            // draw
            iText.DrawInRectWithAttributes(textRect.IntegralRect(), TextAttributes);
        }

        private void Draw(NSRect aRect, NSImage aLeft, NSImage aRight, NSImage aFiller)
        {
            // draw the background
            NSRect leftRect = new NSRect(aRect.MinX, aRect.MinY, aLeft.Size.width, aRect.Height);
            aLeft.DrawInRectFromRectOperationFraction(leftRect, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);

            NSRect rightRect = new NSRect(aRect.MaxX - aRight.Size.width, aRect.MinY, aRight.Size.width, aRect.Height);
            aRight.DrawInRectFromRectOperationFraction(rightRect, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);

            NSRect fillerRect = new NSRect(aRect.MinX + aLeft.Size.width, aRect.MinY, aRect.Width - aLeft.Size.width - aRight.Size.width, aRect.Height);
            aFiller.DrawInRectFromRectOperationFraction(fillerRect, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);

            // draw image
            if (iText != null && iImage != null)
            {
                // button has text and an image
                NSSize imgSz = ImageSize;
                NSRect imgRect = new NSRect(iTextLeft ? fillerRect.MaxX - imgSz.width : fillerRect.MinX,
                                            aRect.MidY - imgSz.height*0.5f,
                                            imgSz.width, imgSz.height);

                // fit the image to the available rect
                imgRect = NSImageHelper.CentreImageInRect(iImage, imgRect);

                iImage.DrawInRectFromRectOperationFraction(imgRect.IntegralRect(), NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);
            }
            else if (iImage != null)
            {
                // image only - draw centred
                NSSize imgSz = ImageSize;
                NSRect dstRect = new NSRect(aRect.MidX - imgSz.width*0.5f, aRect.MidY - imgSz.height*0.5f,
                                            imgSz.width, imgSz.height);

                // fit the image to the available rect
                dstRect = NSImageHelper.CentreImageInRect(iImage, dstRect);

                iImage.DrawInRectFromRectOperationFraction(dstRect.IntegralRect(), NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);
            }
        }

        private NSSize ImageSize
        {
            get
            {
                if (iImage != null && iImageWidth == 0.0f)
                {
                    return iImage.Size;
                }
                else if (iImage != null)
                {
                    return new NSSize(iImageWidth, iImageWidth);
                }
                else
                {
                    return NSSize.NSZeroSize;
                }
            }
        }

        private NSSize TextSize
        {
            get
            {
                if (iText != null)
                {
                    NSSize sz = iText.SizeWithAttributes(TextAttributes);
                    return new NSSize((float)Math.Ceiling(sz.width), (float)Math.Ceiling(sz.height));
                }
                else
                {
                    return NSSize.NSZeroSize;
                }
            }
        }

        private NSDictionary TextAttributes
        {
            get
            {
                NSMutableParagraphStyle style = new NSMutableParagraphStyle();
                style.SetParagraphStyle(NSParagraphStyle.DefaultParagraphStyle);
                style.SetAlignment(NSTextAlignment.NSCenterTextAlignment);
                style.SetLineBreakMode(NSLineBreakMode.NSLineBreakByTruncatingTail);

                NSDictionary dict = NSDictionary.DictionaryWithObjectsAndKeys(iFont, NSAttributedString.NSFontAttributeName,
                                                                              style, NSAttributedString.NSParagraphStyleAttributeName,
                                                                              NSColor.WhiteColor, NSAttributedString.NSForegroundColorAttributeName,
                                                                              null);
                style.Release();

                return dict;
            }
        }

        private NSImage iImage;
        private float iImageWidth;
        private NSString iText;
        private bool iTextLeft;
        private float iTextPadding = 5.0f;
        private NSFont iFont = FontManager.FontLarge;
    }


    // View class to display the background for type 2 buttons
    [ObjectiveCClass]
    public class ButtonHoverType2Bkgd : NSView
    {
        public ButtonHoverType2Bkgd() : base() {}
        public ButtonHoverType2Bkgd(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            iDrawer.DrawNoHover(this.Bounds);
            iDrawer.DrawText(this.Bounds);
        }

        public ButtonType2Drawer Drawer
        {
            set { iDrawer = value; }
        }

        private ButtonType2Drawer iDrawer;
    }


    // Button cell for type 2 hover buttons
    [ObjectiveCClass]
    public class ButtonHoverType2Cell : NSButtonCell
    {
        public ButtonHoverType2Cell() : base () {}
        public ButtonHoverType2Cell(IntPtr aInstance) : base(aInstance) {}

        [ObjectiveCMessage("drawBezelWithFrame:inView:")]
        public override void DrawBezelWithFrameInView(NSRect aFrame, NSView aControlView)
        {
            NSAffineTransform xform = null;
            if (aControlView.IsFlipped)
            {
                // save graphics state for restoring later
                NSGraphicsContext.CurrentContext.SaveGraphicsState();

                xform = new NSAffineTransform();
                xform.TranslateXByYBy(0.0f, aFrame.Height);
                xform.ScaleXByYBy(1.0f, -1.0f);
                xform.Concat();
            }

            if (IsHighlighted)
            {
                iDrawer.DrawDown(aFrame);
            }
            else
            {
                iDrawer.DrawHover(aFrame);
            }

            if (xform != null)
            {
                xform.Release();
                NSGraphicsContext.CurrentContext.RestoreGraphicsState();
            }

            // draw the text after any transforms are applied
            iDrawer.DrawText(aFrame);
        }

        public ButtonType2Drawer Drawer
        {
            set { iDrawer = value; }
        }

        private ButtonType2Drawer iDrawer;
    }


    // Button hover cell for Type2 buttons:
    // - each button state is represented by 3 images that make the background and either 1 additional
    //   image for an icon or text
    // - there are 6 states: no-hover, hover and push for on and off states
    public class ButtonHoverCellType2 : IButtonHoverCell, IButtonHoverType2
    {
        public ButtonHoverCellType2()
        {
            iDrawerOff = new ButtonType2Drawer();
            iDrawerOn = new ButtonType2Drawer();
        }

        public void Initialise(NSView aParent)
        {
            // create the background
            iBkgd = new ButtonHoverType2Bkgd();
            iBkgd.InitWithFrame(aParent.Bounds);
            iBkgd.AutoresizingMask = NSResizingFlags.NSViewWidthSizable;
            aParent.AddSubview(iBkgd);

            // create the button
            iButton = new NSButton();
            iButton.InitWithFrame(aParent.Bounds);
            iButton.Cell = new ButtonHoverType2Cell();
            iButton.AutoresizingMask = NSResizingFlags.NSViewWidthSizable;

            iButton.SetButtonType(NSButtonType.NSMomentaryChangeButton);
            iButton.IsBordered = true;
            iButton.BezelStyle = NSBezelStyle.NSRegularSquareBezelStyle;
            iButton.AlphaValue = 0.0f;

            iButton.ImagePosition = NSCellImagePosition.NSImageOnly;
            iButton.Title = NSString.Empty;
            iButton.AlternateTitle = NSString.Empty;
            iButton.Image = null;
            iButton.AlternateImage = null;

            iBkgd.AddSubview(iButton);

            SetOn(false);
        }

        public void Dealloc()
        {
            iButton.RemoveFromSuperview();
            iButton.Release();

            iBkgd.RemoveFromSuperview();
            iBkgd.Release();

            iDrawerOff.Release();
            iDrawerOn.Release();
        }

        public void SetEnabled(bool aEnabled)
        {
            iButton.IsEnabled = aEnabled;
        }

        public void SetOn(bool aOn)
        {
            if (aOn && iDrawerOn.Image != null)
            {
                iBkgd.Drawer = iDrawerOn;
                iButton.Cell.CastTo<ButtonHoverType2Cell>().Drawer = iDrawerOn;
            }
            else
            {
                iBkgd.Drawer = iDrawerOff;
                iButton.Cell.CastTo<ButtonHoverType2Cell>().Drawer = iDrawerOff;
            }
            iBkgd.NeedsDisplay = true;
        }

        public void SetHovering(bool aHovering)
        {
            iButton.Animator.AlphaValue = aHovering ? 1.0f : 0.0f;
        }

        public float FittedWidth
        {
            get { return iDrawerOff.CalculateSize().width; }
        }

        public NSString KeyEquivalent
        {
            set { iButton.KeyEquivalent = value; }
        }

        public event ActionEventHandler EventClicked
        {
            add { iButton.ActionEvent += value; }
            remove { iButton.ActionEvent -= value; }
        }

        #region IButtonHoverType2 implementation
        public NSImage ImageOff
        {
            set
            {
                iDrawerOff.Image = value;
            }
        }

        public NSImage ImageOn
        {
            set
            {
                iDrawerOn.Image = value;
            }
        }

        public float ImageWidth
        {
            set
            {
                iDrawerOff.ImageWidth = value;
                iDrawerOn.ImageWidth = value;
            }
        }

        public NSString Text
        {
            set
            {
                iDrawerOff.Text = value;
                iDrawerOn.Text = value;
            }
        }

        public bool TextOnLeft
        {
            set
            {
                iDrawerOff.TextLeft = value;
                iDrawerOn.TextLeft = value;
            }
        }

        public NSFont Font
        {
            set
            {
                iDrawerOff.Font = value;
                iDrawerOn.Font = value;
            }
        }
        #endregion IButtonHoverType2 implementation

        private ButtonHoverType2Bkgd iBkgd;
        private NSButton iButton;
        private ButtonType2Drawer iDrawerOn;
        private ButtonType2Drawer iDrawerOff;
    }
}



