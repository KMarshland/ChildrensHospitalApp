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
	/// Represents a cast operation. num:float=(14):float; (int to float cast).
	/// Casts one object of a certain type to a different type.
	/// </summary>
	
	public class CastOperation:Operation{
	
		/// <summary>The target type that will be casted to.</summary>
		public Type ToType;
		/// <summary>The fragment whose value will be casted.</summary>
		public CompiledFragment ToCast;
		
		
		public CastOperation(CompiledMethod method,CompiledFragment toCast,Type toType):base(method){
			ToCast=toCast;
			ToType=toType;
		}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			return ToType;
		}
		
		public override void OutputIL(NitroIL into){
			
			// Get the input type:
			Type casting=ToCast.OutputType(out ToCast);
			
			// Special case - if the thing being casted is an object and
			// the thing we're casting to is a value type, we must unbox instead.
			if(ToType.IsValueType && casting==typeof(object)){
				ToCast.OutputIL(into);
				into.Emit(OpCodes.Unbox_Any,ToType);
				return;
			}
			
			ToCast.OutputIL(into);
			into.Emit(OpCodes.Castclass,ToType);
		}
		
	}
	
}