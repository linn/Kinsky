// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace KinskyTouch {
	
	
	// Base type probably should be UIKit.UIViewController or subclass
	[Foundation.Register("ViewWidgetVolumeButtons")]
	public partial class ViewWidgetVolumeButtons {
		
		private UIKit.UIView __mt_view;
		
		private UIKit.UIButton __mt_buttonDown;
		
		private UIKit.UIButton __mt_buttonMute;
		
		private UIKit.UIButton __mt_buttonUp;
		
		#pragma warning disable 0169
		[Foundation.Connect("view")]
		private UIKit.UIView view {
			get {
				this.__mt_view = ((UIKit.UIView)(this.GetNativeField("view")));
				return this.__mt_view;
			}
			set {
				this.__mt_view = value;
				this.SetNativeField("view", value);
			}
		}
		
		[Foundation.Connect("buttonDown")]
		private UIKit.UIButton buttonDown {
			get {
				this.__mt_buttonDown = ((UIKit.UIButton)(this.GetNativeField("buttonDown")));
				return this.__mt_buttonDown;
			}
			set {
				this.__mt_buttonDown = value;
				this.SetNativeField("buttonDown", value);
			}
		}
		
		[Foundation.Connect("buttonMute")]
		private UIKit.UIButton buttonMute {
			get {
				this.__mt_buttonMute = ((UIKit.UIButton)(this.GetNativeField("buttonMute")));
				return this.__mt_buttonMute;
			}
			set {
				this.__mt_buttonMute = value;
				this.SetNativeField("buttonMute", value);
			}
		}
		
		[Foundation.Connect("buttonUp")]
		private UIKit.UIButton buttonUp {
			get {
				this.__mt_buttonUp = ((UIKit.UIButton)(this.GetNativeField("buttonUp")));
				return this.__mt_buttonUp;
			}
			set {
				this.__mt_buttonUp = value;
				this.SetNativeField("buttonUp", value);
			}
		}
	}
}
