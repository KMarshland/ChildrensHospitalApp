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
using System.Collections;
using System.Collections.Generic;

namespace Nitro{
	
	/// <summary>
	/// Represents a method that can be called at runtime.
	/// Essentially acts like a delegate.
	/// </summary>
	
	public class DynamicMethod<T>{
	
		/// <summary>The name of the method being run.</summary>
		public string Name;
		/// <summary>The object it's being run on.</summary>
		public object Object;
		/// <summary>MethodInfo the method to run.</summary>
		public MethodInfo Method;
		
		/// <summary>Creates a new dynamic method.</summary>
		/// <param name="name">The name of the method to run.</param>
		public DynamicMethod(string name){
			Name=name;
		}
		
		/// <summary>Creates a new dynamic method.</summary>
		/// <param name="name">The name of the method to run.</param>
		/// <param name="onObject">The object to run this method on.</param>
		public DynamicMethod(string name,object onObject){
			Name=name;
			Object=onObject;
		}
		
		/// <summary>Executes this dynamic method with the given arguments.</summary>
		/// <param name="arguments">The arguments to pass to the method</param>
		/// <returns>The return value of the method, if any.</returns>
		public T Run(params object[] arguments){
			if(Method==null){
				
				#if NETFX_CORE
				Method=Object.GetType().GetTypeInfo().GetDeclaredMethod(Name);
				#else
				Method=Object.GetType().GetMethod(Name);
				#endif
				
				if(Method==null){
					throw new Exception("Method '"+Name+"' was not found.");
				}
			}
			
			object result=Method.Invoke(Object,arguments);
			
			if(result==null){
				return default(T);
			}
			
			return (T)result;
		}
		
		/// <summary>Converts this Nitro method into a delegate for use with any other kind of event.</summary>
		/// <param name="type">The type of the target event.</summary>
		public Delegate ToDelegate(Type type){
			
			if(Method==null){
				
				#if NETFX_CORE
				Method=Object.GetType().GetTypeInfo().GetDeclaredMethod(Name);
				#else
				Method=Object.GetType().GetMethod(Name);
				#endif
				
				if(Method==null){
					throw new Exception("Method '"+Name+"' was not found.");
				}
			}
			
			#if NETFX_CORE
			return Method.CreateDelegate(type);
			#else
			return Delegate.CreateDelegate(type,Method);
			#endif
			
		}
		
	}
	
}