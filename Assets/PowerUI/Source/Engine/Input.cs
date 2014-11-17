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
	
	/// <summary>A delegate which can be used to place the mouse in custom locations. See Input.OnResolve2D.</summary>
	public delegate void MousePositionResolver2D();
	/// <summary>A delegate which can be used to run custom WorldUI mouse position handling. See Input.OnResolve3D.</summary>
	public delegate bool MousePositionResolver3D(out RaycastHit hit,UIEvent uiEvent);
	
	/// <summary>
	/// This class manages input such as clicking, hovering and keypresses.
	/// </summary>
	
	public static class Input{
		
		/// <summary>The computed mouseX coordinate on the currently focused UI. May originate from a raycast for WorldUIs.</summary>
		public static int MouseX;
		/// <summary>The computed mouseY coordinate on the currently focused UI. May originate from a raycast for WorldUIs.</summary>
		public static int MouseY;
		/// <summary>The currently focused element.</summary>
		public static Element Focused;
		/// <summary>The position of the mouse in pixels from the top left corner.</summary>
		public static Vector2 MousePosition;
		/// <summary>If WorldUI's receive input, a ray must be fired from CameraFor3DInput to attempt input.
		/// This is the lastest ray result. UI.MouseOver updates this immediately; it's updated at the UI rate otherwise.</summary>
		public static RaycastHit LatestRayHit;
		/// <summary>The camera used for 3D input. Defaults to Camera.main if this is null.</summary>
		public static Camera CameraFor3DInput;
		/// <summary>The mode of input. Use UI.InputMode to change this correctly.
		/// This defines how a click/tap should be resolved to an element.</summary>
		public static InputMode Mode=InputMode.Screen;
		/// <summary>Used to resolve the 2D location of the mouse. If used, apply your results to MousePosition.</summary>
		public static MousePositionResolver2D OnResolve2D;
		/// <summary>Used to resolve the 3D location of the mouse.</summary>
		public static MousePositionResolver3D OnResolve3D;
		/// <summary>The mode of input for worldUI's.</summary>
		public static InputMode WorldInputMode=InputMode.None;
		/// <summary>The set of all elements that currently have the mouse over them.
		/// This is essentially a stack; The element at the front of the list is the element on top.
		/// This is because the event bubbles upwards starting at the deepest element in the DOM.</summary>
		public static List<Element> MouseOvers=new List<Element>();
		/// <summary>The latest elements to receive a mousedown. 
		/// More than one because it's possible to click on a whole bunch of elements at the same time.</summary>
		public static List<Element> LastMouseDown=new List<Element>();
		
		#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
		/// <summary>True if autocorrect should be enabled.</summary>
		public static bool Autocorrect;
		/// <summary>This flag prevents mouseover from firing when a finger is released from the screen.</summary>
		public static bool MouseoverActive;
		/// <summary>The text currently in the keyboard.</summary>
		private static string CachedKeyboardValue;
		/// <summary>The mobile keyboard. This gets opened if the current focused element is a text/password input box.
		/// It gets closed when the element is blurred, or the user closes the keyboard.</summary>
		public static TouchScreenKeyboard MobileKeyboard;
		
		#endif
		
		/// <summary>True if the mouse is over any element on the UI.</summary>
		public static bool MouseOverUI{
			get{
				return (MouseOvers.Count>0);
			}
		}
		
		#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
		/// <summary>Handles if the mobile keyboard should get displayed/hidden.</summary>
		/// <param name="mode">The state that defines how the keyboard should open.</param>
		public static bool HandleKeyboard(KeyboardMode mode){
			if(MobileKeyboard!=null){
				MobileKeyboard.active=false;
				MobileKeyboard=null;
			}
			
			if(mode==null){
				return false;
			}else{
				MobileKeyboard=TouchScreenKeyboard.Open(mode.StartText,mode.Type,Autocorrect,mode.Multiline,mode.Secret);
				KeyboardText=null;
				return true;
			}
		}
		#endif
		
		#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
		/// <summary>The mobile keyboard text.</summary>
		public static string KeyboardText{
			get{
				if(MobileKeyboard==null){
					return null;
				}
				return MobileKeyboard.text;
			}
			set{
				if(MobileKeyboard==null){
					return;
				}
				CachedKeyboardValue=value;
				MobileKeyboard.text=value;
			}
		}
		#endif
		
		/// <summary>Clears all active elements.</summary>
		public static void Clear(){
			MouseOvers.Clear();
			Focused=null;
		}
		
		/// <summary>Clears mouse overs from the list and calls onmouseout with the given event.</summary>
		/// <param name="mouseEvent">The event to use for mouse out calls.</param>
		/// <param name="fromIndex">The (inclusive) index to start clearing from. 0 clears the whole list.</param>
		public static void ClearMouseOvers(UIEvent mouseEvent,int fromIndex){
			if(!MouseOverUI){
				return;
			}
			
			List<Element> newList=null;
			
			if(fromIndex!=0){
				// We'll have to build a new list out of the stuff up to fromIndex:	
				newList=new List<Element>(fromIndex);
				for(int i=0;i<fromIndex;i++){
					newList.Add(MouseOvers[i]);
				}
			}
			
			for(int i=fromIndex;i<MouseOvers.Count;i++){
				MouseOvers[i].MouseOut(mouseEvent);
			}
			
			if(fromIndex==0){
				MouseOvers.Clear();
			}else{
				MouseOvers=newList;
			}
		}
		
		/// <summary>Used internally. Don't call this from within a 2D resolver. Locates the mouse at the given position.</summary>
		public static void SetMousePosition(Vector2 position){
			MousePosition=position;
			
			if(OnResolve2D!=null){
				OnResolve2D();
			}
		}
		
		/// <summary>Updates mouse overs and the mouse position.</summary>
		public static void Update(){
			Vector2 position=UnityEngine.Input.mousePosition;
			
			// MousePosition's Y value is inverted, so flip it:
			position.y=ScreenInfo.ScreenY-1-position.y;
			
			// Apply the position:
			SetMousePosition(position);
			
			// Handle mouse overs - internally sets MouseX and MouseY too:
			OnMouseOver((int)MousePosition.x,(int)MousePosition.y);
			
			// Handle MouseMove:
			if(Focused!=null){
				
				#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
					// Handle mobile keyboard:
					if(MobileKeyboard!=null && MobileKeyboard.active){
						// Did the text change? If so, update the value of Focused.
						if(KeyboardText!=CachedKeyboardValue){
							Focused.value=CachedKeyboardValue=KeyboardText;
						}
					}
				#endif
				
				// Create the move event using the mapped mouseX/mouseY:
				UIEvent moveEvent=new UIEvent(MouseX,MouseY,false);
				moveEvent.target=Focused;
				
				Focused.Handler.OnMouseMove(moveEvent);
			}
		}
		
		/// <summary>Tells the UI a key was released.</summary>
		/// <param name="keyCode">The keycode of the key</param>
		/// <param name="character">The character entered.</param>
		/// <returns>True if the UI consumed the release.</returns>
		public static bool OnKeyUp(int keyCode,char character){
			return OnKeyPress(false,character,keyCode);
		}
		
		/// <summary>Tells the UI a key was pressed down.</summary>
		/// <param name="keyCode">The keycode of the key</param>
		/// <param name="character">The character entered.</param>
		/// <returns>True if the UI consumed the press.</returns>
		public static bool OnKeyDown(int keyCode,char character){
			return OnKeyPress(true,character,keyCode);
		}
		
		/// <summary>Tells the UI a key was pressed.</summary>
		/// <param name="down">True if the key is now down.</param>
		/// <param name="keyCode">The keycode of the key</param>
		/// <param name="character">The character entered.</param>
		/// <returns>True if the UI consumed the keypress.</returns>
		public static bool OnKeyPress(bool down,char character,int keyCode){
			UIEvent uiEvent=new UIEvent(keyCode,character,down);
			
			// Set the current event:
			uiEvent.unityEvent=Event.current;
			
			if(down){
				
				if(UI.document.RunKeyDown(uiEvent)){
					return true;
				}
				
			}else{
				
				if(UI.document.RunKeyUp(uiEvent)){
					return true;
				}
				
			}
			
			if(Focused==null){
				return false;
			}
			
			if(!Focused.isInDocument){
				// It got removed.
				Focused.Unfocus();
				return false;
			}
			
			Focused.Handler.OnKeyPress(uiEvent);
			return true;
		}
		
		/// <summary>Tells the UI the mouse was clicked.</summary>
		/// <param name="x">The x coordinate of the mouse in pixels from the left of the screen.</param>
		/// <param name="y">The y coordinate of the mouse in pixels from the top of the screen.</param>
		/// <returns>True if the mouse was pressed down on the UI.</returns>
		public static bool OnMouseDown(int x,int y){
			UIEvent uiEvent;
			bool result=RunClick(x,y,true,out uiEvent);
			
			if(!result&&Focused!=null){
				// Clicked in empty space but something is focused.
				Focused.Unfocus();
			}
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
			MouseoverActive=true;
			#endif
			
			return result;
		}
		
		/// <summary>Tells the UI the mouse was released.</summary>
		/// <param name="x">The x coordinate of the mouse in pixels from the left of the screen.</param>
		/// <param name="y">The y coordinate of the mouse in pixels from the top of the screen.</param>
		/// <returns>True if the mouse was released on the UI.</returns>
		public static bool OnMouseUp(int x,int y){
			UIEvent uiEvent;
			bool result=RunClick(x,y,false,out uiEvent);
			
			if(Focused!=null){
				// Always run mouseup on it.
				Focused.Handler.OnClick(uiEvent);
			}
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
			MouseoverActive=false;
			ClearMouseOvers(uiEvent,0);
			#endif
			
			return result;
		}
		
		/// <summary>Tells the UI the mouse was clicked or released.</summary>
		/// <param name="x">The x coordinate of the mouse in pixels from the left of the screen.</param>
		/// <param name="y">The y coordinate of the mouse in pixels from the top of the screen.</param>
		/// <param name="mouseDown">True if the button is now down.</param>
		/// <returns>True if the mouse was on the UI.</returns>
		private static bool RunClick(int x,int y,bool mouseDown,out UIEvent uiEvent){
			uiEvent=new UIEvent(x,y,mouseDown);
			
			// Which button?
			if(Event.current!=null){
				uiEvent.keyCode=Event.current.button;
			}
			
			if(UI.document==null || UI.document.html==null){
				return false;
			}
			
			int invertedY=ScreenInfo.ScreenY-1-y;
			
			bool result=false;
			
			if(Mode==InputMode.Screen){
				result=UI.document.html.RunClickOnKids(uiEvent);
			}else if(Mode==InputMode.Physics){
				// Screen physics cast here.
				
				RaycastHit uiHit;
				if(Physics.Raycast(UI.GUICamera.ScreenPointToRay(new Vector2(x,invertedY)),out uiHit)){
					// Did it hit the main UI?
					HitResult hit=HandleUIHit(uiHit);
					result=hit.Success;
					
					if(result){
						// Yes - As this is the main UI, We must have a HitElement available. All we need to do is ClickOn it!
						ClickOn(hit.HitElement,uiEvent);
					}
				}
				
			}
			
			if(!result && WorldInputMode!=InputMode.None){
				// Didn't hit the main UI - handle clicks on WorldUI's.
				RaycastHit worldUIHit;
				
				if(CameraFor3DInput==null){
					CameraFor3DInput=Camera.main;
				}
				
				bool hitSuccess=false;
				
				if(OnResolve3D!=null){
					hitSuccess=OnResolve3D(out worldUIHit,uiEvent);
				}else{
					hitSuccess=Physics.Raycast(CameraFor3DInput.ScreenPointToRay(new Vector2(x,invertedY)),out worldUIHit);
				}
				
				if(hitSuccess){
					// Did it hit a worldUI?
					HitResult hit=HandleWorldUIHit(worldUIHit);
					result=hit.Success;
					
					if(result){
						// Yes it did.
						result=hit.RunClick(uiEvent);
					}
				}
			}
			
			// Clear any LastMouseDown entries:
			if(!mouseDown){
				
				// Clear their active state:
				for(int i=LastMouseDown.Count-1;i>=0;i--){
					// Get the element:
					Element element=LastMouseDown[i];
					
					// Get computed style:
					Css.ComputedStyle computed=element.Style.Computed;
					
					// Clear active:
					computed.UnsetModifier("active");
					
					// Still got the mouse over it?
					if(element.MousedOver!=MouseOverState.Out){
						// Yep! Re-apply hover:
						computed.Hover();
					}
				}
				
				// Clear the set:
				LastMouseDown.Clear();
			}
			
			return result;
		}
		
		/// <summary>Runs a click which may bubble all the way to the root from the given element.</summary>
		/// <param name="element">The element to run a click on.</param>
		/// <param name="uiEvent">The event that represents the click.</param>
		public static void ClickOn(Element element,UIEvent uiEvent){
			if(uiEvent.cancelBubble || element==null || element.Handler.IgnoreSelfClick){
				return;
			}
			element.GotClicked(uiEvent);
			ClickOn(element.ParentNode,uiEvent);
		}
		
		/// <summary>Runs a mouseover which may bubble all the way to the root from the given element.</summary>
		/// <param name="element">The element to run a mouseover on.</param>
		/// <param name="uiEvent">The event that represents the click.</param>
		public static void MouseOn(Element element,UIEvent uiEvent){
			if(uiEvent.cancelBubble || element==null || element.Handler.IgnoreSelfClick){
				return;
			}
			
			if(element.GetType()!=typeof(TextElement)){
				element.MouseOver(uiEvent);
			}
			
			MouseOn(element.ParentNode,uiEvent);
		}
		
		/// <summary>Checks if the mouse is over the UI and runs the mouse over methods.</summary>
		/// <param name="x">The x coordinate of the mouse in pixels from the left of the screen.</param>
		/// <param name="y">The y coordinate of the mouse in pixels from the top of the screen.</param>
		/// <returns>True if the mouse was over the UI.</returns>
		public static bool OnMouseOver(int x,int y){
			MouseY=y;
			MouseX=x;
			
			if(UI.document==null||UI.document.html==null){
				return false;
			}
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
			if(!MouseoverActive){
				return false;
			}
			#endif
			
			UIEvent mouseEvent=new UIEvent(x,y,false);
			bool result=false;
			
			int invertedY=ScreenInfo.ScreenY-1-y;
			
			if(Mode==InputMode.Screen){
				result=UI.document.html.RunMouseOver(mouseEvent);
			}else if(Mode==InputMode.Physics){
				// Screen physics cast here. 
				RaycastHit uiHit;
				if(Physics.Raycast(UI.GUICamera.ScreenPointToRay(new Vector2(x,invertedY)),out uiHit)){
					HitResult hit=HandleUIHit(uiHit);
					result=hit.Success;
					if(result){
						// Great! As this is the main UI, We have a HitElement available.
						// Start from that and bubble the event to the root.
						MouseOn(hit.HitElement,mouseEvent);
					}
				}
			}
			
			if(!result && WorldInputMode!=InputMode.None){
				// World UI input time.
				RaycastHit worldUIHit;
				
				if(CameraFor3DInput==null){
					CameraFor3DInput=Camera.main;
				}
				
				bool hitSuccess=false;
				
				if(OnResolve3D!=null){
					hitSuccess=OnResolve3D(out worldUIHit,mouseEvent);
				}else{
					hitSuccess=Physics.Raycast(CameraFor3DInput.ScreenPointToRay(new Vector2(x,invertedY)),out worldUIHit);
				}
				
				if(hitSuccess){
					LatestRayHit=worldUIHit;
					// Hit something - was it a worldUI?
					HitResult hit=HandleWorldUIHit(worldUIHit);
					result=hit.Success;
					if(result){
						// Yes it was.
						hit.RunMouseOver(mouseEvent);
					}
				}else{
					LatestRayHit=default(RaycastHit);
				}
			}
			
			if(!result){
				ClearMouseOvers(mouseEvent,0);
			}
			return result;
		}
		
		/// <summary>Checks if the given hit was on a WorldUI. If it was, it runs the click and returns true.</summary>
		/// <param name="hit">The successful raycast hit.</param>
		public static HitResult HandleUIHit(RaycastHit hit){
			HitResult result=new HitResult();
			Transform transform=hit.transform;
			
			// Only occurs in physics mode - we're looking for a transform with the following name:
			if(transform.name=="PowerUI-CMesh"){
				// Great! Hit the main UI.
				// Which element did it come from?
				result.FindElement(hit);
			}
			
			return result;
		}
		
		/// <summary>Checks if the given hit was on a WorldUI. If it was, it runs the click and returns true.</summary>
		/// <param name="hit">The successful raycast hit.</param>
		public static HitResult HandleWorldUIHit(RaycastHit hit){
			HitResult result=new HitResult();
			
			Transform transform=hit.transform;
			
			if(WorldInputMode==InputMode.Physics){
				// If we're in Physics mode, we're looking for MeshColliders on batches with the following name.
				if(transform.name=="PowerUI-CMesh"){
					// We got one! Which WorldUI is it?
					if(result.FindWorldUI(hit.transform.parent)){
						// Got the WorldUI in the result now.
						result.FindElement(hit);
					}
					return result;
				}
			}else if(WorldInputMode==InputMode.Screen){
				// If we're in Screen mode, we're looking for a box collider with the following name.
				if(transform.name=="PowerUI-BatchBox"){
					// We got one! Which WorldUI is it?
					if(result.FindWorldUI(transform.parent)){
						// Got the WorldUI in the result now.
						result.ScreenMode=true;
						
						// Next, we need to map the location on the front of the box to our 2D point.
						// First, whats the point relative to the box?
						Vector3 point=transform.InverseTransformPoint(hit.point);
						
						// Great - we now have a relative point from +-0.5 in x and y.
						result.SetRelativePoint(point.x,point.y);
					}
					return result;
				}
			}
			
			return result;
		}
		
	}
	
}