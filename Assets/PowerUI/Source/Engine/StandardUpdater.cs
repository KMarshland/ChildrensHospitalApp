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
using PowerUI;


namespace PowerUI{
	
	/// <summary>
	/// PowerUI creates one of these automatically if it's needed.
	/// It causes the Update routine to occur.
	/// </summary>
	
	public class StandardUpdater:MonoBehaviour{
		
		public void Update(){
			UI.InternalUpdate();
		}
		
		public void OnGUI(){
			#if !NoPowerUIInput
				UI.InternalHandleInput();
			#endif
		}
		
		public void OnDisable(){
			// Called when a scene changes.
			UI.Destroy();
		}
		
	}
	
}