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
	/// Handles the title tag.
	/// Note that the title is set to <see cref="PowerUI.Document.title"/> if you wish to use it.
	/// </summary>
	
	public class TitleTag:HtmlTagHandler{
	
		public override string[] GetTags(){
			return new string[]{"title"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TitleTag();
		}
		
		public override void OnTagLoaded(){
			Element.Document.title=Element.innerHTML;
		}
		
	}
	
}