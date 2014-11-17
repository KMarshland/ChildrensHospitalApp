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

namespace Nitro{

	/// <summary>
	/// Represents a break or continue statement (one which breaks out of a loop, or skips the remaining code within a loop).
	/// </summary>

	public class BreakPoint{
	
		/// <summary>The location of the end of the loop.</summary>
		public Label End;
		/// <summary>The location of the start of the loop.</summary>
		public Label ContinuePoint;
		
		/// <summary>Creates a new break/continue point with the given loop information.</summary>
		/// <param name="continuePoint">The location of the start of the loop.</param>
		/// <param name="end">The location of the end of the loop.</param>
		public BreakPoint(Label continuePoint,Label end){
			End=end;
			ContinuePoint=continuePoint;
		}
		
		/// <summary>Emits a break - a jump to the end of the loop.</summary>
		/// <param name="into">The IL code to emit the instruction into.</param>
		public void Break(NitroIL into){
			into.Emit(OpCodes.Br,End);
		}
		
		/// <summary>Emits a continue - a jump to the start of the loop.</summary>
		/// <param name="into">The IL code to emit the instruction into.</param>
		public void Continue(NitroIL into){
			into.Emit(OpCodes.Br,ContinuePoint);
		}
		
	}
	
}