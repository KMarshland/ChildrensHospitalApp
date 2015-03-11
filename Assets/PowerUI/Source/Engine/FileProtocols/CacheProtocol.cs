using System;
using PowerUI.Css;


namespace PowerUI{
	
	/// <summary>
	/// This protocol is used if you have a Texture2D object and want it on the screen.
	/// You must add the object to the ImageCache with a name, then use cache://thename to access it.
	/// </summary>
	
	public class CacheProtocol:FileProtocol{
		
		public CacheProtocol(){
			UseResolution=false;
		}
		
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
		public override string[] GetNames(){
			return new string[]{"cache","cac"};		
		}
		
		/// <summary>Attempts to get a graphic from the given location using this protocol.</summary>
		/// <param name="package">The image request. GotGraphic must be called on this when the protocol is done.</param>
		/// <param name="path">The location of the file to retrieve using this protocol.</param>
		public override void OnGetGraphic(ImagePackage package,FilePath path){
			if(path.Filetype=="spa"){
				// Grab a runtime SPA:
				package.GotGraphic(SPA.Get(path.Path));
				return;
			}
			
			package.GotGraphic(ImageCache.Get(path.Path));
		}
		
	}
	
}