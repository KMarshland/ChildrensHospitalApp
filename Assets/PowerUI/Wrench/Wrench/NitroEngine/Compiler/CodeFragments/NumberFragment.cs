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
using System.Reflection;
using System.Collections;
using System.Reflection.Emit;
using System.Collections.Generic;
using Wrench;

namespace Nitro{
	
	/// <summary>
	/// Represents a fixed number. (e.g. 14). 
	/// </summary>
	
	public class NumberFragment:CodeFragment{
		
		/// <summary>The set of characters that can form a number. Includes a dot at the start for floats.</summary>
		public static char[] Number=new char[]{'.','0','1','2','3','4','5','6','7','8','9'};
		
		/// <summary>Checks if a number fragment can be read from the stream by seeing if the current character is numeric.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if the character is numeric; false otherwise.</returns>
		public static bool WillHandle(char character){
			// NB: .9 is not a valid number; must always currently start with a numeric char.
			// This is because .something may be a property.
			return (IsOfType(Number,character)!=-1&&character!='.');
		}
		
		/// <summary>True if this number is a floating point value.</summary>
		public bool Float;
		/// <summary>The raw text of the number, e.g. "14.9".</summary>
		public string Value="";
		
		
		/// <summary>Reads a new number fragment from the given lexer.</summary>
		/// <param name="sr">The lexer to read the number from.</param>
		public NumberFragment(CodeLexer sr){
			int index=IsOfType(Number,sr.Peek());
			while(index!=-1){
				Value+=sr.Read();
				if(index==0){
					// Got a dot in it - must be a float.
					Float=true;
				}
				char peek=sr.Peek();
				if(peek=='f'){
					Float=true;
					sr.Read();
					return;
				}else if(peek=='H'&&sr.Peek(1)=='z'){
					Float=true;
					sr.Read();
					sr.Read();
					return;
				}else{
					index=IsOfType(Number,peek);
				}
			}
		}
		
		public override CompiledFragment Compile(CompiledMethod parent){
			if(Float){
				return new CompiledFragment(float.Parse(Value));
			}
			return new CompiledFragment(int.Parse(Value));
		}
		
		public override string ToString(){
			string result=Value;
			if(Float){
				result+="f";
			}
			return result;
		}

	}
	
}