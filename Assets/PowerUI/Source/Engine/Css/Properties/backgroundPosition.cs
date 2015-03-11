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
	/// Represents the background-position: css property.
	/// </summary>
	
	public class BackgroundPosition:CssProperty{
		
		
		public BackgroundPosition(){
			
			// This is a point property:
			Type=ValueType.Point;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"background-position"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			BackgroundImage image=GetBackground(style);
			
			if(value!=null && value.InnerValues!=null){
				bool x=true;
				bool centerX=true;
				Value offsetX=null;
				Value offsetY=null;
				
				// Get the inner value count:
				int count=value.InnerValues.Length;
				
				// Loops for e.g. right 10% top 5%
				for(int i=0;i<count;i++){
					
					// Grab the inner value:
					Value innerValue=value.InnerValues[i];
					
					// Is it text?
					if(innerValue.Type==ValueType.Text){
						
						// Yep!
						switch(innerValue.Text){
							
							case "top":
								// Ignore this:
								offsetY=null;
								break;
							case "right":
								
								// Set to 100%:
								innerValue.SetPercent(1f);
								offsetX=innerValue;
								
								break;
							case "bottom":
								
								// Set to 100%:
								innerValue.SetPercent(1f);
								offsetY=innerValue;
								
								break;
							case "left":
								// Ignore this:
								offsetX=null;
								break;
							case "center":
								
								// Set to 50%:
								innerValue.SetPercent(0.5f);
								
								if(centerX){
									centerX=false;
									offsetX=innerValue;
								}else{
									offsetY=innerValue;
								}
								
								break;
								
						}
						
					}else if(x){
						x=false;
						
						if(offsetX==null){
							offsetX=innerValue;
						}else{
							// E.g. right 5%
							offsetX.Offset(innerValue);
						}
						
					}else{
						
						if(offsetY==null){
							offsetY=innerValue;
						}else{
							offsetY.Offset(innerValue);
						}
						
					}
					
				}
				
				// If either is equiv of zero, ignore it.
				if(offsetX!=null && (offsetX.PX!=0 || offsetX.Single!=0f) ){
					image.OffsetX=offsetX;
				}else{
					image.OffsetX=null;
				}
				
				if(offsetY!=null && (offsetY.PX!=0 || offsetY.Single!=0f) ){
					image.OffsetY=offsetY;
				}else{
					image.OffsetY=null;
				}
				
			}else{
				image.OffsetX=null;
				image.OffsetY=null;
			}
			
			// Request a layout:
			image.RequestLayout();
			
		}
		
	}
	
}



