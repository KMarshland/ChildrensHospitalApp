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
	/// A node which immediately follows an arc line.
	/// This handles the rendering of the arc itself.
	/// </summary>
	
	public class CanvasArcNode:CanvasPathNode{
		
		/// <summary>The radius of the arc.</summary>
		public float Radius;
		/// <summmary>The target angle of the arc.</summary>
		public float EndAngle;
		/// <summmary>The starting angle.</summary>
		public float StartAngle;
		/// <summary>The x location of the center of the circle.</summary>
		public float CircleCenterX;
		/// <summary>The y location of the center of the circle.</summary>
		public float CircleCenterY;
		
		
		/// <summary>Creates a new arc node for the given point.</summary>
		public CanvasArcNode(float x,float y):base(x,y){}
		
		public CanvasArcNode(){}
		
		
		/// <summary>Renders an arc from the previous point to this one.</summary>
		public override void RenderLine(CanvasContext context){
			// Grab the raw drawing data:
			DynamicTexture data=context.ImageData;
			
			// Time to go polar!
			// We're going to rotate around the pole drawing one pixel at a time.
			// For the best accuracy, we first need to find out how much to rotate through per pixel.
			
			// How much must we rotate through overall?
			float angleToRotateThrough=EndAngle-StartAngle;
			
			// How long is the arc?
			// First, what portion of a full circle is it:
			float circlePortion=angleToRotateThrough/(Mathf.PI*2f);
			
			// Next, what's the circumference of that circle
			// (and the above portion of it, thus the length of the arc):
			float arcLength=2f*Mathf.PI*Radius * circlePortion;
			
			if(arcLength==0f){
				// Nothing to draw anyway.
				return;
			}
			
			// So arc length is how many pixels long the arc is.
			// Thus to step that many times, our delta angle is..
			float deltaAngle=angleToRotateThrough/arcLength;
			
			// The current angle:
			float currentAngle=StartAngle;
			
			// The number of pixels:
			int pixelCount=(int)Mathf.Ceil(arcLength);
			
			if(pixelCount<0){
				// Going anti-clockwise. Invert deltaAngle and the pixel count:
				deltaAngle=-deltaAngle;
				pixelCount=-pixelCount;
			}
			
			// Step pixel count times:
			for(int i=0;i<pixelCount;i++){
				// Map from polar angle to coords:
				float x=Radius * (float) Math.Cos(currentAngle);
				float y=Radius * (float) Math.Sin(currentAngle);
				
				// Draw the pixel:
				data.DrawPixel((int)(CircleCenterX+x),data.Height-(int)(CircleCenterY+y),context.StrokeColour);
				
				// Rotate the angle:
				currentAngle+=deltaAngle;
			}
			
		}
		
		/// <summary>Samples this arc to find the point for the given angle.</summary>
		public Vector2 SampleAt(float angle){
			return new Vector2(
				CircleCenterX+(Radius * (float) Math.Cos(angle)),
				CircleCenterY+(Radius * (float) Math.Sin(angle))
			);
		}
		
		/// <summary>Forms an arc between this node and the one before it. Used internally from arcTo.</summary>
		public override void Setup(){
			
			// There's a line to render after the previous node:
			Previous.IsLineAfter=true;
			
		}
	
	}
	
}