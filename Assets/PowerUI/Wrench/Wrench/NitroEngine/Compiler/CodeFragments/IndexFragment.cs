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
using System.Collections;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents something being indexed, e.g. variable[index].
	/// </summary>
	
	public class IndexFragment:CodeFragment{
	
		/// <summary>The brackets which contain the indices.</summary>
		public CodeFragment Brackets;
		/// <summary>The variable being indexed. In most cases, this will be an array of some kind.</summary>
		public CodeFragment Variable;
		
		
		/// <summary>Creates a new index fragment.</summary>
		/// <param name="brackets">The brackets which contain the indices.</param>
		/// <param name="variable">The variable being indexed. This will most commonly be an array.</param>
		public IndexFragment(CodeFragment brackets,CodeFragment variable){
			Brackets=brackets;
			Variable=variable;	
		}
		
		public override CompiledFragment Compile(CompiledMethod parentBlock){
			return new IndexOperation(parentBlock,Variable.Compile(parentBlock),CompilationServices.CompileParameters(Brackets,parentBlock));
		}
		
		public override string ToString(){
			string result=Variable.ToString();
			if(Brackets!=null){
				result+=Brackets.ToString();
			}
			return result;
		}
		
	}
	
}