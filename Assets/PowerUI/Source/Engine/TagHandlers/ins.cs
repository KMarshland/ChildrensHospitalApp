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
	/// Represents a HTML ins(erted) element. Underlines the new text.
	/// </summary>
	
	public class InsTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"ins"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new InsTag();
		}
		
	}
	
}