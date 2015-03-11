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
	/// Represents any pair of brackets. 
	/// </summary>
	
	public class BracketFragment:CodeFragment{
	
		/// <summary>The set of opening brackets.</summary>
		public static char[] Brackets=new char[]{'(','[','{'};
		/// <summary>The set of closing brackets.</summary>
		public static char[] EndBrackets=new char[]{')',']','}'};
	
		/// <summary>Checks if the given character can be handled by a bracket fragment.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if it's an opening bracket; false otherwise.</returns>
		public static bool WillHandle(char character){
			return (IsBracket(character)!=-1);
		}
		
		/// <summary>Checks if the given character is an opening bracket.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>The index of the bracket if it is one; -1 otherwise. ( is 0, [ is 1, { is 2.</returns>
		public static int IsBracket(char character){
			return IsOfType(Brackets,character);
		}
		
		/// <summary>Checks if the given character is an closing bracket.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>The index of the bracket if it is one; -1 otherwise. ) is 0, ] is 1, } is 2.</returns>
		public static int IsEndBracket(char character){
			return IsOfType(EndBrackets,character);
		}
		
		/// <summary>Checks if the given character is either an opening or closing bracket.</summary>
		/// <param name="character">The character to check.</param>
		/// <returns>True if the character is an opening or closing bracket; false otherwise.</returns>
		public static bool AnyBracket(char character){
			return (IsBracket(character)!=-1||(IsEndBracket(character)!=-1));
		}
		
		
		/// <summary>The opening bracket character. E.g. (,{,[.</summary>
		public char Bracket=StringReader.NULL;
		/// <summary>The closing bracket character. E.g. ),},].</summary>
		public char CloseBracket=StringReader.NULL;
		
		
		public BracketFragment(){}
		
		/// <summary>Creates a new BracketFragment.</summary>
		/// <param name="sr">The lexer to read the bracket content from.</param>
		public BracketFragment(CodeLexer sr):this(sr,true){}
		
		/// <summary>Creates a new BracketFragment.</summary>
		/// <param name="sr">The lexer to read the bracket content from.</param>
		/// <param name="readBracket">True if the brackets should be read off (i.e. the first thing done is read a single character). False otherwise.</param>
		public BracketFragment(CodeLexer sr,bool readBracket){
			
			if(readBracket){
				int type=IsBracket(sr.Read());
				
				if(type==-1){
					
					// Seek back just one - we want to re-read the thing we just tested for bracketness:
					sr.Position--;
					
					// No bracket; their implicit - read single operation:
					AddChild(new OperationFragment(sr,this));
					
					// And stop there:
					return;
					
				}else{
					
					// Grab the brackets:
					Bracket=Brackets[type];
					CloseBracket=EndBrackets[type];
				
				}
				
			}
			
			char peek=sr.Peek();
			bool bracket=(IsEndBracket(peek)!=-1);
			
			while(peek!=StringReader.NULL && !bracket){
				
				// Add the operation:
				AddChild(new OperationFragment(sr,this));
				
				// What's next?
				peek=sr.Peek();
				
				// Is it a bracket?
				bracket=(IsEndBracket(peek)!=-1);
			}
			
			if(bracket){
				// Read the last bracket off:
				sr.Read();
			}
			
		}
		
		public override bool Typeable(){
			return true;
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			if(!IsParent){
				return null;
			}
			
			CodeFragment child=FirstChild;
			
			while(child!=null){
				CodeFragment next=child.NextChild;
				CompiledFragment cfrag=child.Compile(method);
				
				if(cfrag!=null){
					cfrag.AddAfter(child);
				}
				
				child.Remove();
				child=next;
			}
			
			if(GivenType!=null){
				Type toType=GivenType.FindType(method.Script);
				
				if(toType==null){
					Error("Type not found: "+GivenType.ToString()+".");
				}
				
				CompiledFragment compiledKid=(CompiledFragment)FirstChild;
				CompiledFragment result=Types.TryCast(method,compiledKid,toType);
				
				if(result==null){
					Error("Cannot cast "+compiledKid.OutputType()+" to "+toType+".");
				}
				
				return result;
			}
			
			return (CompiledFragment)FirstChild;
		}
		
		/// <summary>Attempts to compile this bracket to IL, assuming it is the body of a method.</summary>
		/// <param name="method">The method that is being compiled.</param>
		/// <returns>True if the method returns something.</returns>
		public bool CompileBody(CompiledMethod method){
			Compile(method);
			return CompilationServices.CompileOperations(this,method);
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			if(Bracket=='['){
				
				if(to.LastChild==null){
					Error("Unexpected indexing. must be something[index].");
				}else{
					CodeFragment var=to.LastChild;
					var.Remove();
					
					return new IndexFragment(this,var).AddTo(to,sr);
				}
				
			}else if(Bracket=='{'&&to.LastChild!=null){
				// new TYPE[]{default,default..};
				
				if(to.LastChild.GetType()==typeof(VariableFragment)&&((VariableFragment)(to.LastChild)).Value=="new"){
					VariableFragment var=(VariableFragment)(to.LastChild);
					
					if(var.GivenType==null){
						Error("new must always be followed by a type name.");
					}else if(!var.GivenType.IsArray){
						Error("new Type{} only works when Type is an array and curley brackets define it's default values e.g. new int[]{1,2}");
					}else{
						var.Remove();
						return new ArrayFragment(var.GivenType,this).AddTo(to,sr);
					}
					
				}
				
			}else if(Bracket=='('&&to.LastChild!=null){
				Type type=to.LastChild.GetType();
				
				if(type==typeof(IndexFragment)||type==typeof(PropertyFragment)||(type==typeof(VariableFragment)&&!((VariableFragment)(to.LastChild)).IsKeyword())){
					// Method call!
					CodeFragment meth=to.LastChild;
					meth.Remove();
					
					return new MethodFragment(this,meth).AddTo(to,sr);
				}
				
			}
			
			return base.AddTo(to,sr);
		}
		
		public override string ToString(){
			if(Bracket==StringReader.NULL){
				return base.ToString();
			}
			
			string result=Bracket+base.ToString()+CloseBracket;
			
			if(GivenType!=null){
				result+=GivenType.ToString();
			}
			
			return result;
		}
		
	}
	
}