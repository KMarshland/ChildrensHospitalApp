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
	/// Represents the font-size: css property.
	/// </summary>
	
	public class FontSize:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontSize GlobalProperty;
		
		
		public FontSize(){
			IsTextual=true;
			GlobalProperty=this;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-size"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				text.FontSize=12;
			}else{
				
				if(value.Type==ValueType.Text){
					
					if(string.IsNullOrEmpty(value.Text)){
						text.FontSize=12;
					}else{
						text.FontSize=int.Parse(value.Text);
					}
					
				}else{
					text.FontSize=value.PX;
				}
				
			}
			
			// Set width/height directly to the computed style:
			text.SetDimensions();
			
			// Apply the changes:
			text.SetText();
		}
		
	}
	
}



