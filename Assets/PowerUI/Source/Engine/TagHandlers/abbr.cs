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
	/// Represents a HTML abbr(eviation) element.
	/// </summary>
	
	public class AbbrTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"abbr"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new AbbrTag();
		}
		
	}
	
}