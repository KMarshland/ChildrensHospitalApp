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

using PowerUI.Css;


namespace PowerUI{

	/// <summary>
	/// Represents line breaks.
	/// </summary>

	public class BrTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"br"};
		}
		
		public override bool SelfClosing(){ 
			// This makes <br> useable as well as <br/>
			return true;
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new BrTag();
		}
		
		public override void OnTagLoaded(){
			// If we follow a br, we have height, otherwise just act like a block element.
			Element previousElement=Element.previousSibling;
			
			if(previousElement!=null && previousElement.computedStyle.Display == Css.DisplayType.Block){
				// Act like we have content. 
				// This will enforce the height to be set of the tag when fontsize changes.
				
				// Get the content CSS property:
				CssProperty property=CssProperties.Get("content");
				
				if(property!=null){
					property.Apply(Element.style.Computed,null);
				}
			}
			
			base.OnTagLoaded();
		}
		
	}
	
}