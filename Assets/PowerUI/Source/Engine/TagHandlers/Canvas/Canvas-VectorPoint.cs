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
using PowerUI.Css;
using System.Collections;
using System.Collections.Generic;
using PowerUI;


namespace Blaze{
	
	public partial class VectorPoint{
		
		/// <summary>Used internally. Renders the line between this point and the next one, if there is one.</summary>
		/// <param name="data">The image to draw to.</param>
		public virtual void RenderLine(CanvasContext context){}
		
	}
	
}