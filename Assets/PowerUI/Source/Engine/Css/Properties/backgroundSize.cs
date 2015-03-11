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
	/// Represents the background-size: css property.
	/// </summary>
	
	public class BackgroundSize:CssProperty{
		
		
		public BackgroundSize(){
			
			// This is a point property:
			Type=ValueType.Point;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"background-size"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			PowerUI.Css.BackgroundImage image=GetBackground(style);
			
			if(value==null){
				image.SizeX=null;
				image.SizeY=null;
			}else if(value.Type==Css.ValueType.Text){
				
				if(value.Text=="auto" || value.Text=="initial"){
					// Same as the default:
					image.SizeX=null;
					image.SizeY=null;
					
				}else if(value.Text=="cover"){
					// Same as 100% on both axis:
					image.SizeX=new Css.Value("100%",Css.ValueType.Percentage);
					image.SizeY=new Css.Value("100%",Css.ValueType.Percentage);
					
				}
				
			}else{
				// It's a vector:
				image.SizeX=value[0];
				image.SizeY=value[1];
			}
			
			// Request a layout:
			image.RequestLayout();
			
		}
		
	}
	
}



