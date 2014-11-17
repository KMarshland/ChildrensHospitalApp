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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the color-overlay: css property.
	/// Specific to PowerUI. This overlays the given colour over any element.
	/// </summary>
	
	public class ColorOverlay:CssProperty{
		
		
		public ColorOverlay(){
			
			// This is a color property:
			Type=ValueType.Color;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"color-overlay"};
		}
		
		public override void SetDefault(Css.Value value,ValueType type){
			
			value.Set("#ffffffff",ValueType.Color);
			
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// The new overlay colour:
			Color overlay=Color.white;
			
			if(value!=null){
				overlay=value.ToColor();
			}
			
			// Apply it:
			style.ColorOverlay=overlay;
			
			
			// Special case here - everything needs to be told!
			if(style.BGImage!=null){
				style.BGImage.SetOverlayColour(overlay);
			}
			
			if(style.BGColour!=null){
				style.BGColour.SetOverlayColour(overlay);
			}
			
			if(style.Border!=null){
				style.Border.SetOverlayColour(overlay);
			}
			
			if(style.Text!=null){
				style.Text.SetOverlayColour(overlay);
			}
			
		}
		
	}
	
}



