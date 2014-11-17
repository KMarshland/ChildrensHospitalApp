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
	/// Represents the font-weight: css property.
	/// </summary>
	
	public class FontWeight:CssProperty{
		
		
		public FontWeight(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-weight"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				text.Bold=false;
			}else{
				text.Bold=(value.Text=="bold");
			}
			
			// Apply the changes:
			text.SetText();
		}
		
	}
	
}



