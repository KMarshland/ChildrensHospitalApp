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
using System.Collections;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents the base of "dynamic methods" - a method that can be called at runtime.
	/// Essentially acts like a delegate.
	/// </summary>
	
	public class NitroBaseMethod{
		
		/// <summary>Executes this method with the given arguments.</summary>
		/// <param name="arguments">The arguments to pass to the method</param>
		/// <returns>The return value of the method, if any.</returns>
		public virtual object RunBoxed(params object[] arguments){
			return null;
		}
		
	}
	
}