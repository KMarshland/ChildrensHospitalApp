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
	/// Represents a for or while loop.
	/// </summary>
	
	public class ForFragment:CodeFragment{
	
		/// <summary>This class also represents while loops; Name is either 'for' or 'while'.</summary>
		public string Value;
		/// <summary>The loop code.</summary>
		public BracketFragment Body;
		/// <summary>The parameters of the loop.</summary>
		public BracketFragment Parameters;
		
		/// <summary>Creates and reads a new for/while loop fragment.</summary>
		/// <param name="sr">The lexer to read the loop content from.</param>
		/// <param name="name">This class represents while too. This is the name of the loop; either 'for' or 'while'.</param>
		public ForFragment(CodeLexer sr,string name){
			Value=name;
			Parameters=new BracketFragment(sr);
			Body=new BracketFragment(sr);
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			base.AddTo(to,sr);
			return AddResult.Stop;
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			return new ForOperation(method,Parameters,Body);
		}
		
		public override string ToString(){
			return Value+Parameters.ToString()+Body.ToString();
		}
		
	}
	
}