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
	/// Represents the strong tag.
	/// </summary>
	
	public class StrongTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"strong"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new StrongTag();
		}
		
	}
	
}