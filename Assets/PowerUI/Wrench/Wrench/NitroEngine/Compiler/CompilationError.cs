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

namespace Nitro{

	/// <summary>
	/// Represents an error in the compilation of some nitro code.
	/// </summary>

	public class CompilationException:Exception{
		
		/// <summary>The line number the error occurred on.</summary>
		public int LineNumber;
		
		/// <summary>Creates a new compilation exception with the given line number the error occured on and a message to show.</summary>
		/// <param name="lineNumber">The line number the error occured on.</param>
		/// <param name="errorMessage">A message stating why this error has occured.</param>
		public CompilationException(int lineNumber,string errorMessage):base(errorMessage){
			LineNumber=lineNumber;	
		}
		
		public override string ToString(){
			// Intentionally hides the full stack trace.
			string line=LineNumber.ToString();
			if(LineNumber==-1){
				line="Unknown";
			}
			return "Line "+line+": "+Message;
		}
		
	}
	
}