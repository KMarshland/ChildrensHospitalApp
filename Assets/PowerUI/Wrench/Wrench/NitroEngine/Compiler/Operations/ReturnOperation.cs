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
	/// Represents a return (return 14) operation.
	/// </summary>
	
	public class ReturnOperation:Operation{
	
		public ReturnOperation(CompiledMethod method):base(method){}
		
		public override bool RequiresStoring{
			get{
				return false;
			}
		}
		
		public override Type OutputType(out CompiledFragment v){
			v=this;
			return Input0.OutputType(out Input0);
		}
		
		public override void OutputIL(NitroIL into){
			Type methodType=Method.ReturnType();
			if(Input0!=null){
				Type type=Input0.OutputType(out Input0);
				if(type==null){
					if(methodType.IsValueType){
						// We're returning null and the method returns a value type - this isn't allowed.
						Error("Can't return null here as the output type of the method is a value type.");
					}
				}else if(!methodType.IsAssignableFrom(type)){
					if(Types.IsVoid(methodType)){
						Error("This method cannot return anything (it's got no return type)");
					}else{
						Error("Must return something of type "+methodType+" (the methods return type)");
					}
				}
				Input0.OutputIL(into);
			}else if(!Types.IsVoid(methodType)){
				Error("Must return a value of type "+methodType);
			}
			if(Method.ReturnBay!=null){
				into.Emit(OpCodes.Stloc,Method.ReturnBay);
			}
			into.Emit(OpCodes.Br,Method.EndOfMethod);
		}
		
	}
	
}