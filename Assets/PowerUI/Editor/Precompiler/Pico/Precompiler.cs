//--------------------------------------
//                Pico
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;


namespace Pico{

	/// <summary>
	/// Precompiles modules/ libraries so they don't get rebuilt by Unity every time any source file changes.
	/// This can be used by any of your modules too, so do make use of this side feature to really speed up your build times!
	/// </summary>

	public static class Precompiler{
		
		/// <summary>The path to where source and precompile info is stored. This folder sits alongside the Assets folder in your project.</summary>
		public static string Path="Precompiled";
		
		
		/// <summary>Re-compiles the given module, optionally with UNITY_EDITOR defined. Must have already been precompiled. You can find if it has been with GetModule.</summary>
		public static void Recompile(string moduleName,bool editorMode){
			
			// Get the settings:
			Module settings=GetModule(moduleName);
			
			if(settings==null){
				return;
			}
			
			// Get all the source the files:
			SourceFileSet src=settings.GetSourceFiles();
			
			// Build now:
			if(Build(editorMode,moduleName,src)){
				
				Debug.Log("Recompile of "+moduleName+" was a success!");
				
				// Inform the asset DB:
				AssetDatabase.Refresh();
				
			}
			
		}
		
		/// <summary>Gets the precompile settings for a given "module" - that's just a group of files compiled together.</summary>
		public static Module GetModule(string moduleName){
			
			// Create the settings:
			Module settings=new Module(moduleName);
			
			if(!settings.Exists){
				return null;
			}
			
			return settings;
			
		}
		
		/// <summary>Reverses the precompilation of a particular module. Restores the source files back into the Assets folder.</summary>
		public static void Reverse(string moduleName){
			
			// Get the settings:
			Module settings=GetModule(moduleName);
			
			if(settings==null){
				return;
			}
			
			// Get all the source the files:
			SourceFileSet src=settings.GetSourceFiles();
			
			// Copy them back to being in Assets again:
			if(!src.CopyTo("Assets")){
				return;
			}
			
			// Delete the compiled Dll:
			settings.DeleteDll();
			
			// Back up the precompiled module:
			settings.Backup();
			
			// Inform the asset DB:
			AssetDatabase.Refresh();
			
		}
		
		/// <summary>Creates a precompile module for the given source paths. It's then precompiled, optionally in editor mode.</summary>
		/// <param name="paths">Paths to folders containing source code.</param>
		/// <param name="moduleName">A name which represents this group of files. This can be used to reverse the precompilation, or recompile.</param>
		/// <param name="editor">True if it should be precompiled in editor mode (with UNITY_EDITOR).</param>
		public static void Precompile(List<string> paths,string moduleName,bool editor){
			
			// The target folder where we'll place all the source etc:
			string path=Path+"/"+moduleName;
			
			// Create the folder:
			if(!Directory.Exists(path)){
				Directory.CreateDirectory(path);
				Directory.CreateDirectory(path+"/Source");
			}
			
			// Create the source file set:
			SourceFileSet files=new SourceFileSet("Assets");
			
			// Ignore/include "Editor" folders based on editor setting:
			if(!editor){
				files.Ignore("Editor");
			}
			
			// Ignore SVN folders:
			files.Ignore(".svn");
			
			// Could ignore platform specific folders here too!
			
			// Collect all source files now:
			foreach(string sourcePath in paths){
				files.Add(sourcePath);
			}
			
			// Copy the files out of the Assets folder into one where the source will no longer get compiled by Unity.
			if(!files.CopyTo(path+"/Source")){
				return;
			}
			
			// Write some settings:
			File.WriteAllText(path+"/Settings.conf","Editor="+editor+";");
			
			if(Build(editor,moduleName,files)){
				
				// Delete all the source files from the unity asset folder(s):
				files.Delete();
				
				Debug.Log("Precompiled "+moduleName);
				
				// Inform the asset DB:
				AssetDatabase.Refresh();
				
			}
			
		}
		
		/// <summary>Builds the given set of files into the given module using standard compiler args.</summary>
		public static bool Build(bool editor,string moduleName,SourceFileSet files){
			
			string target=Path+"/"+moduleName+"/"+moduleName+".dll";
			
			// Get the defines:
			string defines=GetFlags(editor);
			
			// Get a provider:
			CSharpCodeProvider compiler=new CSharpCodeProvider();
			
			CompilerParameters parameters=new CompilerParameters();
			parameters.GenerateInMemory=false;
			parameters.GenerateExecutable=true;
			parameters.OutputAssembly=target;
			parameters.CompilerOptions=defines+" /target:library";
			
			// Reference Unity:
			AddUnityDllPaths(editor,parameters.ReferencedAssemblies);
			
			// Build now:
			CompilerResults results = compiler.CompileAssemblyFromSource(parameters,files.ToSourceArray());
			
			if(results.Errors.Count>0){
				// We had errors! May just be warnings though, so let's check:
				
				foreach(CompilerError ce in results.Errors){
					if(ce.IsWarning){
						Debug.LogWarning(ce.ToString());
					}else{
						Debug.LogError(ce.ToString());
					}
				}
				
				if(!File.Exists(target)){
					
					Debug.LogError("Error precompiling "+target+".");
					return false;
					
				}
				
			}
			
			// Copy the DLL:
			File.Copy(target,"Assets/"+moduleName+".dll",true);
			
			return true;
			
		}
		
		/// <summary>Gets the Unity provided set of compiler flags.</summary>
		public static string GetFlags(bool editor){
			
			string[] flags=EditorUserBuildSettings.activeScriptCompilationDefines;
			
			FlagSet set=new FlagSet(flags);
			
			set.EditorMode(editor);
			
			return set.ToString();
			
		}
		
		/// <summary>Adds the paths to the UnityEngine/Editor dlls to the given collection.</summary>
		public static void AddUnityDllPaths(bool editor,StringCollection references){
			
			// Time to go find the UnityEngine and UnityEditor dlls.
			// We can cheat a little here though - we know for a fact that both are already loaded and in use :)
			
			// Unity editor is at..
			Assembly unityEditor=typeof(Editor).Assembly;
			
			// Unity engine is at..
			Assembly unityEngine=typeof(GameObject).Assembly;
			
			if(editor){
				references.Add(unityEditor.Location);
			}
			
			references.Add(unityEngine.Location);
			
		}
		
		/// <summary>Deletes a folder and everything within it. Note that this is only used when deleting a backup.</summary>
		public static void DeleteDirectory(string path){
			
			string[] entries=Directory.GetFiles(path);
			
			foreach(string file in entries){
				File.Delete(file);
			}
			
			entries=Directory.GetDirectories(path);
			
			foreach(string dir in entries){
				DeleteDirectory(dir);
			}
			
			Directory.Delete(path);
			
		}
		
	}
	
}