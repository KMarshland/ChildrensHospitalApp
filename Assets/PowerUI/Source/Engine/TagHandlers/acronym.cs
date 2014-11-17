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
	/// Represents a HTML acronym element. It's reccommended to use abbr instead.
	/// </summary>
	
	public class AcronymTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"acronym"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new AcronymTag();
		}
		
	}
	
}