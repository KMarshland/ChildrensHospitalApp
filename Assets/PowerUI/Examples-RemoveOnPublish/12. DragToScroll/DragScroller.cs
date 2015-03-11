using System;
using PowerUI;
using UnityEngine;


public static class DragScroller{
	
	/// <summary>The starting point for the scrolling.</summary>
	private static int StartY;
	/// <summary>The element currently being scrolled.</summary>
	private static Element Scrolling;

	
	/// <summary>This gets called when the element is clicked on.
	/// It's onmousedown points at this method.</summary>
	public static void StartScroll(UIEvent e){
		if(!e.isLeftMouse){
			// Not a left click.
			return;
		}
		
		// Store the current position of the mouse, and the current scroll offset.
		// We want to find how many pixels it's moved by since it went down:
		StartY=e.clientY + e.target.scrollTop;
		
		// Store the element being scrolled:
		Scrolling=e.target;
		
		// Focus the element - this will cause onmousemove to run:
		Scrolling.Focus();
	}
	
	/// <summary>Called when the element scrolls.</summary>
	public static void ScrollNow(UIEvent e){
		// How much has the mouse moved by?
		int scrolledBy=StartY-e.clientY;
		
		// Whats the furthest it can go?
		// The height of the stuff inside the box - the actual height of the box.
		int limit=Scrolling.contentHeight-Scrolling.scrollHeight;
		
		if(limit<0){
			limit=0;
		}
		
		// Clip it by the limits:
		if(scrolledBy<0){
			// Dragged the content down really far.
			scrolledBy=0;
		}else if(scrolledBy>limit){
			// Dragged the content up too far.
			scrolledBy=limit;
		}
		
		// Scroll by that much:
		Scrolling.scrollTo(0,scrolledBy);
		
		// Check if the mouse went up:
		if(!e.leftMouseDown){
			// Left button isn't down - unfocus the element.
			// This will prevent any further mousemove's:
			Scrolling.Unfocus();
			
			// Running an animation here could be done to create 'drift' where the scrolling decelerates to a smooth stop. Highly reccommended to use the "scroll" CSS property.
			
			// Alternatively, delay the unfocus and have a speed value, then affect the speed from this function to have the same effect.
		}
		
	}
	
}