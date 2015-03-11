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
	/// Represents the less than (A<B) operation.
	/// </summary>
	
	public class LessThanOperation:Operation{
	
		public LessThanOperation(CompiledMethod method,CompiledFragment input0,CompiledFragment input1):base(method){
			Input0=input0;
			Input1=input1;
		}
	
		public override Type OutputType(out CompiledFragment v){
			v=this;
			Type typeA=Input0.OutputType(out Input0);
			Type typeB=Input1.OutputType(out Input1);
			
			CompiledFragment overload=null;
			FindOverload("LessThan",typeA,typeB,ref overload);
			
			if(overload!=null){
				v=overload;
			}
			
			return typeof(bool);
		}
		
		public override void OutputIL(NitroIL into){
			Input0.OutputIL(into);
			Input1.OutputIL(into);
			into.Emit(OpCodes.Clt);
		}
		
	}
	
}