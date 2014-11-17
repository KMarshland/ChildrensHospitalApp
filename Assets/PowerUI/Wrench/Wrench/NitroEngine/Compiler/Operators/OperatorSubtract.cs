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
	/// Represents the subtract (A-B) or negation (-A) operators.
	/// </summary>
	
	public class OperatorSubtract:Operator{
	
		public OperatorSubtract():base("-",18){
			// Also supports right only for negation:
			RightOnly=true;
		}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new SubtractOperation(method,left,right);
		}
		
	}
	
}