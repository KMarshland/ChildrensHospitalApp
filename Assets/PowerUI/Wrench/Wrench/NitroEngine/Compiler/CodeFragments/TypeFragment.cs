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
using Wrench;

namespace Nitro{
	
	/// <summary>
	/// Represents a type. (e.g. new TYPE or :TYPE).
	/// </summary>
	
	public class TypeFragment:CodeFragment{
	
		/// <summary>Checks if a type fragment can be read from a lexer by checking if the current character is a colon.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if the character is a colon (:); false otherwise.</returns>
		public static bool WillHandle(char character){
			return (character==':');
		}
	
		/// <summary>True if this is an array type.</summary>
		public bool IsArray;
		/// <summary>True if this type was read with a colon before it.</summary>
		public bool HasColon;
		/// <summary>The type as a string.</summary>
		public string Value="";
		/// <summary>If it's an array, this is how many dimensions the array has.</summary>
		public int Dimensions=1;
		/// <summary>Generic values may be generics themselves, so this is the set of nested generics. (e.g. List<List<List<List<string>>>>).</summary>
		public TypeFragment[] GenericSet;
		
		
		/// <summary>Creates a new type fragment by reading it and a preceeding colon from the given lexer.</summary>
		/// <param name="sr">The lexer to read from.</param>
		public TypeFragment(CodeLexer sr):this(sr,true){}
		
		/// <summary>Creates a new type fragment by reading it from the given lexer.</summary>
		/// <param name="sr">The lexer to read it from.</param>
		/// <param name="hasColon">True if a colon (:) should be read from the lexer.</param>
		public TypeFragment(CodeLexer sr,bool hasColon){
			HasColon=hasColon;
			if(HasColon){
				// Read off the colon:
				sr.Read();
			}
			// Read until some other block can take over:
			while(true){
				char peek=sr.Peek();
				if(peek=='<'){
					List<TypeFragment> generics=new List<TypeFragment>();
					while(true){
						// Read off the < or the comma:
						sr.Read();
						generics.Add(new TypeFragment(sr,false));
						if(sr.Peek()=='>'){
							sr.Read();
							GenericSet=generics.ToArray();
							break;
						}
					}
					if(sr.Peek()=='['){
						SetArray(sr);
					}
					break;
				}else if(peek=='['){
					SetArray(sr);
					break;
				}else if(peek==','||peek==';'||peek==StringReader.NULL||BracketFragment.AnyBracket(peek)||Operator.IsOperator(peek)){
					// Pass control back to the operation:
					break;
				}
				Value+=char.ToLower(sr.Read());
			}
		}
		
		/// <summary>Reads array indices (array[,,,] = 3 dimensions) from the given lexer and sets the dimensions from the number of them.</summary>
		/// <param name="sr">The lexer to read the indices from.</param>
		private void SetArray(CodeLexer sr){
			IsArray=true;
			// Read off the [ :
			sr.Read();
			while(sr.Peek()!=']'){
				Dimensions++;
				char C=sr.Read();
				if(C!=','){
					Error("Bad array type ("+ToString()+"). myArray:String[]=new String[](40); or myArray:String[]=new String[]{\"V1\",\"V2\"}; is the correct syntax.");
				}
			}
			// read off the ] :
			sr.Read();
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			if(to.LastChild==null||!to.LastChild.Typeable()){
				Error("A type ("+ToString()+") was found in an unexpected place.");
			}
			to.LastChild.GivenType=this;
			return AddResult.Ok;
		}
		
		/// <summary>Attempts to find the type named here and resolve it to a system type.</summary>
		/// <param name="script">The script that should also be checked for types and acts as the security domain.</param>
		/// <returns>A system type if the type could be resolved and allowed successfully; null otherwise.</returns>
		public Type FindType(NitroCode script){
			string name=Value;
			if(GenericSet!=null){
				name+="`"+GenericSet.Length;
			}
			Type baseType=script.GetType(name);
			if(baseType==null){
				CompiledClass cClass=script.GetClass(Value);
				if(cClass==null){
					return null;
				}
				baseType=cClass.GetAsType();
			}
			if(GenericSet!=null){
				if(!baseType.IsGenericTypeDefinition){
					Error(Value+" is not a generic type.");
				}
				Type[] genericTypes=new Type[GenericSet.Length];
				for(int i=GenericSet.Length-1;i>=0;i--){
					if((genericTypes[i]=GenericSet[i].FindType(script))==null){
						return null;
					}
				}
				baseType=baseType.MakeGenericType(genericTypes);
			}
			if(IsArray){
				if(Dimensions==1){
					baseType=baseType.MakeArrayType();
				}else{
					baseType=baseType.MakeArrayType(Dimensions);
				}
			}
			return baseType;
		}
		
		public override string ToString(){
			string result=HasColon?":":"";
			result+=Value;
			if(GenericSet!=null){
				result+="<";
				for(int i=0;i<GenericSet.Length;i++){
					if(i!=0){
						result+=",";
					}
					result+=GenericSet[i].ToString();
				}
				result+=">";
			}
			if(IsArray){
				result+="[";
				for(int i=1;i<Dimensions;i++){
					result+=",";
				}
				result+="]";
			}
			return result;
		}
		
	}
	
}