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
using Nitro;


namespace PowerUI{
	
	/// <summary>
	/// Manages the currently focused element.
	/// </summary>
	
	public static class Focus{
		
		/// <summary>A shortcut to the current focused element (Input.Focused).</summary>
		public static Element Current{
			get{
				return Input.Focused;
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element above.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-up="anElementID" to override which element will be focused next.</summary>
		public static void MoveUp(){
			if(Current==null){
				return;
			}
			
			// Grab the element above:
			Element element=Input.Focused.GetFocusableAbove();
			
			if(element!=null){
				// Focus it:
				element.Focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element below.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-down="anElementID" to override which element will be focused next.</summary>
		public static void MoveDown(){
			if(Current==null){
				return;
			}
			
			// Grab the element below:
			Element element=Input.Focused.GetFocusableBelow();
			
			if(element!=null){
				// Focus it:
				element.Focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element to the left.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-left="anElementID" to override which element will be focused next.</summary>
		public static void MoveLeft(){
			if(Current==null){
				return;
			}
			
			// Grab the element to the left:
			Element element=Input.Focused.GetFocusableLeft();
			
			if(element!=null){
				// Focus it:
				element.Focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element to the right.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-right="anElementID" to override which element will be focused next.</summary>
		public static void MoveRight(){
			if(Current==null){
				return;
			}
			
			// Grab the element to the right:
			Element element=Input.Focused.GetFocusableRight();
			
			if(element!=null){
				// Focus it:
				element.Focus();
			}
		}
		
		/// <summary>Moves the focus to the next element as defined by tabindex. If there is no currently focused element,
		/// a tabindex of 0 will be searched for. If that's not found, then the first focusable element in the DOM will be used.</summary>
		public static void TabNext(){
			// Commit Pending
		}
		
	}
	
}