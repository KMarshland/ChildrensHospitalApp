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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Pico{
	
	/// <summary>
	/// Holds a set of source files.
	/// </summary>
	
	public class SourceFileSet{
		
		/// <summary>The part of the path we strip off when copying this file set elsewhere.</summary>
		public string BasePath;
		public List<string> Files=new List<string>();
		public List<string> Ignores=new List<string>();
		
		
		/// <summary>Creates a source file set with the given base path. This path is only used during CopyTo.</summary>
		public SourceFileSet(string basePath){
			BasePath=basePath;
		}
		
		/// <summary>Is the file/directory with the given name ignored?</summary>
		public bool IsIgnored(string name){
			return Ignores.Contains(name);
		}
		
		/// <summary>Ignores the given file/directory name. E.g. "Editor" will ignore all folders called editor.</summary>
		public void Ignore(string name){
			Ignores.Add(name);
		}
		
		/// <summary>Adds a set of files/directories to this set.</summary>
		public void Add(string[] paths){
			
			for(int i=0;i<paths.Length;i++){
				
				Add(paths[i]);
				
			}
			
		}
		
		/// <summary>Adds a file/directory to this set.</summary>
		public void Add(string basePath){
			
			// Make things a little easier:
			basePath=basePath.Replace("\\","/");
			
			string[] pieces=basePath.Split('/');
			
			string name=pieces[pieces.Length-1];
			
			if(IsIgnored(name)){
				// E.g. Editor folder - ignore it.
				return;
			}
			
			// Is it a directory?
			FileAttributes attribs=File.GetAttributes(basePath);
			
			if((attribs&FileAttributes.Directory)==FileAttributes.Directory){
				// It's a directory.
				
				// Add all files:
				Add(Directory.GetFiles(basePath));
				
				// Add all subdirs:
				Add(Directory.GetDirectories(basePath));
				
				return;
				
			}
			
			// Must end in .cs:
			if(basePath.EndsWith(".cs")){
				
				// Add as a file:
				Files.Add(basePath);
				
			}
			
		}
		
		/// <summary>Copies all files in this set to the given target location.
		/// Note that all paths in this set will first have BasePath remove from them.</sumamry>
		public bool CopyTo(string path){
			
			for(int i=0;i<Files.Count;i++){
				
				// Grab the file:
				string file=Files[i];
				
				if(!file.StartsWith(BasePath+"/")){
					
					Debug.LogError("Copy error! Halting due to safety risk. All source files must share a common path. Path error was with file: "+file);
					return false;
					
				}
				
				// Strip the section:
				string targetPath=path+"/"+file.Substring(BasePath.Length+1);
				
				// Get the directory:
				string dir=Path.GetDirectoryName(targetPath);
				
				// Create it (if it doesn't exist):
				Directory.CreateDirectory(dir);
				
				// Copy:
				File.Copy(file,targetPath,true);
				
			}
			
			return true;
			
		}
		
		/// <summary>Deletes all files in this set. *Should be used with caution of course!*
		/// Do note that this precompiler copies all source files first.</summary>
		public void Delete(){
			
			for(int i=0;i<Files.Count;i++){
				File.Delete(Files[i]);
			}
			
			Files.Clear();
			
		}
		
		/// <summary>Gets an array of files.</summary>
		public string[] ToArray(){
			return Files.ToArray();
		}
		
		/// <summary>Reads all files into a set of source code.</summary>
		public string[] ToSourceArray(){
			
			string[] array=Files.ToArray();
			
			for(int i=0;i<array.Length;i++){
				
				// Get the file path:
				string file=array[i];
				
				// Read it, and write the source into the array:
				array[i]="#line 0 \""+file+"\"\r\n"+File.ReadAllText(file);
				
			}
			
			return array;
			
			
		}
		
	}
	
}