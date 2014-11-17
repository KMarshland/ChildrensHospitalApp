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
	/// Represents HTML5 video tracks.
	/// </summary>

	public class TrackTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"track"};
		}
		
		public override bool SelfClosing(){ 
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TrackTag();
		}
		
	}
	
}