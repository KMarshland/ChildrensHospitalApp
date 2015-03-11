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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the padding: css property.
	/// </summary>
	
	public class Padding:CssProperty{
		
		
		public Padding(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"padding"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
		
			if(value==null){
				
				style.PaddingTop=style.PaddingLeft=0;
				style.PaddingRight=style.PaddingBottom=0;
				
			}else{
				// Top:
				style.PaddingTop=value.GetPX(0);
				
				// Right:
				style.PaddingRight=value.GetPX(1);
				
				// Bottom:
				style.PaddingBottom=value.GetPX(2);
				
				// Left:
				style.PaddingLeft=value.GetPX(3);
			}
			
			style.SetSize();
			
			// Fire percent passes:
			style.Element.SetHeightForKids(style);
			style.Element.SetWidthForKids(style);
			
			style.RequestLayout();
		}
		
	}
	
}



