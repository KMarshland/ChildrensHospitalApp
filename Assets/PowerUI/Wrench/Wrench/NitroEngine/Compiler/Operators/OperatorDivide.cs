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
	/// Represents the divide operator (A/B).
	/// </summary>
	
	public class OperatorDivide:Operator{
	
		public OperatorDivide():base("/",21){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new DivideOperation(method,left,right);
		}
		
	}
	
}