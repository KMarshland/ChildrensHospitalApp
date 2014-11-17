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

using System;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// Dropdown box. Used by select internally.
	/// </summary>
	
	public class DDBoxTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"ddbox"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DDBoxTag();
		}
		
		public override void OnTagLoaded(){
			Element.Document.DropdownBox=Element;
		}
		
		public override bool OnClick(UIEvent clickEvent){
			
			// No bubbling:
			clickEvent.stopPropagation();
			
			return true;
		}
		
		
	}
	
}