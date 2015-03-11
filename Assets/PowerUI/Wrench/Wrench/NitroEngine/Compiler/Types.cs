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
using System.Collections;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Nitro{

	/// <summary>
	/// Global methods that relate to handling variable types and changing/ casting between them.
	/// </summary>

	public static class Types{
		
		/// <summary>True if the given type is a numeric one (e.g. a float, int etc).</summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if it is numeric, false otherwise.</returns>
		public static bool IsNumeric(Type type){
			if(type==null){
				return false;
			}
			if(type.IsArray){
				return false;
			}
			switch((int)Type.GetTypeCode(type)){
				case 3:
				case 6:
				case 7:
				case 9:
				case 11:
				case 13:
				case 14:
				case 15:
				return true;
			};
			return false;
		}
		
		/// <summary>Gets the Run method on the given type.</summary>
		/// <param name="type">The type to get the run method for.</param>
		/// <returns>The MethodInfo for the Run method if found; Null otherwise.</returns>
		public static MethodInfo GetCallable(Type type){
			if(type==null||!type.IsGenericType||type.GetGenericTypeDefinition()!=typeof(DynamicMethod<>)){
				return null;
			}
			return type.GetMethod("Run");
		}
		
		/// <summary>Gets the constructor overload for the given type and using the given arguments.</summary>
		/// <param name="type">The type to get the constructor from.</param>
		/// <param name="args">The set of arguments being passed to the constructor. Used to find the right overload.</param>
		/// <returns>A ConstructorInfo if a matching constructor overload was found; null otherwise.</returns>
		public static ConstructorInfo GetConstructor(Type type,CompiledFragment[] args){
			ConstructorInfo[] constructors=type.GetConstructors();
			if(constructors==null){
				return null;
			}
			Type[] paramSet=GetTypes(args);
			ConstructorInfo constructor=GetConstructor(constructors,paramSet,false);
			if(constructor!=null){
				return constructor;
			}
			return GetConstructor(constructors,paramSet,true);
		}
		
		/// <summary>Searches a set of constructors for an overload that accepts the given types.</summary>
		/// <param name="constructors">The set of constructors to search through.</param>
		/// <param name="paramSet">The types of the arguments to match the overload with.</param>
		/// <param name="withCasting">True if casting should be used to help find a good match.</param>
		/// <returns>The matching ConstructorInfo if found; null otherwise.</returns>
		private static ConstructorInfo GetConstructor(ConstructorInfo[] constructors,Type[] paramSet,bool withCasting){
			
			int bestResult=0;
			ConstructorInfo bestSoFar=null;
			
			for(int i=constructors.Length-1;i>=0;i--){
				ConstructorInfo cInfo=constructors[i];
				
				int result=WillAccept(paramSet,cInfo.GetParameters(),withCasting);
				if(result==-1){
					continue;
				}else if(result==0){
					return cInfo;
				}
				if(bestSoFar==null || result<bestResult){
					bestSoFar=cInfo;
					bestResult=result;
				}
			}
			
			return bestSoFar;
		}
		
		/// <summary>Converts the given set of parameters into a string of name:Type, seperated by commas.</summary>
		/// <param name="set">The set of parameters to turn into a string.</param>
		/// <returns>The set as a string.</returns>
		public static string SetToString(ParameterInfo[] set){
			string result="";
			
			foreach(ParameterInfo info in set){
				if(result!=""){
					result+=",";
				}
				result+=info.Name+":"+info.ParameterType.Name;
			}
			
			return result;
		}
		
		/// <summary>Checks if the given ParameterInfo uses the params keyword.</summary>
		/// <param name="param">The parameter to check.</param>
		/// <returns>True if the parameter uses the params keyword; false otherwise.</returns>
		public static bool IsParams(ParameterInfo param){
			return Attribute.IsDefined(param,typeof(ParamArrayAttribute));
		}
		
		/// <summary>Checks if the given member is from a class being compiled at the moment.</summary>
		/// <param name="member">The member to check.</param>
		/// <returns>True if the member is from a dynamic type (one being compiled); false otherwise.</returns>
		public static bool IsDynamic(MemberInfo member){
			Type t=member.GetType();
			return (t==typeof(MethodBuilder)||t==typeof(FieldBuilder)||t==typeof(ConstructorBuilder)||t==typeof(TypeBuilder)||t==typeof(LocalBuilder)||t==typeof(PropertyBuilder)||t==typeof(ParameterBuilder));
		}
		
		/// <summary>Checks if the given parameter set will accept the given argument set. Used to find an overload.</summary>
		/// <param name="args">The set of arguments.</param>
		/// <param name="parameters">The set of parameters.</param>
		/// <param name="withCasting">True if casting is allowed to perform the match.</param>
		/// <returns>A number which represents how many casts must occur to accept (minimise when selecting an overload).
		/// -1 is returned if it cannot accept at all.</returns>
		public static int WillAccept(Type[] args,ParameterInfo[] parameters,bool withCasting){
			// We will consider accepting this if:
			// Args and parameters are the same length AND parameters does not end with params.
			int argCount=(args==null)?0:args.Length;
			int paramCount=(parameters==null)?0:parameters.Length;
			bool endsWithParams=(paramCount!=0 && IsParams(parameters[parameters.Length-1]));
			
			if(argCount==0&&paramCount==0){
				return 0;
			}
			
			if(!endsWithParams){
				// They must be the same length, otherwise fail it.
				if(argCount!=paramCount){
					return -1;
				}
			}else if(argCount<(paramCount-1)){
				// Not enough arguments.
				// E.g. we have 3 arguments and 4 parameters (where the last is 'params') - that's OK.
				// 2 args and 4 parameters - that isn't.
				return -1;
			}
			
			// Next, for each arg, find which parameter they fit into.
			// Args take priority because if there are no args, there must be no parameters OR one parameter with 'params'.
			
			// We're going to count how many times a cast is required.
			// This resulting value then acts as a weighting as to which overload may be selected.
			int castsRequired=0;
			
			int atParameter=0;
			for(int i=0;i<args.Length;i++){
				// Does args[i] fit in parameters[atParameter]?
				// If it doesn't, return false. 
				ParameterInfo parameter=parameters[atParameter];
				Type parameterType=parameter.ParameterType;
				if(endsWithParams && (atParameter==paramCount-1)){
					parameterType=parameterType.GetElementType();
				}else{
					atParameter++;
				}
				
				Type argumentType=args[i];
				if(argumentType==null){
					// Passing NULL. This is ok if parameterType is not a value type.
					if(parameterType.IsValueType){
						return -1;
					}
				}else{
					if(!parameterType.IsAssignableFrom(argumentType)){
						// Argument type is not equal to or does not inherit parameterType.
						if(!withCasting){
							return -1;
						}else{
							// Can we cast argumentType to parameterType?
							bool explicitCast;
							if(IsCastableTo(argumentType,parameterType,out explicitCast)==null){
								castsRequired++;
							}
						}
					}
				}
			}
			return castsRequired;
		}
		
		/// <summary>Checks if the given method doesn't return anything.</summary>
		/// <param name="methodInfo">The method to check.</param>
		/// <returns>True if the method has no return; false if it does.</returns>
		public static bool NoReturn(MethodInfo methodInfo){
			return IsVoid(methodInfo.ReturnType);
		}
		
		/// <summary>Checks if the given type is either a system or nitro void.</summary>
		/// <returns>True if it is, or if its null. False otherwise.</returns>
		public static bool IsVoid(Type T){
			return (T==null||T==typeof(void)||T==typeof(Void));
		}
		
		/// <summary>Outputs the given argument set into the given IL stream.</summary>
		/// <param name="args">The compiled set of arguments to be outputted.</param>
		/// <param name="method">The method that they are being used in.</param>
		/// <param name="into">The IL stream to output the arguments into.</param>
		/// <param name="parameters">The parameter info used to correctly match the location of the parameters.</param>
		public static void OutputParameters(CompiledFragment[] args,CompiledMethod method,NitroIL into,ParameterInfo[] parameters){
			int argID=0;
			int argCount=0;
			if(args!=null){
				argCount=args.Length;
			}
			for(int paramID=0;paramID<parameters.Length;paramID++){
				ParameterInfo param=parameters[paramID];
				Type paramType=param.ParameterType;
				
				if(IsParams(param)){
					// The rest are going into an array Operation.
					// Get the type we want to cast them all to (because paramType at this stage must be an array):
					paramType=paramType.GetElementType();
					
					CompiledFragment[] ops=new CompiledFragment[argCount-argID];
					int Index=0;
					for(int i=argID;i<argCount;i++){
						CompiledFragment frag=args[i];
						Type fragType=frag.OutputType(out frag);
						if(fragType!=paramType){
							frag=TryCast(method,frag,paramType);
							if(frag==null){
								args[i].Error("Unable to box or cast "+fragType+" to "+paramType+" at parameter "+argID+". Note that you can't null a value type.");
							}
						}
						ops[Index++]=frag;
					}
					CompiledFragment arrayOp=new ArrayOperation(method,paramType,ops);
					arrayOp.OutputType(out arrayOp);
					
					arrayOp.OutputIL(into);
					return;
				}
				
				CompiledFragment argFrag=args[argID++];
				Type argType=argFrag.OutputType(out argFrag);
				if(argType!=paramType){
					CompiledFragment originalFragment=argFrag;
					argFrag=TryCast(method,argFrag,paramType);
					if(argFrag==null){
						originalFragment.Error("Unable to box or cast "+argType+" to "+paramType+" at parameter "+argID+" of method call "+param.Member.Name+". Note that you can't null a value type.");
					}
				}
				
				argFrag.OutputIL(into);
			}
		}
		
		/// <summary>Gets the return type of a named method on the given type.
		/// Note that all overloads must return the same type in Nitro.</summary>
		/// <param name="type">The type to look for the method on.</param>
		/// <param name="name">The name of the method to look for.</param>
		/// <returns>The return type of the method, if it was found. Null otherwise.</returns>
		public static Type MethodReturnType(Type type,string name){
			MethodInfo[] overloads=type.GetMethods();
			for(int i=overloads.Length-1;i>=0;i--){
				MethodInfo mInfo=overloads[i];
				if(mInfo.Name.ToLower()!=name){
					continue;
				}
				return mInfo.ReturnType;
			}
			return null;
		}
		
		/// <summary>Gets the MethodInfo from the given set of methods which matches the given argument set and name.</summary>
		/// <param name="overloads">The set of overloads to find the right overload from.</param>
		/// <param name="name">The name of the method to find. Note that it is case sensitive.
		/// This is used if the method set is a full set of methods from a type.</param>
		/// <param name="argSet">The set of arguments to find a matching overload with.</param>
		/// <returns>The overload if found; null otherwise.</returns>
		public static MethodInfo GetOverload(MethodInfo[] overloads,string name,Type[] argSet){
			return GetOverload(overloads,name,argSet,false);
		}
		
		/// <summary>Gets the MethodInfo from the given set of methods which matches the given argument set and name.</summary>
		/// <param name="overloads">The set of overloads to find the right overload from.</param>
		/// <param name="name">The name of the method to find.
		/// This is used if the method set is a full set of methods from a type.</param>
		/// <param name="argSet">The set of arguments to find a matching overload with.</param>
		/// <param name="ignoreCase">Defines if the search should ignore case on the method names or not.</param>
		/// <returns>The overload if found; null otherwise.</returns>
		public static MethodInfo GetOverload(MethodInfo[] overloads,string name,Type[] argSet,bool ignoreCase){
			// Try without then with casting:
			MethodInfo result=GetOverload(overloads,name,argSet,ignoreCase,false);
			if(result!=null){
				return result;
			}
			// Not found, so try with casting:
			return GetOverload(overloads,name,argSet,ignoreCase,true);
		}
		
		/// <summary>Gets the MethodInfo from the given set of methods which matches the given argument set and name.</summary>
		/// <param name="overloads">The set of overloads to find the right overload from.</param>
		/// <param name="name">The name of the method to find.
		/// This is used if the method set is a full set of methods from a type.</param>
		/// <param name="argSet">The set of arguments to find a matching overload with.</param>
		/// <param name="ignoreCase">Defines if the search should ignore case on the method names or not.</param>
		/// <param name="cast">True if the search should allow casting when performing the match.</param>
		/// <returns>The overload if found; null otherwise.</returns>
		private static MethodInfo GetOverload(MethodInfo[] overloads,string name,Type[] argSet,bool ignoreCase,bool cast){
			if(overloads==null){
				return null;
			}
			
			int bestResult=0;
			MethodInfo bestSoFar=null;
			
			for(int i=overloads.Length-1;i>=0;i--){
				MethodInfo mInfo=overloads[i];
				string mName=mInfo.Name;
				if(ignoreCase){
					mName=mName.ToLower();
				}
				if(name!=null&&mName!=name){
					continue;
				}
				int result=WillAccept(argSet,mInfo.GetParameters(),cast);
				if(result==-1){
					continue;
				}else if(result==0){
					return mInfo;
				}
				if(bestSoFar==null || result<bestResult){
					bestSoFar=mInfo;
					bestResult=result;
				}
			}
			return bestSoFar;
		}
		
		/// <summary>Checks if A and B match. Note that types in A can derive (inherit) types in set B.</summary>
		/// <param name="a">The first set to match.</param>
		/// <param name="b">The second set to match.</param>
		/// <returns>True if A and B match; false otherwise.</returns>
		public static bool TypeSetsMatch(Type[] a,Type[] b){
			if(a==b){
				return true;
			}
			if(a==null){
				return (b.Length==0);
			}else if(b==null){
				return (a.Length==0);
			}
			if(a.Length!=b.Length){
				return false;
			}
			for(int i=a.Length-1;i>=0;i--){
				Type typeA=a[i];
				Type typeB=b[i];
				
				if(typeA==null){
					if(typeB.IsValueType){
						return false;
					}else{
						continue;
					}
				}
				if(!typeB.IsAssignableFrom(typeA)){
					return false;
				}
			}
			return true;
		}
		
		/// <summary>Checks if the given type sets match exactly with no inheritance allowed.</summary>
		/// <param name="a">The first set to match.</param>
		/// <param name="b">The second set to match.</param>
		/// <returns>True if A and B match; false otherwise.</returns>
		public static bool TypeSetsMatchExactly(Type[] a,Type[] b){
			if(a==b){
				return true;
			}
			if(a==null){
				return (b.Length==0);
			}else if(b==null){
				return (a.Length==0);
			}
			if(a.Length!=b.Length){
				return false;
			}
			for(int i=a.Length-1;i>=0;i--){
				if(b[i]!=a[i]){
					return false;
				}
			}
			return true;
		}
		
		/// <summary>Gets the ToString method of the given type, being called on the given fragment.</summary>
		/// <param name="method">The function this operation is occuring in.</param>
		/// <param name="frag">The object that the ToString method is being called on.</param>
		/// <param name="type">The type that fragment contains and the one that the ToString operation must be found on.</param>
		/// <returns>A methodOperation representing the ToString call. Throws an error if frag is null.</returns>
		public static MethodOperation ToStringMethod(CompiledMethod method,CompiledFragment frag,Type type){
			if(type==null){
				frag.Error("Unable to convert null to a string.");
			}
			MethodOperation mo=new MethodOperation(method,type.GetMethod("ToString",new Type[0]));
			mo.CalledOn=frag;
			frag.ParentFragment=mo;
			return mo;
		}
		
		/// <summary>Attempts to generate a cast operation for the given fragment to the given type.</summary>
		/// <param name="method">The method this operation is occuring in.</param>
		/// <param name="frag">The fragment containing the object to cast.</param>
		/// <param name="to">The type to cast it to if possible.</param>
		/// <returns>A cast operation if it is possible; throws an error otherwise.</returns>
		public static CompiledFragment TryCast(CompiledMethod method,CompiledFragment frag,Type to){
			bool isExplicit;
			return TryCast(method,frag,to,out isExplicit);
		}
		
		/// <summary>Attempts to generate a cast operation for the given fragment to the given type.</summary>
		/// <param name="method">The method this operation is occuring in.</param>
		/// <param name="frag">The fragment containing the object to cast.</param>
		/// <param name="to">The type to cast it to if possible.</param>
		/// <param name="isExplicit">True if the casting is explicit. False if it is implicit.</param>
		/// <returns>A cast operation if it is possible; throws an error otherwise.</returns>
		public static CompiledFragment TryCast(CompiledMethod method,CompiledFragment frag,Type to,out bool isExplicit){
			isExplicit=false;
			
			Type from=frag.OutputType(out frag);
			
			if(from==null){
				if(to.IsValueType){
					return null;
				}else{
					return frag;
				}
			}
			
			if(to==typeof(object) && from.IsValueType){
				return new BoxOperation(method,frag);
			}
			
			if(from==to||to.IsAssignableFrom(from)){
				return frag;
			}
			
			// IsAssignableFrom is true if TO inherits FROM:
			if(from.IsAssignableFrom(to)){
				return new CastOperation(method,frag,to);
			}
			
			MethodInfo cast=IsCastableTo(from,to,out isExplicit);
			
			if(cast==null){
				return null;
			}else{
				return new MethodOperation(method,cast,frag);
			}
		}
		
		/// <summary>Gets the name of a type without the namespace. E.g. System.Int32 becomes Int32.</summary>
		/// <param name="typeName">The full typename, including the namespace.</param>
		/// <returns>The type name without the namespace.</returns>
		public static string NameWithoutNamespace(string typeName){
			string[] pieces=typeName.Split('.');
			return pieces[pieces.Length-1];
		}
		
		/// <summary>Checks if one type is castable to another, reporting if it must be done explicitly or not if its possible.</summary>
		/// <param name="from">The type to cast from.</param>
		/// <param name="to">The type to cast to.</param>
		/// <param name="isExplicit">True if the cast must be done explicity.</param>
		/// <returns>The casting methods MethodInfo if a cast is possible at all; Null otherwise.</returns>
		public static MethodInfo IsCastableTo(Type from,Type to,out bool isExplicit){
			isExplicit=false;
			// Does convertible hold it?
			MethodInfo convertMethod=typeof(System.Convert).GetMethod("To"+NameWithoutNamespace(to.Name),new Type[]{from});
			if(convertMethod!=null){
				return convertMethod;
			}
			// Nope - look for its own personal cast operators.
			MethodInfo[] methods=from.GetMethods(BindingFlags.Public|BindingFlags.Static);
			foreach(MethodInfo method in methods){
				if(method.ReturnType!=to){
					continue;
				}
				isExplicit=(method.Name=="op_Explicit");
				if(isExplicit||method.Name=="op_Implicit"){
					return method;
				}
			}
			return null;
		}
		
		/// <summary>Converts a set of compiled fragments into a set of their outputted types.
		/// E.g. if a certain fragment is "a string", one of the types will be a string.</summary>
		/// <param name="frags">The set of fragments to convert.</param>
		/// <returns>A set of types. Each one is the output type of each fragment.</returns>
		public static Type[] GetTypes(CompiledFragment[] frags){
			if(frags==null){
				return new Type[0];
			}
			Type[] result=new Type[frags.Length];
			for(int i=frags.Length-1;i>=0;i--){
				result[i]=frags[i].OutputType();
			}
			return result;
		}
		
		/// <summary>Checks if the given object is a subclass of the given type.</summary>
		/// <param name="obj">The object to check.</param>
		/// <param name="super">The type to compare it with.</param>
		/// <returns>True if the given object is a subclass; false otherwise.</returns>
		public static bool IsSubclass(object obj,Type super){
			return obj.GetType().IsSubclassOf(super);
		}
		
		/// <summary>Checks if the given object is a type of or derives the given type.</summary>
		/// <param name="obj">The object to check.</param>
		/// <param name="super">The type to compare it with.</param>
		/// <returns>True if the given object derives or is of the given type; false otherwise.</returns>
		public static bool IsTypeOf(object obj,Type super){
			// True if obj is any child class Or is the class itself.
			if(obj==null){
				return false;
			}
			return super.IsAssignableFrom(obj.GetType());
		}
		
		/// <summary>Checks if the given code fragment is a CompiledFragment.</summary>
		/// <param name="obj">The fragment to check.</param>
		/// <returns>True if it is a CompiledFragment; false otherwise.</returns>
		public static bool IsCompiled(CodeFragment obj){
			return IsTypeOf(obj,typeof(CompiledFragment));
		}
		
	}
	
}