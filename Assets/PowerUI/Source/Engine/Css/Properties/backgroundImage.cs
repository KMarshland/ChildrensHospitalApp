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
	/// Represents the background-image: css property.
	/// </summary>
	
	public class BackgroundImageProperty:CssProperty{
		
		
		public BackgroundImageProperty(){
			
			// This is a text property:
			Type=ValueType.Text;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-image"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			PowerUI.Css.BackgroundImage image=GetBackground(style);
			
			if(value==null || value.Text=="" || value.Text=="none"){
				
				if(image.Image!=null){
					image.Image.GoingOffDisplay();
					image.Image=null;
				}
				
				// Reverse any isolation:
				image.Include();
			}else{
				image.Image=new ImagePackage(value.Text,image.Element.Document.basepath);
				image.Image.Get(image.ImageReady);
			}
			
			// Request a layout:
			image.RequestLayout();
			
		}
		
	}
	
}



