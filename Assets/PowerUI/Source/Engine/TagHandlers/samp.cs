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
	/// Represents a samp(le) element.
	/// </summary>
	
	public class SampTag:PreTag{
		
		public override string[] GetTags(){
			return new string[]{"samp"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SampTag();
		}
		
		protected override bool AtEnd(MLLexer lexer){
			// Three will do; we're certainly at the end by that point.
			return (lexer.Peek()=='<'&&
					lexer.Peek(1)=='/'&&
					lexer.Peek(2)=='s'&&
					lexer.Peek(3)=='a'&&
					lexer.Peek(4)=='m'
					);
		}
	}
	
}