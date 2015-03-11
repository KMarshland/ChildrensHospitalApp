//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the filter-mode: css property. Specific to PowerUI - this defines the image filtering.
	/// </summary>
	
	public class FilteringMode:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"filter-mode"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			PowerUI.Css.BackgroundImage background=GetBackground(style);
			
			if(value==null){
				background.Filtering=FilterMode.Point;
			}else{
				
				switch(value.Text){
					case "bilinear":
						background.Filtering=FilterMode.Bilinear;
					break;
					case "trilinear":
						background.Filtering=FilterMode.Trilinear;
					break;
					default:
						background.Filtering=FilterMode.Point;
					break;
				}
				
			}
			
			if(background.Image!=null && background.Image.Image!=null){
				background.Image.Image.filterMode=background.Filtering;
			}
			
			// Request a layout:
			background.RequestLayout();
			
		}
		
	}
	
}



