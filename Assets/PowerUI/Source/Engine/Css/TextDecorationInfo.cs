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


namespace PowerUI.Css{
	
	/// <summary>
	/// Info about a line drawn over some text.</summary>
	/// </summary>
	
	public class TextDecorationInfo{
		
		/// <summary>The colour of the line, with colour overlay applied.</summary>
		public Color Colour;
		/// <summary>The colour of the line without overlay.</summary>
		public Color BaseColour;
		/// <summary>The position of the line.</summary>
		public TextLineType Type;
		/// <summary>Is the line colour a custom one? Otherwise the font colour is used.</summary>
		public bool ColourOverride;
		
		
		/// <summary>Creates a new line of the given type.</summary>
		public TextDecorationInfo(TextLineType type){
			Type=type;
		}
		
		/// <summary>Sets a custom colour to this decoration.</summary>
		public void SetColour(Color colour){
			ColourOverride=true;
			BaseColour=colour;
			Colour=colour;
		}
		
		/// <summary>Applies the overlay colour. Only called when ColourOverride is true.</summary>
		public void SetOverlayColour(Color colour){
			Colour=BaseColour*colour;
		}
		
	}
	
}