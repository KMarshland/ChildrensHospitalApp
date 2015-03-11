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
	/// Represents the text-align: css property.
	/// </summary>
	
	public class TextAlign:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"text-align"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				
				// Assume auto:
				style.HorizontalAlign=HorizontalAlignType.Auto;
				
			}else{
			
				switch(value.Text){
					case "right":
						style.HorizontalAlign=HorizontalAlignType.Right;
					break;
					case "justify":
						style.HorizontalAlign=HorizontalAlignType.Justify;
					break;
					case "center":
						style.HorizontalAlign=HorizontalAlignType.Center;
					break;
					case "left":
						style.HorizontalAlign=HorizontalAlignType.Left;
					break;
					default:
						style.HorizontalAlign=HorizontalAlignType.Auto;
					break;
				}
				
			}
			
			style.RequestLayout();
		}
		
	}
	
}



