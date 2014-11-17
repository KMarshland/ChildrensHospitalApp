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
	/// Represents a standard unordered list element.
	/// </summary>
	
	public class UlTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"ul"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new UlTag();
		}
		
	}
	
}