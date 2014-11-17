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
using System.Reflection.Emit;
using System.Collections.Generic;
using Wrench;

namespace Nitro{
	
	/// <summary>
	/// Represents a method call. 
	/// </summary>
	
	public class MethodFragment:CodeFragment{
	
		/// <summary>The name of the method being called.</summary>
		public CodeFragment MethodName;
		/// <summary>The brackets containing the arguments for the method.</summary>
		public BracketFragment Brackets;
		
		
		/// <summary>Creates a new method call.</summary>
		/// <param name="brackets">The brackets containing the arguments for the method.</param>
		/// <param name="methodName">The name of the method being called.</param>
		public MethodFragment(BracketFragment brackets,CodeFragment methodName){
			Brackets=brackets;
			MethodName=methodName;	
		}
		
		public override bool Typeable(){
			return true;
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			if(MethodName.GetType()==typeof(VariableFragment)){
				VariableFragment vfrag=(VariableFragment)MethodName;
				if(vfrag.Value=="new"){
					CodeFragment p=to.LastChild;
					if(p==null||p.GetType()!=typeof(VariableFragment)||((VariableFragment)p).Value!="function"){
						// Constructor call.
						vfrag.Remove();
						return new ConstructorFragment(vfrag.GivenType,Brackets).AddTo(to,sr);
					}
				}
			}
			return base.AddTo(to,sr);
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			CompiledFragment cfrag=MethodName.Compile(method);
			if(!Types.IsTypeOf(cfrag.ActiveValue(),typeof(ISettable))){
				Error("Unable to compile a method call - didn't recognise an invokable object.");	
			}
			CompiledFragment[] parameters=CompilationServices.CompileParameters(Brackets,method);
			return new MethodOperation(method,cfrag,parameters);
		}
		
		public override string ToString(){
			string result=MethodName.ToString();
			if(Brackets!=null){
				result+=Brackets.ToString();
			}
			if(GivenType!=null){
				result+=GivenType.ToString();
			}
			return result;
		}
		
	}
	
}