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


namespace InfiniText{
	
	/// <summary>
	/// Loads glyph substitution tables (ligatures).
	/// Ligatures are language sensitive. Each language group defines a bunch of "features" a font supports.
	/// Features can be, for example, turning 1/2 into a fraction; the "frac" feature.
	/// </summary>
	
	public static class GsubTables{
		
		public static void Load(FontParser parser,int offset,FontFace font){
			
			// Seek there:
			parser.Position=offset;
			
			// Version:
			parser.ReadVersion();
			
			// Script list (directed by UI Language setting):
			int scriptList=parser.ReadUInt16();
			
			// Feature list:
			int featList=parser.ReadUInt16();
			
			// Lookup list:
			int lookList=parser.ReadUInt16();
			
			// Goto script list:
			int objectOffset=scriptList+offset;
			
			parser.Position=objectOffset;
			
			/*
			// How many language scripts?
			int scriptCount=parser.ReadUInt16();
			
			for(int i=0;i<scriptCount;i++){
				
				// Read the script code:
				string scrName=parser.ReadString(4);
				
				// And it's offset:
				int scriptOffset=parser.ReadUInt16()+objectOffset;
				
				int retPosition=parser.Position;
				
				// Seek and load it right away:
				parser.Position=scriptOffset;
				
				// What's the default lang?
				int defaultLangOffset=parser.ReadUInt16();
				
				// How many languages?
				int langCount=parser.ReadUInt16();
				
				for(int l=0;l<langCount;l++){
					
					// Read the lang code:
					string langName=parser.ReadString(4);
					
					// And it's offset - points to list of features:
					int langOffset=parser.ReadUInt16()+objectOffset;
					
				}
				
				parser.Position=retPosition;
				
			}
			*/
			
			// Goto lookup list:
			objectOffset=lookList+offset;
			
			// Seek there:
			parser.Position=objectOffset;
			
			// Load each one:
			int lookCount=parser.ReadUInt16();
			
			// Create:
			LigatureLookupTable[] lookupTables=new LigatureLookupTable[lookCount];
			
			for(int i=0;i<lookCount;i++){
				
				// Create the table:
				LigatureLookupTable table=new LigatureLookupTable();
				
				// Load it:
				int tableOffset=parser.ReadUInt16();
				
				// Cache the position:
				int seekPos=parser.Position;
				
				// Head over to the table:
				parser.Position=objectOffset+tableOffset;
				
				// Load it now:
				table.Load(parser);
				
				// Add to set:
				lookupTables[i]=table;
				
				// Restore position:
				parser.Position=seekPos;
				
			}
			
			// Goto feature list:
			objectOffset=featList+offset;
			
			parser.Position=objectOffset;
			
			// How many features? For now, "liga" is the main feature we're after here.
			int featureCount=parser.ReadUInt16();
			
			for(int i=0;i<featureCount;i++){
				
				// Read the feature code:
				string feature=parser.ReadString(4);
				
				// Table offset:
				int featTable=parser.ReadUInt16();
				
				switch(feature){
					
					case "locl":
					case "liga":
						
						AddToFont(feature,parser,featTable+objectOffset,font,lookupTables);
						
					break;
					
				}
				
			}
			
		}
		
		/// <summary>Adds a ligature table to the font.</summary>
		public static void AddToFont(string name,FontParser parser,int tableOffset,FontFace font,LigatureLookupTable[] tables){
			
			int position=parser.Position;
			
			// Skip params (+2):
			parser.Position=tableOffset+2;
			
			// Index count:
			int count=parser.ReadUInt16();
			
			// For each one..
			for(int i=0;i<count;i++){
				
				// Grab the index:
				//int index=parser.ReadUInt16();
				
				// Grab the table:
				//LigatureLookupTable table=tables[index];
				
			}
			
			parser.Position=position;
			
		}
		
	}

}