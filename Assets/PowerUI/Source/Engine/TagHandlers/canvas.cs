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
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{
	
	/// <summary>
	/// Represents a canvas which lets you draw 2D shapes and polygons on the UI.
	/// </summary>
	
	public class CanvasTag:HtmlTagHandler{
		
		/// <summary>The 2D canvas context. Use getContext("2D") or context to obtain.</summary>
		private CanvasContext2D Context2D;
		
		
		public override string[] GetTags(){
			return new string[]{"canvas"};
		}
		
		public override Wrench.TagHandler GetInstance(){
			return new CanvasTag();
		}
		
		/// <summary>Gets a rendering context for this canvas.</summary>
		public CanvasContext getContext(string contextName){
			// Lowercase it:
			contextName=contextName.ToLower();
			
			// Is it the 2D context?
			if(contextName=="2d"){
				return context2D;
			}
			
			return null;
		}
		
		/// <summary>The 2D canvas context.</summary>
		public CanvasContext2D context2D{
			get{
				if(Context2D==null){
					Context2D=new CanvasContext2D(this);
					
					// Set it up now:
					Context2D.ApplyImageData();
				}
				
				return Context2D;
			}
		}
		
		public override void WidthChanged(){
			if(Context2D!=null){
				Context2D.Resized();
			}
		}
		
		public override void HeightChanged(){
			if(Context2D!=null){
				Context2D.Resized();
			}
		}
		
	}
	
	
	public partial class Element{
		
		/// <summary>Gets a rendering context for this canvas (if it is a canvas element!).</summary>
		/// <param name="text">The context type e.g. "2D".</param>
		public CanvasContext getContext(string text){
			if(Tag=="canvas"){
				return ((CanvasTag)Handler).getContext(text);
			}
			
			return null;
		}
		
	}
	
}