//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using Blaze;


namespace PowerUI.Css{
	
	/// <summary>
	/// This class manages rendering text to the screen.
	/// </summary>
	
	public partial class TextRenderingProperty:DisplayableProperty{
		
		/// <summary>Font weight (boldness). 400 is regular.</summary>
		public int Weight=400;
		/// <summary>True if the text should be italic.</summary>
		public bool Italic;
		/// <summary>The string to render. Note that this is parsed
		/// into <see cref="PowerUI.Css.TextRenderingProperty.Characters"/>.</summary>
		public string Text;
		/// <summary>The size of the font in pixels.</summary>
		public float FontSize;
		/// <summary>How wide a space should be in pixels.</summary>
		public float SpaceSize;
		/// <summary>The colour that the font should be without the colour overlay.</summary>
		public Color BaseColour;
		/// <summary>The base colour combined with the colour overlay.</summary>
		public Color FontColour;
		/// <summary>Should text be automatically aliased? Auto is MaxValue. Set with font-smoothing.</summary>
		public float Alias=float.MaxValue;
		/// <summary>The ascender for the font with the current gap applied.</summary>
		public float Ascender=1f;
		/// <summary>The gap to apply around lines as an em value.</summary>
		public float LineGap=0.2f;
		/// <summary>True if all characters are whitespaces. No batches will be generated.</summary>
		public bool AllWhitespace;
		/// <summary>A scale factor converting the SDF rasters to FontSize units tall.</summary>
		private float ScaleFactor;
		/// <summary>Additional spacing to apply around letters.</summary>
		public float LetterSpacing;
		/// <summary>How and where a line should be drawn if at all (e.g. underline, overline etc.)</summary>
		public TextDecorationInfo TextLine;
		/// <summary>The font to use when rendering.</summary>
		public DynamicFont FontToDraw;
		/// <summary>The number of punctiation characters at the end of the word.</summary>
		public int EndPunctuationCount;
		/// <summary>The number of punctiation characters at the start of the word.</summary>
		public int StartPunctuationCount;
		/// <summary>The set of characters to render. Note that the characters are shared globally.</summary>
		public Glyph[] Characters;
		/// <summary>Kern values for this text, if it has any. Created only if it's needed.</summary>
		private float[] Kerning;
		
		
		/// <summary>Creates a new text rendering property. Note that this must not be called directly.
		/// Set content: instead; if you're doing that from a tag, take a look at BR.</summary>
		/// <param name="element">The element that this is rendering text for.</param>
		public TextRenderingProperty(Element element):base(element){}
		
		
		/// <summary>The width in pixels of the last whitespace of this element, if it's got one.</summary>
		public int EndSpaceSize{
			get{
				
				if(Characters==null || Characters.Length==0){
					return 0;
				}
				
				Glyph character=Characters[Characters.Length-1];
				
				if(character!=null && character.Space){
					// It ends in a space! Return the width of a space as measured by this renderer.
					return (int)SpaceSize;
				}
				
				return 0;
			}
		}
		
		/// <summary>Gets how many letters are being rendered.</summary>
		/// <returns>The number of letters.</returns>
		public int LetterCount(){
			return Text.Length;
		}
		
		/// <summary>Recomputes the space size and inner height of the parent element.</summary>
		public void SetDimensions(){
			
			if(FontToDraw==null || Characters==null){
				return;
			}
			
			float width=0f;
			float size=FontSize;
			float screenHeight=FontToDraw.GetHeight(size) * (1f + LineGap);
			
			Ascender=FontToDraw.GetAscend(size) + (LineGap * size / 2f);
			
			ScaleFactor=size / Fonts.Rasteriser.ScalarX;
			
			ComputedStyle computed=Element.Style.Computed;
			computed.FixedHeight=true;
			
			if(SpaceSize==0f){
				SpaceSize=StandardSpaceSize();
			}
			
			for(int i=0;i<Characters.Length;i++){
				Glyph dChar=Characters[i];
				
				if(dChar==null){
					continue;
				}
				
				if(dChar.Image!=null){
					
					if(CharacterProviders.FixHeight){
						if(dChar.Height>screenHeight){
							screenHeight=dChar.Height;
						}
						
						width+=dChar.Width;
					}else{
						
						width+=FontSize;
						
					}
					
				}else if(dChar.Space){
					width+=SpaceSize;
				}else{
					width+=dChar.AdvanceWidth * size;
				}
				
				width+=LetterSpacing;
			}
			
			computed.InnerHeight=(int)screenHeight;
			computed.InnerWidth=(int)width;
			computed.FixedWidth=true;
			computed.SetSize();
		}
		
		/// <summary>The base space size for this text.</summary>
		public float StandardSpaceSize(){
			if(FontToDraw==null){
				return FontSize/3f;
			}
			
			return FontToDraw.GetSpaceSize(FontSize);
		}
		
		public override void SetOverlayColour(Color colour){
			FontColour=BaseColour*colour;
			
			if(TextLine!=null && TextLine.ColourOverride){
				TextLine.SetOverlayColour(colour);
			}
			
			RequestPaint();
		}
		
		protected override void NowOffScreen(){
			
			if(Characters==null){
				return;
			}
			
			foreach(Glyph character in Characters){
				
				if(character==null){
					continue;
				}
				
				character.OffScreen();
			}
			
		}
		
		protected override bool NowOnScreen(){
			
			if(Characters==null){
				return false;
			}
			
			foreach(Glyph character in Characters){
				
				if(character==null){
					continue;
				}
				
				character.OnScreen();
			}
			
			return true;
			
		}
		
		/// <summary>Called when an @font-face font fully loads.</summary>
		public void FontLoaded(DynamicFont font){
			
			if(FontToDraw==null || Characters==null){
				return;
			}
			
			if(FontToDraw==font){
				Kerning=null;
				SpaceSize=0f;
				SetText();
				
				if(Visible){
					NowOnScreen();
				}
				
				return;
				
			}
				
			DynamicFont fallback=FontToDraw.Fallback;
			while(fallback!=null){
				
				if(fallback==font){
					// Fallback font now available - might change the rendering.
					Kerning=null;
					SpaceSize=0f;
					SetText();
					
					if(Visible){
						NowOnScreen();
					}
					
					return;
				}
				
				fallback=fallback.Fallback;
			}
			
		}
		
		/// <summary>Loads the character array (<see cref="PowerUI.Css.TextRenderingProperty.Characters"/>) from the text string.</summary>
		public void SetText(){
			
			if(Text==null || FontToDraw==null){
				// Can't do it just yet - we have no text/ font to use.
				return;
			}
			
			char[] characters=Text.ToCharArray();
			
			Characters=new Glyph[characters.Length];
			
			// Find the style ID for our properties:
			int fontStyle=Weight;
			
			if(Italic){
				fontStyle|=1;
			}
			
			FontFaceFlags fontStyleID=(FontFaceFlags)fontStyle;
			
			// The number of punctuation marks in a row that have been spotted.
			int punctuationCount=0;
			// This goes true when we hit the first non-punctuation value in the word.
			bool endedStartPunctuation=false;
			
			StartPunctuationCount=0;
			
			// Considered all whitespaces until shown otherwise.
			AllWhitespace=true;
			
			// Next, for each character, find its dynamic character.
			// At the same time we want to find out what dimensions this word has so it can be located correctly.
			Glyph previous=null;
			
			for(int i=0;i<characters.Length;i++){
				char rawChar=characters[i];
				
				Glyph character=null;
				
				// Is it a unicode high/low surrogate pair?
				if(char.IsHighSurrogate(rawChar) && i!=characters.Length-1){
					// Low surrogate follows:
					char lowChar=characters[i+1];
					
					// Get the full charcode:
					int charcode=char.ConvertToUtf32(rawChar,lowChar);
					
					// Grab the surrogate pair char:
					character=FontToDraw.GetCharacter(charcode,fontStyleID);
					
					// Make sure there is no char in the low surrogate spot:
					Characters[i+1]=null;
					// Update this character:
					Characters[i]=character;
					// Skip the low surrogate:
					i++;
				}else{
					character=FontToDraw.GetCharacter((int)characters[i],fontStyleID);
					Characters[i]=character;
				}
				
				
				if(character==null){
					continue;
				}
				
				if(previous!=null){
					
					// Look for a kern pair:
					if(character.Kerning!=null){
						
						float offset;
						
						if(character.Kerning.TryGetValue(previous,out offset)){
							// Got a kern!
							if(Kerning==null){
								Kerning=new float[characters.Length];
							}
							
							Kerning[i]=offset;
						}
						
					}
					
				}
				
				previous=character;
				
				if(!character.Space || character.Image!=null){
					AllWhitespace=false;
				}
				
				if(character.Space || ( !char.IsNumber((char)character.RawCharcode) && !char.IsLetter((char)character.RawCharcode) ) ){
					// Considered punctuation if it's not alphanumeric.
					punctuationCount++;
				}else{
					if(!endedStartPunctuation){
						StartPunctuationCount=punctuationCount;
						endedStartPunctuation=true;
					}
					punctuationCount=0;
				}
				
			}
			
			EndPunctuationCount=punctuationCount;
			
			// Update dimensions:
			SetDimensions();
			
			// And finally request a redraw:
			RequestLayout();
			
		}
		
		public override void Paint(){
			MeshBlock block=FirstBlock;
			while(block!=null){
				block.SetColour(FontColour);
				block.Paint();
				block=block.LocalBlockAfter;
			}
		}
		
		/// <summary>Gets the letter at the given local position in pixels.</summary>
		/// <param name="widthOffset">The position in pixels from the left of this element.</param>
		/// <returns>The number of the letter at this position.</returns>
		public int LetterIndex(int widthOffset){
			if(widthOffset<=0||Characters==null){
				return 0;
			}
			ComputedStyle computed=Element.Style.Computed;
			if(widthOffset>=computed.InnerWidth){
				return Characters.Length;
			}
			
			int intSpaceSize=(int)SpaceSize;
			
			for(int i=0;i<Characters.Length;i++){
				Glyph character=Characters[i];
				
				if(character==null){
					continue;
				}
				
				if(character.Space){
					widthOffset-=intSpaceSize;
				}else{
					widthOffset-=(int)(character.AdvanceWidth * FontSize);
				}
				
				widthOffset-=(int)LetterSpacing;
				
				if(widthOffset<=0){
					return i;
				}
			}
			return Characters.Length;
		}
		
		/// <summary>Gets the horizontal position in pixels of the numbered letter.</summary>
		/// <param name="letterID">The letter to get.</param>
		/// <returns>The position of the left edge of the numbered letter in pixels, relative to the left edge of the UI.</returns>
		public float GetPositionOf(int letterID){
			ComputedStyle computed=Element.Style.Computed;
			float width=computed.OffsetLeft + computed.StyleOffsetLeft;
			return width+LocalPositionOf(letterID);
		}
		
		/// <summary>Gets the horizontal position in pixels of the numbered letter relative to this element.</summary>
		/// <param name="letterID">The letter to get.</param>
		/// <returns>The position of the left edge of the numbered letter in pixels, relative to the left of this element.</returns>
		public float LocalPositionOf(int letterID){
			
			if(Characters==null){
				return 0f;
			}
			
			if(letterID>Characters.Length){
				letterID=Characters.Length;
			}
			
			float result=0f;
			
			for(int i=0;i<letterID;i++){
				Glyph character=Characters[i];
				
				if(character==null){
					continue;
				}
				
				if(character.Space){
					result+=SpaceSize+LetterSpacing;
					continue;
				}
				
				result+=(character.AdvanceWidth * FontSize)+LetterSpacing;
			}
			
			return result;
		}
		
		protected override void Layout(){
			
			if(Characters==null||FontToDraw==null||Characters.Length==0){
				return;
			}
			
			// The blocks we allocate here come from FontToDraw.
			// They use the same renderer and same layout service, but just a different mesh.
			// This is to enable potentially very large font atlases with multiple fonts.
			ComputedStyle computed=Element.Style.Computed;
			Renderman renderer=Element.Document.Renderer;
			
			float top=computed.OffsetTop + computed.StyleOffsetTop;
			float left=computed.OffsetLeft + computed.StyleOffsetLeft;
			
			// Should we auto-alias the text?
			
			// Note that this property "drags" to following elements which is correct.
			// We don't really want to break batching chains for aliasing.
			
			if(Alias==float.MaxValue){
				// Yep! Note all values here are const.
				float aliasing=Fonts.AutoAliasOffset - ( (FontSize-Fonts.AutoAliasRelative) * Fonts.AutoAliasRamp);
				
				if(aliasing>0.1f){
					renderer.FontAliasing=aliasing;
				}
				
			}else{
				
				// Write aliasing:
				renderer.FontAliasing=Alias;
				
			}
			
			if(Extrude!=0f){
				// Compute the extrude now:
				if(Text3D==null){
					Text3D=Get3D(FontSize,FontColour,ref left,ref top);
				}else{
					// Update it.
				}
				
				return;
				
			}else{
				Text3D=null;
			}
			
			
			if(!AllWhitespace){
				// Firstly, make sure the batch is using the right font texture.
				// This may generate a new batch if the font doesn't match the previous or existing font.
				
				// Get the full shape of the element:
				int width=computed.PaddedWidth;
				int height=computed.PaddedHeight;
				int minY=computed.OffsetTop+computed.BorderTop;
				int minX=computed.OffsetLeft+computed.BorderLeft;
				
				BoxRegion boundary=new BoxRegion(minX,minY,width,height);
				
				if(!boundary.Overlaps(renderer.ClippingBoundary)){
					
					if(Visible){
						SetVisibility(false);
					}
					
					return;
				}else if(!Visible){
					
					// ImageLocation will allocate here if it's needed.
					SetVisibility(true);
					
				}
				
			}
			
			float zIndex=computed.ZIndex;
			BoxRegion screenRegion=new BoxRegion();
			
			// First up, underline.
			if(TextLine!=null){
				// We have one. Locate it next.
				float lineWeight=(FontToDraw.StrikeSize * FontSize);
				float yOffset=0f;
				
				switch(TextLine.Type){
				
					case TextLineType.Underline:
						yOffset=Ascender + lineWeight;
					break;
					case TextLineType.StrikeThrough:
						yOffset=(FontToDraw.StrikeOffset * FontSize);
						yOffset=Ascender - yOffset;
					break;
					case TextLineType.Overline:
						yOffset=(lineWeight * 2f);
					break;
				}
				
				Color lineColour=FontColour;
				
				if(TextLine.ColourOverride){
					lineColour=TextLine.Colour;
				}
				
				screenRegion.Set(left,top+yOffset,computed.PixelWidth,lineWeight);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					
					// Ensure we have a batch:
					SetupBatch(null,null);
					
					// This region is visible. Clip it:
					screenRegion.ClipBy(renderer.ClippingBoundary);
					// And get our block ready:
					MeshBlock block=Add();
					// Set the UV to that of the solid block colour pixel:
					block.SetSolidColourUV();
					// Set the colour:
					block.SetColour(lineColour);
					
					block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,zIndex);
				}
				
			}
			
			
			// Next, render the characters.
			// If we're rendering from right to left, flip the punctuation over.
			
			// Is the word itself rightwards?
			bool rightwardWord=false;
			
			if(StartPunctuationCount<Characters.Length){
				// Is the first actual character a rightwards one?
				Glyph firstChar=Characters[StartPunctuationCount];
				
				if(firstChar!=null){
					rightwardWord=firstChar.Rightwards;
				}
				
			}
			
			// Right to left (e.g. arabic):
			if(computed.DrawDirection==DirectionType.RTL){
			
				int end=Characters.Length-EndPunctuationCount;
				
				// Draw the punctuation from the end of the string first, backwards:
				if(EndPunctuationCount>0){
					for(int i=Characters.Length-1;i>=end;i--){
						DrawInvertCharacter(i,ref left,top,renderer,zIndex,screenRegion);
					}
				}
				
				if(rightwardWord){
					// Render the word itself backwards.
					
					for(int i=end-1;i>=StartPunctuationCount;i--){
						DrawCharacter(i,ref left,top,renderer,zIndex,screenRegion);
					}
					
				}else{
				
					// Draw the middle characters:
					for(int i=StartPunctuationCount;i<end;i++){
						DrawCharacter(i,ref left,top,renderer,zIndex,screenRegion);
					}
					
				}
				
				// Draw the punctuation from the start of the string last, backwards:
				
				if(StartPunctuationCount>0){
					
					for(int i=StartPunctuationCount-1;i>=0;i--){
						DrawInvertCharacter(i,ref left,top,renderer,zIndex,screenRegion);
					}
					
				}
				
			}else if(rightwardWord){
				
				// Render the word itself backwards.
				
				for(int i=Characters.Length-1;i>=0;i--){
					DrawCharacter(i,ref left,top,renderer,zIndex,screenRegion);
				}
				
			}else{
				
				// Draw it as is.
				
				for(int i=0;i<Characters.Length;i++){
					
					DrawCharacter(i,ref left,top,renderer,zIndex,screenRegion);
				}
				
			}
			
		}
		
		/// <summary>Draws a character with x-inverted UV's. Used for rendering e.g. "1 < 2" in right-to-left.</summary>
		private void DrawInvertCharacter(int index,ref float left,float top,Renderman renderer,float zIndex,BoxRegion screenRegion){
			
			Glyph character=Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Kerning!=null){
				left+=Kerning[index] * FontSize;
			}
			
			if(character.Space){
				left+=SpaceSize+LetterSpacing;
				return;
			}
			
			float y=top+Ascender-((character.Height+character.MinY) * FontSize);
			
			AtlasLocation locatedAt=character.Location;
			
			if(locatedAt==null){
				// Not in font.
				return;
			}
			
			screenRegion.Set(left + (character.LeftSideBearing * FontSize),y,locatedAt.Width * ScaleFactor,locatedAt.Height * ScaleFactor);
			
			if(screenRegion.Overlaps(renderer.ClippingBoundary)){
				// True if this character is visible.
				
				// Ensure correct batch:
				SetupBatch(null,locatedAt.Atlas);
			
				MeshBlock block=Add();
				block.SetColour(FontColour);
				
				// And clip our meshblock to fit within boundary:
				block.ImageUV=null;
				UVBlock uvs=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,zIndex,locatedAt,block.TextUV);
				
				if(uvs.Shared){
					uvs=new UVBlock(uvs);
				}
				
				// Invert along X:
				float temp=uvs.MinX;
				uvs.MinX=uvs.MaxX;
				uvs.MaxX=temp;
				
				// Assign to the block:
				block.TextUV=uvs;
				
			}
			
			left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
		}
		
		/// <summary>Draws a character and advances the pen onwards.</summary>
		private void DrawCharacter(int index,ref float left,float top,Renderman renderer,float zIndex,BoxRegion screenRegion){
			
			Glyph character=Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Kerning!=null){
				left+=Kerning[index] * FontSize;
			}
			
			AtlasLocation locatedAt;
			
			if(character.Image!=null){
				
				if(!character.Image.Loaded()){
					return;
				}
				
				// It's an image (e.g. Emoji).
				locatedAt=RequireImage(character.Image);
				
				if(locatedAt==null){
					// It needs to be isolated. Big emoji image!
					return;
				}
				
				if(CharacterProviders.FixHeight){
					// Set the region:
					screenRegion.Set(left,top,locatedAt.Width,locatedAt.Height);
				}else{
					screenRegion.Set(left,top,FontSize,FontSize);
				}
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
						
					// Ensure correct batch:
					SetupBatch(locatedAt.Atlas,null);
					
					// If the two overlap, this means it's actually visible.
					MeshBlock block=Add();
					
					// Set it's colour:
					block.SetColour(Element.Style.Computed.ColorOverlay);
					
					// And clip our meshblock to fit within boundary:
					block.TextUV=null;
					block.ImageUV=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,zIndex,locatedAt,block.ImageUV);
				}
				
				left+=(character.AdvanceWidth)+LetterSpacing;
				return;
			}else if(character.Space){
				left+=SpaceSize+LetterSpacing;
				return;
			}
			
			
			locatedAt=character.Location;
			
			if(locatedAt==null){
				// Not in font.
				return;
			}
			
			float y=top+Ascender-((character.Height+character.MinY) * FontSize);
			
			screenRegion.Set(left + (character.LeftSideBearing * FontSize),y,locatedAt.Width * ScaleFactor,locatedAt.Height * ScaleFactor);
			
			if(screenRegion.Overlaps(renderer.ClippingBoundary)){
				// True if this character is visible.
				
				// Ensure correct batch:
				SetupBatch(null,locatedAt.Atlas);
				
				MeshBlock block=Add();
				block.SetColour(FontColour);
				
				// And clip our meshblock to fit within boundary:
				block.ImageUV=null;
				block.TextUV=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,zIndex,locatedAt,block.TextUV);
				
			}
			
			left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
		}
		
		public override string ToString(){
			return Text;
		}
		
		public void ToString(System.Text.StringBuilder builder){
			builder.Append(Text);
		}
		
	}
	
}