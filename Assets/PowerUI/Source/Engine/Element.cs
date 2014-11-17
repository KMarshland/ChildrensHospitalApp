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
using System.Reflection;
using UnityEngine;
using PowerUI.Css;
using Wrench;
using Nitro;


namespace PowerUI{
	
	/// <summary>
	/// This delegate is used for hooking up c# methods with mouse and keyboard events.
	/// Note that in general Nitro is best used for handling these.
	/// </summary>
	public delegate void EventHandler(UIEvent uiEvent);
	
	/// <summary>
	/// This represents a html element in the DOM.
	/// </summary>

	public partial class Element:MLElement{
	
		/// <summary>A custom data object for whatever you would like to pass through for e.g. callbacks.</summary>
		public object Data;
		/// <summary>True if this element has a horizontal scrollbar that it must render. Don't set manually.</summary>
		public bool HScrollbar;
		/// <summary>True if this element has a vertical scrollbar that it must render. Don't set manually.</summary>
		public bool VScrollbar;
		/// <summary>The html document that this element belongs to.</summary>
		public Document Document;
		/// <summary>Internal use only. The parent of this element. Use <see cref="PowerUI.Element.parentNode"/> instead.</summary>
		public Element ParentNode;
		/// <summary>Internal use only. The style of this element. Use <see cref="PowerUI.Element.style"/> instead.</summary>
		public ElementStyle Style;
		/// <summary>The handler for the tag of this element (e.g. a, body, u etc).</summary>
		public HtmlTagHandler Handler;
		/// <summary>Is the mouse is over this element, and if so, did the element consume it? Unreliable from within onmouseover/out - use IsMousedOver for that.</summary>
		public MouseOverState MousedOver;
		/// <summary>The vertical scrollbar that scrolls this element if there is one.</summary>
		public InputTag VerticalScrollbar;
		/// <summary>Internal use only. The set of child elements for this element.</summary>
		protected List<Element> ChildNodes;
		/// <summary>The horizontal scrollbar that scrolls this element if there is one.</summary>
		public InputTag HorizontalScrollbar;
		/// <summary>Internal use only. Children being rendered are set here. This allows multiple threads to access the DOM.</summary>
		public List<Element> KidsToRender;
		/// <summary>This is true if the ChildNodes are being rebuilt. True for a tiny amount of time, but prevents collisions with the renderer thread.</summary>
		public bool IsRebuildingChildren;
		/// <summary>An alternative to Nitro. Called when this element receives a keyup.</summary>
		public event EventHandler OnKeyUp;
		/// <summary>An alternative to Nitro. Called when this element receives a keydown.</summary>
		public event EventHandler OnKeyDown;
		/// <summary>An alternative to Nitro. Called when this element receives a mouseup.</summary>
		public event EventHandler OnMouseUp;
		/// <summary>An alternative to Nitro. Called when this element receives a mouseout.</summary>
		public event EventHandler OnMouseOut;
		/// <summary>An alternative to Nitro. Called when this element receives a mousedown.</summary>
		public event EventHandler OnMouseDown;
		/// <summary>An alternative to Nitro. Called when this element receives a mousemove. Note that it must be focused.</summary>
		public event EventHandler OnMouseMove;
		/// <summary>An alternative to Nitro. Called when this element receives a mouseover.</summary>
		public event EventHandler OnMouseOver;
		/// <summary>An alternative to Nitro. Called when this element receives a loaded event (e.g. iframe).</summary>
		public event EventHandler OnLoadedEvent;
		/// <summary>An alternative to Nitro. Called when this element gets focused.</summary>
		public event EventHandler OnFocus;
		/// <summary>An alternative to Nitro. Called when this element is unfocused (blurred).</summary>
		public event EventHandler OnBlur;
		/// <summary>An alternative to Nitro. Called when this element receives a full click.</summary>
		public event EventHandler OnClick;
		
		
		/// <summary>Creates a new element with the given tag, parenting it to the main UI document.</summary>
		/// <param name="tag">The tag, e.g. "<div id='hello'>".</param>
		public Element(string tag):this(tag,UI.document.body){}
		
		/// <summary>Creates a new element with the given tag and parent.</summary>
		/// <param name="tag">The tag, e.g. "<div id='hello'>".</param>
		/// <param name="parent">The element to parent to.</param>
		public Element(string tag,Element parent):this(parent.Document,new MLLexer(tag),parent){}
		
		/// <summary>Creates a new element for the given document and as a child of the given parent.</summary>
		/// <param name="document">The document that this element will belong to.</param>
		/// <param name="parent">The element that this element will be parented to.</param>
		public Element(Document document,Element parent){
			Document=document;
			ParentNode=parent;
			Style=new ElementStyle(this);
		}
		
		/// <summary>Creates a new element for the given document and as a child of the given parent with content to parse.</summary>
		/// <param name="document">The document that this element will belong to.</param>
		/// <param name="lexer">An MLLexer containing the tag. No children are read; Just this tag only.</param>
		/// <param name="parent">The element that this element will be parented to.</param>
		private Element(Document document,MLLexer lexer,Element parent):this(document,parent){
			ReadTag(lexer);
		}
		
		/// <summary>Runs the given function held in the named attribute (e.g. onkeydown) and checks if that function blocked
		/// the event. In the case of a blocked event, no default action should occur.</summary>
		/// <param name="attribute">The name of the attribute, e.g. onkeydown.</param>
		/// <param name="uiEvent">A standard UIEvent containing e.g. key/mouse information.</param>
		public bool RunBlocked(string attribute,UIEvent uiEvent){
			
			// Run the function:
			object result=Run(attribute,uiEvent);
			
			if(result!=null && result.GetType()==typeof(bool)){
				// It returned true/false - was it false?
				
				if(!(bool)result){
					// Returned false - Blocked it.
					return true;
				}
				
			}
			
			return uiEvent.cancelBubble;
		}
		
		/// <summary>Runs a nitro function whos name is held in the given attribute.</summary>
		/// <param name="attribute">The name of the attribute in lowercase, e.g. "onmousedown".</param>
		/// <param name="args">Additional parameters you would like to pass to your function.</param>
		/// <returns>The value returned by the function.</returns>
		/// <exception cref="NullReferenceException">Thrown if the function does not exist.</exception>
		public object Run(string attribute,params object[] args){
			return RunLiteral(attribute,args);
		}
		
		/// <summary>Runs a nitro function whos name is held in the given attribute with a fixed block of arguments.</summary>
		/// <param name="attribute">The name of the attribute in lowercase, e.g. "onmousedown".</param>
		/// <param name="args">Additional parameters you would like to pass to your function.</param>
		/// <returns>The value returned by the function.</returns>
		/// <exception cref="NullReferenceException">Thrown if the function does not exist.</exception>
		public object RunLiteral(string attribute,object[] args){
			string methodName=this[attribute];
			if(methodName==null){
				return null;
			}
			
			if(methodName.Contains(".")){
				// C# or UnityJS method.
				string[] pieces=methodName.Split('.');
				
				if(pieces.Length!=2){
					Wrench.Log.Add("onmousedown of '"+methodName+"' is invalid. If you're using a c# or UnityJS function, only one . is allowed (className.staticMethodName).");
					return null;
				}
				
				// Grab the class name:
				string className=pieces[0];
				// Go get the type:
				Type type=CodeReference.GetFirstType(className);
				
				if(type==null){
					Wrench.Log.Add("Type not found: "+className);
					return null;
				}
				
				// Update the method name:
				methodName=pieces[1];
				
				// Grab the method info:
				try{
					#if NETFX_CORE
					MethodInfo method=type.GetTypeInfo().GetDeclaredMethod(methodName);
					#else
					MethodInfo method=type.GetMethod(methodName);
					#endif
					// Invoke it:
					return method.Invoke(null,args);
				}catch(Exception e){
					Wrench.Log.Add("Calling method "+className+"."+methodName+"(..) errored: "+e);
					return null;
				}
			}
			
			return Document.RunLiteral(methodName,this,args);
		}
		
		public override void OnChildrenLoaded(){
			Handler.OnChildrenLoaded();
		}
		
		/// <summary>Changes the document used by this element and all it's kids. Used by iframes.</summary>
		/// <param name="document">The new document to use.</param>
		public void SetDocument(Document document){
			Document=document;
			if(ChildNodes!=null){
				for(int i=0;i<ChildNodes.Count;i++){
					ChildNodes[i].SetDocument(document);
				}
			}
			
			if(HScrollbar){
				HorizontalScrollbar.Element.SetDocument(document);
			}
			
			if(VScrollbar){
				VerticalScrollbar.Element.SetDocument(document);
			}
		}
		
		/// <summary>Called by some tags when their content is loaded. E.g. img tag or iframe.</summary>
		/// <param name="objectLoaded">The object which has loaded. E.g. background-image or webpage.</param>
		public void OnLoaded(string objectLoaded){
			Handler.OnLoaded(objectLoaded);
			Run("onloaded",objectLoaded);
			if(OnLoadedEvent!=null){
				OnLoadedEvent(null);
			}
		}
		
		/// <summary>Focuses this element so it receives events such as keypresses.</summary>
		public void Focus(){
			if(Input.Focused==this){
				return;
			}
			if(Input.Focused!=null){
				Input.Focused.Unfocus();
			}
			Input.Focused=this;
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
			// Should we pop up the mobile keyboard?
			KeyboardMode mobile=Handler.OnShowMobileKeyboard();
			if(Input.HandleKeyboard(mobile)){
				Input.KeyboardText=value;
			}
			
			#endif
			
			Run("onfocus");
			Document.window.Event=null;
			Handler.OnFocus();
			if(OnFocus!=null){
				OnFocus(null);
			}
		}
		
		/// <summary>Unfocuses this element so it will no longer receive events like keypresses.</summary>
		public void Unfocus(){
			if(Input.Focused!=this){
				return;
			}
			try{
				Run("onblur");
			}catch(Exception e){
				Debug.LogError("Error in OnBlur: "+e.ToString());
			}
			Input.Focused=null;
			
			if(OnBlur!=null){
				OnBlur(null);
			}
			
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
			// Attempt to hide the keyboard.
			Input.HandleKeyboard(null);
			#endif
			
			Document.window.Event=null;
			Handler.OnBlur();
		}
		
		/// <summary>True if the mouse is over this element. Most accurate compared to MousedOver, but is more intensive.
		/// In general, only use this one from within onmouseover or onmouseout.</summary>
		public bool IsMousedOver(){
			Vector2 point=Input.MousePosition;
			return Style.Computed.Contains((int)point.x,(int)point.y);
		}
		
		/// <summary>Let the element know the mouse has moved over it.</summary>
		/// <param name="mouseEvent">The UIEvent that represents where the mouse is.</param>
		/// <returns>True if this element accepts the mouse; false otherwise.</returns>
		public bool MouseOver(UIEvent mouseEvent){
			
			// Text (TextElement) or variable (VariableElement) elements must not run a mouseover:
			if(GetType()!=typeof(Element)){
				return false;
			}
			
			if(Input.MouseOvers.Count>mouseEvent.bubbleCount){
				if(Input.MouseOvers[mouseEvent.bubbleCount]!=this){
					Input.ClearMouseOvers(mouseEvent,mouseEvent.bubbleCount);
					Input.MouseOvers.Add(this);
				}else{
					// Just bump up bubble count:
					mouseEvent.bubbleCount++;
					
					// Just re-broadcast the previous event state.
					return (MousedOver==MouseOverState.OverConsumed);
				}
			}else{
				Input.MouseOvers.Add(this);
			}
			
			mouseEvent.bubbleCount++;
			
			bool result=false;
			
			// Update css:
			if(Style.Computed.Hover()){
				result=true;
			}
			
			// Set the tooltip, if we've got one:
			string title=this["title"];
			
			if(title!=null){
				Document.tooltip=title;
			}
			
			mouseEvent.target=this;
			Document.window.Event=mouseEvent;
			
			if(this["onmouseover"]!=null){
				Run("onmouseover",mouseEvent);
				result=true;
			}
			
			if(OnMouseOver!=null){
				OnMouseOver(mouseEvent);
				result=true;
			}
			
			if(!result){
				result=HasBackground;
			}
			
			if(result){
				MousedOver=MouseOverState.OverConsumed;
			}else{
				MousedOver=MouseOverState.Over;
			}
			
			return result;
		}
		
		/// <summary>Let the element know the mouse is no longer over it.</summary>
		/// <param name="mouseEvent">The UIEvent that represents where the mouse is.</param>
		public void MouseOut(UIEvent mouseEvent){
			if(MousedOver==MouseOverState.Out){
				return;
			}
			
			MousedOver=MouseOverState.Out;
			
			// Clear the tooltip:
			Document.tooltip=null;
			
			// Update css:
			Style.Computed.Unhover();
			
			mouseEvent.target=this;
			Document.window.Event=mouseEvent;
			Run("onmouseout",mouseEvent);
			if(OnMouseOut!=null){
				OnMouseOut(mouseEvent);
			}
		}
		
		/// <summary>Called by a tag handler when a key press occurs.</summary>
		/// <param name="clickEvent">The event that represents the key press.</param>
		public void OnKeyPress(UIEvent pressEvent){
			if(pressEvent.heldDown){
				Run("onkeydown",pressEvent);
				if(OnKeyDown!=null){
					OnKeyDown(pressEvent);
				}
			}else{
				Run("onkeyup",pressEvent);
				if(OnKeyUp!=null){
					OnKeyUp(pressEvent);
				}
			}
		}
		
		/// <summary>Called by a tag handler when a click occurs.</summary>
		/// <param name="clickEvent">The event that represents the click.</param>
		/// <returns>True if it accepted the click.</returns>
		public bool OnClickEvent(UIEvent clickEvent){
			bool result=false;
			
			if(clickEvent.heldDown){
				Input.LastMouseDown.Add(this);
				
				if(this["onmousedown"]!=null){
					Run("onmousedown",clickEvent);
					result=true;
				}
				
				if(OnMouseDown!=null){
					OnMouseDown(clickEvent);
					result=true;
				}
				
				// Apply the active style:
				if(MousedOver==MouseOverState.OverConsumed){
					Style.Computed.Unhover();
				}
				
				Style.Computed.SetModifier("active");
				
			}else{
				if(this["onmouseup"]!=null){
					Run("onmouseup",clickEvent);
					result=true;
				}
				
				if(OnMouseUp!=null){
					OnMouseUp(clickEvent);
					result=true;
				}
				
				if(MouseWasDown()){
					if(this["onclick"]!=null){
						Run("onclick",clickEvent);
						result=true;
					}
					
					if(OnClick!=null){
						OnClick(clickEvent);
						result=true;
					}
					
				}
				
			}
			return result;
		}
		
		/// <summary>Was the mouse clicked on this element during the last mouse down?</summary>
		public bool MouseWasDown(){
			foreach(Element element in Input.LastMouseDown){
				if(element==this){
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Called on focused elements only (see focus()). Runs the mouse move functions.</summary>
		public void OnMouseMoveEvent(UIEvent moveEvent){
			
			if(this["onmousemove"]!=null){
				Run("onmousemove",moveEvent);
			}
			
			if(OnMouseMove!=null){
				OnMouseMove(moveEvent);
			}
			
		}
		
		/// <summary>True if this element has some form of background applied to it.</summary>
		public bool HasBackground{
			get{
				return Style.Computed.HasBackground;
			}
		}
		
		/// <summary>Refreshes this elements css style if the given selector matches its own.</summary>
		/// <param name="type">The type of the given selector.</param>
		/// <param name="selector">The selector to match with.</param>
		public void RefreshSelector(Css.SelectorType type,string selector){
			ComputedStyle computed=Style.Computed;
			
			switch(type){
				case Css.SelectorType.Class:
					
					bool refresh=false;
					
					if(computed.ClassSelector==selector){
						refresh=true;
					}else if(computed.ExtraClassSelectors!=null){
						
						// Special case if we've got multiple classes on this element.
						
						for(int i=0;i<computed.ExtraClassSelectors.Length;i++){
							
							if(computed.ExtraClassSelectors[i]==selector){
								
								refresh=true;
								
								break;
							}
							
						}
						
					}
					
					if(refresh){
						computed.RefreshSelector(type);
					}
					
				break;
				case Css.SelectorType.ID:
					
					if(computed.IDSelector==selector){
						computed.RefreshSelector(type);
					}
					
				break;
				default:
					
					if(computed.TagSelector==selector){
						computed.RefreshSelector(type);
					}
					
				break;
			}
			
			if(ChildNodes!=null){
				for(int i=0;i<ChildNodes.Count;i++){
					ChildNodes[i].RefreshSelector(type,selector);
				}
			}
			
		}
		
		/// <summary>This is the second pass of layout requests.
		/// It positions the element in global screen space and also fires the render events
		/// which in turn generate or reallocates mesh blocks. This call applies to all it's
		/// children elements too.</summary>
		/// <param name="relativeTo">The current style we are positioning relative to.</param>
		public void PositionGlobally(ComputedStyle relativeTo){
			
			ComputedStyle computed=Style.Computed;
			if(computed.Display==DisplayType.None){
				// Don't draw this element or it's kids.
				return;
			}
			
			// Position globally:
			if(computed.Position==PositionType.Fixed){
				// Fixed elements are nice and simple to deal with - Their essentially absolutely positioned but relative to the html tag.
				
				ComputedStyle html=Document.html.style.Computed;
				
				if(computed.RightPositioned){
					// The right hand edge of parent minus how far from the edge minus the width.
					computed.OffsetLeft=computed.MarginLeft+html.OffsetLeft+html.PixelWidth-computed.PositionRight-computed.PixelWidth;
				}else{
					computed.OffsetLeft=computed.MarginLeft+html.OffsetLeft+computed.PositionLeft;
				}
				
				if(computed.BottomPositioned){
					computed.OffsetTop=computed.MarginTop+html.OffsetTop+html.PixelHeight-computed.PositionBottom-computed.PixelHeight;
				}else{
					computed.OffsetTop=computed.MarginTop+html.OffsetTop+computed.PositionTop;
					
				}
				
				// Change relativeTo here - we are now hopping to this fixed objects kids.
				relativeTo=computed;
				
			}else if(computed.Position==PositionType.Relative){
				// Relative to where they should have been. PositionLeft/PositionRight etc. may be zero, but not always.
				
				// The width of border/padding/margin + the position of the parent + the offet from the parent.
				if(relativeTo==null){
					computed.OffsetLeft=computed.MarginLeft+computed.ParentOffsetLeft;
				}else{
					computed.OffsetLeft=computed.MarginLeft+relativeTo.StyleOffsetLeft + relativeTo.OffsetLeft + computed.ParentOffsetLeft;
				}
				if(computed.RightPositioned){
					computed.OffsetLeft-=computed.PositionRight;
				}else{
					computed.OffsetLeft+=computed.PositionLeft;
				}
				
				if(relativeTo==null){
					computed.OffsetTop=computed.MarginTop+computed.ParentOffsetTop;
				}else{
					computed.OffsetTop=computed.MarginTop+relativeTo.StyleOffsetTop + relativeTo.OffsetTop + computed.ParentOffsetTop;
				}
				if(computed.BottomPositioned){
					computed.OffsetTop-=computed.PositionBottom;
				}else{
					computed.OffsetTop+=computed.PositionTop;
				}
				
				// Vertical alignment:
				bool tableCell=(computed.Display==DisplayType.TableCell);
				if(relativeTo!=null||computed.AutoMarginY||tableCell){
					if(computed.AutoMarginY || relativeTo.VerticalAlign==VerticalAlignType.Middle || (tableCell && computed.VerticalAlign==VerticalAlignType.Middle)){
						// Similar to below - we find the gap, then add *half* of that onto OffsetTop.
						if(tableCell){
							// Move upwards - we're sitting on the line and want to be above it.
							computed.OffsetTop-=(relativeTo.InnerHeight-computed.PixelHeight)/2;
						}else{
							computed.OffsetTop+=(relativeTo.InnerHeight-relativeTo.ContentHeight)/2;
						}
					}else if(relativeTo.VerticalAlign==VerticalAlignType.Bottom){
						// Find the gap - parent height-contentHeight.
						// Then simply add that onto offsetTop.
						computed.OffsetTop+=relativeTo.InnerHeight-relativeTo.ContentHeight;
					}else if(tableCell && computed.VerticalAlign==VerticalAlignType.Top){
						// This time we find the gap and remove it - we're at the bottom by default as a td sits on the line.
						computed.OffsetTop-=relativeTo.InnerHeight-computed.PixelHeight;
					}
				}
				
				// Note: relativeTo does not change here if we're in an inline element:
				if(computed.Display!=DisplayType.Inline){
					relativeTo=computed;
				}
			}else{
				// Absolute - relative to parent. It's ParentOffsetLeft/Top are both zero.
				
				if(computed.RightPositioned){
					// The right hand edge of parent minus how far from the edge minus the width.
					computed.OffsetLeft=computed.MarginLeft+relativeTo.OffsetLeft-relativeTo.StyleOffsetLeft-relativeTo.ScrollLeft+relativeTo.PixelWidth-computed.PositionRight-computed.PixelWidth;
				}else{
					computed.OffsetLeft=computed.MarginLeft+relativeTo.OffsetLeft+relativeTo.StyleOffsetLeft+relativeTo.ScrollLeft+computed.PositionLeft;
				}
				
				if(computed.BottomPositioned){
					computed.OffsetTop=computed.MarginTop+relativeTo.OffsetTop-relativeTo.StyleOffsetTop-relativeTo.ScrollTop+relativeTo.PixelHeight-computed.PositionBottom-computed.PixelHeight;
				}else{
					computed.OffsetTop=computed.MarginTop+relativeTo.OffsetTop+relativeTo.StyleOffsetTop+relativeTo.ScrollTop+computed.PositionTop;
				}
				
				// Set relativeTo to this - this is because the kids of absolute objects are relative to the absolute object itself.
				relativeTo=computed;
			}
			
			// Push the transform to our stack, if we have one.
			if(computed.Transform!=null){
				// Add it to the stack:
				Document.Renderer.Transformations.Push(computed.Transform);
				// Update it:
				computed.Transform.RecalculateMatrix(computed);
			}
			
			// Great, it's good to go!
			computed.Render();
			
			if(KidsToRender!=null||HScrollbar||VScrollbar){
			
				BoxRegion parentBoundary=null;
				
				if(relativeTo==computed){
					// We changed who we're relative to.
					// Change the clipping boundary:
					Renderman renderer=Document.Renderer;
					parentBoundary=renderer.ClippingBoundary;
					
					renderer.SetBoundary(computed);
				}
				
				if(KidsToRender!=null){
					for(int i=0;i<KidsToRender.Count;i++){
						Element child=KidsToRender[i];
						if(child!=null){
							child.PositionGlobally(relativeTo);
						}
					}
				}
				
				if(HScrollbar){
					HorizontalScrollbar.Element.PositionGlobally(relativeTo);
				}
				
				if(VScrollbar){
					VerticalScrollbar.Element.PositionGlobally(relativeTo);
				}

				
				if(relativeTo==computed){
					// Restore the previous boundary before this one: [Note - can't use SetBoundary here as it would destroy the box.]
					Document.Renderer.ClippingBoundary=parentBoundary;
				}
			}
			
			if(computed.Transform!=null){
				// Pop it off again:
				Document.Renderer.Transformations.Pop();
			}
		}
		
		/// <summary>Internal use only. <see cref="PowerUI.Element.formElement"/>.
		/// Scans up the DOM to find the parent form element.</summary>
		/// <returns>The parent form element, if found.</returns>
		public Element GetForm(){
			if(Tag=="form"){
				return this;
			}
			if(ParentNode==null){
				return null;
			}
			return ParentNode.GetForm();
		}
		
		/// <summary>Allocates all text characters that are contained by this element.</summary>
		public virtual void AllocateText(){
			if(!IsRebuildingChildren){
				KidsToRender=ChildNodes;
			}
			
			if(KidsToRender!=null){
				for(int i=0;i<KidsToRender.Count;i++){
					KidsToRender[i].AllocateText();
				}
			}
			
			if(HScrollbar){
				HorizontalScrollbar.Element.AllocateText();
			}
			
			if(VScrollbar){
				VerticalScrollbar.Element.AllocateText();
			}
		}
		
		/// <summary>Positions this element and all it's children relative to their parent.</summary>
		public void PositionLocally(){
			// This is the first pass of layout requests.
			// It locates elements locally whilst also finding their width and height.
			ComputedStyle computed=Style.Computed;
			
			if(computed.Display==DisplayType.None){
				// Don't draw this element or it's kids.
				return;
			}
			
			// If it's a word, we want to calc it's width here.
			if(computed.Text!=null){
				computed.Text.SetWidth();
			}
			
			float depth=Document.Renderer.Depth;
			float maxDepth=Document.Renderer.MaxDepth;
			bool elementPositioned=false;
			
			if(!computed.FixedDepth){
				elementPositioned=(computed.IsOffset() || computed.Position!=PositionType.Relative);
				
				if(elementPositioned){
					// This element has been positioned - make sure it's ontop of the current highest element:
					Document.Renderer.Depth=Document.Renderer.MaxDepth;
					
					if(Document.Renderer.DepthUsed){
						Document.Renderer.IncreaseDepth();
					}else{
						Document.Renderer.DepthUsed=true;
					}
					
					computed.ZIndex=Document.Renderer.Depth;
					
				}else{
					computed.ZIndex=depth;
				}
			}else{
				computed.ZIndex=computed.FixedZIndex;
			}
			
			if(KidsToRender!=null || HScrollbar || VScrollbar){
				if(computed.FixedDepth){
					// Set the depth buffer to this element so it's kids are at the right height; restore it after.
					
					// Offset by the document's depth if we're in a document in a document (e.g. iframe):
					Element documentParent=Document.html.parentNode;
					
					if(documentParent!=null && documentParent!=this){
						computed.ZIndex+=documentParent.Style.Computed.ZIndex;
					}
					
					Document.Renderer.Depth=computed.ZIndex;
					
					if(computed.ZIndex>Document.Renderer.MaxDepth){
						Document.Renderer.MaxDepth=Document.Renderer.Depth;
					}
				}else if(computed.BGImage!=null || computed.BGColour!=null){
					// Only increase the depth if the element has a background image/colour to get it's kids away from.
					Document.Renderer.IncreaseDepth();
				}
			}
			
			if(KidsToRender!=null){
				for(int i=0;i<KidsToRender.Count;i++){
					KidsToRender[i].PositionLocally();
				}
			}
			
			if(HScrollbar){
				HorizontalScrollbar.Element.PositionLocally();
			}
			
			if(VScrollbar){
				VerticalScrollbar.Element.PositionLocally();
			}
			
			// Restore the depth:
			if(elementPositioned){
				// This element has been positioned - everything after it must be ontop of it and all it's kids.
				Document.Renderer.Depth=Document.Renderer.MaxDepth;
				Document.Renderer.IncreaseDepth();
			}else if(computed.FixedDepth){
				Document.Renderer.Depth=depth;
				Document.Renderer.MaxDepth=maxDepth;
			}
			
			if(computed.Display==Css.DisplayType.Inline&&computed.Position==Css.PositionType.Relative){
				// Relative Inline - The kids will be packed onto the next element up's lines (this is done internally by the PackOnLine function).
				// Fixed or absolute pack their own lines.
				if(computed.BGImage==null&&computed.BGColour==null&&computed.Border==null){
					// Only occurs though if the element has no background or border - if it does, it should act like an inline-block element.
					return;
				}
			}
			
			Document.Renderer.BeginLinePack(this);
			
			if(KidsToRender!=null || HScrollbar || VScrollbar){
				
				if(KidsToRender!=null){
					for(int i=0;i<KidsToRender.Count;i++){
						Document.Renderer.PackOnLine(KidsToRender[i]);
					}
				}
				if(HScrollbar){
					Document.Renderer.PackOnLine(HorizontalScrollbar.Element);
				}
				
				if(VScrollbar){
					Document.Renderer.PackOnLine(VerticalScrollbar.Element);
				}
			}
			
			Document.Renderer.EndLinePack(this);
		}
		
		/// <summary>Gets the first child element with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The first child with the tag.</returns>
		public Element getElementByTagName(string tag){
			List<Element> results=getElementsByTagName(tag,true);
			if(results.Count>0){
				return results[0];
			}
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public List<Element> getElementsByTagName(string tag){
			return getElementsByTagName(tag,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <param name="stopWithOne">True if the search should stop when one is found.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public List<Element> getElementsByTagName(string tag,bool stopWithOne){
			List<Element> results=new List<Element>();
			getElementsByTagName(tag,stopWithOne,results);
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public bool getElementsByTagName(string tag,bool stopWithOne,List<Element> results){
			if(ChildNodes==null){
				return false;
			}
			
			for(int i=0;i<ChildNodes.Count;i++){
				Element child=ChildNodes[i];
				if(child==null){
					continue;
				}
				if(child.Tag==tag){
					// Yep, this has it.
					results.Add(child);
					if(stopWithOne){
						return true;
					}
				}
				if(child.getElementsByTagName(tag,stopWithOne,results)){
					// Hit the breaks - stop right here.
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Gets all elements with the given class name(s), seperated by spaces.
		/// May include this element or any of it's kids.</summary>
		/// <param name="className">The name of the classes to find. E.g. "red box".</param>
		/// <returns>A list of all matches.</returns>
		public List<Element> getElementsByClassName(string className){
			List<Element> results=new List<Element>();
			getElementsByClassName(className.Split(' '),results);
			return results;
		}
		
		/// <summary>Gets all elements with the given class name(s).
		/// May include this element or any of it's kids.</summary>
		/// <param name="classes">The name of the classes to find. No duplicates allowed.</param>
		/// <param name="results">The set into which the results are placed.</param>
		public void getElementsByClassName(string[] classes,List<Element> results){
			
			// Grab this elements class names:
			string thisClassName=this["class"];
			
			// Can it be split up?
			if(thisClassName!=null && thisClassName.Contains(" ")){
				// Yep - split them up:
				string[] thisClassNames=thisClassName.Split(' ');
				
				// Are we only looking for one? If so, skip a double loop.
				if(classes.Length==1){
					// Grab the one and only we're looking for:
					string classToFind=classes[0];
					
					for(int t=0;t<thisClassNames.Length;t++){
						if(thisClassNames[t]==classToFind){
							results.Add(this);
							break;
						}
					}
					
				}else if(classes.Length<=thisClassNames.Length){
					// Otherwise we're looking for more than we actually have.
					
					bool add=true;
					
					// For each one we're looking for..
					for(int i=0;i<classes.Length;i++){
						// Is it in this elements set?
						bool inSet=false;
						
						// For each of this elements class names..
						for(int t=0;t<thisClassNames.Length;t++){
							if(thisClassNames[t]==classes[i]){
								// Yep, it's in there!
								inSet=true;
								break;
							}
						}
						
						if(!inSet){
							add=false;
							break;
						}
					}
					
					if(add){
						// Add it in:
						results.Add(this);
					}
				}
				
			}else if(classes.Length==1){
				// Single one - special case here (for speed purposes):
				// This is because this element only has one class value, 
				// thus if we're looking for 2 it can't possibly match.
				if(classes[0]==thisClassName){
					// Add it in:
					results.Add(this);
				}
			}
			
			// Any kids got it?
			if(ChildNodes==null){
				return;
			}
			
			for(int i=0;i<ChildNodes.Count;i++){
				ChildNodes[i].getElementsByClassName(classes,results);
			}
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public List<Element> getElementsByAttribute(string property,string value){
			List<Element> results=new List<Element>();
			getElementsWithProperty(property,value,results);
			return results;
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="attribute">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public List<Element> getElementsWithProperty(string property,string value){
			List<Element> results=new List<Element>();
			getElementsWithProperty(property,value,results);
			return results;
		}
		
		/// <summary>Gets all elements with the given property. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the property to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the property should be; null for any value.</param>
		/// <param name="results">The set of elements to add results to.</param>
		public void getElementsWithProperty(string property,string value,List<Element> results){
			
			if(value==null){
				// It just needs to exist.
				if(Properties.ContainsKey(property)){
					results.Add(this);
				}
			}else if(this[property]==value){
				results.Add(this);
			}
			// Any kids got it?
			if(ChildNodes==null){
				return;
			}
			
			for(int i=0;i<ChildNodes.Count;i++){
				ChildNodes[i].getElementsWithProperty(property,value,results);
			}
		}
		
		/// <summary>Gets an element with the given property. May be this element or any of it's kids.</summary>
		/// <param name="property">The name of the property to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the property should be; null for any value.</param>
		/// <returns>The first element found that matches.</returns>
		public Element getElementWithProperty(string property,string value){
			return getElementByAttribute(property,value);
		}
		
		/// <summary>Gets an element with the given attribute. May be this element or any of it's kids.</summary>
		/// <param name="property">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>The first element found that matches.</returns>
		public Element getElementByAttribute(string property,string value){
			if(value==null){
				// It just needs to exist.
				if(Properties.ContainsKey(property)){
					return this;
				}
			}else{
				if(this[property]==value){
					return this;
				}
			}
			// Any kids got it?
			if(ChildNodes!=null){
				for(int i=0;i<ChildNodes.Count;i++){
					Element result=ChildNodes[i].getElementWithProperty(property,value);
					if(result!=null){
						return result;
					}
				}
			}
			return null;
		}
		
		/// <summary>Sets the tag and the tag handler for this element.</summary>
		/// <param name="tag">The new tag for this element, e.g. "span".</param>
		public override void SetTag(string tag){
			base.SetTag(tag);
			// Span is our default tag.
			Handler=TagHandlers.GetHandler(Tag,"span") as HtmlTagHandler;
			
			if(!SelfClosing){
				SelfClosing=Handler.SelfClosing();
			}
			Handler.Element=this;
			// Apply the tag style:
			Style.Computed.SetSelector(Css.SelectorType.Tag,Tag);
		}
		
		/// <summary>Gets the tag handler for this element.</summary>
		/// <returns>The tag handler.</returns>
		public override TagHandler GetHandler(){
			return Handler;
		}
		
		/// <summary>Performs a mouse over on the child elements of this element.</summary>
		/// <param name="mouseEvent">The event that represents where the mouse is.</param>
		/// <returns>True if any child elements had the mouse over it.</returns>
		public bool RunMouseOverOnKids(UIEvent mouseEvent){
			// Is the x/y co-ords on any of my kids?
			bool result=false;
			
			if(ChildNodes!=null){
				for(int i=ChildNodes.Count-1;i>=0;i--){
					// Backwards is important here! The one at the back is most likely the highest;
					// It's only z-index that might throw this off.
					if(ChildNodes[i].RunMouseOver(mouseEvent)){
						result=true;
					}
					if(mouseEvent.cancelBubble){
						return result;
					}
				}
			}
			
			if(HScrollbar){
				if(HorizontalScrollbar.Element.RunMouseOver(mouseEvent)){
					result=true;
				}
				if(mouseEvent.cancelBubble){
					return result;	
				}
			}
			
			if(VScrollbar){
				if(VerticalScrollbar.Element.RunMouseOver(mouseEvent)){
					result=true;
				}
				if(mouseEvent.cancelBubble){
					return result;	
				}
			}
			
			return result;
		}
		
		/// <summary>Performs a mouse over on this element.</summary>
		/// <param name="clickEvent">The event that represents where the mouse is.</param>
		/// <returns>True if this or any child has the mouse over it.</returns>
		public bool RunMouseOver(UIEvent mouseEvent){
			// Text elements ignore this entirely:
			if(GetType()==typeof(TextElement)){
				return false;
			}
			
			// Returns true if any element was clicked over.
			if(Handler.IgnoreClick || Style.Computed.Display==DisplayType.None){
				return false;
			}
			
			bool contains=Style.Computed.Contains(mouseEvent.clientX,mouseEvent.clientY);
			
			// Run on kids first:
			bool overKids=false;
			if(contains || Style.Computed.OverflowX==OverflowType.Visible || Style.Computed.OverflowY==OverflowType.Visible){
				overKids=RunMouseOverOnKids(mouseEvent);
			}
			
			if(mouseEvent.cancelBubble||Handler.IgnoreSelfClick){
				// True if the element doesn't want to accept clicks on itself (e.g. body/html tags).
				return overKids;
			}
			
			if(contains){
				bool result=MouseOver(mouseEvent);
				return (result || overKids);
			}
			
			return overKids;
		}
		
		/// <summary>Performs a click (mouse down or up) on this element.</summary>
		/// <param name="clickEvent">The event that represents where the mouse is.</param>
		/// <returns>True if this or any child accepted the click.</returns>
		public bool RunClick(UIEvent clickEvent){
			// Text elements ignore this entirely:
			if(GetType()==typeof(TextElement)){
				return false;
			}
			
			if(Handler.IgnoreClick || Style.Computed.Display==DisplayType.None){
				return false;
			}
			
			bool contains=Style.Computed.Contains(clickEvent.clientX,clickEvent.clientY);
			
			// Run on kids first:
			bool kidsResult=false;
			if(contains || Style.Computed.OverflowX==OverflowType.Visible || Style.Computed.OverflowY==OverflowType.Visible){
				kidsResult=RunClickOnKids(clickEvent);
			}
			
			if(clickEvent.cancelBubble||Handler.IgnoreSelfClick){
				// True if the element doesn't want to accept clicks on itself (e.g. body/html tags).
				return kidsResult;
			}
			
			if(contains){
				// Run click on this element:
				bool result=GotClicked(clickEvent);
				return (result || kidsResult);
			}
			return kidsResult;
		}
		
		/// <summary>Run a click on this element.</summary>
		/// <param name="clickEvent">The event that represents the mouse location.</param>
		/// <returns>True if this element accepted the click.</returns>
		public bool GotClicked(UIEvent clickEvent){
			clickEvent.target=this;
			
			bool handlerClick=Handler.OnClick(clickEvent);
			
			if(!handlerClick){
				return HasBackground;
			}
			
			return true;
		}
		
		/// <summary>Resolves any percentage widths for all child elements using the given parent element.</summary>
		/// <param name="parent">The computed style to base percentages on.</param>
		public void SetWidthForKids(ComputedStyle parent){
			SetDimensionForKids(parent,true);
		}
		
		/// <summary>Resolves any percentage heights for all child elements using the given parent element.</summary>
		/// <param name="parent">The computed style to base percentages on.</param>
		public void SetHeightForKids(ComputedStyle parent){
			SetDimensionForKids(parent,false);
		}
		
		/// <summary>Resolves any percentages for all child elements using the given parent element.</summary>
		/// <param name="parent">The computed style to base percentages on.</param>
		/// <param name="isWidth">True if we should use the width of the parent; false for height.</param>
		private void SetDimensionForKids(ComputedStyle parent,bool isWidth){
			int dimension=isWidth?parent.InnerWidth:parent.InnerHeight;
			
			if(ChildNodes!=null){
				for(int i=0;i<ChildNodes.Count;i++){
					ChildNodes[i].Style.Computed.SetParentDimension(dimension,isWidth,parent);
				}
			}
			
			if(HScrollbar){
				HorizontalScrollbar.Element.Style.Computed.SetParentDimension(dimension,isWidth,parent);
			}
			
			if(VScrollbar){
				VerticalScrollbar.Element.Style.Computed.SetParentDimension(dimension,isWidth,parent);
			}
		}
		
		/// <summary>Attempts to run a click on the children of this element.</summary>
		/// <param name="clickEvent">The click event which represents various properties of the mouse.</param>
		/// <returns>True if any child element consumed this event.</returns>
		public bool RunClickOnKids(UIEvent clickEvent){
			// Is the x/y co-ords on any of my kids?
			bool result=false;
			
			if(ChildNodes!=null){
				for(int i=ChildNodes.Count-1;i>=0;i--){
					// Backwards is important here! The one at the back is most likely the highest;
					// It's only z-index that might throw this off.
					if(ChildNodes[i].RunClick(clickEvent)){
						result=true;
					}
					if(clickEvent.cancelBubble){
						return result;
					}
				}
			}
			
			if(HScrollbar){
				if(HorizontalScrollbar.Element.RunClick(clickEvent)){
					result=true;
				}
				if(clickEvent.cancelBubble){
					return result;
				}
			}
			
			if(VScrollbar){
				if(VerticalScrollbar.Element.RunClick(clickEvent)){
					result=true;
				}
				if(clickEvent.cancelBubble){
					return result;
				}
			}
			
			return result;
		}
		
		/// <summary>Looks up the value for a named &variable;</summary>
		/// <param name="variableString">The &name; of the variable to find.</param>
		/// <returns>The variable value; null if it was not found.</returns>
		protected override string GetVariableValue(string variableString){
			return UI.Variables.GetValue(variableString);
		}
		
		/// <summary>Generates a new html element.</summary>
		/// <returns>A new html element.</returns>
		protected override MLElement CreateTagElement(MLLexer lexer){
			Element tag=new Element(Document,lexer,this);
			if(tag.Handler!=null && tag.Handler.Junk()){
				// Junk tag - prevent it entering the DOM.
				return tag;
			}
			AppendNewChild(tag);
			return tag;
		}
		
		/// <summary>Generates a new variable element.</summary>
		/// <returns>A new html variable element.</returns>
		protected override MLVariableElement CreateVariableElement(){
			VariableElement result=new VariableElement(Document,this);
			AppendNewChild(result);
			return result;
		}
		
		/// <summary>Generates a new text element.</summary>
		/// <returns>A new html text element.</returns>
		protected override MLTextElement CreateTextElement(){
			TextElement result=new TextElement(Document,this);
			AppendNewChild(result);
			return result;
		}
		
		public override void ResetVariable(string name){
			if(Handler!=null){
				Handler.OnResetAllVariables();
			}
			if(ChildNodes==null){
				return;
			}
			
			for(int i=0;i<ChildNodes.Count;i++){
				ChildNodes[i].ResetVariable(name);
			}
		}
		
		/// <summary>Requests all child elements to reload their &variables; if they have any.</summary>
		public override void ResetAllVariables(){
			if(Handler!=null){
				Handler.OnResetAllVariables();
			}
			if(ChildNodes==null){
				return;
			}
			
			for(int i=0;i<ChildNodes.Count;i++){
				ChildNodes[i].ResetAllVariables();
			}
		}
		
		/// <summary>Converts this elements content to its pure text format (no html will be in the output).</summary>
		/// <returns>The text only content of this element.</returns>
		public virtual string ToTextString(){
			return textContent;
		}
		
		/// <summary>Converts this element and it's content to it's html representitive.</summary>
		/// <returns>This element and its children as a html string.</returns>
		public override string ToString(){
			string result=base.ToString();
			if(!SelfClosing){
				result+=innerHTML+"</"+Tag+">";
			}
			return result;
		}
		
		/// <summary>Appends the given literal text to the content of this element.
		/// This is good for preventing html injection as the text will be taken literally.</summary>
		/// <param name="text">The literal text to append.</param>
		public void appendTextContent(string text){
			if(string.IsNullOrEmpty(text)){
				return;
			}
			MLLexer lexer=new MLLexer(text);
			ReadContent(lexer,false,true);
		}
		
		/// <summary>Appends the given html text to the content of this element.</summary>
		/// <param name="text">The html text to append.</param>
		public void appendInnerHTML(string text){
			if(string.IsNullOrEmpty(text)){
				return;
			}
			MLLexer lexer=new MLLexer(text);
			ReadContent(lexer,false,false);
		}
		
		/// <summary>Gets or sets the text content of this element (i.e. the content without any html.).
		/// Setting this is good for preventing any html injection as it will be taken literally.</summary>
		public string textContent{
			get{
				string result="";
				if(ChildNodes!=null){
					for(int i=0;i<ChildNodes.Count;i++){
						result+=ChildNodes[i].ToTextString();
					}
				}
				return result;
			}
			set{
				IsRebuildingChildren=true;
				ChildNodes=null;
				if(!string.IsNullOrEmpty(value)){
					appendTextContent(value);
				}else{
					// Clearing children.
					Document.Renderer.RequestLayout();
				}
				IsRebuildingChildren=false;
			}
		}
		
		/// <summary>The css class attribute of this element. Won't ever be null.
		/// Note that it can potentially hold multiple names, e.g. "red button".</summary>
		public string className{
			get{
				string value=this["class"];
				
				if(value==null){
					return "";
				}
				
				return value;
			}
			set{
				this["class"]=value;
			}
		}
		
		/// <summary>The ID of this element. Won't ever be null.</summary>
		public string id{
			get{
				string value=this["id"];
				
				if(value==null){
					return "";
				}
				
				return value;
			}
			set{
				this["id"]=value;
			}
		}
		
		/// <summary>Submits the form this element is in.</summary>
		public void submit(){
			FormTag elementForm=form;
			
			if(elementForm!=null){
				elementForm.submit();
			}
		}
		
		/// <summary>Gets or sets the innerHTML of this element.</summary>
		public string innerHTML{
			get{
				if(Document.AotDocument){
					return "";
				}
				string result="";
				if(ChildNodes!=null){
					for(int i=0;i<ChildNodes.Count;i++){
						result+=ChildNodes[i].ToString();
					}
				}
				return result;
			}
			set{
				if(Tag=="body"){
					Document.ClearStyle();
					Document.ClearCode();
				}
				
				IsRebuildingChildren=true;
				ChildNodes=null;
				
				if(!string.IsNullOrEmpty(value)){
					appendInnerHTML(value);
				}else{
					// Clearing children.
					Document.Renderer.RequestLayout();
				}
				
				IsRebuildingChildren=false;
				if(Tag=="body"){
					Document.NewBody();
				}
			}
		}
		
		/// <summary>The set of child elements of this element.</summary>
		public List<Element> childNodes{
			get{
				return ChildNodes;
			}
			set{
				if(ChildNodes==value){
					return;
				}
				ChildNodes=value;
				Document.Renderer.RequestLayout();
			}
		}
		
		/// <summary>Appends the given element defined as text.</summary>
		/// <param name="text">The element as text, e.g. "<div id='someNewElement'>".</param>
		/// <returns>The newly created element.</returns>
		public Element appendChild(string text){
			Element element=new Element(text,this);
			AppendNewChild(element);
			return element;
		}
		
		/// <summary>Adds the given element to the children of this element.</summary>
		/// <param name="element">The child element to add.</param>
		public void appendChild(Element element){
			// Append:
			AppendNewChild(element);
			// And update it's css by telling it the parent changed.
			// This affects inherit, height/width etc.
			element.style.Computed.ParentChanged();
		}
		
		/// <summary>Adds the given element to the children of this element.
		/// Note that this does not update CSS; it should be used for new elements only.</summary>
		/// <param name="element">The child element to add.</param>
		public void AppendNewChild(Element element){
			if(element==null){
				return;
			}
			element.ParentNode=this;
			element.Document=Document;
			
			if(ChildNodes==null){
				ChildNodes=new List<Element>();
			}
			ChildNodes.Add(element);
			Document.Renderer.RequestLayout();
		}
		
		/// <summary>True if this element is in any document.</summary>
		public bool isInDocument{
			get{
				if(ParentNode==null){
					// Top of the DOM.
					return true;
				}
				// When we hit a DOM element who's parent is unaware of this element as a child
				// then we have reached one that has been removed from the DOM.
				if(ParentNode.isChild(this)){
					return ParentNode.isInDocument;
				}
				return false;
			}
		}
		
		/// <summary>The document this element is on.</summary>
		public Document document{
			get{
				return Document;
			}
		}
		
		/// <summary>Applies to iframes. The document contained in the iframe itself.</summary>
		public Document contentDocument{
			get{
				return firstChild.document;
			}
		}
		
		/// <summary>Checks if the given element is a child of this element.</summary>
		/// <param name="childElement">The element to check if it's a child of this or not.</param>
		/// <returns>True if the given element is actually a child of this.</returns>
		public bool isChild(Element childElement){
			if(ChildNodes==null){
				return false;
			}
			
			for(int i=0;i<ChildNodes.Count;i++){
				if(ChildNodes[i]==childElement){
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Removes the given child from this element.</summary>
		/// <param name="element">The child element to remove.</param>
		public void removeChild(Element element){
			if(ChildNodes!=null){
				ChildNodes.Remove(element);
			}
			element.ParentNode=null;
			Document.Renderer.RequestLayout();
		}
		
		/// <summary>Scrolls the element by the given values.</summary>
		/// <param name="x">The change in x pixels.</param>
		/// <param name="y">The change in y pixels.</param>
		public void scrollBy(int x,int y){
			if(x==0&&y==0){
				return;
			}
			scrollTo(Style.Computed.ScrollLeft+x,Style.Computed.ScrollTop+y);
		}
		
		/// <summary>Scrolls the element to the given exact values.</summary>
		/// <param name="x">The x offset in pixels.</param>
		/// <param name="y">The y offset in pixels.</param>
		public void scrollTo(int x,int y){
			
			bool changed=false;
			
			if(y!=Style.Computed.ScrollTop){
				Style.Computed.ScrollTop=y;
				changed=true;
			}
			
			if(x!=Style.Computed.ScrollLeft){
				Style.Computed.ScrollLeft=x;
				changed=true;
			}
			
			if(changed){
				// Recompute the size:
				Style.Computed.SetSize();
				// And request a redraw:
				Document.Renderer.RequestLayout();
				if(VScrollbar){
					VerticalScrollbar.ElementScrolled();
				}else if(HScrollbar){
					HorizontalScrollbar.ElementScrolled();
				}
			}
			
		}
		
		/// <summary>Forces a layout to occur if one is required.
		/// You should almost never need to call this directly - it's only needed if you want to read the fully
		/// computed size of an element immediately after having updated it's style.</summary>
		public void RequireLayout(){
			Document.Renderer.Layout();
		}
		
		/// <summary>The x location of this element on the screen. Note that you may need to take scrolling into account (scrollLeft).</summary>
		public int offsetLeft{
			get{
				RequireLayout();
				return Style.Computed.ScrollLeft;
			}
		}
		
		/// <summary>The y location of this element on the screen. Note that you may need to take scrolling into account (scrollTop).</summary>
		public int offsetTop{
			get{
				RequireLayout();
				return Style.Computed.ScrollTop;
			}
		}
		
		/// <summary>The amount of pixels the content of this element is scrolled horizontally.</summary>
		public int scrollLeft{
			get{
				RequireLayout();
				return Style.Computed.ScrollLeft;
			}
			set{
				scrollTo(value,scrollTop);
			}
		}
		
		/// <summary>The amount of pixels the content of this element is scrolled vertically.</summary>
		public int scrollTop{
			get{
				RequireLayout();
				return Style.Computed.ScrollTop;
			}
			set{
				scrollTo(scrollLeft,value);
			}
		}
		
		/// <summary>The height of the content inside this element.</summary>
		public int contentHeight{
			get{
				RequireLayout();
				return Style.Computed.ContentHeight;
			}
		}
		
		/// <summary>The width of the content inside this element.</summary>
		public int contentWidth{
			get{
				RequireLayout();
				return Style.Computed.ContentWidth;
			}
		}
		
		/// <summary>The height of this element.</summary>
		public int pixelHeight{
			get{
				RequireLayout();
				return Style.Computed.PixelHeight;
			}
		}
		
		/// <summary>The width of this element.</summary>
		public int pixelWidth{
			get{
				RequireLayout();
				return Style.Computed.PixelWidth;
			}
		}
		
		/// <summary>The height of this element without margins or borders.</summary>
		public int scrollHeight{
			get{
				RequireLayout();
				return Style.Computed.InnerHeight;
			}
		}
		
		/// <summary>The width of this element without margins or borders.</summary>
		public int scrollWidth{
			get{
				RequireLayout();
				return Style.Computed.InnerWidth;
			}
		}
		
		/// <summary>Gets or sets the checked state of this radio/checkbox input. Note that 'checked' is a C# keyword, thus the uppercase.
		/// Nitro is not case-sensitive, so element.checked works fine there.</summary>
		public bool Checked{
			get{
				return (!string.IsNullOrEmpty(this["checked"]));
			}
			set{
				if(value){
					this["checked"]="1";
				}else{
					this["checked"]="";
				}
			}
		}
		
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
		
		/// <summary>Gets a rendering context for this canvas (if it is a canvas element!).</summary>
		/// <param name="text">The context type e.g. "2D".</param>
		public CanvasContext getContext(string text){
			if(Tag=="canvas"){
				return ((CanvasTag)Handler).getContext(text);
			}
			
			return null;
		}
		
		/// <summary>Gets or sets the image from the background of this element.</summary>
		public Texture2D image{
			get{
				if(Style.Computed.BGImage==null||Style.Computed.BGImage.Image==null){
					return null;
				}
				return Style.Computed.BGImage.Image.Image;
			}
			set{
				if(value==null){
					if(Style.Computed.BGImage!=null){
						Style.Computed.BGImage=null;
						Document.Renderer.RequestLayout();
					}
				}else{
					if(Style.Computed.BGImage==null){
						Style.Computed.BGImage=new BackgroundImage(this);
					}
					Style.Computed.BGImage.SetImage(value);
				}
			}
		}
		
		/// <summary>The first child of this element.</summary>
		public Element firstChild{
			get{
				if(ChildNodes==null || ChildNodes.Count==0){
					return null;
				}
				return ChildNodes[0];
			}
		}
		
		/// <summary>The last child of this element.</summary>
		public Element lastChild{
			get{
				if(ChildNodes==null || ChildNodes.Count==0){
					return null;
				}
				return ChildNodes[ChildNodes.Count-1];
			}
		}
		
		/// <summary>The sibling before this one under this elements parent. Null if this is the first child.</summary>
		public Element previousElementSibling{
			get{
				int index=childIndex;
				// No parent or it's the first one.
				if(index<=0){
					return null;
				}
				return ParentNode.childNodes[index-1];
			}
		}
		
		/// <summary>The sibling following this one under this elements parent. Null if this is the last child.</summary>
		public Element nextElementSibling{
			get{
				int index=childIndex;
				// No parent or it's the last one.
				if(index==-1 || index==ParentNode.childNodes.Count-1){
					return null;
				}
				return ParentNode.childNodes[index+1];
			}
		}
		
		/// <summary>The element before this one at this same level in the DOM tree.</summary>
		public Element previousSibling{
			get{
				if(ParentNode==null){
					return null;
				}
				// Get the element before this one under the parent:
				Element previous=previousElementSibling;
				if(previous!=null){
					return previous;
				}
				// This is the first child. Find the previous sibling of parent, then get it's last child.
				previous=ParentNode.previousSibling;
				if(previous==null){
					return null;
				}
				return previous.lastChild;
			}
		}
		
		/// <summary>The element after this one at this same level in the DOM tree.</summary>
		public Element nextSibling{
			get{
				if(ParentNode==null){
					return null;
				}
				// Get the element after this one under the parent:
				Element after=nextElementSibling;
				if(after!=null){
					return after;
				}
				// This is the last child. Find the next sibling of parent, then get it's first child.
				after=ParentNode.previousSibling;
				if(after==null){
					return null;
				}
				return after.firstChild;
			}
		}
		
		/// <summary>Scans up the DOM to find the parent form element.
		/// Note: <see cref="PowerUI.Element.form"/> may be more useful than the element iself.</summary>
		public Element formElement{
			get{
				return GetForm();
			}
		}
		
		/// <summary>Scans up the DOM to find the parent form element's handler.
		/// The object returned provides useful methods such as <see cref="PowerUI.FormTag.submit"/>. </summary>
		public FormTag form{
			get{
				Element formElement=GetForm();
				if(formElement==null){
					return null;
				}
				return ((FormTag)(formElement.Handler));
			}
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <param name="timeToAccelAndDecel">The time, in seconds, to take accelerating and decelerating.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime,float timeToAccelAndDecel){
			return animate(css,constantSpeedTime,timeToAccelAndDecel,timeToAccelAndDecel);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime){
			return animate(css,constantSpeedTime,0f,0f);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <param name="timeToAccelerate">The time, in seconds, to take accelerating.</param>
		/// <param name="timeToDecelerate">The time, in seconds, to take decelerating.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime,float timeToAccelerate,float timeToDecelerate){
			return new UIAnimation(this,css,constantSpeedTime,timeToAccelerate,timeToDecelerate);
		}
		
		/// <summary>Gets or sets if this element is focused.</summary>
		public bool focused{
			
			get{
				return (Input.Focused==this);
			}
			set{
				if(value==true){
					Focus();
				}else{
					Unfocus();
				}
			}
		}
		
		/// <summary>Gets the index of this element in it's parents childNodes.</summary>
		public int childIndex{
			get{
				if(ParentNode==null){
					return -1;
				}
				
				List<Element> kids=ParentNode.childNodes;
				
				for(int i=0;i<kids.Count;i++){
					if(kids[i]==this){
						return i;
					}
				}
				
				return -1;
			}
		}
		
		/// <summary>The number of child elements of this element.</summary>
		public int childElementCount{
			get{
				if(ChildNodes==null){
					return 0;
				}
				return ChildNodes.Count;
			}
		}
		
		/// <summary>Gets or sets the parent html element of this element.</summary>
		public Element parentNode{
			get{
				return ParentNode;
			}
			set{
				if(ParentNode!=null){
					ParentNode.removeChild(this);
				}
				if(value!=null){
					value.appendChild(this);
				}
			}
		}
		
		/// <summary>Gets the computed style of this element.</summary>
		public Css.ComputedStyle computedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Gets the style of this element.</summary>
		public Css.ElementStyle style{
			get{
				return Style;
			}
		}
		
	}
	
}