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
	/// Represents the a property get/set (Of.property) operation.
	/// </summary>
	
	public class PropertyOperation:Operation,ISettable{
		
		/// <summary>The name of the property. Always lowercase.</summary>
		public string Name;
		/// <summary>True if this property is a static one.</summary>
		public bool IsStatic;
		/// <summary>If this is a field, the FieldInfo that represents it.</summary>
		public FieldInfo Field;
		/// <summary>The fragment/object this is a property of. "Of.property".</summary>
		public CompiledFragment Of;
		/// <summary>If this is a property, the PropertyInfo that represents it.</summary>
		public PropertyInfo Property;
		/// <summary>If this is a method, the type it returns.</summary>
		public Type MethodReturnType;
		
		
		public PropertyOperation(CompiledMethod method,string name):this(method,new ThisOperation(method),name){}
		
		public PropertyOperation(CompiledMethod method,CompiledFragment of,string name):base(method){
			if(of!=null){
				of.ParentFragment=this;
			}
			Of=(of!=null)?of:new ThisOperation(method);
			Name=name;	
		}
		
		public override bool IsMemberAccessor(){
			return true;
		}
		
		/// <summary>Gets the type of the object that this is a property of.</summary>
		public Type OfType(){
			Type type=Of.OutputType(out Of);
			Of.ParentFragment=this;
			// Static if the object this is a property of is a type.
			IsStatic=(Of.GetType()==typeof(TypeOperation));
			
			if(type==null){
				Error("Unable to determine type of something. This is required to access the property '"+Name+"' on it.");
			}
			
			if(IsStatic){
				return ((TypeOperation)Of).TypeObject;
			}
			
			return type;
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			
			Type type=OfType();
			
			// Map to functionality:
			CompiledClass Class=null;
			bool isDynamic=Types.IsDynamic(type);
			
			if(!isDynamic){
				if(!Method.Script.AllowUse(type)){
					Error("Unable to access properties of type "+type+" as it has not been made accessible.");
				}
			}else{
				Class=Method.Script.GetClass(type);
			}
			
			if(isDynamic){
				Field=Class.GetField(Name);
			}else{
				Field=type.GetField(Name,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			}
			
			if(Field!=null){
				if(IsStatic&&!Field.IsStatic){
					Error("Property "+Name+" is not static. You must use an object reference to access it.");
				}
				
				return Field.FieldType;
			}
			
			if(isDynamic){
				Property=Class.GetProperty(Name);
			}else{
				Property=type.GetProperty(Name,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			}
			
			if(Property!=null){
				if(IsStatic){
					MethodInfo staticTest=Property.GetGetMethod();
					if(staticTest==null){
						staticTest=Property.GetSetMethod();
					}
					if(!staticTest.IsStatic){
						Error("Property "+Name+" is not static. You must use an object reference to access it.");
					}
				}
				return Property.PropertyType;
			}
			
			if(isDynamic){
				MethodReturnType=Class.MethodReturnType(Name);
			}else{
				MethodReturnType=Types.MethodReturnType(type,Name);
			}
			
			if(MethodReturnType!=null){
				if(Types.IsVoid(MethodReturnType)){
					MethodReturnType=typeof(Void);
				}
				return DynamicMethodCompiler.TypeFor(MethodReturnType);
			}
			
			if(Of.GetType()==typeof(ThisOperation)){
				// This was the first property - it can potentially be a static type name too.
				Type staticType=Method.Script.GetType(Name);
				if(staticType!=null){
					// It's a static type! Generate a new type operation to replace this one and return the type.
					v=new TypeOperation(Method,staticType);
					return v.OutputType(out v);
				}
			}
			
			if(Name=="this"){
				// This is handled here as it allows variables called "This". Use case: PowerUI.
				v=new ThisOperation(Method);
				return v.OutputType(out v);
			}
			
			// Does it support indexing? If so, Do Parent["property"] instead.
			
			MethodOperation mOp=null;
			
			if(Input0!=null){
				// This is a set. Input0 is the object we're setting.
				
				Type setType=Input0.OutputType(out Input0);
				
				// Get the set method:
				MethodInfo mInfo;
				
				if(isDynamic){
					mInfo=Class.FindMethodOverload("set_Item",new Type[]{typeof(string),setType});
				}else{
					// Grab all the methods of the type:
					MethodInfo[] allMethods=type.GetMethods();
					mInfo=Types.GetOverload(allMethods,"set_Item",new Type[]{typeof(string),setType});
				}
				
				if(mInfo==null){
					// It doesn't exist!
					Error("Property '"+ToString()+"' is not a property of "+type.ToString()+".");
				}
				
				// It exists - create the method operation now.
				mOp=new MethodOperation(Method,mInfo,new CompiledFragment(Name),Input0);
				v=mOp;
				mOp.CalledOn=Of;
				return setType;
			}else{
				// Get.
				
				// Get the get method:
				MethodInfo mInfo;
				
				if(isDynamic){
					mInfo=Class.FindMethodOverload("get_Item",new Type[]{typeof(string)});
				}else{
					// Grab all the methods of the type:
					MethodInfo[] allMethods=type.GetMethods();
					mInfo=Types.GetOverload(allMethods,"get_Item",new Type[]{typeof(string)});
				}
				
				
				if(mInfo==null){
					// It doesn't exist!
					Error("'"+ToString()+"' is not a property of "+type.ToString()+".");
				}
				
				// It exists - create the method operation now:
				mOp=new MethodOperation(Method,mInfo,new CompiledFragment(Name));
				v=mOp;
				mOp.CalledOn=Of;
				return mInfo.ReturnType;
			}
			
		}
		
		/// <summary>If this is a method, this attempts to find the correct overload
		/// by using the set of arguments given in the method call.</summary>
		/// <param name="arguments">The set of arguments given in the method call.</param>
		/// <returns>The MethodInfo if found; null otherwise.</returns>
		public MethodInfo GetOverload(CompiledFragment[] arguments){
			Type fragType=OfType();
			
			if(Types.IsDynamic(fragType)){
				CompiledClass cc=Method.Script.GetClass(fragType);
				return (cc==null)?null:cc.FindMethodOverload(Name,arguments);
			}else if(Name=="gettype"&&(arguments==null||arguments.Length==0)){
				return fragType.GetMethod("GetType",new Type[0]);
			}else{
				if(!Method.Script.AllowUse(fragType)){
					Error("Unable to call methods on type "+fragType.Name+" as it is restricted.");
				}
				Type[] paramTypes=Types.GetTypes(arguments);
				MethodInfo result=Types.GetOverload(fragType.GetMethods(),Name,paramTypes,true);
				
				if(IsStatic && result!=null && !result.IsStatic){
					// Special case! This is where we're calling e.g. ToString on a straight type, for example int.ToString();
					// Another example is actually the call below! We're getting a type, then calling a method on the type - not a static method of it.
					// The method is not static yet we're 'expecting' one.
					// So, look for the same method on System.Type and return that instead.
					return Types.GetOverload(typeof(System.Type).GetMethods(),Name,paramTypes,true);
				}
				
				return result;
			}
		}
		
		public void OutputTarget(NitroIL into){
			
			if(IsStatic&&(Field!=null || Property!=null)){
				return;
			}
			
			if(Of==null||(Field==null&&Property==null&&MethodReturnType==null)){
				Error("Unused or invalid property.");
			}
			
			if(MethodReturnType==null){
				Of.OutputIL(into);
			}
			
		}
		
		public void OutputSet(NitroIL into,Type setting){
			
			if(Field!=null){
				
				if(IsStatic){
					into.Emit(OpCodes.Stsfld,Field);
				}else{
					into.Emit(OpCodes.Stfld,Field);
				}
				
			}else if(Property!=null){
				bool useVirtual=!IsStatic && !Property.PropertyType.IsValueType;
				MethodInfo setMethod=Property.GetSetMethod();
				
				if(setMethod==null){
					Error(Name+" is a readonly property.");
				}
				
				into.Emit(useVirtual?OpCodes.Callvirt:OpCodes.Call,setMethod);
			}else{
				Error(Name+" is a function! Unable to set it's value.");
			}
			
		}
		
		public override void OutputIL(NitroIL into){
			OutputTarget(into);
			if(Field!=null){
				if(Field.IsLiteral){
					// Special case - this field isn't actually a field at all!
					// Load it's literal value:
					object literalFieldValue=Field.GetValue(null);
					
					// Get ready to write out the literal value:
					CompiledFragment literalValue=new CompiledFragment(literalFieldValue);
					
					// It might even be from an enum - let's check:
					if(Field.FieldType.IsEnum){
						// Ok great it's from an enum. What type is it?
						Type literalValueType=Enum.GetUnderlyingType(Field.FieldType);
						
						// Use that type to emit the value:
						literalValue.EmitValue(literalValueType,into);
					}else{
						literalValue.OutputIL(into);
					}
				}else if(ParentFragment!=null && ParentFragment.IsMemberAccessor() && Field.FieldType.IsValueType){
					// Are we followed by another PropertyOperation?
					// A following operation in this sitation ends up being the parent.
					// If we are, and we output a valuetype, Ldflda must be used.
					
					if(IsStatic){
						into.Emit(OpCodes.Ldsflda,Field);
					}else{
						into.Emit(OpCodes.Ldflda,Field);
					}
					
				}else if(IsStatic){
					into.Emit(OpCodes.Ldsfld,Field);
				}else{
					into.Emit(OpCodes.Ldfld,Field);
				}
			}else if(Property!=null){
				bool useVirtual=!IsStatic && !Property.PropertyType.IsValueType;
				into.Emit(useVirtual?OpCodes.Callvirt:OpCodes.Call,Property.GetGetMethod());
			}else{
				DynamicMethodCompiler.Compile(Method,Name,MethodReturnType,Of).OutputIL(into);
			}
		}
		
		public override bool EmitsAddress{
			get{
				return (Field!=null && !Field.IsLiteral);
			}
		}
		
		public override string ToString(){
			string result="";
			if(Of!=null){
				result=Of.ToString();
			}
			if(result!=""){
				result+=".";
			}
			result+=Name;
			return result;
		}
		
	}
	
}