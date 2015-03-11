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
	/// Represents a continue operation. It's used to skip remaining code within a loop. It may skip nested loops too using e.g. continue 2;.
	/// </summary>
	
	public class ContinueOperation:Operation{
		
		/// <summary>The number of loops to continue within in the case of nested loops. Set with e.g. continue 2;</summary>
		public int Depth;
		
		public ContinueOperation(CompiledMethod method,int depth):base(method){
			Depth=depth;
		}
		
		public override bool RequiresStoring{
			get{
				return false;
			}
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			return null;
		}
		
		public override void OutputIL(NitroIL into){
			if(!Method.Continue(into,Depth)){
				Error("Nothing to continue in!");
			}
		}
		
	}
	
}