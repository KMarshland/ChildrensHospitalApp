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
	/// Handles the standard inline time element.
	/// </summary>
	
	public class TimeTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"time"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TimeTag();
		}
		
	}
	
}