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

	public static class CharMapTables{
		
		public static bool Load(FontParser parser,int start,FontFace font,Glyph[] glyphs){
			
			// Seek there:
			parser.Position=start;
			
			// Read the version (and check if it's zero):
			if(parser.ReadUInt16()!=0){
				return false;
			}
			
			// Strangely the cmap table can have lots of tables inside it. For now we're looking for the common type 3 table.
			
			// Number of tables:
			int tableCount=parser.ReadUInt16();
			
			// Total characters in font:
			int characterCount=0;
			
			int offset=-1;
			int favour=0;
			
			for(int i = 0; i < tableCount; i += 1) {
				
				// Grab the platform ID:
				int platformId=parser.ReadUInt16();
				
				// And the encoding ID:
				int encodingId=parser.ReadUInt16();
				
				if(platformId==3 || platformId==0){
					
					if(encodingId==10){
						
						// Top favourite - most broad Unicode encoding.
						
						// Read offset:
						offset=(int)parser.ReadUInt32();
						
						break;
						
					}else if(encodingId==1 || encodingId==0){
						
						// Read offset:
						offset=(int)parser.ReadUInt32();
						
						// Mid-range favourite:
						favour=1;
						
						continue;
						
					}else if(favour==0){
						
						// Anything else we'll give a try:
						
						// Read offset (but don't break):
						offset=(int)parser.ReadUInt32();
						
						continue;
						
					}
					
				}
				
				// Skip offset:
				parser.Position+=4;
				
			}
			
			if(offset==-1){
				// We don't support this font :(
				return false;
			}
			
			// Seek to the cmap now:
			parser.Position=start+offset;
			
			// Check it's format 4:
			int format=parser.ReadUInt16();
		
			if(format>6){
				// We now have e.g. 12.0 - another short here:
				parser.Position+=2;
				
				// Size/ structure are both 4 byte ints now:
				parser.Position+=8;
				
			}else{
			
				// Size of the sub-table (map length, u16):
				parser.Position+=2;
				
				// Structure of the sub-table (map language, u16):
				parser.Position+=2;
				
			}
			
			switch(format){
				
				case 0:
				
				// Byte encoding table:
				
				for(int i=0;i<256;i++){
					
					int rByte=parser.ReadByte();
					
					Glyph glyph=glyphs[rByte];
					
					if(glyph!=null){
						characterCount++;
					
						glyph.AddCharcode(i);
					}
					
				}
				
				break;
				case 2:
				
				// The offset to the headers:
				int subOffset=parser.Position + (256 * 2);
				
				// For each high byte:
				for(int i=0;i<256;i++){
					
					// Read the index to the header and zero out the bottom 3 bits:
					int headerPosition=subOffset + (parser.ReadUInt16() & (~7));
					
					// Read the header:
					int firstCode=parser.ReadUInt16(ref headerPosition);
					int entryCount=parser.ReadUInt16(ref headerPosition);
					short idDelta=parser.ReadInt16(ref headerPosition);
					
					// Grab the current position:
					int pos=headerPosition;
					
					// Read the idRangeOffset - the last part of the header:
					pos+=parser.ReadUInt16(ref headerPosition);
					
					int maxCode=firstCode+entryCount;
					
					// Get the high byte value:
					int highByte=(i<<8);
					
					// For each low byte:
					for (int j=firstCode;j<maxCode;j++){
						
						// Get the full charcode (which might not actually exist yet):
						int charCode=highByte+j;
						
						// Read the base of the glyphIndex:
						int p=parser.ReadUInt16(ref pos);
						
						if(p==0){
							continue;
						}
						
						p=(p+idDelta) & 0xFFFF;
						
						if(p==0){
							continue;
						}
						
						Glyph glyph=glyphs[p];
						
						if(glyph!=null){
							
							characterCount++;
							
							glyph.AddCharcode(charCode);
							
						}
						
					}
				}
				
				break;
				case 4:
				
				// Segment count. It's doubled.
				int segCount=(parser.ReadUInt16() >> 1);
				
				// Search range, entry selector and range shift (don't need any):
				parser.Position+=6;
				
				int baseIndex=parser.Position;
				
				int endCountIndex=baseIndex;
				
				baseIndex+=2;
				
				int startCountIndex = baseIndex + segCount * 2;
				int idDeltaIndex = baseIndex + segCount * 4;
				int idRangeOffsetIndex = baseIndex + segCount * 6;
				
				for(int i = 0; i < segCount - 1; i ++){
					
					int endCount = parser.ReadUInt16(ref endCountIndex);
					int startCount = parser.ReadUInt16(ref startCountIndex);
					int idDelta = parser.ReadInt16(ref idDeltaIndex);
					int idRangeOffset = parser.ReadUInt16(ref idRangeOffsetIndex);
					
					for(int c = startCount; c <= endCount;c++){
						
						int glyphIndex;
						
						if(idRangeOffset != 0){
							
							// The idRangeOffset is relative to the current position in the idRangeOffset array.
							// Take the current offset in the idRangeOffset array.
							int glyphIndexOffset = (idRangeOffsetIndex - 2);
							
							// Add the value of the idRangeOffset, which will move us into the glyphIndex array.
							glyphIndexOffset += idRangeOffset;
							
							// Then add the character index of the current segment, multiplied by 2 for USHORTs.
							glyphIndexOffset += (c - startCount) * 2;
							
							glyphIndex=parser.ReadUInt16(ref glyphIndexOffset);
							
							if(glyphIndex!=0){
								glyphIndex = (glyphIndex + idDelta) & 0xFFFF;
							}
							
						}else{
							glyphIndex = (c + idDelta) & 0xFFFF;
						}
						
						// Add a charcode to the glyph now:
						Glyph glyph=glyphs[glyphIndex];
						
						if(glyph!=null){
							characterCount++;
						
							glyph.AddCharcode(c);
						}
					}
					
				}
				
				break;
				
				case 6:
				
				int firstCCode=parser.ReadUInt16();
				int entryCCount=parser.ReadUInt16();
				
				for(int i=0;i<entryCCount;i++){
					
					Glyph glyphC=glyphs[parser.ReadUInt16()];
					
					if(glyphC!=null){
					
						characterCount++;
						
						glyphC.AddCharcode(firstCCode+i);
					
					}
					
				}
				
				break;
				
				case 12:
				
				int groups=(int)parser.ReadUInt32();
				
				for(int i=0;i<groups;i++){
					int startCode=(int)parser.ReadUInt32();
					int endCode=(int)parser.ReadUInt32();
					int startGlyph=(int)parser.ReadUInt32();
					
					int count=(endCode - startCode);
					
					for(int j=0;j<=count;j++){
						
						int glyphIndex=(startGlyph+j);
						
						Glyph glyph=glyphs[glyphIndex];
						
						if(glyph!=null){
							
							characterCount++;
							
							glyph.AddCharcode(startCode+j);
							
						}
						
					}
				}
				
				break;
				
				default:
					Fonts.OnLogMessage("InfiniText does not currently support this font. If you need it, please contact us with this: Format: "+format);
				break;
			}
			
			font.CharacterCount=characterCount;
			
			return true;
			
		}
		
	}

}