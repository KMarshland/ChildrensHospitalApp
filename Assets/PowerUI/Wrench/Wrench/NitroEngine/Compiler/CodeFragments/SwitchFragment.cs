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
	/// Represents a switch block.
	/// </summary>
	
	public class SwitchFragment:CodeFragment{
		
		/// <summary>The block code.</summary>
		public BracketFragment Body;
		/// <summary>The parameters of the block.</summary>
		public BracketFragment Parameters;
		
		/// <summary>Creates and reads a new switch fragment.</summary>
		/// <param name="sr">The lexer to read the switch content from.</param>
		public SwitchFragment(CodeLexer sr){
			Parameters=new BracketFragment(sr);
			Body=new BracketFragment(sr);
		}
		
		public override AddResult AddTo(CodeFragment to,CodeLexer sr){
			base.AddTo(to,sr);
			return AddResult.Stop;
		}
		
		public override CompiledFragment Compile(CompiledMethod method){
			return new SwitchOperation(method,Parameters,Body);
		}
		
		public override string ToString(){
			return "switch"+Parameters.ToString()+Body.ToString();
		}
		
	}
	
}