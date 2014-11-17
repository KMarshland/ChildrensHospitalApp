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
	/// Represents the border around an element.
	/// </summary>
	
	public class BorderProperty:DisplayableProperty{
		
		/// <summary>The width of the border on the top edge.</summary>
		public int WidthTop;
		/// <summary>The width of the border on the left edge.</summary>
		public int WidthLeft;
		/// <summary>The width of the border on the right edge.</summary>
		public int WidthRight;
		/// <summary>The width of the border on the bottom edge.</summary>
		public int WidthBottom;
		
		/// <summary>The colour the border should be with any colour overlay applied. Black if undefined.</summary>
		public Color[] Colour=null;
		/// <summary>The colour of the border without the colour overlay. Black if undefined.</summary>
		public Css.Value BaseColour=null;
		/// <summary>The set of round corners if any.</summary>
		public RoundedCorners Corners=null;
		
		/// <summary>How the border should appear. Note that only solid is currently used.</summary>
		public BorderStyle Style=BorderStyle.Solid;
		
		
		/// <summary>Creates a new border property for the given element.</summary>
		/// <param name="element">The element to give a border to.</param>
		public BorderProperty(Element element):base(element){}
		
		
		public void ResetColour(){
			
			if(BaseColour==null){
				// Default colour (font colour or black if there's no text).
				Colour=null;
			}else{
				
				// Are they all the same?
				if(BaseColour.AllSameValues()){
					// Yep! Not a multicolour border anyway.
					
					// Create the single colour set:
					Colour=new Color[1];
					
				}else{
				
					// Create the colour set:
					Colour=new Color[4];
					
				}
				
			}
			
			// Note that SetOverlayColour always runs immediately after this.
			
		}
		
		public override void SetOverlayColour(Color colour){
			
			if(Colour!=null){
				
				// For each side..
				for(int i=0;i<Colour.Length;i++){
					// Bake the colour:
					Colour[i]=BaseColour.GetColor(i)*colour;
				}
				
			}
			
			RequestPaint();
		}
		
		public override void Paint(){
			
			// Get the computed style:
			ComputedStyle computed=Element.Style.Computed;
			
			// Any meshes in my queue should now change colour:
			MeshBlock block=FirstBlock;
			
			if( Colour==null || Colour.Length==1 ){
				
				// Most common case. This is a single colour border.
				
				// Get the default colour - that's the same as the text colour:
				Color colour=Color.black;
				
				// Does this border have a colour?
				if(Colour==null){
					
					// Grab the text colour if there is one:
					if(computed.Text!=null){
						
						// It's the same as the font colour:
						colour=computed.Text.FontColour;
						
					}else{
					
						// Nope - We need to set alpha:
						colour.a=computed.ColorOverlay.a;
						
					}
					
				}else{
					colour=Colour[0];
				}
				
				// For each block, set the colour:
				while(block!=null){
					block.SetColour(colour);
					block.Paint();
					
					block=block.LocalBlockAfter;
				}
				
				return;
			}
			
		}
		
		protected override void Layout(){
			
			if(Corners!=null){
				Corners.PreLayout();
			}
			
			ComputedStyle computed=Element.Style.Computed;
			
			// Find the zIndex:
			// NB: At same depth as BGColour - right at the back.
			float zIndex=(computed.ZIndex-0.006f);
			
			// Get the co-ord of the top edge:
			int top=computed.OffsetTop;
			int left=computed.OffsetLeft;
			
			// And the dimensions of the lines:
			// Note: boxwidth doesn't include the left/right widths to prevent overlapping.
			int boxWidth=computed.PaddedWidth;
			int boxHeight=computed.PaddedHeight+WidthTop+WidthBottom;
			
			BoxRegion screenRegion=new BoxRegion();
			Renderman renderer=Element.Document.Renderer;
			
			// Get the default colour - that's the same as the text colour:
			Color colour=Color.black;
			
			// Is the border multicoloured?
			bool multiColour=false;
			
			// Does this border have a colour?
			if(Colour==null){
				
				// Grab the text colour if there is one:
				if(computed.Text!=null){
					
					// It's the same as the font colour:
					colour=computed.Text.FontColour;
					
				}else{
				
					// Nope - We need to set alpha:
					colour.a=computed.ColorOverlay.a;
					
				}
				
			}else if(Colour.Length==1){
				colour=Colour[0];
			}else{
				multiColour=true;
			}
			
			for(int i=0;i<4;i++){
				int lineHeight=0;
				int lineWidth=0;
				
				// Co-ords of the top-left corner for our box:
				int cornerY=top;
				int cornerX=left;
				
				if(i==0 || i==2){
					// Top or bottom:
					lineWidth=boxWidth;
					lineHeight=BorderWidth(i);
				}else{
					lineWidth=BorderWidth(i);
					lineHeight=boxHeight;
				}
				
				// Does this border have multiple colours?
				if(multiColour){
					colour=Colour[i];
				}
				
				if(Corners!=null){
					Corners.Layout(i,ref cornerX,ref cornerY,ref lineWidth,ref lineHeight);
				}else{
					switch(i){
						case 0:
							// Top:
							cornerX+=WidthLeft;
							
						break;
						case 1:
							// Right:
							cornerX+=boxWidth+WidthLeft;
							
						break;
						case 2:
							// Bottom:
							cornerY+=boxHeight-WidthBottom;
							cornerX+=WidthLeft;
							
						break;
					}
				}
				
				screenRegion.Set(cornerX,cornerY,lineWidth,lineHeight);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					
					// This region is visible. Clip it:
					screenRegion.ClipBy(renderer.ClippingBoundary);
					
					// And get our block ready:
					MeshBlock block=Add();
					
					// Set the UV to that of the solid block colour pixel:
					block.SetSolidColourUV();
					
					// Set the border colour:
					block.SetColour(colour);
					
					block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,zIndex);
				}
				
			}
		}
		
		/// <summary>Transforms all the blocks that this property has allocated. Note that transformations are a post process.
		/// Special case for borders as it may also internally transform its corners.</summary>
		/// <param name="topTransform">The transform that should be applied to this property.</param>
		public void BorderTransform(Transformation topTransform){
			
			Transform(topTransform);
			
			if(Corners!=null){
				Corners.Transform(topTransform);
			}
			
		}
		
		/// <summary>Applies any transforms (rotate,scale etc) now. Note that tranforms are post-processes
		/// so they are very fast and mostly done by paint events. Special case for borders as it may also transform corners.</summary>
		public void ApplyBorderTransform(){
			
			ApplyTransform();
			
			if(Corners!=null){
				Corners.ApplyTransform();
			}
			
		}
		
		public void SetCorner(RoundCornerPosition position,int radius){
			
			if(Corners==null){
				
				if(radius<=0){
					return;
				}
			
				// Create the corner set:
				Corners=new RoundedCorners(this);
				
			}
			
			// Set the corner:
			switch(position){
				case RoundCornerPosition.TopLeft:
					// Top left corner:
					Corners.SetCorner(ref Corners.TopLeft,position,radius);
				break;
				case RoundCornerPosition.TopRight:
					// Top right corner:
					Corners.SetCorner(ref Corners.TopRight,position,radius);
				break;
				case RoundCornerPosition.BottomRight:
					// Bottom right corner:
					Corners.SetCorner(ref Corners.BottomRight,position,radius);
				break;
				case RoundCornerPosition.BottomLeft:
					// Bottom left corner:
					Corners.SetCorner(ref Corners.BottomLeft,position,radius);
				break;
			}
			
		}
		
		public void RenderCorners(){
			
			if(Corners!=null){
				Corners.RenderCorners();
			}
			
		}
		
		/// <summary>Gets the width of the numbered border.</summary>
		/// <param name="i">The border index. Goes around clockwise: 0=Top, 1=Right, 2=Bottom, 3=Left.</param>
		/// <returns>The width of the border on the given side. Returns the left border if the number is out of range.</returns>
		public int BorderWidth(int i){
			
			switch(i){
				case 0:
					return WidthTop;
				case 1:
					return WidthRight;
				case 2:
					return WidthBottom;
			}
			
			return WidthLeft;
		}
		
	}
	
}