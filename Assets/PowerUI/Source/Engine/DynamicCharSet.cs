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

namespace PowerUI{

	/// <summary>
	/// Represents a set of characters with a given style (e.g. bold/italic).
	/// Internally holds further sets of characters at certain font sizes.
	/// </summary>

	public class DynamicCharSet{
		
		/// <summary>The font style - e.g. bold/italic - of these characters.</summary>
		public FontStyle Style;
		/// <summary>The font all these characters belong to.</summary>
		public DynamicFont Font;
		/// <summary>A map of fontsizes to sets of characters.</summary>
		public Dictionary<int,FontSizeCharSet> FontSizes=new Dictionary<int,FontSizeCharSet>();
		
		
		/// <summary>Creates a new character set for the given font and with the given style.</summary>
		/// <param name="font">The font the characters belong to.</param>
		/// <param name="styleID">The style of the characters:
		/// 0 = Normal.
		/// 1 = Italic.
		/// 2 = Bold.
		/// 3 = Both bold and italic.</param>
		public DynamicCharSet(DynamicFont font,int styleID){
			Font=font;
			if(styleID==0){
				Style=FontStyle.Normal;
			}else if(styleID==1){
				Style=FontStyle.Italic;
			}else if(styleID==2){
				Style=FontStyle.Bold;
			}else{
				Style=FontStyle.BoldAndItalic;
			}
		}
		
		/// <summary>Gets or creates the given character as a surrogate pair at the given fontsize from this set
		/// in a format suitable for display on the screen.</summary>
		/// <param name="lowCharacter">The low character to find.</param>
		/// <param name="highCharacter">The high character to find.</param>
		/// <param name="fontSize">The fontsize to find it at.</param>
		/// <returns>A displayable character.</returns>
		public DynamicCharacter GetCharacter(char lowCharacter,char highCharacter,int fontSize){
			FontSizeCharSet fontSizeSet;
			if(!FontSizes.TryGetValue(fontSize,out fontSizeSet)){
				fontSizeSet=FontSizes[fontSize]=new FontSizeCharSet(fontSize,this);
			}
			
			return fontSizeSet.GetCharacter(lowCharacter,highCharacter);
		}
		
		/// <summary>Gets or creates the given character at the given fontsize from this set
		/// in a format suitable for display on the screen.</summary>
		/// <param name="character">The character to find.</param>
		/// <param name="fontSize">The fontsize to find it at.</param>
		/// <returns>A displayable character.</returns>
		public DynamicCharacter GetCharacter(char character,int fontSize){
			FontSizeCharSet fontSizeSet;
			if(!FontSizes.TryGetValue(fontSize,out fontSizeSet)){
				fontSizeSet=FontSizes[fontSize]=new FontSizeCharSet(fontSize,this);
			}
			
			return fontSizeSet.GetCharacter(character);
		}
		
		/// <summary>Prepares all text for an allocation by setting them as all being not in use.</summary>
		public void PrepareForAllocate(){
			foreach(KeyValuePair<int,FontSizeCharSet> kvp in FontSizes){
				kvp.Value.PrepareForAllocate();
			}
		}
		
		/// <summary>Requests all characters from the fonts atlas. This ensures they are available.</summary>
		public void PrepareForLayout(){
			foreach(KeyValuePair<int,FontSizeCharSet> kvp in FontSizes){
				kvp.Value.PrepareForLayout();
			}
		}
		
		/// <summary>When the font atlas rebuilds, all characters before the set that caused the rebuild must be re-requested.
		/// This method re-requests all sets up to the given one (which should be the one that caused a rebuild).</summary>
		/// <param name="stopAt">The set to stop rebuilding at (exclusive).</param>
		/// <returns>True if the stop occured in this set. No following sets need to be processed.</returns>
		public bool RebuildUpto(FontSizeCharSet stopAt){
			foreach(KeyValuePair<int,FontSizeCharSet> kvp in FontSizes){
				FontSizeCharSet fontSizeSet=kvp.Value;
				if(fontSizeSet==stopAt){
					return true;
				}
				fontSizeSet.RequestFromFont();
			}
			return false;
		}
		
		/// <summary>Called after PrepareForLayout to make sure any buffered characters are up-to-date.</summary>
		/// <param name="infoRequired">True if HasCharacterInfo must be false for a request to occur. Requested anyway if false.</param>
		public void RefreshBufferedCharacters(bool infoRequired){
			foreach(KeyValuePair<int,FontSizeCharSet> kvp in FontSizes){
				kvp.Value.RefreshBufferedCharacters(infoRequired);
			}
		}
		
	}
	
}