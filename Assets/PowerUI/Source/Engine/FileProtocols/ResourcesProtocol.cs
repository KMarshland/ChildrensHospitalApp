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
using PowerUI.Css;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// Handles the resources (default) protocol.
	/// Files here are loaded from the Unity 'Resources' folder in the project.
	/// Note that animation files must end in .bytes (e.g. animation.spa.bytes)
	/// and all images must be read/write enabled as well as have a "To power 2" of none.
	/// </summary>
	
	public class ResourcesProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"","resources","res"};
		}
		
		public override void OnGetGraphic(ImagePackage package,FilePath path){
			
			if(Callback.WillDelay){
				
				// Buffer this call until later - we're not on the main thread.
				
				// Create the callback:
				ResourcesProtocolCallback callback=new ResourcesProtocolCallback(package,path);
				
				// Hook up the protocol handler for speed later:
				callback.Protocol=this;
				
				// Request it to run:
				callback.Go();
				
				return;
			}
			
			if(path.Filetype=="spa"){
				// Animated.
				// Load the binary file - Note: the full file should be called file.spa.bytes for this to work in Unity.
				byte[] binary=((TextAsset)Resources.Load(path.Path)).bytes;
				
				if(binary!=null){
					// Apply it now:
					package.GotGraphic(new SPA(path.Url,binary));
					return;
				}
				
			#if !MOBILE
			}else if(ContentType.IsVideo(path.Filetype)){
				// Video
				MovieTexture movie=(MovieTexture)Resources.Load(path.Directory+path.Filename,typeof(MovieTexture));
				
				if(movie!=null){
					package.GotGraphic(movie);
					return;
				}
				
			#endif
			}else{
				// Image
				Texture2D image=(Texture2D)Resources.Load(path.Directory+path.Filename);
				
				if(image!=null){
					package.GotGraphic(image);
					return;
				}
				
			}
			
			package.GotGraphic("Image not found in resources ("+path.Directory+path.Filename+" from URL '"+path.Url+"').");
			
		}
		
		public override void OnGetData(DataPackage package,FilePath path){
			
			if(Callback.WillDelay){
				
				// Buffer this call until later - we're not on the main thread.
				
				// Create the callback:
				ResourcesProtocolCallback callback=new ResourcesProtocolCallback(package,path);
				
				// Hook up the protocol handler for speed later:
				callback.Protocol=this;
				
				// Request it to run:
				callback.Go();
				
				return;
			}
			
			// Getting a files text content from resources.
			byte[] data=null;
			string error=null;
			string resPath=path.Path;
			
			TextAsset asset=(TextAsset)Resources.Load(resPath);
			
			if(asset==null){
				
				error="File not found in resources ("+path.Directory+path.Filename+" from URL '"+path.Url+"'). Does your file in Unity end with .bytes?";
				
			}else{
				data=asset.bytes;
			}
			
			package.GotData(data,error);
			
		}
		
		public override void OnGetText(TextPackage package,FilePath path){
			
			if(Callback.WillDelay){
				
				// Buffer this call until later - we're not on the main thread.
				
				// Create the callback:
				ResourcesProtocolCallback callback=new ResourcesProtocolCallback(package,path);
				
				// Hook up the protocol handler for speed later:
				callback.Protocol=this;
				
				// Request it to run:
				callback.Go();
				
				return;
			}
			
			// Getting a files text content from resources.
			string text=null;
			string error=null;
			string resPath=null;
			string filetype=path.Filetype;
			
			if(filetype=="html" || filetype=="htm" || filetype=="txt"){
				resPath=path.Directory+path.Filename;
			}else{
				// The file MUST end in .bytes for this to work.
				resPath=path.Path;
			}
			
			TextAsset asset=(TextAsset)Resources.Load(resPath);
			
			if(asset==null){
				
				error="File not found in resources ("+path.Directory+path.Filename+" from URL '"+path.Url+"')";
				
				if(filetype=="css" || filetype=="ns" || filetype=="nitro"){
					error+=" Additionally, note that '."+filetype+"' files are not recognised by Unity as a text file. Try renaming the file to ."+filetype+".bytes in your Resources folder.";
				}
				
			}else{
				text=asset.text;
			}
			
			package.GotText(text,error);
			
		}
		
		public override void OnFollowLink(Element linkElement,FilePath path){
			string target=linkElement["target"];
			if(target!=null && target=="_blank"){
				// Open the given url.
				Application.OpenURL(path.Url);
				return;
			}
			
			// Clear the document so it's obvious to the player the link is now loading:
			linkElement.Document.innerHTML="";
			linkElement.Document.location=path;
			
			// Load the html. Note that path.Url is fully resolved at this point:
			TextPackage package=new TextPackage(path.Url,"");
			package.ExtraData=linkElement.Document;
			package.Get(GotLinkText);
		}
		
		private void GotLinkText(TextPackage package){
			Document document=(Document)package.ExtraData;
			
			if(package.Errored){
				
				if(ErrorHandlers.PageNotFound!=null){
					ErrorHandlers.PageNotFound(new FileErrorInfo(package),document);
				}else{
					document.innerHTML="Error: "+package.Error;
				}
				
			}else{
				document.innerHTML=package.Text;
			}
		}
		
		public override bool FullAccess(FilePath path){
			// This is entirely local and controlled by the developer - it's a safe protocol.
			return true;
		}
		
	}
	
}