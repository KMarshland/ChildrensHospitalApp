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
using Wrench;


namespace PowerUI{
	
	/// <summary>
	/// Handles textarea tags.
	/// </summary>
	
	public class TextareaTag:HtmlTagHandler{
		
		/// <summary>The value text for this input.</summary>
		public string Value;
		/// <summary>This is the cursor element.</summary>
		public Element Cursor;
		/// <summary>The current location of the cursor.</summary>
		public int CursorIndex;
		/// <summary>True if the cursor should be located after the next update.</summary>
		public bool LocateCursor;
		/// <summary>The maximum length of text in this box.</summary>
		public int MaxLength=int.MaxValue;
		
		
		public TextareaTag(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"textarea"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new TextareaTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="value"){
				SetValue(Element["value"]);
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
			}
			
			return false;
		}
		
		public override KeyboardMode OnShowMobileKeyboard(){
			KeyboardMode result=new KeyboardMode();
			return result;
		}
		
		public override void OnParseContent(MLLexer lexer){
			lexer.Literal=true;
			// Keep reading until we hit </textarea>.
		
			string valueText="";
			
			while(!AtEnd(lexer)){
				valueText+=lexer.Read();
			}
			
			// Assign to the value:
			SetValue(valueText);
			
			lexer.Literal=false;
		}
		
		
		/// <summary>Sets the value of this textarea.</summary>
		/// <param name="value">The value to set.</param>
		public void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this textarea, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		public void SetValue(string value,bool html){
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(value.Length>MaxLength){
					// Yep!
					value=value.Substring(0,MaxLength);
				}
			}
			
			if(CursorIndex>value.Length){
				MoveCursor(0);
			}
			
			// Fire onchange:
			Element.Run("onchange");
			
			Element["value"]=Value=value;
			
			if(html){
				Element.innerHTML=value;
			}else{
				Element.textContent=value;
			}
			
			if(Cursor!=null){
				Element.AppendNewChild(Cursor);
			}
		}
		
		
		public override void OnKeyPress(UIEvent pressEvent){
			// We want to fire the nitro event first:
			base.OnKeyPress(pressEvent);
			
			if(Value!=null && CursorIndex>Value.Length){
				MoveCursor(0);
			}
			
			if(pressEvent.heldDown){
				// Add to value unless it's backspace:
				
				string value=Value;
				KeyCode key=((KeyCode)pressEvent.keyCode);
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
						
						string textToPaste=Clipboard.Paste();
						
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
			Vector2 position=Vector2.zero;
			
			if(Element.childNodes.Count>1){
				// Note: If it's equal to 1, ele[0] is the cursor.
				TextElement text=(TextElement)(Element.childNodes[0]);
				if(text!=null){
					position=text.GetPosition(CursorIndex);
				}
			}
			
			// Set it in:
			Cursor.Style.Computed.PositionLeft=((int)position.x);
			Cursor.Style.Computed.PositionTop=((int)position.y)-Element.Style.Computed.ScrollTop;
			
		}
		
		/// <summary>Called when the element is focused.</summary>
		public override void OnFocus(){
			if(Cursor!=null){
				return;
			}
			
			// Add a cursor.
			Element.appendInnerHTML("<div class='cursor-textarea'></div>");
			Cursor=Element.getElementWithProperty("class","cursor-textarea");
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
			if(!clickEvent.heldDown && Element.MouseWasDown()){
				Element.Focus();
				
				// Move the cursor to the click point:
				int localClickX=clickEvent.clientX-Element.Style.Computed.OffsetLeft + Element.Style.Computed.ScrollLeft;
				int localClickY=clickEvent.clientY-Element.Style.Computed.OffsetTop + Element.Style.Computed.ScrollTop;
				
				int index=0;
				
				if(Element.childNodes.Count>1){
					// Note: If it's equal to 1, ele[0] is the cursor.
					TextElement text=(TextElement)(Element.childNodes[0]);
					if(text!=null){
						index=text.LetterIndex(localClickX,localClickY);
					}
				}
				
				MoveCursor(index,true);
			}
			
			base.OnClick(clickEvent);
			clickEvent.stopPropagation();
			
			return true;
		}
		
		/// <summary>Checks if the given lexer has reached the end of the inline textarea content.</summary>
		/// <param name="lexer">The lexer to check if it's reached the &lt;/textarea&gt; tag.</param>
		/// <returns>True if the lexer has reached the end textarea tag; false otherwise.</returns>
		private bool AtEnd(MLLexer lexer){
			// Up to x will do; we're certainly at the end by that point.
			return (lexer.Peek()=='<'&&
					lexer.Peek(1)=='/'&&
					lexer.Peek(2)=='t'&&
					lexer.Peek(3)=='e'&&
					lexer.Peek(4)=='x'
					);
		}
		
	}
	
}