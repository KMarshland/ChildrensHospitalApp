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
	/// Handles the definition tag.
	/// </summary>

	public class DefinitionTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"dfn"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DefinitionTag();
		}
		
	}
	
}