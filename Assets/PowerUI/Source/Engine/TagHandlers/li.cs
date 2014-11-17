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
	/// Represents a standard list entry element.
	/// </summary>
	
	public class LiTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"li"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new LiTag();
		}
		
	}
	
}