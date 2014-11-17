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
	/// Represents the transform-origin-position: css property.
	/// </summary>
	
	public class TransformOriginPosition:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"transform-origin-position"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			// E.g. relative, fixed.
			
			if(style.SetupTransform(value)){
				
				if(value==null){
					
					style.Transform.OriginPosition=PositionType.Relative;
					
				}else{
				
					if(value.Text=="fixed"){
						
						style.Transform.OriginPosition=PositionType.Fixed;
						
					}else if(value.Text=="absolute"){
						
						style.Transform.OriginPosition=PositionType.Absolute;
						
					}else{
						
						style.Transform.OriginPosition=PositionType.Relative;
						
					}
					
				}
				
			}
			
			style.RequestTransform();
		}
		
	}
	
}



