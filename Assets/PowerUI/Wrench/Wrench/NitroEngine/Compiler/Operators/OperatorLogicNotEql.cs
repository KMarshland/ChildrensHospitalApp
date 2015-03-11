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
	/// Represents the logical not equals (!=) operator.
	/// </summary>
	
	public class OperatorLogicNotEql:Operator{
		
		public OperatorLogicNotEql():base("!=",11){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new NotEqualOperation(method,left,right);
		}
		
	}
	
}