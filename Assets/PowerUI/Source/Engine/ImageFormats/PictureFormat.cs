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
using Blaze;


namespace PowerUI{
	
	/// <summary>
	/// Represents the default "picture" format. Png, jpeg etc are handled with this.
	/// </summary>
	
	public class PictureFormat:ImageFormat{
		
		/// <summary>The image texture retrieved.</summary>
		public Texture2D Image;
		/// <summary>An isolated material for this image.</summary>
		public Material IsolatedMaterial;
		
		
		public override string[] GetNames(){
			return new string[]{"pict","jpg","jpeg","png","bmp","gif","tga","iff"};
		}
		
		public override int Height{
			get{
				return Image.height;
			}
		}
		
		public override int Width{
			get{
				return Image.width;
			}
		}
		
		public override Material ImageMaterial{
			get{
				if(IsolatedMaterial==null){
					IsolatedMaterial=new Material(SPA.IsolationShader);
					IsolatedMaterial.SetTexture("_Sprite",Image);
				}
				
				return IsolatedMaterial;
			}
		}
		
		public override bool DrawToAtlas(TextureAtlas atlas,AtlasLocation location){
			
			// Only ever called with a static image:
			Color32[] pixelBlock=Image.GetPixels32();
			
			int index=0;
			int atlasIndex=location.BottomLeftPixel();
			// How many pixels must we add on to the end of the row to get to
			// the start of the row above? This is RowPixelDelta:
			int rowDelta=location.RowPixelDelta();
			
			int height=Image.height;
			int width=Image.width;
			
			for(int h=0;h<height;h++){
				
				for(int w=0;w<width;w++){
					atlas.Pixels[atlasIndex++]=pixelBlock[index++];
				}
				
				atlasIndex+=rowDelta;
			}
			
			return true;
			
		}
		
	}
	
}