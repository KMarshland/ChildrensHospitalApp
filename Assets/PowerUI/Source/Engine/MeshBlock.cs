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
using Blaze;


namespace PowerUI{
	
	/// <summary>
	/// A block of two triangles in a dynamic mesh.
	/// The triangles are positioned to create a flat 2D rectangle.
	/// The colour and position can be adjusted to fit content onto the block.
	/// </summary>
	
	public class MeshBlock{
		
		private static UVBlock BlankUV=new UVBlock(2f,2f,2f,2f);
		
		/// <summary>The colour of the whole block. Applied to vertex colours.</summary>
		public Color Colour;
		/// <summary>The UV coordinate block for a letter off the atlas.</summary>
		public UVBlock TextUV;
		/// <summary>The UV coordinate for an image off the atlas.</summary>
		public UVBlock ImageUV;
		
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
		
		public bool Overlaps(MeshBlock block){
			
			// Check if any of blocks 4 corners are within this square.
			
			Vector3[] vertBuffer=block.ParentMesh.Vertices.Buffer;
			
			// Apply inverse transform to blocks corners:
			for(int i=0;i<4;i++){
				
				// Map it:
				Vector3 point=Transform.ApplyInverse(vertBuffer[block.VertexIndex+i]);
				
				// Simple box test:
				if(point.x<VertexBottomLeft.x || point.x>VertexBottomRight.x || point.y<VertexBottomLeft.y || point.y>VertexTopLeft.y){
					continue;
				}
				
				return true;
				
			}
			
			return false;
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
		public UVBlock SetClipped(BoxRegion boundary,BoxRegion block,Renderman renderer,float zIndex,AtlasLocation imgLocation,UVBlock uvBlock){
			
			// Image defines how big we want the image to be in pixels on the screen.
			// So firstly we need to find the ratio of how scaled our image actually is:
			float originalHeight=block.Height;
			float scaleX=imgLocation.Width/block.Width;
			float scaleY=imgLocation.Height/originalHeight;
			
			// We'll need to clip block and make sure the image block is clipped too:
			float blockX=block.X;
			float blockY=block.Y;
			
			if(block.ClipByChecked(boundary)){
				
				// It actually got clipped - time to do some UV clipping too.
				
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
				
				if(uvBlock==null || uvBlock.Shared){
					
					// Create the UV block:
					uvBlock=new UVBlock();
					
				}
				
				// Get the new max/min values:
				uvBlock.MinX=imgLocation.GetU(block.X+0.2f);
				uvBlock.MaxX=imgLocation.GetU(block.MaxX-0.2f);
				uvBlock.MaxY=imgLocation.GetV(block.MaxY-0.2f);
				uvBlock.MinY=imgLocation.GetV(block.Y+0.2f);
				
			}else{
				
				// Apply the verts:
				ApplyVertices(block,renderer,zIndex);
				
				// Globally share the UV!
				uvBlock=imgLocation;
				
			}
			
			return uvBlock;
			
		}
		
		/// <summary>Applies the SDF outline "location", essentially the thickness of an outline, to this block.</summary>
		public void ApplyOutline(float location){
			
			// Apply the values to the tangents:
			Vector4[] buffer=ParentMesh.Tangents.Buffer;
			
			Vector4 tangent=new Vector4(location,location,0f,0f);
			
			for(int i=0;i<4;i++){
				buffer[VertexIndex+i]=tangent;
			}
			
		}
		
		/// <summary>This locates the vertices of this block in world space to the position defined by the given box.</summary>
		/// <param name="block">The position of the vertices in screen coordinates.</param>
		/// <param name="renderer">The renderer used when rendering this block.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		private void ApplyVertices(BoxRegion block,Renderman renderer,float zIndex){
		
			// Compute the min/max pixels:
			Vector3 min=renderer.PixelToWorldUnit(block.X,block.Y,zIndex);
			Vector3 max=renderer.PixelToWorldUnit(block.MaxX,block.MaxY,zIndex);
			
			// Get the 4 corners:
			VertexTopLeft=min;
			VertexBottomRight=max;
			VertexTopRight=new Vector3(max.x,min.y,min.z); 
			VertexBottomLeft=new Vector3(min.x,max.y,min.z);
			
		}
		
		/// <summary>Sets the UV and image on this block to that of the solid colour pixel.</summary>
		public void SetSolidColourUV(){
			
			// Set the UVs - solid colour is always at y>1:
			ImageUV=null;
			TextUV=null;
			
		}
		
		/// <summary>Writes out the UV and vertex colours to the meshes buffers.</summary>
		public void Paint(){
			// The UVs:
			
			if(ImageUV!=null){
				ImageUV.Write(ParentMesh.UV.Buffer,VertexIndex);
			}else{
				BlankUV.Write(ParentMesh.UV.Buffer,VertexIndex);
			}
			
			if(TextUV!=null){
				TextUV.Write(ParentMesh.UV2.Buffer,VertexIndex);
			}else{
				BlankUV.Write(ParentMesh.UV2.Buffer,VertexIndex);
			}
			
			// Apply the colour:
			Color[] buffer=ParentMesh.Colours.Buffer;
			
			for(int i=0;i<4;i++){
				buffer[VertexIndex+i]=Colour;
			}
			
		}
		
	}
	
}