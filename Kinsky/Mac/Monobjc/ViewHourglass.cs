
using System;

using Monobjc;
using Monobjc.Cocoa;


namespace KinskyDesktop
{
    [ObjectiveCClass]
    public class ViewHourglass : NSView
    {
        public ViewHourglass() : base() {}
        public ViewHourglass(IntPtr aInstance) : base(aInstance) {}

        public void Show(bool aShow)
        {
            if (aShow && IsHidden)
            {
                NSDate dateNow = NSDate.DateWithTimeIntervalSinceNow(0);
                iTimeShown = dateNow.TimeIntervalSinceReferenceDate;
            }

            IsHidden = !aShow;
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect aRect)
        {
            // calculate the number of tick intervals since the hourglass was shown
            NSDate dateNow = NSDate.DateWithTimeIntervalSinceNow(0);
            double timeElapsed = dateNow.TimeIntervalSinceReferenceDate - iTimeShown;
            int numIntervals = (int)(timeElapsed / 0.06);

            // save graphics state for restoring later
            NSGraphicsContext context = NSGraphicsContext.CurrentContext;
            context.SaveGraphicsState();

            // apply transforms to rotate the foreground image
            NSImage bkgd = Properties.Resources.IconHourglass;
            NSImage frgd = Properties.Resources.IconHourglass2;

            NSAffineTransform xform1 = new NSAffineTransform();
            xform1.TranslateXByYBy(Bounds.Width * 0.5f, Bounds.Height * 0.5f);
            xform1.Concat();

            NSAffineTransform xform2 = new NSAffineTransform();
            xform2.RotateByDegrees(-45.0f * (numIntervals % 8));
            xform2.Concat();

            bool scale = Bounds.Width < frgd.Size.width || Bounds.Height < frgd.Size.height;
            NSAffineTransform xform3 = null;
            if (scale)
            {
                float scaleX = Bounds.Width / frgd.Size.width;
                float scaleY = Bounds.Height / frgd.Size.height;
                float ratio = scaleX > scaleY ? scaleX : scaleY;
                xform3 = new NSAffineTransform();
                xform3.ScaleBy(ratio);
                xform3.Concat();
            }

            NSAffineTransform xform4 = new NSAffineTransform();
            xform4.TranslateXByYBy(-frgd.Size.width * 0.5f, -frgd.Size.height * 0.5f);
            xform4.Concat();


            // draw background and foreground
            bkgd.DrawAtPointFromRectOperationFraction(Bounds.origin, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);
            frgd.DrawAtPointFromRectOperationFraction(Bounds.origin, NSRect.NSZeroRect, NSCompositingOperation.NSCompositeSourceOver, 1.0f);

            // clean up
            xform1.Release();
            xform2.Release();
            if (scale)
            {
                xform3.Release();
            }
            xform4.Release();
            context.RestoreGraphicsState();

            // schedule another timer event
            this.PerformSelectorWithObjectAfterDelay(ObjectiveCRuntime.Selector("timerUpdate"), null, 0.06);
        }

        [ObjectiveCMessage("timerUpdate")]
        public void TimerUpdate()
        {
            NeedsDisplay = true;
        }

        private double iTimeShown;
    }
}



