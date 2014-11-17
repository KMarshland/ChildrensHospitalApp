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
	/// Represents the border-radius: css property.
	/// </summary>
	
	public class BorderRadius:CssProperty{
		
		
		public BorderRadius(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"border-radius"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			if(value==null){
				// No corners:
				border.Corners=null;
			}else{
				
				// Apply top left:
				border.SetCorner(RoundCornerPosition.TopLeft,value.GetPX(0));
				
				// Apply top right:
				border.SetCorner(RoundCornerPosition.TopRight,value.GetPX(1));
				
				// Apply bottom right:
				border.SetCorner(RoundCornerPosition.BottomLeft,value.GetPX(2));
				
				// Apply bottom left:
				border.SetCorner(RoundCornerPosition.BottomRight,value.GetPX(3));
				
			}
			
			// Request a layout:
			border.RequestLayout();
			
		}
		
	}
	
}



