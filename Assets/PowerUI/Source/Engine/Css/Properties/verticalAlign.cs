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
	/// Represents the vertical-align: css property.
	/// </summary>
	
	public class VerticalAlign:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"v-align","vertical-align"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				
				style.VerticalAlign=VerticalAlignType.Top;
				
			}else{
				
				if(value.Text=="middle"){
					style.VerticalAlign=VerticalAlignType.Middle;
				}else if(value.Text=="bottom"){
					style.VerticalAlign=VerticalAlignType.Bottom;
				}else{
					style.VerticalAlign=VerticalAlignType.Top;
				}
				
			}
			
			style.RequestLayout();
		}
		
	}
	
}



