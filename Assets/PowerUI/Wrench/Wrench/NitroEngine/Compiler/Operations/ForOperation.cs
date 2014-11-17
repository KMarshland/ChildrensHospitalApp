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
	/// Represents a for or while loop.
	/// </summary>
	
	public class ForOperation:Operation{
	
		/// <summary>The block of code that will be looped over.</summary>
		public BracketFragment Body;
		/// <summary>The parameters are the set of operations within the for loops (brackets).</summary>
		public CompiledFragment[] Parameters;
		
		
		public ForOperation(CompiledMethod method,BracketFragment rules,BracketFragment body):base(method){
			Body=body;
			Parameters=CompilationServices.CompileParameters(rules,method);
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			return null;
		}
		
		public override void OutputIL(NitroIL into){
			// Emit any set operations before the block itself (e.g. i=0):
			for(int i=0;i<Parameters.Length;i++){
				CompiledFragment parameter=Parameters[i];
				if(Types.IsTypeOf(parameter,typeof(SetOperation))){
					// Make sure it isn't a self set - e.g. i++, i=i+10.
					SetOperation set=(SetOperation)parameter;
					if(!set.SelfReferencing()){
						set.OutputIL(into);
						Parameters[i]=null;
					}else{
						set.Output=false;
					}
				}else{
					// Compute it's output type:
					// Note that this must NOT be done for the SET ops as it makes them think
					// We want to use their output, which isn't true!
					parameter.OutputType(out parameter);
					Parameters[i]=parameter;
				}
			}
			Label continuePoint=into.DefineLabel();
			Label start=into.DefineLabel();
			Label end=into.DefineLabel();
			Method.AddBreakPoint(new BreakPoint(continuePoint,end));
			into.MarkLabel(start);
			
			// Logical operations (e.g. i<10):
			for(int i=0;i<Parameters.Length;i++){
				CompiledFragment parameter=Parameters[i];
				if(parameter==null){
					continue;
				}
				if(Types.IsTypeOf(parameter,typeof(SetOperation))){
					// Just checking for a logical operation will make an incremental (i++) operation think
					// It's being daisy chained. That's because its output type is checked for boolean in IsLogical.
					continue;
				}
				if(parameter.IsLogical()){
					parameter.OutputIL(into);
					into.Emit(OpCodes.Brfalse,end);
					Parameters[i]=null;
				}
			}
			
			Body.CompileBody(Method);
			into.MarkLabel(continuePoint);
			
			// Increment/decrement ops (e.g. i++):
			for(int i=0;i<Parameters.Length;i++){
				CompiledFragment cfrag=Parameters[i];
				if(cfrag==null){
					continue;
				}
				cfrag.OutputIL(into);
			}
			into.Emit(OpCodes.Br,start);
			into.MarkLabel(end);
			Method.PopBreakPoint();
		}
		
	}
	
}