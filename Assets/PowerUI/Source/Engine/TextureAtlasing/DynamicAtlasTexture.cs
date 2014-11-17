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
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{

	/// <summary>
	/// Depreciated: Renamed to DynamicTexture.
	/// A dynamic texture writes its pixels directly to the atlas.
	/// This allows for some very dynamic graphics on the UI (for example, a curved health bar).
	/// </summary>
	
	[Obsolete("Dynamic Atlas Textures are obsolete. Use DynamicTexture instead (same API).")]
	public class DynamicAtlasTexture:DynamicTexture{
		
		public DynamicAtlasTexture(int width,int height,string name):base(width,height,name){}
		
	}
	
}