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
	/// Handles an image tag. The src attribute is supported.
	/// </summary>
	
	public class ImgTag:HtmlTagHandler{
		
		/// <summary>The aspect ratio of this image (height/width).</summary>
		public float AspectRatio;
		/// <summary>The image being loaded for this tag.</summary>
		public ImagePackage Image;
		/// <summary>The base width of the image.</summary>
		public Css.Value TagWidth;
		/// <summary>The base height of the image.</summary>
		public Css.Value TagHeight;
		/// <summary>The inverse aspect ratio of this image (height/width).</summary>
		public float InverseAspectRatio;
		
		
		public override string[] GetTags(){
			return new string[]{"img"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new ImgTag();
		}
		
		public override bool SelfClosing(){
			return true;
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				string src=Element["src"];
				
				if(src==null){
					src="";
				}
				
				ComputedStyle computed=Element.Style.Computed;
				computed.ChangeTagProperty("background-image",new Css.Value(src,Css.ValueType.Text));
				return true;
			}
			return false;
		}
		
		public override void HeightChanged(){
			ComputedStyle computed=Element.Style.Computed;
			
			if(Image==null){
				return;
			}
			
			// Is width set?
			if(computed[Css.Properties.Width.GlobalProperty]!=TagWidth){
				// Yep - element defined width itself in some way.
				return;
			}
			
			// Grab the height:
			int height=computed.InnerHeight;
			
			// Figure out the width:
			int width=(int)(height*AspectRatio);
			
			// Apply the new width:
			computed.FixedWidth=true;
			computed.InnerWidth=width;
			computed.SetPixelWidth(false);
			
		}
		
		public override void WidthChanged(){
			ComputedStyle computed=Element.Style.Computed;
			
			if(Image==null){
				return;
			}
			
			// Is height set?
			if(computed[Css.Properties.Height.GlobalProperty]!=TagHeight){
				// Yep - element defined height itself in some way.
				return;
			}
			
			// Grab the width:
			int width=computed.InnerWidth;
			
			// Figure out the height:
			int height=(int)(width*InverseAspectRatio);
			
			// Apply the new height:
			computed.FixedHeight=true;
			computed.InnerHeight=height;
			computed.SetPixelHeight(false);
			
		}
		
		public override void OnLoaded(string type){
			ComputedStyle computed=Element.Style.Computed;
			
			if(computed.BGImage==null){
				return;
			}
			
			Image=computed.BGImage.Image;
			
			if(Image==null){
				return;
			}
			
			float width=(float)Image.Width();
			float height=(float)Image.Height();
			
			// Figure out the aspect ratios:
			AspectRatio=width/height;
			InverseAspectRatio=height/width;
			
			if(Image.PixelPerfect){
				width*=ScreenInfo.ResolutionScale;
				height*=ScreenInfo.ResolutionScale;
			}
			
			// Compute the tag width:
			TagWidth=new Css.Value((int)width+"fpx",Css.ValueType.Pixels);
			
			// Compute the tag height:
			TagHeight=new Css.Value((int)height+"fpx",Css.ValueType.Pixels);
			
			// Act like we're setting tag properties:
			// This is so that class/ID/Style can override any of them.
			computed.ChangeTagProperty("height",TagHeight);
			computed.ChangeTagProperty("width",TagWidth);
			computed.ChangeTagProperty("background-repeat",new Css.Value("no-repeat",Css.ValueType.Text));
			computed.ChangeTagProperty("background-size",new Css.Value("100% 100%",Css.ValueType.Point));
			
		}
		
	}
	
}