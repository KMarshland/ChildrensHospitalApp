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
			if(value==null || value.Text=="normal"){
				text.Weight=400;
			}else if(value.Text=="bold"){
				text.Weight=700;
			}else if(value.PX!=0){
				text.Weight=value.PX;
			}else{
				text.Weight=400;
			}
			
			// Apply the changes:
			text.SetText();
		}
		
	}
	
}



