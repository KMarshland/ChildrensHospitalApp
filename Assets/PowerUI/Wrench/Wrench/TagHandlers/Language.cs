//--------------------------------------
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

namespace Wrench{
	
	/// <summary>
	/// This handles the language tag at the top of a language file.
	/// It must define the name and code of the language.
	/// </summary>
	
	public class LanguageTag:LanguageTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"language","lang"};
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override TagHandler GetInstance(){
			return new LanguageTag();
		}
		
		/// <summary>Applies this tag to the given language.</summary>
		/// <param name="language">The language that this tag came from.</param>
		/// <param name="element">The element from the language file that contains the language name and code as attributes.</param>
		public void Apply(LanguageSet language,LanguageElement element){
			language.Name=element["name"];
			string code=element["code"];
			if(code!=null){
				language.Code=(code).Trim().ToLower();
			}
			
			string direction=element["direction"];
			if(direction==null){
				direction=element["dir"];
			}
			
			if(direction!=null){
				direction=direction.Trim().ToLower();
				language.GoesLeftwards=(direction=="rtl" || direction=="righttoleft" || direction=="leftwards" || direction=="left");
			}else{
				language.GoesLeftwards=false;
			}
			
			string group=element["group"];
			if(group!=null){
				language.Group=group;
			}
		}
		
	}
	
}