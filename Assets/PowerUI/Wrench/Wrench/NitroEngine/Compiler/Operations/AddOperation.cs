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
	/// Represents the add (A+B) operation.
	/// </summary>
	
	public class AddOperation:Operation{
		
		public AddOperation(CompiledMethod method,CompiledFragment input0,CompiledFragment input1):base(method){
			Input0=input0;
			Input1=input1;
		}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			Type typeB=Input1.OutputType(out Input1);
			Type typeA=Input0.OutputType(out Input0);
			if(typeA!=typeB){
				bool BString=(typeB==typeof(string));
				if(typeA==typeof(string)||BString){
					if(BString){
						// This is alright - convert Input0 to a ToString operation.
						Input0=Types.ToStringMethod(Method,Input0,typeA);
						typeA=typeof(string);
					}else{
						Input1=Types.ToStringMethod(Method,Input1,typeB);
						typeB=typeof(string);
					}
				}
			}
			if(typeA==typeof(string)&&typeB==typeof(string)){
				// Adding two strings (concat).
				newOperation=new MethodOperation(Method,typeof(string).GetMethod("Concat",new Type[]{typeof(string),typeof(string)}),Input0,Input1);
			}else{
				typeA=Numerical(typeA,typeB,"Addition",ref newOperation);
			}
			return typeA;
		}
		
		public override void OutputIL(NitroIL into){
			Input0.OutputIL(into);
			Input1.OutputIL(into);
			into.Emit(OpCodes.Add);
		}
		
	}
	
}