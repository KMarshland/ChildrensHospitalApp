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
	/// Represents a property of an object (e.g. something.property).
	/// </summary>
	
	public class PropertyFragment:CodeFragment{
		
		/// <summary>Checks if a property fragment can be read from the stream.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if the character is a ".".</returns>
		public static bool WillHandle(char character){
			// Nb: .9 is not a valid number; must always currently start with a numeric char.
			// This is because .something may be a property.
			return (character=='.');
		}
		
		/// <summary>The property being accessed.</summary>
		public string Value;
		/// <summary>The fragment that this is a property of.</summary>
		public CodeFragment of;
		
		
		/// <summary>Creates a property fragment by reading it from the given lexer.</summary>
		/// <param name="sr">The lexer to read the fragment from.</param>
		public PropertyFragment(CodeLexer sr){
			// Skip the dot:
			sr.Read();
			// Read a 'variable':
			Value+=char.ToLower(sr.Read());
			while(true){
				char peek=sr.Peek();	
				if(peek==';'||peek==','||peek=='.'||peek==StringReader.NULL||BracketFragment.AnyBracket(peek)){
					// Pass control back to the operation:
					break;
				}
				Handler handle=Handlers.Find(peek);
				if(handle!=Handler.Stop&&handle!=Handler.Variable&&handle!=Handler.Number&&handle!=Handler.Property){
					break;
				}
				Value+=char.ToLower(sr.Read());
			}
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			if(to.LastChild==null){
				Error("A property (."+Value+") was found in an unexpected place.");
			}
			// Replace the last child with *THIS*:
			of=to.LastChild;
			of.Remove();
			to.AddChild(this);
			return AddResult.Ok;
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			return new PropertyOperation(method,of.Compile(method),Value);
		}
		
		public override string ToString(){
			return of.ToString()+"."+Value;
		}
		
	}
	
}