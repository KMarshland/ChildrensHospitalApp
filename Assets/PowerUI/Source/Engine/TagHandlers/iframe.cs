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

using PowerUI.Css;

namespace PowerUI{
	
	/// <summary>
	/// Handles iframes.
	/// Supports the src="" attribute.
	/// </summary>
	
	public class IframeTag:HtmlTagHandler{
		
		/// <summary>The src of the page this iframe points to.</summary>
		public string Src;
		/// <summary>True if the tag for this iframe has been loaded.</summary>
		public bool Loaded;
		/// <summary>The document in this iframe.</summary>
		public Document ContentDocument;
		
		
		public IframeTag(){
			IsIsolated=true;
		}
		
		public override string[] GetTags(){
			return new string[]{"iframe"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new IframeTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				Src=Element["src"];
				LoadContent();
				return true;
			}
			
			return false;
		}
		
		/// <summary>Loads the content of this iframe now.</summary>
		private void LoadContent(){
			if(!Loaded){
				return;
			}
			
			string parentLocation=null;
			if(Element.parentNode!=null){
				parentLocation=Element.Document.basepath;
			}
			
			if(string.IsNullOrEmpty(Src)){
				ContentDocument.location=new FilePath("",parentLocation,false);
				SetContent("");
				return;
			}
			
			ContentDocument.location=new FilePath(Src,parentLocation,false);
			
			TextPackage package=new TextPackage(Src,parentLocation);
			package.Get(OnTextReady);
		}
		
		/// <summary>The callback used when the html text for this iframe has been loaded.</summary>
		/// <param name="package">The text package containing the html, if it is ok.</param>
		private void OnTextReady(TextPackage package){
			if(package.Errored){
				// Output it to our iframe:
				SetContent("Error: "+package.Error);
			}else{
				SetContent(package.Text);
			}
			Element.OnLoaded("webpage");
		}
		
		/// <summary>Sets the inner html of this iframe.</summary>
		/// <param name="text">The html to set.</param>
		private void SetContent(string text){
			if(!Loaded){
				return;
			}
			
			ContentDocument.innerHTML=text;
		}
		
		public override void OnTagLoaded(){
			Loaded=true;
			// Iframes generate a new document object for isolation purposes:
			ContentDocument=new Document(Element.Document.Renderer,Element.Document.window);
			// Setup the iframe ref:
			ContentDocument.window.iframe=Element;
			// Grab the parent document:
			Document originalDocument=Element.Document;
			// Temporarily set the document of this element:
			Element.Document=ContentDocument;
			// Append the documents html node as a child of the iframe:
			Element.appendChild(ContentDocument.html);
			
			Element.Document=originalDocument;
			
			LoadContent();
			// And handle style/ other defaults:
			base.OnTagLoaded();
		}
		
	}
	
}