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
	/// Represents the right: css property.
	/// </summary>
	
	public class Right:CssProperty{
		
		
		public Right(){
			
			// This is along the x axis:
			IsXProperty=true;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"right"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			style.RightPositioned=true;
			
			if(value==null){
				style.PositionRight=0;
			}else{
				style.PositionRight=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



