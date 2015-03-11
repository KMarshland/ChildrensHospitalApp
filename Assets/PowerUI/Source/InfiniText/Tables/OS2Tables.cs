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

	public static class OS2Tables{
		
		public static void Load(FontParser parser,int offset,FontFace font){
			
			// Got OS2:
			parser.ReadOS2=true;
			
			// Seek:
			parser.Position=offset;
			
			// version
			int version=parser.ReadUInt16();
			
			// xAvgCharWidth
			parser.ReadInt16();
			
			// usWeightClass
			int weight=parser.ReadUInt16();
			
			// usWidthClass
			parser.ReadUInt16();
			
			// fsType
			parser.ReadUInt16();
			
			// ySubscriptXSize
			parser.ReadInt16();
			
			// ySubscriptYSize
			parser.ReadInt16();
			
			// ySubscriptXOffset
			parser.ReadInt16();
			
			// ySubscriptYOffset
			parser.ReadInt16();
			
			// ySuperscriptXSize
			parser.ReadInt16();
			
			// ySuperscriptYSize
			parser.ReadInt16();
			
			// ySuperscriptXOffset
			parser.ReadInt16();
			
			// ySuperscriptYOffset
			parser.ReadInt16();
			
			// yStrikeoutSize
			font.StrikeSize=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// yStrikeoutPosition
			font.StrikeOffset=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// sFamilyClass
			parser.ReadInt16();
			
			// panose:
			/*
			byte panose=new byte[10];
			
			for(int i=0;i<10;i++){
				panose[i]=parser.ReadByte();
			}
			*/
			parser.Position+=10;
			
			// ulUnicodeRange1
			parser.ReadUInt32();
			
			// ulUnicodeRange2
			parser.ReadUInt32();
			
			// ulUnicodeRange3
			parser.ReadUInt32();
			
			// ulUnicodeRange4
			parser.ReadUInt32();
			
			// achVendID
			parser.ReadTag();
			
			// fsSelection
			int type=parser.ReadUInt16();
			
			bool italic=((type&1)==1);
			// bool strikeout=((type&16)==16);
			// bool underscore=((type&2)==2);
			bool oblique=((type&512)==512);
			bool bold=((type&32)==32);
			bool regular=((type&64)==64);
			bool useTypo=((type&128)==128);
			
			if(!bold || regular){
				// Must be regular:
				weight=400;
			}else if(weight==0){
				weight=700;
			}
			
			font.SetFlags(italic || oblique,weight);
			
			// usFirstCharIndex
			parser.ReadUInt16();
			
			// usLastCharIndex
			parser.ReadUInt16();
			
			// sTypoAscender
			float ascender=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// sTypoDescender
			float descender=(float)parser.ReadInt16()/font.UnitsPerEmF;
			
			// sTypoLineGap
			float lineGap=((float)parser.ReadInt16()/font.UnitsPerEmF);
			
			// usWinAscent
			float wAscender=(float)parser.ReadUInt16()/font.UnitsPerEmF;
			
			// usWinDescent
			float wDescender=(float)parser.ReadUInt16()/font.UnitsPerEmF;
			
			// This is an awkward spec hack due to most windows programs
			// providing the wrong typo descender values.
		
			if(Fonts.UseOS2Metrics){
				
				if(useTypo){
					
					float halfGap=lineGap/2f;
				
					font.Ascender=ascender + halfGap;
					font.Descender=halfGap-descender;
					
					font.LineGap=font.Ascender + font.Descender;
					
				}else{
					font.Ascender=wAscender;
					font.Descender=wDescender;
					
					font.LineGap=font.Ascender + font.Descender;
				}
				
			}
			
			
			if (version >= 1){
				// ulCodePageRange1
				parser.ReadUInt32();
				
				// ulCodePageRange2
				parser.ReadUInt32();
			}
			
			if (version >= 2){
				// sxHeight
				parser.ReadInt16();
				
				// sCapHeight
				parser.ReadInt16();
				
				// usDefaultChar
				parser.ReadUInt16();
				
				// usBreakChar
				parser.ReadUInt16();
				
				// usMaxContent
				parser.ReadUInt16();
			}
			
			
		}
		
	}

}