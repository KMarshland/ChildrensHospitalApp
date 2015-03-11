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
using System.Collections.Generic;

namespace Nitro{

	/// <summary>
	/// Represents a compiled operation which can be emitted to IL.
	/// </summary>

	public class Operation:CompiledFragment{
		
		/// <summary>The line number this operation originates from.</summary>
		public int LineNumber=-1;
		/// <summary>The method that this operation belongs to.</summary>
		public CompiledMethod Method;
		/// <summary>The first input to this operation. Often the input to the left of an operator. Input0+Input1.</summary>
		public CompiledFragment Input0;
		/// <summary>The second input to this operation. Often the input to the right of an operator. Input0+Input1.</summary>
		public CompiledFragment Input1;
		
		
		/// <summary>Creates a new operation that is part of the given method.</summary>
		/// <param name="method">The method that this operation belongs to.</param>
		public Operation(CompiledMethod method){
			Method=method;
			LineNumber=method.CurrentLine;
		}
		
		/// <summary>This operation requires you "storing" its output. A variable does; for(){} does not.</summary>
		public virtual bool RequiresStoring{
			get{
				return true;
			}
		}
		
		public override int GetLineNumber(){
			
			if(LineNumber==-1){
				// Go up the DOM
				return base.GetLineNumber();
			}
			
			return LineNumber;
		}
		
		public override bool IsConstant(){
			return false;
		}
		
		/// <summary>Checks if this operation is part of a set operation.</summary>
		/// <returns>True if a set operation is anywhere in the hierarchy of this operation; false otherwise.</returns>
		public bool IsSet(){
			CodeFragment current=ParentFragment;
			while(current!=null){
				SetOperation set=current as SetOperation;
				if(set!=null){
					return true;
				}
				current=current.ParentFragment;
			}
			return false;
		}
		
		/// <summary>Forces this operation to be a particular type by creating a cast.</summary>
		/// <param name="input">A reference to this operation which may be replaced with a cast operation.</param>
		/// <param name="inputType">The current output type of this operation.</param>
		/// <param name="newType">The type that the output of this operation must be.</param>
		/// <returns>The new output type if successful. Throws an error otherwise.</returns>
		public Type EnforceType(ref CompiledFragment input,Type inputType,Type newType){
			// If inputType inherits newType, it's ok anyway:
			if(newType.IsAssignableFrom(inputType)){
				return inputType;
			}
			bool isExplicit;
			CompiledFragment frag=Types.TryCast(Method,input,newType,out isExplicit);
			if(frag==null){
				Error("Unable to enforce type '"+newType+"' on something that is a '"+inputType+"'");
			}else{
				input=frag;
			}
			return newType;
		}
		
		/// <summary>Forces this operation to have a numerical output.</summary>
		/// <param name="input">A reference to this operation which may be replaced with a cast operation.</param>
		/// <param name="inputType">The current output type of this operation.</param>
		/// <param name="newType">The type that the output of this operation must be.</param>
		/// <returns>The new output type if successful. Throws an error otherwise.</returns>
		public Type Numerical(ref CompiledFragment input,Type inputType,Type defaultType){
			if(Types.IsNumeric(inputType)){
				return inputType;
			}
			return EnforceType(ref input,inputType,defaultType);
		}
		
		/// <summary>Attempts to find the named numerical operation that the given types can perform.</summary>
		/// <param name="typeA">The type of the object on the left of the operator.</param>
		/// <param name="typeB">The type of the object on the right of the operator.</param>
		/// <param name="overloadMethod">The name of the method to look for, e.g. "Addition".</param>
		/// <param name="newOperation">The new method call if it was successfully found.</param>
		/// <returns>The new output type if successful. Throws an error otherwise.</returns>
		public Type Numerical(Type typeA,Type typeB,string overloadMethod,ref CompiledFragment newOperation){
			Type type=FindOverload(overloadMethod,typeA,typeB,ref newOperation);
			if(type==null&&((type=MapNumerical(typeA,typeB))==null)){
				Error("Could not find a suitable method to use with "+typeA+" and "+typeB+" using this operator.");
			}
			return type;
		}
		
		/// <summary>Attempts to make both Input0 and Input1 numerical, returning the output result of an operation if they are.
		/// Usually the returned type will be the one with higher accuracy. E.g. adding an int to a long will result in a long.</summary>
		/// <param name="typeA">The output type of Input0.</param>
		/// <param name="typeB">The output type of Input1.</param>
		/// <returns>The output result of a numerical operation with these types.</returns>
		public Type MapNumerical(Type typeA,Type typeB){
			bool isExplicit=false;
			CompiledFragment frag0=null;
			CompiledFragment frag1=null;
			bool typeANumeric=Types.IsNumeric(typeA);
			bool typeBNumeric=Types.IsNumeric(typeB);
			
			if(typeANumeric&&typeBNumeric){
				if(typeA==typeB){
					return typeA;
				}else{
					// Which is higher accuracy? Map to that one and cast the other.
					frag0=Types.TryCast(Method,Input0,typeB,out isExplicit);
					if(frag0!=null){
						if(isExplicit){
							// Other way is better:
							frag1=Types.TryCast(Method,Input1,typeA);
							if(frag1!=null){
								Input1=frag1;
								return typeA;
							}else{
								Input0=frag0;
								return typeB;
							}
						}else{
							Input0=frag0;
							return typeB;
						}
					}else{
						frag1=Types.TryCast(Method,Input1,typeA);
						if(frag1!=null){
							Input1=frag1;
							return typeA;
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>Looks for an overloaded numerical operator by the given name (e.g. "Addition").
		/// The name will then have op_ appended to the start of it (as used internally by .NET). If found,
		/// the output type of the operation is returned.</summary>
		/// <param name="methodName">The name of the method to look for.</param>
		/// <param name="typeA">The type of the input on the left of the operator (Input0).</param>
		/// <param name="typeB">The type of the input on the right of the operator (Input1).</param>
		/// <param name="newOperation">A method call operation if it was found which is already correctly linked to Input0 and Input1.</param>
		/// <returns>The output type of the method if found.</returns>
		public Type FindOverload(string methodName,Type typeA,Type typeB,ref CompiledFragment newOperation){
			if(typeA==null||typeB==null){
				return null;
			}
			methodName="op_"+methodName;
			// Find an overloaded operator (always static):
			MethodInfo mI=typeA.GetMethod(methodName,new Type[]{typeA,typeB});
			if(mI==null){
				mI=typeB.GetMethod(methodName,new Type[]{typeB,typeA});
				if(mI==null){
					return null;
				}
				newOperation=new MethodOperation(Method,mI,Input1,Input0);
			}else{
				newOperation=new MethodOperation(Method,mI,Input0,Input1);
			}
			return newOperation.OutputType(out newOperation);
		}
		
	}
	
}