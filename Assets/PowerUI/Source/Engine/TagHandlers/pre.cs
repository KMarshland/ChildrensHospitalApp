//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using Wrench;
using System.Text;


namespace PowerUI{
	
	/// <summary>
	/// Represents a pre element.
	/// </summary>
	
	public class PreTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"pre"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new PreTag();
		}
		
		public override void OnParseContent(MLLexer lexer){
			lexer.Literal=true;
			// Keep reading until we hit </pre>.
			StringBuilder text=new StringBuilder();
			
			while(!AtEnd(lexer)){
				char read=lexer.Read();
				
				// Convert any newlines into <br>eaks.
				if(read=='\r'){
					if(lexer.Peek()=='\n'){
						// It will handle the \n instead.
						continue;
					}
					
					text.Append("<br>");
				}else if(read=='<'){
					text.Append("&gt;");
				}else if(read=='&'){
					text.Append("&amp;");
				}else if(read=='\n'){
					text.Append("<br>");
				}else{
					text.Append(read);
				}
				
			}
			
			// Great, good stuff! Apply to its text content (literally):
			if(text.Length!=0){
				Element.innerHTML=text.ToString();
			}
			
			lexer.Literal=false;
		}
		
		/// <summary>Checks if the given lexer has reached the end of the inline pre content.</summary>
		/// <param name="lexer">The lexer to check if it's reached the &lt;/pre&gt; tag.</param>
		/// <returns>True if the lexer has reached the end pre tag; false otherwise.</returns>
		protected virtual bool AtEnd(MLLexer lexer){
			// Three will do; we're certainly at the end by that point.
			return (lexer.Peek()=='<'&&
					lexer.Peek(1)=='/'&&
					lexer.Peek(2)=='p'&&
					lexer.Peek(3)=='r'&&
					lexer.Peek(4)=='e'
					);
		}
		
		
	}
	
}