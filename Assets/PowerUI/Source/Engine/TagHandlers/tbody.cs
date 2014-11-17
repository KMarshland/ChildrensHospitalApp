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
	/// Represents a table body element.
	/// </summary>
	
	public class TableBodyTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"tbody"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TableBodyTag();
		}
		
	}
	
}