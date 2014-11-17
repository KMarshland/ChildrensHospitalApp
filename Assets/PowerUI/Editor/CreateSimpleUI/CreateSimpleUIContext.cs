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

using UnityEditor;
using UnityEngine;
using System.IO;
using Wrench;


namespace PowerUI{
	
	/// <summary>
	/// Useful context option for creating a new HTML file.
	/// </summary>
	
	public class CreateSimpleUIContext:MonoBehaviour{
	
		/// <summary>Creates a new HTML file.</summary>
		[MenuItem("GameObject/Create Other/Simple Main UI")]
		[MenuItem("Assets/PowerUI/Create Simple Main UI")]
		public static void CreateSimpleUI(){
			
			// Already got one?
			if(GameObject.Find("main-ui")!=null){
				Debug.LogError("Only one main UI instance is required per scene.");
				return;
			}
			
			// Create a gameobject:
			GameObject mainUI=new GameObject();
			
			// Give it a name:
			mainUI.name="main-ui";
			
			// Attach a simple UI manager to it:
			mainUI.AddComponent<PoweruiManager>();
			
		}
		
	}
	
}