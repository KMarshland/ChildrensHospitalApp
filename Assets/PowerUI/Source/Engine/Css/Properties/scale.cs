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
	/// Represents the scale: css property.
	/// </summary>
	
	public class Scale:CssProperty{
		
		
		public Scale(){
			
			// This is a rectangle property:
			Type=ValueType.Rectangle;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"scale"};
		}
		
		public override void SetDefault(Css.Value value,ValueType type){
			
			value.Set("100% 100% 100%",ValueType.Rectangle);
			
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(style.SetupTransform(value)){
					
				if(value==null){
					style.Transform.Scale=Vector3.one;
				}else{
					float x=1f;
					float y=1f;
					float z=1f;
					
					if(value[0]!=null){
						x=value[0].Single;
					}
					
					if(value[1]!=null){
						y=value[1].Single;
					}
					
					if(value[2]!=null){
						z=value[2].Single;
					}
					
					style.Transform.Scale=new Vector3(x,y,z);
				}
				
			}
			
			style.RequestTransform();
			
		}
		
	}
	
}



