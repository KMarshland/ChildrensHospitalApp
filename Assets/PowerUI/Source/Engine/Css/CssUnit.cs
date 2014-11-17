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
	
	public class CssUnit:CssFunction{
		
		/// <summary>Text after the raw value. E.g. px.</summary>
		public string Suffix;
		/// <summary>Text before the raw value. E.g. #.</summary>
		public string Prefix;
		
	}
	
}