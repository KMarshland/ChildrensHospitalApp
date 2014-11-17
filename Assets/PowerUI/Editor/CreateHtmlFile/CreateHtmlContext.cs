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
	
	public class CreateHtmlContext:MonoBehaviour{
	
		/// <summary>Creates a new HTML file.</summary>
		[MenuItem("Assets/Create/HTML File")]
		[MenuItem("Assets/PowerUI/New HTML File")]
		public static void CreateHtmlFile(){
			
			// Grab the path:
			string path=AssetDatabase.GetAssetPath(Selection.activeObject);
			
			if(string.IsNullOrEmpty(path)){
				return;
			}
			
			if(!File.Exists(path+"/MyNewHtml.html")){
				// Write a blank file:
				File.WriteAllText(path+"/MyNewHtml.html","");
			}else{
				// Count until we hit one that doesn't exist.
				int count=1;
				
				while(File.Exists(path+"/MyNewHtml-"+count+".html")){
					count++;
				}
				
				// Write it out now:
				File.WriteAllText(path+"/MyNewHtml-"+count+".html","");
				
			}
			
			// Refresh the database:
			AssetDatabase.Refresh();
			
		}
		
	}
	
}