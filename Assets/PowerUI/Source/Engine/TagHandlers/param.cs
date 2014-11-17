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
	/// Represents object parameters. Although object itself isn't supported
	/// this tag is; a page with an object on it can still load.
	/// </summary>

	public class ParamTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"param"};
		}
		
		public override bool SelfClosing(){ 
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ParamTag();
		}
		
	}
	
}