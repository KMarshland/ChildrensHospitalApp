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
	/// Helper editor class for dealing with compiler #symbols.
	/// </summary>

	public static class Symbols{
		
		/// <summary>Checks if the given symbol is present in the define symbols set.</summary>
		/// <param name="symbol">The symbol to look for.</param>
		public static bool IsSymbolDefined(string symbol){
			string defineSymbols=PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentBuildGroup);
			if(defineSymbols==symbol){
				return true;
			}
			if(defineSymbols.StartsWith(symbol+";") || defineSymbols.EndsWith(";"+symbol)){
				return true;
			}
			return defineSymbols.Contains(";"+symbol+";");
		}
		
		/// <summary>Defines the given symbol in the define symbols set.</summary>
		/// <param name="symbol">The symbol to define.</param>
		public static void DefineSymbol(string symbol){
			if(IsSymbolDefined(symbol)){
				return;
			}
			
			// Get the existing set of symbols:
			string defineSymbols=PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentBuildGroup);
			
			if(string.IsNullOrEmpty(defineSymbols)){
				defineSymbols=symbol;
			}else{
				defineSymbols+=";"+symbol;
			}
			
			// Write it back:
			PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentBuildGroup,defineSymbols);
			
		}
		
		/// <summary>Removes the given symbol from the define symbols set.</summary>
		/// <param name="symbol">The symbol to remove, if found.</param>
		public static void UndefineSymbol(string symbol){
			if(!IsSymbolDefined(symbol)){
				return;
			}
			
			// Get the existing set of symbols:
			string defineSymbols=PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentBuildGroup);
			string[] pieces=defineSymbols.Split(';');
			defineSymbols="";
			
			for(int i=0;i<pieces.Length;i++){
				if(pieces[i]==symbol){
					// This is the symbol we want to strip - skip it.
					continue;
				}
				// Add it to the new string we're making.
				if(defineSymbols!=""){
					defineSymbols+=";";
				}
				defineSymbols+=pieces[i];
			}
			
			// Write it back:
			PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentBuildGroup,defineSymbols);
		}
		
		/// <summary>A shortcut to the current build target.</summary>
		private static BuildTargetGroup CurrentBuildGroup{
			get{
				return EditorUserBuildSettings.selectedBuildTargetGroup;
			}
		}
		
	}

}