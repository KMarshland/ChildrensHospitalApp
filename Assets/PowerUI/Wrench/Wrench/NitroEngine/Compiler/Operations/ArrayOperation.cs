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
	/// Represents the array construct (new array[]{default,values};, new array[](length);) operation.
	/// </summary>
	
	public class ArrayOperation:Operation{
		
		/// <summary>The type of the array, e.g. string[].</summary>
		public Type ArrayType;
		/// <summary>A constant value for the size/length of this array. Doesn't require defaults.</summary>
		public int DirectSize;
		/// <summary>The size/length this array will be. Doesn't require defaults.</summary>
		public CompiledFragment Size;
		/// <summary>The set of default/ initial values for this array instance. Doesn't require Size.</summary>
		public CompiledFragment[] DefaultValues;
		
		public ArrayOperation(CompiledMethod method,Type arrayType,CompiledFragment size,CompiledFragment[] defaultValues):base(method){
			Size=size;
			SetDefaults(defaultValues);
			ArrayType=arrayType;
		}
		
		public ArrayOperation(CompiledMethod method,Type elementType,CompiledFragment[] defaultValues):base(method){
			SetDefaults(defaultValues);
			ArrayType=elementType.MakeArrayType();
		}
		
		/// <summary>Sets the default/ intial values for this array instance.</summary>
		/// <param name="defaultValues">The set of default values to use.</param>
		private void SetDefaults(CompiledFragment[] defaultValues){
			if(defaultValues!=null&&defaultValues.Length==0){
				defaultValues=null;
			}
			DefaultValues=defaultValues;
			if(Size==null&&defaultValues!=null){
				DirectSize=defaultValues.Length;
			}
		}
		
		public override Type OutputType(out CompiledFragment newOperation){
			newOperation=this;
			if(Size!=null&&Size.OutputType(out Size)!=typeof(int)){
				Error("The size of a new array must be an integer.");
			}
			if(DefaultValues!=null){
				Type elementType=ArrayType.GetElementType();
				for(int i=0;i<DefaultValues.Length;i++){
					CompiledFragment p=DefaultValues[i];
					Type pType=p.OutputType(out p);
					if(pType==null){
						if(elementType.IsValueType){
							Error("Null cannot be used in an array of value types.");
						}
					}else if(!elementType.IsAssignableFrom(pType)){
						Error("A value defined in an array is not a type that will go into the array.");
					}
					DefaultValues[i]=p;
				}
			}
			return ArrayType;
		}
		
		public override void OutputIL(NitroIL into){
			if(Size!=null){
				Size.OutputIL(into);
			}else{
				into.Emit(OpCodes.Ldc_I4,DirectSize);
			}
			Type ElementType=ArrayType.GetElementType();
			into.Emit(OpCodes.Newarr,ElementType);
			
			
			if(DefaultValues==null || DefaultValues.Length==0){
				return;
			}
			LocalBuilder temp=into.DeclareLocal(ArrayType);
			into.Emit(OpCodes.Stloc,temp);
			// Emit a series of SET's into the array.
			for(int i=0;i<DefaultValues.Length;i++){
				into.Emit(OpCodes.Ldloc,temp);
				into.Emit(OpCodes.Ldc_I4,i);
				DefaultValues[i].OutputIL(into);
				if(ElementType.IsValueType){
					into.Emit(OpCodes.Stelem,ElementType);
				}else{
					into.Emit(OpCodes.Stelem_Ref);
				}
			}
			into.Emit(OpCodes.Ldloc,temp);
		}
		
	}
	
}