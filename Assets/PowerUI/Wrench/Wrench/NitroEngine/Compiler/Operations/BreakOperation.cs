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
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents the break operation. This is used to leave for/while loops.
	/// Break may also be followed by a constant value, e.g. break 2, for leaving nested loops.
	/// </summary>

	
	public class BreakOperation:Operation{
		/// <summary>The number of loops to break out of in the case of nested loops. Set with e.g. break 2;</summary>
		public int Depth;
		
		public BreakOperation(CompiledMethod method,int depth):base(method){
			Depth=depth;
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			return null;
		}
		
		public override void OutputIL(NitroIL into){
			if(!Method.Break(into,Depth)){
				Error("Nothing to break from!");
			}
		}
		
	}
	
}