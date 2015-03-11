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

namespace Nitro{
	
	/// <summary>
	/// Represents a local variable; One defined within the body of a method.
	/// </summary>
	
	public class LocalVariable:Variable{
		
		/// <summary>A type if any of this variable.</summary>
		private Type VariableType;
		/// <summary>The builder that will later be used to compiled this to IL.</summary>
		public LocalBuilder Builder;
		
		
		/// <summary>Creates a new named LocalVarable.</summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="type">The optional type of this variable.</param>
		public LocalVariable(string name,Type type):base(name){
			VariableType=type;
		}
		
		public override Type Type(){
			return VariableType;
		}
		
		public override void OutputSet(NitroIL into,Type setting){
			
			if(VariableType==null){
				
				VariableType=setting;
				
			}else if(setting!=VariableType){
				
				// Overwriting the variable with something of a different type. Create a new one.
				VariableType=setting;
				Builder=null;
				
			}
			
			if(VariableType==null){
				VariableType=typeof(object);
			}
			
			if(Builder==null){
				
				Builder=into.DeclareLocal(VariableType);
				
			}
			
			into.Emit(OpCodes.Stloc,Builder);
		}
		
		/// <summary>Outputs this variable into IL.</summary>
		/// <param name="into">The IL stream to output it into.</param>
		/// <param name="accessingMember">True if we are accessing a field/method of this variable.</param>
		public override void OutputIL(NitroIL into,bool accessingMember){
			
			if(Builder==null){
				
				// This variable hasn't been written to yet, so this is always like reading a null or zero.
				return;
				
			}
			
			if(accessingMember&&VariableType.IsValueType){
				// Must load by reference. It's a value type and we want some property of it.
				into.Emit(OpCodes.Ldloca,Builder);
			}else{
				into.Emit(OpCodes.Ldloc,Builder);
			}
			
		}
		
	}
	
}