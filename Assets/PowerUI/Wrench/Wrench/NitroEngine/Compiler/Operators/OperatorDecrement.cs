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
	/// Represents the decrement (A--) operator.
	/// </summary>
	
	public class OperatorDecrement:Operator{
		
		public OperatorDecrement():base("--",23){
			LeftAndRight=false;
			LeftOnly=true;
		}
		
		protected override Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			CompiledFragment frag=left;
			if(left==null){
				frag=right;
			}
			return new SetOperation(method,frag,new SubtractOperation(method,frag,new CompiledFragment(1)));
		}
	}
	
}