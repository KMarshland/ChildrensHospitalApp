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
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents a typeof (typeof "hi") operation.
	/// </summary>
	
	public class TypeofOperation:Operation{
		
		public static string GetObjectType(object value){
			
			if(value==null){
				return "undefined";
			}
			
			Type type=value.GetType();
			
			if(type==typeof(string)){
				return "string";
			}else if(type==typeof(double) ||
					type==typeof(float) ||
					type==typeof(long) ||
					type==typeof(int) ||
					type==typeof(short)
					){
					
				return "number";
				
			}else if(type==typeof(bool)){
					return "boolean";
			}else{
				return "object";
			}
			
		}
		
		public TypeofOperation(CompiledMethod method):base(method){}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			
			if(Input0!=null){
				
				Type i0Type=Input0.OutputType(out Input0);
				
				if(i0Type!=null && i0Type.IsValueType){
					// Create a box operation:
					Input0=new BoxOperation(Method,Input0);
				}
				
			}
			
			return typeof(string);
		}
		
		public override void OutputIL(NitroIL into){
			
			// Get Input0 onto the stack:
			if(Input0==null){
				into.Emit(OpCodes.Ldnull,typeof(object));
			}else{
				Input0.OutputIL(into);
			}
			// Run GetObjectType() method:
			#if NETFX_CORE
			MethodInfo getType=typeof(TypeofOperation).GetTypeInfo().GetMethod("GetObjectType");
			#else
			MethodInfo getType=typeof(TypeofOperation).GetMethod("GetObjectType");
			#endif
			
			// Call it:
			into.Emit(OpCodes.Call,getType);
			
		}
		
	}
	
}