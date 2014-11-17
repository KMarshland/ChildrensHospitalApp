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
	/// Managed automatically. You don't need to do anything with this class.
	/// When cameras are placed on the main UI, the UI must be broken up and rendered by multiple cameras.
	/// This class represents the set of cameras. It's created on first demand, and used from UI.MainCameraPool.
	/// </summary>
	
	public class CameraPool{
		
		/// <summary>True if layout was called on this camera pool.</summary>
		public bool DidLayout;
		
		/// <summary>The current depth the camera's are being set at.</summary>
		public int CameraDepth;
		/// <summary>Linked list. The last active camera in the pool.</summary>
		public UICamera LastCamera;
		/// <summary>Linked list. The first active camera in the pool.</summary>
		public UICamera FirstCamera;
		/// <summary>True if this renderman must create a new camera on demand.</summary>
		public bool CameraRequested;
		/// <summary>Linked list. The last inactive camera in the pool.</summary>
		public UICamera LastPoolCamera;
		/// <summary>Linked list. The first inactive camera in the pool.</summary>
		public UICamera FirstPoolCamera;
		
		
		/// <summary>Creates a new pool with default values.</summary>
		public CameraPool(){
			CameraDepth=UI.CameraDepth;
		}
		
		/// <summary>Allocates a camera depth.</summary>
		public int AllocatedDepth{
			get{
				int result=CameraDepth+1;
				CameraDepth=result;
				return result;
			}
		}
		
		/// <summary>Called on a pool if a camera was requested.
		/// This call occurs when something on the main UI is being rendered ontop of an inline camera.</summary>
		public bool CheckCameraRequired(){
			
			if(!CameraRequested){
				return false;
			}
			
			// Clear the request:
			CameraRequested=false;
			
			// A camera is required!
			
			// Get the main UI renderer:
			Renderman renderer=UI.GetRenderer();
			
			// We must create a UICamera (or pull one from the pool):
			UICamera camera=GetPooledCamera();
			
			if(camera==null){
				// Create one:
				camera=new UICamera(renderer);
			}
			
			// Add it to the main set:
			AddCamera(camera);
			
			// Next, set the current depth:
			camera.SetDepth(AllocatedDepth);
			
			// Finally, we must now make sure any batches are parented to this new camera.
			// We do this by setting it to the root of the renderer:
			renderer.Node=camera.Gameobject;
			
			return true;
		}
		
		/// <summary>Resets the pool. Called before a main UI layout occurs.</summary>
		public void Reset(){
			CameraRequested=false;
			CameraDepth=UI.CameraDepth;
			
			// Push all active cameras into the inactive pool:
			DidLayout=true;
			FirstPoolCamera=FirstCamera;
			LastPoolCamera=LastCamera;
			FirstCamera=null;
			LastCamera=null;
		}
		
		/// <summary>Destroys everything in this set.</summary>
		public void Destroy(){
			
			// Clear the pool:
			ClearPool();
			
			// Put the whole active set into the inactive pool:
			FirstPoolCamera=FirstCamera;
			FirstCamera=null;
			LastCamera=null;
			
			// Clear the pool again:
			ClearPool();
		}
		
		/// <summary>Clears the inactive pool.</summary>
		public void ClearPool(){
			DidLayout=false;
			UICamera current=FirstPoolCamera;
			FirstPoolCamera=null;
			LastPoolCamera=null;
			
			while(current!=null){
				current.Destroy();
				current=current.CameraAfter;
			}
		}
		
		/// <summary>Gets a camera from the inactive pool. Null if the pool is empty.</summary>
		public UICamera GetPooledCamera(){
			if(FirstPoolCamera==null){
				return null;
			}
			
			UICamera result=FirstPoolCamera;
			FirstPoolCamera=result.CameraAfter;
			result.CameraAfter=null;
			return result;
		}
		
		/// <summary>Adds the given active camera to the main linked list for processing.</summary>
		public void AddCamera(UICamera camera){
			if(FirstCamera==null){
				FirstCamera=LastCamera=camera;
			}else{
				LastCamera=LastCamera.CameraAfter=camera;
			}
		}
		
	}
	
}