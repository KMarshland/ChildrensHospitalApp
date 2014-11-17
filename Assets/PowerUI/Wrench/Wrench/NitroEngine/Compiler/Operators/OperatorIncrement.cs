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
	/// Represents the increment (A++) operator.
	/// </summary>
	
	public class OperatorIncrement:Operator{
	
		public OperatorIncrement():base("++",24){
			LeftAndRight=false;
			LeftOnly=true;
		}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			CompiledFragment frag=left;
			if(left==null){
				frag=right;
			}
			return new SetOperation(method,frag,new AddOperation(method,frag,new CompiledFragment(1)));
		}
		
	}
	
}