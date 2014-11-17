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
	/// Represents an in-html comment which will be completely ignored.
	/// </summary>
	
	public class CommentTag:HtmlTagHandler{
		
		public CommentTag(){
			IgnoreSelfClick=true;
		}
		
		public override bool Junk(){
			// Prevents these tags from entering the DOM.
			return true;
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override string[] GetTags(){
			return new string[]{"?xml"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new CommentTag();
		}
		
	}
	
}