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
	/// Represents the min-height: css property.
	/// </summary>
	
	public class MinHeight:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"min-height"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.MinimumHeight=int.MinValue;
			}else{
				style.MinimumHeight=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



