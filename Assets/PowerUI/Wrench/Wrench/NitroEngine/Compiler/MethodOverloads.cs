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

namespace Nitro{

	/// <summary>
	/// This represents a set of methods each with the same name and return type.
	/// It's used by classes being compiled to track the methods within them.
	/// </summary>

	public class MethodOverloads{
		
		/// <summary>The return type of all methods in this set.</summary>
		public Type ReturnType;
		/// <summary>The set of overloaded methods.</summary>
		public List<CompiledMethod> Methods=new List<CompiledMethod>();
		
		/// <summary>Creates a new method overloads set. All methods must have the same return type.</summary>
		/// <param name="returnType">The return type of all methods in this set.</param>
		public MethodOverloads(Type returnType){
			ReturnType=returnType;
		}
		
		/// <summary>Adds a method to this overload set.</summary>
		/// <param name="method">The method to add.</param>
		public void AddMethod(CompiledMethod method){
			Methods.Add(method);
		}
		
		/// <summary>Makes sure that two overloads don't have the same parameter set. Throws an error if </summary>
		/// <param name="method">The method to check for matches against existing methods in this set.</param>
		public void ParametersOk(CompiledMethod method){
			foreach(CompiledMethod Method in Methods){
				if(Method==method||!Method.ParametersLoaded){
					continue;
				}
				if(Types.TypeSetsMatchExactly(method.ParameterTypes,Method.ParameterTypes)){
					method.Error("Unable to add method "+method.Name+" - two overloads exist with the same parameters.");
				}
			}
		}
		
		/// <summary>Gets the overload from this set that suits the given arguments.</summary>
		/// <param name="arguments">The types of the arguments being provided.</param>
		public MethodInfo GetOverload(Type[] arguments){
			foreach(CompiledMethod method in Methods){
				if(!Types.TypeSetsMatch(arguments,method.ParameterTypes)){
					continue;
				}
				return method.getMethodInfo();
			}
			return null;
		}
		
		/// <summary>Compiles the parameter block for all methods in this set.</summary>
		public void CompileParameters(){
			foreach(CompiledMethod method in Methods){
				method.ParseParameters();
			}
		}
		
		/// <summary>Compiles the body of all methods in this set.</summary>
		public void CompileBody(){
			if(Methods.Count==0){
				return;
			}
			MethodInfo initInfo=null;
			// Grab the first method so we can find the name:
			CompiledMethod tempMethod=Methods[0];
			if(tempMethod.Name=="new"){
				// Do we have INIT? if yes, output a call to it immediately.
				MethodOverloads set=tempMethod.Parent.FindMethodSet(".init");
				if(set!=null){
					initInfo=set.Methods[0].getMethodInfo();
				}
			}else if(tempMethod.Name==".init"){
				// Got constructors? if not make one now into a set so the above occurs.
				MethodOverloads set=tempMethod.Parent.FindMethodSet("new");
				if(set==null){
					set=new MethodOverloads(typeof(Void));
					set.AddMethod(new CompiledMethod(tempMethod.Parent,"new",null,new BracketFragment(),null,0,true));
					set.CompileBody();
				}
			}
			
			foreach(CompiledMethod method in Methods){
				if(method.Name=="new"){
					// This is a constructor. If the constructor body doesn't contain a base constructor call, add one like so.
					if(!NewBaseCall(method.CodeBlock)){
						// Call the base constructor.
						method.ILStream.Emit(OpCodes.Ldarg_0);
						// Find the constructor with no parameters:
						ConstructorInfo baseConstructor=method.Parent.BaseType.GetConstructor(new Type[0]);
						
						if(baseConstructor==null){
							method.CodeBlock.Error("You must call base.new(...); as "+method.Parent.BaseType+" doesn't have a constructor for 0 args.");
						}
						
						// Emit the call:
						method.ILStream.Emit(OpCodes.Call,baseConstructor);
					}
				}
				
				if(initInfo!=null){
					method.ILStream.Emit(OpCodes.Ldarg_0);
					method.ILStream.Emit(OpCodes.Call,initInfo);
				}
				
				bool returns=method.CodeBlock.CompileBody(method);
				if(!returns){
					if(!Types.IsVoid(ReturnType)){
						method.CodeBlock.Error("Not all code paths return a value in '"+method.Name+"'");	
					}
				}
				method.ILStream.MarkLabel(method.EndOfMethod);
				if(!Types.IsVoid(ReturnType)){
					method.ILStream.Emit(OpCodes.Ldloc,method.ReturnBay);
				}
				method.ILStream.Emit(OpCodes.Ret);
				method.Done();
			}
		}
		
		/// <summary>Looks for base.new in the given fragment.</summary>
		/// <param name="fragment">The code fragment to look in.</param>
		/// <returns>True if base.new is in this fragment.</returns>
		private bool NewBaseCall(CodeFragment fragment){
			CodeFragment child=fragment.FirstChild;
			while(child!=null){
				if(child.IsParent()){
					if(NewBaseCall(child)){
						return true;
					}
				}else if(child.GetType()==typeof(MethodFragment)){
					CodeFragment name=((MethodFragment)child).MethodName;
					
					if(name.GetType()==typeof(PropertyFragment) && (((PropertyFragment)name).Value)=="new"){
						return true;
					}
				}
				child=child.NextChild;
			}
			
			return false;
		}
		
	}
	
}