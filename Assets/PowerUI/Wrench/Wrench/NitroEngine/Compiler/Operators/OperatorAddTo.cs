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
	/// Represents the add into (A+=B) operator.
	/// </summary>
	
	public class OperatorAddTo:Operator{
		
		public OperatorAddTo():base("+=",2){}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return new SetOperation(method,left,new AddOperation(method,left,right));
		}
		
	}
	
}