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
	/// Represents a HTML blockquote element.
	/// </summary>
	
	public class BlockquoteTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"blockquote"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BlockquoteTag();
		}
		
	}
	
}