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
	/// Represents the font-spreadover: css property. Specific to PowerUI.
	/// This fedines a maximum font size beyond which PowerUI will start 'spreading'
	/// characters onto other fonts such as specific bold or italic fonts, if they exist.
	/// </summary>
	
	public class FontSpreadover:CssProperty{
		
		
		public FontSpreadover(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-spreadover"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				text.Spreadover=30;
			}else{
				text.Spreadover=value.PX;
			}
			
			// Apply the changes:
			text.SetText();
		}
		
	}
	
}



