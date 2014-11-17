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
	/// Represents a HTML cite element.
	/// </summary>
	
	public class CiteTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"cite"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new CiteTag();
		}
		
	}
	
}