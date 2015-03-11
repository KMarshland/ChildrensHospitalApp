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
	/// Represents the min-width: css property.
	/// </summary>
	
	public class MinWidth:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"min-width"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.MinimumWidth=int.MinValue;
			}else{
				style.MinimumWidth=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



