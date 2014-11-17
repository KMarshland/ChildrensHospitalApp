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
	/// Represents a method/function that is currently being compiled.
	/// </summary>

	public class CompiledMethod{
		
		/// <summary>The line the method starts on.</summary>
		public int Line;
		/// <summary>The name of the method.</summary>
		public string Name;
		/// <summary>The script that this method belongs to.</summary>
		public NitroCode Script;
		/// <summary>The IL stream for this method. This is what will make this method executable.</summary>
		public NitroIL ILStream;
		/// <summary>The location at the end of the method.</summary>
		public Label EndOfMethod;
		/// <summary>True if any variables in this method should be declared on the class.</summary>
		public bool GloballyScoped;
		/// <summary>The class that this method belongs to.</summary>
		public CompiledClass Parent;
		/// <summary>True if the parameter types have been parsed.</summary>
		public bool ParametersLoaded;
		/// <summary>A method builder which provides the ILGenerator.</summary>
		public MethodBuilder Builder;
		/// <summary>The types of each parameter that this method accepts.</summary>
		public Type[] ParameterTypes;
		/// <summary>A location that the return value is placed into.</summary>
		public LocalBuilder ReturnBay;
		/// <summary>The block of code that represents the body of this method.</summary>
		public BracketFragment CodeBlock;
		/// <summary>The bracket fragment that contains the set of parameters.</summary>
		public BracketFragment ParameterBlock;
		/// <summary>Default values for the parameters, if any are provided.</summary>
		public CompiledFragment[] DefaultParameterValues;
		/// <summary>A list which acts like a stack of breakpoints for breaking from nested loops.</summary>
		public List<BreakPoint> BreakPoints=new List<BreakPoint>();
		/// <summary>A fast lookup of variable name to local variable.</summary>
		public Dictionary<string,LocalVariable> LocalSet=new Dictionary<string,LocalVariable>();
		/// <summary>A fast lookup of variable name to parameter variable.</summary>
		public Dictionary<string,ParameterVariable> ParameterSet=new Dictionary<string,ParameterVariable>();
		
		
		public CompiledMethod(string name){
			Name=name;
		}
		
		public CompiledMethod(CompiledClass parent,string name,BracketFragment parameterBlock,BracketFragment codeBlock,TypeFragment retType,int line,bool isPublic){
			Name=name;
			Line=line;
			Parent=parent;
			CodeBlock=codeBlock;
			Script=Parent.Script;
			ParameterBlock=parameterBlock;
			
			Type returnType=null;
			if(retType!=null){
				returnType=retType.FindType(Script);
				if(returnType==null){
					Error("Type '"+retType.Value+"' was not found.");
				}
			}
			string methodName=Name;
			MethodAttributes attrib=isPublic?MethodAttributes.Public:MethodAttributes.Private;
			if(methodName=="new"){
				methodName=".ctor";
				attrib|=MethodAttributes.HideBySig|MethodAttributes.SpecialName|MethodAttributes.RTSpecialName;
			}
			// Does the parent base type define this method?
			// If so, use it's name.
			Type baseType=Parent.Builder.BaseType;
			Type[] pSet=null;
			if(ParameterBlock!=null){
				pSet=new Type[ParameterBlock.ChildCount()];
			}
			MethodInfo mInfo=Types.GetOverload(baseType.GetMethods(),Name,pSet,true);
			if(mInfo!=null){
				methodName=mInfo.Name;
				attrib|=MethodAttributes.Virtual|MethodAttributes.HideBySig;//|MethodAttributes.NewSlot;
			}
			bool isVoid=Types.IsVoid(returnType);
			if(isVoid){
				returnType=typeof(void);
			}
			Builder=Parent.Builder.DefineMethod(
				methodName,
				attrib,
				returnType,
				null
			);
			ILStream=new NitroIL(Builder.GetILGenerator());
			EndOfMethod=ILStream.DefineLabel();
			if(!isVoid){
				ReturnBay=ILStream.DeclareLocal(returnType);
			}
		}
		
		/// <summary>Adds a breakpoint to the set of breakpoints for this method.</summary>
		/// <param name="bp">The breakpoint to add.</param>
		public void AddBreakPoint(BreakPoint bp){
			BreakPoints.Add(bp);
		}
		
		/// <summary>Removes a breakpoint from the set of breakpoints for this method.</summary>
		public void PopBreakPoint(){
			BreakPoints.RemoveAt(BreakPoints.Count-1);
		}
		
		/// <summary>Adds a break into the given IL stream, breaking out of the given number of loops.</summary>
		/// <param name="into">The IL stream to emit the break into.</param>
		/// <param name="depth">The amount of loops to break from.
		/// Can be affected with e.g. break 2; for getting out of a loop in a loop.</param>
		/// <returns>True if it could locate the loop and added the command.</returns>
		public bool Break(NitroIL into,int depth){
			if(depth>BreakPoints.Count){
				return false;
			}
			BreakPoint point=BreakPoints[BreakPoints.Count-depth];
			point.Break(into);
			return true;
		}
		
		/// <summary>Adds a continue into the given IL stream, continuing at the given number of loops up.</summary>
		/// <param name="into">The IL stream to emit the continue into.</param>
		/// <param name="depth">The loop to continue in.
		/// Can be affected with e.g. continue 2; for continuing the outer loop of a loop in a loop.</param>
		/// <returns>True if it could locate the loop and added the command.</returns>
		public bool Continue(NitroIL into,int depth){
			if(depth>BreakPoints.Count){
				return false;
			}
			BreakPoint point=BreakPoints[BreakPoints.Count-depth];
			point.Continue(into);
			return true;
		}
		
		/// <summary>Called when this method is finished compiling this method.</summary>
		public void Done(){
			string parameterString="";
			#if ildebug
			if(ParameterTypes!=null){
				for(int i=0;i<ParameterTypes.Length;i++){
					Type type=ParameterTypes[i];
					if(i!=0){
						parameterString+=",";
					}
					
					if(type==null){
						parameterString+="[NULL]";
					}else{
						parameterString+=type.ToString();
					}
				}
			}
			#endif
			ILStream.Done(Name+"("+parameterString+") finished compiling.");
		}
		
		/// <summary>Throws an error that occured in the compilation of this method with the given message.</summary>
		/// <param name="message">A message to state why this error occured.</param>
		public void Error(string message){
			throw new CompilationException(Line,message);
		}
		
		/// <summary>Gets the return type of this method.</summary>
		/// <returns>The return type of this method.</returns>
		public Type ReturnType(){
			return Builder.ReturnType;
		}
		
		/// <summary>Gets the MethodInfo for this method.</summary>
		/// <returns>The MethodInfo for this method.</returns>
		public MethodInfo getMethodInfo(){
			return (MethodInfo)Builder;
		}
		
		/// <summary>Attempts to find the parameter by the given name.</summary>
		/// <param name="variableName">The name of the parameter to find.</param>
		/// <returns>The parameter, if found. Null otherwise.</returns>
		public ParameterVariable GetParameter(string variableName){
			ParameterVariable result=null;
			ParameterSet.TryGetValue(variableName,out result);
			return result;
		}
		
		/// <summary>Attempts to find the local variable by the given name.</summary>
		/// <param name="variableName">The name of the local variable to find.</param>
		/// <returns>The variable, if found. Null otherwise.</returns>
		public LocalVariable GetLocal(string variableName){
			return GetLocal(variableName,false,null);
		}
		
		/// <summary>Attempts to find the local by the given name and optionally creates it if it's not found.</summary>
		/// <param name="variableName">The name of the local to find.</param>
		/// <param name="create">True if it should be created if not found.</param>
		/// <param name="createType">If create is true, the type of the variable to create. If it's null, object is assumed.</param>
		/// <returns>The local, if found or created. Null otherwise.</returns>
		public LocalVariable GetLocal(string variableName,bool create,Type createType){
			LocalVariable result=null;
			if(!LocalSet.TryGetValue(variableName,out result)&&create){
				if(createType==null){
					createType=typeof(object);
				}
				result=new LocalVariable(variableName,ILStream.DeclareLocal(createType));
				LocalSet.Add(variableName,result);
			}
			return result;
		}
		
		/// <summary>Attempts to get either a local variable or a parameter by the given name.</summary>
		/// <param name="name">The name of the variable to find.</param>
		/// <returns>The variable, if found. Null otherwise.</returns>
		public Variable GetVariable(string name){
			return GetVariable(name,false,null);
		}
		
		/// <summary>Attempts to get either a local variable or a parameter by the given name, creating it if not found.</summary>
		/// <param name="name">The name of the variable to find.</param>
		/// <param name="create">True if it should be created if not found. Will always make a local.</param>
		/// <param name="createType">If create is true, the type of the variable to create. If it's null, object is assumed.</param>
		/// <returns>The variable, if found. Null otherwise.</returns>
		public Variable GetVariable(string name,bool create,Type createType){
			return GetVariable(name,VariableType.Any,create,createType);
		}
		
		/// <summary>Attempts to get either a variable of the given type by the given name, creating it if not found.</summary>
		/// <param name="name">The name of the variable to find.</param>
		/// <param name="type">The type of the variable to find. Local, Parameter or Any.</param>
		/// <param name="create">True if it should be created if not found. Will always make a local.</param>
		/// <param name="createType">If create is true, the type of the variable to create. If it's null, object is assumed.</param>
		/// <returns>The variable, if found. Null otherwise.</returns>
		public Variable GetVariable(string name,VariableType type,bool create,Type createType){
			// What type of variable is it? local, param, new, global etc. 
			// Could be a method call or a property of something.
			// Priority order:
			// Local, Parameter, Class variable [force top priority with 'this'].
			if(string.IsNullOrEmpty(name)){
				return null;
			}
			
			Variable result=null;
			// Is it a local?
			if(type==VariableType.Any||type==VariableType.Local){
				result=GetLocal(name,create,createType);
				if(result!=null){
					return result;
				}
			}
			// Is it a parameter?
			if(type==VariableType.Any||type==VariableType.Parameter){
				result=GetParameter(name);
				if(result!=null){
					return result;
				}
			}
			return null;
		}
		
		/// <summary>Confirms that the recently loaded parameters are valid.</summary>
		private void ParametersOk(){
			ParametersLoaded=true;
			Parent.FindMethodSet(Name).ParametersOk(this);
		}
		
		/// <summary>Loads the parameter block into a set of types.</summary>
		public void ParseParameters(){
			if(DefaultParameterValues!=null){
				DefaultParameterValues=null;
				Builder.SetParameters(null);
			}
			
			if(ParameterBlock==null||!ParameterBlock.IsParent()){
				// No inputs anyway, e.g. test(){..}
				ParametersOk();
				return;
			}
			
			// Each of blocks children is an operation segment.
			
			ParameterTypes=new Type[ParameterBlock.ChildCount()];
			DefaultParameterValues=new CompiledFragment[ParameterTypes.Length];
			int index=0;
			// For each parameter..
			CodeFragment current=ParameterBlock.FirstChild;
			while(current!=null){
				if(!current.IsParent()){
					Error("Invalid function definition input variable found.");
				}
				// Default value of this variable (if any). E.g. var1=true,var2..
				CompiledFragment DefaultValue=null;
				
				CodeFragment inputName=current.FirstChild;
				if(inputName.GetType()!=typeof(VariableFragment)){
					Error("Invalid function definition inputs for "+Name+". Must be (var1:type,var2:type[=a default value],var3:type..). [=..] is optional and can be used for any of the variables.");
				}
				
				string paramName=((VariableFragment)inputName).Value;
				if(inputName.NextChild==null){
				}else if(inputName.NextChild!=null&&!Types.IsTypeOf(inputName.NextChild,typeof(OperatorFragment))){
					//no type OR default, or the block straight after isn't : or =
					Error("Invalid function parameters for "+Name+". '"+paramName+"' is missing a type or default value.");
				}else{
					OperatorFragment opFrag=(OperatorFragment)inputName.NextChild;
					//it must be a set otherwise it's invalid.
					if(opFrag.Value==null||opFrag.Value.GetType()!=typeof(OperatorSet)){
						Error("Invalid default function parameter value provided for "+Name+". '"+paramName+"'.");
					}
					current.FirstChild=inputName.NextChild.NextChild;
					if(!current.IsParent()){
						Error("Invalid default function definition. Must be (name:type=expr,..)");
					}
					DefaultValue=current.Compile(this);
				}
				if(inputName.GivenType!=null){
					ParameterTypes[index]=inputName.GivenType.FindType(Script);
				}else if(DefaultValue!=null){
					ParameterTypes[index]=DefaultValue.OutputType(out DefaultValue);
				}else{
					Error("Parameter "+paramName+"'s type isn't given. Should be e.g. ("+paramName+":String,..).");
				}
				DefaultParameterValues[index]=DefaultValue;
				
				if(ParameterSet.ContainsKey(paramName)){
					Error("Cant use the same parameter name twice. "+paramName+" in function "+Name+".");
				}else if(ParameterTypes[index]==null){
					Error("Type not given or invalid for parameter "+paramName+" in function "+Name+".");
				}else{
					ParameterSet.Add(paramName,new ParameterVariable(paramName,ParameterTypes[index]));
				}
				current=current.NextChild;
				index++;
			}
			// Write the Type[] block to the MethodBuilder:
			Builder.SetParameters(ParameterTypes);
			// Next, go over all the parameters generating a ParameterBuilder for each one:
			index=1;
			foreach(KeyValuePair<string,ParameterVariable> kvp in ParameterSet){
				kvp.Value.Builder=Builder.DefineParameter(index,ParameterAttributes.None,kvp.Key);
				index++;
			}
			ParametersOk();
		}
		
	}
	
}