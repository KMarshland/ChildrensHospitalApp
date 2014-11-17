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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{
	
	/// <summary>A delegate used for callbacks when an image is done loading and can now be used.</summary>
	public delegate void OnImageReady(ImagePackage package);
	
	/// <summary>
	/// An object which holds and retrieves different types of graphics
	/// such as animations, videos (pro only) and textures.
	/// </summary>
	
	public class ImagePackage{
		
		/// <summary>The animation that is in use, if any.</summary>
		public SPA SPAFile;
		/// <summary>True if <see cref="PowerUI.ImagePackage.Video"/> was set.</summary>
		public bool IsVideo;
		/// <summary>Any error that occured whilst retrieving the graphic.</summary>
		public string Error;
		/// <summary>The file that was requested.</summary>
		public FilePath File;
		/// <summary>True if <see cref="PowerUI.ImagePackage.AnimationInstance"/> was set.</summary>
		public bool Animated;
		/// <summary>True if <see cref="PowerUI.ImagePackage.DynamicImage"/> was set.</summary>
		public bool IsDynamic;
		/// <summary>The type of file that was requested (e.g. "png" or "jpg")</summary>
		public string FileType;
		/// <summary>The image texture retrieved.</summary>
		public Texture2D Image;
		/// <summary>A custom data object for passing anything else when the callback runs.</summary>
		public object ExtraData;
		
		#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
		/// <summary>The video retrieved.</summary>
		public MovieTexture Video;
		#endif
		
		/// <summary>An instance of the animation retrieved.</summary>
		public SPAInstance Animation;
		/// <summary>True when an image should display itself whilst taking resolution into account.</summary>
		public bool PixelPerfect=true;
		/// <summary>The material the video is available from. See <see cref="PowerUI.ImagePackage.VideoMaterial"/>.</summary>
		private Material _VideoMaterial;
		/// <summary>The dynamic image retrieved.</summary>
		public DynamicTexture DynamicImage;
		/// <summary>The callback to run when the graphic has been retrieved (or an error occured).</summary>
		public event OnImageReady ImageReady;

		
		/// <summary>Creates a new text package for the named file to get.
		/// You must then call <see cref="PowerUI.TextPackage.Get"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		public ImagePackage(string src,string relativeTo){
			SetPath(src,relativeTo,true);
		}
		
		/// <summary>Creates a new text package for the named file to get.
		/// You must then call <see cref="PowerUI.TextPackage.Get"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		/// <param name="useResolution">True if the resolution string should be appended to the name.</param>
		public ImagePackage(string src,string relativeTo,bool useResolution){
			SetPath(src,relativeTo,useResolution);
		}
		
		/// <summary>Creates an image package containing the given image.</summary>
		/// <param name="image">The image for this image package. Used to display cached graphics.</param>
		public ImagePackage(Texture2D image){
			Image=image;
			SetPath(image.name,null);
		}
		
		/// <summary>Creates an image package containing the given dynamic image.</summary>
		/// <param name="image">The image for this image package. Used to display cached graphics.</param>
		public ImagePackage(DynamicTexture image){
			SetPath("",null);
			IsDynamic=true;
			DynamicImage=image;
		}
		
		/// <summary>If the package contains a video, this gets the material that the video will playback on.</summary>
		public Material VideoMaterial{
			get{
				return ImageMaterial;
			}
		}
		
		/// <summary>A material with just the single frame on it.</summary>
		public Material ImageMaterial{
			get{
				if(_VideoMaterial==null){
					_VideoMaterial=new Material(Shader.Find("PowerUI Animation Shader"));
					if(Image!=null){
						_VideoMaterial.SetTexture("_Sprite",Image);
					}else if(DynamicImage!=null){
						_VideoMaterial.SetTexture("_Sprite",DynamicImage.GetTexture());
						#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
					}else if(Video!=null){
						_VideoMaterial.SetTexture("_Sprite",Video);
						#endif
					}
				}
				return _VideoMaterial;
			}
		}
		
		/// <summary>The fully resolved URL requested.</summary>
		public string Url{
			get{
				return File.Url;
			}
		}
		
		/// <summary>Sets up the filepath to the given url which may be relative to a given location.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any. May be null.</param>
		/// <param name="useResolution">True if the resolution string should be appended to the name.</param>
		private void SetPath(string src,string relativeTo,bool useResolution){
			File=new FilePath(src,relativeTo,useResolution);
			FileType=File.Filetype.ToLower();
			PixelPerfect=File.UsedResolution;
		}
		
		/// <summary>Sets up the filepath to the given url which may be relative to a given location.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any. May be null.</param>
		private void SetPath(string src,string relativeTo){
			SetPath(src,relativeTo,true);
		}
	
		/// <summary>Sends the request off and defines a callback to run when the result is ready.</summary>
		/// <param name="imageReady">The callback to run when the graphic has been retrieved.
		/// Note that the callback must check if the result is <see cref="PowerUI.ImagePackage.Ok"/>.</param>
		public void Get(OnImageReady imageReady){
			ImageReady+=imageReady;
			
			if(string.IsNullOrEmpty(Url)){
				ImageReady(this);
				return;
			}
			
			// Exception - Is it an animation that has been cached?
			if(File.Filetype=="spa"){
				// Might already be loaded - let's check:
				SPA animation=SPA.Get(Url);
				if(animation!=null){
					//It's already been loaded - use that.
					GotGraphic(animation);
					return;
				}
			}
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=File.Handler;
			if(fileProtocol!=null){
				fileProtocol.OnGetGraphic(this,File);
			}
		}
		
		/// <summary>True if there is no error and the graphic is ok.</summary>
		public bool Ok{
			get{
				return (Error==null);
			}
		}
		
		/// <summary>True if there was an error and the graphic is not ok.</summary>
		public bool Errored{
			get{
				return (Error!=null);
			}
		}
		
		/// <summary>Called if the same resource was requested before so the image can be redisplayed quickly.
		/// This will always preceed some other result function.</summary>
		public void GotCached(ImagePackage package){
			Error=package.Error;
			if(package.Animated){
				GotGraphic(package.SPAFile);
			}else if(!package.IsVideo){
				GotGraphic(package.Image);
			}
		}
		
		/// <summary>Called by the file handler when the response errored.</summary>
		/// <param name="error">The error that occured.</param>
		public void GotGraphic(string error){
			Error=error;
			Wrench.Log.Add("PowerUI Image error: "+error);
			ImageReady(this);
		}
		
		/// <summary>Called by the file handler when a dynamic atlas texture was retrieved successfully.</summary>
		/// <param name="image">The image received.</param>
		public void GotGraphic(DynamicTexture image){
			Clear();
			IsDynamic=true;
			DynamicImage=image;
			ImageReady(this);
		}
		
		/// <summary>Called by the file handler when an animation was retrieved successfully.</summary>
		/// <param name="animation">The animation received.</param>
		public void GotGraphic(SPA animation){
			Clear();
			Animated=true;
			SPAFile=animation;
			ImageReady(this);
		}
		
		#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
		/// <summary>Called by the file handler when a video was retrieved successfully.</summary>
		/// <param name="video">The video received.</param>
		public void GotGraphic(MovieTexture movie){
			Clear();
			Video=movie;
			IsVideo=true;
			ImageReady(this);
		}
		#endif
		
		/// <summary>Called by the file handler when an image was retrieved successfully.</summary>
		/// <param name="image">The image received.</param>
		public void GotGraphic(Texture2D image){
			Clear();
			Image=image;
			ImageReady(this);
		}
		
		/// <summary>Called when this image is going to be displayed.</summary>
		public void GoingOnDisplay(){
			if(Animated&&Animation==null&&SPAFile!=null){
				Animation=SPAFile.GetInstance();
			}
		}
		
		/// <summary>Called when this image is no longer being displayed.</summary>
		public void GoingOffDisplay(){
			if(Animated&&Animation!=null){
				Animation.Stop();
				Animation=null;
			}
		}
		
		/// <summary>Removes all content from this image package.</summary>
		private void Clear(){
			// Clear any animation:
			GoingOffDisplay();
			Error=null;
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			Video=null;
			#endif
			Image=null;
			SPAFile=null;
			IsVideo=false;
			Animated=false;
			IsDynamic=false;
			DynamicImage=null;
		}
		
		/// <summary>Checks if this package contains something loaded and useable.</summary>
		/// <returns>True if there is a useable graphic in this package.</returns>
		public bool Loaded(){
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			return (DynamicImage!=null||Image!=null||SPAFile!=null||Video!=null);
			#else
			return (DynamicImage!=null||Image!=null||SPAFile!=null);
			#endif
		}
		
		/// <summary>Gets the width of the graphic in this package.
		/// Note that you should check if it is <see cref="PowerUI.ImagePackage.Loaded"/> first.</summary>
		/// <returns>The width of the graphic.</returns>
		public int Width(){
			if(IsDynamic){
				return DynamicImage.Width;
			}else if(Animated){
				// We want the width of a frame:
				return SPAFile.FrameWidth;
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			}else if(IsVideo){
				return Video.width;
			#endif
			}else{
				return Image.width;
			}
		}
		
		/// <summary>Gets the height of the graphic in this package.
		/// Note that you should check if it is <see cref="PowerUI.ImagePackage.Loaded"/> first.</summary>
		/// <returns>The height of the graphic.</returns>
		public int Height(){
			if(IsDynamic){
				return DynamicImage.Height;
			}else if(Animated){
				// We want the height of a frame:
				return SPAFile.FrameHeight;
			#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8
			}else if(IsVideo){
				return Video.height;
			#endif
			}else{
				return Image.height;
			}
		}
		
	}
	
}