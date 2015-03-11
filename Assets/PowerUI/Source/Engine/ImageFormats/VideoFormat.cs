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

#if !MOBILE

using System;
using PowerUI.Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Represents the video format.
	/// </summary>
	
	public class VideoFormat:ImageFormat{
		
		/// <summary>The video retrieved.</summary>
		public MovieTexture Video;
		/// <summary>An isolated material for this image.</summary>
		public Material IsolatedMaterial;
		
		
		public override string[] GetNames(){
			return new string[]{"mov","mpg","mpeg","mp4","avi","asf","ogg","ogv"};
		}
		
		public override Material ImageMaterial{
			get{
				if(IsolatedMaterial==null){
					IsolatedMaterial=new Material(SPA.IsolationShader);
					IsolatedMaterial.SetTexture("_Sprite",Video);
				}
				
				return IsolatedMaterial;
			}
		}
		
		public override int Height{
			get{
				return Video.height;
			}
		}
		
		public override int Width{
			get{
				return Video.width;
			}
		}
		
	}
	
}

#endif