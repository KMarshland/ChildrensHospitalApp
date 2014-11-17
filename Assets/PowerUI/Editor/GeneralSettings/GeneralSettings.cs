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
	/// Displays some general settings for PowerUI.
	/// </summary>

	public class GeneralSettings : EditorWindow{
		
		/// <summary>A timer which causes Update to run once every 2 seconds.</summary>
		public static int UpdateTimer;
		/// <summary>True if this project manages UI.HandleInput for itself.</summary>
		public static bool CustomInput;
		/// <summary>True if all PowerUI classes should be 'isolated' inside the PowerUI namespace.
		/// The only class actually outside by default is the 'UI' class, for convenience.</summary>
		public static bool IsolatePowerUI;
		
		
		// Add menu item named "General Settings" to the PowerUI menu:
		[MenuItem("Window/PowerUI/General Settings")]
		public static void ShowWindow(){
			// Load isolation value:
			IsolatePowerUI=Symbols.IsSymbolDefined("IsolatePowerUI");
			
			// Load custom input value:
			CustomInput=Symbols.IsSymbolDefined("NoPowerUIInput");
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(GeneralSettings));
			
			// Give it a title:
			window.title="General Settings";
		}
		
		// Called at 100fps.
		void Update(){
			UpdateTimer++;
			
			if(UpdateTimer<200){
				return;
			}
			
			UpdateTimer=0;
			// Reduced now to once every 2 seconds.
			
			// Load isolation value, just incase it changed in the actual defines window:
			IsolatePowerUI=Symbols.IsSymbolDefined("IsolatePowerUI");
			
			// Load custom input value, just incase it changed in the actual defines window:
			CustomInput=Symbols.IsSymbolDefined("NoPowerUIInput");
		}
		
		void OnGUI(){
			bool previousValue=IsolatePowerUI;
			IsolatePowerUI=EditorGUILayout.Toggle("Isolate UI classes",previousValue);
			EditorGUILayout.HelpBox("This isolates the 'UI' class inside the PowerUI namespace just incase you've got a class called UI of your own.",MessageType.Info);
			
			if(previousValue!=IsolatePowerUI){
				OnIsolateChanged();
			}
			
			previousValue=CustomInput;
			CustomInput=EditorGUILayout.Toggle("Custom Input",previousValue);
			EditorGUILayout.HelpBox("Check if you'd like to call UI.HandleInput directly so you can use it's return value.",MessageType.Info);
			
			if(previousValue!=CustomInput){
				OnCustomInputChanged();
			}
			
		}
		
		/// <summary>Called when the isolate checkbox is changed.</summary>
		private void OnIsolateChanged(){
			
			if(IsolatePowerUI){
				Symbols.DefineSymbol("IsolatePowerUI");
			}else{
				Symbols.UndefineSymbol("IsolatePowerUI");
			}
		}
		
		/// <summary>Called when the custom input checkbox is changed.</summary>
		private void OnCustomInputChanged(){
			
			if(CustomInput){
				Symbols.DefineSymbol("NoPowerUIInput");
			}else{
				Symbols.UndefineSymbol("NoPowerUIInput");
			}
		}
		
	}

}