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
	/// Represents the color: css property.
	/// </summary>
	
	public class ColorProperty:CssProperty{
		
		
		public ColorProperty(){
			IsTextual=true;
			
			// This is a color property:
			Type=ValueType.Color;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"color"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				text.BaseColour=Color.black;
			}else{
				text.BaseColour=value.ToColor();
			}
			
			// Let it know a colour changed:
			text.ColourChanged();
		}
		
	}
	
}



