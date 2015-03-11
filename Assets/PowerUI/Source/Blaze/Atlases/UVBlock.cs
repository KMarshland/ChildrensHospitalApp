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


namespace Blaze{
	
	/// <summary>
	/// Represents a block of four UV coordinates. Commonly shared globally.
	/// </summary>
	
	public partial class UVBlock{
	
		/// <summary>The min UV x coordinate.</summary>
		internal float MinX;
		/// <summary>The min UV y coordinate.</summary>
		internal float MinY;
		/// <summary>The max UV x coordinate.</summary>
		internal float MaxX;
		/// <summary>The max UV y coordinate.</summary>
		internal float MaxY;
		
		
		internal UVBlock(){}
		
		internal UVBlock(UVBlock copy){
			
			MinX=copy.MinX;
			MinY=copy.MinY;
			MaxX=copy.MaxX;
			MaxY=copy.MaxY;
			
		}
		
		internal UVBlock(float minX,float maxX,float minY,float maxY){
			
			MinX=minX;
			MinY=minY;
			MaxX=maxX;
			MaxY=maxY;
			
		}
		
		/// <summary>True if this UV block is a globally shared one.</summary>
		public virtual bool Shared{
			get{
				return false;
			}
		}
		
	}
	
}