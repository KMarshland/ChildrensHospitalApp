//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//   Kulestar would like to thank the following:
//    PDF.js, Microsoft, Adobe and opentype.js
//    For providing implementation details and
// specifications for the TTF and OTF file formats.
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{

	public class LigatureLookupTable{
		
		/// <summary>The substitution set. Indexed by the first character in the substitution.</summary>
		public Dictionary<int,List<LigatureSubstitution>> Substitutions;
		
		
		public void Load(FontParser parser){
			
			int start=parser.Position;
			
			// Table type:
			parser.ReadUInt16();
			
			parser.ReadUInt16();
			
			int count=parser.ReadUInt16();
			
			// create the set:
			Substitutions=new Dictionary<int,List<LigatureSubstitution>>();
			
			// For each one..
			for(int i=0;i<count;i++){
				
				// Get the offset:
				int offset=parser.ReadUInt16();
				
				// Create the substitution entry:
				LigatureSubstitution substitution=new LigatureSubstitution();
				
				// Get the current position:
				int position=parser.Position;
				
				// Hop there:
				parser.Position=start+offset;
				
				// Load it:
				substitution.Load(parser);
				
				// Go back:
				parser.Position=position;
				
			}
			
			// Extra flags:
			parser.ReadUInt16();
			
		}
		
	}
	
}