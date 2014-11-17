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
	/// Handles the mark tag.
	/// </summary>

	public class MarkTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"mark"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new MarkTag();
		}
		
	}
	
}