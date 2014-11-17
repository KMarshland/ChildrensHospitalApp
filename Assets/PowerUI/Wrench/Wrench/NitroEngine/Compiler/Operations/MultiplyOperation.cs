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
	/// Represents the multiplication (A*B) operation.
	/// </summary>
	
	public class MultiplyOperation:Operation{
	
		public MultiplyOperation(CompiledMethod method,CompiledFragment input0,CompiledFragment input1):base(method){
			Input0=input0;
			Input1=input1;
		}
	
		public override Type OutputType(out CompiledFragment v){
			v=this;
			Type TypeA=Input1.OutputType(out Input1);
			Type TypeB=Input0.OutputType(out Input0);
			return Numerical(TypeA,TypeB,"Multiply",ref v);
		}
		
		public override void OutputIL(NitroIL into){
			Input0.OutputIL(into);
			Input1.OutputIL(into);
			into.Emit(OpCodes.Mul);
		}
		
	}
	
}