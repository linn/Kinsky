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
	[Foundation.Register("CellPlaylistItemFactory")]
	public partial class CellPlaylistItemFactory {
		
		private CellPlaylistItem __mt_cellPlaylistItem;
		
		#pragma warning disable 0169
		[Foundation.Connect("cellPlaylistItem")]
		private CellPlaylistItem cellPlaylistItem {
			get {
				this.__mt_cellPlaylistItem = ((CellPlaylistItem)(this.GetNativeField("cellPlaylistItem")));
				return this.__mt_cellPlaylistItem;
			}
			set {
				this.__mt_cellPlaylistItem = value;
				this.SetNativeField("cellPlaylistItem", value);
			}
		}
	}
	
	// Base type probably should be UIKit.UITableViewCell or subclass
	[Foundation.Register("CellPlaylistItem")]
	public partial class CellPlaylistItem {
		
		private UIKit.UIImageView __mt_imageViewArtwork;
		
		private UIKit.UILabel __mt_labelTitle;
		
		private UIKit.UILabel __mt_labelDurationBitrate;
		
		private UIKit.UIImageView __mt_imageViewPlaying;
		
		private UIKit.UILabel __mt_labelAlbum;
		
		private UIKit.UILabel __mt_labelArtist;
		
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
		
		[Foundation.Connect("labelDurationBitrate")]
		private UIKit.UILabel labelDurationBitrate {
			get {
				this.__mt_labelDurationBitrate = ((UIKit.UILabel)(this.GetNativeField("labelDurationBitrate")));
				return this.__mt_labelDurationBitrate;
			}
			set {
				this.__mt_labelDurationBitrate = value;
				this.SetNativeField("labelDurationBitrate", value);
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
		
		[Foundation.Connect("labelAlbum")]
		private UIKit.UILabel labelAlbum {
			get {
				this.__mt_labelAlbum = ((UIKit.UILabel)(this.GetNativeField("labelAlbum")));
				return this.__mt_labelAlbum;
			}
			set {
				this.__mt_labelAlbum = value;
				this.SetNativeField("labelAlbum", value);
			}
		}
		
		[Foundation.Connect("labelArtist")]
		private UIKit.UILabel labelArtist {
			get {
				this.__mt_labelArtist = ((UIKit.UILabel)(this.GetNativeField("labelArtist")));
				return this.__mt_labelArtist;
			}
			set {
				this.__mt_labelArtist = value;
				this.SetNativeField("labelArtist", value);
			}
		}
	}
}
