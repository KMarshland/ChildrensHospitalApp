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
using UnityEngine;


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the direction: css property.
	/// </summary>
	
	public class Direction:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"direction"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				
				style.DrawDirection=DirectionType.LTR;
				
			}else{
			
				if(value.Text=="rtl"){
					style.DrawDirection=DirectionType.RTL;
				}else{
					style.DrawDirection=DirectionType.LTR;
				}
				
			}
				
			style.RequestLayout();
		}
		
	}
	
}



