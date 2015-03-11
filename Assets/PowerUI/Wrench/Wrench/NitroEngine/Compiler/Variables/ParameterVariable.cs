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
	/// Represents a parameter variable; A value that is received in a method as a parameter. methodName(param1,param2){.
	/// </summary>
	
	public class ParameterVariable:Variable{
	
		/// <summary>The type of this parameters value.</summary>
		public Type ParameterType;
		/// <summary>The builder that will later be used to compile this to IL. Set by a CompiledMethod.</summary>
		public ParameterBuilder Builder;
		
		
		/// <summary>Creates a new named ParameterVarable.</summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="type">The type of the value in this parameter.</param>
		public ParameterVariable(string name,Type type):base(name){
			ParameterType=type;
		}
		
		public override Type Type(){
			return ParameterType;
		}
		
		public override void OutputSet(NitroIL into,Type setting){
			into.Emit(OpCodes.Starg,Builder.Position);
		}
		
		/// <summary>Outputs this variable into IL.</summary>
		/// <param name="into">The IL stream to output it into.</param>
		/// <param name="accessingMember">True if we are accessing a field/method of this variable.</param>
		public override void OutputIL(NitroIL into,bool accessingMember){
			if(accessingMember&&ParameterType.IsValueType){
				// Must load by reference. It's a value type and we want some property of it.
				into.Emit(OpCodes.Ldarga,Builder.Position);
			}else{
				into.Emit(OpCodes.Ldarg,Builder.Position);
			}
		}
		
	}
	
}