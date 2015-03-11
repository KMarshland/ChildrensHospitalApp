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
	/// Represents the input tag which handles various types of input on forms.
	/// Note that all input tags are expected to be on a form to work correctly.
	/// E.g. radio buttons won't work if they are not on a form.
	/// Supports the type, name, value and checked attributes.
	/// Also supports a 'content' attribute which accepts a value as html; great for buttons.
	/// </summary>
	
	public class InputTag:HtmlTagHandler{
		
		/// <summary>Used by password inputs. True if this input's value is hidden.</summary> 
		public bool Hidden;
		/// <summary>The value text for this input.</summary>
		public string Value;
		/// <summary>For boolean (radio or checkbox) inputs, this is true if this one is checked.</summary>
		public bool Checked;
		/// <summary>For text or password input boxes, this is the cursor.</summary>
		public Element Cursor;
		/// <summary>The type of input that this is; e.g. text/password/radio etc.</summary>
		public InputType Type;
		/// <summary>The current location of the cursor.</summary>
		public int CursorIndex;
		/// <summary>True if the cursor should be located after the next update.</summary>
		public bool LocateCursor;
		/// <summary>Used by scrollbars. This is the name of the target element that will be
		/// scrolled if there is one. Parent is used otherwise.</summary>
		public string TargetName;
		/// <summary>Used by scrollbars. This is the element that will get scrolled.</summary>
		public Element TargetElement;
		/// <summary>Used by scrollbars. The tag handler for the scrollbars tab.</summary>
		public ScrollTabTag ScrollTab;
		/// <summary>The maximum length of text in this box.</summary>
		public int MaxLength=int.MaxValue;
		/// <summary>Set this true in an overriding class to receive scrolling progress. See OnScrolled.</summary>
		public bool DivertOutput;
		
		public InputTag(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		/// <summary>Used by e.g. sliders. This receives the scrolling progress if DivertOutput is true.</summary>
		public virtual void OnScrolled(float progress){}
		
		public override string[] GetTags(){
			return new string[]{"input"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new InputTag();
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				string type=Element["type"];
				if(type==null){
					type="text";
				}
				// Change the style. This requests a layout internally.
				Element.Style.Computed.SetSelector(Css.SelectorType.Tag,"input[type=\""+type+"\"]");
				
				if(type=="radio"){
					Type=InputType.Radio;
				}else if(type=="checkbox"){
					Type=InputType.Checkbox;
				}else if(type=="vscroll"){
					Type=InputType.VScroll;
					Element.innerHTML="<scrollup><vscrolltab><scrolldown>";
				}else if(type=="hscroll"){
					Type=InputType.HScroll;
					Element.innerHTML="<scrollleft><hscrolltab><scrollright>";
				}else if(type=="submit"){
					Type=InputType.Submit;
					SetValue("Submit");
				}else if(type=="button"){
					Type=InputType.Button;
				}else if(type=="hidden"){
					Type=InputType.Hidden;
				}else{
					Type=InputType.Text;					
					Hidden=(type=="password");
				}
				
				return true;
			}else if(property=="maxlength"){
				
				string value=Element["maxlength"];
				
				if(string.IsNullOrEmpty(value)){
					// It's blank - set it to the default.
					MaxLength=int.MaxValue;
				}else{
					// Parse the maximum length from the string:
					if(int.TryParse(value,out MaxLength)){
						// Clip the value if we need to:
						if(Value!=null && Value.Length>MaxLength){
							SetValue(Value);
						}
					}else{
						// Not a number!
						MaxLength=int.MaxValue;
					}
				}
				
				return true;
			}else if(property=="target"){
				TargetName=Element["target"];
				TargetElement=null;
				return true;
			
			}else if(property=="checked"){
				
				// Get the checked state:
				string state=Element["checked"];
				
				// Awkwardly, null/ empty is checked.
				// 0 or false are not checked, anything else is!
				
				if( string.IsNullOrEmpty(state) ){
					
					Select();
					
				}else{
					state=state.ToLower().Trim();
					
					if(state=="0" || state=="false"){
						
						Unselect();
						
					}else{
						
						Select();
						
					}
					
				}
				
				RequestLayout();
				return true;
			}else if(property=="value"){
				SetValue(Element["value"]);
				return true;
			}else if(property=="content"){
				SetValue(Element["content"],true);
				return true;
			}
			return false;
		}
		
		public override KeyboardMode OnShowMobileKeyboard(){
			if(!IsTextInput()){
				return null;
			}
			
			KeyboardMode result=new KeyboardMode();
			result.Secret=Hidden;
			return result;
		}
		
		/// <summary>Used by boolean inputs (checkbox/radio). Unselects this from being the active one.</summary>
		public void Unselect(){
			if(!Checked){
				return;
			}
			
			Checked=false;
			
			// Clear checked:
			Element["checked"]="0";
			
			if(Type==InputType.Checkbox){
				SetValue(null);
			}
			
			Element.innerHTML="";
		}
		
		/// <summary>Used by boolean inputs (checkbox/radio). Selects this as the active one.</summary>
		public void Select(){
			if(Checked){
				return;
			}
			Checked=true;
			
			// Set checked:
			Element["checked"]="1";
			
			if(Type==InputType.Radio){
				// Firstly, unselect all other radio elements with this same name:
				string name=Element["name"];
				
				if(Element.form!=null){
					
					List<Element> allInputs=Element.form.GetAllInputs();
					
					foreach(Element element in allInputs){
						if(element==Element){
							// Skip this element
							continue;
						}
						
						if(element["type"]=="radio"){
							// Great, got one - same name?
							
							if(element["name"]==name){
								// Yep; unselect it.
								((InputTag)(element.Handler)).Unselect();
							}
							
						}
						
					}
					
				}
				
				Element.Run("onchange");
				
				Element.innerHTML="<div style='width:60%;height:60%;background:#ffffff;border-radius:4px;'></div>";
				
			}else if(Type==InputType.Checkbox){
				SetValue("1");
				
				Element.innerHTML="x";
			}
			
		}
		
		public override void OnTagLoaded(){
			if(IsScrollInput()){
				RecalculateTab();
			}
		}
		
		/// <summary>Scrolls a scrollbar by the given number of pixels.
		/// This may fail if it's already at an end of the bar and can't move any further.</summary>
		/// <param name="pixels">The number of pixels the scrollbar tab will try to move by.</param>
		public void ScrollBy(int pixels){
			if(ScrollTab!=null){
				ScrollTab.ScrollBy(pixels);
			}
		}
		
		/// <summary>Scrolls a scrollbar to the given location in pixels.</summary>
		/// <param name="location">The number of pixels the scrollbar tab will try to locate at.</param>
		public void ScrollTo(int location){
			if(ScrollTab!=null){
				ScrollTab.ScrollTo(location,true);
			}
		}
		
		/// <summary>Scrolls a scrollbar to the given 0-1 location along the bar.</summary>
		/// <param name="position">The 0-1 location along the bar that the tab will locate at.</param>
		public void ScrollTo(float position){
			if(ScrollTab!=null){
				ScrollTab.ScrollTo(position,true);
			}
		}
		
		/// <summary>Called when ScrollTop or ScrollLeft has changed.</summary>
		public void ElementScrolled(){
			if(ScrollTab!=null){
				Element target=GetTarget();
				if(target==null){
					return;
				}
				ComputedStyle cs=target.style.Computed;
				
				float barProgress=0f;
				
				if(Type==InputType.HScroll){
					barProgress=(float)cs.ScrollLeft / (float)cs.ContentWidth;
				}else{
					barProgress=(float)cs.ScrollTop / (float)cs.ContentHeight;
				}
				if(barProgress<0f){
					barProgress=0f;
				}else if(barProgress>1f){
					barProgress=1f;
				}
				
				ScrollTab.ElementScrolled(barProgress);
			}
		}
		
		/// <summary>Recalculates the tab size of a scroll bar.</summary>
		public virtual void RecalculateTab(){
			if(ScrollTab==null){
				if(Element.childNodes.Count>1){
					Element scrollTab=Element.childNodes[1];
					
					if(scrollTab!=null){
						ScrollTab=((ScrollTabTag)scrollTab.Handler);
					}
				}
			}
			
			if(ScrollTab!=null){
				
				Element target=GetTarget();
				if(target==null){
					return;
				}
				ComputedStyle computed=target.style.Computed;
				
				bool useX=ScrollTab.UseX();
				OverflowType overflowMode=useX?computed.OverflowX : computed.OverflowY;
				float visible=useX?computed.VisiblePercentageX() : computed.VisiblePercentageY();
				
				if(visible>1f){
					visible=1f;
				}else if(visible<0f){
					visible=0f;
				}
				
				if(overflowMode==OverflowType.Auto){
					// Handle AUTO here.
					// Hide the bar by directly setting it's display style if the whole thing is visible - i.e. visible = 1(00%).
					ComputedStyle barStyle=Element.Style.Computed;
					if(visible==1f){
						barStyle.DisplayNone();
					}else if(barStyle.Display==DisplayType.None){
						// Make it visible again:
						barStyle.Display=DisplayType.InlineBlock;
					}
					
				}
				ScrollTab.ApplyTabSize(visible);
			}
		}
		
		/// <summary>Checks if this scrollbar has no target attribute.</summary>
		/// <returns>True if it has no target attribute; false if it does.</returns>
		public bool NoTarget(){
			return (TargetName==null);
		}
		
		/// <summary>Sets the value of this input box.</summary>
		/// <param name="value">The value to set.</param>
		public void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this input box, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		public void SetValue(string value,bool html){
			if(IsScrollInput()){
				return;
			}
			
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(value!=null && value.Length>MaxLength){
					// Yep!
					value=value.Substring(0,MaxLength);
				}
			}
			
			if(value==null || CursorIndex>value.Length){
				MoveCursor(0);
			}
			
			// Fire onchange:
			Element.Run("onchange");
			
			Element["value"]=Value=value;
			if(!IsBoolInput()){
				if(Hidden){
					// Unfortunately the new string(char,length); constructor isn't reliable.
					// Build the string manually here.
					StringBuilder sb=new StringBuilder("",value.Length);
					for(int i=0;i<value.Length;i++){
						sb.Append('*');
					}
					
					if(html){
						Element.innerHTML=sb.ToString();
					}else{
						Element.textContent=sb.ToString();
					}
				}else{
					if(html){
						Element.innerHTML=value;
					}else{
						Element.textContent=value;
					}
				}
			}
			
			if(IsTextInput()){
				if(Cursor!=null){
					Element.AppendNewChild(Cursor);
				}
			}
		}
		
		/// <summary>Used only by scrollbars. Gets the target element to be scrolled.</summary>
		/// <returns>The target element.</returns>
		public Element GetTarget(){
			if(TargetElement!=null){
				return TargetElement;
			}
			
			if(!string.IsNullOrEmpty(TargetName)){
				TargetElement=Element.Document.getElementById(TargetName);
			}
			
			bool isParent=false;
			
			if(TargetElement==null){
				// The parent of the scrollbar.
				TargetElement=Element.parentNode;
				isParent=true;
			}
			
			if(TargetElement!=null){
				if(Type==InputType.VScroll){
					TargetElement.VerticalScrollbar=this;
					TargetElement.VScrollbar=isParent;
				}else{
					TargetElement.HorizontalScrollbar=this;
					TargetElement.HScrollbar=isParent;
				}
			}
			
			return TargetElement;
		}
		
		/// <summary>Checks if this is a horizontal or vertical scrollbar.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsScrollInput(){
			return (Type==InputType.VScroll||Type==InputType.HScroll);
		}
		
		/// <summary>Checks if this is a radio or checkbox input box.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsBoolInput(){
			return (Type==InputType.Radio||Type==InputType.Checkbox);
		}
		
		/// <summary>Checks if this is a text or password input box.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsTextInput(){
			return (Type==InputType.Text);
		}
		
		public override void OnKeyPress(UIEvent pressEvent){
			// We want to fire the nitro event first:
			base.OnKeyPress(pressEvent);
			
			// How long is the current value?
			int length=0;
			
			if(Value!=null){
				length=Value.Length;
			}
			
			// Is the cursor too far over?
			if(CursorIndex>length){
				MoveCursor(0);
			}
			
			if(pressEvent.cancelBubble){
				return;
			}
			
			if(pressEvent.heldDown){
				if(IsTextInput()){
					// Add to value if pwd/text, unless it's backspace:
					string value=Value;
					
					// Grab the keycode:
					KeyCode key=pressEvent.unityKeyCode;
					
					if(key==KeyCode.LeftArrow){
						MoveCursor(CursorIndex-1,true);
					}else if(key==KeyCode.RightArrow){
						MoveCursor(CursorIndex+1,true);
					}else if(key==KeyCode.Backspace){
						// Delete the character before the cursor.
						if(string.IsNullOrEmpty(value)||CursorIndex==0){
							return;
						}
						value=value.Substring(0,CursorIndex-1)+value.Substring(CursorIndex,value.Length-CursorIndex);
						int index=CursorIndex;
						SetValue(value);
						MoveCursor(index-1);
					}else if(key==KeyCode.Delete){
						// Delete the character after the cursor.
						if(string.IsNullOrEmpty(value)||CursorIndex==value.Length){
							return;
						}
						
						value=value.Substring(0,CursorIndex)+value.Substring(CursorIndex+1,value.Length-CursorIndex-1);
						SetValue(value);
					}else if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
						// Does the form have a submit button? If so, submit now.
						// Also call a convenience (non-standard) "onenter" method.
						
						FormTag form=Element.form;
						if(form!=null && form.HasSubmitButton){
							form.submit();
						}
						
						return;
					}else if(key==KeyCode.Home){
						// Hop to the start:
						
						MoveCursor(0,true);
						
					}else if(key==KeyCode.End){
						// Hop to the end:
						
						int maxCursor=0;
						
						if(value!=null){
							maxCursor=value.Length;
						}
						
						MoveCursor(maxCursor,true);
						
					}else if(pressEvent.ctrlKey){
						
						if(key==KeyCode.V){
							
							// Run the onpaste function.
							if(Element.RunBlocked("onpaste",pressEvent)){
								// It blocked it.
								return;
							}
							
							// Paste the text, stripping any newlines:
							string textToPaste=Clipboard.Paste().Replace("\r","").Replace("\n","");
							
							if(!string.IsNullOrEmpty(textToPaste)){
								// Drop the character in the string at cursorIndex
								if(value==null){
									value=""+textToPaste;
								}else{
									value=value.Substring(0,CursorIndex)+textToPaste+value.Substring(CursorIndex,value.Length-CursorIndex);
								}
								
								SetValue(value);
								MoveCursor(CursorIndex+textToPaste.Length);
							}
							
						}else if(key==KeyCode.C){
							
							// Run the oncopy function.
							if(Element.RunBlocked("oncopy",pressEvent)){
								// It blocked it.
								return;
							}
							
							Clipboard.Copy(value);
						}
						
					}else if(char.IsControl(pressEvent.character)){
						// Ignore it.
						
						return;
					}else{
						if(pressEvent.character=='\0'){
							return;
						}
						
						// Drop the character in the string at cursorIndex
						if(value==null){
							value=""+pressEvent.character;
						}else{
							value=value.Substring(0,CursorIndex)+pressEvent.character+value.Substring(CursorIndex,value.Length-CursorIndex);
						}
						
						SetValue(value);
						MoveCursor(CursorIndex+1);
					}
				}
			}
		}
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		public void MoveCursor(int index){
			MoveCursor(index,false);
		}
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		/// <param name="immediate">True if the cursor should be moved right now.</param>
		public void MoveCursor(int index,bool immediate){
			if(index<0||Value==null){
				index=0;
			}else if(index>Value.Length){
				// Inclusive - we can be positioned before or after it.
				index=Value.Length;
			}
			
			if(index==CursorIndex){
				return;
			}
			
			CursorIndex=index;
			
			if(Cursor==null){
				return;
			}
			
			LocateCursor=true;

			if(immediate){
				// We have enough info to place the cursor already.
				// Request a layout.
				RequestLayout();
			}
			// Otherwise locating the cursor is delayed until after the new value has been rendered.
			// This is used immediately after we set the value.
		}
		
		/// <summary>Called immediately after the text pass on the focused element.</summary>
		public override void OnRenderPass(){
			// This must be done here as the width of words is not known until this point.
			if(Cursor==null||!LocateCursor){
				return;
			}
			LocateCursorNow();
		}
		
		/// <summary>Positions the cursor immediately.</summary>
		private void LocateCursorNow(){
			LocateCursor=false;
			// Position the cursor - we need to locate the letter's exact position first:
			// ..Which will be in the text element:
			float position=0f;
			
			if(Element.childNodes.Count>1){
				// Note: If it's equal to 1, ele[0] is the cursor.
				TextElement text=(TextElement)(Element.childNodes[0]);
				int index=CursorIndex;
				
				if(text!=null){
					Vector2 fullPosition=text.GetPosition(ref index);
					position=fullPosition.x;
				}
			}
			
			// Scroll it if the cursor is beyond the end of the box:
			int boxSize=Element.Style.Computed.InnerWidth;
			if(position>boxSize){
				Element.Style.Computed.ScrollLeft=(int)position-Element.Style.Computed.InnerWidth;
			}else{
				Element.Style.Computed.ScrollLeft=0;
			}
			
			// Set it in:
			Element.Style.Computed.SetSize();
			
			if(position>boxSize){
				Cursor.Style.Computed.PositionLeft=boxSize-1;
			}else{
				Cursor.Style.Computed.PositionLeft=((int)position);
			}
		}
		
		/// <summary>Called when the element is focused.</summary>
		public override void OnFocus(){
			if(!IsTextInput()||Cursor!=null){
				return;
			}
			// Add a cursor.
			Element.appendInnerHTML("<div class='cursor'></div>");
			Cursor=Element.getElementByAttribute("class","cursor");
			CursorIndex=0;
		}
		
		/// <summary>Called when the element is unfocused/blurred.</summary>
		public override void OnBlur(){
			if(Cursor==null){
				return;
			}
			// Remove the cursor:
			Cursor.parentNode.removeChild(Cursor);
			Cursor=null;
		}
		
		public override bool OnClick(UIEvent clickEvent){
			
			// Did the mouse go up, and was the element clicked down on too?
			if(!clickEvent.heldDown && Element.MouseWasDown()){
				
				// Focus it:
				Element.Focus();
				
				if(Type==InputType.Submit){
					// Find the form and then attempt to submit it.
					FormTag form=Element.form;
					if(form!=null){
						form.submit();
					}
				}else if(IsScrollInput()){
					// Clicked somewhere on a scrollbar:
					// Figure out where the click was, and scroll there.
				}else if(Type==InputType.Radio){
					Select();
				}else if(Type==InputType.Checkbox){
					
					if(Checked){
						Unselect();
					}else{
						Select();
					}
					
				}else if(IsTextInput()){
					// Move the cursor to the click point:
					int localClick=clickEvent.clientX-Element.Style.Computed.OffsetLeft + Element.Style.Computed.ScrollLeft;
					int index=0;
					
					if(Element.childNodes.Count>1){
						// Note: If it's equal to 1, ele[0] is the cursor.
						TextElement text=(TextElement)(Element.childNodes[0]);
						if(text!=null){
							index=text.LetterIndex(localClick);
						}
					}
					
					MoveCursor(index,true);
				}
				
			}
			
			base.OnClick(clickEvent);
			clickEvent.stopPropagation();
			
			return true;
		}
		
	}
	
	
	public partial class Element{
		
		
		/// <summary>Gets or sets the value of this element. Input/Select elements only.</summary>
		public string value{
			get{
				if(Tag=="select"){
					return ((SelectTag)Handler).GetValue();
				}
				return this["value"];
			}
			set{
				this["value"]=value;
			}
		}
		
		/// <summary>Gets or sets the value as html for this element. Input/Select elements only.</summary>
		public string content{
			get{
				if(Tag=="select"){
					return ((SelectTag)Handler).GetValue();
				}
				return this["content"];
			}
			set{
				this["content"]=value;
			}
		}
		
	}
	
}