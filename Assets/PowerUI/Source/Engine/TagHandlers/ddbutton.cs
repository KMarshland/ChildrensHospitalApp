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
	/// Dropdown button tag. Used by select internally - when clicked, it displays the dropdown menu.
	/// </summary>
	
	public class DDButtonTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"ddbutton"};
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DDButtonTag();
		}
		
		public override void OnTagLoaded(){
			Element.innerHTML="v";
		}
		
	}
	
}