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
	/// Represents a HTML5 s element. Defines text which is no longer correct and is striked through.
	/// </summary>
	
	public class STag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"s"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new STag();
		}
		
	}
	
}