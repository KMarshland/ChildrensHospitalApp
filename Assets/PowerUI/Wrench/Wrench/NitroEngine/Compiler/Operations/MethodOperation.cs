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
	/// Represents a method call.
	/// </summary>
	
	public class MethodOperation:Operation{
		
		/// <summary>The method to call when this operation runs.</summary>
		public MethodInfo MethodToCall;
		/// <summary>The object that this method is called on. E.g. Something.SomeMethod();</summary>
		public CompiledFragment CalledOn;
		/// <summary>The arguments to pass to the method in the call.</summary>
		public CompiledFragment[] Arguments;
		
		
		public MethodOperation(CompiledMethod method,MethodInfo methodInfo,params CompiledFragment[] arguments):base(method){
			MethodToCall=methodInfo;
			Arguments=arguments;
		}
		
		public MethodOperation(CompiledMethod method,CompiledFragment calledOn,CompiledFragment[] arguments):base(method){
			CalledOn=calledOn;
			Arguments=arguments;
		}
		
		public override bool RequiresStoring{
			get{
				return false;
			}
		}
		
		public override bool IsMemberAccessor(){
			return true;
		}
		
		/// <summary>Loads the MethodToCall value if it needs to.</summary>
		private void GetMethodInfo(){
			if(MethodToCall!=null){
				return;
			}
			if(Arguments!=null){
				for(int i=0;i<Arguments.Length;i++){
					CompiledFragment p=Arguments[i];
					p.OutputType(out p);
					Arguments[i]=p;
				}
			}
			
			// Note: Most things go via a DynamicMethod here.
			// This is because CalledOn is mostly a PropertyOperation.
			// It's converted to a static singular MethodInfo with GetOverload below though.
			
			Type fragType=CalledOn.OutputType(out CalledOn);
			PropertyOperation prop=CalledOn as PropertyOperation;
				
			if(prop!=null&&prop.MethodReturnType!=null){
				MethodToCall=prop.GetOverload(Arguments);
				if(MethodToCall==null){
					Error("Method "+prop.Name+" was not found.");
				}
				CalledOn=MethodToCall.IsStatic?null:prop.Of;
			}else{
				MethodToCall=Types.GetCallable(fragType);
			}
			
			if(MethodToCall==null){
				Error("Unable to run '"+Name+"' as a method.");
			}
		}
		
		/// <summary>The original property name.</summary>
		public string Name{
			get{
				PropertyOperation prop=CalledOn as PropertyOperation;
				if(prop==null){
					return "";
				}
				return prop.Name;
			}
		}
		
		/// <summary>The raw method name.</summary>
		public string MethodName{
			get{
				GetMethodInfo();
				
				if(MethodToCall!=null){
					return MethodToCall.Name;
				}
				
				return "";
			}
		}
		
		public override Type OutputType(out CompiledFragment nv){
			nv=this;
			GetMethodInfo();
			if(Types.NoReturn(MethodToCall)){
				Error("'"+Name+"' does not return anything");
			}
			return MethodToCall.ReturnType;
		}
		
		public override void OutputIL(NitroIL into){
			GetMethodInfo();
			// First, the instance this method is on:
			bool useVirtual=(CalledOn!=null);
			
			if(useVirtual){
				Type type=CalledOn.OutputType(out CalledOn);
				CalledOn.OutputIL(into);
				if(type.IsValueType){
					
					if(!CalledOn.EmitsAddress){
						// Value must be set into a temporary local and reloaded (but as an address).
						// Future optimization may be to pool these.
						LocalBuilder builder=into.DeclareLocal(type);
						into.Emit(OpCodes.Stloc,builder);
						into.Emit(OpCodes.Ldloca,builder);
					}
					
					useVirtual=false;
				}
			}
			
			// Next, its arguments:
			if(Types.IsDynamic(MethodToCall)){
				if(Arguments!=null){
					for(int i=0;i<Arguments.Length;i++){
						Arguments[i].OutputIL(into);
					}
				}
			}else{
				Types.OutputParameters(Arguments,Method,into,MethodToCall.GetParameters());
			}
			
			// And emit the call:
			if(useVirtual){
				into.Emit(OpCodes.Callvirt,MethodToCall);
			}else{
				into.Emit(OpCodes.Call,MethodToCall);
			}
		}
		
	}
	
}