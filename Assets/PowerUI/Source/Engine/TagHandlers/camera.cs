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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	#define PRE_UNITY4
#endif

using System;
using PowerUI.Css;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// Custom tag for an inline camera. You can place your UI before and after this as normal.
	/// You must importantly set the path="" attribute to the path in the hierarchy of the camera itself.
	/// This tag also has a mask="file_path" attribute which can be used to shape the camera
	/// in interesting ways e.g. a circular minimap.
	/// You must also set the height and width of this element using either css or height="" and width="".
	/// </summary>
	
	public class CameraTag:HtmlTagHandler{
		
		/// <summary>The camera component iself. Set with path="hierarchy_path".</summary>
		public Camera Camera;
		/// <summary>The transform of the mask, if there is one. Set with mask="file_path".</summary>
		public Transform Mask;
		/// <summary>The depth factor of a perspective camera.</summary>
		public float DepthFactor;
		/// <summary>The field of view of a perspective camera.</summary>
		public float FieldOfView;
		
		
		public override string[] GetTags(){
			return new string[]{"camera"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new CameraTag();
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="path"){
				// Go get the camera now!
				
				// Clear any existing one:
				Camera=null;
				
				// Grab the path itself:
				string path=Element["path"];
				
				// Get it:
				GameObject gameObject=GameObject.Find(path);
				
				if(gameObject!=null){
					// Grab the camera:
					Camera=gameObject.GetComponent<Camera>();
				}
				
				if(Camera!=null){
					// Setup the clear flags:
					Camera.clearFlags=CameraClearFlags.Depth;
				}
				
				ParentMask();
				
				return true;
			}else if(property=="mask"){
				// We've got a mask!
				
				// Grab the file path:
				string maskFile=Element["mask"];
				
				if(maskFile==null){
					SetMask(null);
				}else{
					// Create a package to get the mask:
					ImagePackage package=new ImagePackage(maskFile,Element.Document.basepath);
					
					// Go get it, calling ImageReady when it's been retrieved.
					package.Get(ImageReady);
				}
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Called when a mask image has been received.</summary>
		/// <param name="package">The package holding the mask image.</param>
		public void ImageReady(ImagePackage package){
			// Apply the mask:
			SetMask(package.Image);
		}
		
		/// <summary>Parents the mask to the camera, if there is one.</summary>
		private void ParentMask(){
			if(Mask==null || Camera==null){
				return;
			}
			
			// Parent it:
			Mask.parent=Camera.transform;
			
			// Reset the location:
			Mask.localRotation=Quaternion.identity;
			Mask.localPosition=new Vector3(0f,0f,Camera.nearClipPlane+0.5f);
			Mask.localScale=new Vector3(80f,80f,80f);
		}
		
		/// <summary>Applies an alpha mask over the camera itself. This can be used to shape the camera
		/// into e.g. a circle.</summary>
		/// <param name="image">The mask image. Originates from the mask="file_path" attribute.</param>
		public void SetMask(Texture2D image){
			if(Element.Document.AotDocument){
				return;
			}
			
			if(Mask==null){
				// Create the gameobject.
				
				if(image==null){
					return;
				}
				
				// Create the object:
				#if PRE_UNITY4
				GameObject maskObject=GameObject.CreatePrimitive(PrimitiveType.Plane);
				#else
				GameObject maskObject=GameObject.CreatePrimitive(PrimitiveType.Quad);
				#endif
				
				// Remove the MC:
				MeshCollider collider=maskObject.GetComponent<MeshCollider>();
				
				if(collider!=null){
					// Remove it:
					GameObject.Destroy(collider);
				}
				
				// Grab the transform:
				Mask=maskObject.transform;
				
				// Set the name:
				maskObject.name="#PowerUI-Mask";
				
				// If possible, parent it to the camera now:
				ParentMask();
				
				// Grab the renderer:
				MeshRenderer renderer=maskObject.GetComponent<MeshRenderer>();
				
				// Grab the material:
				Material material=renderer.material;
				
				// Apply the shader:
				material.shader=MaskShader;
				
				// Set the offset - it must be the first thing rendered always:
				material.renderQueue=1;
				
				// Apply the mask texture:
				material.SetTexture("_Mask",image);
				
			}
			
		}
		
		/// <summary>Gets or creates the main UI camera pool.</summary>
		public CameraPool MainCameraPool{
			get{
				// Grab the current pool:
				CameraPool result=UI.MainCameraPool;
				
				if(result==null){
					// None yet - create one:
					UI.MainCameraPool=result=new CameraPool();
				}
				
				return result;
			}
		}
		
		/// <summary>Called during the layout pass.</summary>
		public override void OnLayout(){
			if(Camera==null){
				return;
			}
			
			// Get (or create) the pool:
			CameraPool pool=MainCameraPool;
			
			// Get the depth for this camera:
			int currentDepth=pool.AllocatedDepth;
			
			// Set it's depth:
			Camera.depth=currentDepth;
			
			// Grab the computed style:
			ComputedStyle computed=Element.Style.Computed;
			
			// Set the screen rect:
			Camera.pixelRect=new Rect(computed.OffsetLeft+computed.StyleOffsetLeft,ScreenInfo.ScreenY-computed.OffsetTop-computed.InnerHeight-computed.StyleOffsetTop,computed.InnerWidth,computed.InnerHeight);
			
			// Figure out the mask size, if there is one:
			if(Mask!=null){
				
				// Figure out the aspect ratio:
				float aspect=((float)computed.InnerWidth/(float)computed.InnerHeight);
				
				if(Camera.orthographic){
					if(DepthFactor!=0f){
						DepthFactor=0f;
					}
					
					// Grab the orthoSize:
					float size=Camera.orthographicSize*2f;
					
					// Set the scale:
					Mask.localScale=new Vector3(size*aspect,size,1f);
				}else{
					
					if(DepthFactor==0f || Camera.fieldOfView!=FieldOfView){
						ComputeDepth();
					}
					
					// It just needs to be depth factor long:
					
					Mask.localScale=new Vector3(
						DepthFactor*aspect,
						DepthFactor,
						1f
					);
					
				}
			}
			
			// Any UI after this camera must be rendered with a new camera.
			// Tell the pool it now requires a camera. It's only created if one is actually needed.
			pool.CameraRequested=true;
		}
		
		/// <summary>Computes the depth factor for a perspective camera when it's required for a mask.</summary>
		private void ComputeDepth(){
			
			// The depth that the mask is at (constant):
			float depth=Mask.localPosition.z;
			
			// Update the FOV:
			FieldOfView=Camera.fieldOfView;
			
			// Forming a triangle from the camera, figure out how "long" the mask should be:
			float opp=depth*(float)Math.Tan((FieldOfView/2f)*Mathf.Deg2Rad);
			
			// The depth factor is the target length/1, which is just target length:
			// opp was using a triangle (only one half of the cameras view), so *2:
			DepthFactor=opp*2f;
			
		}
		
		/// <summary>The shader used for the camera mask.</summary>
		public Shader MaskShader{
			get{
				return Shader.Find("PowerUI/Camera Mask");
			}
		}
		
	}
	
}