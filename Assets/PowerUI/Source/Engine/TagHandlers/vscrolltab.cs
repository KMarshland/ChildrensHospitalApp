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
	/// Handles the tab (draggable part) of a vertical scrollbar.
	/// </summary>
	
	public class VScrollTabTag:ScrollTabTag{
		
		/// <summary>The y location of the mouse in pixels from the top when it clicked.</summary>
		public int MouseY;
		/// <summary>The start y location of the tab when the mouse clicked it.</summary>
		public int StartY;
		
		
		public override string[] GetTags(){
			return new string[]{"vscrolltab"};
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new VScrollTabTag();
		}
		
		public override void Clicked(UIEvent clickEvent){
			// Track where the mouse started off:
			MouseY=clickEvent.clientY;
			
			// And where the bar started off too:
			StartY=Element.style.Computed.PositionTop;
		}
		
		protected override void SetTabSize(int newSize){
			Element.style.height=newSize+"fpx";
			ScrollBy(0,true,true);
		}
		
		public override int BarSize(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			return ScrollBar.Element.style.Computed.InnerHeight-SizeBefore()-SizeAfter();
		}
		
		public override int TabSize(){
			return Element.style.Computed.InnerHeight;
		}
		
		public override int StyleSize(){
			// Grab the computed style:
			ComputedStyle style=Element.style.Computed;
			
			// Return the vertical style size. That's:
			return style.PixelHeight-style.InnerHeight;
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
			
			return firstChild.style.Computed.PixelHeight;
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
			
			return lastChild.style.Computed.PixelHeight;
		}
		
		public override int TabPosition(){
			return Element.style.Computed.PositionTop;
		}
		
		public override void OnMouseMove(UIEvent mouseEvent){
			int deltaY=mouseEvent.clientY-MouseY;
			
			if(deltaY==0){
				return;
			}
			
			ScrollBy(deltaY,false,true);
		}
		
		public override void ScrollTo(int location,bool scrollTarget){
			
			StartY=0;
			ScrollBy(location,false,scrollTarget);
			
		}
		
		public override void ScrollBy(int deltaY,bool fromCurrent,bool scrollTarget){
			// Scroll it by deltaY from StartY.
			int newLocation=deltaY;
			
			ComputedStyle style=Element.style.Computed;
			
			if(fromCurrent){
				newLocation+=style.PositionTop;
			}else{
				newLocation+=StartY;
			}
			
			// Get the size of the button before the tab:
			int sizeBefore=SizeBefore();
			
			int barSize=BarSize();
			
			int max=barSize+sizeBefore-style.PixelHeight;
			
			if(newLocation<sizeBefore){
				newLocation=sizeBefore;
			}else if(newLocation>max){
				newLocation=max;
			}
			
			if(newLocation==style.PositionTop){
				return;
			}
			
			Element.style.top=newLocation+"fpx";
			
			if(scrollTarget){
				
				if(ScrollBar.DivertOutput){
					
					int tabSize=style.PixelHeight;
					
					float progress=(float)(newLocation-sizeBefore)/(float)(barSize-tabSize);
					
					ScrollBar.OnScrolled(progress);
					
				}else{
				
					Element target=ScrollBar.GetTarget();
					
					if(target!=null){
						float progress=(float)(newLocation-sizeBefore)/(float)barSize;
						
						target.style.Computed.ScrollTop=((int)(progress * target.style.Computed.ContentHeight));
						// Recompute the size:
						target.style.Computed.SetSize();
						// And request a redraw:
						target.Document.Renderer.RequestLayout();
					}
				
				}
				
			}
			
		}
		
	}
	
}