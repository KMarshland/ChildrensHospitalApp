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
	/// Represents the border-image-slice: css property.
	/// </summary>
	
	public class BorderImageSlice:CssProperty{
		
		
		public BorderImageSlice(){
		
			// This is a rect property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"border-image-slice"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			// Tell it a colour changed:
			border.ColourChanged();
			
		}
		
	}
	
}



