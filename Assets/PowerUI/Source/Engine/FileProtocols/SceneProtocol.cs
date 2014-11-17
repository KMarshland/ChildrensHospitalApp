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

using PowerUI.Css;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// This scene:// protocol enables a link to point to another scene.
	/// E.g. href="scene://sceneName" will load the scene called 'sceneName' when clicked.
	/// </summary>
	
	public class SceneProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"scene"};
		}
		
		public override void OnFollowLink(Element linkElement,FilePath path){
			Application.LoadLevel(path.Directory+path.File);
		}
		
	}
	
}