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
	/// Represents the center tag.
	/// </summary>
	
	public class CenterTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"center"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new CenterTag();
		}
		
	}
	
}