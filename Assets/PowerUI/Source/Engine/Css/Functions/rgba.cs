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


namespace PowerUI.Css.Functions{
	
	/// <summary>
	/// Represents the rgb() and rgba() css functions.
	/// </summary>
	
	public class Rgba:CssFunction{
		
		
		public Rgba(){
			
		}
		
		
		public override string[] GetNames(){
			return new string[]{"rgb","rgba"};
		}
		
	}
	
}



