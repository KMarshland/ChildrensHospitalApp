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
	/// Represents the top: css property.
	/// </summary>
	
	public class Top:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"top"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			style.BottomPositioned=false;
			
			if(value==null){
				style.PositionTop=0;
			}else{
				style.PositionTop=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



