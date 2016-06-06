using System;
using System.Collections.Generic;

using Monobjc;
using Monobjc.Cocoa;
using System.Runtime.InteropServices;

namespace Linn.Toolkit.Cocoa
{
    public interface IOptionMonobjc : IDisposable
    {
        NSView View { get; }
        float Height { get; }
    }


    // base class for option controls to handle the interaction with the Option object
    // that the control represents
    public abstract class OptionMonobjc<T> : IOptionMonobjc where T : Option
    {
        protected OptionMonobjc(T aOption)
        {
            iOption = aOption;
            iOption.EventValueChanged += ValueChanged;
            iOption.EventAllowedChanged += AllowedChanged;
        }

        public void Dispose()
        {
            iOption.EventValueChanged -= ValueChanged;
            iOption.EventAllowedChanged -= AllowedChanged;
        }

        public abstract NSView View { get; }

        public float Height
        {
            get { return View.Frame.Height; }
        }

        protected abstract void OptionValueChanged(object sender, EventArgs e);
        protected abstract void OptionAllowedChanged(object sender, EventArgs e);

        private void ValueChanged(object sender, EventArgs e)
        {
            if (View.InvokeRequired)
            {
                View.BeginInvoke(new EventHandler<EventArgs>(OptionValueChanged), sender, e);
            }
            else
            {
                OptionValueChanged(sender, e);
            }
        }

        private void AllowedChanged(object sender, EventArgs e)
        {
            if (View.InvokeRequired)
            {
                View.BeginInvoke(new EventHandler<EventArgs>(OptionAllowedChanged), sender, e);
            }
            else
            {
                OptionAllowedChanged(sender, e);
            }
        }

        protected T iOption;
    }


    // Control for OptionEnumerated
    public class OptionEnumeratedMonobjc : OptionMonobjc<Option>
    {
        public OptionEnumeratedMonobjc(NSRect aRect, Option aOption)
            : base(aOption)
        {
            iPopUpButton = new NSPopUpButton();
			iPopUpButton.SizeToFit();
			iPopUpButton.Frame = new NSRect(aRect.MinX, aRect.MinY - iPopUpButton.Frame.Height, aRect.Width, iPopUpButton.Frame.Height);

            OptionAllowedChanged(this, EventArgs.Empty);

            iPopUpButton.ActionEvent += ActionEvent;
        }

        public override NSView View
        {
            get { return iPopUpButton; }
        }

        protected override void OptionAllowedChanged(object sender, EventArgs e)
        {
            iPopUpButton.RemoveAllItems();

            foreach (string s in iOption.Allowed)
            {
                iPopUpButton.AddItemWithTitle(new NSString(s));
            }

            OptionValueChanged(this, EventArgs.Empty);
        }

        protected override void OptionValueChanged(object sender, EventArgs e)
        {
            iPopUpButton.SelectItemWithTitle(new NSString(iOption.Value));
        }

        private void ActionEvent(Id aSender)
        {
            NSPopUpButton button = aSender.CastTo<NSPopUpButton>();
            iOption.Set(button.SelectedItem.Title.ToString());
        }

        private NSPopUpButton iPopUpButton;
    }

    // Control for OptionNetworkInterfaceMonobjc
    public class OptionNetworkInterfaceMonobjc : OptionMonobjc<Option>
    {
        public OptionNetworkInterfaceMonobjc(NSRect aRect, Option aOption)
            : base(aOption)
        {
            iPopUpButton = new NSPopUpButton();
            iPopUpButton.SizeToFit();
            iPopUpButton.Frame = new NSRect(aRect.MinX, aRect.MinY - iPopUpButton.Frame.Height, aRect.Width, iPopUpButton.Frame.Height);

            OptionAllowedChanged(this, EventArgs.Empty);

            iPopUpButton.ActionEvent += ActionEvent;
        }

        public override NSView View
        {
            get { return iPopUpButton; }
        }

        protected override void OptionAllowedChanged(object sender, EventArgs e)
        {
            iPopUpButton.RemoveAllItems();
            Dictionary<string, string> lookup = GetInterfaceFriendlyNamesMap();

            foreach (string s in iOption.Allowed)
            {
                string bsdName = s;
                string title = bsdName;
                if (lookup.ContainsKey(bsdName))
                {
                    title = lookup[bsdName];
                }
                iPopUpButton.AddItemWithTitle(new NSString(title));
            }

            OptionValueChanged(this, EventArgs.Empty);
        }

        protected override void OptionValueChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> lookup = GetInterfaceFriendlyNamesMap();
            string bsdName = iOption.Value;
            string title = bsdName;
            if (lookup.ContainsKey(bsdName))
            {
                title = lookup[bsdName];
            }
            iPopUpButton.SelectItemWithTitle(new NSString(title));
        }

        private void ActionEvent(Id aSender)
        {
            NSPopUpButton button = aSender.CastTo<NSPopUpButton>();
            Dictionary<string, string> lookup = GetInterfaceFriendlyNamesMap();
            string title = button.SelectedItem.Title.ToString();
            foreach(string key in lookup.Keys)
            {
                if (lookup[key] == title)
                {
                    title = key;
                }
            }
            iOption.Set(title);
        }

        private Dictionary<string, string> GetInterfaceFriendlyNamesMap()
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            NSArray adapters = new Id(SCNetworkInterfaceCopyAll()).CastAs<NSArray>();
            foreach(Id adapter in adapters)
            {
                Id bsdNameId = new Id(SCNetworkInterfaceGetBSDName(adapter.NativePointer));
                NSString bsdName = bsdNameId.CastAs<NSString>();

                Id displayNameId = new Id(SCNetworkInterfaceGetLocalizedDisplayName(adapter.NativePointer));
                NSString displayName = displayNameId.CastAs<NSString>();

                results.Add(bsdName.ToString(), displayName.ToString());
            }
            adapters.Release();
            return results;
        }

        private NSPopUpButton iPopUpButton;

        
        [DllImport ("/System/Library/Frameworks/SystemConfiguration.framework/Versions/Current/SystemConfiguration")]
        private static extern IntPtr SCNetworkInterfaceCopyAll();

        [DllImport ("/System/Library/Frameworks/SystemConfiguration.framework/Versions/Current/SystemConfiguration")]
        private static extern IntPtr SCNetworkInterfaceGetBSDName(IntPtr aInterface);

        [DllImport ("/System/Library/Frameworks/SystemConfiguration.framework/Versions/Current/SystemConfiguration")]
        private static extern IntPtr SCNetworkInterfaceGetLocalizedDisplayName(IntPtr aInterface);
    }

    // Abstract control for a path
    public abstract class OptionPathMonobjc : OptionMonobjc<Option>
    {
        protected OptionPathMonobjc(NSRect aRect, Option aOption)
            : base(aOption)
        {
            iView = new NSView();
            
            iTextField = new NSTextField();
            iTextField.SetTitleWithMnemonic(new NSString(iOption.Value));
            iTextField.IsEditable = false;
            iTextField.IsSelectable = false;
            //textField.AutoresizingMask = NSResizingFlags.NSViewWidthSizable | NSResizingFlags.NSViewMaxXMargin | NSResizingFlags.NSViewMaxYMargin;

            iButtonChange = new NSButton();
            iButtonChange.BezelStyle = NSBezelStyle.NSRoundedBezelStyle;
            iButtonChange.Title = new NSString("Change...");
            iButtonChange.SizeToFit();
			
            iButtonChange.ActionEvent += ActionEventChange;
			
            float height = 60 + iButtonChange.Frame.Height;
			
            iView.Frame = new NSRect(aRect.MinX, aRect.MinY - height, aRect.Width, height);
            iTextField.Frame = new NSRect(0, height - 60, aRect.Width, 60);
            iButtonChange.Frame = new NSRect(iView.Frame.Width - iButtonChange.Frame.Width, iTextField.Frame.MinY - iButtonChange.Frame.Height, iButtonChange.Frame.Width, iButtonChange.Frame.Height);

            iView.AddSubview(iTextField);
            iView.AddSubview(iButtonChange);
        }

        public override NSView View
        {
            get { return iView; }
        }

        protected abstract void ActionEventChange(Id aSender);

        protected override void OptionValueChanged(object sender, EventArgs e)
        {
            iTextField.SetTitleWithMnemonic(new NSString(iOption.Value));
        }

        protected override void OptionAllowedChanged(object sender, EventArgs e)
        {
        }

		private NSButton iButtonChange;
		private NSTextField iTextField;
        protected NSView iView;
    }


    // Control for a file path option
    public class OptionFilePathMonobjc : OptionPathMonobjc
    {
        public OptionFilePathMonobjc(NSRect aRect, Option aOptionPath)
            : base(aRect, aOptionPath)
        {
        }

        protected override void ActionEventChange(Id aSender)
        {
            NSOpenPanel panel = NSOpenPanel.OpenPanel;

            panel.CanChooseFiles = true;
            panel.CanChooseDirectories = false;
			panel.AllowsMultipleSelection = false;
            panel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(new NSString(iOption.Value), new NSString("Untitled"), iView.Window, ChangeDidEnd, IntPtr.Zero);
        }

        private void ChangeDidEnd(NSOpenPanel aPanel, int aReturnCode, IntPtr aContextInfo)
        {
            if (aReturnCode == NSOpenPanel.NSOKButton)
            {
                if (aPanel == null)
                {
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 aPanel is null ({1}, {2})", DateTime.Now, iOption.Name, iOption.Value));
                }
                if (aPanel.Filename == null)
                {
                    NSURL url = aPanel.SendMessage<NSURL>("URL");

                    UserLog.WriteLine(string.Format("{0}: Logging for #795 Filename is null ({1}, {2})", DateTime.Now, iOption.Name, iOption.Value));
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 URL ({1})", DateTime.Now, (url != null) ? url.ToString() : "null"));
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 URLs count ({1})", DateTime.Now, aPanel.URLs.Count));
                    for (uint i=0 ; i<aPanel.URLs.Count ; i++)
                    {
                        url = aPanel.URLs[(int)i].SafeCastAs<NSURL>();
                        UserLog.WriteLine(string.Format("{0}: Logging for #795 URLs[{1}] ({2})", DateTime.Now, i, (url != null) ? url.ToString() : "null"));
                    }
                }
				if (aPanel.Filename != null)
				{
                	iOption.Set(aPanel.Filename.ToString());
				}else if(aPanel.URLs.Count > 0)//#795: root unc share comes through with null Directory, not sure whether this can happen with filename property too, so have put this in for safety.
				{
					NSURL url = aPanel.URLs[0].SafeCastAs<NSURL>();
                	iOption.Set( url.SendMessage<NSString>("path").ToString());
				}
            }
        }
    }


    // Control for a folder path option
    public class OptionFolderPathMonobjc : OptionPathMonobjc
    {
        public OptionFolderPathMonobjc(NSRect aRect, Option aOptionPath)
            : base(aRect, aOptionPath)
        {
        }

        protected override void ActionEventChange(Id aSender)
        {
            NSOpenPanel panel = NSOpenPanel.OpenPanel;

            panel.CanChooseFiles = false;
            panel.CanChooseDirectories = true;
			panel.AllowsMultipleSelection = false;
            panel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(new NSString(iOption.Value), new NSString("Untitled"), iView.Window, ChangeDidEnd, IntPtr.Zero);
        }

        private void ChangeDidEnd(NSOpenPanel aPanel, int aReturnCode, IntPtr aContextInfo)
        {
            if (aReturnCode == NSOpenPanel.NSOKButton)
            {
                if (aPanel == null)
                {
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 aPanel is null ({1}, {2})", DateTime.Now, iOption.Name, iOption.Value));
                }
                if (aPanel.Directory == null)
                {
                    NSURL url = aPanel.SendMessage<NSURL>("directoryURL");

                    UserLog.WriteLine(string.Format("{0}: Logging for #795 directory is null ({1}, {2})", DateTime.Now, iOption.Name, iOption.Value));
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 directoryURL ({1})", DateTime.Now, (url != null) ? url.ToString() : "null"));
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 URLs count ({1})", DateTime.Now, aPanel.URLs.Count));
                    for (uint i=0 ; i<aPanel.URLs.Count ; i++)
                    {
                        url = aPanel.URLs[(int)i].SafeCastAs<NSURL>();
                        UserLog.WriteLine(string.Format("{0}: Logging for #795 URLs[{1}] ({2})", DateTime.Now, i, (url != null) ? url.ToString() : "null"));
                    }
                }
				if (aPanel.Directory != null)
				{
                	iOption.Set(aPanel.Directory.ToString());
				}else if(aPanel.URLs.Count > 0)//#795: root unc share comes through with null Directory
				{
					NSURL url = aPanel.URLs[0].SafeCastAs<NSURL>();
                	iOption.Set(url.SendMessage<NSString>("path").ToString());
				}
            }
        }
    }
	

    // Control for an OptionBool
    public class OptionBoolMonobjc : OptionMonobjc<OptionBool>
	{
		public OptionBoolMonobjc(NSRect aRect, Option aOption)
            : base(aOption as OptionBool)
		{
			iButton = new NSButton();
			iButton.SetButtonType(NSButtonType.NSSwitchButton);
			iButton.BezelStyle = NSBezelStyle.NSRegularSquareBezelStyle;
			iButton.SetTitleWithMnemonic(new NSString(""));
			iButton.SizeToFit();
			iButton.Frame = new NSRect(aRect.MinX, aRect.MinY - iButton.Frame.Height, aRect.Width, iButton.Frame.Height);
			
			iButton.State = iOption.Native ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
			
			iButton.ActionEvent += ActionEvent;
		}
		
		public override NSView View
		{
			get { return iButton; }
		}

        protected override void OptionValueChanged (object sender, EventArgs e)
        {
            iButton.State = iOption.Native ? NSCellStateValue.NSOnState : NSCellStateValue.NSOffState;
        }

        protected override void OptionAllowedChanged (object sender, EventArgs e)
        {
        }

		private void ActionEvent(Id aSender)
        {
            NSButton button = aSender.CastTo<NSButton>();
            iOption.Native = (button.State == NSCellStateValue.NSOnState);
        }
		
		private NSButton iButton;
	}
	

    // Data source for the control for a list option
    [ObjectiveCClass]
	public class OptionListMonobjcDataSource : NSObject
	{
		public OptionListMonobjcDataSource() {}
		public OptionListMonobjcDataSource(IntPtr aInstance) : base(aInstance) {}
		
		public void SetList(IList<string> aList)
		{
			iList = aList;
		}

		[ObjectiveCMessage("numberOfRowsInTableView:")]
		public int numberOfRowsInTableView(NSTableView aTableView)
		{
			return iList.Count;
		}
		
		[ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
		public NSObject tableView_objectValueForTableColumn_row(NSTableView aTableView, NSTableColumn aTableColumn, int aRowIndex)
		{
			return new NSString(iList[aRowIndex]);
		}
		
        private IList<string> iList;
	}


    // Abstract control for a list option
    public abstract class OptionListMonobjc : OptionMonobjc<Option>
	{
		public OptionListMonobjc(NSRect aRect, Option aOption)
            : base(aOption)
		{
            iList = new List<string>(StringListConverter.StringToList(iOption.Value));
            iDataSource = new OptionListMonobjcDataSource();
            iDataSource.SetList(iList);

			iView = new NSView();
			
			iButtonAdd = new NSButton();
			iButtonAdd.BezelStyle = NSBezelStyle.NSRoundedBezelStyle;
            iButtonAdd.Title = new NSString("Add");
            iButtonAdd.SizeToFit();
			
			iButtonAdd.ActionEvent += ActionEventAdd;
			
			iButtonRemove = new NSButton();
			iButtonRemove.BezelStyle = NSBezelStyle.NSRoundedBezelStyle;
			iButtonRemove.SetTitleWithMnemonic(new NSString("Remove"));
			iButtonRemove.SizeToFit();			
			float height = 100 + iButtonAdd.Frame.Height;
			
			iButtonRemove.ActionEvent += ActionEventRemove;
			
			iView.Frame = new NSRect(aRect.MinX, aRect.MinY - height, aRect.Width, height);
			
			NSScrollView scrollView = new NSScrollView();
			
			scrollView.HasVerticalScroller = true;
			scrollView.HasHorizontalScroller = true;
			scrollView.AutohidesScrollers = true;
			
			NSTableColumn tableColumn = new NSTableColumn();
			tableColumn.ResizingMask = NSTableColumnResizingMasks.NSTableColumnAutoresizingMask | NSTableColumnResizingMasks.NSTableColumnUserResizingMask;
			tableColumn.IsEditable = false;
			
			iTableView = new NSTableView();
			iTableView.DataSource = iDataSource;
			iTableView.HeaderView = null;
			iTableView.UsesAlternatingRowBackgroundColors = true;
			iTableView.AddTableColumn(tableColumn);
			
			scrollView.Frame = new NSRect(0, height - 100, aRect.Width, 100);
			iTableView.Frame = scrollView.ContentView.Bounds;
			tableColumn.Width = iTableView.Bounds.Width - 3;
			
			scrollView.DocumentView = iTableView;
			
			iButtonAdd.Frame = new NSRect(iView.Frame.Width - iButtonAdd.Frame.Width - iButtonRemove.Frame.Width, scrollView.Frame.MinY - iButtonAdd.Frame.Height, iButtonAdd.Frame.Width, iButtonAdd.Frame.Height);
			iButtonRemove.Frame = new NSRect(iView.Frame.Width - iButtonRemove.Frame.Width, scrollView.Frame.MinY - iButtonRemove.Frame.Height, iButtonRemove.Frame.Width, iButtonRemove.Frame.Height);
			
			iView.AddSubview(scrollView);
			iView.AddSubview(iButtonAdd);
			iView.AddSubview(iButtonRemove);
			
			iTableView.ReloadData();
		}
		
		public override NSView View
		{
			get { return iView; }
		}
        
		protected abstract void OnAdd();
		
		private void ActionEventAdd(Id aSender)
		{
			OnAdd();
		}
		
		private void ActionEventRemove(Id aSender)
		{
			NSIndexSet rows = iTableView.SelectedRowIndexes;
			
			uint index = rows.LastIndex;
			while(index != FoundationFramework.NSNotFound)
			{
				if(index < iList.Count)
				{
					iList.RemoveAt((int)index);
                    iOption.Set(StringListConverter.ListToString(iList));
                }
				index = rows.IndexLessThanIndex(index);
			}
		}

        protected override void OptionValueChanged(object sender, EventArgs e)
        {
            iList = new List<string>(StringListConverter.StringToList(iOption.Value));
            iDataSource.SetList(iList);
            iTableView.ReloadData();
        }

        protected override void OptionAllowedChanged(object sender, EventArgs e)
        {
        }

        protected void Add(string aValue)
        {
            iList.Add(aValue);
            iOption.Set(StringListConverter.ListToString(iList));
        }
		
		protected NSView iView;
		
		private NSTableView iTableView;
		private NSButton iButtonAdd;
		private NSButton iButtonRemove;
		
        private List<string> iList;
        private OptionListMonobjcDataSource iDataSource;
	}

    // Control for a list of folders option
	public class OptionListFolderPathMonobjc : OptionListMonobjc
	{
		public OptionListFolderPathMonobjc(NSRect aRect, Option aOption)
			: base(aRect, aOption)
		{
		}
		
		protected override void OnAdd ()
		{
			NSOpenPanel panel = NSOpenPanel.OpenPanel;

            panel.CanChooseFiles = false;
            panel.CanChooseDirectories = true;
			panel.AllowsMultipleSelection = false;
            panel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(new NSString(""), new NSString(iOption.Description), iView.Window, ChangeDidEnd, IntPtr.Zero);
        }

        private void ChangeDidEnd(NSOpenPanel aPanel, int aReturnCode, IntPtr aContextInfo)
        {
            if(aReturnCode == NSOpenPanel.NSOKButton)
            {
                if (aPanel == null)
                {
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 aPanel is null ({1}, {2})", DateTime.Now, iOption.Name, iOption.Value));
                }
                if (aPanel.Directory == null)
                {
                    NSURL url = aPanel.SendMessage<NSURL>("directoryURL");

                    UserLog.WriteLine(string.Format("{0}: Logging for #795 directory is null ({1}, {2})", DateTime.Now, iOption.Name, iOption.Value));
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 directoryURL ({1})", DateTime.Now, (url != null) ? url.ToString() : "null"));
                    UserLog.WriteLine(string.Format("{0}: Logging for #795 URLs count ({1})", DateTime.Now, aPanel.URLs.Count));
                    for (uint i=0 ; i<aPanel.URLs.Count ; i++)
                    {
                        url = aPanel.URLs[(int)i].SafeCastAs<NSURL>();
                        UserLog.WriteLine(string.Format("{0}: Logging for #795 URLs[{1}] ({2})", DateTime.Now, i, (url != null) ? url.ToString() : "null"));
                    }
                }
				if (aPanel.Directory != null)
				{
                	Add(aPanel.Directory.ToString());
				}else if(aPanel.URLs.Count > 0) //#795: root unc share comes through with null Directory
				{
					NSURL url = aPanel.URLs[0].SafeCastAs<NSURL>();
                	Add(url.SendMessage<NSString>("path").ToString());
				}
            }
        }
	}
}

