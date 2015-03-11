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
	/// Represents the text-decoration-color: css property.
	/// </summary>
	
	public class TextDecorationColor:CssProperty{
		
		
		public TextDecorationColor(){
			
			// This is a color property:
			Type=ValueType.Color;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration-color"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			if(text.TextLine==null){
				return;
			}
			
			// Apply the property:
			if(value==null || value.Text=="initial"){
				// No longer custom:
				text.TextLine.ColourOverride=false;
			}else{
				// Set the colour:
				text.TextLine.SetColour(value.ToColor());
			}
			
			// Let it know a colour changed:
			text.ColourChanged();
			
		}
		
	}
	
}



