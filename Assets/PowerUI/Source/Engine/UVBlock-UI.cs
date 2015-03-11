//--------------------------------------
//                Blaze
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using PowerUI;
using UnityEngine;


namespace Blaze{
	
	/// <summary>
	/// Represents a block of four UV coordinates. Commonly shared globally.
	/// </summary>
	
	public partial class UVBlock{
		
		/// <summary>Writes out this block of UV's to the given buffer.</summary>
		internal void Write(Vector2[] buffer,int index){
			
			// Top Left:
			buffer[index]=new Vector2(MinX,MaxY);
			
			// Top Right:
			buffer[index+1]=new Vector2(MaxX,MaxY);
			
			// Bottom Left:
			buffer[index+2]=new Vector2(MinX,MinY);
			
			// Bottom Right:
			buffer[index+3]=new Vector2(MaxX,MinY);
			
		}
		
	}
	
}