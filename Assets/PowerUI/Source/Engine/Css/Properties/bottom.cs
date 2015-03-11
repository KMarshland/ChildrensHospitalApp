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
	/// Represents the bottom: css property.
	/// </summary>
	
	public class Bottom:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"bottom"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			style.BottomPositioned=true;
			
			if(value==null){
				style.PositionBottom=0;
			}else{
				style.PositionBottom=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



