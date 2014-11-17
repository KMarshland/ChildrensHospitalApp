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
using System.Collections;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents the bitwise XOR (A^B) operator.
	/// </summary>
	
	public class OperatorBitwiseXOr:Operator{
		
		public OperatorBitwiseXOr():base("^",8){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new BitwiseXOrOperation(method,left,right);
		}
		
	}
	
}