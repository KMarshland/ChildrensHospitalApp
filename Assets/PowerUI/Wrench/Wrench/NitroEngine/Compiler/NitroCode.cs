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
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Nitro{
	
	public delegate void AotFileEvent(string path);
	
	/// <summary>
	/// This class represents a compiled script. It's not directly callable though.
	/// You'll need to Instance an object (and cast to whatever baseType used) using the Instance method and can then call methods on that.
	/// </summary>
	
	public class NitroCode{
		
		/// <summary>True if Setup has been called.</summary>
		private static bool Started;
		/// <summary>A counter for how many scripts have been compiled. Used to generate a unique module name.</summary>
		public static int ModuleCounter;
		/// <summary>Called when the AOT file we want to write to already exists.</summary>
		public static AotFileEvent OnAotFileExists;
		
		
		/// <summary>Sets up the compiler.</summary>
		public static void Setup(){
			if(Started){
				return;
			}
			Started=true;
			Type[] allTypes=Assembly.GetExecutingAssembly().GetTypes();
			Type operatorT=typeof(Operator);
			// For each type..
			for(int i=allTypes.Length-1;i>=0;i--){
				Type type=allTypes[i];
				if(type.IsSubclassOf(operatorT)){
					// Register it as being an available type of operator.
					Operator.Add((Operator)Activator.CreateInstance(type));
				}
			}
			TypeAliases.Setup();
		}
		
		/// <summary>The raw code to compile.</summary>
		public string Code;
		/// <summary>The fully compiled type. Must call Instance to make use of it.</summary>
		private Type CompiledType;
		/// <summary>The builder that creates this script.</summary>
		public ModuleBuilder Builder;
		/// <summary>The class to derive from.</summary>
		private CompiledClass BaseClass;
		/// <summary>The set of referenced assemblies from the top of this script file.</summary>
		private List<CodeReference> References;
		/// <summary>The security domain manager which defines what the code can access.</summary>
		private NitroDomainManager ScriptDomainManager;
		/// <summary>A set of all compiled classes defined within the code.</summary>
		private Dictionary<string,CompiledClass> Types=new Dictionary<string,CompiledClass>();
		
		
		public NitroCode():this(null,null,null,null,null){}
		
		/// <summary>Creates a new NitroCode with the given nitro code to compile.
		/// Uses the default security manager and derives <see cref="Nitro.Script"/>.</summary>
		/// <param name="code">The code to compile.</param>
		public NitroCode(string code):this(code,null,null,null,null){}
		
		/// <summary>Creates a new NitroCode with the given nitro code and type to derive from. Uses the default security manager.</summary>
		/// <param name="code">The code to compile.</param>
		/// <param name="baseType">The type that the compiled code will inherit.
		/// This is the type to cast <see cref="Nitro.NitroCode.Instance"/> to.</param>
		public NitroCode(string code,Type baseType):this(code,baseType,null,null,null){}
		
		/// <summary>Creates a new NitroCode with the given security manager.
		/// You'll need to call <see cref="Nitro.NitroCode.Compile"/> manually.</summary>
		/// <param name="manager">The security manager which defines what the code can access.</param>
		public NitroCode(NitroDomainManager manager):this(null,null,manager,null,null){}
		
		/// <summary>Creates a new NitroCode with the given nitro code and security manager.</summary>
		/// <param name="code">The code to compile.</param>
		/// <param name="manager">The security manager which defines what the code can access.</param>
		public NitroCode(string code,NitroDomainManager manager):this(code,null,manager,null,null){}
		
		/// <summary>Creates a new NitroCode with the given nitro code, type to derive from and security manager.</summary>
		/// <param name="code">The code to compile.</param>
		/// <param name="baseType">The type that the compiled code will inherit.
		/// This is the type to cast <see cref="Nitro.NitroCode.Instance"/> to.</param>
		/// <param name="manager">The security manager which defines what the code can access.</param>
		public NitroCode(string code,Type baseType,NitroDomainManager manager):this(code,baseType,manager,null,null){}
		
		/// <summary>Creates a new NitroCode with the given nitro code, type to derive from and security manager.</summary>
		/// <param name="code">The code to compile.</param>
		/// <param name="baseType">The type that the compiled code will inherit.
		/// This is the type to cast <see cref="Nitro.NitroCode.Instance"/> to.</param>
		/// <param name="manager">The security manager which defines what the code can access.</param>
		/// <param name="aotFile">A DLL path to write the compiled code to (For e.g. AOT compilation).</param>
		public NitroCode(string code,Type baseType,NitroDomainManager manager,string aotFile,string aotAssemblyName){
			if(manager==null){
				manager=NitroDomainManager.GetDefaultManager();
			}
			ScriptDomainManager=manager;
			References=manager.GetDefaultReferences();
			
			if(code!=null){
				Compile(code,baseType,aotFile,aotAssemblyName);
			}
		}
		
		/// <summary>Compiles the given code now deriving from the given object.</summary>
		/// <param name="code">The code to compile</param>
		/// <param name="baseType">The type to inherit from. If null, the code will inherit from the default Script type.</param>
		public void Compile(string code,Type baseType){
			Compile(code,baseType,null,null);
		}
		
		/// <summary>Compiles the given code now deriving from the given object.</summary>
		/// <param name="code">The code to compile</param>
		/// <param name="baseType">The type to inherit from. If null, the code will inherit from the default Script type.</param>
		/// <param name="aotFile">A DLL path to write the compiled code to (For e.g. AOT compilation).</param>
		public void Compile(string code,Type baseType,string aotFile,string aotAssemblyName){
			Code=code;
			if(baseType==null){
				baseType=typeof(Script);
			}
			
			// Are we compiling to a file?
			string aotFilename="";
			string assemblyPath=null;
			bool aot=!string.IsNullOrEmpty(aotFile);
			
			// The assembly name:
			AssemblyName assemblyName=null;
			
			if(aot){
				// Grab the file name (used below too):
				aotFilename=System.IO.Path.GetFileName(aotFile);
				// Setup the assembly name:
				assemblyName=new AssemblyName(aotAssemblyName);
			}else{
				assemblyName=new AssemblyName("$SS_"+ModuleCounter);
			}
			
			// Assembly access:
			AssemblyBuilderAccess access=AssemblyBuilderAccess.Run;
			
			if(aot){
				// We're ahead-of-time compiling this to a file.
				
				// Grab the directory the file must go in:
				assemblyPath=System.IO.Path.GetDirectoryName(aotFile);
				
				if(assemblyPath!=null){
					if(!Directory.Exists(assemblyPath)){
						Directory.CreateDirectory(assemblyPath);
					}
				}
				
				access=AssemblyBuilderAccess.Save;
			}
			
			// Create the assembly builder. If we're AOT compiling, it's saveable.
			AssemblyBuilder assemblyBuilder=AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName,access,assemblyPath);
			
			if(aot){
				// Create the module:
				Builder=assemblyBuilder.DefineDynamicModule("SS-DMOD",aotFilename);
			}else{
				Builder=assemblyBuilder.DefineDynamicModule("SS-DMOD");
			}
			
			ModuleCounter++;
			
			// Ok - let's start compiling. Define a base class that all code goes into:
			BaseClass=new CompiledClass();
			
			// That class will be known as..
			BaseClass.StartType("NitroScriptCode",this,baseType);
			
			// Start parsing the code into a tree of fragments:
			CodeLexer sr=new CodeLexer(Code);
			BaseClass.ClassFragment=new BaseFragment(sr);
			
			// Search the fragments for any classes:
			FindClasses(BaseClass.ClassFragment);
			
			// Compile the classes we found:
			foreach(KeyValuePair<string,CompiledClass>kvp in Types){
				kvp.Value.Compile();
			}
			
			// Compile the base class:
			BaseClass.Compile();
			CompiledType=BaseClass.compiledType;
			Types=null;
			BaseClass=null;
			
			if(aot){
				// Great - overwrite it.
				if(File.Exists(aotFile)){
					if(OnAotFileExists==null){
						File.Delete(aotFile);
					}else{
						OnAotFileExists(aotFile);
					}
				}
				
				assemblyBuilder.Save(aotFilename);
			}
		}
		
		/// <summary>Adds a reference which is named in the given fragment. Using name; is handled here.</summary>
		/// <param name="usingName">The fragment containing the reference.</param>
		public void AddReference(CodeFragment usingName){
			string fullName=ParseReference(usingName);
			
			if(References==null){
				References=new List<CodeReference>();
			}
			
			// The fullName value is now e.g. 'System.Generics'
			References.Add(new CodeReference(fullName));
		}
		
		/// <summary>Retrieves a section of the reference from the given fragment.
		/// This is required as a reference is likely to be a chain of property fragments which must
		/// be followed to retrieve the full reference.</summary>
		/// <param name="usingName">The current fragment to handle.</param>
		/// <returns>The reference as a string, e.g. 'System.Generics'</returns>
		private string ParseReference(CodeFragment usingName){
			if(usingName==null){
				return "";
			}
			Type type=usingName.GetType();
			if(type==typeof(VariableFragment)){
				// e.g. using System;
				return ((VariableFragment)usingName).Value;
			}else if(type==typeof(PropertyFragment)){
				// e.g. using System.Generic;
				// Follow the stack of 'of' until you hit null.
				PropertyFragment property=((PropertyFragment)usingName);
				string text=property.Value;
				string propertyOf=ParseReference(property.of);
				if(propertyOf!=""){
					return propertyOf+"."+text;
				}else{
					return text;
				}
			}
			return "";
		}
		
		/// <summary>Finds all classes within the given code fragment, identified with 'class'.</summary>
		/// <param name="frag">The fragment to search.</param>
		private void FindClasses(CodeFragment frag){
			CodeFragment child=frag.FirstChild;
			while(child!=null){
				CodeFragment next=child.NextChild;
				
				if(child.IsParent){
					
					FindClasses(child);
					
				}else if(child.GetType()==typeof(VariableFragment)){
					VariableFragment vfrag=(VariableFragment)child;
					
					if(vfrag.Value=="class"){
						bool isPublic;
						Modifiers.Handle(vfrag,out isPublic);
						
						Type baseType=null;
						CodeFragment nameBlock=vfrag.NextChild;
						
						if(nameBlock==null||nameBlock.GetType()!=typeof(VariableFragment)){
							vfrag.Error("Class keyword used but no class name given.");
						}
						
						vfrag.Remove();
						vfrag=(VariableFragment)nameBlock;
						
						if(vfrag.IsKeyword()){
							vfrag.Error("Can't use keywords for class names.");
						}
						
						string name=vfrag.Value;
						
						if(Types.ContainsKey(name)){
							vfrag.Error("Cannot redefine class "+name+" - it already exists in this scope.");
						}
						
						if(vfrag.GivenType!=null){
							baseType=vfrag.GivenType.FindType(this);
						}
						
						if(baseType==null){
							baseType=typeof(object);
						}
						
						CodeFragment cblock=vfrag.NextChild;
						
						if(cblock==null||cblock.GetType()!=typeof(BracketFragment)){
							vfrag.Error("Class "+name+" is missing it's code body. Correct syntax: class "+name+"{ .. }.");
						}
						
						next=cblock.NextChild;
						vfrag.Remove();
						cblock.Remove();
						Types.Add(name,new CompiledClass(cblock,this,name,baseType,isPublic));
					}
					
				}
				
				child=next;
			}
			
		}
		
		/// <summary>Checks if a type is allowed to be used.</summary>
		/// <param name="ofType">The type to check for clearance.</param>
		/// <returns>True if it's allowed, false otherwise.</returns>
		public bool AllowUse(Type ofType){
			
			if(typeof(NitroDomainManager).IsAssignableFrom(ofType)){
				// Never allowed to use a nitro security domain.
				return false;
			}
			
			if(ScriptDomainManager.AllowsEverything()){
				return true;
			}
			
			if(ScriptDomainManager.IsAllowed(ofType)){
				return true;
			}
			
			return false;
		}
		
		/// <summary>Checks if this code contains a method with the given name.</summary>
		/// <param name="name">The method name to search for.</param>
		/// <returns>True if this code contains the named method.</returns>
		public bool ContainsMethod(string name){
			
			if(BaseClass==null){
				
				// It's been compiled.
				
				if(CompiledType==null){
				
					// Well, a compile attempt happened at least!
					return false;
				}
				
				// Grab the method from the compiled type instead:
				return (CompiledType.GetMethod(name.ToLower())!=null);
			}
			
			return BaseClass.ContainsMethod(name);
		}
		
		/// <summary>Checks if this code contains a field with the given name.</summary>
		/// <param name="field">The field name to search for.</param>
		/// <returns>True if this code contains the named field.</returns>
		public bool ContainsField(string field){
			
			if(BaseClass==null){
				
				// It's been compiled.
				
				if(CompiledType==null){
				
					// Well, a compile attempt happened at least!
					return false;
				}
				
				// Grab the method from the compiled type instead:
				return (CompiledType.GetField(field.ToLower())!=null);
			}
			
			return BaseClass.ContainsField(field);
		}
		
		/// <summary>Searches for the system type with the given name by first performing an alias lookup.</summary>
		/// <param name="name">The type name to search for.</param>
		/// <returns>A system type if found; null otherwise.</returns>
		public Type GetType(string name){
			// Alias lookup - is name an alias? (e.g. 'int')
			Type type=TypeAliases.Find(name);
			
			if(type!=null){
				// It sure was - return the result.
				return type;
			}
			
			// Check if we can straight get the type by this name.
			// Go hunting - can we find typeName anywhere?
			type=Type.GetType(name,false,true);
			
			if(type!=null){
				// Wohoo that was easy!
				return type;
			}
			
			// Start looking around in our Referenced namespaces to find this type.
			// This is done because e.g. Socket is named System.Net.Socket in the System.Net namespace.
			// As the reference knows which assembly to look in, these are quick to handle.
			if(References==null){
				return null;
			}
			
			foreach(CodeReference reference in References){
				type=reference.GetType(name);
				
				if(type!=null){
					return type;
				}
				
			}
			
			// It could also be in an assembly on it's own without a namespace.
			// Lets look for that next.
			// Make sure all available assemblies are setup for use:
			CodeReference.Setup();
			
			// And check in each one:
			foreach(KeyValuePair<string,CodeAssembly> assembly in CodeReference.Assemblies){
				
				if(assembly.Value.Current || assembly.Value.NitroAOT){
					// This was the first thing we checked, or is a Nitro assembly - skip.
					continue;
				}
				
				type=assembly.Value.GetType(name);
				
				if(type!=null){
					return type;
				}
				
			}
			
			return null;
		}
		
		/// <summary>Attempts to get a CompiledClass object from its system type.</summary>
		/// <param name="type">The system type to look for.</param>
		/// <returns>A compiled class if the type was found; Null otherwise.</returns>
		public CompiledClass GetClass(Type type){
			return GetClass(type.Name);
		}
		
		/// <summary>Attempts to get a CompiledClass object from its class name.</summary>
		/// <param name="name">The name of the class to look for.</param>
		/// <returns>A CompiledClass if the class was found; Null otherwise.</returns>
		public CompiledClass GetClass(string name){
			if(name=="NitroScriptCode"){
				return BaseClass;
			}
			
			CompiledClass result;
			Types.TryGetValue(name,out result);
			return result;
		}
		
		/// <summary>Instances this code as an object which can have methods called upon it.
		/// If you defined a baseType in the NitroCode objects constructor, cast the result of this to that type.
		/// Otherwise, you can cast it to a <see cref="Nitro.Script"/> object which makes calling the methods easier.</summary>
		/// <returns>An instance of the compiled code.</returns>
		public object Instance(){
			if(CompiledType==null){
				return null;
			}
			
			return Activator.CreateInstance(CompiledType);
		}
		
		/// <summary>The compiled useable type of the object returned by Instance.</summary>
		public Type OutputType{
			get{
				return CompiledType;
			}
		}
		
	}
	
}