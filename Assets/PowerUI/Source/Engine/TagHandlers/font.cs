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
	/// Handles the standard inline font element.
	/// </summary>
	
	public class FontTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"font"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new FontTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			ComputedStyle computed=Element.Style.Computed;
			
			string value=Element[property];
			if(value==null){
				value="";
			}
			
			if(property=="color"){
				computed.ChangeTagProperty("color",new Css.Value(value,Css.ValueType.Color));
			}else if(property=="size"){
				if(value!=""){
					value+="px";
				}
				computed.ChangeTagProperty("font-size",new Css.Value(value));
			}else if(property=="face"){
				computed.ChangeTagProperty("font-family",new Css.Value(value,Css.ValueType.Text));
			}else{
				return false;
			}
			
			return true;
		}
		
	}
	
}