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

namespace PowerUI{
	
	/// <summary>
	/// Represents the mouseover state an element is in.
	/// </summary>
	
	public enum MouseOverState{
		/// <summary>Mouse is over, but it didn't consume the event.</summary>
		Over,
		/// <summary>Mouse is not over the element.</summary>
		Out,
		/// <summary>Mouse is over and it consumed the event.</summary>
		OverConsumed
	}
	
}