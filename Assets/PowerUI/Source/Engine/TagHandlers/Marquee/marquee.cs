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
using PowerUI.Css;


namespace PowerUI{
	
	/// <summary>
	/// Represents a non-standard marquee tag.
	/// </summary>
	
	public class MarqueeTag:HtmlTagHandler{
		
		/// <summary>True when this marquee is active.</summary>
		public bool Active;
		/// <summary>The amount of times this will loop.</summary>
		public int Loop=-1;
		/// <summary>The underlying timer which causes the scrolling.</summary>
		private UITimer Timer;
		/// <summary>The amount of pixels a scroll will cause.</summary>
		public int ScrollAmount=6;
		/// <summary>The time in milliseconds between scrolls.</summary>
		public int ScrollDelay=85;
		/// <summary>How this marquee scrolls.</summary>
		public MarqueeBehaviour Behaviour=MarqueeBehaviour.Scroll;
		/// <summary>The direction of the marquee.</summary>
		public MarqueeDirection Direction=MarqueeDirection.Left;
		
		
		/// <summary>Call this to begin a marquee.</summary>
		public void Start(){
			
			if(Active){
				return;
			}
			
			Active=true;
			
			// Start our timer:
			Timer=new UITimer(false,ScrollDelay,OnTick);
			
			Element.Run("onstart");
			
		}
		
		/// <summary>Call this to stop a scrolling marquee.</summary>
		public void Stop(){
			
			if(!Active){
				return;
			}
			
			Active=false;
			
			// Stop and clear the timer:
			Timer.Stop();
			
			Timer=null;
			
			Element.Run("onstop");
			
		}
		
		private void OnTick(){
			
			// Grab the computed style:
			ComputedStyle style=Element.style.Computed;
			
			int amount=ScrollAmount;
			
			// Is it odd? If so, it travels in the inverted direction.
			if(((int)Direction&1)==1){
				
				// Invert the direction:
				amount=-ScrollAmount;
				
			}
			
			if((int)Direction<=2){
				
				// Vertical scroll:
				style.ScrollTop+=amount;
			
				// Grab the content height:
				int contentHeight=style.ContentHeight;
				
				// Grab the parent height:
				int height=style.InnerHeight;
				
				switch(Behaviour){
					
					case MarqueeBehaviour.Scroll:
						
						if(style.ScrollTop<-height){
							
							// Wrap:
							style.ScrollTop=contentHeight;
							
							Wrapped();
							
						}else if(style.ScrollTop>contentHeight){
							
							// Wrap:
							style.ScrollTop=-height;
							
							Wrapped();
							
						}
						
					break;
					
					case MarqueeBehaviour.Alternate:
						
						int minimum=-(height-contentHeight);
						
						if(minimum>=0){
							
							// No space to bounce anyway.
							return;
							
						}
						
						if(style.ScrollTop>0){
							
							// Reset:
							style.ScrollTop=0;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Up){
								
								Direction=MarqueeDirection.Down;
								
							}else{
								
								Direction=MarqueeDirection.Up;
								
							}
							
							Bounced();
							
						}else if(style.ScrollTop<minimum){
							
							style.ScrollTop=minimum;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Up){
								
								Direction=MarqueeDirection.Down;
								
							}else{
								
								Direction=MarqueeDirection.Up;
								
							}
							
							Bounced();
							
						}
						
					break;
					
				}
				
			}else{
				
				// Horizontal scroll:
				style.ScrollLeft+=amount;
				
				// Grab the content width:
				int contentWidth=style.ContentWidth;
				
				// Grab the parent width:
				int width=style.InnerWidth;
				
				switch(Behaviour){
					
					case MarqueeBehaviour.Scroll:
						
						if(style.ScrollLeft<-width){
							
							// Wrap:
							style.ScrollLeft=contentWidth;
							
							Wrapped();
							
						}else if(style.ScrollLeft>contentWidth){
							
							// Wrap:
							style.ScrollLeft=-width;
							
							Wrapped();
							
						}
						
					break;
					
					case MarqueeBehaviour.Alternate:
						
						int minimum=-(width-contentWidth);
						
						if(minimum>=0){
							
							// No space to bounce anyway.
							return;
							
						}
						
						if(style.ScrollLeft>0){
							
							// Reset:
							style.ScrollLeft=0;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Left){
								
								Direction=MarqueeDirection.Right;
								
							}else{
								
								Direction=MarqueeDirection.Left;
								
							}
							
							Bounced();
							
						}else if(style.ScrollLeft<minimum){
							
							style.ScrollLeft=minimum;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Left){
								
								Direction=MarqueeDirection.Right;
								
							}else{
								
								Direction=MarqueeDirection.Left;
								
							}
							
							Bounced();
							
						}
						
					break;
					
				}
				
			}
			
			// The below block of code comes from inside the scrollTo function:
			
			// Recompute the size:
			style.SetSize();
			
			// And request a redraw:
			style.RequestLayout();
			
			if(Element.VScrollbar){
				Element.VerticalScrollbar.ElementScrolled();
			}else if(Element.HScrollbar){
				Element.HorizontalScrollbar.ElementScrolled();
			}
			
		}
		
		/// <summary>Called when the marquee bounces the content.</summary>
		private void Bounced(){
			
			// Trigger:
			Element.Run("onbounce");
			
			// Consider looping too:
			Wrapped();
			
		}
		
		/// <summary>Called when the marquee wraps.</summary>
		private void Wrapped(){
			
			if(Loop==-1){
				return;
			}
			
			Loop--;
			
			if(Loop==0){
				
				// Stop the marquee:
				Stop();
				
				// Fire the finish event:
				Element.Run("onfinish");
				
			}
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="loop"){
				
				Loop=int.Parse(Element["loop"]);
				
				if(Loop==0){
					Loop=1;
				}else if(Loop<0){
					Loop=-1;
				}
				
			}else if(property=="scrollamount"){
				
				ScrollAmount=int.Parse(Element["scrollamount"]);
				
			}else if(property=="scrolldelay"){
				
				ScrollDelay=int.Parse(Element["scrolldelay"]);
				
				if(ScrollDelay<50){
					
					// No super fast scrolling - it's too distracting. Use animate for effects like that.
					
					ScrollDelay=50;
					
				}
				
			}else if(property=="behaviour"){
				
				ApplyBehaviour(Element["behaviour"]);
				
			}else if(property=="behavior"){
				
				ApplyBehaviour(Element["behavior"]);
				
			}else if(property=="direction"){
				
				// Grab the direction:
				string direction=Element["direction"];
				
				switch(direction){
					
					case "left":
						Direction=MarqueeDirection.Left;
					break;
					
					case "right":
						Direction=MarqueeDirection.Right;
					break;
					
					case "up":
						Direction=MarqueeDirection.Up;
					break;
					
					case "down":
						Direction=MarqueeDirection.Down;
					break;
					
				}
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		private void ApplyBehaviour(string behaviour){
			
			switch(behaviour){
				
				case "scroll":
					Behaviour=MarqueeBehaviour.Scroll;
				break;
				
				case "slide":
					Behaviour=MarqueeBehaviour.Slide;
				break;
				
				case "alternate":
					Behaviour=MarqueeBehaviour.Alternate;
				break;
				
			}
			
		}
		
		public override string[] GetTags(){
			return new string[]{"marquee"};
		}
		
		public override void OnChildrenLoaded(){
			
			Start();
			
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new MarqueeTag();
		}
		
	}
	
}