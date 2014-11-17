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
	/// Represents the text-clip: css property. Specific to PowerUI. This defines how characters are clipped when text is near an edge.
	/// By default this is set to "fast" which causes a minor squishing effect. It gets noticeable though with large text.
	/// In which case, set this to "clip".
	/// </summary>
	
	public class TextClip:CssProperty{
		
		
		public TextClip(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-clip"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			if(value==null){
				
				text.TextClipping=TextClipType.Fast;
				
			}else{
				string valueText=value.Text;
				
				if(valueText=="clip" || valueText=="on"){
					text.TextClipping=TextClipType.Clip;
				}else{
					text.TextClipping=TextClipType.Fast;
				}
				
			}
			
			// Apply:
			text.RequestLayout();
		}
		
	}
	
}



