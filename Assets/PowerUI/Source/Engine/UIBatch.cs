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
	/// A UI Batch represents a block of the UI that can be safely rendered
	/// with a single drawcall (i.e. all in one mesh). Batches can be isolated - that
	/// means it can have its own material or texture atlas. Batches are created on
	/// demand by the renderer and based on the requests of the elements being rendered.
	/// An animation, for example, will generate an isolated batch.
	/// </summary>
	
	public class UIBatch{
		
		/// <summary>True if this batch is an isolated one; i.e. one with a custom material or atlas.</summary>
		public bool Isolated;
		/// <summary>The mesh for this batch.</summary>
		public DynamicMesh Mesh;
		/// <summary>True if PrepareForLayout was called.</summary>
		public bool PrepareCalled;
		/// <summary>The renderer that will render this batch.</summary>
		public Renderman Renderer;
		/// <summary>Batches are stored as a linked list. This is the batch after this one.</summary>
		public UIBatch BatchAfter;
		/// <summary>Batches are stored as a linked list. This is the batch before this one.</summary>
		public UIBatch BatchBefore;
		/// <summary>If isolated, this is the property that created the isolated batch.</summary>
		public DisplayableProperty IsolatedProperty;
		
		
		/// <summary>Creates a new UI Batch which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer that will render this batch.</param>
		public UIBatch(Renderman renderer){
			Renderer=renderer;
			
			Mesh=new DynamicMesh(this);
			
			RenderWithCamera(renderer.RenderLayer);
			
			if(Renderer.RenderingInWorld){
				SetInputMode(PowerUI.Input.WorldInputMode);
			}else{
				SetInputMode(PowerUI.Input.Mode);
			}
		}
		
		/// <summary>Creates a new UI Batch which will be rendered with the given renderer and isolation.</summary>
		/// <param name="renderer">The renderer that will render this batch.</param>
		/// <param name="isolation">The property that wants this batch to be isolated so it can have its own material/atlas.</param>
		public UIBatch(Renderman renderer,DisplayableProperty isolation):this(renderer){
			SetIsolation(isolation);
		}
		
		/// <summary>Called when the atlas has been created.</summary>
		/// <param name="atlas">The atlas that was created.</param>
		public void AtlasCreated(TextureAtlas atlas){
			if(Isolated){
				return;
			}
			Mesh.ApplyAtlas(Renderer.SharedAtlas);
		}
		
		/// <summary>Sets this batch to be isolated if the given property wants it to be.</summary>
		/// <param name="isolation">The property whose isolation settings will be copied.</param>
		public void SetIsolation(DisplayableProperty isolation){
			bool wasIsolated=Isolated;
			Isolated=(isolation!=null);
			IsolatedProperty=isolation;
			
			if(!Isolated){
				if(wasIsolated){
					// Change the material.
					Mesh.SetMaterial();
				}
				Mesh.ApplyAtlas(Renderer.SharedAtlas);
			}
		}
		
		/// <summary>Called when the physics input mode changes.</summary>
		/// <param name="mode">The new input mode to use.</param>
		public void SetInputMode(InputMode mode){
			Mesh.SetInputMode(mode);
		}
		
		public void SetRenderDepth(int index){
			// Transparent + offset.
			Mesh.Material.renderQueue=3000+index;
		}
		
		/// <summary>Flushes this batches mesh. <see cref="PowerUI.DynamicMesh.Flush"/>.</summary>
		public void Flush(){
			Mesh.Flush();
		}
		
		/// <summary>Optimises this batches atlas, if it has one.</summary>
		public void OptimizeAtlas(){
			if(Mesh.Atlas==null){
				return;
			}
			
			if(Mesh.Atlas.OptimizeRequested){
				// Optimise it now:
				Mesh.Atlas.Optimize();
			}
		}
		
		/// <summary>Flushes this batches atlas. <see cref="PowerUI.TextureAtlas.Flush"/>.</summary>
		public void FlushAtlas(){
			if(Mesh.Atlas==null){
				return;
			}
			
			Mesh.Atlas.Flush();
		}
		
		/// <summary>Tells the mesh that we're done laying it out. <see cref="PowerUI.DynamicMesh.CompletedLayout"/>.</summary>
		public void CompletedLayout(){
			if(Isolated){
				// Was PrepLayout called on this update?
				// If not, Destroy it.
				if(!PrepareCalled){
					Destroy();
					return;
				}
			}
			
			PrepareCalled=false;
			
			// First, set it's depth so it knows where to render:
			SetRenderDepth(Renderer.BatchDepth++);
			
			Mesh.CompletedLayout();
		}
		
		/// <summary>Tells the mesh that we are about to perform a layout. <see cref="PowerUI.DynamicMesh.PrepareForLayout"/>.</summary>
		public void PrepareForLayout(){
			PrepareCalled=true;
			Mesh.PrepareForLayout();
		}
		
		/// <summary>Allocates a block in this batches mesh. <see cref="PowerUI.DynamicMesh.Allocate"/>.</summary>
		public MeshBlock Allocate(){
			return Mesh.Allocate();
		}
		
		/// <summary>Adds the given texture to the texture atlas. Note that the atlas buffers
		/// textures internally and won't be rebuilt each time.</summary>
		/// <param name="texture">The texture to add to the atlas.</param>
		public AtlasLocation AddTexture(Texture2D texture){
			return Mesh.AddTexture(texture);
		}
		
		/// <summary>Changes the font texture used by this batch. Note that this will only work if this
		/// batch is not isolated and uses the default UI shader.</summary>
		/// <param name="fontTexture">The new font texture to use.</param>
		public void ChangeFontTexture(Texture2D fontTexture){
			Mesh.ChangeFontTexture(fontTexture);
		}
		
		/// <summary>Puts this batch into the given layer ID.</summary>
		/// <param name="id">The ID of the layer.</param>
		public void RenderWithCamera(int id){
			Mesh.OutputGameObject.layer=id;
		}
		
		/// <summary>Gets or sets the active font texture in use.</summary>
		public Texture2D FontTexture{
			get{
				return Mesh.FontTexture;
			}
			set{
				ChangeFontTexture(value);
			}
		}
		
		/// <summary>Permanently destroys this UI batch.</summary>
		public void Destroy(){
			if(Renderer==null){
				return;
			}
			
			if(IsolatedProperty!=null){
				IsolatedProperty.Isolated=false;
				IsolatedProperty.OnBatchDestroy();
				IsolatedProperty=null;
			}
			
			if(Mesh!=null){
				Mesh.Destroy();
				Mesh=null;
			}
			Renderer=null;
		}
		
	}
	
}