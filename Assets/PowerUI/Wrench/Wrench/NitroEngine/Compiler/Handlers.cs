//--------------------------------------
//         Nitro Script Engine
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Wrench;

namespace Nitro{

	/// <summary>
	/// Used when parsing code into code fragments.
	/// This class figures out what type of fragment we have next in order for that type to 'handle' the text from the code.
	/// </summary>

	public static class Handlers{
	
		/// <summary>Finds which type of fragment will accept and handle the given character.</summary>
		/// <param name="peek">The character to find a handler for.</param>
		/// <returns>The handler which will deal with this character. May also be told to stop if no handler is available.</returns>
		public static Handler Find(char peek){	
			if(peek==StringReader.NULL||BracketFragment.IsEndBracket(peek)!=-1){
				return Handler.Stop;
			}else if(BracketFragment.WillHandle(peek)){
				return Handler.Brackets;
			}else if(StringFragment.WillHandle(peek)){
				return Handler.String;
			}else if(TypeFragment.WillHandle(peek)){
				return Handler.Type;
			}else if(OperatorFragment.WillHandle(peek)){
				return Handler.Operator;
			}else if(PropertyFragment.WillHandle(peek)){
				return Handler.Property;
			}else if(NumberFragment.WillHandle(peek)){
				return Handler.Number;
			}
			return Handler.Variable;
		}
		
		/// <summary>Reads a code fragment from the code using a known handler.</summary>
		/// <param name="handle">The handler to use.</param>
		/// <param name="sr">The raw code stream</param>
		/// <returns>A new code fragment, or null if told to stop with Handler.Stop.</returns>
		public static CodeFragment Handle(Handler handle,CodeLexer sr){
			if(handle==Handler.Brackets){
				return new BracketFragment(sr);
			}else if(handle==Handler.String){
				return new StringFragment(sr);
			}else if(handle==Handler.Type){
				return new TypeFragment(sr);
			}else if(handle==Handler.Operator){
				return new OperatorFragment(sr);
			}else if(handle==Handler.Number){
				return new NumberFragment(sr);
			}else if(handle==Handler.Variable){
				return new VariableFragment(sr);
			}else if(handle==Handler.Property){
				return new PropertyFragment(sr);
			}else{
				return null;
			}
		}
		
	}
	
}