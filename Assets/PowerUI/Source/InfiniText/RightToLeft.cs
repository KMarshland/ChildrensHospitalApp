//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace InfiniText{
	
	/// <summary>
	/// A "map" of right-to-left characters. They come in four major blocks. Originally was a map of exceptions from these blocks, but the exceptions are not necessary.
	/// </summary>
	
	public static class RightToLeft{
		
		/// <summary>The minimum rightwards character, block A.</summary>
		public const int MinimumA=0x5BE;
		/// <summary>The minimum rightwards character, block A.</summary>
		public const int MaximumA=0x85E;
		/// <summary>The minimum rightwards character, block B.</summary>
		public const int MinimumB=0xFB1D;
		/// <summary>The minimum rightwards character, block B.</summary>
		public const int MaximumB=0xFDFD;
		/// <summary>The minimum rightwards character, block C.</summary>
		public const int MinimumC=0xFE70;
		/// <summary>The minimum rightwards character, block C.</summary>
		public const int MaximumC=0xFEFC;
		/// <summary>The minimum rightwards character, block D.</summary>
		public const int MinimumD=0x10800;
		/// <summary>The minimum rightwards character, block D.</summary>
		public const int MaximumD=0x10C48;
		
		
		/// <summary>Is the given character rightwards?</summary>
		public static bool Rightwards(int character){
			if(character<MinimumA || character>MaximumD){
				// Out of range of all 3 blocks.
				return false;
			}
			
			if(character>MaximumA && character<MinimumB){
				// In the gap between blocks A and B.
				return false;
			}
			
			if(character>MaximumB && character<MinimumC){
				// In the gap between blocks B and C.
				return false;
			}
			
			if(character>MaximumC && character<MinimumD){
				// In the gap between blocks C and D.
				return false;
			}
			
			// Must be in the blocks otherwise.
			return true;
		}
		
	}
	
}