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
	/// Represents a code fragment that has been compiled and can now be written out as IL.
	/// </summary>

	public class CompiledFragment:CodeFragment{
		
		/// <summary>A constant value if this fragment has one. E.g. strings, ints etc. go here.</summary>
		public object Value;
		
		
		/// <summary>An empty constructor used for inheriting this class.</summary>
		public CompiledFragment(){}
		
		/// <summary>Gets the active value of this fragment.</summary>
		/// <returns>If this fragment is a constant, the active value is the constant itself.
		/// Otherwise, the active value is the actual fragment.</returns>
		public object ActiveValue(){
			if(Value!=null){
				return Value;
			}
			return this;
		}
		
		/// <summary>Creates a new compiled fragment for the given constant value.</summary>
		/// <param name="value">The constant value for this fragment.</param>
		public CompiledFragment(object value){
			Value=value;
		}
		
		/// <summary>Checks if this fragment is a constant value.</summary>
		/// <returns>True if it's a constant; false otherwise.</returns>
		public virtual bool IsConstant(){
			return true;
		}
		
		/// <summary>Checks if this fragment can be used logically.</summary>
		/// <returns>True if this operation returns a boolean.</returns>
		public bool IsLogical(){
			return (OutputType()==typeof(bool));
		}
		
		/// <summary>Returns the type that this fragment produces when it's executed. For example, int+int will output another int.
		/// This must only be used when it's known that a fragment won't replace itself.</summary>
		/// <returns>The type that this fragment returns.</returns>
		public Type OutputType(){
			CompiledFragment v=null;
			return OutputType(out v);
		}
		
		/// <summary>Returns the type that this fragment produces when it's executed.
		/// For example, int+int will output another int.</summary>
		/// <param name="v">In some cases, checking the output type will generate a new fragment. This value is the new fragment
		/// and it must always replace the original fragment that OutputType was called on.</param>
		/// <returns>The type that this fragment returns.</returns>
		public virtual Type OutputType(out CompiledFragment v){
			v=this;
			if(Value==null){
				return null;
			}
			Type type=Value.GetType();
			if(Types.IsTypeOf(Value,typeof(Variable))){
				type=((Variable)Value).Type();
			}
			return type;
		}
		
		/// <summary>Compiles this CodeFragment into a CompiledFragment.</summary>
		/// <param name="method">The method that this fragment occurs in.</param>
		/// <returns>A CompiledFragment is already compiled, so it returns itself. This is called in nested operations.</returns>
		public override CompiledFragment Compile(CompiledMethod method){
			return this;
		}
		
		/// <summary>Generates the IL of this operation into the given stream.</summary>
		/// <param name="into">The IL stream to output the IL into</param>
		public virtual void OutputIL(NitroIL into){
			if(Value==null){
				into.Emit(OpCodes.Ldnull);
				return;
			}
			// Output the variable that represents the result. Note: used by operations which are variables too.
			EmitValue(Value.GetType(),into);
		}
		
		/// <summary>Emits the Value held by this compiled fragment if possible.</summary>
		/// <param name="type">The type of the value (or one of the base types of the value).</param>
		/// <param name="into">The IL stream to output the IL into.</param>
		public void EmitValue(Type type,NitroIL into){
			if(type==typeof(string)){
				into.Emit(OpCodes.Ldstr,(string)Value);
			}else if(type==typeof(int)){
				into.Emit(OpCodes.Ldc_I4,(int)Value);
			}else if(type==typeof(uint)){
				into.Emit(OpCodes.Ldc_I4,(uint)Value);
			}else if(type==typeof(long)){
				into.Emit(OpCodes.Ldc_I8,(long)Value);
			}else if(type==typeof(ulong)){
				into.Emit(OpCodes.Ldc_I8,(ulong)Value);
			}else if(type==typeof(float)){
				into.Emit(OpCodes.Ldc_R4,(float)Value);
			}else if(type==typeof(double)){
				into.Emit(OpCodes.Ldc_R8,(double)Value);
			}else if(type==typeof(OpCode)){
				into.Emit((OpCode)Value);
			}else if(type==typeof(bool)){
				into.Emit(OpCodes.Ldc_I4,((bool)Value)?1:0);
			}else if(type==typeof(short)){
				into.Emit(OpCodes.Ldc_I4,(short)Value);
			}else if(type==typeof(ushort)){
				into.Emit(OpCodes.Ldc_I4,(ushort)Value);
			}else if(type==typeof(byte)){
				into.Emit(OpCodes.Ldc_I4,(byte)Value);
			}else if(type==typeof(sbyte)){
				into.Emit(OpCodes.Ldc_I4,(sbyte)Value);
			}else if(Types.IsSubclass(Value,typeof(Variable))){
				// Is parent a type of methodoperation or propertyoperation?
				// If it is, IsMemberAccessor is true.
				((Variable)Value).OutputIL(into, (ParentFragment!=null && ParentFragment.IsMemberAccessor()) );
			}else{
				Error("Didn't know how to output value "+Value+" (type is "+type+")");
			}
		}
		
	}
	
}