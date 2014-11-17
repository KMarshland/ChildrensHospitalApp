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
	/// Handles the meta tag. These are essentially just ignored by PowerUI.
	/// </summary>
	
	public class MetaTag:HtmlTagHandler{
	
		public override bool SelfClosing(){
			return true;
		}
		
		public override string[] GetTags(){
			return new string[]{"meta"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new MetaTag();
		}
		
	}
	
}