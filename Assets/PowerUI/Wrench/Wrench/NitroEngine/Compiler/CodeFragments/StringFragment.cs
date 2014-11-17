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
	/// Represents a fixed text string in the code. (e.g. "hello").
	/// </summary>
	
	public class StringFragment:CodeFragment{
	
		/// <summary>The character used to delimit any quotes found within a string.</summary>
		public static char Delimiter='\\';
		/// <summary>A set of quotes (", ') that can mark the start and end of a string.</summary>
		public static char[] Quotes=new char[]{'"','\''};
		
		
		/// <summary>Checks if a string can be read from a stream by seeing if the current character is a quote.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if the character is a quote; false otherwise.</returns>
		public static bool WillHandle(char character){
			return (IsQuote(character)!=-1);
		}
		
		/// <summary>Checks if the given character is a quote (",').</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>The index of the quote if it is one. " is 0, ' is 1. -1 otherwise.</returns>
		public static int IsQuote(char character){
			return IsOfType(Quotes,character);
		}
		
		/// <summary>The literal string text. It does not contain the quotes and has reversed any delimiters within it.</summary>
		public string Value="";
		
		
		/// <summary>Creates a new string fragment by reading it from the given lexer.</summary>
		/// <param name="sr">The lexer to read it from.</param>
		public StringFragment(CodeLexer sr){
			sr.Literal=true;
			// Read off the quote:
			char Quote=sr.Read();
			char Char=sr.Read();
			bool Delimited=false;
			
			while( Delimited || Char!=Quote && Char!=StringReader.NULL){
				
				if(Char=='\\'&&!Delimited){
					Delimited=true;
				}else{
					Delimited=false;
					Value+=Char;
				}
				
				Char=sr.Read();
			}
			
			sr.Literal=false;
			
			// Read off any junk after the quote:
			while(sr.ReadJunk()){
				sr.DidReadJunk=true;
			}
			
			if(Char==StringReader.NULL){
				Error("Unterminated string found.");
			}
		}
		
		public StringFragment(string value){
			Value=value;
		}
		
		public override CompiledFragment Compile(CompiledMethod parent){
			return new CompiledFragment(Value);
		}
		
		public override string ToString(){
			return "\""+System.Text.RegularExpressions.Regex.Replace(Value,@"[\\'""]", @"\$0")+"\"";
		}
		
	}
	
}