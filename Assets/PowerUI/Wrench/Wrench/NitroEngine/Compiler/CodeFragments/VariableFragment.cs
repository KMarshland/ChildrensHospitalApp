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
	/// Represents a variable. These are essentially text in the code that are not keywords. (e.g. hello=14;)
	/// </summary>
	
	public class VariableFragment:CodeFragment{
		
		/// <summary>The set of all keywords used by Nitro.</summary>
		public static string[] Keywords=new string[]{"return","continue","break","using"};
		
		/// <summary>The raw variables name.</summary>
		public string Value="";
		/// <summary>The line that this variable was found on.</summary>
		public int LineNumber=-1;
		
		
		/// <summary>Creates a new variable fragment from the given variable text.</summary>
		/// <param name="value">The raw variables name.</param>
		public VariableFragment(string value){
			Value=value;
		}
		
		/// <summary>Creates a new variable fragment by reading it from the given lexer.</summary>
		/// <param name="sr">The lexer to read the variable from.</param>
		public VariableFragment(CodeLexer sr){
			Value+=char.ToLower(sr.Read());
			LineNumber=sr.LineNumber;
			// Read until some other block can take over:
			while(true){
				char peek=sr.Peek();	
				if(peek==';'||peek==','||peek==StringReader.NULL||BracketFragment.AnyBracket(peek)){
					// Pass control back to the operation:
					break;
				}
				if(sr.PeekJunk()){
					// Is Value anything special?
					if(Value=="var"){
						// It's Junk
						Value="";
						break;
					}else if(Value=="private"){
						break;
					}else if(Value=="function"){
						break;
					}else if(Value=="class"){
						break;
					}else if(Value=="new"){
						GivenType=new TypeFragment(sr,false);
						break;
					}else if(IsKeyword()){
						break;
					}
				}
				Handler handle=Handlers.Find(peek);
				if(handle!=Handler.Stop&&handle!=Handler.Variable&&handle!=Handler.Number){
					break;
				}
				Value+=char.ToLower(sr.Read());
			}
		}
		
		/// <summary>Checks if this is a keyword (e.g. return, break).</summary>
		/// <returns>True if this is a keyword.</returns>
		public bool IsKeyword(){
			for(int i=Keywords.Length-1;i>=0;i--){
				if(Value==Keywords[i]){
					return true;
				}
			}
			return false;
		}
		
		public override int GetLineNumber(){
			if(LineNumber!=-1){
				return LineNumber;
			}
			return base.GetLineNumber();
		}
		
		public override bool Typeable(){
			return true;
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			if(Value==""){
				return AddResult.Ok;
			}else if(Value=="for"||Value=="while"){
				return new ForFragment(sr,Value).AddTo(to,sr);
			}else if(Value=="if"){
				return new IfFragment(sr).AddTo(to,sr);
			}else if(Value=="else"){
				CodeFragment previous=to.ParentFragment;
				// Parent->prev operation->last object. Should be an IF.
				if(previous==null||((previous=previous.LastChild)==null)||((previous=previous.LastChild)==null)||previous.GetType()!=typeof(IfFragment)){
					Error("Else can only be applied to an if. E.g. if(){}else{}.");
				}
				IfFragment ifFrag=(IfFragment)previous;
				ifFrag.ApplyElseTo.IfFalse=new BracketFragment(sr);
				return AddResult.Stop;
			}else if(Value=="elseif"){
				CodeFragment previous=to.ParentFragment;
				// Parent->prev operation->last object. Should be an IF.
				if(previous==null||((previous=previous.LastChild)==null)||((previous=previous.LastChild)==null)||previous.GetType()!=typeof(IfFragment)){
					Error("Else if can only be applied to an if. E.g. if(){}else if{}..");
				}
				IfFragment ifFrag=(IfFragment)previous;
				IfFragment newfrag=new IfFragment(sr);
				
				 BracketFragment bf=new BracketFragment();
				  OperationFragment op=new OperationFragment();
				  op.AddChild(newfrag);
				 bf.AddChild(op);
				ifFrag.ApplyElseTo.IfFalse=bf;
				ifFrag.ApplyElseTo=newfrag;
				return AddResult.Stop;
			}else{
				return base.AddTo(to,sr);
			}
		}
		
		public override CompiledFragment Compile(CompiledMethod function){
			if(Value=="null"){
				return new CompiledFragment(null);
			}else if(Value=="base"){
				return new BaseOperation(function);
			}else if(Value=="true"||Value=="false"){
				return new CompiledFragment(Value=="true");
			}else if(Value=="new"){
				Error("A constructor is missing its brackets.");
				return null;
			}else if(IsKeyword()){
				return KeyWords.Compile(this,function);
			}else{
				// It isn't a keyword (if,for..)
				bool isSet=(GivenType!=null||(NextChild!=null&&Types.IsTypeOf(NextChild,typeof(OperatorSet))));
				Type varType=null;
				Variable var=function.GetVariable(Value);
				if(isSet){
					if(var==null){
						if(GivenType==null){
							Error("Currently all variables must be given a type.");
						}
						varType=GivenType.FindType(function.Script);
						if(varType==null){
							Error("Type '"+GivenType.Value+"' was not found.");
						}
						var=function.GetVariable(Value,isSet,varType);
					}
				}
				if(var!=null){
					return new CompiledFragment(var);
				}
				return new PropertyOperation(function,Value);
			}
		}
		
		public override string ToString(){
			string result=Value;
			if(GivenType!=null){
				result+=GivenType.ToString();
			}
			if(NextChild!=null){
				Type t=NextChild.GetType();
				if(t!=typeof(OperatorFragment)){
					result+=" ";
				}
			}
			return result;
		}
		
	}
	
}