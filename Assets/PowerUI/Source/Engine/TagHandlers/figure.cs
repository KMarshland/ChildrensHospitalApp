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
	/// Represents a HTML figure element.
	/// </summary>
	
	public class FigureTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"figure"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new FigureTag();
		}
		
	}
	
}