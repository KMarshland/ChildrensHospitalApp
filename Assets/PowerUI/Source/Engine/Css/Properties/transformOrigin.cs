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

namespace PowerUI.Css.Properties{
	
	/// <summary>
	/// Represents the transform-origin: css property.
	/// </summary>
	
	public class TransformOrigin:CssProperty{
		
		
		public TransformOrigin(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"transform-origin"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			// E.g. 50% 10% 100%, 0px 0px 0px.
			
			if(style.SetupTransform(value)){
				
				style.Transform.OriginOffset=value;
			
			}
			
			style.RequestTransform();
		}
		
	}
	
}



