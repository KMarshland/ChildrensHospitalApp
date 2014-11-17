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
		
		/// <summary>The builder that will later be used to compiled this to IL.</summary>
		public LocalBuilder Builder;
		
		
		/// <summary>Creates a new named LocalVarable.</summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="builder">The local builder used to output this variable in IL.</param>
		public LocalVariable(string name,LocalBuilder builder):base(name){
			Builder=builder;
		}
		
		public override Type Type(){
			return Builder.LocalType;
		}
		
		public override void OutputSet(NitroIL into){
			into.Emit(OpCodes.Stloc,Builder);
		}
		
		/// <summary>Outputs this variable into IL.</summary>
		/// <param name="into">The IL stream to output it into.</param>
		/// <param name="accessingMember">True if we are accessing a field/method of this variable.</param>
		public override void OutputIL(NitroIL into,bool accessingMember){
			if(accessingMember&&Builder.LocalType.IsValueType){
				// Must load by reference. It's a value type and we want some property of it.
				into.Emit(OpCodes.Ldloca,Builder);
			}else{
				into.Emit(OpCodes.Ldloc,Builder);
			}
		}
		
	}
	
}