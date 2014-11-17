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
	/// Represents a single operation.
	/// </summary>
	
	public class OperationFragment:CodeFragment{
		
		/// <summary>The line that this operation was found on.</summary>
		public int LineNumber;
		
		/// <summary>Creates a new empty operation fragment.</summary>
		public OperationFragment(){}
		
		
		/// <summary>Reads a new operation from the given lexer.</summary>
		/// <param name="sr">The lexer to read the operation from.</param>
		/// <param name="parent">The fragment to parent the operation to.</param>
		public OperationFragment(CodeLexer sr,CodeFragment parent){
			ParentFragment=parent;
			LineNumber=sr.LineNumber;
			while(true){
				char peek=sr.Peek();
				if(peek==StringReader.NULL){
					return;
				}
				if(peek==';'||peek==','){
					// Read it off:
					sr.Read();
					return;
				}
				Handler handler=Handlers.Find(peek);
				if(handler==Handler.Stop){
					return;
				}
				if(Handlers.Handle(handler,sr).AddTo(this,sr)==AddResult.Stop){
					return;
				}
			}
		}
		
		public override int GetLineNumber(){
			return LineNumber;
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			if(FirstChild==null){
				return null;
			}
			CompilationServices.CompileOperators(this,method);
			return (CompiledFragment)FirstChild.Compile(method);
		}
		
		public override string ToString(){
			string result=base.ToString();
			if(result!=""&&NextChild!=null){
				result+=";";
			}
			return result;
		}
		
	}
	
}