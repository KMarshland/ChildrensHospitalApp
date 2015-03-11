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
			bool localMode=false;
			
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
				
				// Handle the fragment:
				CodeFragment fragment;
				
				try{
					
					// Try to get the code fragment:
					fragment=Handlers.Handle(handler,sr);
					
				}catch(CompilationException e){
					
					if(e.LineNumber==-1){
						// Setup line number:
						e.LineNumber=LineNumber;
					}
					
					// Rethrow:
					throw e;
				}
				
				if(localMode){
					
					// Should always be a VariableFragment:
					
					if(fragment.GetType()==typeof(VariableFragment)){
						
						VariableFragment local=(VariableFragment)fragment;
						local.AfterVar=true;
						
					}
					
					localMode=false;
				}
				
				// Try adding the fragment to the operation:
				AddResult status=fragment.AddTo(this,sr);
				
				// What was the outcome?
				switch(status){
					case AddResult.Stop:
						// Halt.
						return;
					case AddResult.Local:
						// Local next:
						localMode=true;
					break;
					// Ok otherwise.
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
			
			// Apply the current line:
			method.CurrentLine=LineNumber;
			
			try{
				
				// Compile operator chains: (d=a+b+c;)
				CompilationServices.CompileOperators(this,method);
				
				// Compile the now singular operator:
				CompiledFragment cFrag=FirstChild.Compile(method) as CompiledFragment;
				
				return cFrag;
				
			}catch(CompilationException e){
				
				if(e.LineNumber==-1){
					// Setup line number:
					e.LineNumber=LineNumber;
				}
				
				// Rethrow:
				throw e;
			}
			
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