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
	/// Represents a standard div block element.
	/// </summary>
	
	public class DivTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"div"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DivTag();
		}
		
	}
	
}