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

namespace PowerUI{
	
	/// <summary>
	/// Represents a character within a font for display on the screen
	/// with a specific fontsize and weight.
	/// </summary>
	
	public class DynamicCharacter{
	
		/// <summary>True if this character is a whitespace.</summary>
		public bool Space;
		/// <summary>True if this character has been used on the screen.</summary>
		public bool InUse;
		/// <summary>The width in pixels of the character.</summary>
		public float Width;
		/// <summary>The height of the character in pixels.</summary>
		public float Height;
		/// <summary>True if this character is flipped on the texture atlas.</summary>
		public bool Flipped;
		/// <summary>True if this character is an image, e.g. an Emoji icon.</summary>
		public bool IsImage;
		/// <summary>True if this is a rightwards character (e.g. Arabic).</summary>
		public bool Rightwards;
		/// <summary>The charcode of the character to display.</summary>
		public int RawCharcode;
		/// <summary>How far the renderer should advance when drawing this character.</summary>
		public float DeltaWidth;
		/// <summary>UV coords per character pixel on y.</summary>
		public float UVPerPixelY;
		/// <summary>UV coords per character pixel on x.</summary>
		public float UVPerPixelX;
		/// <summary>The character being displayed.</summary>
		public char RawCharacter;
		/// <summary>The UV coordinates of the top left corner.</summary>
		public Vector2 UVTopLeft;
		/// <summary>The UV coordinates of the top right corner.</summary>
		public Vector2 UVTopRight;
		/// <summary>A graphical image representation of this character for e.g. Emoji.</summary>
		public ImagePackage Image;
		/// <summary>How tall the ascendor of this character is.</summary>
		public float AscendorOffset;
		/// <summary>The UV coordinates of the bottom left corner.</summary>
		public Vector2 UVBottomLeft;
		/// <summary>The UV coordinates of the bottom right corner.</summary>
		public Vector2 UVBottomRight;
		/// <summary>True if character is set.</summary>
		public bool HasCharacterInfo;
		/// <summary>The Unity CharacterInfo for this character.</summary>
		public CharacterInfo Character;
		/// <summary>The set of characters this character belongs to.</summary>
		public FontSizeCharSet CharacterSet;
		
		
		/// <summary>Creates a character that represents a whitespace.</summary>
		public DynamicCharacter(){
			Space=true;
		}
		
		/// <summary>Creates the given character which will belong to the given set.</summary>
		/// <param name="rawCharacter">The character to display.</param>
		/// <param name="charSet">The set of characters it will belong to.</param>
		public DynamicCharacter(char rawCharacter,FontSizeCharSet charSet){
			CharacterSet=charSet;
			RawCharacter=rawCharacter;
			
			// Is it rightwards?
			Rightwards=RightToLeft.Rightwards((int)rawCharacter);
		}
		
		/// <summary>Creates the given character which will belong to the given set.</summary>
		/// <param name="charcode">The charcode of the character to display.</param>
		/// <param name="charSet">The set of characters it will belong to.</param>
		public DynamicCharacter(int charcode,FontSizeCharSet charSet){
			CharacterSet=charSet;
			RawCharcode=charcode;
			
			// Is it rightwards?
			Rightwards=RightToLeft.Rightwards(charcode);
		}
		
		/// <summary>Sets the UV and dimensions of this character from the given characterInfo.
		/// Can only be called when the characterInfo has a place on the fonts atlas.</summary>
		/// <param name="character">The character to set.</param>
		public void SetCharacter(CharacterInfo character){
			Character=character;
			Rect uv=character.uv;
			
			// Pregenerate the UV set:
			if(character.flipped){
				UVTopLeft=new Vector2(2f+uv.xMax,uv.y);
				UVTopRight=new Vector2(2f+uv.xMax,uv.yMax);
				UVBottomLeft=new Vector2(2f+uv.x,uv.y);
				UVBottomRight=new Vector2(2f+uv.x,uv.yMax);
				// Width and height are inverted.
				// Set them to the size in UV coords - we'll divide shortly:
				UVPerPixelX=UVTopRight.y-UVBottomLeft.y;
				UVPerPixelY=UVTopRight.x-UVBottomLeft.x;
			}else{
				UVTopLeft=new Vector2(2f+uv.x,uv.yMax);
				UVTopRight=new Vector2(2f+uv.xMax,uv.yMax);
				UVBottomLeft=new Vector2(2f+uv.x,uv.y);
				UVBottomRight=new Vector2(2f+uv.xMax,uv.y);
				// Set them to the size in UV coords - we'll divide shortly:
				UVPerPixelY=UVTopRight.y-UVBottomLeft.y;
				UVPerPixelX=UVTopRight.x-UVBottomLeft.x;
			}
			
			Flipped=character.flipped;
			
			// And next up the width/height of the character:
			Width=character.vert.width;
			Height=-character.vert.height;
			
			// Divide the size in uv coords by width and height to make them actually uv per pixel:
			UVPerPixelY/=Height;
			UVPerPixelX/=Width;
			
			// How far should be advanced when rendering this character:
			DeltaWidth=character.width-character.vert.x;
			//Next, figure out how far we need to move this character down such that it sits on the baseline.
			
			//Next, we take the top of this character away - it's at character.vert.y to get the offset we want:
			AscendorOffset = (float)Math.Floor(CharacterSet.CharacterTop - character.vert.y);
			
			if(Width==0 && Height==0){
				NotFound();
			}
		}
		
		/// <summary>Gets the font size of this character.</summary>
		public int FontSize{
			get{
				return CharacterSet.FontSize;
			}
		}
		
		/// <summary>Called when this character was not found in the font.
		/// Maybe it exists as an image (e.g. an Emoji image).</summary>
		public void NotFound(){
			HasCharacterInfo=false;
			// Let's start looking for it:
			CharacterProviders.Find(this);
		}
		
		/// <summary>Called when an image is found for this character. Used by e.g. Emoji.</summary>
		/// <param name="package">The image that was found.</param>
		public void Found(ImagePackage package){
			if(package==null){
				IsImage=false;
				return;
			}
			
			Image=package;
			
			// Great! We found this character somewhere else as an image:
			IsImage=true;
			Width=Image.Width();
			Height=Image.Height();
			
			// Set up dimensions:
			UVPerPixelY=UVTopRight.y-UVBottomLeft.y;
			UVPerPixelX=UVTopRight.x-UVBottomLeft.x;
			
			// Divide the size in uv coords by width and height to make them actually uv per pixel:
			UVPerPixelY/=Height;
			UVPerPixelX/=Width;
			
			// And apply delta width too:
			DeltaWidth=Width;
			
			// Queue up a global layout.
			UI.document.Renderer.RequestLayout();
		}
		
		/// <summary>The raw charcode for this character.</summary>
		public int Charcode{
			get{
				if(RawCharcode!=0){
					return RawCharcode;
				}
				
				return (int)RawCharacter;
			}
		}
		
	}
	
}