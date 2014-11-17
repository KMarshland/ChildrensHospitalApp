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
	/// Handles the standard sub(script) element.
	/// </summary>
	
	public class SubTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"sub"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SubTag();
		}
		
	}
	
}