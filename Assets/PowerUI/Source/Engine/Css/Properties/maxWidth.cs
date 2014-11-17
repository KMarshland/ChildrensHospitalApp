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
	/// Represents the max-width: css property.
	/// </summary>
	
	public class MaxWidth:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"max-width"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.MaximumWidth=int.MaxValue;
			}else{
				style.MaximumWidth=value.PX;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



