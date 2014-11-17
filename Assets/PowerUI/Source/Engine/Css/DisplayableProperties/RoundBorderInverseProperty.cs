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
	/// Represents the inverse section of round borders.
	/// Simply acts as a block allocation container, separating the "inverse" corner from the main coloured corner.
	/// </summary>
	
	public class RoundBorderInverseProperty:DisplayableProperty{
		
		
		/// <summary>Creates a new border property for the given element.</summary>
		/// <param name="element">The element to give a border to.</param>
		public RoundBorderInverseProperty(Element element):base(element){
			
		}
		
		
		public override void SetOverlayColour(Color colour){}
		
		public override void Paint(){}
		
		protected override void Layout(){}
		
	}
	
}