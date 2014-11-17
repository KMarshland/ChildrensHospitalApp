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
	/// Represents an unbox operation. These convert an object into a value type.
	/// </summary>
	
	public class UnboxOperation:Operation{
	
		/// <summary>The value type being boxed into an object.</summary>
		public Type ToType;
		/// <summary>The fragment being unboxed.</summary>
		public CompiledFragment ToUnbox;
		
		public UnboxOperation(CompiledMethod method,CompiledFragment unBox,Type targetType):base(method){
			ToUnbox=unBox;
			ToType=targetType;
		}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			return ToType;
		}
		
		public override void OutputIL(NitroIL into){
			ToUnbox.OutputIL(into);
			into.Emit(OpCodes.Unbox_Any,ToType);
		}
		
	}
	
}