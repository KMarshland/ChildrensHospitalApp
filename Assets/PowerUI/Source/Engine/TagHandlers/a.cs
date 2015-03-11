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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using PowerUI.Css;
using System.Collections;
using System.Collections.Generic;


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
				
				#if MOBILE || UNITY_METRO
				
				// First, look for <source> elements.
				
				// Grab the kids:
				List<Element> kids=Element.childNodes;
				
				if(kids!=null){
					// For each child, grab it's src value. Favours the most suitable protocol for this platform (e.g. market:// on android).
					
					foreach(Element child in kids){
						
						if(child.Tag!="source"){
							continue;
						}
						
						// Grab the src:
						string childSrc=child["src"];
						
						if(childSrc==null){
							continue;
						}
						
						// Get the optional type - it can be Android,W8,IOS,Blackberry:
						string type=child["type"];
						
						if(type!=null){
							type=type.Trim().ToLower();
						}
						
						#if UNITY_ANDROID
							
							if(type=="android" || childSrc.StartsWith("market:")){
								
								Href=childSrc;
								
							}
							
						#elif UNITY_WP8 || UNITY_METRO
						
							if(type=="w8" || type=="wp8" || type=="windows" || childSrc.StartsWith("ms-windows-store:")){
								
								Href=childSrc;
								
							}
							
						#elif UNITY_IPHONE
							
							if(type=="ios" || childSrc.StartsWith("itms:") || childSrc.StartsWith("itms-apps:")){
								
								Href=childSrc;
								
							}
							
							
						#endif
						
					}
					
				}
				
				#endif
					
				
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