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
	
	public partial class ElementStyle:Style{
	
		public int fastLeft{
			get{
				
				return -1;
				
			}
			set{
				
				Renderman renderer=Element.document.Renderer;
				
				if(renderer.RenderingInWorld){
					
					translateX=((float)value+renderer.InWorldUI.WorldScreenOrigin.x).ToString();
					
				}else{
					
					translateX=( value*ScreenInfo.WorldPerPixel.x ).ToString();
					
				}
			}
		}
	
		public int fastTop{
			get{
				
				return -1;
				
			}
			set{
				
				Renderman renderer=Element.document.Renderer;
				
				if(renderer.RenderingInWorld){
					
					translateY=((float)value+renderer.InWorldUI.WorldScreenOrigin.y).ToString();
					
				}else{
					
					float topValue=-(float)value;
					
					translateY=( (topValue*ScreenInfo.WorldPerPixel.y) ).ToString();
					
				}
				
			}
		}
	
	}
	
}