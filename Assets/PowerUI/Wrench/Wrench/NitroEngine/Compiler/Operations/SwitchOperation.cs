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
	/// Represents a switch.
	/// </summary>
	
	public class SwitchOperation:Operation{
	
		/// <summary>The object being switched.</summary>
		public Variable Switching;
		/// <summary>The block of code that contains the cases.</summary>
		public BracketFragment Body;
		
		
		public SwitchOperation(CompiledMethod method,BracketFragment switching,BracketFragment body):base(method){
			Body=body;
			
			if(switching.ChildCount()!=1){
				
				switching.Error("Too many entries inside this switches brackets. Should be e.g. switch(name){ .. }");
				
			}
			
			// Compile the switching frag:
			CompiledFragment variableFrag=switching.FirstChild.Compile(method);
			
			// Get the active value - this should be a variable object:
			object activeValue=variableFrag.ActiveValue();
			
			// Try and apply it:
			Switching=activeValue as Variable;
			
			if(Switching==null){
				
				switching.Error("Can only switch variables.");
				
			}
			
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
			
			Wrench.Log.Add("Switch operations are currently unsupported. The whole switch block will be ignored - use ifs instead.");
			
		}
		
	}
	
}