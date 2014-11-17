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
	/// Represents a construct operation. E.g. new typename();
	/// </summary>
	
	public class ConstructOperation:Operation{
		
		/// <summary>The type of object being constructed.</summary>
		public Type ObjectType;
		/// <summary>The constructor that will be called.</summary>
		public ConstructorInfo Constructor;
		/// <summary>The parameters being passed to the constructor.</summary>
		public CompiledFragment[] Parameters;
		
		
		public ConstructOperation(TypeFragment type,BracketFragment brackets,CompiledMethod method):base(method){
			if(type==null){
				Error("A constructor is missing the type to construct. E.g. new myClass();");
			}
			ObjectType=type.FindType(method.Script);
			if(ObjectType==null){
				Error("Couldn't find type '"+type+"'.");
			}
			// Compile the brackets - what types to they have?
			Parameters=CompilationServices.CompileParameters(brackets,method);
			SetConstructor();
		}
		
		public ConstructOperation(CompiledMethod method,Type type,params CompiledFragment[] parameters):base(method){
			ObjectType=type;
			Parameters=parameters;
			SetConstructor();
		}
		
		/// <summary>Sets the constructor by loading it from the ObjectType.</summary>
		public void SetConstructor(){
			Constructor=Types.GetConstructor(ObjectType,Parameters);
			if(Constructor==null){
				Error("No constructor found that matches the given parameters. "+ObjectType+", "+Parameters);
			}
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			if(Parameters!=null){
				for(int i=0;i<Parameters.Length;i++){
					CompiledFragment p=Parameters[i];
					p.OutputType(out p);
					Parameters[i]=p;
				}
			}
			return ObjectType;
		}
		
		public override void OutputIL(NitroIL into){
			if(Types.IsDynamic(Constructor)){
				if(Parameters!=null){
					for(int i=0;i<Parameters.Length;i++){
						Parameters[i].OutputIL(into);
					}
				}
			}else{
				Types.OutputParameters(Parameters,Method,into,Constructor.GetParameters());
			}
			into.Emit(OpCodes.Newobj,Constructor);
		}
	}
	
}