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
	/// Represents the skew: css property.
	/// </summary>
	
	public class Skew:CssProperty{
		
		public Skew(){
			
			// This is a point type property:
			Type=ValueType.Point;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"skew"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(style.SetupTransform(value)){
				
				if(value==null){
					
					style.Transform.Skew=Matrix4x4.identity;
					
				}else{
					
					// Get the radian values:
					float rotationX=value.GetRadians(0);
					float rotationY=value.GetRadians(1);
					
					// Get the base matrix:
					Matrix4x4 skewMatrix=Matrix4x4.identity;
					
					// Find the tan values:
					rotationX=Mathf.Tan(rotationX);
					rotationY=Mathf.Tan(rotationY);
					
					// Apply them to the matrix using the flattened accessor:
					skewMatrix[4]=rotationX;
					skewMatrix[1]=rotationY;
					
					// Set the skew:
					style.Transform.Skew=skewMatrix;
					
				}
				
			}
			
			style.RequestTransform();
		}
		
	}
	
}



