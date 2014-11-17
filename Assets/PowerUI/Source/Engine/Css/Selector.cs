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
	/// Used when a,series,of,selectors are defined in CSS. 
	/// They are stored in this parsed structure until the full block is loaded.
	/// <summary>

	public class Selector{
		
		/// <summary>A :modifier for the selector, e.g. "hover".</summary>
		public string Modifier;
		/// <summary>The style block for this selector.</summary>
		public SelectorStyle Style;
		/// <summary>The raw selector itself.</summary>
		public string SelectorText;
		
		
		public Selector(StyleSheet sheet,string selectorText,string modifier){
			SelectorText=selectorText;
			Modifier=modifier;
			
			// Start it off immediately:
			Style=sheet.StartSelector(selectorText,modifier);
		}
		
	}
	
}