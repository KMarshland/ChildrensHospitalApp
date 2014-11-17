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
	/// Represents line break opportunities.
	/// </summary>

	public class WbrTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"wbr"};
		}
		
		public override bool SelfClosing(){ 
			// This makes <wbr> useable as well as <wbr/>
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new WbrTag();
		}
		
	}
	
}