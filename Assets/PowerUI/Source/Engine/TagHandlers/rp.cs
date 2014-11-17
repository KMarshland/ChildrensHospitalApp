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
	/// Handles the rp tag.
	/// </summary>

	public class RPTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"rp"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new RPTag();
		}
		
	}
	
}