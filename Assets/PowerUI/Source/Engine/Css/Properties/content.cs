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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the content: css property.
	/// Note that this one is *not* textual - this is actually correct.
	/// It doesn't apply to text - rather it 'is' text.
	/// </summary>
	
	public class Content:CssProperty{
		
		
		public Content(){
			// Note that it is not IsTextual - this is correct!
			
			// This is a text property:
			Type=ValueType.Text;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"inner-text","content"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text property:
			TextRenderingProperty text=style.Text;
			
			if(text==null){
				// Create one - note that content is the only property that can create a TRP.
				style.Text=text=new TextRenderingProperty(style.Element);
				
				// Next, apply every textual style immediately.
				// Note: properties with multiple names are applied multiple times.
				// This is fine. It's expected to be such a rare case and it will work fine anyway.
				foreach(KeyValuePair<string,CssProperty> kvp in CssProperties.TextProperties){
					SetValue(kvp.Value,style);
				}
				
			}
			
			// Set the actual text:
			if(value==null){
				text.Text="";
			}else{
				text.Text=value.Text;
			}
				
			// And apply it:
			text.SetText();
			
			// Flag it as having changed:
			text.Changed=true;
			
		}
		
		/// <summary>Sets the named css property from the given style if the property exists in the style.</summary>
		/// <param name="property">The css property, e.g. color.</param>
		/// <param name="style">The style to load value of the property from. This should be the computed style for the parent element.</param>
		private void SetValue(CssProperty property,ComputedStyle style){
			// Get the current value:
			Value value=style[property];
			
			if(value!=null){
				// Apply it:
				property.Apply(style,value);
			}
		}
		
	}
	
}



