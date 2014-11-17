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
	/// Handles the HTML5 figure caption element.
	/// </summary>
	
	public class FigCaptionTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"figcaption"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new FigCaptionTag();
		}
		
	}
	
}