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
	/// Represents a constructor call.
	/// </summary>
	
	public class ConstructorFragment:CodeFragment{
	
		/// <summary>The type of the object to construct.</summary>
		public TypeFragment NewType;
		/// <summary>The arguments for the constructor as a bracket fragment.</summary>
		public BracketFragment Brackets;
		
		
		
		/// <summary>Creates a new Constructor Fragment.</summary>
		/// <param name="type">The type of the object to construct.</param>
		/// <param name="brackets">The arguments for the constructor as a bracket fragment.</param>
		public ConstructorFragment(TypeFragment type,BracketFragment brackets){
			NewType=type;
			Brackets=brackets;
			
			// Add them as children such that code tree iterators can visit them:
			AddChild(Brackets);
			
		}
		
		public override CompiledFragment Compile(CompiledMethod parent){
			return new ConstructOperation(NewType,Brackets,parent);
		}
		
		public override string ToString(){
			string result="new "+NewType.ToString();
			
			if(Brackets!=null){
				result+=Brackets.ToString();
			}
			
			return result;
		}
		
	}
	
}