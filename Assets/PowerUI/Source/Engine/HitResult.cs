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
	/// Represents the result of a Physics Mode input test. These are used when a player clicks on the UI
	/// and the UI is in Physics mode or when WorldUI's are listening for input.
	/// </summary>
	
	public class HitResult{
	
		/// <summary>The x location of the mouse relative to the UI from the left.</summary>
		public int MouseX;
		/// <summary>The y location of the mouse relative to the UI from the top.</summary>
		public int MouseY;
		/// <summary>True if no attempt was made to resolve the element.</summary>
		public bool ScreenMode;
		/// <summary>True if this hit was successful.</summary>
		private bool Successful;
		/// <summary>The worldUI that was hit, if any.</summary>
		public WorldUI OnWorldUI;
		/// <summary>The element that was hit, if any.</summary>
		public Element HitElement;
		
		/// <summary>Sets the location the mouse was at, relative to the UI.</summary>
		/// <param name="x">The x coordinate of the mouse on the screen, measured from the left.</param>
		/// <param name="y">The y coordinate of the mouse on the screen, measured from the top.</param>
		public void SetPoint(int x,int y){
			MouseX=x;
			MouseY=y;
		}
		
		/// <summary>Sets mouseX and mouseY from a relative 1 unit range coordinates.</summary>
		/// <param name="x">The relative x coordinate in the range +-0.5.</param>
		/// <param name="y">The relative y coordinate in the range +-0.5.</param>
		public void SetRelativePoint(float x,float y){
			// Offset so they enter the 0->1 range:
			x+=0.5f;
			// Note that y is inverted because the page goes downwards.
			// This is like inverting y, then adding the origin to it.
			y=0.5f-y;
			// Then multiply by the pixel dimensions:
			MouseX=(int)(x*OnWorldUI.pixelWidth);
			MouseY=(int)(y*OnWorldUI.pixelHeight);
		}
		
		/// <summary>Runs a click with the given UI event based on where on the UI this hit occured.</summary>
		public bool RunClick(UIEvent uiEvent){
			if(!Success){
				return false;
			}
			
			// Update coords:
			uiEvent.clientX=MouseX;
			uiEvent.clientY=MouseY;
			
			// Does it have a fully resolved element?
			if(HitElement!=null){
				// Yes it does - immediately bubble downwards from this element:
				Input.ClickOn(HitElement,uiEvent);
				return true;
			}else{
				// Nope; Instead treat this like any ordinary 2D click but on a WorldUI.
				return OnWorldUI.document.html.RunClickOnKids(uiEvent);
			}
		}
		
		/// <summary>Runs a mouse over event for the UI, if any, that this hit occured on.</summary>
		public void RunMouseOver(UIEvent mouseEvent){
			
			if(!Success){
				return;
			}
			
			// Update coords:
			mouseEvent.clientX=MouseX;
			mouseEvent.clientY=MouseY;
			
			Input.MouseY=MouseY;
			Input.MouseX=MouseX;
			
			// Does it have a fully resolved element?
			if(HitElement!=null){
				// Yes it does - start from that and bubble directly.
				Input.MouseOn(HitElement,mouseEvent);
			}else{
				// Nope - Do the mouseover event here with the WorldUI:
				OnWorldUI.document.RunMouseMove(mouseEvent);
			}
			
		}
		
		/// <summary>Finds and sets OnWorldUI from the given transform.
		/// If successful, Success will be true.</summary>
		/// <returns>True if successful, false otherwise.</returns>
		public bool FindWorldUI(Transform transform){
			if(WorldUI.PhysicsLookup==null){
				OnWorldUI=null;
				Successful=false;
			}else{
				Successful=WorldUI.PhysicsLookup.TryGetValue(transform,out OnWorldUI);
			}
			return Successful;
		}
		
		/// <summary>Sets the element that was clicked on.</summary>
		/// <param name="hitElement">The element that was clicked on.</param>
		public void SetElement(Element hitElement){
			HitElement=hitElement;
			Successful=(hitElement!=null);
		}
		
		/// <summary>Attempts to find the element from the set WorldUI; if there is no WorldUI, the main UI is used.
		/// This search uses the triangle of the hit to figure out exactly which element was clicked.</summary>
		/// <param name="hit">The hit in 3D that must be resolved to an element.</param>
		public void FindElement(RaycastHit hit){
			// Which triangle was hit, and as a result, which element did it come from?
			// If the element is found, apply it to our result; otherwise assume unsuccessful hit.
			Renderman renderer=null;
			if(OnWorldUI!=null){
				renderer=OnWorldUI.Renderer;
			}else{
				renderer=UI.GetRenderer();
			}
			
			if(renderer==null){
				return;
			}
			
			Transform transform=hit.transform;
			
			// Which batch? Will only be from the non-pooled ones:
			UIBatch current=renderer.FirstBatch;
			
			while(current!=null){
				if(current.Mesh.OutputTransform==transform){
					// Got it!
					break;
				}
				current=current.BatchAfter;
			}
			
			if(current==null){
				return;
			}
			
			// Current is the batch the hit was on. Next, resolve to the MeshBlock and finally to the element that made it.
		}
		
		/// <summary>True if this hit was successful.</summary>
		public bool Success{
			get{
				return Successful;
			}
		}
		
		/// <summary>True if this hit was on a WorldUI.</summary>
		public bool InWorld{
			get{
				return (OnWorldUI!=null);
			}
		}
		
	}
	
}