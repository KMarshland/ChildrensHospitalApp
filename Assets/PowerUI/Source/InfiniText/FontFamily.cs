//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{
	
	/// <summary>A font family is a collection of font faces (Font objects). 
	/// A font family is e.g. Arial, and it's faces are e.g. bold and italic variants.</summary>
	
	public class FontFamily{
		
		/// <summary>The name of this family.</summary>
		public string Name;
		/// <summary>The regular "default" font. Always set (first font to join).</summary>
		public FontFace Regular;
		/// <summary>True if all faces of this font should disable their extrusion. Enabled by default, but it can make some rare fonts go streaky.</summary>
		public bool DisableExtrude;
		/// <summary>Set of all bold faces that are not italic.</summary>
		public List<FontFace> Bold;
		/// <summary>Set of all italic faces.</summary>
		public List<FontFace> Italics;
		/// <summary>See InvertNormals.</summary>
		public bool InvertedNormals=false;
		/// <summary>All self-declared italic fonts.</summary>
		public Dictionary<FontFaceFlags,FontFace> FontFaces=new Dictionary<FontFaceFlags,FontFace>();
		
		
		public FontFamily(string name){
			Name=name;
			DisableExtrude=Fonts.DisableExtrude;
			InvertedNormals=Fonts.InvertedNormals;
		}
		
		/// <summary>Adds the given font face to this family.</summary>
		public void Add(FontFace font){
			
			font.DisableExtrude=DisableExtrude;
			
			// Grab the flags which represent the styling of this font:
			FontFaceFlags flags=font.StyleFlags;
			
			// Push:
			FontFaces[flags]=font;
			
			if(Regular==null || font.Regular){
				Regular=font;
			}
			
			bool italic=font.Italic;
			
			if(font.Bold && !italic){
				
				if(Bold==null){
					Bold=new List<FontFace>();
				}
				
				Bold.Add(font);
				
			}
			
			if(italic){
				
				if(Italics==null){
					Italics=new List<FontFace>();
				}
				
				Italics.Add(font);
				
			}
			
		}
		
		/// <summary>Use this to disable or enable SDF extrusion. In some rare cases, it can make fonts go streaky. Enabled by default.</summary>
		public void SetExtrude(bool active){
			
			if(active==DisableExtrude){
				return;
			}
			
			DisableExtrude=active;
			
			foreach(KeyValuePair<FontFaceFlags,FontFace> kvp in FontFaces){
				
				kvp.Value.DisableExtrude=active;
				
			}
			
		}
		
		/// <summary>Use this to invert the normals of the glyphs in this face. Can be used to make letters go more/less bold.</summary>
		public void InvertNormals(){
			
			InvertedNormals=!InvertedNormals;
			
			foreach(KeyValuePair<FontFaceFlags,FontFace> kvp in FontFaces){
				
				kvp.Value.InvertNormals();
				
			}
			
		}
		
		/// <summary>Gets or synthesises a glyph for the given style settings which include weight and italic. Style code 400 means regular.</summary>
		public Glyph GetGlyph(int charcode,FontFaceFlags style){
			
			// Try getting the face:
			FontFace face=null;
			
			if(FontFaces.TryGetValue(style,out face)){
				
				// Great! Raster using that font face:
				return face.GetGlyph(charcode);
				
			}
			
			// Get the code as an int:
			int styleCode=(int)style;
			
			// Italic?
			bool italic=((styleCode&1)==1);
			
			// Get the font weight:
			int weight=(styleCode&~1);
			
			if(weight==0){
				// Regular:
				weight=400;
			}
			
			int match=0;
			
			if(italic){
				
				// Find one with the closest weight match.
				face=BestWeight(Italics,weight,out match);
				
			}else if(weight==400){
				
				// Regular. Note that regular must be set - the family wouldn't exist otherwise.
				return Regular.GetGlyph(charcode);
				
			}else{
				
				// Find best weight match. Must also be not italic (this is why the bold set doesn't contain italic ones).
				face=BestWeight(Bold,weight,out match);
				
			}
			
			if(face==null || match!=0){
				
				// We're synthesizing!
				
				if(face==null){
					face=Regular;
				}
				
				// Derive from face.
				face=face.CreateSynthetic(italic,weight);
				
			}
			
			return face.GetGlyph(charcode);
			
		}
		
		public FontFace BestWeight(List<FontFace> faces,int weight,out int bestWeightDiff){
			
			bestWeightDiff=int.MaxValue;
			
			if(faces==null){
				return null;
			}
			
			FontFace selected=null;
			
			foreach(FontFace font in faces){
				
				int diff=font.Weight-weight;
				
				if(diff==0){
					
					// Clear best:
					bestWeightDiff=0;
					
					// Direct match.
					selected=font;
					
					break;
					
				}else if(diff<bestWeightDiff){
					
					// Better match.
					bestWeightDiff=diff;
					
					selected=font;
					
				}
				
			}
			
			return selected;
			
		}
		
	}

}