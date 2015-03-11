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
	/// Represents an indexing operation on an array. e.g. AppliedTo[Index].
	/// </summary>
	
	public class IndexOperation:Operation,ISettable{
		
		/// <summary>The type of an element in the array.</summary>
		public Type ElementType;
		/// <summary>The fragment being used for the index. Mostly numeric or a string.</summary>
		public CompiledFragment Index;
		/// <summary>The array the index is being looked up in. Generally this will be a variable to something that is an array.</summary>
		public CompiledFragment AppliedTo;
		
		
		public IndexOperation(CompiledMethod method,CompiledFragment appliedTo,CompiledFragment[] indices):base(method){
			AppliedTo=appliedTo;
			if(indices.Length==0){
				Error("No index value given. must be e.g. array[1].");
			}else if(indices.Length>1){
				Error("Multidimension arrays are not currently supported.");
			}
			Index=indices[0];
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			if(ElementType==null){
				Type type=AppliedTo.OutputType(out AppliedTo);
				Type indexType=Index.OutputType(out Index);
				if(!type.IsArray){
					// Using an index on something that isn't an array - this maps to the get_Item/set_Item functions.
					MethodInfo[] allMethods=type.GetMethods();
					
					MethodOperation mOp=null;
					
					if(Input0!=null){
						// Set. Input0 is the object we're setting.
						Type setType=Input0.OutputType(out Input0);
						MethodInfo mInfo=Types.GetOverload(allMethods,"set_Item",new Type[]{indexType,setType});
						if(mInfo==null){
							Error("This object does not support setting values with ["+indexType+"]="+setType+".");
						}
						mOp=new MethodOperation(Method,mInfo,Index,Input0);
						v=mOp;
						mOp.CalledOn=AppliedTo;
						return setType;
					}else{
						// Get.
						MethodInfo mInfo=Types.GetOverload(allMethods,"get_Item",new Type[]{indexType});
						if(mInfo==null){
							Error("Unable to index [] something that as it is not an array.");
						}
						mOp=new MethodOperation(Method,mInfo,Index);
						v=mOp;
						mOp.CalledOn=AppliedTo;
						return mInfo.ReturnType;
					}
					
				}else{
					EnforceType(ref Index,indexType,typeof(int));
				}
				ElementType=type.GetElementType();
			}
			return ElementType;
		}
		
		public void OutputTarget(NitroIL into){
			if(ElementType==null){
				Error("Unused index operation");
			}
			AppliedTo.OutputIL(into);
			Index.OutputIL(into);
		}
		
		public void OutputSet(NitroIL into,Type setting){
			if(ElementType.IsValueType){
				into.Emit(OpCodes.Stelem,ElementType);
			}else{
				into.Emit(OpCodes.Stelem_Ref);
			}
		}
		
		public override void OutputIL(NitroIL into){
			OutputTarget(into);
			if(ElementType.IsValueType){
				into.Emit(OpCodes.Ldelem,ElementType);
			}else{
				into.Emit(OpCodes.Ldelem_Ref);
			}
		}
		
	}
	
}