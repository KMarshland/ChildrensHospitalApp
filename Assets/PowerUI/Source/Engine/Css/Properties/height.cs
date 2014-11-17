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
	/// Represents the height: css property.
	/// </summary>
	
	public class Height:CssProperty{
		
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static Height GlobalProperty;
		
		
		public Height(){
			
			// It's the height property:
			IsWidthOrHeight=true;
			
			// Grab a global reference:
			GlobalProperty=this;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"height"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.FixedHeight=false;
				style.InnerHeight=0;
			}else{
				style.FixedHeight=true;
				style.InnerHeight=value.PX;
			}
			
			// Fire percent pass:
			style.Element.SetHeightForKids(style);
			
			style.SetSize();
			
			style.RequestLayout();
		}
		
	}
	
}



