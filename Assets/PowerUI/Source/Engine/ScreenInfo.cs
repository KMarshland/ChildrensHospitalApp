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
using UnityEngine;

namespace PowerUI{

	/// <summary>
	/// Provides general information about the screen such as where the corners are in world units.
	/// Note: The world origin is in the middle of the screen.
	/// The provided world screen origin is the top left corner.
	/// </summary>

	public static class ScreenInfo{
		
		/// <summary>The screen width in pixels.</summary> 
		public static int ScreenX;
		/// <summary>The screen height in pixels.</summary>
		public static int ScreenY;
		/// <summary>The height of the screen in pixels as a float.</summary>
		public static float ScreenYFloat;
		/// <summary>The current resolution name of the UI.</summary>
		public static string ResolutionName;
		/// <summary>How many pixels are gained/lost per unit depth on screen size. Varies with field of view.</summary>
		public static float DepthDepreciation;
		/// <summary>The current resolution scale of the UI.</summary>
		public static float ResolutionScale=1f;
		/// <summary>The height/width of the screen in world units at zero depth.</summary>
		public static Vector2 WorldSize=Vector2.zero;
		/// <summary>The current resolution of this document.</summary>
		private static ResolutionInfo CurrentResolution;
		/// <summary>The amount of world units per screen pixel at zero depth.</summary>
		public static Vector2 WorldPerPixel=Vector2.zero;
		/// <summary>The location of the screen origin (top left corner) in world units at zero depth.</summary>
		public static Vector3 WorldScreenOrigin=Vector3.zero;
		
		
		/// <summary>Causes all settings here to be refreshed on the next update.</summary>
		public static void Clear(){
			ScreenX=ScreenY=0;
		}
		
		/// <summary> Checks if the screen size changed and repaints if it did.
		/// Called by <see cref="UI.Update"/>.</summary>
		public static void Update(){
		
			if(UI.GUICamera==null){
				return;
			}
			
			bool changedX=false;
			bool changedY=false;
			
			if(Screen.width!=ScreenX){
				ScreenX=Screen.width;
				changedX=true;
			}
			
			if(Screen.height!=ScreenY){
				ScreenY=Screen.height;
				ScreenYFloat=(float)ScreenY;
				changedY=true;
			}
			
			if(!changedX&&!changedY){
				return;
			}
			
			// Firstly, find the bottom left and top right corners at UI.CameraDistance z units away (zero z-index):
			Vector3 bottomLeft=UI.GUICamera.ScreenToWorldPoint(new Vector3(0f,0f,UI.CameraDistance));
			Vector3 topRight=UI.GUICamera.ScreenToWorldPoint(new Vector3(ScreenX,ScreenY,UI.CameraDistance));
			
			
			// With those, we can now find the size of the screen in world units:
			WorldSize.x=topRight.x-bottomLeft.x;
			WorldSize.y=topRight.y-bottomLeft.y;
			
			// Finally, calculate WorldPerPixel at zero depth:
			
			// Mapping PX to world units:
			WorldPerPixel.x=WorldSize.x/(float)ScreenX;
			WorldPerPixel.y=WorldSize.y/(float)ScreenY;
			
			// Set where the origin is. All rendering occurs relative to this point.
			// It's offset by 0.2 pixels to target a little closer to the middle of each pixel. This helps Pixel filtering look nice and clear.
			WorldScreenOrigin.y=bottomLeft.y + (0.4f * WorldPerPixel.y);
			WorldScreenOrigin.x=bottomLeft.x - (0.4f * WorldPerPixel.x);
			
			// Resize (NB: Internally will cause a layout request):
			if(changedX){
				UI.document.html.style.width=((int)((float)ScreenX/ResolutionScale))+"px";
			}
			if(changedY){
				UI.document.html.style.height=((int)((float)ScreenY/ResolutionScale))+"px";
			}
			
		}
		
		/// <summary>The current resolution of the UI.</summary>
		public static ResolutionInfo Resolution{
			get{
				return CurrentResolution;
			}
			set{
				CurrentResolution=value;
				
				if(value==null){
					ResolutionName=null;
					ResolutionScale=1f;
				}else{
					ResolutionScale=value.Scale;
					ResolutionName=value.Name;
					if(string.IsNullOrEmpty(ResolutionName)){
						ResolutionName=null;
					}
				}
			}
		}
		
	}
	
}