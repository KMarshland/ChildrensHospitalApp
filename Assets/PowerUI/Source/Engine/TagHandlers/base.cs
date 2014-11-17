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
	/// Represents a HTML base element.
	/// </summary>
	
	public class BaseTag:HtmlTagHandler{
		
		public override bool SelfClosing(){ 
			return true;
		}
		
		public override string[] GetTags(){
			return new string[]{"base"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BaseTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="href"){
				string href=Element["href"];
				
				// Change the documents base path:
				Element.document.location.basepath=href;
				
				return true;
			}else if(property=="target"){
				string target=Element["href"];
				
				// Change the documents target override:
				Element.document.location.target=target;
			}
			
			
			return false;
		}
		
	}
	
}