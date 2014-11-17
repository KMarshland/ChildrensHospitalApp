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
	
	public class HeadTag:HtmlTagHandler{
		
		/// <summary>
		/// Represents the html head tag. Not required by PowerUI.
		/// </summary>
		
		public HeadTag(){
			IgnoreSelfClick=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"head"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new HeadTag();
		}
		
	}
	
}