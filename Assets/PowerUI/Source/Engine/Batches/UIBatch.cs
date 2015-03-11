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
using Blaze;
using InfiniText;


namespace PowerUI{
	
	/// <summary>
	/// A UI Batch represents a block of the UI that can be safely rendered
	/// with a single drawcall (i.e. all in one mesh). Batches can be isolated - that
	/// means it can have its own material or texture atlas. Batches are created on
	/// demand by the renderer and based on the requests of the elements being rendered.
	/// An animation, for example, will generate an isolated batch.
	/// </summary>
	
	public class UIBatch{
		
		/// <summary>True when this batch has been fully initialised.</summary>
		public bool Setup;
		/// <summary>Are we in physics mode?</summary>
		public bool PhysicsMode;
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
		/// <summary>The current font atlas applied to this batch.</summary>
		public TextureAtlas FontAtlas;
		/// <summary>The current graphics atlas applied to this batch.</summary>
		public TextureAtlas GraphicsAtlas;
		/// <summary>If isolated, this is the property that created the isolated batch.</summary>
		public DisplayableProperty IsolatedProperty;
		
		
		/// <summary>Creates a new UI Batch which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer that will render this batch.</param>
		public UIBatch(Renderman renderer){
			
			Mesh=new DynamicMesh(this);
			
			ChangeRenderer(renderer);
			
		}
		
		/// <summary>Called when the renderer for this batch has changed.</summary>
		public void ChangeRenderer(Renderman renderer){
			
			Renderer=renderer;
			
			RenderWithCamera(renderer.RenderLayer);
			
			// Let the mesh know that the parent changed:
			Mesh.ChangeParent();
			
			InputMode mode;
			
			if(Renderer.RenderingInWorld){
				mode=PowerUI.Input.WorldInputMode;
			}else{
				mode=PowerUI.Input.Mode;
			}
			
			bool isPhysics=(mode==InputMode.Physics);
			
			if(isPhysics==PhysicsMode){
				return;
			}
			
			Mesh.SetPhysicsMode(isPhysics);
		}
		
		/// <summary>Sets the isolated state of this batch.</summary>
		public void IsIsolated(DisplayableProperty property){
			
			if(Isolated && Setup){
				// No change.
				return;
			}
			
			Setup=true;
			Isolated=true;
			FontAtlas=null;
			GraphicsAtlas=null;
			IsolatedProperty=property;
			
		}
		
		/// <summary>Sets the isolated state of this batch.</summary>
		public void NotIsolated(TextureAtlas graphics,TextureAtlas font,float alias){
			
			if(!Isolated && Setup){
				return;
			}
			
			Setup=true;
			Isolated=false;
			IsolatedProperty=null;
			
			Mesh.SetGlobalMaterial();
			SetFontAtlas(font,alias);
			SetGraphicsAtlas(graphics);
		}
		
		/// <summary>Sets the font atlas for this batch.</summary>
		public void SetFontAtlas(TextureAtlas font,float alias){
			FontAtlas=font;
			Mesh.SetFontAtlas(font);
			
			Mesh.Material.SetFloat("TopFontAlias",Fonts.OutlineLocation+alias);
			Mesh.Material.SetFloat("BottomFontAlias",Fonts.OutlineLocation-alias);
		}
		
		/// <summary>Sets the graphics atlas for this batch.</summary>
		public void SetGraphicsAtlas(TextureAtlas graphics){
			GraphicsAtlas=graphics;
			Mesh.SetGraphicsAtlas(graphics);
		}
		
		/// <summary>Called when the physics input mode changes.</summary>
		/// <param name="isPhysics">The new input mode to use.</param>
		public void SetPhysicsMode(bool isPhysics){
			
			// Are we changing?
			if(isPhysics==PhysicsMode){
				return;
			}
			
			PhysicsMode=isPhysics;
			
			Mesh.SetPhysicsMode(isPhysics);
		}
		
		public void SetRenderDepth(int index){
			// Transparent + offset.
			Mesh.Material.renderQueue=3000+index;
		}
		
		/// <summary>Flushes this batches mesh. <see cref="PowerUI.DynamicMesh.Flush"/>.</summary>
		public void Flush(){
			Mesh.Flush();
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
		
		/// <summary>Puts this batch into the given layer ID.</summary>
		/// <param name="id">The ID of the layer.</param>
		public void RenderWithCamera(int id){
			Mesh.OutputGameObject.layer=id;
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