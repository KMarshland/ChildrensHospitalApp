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
	/// Represents the a rounded corner of a border.
	/// This works by creating two sections - the "inverse" border and the border itself.
	/// The inverse border is essentially a series of transparent whose purpose is just to affect the depth buffer.
	/// </summary>
	
	public class RoundCorner{
		
		/// <summary>The amount of pixels per section of the corner. The higher this is, the smoother the corner (but the lower the performance).</summary>
		public static int Resolution=4;
		
		/// <summary>The edge index that this corner goes to.</summary>
		public int ToIndex;
		/// <summary>The edge index that this corner goes from.</summary>
		public int FromIndex;
		/// <summary>The radius of this corner.</summary>
		public int CornerRadius;
		/// <summary>How far the outer arc is from the corner of the original box at each block. Note that this is always round; One axis is CornerRadius-theOtherAxis.</summary>
		private Vector2[] OuterArc;
		/// <summary>How far the inner arc is from the corner of the original box at each block.</summary>
		private Vector2[] InnerArc;
		/// <summary>The number of blocks (fragments of the curve; translate directly to MeshBlocks) that this corner is constructed of.</summary>
		private int BlocksRequired;
		/// <summary>The border this corner belongs to.</summary>
		public BorderProperty Border;
		/// <summary>The number of inverse blocks required.</summary>
		public int InverseBlocksRequired;
		/// <summary>The parent round corners set.</summary>
		public RoundedCorners RoundCorners;
		/// <summary>The position of this corner (e.g. top left).</summary>
		public RoundCornerPosition Position;
		/// <summary>The inverse property which inverse fragments are rendered to.</summary>
		public RoundBorderInverseProperty InverseBorder;
		
		
		/// <summary>Creates a new rounded corner in the given position.</summary>
		public RoundCorner(RoundedCorners roundCorners,RoundCornerPosition position){
			Position=position;
			RoundCorners=roundCorners;
			Border=roundCorners.Border;
			InverseBorder=roundCorners.InverseBorder;
			
			// Get the to index:
			ToIndex=(int)position;
			
			// Get the from index:
			FromIndex=ToIndex-1;
			
			// May need to wrap it:
			if(FromIndex==-1){
				FromIndex=3;
			}
			
		}
		
		/// <summary>The radius of this corner.</summary>
		public int Radius{
			get{
				return CornerRadius;
			}
			set{
				// Has it changed?
				if(CornerRadius==value){
					return;
				}
				
				CornerRadius=value;
				
				// How many blocks does this corner require? Rounded up.
				BlocksRequired=((value-1)/Resolution)+1;
				
				// How many inverse blocks?
				InverseBlocksRequired=((BlocksRequired-1)/2)+1;
				
				// get the required arc size:
				int arcSizeRequired=BlocksRequired+1;
				
				// Create the outer arc:
				if(OuterArc==null || OuterArc.Length!=arcSizeRequired){
				
					// It's required:
					OuterArc=new Vector2[arcSizeRequired];
					
				}
				
				// Recompute the arcs now:
				RecomputeArcs();
				
			}
		}
		
		/// <summary>Recomputes both arcs simultaneously.</summary>
		private void RecomputeArcs(){
			
			// How big is the outer arc?
			int size=OuterArc.Length;
			
			if(InnerArc==null || InnerArc.Length!=size){
				// Create the set now:
				InnerArc=new Vector2[size];
			}
			
			// We're next going to be doing some polars here. Everything is clockwise.
			// We're essentially going to rotate around the virtual center of a circle, changing our radius as we go.
			
			// The current angle in radians:
			float currentAngle;
			
			// The virtual center of the circle, relative to the corner:
			float centerX;
			float centerY;
			
			// The source border width:
			int sourceWidth;
			// The target border width:
			int targetWidth;
			
			// Get radius as a float:
			float cornerRadius=(float)CornerRadius;
			
			// The starting radius:
			float radius=cornerRadius;
			
			switch(Position){
				case RoundCornerPosition.TopLeft:
					
					// The center is up on Y and up on X:
					centerX=radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI;
					
					// Get the widths:
					sourceWidth=Border.WidthLeft;
					targetWidth=Border.WidthTop;
					
				break;
				case RoundCornerPosition.TopRight:
					
					// The center is up on Y and down on X:
					centerX=-radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*1.5f;
					
					// Get the widths:
					sourceWidth=Border.WidthTop;
					targetWidth=Border.WidthRight;
					
				break;
				case RoundCornerPosition.BottomRight:
					
					// The center is down on Y and down on X:
					centerX=-radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=0f;
					
					// Get the widths:
					sourceWidth=Border.WidthRight;
					targetWidth=Border.WidthBottom;
					
				break;
				default:
				case RoundCornerPosition.BottomLeft:
					
					// The center is down on Y and up on X:
					centerX=radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*0.5f;
					
					// Get the widths:
					sourceWidth=Border.WidthBottom;
					targetWidth=Border.WidthLeft;
					
				break;
			}
			
			// Remove source width from radius:
			radius-=(float)sourceWidth;
			
			// What's the maximum number of iterations?
			float maximumValue=(float)(size-1);
			
			// Figure out delta angle (we'll be travelling through PI/2 degrees):
			float deltaAngle=(Mathf.PI*0.5f)/maximumValue;
			
			// Figure out delta radius based on source/target:
			float deltaRadius=(float)(sourceWidth-targetWidth)/maximumValue;
			
			// Next, for each point..
			for(int i=0;i<size;i++){
				
				// Get the cos/sin of the current angle:
				float cosAngle=Mathf.Cos(currentAngle);
				float sinAngle=Mathf.Sin(currentAngle);
				
				// Figure out the outer arc value:
				OuterArc[i]=new Vector2(centerX+(cosAngle*cornerRadius),centerY+(sinAngle*cornerRadius));
				
				// And also the inner arc value:
				InnerArc[i]=new Vector2(centerX+(cosAngle*radius),centerY+(sinAngle*radius));
				
				// Move the angle along:
				currentAngle+=deltaAngle;
				
				// Move the radius along:
				radius+=deltaRadius;
				
			}
			
		}
		
		/// <summary>Recomputes the inner arc. The inner arc is special because it depends on border width.
		/// The two borders that this corner connects may be different widths, so it may have to transition from one thickness to another.</summary>
		public void RecomputeInnerArc(){
			
			// How big is the outer arc?
			int size=OuterArc.Length;
			
			if(InnerArc==null || InnerArc.Length!=size){
				// Create the set now:
				InnerArc=new Vector2[size];
			}
			
			// We're next going to be doing some polars here. Everything is clockwise.
			// We're essentially going to rotate around the virtual center of a circle, changing our radius as we go.
			
			// The current angle in radians:
			float currentAngle;
			
			// The virtual center of the circle, relative to the corner:
			float centerX;
			float centerY;
			
			// The source border width:
			int sourceWidth;
			// The target border width:
			int targetWidth;
			
			// Get radius as a float:
			float cornerRadius=(float)CornerRadius;
			
			// The starting radius:
			float radius=cornerRadius;
			
			switch(Position){
				case RoundCornerPosition.TopLeft:
					
					// The center is up on Y and up on X:
					centerX=radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI;
					
					// Get the widths:
					sourceWidth=Border.WidthLeft;
					targetWidth=Border.WidthTop;
					
				break;
				case RoundCornerPosition.TopRight:
					
					// The center is up on Y and down on X:
					centerX=-radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*1.5f;
					
					// Get the widths:
					sourceWidth=Border.WidthTop;
					targetWidth=Border.WidthRight;
					
				break;
				case RoundCornerPosition.BottomRight:
					
					// The center is down on Y and down on X:
					centerX=-radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=0f;
					
					// Get the widths:
					sourceWidth=Border.WidthRight;
					targetWidth=Border.WidthBottom;
					
				break;
				default:
				case RoundCornerPosition.BottomLeft:
					
					// The center is down on Y and up on X:
					centerX=radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*0.5f;
					
					// Get the widths:
					sourceWidth=Border.WidthBottom;
					targetWidth=Border.WidthLeft;
					
				break;
			}
			
			// Remove source width from radius:
			radius-=(float)sourceWidth;
			
			// What's the maximum number of iterations?
			float maximumValue=(float)(size-1);
			
			// Figure out delta angle (we'll be travelling through PI/2 degrees):
			float deltaAngle=(Mathf.PI*0.5f)/maximumValue;
			
			// Figure out delta radius based on source/target:
			float deltaRadius=(float)(sourceWidth-targetWidth)/maximumValue;
			
			// Next, for each point..
			for(int i=0;i<size;i++){
				
				// Get the cos/sin of the current angle:
				float cosAngle=Mathf.Cos(currentAngle);
				float sinAngle=Mathf.Sin(currentAngle);
				
				// And also the inner arc value:
				InnerArc[i]=new Vector2(centerX+(cosAngle*radius),centerY+(sinAngle*radius));
				
				// Move the angle along:
				currentAngle+=deltaAngle;
				
				// Move the radius along:
				radius+=deltaRadius;
				
			}
			
		}
		
		/// <summary>Renders the inverse of this corner for the border.</summary>
		public void RenderInverse(float cornerX,float cornerY){
			
			// Grab the renderer:
			Renderman renderer=RoundCorners.Renderer;
			
			// Get the z-Index:
			float zIndex=renderer.Depth+0.004f;
			
			// Grab the size of the outer arc array:
			int arcSize=OuterArc.Length;
			
			int currentIndex=0;
			
			// Resolve the corner:
			Vector3 corner=renderer.PixelToWorldUnit(cornerX,cornerY,zIndex);
			
			// For each inverse block:
			for(int i=0;i<InverseBlocksRequired;i++){
				
				// Get a block:
				MeshBlock block=InverseBorder.Add();
				
				// Set the clear colour:
				block.SetColour(Color.clear);
				
				// Always going to be space to sample two. Sample the first:
				Vector2 outerPoint=OuterArc[currentIndex];
				
				// Apply the triangle:
				block.VertexTopRight=corner;
				
				// Apply the first:
				block.VertexTopLeft=renderer.PixelToWorldUnit(cornerX+outerPoint.x,cornerY+outerPoint.y,zIndex);
				
				// Sample the second:
				outerPoint=OuterArc[currentIndex+1];
				
				// Apply the second:
				block.VertexBottomLeft=renderer.PixelToWorldUnit(cornerX+outerPoint.x,cornerY+outerPoint.y,zIndex);
				
				if((currentIndex+2)>=arcSize){
					// Match the previous vertex:
					block.VertexBottomRight=block.VertexBottomLeft;
				}else{
					// Grab the next point along:
					outerPoint=OuterArc[currentIndex+2];
					
					// Resolve and apply the third:
					block.VertexBottomRight=renderer.PixelToWorldUnit(cornerX+outerPoint.x,cornerY+outerPoint.y,zIndex);
				}
				
				// Move index along:
				currentIndex+=2;
				
			}
			
		}
		
		public void Render(float alpha,float cornerX,float cornerY){
			
			// Grab the renderer:
			Renderman renderer=RoundCorners.Renderer;
			
			// Get the z-Index:
			float zIndex=renderer.Depth+0.006f;
			
			// Figure out where half way is (divide by 2):
			int halfway=(BlocksRequired>>1);
			
			Color colour;
			
			if(Border.Colour==null){
				
				if(RoundCorners.Computed.Text!=null){
					
					// Same as the font colour:
					colour=RoundCorners.Computed.Text.FontColour;
					
				}else{
					// Get the default colour:
					colour=Color.black;
					
					// Alpha is required:
					colour.a=alpha;
				}
				
			}else if(Border.Colour.Length==1){
				
				// Get the only colour:
				colour=Border.Colour[0];
				
			}else{
				
				// Get the first colour:
				colour=Border.Colour[FromIndex];
				
			}
			
			// Grab the clipping boundary:
			BoxRegion clip=renderer.ClippingBoundary;
			
			// Make it relative to the corners location:
			float minClipX=clip.X-cornerX;
			float minClipY=clip.Y-cornerY;
			float maxClipX=clip.MaxX-cornerX;
			float maxClipY=clip.MaxY-cornerY;
			
			// For each block..
			for(int i=0;i<BlocksRequired;i++){
				
				// Get a block:
				MeshBlock block=Border.Add();
				
				// Read the outer arc:
				Vector2 outerPointA=OuterArc[i];
				
				// Figure out the bounding box (constant for a particular block).
				float minX=outerPointA.x;
				float maxX=minX;
				float minY=outerPointA.y;
				float maxY=minY;
				
				Vector2 outerPointB=OuterArc[i+1];
				
				// Update the bounding box:
				if(outerPointB.x<minX){
					minX=outerPointB.x;
				}else if(outerPointB.x>maxX){
					maxX=outerPointB.x;
				}
				
				if(outerPointB.y<minY){
					minY=outerPointB.y;
				}else if(outerPointB.y>maxY){
					maxY=outerPointB.y;
				}
				
				// Line segment A->B on the "outer" arc.
				
				// Read the inner arc:
				Vector2 innerPointA=InnerArc[i];
				
				// Update the bounding box:
				if(innerPointA.x<minX){
					minX=innerPointA.x;
				}else if(innerPointA.x>maxX){
					maxX=innerPointA.x;
				}
				
				if(innerPointA.y<minY){
					minY=innerPointA.y;
				}else if(innerPointA.y>maxY){
					maxY=innerPointA.y;
				}
				
				Vector2 innerPointB=InnerArc[i+1];
				
				// Update the bounding box:
				if(innerPointB.x<minX){
					minX=innerPointB.x;
				}else if(innerPointB.x>maxX){
					maxX=innerPointB.x;
				}
				
				if(innerPointB.y<minY){
					minY=innerPointB.y;
				}else if(innerPointB.y>maxY){
					maxY=innerPointB.y;
				}
				
				// How does our bounding box compare to the clipping region?
				if(maxX<minClipX){
					continue;
				}else if(minX>maxClipX){
					continue;
				}
				
				if(maxY<minClipY){
					continue;
				}else if(minY>maxClipY){
					continue;
				}
				
				// Line segment A->B on the "inner" arc.
				
				// Set the UV to that of the solid block colour pixel:
				block.SetSolidColourUV();
				
				// Get the border colour:
				if(i==halfway){
					// Get the next colour:
					
					if(Border.Colour!=null && Border.Colour.Length!=1){
						colour=Border.Colour[ToIndex];
					}
				}
				
				// Set the border colour:
				block.SetColour(colour);
				
				// Apply the block region:
				block.VertexTopLeft=renderer.PixelToWorldUnit(cornerX+outerPointA.x,cornerY+outerPointA.y,zIndex);
				block.VertexTopRight=renderer.PixelToWorldUnit(cornerX+outerPointB.x,cornerY+outerPointB.y,zIndex); 
				
				block.VertexBottomLeft=renderer.PixelToWorldUnit(cornerX+innerPointA.x,cornerY+innerPointA.y,zIndex);
				block.VertexBottomRight=renderer.PixelToWorldUnit(cornerX+innerPointB.x,cornerY+innerPointB.y,zIndex);
				
			}
			
			
		}
		
	}
	
}