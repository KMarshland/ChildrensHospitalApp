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
	
	public class H6Tag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"h6"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new H6Tag();
		}
		
	}
	
}