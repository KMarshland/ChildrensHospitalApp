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
			
			fontName=fontName.Replace("\"","");
			
			text.FontToDraw=text.Element.Document.Renderer.GetOrCreateFont(fontName);
			
			if(text.FontToDraw==null){
				
				// It's not a font at all.
				// So, we're gonna next pick the 'first' (random) font in the renderer set and use that.
				// And for future reffing, we'll then map the fontName to that too.
				
				Dictionary<string,DynamicFont> fonts=text.Element.Document.Renderer.ActiveFonts;
				
				if(fonts.Count>0){
					IEnumerator<string> enumerator=fonts.Keys.GetEnumerator();
					enumerator.MoveNext();
					
					text.FontToDraw=fonts[enumerator.Current];
					
					// Push for quick reference later:
					fonts[fontName]=text.FontToDraw;
				}else{
					// None loaded! Track up the tree in search of a font that works.
					SearchForFont(text,text.Element.ParentNode);
				}
				
			}
			
			// Recalculate the sizes of the text:
			text.SetDimensions();
			
			// Apply the changes:
			text.SetText();
		}
		
		/// <summary>Searches for a suitable font in the given elements style. 
		/// This occurs if the font name for the current element is invalid.</summary>
		/// <param name="element">The element to search. Checks its parent if this is also invalid.</param>
		public void SearchForFont(TextRenderingProperty text,Element element){
			if(element==null){
				return;
			}
			
			// Get the current value for the font-family property:
			Value font=element.Style.Computed[this];
			
			if(font!=null){
				
				string fontName=font.Text.Replace("\"","");
				
				text.FontToDraw=text.Element.Document.Renderer.GetOrCreateFont(fontName);
				
				if(text.FontToDraw!=null){
					return;
				}
				
			}
			
			SearchForFont(text,element.ParentNode);
		}
		
	}
	
}



