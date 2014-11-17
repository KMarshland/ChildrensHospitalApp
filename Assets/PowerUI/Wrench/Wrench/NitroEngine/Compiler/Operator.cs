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

namespace Nitro{

	/// <summary>
	/// Base class of all operators (+,- etc) and a global lookup for finding an operator instance from its text.
	/// When the system starts, all operators are automatically discovered by looking for any Operator classes.
	/// You can essentially create your own by simply deriving the Operator class.
	/// </summary>

	public class Operator{
	
		/// <summary>The first character of an operator (as some have 2 or more; e.g. +=, &&)</summary>
		public static Dictionary<char,int> Starts=new Dictionary<char,int>();
		/// <summary>A lookup for an operators text to the operator instance. e.g. '+' to the add operator.</summary>
		public static Dictionary<string,Operator> FullOperators=new Dictionary<string,Operator>();
		
		/// <summary>Adds a useable operator to the global lookup.</summary>
		/// <param name="newOperator">The operator to add.</param>
		public static void Add(Operator newOperator){
			string pattern=newOperator.Pattern;
			// The first character is cached for fast lookup purposes.
			char first=pattern[0];
			if(!Starts.ContainsKey(first)){
				Starts.Add(first,0);
			}
			FullOperators.Add(pattern,newOperator);
		}
		
		
		/// <summary>The priority of this operator over others. Essentially implements bodmas.</summary>
		public int Priority;
		/// <summary>True if this operator only uses the content to its left. e.g. LEFT++.</summary>
		public bool LeftOnly;
		/// <summary>True if this operator only uses the content to its right. e.g. !RIGHT.</summary>
		public bool RightOnly;
		/// <summary>The operator text pattern, for example '+'</summary>
		public string Pattern;
		/// <summary>True if this operator uses the content on both the left and right of it. e.g. LEFT+RIGHT.</summary>
		public bool LeftAndRight=true;
		
		/// <summary>Creates a new operator with the given text and priority.</summary>
		/// <param name="pattern">The operators text, such as "+" or "++".</param>
		/// <param name="priority">The priority over other operators. The higher this value, the </param>
		public Operator(string pattern,int priority){
			Pattern=pattern;
			Priority=priority;
		}
		
		/// <summary>Converts the the given fragments into a compiled operation by first checking the fragments are ok for this operator.</summary>
		/// <param name="left">The fragment to the left of the operator.</param>
		/// <param name="right">The fragment to the right of the operation.</param>
		/// <param name="method">The method the operation will be compiled into.</param>
		public Operation ToOperation(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			bool leftNull=(left==null);
			bool rightNull=(right==null);
			if(leftNull&&rightNull){
				return null;
			}
			if(leftNull&&!rightNull&&!RightOnly){
				return null;
			}
			if(!leftNull&&rightNull&&!LeftOnly){
				return null;
			}
			if(!leftNull&&!rightNull&&!LeftAndRight){
				return null;
			}
			return Compile(left,right,method);
		}
		
		/// <summary>Converts the given fragments into a compiled operation. Overidden by the actual operators.</summary>
		/// <param name="left">The fragment to the left of the operator.</param>
		/// <param name="right">The fragment to the right of the operation.</param>
		/// <param name="method">The method the operation will be compiled into.</param>
		protected virtual Operation Compile(CompiledFragment left,CompiledFragment right,CompiledMethod method){
			return null;
		}
		
	}
	
}