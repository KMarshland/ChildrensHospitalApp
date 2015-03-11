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
	/// Represents the greater than or equal (A>=B) operator.
	/// </summary>
	
	public class OperatorGreaterThanOrEql:Operator{
		
		public OperatorGreaterThanOrEql():base(">=",13){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new GreaterThanOrEqualOperation(method,left,right);
		}
		
	}
	
}