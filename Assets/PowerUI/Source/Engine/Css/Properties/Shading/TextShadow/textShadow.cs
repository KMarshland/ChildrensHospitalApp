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
using PowerUI.Css;


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the text-shadow: css property.
	/// </summary>
	
	public class TextShadow:CssProperty{
		
		
		public TextShadow(){
			IsTextual=true;
			Type=ValueType.Rectangle;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-shadow"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Got text at all?:
			if(GetText(style)==null){
				return;
			}
			
			// Apply the property:
			if(value==null || value.Text=="none"){
				
				// Clear the shadow:
				style.TextShadow=null;
				
			}else{
			
				// The glow properties:
				int blur=0;
				Color colour=Color.black;
				
				ShadowData data=style.TextShadow;
				
				if(data==null){
					data=new ShadowData();
					style.TextShadow=data;
				}
				
				data.HOffset=value[0].PX;
				data.VOffset=value[1].PX;
				
				// Grab the blur:
				Value innerValue=value[2];
				
				if(innerValue.Type==ValueType.Color){
					colour=innerValue.ToColor();
				}else{
					blur=innerValue.PX;
					
					// Grab the colour:
					innerValue=value[3];
					
					if(innerValue.Type==ValueType.Color){
						colour=innerValue.ToColor();
					}
					
				}
			
				if(colour.a==1f){
					// Default transparency:
					colour.a=0.8f;
				}
				
				data.Colour=colour;
				data.Blur=blur;
				
			}
			
			// Apply the changes - doesn't change anything about the actual text, so we just want a layout:
			style.RequestLayout();
			
		}
		
	}
	
}

namespace PowerUI.Css{
	
	public partial class ShaderData{
		
		/// <summary>The shadow for the text, if there is one.</summary>
		public ShadowData TextShadow;
		
	}
	
	public partial class ComputedStyle{
		
		/// <summary>The text shadow for this computed style.</summary>
		public ShadowData TextShadow{
			get{
				if(Shading==null){
					return null;
				}
				
				return Shading.TextShadow;
			}
			set{
				
				// Got any shading data?
				if(Shading==null){
					
					if(value==null){
						return;
					}
					
					// Create shading and apply:
					RequireShading().TextShadow=value;
					
					// Set the flag:
					Shading.SetFlag(4,true);
					
				}else{
					
					Shading.TextShadow=value;
					
					// Set the flag:
					Shading.SetFlag(4,(value!=null));
					
					if(value==null){
						
						// Optimise it:
						Shading.Optimise();
						
					}
					
				}
				
			}
		}
		
	}
	
}



