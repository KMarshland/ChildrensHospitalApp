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
	/// Represents a small element.
	/// </summary>
	
	public class SmallTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"small"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SmallTag();
		}
		
	}
	
}