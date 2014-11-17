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
	/// Represents the border-color: css property.
	/// </summary>
	
	public class BorderColor:CssProperty{
		
		
		public BorderColor(){
		
			// This is a rect property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"border-color"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			
			if(value!=null && value.Type==ValueType.Text){
				
				if(value.Text=="transparent"){
					// Currently assume the default colour (black):
					// (Use #00000000 instead)
					value=null;
				}
				
			}
			
			// Apply the base colour:
			border.BaseColour=value;
			
			// Reset the border colour:
			border.ResetColour();
			
			// Tell it a colour changed:
			border.ColourChanged();
			
		}
		
	}
	
}



