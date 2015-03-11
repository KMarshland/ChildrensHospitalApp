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
			
			Input.Focusing=this;
			
			if(Input.Focused!=null){
				Input.Focused.Unfocus();
			}
			Input.Focused=this;
			
			#if MOBILE
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
			
			#if MOBILE
			// Attempt to hide the keyboard.
			Input.HandleKeyboard(null);
			#endif
			
			Document.window.Event=null;
			Handler.OnBlur();
		}
		
		/// <summary>Called by a tag handler when a key press occurs.</summary>
		/// <param name="clickEvent">The event that represents the key press.</param>
		public void OnKeyPress(UIEvent pressEvent){
			
			pressEvent.target=this;
			
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
			getElementsByAttribute(property,value,results);
			return results;
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="attribute">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public List<Element> getElementsWithProperty(string property,string value){
			List<Element> results=new List<Element>();
			getElementsByAttribute(property,value,results);
			return results;
		}
		
		/// <summary>Gets all elements with the given property. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the property to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the property should be; null for any value.</param>
		/// <param name="results">The set of elements to add results to.</param>
		public void getElementsByAttribute(string property,string value,List<Element> results){
			
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
				ChildNodes[i].getElementsByAttribute(property,value,results);
			}
		}
		
		/// <summary>Gets an element with the given property. May be this element or any of it's kids.</summary>
		/// <param name="property">The name of the property to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the property should be; null for any value.</param>
		/// <returns>The first element found that matches.</returns>
		/*
		public Element getElementWithProperty(string property,string value){
			return getElementByAttribute(property,value);
		}
		*/
		
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
					Element result=ChildNodes[i].getElementByAttribute(property,value);
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
		public override void ToString(System.Text.StringBuilder builder){
			base.ToString(builder);
			
			if(!SelfClosing){
				
				if(ChildNodes!=null){
					
					for(int i=0;i<ChildNodes.Count;i++){
						ChildNodes[i].ToString(builder);
					}
					
				}
				
				builder.Append("</");
				builder.Append(Tag);
				builder.Append(">");
			}
			
		}
		
		public override string ToString(){
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			ToString(builder);
			return builder.ToString();
		}
		
		/// <summary>Appends the given literal text to the content of this element.
		/// This is good for preventing html injection as the text will be taken literally.</summary>
		/// <param name="text">The literal text to append.</param>
		public void appendTextContent(string text){
			if(string.IsNullOrEmpty(text)){
				return;
			}
			MLLexer lexer=new MLLexer(text,true);
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
				
				if(ChildNodes!=null){
					ClearChildNodes();
				}
				
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
		
		/// <summary>Gets or sets the innerHTML of this element.</summary>
		public string innerHTML{
			get{
				if(Document.AotDocument){
					return "";
				}
				System.Text.StringBuilder result=new System.Text.StringBuilder();
				
				if(ChildNodes!=null){
					
					for(int i=0;i<ChildNodes.Count;i++){
						ChildNodes[i].ToString(result);
					}
					
				}
				
				return result.ToString();
			}
			set{
				if(Tag=="body"){
					Document.ClearEvents();
					Document.ClearStyle();
					Document.ClearCode();
				}
				
				IsRebuildingChildren=true;
				
				if(ChildNodes!=null){
					ClearChildNodes();
				}
				
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
		
		/// <summary>Called when this element got removed from the DOM.</summary>
		public void RemovedFromDOM(){
			
			if(Document.AttributeIndex!=null){
				
				// Remove this from the DOM attribute cache:
				Document.RemoveCachedElement(this);
				
			}
			
			// Let the style know we went offscreen:
			Style.Computed.WentOffScreen();
			
			if(HScrollbar){
				HorizontalScrollbar.Element.RemovedFromDOM();
			}
			
			if(VScrollbar){
				VerticalScrollbar.Element.RemovedFromDOM();
			}
			
			if(ChildNodes!=null){
				
				for(int i=0;i<ChildNodes.Count;i++){
					ChildNodes[i].RemovedFromDOM();
				}
				
			}
			
		}
		
		/// <summary>Called when this element goes offscreen.</summary>
		public void WentOffScreen(){
			
			Style.Computed.WentOffScreen();
			
			if(HScrollbar){
				HorizontalScrollbar.Element.WentOffScreen();
			}
			
			if(VScrollbar){
				VerticalScrollbar.Element.WentOffScreen();
			}
			
			if(ChildNodes!=null){
				
				for(int i=0;i<ChildNodes.Count;i++){
					ChildNodes[i].WentOffScreen();
				}
				
			}
			
		}
		
		/// <summary>Clears the child node set such that they no longer have a parent.</summary>
		private void ClearChildNodes(){
			
			int count=ChildNodes.Count;
			
			// For each child node..
			for(int i=0;i<count;i++){
				
				// Get the child:
				Element child=ChildNodes[i];
				
				// Clear it's parent node:
				// *Must be capitals!* .parentNode would be wrong.
				child.ParentNode=null;
				
				// Tell it that it's gone offscreen:
				child.WentOffScreen();
				
			}
			
		}
		
		/// <summary>The set of child elements of this element.</summary>
		public List<Element> childNodes{
			get{
				return ChildNodes;
			}
			
			set{
				if(ChildNodes==value){
					
					// If their equal, the order may still have changed.
					if(value==null){
						
						return;
						
					}
					
				}else{
					
					if(ChildNodes!=null){
						ClearChildNodes();
					}
					
					ChildNodes=value;
					
				}
				
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
			
			if(Document.AttributeIndex!=null){
				// Index element if needed:
				element.AddToAttributeLookups();
			}
			
			if(ChildNodes==null){
				ChildNodes=new List<Element>();
			}
			
			ChildNodes.Add(element);
			Document.Renderer.RequestLayout();
		}
		
		/*
		/// <summary>True if this element is in a document. Note that isRooted may be more useful.</summary>
		public bool isInDocument{
			get{
				
				return (Document!=null);
				
			}
		}
		*/
		
		/// <summary>True if this element is in any document and is rooted.</summary>
		public bool isRooted{
			get{
				
				if(Document==null){
					
					// Nope!
					return false;
					
				}
				
				// Grab the html node:
				Element htmlNode=Document.html;
				
				if(ParentNode==null){
					// Top of the DOM?
					return (htmlNode==this);
				}
				
				// Grab the parent:
				Element current=ParentNode;
				
				// While the current parent has a parent..
				while(current.ParentNode!=null){
					
					if(current==htmlNode){
						return true;
					}
					
					// Go to the next one - we know for sure it's not null.
					current=current.ParentNode;
					
				}
				
				// Check if it's the html node:
				return (current==htmlNode);
				
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
			element.RemovedFromDOM();
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
			set{
				style.left=value+"px";
			}
		}
		
		/// <summary>The y location of this element on the screen. Note that you may need to take scrolling into account (scrollTop).</summary>
		public int offsetTop{
			get{
				RequireLayout();
				return Style.Computed.ScrollTop;
			}
			set{
				style.top=value+"px";
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
				string check=this["checked"];
				
				if(!string.IsNullOrEmpty(check)){
					
					if(check=="0" || check.ToLower()=="false"){
						
						return false;
						
					}
					
					return true;
					
				}
				
				return false;
			}
			set{
				if(value){
					this["checked"]="1";
				}else{
					this["checked"]="";
				}
			}
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