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
	/// Represents a standard dir block element.
	/// </summary>
	
	public class DirTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"dir"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DirTag();
		}
		
	}
	
}