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
	/// Represents a HTML5 summary element.
	/// </summary>
	
	public class SummaryTag:HtmlTagHandler{
		
		/// <summary>The details element this affects when clicked.</summary>
		public Element Details;
		
		
		public override string[] GetTags(){
			return new string[]{"summary"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SummaryTag();
		}
		
		
		public override bool OnClick(UIEvent clickEvent){
			base.OnClick(clickEvent);
			
			if(Details==null){
				return false;
			}
				
			if(!clickEvent.heldDown){
				// Hide/show the details element.
				
				// Grab the details computed style:
				ComputedStyle computed=Details.Style.Computed;
				
				// The display it's going to:
				string display;
				
				// Is it currently visible?
				if(computed.Display==DisplayType.None){
					// Nope! Show it.
					display="block";
				}else{
					// Yep - hide it.
					display="none";
				}
				
				// Change it's display:
				computed.ChangeTagProperty("display",new Css.Value(display,Css.ValueType.Text));
				
			}
			
			return true;
		}
		
	}
	
}