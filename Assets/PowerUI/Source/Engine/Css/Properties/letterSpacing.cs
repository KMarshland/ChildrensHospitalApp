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
	/// Represents the letter-spacing: css property.
	/// </summary>
	
	public class LetterSpacing:CssProperty{
		
		
		public LetterSpacing(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"letter-spacing"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
			
				// No spacing:
				text.LetterSpacing=0;
			
			}else if(value.Type==ValueType.Pixels){
				
				// Fixed space:
				text.LetterSpacing=value.PX;
				
			}else if(value.Single!=0f){
				
				// Apply a relative %:
				text.LetterSpacing=text.FontSize*(value.Single-1f);
				
			}else{
				
				// Default no spacing:
				text.LetterSpacing=0;
				
			}
			
			// Apply:
			text.RequestLayout();
			
			// Prompt the renderer to recalculate the width of the word:
			text.NoTextChange=false;
		}
		
	}
	
}



