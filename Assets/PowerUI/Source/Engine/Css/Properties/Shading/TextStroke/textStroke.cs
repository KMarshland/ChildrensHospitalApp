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
	/// Represents the text-stroke: css property. Used to add outlines to text.
	/// </summary>
	
	public class TextStroke:CssProperty{
		
		
		public TextStroke(){
			IsTextual=true;
			Type=ValueType.Rectangle;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-stroke","text-outline"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Got any text at all?:
			if(GetText(style)==null){
				return;
			}
			
			// Apply the property:
			if(value==null || value.Text=="none"){
				
				// Clear the stroke:
				style.TextStroke=null;
				
			}else{
			
				// The stroke properties:
				int blur=0;
				Color colour=Color.black;
				
				int thickness=value[0].PX;
				
				if(thickness==0){
					
					style.TextStroke=null;
					
				}else{
					
					StrokeData data=style.TextStroke;
					
					if(data==null){
						data=new StrokeData();
						style.TextStroke=data;
					}
					
					data.Thickness=thickness;
				
					// Grab the blur:
					Value innerValue=value[1];
					
					if(innerValue.Type==ValueType.Color){
						colour=innerValue.ToColor();
					}else{
						blur=innerValue.PX;
						
						// Grab the colour:
						innerValue=value[2];
						
						if(innerValue.Type==ValueType.Color){
							colour=innerValue.ToColor();
						}
						
					}
					
					data.Colour=colour;
					data.Blur=blur;
				
				}
				
			}
			
			// Apply the changes - doesn't change anything about the actual text, so we just want a layout:
			style.RequestLayout();
		}
		
	}
	
}

namespace PowerUI.Css{
	
	public partial class ShaderData{
		
		/// <summary>The stroke for the text, if there is one.</summary>
		public StrokeData TextStroke;
		
	}
	
	public partial class ComputedStyle{
		
		/// <summary>The text shadow for this computed style.</summary>
		public StrokeData TextStroke{
			get{
				if(Shading==null){
					return null;
				}
				
				return Shading.TextStroke;
			}
			set{
				
				// Got any shading data?
				if(Shading==null){
					
					if(value==null){
						return;
					}
					
					// Create shading and apply:
					RequireShading().TextStroke=value;
					
					// Set the flag:
					Shading.SetFlag(2,true);
					
				}else{
					
					Shading.TextStroke=value;
					
					// Set the flag:
					Shading.SetFlag(2,(value!=null));
					
					if(value==null){
						
						// Optimise it:
						Shading.Optimise();
						
					}
					
				}
				
			}
		}
		
	}
	
}



