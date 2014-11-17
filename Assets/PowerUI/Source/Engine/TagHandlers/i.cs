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
	/// Handles the italics tag.
	/// </summary>

	public class ItalicTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"i"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ItalicTag();
		}
		
	}
	
}