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
	/// Represents a standard header block element.
	/// </summary>
	
	public class H4Tag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"h4"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new H4Tag();
		}
		
	}
	
}