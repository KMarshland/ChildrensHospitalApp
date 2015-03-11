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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Nitro{

	/// <summary>
	/// Provides some global methods for compiling operations into executable instructions.
	/// </summary>

	public static class CompilationServices{
		
		/// <summary>Compiles an operation fragment into an executable operation. The fragment may contain multiple
		/// operators and some may even be the same so the furthest right takes priority; 1+2+3 becomes 1+(2+3).
		/// The compiled fragments are placed back into the same operation fragment. When it's complete, the operation
		/// will only contain one compiled fragment.</summary>
		/// <param name="fragment">The operation fragment to compile.</param>
		/// <param name="parentBlock">The method the operations are compiling into.</param>
		public static void CompileOperators(OperationFragment fragment,CompiledMethod parentBlock){
			CodeFragment child=fragment.FirstChild;
			OperatorFragment highestPriority=null;
			
			while(child!=null){
				
				if(child.GetType()==typeof(OperatorFragment)){
					OperatorFragment thisOperator=(OperatorFragment)child;
					
					// <= below enforces furthest right:
					if(highestPriority==null||highestPriority.Value.Priority<=thisOperator.Value.Priority){
						highestPriority=thisOperator;
					}
					
				}
				
				child=child.NextChild;
			}
			
			if(highestPriority==null){
				return;
			}
			
			CodeFragment left=highestPriority.PreviousChild;
			CodeFragment right=highestPriority.NextChild;
			CodeFragment leftUsed=left;
			CodeFragment rightUsed=right;
			
			if(left==null||left.GetType()==typeof(OperatorFragment)){
				leftUsed=null;
			}else if(!Types.IsCompiled(left)){
				leftUsed=left.Compile(parentBlock);
			}
			
			if(right==null||right.GetType()==typeof(OperatorFragment)){
				rightUsed=null;
			}else if(!Types.IsCompiled(right)){
				rightUsed=right.Compile(parentBlock);
			}
			
			Operation newFragment=highestPriority.Value.ToOperation((CompiledFragment)leftUsed,(CompiledFragment)rightUsed,parentBlock);
			
			if(newFragment==null){
				highestPriority.Error("Error: An operator has been used but with nothing to use it on! (It was a '"+highestPriority.Value.Pattern+"')");
			}
			
			// Replace out Left, Right and the operator itself with the new fragment:
			highestPriority.Remove();
			
			if(left==null){
				newFragment.AddBefore(right);
			}else{
				newFragment.AddAfter(left);
			}
			
			if(rightUsed!=null){
				right.Remove();
			}
			
			if(leftUsed!=null){
				left.Remove();
			}
			
			// And call again to collect the rest:
			CompileOperators(fragment,parentBlock);
		}
		
		/// <summary>Compiles all operations in a given fragment into executable IL.</summary>
		/// <param name="fragment">The parent fragment. Most likely represents a pair of brackets.</param>
		/// <param name="block">The method that the operations represent.</param>
		/// <returns>True if the block returns something.</returns>
		public static bool CompileOperations(CodeFragment fragment,CompiledMethod block){
			
			CodeFragment child=fragment.FirstChild;
			bool returns=false;
			
			while(child!=null){
				
				if(Types.IsTypeOf(child,typeof(Operation))){
					
					Operation operation=(Operation)child;
					Type t=operation.GetType();
					
					try{
						
						if(operation.RequiresStoring){
							child.Error("Unexpected operation or value - you must give somewhere to hold the result of this.");
						}
						
						// Add the above operation to the block's IL:
						operation.OutputIL(block.ILStream);
						
					}catch(CompilationException e){
						
						// Setup line number:
						if(e.LineNumber==-1){
							e.LineNumber=operation.LineNumber;
						}
						
						// Rethrow:
						throw e;
						
					}
					
					
					if(t==typeof(MethodOperation)){
						
						Type returnType=(((MethodOperation)operation).MethodToCall).ReturnType;
						
						if(returnType!=null && returnType!=typeof(void)){
							// Note this doesn't include (nitro) Void because it's an object as far as .net is concerned!
							block.ILStream.Emit(OpCodes.Pop);
						}
						
					}
					
					returns=(t==typeof(ReturnOperation)||(t==typeof(IfOperation)&&((IfOperation)operation).AllRoutesReturn));
					
					if(returns){
						
						if(child.NextChild!=null){
							Wrench.Log.Add("Warning: Unreachable code detected at line "+child.NextChild.GetLineNumber());
							child.NextChild=null;
						}
						
						break;
					}
					
				}
				
				child=child.NextChild;
			}
			
			return returns;
		}
		
		/// <summary>Compiles a set of parameters into an array of compiled fragments.</summary>
		/// <param name="brackets">The parent block which contains each parameter as a child.</param>
		/// <param name="parentBlock">The method the parameters are for.</param>
		/// <returns>A set of compiled fragments.</returns>
		public static CompiledFragment[] CompileParameters(CodeFragment brackets,CompiledMethod parentBlock){
			if(!brackets.IsParent){
				return null;
			}
			
			int count=0;
			brackets.Compile(parentBlock);
			CodeFragment child=brackets.FirstChild;
			CompiledFragment[] output=new CompiledFragment[brackets.ChildCount()];
			
			while(child!=null){
				CompiledFragment frag=(CompiledFragment)child;
				
				if(frag==null){
					return null;
				}
				
				output[count]=frag;
				count++;
				child=child.NextChild;
			}
			
			return output;
		}
		
	}
	
}