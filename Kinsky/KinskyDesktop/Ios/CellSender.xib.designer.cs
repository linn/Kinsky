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
	[Foundation.Register("CellSenderController")]
	public partial class CellSenderController {
		
		private CellSender __mt_cellSender;
		
		#pragma warning disable 0169
		[Foundation.Connect("cellSender")]
		private CellSender cellSender {
			get {
				this.__mt_cellSender = ((CellSender)(this.GetNativeField("cellSender")));
				return this.__mt_cellSender;
			}
			set {
				this.__mt_cellSender = value;
				this.SetNativeField("cellSender", value);
			}
		}
	}
	
	// Base type probably should be UIKit.UITableViewCell or subclass
	[Foundation.Register("CellSender")]
	public partial class CellSender {
		
		private UIKit.UIImageView __mt_imageViewArtwork;
		
		private UIKit.UILabel __mt_labelTitle;
		
		private UIKit.UIImageView __mt_imageViewPlaying;
		
		private UIKit.UIButton __mt_buttonRoom;
		
		private CellSenderController __mt_controller;
		
		#pragma warning disable 0169
		[Foundation.Connect("imageViewArtwork")]
		private UIKit.UIImageView imageViewArtwork {
			get {
				this.__mt_imageViewArtwork = ((UIKit.UIImageView)(this.GetNativeField("imageViewArtwork")));
				return this.__mt_imageViewArtwork;
			}
			set {
				this.__mt_imageViewArtwork = value;
				this.SetNativeField("imageViewArtwork", value);
			}
		}
		
		[Foundation.Connect("labelTitle")]
		private UIKit.UILabel labelTitle {
			get {
				this.__mt_labelTitle = ((UIKit.UILabel)(this.GetNativeField("labelTitle")));
				return this.__mt_labelTitle;
			}
			set {
				this.__mt_labelTitle = value;
				this.SetNativeField("labelTitle", value);
			}
		}
		
		[Foundation.Connect("imageViewPlaying")]
		private UIKit.UIImageView imageViewPlaying {
			get {
				this.__mt_imageViewPlaying = ((UIKit.UIImageView)(this.GetNativeField("imageViewPlaying")));
				return this.__mt_imageViewPlaying;
			}
			set {
				this.__mt_imageViewPlaying = value;
				this.SetNativeField("imageViewPlaying", value);
			}
		}
		
		[Foundation.Connect("buttonRoom")]
		private UIKit.UIButton buttonRoom {
			get {
				this.__mt_buttonRoom = ((UIKit.UIButton)(this.GetNativeField("buttonRoom")));
				return this.__mt_buttonRoom;
			}
			set {
				this.__mt_buttonRoom = value;
				this.SetNativeField("buttonRoom", value);
			}
		}
		
		[Foundation.Connect("controller")]
		private CellSenderController controller {
			get {
				this.__mt_controller = ((CellSenderController)(this.GetNativeField("controller")));
				return this.__mt_controller;
			}
			set {
				this.__mt_controller = value;
				this.SetNativeField("controller", value);
			}
		}
	}
}
