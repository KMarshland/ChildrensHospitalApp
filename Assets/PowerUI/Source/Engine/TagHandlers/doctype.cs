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
	/// Represents the doctype tag.
	/// </summary>
	
	public class DoctypeTag:HtmlTagHandler{
		
		public DoctypeTag(){
			IgnoreSelfClick=true;
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override string[] GetTags(){
			return new string[]{"doctype","!doctype"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DoctypeTag();
		}
		
	}
	
}