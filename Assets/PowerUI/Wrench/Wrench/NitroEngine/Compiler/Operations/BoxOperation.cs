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
	/// Represents a box operation. These convert a value type into an object.
	/// </summary>
	
	public class BoxOperation:Operation{
	
		/// <summary>The value type being boxed into an object.</summary>
		public Type FromType;
		/// <summary>The fragment being boxed.</summary>
		public CompiledFragment ToBox;
		
		public BoxOperation(CompiledMethod method,CompiledFragment toBox):base(method){
			ToBox=toBox;
		}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			if(FromType==null){
				FromType=ToBox.OutputType(out ToBox);
			}
			return typeof(object);
		}
		
		public override void OutputIL(NitroIL into){
			if(FromType==null){
				FromType=ToBox.OutputType(out ToBox);
			}
			ToBox.OutputIL(into);
			into.Emit(OpCodes.Box,FromType);
		}
		
	}
	
}