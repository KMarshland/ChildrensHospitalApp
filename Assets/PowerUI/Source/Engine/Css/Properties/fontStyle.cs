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
	/// Represents the font-style: css property.
	/// </summary>
	
	public class FontStyle:CssProperty{
		
		
		public FontStyle(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-style"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				text.Italic=false;
			}else{
				text.Italic=(value.Text=="italic" || value.Text=="oblique");
			}
			
			// Apply the changes:
			text.SetText();
		}
		
	}
	
}



