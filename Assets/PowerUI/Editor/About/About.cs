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
using System.Threading;
using System.IO;

namespace PowerUI{

	/// <summary>
	/// Displays some info about PowerUI.
	/// </summary>

	public class About : EditorWindow{
		
		/// <summary>The major version number of PowerUI.</summary>
		public const int Major=1;
		/// <summary>The minor version number of PowerUI.</summary>
		public const int Minor=9;
		/// <summary>The revision number of PowerUI.</summary>
		public const int Revision=122;
		
		
		/// <summary>The version of PowerUI.</summary>
		public static string Version{
			
			get{
				
				return Major+"."+Minor+"."+Revision;
				
			}
			
		}
		
		// Add menu item named "About" to the PowerUI menu:
		[MenuItem("Window/PowerUI/About")]
		public static void ShowWindow(){
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(About));
			
			// Give it a title:
			window.title="About PowerUI";
			
		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("PowerUI is a large UI framework which renders HTML and CSS.\r\n\r\nHelp: http://powerui.kulestar.com/\r\n\r\nVersion: "+Version);
			
		}
		
	}

}