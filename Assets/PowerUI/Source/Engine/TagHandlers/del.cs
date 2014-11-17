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
	/// Represents a HTML del(eted) element. These get striked through.
	/// </summary>
	
	public class DelTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"del"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DelTag();
		}
		
	}
	
}