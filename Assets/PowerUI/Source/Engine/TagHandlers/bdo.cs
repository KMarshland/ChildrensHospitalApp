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
	/// Represents a HTML5 Bi-direction override element.
	/// </summary>
	
	public class BdoTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"bdo"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BdoTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="dir"){
				// Grab the direction:
				string direction=Element["dir"];
				
				// Grab the computed style:
				ComputedStyle computed=Element.style.Computed;
				
				// Apply it to CSS - it's exactly the same value for the direction CSS property:
				computed.ChangeTagProperty("direction",new Css.Value(direction,Css.ValueType.Text));
				
				return true;
			}
			
			
			return false;
		}
	}
	
}