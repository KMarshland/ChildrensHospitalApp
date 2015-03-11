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
	/// Represents the float: css property.
	/// </summary>
	
	public class Float:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"float"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				
				// Assume none if blank:
				style.Float=FloatType.None;
				
			}else{
			
				switch(value.Text){
					case "left":
						
						// Float left:
						style.Float=FloatType.Left;
						
					break;
					case "right":
						
						// Float right:
						style.Float=FloatType.Right;
						
					break;
					default:
						
						// No float (default):
						style.Float=FloatType.None;
						
					break;
				}
				
			}
			
			style.RequestLayout();
			
		}
		
	}
	
}



