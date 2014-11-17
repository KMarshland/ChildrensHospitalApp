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
	/// Represents the text-decoration: css property.
	/// </summary>
	
	public class TextDecoration:CssProperty{
		
		
		public TextDecoration(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				text.TextLine=TextLineType.None;
			}else{
				
				switch(value.Text){
				
					case "underline":
						text.TextLine=TextLineType.Underline;
					break;
					case "overline":
						text.TextLine=TextLineType.Overline;
					break;
					case "line-through":
						text.TextLine=TextLineType.StrikeThrough;
					break;
					default:
						text.TextLine=TextLineType.None;
					break;
					
				}
				
			}
			
			// Apply the changes:
			text.SetText();
		}
		
	}
	
}



