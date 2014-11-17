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

namespace PowerUI.Css{
	
	/// <summary>
	/// The .style property of a html element.
	/// </summary>
	
	public class ElementStyle:Style{
		
		/// <summary>True if this element is being repainted on the next frame.</summary>
		public bool IsPainting;
		/// <summary>Used for paint events. The next element style to paint.</summary>
		public ElementStyle Next;
		/// <summary>The computed style of this element.</summary>
		public ComputedStyle Computed;	
		
		
		/// <summary>Creates a new element style for the given element.</summary>
		/// <param name="element">The element that this will be the style for.</param>
		public ElementStyle(Element element):base(element){
			Computed=new ComputedStyle(element);
		}
		
		public override void OnChanged(CssProperty property,Value newValue){
			// Update the computed object:
			Computed.ChangeProperty(property,newValue);
		}
		
		public override ComputedStyle GetComputed(){
			return Computed;
		}
		
		/// <summary>Sets the top left border radius.</summary>
		public string borderTopLeftRadius{
			set{
				Set("border-top-radius",value);
			}
			get{
				return GetString("border-top-radius");
			}
		}
		
		/// <summary>Sets the top right border radius.</summary>
		public string borderTopRightRadius{
			set{
				Set("border-right-radius",value);
			}
			get{
				return GetString("border-right-radius");
			}
		}
		
		/// <summary>Sets the bottom right border radius.</summary>
		public string borderBottomRightRadius{
			set{
				Set("border-bottom-radius",value);
			}
			get{
				return GetString("border-bottom-radius");
			}
		}
		
		/// <summary>Sets the bottom left border radius.</summary>
		public string borderBottomLeftRadius{
			set{
				Set("border-left-radius",value);
			}
			get{
				return GetString("border-left-radius");
			}
		}
		
		/// <summary>Sets the border radius of all corners.</summary>
		public string borderRadius{
			set{
				Set("border-radius",value);
			}
			get{
				return GetString("border-radius");
			}
		}
		
		/// <summary>Sets the text of this element. Internal use only.</summary>
		public string innerText{
			set{
				Set("inner-text",value);
			}
			get{
				return GetString("inner-text");
			}
		}
		
		/// <summary>Sets the text content of this element.</summary>
		public string content{
			set{
				Set("inner-text",value);
			}
			get{
				return GetString("inner-text");
			}
		}
		
		/// <summary>Sets the line height for this element, e.g. "200%".</summary>
		public string lineHeight{
			set{
				Set("line-height",value);
			}
			get{
				return GetString("line-height");
			}
		}
		
		/// <summary>Sets the gap between letters for custom kerning. You may also use a percentage, 
		/// e.g. 100% is no spacing and 200% is a gap that is the size of the font size between letters.</summary>
		public string letterSpacing{
			set{
				Set("letter-spacing",value);
			}
			get{
				return GetString("letter-spacing");
			}
		}
		
		/// <summary>Sets the gap between words for custom kerning.</summary>
		public string wordSpacing{
			set{
				Set("word-spacing",value);
			}
			get{
				return GetString("word-spacing");
			}
		}
		
		/// <summary>Sets the depth of this element. The higher the value, the higher up the element.</summary>
		public string zIndex{
			set{
				Set("z-index",value);
			}
			get{
				return GetString("z-index");
			}
		}
		
		/// <summary>Defines how text should wrap onto new lines. Either "nowrap" or "normal".</summary>
		public string whiteSpace{
			set{
				Set("white-space",value);
			}
			get{
				return GetString("white-space");
			}
		}
		
		/// <summary>The minimum height for this element. Default is 0px.</summary>
		public string minHeight{
			set{
				Set("min-height",value);
			}
			get{
				return GetString("min-height");
			}
		}
		
		/// <summary>The minimum width for this element. Default is 0px.</summary>
		public string minWidth{
			set{
				Set("min-width",value);
			}
			get{
				return GetString("min-width");
			}
		}
		
		/// <summary>The maximum height for this element.</summary>
		public string maxHeight{
			set{
				Set("max-height",value);
			}
			get{
				return GetString("max-height");
			}
		}
		
		/// <summary>The maximum width for this element.</summary>
		public string maxWidth{
			set{
				Set("max-width",value);
			}
			get{
				return GetString("max-width");
			}
		}
		
		/// <summary>How scrolling text gets clipped. By default this is "fast" and makes text look like it is being squished. "fast" or "clip".</summary>
		public string textClip{
			set{
				Set("text-clip",value);
			}
			get{
				return GetString("text-clip");
			}
		}
		
		/// <summary>The margin around the outside of the element. E.g. "6px" (all sides) or "4px 5px 4px 5px" (top,right,bottom,left).</summary>
		public string margin{
			set{
				Set("margin",value);
			}
			get{
				return GetString("margin");
			}
		}
		
		/// <summary>The size of the left margin around the outside of the element. E.g. "5px".</summary>
		public string marginLeft{
			set{
				Set("margin-left",value);
			}
		}
		
		/// <summary>The size of the right margin around the outside of the element. E.g. "5px".</summary>
		public string marginRight{
			set{
				Set("margin-right",value);
			}
		}
		
		/// <summary>The size of the top margin around the outside of the element. E.g. "5px".</summary>
		public string marginTop{
			set{
				Set("margin-top",value);
			}
		}
		
		/// <summary>The size of the bottom margin around the outside of the element. E.g. "5px".</summary>
		public string marginBottom{
			set{
				Set("margin-bottom",value);
			}
		}
		
		/// <summary>The size of the padding inside the element. E.g. "20px" (all sides) or "4px 5px 4px 5px" (top,right,bottom,left).</summary>
		public string padding{
			set{
				Set("padding",value);
			}
			get{
				return GetString("padding");
			}
		}
		
		/// <summary>The size of the left padding inside the element. E.g. "5px".</summary>
		public string paddingLeft{
			set{
				Set("padding-left",value);
			}
		}
		
		/// <summary>The size of the right padding inside the element. E.g. "5px".</summary>
		public string paddingRight{
			set{
				Set("padding-right",value);
			}
		}
		
		/// <summary>The size of the top padding inside the element. E.g. "5px".</summary>
		public string paddingTop{
			set{
				Set("padding-top",value);
			}
		}
		
		/// <summary>The size of the bottom padding inside the element. E.g. "5px".</summary>
		public string paddingBottom{
			set{
				Set("padding-bottom",value);
			}
		}
		
		/// <summary>The style of the border around the element. Can only be "solid" for now.</summary>
		public string borderStyle{
			set{
				Set("border-style",value);
			}
			get{
				return GetString("border-style");
			}
		}
		
		/// <summary>The width of the border around the element. E.g. "2px" (all sides) or "4px 5px 4px 5px" (top,right,bottom,left).</summary>
		public string borderWidth{
			set{
				Set("border-width",value);
			}
			get{
				return GetString("border-width");
			}
		}
		
		/// <summary>The colour of the border around the element. E.g. "#ffffff". Also supports alpha (e.g. #ffffff77).</summary>
		public string borderColor{
			set{
				Set("border-color","#"+value);
			}
			get{
				return GetString("border-color");
			}
		}
		
		/// <summary>A shortcut for defining width, style and colour of all sides in one go. E.g. "2px solid #ffffff".</summary>
		public string border{
			set{
				Set("border",value);
			}
		}
		
		/// <summary>The width of the left border. E.g. "2px".</summary>
		public string borderLeft{
			set{
				Set("border-left",value);
			}
		}
		
		/// <summary>The width of the right border. E.g. "2px".</summary>
		public string borderRight{
			set{
				Set("border-right",value);
			}
		}
		
		/// <summary>The width of the top border. E.g. "2px".</summary>
		public string borderTop{
			set{
				Set("border-top",value);
			}
		}
		
		/// <summary>The width of the bottom border. E.g. "2px".</summary>
		public string borderBottom{
			set{
				Set("border-bottom",value);
			}
		}
		
		/// <summary>The style of the left border. E.g. "solid".</summary>
		public string borderLeftStyle{
			set{
				Set("border-left-style",value);
			}
		}
		
		/// <summary>The style of the right border. E.g. "solid".</summary>
		public string borderRightStyle{
			set{
				Set("border-right-style",value);
			}
		}
		
		/// <summary>The style of the top border. E.g. "solid".</summary>
		public string borderTopStyle{
			set{
				Set("border-top-style",value);
			}
		}
		
		/// <summary>The style of the bottom border. E.g. "solid".</summary>
		public string borderBottomStyle{
			set{
				Set("border-bottom-style",value);
			}
		}
		
		/// <summary>The color of the left border.</summary>
		public string borderLeftColor{
			set{
				Set("border-left-color",value);
			}
		}
		
		/// <summary>The color of the right border.</summary>
		public string borderRightColor{
			set{
				Set("border-right-color",value);
			}
		}
		
		/// <summary>The color of the top border.</summary>
		public string borderTopColor{
			set{
				Set("border-top-color",value);
			}
		}
		
		/// <summary>The color of the bottom border.</summary>
		public string borderBottomColor{
			set{
				Set("border-bottom-color",value);
			}
		}
		
		/// <summary>The text direction.</summary>
		public string direction{
			set{
				Set("direction",value);
			}
			get{
				return GetString("direction");
			}
		}
		
		/// <summary>How this element should sit around other elements. "inline", "inline-block", "block", "none" (not visible).</summary>
		public string display{
			set{
				Set("display",value);
			}
			get{
				return GetString("display");
			}
		}
		
		/// <summary>Is this element visible? If not, it still takes up space. See display to make it act like it's not there at all.</summary>
		public string visibility{
			set{
				Set("visibility",value);
			}
			get{
				return GetString("visibility");
			}
		}
		
		/// <summary>Can be used to apply a line to text. E.g. "underline", "line-through", "overline", "none".</summary>
		public string textDecoration{
			set{
				Set("text-decoration",value);
			}
			get{
				return GetString("text-decoration");
			}
		}
		
		/// <summary>This property has strict usage. It must refer to the name of a font in resources only; e.g. "PowerUI/Arial".
		/// Currently, there can only be one font in use on screen. There is certainly scope for adding more in the future.</summary>
		public string fontFamily{
			set{
				Set("font-family",value);
			}
			get{
				return GetString("font-family");
			}
		}
		
		/// <summary>Sets the styling of the font. "italic", "oblique", "none".</summary>
		public string fontStyle{
			set{
				Set("font-style",value);
			}
			get{
				return GetString("font-style");
			}
		}
		
		/// <summary>Sets the weight (thickness) of the font. "bold", "normal".</summary>
		public string fontWeight{
			set{
				Set("font-weight",value);
			}
			get{
				return GetString("font-weight");
			}
		}
		
		/// <summary>The minimum font size in px at which spreadover occurs. As all characters get written to a texture, large font
		/// may cause this texture to fill up. Spreadover is the font size at which PowerUI will start searching for e.g.
		/// a specific bold version of the font to 'spread' or offload some of the characters onto.</summary>
		public string fontSpreadover{
			set{
				Set("font-spreadover",value);
			}
			get{
				return GetString("font-spreadover");
			}
		}
		
		/// <summary>The size of the font. E.g. "1.5em", "10px".</summary>
		public string fontSize{
			set{
				Set("font-size",value);
			}
			get{
				return GetString("font-size");
			}
		}
		
		/// <summary>The red component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayR{
			set{
				Set("color-overlay-r",value);
			}
			get{
				return GetString("color-overlay",0);
			}
		}
		
		/// <summary>The green component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayG{
			set{
				Set("color-overlay-g",value);
			}
			get{
				return GetString("color-overlay",1);
			}
		}
		
		/// <summary>The blue component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayB{
			set{
				Set("color-overlay-b",value);
			}
			get{
				return GetString("color-overlay",2);
			}
		}
		
		/// <summary>The alpha component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayA{
			set{
				Set("color-overlay-a",value);
			}
			get{
				return GetString("color-overlay",3);
			}
		}
		
		/// <summary>A colour to apply over the top of this element. E.g. "#ff0000" will make it get a red tint.</summary>
		public string colorOverlay{
			set{
				Set("color-overlay",value);
			}
			get{
				return GetString("color-overlay");
			}
		}
		
		/// <summary>The opacity of the element as a value from 0->1. e.g. "0.5".</summary>
		public string opacity{
			set{
				Set("opacity",value);
			}
			get{
				return GetString("color-overlay",3);
			}
		}
		
		/// <summary>The font colour. E.g. "#ffffff".</summary>
		public string color{
			set{
				Set("color",value);
			}
			get{
				return GetString("color");
			}
		}
		
		/// <summary>The colour of a solid background. E.g. "#ffffff".</summary>
		public string backgroundColor{
			set{
				Set("background-color","#"+value);
			}
			get{
				return GetString("background-color");
			}
		}
		
		/// <summary>The location of a background image. E.g. "url(imgInResources.png)", "url(cache://cachedImage)".
		/// See <see cref="PowerUI.FileProtocol"/> for more information on the default protocols://.</summary>
		public string backgroundImage{
			set{
				Set("background-image",value);
			}
			get{
				return GetString("background-image");
			}
		}
		
		/// <summary>The offset of the background. E.g. "10px 5px" (x,y).</summary>
		public string backgroundPosition{
			set{
				Set("background-position",value);
			}
			get{
				return GetString("background-position");
			}
		}
		
		/// <summary>How the background should be repeated if at all. E.g. "repeat-x", "repeat-y", "none".</summary>
		public string backgroundRepeat{
			set{
				Set("background-repeat",value);
			}
			get{
				return GetString("background-repeat");
			}
		}
		
		/// <summary>How the background should be scaled if at all. E.g. "100% 100%" or "auto" (default).</summary>
		public string backgroundSize{
			set{
				Set("background-size",value);
			}
			get{
				return GetString("background-size");
			}
		}
		
		/// <summary>How the width of the background should be scaled if at all. E.g. "100%" or "auto" (default).</summary>
		public string backgroundSizeX{
			set{
				Set("background-size-x",value);
			}
			get{
				return GetString("background-size-x");
			}
		}
		
		/// <summary>How the height of the background should be scaled if at all. E.g. "100%" or "auto" (default).</summary>
		public string backgroundSizeY{
			set{
				Set("background-size-y",value);
			}
			get{
				return GetString("background-size-y");
			}
		}
		
		/// <summary>A shortcut for applying an image, solid colour and the repeat setting all at once.
		/// E.g. "url(myImage.png) repeat-x #000000".</summary>
		public string background{
			set{
				Set("background",value);
			}
		}
		
		/// <summary>Sets what should happen if the content of an element overflows its boundaries. "scroll scroll" (x,y).</summary>
		public string overflow{
			set{
				Set("overflow",value);
			}
			get{
				return GetString("overflow");
			}
		}
		
		/// <summary>What happens if the content of an element overflows the x boundary. "scroll", "hidden", "auto", "visible".</summary>
		public string overflowX{
			set{
				Set("overflow-x",value);
			}
			get{
				return GetString("overflow-x");
			}
		}
		
		/// <summary>What happens if the content of an element overflows the y boundary. "scroll", "hidden", "auto", "visible".</summary>
		public string overflowY{
			set{
				Set("overflow-y",value);
			}
			get{
				return GetString("overflow-y");
			}
		}
		
		/// <summary>How images should be filtered. "point", "bilinear","trilinear".</summary>
		public string filterMode{
			set{
				Set("filter-mode",value);
			}
			get{
				return GetString("filter-mode");
			}
		}
		
		/// <summary>Should an image be on the atlas? May be globally overridden with UI.RenderMode. "true" (default) or "false".</summary>
		public string onAtlas{
			set{
				Set("on-atlas",value);
			}
			get{
				return GetString("on-atlas");
			}
		}
		
		/// <summary>Sets the width of this element. E.g. "50%", "120px".</summary>
		public string width{
			set{
				Set("width",value);
			}
			get{
				return GetString("width");
			}
		}
	
		/// <summary>Sets the height of this element. E.g. "50%", "120px".</summary>
		public string height{
			set{
				Set("height",value);
			}
			get{
				return GetString("height");
			}
		}
		
		/// <summary>How far from the left this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string left{
			set{
				Set("left",value);
			}
			get{
				return GetString("left");
			}
		}
		
		/// <summary>How far from the right this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string right{
			set{
				Set("right",value);
			}
			get{
				return GetString("right");
			}
		}
		
		/// <summary>How far from the top this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string top{
			set{
				Set("top",value);
			}
			get{
				return GetString("top");
			}
		}
		
		/// <summary>How far from the bottom this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string bottom{
			set{
				Set("bottom",value);
			}
			get{
				return GetString("bottom");
			}
		}
		
		/// <summary>The vertical alignment of child elements. "top","middle","bottom".</summary>
		public string vAlign{
			set{
				Set("v-align",value);
			}
			get{
				return GetString("v-align");
			}
		}
		
		/// <summary>The vertical alignment of child elements. "top","middle","bottom".</summary>
		public string verticalAlign{
			set{
				Set("v-align",value);
			}
			get{
				return GetString("v-align");
			}
		}
		
		/// <summary>The horizontal alignment of text and other elements. "left", "right", "center", "justify".</summary>
		public string textAlign{
			set{
				Set("text-align",value);
			}
			get{
				return GetString("text-align");
			}
		}
		
		/// <summary>The position of this element. "fixed","relative","absolute".</summary>
		public string position{
			set{
				Set("position",value);
			}
			get{
				return GetString("position");
			}
		}
		
		/// <summary>The x component of the skew.</summary>
		public string skewX{
			set{
				Set("skew-x",value);
			}
			get{
				return GetString("skew",0);
			}
		}
		
		/// <summary>The y component of the skew.</summary>
		public string skewY{
			set{
				Set("skew-y",value);
			}
			get{
				return GetString("skew",1);
			}
		}
		
		/// <summary>The z component of the skew.</summary>
		public string skewZ{
			set{
				Set("skew-z",value);
			}
			get{
				return GetString("skew",2);
			}
		}
		
		/// <summary>Applies a skew to the element in 3D space.</summary>
		public string skew{
			set{
				Set("skew",value);
			}
			get{
				return GetString("skew");
			}
		}
		
		/// <summary>The x component of the rotation. E.g. "30deg".</summary>
		public string rotateX{
			set{
				Set("rotate-x",value);
			}
			get{
				return GetString("rotate",0);
			}
		}
		
		/// <summary>The y component of the rotation. E.g. "30deg".</summary>
		public string rotateY{
			set{
				Set("rotate-y",value);
			}
			get{
				return GetString("rotate",1);
			}
		}
		
		/// <summary>The z component of the rotation. E.g. "30deg".</summary>
		public string rotateZ{
			set{
				Set("rotate-z",value);
			}
			get{
				return GetString("rotate",2);
			}
		}
		
		/// <summary>Applies a rotation to the element in 3D space. E.g. "5deg 6deg 4deg".</summary>
		public string rotate{
			set{
				Set("rotate",value);
			}
			get{
				return GetString("rotate");
			}
		}
		
		/// <summary>The x component of a translation in world space. E.g. "1.4".</summary>
		public string translateX{
			set{
				Set("translate-x",value);
			}
			get{
				return GetString("translate",0);
			}
		}
		
		/// <summary>The y component of a translation in world space. E.g. "1.4".</summary>
		public string translateY{
			set{
				Set("translate-y",value);
			}
			get{
				return GetString("translate",1);
			}
		}
		
		/// <summary>The z component of a translation in world space. E.g. "1.4".</summary>
		public string translateZ{
			set{
				Set("translate-z",value);
			}
			get{
				return GetString("translate",2);
			}
		}
		
		/// <summary>Applies a translation to the element in 3D world space. E.g. "10 14 1.4".</summary>
		public string translate{
			set{
				Set("translate",value);
			}
			get{
				return GetString("translate");
			}
		}
		
		/// <summary>The x component of this elements scale. E.g. "110%".</summary>
		public string scaleX{
			set{
				Set("scale-x",value);
			}
			get{
				return GetString("scale",0);
			}
		}
		
		/// <summary>The y component of this elements scale. E.g. "110%".</summary>
		public string scaleY{
			set{
				Set("scale-y",value);
			}
			get{
				return GetString("scale",1);
			}
		}
		
		/// <summary>The z component of this elements scale. E.g. "110%".</summary>
		public string scaleZ{
			set{
				Set("scale-z",value);
			}
			get{
				return GetString("scale",2);
			}
		}
		
		/// <summary>The scale of this element. E.g. "150% 140%", "200%", "200% 200% 110%".</summary>
		public string scale{
			set{
				Set("scale",value);
			}
			get{
				return GetString("scale");
			}
		}
		
		/// <summary>The location of the transform origin in 2D screen space. E.g. "10px 10px", "50% 50%".</summary>
		public string transformOrigin{
			set{
				Set("transform-origin",value);
			}
			get{
				return GetString("transform-origin");
			}
		}
		
		/// <summary>The x component of the location of the transform origin in 2D screen space.</summary>
		public string transformOriginX{
			set{
				Set("transform-origin-x",value);
			}
			get{
				return GetString("transform-origin",0);
			}
		}
		
		/// <summary>The y component of the location of the transform origin in 2D screen space.</summary>
		public string transformOriginY{
			set{
				Set("transform-origin-y",value);
			}
			get{
				return GetString("transform-origin",1);
			}
		}
		
		/// <summary>How the origin is positioned. "relative" (to the top left corner of the element)
		/// or "fixed" (fixed location on the screen).</summary>
		public string transformOriginPosition{
			set{
				Set("transform-origin-position",value);
			}
			get{
				return GetString("transform-origin-position");
			}
		}
		
		public override string GetString(string property,int innerIndex){
			return Computed.GetString(property,innerIndex);
		}
		
		public override string GetString(string property){
			return Computed.GetString(property);
		}
		
	}

}