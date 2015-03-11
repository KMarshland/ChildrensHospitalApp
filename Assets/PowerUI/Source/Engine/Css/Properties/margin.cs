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


namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the margin: css property.
	/// </summary>
	
	public class Margin:CssProperty{
		
		
		public Margin(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"margin"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			style.AutoMarginX=style.AutoMarginY=false;
			
			if(value==null){
				style.MarginTop=style.MarginBottom=0;
				style.MarginLeft=style.MarginRight=0;
			}else{
			
				// Top:
				Value innerValue=value[0];
				
				if(innerValue!=null){
					
					if(innerValue.IsAuto()){
						style.AutoMarginY=true;
					}else{
						style.MarginTop=innerValue.PX;
					}
					
				}else{
					style.MarginTop=0;
				}
				
				// Right:
				innerValue=value[1];
				
				if(innerValue!=null){
					
					if(innerValue.IsAuto()){
						style.AutoMarginX=true;
					}else{
						style.MarginRight=innerValue.PX;
					}
					
				}else{
					style.MarginRight=0;
				}
				
				// Bottom:
				innerValue=value[2];
				
				if(innerValue!=null){
					
					if(!style.AutoMarginY || !innerValue.IsAuto() ){
						style.AutoMarginY=false;
						style.MarginBottom=innerValue.PX;
					}
					
				}else{
					style.MarginBottom=0;
				}
				
				// Left:
				innerValue=value[3];
				if(innerValue!=null){
					if(!style.AutoMarginX || !innerValue.IsAuto() ){
						style.AutoMarginX=false;
						style.MarginLeft=innerValue.PX;
					}
				}else{
					style.MarginLeft=0;
				}
				
			}
			
			style.SetSize();
			
			style.RequestLayout();
		}
		
	}
	
}



