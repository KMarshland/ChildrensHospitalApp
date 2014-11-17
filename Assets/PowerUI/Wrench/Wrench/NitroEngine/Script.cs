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

namespace Nitro{

	/// <summary>
	/// A base class that code instances derive from by default.
	/// <see cref="Nitro.NitroCode.Instance"/> returns an object which should be casted to this
	/// unless a different base type was used in the NitroCode constructor.
	/// </summary>

	public class Script{
		
		/// <summary>Runs the named method that is contained within this script.</summary>
		/// <param name="functionName">The method to run.</param>
		/// <param name="args">The arguments to pass to the method.</param>
		/// <returns>The returned object of the method, if any.</returns>
		public object Run(string functionName,params object[] args){
			return RunLiteral(functionName,null,args);
		}
		
		/// <summary>Runs the named overloaded method that is contained within this script.</summary>
		/// <param name="functionName">The method to run.</param>
		/// <param name="typeSet">The types of the arguments that will be used to find the correct overload.</param>
		/// <param name="args">The arguments to pass to the method.</param>
		/// <returns>The returned object of the method, if any.</returns>
		public object Run(string functionName,Type[] typeSet,params object[] args){
			return RunLiteral(functionName,typeSet,args);
		}
		
		/// <summary>Any code found outside of functions is placed into this method.
		/// It's done like this so when the code is instanced it can have properties set then have this called.</summary>
		public virtual void OnScriptReady(){}
		
		/// <summary>Runs the named method that is contained within this script.</summary>
		/// <param name="functionName">The method to run.</param>
		/// <param name="args">The set of arguments to pass to the method.</param>
		/// <returns>The returned object of the method, if any.</returns>
		public object RunLiteral(string functionName,object[] args){
			return RunLiteral(functionName,null,args);
		}
		
		/// <summary>Runs the named overloaded method that is contained within this script.</summary>
		/// <param name="functionName">The method to run.</param>
		/// <param name="typeSet">The types of the arguments that will be used to find the correct overload.</param>
		/// <param name="args">The set of arguments to pass to the method.</param>
		/// <returns>The returned object of the method, if any.</returns>
		public object RunLiteral(string functionName,Type[] typeSet,object[] args){
			return RunLiteral(functionName,typeSet,args,false);
		}
		
		/// <summary>Runs the named overloaded method that is contained within this script.</summary>
		/// <param name="functionName">The method to run.</param>
		/// <param name="args">The set of arguments to pass to the method.</param>
		/// <param name="optional">True if calling this method is optional. No error is thrown if not found.</param>
		/// <returns>The returned object of the method, if any.</returns>
		public object RunLiteral(string functionName,object[] args,bool optional){
			return RunLiteral(functionName,null,args,optional);
		}
		
		/// <summary>Runs the named overloaded method that is contained within this script.</summary>
		/// <param name="functionName">The method to run.</param>
		/// <param name="typeSet">The types of the arguments that will be used to find the correct overload.</param>
		/// <param name="args">The set of arguments to pass to the method.</param>
		/// <param name="optional">True if calling this method is optional. No error is thrown if not found.</param>
		/// <returns>The returned object of the method, if any.</returns>
		public object RunLiteral(string functionName,Type[] typeSet,object[] args,bool optional){
			functionName=functionName.ToLower();
			MethodInfo mInfo=null;
			try{
				
				if(typeSet==null){
					#if NETFX_CORE
					mInfo=GetType().GetTypeInfo().GetDeclaredMethod(functionName);
					#else
					mInfo=GetType().GetMethod(functionName);
					#endif
				}else{
					#if NETFX_CORE
					mInfo=GetType().GetTypeInfo().GetDeclaredMethod(functionName);
					#else
					mInfo=GetType().GetMethod(functionName,typeSet);
					#endif
				}
				
			}catch{}
			if(mInfo==null){
				if(optional){
					return null;
				}
				throw new Exception("Method "+functionName+" was not found. If this method is overloaded, you must give the type set for the particular overload you want.");
			}
			return mInfo.Invoke(this,args);
		}
		
		/// <summary>Gets or sets a field with the given name.</summary>
		/// <param name="property">The name of the field.</param>
		public object this[string property]{
			get{
				FieldInfo field=null;
				try{
					
					#if NETFX_CORE
					field=GetType().GetTypeInfo().GetDeclaredField(property);
					#else
					field=GetType().GetField(property);
					#endif
					
				}catch{}
				if(field!=null){
					return field.GetValue(this);
				}
				return null;
			}
			set{
				FieldInfo field=null;
				try{
					
					#if NETFX_CORE
					field=GetType().GetTypeInfo().GetDeclaredField(property);
					#else
					field=GetType().GetField(property);
					#endif
					
				}catch{}
				if(field!=null){
					field.SetValue(this,value);
				}
			}
		}
		
	}
	
}