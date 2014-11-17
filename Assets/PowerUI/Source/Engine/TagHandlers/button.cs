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
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{
	
	/// <summary>
	/// Represents a html button. This is the same as &lt;input type="button"&gt;, but a little shorter.
	/// </summary>
	
	public class ButtonTag:HtmlTagHandler{
		
		/// <summary>The value text for this button.</summary>
		public string Value;
		
		
		public ButtonTag(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"button"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ButtonTag();
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="value"){
				SetValue(Element["value"]);
				return true;
			}else if(property=="content"){
				SetValue(Element["content"],true);
				return true;
			}
			
			return false;
		}
		
		/// <summary>Sets the value of this button.</summary>
		/// <param name="value">The value to set. Note that HTML is stripped here.</param>
		public void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this button, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		public void SetValue(string value,bool html){
			
			Element["value"]=Value=value;
			
			if(html){
				Element.innerHTML=value;
			}else{
				Element.textContent=value;
			}
			
		}
		
		public override bool OnClick(UIEvent clickEvent){
			
			base.OnClick(clickEvent);
			clickEvent.stopPropagation();
			
			return true;
		}
		
	}
	
}