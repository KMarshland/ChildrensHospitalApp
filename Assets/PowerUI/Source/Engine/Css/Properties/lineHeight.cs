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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the line-height: css property.
	/// </summary>
	
	public class LineHeight:CssProperty{
		
		public LineHeight(){
			IsTextual=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"line-height"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			if(value==null || value.Text=="normal"){
				text.LineGap=0.2f;
			}else{
				text.LineGap=value.Single-1f;
			}
			
			// Apply the changes:
			text.SetDimensions();
			
		}
		
	}
	
}



