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
	/// Represents the SPA animation format.
	/// </summary>
	
	public class SpaFormat:ImageFormat{
		
		/// <summary>The animation that is in use, if any.</summary>
		public SPA SPAFile;
		/// <summary>An instance of the animation retrieved.</summary>
		public SPAInstance Animation;
		
		
		public override string[] GetNames(){
			return new string[]{"spa"};
		}
		
		public override int Height{
			get{
				// We want the height of a frame:
				return SPAFile.FrameHeight;
			}
		}
		
		public override int Width{
			get{
				// We want the width of a frame:
				return SPAFile.FrameWidth;
			}
		}
		
		public override void Clear(){
			SPAFile=null;
		}
		
		public override void GoingOnDisplay(){
			
			if(Animation==null && SPAFile!=null){
				Animation=SPAFile.GetInstance();
			}
			
		}
		
		public override void GoingOffDisplay(){
		
			if(Animation!=null){
				Animation.Stop();
				Animation=null;
			}
			
		}
		
	}
	
}