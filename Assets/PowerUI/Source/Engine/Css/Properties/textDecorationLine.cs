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
	/// Represents the text-decoration-line: css property.
	/// </summary>
	
	public class TextDecorationLine:CssProperty{
		
		
		public TextDecorationLine(){
			IsTextual=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration-line"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				return;
			}
			
			// Get the decoration:
			TextDecorationInfo decoration=text.TextLine;
			
			// Apply the property:
			if(value==null){
				
				if(decoration==null){
					return;
				}
				
				text.TextLine=null;
				
			}else{
				
				TextLineType type;
				
				switch(value.Text){
				
					case "underline":
						type=TextLineType.Underline;
					break;
					case "overline":
						type=TextLineType.Overline;
					break;
					case "line-through":
						type=TextLineType.StrikeThrough;
					break;
					default:
						type=TextLineType.None;
						
						if(decoration==null){
							return;
						}
						
						text.TextLine=null;
						
					break;
					
				}
				
				if(type!=TextLineType.None){
					
					// We need a line.
					
					if(decoration==null){
						decoration=new TextDecorationInfo(type);
						text.TextLine=decoration;
						
						// Need colour applying?
						value=style["text-decoration-color"];
						
						if(value!=null && value.Text!="initial"){
							// Apply colour:
							decoration.SetColour(value.ToColor());
						}
						
					}else{
						decoration.Type=type;
					}
					
				}
				
			}
			
			// Apply the changes - doesn't change anything about the text:
			text.RequestLayout();
		}
		
	}
	
}



