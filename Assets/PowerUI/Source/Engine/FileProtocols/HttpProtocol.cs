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
using Wrench;
using PowerUI.Css;
using UnityEngine;
using UnityHttp;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// Handles the http:// protocol.
	/// Downloads files and text from the web and also handles web links.
	/// </summary>
	
	public class HttpProtocol:FileProtocol{
		
		
		public HttpProtocol(){
			
			UseResolution=false;
			
		}
		
		/// <summary>A temporary cache. This is used to briefly provide a previous result in the event of innerHTML being reloaded.
		/// The source file may still be requested though, depending on Unities WWW.</summary>
		public static Dictionary<string,ImagePackage> Cache=new Dictionary<string,ImagePackage>();
		
		
		/// <summary>Gets a previous result from the cache if one can be found.</summary>
		/// <param name="url">The url that will be requested.</param>
		public static ImagePackage GetFromCache(string url){
			ImagePackage result;
			Cache.TryGetValue(url,out result);
			return result;
		}
		
		/// <summary>Adds the given result to the cache for future request use.</summary>
		/// <param name="url">The url that will be requested.</param>
		/// <param name="result">The previous result to add to the cache.</param>
		public static void AddToCache(string url,ImagePackage result){
			if(result==null){
				return;
			}
			Cache[url]=result;
		}
		
		public override string[] GetNames(){
			return new string[]{"http","web","https"};
		}
		
		public override void OnGetGraphic(ImagePackage package,FilePath path){
			// Work like a proper browser - let's go get the image from the web.
			// Second is for video - it's called when the video is ready for playback but
			// Not necessarily fully downloaded.
			
			// First, was this graphic already requested?
			// If so, we'll provide that immediately.
			ImagePackage previousRequest=GetFromCache(path.Url);
			
			if(previousRequest!=null){
				// Great - provide this packages result first.
				// This prevents any flashing if innerHTML is loaded.
				package.GotCached(previousRequest);
				return;
			}
			
			HttpRequest request=new HttpRequest(path.Url,GotGraphicResult);
			request.OnRequestReady+=GotGraphicResult;
			request.ExtraData=package;
			request.Send();
		}
		
		public override void OnGetText(TextPackage package,FilePath path){
			// Work like a proper browser - Let's go grab text from the given url.
			// Note that this will only work with simple sites (no JS - nitro only) or ones built specifically for PowerUI.
			HttpRequest request=new HttpRequest(path.Url,GotTextResult);
			request.ExtraData=package;
			request.Send();
		}
		
		public override void OnFollowLink(Element linkElement,FilePath path){
			
			// Resolve the link elements target:
			Document targetDocument=ResolveTarget(linkElement);
			
			if(targetDocument==null){
				
				// Open the URL outside of Unity:
				Application.OpenURL(path.Url);
				
			}else{
				
				// Load into that document:
				LoadIntoDocument(path,targetDocument);
				
			}
			
		}
		
		/// <summary>Resolves the target in the given element (if any) to a document.</summary>
		/// <returns>The targeted document. Null if there is no document at all and the target is essentially outside of Unity.</returns>
		public Document ResolveTarget(Element linkElement){
			string target=linkElement["target"];
			
			// Grab the document the element is in:
			Document document=linkElement.document;
			
			if(target==null){
				// No target - does the document define a default one?
				// Note that this is set with the "base" html tag.
				
				if(document.location!=null){
					target=document.location.target;
				}
			}
			
			// Null target is the same as _self.
			if(string.IsNullOrEmpty(target)){
				target="_self";
			}
			
			// Grab the window:
			Window window=document.window;
			
			switch(target){
				case "_blank":
					
					// Open the given url outside Unity.
					return null;
				
				case "_top":
					// Open the given URL at the top window.
					
					return window.top.document;
					
				case "_parent":
					
					// Open it there:
					return window.parent.document;
					
				case "_self":
					
					// Open it in this document:
					return document;
					
				case "_main":
					
					// Open into the main UI:
					return UI.document;
					
				default:
					// Anything else and it's the name of an iframe (preferred) or a WorldUI.
					
					// Get the element by name:
					Element iframeElement=document.getElementByAttribute("name",target);
					
					if(iframeElement==null){
						
						// WorldUI with this name?
						WorldUI ui=WorldUI.Find(target);
						
						if(ui==null){
							
							// Not found - same as self:
							return document;
							
						}
						
						// Load into the WorldUI:
						return ui.document;
						
					}
					
					// Great, we have an iframe - grab the content document:
					return iframeElement.contentDocument;
					
			}
			
		}
		
		/// <summary>Loads a link into the given document.</summary>
		/// <param name="path">The path the link was pointing at.</param>
		/// <param name="document">The document the link will load into.</param>
		private void LoadIntoDocument(FilePath path,Document document){
			
			// Clear the document so it's obvious to the player the link is now loading:
			document.innerHTML="";
			document.location=path;
			
			// Load the html. Note that path.Url is fully resolved at this point:
			TextPackage package=new TextPackage(path.Url,"");
			package.ExtraData=document;
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
		
		public override void OnPostForm(FormData form,Element formElement,FilePath path){
			// Post to HTTP; Action is our URL.
			HttpRequest request=new HttpRequest(path.Url,OnFormSent);
			request.ExtraData=formElement;
			request.AttachForm(form.ToUnityForm());
			request.Send();
		}
		
		private void OnFormSent(HttpRequest request){
			Element element=(Element)request.ExtraData;
			
			// Attempt to run ondone:
			object result=element.Run("ondone",request);
			
			if(result!=null && result.GetType()==typeof(bool) && (((bool)result)==false) ){
				// The ondone function returned false. Don't load into a target at all.
				return;
			}
			
			// Load the result into target now.
			Document document=ResolveTarget(element);
			
			if(document==null){
				// Posting a form to an external target.
				
				Log.Add("Warning: Unity cannot post to external targets. The page will be loaded a second time.");
				
				// Open the URL outside of Unity:
				Application.OpenURL(request.Url);
				
			}else{
				
				if(request.Errored){
					if(ErrorHandlers.PageNotFound!=null){
						ErrorHandlers.PageNotFound(new HttpErrorInfo(request),document);
					}else{
						document.innerHTML="Error: "+request.Error;
					}
				}else{
					document.innerHTML=request.Text;
				}
				
			}
			
		}
		
		private void GotTextResult(HttpRequest request){
			TextPackage package=(TextPackage)request.ExtraData;
			
			package.GotText(request.Text,request.Error);
		}
		
		private void GotGraphicResult(HttpRequest request){
			ImagePackage package=(ImagePackage)request.ExtraData;
			
			if(request.Errored){
				package.GotGraphic(request.Error);
			}
			
			// Cache it:
			AddToCache(request.Url,package);
			
			string url=request.Url.ToLower();
			
			// Split by dot for the type:
			string[] pieces=url.Split('.');
			
			// Grab the type:
			string type=pieces[pieces.Length-1];
			
			
			if(type=="spa"){
				// Animation
				package.GotGraphic(new SPA(request.Url,request.Bytes));
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			}else if(ContentType.IsVideo(type)){
				// Video
				package.GotGraphic(request.Video);
			#endif
			}else if(request.Image!=null){
				// Image
				package.GotGraphic(request.Image);
			}else{
				package.GotGraphic(request.Text);
			}
		}
		
	}
	
}