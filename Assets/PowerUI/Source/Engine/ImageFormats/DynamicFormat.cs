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
	/// Represents the built in dynamic image format.
	/// </summary>
	
	public class DynamicFormat:ImageFormat{
		
		/// <summary>The dynamic image retrieved.</summary>
		public DynamicTexture DynamicImage;
		
		
		public override int Height{
			get{
				return DynamicImage.Height;
			}
		}
		
		public override int Width{
			get{
				return DynamicImage.Width;
			}
		}
		
		public override void Clear(){
			DynamicImage=null;
		}
		
	}
	
}