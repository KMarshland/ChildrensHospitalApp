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
	/// Handles the scroll down button on scrollbars.
	/// </summary>
	
	public class ScrollDownTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"scrolldown"};
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ScrollDownTag();
		}
		
		public override bool OnClick(UIEvent clickEvent){
			// Get the scroll bar:
			InputTag scroll=((InputTag)(Element.parentNode.Handler));
			// And scroll it:
			scroll.ScrollBy(1);
			// Prevent bubbling:
			clickEvent.stopPropagation();
			
			return true;
		}
		
	}
	
}