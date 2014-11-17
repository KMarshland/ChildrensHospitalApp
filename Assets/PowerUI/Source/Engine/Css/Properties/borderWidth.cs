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
	/// Represents the border-width: css property.
	/// </summary>
	
	public class BorderWidth:CssProperty{
		
		
		public BorderWidth(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"border-width"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			if(value==null){
				border.WidthTop=border.WidthLeft=0;
				border.WidthRight=border.WidthBottom=0;
			}else{
				border.WidthTop=value.GetPX(0);
				border.WidthRight=value.GetPX(1);
				border.WidthBottom=value.GetPX(2);
				border.WidthLeft=value.GetPX(3);
			}
			
			// Does the border have any corners? If so, we need to update them:
			if(border.Corners!=null){
				border.Corners.Recompute();
			}
			
			// Request a layout:
			border.RequestLayout();
			
			// Set the styles size:
			style.SetSize();
		}
		
	}
	
}



