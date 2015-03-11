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
	/// Represents a set (A=B) operation.
	/// </summary>
	
	public class SetOperation:Operation{
	
		/// <summary>True if the output of the set is used for dolly-chaining sets. E.g. A=B=C.</summary>
		public bool Output=false;
		
		public SetOperation(CompiledMethod method,CompiledFragment input0,CompiledFragment input1):base(method){
			Input0=input0;
			Input1=input1;
		}
		
		public override bool RequiresStoring{
			get{
				return false;
			}
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			Output=true;
			return Input0.OutputType(out Input0);
		}
		
		/// <summary>Looks for the use of the given variable within this operation.</summary>
		/// <param name="cfrag">The fragment to look in, e.g. this.Input1.</param>
		/// <param name="findingVar">The variable being searched for.</param>
		/// <returns>True if it was found in there, false otherwise.</returns>
		public bool LookFor(CompiledFragment cfrag,Variable findingVar){
			if(cfrag==null){
				return false;
			}
			
			if(cfrag.Value==null&&Types.IsTypeOf(cfrag,typeof(Operation))){
				Operation op=(Operation)cfrag;
				
				if(LookFor(op.Input0,findingVar)){
					return true;
				}
				
				if(LookFor(op.Input1,findingVar)){
					return true;
				}
				
			}else if(cfrag.Value!=null&&Types.IsTypeOf(cfrag.Value,typeof(Variable))){
				return ((Variable)cfrag.Value).Equals(findingVar);
			}
			
			return false;
		}
		
		/// <summary>Is the object in I0 mentioned anywhere in I1? Used to distinguish incrementals from straight sets.</summary>
		/// <returns>True if it is, false otherwise.</returns>
		public bool SelfReferencing(){
			
			if(Input1!=null&&Input0.Value!=null&&Types.IsTypeOf(Input0.Value,typeof(Variable))){
				Variable var=(Variable)Input0.Value;
				
				return LookFor(Input1,var);
			}
			
			return false;
		}
		
		public override void OutputIL(NitroIL into){
			
			Type type2=Input1.OutputType(out Input1);
			
			// Is it something which is being ["indexed"]? May apply to properties too.
			bool indexOperation=(Input0.GetType()==typeof(IndexOperation));
			bool propertyOperation=(Input0.GetType()==typeof(PropertyOperation));
			
			if(indexOperation || propertyOperation){
				// Hook up what we will be setting for the index to handle if it needs to.
				// Note that the object will not change as we have already run it's OutputType call above.
				((Operation)Input0).Input0=Input1;
			}
			
			// Update input0 by computing the type it outputs:
			Type type1=Input0.OutputType(out Input0);
			
			object value=Input0.ActiveValue();
			
			if(value.GetType()!=typeof(LocalVariable)){
				
				// Local vars can change type so this doesn't affect them.
				
				if(type1==null){
					Error("Can't set to nothing.");
				}
				
				if(type2==null){
					
					if(type1.IsValueType){
						Error("Can't set "+type1+" to null because it's a value type.");
					}
					
				}else if(!type1.IsAssignableFrom(type2)){
					Error("Can't implicity convert "+type2+" to "+type1+".");
				}
				
			}
			
			if(Types.IsTypeOf(value,typeof(ISettable))){
				ISettable Value=(ISettable)value;
				
				Value.OutputTarget(into);
				Input1.OutputIL(into);
				Value.OutputSet(into,type2);
				
				if(Output){
					// Daisy chaining these sets.
					Input0.OutputIL(into);
				}
				
			}else if(indexOperation && value.GetType()==typeof(MethodOperation)){
				// This is ok! We've called something like obj["hello"]=input1;
				// Just output the method call:
				Input0.OutputIL(into);
				
				if(Output){
					Error("Can't daisy chain with an indexer. Place your indexed object furthest left.");
				}
				
			}else if(propertyOperation && value.GetType()==typeof(MethodOperation) && ((MethodOperation)value).MethodName=="set_Item"){
				// This is also ok - we've done something like object.property=value; and it mapped to object["property"]=value;
				// Just output the method call:
				Input0.OutputIL(into);
				
				if(Output){
					Error("Can't daisy chain with a property set here. Place your indexed object furthest left.");
				}
				
			}else{
				Error("Attempted to set to something (a "+value.GetType()+") that isn't a variable.");
			}
			
		}
		
	}
	
}