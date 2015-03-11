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
	/// Represents the z-index: css property.
	/// </summary>
	
	public class ZIndex:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"z-index"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.FixedDepth=false;
				style.ZIndex=0f;
			}else{
				style.FixedDepth=true;
				style.FixedZIndex=value.ToFloat() * style.Element.Document.Renderer.DepthResolution;
			}
			
			style.RequestLayout();
		}
		
	}
	
}



