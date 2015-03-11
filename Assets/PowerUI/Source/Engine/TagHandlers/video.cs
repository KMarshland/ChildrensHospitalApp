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
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{
	
	/// <summary>
	/// Handles a video. Note that videos can also be used for the css background-image property.
	/// You must also set the height and width of this element using either css or height="" and width="".
	/// </summary>
	
	public class VideoTag:HtmlTagHandler{
		
		public override string[] GetTags(){
			return new string[]{"video"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new VideoTag();
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				Element.style.backgroundImage=Element["src"];
				return true;
			}
			
			return false;
		}
		
		public override void OnChildrenLoaded(){
			// Does this video tag have <source> elements as kids?
			string src=Element["src"];
			
			if(src!=null){
				return;
			}
			
			// Grab the kids:
			List<Element> kids=Element.childNodes;
			
			if(kids==null){
				return;
			}
			
			// For each child, grab it's src value. Favours .ogg.
			
			foreach(Element child in kids){
				// Grab the src:
				string childSrc=child["src"];
				
				if(childSrc==null){
					continue;
				}
				
				#if !MOBILE
				// End with ogg, or do we have no source at all?
				if(src==null || childSrc.ToLower().EndsWith(".ogg")){
					src=childSrc;
				}
				#else
				// End with spa, or do we have no source at all?
				if(src==null || childSrc.ToLower().EndsWith(".spa")){
					src=childSrc;
				}
				#endif
				
			}
			
			if(src!=null){
				// Apply it now:
				Element.style.backgroundImage=src;
			}
			
		}
		
		#if !MOBILE
		/// <summary>The source movie texture.</summary>
		public MovieTexture video{
			get{
				// Grab the background image:
				BackgroundImage image=Element.style.Computed.BGImage;
				
				if(image==null || image.Image==null){
					// Not got a background image. Stop there.
					return null;
				}
				
				// Grab the video:
				return image.Image.Video;
			}
		}
		#endif
		
	}
	
	#if !MOBILE
	/// <summary>
	/// This class extends Element to include an easy to use element.video property (unavailable on mobile).
	/// </summary>
	
	public partial class Element{
		
		/// <summary>Gets the video tag associated with this element (if it's a video element).</summary>
		public VideoTag videoHandler{
			get{
				return (VideoTag)(GetHandler());
			}
		}
	
		/// <summary>The source movie texture.</summary>
		public MovieTexture video{
			get{
				return videoHandler.video;
			}
		}
		
		/// <summary>Is the video playing?</summary>
		public bool playing{
			get{
				return video.isPlaying;
			}
		}
		
		/// <summary>Is the video paused?</summary>
		public bool paused{
			get{
				return !video.isPlaying;
			}
		}
		
		/// <summary>Stops the video.</summary>
		public void stop(){
			MovieTexture movie=video;
			
			if(!movie.isPlaying){
				return;
			}
			
			movie.Stop();
			
			// Fire an onstop event:
			Run("onstop");
		}
		
		/// <summary>Pauses the video.</summary>
		public void pause(){
			MovieTexture movie=video;
			
			if(!movie.isPlaying){
				return;
			}
			
			movie.Pause();
			
			// Fire an onpause event:
			Run("onpause");
		}
		
		/// <summary>Plays the video.</summary>
		public void play(){
			MovieTexture movie=video;
			
			if(movie.isPlaying){
				return;
			}
			
			movie.Play();
			
			// Fire an onplay event:
			Run("onplay");
		}
		
		/// <summary>Gets the audio track of the video.</summary>
		public AudioClip audioTrack{
			get{
				return video.audioClip;
			}
		}
		
	}
	#endif
	
}