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
using UnityEngine;


namespace PowerUI.Css.Functions{
	
	/// <summary>
	/// Represents the url() css function.
	/// </summary>
	
	public class Url:CssFunction{
		
		
		public Url(){
			
		}
		
		
		public override string[] GetNames(){
			return new string[]{"url"};
		}
		
	}
	
}



