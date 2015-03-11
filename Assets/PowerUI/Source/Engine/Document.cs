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
using Nitro;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>A delegate used when the title or tooltip of a document changes. 
	/// See Document.OnTitleChange and Document.OnTooltipChange.</summary>
	public delegate void TitleChange(Document document);
	
	/// <summary>A delegate used when the dimensions of this document changes.</summary>
	public delegate void OnSizeChange();
	
	/// <summary>A delegate used when a keydown/up is received.</summary>
	public delegate bool InputEvent(UIEvent e);
	
	
	/// <summary>
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use PowerUI.Document.innerHTML to set it's content.
	/// </summary>

	public partial class Document{
	
		/// <summary>The default style sheet. Contains styling for e.g. div/span etc.</summary>
		public static Css.StyleSheet DefaultStyleSheet;
		/// <summary>The HTML element of the document.
		/// This is the outermost tag of the document.</summary>
		public Element html;
		/// <summary>The body element of the document.
		/// It's contained within the html element. Set the innerHTML of this.</summary>
		public Element body;
		/// <summary>The window that this document belongs to.</summary>
		public Window window;
		/// <summary>The current location (i.e. base URL) of this document.
		/// Originates from the src attribute of iframes.</summary>
		public FilePath location;
		/// <summary>True if this is a Nitro AOT compilation document.</summary>
		public bool AotDocument;
		/// <summary>The renderer which will render this document.</summary>
		public Renderman Renderer;
		/// <summary>All code in script tags is buffered and compiled in one go. This is the buffer.</summary>
		public string[] CodeBuffer;
		/// <summary>An instance of the compiled code on this page.
		/// May also be null of there is no script on the page.</summary>
		public UICode CodeInstance;
		/// <summary>A global dropdown box for showing dropdown content. Note that this is only available on the top document.
		/// See <see cref="PowerUI.Window.top"/>.</summary>
		public Element DropdownBox;
		/// <summary>Some styles are loaded externally. Them and any styles after them are buffered to be loaded in order.</summary>
		public string[] StyleBuffer;
		/// <summary>True if we're done parsing and setting the innerHTML of the body tag.
		/// Used to guage when the code should be compiled.</summary>
		public bool FinishedParsing;
		/// <summary>The css stylesheet for this document.
		/// All style tags place their content into this stylesheet.</summary>
		public Css.StyleSheet Style;
		/// <summary>The title of the document. This originates from <title> tags.</summary>
		private string CurrentTitle;
		/// <summary>Only used by Nitro AOT. The location of the html file for error reporting.</summary>
		public string ScriptLocation;
		/// <summary>Called when the document resizes.</summary>
		public OnSizeChange OnResized;
		/// <summary>The tooltip of the document. This originates from <.. title="tooltip">. See Document.tooltip.</summary>
		private string CurrentTooltip;
		/// <summary>Called when a key goes up.</summary>
		public event InputEvent KeyUp;
		/// <summary>Called when a key goes down.</summary>
		public event InputEvent KeyDown;
		/// <summary>Called when the mouse moves.</summary>
		public event InputEvent MouseMove;
		/// <summary>Called when the title of this document changes.</summary>
		public TitleChange OnTitleChange;
		/// <summary>Called when the tooltip for this document changes.</summary>
		public TitleChange OnTooltipChange;
		/// <summary>A method called when any key is released anywhere. Note: this applies only to the main UI document (not world UI's).</summary>
		public DynamicMethod<Nitro.Void> onkeyup;
		/// <summary>A method called when the document resizes.</summary>
		public DynamicMethod<Nitro.Void> onresize;
		/// <summary>A method called when any key is pressed anywhere. Note: this applies only to the main UI document (not world UI's).</summary>
		public DynamicMethod<Nitro.Void> onkeydown;
		/// <summary>A method called when the mouse moves over this document.</summary>
		public DynamicMethod<Nitro.Void> onmousemove;
		/// <summary>A set of all fonts available to this renderer, indexed by font name.</summary>
		public Dictionary<string,DynamicFont> ActiveFonts;
		#if !NoNitroRuntime
		/// <summary>The nitro securty domain for this document.</summary>
		private NitroDomainManager SecurityDomain;
		#endif
		
		
		/// <summary>Creates a new document which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer to use when rendering this document.</param>
		public Document(Renderman renderer):this(renderer,null){}
		
		/// <summary>Creates a new document which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer to use when rendering this document.</param>
		/// <param name="parentWindow">The window that will become the parent window. Used in e.g. iframes.</param>
		public Document(Renderman renderer,Window parentWindow):this(renderer,parentWindow,false){}
		
		/// <summary>Creates a new document which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer to use when rendering this document.</param>
		/// <param name="parentWindow">The window that will become the parent window. Used in e.g. iframes.</param>
		/// <param name="aot">True if this is a Nitro AOT document (used in the Editor only).</param>
		public Document(Renderman renderer,Window parentWindow,bool aot):base(){
			AotDocument=aot;
			
			if(!aot && DefaultStyleSheet==null){
				// No default styles loaded yet. Load them now.
				string styleText=((TextAsset)Resources.Load("style")).text;
				// Have they applied any overrides?
				TextAsset extraStyle=Resources.Load("customStyle") as TextAsset;
				if(extraStyle!=null && extraStyle.text!=null){
					styleText+="\n\n"+extraStyle.text;
				}
				DefaultStyleSheet=new Css.StyleSheet(this);
				DefaultStyleSheet.ParseCss(styleText);
			}
			
			#if !NoNitroRuntime
			// Get the default security domain:
			SecurityDomain=UI.DefaultSecurityDomain;
			#endif
			
			Renderer=renderer;
			
			window=new Window();
			window.document=this;
			window.parent=parentWindow;
			if(parentWindow!=null){
				window.top=parentWindow.top;
			}else{
				window.top=window;
			}
			
			ActiveFonts=new Dictionary<string,DynamicFont>();
			Style=new Css.StyleSheet(this);
			html=new Element(this,null);
			html.SetTag("html");
			string ddbox="";
			
			if(parentWindow==null){
				// Dropdown box belongs to the top window only:
				ddbox="<ddbox></ddbox>";
			}
			
			html.innerHTML="<body></body>"+ddbox;
		}
		
		/// <summary>Gets the font with the given name. May load it from the cache or generate a new one.</summary>
		/// <param name="fontName">The name of the font to find.</param>
		/// <returns>A dynamic font if found; null otherwise.</returns>
		public DynamicFont GetOrCreateFont(string fontName){
			
			if(fontName==null || AotDocument){
				return null;
			}
			
			DynamicFont result;
			// Cache contains all available fonts for this document/ renderer.
			ActiveFonts.TryGetValue(fontName,out result);
			
			if(result==null){
				
				// Go get the font now:
				result=DynamicFont.Get(fontName);
				
				// And add it:
				ActiveFonts[fontName]=result;
				
			}
			
			return result;
		}
		
		/// <summary>Gets the font with the given name.</summary>
		/// <param name="fontName">The name of the font to find.</param>
		/// <returns>A dynamic font if found; null otherwise.</returns>
		public DynamicFont GetFont(string fontName){
			if(fontName==null){
				return null;
			}
			
			DynamicFont result;
			ActiveFonts.TryGetValue(fontName,out result);
			return result;
		}
		
		/// <summary>Writes the given html to the end of the document.</summary>
		/// <param name="text">The html to write.</param>
		public void write(string text){
			body.appendInnerHTML(text);
		}
		
		/// <summary>Clears the document of all it's content, including scripts and styles.</summary>
		public void clear(){
			
			ClearEvents();
			
			if(body!=null){
				// Gracefully clear the innerHTML.
				body.innerHTML="";
				return;
			}
			
			ClearCode();
			ClearStyle();
			
		}
		
		/// <summary>Clears all css style definitions from this document.</summary>
		public void ClearStyle(){
			Style=new Css.StyleSheet(this);
			StyleBuffer=null;
		}
		
		/// <summary>Clears all events on this document.</summary>
		public void ClearEvents(){
			
			KeyUp=null;
			KeyDown=null;
			MouseMove=null;
			
			OnTitleChange=null;
			OnTooltipChange=null;
			
			OnResized=null;
			
			onresize=null;
			onkeydown=null;
			onkeyup=null;
			onmousemove=null;
			
		}
		
		/// <summary>Runs the keyup events.</summary>
		/// <param name="e">The event which has occured.</param>
		public bool RunKeyUp(UIEvent e){
			
			if(KeyUp!=null && KeyUp(e)){
				return true;
			}
			
			if(onkeyup!=null){
				onkeyup.Run(e);
			}
			
			return false;
		}
		
		/// <summary>Called when the mouse moves over this document.</summary>
		/// <param name="e">The mouse event containing the position.</param>
		public bool RunMouseMove(UIEvent e){
			
			// Run mouse over on the HTML element (and internally bubbles to it's kids):
			bool result=html.RunMouseOver(e);
			
			// Run the mousemove C# event:
			if(MouseMove!=null){
				MouseMove(e);
			}
			
			// Run the Nitro event:
			if(onmousemove!=null){
				// Run the Nitro event:
				onmousemove.Run(e);
			}
			
			return result;
		}
		
		/// <summary>Runs the keydown events.</summary>
		/// <param name="e">The event which has occured.</param>
		public bool RunKeyDown(UIEvent e){
			
			if(KeyDown!=null && KeyDown(e)){
				return true;
			}
			
			if(onkeydown!=null){
				onkeydown.Run(e);
			}
			
			return false;
		}
		
		/// <summary>The path that this document is relative to (if any).</summary>
		public string basepath{
			get{
				if(location==null){
					return null;
				}
				
				return location.basepath;
			}
		}
		
		/// <summary>The title of the document. This originates from <title> tags.</summary>
		public string title{
			get{
				return CurrentTitle;
			}
			set{
				CurrentTitle=value;
				if(OnTitleChange!=null){
					OnTitleChange(this);
				}
			}
		}
		
		/// <summary>The tooltip of the document. This originates from <.. title="tooltip">.
		/// Note that this is set internally.</summary>
		public string tooltip{
			get{
				return CurrentTooltip;
			}
			set{
				if(CurrentTooltip==value){
					return;
				}
				
				CurrentTooltip=value;
				if(OnTooltipChange!=null){
					OnTooltipChange(this);
				}
			}
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public object this[string index]{
			get{
				if(CodeInstance==null){
					return null;
				}
				return CodeInstance[index];
			}
			set{
				if(CodeInstance==null){
					return;
				}
				CodeInstance[index]=value;
			}
		}
		
		/// <summary>Creates a new element in this document. You'll need to parent it to something.
		/// E.g. with thisDocument.body.appendChild(...). Alternative to innerHTML and appendInnerHTML.</summary>
		/// <param name='tag'>The tag, e.g. <div id='myNewElement' .. ></param>
		public Element createElement(string tag){
			Element result=new Element(tag,body);
			result.OnChildrenLoaded();
			result.GetHandler().OnTagLoaded();
			return result;
		}
		
		/// <summary>A shortcut for calling the nitro OnWindowOpen function.</summary>
		/// <param name="extra">Additional parameters to pass into the nitro domain.</param>
		/// <returns>The value that OnWindowOpen returned, if any.</returns>
		public object OnWindowOpen(params object[] extra){
			return RunLiteral("onwindowopen",body,extra,true);
		}
		
		/// <summary>Runs a nitro function by name with optional arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">Optional arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object Run(string name,params object[] args){
			return RunLiteral(name,body,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object[] args){
			return RunLiteral(name,body,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="element">The element to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,Element element,object[] args){
			return RunLiteral(name,element,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object[] args,bool optional){
			return RunLiteral(name,body,args,optional);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="element">The element to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,Element element,object[] args,bool optional){
			if(string.IsNullOrEmpty(name)||CodeInstance==null){
				return null;
			}
			CodeInstance.This=element;
			return CodeInstance.RunLiteral(name,args,optional);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="element">The element to use for the 'this' value.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunOptionally(string name,Element element,params object[] args){
			return RunLiteral(name,element,args,true);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunOptionally(string name,params object[] args){
			return RunLiteral(name,body,args,true);
		}
		
		/// <summary>Adds the given css style to the document. Used by style tags.</summary>
		/// <param name="css">The css to add to the document.</param>
		public void AddStyle(string css){
			AddStyle(css,-1);
		}
		
		/// <summary>Adds the given css style to the document. Used by style tags.</summary>
		/// <param name="css">The css to add to the document.</param>
		/// <param name="index">The index in the style buffer to add the css into.</param>
		public void AddStyle(string css,int index){
			if(index==-1){
				index=GetStyleIndex();
			}
			StyleBuffer[index]=css;
			TryStyle();
		}
		
		/// <summary>Gets a new style index in the StyleBuffer array.</summary>
		/// <returns>A new index in the StyleBuffer array.</returns>
		public int GetStyleIndex(){
			int length=0;
			if(StyleBuffer!=null){
				length=StyleBuffer.Length;
			}
			string[] newInstances=new string[length+1];
			for(int i=0;i<length;i++){
				newInstances[i]=StyleBuffer[i];
			}
			StyleBuffer=newInstances;
			return length;
		}
		
		/// <summary>Attempts to apply any added css. It's only successful if there are no nulls in the style buffer.</summary>
		/// <returns>Returns false if we're still waiting on css to download.</returns>
		public bool TryStyle(){
			if(StyleBuffer==null || AotDocument){
				return true;
			}
			if(StyleBuffer.Length==0){
				StyleBuffer=null;
				return true;
			}
			for(int i=0;i<StyleBuffer.Length;i++){
				if(StyleBuffer[i]==null){
					return false;
				}
			}
			// Good to go!
			string styleToCompile="";
			for(int i=0;i<StyleBuffer.Length;i++){
				styleToCompile+=StyleBuffer[i]+"\n";
			}
			StyleBuffer=null;
			// Parse it in now:
			Style.ParseCss(styleToCompile);
			return true;
		}
		
		
		/// <summary>Adds the given nitro code to the document. Used by script tags.
		/// Note that this will not work at runtime if the code if this document has already been compiled.</summary>
		/// <param name="code">The nitro code to add to the document.</param>
		public void AddCode(string code){
			AddCode(code,-1);
		}
		
		/// <summary>Adds the given nitro code to the document. Used by script tags.
		/// Note that this will not work at runtime if the code if this document has already been compiled.</summary>
		/// <param name="code">The nitro code to add to the document.</param>
		/// <param name="index">The index in the code buffer to add the code into.</param>
		public void AddCode(string code,int index){
			if(index==-1){
				index=GetCodeIndex();
			}
			CodeBuffer[index]=code;
		}
		
		/// <summary>Clears all code.</summary>
		public void ClearCode(){
			CodeBuffer=null;
			CodeInstance=null;
			FinishedParsing=false;
		}
		
		/// <summary>Gets a new code index in the CodeBuffer array.</summary>
		/// <returns>A new index in the CodeBuffer array.</returns>
		public int GetCodeIndex(){
			int length=0;
			if(CodeBuffer!=null){
				length=CodeBuffer.Length;
			}
			string[] newInstances=new string[length+1];
			for(int i=0;i<length;i++){
				newInstances[i]=CodeBuffer[i];
			}
			CodeBuffer=newInstances;
			return length;
		}
		
		/// <summary>Attempts to compile the code then run OnWindowLoaded. It's only successful if there are no nulls in the code buffer.</summary>
		/// <returns>Returns false if we're still waiting on code to download.</returns>
		public bool TryCompile(){
			
			if(CodeBuffer==null){
				return true;
			}
			
			if(CodeBuffer.Length==0){
				CodeBuffer=null;
				return true;
			}
			
			for(int i=0;i<CodeBuffer.Length;i++){
				if(CodeBuffer[i]==null){
					return false;
				}
			}
			
			#if !NoNitroRuntime
			// Iframe security check - can code from this domain run at all?
			// We have the Nitro runtime so it could run unwanted code.
			if(window.parent!=null && location!=null && !location.fullAccess){
				
				// It's an iframe to some unsafe location. We must have a security domain for this to be allowed at all.
				if(SecurityDomain==null || !SecurityDomain.AllowAccess(location.Protocol,location.host,location.ToString())){
					Wrench.Log.Add("Warning: blocked Nitro on a webpage - You must use a security domain to allow this. See http://help.kulestar.com/nitro-security/ for more.");
					return true;
				}
				
			}
			#endif
			
			// Good to go!
			FinishedParsing=false;
			string codeToCompile="";
			
			for(int i=0;i<CodeBuffer.Length;i++){
				codeToCompile+=CodeBuffer[i]+"\n";
			}
			
			CodeBuffer=null;
			
			if(!AotDocument){
				CodeInstance=NitroCache.TryGetScript(codeToCompile);
			}
			
			try{
				#if !NoNitroRuntime
				string aotFile=null;
				string aotAssemblyName=null;
				
				if(AotDocument){
					aotFile="";
					string[] pieces=ScriptLocation.Split('.');
					for(int i=0;i<pieces.Length-1;i++){
						if(i!=0){
							aotFile+=".";
						}
						aotFile+=pieces[i];
					}
					aotFile+="-nitro-aot.dll";
					aotAssemblyName=NitroCache.GetCodeSeed(codeToCompile)+".ntro";
				}
			
				if(CodeInstance==null){
					
					NitroCode script=new NitroCode(codeToCompile,UI.BaseCodeType,SecurityDomain,aotFile,aotAssemblyName);
					if(AotDocument){
						// Internally the constructor will write it to the named file.
						return true;
					}
					CodeInstance=(UICode)script.Instance();
					CodeInstance.BaseScript=script;
				}
				#endif
				
				if(CodeInstance!=null){
					CodeInstance.document=this;
					CodeInstance.window=window;
					CodeInstance.OnWindowLoaded();
					CodeInstance.Start();
					// Standard method that must be called.
					// Any code outside of functions gets dropped in here:
					CodeInstance.OnScriptReady();
				}
			}catch(Exception e){
				string scriptLocation=ScriptLocation;
				
				if(string.IsNullOrEmpty(scriptLocation)){
					// Use document.location instead:
					scriptLocation=location.ToString();
				}
				
				if(!string.IsNullOrEmpty(scriptLocation)){
					scriptLocation=" (At "+scriptLocation+")";
				}
				
				Wrench.Log.Add("Script error"+scriptLocation+": "+e);
			}
			return true;
		}
		
		/// <summary>An iteratable set of all elements from this document.
		/// Whilst iterating you can actively skip nodes, so it's often useful to cache this first.</summary>
		public DocumentElements allElements{
			get{
				return new DocumentElements(this);
			}
		}
		
		/// <summary>Gets the first child element with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The first child with the tag.</returns>
		public Element getElementByTagName(string tag){
			List<Element> results=html.getElementsByTagName(tag,true);
			
			if(results.Count>0){
				return results[0];
			}
			
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public List<Element> getElementsByTagName(string tag){
			return html.getElementsByTagName(tag,false);
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public List<Element> getElementsByAttribute(string property,string value){
			return html.getElementsByAttribute(property,value);
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public Element getElementByAttribute(string property,string value){
			return html.getElementByAttribute(property,value);
		}
		
		/// <summary>Gets all elements with the given class name(s), seperated by spaces.
		/// May include this element or any of it's kids.</summary>
		/// <param name="className">The name of the classes to find. E.g. "red box".</param>
		/// <returns>A list of all matches.</returns>
		public List<Element> getElementsByClassName(string className){
			return html.getElementsByClassName(className);
		}
		
		/// <summary>Gets the first html element found with the given ID attribute.</summary>
		/// <param name="id">The ID of the element to search for.</param>
		/// <returns>If found, a html element with the given ID; null otherwise.</returns>
		public Element getElementById(string id){
			return html.getElementByAttribute("id",id);
		}
		
		/// <summary>Gets a style definition by css selector from the StyleSheet.
		/// If it's not found in the style sheet, the default stylesheet is checked.</summary>
		/// <param name="selector">The css selector to search for.</param>
		/// <returns>If found, a selector style definition; null otherwise.</returns>
		public Css.SelectorStyle getStyleBySelector(string selector){
			if(Style==null || AotDocument){
				return null;
			}
			
			Css.SelectorStyle result=Style.GetStyleBySelector(selector);
			
			if(result==null){
				result=DefaultStyleSheet.GetStyleBySelector(selector);
			}
			
			return result;
		}
		
		/// <summary>Gets or sets the innerHTML of this document.</summary>
		public string innerHTML{
			get{
				if(html!=null){
					return html.ToString();
				}else{
					return "";
				}
			}
			set{
				// Overwrite the innerHTML - internally updates body/html elements if needed:
				body.innerHTML=value;
			}
		}
		
		/// <summary>Called after the innerHTML of the body tag was changed.</summary>
		public void NewBody(){
			
			// Is this an iframes document?
			if(window.iframe!=null){
				// Yes it is.
				// The new HTML may have contained a HTML tag and just replaced document.html with it.
				// In this case though, the original HTML tag is the child of the iframe element.
				// So, we need to check if the HTML tag changed.
				
				// Grab the original one:
				Element previousHtml=window.iframe.firstChild;
				
				// Have we got an original one, and has it changed?
				if(previousHtml!=null && html!=previousHtml){
					// Yep, it changed!
					
					// Remove all kids from the iframe:
					window.iframe.innerHTML="";
					
					// Put the new html node in the iframe:
					window.iframe.appendChild(html);
					
					// The html documents changed - reapply this to it:
					html.Document=this;
				}
			}
			
			if(DropdownBox!=null){
				// Change its parent:
				
				if(DropdownBox.parentNode!=null){
					DropdownBox.parentNode.removeChild(DropdownBox);
				}
				
				html.appendChild(DropdownBox);
			}
			
			TryStyle();
			
			if(!TryCompile()){
				// We're downloading code.
				// This flag lets the downloader know it needs to also attempt a TryCompile.
				FinishedParsing=true;
			}
			
		}
		
	}
	
}