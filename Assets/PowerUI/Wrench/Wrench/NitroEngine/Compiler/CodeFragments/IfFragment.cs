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
	/// Represents an if statement.
	/// </summary>
	
	public class IfFragment:CodeFragment{
		
		/// <summary>The fragment that any following else or else if conditions should be applied to.</summary>
		public IfFragment ApplyElseTo;
		/// <summary>The block to run if the conditions are true.</summary>
		public BracketFragment IfTrue;
		/// <summary>The block to run if the conditions are false.</summary>
		public BracketFragment IfFalse;
		/// <summary>The conditions to check if true/false.</summary>
		public BracketFragment Condition;
		
		
		/// <summary>Creates and reads a new if(){} fragment. Note that the else section is not read.</summary>
		/// <param name="sr">The lexer to read the if condition and its true bracket block from.</param>
		public IfFragment(CodeLexer sr){
			ApplyElseTo=this;
			Condition=new BracketFragment(sr);
			IfTrue=new BracketFragment(sr);
			
			// Add them as children such that code tree iterators can visit them:
			AddChild(Condition);
			AddChild(IfTrue);
			
		}
		
		public void SetIfFalse(BracketFragment ifFalse){
			
			IfFalse=ifFalse;
			
			// Add as child such that code tree iterators can visit them:
			AddChild(IfFalse);
			
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			base.AddTo(to,sr);
			return AddResult.Stop;
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			return new IfOperation(method,Condition,IfTrue,IfFalse);
		}
		
		public override string ToString(){
			string result="if"+Condition.ToString()+IfTrue.ToString();
			
			if(IfFalse!=null){
				result+="else"+IfFalse.ToString();
			}
			
			return result;
		}
		
	}
	
}