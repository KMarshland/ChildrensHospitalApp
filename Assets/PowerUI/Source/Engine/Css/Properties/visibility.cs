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
	/// Represents the visibility: css property.
	/// </summary>
	
	public class Visibility:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"visibility"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			if(value==null){
				
				// Assume visible if blank:
				style.Visibility=VisibilityType.Visible;
				
			}else{
				
				if(value.Text=="hidden"){
					style.Visibility=VisibilityType.Hidden;
				}else{
					style.Visibility=VisibilityType.Visible;
				}
				
			}
			
			style.RequestLayout();
		}
		
	}
	
}



