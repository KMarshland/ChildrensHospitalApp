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
	/// Handles the standard inline span element.
	/// </summary>
	
	public class SpanTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"span"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SpanTag();
		}
		
	}
	
}