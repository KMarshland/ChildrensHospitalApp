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
	/// Represents special keywords in the code that must be handled differently, such as return or break.
	/// </summary>

	public static class KeyWords{
		
		/// <summary>Compiles a known keyword fragment into the given method.</summary>
		/// <param name="kwd">The known keyword fragment. return, using, break or continue.</param>
		/// <param name="method">The method to compile the fragment into.</param>
		/// <returns>A compiled instruction of the given keyword.</returns>
		public static CompiledFragment Compile(VariableFragment kwd,CompiledMethod method){
			OperationFragment parent=(OperationFragment)kwd.ParentFragment;
			if(kwd.Value=="return"){
				if(kwd.PreviousChild!=null){
					kwd.Error("Return cannot follow other operations. You might have a missing ;.");
				}
				kwd.Remove();
				ReturnOperation returnOperation=new ReturnOperation(method);
				returnOperation.LineNumber=parent.LineNumber;
				if(parent.FirstChild!=null){
					if((returnOperation.Input0=parent.Compile(method))==null){
						return null;
					}
				}
				parent.FirstChild=returnOperation;
				return returnOperation;
			}else if(kwd.Value=="using"){
				if(kwd.PreviousChild!=null){
					kwd.Error("Using cannot follow other operations. You might be missing a ;.");
				}
				kwd.Remove();
				if(parent.FirstChild!=null){
					method.Script.AddReference(parent.FirstChild);
				}
				return null;
			}else if(kwd.Value=="break"){
				kwd.Remove();
				BreakOperation breakOperation=new BreakOperation(method,1);
				breakOperation.LineNumber=parent.LineNumber;
				if(parent.FirstChild!=null){
					CompiledFragment cf=parent.Compile(method);
					if(cf.Value!=null&&cf.Value.GetType()==typeof(int)){
						breakOperation.Depth=(int)cf.Value;
						if(breakOperation.Depth<=0){
							parent.Error("Break operations must be greater than 0 loops.");
						}
					}else{
						parent.Error("Break statements can only be followed by a fixed integer constant, e.g. break 2; or just break;");
					}
				}
				parent.FirstChild=breakOperation;
				return breakOperation;
			}else{
				kwd.Remove();
				ContinueOperation cOperation=new ContinueOperation(method,1);
				cOperation.LineNumber=parent.LineNumber;
				if(parent.FirstChild!=null){
					CompiledFragment cf=parent.Compile(method);
					if(cf.Value!=null&&cf.Value.GetType()==typeof(int)){
						cOperation.Depth=(int)cf.Value;
						if(cOperation.Depth<=0){
							parent.Error("Continue operations must be greater than 0 loops.");
						}
					}else{
						parent.Error("Continue statements can only be followed by a fixed integer constant, e.g. continue 2; or just continue;");
					}
				}
				parent.FirstChild=cOperation;
				return cOperation;
			}
		}
		
	}
	
}