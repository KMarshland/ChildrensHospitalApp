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
	/// Represents a type being used as an object. E.g. for statics, such as string.concat(..).
	/// </summary>
	
	public class TypeOperation:Operation{
	
		/// <summary>The type this object represents.</summary>
		public Type TypeObject;
		
		public TypeOperation(CompiledMethod method,Type typeObject):base(method){
			TypeObject=typeObject;
		}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			return typeof(System.Type);
		}
		
		public override void OutputIL(NitroIL into){
			into.Emit(OpCodes.Ldtoken,TypeObject);
			MethodInfo methodToCall=typeof(System.Type).GetMethod("GetTypeFromHandle");
			into.Emit(OpCodes.Call,methodToCall);
		}
		
	}
	
}