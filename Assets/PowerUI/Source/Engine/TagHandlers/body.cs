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
	/// Represents the body tag. Note that this is added automatically by PowerUI and isn't required.
	/// </summary>
	
	public class BodyTag:HtmlTagHandler{
		
		public BodyTag(){
			IgnoreSelfClick=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"body"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BodyTag();
		}
		
		public override void OnTagLoaded(){
			Element.Document.body=Element;
		}
		
	}
	
}