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
using System.Collections;
using System.Collections.Generic;
using Blaze;


namespace PowerUI{
	
	/// <summary>
	/// A mesh made up of a dynamic number of "blocks".
	/// Each block always consists of two triangles and 4 vertices.
	/// This is used for displaying the content in 3D with a single mesh.
	/// </summary>
	
	public class DynamicMesh{
	
		/// <summary>The Hybrid GUI Shader. Set on demand.</summary>
		public static Shader UIShader;
	
		/// <summary>The batch this mesh belongs to.</summary>
		public UIBatch Batch;
		/// <summary>The number of blocks that have been allocated.</summary>
		public int BlockCount;
		/// <summary>The unity mesh that this will be flushed into.</summary>
		public Mesh OutputMesh;
		/// <summary>The material to use when rendering.</summary>
		public Material Material;
		/// <summary>The number of blocks that were allocated last UI update.</summary>
		private int LastBlockCount;
		/// <summary>The tail of the linked list of all allocated blocks.</summary>
		public MeshBlock LastBlock;
		/// <summary>The head of the linked list of all allocated blocks.</summary>
		public MeshBlock FirstBlock;
		/// <summary>The renderer being used to layout the blocks.</summary>
		private MeshRenderer Renderer;
		/// <summary>The material used by this mesh when it is using an atlas.</summary>
		private Material GlobalMaterial;
		/// <summary>The transform of this mesh.</summary>
		public Transform OutputTransform;
		/// <summary>The gameobjec that holds this mesh.</summary>
		public GameObject OutputGameObject;
		/// <summary>An array of uv coordinates.</summary>
		public FixedSizeBuffer<Vector2> UV;
		/// <summary>An array of uv coordinates.</summary>
		public FixedSizeBuffer<Vector2> UV2;
		/// <summary>An array of triangles.</summary>
		public FixedSizeBuffer<int> Triangles;
		/// <summary>An array of vertex colours.</summary>
		public FixedSizeBuffer<Color> Colours;
		/// <summary>An array of normals.</summary>
		public FixedSizeBuffer<Vector3> Normals;
		/// <summary>An array of vertex coordinates.</summary>
		public FixedSizeBuffer<Vector3> Vertices;
		/// <summary>An array of tangent values.</summary>
		public FixedSizeBuffer<Vector4> Tangents;
		/// <summary>The collider that receives clicks in physics mode.</summary>
		public MeshCollider FullPhysicsModeCollider;
		
		
		/// <summary>Creates a new dynamic mesh which uses the hybrid shader and a texture atlas.</summary>
		public DynamicMesh(UIBatch batch){
			Batch=batch;
			
			if(UIShader==null){
				UIShader=Shader.Find("StandardUI Unlit");
			}
			
			GlobalMaterial=new Material(UIShader);
			
			Setup();
		}
		
		/// <summary>Changes the font atlas used by the default material.</summary>
		public void SetFontAtlas(TextureAtlas atlas){
			Texture2D texture;
			
			if(atlas==null){
				texture=null;
			}else{
				texture=atlas.Texture;
			}
			
			Material.SetTexture("_Font",texture);
		}
		
		/// <summary>Sets a default material to this mesh.</summary>
		public void SetGraphicsAtlas(TextureAtlas atlas){
			Texture2D texture;
			
			if(atlas==null){
				texture=null;
			}else{
				texture=atlas.Texture;
			}
			
			Material.SetTexture("_Atlas",texture);
		}
		
		public void SetGlobalMaterial(){
			SetMaterial(GlobalMaterial);
		}
		
		/// <summary>Applies the given material to this mesh.</summary>
		/// <param name="material">The material to apply.</param>
		public void SetMaterial(Material material){
			if(Material==material){
				return;
			}
			
			Material=material;
			
			if(Renderer==null){
				return;
			}
			
			Renderer.sharedMaterial=Material;
		}
		
		/// <summary>Called only by constructors. This creates the actual mesh and the buffers for verts/tris etc.</summary>
		private void Setup(){
			UV=new FixedSizeBuffer<Vector2>(4,false);
			Triangles=new FixedSizeBuffer<int>(6,true);
			Colours=new FixedSizeBuffer<Color>(4,false);
			Vertices=new FixedSizeBuffer<Vector3>(4,false);
			UV2=new FixedSizeBuffer<Vector2>(4,false);
			
			OutputMesh=new Mesh();
			OutputGameObject=new GameObject();
			OutputTransform=OutputGameObject.GetComponent<Transform>();	
			
			Renderer=OutputGameObject.AddComponent<MeshRenderer>();
			Renderer.sharedMaterial=Material;
			
			MeshFilter filter=OutputGameObject.AddComponent<MeshFilter>();
			filter.mesh=OutputMesh;
		}
		
		/// <summary>Let the mesh know that normals are required.</summary>
		public void RequireNormals(){
			
			Normals=new FixedSizeBuffer<Vector3>(4,false);
			
		}
		
		/// <summary>Let the mesh know that tangents are required.</summary>
		public void RequireTangents(){
			
			Tangents=new FixedSizeBuffer<Vector4>(4,false);
			
		}
		
		/// <summary>Called to update the parenting of this mesh.</summary>
		public void ChangeParent(){
		
			if(Batch!=null && Batch.Renderer!=null && Batch.Renderer.Node!=null){
				OutputTransform.parent=Batch.Renderer.Node.transform;
			}else if(UI.GUINode!=null){
				OutputTransform.parent=UI.GUINode.transform;
			}
			
			// Make sure the object is correctly transformed:
			OutputTransform.localScale=Vector3.one;
			OutputTransform.localPosition=Vector3.zero;
			OutputTransform.localRotation=Quaternion.identity;
			
		}
		
		/// <summary>Called to tell this mesh to update which input mode its working in.
		/// E.g. this may revert this mesh from screen mode to physics mode, or vice versa.</summary>
		public void SetPhysicsMode(bool physics){
			
			if(physics){
				if(FullPhysicsModeCollider==null){
					FullPhysicsModeCollider=OutputGameObject.AddComponent<MeshCollider>();
					FullPhysicsModeCollider.sharedMesh=OutputMesh;
					OutputGameObject.name="PowerUI-CMesh";
				}
			}else if(FullPhysicsModeCollider!=null){
				GameObject.Destroy(FullPhysicsModeCollider);
				FullPhysicsModeCollider=null;
			}
			
		}
		
		/// <summary>Let the mesh know it's about to undergo a layout routine.
		/// <see cref="PowerUI.Renderman.Layout"/>.</summary>
		public void PrepareForLayout(){
			FirstBlock=LastBlock=null;
			LastBlockCount=BlockCount;
			BlockCount=0;
		}
		
		/// <summary>Let the mesh know a layout routine was completed.
		/// <see cref="PowerUI.Renderman.Layout"/>.</summary>
		public void CompletedLayout(){
			if(BlockCount!=LastBlockCount){
				// We gained or lost some blocks - resize our buffers:
				UV.Resize(BlockCount);
				UV2.Resize(BlockCount);
				
				if(Normals!=null){
					Normals.Resize(BlockCount);
				}
				
				if(Tangents!=null){
					Tangents.Resize(BlockCount);
				}
				
				Colours.Resize(BlockCount);
				Vertices.Resize(BlockCount);
				Triangles.Resize(BlockCount);
			}
		
			// Allocate the new block indices:
			int vertexIndex=0;
			int triangleIndex=0;
			
			MeshBlock currentBlock=FirstBlock;
			while(currentBlock!=null){
				currentBlock.VertexIndex=vertexIndex;
				// Write the triangles:
				// First triangle - Top left corner:
				Triangles.Buffer[triangleIndex++]=vertexIndex;
				// Top right corner:
				Triangles.Buffer[triangleIndex++]=vertexIndex+1;
				// Bottom left corner:
				Triangles.Buffer[triangleIndex++]=vertexIndex+2;
				// Second triangle - Top right corner:
				Triangles.Buffer[triangleIndex++]=vertexIndex+1;
				// Bottom right corner:
				Triangles.Buffer[triangleIndex++]=vertexIndex+3;
				// Bottom left corner:
				Triangles.Buffer[triangleIndex++]=vertexIndex+2;
				
				// Write the block:
				currentBlock.Layout();
				currentBlock.Paint();
				
				vertexIndex+=4;
				currentBlock=currentBlock.BlockAfter;
			}
			
			// Output the new mesh data:
			Flush();
			
		}
		
		/// <summary>Allocates a block from this mesh. The block can then have
		/// its vertices/triangles edited. Changes will be outputted visually when <see cref="PowerUI.DynamicMesh.Flush"/> is called.</summary>
		public MeshBlock Allocate(){
			MeshBlock result=new MeshBlock(this);
			result.AddToParent();
			return result;
		}
		
		/// <summary>Outputs all the verts/triangles etc to the underlying unity mesh.</summary>
		public void Flush(){
			// Strip old triangles:
			OutputMesh.triangles=null;
			
			// Apply the vertices:
			OutputMesh.vertices=Vertices.Buffer;
			OutputMesh.colors=Colours.Buffer;
			OutputMesh.uv=UV.Buffer;
			OutputMesh.uv2=UV2.Buffer;
			
			if(Normals!=null){
				OutputMesh.normals=Normals.Buffer;
			}
			
			if(Tangents!=null){
				OutputMesh.tangents=Tangents.Buffer;
			}
			
			//And apply the triangles:
			OutputMesh.triangles=Triangles.Buffer;
			OutputMesh.RecalculateBounds();
		}
		
		/// <summary>Permanently destroys this mesh.</summary>
		public void Destroy(){
			OutputMesh=null;
			OutputTransform=null;
			
			if(OutputGameObject!=null){
				GameObject.Destroy(OutputGameObject);
				OutputGameObject=null;
			}
			
			UV=null;
			UV2=null;
			Colours=null;
			Normals=null;
			Vertices=null;
			Triangles=null;
			Tangents=null;
			
		}
		
	}
	
}