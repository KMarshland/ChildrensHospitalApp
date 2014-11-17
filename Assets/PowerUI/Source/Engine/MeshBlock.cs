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

namespace PowerUI{
	
	/// <summary>
	/// A block of two triangles in a dynamic mesh.
	/// The triangles are positioned to create a flat 2D rectangle.
	/// The colour and position can be adjusted to fit content onto the block.
	/// </summary>
	
	public class MeshBlock{
	
		/// <summary>The colour of the whole block. Applied to vertex colours.</summary>
		public Color Colour;
		/// <summary>The uv coordinate for the top left corner.</summary>
		public Vector2 UVTopLeft;
		/// <summary>The uv coordinate for the top right corner.</summary>
		public Vector2 UVTopRight;
		/// <summary>The uv coordinate for the bottom left corner.</summary>
		public Vector2 UVBottomLeft;
		/// <summary>The uv coordinate for the bottom right corner.</summary>
		public Vector2 UVBottomRight;
		/// <summary>The vertex in the top left corner.</summary>
		public Vector3 VertexTopLeft;
		/// <summary>The vertex in the top right corner.</summary>
		public Vector3 VertexTopRight;
		/// <summary>The vertex in the bottom left corner.</summary>
		public Vector3 VertexBottomLeft;
		/// <summary>The vertex in the bottom right corner.</summary>
		public Vector3 VertexBottomRight;
		/// <summary>The index of the first vertex in the meshes vertex array.</summary>
		public int VertexIndex;
		/// <summary>Blocks are stored as a linked list. This is the block following this one.</summary>
		public MeshBlock BlockAfter;
		/// <summary>The dynamic mesh that this block was Allocated from.</summary>
		public DynamicMesh ParentMesh;
		/// <summary>The transform to apply to this blocks vertices as a post process.</summary>
		public Transformation Transform;
		/// <summary>The block following this one in a display property linked list.</summary>
		public MeshBlock LocalBlockAfter;
		
		/// <summary>Creates a new block that belongs to a given mesh.</summary>
		/// <param name="parentMesh">The mesh the block will belong to.</param>
		public MeshBlock(DynamicMesh parentMesh){
			ParentMesh=parentMesh;
			Colour=Color.white;
		}
		
		/// <summary>Adds this block to the parent meshes linked list.
		/// Call this during a reflow to reuse this same block again.</summary>
		public void AddToParent(){
			BlockAfter=null;
			ParentMesh.BlockCount++;
			// Push it into the block chain:
			if(ParentMesh.FirstBlock==null){
				ParentMesh.FirstBlock=ParentMesh.LastBlock=this;	
			}else{
				ParentMesh.LastBlock=ParentMesh.LastBlock.BlockAfter=this;
			}
		}
		
		/// <summary>Sets the vertex colours of this block.</summary>
		/// <param name="colour">The colour to set them to.</param>
		public void SetColour(Color colour){
			Colour=colour;
		}
		
		/// <summary>Writes the vertices to the meshes buffer. Called during a layout event.</summary>
		public void Layout(){
			// Apply the vertices:
			if(Transform!=null){
				// Top Left:
				ParentMesh.Vertices.Buffer[VertexIndex]=Transform.Apply(VertexTopLeft);
				// Top Right:
				ParentMesh.Vertices.Buffer[VertexIndex+1]=Transform.Apply(VertexTopRight);
				// Bottom Left:
				ParentMesh.Vertices.Buffer[VertexIndex+2]=Transform.Apply(VertexBottomLeft);
				// Bottom Right:
				ParentMesh.Vertices.Buffer[VertexIndex+3]=Transform.Apply(VertexBottomRight);
			}else{
				// Top Left:
				ParentMesh.Vertices.Buffer[VertexIndex]=VertexTopLeft;
				// Top Right:
				ParentMesh.Vertices.Buffer[VertexIndex+1]=VertexTopRight;
				// Bottom Left:
				ParentMesh.Vertices.Buffer[VertexIndex+2]=VertexBottomLeft;
				// Bottom Right:
				ParentMesh.Vertices.Buffer[VertexIndex+3]=VertexBottomRight;
			}
		}
		
		/// <summary>Sets the vertices of this box to that specified by the given block
		/// but clipped to fit within a boundary.</summary>
		/// <param name="boundary">The clipping boundary. The vertices will be clipped to within this.</param>
		/// <param name="block">The position of the vertices.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		public void SetClipped(BoxRegion boundary,BoxRegion block,Renderman renderer,float zIndex){
			// Clipping with no image/ affect on UVs:
			block.ClipBy(boundary);
			// And just apply the result:
			ApplyVertices(block,renderer,zIndex);
		}
		
		/// <summary>Sets the vertices of this box to that specified by the given block
		/// but clipped to fit within a boundary. At the same time, an image is applied
		/// to the block and its UV coordinates are also clipped.</summary>
		/// <param name="boundary">The clipping boundary. The vertices will be clipped to within this.</param>
		/// <param name="block">The position of the vertices.</param>
		/// <param name="renderer">The renderer that will render this block.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		/// <param name="imgLocation">The location of the image on the meshes atlas.</param>
		public void SetClipped(BoxRegion boundary,BoxRegion block,Renderman renderer,float zIndex,AtlasLocation imgLocation){
			
			// Image defines how big we want the image to be in pixels on the screen.
			// So firstly we need to find the ratio of how scaled our image actually is:
			float originalHeight=block.Height;
			float scaleX=imgLocation.Width/block.Width;
			float scaleY=imgLocation.Height/originalHeight;
			
			// We'll need to clip block and make sure the image block is clipped too:
			float blockX=block.X;
			float blockY=block.Y;
			
			block.ClipBy(boundary);
			
			// Apply the verts:
			ApplyVertices(block,renderer,zIndex);
			
			block.X-=blockX;
			block.Y-=blockY;
			block.MaxX-=blockX;
			block.MaxY-=blockY;
			
			// Flip the gaps (the clipped and now 'missing' sections) - UV's are inverted relative to the vertices.
			
			// Bottom gap is just block.Y:
			float bottomGap=block.Y;
			
			// Top gap is the original height - the new maximum; write it to the bottom gap:
			block.Y=originalHeight-block.MaxY;
			
			// Update the top gap:
			block.MaxY=originalHeight-bottomGap;
			
			// Image was in terms of real screen pixels, so now we need to scale it to being in 'actual image' pixels.
			// From there, the region 
			block.X*=scaleX;
			block.MaxX*=scaleX;
			
			block.Y*=scaleY;
			block.MaxY*=scaleY;
			
			// We have an image. Set UV's too:
			UVTopLeft=new Vector2(imgLocation.GetU(block.X+0.2f),imgLocation.GetV(block.MaxY-0.2f));
			UVTopRight=new Vector2(imgLocation.GetU(block.MaxX-0.2f),imgLocation.GetV(block.MaxY-0.2f));
			UVBottomLeft=new Vector2(imgLocation.GetU(block.X+0.2f),imgLocation.GetV(block.Y+0.2f));
			UVBottomRight=new Vector2(imgLocation.GetU(block.MaxX-0.2f),imgLocation.GetV(block.Y+0.2f));
			
		}
		
		/// <summary>This locates the vertices of this block in world space to the position defined by the given box.</summary>
		/// <param name="block">The position of the vertices in screen coordinates.</param>
		/// <param name="renderer">The renderer used when rendering this block.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		private void ApplyVertices(BoxRegion block,Renderman renderer,float zIndex){
			VertexTopLeft=renderer.PixelToWorldUnit(block.X,block.Y,zIndex);
			VertexTopRight=renderer.PixelToWorldUnit(block.MaxX,block.Y,zIndex); 
			VertexBottomLeft=renderer.PixelToWorldUnit(block.X,block.MaxY,zIndex);
			VertexBottomRight=renderer.PixelToWorldUnit(block.MaxX,block.MaxY,zIndex);
		}
		
		/// <summary>Sets the UV and image on this block to that of the solid colour pixel.</summary>
		public void SetSolidColourUV(){
			// Set the UVs - solid colour is always at y>1:
			UVTopRight=UVBottomLeft=UVBottomRight=UVTopLeft=new Vector2(0f,2f);
		}
		
		/// <summary>Writes out the UV and vertex colours to the meshes buffers.</summary>
		public void Paint(){
			// The UV:
			// Top Left:
			ParentMesh.UV.Buffer[VertexIndex]=UVTopLeft;
			// Top Right:
			ParentMesh.UV.Buffer[VertexIndex+1]=UVTopRight;
			// Bottom Left:
			ParentMesh.UV.Buffer[VertexIndex+2]=UVBottomLeft;
			// Bottom Right:
			ParentMesh.UV.Buffer[VertexIndex+3]=UVBottomRight;
			
			// Apply the colour:
			for(int i=0;i<4;i++){
				ParentMesh.Colours.Buffer[VertexIndex+i]=Colour;
			}
		}
		
	}
	
}