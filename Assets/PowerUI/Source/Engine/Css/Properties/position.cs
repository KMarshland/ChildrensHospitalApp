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
	/// Represents the position: css property.
	/// </summary>
	
	public class Position:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"position"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				// Assume relative:
				style.Position=PositionType.Relative;
			}else{
				
				if(value.Text=="fixed"){
					style.Position=PositionType.Fixed;
				}else if(value.Text=="absolute"){
					style.Position=PositionType.Absolute;
				}else{
					style.Position=PositionType.Relative;
				}
				
			}
			
			style.RequestLayout();
		}
		
	}
	
}



