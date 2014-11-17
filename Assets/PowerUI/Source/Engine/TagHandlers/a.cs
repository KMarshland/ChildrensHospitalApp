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
using PowerUI.Css;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// Represents a clickable link. Note that target is handled internally by the http protocol.
	/// </summary>
	
	public class ClickLinkTag:HtmlTagHandler{
		
		/// <summary>The target url that should be loaded when clicked.</summary>
		public string Href;
		
		
		public ClickLinkTag(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"a"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ClickLinkTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="href"){
				Href=Element["href"];
				return true;
			}
			
			return false;
		}
		
		public override bool OnClick(UIEvent clickEvent){
			base.OnClick(clickEvent);
			
			if(!clickEvent.heldDown){
				// Time to go to our Href.
				
				if(!string.IsNullOrEmpty(Href)){
					FilePath path=new FilePath(Href,Element.Document.basepath,false);
					
					// Do we have a file protocol handler available?
					FileProtocol fileProtocol=path.Handler;
					
					if(fileProtocol!=null){
						fileProtocol.OnFollowLink(Element,path);
					}
				}
				
				clickEvent.stopPropagation();
			}
			
			return true;
		}
		
	}
	
}