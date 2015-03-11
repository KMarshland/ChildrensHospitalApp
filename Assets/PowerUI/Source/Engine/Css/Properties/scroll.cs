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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// An animatable scroll-top and scroll-left CSS property.
	/// </summary>
	
	public class Scroll:CssProperty{
		
		
		public Scroll(){
			
			// This is a rect property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"scroll"};
		}
		
		public override void SetDefault(Value value,ValueType type){
			
			value.Set("",Type);
			
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			int previousTop=style.ScrollTop;
			int previousLeft=style.ScrollLeft;
			
			if(value==null){
				// Clear scroll:
				style.ScrollTop=0;
				style.ScrollLeft=0;
			}else{
				
				// (CSS order T,L)
				
				// Top is #0:
				style.ScrollTop=value.GetPX(0);
				
				// Left is #3:
				style.ScrollLeft=value.GetPX(3);
				
			}
			
			if(style.ScrollLeft==previousLeft && style.ScrollTop==previousTop){
				
				return;
				
			}
			
			// Grab the element:
			Element element=style.Element;
			
			// The below block of code comes from inside the scrollTo function:
			
			// Recompute the size:
			style.SetSize();
			
			// And request a redraw:
			style.RequestLayout();
			
			if(element.VScrollbar){
				element.VerticalScrollbar.ElementScrolled();
			}else if(element.HScrollbar){
				element.HorizontalScrollbar.ElementScrolled();
			}
			
		}
		
	}
	
}



