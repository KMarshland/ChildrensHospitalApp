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
	/// Represents the line-height: css property.
	/// </summary>
	
	public class LineHeight:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"line-height"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.LineHeight=1f;
			}else if(value.Type==Css.ValueType.Text && value.Text=="normal"){
				style.LineHeight=1f;
			}else{
				style.LineHeight=value.Single;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



