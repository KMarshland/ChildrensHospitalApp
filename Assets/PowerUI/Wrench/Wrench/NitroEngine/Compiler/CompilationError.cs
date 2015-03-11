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
		public int LineNumber=-1;
		
		/// <summary>Creates a new compilation exception with the given line number the error occured on and a message to show.</summary>
		/// <param name="lineNumber">The line number the error occured on.</param>
		/// <param name="errorMessage">A message stating why this error has occured.</param>
		public CompilationException(string errorMessage):base(errorMessage){	
		}
		
		public override string ToString(){
			// Intentionally hides the full stack trace.
			string line;
			
			if(LineNumber==-1){
				line="Unknown";
			}else{
				line=LineNumber.ToString();
			}
			
			return "Line "+line+": "+Message;
		}
		
	}
	
}