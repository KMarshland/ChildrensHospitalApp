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
	/// Represents the translate: css property.
	/// </summary>
	
	public class Translate:CssProperty{
		
		
		public Translate(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"translate"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(style.SetupTransform(value)){
			
				if(value==null){
					style.Transform.Translate=Vector3.zero;
				}else{
					style.Transform.Translate=new Vector3(value.GetFloat(0),value.GetFloat(1),value.GetFloat(2));
				}
				
			}
			
			style.RequestTransform();
		}
		
	}
	
}



