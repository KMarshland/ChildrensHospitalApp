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
	/// Represents a standard ordered list element.
	/// </summary>
	
	public class OlTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"ol"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new OlTag();
		}
		
	}
	
}