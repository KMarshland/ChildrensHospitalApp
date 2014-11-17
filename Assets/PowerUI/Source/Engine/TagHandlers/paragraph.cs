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
	/// Represents a standard paragraph (p) block element.
	/// </summary>
	
	public class ParagraphTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"p"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ParagraphTag();
		}
		
	}
	
}