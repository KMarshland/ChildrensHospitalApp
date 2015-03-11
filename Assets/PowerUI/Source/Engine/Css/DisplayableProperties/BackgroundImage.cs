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
using UnityEngine;
using Blaze;

namespace PowerUI.Css{
	
	/// <summary>
	/// Represents the background image of an element.
	/// May also be a video (pro only) or animation.
	/// </summary>
	
	public partial class BackgroundImage:DisplayableProperty{
	
		/// <summary>How much to move the image over by on the x axis. % or px.</summary>
		public Css.Value OffsetX;
		/// <summary>How much to move the image over by on the y axis. % or px.</summary>
		public Css.Value OffsetY;
		/// <summary>The width of the image (background-size property).</summary>
		public Css.Value SizeX;
		/// <summary>The height of the image (background-size property).</summary>
		public Css.Value SizeY;
		/// <summary>True if the image should be repeated on the x axis.</summary>
		public bool RepeatX=true;
		/// <summary>True if the image should be repeated on the y axis.</summary>
		public bool RepeatY=true;
		/// <summary>The graphic to display.</summary>
		public ImagePackage Image;
		/// <summary>True if this image should be isolated regardless.</summary>
		public bool ForcedIsolate;
		/// <summary>The location of the image on an atlas if it's on one.</summary>
		public AtlasLocation ImageLocation;
		/// <summary>The filter mode to display the image with.</summary>
		public FilterMode Filtering=FilterMode.Point;
		
		
		/// <summary>Creates a new displayable background image for the given element.</summary>
		/// <summary>The element that will have a background image applied.</summary>
		public BackgroundImage(Element element):base(element){}
		
		public override void SetOverlayColour(Color colour){
			RequestPaint();
		}
		
		public bool Overlaps(BackgroundImage image){
			
			// Get the first image block:
			MeshBlock block=image.FirstBlock;
			
			if(block==null || FirstBlock==null){
				return false;
			}
			
			// Check if images verts are contained within any of mine.
			MeshBlock current=FirstBlock;
			
			while(current!=null){
				
				if(current.Overlaps(block)){
					
					return true;
				
				}
				
				current=current.LocalBlockAfter;
				
			}
			
			return false;
			
		}
		
		public override void OnBatchDestroy(){
			if(Image!=null){
				Image.GoingOffDisplay();
			}
			base.OnBatchDestroy();
		}
		
		public override void Paint(){
			// Any meshes in this elements queue should now change colour:
			Color colour=Element.Style.Computed.ColorOverlay;
			MeshBlock block=FirstBlock;
			while(block!=null){
				block.SetColour(colour);
				block.Paint();
				block=block.LocalBlockAfter;
			}
		}
		
		/// <summary>Applies the given image to the background.</summary>
		/// <param name="image">The image to use.</param>
		public void SetImage(Texture2D image){
			Image=new ImagePackage(image);
			ImageReady(Image);
			ComputedStyle computed=Element.Style.Computed;
			
			if(!computed.FixedWidth){
				// Set the width:
				computed.ChangeTagProperty("width",new Css.Value(Image.Width()+"px",Css.ValueType.Pixels));
			}
			
			if(!computed.FixedHeight){
				// And the height too:
				computed.ChangeTagProperty("height",new Css.Value(Image.Height()+"px",Css.ValueType.Pixels));
			}
			
			Element.Document.Renderer.RequestLayout();
		}
		
		/// <summary>Manually apply an image package to the background of this element.</summary>
		public void SetImage(ImagePackage package){
			// Apply it:
			Image=package;
			
			// Let this object know the package is ready:
			ImageReady(package);
		}
		
		/// <summary>A callback used when the graphic has been loaded and is ready for display.</summary>
		public void ImageReady(ImagePackage package){
			if(!Image.Loaded()){
				return;
			}
			
			Element.OnLoaded("background-image");
			
			RequestLayout();
			
			int imageWidth=Image.Width();
			int imageHeight=Image.Height();
			// Make sure offsets aren't bigger than the dimensions of a single image:
			
			if(RepeatX && OffsetX!=null){
				if(imageWidth==0){
					OffsetX=null;
				}else{
					OffsetX.PX=OffsetX.PX%imageWidth;
				}
			}
			
			if(RepeatY && OffsetY!=null){
				if(imageHeight==0){
					OffsetY=null;
				}else{
					OffsetY.PX=OffsetY.PX%imageHeight;
				}
			}
			
			if(Image!=null && Filtering!=FilterMode.Point && Image.Image!=null){
				Image.Image.filterMode=Filtering;
			}
		}
		
		protected override void NowOffScreen(){
			
			if(ImageLocation==null){
				return;
			}
			
			if(ImageLocation.DecreaseUsage()){
				ImageLocation=null;
			}
			
		}
		
		protected override bool NowOnScreen(){
			
			if(Image==null || Image.Image==null){
				// Reject the visibility state change.
				ImageLocation=null;
				return false;
			}
			
			ImageLocation=RequireImage(Image);
			
			return true;
			
		}
		
		protected override void Layout(){
			if(Image==null || !Image.Loaded()){
				return;
			}
			
			if(Clipping==BackgroundClipping.Text){
				return;
			}
			
			Renderman renderer=Element.Document.Renderer;
			
			if(Image.Animated || Image.IsDynamic || renderer.RenderMode==RenderMode.NoAtlas || Filtering!=FilterMode.Point || ForcedIsolate){
				// SPA is an animation format, so we need a custom texture atlas to deal with it.
				// This is because the frames of any animation would quickly exhaust our global texture atlas.
				// So to get a custom atlas, we must isolate this property.
				Isolate();
			}else if(Image.IsVideo){
				// Similarly with a video, we need to isolate it aswell.
				Isolate();
				
				#if !MOBILE
				if(!Image.Video.isPlaying && Element["autoplay"]!=null){
					// Play now:
					Image.Video.Play();
					
					// Fire an onplay event:
					Element.Run("onplay");
					
					// Clear:
					Element["autoplay"]=null;
				}
				
				#endif
			}else{
				// Reverse isolation, if we are isolated already:
				Include();
			}
			
			ComputedStyle computed=Element.Style.Computed;
			
			// Get the full shape of the element:
			int width=computed.PaddedWidth;
			int height=computed.PaddedHeight;
			int minY=computed.OffsetTop+computed.BorderTop;
			int minX=computed.OffsetLeft+computed.BorderLeft;
			
			if(width==0||height==0){
				if(Visible){
					SetVisibility(false);
				}
				return;
			}
			
			BoxRegion boundary=new BoxRegion(minX,minY,width,height);
			
			if(!boundary.Overlaps(renderer.ClippingBoundary)){
				
				if(Visible){
					SetVisibility(false);
				}
				
				return;
			}else if(!Visible){
				
				// ImageLocation will allocate here if it's needed.
				SetVisibility(true);
				
			}
			
			boundary.ClipBy(renderer.ClippingBoundary);
			
			// Texture time - get it's location on that atlas:
			AtlasLocation locatedAt=ImageLocation;
			
			if(locatedAt==null){
				// We're not using the atlas here.
				
				if(!Isolated){
					Isolate();
				}
				
				int imgWidth=Image.Width();
				int imgHeight=Image.Height();
				locatedAt=new AtlasLocation(0,0,imgWidth,imgHeight,imgWidth,imgHeight);
			}
			
			// Isolation is all done - safe to setup the batch now:
			SetupBatch(locatedAt.Atlas,null);
			
			// Great - Use locatedAt.Width/locatedAt.Height - this removes any risk of overflowing into some other image.
			
			int imageCountX=1;
			int imageCountY=1;
			int trueImageWidth=locatedAt.Width;
			int trueImageHeight=locatedAt.Height;
			int imageWidth=trueImageWidth;
			int imageHeight=trueImageHeight;
			bool autoX=false;
			bool autoY=false;
			
			if(Image.PixelPerfect){
				imageWidth=(int)(imageWidth*ScreenInfo.ResolutionScale);
				imageHeight=(int)(imageWidth*ScreenInfo.ResolutionScale);
			}
			
			if(SizeX!=null){
				if(SizeX.Single!=0f){
					imageWidth=(int)(width*SizeX.Single);
				}else if(SizeX.PX!=0){
					imageWidth=SizeX.PX;
				}else if(SizeX.IsAuto()){
					autoX=true;
				}
			}
			
			if(SizeY!=null){
				if(SizeY.Single!=0f){
					imageHeight=(int)(height*SizeY.Single);
				}else if(SizeY.PX!=0){
					imageHeight=SizeY.PX;
				}else if(SizeY.IsAuto()){
					autoY=true;
				}
			}
			
			if(autoX){
				
				imageWidth=imageHeight * trueImageWidth / trueImageHeight;
				
			}else if(autoY){
				
				imageHeight=imageWidth * trueImageHeight / trueImageWidth;
				
			}
			
			// offsetX and offsetY are the images position offset from where it should be (e.g. x of -200 means it's 200px left)
			
			// Resolve the true offset values:
			int offsetX=0;
			int offsetY=0;
			
			if(OffsetX!=null){
				
				// Resolve a potential mixed % and px:
				offsetX=OffsetX.GetMixed(width-imageWidth);
				
			}
			
			if(OffsetY!=null){
				
				// Resolve a potential mixed % and px:
				offsetY=OffsetY.GetMixed(height-imageHeight);
				
			}
			
			if(RepeatX){
				// Get the rounded up number of images:
				imageCountX=(width-1)/imageWidth+1;
				
				if(offsetX!=0){
					// If we have an offset, another image is introduced.
					imageCountX++;
				}
			}
			
			if(RepeatY){
				// Get the rounded up number of images:
				imageCountY=(height-1)/imageHeight+1;
				if(offsetY!=0){
					// If we have an offset, another image is introduced.
					imageCountY++;
				}
			}
			
			int blockX=minX+offsetX;
			int blockY=minY+offsetY;
			
			if(RepeatX&&offsetX>0){
				// We're repeating and the image is offset by a +ve number.
				// This means a small gap, OffsetX px wide, is open on this left side.
				// So to fill it, we need to offset this first image by a much bigger number - the value imageWidth-OffsetX.
				blockX-=(imageWidth-offsetX);
				// This results in the first image having OffsetX pixels exposed in the box - this is what we want.
			}
			
			if(RepeatY&&offsetY>0){
				// Similar thing to above:
				blockY-=(imageHeight-offsetY);
			}
			
			BoxRegion screenRegion=new BoxRegion();

			bool first=true;
			int startX=blockX;
			Color colour=computed.ColorOverlay;
			float zIndex=(computed.ZIndex-0.003f);
			
			for(int y=0;y<imageCountY;y++){
				for(int x=0;x<imageCountX;x++){
					// Draw at blockX/blockY.
					screenRegion.Set(blockX,blockY,imageWidth,imageHeight);
					
					if(screenRegion.Overlaps(boundary)){
						// If the two overlap, this means it's actually visible.
						MeshBlock block=Add();
						
						if(Image.Animated&&first){
							first=false;
							// Make sure we have an instance:
							Image.GoingOnDisplay();
							block.ParentMesh.SetMaterial(Image.Animation.AnimatedMaterial);
						}else if(Image.IsVideo&&first){
							first=false;
							block.ParentMesh.SetMaterial(Image.VideoMaterial);
						}else if(Isolated&&first){
							first=false;
							block.ParentMesh.SetMaterial(Image.ImageMaterial);
						}
						
						// Set it's colour:
						block.SetColour(colour);
						
						// And clip our meshblock to fit within boundary:
						block.TextUV=null;
						block.ImageUV=block.SetClipped(boundary,screenRegion,renderer,zIndex,locatedAt,block.ImageUV);
					}
					
					blockX+=imageWidth;
				}
				blockX=startX;
				blockY+=imageHeight;
			}
			
		}
		
	}
	
}