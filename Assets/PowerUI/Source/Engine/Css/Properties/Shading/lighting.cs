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
using System.Collections;
using System.Collections.Generic;
using PowerUI.Css;


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the lighting: css property.
	/// It's either on or off.
	/// </summary>
	
	public class Lighting:CssProperty{
		
		
		public Lighting(){
			
			// This is a bool property:
			Type=ValueType.Boolean;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"lighting"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Is it lit?
			bool lit=(value!=null && value.Boolean);
			
			if(!lit && style.Shading==null){
				// Essentially ignore it anyway.
				return;
			}
			
			if(style.Shading==null){
				// It's lit:
				style.RequireShading().Lit=true;
			}else{
				// It's not lit:
				style.Shading.Lit=false;
				
				// Optimise - might no longer need the shading info:
				style.Shading.Optimise();
			}
			
			// Request a layout now:
			style.RequestLayout();
			
		}
		
	}
	
}
