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
	/// Represents a standard legend element.
	/// </summary>
	
	public class LegendTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"legend"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new LegendTag();
		}
		
	}
	
}