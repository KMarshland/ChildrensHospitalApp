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
	/// Represents the javascript window object.
	/// </summary>
	
	public class Window{
		
		/// <summary>The very top window for this UI.</summary>
		public Window top;
		/// <summary>Represents 'this' object. Provided for javascript compatability.</summary>
		public Window self;
		/// <summary>The latest event to have occured.</summary>
		public UIEvent Event;
		/// <summary>The parent window of this one.</summary>
		public Window parent;
		/// <summary>The iframe element that this window is in.</summary>
		public Element iframe;
		/// <summary>The document for the given window.</summary>
		public Document document;
		
		
		/// <summary>Creates a new window object.</summary>
		public Window(){
			self=this;
		}
		
	}
	
}