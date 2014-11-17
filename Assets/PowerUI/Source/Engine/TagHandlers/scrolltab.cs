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

namespace PowerUI{
	
	/// <summary>
	/// Handles the tab (draggable part) of a horizontal or vertical scrollbar.
	/// </summary>
	
	public class ScrollTabTag:HtmlTagHandler{
		
		/// <summary>The scrollbar this tab belongs to.</summary>
		public InputTag ScrollBar;
		
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override bool OnClick(UIEvent clickEvent){
			GetScrollBar();
			
			if(clickEvent.heldDown){
				// Focus the tab - this causes onmousemove to fire:
				Element.Focus();
				// And tell the tab it got clicked:
				Clicked(clickEvent);
			}else{
				// Unfocus the tab.
				Element.Unfocus();
			}
			
			clickEvent.stopPropagation();
			
			return true;
		}
		
		/// <summary>The size (either width or height) of the element before the scroll tab.</summary>
		public virtual int SizeBefore(){
			return 0;
		}
		
		/// <summary>The size (either width or height) of the element after the scroll tab.</summary>
		public virtual int SizeAfter(){
			return 0;
		}
		
		/// <summary>The total border/padding size on this axis of the tab itself.</summary>
		public virtual int StyleSize(){
			return 0;
		}
		
		/// <summary>Called when the element has been scrolled.</summary>
		/// <param name="progress">A value from 0->1 which denotes how much the content has been scrolled by.</param>
		public void ElementScrolled(float progress){
			// Get the position the tab should be at (Compensating for the arrow button):
			int scrollPoint=(int)(progress*BarSize())+SizeBefore();
			
			// Make it relative to the current location:
			if(UseX()){
				scrollPoint-=Element.style.Computed.PositionLeft;
			}else{
				scrollPoint-=Element.style.Computed.PositionTop;
			}
			
			// Scroll the tab only:
			ScrollBy(scrollPoint,true,false);
		}
		
		/// <summary>Sets up the <see cref="PowerUI.ScrollTabTag.ScrollBar"/> property.</summary>
		public void GetScrollBar(){
			if(ScrollBar==null){
				// Get the scroll bar:
				ScrollBar=((InputTag)(Element.parentNode.Handler));
			}
		}
		
		/// <summary>Checks if this is a horizontal scrollbar.</summary>
		///	<returns>True if this is a horizontal scrollbar which uses the x axis; false otherwise.</returns>
		public virtual bool UseX(){
			return false;
		}
		
		/// <summary>Gets the length of the bar in pixels.</summary>
		/// <returns>The pixel length of the scrollbar.</returns>
		public virtual int BarSize(){
			return 0;
		}
		
		/// <summary>The current position of the tab. Note that width of elements before the tab like buttons are included in this.</summary>
		public virtual int TabPosition(){
			return 0;
		}
		
		/// <summary>Gets this tabs progress along the scrollbar, taking into account the size of the tab itself.</summary>
		public float TabProgress(){
			// Get the size of the track, accounting for the tab itself:
			int trackSize=(BarSize()-TabSize());
			
			// Get the tab position:
			int position=(TabPosition()-SizeBefore());
			
			return (float)position / (float)trackSize;
		}
		
		/// <summary>Scrolls this scrollbar by the given number of pixels.
		/// Note that this may fail if the scrollbar cannot scroll any further.</summary>
		/// <param name="pixels">The number of pixels to scroll this bar by.</param>
		public void ScrollBy(int pixels){
			ScrollBy(pixels,true,true);
		}
		
		/// <summary>Scrolls this scrollbar by the given number of pixels, optionally relative to a fixed point on the bar.
		/// Note that this may fail if the scrollbar cannot scroll any further.</summary>
		/// <param name="pixels">The number of pixels to scroll this bar by.</param>
		/// <param name="fromCurrent">True if pixels is relative to where the tab currently is. False if pixels is relative
		/// to where the bar was when the mouse was clicked. See e.g. <see cref="PowerUI.VScrollTabTag.StartY"/>.</param>
		/// <param name="scrollTarget">True if the target should also be scrolled.</param>
		public virtual void ScrollBy(int pixels,bool fromCurrent,bool scrollTarget){}
		
		/// <summary>Called when the tab is clicked on.</summary>
		/// <param name="clickEvent">The mouse click event.</param>
		public virtual void Clicked(UIEvent clickEvent){}
		
		/// <summary>Makes the tab a percentage size relative to the length of the bar.</summary>
		/// <param name="percentSize">A value from 0->1 that represents how visible the content
		/// is and as a result how long the tab is.</param>
		public void ApplyTabSize(float percentSize){
			
			// How wide is the border/padding of the tab?
			int styleSize=StyleSize();
			
			// How big should the new tab be?
			int newTabSize=(((int)(percentSize*BarSize()))-styleSize);
			
			if(newTabSize==TabSize()){
				// It didn't change.
				return;
			}
			
			// Apply the new tab size:
			SetTabSize(newTabSize);
		}
		
		/// <summary>Sets the tab to be the given size in pixels.</summary>
		/// <param name="newSize">The size in pixels of the tab.</param>
		protected virtual void SetTabSize(int newSize){}
		
		/// <summary>Gets the tabs size in pixels.</summary>
		/// <returns>The size of the tab in pixels.</returns>
		public virtual int TabSize(){
			return 0;
		}
		
	}
	
}