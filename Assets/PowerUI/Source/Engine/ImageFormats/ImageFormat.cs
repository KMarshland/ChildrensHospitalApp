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
using Blaze;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Represents a specific type of image format, e.g. a video or an SVG.
	/// </summary>
	
	public class ImageFormat{
		
		/// <summary>The set of lowercase file types that this format will handle.</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>The height of the image.</summary>
		public virtual int Height{
			get{
				return 0;
			}
		}
		
		/// <summary>The width of the image.</summary>
		public virtual int Width{
			get{
				return 0;
			}
		}
		
		/// <summary>A single-frame image material. Used for e.g. videos and animations.</summary>
		public virtual Material ImageMaterial{
			get{
				return null;
			}
		}
		
		/// <summary>Resets this image format container.</summary>
		public virtual void Clear(){
		}
		
		/// <summary>Called when this image is going to be displayed.</summary>
		public virtual void GoingOnDisplay(){
		}
		
		/// <summary>Called when this image is going to stop being displayed.</summary>
		public virtual void GoingOffDisplay(){
		}
		
		/// <summary>Draws this image to the given atlas.</summary>
		public virtual bool DrawToAtlas(TextureAtlas atlas,AtlasLocation location){
			return false;
		}
		
	}
	
}