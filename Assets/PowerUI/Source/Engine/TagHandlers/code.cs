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

using Wrench;


namespace PowerUI{
	
	/// <summary>
	/// Represents a code element.
	/// </summary>
	
	public class CodeTag:PreTag{
		
		public override string[] GetTags(){
			return new string[]{"code"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new CodeTag();
		}
		
		public override bool AtEnd(MLLexer lexer){
			// Three will do; we're certainly at the end by that point.
			return (lexer.Peek()=='<'&&
					lexer.Peek(1)=='/'&&
					lexer.Peek(2)=='c'&&
					lexer.Peek(3)=='o'&&
					lexer.Peek(4)=='d'
					);
		}
		
	}
	
}