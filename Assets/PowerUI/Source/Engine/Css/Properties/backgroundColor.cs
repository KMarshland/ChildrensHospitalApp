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
	/// Represents the background-color: css property.
	/// </summary>
	
	public class BackgroundColor:CssProperty{
		
		
		public BackgroundColor(){
			// This is a color property:
			Type=ValueType.Color;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"background-color"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.BGColour=null;
			}else{
				BackgroundColour colour=style.BGColour;
				
				if(colour==null){
					// Create one:
					style.BGColour=colour=new BackgroundColour(style.Element);
				}
				
				// Change the base colour:
				colour.BaseColour=value.ToColor();
				
				// Tell it a colour changed:
				colour.ColourChanged();
				
				// display:inline can't have a bg-colour:
				style.EnforceNoInline();
			}
			
			style.RequestLayout();
		}
		
	}
	
}



