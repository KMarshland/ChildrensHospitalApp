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
	/// Represents the white-space: css property.
	/// </summary>
	
	public class WhiteSpace:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"white-space"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				
				style.WhiteSpace=WhiteSpaceType.Normal;
				
			}else{
			
				if(value.Text=="nowrap"){
					style.WhiteSpace=WhiteSpaceType.NoWrap;
				}else{
					style.WhiteSpace=WhiteSpaceType.Normal;
				}
				
			}
			
			style.RequestLayout();
		}
		
	}
	
}



