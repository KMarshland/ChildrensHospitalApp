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
	/// Represents the shader-family: css property.
	/// This can be used to assign custom shaders.
	/// Note that it's a "family" because there is a group of shaders which can potentially be used depending on other CSS settings (e.g. lit and unlit).
	/// Each shader must be named e.g:
	///
	/// FamilyName Unlit
	/// - The main shader most commonly used. Required.
	///
	/// FamilyName Isolated
	/// - The fallback shader when no others are suitable. Required.
	///
	/// FamilyName Lit
	/// - Optional. Lit variant.
	/// 
	/// Note that you should also put shaders in Resources so Unity doesn't accidentally remove them from your project.
	/// </summary>
	
	public class ShaderFamily:CssProperty{
		
		
		public ShaderFamily(){
			
			// This is a text property:
			Type=ValueType.Text;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"shader-family"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			ShaderSet family=null;
			
			// Apply:
			if(value!=null){
				
				// Lowercase so we can have the best chance at spotting the standard set (which is optimised for).
				string familyLC=value.Text.ToLower();
				
				if(familyLC!="standardui" && familyLC!="standard" && familyLC!="" && familyLC!="none"){
					
					// Get the family:
					family=ShaderSet.Get(value.Text);
					
				}
				
			}
			
			// Apply it here:
			if(style.Shading!=null){
				
				// Update it:
				style.Shading.Shaders=family;
				
				if(family==null){
					// Check if the shading data is no longer in use:
					style.Shading.Optimise();
				}
				
			}else if(family!=null){
				
				style.RequireShading().Shaders=family;
				
			}
			
			// Request a layout now:
			style.RequestLayout();
			
		}
		
	}
	
}