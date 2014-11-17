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
using Nitro;

/// <summary>
/// Provides Ahead-of-Time compiled Nitro.
/// </summary>

namespace PowerUI{

	public static class NitroCache{
	
		public static string GetCodeSeed(string code){
			if(string.IsNullOrEmpty(code)){
				return "";
			}
			
			ulong seed=0;
			int shiftBy=0;
			
			for(int i=0;i<code.Length;i++){
				byte charCode=(byte)code[i];
				seed+=((ulong)charCode<<shiftBy);
				shiftBy+=8;
				if(shiftBy==64){
					shiftBy=0;
				}
			}
			
			return code.Length+"-"+seed;
		}
	
		public static UICode TryGetScript(string code){
			string seed=GetCodeSeed(code);
			
			if(seed==""){
				return null;
			}
			
			// Does the seed exist in the cache?
			
			Assembly assembly=CodeReference.GetAssembly(seed+".ntro");
			
			if(assembly!=null){
				// Great - got the assembly. Grab the type:
				Type scriptType=assembly.GetType("NitroScriptCode");
				
				if(scriptType==null){
					return null;
				}
				
				// Instance it:
				return (UICode)Activator.CreateInstance(scriptType);
			}
			
			return null;
		}
	
	}

}