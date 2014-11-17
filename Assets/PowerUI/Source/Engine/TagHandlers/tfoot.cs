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
	/// Represents a table footer element.
	/// </summary>
	
	public class TableFooterTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"tfoot"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TableFooterTag();
		}
		
	}
	
}