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
	/// Represents the base object. Use this for calling parent methods in overriding methods.
	/// </summary>
	
	public class BaseOperation:Operation{
	
		public BaseOperation(CompiledMethod method):base(method){}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			return Method.Parent.BaseType;
		}
		
		public override void OutputIL(NitroIL into){
			into.Emit(OpCodes.Ldarg_0);
		}
		
	}
	
}