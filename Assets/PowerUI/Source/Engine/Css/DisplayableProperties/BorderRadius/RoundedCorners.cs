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
	/// Represents the set of round corners for a particular border.
	/// </summary>
	
	public class RoundedCorners{
		
		/// <summary>The renderer which will render these corners.</summary>
		public Renderman Renderer;
		/// <summary>A rounded corner in the top left, if any.</summary>
		public RoundCorner TopLeft;
		/// <summary>A rounded corner in the top right, if any.</summary>
		public RoundCorner TopRight;
		/// <summary>The parent border this set belongs to.</summary>
		public BorderProperty Border;
		/// <summary>A rounded corner in the bottom left, if any.</summary>
		public RoundCorner BottomLeft;
		/// <summary>The computed style of the parent element.</summary>
		public ComputedStyle Computed;
		/// <summary>A rounded corner in the bottom right, if any.</summary>
		public RoundCorner BottomRight;
		/// <summary>The inverse fragment renderer for this set.</summary>
		public RoundBorderInverseProperty InverseBorder;
		
		public RoundedCorners(BorderProperty border){
			Border=border;
			
			// Grab the renderer:
			Renderer=Border.Element.Document.Renderer;
			
			// Grab the computed style:
			Computed=border.Element.style.Computed;
			
			// Create the inverse border set:
			InverseBorder=new RoundBorderInverseProperty(border.Element);
		}
		
		/// <summary>Sets a round corner to this border.</summary>
		/// <param name="corner">The property that it's going onto or coming from.</param>
		/// <param name="position">The type of corner that it is.</param>
		/// <param name="radius">The border radius.</param>
		public void SetCorner(ref RoundCorner corner,RoundCornerPosition position,int radius){
			
			if(radius<=0){
				
				if(corner!=null){
					// Clear it:
					corner=null;
					
					// Got rounded corners now?
					bool hasRoundedCorners=(
						TopLeft!=null || TopRight!=null ||
						BottomLeft!=null || BottomRight!=null
					);
					
					if(!hasRoundedCorners){
						// Clear the corner set:
						Border.Corners=null;
					}
					
				}
				
				return;
			}
			
			// A corner is now required:
			
			if(corner==null){
				
				// Create it now:
				corner=new RoundCorner(this,position);
				
			}
			
			// Apply the radius:
			corner.Radius=radius;
			
		}
		
		/// <summary>Transforms all the blocks that this property has allocated. Note that transformations are a post process.
		/// Special case for borders as it may also internally transform its corners.</summary>
		/// <param name="topTransform">The transform that should be applied to this property.</param>
		public void Transform(Transformation topTransform){
			
			InverseBorder.Transform(topTransform);
			
		}
		
		/// <summary>Applies any transforms (rotate,scale etc) now. Note that tranforms are post-processes
		/// so they are very fast and mostly done by paint events.</summary>
		public void ApplyTransform(){
			
			InverseBorder.ApplyTransform();
			
		}
		
		/// <summary>Called right before round corners are about to be layed out.</summary>
		public void PreLayout(){
			
			// Clear the blocks of the inverse border:
			InverseBorder.ClearBlocks();
			
		}
		
		/// <summary>Renders and performs a layout of the round corners.</summary>
		public void Layout(int i,ref int cornerX,ref int cornerY,ref int lineWidth,ref int lineHeight){
			
			// Get the co-ord of the top edge:
			int top=Computed.OffsetTop;
			int left=Computed.OffsetLeft;
			
			// And the dimensions of the lines:
			// Note: boxwidth doesn't include the left/right widths to prevent overlapping.
			int boxWidth=Computed.PaddedWidth;
			int boxHeight=Computed.PaddedHeight+Border.WidthTop+Border.WidthBottom;
			
			
			switch(i){
				case 0:
					// Top:
					
					// Move over by the top-left corner:
					if(TopLeft!=null){
						
						// Render the top left corner:
						TopLeft.RenderInverse((float)left,(float)top);
						
						// We have a corner in the top left.
						cornerX+=TopLeft.CornerRadius;
						lineWidth+=Border.WidthLeft-TopLeft.CornerRadius;
						
						
					}else{
						cornerX+=Border.WidthLeft;
					}
					
					// And shrink by the top-right corner:
					if(TopRight!=null){
						// We have a corner in the top right.
						lineWidth+=Border.WidthRight-TopRight.CornerRadius;
					}
					
				break;
				case 1:
					// Right:
					cornerX+=boxWidth+Border.WidthLeft;
					
					if(TopRight!=null){
						
						// Render the top right corners inverse now:
						TopRight.RenderInverse((float)(left+boxWidth+Border.WidthLeft+Border.WidthRight),(float)top);
						
						// We have a corner in the top right.
						cornerY+=TopRight.CornerRadius;
						lineHeight-=TopRight.CornerRadius;
					}
					
					if(BottomRight!=null){
						// We have a corner in the top right.
						lineHeight-=BottomRight.CornerRadius;
					}
					
				break;
				case 2:
					// Bottom:
					cornerY+=boxHeight-Border.WidthBottom;
					
					if(BottomLeft!=null){
						
						// We have a corner in the bottom left.
						cornerX+=BottomLeft.CornerRadius;
						lineWidth+=Border.WidthLeft-BottomLeft.CornerRadius;
					}else{
						
						cornerX+=Border.WidthLeft;
						
					}
					
					if(BottomRight!=null){
						
						// Render the bottom right corners inverse now:
						BottomRight.RenderInverse((float)(left+boxWidth+Border.WidthLeft+Border.WidthRight),(float)(top+boxHeight));
						
						// We have a corner in the top right.
						lineWidth+=Border.WidthRight-BottomRight.CornerRadius;
					}
					
				break;
				case 3:
					// Left:
					// (No change to the point itself).
					
					if(TopLeft!=null){
						// We have a corner in the top left.
						cornerY+=TopLeft.CornerRadius;
						lineHeight-=TopLeft.CornerRadius;
					}
					
					if(BottomLeft!=null){
						
						// Render the bottom left corners inverse now:
						BottomLeft.RenderInverse((float)left,(float)(top+boxHeight));
						
						// We have a corner in the bottom left.
						lineHeight-=BottomLeft.CornerRadius;
					}
					
				break;
			}
			
		}
		
		public void Recompute(){
			
			if(TopLeft!=null){
				
				// Got a top left corner - recompute it's inner arc:
				TopLeft.RecomputeInnerArc();
				
			}
			
			if(TopRight!=null){
				
				// Got a top left corner - render it now:
				TopRight.RecomputeInnerArc();
				
			}
			
			if(BottomRight!=null){
				
				// Got a top left corner - render it now:
				BottomRight.RecomputeInnerArc();
				
			}
			
			if(BottomLeft!=null){
				
				// Got a top left corner - render it now:
				BottomLeft.RecomputeInnerArc();
				
			}
			
		}
		
		/// <summary>Renders round corners.</summary>
		public void RenderCorners(){
			
			// Get the co-ord of the top edge:
			float top=(float)Computed.OffsetTop;
			float left=(float)Computed.OffsetLeft;
			
			// And the dimensions of the lines:
			float boxWidth=(float)(Computed.PaddedWidth+Border.WidthLeft+Border.WidthRight);
			float boxHeight=(float)(Computed.PaddedHeight+Border.WidthTop+Border.WidthBottom);
			
			// Grab the colour overlay alpha:
			float alpha=Computed.ColorOverlay.a;
			
			if(TopLeft!=null){
				
				// Got a top left corner - render it now:
				TopLeft.Render(alpha,left,top);
				
			}
			
			if(TopRight!=null){
				
				// Got a top left corner - render it now:
				TopRight.Render(alpha,left+boxWidth,top);
				
			}
			
			if(BottomRight!=null){
				
				// Got a top left corner - render it now:
				BottomRight.Render(alpha,left+boxWidth,top+boxHeight);
				
			}
			
			if(BottomLeft!=null){
				
				// Got a top left corner - render it now:
				BottomLeft.Render(alpha,left,top+boxHeight);
				
			}
		}
		
		
	}
	
}