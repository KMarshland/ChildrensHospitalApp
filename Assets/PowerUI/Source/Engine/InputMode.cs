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
	/// Represents the method used when capturing mouse input.
	/// <see cref="UI.Input.Mode"/>
	/// </summary>
	
	public enum InputMode{
		/// <summary>Uses physics for click detection. This mode enables World UI's to accept clicks
		/// and is more intensive than Screen.</summary>
		Physics,
		/// <summary>Uses 2D resolving for click detection. Default for the main UI.</summary>
		Screen,
		/// <summary>No click input will be accepted. This is the default for WorldUI's.</summary>
		None
	}
	
}