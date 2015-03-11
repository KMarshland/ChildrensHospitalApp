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
	/// Represents the font-smoothing: css property.
	/// </summary>
	
	public class FontSmoothing:CssProperty{
		
		
		public FontSmoothing(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-smoothing"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			bool auto=(value==null || value.Text=="auto" || value.Text=="anti-alias");
			
			if(auto){
				text.Alias=float.MaxValue;
			}else if(value.PX!=0){
				text.Alias=(float)value.PX;
			}else{
				text.Alias=value.Single;
			}
			
			// Set width/height directly to the computed style:
			text.SetDimensions();
			
			// Request a redraw:
			style.RequestLayout();
			
		}
		
	}
	
}



