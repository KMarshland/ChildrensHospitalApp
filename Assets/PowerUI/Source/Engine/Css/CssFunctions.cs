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
using System.Reflection;
using System.Collections.Generic;
using Nitro;
using Wrench;


namespace PowerUI.Css{
	
	/// <summary>
	/// A global lookup of function name to function. E.g. rgba() is a CSS function.
	/// Css functions are instanced globally and mapped to the names they use.
	/// Note that functions are not instanced per element.
	/// </summary>
	
	public static class CssFunctions{
		
		/// <summary>The lookup itself. Matches name (e.g. "rgba") to the function that will process it.</summary>
		public static Dictionary<string,CssFunction> Functions;
		
		
		#if NETFX_CORE
		
		/// <summary>Sets up the global lookup by searching for any classes which inherit from CssFunction.</summary>
		public static void Setup(){
			if(Functions!=null){
				return;
			}
			
			// Create the set:
			Functions=new Dictionary<string,CssFunction>();
			
			// For each type..
			foreach(TypeInfo type in Assemblies.Current.DefinedTypes){
				
				if(type.IsGenericType){
					continue;
				}
				
				// Is it a CSS function?
				if(type.IsSubclassOf(typeof(CssFunction))){
					// Yes - add it:
					Add((CssFunction)Activator.CreateInstance(type.AsType()));
				}
				
			}
			
		}
		
		#else
		
		/// <summary>Sets up the global lookup by searching for any classes which inherit from CssFunction.</summary>
		public static void Setup(Type[] allTypes){
			if(Functions!=null){
				return;
			}
			
			// Create the set:
			Functions=new Dictionary<string,CssFunction>();
			
			// For each type..
			for(int i=allTypes.Length-1;i>=0;i--){
				// Grab it:
				Type type=allTypes[i];
				
				if(type.IsGenericType){
					continue;
				}
				
				// Is it a CSS function?
				if(TypeData.IsSubclassOf(type,typeof(CssFunction))){
					// Yes - add it:
					Add((CssFunction)Activator.CreateInstance(type));
				}
				
			}
			
		}
		
		#endif
		
		/// <summary>Adds a CSS function to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="cssFunction">The function to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(CssFunction cssFunction){
			
			string[] names=cssFunction.GetNames();
			
			if(names==null||names.Length==0){
				return false;
			}
			
			for(int i=0;i<names.Length;i++){
				
				// Grab the name:
				string name=names[i].ToLower();
				
				// Is it the first? If so, set the name:
				if(i==0){
					cssFunction.Name=name;
				}
				
				// Add it to functions:
				Functions[name]=cssFunction;
				
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the named function, returning the global function if it's found.</summary>
		/// <param name="name">The function to look for.</param>
		/// <returns>The global CssFunction if the function was found; Null otherwise.</returns>
		public static CssFunction Get(string name){
			CssFunction globalFunction=null;
			Functions.TryGetValue(name,out globalFunction);
			return globalFunction;
		}

	}
	
}