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
	/// Represents a table header element.
	/// </summary>
	
	public class TableHeaderTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"thead"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TableHeaderTag();
		}
		
	}
	
}