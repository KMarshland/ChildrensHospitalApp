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
	/// Represents the border-style: css property.
	/// </summary>
	
	public class BorderStyleProperty:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-style"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			if(value==null || string.IsNullOrEmpty(value.Text) || value.Text=="solid"){
				
				border.Style=BorderStyle.Solid;
				
			}else if(value.Text=="dashed"){
			
				border.Style=BorderStyle.Dashed;
				
			}
			
			// Request a layout:
			border.RequestLayout();
			
		}
		
	}
	
}



