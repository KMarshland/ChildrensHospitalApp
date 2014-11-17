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
	/// Represents a new array construct.
	/// </summary>
	
	public class ArrayFragment:CodeFragment{
	
		/// <summary>The type of the array, e.g. string[].</summary>
		public TypeFragment ArrayType;
		/// <summary>The set of initial values for the array. E.g. new string[]{"Value1","Value2"};</summary>
		public BracketFragment Defaults;
		
		
		public ArrayFragment(TypeFragment arrayType,BracketFragment defaults){
			ArrayType=arrayType;
			Defaults=defaults;
		}
		
		public override CompiledFragment Compile(CompiledMethod parent){
			return new ArrayOperation(parent,ArrayType.FindType(parent.Script),null,CompilationServices.CompileParameters(Defaults,parent));
		}
		
		public override string ToString(){
			string result="new "+ArrayType.ToString();
			if(Defaults!=null){
				result+=Defaults.ToString();
			}
			return result;
		}
		
	}
	
}