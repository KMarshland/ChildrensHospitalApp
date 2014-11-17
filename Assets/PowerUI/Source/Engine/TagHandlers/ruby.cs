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
	/// Handles the ruby tag.
	/// </summary>

	public class RubyTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"ruby"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new RubyTag();
		}
		
	}
	
}