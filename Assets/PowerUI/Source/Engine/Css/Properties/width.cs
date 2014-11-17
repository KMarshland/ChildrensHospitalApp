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
	/// Represents the width: css property.
	/// </summary>
	
	public class Width:CssProperty{
		
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static Width GlobalProperty;
		
		
		public Width(){
			
			// It's along x:
			IsXProperty=true;
			
			// It's the width property:
			IsWidthOrHeight=true;
			
			// Grab a global reference:
			GlobalProperty=this;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"width"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.FixedWidth=false;
				style.InnerWidth=0;
			}else{
				style.FixedWidth=true;
				style.InnerWidth=value.PX;
			}
			
			// Fire percent pass:
			style.Element.SetWidthForKids(style);
			
			style.SetSize();
			
			style.RequestLayout();
		}
		
	}
	
}



