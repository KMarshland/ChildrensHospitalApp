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
using System.Collections;
using System.Collections.Generic;


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the font-family: css property.
	/// </summary>
	
	public class FontFamily:CssProperty{
		
		
		public FontFamily(){
			IsTextual=true;
			
			// This is a text property:
			Type=ValueType.Text;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-family"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Apply the property:
			
			string fontName=value.Text;
			
			if(fontName==null){
				return;
			}
			
			Find(fontName,text);
			
			if(text.FontToDraw==null){
				
				// No fonts at all.
				// We're going to load and use the default one:
				text.FontToDraw=DynamicFont.GetDefaultFamily();
				
			}
			
			// Apply the changes:
			text.SetText();
			
		}
		
		/// <summary>Finds and connects the font(s) for the given text renderer.</summary>
		private void Find(string fontName,TextRenderingProperty text){
			
			fontName=fontName.Replace("\"","");
			
			string[] pieces=fontName.Split(',');
			
			DynamicFont current=null;
			
			// Grab the doc:
			Document doc=text.Element.Document;
			
			for(int i=0;i<pieces.Length;i++){
				
				// Trim the name:
				fontName=pieces[i].Trim();
				
				// Get the font from this DOM:
				DynamicFont backup=doc.GetOrCreateFont(fontName);
				
				if(backup==null){
					// Font not described in the HTML at all or isn't available in the project.
					continue;
				}
				
				if(current!=null){
					
					// Hook up the fallback:
					current.Fallback=backup;
					
				}
				
				current=backup;
				
				if(text.FontToDraw==null){
					text.FontToDraw=current;
				}
				
			}
			
		}
		
	}
	
}



