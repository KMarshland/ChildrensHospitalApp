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
	/// Represents a subtract (A-B) or negation (-A) operation.
	/// </summary>
	
	public class SubtractOperation:Operation{
	
		public SubtractOperation(CompiledMethod method,CompiledFragment input0,CompiledFragment input1):base(method){
			Input0=input0;
			Input1=input1;
		}
	
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			Type typeB=Input1.OutputType(out Input1);
			if(Input0==null){
				// Negation
				return Numerical(ref Input1,typeB,typeof(float));
			}
			Type typeA=Input0.OutputType(out Input0);
			return Numerical(typeA,typeB,"Subtraction",ref newOperation);
		}
		
		public override void OutputIL(NitroIL into){
			if(Input0==null){
				Input1.OutputIL(into);
				into.Emit(OpCodes.Neg);
			}else{
				Input0.OutputIL(into);
				Input1.OutputIL(into);
				into.Emit(OpCodes.Sub);
			}
		}
		
	}
	
}