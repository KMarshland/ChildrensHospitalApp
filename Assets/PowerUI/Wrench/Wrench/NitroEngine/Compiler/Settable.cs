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

using System.Reflection.Emit;

namespace Nitro{
	
	/// <summary>
	/// Represents an object which may have a value set to it.
	/// e.g. a variable or an indexed array (something[14]=value;)
	/// </summary>
	
	internal interface ISettable{
		
		/// <summary>Generates the instruction which performs the set.</summary>
		void OutputSet(NitroIL into);
		/// <summary>Outputs the location where the value must be set to.</summary>
		void OutputTarget(NitroIL into);
		
	}
	
}