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

namespace PowerUI{
	
	/// <summary>
	/// Handles the link tag commonly used for linking external style sheets.
	/// Note that this isn't for clickable links - that's the a tag as defined in html.
	/// The href must end in .css, or either rel="stylesheet" or type="text/css" must be defined.
	/// Otherwise, this tag is ignored by PowerUI.
	/// </summary>
	
	public class LinkTag:HtmlTagHandler{
		
		/// <summary>True if this links to CSS.</summary>
		public bool IsCSS;
		/// <summary>The external path to the CSS.</summary>
		public string Href;
		/// <summary>Style index is used to place style into the style buffer so it's placed correctly relative to all other style.
		/// This is required if this style takes longer to load than style after it.</summary>
		public int StyleIndex=-1;
		
		
		public override bool SelfClosing(){ 
			return true;
		}
		
		public override string[] GetTags(){
			return new string[]{"link"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new LinkTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="rel"){
				string rel=Element["rel"];
				if(rel!=null){
					if((rel.Trim().ToLower())=="stylesheet"){
						IsCSS=true;
					}
				}
				LoadContent();
			}else if(property=="type"){
				string type=Element["type"];
				if(type!=null){
					if((type.Trim().ToLower())=="text/css"){
						IsCSS=true;
					}
				}
				LoadContent();
			}else if(property=="href"){
				Href=Element["href"];
				if(!IsCSS){
					IsCSS=Href.ToLower().EndsWith(".css");
				}
				LoadContent();
			}
			
			return false;
		}
		
		/// <summary>Loads external CSS if a href is available and it's known to be css.</summary>
		public void LoadContent(){
			if(!IsCSS || string.IsNullOrEmpty(Href) || StyleIndex!=-1){
				return;
			}
			
			// Let's go get it!
			// The style index makes sure that this style is loaded into this position relative to other style on the page:
			StyleIndex=Element.Document.GetStyleIndex();
			TextPackage package=new TextPackage(Href,Element.Document.basepath);
			package.Get(OnTextReady);
		}
		
		/// <summary>The callback for the request to get the external style.</summary>
		/// <param name="package">The text package containing the style if the request was ok.</param>
		private void OnTextReady(TextPackage package){
			if(Element.Document==null || StyleIndex<0){
				return;
			}
			// The element is still somewhere on the UI.
			string style="";
			if(package.Ok){	
				// Grabbed it okay.
				style=package.Text;
			}
			Element.Document.AddStyle(style,StyleIndex);
			StyleIndex=-1;
		}
		
	}
	
}