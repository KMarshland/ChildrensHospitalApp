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
	/// Represents a HTML5 details element.
	/// </summary>
	
	public class DetailsTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"details"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new DetailsTag();
		}
		
		public override void OnChildrenLoaded(){
			// Grab the summary element:
			Element summary=Element.getElementByTagName("summary");
			
			if(summary!=null){
				// Pop it out:
				summary.parentNode.removeChild(summary);
				
				// Grab the summary tag handler:
				SummaryTag summaryTag=(SummaryTag)summary.GetHandler();
				
				// Set this details element to it:
				summaryTag.Details=Element;
				
				// Insert it as a child alongside this details tag:
				Element.parentNode.appendChild(summary);
				
				// We know for sure that summary is the last element, and this details
				// tag is immediately after it. Their the wrong way around, so simply flip them over:
				List<Element> children=Element.parentNode.childNodes;
				
				// Whats the index of the last child?
				int last=children.Count-1;
				
				// The last one is now Element:
				children[last]=Element;
				
				// And the one before it is summary:
				children[last-1]=summary;
				
			}
			
			// And handle style/ other defaults:
			base.OnTagLoaded();
		}
		
	}
	
}