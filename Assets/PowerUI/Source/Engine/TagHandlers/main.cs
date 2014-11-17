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
	/// Represents a HTML5 main content block element.
	/// </summary>
	
	public class MainTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"main"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new MainTag();
		}
		
	}
	
}