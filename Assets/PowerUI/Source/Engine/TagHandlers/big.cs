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
	/// Represents a big element.
	/// </summary>
	
	public class BigTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"big"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BigTag();
		}
		
	}
	
}