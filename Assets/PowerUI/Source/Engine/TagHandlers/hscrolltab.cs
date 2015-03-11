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

using PowerUI.Css;

namespace PowerUI{
	
	/// <summary>
	/// Handles the tab (draggable part) of a horizontal scrollbar.
	/// </summary>
	
	public class HScrollTabTag:ScrollTabTag{
		
		/// <summary>The x location of the mouse in pixels from the left when it clicked.</summary>
		public int MouseX;
		/// <summary>The start x location of the tab when the mouse clicked it.</summary>
		public int StartX;
		
		
		public override string[] GetTags(){
			return new string[]{"hscrolltab"};
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new HScrollTabTag();
		}
		
		public override void Clicked(UIEvent clickEvent){
			// Track where the mouse started off:
			MouseX=clickEvent.clientX;
			
			// And where the bar started off too:
			StartX=Element.style.Computed.PositionLeft;
		}
		
		public override bool UseX(){
			return true;
		}
		
		protected override void SetTabSize(int newSize){
			Element.style.width=newSize+"fpx";
			ScrollBy(0,true,true);
		}
		
		public override int BarSize(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			return ScrollBar.Element.style.Computed.InnerWidth-SizeBefore()-SizeAfter();
		}
		
		public override int TabSize(){
			return Element.style.Computed.InnerWidth;
		}
		
		public override int StyleSize(){
			// Grab the computed style:
			ComputedStyle style=Element.style.Computed;
			
			// Return the horizontal style size. That's:
			return style.PixelWidth-style.InnerWidth;
		}
		
		public override int SizeBefore(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			// Grab the first child:
			Element firstChild=ScrollBar.Element.firstChild;
			
			if(firstChild==Element){
				// No button before:
				return 0;
			}
			
			return firstChild.style.Computed.PixelWidth;
		}
		
		public override int SizeAfter(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			// Grab the last child:
			Element lastChild=ScrollBar.Element.lastChild;
			
			if(lastChild==Element){
				// No button after:
				return 0;
			}
			
			return lastChild.style.Computed.PixelWidth;
		}
		
		public override int TabPosition(){
			return Element.style.Computed.PositionLeft;
		}
		
		public override void OnMouseMove(UIEvent mouseEvent){
			int deltaX=mouseEvent.clientX-MouseX;
			
			if(deltaX==0){
				return;
			}
			
			ScrollBy(deltaX,false,true);
		}
		
		public override void ScrollTo(int location,bool scrollTarget){
			
			StartX=0;
			ScrollBy(location,false,scrollTarget);
			
		}
		
		public override void ScrollBy(int deltaX,bool fromCurrent,bool scrollTarget){
			// Scroll it by deltaX from StartX.
			int newLocation=deltaX;
			
			if(fromCurrent){
				newLocation+=Element.style.Computed.PositionLeft;
			}else{
				newLocation+=StartX;
			}
			
			// Get the size of the button before the tab:
			int sizeBefore=SizeBefore();
			
			int barSize=BarSize();
			
			int max=barSize+sizeBefore-Element.style.Computed.PixelWidth;
			
			if(newLocation<sizeBefore){
				newLocation=sizeBefore;
			}else if(newLocation>max){
				newLocation=max;
			}
			
			if(newLocation==Element.style.Computed.PositionLeft){
				return;
			}
			
			Element.style.left=newLocation+"fpx";
			
			if(scrollTarget){
				Element target=ScrollBar.GetTarget();
				
				if(target!=null){
					float progress=(float)(newLocation-sizeBefore)/(float)barSize;
					
					target.style.Computed.ScrollLeft=((int)(progress * target.style.Computed.ContentWidth));
					// Recompute the size:
					target.style.Computed.SetSize();
					// And request a redraw:
					target.Document.Renderer.RequestLayout();
				}
			}
			
		}
		
	}
	
}