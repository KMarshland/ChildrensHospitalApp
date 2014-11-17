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
	/// Represents the bold tag.
	/// </summary>
	
	public class BoldTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"b"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BoldTag();
		}
		
	}
	
}