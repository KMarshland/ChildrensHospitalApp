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
	/// Represents the bitwise and (A&B) operator.
	/// </summary>
	
	public class OperatorBitwiseAnd:Operator{
		
		public OperatorBitwiseAnd():base("&",9){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new AndOperation(method,left,right);
		}
		
	}
	
}