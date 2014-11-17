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
	/// Represents the subtract from (A-=B) operator.
	/// </summary>
	
	public class OperatorSubtractFrom:Operator{
	
		public OperatorSubtractFrom():base("-=",1){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new SetOperation(method,left,new SubtractOperation(method,left,right));
		}
		
	}
	
}