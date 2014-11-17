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
	/// Override this class to provide methods for loading language files.
	/// They may be delivered in many different ways because of this.
	/// </summary>

	public class LanguageLoader{
	
		/// <summary>The path to the location of the languages.</summary>
		public string Path;
		/// <summary>The set of all languages if they've all been loaded.</summary>
		private LanguageSet[] AllLanguagesLoaded;
		
		
		/// <summary>Creates a new loader for the given path.</summary>
		public LanguageLoader(string path){
			Path=path;
		}
		
		/// <summary>Loads a standard group by name.</summary>
		/// <param name="groupName">The name of the group.</param>
		/// <param name="code">The language code.</param>
		public virtual LanguageSet GetGroup(string groupName,string code){
			return GetLanguage(groupName.Replace('.','/')+"/"+code);
		}
		
		/// <summary>Gets the language with the given code. Called very rarely.</summary>
		/// <param name="code">The language code to look for.</param>
		/// <returns>A Language set if the language was found; null otherwise.</returns>
		public LanguageSet Get(string code){
			if(AllLanguagesLoaded!=null){
				foreach(LanguageSet data in AllLanguagesLoaded){
					if(data.Code==code){
						return data;
					}
				}
			}	
			
			// Not already loaded - pass up to parent.
			return GetLanguage(code);
		}
		
		/// <summary>Gets all available languages.</summary>
		/// <returns>The set of all available languages.</returns>
		public LanguageSet[] AllLanguages(){
			if(AllLanguagesLoaded!=null){
				return AllLanguagesLoaded;
			}
			
			// Load them now.
			AllLanguagesLoaded=GetAllLanguages();
			return AllLanguagesLoaded;
		}
		
		/// <summary>Gets all available languages.</summary>
		/// <returns>The set of all available languages.</returns>
		protected virtual LanguageSet[] GetAllLanguages(){
			return null;
		}
		
		/// <summary>Override this to get a language by given code.</summary>
		/// <param name="code">The language code (e.g. "en").</param>
		/// <returns>A language set if the language was found; null otherwise.</returns>
		protected virtual LanguageSet GetLanguage(string code){
			return null;
		}
		
	}
	
}