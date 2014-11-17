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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the text-align-last: css property.
	/// </summary>
	
	public class TextAlignLast:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"text-align-last"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			if(value==null){
				// Assume auto:
				style.HorizontalAlignLast=HorizontalAlignType.Auto;
				
			}else{
				
				if(value.Text=="right"){
					style.HorizontalAlignLast=HorizontalAlignType.Right;
				}else if(value.Text=="justify"){
					style.HorizontalAlignLast=HorizontalAlignType.Justify;
				}else if(value.Text=="center"){
					style.HorizontalAlignLast=HorizontalAlignType.Center;
				}else if(value.Text=="left"){
					style.HorizontalAlignLast=HorizontalAlignType.Left;
				}else if(value.Text=="start"){
					style.HorizontalAlignLast=HorizontalAlignType.Start;
				}else if(value.Text=="end"){
					style.HorizontalAlignLast=HorizontalAlignType.End;
				}else{
					style.HorizontalAlignLast=HorizontalAlignType.Auto;
				}
				
			}
			
			style.RequestLayout();
		}
		
	}
	
}



