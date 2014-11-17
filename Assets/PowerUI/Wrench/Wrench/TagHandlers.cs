//--------------------------------------
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections.Generic;
using Nitro;

namespace Wrench{
	
	/// <summary>
	/// A global lookup of tag text to handler.
	/// TagHandlers are instanced globally and mapped to the tags they accept.
	/// When a tag is found, it is then instanced. One instance of a tag is created per element.
	/// </summary>
	
	public static class TagHandlers{
		
		/// <summary>The lookup itself. Matches tag text (e.g. "div") to the handler that will process it.</summary>
		public static Dictionary<string,TagHandler> Handlers;
		
		/// <summary>Sets up the global lookup by searching for any classes which inherit from TagHandler.</summary>
		public static void Setup(){
			if(Handlers!=null){
				return;
			}
			
			Handlers=new Dictionary<string,TagHandler>();
			
			#if NETFX_CORE
			
			// For each type..
			foreach(TypeInfo type in Assemblies.Current.DefinedTypes){
				
				if(type.IsGenericType){
					continue;
				}
				
				if( type.IsSubclassOf(typeof(TagHandler)) ){
					AddHandler((TagHandler)Activator.CreateInstance(type.AsType()));
				}
				
			}
			
			#else
			
			Type[] allTypes=Assemblies.Current.GetTypes();
			
			// For each type..
			for(int i=allTypes.Length-1;i>=0;i--){
				Type type=allTypes[i];
				
				if(type.IsGenericType){
					continue;
				}
				
				if( TypeData.IsSubclassOf(type,typeof(TagHandler)) ){
					AddHandler((TagHandler)Activator.CreateInstance(type));
				}
				
			}
			
			#endif
			
		}
		
		/// <summary>Gets the default TagHandler.</summary>
		public static TagHandler Default(){
			return new TagHandler();
		}
		
		/// <summary>Adds a handler to the set.
		/// This is generally done automatically, but you can also add it manually if you wish.</summary>
		/// <param name="handler">The handler to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool AddHandler(TagHandler handler){
			
			// Grab the extension:
			string extension=handler.TagExtension;
			
			if(extension!=null){
				extension=extension.ToLower()+"-";
			}
			
			string[] tags=handler.GetTags();
			
			if(tags==null||tags.Length==0){
				return false;
			}
			
			for(int i=0;i<tags.Length;i++){
				// Grab the tag name:
				string tagName=tags[i].ToLower();
				
				if(extension!=null){
					// Drop it in at the start.
					tagName=extension+tagName;
				}
				
				TagHandler existingHandler;
				
				// Does it already exist?
				if(Handlers.TryGetValue(tagName,out existingHandler)){
					// Yes - compare priorities:
					if(handler.Priority<existingHandler.Priority){
						// We're trying to override with a lower priority.
						continue;
					}else{
						// override it!
						Handlers[tagName]=handler;
					}
					
				}else{
					// Directly add it in:
					Handlers.Add(tagName,handler);
				}
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the tag with the given name.
		/// If it's not found, a default tag which is known to exist can be returned instead.
		/// The handler for the found tag is then instanced and the instance is returned.
		/// For example, tag "h1" with a default of "span".</summary>
		/// <param name="tag">The tag to look for.</param>
		/// <param name="defaultTag">If the given tag is not found, this is used instead.</param>
		/// <returns>An instance of the tag handler for the tag. Throws an error if tag or defaultTag are not found.</returns>
		public static TagHandler GetHandler(string tag,string defaultTag){
			TagHandler globalHandler=GetHandler(tag);
			if(globalHandler==null){
				globalHandler=GetHandler(defaultTag);
			}
			
			return globalHandler.GetInstance();
		}
		
		/// <summary>Attempts to find the named tag, returning the global handler if it's found.</summary>
		/// <param name="tag">The tag to look for.</param>
		/// <returns>The global TagHandler if the tag was found; Null otherwise.</returns>
		public static TagHandler GetHandler(string tag){
			TagHandler handler=null;
			Handlers.TryGetValue(tag,out handler);
			return handler;
		}

	}
	
}