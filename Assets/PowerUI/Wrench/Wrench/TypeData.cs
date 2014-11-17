//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Wrench{
	
	/// <summary>
	/// Provides methods for dealing with types across platforms.
	/// </summary>
	
	public static class TypeData{
		
		public static bool IsSubclassOf(Type toCheck,Type classOf){
			#if NETFX_CORE
			return toCheck.GetTypeInfo().IsSubclassOf(classOf);
			#else
			return toCheck.IsSubclassOf(classOf);
			#endif
			
		}
		
		#if !NETFX_CORE
		/// <summary>Gets the set of generic parameters for the given generic type.</summary>
		public static Type[] GenericArguments(Type forType){
			return forType.GetGenericArguments();
		}
		#endif
		
	}
	
}