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
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PowerUI{

	/// <summary>
	/// Represents a set of characters all at a fixed fontsize and styling (bold/italic etc).
	/// </summary>
	
	public class FontSizeCharSet{
		
		/// <summary>The fontsize.</summary>
		public int FontSize;
		/// <summary>The style of the font; e.g. bold, italic etc.</summary>
		public FontStyle Style;
		/// <summary>The parent font, cached for quick access.</summary>
		public DynamicFont Font;
		/// <summary>The location of the top of a full character.</summary>
		public float CharacterTop;
		/// <summary>All unique characters in this set.</summary>
		public string AllCharacters;
		/// <summary>The parent style set.</summary>
		public DynamicCharSet StyleSet;
		/// <summary>The set of surrogated characters. That's characters formed from two char's (see surrogate pairs in unicode).</summary>
		public Dictionary<int,DynamicCharacter> SurrogateCharacters;
		/// <summary>The set of all characters.</summary>
		public Dictionary<char,DynamicCharacter> Characters=new Dictionary<char,DynamicCharacter>();
		
		
		/// <summary>Creates a new fontsize set.</summary>
		/// <param name="fontSize">The fontsize of all characters in the set.</param>
		/// <param name="styleSet">The parent style set.</param>
		public FontSizeCharSet(int fontSize,DynamicCharSet styleSet){
			FontSize=fontSize;
			StyleSet=styleSet;
			Style=styleSet.Style;
			Font=StyleSet.Font;
			// First we need to find out where the top of a full character is.
			// The character sits on the baseline and extends upwards (through 'full ascend'), so it's top is at:
			CharacterTop=Font.Baseline + Font.GetFullAscend(FontSize);
		}
		
		/// <summary>Gets the given character from the set as a surrogate pair.</summary>
		/// <param name="lowCharacter">The low character to retrieve.</param>
		/// <param name="highCharacter">The high character to retrieve.</param>
		/// <returns>A renderable character. It will be created if it was not found.</returns>
		public DynamicCharacter GetCharacter(char lowCharacter,char highCharacter){
			// What's the full charcode?
			int charcode=char.ConvertToUtf32(highCharacter,lowCharacter);
			
			// Setup the lookup, if needed:
			if(SurrogateCharacters==null){
				SurrogateCharacters=new Dictionary<int,DynamicCharacter>();
			}
			
			DynamicCharacter result;
			if(!SurrogateCharacters.TryGetValue(charcode,out result)){
				result=SurrogateCharacters[charcode]=new DynamicCharacter(charcode,this);
			}
			
			return result;
		}
		
		/// <summary>Gets the given character from the set.</summary>
		/// <param name="character">The character to retrieve.</param>
		/// <returns>A renderable character. It will be created if it was not found.</returns>
		public DynamicCharacter GetCharacter(char character){
			DynamicCharacter result;
			if(!Characters.TryGetValue(character,out result)){
				result=Characters[character]=new DynamicCharacter(character,this);
			}
			return result;
		}
		
		/// <summary>Causes all the characters we are trying to render to be deallocated.</summary>
		public void PrepareForAllocate(){
			foreach(KeyValuePair<char,DynamicCharacter> kvp in Characters){
				kvp.Value.InUse=false;
			}
			
			if(SurrogateCharacters!=null){
				foreach(KeyValuePair<int,DynamicCharacter> kvp in SurrogateCharacters){
					kvp.Value.InUse=false;
				}
			}
		}
		
		/// <summary>Called immediately before a layout occurs and makes sure all unique 
		/// characters are requested from the font.</summary>
		public void PrepareForLayout(){
		
			// Figure out all the unique characters:
			StringBuilder newCharacterString=new StringBuilder("",Characters.Count);
			
			foreach(KeyValuePair<char,DynamicCharacter> kvp in Characters){
				if(kvp.Value.InUse){
					newCharacterString.Append(kvp.Value.RawCharacter);
				}
			}
			
			AllCharacters=newCharacterString.ToString();
			
			// Request all characters from the font:
			RequestFromFont();
		}
		
		/// <summary>Request all characters from the font.</summary>
		public void RequestFromFont(){
			Font.RawFont.RequestCharactersInTexture(AllCharacters,FontSize,Style);
		}
		
		/// <summary>Called after PrepareForLayout. Makes sure all characters in the buffer are up-to-date.</summary>
		/// <param name="infoRequired">True if HasCharacterInfo must be false for a request to occur. Requested anyway if false.</param>
		public void RefreshBufferedCharacters(bool infoRequired){
			
			Font rawFont=Font.RawFont;
			
			foreach(KeyValuePair<char,DynamicCharacter> kvp in Characters){
				DynamicCharacter character=kvp.Value;
				
				if(character.Space || character.IsImage || ((!character.InUse || character.HasCharacterInfo) && infoRequired)){
					continue;
				}
				// This character must be requested - lets go get it now.
				// Its in use, not a space and either doesn't have character info or we're refreshing all anyway.
				CharacterInfo info;
				if(rawFont.GetCharacterInfo(kvp.Key,out info,FontSize,Style)){
					character.HasCharacterInfo=true;
					character.SetCharacter(info);
				}else{
					character.NotFound();
				}
			}
			
			// Note: Surrogates are always images, so we only need the not found checks here: 
			
			if(SurrogateCharacters!=null){
				
				foreach(KeyValuePair<int,DynamicCharacter> kvp in SurrogateCharacters){
					DynamicCharacter character=kvp.Value;
					
					// Skip if it already knows it's an image:
					if(character.IsImage){
						continue;
					}
					
					// It won't ever be found:
					character.NotFound();
				}
				
			}
			
		}
		
	}
	
}