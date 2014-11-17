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
	/// Handles the rt tag.
	/// </summary>

	public class RTTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"rt"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new RTTag();
		}
		
	}
	
}