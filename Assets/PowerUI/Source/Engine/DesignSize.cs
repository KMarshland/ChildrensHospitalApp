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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

using System;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// This class represents design size information. Used by UI.Resolution to essentially match the visual appearance of your design on every screen.
	/// </summary>
	
	public class DesignSize{
		
		/// <summary>The DPI of this design. The default DPI is 72.</summary>
		public int Dpi=72;
		/// <summary>The inch width of your design.</summary>
		public float Width;
		/// <summary>The inch width of your design.</summary>
		public float Height;
		
		
		/// <summary>Used to make PowerUI match the visual appearance of a single design across all devices. 72 dpi is assumed.
		/// Provide the width and height of your design, and pass this object to a new ResolutionInfo object.
		/// It's reccommended that your design is quite large for the best appearance on all devices.</summary>
		/// <param name="width">The width of your design.</param>
		/// <param name="height">The height of your design.</param>
		public DesignSize(int width,int height){
			
			Height=ToInches(height);
			Width=ToInches(width);
			
		}
		
		/// <summary>Used to make PowerUI match the visual appearance of a single design across all devices, with a specified DPI of your design.
		/// Provide the width and height of your design, and pass this object to a new ResolutionInfo object.
		/// It's reccommended that your design is quite large for the best appearance on all devices.</summary>
		/// <param name="width">The width of your design.</param>
		/// <param name="height">The height of your design.</param>
		public DesignSize(int width,int height,int dpi){
			
			Dpi=dpi;
			
			Height=ToInches(height);
			Width=ToInches(width);
			
		}
		
		/// <summary>Gets the given pixel size as a size in inches.</summary>
		/// <param name="px">The pixel size.</param>
		/// <returns>The size in inches.</returns>
		private float ToInches(int px){
			
			return (float)px / (float)Dpi;
			
		}
		
		/// <summary>The resolution scale of this design size for the current screen.</summary>
		public float Scale{
			
			get{
				
				#if PRE_UNITY3_5
				float dpi=(float)Dpi;
				#else
				
				// Grab the screen DPI:
				float dpi=Screen.dpi;
				
				if(dpi==0f){
					
					// This commonly happens in the editor. Use the design DPI:
					dpi=(float)Dpi;
					
				}
				
				#endif
				
				// Get the screen size:
				int screenWidth=Screen.width;
				
				int screenHeight=Screen.height;
				
				// Get the screen size in inches:
				float inchWidth=(float)screenWidth / dpi;
				
				float inchHeight=(float)screenHeight / dpi;
				
				// Resize the design such that it gets resized as little as possible.
				
				float horizontalScale=inchWidth / Width;
				
				float verticalScale=inchHeight / Height;
				
				// Which axis results in the smallest distortion? This is the scale which is closest to one.
				
				// How far is horizontal from one?
				float horizontalDelta = 1f-horizontalScale;
				
				if(horizontalDelta<0f){
					
					horizontalDelta=-horizontalDelta;
					
				}
				
				// How far is vertical from one?
				float verticalDelta = 1f-verticalScale;
				
				if(verticalDelta<0f){
					
					verticalDelta=-verticalDelta;
					
				}
				
				if(horizontalDelta < verticalDelta){
					
					// Horizontal is closest.
					return horizontalScale;
					
				}
				
				// Vertical is closest.
				return verticalScale;
				
			}
			
		}
		
	}
	
}