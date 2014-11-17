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
	/// Represents the max-height: css property.
	/// </summary>
	
	public class MaxHeight:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"max-height"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.MaximumHeight=int.MaxValue;
			}else{
				style.MaximumHeight=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



