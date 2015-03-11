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
	/// Represents a usage of an operator (e.g. '+').
	/// </summary>
	
	public class OperatorFragment:CodeFragment{
		
		/// <summary>Checks if an operator can be read from the stream by checking if the given character is the start of an operator.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if the character is the start of an operator.</returns>
		public static bool WillHandle(char character){
			return Operator.Starts.ContainsKey(character);
		}
		
		
		/// <summary>The operator being used.</summary>
		public Operator Value;
		
		
		/// <summary>Creates a new operator from the given raw text, e.g. "+".</summary>
		/// <param name="operatorText">The raw text operator.</param>
		public OperatorFragment(string operatorText){
			Set(operatorText);
		}
		
		/// <summary>Creates a new operator by reading one from given the lexer.</summary>
		/// <param name="sr">The lexer to read the operator from.</param>
		public OperatorFragment(CodeLexer sr){
			char operatorChar=sr.Read();
			// First check if this character plus the next one makes a valid operator (e.g. +=):
			char peek=sr.Peek();
			if(peek!=StringReader.NULL&&Set(""+operatorChar+peek)){
				// Yes it does - Make it official by reading it off:
				sr.Read();
				// We called set in the if so don't do it again:
				return;
			}
			
			Set(""+operatorChar);
		}
		
		/// <summary>Is this the set operator?</summary>
		public bool IsSetOperator{
			get{
				return (Value!=null && Value.GetType()==typeof(OperatorSet));
			}
		}
		
		/// <summary>Sets the current operator of this fragment from the operator as a string (e.g. "+").</summary>
		/// <param name="value">The operator as a string.</param>
		/// <returns>True if the string is an operator and it was set successfully; False otherwise.</returns>
		public bool Set(string value){
			return Operator.FullOperators.TryGetValue(""+value,out Value);
		}
		
		public override string ToString(){
			return Value.Pattern;
		}
		
	}
	
}