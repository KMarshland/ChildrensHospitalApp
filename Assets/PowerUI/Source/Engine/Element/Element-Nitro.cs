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
using Wrench;


namespace PowerUI{
	
	
	public partial class Element{
		
		
		/// <summary>Runs the given function held in the named attribute (e.g. onkeydown) and checks if that function blocked
		/// the event. In the case of a blocked event, no default action should occur.</summary>
		/// <param name="attribute">The name of the attribute, e.g. onkeydown.</param>
		/// <param name="uiEvent">A standard UIEvent containing e.g. key/mouse information.</param>
		public bool RunBlocked(string attribute,UIEvent uiEvent){
			
			// Run the function:
			object result=Run(attribute,uiEvent);
			
			if(result!=null && result.GetType()==typeof(bool)){
				// It returned true/false - was it false?
				
				if(!(bool)result){
					// Returned false - Blocked it.
					return true;
				}
				
			}
			
			return uiEvent.cancelBubble;
		}
		
		/// <summary>Runs a nitro function whos name is held in the given attribute.</summary>
		/// <param name="attribute">The name of the attribute in lowercase, e.g. "onmousedown".</param>
		/// <param name="args">Additional parameters you would like to pass to your function.</param>
		/// <returns>The value returned by the function.</returns>
		/// <exception cref="NullReferenceException">Thrown if the function does not exist.</exception>
		public object Run(string attribute,params object[] args){
			return RunLiteral(attribute,args);
		}
		
		/// <summary>Runs a nitro function whos name is held in the given attribute with a fixed block of arguments.</summary>
		/// <param name="attribute">The name of the attribute in lowercase, e.g. "onmousedown".</param>
		/// <param name="args">Additional parameters you would like to pass to your function.</param>
		/// <returns>The value returned by the function.</returns>
		/// <exception cref="NullReferenceException">Thrown if the function does not exist.</exception>
		public object RunLiteral(string attribute,object[] args){
			string methodName=this[attribute];
			if(methodName==null){
				return null;
			}
			
			if(methodName.Contains(".")){
				// C# or UnityJS method.
				string[] pieces=methodName.Split('.');
				
				if(pieces.Length!=2){
					Wrench.Log.Add("onmousedown of '"+methodName+"' is invalid. If you're using a C# or UnityJS function, only one . is allowed (className.staticMethodName).");
					return null;
				}
				
				// Grab the class name:
				string className=pieces[0];
				// Go get the type:
				Type type=CodeReference.GetFirstType(className);
				
				if(type==null){
					Wrench.Log.Add("Type not found: "+className);
					return null;
				}
				
				// Update the method name:
				methodName=pieces[1];
				
				// Grab the method info:
				try{
					#if NETFX_CORE
					MethodInfo method=type.GetTypeInfo().GetDeclaredMethod(methodName);
					#else
					MethodInfo method=type.GetMethod(methodName);
					#endif
					// Invoke it:
					return method.Invoke(null,args);
				}catch(Exception e){
					Wrench.Log.Add("Calling method "+className+"."+methodName+"(..) errored: "+e);
					return null;
				}
			}
			
			return Document.RunLiteral(methodName,this,args);
		}
		
		
	}
	
}