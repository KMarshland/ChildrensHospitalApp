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
	/// Represents a HTML5 description element.
	/// </summary>
	
	public class DescriptionDTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"dd"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DescriptionDTag();
		}
		
	}
	
}