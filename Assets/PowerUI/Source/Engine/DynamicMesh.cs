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
		/// <summary>The texture atlas to use on the material.</summary>
		public TextureAtlas Atlas;
		/// <summary>The number of blocks that were allocated last UI update.</summary>
		private int LastBlockCount;
		/// <summary>The tail of the linked list of all allocated blocks.</summary>
		public MeshBlock LastBlock;
		/// <summary>The head of the linked list of all allocated blocks.</summary>
		public MeshBlock FirstBlock;
		/// <summary>The current font texture used by the default UI material.</summary>
		public Texture2D FontTexture;
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
		/// <summary>An array of triangles.</summary>
		public FixedSizeBuffer<int> Triangles;
		/// <summary>An array of vertex colours.</summary>
		public FixedSizeBuffer<Color> Colours;
		/// <summary>An array of vertex coordinates.</summary>
		public FixedSizeBuffer<Vector3> Vertices;
		/// <summary>The collider that receives clicks in physics mode.</summary>
		public MeshCollider FullPhysicsModeCollider;
		
		
		/// <summary>Creates a new dynamic mesh which uses the hybrid shader and a texture atlas.</summary>
		public DynamicMesh(UIBatch batch){
			Batch=batch;
			SetMaterial();
			Setup();
		}
		
		/// <summary>Creates a new dynamic mesh, applying the given atlas to it.</summary>
		/// <param name="atlas">The texture atlas to use.</param>
		public DynamicMesh(UIBatch batch,TextureAtlas atlas){
			Batch=batch;
			Atlas=atlas;
			SetMaterial();
			Setup();
		}
		
		/// <summary>Changes the font texture used by the default material.</summary>
		/// <param name="fontTexture">The new texture to use.</param>
		public void ChangeFontTexture(Texture2D fontTexture){
			if(FontTexture==fontTexture){
				return;
			}
			FontTexture=fontTexture;
			Material.SetTexture("_Font",fontTexture);
		}
		
		/// <summary>Sets the default hybrid (text and graphics) shader and a new atlas.</summary>
		public void SetMaterialAndAtlas(){
			Atlas=new TextureAtlas();
			SetMaterial();
		}
		
		/// <summary>Sets a default material to this mesh. Note that this must only run when an atlas is available.</summary>
		public void SetMaterial(){
			if(UIShader==null){
				UIShader=Shader.Find("Hybrid GUI Shader");
			}
			
			if(GlobalMaterial==null){
				GlobalMaterial=new Material(UIShader);
			}
			
			Material material=GlobalMaterial;
			Material=null;
			SetMaterial(material);
			ApplyAtlas();
		}
		
		/// <summary>Applies a TextureAtlas to the material of this mesh if there is one.</summary>
		public void ApplyAtlas(TextureAtlas atlas){
			Atlas=atlas;
			ApplyAtlas();
		}
		
		/// <summary>Applies the TextureAtlas to the material of this mesh if there is one.</summary>
		public void ApplyAtlas(){
			if(Material==null){
				return;
			}
			if(Atlas!=null){
				Material.SetTexture("_Atlas",Atlas.Texture);
			}else{
				Material.SetTexture("_Atlas",null);
			}
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
			Renderer.material=Material;
		}
		
		/// <summary>Called only by constructors. This creates the actual mesh and the buffers for verts/tris etc.</summary>
		private void Setup(){
			UV=new FixedSizeBuffer<Vector2>(4);
			Triangles=new FixedSizeBuffer<int>(6);
			Colours=new FixedSizeBuffer<Color>(4);
			Vertices=new FixedSizeBuffer<Vector3>(4);
			
			OutputMesh=new Mesh();
			OutputGameObject=new GameObject();
			OutputTransform=OutputGameObject.transform;	
			
			Renderer=OutputGameObject.AddComponent<MeshRenderer>();
			Renderer.material=Material;
			
			MeshFilter filter=OutputGameObject.AddComponent<MeshFilter>();
			filter.mesh=OutputMesh;

			if(Batch!=null && Batch.Renderer!=null && Batch.Renderer.Node!=null){
				OutputGameObject.transform.parent=Batch.Renderer.Node.transform;
			}else if(UI.GUINode!=null){
				OutputGameObject.transform.parent=UI.GUINode.transform;
			}
			
			// Make sure the object is correctly transformed:
			OutputTransform.localScale=Vector3.one;
			OutputTransform.localPosition=Vector3.zero;
			OutputTransform.localRotation=Quaternion.identity;
		}
		
		/// <summary>Called to tell this mesh to update which input mode its working in.
		/// E.g. this may revert this mesh from screen mode to physics mode, or vice versa.</summary>
		public void SetInputMode(InputMode mode){
			if(mode==InputMode.Physics){
				if(FullPhysicsModeCollider==null){
					FullPhysicsModeCollider=OutputGameObject.AddComponent<MeshCollider>();
					FullPhysicsModeCollider.sharedMesh=OutputMesh;
				}
				OutputGameObject.name="PowerUI-CMesh";
			}else if(FullPhysicsModeCollider!=null){
				GameObject.Destroy(FullPhysicsModeCollider);
				FullPhysicsModeCollider=null;
			}
		}
		
		/// <summary>Adds the given texture to the atlas.
		/// Note that the atlas buffers internally and won't be rebuilt every update.</summary>
		/// <param name="texture">The texture to add to the atlas.</param>
		public AtlasLocation AddTexture(Texture2D texture){
			CreateAtlas();
			if(Atlas==null){
				return null;
			}
			AtlasLocation location=Atlas.Add(texture);
			if(location==null){
				return null;
			}
			location.AddedThisUpdate=true;
			return location;
		}
		
		public void CreateAtlas(){
			if(Atlas==null && Batch!=null && !Batch.Isolated){
				Atlas=Batch.Renderer.SharedAtlas;
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
			
			if(Atlas==null){
				return;
			}
			
			// Update the texture atlas by removing the textures that were not 'AddedThisUpdate'.
			AtlasLocation firstToRemove=null;
			bool somethingToRemove=false;
			
			foreach(KeyValuePair<Texture2D,AtlasLocation> kvp in Atlas.ActiveTextures){
				if(kvp.Value.AddedThisUpdate){
					kvp.Value.AddedThisUpdate=false;
					continue;
				}
				// Remove the texture from the atlas, without updating ActiveTextures just yet:
				// If we updated ActiveTextures, we would be modifying it whilst iterating which will error.
				kvp.Value.Deselect();
				if(!somethingToRemove){
					somethingToRemove=true;
					firstToRemove=Atlas.LastEmpty;
				}
			}
			
			if(!somethingToRemove){
				return;
			}
			
			// To prevent the above error, we'll instead look out for new objects added to the end of the atlases empty queue - from firstToRemove onwards.
			
			while(firstToRemove!=null){
				// Foreach newly removed element, remove its texture from the ActiveTextures dictionary.
				// This is much more performant than rebuilding the dictionary as in most cases this will be a small set from a large dictionary.
				Atlas.CanOptimize=true;
				
				Atlas.ActiveTextures.Remove(firstToRemove.Texture);
				firstToRemove=firstToRemove.EmptyAfter;
			}
			
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
			
			//And apply the triangles:
			OutputMesh.triangles=Triangles.Buffer;
			OutputMesh.RecalculateNormals();
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
			Atlas=null;
			Colours=null;
			Vertices=null;
			Triangles=null;
		}
		
	}
	
}