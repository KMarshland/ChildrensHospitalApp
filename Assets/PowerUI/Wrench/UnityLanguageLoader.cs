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

using UnityEngine;

namespace Wrench{

	/// <summary>
	/// This class loads data for a language within Unity at runtime.
	/// The language must always be located in the Resources folder, under Resources/Languages/{path}.
	/// </summary>

	public class UnityLanguageLoader:LanguageLoader{
		
		private LanguageSet[] Languages;
		
		public UnityLanguageLoader(string path):base("Languages/"+path){}
		
		/// <summary>Gets the named language.</summary>
		/// <param name="code">The language code to find. Defined in the language tag at the top of the languages file.</param>
		protected override LanguageSet GetLanguage(string code){
			TextAsset asset=(TextAsset)(Resources.Load(Path+"/"+code,typeof(TextAsset)));
			
			if(asset==null){
				return null;
			}

			return new LanguageSet(asset.text,this);
		}
		
		/// <summary>Gets all available languages.</summary>
		protected override LanguageSet[] GetAllLanguages(){
			
			if(Languages==null){
				
				// Load all:
				object[] assets=Resources.LoadAll(Path,typeof(TextAsset));
				
				// Create the set:
				Languages=new LanguageSet[assets.Length];
				
				// For some reason Unity treats folders as TextAssets, so we've got to strip them if they exist.
				int directoryCount=0;
				
				for(int i=0;i<Languages.Length;i++){
					
					// Create the set:
					LanguageSet set=new LanguageSet(((TextAsset)assets[i]).text,this);
					
					// Folder?
					if(set.Code==null){
						
						// Yep - or no useable language code - skip.
						directoryCount++;
						
						continue;
						
					}
					
					// Write it:
					Languages[i]=set;
					
				}
				
				if(directoryCount!=0){
					
					// Resize:
					LanguageSet[] tempSet=new LanguageSet[Languages.Length-directoryCount];
					
					int index=0;
					
					// For each one..
					for(int i=0;i<Languages.Length;i++){
						
						// Grab the set:
						LanguageSet set=Languages[i];
						
						if(set==null){
							
							// It was skipped above.
							continue;
							
						}
						
						// Add to temp set:
						tempSet[index]=set;
						
						index++;
						
					}
					
					// Apply temp set to languages:
					Languages=tempSet;
					
				}
				
			}
			
			return Languages;
		}
		
	}
	
}