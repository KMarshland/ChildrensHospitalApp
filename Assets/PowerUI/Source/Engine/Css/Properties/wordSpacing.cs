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
	/// Represents the word-spacing: css property.
	/// </summary>
	
	public class WordSpacing:CssProperty{
		
		
		public WordSpacing(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"word-spacing"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Get the standard space size for this text property:
			float standardSize=text.FontSize/3f;
			
			// Apply the property:
			if(value==null){
				
				// Standard space size:
				text.SpaceSize=standardSize;
			
			}else if(value.Type==ValueType.Em){
			
				// Em value:
				text.SpaceSize=(float)text.FontSize*value.Single;
			
			}else if(value.Type==ValueType.Pixels){
				
				// Straight pixel size:
				text.SpaceSize=(float)value.PX;
				
			}else if(value.Single!=0f){
				
				// Relative, e.g. 200%
				text.SpaceSize=standardSize*value.Single;
				
			}else{
				
				// Standard space size:
				text.SpaceSize=standardSize;
				
			}
			
			// Apply:
			text.RequestLayout();
			
			// Prompt the renderer to recalculate the width of the words:
			text.NoTextChange=false;
		}
		
	}
	
}



