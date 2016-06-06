
using System;
using System.Collections.Generic;

using Monobjc;
using Monobjc.Cocoa;


namespace Linn.Toolkit.Cocoa
{
    public class OptionPageMonobjc : IDisposable
    {
        public OptionPageMonobjc(IOptionPage aOptionsPage, NSRect aFrameRect)
        {

            iName = aOptionsPage.Name;

            iView = new NSView(aFrameRect);

            iControls = new List<IOptionMonobjc>();

            float y = iView.Frame.Height - 10;
            float mid = iView.Frame.Width * 0.5f;
            foreach (Option option in aOptionsPage.Options)
            {
                NSTextField label = new NSTextField();
                label.SetTitleWithMnemonic(new NSString(option.Name + ":"));
                label.IsSelectable = false;
                label.IsEditable = false;
                label.IsBordered = false;
                label.DrawsBackground = false;
                label.Alignment = NSTextAlignment.NSLeftTextAlignment;
                label.SizeToFit();
                label.Frame = new NSRect(10, y - label.Frame.Height, mid - 20, label.Frame.Height);

                iView.AddSubview(label);

                IOptionMonobjc o = null;

                if (option is OptionEnum || option is OptionNetworkInterface)
                {
                    o = new OptionNetworkInterfaceMonobjc(new NSRect(mid, y, mid - 10, 20), option);
                }
                else if (option is OptionFilePath)
				{
                    o = new OptionFilePathMonobjc(new NSRect(mid, y, mid - 10, 20), option);
				}
                else if (option is OptionFolderPath)
				{
                    o = new OptionFolderPathMonobjc(new NSRect(mid, y, mid - 10, 20), option);
				}
				else if (option is OptionBool)
				{
                    o = new OptionBoolMonobjc(new NSRect(mid, y, mid - 10, 20), option);
				}
                else if (option is OptionListFolderPath)
				{
					o = new OptionListFolderPathMonobjc(new NSRect(mid, y, mid - 10, 20), option);
                }

                if (o != null)
                {
                    iView.AddSubview(o.View);
                    y -= o.Height;
                    iControls.Add(o);
                }
            }
        }

        public NSView View
        {
            get { return iView; }
        }

        public void Dispose()
        {
            foreach (IOptionMonobjc control in iControls)
            {
                control.Dispose();
            }
        }

        public string Name
        {
            get
            {
                return iName;
            }
        }

        private NSView iView;
        private List<IOptionMonobjc> iControls;
        private string iName;
    }
}
