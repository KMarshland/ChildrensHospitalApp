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
using PowerUI.Css;
using Wrench;
using System.Text;


namespace PowerUI{

	/// <summary>
	/// Handles the style tag containing inline css.
	/// Use the link tag if you wish to add external css.
	/// </summary>

	public class StyleTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"css","style"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new StyleTag();
		}
		
		public override void OnParseContent(MLLexer lexer){
			lexer.Literal=true;
			// Keep reading until we hit </style>.
			StringBuilder styleText=new StringBuilder();
			
			while(!AtEnd(lexer)){
				styleText.Append(lexer.Read());
			}
			
			// Great, good stuff. Add to the document's style:
			if(styleText.Length!=0){
				Element.Document.AddStyle(styleText.ToString());
			}
			
			lexer.Literal=false;
		}
		
		/// <summary>Checks if the given lexer has reached the end of the inline style content.</summary>
		/// <param name="lexer">The lexer to check if it's reached the &lt;/style&gt; tag.</param>
		/// <returns>True if the lexer has reached the end style tag; false otherwise.</returns>
		private bool AtEnd(MLLexer lexer){
			// Up to y will do; we're certainly at the end by that point.
			return (lexer.Peek()=='<'&&
					lexer.Peek(1)=='/'&&
					lexer.Peek(2)=='s'&&
					lexer.Peek(3)=='t'&&
					lexer.Peek(4)=='y'
					);
		}
		
	}
	
}