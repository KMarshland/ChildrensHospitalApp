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
using System.Collections;
using System.Collections.Generic;
using Wrench;
using Nitro;

namespace PowerUI{
	
	/// <summary>
	/// Manages all current image formats.
	/// </summary>
	
	public static class ImageFormats{
		
		/// <summary>The "picture" format (jpeg, png etc) is the default handler.</summary>
		public static string UnrecognisedImageHandler="pict";
		/// <summary>The set of available formats. Use get to access.</summary>
		public static Dictionary<string,ImageFormat> Formats;
		
		
		/// <summary>Sets up all available image formats.
		/// Called internally by UI.Setup.</summary>
		public static void Setup(){
			if(Formats!=null){
				return;
			}
			
			Formats=new Dictionary<string,ImageFormat>();
			
			#if NETFX_CORE
			
			// For each type..
			foreach(TypeInfo type in Assemblies.Current.DefinedTypes){
				
				// Is it a ImageFormat?
				if( type.IsSubclassOf(typeof(ImageFormat)) ){
					// Yes it is - add it.
					Add((ImageFormat)Activator.CreateInstance(type.AsType()));
				}
				
			}
			
			#else
			
			// Get all types:
			Type[] allTypes=Assemblies.Current.GetTypes();
			
			// For each type..
			
			for(int i=allTypes.Length-1;i>=0;i--){
				Type type=allTypes[i];
				
				// Is it a ImageFormat?
				if( TypeData.IsSubclassOf(type,typeof(ImageFormat)) ){
					// Yes it is - add it.
					Add((ImageFormat)Activator.CreateInstance(type));
				}
			}
			
			#endif
			
		}
		
		/// <summary>Adds the given image format to the global set for use.
		/// Note that you do not need to call this manually; Just deriving from
		/// ImageFormat is all that is required.</summary>
		/// <param name="format">The new format to add.</param>
		public static void Add(ImageFormat format){
			string[] nameSet=format.GetNames();
			
			if(nameSet==null){
				return;
			}
			
			foreach(string name in nameSet){
				Formats[name.ToLower()]=format;
			}
		}
		
		/// <summary>Gets a format by the given file type.</summary>
		/// <param name="type">The name of the format, e.g. "png".</param>
		/// <returns>An ImageFormat if found; null otherwise.</returns>
		public static ImageFormat Get(string type){
			if(type==null){
				type="";
			}
			
			ImageFormat result=null;
			if(!Formats.TryGetValue(type.ToLower(),out result)){
				
				// Get the unrecognised handler:
				Formats.TryGetValue(UnrecognisedImageHandler,out result);
				
			}
			
			return result;
		}
		
	}
	
}