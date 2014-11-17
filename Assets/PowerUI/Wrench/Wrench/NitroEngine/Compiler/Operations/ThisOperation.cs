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
	/// Represents the this keyword which is used to enforce accessing a property of this class over using a local variable.
	/// </summary>
	
	public class ThisOperation:Operation{
	
		public ThisOperation(CompiledMethod method):base(method){}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			return Method.Parent.GetAsType();
		}
		
		public override void OutputIL(NitroIL into){
			into.Emit(OpCodes.Ldarg_0);
		}
		
	}
	
}