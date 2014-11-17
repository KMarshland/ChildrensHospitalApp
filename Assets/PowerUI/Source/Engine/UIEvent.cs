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
	/// A UI Event represents a click or keypress.
	/// An event object is always provided with any onmousedown/onmouseup/onkeydown etc.
	/// </summary>
	
	public class UIEvent{
		
		/// <summary>The x location of the mouse, as measured from the left of the screen.</summary>
		public int clientX;
		/// <summary>The y location of the mouse, as measured from the top of the screen.</summary>
		public int clientY;
		/// <summary>The keycode of the key pressed.</summary>
		public int keyCode;
		/// <summary>True if the mouse button or key is currently down.</summary>
		public bool heldDown;
		/// <summary>The element that was clicked on or focused.</summary>
		public Element target;
		/// <summary>The character that has been typed.</summary>
		public char character;
		/// <summary>A counter that tracks how many times this event has bubbled so far.</summary>
		public int bubbleCount;
		/// <summary>The source Unity event.</summary>
		public Event unityEvent;
		/// <summary>Set to true if you do not want this event to bubble any further.</summary>
		public bool cancelBubble;
		
		
		/// <summary>Creates a new UI event for the mouse.</summary>
		/// <param name="x">The x location of the mouse.</param>
		/// <param name="y">The y location of the mouse.</param>
		/// <param name="down">True if the button is held down.</param>
		public UIEvent(int x,int y,bool down){
			clientX=x;
			clientY=y;
			heldDown=down;
		}
		
		/// <summary>Creates a new UI event for a keypress.</summary>
		/// <param name="key">The keycode.</param>
		/// <param name="ch">The newly typed character.</param>
		/// <param name="down">True if the key is held down.</param>
		public UIEvent(int key,char ch,bool down){
			keyCode=key;
			character=ch;
			heldDown=down;
		}
		
		/// <summary>Stops the event bubbling to any other elements.</summary>
		public void stopPropagation(){
			cancelBubble=true;
		}
		
		/// <summary>Gets the keycode as a UnityEngine.KeyCode.</summary>
		public KeyCode unityKeyCode{
			get{
				return (KeyCode)keyCode;
			}
		}
		
		/// <summary>The mouse button that was pressed. See isLeftMouse and isRightMouse for clearer ways of using this value.</summary>
		public int button{
			get{
				return keyCode;
			}
		}
		
		/// <summary>Is the left mouse button currently down?</summary>
		public bool leftMouseDown{
			get{
				return UnityEngine.Input.GetMouseButton(0);
			}
		}
		
		/// <summary>Is the right mouse button currently down?</summary>
		public bool rightMouseDown{
			get{
				return UnityEngine.Input.GetMouseButton(1);
			}
		}
		
		/// <summary>Mouseup/down only. Was it the left mouse button?</summary>
		public bool isLeftMouse{
			get{
				return (button==0);
			}
		}
		
		/// <summary>Mouseup/down only. Was it the right mouse button?</summary>
		public bool isRightMouse{
			get{
				return (button==1);
			}
		}
		
		/// <summary>Is a control key down?</summary>
		public bool ctrlKey{
			get{
				if(unityEvent==null){
					return false;
				}
				
				return unityEvent.control;
			}
		}
		
		/// <summary>Is a shift key down?</summary>
		public bool shiftKey{
			get{
				if(unityEvent==null){
					return false;
				}
				
				return unityEvent.shift;
			}
		}
		
		/// <summary>Is an alt key down?</summary>
		public bool altKey{
			get{
				if(unityEvent==null){
					return false;
				}
				
				return unityEvent.alt;
			}
		}
		
		/// <summary>The document that this event has come from, if any.</summary>
		public Document document{
			get{
				
				if(target==null){
					return null;
				}
				
				return target.document;
			}
		}
		
		/// <summary>The WorldUI that this event has come from, if any.</summary>
		public WorldUI worldUI{
			get{
				
				if(target==null){
					return null;
				}
				
				return target.document.Renderer.InWorldUI;
			}
		}
		
	}
	
}