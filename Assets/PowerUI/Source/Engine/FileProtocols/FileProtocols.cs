//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
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
	/// Manages all current file protocols://.
	/// File protocols such as http, cache, dynamic, scene etc enable PowerUI to load files in
	/// custom ways - for example if your game uses a specialised cdn, you may easily implement
	/// it as a new FileProtocol.
	/// </summary>
	
	public static class FileProtocols{
		
		public static Dictionary<string,FileProtocol> Protocols;
		
		/// <summary>Sets up all available file protocols by scanning around
		/// for all classes which inherit from the FileProtocol type.
		/// Called internally by UI.Setup.</summary>
		public static void Setup(){
			if(Protocols!=null){
				return;
			}
			
			Protocols=new Dictionary<string,FileProtocol>();
			
			#if NETFX_CORE
			
			// For each type..
			foreach(TypeInfo type in Assemblies.Current.DefinedTypes){
				
				// Is it a FileProtocol?
				if( type.IsSubclassOf(typeof(FileProtocol)) ){
					// Yes it is - add it.
					Add((FileProtocol)Activator.CreateInstance(type.AsType()));
				}
				
			}
			
			// Startup the CSS engine:
			Css.CssProperties.Setup();
			
			#else
			
			// Get all types:
			Type[] allTypes=Assemblies.Current.GetTypes();
			
			// For each type..
			
			for(int i=allTypes.Length-1;i>=0;i--){
				Type type=allTypes[i];
				
				// Is it a FileProtocol?
				if( TypeData.IsSubclassOf(type,typeof(FileProtocol)) ){
					// Yes it is - add it.
					Add((FileProtocol)Activator.CreateInstance(type));
				}
			}
			
			// Startup the CSS engine:
			Css.CssProperties.Setup(allTypes);
			
			#endif
			
		}
		
		/// <summary>Adds the given file protocol to the global set for use.
		/// Note that you do not need to call this manually; Just deriving from
		/// FileProtocol is all that is required.</summary>
		/// <param name="protocol">The new protocol to add.</param>
		public static void Add(FileProtocol protocol){
			string[] nameSet=protocol.GetNames();
			
			if(nameSet==null){
				return;
			}
			
			foreach(string name in nameSet){
				Protocols[name.ToLower()]=protocol;
			}
		}
		
		/// <summary>Gets a protocol by the given name.</summary>
		/// <param name="protocol">The name of the protocol, e.g. "http".</param>
		/// <returns>A FileProtocol if found; null otherwise.</returns>
		public static FileProtocol Get(string protocol){
			if(protocol==null){
				protocol="";
			}
			
			FileProtocol result=null;
			Protocols.TryGetValue(protocol.ToLower(),out result);
			return result;
		}
		
	}
	
}