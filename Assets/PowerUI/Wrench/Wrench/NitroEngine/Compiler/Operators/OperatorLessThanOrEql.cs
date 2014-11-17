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
	/// Represents the less than or equal (A<=B) operator.
	/// </summary>
	
	public class OperatorLessThanOrEql:Operator{
		
		public OperatorLessThanOrEql():base("<=",12){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new OrOperation(method,new LessThanOperation(method,left,right),new EqualsOperation(method,left,right));
		}
		
	}
	
}