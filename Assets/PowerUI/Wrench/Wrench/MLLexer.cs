//--------------------------------------
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace Wrench{

	/// <summary>
	/// A Markup Language (ML) lexer reads xml-like markup languages from a string.
	/// It's used to help form a DOM like structure from any markup string such as
	/// html, sml, xml etc.
	/// </summary>

	public class MLLexer:StringReader{
		
		/// <summary>Set this to true if the lexer should be in literal mode. This prevents it stripping any junk such as tabs.</summary>
		public bool Literal;
		/// <summary>The current line number.</summary>
		public int LineNumber=1;
		/// <summary>True if the lexer read some junk such as tabs from the string.</summary>
		public bool DidReadJunk;
		
		
		/// <summary>Starts a new lexer, optionally placing it immediately in literal mode.</summary>
		public MLLexer(string str,bool literal):base(str){
			
			Literal=literal;
			
			if(!Literal){
				
				// Skip any initial junk:
				while(ReadJunk()){}
				
				if(Text.Whitespace==WhitespaceMode.Normal){
					// Skip initial whitelines:
					SkipSpaces();
				}
				
			}
			
		}
		
		///<summary>Creates a new lexer for the given xml-like string.</summary>
		public MLLexer(string str):base(str){
			// Skip any initial junk:
			while(ReadJunk()){}
			
			if(Text.Whitespace==WhitespaceMode.Normal){
				// Skip initial whitelines:
				SkipSpaces();
			}
		}
		
		/// <summary>Reads a character from the string.</summary>
		/// <returns>The character that was read. <see cref="Wrench.StringReader.NULL"/> is returned if the end of the input is reached.</returns>
		public override char Read(){
			char read=base.Read();
			DidReadJunk=false;
			
			if(read=='\n'){
				LineNumber++;
			}
			
			if(!Literal){
				
				if(Text.Whitespace==WhitespaceMode.Normal){
					if(read==' '){
						// Condense spaces by skipping any others:
						SkipSpaces();
					}
				}
				
				while(ReadJunk()){
					DidReadJunk=true;
				}
			}
			
			return read;
		}
		
		public override int GetLineNumber(){
			return LineNumber;
		}
		
		/// <summary>Call this to exit literal mode.</summary>
		public void ExitLiteral(){
			Literal=false;
			
			// Read junk now:
			DidReadJunk=false;
			if(!Literal){
				while(ReadJunk()){
					DidReadJunk=true;
				}
			}
		}
		
		/// <summary>Checks if there is any junk such as tabs next in the lexer.</summary>
		/// <returns>True if any junk was next. Note that it has already been read off.</returns>
		public bool PeekJunk(){
			return DidReadJunk;
		}
		
		/// <summary>Reads junk such as tabs from the lexer.</summary>
		/// <returns>True if any junk was read.</returns>
		public bool ReadJunk(){
			char c=Peek();
			
			if(c==StringReader.NULL){
				// End of string
				return false;
			}
			
			if(c=='<'){
				if(Peek(1)=='!' && Peek(2)=='-' && Peek(3)=='-'){
					// Comment
					Advance(4);
					
					c=Peek();
					
					while(c!=StringReader.NULL){
						if(c=='-' && Peek(1)=='-' && Peek(2)=='>'){
							Advance(3);
							break;
						}
						
						Advance();
						c=Peek();
					}
					
					return (c!=StringReader.NULL);
				}
				
			}else if(c=='\t'){
				Advance();
				
				return true;
			}else if(c=='\n'){
				Advance();
				
				if(Text.Whitespace==WhitespaceMode.Normal){
					// Skip spaces on the newline.
					SkipSpaces();
				}
				
				LineNumber++;
				
				return true;
			}else if(c=='\r'){
				Advance();
				
				if(Text.Whitespace==WhitespaceMode.Normal){
					// Skip spaces on the newline.
					SkipSpaces();
				}
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Skips any whitespaces that occur next.</summary>
		public void SkipSpaces(){
			char peek=Peek();
			
			while(peek==' '){
				Read();
				peek=Peek();
			}
		}
		
	}
	
}