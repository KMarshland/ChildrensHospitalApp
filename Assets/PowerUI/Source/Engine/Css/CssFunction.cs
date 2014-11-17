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


namespace PowerUI.Css{
	
	/// <summary>
	/// A CSS function. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally.
	/// </summary>
	
	public class CssFunction{
		
		/// <summary>The main name of this function. Originates from the first result returned by GetNames.</summary>
		public string Name;
		
		/// <summary>The set of all function names that this one will handle. Usually just one. Lowercase.
		/// e.g. "rgb", "rgba".</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
	}
	
}