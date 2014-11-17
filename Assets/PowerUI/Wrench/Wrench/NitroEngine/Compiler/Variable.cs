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
	/// The base class for a variable and its value. E.g. var something:type;
	/// </summary>

	public class Variable:ISettable{
	
		/// <summary>The name of the variable.</summary>
		public string Name;
		
		/// <summary>Creates a new variable with the given name.</summary>
		/// <param name="name">The name of the variable.</param>
		public Variable(string name){
			Name=name;
		}
		
		/// <summary>The type of the value that this variable holds. Note: This is not related to VariableType.</summary>
		/// <returns>The type of the value.</returns>
		public virtual Type Type(){
			return typeof(object);
		}
		
		/// <summary>Outputs this variable to read its content.</summary>
		/// <param name="into">The IL stream it should be put into.</param>
		/// <param name="accessingMember">True if the variable is being outputted and immediately having
		/// a method/property/field accessed. This is important for value type fields.</param>
		public virtual void OutputIL(NitroIL into,bool accessingMember){}
		
		/// <summary>Outputs this variable to write its content.</summary>
		/// <param name="into">The IL stream a set should be put into.</param>
		public virtual void OutputSet(NitroIL into){}
		
		/// <summary>Used with OutputSet, this outputs any additional information that 'targets' where this variable is located.</summary>
		/// <param name="into">The IL stream a set should be put into.</param>
		public virtual void OutputTarget(NitroIL into){}
		
		/// <summary>Checks if two variables are equal to each other.</summary>
		/// <param name="other">The variable to check for equality with this one.</param>
		/// <returns>True if this and the given variable is equal.</returns>
		public bool Equals(Variable other){
			if(other==null||other.GetType()!=GetType()){
				return false;
			}
			return (Name==other.Name);
		}

	}
	
}