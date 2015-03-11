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
using PowerUI.Css;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// Automatically resizes the "actual" image to prevent wasting memory.
	/// With this, you can have one set of high-res images for all your devices and they'll just fit.
	/// Requests from Resources only.
	/// </summary>
	
	public class ResizeProtocol:ResourcesProtocol{
		
		public ResizeProtocol(){
			
			UseResolution=false;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"resize"};
		}
		
		public override void OnGetGraphic(ImagePackage package,FilePath path){
			
			// Already resized?
			ResizedImage resized=ResizedImages.Get(path.Path);
			
			if(resized!=null){
				
				// Sure is!
				package.GotGraphic(resized.Image);
				
				return;
				
			}
			
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
				
			}else{
				// Image
				Texture2D image=(Texture2D)Resources.Load(path.Directory+path.Filename);
				
				// Resize the image:
				resized=ResizedImages.Add(path.Path,image);
				
				if(image!=null){
					package.GotGraphic(resized.Image);
					return;
				}
				
			}
			
			package.GotGraphic("Image not found in resources ("+path.Directory+path.Filename+" from URL '"+path.Url+"').");
			
		}
		
	}
	
}