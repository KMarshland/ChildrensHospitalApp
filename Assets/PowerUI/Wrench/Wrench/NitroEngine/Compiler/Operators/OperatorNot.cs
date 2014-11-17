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
	/// Represents the not (!A) operator.
	/// </summary>
	
	public class OperatorNot:Operator{
	
		public OperatorNot():base("!",25){
			LeftAndRight=false;
			RightOnly=true;
		}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new EqualsOperation(method,new CompiledFragment(0),right);
		}
	}
	
}