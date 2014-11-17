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
	/// Represents a HTML strike element. Strikes through this text.
	/// </summary>
	
	public class StrikeTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"strike"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new StrikeTag();
		}
		
	}
	
}