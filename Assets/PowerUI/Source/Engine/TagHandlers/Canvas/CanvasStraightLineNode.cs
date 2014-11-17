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
using PowerUI.Css;


namespace PowerUI{
	
	/// <summary>
	/// A node which immediately follows a straight line.
	/// This handles the rendering of the line itself.
	/// </summary>
	
	public class CanvasStraightLineNode:CanvasPathNode{
		
		/// <summary>The minimum point of the bounding box of the line.</summary>
		public Vector2 MaximumBounds=Vector2.zero;
		/// <summary>The maximum point of the bounding box of the line.</summary>
		public Vector2 MinimumBounds=Vector2.zero;
		
		
		/// <summary>Creates a new straight line node for the given point.</summary>
		public CanvasStraightLineNode(float x,float y):base(x,y){}
		
		
		public override void RecalculateBounds(){
			
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			// Which x is smaller?
			if(previousX<X){
				// This one is bigger.
				MinimumBounds.x=previousX;
				MaximumBounds.x=X;
			}else{
				// Previous is bigger.
				MinimumBounds.x=X;
				MaximumBounds.x=previousX;
			}
			
			// Which y is smaller?
			if(previousY<Y){
				// This one is bigger.
				MinimumBounds.y=previousY;
				MaximumBounds.y=Y;
			}else{
				// Previous is bigger.
				MinimumBounds.y=Y;
				MaximumBounds.y=previousY;
			}
		}
		
		/// <summary>Renders a straight line from the previous point to this one.</summary>
		public override void RenderLine(CanvasContext context){
			// Grab the raw drawing data:
			DynamicTexture data=context.ImageData;
			
			// Invert y:
			int endY=data.Height-(int)Y;
			int startY=data.Height-(int)Previous.Y;
			
			// Grab X:
			int endX=(int)X;
			int startX=(int)Previous.X;
			
			data.DrawLine(startX,startY,endX,endY,context.StrokeColour);
		}
		
		/// <summary>Forms a straight line from this node to the one before it. Used internally from lineTo.</summary>
		public override void Setup(){
			// Form a straight line between 'me' and 'previous'.
			
			// There's a line to render after the previous node:
			Previous.IsLineAfter=true;
			
		}
		
	}
	
}