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
	/// Represents a HTML5 Bi-direction isolation element.
	/// </summary>
	
	public class BdiTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"bdi"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BdiTag();
		}
		
	}
	
}