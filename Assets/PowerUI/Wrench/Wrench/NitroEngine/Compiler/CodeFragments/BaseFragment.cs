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
	/// Represents the root node/fragment of all code.
	/// </summary>

	public class BaseFragment:BracketFragment{

		public BaseFragment(CodeLexer sr):base(sr,false){}
		
	}
	
}