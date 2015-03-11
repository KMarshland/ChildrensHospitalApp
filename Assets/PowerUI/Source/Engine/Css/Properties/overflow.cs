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
	/// Represents the overflow: css property.
	/// </summary>
	
	public class Overflow:CssProperty{
		
		
		public Overflow(){
			
			// This is a point property:
			Type=ValueType.Point;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"overflow"};
		}
		
		public override void Apply(ComputedStyle style,Value value){
			
			if(value==null){
				style.OverflowX=OverflowType.Visible;
				style.OverflowY=OverflowType.Visible;
			}else{
				style.OverflowX=GetOverflowType(value.GetText(0));
				style.OverflowY=GetOverflowType(value.GetText(1));
			}
			
			style.ResetScrollbars();
			
			style.RequestLayout();
		}
		
		/// <summary>Parses the string overflow text, e.g. "auto", into an overflowType value.</summary>
		/// <param name="overflow">The overflow text as a string.</param>
		/// <returns>The overflow value that was represented by the text.</returns>
		private OverflowType GetOverflowType(string overflow){
			
			switch(overflow){
				case "hidden":
					return OverflowType.Hidden;
				case "auto":
					return OverflowType.Auto;
				case "scroll":
					return OverflowType.Scroll;
			}
			
			return OverflowType.Visible;
		}
		
	}
	
}



