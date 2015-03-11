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
using System.Text;

namespace PowerUI{
	
	/// <summary>
	/// Handles script tags. They should have type="text/nitro" to be handled by PowerUI; Javascript is ignored.
	/// The src="" attribute is also supported if you wish to reuse script by loading it externally.
	/// </summary>
	
	public class ScriptTag:HtmlTagHandler{
		
		/// <summary>Dump is true if this script tag content should be completely ignored.</summary>
		public bool Dump;
		/// <summary>The external path to this script.</summary>
		public string Src;
		/// <summary>True if the tag is fully loaded; false if it's still being parsed.</summary>
		public bool Loaded;
		/// <summary>Code index is used to place script into the code buffer so it's placed correctly relative to all other code.
		/// This is required if this script takes longer to load than script after it.</summary>
		public int CodeIndex=-1;
		
		
		public override string[] GetTags(){
			return new string[]{"script","js"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ScriptTag();
		}
		
		public override void OnParseContent(MLLexer lexer){
			lexer.Literal=true;
			// Keep reading until we hit </script>.
			
			StringBuilder codeText=new StringBuilder();
			
			// Let the parser know what line it's really at:
			codeText.Append("\n#l"+(lexer.LineNumber-1)+"\n");
			
			// Get the initial "empty" size:
			int size=codeText.Length;
			
			while(!AtEnd(lexer)){
				codeText.Append(lexer.Read());
			}
			
			// Great, good stuff. Add to the Document's code but only if this tag shouldn't be dumped:
			if(!Dump){
				if(codeText.Length!=size){
					Element.Document.AddCode(codeText.ToString());
				}
			}else{
				Wrench.Log.Add("Warning: Some script has been ignored due to it's type ('"+Element["type"]+"'). Did you mean 'text/nitro'?");
			}
			lexer.Literal=false;
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				// Ignore any type containing J (for javascript).
				Dump=Element["type"].ToLower().Contains("j");
				return true;
			}else if(property=="src"){
				Src=Element["src"];
				LoadContent();
				return true;
			}
			
			return false;
		}
		
		/// <summary>Sends a request off to get the content of this tag if it's external (i.e. has an src attribute).</summary>
		private void LoadContent(){
			if(!Loaded || string.IsNullOrEmpty(Src)){
				return;
			}
			// The code index makes sure that this script is loaded into this position relative to other script on the page:
			CodeIndex=Element.Document.GetCodeIndex();
			TextPackage package=new TextPackage(Src,Element.Document.basepath);
			package.Get(OnTextReady);
		}
		
		/// <summary>The callback for the request to get the external script.</summary>
		/// <param name="package">The text package containing the script if the request was ok.</param>
		private void OnTextReady(TextPackage package){
			if(Element.Document==null || CodeIndex<0){
				return;
			}
			// The element is still somewhere on the UI.
			string code="";
			if(package.Ok){	
				// Grabbed it okay.
				code=package.Text;
			}
			Element.Document.AddCode(code,CodeIndex);
			CodeIndex=-1;
			// We might also be the last code to download - attempt a compile now, but only if the document is done parsing.
			// If the doc isn't done parsing, it might not have added all the script to the buffer yet (and will try compile itself).
			if(Element.Document.FinishedParsing){
				Element.Document.TryCompile();
			}
		}
		
		public override void OnTagLoaded(){
			Loaded=true;
			LoadContent();
			base.OnTagLoaded();
		}
		
		/// <summary>Checks if the given lexer has reached the end of the inline script content.</summary>
		/// <param name="lexer">The lexer to check if it's reached the &lt;/script&gt; tag.</param>
		/// <returns>True if the lexer has reached the end script tag; false otherwise.</returns>
		private bool AtEnd(MLLexer lexer){
			// Up to r will do; we're certainly at the end by that point.
			return (lexer.Peek()=='<'&&
					lexer.Peek(1)=='/'&&
					lexer.Peek(2)=='s'&&
					lexer.Peek(3)=='c'&&
					lexer.Peek(4)=='r'
					);
		}
		
	}
	
}