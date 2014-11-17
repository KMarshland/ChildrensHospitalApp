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
	/// Represents the left: css property.
	/// </summary>
	
	public class Left:CssProperty{
		
		
		public Left(){
			
			// This is along the x axis:
			IsXProperty=true;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"left"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			style.RightPositioned=false;
			
			if(value==null){
				style.PositionLeft=0;
			}else{
				style.PositionLeft=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



