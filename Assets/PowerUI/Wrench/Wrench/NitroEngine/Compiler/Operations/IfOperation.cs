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
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents an if operation.
	/// </summary>
	
	public class IfOperation:Operation{
	
		/// <summary>True if both true and false blocks return something.</summary>
		public bool AllRoutesReturn;
		/// <summary>The block of code to execute if the conditions are true.</summary>
		public BracketFragment IfTrue;
		/// <summary>The block of code to execute if the conditions are false.</summary>
		public BracketFragment IfFalse;
		/// <summary>The set of conditions that must be true. Note that only 1 (the first) is considered at the moment.</summary>
		public CompiledFragment[] Conditions;
		
		
		public IfOperation(CompiledMethod method,BracketFragment condition,BracketFragment ifTrue,BracketFragment ifFalse):base(method){
			IfTrue=ifTrue;
			IfFalse=ifFalse;
			
			Conditions=CompilationServices.CompileParameters(condition,method);
			
			if(Conditions==null||Conditions.Length==0){
				Error("An if was defined but with nothing to check (e.g. if(this is empty!){..} )");
			}
			
		}
		
		public override bool RequiresStoring{
			get{
				return false;
			}
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			
			for(int i=0;i<Conditions.Length;i++){
				CompiledFragment cond=Conditions[i];
				cond.OutputType(out cond);
				Conditions[i]=cond;
			}
			
			return null;
		}
		
		public override void OutputIL(NitroIL into){
			// Ensure our conditions output type is computed:
			OutputType();
			
			Label End=into.DefineLabel();
			Label Else=into.DefineLabel();
			Conditions[0].OutputIL(into);
			into.Emit(OpCodes.Brfalse,Else);
			AllRoutesReturn=IfTrue.CompileBody(Method);
			into.Emit(OpCodes.Br,End);
			into.MarkLabel(Else);
			
			if(IfFalse!=null){
				bool returns=IfFalse.CompileBody(Method);
				AllRoutesReturn=(AllRoutesReturn&&returns);
			}else{
				AllRoutesReturn=false;
			}
			
			into.MarkLabel(End);
		}
		
	}
	
}