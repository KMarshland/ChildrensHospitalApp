//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using PowerUI.Css;

namespace PowerUI{
	
	/// <summary>
	/// This protocol, dynamic:// is for dynamic textures.
	/// They draw directly to the texture atlas and achieve high performance.
	/// Used by e.g. Curved Healthbars.
	/// </summary>
	
	public class DynamicProtocol:FileProtocol{
		
		public DynamicProtocol(){
			UseResolution=false;
		}
		
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
		public override string[] GetNames(){
			
			return new string[]{"dynamic","dyn"};
			
		}
		
		public override void OnGetGraphic(ImagePackage package,FilePath path){
			package.GotGraphic(DynamicTexture.Get(path.Path));
		}
		
	}
	
}