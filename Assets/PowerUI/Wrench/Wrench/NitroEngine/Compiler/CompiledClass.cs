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
	/// Represents a class being compiled from a nitro code string.
	/// </summary>

	public class CompiledClass{
		
		/// <summary>The type that this class derives from, if any.</summary>
		public Type BaseType;
		/// <summary>When fully compiled, this is the resulting system type of this class.</summary>
		public Type compiledType;
		/// <summary>The script this class belongs to.</summary>
		public NitroCode Script;
		/// <summary>True if this is a public class, false for private.</summary>
		public bool IsPublic=true;
		/// <summary>A counter used to name anonymous methods.</summary>
		protected int AnonymousCount;
		/// <summary>The type builder used to construct a system type from this class.</summary>
		public TypeBuilder Builder;
		/// <summary>The content of this whole class as parsed from the code.</summary>
		public CodeFragment ClassFragment;
		/// <summary>All the fields of this class.</summary>
		public Dictionary<string,FieldInfo> Fields=new Dictionary<string,FieldInfo>();
		/// <summary>All methods contained by this class grouped into overloads.</summary>
		public Dictionary<string,MethodOverloads> Methods=new Dictionary<string,MethodOverloads>();
		
		/// <summary>Creates a new class. Note you must call StartType manually.</summary>
		public CompiledClass(){}
		
		public CompiledClass(CodeFragment classFragment,NitroCode script,string name,Type baseType,bool isPublic){
			IsPublic=isPublic;
			StartType(name,script,baseType);
			ClassFragment=classFragment;
		}
		
		/// <summary>Starts creating this class.</summary>
		/// <param name="name">The name of the class.</param>
		/// <param name="script">The parent script this class belongs to.</param>
		/// <param name="baseType">The type that this class will derive from.</param>
		public void StartType(string name,NitroCode script,Type baseType){
			BaseType=baseType;
			Script=script;
			TypeAttributes attribs=IsPublic?TypeAttributes.Public:TypeAttributes.NotPublic;
			Builder=script.Builder.DefineType(name,attribs,baseType);
		}
		
		/// <summary>Gets this class as a system type.</summary>
		/// <returns>This classes system type.</returns>
		public Type GetAsType(){
			return (Type)Builder;
		}
		
		/// <summary>Compiles this class from its ClassFragment.</summary>
		public void Compile(){
			// Step 1. Find all methods of classes.
			FindMethods(ClassFragment);
			
			// Step 2. Find all the properties of classes.
			FindProperties(ClassFragment);
			
			// Step 3. Compile the method signatures.
			foreach(KeyValuePair<string,MethodOverloads> kvp in Methods){
				kvp.Value.CompileParameters();
			}
			
			// Step 4. Compile method bodies:
			foreach(KeyValuePair<string,MethodOverloads> kvp in Methods){
				kvp.Value.CompileBody();
			}
			
			// Step 5. Build the type.
			compiledType=Builder.CreateType();
		}
		
		/// <summary>Finds the method overload set with the given name and creates it if it doesn't exist.</summary>
		/// <param name="name">The name of the method.</param>
		/// <param name="returnType">The type that this method returns.</param>
		/// <returns>The method overload set.</returns>
		private MethodOverloads MakeOrFind(string name,Type returnType){
			MethodOverloads set;
			if(!Methods.TryGetValue(name,out set)){
				if(Types.IsVoid(returnType)){
					returnType=typeof(Void);
				}
				set=new MethodOverloads(returnType);
				Methods.Add(name,set);
			}
			return set;
		}
		
		/// <summary>Checks if this class contains the method with the given name.</summary>
		/// <param name="name">The name of the method to look for.</param>
		/// <returns>True if the method is in this class, false otherwise.</returns>
		public bool ContainsMethod(string name){
			return (FindMethodSet(name)!=null);
		}
		
		/// <summary>Checks if this class contains the field with the given name.</summary>
		/// <param name="field">The name of the field to look for.</param>
		/// <returns>True if the field is in this class, false otherwise.</returns>
		public bool ContainsField(string field){
			return (GetField(field)!=null);
		}
		
		/// <summary>Gets the return type of a given method by name. Note all overloads must return the same thing.</summary>
		/// <returns>The type of object this method returns.</returns>
		public Type MethodReturnType(string name){
			MethodOverloads set=FindMethodSet(name);
			if(set!=null){
				return set.ReturnType;
			}
			MethodInfo[] methods=Builder.BaseType.GetMethods();
			for(int i=0;i<methods.Length;i++){
				MethodInfo method=methods[i];
				if(method.Name.ToLower()==name){
					return method.ReturnType;
				}
			}
			return null;
		}
		
		/// <summary>Gets a particular method (it may be overloaded) from this class.</summary>
		/// <param name="name">The name of the method.</param>
		/// <param name=arguments">The set of arguments being used in calling this method.</param>
		/// <returns>The MethodInfo for the method if found; null otherwise.</returns>
		public MethodInfo FindMethodOverload(string name,CompiledFragment[] arguments){
			MethodOverloads set=FindMethodSet(name);
			if(set==null){
				return Types.GetOverload(Builder.BaseType.GetMethods(),name,Types.GetTypes(arguments),true);
			}
			return set.GetOverload(Types.GetTypes(arguments));
		}
		
		/// <summary>Gets a particular method (it may be overloaded) from this class.</summary>
		/// <param name="name">The name of the method.</param>
		/// <param name=arguments">The types of the set of arguments being used in calling this method.</param>
		/// <returns>The MethodInfo for the method if found; null otherwise.</returns>
		public MethodInfo FindMethodOverload(string name,Type[] arguments){
			MethodOverloads set=FindMethodSet(name);
			if(set==null){
				return Types.GetOverload(Builder.BaseType.GetMethods(),name,arguments,true);
			}
			return set.GetOverload(arguments);
		}
		
		/// <summary>Finds the overload set for the method with the given name.</summary>
		/// <param name="name">The name of the method.</param>
		/// <returns>The methods overload set if found; null otherwise.</returns>
		public MethodOverloads FindMethodSet(string name){
			MethodOverloads mIP;
			Methods.TryGetValue(name,out mIP);
			return mIP;
		}
		
		/// <summary>Gets the property from this class with the given name.</summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>A PropertyInfo object if the field exists; null otherwise.</returns>
		public PropertyInfo GetProperty(string name){
			if(Types.IsDynamic(Builder.BaseType)){
				return null;
			}
			return Builder.BaseType.GetProperty(name,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		}
		
		/// <summary>Gets the field from this class with the given name.</summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>A FieldInfo object if the field exists; null otherwise.</returns>
		public FieldInfo GetField(string name){
			FieldInfo fInfo=null;
			if(Fields.TryGetValue(name,out fInfo)){
				return fInfo;
			}
			return Builder.BaseType.GetField(name,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		}
		
		/// <summary>Finds properties in the given fragment.</summary>
		/// <param name="fragment">The fragment to search.</param>
		public void FindProperties(CodeFragment fragment){
			CodeFragment child=fragment.FirstChild;
			CodeFragment next=null;
			
			while(child!=null){
				next=child.NextChild;
				
				if(child.IsParent()){
					
					if(child.GetType()==typeof(OperationFragment)){
						// Is it private? Note that if it is, "private" is removed.
						bool isPublic=!Modifiers.Check(child.FirstChild,"private");
						
						// Grab the property name:
						CodeFragment propName=child.FirstChild;
						
						if(propName==null){
							child.Error("This value must be followed by something.");
						}
						
						if(propName.GetType()!=typeof(VariableFragment)){
							propName.Error("Didn't recognise this as a property. Please note that all code you'd like to run immediately should be inside a function called Start, or in the constructor of a class.");
						}
						
						VariableFragment vfrag=((VariableFragment)child.FirstChild);
						
						if(vfrag.IsKeyword()){
							propName.Error("Can't use keywords as property names.");
						}
						
						string Name=vfrag.Value;
						
						if(Fields.ContainsKey(Name)){
							propName.Error("This property already exists in this class.");
						}
						
						CodeFragment defaultValue=null;
						
						if(vfrag.NextChild==null){
						}else if(vfrag.NextChild!=null&&!Types.IsTypeOf(vfrag.NextChild,typeof(OperatorFragment))){
							// No type OR default, or the block straight after isn't : or =
							propName.Error("Invalid property '"+Name+"' - missing a type or default value.");
						}else{
							
							OperatorFragment opFrag=(OperatorFragment)vfrag.NextChild;
							// It must be a set otherwise it's invalid.
							if(opFrag.Value==null||opFrag.Value.GetType()!=typeof(OperatorSet)){
								propName.Error("Invalid default value provided for '"+Name+"'.");
							}
							
							defaultValue=child;
						}
						
						DefineField(Name,vfrag,isPublic,ref defaultValue);
						
						child.Remove();
						
						if(defaultValue!=null){
							GetInit().CodeBlock.AddChild(defaultValue);
						}
					}else{
						FindProperties(child);
					}
				}
				child=next;
			}
		}
		
		/// <summary>Defines a new field on this class.</summary>
		/// <param name="name">The name of the field.</param>
		/// <param name="type">The type of the value held by this field.</param>
		/// <returns>A new FieldBuilder.</returns>
		protected virtual void DefineField(string name,VariableFragment nameFragment,bool isPublic,ref CodeFragment defaultValue){
			
			Type type=null;
			
			if(nameFragment.GivenType!=null){
				type=nameFragment.GivenType.FindType(Script);
				nameFragment.GivenType=null;
				
				if(type==null){
					nameFragment.Error(name+" has a type that was not recognised.");
				}
				
			}else{
				nameFragment.Error(name+"'s type isn't given. Should be e.g. "+name+":String if it has no default value.");
			}
			
			FieldBuilder field=Builder.DefineField(name,type,isPublic?FieldAttributes.Public:FieldAttributes.Private);
			Fields[name]=field;
		}
		
		/// <summary>Gets the OnScriptReady method. May create it if it's not available.</summary>
		/// <returns>The start method. All code outside of functions that isn't a variable goes into this.</returns>
		private CompiledMethod GetStartMethod(){
			MethodOverloads set=MakeOrFind("OnScriptReady",null);
			if(set.Methods.Count==0){
				set.AddMethod(new CompiledMethod(this,"OnScriptReady",null,new BracketFragment(),null,0,true));
			}
			CompiledMethod method=set.Methods[0];
			method.GloballyScoped=true;
			return method;
		}
		
		/// <summary>Gets the init method. May create it if it's not already available.</summary>
		/// <returns>The init method.</returns>
		private CompiledMethod GetInit(){
			MethodOverloads set=MakeOrFind(".init",null);
			if(set.Methods.Count==0){
				set.AddMethod(new CompiledMethod(this,".init",null,new BracketFragment(),null,0,true));
			}
			return set.Methods[0];
		}
		
		/// <summary>Finds methods within the given fragment by looking for 'function'.</summary>
		/// <param name="fragment">The fragment to search.</param>
		public void FindMethods(CodeFragment fragment){
			CodeFragment child=fragment.FirstChild;
			while(child!=null){
				CodeFragment next=child.NextChild;
				if(child.IsParent()){
					FindMethods(child);
				}else if(child.GetType()==typeof(VariableFragment)){
					VariableFragment vfrag=((VariableFragment)child);
					CodeFragment toRemove=null;
					string Value=vfrag.Value;
					if(Value=="function"){
						// Found a function.
						bool isPublic;
						Modifiers.Handle(vfrag,out isPublic);
						
						int line=vfrag.LineNumber;
						// The return type could be on the function word (function:String{return "hey!";})
						TypeFragment returnType=child.GivenType;
						
						toRemove=child;
						child=child.NextChild;
						if(child==null){
							fragment.Error("Keyword 'function' can't be used on its own.");
						}
						toRemove.Remove();
						string name="";
						bool anonymous=false;
						BracketFragment parameters=null;
						
						if(child.GetType()==typeof(MethodFragment)){
							MethodFragment method=(MethodFragment)child;
							name=((VariableFragment)(method.MethodName)).Value;
							parameters=method.Brackets;
							toRemove=child;
							child=child.NextChild;
							toRemove.Remove();
							returnType=method.GivenType;
						}else if(child.GetType()==typeof(VariableFragment)){
							// Found the name
							vfrag=(VariableFragment)child;
							if(vfrag.IsKeyword()){
								vfrag.Error("Keywords cannot be used as function names ("+vfrag.Value+").");
							}
							name=vfrag.Value;
							if(returnType==null){
								returnType=child.GivenType;
							}
							toRemove=child;
							child=child.NextChild;
							toRemove.Remove();
							if(child==null){
								fragment.Error("Invalid function definition ("+name+"). All brackets are missing or arent valid.");
							}
							
						}else{
							anonymous=true;
						}
						
						next=AddFoundMethod(fragment,child,name,anonymous,parameters,returnType,line,isPublic);
						
					}
				}else if(child.GetType()==typeof(MethodFragment)){
					// Looking for anonymous methods ( defined as function() )
					MethodFragment method=(MethodFragment)child;
					if(method.MethodName.GetType()==typeof(VariableFragment)){
						VariableFragment methodName=((VariableFragment)(method.MethodName));
						string name=methodName.Value;
						if(name=="function"){
							// Found an anonymous function, function():RETURN_TYPE{}.
							// Note that function{}; is also possible and is handled above.
							CodeFragment toRemove=child;
							child=child.NextChild;
							toRemove.Remove();
							
							next=AddFoundMethod(fragment,child,null,true,method.Brackets,method.GivenType,methodName.LineNumber,true);
						}else if(method.Brackets!=null){
							FindMethods(method.Brackets);
						}
					}else if(method.Brackets!=null){
						FindMethods(method.Brackets);
					}
				}
				child=next;
			}
		}
		
		/// <summary>Adds a method that was found into this classes set of methods to compile.</summary>
		/// <param name="fragment">The first fragment of the method, used for generating errors. This gives a valid line number.</param>
		/// <param name="body">The block of code for this method.</param>
		/// <param name="name">The name of the method. Null if anonymous is true.</param>
		/// <param name="anonymous">True if this method is an anonymous one and requires a name.</param>
		/// <param name="parameters">The set of parameters for this method.</param>
		/// <param name="returnType">The type that this method returns.</param>
		/// <param name="line">The line number of the method.</param>
		/// <param name="isPublic">True if this is a public method; false for private.</param>
		/// <returns>The first fragment following the method, if there is one.</returns>
		protected virtual CodeFragment AddFoundMethod(CodeFragment fragment,CodeFragment body,string name,bool anonymous,BracketFragment parameters,TypeFragment returnType,int line,bool isPublic){
			if(body==null){
				fragment.Error("Invalid function definition ("+name+"). The content block {} is missing or isnt valid.");
			}
			
			if(anonymous){
				name="Compiler-Generated-$"+AnonymousCount;
				AnonymousCount++;
			}
			
			// The following is the explicit code block for this function:
			BracketFragment codeBlock=(BracketFragment)body;
			CompiledMethod cMethod=new CompiledMethod(this,name,parameters,codeBlock,returnType,line,isPublic);
			MethodOverloads set=MakeOrFind(name,cMethod.Builder.ReturnType);
			CodeFragment next=body.NextChild;
			if(anonymous){
				CodeFragment newChild=DynamicMethodCompiler.Compile(cMethod,name,set.ReturnType,new ThisOperation(cMethod));
				newChild.AddAfter(body);
			}
			body.Remove();
			set.AddMethod(cMethod);
			FindMethods(codeBlock);
			return next;
		}
		
	}
	
}