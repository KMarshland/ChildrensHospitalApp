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


namespace PowerUI.Css{
	
	/// <summary>
	/// This class manages rendering text to the screen.
	/// </summary>
	
	public class TextRenderingProperty:DisplayableProperty{
		
		/// <summary>True if the text should be bold.</summary>
		public bool Bold;
		/// <summary>True if the text should be italic.</summary>
		public bool Italic;
		/// <summary>The string to render. Note that this is parsed
		/// into <see cref="PowerUI.Css.TextRenderingProperty.Characters"/>.</summary>
		public string Text;
		/// <summary>The size of the font in pixels.</summary>
		public int FontSize;
		/// <summary>How wide a space should be in pixels.</summary>
		public float SpaceSize;
		/// <summary>The colour that the font should be without the colour overlay.</summary>
		public Color BaseColour;
		/// <summary>The base colour combined with the colour overlay.</summary>
		public Color FontColour;
		/// <summary>True if there have been no text changes.</summary>
		public bool NoTextChange;
		
		/// <summary>The thickness of the text line, if there is one.</summary>
		public int TextLineWeight;
		/// <summary>True if all characters are whitespaces. No batches will be generated.</summary>
		public bool AllWhitespace;
		/// <summary>Additional spacing to apply around letters.</summary>
		public float LetterSpacing;
		/// <summary>How and where a line should be drawn if at all (e.g. underline, overline etc.)</summary>
		public TextLineType TextLine;
		/// <summary>The font to use when rendering.</summary>
		public DynamicFont FontToDraw;
		/// <summary>The number of punctiation characters at the end of the word.</summary>
		public int EndPunctuationCount;
		/// <summary>The number of punctiation characters at the start of the word.</summary>
		public int StartPunctuationCount;
		/// <summary>The set of characters to render. Note that the characters are shared globally.</summary>
		public DynamicCharacter[] Characters;
		
		
		/// <summary>The minimum font size at which spreadover occurs. As all characters get written to a texture, large font
		/// may cause this texture to fill up. Spreadover is the font size at which PowerUI will start searching for e.g.
		/// a specific bold version of the font to 'spread' or offload some of the characters onto.</summary>
		public int Spreadover=30;
		/// <summary>How text should be clipped at the edges of scrolling regions. Set with text-clip css property.</summary>
		public TextClipType TextClipping=TextClipType.Fast;
		
		
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
				
				if(Characters[Characters.Length-1].Space){
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
		
		/// <summary>Recomputes the baseline, space size and inner height of the parent element.</summary>
		public void SetDimensions(){
			if(FontToDraw==null){
				return;
			}
			
			ComputedStyle computed=Element.Style.Computed;
			computed.Baseline=(int)FontToDraw.GetDescend(FontSize);
			computed.FixedHeight=true;
			computed.InnerHeight=(int)FontToDraw.GetHeight(FontSize);
			
			if(SpaceSize==0f){
				SpaceSize=FontToDraw.GetSpaceSize(FontSize);
			}
			
			computed.SetSize();
		}
		
		public override void SetOverlayColour(Color colour){
			FontColour=BaseColour*colour;
			RequestPaint();
		}
		
		/// <summary>Allocates the characters that are held by this text object so they don't get purged.</summary>
		public void AllocateText(){
			if(Characters==null){
				return;
			}
			
			foreach(DynamicCharacter character in Characters){
				if(character==null){
					continue;
				}
				character.InUse=true;
			}
		}
		
		/// <summary>Loads the character array (<see cref="PowerUI.Css.TextRenderingProperty.Characters"/>) from the text string.</summary>
		public void SetText(){
			
			if(Text==null||FontToDraw==null){
				// Can't do it just yet - we have no text/ font to use.
				return;
			}
			
			char[] characters=Text.ToCharArray();
			
			Characters=new DynamicCharacter[characters.Length];
			
			if(TextLine==TextLineType.None){
				TextLineWeight=0;
			}else{
				TextLineWeight=FontSize/12;
				if(TextLineWeight<0){
					TextLineWeight=1;
				}
			}
			
			// Find the style ID for our properties:
			int fontStyleID=0;
			
			if(Bold&&Italic){
				fontStyleID=3;
			}else if(Bold){
				fontStyleID=2;
			}else if(Italic){
				fontStyleID=1;
			}
			
			// If the text is large, Unity may not be able to fit all characters onto the texture atlas.
			// So instead we go hunting for a font that's already bold/italic if this is and use that instead.
			if(FontSize>=Spreadover){
				if(fontStyleID!=FontToDraw.StyleID){
					FontToDraw=FontToDraw.GetAlternateFont(fontStyleID);
				}
			}else if(FontToDraw.StyleID!=0){
				// Reset it back to the main one:
				FontToDraw=FontToDraw.GetAlternateFont(0);
			}
			
			// The number of punctuation marks in a row that have been spotted.
			int punctuationCount=0;
			// This goes true when we hit the first non-punctuation value in the word.
			bool endedStartPunctuation=false;
			
			StartPunctuationCount=0;
			
			// Considered all whitespaces until shown otherwise.
			AllWhitespace=true;
			
			// Next, for each character, find its dynamic character.
			// At the same time we want to find out what dimensions this word has so it can be located correctly.
			for(int i=0;i<characters.Length;i++){
				char rawChar=characters[i];
				
				DynamicCharacter character=null;
				
				// Is it a unicode high/low surrogate pair?
				if(char.IsHighSurrogate(rawChar) && i!=characters.Length-1){
					// Low surrogate follows:
					char lowChar=characters[i+1];
					
					// Grab the surrogate pair char:
					character=FontToDraw.GetCharacter(lowChar,rawChar,FontSize,fontStyleID);
					
					// Make sure there is no char in the low surrogate spot:
					Characters[i+1]=null;
					// Update this character:
					Characters[i]=character;
					// Skip the low surrogate:
					i++;
				}else{
					character=FontToDraw.GetCharacter(characters[i],FontSize,fontStyleID);
					Characters[i]=character;
				}
				
				
				if(character==null){
					continue;
				}
				
				if(!character.Space){
					AllWhitespace=false;
				}
				
				if(character.Space || ( !char.IsNumber(character.RawCharacter) && !char.IsLetter(character.RawCharacter) ) ){
					// Considered punctuation if it's not alphanumeric.
					punctuationCount++;
				}else{
					if(!endedStartPunctuation){
						StartPunctuationCount=punctuationCount;
						endedStartPunctuation=true;
					}
					punctuationCount=0;
				}
				
				if(character.Space){
					continue;
				}
			}
			
			EndPunctuationCount=punctuationCount;
			
			// And finally request a redraw:
			RequestLayout();
			NoTextChange=false;
		}
		
		/// <summary>Recomputes the width of this element.</summary>
		public void SetWidth(){
			if(Characters==null||NoTextChange){
				return;
			}
			NoTextChange=true;
			float width=0f;
			float height=0f;
			
			for(int i=0;i<Characters.Length;i++){
				DynamicCharacter dChar=Characters[i];
				
				if(dChar==null){
					continue;
				}
				
				if(dChar.Space){
					width+=SpaceSize;
				}else{
					width+=dChar.DeltaWidth;
				}
				
				if(dChar.IsImage&& dChar.Height>height){
					// Emoji char.
					
					if(height==0f){
						height=FontToDraw.GetHeight(FontSize);
						
						if(dChar.Height>height){
							height=dChar.Height;
						}
					}else{
						height=dChar.Height;
					}
				}
				
				width+=LetterSpacing;
			}
			ComputedStyle computed=Element.Style.Computed;
			if(height!=0f){
				
				computed.InnerHeight=(int)height;
			}
			computed.InnerWidth=(int)width;
			computed.FixedWidth=true;
			computed.SetSize();
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
				DynamicCharacter character=Characters[i];
				if(character==null){
					continue;
				}
				
				if(character.Space){
					widthOffset-=intSpaceSize;
				}else{
					widthOffset-=(int)character.DeltaWidth;
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
				DynamicCharacter character=Characters[i];
				
				if(character==null){
					continue;
				}
				
				if(character.Space){
					result+=SpaceSize+LetterSpacing;
					continue;
				}
				result+=character.DeltaWidth+LetterSpacing;
			}
			return result;
		}
		
		protected override void Layout(){
			if(Characters==null||FontToDraw==null||Characters.Length==0){
				return;
			}
			
			if(!AllWhitespace){
				// Firstly, make sure the batch is using the right font texture.
				// This may generate a new batch if the font doesn't match the previous or existing font.
				SetFontTexture(FontToDraw.FontTexture);
			}
			
			// The blocks we allocate here come from FontToDraw.
			// They use the same renderer and same layout service, but just a different mesh.
			// This is to enable potentially very large font atlases with multiple fonts.
			ComputedStyle computed=Element.Style.Computed;
			
			float top=computed.OffsetTop + computed.StyleOffsetTop;
			float left=computed.OffsetLeft + computed.StyleOffsetLeft;
			
			float zIndex=computed.ZIndex;
			BoxRegion screenRegion=new BoxRegion();
			Renderman renderer=Element.Document.Renderer;
			
			// First up, underline.
			if(TextLine!=TextLineType.None){
				// We have one. Locate it next.
				int yOffset=0;
				if(TextLine==TextLineType.Underline){
					yOffset=computed.InnerHeight-computed.Baseline;
				}else if(TextLine==TextLineType.StrikeThrough){
					yOffset=computed.InnerHeight/2;
				}
				screenRegion.Set(left,top+yOffset-(TextLineWeight/2),computed.InnerWidth,TextLineWeight);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					// This region is visible. Clip it:
					screenRegion.ClipBy(renderer.ClippingBoundary);
					// And get our block ready:
					MeshBlock block=Add();
					// Set the UV to that of the solid block colour pixel:
					block.SetSolidColourUV();
					// Set the font colour:
					block.SetColour(FontColour);
					
					block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,zIndex);
				}
			}
			
			// Next, render the characters.
			// If we're rendering from right to left, flip the punctuation over.
			
			// Is the word itself rightwards?
			bool rightwardWord=false;
			
			if(StartPunctuationCount<Characters.Length){
				// Is the first actual character a rightwards one?
				rightwardWord=Characters[StartPunctuationCount].Rightwards;
			}
			
			// Right to left (e.g. arabic):
			if(computed.DrawDirection==DirectionType.RTL){
			
				int end=Characters.Length-EndPunctuationCount;
				
				// Draw the punctuation from the end of the string first, backwards:
				if(EndPunctuationCount>0){
					for(int i=Characters.Length-1;i>=end;i--){
						DrawInvertCharacter(Characters[i],ref left,top,renderer,zIndex,screenRegion);
					}
				}
				
				if(rightwardWord){
					// Render the word itself backwards.
					
					for(int i=end-1;i>=StartPunctuationCount;i--){
						DrawCharacter(Characters[i],ref left,top,renderer,zIndex,screenRegion);
					}
					
				}else{
				
					// Draw the middle characters:
					for(int i=StartPunctuationCount;i<end;i++){
						DrawCharacter(Characters[i],ref left,top,renderer,zIndex,screenRegion);
					}
					
				}
				
				// Draw the punctuation from the start of the string last, backwards:
				
				if(StartPunctuationCount>0){
					
					for(int i=StartPunctuationCount-1;i>=0;i--){
						DrawInvertCharacter(Characters[i],ref left,top,renderer,zIndex,screenRegion);
					}
					
				}
				
			}else if(rightwardWord){
				
				// Render the word itself backwards.
				
				for(int i=Characters.Length-1;i>=0;i--){
					DrawCharacter(Characters[i],ref left,top,renderer,zIndex,screenRegion);
				}
				
			}else{
				
				// Draw it as is.
				
				for(int i=0;i<Characters.Length;i++){
					DrawCharacter(Characters[i],ref left,top,renderer,zIndex,screenRegion);
				}
				
			}
			
		}
		
		/// <summary>Draws a character with x-inverted UV's. Used for rendering e.g. "1 < 2" in right-to-left.</summary>
		private void DrawInvertCharacter(DynamicCharacter character,ref float left,float top,Renderman renderer,float zIndex,BoxRegion screenRegion){
			if(character==null){
				return;
			}
			
			if(character.Space){
				left+=SpaceSize+LetterSpacing;
				return;
			}
			
			float y=top+character.AscendorOffset;
			
			screenRegion.Set(left,y,character.Width,character.Height);
			
			if(screenRegion.Overlaps(renderer.ClippingBoundary)){
				// True if this character is visible.
				
				MeshBlock block=Add();
				block.SetColour(FontColour);
				
				// And clip our meshblock to fit within boundary:
				block.SetClipped(renderer.ClippingBoundary,screenRegion,Element.Document.Renderer,zIndex);
				
				if(TextClipping==TextClipType.Fast){
					// No UV clipping. Note the inversion here!
					block.UVTopLeft=character.UVTopRight;
					block.UVTopRight=character.UVTopLeft;
					block.UVBottomLeft=character.UVBottomRight;
					block.UVBottomRight=character.UVBottomLeft;
				}else{
					// Clip the UVs.
					
					// Find the size in UV coords that was chopped off the top and left edges:
					float choppedOffTop=(screenRegion.Y-y) * character.UVPerPixelY;
					float choppedOffLeft=(screenRegion.X-left) * character.UVPerPixelX;
					
					if(character.Flipped){
						// Flipped character.
						float maxU=character.UVTopRight.x - choppedOffTop;
						float minV=character.UVBottomLeft.y + choppedOffLeft;
						
						float minU=maxU - (screenRegion.Height * character.UVPerPixelY);
						float maxV=minV + (screenRegion.Width * character.UVPerPixelX);
						
						// Note that the following is inverted.
						block.UVTopRight=new Vector2(maxU,minV);
						block.UVTopLeft=new Vector2(maxU,maxV);
						block.UVBottomRight=new Vector2(minU,minV);
						block.UVBottomLeft=new Vector2(minU,maxV);
						
					}else{
						// UV's are as you'd expect here.
						float minU=character.UVBottomLeft.x + choppedOffLeft;
						float maxV=character.UVTopRight.y - choppedOffTop;
						
						float maxU=minU + (screenRegion.Width * character.UVPerPixelX);
						float minV=maxV - (screenRegion.Height * character.UVPerPixelY);
						
						// Note that the following is inverted.
						block.UVTopRight=new Vector2(minU,maxV);
						block.UVTopLeft=new Vector2(maxU,maxV);
						block.UVBottomRight=new Vector2(minU,minV);
						block.UVBottomLeft=new Vector2(maxU,minV);
					}
				}
			}
			
			left+=character.DeltaWidth+LetterSpacing;
		}
		
		/// <summary>Draws a character and advances the pen onwards.</summary>
		private void DrawCharacter(DynamicCharacter character,ref float left,float top,Renderman renderer,float zIndex,BoxRegion screenRegion){
			if(character==null){
				return;
			}
			
			if(character.Space){
				left+=SpaceSize+LetterSpacing;
				return;
			}
			
			if(character.IsImage){
				// It's an image (e.g. Emoji).
				AtlasLocation locatedAt=AddTexture(character.Image.Image);
				
				if(locatedAt==null){
					// We didn't have any space for the image on the atlas.
					return;
				}
				
				// Set the region:
				screenRegion.Set(left,top,locatedAt.Width,locatedAt.Height);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					
					// If the two overlap, this means it's actually visible.
					MeshBlock block=Add();
					
					// Set it's colour:
					block.SetColour(Element.Style.Computed.ColorOverlay);
					
					// And clip our meshblock to fit within boundary:
					block.SetClipped(renderer.ClippingBoundary,screenRegion,Element.Document.Renderer,zIndex,locatedAt);
				}
				
				left+=character.DeltaWidth+LetterSpacing;
				return;
			}
			
			float y=top+character.AscendorOffset;
			
			screenRegion.Set(left,y,character.Width,character.Height);
			
			if(screenRegion.Overlaps(renderer.ClippingBoundary)){
				// True if this character is visible.
				
				MeshBlock block=Add();
				block.SetColour(FontColour);
				
				// And clip our meshblock to fit within boundary:
				block.SetClipped(renderer.ClippingBoundary,screenRegion,Element.Document.Renderer,zIndex);
				
				if(TextClipping==TextClipType.Fast){
					// No UV clipping.
					block.UVTopLeft=character.UVTopLeft;
					block.UVTopRight=character.UVTopRight;
					block.UVBottomLeft=character.UVBottomLeft;
					block.UVBottomRight=character.UVBottomRight;
				}else{
					// Clip the UVs.
					
					// Find the size in UV coords that was chopped off the top and left edges:
					float choppedOffTop=(screenRegion.Y-y) * character.UVPerPixelY;
					float choppedOffLeft=(screenRegion.X-left) * character.UVPerPixelX;
					
					if(character.Flipped){
						// Flipped character.
						float maxU=character.UVTopRight.x - choppedOffTop;
						float minV=character.UVBottomLeft.y + choppedOffLeft;
						
						float minU=maxU - (screenRegion.Height * character.UVPerPixelY);
						float maxV=minV + (screenRegion.Width * character.UVPerPixelX);
						
						block.UVTopLeft=new Vector2(maxU,minV);
						block.UVTopRight=new Vector2(maxU,maxV);
						block.UVBottomLeft=new Vector2(minU,minV);
						block.UVBottomRight=new Vector2(minU,maxV);
						
					}else{
						// UV's are as you'd expect here.
						float minU=character.UVBottomLeft.x + choppedOffLeft;
						float maxV=character.UVTopRight.y - choppedOffTop;
						
						float maxU=minU + (screenRegion.Width * character.UVPerPixelX);
						float minV=maxV - (screenRegion.Height * character.UVPerPixelY);
						
						block.UVTopLeft=new Vector2(minU,maxV);
						block.UVTopRight=new Vector2(maxU,maxV);
						block.UVBottomLeft=new Vector2(minU,minV);
						block.UVBottomRight=new Vector2(maxU,minV);
					}
				}
			}
			left+=character.DeltaWidth+LetterSpacing;
		}
		
		public override string ToString(){
			return Text;
		}
		
	}
	
}