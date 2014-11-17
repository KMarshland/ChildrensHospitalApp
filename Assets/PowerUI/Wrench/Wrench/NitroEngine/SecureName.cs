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


namespace Nitro{
	
	/// <summary>
	/// Represents a name of a namespace or type that can be allowed or blocked from use.
	/// </summary>
	
	public class SecureName{
		
		/// <summary>The name to match. Either a namespace (e.g. System.Collections) or a fully qualified type (e.g. System.String).
		public string Name;
		
		
		/// <summary>Creates a new secure name for the given type name (e.g. 
		public SecureName(string name){
			Name=name;	
		}
		
		/// <summary>Checks if the given type falls in the scope named here.</summary>
		/// <param name="type">The typqe to check for a match</param>
		/// <returns>True if there is a successful match; false otherwise.</returns>
		public bool Matches(Type type){
			string with=type.FullName;
			return (Name==with || with.StartsWith(Name+"."));
		}
		
	}
	
}