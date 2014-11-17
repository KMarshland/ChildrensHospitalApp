//--------------------------------------
//         Nitro Script Engine
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// A security manager which defines what is accessible by Nitro.
	/// </summary>
	
	public class NitroDomainManager{
		
		/// <summary>The default security manager if none is provided.</summary>
		private static NitroDomainManager DefaultManager=new NitroDomainManager();
		
		/// <summary>Gets the default security manager.</summary>
		public static NitroDomainManager GetDefaultManager(){
			return DefaultManager;
		}
		
		/// <summary>True if this domain allows everything.</summary>
		private bool AllowAll;
		/// <summary>A set of all blocked types by name.</summary>
		private List<SecureName> BlockedNames;
		/// <summary>A set of classes included and allowed by default.</summary>
		private List<CodeReference> DefaultReferences;
		/// <summary>A set of all allowed types by name.</summary>
		private List<SecureName> AllowedNames=new List<SecureName>();
		
		
		/// <summary>Creates a default security manager which allows the Nitro and System namespaces.</summary>
		public NitroDomainManager(){
			AddReference(".Nitro");
			AddReference("System.System");
			
			Allow("Nitro");
			Allow("System");
		}
		
		/// <summary>Clears all default references.</summary>
		public void ClearDefaultReferences(){
			DefaultReferences=null;
		}
		
		/// <summary>Gets the default references.</summary>
		/// <returns>The list of default code references.</returns>
		public List<CodeReference> GetDefaultReferences(){
			if(DefaultReferences==null){
				return null;
			}
			List<CodeReference> results=new List<CodeReference>(DefaultReferences.Count);
			foreach(CodeReference reference in DefaultReferences){
				results.Add(reference);
			}
			return results;
		}
		
		/// <summary>Adds the given text as a reference. Must also include the assembly unless it is in 'this' one.</summary>
		/// <param name="text">The reference to add, e.g. "Nitro" or "System.System".</param>
		protected void AddReference(string text){
			if(DefaultReferences==null){
				DefaultReferences=new List<CodeReference>();
			}
			CodeReference codeRef=new CodeReference(text);
			DefaultReferences.Add(codeRef);
		}
		
		/// <summary>This value states if this domain allows every type.
		/// Do note that the NitroDomainManager type is always blocked.</summary>
		/// <returns>True if it allows every type, false otherwise.</returns>
		public bool AllowsEverything(){
			return AllowAll;
		}
		
		/// <summary>Checks if the named type is allowed by this domain.</summary>
		/// <param name="name">The type to check for.</param>
		/// <returns>True if the type is allowed by this domain.</returns>
		public bool IsAllowed(Type type){
			// Check that its not blocked:
			if(Blocked(type)){
				return false;
			}
			if(type==null || AllowedNames==null){
				return false;
			}
			foreach(SecureName allowed in AllowedNames){
				if(allowed.Matches(type)){
					return true;
				}
			}
			return false;
		}
		
		/// <summary>Call this to make this domain allow any type except for those in its block list.</summary>
		protected void AllowEverything(){
			AllowAll=true;
		}
		
		/// <summary>Lets this domain allow the given type/ namespace.
		/// You must derive this class and call this from within the constructor.</summary>
		/// <param name="name">The type/ namespace to allow.</param>
		protected void Allow(string name){
			if(AllowedNames==null){
				AllowedNames=new List<SecureName>();
			}
			AllowedNames.Add(new SecureName(name));
		}
		
		/// <summary>Lets this domain block the given type/ namespace. Used when this domain allows everything except a certain few.
		/// You must derive this class and call this from within the constructor.</summary>
		/// <param name="name">The name of the type/ namespace to block.</param>
		protected void Block(string name){
			if(BlockedNames==null){
				BlockedNames=new List<SecureName>();
			}
			BlockedNames.Add(new SecureName(name));
		}
		
		/// <summary>Checks if the given type is blocked by this domain.</summary>
		/// <param name="type">The type to check for.</param>
		/// <returns>True if this domain blocks the named type.</returns>
		private bool Blocked(Type type){
			if(type==null || BlockedNames==null){
				return false;
			}
			foreach(SecureName blocked in BlockedNames){
				if(blocked.Matches(type)){
					return true;
				}
			}
			return false;
		}
		
		/// <summary>Checks if the given remote location is allowed any access at all.
		/// Note that this can internally update the domain itself if it wants to apply further restrictions.</summary>
		/// <param name="protocol">The protocol:// of the remote location. Lowercase and usually e.g. http(s).</param>
		/// <param name="host">The first part of the location after the protocol. E.g. www.kulestar.com.</param>
		/// <param name="fullPath">The whole path requesting access. E.g. http://www.kulestar.com/.</param>
		public bool AllowAccess(string protocol,string host,string fullPath){
			return false;
		}
		
	}
	
}