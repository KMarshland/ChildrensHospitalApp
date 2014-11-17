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
	/// Represents a standard horizontal rule element.
	/// </summary>
	
	public class HRTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"hr"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new HRTag();
		}
		
	}
	
}