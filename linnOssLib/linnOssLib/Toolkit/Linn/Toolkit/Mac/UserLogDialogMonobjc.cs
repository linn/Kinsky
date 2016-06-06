using System;

using Monobjc;
using Monobjc.Cocoa;

namespace Linn.Toolkit.Cocoa
{
	[ObjectiveCClass]
	public class UserLogDialogMonobjc : NSObject
	{
		public UserLogDialogMonobjc()
		{	
		}
		
		public UserLogDialogMonobjc(IntPtr aInstance)
			: base(aInstance)
		{
		}
		
		[ObjectiveCField]
		public NSTextView TextView;
		[ObjectiveCField]
		public NSWindow Window;
		
		[ObjectiveCMessage("awakeFromNib")]
		public void awakeFromNib()
		{
			Console.WriteLine("UserLog awakeFromNib...");
			TextView.TextStorage.MutableString.AppendString(new NSString(UserLog.Text));
		}
		
		/*public static void Create(string aText)
		{
			NSWindow window = new NSWindow(new NSRect(0, 0, 100, 100),
                                           NSWindowStyleMasks.NSClosableWindowMask |
                                           NSWindowStyleMasks.NSTitledWindowMask |
			                               NSWindowStyleMasks.NSResizableWindowMask,
                                           NSBackingStoreType.NSBackingStoreBuffered,
                                           false);
            NSView view = new NSView(new NSRect(0, 0, 100, 100));
			view.AutoresizesSubviews = true;
            window.SendMessage("setContentView:", new object[] { view });
			
			NSScrollView scrollView = new NSScrollView(new NSRect(-1, -1, 100, 100));
			scrollView.AutoresizingMask = //NSResizingFlags.NSViewMinXMargin | NSResizingFlags.NSViewMaxXMargin |
                //NSResizingFlags.NSViewMinYMargin | NSResizingFlags.NSViewMaxYMargin |
					NSResizingFlags.NSViewWidthSizable | NSResizingFlags.NSViewHeightSizable;
			scrollView.HorizontalLineScroll = 10;
			scrollView.VerticalLineScroll = 10;
			scrollView.HorizontalPageScroll = 10;
			scrollView.VerticalPageScroll = 10;
			scrollView.HasVerticalScroller = true;
			scrollView.AutoresizesSubviews = true;
			view.AddSubview(scrollView);
			
			NSTextView textView = new NSTextView(new NSRect(0, 0, 15, 15));//new NSRect(scrollView.Frame.MinX, scrollView.Frame.MinY, scrollView.Frame.Width, 1.0e7f));
			textView.MinSize = new NSSize(223, 0);
			textView.MaxSize = new NSSize(741, 1.0e7f);
			textView.AutoresizingMask = NSResizingFlags.NSViewMinXMargin | NSResizingFlags.NSViewMaxXMargin |
                NSResizingFlags.NSViewMinYMargin | NSResizingFlags.NSViewMaxYMargin |
					NSResizingFlags.NSViewWidthSizable | NSResizingFlags.NSViewHeightSizable;
			textView.IsVerticallyResizable = true;
			//textView.IsHorizontallyResizable = true;
			textView.AutoresizesSubviews = true;
			textView.TextContainer.WidthTracksTextView = true;
			textView.TextContainer.HeightTracksTextView = true;
			//textView.SizeToFit();
			scrollView.AddSubview(textView);
			textView.SizeToFit();
			
			//textView.IsEditable = false;
			//NSAttributedString s = new NSAttributedString(new NSString(aText));
			//textView.TextStorage.SetAttributedString(s);
			
			window.IsReleasedWhenClosed = true;
			window.MakeKeyAndOrderFront(null);
		}*/
	}
}