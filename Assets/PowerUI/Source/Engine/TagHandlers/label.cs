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
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// Handles the standard inline label element.
	/// Clicking on them acts just like clicking on the input they target.
	/// </summary>
	
	public class LabelTag:HtmlTagHandler{
		
		/// <summary>The ID of the element the clicks of this get 'directed' at.
		/// If blank/null, the first child of this element that is an 'input' is used.</summary>
		public string ForElement;
		
		public override string[] GetTags(){
			return new string[]{"label"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new LabelTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			if(property=="for"){
				ForElement=Element["for"];
				return true;
			}
			return false;
		}
		
		/// <summary>Gets the element this label is for. If found, it should always be an input.</summary>
		public Element GetFor(){
			if(string.IsNullOrEmpty(ForElement)){
				// Use the first child that is an 'input' element.
				List<Element> kids=Element.childNodes;
				if(kids!=null){
					for(int i=0;i<kids.Count;i++){
						Element child=kids[i];
						if(child!=null && child.Tag=="input"){
							// Got it! Stop right there.
							return child;
						}
					}
				}
			}else{
				// ForElement is an ID - lets go find the element in the document with that ID.
				return Element.Document.getElementById(ForElement);
			}
			
			return null;
		}
		
		public override bool OnClick(UIEvent clickEvent){
			// Who wants the click? That's the for element:
			Element forElement=GetFor();
			
			if(forElement!=null){
				// Prevent any propagation - we sure don't want it clicking this element again (which may occur if
				// forElement is one of this elements kids and it propagated upwards).
				clickEvent.stopPropagation();
				// Click it:
				forElement.GotClicked(clickEvent);
			}
			// Run this elements onmousedown/up etc.
			base.OnClick(clickEvent);
			
			return true;
		}
		
	}
	
}