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

using Wrench;

namespace PowerUI{
	
	/// <summary>
	/// A base class for all html tag types (e.g. script, a, body etc).
	/// These tag handlers tell the UI how to render and work with this type of tag.
	/// Tag handlers are stored globally for lookup and instanced per element.
	/// </summary>
	
	public class HtmlTagHandler:TagHandler{
		
		/// <summary>True if this element is isolated from style changes by its parent (e.g. used by iframes).</summary>
		public bool IsIsolated;
		/// <summary>The element this tag handler is attached to.</summary>
		public Element Element;
		/// <summary>True if the element should ignore all clicks.</summary>
		public bool IgnoreClick;
		/// <summary>True if this tag is focusable.</summary>
		public bool IsFocusable;
		/// <summary>Set this to true if this element should ignore clicks on itself, but not its kids (e.g. html or body).</summary>
		public bool IgnoreSelfClick;
		
		/// <summary>Requests the renderer handling this element to layout next update.</summary>
		public void RequestLayout(){
			Element.Document.Renderer.RequestLayout();
		}
		
		/// <summary>Tells the parser to not include this element in the DOM.</summary>
		/// <returns>True if this tag should be dumped and not enter the DOM once fully loaded.</returns>
		public virtual bool Junk(){
			return false;
		}
		
		/// <summary>Called when all variable (&Variable;) values must have their content reloaded.</summary>
		public virtual void OnResetAllVariables(){}
		
		/// <summary>Called when the named variable variable (&name;) values must have its content reloaded.</summary>
		public virtual void OnResetVariable(string name){}
		
		/// <summary>Called during a layout event on only the focused element.</summary>
		public virtual void OnRenderPass(){}
		
		/// <summary>Called when the fixed height of this element changes.</summary>
		public virtual void HeightChanged(){}
		
		/// <summary>Called when the width of this element changes.</summary>
		public virtual void WidthChanged(){}
		
		/// <summary>Called during a layout event on all elements.</summary>
		public virtual void OnLayout(){}
		
		/// <summary>Called when this element comes into focus.</summary>
		public virtual void OnFocus(){}
		
		/// <summary>Called when this element becomes unfocused.</summary>
		public virtual void OnBlur(){}
		
		/// <summary>Called when this elements content has been loaded. Used by e.g. iframe or img.</summary>
		/// <param name="objectLoaded">The object which has loaded. E.g. background-image or webpage.</param>
		public virtual void OnLoaded(string objectLoaded){}
		
		/// <summary>Called on the focused element when a key is pressed or released.</summary>
		/// <param name="pressEvent">The UIEvent describing the press; e.g. which key.</param>
		public virtual void OnKeyPress(UIEvent pressEvent){
			Element.OnKeyPress(pressEvent);
		}
		
		/// <summary>Called on the focused element when the mouse is moved.</summary>
		/// <param name="clickEvent">The UIEvent describing the click.</param>
		public virtual void OnMouseMove(UIEvent moveEvent){
			Element.OnMouseMoveEvent(moveEvent);
		}
		
		/// <summary>Called when the element is clicked (or the mouse is released over it).</summary>
		/// <param name="clickEvent">The UIEvent describing the click.</param>
		public virtual bool OnClick(UIEvent clickEvent){
			return Element.OnClickEvent(clickEvent);
		}
		
		/// <summary>Called when PowerUI attempts to display the mobile keyboard (only called on mobile platforms).</summary>
		/// <returns>A KeyboardType if this element wants the keyboard to show up (KeyboardType.None otherwise).</returns>
		public virtual KeyboardMode OnShowMobileKeyboard(){
			return null;
		}
		
		/// <summary>Called when the elements kids are fully loaded.</summary>
		public virtual void OnChildrenLoaded(){}
		
		/// <summary>Called when an attribute of the element was changed.
		/// Returns true if the method handled the change to prevent unnecessary checks.</summary>
		public override bool OnAttributeChange(string property){
			if(property=="id"){
				Element.Style.Computed.SetSelector(Css.SelectorType.ID,"#"+Element["id"]);
				return true;
			}else if(property=="style"){
				Element.Style.cssText=Element["style"];
				return true;
			}else if(property=="class"){
				Element.Style.Computed.SetSelector(Css.SelectorType.Class,"."+Element["class"]);
				return true;
			}else if(property=="name"){
				// Nothing happens with this one - ignore it.
				return true;
			}else if(property=="onmousedown"){
				if(Element.Style.Computed.Display==Css.DisplayType.Inline){
					Wrench.Log.Add("Warning: onmousedown doesn't currently work with display:inline (span's). Use display:inline-block instead.");
				}
				return true;
			}else if(property=="onmouseup"){
				if(Element.Style.Computed.Display==Css.DisplayType.Inline){
					Wrench.Log.Add("Warning: onmouseup doesn't currently work with display:inline (span's). Use display:inline-block instead.");
				}
				return true;
			}else if(property=="onkeydown"){
				return true;
			}else if(property=="onkeyup"){
				return true;
			}else if(property=="height"){
				string height=Element["height"];
				if(height.IndexOf("%")==-1 && height.IndexOf("px")==-1 && height.IndexOf("em")==-1){
					height+="px";
				}
				Element.style.height=height;
				return true;
			}else if(property=="width"){	
				string width=Element["width"];
				if(width.IndexOf("%")==-1 && width.IndexOf("px")==-1 && width.IndexOf("em")==-1){
					width+="px";
				}
				Element.style.width=width;
				return true;
			}else if(property=="align"){
				Element.style.textAlign=Element["align"];
				return true;
			}else if(property=="valign"){
				Element.style.vAlign=Element["valign"];
				return true;
			}
			return false;
		}
		
	}
	
}