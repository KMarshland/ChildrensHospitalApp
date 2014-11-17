using System;
using System.Collections;
using System.Collections.Generic;

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
	/// Represents HTML5 audio sources.
	/// </summary>

	public class SourceTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"source"};
		}
		
		public override bool SelfClosing(){ 
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new SourceTag();
		}
		
	}
	
}